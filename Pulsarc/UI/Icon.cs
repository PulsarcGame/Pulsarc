using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI
{
    class Icon : Drawable
    {
        public Icon(string name) : base(Skin.assets[name]) { }
    }
}
