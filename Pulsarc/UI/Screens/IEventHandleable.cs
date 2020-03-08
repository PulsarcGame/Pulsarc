using Pulsarc.Beatmaps;
using Pulsarc.UI.Screens.Gameplay;

namespace Pulsarc.UI.Screens
{
    // A collection of methods to make event handling smooth
    // over both the Gameplay and Editor engines.
    public interface IEventHandleable
    {
        /// <summary>
        /// Get the Beatmap being used by this EventHandleable
        /// </summary>
        /// <returns></returns>
        Beatmap GetCurrentBeatmap();

        /// <summary>
        /// Does this EventHandleable use a Crosshair?
        /// </summary>
        /// <returns></returns>
        bool HasCrosshair();

        /// <summary>
        /// Get the crosshair of this EventHandleable, if it has one.
        /// </summary>
        /// <returns>The crosshair object being used by this EventHandleable, or null if
        /// there is no crosshair.</returns>
        Crosshair GetCrosshair();

        /// <summary>
        /// Get the time of this EventHandleable.
        /// </summary>
        /// <returns></returns>
        double GetCurrentTime();
    }
}
