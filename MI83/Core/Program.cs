namespace MI83.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using static Instruction;

	abstract class Program
	{
		private readonly Computer _computer;
		private object _response;
		private bool _isWaiting;

		public Program(Computer computer)
		{
			_computer = computer;
			_response = null;
			_isWaiting = false;
		}

		public int FG { get; set; } = 5;

		public int BG { get; set; } = 0;

		public Task Execute() => Task.Factory.StartNew(() =>
		{
			var returnValue = Main();
			_ExitPrgm(returnValue);
		});

		protected abstract object Main();

		private void Publish(Instruction instruction)
		{
			_computer.Instructions.Enqueue(instruction);
		}

		private object BeginRequest(Instruction instruction)
		{
			_isWaiting = true;
			_computer.Instructions.Enqueue(instruction);
			while (_isWaiting)
			{
				// 1000ms / 60fps ~16.6 round up to 17, i.e. wait until approx next frame.
				Thread.Sleep(17);
			}
			return _response;
		}

		public void EndRequest(object value)
		{
			_response = value;
			_isWaiting = false;
		}

		protected void DispHome() => Publish(new DispHome());
		protected void DispGraph() => Publish(new DispGraph());
		protected void RunPrgm(string prgmName) => BeginRequest(new RunPrgm(prgmName));
		protected int GetKey() => (int)BeginRequest(new GetKey());
		protected void Pause() => BeginRequest(new Pause());
		protected string[] GetSuppDispRes() => (string[])BeginRequest(new GetSuppDispRes());
		protected int GetDispRes() => (int)BeginRequest(new GetDispRes());
		protected void SetDispRes(int dispResIdx) => Publish(new SetDispRes(dispResIdx));
		protected int GetFG() => (int)BeginRequest(new GetFG());
		protected void SetFG(int paletteIdx) => Publish(new SetFG(paletteIdx));
		protected int GetBG() => (int)BeginRequest(new GetBG());
		protected void SetBG(int paletteIdx) => Publish(new SetBG(paletteIdx));

		protected void ClrHome() => Publish(new ClrHome());
		protected void Output(int row, int col, string text) => Publish(new Output(row, col, text));
		protected void Disp(object text) => Publish(new Disp(text));
		protected string Input(string prompt) => (string)BeginRequest(new Input(prompt));
		protected (int, int) Menu(IEnumerable<object> tabs) => ((int, int))BeginRequest(new Menu(tabs));

		protected void ClrDraw() => Publish(new ClrDraw());
		protected void Pixel(int x, int y) => Publish(new Pixel(x, y));
		protected void Line(int x1, int y1, int x2, int y2) => Publish(new Line(x1, y1, x2, y2));
		protected void Horizontal(int y) => Publish(new Horizontal(y));
		protected void Vertical(int x) => Publish(new Vertical(x));

		protected void _CreatePrgm(string prgmName) => Publish(new _CreatePrgm(prgmName));
		protected void _EditPrgm(string prgmName) => BeginRequest(new _EditPrgm(prgmName));
		protected void _ExitPrgm(object value) => Publish(new _ExitPrgm(value));
	}
}
