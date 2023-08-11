#ifndef CPPMORPIONSOLITAIRE_GRAPHGAME_H
#define CPPMORPIONSOLITAIRE_GRAPHGAME_H

#include <string>
#include <vector>
#include "Game.h"
#include "Node.h"

using std::string, std::vector;

template <size_t length, bool disjoint>
class GraphGame : public Game<length, disjoint> {
private:
    vector<Node<length, disjoint>> nodes;

    void buildGraph();
    void addNode(const Move<length, disjoint>& move);
    vector<Move<length, disjoint>> getSequenceOfMoves(int score = 0);
    void playNestedMC(int level, vector<Move<length, disjoint>> & bestBranch);
    static int randomInt(int max, int min = 0);

public:
    explicit GraphGame(char type = 'c');
    explicit GraphGame(const Grid& grid);

    int getScore() const override;
    int getNumberOfMoves() const;
    void play(const Move<length, disjoint>& move);
    void play(int index);
    optional<Move<length, disjoint>> findMove(const Line& line) const;
    optional<Move<length, disjoint>> findMove(const Line& line, Point dot) const;
    bool tryPlay(const Line& line) override;
    bool tryPlay(const Line& line, Point dot) override;
    void playAtRandom(int n);
    void playAtRandom();
    void playNestedMC(int level);
    void undo() override;
    void undo(int steps) override;
    void restart() override;
    void print() const override;
    void revertToScore(int score);
    void revertToRandomScore();

    static GraphGame<length, disjoint> importJSON(const string& json);
    static vector<int> repeatPlayAtRandom(int n, char type = 'c');
};


#endif //CPPMORPIONSOLITAIRE_GRAPHGAME_H
