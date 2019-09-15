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
        /// <summary>
        /// A "Back Button" that will remove the current screen when clicked.
        /// </summary>
        /// <param name="skinAsset"></param>
        /// <param name="position">The position of the ReturnButton</param>
        /// <param name="anchor">The anchor of the ReturnButton. Defaults to BottomLeft instead of TopLeft.</param>
        public ReturnButton(string skinAsset, Vector2 position, Anchor anchor = Anchor.BottomLeft) : base(Skin.assets[skinAsset], position, anchor: anchor)
        {
        }

        /// <summary>
        /// When this ReturnButton is clicked, remove the current screen from view.
        /// </summary>
        public void onClick()
        {
            ScreenManager.RemoveScreen(true);
        }
    }
}
