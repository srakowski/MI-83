using System;

namespace MI83.Core
{
	class Computer
	{
		public Computer()
		{
			Display = new Display();
			HomeScreen = new HomeScreen(this);
		}

		public Display Display { get; }

		public HomeScreen HomeScreen { get; }

		public GraphicsScreen GraphicsScreen { get; }

		public IDisplayMode ActiveDisplayMode { get; private set; }

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
