using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Result.UI
{
    class GradeContainer : Drawable
    {
        Grade grade;
        public GradeContainer(Vector2 position, string gradeLetter) : base(Skin.assets["score_grade_container"])
        {
            grade = new Grade(gradeLetter);
            changePosition(position);
        }

        public new void changePosition(Vector2 position)
        {
            base.changePosition(position);
            grade.changePosition(new Vector2(position.X + (int) (texture.Width / 2.1), position.Y + texture.Height / 2 - grade.texture.Height / 2));
        }

        public override void Draw()
        {
            base.Draw();
            grade.Draw();
        }
    }
}
