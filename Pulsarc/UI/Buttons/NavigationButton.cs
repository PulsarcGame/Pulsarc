using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Pulsarc.UI.Screens;
using Wobble.Screens;

namespace Pulsarc.UI.Buttons
{
    sealed class NavigationButton : Drawable
    {
        private readonly TextDisplayElement _text;
        private readonly PulsarcScreen _screen;

        private readonly bool _removeFirst;

        /// <summary>
        /// A button that allows the user to navigate to a new Screen.
        /// </summary>
        /// <param name="screen">The screen this button will navigate to.</param>
        /// <param name="type">The button type this navigation button is.</param>
        /// <param name="text">The text this button will display.</param>
        /// <param name="position">The position of this button.</param>
        /// <param name="anchor">The anchor for this button, defaults to Anchor.Center.</param>
        /// <param name="removeFirst">Whether or not the current screen should be removed
        /// <param name="textAnchor"/>The anchor for the text on this button.</param>
        /// <param name="textAnchor"></param>
        /// <param name="textColor">The color of the text.</param>
        /// <param name="fontSize">The font size of the text.</param>
        /// before moving to the navigation screen. True = Remove Current screen before navigating.
        public NavigationButton(PulsarcScreen screen, int type, string text, Vector2 position, Anchor anchor = Anchor.Center, bool removeFirst = false, Anchor textAnchor = Anchor.Center, Color? textColor = null, int fontSize = 18)
            : this(screen, type, position, new TextDisplayElement(text, new Vector2(position.X, position.Y), fontSize, textAnchor, textColor)) {}
        

        public NavigationButton(PulsarcScreen screen, int type, Vector2 position, TextDisplayElement text, Anchor anchor = Anchor.Center, bool removeFirst = false)
            : base(Skin.Assets["button_back_" + type], position, anchor: anchor)
        {
            _text = text;
            // TODO: Change positioniong depending on textAnchor
            // TODO: Position text properly, without using hacky workarounds
            _text.Move(new Vector2((1 - Scale) * -10, (1 - Scale) * -10));

            _screen = screen;
            _removeFirst = removeFirst;

            Hover = new Drawable(Skin.Assets["button_hover_" + type], position, anchor: anchor);
        }

        /// <summary>
        /// Change the active screen to this button's assigned screen.
        /// </summary>
        public void Navigate()
        {
            if (_removeFirst)
                ScreenManager.RemoveScreen(true);

            ScreenManager.AddScreen(_screen);
            
            if (!_screen.Initialized)
                _screen.Init();
        }

        /// <summary>
        /// Move the Navigation Button and its parts.
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="heightScaled"></param>
        public override void Move(Vector2 delta, bool? heightScaled = null)
        {
            base.Move(delta, heightScaled);
            _text.Move(delta, heightScaled);
        }

        public override void Draw()
        {
            base.Draw();
            _text.Draw();
        }
    }
}
