using Microsoft.Xna.Framework;
using Pulsarc.UI.Screens.Gameplay.UI;
using System.Diagnostics;

namespace Pulsarc.UI
{
    public class FPS : TextDisplayElementFixedSize
    {
        private Stopwatch fpsWatch;

        private static int fpsResolution;
        private static int frames = 0;

        /// <summary>
        /// A little tracker in the corner of the screen that gives an
        /// approximation of how many frames passed each second.
        /// </summary>
        /// <param name="position">The position of the FPS counter.</param>
        /// <param name="fontSize">The size of the FPS counter.</param>
        /// <param name="anchor">Anchor position for the FPS counter.</param>
        public FPS(Vector2 position, int fontSize = 14, Anchor anchor = Anchor.TopLeft)
            : base("", position, "fps", fontSize, anchor)
        {
            // TODO? Make FPSResolution customizeable by player
            fpsResolution = 10;

            fpsWatch = new Stopwatch();
            fpsWatch.Start();
        }

        public override void Draw()
        {
            frames++;

            if (fpsWatch.ElapsedMilliseconds > 1000 / fpsResolution)
            {
                Update(frames * fpsResolution);

                frames = 0;
                fpsWatch.Restart();
            }

            base.Draw();
        }
    }
}
