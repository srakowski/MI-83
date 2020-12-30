namespace MI83.Core
{
	using System;
	using System.Collections;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;

	class Computer
	{
		private const int MaxInstructionsPerFrame = 20;
		private Stack<Program> _programStack;

		public Computer()
		{
			_programStack = new Stack<Program>();
			Shutdown = false;
			Instructions = new ConcurrentQueue<Instruction>();
			Display = new Buffers.Display();
			Home = new Buffers.Home(Display.Resolution.Height / SysFont.CharHeight, Display.Resolution.Width / SysFont.CharWidth);
			Graphics = new Buffers.Graphics(Buffers.Display.MaxResolution.Width, Buffers.Display.MaxResolution.Height);
			InputBuffer = new Buffers.Input();
			DisplayMode = DisplayMode.Home;
		}

		public bool Shutdown { get; private set; }

		public ConcurrentQueue<Instruction> Instructions { get; }

		public Buffers.Display Display { get; }

		public DisplayMode DisplayMode { get; set; }

		public Buffers.Home Home { get; }

		public Buffers.Graphics Graphics { get; }

		public Buffers.Input InputBuffer { get; }

		public Program ActiveProgram => _programStack.Any() ? _programStack.Peek() : null;

		public void Boot()
		{
			ExecuteProgram(new Programs.RootPrgm(this));
		}

		public void Tick()
		{
			var i = 0;
			while (Instructions.TryDequeue(out var instruction))
			{
				Interpret(instruction);
				i++;
				if (i >= MaxInstructionsPerFrame)
					break;
			}
		}

		public void RenderActiveDisplayMode()
		{
			if (DisplayMode == DisplayMode.Home)
			{
				Home.Render(Display);
			}
			else if (DisplayMode == DisplayMode.Graphics)
			{
				Graphics.Render(Display);
			}
		}

		private void Interpret(Instruction instruction)
		{
			switch (instruction)
			{
				case Instruction.DispHome:
					DisplayMode = DisplayMode.Home;
					break;

				case Instruction.DispGraph:
					DisplayMode = DisplayMode.Graphics;
					break;

				case Instruction.RunPrgm runPrgm:
					ExecuteProgram(LoadPythonProgram(runPrgm));
					break;

				case Instruction.GetKey getKey:
					ReturnImmediate(InputBuffer.GetLastKeyUp());
					break;

				case Instruction.Pause pause:
					ExecuteProgram(new Programs.PausePrgm(this));
					break;

				case Instruction.GetSuppDispRes:
					ReturnImmediate(Display.GetSuppDispRes());
					break;

				case Instruction.GetDispRes:
					ReturnImmediate(Display.GetDispRes());
					break;

				case Instruction.SetDispRes setDispRes:
					Display.SetDispRes(setDispRes.DispResIdx);
					Home.Resize(Display.Resolution.Height / SysFont.CharHeight, Display.Resolution.Width / SysFont.CharWidth);
					break;

				case Instruction.GetFG:
					ReturnImmediate(ActiveProgram.FG);
					break;

				case Instruction.SetFG setFG:
					ActiveProgram.FG = setFG.PaletteIdx;
					break;

				case Instruction.GetBG:
					ReturnImmediate(ActiveProgram.BG);
					break;

				case Instruction.SetBG setBG:
					ActiveProgram.BG = setBG.PaletteIdx;
					break;

				case Instruction._CreatePrgm createPrgm:
					Disk.WritePrgm(createPrgm.PrgmName, null);
					break;

				case Instruction._EditPrgm editPrgm:
					ExecuteProgram(new Programs.EditorPrgm(this, editPrgm.PrgmName));
					break;

				case Instruction._ExitPrgm exitPrgm:
					ExitProgram(exitPrgm.Value);
					break;

				case Instruction._BeginText:
					InputBuffer.BeginTextInput();
					break;

				case Instruction._GetText:
					ReturnImmediate(InputBuffer.GetTextInput());
					break;

				case Instruction._EndText:
					InputBuffer.EndTextInput();
					break;

				case Instruction.ClrHome:
					DisplayMode = DisplayMode.Home;
					Home.Clear((byte)ActiveProgram.FG, (byte)ActiveProgram.BG);
					break;

				case Instruction.Output output:
					DisplayMode = DisplayMode.Home;
					Home.Output(output.Row, output.Col, output.Text, (byte)ActiveProgram.FG, (byte)ActiveProgram.BG);
					break;

				case Instruction.Disp disp:
					DisplayMode = DisplayMode.Home;
					Home.Disp(disp.Value, (byte)ActiveProgram.FG, (byte)ActiveProgram.BG);
					break;

				case Instruction.Input input:
					ExecuteProgram(new Programs.InputPrgm(this, input.Prompt));
					break;

				case Instruction.Menu menu:
					ExecuteProgram(new Programs.MenuPrgm(this, menu.Tabs));
					break;

				case Instruction._Prompt prompt:
					ReturnImmediate(Home.Prompt(prompt.Text, (byte)ActiveProgram.FG, (byte)ActiveProgram.BG));
					break;

				case Instruction._Scroll:
					Home.Scroll((byte)ActiveProgram.FG, (byte)ActiveProgram.BG);
					break;

				case Instruction._GetHomeDim:
					ReturnImmediate((Home.Rows, Home.Cols));
					break;

				case Instruction._Cursor cursor:
					Home.RenderCursor(cursor.Row, cursor.Col, cursor.On);
					break;

				case Instruction.ClrDraw:
					DisplayMode = DisplayMode.Graphics;
					Graphics.ClrDraw((byte)ActiveProgram.BG);
					break;

				case Instruction.Pixel pixel:
					DisplayMode = DisplayMode.Graphics;
					Graphics.Pixel(pixel.X, pixel.Y, (byte)ActiveProgram.FG);
					break;

				case Instruction.Line line:
					DisplayMode = DisplayMode.Graphics;
					Graphics.Line(line.X1, line.Y1, line.X2, line.Y2, (byte)ActiveProgram.FG);
					break;

				case Instruction.Horizontal horizontal:
					DisplayMode = DisplayMode.Graphics;
					Graphics.Horizontal(horizontal.Y, (byte)ActiveProgram.FG);
					break;

				case Instruction.Vertical vertical:
					DisplayMode = DisplayMode.Graphics;
					Graphics.Vertical(vertical.X, (byte)ActiveProgram.FG);
					break;
			}
		}

		private Program LoadPythonProgram(Instruction.RunPrgm runPrgm)
		{
			var code = Disk.ReadPrgm(runPrgm.PrgmName);
			return new Programs.PythonPrgm(this, code);
		}

		private void ExecuteProgram(Program prog)
		{
			_programStack.Push(prog);
			prog.Execute();
		}

		private void ExitProgram(object exitValue)
		{
			if (_programStack.Any())
			{
				_programStack.Pop();
			}

			if (!_programStack.Any())
			{
				Shutdown = true;
				return;
			}

			ActiveProgram?.EndRequest(exitValue);
		}

		private void ReturnImmediate(object value)
		{
			ActiveProgram?.EndRequest(value);
		}
	}
}
