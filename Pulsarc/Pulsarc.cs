using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.UI.Screens.Gameplay.UI;
using Pulsarc.Utils;
using Pulsarc.Utils.BeatmapConversion;
using System.Diagnostics;
using Wobble.Platform;
using Wobble.Screens;

namespace Pulsarc
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Pulsarc : Game
    {
        static public GraphicsDeviceManager graphics;
        static public SpriteBatch spriteBatch;
        static public int xBaseRes = 1920;

        // for playtesting
        static public string toPlayFolder = "0 - Unknown - siqlo - Vantablack (Intralism)";
        static public string toPlaydiff = "Unknown - siqlo - Vantablack [Converted] (Intralism)";

        static public string convertFrom = "Mania";
        static public string toConvert = @"E:\osu!\Songs\682489 Between the Buried and Me - The Parallax II Future Sequence";

        //temp
        Stopwatch fpsWatch;
        FPS fpsDisplay;
        int fpsResolution;
        static public int frames;
        bool converting = false;

        public Pulsarc()
        {
            NativeAssemblies.Copy();

            graphics = new GraphicsDeviceManager(this);

            // Set the game in fullscreen (according to the user monitor)
            // TODO : Read from config file for user preference
            graphics.PreferredBackBufferHeight =(int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 1.2f);
            graphics.PreferredBackBufferWidth = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 1.2f);
            //graphics.IsFullScreen = true;
            graphics.SynchronizeWithVerticalRetrace = false;
            base.IsFixedTimeStep = false;

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

            KeyboardInputManager.StartThread();

            // Fps
            fpsDisplay = new FPS(new Vector2());
            fpsResolution = 10;
            fpsWatch = new Stopwatch();
            fpsWatch.Start();
            frames = 0;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            AssetsManager.Initialize(Content);

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

            // Move all this in an input handler
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !GameplayEngine.active)
            {
                GameplayEngine gameplay = new GameplayEngine();
                ScreenManager.AddScreen(gameplay);
                gameplay.Init(toPlayFolder, toPlaydiff);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S) && !converting && !GameplayEngine.active)
            {
                converting = true;
                BeatmapConverter converter;

                switch(convertFrom.ToLower()) 
                { 
                    case "mania":
                        converter = new ManiaToPulsarc();
                        break;
                    default:
                        converter = new IntralismToPulsarc();
                        break;
                }

                converter.Save(toConvert);
            }
            if(Keyboard.GetState().IsKeyUp(Keys.S) && converting)
            {
                converting = false;
            }

            ScreenManager.Update(gameTime);

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

            ScreenManager.Draw(gameTime);

            // FPS

            frames++;

            if (fpsWatch.ElapsedMilliseconds > 1000 / fpsResolution)
            {
                fpsDisplay.Update(frames * fpsResolution);
                frames = 0;
                fpsWatch.Restart();
            }

            fpsDisplay.Draw();

            base.Draw(gameTime);
            spriteBatch.End();
        }

        static public Vector2 getDimensions()
        {
            return new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
        }
    }
}
