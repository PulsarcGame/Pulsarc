using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.UI.Common;
using Pulsarc.UI.Screens.Editor.UI;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Utils;
using System.Collections.Generic;

namespace Pulsarc.UI.Screens.Editor
{
    public class DebugEditor : ACEEditor
    {
        int lastScrollValue = 0;

        Vector2 beatCircleCenter = AnchorUtil.FindScreenPosition(Anchor.Center);

        public Crosshair Crosshair { get; protected set; }

        List<BeatCircle> circles = new List<BeatCircle>()
        {
            new BeatCircle(Beat.Whole, 1, 1),
            //new BeatCircle(Beat.Half, 1, 1),
            //new BeatCircle(Beat.Third, 1, 1),
            //new BeatCircle(Beat.Fourth, 1, 1),
            //new BeatCircle(Beat.Sixth, 1, 1),
            //new BeatCircle(Beat.Eighth, 1, 1),
            //new BeatCircle(Beat.Twelveth, 1, 1),
            //new BeatCircle(Beat.Sixteenth, 1, 1),
        };

        public DebugEditor() => Background = new Background("menu_background");

        protected ACEEditorView CreateEditorView()
        {
            return null;//new DebugEditorView(this);
        }

        public override void Update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();

            if (ms.ScrollWheelValue < lastScrollValue)
            {
                foreach (BeatCircle circle in circles)
                {
                    circle.Resize(circle.currentSize.X - 10);
                }
            }
            else if (ms.ScrollWheelValue > lastScrollValue)
            {
                foreach (BeatCircle circle in circles)
                {
                    circle.Resize(circle.currentSize.X + 10);
                }
            }

            lastScrollValue = ms.ScrollWheelValue;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (BeatCircle circle in circles)
            {
                circle.Draw();
            }
        }
    }
}
