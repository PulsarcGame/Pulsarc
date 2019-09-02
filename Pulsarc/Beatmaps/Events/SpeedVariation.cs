using System.Globalization;
using Pulsarc.UI.Screens.Gameplay;

namespace Pulsarc.Beatmaps.Events
{
    public class SpeedVariation : Event
    {
        /// <summary>
        /// Initializes a SpeedVariation that sets its stats with "line",
        /// and traqcks 
        /// </summary>
        /// <param name="line"></param>
        public SpeedVariation(string line) : base(line)
        {
            // Check if the type for this event is "SpeedVariation"
            if (type != EventType.SpeedVariation)
            {
                ThrowWrongEventTypeException(this, EventType.SpeedVariation);
            }
        }

        public override void Handle(GameplayEngine gameplayEngine)
        {
            throw new System.NotImplementedException();
        }
    }
}
