using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Pulsarc.UI.Screens;
using System;
using System.Collections.Generic;
using System.Text;
using Wobble.Screens;

namespace Pulsarc.UI.Buttons
{
    class NavigationButton : Drawable
    {
        TextDisplayElement text;
        PulsarcScreen screen;

        bool removeFirst;

        /// <summary>
        /// A button that allows the user to navigate to a new Screen.
        /// </summary>
        /// <param name="screen">The screen this button will navigate to.</param>
        /// <param name="type">The button type this navigation button is.</param>
        /// <param name="text">The text this button will display.</param>
        /// <param name="position">The position of this button.</param>
        /// <param name="anchor">The anchor for this button, defaults to Anchor.Center.</param>
        /// <param name="removeFirst">Whether or not the current screen should be removed
        /// before moving to the navigation screen. True = Remove Current screen before navigating.</param>
        public NavigationButton(PulsarcScreen screen, int type, string text, Vector2 position, Anchor anchor = Anchor.Center, bool removeFirst = false) : base(Skin.assets["button_back_"+type], position, anchor: anchor)
        {
            this.text = new TextDisplayElement(text,new Vector2(position.X - (1 - scale)*10, position.Y - (1 - scale) * 10),color: Color.Black, anchor: Anchor.Center);
            this.screen = screen;
            this.removeFirst = removeFirst;

            hover = new Drawable(Skin.assets["button_hover_"+type], position, anchor: anchor);
            hoverObject = true;
        }

        /// <summary>
        /// Navigate to this button's assigned screen.
        /// </summary>
        public void navigate()
        {

            if(removeFirst)
            {
                ScreenManager.RemoveScreen(true);
            }
            ScreenManager.AddScreen(screen);
            if(!screen.initialized)
            {
                screen.Init();
            }
        }

        public override void Draw()
        {
            base.Draw();
            text.Draw();
        }
    }
}
