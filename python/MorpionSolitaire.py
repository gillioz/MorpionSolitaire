
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
    
    def __init__(self, pattern = '', n_points = 36, width = 10, db = ''):
        '''
        Constructor for the class Grid, returning either
         - an empty grid, or
         - a pattern from the dictionary STARTING_GRIDS, or
         - a random pattern with a given number of points and a given width
        '''
        if pattern in self.STARTING_GRIDS.keys():
            w, h = self.STARTING_GRIDS[pattern].shape
            x0 = (self.GRID_SIZE - w)//2
            y0 = (self.GRID_SIZE - h)//2
            self[0,x0:x0+w,y0:y0+h] = self.STARTING_GRIDS[pattern].astype(bool)
        elif pattern == 'random':
            if n_points > width * width:
                raise Exception('There is no space to have {0} points in a {1}x{1} grid'.format(n_points, width))
            x0 = (self.GRID_SIZE - width)//2
            i = 0
            while i < n_points:
                x, y = x0 + np.random.randint(width, size = 2)
                if self[0,x,y] == False:
                    self[0,x,y] = True
                    i = i + 1
            
	
    def print(self, view = 20, figsize = 6,
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
        fig.add_subplot(aspect=1, autoscale_on=False,
                        xlim=viewwindow, ylim=viewwindow,
                        xticks=viewrange, xticklabels='',
                        yticks=viewrange, yticklabels='')
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
    
    def image(self):
        '''
        Returns a square array of booleans (0 or 1)
        representing the current grid
        '''
        size = 3 * self.GRID_SIZE - 2
        im = np.empty((size, size), dtype=bool)
        im[0::3,0::3] = self[0,:,:]
        im[1::3,0::3] = self[1,:-1,:]
        im[2::3,0::3] = self[1,:-1,:]
        im[0::3,1::3] = self[2,:,:-1]
        im[0::3,2::3] = self[2,:,:-1]
        im[1::3,1::3] = self[3,:-1,:-1]
        im[2::3,2::3] = self[3,:-1,:-1]
        im[1::3,2::3] = self[4,1:,:-1]
        im[2::3,1::3] = self[4,1:,:-1]
        return im
        
    
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
        The class Game is an abstract class: one should not use this
        constructor directly (it raises an exception) but use instead
        one of the child classes NewGame or RunningGame
        '''
        raise Exception('The class "Game" is an abstract class: it cannot be instantiated.')
    
    def print(self, show_legal_moves = False,
              legal_moves_color = 'b', legal_moves_marker = 'o',
              legal_moves_markersize = 4, **kwargs):
        '''
        Displays the grid, together with the score and number of legal moves,
        and optionally all the legal moves
        '''
        self.grid.print(**kwargs)
        plt.title('Score : {}      # legal moves: {}'.format(self.score,len(self.moves)))
        if show_legal_moves:
            for move in self.moves:
                x, y, dir, n = move
                dx, dy = self.grid.DIRECTIONS[dir]
                plt.plot(x, y, color = legal_moves_color,
                         marker = legal_moves_marker,
                         markersize = legal_moves_markersize    )
                plt.plot([x - n*dx, x + (self.seg_len - n)*dx],
                         [y - n*dy, y + (self.seg_len - n)*dy],
                         color = legal_moves_color)
    
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
            if len(self.moves) == 1:
                i = 0
            elif model is None:
                i = np.random.randint(len(self.moves))
            else:
                weights = self.compute_weights(model)
                if t > 0.0:
                    # the next 3 lines of code simply compute exp(weights/t)
                    # but in a manner that is numerically safe 
                    weights = weights/t
                    # normalize the weights so that the maximum is 0 after exponentiating
                    weights = weights - max(weights)
                    # set to zero any weight that is too small 
                    weights = np.where(weights < -20.0, 0.0, np.exp(weights))
                    # normalize
                    probs = weights/weights.sum()
                    i = np.random.choice(range(len(weights)), p = probs)
                else:
                    i = np.argmax(weights)
        else:
            i = index % len(self.moves)
        if copy_grid:
            newgrid = self.grid.copy()
        newgame = RunningGame(self, self.moves[i])
        if copy_grid:
            self.grid = newgrid
        return newgame.play(index = index, model = model, t = t, depth = depth - 1)
    
    def explore_depth(self, depth):
        '''
        Performs a systematic exploration of the possible moves
        and returns a number between zero and depth
        corresponding to the maximal number of moves that is allowed.
        
        WARNING: This method is computationally intensive when called
                 with a large depth.
        '''
        if depth == 0 or len(self.moves) == 0:
            return 0
        if depth == 1:
            return 1
        max_depth = 1
        currentgrid = self.grid.copy()
        for move in self.moves:
            max_depth = max(max_depth, RunningGame(self, move).explore_depth(depth - 1) + 1)
            self.grid = currentgrid.copy()
            if max_depth >= depth:
                return depth
        return max_depth
        
    
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
            grids.append(np.array(self.grid.image()))
            self.grid.remove_segment(move, seg_len = self.seg_len)
        grids = np.array(grids)
        return model(grids)
    
    def save(self, filename):
        '''
        Saves the game data as "filename_type.npy",
        so that it can later be loaded again
        '''
        file = filename + '_' + self.starting_pattern() + str(self.seg_len + 1)
        if self.touching_rule == False:
            file += 'D'
        file += '.npy'
        moves = np.array(self.moves_list())
        np.save(file, moves)
        print('Game saved to:' , file)
        return


class NewGame(Game):
    '''
    Child class of Game, to be used to initialize a game
    '''
    
    STARTING_MOVES = {
        'cross': [(10, 14, 1, 0), (10, 17, 1, 0), (11, 13, 2, 0), (11, 18, 2, 4),
                  (13, 11, 1, 0), (13, 13, 4, 2), (13, 18, 3, 2), (13, 20, 1, 0),
                  (14, 10, 2, 0), (14, 15, 2, 4), (14, 16, 2, 0), (14, 21, 2, 4),
                  (15, 14, 1, 4), (15, 17, 1, 4), (16, 14, 1, 0), (16, 17, 1, 0),
                  (17, 10, 2, 0), (17, 15, 2, 4), (17, 16, 2, 0), (17, 21, 2, 4),
                  (18, 11, 1, 4), (18, 13, 3, 2), (18, 18, 4, 2), (18, 20, 1, 4),
                  (20, 13, 2, 0), (20, 18, 2, 4), (21, 14, 1, 4), (21, 17, 1, 4)],
        'pipe': [(10, 15, 4, 4), (10, 16, 3, 0), (11, 13, 2, 0), (11, 18, 2, 4),
                 (13, 11, 1, 0), (13, 14, 1, 0), (13, 17, 1, 0), (13, 20, 1, 0),
                 (14, 13, 2, 0), (14, 18, 2, 4), (15, 10, 4, 0), (15, 21, 3, 4),
                 (16, 10, 3, 0), (16, 21, 4, 4), (17, 13, 2, 0), (17, 18, 2, 4),
                 (18, 11, 1, 4), (18, 14, 1, 4), (18, 17, 1, 4), (18, 20, 1, 4),
                 (20, 13, 2, 0), (20, 18, 2, 4), (21, 15, 3, 4), (21, 16, 4, 0)]
    }
    
    def __init__(self, pattern='cross', seg_len = 4, touching_rule = True,
                 iteration = 100):
        '''
        Constructor for the class NewGame, taking as arguments
        the initial pattern and the rules
        '''
        self.grid = Grid(pattern)
        self.seg_len = seg_len
        self.touching_rule = touching_rule
        self.score = 0
        self.pattern = pattern
        if pattern in self.STARTING_MOVES.keys():
            self.moves = self.STARTING_MOVES[pattern]
        else:
            self.moves = self.grid.compute_legal_moves(seg_len)
        # in case the pattern is random, there is a chance that the game returns no legal moves:
        # if this is the case, re-initialize the game (this is done at most a 100 times)
        if len(self.moves) == 0 and iteration > 0:
            self.__init__(pattern, seg_len, touching_rule, iteration - 1)

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
    
    def games_list(self):
        '''
        Returns a list containing the current game
        '''
        return [self]
    
    def moves_list(self):
        '''
        Returns a list of moves that led to the current game,
        i.e. an empty list
        '''
        return []
    
    def starting_pattern(self):
        '''
        Gets the starting pattern of the game
        '''
        return self.pattern
    
    def load(self, filename):
        '''
        Creates a sequence of games
        following the list of moves stored in "filename",
        and returns the last game
        '''
        moves = np.load(filename)
        game = self
        for m in moves:
            game = RunningGame(game, m)
        return game


class RunningGame(Game):
    '''
    Child class of Game, automatically created when the method play is called
    '''
    
    def __init__(self, game, move):
        '''
        Constructor for the class PlayinGame, taking as argument
        a reference game and a move
        '''
        self.grid = game.grid
        game.grid = None
        self.seg_len = game.seg_len
        self.touching_rule = game.touching_rule
        self.score = game.score + 1
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
            self.parent.grid = self.grid
            self.grid = None
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
    
    def games_list(self):
        '''
        Returns a list of all the games corresponding to intermediate steps,
        constructing the grid if necessary
        '''
        if self.parent.grid is None:
            self.parent.grid = self.grid.copy()
            self.parent.grid.remove_segment(self.last_move, seg_len = self.seg_len)
        list = self.parent.games_list()
        list.append(self)
        return list
    
    def moves_list(self):
        '''
        Returns a list of moves that led to the current game
        '''
        list = self.parent.moves_list()
        list.append(self.last_move)
        return list

    def starting_pattern(self):
        '''
        Gets the starting pattern of the game,
        stored in an instance of a parent NewGame
        '''
        return self.parent.starting_pattern()
    