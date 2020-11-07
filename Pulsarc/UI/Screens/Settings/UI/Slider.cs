using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Pulsarc.Utils;
using System;
using Wobble.Logging;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class Slider : Setting
    {
        // The Selector "Knob" for this slider.
        public SliderSelector Selector { get; protected set; } = new SliderSelector();

        // Min/Max Slider values
        public int MinValue { get; protected set; }
        public int MaxValue { get; protected set; }
        // Minimum value but blocking the slider instead of changing the represented percentage
        public int HiddenMin { get; protected set; }
        
        // How much each pixel of selector movement changes the value.
        public int Step { get; protected set; }

        //
        public int DisplayDivider { get; protected set; }
        public int DisplayPrecision { get; protected set; }

        // How far away (in pixels) from the edges the SliderSelector needs to stop at.
        public int EdgeOffset { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startingValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        public Slider(string title, string more, Vector2 position, string type, int startingValue = 50, int minValue = 0, int maxValue = 100, int step = 1, int displayDivider = 1, int displayPrecision = 2, int edgeOffset = 15, int hiddenmin = 0)
            : base(title, more, position, Skin.Assets["slider"], Anchor.CenterLeft, startingValue, type)
        {
            Selector.Resize(50);

            Value = startingValue;
            Step = step;
            DisplayDivider = displayDivider;
            DisplayPrecision = displayPrecision;
            MinValue = minValue;
            MaxValue = maxValue;
            HiddenMin = hiddenmin;
            EdgeOffset = edgeOffset;

            Hold = true;
            SetSelector();
            Update();
        }


        /// <summary>
        /// Moves the selector to the provided value, and changes the current value of this slider to that value.
        /// </summary>
        /// <param name="value">The value to move the selector to.</param>
        public void SetSelector(int value)
        {
            int stepdist = Value - value;

            if (Math.Abs(stepdist) >= Step/2)
            {
                Value -= Step * (stepdist / Step);
                Update();
            }

            float percentagePosition = FindSelectorPosition();

            int rangeMin = EdgeOffset;
            int rangeMax = (int)currentSize.X - EdgeOffset;

            int selectorRange = rangeMax - rangeMin;

            int position = (int)(percentagePosition * selectorRange);

            Selector.ChangePosition(new Vector2(Position.X + EdgeOffset + position, Position.Y));
        }

        /// <summary>
        /// Update this slider.
        /// </summary>
        public void Update()
        {
            Title.RestartThenAppend(" : " + Math.Round(Value / (float)DisplayDivider, DisplayPrecision).ToString());
        }

        public void SetSelectorPercent(float percent)
        {
            SetSelector((int) (MinValue + (MaxValue - MinValue) * percent));
        }

        /// <summary>
        /// Moves the selector to the current value
        /// </summary>
        public void SetSelector()
        {
            SetSelector(Value);
        }

        /// <summary>
        /// Finds the position of the Selector for this Slider. (0-1f relative to the size of the Slider)
        /// </summary>
        /// <param name="value">The value to find the position for.</param>
        /// <returns>A float between 0f and 1f that represents the position the Selector should be in relative to the Slider.</returns>
        public float FindSelectorPosition(int value)
        {
            int range = MaxValue - MinValue;

            float position = (value - MinValue) / (float)range;

            // If position is greater than 1, return 1, else if its lower than 0, return 0
            // If neither of those, return position
            return position > 1f ? 1f : position < 0f ? 0f : position;
        }

        /// <summary>
        /// Finds the position of the Selector for this Slider (0f-1f relative to the size of the Slider)
        /// This instance of the method uses the currentValue of this slider.
        /// </summary>
        /// <returns>A float between 0f and 1f that represents the position the Selector should be in relation to the size of the Slider.</returns>
        public float FindSelectorPosition()
        {
            return FindSelectorPosition(Value);
        }

        public override dynamic GetSaveValue()
        {
            return Value / (float)DisplayDivider;
        }

        public override void SetSaveValue(dynamic value)
        {
            Value = value * DisplayDivider;
        }

        public override void Move(Vector2 delta, bool? heightScaled = null)
        {
            base.Move(delta, heightScaled);
            Selector.Move(delta, heightScaled);
        }

        public override void Draw()
        {
            base.Draw();
            Selector.Draw();
        }

        public override void OnClick(Vector2 mousePosition)
        {
            SetSelectorPercent((mousePosition.X - TruePosition.X) / ((Texture.Width) * Scale));
        }
    }
}
