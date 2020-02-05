using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using Pulsarc.Utils;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    public class MapTimer : FillBar
    {
        public static Texture2D DefaultTexture = Skin.Assets["map_timer"];

        public MapTimer(double totalTime, FillBarDirection fillDirection, double startingTime = 0)
        : base
        (
            DefaultTexture,
            AnchorUtil.FindScreenPosition(Anchor.TopRight),
            0,
            (float)totalTime,
            (float)startingTime,
            anchor: Anchor.TopRight,
            fillDirection: fillDirection
        )
            => Resize(new Vector2(DefaultTexture.Width, Pulsarc.CurrentHeight));
    }
}
