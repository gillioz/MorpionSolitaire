#include "../include/ImagePoint.h"


GridPoint ImagePoint::toGridPoint() const
{
    return {x /3, y /3};
}

bool ImagePoint::isDot(){
    return this->x % 3 == 1 && this->y % 3 == 1;
}