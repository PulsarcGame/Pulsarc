using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.Text;
using Wobble.Screens;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Beatmaps;

namespace Pulsarc.UI.Buttons
{
    class RetryButton : Drawable
    {
        /// <summary>
        /// A button on the result screen that easily allows the player
        /// to retry the map they just played.
        /// </summary>
        /// <param name="skinAsset"></param>
        /// <param name="position">The position of the Retry Button.</param>
        /// <param name="anchor">The anchor of the Retry Button. Defaults to BottomRight instead of TopLeft.</param>
        public RetryButton(string skinAsset, Vector2 position, Anchor anchor = Anchor.BottomRight) : base(Skin.assets[skinAsset], position, anchor: anchor)
        {
        }

        /// <summary>
        /// This method is called when the RetryButton is clicked on, it restarts
        /// the map using the Beatmap provided.
        /// </summary>
        /// <param name="beatmap">The Beatmap to play.</param>
        public void onClick(Beatmap beatmap)
        {
            ScreenManager.RemoveScreen(true);
            GameplayEngine gameplay = new GameplayEngine();
            gameplay.Init(beatmap);
            ScreenManager.AddScreen(gameplay);
        }
    }
}