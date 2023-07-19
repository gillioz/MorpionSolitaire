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

class Game {
protected:
    Grid grid;
    Image image;

    void buildImage();
    void tryAddMoveToList(const Line& line, vector<Move>& listOfMoves) const;

    const int HORIZONTAL = makePoint(3, 0);
    const int VERTICAL = makePoint(0, 3);
    const int DIAGONAL1 = makePoint(3, 3);
    const int DIAGONAL2 = makePoint(3, -3);

public:
    explicit Game(char type = 'c', int length = 4, bool disjoint = false, bool build = true);

    optional<Move> tryBuildMove(const Line& line) const;
    optional<Move> tryBuildMove(const Line& line, Point dot) const;
    bool isValidMove(const Move& move) const;
    void applyMove(const Move& move);
    virtual bool tryPlay(const Line& line);
    virtual bool tryPlay(const Line& line, Point dot);
    vector<Move> findAllMoves() const;
    vector<Move> findNewMoves(Point dot) const;
    virtual void undo();
    virtual void undo(int steps);
    virtual void restart();
    virtual int getScore() const;
    virtual void print() const;
};


#endif //CPPMORPIONSOLITAIRE_GAME_H
