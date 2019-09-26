using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Pulsarc.UI.Common;
using Pulsarc.UI.Screens.Gameplay.UI;
using System;
using System.Collections.Generic;
using Wobble.Logging;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class GameplayEngineView : ScreenView
    {

        // UI Elements
        List<TextDisplayElementFixedSize> uiElements = new List<TextDisplayElementFixedSize>();

        JudgeBox judgeBox;
        AccuracyMeter accMeter;
        public Crosshair crosshair;

        Background background;

        private GameplayEngine GetGameplayEngine() { return (GameplayEngine)Screen; }

        /// <summary>
        /// Create the GameplayEngineView, in otherwords the UI/HUD elements during Gameplay.
        /// </summary>
        /// <param name="screen">The screen this GameplayEngineView is working with.</param>
        public GameplayEngineView(Screen screen) : base(screen) { }

        /// <summary>
        /// Initialize this GameplayEngineView with new UI elements.
        /// </summary>
        public void Init()
        {
            // Initialize UI depending on skin config
            crosshair = GetGameplayEngine().crosshair;

            addTextDisplayElement("Score");
            addTextDisplayElement("Acc");
            addTextDisplayElement("Combo");

            setUpJudgeBox();

            setUpAccMeter();

            background = GetGameplayEngine().background;
        }

        private void setUpJudgeBox()
        {
            Vector2 startPos = Skin.getConfigStartPosition("gameplay", "Properties", "JudgeStartPos");
            Anchor anchor = getSkinnablePropertyAnchor("JudgeAnchor");

            judgeBox = new JudgeBox(startPos, anchor);

            int offsetX = getSkinnablePropertyInt("JudgeX");
            int offsetY = getSkinnablePropertyInt("JudgeY");

            judgeBox.move(offsetX, offsetY);
        }

        private void setUpAccMeter()
        {
            Vector2 startPos = Skin.getConfigStartPosition("gameplay", "Properties", "AccMeterStartPos");
            Anchor anchor = getSkinnablePropertyAnchor("AccMeterAnchor");

            Vector2 size = new Vector2(
                getSkinnablePropertyInt("AccMeterWidth"),
                getSkinnablePropertyInt("AccMeterHeight")
            );

            accMeter = new AccuracyMeter(startPos, size, anchor);

            int offsetX = getSkinnablePropertyInt("AccMeterX");
            int offsetY = getSkinnablePropertyInt("AccMeterY");

            accMeter.move(offsetX, offsetY);
        }

        /// <summary>
        /// Add a navigation button to the main menu, using the config to
        /// determine their positioning and other properties.
        /// </summary>
        /// <param name="typeName">The "typeName" of the button, or the prefix in the config.</param>
        private void addTextDisplayElement(string typeName)
        {
            // Find variables for TDE
            Vector2 position = Skin.getConfigStartPosition("gameplay", "Properties", typeName + "StartPos"); // Vector2 position;
            int fontSize = getSkinnablePropertyInt(typeName + "TextFontSize");
            Anchor textAnchor = getSkinnablePropertyAnchor(typeName + "Anchor"); // Anchor textAnchor;
            Color textColor = Skin.getConfigColor("gameplay", "Properties", typeName + "TextColor"); // Color textColor;

            // Make TDE
            // If this is the combo, change append to "x", if acc change it to "%"
            string append = typeName == "Combo" ? "x" : (typeName == "Acc" ? "%" : "");
            // If this is the acc, change numberFormat to "#,##.00" 
            string numberFormat = typeName.Equals("Acc") ? "#,##.00" : "#,#0";

            TextDisplayElementFixedSize text = new TextDisplayElementFixedSize("", position, append, fontSize, textAnchor, textColor);
            text.numberFormat = numberFormat;

            // Offset
            Vector2 offset = new Vector2(
                getSkinnablePropertyInt(typeName + "X"),
                getSkinnablePropertyInt(typeName + "Y"));

            text.move(offset);

            // Add
            uiElements.Add(text);
        }

        /// <summary>
        /// Find a float from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float getSkinnablePropertyFloat(string key)
        {
            return Skin.getConfigFloat("gameplay", "Properties", key);
        }

        /// <summary>
        /// Find a int from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int getSkinnablePropertyInt(string key)
        {
            return Skin.getConfigInt("gameplay", "Properties", key);
        }

        /// <summary>
        /// Find an Anchor from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor getSkinnablePropertyAnchor(string key)
        {
            return Skin.getConfigAnchor("gameplay", "Properties", key);
        }

        /// <summary>
        /// Find a string from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The string of the key provided.</returns>
        private string getSkinnablePropertyString(string key)
        {
            return Skin.getConfigString("gameplay", "Properties", key);
        }

        /// <summary>
        /// Add a hit and its judgement.
        /// </summary>
        /// <param name="time">The time of the hit.</param>
        /// <param name="error">The error of the hit (deltaTime / audio rate)</param>
        /// <param name="judge">The base score of the judgement.</param>
        public void addHit(double time, int error, int judge)
        {
            accMeter.addError(time, error);
            addJudge(time, judge);
        }

        /// <summary>
        /// Add a judgement.
        /// </summary>
        /// <param name="time">The time of the judgement.</param>
        /// <param name="judge">The base score of the judgement.</param>
        public void addJudge(double time, int judge)
        {
            judgeBox.Add(time, judge);
        }

        public override void Update(GameTime gameTime)
        {
            // Score
            uiElements[0].Update(GetGameplayEngine().score_display);

            // Acc
            double accuracyTotal = 0;
            foreach (JudgementValue judge in GetGameplayEngine().judgements)
            {
                accuracyTotal += judge.acc;
            }

            // If no judgements have happened, 100%, otherwise, find the acc
            double value = 100d;

            int count = GetGameplayEngine().judgements.Count;
            if (count > 0)
            {
                value *= accuracyTotal / count;
            }

            value = Math.Round(value, 2);

            if (value < 1)
            {
                uiElements[1].Update("0" + value.ToString(uiElements[1].numberFormat));
            }
            else
            {
                uiElements[1].Update(value);
            }

            // Combo
            uiElements[2].Update(GetGameplayEngine().combo);

            judgeBox.Update(GetGameplayEngine().time);
            accMeter.Update(GetGameplayEngine().time);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!GameplayEngine.active) return;

            // Draw everything

            // Don't bother drawing the background if dim is 100%
            if (background.dim && background.dimTexture.opacity != 1f || !background.dim)
            {
                background.Draw();
            }

            crosshair.Draw();
            drawArcs();

            foreach (TextDisplayElementFixedSize tdef in uiElements)
            {
                tdef.Draw();
            }

            judgeBox.Draw();
            accMeter.Draw();
        }

        private void drawArcs()
        {
            bool skip;
            for (int i = 0; i < GetGameplayEngine().keys; i++)
            {
                skip = false;
                for (int k = 0; k < GetGameplayEngine().columns[i].updateHitObjects.Count && !skip; k++)
                {

                    if (GetGameplayEngine().columns[i].updateHitObjects[k].Value.IsSeen())
                    {
                        GetGameplayEngine().columns[i].updateHitObjects[k].Value.Draw();
                    }
                    if (GetGameplayEngine().columns[i].updateHitObjects[k].Key - GetGameplayEngine().msIgnore > GetGameplayEngine().time)
                    {
                        skip = true;
                    }
                }
            }
        }

        public bool isActive()
        {
            return GameplayEngine.active;
        }

        public override void Destroy()
        {
            GetGameplayEngine().Reset();
        }
    }
}
