using System.Diagnostics;
using Microsoft.Xna.Framework;
using Pulsarc.UI.Screens.Gameplay.UI;

namespace Pulsarc.UI
{
    public class Fps : TextDisplayElementFixedSize
    {
        private Stopwatch fpsWatch;

        private static int _fpsResolution;
        private static int _frames;

        /// <summary>
        /// A little tracker in the corner of the screen that gives an
        /// approximation of how many frames passed each second.
        /// </summary>
        /// <param name="position">The position of the FPS counter.</param>
        /// <param name="fontSize">The size of the FPS counter.</param>
        /// <param name="anchor">Anchor position for the FPS counter.</param>
        public Fps(Vector2 position, int fontSize = 14, Anchor anchor = Anchor.TopLeft)
            : base("", position, "fps", fontSize, anchor)
        {
            // TODO? Make FPSResolution customizeable by player
            _fpsResolution = 10;

            fpsWatch = new Stopwatch();
            fpsWatch.Start();
        }

        public override void Draw()
        {
            _frames++;

            if (fpsWatch.ElapsedMilliseconds > 1000 / _fpsResolution)
            {
                Update(_frames * _fpsResolution);

                _frames = 0;
                fpsWatch.Restart();
            }

            base.Draw();
        }
    }
}
