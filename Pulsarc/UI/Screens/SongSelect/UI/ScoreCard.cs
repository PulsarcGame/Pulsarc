using Microsoft.Xna.Framework;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.UI.Screens.Result.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    public class ScoreCard : Drawable
    {
        ScoreData scoreData;

        Grade grade;
        ScoreCardRank rank;
        ScoreCardScore score;
        ScoreCardAccuracy acc;
        ScoreCardCombo combo;

        public ScoreCard(ScoreData data, Vector2 position, int rankPosition) : base()
        {
            scoreData = data;

            grade = new Grade(scoreData.grade, new Vector2(position.X + 80, position.Y + 20), 0.2f);
            rank = new ScoreCardRank(new Vector2(position.X + 10, position.Y + 20), Color.White);
            score = new ScoreCardScore(new Vector2(position.X + 140, position.Y + 20), Color.White);
            acc = new ScoreCardAccuracy(new Vector2(position.X + 140, position.Y + 0), Color.White);
            combo = new ScoreCardCombo(new Vector2(position.X + 140, position.Y + 40), Color.White);

            rank.Update(rankPosition);
            score.Update(scoreData.score);
            acc.Update(scoreData.accuracy);
            combo.Update(scoreData.maxcombo);
        }

        public override void move(Vector2 delta)
        {
            rank.move(delta);
            grade.move(delta);
            score.move(delta);
            acc.move(delta);
            combo.move(delta);
        }

        public override void Draw()
        {
            grade.Draw();
            rank.Draw();
            score.Draw();
            acc.Draw();
            combo.Draw();
        }
    }
}
