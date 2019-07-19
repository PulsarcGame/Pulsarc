
namespace Pulsarc.Beatmaps
{
    class TimingPoint
    {
        public int time;
        public int bpm;

        public TimingPoint(int time, int bpm)
        {
            this.time = time;
            this.bpm = bpm;
        }

        public string toString()
        {
            return time + "," + bpm;
        }
    }
}
