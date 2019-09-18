using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Skinning;
using Pulsarc.Utils.Maths;
using System;
using Wobble.Logging;

namespace Pulsarc.UI
{
    public enum Anchor
    {
        Center = 0,
        TopRight = 1,
        CenterRight = 2,
        BottomRight = 3,
        TopLeft = 4,
        CenterLeft = 5,
        BottomLeft = 6,
        CenterTop = 7,
        CenterBottom = 8,
    }

    public class Drawable
    {
        // The texture for this Drawable.
        private Texture2D texture;

        // Makes sure that important variables are correctly updated when Texture is set.
        public virtual Texture2D Texture
        {
            get => texture;

            set
            {
                texture = value;

                // Change to default texture if value was null
                if (texture == null)
                {
                    texture = Skin.DefaultTexture;
                }

                // Update currentSize and drawnPart
                currentSize = new Vector2(texture.Width * scale, texture.Height * scale);
                drawnPart = new Rectangle(new Point(0, 0), new Point(texture.Width, texture.Height));

                // Update position to refresh anchorPosition
                changePosition(truePosition, true);
            }
        }

        // If the cursor is hovering this Drawable, it will be changed to the "hover" Drawable.
        private Drawable hover = null;
        public virtual Drawable Hover { get; protected set; }

        public virtual bool HoverObject { get => Hover != null; }

        // The true position of this Drawable, as if it was anchored to Anchor.TopLeft.
        public Vector2 truePosition;

        // The position of this Drawable relative to its anchor.
        public Vector2 anchorPosition;

        // The origin (center) of this Drawable.
        public Vector2 origin;

        // What part of the Drawable is drawn, determined by this Rectangle object.
        public Rectangle drawnPart;

        // The Anchor of the drawable, determines how this Drawable will be rendered in relation to its position.
        public Anchor anchor;

        // The current size of this drawable in pixels.
        public Vector2 currentSize;

        // The opacity of this Drawable, 1 is fully opaque, 0 is fully transparent.
        public float opacity = 1f;

        // Used to force aspect ratio when using Resize.
        protected float aspectRatio;

        // How big this Drawable is compared to its base texture dimensions.
        public float scale = 1;

        // Whether this Drawable's size scales with the Height or the Width of the game
        // True means this drawable will scale with the game's height
        // False means this drawable will scale with the game's width
        private bool heightScaled = true;
        public virtual bool HeightScaled
        {
            get => heightScaled;
            protected set
            {
                heightScaled = value;
            }
        }

        // The angle of this Drawable.
        protected float rotation = 0;


        /// <summary>
        /// A 2D object that can be rendered on screen.
        /// </summary>
        /// <param name="texture">The texture for this Drawable.</param>
        /// <param name="position">The position of this Drawable, relative
        /// to the base dimensions (1920 x 1080)</param>
        /// <param name="size">How large this object is (in pixels).</param>
        /// <param name="aspectRatio">The aspect ratio of this Drawable.
        /// Leave at default of -1 unless this Drawable needs to be displayed
        /// with a consistent aspect ratio.</param>
        /// <param name="anchor">The anchor of this Drawable, the default is
        /// TopLeft, which means that the Drawable will be drawn below and to
        /// the right of the position.</param>
        public Drawable(Texture2D texture, Vector2 position, Vector2 size, float aspectRatio = -1, Anchor anchor = Anchor.TopLeft, bool heightScaled = true)
        {
            // Define variables
            origin = new Vector2(0, 0);
            Texture = texture;
            this.aspectRatio = aspectRatio;
            this.anchor = anchor;
            this.heightScaled = heightScaled;

            Resize(size * Pulsarc.HeightScale);
            changePosition(position);
        }

        /// <summary>
        /// A 2D object that can be rendered on screen, using the size of the
        /// texture provided.
        /// </summary>
        /// <param name="texture">The texture for this Drawable. The dimensions
        /// determine how large this object will be.</param>
        /// <param name="position">The position of this Drawable, relative to
        /// the base dimensions (1920 x 1080)</param>
        /// <param name="aspectRatio">The aspect ratio of this Drawable.
        /// Leave at default of -1 unless this Drawable needs to be displayed
        /// with a consistent aspect ratio.</param>
        /// <param name="anchor">The anchor of this Drawable, the default is
        /// TopLeft, which means that the Drawable will be drawn below and to
        /// the right of the position.</param>
        public Drawable(Texture2D texture, Vector2 position, float aspectRatio = -1, Anchor anchor = Anchor.TopLeft, bool heightScaled = true)
            : this(texture, position, new Vector2(texture.Width, texture.Height), aspectRatio, anchor, heightScaled) { }


