using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Result.UI
{
    class Grade : Drawable
    {
        public Grade(string grade, Vector2 position, float scale) : base(Skin.assets["grade_"+grade], position)
        {
            Resize(texture.Width * scale);

            changePosition(new Vector2(position.X - (texture.Width * scale) / 2, position.Y - (texture.Height * scale) / 2));
        }
    }
}
