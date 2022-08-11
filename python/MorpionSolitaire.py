import numpy as np
import json
from typing import List


class ImageCoordinates:
    x: int
    y: int

    def __init__(self, x: int, y: int) -> None:
        self.x = x
        self.y = y

    # def __init__(self, coord: ImageCoordinates):
    #     self.x = coord.x
    #     self.y = coord.y

    def add(self, x, y) -> None:
        self.x += x
        self.y += y

    def to_tuple(self) -> (int, int):
        return self.x, self.y


################################################

class GridCoordinates:
    x: int
    y: int

    def __init__(self, x: int, y: int) -> None:
        self.x = x
        self.y = y

    # def __init__(self, coord: GridCoordinates):
    #     self.x = coord.x
    #     self.y = coord.y

    def to_image_coordinates(self) -> ImageCoordinates:
        return ImageCoordinates(3 * self.x, 3 * self.y)

    def to_tuple(self) -> (int, int):
        return self.x, self.y


################################################

class DrawingStep:
    dots: List[GridCoordinates]
    line: List[GridCoordinates]

    def __init__(self):
        self.dots = []
        self.line = []

    def add_line(self, p1: GridCoordinates, p2: GridCoordinates) -> None:
        self.line.append(p1)
        self.line.append(p2)

    def add_dot(self, p: GridCoordinates) -> None:
        self.dots.append(p)

    def json_description(self) -> json:
        element_dictionary = {}
        if len(self.dots) > 0:
            element_dictionary['dots'] = [point.to_tuple() for point in self.dots]
        if len(self.line) > 0:
            element_dictionary['line'] = [point.to_tuple() for point in self.line]
        return json.dumps(element_dictionary)

################################################

class DrawingCross(DrawingStep):

    def __init__(self):
        super().__init__()
        self.dots = [
            GridCoordinates(3, 0), GridCoordinates(4, 0), GridCoordinates(5, 0), GridCoordinates(6, 0),
            GridCoordinates(3, 1), GridCoordinates(6, 1), GridCoordinates(3, 2), GridCoordinates(6, 2),
            GridCoordinates(0, 3), GridCoordinates(1, 3), GridCoordinates(2, 3), GridCoordinates(3, 3),
            GridCoordinates(6, 3), GridCoordinates(7, 3), GridCoordinates(8, 3), GridCoordinates(9, 3),
            GridCoordinates(0, 4), GridCoordinates(9, 4), GridCoordinates(0, 5), GridCoordinates(9, 5),
            GridCoordinates(0, 6), GridCoordinates(1, 6), GridCoordinates(2, 6), GridCoordinates(3, 6),
            GridCoordinates(6, 6), GridCoordinates(7, 6), GridCoordinates(8, 6), GridCoordinates(9, 6),
            GridCoordinates(3, 7), GridCoordinates(6, 7), GridCoordinates(3, 8), GridCoordinates(6, 8),
            GridCoordinates(3, 9), GridCoordinates(4, 9), GridCoordinates(5, 9), GridCoordinates(6, 9)]
        self.line = []

################################################

