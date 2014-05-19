using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace ProjectZero.SoundSystem
{
    public abstract class SoundHandle : SoundSystemHandle
    {
        /// <summary>
        /// Will be null if sound isn't loaded.
        /// </summary>
        /// <returns></returns>
        public abstract SoundEffect Sound { get; }

        public abstract TimeSpan Duration { get; }
    }
}
