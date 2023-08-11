#ifndef CPPMORPIONSOLITAIRE_POINT_H
#define CPPMORPIONSOLITAIRE_POINT_H


//#include <cstdint>

const int IMAGESIZE = 3 * 64;

// Points are pairs of integers in the range (0, IMAGESIZE),
// stored for convenience into a single int
typedef int Point;

Point makePoint(int x, int y);
int getX(Point pt);
int getY(Point pt);

const int HORIZONTAL = makePoint(3, 0);
const int VERTICAL = makePoint(0, 3);
const int DIAGONAL1 = makePoint(3, 3);
const int DIAGONAL2 = makePoint(3, -3);

#endif //CPPMORPIONSOLITAIRE_POINT_H
