// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Wobble.IO
{
    public class DllResourceStore : IResourceStore<byte[]>
    {
        private readonly Assembly assembly;
        private readonly string space;

        public DllResourceStore(string dllName)
        {
            assembly = Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), dllName));
            space = Path.GetFileNameWithoutExtension(dllName);
        }

        public byte[] Get(string name)
        {
            using (var input = GetStream(name))
            {
                if (input == null)
                    return null;

                var buffer = new byte[input.Length];
                input.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        public virtual async Task<byte[]> GetAsync(string name)
        {
            using (var input = GetStream(name))
            {
                if (input == null)
                    return null;

                var buffer = new byte[input.Length];
                await input.ReadAsync(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        public Stream GetStream(string name)
        {
            var split = name.Split('/');
            for (var i = 0; i < split.Length - 1; i++)
                split[i] = NormalizeManifestSegment(split[i]);

            return assembly?.GetManifestResourceStream($@"{space}.{string.Join(".", split)}");
        }

        public IEnumerable<string> GetAvailableResources() => assembly.GetManifestResourceNames();

        private static string NormalizeManifestSegment(string segment)
        {
            segment = segment.Replace('-', '_');

            return !string.IsNullOrEmpty(segment) && char.IsDigit(segment[0])
                ? $"_{segment}"
                : segment;
        }

        #region IDisposable Support

        private bool isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                isDisposed = true;
            }
        }

        ~DllResourceStore() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
