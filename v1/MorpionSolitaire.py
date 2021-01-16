
from PIL import Image, ImageDraw, ImageFont
from random import choice, shuffle


#####################################################

# The class Grid is used only for drawing:

class Grid:

	def __init__(self, rules):
		self.rules = rules
			# set of rules specifying dimensions of the grid, length of a segment and directions
		self.points = [[-1 for y in range(self.rules.dim)] for x in range(self.rules.dim)]
			# format: -1 means empty, 0 starting point, positive number point added at level n
		self.lines = []
			# format: (x1, y1, x2, y2)
		self.arrows = []
			# format: (x, y, vx, vy)
		self.legalmoves = []
	
	def draw(self):
		# create image
		imagesize = 40*self.rules.dim
		image = Image.new(mode='RGB', size=(imagesize, imagesize), color='white')
		drawing = ImageDraw.Draw(image)
		textfont = ImageFont.truetype('/usr/share/fonts/truetype/freefont/FreeSerif.ttf', 12)
		# define a lattice of points
		p = [(i + 0.5)*image.width/self.rules.dim for i in range(self.rules.dim)]
		# draw the grid
		for i in range(self.rules.dim):
			drawing.line(((p[i], 0), (p[i], image.height)), fill='blue')
			drawing.line(((0, p[i]), (image.width, p[i])), fill='blue')		
		# draw the lines
		linewidth = 4
		for line in self.lines:
			drawing.line(((p[line[0]], p[line[1]]), (p[line[2]], p[line[3]])),
			fill='black', width=linewidth)
		# draw the arrows
		arrowwidth = 4
		arrowlength = 16
		for arrow in self.arrows:
			if self.points[arrow[0]][arrow[1]] > 0:
				if arrow[2]*arrow[3] == 0:	# horizontal or vertical line
					arrowwidth = 12
					arrowlength = 24
				else:						# diagonal line
					arrowwidth = 8
					arrowlength = 16
			else:
				if arrow[2]*arrow[3] == 0:	# horizontal or vertical line
					arrowwidth = 6
					arrowlength = 15
				else:						# diagonal line
					arrowwidth = 4
					arrowlength = 10
			drawing.polygon([
				(p[arrow[0]] + arrowwidth * arrow[3], p[arrow[1]] - arrowwidth * arrow[2]),
				(p[arrow[0]] + arrowlength * arrow[2], p[arrow[1]] + arrowlength * arrow[3]),
				(p[arrow[0]] - arrowwidth * arrow[3], p[arrow[1]] + arrowwidth * arrow[2])],
				fill = 'black')
		# draw the points
		pointradius = 6
		circleinnerradius = 10
		circleouterradius = 12
		for x in range(self.rules.dim):
			for y in range(self.rules.dim):
				if self.points[x][y] == 0:
					drawing.ellipse((p[x] - pointradius, p[y] - pointradius,
						p[x] + pointradius, p[y] + pointradius),
						fill = 'black')
				elif self.points[x][y] > 0:
					drawing.ellipse(
						(p[x] - circleouterradius, p[y] - circleouterradius,
						p[x] + circleouterradius, p[y] + circleouterradius),
						fill = 'black')
					drawing.ellipse(
						(p[x] - circleinnerradius, p[y] - circleinnerradius,
						p[x] + circleinnerradius, p[y] + circleinnerradius),
						fill = 'white')
					label = str(self.points[x][y])
					w, h = drawing.textsize(label, font = textfont)
					drawing.text((p[x] - w/2, p[y] - h/2), label,
						font = textfont, fill = 'black')
		# draw the legal moves
		for (x,y) in self.legalmoves:
			drawing.ellipse((p[x] - pointradius, p[y] - pointradius,
				p[x] + pointradius, p[y] + pointradius),
				fill = 'LightGray')
		# display the drawing
		image.show()
			
	def addpoint(self, x, y):
		self.points[x][y] = 0
		
	def addline(self, x1, y1, x2, y2):
		self.lines.append((x1, y1, x2, y2))
	
	def addsegment(self, segment, level):
		x0, y0, d, n = segment
		dx, dy = self.rules.vector[d]
		x1 = x0 - dx * n
		y1 = y0 - dy * n
		x2 = (x0 + dx * (self.rules.segmentlength - n))
		y2 = (y0 + dy * (self.rules.segmentlength - n))
		self.points[x0][y0] = level
		self.lines.append((x1, y1, x2, y2))
		if n > 0:
