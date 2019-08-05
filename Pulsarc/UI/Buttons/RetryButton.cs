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
    //TODO: Make RetryButton work 100% of the time.
    class RetryButton : Drawable
    {
        public RetryButton(string skinAsset, Vector2 position, Anchor anchor = Anchor.BottomRight) : base(Skin.assets[skinAsset], position, anchor: anchor)
        {
        }

        public void onClick(Beatmap beatmap)
        {
            ScreenManager.RemoveScreen(true);
            GameplayEngine gameplay = new GameplayEngine();
            gameplay.Init(beatmap);
            ScreenManager.AddScreen(gameplay);
        }
    }
}