class Segment(DrawingStep):
    image_dots: List[ImageCoordinates]
    image_lines: List[ImageCoordinates]
    valid: bool

    def __init__(self, p1: GridCoordinates, p2: GridCoordinates, length: int, no_touching_rule: bool) -> None:
        if (length == 0):
            raise Exception('Invalid segment length: 0')
        DrawingStep.__init__(self)
        self.add_line(p1, p2)
        self.image_dots = []
        self.image_lines = []
        self.valid = False
        w = p2.x - p1.x
        h = p2.y - p1.y
        if ((w != 0) and (abs(w) != length)) or ((h != 0) and (abs(h) != length)) or ((w == 0) and (h == 0)):
            raise Exception(
                'No segment can be defined between the points ({0},{1}) and ({2},{3})'.format(p1.x, p1.y, p2.x, p2.y))
        dx = w // length
        dy = h // length
        x, y = p1.to_image_coordinates().to_tuple()
        if not no_touching_rule:
            index_range = range(0, 3 * length + 1)
        else:
            index_range = range(-1, 3 * length + 2)
        for index in index_range:
            p = ImageCoordinates(x + index * dx, y + index * dy)
            if index % 3 == 0:
                self.image_dots.append(p)
            else:
                self.image_lines.append(p)

    def validate(self, dot: ImageCoordinates) -> None:
        self.image_lines = [dot]
        self.add_dot(GridCoordinates(dot.x // 3, dot.y //3))    # use dot.to_grid_coordinates() instead (to be implemented)
        self.valid = True


################################################

class Grid:
    """
    docstring
    """
    dimensions: ImageCoordinates
    origin: ImageCoordinates
    image: np.ndarray
    size_increment: int

    def __init__(self,
                 dimensions: GridCoordinates = GridCoordinates(20, 20),
                 origin: GridCoordinates = GridCoordinates(5, 5),
                 size_increment: int = 1) -> None:
        self.dimensions = dimensions.to_image_coordinates()
        self.origin = origin.to_image_coordinates()
        self.origin.add(1, 1)
        self.create_empty_image()
        self.size_increment = 3 * size_increment

    def create_empty_image(self) -> None:
        self.image = np.full((self.dimensions.x, self.dimensions.y), False)

    def get(self, point: ImageCoordinates) -> bool:
        x = self.origin.x + point.x
        y = self.origin.y + point.y
        w, h = self.image.shape
        if x < 0 or x >= w:
            return False
        if y < 0 or y >= h:
            return False
        return self.image[x, y]

    def set(self, point: ImageCoordinates, value: bool) -> None:
        x = self.origin.x + point.x
        y = self.origin.y + point.y
        w, h = self.image.shape
        if x < 0:
            self.extend_left()
            self.set(point, value)
            return None
        if x >= w:
            self.extend_right()
            self.set(point, value)
            return None
        if y < 0:
            self.extend_bottom()
            self.set(point, value)
            return None
        if y >= h:
            self.extend_top()
            self.set(point, value)
            return None
        self.image[x, y] = value
        # add here: trigger for grid extension

    def extend_left(self) -> None:
        # update dimensions
        self.dimensions.x += self.size_increment
        # update grid
        image_copy = self.image
        self.create_empty_image()
        self.image[self.size_increment:] = image_copy
        # update origin
        self.origin.x += self.size_increment

    def extend_right(self) -> None:
        # update dimensions
        self.dimensions.x += self.size_increment
        # update grid
        image_copy = self.image
        self.create_empty_image()
        self.image[:self.dimensions.x - self.size_increment] = image_copy

    def extend_bottom(self) -> None:
        # update dimensions
        self.dimensions.y += self.size_increment
        # update grid
        image_copy = self.image
        self.create_empty_image()
        self.image[:, self.size_increment:] = image_copy
        # update origin
        self.origin.y += self.size_increment

    def extend_top(self) -> None:
        # update dimensions
        self.dimensions.y += self.size_increment
        # update grid
        image_copy = self.image
        self.create_empty_image()
        self.image[:, :self.dimensions.y - self.size_increment] = image_copy

    def print_image_as_text(self) -> None:
        w, h = self.image.shape
        contour_row = '+'
        for x in range(w):
            contour_row += '~'
        contour_row += '+'
        print(contour_row)
        for y in reversed(range(h)):
            row = 's'
            for x in range(w):
                if self.image[x, y]:
                    x0 = x % 3
                    y0 = y % 3
                    if x0 == 1:
                        if y0 == 1:
                            row += 'o'
                        else:
                            row += '|'
                    else:
                        if y0 == 1:
                            row += '-'
                        else:
                            if (x0 + y0) % 4 == 0:
                                row += '/'
                            else:
                                row += '\\'
                else:
                    row += ' '
            row += 's'
            print(row)
        print(contour_row)

    def validate(self, segment: Segment) -> bool:
        if segment.valid:
            return False
        for pt in segment.image_lines:
            if self.get(pt):
                return False
        dots_count = 0
        empty_dot = None
        for pt in segment.image_dots:
            if not self.get(pt):
                dots_count += 1
                empty_dot = pt
        segment.validate(empty_dot)
        return True

    def apply(self, segment: Segment, value: bool) -> None:
        for pt in segment.image_lines:
            self.set(pt, value)
        for pt in segment.image_dots:
            self.set(pt, value)

    def add(self, segment: Segment) -> None:
        self.apply(segment, True)

    def remove(self, segment: Segment) -> None:
        self.apply(segment, False)

    def add_points(self, points: List[GridCoordinates]) -> None:
        for pt in points:
            self.set(pt.to_image_coordinates(), True)


################################################
# TESTS

def tests() -> None:
    test_grid = Grid()
    test_grid.extend_left()
    test_grid.extend_right()
    test_grid.extend_top()
    test_grid.extend_bottom()
    # print(test_grid.dimensions.x, test_grid.dimensions.y, test_grid.image.shape)


################################################
# MAIN
if __name__ == '__main__':
    tests()
    my_grid = Grid(dimensions=GridCoordinates(6, 6),
                   origin=GridCoordinates(0,0))
    # my_grid.add(my_segment)
    my_grid.print_image_as_text()
    my_grid.add_points(DrawingCross().dots)
    my_grid.print_image_as_text()

    my_segment = Segment(GridCoordinates(4, 0), GridCoordinates(0, 4), 4, True)
    my_segment = Segment(GridCoordinates(1, 1), GridCoordinates(1, 5), 4, True)
    print(my_grid.validate(my_segment))
    print(my_segment.json_description())
    my_grid.add(my_segment)
    my_grid.print_image_as_text()




