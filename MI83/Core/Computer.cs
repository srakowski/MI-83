using MI83.Core.Buffers;
using System.Collections.Generic;
using System.Linq;
using InputBuffer = MI83.Core.Buffers.Input;

namespace MI83.Core;

class Computer
{
	private Stack<IProgram> _programStack;

	public Computer()
	{
		_programStack = new Stack<IProgram>();
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

	public IProgram ActiveProgram => _programStack.Any() ? _programStack.Peek() : null;

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
		_programStack.Push(new MI83BasicProgram(code));
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
			or "Pause"
			or "Disp"
			or "ClrHome"
			or "GetSuppDispRes"
			or "SetDispRes"
			or "ExitPrgm";

    public MI83BasicProgram.TypedValue ExecuteSysCommand(string cmd, MI83BasicProgram.ListValue parms)
    {
		System.Console.WriteLine($"{cmd} executed with {parms?.Values?.Count ?? 0} params.");

		if (cmd is "Disp")
        {
			DisplayMode = DisplayMode.Home;
			HomeBuffer.Disp((parms.Values.First() as MI83BasicProgram.StringValue).Value, (byte)5, (byte)0);
		}

		if (cmd is "Pause")
        {
			_programStack.Push(new PauseProgram());
        }

		return new MI83BasicProgram.ListValue(new List<MI83BasicProgram.TypedValue>
        {
			new MI83BasicProgram.NumericValue(2),
			new MI83BasicProgram.NumericValue(0),
		});
    }
}
