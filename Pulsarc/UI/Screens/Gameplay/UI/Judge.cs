using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class Judge : Drawable
    {
        public Judge(int key, Vector2 position) : base(Skin.judges[key], position)
        {
            changePosition(new Vector2(960 - texture.Width / 2, 540 - texture.Height / 2));
        }
    }
}
