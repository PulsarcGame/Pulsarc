using Wobble.Discord;
using Wobble.Discord.RPC;

namespace Pulsarc.Utils
{
    static class PulsarcDiscord
    {
        static RichPresence presence;
        static public void Initialize()
        {
            DiscordManager.CreateClient("604680029439393812", Wobble.Discord.RPC.Logging.LogLevel.None);
            presence = new RichPresence();
            DiscordManager.Client.SetPresence(presence);
        }
    }
}
