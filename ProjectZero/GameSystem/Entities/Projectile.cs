using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectZero.GameSystem.Entities
{
    public class Projectile : Movable
    {
        private const float Speed = 160.0f;
        private const float Size = 4.0f;
        private const float PredictAheadSeconds = 0.5f;

        public Projectile(World world, Vector2 position) : base(null, world, isAnimation: false)
        {
            Position = position;
            FindNearestMonsterAndSetVelocity();
        }

        private void FindNearestMonsterAndSetVelocity()
        {
            Monster nearestMonster = null;
            double nearestMonsterDistanceSquared = double.MaxValue;

            foreach (var monster in World.Entities.OfType<Monster>().Where(x => x.IsVisible && x.Animation.IsPlaying))
            {
                double distanceSquared = (monster.Position - Position).LengthSquared();

                if (distanceSquared < nearestMonsterDistanceSquared)
                {
                    nearestMonsterDistanceSquared = distanceSquared;
                    nearestMonster = monster;
                }
            }

            if (nearestMonster == null)
            {
                // no active monster, so just die.
                World.RemoveEntity(this);
                Velocity = Vector2.Zero;
                return;
            }

            // TODO:    should we always shoot in vert/horz straight lines? if so, then this need to change.
            //          play sound.
            float sizeFactor = (Map.TileSize * (Map.TileSize / (float)nearestMonster.Animation.TileSize));
            // sizeFactor + TileSize = center of image = tile center. also predict ahead for monster position and use that as target.
            Vector2 shootDirection = (nearestMonster.Position + new Vector2(sizeFactor + Map.TileSize) + nearestMonster.Velocity * PredictAheadSeconds) - Position;
            shootDirection.Normalize();
            Velocity = shootDirection * Speed;
        }

        public override void RegisterContent()
        {
            
        }

        public override void ContentLoaded()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            // TODO:    should check for collision with monster here at Position + Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds
            //          if so. remove monster(death animation?), add score and remove this.
            //          also. if out of map bounds, remove this.
            World.Renderer.FillRect(new Rectangle((int)Position.X, (int)Position.Y, (int)Size, (int)Size), Color.Black, RenderSystem.Layer.Dynamic);
            Position = Position + Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
