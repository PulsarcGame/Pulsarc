using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsarc.Gameplay
{
    class Column
    {
        public int side;
        public List<HitObject> hitObjects;

        public Column(int side)
        {
            this.side = side;
            hitObjects = new List<HitObject>();
        }
    }
}
