using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectZero.RenderSystem;

namespace ProjectZero.Framework
{
    public class FpsMeter
    {
        private const int FrameCount = 6;
        private long[] _fpsPreviousTimes = new long[FrameCount];
        private long _fpsPreviousTime;
        private int _fpsIndex;
        private string _fpsString;
        private readonly Renderer _renderer;
        private readonly Color _color;
        private readonly float _y;
        private readonly FontHandle _font;

        public FpsMeter(Renderer renderer, Color color, float y, FontHandle font)
        {
            _renderer = renderer;
            _color = color;
            _y = y;
            _font = font;
        }

        public void Update(GameTime gameTime)
        {
            long elapsed = (int)(gameTime.TotalGameTime.TotalMilliseconds * 1000);
            long frameTime = elapsed - _fpsPreviousTime;

            _fpsPreviousTime = elapsed;
            _fpsPreviousTimes[_fpsIndex % FrameCount] = frameTime;

            _fpsIndex++;

            if (_fpsIndex > FrameCount)
            {
                // average multiple frames together to smooth changes out a bit
                long total = 0;

                for (int i = 0; i < FrameCount; i++)
                {
                    total += _fpsPreviousTimes[i];
                }

                if (total == 0)
                {
                    total = 1;
                }

                long fps = (1000000 * FrameCount) / total;
                //fps = (fps + 500) / 1000;

                _fpsString = string.Format("{0} FPS", fps);                
            }
        }

        public void Draw()
        {
            if (_fpsIndex > FrameCount)
            {
                var position = _font.Font.MeasureString(_fpsString);
                position.X = _renderer.GraphicsDevice.Viewport.Width - position.X;
                position.Y = _y;
                _renderer.DrawString(_font, _fpsString, position, _color, Layer.Last);
            }
        }
    }
}
