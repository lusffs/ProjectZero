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

        public BaseTower(World world) : base("images/tower.png", world, isAnimation: false)
        {
            Solid = true;
        }

        public override void ContentLoaded()
        {
            base.ContentLoaded();
            _sizeOffset = new Vector2(Map.TileSize * (Map.TileSize / (float)Image.Width), Map.TileSize * (Map.TileSize / (float)Image.Height));
        }

        public override BaseEntity Clone(Vector2 position)
        {
            return new BaseTower(World)
            {
                Animation = Animation,
                AssetFileName = AssetFileName,
                Image = Image,
                Position = new Vector2(position.X * Map.TileSize, position.Y * Map.TileSize) - _sizeOffset,
                Solid = Solid,
                _sizeOffset = _sizeOffset
            };
        }

        public virtual int Price
        {
            get
            {
                return 13;
            } 
        }
             
    }
}
