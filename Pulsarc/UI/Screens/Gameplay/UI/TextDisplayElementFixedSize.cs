using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Utils;
using System.Text;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class TextDisplayElementFixedSize : TextDisplayElement
    {
        public string lastValueString = "";
        public int lastValueInt = -1;
        public long lastValueLong = -1;
        public double lastValueDouble = -1;

        public string append;
        public TextDisplayElementFixedSize(string name, Vector2 position, string append, int fontSize = 18, bool centered = false) : base (name,position, fontSize, centered)
        {
            this.append = append;
        }

        public new void Update(string value)
        {
            if (value != lastValueString)
            {
                base.Update(value.ToString() + append);
                lastValueString = value;
            }
        }

        public void Update(int value)
        {
            if (value != lastValueInt)
            {
                base.Update(value.ToString() + append);
                lastValueInt = value;
            }
        }

        public void Update(long value)
        {
            if (value != lastValueLong)
            {
                base.Update(value.ToString() + append);
                lastValueLong = value;
            }
        }

        public void Update(double value)
        {
            if (value != lastValueDouble)
            {
                base.Update(value.ToString() + append);
                lastValueDouble = value;
            }
        }
    }
}
