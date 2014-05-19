using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectZero.Framework.PathFinding;
using ProjectZero.GameSystem.Entities;
using ProjectZero.RenderSystem;
using ProjectZero.SoundSystem;

namespace ProjectZero.GameSystem
{
    public class World
    {
        public Renderer Renderer { get; private set; }

        public SoundRenderer SoundRenderer { get; private set; }

        public Map Map { get; private set; }

        public World(Renderer renderer, SoundRenderer soundRenderer)
        {
            Renderer = renderer;
            SoundRenderer = soundRenderer;
        }

        public void Initialize()
        {
            Map = new Map("maps/test", this);

        }

        private TextureHandle _segmentTexture;

        private TextureHandle _tower;

        private bool _mapHasNewBlockingEntity = true;
        private List<Tuple<int, int>> _path = new List<Tuple<int, int>>(); 

        public void RegisterContent()
        {
            Map.RegisterContent();

            _segmentTexture = Renderer.RegisterTexture2D("images/path_marker1.png");
            _tower = Renderer.RegisterTexture2D("images/tower.png");
        }

        public void ContentLoaded()
        {
            Map.ContentLoaded();            
        }

        public void Update(GameTime gameTime)
        {
            Map.Update(gameTime);

            if (_mapHasNewBlockingEntity)
            {
                _path = PathFinder.GetShortestPath(Map.Cells, Tuple.Create(0, 0), Tuple.Create(10, 22));
                _mapHasNewBlockingEntity = false;
            }
            
            if (_path != null)
            {
                foreach (var segment in _path)
                {
                    Renderer.DrawImage(_segmentTexture, new Vector2(segment.Item2 * Map.TileSize, segment.Item1 * Map.TileSize));
                }
            }

            for (int row = 0; row < Map.Rows; row++)
            {
                for (int column = 0; column < Map.Columns; column++)
                {
                    if (Map.Grid[row][column].Solid)
                    {
                        Renderer.DrawImage(_tower, new Vector2(column * Map.TileSize - 16, row * Map.TileSize - 16));
                    }
                }
            }            
        }

        private Random _random = new Random();

        public void AddTower()
        {
            _mapHasNewBlockingEntity = true;
            int x = _random.Next(Map.Columns);
            int y = _random.Next(Map.Rows);

            var cell = Map.Cells[y, x];
            int runaway = 0;

            while (cell.IsStart || cell.IsTarget)
            {
                x = _random.Next(Map.Columns);
                y = _random.Next(Map.Rows);
                cell = Map.Cells[y, x];

                if (runaway++ > 1000)
                {
                    return;
                }
            }

            Map.Grid[y][x].Solid = true;
            Map.Cells[y, x].IsBlocked = true;
            // If the new tower blocks the path totally, don't allow it
            if (!PathFinder.PathExists(Map.Cells, Tuple.Create(0, 0), Tuple.Create(10, 22)))
            {
                Map.Grid[y][x].Solid = false;
                Map.Cells[y, x].IsBlocked = false;
            }
        }
    }
}
