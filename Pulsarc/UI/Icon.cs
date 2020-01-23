using Pulsarc.Skinning;

namespace Pulsarc.UI
{
    class Icon : Drawable
    {
        public Icon(string name) : base(Skin.Assets[name]) { }
    }
}
