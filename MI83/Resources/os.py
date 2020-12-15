ClrHome()
selection = Menu([\
	('EXEC', ['None', 'Noop']),\
	('EDIT', ['None', 'Noop']),\
	('NEW', ['CREATE PROGRAM']),\
	('SYS', ['SHUTDOWN'])\
])
ClrHome()
Disp('You selected ' + str(selection))