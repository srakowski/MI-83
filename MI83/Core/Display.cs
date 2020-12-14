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

		private DisplayByte[,] _buffer;

		public Display()
		{
			UpdateResolution(0);
		}

		public Resolution Resolution => new Resolution(_buffer.GetLength(1) * 2, _buffer.GetLength(0));

		public void UpdateResolution(int supportedResolutionIdx)
		{
			var safeIdx = supportedResolutionIdx % SupportedResolutions.Length;
			var resolution = SupportedResolutions[safeIdx];
			_buffer = new DisplayByte[resolution.Height, resolution.Width / 2];
		}

		public void Walk(Action<Point, Color> onPixel)
		{
			for (var y = 0; y < _buffer.GetLength(0); y++)
			{
				for (var x = 0; x < _buffer.GetLength(1); x++)
				{
					var offset = x * 2;
					onPixel(new Point(offset, y), ColorPalette[_buffer[y, x].Pixel1]);
					onPixel(new Point(offset + 1, y), ColorPalette[_buffer[y, x].Pixel2]);
				}
			}
		}
	}

	struct DisplayByte
	{
		private const byte UpperMask = 0b1111_0000;
		private const byte LowerMask = 0b0000_1111;

		private byte _data;

		public DisplayByte(byte pixel1, byte pixel2)
		{
			_data = (byte)(((pixel1 & LowerMask) << 4) | (pixel2 & LowerMask));
		}

		public byte Pixel1
		{
			get => (byte)((_data >> 4) & LowerMask);
			set => _data = (byte)((_data & LowerMask) | ((value & LowerMask) << 4));
		}

		public byte Pixel2
		{
			get => (byte)(_data & LowerMask);
			set => _data = (byte)((_data & UpperMask) | (value & LowerMask));
		}
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
