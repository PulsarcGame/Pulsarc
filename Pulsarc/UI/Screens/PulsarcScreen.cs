using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Pulsarc.Utils;
using Wobble.Screens;

namespace Pulsarc.UI.Screens
{
    public abstract class PulsarcScreen : Screen
    {
        public bool Initialized { get; protected set; } = false;

        public virtual string DiscordDetails { get; protected set; } = "";
        public virtual string DiscordState { get; protected set; } = "";
        public virtual bool DiscordTimer => false;

        public virtual void Init()
        {
            Initialized = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (PulsarcDiscord.Presence.Details != DiscordDetails || PulsarcDiscord.Presence.State != DiscordState)
                PulsarcDiscord.SetStatus(DiscordDetails, DiscordState, DiscordTimer);
        }
    }
}
