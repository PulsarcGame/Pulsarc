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
            setConfigAndSection();
        }

        protected abstract void setConfigAndSection();

        public override void Draw()
        {
            base.Draw();

            foreach (TextDisplayElement tde in textElements)
            {
                tde.Draw();
            }
        }

        public override void move(Vector2 delta, bool scaledPositioning = true)
        {
            base.move(delta, scaledPositioning);

            foreach (TextDisplayElement tde in textElements)
            {
                tde.move(delta, scaledPositioning);
            }
        }

        public override void scaledMove(Vector2 delta)
        {
            base.scaledMove(delta);

            foreach (TextDisplayElement tde in textElements)
            {
                tde.scaledMove(delta);
            }
        }

        /// <summary>
        /// Add a TextDisplayElement to the Card, using the config to
        /// determine their positioning and other properties.
        /// </summary>
        /// <param name="typeName">The "typeName" of the button, or the prefix in the config.</param>
        protected virtual void addTextDisplayElement(string typeName)
        {
            // Find variables for TDE
            Vector2 position = Skin.getStartPosition(config, section, typeName + "StartPos", this); // Vector2 position;
            int fontSize = getSkinnableInt(typeName + "FontSize"); // int fontSize
            Anchor anchor = getSkinnableAnchor(typeName + "Anchor"); // Anchor textAnchor;
            Color color = Skin.getColor(config, section, typeName + "Color"); // Color textColor;

            // Make TDE
            TextDisplayElement text = new TextDisplayElement("", position, fontSize, anchor, color);

            // Offset
            Vector2 offset = new Vector2(
                getSkinnableInt(typeName + "X"),
                getSkinnableInt(typeName + "Y"));

            text.scaledMove(offset);

            //Add TDE
            textElements.Add(text);
        }

        /// <summary>
        /// Find a float from the Metadata section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        protected float getSkinnableFloat(string key)
        {
            return Skin.getConfigFloat(config, section, key);
        }

        /// <summary>
        /// Find a int from the Metadata section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        protected int getSkinnableInt(string key)
        {
            return Skin.getConfigInt(config, section, key);
        }

        /// <summary>
        /// Find an Anchor from the Metadata section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        protected Anchor getSkinnableAnchor(string key)
        {
            return Skin.getConfigAnchor(config, section, key);
        }

        /// <summary>
        /// Find a string from the Metadata section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The string of the key provided.</returns>
        protected string getSkinnableString(string key)
        {
            return Skin.getConfigString(config, section, key);
        }
    }
}
