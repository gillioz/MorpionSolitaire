#include "../include/GridPoint.h"

ImagePoint GridPoint::toImagePoint(int offset)  const
{
    return {3 * x + offset, 3 * y + offset};
}