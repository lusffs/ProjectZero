using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectZero.RenderSystem
{
    public abstract class RendererHandle
    {
        public abstract void Load(GraphicsDevice graphicsDevice, ContentManager contentManager);

        public abstract void Unload();
    }
}
