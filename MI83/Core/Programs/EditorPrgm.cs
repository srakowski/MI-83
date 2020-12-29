namespace MI83.Core.Programs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	class EditorPrgm : Program
	{
		private readonly string _prgmName;

		public EditorPrgm(Computer computer, string prgmName) : base(computer)
		{
			_prgmName = prgmName;
		}

		protected override object Main()
		{
			ClrHome();
			return null;
		}
	}
}

//public void EditPrgm(string name)
//{
//	var text = Disk.ReadPrgm(name);
//	var codeEditor = new CodeEditor(text);
//	_keyUpBuffer = new Queue<Keys>();
//	_inputBuffer = new Queue<char>();
//	var hs = _computer.Home;
//	var end = false;
//	while (!end)
//	{
//		hs.ClrHome();
//		hs.Disp($"PROGRAM:{name}\n");
//		var visibleLines = codeEditor.GetVisibleLines();
//		foreach (var line in visibleLines)
//		{
//			hs.Disp($":{line}\n");
//		}
//		var cursorPos = codeEditor.GetCursorPos();
//		hs.SetCursor(cursorPos.Y + 1, cursorPos.X + 1);
//		hs.RenderCursor();

//		while (!_keyUpBuffer.Any() && !_inputBuffer.Any())
//		{
//			Thread.Sleep(1);
//		}

//		while (_keyUpBuffer.Any() && !end)
//		{
//			var key = _keyUpBuffer.Dequeue();
//			switch (key)
//			{
//				case Keys.Up: codeEditor.CursorUp(); break;
//				case Keys.Down: codeEditor.CursorDown(); break;
//				case Keys.Left: codeEditor.CursorLeft(); break;
//				case Keys.Right: codeEditor.CursorRight(); break;
//				case Keys.Home: codeEditor.CursorHome(); break;
//				case Keys.End: codeEditor.CursorEnd(); break;
//				case Keys.Escape:
//					end = true;
//					break;
//			}
//		}

//		while (_inputBuffer.Any() && !end)
//		{
//			var next = _inputBuffer.Dequeue();
//			switch (next)
//			{
//				case '\r':
//				case '\n':
//					codeEditor.NewLine();
//					break;
//				case '\b':
//					codeEditor.BackSpace();
//					break;
//				default:
//					if (!char.IsControl(next))
//					{
//						codeEditor.TypeChar(next);
//					}
//					break;
//			}
//		}
//	}
//	_inputBuffer = null;
//	_keyUpBuffer = null;

//	var completedCode = codeEditor.GetCode();
//	Disk.WritePrgm(name, completedCode);
//}

//namespace MI83.Core
//{
//	using Microsoft.Xna.Framework;
//	using System;
//	using System.Collections.Generic;
//	using System.Linq;
//	using System.Text;

//	class CodeEditor
//	{
//		class EditCursor
//		{
//			public EditCursor(int line, int column)
//			{
//				Line = line;
//				Column = column;
//			}
//			public int Line { get; set; }
//			public int Column { get; set; }
//		}

//		class EditRange
//		{
//			public EditRange(int top, int rows)
//			{
//				Top = top;
//				Rows = rows;
//			}
//			public int Top { get; set; }
//			public int Rows { get; set; }
//		}

//		private EditCursor _cursor;
//		private EditRange _editRange;
//		private List<StringBuilder> _code;

//		public CodeEditor(string rawCode)
//		{

//			_cursor = new EditCursor(0, 0);
//			_editRange = new EditRange(0, 10);
//			_code = new List<StringBuilder>(
//				rawCode
//					.Split('\n')
//					.Select(s => s.Replace("\n", "").Replace("\r", ""))
//					.Select(l => new StringBuilder(l))
//				);
//		}

//		public string GetCode()
//		{
//			return string.Join("\r\n", _code.Select(l => l.ToString()));
//		}

//		public void CursorUp()
//		{
//			_cursor.Line--;
//			_cursor.Line = _cursor.Line < 0 ? 0 : _cursor.Line;
//			FixCursorColumnInLine();
//			_editRange.Top = _cursor.Line < _editRange.Top
//				? _cursor.Line
//				: _editRange.Top;
//		}

//		public void CursorDown()
//		{
//			_cursor.Line++;

//			_cursor.Line = _cursor.Line >= _code.Count
//				? _code.Count - 1
//				: _cursor.Line;

//			FixCursorColumnInLine();

//			_editRange.Top = _cursor.Line >= _editRange.Top + _editRange.Rows
//				? _cursor.Line - _editRange.Rows
//				: _editRange.Top;
//		}

//		public void CursorLeft()
//		{
//			if (_cursor.Column == 0 && _cursor.Line == 0)
//				return;

//			_cursor.Column--;
//			if (_cursor.Column < 0)
//			{
//				CursorUp();
//				CursorEnd();
//			}
//		}

//		public void CursorRight()
//		{
//			if (_cursor.Column == _code[_cursor.Line].Length &&
//				_cursor.Line == _code.Count - 1)
//				return;

//			_cursor.Column++;
//			if (_cursor.Column > _code[_cursor.Line].Length)
//			{
//				CursorDown();
//				CursorHome();
//			}
//		}

//		public void CursorEnd()
//		{
//			_cursor.Column = _code[_cursor.Line].Length;
//		}

//		public void CursorHome()
//		{
//			_cursor.Column = 0;
//		}

//		private void FixCursorColumnInLine()
//		{
//			if (_cursor.Column > _code[_cursor.Line].Length)
//			{
//				_cursor.Column = _code[_cursor.Line].Length;
//			}
//		}

//		public string[] GetVisibleLines()
//		{
//			return _code
//				.Skip(_editRange.Top)
//				.Take(_editRange.Rows)
//				.Select(sb => sb.ToString())
//				.ToArray();
//		}

//		public Point GetCursorPos()
//		{
//			return new Point(
//				_cursor.Column,
//				_cursor.Line - _editRange.Top
//			);
//		}

//		public void NewLine()
//		{
//			var currentLine = _code[_cursor.Line];
//			var newCodeLine = new StringBuilder();
//			if (_cursor.Column < currentLine.Length)
//			{
//				var sub = currentLine.ToString().Substring(_cursor.Column);
//				newCodeLine.Append(sub);
//				currentLine.Remove(_cursor.Column, sub.Length);
//			}
//			_code.Insert(_cursor.Line + 1, newCodeLine);
//			CursorDown();
//			CursorHome();
//		}

//		public void BackSpace()
//		{
//			var line = _code[_cursor.Line];
//			if (_cursor.Column == 0 && _cursor.Line == 0)
//			{
//				return;
//			}
//			else if (_cursor.Column == 0)
//			{
//				var prevLine = _code[_cursor.Line - 1];
//				CursorUp();
//				CursorEnd();
//				prevLine.Append(line);
//				_code.Remove(line);
//			}
//			else
//			{
//				line.Remove(_cursor.Column - 1, 1);
//				CursorLeft();
//			}
//		}

//		public void TypeChar(char value)
//		{
//			var line = _code[_cursor.Line];
//			line.Insert(_cursor.Column, value);
//			_cursor.Column++;
//		}
//	}
//}
