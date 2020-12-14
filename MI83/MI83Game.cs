﻿namespace MI83
{
	using MI83.Core;
	using MI83.Infrastructure;
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Graphics;
	using System.Linq;
	using System.Threading.Tasks;

	public class MI83Game : Game
	{
		private GraphicsDeviceManager _graphics;
		private Computer _computer;
		private int _maxSupportedWidth;
		private int _maxSupportedHeight;
		private SpriteBatch _spriteBatch;
		private Color[] _renderData;
		private Texture2D _renderTarget;
		private ViewportAdapter _viewportAdapter;

		public MI83Game()
		{
			_graphics = new GraphicsDeviceManager(this);
			_computer = new Computer();
			_maxSupportedWidth = Display.SupportedResolutions.Max(r => r.Width);
			_maxSupportedHeight = Display.SupportedResolutions.Max(r => r.Height);

			Window.AllowUserResizing = true;
		}

		protected override void Initialize()
		{
			base.Initialize();

			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_renderData = new Color[_maxSupportedWidth * _maxSupportedHeight];

			_renderTarget = new Texture2D(
				GraphicsDevice,
				_maxSupportedWidth,
				_maxSupportedHeight);

			_viewportAdapter = new ViewportAdapter(
				Window,
				GraphicsDevice,
				_computer.Display.Resolution.Width,
				_computer.Display.Resolution.Height);

			_viewportAdapter.Reset();

			Window.TextInput += _computer.HomeScreen.Window_TextInput;

			// TODO: Delete test program here
			//Task.Factory.StartNew(() =>
			//{
			//	_computer.HomeScreen.ClrHome();
			//	_computer.HomeScreen.Disp("Hello World!");
			//	_computer.HomeScreen.Output(_computer.HomeScreen.Rows - 1, 0, "Press [Enter]\n");
			//	_computer.HomeScreen.Disp("Again\n");
			//	var result = _computer.HomeScreen.Input("Name: ");
			//	_computer.HomeScreen.Disp($"Hello {result}");
			//});
		}

		protected override void Update(GameTime gameTime) { }

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(new Color(210, 218, 196));

			_computer.RenderActiveDisplayMode();

			_computer.Display.Walk((pos, col) =>
			{
				_renderData[(pos.Y * _maxSupportedWidth) + pos.X] = col;
			});

			_renderTarget.SetData(_renderData);

			_spriteBatch.Begin(
				samplerState: SamplerState.PointClamp,
				transformMatrix: _viewportAdapter.GetScaleMatrix());

			_spriteBatch.Draw(
				_renderTarget,
				Vector2.Zero,
				new Rectangle(0, 0, _computer.Display.Resolution.Width, _computer.Display.Resolution.Height),
				Color.White);

			_spriteBatch.End();
		}
	}
}