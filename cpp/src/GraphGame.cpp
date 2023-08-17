#include "../include/GraphGame.h"
#include "../include/GridDTO.h"

#include <algorithm>
#include <iostream>
#include <random>

using std::cout, std::endl;
using std::string;

template <size_t length, bool disjoint>
GraphGame<length, disjoint>::GraphGame(char type) : Game<length, disjoint>(type), nodes()
{
    buildGraph();
}

template <size_t length, bool disjoint>
GraphGame<length, disjoint>::GraphGame(const Grid& grid) : Game<length, disjoint>(grid, false), nodes()
{
    buildGraph();
}

template <size_t length, bool disjoint>
void GraphGame<length, disjoint>::buildGraph()
{
    Game<length, disjoint>::image.clear();

    // add initial points
    for (Point dot: Game<length, disjoint>::grid.initialDots)
        Game<length, disjoint>::image.value[dot] = true;
    nodes.emplace_back(Game<length, disjoint>::findAllMoves()); // computes all possible moves

    // add moves successively
    for (const GridMove& gridMove: Game<length, disjoint>::grid.moves)
    {
        optional<Move<length, disjoint>> move = findMove(gridMove.line, gridMove.dot);
        if (!move.has_value())
            throw std::logic_error("Trying to load a grid with an invalid segment");
        Game<length, disjoint>::image.apply(*move);
        addNode(*move);
    }
}

template <size_t length, bool disjoint>
int GraphGame<length, disjoint>::getScore() const
{
    return (int)nodes.size() - 1;
}

template <size_t length, bool disjoint>
int GraphGame<length, disjoint>::getNumberOfMoves() const
{
    return (int)nodes.back().branches.size();
}

template <size_t length, bool disjoint>
void GraphGame<length, disjoint>::play(const Move<length, disjoint>& move)
{
    Game<length, disjoint>::applyMove(move);
    addNode(move);
}

template <size_t length, bool disjoint>
void GraphGame<length, disjoint>::addNode(const Move<length, disjoint>& move)
{
    vector<Move<length, disjoint>> branches = Game<length, disjoint>::findNewMoves(move.dot);
    for (const Move<length, disjoint>& branch: nodes.back().branches)
        if (Game<length, disjoint>::isStillValidMove(branch))
            branches.emplace_back(branch);
    nodes.emplace_back(move, branches);
}

template <size_t length, bool disjoint>
void GraphGame<length, disjoint>::play(int index)
{
    play(nodes.back().branches[index]);
}

template <size_t length, bool disjoint>
optional<Move<length, disjoint>> GraphGame<length, disjoint>::findMove(const Line& line) const
{
    for (const Move<length, disjoint>& move: nodes.back().branches)
        if (move.line == line)
        {
            return move;
        }

    return {};
}

template <size_t length, bool disjoint>
optional<Move<length, disjoint>> GraphGame<length, disjoint>::findMove(const Line& line, Point dot) const
{
    for (const Move<length, disjoint>& move: nodes.back().branches)
        if (move.line == line && move.dot == dot)
        {
            return move;
        }

    return {};
}

template <size_t length, bool disjoint>
bool GraphGame<length, disjoint>::tryPlay(const Line &line)
{
    optional<Move<length, disjoint>> move = findMove(line);
    if (move.has_value())
    {
        play(*move);
        return true;
    }

    return false;
}

template <size_t length, bool disjoint>
bool GraphGame<length, disjoint>::tryPlay(const Line &line, Point dot)
{
    optional<Move<length, disjoint>> move = findMove(line, dot);
    if (move.has_value())
    {
        play(*move);
        return true;
    }

    return false;
}

template <size_t length, bool disjoint>
int GraphGame<length, disjoint>::randomInt(int max, int min)
{
    static std::random_device rd;
    static std::mt19937 gen(rd());
    std::uniform_int_distribution<> dist(min, max);
    return dist(gen);
}

template <size_t length, bool disjoint>
void GraphGame<length, disjoint>::playAtRandom(int n)
{
    if (n <= 0) return;

    int numberOfMoves = getNumberOfMoves();
    if (numberOfMoves == 0) return;

    play(randomInt(numberOfMoves - 1));
    playAtRandom(n - 1);
}

template <size_t length, bool disjoint>
void GraphGame<length, disjoint>::playAtRandom()
{
    while (true)
    {
        int numberOfMoves = getNumberOfMoves();
        if (numberOfMoves == 0) return;

        play(randomInt(numberOfMoves - 1));
    }
}

template <size_t length, bool disjoint>
void GraphGame<length, disjoint>::undo()
{
    if ((int)nodes.size() <= 1)
        throw std::invalid_argument("Cannot undo at score zero");

    // remove last move of the grid
    Game<length, disjoint>::grid.moves.pop_back();

    // undo last move on the value
    Game<length, disjoint>::image.apply(nodes.back().root.value(), false);

    // remove last node
    nodes.pop_back();
}

template <size_t length, bool disjoint>
void GraphGame<length, disjoint>::undo(int steps)
{
    while (steps > 0 && !Game<length, disjoint>::grid.moves.empty())
    {
        undo();
        steps--;
    }
}

template <size_t length, bool disjoint>
void GraphGame<length, disjoint>::restart()
{
    undo(getScore());
}

template <size_t length, bool disjoint>
void GraphGame<length, disjoint>::print() const
{
    Game<length, disjoint>::print();
    cout << "Number of possible moves: " << getNumberOfMoves() << endl;
}

template <size_t length, bool disjoint>
void GraphGame<length, disjoint>::revertToScore(int score)
{
    undo(getScore() - score);
}

template <size_t length, bool disjoint>
void GraphGame<length, disjoint>::revertToRandomScore()
{
    revertToScore(randomInt(getScore() - 2));
}

