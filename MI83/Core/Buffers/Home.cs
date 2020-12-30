namespace MI83.Core.Buffers
{
	using Microsoft.Xna.Framework;
	using System;
	using System.Linq;

	class Home
	{
		struct Cursor
		{
			public int Row;
			public int Col;
		}

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

		public void Clear(byte color)
		{
			for (var row = 0; row < Rows; row++)
			{
				for (var col = 0; col < Cols; col++)
				{
					_buffer[row, col] = new Cell('\0', color, color);
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

			while (!IsBlankRow(cursor.Row) && cursor.Row < Rows)
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
				var text = value.ToString();
				foreach (var ch in text.Substring(0, Cols).Reverse())
				{
					_buffer[cursor.Row, Cols - 1 - cursor.Col] = new Cell(ch, fg, bg);
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

		public void Render(Display display)
		{
			for (var row = 0; row < Rows; row++)
			{
				for (var col = 0; col < Cols; col++)
				{
					RenderCharCellAt(display, row, col);
				}
			}
		}

		private void RenderCharCellAt(Display display, int row, int col)
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
						? charCell.FG
						: charCell.BG;
				}
			}
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

		private void Scroll(byte fg, byte bg)
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
	}
}

//namespace MI83.Core
//{
//	using MI83.Infrastructure;
//	using Microsoft.Xna.Framework;
//	using Microsoft.Xna.Framework.Input;
//	using System;
//	using System.Collections.Generic;
//	using System.Linq;
//	using System.Text;
//	using System.Threading;

//	class HomeScreen : IDisplayMode
//	{
//		private readonly Computer _computer;
//		private SysFont _font;
//		private CharCellBuffer _buffer;
//		private Cursor _cursor;
//		private Queue<char> _inputBuffer;
//		private Queue<Keys> _keyUpBuffer;
//		private Keys? _lastKeyUp;

//		public HomeScreen(Computer computer)
//		{
//			_computer = computer;
//			_computer.Display.OnResolutionChanged += Display_OnResolutionChanged;

//			_font = new SysFont();

//			_buffer = new CharCellBuffer(0, 0, FG, BG);
//			_cursor = new Cursor(_buffer);

//			ResizeBufferToResolution(_computer.Display.Resolution);
//		}

//		public int Rows => _buffer.Rows;

//		public int Cols => _buffer.Cols;

//		public int FG => _computer.Display.FG;

//		public int BG => _computer.Display.BG;

//		public void ClrHome()
//		{
//			_computer.DisplayMode = DisplayMode.Home;
//			_buffer.Clear(FG, BG);
//			_cursor.SetPosition(0, 0);
//		}

//		public void Output(int row, int col, string text)
//		{
//			_computer.DisplayMode = DisplayMode.Home;
//			_cursor.SetPosition(row, col);
//			foreach (var c in text)
//			{
//				_cursor.WriteChar(c, FG, BG, OverflowMode.WrapAndTruncate);
//			}
//		}

//		public void Disp(string text)
//		{
//			_computer.DisplayMode = DisplayMode.Home;
//			foreach (var c in text)
//			{
//				_cursor.WriteChar(c, FG, BG, OverflowMode.WrapAndScroll);
//			}
//		}

//		public string Input(string prompt)
//		{
//			_computer.DisplayMode = DisplayMode.Home;
//			Disp(prompt);
//			_inputBuffer = new Queue<char>();
//			_keyUpBuffer = new Queue<Keys>();
//			var value = new StringBuilder();
//			var cursorBlink = 0;
//			var cursorOn = true;
//			var end = false;
//			while (!end)
//			{
//				Thread.Sleep(1);
//				cursorBlink++;

//				while (_keyUpBuffer.Any() && !end)
//				{
//					var next = _keyUpBuffer.Dequeue();
//					if (next == Keys.Enter)
//					{
//						_cursor.WriteChar('\n', FG, BG, OverflowMode.WrapAndScroll);
//						end = true;
//					}
//					else if (next == Keys.Back && value.Length > 0)
//					{
//						_cursor.Render(FG, BG, on: false);
//						_cursor.WriteChar('\b', FG, BG, OverflowMode.WrapAndScroll);
//						value.Remove(value.Length - 1, 1);
//					}
//				}

//				while (_inputBuffer.Any() && !end)
//				{
//					var next = _inputBuffer.Dequeue();
//					if (!char.IsControl(next))
//					{
//						_cursor.WriteChar(next, FG, BG, OverflowMode.WrapAndScroll);
//						value.Append(next);
//					}
//				}

//				if (cursorBlink > 300)
//				{
//					cursorOn = !cursorOn;
//					cursorBlink = 0;
//				}

//				_cursor.Render(FG, BG, cursorOn);
//			}

//			_inputBuffer = null;
//			_keyUpBuffer = null;
//			return value.ToString();
//		}

//		public void SetCursor(int row, int col)
//		{
//			_cursor.SetPosition(row, col);
//		}

//		public void RenderCursor()
//		{
//			_cursor.Render(FG, BG, true);
//		}

//		public void Pause()
//		{
//			_keyUpBuffer = new Queue<Keys>();
//			var end = false;
//			while (!end)
//			{
//				while (_keyUpBuffer.Any())
//				{
//					var key = _keyUpBuffer.Dequeue();
//					if (key == Keys.Enter)
//					{
//						end = true;
//						break;
//					}
//				}
//				Thread.Sleep(1);
//			}
//			_keyUpBuffer = null;
//		}

//		public (int, int) Menu(IEnumerable<object> menu)
//		{
//			ClrHome();

//			var selectedTabIdx = 0;
//			var selectedOptIdx = 0;

//			var tabs = menu.Select(tab =>
//			{
//				var t = (tab as IEnumerable<object>).ToArray();
//				return new
//				{
//					Name = t[0] as string,
//					Options = t.Skip(1).Select(o => o as string).ToArray()
//				};
//			}).ToArray();

//			_keyUpBuffer = new Queue<Keys>();
//			var selection = (-1, -1);
//			while (selection is (-1, -1))
//			{
//				_buffer.Clear(FG, BG);
//				_cursor.SetPosition(0, 0);
//				var selectedTab = tabs[selectedTabIdx];
//				foreach (var tab in tabs)
//				{
//					_cursor.Write(
//						tab.Name,
//						tab == selectedTab ? BG : FG,
//						tab == selectedTab ? FG : BG,
//						OverflowMode.WrapAndTruncate);

//					_cursor.WriteChar(' ', FG, BG, OverflowMode.WrapAndTruncate);
//				}

//				var selectedOpt = selectedOptIdx < selectedTab.Options.Length
//					? selectedTab.Options[selectedOptIdx]
//					: null;

//				var r = 1;
//				_cursor.SetPosition(r, 0);
//				foreach (var opt in selectedTab.Options)
//				{
//					var num = $"{r}:";
//					_cursor.Write(
//						num,
//						opt == selectedOpt ? BG : FG,
//						opt == selectedOpt ? FG : BG,
//						OverflowMode.WrapAndTruncate);

//					_cursor.Write(opt, FG, BG, OverflowMode.WrapAndTruncate);
//					_cursor.SetPosition(++r, 0);
//				}

//				while (!_keyUpBuffer.Any())
//				{
//					Thread.Sleep(1);
//				}

//				while (_keyUpBuffer.Any())
//				{
//					var key = _keyUpBuffer.Dequeue();
//					if (key == Keys.Right)
//					{
//						selectedTabIdx++;
//						selectedTabIdx = selectedTabIdx >= tabs.Length ? 0 : selectedTabIdx;
//						selectedOptIdx = 0;
//						break;
//					}
//					else if (key == Keys.Left)
//					{
//						selectedTabIdx--;
//						selectedTabIdx = selectedTabIdx < 0 ? tabs.Length - 1 : selectedTabIdx;
//						selectedOptIdx = 0;
//						break;
//					}
//					else if (key == Keys.Up)
//					{
//						selectedOptIdx--;
//						selectedOptIdx = selectedOptIdx < 0 ? selectedTab.Options.Length - 1 : selectedOptIdx;
//						break;
//					}
//					else if (key == Keys.Down)
//					{
//						selectedOptIdx++;
//						selectedOptIdx = selectedOptIdx >= selectedTab.Options.Length ? 0 : selectedOptIdx;
//						break;
//					}
//					else if (key == Keys.Enter &&
//						selectedTabIdx >= 0 && selectedOptIdx >= 0)
//					{
//						selection = (selectedTabIdx, selectedOptIdx);
//						break;
//					}
//				}
//			}
//			_keyUpBuffer = null;
//			return selection;
//		}

//		/// <summary>
//		/// TODO: move out of Home Screen...
//		/// </summary>
//		/// <returns></returns>
//		public int GetKey()
//		{
//			var value = _lastKeyUp.HasValue ? (int)_lastKeyUp.Value : -1;
//			_lastKeyUp = null;
//			return value;
//		}

//		public void Render(Display display)
//		{
//			for (var row = 0; row < _buffer.Rows; row++)
//			{
//				for (var col = 0; col < _buffer.Cols; col++)
//				{
//					RenderCharCellAt(display, row, col);
//				}
//			}
//		}

//		private void RenderCharCellAt(Display display, int row, int col)
//		{
//			var charCell = _buffer[row, col];
//			var charBitmap = _font[charCell.Value];
//			for (var y = 0; y < SysFont.CharHeight; y++)
//			{
//				for (var x = 0; x < SysFont.CharWidth; x++)
//				{
//					var pixelOn = charBitmap[x, y];
//					var pixelX = (col * SysFont.CharWidth) + x;
//					var pixelY = (row * SysFont.CharHeight) + y;

//					display[pixelY, pixelX] = pixelOn
//						? charCell.FG
//						: charCell.BG;
//				}
//			}
//		}

//		public void Window_TextInput(object sender, TextInputEventArgs e)
//		{
//			_inputBuffer?.Enqueue(e.Character);
//		}

//		public void Window_KeyUp(object sender, InputKeyEventArgs e)
//		{
//			_keyUpBuffer?.Enqueue(e.Key);
//			_lastKeyUp = e.Key;
//		}

//		private void Display_OnResolutionChanged(object sender, ResolutionChangedEventArgs e)
//		{
//			ResizeBufferToResolution(e.NewResolution);
//		}

//		private void ResizeBufferToResolution(Resolution res)
//		{
//			_buffer.Resize(res.Height / SysFont.CharHeight, res.Width / SysFont.CharWidth, FG, BG);
//		}
//	}

//	enum OverflowMode : byte
//	{
//		WrapAndTruncate,
//		WrapAndScroll
//	}

//	class Cursor
//	{
//		private readonly CharCellBuffer _buffer;
//		private int _row = 0;
//		private int _col = 0;

//		public Cursor(CharCellBuffer buffer)
//		{
//			_buffer = buffer;
//		}

//		public void SetPosition(int row, int col)
//		{
//			_row = row >= 0 ? row : 0;
//			_col = col >= 0 ? col : 0;
//		}

//		public void Render(int fg, int bg, bool on)
//		{
//			var truncated = ProcessOverflow(fg, bg, OverflowMode.WrapAndScroll);
//			if (truncated)
//			{
//				return;
//			}
//			_buffer[_row, _col] = new CharCell((char)0, fg, on ? fg : bg);
//		}

//		public void Write(string value, int fg, int bg, OverflowMode overflowMode)
//		{
//			foreach (var c in value)
//			{
//				WriteChar(c, fg, bg, overflowMode);
//			}
//		}

//		public void WriteChar(char value, int fg, int bg, OverflowMode overflowMode)
//		{
//			var truncated = ProcessOverflow(fg, bg, overflowMode);
//			if (truncated)
//			{
//				return;
//			}

//			switch (value)
//			{
//				case '\r':
//				case '\n':
//					Wrap();
//					break;

//				case '\b':
//					_col--;
//					_col = _col < 0 ? 0 : _col;
//					_buffer[_row, _col] = new CharCell('\0', fg, bg);
//					break;

//				default:
//					_buffer[_row, _col] = new CharCell(value, fg, bg);
//					_col++;
//					break;
//			}
//		}

//		private bool ProcessOverflow(int fg, int bg, OverflowMode overflowMode)
//		{
//			if (_col >= _buffer.Cols)
//			{
//				Wrap();
//			}

//			if (_row >= _buffer.Rows)
//			{
//				if (overflowMode == OverflowMode.WrapAndScroll)
//				{
//					Scroll(fg, bg);
//				}
//				else if (overflowMode == OverflowMode.WrapAndTruncate)
//				{
//					return true;
//				}
//				else
//				{
//					throw new Exception($"{nameof(OverflowMode)} not recognized, value {overflowMode}");
//				}
//			}

//			return false;
//		}

//		private void Wrap()
//		{
//			_col = 0;
//			_row++;
//		}

//		private void Scroll(int fg, int bg)
//		{
//			for (var row = 0; row < _buffer.Rows; row++)
//			{
//				for (var col = 0; col < _buffer.Cols; col++)
//				{
//					_buffer[row, col] = row < _buffer.Rows - 1
//						? _buffer[row + 1, col]
//						: new CharCell('\0', fg, bg);
//				}
//			}
//			_row = _buffer.Rows - 1;
//		}
//	}

//	class CharCellBuffer
//	{
//		private CharCell[,] _buffer;

//		public CharCellBuffer(int rows, int cols, int fg, int bg)
//		{
//			Resize(rows, cols, fg, bg);
//		}

//		public int Rows => _buffer.GetLength(0);

//		public int Cols => _buffer.GetLength(1);

//		public CharCell this[int row, int col]
//		{
//			get => _buffer[row, col];
//			set => _buffer[row, col] = value;
//		}

//		public void Clear(int fg, int bg)
//		{
//			for (var row = 0; row < Rows; row++)
//			{
//				for (var col = 0; col < Cols; col++)
//				{
//					_buffer[row, col] = new CharCell('\0', fg, bg);
//				}
//			}
//		}

//		public void Resize(int rows, int cols, int fg, int bg)
//		{
//			_buffer = new CharCell[rows, cols];
//			Clear(fg, bg);
//		}
//	}

//	struct CharCell
//	{
//		public CharCell(char value, int fg, int bg)
//		{
//			Value = value;
//			FG = fg;
//			BG = bg;
//		}

//		public char Value;
//		public int FG;
//		public int BG;
//	}
//}
