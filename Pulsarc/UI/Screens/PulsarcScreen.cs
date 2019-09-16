using System;
using System.Collections.Generic;
using System.Text;
using Wobble.Screens;

namespace Pulsarc.UI.Screens
{
    public abstract class PulsarcScreen : Screen
    {
        public bool initialized = false;
        public virtual void Init()
        {
            initialized = true;
        }
    }
}
