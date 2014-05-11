using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using XnaInput = Microsoft.Xna.Framework.Input;
using ProjectZero.InputSystem;
using ProjectZero.RenderSystem;
using ProjectZero.SoundSystem;

namespace ProjectZero.GameSystem
{
    public class Game
    {
        private readonly Renderer _renderer;
        private readonly SoundRenderer _soundRenderer;
        private readonly Input _input;

        private GameTime _gameTime = new GameTime();

        public Game(Renderer renderer, SoundRenderer soundRenderer, Input input)
        {
            _renderer = renderer;
            _soundRenderer = soundRenderer;
            _input = input;

            _input.KeyEventHandler += KeyHandle;
            _input.MouseEventHandler += MouseHandle;
        }

        public void Initialize()
        {
            // initialize any game state here.
            var r = new Random();
            slimeXTest = 50 + r.Next(150);
            slimeYTest = 50 + r.Next(150);
        }

        TextureHandle slimeImageTest;
        float slimeXTest, slimeYTest;

        FontHandle fontTest;

        SoundHandle soundTest;

        public void RegisterContent()
        {
            // register any content here through sub systems.
            slimeImageTest = _renderer.RegisterTexture2D("images/slime.png");
            fontTest = _renderer.RegisterFont("fonts/console");

            soundTest = _soundRenderer.RegisterSound("sound/plong.wav");
        }

        double soundTestLastTime = 0;

        public void Frame(GameTime gameTime)
        {
            _gameTime = gameTime;

            // update game state stuff here.

            // update output here based on updated game state stuff.

            if (gameTime.TotalGameTime.TotalMilliseconds > soundTestLastTime + 2000)
            {
                soundTestLastTime = gameTime.TotalGameTime.TotalMilliseconds;
                _soundRenderer.PlaySound(soundTest);
            }

            _renderer.ClearScreen(Color.AliceBlue);

            _renderer.DrawImage(slimeImageTest, new Vector2(slimeXTest, slimeYTest));

            _renderer.DrawImage(slimeImageTest, new Vector2(10, 10), 50, 50);

            _renderer.DrawString(fontTest, string.Format("Hello World!!! {0}", gameTime.TotalGameTime.TotalMilliseconds), new Vector2(_renderer.GraphicsDevice.Viewport.Width / 2, 10), Color.Black);
        }

        private void MouseHandle(object sender, MouseEventArgs e)
        {
            
        }

        private void KeyHandle(object sender, KeyEventArgs e)
        {
            const float speed = 50; // 50 pixels per second.

            if (e.Key == XnaInput.Keys.Left && e.State == KeyState.Pressed)
            {
                slimeXTest -= speed * _gameTime.ElapsedGameTime.Milliseconds / 1000f;
            }
            else if (e.Key == XnaInput.Keys.Right && e.State == KeyState.Pressed)
            {
                slimeXTest += speed * _gameTime.ElapsedGameTime.Milliseconds / 1000f;
            }
            else if (e.Key == XnaInput.Keys.Up && e.State == KeyState.Pressed)
            {
                slimeYTest -= speed * _gameTime.ElapsedGameTime.Milliseconds / 1000f;
            }
            else if (e.Key == XnaInput.Keys.Down && e.State == KeyState.Pressed)
            {
                slimeYTest += speed * _gameTime.ElapsedGameTime.Milliseconds / 1000f;
            }
        }
    }
}
