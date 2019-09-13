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
        public Grade(string grade_, Vector2 position, float scale) : base(Skin.assets["grade_" + grade_[0].ToString()], position)
        {
            subGrades = new List<Grade>();

            Resize(Texture.Width * scale);

            for (int i = 0; i < grade_.Length - 1; i++)
            {
                subGrades.Add(new Grade(grade_[0].ToString(), position, scale));
                if (i % 2 == 0)
                {
                    foreach (Grade g in subGrades)
                    {
                        g.move(new Vector2(-Texture.Width * this.scale / 2.5f, 0));
                    }
                }
            }

            changePosition(new Vector2(position.X - (Texture.Width * scale) / 2, position.Y - (Texture.Height * scale) / 2));

            for (int i = 0; i < grade_.Length - 1; i++)
            {
                if (i % 2 == 0)
                {
                    move(new Vector2(Texture.Width * this.scale / 2.5f, 0));
                }
            }
        }

        public override void Draw()
        {
            foreach(Grade oneGrade in subGrades)
            {
                oneGrade.Draw();
            }
            base.Draw();
        }
    }
}
