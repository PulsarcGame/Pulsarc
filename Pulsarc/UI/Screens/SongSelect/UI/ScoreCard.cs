using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.UI.Screens.Result.UI;
using System;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    public class ScoreCard : Card
    {
        public static Texture2D StaticTexture = Skin.assets["scorecard"];

        ScoreData scoreData;

        Grade grade;

        public ScoreCard(ScoreData data, Vector2 position, int rankPosition, Anchor anchor = Anchor.TopLeft) : base(StaticTexture, position, anchor)
        {
            // set scoredata
            scoreData = data;

            // set grade
            float scale = getSkinnableFloat("GradeScale");

            Vector2 startPos = Skin.getStartPosition(config, section, "GradeStartPos", this);

            Anchor gradeAnchor = getSkinnableAnchor("GradeAnchor");

            grade = new Grade(scoreData.grade, startPos, scale);

            int gradeXOffset = getSkinnableInt("GradeX");
            int gradeYOffset = getSkinnableInt("GradeY");
            grade.scaledMove(gradeXOffset, gradeYOffset);

            // set other data
            addTextDisplayElement("Rank");
            textElements[0].Update("#" + rankPosition);

            addTextDisplayElement("Score");
            textElements[1].Update(scoreData.score.ToString("#,##"));

            addTextDisplayElement("Acc");
            textElements[2].Update(Math.Round(scoreData.accuracy * 100, 2).ToString("#,##.00") + "%");

            addTextDisplayElement("Combo");
            textElements[3].Update("x" + scoreData.maxCombo);
        }

        protected override void setConfigAndSection()
        {
            config = "song_select";
            section = "ScoreCardData";
        }

        public override void move(Vector2 delta, bool scaledPositioning = true)
        {
            base.move(delta, scaledPositioning);
            grade.move(delta, scaledPositioning);
        }

        public override void scaledMove(Vector2 delta)
        {
            base.scaledMove(delta);
            grade.scaledMove(delta);
        }

        public override void Draw()
        {
            base.Draw();
            grade.Draw();
        }
    }
}
