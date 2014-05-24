using System;
using System.Collections.Generic;
using System.IO;
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
    public class Map
    {
        /// <summary>
        /// In tiles.
        /// </summary>
        public const int Columns = 25;

        /// <summary>
        /// In tiles.
        /// </summary>
        public const int Rows = 15;

        public const int TileSize = 32;

        public BaseEntity[][] Grid { get; private set; }

        public MonsterSpawn MonsterSpawn { get; private set; }

        public DefensePoint DefensePoint { get; private set; }

        public Cell[,] Cells { get; private set; }

        public World World { get; private set; }

        public Map(string mapName, World world)
        {
            World = world;

            Grid = new BaseEntity[Rows][];

            using (var file = File.OpenText(Path.Combine(world.Renderer.ContentManager.RootDirectory, mapName + ".map")))
            {
                string line = file.ReadLine();
                string[] pos = line.Split(' ');
                MonsterSpawn = new MonsterSpawn(world) { Position = new Vector2(int.Parse(pos[0]) * TileSize, int.Parse(pos[1]) * TileSize) };
                line = file.ReadLine();
                pos = line.Split(' ');
                DefensePoint = new DefensePoint(world) { Position = new Vector2(int.Parse(pos[0]) * TileSize, int.Parse(pos[1]) * TileSize) };

                line = file.ReadLine();
                int row = 0;
                while (line != null)
                {
                    List<BaseEntity> columns = new List<BaseEntity>();
                    string[] typeAndNumberOfColumns = line.Split(' ');
                    int column = 0;
                    foreach (var typeAndNumberOfColumn in typeAndNumberOfColumns)
                    {
                        string type = new string(typeAndNumberOfColumn.TakeWhile(x => Char.IsLetter(x)).ToArray());
                        string numberOfColumns = new string(typeAndNumberOfColumn.SkipWhile(x => Char.IsLetter(x)).ToArray());
                        int numOfColumns = int.Parse(numberOfColumns);
                        columns.AddRange(Enumerable.Range(0, numOfColumns).Select((_x, index) => _factory[type](world, column + index, row)));
                        column += numOfColumns;
                    }
                    // TODO:    check columns.Count == Columns
                    Grid[row] = columns.ToArray();
                    row++;
                    line = file.ReadLine();
                }

                // TODO:    check row == Rows
            }
            InitCells();
        }

        private void InitCells()
        {
            Cells = new Cell[Rows, Columns];
            
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    var cell = new Cell();
                    cell.IsBlocked = Grid[row][column].Solid;
                    cell.IsStart = MonsterSpawn.Position.X == column * TileSize && MonsterSpawn.Position.Y == row * TileSize;
                    cell.IsTarget = DefensePoint.Position.X == column * TileSize && DefensePoint.Position.Y == row * TileSize;
                    Cells[row, column] = cell;
                }
            }            
        }

        private TextureHandle _monsterSpawnTexture;
        private TextureHandle _defensePointTexture;

        public void Update(GameTime gameTime)
        {
            ForAllGridCells(x => x.Update(gameTime));

            // TODO:    should be through entity update.
            World.Renderer.DrawImage(_monsterSpawnTexture, MonsterSpawn.Position, Layer.Fixed);
            World.Renderer.DrawImage(_defensePointTexture, DefensePoint.Position, Layer.Fixed);
        }

        public void ContentLoaded()
        {
            ForAllGridCells(x => x.ContentLoaded());
        }

        public void RegisterContent()
        {
            ForAllGridCells(x => x.RegisterContent());

            _monsterSpawnTexture = World.Renderer.RegisterTexture2D("images/start.png");
            _defensePointTexture = World.Renderer.RegisterTexture2D("images/end.png");
        }

        private void ForAllGridCells(Action<BaseEntity> action)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    action(Grid[row][column]);
                }
            }
        }
        private Dictionary<string, Func<World, int, int, BaseEntity>> _factory = new Dictionary<string, Func<World, int, int, BaseEntity>>()
        {
            { "g", (world, x, y) => Create(new SpriteEntity("images/tiles/grass.png", world, isAnimation: false, layer: Layer.Map), world, x, y, TileSize)},
            { "rg", (world, x, y) => Create(new SpriteEntity("images/tiles/rock_in_grass.png", world, isAnimation: false, layer: Layer.Map) { Solid = true }, world, x, y, TileSize)}
        };

        private static BaseEntity Create(BaseEntity e, World world, int x, int y, int tileSize)
        {
            e.Position.X = x * tileSize;
            e.Position.Y = y * tileSize;
            return e;
        } 
    }
}