#			self.arrows.append((x1, y1, dx, dy))
			self.arrows.append((x0, y0, -1 * dx, -1 * dy))
		if n < self.rules.segmentlength:
			self.arrows.append((x0, y0, dx, dy))
#			self.arrows.append((x2, y2, -1 * dx, -1 * dy))

	def addlegalmove(self, x, y):
		self.legalmoves.append((x,y))

#####################################################

# stores a configuration at a given evolution time; the class Graph does not know about its history

class Graph:

	def __init__(self, rules):
		self.rules = rules
		self.cell = [[[0 for i in range(5)] for y in range(self.rules.dim)]
			for x in range(self.rules.dim)]

	def copy(self):
		new = Graph(self.rules)
		for x in range(self.rules.dim):
			for y in range(self.rules.dim):
				for i in range(5):
					new.cell[x][y][i] = self.cell[x][y][i]
		return new
		
	def addsegment(self, segment):
		x, y, d, n = segment
		self.cell[x][y][0] = 1
		dx, dy = self.rules.vector[d]
		x0 = x - n * dx
		y0 = y - n * dy
		for i in range(self.rules.segmentlength):
			self.cell[x0 + i * dx][y0 + i * dy][d + 1] = 1
	
	def drawcross(self, origin):
		x, y = origin
		n = self.rules.segmentlength
		# check that the cross fits in the graph:
		if x < 0 or y < 0 or x + 3*n - 3 >= self.rules.dim or y + 3*n - 3 >= self.rules.dim:
			print('drawcross: The cross does not fit in the graph, no cross drawn')
			return
		for i in range(n):
			self.cell[x][y + n - 1 + i][0] = 1
			self.cell[x + n - 1][y + i][0] = 1
			self.cell[x + n - 1][y + 2*n - 2 + i][0] = 1
			self.cell[x + 2*n - 2][y + i][0] = 1
			self.cell[x + 2*n - 2][y + 2*n - 2 + i][0] = 1
			self.cell[x + 3*n - 3][y + n - 1 + i][0] = 1
		for i in range(1, n-1):
			self.cell[x + n - 1 + i][y][0] = 1
			self.cell[x + i][y + n - 1][0] = 1
			self.cell[x + 2*n - 2 + i][y + n - 1][0] = 1
			self.cell[x + i][y + 2*n - 2][0] = 1
			self.cell[x + 2*n - 2 + i][y + 2*n - 2][0] = 1
			self.cell[x + n - 1 + i][y + 3*n - 3][0] = 1

	def drawpipe(self, origin):
		x, y = origin
		n = self.rules.segmentlength
		# check that the pipe fits in the graph:
		if x < 0 or y < 0 or x + 3*n - 3 >= self.rules.dim or y + 3*n - 3 >= self.rules.dim:
			print('drawcross: The cross does not fit in the graph, no cross drawn')
		for i in range(n):
			self.cell[x][y + n - 1 + i][0] = 1
			self.cell[x + 3*n - 3][y + n - 1 + i][0] = 1
			self.cell[x + n - 1 + i][y][0] = 1
			self.cell[x + n - 1 + i][y + 3*n - 3][0] = 1
			self.cell[x + n - 1][y + n - 1 + i][0] = 1
			self.cell[x + 2*n - 2][y + n - 1 + i][0] = 1
		for i in range(1, n-1):
			self.cell[x + i][y + n - 1 - i][0] = 1
			self.cell[x + i][y + 2*n - 2 + i][0] = 1
			self.cell[x + 3*n - 3 - i][y + n - 1 - i][0] = 1
			self.cell[x + 3*n - 3 - i][y + 2*n - 2 + i][0] = 1
			self.cell[x + n - 1 + i][y + n - 1][0] = 1
			self.cell[x + n - 1 + i][y + 2*n - 2][0] = 1
		
	def drawongrid(self, grid):
		for x in range(self.rules.dim):
			for y in range(self.rules.dim):
				if self.cell[x][y][0]:
					grid.addpoint(x,y)
				if self.cell[x][y][1]:
					grid.addline(x,y,x+1,y)
				if self.cell[x][y][2]:
					grid.addline(x,y,x,y+1)
				if self.cell[x][y][3]:
					grid.addline(x,y,x+1,y+1)
				if self.cell[x][y][4]:
					grid.addline(x,y,x+1,y-1)

	def draw(self):
		grid = Grid(self.rules)
		self.drawongrid(grid)
		grid.draw()

	def print(self):
		# warning: this does not work properly
		print('X', end='')
		for site in self.cell[0]:
			print('ZZ', end='')
		print('X')
		for line in self.cell:
			print('N', end='')
			for site in line:
				if site[0]:
					print('O', end='')
				else:
					print(' ', end='')
				if site[2]:
					print('-', end='')
				else:
					print(' ', end='')
			print('N')
			print('N', end='')
			for site in line:
				if site[1]:
					print('|', end='')
				else:
					print(' ', end='')
				if site[3]:
					if site[4]:
						print('X', end='')
					else:
						print('\\', end='')
				else:
					if site[4]:
						print('/', end='')
					else:
						print(' ', end='')
			print('N')
		print('X', end='')
		for site in self.cell[0]:
			print('ZZ', end='')
		print('X')

