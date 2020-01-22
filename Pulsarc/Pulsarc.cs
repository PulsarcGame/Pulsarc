using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pulsarc.UI;
using Pulsarc.UI.Screens;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.UI.Screens.MainMenu;
using Pulsarc.UI.Screens.SongSelect;
using Pulsarc.Utils;
using Pulsarc.Utils.BeatmapConversion;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Wobble;
using Wobble.Input;
using Wobble.Platform;
using Wobble.Screens;

namespace Pulsarc
{
    /// <summary>
    /// Main game object
    /// </summary>
    public class Pulsarc : WobbleGame
    {
        private static Pulsarc pulsarc;
        public static SpriteBatch SpriteBatch => GameBase.Game.SpriteBatch;
        public static GraphicsDeviceManager Graphics => GameBase.Game.Graphics;

        // Whether or not the Cursor is shown.
        public static bool DisplayCursor { get; set; } = true;
        private Cursor cursor;

        // Base Dimensions
        public const int BASE_WIDTH = 1920;
        public const int BASE_HEIGHT = 1080;
        public const float BASE_ASPECT_RATIO = 16f / 9f;

        // Current Dimensions
        public static int CurrentWidth => Config.GetInt("Graphics", "ResolutionWidth");
        public static int CurrentHeight => Config.GetInt("Graphics", "ResolutionHeight");
        public static float CurrentAspectRatio => (float)CurrentWidth / CurrentHeight;
        public static Rectangle ScreenSpace => new Rectangle(0, 0, CurrentWidth, CurrentHeight);

        // Scales for moving/resizing Drawables accurately on different dimensions.
        public static float HeightScale => (float)CurrentHeight / BASE_HEIGHT;
        public static float WidthScale => (float)CurrentWidth / BASE_WIDTH;

        // The camera controlling the game's viewport
        private Camera gameCamera;

        // Static song selection screen for playing and managing user audio everywhere
        public static SongSelection SongScreen;

        // Whether or not the cursor should be displayed
        private bool displayCursor => Pulsarc.DisplayCursor;

        // FPS
        private FPS fpsDisplay;

        // Map Converting Flag
        private bool converting = false;

        private PulsarcScreen lastScreen = null;

        protected override bool IsReadyToUpdate { get; set; }

        public Pulsarc()
        {
            pulsarc = this;

            // Set up stored Data access
            DataManager.Initialize();

            // Load user config
            Config.Initialize();

            // Set default resolution if not set, and fullscreen when at least one isn't set.
            if (Config.GetInt("Graphics", "ResolutionWidth") <= 0)
            {
                Config.SetInt("Graphics", "ResolutionWidth", GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width);
                Config.SetInt("Graphics", "FullScreen", 2);
            }

            bool heightIsZero = false;
            if (Config.GetInt("Graphics", "ResolutionHeight") <= 0)
            {
                Config.SetInt("Graphics", "ResolutionHeight", GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
                Config.SetInt("Graphics", "FullScreen", 2);
            }

            // Set Graphics preferences according to config.ini
            Graphics.PreferredBackBufferWidth = Config.GetInt("Graphics", "ResolutionWidth");
            Graphics.PreferredBackBufferHeight = Config.GetInt("Graphics", "ResolutionHeight");

            Graphics.IsFullScreen = Config.GetInt("Graphics", "FullScreen") >= 1;
            Window.IsBorderless = Config.GetInt("Graphics", "FullScreen") >= 2;

            Graphics.SynchronizeWithVerticalRetrace = Config.GetInt("Graphics", "VSync") == 1;

            // Force the game to update at fixed time intervals
            IsFixedTimeStep = Config.GetInt("Graphics", "FPSLimit") != 0;

            // Set the time interval to match the FPSLimit
            if (IsFixedTimeStep)
                TargetElapsedTime = TimeSpan.FromSeconds(1 / (float)Config.GetInt("Graphics", "FPSLimit"));

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
            PulsarcLogger.Logging = Config.GetBool("Logger", "AllMessages");
            PulsarcLogger.Initialize();

            // Initialize Discord Rich Presence
            PulsarcDiscord.Initialize();

            // Set the default culture (Font formatting Locals) for this thread
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // Start the thread listening for user input
            InputManager.StartThread();

            // Start our time and frame-time tracker
            PulsarcTime.Start();

            // Initialize FPS
            fpsDisplay = new FPS(Vector2.Zero);

            // Initialize the game camera
            gameCamera = new Camera(Graphics.GraphicsDevice.Viewport, (int)GetDimensions().X, (int)GetDimensions().Y, 1);
            gameCamera.Pos = new Vector2(GetDimensions().X / 2, GetDimensions().Y / 2);

            // Start the song selection in the background to have music when entering the game
            SongScreen = new SongSelection();
            SongScreen.Init();

            // Create and display the default game screen
            // (Currently Main menu. In the future can implement an intro)
            Menu firstScreen = new Menu();
            ScreenManager.AddScreen(firstScreen);

            cursor = new Cursor();

            IsReadyToUpdate = true;
        }

        /// <summary>
        /// Initialize and load all game compiled content
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

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
            if (!IsReadyToUpdate) return;

            Thread.Yield();

            PulsarcTime.Update();

            CheckScreen();

            cursor.SetPos(MouseManager.CurrentState.Position);

            ConvertMaps();

            // Let ScreenManager handle the updating of the current active screen
            ScreenManager.Update(gameTime);

            base.Update(gameTime);
        }

