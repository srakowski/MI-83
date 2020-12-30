namespace MI83.Core.Buffers
{
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Input;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	class Home
	{
		struct Cell
		{
			public Cell(char value, byte fg, byte bg)
			{
				Value = value;
				FG = fg;
				BG = bg;
			}

			public char Value;
			public int FG;
			public int BG;
		}

		struct Cursor
		{
			public int Row;
			public int Col;
		}

		private Cursor? _cursor;
		private readonly SysFont _font;
		private Cell[,] _buffer;

		public Home(int rows, int cols)
		{
			_font = new SysFont();
			Resize(rows, cols);
		}

		public int Rows => _buffer.GetLength(0);

		public int Cols => _buffer.GetLength(1);

		public void Resize(int rows, int cols)
		{
			_buffer = new Cell[rows, cols];
		}

		public void Clear(byte fg, byte bg)
		{
			for (var row = 0; row < Rows; row++)
			{
				for (var col = 0; col < Cols; col++)
				{
					_buffer[row, col] = new Cell('\0', fg, bg);
				}
			}
		}

		public void Output(int row, int col, string text, byte fg, byte bg)
		{
			if (row < 0 || col < 0 || row >= Rows || col >= Cols)
			{
				throw new IndexOutOfRangeException();
			}

			var cursor = new Cursor { Row = row, Col = col };
			foreach (var ch in text)
			{
				switch (ch)
				{
					case '\n':
					case '\r':
						cursor.Row++;
						cursor.Col = 0;
						break;

					case '\b':
						cursor.Col--;
						if (cursor.Col < 0)
						{
							if (cursor.Row > 0)
							{
								cursor.Row--;
								cursor.Col = cursor.Col - 1;
							}
							else
							{
								cursor.Col = 0;
							}
						}
						break;

					default:
						_buffer[cursor.Row, cursor.Col] = new Cell(ch, fg, bg);
						cursor.Col++;
						break;
				}

				if (cursor.Col >= Cols)
				{
					cursor.Row++;
					cursor.Col = 0;
				}

				if (cursor.Row >= Rows)
				{
					cursor.Col = 0;
					break;
				}
			}
		}

		public void Disp(object value, byte fg, byte bg)
		{
			var cursor = new Cursor { Row = 0, Col = 0 };

			while (cursor.Row < Rows && !IsBlankRow(cursor.Row))
			{
				cursor.Row++;
			}

			if (cursor.Row >= Rows)
			{
				Scroll(fg, bg);
				cursor.Row = Rows - 1;
			}

			if (value is string)
			{
				var text = value as string;
				foreach (var ch in text)
				{
					_buffer[cursor.Row, cursor.Col] = new Cell(ch, fg, bg);
					cursor.Col++;
					if (cursor.Col >= Cols)
					{
						_buffer[cursor.Row, Cols - 1].Value = (char)240;
						break;
					}
				}
			}
			else
			{
				cursor.Col = Cols - 1;
				var text = value.ToString();
				foreach (var ch in text.Substring(0, Math.Min(text.Length, Cols)).Reverse())
				{
					_buffer[cursor.Row, cursor.Col] = new Cell(ch, fg, bg);
					cursor.Col--;
				}
			}

			cursor.Row++;
			cursor.Col = 0;
			if (cursor.Row >= Rows)
			{
				Scroll(fg, bg);
			}
		}

		public (int Row, int Col) Prompt(string text, byte fg, byte bg)
		{
			var cursor = new Cursor { Row = 0, Col = 0 };

			while (!IsBlankRow(cursor.Row) && cursor.Row < Rows)
			{
				cursor.Row++;
			}

			if (cursor.Row >= Rows)
			{
				Scroll(fg, bg);
				cursor.Row = Rows - 1;
			}

			foreach (var ch in text)
			{
				_buffer[cursor.Row, cursor.Col] = new Cell(ch, fg, bg);
				cursor.Col++;
				if (cursor.Col >= Cols)
				{
					_buffer[cursor.Row, Cols - 1].Value = (char)240;
					break;
				}
			}

			if (cursor.Col >= Cols)
			{
				cursor.Col = 0;
				cursor.Row++;
				if (cursor.Row >= Rows)
				{
					Scroll(fg, bg);
				}
			}

			return (cursor.Row, cursor.Col);
		}

		public void Scroll(byte fg, byte bg)
		{
			for (var row = 0; row < Rows; row++)
			{
				for (var col = 0; col < Cols; col++)
				{
					_buffer[row, col] = row < Rows - 1
						? _buffer[row + 1, col]
						: new Cell('\0', fg, bg);
				}
			}
		}

		public void Render(Display display)
		{
			for (var row = 0; row < Rows; row++)
			{
				for (var col = 0; col < Cols; col++)
				{
					RenderCharCellAt(display, row, col, _cursor?.Row == row && _cursor?.Col == col);
				}
			}
		}

		private void RenderCharCellAt(Display display, int row, int col, bool invert)
		{
			var charCell = _buffer[row, col];
			var charBitmap = _font[charCell.Value];
			for (var y = 0; y < SysFont.CharHeight; y++)
			{
				for (var x = 0; x < SysFont.CharWidth; x++)
				{
					var pixelOn = charBitmap[x, y];
					var pixelX = (col * SysFont.CharWidth) + x;
					var pixelY = (row * SysFont.CharHeight) + y;
					display[pixelY, pixelX] = pixelOn
						? (invert ? charCell.BG : charCell.FG)
						: (invert ? charCell.FG : charCell.BG);
				}
			}
		}

		public void RenderCursor(int row, int col, bool on)
		{
			_cursor = on ? new Cursor { Row = row, Col = col } : null;
		}

		private bool IsBlankRow(int row)
		{
			for (var col = 0; col < Cols; col++)
			{
				if (_buffer[row, col].Value != 0)
				{
					return false;
				}
			}
			return true;
		}
	}
}
