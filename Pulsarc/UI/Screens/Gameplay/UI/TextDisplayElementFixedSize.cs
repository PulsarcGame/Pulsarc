using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Utils;
using System.Text;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class TextDisplayElementFixedSize : TextDisplayElement
    {
        public dynamic lastValue;
        public string append;
        public TextDisplayElementFixedSize(string name, Vector2 position, string append, int fontSize = 18, bool centered = false) : base (name,position, fontSize, centered)
        {
            this.append = append;
        }

        public void Update(dynamic value)
        {
            if (value != lastValue)
            {
                base.Update((string) (value.ToString())+append);
                lastValue = value;
            }
        }
    }
}
