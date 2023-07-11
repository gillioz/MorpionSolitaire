#ifndef CPPMORPIONSOLITAIRE_GRIDFOOTPRINT_H
#define CPPMORPIONSOLITAIRE_GRIDFOOTPRINT_H

#include "GridPoint.h"

class Grid;

struct GridFootprint {
    int xMin, xMax, yMin, yMax;

    explicit GridFootprint(const Grid& grid);

    void add(const Point& pt);
    GridPoint minCorner() const;
    GridPoint maxCorner() const;
};


#endif //CPPMORPIONSOLITAIRE_GRIDFOOTPRINT_H
