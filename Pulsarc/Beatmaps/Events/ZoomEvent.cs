using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Utils;
using Pulsarc.Utils.Maths;
using Wobble.Logging;

namespace Pulsarc.Beatmaps.Events
{
    public enum ZoomType
    {
        // The lerp zoom-style of Intralism
        Intralizoom = -1,
        // The change in zoom happens instantaneously
        Step = 0,
        // The zoom transitions linearly over time
        Linear = 1,
    }

    /// <summary>
    /// An event that controls the level of zoom in the game
    /// </summary>
    public class ZoomEvent : Event
    {
        private enum ZoomParameter
        {
            // ZoomType index
            ZoomType = 0,
            // Level amount index
            ZoomLevel = 1,
            // End time index
            EndPoint = 2,
        }

        // The type of zoom for this event.
        public ZoomType ZoomType { get; private set; }

        // The zoom amount this event will go to.
        // In code, this represents the diameter of the crosshair in pixels.
        // In the .psc this represents the diameter of the crosshair in pixels on a 1080px high screen.
        // TODO?? This could represent the zLocation of the crosshair instead.
        public float ZoomLevel { get; private set; }
        
        // The time of the last frame, used in calculations to handle the next frame
        //private double lastFrameTime = -1;
        // This ZoomEvent's end time, for non-Intralizooms
        public int EndTime { get; private set; }

        // The current zoom level, used in calcuations to decide how to handle the next frame.
        private float currentZoomLevel;

        // The starting zoom level, used in calcuations to decide how to handle the next frame.
        // Negative Zoom Level is not necessary, so it is set to -1 to help set the startZoomLevel
        // when Handle is first called.
        private float startZoomLevel = -1;

        /// <summary>
        /// Initializes a ZoomEvent that sets its stats with "line".
        /// line should be in this format: [Time (ms)],1,[ZoomType],[ZoomLevel],[EndTime (ms)]
        /// </summary>
        /// <param name="line">A line from a .psc beatmap that defines an event.</param>
        /// <exception cref="WrongEventTypeException"></exception>
        public ZoomEvent(string line) : base(line)
        {
            // Check if the type for this event is "zoom"
            if (Type != EventType.Zoom)
                ThrowWrongEventTypeException(this, EventType.Zoom);

            // Find the remaining values using the List<string> Parameters
            // that was created in the base constructor.
            ZoomType = (ZoomType)int.Parse(Parameters[(int)ZoomParameter.ZoomType]);
            ZoomLevel = float.Parse(Parameters[(int)ZoomParameter.ZoomLevel]) * Pulsarc.HeightScale;
            EndTime = int.Parse(Parameters[(int)ZoomParameter.EndPoint]);
        }

        /// <summary>
        /// Initializes a ZoomEvent on the spot.
        /// </summary>
        /// <param name="time">The time this event is activated.</param>
        /// <param name="zoomType">The type of zoom this is.</param>
        /// <param name="zoomLevel">The amount of zoom to go to.</param>
        /// <param name="endTime">The deactivation time of this zoom event.</param>
        public ZoomEvent(int time, ZoomType zoomType, float zoomLevel, int endTime)
            : base(time, EventType.Zoom)
        {
            ZoomType = zoomType;
            ZoomLevel = zoomLevel * Pulsarc.HeightScale;
            EndTime = endTime;
        }

        /// <summary>
        /// Perform the actions this ZoomEvent is assigned to.
        /// </summary>
        /// <param name="gameplayEngine">The currently playing Gameplay Engine to modify the zoom of.</param>
        /// <exception cref="WrongEventTypeException"></exception>
        public override void Handle(GameplayEngine gameplayEngine)
        {
            // if the startZoomLevel hasn't been set, set it now.
            if (startZoomLevel == -1)
                startZoomLevel = gameplayEngine.Crosshair.Diameter;

            currentZoomLevel = gameplayEngine.Crosshair.Diameter;

            // Use the correct handling method depending on this
            // ZoomEvent's ZoomType
            switch (ZoomType)
            {
                case ZoomType.Intralizoom:
                    HandleIntralizooms(gameplayEngine);
                    break;
                case ZoomType.Step:
                    HandleStepZoom(gameplayEngine);
                    break;
                case ZoomType.Linear:
                    HandleLinearZooms(gameplayEngine);
                    break;
                default:
                    throw new WrongEventTypeException($"Invalid zoom type! ZoomTypes can be from -1 to 1, and the type called was {(int)ZoomType}!");
            }
        }

