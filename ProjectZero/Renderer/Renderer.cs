using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectZero.Renderer
{
    public partial class Renderer
    {
        public GraphicsDevice GraphicsDevice { get; private set; }

        public ContentManager ContentManager { get; private set; }

        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        private SpriteBatch _imageSpriteBatch;

        public Renderer(GraphicsDeviceManager graphicsDeviceManager, ContentManager contentManager)
        {
            GraphicsDeviceManager = graphicsDeviceManager;
            ContentManager = contentManager;            
        }        
    }
}
