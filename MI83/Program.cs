﻿using MI83.Core;
using MI83.Core.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

using (var game = new MI83Engine())
{
    game.Run();
}

public class MI83Engine : Game
{
    private GraphicsDeviceManager _graphics;
    private Computer _computer;
    private readonly Color _backgroundColor;
    private int _maxSupportedWidth;
    private int _maxSupportedHeight;
    private SpriteBatch _spriteBatch;
    private Color[] _renderData;
    private Texture2D _renderTarget;
    private KeyboardState _prevKB;
    private KeyboardState _currKB;

    public MI83Engine()
    {
        _graphics = new GraphicsDeviceManager(this);
        _computer = new Computer();
        _backgroundColor = new Color(0xBC, 0xC1, 0x9C);
        _maxSupportedWidth = Display.MaxResolution.Width;
        _maxSupportedHeight = Display.MaxResolution.Height;
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

        Window.TextInput += _computer.InputBuffer.Window_TextInput;
        Window.KeyDown += _computer.InputBuffer.Window_KeyDown;
        Window.KeyUp += _computer.InputBuffer.Window_KeyUp;

        _computer.Boot();
    }

    protected override void Update(GameTime gameTime)
    {
        _prevKB = _currKB;
        _currKB = Keyboard.GetState();

        _computer.Tick();

        if (_computer.Shutdown)
        {
            Exit();
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(_backgroundColor);
        _computer.RenderActiveDisplayMode();

        _computer.DisplayBuffer.Walk((pos, col) =>
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
                _computer.DisplayBuffer.Resolution.Width * 3,
                _computer.DisplayBuffer.Resolution.Height * 3),
            Color.White,
            0f,
            Vector2.Zero,
            (float)_maxSupportedWidth / _computer.DisplayBuffer.Resolution.Width,
            SpriteEffects.None,
            0f);

        _spriteBatch.End();
    }
}