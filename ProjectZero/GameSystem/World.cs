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

        private Tower _tower;

        private GameTime _lastGameTime;

        private List<BaseEntity> _addedFrameEntites = new List<BaseEntity>();
        private List<BaseEntity> _removedFrameEntities = new List<BaseEntity>();

        public void RegisterContent()
        {
            Map.RegisterContent();

            _segmentTexture = Renderer.RegisterTexture2D("images/path_marker1.png");

            Entities.Add(new Monster("slime", this) { IsVisible = false });
            Entities[0].RegisterContent();

            _tower = new Tower(this);
            _tower.RegisterContent();
        }

        public void ContentLoaded()
        {
            Map.ContentLoaded();
            Entities.ForEach(x => x.ContentLoaded());
            _tower.ContentLoaded();         
        }

        public void Update(GameTime gameTime)
        {
            _lastGameTime = gameTime;

            Entities.AddRange(_addedFrameEntites);
            _addedFrameEntites.Clear();
            Entities.RemoveAll(x => _removedFrameEntities.Contains(x));
            _removedFrameEntities.Clear();

            Map.Update(gameTime);

            if (_mapHasNewBlockingEntity)
            {
                Point start, end;
                GetMonsterSpawnAndDefensePoints(out start, out end);
                _path = PathFinder.GetPath(Map.Cells, start, end);                
                _mapHasNewBlockingEntity = false;
            }
            
            if (_path != null)
            {
                foreach (var segment in _path)
                {
                    Renderer.DrawImage(_segmentTexture, new Vector2(segment.X * Map.TileSize, segment.Y * Map.TileSize), Layer.Path);
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

            foreach (var monster in Entities.OfType<Monster>())
            {
                monster.IsVisible = true;
                monster.WalkPath(_path, Map.MonsterSpawn.Position);
            }

            foreach (var tower in Entities.OfType<Tower>())
            {
                tower.StartDefending(_lastGameTime);
            }
        }

        private void GetMonsterSpawnAndDefensePoints(out Point monsterSpawn, out Point defensePoint)
        {
            monsterSpawn = new Point((int)(Map.MonsterSpawn.Position.X / Map.TileSize), (int)(Map.MonsterSpawn.Position.Y / Map.TileSize));
            defensePoint = new Point((int)(Map.DefensePoint.Position.X / Map.TileSize), (int)(Map.DefensePoint.Position.Y / Map.TileSize));
        }

        /// <summary>
        /// Will add <paramref name="entity"/> next frame. If not also removed in same frame.
        /// </summary>
        /// <param name="entity"></param>
        public void AddEntity(BaseEntity entity)
        {
            _addedFrameEntites.Add(entity);
        }

        /// <summary>
        /// Remove <paramref name="entity"/> next frame, even if added this frame.
        /// </summary>
        /// <param name="entity"></param>
        public void RemoveEntity(BaseEntity entity)
        {
            _removedFrameEntities.Add(entity);
        }

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

            Entities.Add(_tower.Clone(new Vector2(position.X, position.Y)));
        }
    }
}
