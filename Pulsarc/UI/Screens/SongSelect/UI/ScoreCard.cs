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

        // Stats
        public static readonly Anchor DefaultAnchor = Skin.GetConfigAnchor("song_select", "Properties", "ScoreCardAnchor");

        private static readonly int OffsetX = Skin.GetConfigInt("song_select", "Properties", "ScoreCardX");
        private static readonly int OffsetY = Skin.GetConfigInt("song_select", "Properties", "ScoreCardY");
        public static readonly Vector2 StartPosition = Skin.GetConfigStartPosition("song_select", "Properties", "ScoreCardStartPos") + new Vector2(OffsetX, OffsetY);

        public static readonly int Margin = Skin.GetConfigInt("song_select", "Properties", "ScoreCardMargin");
        public static readonly int TotalHeight = DefaultTexture.Height + Margin;
        public static readonly int TotalWidth = DefaultTexture.Width + Margin;

        public ScoreCard(ScoreData data, int rankPosition)
            : base(DefaultTexture, StartPosition, DefaultAnchor)
        {
            // set scoredata
            scoreData = data;

            // set grade
            SetGrade();

            // set other data
            SetData(rankPosition);
        }

        private void SetGrade()
        {
            float scale = GetSkinnableFloat("GradeScale");

            Vector2 startPos = Skin.GetConfigStartPosition(Config, Section, "GradeStartPos", this);

            Anchor gradeAnchor = GetSkinnableAnchor("GradeAnchor");

            grade = new Grade(scoreData.grade, startPos, scale, gradeAnchor);

            int gradeXOffset = GetSkinnableInt("GradeX");
            int gradeYOffset = GetSkinnableInt("GradeY");
            grade.ScaledMove(gradeXOffset, gradeYOffset);
        }

        private void SetData(int rankPosition)
        {
            AddTextDisplayElement("Rank");
            TextElements[0].Update("#" + rankPosition);

            AddTextDisplayElement("Score");
            TextElements[1].Update(scoreData.score.ToString("#,##"));

            AddTextDisplayElement("Acc");
            TextElements[2].Update(Math.Round(scoreData.accuracy * 100, 2).ToString("#,##.00") + "%");

            AddTextDisplayElement("Combo");
            TextElements[3].Update("x" + scoreData.maxCombo);

            AddTextDisplayElement("Rate");
            TextElements[4].Update("x"+Math.Round(scoreData.rate, 2).ToString("#.00"));

            AddTextDisplayElement("Username");
            TextElements[5].Update(scoreData.username);
        }

        protected override void SetConfigAndSection()
        {
            Config = "song_select";
            Section = "ScoreCardData";
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
