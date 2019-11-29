using System;
using System.Collections.Generic;
using Wobble.Discord;
using Wobble.Discord.RPC;

namespace Pulsarc.Utils
{
    static class PulsarcDiscord
    {
        private const string LOGO = "pulsarc_logo";

        private static double lastUpdateTime = -15000;

        private const double UPDATE_WAIT_TIME = 15000;

        private static bool Waiting => PulsarcTime.CurrentElapsedTime < lastUpdateTime + UPDATE_WAIT_TIME && waitingTriggered;
        private static bool waitingTriggered = false;

        public static RichPresence Presence { get; private set; }

        static public void Initialize()
        {
            // 604680029439393812 <- Adri's AppID, in case we go back to that one.
            // 649491503391047701 <- FRUP's AppID, currently being used.
            DiscordManager.CreateClient("649491503391047701", Wobble.Discord.RPC.Logging.LogLevel.None);
            Presence = new RichPresence();
            Presence.Assets = new Assets();
            Presence.Timestamps = new Timestamps();
        }

        static public void SetStatus(string details, string state, bool timer = false)
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

        static public void SetImage(string imagePath, bool toggleUpdate = true)
        {
            Presence.Assets.LargeImageKey = imagePath;

            if (toggleUpdate)
                UpdatePresence();
        }

        static public void SetState(string state, bool toggleUpdate = true)
        {
            Presence.State = state;

            if (toggleUpdate)
                UpdatePresence();
        }

        static public void SetDetails(string details, bool toggleUpdate = true)
        {
            Presence.Details = details;

            if (toggleUpdate)
                UpdatePresence();
        }

        static private void UpdatePresence()
        {
            DiscordManager.Client.SetPresence(Presence);
        }
    }
}
