using Microsoft.Xna.Framework;

namespace Pulsarc.UI.Screens.Result.UI
{
    class Score : TextDisplayElement
    {
        public Score(Vector2 position, int fontSize = 20, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor)
        {
        }

        public void Update(long value)
        {
            Update(value.ToString());
        }
    }
}