        /// <summary>
        /// Zooms the gameplay to imitate Intralism's zoom method.
        /// </summary>
        /// <param name="gameplayEngine">The gameplay engine to modify.</param>
        private void HandleIntralizooms(GameplayEngine gameplayEngine)
        {
            FindNextEvent(gameplayEngine);

            // If the next event is less than the current time
            // then deactivate this event as it is no longer needed.

            // This is similar to how Intralism handles PlayerDistance, where it has a
            // local PlayerDistance that is what the player sees, and a semi-constant "PlayerDistance"
            // That is changed by the most recent SetPlayerDistance map event.
            Active = !(nextEvent != null && nextEvent.Time < gameplayEngine.Time);

            // Get smoothDeltaTime and return if there was no change in deltaTime
            double smoothDeltaTime = PulsarcTime.SmoothDeltaTime;

            if (smoothDeltaTime == 0 || !Active)
                return;

            // Resize the crosshair
            gameplayEngine.Crosshair.Resize(
                PulsarcMath.Lerp(
                    // Starting position (current position)
                    currentZoomLevel,
                    // Ending position (zoomLevel)
                    ZoomLevel,
                    // Amount to lerp by (Pulsarc: deltaTime(ms) / 200 = Intralism: deltaTime(s) * 5)
                    (float)smoothDeltaTime / 200f));
        }

        /// <summary>
        /// Zooms the gameplay instantaneously to this ZoomEvents zoom level
        /// </summary>
        /// <param name="gameplayEngine">The gameplay engine to modify.</param>
        private void HandleStepZoom(GameplayEngine gameplayEngine)
        {
            gameplayEngine.Crosshair.Resize(ZoomLevel);
            Active = false;
        }

        /// <summary>
        /// Zooms the gameplay linearly from the starting position to the end position.
        /// </summary>
        /// <param name="gameplayEngine">The gameplay engine to modify.</param>
        private void HandleLinearZooms(GameplayEngine gameplayEngine)
        {
            // Find the amount of time between the activation time to the end time.
            double deltaTime = EndTime - Time;

            // Find the amount of zoom between the start zoom to the end zoom.
            float deltaZoom = ZoomLevel - startZoomLevel;

            // Find the amount of time between now and the event time
            double deltaCurrentTime = gameplayEngine.Time - Time;

            // Find the percentage of time that has past between the event time and the end time
            double zoomFactor = 1 - ((deltaTime - deltaCurrentTime) / deltaTime);

            // Find what the current zoom should be.
            float currentZoom = startZoomLevel + (deltaZoom * (float)zoomFactor);

            // If deltaZoom is negative, and currentZoom is less than ZoomLevel, then change currentZoom to ZoomLevel
            // If deltaZoom is positive, and currentZoom is greater than ZoomLevel, then change currentZoom to ZoomLevel
            // This makes sure zoom doesn't go over or under its intended level
            bool negative = deltaZoom < 0;

            if (negative && currentZoom < ZoomLevel)
                currentZoom = ZoomLevel;
            else if (!negative && currentZoom > ZoomLevel)
                currentZoom = ZoomLevel;

            // Resize the crosshair
            gameplayEngine.Crosshair.Resize(currentZoom);

            // Active as long as currentZoom hasn't reached zoom level
            // The above if-block makes sure that currentZoom will equal ZoomLevel eventually.
            Active = currentZoom != ZoomLevel;
        }

        /// <summary>
        /// Returns what this event looks like in the .psc beatmap
        /// file.
        /// Format is: [Time (ms)],1,[ZoomType],[ZoomLevel],[EndTime (ms)]"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Time},1,{(int)ZoomType},{ZoomLevel / Pulsarc.HeightScale},{EndTime}";
        }
    }
}
