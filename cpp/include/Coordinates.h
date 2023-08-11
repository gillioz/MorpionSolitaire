#ifndef CPPMORPIONSOLITAIRE_COORDINATES_H
#define CPPMORPIONSOLITAIRE_COORDINATES_H


#include <utility>
#include "Point.h"

using std::pair;

struct Coordinates : public pair<int, int>
{
    Coordinates(int x, int y);
    explicit Coordinates(Point pt);

    int x() const;
    int y() const;

    Point toPoint() const;
};

#endif //CPPMORPIONSOLITAIRE_COORDINATES_H