template <size_t length, bool disjoint>
vector<int> GraphGame<length, disjoint>::repeatPlayAtRandom(int n, char type)
{
    vector<int> result(60);
    GraphGame<length, disjoint> gameGraph(type);
    for (int i = 0; i < n; i++)
    {
        gameGraph.restart();
        gameGraph.playAtRandom();

        int score = gameGraph.getScore();
        while (score >= result.size()){
            result.push_back(0);
        }
        result[score]++;
    }
    return result;
}

template <size_t length, bool disjoint>
vector<Move<length, disjoint>> GraphGame<length, disjoint>::getSequenceOfMoves(int score) const
{
    vector<Move<length, disjoint>> result;
    for (int i = (int)nodes.size() - 1; i > score; i--)
        result.emplace_back(nodes[i].root.value());
    return result;
}

template <size_t length, bool disjoint>
void GraphGame<length, disjoint>::playNestedMC(int level, vector<Move<length, disjoint>> & bestBranch)
{
    if (level == 0)
        playAtRandom();
    else
    {
        if (nodes.back().branches.empty())
            return;
        int currentScore = getScore();
        int bestScore = currentScore + (int)bestBranch.size();
        const vector<Move<length, disjoint>> branches = nodes.back().branches;
        for (const Move<length, disjoint>& move: branches)
        {
            play(move);
            playNestedMC(level - 1);
            int score = getScore();
            if (score > bestScore){
                bestScore = score;
                bestBranch = getSequenceOfMoves(currentScore);
            }
            revertToScore(currentScore);
        }
        play(bestBranch.back());
        bestBranch.pop_back();
        playNestedMC(level, bestBranch);
    }
}

template <size_t length, bool disjoint>
void GraphGame<length, disjoint>::playNestedMC(int level)
{
    vector<Move<length, disjoint>> bestBranch;
    playNestedMC(level, bestBranch);
}

template <size_t length, bool disjoint>
void GraphGame<length, disjoint>::playFastNestedMC(int level, bool checkMoveOrdering, vector<Move<length, disjoint>> & bestBranch)
{
    if (level == 0)
        playAtRandom();
    else
    {
        if (nodes.back().branches.empty())
            return;
        int currentScore = getScore();
        int bestScore = currentScore + (int)bestBranch.size();
        const vector<Move<length, disjoint>> branches = nodes.back().branches;
        const optional<Move<length, disjoint>> lastMove = nodes.back().root;
        for (const Move<length, disjoint>& move: branches)
            // when 'checkMoveOrdering' is active, skip moves that commute with the last move and do not obey canonical ordering
            if (!checkMoveOrdering
                || std::find(move.existingDots.begin(), move.existingDots.end(), lastMove->dot) != move.existingDots.end()
                || move.line.pt1 < lastMove->line.pt1
                || (move.line.pt1 == lastMove->line.pt1 && move.line.pt2 < lastMove->line.pt2))
            {
                play(move);
                playFastNestedMC(level - 1, true);
                int score = getScore();
                if (score > bestScore){
                    bestScore = score;
                    bestBranch = getSequenceOfMoves(currentScore);
                }
                revertToScore(currentScore);
            }
        if (bestBranch.empty())
            return;
        play(bestBranch.back());
        bestBranch.pop_back();
        playFastNestedMC(level, false, bestBranch);
    }
}

template <size_t length, bool disjoint>
void GraphGame<length, disjoint>::playFastNestedMC(int level, bool checkMoveOrdering)
{
    vector<Move<length, disjoint>> bestBranch;
    playFastNestedMC(level, checkMoveOrdering, bestBranch);
}

template <size_t length, bool disjoint>
void GraphGame<length, disjoint>::playFastNestedMC(int level)
{
    vector<Move<length, disjoint>> bestBranch;
    playFastNestedMC(level, false, bestBranch);
}

//template <size_t length, bool disjoint>
//vector<vector<Move<length, disjoint>>> GraphGame<length, disjoint>::findOrderedSequencesOfMoves(int level)
//{
//    vector<vector<Move<length, disjoint>>> result;
//
//    const vector<Move<length, disjoint>> branches = nodes.back().branches;
//    for (const Move<length, disjoint>& branch: branches)
//    {
//        if (level < 2)
//        {
//            vector<Move<length, disjoint>> sequence = {branch};
//            result.push_back(sequence);
//        }
//        else
//        {
//            play(branch);
//            vector<vector<Move<length, disjoint>>> previousSequences = findOrderedSequencesOfMoves(level - 1);
//            undo();
//
//            for (vector<Move<length, disjoint>> sequence: previousSequences)
//            {
//                // check whether the last two moves of the sequence commute and are not ordered canonically
//                const Move<length, disjoint> & nextMove = sequence.back();
//                const array<Point, length> & existingDots = nextMove.existingDots;
//                if (std::find(existingDots.begin(), existingDots.end(), branch.dot) != existingDots.end()
//                    || nextMove.line.pt1 < branch.line.pt1
//                    || (nextMove.line.pt1 == branch.line.pt1 && nextMove.line.pt2 < branch.line.pt2))
//                {
//                    // if this is not the case, then add them to the list of ordered sequences
//                    sequence.push_back(branch);
//                    result.push_back(sequence);
//                }
//            }
//        }
//    }
//
//    return result;
//}

template <size_t length, bool disjoint>
GraphGame<length, disjoint> GraphGame<length, disjoint>::importJSON(const string& json)
{
    GridDTO dto(json);
    return GraphGame<length, disjoint>(dto.toGrid());
}


template class GraphGame <4, false>;
template class GraphGame <4, true>;
template class GraphGame <3, false>;
template class GraphGame <3, true>;