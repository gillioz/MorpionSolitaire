#ifndef CPPMORPIONSOLITAIRE_GRIDLINE_H
#define CPPMORPIONSOLITAIRE_GRIDLINE_H

#include "GridPoint.h"


struct GridLine {
    const GridPoint pt1;
    const GridPoint pt2;

    GridLine(GridPoint pt1, GridPoint pt2);

    bool operator==(const GridLine& other) const;
    int width() const;
    int height() const;
};


#endif //CPPMORPIONSOLITAIRE_GRIDLINE_H
