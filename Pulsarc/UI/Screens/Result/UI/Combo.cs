using Microsoft.Xna.Framework;

namespace Pulsarc.UI.Screens.Result.UI
{
    class Combo : TextDisplayElement
    {
        public Combo(Vector2 position, int fontSize = 20, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor)
        {
        }

        public void Update(int value)
        {
            Update(value.ToString() + "x");
        }
    }
}
