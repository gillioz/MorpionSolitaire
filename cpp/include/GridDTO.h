#ifndef CPPMORPIONSOLITAIRE_GRIDDTO_H
#define CPPMORPIONSOLITAIRE_GRIDDTO_H

#include <array>
#include <string>
#include <vector>
#include "Grid.h"

using std::array, std::string, std::vector;


struct GridDTO
{
    int length;
    bool disjoint;
    vector<array<int, 2>> initialDots;
    vector<array<int, 6>> moves;

    explicit GridDTO(const Grid& grid);
    explicit GridDTO(const string& json);
    Grid toGrid() const;
    string toJSON() const;

    static const string TITLE;
    static const string VERSION;
};

#endif //CPPMORPIONSOLITAIRE_GRIDDTO_H
