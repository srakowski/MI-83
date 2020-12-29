namespace MI83.Core
{
	using System;
	using System.Collections;
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

		public Task Execute() => Task.Factory.StartNew(() =>
		{
			var returnValue = Main();
			Publish(new Instruction.RetPrgm(returnValue));
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

		protected void ClrHome() => Publish(new ClrHome());
	}
}


//| Command | Description |
//| -------| -----------|
//|DispHome()| Switches to the Home Screen.|
//|DispGraph()| Switches to Graphics Screen.|
//|RunPrgm(prgmName) | Runs the specified program.|
//|GetKey(): keycode | Returns the code of the last key pressed.|
//|Pause()| Waits for the user to press the[Enter] key.|
//|GetSuppDispRes(): [dispRes]| Return a list of supported  resolutions.|
//|GetDispRes(dispResIdx)| Gets the index of the current resolution of the display.|
//|SetDispRes(dispResIdx)| Sets the resolution of the display.|
//|GetFG(): palette_idx | Gets the foreground color.|
//|SetFG(palette_idx)| Sets foregroud color from palette. Takes effect on future commands.|
//|GetBG(): palette_idx | Gets the background color.|
//|SetBG(palette_idx)| Sets foregroud color from palette. Takes effect on future commands.|

//### Home Screen Commands 

//|Command|Description|
//|-------|-----------|
//|ClrHome()| Clears the home screen.|
//|Output(row, col, text)| Outputs text value at x/y coordinates.|
//|Disp(text)| Displays a line of text.|
//|Input(prompt): value | Reads a line of text.|
//|Menu([(tab_name, [menu_opt_1, ...]), ...]): (tab_idx, opt_idx) | Displays a menu.|