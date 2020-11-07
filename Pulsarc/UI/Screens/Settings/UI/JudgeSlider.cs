using Microsoft.Xna.Framework;
using Pulsarc.UI.Screens.Gameplay;
using System;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class JudgeSlider : Slider
    {
        private JudgeSlider previous;
        private JudgeSlider next;

        public JudgeSlider(string title, string more, Vector2 position, string type, JudgeSlider previous, int startingValue = 50, int minValue = 0, int maxValue = 100, int step = 1, int displayDivider = 1, int displayPrecision = 2, int edgeOffset = 15) : 
            base(title, more, position, type, startingValue, minValue, maxValue, step, displayDivider, displayPrecision, edgeOffset)
        {
            this.previous = previous;
        }

        public override void OnClick(Vector2 mousePosition)
        {
            base.OnClick(mousePosition);
            if (previous != null)
            {
                previous.UpdateValueMaxPrevious();
            }
            UpdateValueMinNext();
        }

        public void UpdateValueMaxPrevious()
        {
            if (next != null)
            {
                int max = next.Value;
                if (max < Value)
                {
                    SetSelector(max);
                }
            }
            if (previous != null)
            {
                previous.UpdateValueMaxPrevious();
            }
        }

        public void UpdateValueMinNext()
        {
            if (previous != null)
            {
                int min = previous.Value;
                if (min > Value)
                {
                    SetSelector(min);
                }
            }
            if (next != null)
            {
                next.UpdateValueMinNext();
            }
        }

        public void SetNext(JudgeSlider next)
        {
            this.next = next;
        }

        public override void Save(string category, string key)
        {
            base.Save(category, key);
            Judgement.Refresh();
        }
    }
}
