using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Result.UI
{
    class Grade : Drawable
    {
        public Grade(string grade) : base(Skin.assets["grade_"+grade]) { }
    }
}
