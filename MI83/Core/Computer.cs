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
			GraphicsScreen = new GraphicsScreen(this);
			ActiveDisplayMode = HomeScreen;
		}

		public bool Shutdown { get; private set; }

		public Display Display { get; }

		public ProgramRegistry ProgramRegistry { get; }

		public HomeScreen HomeScreen { get; }

		public GraphicsScreen GraphicsScreen { get; }

		public IDisplayMode ActiveDisplayMode { get; private set; }

		public int FG { get; private set; } = 5;

		public int BG { get; private set; } = 0;

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

		public void DispHome()
		{
			ActiveDisplayMode = HomeScreen;
		}

		public void DispGrap()
		{
			ActiveDisplayMode = GraphicsScreen;
		}

		public void RenderActiveDisplayMode()
		{
			ActiveDisplayMode.Render(Display);
		}

		public void SetFG(int paletteIdx)
		{
			FG = paletteIdx % Display.ColorPalette.Length;
		}

		public void SetBG(int paletteIdx)
		{
			BG = paletteIdx % Display.ColorPalette.Length;
		}
	}
}
