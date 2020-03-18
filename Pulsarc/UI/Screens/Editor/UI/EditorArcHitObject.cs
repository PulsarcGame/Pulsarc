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
            if (Editor.SameAsLastFrame(currentTime, currentScale, crosshairZLoc)) { return; }

            SetZLocation(currentTime, currentScale * BaseSpeed, crosshairZLoc);

            Resize(FindArcRadius());
        }

        public override bool IsSeen() => base.IsSeen() && ZLocation < MAX_Z_LOCATION;
    }
}
