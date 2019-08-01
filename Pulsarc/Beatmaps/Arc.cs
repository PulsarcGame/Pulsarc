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

        public override string ToString()
        {
            // timestamp,type -> 432,0110 
            return time + "," + Convert.ToString(type, 2).PadLeft(4, '0');
        }
    }
}
