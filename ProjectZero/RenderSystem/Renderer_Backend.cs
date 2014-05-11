﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectZero.RenderSystem
{
    public partial class Renderer
    {
        private List<Command> _commands = new List<Command>();

        public void Render(GameTime gameTime)
        {
            foreach (var c in _commands)
            {
                c.Render(this, gameTime);
            }
            _commands.Clear();
        }

        private abstract class Command
        {
            public abstract void Render(Renderer renderer, GameTime gameTime);
        }

        private class ClearColorCommand : Command
        {
            private readonly Color _color;

            public ClearColorCommand(Color color)
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
            private readonly Vector2? _poistion;
            private readonly Rectangle? _drawRect;
            private readonly SpriteBatch _spriteBatch;

            public DrawImageCommand(TextureHandle texture, Vector2? position, Rectangle? drawRect, SpriteBatch spriteBatch)
            {
                _texture = texture;
                _poistion = position;
                _drawRect = drawRect;
                _spriteBatch = spriteBatch;
            }

            public override void Render(Renderer renderer, GameTime gameTime)
            {
                // TODO:    should include sort mode, blend state.
                _spriteBatch.Begin();
                _spriteBatch.Draw((Texture2D)_texture.Texture, position: _poistion, drawRectangle: _drawRect);
                _spriteBatch.End();
            }
        }

        private class DrawStringCommand : Command
        {
            private readonly FontHandle _font;
            private readonly Vector2 _poistion;
            private readonly Color _color;
            private readonly string _text;
            private readonly SpriteBatch _spriteBatch;

            public DrawStringCommand(FontHandle font, Vector2 position, Color color, string text, SpriteBatch spriteBatch)
            {
                _font = font;
                _poistion = position;
                _color = color;
                _text = text;
                _spriteBatch = spriteBatch;
            }

            public override void Render(Renderer renderer, GameTime gameTime)
            {
                // TODO:    should include sort mode, blend state.
                _spriteBatch.Begin();// SpriteSortMode.Immediate, BlendState.AlphaBlend);
                _spriteBatch.DrawString(_font.Font, _text, _poistion, _color);
                _spriteBatch.End();
            }
        }
    }
}