using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace Pulsarc.Utils
{
    public static class Screenshotter
    {
        private static Keys screenshotKey = Config.Bindings["Screenshot"];

        private static bool pressedAlready = false;

        public static void Update()
        {
            // If screenshotKey is pressed, take a screenshot
            if (InputManager.PressedKeys.Contains(screenshotKey))
            {
                try
                {
                    if (!pressedAlready)
                    {
                        TakeScreenshot();
                    }
                }
                catch (Exception e)
                {
                    PulsarcLogger.Warning(e.ToString());
                }

                pressedAlready = true;
            }
            else if (pressedAlready)
            {
                pressedAlready = false;
            }
        }

        public static void TakeScreenshot()
        {
            Texture2D screenTexture = new Texture2D(
                Pulsarc.GraphicsDevice,
                Pulsarc.GraphicsDevice.PresentationParameters.BackBufferWidth,
                Pulsarc.GraphicsDevice.PresentationParameters.BackBufferHeight,
                true,
                Pulsarc.GraphicsDevice.PresentationParameters.BackBufferFormat);
            
            Rectangle bounds = screenTexture.Bounds;

            Color[] data = new Color[bounds.Width * bounds.Height];

            screenTexture.GetData(0, bounds, data, 0, data.Length);

            Pulsarc.GraphicsDevice.GetBackBufferData(data);

            screenTexture.SetData(data);

            string name = FindName();

            // If the game doesn't hae a screenshots folder for any reason, make it.
            // TODO: Custom screenshot directory?
            if (!Directory.Exists($"Screenshots/"))
            {
                Directory.CreateDirectory($"Screenshots/");
            }

            using (FileStream fileStream = File.Create($"Screenshots/{name}.png", 4096))
            {
                screenTexture.SaveAsPng(fileStream, bounds.Width, bounds.Height);
            }
        }

        /// <summary>
        /// Name the file *very* similarly to how steam formats its screenshots
        /// </summary>
        private static string FindName()
        {
            string baseName = DateTime.Now.ToString("yyMMddHHmmss");
            int suffix = 0;
            string fullName = baseName + $"-{suffix}";

            // If there's already a file with this name, change the suffix of the file name
            while (File.Exists($"Screenshots/{fullName}"))
            {
                suffix++;

                fullName = baseName + $"_{suffix}";
            }

            return fullName;
        }
    }
}
