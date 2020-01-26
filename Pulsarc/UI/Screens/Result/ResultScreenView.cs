using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Skinning;
using Pulsarc.Beatmaps;
using Pulsarc.UI.Buttons;
using Pulsarc.UI.Common;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.UI.Screens.Result.UI;
using Pulsarc.Utils;
using System.Collections.Generic;
using Wobble.Screens;
using System;

namespace Pulsarc.UI.Screens.Result
{
    class ResultScreenView : ScreenView
    {
        private string Config => "result_screen";
        private string[] Sections => new string[] { "Properties", "Metadata", "Judgements" };

        private ResultScreen GetResultScreen() { return (ResultScreen)Screen; }

        // Buttons
        private RetryButton buttonRetry;
        private ReturnButton buttonBack;
        private ButtonAdvanced buttonAdvanced;

        private Beatmap beatmap;

        // Play stats
        private List<TextDisplayElement> playStats = new List<TextDisplayElement>();
        private Grade grade;

        // Background and scorecard designs
        private ResultCard scorecard;
        private Background background;
        private Background mapBackground;

        // Metadata
        private List<TextDisplayElement> metadata = new List<TextDisplayElement>();

        // Judges and the TDE that tracks the amount of each
        private List<KeyValuePair<Judge,TextDisplayElement>> judgements;

        private HitErrorGraph hitErrorGraph;

        /// <summary>
        /// ResultScreenView draws everything needed for the Result Screen.
        /// </summary>
        /// <param name="screen">The screen to draw on.</param>
        /// <param name="accuracy">The accuracy of the play.</param>
        /// <param name="grade">The grade of the play.</param>
        /// <param name="beatmap">The beatmap that was played.</param>
        public ResultScreenView(Screen screen, double accuracy, string grade, Beatmap beatmap, Background mapBackground) : base(screen)
        {
            this.beatmap = beatmap;

            // TODO: Redesign ResultScorecard to work on other aspect ratios than 16:9
            AddScoreCard();

            AddButtons();

            AddBackgrounds(mapBackground);

            AddPlayStats(accuracy, grade);

            AddMetadata();

            AddHitErrorGraph();

            AddJudges();
        }

        #region Setup Methods
        private void AddScoreCard()
        {
            Vector2 startPos = Skin.GetConfigStartPosition(Config, Sections[0], "ScoreCardStartPos");
            Anchor anchor = GetSkinnablePropertyAnchor("ScoreCardAnchor");

            scorecard = new ResultCard(startPos, anchor);

            int offsetX = GetSkinnablePropertyInt("ScoreCardX");
            int offsetY = GetSkinnablePropertyInt("ScoreCardY");

            scorecard.Move(offsetX, offsetY);
        }

        private void AddButtons()
        {
            buttonBack = new ReturnButton("result_button_back", AnchorUtil.FindScreenPosition(Anchor.BottomLeft));
            buttonRetry = new RetryButton("result_button_retry", AnchorUtil.FindScreenPosition(Anchor.BottomRight));
            buttonAdvanced = new ButtonAdvanced(AnchorUtil.FindScreenPosition(Anchor.BottomLeft));

            // Move the advanced button to the right spot
            float width = buttonAdvanced.Texture.Width;
            float height = buttonAdvanced.Texture.Height;
            buttonAdvanced.Move(new Vector2(width, -height));
        }

        private void AddBackgrounds(Background mapBackground)
        {
            // Skinned Background
            background = new Background("result_background");

            // Map Background
            this.mapBackground = mapBackground;

            float scale = GetSkinnablePropertyFloat("MapBGScale") * Pulsarc.HeightScale;
            Vector2 startPosition = Skin.GetConfigStartPosition(Config, Sections[0], "MapBGStartPos", scorecard);

            int offsetX = GetSkinnablePropertyInt("MapBGX");
            int offsetY = GetSkinnablePropertyInt("MapBGY");

            mapBackground.ChangePosition(startPosition);

            mapBackground.Move(offsetX, offsetY);
        }

