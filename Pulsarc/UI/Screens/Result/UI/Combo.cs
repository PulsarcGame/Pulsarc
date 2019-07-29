using Microsoft.Xna.Framework;

namespace Pulsarc.UI.Screens.Result.UI
{
    class Combo : TextDisplayElement
    {
        public Combo(Vector2 position, Color color, int fontSize = 20, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor, color)
        {
        }

        public void Update(int value)
        {
            Update(value.ToString("#,#0") + "x");
        }
    }
}
