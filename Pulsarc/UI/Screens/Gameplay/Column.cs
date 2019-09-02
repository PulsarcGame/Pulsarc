using System.Collections.Generic;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class Column
    {
        // Which side this column represents
        public int side;

        // All HitObjects for this column.
        public List<HitObject> hitObjects;

        //
        public List<KeyValuePair<long,HitObject>> updateHitObjects;
        
        /// <summary>
        /// A Column is a "track" where all arcs from a specific direction are kept track of.
        /// </summary>
        /// <param name="side"></param>
        public Column(int side)
        {
            this.side = side;
            hitObjects = new List<HitObject>();
            updateHitObjects = new List<KeyValuePair<long, HitObject>>();
        }

        /// <summary>
        /// Adds the provided HitObject to this Column
        /// </summary>
        /// <param name="hitObject">The HitObject to add</param>
        /// <param name="speed">Current game speed</param>
        /// <param name="crosshairZLoc">The z-axis position of the Crosshair</param>
        public void AddHitObject(HitObject hitObject, double speed, float crosshairZLoc)
        {
            if (hitObject.hittable)
            {
                hitObjects.Add(hitObject);
                updateHitObjects.Add(new KeyValuePair<long, HitObject>(hitObject.IsSeenAt(speed, crosshairZLoc), hitObject));
            }
            else
            {
                hitObjects.Insert(0, hitObject);
                updateHitObjects.Insert(0, new KeyValuePair<long, HitObject>(hitObject.IsSeenAt(speed, crosshairZLoc), hitObject));
            }
        }

        /// <summary>
        /// Organizes the hitobjects to avoid updating yet unseen hitobjects.
        /// </summary>
        public void SortUpdateHitObjects()
        {
            updateHitObjects.Sort((x, y) => x.Key.CompareTo(y.Key));
        }
    }
}
