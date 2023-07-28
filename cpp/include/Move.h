#ifndef CPPMORPIONSOLITAIRE_MOVE_H
#define CPPMORPIONSOLITAIRE_MOVE_H

#include <array>
#include "Point.h"
#include "GridMove.h"
#include "ImageMove.h"

using std::array;

template <size_t length, bool disjoint>
struct Move : GridMove, ImageMove<length, disjoint>
{
    const array<Point, length> existingDots;

    Move(Point dot, Line line, const ImageMove<length, disjoint>& points, const array<Point, length>& existingDots)
            : GridMove(dot, line), ImageMove<length, disjoint>(points), existingDots(existingDots) {};
};


#endif //CPPMORPIONSOLITAIRE_MOVE_H
