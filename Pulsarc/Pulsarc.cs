using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Gameplay;
using Pulsarc.Skinning;
using Pulsarc.Utils;

namespace Pulsarc
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Pulsarc : Game
    {
        static public GraphicsDeviceManager graphics;
        static public SpriteBatch spriteBatch;
        static public GameplayEngine gameplayEngine;


        public Pulsarc()
        {
            graphics = new GraphicsDeviceManager(this);

            // Set the game in fullscreen (according to the user monitor)
            // TODO : Read from config file for user preference
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.IsFullScreen = true;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            Skin.LoadSkin("DefaultSkin");

            KeyboardInputManager.StartThread();
            gameplayEngine = new GameplayEngine();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO : Load all menu images etc
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here (Skin)
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !gameplayEngine.isActive())
                gameplayEngine.Init("0 - Noone - Test (Adri)", "Noone - Test [test] (Adri)");

            if (Keyboard.GetState().IsKeyDown(Keys.Delete))
                gameplayEngine.Reset();

            if (gameplayEngine.isActive())
            {
                gameplayEngine.Update();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            GraphicsDevice.Clear(Color.Black);

            if (gameplayEngine.isActive())
            {
                gameplayEngine.Draw();
            }

            base.Draw(gameTime);
            spriteBatch.End();
        }

        static public Vector2 getDimensions()
        {
            return new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
        }
    }
}
