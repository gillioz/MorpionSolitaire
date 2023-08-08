#include "../include/Grid.h"
#include "../include/Coordinates.h"

Grid::Grid(char type, int length, bool disjoint)
    : length(length), disjoint(disjoint), initialDots(getInitialDots(type, length)) {}

Grid::Grid(int length, bool disjoint, const vector<Point>& initialDots, const vector<GridMove>& moves)
    : length(length), disjoint(disjoint), initialDots(initialDots), moves(moves) {}

void Grid::add(const GridMove& move)
{
    moves.emplace_back(move);
}

void Grid::remove()
{
    moves.pop_back();
}

void Grid::remove(int steps)
{
    while (steps > 0 && !moves.empty())
    {
        moves.pop_back();
        steps--;
    }
}

int Grid::getScore() const
{
    return (int)moves.size();
}

Point Grid::makePointFromCoordinates(int x, int y)
{
    return Coordinates(x, y).toPoint();
}

vector<Point> Grid::cross4()
{
    return {
            makePointFromCoordinates(30, 27), makePointFromCoordinates(31, 27), makePointFromCoordinates(32, 27), makePointFromCoordinates(33, 27),
            makePointFromCoordinates(30, 28), makePointFromCoordinates(33, 28), makePointFromCoordinates(30, 29), makePointFromCoordinates(33, 29),
            makePointFromCoordinates(27, 30), makePointFromCoordinates(28, 30), makePointFromCoordinates(29, 30), makePointFromCoordinates(30, 30),
            makePointFromCoordinates(33, 30), makePointFromCoordinates(34, 30), makePointFromCoordinates(35, 30), makePointFromCoordinates(36, 30),
            makePointFromCoordinates(27, 31), makePointFromCoordinates(36, 31), makePointFromCoordinates(27, 32), makePointFromCoordinates(36, 32),
            makePointFromCoordinates(27, 33), makePointFromCoordinates(28, 33), makePointFromCoordinates(29, 33), makePointFromCoordinates(30, 33),
            makePointFromCoordinates(33, 33), makePointFromCoordinates(34, 33), makePointFromCoordinates(35, 33), makePointFromCoordinates(36, 33),
            makePointFromCoordinates(30, 34), makePointFromCoordinates(33, 34), makePointFromCoordinates(30, 35), makePointFromCoordinates(33, 35),
            makePointFromCoordinates(30, 36), makePointFromCoordinates(31, 36), makePointFromCoordinates(32, 36), makePointFromCoordinates(33, 36)
    };
}

vector<Point> Grid::cross3()
{
    return {
            makePointFromCoordinates(30, 28), makePointFromCoordinates(31, 28), makePointFromCoordinates(32, 28),
            makePointFromCoordinates(30, 29), makePointFromCoordinates(32, 29),
            makePointFromCoordinates(28, 30), makePointFromCoordinates(29, 30), makePointFromCoordinates(30, 30),
            makePointFromCoordinates(32, 30), makePointFromCoordinates(33, 30), makePointFromCoordinates(34, 30),
            makePointFromCoordinates(28, 31), makePointFromCoordinates(34, 31),
            makePointFromCoordinates(28, 32), makePointFromCoordinates(29, 32), makePointFromCoordinates(30, 32),
            makePointFromCoordinates(32, 32), makePointFromCoordinates(33, 32), makePointFromCoordinates(34, 32),
            makePointFromCoordinates(30, 33), makePointFromCoordinates(32, 33),
            makePointFromCoordinates(30, 34), makePointFromCoordinates(31, 34), makePointFromCoordinates(32, 34)
    };
}

vector<Point> Grid::pipe()
{
    return {
            makePointFromCoordinates(30, 27), makePointFromCoordinates(31, 27), makePointFromCoordinates(32, 27), makePointFromCoordinates(33, 27),
            makePointFromCoordinates(29, 28), makePointFromCoordinates(34, 28), makePointFromCoordinates(28, 29), makePointFromCoordinates(35, 29),
            makePointFromCoordinates(27, 30), makePointFromCoordinates(30, 30), makePointFromCoordinates(31, 30), makePointFromCoordinates(32, 30),
            makePointFromCoordinates(33, 30),
            makePointFromCoordinates(36, 30),
            makePointFromCoordinates(27, 31), makePointFromCoordinates(30, 31), makePointFromCoordinates(33, 31), makePointFromCoordinates(36, 31),
            makePointFromCoordinates(27, 32), makePointFromCoordinates(30, 32), makePointFromCoordinates(33, 32), makePointFromCoordinates(36, 32),
            makePointFromCoordinates(27, 33), makePointFromCoordinates(30, 33), makePointFromCoordinates(31, 33), makePointFromCoordinates(32, 33),
            makePointFromCoordinates(33, 33),
            makePointFromCoordinates(36, 33),
            makePointFromCoordinates(28, 34), makePointFromCoordinates(35, 34), makePointFromCoordinates(29, 35), makePointFromCoordinates(34, 35),
            makePointFromCoordinates(30, 36), makePointFromCoordinates(31, 36), makePointFromCoordinates(32, 36), makePointFromCoordinates(33, 36)
    };
}

vector<Point> Grid::getInitialDots(char type, int length)
{
    if (type == 'c' && length == 4)
        return cross4();
    if (type == 'c' && length == 3)
        return cross3();
    if (type == 'p' && length == 4)
        return pipe();
    return {};
}