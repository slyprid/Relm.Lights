using Microsoft.Xna.Framework;

namespace Relm.Lights.Demo.Win
{
    public class Animation
    {
        private readonly Rectangle[] _frames;
        private float _frameLength = 1f / 5f;
        private float _timer = 0f;
        private int _currentFrame = 0;

        public int FramesPerSecond
        {
            get => (int)(1f / _frameLength);
            set => _frameLength = 1f / (float)value;
        }

        public Rectangle CurrentFrame => _frames[_currentFrame];

        public Animation(int width, int height, int numFrames, int xOffset, int yOffset)
        {
            _frames = new Rectangle[numFrames];
            var frameWidth = width / numFrames;
            for (var i = 0; i < numFrames; i++)
            {
                _frames[i] = new Rectangle(xOffset + (frameWidth * i), yOffset, frameWidth, height);
            }
        }

        public void Update(float elapsed)
        {
            _timer += elapsed;

            if (!(_timer >= _frameLength)) return;
            _timer = 0f;
            _currentFrame = (_currentFrame + 1) % _frames.Length;
        }

        public void Reset()
        {
            _currentFrame = 0;
            _timer = 0f;
        }
    }
}