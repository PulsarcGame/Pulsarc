using Pulsarc.Beatmaps;
using Pulsarc.UI.Screens.Editor;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect.UI
{
    public class EditCard : BeatmapCard
    {
        public EditCard(Beatmap beatmap, int index) : base(beatmap, index) { }

        protected override void TransitionToNewScreen()
        {
            ACEEditor editor = new ACEEditor();
            ScreenManager.AddScreen(editor);
            editor.Init(Beatmap);
        }
    }
}
