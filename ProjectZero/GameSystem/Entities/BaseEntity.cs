using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectZero.RenderSystem;
using ProjectZero.SoundSystem;

namespace ProjectZero.GameSystem.Entities
{
    public abstract class BaseEntity
    {
        public Vector2 Position;

        protected World World { get; private set; }

        public BaseEntity(World world)
        {
            World = world;
        }

        public virtual void RegisterContent()
        {
            
        }

        public virtual void ContentLoaded()
        {
            
        }

        public virtual void Update(GameTime gameTime)
        {
            
        }

        public bool Solid { get; set; }
    }
}
