using System;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class Crosshair : Drawable
    {
        public double radius;

        public Crosshair(double currentCrosshairRadius) : base(Skin.assets["crosshair"])
        {
            Vector2 screen = new Vector2(Pulsarc.xBaseRes, Pulsarc.xBaseRes / Pulsarc.baseRatio);
            radius = currentCrosshairRadius;

            origin.X = (screen.X / 2) + ((texture.Width - screen.X) / 2);
            origin.Y = (screen.Y / 2) + ((texture.Height - screen.Y) / 2);
            position.X = screen.X / 2;
            position.Y = screen.Y / 2;
            Resize((float)radius);
            changePosition(position);

            rotation = (float)(45 * (Math.PI / 180));
        }

        public double getZLocation()
        {
            return 960 * (texture.Width / 2) / radius;
        }
    }
}
