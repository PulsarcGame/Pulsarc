using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using System;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class Slider : Setting
    {
        public SliderSelector selector = new SliderSelector();

        public int minValue;
        public int maxValue;
        public int step;
        public int displayDivider;
        public int displayPrecision;

        // How far away (in pixels) from the edges the SliderSelector needs to stop at.
        public int edgeOffset;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startingValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        public Slider(string title, string more, Vector2 position, string type, int startingValue = 50, int minValue = 0, int maxValue = 100, int step = 1, int displayDivider = 1, int displayPrecision = 2, int edgeOffset = 15) : base(title, more, position, Skin.assets["slider"], -1, Anchor.CenterLeft, startingValue, type)
        {
            selector.Resize(50);

            value = startingValue;
            this.step = step;
            this.displayDivider = displayDivider;
            this.displayPrecision = displayPrecision;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.edgeOffset = edgeOffset;

            setSelector();
            Update();
        }


        /// <summary>
        /// Moves the selector to the provided value, and changes the current value of this slider to that value.
        /// </summary>
        /// <param name="value">The value to move the selector to.</param>
        public void setSelector(int value)
        {
            int stepdist = this.value - value;
            if(Math.Abs(stepdist) >= step/2)
            {
                this.value -= step * (stepdist / step);
                Update();
            }
            float percentagePosition = findSelectorPosition();

            int rangeMin = edgeOffset;
            int rangeMax = (int)currentSize.X - edgeOffset;

            int selectorRange = rangeMax - rangeMin;

            int position = (int)(percentagePosition * selectorRange);

            selector.changePosition(new Vector2(basePosition.X + position, basePosition.Y));
        }

        public void Update()
        {
            title.Update(" : " + Math.Round(value / (float)displayDivider, displayPrecision).ToString());
        }

        public void setSelectorPercent(float percent)
        {
            Console.WriteLine(percent);
            setSelector((int) (minValue + (maxValue - minValue) * percent));
        }

        /// <summary>
        /// Moves the selector to the current value
        /// </summary>
        public void setSelector()
        {
            setSelector(value);
        }

        /// <summary>
        /// Finds the position of the Selector for this Slider. (0-1f relative to the size of the Slider)
        /// </summary>
        /// <param name="value">The value to find the position for.</param>
        /// <returns>A float between 0f and 1f that represents the position the Selector should be in relative to the Slider.</returns>
        public float findSelectorPosition(int value)
        {
            int range = maxValue - minValue;

            float position = (value - minValue) / (float)range;

            // If position is greater than 1, return 1, else if its lower than 0, return 0
            // If neither of those, return position
            return position > 1f ? 1f : position < 0f ? 0f : position;
        }

        /// <summary>
        /// Finds the position of the Selector for this Slider (0f-1f relative to the size of the Slider)
        /// This instance of the method uses the currentValue of this slider.
        /// </summary>
        /// <returns>A float between 0f and 1f that represents the position the Selector should be in relation to the size of the Slider.</returns>
        public float findSelectorPosition()
        {
            return findSelectorPosition(value);
        }

        public override dynamic getSaveValue()
        {
            return value / (float) displayDivider;
        }

        public override void Draw()
        {
            base.Draw();
            selector.Draw();
        }

        public override void onClick(Point mousePosition)
        {
            setSelectorPercent((float) Math.Round((mousePosition.X - drawPosition.X) / ((Texture.Width) * scale),2));
        }
    }
}
