using Microsoft.Xna.Framework;
using Pulsarc.Utils;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.MainMenu
{
    class Menu : PulsarcScreen
    {
        public override ScreenView View { get; protected set; }

        private const string DISCORD_DETAILS = "In the menus";

        public Menu()
        {
            View = new MenuView(this);
            PulsarcDiscord.SetStatus("", DISCORD_DETAILS);
        }

        public override void Update(GameTime gameTime) => View?.Update(gameTime);

        public override void UpdateDiscord()
        {
            if (PulsarcDiscord.Presence.Details != DISCORD_DETAILS)
            {
                PulsarcDiscord.SetStatus("", DISCORD_DETAILS);
            }
        }
    }
}
