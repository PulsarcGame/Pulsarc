using Microsoft.Xna.Framework;

namespace Pulsarc.UI.Screens.Gameplay.UI
{
    class Score : TextDisplayElementFixedSize
    {
        public Score(Vector2 position, int fontSize = 20, bool centered = false) : base("", position, 9, fontSize, centered)
        {
        }

        public void Update(long value)
        {
            Update(value.ToString());
        }
    }
}
