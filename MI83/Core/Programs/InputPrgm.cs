namespace MI83.Core.Programs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	class InputPrgm : Program
	{
		private readonly string _prompt;

		public InputPrgm(Computer computer, string prompt) : base(computer)
		{
			_prompt = prompt;
		}

		protected override object Main()
		{
			var (rows, cols) = _GetHomeDim();
			var (row, col) = _Prompt(_prompt ?? "?");

			var buffer = new StringBuilder();
			var bufferIdx = 0;

			_BeginText();

			var cursorOn = true;
			var cursorTimer = 0;

			var end = false;
			while (!end)
			{
				Thread.Sleep(1);

				var text = _GetText();

				foreach (var ch in text)
				{
					if (ch == '\n' || ch == '\r')
					{
						end = true;
						break;
					}
					else if (ch == '\b')
					{
						if (bufferIdx > 0)
						{
							buffer.Remove(bufferIdx - 1, 1);
							bufferIdx--;
							buffer.Append('\0');
						}
					}
					else
					{
						buffer.Insert(bufferIdx, ch);
						bufferIdx++;
					}
				}

				if (end)
				{
					break;
				}

				var r = row;
				var c = col;
				foreach (var ch in buffer.ToString())
				{
					if (r >= 0)
					{
						Output(r, c, ch.ToString());
					}
					c++;
					if (c >= cols)
					{
						r++;
						c = 0;
						if (r >= rows)
						{
							_Scroll();
							row--;
							r--;
						}
					}
				}

				var cr = row + ((bufferIdx + col) / cols);
				var cc = (bufferIdx + col) % cols;
				_Cursor(cr, cc, cursorOn);

				cursorTimer += 10;
				if (cursorTimer > 200)
				{
					cursorOn = !cursorOn;
					cursorTimer = 0;
				}
			}

			_EndText();
			_Cursor(0, 0, on: false);

			buffer.Replace("\0", null);
			return buffer.ToString();
		}
	}
}
