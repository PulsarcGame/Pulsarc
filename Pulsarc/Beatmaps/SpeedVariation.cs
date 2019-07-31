using System.Globalization;

namespace Pulsarc.Beatmaps
{
    public class SpeedVariation
    {
        public int time;
        public int type;
        public double intensity;

        public SpeedVariation(int time, int type, double intensity)
        {
            this.time = time;
            this.type = type;
            this.intensity = intensity;
        }

        public override string ToString()
        {
            // timestamp,type,intensity_factor -> 432,1,2.5
            return time + "," + type + "," + intensity.ToString("0.000", CultureInfo.InvariantCulture);
        }
    }
}
