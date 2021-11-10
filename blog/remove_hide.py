import fileinput, re
flag = False
hide = False
code = False
for line in fileinput.input(inplace=True):
	if flag:
		if re.search('^#.?hide', line) is not None:
			hide = True
		elif re.search('^```', line) is not None:
			code = False
		else:
			print(temp, end='')
			print(line, end='')
		flag = False
	elif re.search('^```', line) is not None:
		code = not code
		if code:
			flag = True
			temp = line
		else:
			if hide:
				hide = False
			else:
				print(line, end='')
	else:
		if hide == False:
			print(line, end='')

