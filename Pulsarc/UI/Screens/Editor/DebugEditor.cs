using Microsoft.Xna.Framework;
using Pulsarc.Beatmaps;
using Pulsarc.Beatmaps.Events;
using Pulsarc.UI.Screens.Editor.UI;
using Pulsarc.UI.Screens.Gameplay;
using System.Collections.Generic;

namespace Pulsarc.UI.Screens.Editor
{
    public class DebugEditor : EditorEngine
    {
        List<BeatCircle> circles = new List<BeatCircle>()
        {
            new BeatCircle(Beat.Whole, 1, 1),
            /*new BeatCircle(Beat.Half, 0, 1),
            new BeatCircle(Beat.Third, 0, 1),
            new BeatCircle(Beat.Fourth, 0, 1),
            new BeatCircle(Beat.Sixth, 0, 1),
            new BeatCircle(Beat.Eighth, 0, 1),
            new BeatCircle(Beat.Twelveth, 0, 1),*/
        };

        public DebugEditor(Beatmap beatmap) : base(beatmap)
        {
        }

        public override void Update(GameTime gameTime)
        {
            foreach (BeatCircle circle in circles)
                circle.RecalcPos(1, 1, new Crosshair().GetZLocation());
        }

        public override void Draw(GameTime gameTime)
        {
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
    }
}
