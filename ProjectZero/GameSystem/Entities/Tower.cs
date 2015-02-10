using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectZero.RenderSystem;

namespace ProjectZero.GameSystem.Entities
{
    public class Tower : BaseTower
    {
        private const float Range = 3.0f;
        private bool _shouldDrawRange;
        private TextureHandle _rangeTexture;
        private bool _isDefending;
        private const float FireRateInMilliSeconds = 2000.0f;
        private const int FireRateRandomMilliSeconds = 400;
        private double _lastFireTime;

        public Tower(World world) : base(world)
        {
            _rangeTexture = World.Renderer.RegisterTexture2D("images/ui/radius.png");
        }
        
        public override BaseEntity Clone(Vector2 position)
        {
            return new Tower(World)
            {
                Animation = Animation,
                AssetFileName = AssetFileName,
                Image = Image,
                Position = new Vector2(position.X * Map.TileSize, position.Y * Map.TileSize) - _sizeOffset,
                Solid = Solid,
                _shouldDrawRange = false,
                _rangeTexture = _rangeTexture,
                _sizeOffset = _sizeOffset
            };
        }

        private static Random _random = new Random();

        public bool IsDefending { get { return _isDefending; } }

        public void StartDefending(GameTime gameTime)
        {
            _isDefending = true;
            _lastFireTime = gameTime.TotalGameTime.TotalMilliseconds + _random.Next(FireRateRandomMilliSeconds);
        }

        public override void Update(GameTime gameTime)
        {
            DrawRange();
            Defend(gameTime);

            base.Update(gameTime);
        }

        private void DrawRange()
        {
            if (!_shouldDrawRange)
            {
                return;
            }
            _shouldDrawRange = false;

            Vector2 tileSize = new Vector2(Map.TileSize, Map.TileSize);
            // upper right, tile start.
            Vector2 position = Position + _sizeOffset;
            // center of tile.
            position += _sizeOffset;
            // center of tile - range/radius * tileSize.
            position -= Range * tileSize;
            World.Renderer.DrawImage(_rangeTexture, position, (int)(Range * 2 * Map.TileSize), (int)(Range * 2 * Map.TileSize), Layer.Last);
        }

        public bool ShouldDrawRange(Point mousePosition)
        {
            var min = Position + _sizeOffset;
            var max = min + new Vector2(Map.TileSize, Map.TileSize);

            if (mousePosition.X >= min.X && mousePosition.Y >= min.Y &&
                mousePosition.X <= max.X && mousePosition.Y <= max.Y)
            {
                _shouldDrawRange = true;
                return true;
            }

            _shouldDrawRange = false;
            return false;
        }

        private void Defend(GameTime gameTime)
        {
            if (!_isDefending)
            {
                return;
            }

            if (gameTime.TotalGameTime.TotalMilliseconds < _lastFireTime)
            {
                return;
            }

            _lastFireTime = gameTime.TotalGameTime.TotalMilliseconds + FireRateInMilliSeconds + _random.Next(FireRateRandomMilliSeconds);

            if (World.Entities.OfType<Monster>().Any())
            {
                World.AddEntity(new Projectile(World, Position + _sizeOffset));
            }
        }
    }
}
