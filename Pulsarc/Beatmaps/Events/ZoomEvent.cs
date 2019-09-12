using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Utils;
using Pulsarc.Utils.Maths;

namespace Pulsarc.Beatmaps.Events
{
    public enum ZoomType
    {
        Intralizoom = -1, // The zoom style of Intralism
        Step = 0, // The change in zoom happens instantaneously
        Linear = 1, // The zoom transitions in a linear manner
    }

    /// <summary>
    /// 
    /// </summary>
    public class ZoomEvent : Event
    {
        private const int IntralismZoomTime = 1850; // Time in milleseconds it takes to complete the Intralism zoom animation

        private enum ZoomParameter
        {
            ZoomType = 0, // ZoomType is always index 0 in parameters
            ZoomLevel = 1, // ZoomLevel is always index 1 in parameters
            EndPoint = 2, // EndPoint time is always index 2 in parameters
        }

        // The type of zoom for this event.
        public ZoomType zoomType;

        // The end zoom level
        // Currently, this level represents the size of the crosshair in pixels on a 1920x1080 base canvas.
        // TODO?: This could represent the zLocation of the crosshair instead.
        public float zoomLevel;

        // The current zoom level, used in calcuations to decide how to handle the next frame.
        public float currentZoomLevel;

        // The starting zoom level, used in calcuations to decide how to handle the next frame.
        // Negative Zoom Level is not necessary, so it is set to -1 to help set the startZoomLevel
        // when Handle is first called.
        public float startZoomLevel = -1;

        // The index of this zoom.
        private int index;

        // This ZoomEvent's end time, for non-Intralizooms
        private int endTime;

        private bool similarEventsCalled = false;

        // The time of the last frame, used in calculations to handle the next frame
        //private double lastFrameTime = -1;

        /// <summary>
        /// Initializes a ZoomEvent that sets its stats with "line".
        /// line should be in this format: [Time (ms)],1,[ZoomType],[ZoomLevel],[EndTime (ms)]
        /// </summary>
        /// <param name="line">A line from a .psc beatmap that defines an event.</param>
        /// <exception cref="WrongEventTypeException"></exception>
        public ZoomEvent(string line) : base(line)
        {
            // Check if the type for this event is "zoom"
            if (type != EventType.Zoom)
            {
                ThrowWrongEventTypeException(this, EventType.Zoom);
            }

            zoomType = (ZoomType)int.Parse(parameters[(int)ZoomParameter.ZoomType]);
            zoomLevel = float.Parse(parameters[(int)ZoomParameter.ZoomLevel]);
            endTime = int.Parse(parameters[(int)ZoomParameter.EndPoint]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameplayEngine"></param>
        /// <exception cref="WrongEventTypeException"></exception>
        public override void Handle(GameplayEngine gameplayEngine)
        {
            // if the startZoomLevel hasn't been set, set it now.
            if (startZoomLevel == -1) startZoomLevel = gameplayEngine.crosshair.diameter;

            currentZoomLevel = gameplayEngine.crosshair.diameter;

            // Use the correct handling method depending on this
            // ZoomEvent's ZoomType
            switch (zoomType)
            {
                case ZoomType.Intralizoom:
                    handleIntralizooms(gameplayEngine);
                    break;
                case ZoomType.Step:
                    handleStepZoom(gameplayEngine);
                    break;
                case ZoomType.Linear:
                    handleLinearZooms(gameplayEngine);
                    break;
                default:
                    throw new WrongEventTypeException("Invalid zoom type! ZoomTypes can be from -1 to 1, and the type called was " + (int)zoomType + "!");
            }
        }

        /// <summary>
        /// Zooms the gameplay to imitate Intralism's zoom method.
        /// </summary>
        /// <param name="gameplayEngine">The gameplay engine to modify.</param>
        private void handleIntralizooms(GameplayEngine gameplayEngine)
        {
            /* If lastFrameTime hasn't been set yet, set it.
            if (lastFrameTime == -1)
            {
                lastFrameTime = gameplayEngine.time;
                return;
            }*/

            if (!similarEventsCalled)
            {
                findAllSimilarEvents(gameplayEngine);
                similarEventsCalled = true;
            }

            // If any event has a greater time than this, and the current time is past
            // that event's time, then deactivate this event as it is no longer needed.

            // This is similar to how Intralism handles PlayerDistance, where it has a
            // local PlayerDistance that is what the player sees, and a semi-constant "PlayerDistance"
            // That is changed by the most recent SetPlayerDistance map event.
            foreach (Event evnt in similarEvents)
            {
                if (evnt.time > time && evnt.time < gameplayEngine.time)
                {
                    active = false;
                    return;
                }
            }

            double deltaTime = PulsarcTime.SmoothDeltaTime;
            if (deltaTime == 0)
            {
                return;
            }

            gameplayEngine.crosshair.Resize(PulsarcMath.Lerp(gameplayEngine.crosshair.diameter, zoomLevel, (float)deltaTime / 100f));
        }

        /// <summary>
        /// Zooms the gameplay instantaneously to this ZoomEvents zoom level
        /// </summary>
        /// <param name="gameplayEngine">The gameplay engine to modify.</param>
        private void handleStepZoom(GameplayEngine gameplayEngine)
        {
            gameplayEngine.crosshair.Resize(zoomLevel);
            active = false;
        }

        /// <summary>
        /// Zooms the gameplay linearly from the starting position to the end position.
        /// </summary>
        /// <param name="gameplayEngine">The gameplay engine to modify.</param>
        private void handleLinearZooms(GameplayEngine gameplayEngine)
        {
            // Comments show an example to help aid understanding of the code.
            double deltaTime = endTime - time; // 1000 ms
            float deltaZoom = zoomLevel - startZoomLevel; // -200 px

            double deltaCurrentTime = gameplayEngine.time - time; // Variable (between time and endPoint) between 1-1000 ms

            double zoomFactor = 1 - ((deltaTime - deltaCurrentTime) / deltaTime); // 1 - (1000 - variable) / 1000, supposed to represent the percentage of time that has past between time and endPoint


            float currentZoom = startZoomLevel + (deltaZoom * (float)zoomFactor); // What should current zoom be right now? 300 + (-200 * (0-1)) = (between 300-100)

            bool negative = deltaZoom < 0;

            // If deltaZoom is negative, and currentZoom is less than zoomLevel, then change currentZoom to zoomLevel
            // If deltaZoom is positive, and currentZoom is greater than zoomLevel, then change currentZoom to zoomLevel
            // Otherwise, keep currentZoom the same
            // This makes sure zoom doesn't go over or under its intended goal
            if (negative && currentZoom < zoomLevel)
            {
                currentZoom = zoomLevel;
            }
            else if (!negative && currentZoom > zoomLevel)
            {
                currentZoom = zoomLevel;
            }

            gameplayEngine.crosshair.Resize(currentZoom);

            active = currentZoom != zoomLevel;
        }
    }
}
