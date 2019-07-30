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

        /// <summary>
        /// TextDisplayElement that will be updated regularily. Improves performances with strings.
        /// </summary>
        /// <param name="name">String to be prepended to the displayed value</param>
        /// <param name="position">Where to display the text element</param>
        /// <param name="append">String to be appended to the displayed value</param>
        /// <param name="fontSize">The size to be used for drawing the font</param>
        /// <param name="anchor">The anchor</param>
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
