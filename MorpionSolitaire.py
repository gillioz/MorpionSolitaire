
import numpy as np
from PIL import Image
import matplotlib as mpl
import matplotlib.pyplot as plt
from PIL.DdsImagePlugin import DXGI_FORMAT_BC7_TYPELESS

class Grid:
    '''
    Array of cells, each containing a point and 4 lines in the horizontal,
    vertical, and both diagonal directions, following the conventions
     
      (4) (2) (3)
        \  |  /
         \ | /
          \|/ 
           X---(1)
          (0)
    '''
    
    DIMENSION = 22
    
    STARTING_GRIDS = {
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
    
    DIRECTIONS = ((0,0), (1, 0), (0, 1), (1,1), (-1,1))
    
    def __init__(self, pattern = ''):
        '''
        Constructor for the class Grid, returning either an empty grid
        or a pattern according to the dictionary STARTING_GRIDS
        '''
        self.cell = np.full((self.DIMENSION, self.DIMENSION, 5), False)
        if pattern in self.STARTING_GRIDS.keys():
            w, h = self.STARTING_GRIDS[pattern].shape
            x0 = (self.DIMENSION - w)//2
            y0 = (self.DIMENSION - h)//2
            self.cell[x0:x0+w,y0:y0+h,0] = self.STARTING_GRIDS[pattern].astype(bool)
    
    def copy(self):
        '''
        Deep copy of a grid, i.e. copy of its cells
        '''
        newgrid = Grid()
        newgrid.cell = np.copy(self.cell)
        return newgrid
	
    def print(self, size = 6, fct = None,
              color = 'k', marker = 'o', markersize = 4):
        '''
        Prints the Grid using matplotlib,
        and possibly add features with the function fct
        '''
        fig = plt.figure(figsize=(size, size))
        ax = fig.gca(aspect=1, autoscale_on = False,
                     xticks=range(self.DIMENSION), xticklabels = '',
                     yticks=range(self.DIMENSION), yticklabels = '')
        plt.grid()
        for x in range(self.DIMENSION):
            for y in range(self.DIMENSION):
                if self.cell[x,y,0]:
                    plt.plot(x, y, color = color,
                             marker = marker, markersize = markersize)
                for dir in range(1, 5):
                    if self.cell[x,y,dir]:
                        plt.plot([x,x+self.DIRECTIONS[dir][0]],
                                 [y,y+self.DIRECTIONS[dir][1]],
                                 color = color)
        if not(fct is None):
            fct(plt)
        plt.show()
    
    def image(self):
        '''
        Generates a PIL image from the grid, used for deep learning
        '''
        n = 3*self.DIMENSION - 2
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

    def add_segment(self, move, seg_len = 4,
                    touching_rule = True, check_legal = True):
        '''
        Adds a segment of length seg_len to the Grid.
        A segment is a 4-tuple (x, y, dir, n), where
         - (x, y) are the coordinates of the point to be added
         - dir is the direction according to the list DIRECTIONS
         - n is the position of the point on the segment
         
        By default this function verifies first is the move
        is a legal one, although this can be overriden.
        '''
        if check_legal:
            if self.is_legal(move, seg_len, touching_rule = touching_rule) == False:
                raise Exception('Trying to add a segment {} of length {} at an illegal position'.format(move, seg_len))
        x, y, dir, n = move
        dx, dy = self.DIRECTIONS[dir]
        self.cell[x, y, 0] = True
        for i in range(seg_len):
            self.cell[x+(i-n)*dx, y+(i-n)*dy, dir] = True
    
    def remove_segment(self, move, seg_len = 4):
        '''
        Removes a segment following the same convention as add_segment
        '''
        x, y, dir, n = move
        dx, dy = self.DIRECTIONS[dir]
        self.cell[x, y, 0] = False
        for i in range(seg_len):
            self.cell[x+(i-n)*dx, y+(i-n)*dy, dir] = False
    
    def is_legal(self, move, seg_len = 4, touching_rule = True):
        '''
        Verifies if a particular move (adding a segment)
        is legal according to the rules specified by
        seg_len and touch_rules
        '''
        x, y, dir, n = move
        dx, dy = self.DIRECTIONS[dir]
        x1 = x - n*dx
        y1 = y - n*dy
        x2 = x + (seg_len-n)*dx
        y2 = y + (seg_len-n)*dy
        L = seg_len
        if touching_rule == False:
            L = L + 2
            x1 = x1 - dx
            y1 = y1 - dy
            x2 = x2 + dx
            y2 = y2 + dy
        # check that the segment is completely inside the grid
        if (x1 < 0  or x1 >= self.DIMENSION
            or x2 < 0 or x2 >= self.DIMENSION
            or y1 < 0 or y1 >= self.DIMENSION
            or y2 < 0 or y2 >= self.DIMENSION):
            return False
        # check that the point is free
        if self.cell[x, y, 0]:
            return False
        # check that the other points are occupied
        for i in range(seg_len + 1):
            if i != n and self.cell[x+(i-n)*dx, y+(i-n)*dy, 0] == False:
                return False
        # check that the lines are free
        for i in range(L):
            if self.cell[x1 + i*dx, y1 + i*dy, dir]:
                return False        
        return True
    
    def compute_legal_moves(self, seg_len = 4, touching_rule = True):
        '''
        Computes the list of all legal moves.
        This method is inefficient: compute_legal_moves_around
        should be use instead when possible
        '''
        moves = []
        for x in range(self.DIMENSION):
            for y in range(self.DIMENSION):
                for dir in range(1,5):
                    for n in range(seg_len + 1):
                        if self.is_legal((x,y, dir, n), seg_len,
                                         touching_rule = touching_rule):
                            moves.append((x,y, dir, n))
        return moves
    
    def compute_legal_moves_around(self, p, seg_len = 4, touching_rule = True):
        '''
        Computes all the legal moves that are touching
        the point p
        '''
        moves = []
        x0, y0 = p
        for dir in range(1,5):
            dx, dy = self.DIRECTIONS[dir]
            for i in range(1, seg_len + 1):
                x = x0 + i*dx
                y = y0 + i*dy
                for n in range(i, seg_len + 1):
                    if self.is_legal((x,y, dir, n), seg_len,
                                     touching_rule = touching_rule):
                        moves.append((x,y, dir, n))
                x = x0 - i*dx
                y = y0 - i*dy
                for n in range(0, seg_len + 1 - i):
                    if self.is_legal((x,y, dir, n), seg_len,
                                     touching_rule = touching_rule):
                        moves.append((x,y, dir, n))
        return moves
                            
             


class Game:
    '''
    A Game consists in a Grid, a set of rules (seg_len and touching_rule),
    a list of legal moves, and a score.
    '''
    
    def __init__(self, grid, seg_len = 4, touching_rule = True,
                 moves = None, score = 0):
        '''
        The class Game is meant to be an abstract class:
        one should not use this constructor directly but use instead
        one of the child classes StartingGame or PlayingGame
        '''
        self.grid = grid
        self.seg_len = seg_len
        self.touching_rule = touching_rule
        self.moves = moves
        self.score = score
    
    def print(self, show_legal_moves = False,
              size = 6, color = 'k',
              marker = 'o', markersize = 4,
              legal_moves_color = 'b'):
        '''
        Displays the grid, together with the score and number of legal moves,
        and optionally all the legal moves
        '''
        def pltfct(plt):
            plt.title('Score : {}      # legal moves: {}'.format(self.score,len(self.moves)))
            if show_legal_moves:
                for move in self.moves:
                    x, y, dir, n = move
                    dx, dy = self.grid.DIRECTIONS[dir]
                    plt.plot(x, y, color = legal_moves_color,
                             marker = marker, markersize = markersize)
                    plt.plot([x - n*dx, x + (self.seg_len - n)*dx],
                             [y - n*dy, y + (self.seg_len - n)*dy],
                             color = legal_moves_color)
        self.grid.print(size = size, color = color, marker = marker, markersize = markersize,
                        fct = pltfct)
    
    def play(self, fct, depth = -1, check_legal = True):
        '''
        Recursively defines a new instance of Game, adding one segment at a time,
        until there are no more legal moves or a given depth is attained.
        The legal move is picked according to a function fct.
        
        The method can be optionally sped up by omitting to check
        whether a move is legal or not.
        '''
        if depth == 0 or len(self.moves) == 0:
            return self
        index = fct(self)
        newgame = PlayingGame(self, self.moves[index], check_legal)
        return newgame.play(fct, depth = depth - 1, check_legal = check_legal)


class StartingGame(Game):
    '''
    Child class of Game, to be used to initialize a game
    '''
    
    def __init__(self, pattern='cross', seg_len = 4, touching_rule = True):
        '''
        Constructor for the class StartingGame, taking as arguments
        the initial pattern and the rules
        '''
        Game.__init__(self, Grid(pattern),
                      seg_len = seg_len, touching_rule = touching_rule,
                      score = 0)
        self.moves = self.grid.compute_legal_moves(seg_len)

    def get_parent(self, depth):
        '''
        Returns the parent grid, which for a StartingGrid can only be itself
        '''
        if depth > 0:
            raise Exception('No parent at this depth')
        return self

class PlayingGame(Game):
    '''
    Child class of Game, automatically created when the method play is called
    '''
    
    def __init__(self, game, move, check_legal = True):
        '''
        Constructor for the class PlayinGame, taking as argument
        a reference game and a move
        '''
        Game.__init__(self, game.grid.copy(),
                      seg_len = game.seg_len, touching_rule = game.touching_rule,
                      score = game.score + 1)
        self.parent = game
        self.grid.add_segment(move, self.seg_len, check_legal = check_legal)
        self.moves = [m for m in game.moves
                      if self.grid.is_legal(m, self.seg_len, touching_rule = self.touching_rule)]
        self.moves.extend(self.grid.compute_legal_moves_around(move[0:2], self.seg_len,
                                                               touching_rule = self.touching_rule))


    def get_parent(self, depth):
        '''
        Gives access to the parent game at a given depth
        '''
        if depth > 0:
            return self.parent.get_parent(depth - 1)
        return self

