using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    class BeatmapCardDifficulty : Drawable
    {
        private readonly Drawable _fill;

        public BeatmapCardDifficulty(Vector2 position, float percent, Anchor anchor)
            : base (Skin.Assets["card_diff_bar"], position, anchor: anchor)
        {
            _fill = new Drawable(Skin.Assets["card_diff_fill"], position, anchor: anchor);
            _fill.DrawnPart = new Rectangle(0,0,(int) (_fill.Texture.Width * percent), Texture.Height);
        }

        public override void Move(Vector2 delta, bool? heightScaled = null)
        {
            base.Move(delta, heightScaled);
            _fill.Move(delta, heightScaled);
        }

        public override void ChangePosition(Vector2 position, bool topLeftPositioning = false)
        {
            base.ChangePosition(position, topLeftPositioning);
            _fill?.ChangePosition(position, topLeftPositioning);
        }

        public override void Draw()
        {
            _fill.Draw();
            base.Draw();
        }
    }
}
