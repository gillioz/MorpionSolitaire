#include "../include/GraphGame.h"
#include <cstdlib>
#include <fstream>
#include <iomanip>
#include <iostream>
#include <random>
#include <sstream>
#include <string>

using namespace std;

int main(int argc, char *argv[])
{
    if (argc < 3 || argc > 6)
    {
        cout << "Usage: " << argv[0]
            << " <output path> <algorithm level> <optional: game type> <optional: line length> <optional: disjoint>" << endl;
        return 0;
    }

    string path(argv[1]);
    if (path.substr(path.length() - 1) != "/")
        path.append("/");

    const int level = atoi(argv[2]);

    const char type = (argc > 3) ? argv[3][0] : 'c';
    string fullType = type == 'c' ? "cross" : type == 'p' ? "pipe" : "none";

    const int length = (argc > 4) ? atoi(argv[4]) : 4;
    fullType.append(to_string(length + 1));

    const bool disjoint = argc > 5 && argv[5][0] == 'D';
    fullType.append(disjoint ? "D" : "T");

    random_device rd;
    mt19937 gen(rd());
    uniform_int_distribution<> distr(0,99999);
    distr(gen);

    string json;
    int score;

    cout << "Creating a game of type '" << fullType << "'" << endl;
    cout << "Running level-" << level << " nested Monte-Carlo algorithm..." << flush;
    if (length == 4 && !disjoint)
    {
        GraphGame<4, false> game(type);
        game.playFastNestedMC(level);
        score = game.getScore();
        json = game.exportJSON();
    }
    else if (length == 4)
    {
        GraphGame<4, true> game(type);
        game.playFastNestedMC(level);
        score = game.getScore();
        json = game.exportJSON();
    }
    else if (length == 3 && !disjoint)
    {
        GraphGame<3, false> game(type);
        game.playFastNestedMC(level);
        score = game.getScore();
        json = game.exportJSON();
    }
    else if (length == 3)
    {
        GraphGame<3, true> game(type);
        game.playFastNestedMC(level);
        score = game.getScore();
        json = game.exportJSON();
    }
    else
    {
        cout << "failed!" << endl << "ERROR: invalid game format" << endl;
        return 0;
    }
    cout << "done!" << endl;
    cout << "Score reached: " << score << endl;


    stringstream filename;
    filename << path << fullType << "_" << setfill('0') << setw(3) << score << "_"  << setw(5) << distr(gen) << ".json";
    cout << "Writing result to file " << filename.str() << "...";
    ofstream file(filename.str());
    file << json;
    file.close();
    cout << "done!" << endl;

    return 0;
}
