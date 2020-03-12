using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Beatmaps;
using Pulsarc.Skinning;
using Pulsarc.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Editor.UI
{
    public class TimingPointCircle : BeatCircle
    {
        private TimingPoint timingPoint;

        public override int Time => timingPoint.Time;

        public TimingPointCircle(TimingPoint timingPoint, double scale)
            : base(Beat.TimingPoint, timingPoint.Time, scale) => this.timingPoint = timingPoint;

        public TimingPointCircle(int time, double bpm, double scale)
            : this(new TimingPoint(time, bpm), scale) { }
    }
}
