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
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                }
                c.Render(this, gameTime);                
            }
            if (spriteBatch != null)
            {
                spriteBatch.End();
            }            
        }

        private class CommandComparer : IComparer<Command>
        {
            public int Compare(Command x, Command y)
            {                
                if (x == y)
                {
                    return 0;
                }

                if (x.Layer < y.Layer)
                {
                    return -1;
                }
                else if (x.Layer > y.Layer)
                {
                    return 1;
                }

                return 0;
            }
        }

        private abstract class Command
        {
            public Command(SpriteBatch spriteBatch, Layer layer)
            {
                SpriteBatch = spriteBatch;
                Layer = layer; 
            }

            public abstract void Render(Renderer renderer, GameTime gameTime);

            public SpriteBatch SpriteBatch { get; private set; }

            public Layer Layer { get; private set; }            
        }

        private class ClearColorCommand : Command
        {
            private readonly Color _color;

            public ClearColorCommand(Color color) : base(null, Layer.Clear)
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
            private readonly int _addedIndex;


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

            public DrawImageCommand(TextureHandle texture, Vector2? position, Rectangle? drawRect, Rectangle? sourceRect, SpriteBatch spriteBatch, Layer layer, int addedIndex) : base(spriteBatch, layer)
            {
                _texture = texture;
                _position = position;
                _drawRect = drawRect;
                _sourceRect = sourceRect;
                _addedIndex = addedIndex;               
            }

            public override void Render(Renderer renderer, GameTime gameTime)
            {
                float depth = 1.0f - (SortValue / (Map.Columns * Map.Rows)) * 0.5f - _addedIndex * 1.0f / (Map.Columns * Map.Rows) * 0.5f;                
                SpriteBatch.Draw((Texture2D)_texture.Texture, position: _position, drawRectangle: _drawRect, sourceRectangle: _sourceRect, depth: depth);                
            }
        }

        private class DrawStringCommand : Command
        {
            private readonly FontHandle _font;
            private readonly Vector2 _position;
            private readonly Color _color;
            private readonly string _text;
            private readonly float _scale;

            public DrawStringCommand(FontHandle font, Vector2 position, Color color, string text, SpriteBatch spriteBatch, Layer layer, float scale) : base(spriteBatch, layer)
            {
                _font = font;
                _position = position;
                _color = color;
                _text = text;
                _scale = scale;
            }

            public override void Render(Renderer renderer, GameTime gameTime)
            {
                SpriteBatch.DrawString(_font.Font, _text, _position, _color, rotation: 0, origin: Vector2.Zero, scale: _scale, effects: SpriteEffects.None, depth:  0);
            }
        }

        private class FillRectCommand : Command
        {
            private readonly Rectangle _rect;
            private readonly Color _color;
            private readonly Texture2D _whiteTexture;
            private readonly int _addedIndex;

            public FillRectCommand(Rectangle rect, Color color, SpriteBatch spriteBatch, Texture2D whiteTexture, Layer layer, int addedIndex) : base(spriteBatch, layer)
            {
                _rect = rect;
                _color = color;
                _whiteTexture = whiteTexture;
                _addedIndex = addedIndex;
            }

            private float SortValue
            {
                get
                {
                    return (int)(_rect.X / Map.TileSize) + (int)(_rect.Y / Map.TileSize) * Map.Rows;
                }
            }

            public override void Render(Renderer renderer, GameTime gameTime)
            {
                float depth = 1.0f - (SortValue / (Map.Columns * Map.Rows)) * 0.5f - _addedIndex * 1.0f / (Map.Columns * Map.Rows) * 0.5f;
                SpriteBatch.Draw(_whiteTexture, drawRectangle: _rect, color: _color, depth: depth);
            }
        }
    }
}
