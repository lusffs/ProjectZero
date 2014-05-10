using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectZero.Renderer;

namespace ProjectZero
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ProjectZeroGame : Game
    {
        private Renderer.Renderer _renderer;

        public ProjectZeroGame()
        {
            Content.RootDirectory = "";
            _renderer = new Renderer.Renderer(new GraphicsDeviceManager(this), Content);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            slimeImage = _renderer.RegisterTexture2D("Slime.png");

            _renderer.LoadContent(GraphicsDevice);
        }

        TextureHandle slimeImage;

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            _renderer.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            _renderer.ClearScreen(Color.AliceBlue);

            _renderer.DrawImage(slimeImage, new Vector2(100, 100));

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _renderer.RenderToScreen(gameTime);

            base.Draw(gameTime);
        }
    }
}
