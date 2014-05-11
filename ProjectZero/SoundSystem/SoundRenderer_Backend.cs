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
        private List<Command> _commands = new List<Command>();

        public void Render(GameTime gameTime)
        {
            foreach (var c in _commands)
            {
                c.Render(this, gameTime);
            }
            _commands.Clear();
        }

        private abstract class Command
        {
            public abstract void Render(SoundRenderer renderer, GameTime gameTime);
        }

        private class PlaySoundCommand : Command
        {
            private readonly SoundHandle _sound;
            private readonly float _volume;
            private readonly float _pitch;
            private readonly float _pan;

            public PlaySoundCommand(SoundHandle sound, float volume, float pitch = 0f, float pan = 0f)
            {
                _sound = sound;
                _volume = volume;
                _pitch = pitch;
                _pan = pan;
            }

            public override void Render(SoundRenderer renderer, GameTime gameTime)
            {                
                _sound.Sound.Play(_volume, _pitch, _pan);                
            }
        }
    }
}
