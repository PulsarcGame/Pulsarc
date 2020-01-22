using Microsoft.Xna.Framework;
using Pulsarc.Utils.Audio;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class RateSlider : Slider
    {

        public RateSlider(string title, string more, Vector2 position, string type, int startingValue = 50, int minValue = 0, int maxValue = 100, int step = 1, int displayDivider = 1, int displayPrecision = 2, int edgeOffset = 15) :
            base(title, more, position, type, startingValue, minValue, maxValue, step, displayDivider, displayPrecision, edgeOffset)
        { }

        public override void OnClick(Point mousePosition) => base.OnClick(mousePosition);

        public override void Save(string category, string key)
        {
            base.Save(category, key);
            AudioManager.UpdateRate();
        }
    }
}
