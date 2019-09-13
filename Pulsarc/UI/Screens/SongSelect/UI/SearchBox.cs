using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    class SearchBox : Drawable
    {
        TextDisplayElement textDisplay;

        public SearchBox(string search, Vector2 position, Anchor anchor = Anchor.TopLeft) : base(Skin.assets["searchbox"], position, anchor: anchor)
        {
            textDisplay = new TextDisplayElement(search, new Vector2(position.X - Texture.Width + 20, position.Y + Texture.Height/4), anchor: Anchor.CenterLeft);
        }

        public void Update(string text)
        {
            textDisplay.Update(text);
        }

        public void addText(string c)
        {
            textDisplay.text.Append(c);
        }

        public void removeLast()
        {
            textDisplay.text.Length--;
        }

        public void clear()
        {
            textDisplay.text.Clear();
        }

        public string getText()
        {
            return textDisplay.text.ToString();
        }

        public override void Draw()
        {
            base.Draw();
            textDisplay.Draw();
        }
    }
}
