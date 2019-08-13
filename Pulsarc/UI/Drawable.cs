using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Utils.Maths;
using System;

namespace Pulsarc.UI
{
    public class Drawable
    {
        /// <summary>The texture for this Drawable.</summary>
        public Texture2D texture;
        /// <summary>If the cursor is hovering this Drawable, it will be changed to the "hover" Drawable.</summary>
        public Drawable hover = null;

        public bool hoverObject = false;

        /// <summary>The position of this Drawable, relative to its anchor.</summary>
        public Vector2 position;
        /// <summary> </summary>
        public Vector2 drawPosition;
        /// <summary>The origin (center) of this Drawable.</summary>
        public Vector2 origin;
        /// <summary>What part of the Drawable is drawn, determined by a Rectangle object.</summary>
        public Rectangle drawnPart;
        /// <summary>The Anchor of the drawable, determines how this Drawable will be rendered in relation to its position.</summary>
        public Anchor anchor;

        /// <summary>Used to force aspect ratio when using Resize.</summary>
        protected float aspectRatio;
        /// <summary>How big this Drawable is compared to its base texture dimensions.</summary>
        protected float scale = 1;
        /// <summary>The angle of this Drawable.</summary>
        protected float rotation = 0;

        /// <summary>
        /// A 2D object that can be rendered on screen.
        /// </summary>
        /// <param name="texture">The texture for this Drawable.</param>
        /// <param name="position">The position of this Drawable, relative to the base dimensions (1920 x 1080)</param>
        /// <param name="size">How large this object is (in pixels).</param>
        /// <param name="aspectRatio">The aspect ratio of this Drawable. Leave at default of -1 unless this Drawable needs to be displayed with a consistent aspect ratio.</param>
        /// <param name="anchor">The anchor of this Drawable, the default is TopLeft, which means that the Drawable will be drawn below and to the right of the position.</param>
        public Drawable(Texture2D texture, Vector2 position, Vector2 size, float aspectRatio = -1, Anchor anchor = Anchor.TopLeft)
        {
            // Define variables
            origin = new Vector2(0, 0);
            this.texture = texture;
            this.aspectRatio = aspectRatio;
            this.anchor = anchor;

            drawnPart = new Rectangle(new Point(0, 0), new Point(texture.Width, texture.Height));

            // Resize to the defined size.
            Resize(size);
            // set this Drawabl's position.
            changePosition(position);
        }

        /// <summary>
        /// A 2D object that can be rendered on screen, using the size of the texture provided.
        /// </summary>
        /// <param name="texture">The texture for this Drawable. The dimensions determine how large this object will be.</param>
        /// <param name="position">The position of this Drawable, relative to the base dimensions (1920 x 1080)</param>
        /// <param name="aspectRatio">The aspect ratio of this Drawable. Leave at default of -1 unless this Drawable needs to be displayed with a consistent aspect ratio.</param>
        /// <param name="anchor">The anchor of this Drawable, the default is TopLeft, which means that the Drawable will be drawn below and to the right of the position.</param>
        public Drawable(Texture2D texture, Vector2 position, float aspectRatio = -1, Anchor anchor = Anchor.TopLeft)
            : this(texture, position, new Vector2(texture.Width, texture.Height), aspectRatio, anchor) { }


        /// <summary>
        /// A 2D object that can be rendered on screen, using the size of the texture provided, and with a starting location of (0, 0).
        /// </summary>
        /// <param name="texture">The texture for this Drawable. The dimensions determine how large this object will be.</param>
        /// <param name="aspectRatio">The aspect ratio of this Drawable. Leave at default of -1 unless this Drawable needs to be displayed with a consistent aspect ratio.</param>
        /// <param name="anchor">The anchor of this Drawable, the default is TopLeft, which means that the Drawable will be drawn below and to the right of the position.</param>
        public Drawable(Texture2D texture, float aspectRatio = -1, Anchor anchor = Anchor.TopLeft)
            : this(texture, new Vector2(0, 0), new Vector2(texture.Width, texture.Height), aspectRatio, anchor) { }


        /// <summary>
        /// A blank 2D Object that can be rendered on screen.
        /// </summary>
        public Drawable() { }

        /// <summary>
        /// Resize this Drawable to fit the dimensions provided.
        /// </summary>
        /// <param name="size">A Vector2 that represents the new dimensions this Drawable should resize to. Width = size.X, Height = size.Y</param>
        /// <param name="resolutionScale">Whether the resize should consider the current resolution or not. Leave at default of true unless this resize does not need to consider the current resolution to display properly.</param>
        public virtual void Resize(Vector2 size, bool resolutionScale = true)
        {
            // Find the aspect ratio of the requested size change.
            float newAspect = size.X / size.Y;

            // If aspect ratio is not -1 and the new aspect ratio does not equal the aspect ratio of this Drawable, don't resize, and throw a console error.
            if (aspectRatio != -1 && newAspect != aspectRatio)
            {
                Fraction aspect = new Fraction(newAspect);
                Console.WriteLine("Invalid aspect ratio : " + size.X + "x" + size.Y + " isn't " + aspect.ToString());
                return;
            }

            // Set the scale of this Drawable using the parameters provided.
            scale = size.X / texture.Width *  (resolutionScale ? (Pulsarc.getDimensions().X / Pulsarc.xBaseRes) : 1);
        }

