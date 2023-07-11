#ifndef CPPMORPIONSOLITAIRE_IMAGE_H
#define CPPMORPIONSOLITAIRE_IMAGE_H

#include <optional>
#include "ImagePoint.h"
#include "ImageMove.h"
#include "Move.h"
#include "GridLine.h"
#include "Grid.h"

using std::optional;

struct Image {
    static const int IMAGE_SIZE = 3 * 64;

    bool image[IMAGE_SIZE][IMAGE_SIZE] = {false };

    void clear();
    bool get(ImagePoint pt) const;
    void set(ImagePoint pt, bool value);
    optional<Move> tryBuildMove(const GridLine& line, int length, bool disjoint) const;
    bool isValidMove(const ImageMove& move) const;
    bool isValidMove(const Move& move) const;
    void apply(const ImageMove& move, bool value = true);
    void print(int xMin = 0, int xMax = IMAGE_SIZE, int yMin = 0, int yMax = IMAGE_SIZE) const;
};



#endif //CPPMORPIONSOLITAIRE_IMAGE_H
