
namespace Pulsarc.Beatmaps
{
    /// <summary>
    /// A class that represents a "TimingPoint", or when the BPM in a song changes.
    /// </summary>
    public class TimingPoint
    {
        // The Time this TimingPoint activates
        private int Time { get; set; }
        // The BPM set by this TimingPoint
        private int Bpm { get; set; }

        public TimingPoint(int time, int bpm)
        {
            Time = time;
            Bpm = bpm;
        }

        public override string ToString()
        {
            return $"{Time},{Bpm}";
        }
    }
}