#####################################################

# the class branch only knowes about the history of the construction; it might or might not have a graph attached

class Branch:

	def __init__(self, rules, previous=None, segment=None):
		self.rules = rules
		self.previous = previous
		self.next = []
		self.segment = segment
		if previous is None or segment is None:
			self.level = 0
			self.graph = Graph(rules)
			self.legalmoves = [] 
		else:
			self.level = previous.level + 1
			self.graph = previous.graph.copy()
			self.graph.addsegment(segment)
			self.legalmoves = self.rules.findlegalmoves(self.graph,
				self.previous.legalmoves, self.segment)
	
	def __del__(self):
		for branch in self.next:
			del branch
		del self.next
		del self.graph
		del self.legalmoves
	
	def newdescendant(self, segment):
		self.next.append(Branch(self.rules, self, segment))
	
	def chain(self):
		if self.previous is None:
			return []
		previouschain =self.previous.chain()
		previouschain.append(self)
		return previouschain

	def drawongrid(self,grid):
#		if type(self.previous) is type(self):
		if self.previous is not None:
			self.previous.drawongrid(grid)
			grid.addsegment(self.segment, self.level)
		else:
			self.graph.drawongrid(grid)

	def draw(self):
		grid = Grid(self.rules)
		self.drawongrid(grid)
		for move in self.legalmoves:
			grid.addlegalmove(move[0], move[1])
		grid.draw()
		
	def buildnextlevel(self):
		if len(self.next) > 0:
			print('buildnextlevel: branch exists already, no new branch created')
		else:
			for move in self.legalmoves:
				self.newdescendant(move)
		# to be implemented: delete branches that are equivalent under symmetry
	
	def randomexploration(self):
		if len(self.legalmoves) == 0:
			return self
		if len(self.next) > 0:
			print('randomexploration: branch exists already, no new branch created')
			return self.next[0]
		self.newdescendant(choice(self.legalmoves))
		return self.next[0].randomexploration()
	
	def cleardescendants(self):
		# delete all descendants, but not the branch itself
		for branch in self.next:
			del branch
		del self.next
		self.next = []
		
	def printbranchinfo(self):
		branchchain = self.chain()
		print('Number of legal moves per level:')
		print([len(branch.legalmoves) for branch in branchchain])
		print()

#####################################################

# a special type of branch that supports weighting of graphs for Monte Carlo exploration purposes

