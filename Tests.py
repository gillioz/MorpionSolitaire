#!/usr/bin/env python3.6

from MorpionSolitaire import *

print()
print('Unweighted Monte-Carlo')
print('**********************')

game = MorpionSolitaire(16, 4)

game.seed.graph.cell[7][8][0] = 1
game.seed.graph.cell[7][9][0] = 1
game.seed.graph.cell[7][10][0] = 1
game.seed.graph.cell[7][11][0] = 1
game.seed.graph.cell[8][7][0] = 1
game.seed.graph.cell[9][7][0] = 1
game.seed.graph.cell[10][7][0] = 1
game.seed.graph.cell[11][7][0] = 1
game.seed.graph.cell[4][9][0] = 1
game.seed.graph.cell[3][10][0] = 1
game.seed.graph.cell[2][11][0] = 1

game.seed.legalmoves = game.findlegalmoves(game.seed.graph)
#game.seed.draw()


m = 10000
n = 0
for i in range(m):
	randombranch = game.seed.randomexploration()
	if randombranch.level == 3:
		n = n + 1
	game.seed.cleardescendants()

print('Level 3 attained', n, 'times out of', m)
print('Ratio:', n/m, '(expected:', 1/8, ')') 


print()
print('Weighted Monte-Carlo')
print('********************')

game = MorpionSolitaire(16, 4, True)

game.seed.graph.cell[7][8][0] = 1
game.seed.graph.cell[7][9][0] = 1
game.seed.graph.cell[7][10][0] = 1
game.seed.graph.cell[7][11][0] = 1
game.seed.graph.cell[8][7][0] = 1
game.seed.graph.cell[9][7][0] = 1
game.seed.graph.cell[10][7][0] = 1
game.seed.graph.cell[11][7][0] = 1
game.seed.graph.cell[4][9][0] = 1
game.seed.graph.cell[3][10][0] = 1
game.seed.graph.cell[2][11][0] = 1

game.seed.legalmoves = game.findlegalmoves(game.seed.graph)
#game.seed.draw()


m = 10000
n = 0
for i in range(m):
	randombranch = game.seed.randomexploration()
	if randombranch.level == 3:
		n = n + 1
	game.seed.cleardescendants()

print('Level 3 attained', n, 'times out of', m)
print('Ratio:', n/m, '(expected:', 1/6, ')') 

