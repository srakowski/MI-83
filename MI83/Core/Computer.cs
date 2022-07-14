using MI83.Core.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InputBuffer = MI83.Core.Buffers.Input;
using static MI83.Core.SysCommands;
using static MI83.Core.MI83BasicProgram;

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
		RunProgram("ROOT");
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

	public void RunProgram(string prgmName)
    {
        if (Shutdown) return;
        var code = Disk.ReadPrgm(prgmName);
        var program = new MI83BasicProgram(code);
        RunPrgm(program);
    }

    public void RunPrgm(IProgram program)
    {
        _programStack.Push(program);
    }

    public void ExitProgram()
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
		ExitProgram();
	}

	public void ExitWithRuntimeError()
    {
		ExitProgram();
    }

	public bool HasSysCommand(string value) =>
		typeof(SysCommands)
			.GetMethods(BindingFlags.Static | BindingFlags.Public)
			.Any(c => c.Name == value);

    public TypedValue ExecuteSysCommand(string cmd, ListValue parms)
    {
		// System.Console.WriteLine($"{cmd} executed with {parms?.Values?.Count ?? 0} params.");

		if (cmd is "Disp")
        {
			var p1 = parms.Values.First();
			switch (p1)
			{
				case StringValue sv: this.Disp(sv.Value); break;
				case NumericValue nv: this.Disp(nv.Value.ToString()); break;
				default: this.Disp("?"); break;
			}
		}

        if (cmd is "Pause")
        {
			this.Pause();
        }

		if (cmd is "GetKey")
        {
			return new NumericValue(this.GetKey());
        }

		if (cmd is "ExitPrgm")
        {
			this.ExitPrgm();
        }

		return new ListValue(new List<TypedValue>
        {
			new NumericValue(2),
			new NumericValue(0),
		});
    }
}
