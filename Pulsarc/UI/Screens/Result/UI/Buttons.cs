using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Result.UI
{
    class ButtonBack : Drawable
    {
        public ButtonBack(Vector2 position) : base(Skin.assets["result_button_back"], position)
        {
        }
    }
    class ButtonRetry : Drawable
    {
        public ButtonRetry(Vector2 position) : base(Skin.assets["result_button_retry"], position)
        {
        }
    }
    class ButtonAdvanced : Drawable
    {
        public ButtonAdvanced(Vector2 position) : base(Skin.assets["result_button_advanced"], position)
        {
        }
    }
}
