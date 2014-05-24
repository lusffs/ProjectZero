using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectZero.Framework;
using ProjectZero.RenderSystem;
using ProjectZero.SoundSystem;

namespace ProjectZero.GameSystem.Entities
{
    public class StaticEntity : BaseEntity
    {
        
        public StaticEntity(World world) : base(world)
        {            
        }

        public override BaseEntity Clone(Vector2 position)
        {
            return new StaticEntity(World) { Position = position, Solid = Solid };
        }
    }
}
