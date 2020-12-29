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
			Display = new Display();
			System = new Sys(this);
			Home = new HomeScreen(this);
			HomeBuffer = new Buffers.Home();
			Graphics = new GraphicsScreen(this);
			DisplayMode = DisplayMode.Home;
		}

		public bool Shutdown { get; private set; }

		public ConcurrentQueue<Instruction> Instructions { get; }

		public Display Display { get; }

		public DisplayMode DisplayMode { get; set; }

		public Sys System { get; }

		public HomeScreen Home { get; }

		public Buffers.Home HomeBuffer { get; }

		public GraphicsScreen Graphics { get; }

		public void Boot()
		{
			var prog = new Programs.PrgmMenu(this);
			_programStack.Push(prog);
			prog.Execute();
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
			var activeBuffer = DisplayMode == DisplayMode.Home ? (IDisplayMode)Home : Graphics;
			activeBuffer.Render(Display);
		}

		private void Interpret(Instruction instruction)
		{
			switch (instruction)
			{
				case Instruction.RunPrgm runPrgm:
					RunProgram(runPrgm);
					break;

				case Instruction.RetPrgm retPrgm:
					ReturnProgram(retPrgm);
					break;

				case Instruction.DispHome:
					DisplayMode = DisplayMode.Home;
					break;

				case Instruction.DispGraph:
					DisplayMode = DisplayMode.Graphics;
					break;

				case Instruction.GetKey getKey:
					break;

				case Instruction.Pause pause:
					break;

				case Instruction.ClrHome:
					DisplayMode = DisplayMode.Home;
					HomeBuffer.Clear();
					break;
			}
		}

		private void RunProgram(Instruction.RunPrgm runPrgm)
		{
			var code = Disk.ReadPrgm(runPrgm.PrgmName);
			var prog = new Programs.PythonProgram(this, code);
			_programStack.Push(prog);
			prog.Execute();
		}

		private void ReturnProgram(Instruction.RetPrgm retPrgm)
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

			_programStack
				.Peek()
				.EndRequest(retPrgm.Value);
		}
	}
}
