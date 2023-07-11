//
// Created by marc on 6/30/23.
//

#ifndef CPPMORPIONSOLITAIRE_IMAGEMOVE_H
#define CPPMORPIONSOLITAIRE_IMAGEMOVE_H

#include <vector>
#include "ImagePoint.h"

using std::vector;


struct ImageMove {
    const vector<ImagePoint> points;

    explicit ImageMove(const vector<ImagePoint>& points) : points(points) {};
};


#endif //CPPMORPIONSOLITAIRE_IMAGEMOVE_H
