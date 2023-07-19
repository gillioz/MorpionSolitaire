#include "../include/Game.h"
#include "../include/Coordinates.h"
#include "../include/GridFootprint.h"
#include <iostream>
#include <utility>

using std::cout, std::endl;
using std::string;

Game::Game(char type, int length, bool disjoint, bool build) : grid(type, length, disjoint)
{
    if (build)
        buildImage();
}

void Game::buildImage()
{
    image.clear();

    // add initial points
    for (Point dot: grid.initialDots)
        image.value[dot] = true;

    // add moves successively
    for (const GridMove& gridMove: grid.moves)
    {
        optional<Move> move = tryBuildMove(gridMove.line);
        if (!move.has_value())
            throw std::logic_error("Trying to load a grid with an invalid segment");
        image.apply(move.value());
    }
}

optional<Move> Game::tryBuildMove(const Line& line) const
{
    return image.tryBuildMove(line, grid.length, grid.disjoint);
}

optional<Move> Game::tryBuildMove(const Line& line, Point dot) const
{
    optional<Move> move = tryBuildMove(line);
    if (!move.has_value() || move->dot != dot)
        return {};
    return move;
}

bool Game::isValidMove(const Move& move) const
{
    return image.isValidMove(move);
}

void Game::applyMove(const Move& move)
{
    grid.add(move);
    image.apply(move);
}

bool Game::tryPlay(const Line& line)
{
    optional<Move> move = tryBuildMove(line);

    if (!move.has_value())
        return false;

    applyMove(move.value());
    return true;
}

bool Game::tryPlay(const Line& line, Point dot)
{
    optional<Move> move = tryBuildMove(line, dot);

    if (!move.has_value())
        return false;

    applyMove(move.value());
    return true;
}

void Game::tryAddMoveToList(const Line& line, vector<Move>& listOfMoves) const
{
    optional<Move> move = tryBuildMove(line);
    if (move.has_value())
        listOfMoves.push_back(move.value());
}

vector<Move> Game::findAllMoves() const
{
    GridFootprint footprint(grid);

    vector<Move> result;

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

vector<Move> Game::findNewMoves(Point dot) const
{
    vector<Move> result;

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

void Game::undo()
{
    grid.remove();
    buildImage();
}

void Game::undo(int steps)
{
    grid.remove(steps);
    buildImage();
}

void Game::restart()
{
    undo(getScore());
}

int Game::getScore() const
{
    return grid.getScore();
}

void Game::print() const
{
    GridFootprint footprint(grid);
    footprint.pad(2);
    Point min = footprint.min.toPoint();
    Point max = footprint.max.toPoint();

    image.print(getX(min), getX(max), getY(min), getY(max));
    cout << "Score: " << getScore() << endl;
}