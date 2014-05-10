using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaInput = Microsoft.Xna.Framework.Input;
using ProjectZero.InputSystem;
using ProjectZero.RenderSystem;

namespace ProjectZero
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ProjectZeroGame : Game
    {
        private Renderer _renderer;
        private Input _input;
        private GameSystem.Game _game;

        public ProjectZeroGame()
        {
            Content.RootDirectory = "Content";
            _renderer = new Renderer(new GraphicsDeviceManager(this), Content);
            _input = new Input();
            _game = new GameSystem.Game(_renderer, _input);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _game.Initialize();
            base.Initialize();
        }

        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _game.RegisterContent();
            // this will actually load content.
            _renderer.LoadContent(GraphicsDevice);
        }

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
            // collect all input and handle it.
            _input.Frame(XnaInput.Keyboard.GetState(), XnaInput.Mouse.GetState());
            // run game frame for updating game state and generate new output.
            _game.Frame(gameTime);

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
