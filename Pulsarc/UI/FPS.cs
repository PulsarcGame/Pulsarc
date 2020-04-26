using Microsoft.Xna.Framework;
using Pulsarc.UI.Screens.Gameplay.UI;
using Pulsarc.Utils;
using System.Diagnostics;

namespace Pulsarc.UI
{
    public class FPS : TextDisplayElementFixedSize
    {
        private double lastRestartTime;

        // How many times per second the FPS counter updates.
        // TODO? Make FPSResolution customizeable by player
        private static int updatesPerSecond = 10;

        // How many frames have passed since the last update
        private static int frames = -1;

        /// <summary>
        /// A little tracker in the corner of the screen that gives an
        /// approximation of how many frames passed each second.
        /// </summary>
        /// <param name="position">The position of the FPS counter.</param>
        /// <param name="fontSize">The size of the FPS counter.</param>
        /// <param name="anchor">Anchor position for the FPS counter.</param>
        public FPS(Vector2 position, int fontSize = 14, Anchor anchor = Anchor.TopLeft)
            : base("", position, "fps", fontSize, anchor)
        { }

        public override void Draw()
        {
            frames++;

            if (PulsarcTime.CurrentElapsedTime > lastRestartTime + 1000 / updatesPerSecond)
            {
                Update(frames * updatesPerSecond);

                lastRestartTime = PulsarcTime.CurrentElapsedTime;
                frames = -1;
            }

            base.Draw();
        }
    }
}
