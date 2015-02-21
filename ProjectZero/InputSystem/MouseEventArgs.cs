using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace ProjectZero.InputSystem
{
    public class MouseEventArgs
    {
        /// <summary>
        /// In virtual screen coordinates.
        /// </summary>
        /// <returns></returns>
        public int X { get; private set; }

        /// <summary>
        /// In virtual screen coordinates.
        /// </summary>
        /// <returns></returns>
        public int Y { get; private set; }

        public MouseButton Button { get; private set; }

        public KeyState State { get; private set; }

        public MouseEventArgs(int x, int y, MouseButton button, KeyState state)
        {
            X = x;
            Y = y;
            Button = button;
            State = state;
        }
    }
}
