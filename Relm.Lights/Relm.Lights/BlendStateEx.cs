using Microsoft.Xna.Framework.Graphics;

namespace Relm.Lights
{
    public static class BlendStateEx
    {
        public static BlendState Multiply { get; private set; }

        static BlendStateEx()
        {
            Multiply = new BlendState
            {
                AlphaSourceBlend = Blend.DestinationAlpha,
                AlphaDestinationBlend = Blend.Zero,
                AlphaBlendFunction = BlendFunction.Add,
                ColorSourceBlend = Blend.DestinationColor,
                ColorDestinationBlend = Blend.Zero,
                ColorBlendFunction = BlendFunction.Add
            };
        }
    }
}