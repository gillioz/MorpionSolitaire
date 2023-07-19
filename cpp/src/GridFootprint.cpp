#include "../include/GridFootprint.h"

GridFootprint::GridFootprint(const Grid& grid) : min(Point(IMAGESIZE / 2)), max(Point(IMAGESIZE / 2))
{
    if (grid.initialDots.empty())
        return;

    min = Coordinates(grid.initialDots[0]);
    max = min;

    for (Point pt: grid.initialDots)
        add(pt);
    for (const GridMove& move: grid.moves)
    {
        add(move.line.pt1);
        add(move.line.pt2);
    }
}

void GridFootprint::add(Coordinates coord)
{
    if (coord.x() < min.x())
        min.first = coord.x();
    else if (coord.x() > max.x())
        max.first = coord.x();

    if (coord.y() < min.y())
        min.second = coord.y();
    else if (coord.y() > max.y())
        max.second = coord.y();
}

void GridFootprint::add(Point pt)
{
    add(Coordinates(pt));
}

void GridFootprint::pad(int width)
{
    min.first -= width;
    min.second -= width;
    max.first += width;
    max.second += width;
}