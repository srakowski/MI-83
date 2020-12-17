# MI-83 "OS"

def run_create_program():
	ClrHome()
	Disp("PROGRAM\n")
	name = Input("Name=")
	CreatePrgm(name)

def run_edit_program(program_name):
	oldRes = GetDispRes()
	SetDispRes(2)
	ClrHome()
	prgm = ReadPrgm(program_name).split('\n')
	end = False
	while not end:
		Output(0, 0, "PROGRAM:" + program_name + "\n")
		lines = prgm[0:10]
		for line in lines:
			Disp(":" + line)
		Pause()
		end = True
	SetDispRes(oldRes)

def run_program(program_name):
	RunPrgm(program_name)
	pass

def run_display():
	ClrHome()
	resolutions = GetSuppDispRes()
	selection = Menu([("DISPLAY", resolutions)])
	SetDispRes(selection[1])

shutdown = False
while not shutdown:
	ClrHome()
	programs = GetPrgms()
	selection = Menu([\
		("EXEC", programs),\
		("EDIT", programs),\
		("NEW", ["Create New"]),\
		("SYS", ["Display", "Shutdown"])\
	])

	tab_idx = selection[0]
	option_idx = selection[1]

	if tab_idx == 0:
		run_program(programs[option_idx])

	elif tab_idx == 1:
		run_edit_program(programs[option_idx])

	elif tab_idx == 2:
		if option_idx == 0:
			run_create_program()

	elif tab_idx == 3:
		if option_idx == 0:
			run_display()
		elif option_idx == 1:
			shutdown = True