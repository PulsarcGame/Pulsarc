using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using Pulsarc.UI.Screens.Result.UI;
using Pulsarc.Utils.SQLite;
using System;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    public class ScoreCard : Card
    {
        public static Texture2D DefaultTexture => Skin.Assets["scorecard"];

        private ScoreData scoreData;

        private Grade grade;

        public ScoreCard(ScoreData data, Vector2 position, int rankPosition, Anchor anchor = Anchor.TopLeft)
            : base(DefaultTexture, position, anchor)
        {
            // set scoredata
            scoreData = data;

            // set grade
            setGrade();

            // set other data
            setData(rankPosition);
        }

        private void setGrade()
        {
            float scale = GetSkinnableFloat("GradeScale");

            Vector2 startPos = Skin.GetConfigStartPosition(config, section, "GradeStartPos", this);

            Anchor gradeAnchor = GetSkinnableAnchor("GradeAnchor");

            grade = new Grade(scoreData.grade, startPos, scale, gradeAnchor);

            int gradeXOffset = GetSkinnableInt("GradeX");
            int gradeYOffset = GetSkinnableInt("GradeY");
            grade.ScaledMove(gradeXOffset, gradeYOffset);
        }

        private void setData(int rankPosition)
        {
            AddTextDisplayElement("Rank");
            textElements[0].Update("#" + rankPosition);

            AddTextDisplayElement("Score");
            textElements[1].Update(scoreData.score.ToString("#,##"));

            AddTextDisplayElement("Acc");
            textElements[2].Update(Math.Round(scoreData.accuracy * 100, 2).ToString("#,##.00") + "%");

            AddTextDisplayElement("Combo");
            textElements[3].Update("x" + scoreData.maxCombo);
        }

        protected override void SetConfigAndSection()
        {
            config = "song_select";
            section = "ScoreCardData";
        }

        public override void Move(Vector2 delta, bool scaledPositioning = true)
        {
            base.Move(delta, scaledPositioning);
            grade.Move(delta, scaledPositioning);
        }

        public override void ScaledMove(Vector2 delta)
        {
            base.ScaledMove(delta);
            grade.ScaledMove(delta);
        }

        public override void Draw()
        {
            base.Draw();
            grade.Draw();
        }
    }
}
