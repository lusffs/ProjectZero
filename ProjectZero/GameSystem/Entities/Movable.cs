using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectZero.GameSystem.Entities
{
    public class Movable : SpriteEntity
    {
        public Vector2 Velocity;

        public Movable(string assetFileName, World world, bool isAnimation = true) : base(assetFileName, world, isAnimation)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Position = Position + Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
