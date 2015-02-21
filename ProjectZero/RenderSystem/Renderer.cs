using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectZero.GameSystem;

namespace ProjectZero.RenderSystem
{
    public partial class Renderer
    {
        public const int ScreenWidth = 800;
        public const int ScreenHeight = 480;

        public GraphicsDevice GraphicsDevice { get; private set; }

        public ContentManager ContentManager { get; private set; }

        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        private SpriteBatch[] _layers = new SpriteBatch[(int)Layer.Last + 1];

        public Renderer(GraphicsDeviceManager graphicsDeviceManager, ContentManager contentManager)
        {
            GraphicsDeviceManager = graphicsDeviceManager;
            ContentManager = contentManager;            
        }

        public Vector2? AdjustFromVirtual(Vector2? position)
        {
            if (position == null)
            {
                return null;
            }

            var scale = GetVirtualScale();

            return new Vector2(scale.X * position.Value.X, scale.Y * position.Value.Y);
        }

        public Vector2 AdjustFromVirtual(Vector2 position)
        {
            var scale = GetVirtualScale();

            return new Vector2(scale.X * position.X, scale.Y * position.Y);
        }

        public Rectangle? AdjustFromVirtual(Rectangle? rectangle)
        {
            if (rectangle == null)
            {
                return null;
            }

            var scale = GetVirtualScale();

            return new Rectangle(
                (int)(scale.X * rectangle.Value.X),
                (int)(scale.Y * rectangle.Value.Y),
                (int)(scale.X * rectangle.Value.Width),
                (int)(scale.Y * rectangle.Value.Height));
        }

        public void AdjustToVirtual(int x, int y, out int xScaled, out int yScaled)
        {
            var scale = GetRealScale();
            xScaled = (int)(scale.X * x);
            yScaled = (int)(scale.Y * y);
        }

        public Vector2 AdjustToVirtual(Vector2 position)
        {
            var scale = GetRealScale();

            return new Vector2(scale.X * position.X, scale.Y * position.Y);
        }

        private Vector2 GetVirtualScale()
        {
            // TODO:    should include Viewport.X/Y? normally 0, so ignore for now.
            return new Vector2(
                GraphicsDevice.Viewport.Width / (float)ScreenWidth,
                GraphicsDevice.Viewport.Height / (float)ScreenHeight);
        }

        private Vector2 GetRealScale()
        {
            // TODO:    should include Viewport.X/Y? normally 0, so ignore for now.
            return new Vector2(
                ScreenWidth / (float)GraphicsDevice.Viewport.Width,
                ScreenHeight / (float)GraphicsDevice.Viewport.Height);
        }
    }
}
