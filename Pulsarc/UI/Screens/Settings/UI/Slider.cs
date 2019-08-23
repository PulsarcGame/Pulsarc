using Microsoft.Xna.Framework;
using Pulsarc.Skinning;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class Slider : Drawable
    {
        public SliderSelector selector = new SliderSelector();

        public int currentValue;
        public int minValue;
        public int maxValue;

        // How far away (in pixels) from the edges the SliderSelector needs to stop at.
        public int edgeOffset;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startingValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        public Slider(int startingValue = 50, int minValue = 0, int maxValue = 100, int edgeOffset = 15) : base(Skin.assets["slider"], -1, Anchor.CenterLeft)
        {
            selector.Resize(50);

            currentValue = startingValue;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.edgeOffset = edgeOffset;

            setSelector();
        }


        /// <summary>
        /// Moves the selector to the provided value, and changes the current value of this slider to that value.
        /// </summary>
        /// <param name="value">The value to move the selector to.</param>
        public void setSelector(int value)
        {
            currentValue = value;
            float percentagePosition = findSelectorPosition();

            int rangeMin = edgeOffset;
            int rangeMax = (int)currentSize.X - edgeOffset;

            int selectorRange = rangeMax - rangeMin;

            int position = (int)(percentagePosition * selectorRange);

            selector.position = new Vector2(this.position.X + edgeOffset + position, this.position.Y);

            System.Diagnostics.Debug.WriteLine("\nPosition should be: " + position);
            System.Diagnostics.Debug.WriteLine("\nSlider's position is: " + this.position.ToString());
            System.Diagnostics.Debug.WriteLine("\nSelector's position is: " + selector.position.ToString());
        }

        /// <summary>
        /// Moves the selector to the current value
        /// </summary>
        public void setSelector()
        {
            setSelector(currentValue);
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
            return findSelectorPosition(currentValue);
        }

        public override void Draw()
        {
            base.Draw();
            if (selector.position.Y != position.Y)
            {
                setSelector();
            }
            selector.Draw();
        }
    }
}
