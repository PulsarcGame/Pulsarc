using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Utils.Maths;
using System;

namespace Pulsarc.UI
{
    public class Drawable
    {
        public Texture2D texture;

        public Vector2 position;
        public Vector2 origin;
        public Rectangle drawnPart;

        protected float aspectRatio; // Used to force aspect ratio when using Resize
        protected float scale = 1;
        protected float rotation = 0;

        public Drawable(Texture2D texture, Vector2 position, Vector2 size, float aspectRatio = -1)
        {
            origin = new Vector2(0, 0);
            this.texture = texture;
            this.aspectRatio = aspectRatio;

            drawnPart = new Rectangle(new Point(0, 0), new Point(texture.Width, texture.Height));

            Resize(size);
            changePosition(position);
        }

        public Drawable(Texture2D texture, Vector2 position, float aspectRatio = -1)
            : this(texture, position, new Vector2(texture.Width, texture.Height), aspectRatio) { }


        public Drawable(Texture2D texture, float aspectRatio = -1)
            : this(texture, new Vector2(0, 0), new Vector2(texture.Width, texture.Height), aspectRatio) { }


        public Drawable() { }

        public void Resize(Vector2 size, bool resolutionScale = true)
        {
            float newAspect = size.X / size.Y;
            if (aspectRatio != -1 && newAspect != aspectRatio)
            {
                Fraction aspect = new Fraction(newAspect);
                Console.WriteLine("Invalid aspect ratio : " + size.X + "x" + size.Y + " isn't " + aspect.ToString());
                return;
            }
            scale = size.X / texture.Width *  (resolutionScale ? (Pulsarc.getDimensions().X / Pulsarc.xBaseRes) : 1);
        }

        public void Resize(float size, bool resolutionScale = true)
        {
            Resize(new Vector2(size, size), resolutionScale);
        }

        public void setRotation(float degrees)
        {
            if(degrees>=-360 && degrees<=360)
            {
                rotation = (float) (degrees * (Math.PI / 180));
            }
        }

        public void changePosition(Vector2 position)
        {
            this.position = new Vector2(position.X / 1920 * Pulsarc.getDimensions().X, position.Y / 1080 * Pulsarc.getDimensions().Y);
        }

        public virtual void Draw()
        {
            Pulsarc.spriteBatch.Draw(texture, position, drawnPart, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
        }
    }
}
