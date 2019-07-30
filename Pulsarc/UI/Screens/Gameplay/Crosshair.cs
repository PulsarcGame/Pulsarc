using System;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class Crosshair : Drawable
    {
        public Crosshair(int currentCrosshairRadius) : base(Skin.assets["crosshair"])
        {
            Vector2 screen = new Vector2(Pulsarc.xBaseRes, Pulsarc.xBaseRes / Pulsarc.baseRatio);
            float radius = currentCrosshairRadius;

            origin.X = (screen.X / 2) + ((texture.Width - screen.X) / 2);
            origin.Y = (screen.Y / 2) + ((texture.Height - screen.Y) / 2);
            position.X = screen.X / 2;
            position.Y = screen.Y / 2;
            Resize(radius);
            changePosition(position);

            rotation = (float)(45 * (Math.PI / 180));
        }
    }
}
