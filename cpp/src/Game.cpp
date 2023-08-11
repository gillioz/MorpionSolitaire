#include "../include/Game.h"
#include "../include/Coordinates.h"
#include "../include/GridFootprint.h"
#include "../include/GridDTO.h"
#include <iostream>

using std::cout, std::endl;
using std::string, std::ifstream, std::ofstream, std::ostringstream;

template <size_t length, bool disjoint>
Game<length, disjoint>::Game(char type, bool build) : grid(type, length, disjoint)
{
    if (build)
        buildImage();
}

template <size_t length, bool disjoint>
Game<length, disjoint>::Game(const Grid& grid, bool build) : grid(grid)
{
    if (grid.length != length || grid.disjoint != disjoint)
        throw std::logic_error("Cannot create game from a grid with incompatible properties");

    if (build)
        buildImage();
}

template <size_t length, bool disjoint>
void Game<length, disjoint>::buildImage()
{
    image.clear();

    // add initial points
    for (Point dot: grid.initialDots)
        image.value[dot] = true;

    // add moves successively
    for (const GridMove& gridMove: grid.moves)
    {
        optional<Move<length, disjoint>> move = tryBuildMove(gridMove.line);
        if (!move.has_value())
            throw std::logic_error("Trying to load a grid with an invalid segment");
        image.apply(*move);
    }
}

template <size_t length, bool disjoint>
optional<Move<length, disjoint>> Game<length, disjoint>::tryBuildMove(const Line& line) const
{
    return image.tryBuildMove(line);
}

template <size_t length, bool disjoint>
optional<Move<length, disjoint>> Game<length, disjoint>::tryBuildMove(const Line& line, Point dot) const
{
    optional<Move<length, disjoint>> move = tryBuildMove(line);
    if (!move.has_value() || move->dot != dot)
        return {};
    return move;
}

template <size_t length, bool disjoint>
bool Game<length, disjoint>::isValidMove(const Move<length, disjoint>& move) const
{
    return image.isValidMove(move);
}

template <size_t length, bool disjoint>
bool Game<length, disjoint>::isStillValidMove(const Move<length, disjoint>& move) const
{
    return image.isStillValidMove(move);
}

template <size_t length, bool disjoint>
void Game<length, disjoint>::applyMove(const Move<length, disjoint>& move)
{
    grid.add(move);
    image.apply(move);
}

template <size_t length, bool disjoint>
bool Game<length, disjoint>::tryPlay(const Line& line)
{
    optional<Move<length, disjoint>> move = tryBuildMove(line);

    if (!move.has_value())
        return false;

    applyMove(*move);
    return true;
}

template <size_t length, bool disjoint>
bool Game<length, disjoint>::tryPlay(const Line& line, Point dot)
{
    optional<Move<length, disjoint>> move = tryBuildMove(line, dot);

    if (!move.has_value())
        return false;

    applyMove(*move);
    return true;
}

template <size_t length, bool disjoint>
void Game<length, disjoint>::tryAddMoveToList(const Line& line, vector<Move<length, disjoint>>& listOfMoves) const
{
    optional<Move<length, disjoint>> move = tryBuildMove(line);
    if (move.has_value())
        listOfMoves.push_back(move.value());
}

template <size_t length, bool disjoint>
vector<Move<length, disjoint>> Game<length, disjoint>::findAllMoves() const
{
    GridFootprint footprint(grid);

    vector<Move<length, disjoint>> result;

    for (int x = footprint.min.x(); x <= footprint.max.x(); x++)
        for (int y = footprint.min.y() - 1; y <= footprint.max.y() - grid.length + 1; y++)
        {
            Point pt = Coordinates(x, y).toPoint();
            tryAddMoveToList({pt, pt + grid.length * VERTICAL}, result);
        }

    for (int x = footprint.min.x() - 1; x <= footprint.max.x() - grid.length + 1; x++)
        for (int y = footprint.min.y(); y <= footprint.max.y(); y++)
        {
            Point pt = Coordinates(x, y).toPoint();
            tryAddMoveToList({pt, pt + grid.length * HORIZONTAL}, result);
        }

    for (int x = footprint.min.x() - 1; x <= footprint.max.x() - grid.length + 1; x++)
        for (int y = footprint.min.y() - 1; y <= footprint.max.y() - grid.length + 1; y++)
        {
            Point pt = Coordinates(x, y).toPoint();
            tryAddMoveToList({pt, pt + grid.length * DIAGONAL1}, result);
        }

    for (int x = footprint.min.x() - 1; x <= footprint.max.x() - grid.length + 1; x++)
        for (int y = footprint.min.y() + grid.length - 1; y <= footprint.max.y() + 1; y++) {
            Point pt = Coordinates(x, y).toPoint();
            tryAddMoveToList({pt, pt + grid.length * DIAGONAL2}, result);
        }

    return result;
}

template <size_t length, bool disjoint>
vector<Move<length, disjoint>> Game<length, disjoint>::findNewMoves(Point dot) const
{
    vector<Move<length, disjoint>> result;

    for (int i1 = 0; i1 <= grid.length; i1++)
    {
        int i2 = i1 - grid.length;
        tryAddMoveToList({dot + i1 * HORIZONTAL, dot + i2 * HORIZONTAL}, result);
        tryAddMoveToList({dot + i1 * VERTICAL, dot + i2 * VERTICAL}, result);
        tryAddMoveToList({dot + i1 * DIAGONAL1, dot + i2 * DIAGONAL1}, result);
        tryAddMoveToList({dot + i1 * DIAGONAL2, dot + i2 * DIAGONAL2}, result);
    }

    return result;
}

template <size_t length, bool disjoint>
void Game<length, disjoint>::undo()
{
    grid.remove();
    buildImage();
}

template <size_t length, bool disjoint>
void Game<length, disjoint>::undo(int steps)
{
    grid.remove(steps);
    buildImage();
}

template <size_t length, bool disjoint>
void Game<length, disjoint>::restart()
{
    undo(getScore());
}

template <size_t length, bool disjoint>
int Game<length, disjoint>::getScore() const
{
    return grid.getScore();
}

template <size_t length, bool disjoint>
void Game<length, disjoint>::print() const
{
    GridFootprint footprint(grid);
    footprint.pad(2);
    Point min = footprint.min.toPoint();
    Point max = footprint.max.toPoint();

    image.print(getX(min), getX(max), getY(min), getY(max));
    cout << "Score: " << getScore() << endl;
}

template <size_t length, bool disjoint>
string Game<length, disjoint>::exportJSON() const
{
    GridDTO dto(grid);
    return dto.toJSON();
}

template <size_t length, bool disjoint>
Game<length, disjoint> Game<length, disjoint>::importJSON(const string& json)
{
    GridDTO dto(json);
    return Game<length, disjoint>(dto.toGrid());
}

template class Game <4, false>;
template class Game <4, true>;
template class Game <3, false>;
template class Game <3, true>;