        private void CheckScreen()
        {
            if (ScreenManager.Screens.Peek() != lastScreen)
            {
                PulsarcScreen currentScreen = (PulsarcScreen)ScreenManager.Screens.Peek();
                lastScreen = currentScreen;

                currentScreen.EnteredScreen();
            }
        }

        private async void ConvertMaps()
        {
            // Temporary measure for converting intralism or osu!mania beatmaps
            // TODO Make a Converter UI
            if (!converting && !GameplayEngine.Active
                && Keyboard.GetState().IsKeyDown(Config.Bindings["Convert"])
                && ScreenManager.Screens.Peek().GetType().Name == "SongSelection")
            {
                converting = true;
                IBeatmapConverter converter;

                Config.Reload();
                string convertFrom = Config.Get["Converting"]["Game"];
                string toConvert = Config.Get["Converting"]["Path"];

                // What extension to check for
                string extension;

                switch (convertFrom.ToLower())
                {
                    case "mania":
                        converter = new ManiaToPulsarc();
                        extension = "*.osu";
                        break;
                    default:
                        converter = new IntralismToPulsarc();
                        extension = "config.txt";
                        break;
                }

                // Get all subfolder names 
                string[] directories = Directory.GetDirectories(toConvert);

                // If there is no maps in this folder, and the number of sub-folders is 2 or more,
                // do a Batch Convert
                if (Directory.GetFiles(toConvert, extension).Length == 0 && directories.Length >= 2)
                    foreach (string directory in directories)
                        converter.Save(directory);

                // Otherwise convert one map
                else
                    converter.Save(toConvert);

                ((SongSelection)ScreenManager.Screens.Peek()).RescanBeatmaps();
            }
            else if (converting && Keyboard.GetState().IsKeyUp(Config.Bindings["Convert"]))
                converting = false;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (!IsReadyToUpdate) return;

            // Begin the spritebatch in relation to the camera
            SpriteBatch.Begin(SpriteSortMode.Deferred,
                    null, null, null, null, null,
                    gameCamera.GetTransformation());

            GraphicsDevice.Clear(Color.Black);

            // Let ScreenManager handle the drawing of the current active screen
            ScreenManager.Draw(gameTime);

            // FPS
            if (fpsDisplay != null)
                fpsDisplay.Draw();

            base.Draw(gameTime);

            if (displayCursor)
                cursor.Draw();

            SpriteBatch.End();
        }

        /// <summary>
        /// Used for getting the game's base screen dimensions currently (1920x1080)
        /// in a Vector2 Object
        /// </summary>
        static public Vector2 GetBaseScreenDimensions()
        {
            return new Vector2(BASE_WIDTH, BASE_HEIGHT);
        }

        /// <summary>
        /// Sees if the current aspect ratio is wider than 16:9.
        /// </summary>
        /// <returns></returns>
        static public bool IsPulsarcWiderThan16by9()
        {
            return CurrentAspectRatio > BASE_ASPECT_RATIO;
        }

        /// <summary>
        /// Used for getting the game's current dimensions in a Vector2 object
        /// </summary>
        static public Vector2 GetDimensions()
        {
            return new Vector2(CurrentWidth, CurrentHeight);
        }

        static public void Quit()
        {
            pulsarc.Exit();
        }
    }
}
