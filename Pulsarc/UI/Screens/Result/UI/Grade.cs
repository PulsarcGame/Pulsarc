using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Result.UI
{
    class Grade : Drawable
    {
        List<Grade> subGrades;

        public Grade(string grade_, Vector2 position, float scale, Anchor anchor = Anchor.Center) : base(Skin.assets["grade_" + grade_[0].ToString()], position, anchor: anchor)
        {
            subGrades = new List<Grade>();

            Resize(currentSize.X * scale);

            for (int i = 0; i < grade_.Length - 1; i++)
            {
                subGrades.Add(new Grade(grade_[0].ToString(), position, scale));
                if (i % 2 == 0)
                {
                    foreach (Grade subGrade in subGrades)
                    {
                        subGrade.move(new Vector2(currentSize.X / 2.5f, 0));
                    }
                }
            }

            changePosition(new Vector2(position.X - currentSize.X / 2, position.Y - currentSize.Y / 2));

            /*for (int i = 0; i < grade_.Length - 1; i++)
            {
                if (i % 2 == 0)
                {
                    move(new Vector2(currentSize.X / 2.5f, 0));
                }
            }*/
        }

        public override void move(Vector2 position, bool scaledPositioning = true)
        {
            base.move(position, scaledPositioning);

            foreach (Grade subGrade in subGrades)
            {
                subGrade.move(position, scaledPositioning);
            }
        }

        public override void scaledMove(Vector2 position)
        {
            base.scaledMove(position);

            foreach (Grade subGrade in subGrades)
            {
                subGrade.scaledMove(position);
            }
        }

        public override void Draw()
        {
            foreach(Grade subGrade in subGrades)
            {
                subGrade.Draw();
            }
            base.Draw();
        }
    }
}
