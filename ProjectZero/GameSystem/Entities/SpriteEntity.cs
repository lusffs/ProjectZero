using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProjectZero.Framework;
using ProjectZero.RenderSystem;

namespace ProjectZero.GameSystem.Entities
{
    public class SpriteEntity : StaticEntity
    {
        public string AssetFileName { get; protected set; }

        public Animation Animation { get; protected set; }

        public TextureHandle Image { get; set; }

        public int Width
        {
            get
            {
                if (Animation != null)
                {
                    return Animation.TileSize;
                }
                return Image.Width;
            }
        }

        public int Height
        {
            get
            {
                if (Animation != null)
                {
                    return Animation.TileSize;
                }
                return Image.Height;
            }
        }

        public SpriteEntity(string assetFileName, World world, bool isAnimation = true) : base(world)
        {
            if (isAnimation)
            {
                AssetFileName = assetFileName;
                Animation = new Animation(World.Renderer, AssetFileName);
            }
            else
            {
                AssetFileName = assetFileName;
                Animation = null;
            }
        }


        public override void RegisterContent()
        {
            if (Animation != null)
            {
                Animation.RegisterContent();
            }
            else
            {
                Image = World.Renderer.RegisterTexture2D(AssetFileName);
            }
        }

        public override void ContentLoaded()
        {
            if (Animation == null)
            {
                return;
            }

            Animation.ContentLoaded();
            Animation.Play();
        }

        public override void Update(GameTime gameTime)
        {
            if (Animation != null)
            {
                Animation.Update(Position, gameTime);
            }
            else
            {
                World.Renderer.DrawImage(Image, Position);
            }
        }
    }
}
