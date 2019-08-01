﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Beatmaps;
using Pulsarc.UI.Screens.SongSelect.UI;
using Pulsarc.Utils;
using System.Collections.Generic;
using System.IO;
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

            foreach (string dir in Directory.GetDirectories("Songs/"))
            {
                foreach (string file in Directory.GetFiles(dir, "*.psc"))
                {
                    beatmaps.Add(BeatmapHelper.Load(file));
                }
            }

            View = new SongSelectionView(this, beatmaps);
        }

        public override void Update(GameTime gameTime)
        {
            while (KeyboardInputManager.keyboardPresses.Count > 0)
            {
                KeyValuePair<double, Keys> press = KeyboardInputManager.keyboardPresses.Dequeue();

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