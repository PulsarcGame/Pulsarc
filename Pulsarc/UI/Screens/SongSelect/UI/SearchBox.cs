using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    class SearchBox : Drawable
    {
        private TextDisplayElement textDisplay;

        public SearchBox(string search, Vector2 position, Anchor anchor = Anchor.TopLeft)
            : base(Skin.Assets["searchbox"], position, anchor: anchor)
        {
            string config = "song_select";
            string section = "Properties";
            string name = "SearchBarText";

            Anchor textAnchor = Skin.GetConfigAnchor(config, section, $"{name}Anchor");

            Vector2 startPos = Skin.GetConfigStartPosition(config, section, $"{name}StartPos", this);

            int fontSize = Skin.GetConfigInt(config, section, $"{name}FontSize");

            Color textColor = Skin.GetConfigColor(config, section, $"{name}Color");

            int offsetX = Skin.GetConfigInt(config, section, $"{name}X");
            int offsetY = Skin.GetConfigInt(config, section, $"{name}Y");

            textDisplay = new TextDisplayElement(search, startPos, fontSize, textAnchor, textColor);
            textDisplay.ScaledMove(offsetX, offsetY);
            textDisplay.ProcessedPosition = textDisplay.TruePosition;
        }

        public void Update(string text)
        {
            textDisplay.Update(text);
        }

        public void AddText(string c)
        {
            textDisplay.Text.Append(c);
        }

        public void RemoveLast()
        {
            textDisplay.Text.Length--;
        }

        public void Clear()
        {
            textDisplay.Text.Clear();
        }

        public string GetText()
        {
            return textDisplay.Text.ToString();
        }

        public override void Draw()
        {
            base.Draw();
            textDisplay.Draw();
        }
    }
}
