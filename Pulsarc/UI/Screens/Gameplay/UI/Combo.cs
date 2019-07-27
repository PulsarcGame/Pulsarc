using Microsoft.Xna.Framework;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class Combo : TextDisplayElementFixedSize
    {
        public Combo(Vector2 position, int fontSize = 14, bool centered = false) : base("", position, 7, fontSize, centered)
        {
        }

        public void Update(int value)
        {
            Update(value.ToString(),"x");
        }
    }
}
