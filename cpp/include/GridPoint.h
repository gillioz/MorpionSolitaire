#ifndef CPPMORPIONSOLITAIRE_GRIDPOINT_H
#define CPPMORPIONSOLITAIRE_GRIDPOINT_H

#include "Point.h"
#include "ImagePoint.h"

struct ImagePoint;

struct GridPoint : Point {
    explicit GridPoint(Point pt) : Point(pt) {};
    GridPoint(int x, int y) : Point(x, y) {};

    ImagePoint toImagePoint(int offset = 1) const;
};


#endif //CPPMORPIONSOLITAIRE_GRIDPOINT_H
