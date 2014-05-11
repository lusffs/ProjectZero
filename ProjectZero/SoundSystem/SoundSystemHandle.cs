using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectZero.SoundSystem
{
    public abstract class SoundSystemHandle
    {
        public abstract void Load();

        public abstract void Unload();
    }
}
