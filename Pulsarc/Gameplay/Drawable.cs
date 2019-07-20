using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pulsarc.Utils.Maths;
using System;

namespace Pulsarc.Gameplay
{
    class Drawable
    {
        public Texture2D texture;

        public Vector2 position;
        public Vector2 origin;

        protected float aspectRatio; // Used to force aspect ratio when using Resize
        protected float scale = 1;
        protected float rotation = 0;

        public Drawable(Texture2D texture, Vector2 position, Vector2 size, float aspectRatio = -1)
        {
            origin = new Vector2(0, 0);
            this.texture = texture;
            this.position = position;
            this.aspectRatio = aspectRatio;
            Resize(size);
        }

        public Drawable(Texture2D texture, Vector2 position, float aspectRatio = -1)
            : this(texture, position, new Vector2(texture.Width, texture.Height), aspectRatio) {}
        

        public Drawable(Texture2D texture, float aspectRatio = -1) 
            : this(texture, new Vector2(0, 0), new Vector2(texture.Width, texture.Height), aspectRatio) { }

        public void Resize(Vector2 size)
        {
            float newAspect = size.X / size.Y;
            if (aspectRatio != -1 && newAspect != aspectRatio)
            {
                Fraction aspect = new Fraction(newAspect);
                Console.WriteLine("Invalid aspect ratio : " + size.X + "x" + size.Y + " isn't " + aspect.ToString());
                return;
            }
            scale = size.X / texture.Width;
        }

        public void Resize(float size)
        {
            Resize(new Vector2(size, size));
        }

        public void setRotation(float degrees)
        {
            if(degrees>=-360 && degrees<=360)
            {
                rotation = (float) (degrees * (Math.PI / 180));
            }
        }

        public void Draw()
        {
            Pulsarc.spriteBatch.Draw(texture, position, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
        }
    }
}
