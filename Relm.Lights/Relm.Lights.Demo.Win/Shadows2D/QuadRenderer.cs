using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Relm.Lights.Demo.Win.Shadows2D
{
    public class QuadRenderer
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly VertexPositionTexture[] _vertices;
        private readonly short[] _indexData = null;

        public QuadRenderer(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            _vertices = new[]
            {
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(1,1)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(0,1)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(0,0)),
                new VertexPositionTexture(
                    new Vector3(0,0,0),
                    new Vector2(1,0))
            };

            _indexData = new short[] { 0, 1, 2, 2, 3, 0 };
        }

        public void Render(Vector2 v1, Vector2 v2)
        {
            _vertices[0].Position.X = v2.X;
            _vertices[0].Position.Y = v1.Y;

            _vertices[1].Position.X = v1.X;
            _vertices[1].Position.Y = v1.Y;

            _vertices[2].Position.X = v1.X;
            _vertices[2].Position.Y = v2.Y;

            _vertices[3].Position.X = v2.X;
            _vertices[3].Position.Y = v2.Y;

            _graphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, _vertices, 0, 4, _indexData, 0, 2);
        }
    }
}