class WeightedBranch(Branch):

	def __init__(self, rules, previous=None, segment=None):
		Branch.__init__(self, rules, previous, segment)
		if previous is None or segment is None:
			self.unusedpoints = []	# list of points to which no new line has ever been attached
		else:
			segmentpoints = rules.segmentpoints(segment)
			self.unusedpoints = []			
			for point in previous.unusedpoints:
				if point not in segmentpoints:
					self.unusedpoints.append(point)
			self.unusedpoints.append(tuple(segment[0:2]))
	
	def __del__(self):
		Branch.__del__(self)
		del self.unusedpoints

	def newdescendant(self, segment):
		self.next.append(WeightedBranch(self.rules, self, segment))
	
	def randomexploration(self):
		if len(self.legalmoves) == 0:
			return self
		if len(self.next) > 0:
			print('randomexploration: branch exists already, no new branch created')
			return self.next[0]
		weightedlegalmoves = []
		for move in self.legalmoves:
			weight = 1
			for point in self.rules.segmentpoints(move):
				if point in self.unusedpoints:
					weight = weight * 2
			weightedlegalmoves = weightedlegalmoves + ([move] * weight)
		self.newdescendant(choice(weightedlegalmoves))
		return self.next[0].randomexploration()
		
	def printbranchinfo(self):
		Branch.printbranchinfo(self)
		branchchain = self.chain()
		print('Number of unused points per level:')
		print([len(branch.unusedpoints) for branch in branchchain])
		print()

#####################################################

class MorpionSolitaire:

	vector = [ (1,0), (0,1), (1,1), (1,-1) ]
	
	def __init__(self, dim, segmentlength, weighted = 0):
		self.ruleset = 'Rules: {} {}'
		self.startinggraph = 'Graph: {} Dimensions: {} Origin: {} {}'
		self.dim = dim
		self.segmentlength = segmentlength
		if weighted:
			self.seed = WeightedBranch(self)
		else:
			self.seed = Branch(self)
	
	def printinfo(self):
		print('-----------------')
		print('Morpion solitaire')
		print('-----------------')
		print(self.ruleset)
		print(self.startinggraph)
		print()
	
	def islegalmove(self, graph, segment):
		x, y, d, n = segment
		dx, dy = self.vector[d]
		x1 = x - dx * n
		y1 = y - dy * n
		x2 = (x + dx * (self.segmentlength - n))
		y2 = (y + dy * (self.segmentlength - n))
		if x1 < 0 or y1 < 0 or y2 <0 or x2 >= self.dim or y1 >= self.dim or y2 >= self.dim:
			return 0
		if graph.cell[x][y][0]:
			return 0
		for i in range(n):
			if not graph.cell[x + (i-n)*dx][y + (i-n)*dy][0]:
				return 0
		for i in range(self.segmentlength - n):
			if not graph.cell[x + (i + 1)*dx][y + (i + 1)*dy][0]:
				return 0
		for i in range(self.segmentlength):
			if graph.cell[x + (i-n)*dx][y + (i-n)*dy][d + 1]:
				return 0
		return 1
	
	def findlegalmoves(self, graph, previousmoves = None, previoussegment = None):
		list = []
		if previoussegment is None: # brute force search
			for x in range(self.dim):
				for y in range(self.dim):
					for d in range(4):
						for n in range(self.segmentlength + 1):
							move = (x, y, d, n) 
							if self.islegalmove(graph, move):
								list.append(move)
		else:
			x, y, previousd, previousn = previoussegment
			for d in range(4):
				if d != previousd:
					dx,dy = self.vector[d]
					for i in range(self.segmentlength + 1):
						for j in range(self.segmentlength + 1):
							if j != i:
								move = (	x - (i - j)*dx, y - (i - j)*dy, d, j)
								if self.islegalmove(graph, move):
									list.append(move)
			for move in previousmoves:
				if self.islegalmove(graph, move):
					list.append(move)
		return list
	
	def segmentpoints(self, segment):
		x, y, d, n = segment
		dx, dy = self.vector[d]
		irange = [i for i in range(n)] + [i for i in range(n+1, self.segmentlength + 1)]
		return [(x - (n-i)*dx, y - (n-i)*dy) for i in irange]
	
	def getfirstbranch(self):
		firstbranch = self.seed
		while len(firstbranch.next) > 0:
			firstbranch = firstbranch.next[0]
		return firstbranch
	
	def write(self, branch, filename):
		print('Writing graph to file:',filename)
		with open(filename,'w') as file:
			file.write('MorpionSolitaire\n')
			file.write(self.ruleset + '\n')
			file.write(self.startinggraph + '\n')
			for x in branch.chain():
				file.write(str(x.level))
				for m in x.segment:
					file.write(' ' + str(m))
				file.write('\n')
		file.close()
	
	def readformatinopenedfile(openedfile, weighted = 0):
		title = openedfile.readline().split()
		ruleset = openedfile.readline().split()
		startinggraph = openedfile.readline().split()
		if title[0] != 'MorpionSolitaire':
			print('MorpionSolitaire.read(): Error in file header')
			return
		if ruleset[0] != 'Rules:':
			print('MorpionSolitaire.read(): Error in rule set')
			return
		if (startinggraph[0] != 'Graph:'
			or startinggraph[2] != 'Dimensions:'
			or startinggraph[4] != 'Origin:'):
			print('MorpionSolitaire.read(): Error in definition of starting graph')
			return
		dim = int(startinggraph[3])
		origin = (int(startinggraph[5]), int(startinggraph[6]))
		if ruleset[1:3] == ['5', 'T']:
			if startinggraph[1] == 'cross':
				game = MorpionSolitaireCross5T(dim, origin, weighted)
			elif startinggraph[1] == 'pipe':
				game = MorpionSolitairePipe5T(dim, origin, weighted)
			else:
				print('MorpionSolitaire.read(): Unknown game type')
				return
		elif ruleset[1:3] == ['5', 'D']:
			if startinggraph[1] == 'cross':
				game = MorpionSolitaireCross5D(dim, origin, weighted)
			elif startinggraph[1] == 'pipe':
				game = MorpionSolitairePipe5D(dim, origin, weighted)
			else:
				print('MorpionSolitaire.read(): Unknown game type')
				return
		elif ruleset[1:3] == ['4', 'T']:
			if startinggraph[1] == 'cross':
				game = MorpionSolitaireCross4T(dim, origin, weighted)
			else:
				print('MorpionSolitaire.read(): Unknown game type')
				return
		elif ruleset[1:3] == ['4', 'D']:
			if startinggraph[1] == 'cross':
				game = MorpionSolitaireCross4D(dim, origin, weighted)
			else:
				print('MorpionSolitaire.read(): Unknown game type')
				return
		else:
			print('MorpionSolitaire.read(): Unknown game rules')
			return
		return game
		
	def readformat(filename, weighted = 0):
		with open(filename,'r') as file:
			game = MorpionSolitaire.readformatinopenedfile(file, weighted)
		file.close()
		return game
	
	def read(filename, weighted = 0):
		with open(filename,'r') as file:
			game = MorpionSolitaire.readformatinopenedfile(file, weighted)
			data = file.readlines()
			segments = [[int(x) for x in line.split()] for line in data]
			branch = game.seed
			for segment in segments:
				branch.newdescendant(segment[1:5])
				branch = branch.next[0]
		file.close()
		return game
	
	def readcount(filename):
		# open a file and counts the level of the graph
		with open(filename,'r') as file:
			data = file.readlines()
			return len(data) - 3
		file.close()
	

