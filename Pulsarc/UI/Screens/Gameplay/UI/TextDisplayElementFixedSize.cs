using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Utils;
using System.Globalization;
using System.Text;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class TextDisplayElementFixedSize : TextDisplayElement
    {
        public string lastValueString = "";
        public int lastValueInt = -1;
        public long lastValueLong = -1;
        public double lastValueDouble = -1;
        public string numberFormat = "#,#0";

        public string append;
        public TextDisplayElementFixedSize(string name, Vector2 position, string append, int fontSize = 18, Anchor anchor = Anchor.Center) : base (name,position, fontSize, anchor)
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
                base.Update(value.ToString(numberFormat) + append);
                lastValueInt = value;
            }
        }

        public void Update(long value)
        {
            if (value != lastValueLong)
            {
                base.Update(value.ToString(numberFormat) + append);
                lastValueLong = value;
            }
        }

        public void Update(double value)
        {
            if (value != lastValueDouble)
            {
                base.Update(value.ToString(numberFormat) + append);
                lastValueDouble = value;
            }
        }
    }
}
