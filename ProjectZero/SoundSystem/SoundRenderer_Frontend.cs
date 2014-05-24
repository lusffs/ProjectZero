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
        public void BeginFrame()
        {
            _commands.Clear();
        }

        public void PlaySound(SoundHandle sound, float volume = 0, float pan = 0)
        {
            _commands.Add(new PlaySoundCommand(sound, volume: volume <= 0 ? SoundEffect.MasterVolume : volume, pan: pan));
        }        
    }
}
