using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Relm.Lights
{
    public class Hull
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }

        public Rectangle Bounds => new ((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
    }
}