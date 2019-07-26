using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Result.UI
{
    class Scorecard : Drawable
    {
        public Scorecard(Vector2 position) : base(Skin.assets["result_scorecard"], position) {}
    }
}
