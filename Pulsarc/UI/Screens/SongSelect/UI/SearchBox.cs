using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    public class SearchBox : Drawable
    {
        private readonly TextDisplayElement _textDisplay;

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

            _textDisplay = new TextDisplayElement(search, startPos, fontSize, textAnchor, textColor);
            _textDisplay.Move(offsetX, offsetY);
            _textDisplay.ProcessedPosition = _textDisplay.TruePosition;
        }

        public void Update(string text)
        {
            _textDisplay.Update(text);
        }

        public void AddText(string c)
        {
            _textDisplay.Text.Append(c);
        }

        public void RemoveLast()
        {
            _textDisplay.Text.Length--;
        }

        public void Clear()
        {
            _textDisplay.Text.Clear();
        }

        public string GetText()
        {
            return _textDisplay.Text.ToString();
        }

        public override void Draw()
        {
            base.Draw();
            _textDisplay.Draw();
        }
    }
}
