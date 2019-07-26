using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

            this.accuracy = new Accuracy(new Vector2(470, 300), 35);
            this.accuracy.Update(accuracy);

            score = new Score(new Vector2(1000, 300), 35);
            score.Update(GetResultScreen().display_score);

            grade_container = new GradeContainer(new Vector2(-425,115), grade);

            replay_button.updatePosition(new Vector2(1920 - replay_button.texture.Width, 270));
            return_button.updatePosition(new Vector2(1920 - return_button.texture.Width, 550));

            hitErrorGraph = new HitErrorGraph(new Vector2(390, 410), (int)(785 / 1920f * Pulsarc.getDimensions().X), (int) (270 / 1080f * Pulsarc.getDimensions().Y), GetResultScreen().hits);
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
