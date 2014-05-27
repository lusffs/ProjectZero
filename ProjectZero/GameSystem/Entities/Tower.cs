using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectZero.RenderSystem;

namespace ProjectZero.GameSystem.Entities
{
    public class Tower : SpriteEntity
    {
        private Vector2 _sizeOffset;
        private const float Range = 3;
        private bool _shouldDrawRange;
        private TextureHandle _rangeTexture;

        public Tower(World world) : base("images/tower.png", world, isAnimation: false)
        {
            Solid = true;
            _rangeTexture = World.Renderer.RegisterTexture2D("images/ui/radius.png");
        }

        public override void ContentLoaded()
        {
            base.ContentLoaded();
            _sizeOffset = new Vector2(Map.TileSize * (Map.TileSize / (float)Image.Width), Map.TileSize * (Map.TileSize / (float)Image.Height));
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
        
        public void StartDefending()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            DrawRange();
            
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
    }
}
