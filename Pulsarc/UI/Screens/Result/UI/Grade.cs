using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Result.UI
{
    /// <summary>
    /// The Icon that displays the grade in the ResultScreen
    /// </summary>
    sealed class Grade : Drawable
    {
        // Used for "AAA"/"AA"/"SSS"/"SS" to draw the extra letters.
        private readonly List<Grade> _suffixGrades;

        public Grade(string grade, Vector2 position, float scale, Anchor anchor = Anchor.Center)
            : base(Skin.Assets[$"grade_{grade[0]}"], position, anchor: anchor)
        {
            _suffixGrades = new List<Grade>();

            Resize(CurrentSize.X * scale);

            // For each extra letter, add another suffix to suffixGrades
            for (int i = 0; i < grade.Length - 1; i++)
            {
                _suffixGrades.Add(new Grade($"{grade[0]}", position, scale));

                if (i % 2 != 0) continue;
                foreach (Grade suffixGrade in _suffixGrades)
                    suffixGrade.Move(new Vector2(CurrentSize.X / 2.5f, 0));
            }

            ChangePosition(new Vector2(position.X - CurrentSize.X / 2, position.Y - CurrentSize.Y / 2));
        }

        public override void Move(Vector2 delta, bool? heightScaled = null)
        {
            base.Move(delta, heightScaled);

            foreach (Grade subGrade in _suffixGrades)
                subGrade.Move(delta, heightScaled);
        }

        public override void Draw()
        {
            foreach(Grade subGrade in _suffixGrades)
                subGrade.Draw();

            base.Draw();
        }
    }
}
