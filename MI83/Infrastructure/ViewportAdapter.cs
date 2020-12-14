//  Derived heavily from the BoxingViewportAdapter from the MonoGame.Extended 
//  project https://github.com/craftworkgames/MonoGame.Extended, under the 
//  proceeding license:
//
//  The MIT License(MIT)
//  
//  Copyright(c) 2015 Dylan Wilson
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.

namespace MI83.Infrastructure
{
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Graphics;
	using System;

	enum BoxingMode
	{
		None,
		Letterbox,
		Pillarbox
	}

	class ViewportAdapter
	{
		private readonly GameWindow _window;

		private readonly GraphicsDeviceManager _graphicsDeviceManager;

		public GraphicsDevice GraphicsDevice { get; }

		public int VirtualWidth { get; set; }

		public int VirtualHeight { get; set; }

		public Viewport Viewport => GraphicsDevice.Viewport;

		public int ViewportWidth => GraphicsDevice.Viewport.Width;

		public int ViewportHeight => GraphicsDevice.Viewport.Height;

		public Rectangle BoundingRectangle => new Rectangle(0, 0, VirtualWidth, VirtualHeight);

		public Point Center => BoundingRectangle.Center;

		public BoxingMode BoxingMode { get; private set; }

		public ViewportAdapter(GameWindow window, GraphicsDevice graphicsDevice,
			int virtualWidth, int virtualHeight)
		{
			_window = window;
			_window.ClientSizeChanged += OnClientSizeChanged;
			GraphicsDevice = graphicsDevice;
			VirtualWidth = virtualWidth;
			VirtualHeight = virtualHeight;
		}

		public ViewportAdapter(GameWindow window, GraphicsDeviceManager graphicsDeviceManager,
			int virtualWidth, int virtualHeight)
			: this(window, graphicsDeviceManager.GraphicsDevice, virtualWidth, virtualHeight)
		{
			this._graphicsDeviceManager = graphicsDeviceManager;
		}

		public Matrix GetScaleMatrix()
		{
			var scaleX = (float)ViewportWidth / VirtualWidth;
			var scaleY = (float)ViewportHeight / VirtualHeight;
			return Matrix.CreateScale(scaleX, scaleY, 1.0f);
		}

		public Point PointToScreen(Point point)
		{
			return PointToScreen(point.X, point.Y);
		}

		public Point PointToScreen(int x, int y)
		{
			var viewport = GraphicsDevice.Viewport;
			var vx = x - viewport.X;
			var vy = y - viewport.Y;
			var scaleMatrix = GetScaleMatrix();
			var invertedMatrix = Matrix.Invert(scaleMatrix);
			return Vector2.Transform(new Vector2(vx, vy), invertedMatrix).ToPoint();
		}

		public void Reset()
		{
			OnClientSizeChanged(this, EventArgs.Empty);
		}

		private void OnClientSizeChanged(object sender, EventArgs eventArgs)
		{
			var viewport = GraphicsDevice.Viewport;

			var worldScaleX = (float)viewport.Width / VirtualWidth;
			var worldScaleY = (float)viewport.Height / VirtualHeight;

			var safeScaleX = (float)viewport.Width / (VirtualWidth);
			var safeScaleY = (float)viewport.Height / (VirtualHeight);

			var worldScale = MathHelper.Max(worldScaleX, worldScaleY);
			var safeScale = MathHelper.Min(safeScaleX, safeScaleY);
			var scale = MathHelper.Min(worldScale, safeScale);

			var width = (int)(scale * VirtualWidth + 0.5f);
			var height = (int)(scale * VirtualHeight + 0.5f);

			if ((height >= viewport.Height) && (width < viewport.Width))
				BoxingMode = BoxingMode.Pillarbox;
			else if ((width >= viewport.Height) && (height < viewport.Height))
				BoxingMode = BoxingMode.Letterbox;
			else
				BoxingMode = BoxingMode.None;

			var x = viewport.Width / 2 - width / 2;
			var y = viewport.Height / 2 - height / 2;
			GraphicsDevice.Viewport = new Viewport(x, y, width, height);

			// Needed for a DirectX bug in MonoGame 3.4. Hopefully it will be fixed in future versions
			// see http://gamedev.stackexchange.com/questions/68914/issue-with-monogame-resizing
			if ((_graphicsDeviceManager != null) &&
				((_graphicsDeviceManager.PreferredBackBufferWidth != _window.ClientBounds.Width) ||
				 (_graphicsDeviceManager.PreferredBackBufferHeight != _window.ClientBounds.Height)))
			{
				_graphicsDeviceManager.PreferredBackBufferWidth = _window.ClientBounds.Width;
				_graphicsDeviceManager.PreferredBackBufferHeight = _window.ClientBounds.Height;
				_graphicsDeviceManager.ApplyChanges();
			}
		}
	}
}