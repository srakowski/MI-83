namespace MI83.Core.Programs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	class RootPrgm : Program
	{
		public RootPrgm(Computer computer) : base(computer) { }

		protected override object Main()
		{
			bool shutdown = false;
			while (!shutdown)
			{
				ClrHome();
				Disp("PROGRAM\n");
				var progs = Disk.GetPrgms();
				var selections = Menu(new[] {
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
						_EditPrgm(progs[optionIdx]);
						break;

					case 2 when optionIdx == 0:
						ClrHome();
						Disp("PROGRAM\n");
						var name = Input("Name=");
						_CreatePrgm(name);
						_EditPrgm(name);
						break;

					case 3 when optionIdx == 0:
						ClrHome();
						var resolutions = GetSuppDispRes();
						var selection = Menu(new[] { resolutions.Prepend("DISPLAY") });
						SetDispRes(selection.Item2);
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
