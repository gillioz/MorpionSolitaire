#ifndef CPPMORPIONSOLITAIRE_IMAGEPOINT_H
#define CPPMORPIONSOLITAIRE_IMAGEPOINT_H

#include "Point.h"
#include "GridPoint.h"

struct GridPoint;

struct ImagePoint : Point {
    ImagePoint(int x, int y) : Point(x, y) {};

    GridPoint toGridPoint() const;
    bool isDot();
};


#endif //CPPMORPIONSOLITAIRE_IMAGEPOINT_H
