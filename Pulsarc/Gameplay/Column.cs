using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulsarc.Gameplay
{
    public class Column
    {
        public int side;
        public List<HitObject> hitObjects;
        public List<KeyValuePair<long,HitObject>> updateHitObjects;

        public Column(int side)
        {
            this.side = side;
            hitObjects = new List<HitObject>();
            updateHitObjects = new List<KeyValuePair<long, HitObject>>();
        }

        public void AddHitObject(HitObject hitObject)
        {
            hitObjects.Add(hitObject);
            updateHitObjects.Add(new KeyValuePair<long,HitObject>(hitObject.IsSeenAt(hitObject.getDistanceToCrosshair(0, 1), 1), hitObject));
        }

        public void SortUpdateHitObjects()
        {
            updateHitObjects.Sort((x, y) => x.Key.CompareTo(y.Key));
        }
    }
}
