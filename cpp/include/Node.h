#ifndef CPPMORPIONSOLITAIRE_NODE_H
#define CPPMORPIONSOLITAIRE_NODE_H

#include <memory>
#include <optional>
#include <vector>
#include "Move.h"

using std::optional, std::vector;

template <size_t length, bool disjoint>
struct Node {
    const optional<Move<length, disjoint>> root;
    const vector<Move<length, disjoint>> branches;

    explicit Node(const vector<Move<length, disjoint>> & branches) : root(), branches(branches) {};
    Node(const Move<length, disjoint>& root, vector<Move<length, disjoint>> branches)
        : root(root), branches(std::move(branches)) {};
};


#endif //CPPMORPIONSOLITAIRE_NODE_H
