using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Wobble.Assets;
using Wobble.Graphics.Animations;

namespace Wobble.Graphics.Sprites
{
    /// <summary>
    ///     Defines the slice margins for 9-slice scaling.
    /// </summary>
    public struct SliceMargins
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;

        /// <summary>
        ///     Creates margins with the same value for all sides.
        /// </summary>
        public SliceMargins(int all)
        {
            Left = Right = Top = Bottom = all;
        }

        /// <summary>
        ///     Creates margins with horizontal and vertical values.
        /// </summary>
        public SliceMargins(int horizontal, int vertical)
        {
            Left = Right = horizontal;
            Top = Bottom = vertical;
        }

        /// <summary>
        ///     Creates margins with individual values for each side.
        /// </summary>
        public SliceMargins(int left, int right, int top, int bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }
    }

    /// <summary>
    ///     A sprite that uses 9-slice scaling (also known as 9-patch).
    ///     Corners maintain their original size, edges stretch in one direction,
    ///     and the center stretches in both directions.
    /// </summary>
    public class NineSliceSprite : Drawable
    {
        private Texture2D _image;
        private SliceMargins _margins;
        private Color _tint = Color.White;
        private float _alpha = 1f;
        private Color _color = Color.White;

        // Cached source rectangles for each of the 9 slices
        private Rectangle _srcTopLeft, _srcTopCenter, _srcTopRight;
        private Rectangle _srcMiddleLeft, _srcMiddleCenter, _srcMiddleRight;
        private Rectangle _srcBottomLeft, _srcBottomCenter, _srcBottomRight;

        /// <summary>
        ///     The image texture for the 9-slice sprite.
        /// </summary>
        public Texture2D Image
        {
            get => _image;
            set
            {
                if (value == null)
                    return;

                _image = value;
                RecalculateSourceRectangles();
                RecalculateRectangles();
            }
        }

        /// <summary>
        ///     The slice margins defining how the texture is divided.
        /// </summary>
        public SliceMargins Margins
        {
            get => _margins;
            set
            {
                _margins = value;
                RecalculateSourceRectangles();
            }
        }

        /// <summary>
        ///     The tint color of the sprite.
        /// </summary>
        public Color Tint
        {
            get => _tint;
            set
            {
                _tint = value;
                _color = _tint * _alpha;
            }
        }

        /// <summary>
        ///     The transparency of the sprite (0.0 = invisible, 1.0 = fully visible).
        /// </summary>
        public float Alpha
        {
            get => _alpha;
            set
            {
                _alpha = value;
                _color = _tint * _alpha;

                if (!SetChildrenAlpha)
                    return;

                foreach (var child in Children)
                {
                    if (child is Sprite sprite)
                        sprite.Alpha = value;
                    else if (child is NineSliceSprite nineSlice)
                        nineSlice.Alpha = value;
                }
            }
        }

        /// <summary>
        ///     If true, setting Alpha will also set the alpha of all children.
        /// </summary>
        public bool SetChildrenAlpha { get; set; }

        /// <summary>
        ///     Creates a new 9-slice sprite with the specified texture and margins.
        /// </summary>
        public NineSliceSprite(Texture2D texture, SliceMargins margins)
        {
            _margins = margins;
            Image = texture;
        }

        /// <summary>
        ///     Creates a new 9-slice sprite with default configuration.
        /// </summary>
        public NineSliceSprite()
        {
        }

        /// <inheritdoc />
        public override void Draw(GameTime gameTime)
        {
            if (Image == null)
                Image = WobbleAssets.WhiteBox;

            if (SpriteBatchOptions != null)
            {
                _ = GameBase.Game.TryEndBatch();
                GameBase.DefaultSpriteBatchInUse = false;
                SpriteBatchOptions.Begin();
                DrawToSpriteBatch();
            }
            else if (!GameBase.DefaultSpriteBatchInUse && !UsePreviousSpriteBatchOptions)
            {
                _ = GameBase.Game.TryEndBatch();
                GameBase.DefaultSpriteBatchOptions.Begin();
                GameBase.DefaultSpriteBatchInUse = true;
                DrawToSpriteBatch();
            }
            else
            {
                try
                {
                    DrawToSpriteBatch();
                }
                catch (Exception)
                {
                    GameBase.DefaultSpriteBatchOptions.Begin();
                    GameBase.DefaultSpriteBatchInUse = true;
                    DrawToSpriteBatch();
                }
            }

            base.Draw(gameTime);
        }

        /// <inheritdoc />
        public override void DrawToSpriteBatch()
        {
            if (!Visible)
                return;

            var spriteBatch = GameBase.Game.SpriteBatch;
            var screenPos = ScreenRectangle.Position;
            var screenSize = ScreenRectangle.Size;

            // Calculate destination sizes
            var leftW = Math.Min(_margins.Left, (int)screenSize.Width);
            var rightW = Math.Min(_margins.Right, (int)screenSize.Width - leftW);
            var centerW = Math.Max(0, (int)screenSize.Width - leftW - rightW);

            var topH = Math.Min(_margins.Top, (int)screenSize.Height);
            var bottomH = Math.Min(_margins.Bottom, (int)screenSize.Height - topH);
            var centerH = Math.Max(0, (int)screenSize.Height - topH - bottomH);

            // Calculate destination positions
            int x0 = (int)screenPos.X, x1 = x0 + leftW, x2 = x1 + centerW;
            int y0 = (int)screenPos.Y, y1 = y0 + topH, y2 = y1 + centerH;

            // Draw all 9 slices
            if (topH > 0)
            {
                if (leftW > 0) spriteBatch.Draw(Image, new Rectangle(x0, y0, leftW, topH), _srcTopLeft, _color);
                if (centerW > 0) spriteBatch.Draw(Image, new Rectangle(x1, y0, centerW, topH), _srcTopCenter, _color);
                if (rightW > 0) spriteBatch.Draw(Image, new Rectangle(x2, y0, rightW, topH), _srcTopRight, _color);
            }

            if (centerH > 0)
            {
                if (leftW > 0) spriteBatch.Draw(Image, new Rectangle(x0, y1, leftW, centerH), _srcMiddleLeft, _color);
                if (centerW > 0) spriteBatch.Draw(Image, new Rectangle(x1, y1, centerW, centerH), _srcMiddleCenter, _color);
                if (rightW > 0) spriteBatch.Draw(Image, new Rectangle(x2, y1, rightW, centerH), _srcMiddleRight, _color);
            }

            if (bottomH > 0)
            {
                if (leftW > 0) spriteBatch.Draw(Image, new Rectangle(x0, y2, leftW, bottomH), _srcBottomLeft, _color);
                if (centerW > 0) spriteBatch.Draw(Image, new Rectangle(x1, y2, centerW, bottomH), _srcBottomCenter, _color);
                if (rightW > 0) spriteBatch.Draw(Image, new Rectangle(x2, y2, rightW, bottomH), _srcBottomRight, _color);
            }
        }

        private void RecalculateSourceRectangles()
        {
            if (Image == null)
                return;

            int texW = Image.Width, texH = Image.Height;

            int left = Math.Min(_margins.Left, texW);
            int right = Math.Min(_margins.Right, texW - left);
            int centerW = Math.Max(0, texW - left - right);

            int top = Math.Min(_margins.Top, texH);
            int bottom = Math.Min(_margins.Bottom, texH - top);
            int centerH = Math.Max(0, texH - top - bottom);

            _srcTopLeft = new Rectangle(0, 0, left, top);
            _srcTopCenter = new Rectangle(left, 0, centerW, top);
            _srcTopRight = new Rectangle(left + centerW, 0, right, top);

            _srcMiddleLeft = new Rectangle(0, top, left, centerH);
            _srcMiddleCenter = new Rectangle(left, top, centerW, centerH);
            _srcMiddleRight = new Rectangle(left + centerW, top, right, centerH);

            _srcBottomLeft = new Rectangle(0, top + centerH, left, bottom);
            _srcBottomCenter = new Rectangle(left, top + centerH, centerW, bottom);
            _srcBottomRight = new Rectangle(left + centerW, top + centerH, right, bottom);
        }

        /// <summary>
        ///     Fades the sprite to a given color.
        /// </summary>
        public virtual void FadeToColor(Color color, double dt, float scale)
        {
            var r = MathHelper.Lerp(Tint.R, color.R, (float)Math.Min(dt / scale, 1));
            var g = MathHelper.Lerp(Tint.G, color.G, (float)Math.Min(dt / scale, 1));
            var b = MathHelper.Lerp(Tint.B, color.B, (float)Math.Min(dt / scale, 1));

            Tint = new Color((int)r, (int)g, (int)b);
        }

        /// <summary>
        ///     Fades to a tint using easing.
        /// </summary>
        public NineSliceSprite FadeToColor(Color color, Easing easingType, int time)
        {
            lock (Animations)
                Animations.Add(new Animation(easingType, Tint, color, time));

            return this;
        }

        /// <summary>
        ///     Fades to a target alpha value.
        /// </summary>
        public NineSliceSprite FadeTo(float alpha, Easing easingType, int time)
        {
            lock (Animations)
                Animations.Add(new Animation(AnimationProperty.Alpha, easingType, Alpha, alpha, time));

            return this;
        }
    }
}
