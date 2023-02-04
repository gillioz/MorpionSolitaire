MorpionSolitaire
================

This is a Python implementation of the pen-and-paper game **Morpion Solitaire** (aka. **Join Five**, see the [Wikipedia page](https://en.wikipedia.org/wiki/Join_Five)).
It is a simple game, but with a very interesting dynamics. In its classical version, a [world record of 172 points](http://www.chrisrosin.com/morpion/index.html) has been obtained using a Monte-Carlo search.

My goal is to develop a deep learning model to explore even more efficiently the possibilities of the game. At the moment, only random exploration is implemented, and the configurations reached in this way are typically far from optimal.


Implementation
--------------

This repository contains both an original implementation of the game in Python, and a more recent implementation in C# (.NET). The latter is not only much faster, it contains a playable version of the game with some cool features.

### Python

The source code is in the folder `python`. The main library is `MorpionSolitaire.py`. The human-readable content is in the Jupyter notebooks:
- `Documentation.ipynb`: package documentation, with examples
- `Random_Exploration.ipynb`: notebook performing random exploration of the game outputs

### .NET

The C# implementation of the game is in the folder `dotnet`. There are several projects in there:
- `MorpionSolitaire` is the core implementation of the game.
- `MorpionSolitaireGraph` is an extension that contains several useful features, such as keeping track of possible moves at every stage of the game.
- `MorpionSolitaireWeb` implements a playable version of the game as a web application.
- `MorpionSolitaireCLI` is a command-line interface for performing exploration tasks, such as playing a large number of games randomly and extracting some statistics.


Alternative rules
-----------------

In addition to the original set of rules, several variants that are playable on [joinfive.com](http://joinfive.com/) have been (or will be) implemented too:
- Cross 5T (original rules)
- Cross 5D (consecutive lines are not allowed to touch)
- Cross 4T (connecting 4 points instead of 5)
- Cross 4D
- Pipe 5T (different starting grid, in the shape of a pipe instead of a cross)
- Pipe 5D
- random scattered set of starting points


Deep learning
-------------

