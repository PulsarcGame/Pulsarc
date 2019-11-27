using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using System.Collections.Generic;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    /// <summary>
    /// A Drawable that has multiple TDE's for display along with the base card.
    /// </summary>
    public abstract class Card : Drawable
    {
        // Keep track of whether this card has been clicked on or not.
        protected bool isClicked = false;

        // List of text elements to display
        protected List<TextDisplayElement> textElements = new List<TextDisplayElement>();

        // Config speicifics
        protected string config, section;

        public Card(Texture2D texture, Vector2 position, Anchor anchor)
            : base(texture, position, anchor: anchor)
        {
            SetConfigAndSection();
        }

        protected abstract void SetConfigAndSection();

        public override void Draw()
        {
            base.Draw();

            for (int i = 0; i < textElements.Count; i++)
                textElements[i].Draw();
        }

        public override void Move(Vector2 delta, bool scaledPositioning = true)
        {
            base.Move(delta, scaledPositioning);

            for (int i = 0; i < textElements.Count; i++)
                textElements[i].Move(delta, scaledPositioning);
        }

        public override void ScaledMove(Vector2 delta)
        {
            base.ScaledMove(delta);

            for (int i = 0; i < textElements.Count; i++)
                textElements[i].ScaledMove(delta);
        }

        /// <summary>
        /// Add a TextDisplayElement to the Card, using the config to
        /// determine their positioning and other properties.
        /// </summary>
        /// <param name="typeName">The "typeName" of the button, or the prefix in the config.</param>
        protected virtual void AddTextDisplayElement(string typeName)
        {
            // Find variables for TDE
            Vector2 position = Skin.GetConfigStartPosition(config, section, typeName + "StartPos", this); // Vector2 position;
            int fontSize = GetSkinnableInt(typeName + "FontSize"); // int fontSize
            Anchor anchor = GetSkinnableAnchor(typeName + "Anchor"); // Anchor textAnchor;
            Color color = Skin.GetConfigColor(config, section, typeName + "Color"); // Color textColor;

            // Make TDE
            TextDisplayElement text = new TextDisplayElement("", position, fontSize, anchor, color);

            // Offset
            Vector2 offset = new Vector2(
                GetSkinnableInt(typeName + "X"),
                GetSkinnableInt(typeName + "Y"));

            text.Move(offset);

            //Add TDE
            textElements.Add(text);
        }

        #region GeSkinnable Methods
        /// <summary>
        /// Find a float from the Metadata section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        protected float GetSkinnableFloat(string key)
        {
            return Skin.GetConfigFloat(config, section, key);
        }

        /// <summary>
        /// Find a int from the Metadata section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        protected int GetSkinnableInt(string key)
        {
            return Skin.GetConfigInt(config, section, key);
        }

        /// <summary>
        /// Find an Anchor from the Metadata section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        protected Anchor GetSkinnableAnchor(string key)
        {
            return Skin.GetConfigAnchor(config, section, key);
        }

        /// <summary>
        /// Find a string from the Metadata section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The string of the key provided.</returns>
        protected string GetSkinnableString(string key)
        {
            return Skin.GetConfigString(config, section, key);
        }
        #endregion
    }
}
