using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Relm.Lights.Demo.Win
{
    public class DemoGame 
        : Game
    {
        #region Variables 

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private int _screenWidth = 1280;
        private int _screenHeight = 720;
        private int _tileWidth = 48;
        private int _tileHeight = 48;
        private int _scale = 1;
        private float _lightScale = 1f;
        private readonly Vector2 _lightSize = new (64f);

        private Texture2D _dungeonTexture;
        private Texture2D _exteriorTexture;
        private Texture2D _interiorTexture;
        private Texture2D _shipTexture;
        private Texture2D _worldTexture;
        private Texture2D _lightTexture;
        private Texture2D _whitePixel;

        private RenderTarget2D _mainTarget;
        
        private Vector2 _mousePosition;

        private Light _playerLight;

        private LightRenderer _lightRenderer;

        private List<Hull> _hulls;

        private readonly Random _rnd;

        #endregion

        public DemoGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = _screenWidth;
            _graphics.PreferredBackBufferHeight = _screenHeight;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _rnd = new Random();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _dungeonTexture = Content.Load<Texture2D>("gfx/dungeon");
            _exteriorTexture = Content.Load<Texture2D>("gfx/exterior");
            _interiorTexture = Content.Load<Texture2D>("gfx/interior");
            _shipTexture = Content.Load<Texture2D>("gfx/ship");
            _worldTexture = Content.Load<Texture2D>("gfx/world");
            _lightTexture = Content.Load<Texture2D>("gfx/light");

            _whitePixel = new Texture2D(GraphicsDevice, 1, 1);
            _whitePixel.SetData(new [] { Color.White });

            _lightRenderer = new LightRenderer(GraphicsDevice, _screenWidth, _screenHeight, _lightTexture);
            _playerLight = _lightRenderer.Add("Player", new Light());

            _mainTarget = new RenderTarget2D(GraphicsDevice, _screenWidth, _screenHeight);

            _hulls = new List<Hull>();
        }

        protected override void Update(GameTime gameTime)
        {
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                _lightRenderer.Add(new Light
                {
                    Position = _mousePosition - ((_lightSize * _lightScale) / 2),
                    Width = _lightSize.X,
                    Height = _lightSize.Y,
                    Scale = _lightScale,
                    Color = Color.FromNonPremultiplied(_rnd.Next(0, 255), _rnd.Next(0, 255), _rnd.Next(0, 255), 255)
                });
            }

            if (_currentMouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released)
            {
                var position = new Vector2((int)(_mousePosition.X / _tileWidth) * _tileWidth, (int)(_mousePosition.Y / _tileHeight) * _tileHeight);
                _hulls.Add(new Hull
                {
                    Position = position,
                    Size = new Vector2(_tileWidth, _tileHeight)
                });
            }

            if (_currentMouseState.ScrollWheelValue > _previousMouseState.ScrollWheelValue)
            {
                _lightScale += 0.1f;
            }
            if (_currentMouseState.ScrollWheelValue < _previousMouseState.ScrollWheelValue)
            {
                _lightScale -= 0.1f;
            }

            if (_lightScale <= 1f) _lightScale = 1f;

            _mousePosition = new Vector2(_currentMouseState.X, _currentMouseState.Y);

            _playerLight.Position = _mousePosition - ((_lightSize * _lightScale) / 2);
            _playerLight.Width = _lightSize.X;
            _playerLight.Height = _lightSize.Y;
            _playerLight.Scale = _lightScale;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _lightRenderer.RenderLights(gameTime);
            RenderTiles();
            _lightRenderer.Blend(_mainTarget);

            base.Draw(gameTime);
        }
        
        private void RenderTiles()
        {
            GraphicsDevice.SetRenderTarget(_mainTarget);

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            var srcRect = new Rectangle(480, 480, _tileWidth, _tileHeight);
            for (var y = 0; y < _screenHeight; y += _tileHeight * _scale)
            {
                for (var x = 0; x < _screenWidth; x += _tileWidth * _scale)
                {
                    var destRect = new Rectangle(x, y, _tileWidth * _scale, _tileHeight * _scale);
                    _spriteBatch.Draw(_exteriorTexture, destRect, srcRect, Color.White);
                }
            }

            srcRect = new Rectangle(1296, 336, _tileWidth, _tileHeight);
            foreach (var hull in _hulls)
            {
                _spriteBatch.Draw(_exteriorTexture, hull.Bounds, srcRect, Color.White);
            }

            _spriteBatch.End();
        }
    }
}