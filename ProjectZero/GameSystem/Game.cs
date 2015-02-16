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
using ProjectZero.GameSystem.Entities;
using ProjectZero.GameSystem.Economy;

namespace ProjectZero.GameSystem
{
    public class Game
    {
        private readonly Renderer _renderer;
        private readonly SoundRenderer _soundRenderer;
        private readonly Input _input;
        private readonly World _world;
        private readonly ProductList _productList;
        private readonly Wallet _wallet;

        public Game(Renderer renderer, SoundRenderer soundRenderer, Input input)
        {
            _renderer = renderer;
            _soundRenderer = soundRenderer;
            _input = input;
            _world = new World(_renderer, _soundRenderer);
            _productList = new ProductList(_renderer);
            _wallet = new Wallet(_renderer);

            _input.KeyEventHandler += KeyHandle;
            _input.MouseEventHandler += MouseHandle;
            _input.MouseEventHandler += _productList.MouseHandle;
        }

        public void Initialize()
        {
            // initialize any game state here.
            _world.Initialize();
            _productList.Init(_world);
        }

        TextureHandle _mousePointer;
        Point _mousePosition;
        

        public void RegisterContent()
        {
            // register any content here through sub system 
            _world.RegisterContent();

            _productList.RegisterContent();
            _wallet.RegisterContent();

            _mousePointer = _renderer.RegisterTexture2D("images/ui/cursor.png");            
        }

        public void ContentLoaded()
        {
            _world.ContentLoaded();
            _productList.ContentLoaded();
            _wallet.ConentLoaded();
        }

        public void Frame(GameTime gameTime)
        {            
            _soundRenderer.BeginFrame();
            _renderer.BeginFrame();

            _renderer.ClearScreen(Color.Pink);
            
            _world.Update(gameTime);
            DrawMouse();
         
            _productList.Draw(gameTime);
            _wallet.Draw(gameTime);
        }        

        private void DrawMouse()
        {
            if (_mousePosition.X >= 0 && _mousePosition.Y >= 0)
            {
                _renderer.DrawImage(_mousePointer, new Vector2(_mousePosition.X, _mousePosition.Y), Layer.Last);                
            }
        }

        private void MouseHandle(object sender, MouseEventArgs e)
        {
            _mousePosition.X = e.X;
            _mousePosition.Y = e.Y;

            //Invalid click
            if (_mousePosition.X < 0 || (_mousePosition.X / Map.TileSize) > Map.Columns - 1 || _mousePosition.Y < 0 || (_mousePosition.Y / Map.TileSize) > Map.Rows - 1)
                return;

            Wallet.Transaction transaction;
            if (_productList.InBuyMode && e.Button == MouseButton.Left && e.State == KeyState.Down && (transaction = _wallet.Reservation(_productList.BuyTower)) != null)
            {
                if (_world.AddTower(new Point((int)(_mousePosition.X / Map.TileSize), (int)(_mousePosition.Y / Map.TileSize)), _productList.BuyTower))
                {
                    _wallet.Purchase(transaction);
                }
            }

            if (e.Button == MouseButton.Right && e.State == KeyState.Down)
            {
                _world.AddMonster();
            }
        }

        private void KeyHandle(object sender, KeyEventArgs e)
        {
            
        }
    }
}
