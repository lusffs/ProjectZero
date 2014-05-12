using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectZero.RenderSystem
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
            _commands.Add(new DrawImageCommand(texture, position, null, null, _imageSpriteBatch));
        }

        public void DrawImage(TextureHandle texture, Vector2 position, int width, int height)
        {
            Texture2D t = (Texture2D)texture.Texture;
            Debug.Assert(t != null, "texture should be a handle for Texture2D");
            _commands.Add(new DrawImageCommand(texture, null, new Rectangle((int)position.X, (int)position.Y, width, height), null, _imageSpriteBatch));
        }

        public void DrawImage(TextureHandle texture, Vector2 position, int width, int height, Rectangle sourceRect)
        {
            Texture2D t = (Texture2D)texture.Texture;
            Debug.Assert(t != null, "texture should be a handle for Texture2D");
            _commands.Add(new DrawImageCommand(texture, null, new Rectangle((int)position.X, (int)position.Y, width, height), sourceRect, _imageSpriteBatch));
        }

        public void DrawString(FontHandle font, string text, Vector2 position, Color color)
        {
            _commands.Add(new DrawStringCommand(font, position, color, text, _textSpriteBatch));
        }
    }
}
