using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Utils;

namespace Pulsarc.UI.Screens.Editor.UI
{
    public class EditorArcHitObject : HitObject, IEditorHitObject
    {
        private static Texture2D defaultTexture = Skin.Assets["arcs"];
        private static Texture2D selected = PulsarcDrawing.CreateSelectTexture(defaultTexture);

        // Currently about twice the Z Location that arcs normally dissapear at on speed 25
        // TODO: Find a good value for this, maybe it needs to change with speed/scale?
        private const int MAX_Z_LOCATION = 8000;

        public EditorArcHitObject(int time, double angle, int keys, double baseSpeed)
            : base(time, angle, keys, baseSpeed, false)
        {
            Hittable = false;
        }

        public void Select()
        {
            Texture = selected;
        }

        public void Deselect()
        {
            Texture = defaultTexture;
        }

        public override void RecalcPos(int currentTime, double currentScale, double crosshairZLoc)
        {
            if (SameAsLastFrame(currentTime, currentScale, crosshairZLoc)) { return; }

            SetZLocation(currentTime, currentScale * BaseSpeed, crosshairZLoc);

            Resize(FindArcRadius());
        }

        public override bool IsSeen() => base.IsSeen() && ZLocation < MAX_Z_LOCATION;
        
        private bool SameAsLastFrame(int time, double scale, double crosshairZLoc)
        {
            // The conditionals we're looking at
            bool timeSame = time == lastFrameTime;
            bool scaleSame = scale == lastFrameScale;
            bool crosshairZLocSame = crosshairZLoc == lastFrameCrosshairZLoc;

            // If any of the current values are different,
            // change the lastFrame values to the current value.
            lastFrameTime = !timeSame ? time : lastFrameTime;
            lastFrameScale = !scaleSame ? scale : lastFrameScale;
            lastFrameCrosshairZLoc = !crosshairZLocSame ? crosshairZLoc : lastFrameCrosshairZLoc;

            // If all our conditionals were true, then this frame is the same.
            // If at least one of our conditionals were false, this frame is different.
            return timeSame && scaleSame && crosshairZLocSame;
        }

        private int lastFrameTime = 0;
        private double lastFrameScale = 0;
        private double lastFrameCrosshairZLoc = 0;
    }
}
