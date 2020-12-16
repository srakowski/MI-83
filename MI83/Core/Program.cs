namespace MI83.Core
{
	using IronPython.Hosting;
	using Microsoft.Scripting.Hosting;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	class Program
	{
		private Computer _computer;
		private ScriptEngine _engine;
		private string _code;

		public Program(Computer computer, string code)
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

				// Programs
				scope.SetVariable(nameof(ProgramRegistry.GetPrgms), new Func<string[]>(_computer.ProgramRegistry.GetPrgms));
				scope.SetVariable(nameof(ProgramRegistry.CreatePrgm), new Action<string>(_computer.ProgramRegistry.CreatePrgm));
				scope.SetVariable(nameof(ProgramRegistry.ReadPrgm), new Func<string, string>(_computer.ProgramRegistry.ReadPrgm));
				scope.SetVariable(nameof(ProgramRegistry.RunPrgm), new Action<string>(_computer.ProgramRegistry.RunPrgm));

				// Home Screen
				scope.SetVariable(nameof(HomeScreen.ClrHome), new Action(_computer.HomeScreen.ClrHome));
				scope.SetVariable(nameof(HomeScreen.Output), new Action<int, int, string>(_computer.HomeScreen.Output));
				scope.SetVariable(nameof(HomeScreen.Disp), new Action<string>(_computer.HomeScreen.Disp));
				scope.SetVariable(nameof(HomeScreen.Input), new Func<string, string>(_computer.HomeScreen.Input));
				scope.SetVariable(nameof(HomeScreen.Pause), new Action(_computer.HomeScreen.Pause));
				scope.SetVariable(nameof(HomeScreen.Menu), new Func<IEnumerable<object>, (int, int)>(_computer.HomeScreen.Menu));
				scope.SetVariable(nameof(HomeScreen.SetFG), new Action<int>(_computer.HomeScreen.SetFG));
				scope.SetVariable(nameof(HomeScreen.SetBG), new Action<int>(_computer.HomeScreen.SetBG));

				_engine.CreateScriptSourceFromString(_code)
					.Execute(scope);
			}
			catch (Exception ex)
			{
				var menu = new object[]
				{
					new object[] {
						$"ERR:{ex.GetType().Name.ToUpper()}",
						new [] { "Quit" }
					}
				};
				_computer.HomeScreen.Menu(menu.Cast<object>().AsEnumerable());
			}
		}
	}
}
