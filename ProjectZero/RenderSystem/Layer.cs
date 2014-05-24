using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectZero.RenderSystem
{
    public enum Layer
    {
        /// <summary>
        /// Only for ClearScreen, should not be used directly.
        /// </summary>
        Clear = 0,

        Map = 1,
        Path = 2,
        Fixed = 3,
        Dynamic = 4,
        Last = 5
    }
}
