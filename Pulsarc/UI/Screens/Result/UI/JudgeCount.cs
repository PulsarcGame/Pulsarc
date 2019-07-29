using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Result.UI
{
    class JudgeCount : TextDisplayElement
    {
        public string name;
        public JudgeCount(string name, Vector2 position, Color color, int fontSize = 20, Anchor anchor = Anchor.TopLeft) : base("", position, fontSize, anchor, color)
        {
            this.name = name;
        }

        public void Update(int value)
        {
            Update(value.ToString("#,#0"));
        }
    }
}
