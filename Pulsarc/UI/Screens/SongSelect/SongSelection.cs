using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Beatmaps;
using Pulsarc.UI.Screens.SongSelect.UI;
using Pulsarc.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wobble.Input;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.SongSelect
{
    class SongSelection : PulsarcScreen
    {

        public override ScreenView View { get; protected set; }

        public List<BeatmapCard> cards;

        int lastScrollValue = 0;
        bool leftClicking = false;
        Vector2 leftClickingPos;

        public int currentFocus = 0;

        public SongSelection()
        {
        }

        public override void Init()
        {
            RefreshBeatmaps();
        }

        public void RefreshBeatmaps()
        {
            List<Beatmap> beatmaps = new List<Beatmap>();
            cards = new List<BeatmapCard>();

            // Ultimately, this should be loaded from a cache and updated incrementally or when doing a refresh
            foreach (string dir in Directory.GetDirectories("Songs/"))
            {
                foreach (string file in Directory.GetFiles(dir, "*.psc"))
                {
                    beatmaps.Add(BeatmapHelper.Load(file));
                }
            }
            beatmaps = sortBeatmaps(beatmaps, "difficulty");
            View = new SongSelectionView(this, beatmaps);
        }

        public List<Beatmap> sortBeatmaps(List<Beatmap> beatmaps, string sort)
        {
            switch(sort)
            {
                case "difficulty":
                    return beatmaps.OrderBy(i => i.Difficulty).ToList();
                case "artist":
                    return beatmaps.OrderBy(i => i.Artist).ToList();
                case "title":
                    return beatmaps.OrderBy(i => i.Title).ToList();
                case "mapper":
                    return beatmaps.OrderBy(i => i.Mapper).ToList();
                case "version":
                    return beatmaps.OrderBy(i => i.Version).ToList();
                default:
                    return beatmaps;
            }
        }

        public override void Update(GameTime gameTime)
        {
            while (InputManager.keyboardPresses.Count > 0)
            {
                KeyValuePair<double, Keys> press = InputManager.keyboardPresses.Dequeue();

                if (press.Value == Keys.Escape || press.Value == Keys.Delete)
                {
                    ScreenManager.RemoveScreen(true);
                }
            }

            MouseState ms = Mouse.GetState();
            
            if (ms.ScrollWheelValue < lastScrollValue)
            {
                currentFocus--;
            }
            else if (ms.ScrollWheelValue > lastScrollValue)
            {
                currentFocus++;
            }

            lastScrollValue = ms.ScrollWheelValue;

            if (!leftClicking && ms.LeftButton == ButtonState.Pressed)
            {
                leftClicking = true;
                leftClickingPos = new Vector2(ms.Position.X, ms.Position.Y);
            }
            else if (leftClicking && ms.LeftButton == ButtonState.Released)
            {
                leftClicking = false;
                Vector2 leftReleasePos = new Vector2(ms.Position.X, ms.Position.Y);
                foreach (BeatmapCard card in cards)
                {
                    if (card.clicked(leftClickingPos) && card.clicked(leftReleasePos))
                    {
                        card.onClick();
                    }
                }
            }
            else
            {
                View?.Update(gameTime);
            }
        }
    }
}
