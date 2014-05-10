﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using XnaInput = Microsoft.Xna.Framework.Input;
using ProjectZero.InputSystem;
using ProjectZero.RenderSystem;

namespace ProjectZero.GameSystem
{
    public class Game
    {
        private readonly Renderer _renderer;
        private readonly Input _input;

        private GameTime _gameTime = new GameTime();

        public Game(Renderer renderer, Input input)
        {
            _renderer = renderer;
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

        public void RegisterContent()
        {
            // register any content here through sub systems.
            slimeImageTest = _renderer.RegisterTexture2D("images/Slime.png");
        }

        public void Frame(GameTime gameTime)
        {
            _gameTime = gameTime;

            _renderer.ClearScreen(Color.AliceBlue);

            _renderer.DrawImage(slimeImageTest, new Vector2(slimeXTest, slimeYTest));
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
