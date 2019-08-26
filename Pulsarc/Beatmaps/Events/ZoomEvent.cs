using Pulsarc.UI.Screens.Gameplay;
using System;
using System.Collections.Generic;

namespace Pulsarc.Beatmaps.Events
{
    public enum ZoomType
    {
        Intralizoom = -1, // The zoom style of Intralism
        Step = 0, // The change in zoom happens instantaneously
        Linear = 1, // The zoom transitions in a linear manner
        EndPoint = -69, // This is a special ZoomType that does not effect the playfield, but is used to assist with Linear, Spline, and other zoom types that don't have a "next event" reference point.
    }

    public class ZoomEvent : Event
    {
        private enum ZoomParameter
        {
            ZoomType = 0, // ZoomType is always index 0 in parameters
            ZoomLevel = 1, //ZoomLevel is always index 1 in paraemeters
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
        public float startZoomLevel;

        // The index of this zoom.
        private int index;


        // The next ZoomEvent
        private ZoomEvent nextZoomEvent;

        /// <summary>
        /// Initializes a ZoomEvent that sets its stats with "line", and tracks
        /// its position in relation to other ZoomEvents using index
        /// </summary>
        /// <param name="line">A line from a .psc beatmap that defines an event.</param>
        /// <param name="index">The index of this ZoomEvent in relation to all other ZoomEvents in a map.</param>
        /// <exception cref="WrongEventTypeException"></exception>
        public ZoomEvent(string line, int index) : base(line)
        {
            // Check if the type for this event is "zoom"
            if (type != EventType.Zoom)
            {
                ThrowWrongEventTypeException(this, EventType.Zoom);
            }

            this.index = index;
            zoomType = (ZoomType)int.Parse(parameters[(int)ZoomParameter.ZoomType]);
            zoomLevel = float.Parse(parameters[(int)ZoomParameter.ZoomLevel]);
        }

        /// <summary>
        /// Initializes an EndPoint ZoomEvent. Used by ZoomEvents that don't
        /// have a nextZoomEvent provided by the beatmap
        /// </summary>
        /// <param name="time">The time of this ZoomEvent</param>
        public ZoomEvent(int time) : base(time, EventType.Zoom)
        {
            zoomType = ZoomType.EndPoint;

            zoomLevel = -1;
            startZoomLevel = -1;
            currentZoomLevel = -1;

            index = -1;

            nextZoomEvent = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameplayEngine"></param>
        /// <exception cref="WrongEventTypeException"></exception>
        public override void Handle(GameplayEngine gameplayEngine)
        {
            if (zoomType != ZoomType.EndPoint && nextZoomEvent == null)
            {
                startZoomLevel = gameplayEngine.crosshair.diameter;
                findAllSimilarEvents(gameplayEngine);

                try
                {
                    nextZoomEvent = (ZoomEvent)similarEvents[index + 1];
                }
                // If there isn't a next zoom event, make a personal EndPoint zoom 100 ms from now.
                catch (ArgumentOutOfRangeException e)
                {
                    nextZoomEvent = new ZoomEvent(time + 100);
                }
            }

            currentZoomLevel = gameplayEngine.crosshair.diameter;

            switch(zoomType)
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
                case ZoomType.EndPoint:
                    break;
                default:
                    throw new WrongEventTypeException("Invalid zoom type! ZoomTypes can be from 0 to 2, and the type called was " + zoomType + "!");
            }
        }

        private void handleIntralizooms(GameplayEngine gameplayEngine)
        {

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

        private void handleLinearZooms(GameplayEngine gameplayEngine)
        {
            double deltaTime = nextZoomEvent.time - time; // 1000
            float deltaZoom = zoomLevel - startZoomLevel; // 100

            double deltaCurrentTime = gameplayEngine.time - time; // Variable

            double zoomFactor = 1 - ((deltaTime - deltaCurrentTime) / deltaTime); // (1000 - variable) / 1000

            float currentZoom = startZoomLevel + (deltaZoom * (float)zoomFactor);

            bool negative = deltaZoom < 0;

            // If deltaZoom is negative, and currentZoom is less than zoomLevel, then change currentZoom to zoomLevel
            // If deltaZoom is positive, and currentZoom is greater than zoomLevel, then change currentZoom to zoomLevel
            // Otherwise, keep currentZoom the same
            currentZoom = negative ? (currentZoom < zoomLevel ? zoomLevel : currentZoom) : (currentZoom > zoomLevel ? zoomLevel : currentZoom);

            gameplayEngine.crosshair.Resize(currentZoom);

            active = currentZoom == zoomLevel;
        }
    }
}
