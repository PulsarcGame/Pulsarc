using Microsoft.Xna.Framework;
using Pulsarc.Skinning;
using Pulsarc.Utils;
using System;

namespace Pulsarc.UI.Screens.Settings.UI
{
    public class RateSlider : Slider
    {

        public RateSlider(string title, string more, Vector2 position, string type, int startingValue = 50, int minValue = 0, int maxValue = 100, int step = 1, int displayDivider = 1, int displayPrecision = 2, int edgeOffset = 15) : 
            base(title, more, position, type, startingValue, minValue, maxValue, step, displayDivider, displayPrecision, edgeOffset)
        { }


        public override void Save(string category, string key)
        {
            base.Save(category, key);

            if(AudioManager.active)
            {
                double time = AudioManager.getTime();
                string audio = AudioManager.song_path;
                AudioManager.Stop();

                AudioManager.song_path = audio;
                AudioManager.audioRate = Config.getFloat("Gameplay", "SongRate");
                AudioManager.StartLazyPlayer();

                if (time != 0)
                {
                    AudioManager.deltaTime((long) time);
                }

                Console.WriteLine("Now Playing: " + AudioManager.song_path + " at " + AudioManager.audioRate + " rate");
            }
        }
    }
}
