#include "../include/Grid.h"

Grid::Grid(char type, int length, bool disjoint)
    : length(length), disjoint(disjoint), initialDots(getInitialDots(type, length)) {}

void Grid::add(const GridMove& move)
{
    moves.push_back(move);
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

vector<GridPoint> Grid::cross4()
{
    return {
            GridPoint(30, 27), GridPoint(31, 27), GridPoint(32, 27), GridPoint(33, 27),
            GridPoint(30, 28), GridPoint(33, 28), GridPoint(30, 29), GridPoint(33, 29),
            GridPoint(27, 30), GridPoint(28, 30), GridPoint(29, 30), GridPoint(30, 30),
            GridPoint(33, 30), GridPoint(34, 30), GridPoint(35, 30), GridPoint(36, 30),
            GridPoint(27, 31), GridPoint(36, 31), GridPoint(27, 32), GridPoint(36, 32),
            GridPoint(27, 33), GridPoint(28, 33), GridPoint(29, 33), GridPoint(30, 33),
            GridPoint(33, 33), GridPoint(34, 33), GridPoint(35, 33), GridPoint(36, 33),
            GridPoint(30, 34), GridPoint(33, 34), GridPoint(30, 35), GridPoint(33, 35),
            GridPoint(30, 36), GridPoint(31, 36), GridPoint(32, 36), GridPoint(33, 36)
    };
}

vector<GridPoint> Grid::cross3()
{
    return {
            GridPoint(30, 28), GridPoint(31, 28), GridPoint(32, 28),
            GridPoint(30, 29), GridPoint(32, 29),
            GridPoint(28, 30), GridPoint(29, 30), GridPoint(30, 30),
            GridPoint(32, 30), GridPoint(33, 30), GridPoint(34, 30),
            GridPoint(28, 31), GridPoint(34, 31),
            GridPoint(28, 32), GridPoint(29, 32), GridPoint(30, 32),
            GridPoint(32, 32), GridPoint(33, 32), GridPoint(34, 32),
            GridPoint(30, 33), GridPoint(32, 33),
            GridPoint(30, 34), GridPoint(31, 34), GridPoint(32, 34)
    };
}

vector<GridPoint> Grid::pipe()
{
    return {
            GridPoint(30, 27), GridPoint(31, 27), GridPoint(32, 27), GridPoint(33, 27),
            GridPoint(29, 28), GridPoint(34, 28), GridPoint(28, 29), GridPoint(35, 29),
            GridPoint(27, 30), GridPoint(30, 30), GridPoint(31, 30), GridPoint(32, 30), GridPoint(33, 30), GridPoint(36, 30),
            GridPoint(27, 31), GridPoint(30, 31), GridPoint(33, 31), GridPoint(36, 31),
            GridPoint(27, 32), GridPoint(30, 32), GridPoint(33, 32), GridPoint(36, 32),
            GridPoint(27, 33), GridPoint(30, 33), GridPoint(31, 33), GridPoint(32, 33), GridPoint(33, 33), GridPoint(36, 33),
            GridPoint(28, 34), GridPoint(35, 34), GridPoint(29, 35), GridPoint(34, 35),
            GridPoint(30, 36), GridPoint(31, 36), GridPoint(32, 36), GridPoint(33, 36)
    };
}

vector<GridPoint> Grid::getInitialDots(char type, int length)
{
    if (type == 'c' && length == 4)
        return cross4();
    if (type == 'c' && length == 3)
        return cross3();
    if (type == 'p' && length == 4)
        return pipe();
    return {};
}