import numpy as np
from typing import Tuple, List


# to do: implement class point instead of Tuple[int,int], so that operations can be done
class Point():
    x: int
    y: int

    pass


class Grid():
    """
    docstring
    """
    dimensions: Tuple[int, int]
    origin: Tuple[int, int]
    grid: np.ndarray
    size_increment: int

    def __init__(self,
                 dimensions: Tuple[int, int] = (20, 20),
                 origin: Tuple[int, int] = (5, 5),
                 size_increment: int = 5):
        w, h = dimensions
        self.dimensions = (3 * w, 3 * h)
        x, y = origin
        self.origin = (3 * x + 1, 3 * y + 1)
        self.grid = np.full(self.dimensions, False)
        self.size_increment = 3 * size_increment

    def get(self, point: Tuple[int, int]):
        x, y = point
        x0, y0 = self.origin
        return self.grid[x0 + x, y0 + y]

    def set(self, point: Tuple[int, int], value: bool = True):
        x, y = point
        x0, y0 = self.origin
        self.grid[x0 + x, y0 + x] = value

    def extend_left(self):
        # update dimensions
        w, h = self.dimensions
        self.dimensions = (w + self.size_increment, h)
        # update grid
        grid_copy = self.grid
        self.grid = np.full(self.dimensions, False)
        self.grid[self.size_increment:] = grid_copy
        # update origin
        x, y = self.origin
        self.origin = (x + self.size_increment, y)

    def extend_right(self):
        # update dimensions
        w, h = self.dimensions
        self.dimensions = (w + self.size_increment, h)
        # update grid
        grid_copy = self.grid
        self.grid = np.full(self.dimensions, False)
        self.grid[:w] = grid_copy

    def extend_bottom(self):
        # update dimensions
        w, h = self.dimensions
        self.dimensions = (w, h + self.size_increment)
        # update grid
        grid_copy = self.grid
        self.grid = np.full(self.dimensions, False)
        self.grid[:,self.size_increment:] = grid_copy
        # update origin
        x, y = self.origin
        self.origin = (x, y + self.size_increment)

    def extend_top(self):
        # update dimensions
        w, h = self.dimensions
        self.dimensions = (w, h + self.size_increment)
        # update grid
        grid_copy = self.grid
        self.grid = np.full(self.dimensions, False)
        self.grid[:,:h] = grid_copy


class Segment():

    p1: Tuple[int, int]
    p2: Tuple[int, int]
    dots: List[Point]
    lines: List[Point]

    def __init__(self, p1: Tuple[int, int], p2: Tuple[int, int], length: int):
        self.p1 = p1
        self.p2 = p2
        # to do: verify the length of the segment, and then generate a list of dots and lines through which the segment goes



# MAIN
if __name__ == '__main__':
    mygrid = Grid()
    print(mygrid.grid.shape)
    print(mygrid.dimensions)
    mygrid.extend_left()
    mygrid.extend_right()
    mygrid.extend_top()
    mygrid.extend_bottom()
    print(mygrid.grid.shape)
    print(mygrid.dimensions)
