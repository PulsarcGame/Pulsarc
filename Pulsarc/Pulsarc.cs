using IniParser;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pulsarc.UI;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.UI.Screens.Gameplay.UI;
using Pulsarc.UI.Screens.SongSelect;
using Pulsarc.Utils;
using Pulsarc.Utils.BeatmapConversion;
using System.Diagnostics;
using Wobble.Logging;
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

        static public bool display_cursor = true;

        Camera game_camera;
        Cursor cursor;
        

        //temp
        Stopwatch fpsWatch;
        FPS fpsDisplay;
        int fpsResolution;
        static public int frames;
        bool converting = false;

        public Pulsarc()
        {
            NativeAssemblies.Copy();
            Logger.Initialize();
            PulsarcDiscord.Initialize();
            Config.Initialize();

            graphics = new GraphicsDeviceManager(this);

            // Set the game in fullscreen (according to the user monitor)
            // TODO : Read from config file for user preference
            graphics.PreferredBackBufferHeight =(int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 1.2f);
            graphics.PreferredBackBufferWidth = (int) (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width  / 1.2f);
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
            
            game_camera = new Camera(graphics.GraphicsDevice.Viewport, (int) getDimensions().X, (int)getDimensions().Y, 1);
            game_camera.Pos = new Vector2(getDimensions().X / 2, getDimensions().Y / 2);

            SongSelection firstScreen = new SongSelection();
            ScreenManager.AddScreen(firstScreen);

            cursor = new Cursor();
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
            cursor.setPos(Mouse.GetState().Position);    

            if (!GameplayEngine.active && Keyboard.GetState().IsKeyDown(Keys.S) && ScreenManager.Screens.Peek().GetType().Name == "SongSelection")
            {
                converting = true;
                BeatmapConverter converter;

                string convertFrom = Config.get["Converting"]["Game"];
                string toConvert = Config.get["Converting"]["Path"];

                switch (convertFrom.ToLower()) 
                { 
                    case "mania":
                        converter = new ManiaToPulsarc();
                        break;
                    default:
                        converter = new IntralismToPulsarc();
                        break;
                }

                converter.Save(toConvert);
                ((SongSelection)ScreenManager.Screens.Peek()).RefreshBeatmaps();
            } else if(converting && Keyboard.GetState().IsKeyUp(Keys.S))
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
            spriteBatch.Begin(SpriteSortMode.Deferred,
                    null, null, null, null, null,
                    game_camera.GetTransformation());

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

            if(display_cursor)
                cursor.Draw();

            spriteBatch.End();
        }

        static public Vector2 getDimensions()
        {
            return new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
        }
    }
}
