using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectZero.Renderer
{
    public partial class Renderer
    {
        public void ClearScreen(Color color)
        {
            _commands.Add(new ClearColorCommand(color));
        }

        public void DrawImage(TextureHandle texture, Vector2 position)
        {
            Texture2D t = (Texture2D)texture.Texture;
            Debug.Assert(t != null, "texture should be a handle for Texture2D");
            _commands.Add(new DrawImageCommand(texture, position, null, _imageSpriteBatch));
        }
    }
}
