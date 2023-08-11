#ifndef CPPMORPIONSOLITAIRE_LINE_H
#define CPPMORPIONSOLITAIRE_LINE_H

#include "Point.h"


struct Line {
    const Point pt1;
    const Point pt2;

    Line(Point pt1, Point pt2);

    bool operator==(const Line& other) const;
};


#endif //CPPMORPIONSOLITAIRE_LINE_H
