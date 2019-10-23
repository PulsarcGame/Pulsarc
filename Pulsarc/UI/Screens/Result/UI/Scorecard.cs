using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Result.UI
{
    // TODO: Redesign ResultScorecard to work on other aspect ratios than 16:9
    class ResultScorecard : Drawable
    {
        public ResultScorecard(Vector2 position, Anchor anchor = Anchor.Center)
            : base(Skin.assets["result_scorecard"], position, anchor: anchor) { }
    }
}
