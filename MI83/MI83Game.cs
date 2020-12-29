namespace MI83
{
	using MI83.Core;
	using MI83.Infrastructure;
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Graphics;
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	public class MI83Game : Game
	{
		private GraphicsDeviceManager _graphics;
		private Computer _computer;
		private readonly Color _backgroundColor;
		private int _maxSupportedWidth;
		private int _maxSupportedHeight;
		private SpriteBatch _spriteBatch;
		private Color[] _renderData;
		private Texture2D _renderTarget;

		public MI83Game()
		{
			_graphics = new GraphicsDeviceManager(this);
			_computer = new Computer();
			_backgroundColor = new Color(0xBC, 0xC1, 0x9C);

			_maxSupportedWidth = Display.SupportedResolutions.Max(r => r.Width);
			_maxSupportedHeight = Display.SupportedResolutions.Max(r => r.Height);
		}

		protected override void Initialize()
		{
			Window.Title = "MI-83 Fantasy Game Console";

			_graphics.PreferredBackBufferWidth = 1024;
			_graphics.PreferredBackBufferHeight = 576;
			_graphics.ApplyChanges();

			base.Initialize();

			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_renderData = new Color[(_maxSupportedWidth * 3) * (_maxSupportedHeight * 3)];
			_renderTarget = new Texture2D(
				GraphicsDevice,
				_maxSupportedWidth * 3,
				_maxSupportedHeight * 3);

			Window.TextInput += _computer.Home.Window_TextInput;
			Window.KeyUp += _computer.Home.Window_KeyUp;

			Window.TextInput += _computer.System.Window_TextInput;
			Window.KeyUp += _computer.System.Window_KeyUp;

			_computer.Display.OnResolutionChanged += Display_OnResolutionChanged;
			_computer.Boot();
		}

		protected override void Update(GameTime gameTime)
		{
			if (_computer.Shutdown)
			{
				Exit();
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(_backgroundColor);
			_computer.RenderActiveDisplayMode();

			_computer.Display.Walk((pos, col) =>
			{
				for (var y = 0; y < 3; y++)
				{
					for (var x = 0; x < 3; x++)
					{
						var i =
							(((pos.Y * 3) + y) * (3 * _maxSupportedWidth)) +
							((pos.X * 3) + x);

						var shade = new[] { col.R, col.G, col.B }.Max();
						var b = shade > 0x60 ? 4 : 10;
						if (y < 2 && x < 2)
						{
							if ((y == 0 && x == 1) || (y == 1 && x == 0))
							{
								b = (b - (b / 4));
							}
							else if (y == 1 && x == 1)
							{
								b = (b - (b / 2));
							}
						}
						else
						{
							b = 0;
						}

						col = col == Color.Black ? _backgroundColor : col;
						col = new Color(col.R - b, col.G - b, col.B - b, col.A);

						_renderData[i] = col;
					}
				}
			});

			_renderTarget.SetData(_renderData);

			_spriteBatch.Begin(
				samplerState: SamplerState.AnisotropicClamp);

			_spriteBatch.Draw(
				_renderTarget,
				new Vector2((GraphicsDevice.Viewport.Width - (_maxSupportedWidth * 3)) / 2, 0),
				new Rectangle(0, 0,
					_computer.Display.Resolution.Width * 3,
					_computer.Display.Resolution.Height * 3),
				Color.White,
				0f,
				Vector2.Zero,
				(float)_maxSupportedWidth / _computer.Display.Resolution.Width,
				SpriteEffects.None,
				0f);

			_spriteBatch.End();
		}

		private void Display_OnResolutionChanged(object sender, ResolutionChangedEventArgs e)
		{
		}
	}
}