#####################################################


class MorpionSolitaire5T(MorpionSolitaire):
	
	def __init__(self, dim, weighted = 0):
		MorpionSolitaire.__init__(self, dim, 4, weighted)
		self.ruleset = self.ruleset.format('5','T')

class MorpionSolitaire5D(MorpionSolitaire):
	
	def __init__(self, dim, weighted = 0):
		MorpionSolitaire.__init__(self, dim, 4, weighted)	
		self.ruleset = self.ruleset.format('5','D')
	
	def islegalmove(self, graph, segment):
		# overloading of the method islegalmove() to forbid segments with touching ends
		if not MorpionSolitaire.islegalmove(self, graph, segment):
			return 0
		x, y, d, n = segment
		dx, dy = self.vector[d]
		x1 = x - (n+1)*dx
		y1 = y - (n+1)*dy
		if x1 > 0 and y1 > 0 and y1 <= self.dim and graph.cell[x1][y1][d+1]:
			return 0
		x2 = x + (self.segmentlength-n+1)*dx
		y2 = y + (self.segmentlength-n+1)*dy
		if x2 <= self.dim and y2 > 0 and y2 <= self.dim and graph.cell[x2][y2][d+1]:
			return 0
		return 1



class MorpionSolitaire4T(MorpionSolitaire):
	
	def __init__(self, dim, weighted = 0):
		MorpionSolitaire.__init__(self, dim, 3, weighted)
		self.ruleset = self.ruleset.format('4','T')

