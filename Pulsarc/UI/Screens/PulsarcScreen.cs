using System;
using System.Collections.Generic;
using System.Text;
using Wobble.Screens;

namespace Pulsarc.UI.Screens
{
    public abstract class PulsarcScreen : Screen
    {
        public bool Initialized { get; protected set; } = false;

        public virtual void Init()
        {
            Initialized = true;
        }
    }
}