        /// <summary>
        /// Resize this Drawable to the square length provided.
        /// </summary>
        /// <param name="size">The square length this drawable should be reszied to. Width = size, Length = size.</param>
        /// <param name="resolutionScale">Whether the resize should consider the current resolution or not. Leave at default of true unless this resize does not need to consider the current resolution to display properly.</param>
        public virtual void Resize(float size, bool resolutionScale = true)
        {
            Resize(new Vector2(size, size), resolutionScale);
        }

        /// <summary>
        /// Set a new rotation for this Drawable.
        /// </summary>
        /// <param name="degrees">The angle of rotation in degrees.</param>
        public void setRotation(float degrees)
        {
            if(degrees>=-360 && degrees<=360)
            {
                rotation = (float) (degrees * (Math.PI / 180));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        static public Vector2 getResponsivePosition(Vector2 position)
        {
            return new Vector2(position.X / Pulsarc.xBaseRes * Pulsarc.getDimensions().X, position.Y / Pulsarc.yBaseRes * Pulsarc.getDimensions().Y);
        }

        /// <summary>
        /// Change the position of this drawable to the Vector2 coordinates provided.
        /// </summary>
        /// <param name="position">The coordinates of this Drawables new position. New positon = (position.X, position.Y)</param>
        public void changePosition(Vector2 position)
        {
            this.position = getResponsivePosition(position);

            Vector2 newPos = position;
            Vector2 size;
            if (texture != null)
            {
                size = new Vector2(texture.Width, texture.Height);
            } else
            {
                size = new Vector2(0,0);
            }

            switch (anchor)
            {
                case Anchor.Center:
                    newPos.X -= size.X / 2;
                    newPos.Y -= size.Y / 2;
                    break;
                case Anchor.TopRight:
                    newPos.X -= size.X;
                    break;
                case Anchor.CenterRight:
                    newPos.X -= size.X;
                    newPos.Y -= size.Y / 2;
                    break;
                case Anchor.BottomRight:
                    newPos.X -= size.X;
                    newPos.Y -= size.Y;
                    break;
                case Anchor.BottomLeft:
                    newPos.Y -= size.Y;
                    break;
                case Anchor.CenterLeft:
                    newPos.Y -= size.Y / 2;
                    break;
                case Anchor.TopLeft:
                default:
                    break;
            }

            drawPosition = getResponsivePosition(newPos);
        }

        /// <summary>
        /// Move this Drawable from its current coordinate + the coordinate provided by position. 
        /// </summary>
        /// <param name="position">How much this Drawable should move. New Position = (Drawable.position.X + position.X, Drawable.positionY + position.Y)</param>
        public virtual void move(Vector2 position)
        {
            this.position += getResponsivePosition(position);
            this.drawPosition += getResponsivePosition(position);
        }

        /// <summary>
        /// Returns whether or not this Drawable was clicked on.
        /// </summary>
        /// <param name="mousePos">The position of the cursor.</param>
        /// <returns>True if clicked, false if not clicked.</returns>
        public bool clicked(Vector2 mousePos)
        {
            return mousePos.X >= drawPosition.X && mousePos.X <= drawPosition.X + texture.Width && mousePos.Y >= drawPosition.Y && mousePos.Y <= drawPosition.Y + texture.Height;
        }

        /// <summary>
        /// Returns whether or not this Drawable was clicked on.
        /// </summary>
        /// <param name="mousePos">The position of the cursor.</param>
        /// <returns>True if clicked, false if not clicked.</returns>
        public bool clicked(Point mousePos)
        {
            return mousePos.X >= drawPosition.X && mousePos.X <= drawPosition.X + texture.Width && mousePos.Y >= drawPosition.Y && mousePos.Y <= drawPosition.Y + texture.Height;
        }

        /// <summary>
        /// Determine whether this Drawable is currently visible on screen.
        /// </summary>
        /// <returns>True if on screen, false if not on screen.</returns>
        public bool onScreen()
        {
            return new Rectangle((int)drawPosition.X, (int)drawPosition.Y, texture.Width, texture.Height).Intersects(new Rectangle(0, 0, (int)Pulsarc.getDimensions().X, (int)Pulsarc.getDimensions().Y));
        }

        /// <summary>
        /// Determine whether this Drawable is intersecting with the provided Drawable.
        /// </summary>
        /// <param name="drawable">The Drawable that may be or may not be intersecting with this Drawalbe.</param>
        /// <returns>True if the two Drawables intersect, false if they dont'.</returns>
        public bool intersects(Drawable drawable)
        {
            return new Rectangle((int)drawPosition.X, (int)drawPosition.Y, texture.Width, texture.Height)
                .Intersects(
                    new Rectangle((int) drawable.drawPosition.X, (int)drawable.drawPosition.Y, drawable.texture.Width, drawable.texture.Height));
        }

        /// <summary>
        /// Draw this Drawable.
        /// </summary>
        public virtual void Draw()
        {
            Pulsarc.spriteBatch.Draw(texture, drawPosition, drawnPart, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);

            if(hover != null && clicked(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)))
            {
                hover.Draw();
            }
        }
    }
}
