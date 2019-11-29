using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    class BeatmapCardDifficulty : Drawable
    {
        private Drawable fill;

        public BeatmapCardDifficulty(Vector2 position, float percent, Anchor anchor)
            : base (Skin.Assets["card_diff_bar"], position, anchor: anchor)
        {
            fill = new Drawable(Skin.Assets["card_diff_fill"], position, anchor: anchor);
            fill.drawnPart = new Rectangle(0,0,(int) (fill.Texture.Width * percent), Texture.Height);
        }

        public override void Move(Vector2 delta, bool scaledPositioning = true)
        {
            base.Move(delta, scaledPositioning);
            fill.Move(delta, scaledPositioning);
        }

        public override void ChangePosition(Vector2 position, bool topLeftPositioning = false)
        {
            base.ChangePosition(position, topLeftPositioning);
            fill?.ChangePosition(position, topLeftPositioning);
        }

        public override void Draw()
        {
            fill.Draw();
            base.Draw();
        }
    }
}
