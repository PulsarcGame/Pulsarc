using Pulsarc.Skinning;
using Pulsarc.UI.Screens.Gameplay;
using Wobble.Audio.Samples;

namespace Pulsarc.Utils.Audio
{
    public static class SampleManager
    {
        private static double SampleVolume = Config.EffectVolume.Value;

        private static bool MAXHitsound;
        private static bool PerfectHitsound;
        private static bool GreatHitsound;
        private static bool GoodHitsound;
        private static bool BadHitsound;
        private static bool MissSound;

        public static void Init()
        {
            MAXHitsound = Skin.GetConfigBool("audio", "Hitsounds", "MAXHitsound");
            PerfectHitsound = Skin.GetConfigBool("audio", "Hitsounds", "PerfectHitsound");
            GreatHitsound = Skin.GetConfigBool("audio", "Hitsounds", "GreatHitsound");
            GoodHitsound = Skin.GetConfigBool("audio", "Hitsounds", "GoodHitsound");
            BadHitsound = Skin.GetConfigBool("audio", "Hitsounds", "BadHitsound");
            MissSound = Skin.GetConfigBool("audio", "Hitsounds", "MissSound");
        }

        public static void PlayHitSound(in JudgementValue judge)
        {
            bool playSound;

            switch (judge.Name)
            {
                case "max":
                    playSound = MAXHitsound;
                    break;
                case "perfect":
                    playSound = PerfectHitsound;
                    break;
                case "great":
                    playSound = GreatHitsound;
                    break;
                case "good":
                    playSound = GoodHitsound;
                    break;
                case "bad":
                    playSound = BadHitsound;
                    break;
                case "miss":
                    PlayMissSound();
                    return;
                default:
                    return;
            }

            if (playSound)
            {
                PlaySample("hit");
            }
        }

        public static void PlayMissSound()
        {
            if (MissSound)
            {
                PlaySample("miss");
            }
        }

        private static void PlaySample(string name)
        {
            using (AudioSampleChannel channel = Skin.Sounds[name].CreateChannel())
            {
                channel.Volume = SampleVolume;
                channel.Play();
            }
        }
    }
}
