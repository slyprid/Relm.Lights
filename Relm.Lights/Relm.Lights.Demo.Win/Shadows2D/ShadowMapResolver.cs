using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1;
using Effect = Microsoft.Xna.Framework.Graphics.Effect;

namespace Relm.Lights.Demo.Win.Shadows2D
{
    public class ShadowMapResolver
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly int _reductionChainCount;
        private readonly int _baseSize;
        private readonly QuadRenderer _quadRenderer;
        
        private Effect _resolveShadowsEffect;
        private Effect _reductionEffect;
        
        private RenderTarget2D _distortTarget;
        private RenderTarget2D _shadowMap;
        private RenderTarget2D _shadowsTarget;
        private RenderTarget2D _processedShadowsTarget;
        private RenderTarget2D _distancesTarget;
        private RenderTarget2D[] _reductionTargets;

        public ShadowMapResolver(GraphicsDevice graphicsDevice, QuadRenderer quadRenderer, ShadowMapSize maxShadowMapSize)
        {
            _graphicsDevice = graphicsDevice;
            _quadRenderer = quadRenderer;

            _reductionChainCount = (int)maxShadowMapSize;
            _baseSize = 2 << _reductionChainCount;
        }

        public void LoadContent(ContentManager content)
        {
            _reductionEffect = content.Load<Effect>("reductionEffect");
            _resolveShadowsEffect = content.Load<Effect>("resolveShadowsEffect");

            _distortTarget = new RenderTarget2D(_graphicsDevice, _baseSize, _baseSize, false, SurfaceFormat.HalfVector2, DepthFormat.None);
            _distancesTarget = new RenderTarget2D(_graphicsDevice, _baseSize, _baseSize, false, SurfaceFormat.HalfVector2, DepthFormat.None);
            _shadowMap = new RenderTarget2D(_graphicsDevice, 2, _baseSize, false, SurfaceFormat.HalfVector2, DepthFormat.None);
            _reductionTargets = new RenderTarget2D[_reductionChainCount];
            for (var i = 0; i < _reductionChainCount; i++)
            {
                _reductionTargets[i] = new RenderTarget2D(_graphicsDevice, 2 << i, _baseSize, false, SurfaceFormat.HalfVector2, DepthFormat.None);
            }


            _shadowsTarget = new RenderTarget2D(_graphicsDevice, _baseSize, _baseSize);
            _processedShadowsTarget = new RenderTarget2D(_graphicsDevice, _baseSize, _baseSize);
        }

        public void ResolveShadows(Texture2D shadowCastersTexture, RenderTarget2D result, Vector2 lightPosition)
        {
            _graphicsDevice.BlendState = BlendState.Opaque;

            ExecuteTechnique(shadowCastersTexture, _distancesTarget, "ComputeDistances");
            ExecuteTechnique(_distancesTarget, _distortTarget, "Distort");
            ApplyHorizontalReduction(_distortTarget, _shadowMap);
            ExecuteTechnique(null, _shadowsTarget, "DrawShadows", _shadowMap);
            ExecuteTechnique(_shadowsTarget, _processedShadowsTarget, "BlurHorizontally");
            ExecuteTechnique(_processedShadowsTarget, result, "BlurVerticallyAndAttenuate");
        }

        private void ExecuteTechnique(Texture2D source, RenderTarget2D destination, string techniqueName, Texture2D shadowMap = null)
        {
            var renderTargetSize = new Vector2((float)_baseSize, (float)_baseSize);
            _graphicsDevice.SetRenderTarget(destination);
            _graphicsDevice.Clear(Color.White);
            _resolveShadowsEffect.Parameters["renderTargetSize"].SetValue(renderTargetSize);

            if (source != null) _resolveShadowsEffect.Parameters["InputTexture"].SetValue(source);
            if (shadowMap != null) _resolveShadowsEffect.Parameters["ShadowMapTexture"].SetValue(shadowMap);

            _resolveShadowsEffect.CurrentTechnique = _resolveShadowsEffect.Techniques[techniqueName];

            foreach (var pass in _resolveShadowsEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _quadRenderer.Render(Vector2.One * -1, Vector2.One);
            }
            _graphicsDevice.SetRenderTarget(null);
        }

        private void ApplyHorizontalReduction(RenderTarget2D source, RenderTarget2D destination)
        {
            var step = _reductionChainCount - 1;
            var s = source;
            var d = _reductionTargets[step];
            _reductionEffect.CurrentTechnique = _reductionEffect.Techniques["HorizontalReduction"];

            while (step >= 0)
            {
                d = _reductionTargets[step];

                _graphicsDevice.SetRenderTarget(d);
                _graphicsDevice.Clear(Color.White);

                _reductionEffect.Parameters["SourceTexture"].SetValue(s);
                var textureDim = new Vector2(1.0f / (float)s.Width, 1.0f / (float)s.Height);
                _reductionEffect.Parameters["TextureDimensions"].SetValue(textureDim);

                foreach (var pass in _reductionEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _quadRenderer.Render(Vector2.One * -1, new Vector2(1, 1));
                }

                _graphicsDevice.SetRenderTarget(null);

                s = d;
                step--;
            }

            _graphicsDevice.SetRenderTarget(destination);
            _reductionEffect.CurrentTechnique = _reductionEffect.Techniques["Copy"];
            _reductionEffect.Parameters["SourceTexture"].SetValue(d);

            foreach (var pass in _reductionEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _quadRenderer.Render(Vector2.One * -1, new Vector2(1, 1));
            }

            _reductionEffect.Parameters["SourceTexture"].SetValue(_reductionTargets[_reductionChainCount - 1]);
            _graphicsDevice.SetRenderTarget(null);
        }
    }
}