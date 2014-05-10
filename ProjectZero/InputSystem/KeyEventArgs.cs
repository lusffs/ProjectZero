using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace ProjectZero.InputSystem
{
    public class KeyEventArgs : EventArgs
    {
        public Keys Key { get; private set; }

        public KeyState State { get; private set; }

        public KeyEventArgs(Keys key, KeyState state)
        {
            Key = key;
            State = state;
        }
    }
}
