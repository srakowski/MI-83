namespace MI83.Core
{
	using System;
	using System.Collections.Generic;

	class Computer
	{
		public Computer()
		{
			Display = new Display();
			Programs = new Stack<Program>();
			HomeScreen = new HomeScreen(this);
			ActiveDisplayMode = HomeScreen;
		}

		public Display Display { get; }

		public Stack<Program> Programs { get; }

		public HomeScreen HomeScreen { get; }

		public GraphicsScreen GraphicsScreen { get; }

		public IDisplayMode ActiveDisplayMode { get; private set; }

		public void Boot()
		{
			var os = Rom.Get("os.py");
			var prog = new Program(this, os);
			prog.Execute();
			Programs.Push(prog);
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
