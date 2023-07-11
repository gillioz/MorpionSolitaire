#ifndef CPPMORPIONSOLITAIRE_GRAPHGAME_H
#define CPPMORPIONSOLITAIRE_GRAPHGAME_H

#include <string>
#include <vector>
#include "Game.h"
#include "Node.h"

using std::string, std::vector;

class GraphGame : public Game {
private:
    vector<Node> nodes;

    void buildGraph();
    static int randomInt(int max, int min = 0);

public:
    explicit GraphGame(char type = 'c', int length = 4, bool disjoint = false);

    int getScore() const override;
    int getNumberOfMoves() const;
    void play(const Move& move);
    void play(int index);
    bool tryPlay(const GridLine &line) override;
    bool tryPlay(const GridLine &line, const GridPoint& dot) override;
    void playAtRandom(int n);
    void playAtRandom();
    void playNestedMC(int level);
    void undo() override;
    void undo(int steps) override;
    void restart() override;
    void print() const override;
    void revertToScore(int score);
    void revertToRandomScore();

    static vector<int> repeatPlayAtRandom(int n, char type = 'c', int length = 4, bool disjoint = false);
};


#endif //CPPMORPIONSOLITAIRE_GRAPHGAME_H
