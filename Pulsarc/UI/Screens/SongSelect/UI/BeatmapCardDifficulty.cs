using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    class BeatmapCardDifficulty : Drawable
    {
        Drawable fill;
        public BeatmapCardDifficulty(Vector2 position, float percent) : base (Skin.assets["card_diff_bar"], position)
        {
            fill = new Drawable(Skin.assets["card_diff_fill"], position);
            fill.drawnPart = new Rectangle(0,0,(int) (fill.texture.Width * percent), texture.Height);
        }
        public override void Move(Vector2 delta)
        {
            base.Move(delta);
            fill.Move(delta);
        }
        public override void Draw()
        {
            fill.Draw();
            base.Draw();
        }
    }
}
