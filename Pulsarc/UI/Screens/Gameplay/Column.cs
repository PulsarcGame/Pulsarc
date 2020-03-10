using System.Collections.Generic;
using System.Linq;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class Column
    {
        // Which side this column represents
        // Currently not being used.
        private int side;

        // All HitObjects for this column.
        public List<HitObject> HitObjects { get; private set; }

        // All the hitobjects to be updated
        public List<HitObject> UpdateHitObjects { get; private set; }

        /// <summary>
        /// A Column is a "track" where all arcs from a specific direction are kept track of.
        /// </summary>
        /// <param name="side"></param>
        public Column(int side)
        {
            this.side = side;
            HitObjects = new List<HitObject>();
            UpdateHitObjects = new List<HitObject>();
        }

        /// <summary>
        /// Adds the provided HitObject to this Column
        /// </summary>
        /// <param name="hitObject">The HitObject to add</param>
        /// <param name="speed">Current game speed</param>
        /// <param name="crosshairZLoc">The z-axis position of the Crosshair</param>
        public void AddHitObject(HitObject hitObject, double speed, float crosshairZLoc)
        {
            // If this HitObject is hittable, add it to the end of the list.
            if (hitObject.Hittable)
            {
                HitObjects.Add(hitObject);
                UpdateHitObjects.Add(hitObject);
            }
            // If this HitObject is not hittable (Fading Out Effect Arcs), add it to the front of the list.
            else
            {
                HitObjects.Insert(0, hitObject);
                UpdateHitObjects.Insert(0, hitObject);
            }
        }

        /// <summary>
        /// Organizes the hitobjects to avoid updating yet unseen hitobjects.
        /// </summary>
        public void SortUpdateHitObjects()
        {
            UpdateHitObjects.Sort((x, y) => x.Time.CompareTo(y.Time));
        }
    }
}
