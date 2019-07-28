using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Result.UI
{
    class Judge : Drawable
    {
        public Judge(int judge, Vector2 position, float scale) : base(Skin.judges[judge], position)
        {
            Resize(texture.Width * scale);
        }
    }
}
