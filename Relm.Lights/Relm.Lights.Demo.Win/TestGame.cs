using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Relm.Lights.Demo.Win.Shadows2D;

namespace Relm.Lights.Demo.Win
{
    public class TestGame
        : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        private int _screenWidth = 800;
        private int _screenHeight = 600;

        private Vector2 _lightPosition;
        private Vector2 _lightPosition2;
        private Texture2D _testTexture;
        private Texture2D _dot;
        private Texture2D _tileTexture;

        private QuadRenderer _quadRenderer;
        private ShadowMapResolver _shadowMapResolver;
        private LightArea _lightArea1;
        private LightArea _lightArea2;
        private RenderTarget2D _screenShadows;

        private Cat _cat;

        public TestGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = _screenWidth;
            _graphics.PreferredBackBufferHeight = _screenHeight;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _quadRenderer = new QuadRenderer(GraphicsDevice);

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _shadowMapResolver = new ShadowMapResolver(GraphicsDevice, _quadRenderer, ShadowMapSize.Size256);
            _shadowMapResolver.LoadContent(Content);


            _lightArea1 = new LightArea(GraphicsDevice, ShadowMapSize.Size512);
            _lightArea2 = new LightArea(GraphicsDevice, ShadowMapSize.Size512);


            _testTexture = Content.Load<Texture2D>("cat4");
            _dot = Content.Load<Texture2D>("dot");
            _tileTexture = Content.Load<Texture2D>("tile");
            _lightPosition = new Vector2(276, 276);
            _lightPosition2 = new Vector2(560, 154);
            _screenShadows = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            var catTexture = Content.Load<Texture2D>("catWalk");

            _cat = new Cat(catTexture, 14, new Vector2(64, 128))
            {
                Position = new Vector2(600, 300)
            };

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var movement = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
            movement.Y *= -1.0f;

            var movement2 = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;
            movement2.Y *= -1.0f;

            var ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Left)) movement.X = -1.0f;
            if (ks.IsKeyDown(Keys.Right)) movement.X = 1.0f;
            if (ks.IsKeyDown(Keys.Up)) movement.Y = -1.0f;
            if (ks.IsKeyDown(Keys.Down)) movement.Y = 1.0f;

            _lightPosition += movement * 100.0f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _lightPosition2 += movement2 * 100.0f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            _cat.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _lightArea1.Position = _lightPosition;
            _lightArea1.BeginDrawingShadowCasters();
            DrawCasters(_lightArea1);
            _lightArea1.EndDrawingShadowCasters();
            _shadowMapResolver.ResolveShadows(_lightArea1.RenderTarget, _lightArea1.RenderTarget, _lightPosition);

            _lightArea2.Position = _lightPosition2;
            _lightArea2.BeginDrawingShadowCasters();
            DrawCasters(_lightArea2);
            _lightArea2.EndDrawingShadowCasters();
            _shadowMapResolver.ResolveShadows(_lightArea2.RenderTarget, _lightArea2.RenderTarget, _lightPosition2);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_lightArea1.RenderTarget, _lightArea1.Position - _lightArea1.Size * 0.5f, Color.Red);
            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(_screenShadows);
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            _spriteBatch.Draw(_lightArea1.RenderTarget, _lightArea1.Position - _lightArea1.Size * 0.5f, Color.Red);
            _spriteBatch.Draw(_lightArea2.RenderTarget, _lightArea2.Position - _lightArea2.Size * 0.5f, Color.Blue);
            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);


            GraphicsDevice.Clear(Color.Black);

            DrawGround();

            var blendState = new BlendState
            {
                ColorSourceBlend = Blend.DestinationColor,
                ColorDestinationBlend = Blend.SourceColor
            };

            _spriteBatch.Begin(SpriteSortMode.Immediate, blendState);
            _spriteBatch.Draw(_screenShadows, Vector2.Zero, Color.White);
            _spriteBatch.End();

            DrawScene();

            base.Draw(gameTime);
        }

        private void DrawCasters(LightArea lightArea)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_testTexture, lightArea.ToRelativePosition(Vector2.Zero), Color.Black);
            _cat.Draw(_spriteBatch, lightArea.ToRelativePosition(_cat.Position), Color.Black);
            _spriteBatch.End();
        }

        private void DrawScene()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            _spriteBatch.Draw(_testTexture, new Rectangle(0, 0, 512, 512), new Color(new Vector4(1, 1, 1, 1.0f)));
            _cat.Draw(_spriteBatch, _cat.Position, Color.White);
            DrawDot(_lightPosition);
            DrawDot(_lightPosition2);
            _spriteBatch.End();
        }
        private void DrawGround()
        {
            var source = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);
            _spriteBatch.Draw(_tileTexture, Vector2.Zero, source, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            _spriteBatch.End();

        }

        private void DrawDot(Vector2 pos)
        {
            pos -= new Vector2(8, 8);
            _spriteBatch.Draw(_dot, pos, Color.White);
        }
    }
}