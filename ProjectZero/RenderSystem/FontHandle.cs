using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectZero.RenderSystem
{
    public abstract class FontHandle : RendererHandle
    {
        /// <summary>
        /// Will be null if font isn't loaded.
        /// </summary>
        /// <returns></returns>
        public abstract SpriteFont Font { get; }
    }
}
