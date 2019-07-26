using Microsoft.Xna.Framework;

namespace Pulsarc.UI.Screens.Result.UI
{
    class Score : TextDisplayElement
    {
        public Score(Vector2 position, int fontSize = 20, bool centered = false) : base("", position, fontSize, centered)
        {
        }

        public void Update(long value)
        {
            Update(value.ToString());
        }
    }
}
