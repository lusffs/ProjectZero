﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectZero.RenderSystem
{
    public partial class Renderer
    {
        public GraphicsDevice GraphicsDevice { get; private set; }

        public ContentManager ContentManager { get; private set; }

        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        /// <summary>
        /// All DrawImage uses this.
        /// </summary>
        private SpriteBatch _imageSpriteBatch;

        /// <summary>
        /// All DrawText uses this.
        /// </summary>
        private SpriteBatch _textSpriteBatch;

        public Renderer(GraphicsDeviceManager graphicsDeviceManager, ContentManager contentManager)
        {
            GraphicsDeviceManager = graphicsDeviceManager;
            ContentManager = contentManager;            
        }        
    }
}