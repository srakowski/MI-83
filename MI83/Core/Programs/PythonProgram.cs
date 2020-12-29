namespace MI83.Core.Programs
{
	using IronPython.Hosting;
	using Microsoft.Scripting.Hosting;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	class PythonProgram : IProgram
	{
		private Computer _computer;
		private ScriptEngine _engine;
		private string _code;

		public PythonProgram(Computer computer, string code)
		{
			_computer = computer;
			_engine = Python.CreateEngine();
			_code = code;
		}

		public Task Execute()
		{
			return Task.Factory.StartNew(ExecuteInThread);
		}

		private void ExecuteInThread()
		{
			try
			{
				var scope = _engine.CreateScope();

				// System
				scope.SetVariable(nameof(Sys.RunPrgm), new Action<string>(_computer.System.RunPrgm));
				scope.SetVariable(nameof(HomeScreen.GetKey), new Func<int>(_computer.Home.GetKey));
				scope.SetVariable(nameof(Display.GetSuppDispRes), new Func<string[]>(_computer.Display.GetSuppDispRes));
				scope.SetVariable(nameof(Display.GetDispRes), new Func<int>(_computer.Display.GetDispRes));
				scope.SetVariable(nameof(Display.SetDispRes), new Action<int>(_computer.Display.SetDispRes));
				scope.SetVariable(nameof(Computer.SetFG), new Action<int>(_computer.SetFG));
				scope.SetVariable(nameof(Computer.SetBG), new Action<int>(_computer.SetBG));
				scope.SetVariable(nameof(Computer.DispHome), new Action(_computer.DispHome));
				scope.SetVariable(nameof(Computer.DispGrap), new Action(_computer.DispGrap));

				// Home Screen
				scope.SetVariable(nameof(HomeScreen.ClrHome), new Action(_computer.Home.ClrHome));
				scope.SetVariable(nameof(HomeScreen.Output), new Action<int, int, string>(_computer.Home.Output));
				scope.SetVariable(nameof(HomeScreen.Disp), new Action<string>(_computer.Home.Disp));
				scope.SetVariable(nameof(HomeScreen.Input), new Func<string, string>(_computer.Home.Input));
				scope.SetVariable(nameof(HomeScreen.Pause), new Action(_computer.Home.Pause));
				scope.SetVariable(nameof(HomeScreen.Menu), new Func<IEnumerable<object>, (int, int)>(_computer.Home.Menu));

				// Graphics Screen
				scope.SetVariable(nameof(GraphicsScreen.ClrDraw), new Action(_computer.Graphics.ClrDraw));
				scope.SetVariable(nameof(GraphicsScreen.Pixel), new Action<int, int>(_computer.Graphics.Pixel));
				scope.SetVariable(nameof(GraphicsScreen.Line), new Action<int, int, int, int>(_computer.Graphics.Line));
				scope.SetVariable(nameof(GraphicsScreen.Horizontal), new Action<int>(_computer.Graphics.Horizontal));
				scope.SetVariable(nameof(GraphicsScreen.Vertical), new Action<int>(_computer.Graphics.Vertical));

				_engine.CreateScriptSourceFromString(_code)
					.Execute(scope);
			}
			catch (Exception ex)
			{
				var menu = new object[]
				{
					new object[] {
						$"ERR:{ex.GetType().Name.ToUpper()}",
						"Quit"
					}
				};
				_computer.Home.Menu(menu.Cast<object>().AsEnumerable());
			}
		}
	}
}
