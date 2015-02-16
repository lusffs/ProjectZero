using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectZero.GameSystem.Entities
{
    public class BaseTower : SpriteEntity
    {
        protected Vector2 _sizeOffset;

        private readonly int _price;
        
        public BaseTower(World world, int price, string assetFileName)
            : base(assetFileName, world, isAnimation: false)
        {
            Solid = true;
            _price = price;
        }

        // TODO:_   move to sprite.
        public Vector2 SizeOffset
        {
            get
            {
                return _sizeOffset;
            }
        }

        public override void ContentLoaded()
        {
            base.ContentLoaded();
            _sizeOffset = new Vector2(Map.TileSize * (Map.TileSize / (float)Image.Width), Map.TileSize * (Map.TileSize / (float)Image.Height));
        }

        public override BaseEntity Clone(Vector2 position)
        {
            return new BaseTower(World, _price, AssetFileName)
            {
                Animation = Animation,
                AssetFileName = AssetFileName,
                Image = Image,
                Position = new Vector2(position.X * Map.TileSize, position.Y * Map.TileSize) - _sizeOffset,
                Solid = Solid,
                _sizeOffset = _sizeOffset
            };
        }

        public int Price
        {
            get
            {
                return _price;
            } 
        }
             
    }
}
