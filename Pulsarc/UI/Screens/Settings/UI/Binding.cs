using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Skinning;
using Pulsarc.Utils;

namespace Pulsarc.UI.Screens.Settings.UI
{
    class Binding : Setting
    {
        private Drawable listening;
        private TextDisplayElement key;

        /// <summary>
        /// Creates a new "Binding" Setting, a setting that keeps track of and
        /// can rebind a key button to a specific action.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="more"></param>
        /// <param name="position"></param>
        /// <param name="startingValue"></param>
        public Binding(string title, string more, Vector2 position, string startingValue) : 
            base(title, position, Skin.Assets["settings_binding"], Anchor.CenterLeft, startingValue, "string")
        {
            listening = new Drawable(Skin.Assets["settings_binding_focus"], position, anchor: Anchor.CenterLeft);

            key = new TextDisplayElement("", new Vector2(position.X + listening.CurrentSize.X/2, position.Y), color: Color.Black, anchor: Anchor.Center) ;

            key.Update(GetSaveValue());

        }

        protected sealed override dynamic GetSaveValue()
        {
            return Value;
        }

        public override void HandleKeyEvent(Keys pressed) 
        {
            if (KeyListen)
            {
                Value = pressed.ToString();
                key.Update(GetSaveValue());
                Config.SetValue("Bindings", Text, GetSaveValue());
                Config.AddBinding(Text);
                KeyListen = false;
            }
        }

        public override void Move(Vector2 delta, bool? heightScaled = null)
        {
            base.Move(delta, heightScaled);
            listening.Move(delta, heightScaled);
            key.Move(delta, heightScaled);
        }

        /// <summary>
        /// When this is clicked, reverse the KeyListen state
        /// to record or exit the key binding state.
        /// </summary>
        /// <param name="mousePosition"></param>
        public override void OnClick(Point mousePosition)
        {
            KeyListen = !KeyListen;
        }

        public override void Draw()
        {
            if (KeyListen)
            {
                listening.Draw();
                Title.Draw();
            }
            else
            {
                base.Draw();
                key.Draw();
            }
        }
    }
}
