using Pulsarc.UI.Screens.Gameplay;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.Beatmaps
{
    abstract public class Event
    {
        // Time that this event activates
        public int time;

        // The type of event this is
        public string type;

        // The list of parameters for this event
        public List<string> parameters;

        // Whether or not this event is currently being handled
        public bool active = false;

        /// <summary>
        /// Creates a new Event from the string provided.
        /// </summary>
        /// <param name="line">A line from a .psc beatmap that defines an event.</param>
        public Event(string line)
        {
            parameters = new List<string>();
            Queue<string> parts = new Queue<string>(line.Split(','));

            time = int.Parse(parts.Dequeue());
            type = parts.Dequeue();
            parameters.AddRange(parts);            
        }

        /// <summary>
        /// Returns this Event as a string in the same format as the string used in a.psc beatmap to define this event.
        /// </summary>
        /// <returns>This Event in this format: "[Time(ms)],[Type],[Parameter 1],[Parameter 2],[...],[Parameter n]"</returns>
        public override string ToString()
        {
            return time + "," + type + "," + string.Join(',',parameters.ToArray());
        }

        /// <summary>
        /// An abstract method for how this Event should act while it's being handled.
        /// </summary>
        /// <param name="gameplayEngine">The currently running GameplayEngine this event being handled in.</param>
        public abstract void Handle(GameplayEngine gameplayEngine);
    }
}
