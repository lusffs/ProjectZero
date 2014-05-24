using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectZero.GameSystem;

namespace ProjectZero.RenderSystem
{
    public partial class Renderer
    {
        private List<Command> _commands = new List<Command>();

        public void Render(GameTime gameTime)
        {
            _commands.Sort(new CommandComparer());
            SpriteBatch spriteBatch = null;
            foreach (var c in _commands)
            {
                if (spriteBatch != c.SpriteBatch)
                {
                    if (spriteBatch != null)
                    {
                        spriteBatch.End();
                    }
                    spriteBatch = c.SpriteBatch;
                    spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                }
                c.Render(this, gameTime);                
            }
            if (spriteBatch != null)
            {
                spriteBatch.End();
            }
            _commands.Clear();            
        }

        private class CommandComparer : IComparer<Command>
        {
            public int Compare(Command x, Command y)
            {                
                if (x == y)
                {
                    return 0;
                }

                // Clear always first.
                if (x.SpriteBatch == null)
                {
                    return -1;
                }
                if (y.SpriteBatch == null)
                {
                    return 1;
                }

                if (x.SpriteIndex < y.SpriteIndex)
                {
                    return -1;
                }
                else if (x.SpriteIndex > y.SpriteIndex)
                {
                    return 1;
                }

                /*if (x.AddedIndex < y.AddedIndex)
                {
                    return -1;
                }
                else if (x.AddedIndex > y.AddedIndex)
                {
                    return 1;
                }*/
                return 0;
            }
        }

        private abstract class Command
        {
            public Command(SpriteBatch spriteBatch, int spriteIndex, int addedIndex)
            {
                SpriteBatch = spriteBatch;
                SpriteIndex = spriteIndex;
                AddedIndex = addedIndex;
            }

            public abstract void Render(Renderer renderer, GameTime gameTime);

            public SpriteBatch SpriteBatch { get; private set; }

            public int SpriteIndex { get; private set; }

            public int AddedIndex { get; private set; }
        }

        private class ClearColorCommand : Command
        {
            private readonly Color _color;

            public ClearColorCommand(Color color) : base(null, int.MinValue, int.MinValue)
            {
                _color = color;
            }

            public override void Render(Renderer renderer, GameTime gameTime)
            {
                renderer.GraphicsDevice.Clear(_color);
            }
        }

        
        private class DrawImageCommand : Command
        {
            private readonly TextureHandle _texture;
            private readonly Vector2? _position;
            private readonly Rectangle? _drawRect;
            private readonly Rectangle? _sourceRect;            
            // for mouse pointer and so on.
            private readonly bool _forceDrawLast;

            private float SortValue
            {
                get
                {
                    if (_position != null)
                    {
                        return (int)(_position.Value.X / Map.TileSize) + (int)(_position.Value.Y / Map.TileSize) * Map.Rows;
                    }

                    return (int)(_drawRect.Value.X / Map.TileSize) + (int)(_drawRect.Value.Y / Map.TileSize) * Map.Rows;
                }
            }

            public DrawImageCommand(TextureHandle texture, Vector2? position, Rectangle? drawRect, Rectangle? sourceRect, SpriteBatch spriteBatch, int spriteIndex, int addedIndex, bool forceDrawLast = false) : base(spriteBatch, spriteIndex, addedIndex)
            {
                _texture = texture;
                _position = position;
                _drawRect = drawRect;
                _sourceRect = sourceRect;
                _forceDrawLast = forceDrawLast;                
            }

            public override void Render(Renderer renderer, GameTime gameTime)
            {
                float depth = 1.0f - (SortValue / (Map.Columns * Map.Rows)) * 0.5f - AddedIndex * 1.0f / (Map.Columns * Map.Rows) * 0.5f;
                //float depth = SortValue / (Map.Columns * Map.Rows);
                SpriteBatch.Draw((Texture2D)_texture.Texture, position: _position, drawRectangle: _drawRect, sourceRectangle: _sourceRect, depth: depth);                
            }
        }

        private class DrawStringCommand : Command
        {
            private readonly FontHandle _font;
            private readonly Vector2 _position;
            private readonly Color _color;
            private readonly string _text;

            public DrawStringCommand(FontHandle font, Vector2 position, Color color, string text, SpriteBatch spriteBatch, int spriteIndex, int addedIndex) : base(spriteBatch, spriteIndex, addedIndex)
            {
                _font = font;
                _position = position;
                _color = color;
                _text = text;            
            }


            private float SortValue
            {
                get
                {
                    float totalGridCells = Map.Rows * Map.Columns;

                    return (_position.X / Map.TileSize) + (_position.Y / Map.TileSize * Map.Rows);// + totalGridCells * _addedIndex;                    
                }
            }

            public override void Render(Renderer renderer, GameTime gameTime)
            {
                SpriteBatch.DrawString(_font.Font, _text, _position, _color);                
            }
        }
    }
}
