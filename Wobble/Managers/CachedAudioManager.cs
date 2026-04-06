using System.Collections.Concurrent;
using System.Collections.Generic;
using Wobble.Audio.Samples;
using Wobble.Audio.Tracks;

namespace Wobble.Managers
{
    public static class CachedAudioManager
    {
        /// <summary>
        /// </summary>
        public static ConcurrentDictionary<string, AudioTrack> Tracks { get; } = new ConcurrentDictionary<string, AudioTrack>();

        /// <summary>
        /// </summary>
        public static ConcurrentDictionary<string, AudioSample> Samples { get; } = new ConcurrentDictionary<string, AudioSample>();

        /// <summary>
        ///     Loads an AudioTrack and caches it for later use
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AudioTrack LoadTrack(string name)
        {
            if (Tracks.TryGetValue(name, out var track))
                return track;

            var loadedTrack = new AudioTrack(GameBase.Game.Resources.Get(name));
            var finalTrack = Tracks.GetOrAdd(name, loadedTrack);

            if (finalTrack != loadedTrack)
                loadedTrack.Dispose();

            return finalTrack;
        }

        /// <summary>
        ///     Loads an AudioSample and caches it for later use
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AudioSample LoadSample(string name)
        {
            if (Samples.TryGetValue(name, out var sample))
                return sample;

            var loadedSample = new AudioSample(GameBase.Game.Resources.Get(name));
            var finalSample = Samples.GetOrAdd(name, loadedSample);

            if (finalSample != loadedSample)
                loadedSample.Dispose();

            return finalSample;
        }
    }
}