namespace MI83.Core
{
	using System;
	using System.Collections;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	class Computer
	{
		private Stack<Program> _programStack;

		public Computer()
		{
			_programStack = new Stack<Program>();
			Shutdown = false;
			Instructions = new ConcurrentQueue<Instruction>();
			Display = new Buffers.Display();
			Home = new Buffers.Home();
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
			while (Instructions.TryDequeue(out var instruction))
			{
				Interpret(instruction);
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
					break;

				case Instruction.GetFG:
					ReturnImmediate(Display.FG);
					break;

				case Instruction.SetFG setFG:
					Display.SetFG(setFG.PaletteIdx);
					break;

				case Instruction.GetBG:
					ReturnImmediate(Display.BG);
					break;

				case Instruction.SetBG setBG:
					Display.SetFG(setBG.PaletteIdx);
					break;

				case Instruction.ClrHome:
					DisplayMode = DisplayMode.Home;
					Home.Clear();
					break;

				case Instruction.Output output:
					DisplayMode = DisplayMode.Home;
					Home.Output(output.Row, output.Col, output.Text);
					break;

				case Instruction.Disp disp:
					DisplayMode = DisplayMode.Home;
					Home.Disp(disp.Text);
					break;

				case Instruction.DispLine dispLine:
					DisplayMode = DisplayMode.Home;
					Home.DispLine(dispLine.Text);
					break;

				case Instruction.Input input:
					ExecuteProgram(new Programs.InputPrgm(this, input.Prompt));
					break;

				case Instruction.Menu menu:
					ExecuteProgram(new Programs.MenuPrgm(this, menu.Tabs));
					break;

				case Instruction.ClrDraw:
					DisplayMode = DisplayMode.Graphics;
					Graphics.ClrDraw((byte)Display.BG);
					break;

				case Instruction.Pixel pixel:
					DisplayMode = DisplayMode.Graphics;
					Graphics.Pixel(pixel.X, pixel.Y, (byte)Display.FG);
					break;

				case Instruction.Line line:
					DisplayMode = DisplayMode.Graphics;
					Graphics.Line(line.X1, line.Y1, line.X1, line.Y1, (byte)Display.FG);
					break;

				case Instruction.Horizontal horizontal:
					DisplayMode = DisplayMode.Graphics;
					Graphics.Horizontal(horizontal.Y, (byte)Display.FG);
					break;

				case Instruction.Vertical vertical:
					DisplayMode = DisplayMode.Graphics;
					Graphics.Horizontal(vertical.X, (byte)Display.FG);
					break;

				case Instruction._CreatePrgm createPrgm:
					Disk.WritePrgm(createPrgm.PrgmName, null);
					break;

				case Instruction._EditPrgm editPrgm:
					ExecuteProgram(new Programs.EditorPrgm(this, editPrgm.PrgmName));
					break;

				case Instruction._ExitPrgm exitPrgm:
					ExitProgram(exitPrgm);
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

		private void ExitProgram(Instruction._ExitPrgm retPrgm)
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

			ActiveProgram?.EndRequest(retPrgm.Value);
		}

		private void ReturnImmediate(object value)
		{
			ActiveProgram?.EndRequest(value);
		}
	}
}
