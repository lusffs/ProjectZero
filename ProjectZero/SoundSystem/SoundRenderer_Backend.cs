using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

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
    }
}
