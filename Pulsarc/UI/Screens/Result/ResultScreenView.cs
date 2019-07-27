using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Pulsarc.UI.Screens.Result.UI;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Result
{
    class ResultScreenView : ScreenView
    {
        ResultScreen GetResultScreen() { return (ResultScreen) Screen; }

        Button replay_button;
        Button return_button;

        Accuracy accuracy;
        Score score;

        GradeContainer grade_container;
        Scorecard scorecard;
        Background background;

        HitErrorGraph hitErrorGraph;

        public ResultScreenView(Screen screen, double accuracy, string grade) : base(screen)
        {
            replay_button = new Button("result_replay", 72);
            return_button = new Button("result_return", 72);

            scorecard = new Scorecard(new Vector2(-100, 190));
            background = new Background();

            this.accuracy = new Accuracy(new Vector2(getSkinnablePosition("AccuracyX"), getSkinnablePosition("AccuracyY")), 35);
            this.accuracy.Update(accuracy);

            score = new Score(new Vector2(getSkinnablePosition("ScoreX"), getSkinnablePosition("ScoreY")), 35);
            score.Update(GetResultScreen().display_score);

            grade_container = new GradeContainer(new Vector2(-425,115), grade);

            replay_button.updatePosition(new Vector2(1920 - replay_button.texture.Width, 270));
            return_button.updatePosition(new Vector2(1920 - return_button.texture.Width, 550));

            hitErrorGraph = new HitErrorGraph(
                new Vector2(getSkinnablePosition("HitErrorX")
                            , getSkinnablePosition("HitErrorY"))
               , (int)(getSkinnablePosition("HitErrorWidth") / 1920f * Pulsarc.getDimensions().X)
               , (int) (getSkinnablePosition("HitErrorHeight") / 1080f * Pulsarc.getDimensions().Y)
               , GetResultScreen().hits);
        }

        private float getSkinnablePosition(string key)
        {
            return Skin.getConfigFloat("result_screen", "Positions", key);
        }

        public override void Destroy()
        {
            // Destroy
        }

        public override void Draw(GameTime gameTime)
        {
            background.Draw();
            replay_button.Draw();
            return_button.Draw();
            scorecard.Draw();
            grade_container.Draw();
            accuracy.Draw();
            score.Draw();
            hitErrorGraph.Draw();
        }

        public override void Update(GameTime gameTime)
        {

        }
    }
}
