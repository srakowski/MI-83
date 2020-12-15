namespace MI83.Core
{
	using IronPython.Hosting;
	using Microsoft.Scripting.Hosting;
	using System;
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

		public void Execute()
		{
			Task.Factory.StartNew(ExecuteInThread);
		}

		private void ExecuteInThread()
		{
			var scope = _engine.CreateScope();
			scope.SetVariable(nameof(HomeScreen.ClrHome), new Action(_computer.HomeScreen.ClrHome));
			scope.SetVariable(nameof(HomeScreen.Disp), new Action<string>(_computer.HomeScreen.Disp));
			scope.SetVariable(nameof(HomeScreen.Input), new Func<string, string>(_computer.HomeScreen.Input));
			_engine.CreateScriptSourceFromString(_code)
				.Execute(scope);
		}
	}
}