        private void AddPlayStats(double accuracy, string grade)
        {
            // Grade
            Vector2 startPosition = Skin.GetConfigStartPosition(Config, Sections[0], "GradeStartPos");
            float scale = GetSkinnablePropertyFloat("GradeScale");
            Anchor anchor = GetSkinnablePropertyAnchor("GradeAnchor");

            this.grade = new Grade(grade, startPosition, scale, anchor);

            int offsetX = GetSkinnablePropertyInt("GradeX");
            int offsetY = GetSkinnablePropertyInt("GradeY");

            this.grade.Move(offsetX, offsetY);

            // TDEs
            playStats.Add(MakeTextDisplayElement("Score", Sections[0]));
            playStats[0].Update(GetResultScreen().DisplayScore.ToString("#,#0"));

            playStats.Add(MakeTextDisplayElement("Acc", Sections[0]));
            playStats[1].Update(Math.Round(accuracy * 100, 2).ToString("#,##.00") + "%");

            playStats.Add(MakeTextDisplayElement("Combo", Sections[0]));
            playStats[2].Update(GetResultScreen().Combo.ToString("#,#0") + "x");
        }

        private void AddMetadata()
        {
            metadata.Add(MakeTextDisplayElement("Title", Sections[1]));
            metadata[0].Update(GetResultScreen().Beatmap.Title);

            metadata.Add(MakeTextDisplayElement("Artist", Sections[1]));
            metadata[1].Update(GetResultScreen().Beatmap.Artist);

            metadata.Add(MakeTextDisplayElement("Version", Sections[1]));
            metadata[2].Update(GetResultScreen().Beatmap.Version);

            metadata.Add(MakeTextDisplayElement("Mapper", Sections[1]));
            metadata[3].Update(GetResultScreen().Beatmap.Mapper);
        }

        private void AddHitErrorGraph()
        {
            hitErrorGraph = new HitErrorGraph
            (
                Skin.GetConfigStartPosition(Config, Sections[0], "HitErrorStartPos", scorecard),
                (int)(GetSkinnablePropertyInt("HitErrorWidth") * Pulsarc.HeightScale),
                (int)(GetSkinnablePropertyInt("HitErrorHeight") * Pulsarc.HeightScale),
                GetResultScreen().Hits,
                GetSkinnablePropertyAnchor("HitErrorAnchor")
            );

            int offsetX = GetSkinnablePropertyInt("HitErrorX");
            int offsetY = GetSkinnablePropertyInt("HitErrorY");

            hitErrorGraph.Move(offsetX, offsetY);
        }

        private void AddJudges()
        {
            judgements = new List<KeyValuePair<Judge, TextDisplayElement>>();

            foreach (JudgementValue judge in Judgement.Judgements)
                AddJudgeInfo(judge.Name);

            foreach (KeyValuePair<Judge, TextDisplayElement> judgePair in judgements)
            {
                string name = judgePair.Value.Name;
                judgePair.Value.Name = "";
                judgePair.Value.Update(GetResultScreen().JudgesCount[name].ToString("#,#0"));
            }
        }

        /// <summary>
        /// Add the judgement and the total of that judgement to the Result Screen.
        /// </summary>
        /// <param name="name">The name of the judgement.</param>
        private void AddJudgeInfo(string name)
        {
            string configName = char.ToUpper(name[0]) + name.Substring(1);
            JudgementValue judgement = Judgement.GetJudgementValueByName(name);

            // Judge
            Vector2 position = Skin.GetConfigStartPosition(Config, Sections[2], $"{configName}StartPos", scorecard);
            int offsetX = GetSkinnableJudgementInt($"{configName}X");
            int offsetY = GetSkinnableJudgementInt($"{configName}Y");
            float scale = GetSkinnableJudgementFloat($"{configName}Scale") * Pulsarc.HeightScale;

            Judge judge = new Judge(judgement.Score, position, scale);

            judge.Move(offsetX, offsetY);

            // JudgeCount
            TextDisplayElement text = MakeTextDisplayElement($"{configName}Count", Sections[2]);
            text.Name = name;
            text.Color = judgement.Color;

            judgements.Add(new KeyValuePair<Judge, TextDisplayElement>(judge, text));
        }
        #endregion

