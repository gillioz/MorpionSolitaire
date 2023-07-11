#include <iostream>
#include <cassert>
#include "../include/Game.h"

using namespace std;

int main(){
    cout << "Running Morpion Solitaire tests" << endl;

    cout << "Creating a game...";
    Game game;
    cout << "ok" << endl;

    cout << "Counting possible moves...";
    assert (game.findAllMoves().size() == 28);
    assert (game.tryPlay({{30, 36}, {34, 36}}));
    assert (game.findAllMoves().size() == 26);
    cout << "ok" << endl;

    cout << "Trying to add a bunch of illegal segments...";
    assert (!game.tryPlay({{30, 30}, {30, 30}}));
    assert (!game.tryPlay({{30, 30}, {30, 34}}));
    assert (!game.tryPlay({{30, 26}, {34, 30}}));
    cout << "ok" << endl;

    cout << "Adding a bunch of legal segments...";
    assert (game.tryPlay({{29, 27}, {33, 27}}));
    assert (game.tryPlay({{31, 27}, {27, 31}}));
    assert (game.tryPlay({{30, 26}, {30, 30}}));
    assert (game.tryPlay({{30, 26}, {34, 30}}));
    assert (game.getScore() == 5);
    cout << "ok" << endl;

    cout << "Undo and redo a move...";
    assert (!game.tryPlay({{30, 26}, {34, 30}}));
    game.undo();
    assert (game.getScore() == 4);
    assert (game.tryPlay({{30, 26}, {34, 30}}));
    assert (game.getScore() == 5);
    assert (!game.tryPlay({{30, 26}, {34, 30}}));
    cout << "ok" << endl;

    cout << "Finding new segments after a move a bunch of legal segments...";
    Move* move = game.tryBuildMove({{30,32},{30,36}});
    assert (move != nullptr);
    game.applyMove(*move);
    assert (game.findNewMoves(move->dot).empty());
    move = game.tryBuildMove({{33,32},{33,36}});
    assert (move != nullptr);
    game.applyMove(*move);
    assert (game.findNewMoves(move->dot).size() == 1);
    cout << "ok" << endl;

    cout << "Restart the game...";
    game.restart();
    assert (game.getScore() == 0);
    cout << "ok" << endl;

    cout << "Create empty game...";
    Game emptyGame('e');
    assert (emptyGame.getScore() == 0);
    assert (emptyGame.findAllMoves().empty());
    cout << "ok" << endl;
}