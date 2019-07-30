using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.MainMenu.UI
{
    class GameIcon : Drawable
    {
        public GameIcon(Vector2 position) : base(Skin.assets["menu_game_icon"], position) {}
    }
}
