using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Beatmaps;
using Pulsarc.Skinning;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Utils;
using Pulsarc.Utils.Maths;
using System;
using System.Collections.Generic;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    public class BeatmapCard : Card
    {
        public static readonly Texture2D StaticTexture = Skin.assets["beatmap_card"];

        public Beatmap beatmap;

        private bool isClicked = false;
        
        // How far this card will extend when selected
        private static float clickedDistance = Skin.getConfigFloat("song_select", "Properties", "BeatmapCardSelectOffset");
        // What direction this card will extend in
        private static string clickedDirection = Skin.getConfigString("song_select", "Properties", "BeatmapCardSelectDirection");

        private float currentClickDistance = 0f;
        private float lastClickDistance = 0f;

        // The difficulty of the map represented as a bar
        BeatmapCardDifficulty diffBar;

        // Metadata
        List<TextDisplayElement> metadata = new List<TextDisplayElement>();

        /// <summary>
        /// A card displayed on the Song Select Screen. When clicked it loads the beatmap associated with this card.
        /// </summary>
        /// <param name="beatmap">The beatmap associated with this card.</param>
        /// <param name="truePosition">The position of the card.</param>
        /// <param name="size">The size of the card.</param>
        public BeatmapCard(Beatmap beatmap, Vector2 position, Vector2 size, Anchor anchor = Anchor.CenterRight) : base(position, size, anchor)
        {
            this.beatmap = beatmap;

            float percent = (float) (beatmap.Difficulty / 10f);
            diffBar = new BeatmapCardDifficulty(new Vector2(AnchorUtil.FindScreenPosition(Anchor.CenterRight).X, truePosition.Y), percent <= 10 ? percent >= 0 ? percent : 0 : 10);
            diffBar.scaledMove(-10, 165); // TODO: Make these values customizeable for custom skins.

            addMetadataTDE("Title");
            metadata[0].Update(beatmap.Title);

            addMetadataTDE("Artist");
            metadata[1].Update(beatmap.Artist);

            addMetadataTDE("Version");
            metadata[2].Update(beatmap.Version);

            addMetadataTDE("Mapper");
            metadata[3].Update(beatmap.Mapper);

            addMetadataTDE("Difficulty");
            metadata[4].Update(string.Format("{0:0.00}", beatmap.Difficulty));
        }
        
        /// <summary>
        /// Add metadata TextDisplayElement to the Beatmap card, using the config to
        /// determine their positioning and other properties.
        /// </summary>
        /// <param name="typeName">The "typeName" of the button, or the prefix in the config.</param>
        private void addMetadataTDE(string typeName)
        {
            // Find variables for TDE
            Vector2 position = Skin.getStartPosition("song_select", "Metadata", typeName + "StartPos", this); // Vector2 position;
            int fontSize = getSkinnableMetadataInt(typeName + "FontSize");
            Anchor anchor = getSkinnableMetadataAnchor(typeName + "Anchor"); // Anchor textAnchor;
            Color color = Skin.getColor("song_select", "Metadata", typeName + "Color"); // Color textColor;

            // Make TDE
            TextDisplayElement text = new TextDisplayElement("", position, fontSize, anchor, color);

            // Offset
            Vector2 offset = new Vector2(
                getSkinnableMetadataInt(typeName + "X"),
                getSkinnableMetadataInt(typeName + "Y"));

            text.move(offset);

            //Add TDE
            metadata.Add(text);
        }

        public override void Draw()
        {
            base.Draw();
            diffBar.Draw();

            foreach (TextDisplayElement tde in metadata)
            {
                tde.Draw();
            }
        }

        /// <summary>
        /// The card moving in and out depending on its selected state.
        /// </summary>
        public void adjustClickDistance()
        {
            // If clicked, smoothly move to the clicked distance
            if (isClicked && currentClickDistance < clickedDistance)
            {
                currentClickDistance = PulsarcMath.Lerp(currentClickDistance, clickedDistance, (float)PulsarcTime.DeltaTime / 100f);
            }
            // Else if not clicked and currentClickDistacne is greater than 0, smoothly move to 0
            else if (!isClicked && currentClickDistance > 0)
            {
                currentClickDistance = PulsarcMath.Lerp(currentClickDistance, 0, (float)PulsarcTime.DeltaTime / 100f);
            }
            // Else, end the method.
            else
            {
                return;
            }

            float diff = lastClickDistance - currentClickDistance;
            lastClickDistance = currentClickDistance;

            switch (clickedDirection)
            {
                case "Left":
                    scaledMove(new Vector2(diff, 0));
                    break;
                case "Right":
                    scaledMove(new Vector2(-diff, 0));
                    break;
                case "Up":
                    scaledMove(new Vector2(0, diff));
                    break;
                case "Down":
                    scaledMove(new Vector2(0, -diff));
                    break;
                default:
                    goto case "Left";
            }
        }

        /// <summary>
        /// When clicked, start playing the beatmap.
        /// </summary>
        public void onClick()
        {
            if (isClicked)
            {
                GameplayEngine gameplay = new GameplayEngine();
                ScreenManager.AddScreen(gameplay);
                gameplay.Init(beatmap);
            } else
            {
                string path = beatmap.getFullAudioPath();
                if (AudioManager.song_path != path)
                {
                    AudioManager.song_path = beatmap.getFullAudioPath();
                    AudioManager.audioRate = Config.getFloat("Gameplay", "SongRate");
                    AudioManager.StartLazyPlayer();

                    if(beatmap.PreviewTime != 0)
                    {
                        AudioManager.deltaTime(beatmap.PreviewTime);
                    }

                    Console.WriteLine("Now Playing: " + AudioManager.song_path);
                }
            }
        }

        public void setClicked(bool set)
        {
            isClicked = set;
        }

        /// <summary>
        /// Move this card and all related drawables by the provided delta.
        /// </summary>
        /// <param name="delta">How much to move from the current position.</param>
        public override void move(Vector2 delta, bool scaledPositioning = true)
        {
            foreach (TextDisplayElement tde in metadata)
            {
                tde.move(delta, scaledPositioning);
            }
            diffBar.move(delta, scaledPositioning);
            base.move(delta, scaledPositioning);
        }

        public override void scaledMove(Vector2 delta)
        {
            foreach (TextDisplayElement tde in metadata)
            {
                tde.scaledMove(delta);
            }
            diffBar.scaledMove(delta);
            base.scaledMove(delta);
        }

        /// <summary>
        /// Find a float from the Metadata section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float getSkinnableMetadataFloat(string key)
        {
            return Skin.getConfigFloat("song_select", "Metadata", key);
        }

        /// <summary>
        /// Find a int from the Metadata section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int getSkinnableMetadataInt(string key)
        {
            return Skin.getConfigInt("song_select", "Metadata", key);
        }

        /// <summary>
        /// Find an Anchor from the Metadata section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor getSkinnableMetadataAnchor(string key)
        {
            return Skin.getConfigAnchor("song_select", "Metadata", key);
        }

        /// <summary>
        /// Find a string from the Metadata section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The string of the key provided.</returns>
        private string getSkinnableMetadataString(string key)
        {
            return Skin.getConfigString("song_select", "Metadata", key);
        }
    }
}
