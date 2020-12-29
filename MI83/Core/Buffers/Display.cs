namespace MI83.Core.Buffers
{
	using Microsoft.Xna.Framework;
	using System;
	using System.Linq;

	class Display
	{
		public readonly static Resolution MaxResolution = new Resolution(288, 192);

		public readonly static Color[] ColorPalette =
			// new Color(0xBC, 0xC1, 0x9C) background?
			new Color[]
			{
				Color.Black,
				new Color(0x36, 0x36, 0x96), // 1 Dark-Blue
				new Color(0x57, 0x1E, 0x57), // 2 Dark-Purple
				new Color(0x3F, 0x5E, 0x3F), // 3 Dark-Green
				new Color(0x7C, 0x5E, 0x40), // 4 Brown
				new Color(0x46, 0x46, 0x40), // 5 Dark-Gray
				new Color(0x9E, 0x9E, 0x91), // 6 Light-Gray
				new Color(0xDD, 0xDD, 0xCC), // 7 White
				new Color(0xB4, 0x4A, 0x4A), // 8 Red
				new Color(0xD6, 0x68, 0x20), // 9 Orange
				new Color(0xDD, 0xD2, 0x11), // 10 Yellow
				new Color(0x7D, 0xCC, 0x7C), // 11 Green
				new Color(0x54, 0x54, 0xCC), // 12 Blue
				new Color(0x8F, 0x67, 0xAA), // 13 lavender
				new Color(0xC9, 0x5C, 0xC9), // 14 Pink
				new Color(0xEF, 0xC4, 0xA7) // 15 Peach Puff
			};

		public readonly static Resolution[] SupportedResolutions =
			new Resolution[]
			{
				new Resolution(192, 128),
				new Resolution(96, 64),
				MaxResolution,
			};

		public int FG { get; private set; } = 5;

		public int BG { get; private set; } = 0;

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

		public string[] GetSuppDispRes()
		{
			return SupportedResolutions
				.Select(r => $"{r.Width}x{r.Height}{(r.Equals(Resolution) ? "*" : "")}")
				.ToArray();
		}

		public int GetDispRes()
		{
			// TODO: less ugly way to do this?
			return SupportedResolutions
				.Select((r, i) => new { R = r, Index = i })
				.First(x => x.R.Equals(Resolution))
				.Index;
		}

		public void SetDispRes(int dispResIdx)
		{
			UpdateResolution(dispResIdx);
		}

		public void SetFG(int paletteIdx)
		{
			FG = paletteIdx % Display.ColorPalette.Length;
		}

		public void SetBG(int paletteIdx)
		{
			BG = paletteIdx % Display.ColorPalette.Length;
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
