using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pulsarc.UI;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.UI.Screens.Gameplay.UI;
using Pulsarc.UI.Screens.MainMenu;
using Pulsarc.UI.Screens.SongSelect;
using Pulsarc.Utils;
using Pulsarc.Utils.BeatmapConversion;
using Pulsarc.Utils.SQLite;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Wobble.Input;
using Wobble.Logging;
using Wobble.Platform;
using Wobble.Screens;

namespace Pulsarc
{
    /// <summary>
    /// Main game object
    /// </summary>
    public class Pulsarc : Game
    {
        static public Pulsarc pulsarc;
        static public GraphicsDeviceManager graphics;
        static public SpriteBatch spriteBatch;

        // Width and Height used for reference in making the game responsive
        static public int xBaseRes = 1920;
        static public int yBaseRes = 1080;
        static public float baseRatio = 16/9f;

        // Current Width and Height of the game
        static public int currentWidth;
        static public int currentHeight;
        static public float currentAspectRatio;

        // Whether or not the in-game cursor is displayed 
        static public bool display_cursor = true;
        static public Cursor cursor;

        // The camera controlling the game's viewport
        Camera game_camera;

        // Static song selection screen for playing and managing user audio everywhere
        static public SongSelection songScreen;
        

        // All of these should be brought to other classes
        Stopwatch fpsWatch;
        FPS fpsDisplay;
        int fpsResolution;
        static public int frames;
        bool converting = false;

        public Pulsarc()
        {
            pulsarc = this;

            // Set up stored Data access
            DataManager.Initialize();

            // Load user config
            Config.Initialize();

            // Set default resolution if not set, and fullscreen when at least one isn't set.
            if (Config.getInt("Graphics", "ResolutionWidth") <= 0)
            {
                Config.setInt("Graphics","ResolutionWidth",GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width);
                Config.setInt("Graphics", "FullScreen", 1);
            }
            if (Config.getInt("Graphics", "ResolutionHeight") <= 0)
            {
                Config.setInt("Graphics", "ResolutionHeight", GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
                Config.setInt("Graphics", "FullScreen", 1);
            }

            currentWidth = Config.getInt("Graphics", "ResolutionWidth");
            currentHeight = Config.getInt("Graphics", "ResolutionHeight");
            currentAspectRatio = currentWidth / (float)currentHeight;

            // Create the game's application window
            graphics = new GraphicsDeviceManager(this);

            // Set Graphics preferences according to config.ini
            graphics.PreferredBackBufferWidth = Config.getInt("Graphics", "ResolutionWidth");
            graphics.PreferredBackBufferHeight = Config.getInt("Graphics", "ResolutionHeight");
            graphics.IsFullScreen = Config.getInt("Graphics", "FullScreen") == 1;
            graphics.SynchronizeWithVerticalRetrace = Config.getInt("Graphics", "VSync") == 1;

            IsFixedTimeStep = Config.getInt("Graphics", "FPSLimit") != 0;  //Force the game to update at fixed time intervals
            if (IsFixedTimeStep)
            {
                TargetElapsedTime = TimeSpan.FromSeconds(1 / (float) Config.getInt("Graphics", "FPSLimit"));  //Set the time interval to 1/30th of a second
            }

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Initialize most static objects and dependencies
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Copy, if needed, the required assemblies (BASS) for 32 or 64bit CPUs
            NativeAssemblies.Copy();

            // Initialize the logging tool for troubleshooting
            Logger.Initialize();

            // Initialize Discord Rich Presence
            PulsarcDiscord.Initialize();

            // Set the default culture (Font formatting Locals) for this thread
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // Start the thread listening for user input
            InputManager.StartThread();

            // Fps
            fpsDisplay = new FPS(new Vector2());
            fpsResolution = 10;
            fpsWatch = new Stopwatch();
            fpsWatch.Start();
            frames = 0;

            // Start our time and frame-time tracker
            PulsarcTime.Start();
            
            // Initialize the game camera
            game_camera = new Camera(graphics.GraphicsDevice.Viewport, (int) getDimensions().X, (int)getDimensions().Y, 1);
            game_camera.Pos = new Vector2(getDimensions().X / 2, getDimensions().Y / 2);

            // Start the song selection in the background to have music when entering the game
            songScreen = new SongSelection();
            songScreen.Init();

            // Create and display the default game screen
            // (Currently Main menu. In the future can implement an intro)
            Menu firstScreen = new Menu();
            ScreenManager.AddScreen(firstScreen);

            cursor = new Cursor();
        }

        /// <summary>
        /// Initialize and load all game compiled content
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize and load all game compiled content
            AssetsManager.Initialize(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            AssetsManager.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            PulsarcTime.Update();

            cursor.setPos(MouseManager.CurrentState.Position);

            // Temporary measure for converting intralism or osu!mania beatmaps
            if (!GameplayEngine.active && Keyboard.GetState().IsKeyDown(Config.bindings["Convert"]) && ScreenManager.Screens.Peek().GetType().Name == "SongSelection")
            {
                converting = true;
                BeatmapConverter converter;

                Config.Reload();
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
                ((SongSelection)ScreenManager.Screens.Peek()).rescanBeatmaps();
            } else if(converting && Keyboard.GetState().IsKeyUp(Config.bindings["Convert"]))
            {
                converting = false;
            }

            // Let ScreenManager handle the updating of the current active screen
            ScreenManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Begin the spritebatch in relation to the camera
            spriteBatch.Begin(SpriteSortMode.Deferred,
                    null, null, null, null, null,
                    game_camera.GetTransformation());

            GraphicsDevice.Clear(Color.Black);

            // Let ScreenManager handle the drawing of the current active screen
            ScreenManager.Draw(gameTime);

            // FPS
            drawFPSCounter();

            base.Draw(gameTime);

            if(display_cursor)
                cursor.Draw();

            spriteBatch.End();
        }

        private void drawFPSCounter()
        {
            frames++;

            if (fpsWatch.ElapsedMilliseconds > 1000 / fpsResolution)
            {
                fpsDisplay.Update(frames * fpsResolution);
                frames = 0;
                fpsWatch.Restart();
            }

            fpsDisplay.Draw();
        }

        /// <summary>
        /// Used for getting the game's dimensions in a Vector2 object
        /// </summary>
        static public Vector2 getDimensions()
        {
            return new Vector2(currentWidth, currentHeight);
        }

        /// <summary>
        /// Used for getting the game's base screen dimensions in a Vector2 Object
        /// </summary>
        static public Vector2 getBaseScreenDimensions()
        {
            return new Vector2(xBaseRes, yBaseRes);
        }

        static public void Quit()
        {
            pulsarc.Exit();
        }
    }
}
