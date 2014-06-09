using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ProjectZero.GameSystem.Entities
{
    public class Monster : Movable
    {
        public Monster(string assetFileName, World world, bool isAnimation = true) : base(assetFileName, world, isAnimation)
        {

        }

        public override void ContentLoaded()
        {
            base.ContentLoaded();
            if (Animation.TileSize > Map.TileSize)
            {
                Position.X -= Map.TileSize * (Map.TileSize / (float)Animation.TileSize);
                Position.Y -= Map.TileSize * (Map.TileSize / (float)Animation.TileSize);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsVisible)
            {
                return;
            }

            bool targetWillBeHit = Animation.IsPlaying && _path != null && TargetWillBeHit(gameTime);
            base.Update(gameTime);
            if (targetWillBeHit && Animation.IsPlaying)
            {
                MoveTowardsNextTarget();
            }
        }

        private void MoveTowardsNextTarget()
        {
            _currentPathIndex++;
            float sizeFactor = (Map.TileSize * (Map.TileSize / (float)Animation.TileSize));
            Position.X = _path[_currentPathIndex - 1].X * Map.TileSize - sizeFactor;
            Position.Y = _path[_currentPathIndex - 1].Y * Map.TileSize - sizeFactor;

            var direction = new Vector2();
            direction.X = (_path[_currentPathIndex].X * Map.TileSize) - _path[_currentPathIndex - 1].X * Map.TileSize;// - (Map.TileSize * (Map.TileSize / (float)Animation.TileSize));
            direction.Y = (_path[_currentPathIndex].Y * Map.TileSize) - _path[_currentPathIndex - 1].Y * Map.TileSize;// - (Map.TileSize * (Map.TileSize / (float)Animation.TileSize));
            direction.Normalize();
            Velocity.X = direction.X * Speed;
            Velocity.Y = direction.Y * Speed;

            if (Position.X + sizeFactor >= _path[_currentPathIndex].X * Map.TileSize)
            {
                if (Position.Y + sizeFactor > _path[_currentPathIndex].Y * Map.TileSize)
                {
                    Animation.Direction = Framework.AnimationDirection.Up;
                }
                else if (Position.Y + sizeFactor < _path[_currentPathIndex].Y * Map.TileSize)
                {
                    Animation.Direction = Framework.AnimationDirection.Down;
                }
                else
                {
                    Animation.Direction = Framework.AnimationDirection.Left;
                }
            }
            else
            {
                Animation.Direction = Framework.AnimationDirection.Right;
            }
        }

        private bool TargetWillBeHit(GameTime gameTime)
        {
            float sizeFactor = (Map.TileSize * (Map.TileSize / (float)Animation.TileSize));
            var position = Position + new Vector2(sizeFactor);
            var nextPosition = position + Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds; 
            var target = new Vector2(_path[_currentPathIndex].X * Map.TileSize, _path[_currentPathIndex].Y * Map.TileSize);
            float t = (nextPosition - position).Length() / (target - position).Length();
            if (t < 1)
            {
                return false;
            }

            if (t > 1)
            {
                // TODO:    should clamp velocity here to exact target point. will be one frame where we
                //          draw beyond the target point.
            }

            if (_currentPathIndex + 1 == _path.Count)
            {
                // TODO:    remove this.
                Animation.Stop();
                Velocity = Vector2.Zero;
                return false;
            }            

            return true;            
        }

        private List<Point> _path = null;

        private const float Speed = 800f / 12f;
        
        public void WalkPath(List<Point> path, Vector2 startPosition)
        {
            _path = path;
            _currentPathIndex = 0;
            Animation.Play();

            float sizeFactor = (Map.TileSize * (Map.TileSize / (float)Animation.TileSize));
            Position.X = startPosition.X - sizeFactor;
            Position.Y = startPosition.Y - sizeFactor;
            var direction = new Vector2();
            direction.X = (path[0].X * Map.TileSize) - startPosition.X;// - (Map.TileSize * (Map.TileSize / (float)Animation.TileSize));
            direction.Y = (path[0].Y * Map.TileSize) - startPosition.Y;// - (Map.TileSize * (Map.TileSize / (float)Animation.TileSize));
            direction.Normalize();
            Velocity.X = direction.X * Speed;
            Velocity.Y = direction.Y * Speed;

            if (Position.X + sizeFactor >= path[0].X * Map.TileSize)
            {
                if (Position.Y + sizeFactor> path[0].Y * Map.TileSize)
                {
                    Animation.Direction = Framework.AnimationDirection.Up;
                }
                else if (Position.Y + sizeFactor < path[0].Y * Map.TileSize)
                {
                    Animation.Direction = Framework.AnimationDirection.Down;
                }
                else
                {
                    Animation.Direction = Framework.AnimationDirection.Left;
                }
            }
            else
            {
                Animation.Direction = Framework.AnimationDirection.Right;
            }
            
        }

        private int _currentPathIndex;

        public void Die()
        {
            // TODO:    play sound. death animation, then remove?
            World.RemoveEntity(this);
            Velocity = Vector2.Zero;
        }
    }
}
