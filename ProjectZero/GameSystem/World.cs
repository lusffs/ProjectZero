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

        public List<Point> Path { get { return _path; } }

        public int PlayerScore { get; set; }

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

        private FontHandle _scoreFont;

        private Monster _slime;

        public void RegisterContent()
        {
            Map.RegisterContent();

            _segmentTexture = Renderer.RegisterTexture2D("images/path_marker1.png");

            _slime = new Monster("slime", this) { IsVisible = false };
            _slime.RegisterContent();

            _tower = new Tower(this);
            _tower.RegisterContent();

            _scoreFont = Renderer.RegisterFont("fonts/console");
        }

        public void ContentLoaded()
        {
            Map.ContentLoaded();
            Entities.ForEach(x => x.ContentLoaded());
            _tower.ContentLoaded();
            _slime.ContentLoaded();
        }

        public void Update(GameTime gameTime)
        {
            _lastGameTime = gameTime;

            AddAndRemoveFrameEntities();

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

            string scoreString = string.Format("SCORE {0}", PlayerScore);
            float scale = 2.0f;
            var scoreSize = _scoreFont.Font.MeasureString(scoreString) * scale;
            Renderer.DrawString(_scoreFont, scoreString, new Vector2(Renderer.GraphicsDevice.Viewport.Width, Renderer.GraphicsDevice.Viewport.Height - 16) - scoreSize, Color.WhiteSmoke, Layer.Last, scale);
        }

        private void AddAndRemoveFrameEntities()
        {
            Entities.AddRange(_addedFrameEntites);
            _addedFrameEntites.Clear();
            Entities.RemoveAll(x => _removedFrameEntities.Contains(x));
            _removedFrameEntities.Clear();
        }

        public void AddMonster()
        {
            if (_path == null)
            {
                return;
            }

            AddSlime();

            foreach (var monster in Entities.OfType<Monster>())
            {
                if (!monster.IsVisible)
                {
                    monster.IsVisible = true;
                    monster.WalkPath(_path, Map.MonsterSpawn.Position);
                }
            }

            foreach (var tower in Entities.OfType<Tower>())
            {
                if (!tower.IsDefending)
                {
                    tower.StartDefending(_lastGameTime);
                }
            }
        }

        private void AddSlime()
        {
            // TODO:    make a class out for slime?
            var monsterToAdd = _slime.Clone(Vector2.Zero);
            Entities.Add(monsterToAdd);            
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

        public bool AddTower(Point position)
        {
            _mapHasNewBlockingEntity = true;
            var cell = Map.Cells[position.Y, position.X];
            var gridCell = Map.Grid[position.Y][position.X];
            if (cell.IsStart || cell.IsTarget || gridCell.Solid)
            {
                return false;
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
                return false;
            }

            Entities.Add(_tower.Clone(new Vector2(position.X, position.Y)));

            return true;
        }
    }
}
