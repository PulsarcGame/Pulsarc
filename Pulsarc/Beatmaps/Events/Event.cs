using Pulsarc.UI.Screens.Gameplay;
using System;
using System.Collections.Generic;

namespace Pulsarc.Beatmaps.Events
{
    /// <summary>
    /// Enum for the different types of Events there can be
    /// </summary>
    public enum EventType
    {
        Invalid = -1,
        Event = 0,
        Zoom = 1,
        SpeedVariation = 2,
    }

    /// <summary>
    /// Enum that keeps track of parameter indexes that never change
    /// </summary>
    public enum EventIndex
    {
        // EventTime index
        Time = 0,
        // EventType index
        Type = 1,
    }

    /// <summary>
    /// A custom Exception for Event-derived objects to throw when
    /// they are being treated like an event that they are not.
    /// </summary>
    public class WrongEventTypeException : Exception
    {
        public WrongEventTypeException() { }

        public WrongEventTypeException(string message)
            : base(message) { }

        public WrongEventTypeException(string message, Exception inner)
            : base(message, inner) { }
    }

    /// <summary>
    /// Gameplay Elements, Zooms, SpeedVariation, Rotation, etc.
    /// </summary>
    public abstract class Event
    {
        // The time when this event activates (ms)
        public int Time { get; protected set; }

        // The type of event this is
        public EventType Type { get; protected set; }

        // The list of parameters for this event
        public List<string> Parameters { get; protected set; } = new List<string>();

        // The list of all events of this type for the beatmap
        // Legacy?
        protected List<Event> similarEvents = new List<Event>();
        protected bool similarEventsCalled = false;
        protected List<Event> futureEvents = new List<Event>();
        protected bool futureEventsCalled = false;

        // The next similar event in gameplay according to Time
        // Set in FindNextEvent
        protected Event nextEvent;
        protected bool foundNextEvent = false;

        // Whether or not this event is currently being handled
        public bool Active { get; set; } = false;

        /// <summary>
        /// Creates a new Event from the string provided.
        /// </summary>
        /// <param name="line">A line from a .psc beatmap that defines an event.</param>
        public Event(string line)
        {
            Queue<string> parts = new Queue<string>(line.Split(','));

            Time = int.Parse(parts.Dequeue());
            Type = (EventType)int.Parse(parts.Dequeue());
            Parameters.AddRange(parts);
        }

        /// <summary>
        /// Currently not used. Keeping in case it proves useful in the future.
        /// Creates a new Event using the time and type provided.
        /// Used by other Events that need a "nextEvent" for calculations.
        /// </summary>
        /// <param name="time">The this event activates</param>
        /// <param name="type">The type of event this is.</param>
        private Event(int time, EventType type)
        {
            Time = time;
            Type = type;

            Parameters = new List<string>();
        }

        /// <summary>
        /// Finds the EventType for the string provided.
        /// </summary>
        /// <param name="type">The string to find the corresponding EventType of.</param>
        /// <returns>The appropriate EventType, or EventType.Invalid if one was not found.</returns>
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

        /// <summary>
        /// Finds the EventType for the int provided.
        /// </summary>
        /// <param name="type">The int to find the corresponding EventType of.</param>
        /// <returns>The appropriate EventType, or EventType.Invalid if one was not found.</returns>
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
            throw new WrongEventTypeException($"The event [{evnt.ToString()}] was treated as a(n) " +
                $"{GetStringOfEventType(eventType)} event, when it is actually a(n) " +
                $"{GetStringOfEventType(evnt.Type)} event instead!");
        }

        /// <summary>
        /// Returns this Event as a string in the same format as the string used in a .psc beatmap to define this event.
        /// </summary>
        /// <returns>This Event in this format: "[Time(ms)],[EventType],[Parameter 1],[Parameter 2],[...],[Parameter n]"</returns>
        public override string ToString()
        {
            int type = (int)Type;
            string parameters = string.Join(',', Parameters.ToArray());

            return $"{Time},{type},{parameters}";
        }

        /// <summary>
        /// An abstract method for how this Event should act while it's being handled.
        /// </summary>
        /// <param name="gameplayEngine">The currently running GameplayEngine this event being handled in.</param>
        public abstract void Handle(GameplayEngine gameplayEngine);

        /// <summary>
        /// Find the next event in gameplayEngine that shares the same type as this Event
        /// and has a greater time than this event.
        /// </summary>
        /// <param name="gameplayEngine">The gameplayEngine to look through</param>
        protected void FindNextEvent(GameplayEngine gameplayEngine)
        {
            if (foundNextEvent)
                return;

            FindAllSimilarEvents(gameplayEngine);

            nextEvent = gameplayEngine.CurrentBeatmap.Events.Find(GreaterEventTime);
            foundNextEvent = true;
        }

        /// <summary>
        /// Whether or not the provided event has a greater time than this event.
        /// </summary>
        /// <param name="evt">The event to compare to</param>
        /// <returns>True if evnt's time is greater than this.</returns>
        private bool GreaterEventTime(Event evnt)
        {
            return evnt.Time > Time && SameEventType(evnt);
        }

        /// <summary>
        /// Currently not used. Keeping in case it proves useful in the future.
        /// Find all events in gameplayEngine that share the same type as this Event.
        /// </summary>
        /// <param name="gameplayEngine">The gameplayEngine to look through</param>
        protected void FindAllSimilarEvents(GameplayEngine gameplayEngine)
        {
            // Only need to call this once, inherited classes can set similarEventsCalled to false if they want to call again.
            if (similarEventsCalled)
                return;

            similarEvents = gameplayEngine.CurrentBeatmap.Events.FindAll(SameEventType);
            similarEventsCalled = true;
        }

        /// <summary>
        /// Currently not used. Keeping in case it proves useful in the future.
        /// Find all events in gameplayEngine that share the same type as this Event
        /// and have a greater time than this event.
        /// </summary>
        /// <param name="gameplayEngine">The gameplayEngine to look through</param>
        protected void FindAllSimilarFutureEvents(GameplayEngine gameplayEngine)
        {
            if (futureEventsCalled)
                return;

            FindAllSimilarEvents(gameplayEngine);

            futureEvents = similarEvents.FindAll(GreaterEventTime);
            futureEventsCalled = true;
        }

        /// <summary>
        /// Currently not used. Keeping in case it proves useful in the future.
        /// Whether or not the provided event has the same type as this event.
        /// </summary>
        /// <param name="evt">The event to compare to</param>
        /// <returns>True if evt and this Event share the same type.</returns>
        private bool SameEventType(Event evnt)
        {
            return evnt.Type == Type;
        }

    }
}
