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
        int offsetX;
        int offsetY;

        public SearchBox(string search, Vector2 position, Anchor anchor = Anchor.TopLeft) : base(Skin.assets["searchbox"], position, anchor: anchor)
        {
            string config = "song_select";
            string section = "Properties";
            string name = "SearchBarText";

            Anchor textAnchor = Skin.getConfigAnchor(config, section, name + "Anchor");

            Vector2 startPos = Skin.getConfigStartPosition(config, section, name + "StartPos", this);

            int fontSize = Skin.getConfigInt(config, section, name + "FontSize");

            Color textColor = Skin.getConfigColor(config, section, name + "Color");

            offsetX = Skin.getConfigInt(config, section, name + "X");
            offsetY = Skin.getConfigInt(config, section, name + "Y");

            textDisplay = new TextDisplayElement(search, startPos, fontSize, textAnchor, textColor);
            textDisplay.scaledMove(offsetX, offsetY);
            textDisplay.processedPosition = textDisplay.truePosition;
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
