using Microsoft.Xna.Framework;
using System;
using System.Reflection;

namespace Pulsarc.UI
{
    public class VersionCounter : TextDisplayElement
    {
        public static string FullVersion => Pulsarc.CurrentVersion.ToString();
        // Remove the extra ".0" at the end of FullVersion
        public static string Version => FullVersion.Substring(0, FullVersion.LastIndexOf('.'));
        public static string VersionName = "v" + Version + "-alpha";
        
        // Temporary until we fix font shit.
        private const int X_OFFSET = -22;
        private const int Y_OFFSET = -6;

        public VersionCounter(Vector2 position, int fontSize = 20, Anchor anchor = Anchor.BottomRight)
            : base(VersionName, position, fontSize, anchor)
            => Move(X_OFFSET, Y_OFFSET);
    }
}
