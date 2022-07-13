using MI83.Core.Buffers;
using System.Collections.Generic;
using System.Linq;
using static MI83.Core.Instruction;
using InputBuffer = MI83.Core.Buffers.Input;

namespace MI83.Core;

class Computer
{
	private Stack<Program> _programStack;

	public Computer()
	{
		_programStack = new Stack<Program>();
		Shutdown = false;
		DisplayBuffer = new Display();
		HomeBuffer = new Home(DisplayBuffer.Resolution.Height / SysFont.CharHeight, DisplayBuffer.Resolution.Width / SysFont.CharWidth);
		GraphicsBuffer = new Graphics(Display.MaxResolution.Width, Display.MaxResolution.Height);
		InputBuffer = new InputBuffer();
		DisplayMode = DisplayMode.Home;
	}

	public bool Shutdown { get; set; }

	public Display DisplayBuffer { get; }

	public DisplayMode DisplayMode { get; set; }

	public Home HomeBuffer { get; }

	public Graphics GraphicsBuffer { get; }

	public InputBuffer InputBuffer { get; }

	public Program ActiveProgram => _programStack.Any() ? _programStack.Peek() : null;

	public void Boot()
	{
		if (Shutdown) return;
		RunPrgm("ROOT");
	}

	public void Tick()
	{
		if (Shutdown) return;
		ActiveProgram?.ExecuteNextInstruction(this);
	}

	public void RenderActiveDisplayMode()
	{
		if (Shutdown) return;
		if (DisplayMode == DisplayMode.Home)
		{
			HomeBuffer.Render(DisplayBuffer);
		}
		else if (DisplayMode == DisplayMode.Graphics)
		{
			GraphicsBuffer.Render(DisplayBuffer);
		}
	}

	public void RunPrgm(string prgmName)
	{
		if (Shutdown) return;
		var code = Disk.ReadPrgm(prgmName);
		_programStack.Push(new Program(code));
	}

    public void ExitPrgm()
    {
		if (Shutdown) return;
		_programStack.Pop();
		if (!_programStack.Any())
        {
			Shutdown = true;
        }
    }

	public void ExitWithSyntaxError()
	{
		ExitPrgm();
	}

	public void ExitWithRuntimeError()
    {
		ExitPrgm();
    }

    public bool HasSysCommand(string value) =>
		value is "GetPrgms"
			or "Menu"
			or "RunPrgm"
			or "EditPrgm"
			or "Input"
			or "CreatePrgm"
			or "Disp"
			or "ClrHome"
			or "GetSuppDispRes"
			or "SetDispRes"
			or "ExitPrgm";

    public Program.TypedValue ExecuteSysCommand(string cmd, Program.ListValue parms)
    {
		System.Console.WriteLine($"{cmd} executed with {parms?.Values?.Count ?? 0} params.");
		return new Program.ListValue(new List<Program.TypedValue>
        {
			new Program.NumericValue(2),
			new Program.NumericValue(0),
		});
    }
}
