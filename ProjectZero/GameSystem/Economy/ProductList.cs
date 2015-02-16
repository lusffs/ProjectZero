using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectZero.GameSystem.Entities;
using ProjectZero.InputSystem;
using ProjectZero.RenderSystem;

namespace ProjectZero.GameSystem.Economy
{
    public class ProductList
    {
        private Func<World, BaseTower>[] _towerFactory = new Func<World, BaseTower>[]
        {
            world => new Tower(world, 13),
            world => new MagicTower(world, 19)
        };

        private BaseTower[] _towers;

        private readonly Renderer _renderer;

        private FontHandle _font;

        private TextureHandle _mouseTower;

        private World _world;

        private Point _mousePosition;

        public ProductList(Renderer renderer)
        {
            _renderer = renderer;
        }     

        public void Init(World world)
        {
            _world = world;

            _towers = new BaseTower[_towerFactory.Length];
            for (var i = 0; i < _towerFactory.Length; i++)
            {
                _towers[i] = _towerFactory[i](world);                
                // TODO:    flytta till ContentLoad och använd korrekt uträkning.
                _towers[i].Position = new Vector2(i * Map.TileSize - 16, (Map.Rows + 1) * Map.TileSize - 16);
            }
        }

        public void ContentLoaded()
        {
            foreach (var i in _towers)
            {
                i.ContentLoaded();
            }
        }

        public void RegisterContent()
        {
            foreach (var i in _towers)
            {
                i.RegisterContent();
            }

            _font = _renderer.RegisterFont("fonts/console");

            _mouseTower = _renderer.RegisterTexture2D("images/ui/tower_marker.png");
        }

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < Map.Columns; i++)
            {
                _renderer.FillRect(new Rectangle(i * Map.TileSize, Map.Rows * Map.TileSize, Map.TileSize, Map.TileSize * Map.TileSize),
                    Color.Black, Layer.Map);
            }

            for (int i = 0; i < _towers.Length; i++)
            {                
                _renderer.DrawString(_font, _towers[i].Price.ToString(), new Vector2(i * Map.TileSize + 8, Map.Rows * Map.TileSize), Color.White, Layer.Map, scale: 1f);
                _towers[i].Update(gameTime);
            }

            DrawTowerMarkerOrTowerRadius();
        }

        private void DrawTowerMarkerOrTowerRadius()
        {
            // only draw if we are not touching the left/up border of the window.
            // window managment will start drawing regular cursor at that point resulting
            // in double pointer.
            if (_mousePosition.X >= 0 && _mousePosition.Y >= 0)
            {
                foreach (var tower in _world.Entities.OfType<Tower>())
                {
                    if (tower.ShouldDrawRange(_mousePosition))
                    {
                        return;
                    }
                }

                if (InBuyMode)
                {
                    float sizeFactor = (Map.TileSize * (Map.TileSize / (float)_mouseTower.Width));
                    _renderer.DrawImage(_mouseTower,
                        new Vector2(_mousePosition.X / Map.TileSize * Map.TileSize - sizeFactor,
                                    _mousePosition.Y / Map.TileSize * Map.TileSize - sizeFactor),
                        Layer.Last);
                }
            }
        }

        public void MouseHandle(object sender, MouseEventArgs e)
        {
            _mousePosition.X = e.X;
            _mousePosition.Y = e.Y;

            if (e.State == KeyState.Down)
            {
                if (e.X > 0 && e.X < Map.Columns * Map.TileSize && e.Y > Map.Rows * Map.TileSize && e.Y < (Map.Rows + 2) * Map.TileSize)
                {
                    InBuyMode = true;
                    // TODO:    handle diffrent tower sizes and snap.
                    int index = (int)(e.X / (Map.TileSize));
                    if (index >= _towers.Length)
                    {
                        index = _towers.Length - 1; 
                    }
                    BuyTower = _towers[index];
                }
                else
                {
                    InBuyMode = false;
                    BuyTower = null;
                }
            }
        }

        public bool InBuyMode { get; private set; }

        public BaseTower BuyTower { get; private set; }
    }
}
