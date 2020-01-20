using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class Judge : Drawable
    {
        public Judge(int key, Vector2 position)
            : base(Skin.Judges[key], position, -1, Anchor.Center)
        { }
    }
}
