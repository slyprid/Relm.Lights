using System;
using Microsoft.Xna.Framework;

namespace Relm.Lights
{
    public class Light
    {
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public float Scale { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public Rectangle Bounds => new((int)Position.X, (int)Position.Y, (int)(Width * Scale), (int)(Height * Scale));

        public Light()
        {
            Name = Guid.NewGuid().ToString();
            Position = Vector2.Zero;
            Color = Color.White;
            Scale = 1;
            Width = 64;
            Height = 64;
        }
    }
}