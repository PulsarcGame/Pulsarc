using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Beatmaps;
using Pulsarc.Beatmaps.Events;
using Pulsarc.UI.Common;
using Pulsarc.UI.Screens.Editor.UI;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Utils;
using System.Collections.Generic;
using Wobble.Logging;

namespace Pulsarc.UI.Screens.Editor
{
    public class DebugEditor : EditorEngine
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

        public DebugEditor(Beatmap beatmap) : base(beatmap)
        {
            Background = new Background("menu_background");
        }

        public override void Update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();

            if (ms.ScrollWheelValue < lastScrollValue)
                foreach (BeatCircle circle in circles)
                    circle.Resize(circle.currentSize.X - 10);
            else if (ms.ScrollWheelValue > lastScrollValue)
                foreach (BeatCircle circle in circles)
                    circle.Resize(circle.currentSize.X + 10);

            lastScrollValue = ms.ScrollWheelValue;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (BeatCircle circle in circles)
                circle.Draw();
        }

        public override void AddEvent(Event evnt)
        {
            throw new System.NotImplementedException();
        }

        public override void AddEvent(int time, EventType eventType)
        {
            throw new System.NotImplementedException();
        }

        public override void AddHitObject(IEditorHitObject hitObject)
        {
            throw new System.NotImplementedException();
        }

        public override void AddHitObject(int time)
        {
            throw new System.NotImplementedException();
        }

        public override void AddTimingPoint(TimingPoint timingPoint)
        {
            throw new System.NotImplementedException();
        }

        public override void AddTimingPoint(int time, double bpm)
        {
            throw new System.NotImplementedException();
        }

        public override void AddZoomEvent(ZoomEvent zoomEvent)
        {
            throw new System.NotImplementedException();
        }

        public override void AddZoomEvent(int time, ZoomType zoomType, float zoomLevel, int endTime)
        {
            throw new System.NotImplementedException();
        }

        protected override void CreateColumns()
        {
            throw new System.NotImplementedException();
        }

        public override bool HasCrosshair()
        {
            return true;
        }

        public override Crosshair GetCrosshair()
        {
            return Crosshair;
        }
    }
}
