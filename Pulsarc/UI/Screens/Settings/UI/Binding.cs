using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Skinning;
using Pulsarc.Utils;

namespace Pulsarc.UI.Screens.Settings.UI
{
    class Binding : Setting
    {
        Drawable listening;
        TextDisplayElement key;

        public Binding(string title, string more, Vector2 position, string startingValue) : 
            base(title, more, position, Skin.assets["settings_binding"], -1, Anchor.CenterLeft, startingValue, "string")
        {
            listening = new Drawable(Skin.assets["settings_binding_focus"], position, anchor: Anchor.CenterLeft);

            key = new TextDisplayElement("", new Vector2(position.X + listening.currentSize.X/2, position.Y), color: Color.Black, anchor: Anchor.Center) ;

            key.Update(getSaveValue());

        }
        public override dynamic getSaveValue()
        {
            return value;
        }

        public override void handleKeyEvent(Keys pressed) 
        {
            if(keyListen)
            {
                value = pressed.ToString();
                key.Update(getSaveValue());
                Config.setValue("Bindings", text, getSaveValue());
                Config.addBinding(text);
                keyListen = false;
            }
        }

        public override void move(Vector2 position, bool scaledPositioning = true)
        {
            base.move(position, scaledPositioning);
            listening.move(position, scaledPositioning);
            key.move(position, scaledPositioning);
        }

        public override void onClick(Point mousePosition)
        {
            keyListen = !keyListen;
        }

        public override void Draw()
        {
            if(keyListen)
            {
                listening.Draw();
                title.Draw();
            } else
            {
                base.Draw();
                key.Draw();
            }
        }
    }
}
