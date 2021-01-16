#!/usr/bin/env python3.6

import sys
import os.path
from MorpionSolitaire import *



if len(sys.argv) < 2 or len(sys.argv) > 4:
	print('Usage:', sys.argv[0], '<name of the file to read> [number of iterations] [print info every # iterations]')
	sys.exit()

filename = sys.argv[1]

if not os.path.isfile(filename):
	print('File', filename, 'does not exist')
	sys.exit()
	
#game = MorpionSolitaire.readformat(filename)
game = MorpionSolitaire.readformat(filename, 1)
game.printinfo()


record = MorpionSolitaire.readcount(filename)
print('Record:', record)
print()

iterations = 10
display = 1

if len(sys.argv) > 2:
	iterations = int(sys.argv[2])
if len(sys.argv) > 3:
	display = int(sys.argv[3])

nmax = 0
histogram = []
for i in range(iterations):
	randombranch = game.seed.randomexploration()
	n = randombranch.level
	if n > nmax:
		nmax = n
		best = randombranch
		game.seed.next = []
		histogram.extend([0] * (n - len(histogram)))
	else:
		game.seed.cleardescendants()
	histogram[n-1] = histogram[n-1] + 1
	if (i+1) % display == 0:
		print('Step:',i+1,'  max:', nmax,'  this step:',n)

print()
print('Frequency of scores:')
print('********************')
print(histogram)

print()
print('Best result:')
print('***********')
print('Final score:', best.level)

best.printbranchinfo()
best.draw()

if best.level > record:
	print()
	print('Replacing old record (', record,') with new one (', best.level, ')')
	game.write(best, filename)
	print()


