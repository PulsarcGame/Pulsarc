using Pulsarc.Beatmaps;
using Pulsarc.UI.Screens.Gameplay;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    public class PlayCard : BeatmapCard
    {
        public PlayCard(Beatmap beatmap, int index) : base(beatmap, index) { }

        protected override void TransitionToNewScreen()
        {
            GameplayEngine gameplay = new GameplayEngine();
            ScreenManager.AddScreen(gameplay);
            gameplay.Init(Beatmap);
        }
    }
}
