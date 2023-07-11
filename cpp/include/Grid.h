#ifndef CPPMORPIONSOLITAIRE_GRID_H
#define CPPMORPIONSOLITAIRE_GRID_H

#include <vector>
#include "GridPoint.h"
#include "GridMove.h"
#include "GridFootprint.h"

using std::vector;

struct Grid
{
    const int length;
    const bool disjoint;
    const vector<GridPoint> initialDots;
    vector<GridMove> moves;

    explicit Grid(char type = 'e', int length = 4, bool disjoint = false);

    void add(const GridMove& move);
    void remove();
    void remove(int steps);
    int getScore() const;

    static vector<GridPoint> getInitialDots(char type, int length);
    static vector<GridPoint> cross3();
    static vector<GridPoint> cross4();
    static vector<GridPoint> pipe();
};


#endif //CPPMORPIONSOLITAIRE_GRID_H
