using Microsoft.Xna.Framework;
using Pulsarc.UI.Screens.BaseEngine;
using Pulsarc.UI.Screens.Editor.UI;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Utils;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Editor
{
    public class ACEEditorView : ArcCrosshairEngineView
    {
        private ACEEditor GetEditor() => (ACEEditor)Screen;

        public ACEEditorView(Screen screen) : base(screen) { }

        public override void Destroy()
        {
            throw new System.NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            // Don't bother drawing the background if dim is 100%
            if ((Background.Dimmed && Background.DimTexture.Opacity != 0f) || !Background.Dimmed)
            {
                Background.Draw();
            }

            Crosshair.Draw();
            DrawBeatCircles();
            DrawArcs();
        }

        private void DrawBeatCircles()
        {
            for (int i = 0; i < GetEditor().BeatCircles.Count; i++)
            {
                BeatCircle beatCircle = GetEditor().BeatCircles[i];

                if (beatCircle.IsSeen())
                {
                    beatCircle.Draw();
                }

                double speed = GetEngine().CurrentSpeedMultiplier;
                float zLocation = GetEngine().GetCrosshair().GetZLocation();

                if (beatCircle.IsSeenAt(speed, zLocation) - GetEngine().IgnoreTime
                    > GetEngine().GetCurrentTime())
                {
                    break;
                }
            }
        }
    }
}
