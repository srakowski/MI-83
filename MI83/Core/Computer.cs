namespace MI83.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	class Computer
	{
		public Computer()
		{
			Display = new Display();
			ProgramRegistry = new ProgramRegistry(this);
			HomeScreen = new HomeScreen(this);
			ActiveDisplayMode = HomeScreen;
		}

		public bool Shutdown { get; private set; }

		public Display Display { get; }

		public ProgramRegistry ProgramRegistry { get; }

		public HomeScreen HomeScreen { get; }

		public GraphicsScreen GraphicsScreen { get; }

		public IDisplayMode ActiveDisplayMode { get; private set; }

		public void Boot()
		{
			var os = Rom.Get("os.py");
			var prog = new Program(this, os);
			prog.Execute()
				.ContinueWith(_ =>
				{
					Shutdown = true;
				});
		}

		public void DisplayHomeScreen()
		{
			ActiveDisplayMode = HomeScreen;
		}

		public void DisplayGraphScreen()
		{
			ActiveDisplayMode = GraphicsScreen;
		}

		public void RenderActiveDisplayMode()
		{
			ActiveDisplayMode.Render(Display);
		}
	}
}