        /// <summary>
        /// A 2D object that can be rendered on screen, using the size of the
        /// texture provided, and with a starting location of (0, 0).
        /// </summary>
        /// <param name="texture">The texture for this Drawable. The dimensions
        /// determine how large this object will be.</param>
        /// <param name="aspectRatio">The aspect ratio of this Drawable.
        /// Leave at default of -1 unless this Drawable needs to be displayed
        /// with a consistent aspect ratio.</param>
        /// <param name="anchor">The anchor of this Drawable, the default is
        /// TopLeft, which means that the Drawable will be drawn below and to
        /// the right of the position.</param>
        public Drawable(Texture2D texture, float aspectRatio = -1, Anchor anchor = Anchor.TopLeft, bool heightScaled = true)
            : this(texture, new Vector2(0, 0), new Vector2(texture.Width, texture.Height), aspectRatio, anchor, heightScaled) { }


        /// <summary>
        /// A blank 2D Object that can be rendered on screen.
        /// </summary>
        public Drawable() { }

        /// <summary>
        /// Resize this Drawable to fit the dimensions provided.
        /// If Height scaled, it will Resize Height over Width.
        /// Otherwise it will Resize Width over Height
        /// </summary>
        /// <param name="size">A Vector2 that represents the new dimensions
        /// this Drawable should resize to. Width = size.X, Height = size.Y</param>
        public virtual void Resize(Vector2 size)
        {
            if (heightScaled)
            {
                ResizeHeight(size);
            }
            else
            {
                ResizeWidth(size);
            }
        }

        /// <summary>
        /// Resize this Drawable to the square length provided.
        /// This resizses the height (or width) to the square length provided,
        /// the opposite length will scale itself accordingly.
        /// </summary>
        /// <param name="size">The square length this drawable should be reszied to.
        /// Height = size, Width = Height.</param>
        public virtual void Resize(float size)
        {
            Resize(new Vector2(size,size));
        }

        /// <summary>
        /// Resize this Drawable to fit the dimensions provided, focusing on height
        /// over width
        /// </summary>
        /// <param name="size">A Vector2 that represents the new dimensions
        /// this Drawable should resize to. Width = size.X, Height = size.Y</param>
        protected virtual void ResizeHeight(Vector2 size)
        {
            Vector2 oldSize = currentSize;

            // Find the aspect ratio of the requested size change.
            currentSize = size;
            float newAspect = currentSize.X / currentSize.Y;

            // If aspect ratio is not -1 and the new aspect ratio does not equal the aspect ratio of this Drawable, don't resize, and throw a console error.
            if (aspectRatio != -1 && newAspect != aspectRatio)
            {
                Fraction aspect = new Fraction(newAspect);
                Logger.Debug("Invalid aspect ratio : " + currentSize.X + "x" + currentSize.Y + " isn't " + aspect.ToString(), LogType.Runtime);

                currentSize = oldSize;
                return;
            }
            
            currentSize = size;

            // Set the scale of this Drawable using the parameters provided.
            scale = currentSize.Y / Texture.Height;

            // Fix currentSize.X
            currentSize.X = Texture.Width * scale;
        }

        /// <summary>
        /// Resize this Drawable to fit the dimensions provided, focusing on width
        /// over height
        /// </summary>
        /// <param name="size">A Vector2 that represents the new dimensions
        /// this Drawable should resize to. Width = size.X, Height = size.Y</param>
        public virtual void ResizeWidth(Vector2 size)
        {
            Vector2 oldSize = currentSize;

            // Find the aspect ratio of the requested size change.
            currentSize = size;
            float newAspect = currentSize.X / currentSize.Y;

            // If aspect ratio is not -1 and the new aspect ratio does not equal the aspect ratio of this Drawable, don't resize, and throw a console error.
            if (aspectRatio != -1 && newAspect != aspectRatio)
            {
                Fraction aspect = new Fraction(newAspect);
                Logger.Debug("Invalid aspect ratio : " + currentSize.X + "x" + currentSize.Y + " isn't " + aspect.ToString(), LogType.Runtime);

                currentSize = oldSize;
                return;
            }

            currentSize = size;

            // Set the scale of this Drawable using the parameters provided.
            scale = currentSize.X / Texture.Width;

            // Fix currentSize.Y
            currentSize.Y = Texture.Height * scale;
        }

