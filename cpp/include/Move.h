#ifndef CPPMORPIONSOLITAIRE_MOVE_H
#define CPPMORPIONSOLITAIRE_MOVE_H

#include <vector>
#include "Point.h"
#include "GridMove.h"
#include "ImageMove.h"

using std::vector;

struct Move : GridMove, ImageMove
{
    const vector<Point> supportPoints;

    Move(Point dot, Line line, const ImageMove& points, const vector<Point>& supportPoints)
            : GridMove(dot, line), ImageMove(points), supportPoints(supportPoints) {};
};


#endif //CPPMORPIONSOLITAIRE_MOVE_H
