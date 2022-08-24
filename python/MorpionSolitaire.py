import numpy as np
import json
from typing import List


class GridCoordinates:
    x: int
    y: int

    def __init__(self, x: int, y: int) -> None:
        self.x = x
        self.y = y

    def to_tuple(self) -> (int, int):
        return self.x, self.y


################################################
class ImageCoordinates:
    x: int
    y: int

    def __init__(self, *args) -> None:
        if isinstance(args[0], GridCoordinates):
            self.x = 3 * args[0].x
            self.y = 3 * args[0].y
        elif isinstance(args[0], int) and isinstance(args[1], int):
            self.x = args[0]
            self.y = args[1]

    def add(self, x, y) -> None:
        self.x += x
        self.y += y

    def to_tuple(self) -> (int, int):
        return self.x, self.y

    def to_grid_coordinates(self) -> GridCoordinates:
        return GridCoordinates(self.x // 3, self.y // 3)

    def is_dot(self) -> bool:
        if (self.x % 3 == 0) and (self.y % 3 == 0):
            return True
        return False


################################################

class ImageAction:
    pixels: List[ImageCoordinates]

    def __init__(self):
        self.pixels = []


################################################

class GridAction:
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

    def to_json(self) -> json:
        element_dictionary = {}
        if len(self.dots) > 0:
            element_dictionary['dots'] = [point.to_tuple() for point in self.dots]
        if len(self.line) > 0:
            element_dictionary['line'] = [point.to_tuple() for point in self.line]
        return json.dumps(element_dictionary)


################################################

class GameLink:
    grid_action: GridAction
    image_action: ImageAction

    def __init__(self):
        self.grid_action = GridAction()
        self.image_action = ImageAction()


################################################

class Image:
    dimensions: ImageCoordinates
    origin: ImageCoordinates
    image: np.ndarray  # can this eventually be inherited?
    size_increment: int

    def __init__(self, dimensions: GridCoordinates, origin: GridCoordinates,
                 size_increment: int = 1) -> None:
        self.dimensions = ImageCoordinates(dimensions)
        self.origin = ImageCoordinates(origin)
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

    def print(self) -> None:
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

    def is_valid(self, action: ImageAction) -> bool:
        for pt in action.pixels:
            if self.get(pt):
                return False
        return True

    def apply(self, action: ImageAction, value: bool) -> None:
        for pt in action.pixels:
            self.set(pt, value)

    def add(self, action: ImageAction) -> None:
        self.apply(action, True)

    def remove(self, action: ImageAction) -> None:
        self.apply(action, False)


################################################
class Grid:
    actions: List[GridAction]

    def __init__(self) -> None:
        self.actions = []

    def to_svg(self, image: Image,
               unit_cell_width: int = 40,
               grid_style: str = "stroke:rgb(127,127,127);stroke-width:1") -> List[str]:
        svg = []
        w, h = image.dimensions.to_grid_coordinates().to_tuple()
        x, y = image.origin.to_grid_coordinates().to_tuple()
        w += 1
        h += 1
        xmin = -0.5 - x
        ymin = -0.5 - y
        xmax = xmin + w
        ymax = ymin + h
        svg.append((
            f'<svg width="{unit_cell_width * w:d}" height="{unit_cell_width * h:d}" '
            f'viewbox="{xmin:.1f} {ymin:.1f} {w} {h}">'
        ))
        for i in range(w):
            svg.append((f'\t<line x1="{i - x}" y1="{ymin:.1f}" x2="{i - x}" y2="{ymax:.1f}" '
                        f'style="{grid_style}">'))
        for i in range(h):
            svg.append((f'\t<line x1="{xmin:.1f}" y1="{i - y}" x2="{xmax:.1f}" y2="{i - y}" '
                        f'style="{grid_style}">'))
        svg.append('</svg>')
        return svg


################################################

class Segment(GameLink):
    def __init__(self, p1: GridCoordinates, p2: GridCoordinates,
                 length: int, no_touching_rule: bool,
                 image: Image) -> None:
        if length == 0:
            raise Exception('Invalid segment length: 0')
        GameLink.__init__(self)
        w = p2.x - p1.x
        h = p2.y - p1.y
        if ((w != 0) and (abs(w) != length)) or ((h != 0) and (abs(h) != length)) or ((w == 0) and (h == 0)):
            raise Exception(
                'No segment can be defined between the points ({0},{1}) and ({2},{3})'.format(p1.x, p1.y, p2.x, p2.y))
        dx = w // length
        dy = h // length
        x, y = ImageCoordinates(p1).to_tuple()
        if not no_touching_rule:
            index_range = range(0, 3 * length + 1)
        else:
            index_range = range(-1, 3 * length + 2)
        pixels = [ImageCoordinates(x + i * dx, y + i * dy) for i in index_range]
        empty_dots_count = 0
        empty_dot = None
        for pt in pixels:
            if pt.is_dot():
                if not image.get(pt):
                    empty_dots_count += 1
                    empty_dot = pt
            else:
                if image.get(pt):
                    raise Exception('The segment cannot overlap existing lines')
        if empty_dots_count != 1:
            raise Exception('The segment must go through {0} existing dots exactly'.format(length))

        self.image_action.pixels.append(empty_dot)
        for pt in pixels:
            if not pt.is_dot():
                self.image_action.pixels.append(pt)
        self.grid_action.add_dot(
            GridCoordinates(empty_dot.x // 3,
                            empty_dot.y // 3))  # use dot.to_grid_coordinates() instead (to be implemented)
        self.grid_action.add_line(p1, p2)


################################################

class Cross(GameLink):

    def __init__(self):
        super().__init__()
        self.grid_action.dots = [
            GridCoordinates(3, 0), GridCoordinates(4, 0), GridCoordinates(5, 0), GridCoordinates(6, 0),
            GridCoordinates(3, 1), GridCoordinates(6, 1), GridCoordinates(3, 2), GridCoordinates(6, 2),
            GridCoordinates(0, 3), GridCoordinates(1, 3), GridCoordinates(2, 3), GridCoordinates(3, 3),
            GridCoordinates(6, 3), GridCoordinates(7, 3), GridCoordinates(8, 3), GridCoordinates(9, 3),
            GridCoordinates(0, 4), GridCoordinates(9, 4), GridCoordinates(0, 5), GridCoordinates(9, 5),
            GridCoordinates(0, 6), GridCoordinates(1, 6), GridCoordinates(2, 6), GridCoordinates(3, 6),
            GridCoordinates(6, 6), GridCoordinates(7, 6), GridCoordinates(8, 6), GridCoordinates(9, 6),
            GridCoordinates(3, 7), GridCoordinates(6, 7), GridCoordinates(3, 8), GridCoordinates(6, 8),
            GridCoordinates(3, 9), GridCoordinates(4, 9), GridCoordinates(5, 9), GridCoordinates(6, 9)]
        for dot in self.grid_action.dots:
            self.image_action.pixels.append(ImageCoordinates(dot))


################################################

class GameNode:
    root: GameLink
    branches: List[GameLink]

    pass


################################################
class Game:
    grid: Grid
    image: Image
    length: int
    index: int
    nodes: List[GameNode]  # can it be inherited instead?

    def __init__(self,
                 dimensions: GridCoordinates = GridCoordinates(20, 20),
                 origin: GridCoordinates = GridCoordinates(5, 5)
                 ):
        self.grid = Grid()
        self.image = Image(dimensions, origin)
        self.length = 0  # eventually 1
        self.index = 0
        self.nodes = []  # eventually a starting configuration

    def set_index(self, index: int):
        pass


################################################

class SvgImage:
    resolution: int
    width: int
    height: int
    elements: List[str]
    grid_style: str

    def __init__(self, grid: Grid, resolution: int = 20) -> None:
        self.resolution = resolution
        w, h = grid.dimensions.to_tuple()
        self.width = resolution * w + 1
        self.height = resolution * h + 1
        self.grid_style = "stroke:rgb(127,127,127);stroke-width:1"
        self.elements = []
        for x in range(w + 1):
            self.elements.append(
                f'<line x1="{resolution * x}" y1="0" x2="{resolution * x}" y2="{resolution * h}" style="{self.grid_style}"/>')
        for y in range(h + 1):
            self.elements.append(
                f'<line x1="0" y1="{resolution * y}" x2="{resolution * w}" y2="{resolution * y}" style="{self.grid_style}"/>')

    def add_grid(self, grid: Grid) -> None:
        w, h = grid.dimensions.to_tuple()
        for x in range(w + 1):
            self.elements.append(f'<line x1="{x}" y1="0" x2="{x}" y2="{h}" style="{self.grid_style}"')
        for y in range(h + 1):
            self.elements.append(f'<line x1="0" y1="{y}" x2="{w}" y2="{y}" style="{self.grid_style}"')

    def add_step(self, drawing: GridAction) -> None:
        pass

    def save(self, filename: str) -> None:
        with open(filename, 'w') as f:
            f.write('<html>\n')
            f.write('<body>\n')
            # f.write('<h1>Title</h1>\n')
            f.write(f'<svg width="{self.width:d}" height="{self.height:d}">\n')
            for e in self.elements:
                f.write('  ' + e + '\n')
            f.write('</svg>\n')
            f.write('</body>\n')
            f.write('</html>\n')
            print(f'File written to: {filename}')


################################################
# PLAYABLE COMMAND-LINE GAME

def command_line_game() -> None:
    image = Image(GridCoordinates(12, 12), GridCoordinates(1, 1))
    image.add(Cross().image_action)
    score = 0
    image.print()
    while True:
        print()
        print()
        print('Draw a segment:')
        x1 = int(input(' - start point: x:  '))
        y1 = int(input('                y:  '))
        x2 = int(input(' - end point: x:  '))
        y2 = int(input('              y:  '))
        try:
            segment = Segment(GridCoordinates(x1, y1), GridCoordinates(x2, y2), 4, False, image)
        except Exception as e:
            print('This is not a valid segment:', e)
        else:
            image.add(segment.image_action)
            score += 1
            print()
            image.print()
            print()
            print('Score:', score)


################################################
# TESTS

def tests() -> None:
    test_image = Image(dimensions=GridCoordinates(20, 20),
                       origin=GridCoordinates(5, 5))
    # print(test_image.dimensions.x, test_image.dimensions.y, test_image.image.shape)
    test_image.extend_left()
    test_image.extend_right()
    test_image.extend_top()
    test_image.extend_bottom()
    # print(test_image.dimensions.x, test_image.dimensions.y, test_image.image.shape)


################################################
# MAIN
if __name__ == '__main__':
    tests()
    # command_line_game()
    image = Image(dimensions=GridCoordinates(20, 20),
                  origin=GridCoordinates(5, 5))
    image = Image(dimensions=GridCoordinates(6, 6),
                  origin=GridCoordinates(1, 2))
    grid = Grid()
    html = grid.to_svg(image)
    for line in html:
        print(line)
    with open('drawing.html', 'w') as f:
        f.write('<html>\n')
        f.write('<body>\n')
        # f.write('<h1>Title</h1>\n')
        for line in html:
            f.write('\t' + line + '\n')
        f.write('</body>\n')
        f.write('</html>\n')
    # output = SvgImage(my_grid)
    # output.save('drawing.html')
