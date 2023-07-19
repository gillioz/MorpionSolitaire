#include <iostream>
#include <cassert>
#include <chrono>
#include "../include/GraphGame.h"
#include "../include/Coordinates.h"

using namespace std;

int main()
{
    cout << "Running Morpion Solitaire tests" << endl;

    cout << "Creating a game...";
    GraphGame game;
    cout << "ok" << endl;

    cout << "Counting possible moves...";
    assert (game.getNumberOfMoves() == 28);
    assert (game.tryPlay({Coordinates(30, 36).toPoint(), Coordinates(34, 36).toPoint()}));
    assert (game.getNumberOfMoves() == 26);
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

    cout << "Restart the game...";
    game.restart();
    assert (game.getScore() == 0);
    assert (game.getNumberOfMoves() == 28);
    cout << "ok" << endl;

    cout << "Play several moves at random...";
    game.playAtRandom(3);
    assert (game.getScore() == 3);
    assert (game.getNumberOfMoves() > 0);
    cout << "ok" << endl;

    cout << "Play a game at random...";
    game.playAtRandom();
    assert (game.getScore() >= 20);
    assert (game.getNumberOfMoves() == 0);
    cout << "ok" << endl;

    cout << "Restart the game...";
    game.restart();
    assert (game.getScore() == 0);
    assert (game.getNumberOfMoves() == 28);
    cout << "ok" << endl;

    cout << "Playing using the nested Monte-Carlo algorithm...";
    game.restart();
    game.playNestedMC(2);
    assert (game.getScore() >= 20);
    cout << "ok" << endl;

    cout << "Restart the game...";
    game.restart();
    assert (game.getScore() == 0);
    assert (game.getNumberOfMoves() == 28);
    cout << "ok" << endl;

    cout << "Create empty game...";
    GraphGame emptyGame('e');
    assert (emptyGame.getScore() == 0);
    assert (emptyGame.getNumberOfMoves() == 0);
    cout << "ok" << endl;

    cout << "Number of random game played per second: ";
    int nGames = 1000;
    auto timeBefore = std::chrono::high_resolution_clock::now();
    auto series = GraphGame::repeatPlayAtRandom(nGames);
    auto timeAfter = std::chrono::high_resolution_clock::now();
    std::chrono::duration<double, std::milli> timeInterval = timeAfter - timeBefore;
    cout << (int)((double)nGames / timeInterval.count() * 1000.0) << endl;
}