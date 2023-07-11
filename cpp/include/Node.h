#ifndef CPPMORPIONSOLITAIRE_NODE_H
#define CPPMORPIONSOLITAIRE_NODE_H

#include <utility>
#include <vector>
#include "Move.h"

using std::vector;

struct Node {
    const int score;
    const Node* const parent;
    const Move* const root;
    const vector<Move> branches;

    explicit Node(const vector<Move> & branches) : score(0), parent(nullptr), root(nullptr), branches(branches) {};
    Node(const Node& parent, const Move& root, vector<Move> branches)
        : score(parent.score + 1), parent(&parent), root(&root), branches(std::move(branches)) {};
};


#endif //CPPMORPIONSOLITAIRE_NODE_H