class MorpionSolitaire4D(MorpionSolitaire):
	
	def __init__(self, dim, weighted = 0):
		MorpionSolitaire.__init__(self, dim, 3, weighted)	
		self.ruleset = self.ruleset.format('4','D')
	
	def islegalmove(self, graph, segment):
		# overloading of the method islegalmove() to forbid segments with touching ends
		if not MorpionSolitaire.islegalmove(self, graph, segment):
			return 0
		x, y, d, n = segment
		dx, dy = self.vector[d]
		x1 = x - (n+1)*dx
		y1 = y - (n+1)*dy
		if x1 > 0 and y1 > 0 and y1 <= self.dim and graph.cell[x1][y1][d+1]:
			return 0
		x2 = x + (self.segmentlength-n+1)*dx
		y2 = y + (self.segmentlength-n+1)*dy
		if x2 <= self.dim and y2 > 0 and y2 <= self.dim and graph.cell[x2][y2][d+1]:
			return 0
		return 1


#####################################################

class MorpionSolitaireCross5D(MorpionSolitaire5D):
	
	def __init__(self, dim = 32, origin = (11, 11), weighted = 0):
		MorpionSolitaire5D.__init__(self, dim, weighted)
		self.startinggraph = self.startinggraph.format('cross', 
			str(dim), str(origin[0]), str(origin[1]))
		self.seed.graph.drawcross(origin)
		self.seed.legalmoves = self.findlegalmoves(self.seed.graph)

class MorpionSolitaireCross5T(MorpionSolitaire5T):
	
	def __init__(self, dim = 32, origin = (11, 11), weighted = 0):
		MorpionSolitaire5T.__init__(self, dim, weighted)
		self.startinggraph = self.startinggraph.format('cross',
			str(dim), str(origin[0]), str(origin[1]))
		self.seed.graph.drawcross(origin)
		self.seed.legalmoves = self.findlegalmoves(self.seed.graph)

class MorpionSolitairePipe5D(MorpionSolitaire5D):
	
	def __init__(self, dim = 32, origin = (11, 11), weighted = 0):
		MorpionSolitaire5D.__init__(self, dim, weighted)
		self.startinggraph = self.startinggraph.format('pipe',
			str(dim), str(origin[0]), str(origin[1]))
		self.seed.graph.drawpipe(origin)
		self.seed.legalmoves = self.findlegalmoves(self.seed.graph)

class MorpionSolitairePipe5T(MorpionSolitaire5T):
	
	def __init__(self, dim = 32, origin = (11, 11), weighted = 0):
		MorpionSolitaire5T.__init__(self, dim, weighted)
		self.startinggraph = self.startinggraph.format('pipe',
			str(dim), str(origin[0]), str(origin[1]))
		self.seed.graph.drawpipe(origin)
		self.seed.legalmoves = self.findlegalmoves(self.seed.graph)


class MorpionSolitaireCross4D(MorpionSolitaire4D):
	
	def __init__(self, dim = 32, origin = (11, 11), weighted = 0):
		MorpionSolitaire4D.__init__(self, dim, weighted)
		self.startinggraph = self.startinggraph.format('cross', 
			str(dim), str(origin[0]), str(origin[1]))
		self.seed.graph.drawcross(origin)
		self.seed.legalmoves = self.findlegalmoves(self.seed.graph)

class MorpionSolitaireCross4T(MorpionSolitaire5T):
	
	def __init__(self, dim = 32, origin = (11, 11), weighted = 0):
		MorpionSolitaire4T.__init__(self, dim, weighted)
		self.startinggraph = self.startinggraph.format('cross',
			str(dim), str(origin[0]), str(origin[1]))
		self.seed.graph.drawcross(origin)
		self.seed.legalmoves = self.findlegalmoves(self.seed.graph)





