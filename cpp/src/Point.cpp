#include "../include/Point.h"

bool Point::operator==(const Point& other) const
{
    return (x == other.x) && (y == other.y);
}

bool Point::operator!=(const Point& other) const
{
    return !operator==(other);
}

bool Point::operator<(const Point &other) const
{
    return x < other.x || (x == other.x && y < other.y);
}

bool Point::operator>(const Point &other) const
{
    return x > other.x || (x == other.x && y > other.y);
}

Point Point::min(const Point &pt1, const Point &pt2)
{
    if (pt1 < pt2)
        return pt1;
    return pt2;
}

Point Point::max(const Point &pt1, const Point &pt2)
{
    if (pt1 < pt2)
        return pt2;
    return pt1;
}