using Microsoft.Xna.Framework;
using System;

namespace MI83.Core.Buffers
{
	class Input
	{
		private int _lastKeyUp;

		public int GetLastKeyUp()
		{
			var value = _lastKeyUp;
			_lastKeyUp = -1;
			return value;
		}

		public void Window_TextInput(object sender, TextInputEventArgs e)
		{
		}

		public void Window_KeyDown(object sender, InputKeyEventArgs e)
		{
		}

		public void Window_KeyUp(object sender, InputKeyEventArgs e)
		{
			_lastKeyUp = (int)e.Key;
		}
	}
}
