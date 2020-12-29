namespace MI83.Core.Buffers
{
	using System.Linq;

	class Graphics
	{
		private readonly byte[,] _buffer;

		public Graphics(int width, int height)
		{
			_buffer = new byte[height, width];
		}

		public void ClrDraw(byte color)
		{
			for (var y = 0; y < _buffer.GetLength(0); y++)
			{
				for (var x = 0; x < _buffer.GetLength(1); x++)
				{
					_buffer[y, x] = color;
				}
			}
		}

		public void Pixel(int x, int y, byte color)
		{
			Plot(x, y, color);
		}

		public void Line(int x1, int y1, int x2, int y2, byte color)
		{
			var dx = x2 - x1;
			var dir = x2 < x1 ? -1 : 1;
			var dy = y2 - y1;
			for (var i = 0; i < dx; i++)
			{
				var x = x1 + (i * dir);
				var y = y1 + dy * (x - x1) / dx;
				Plot(x, y, color);
			}
		}

		public void Horizontal(int y, byte color)
		{
			for (var x = 0; x < _buffer.GetLength(1); x++)
			{
				Plot(x, y, color);
			}
		}

		public void Vertical(int x, byte color)
		{
			for (var y = 0; y < _buffer.GetLength(0); y++)
			{
				Plot(x, y, color);
			}
		}

		private void Plot(int x, int y, byte color)
		{
			if (x < 0 || x >= _buffer.GetLength(1) ||
				y < 0 || y >= _buffer.GetLength(0))
			{
				return;
			}

			_buffer[y, x] = color;
		}

		// TODO
		// Circle(x, y, r)|Draws a circle centered on x/y coordinate with the given radius.|
		// Rectangle(x1, y1, x2, y2)|Draws a rectangle from x1/y1 coordinates to x2/y2.|
		// Sprite(sprite_idx, x, y)|Draw a sprite from the sprite buffer to the screen at x/y coordinates.|
		// StorePic(buf_idx)|Takes a 'picture' of the current screen and stores to buffer index.|
		// RecallPic(buf_idx)|Draws the picture at the buffer index.|

		public void Render(Display display)
		{
			for (var y = 0; y < _buffer.GetLength(0); y++)
			{
				for (var x = 0; x < _buffer.GetLength(1); x++)
				{
					display[y, x] = _buffer[y, x];
				}
			}
		}
	}
}
