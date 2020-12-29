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
			System = new Sys(this);
			Home = new HomeScreen(this);
			Graphics = new GraphicsScreen(this);
			ActiveDisplayMode = Home;
		}

		public bool Shutdown { get; private set; }

		public Display Display { get; }

		public Sys System { get; }

		public HomeScreen Home { get; }

		public GraphicsScreen Graphics { get; }

		public IDisplayMode ActiveDisplayMode { get; private set; }

		public int FG { get; private set; } = 5;

		public int BG { get; private set; } = 0;

		public void Boot()
		{
			var prog = new Programs.PrgmMenu(this);
			prog.Execute()
				.ContinueWith(_ =>
				{
					Shutdown = true;
				});
		}

		public void DispHome()
		{
			ActiveDisplayMode = Home;
		}

		public void DispGrap()
		{
			ActiveDisplayMode = Graphics;
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
