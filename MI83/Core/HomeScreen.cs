namespace MI83.Core
{
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Input;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;

	class HomeScreen : IDisplayMode
	{
		private readonly Computer _computer;
		private SysFont _font;
		private CharCellBuffer _buffer;
		private Cursor _cursor;
		private Queue<char> _inputBuffer;

		public HomeScreen(Computer computer)
		{
			_computer = computer;
			_computer.Display.OnResolutionChanged += Display_OnResolutionChanged;

			_font = new SysFont();

			_buffer = new CharCellBuffer(0, 0, FG, BG);
			_cursor = new Cursor(_buffer);

			ResizeBufferToResolution(_computer.Display.Resolution);
		}

		public int Rows => _buffer.Rows;

		public int Cols => _buffer.Cols;

		public int FG { get; private set; } = 1;

		public int BG { get; private set; } = 0;

		public void ClrHome()
		{
			_computer.DisplayHomeScreen();
			_buffer.Clear(FG, BG);
			_cursor.SetPosition(0, 0);
		}

		public void Output(int row, int col, string text)
		{
			_computer.DisplayHomeScreen();
			_cursor.SetPosition(row, col);
			foreach (var c in text)
			{
				_cursor.WriteChar(c, FG, BG, OverflowMode.WrapAndTruncate);
			}
		}

		public void Disp(string text)
		{
			_computer.DisplayHomeScreen();
			foreach (var c in text)
			{
				_cursor.WriteChar(c, FG, BG, OverflowMode.WrapAndScroll);
			}
		}

		public void DispGraph()
		{
			_computer.DisplayGraphScreen();
		}

		public string Input(string prompt)
		{
			_computer.DisplayHomeScreen();
			Disp(prompt);
			_inputBuffer = new Queue<char>();
			var value = new StringBuilder();
			var end = false;
			while (!end)
			{
				Thread.Sleep(1);
				while (_inputBuffer.Count > 0 && !end)
				{
					var next = _inputBuffer.Dequeue();
					switch (next)
					{
						case '\r':
							_cursor.WriteChar(next, FG, BG, OverflowMode.WrapAndScroll);
							end = true;
							break;
						case '\b':
							if (value.Length > 0)
							{
								_cursor.WriteChar(next, FG, BG, OverflowMode.WrapAndScroll);
								value.Remove(value.Length - 1, 1);
							}
							break;
						default:
							if (!char.IsControl(next))
							{
								_cursor.WriteChar(next, FG, BG, OverflowMode.WrapAndScroll);
								value.Append(next);
							}
							break;
					}
				}
			}
			_inputBuffer = null;
			return value.ToString();
		}

		public void Pause()
		{
			_computer.DisplayHomeScreen();
		}

		public (int, int) Menu(IEnumerable<object> menu)
		{
			ClrHome();

			var selectedTabIdx = 0;
			var selectedOptIdx = 0;

			var tabs = menu.Select(tab =>
			{
				var t = (tab as IEnumerable<object>).ToArray();
				return new
				{
					Name = t[0] as string,
					Options = (t[1] as IEnumerable<object>).Select(o => o as string).ToArray()
				};
			}).ToArray();

			var selection = (-1, -1);
			while (selection is (-1, -1))
			{
				_buffer.Clear(FG, BG);
				_cursor.SetPosition(0, 0);
				var selectedTab = tabs[selectedTabIdx];
				foreach (var tab in tabs)
				{
					_cursor.Write(
						tab.Name,
						tab == selectedTab ? BG : FG,
						tab == selectedTab ? FG : BG,
						OverflowMode.WrapAndTruncate);

					_cursor.WriteChar(' ', FG, BG, OverflowMode.WrapAndTruncate);
				}

				var selectedOpt = selectedOptIdx <= selectedTab.Options.Length
					? selectedTab.Options[selectedOptIdx]
					: null;

				var r = 1;
				_cursor.SetPosition(r, 0);
				foreach (var opt in selectedTab.Options)
				{
					var num = $"{r}:";
					_cursor.Write(
						num,
						opt == selectedOpt ? BG : FG,
						opt == selectedOpt ? FG : BG,
						OverflowMode.WrapAndTruncate);

					_cursor.Write(opt, FG, BG, OverflowMode.WrapAndTruncate);
					_cursor.SetPosition(++r, 0);
				}

				var prevKB = new KeyboardState();
				var currKB = new KeyboardState();
				while (true)
				{
					prevKB = currKB;
					currKB = Keyboard.GetState();
					if (prevKB.IsKeyDown(Keys.Right) && currKB.IsKeyUp(Keys.Right))
					{
						selectedTabIdx++;
						selectedTabIdx = selectedTabIdx >= tabs.Length ? 0 : selectedTabIdx;
						selectedOptIdx = 0;
						break;
					}
					else if (prevKB.IsKeyDown(Keys.Left) && currKB.IsKeyUp(Keys.Left))
					{
						selectedTabIdx--;
						selectedTabIdx = selectedTabIdx < 0 ? tabs.Length - 1 : selectedTabIdx;
						selectedOptIdx = 0;
						break;
					}
					else if (prevKB.IsKeyDown(Keys.Up) && currKB.IsKeyUp(Keys.Up))
					{
						selectedOptIdx--;
						selectedOptIdx = selectedOptIdx < 0 ? selectedTab.Options.Length - 1 : selectedOptIdx;
						break;
					}
					else if (prevKB.IsKeyDown(Keys.Down) && currKB.IsKeyUp(Keys.Down))
					{
						selectedOptIdx++;
						selectedOptIdx = selectedOptIdx >= selectedTab.Options.Length ? 0 : selectedOptIdx;
						break;
					}
					else if (prevKB.IsKeyDown(Keys.Enter) && currKB.IsKeyUp(Keys.Enter) &&
						selectedTabIdx >= 0 && selectedOptIdx >= 0)
					{
						selection = (selectedTabIdx, selectedOptIdx);
						break;
					}
					Thread.Sleep(1);
				}
			}
			return selection;
		}

		public int GetKey()
		{
			_computer.DisplayHomeScreen();
			return 0;
		}

		public void Title(string text)
		{
			_computer.DisplayHomeScreen();
		}

		public void SetFG(int paletteIdx)
		{
			_computer.DisplayHomeScreen();
			FG = paletteIdx % Display.ColorPalette.Length;
		}

		public void SetBG(int paletteIdx)
		{
			_computer.DisplayHomeScreen();
			BG = paletteIdx % Display.ColorPalette.Length;
		}

		public void Render(Display display)
		{
			for (var row = 0; row < _buffer.Rows; row++)
			{
				for (var col = 0; col < _buffer.Cols; col++)
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

		public void Window_TextInput(object sender, TextInputEventArgs e)
		{
			_inputBuffer?.Enqueue(e.Character);
		}

		private void Display_OnResolutionChanged(object sender, ResolutionChangedEventArgs e)
		{
			ResizeBufferToResolution(e.NewResolution);
		}

		private void ResizeBufferToResolution(Resolution res)
		{
			_buffer.Resize(res.Height / SysFont.CharHeight, res.Width / SysFont.CharWidth, FG, BG);
		}
	}

	enum OverflowMode : byte
	{
		WrapAndTruncate,
		WrapAndScroll
	}

	class Cursor
	{
		private readonly CharCellBuffer _buffer;
		private int _row = 0;
		private int _col = 0;

		public Cursor(CharCellBuffer buffer)
		{
			_buffer = buffer;
		}

		public void SetPosition(int row, int col)
		{
			_row = row >= 0 ? row : 0;
			_col = col >= 0 ? col : 0;
		}

		public void Write(string value, int fg, int bg, OverflowMode overflowMode)
		{
			foreach (var c in value)
			{
				WriteChar(c, fg, bg, overflowMode);
			}
		}

		public void WriteChar(char value, int fg, int bg, OverflowMode overflowMode)
		{
			if (_col >= _buffer.Cols)
			{
				Wrap();
			}

			if (_row >= _buffer.Rows)
			{
				if (overflowMode == OverflowMode.WrapAndScroll)
				{
					Scroll(fg, bg);
				}
				else if (overflowMode == OverflowMode.WrapAndTruncate)
				{
					return;
				}
				else
				{
					throw new Exception($"{nameof(OverflowMode)} not recognized, value {overflowMode}");
				}
			}

			switch (value)
			{
				case '\r':
				case '\n':
					Wrap();
					break;

				case '\b':
					_col--;
					_col = _col < 0 ? 0 : _col;
					_buffer[_row, _col] = new CharCell('\0', fg, bg);
					break;

				default:
					_buffer[_row, _col] = new CharCell(value, fg, bg);
					_col++;
					break;
			}
		}

		private void Wrap()
		{
			_col = 0;
			_row++;
		}

		private void Scroll(int fg, int bg)
		{
			for (var row = 0; row < _buffer.Rows; row++)
			{
				for (var col = 0; col < _buffer.Cols; col++)
				{
					_buffer[row, col] = row < _buffer.Rows - 1
						? _buffer[row + 1, col]
						: new CharCell('\0', fg, bg);
				}
			}
			_row = _buffer.Rows - 1;
		}
	}

	class CharCellBuffer
	{
		private CharCell[,] _buffer;

		public CharCellBuffer(int rows, int cols, int fg, int bg)
		{
			Resize(rows, cols, fg, bg);
		}

		public int Rows => _buffer.GetLength(0);

		public int Cols => _buffer.GetLength(1);

		public CharCell this[int row, int col]
		{
			get => _buffer[row, col];
			set => _buffer[row, col] = value;
		}

		public void Clear(int fg, int bg)
		{
			for (var row = 0; row < Rows; row++)
			{
				for (var col = 0; col < Cols; col++)
				{
					_buffer[row, col] = new CharCell('\0', fg, bg);
				}
			}
		}

		public void Resize(int rows, int cols, int fg, int bg)
		{
			_buffer = new CharCell[rows, cols];
			Clear(fg, bg);
		}
	}

	struct CharCell
	{
		public CharCell(char value, int fg, int bg)
		{
			Value = value;
			FG = fg;
			BG = bg;
		}

		public char Value;
		public int FG;
		public int BG;
	}
}
