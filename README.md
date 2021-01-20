# MorpionSolitaire

This is a Python implementation of the pen-and-paper game **Morpion Solitaire** (aka. **Join Five**, see the [Wikipedia page](https://en.wikipedia.org/wiki/Join_Five)).
It is a simple game, but with a very interesting dynamics. In its classical version, a [world record of 172 points](http://www.chrisrosin.com/morpion/index.html) has been obtained using a Monte-Carlo search.

My goal is to develop a deep learning model to explore even more efficiently the possibilities of the game. At the moment, only random exploration is implemented, and the configurations reached in this way are typically far from optimal.

The main library is MorpionSolitaire.py. The human-readable content is in the Jupyter notebooks:
- `Documentation.ipynb`: package documentation, with examples
- `Random_Exploration.ipynb`: notebook performing random exploration of the game outputs

In addition to the original set of rules, several variants that are playable on [joinfive.com](http://joinfive.com/) have been (or will be) implemented too:
- Cross 5T (original rules)
- Cross 5D (consecutive lines are not allowed to touch)
- Cross 4T (connecting 4 points instead of 5)
- Cross 4D
- Pipe 5T (different starting grid, in the shape of a pipe instead of a cross)
- Pipe 5D
- random scattered set of starting points

