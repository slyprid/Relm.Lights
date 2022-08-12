using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Relm.Lights
{
    public class LightRenderer
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;
        private readonly Texture2D _lightTexture;
        private readonly List<string> _lightLookup;
        private readonly RenderTarget2D _lightMap;

        public List<Light> Lights { get; set; }

        public Light this[string name] => Lights[_lightLookup.IndexOf(name)];
        public Light this[int index] => Lights[index];

        public LightRenderer(GraphicsDevice graphicsDevice, int screenWidth, int screenHeight, Texture2D lightTexture)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _lightTexture = lightTexture;
            _lightLookup = new List<string>();
            Lights = new List<Light>();

            _lightMap = new RenderTarget2D(_graphicsDevice, screenWidth, screenHeight);
        }

        #region Update / Draw
        
        public void Update(GameTime gameTime)
        {

        }

        public void RenderLights(GameTime gameTime)
        {
            _graphicsDevice.SetRenderTarget(_lightMap);

            _graphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            
            foreach (var light in Lights)
            {
                _spriteBatch.Draw(_lightTexture, light.Bounds, light.Color);
            }

            _spriteBatch.End();
        }

        public void Blend(Texture2D mainTarget)
        {
            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.Clear(Color.White);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendStateEx.Multiply);
            _spriteBatch.Draw(mainTarget, Vector2.Zero, Color.White);
            _spriteBatch.Draw(_lightMap, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }

        #endregion

        #region Light Methods

        public Light Add(Light light)
        {
            _lightLookup.Add(light.Name);
            Lights.Add(light);
            return light;
        }

        public Light Add(string name, Light light)
        {
            light.Name = name;
            _lightLookup.Add(light.Name);
            Lights.Add(light);
            return light;
        }

        #endregion
    }
}