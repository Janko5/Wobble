using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using Microsoft.Xna.Framework.Graphics;
using Wobble.Assets;

namespace Wobble.Managers
{
    public static class TextureManager
    {
        /// <summary>
        /// </summary>
        public static ConcurrentDictionary<string, Texture2D> Textures { get; } = new ConcurrentDictionary<string, Texture2D>();

        /// <summary>
        /// </summary>
        public static ConcurrentDictionary<string, List<Texture2D>> TextureAtlases { get; } = new ConcurrentDictionary<string, List<Texture2D>>();

        /// <summary>
        ///    Loads a texture and caches it for later use
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Texture2D Load(string name)
        {
            if (Textures.TryGetValue(name, out var tex))
                return tex;

            var loadedTex = AssetLoader.LoadTexture2D(GameBase.Game.Resources.Get(name));
            var finalTex = Textures.GetOrAdd(name, loadedTex);

            if (finalTex != loadedTex)
                loadedTex.Dispose();

            return finalTex;
        }

        /// <summary>
        ///     Loads a texture atlas and caches it for later use
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static List<Texture2D> LoadAtlas(string name, int rows, int columns)
        {
            if (TextureAtlases.TryGetValue(name, out var textures))
                return textures;

            var tex = AssetLoader.LoadTexture2D(GameBase.Game.Resources.Get(name));
            var loadedTextures = AssetLoader.LoadSpritesheetFromTexture(tex, rows, columns);
            var finalTextures = TextureAtlases.GetOrAdd(name, loadedTextures);

            if (finalTextures != loadedTextures)
            {
                foreach (var t in loadedTextures)
                    t.Dispose();

                tex.Dispose();
            }

            return finalTextures;
        }
    }
}