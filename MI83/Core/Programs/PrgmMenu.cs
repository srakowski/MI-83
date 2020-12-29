namespace MI83.Core.Programs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	class PrgmMenu : Program
	{
		private readonly Computer _computer;
		private readonly Sys _system;
		private readonly HomeScreen _home;

		public PrgmMenu(Computer computer) : base(computer)
		{
			_computer = computer;
			_system = _computer.System;
			_home = _computer.Home;
		}

		protected override object Main()
		{
			bool shutdown = false;
			while (!shutdown)
			{
				_home.ClrHome();
				_home.Disp("PROGRAM\n");
				var progs = _system.GetPrgms();
				var selections = _home.Menu(new[] {
					progs.Prepend("EXEC"),
					progs.Prepend("EDIT"),
					new [] { "NEW", "Create New" },
					new [] { "SYS", "Display", "Shutdown" }
				});

				var tabIdx = selections.Item1;
				var optionIdx = selections.Item2;

				switch (tabIdx)
				{
					case 0:
						RunPrgm(progs[optionIdx]);
						break;

					case 1:
						_system.EditPrgm(progs[optionIdx]);
						break;

					case 2 when optionIdx == 0:
						_home.ClrHome();
						_home.Disp("PROGRAM\n");
						var name = _home.Input("Name=");
						_system.CreatePrgm(name);
						_system.EditPrgm(name);
						break;

					case 3 when optionIdx == 0:
						//	ClrHome()
						//	resolutions = GetSuppDispRes()
						//	selection = Menu([("DISPLAY", resolutions)])
						//	SetDispRes(selection[1])
						break;

					case 3 when optionIdx == 1:
						shutdown = true;
						break;
				}
			}
			return null;
		}
	}
}
