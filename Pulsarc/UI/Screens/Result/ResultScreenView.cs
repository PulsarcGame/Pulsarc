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
        public readonly string config = "result_screen";
        public readonly string[] sections = { "Properties", "Metadata", "Judgements" };

        ResultScreen GetResultScreen() { return (ResultScreen) Screen; }

        // Buttons
        RetryButton button_retry;
        ReturnButton button_back;
        ButtonAdvanced button_advanced;

        Beatmap beatmap;

        // Play stats
        List<TextDisplayElement> playStats = new List<TextDisplayElement>();
        Grade grade;

        // Background and scorecard designs
        ResultScorecard scorecard;
        Background background;
        Background mapBackground;

        // Metadata
        List<TextDisplayElement> metadata = new List<TextDisplayElement>();

        // Judges and the TDE that tracks the amount of each
        List<KeyValuePair<Judge,TextDisplayElement>> judgements;

        HitErrorGraph hitErrorGraph;

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
            addScoreCard();

            addButtons();

            addBackgrounds(mapBackground);

            addPlayStats(accuracy, grade);

            addMetadata();

            addHitErrorGraph();

            addJudges();
        }

        private void addScoreCard()
        {
            Vector2 startPos = Skin.getConfigStartPosition(config, sections[0], "ScoreCardStartPos");
            Anchor anchor = getSkinnablePropertyAnchor("ScoreCardAnchor");

            scorecard = new ResultScorecard(startPos, anchor);

            int offsetX = getSkinnablePropertyInt("ScoreCardX");
            int offsetY = getSkinnablePropertyInt("ScoreCardY");

            scorecard.move(offsetX, offsetY);
        }

        private void addButtons()
        {
            button_back = new ReturnButton("result_button_back", AnchorUtil.FindScreenPosition(Anchor.BottomLeft));
            button_retry = new RetryButton("result_button_retry", AnchorUtil.FindScreenPosition(Anchor.BottomRight));
            button_advanced = new ButtonAdvanced(AnchorUtil.FindScreenPosition(Anchor.BottomLeft));

            // Move the advanced button to the right spot
            float width = button_advanced.Texture.Width;
            float height = button_advanced.Texture.Height;
            button_advanced.scaledMove(new Vector2(width, -height));
        }

        private void addBackgrounds(Background mapBackground)
        {
            // Skinned Background
            background = new Background("result_background");

            // Map Background
            this.mapBackground = mapBackground;

            float scale = getSkinnablePropertyFloat("MapBGScale") * Pulsarc.HeightScale;
            Vector2 startPosition = Skin.getConfigStartPosition(config, sections[0], "MapBGStartPos", scorecard);

            int offsetX = getSkinnablePropertyInt("MapBGX");
            int offsetY = getSkinnablePropertyInt("MapBGY");

            mapBackground.changePosition(startPosition);

            mapBackground.move(offsetX, offsetY);
        }

        private void addPlayStats(double accuracy, string grade)
        {
            // Grade
            Vector2 startPosition = Skin.getConfigStartPosition(config, sections[0], "GradeStartPos");
            float scale = getSkinnablePropertyFloat("GradeScale");
            Anchor anchor = getSkinnablePropertyAnchor("GradeAnchor");

            this.grade = new Grade(grade, startPosition, scale, anchor);

            int offsetX = getSkinnablePropertyInt("GradeX");
            int offsetY = getSkinnablePropertyInt("GradeY");

            this.grade.move(offsetX, offsetY);

            // TDEs
            playStats.Add(makeTextDisplayElement("Score", sections[0]));
            playStats[0].Update(GetResultScreen().display_score.ToString("#,#0"));

            playStats.Add(makeTextDisplayElement("Acc", sections[0]));
            playStats[1].Update(Math.Round(accuracy * 100, 2).ToString("#,##.00") + "%");

            playStats.Add(makeTextDisplayElement("Combo", sections[0]));
            playStats[2].Update(GetResultScreen().combo.ToString("#,#0") + "x");
        }

        private void addMetadata()
        {
            metadata.Add(makeTextDisplayElement("Title", sections[1]));
            metadata[0].Update(GetResultScreen().beatmap.Title);

            metadata.Add(makeTextDisplayElement("Artist", sections[1]));
            metadata[1].Update(GetResultScreen().beatmap.Artist);

            metadata.Add(makeTextDisplayElement("Version", sections[1]));
            metadata[2].Update(GetResultScreen().beatmap.Version);

            metadata.Add(makeTextDisplayElement("Mapper", sections[1]));
            metadata[3].Update(GetResultScreen().beatmap.Mapper);
        }

        private void addHitErrorGraph()
        {
            hitErrorGraph = new HitErrorGraph
            (
                Skin.getConfigStartPosition(config, sections[0], "HitErrorStartPos", scorecard),
                (int)(getSkinnablePropertyInt("HitErrorWidth") * Pulsarc.HeightScale),
                (int)(getSkinnablePropertyInt("HitErrorHeight") * Pulsarc.HeightScale),
                GetResultScreen().hits,
                getSkinnablePropertyAnchor("HitErrorAnchor")
            );

            int offsetX = getSkinnablePropertyInt("HitErrorX");
            int offsetY = getSkinnablePropertyInt("HitErrorY");

            hitErrorGraph.scaledMove(offsetX, offsetY);
        }

        private void addJudges()
        {
            judgements = new List<KeyValuePair<Judge, TextDisplayElement>>();

            foreach (JudgementValue judge in Judgement.judgements)
            {
                addJudgeInfo(judge.name);
            }
            foreach (KeyValuePair<Judge, TextDisplayElement> judgePair in judgements)
            {
                string name = judgePair.Value.name;
                judgePair.Value.name = "";
                judgePair.Value.Update(GetResultScreen().judges_count[name].ToString("#,#0"));
            }
        }

        /// <summary>
        /// Add the judgement and the total of that judgement to the Result Screen.
        /// </summary>
        /// <param name="name">The name of the judgement.</param>
        private void addJudgeInfo(string name)
        {
            string configName = char.ToUpper(name[0]) + name.Substring(1);
            JudgementValue judgement = Judgement.getByName(name);

            // Judge
            Vector2 position = Skin.getConfigStartPosition(config, sections[2], configName + "StartPos", scorecard);
            int offsetX = getSkinnableJudgementInt(configName + "X");
            int offsetY = getSkinnableJudgementInt(configName + "Y");
            float scale = getSkinnableJudgementFloat(configName + "Scale") * Pulsarc.HeightScale;

            Judge judge = new Judge(judgement.score, position, scale);

            judge.scaledMove(offsetX, offsetY);

            // JudgeCount
            TextDisplayElement text = makeTextDisplayElement(configName + "Count", sections[2]);
            text.name = name;
            text.color = judgement.color;

            judgements.Add(new KeyValuePair<Judge, TextDisplayElement>(judge, text));
        }

        private TextDisplayElement makeTextDisplayElement(string typeName, string section)
        {
            // Find variables for the TDE
            Vector2 position = Skin.getConfigStartPosition(config, section, typeName + "StartPos", scorecard);
            int fontSize = Skin.getConfigInt(config, section, typeName + "FontSize");
            Anchor textAnchor = Skin.getConfigAnchor(config, section, typeName + "Anchor");
            Color textColor = Color.White;

            // For judgement text, which finds colors from judgements.ini in addJudgeInfo()
            try
            {
                textColor = Skin.getConfigColor(config, section, typeName + "Color");
            }
            catch { }

            //Make TDE
            TextDisplayElement text = new TextDisplayElement("", position, fontSize, textAnchor, textColor);

            // Offset
            Vector2 offset = new Vector2(
                Skin.getConfigInt(config, section, typeName + "X"),
                Skin.getConfigInt(config, section, typeName + "Y"));

            text.scaledMove(offset);

            return text;
        }

        /// <summary>
        /// Find a float position from the Properties section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float getSkinnablePropertyFloat(string key)
        {
            return Skin.getConfigFloat(config, sections[0], key);
        }

        /// <summary>
        /// Find a float position from the Metadata section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float getSkinnableMetadataFloat(string key)
        {
            return Skin.getConfigFloat(config, sections[1], key);
        }

        /// <summary>
        /// Find a float position from the Judgements section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float getSkinnableJudgementFloat(string key)
        {
            return Skin.getConfigFloat(config, sections[2], key);
        }

        /// <summary>
        /// Find a int from the Properties section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int getSkinnablePropertyInt(string key)
        {
            return Skin.getConfigInt(config, sections[0], key);
        }

        /// <summary>
        /// Find a int from the Metadata section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int getSkinnableMetadataInt(string key)
        {
            return Skin.getConfigInt(config, sections[1], key);
        }

        /// <summary>
        /// Find a int from the Judgements section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int getSkinnableJudgementInt(string key)
        {
            return Skin.getConfigInt(config, sections[2], key);
        }

        /// <summary>
        /// Find an Anchor from the Properties section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor getSkinnablePropertyAnchor(string key)
        {
            return Skin.getConfigAnchor(config, sections[0], key);
        }

        /// <summary>
        /// Find an Anchor from the Metadata section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor getSkinnableMetadataAnchor(string key)
        {
            return Skin.getConfigAnchor(config, sections[1], key);
        }

        /// <summary>
        /// Find an Anchor from the Judgements section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor getSkinnableJudgementAnchor(string key)
        {
            return Skin.getConfigAnchor(config, sections[2], key);
        }

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

            button_advanced.Draw();
            button_back.Draw();
            button_retry.Draw();

            scorecard.Draw();

            foreach (TextDisplayElement playStat in playStats)
            {
                playStat.Draw();
            }
            grade.Draw();

            foreach (TextDisplayElement data in metadata)
            {
                data.Draw();
            }

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
            while (InputManager.keyboardPresses.Count > 0)
            {
                KeyValuePair<double, Keys> press = InputManager.keyboardPresses.Dequeue();

                if (press.Value == Keys.Escape || press.Value == Keys.Delete)
                {
                    ScreenManager.RemoveScreen(true);
                }
            }

            // See if the retry or back buttons were pressed.
            if (InputManager.isLeftClick())
            {
                Point pos = InputManager.lastMouseClick.Key.Position;
                if (button_back.clicked(pos))
                {
                    button_back.onClick();
                }
                else if (button_retry.clicked(pos))
                {
                    button_retry.onClick(beatmap);
                }
            }
        }
    }
}
