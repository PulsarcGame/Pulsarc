using Pulsarc.Skinning;

namespace Pulsarc.UI.Common
{
    class Background : Drawable
    {
        public Background(string skinAsset) : base(Skin.assets[skinAsset]) {}
    }
}
