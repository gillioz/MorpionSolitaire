#include "../include/Game.h"
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
    for (const GridPoint& dot: grid.initialDots)
        image.set(dot.toImagePoint(), true);

    // add moves successively
    for (const GridMove& gridMove: grid.moves)
    {
        Move* move = tryBuildMove(gridMove.line);
        if (move == nullptr)
            throw string("Trying to load a grid with an invalid segment");
        image.apply(*move);
        delete move;
    }
}

Move* Game::tryBuildMove(const GridLine& line) const
{
    return image.tryBuildMove(line, grid.length, grid.disjoint);
}

Move* Game::tryBuildMove(const GridLine& line, const GridPoint& dot) const
{
    Move* move = tryBuildMove(line);
    if (move == nullptr || move->dot != dot)
        return nullptr;
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

bool Game::tryPlay(const GridLine& line)
{
    Move* move = tryBuildMove(line);

    if (move == nullptr)
        return false;

    applyMove(*move);
    delete move;
    return true;
}

bool Game::tryPlay(const GridLine& line, const GridPoint &dot)
{
    Move* move = tryBuildMove(line, dot);

    if (move == nullptr)
        return false;

    applyMove(*move);
    delete move;
    return true;
}

void Game::tryAddMoveToList(const GridLine& line, vector<Move>& listOfMoves) const
{
    Move* move = tryBuildMove(line);
    if (move != nullptr)
        listOfMoves.push_back(*move);
    delete move;
}

vector<Move> Game::findAllMoves() const
{
    GridFootprint footprint(grid);
    vector<Move> result;

    for (int x = footprint.xMin; x <= footprint.xMax; x++)
        for (int y = footprint.yMin - 1; y <= footprint.yMax - grid.length + 1; y++)
            tryAddMoveToList({{x, y}, {x, y + grid.length}}, result);

    for (int x = footprint.xMin - 1; x <= footprint.xMax - grid.length + 1; x++)
        for (int y = footprint.yMin; y <= footprint.yMax; y++)
            tryAddMoveToList({{x, y}, {x + grid.length, y}}, result);

    for (int x = footprint.xMin - 1; x <= footprint.xMax - grid.length + 1; x++)
        for (int y = footprint.yMin - 1; y <= footprint.yMax - grid.length + 1; y++)
        {
            tryAddMoveToList({{x, y}, {x + grid.length, y + grid.length}}, result);
            tryAddMoveToList({{x + grid.length, y}, {x, y + grid.length}}, result);
        }

    return result;
}

vector<Move> Game::findNewMoves(const GridPoint &dot) const
{
    vector<Move> result;

    for (int d1 = 0; d1 <= grid.length; d1++)
    {
        int d2 = d1 - grid.length;
        tryAddMoveToList({{dot.x, dot.y + d1}, {dot.x, dot.y + d2}}, result);
        tryAddMoveToList({{dot.x + d1, dot.y}, {dot.x + d2, dot.y}}, result);
        tryAddMoveToList({{dot.x + d1, dot.y + d1}, {dot.x + d2, dot.y + d2}}, result);
        tryAddMoveToList({{dot.x + d1, dot.y - d1}, {dot.x + d2, dot.y - d2}}, result);
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
    ImagePoint minCorner = footprint.minCorner().toImagePoint(-3);
    ImagePoint maxCorner = footprint.maxCorner().toImagePoint(6);
    image.print(minCorner.x, maxCorner.x, minCorner.y, maxCorner.y);
    cout << "Score: " << getScore() << endl;
}