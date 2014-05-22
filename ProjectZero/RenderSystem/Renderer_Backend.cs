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

            foreach (var c in _commands)
            {
                c.Render(this, gameTime);
            }
            _commands.Clear();            
        }

        private class CommandComparer : IComparer<Command>
        {
            public int Compare(Command x, Command y)
            {                
                ClearColorCommand clearX = x as ClearColorCommand;
                ClearColorCommand clearY = y as ClearColorCommand;
                if (clearX != null && clearY != null)
                {
                    return clearX.AddedIndex <= clearY.AddedIndex ? -1 : 1;
                }

                if (clearX != null)
                {
                    return clearX.AddedIndex < y.AddedIndex ? -1 : 1;
                }

                if (clearY != null)
                {
                    return x.AddedIndex < clearY.AddedIndex ? -1 : 1;
                }

                if (x.SortValue < y.SortValue)
                {
                    return -1;
                }
                else if (x.SortValue > y.SortValue)
                {
                    return 1;
                }


                // same position in world, so decide based on added index.
                if (x.AddedIndex < y.AddedIndex)
                {
                    return -1;
                }
                else if (x.AddedIndex > y.AddedIndex)
                {
                    return 1;
                }

                return 0;                
            }
        }

        private abstract class Command
        {
            public abstract void Render(Renderer renderer, GameTime gameTime);

            public abstract int SortValue { get; }

            public abstract int AddedIndex { get; }
        }

        private class ClearColorCommand : Command
        {
            private readonly Color _color;
            private readonly int _addedIndex;

            public ClearColorCommand(Color color, int addedIndex)
            {
                _color = color;
                _addedIndex = addedIndex;
            }

            public override int SortValue
            {
                get
                {
                    // always first, but in order added with all the rest of the commands.
                    // will special case when sorting on added index.
                    // all commands with AddedIndex > will be after.
                    return int.MinValue;
                }                
            }

            public override int AddedIndex
            {
                get
                {
                    return _addedIndex;
                }
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
            private readonly SpriteBatch _spriteBatch;
            private readonly int _addedIndex;
            // for mouse pointer and so on.
            private readonly bool _forceDrawLast;

            public override int SortValue
            {
                get
                {
                    
                    
                    // in map order.
                    int i = _position != null ?
                        ((int)(_position.Value.X / Map.TileSize) + (int)(_position.Value.Y / Map.TileSize) * Map.Rows) :
                        (_drawRect.Value.X / Map.TileSize + _drawRect.Value.Y / Map.TileSize * Map.Rows);
                    return !_forceDrawLast ? i : (int.MaxValue - i);
                }
            }

            public override int AddedIndex
            {
                get
                {
                    return _addedIndex;
                }
            }

            public DrawImageCommand(TextureHandle texture, Vector2? position, Rectangle? drawRect, Rectangle? sourceRect, SpriteBatch spriteBatch, int addedIndex, bool forceDrawLast = false)
            {
                _texture = texture;
                _position = position;
                _drawRect = drawRect;
                _sourceRect = sourceRect;
                _spriteBatch = spriteBatch;
                _addedIndex = addedIndex;
                _forceDrawLast = forceDrawLast;
            }

            public override void Render(Renderer renderer, GameTime gameTime)
            {
                // TODO:    should include sort mode, blend state.
                _spriteBatch.Begin();
                _spriteBatch.Draw((Texture2D)_texture.Texture, position: _position, drawRectangle: _drawRect, sourceRectangle: _sourceRect);
                _spriteBatch.End();
            }
        }

        private class DrawStringCommand : Command
        {
            private readonly FontHandle _font;
            private readonly Vector2 _position;
            private readonly Color _color;
            private readonly string _text;
            private readonly SpriteBatch _spriteBatch;
            private readonly int _addedIndex;

            public override int SortValue
            {
                get
                {
                    // in map order, but always last.
                    int i = (int)(_position.X / Map.TileSize) + (int)(_position.Y / Map.TileSize) * Map.Rows;
                    return int.MaxValue - i;
                }
            }

            public override int AddedIndex
            {
                get
                {
                    return _addedIndex;
                }
            }

            public DrawStringCommand(FontHandle font, Vector2 position, Color color, string text, SpriteBatch spriteBatch, int addedIndex)
            {
                _font = font;
                _position = position;
                _color = color;
                _text = text;
                _spriteBatch = spriteBatch;
                _addedIndex = addedIndex;
            }

            public override void Render(Renderer renderer, GameTime gameTime)
            {
                // TODO:    should include sort mode, blend state.
                _spriteBatch.Begin();// SpriteSortMode.Immediate, BlendState.AlphaBlend);
                _spriteBatch.DrawString(_font.Font, _text, _position, _color);
                _spriteBatch.End();
            }
        }
    }
}
