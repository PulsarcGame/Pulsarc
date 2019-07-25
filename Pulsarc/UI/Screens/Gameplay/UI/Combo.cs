using Microsoft.Xna.Framework;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class Combo : TextDisplayElement
    {
        public Combo(Vector2 position, int fontSize = 14, bool centered = false) : base("", position, fontSize, centered)
        {
        }

        public void Update(int value)
        {
            Update(value + "x");
        }
    }
}
