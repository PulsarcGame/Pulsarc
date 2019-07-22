using System;

namespace Pulsarc.Beatmaps
{
    public class Arc
    {
        public int time;
        public int type;

        public Arc(int time, int type)
        {
            this.time = time;
            this.type = type;
        }

        public string toString()
        {
            return time + "," + Convert.ToString(type, 2).PadLeft(4, '0');
        }
    }
}