        /// <summary>
        /// Set a new rotation for this Drawable.
        /// </summary>
        /// <param name="degrees">The angle of rotation in degrees.</param>
        public void setRotation(float degrees)
        {
            if (degrees >= -360 && degrees <= 360)
            {
                rotation = (float)(degrees * (Math.PI / 180));
            }
        }

        /// <summary>
        /// Change the position of this drawable to the coordinates provided.
        /// </summary>
        /// <param name="position">The coordinates of this Drawables new position.
        /// New positon = (position.X, position.Y)</param>
        /// <param name="truePositioning">Whether this should consider the Anchor
        /// when positioning, or just the raw position provided. If true the
        /// positioning acts as if the Anchor was TopLeft.</param>
        public void changePosition(Vector2 position, bool truePositioning = false)
        {
            if (truePositioning)
            {
                truePosition = position;
                findAnchorPosition();
                return;
            }

            Vector2 newPos = position;

            switch (anchor)
            {
                case Anchor.Center:
                    position.X -= currentSize.X / 2;
                    position.Y -= currentSize.Y / 2;
                    break;
                case Anchor.TopRight:
                    position.X -= currentSize.X;
                    break;
                case Anchor.CenterRight:
                    position.X -= currentSize.X;
                    position.Y -= currentSize.Y / 2;
                    break;
                case Anchor.BottomRight:
                    position.X -= currentSize.X;
                    position.Y -= currentSize.Y;
                    break;
                case Anchor.BottomLeft:
                    position.Y -= currentSize.Y;
                    break;
                case Anchor.CenterLeft:
                    position.Y -= currentSize.Y / 2;
                    break;
                case Anchor.CenterTop:
                    position.X -= currentSize.X / 2;
                    break;
                case Anchor.CenterBottom:
                    position.X -= currentSize.X / 2;
                    position.Y -= currentSize.Y;
                    break;
                case Anchor.TopLeft:
                default:
                    break;
            }

            truePosition = position;
            findAnchorPosition();
        }

        /// <summary>
        /// Change the position of this drawable to the coordinates provided.
        /// </summary>
        /// <param name="x">The X coordinate of this Drawables new position.</param>
        /// <param name="y">The Y coordinate of this Drawables new position.</param>
        public void changePosition(float x, float y)
        {
            changePosition(new Vector2(x, y));
        }

        /// <summary>
        /// Move this Drawable from its current coordinate by the amount provided.
        /// </summary>
        /// <param name="position">How much this Drawable should move.</param>
        /// <param name="scaledPositioning">Whether or not this Drawable should move according
        /// to the Height/Width scaling.</param>
        public virtual void move(Vector2 position, bool scaledPositioning = true)
        {
            Hover?.move(position, scaledPositioning);

            if (!scaledPositioning)
            {
                truePosition += position;
                findAnchorPosition();
                return;
            }

            // If not height scaled, Y movement is normal and X movement is scaled
            // If height scaled, X movement is normal and Y movement is scaled
            truePosition.X += position.X * (!heightScaled ? Pulsarc.WidthScale : 1);
            truePosition.Y += position.Y * (heightScaled ? Pulsarc.HeightScale : 1);

            findAnchorPosition();
        }

        /// <summary>
        /// Move this Drawable from its current coordinate by the amount provided.
        /// </summary>
        /// <param name="xDelta">How much this Drawable should move on the X coordinate.</param>
        /// <param name="yDelta">How much this Drawable should move on the Y coordinate.</param>
        /// <param name="scaledPositioning">Whether or not this Drawable should move according
        /// to the Height/Width scaling.</param>
        public virtual void move(float xDelta, float yDelta, bool scaledPositioning = true)
        {
            move(new Vector2(xDelta, yDelta), scaledPositioning);
        }

