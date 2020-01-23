using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Result.UI
{
    sealed class Judge : Drawable
    {
        public Judge(int judge, Vector2 position, float scale) : base(Skin.Judges[judge], position)
        {
            Resize(Texture.Width * scale);
        }
    }
}
