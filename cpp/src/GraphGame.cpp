#include "../include/GraphGame.h"
#include <algorithm>
#include <iostream>
#include <memory>
#include <random>

using std::cout, std::endl;
using std::string;

GraphGame::GraphGame(char type, int length, bool disjoint) : Game(type, length, disjoint, false), nodes()
{
    buildGraph();
}

void GraphGame::buildGraph()
{
    image.clear();

    // add initial points
    for (const GridPoint& dot: grid.initialDots)
        image.set(dot.toImagePoint(), true);
    nodes.emplace_back(findAllMoves()); // computes all possible moves

    // add moves successively
    for (const GridMove& gridMove: grid.moves)
        if (!tryPlay(gridMove.line, gridMove.dot))
            throw std::logic_error("Trying to load a grid with an invalid segment");
}

int GraphGame::getScore() const
{
    return (int)nodes.size() - 1;
}

int GraphGame::getNumberOfMoves() const
{
    return (int)nodes.back().branches.size();
}

void GraphGame::play(const Move& move)
{
    applyMove(move);
    vector<Move> branches = findNewMoves(move.dot);
    for (const Move& branch: nodes.back().branches)
        if (isValidMove(branch))
            branches.push_back(branch);
    nodes.emplace_back(move, branches);
}

void GraphGame::play(int index)
{
    play(nodes.back().branches[index]);
}

bool GraphGame::tryPlay(const GridLine &line)
{
    for (const Move& move: nodes.back().branches)
        if (move.line == line)
        {
            play(move);
            return true;
        }

    return false;
}

bool GraphGame::tryPlay(const GridLine &line, const GridPoint &dot)
{
    for (const Move& move: nodes.back().branches)
        if (move.line == line)
        {
            if (dot != move.dot)
                return false;
            play(move);
            return true;
        }

    return false;
}

int GraphGame::randomInt(int max, int min)
{
    static std::random_device rd;
    static std::mt19937 gen(rd());
    std::uniform_int_distribution<> dist(min, max);
    return dist(gen);
}

void GraphGame::playAtRandom(int n)
{
    if (n <= 0) return;

    int numberOfMoves = getNumberOfMoves();
    if (numberOfMoves == 0) return;

    play(randomInt(numberOfMoves - 1));
    playAtRandom(n - 1);
}

void GraphGame::playAtRandom()
{
    while (true)
    {
        int numberOfMoves = getNumberOfMoves();
        if (numberOfMoves == 0) return;

        play(randomInt(numberOfMoves - 1));
    }
}

void GraphGame::undo()
{
    if ((int)nodes.size() <= 1)
        throw std::invalid_argument("Cannot undo at score zero");

    // remove last move of the grid
    grid.moves.pop_back();

    // undo last move on the image
    image.apply(*nodes.back().root, false);

    // remove last node
    nodes.pop_back();
}

void GraphGame::undo(int steps)
{
    while (steps > 0 && !grid.moves.empty())
    {
        undo();
        steps--;
    }
}

void GraphGame::restart()
{
    undo(getScore());
}

void GraphGame::print() const
{
    Game::print();
    cout << "Number of possible moves: " << getNumberOfMoves() << endl;
}

void GraphGame::revertToScore(int score)
{
    undo(getScore() - score);
}

void GraphGame::revertToRandomScore()
{
    revertToScore(randomInt(getScore() - 2));
}

vector<int> GraphGame::repeatPlayAtRandom(int n, char type, int length, bool disjoint)
{
    vector<int> result(60);
    GraphGame gameGraph(type, length, disjoint);
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

void GraphGame::playNestedMC(int level)
{
    if (level == 0)
        playAtRandom();
    else
    {
        if (nodes.back().branches.empty())
            return;
        int currentScore = getScore();
        const vector<Move>& branches = nodes.back().branches;
        int branchCount = branches.size();
        int bestScore = currentScore;
        int bestMove;
        for (int i = 0; i < branchCount; i++)
        {
            play(i);
            playNestedMC(level - 1);
            int score = getScore();
            if (score > bestScore){
                bestScore = score;
                bestMove = i;
            }
            revertToScore(currentScore);
        }
        play(bestMove);
        playNestedMC(level);
    }
}