using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Utils.Maths;
using System;

namespace Pulsarc.UI
{
    public class Drawable
    {
        public Texture2D texture;
        public Drawable hover = null;

        public bool hoverObject = false;

        public Vector2 position;
        public Vector2 drawPosition;
        public Vector2 origin;
        public Rectangle drawnPart;
        public Anchor anchor;

        // The current base size of this drawable in pixels.
        public Vector2 baseSize;

        // The opacity of this Drawable, 1 is fully opaque, 0 is fully transparent.
        public float opacity = 1f;

        protected float aspectRatio; // Used to force aspect ratio when using Resize
        protected float scale = 1;
        protected float rotation = 0;

        public Drawable(Texture2D texture, Vector2 position, Vector2 size, float aspectRatio = -1, Anchor anchor = Anchor.TopLeft)
        {
            origin = new Vector2(0, 0);
            this.texture = texture;
            this.aspectRatio = aspectRatio;
            this.anchor = anchor;

            drawnPart = new Rectangle(new Point(0, 0), new Point(texture.Width, texture.Height));

            Resize(size);
            changePosition(position);
        }

        public Drawable(Texture2D texture, Vector2 position, float aspectRatio = -1, Anchor anchor = Anchor.TopLeft)
            : this(texture, position, new Vector2(texture.Width, texture.Height), aspectRatio, anchor) { }


        public Drawable(Texture2D texture, float aspectRatio = -1, Anchor anchor = Anchor.TopLeft)
            : this(texture, new Vector2(0, 0), new Vector2(texture.Width, texture.Height), aspectRatio, anchor) { }


        public Drawable() { }

        public virtual void Resize(Vector2 size, bool resolutionScale = true)
        {
            baseSize = size;
            float newAspect = baseSize.X / baseSize.Y;
            if (aspectRatio != -1 && newAspect != aspectRatio)
            {
                Fraction aspect = new Fraction(newAspect);
                Console.WriteLine("Invalid aspect ratio : " + baseSize.X + "x" + baseSize.Y + " isn't " + aspect.ToString());
                return;
            }
            scale = baseSize.X / texture.Width *  (resolutionScale ? (Pulsarc.getDimensions().X / Pulsarc.xBaseRes) : 1);
        }

        public virtual void Resize(float size, bool resolutionScale = true)
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

        static public Vector2 getResponsivePosition(Vector2 position)
        {
            return new Vector2(position.X / Pulsarc.xBaseRes * Pulsarc.getDimensions().X, position.Y / Pulsarc.yBaseRes * Pulsarc.getDimensions().Y);
        }

        public void changePosition(float x, float y)
        {
            changePosition(new Vector2(x, y));
        }

        public void changePosition(Vector2 position)
        {
            this.position = getResponsivePosition(position);

            Vector2 newPos = position;
            /*Vector2 size;
            if (texture != null)
            {
                size = new Vector2(texture.Width, texture.Height);
            } else
            {
                size = new Vector2(0,0);
            }*/

            switch (anchor)
            {
                case Anchor.Center:
                    newPos.X -= baseSize.X / 2;
                    newPos.Y -= baseSize.Y / 2;
                    break;
                case Anchor.TopRight:
                    newPos.X -= baseSize.X;
                    break;
                case Anchor.CenterRight:
                    newPos.X -= baseSize.X;
                    newPos.Y -= baseSize.Y / 2;
                    break;
                case Anchor.BottomRight:
                    newPos.X -= baseSize.X;
                    newPos.Y -= baseSize.Y;
                    break;
                case Anchor.BottomLeft:
                    newPos.Y -= baseSize.Y;
                    break;
                case Anchor.CenterLeft:
                    newPos.Y -= baseSize.Y / 2;
                    break;
                case Anchor.TopLeft:
                default:
                    break;
            }

            drawPosition = getResponsivePosition(newPos);
        }

        public virtual void move(Vector2 position)
        {
            this.position += getResponsivePosition(position);
            this.drawPosition += getResponsivePosition(position);
        }

        public bool clicked(Vector2 mousePos)
        {
            return mousePos.X >= drawPosition.X && mousePos.X <= drawPosition.X + texture.Width && mousePos.Y >= drawPosition.Y && mousePos.Y <= drawPosition.Y + texture.Height;
        }

        public bool clicked(Point mousePos)
        {
            return mousePos.X >= drawPosition.X && mousePos.X <= drawPosition.X + texture.Width && mousePos.Y >= drawPosition.Y && mousePos.Y <= drawPosition.Y + texture.Height;
        }

        public bool onScreen()
        {
            return new Rectangle((int)drawPosition.X, (int)drawPosition.Y, texture.Width, texture.Height).Intersects(new Rectangle(0, 0, (int)Pulsarc.getDimensions().X, (int)Pulsarc.getDimensions().Y));
        }

        public bool intersects(Drawable drawable)
        {
            return new Rectangle((int)drawPosition.X, (int)drawPosition.Y, texture.Width, texture.Height)
                .Intersects(
                    new Rectangle((int) drawable.drawPosition.X, (int)drawable.drawPosition.Y, drawable.texture.Width, drawable.texture.Height));
        }

        public virtual void Draw()
        {
            Color color = Color.White;

            if (opacity != 1f)
                color *= opacity;

            Pulsarc.spriteBatch.Draw(texture, drawPosition, drawnPart, color, rotation, origin, scale, SpriteEffects.None, 0f);

            if(hover != null && clicked(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)))
            {
                hover.Draw();
            }
        }
    }
}
