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

        public void AddHitObject(HitObject hitObject, double speed, int crosshairRadius)
        {
            hitObjects.Add(hitObject);
            //updateHitObjects.Add(new KeyValuePair<long,HitObject>(hitObject.IsSeenAt(hitObject.findZLocation(0, speed), speed), hitObject));
            updateHitObjects.Add(new KeyValuePair<long, HitObject>(hitObject.IsSeenAt(hitObject.findZLocation(0, speed, crosshairRadius), speed), hitObject)); //Needed to add crosshairRadius - FRUP
        }

        public void SortUpdateHitObjects()
        {
            // Organizes the hitobjects to avoid updating yet unseen hitobjects
            updateHitObjects.Sort((x, y) => x.Key.CompareTo(y.Key));
        }
    }
}