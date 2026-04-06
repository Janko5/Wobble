using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MonoGame.Extended.BitmapFonts;
using Wobble.Graphics.Sprites.Text;
using Wobble.Logging;

namespace Wobble.Managers
{
    public static class FontManager
    {
        /// <summary>
        /// </summary>
        public static ConcurrentDictionary<string, BitmapFont> BitmapFonts { get; } = new ConcurrentDictionary<string, BitmapFont>();

        /// <summary>
        /// </summary>
        public static ConcurrentDictionary<string, WobbleFontStore> WobbleFonts { get; } = new ConcurrentDictionary<string, WobbleFontStore>();

        /// <summary>
        ///     Loads and caches a bitmap font
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static BitmapFont LoadBitmapFont(string name)
        {
            if (BitmapFonts.TryGetValue(name, out var font))
                return font;

            var loadedFont = GameBase.Game.Content.Load<BitmapFont>(name);
            return BitmapFonts.GetOrAdd(name, loadedFont);
        }

        /// <summary>
        ///     Loads and caches a WobbleFont
        /// </summary>
        /// <param name="name"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static void CacheWobbleFont(string name, WobbleFontStore font)
        {
            if (!WobbleFonts.TryAdd(name, font))
                throw new ArgumentException("A font with this name already exists!");

            Logger.Debug($"Loaded font: {name}", LogType.Runtime);
        }

        /// <summary>
        ///     Retrieves a WobbleFont if cached
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static WobbleFontStore GetWobbleFont(string name) => WobbleFonts[name];
    }
}