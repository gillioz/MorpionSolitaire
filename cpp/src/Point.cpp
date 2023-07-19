#include "../include/Point.h"

Point makePoint(int x, int y)
{
    return IMAGESIZE * x + y;
}

int getX(Point pt)
{
    return pt / IMAGESIZE;
}

int getY(Point pt)
{
    return pt % IMAGESIZE;
}