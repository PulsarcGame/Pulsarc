using Microsoft.Xna.Framework;

namespace Pulsarc.UI.Screens.Editor.UI
{
    public enum Beat
    {
        Whole = 1,
        Half = 2,
        Third = 3,
        Fourth = 4,
        Sixth = 6,
        Eighth = 8,
        Twelveth = 12,
        Sixteenth = 16
    }

    public abstract class BeatDisplay : Drawable
    {
        // The color for each kind of beat.
        protected static Color WholeBeatColor = Color.White;
        protected static Color HalfBeatColor = Color.LightGray;
        protected static Color ThirdBeatColor = Color.Pink;
        protected static Color FourthBeatColor => Color.Red;
        protected static Color SixthBeatColor => Color.DarkRed;
        protected static Color EightBeatColor => Color.Blue;
        protected static Color TwelvethBeatColor => Color.LightBlue;
        protected static Color SixteenthBeatColor => Color.Yellow;

        // The Beat this object represents
        public Beat Beat { get; protected set; }

        // The time that this beat corresponds to in the audio
        public int Time { get; protected set; }
        
        // The Scale of the editor
        public float EditorScale { get; protected set; }

        public BeatDisplay(Beat beat, int time, float scale)
        {
            Beat = beat;
            Time = time;
            EditorScale = scale;

            SetBeatTexture();
        }

        /// <summary>
        /// Set the Texture for this BeatDisplay
        /// </summary>
        protected abstract void SetBeatTexture();
    }
}
