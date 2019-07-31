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
        public NavigationButton(PulsarcScreen screen, int type, string text, Vector2 position, Anchor anchor = Anchor.Center, bool removeFirst = false) : base(Skin.assets["button_back_"+type], position, anchor: anchor)
        {
            this.text = new TextDisplayElement(text,new Vector2(position.X - (1 - scale)*10, position.Y - (1 - scale) * 10),color: Color.Black, anchor: Anchor.Center);
            this.screen = screen;
            this.removeFirst = removeFirst;

            hover = new Drawable(Skin.assets["button_hover_"+type], position, anchor: anchor);
            hoverObject = true;
        }

        public void navigate()
        {

            if(removeFirst)
            {
                ScreenManager.RemoveScreen(true);
            }
            ScreenManager.AddScreen(screen);
            screen.Init();
        }

        public override void Draw()
        {
            base.Draw();
            text.Draw();
        }
    }
}
