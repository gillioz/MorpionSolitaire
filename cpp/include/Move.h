#ifndef CPPMORPIONSOLITAIRE_MOVE_H
#define CPPMORPIONSOLITAIRE_MOVE_H

#include <vector>
#include "ImagePoint.h"
#include "GridMove.h"
#include "ImageMove.h"

using std::vector;

struct Move : GridMove, ImageMove
{
    const vector<ImagePoint> supportPoints;

    Move(GridPoint dot, GridLine line, const vector<ImagePoint> &points, const vector<ImagePoint> &supportPoints)
            : GridMove(dot, line), ImageMove(points), supportPoints(supportPoints) {};
};


#endif //CPPMORPIONSOLITAIRE_MOVE_H
