using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Utils;
using System.Text;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class TextDisplayElementFixedSize : TextDisplayElement
    {
        StringBuilder stringBuilder;

        public TextDisplayElementFixedSize(string name, Vector2 position, int capacity, int fontSize = 18, bool centered = false) : base (name,position, fontSize, centered)
        {
            stringBuilder = new StringBuilder(capacity);
        }

        public void Update(string value, string append)
        {
            Update(stringBuilder.Append(value).Append(append).ToString());
            stringBuilder.Clear();
        }
    }
}
