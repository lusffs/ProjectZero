using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectZero.Framework;

namespace ProjectZero.GameSystem.Entities
{
    public class MagicTower : Tower
    {
        private Animation _magicAnimation;

        public MagicTower(World world, int price) : base(world, price)
        {
            _magicAnimation = new Animation(World.Renderer, World.SoundRenderer, "magictower", singleDirection: true);
            _magicAnimation.Play();
        }

        public override void RegisterContent()
        {
            base.RegisterContent();
            _magicAnimation.RegisterContent();
        }

        public override void ContentLoaded()
        {
            base.ContentLoaded();
            _magicAnimation.ContentLoaded();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _magicAnimation.Update(Position, gameTime, RenderSystem.Layer.Dynamic);
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
                _sizeOffset = _sizeOffset,
                _magicAnimation = _magicAnimation
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
