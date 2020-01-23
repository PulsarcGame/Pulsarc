using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Skinning;
using Pulsarc.UI.Screens.Result.UI;
using Pulsarc.Utils.SQLite;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    public class ScoreCard : Card
    {
        private static Texture2D DefaultTexture => Skin.Assets["scorecard"];

        private ScoreData scoreData;

        private Grade _grade;

        // Stats
        private static readonly Anchor DefaultAnchor = Skin.GetConfigAnchor("song_select", "Properties", "ScoreCardAnchor");

        private static readonly int OffsetX = Skin.GetConfigInt("song_select", "Properties", "ScoreCardX");
        private static readonly int OffsetY = Skin.GetConfigInt("song_select", "Properties", "ScoreCardY");
        private static readonly Vector2 StartPosition = Skin.GetConfigStartPosition("song_select", "Properties", "ScoreCardStartPos") + new Vector2(OffsetX, OffsetY);

        private static readonly int Margin = Skin.GetConfigInt("song_select", "Properties", "ScoreCardMargin");
        private static readonly int TotalHeight = DefaultTexture.Height + Margin;
        public static readonly int TotalWidth = DefaultTexture.Width + Margin;

        private int Index { get; set; }

        private Vector2 PersonalStartPosition => StartPosition + personalStartPosOffset;
        private Vector2 personalStartPosOffset;

        public ScoreCard(ScoreData data, int rankPosition)
            : base(DefaultTexture, StartPosition, DefaultAnchor)
        {
            // set scoredata
            scoreData = data;

            // Set proper position
            Index = rankPosition - 1;
            personalStartPosOffset = new Vector2(0, TotalHeight * Pulsarc.HeightScale * Index);

            ChangePosition(PersonalStartPosition);

            // set grade
            SetGrade();

            // set other data
            SetData(rankPosition);
        }

        public sealed override void ChangePosition(Vector2 position, bool topLeftPositioning = false)
        {
            base.ChangePosition(position, topLeftPositioning);
        }

        private void SetGrade()
        {
            float scale = GetSkinnableFloat("GradeScale");

            Vector2 startPos = Skin.GetConfigStartPosition(Config, Section, "GradeStartPos", this);

            Anchor gradeAnchor = GetSkinnableAnchor("GradeAnchor");

            _grade = new Grade(scoreData.Grade, startPos, scale, gradeAnchor);

            int gradeXOffset = GetSkinnableInt("GradeX");
            int gradeYOffset = GetSkinnableInt("GradeY");
            _grade.Move(gradeXOffset, gradeYOffset);
        }

        private void SetData(int rankPosition)
        {
            AddTextDisplayElement("Rank");
            TextElements[0].Update("#" + rankPosition);

            AddTextDisplayElement("Score");
            TextElements[1].Update(scoreData.Score.ToString("#,##"));

            AddTextDisplayElement("Acc");
            TextElements[2].Update(Math.Round(scoreData.Accuracy * 100, 2).ToString("#,##.00") + "%");

            AddTextDisplayElement("Combo");
            TextElements[3].Update("x" + scoreData.MaxCombo);

            AddTextDisplayElement("Rate");
            TextElements[4].Update("x"+Math.Round(scoreData.Rate, 2).ToString("#.00"));

            AddTextDisplayElement("Username");
            TextElements[5].Update(scoreData.Username);
        }

        protected override void SetConfigAndSection()
        {
            Config = "song_select";
            Section = "ScoreCardData";
        }

        public override void Move(Vector2 delta, bool? heightScaled = null)
        {
            base.Move(delta, heightScaled);
            _grade.Move(delta, heightScaled);
        }

        public override void Draw()
        {
            base.Draw();
            _grade.Draw();
        }
    }
}
