using ManagedBass;
using Pulsarc.Skinning;
using System;
using System.Collections.Generic;
using System.Text;
using Wobble.Audio.Samples;

namespace Pulsarc.Utils.Audio
{
    public static class AudioSamplePlayer
    {
        private static double SampleVolume => Config.GetInt("Audio", "EffectVolume");

        public static void PlayHitSound(int judge = 0) => PlaySample("hitsound");

        public static void PlaySample(string name)
        {
            using (AudioSampleChannel channel = Skin.Sounds[name].CreateChannel())
            {
                channel.Volume = SampleVolume;
                channel.Play();
            }
        }
    }
}
