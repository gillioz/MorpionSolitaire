#ifndef CPPMORPIONSOLITAIRE_IMAGE_H
#define CPPMORPIONSOLITAIRE_IMAGE_H

#include <optional>
#include "Point.h"
#include "ImageMove.h"
#include "Move.h"
#include "Line.h"
#include "Grid.h"

using std::optional;

struct Image {
    bool value[IMAGESIZE * IMAGESIZE] = {false };

    void clear();
    optional<Move> tryBuildMove(const Line& line, int length, bool disjoint) const;
    bool isValidMove(const ImageMove& move) const;
    bool isValidMove(const Move& move) const;
    void apply(const ImageMove& move, bool value = true);
    void print(int xMin = 0, int xMax = IMAGESIZE, int yMin = 0, int yMax = IMAGESIZE) const;
};


#endif //CPPMORPIONSOLITAIRE_IMAGE_H
