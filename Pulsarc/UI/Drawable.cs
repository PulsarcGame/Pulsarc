using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Skinning;
using Pulsarc.Utils;
using Pulsarc.Utils.Maths;
using System;
using Wobble.Logging;

namespace Pulsarc.UI
{
    /// <summary>
    /// An enum that represents the 9 different "Anchors" that a Drawable utilizes
    /// For positioning and rotation.
    /// </summary>
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
                    texture = Skin.DefaultTexture;

                // Update currentSize and drawnPart
                currentSize = new Vector2(texture.Width * Scale, texture.Height * Scale);
                drawnPart = new Rectangle(new Point(0, 0), new Point(texture.Width, texture.Height));

                // Update position to refresh anchorPosition
                ChangePosition(TruePosition, true);
            }
        }

        // If the cursor is hovering this Drawable, it will be changed to the "hover" Drawable.
        private Drawable hover = null;

        // Makes sure that important variables are correctly updated when Hover is set.
        public virtual Drawable Hover
        {
            get => hover;

            protected set
            {
                hover = value;
                hover.ChangePosition(TruePosition, true);
                hover.Scale = Scale;
                hover.HeightScaled = HeightScaled;
            }
        }

        // Whether or not this Drawable has a Hover object
        public virtual bool HasHover { get => Hover != null; }

        // The true position of this Drawable, as if it was anchored to Anchor.TopLeft.
        public Vector2 TruePosition;

        // The position of this Drawable relative to its anchor.
        public Vector2 Position;

        // The origin (center) of this Drawable.
        public Vector2 origin;

        // What part of the Drawable is drawn, determined by this Rectangle object.
        public Rectangle drawnPart;

        // The Anchor of the drawable, determines how this Drawable will be rendered in relation
        // to its position.
        public Anchor Anchor { get; protected set; }

        // The current size of this drawable in pixels.
        public Vector2 currentSize;

        // The opacity of this Drawable, 1 is fully opaque, 0 is fully transparent.
        public float Opacity { get; set; } = 1f;

        // Used to force aspect ratio when using Resize.
        protected float AspectRatio { get; set; }

        // How big this Drawable is compared to its base texture dimensions.
        public float Scale { get; protected set; } = 1;

        // Whether this Drawable's size scales with the Height or the Width of the game
        // True means this drawable will scale with the game's height
        // False means this drawable will scale with the game's width
        public virtual bool HeightScaled { get; protected set; }

        // The angle of this Drawable.
        public float Rotation { get; protected set; } = 0;


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
        public Drawable(Texture2D texture, Vector2 position, Vector2 size, float aspectRatio = -1,
            Anchor anchor = Anchor.TopLeft, bool heightScaled = true)
        {
            // Define variables
            origin = Vector2.Zero;
            Texture = texture;
            AspectRatio = aspectRatio;
            Anchor = anchor;
            HeightScaled = heightScaled;

            Resize(size * Pulsarc.HeightScale);
            ChangePosition(position);
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
        public Drawable(Texture2D texture, Vector2 position, float aspectRatio = -1,
            Anchor anchor = Anchor.TopLeft, bool heightScaled = true)
        : this(texture, position, new Vector2(texture.Width, texture.Height),
            aspectRatio, anchor, heightScaled)
        { }


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
        public Drawable(Texture2D texture, float aspectRatio = -1, Anchor anchor = Anchor.TopLeft,
            bool heightScaled = true)
        : this(texture, new Vector2(0, 0), new Vector2(texture.Width, texture.Height),
            aspectRatio, anchor, heightScaled)
        { }


        /// <summary>
        /// A blank 2D Object that can be rendered on screen.
        /// </summary>
        public Drawable()
        { }

        #region Scale and Movement Methods
        /// <summary>
        /// Resize this Drawable to fit the dimensions provided.
        /// If Height scaled, it will Resize Height over Width.
        /// Otherwise it will Resize Width over Height
        /// </summary>
        /// <param name="size">A Vector2 that represents the new dimensions
        /// this Drawable should resize to. Width = size.X, Height = size.Y</param>
        public virtual void Resize(Vector2 size)
        {
            if (HeightScaled)
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
        public virtual void Resize(float size) => Resize(new Vector2(size, size));

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
            if (AspectRatio != -1 && newAspect != AspectRatio)
            {
                Fraction aspect = new Fraction(newAspect);
                PulsarcLogger.Debug($"Invalid aspect ratio : {currentSize.X}x{currentSize.Y} isn't {aspect.ToString()}", LogType.Runtime);

                currentSize = oldSize;
                return;
            }
            
            currentSize = size;

            // Set the scale of this Drawable using the parameters provided.
            Scale = currentSize.Y / Texture.Height;

            // Fix currentSize.X
            currentSize.X = Texture.Width * Scale;
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
            if (AspectRatio != -1 && newAspect != AspectRatio)
            {
                Fraction aspect = new Fraction(newAspect);
                PulsarcLogger.Debug($"Invalid aspect ratio : {currentSize.X}x{currentSize.Y} isn't {aspect.ToString()}", LogType.Runtime);

                currentSize = oldSize;
                return;
            }

            currentSize = size;

            // Set the scale of this Drawable using the parameters provided.
            Scale = currentSize.X / Texture.Width;

            // Fix currentSize.Y
            currentSize.Y = Texture.Height * Scale;
        }

        /// <summary>
        /// Set a new rotation for this Drawable.
        /// </summary>
        /// <param name="degrees">The angle of rotation in degrees.</param>
        public void SetRotation(float degrees)
        {
            bool inRange = degrees >= -360 && degrees <= 360;
            float adjustedDegrees = degrees;

            if (inRange)
                Rotation = (float)(degrees * (Math.PI / 180));
            // If rotation is greater than 360 or less than -360, keep adding/subtracting
            // 360 until we get a value within the desired range, then use that for rotation.
            else
            {
                while (!inRange)
                {
                    if (adjustedDegrees < -360)
                    {
                        adjustedDegrees += 360;
                    }
                    else
                    {
                        adjustedDegrees -= 360;
                    }

                    inRange = adjustedDegrees >= -360 && adjustedDegrees <= 360;
                }

                Rotation = (float)(adjustedDegrees * (Math.PI / 180));
            }
        }

        /// <summary>
        /// Change the position of this drawable to the coordinates provided.
        /// </summary>
        /// <param name="position">The coordinates of this Drawables new position.
        /// New positon = (position.X, position.Y)</param>
        /// <param name="topLeftPositioning">Whether this should consider the Anchor
        /// when positioning, or just the raw position provided. If true the
        /// positioning acts as if the Anchor was TopLeft ("raw" position).</param>
        public virtual void ChangePosition(Vector2 position, bool topLeftPositioning = false)
        {
            if (topLeftPositioning)
            {
                TruePosition = position;
                FindAnchorPosition();
                return;
            }

            switch (Anchor)
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

            TruePosition = position;
            FindAnchorPosition();
        }

        /// <summary>
        /// Change the position of this drawable to the coordinates provided.
        /// </summary>
        /// <param name="x">The X coordinate of this Drawables new position.</param>
        /// <param name="y">The Y coordinate of this Drawables new position.</param>
        /// <param name="topLeftPositioning">Whether this should consider the Anchor
        /// when positioning, or just the raw position provided. If true the
        /// positioning acts as if the Anchor was TopLeft.</param>
        public void ChangePosition(float x, float y, bool topLeftPositioning = false)
        {
            ChangePosition(new Vector2(x, y), topLeftPositioning);
        }

        /// <summary>
        /// Move this Drawable from its current coordinate by the amount provided,
        /// movement is scaled by the current game resolution for both axis.
        /// </summary>
        /// <param name="delta">A Vector2 representing the movement this Drawable
        /// should take in the X and Y axises</param>
        /// <param name="heightScaled">True if the movement is scaled by the game height,
        /// false if the movement is scaled by the game width,
        /// (default) null to use use this Drawable's HeightScaled member to decide.</param>
        public virtual void Move(Vector2 delta, bool? heightScaled = null)
        {
            // If Hover exists, move it
            Hover?.Move(delta, heightScaled);

            // Find out what scale we're using, height or width
            bool useHeightScale = heightScaled ?? HeightScaled;

            // If height scaled, scale movement by height scale, otherwise by width scale
            float scale = useHeightScale ? Pulsarc.HeightScale : Pulsarc.WidthScale;

            TruePosition.X += delta.X * scale;
            TruePosition.Y += delta.Y * scale;

            FindAnchorPosition();
        }

        /// <summary>
        /// Move this Drawable from its current coordinate by the amount provided,
        /// movement is scaled by the current game resolution for both axis.
        /// </summary>
        /// <param name="xDelta">The amount of movement this Drawable should take in
        /// the X axis</param>
        /// <param name="yDelta">The amount of movement this Drawable should take in
        /// the Y axis</param>
        /// <param name="heightScaled">True if height scaled movement, false if width scaled,
        /// null to use this Drawable's HeightScaled member.</param>
        public virtual void Move(float xDelta, float yDelta, bool? heightScaled = null)
        {
            Move(new Vector2(xDelta, yDelta), heightScaled);
        }

        /// <summary>
        /// Find the correct position for this Drawable based
        /// on its anchor and adjust accordingly. Call after changing the Drawable's
        /// position.
        /// </summary>
        protected virtual void FindAnchorPosition()
        {
            Vector2 position = TruePosition;

            switch (Anchor)
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

            Position = position;
        }

        /// <summary>
        /// Move this Drawable from its current coordinate by the amount provided. Movement
        /// is scaled in one axis.
        /// TODO: Change Move() to scale in both axis for consistency (i.e, act like ScaledMove()
        /// This will require a rewrite of everything that uses Move().
        /// </summary>
        /// <param name="position">How much this Drawable should move.</param>
        /// <param name="followScale">Whether or not this Drawable should move according
        /// to the current Height/Width scaling. Default is true.</param>
        [Obsolete("This was the old move method for Drawables, it is now deprecated, use Move()")]
        public virtual void OldMove(Vector2 position, bool followScale = true)
        {
            // If Hover exists, move it.
            Hover?.OldMove(position, followScale);

            if (!followScale)
            {
                TruePosition += position;
                FindAnchorPosition();
                return;
            }

            // If not height scaled, Y movement is normal and X movement is scaled
            // If height scaled, X movement is normal and Y movement is scaled
            TruePosition.X += position.X * (!HeightScaled ? Pulsarc.WidthScale : 1);
            TruePosition.Y += position.Y * (HeightScaled ? Pulsarc.HeightScale : 1);

            FindAnchorPosition();
        }

        /// <summary>
        /// Move this Drawable from its current coordinate by the amount provided. Movement
        /// is scaled in one axis.
        /// TODO: Change Move() to scale in both axis for consistency (i.e, act like ScaledMove()
        /// This will require a rewrite of everything that uses Move().
        /// </summary>
        /// <param name="xDelta">How much this Drawable should move on the X coordinate.</param>
        /// <param name="yDelta">How much this Drawable should move on the Y coordinate.</param>
        /// <param name="followScale">Whether or not this Drawable should move according
        /// to the current Height/Width scaling. Default is true.</param>
        [Obsolete("This was the old move method for Drawables, it is now deprecated, use Move()")]
        public virtual void OldMove(float xDelta, float yDelta, bool followScale = true)
        {
            OldMove(new Vector2(xDelta, yDelta), followScale);
        }
        #endregion

        #region Mouse State Methods
        /// <summary>
        /// Returns whether or not this Drawable was clicked on.
        /// </summary>
        /// <param name="mousePos">The position of the cursor.</param>
        /// <returns>True if clicked, false if not clicked.</returns>
        public bool Hovered(Vector2 mousePos)
            =>  mousePos.X >= TruePosition.X 
                && mousePos.X <= TruePosition.X + (drawnPart.Width * Scale)
                && mousePos.Y >= TruePosition.Y
                && mousePos.Y <= TruePosition.Y + (drawnPart.Height * Scale);

        /// <summary>
        /// Returns whether or not this Drawable was clicked on.
        /// </summary>
        /// <param name="mousePos">The position of the cursor.</param>
        /// <returns>True if clicked, false if not clicked.</returns>
        public bool Hovered(Point mousePos) => Hovered(mousePos.X, mousePos.Y);

        /// <summary>
        /// Returns whether or not this Drawable was clicked on.
        /// </summary>
        /// <param name="mouseX">The X position of the mouse.</param>
        /// <param name="mouseY">The Y position of the mouse.</param>
        /// <returns>True if clicked, false if not clicked.</returns>
        public bool Hovered(float mouseX, float mouseY) => Hovered(new Vector2(mouseX, mouseY));

        public bool Hovered(MouseState mouseState) => Hovered(mouseState.X, mouseState.Y);

        public bool Pressed(MouseState mouseState)
            => mouseState.LeftButton == ButtonState.Pressed && Hovered(mouseState);

        public bool Released(MouseState mouseState)
            => mouseState.LeftButton == ButtonState.Released && Hovered(mouseState);

        public bool Clicked(MouseState firstState, MouseState secondState)
            => Pressed(firstState) && Released(secondState);
        #endregion

        #region Checks
        /// <summary>
        /// Determine whether this Drawable is currently visible on screen.
        /// </summary>
        /// <returns>True if on screen, false if not on screen.</returns>
        public bool OnScreen() =>   new Rectangle(
                                        (int)TruePosition.X, (int)TruePosition.Y,
                                        (int)currentSize.X, (int)currentSize.Y)
                                    .Intersects(Pulsarc.ScreenSpace);

        /// <summary>
        /// Determine whether this Drawable is intersecting with the provided Drawable.
        /// </summary>
        /// <param name="drawable">The Drawable that may be or may not be intersecting with this Drawalbe.</param>
        /// <returns>True if the two Drawables intersect, false if they dont'.</returns>
        public bool Intersects(Drawable drawable)
        {
            return new Rectangle(
                (int)TruePosition.X, (int)TruePosition.Y,
                (int)currentSize.X, (int)currentSize.Y)
            .Intersects(new Rectangle(
                (int)drawable.TruePosition.X, (int)drawable.TruePosition.Y,
                (int)drawable.currentSize.X, (int)drawable.currentSize.Y));
        }
        #endregion

        /// <summary>
        /// Draw this Drawable.
        /// </summary>
        public virtual void Draw()
        {
            // If opacity isn't 1, color is just Color.White, otherwise it's Color.White * the opacity.
            Color color = Opacity != 1 ? Color.White * Opacity : Color.White;

            Pulsarc.SpriteBatch.Draw(Texture, TruePosition, drawnPart, color, Rotation, origin, Scale, SpriteEffects.None, 0f);

            // If this item is hovered by the mouse, display the hover drawable.
            if (Hover != null && Hovered(Mouse.GetState()))
            {
                Hover.Draw();
            }
        }
    }
}

