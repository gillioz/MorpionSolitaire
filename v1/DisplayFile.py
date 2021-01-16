#!/usr/bin/env python3.6

import sys
import os.path
from MorpionSolitaire import *

if len(sys.argv) != 2:
	print('Usage:', sys.argv[0], '<name of the file to read>')
	sys.exit()

filename = sys.argv[1]

if not os.path.isfile(filename):
	print('File', filename, 'does not exist')
	sys.exit()

game = MorpionSolitaire.read(filename, 1)

game.printinfo()
branch = game.getfirstbranch()

print('Score:', branch.level)
print()

branch.printbranchinfo()

branch.draw()
#branch.graph.draw()

