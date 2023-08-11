#include "../include/Coordinates.h"


Coordinates::Coordinates(int x, int y) : pair<int, int>(x, y) {}

Coordinates::Coordinates(Point pt)
    : pair<int, int>(getX(pt) / 3, getY(pt) / 3) {}

Point Coordinates::toPoint() const
{
    return makePoint(3 * first + 1, 3 * second + 1);
}

int Coordinates::x() const
{
    return first;
}

int Coordinates::y() const
{
    return second;
}