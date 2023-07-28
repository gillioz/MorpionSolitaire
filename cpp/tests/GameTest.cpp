#include <cassert>
#include <iostream>
#include "../include/Game.h"
#include "../include/Coordinates.h"

using namespace std;

int main(){
    cout << "Running Morpion Solitaire tests" << endl;

    cout << "Creating a game...";
    Game<4, false> game;
    cout << "ok" << endl;

    cout << "Counting possible moves...";
    assert (game.findAllMoves().size() == 28);
    assert (game.tryPlay({Coordinates(30, 36).toPoint(), Coordinates(34, 36).toPoint()}));
    assert (game.findAllMoves().size() == 26);
    cout << "ok" << endl;

    cout << "Trying to add a bunch of illegal segments...";
    assert (!game.tryPlay({Coordinates(30, 30).toPoint(), Coordinates(30, 30).toPoint()}));
    assert (!game.tryPlay({Coordinates(30, 30).toPoint(), Coordinates(30, 34).toPoint()}));
    assert (!game.tryPlay({Coordinates(30, 26).toPoint(), Coordinates(34, 30).toPoint()}));
    cout << "ok" << endl;

    cout << "Adding a bunch of legal segments...";
    assert (game.tryPlay({Coordinates(29, 27).toPoint(), Coordinates(33, 27).toPoint()}));
    assert (game.tryPlay({Coordinates(31, 27).toPoint(), Coordinates(27, 31).toPoint()}));
    assert (game.tryPlay({Coordinates(30, 26).toPoint(), Coordinates(30, 30).toPoint()}));
    assert (game.tryPlay({Coordinates(30, 26).toPoint(), Coordinates(34, 30).toPoint()}));
    assert (game.getScore() == 5);
    cout << "ok" << endl;

    cout << "Undo and redo a move...";
    assert (!game.tryPlay({Coordinates(30, 26).toPoint(), Coordinates(34, 30).toPoint()}));
    game.undo();
    assert (game.getScore() == 4);
    assert (game.tryPlay({Coordinates(30, 26).toPoint(), Coordinates(34, 30).toPoint()}));
    assert (game.getScore() == 5);
    assert (!game.tryPlay({Coordinates(30, 26).toPoint(), Coordinates(34, 30).toPoint()}));
    cout << "ok" << endl;

    cout << "Finding new segments after a move a bunch of legal segments...";
    auto move1 = game.tryBuildMove({Coordinates(30,32).toPoint(), Coordinates(30,36).toPoint()});
    assert (move1.has_value());
    game.applyMove(move1.value());
    assert (game.findNewMoves(move1->dot).empty());
    auto move2 = game.tryBuildMove({Coordinates(33,32).toPoint(), Coordinates(33,36).toPoint()});
    assert (move2.has_value());
    game.applyMove(move2.value());
    assert (game.findNewMoves(move2->dot).size() == 1);
    cout << "ok" << endl;

    cout << "Restart the game...";
    game.restart();
    assert (game.getScore() == 0);
    cout << "ok" << endl;

    cout << "Create empty game...";
    Game<4, false> emptyGame('e');
    assert (emptyGame.getScore() == 0);
    assert (emptyGame.findAllMoves().empty());
    cout << "ok" << endl;
}