
import numpy as np
from PIL import Image
import matplotlib as mpl
import matplotlib.pyplot as plt
#from PIL import Image, ImageDraw, ImageFont
#from random import choice, shuffle

startinggrids = {
   'cross': np.array([[0, 0, 0, 1, 1, 1, 1, 0, 0, 0],
                      [0, 0, 0, 1, 0, 0, 1, 0, 0, 0],
                      [0, 0, 0, 1, 0, 0, 1, 0, 0, 0],
                      [1, 1, 1, 1, 0, 0, 1, 1, 1, 1],
                      [1, 0, 0, 0, 0, 0, 0, 0, 0, 1],
                      [1, 0, 0, 0, 0, 0, 0, 0, 0, 1],
                      [1, 1, 1, 1, 0, 0, 1, 1, 1, 1],
                      [0, 0, 0, 1, 0, 0, 1, 0, 0, 0],
                      [0, 0, 0, 1, 0, 0, 1, 0, 0, 0],
                      [0, 0, 0, 1, 1, 1, 1, 0, 0, 0]]),
   'pipe': np.array([[0, 0, 0, 1, 1, 1, 1, 0, 0, 0],
                     [0, 0, 1, 0, 0, 0, 0, 1, 0, 0],
                     [0, 1, 0, 0, 0, 0, 0, 0, 1, 0],
                     [1, 0, 0, 1, 1, 1, 1, 0, 0, 1],
                     [1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
                     [1, 0, 0, 1, 0, 0, 1, 0, 0, 1],
                     [1, 0, 0, 1, 1, 1, 1, 0, 0, 1],
                     [0, 1, 0, 0, 0, 0, 0, 0, 1, 0],
                     [0, 0, 1, 0, 0, 0, 0, 1, 0, 0],
                     [0, 0, 0, 1, 1, 1, 1, 0, 0, 0]]),
   'cross4': np.array([[0, 0, 1, 1, 1, 0, 0],
                       [0, 0, 1, 0, 1, 0, 0],
                       [1, 1, 1, 0, 1, 1, 1],
                       [1, 0, 0, 0, 0, 0, 1],
                       [1, 1, 1, 0, 1, 1, 1],
                       [0, 0, 1, 0, 1, 0, 0],
                       [0, 0, 1, 1, 1, 0, 0]])
}

directions = ((0,0), (1, 0), (0, 1), (1,1), (-1,1))
# Conventions for unit cell and directions:
#
# (4) (2) (3)
#   \  |  /
#    \ | /
#     \|/ 
#      O---(1)
#     (0)

class Grid:
    dimension = 22
    
    def __init__(self, pattern = ''):
        self.cell = np.full((self.dimension, self.dimension, 5), False)
        if pattern in startinggrids.keys():
            w, h = startinggrids[pattern].shape
            x0 = (self.dimension - w)//2
            y0 = (self.dimension - h)//2
            self.cell[x0:x0+w,y0:y0+h,0] = startinggrids[pattern].astype(bool)
    
    def copy(self):
        newgrid = Grid()
        newgrid.cell = np.copy(self.cell)
        return newgrid
	
    def print(self, size=6, color='k', marker='o', markersize=4, fct=None):
        fig = plt.figure(figsize=(size, size))
        ax = fig.gca(aspect=1, autoscale_on = False,
                     xticks=range(self.dimension), xticklabels='',
                     yticks=range(self.dimension), yticklabels='')
        plt.grid()
        for x in range(self.dimension):
            for y in range(self.dimension):
                if self.cell[x,y,0]:
                    plt.plot(x,y,color=color,marker=marker, markersize=markersize)
                for dir in range(1, 5):
                    if self.cell[x,y,dir]:
                        plt.plot([x,x+directions[dir][0]],[y,y+directions[dir][1]],color=color)
        if not(fct is None):
            fct(plt)
        plt.show()
    
    def image(self):
        n = 3*self.dimension - 2
        im = np.empty((n,n), dtype=bool)
        im[0::3,0::3] = self.cell[:,:,0]
        im[1::3,0::3] = self.cell[:-1,:,1]
        im[2::3,0::3] = self.cell[:-1,:,1]
        im[0::3,1::3] = self.cell[:,:-1,2]
        im[0::3,2::3] = self.cell[:,:-1,2]
        im[1::3,1::3] = self.cell[:-1,:-1,3]
        im[2::3,2::3] = self.cell[:-1,:-1,3]
        im[1::3,2::3] = self.cell[1:,:-1,4]
        im[2::3,1::3] = self.cell[1:,:-1,4]
        return Image.fromarray(im)

    def addsegment(self, move, seglen, checklegalmove=True):
        # move: x, y, dir, n
        # x, y: position of the point to be added
        # dir: direction
        # n: position of the point on the segment
        if checklegalmove:
            if self.islegalmove(move, seglen) == False:
                raise Exception('Trying to add a segment {} of length {} at an illegal position'.format(move, seglen))
        x, y, dir, n = move
        dx, dy = directions[dir]
        self.cell[x, y, 0] = True
        for i in range(seglen):
            self.cell[x+(i-n)*dx, y+(i-n)*dy, dir] = True
    
    def islegalmove(self, move, seglen):
        x, y, dir, n = move
        dx, dy = directions[dir]
        x1, y1 = x - n*dx, y - n*dy
        x2, y2 = x + (seglen-n)*dx, y + (seglen-n)*dy
        if (x1 < 0  or x1 >= self.dimension
            or x2 < 0 or x2 >= self.dimension
            or y1 < 0 or y1 >= self.dimension
            or y2 < 0 or y2 >= self.dimension):
            return False
        if self.cell[x, y, 0]:
            return False
        for i in range(seglen+1):
            if i != n and self.cell[x+(i-n)*dx, y+(i-n)*dy, 0] == False:
                return False
        for i in range(seglen):
            if self.cell[x+(i-n)*dx, y+(i-n)*dy, dir]:  # here one can add the non-touching rule
                return False        
        return True
    
    def computelegalmoves(self, seglen):
        moves = []
        for x in range(self.dimension):
            for y in range(self.dimension):
                for dir in range(1,5):
                    for n in range(seglen + 1):
                        if self.islegalmove((x,y, dir, n), seglen):
                            moves.append((x,y, dir, n))
        return moves
    
    def computelegalmovesaroundpt(self, p, seglen):
        moves = []
        x0, y0 = p
        for dir in range(1,5):
            dx, dy = directions[dir]
            for n in range(seglen + 1):
                for i in range(-1*seglen, seglen+1):
                    x = x0 + i*dx
                    y = y0 + i*dy
                    if self.islegalmove((x,y, dir, n), seglen):
                        moves.append((x,y, dir, n))
        return moves
                            
             


class Game:
    
    def __init__(self, grid, seglen = 4, moves=None, score = 0):
        self.grid = grid
        self.seglen = seglen # segment length: could be 3 or 4
        self.moves = moves
        self.score = score
    
    def print(self, size=6, color='k', marker='o', markersize=4):
        def pltfct(plt):
            plt.title('Score : {}      # legal moves: {}'.format(self.score,len(self.moves)))
        self.grid.print(size = size, color = color, marker = marker, markersize = markersize,
                        fct = pltfct)
    
    def play(self, fct, depth=-1):
        if depth == 0 or len(self.moves) == 0:
            return self
        index = fct(self)
        newgame = PlayingGame(self, self.moves[index])
        return newgame.play(fct, depth = depth - 1)


class StartingGame(Game):
    
    def __init__(self, startinggrid='cross', seglen = 4):
        Game.__init__(self, Grid(startinggrid), seglen=seglen, moves=None, score=0)
        self.moves = self.grid.computelegalmoves(seglen)


class PlayingGame(Game):
    
    def __init__(self, game, newmove):
        Game.__init__(self, game.grid.copy(), seglen = game.seglen,
                      score = game.score + 1)
#        self.lastgame = game    # later on add this as a pointer to the parent game
        self.grid.addsegment(newmove, self.seglen)
        self.moves = [m for m in game.moves
                      if self.grid.islegalmove(m, self.seglen)]
        self.moves.extend(self.grid.computelegalmovesaroundpt(newmove[0:2], self.seglen))



