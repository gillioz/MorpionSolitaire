#ifndef CPPMORPIONSOLITAIRE_GRIDMOVE_H
#define CPPMORPIONSOLITAIRE_GRIDMOVE_H

#include "Point.h"
#include "Line.h"

struct GridMove {
    const Point dot;
    const Line line;

    GridMove(Point dot, Line line) : dot(dot), line(line) {};
};


#endif //CPPMORPIONSOLITAIRE_GRIDMOVE_H
