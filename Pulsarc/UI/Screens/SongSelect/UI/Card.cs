using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using Pulsarc.Utils;
using System.Collections.Generic;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    /// <summary>
    /// A Drawable that has multiple TDE's for display along with the base card.
    /// </summary>
    public abstract class Card : Drawable
    {
        // Keep track of whether this card has been clicked on or not.
        protected bool IsSelected = false;

        // List of text elements to display
        protected List<TextDisplayElement> TextElements = new List<TextDisplayElement>();
        protected List<Vector2> TextElementOffsets = new List<Vector2>();
        protected List<Anchor> TextElementStartAnchors = new List<Anchor>();

        // Config speicifics
        protected string Config, Section;

        public Card(Texture2D texture, Vector2 position, Anchor anchor)
            : base(texture, position, anchor: anchor) => SetConfigAndSection();

        /// <summary>
        /// Set the config specific variables for the GetSkinnable methods.
        /// </summary>
        protected abstract void SetConfigAndSection();

        public override void Draw()
        {
            base.Draw();

            for (int i = 0; i < TextElements.Count; i++)
            {
                TextElements[i].Draw();
            }
        }

        public override void Move(Vector2 delta, bool? heightScaled = null)
        {
            base.Move(delta, heightScaled);
            UpdateElements();
        }

        public override void ChangePosition(Vector2 position, bool topLeftPositioning = false)
        {
            base.ChangePosition(position, topLeftPositioning);
            UpdateElements();
        }

        /// <summary>
        /// Updates all the elements on this card to move in the right spot.
        /// Inherited classes can add new elements in an overriden method,
        /// don't forget "base.UpdateElements()"
        /// </summary>
        protected virtual void UpdateElements()
        {
            // Don't bother updating if we aren't on screen.
            if (!OnScreen()) { return; }

            for (int i = 0; i < TextElements.Count; i++)
            {
                TextElements[i]?.ChangePosition
                (
                    AnchorUtil.FindDrawablePosition(TextElementStartAnchors[i], this)
                        + TextElementOffsets[i]
                );
            }
        }

        /// <summary>
        /// Add a TextDisplayElement to the Card, using the config to
        /// determine their positioning and other properties.
        /// </summary>
        /// <param name="typeName">The "typeName" of the button, or the prefix in the config.</param>
        protected virtual void AddTextDisplayElement(string typeName)
        {
            // Find variables for TDE
            Vector2 position = Skin.GetConfigStartPosition(Config, Section, typeName + "StartPos", out Anchor startAnchor, this);
            int fontSize = GetSkinnableInt(typeName + "FontSize");
            Anchor anchor = GetSkinnableAnchor(typeName + "Anchor");
            Color color = Skin.GetConfigColor(Config, Section, typeName + "Color");

            // Make TDE
            TextDisplayElement text = new TextDisplayElement("", position, fontSize, anchor, color);

            // Offset
            Vector2 offset = new Vector2(
                GetSkinnableInt(typeName + "X"),
                GetSkinnableInt(typeName + "Y"));

            text.Move(offset);

            //Add TDE
            TextElements.Add(text);
            TextElementOffsets.Add(text.Position - AnchorUtil.FindDrawablePosition(startAnchor, this));
            TextElementStartAnchors.Add(startAnchor);
        }

        #region GeSkinnable Methods
        /// <summary>
        /// Find a float from the Metadata section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        protected float GetSkinnableFloat(string key) => Skin.GetConfigFloat(Config, Section, key);

        /// <summary>
        /// Find a int from the Metadata section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        protected int GetSkinnableInt(string key) => Skin.GetConfigInt(Config, Section, key);

        /// <summary>
        /// Find an Anchor from the Metadata section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        protected Anchor GetSkinnableAnchor(string key) => Skin.GetConfigAnchor(Config, Section, key);

        /// <summary>
        /// Find a string from the Metadata section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The string of the key provided.</returns>
        protected string GetSkinnableString(string key) => Skin.GetConfigString(Config, Section, key);
        #endregion
    }
}
