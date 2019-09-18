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
        public NavigationButton(PulsarcScreen screen, int type, string text, Vector2 position, Anchor anchor = Anchor.Center, bool removeFirst = false, Anchor textAnchor = Anchor.Center, Color? textColor = null, int fontSize = 18)
        : this(screen, type, position, new TextDisplayElement(text, new Vector2(position.X, position.Y), fontSize, textAnchor, textColor)) {}
        

        public NavigationButton(PulsarcScreen screen, int type, Vector2 position, TextDisplayElement text, Anchor anchor = Anchor.Center, bool removeFirst = false)
            : base(Skin.assets["button_back_" + type], position, anchor: anchor)
        {
            this.text = text;
            this.text.move(new Vector2((1 - scale) * -10, (1 - scale) * -10)); // TODO: Change positioniong depending on textAnchor | Position text properly, without using hacky workarounds

            this.screen = screen;
            this.removeFirst = removeFirst;

            Hover = new Drawable(Skin.assets["button_hover_" + type], position, anchor: anchor);
        }

        /// <summary>
        /// Change the active screen to this button's assigned screen.
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

        /// <summary>
        /// Move the Navigation Button and its parts.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="scaledPositioning"></param>
        public override void move(Vector2 position, bool scaledPositioning = true)
        {
            base.move(position, scaledPositioning);
            text.move(position, scaledPositioning);
        }

        public override void Draw()
        {
            base.Draw();
            text.Draw();
        }
    }
}
