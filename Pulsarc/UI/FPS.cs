using Microsoft.Xna.Framework;
using Pulsarc.UI.Screens.Gameplay.UI;

namespace Pulsarc.UI
{
    class FPS : TextDisplayElementFixedSize
    {
        public FPS(Vector2 position, int fontSize = 14, Anchor anchor = Anchor.TopLeft) : base("", position, "fps", fontSize, anchor)
        {
        }
    }
}
