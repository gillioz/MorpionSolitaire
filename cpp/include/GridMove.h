#ifndef CPPMORPIONSOLITAIRE_GRIDMOVE_H
#define CPPMORPIONSOLITAIRE_GRIDMOVE_H

#include <utility>

#include "GridPoint.h"
#include "GridLine.h"


struct GridMove {
    const GridPoint dot;
    const GridLine line;

    GridMove(GridPoint dot, GridLine line) : dot(dot), line(std::move(line)) {};
};


#endif //CPPMORPIONSOLITAIRE_GRIDMOVE_H
