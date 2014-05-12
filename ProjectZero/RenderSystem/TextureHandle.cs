using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectZero.RenderSystem
{
    public abstract class TextureHandle : RendererHandle
    {
        /// <summary>
        /// Will be null if texture isn't loaded.
        /// </summary>
        /// <returns></returns>
        public abstract Texture Texture { get; }

        public abstract int Width { get; }

        public abstract int Height { get; }

    }
}
