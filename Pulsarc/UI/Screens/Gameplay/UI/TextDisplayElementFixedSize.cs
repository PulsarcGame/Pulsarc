using Microsoft.Xna.Framework;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    public class TextDisplayElementFixedSize : TextDisplayElement
    {
        // Last Values
        private string _lastValueString = "";
        private int _lastValueInt = -1;
        private long _lastValueLong = -1;
        private double _lastValueDouble = -1;

        // Format for the current Value
        public string NumberFormat { get; set; } = "#,#0";

        private string append;

        /// <summary>
        /// TextDisplayElement that will be updated regularily. Improves performances with strings.
        /// </summary>
        /// <param name="name">String to be prepended to the displayed value</param>
        /// <param name="position">Where to display the text element</param>
        /// <param name="append">String to be appended to the displayed value</param>
        /// <param name="fontSize">The size to be used for drawing the font</param>
        /// <param name="anchor">The anchor</param>
        /// <param name="color"></param>
        public TextDisplayElementFixedSize(string name, Vector2 position, string append, int fontSize = 18, Anchor anchor = Anchor.Center, Color? color = null)
            : base (name, position, fontSize, anchor, color)
        {
            this.append = append;
        }

        /// <summary>
        /// Update text to the provided string.
        /// </summary>
        /// <param name="value">String the text should display.</param>
        public new void Update(string value)
        {
            if (value == _lastValueString) return;
            base.Update(value + append);
            _lastValueString = value;
        }

        /// <summary>
        /// Update text to the provided integer.
        /// </summary>
        /// <param name="value">The int the text should display, using
        /// this TDE's number format.</param>
        public void Update(int value)
        {
            if (value == _lastValueInt) return;
            base.Update(value.ToString(NumberFormat) + append);
            _lastValueInt = value;
        }

        /// <summary>
        /// Update text to the provided long.
        /// </summary>
        /// <param name="value">The long the text should display, using
        /// this TDE's number format.</param>
        public void Update(long value)
        {
            if (value == _lastValueLong) return;
            base.Update(value.ToString(NumberFormat) + append);
            _lastValueLong = value;
        }

        /// <summary>
        /// Update text to the provided double.
        /// </summary>
        /// <param name="value">The double the text should display, using
        /// this TDE's number format.</param>
        public void Update(double value)
        {
            if (value == _lastValueDouble) return;
            base.Update(value.ToString(NumberFormat) + append);
            _lastValueDouble = value;
        }
    }
}
