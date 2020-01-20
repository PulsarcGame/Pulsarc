using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Pulsarc.Utils;
using Wobble.Logging;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class Slider : Setting
    {
        // The Selector "Knob" for this slider.
        private SliderSelector Selector { get; set; } = new SliderSelector();

        // Min/Max Slider values
        private int MinValue { get; set; }
        private int MaxValue { get; set; }
        
        // How much each pixel of selector movement changes the value.
        private int Step { get; set; }

        //
        private int DisplayDivider { get; set; }
        private int DisplayPrecision { get; set; }

        // How far away (in pixels) from the edges the SliderSelector needs to stop at.
        private int EdgeOffset { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="startingValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="title"></param>
        /// <param name="more"></param>
        /// <param name="position"></param>
        /// <param name="step"></param>
        /// <param name="displayDivider"></param>
        /// <param name="displayPrecision"></param>
        /// <param name="edgeOffset"></param>
        public Slider(string title, string more, Vector2 position, string type, int startingValue = 50, int minValue = 0, int maxValue = 100, int step = 1, int displayDivider = 1, int displayPrecision = 2, int edgeOffset = 15)
            : base(title, position, Skin.Assets["slider"], Anchor.CenterLeft, startingValue, type)
        {
            Selector.Resize(50);

            Value = startingValue;
            Step = step;
            DisplayDivider = displayDivider;
            DisplayPrecision = displayPrecision;
            MinValue = minValue;
            MaxValue = maxValue;
            EdgeOffset = edgeOffset;

            Hold = true;
            SetSelector();
            Update();
        }


        /// <summary>
        /// Moves the selector to the provided value, and changes the current value of this slider to that value.
        /// </summary>
        /// <param name="value">The value to move the selector to.</param>
        private void SetSelector(int value)
        {
            int stepdist = Value - value;

            if (Math.Abs(stepdist) >= Step/2)
            {
                Value -= Step * (stepdist / Step);
                Update();
            }

            float percentagePosition = FindSelectorPosition();

            int rangeMin = EdgeOffset;
            int rangeMax = (int)CurrentSize.X - EdgeOffset;

            int selectorRange = rangeMax - rangeMin;

            int position = (int)(percentagePosition * selectorRange);

            Selector.ChangePosition(new Vector2(AnchorPosition.X + EdgeOffset + position, AnchorPosition.Y));
        }

        /// <summary>
        /// Update this slider.
        /// </summary>
        private void Update()
        {
            Title.Update(" : " + Math.Round(Value / (float)DisplayDivider, DisplayPrecision).ToString());
        }

        private void SetSelectorPercent(float percent)
        {
            PulsarcLogger.Debug(percent.ToString(CultureInfo.InvariantCulture), LogType.Runtime);
            SetSelector((int) (MinValue + (MaxValue - MinValue) * percent));
        }

        /// <summary>
        /// Moves the selector to the current value
        /// </summary>
        private void SetSelector()
        {
            SetSelector(Value);
        }

        /// <summary>
        /// Finds the position of the Selector for this Slider. (0-1f relative to the size of the Slider)
        /// </summary>
        /// <param name="value">The value to find the position for.</param>
        /// <returns>A float between 0f and 1f that represents the position the Selector should be in relative to the Slider.</returns>
        private float FindSelectorPosition(int value)
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
        private float FindSelectorPosition()
        {
            return FindSelectorPosition(Value);
        }

        protected override dynamic GetSaveValue()
        {
            return Value / (float) DisplayDivider;
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

        public override void OnClick(Point mousePosition)
        {
            SetSelectorPercent((mousePosition.X - TruePosition.X) / (Texture.Width * Scale));
        }
    }
}
