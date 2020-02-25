using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    class BeatmapCardDifficulty : BorderedFillBar
    {
        public static Texture2D DefaultTexture = Skin.Assets["card_diff_fill"];
        public static Texture2D DefaultBorderTexture = Skin.Assets["card_diff_bar"];

        public BeatmapCardDifficulty(Vector2 position, float difficulty, Anchor anchor)
        : base
        (
            DefaultTexture,
            position,
            0,
            10,
            difficulty,
            DefaultBorderTexture,
            anchor: anchor
        )
        { }
    }
}