        private TextDisplayElement MakeTextDisplayElement(string typeName, string section)
        {
            // Find variables for the TDE
            Vector2 position = Skin.GetConfigStartPosition(Config, section, $"{typeName}StartPos", scorecard);
            int fontSize = Skin.GetConfigInt(Config, section, $"{typeName}FontSize");
            Anchor textAnchor = Skin.GetConfigAnchor(Config, section, $"{typeName}Anchor");
            Color textColor = Color.White;

            // This Try Block is for the judgement text, which find their colors
            // from judgements.ini in AddJudgeInfo() instead of result_screen.ini here
            try
            {
                textColor = Skin.GetConfigColor(Config, section, $"{typeName}Color");
            }
            catch { }

            //Make TDE
            TextDisplayElement text = new TextDisplayElement("", position, fontSize, textAnchor, textColor);

            // Offset
            Vector2 offset = new Vector2(
                Skin.GetConfigInt(Config, section, $"{typeName}X"),
                Skin.GetConfigInt(Config, section, $"{typeName}Y"));

            text.Move(offset);

            return text;
        }

        #region GetConfig Methods
        /// <summary>
        /// Find a float position from the Properties section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float GetSkinnablePropertyFloat(string key)
        {
            return Skin.GetConfigFloat(Config, Sections[0], key);
        }

        /// <summary>
        /// Find a float position from the Metadata section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float GetSkinnableMetadataFloat(string key)
        {
            return Skin.GetConfigFloat(Config, Sections[1], key);
        }

        /// <summary>
        /// Find a float position from the Judgements section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float GetSkinnableJudgementFloat(string key)
        {
            return Skin.GetConfigFloat(Config, Sections[2], key);
        }

        /// <summary>
        /// Find a int from the Properties section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int GetSkinnablePropertyInt(string key)
        {
            return Skin.GetConfigInt(Config, Sections[0], key);
        }

        /// <summary>
        /// Find a int from the Metadata section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int GetSkinnableMetadataInt(string key)
        {
            return Skin.GetConfigInt(Config, Sections[1], key);
        }

        /// <summary>
        /// Find a int from the Judgements section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int GetSkinnableJudgementInt(string key)
        {
            return Skin.GetConfigInt(Config, Sections[2], key);
        }

        /// <summary>
        /// Find an Anchor from the Properties section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor GetSkinnablePropertyAnchor(string key)
        {
            return Skin.GetConfigAnchor(Config, Sections[0], key);
        }

        /// <summary>
        /// Find an Anchor from the Metadata section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor GetSkinnableMetadataAnchor(string key)
        {
            return Skin.GetConfigAnchor(Config, Sections[1], key);
        }

        /// <summary>
        /// Find an Anchor from the Judgements section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor GetSkinnableJudgementAnchor(string key)
        {
            return Skin.GetConfigAnchor(Config, Sections[2], key);
        }
        #endregion  

        public override void Destroy()
        {
            // Destroy
        }

        /// <summary>
        /// Draw everything
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public override void Draw(GameTime gameTime)
        {
            mapBackground.Draw();
            background.Draw();

            buttonAdvanced.Draw();
            buttonBack.Draw();
            buttonRetry.Draw();

            scorecard.Draw();

            foreach (TextDisplayElement playStat in playStats)
                playStat.Draw();

            grade.Draw();

            foreach (TextDisplayElement data in metadata)
                data.Draw();

            foreach (KeyValuePair<Judge, TextDisplayElement> judgePair in judgements)
            {
                judgePair.Key.Draw();
                judgePair.Value.Draw();
            }

            hitErrorGraph.Draw();
        }

        /// <summary>
        /// Detect if any buttons/relevant keybinds are being pressed.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        public override void Update(GameTime gameTime)
        {
            // If "escape" or "delete" was pressed, go back to the Song select
            while (InputManager.KeyboardPresses.Count > 0)
            {
                KeyValuePair<double, Keys> press = InputManager.KeyboardPresses.Dequeue();

                if (press.Value == Keys.Escape || press.Value == Keys.Delete)
                    ScreenManager.RemoveScreen(true);
            }

            // Were the retry or back buttons clicked?
            if (InputManager.IsLeftClick())
            {
                Point pos = InputManager.LastMouseClick.Key.Position;

                if (buttonBack.Hovered(pos))
                    buttonBack.OnClick();
                else if (buttonRetry.Hovered(pos))
                    buttonRetry.OnClick(beatmap);
            }
        }
    }
}
