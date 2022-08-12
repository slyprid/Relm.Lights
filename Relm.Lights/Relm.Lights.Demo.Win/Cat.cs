using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Relm.Lights.Demo.Win
{
    public class Cat
    {
        private readonly Texture2D _texture;             
        private readonly Animation _walkingAnimation;     
        private readonly Vector2 _origin;                 

        public Vector2 Position { get; set; }

        public Cat(Texture2D texture, int frameCount, Vector2 origin)
        {
            _texture = texture;
            _origin = origin;


            _walkingAnimation = new Animation(texture.Width, texture.Height, frameCount, 0, 0)
            {
                FramesPerSecond = 14 * 2
            };

            Position = Vector2.Zero;
        }

        public void Update(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _walkingAnimation.Update(elapsed);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            spriteBatch.Draw(_texture, position, _walkingAnimation.CurrentFrame, color, 0.0f, _origin, 1.0f, SpriteEffects.None, 1.0f);
        }

    }
}