
import numpy as np
import matplotlib as mpl
import matplotlib.pyplot as plt

class Grid(np.ndarray):
    '''
    A grid is an array of size 32 x 32, with 5 channels corresponding to
    a point and 4 lines forming the unit cell, following the convention
     
      (4) (2) (3)
        \  |  /
         \ | /
          \|/ 
           X---(1)
          (0)
    '''
    
    GRID_SIZE = 32
    
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
    
    def __new__(cls, *args):
        return np.full((5, cls.GRID_SIZE, cls.GRID_SIZE), False).view(cls)
    
    def __init__(self, pattern = ''):
        '''
        Constructor for the class Grid, returning either an empty grid
        or a pattern according to the dictionary STARTING_GRIDS
        '''
        if pattern in self.STARTING_GRIDS.keys():
            w, h = self.STARTING_GRIDS[pattern].shape
            x0 = (self.GRID_SIZE - w)//2
            y0 = (self.GRID_SIZE - h)//2
            self[0,x0:x0+w,y0:y0+h] = self.STARTING_GRIDS[pattern].astype(bool)
	
    def print(self, view = 20, figsize = 6, fct = None,
              color = 'k', marker = 'o', markersize = 4):
        '''
        Prints the Grid using matplotlib,
        and possibly add features with the function fct
        
        Note that by default the view is cropped to a grid of 20x20,
        as most of the grids contain many empty cells along the boundary
        '''
        if view > self.GRID_SIZE:
            view = self.GRID_SIZE
        offset = (self.GRID_SIZE - view) // 2
        viewwindow = (offset, offset + view - 1)
        viewrange = range(offset, offset + view)
        fig = plt.figure(figsize=(figsize, figsize))
        ax = fig.gca(aspect=1, autoscale_on = False,
                     xlim = viewwindow, ylim = viewwindow,
                     xticks=viewrange, xticklabels = '',
                     yticks=viewrange, yticklabels = '')
        plt.grid()
        for x in viewrange:
            for y in viewrange:
                if self[0, x, y]:
                    plt.plot(x, y, color = color,
                             marker = marker, markersize = markersize)
                for dir in range(1, 5):
                    if self[dir, x, y]:
                        plt.plot([x,x+self.DIRECTIONS[dir][0]],
                                 [y,y+self.DIRECTIONS[dir][1]],
                                 color = color)
        if not(fct is None):
            fct(plt)
        plt.show()
    
    def add_segment(self, move, seg_len = 4,
                    touching_rule = True, check_legal = True):
        '''
        Adds a segment of length seg_len to the Grid.
        A segment is a 4-tuple (x, y, dir, n), where
         - (x, y) are the coordinates of the point to be added
         - dir is the direction according to the list DIRECTIONS
         - n is the position of the point on the segment
         
        By default this function verifies first is the move
        is a legal one, although this step can be omitted.
        '''
        if check_legal:
            if self.is_legal(move, seg_len, touching_rule = touching_rule) == False:
                raise Exception('Trying to add a segment {} of length {} at an illegal position'.format(move, seg_len))
        x, y, dir, n = move
        dx, dy = self.DIRECTIONS[dir]
        self[0, x, y] = True
        for i in range(seg_len):
            self[dir, x+(i-n)*dx, y+(i-n)*dy] = True
    
    def remove_segment(self, move, seg_len = 4):
        '''
        Removes a segment following the same convention as add_segment
        '''
        x, y, dir, n = move
        dx, dy = self.DIRECTIONS[dir]
        self[0, x, y] = False
        for i in range(seg_len):
            self[dir, x+(i-n)*dx, y+(i-n)*dy] = False
    
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
        # including the empty parts to be checked at each end if touching_rule = False
        if (x1 < 0  or x1 >= self.GRID_SIZE
            or x2 < 0 or x2 >= self.GRID_SIZE
            or y1 < 0 or y1 >= self.GRID_SIZE
            or y2 < 0 or y2 >= self.GRID_SIZE):
            return False
        # check that the point is free
        if self[0, x, y]:
            return False
        # check that the other points are occupied
        for i in range(seg_len + 1):
            if i != n and self[0, x+(i-n)*dx, y+(i-n)*dy] == False:
                return False
        # check that the lines are free
        for i in range(L):
            if self[dir, x1 + i*dx, y1 + i*dy]:
                return False        
        return True
    
    def compute_legal_moves(self, seg_len = 4, touching_rule = True):
        '''
        Computes the list of all legal moves.
        This method is inefficient: compute_legal_moves_around
        should be use instead when possible
        '''
        moves = []
        for x in range(self.GRID_SIZE):
            for y in range(self.GRID_SIZE):
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
    
    def __init__(self, grid = None, seg_len = 4, touching_rule = True,
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
              view = 20, figsize = 6, color = 'k',
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
        self.grid.print(view = view, figsize = figsize,
                        color = color, marker = marker,
                        markersize = markersize,
                        fct = pltfct)
    
    def play(self, index = None, model = None, t = 0.0, depth = -1,
             copy_grid = False):
        '''
        Recursively defines a new instance of Game, adding one segment at a time,
        until there are no more legal moves or a given depth is attained.
        
        The argument index indicates which legal move to choose.
        If index is None, then the move is chosen according to a model.
        If model is None, then the move is taken at random.
        
        A model determines the index from a weighted distribution
        with a probability that depends on the "temperature" t:
        - when t is zero, the best move is chosen
        - when t is non-zero, the probability of each move is given by
          exp(w/t) where w is the weight
        '''
        if depth == 0 or len(self.moves) == 0:
            return self
        if index is None:
            if model is None:
                i = np.random.randint(len(self.moves))
            else:
                weights = self.compute_weights(model)
                if t > 0.0:
                    # the next 3 lines of code simply compute exp(weights/t)
                    # but in a manner that is numerically safe 
                    weights = weights/t
                    # normalize the weights so that the maximum is 1 after exponentiating
                    weights = weights - max(weights)
                    # therefore we can set to zero any weight that is too small 
                    weights = np.where(weights < -20.0, 0.0, np.exp(weights))
                    probs = weights/weights.sum()
                    i = np.random.choice(range(len(weights)),
                                         p = probs)
                else:
                    i = np.argmax(weights)
        else:
            i = index
        if copy_grid:
            newgrid = self.grid.copy()
        newgame = PlayingGame(self, self.moves[i])
        if copy_grid:
            self.grid = newgrid
        return newgame.play(index = index, model = model, t = t, depth = depth - 1)
    
    def compute_weights(self, model):
        '''
        Compute weights for each legal move by applying a model
        to the grid corresponding to this move.
        
        The model takes as input an array of n times the grid's shape
        and must returns another array of shape n
        (n being the number of legal moves)
        '''
        grids = []
        for move in self.moves:
            self.grid.add_segment(move, seg_len = self.seg_len,
                                  touching_rule = self.touching_rule,
                                  check_legal = False)
            grids.append(np.array(self.grid.copy()))
            self.grid.remove_segment(move, seg_len = self.seg_len)
        grids = np.array(grids)
        return model(grids)
    


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
        self.pattern = pattern
        self.moves = self.grid.compute_legal_moves(seg_len)

    def unplay(self, depth):
        '''
        Returns the parent game, which for a StartingGrid is the game itself
        '''
        if depth > 0:
            raise Exception('No parent at this depth')
        return self
    
    def sequence(self):
        '''
        For a starting game, sequence returns a string that contains the game info
        '''
        string = '# pattern: {} | seg_len: {} | touching_allowed:{}'
        return [string.format(self.pattern, self.seg_len, self.touching_rule)]
    
    def games_list(self, grid = None):
        '''
        Returns a list containing the current game, constructing the grid if necessary
        '''
        if grid is not None:
            self.grid = grid
        return [self]



class PlayingGame(Game):
    '''
    Child class of Game, automatically created when the method play is called
    '''
    
    def __init__(self, game, move):
        '''
        Constructor for the class PlayinGame, taking as argument
        a reference game and a move
        '''
        Game.__init__(self, game.grid,
                      seg_len = game.seg_len, touching_rule = game.touching_rule,
                      score = game.score + 1)
        self.parent = game
        self.last_move = move
        self.grid.add_segment(move, self.seg_len, check_legal = False)
        self.moves = [m for m in game.moves
                      if self.grid.is_legal(m, self.seg_len, touching_rule = self.touching_rule)]
        self.moves.extend(self.grid.compute_legal_moves_around(move[0:2], self.seg_len,
                                                               touching_rule = self.touching_rule))


    def unplay(self, depth):
        '''
        Undo the last steps and gives access to the parent game at a given depth
        '''
        if depth > 0:
            self.grid.remove_segment(self.last_move, seg_len = self.seg_len)
            # 'delete' the current game by emptying its score and moves
            self.score = -1
            self.moves = []
            return self.parent.unplay(depth - 1)
        return self
    
    def sequence(self):
        '''
        Returns a list containing the score, number of legal moves, and last move 
        for all the steps leading to the current game
        '''
        list = self.parent.sequence()
        list.append([self.score, len(self.moves), self.last_move])
        return list
    
    def games_list(self, grid = None):
        '''
        Returns a list of all the games corresponding to intermediate steps,
        constructing the grid if necessary
        '''
        if grid is not None:
            self.grid = grid
        grid_copy = self.grid.copy()
        grid_copy.remove_segment(self.last_move, seg_len = self.seg_len)    
        list = self.parent.games_list(grid_copy)
        list.append(self)
        return list
