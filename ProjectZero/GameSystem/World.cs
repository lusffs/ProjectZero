using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectZero.Framework;
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

        public List<BaseEntity> Entities { get; private set; }

        public World(Renderer renderer, SoundRenderer soundRenderer)
        {
            Renderer = renderer;
            SoundRenderer = soundRenderer;
            Entities = new List<BaseEntity>();
        }

        public void Initialize()
        {
            Map = new Map("maps/test", this);
        }

        private TextureHandle _segmentTexture;

        private bool _mapHasNewBlockingEntity = true;
        private List<Point> _path = new List<Point>();

        private TextureHandle _towerTexture;

        public void RegisterContent()
        {
            Map.RegisterContent();

            _segmentTexture = Renderer.RegisterTexture2D("images/path_marker1.png");

            Entities.Add(new Monster("slime", this));
            Entities[0].RegisterContent();

            _towerTexture = Renderer.RegisterTexture2D("images/tower.png");
        }

        public void ContentLoaded()
        {
            Map.ContentLoaded();
            Entities.ForEach(x => x.ContentLoaded());            
        }

        public void Update(GameTime gameTime)
        {
            Map.Update(gameTime);

            if (_mapHasNewBlockingEntity)
            {
                Point start, end;
                GetMonsterSpawnAndDefensePoints(out start, out end);
                _path = PathFinder.GetShortestPath(Map.Cells, start, end);                
                _mapHasNewBlockingEntity = false;
            }
            
            if (_path != null)
            {
                foreach (var segment in _path)
                {
                    Renderer.DrawImage(_segmentTexture, new Vector2(segment.X * Map.TileSize, segment.Y * Map.TileSize));
                }                
            }

            Entities.ForEach(x => x.Update(gameTime));                  
        }

        public void AddMonster()
        {
            if (_path == null)
            {
                return;
            }

            ((Monster)Entities[Entities.Count - 1]).WalkPath(_path, Map.MonsterSpawn.Position);
        }

        private void GetMonsterSpawnAndDefensePoints(out Point monsterSpawn, out Point defensePoint)
        {
            monsterSpawn = new Point((int)(Map.MonsterSpawn.Position.X / Map.TileSize), (int)(Map.MonsterSpawn.Position.Y / Map.TileSize));
            defensePoint = new Point((int)(Map.DefensePoint.Position.X / Map.TileSize), (int)(Map.DefensePoint.Position.Y / Map.TileSize));
        }

        private Random _random = new Random();

        public void AddTower(Point position)
        {
            _mapHasNewBlockingEntity = true;
            var cell = Map.Cells[position.Y, position.X];
            var gridCell = Map.Grid[position.Y][position.X];
            if (cell.IsStart || cell.IsTarget || gridCell.Solid)
            {
                return;
            }

            gridCell.Solid = true;
            cell.IsBlocked = true;
            // If the new tower blocks the path totally, don't allow it
            Point start, end;
            GetMonsterSpawnAndDefensePoints(out start, out end);
            if (!PathFinder.PathExists(Map.Cells, start, end))
            {
                gridCell.Solid = false;
                cell.IsBlocked = false;
                return;
            }

            Entities.Insert(0, new SpriteEntity("images/tower.png", this, false)
            {
                Image = _towerTexture,
                Solid = true,
                Position = new Vector2(position.X * Map.TileSize - Map.TileSize * (Map.TileSize / (float)_towerTexture.Width), position.Y * Map.TileSize - Map.TileSize * (Map.TileSize / (float)_towerTexture.Height))
            });
        }
    }
}
