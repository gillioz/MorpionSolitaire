#ifndef CPPMORPIONSOLITAIRE_IMAGEMOVE_H
#define CPPMORPIONSOLITAIRE_IMAGEMOVE_H

#include <array>
#include "Point.h"

template <size_t length, bool disjoint>
using ImageMove = std::array<Point, disjoint ? 2 * length + 3 : 2 * length + 1>;

#endif //CPPMORPIONSOLITAIRE_IMAGEMOVE_H
