using Pulsarc.UI.Screens.Gameplay;
using System;
using System.Collections.Generic;

namespace Pulsarc.Beatmaps.Events
{
    public enum EventType
    {
        Invalid = -1,
        Event = 0,
        Zoom = 1,
        SpeedVariation = 2,
    }

    public enum EventIndex
    {
        Time = 0, // Time is always index 0 in an event line
        Type = 1, // Type is always index 1 in an event line
    }

    abstract public class Event
    {
        // Time that this event activates (ms)
        public int time;

        // The type of event this is
        public EventType type;

        // The list of parameters for this event
        public List<string> parameters;

        // The list of all events of this type
        public List<Event> similarEvents;

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
            type = (EventType)int.Parse(parts.Dequeue());
            parameters.AddRange(parts);            
        }

        /// <summary>
        /// Creates a new Event using the time and type provided.
        /// Used by other Events that need a "nextEvent" for calculations.
        /// </summary>
        /// <param name="time">The this event activates</param>
        /// <param name="type">The type of event this is.</param>
        public Event(int time, EventType type)
        {
            this.time = time;
            this.type = type;

            parameters = new List<string>();
        }

        /// <summary>
        /// Finds the EventType for the string provided.
        /// </summary>
        /// <param name="type">The string to find the corresponding EventType of.</param>
        /// <returns>The appropriate EventType, or empty if one was not found.</returns>
        public static EventType GetEventType(string type)
        {
            
            switch (type.ToLower())
            {
                case "event":
                    return EventType.Event;
                case "zoom":
                    return EventType.Zoom;
                case "speedvariation":
                        return EventType.SpeedVariation;
                default:
                    return EventType.Invalid;
            }
        }

        public static EventType GetEventType(int type)
        {
            switch (type)
            {
                case (int)EventType.Event:
                    return EventType.Event;
                case (int)EventType.Zoom:
                    return EventType.Zoom;
                case (int)EventType.SpeedVariation:
                    return EventType.SpeedVariation;
                default:
                    return EventType.Invalid;
            }
        }

        /// <summary>
        /// Finds the string for the EventType provided.
        /// </summary>
        /// <param name="eventType">The EventType to find the corresponding string of. Can be null.</param>
        /// <returns>"event", "zoom", "speedvariation", or "invalid"</returns>
        public static string GetStringOfEventType(EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Event:
                    return "event";
                case EventType.Zoom:
                    return "zoom";
                case EventType.SpeedVariation:
                    return "speedvariation";
                default:
                    return "invalid";
            }
        }

        /// <summary>
        /// Throws a WrongEvenTypeException, protected so only Event-derived classes can use it.
        /// </summary>
        /// <param name="evnt">The event</param>
        /// <param name="eventType"></param>
        protected static void ThrowWrongEventTypeException(Event evnt, EventType eventType)
        {
            throw new WrongEventTypeException("The event [" + evnt.ToString() + "] was treated as a(n) " + GetStringOfEventType(eventType) + " event, when it is actually a(n) " + GetStringOfEventType(evnt.type) + " event instead!");
        }

        /// <summary>
        /// Returns this Event as a string in the same format as the string used in a.psc beatmap to define this event.
        /// </summary>
        /// <returns>This Event in this format: "[Time(ms)],[Type],[Parameter 1],[Parameter 2],[...],[Parameter n]"</returns>
        public override string ToString()
        {
            return time + "," + (int)type + "," + string.Join(',',parameters.ToArray());
        }

        /// <summary>
        /// An abstract method for how this Event should act while it's being handled.
        /// </summary>
        /// <param name="gameplayEngine">The currently running GameplayEngine this event being handled in.</param>
        public abstract void Handle(GameplayEngine gameplayEngine);

        public void findAllSimilarEvents(GameplayEngine gameplayEngine)
        {
            similarEvents = gameplayEngine.currentBeatmap.events.FindAll(FindSimilarEvent);
        }

        private bool FindSimilarEvent(Event evnt)
        {
            return type == evnt.type;
        }
    }

    /// <summary>
    /// A custom Exception for Event-derived objects to throw when
    /// they are being treated like an event they are not.
    /// </summary>
    public class WrongEventTypeException : Exception
    {
        public WrongEventTypeException() { }

        public WrongEventTypeException(string message)
            : base(message) { }
        
        public WrongEventTypeException(string message, Exception inner)
            : base(message, inner) { }
    }
}
