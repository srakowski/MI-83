namespace MI83.Core.Programs
{
	using IronPython.Hosting;
	using Microsoft.Scripting.Hosting;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	class PythonPrgm : Program
	{
		private Computer _computer;
		private ScriptEngine _engine;
		private string _code;

		public PythonPrgm(Computer computer, string code) : base(computer)
		{
			_computer = computer;
			_engine = Python.CreateEngine();
			_code = code;
		}

		protected override object Main()
		{
			try
			{
				var scope = _engine.CreateScope();

				// System
				scope.SetVariable(nameof(RunPrgm), new Action<string>(RunPrgm));
				scope.SetVariable(nameof(GetKey), new Func<int>(GetKey));
				scope.SetVariable(nameof(Pause), new Action(Pause));
				scope.SetVariable(nameof(GetSuppDispRes), new Func<string[]>(GetSuppDispRes));
				scope.SetVariable(nameof(GetDispRes), new Func<int>(GetDispRes));
				scope.SetVariable(nameof(SetDispRes), new Action<int>(SetDispRes));
				scope.SetVariable(nameof(GetFG), new Func<int>(GetFG));
				scope.SetVariable(nameof(SetFG), new Action<int>(SetFG));
				scope.SetVariable(nameof(GetBG), new Func<int>(GetBG));
				scope.SetVariable(nameof(SetBG), new Action<int>(SetBG));
				scope.SetVariable(nameof(DispHome), new Action(DispHome));
				scope.SetVariable(nameof(DispGraph), new Action(DispGraph));

				// Home Screen
				scope.SetVariable(nameof(ClrHome), new Action(ClrHome));
				scope.SetVariable(nameof(Output), new Action<int, int, string>(Output));
				scope.SetVariable(nameof(Disp), new Action<object>(Disp));
				scope.SetVariable(nameof(Input), new Func<string, string>(Input));
				scope.SetVariable(nameof(Menu), new Func<IEnumerable<object>, (int, int)>(Menu));

				// Graphics Screen
				scope.SetVariable(nameof(ClrDraw), new Action(ClrDraw));
				scope.SetVariable(nameof(Pixel), new Action<int, int>(Pixel));
				scope.SetVariable(nameof(Line), new Action<int, int, int, int>(Line));
				scope.SetVariable(nameof(Horizontal), new Action<int>(Horizontal));
				scope.SetVariable(nameof(Vertical), new Action<int>(Vertical));

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
				Menu(menu.Cast<object>().AsEnumerable());
			}
			return null;
		}
	}
}
