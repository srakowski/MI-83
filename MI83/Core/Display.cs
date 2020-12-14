namespace MI83.Core
{
	using Microsoft.Xna.Framework;
	using System;

	class Display
	{
		public readonly static Color[] ColorPalette =
			new Color[]
			{
				new Color(195,203,178),
				Color.Black,        Color.White,    Color.Gray,
				Color.Blue,         Color.Red,      Color.Green,    Color.Yellow,
				Color.Purple,       Color.Orange,   Color.Cyan,     Color.Black,
				Color.Black,        Color.Black,    Color.Black,    Color.Black
			};

		public readonly static Resolution[] SupportedResolutions =
			new Resolution[]
			{
				new Resolution(192, 128),
				new Resolution(96, 64),
				new Resolution(384, 256),
			};

		public event EventHandler<ResolutionChangedEventArgs> OnResolutionChanged;

		private DisplayByte[,] _buffer;

		public Display()
		{
			UpdateResolution(0);
		}

		public Resolution Resolution => new Resolution(_buffer.GetLength(1), _buffer.GetLength(0));

		public DisplayByte this[int y, int x]
		{
			get => _buffer[y, x];
			set
			{
				var res = Resolution;
				if (y < 0 || y >= res.Height || x < 0 || x >= res.Width)
					return;

				_buffer[y, x] = value;
			}
		}

		public void UpdateResolution(int supportedResolutionIdx)
		{
			var safeIdx = supportedResolutionIdx % SupportedResolutions.Length;
			var resolution = SupportedResolutions[safeIdx];
			_buffer = new DisplayByte[resolution.Height, resolution.Width];
			OnResolutionChanged?.Invoke(this, new ResolutionChangedEventArgs(Resolution));
		}

		public void Walk(Action<Point, Color> onPixel)
		{
			for (var y = 0; y < _buffer.GetLength(0); y++)
			{
				for (var x = 0; x < _buffer.GetLength(1); x++)
				{
					onPixel(new Point(x, y), ColorPalette[_buffer[y, x]]);
				}
			}
		}

		public void Clear(byte color)
		{
			var safeColor = (byte)(color % ColorPalette.Length);
			for (var y = 0; y < _buffer.GetLength(0); y++)
			{
				for (var x = 0; x < _buffer.GetLength(1); x++)
				{
					_buffer[y, x] = safeColor;
				}
			}
		}
	}

	struct DisplayByte
	{
		private byte _value;

		public DisplayByte(int value)
		{
			_value = (byte)(value % Display.ColorPalette.Length);
		}

		public static implicit operator DisplayByte(int value) => new DisplayByte(value);

		public static implicit operator DisplayByte(byte value) => new DisplayByte(value);

		public static implicit operator int(DisplayByte value) => value._value;

		public static implicit operator byte(DisplayByte value) => value._value;
	}

	struct Resolution
	{
		public Resolution(int width, int height)
		{
			Width = width;
			Height = height;
		}

		public int Width { get; }

		public int Height { get; }
	}
}
