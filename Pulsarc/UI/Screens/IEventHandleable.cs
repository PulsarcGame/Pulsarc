using Pulsarc.Beatmaps;
using Pulsarc.UI.Screens.Gameplay;

namespace Pulsarc.UI.Screens
{
    // A collection of methods to make event handling smooth
    // over both the Gameplay and Editor engines.
    public interface IEventHandleable
    {
        Beatmap GetCurrentBeatmap();

        bool HasCrosshair();
        Crosshair GetCrosshair();

        double GetCurrentTime();
    }
}
