using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.Text;
using Wobble.Screens;

namespace Pulsarc.UI.Buttons
{
    class ReturnButton : Drawable
    {
        public ReturnButton(string skinAsset, Vector2 position) : base(Skin.assets[skinAsset], position)
        {
        }

        public void onClick()
        {
            ScreenManager.RemoveScreen(true);
        }
    }
}
