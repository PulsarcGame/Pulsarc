using System;

namespace Pulsarc.Beatmaps
{
    /// <summary>
    /// A class used in the aid of loading and converting beatmaps.
    /// </summary>
    public class Arc
    {
        public int Time { get; protected set; }
        public int Type { get; protected set; }

        public Arc(int time, int type)
        {
            Time = time;
            Type = type;
        }

        /// <summary>
        /// Converts the arc into a string format, for use with beatmap (.psc)
        /// files.
        /// </summary>
        /// <returns>The arc as seen in .psc. "[Time],[ArcType]".
        /// e.g. A left-down arc at 432 milleseconds would look like "432,0110"</returns>
        public override string ToString()
        {
            string arcType = Convert.ToString(Type, 2).PadLeft(4, '0');

            return $"{Time},{arcType}";
        }
    }
}
