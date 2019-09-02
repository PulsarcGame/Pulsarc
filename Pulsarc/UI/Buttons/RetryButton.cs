using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Wobble.Screens;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Beatmaps;
using Pulsarc.UI.Common;

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
        public void onClick(Beatmap beatmap, Background mapBackground)
        {
            // Reset mapBackground for gameplay
            mapBackground.dim = mapBackground.dimTexture.opacity > 0;
            mapBackground.move(new Vector2(Pulsarc.xBaseRes / -2 + 200, 0));
            
            ScreenManager.RemoveScreen(true);
            GameplayEngine gameplay = new GameplayEngine(mapBackground);
            gameplay.Init(beatmap);
            ScreenManager.AddScreen(gameplay);
        }
    }
}