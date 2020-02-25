using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using Pulsarc.Utils;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    public class MapTimer : FillBar
    {
        public static Texture2D DefaultTexture = Skin.Assets["map_timer"];

        /// <summary>
        /// Creates a FillBar that is used to represent the time spent in a map.
        /// </summary>
        /// <param name="totalTime">The total time the map will play for (in ms).</param>
        /// <param name="fillDirection">The direction this will fill in.</param>
        /// <param name="startingTime">The current time (in ms) of the map.
        /// Change this if starting at a point in the map that isn't the start. Default is 0.</param>
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
