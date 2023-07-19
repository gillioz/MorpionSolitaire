#ifndef CPPMORPIONSOLITAIRE_NODE_H
#define CPPMORPIONSOLITAIRE_NODE_H

#include <memory>
#include <optional>
#include <vector>
#include "Move.h"

using std::optional, std::vector;

struct Node {
    const optional<Move> root;
    const vector<Move> branches;

    explicit Node(const vector<Move> & branches) : root(), branches(branches) {};
    Node(const Move& root, vector<Move> branches) : root(root), branches(std::move(branches)) {};
};


#endif //CPPMORPIONSOLITAIRE_NODE_H
