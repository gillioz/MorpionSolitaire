#ifndef CPPMORPIONSOLITAIRE_GAME_H
#define CPPMORPIONSOLITAIRE_GAME_H

#include <optional>
#include <vector>
#include "Grid.h"
#include "Image.h"
#include "GridLine.h"
#include "Move.h"

using std::optional, std::vector;

class Game {
protected:
    Grid grid;
    Image image;

    void buildImage();
    void tryAddMoveToList(const GridLine& line, vector<Move>& listOfMoves) const;

public:
    explicit Game(char type = 'c', int length = 4, bool disjoint = false, bool build = true);

    optional<Move> tryBuildMove(const GridLine& line) const;
    optional<Move> tryBuildMove(const GridLine& line, const GridPoint& dot) const;
    bool isValidMove(const Move& move) const;
    void applyMove(const Move& move);
    virtual bool tryPlay(const GridLine& line);
    virtual bool tryPlay(const GridLine& line, const GridPoint& dot);
    vector<Move> findAllMoves() const;
    vector<Move> findNewMoves(const GridPoint& dot) const;
    virtual void undo();
    virtual void undo(int steps);
    virtual void restart();
    virtual int getScore() const;
    virtual void print() const;
};


#endif //CPPMORPIONSOLITAIRE_GAME_H
