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
using Wobble.Input;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Result
{
    class ResultScreenView : ScreenView
    {
        ResultScreen GetResultScreen() { return (ResultScreen) Screen; }

        // Buttons
        RetryButton button_retry;
        ReturnButton button_back;
        ButtonAdvanced button_advanced;

        Beatmap beatmap;

        // Play stats
        Accuracy accuracy;
        Score score;
        Combo combo;

        Grade grade;

        // Background and scorecard designs
        Scorecard scorecard;
        Background background;
        Background mapBackground;

        // Metadata
        Title title;
        Artist artist;
        Version version;
        Mapper mapper;

        List<KeyValuePair<Judge,JudgeCount>> judgements;
    
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

            scorecard = new Scorecard();

            addButtons();

            addBackgrounds(mapBackground);

            addPlayStats(accuracy, grade);

            addMetadata();

            addHitErrorGraph();

            addJudges();
        }

        private void addButtons()
        {
            button_advanced = new ButtonAdvanced(AnchorUtil.FindScreenPosition(Anchor.BottomLeft));
            float width = button_advanced.currentSize.X;
            float height = button_advanced.currentSize.Y;
            button_advanced.move(new Vector2(width, -height));

            button_back = new ReturnButton("result_button_back", AnchorUtil.FindScreenPosition(Anchor.BottomLeft));
            button_retry = new RetryButton("result_button_retry", AnchorUtil.FindScreenPosition(Anchor.BottomRight));
        }

        private void addBackgrounds(Background mapBackground)
        {
            background = new Background("result_background");

            this.mapBackground = mapBackground;
            mapBackground.move(new Vector2(Pulsarc.CurrentWidth / 10, 0));
        }

        private void addPlayStats(double accuracy, string grade)
        {
            this.grade = new Grade(grade, new Vector2(getSkinnablePropertyFloat("GradeX"), getSkinnablePropertyFloat("GradeY")), getSkinnablePropertyFloat("GradeScale"));

            score = new Score(new Vector2(getSkinnablePropertyFloat("ScoreX"), getSkinnablePropertyFloat("ScoreY")), new Color(74, 245, 254), getSkinnablePropertyInt("ScoreSize"), getSkinnablePropertyAnchor("ScoreAnchor"));
            combo = new Combo(new Vector2(getSkinnablePropertyFloat("ComboX"), getSkinnablePropertyFloat("ComboY")), new Color(74, 245, 254), getSkinnablePropertyInt("ComboSize"), getSkinnablePropertyAnchor("ComboAnchor"));
            this.accuracy = new Accuracy(new Vector2(getSkinnablePropertyFloat("AccuracyX"), getSkinnablePropertyFloat("AccuracyY")), new Color(74, 245, 254), getSkinnablePropertyInt("AccuracySize"), getSkinnablePropertyAnchor("AccuracyAnchor"));

            this.accuracy.Update(accuracy);
            combo.Update(GetResultScreen().combo);
            score.Update(GetResultScreen().display_score);
        }

        private void addMetadata()
        {
            title = new Title(new Vector2(getSkinnablePropertyFloat("TitleX"), getSkinnablePropertyFloat("TitleY")), getSkinnablePropertyInt("TitleSize"), getSkinnablePropertyAnchor("TitleAnchor"));
            artist = new Artist(new Vector2(getSkinnablePropertyFloat("ArtistX"), getSkinnablePropertyFloat("ArtistY")), getSkinnablePropertyInt("ArtistSize"), getSkinnablePropertyAnchor("ArtistAnchor"));
            version = new Version(new Vector2(getSkinnablePropertyFloat("VersionX"), getSkinnablePropertyFloat("VersionY")), getSkinnablePropertyInt("VersionSize"), getSkinnablePropertyAnchor("VersionAnchor"));
            mapper = new Mapper(new Vector2(getSkinnablePropertyFloat("MapperX"), getSkinnablePropertyFloat("MapperY")), new Color(74, 245, 254), getSkinnablePropertyInt("MapperSize"), getSkinnablePropertyAnchor("MapperAnchor"));
            
            title.Update(GetResultScreen().beatmap.Title);
            artist.Update(GetResultScreen().beatmap.Artist);
            version.Update(GetResultScreen().beatmap.Version);
            mapper.Update(GetResultScreen().beatmap.Mapper);
        }

        private void addHitErrorGraph()
        {
            hitErrorGraph = new HitErrorGraph(
                new Vector2(getSkinnablePropertyFloat("HitErrorX")
                            , getSkinnablePropertyFloat("HitErrorY"))
               , (int)(getSkinnablePropertyFloat("HitErrorWidth") / 1920f * Pulsarc.getDimensions().X)
               , (int)(getSkinnablePropertyFloat("HitErrorHeight") / 1080f * Pulsarc.getDimensions().Y)
               , GetResultScreen().hits);
        }

        private void addJudges()
        {
            judgements = new List<KeyValuePair<Judge, JudgeCount>>();

            foreach (JudgementValue judge in Judgement.judgements)
            {
                addJudgeInfo(judge.name);
            }
            foreach (KeyValuePair<Judge, JudgeCount> judgePair in judgements)
            {
                judgePair.Value.Update(GetResultScreen().judges_count[judgePair.Value.name]);
            }
        }

        /// <summary>
        /// Find a float position from the Properties section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float getSkinnablePropertyFloat(string key)
        {
            return Skin.getConfigFloat("result_screen", "Properties", key);
        }

        /// <summary>
        /// Find a float position from the Metadata section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float getSkinnableMetadataFloat(string key)
        {
            return Skin.getConfigFloat("result_screen", "Metadata", key);
        }

        /// <summary>
        /// Find a float position from the Grades section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float getSkinnableGradeFloat(string key)
        {
            return Skin.getConfigFloat("result_screen", "Grades", key);
        }

        /// <summary>
        /// Find a int from the Properties section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int getSkinnablePropertyInt(string key)
        {
            return Skin.getConfigInt("result_screen", "Properties", key);
        }

        /// <summary>
        /// Find a int from the Metadata section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int getSkinnableMetadataInt(string key)
        {
            return Skin.getConfigInt("result_screen", "Metadata", key);
        }

        /// <summary>
        /// Find a int from the Grades section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int getSkinnableGradeInt(string key)
        {
            return Skin.getConfigInt("result_screen", "Grades", key);
        }

        /// <summary>
        /// Find an Anchor from the Properties section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor getSkinnablePropertyAnchor(string key)
        {
            return Skin.getConfigAnchor("result_screen", "Properties", key);
        }

        /// <summary>
        /// Find an Anchor from the Metadata section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor getSkinnableMetadataAnchor(string key)
        {
            return Skin.getConfigAnchor("result_screen", "Metadata", key);
        }

        /// <summary>
        /// Find an Anchor from the Grades section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor getSkinnableGradeAnchor(string key)
        {
            return Skin.getConfigAnchor("result_screen", "Grades", key);
        }

        /// <summary>
        /// Add the judgement and the total of that judgement to the Result Screen.
        /// </summary>
        /// <param name="name">The name of the judgement.</param>
        private void addJudgeInfo(string name)
        {
            string firstUpper = char.ToUpper(name[0]) + name.Substring(1);
            JudgementValue judgement = Judgement.getByName(name);
            judgements.Add(new KeyValuePair<Judge, JudgeCount>(
                        new Judge(judgement.score,
                            new Vector2(getSkinnablePropertyInt(firstUpper + "X"), getSkinnablePropertyInt(firstUpper + "Y")),
                            getSkinnablePropertyInt(firstUpper + "Scale")),
                        new JudgeCount(name, 
                            new Vector2(getSkinnablePropertyInt(firstUpper + "CountX"), getSkinnablePropertyInt(firstUpper + "CountY")),
                            judgement.color,
                            getSkinnablePropertyInt(firstUpper + "CountSize"),
                            getSkinnablePropertyAnchor(firstUpper + "CountAnchor"))
                        ));
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
            accuracy.Draw();
            score.Draw();
            combo.Draw();
            title.Draw();
            artist.Draw();
            version.Draw();
            mapper.Draw();
            grade.Draw();
            foreach (KeyValuePair<Judge, JudgeCount> judgePair in judgements)
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
