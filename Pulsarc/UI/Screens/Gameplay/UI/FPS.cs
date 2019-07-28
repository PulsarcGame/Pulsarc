using Microsoft.Xna.Framework;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class FPS : TextDisplayElementFixedSize
    {
        public FPS(Vector2 position, int fontSize = 14, Anchor anchor = Anchor.TopLeft) : base("", position, "fps", fontSize, anchor)
        {
        }
    }
}
