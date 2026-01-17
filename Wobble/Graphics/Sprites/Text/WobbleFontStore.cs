using System;
using System.Collections.Generic;
using FontStashSharp;
using Microsoft.Xna.Framework.Graphics;

namespace Wobble.Graphics.Sprites.Text
{
    public class WobbleFontStore
    {
        private float _fontSize;
        private readonly FontSystem _fontSystem;

        /// <summary>
        ///     All of the contained fonts at different sizes
        /// </summary>
        public DynamicSpriteFont Store { get; set; }

        /// <summary>
        ///     The size the font was initially created ad
        /// </summary>
        public int DefaultSize { get; }

        public float FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                Store = _fontSystem.GetFont(value);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="size"></param>
        /// <param name="font"></param>
        /// <param name="addedFonts"></param>
        public WobbleFontStore(int size, byte[] font, Dictionary<string, byte[]> addedFonts = null)
        {
            DefaultSize = size;

            var settings = new FontSystemSettings
            {
                // Renders font at higher resolution (e.g., 2f = 2x quality), then scales down for sharper text
                // Default: 1f
                FontResolutionFactor = 1f,

                // Width and height of anti-aliasing blur kernel (0 = none, higher = more blur on edges)
                // Default: 0
                KernelWidth = 0,
                KernelHeight = 0,

                // Controls how semi-transparent pixels are stored in the font texture atlas.
                // true (default) = premultiplied alpha, requires BlendState.AlphaBlend
                // false = straight alpha, requires BlendState.NonPremultiplied
                // Wobble uses BlendState.NonPremultiplied by default, so false removes dark halos.
                PremultiplyAlpha = false
            };

            _fontSystem = new FontSystem(settings);
            _fontSystem.AddFont(font);
            Store = _fontSystem.GetFont(size);

            if (addedFonts == null)
            {
                FontSize = size;
                return;
            }

            foreach (var f in addedFonts)
                AddFont(f.Key, f.Value);
            FontSize = size;
        }

        /// <summary>
        ///     Adds a font to the store from a byte[]
        /// </summary>
        /// <param name="name"></param>
        /// <param name="font"></param>
        public void AddFont(string name, byte[] font) => _fontSystem.AddFont(font);
    }
}