namespace MI83.Core
{
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Input;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading;

	class ProgramRegistry
	{
		private const string ProgramsDirectory = "./prgms";
		private Computer _computer;

		public ProgramRegistry(Computer computer)
		{
			_computer = computer;
		}

		public string[] GetPrgms()
		{
			CreatePrgmsDirectoryIfItDoesNotExist();
			return Directory.GetFiles(ProgramsDirectory, "*.prgm")
				.Select(f => Path.GetFileNameWithoutExtension(f))
				.ToArray();
		}

		public void CreatePrgm(string name)
		{
			CreatePrgmsDirectoryIfItDoesNotExist();
			File.WriteAllText(CreatePrgmFileName(name), null);
		}

		private Queue<char> _inputBuffer = null;
		private Queue<Keys> _keyUpBuffer = null;

		public void Window_KeyUp(object sender, InputKeyEventArgs e) => _keyUpBuffer?.Enqueue(e.Key);

		public void Window_TextInput(object sender, TextInputEventArgs e) => _inputBuffer?.Enqueue(e.Character);

		public void EditPrgm(string name)
		{
			var text = File.ReadAllText(CreatePrgmFileName(name));
			var codeEditor = new CodeEditor(text);
			_keyUpBuffer = new Queue<Keys>();
			_inputBuffer = new Queue<char>();
			var hs = _computer.HomeScreen;
			var end = false;
			while (!end)
			{
				hs.ClrHome();
				hs.Disp($"PROGRAM:{name}\n");
				var visibleLines = codeEditor.GetVisibleLines();
				foreach (var line in visibleLines)
				{
					hs.Disp($":{line}\n");
				}
				var cursorPos = codeEditor.GetCursorPos();
				hs.SetCursor(cursorPos.Y + 1, cursorPos.X + 1);
				hs.RenderCursor();

				while (!_keyUpBuffer.Any() && !_inputBuffer.Any())
				{
					Thread.Sleep(1);
				}

				while (_keyUpBuffer.Any() && !end)
				{
					var key = _keyUpBuffer.Dequeue();
					switch (key)
					{
						case Keys.Up: codeEditor.CursorUp(); break;
						case Keys.Down: codeEditor.CursorDown(); break;
						case Keys.Left: codeEditor.CursorLeft(); break;
						case Keys.Right: codeEditor.CursorRight(); break;
						case Keys.Home: codeEditor.CursorHome(); break;
						case Keys.End: codeEditor.CursorEnd(); break;
						case Keys.Escape:
							end = true;
							break;
					}
				}

				while (_inputBuffer.Any() && !end)
				{
					var next = _inputBuffer.Dequeue();
					switch (next)
					{
						case '\r': case '\n':
							codeEditor.NewLine();
							break;
						case '\b':
							codeEditor.BackSpace();
							break;
						default:
							if (!char.IsControl(next))
							{
								codeEditor.TypeChar(next);
							}
							break;
					}
				}
			}
			_inputBuffer = null;
			_keyUpBuffer = null;

			var completedCode = codeEditor.GetCode();
			File.WriteAllText(CreatePrgmFileName(name), completedCode);
		}

		public void RunPrgm(string name)
		{
			var code = File.ReadAllText(CreatePrgmFileName(name));
			var progTask = new Program(_computer, code).Execute();
			while (!progTask.IsCompleted)
			{
				Thread.Sleep(1);
			}
		}

		private static void CreatePrgmsDirectoryIfItDoesNotExist()
		{
			Directory.CreateDirectory(ProgramsDirectory);
		}

		private static string CreatePrgmFileName(string name)
		{
			return Path.Combine(ProgramsDirectory, $"{name.ToUpper()}.prgm");
		}
	}
}
