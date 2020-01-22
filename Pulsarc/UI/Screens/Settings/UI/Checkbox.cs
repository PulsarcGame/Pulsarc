using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Settings.UI
{
    class Checkbox : Setting
    {
        /// <summary>
        /// The X that signifies if this option is activated or not.
        /// </summary>
        private Drawable cross;

        public Checkbox(string title, string more, Vector2 position, string type, bool startingValue) :
            base(title, more, position, Skin.Assets["settings_checkbox"], Anchor.CenterLeft, startingValue, type)
        {
            cross = new Drawable(Skin.Assets["settings_checkbox_cross"], position, anchor: Anchor.CenterLeft);

            cross.ChangePosition(position);
        }

        public override dynamic GetSaveValue() => Value;

        /// <summary>
        /// When this is click reverse the state of the value,
        /// from true to false or false to true.
        /// </summary>
        /// <param name="mousePosition"></param>
        public override void OnClick(Point mousePosition) => Value = !(bool)Value;

        public override void Move(Vector2 delta, bool? heightScaled = null)
        {
            base.Move(delta, heightScaled);
            cross.Move(delta, heightScaled);
        }

        public override void Draw()
        {
            base.Draw();

            if (Value)
            {
                cross.Draw();
            }
        }
    }
}
