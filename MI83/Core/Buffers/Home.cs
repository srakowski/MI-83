namespace MI83.Core.Buffers
{
	using Microsoft.Xna.Framework;
	using System;

	class Cursor
	{
		public int Row { get; set; }
		public int Col { get; set; }
	}

	class Home
	{
		private readonly Cursor _cursor;

		public Home()
		{
			_cursor = new Cursor();
		}

		public void Clear()
		{
		}
	}
}
