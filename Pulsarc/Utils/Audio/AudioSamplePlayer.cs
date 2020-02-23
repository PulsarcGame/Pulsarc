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
        private static int HitSoundID = Skin.Sounds["hitsound"];
        private static int HitSoundChannelID;

        public static void Init()
        {
            HitSoundID = Skin.Sounds["hitsounds"];
            HitSoundChannelID = Bass.SampleGetChannel(HitSoundID);
        }

        public static void PlayHitSound()
        {
            try
            {
                Bass.ChannelPlay(HitSoundChannelID);
            }
            catch
            {
                PulsarcLogger.Debug("Some shit went wrong!!!");
            }
        }
    }
}
