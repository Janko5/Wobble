using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ManagedBass;

namespace Wobble.Audio
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown when an illegal action is called during
    ///     audio engine usage.
    /// </summary>
    public class AudioEngineException : Exception
    {
        /// <summary>
        ///     The BASS error code associated with this exception.
        /// </summary>
        public Errors BassError { get; }

        public AudioEngineException() { }

        public AudioEngineException(string message) : base(message) { }

        public AudioEngineException(string message, Errors error) : base(message)
        {
            BassError = error;
        }
    }
}
