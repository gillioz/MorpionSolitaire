#include "../include/Line.h"
#include <algorithm>

Line::Line(Point pt1, Point pt2) : pt1(std::min(pt1, pt2)), pt2(std::max(pt1, pt2)) {}

bool Line::operator==(const Line& other) const
{
    return pt1 == other.pt1 && pt2 == other.pt2;
}