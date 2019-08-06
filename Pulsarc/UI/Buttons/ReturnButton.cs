using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Wobble.Screens;

namespace Pulsarc.UI.Buttons
{
    class ReturnButton : Drawable
    {
        public ReturnButton(string skinAsset, Vector2 position, Anchor anchor = Anchor.BottomLeft) : base(Skin.assets[skinAsset], position, anchor: anchor)
        {
        }

        public void OnClick()
        {
            ScreenManager.RemoveScreen(true);
        }
    }
}