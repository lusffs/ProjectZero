using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectZero.GameSystem.Entities
{
    public class MagicTower : Tower
    {
        public MagicTower(World world, int price) : base(world, price)
        {
                
        }

        public override BaseEntity Clone(Vector2 position)
        {
            return new MagicTower(World, Price)
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

        protected override float FireRateInMilliSeconds
        {
            get
            {
                return base.FireRateInMilliSeconds * 0.7f;
            }
        }

        protected override float ProjectileSpeed
        {
            get
            {
                return base.ProjectileSpeed * 1.2f;
            }
        }
    }
}
