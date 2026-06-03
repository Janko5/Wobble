using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Wobble.Graphics.Sprites
{
    public class SpriteAlphaMaskBlend : Sprite
    {
        private RenderTarget2D RenderTarget { get; set; }

        private readonly BlendState blend = new BlendState
        {
            AlphaSourceBlend = Blend.DestinationAlpha,
            AlphaBlendFunction = BlendFunction.Subtract,
            AlphaDestinationBlend = Blend.InverseDestinationAlpha
        };

        public Texture2D PerformBlend(Texture2D srcTexture, Texture2D srcMask)
        {
            if (srcTexture == null || srcTexture.IsDisposed || srcMask == null || srcMask.IsDisposed)
                return srcTexture;

            var previousRenderTarget = RenderTarget;
            var nextRenderTarget = new RenderTarget2D(GameBase.Game.GraphicsDevice, srcTexture.Width, srcTexture.Height, false,
                GameBase.Game.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None);
            var spriteBatchStarted = false;
            var completed = false;

            RenderTarget = nextRenderTarget;

            try
            {
                GameBase.Game.GraphicsDevice.SetRenderTarget(RenderTarget);

                // Attempt to end the spritebatch
                _ = GameBase.Game.TryEndBatch();

                GameBase.Game.SpriteBatch.Begin(blendState: blend);
                spriteBatchStarted = true;

                GameBase.Game.SpriteBatch.Draw(srcMask, srcTexture.Bounds, Color.White);
                GameBase.Game.SpriteBatch.Draw(srcTexture, srcTexture.Bounds, Color.White);
                GameBase.Game.SpriteBatch.End();
                spriteBatchStarted = false;
                completed = true;
            }
            finally
            {
                try
                {
                    if (spriteBatchStarted)
                        _ = GameBase.Game.TryEndBatch();
                }
                finally
                {
                    GameBase.Game.GraphicsDevice.SetRenderTarget(null);

                    if (!completed)
                    {
                        RenderTarget?.Dispose();
                        RenderTarget = previousRenderTarget;
                    }
                }
            }

            GameBase.Game.GraphicsDevice.Clear(Color.Black);

            if (previousRenderTarget != null && !previousRenderTarget.IsDisposed && !ReferenceEquals(previousRenderTarget, srcTexture))
                previousRenderTarget.Dispose();

            return RenderTarget;
        }

        public override void Destroy()
        {
            if (RenderTarget != null)
            {
                RenderTarget.Dispose();
                RenderTarget = null;
            }

            base.Destroy();
        }
    }
}
