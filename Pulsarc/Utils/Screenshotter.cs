using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Pulsarc.Utils
{
    public static class Screenshotter
    {
        public static Keys ScreenshotKey => Config.Bindings["Screenshot"];

        public static bool ReadyToScreenshot { get; private set; } = false;
        public static bool Screenshotting { get; private set; } = false;

        public static async void Update()
        {
            if (InputManager.PressActions.Count <= 0) { return; }

            bool pressingKey = InputManager.PressedKeys.Contains(ScreenshotKey);

            // If pressed, get ready to screenshot
            if (pressingKey && !ReadyToScreenshot)
            {
                ReadyToScreenshot = true;
            }
            // If ready to screenshot and the key is released, screenshot!
            else if (!pressingKey && ReadyToScreenshot)
            {
                Screenshotting = true;
                TakeScreenshot();
                Screenshotting = false;
                
                // Make sure we don't accidentally rapid-fire screenshots from holding.
                ReadyToScreenshot = false;
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

            while (File.Exists($"Screenshots/{fullName}"))
            {
                suffix++;

                fullName = baseName + $"_{suffix}";
            }

            return fullName;
        }
    }
}
