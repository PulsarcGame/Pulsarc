using Microsoft.Xna.Framework;

namespace Pulsarc.UI
{
    public class VersionCounter : TextDisplayElement
    {
        private const string VERSION_NAME = "v1.3.0-alpha";
        
        // Temporary until we fix font shit.
        private const int X_OFFSET = -22;
        private const int Y_OFFSET = -6;

        public VersionCounter(Vector2 position, int fontSize = 20, Anchor anchor = Anchor.BottomRight)
            : base(VERSION_NAME, position, fontSize, anchor) => Move(X_OFFSET, Y_OFFSET);
    }
}
