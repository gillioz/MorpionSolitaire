#ifndef CPPMORPIONSOLITAIRE_GRID_H
#define CPPMORPIONSOLITAIRE_GRID_H

#include <vector>
#include "Point.h"
#include "GridMove.h"

using std::vector;


struct Grid
{
    const int length;
    const bool disjoint;
    const vector<Point> initialDots;
    vector<GridMove> moves;

    explicit Grid(char type = 'e', int length = 4, bool disjoint = false);
    Grid(int length, bool disjoint, const vector<Point>& initialDots, const vector<GridMove>& moves = {});

    void add(const GridMove& move);
    void remove();
    void remove(int steps);
    int getScore() const;

    static vector<Point> getInitialDots(char type, int length);
    static vector<Point> cross3();
    static vector<Point> cross4();
    static vector<Point> pipe();

private:
    static Point makePointFromCoordinates(int x, int y);
};


#endif //CPPMORPIONSOLITAIRE_GRID_H
