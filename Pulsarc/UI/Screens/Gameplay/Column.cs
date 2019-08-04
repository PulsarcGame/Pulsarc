using System.Collections.Generic;

namespace Pulsarc.UI.Screens.Gameplay
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

        public void AddHitObject(HitObject hitObject, double speed, double crosshairZLoc)
        {
            hitObjects.Add(hitObject);
            updateHitObjects.Add(new KeyValuePair<long, HitObject>(hitObject.IsSeenAt((int)hitObject.calcZLocation(0, speed, crosshairZLoc), speed, crosshairZLoc), hitObject));
        }

        public void SortUpdateHitObjects()
        {
            // Organizes the hitobjects to avoid updating yet unseen hitobjects
            updateHitObjects.Sort((x, y) => x.Key.CompareTo(y.Key));
        }
    }
}
