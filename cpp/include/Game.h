#ifndef CPPMORPIONSOLITAIRE_GAME_H
#define CPPMORPIONSOLITAIRE_GAME_H

#include <optional>
#include <vector>
#include "Grid.h"
#include "Image.h"
#include "Line.h"
#include "Move.h"
#include "Point.h"

using std::optional, std::vector;

template <size_t length, bool disjoint>
class Game {
protected:
    Grid grid;
    Image<length, disjoint> image;

    void buildImage();
    void tryAddMoveToList(const Line& line, vector<Move<length, disjoint>>& listOfMoves) const;

public:
    explicit Game(char type = 'c', bool build = true);

    optional<Move<length, disjoint>> tryBuildMove(const Line& line) const;
    optional<Move<length, disjoint>> tryBuildMove(const Line& line, Point dot) const;
    bool isValidMove(const Move<length, disjoint>& move) const;
    void applyMove(const Move<length, disjoint>& move);
    virtual bool tryPlay(const Line& line);
    virtual bool tryPlay(const Line& line, Point dot);
    vector<Move<length, disjoint>> findAllMoves() const;
    vector<Move<length, disjoint>> findNewMoves(Point dot) const;
    virtual void undo();
    virtual void undo(int steps);
    virtual void restart();
    virtual int getScore() const;
    virtual void print() const;
};


#endif //CPPMORPIONSOLITAIRE_GAME_H
