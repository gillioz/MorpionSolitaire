#include "../include/GridLine.h"

GridLine::GridLine(GridPoint pt1, GridPoint pt2) : pt1(Point::min(pt1, pt2)), pt2(Point::max(pt1, pt2)) {}

bool GridLine::operator==(const GridLine& other) const
{
    return pt1 == other.pt1 && pt2 == other.pt2;
}

int GridLine::width() const
{
    return pt2.x - pt1.x;
}

int GridLine::height() const
{
    return pt2.y - pt1.y;
}