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

        RetryButton button_retry;
        ReturnButton button_back;
        ButtonAdvanced button_advanced;

        Beatmap beatmap;

        Accuracy accuracy;
        Score score;
        Combo combo;

        Grade grade;

        Scorecard scorecard;
        Background background;

        Title title;
        Artist artist;
        Version version;
        Mapper mapper;

        List<KeyValuePair<Judge,JudgeCount>> judgements;

        HitErrorGraph hitErrorGraph;

        public ResultScreenView(Screen screen, double accuracy, string grade, Beatmap beatmap) : base(screen)
        {
            judgements = new List<KeyValuePair<Judge, JudgeCount>>();
            this.beatmap = beatmap;

            button_advanced = new ButtonAdvanced(new Vector2(0, 1080));
            button_back = new ReturnButton("result_button_back", new Vector2(0, 1080));
            button_retry = new RetryButton("result_button_retry", new Vector2(1920, 1080));

            scorecard = new Scorecard();
            background = new Background("result_background");

            this.grade = new Grade(grade, new Vector2(getSkinnablePosition("GradeX"), getSkinnablePosition("GradeY")), getSkinnablePosition("GradeScale"));

            score = new Score(new Vector2(getSkinnablePosition("ScoreX"), getSkinnablePosition("ScoreY")), new Color(74,245,254), getSkinnableInt("ScoreSize"), getSkinnableAnchor("ScoreAnchor"));
            combo = new Combo(new Vector2(getSkinnablePosition("ComboX"), getSkinnablePosition("ComboY")), new Color(74, 245, 254), getSkinnableInt("ComboSize"), getSkinnableAnchor("ComboAnchor"));
            this.accuracy = new Accuracy(new Vector2(getSkinnablePosition("AccuracyX"), getSkinnablePosition("AccuracyY")), new Color(74, 245, 254), getSkinnableInt("AccuracySize"), getSkinnableAnchor("AccuracyAnchor"));

            title = new Title(new Vector2(getSkinnablePosition("TitleX"), getSkinnablePosition("TitleY")), getSkinnableInt("TitleSize"), getSkinnableAnchor("TitleAnchor"));
            artist = new Artist(new Vector2(getSkinnablePosition("ArtistX"), getSkinnablePosition("ArtistY")), getSkinnableInt("ArtistSize"), getSkinnableAnchor("ArtistAnchor"));
            version = new Version(new Vector2(getSkinnablePosition("VersionX"), getSkinnablePosition("VersionY")), getSkinnableInt("VersionSize"), getSkinnableAnchor("VersionAnchor"));
            mapper = new Mapper(new Vector2(getSkinnablePosition("MapperX"), getSkinnablePosition("MapperY")), new Color(74, 245, 254), getSkinnableInt("MapperSize"), getSkinnableAnchor("MapperAnchor"));

            button_advanced.move(new Vector2(button_back.texture.Width, -button_advanced.texture.Height));

            this.accuracy.Update(accuracy);
            combo.Update(GetResultScreen().combo);
            score.Update(GetResultScreen().display_score);

            title.Update(GetResultScreen().beatmap.Title);
            artist.Update(GetResultScreen().beatmap.Artist);
            version.Update(GetResultScreen().beatmap.Version);
            mapper.Update(GetResultScreen().beatmap.Mapper);

            hitErrorGraph = new HitErrorGraph(
                new Vector2(getSkinnablePosition("HitErrorX")
                            , getSkinnablePosition("HitErrorY"))
               , (int)(getSkinnablePosition("HitErrorWidth") / 1920f * Pulsarc.getDimensions().X)
               , (int) (getSkinnablePosition("HitErrorHeight") / 1080f * Pulsarc.getDimensions().Y)
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

        private float getSkinnablePosition(string key)
        {
            return Skin.getConfigFloat("result_screen", "Positions", key);
        }

        private int getSkinnableInt(string key)
        {
            return Skin.getConfigInt("result_screen", "Positions", key);
        }

        private Anchor getSkinnableAnchor(string key)
        {
            return Skin.getConfigAnchor("result_screen", "Positions", key);
        }

        private void addJudgeInfo(string name)
        {
            string firstUpper = char.ToUpper(name[0]) + name.Substring(1);
            JudgementValue judgement = Judgement.getByName(name);
            judgements.Add(new KeyValuePair<Judge, JudgeCount>(
                        new Judge(judgement.score,
                            new Vector2(getSkinnableInt(firstUpper + "X"), getSkinnableInt(firstUpper + "Y")),
                            getSkinnableInt(firstUpper + "Scale")),
                        new JudgeCount(name, 
                            new Vector2(getSkinnableInt(firstUpper + "CountX"), getSkinnableInt(firstUpper + "CountY")),
                            judgement.color,
                            getSkinnableInt(firstUpper + "CountSize"),
                            getSkinnableAnchor(firstUpper + "CountAnchor"))
                        ));
        }

        public override void Destroy()
        {
            // Destroy
        }

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

        public override void Update(GameTime gameTime)
        {
            while (InputManager.keyboardPresses.Count > 0)
            {
                KeyValuePair<double, Keys> press = InputManager.keyboardPresses.Dequeue();

                if (press.Value == Keys.Escape || press.Value == Keys.Delete)
                {
                    ScreenManager.RemoveScreen(true);
                }
            }

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
