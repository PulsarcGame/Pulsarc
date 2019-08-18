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
        public ResultScreenView(Screen screen, double accuracy, string grade, Beatmap beatmap) : base(screen)
        {
            judgements = new List<KeyValuePair<Judge, JudgeCount>>();
            this.beatmap = beatmap;

            button_advanced = new ButtonAdvanced(new Vector2(0, 1080));
            button_back = new ReturnButton("result_button_back", new Vector2(0, 1080));
            button_retry = new RetryButton("result_button_retry", new Vector2(1920, 1080));

            scorecard = new Scorecard();
            background = new Background("result_background");

            this.grade = new Grade(grade, new Vector2(getSkinnablePositionFloat("GradeX"), getSkinnablePositionFloat("GradeY")), getSkinnablePositionFloat("GradeScale"));

            score = new Score(new Vector2(getSkinnablePositionFloat("ScoreX"), getSkinnablePositionFloat("ScoreY")), new Color(74,245,254), getSkinnablePositionInt("ScoreSize"), getSkinnablePositionAnchor("ScoreAnchor"));
            combo = new Combo(new Vector2(getSkinnablePositionFloat("ComboX"), getSkinnablePositionFloat("ComboY")), new Color(74, 245, 254), getSkinnablePositionInt("ComboSize"), getSkinnablePositionAnchor("ComboAnchor"));
            this.accuracy = new Accuracy(new Vector2(getSkinnablePositionFloat("AccuracyX"), getSkinnablePositionFloat("AccuracyY")), new Color(74, 245, 254), getSkinnablePositionInt("AccuracySize"), getSkinnablePositionAnchor("AccuracyAnchor"));

            title = new Title(new Vector2(getSkinnablePositionFloat("TitleX"), getSkinnablePositionFloat("TitleY")), getSkinnablePositionInt("TitleSize"), getSkinnablePositionAnchor("TitleAnchor"));
            artist = new Artist(new Vector2(getSkinnablePositionFloat("ArtistX"), getSkinnablePositionFloat("ArtistY")), getSkinnablePositionInt("ArtistSize"), getSkinnablePositionAnchor("ArtistAnchor"));
            version = new Version(new Vector2(getSkinnablePositionFloat("VersionX"), getSkinnablePositionFloat("VersionY")), getSkinnablePositionInt("VersionSize"), getSkinnablePositionAnchor("VersionAnchor"));
            mapper = new Mapper(new Vector2(getSkinnablePositionFloat("MapperX"), getSkinnablePositionFloat("MapperY")), new Color(74, 245, 254), getSkinnablePositionInt("MapperSize"), getSkinnablePositionAnchor("MapperAnchor"));

            button_advanced.move(new Vector2(button_back.texture.Width, -button_advanced.texture.Height));

            this.accuracy.Update(accuracy);
            combo.Update(GetResultScreen().combo);
            score.Update(GetResultScreen().display_score);

            title.Update(GetResultScreen().beatmap.Title);
            artist.Update(GetResultScreen().beatmap.Artist);
            version.Update(GetResultScreen().beatmap.Version);
            mapper.Update(GetResultScreen().beatmap.Mapper);

            hitErrorGraph = new HitErrorGraph(
                new Vector2(getSkinnablePositionFloat("HitErrorX")
                            , getSkinnablePositionFloat("HitErrorY"))
               , (int)(getSkinnablePositionFloat("HitErrorWidth") / 1920f * Pulsarc.getDimensions().X)
               , (int) (getSkinnablePositionFloat("HitErrorHeight") / 1080f * Pulsarc.getDimensions().Y)
               , GetResultScreen().hits);

            foreach(JudgementValue judge in Judgement.judgements)
            {
                addJudgeInfo(judge.name);
            }
            foreach(KeyValuePair<Judge,JudgeCount> judgePair in judgements)
            {
                judgePair.Value.Update(GetResultScreen().judges_count[judgePair.Value.name]);
            }
        }

        /// <summary>
        /// Find a float position from the Position section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The float value of the key provided.</returns>
        private float getSkinnablePositionFloat(string key)
        {
            return Skin.getConfigFloat("result_screen", "Positions", key);
        }

        /// <summary>
        /// Find a int from the Position section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The int value of the key provided.</returns>
        private int getSkinnablePositionInt(string key)
        {
            return Skin.getConfigInt("result_screen", "Positions", key);
        }

        /// <summary>
        /// Find an Anchor from the Position section of the Result Screen config.
        /// </summary>
        /// <param name="key">The key of the value to find.</param>
        /// <returns>The Anchor of the key provided.</returns>
        private Anchor getSkinnablePositionAnchor(string key)
        {
            return Skin.getConfigAnchor("result_screen", "Positions", key);
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
                            new Vector2(getSkinnablePositionInt(firstUpper + "X"), getSkinnablePositionInt(firstUpper + "Y")),
                            getSkinnablePositionInt(firstUpper + "Scale")),
                        new JudgeCount(name, 
                            new Vector2(getSkinnablePositionInt(firstUpper + "CountX"), getSkinnablePositionInt(firstUpper + "CountY")),
                            judgement.color,
                            getSkinnablePositionInt(firstUpper + "CountSize"),
                            getSkinnablePositionAnchor(firstUpper + "CountAnchor"))
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
