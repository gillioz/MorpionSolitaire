#ifndef CPPMORPIONSOLITAIRE_NODE_H
#define CPPMORPIONSOLITAIRE_NODE_H

#include <memory>
#include <utility>
#include <vector>
#include "Move.h"

using std::shared_ptr, std::vector;

struct Node {
    const shared_ptr<const Move> root;
    const vector<Move> branches;

    explicit Node(const vector<Move> & branches) : root(), branches(branches) {};
    Node(const Move& root, vector<Move> branches)
        : root(std::make_shared<const Move>(root)), branches(std::move(branches)) {};
};


#endif //CPPMORPIONSOLITAIRE_NODE_H
