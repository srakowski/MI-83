namespace MI83.Core.Buffers
{
	using Microsoft.Xna.Framework;
	using System;
	using System.Collections.Generic;

	class Input
	{
		private List<char> _textInput;
		private int _lastKeyUp;

		public int GetLastKeyUp()
		{
			var value = _lastKeyUp;
			_lastKeyUp = -1;
			return value;
		}

		public void BeginTextInput()
		{
			_textInput = new List<char>();
		}

		public char[] GetTextInput()
		{
			var value = _textInput.ToArray();
			_textInput.Clear();
			return value;
		}

		public void EndTextInput()
		{
			_textInput?.Clear();
			_textInput = null;
		}

		public void Window_TextInput(object sender, TextInputEventArgs e)
		{
			if (_textInput == null) return;
			_textInput.Add(e.Character);
		}

		public void Window_KeyDown(object sender, InputKeyEventArgs e)
		{
		}

		public void Window_KeyUp(object sender, InputKeyEventArgs e)
		{
			_lastKeyUp = (int)e.Key;
		}

		public void Reset()
		{
			_lastKeyUp = -1;
			_textInput?.Clear();
			_textInput = null;
		}
	}
}
