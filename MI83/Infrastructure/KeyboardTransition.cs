namespace MI83.Infrastructure
{
	using Microsoft.Xna.Framework.Input;
	using System;

	class KeyboardTransition
	{
		private KeyboardState _prev;
		private KeyboardState _curr;

		public KeyboardTransition()
		{
			_prev = new KeyboardState();
			_curr = Keyboard.GetState();
		}

		public void Update()
		{
			_prev = _curr;
			_curr = Keyboard.GetState();
		}

		public bool WasPressed(Keys key)
		{
			return _prev.IsKeyDown(key) && _curr.IsKeyUp(key);
		}
	}
}
