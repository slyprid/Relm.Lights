using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Relm.Lights.Demo.Win.Shadows2D
{
    public class LightArea
    {
        private GraphicsDevice _graphicsDevice;
        
        public RenderTarget2D RenderTarget { get; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }

        public LightArea(GraphicsDevice graphicsDevice, ShadowMapSize size)
        {
            _graphicsDevice = graphicsDevice;
            var baseSize = 2 << (int)size;
            Size = new Vector2(baseSize);
            RenderTarget = new RenderTarget2D(_graphicsDevice, baseSize, baseSize);
        }

        public Vector2 ToRelativePosition(Vector2 worldPosition)
        {
            return worldPosition - (Position - Size * 0.5f);
        }

        public void BeginDrawingShadowCasters()
        {
            _graphicsDevice.SetRenderTarget(RenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
        }

        public void EndDrawingShadowCasters()
        {
            _graphicsDevice.SetRenderTarget(null);
        }
    }
}