using Wobble.Screens;

namespace Pulsarc.UI.Screens
{
    public abstract class PulsarcScreen : Screen
    {
        public bool Initialized { get; protected set; } = false;

        public virtual void Init() => Initialized = true;

        public abstract void UpdateDiscord();

        /// <summary>
        /// This method is called by Pulsarc.CheckScreen() whenever a screen is entered or reentered.
        /// </summary>
        public virtual void EnteredScreen() => UpdateDiscord();
    }
}
