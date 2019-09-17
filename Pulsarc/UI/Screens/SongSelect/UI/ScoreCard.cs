using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
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

        public ScoreCard(ScoreData data, Vector2 position, int rankPosition) : base(Skin.assets["scorecard"])
        {
            scoreData = data;

            changePosition(position);
            grade = new Grade(scoreData.grade, new Vector2(position.X + 180, position.Y + 75), 0.2f);
            rank = new ScoreCardRank(new Vector2(position.X + 70, position.Y + 70), Color.White, anchor: Anchor.CenterLeft);
            score = new ScoreCardScore(new Vector2(position.X + 260, position.Y + 50), Color.White);
            acc = new ScoreCardAccuracy(new Vector2(position.X + 580, position.Y + 70), Color.White, anchor: Anchor.CenterRight);
            combo = new ScoreCardCombo(new Vector2(position.X + 260, position.Y + 90), Color.White);

            rank.Update(rankPosition);
            score.Update(scoreData.score);
            acc.Update(scoreData.accuracy);
            combo.Update(scoreData.maxcombo);
        }

        public override void move(Vector2 delta, bool truePositioning = false)
        {
            rank.move(delta, truePositioning);
            grade.move(delta, truePositioning);
            score.move(delta, truePositioning);
            acc.move(delta, truePositioning);
            combo.move(delta, truePositioning);
        }

        public override void Draw()
        {
            base.Draw();
            grade.Draw();
            rank.Draw();
            score.Draw();
            acc.Draw();
            combo.Draw();
        }
    }
}
