# MorpionSolitaire

This is a Python implementation of the pen-and-paper game Morpion Solitaire (aka. Join Five, https://en.wikipedia.org/wiki/Join_Five).
It performs random exploration of game outputs, and gives various statistics at any intermediate stage (number of legal moves, number of points that are not being used).

In addition to the original set of rules, several variants that are playable on http://joinfive.com/ have been implemented:
- Cross 5T (original rules)
- Cross 5D (consecutive lines are not allowed to touch)
- Cross 4T (connecting 4 points instead of 5)
- Cross 4D
- Pipe 5T (different starting grid, in the shape of a pipe instead of a cross)
- Pipe 5D
In each case the best configuration reached by random exploration is provided (e.g. bestCross5T.dat). Note that the configurations reached in this way are typically far from the current world record of 172 points.

The main library is MorpionSolitaire.py. The executable files are:
- RandomExploration.py: performs a random exploration of the game outputs; takes as input one of the files (e.g. bestCross5T.dat) to initialize the rules, and (optional) the number of games to be played and the frequency at which statistics is shown
- DisplayFile.py: displays any configuration, e.g. bestCross5T.dat
- Tests.py: tests of the difference between weighted and unweighted Monte-Carlo implementations

Despite simple rules, the dynamics of the game is very interesting: figure Stats_Cross5T.png shows that most random explorations end with a miserable score of about 20-25 points (original rules), but when the 40-points barrier is passed, they are again many more possible outputs. It is in fact very hard to reach high scores from a purely random exploration.
