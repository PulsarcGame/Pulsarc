using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Pulsarc.UI.Screens.BaseEngine;
using Pulsarc.UI.Screens.Gameplay.UI;
using Pulsarc.Utils;
using System;
using System.Collections.Generic;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class GameplayEngineView : ArcCrosshairEngineView
    {
        // UI Elements
        private List<TextDisplayElementFixedSize> uiElements = new List<TextDisplayElementFixedSize>();
        private JudgeBox judgeBox;
        private AccuracyMeter accMeter;

        private MapTimer mapTimer;

        private GameplayEngine GetGameplayEngine() => (GameplayEngine)Screen;

        /// <summary>
        /// Create the GameplayEngineView, in otherwords the UI/HUD elements during Gameplay.
        /// </summary>
        /// <param name="screen">The screen this GameplayEngineView is working with.</param>
        public GameplayEngineView(Screen screen) : base(screen) { }

        /// <summary>
        /// Initialize this GameplayEngineView with UI elements.
        /// </summary>
        public override void Init()
        {
            if (AlreadyInitialized()) { return; }

            AddTextDisplayElement("Score");
            AddTextDisplayElement("Acc");
            AddTextDisplayElement("Combo");

            SetUpJudgeBox();

            SetUpAccMeter();

            base.Init();
        }

        /// <summary>
        /// Create the Judge Box according to the skin config.
        /// </summary>
        private void SetUpJudgeBox()
        {
            Vector2 startPos = Skin.GetConfigStartPosition("gameplay", "Properties", "JudgeStartPos");
            Anchor anchor = GetSkinnablePropertyAnchor("JudgeAnchor");

            judgeBox = new JudgeBox(startPos, anchor);

            int offsetX = GetSkinnablePropertyInt("JudgeX");
            int offsetY = GetSkinnablePropertyInt("JudgeY");

            judgeBox.Move(offsetX, offsetY);
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

            accMeter = new AccuracyMeter(startPos, size, anchor);

            int offsetX = GetSkinnablePropertyInt("AccMeterX");
            int offsetY = GetSkinnablePropertyInt("AccMeterY");

            accMeter.Move(offsetX, offsetY);
        }

        internal void SetUpMapTimer()
        {
            // TODO: make fill direction skinnable
            mapTimer = new MapTimer(GetGameplayEngine().MapEndTime, FillBarDirection.UpToDown);
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
            string append = typeName == "Combo" ? "x" : (typeName == "Acc" ? "%" : "");
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

        #region GetSkinnable Methods
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
        #endregion

        /// <summary>
        /// Add a hit and its judgement.
        /// </summary>
        /// <param name="time">The time of the hit.</param>
        /// <param name="error">The error of the hit (deltaTime / audio rate)</param>
        /// <param name="judge">The base score of the judgement.</param>
        public void AddHit(double time, int error, int judge)
        {
            accMeter.AddError(time, error);
            judgeBox.Add(time, judge);
        }

        public void AddMiss(double time)
        {
            judgeBox.Add(time, 0);
        }

        // TODO: Cleanup
        public override void Update(GameTime gameTime)
        {
            // Score
            uiElements[0].Update(GetGameplayEngine().ScoreDisplay);

            // Acc
            double accuracyTotal = 0;

            List<JudgementValue> judgements = GetGameplayEngine().Judgements;

            for (int i = 0; i < judgements.Count; i++)
            {
                accuracyTotal += judgements[i].Acc;
            }

            // If no judgements have happened, 100%, otherwise, find the acc
            double value = 100d;

            int count = judgements.Count;

            if (count > 0)
            {
                value *= accuracyTotal / count;
            }

            value = Math.Round(value, 2);

            if (value < 1)
            {
                uiElements[1].Update("0" + value.ToString(uiElements[1].NumberFormat));
            }
            else
            {
                uiElements[1].Update(value);
            }

            // Combo
            uiElements[2].Update(GetGameplayEngine().Combo);

            // Update everything
            judgeBox.Update(Time);
            accMeter.Update(Time);
            mapTimer.CurrentValue = (float)Time;
        }

        /// <summary>
        /// Draw Everything
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (!GameplayEngine.Active) { return; }

            // Draw crosshair/background/arcs
            base.Draw(gameTime);

            // Draw UI
            for (int i = 0; i < uiElements.Count; i++)
            {
                uiElements[i].Draw();
            }

            judgeBox.Draw();
            accMeter.Draw();
            mapTimer.Draw();
        }

        public bool IsActive() => GameplayEngine.Active;

        public override void Destroy() => GetGameplayEngine().Reset();
    }
}
