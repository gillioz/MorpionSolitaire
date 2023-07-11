#include "../include/GridFootprint.h"
#include "../include/Grid.h"

GridFootprint::GridFootprint(const Grid& grid)
{
    if (grid.initialDots.empty())
    {
        xMin = 32;
        yMin = 32;
        xMax = 32;
        yMax = 32;
        return;
    }
    GridPoint startingPoint = grid.initialDots[0];
    xMin = startingPoint.x;
    yMin = startingPoint.y;
    xMax = startingPoint.x;
    yMax = startingPoint.y;

    for (const GridPoint& pt: grid.initialDots)
        add(pt);
    for (const GridMove& move: grid.moves)
    {
        add(move.line.pt1);
        add(move.line.pt2);
    }
}

void GridFootprint::add(const Point& pt)
{
    if (pt.x < xMin)
        xMin = pt.x;
    else if (pt.x > xMax)
        xMax = pt.x;

    if (pt.y < yMin)
        yMin = pt.y;
    else if (pt.y > yMax)
        yMax = pt.y;
}

GridPoint GridFootprint::minCorner() const
{
    return {xMin, yMin};
}

GridPoint GridFootprint::maxCorner() const
{
    return {xMax, yMax};
}