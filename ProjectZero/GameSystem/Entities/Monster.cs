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
            // var nextPos = Velocity * gameTime
            // if (nexPos >= path[currentPath])
            // {
            //      // i nästa frame ska detta ske.
            //      Velocity = ...
            //      Animation.Direct = ...
            //      currentPath++
            // }
            base.Update(gameTime);
        }

        public void WalkPath(List<Point> path, Vector2 startPosition)
        {
            _currentPathIndex = 0;
            float sizeFactor = (Map.TileSize * (Map.TileSize / (float)Animation.TileSize));
            Position.X = startPosition.X - sizeFactor;
            Position.Y = startPosition.Y - sizeFactor;
            var direction = new Vector2();
            direction.X = startPosition.X - (path[0].X * Map.TileSize);// - (Map.TileSize * (Map.TileSize / (float)Animation.TileSize));
            direction.Y = startPosition.Y + (path[0].Y * Map.TileSize);// - (Map.TileSize * (Map.TileSize / (float)Animation.TileSize));
            direction.Normalize();
            Velocity.X = direction.X * (World.Renderer.GraphicsDevice.Viewport.Width / 4f);
            Velocity.Y = direction.Y * (World.Renderer.GraphicsDevice.Viewport.Width / 4f);

            if (Position.X + sizeFactor >= path[0].X)
            {
                if (Position.Y + sizeFactor> path[0].Y)
                {
                    Animation.Direction = Framework.AnimationDirection.Up;
                }
                else if (Position.Y + sizeFactor < path[0].Y)
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
    }
}
