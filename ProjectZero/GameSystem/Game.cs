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
using ProjectZero.Framework;

namespace ProjectZero.GameSystem
{
    public class Game
    {
        private readonly Renderer _renderer;
        private readonly SoundRenderer _soundRenderer;
        private readonly Input _input;
        private readonly World _world;

        public Game(Renderer renderer, SoundRenderer soundRenderer, Input input)
        {
            _renderer = renderer;
            _soundRenderer = soundRenderer;
            _input = input;
            _world = new World(_renderer, _soundRenderer);

            _input.KeyEventHandler += KeyHandle;
            _input.MouseEventHandler += MouseHandle;
        }

        public void Initialize()
        {
            // initialize any game state here.
            _world.Initialize();            
        }

        public void RegisterContent()
        {
            // register any content here through sub systems.
            _world.RegisterContent();
        }

        public void ContentLoaded()
        {
            _world.ContentLoaded();
        }

        public void Frame(GameTime gameTime)
        {
            _world.Update(gameTime);
        }


        private void MouseHandle(object sender, MouseEventArgs e)
        {
            
        }

        private void KeyHandle(object sender, KeyEventArgs e)
        {
            if (e.Key == XnaInput.Keys.A && e.State == KeyState.Down)
            {
                _world.AddTower();
            }            
        }
    }
}
