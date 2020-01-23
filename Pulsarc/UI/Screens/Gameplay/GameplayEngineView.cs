using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Pulsarc.UI.Screens.Gameplay.UI;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class GameplayEngineView : ScreenView
    {
        // UI Elements
        private List<TextDisplayElementFixedSize> uiElements = new List<TextDisplayElementFixedSize>();
        private JudgeBox _judgeBox;
        private AccuracyMeter _accMeter;
        private Crosshair _crosshair;

        private Background _background;

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
            _crosshair = GetGameplayEngine().Crosshair;

            AddTextDisplayElement("Score");
            AddTextDisplayElement("Acc");
            AddTextDisplayElement("Combo");

            SetUpJudgeBox();

            SetUpAccMeter();

            _background = GetGameplayEngine().Background;
        }

        /// <summary>
        /// Create the Judge Box according to the skin config.
        /// </summary>
        private void SetUpJudgeBox()
        {
            Vector2 startPos = Skin.GetConfigStartPosition("gameplay", "Properties", "JudgeStartPos");
            Anchor anchor = GetSkinnablePropertyAnchor("JudgeAnchor");

            _judgeBox = new JudgeBox(startPos, anchor);

            int offsetX = GetSkinnablePropertyInt("JudgeX");
            int offsetY = GetSkinnablePropertyInt("JudgeY");

            _judgeBox.Move(offsetX, offsetY);
        }

        /// <summary>
        /// Create the Accuracy Meter according to the skin config.
        /// </summary>
        private void SetUpAccMeter()
        {
            Vector2 startPos = Skin.GetConfigStartPosition("gameplay", "Properties", "AccMeterStartPos");
            Anchor anchor = GetSkinnablePropertyAnchor("AccMeterAnchor");

            Vector2 size = new Vector2(
                GetSkinnablePropertyInt("AccMeterWidth"),
                GetSkinnablePropertyInt("AccMeterHeight")
            );

            _accMeter = new AccuracyMeter(startPos, size, anchor);

            int offsetX = GetSkinnablePropertyInt("AccMeterX");
            int offsetY = GetSkinnablePropertyInt("AccMeterY");

            _accMeter.Move(offsetX, offsetY);
        }

        /// <summary>
        /// Add TDEs to the gameplay engine, using the config to
        /// determine their positioning and other properties.
        /// </summary>
        /// <param name="typeName">The "typeName" of the button, or the prefix in the config.</param>
        private void AddTextDisplayElement(string typeName)
        {
            // Find variables for TDE
            Vector2 position = Skin.GetConfigStartPosition("gameplay", "Properties", $"{typeName}StartPos"); // Vector2 position;
            int fontSize = GetSkinnablePropertyInt($"{typeName}TextFontSize");
            Anchor textAnchor = GetSkinnablePropertyAnchor($"{typeName}Anchor"); // Anchor textAnchor;
            Color textColor = Skin.GetConfigColor("gameplay", "Properties", $"{typeName}TextColor"); // Color textColor;

            // Make TDE
            // If this is the combo, change append to "x", if acc change it to "%"
            string append = typeName == "Combo" ? "x" : typeName == "Acc" ? "%" : "";
            // If this is the acc, change numberFormat to "#,##.00" 
            string numberFormat = typeName.Equals("Acc") ? "#,##.00" : "#,#0";

            TextDisplayElementFixedSize text = new TextDisplayElementFixedSize("", position, append, fontSize, textAnchor, textColor);
            text.NumberFormat = numberFormat;

            // Offset
            Vector2 offset = new Vector2(
                GetSkinnablePropertyInt($"{typeName}X"),
                GetSkinnablePropertyInt($"{typeName}Y"));

            text.Move(offset);

            // Add
            uiElements.Add(text);
        }

        /// <summary>
        /// Find a float from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float GetSkinnablePropertyFloat(string key)
        {
            return Skin.GetConfigFloat("gameplay", "Properties", key);
        }

        /// <summary>
        /// Find a int from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int GetSkinnablePropertyInt(string key)
        {
            return Skin.GetConfigInt("gameplay", "Properties", key);
        }

        /// <summary>
        /// Find an Anchor from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor GetSkinnablePropertyAnchor(string key)
        {
            return Skin.GetConfigAnchor("gameplay", "Properties", key);
        }

        /// <summary>
        /// Find a string from the Properties section of the Song Select config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The string of the key provided.</returns>
        private string GetSkinnablePropertyString(string key)
        {
            return Skin.GetConfigString("gameplay", "Properties", key);
        }

        /// <summary>
        /// Add a hit and its judgement.
        /// </summary>
        /// <param name="time">The time of the hit.</param>
        /// <param name="error">The error of the hit (deltaTime / audio rate)</param>
        /// <param name="judge">The base score of the judgement.</param>
        public void AddHit(double time, int error, int judge)
        {
            _accMeter.AddError(time, error);
            AddJudge(time, judge);
        }

        /// <summary>
        /// Add a judgement.
        /// </summary>
        /// <param name="time">The time of the judgement.</param>
        /// <param name="judge">The base score of the judgement.</param>
        public void AddJudge(double time, int judge)
        {
            _judgeBox.Add(time, judge);
        }

        public override void Update(GameTime gameTime)
        {
            // Score
            uiElements[0].Update(GetGameplayEngine().ScoreDisplay);

            // Acc
            double accuracyTotal = GetGameplayEngine().Judgements.Sum(judge => judge.Acc);

            // If no judgements have happened, 100%, otherwise, find the acc
            double value = 100d;

            int count = GetGameplayEngine().Judgements.Count;

            if (count > 0)
                value *= accuracyTotal / count;

            value = Math.Round(value, 2);

            if (value < 1)
                uiElements[1].Update("0" + value.ToString(uiElements[1].NumberFormat));
            else
                uiElements[1].Update(value);

            // Combo
            uiElements[2].Update(GetGameplayEngine().Combo);

            _judgeBox.Update(GetGameplayEngine().Time);
            _accMeter.Update(GetGameplayEngine().Time);
        }

        /// <summary>
        /// Draw Everything
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (!GameplayEngine.Active)
                return;

            // Don't bother drawing the background if dim is 100%
            if (_background.Dimmed && _background.DimTexture.Opacity != 1f || !_background.Dimmed)
                _background.Draw();

            _crosshair.Draw();
            DrawArcs();

            foreach (TextDisplayElementFixedSize tdef in uiElements)
                tdef.Draw();

            _judgeBox.Draw();
            _accMeter.Draw();
        }

        private void DrawArcs()
        {
            // Go through each key
            for (int i = 0; i < GetGameplayEngine().KeyCount; i++)
            {
                var skip = false;

                // Go through the arcs in each column
                for (int k = 0; k < GetGameplayEngine().Columns[i].UpdateHitObjects.Count && !skip; k++)
                {
                    // If the arc is on screen, draw it.
                    if (GetGameplayEngine().Columns[i].UpdateHitObjects[k].Value.IsSeen())
                        GetGameplayEngine().Columns[i].UpdateHitObjects[k].Value.Draw();

                    // If the arc is inside the "IgnoreTime" window, stop bothering to
                    // look at the rest of the arcs in this column.
                    if (GetGameplayEngine().Columns[i].UpdateHitObjects[k].Key - GetGameplayEngine().IgnoreTime > GetGameplayEngine().Time)
                        skip = true;
                }
            }
        }

        public bool IsActive()
        {
            return GameplayEngine.Active;
        }

        public override void Destroy()
        {
            GetGameplayEngine().Reset();
        }
    }
}
