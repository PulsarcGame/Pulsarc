using System.Collections.Generic;
using Wobble.Discord;
using Wobble.Discord.RPC;

namespace Pulsarc.Utils
{
    static class PulsarcDiscord
    {
        private const string LOGO = "pulsarc_logo";

        private const double LastUpdateTime = -15000;

        private const double UpdateWaitTime = 15000;

        private static bool Waiting => PulsarcTime.CurrentElapsedTime < LastUpdateTime + UpdateWaitTime && waitingTriggered;
        private static bool waitingTriggered = false;

        public static RichPresence Presence { get; private set; }

        public static void Initialize()
        {
            Dictionary<string, string> appIdSwitch = new Dictionary<string, string>()
            {
                { "Adri", "604680029439393812" },
                { "FRUP", "600885985219444741" },
                { "Rubik", "649491503391047701"}
            };
            
            DiscordManager.CreateClient(appIdSwitch["Rubik"]);
            Presence = new RichPresence {Assets = new Assets(), Timestamps = new Timestamps()};
        }

        public static void SetStatus(string details, string state, bool timer = false)
        {
            SetDetails(details, false);
            SetState(state, false);

            if (Presence.Assets.LargeImageKey != LOGO)
                SetImage(LOGO, false);

            // Can't figure the timer right now, will get to it later - FRUP
            // if (timer)
            //   Presence.Timestamps.Start = DateTime.Now.Add(DateTime.UnixEpoch);
            //else
            //    Presence.Timestamps.Start = null;*/

            //Presence.Timestamps.End = DateTime.Now.AddMilliseconds(timer);

            UpdatePresence();
        }

        private static void SetImage(string imagePath, bool toggleUpdate = true)
        {
            Presence.Assets.LargeImageKey = imagePath;

            if (toggleUpdate)
                UpdatePresence();
        }

        private static void SetState(string state, bool toggleUpdate = true)
        {
            Presence.State = state;

            if (toggleUpdate)
                UpdatePresence();
        }

        private static void SetDetails(string details, bool toggleUpdate = true)
        {
            Presence.Details = details;

            if (toggleUpdate)
                UpdatePresence();
        }

        private static void UpdatePresence()
        {
            DiscordManager.Client.SetPresence(Presence);
        }
    }
}