        public virtual void scaledMove(Vector2 position)
        {
            truePosition.X += position.X * Pulsarc.WidthScale;
            truePosition.Y += position.Y * Pulsarc.HeightScale;

            findAnchorPosition();
        }

        public virtual void scaledMove(float xDelta, float yDelta)
        {
            scaledMove(new Vector2(xDelta, yDelta));
        }

        /// <summary>
        /// Returns whether or not this Drawable was clicked on.
        /// </summary>
        /// <param name="mousePos">The position of the cursor.</param>
        /// <returns>True if clicked, false if not clicked.</returns>
        public bool clicked(Vector2 mousePos)
        {
            return mousePos.X >= truePosition.X && mousePos.X <= truePosition.X + (drawnPart.Width * scale) && mousePos.Y >= truePosition.Y && mousePos.Y <= truePosition.Y + (drawnPart.Height * scale);
        }

        /// <summary>
        /// Returns whether or not this Drawable was clicked on.
        /// </summary>
        /// <param name="mousePos">The position of the cursor.</param>
        /// <returns>True if clicked, false if not clicked.</returns>
        public bool clicked(Point mousePos)
        {
            return mousePos.X >= truePosition.X && mousePos.X <= truePosition.X + (drawnPart.Width * scale) && mousePos.Y >= truePosition.Y && mousePos.Y <= truePosition.Y + (drawnPart.Height * scale);
        }

        /// <summary>
        /// Determine whether this Drawable is currently visible on screen.
        /// </summary>
        /// <returns>True if on screen, false if not on screen.</returns>
        public bool onScreen()
        {
            return new Rectangle((int)truePosition.X, (int)truePosition.Y, Texture.Width, Texture.Height).Intersects(new Rectangle(0, 0, (int)Pulsarc.getDimensions().X, (int)Pulsarc.getDimensions().Y));
        }

        /// <summary>
        /// Determine whether this Drawable is intersecting with the provided Drawable.
        /// </summary>
        /// <param name="drawable">The Drawable that may be or may not be intersecting with this Drawalbe.</param>
        /// <returns>True if the two Drawables intersect, false if they dont'.</returns>
        public bool intersects(Drawable drawable)
        {
            return new Rectangle((int)truePosition.X, (int)truePosition.Y, Texture.Width, Texture.Height)
                .Intersects(
                    new Rectangle((int) drawable.truePosition.X, (int)drawable.truePosition.Y, drawable.Texture.Width, drawable.Texture.Height));
        }

        /// <summary>
        /// Draw this Drawable.
        /// </summary>
        public virtual void Draw()
        {
            // If opacity isn't 1, color is just Color.White, otherwise it's Color.White * the opacity.
            Color color = opacity != 1 ? Color.White * opacity : Color.White;

            Pulsarc.spriteBatch.Draw(Texture, truePosition, drawnPart, color, rotation, origin, scale, SpriteEffects.None, 0f);

            if (Hover != null && clicked(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)))
            {
                Hover.Draw();
            }
        }

        private void findAnchorPosition()
        {
            Vector2 position = truePosition;

            switch (anchor)
            {
                case Anchor.Center:
                    position.X += currentSize.X / 2;
                    position.Y += currentSize.Y / 2;
                    break;
                case Anchor.TopRight:
                    position.X += currentSize.X;
                    break;
                case Anchor.CenterRight:
                    position.X += currentSize.X;
                    position.Y += currentSize.Y / 2;
                    break;
                case Anchor.BottomRight:
                    position.X += currentSize.X;
                    position.Y += currentSize.Y;
                    break;
                case Anchor.BottomLeft:
                    position.Y += currentSize.Y;
                    break;
                case Anchor.CenterLeft:
                    position.Y += currentSize.Y / 2;
                    break;
                case Anchor.CenterTop:
                    position.X += currentSize.X / 2;
                    break;
                case Anchor.CenterBottom:
                    position.X += currentSize.X / 2;
                    position.Y += currentSize.Y;
                    break;
                case Anchor.TopLeft:
                default:
                    break;
            }

            anchorPosition = position;
        }
    }
}

