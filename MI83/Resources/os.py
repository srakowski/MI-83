# MI-83 "OS"

def reset_colors():
	SetBG(0)
	SetFG(5)

def run_create_program():
	ClrHome()
	Disp("PROGRAM\n")
	name = Input("Name=")
	CreatePrgm(name)

def run_edit_program(program_name):
	EditPrgm(program_name)

def run_program(program_name):
	RunPrgm(program_name)
	reset_colors()

def run_display():
	ClrHome()
	resolutions = GetSuppDispRes()
	selection = Menu([("DISPLAY", resolutions)])
	SetDispRes(selection[1])

reset_colors()
shutdown = False
while not shutdown:
	ClrHome()
	programs = GetPrgms()
	selection = Menu([\
		["EXEC"] + programs,\
		["EDIT"] + programs,\
		["NEW", "Create New"],\
		["SYS", "Display", "Shutdown"]\
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