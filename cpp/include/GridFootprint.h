#ifndef CPPMORPIONSOLITAIRE_GRIDFOOTPRINT_H
#define CPPMORPIONSOLITAIRE_GRIDFOOTPRINT_H

#include "Grid.h"
#include "Point.h"
#include "Coordinates.h"

struct GridFootprint {
    Coordinates min, max;
//    int xMin, xMax, yMin, yMax;

    explicit GridFootprint(const Grid& grid);

    void add(Coordinates coord);
    void add(Point pt);
    void pad(int width);
};


#endif //CPPMORPIONSOLITAIRE_GRIDFOOTPRINT_H
