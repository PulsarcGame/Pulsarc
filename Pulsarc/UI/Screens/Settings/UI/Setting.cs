using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Utils;
using Wobble.Logging;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public abstract class Setting : Drawable
    {
        // Current value of this setting
        private protected dynamic Value { get; set; }

        // Type of setting and the text for the setting
        private string Type { get; set; }
        protected string Text { get; set; }

        // The TDEs to display the text of this Setting
        protected TextDisplayElement Title { get; set; }
        // NOTE: More isn't being used currently
        public TextDisplayElement More { get; protected set; }

        // Whether this input needs constant input (true) or single click (false)
        public bool Hold { get; protected set; } = false;

        // Whether this input is currently listening for a Key Event
        public bool KeyListen { get; protected set; } = false;

        /// <summary>
        /// Initializes a setting that can change different options in Pulsarc.
        /// </summary>
        /// <param name="title">Name of the Setting</param>
        /// <param name="position">Setting's position.</param>
        /// <param name="texture">The texture for the Setting.</param>
        /// <param name="anchor">The anchor position for this Setting.</param>
        /// <param name="baseValue">The value this setting starts with.</param>
        /// <param name="type">The type of variable this setting changes.</param>
        public Setting(string title, Vector2 position, Texture2D texture, Anchor anchor, dynamic baseValue, string type)
            : base(texture, position, anchor: anchor)
        {
            Value = baseValue;
            Type = type;
            PulsarcLogger.Important($"{type} set for {title}", LogType.Runtime);
            Text = title;
            Title = new TextDisplayElement(title, new Vector2(position.X - 50, position.Y), anchor: Anchor.CenterRight);
        }

        /// <summary>
        /// Method that checks whether this setting has been clicked on, and
        /// what to do when clicked.
        /// </summary>
        /// <param name="mousePosition">The current mouse position to check if
        /// this was clicked.</param>
        public abstract void OnClick(Point mousePosition);

        public override void Draw()
        {
            base.Draw();
            Title.Draw();
        }

        public override void Move(Vector2 delta, bool? heightScaled = null)
        {
            base.Move(delta, heightScaled);
            Title.Move(delta, heightScaled);
        }

        /// <summary>
        /// Save this setting into the config.ini file.
        /// </summary>
        /// <param name="category">Category to save under</param>
        /// <param name="key">The key to modify.</param>
        public virtual void Save(string category, string key)
        {
            switch(Type)
            {
                case "float":
                    Config.SetFloat(category, key,(float) GetSaveValue());
                    break;
                case "int":
                    Config.SetInt(category, key,(int) GetSaveValue());
                    break;
                case "double":
                    Config.SetDouble(category, key,(double) GetSaveValue());
                    break;
                case "bool":
                    Config.SetBool(category, key, (bool)GetSaveValue());
                    break;
                case "string":
                    Config.SetString(category, key, (string)GetSaveValue());
                    break;
                default:
                    PulsarcLogger.Error($"Cannot save type {Type} in category {category} for setting {key}", LogType.Runtime);
                    break;
            }
        }

        /// <summary>
        /// Handle a key event.
        /// </summary>
        /// <param name="key">The key to handle.</param>
        public virtual void HandleKeyEvent(Keys key) { }

        /// <summary>
        /// Get the current value that can be saved.
        /// </summary>
        /// <returns>This setting's current value.</returns>
        protected abstract dynamic GetSaveValue();
    }
}
