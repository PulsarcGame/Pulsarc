using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Result.UI
{
    // TODO: Redesign ResultScorecard to work on other aspect ratios than 16:9
    class ResultCard : Drawable
    {
        public ResultCard(Vector2 position, Anchor anchor = Anchor.Center)
            : base(Skin.Assets["result_scorecard"], position, anchor: anchor) { }
    }
}
