#ifndef CPPMORPIONSOLITAIRE_IMAGE_H
#define CPPMORPIONSOLITAIRE_IMAGE_H

#include <optional>
#include "Point.h"
#include "ImageMove.h"
#include "Move.h"
#include "Line.h"
#include "Grid.h"

using std::optional;

template <size_t length, bool disjoint>
struct Image {
    bool value[IMAGESIZE * IMAGESIZE] = {false };

    void clear();
    optional<Move<length, disjoint>> tryBuildMove(const Line& line) const;
    bool isValidMove(const ImageMove<length, disjoint>& move) const;
    bool isValidMove(const Move<length, disjoint>& move) const;
    void apply(const ImageMove<length, disjoint>& move, bool value = true);
    void print(int xMin = 0, int xMax = IMAGESIZE, int yMin = 0, int yMax = IMAGESIZE) const;
};


#endif //CPPMORPIONSOLITAIRE_IMAGE_H
