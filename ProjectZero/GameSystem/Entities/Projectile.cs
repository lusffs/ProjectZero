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
        private const float Speed = 120.0f;
        private const float Size = 4.0f;
        private const float PredictAheadSeconds = 0.5f;

        public Projectile(World world, Vector2 position) : base(null, world, isAnimation: false)
        {
            Position = position;
            Position.X += Size / -2.0f + Map.TileSize / 2.0f;    // offset x to middle of tile. will shoot out from top middle.
#if TRACK_MONSTER
            FindNearestMonsterAndSetVelocity();
#else
            FindNearestTrackAndSetVelocity();
#endif
        }

        private void FindNearestTrackAndSetVelocity()
        {
            var path = World.Path;
            if (path == null || path.Count < 1 || !World.Entities.OfType<Monster>().Any(x => x.IsVisible && x.Animation.IsPlaying))
            {
                // no active path or no active monster, so just die.
                World.RemoveEntity(this);
                Velocity = Vector2.Zero;
                return;
            }

            Point nearestPath = default(Point);
            double nearestPathDistanceSquared = double.MaxValue;
            foreach (var point in path)
            {
                double distanceSquared = (new Vector2(point.X * Map.TileSize, point.Y * Map.TileSize) - Position).LengthSquared();

                if (distanceSquared < nearestPathDistanceSquared)
                {
                    nearestPathDistanceSquared = distanceSquared;
                    nearestPath = point;
                }
            }

            // TODO:    play sound.
            //          this will fail if path segment is not extending through axial direction.
            //          ex. path segment (10, 5) -> (20, 5) and tower (5, 10), will pick (0, -1).
            Vector2 shootDirection = (new Vector2(nearestPath.X * Map.TileSize, nearestPath.Y * Map.TileSize) + new Vector2(Map.TileSize)) - Position;
            shootDirection.Normalize();
            // take closest axial direction.
            if (Math.Abs(shootDirection.X) > Math.Abs(shootDirection.Y))
            {
                shootDirection.Y = 0;
                shootDirection.X = 1.0f * Math.Sign(shootDirection.X);
            }
            else
            {
                shootDirection.X = 0;
                shootDirection.Y = 1.0f * Math.Sign(shootDirection.Y);
            }
            Velocity = shootDirection * Speed;
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
            // TODO:   if out of map bounds, remove this.

            if (MonsterHit(gameTime))
            {
                return;
            }
            World.Renderer.FillRect(new Rectangle((int)Position.X, (int)Position.Y, (int)Size, (int)Size), Color.Black, RenderSystem.Layer.Dynamic);
            Position = Position + Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        private bool MonsterHit(GameTime gameTime)
        {
            Monster nearestMonsterHit = null;
            double nearestMonsterHitDistanceSquared = double.MaxValue;
            var nextPosition = Position = Position + Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (var monster in World.Entities.OfType<Monster>().Where(x => x.IsVisible && x.Animation.IsPlaying))
            {
                var monsterOffset = new Vector2(Map.TileSize * (Map.TileSize / (float)monster.Animation.TileSize));
                //return (abs(a.x - b.x) * 2 < (a.width + b.width)) && (abs(a.y - b.y) * 2 < (a.height + b.height));
                if (Math.Abs(nextPosition.X - monster.Position.X + monsterOffset.X) * 2.0f <= (Size + monster.Animation.TileSize) &&
                    Math.Abs(nextPosition.Y - monster.Position.Y + monsterOffset.Y) * 2.0f <= (Size + monster.Animation.TileSize))
                {
                    double distanceSquared = (monster.Position - nextPosition).LengthSquared();

                    if (distanceSquared < nearestMonsterHitDistanceSquared)
                    {
                        nearestMonsterHitDistanceSquared = distanceSquared;
                        nearestMonsterHit = monster;
                    }
                }                
            }

            if (nearestMonsterHit == null)
            {
                return false;
            }

            nearestMonsterHit.Die();
            // TODO:    should update player score here.
            //          uncomment remove, only for testing impact point.
            //World.RemoveEntity(this);
            Velocity = Vector2.Zero;

            return true;
        }
    }
}
