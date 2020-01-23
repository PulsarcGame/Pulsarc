using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Beatmaps;
using Pulsarc.Skinning;
using Pulsarc.UI.Buttons;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.UI.Screens.Result.UI;
using Pulsarc.Utils;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Result
{
    class ResultScreenView : ScreenView
    {
        private string Config => "result_screen";
        private string[] Sections => new[] { "Properties", "Metadata", "Judgements" };

        private ResultScreen GetResultScreen() { return (ResultScreen)Screen; }

        // Buttons
        private RetryButton _buttonRetry;
        private ReturnButton _buttonBack;
        private ButtonAdvanced _buttonAdvanced;

        private readonly Beatmap _beatmap;

        // Play stats
        private readonly List<TextDisplayElement> _playStats = new List<TextDisplayElement>();
        private Grade _grade;

        // Background and scorecard designs
        private ResultCard _scorecard;
        private Background _background;
        private Background _mapBackground;

        // Metadata
        private readonly List<TextDisplayElement> _metadata = new List<TextDisplayElement>();

        // Judges and the TDE that tracks the amount of each
        private List<KeyValuePair<Judge,TextDisplayElement>> _judgements;

        private HitErrorGraph _hitErrorGraph;

        /// <summary>
        /// ResultScreenView draws everything needed for the Result Screen.
        /// </summary>
        /// <param name="screen">The screen to draw on.</param>
        /// <param name="accuracy">The accuracy of the play.</param>
        /// <param name="grade">The grade of the play.</param>
        /// <param name="beatmap">The beatmap that was played.</param>
        /// <param name="mapBackground"></param>
        public ResultScreenView(Screen screen, double accuracy, string grade, Beatmap beatmap, Background mapBackground) : base(screen)
        {
            _beatmap = beatmap;

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

            _scorecard = new ResultCard(startPos, anchor);

            int offsetX = GetSkinnablePropertyInt("ScoreCardX");
            int offsetY = GetSkinnablePropertyInt("ScoreCardY");

            _scorecard.Move(offsetX, offsetY);
        }

        private void AddButtons()
        {
            _buttonBack = new ReturnButton("result_button_back", AnchorUtil.FindScreenPosition(Anchor.BottomLeft));
            _buttonRetry = new RetryButton("result_button_retry", AnchorUtil.FindScreenPosition(Anchor.BottomRight));
            _buttonAdvanced = new ButtonAdvanced(AnchorUtil.FindScreenPosition(Anchor.BottomLeft));

            // Move the advanced button to the right spot
            float width = _buttonAdvanced.Texture.Width;
            float height = _buttonAdvanced.Texture.Height;
            _buttonAdvanced.Move(new Vector2(width, -height));
        }

        private void AddBackgrounds(Background mapBackground)
        {
            // Skinned Background
            _background = new Background("result_background");

            // Map Background
            _mapBackground = mapBackground;

            float scale = GetSkinnablePropertyFloat("MapBGScale") * Pulsarc.HeightScale;
            Vector2 startPosition = Skin.GetConfigStartPosition(Config, Sections[0], "MapBGStartPos", _scorecard);

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

            _grade = new Grade(grade, startPosition, scale, anchor);

            int offsetX = GetSkinnablePropertyInt("GradeX");
            int offsetY = GetSkinnablePropertyInt("GradeY");

            _grade.Move(offsetX, offsetY);

            // TDEs
            _playStats.Add(MakeTextDisplayElement("Score", Sections[0]));
            _playStats[0].Update(GetResultScreen().DisplayScore.ToString("#,#0"));

            _playStats.Add(MakeTextDisplayElement("Acc", Sections[0]));
            _playStats[1].Update(Math.Round(accuracy * 100, 2).ToString("#,##.00") + "%");

            _playStats.Add(MakeTextDisplayElement("Combo", Sections[0]));
            _playStats[2].Update(GetResultScreen().Combo.ToString("#,#0") + "x");
        }

        private void AddMetadata()
        {
            _metadata.Add(MakeTextDisplayElement("Title", Sections[1]));
            _metadata[0].Update(GetResultScreen().Beatmap.Title);

            _metadata.Add(MakeTextDisplayElement("Artist", Sections[1]));
            _metadata[1].Update(GetResultScreen().Beatmap.Artist);

            _metadata.Add(MakeTextDisplayElement("Version", Sections[1]));
            _metadata[2].Update(GetResultScreen().Beatmap.Version);

            _metadata.Add(MakeTextDisplayElement("Mapper", Sections[1]));
            _metadata[3].Update(GetResultScreen().Beatmap.Mapper);
        }

        private void AddHitErrorGraph()
        {
            _hitErrorGraph = new HitErrorGraph
            (
                Skin.GetConfigStartPosition(Config, Sections[0], "HitErrorStartPos", _scorecard),
                (int)(GetSkinnablePropertyInt("HitErrorWidth") * Pulsarc.HeightScale),
                (int)(GetSkinnablePropertyInt("HitErrorHeight") * Pulsarc.HeightScale),
                GetResultScreen().Hits,
                GetSkinnablePropertyAnchor("HitErrorAnchor")
            );

            int offsetX = GetSkinnablePropertyInt("HitErrorX");
            int offsetY = GetSkinnablePropertyInt("HitErrorY");

            _hitErrorGraph.Move(offsetX, offsetY);
        }

        private void AddJudges()
        {
            _judgements = new List<KeyValuePair<Judge, TextDisplayElement>>();

            foreach (JudgementValue judge in Judgement.Judgements)
                AddJudgeInfo(judge.Name);

            foreach (KeyValuePair<Judge, TextDisplayElement> judgePair in _judgements)
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
            Vector2 position = Skin.GetConfigStartPosition(Config, Sections[2], $"{configName}StartPos", _scorecard);
            int offsetX = GetSkinnableJudgementInt($"{configName}X");
            int offsetY = GetSkinnableJudgementInt($"{configName}Y");
            float scale = GetSkinnableJudgementFloat($"{configName}Scale") * Pulsarc.HeightScale;

            Judge judge = new Judge(judgement.Score, position, scale);

            judge.Move(offsetX, offsetY);

            // JudgeCount
            TextDisplayElement text = MakeTextDisplayElement($"{configName}Count", Sections[2]);
            text.Name = name;
            text.Color = judgement.Color;

            _judgements.Add(new KeyValuePair<Judge, TextDisplayElement>(judge, text));
        }
        #endregion

        private TextDisplayElement MakeTextDisplayElement(string typeName, string section)
        {
            // Find variables for the TDE
            Vector2 position = Skin.GetConfigStartPosition(Config, section, $"{typeName}StartPos", _scorecard);
            int fontSize = Skin.GetConfigInt(Config, section, $"{typeName}FontSize");
            Anchor textAnchor = Skin.GetConfigAnchor(Config, section, $"{typeName}Anchor");
            Color textColor = Color.White;

            // This Try Block is for the judgement text, which find their colors
            // from judgements.ini in AddJudgeInfo() instead of result_screen.ini here
            try
            {
                textColor = Skin.GetConfigColor(Config, section, $"{typeName}Color");
            }
            catch
            {
                // ignored
            }

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
            _mapBackground.Draw();
            _background.Draw();

            _buttonAdvanced.Draw();
            _buttonBack.Draw();
            _buttonRetry.Draw();

            _scorecard.Draw();

            foreach (TextDisplayElement playStat in _playStats)
                playStat.Draw();

            _grade.Draw();

            foreach (TextDisplayElement data in _metadata)
                data.Draw();

            foreach (KeyValuePair<Judge, TextDisplayElement> judgePair in _judgements)
            {
                judgePair.Key.Draw();
                judgePair.Value.Draw();
            }

            _hitErrorGraph.Draw();
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

                if (_buttonBack.Hovered(pos))
                    _buttonBack.OnClick();
                else if (_buttonRetry.Hovered(pos))
                    _buttonRetry.OnClick(_beatmap);
            }
        }
    }
}
