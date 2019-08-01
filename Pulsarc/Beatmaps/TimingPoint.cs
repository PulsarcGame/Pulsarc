
namespace Pulsarc.Beatmaps
{
    public class TimingPoint
    {
        public int time;
        public int bpm;

        public TimingPoint(int time, int bpm)
        {
            this.time = time;
            this.bpm = bpm;
        }

        public override string ToString()
        {
            return time + "," + bpm;
        }
    }
}
