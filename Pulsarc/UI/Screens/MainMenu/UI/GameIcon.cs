using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.MainMenu.UI
{
    class GameIcon : Drawable
    {
        public GameIcon(Vector2 position, Anchor anchor = Anchor.CenterTop)
            : base(Skin.Assets["menu_game_icon"], position, anchor: anchor) {}
    }
}
