using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace ProjectZero.SoundSystem
{
    public partial class SoundRenderer
    {
        public void PlaySound(SoundHandle sound)
        {
            _commands.Add(new PlaySoundCommand(sound, SoundEffect.MasterVolume));
        }
    }
}
