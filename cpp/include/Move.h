#ifndef CPPMORPIONSOLITAIRE_MOVE_H
#define CPPMORPIONSOLITAIRE_MOVE_H

#include <vector>
#include "Point.h"
#include "GridMove.h"
#include "ImageMove.h"

using std::vector;

struct Move : GridMove, ImageMove
{
    const vector<Point> existingDots;

    Move(Point dot, Line line, const ImageMove& points, const vector<Point>& existingDots)
            : GridMove(dot, line), ImageMove(points), existingDots(existingDots) {};
};


#endif //CPPMORPIONSOLITAIRE_MOVE_H
