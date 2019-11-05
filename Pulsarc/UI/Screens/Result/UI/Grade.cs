using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using System.Collections.Generic;

namespace Pulsarc.UI.Screens.Result.UI
{
    /// <summary>
    /// The Icon that displays the grade in the ResultScreen
    /// </summary>
    class Grade : Drawable
    {
        // Used for "AAA"/"AA"/"SSS"/"SS" to draw the extra letters.
        private List<Grade> suffixGrades;

        public Grade(string grade_, Vector2 position, float scale, Anchor anchor = Anchor.Center)
            : base(Skin.Assets[$"grade_{grade_[0]}"], position, anchor: anchor)
        {
            suffixGrades = new List<Grade>();

            Resize(currentSize.X * scale);

            // For each extra letter, add another suffix to suffixGrades
            for (int i = 0; i < grade_.Length - 1; i++)
            {
                suffixGrades.Add(new Grade($"{grade_[0]}", position, scale));

                if (i % 2 == 0)
                    foreach (Grade suffixGrade in suffixGrades)
                        suffixGrade.Move(new Vector2(currentSize.X / 2.5f, 0));
            }

            ChangePosition(new Vector2(position.X - currentSize.X / 2, position.Y - currentSize.Y / 2));
        }

        public override void Move(Vector2 position, bool scaledPositioning = true)
        {
            base.Move(position, scaledPositioning);

            foreach (Grade subGrade in suffixGrades)
                subGrade.Move(position, scaledPositioning);
        }

        public override void ScaledMove(Vector2 position)
        {
            base.ScaledMove(position);

            foreach (Grade subGrade in suffixGrades)
                subGrade.ScaledMove(position);
        }

        public override void Draw()
        {
            foreach(Grade subGrade in suffixGrades)
                subGrade.Draw();

            base.Draw();
        }
    }
}
