using Microsoft.Xna.Framework;
using Pulsarc.UI.Screens.Gameplay.UI;
using System;
using System.Collections.Generic;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Gameplay
{
    public class GameplayEngineView : ScreenView
    {

        // UI Elements

        Accuracy accuracyDisplay;
        Score scoreDisplay;
        Combo comboDisplay;
        JudgeBox judgeBox;
        AccuracyMeter accMeter;
        public Crosshair crosshair;

        private GameplayEngine GetGameplayEngine() { return (GameplayEngine)Screen; }
        public GameplayEngineView(Screen screen) : base(screen) { }

        public void Init()
        {
            // Initialize UI
            crosshair = new Crosshair(GetGameplayEngine().currentCrosshairRadius);

            scoreDisplay = new Score(new Vector2(Pulsarc.getDimensions().X / 2, 20), centered: true);
            accuracyDisplay = new Accuracy(new Vector2(Pulsarc.getDimensions().X / 2, 50), centered: true);
            comboDisplay = new Combo(new Vector2(Pulsarc.getDimensions().X / 2, 80), centered: true);

            judgeBox = new JudgeBox(new Vector2(Pulsarc.getDimensions().X / 2, Pulsarc.getDimensions().Y / 2));
            accMeter = new AccuracyMeter(new Vector2(960, 1080 - 25), new Vector2(200, 25));
        }

        public void addHit(long time, int error, int judge)
        {
            accMeter.addError(time, error);
            addJudge(time, judge);
        }

        public void addJudge(long time, int judge)
        {
            judgeBox.Add(time, judge);
        }

        public override void Update(GameTime gameTime)
        {

            double accuracyTotal = 0;
            foreach (JudgementValue judge in GetGameplayEngine().judgements)
            {
                accuracyTotal += judge.acc;
            }

            accuracyDisplay.Update(GetGameplayEngine().errors.Count > 0 ? accuracyTotal / GetGameplayEngine().errors.Count : 1);
            scoreDisplay.Update(GetGameplayEngine().score_display);
            comboDisplay.Update(GetGameplayEngine().combo);
            judgeBox.Update(GetGameplayEngine().getElapsed());
            accMeter.Update(GetGameplayEngine().getElapsed());
        }

        public override void Draw(GameTime gameTime)
        {
            if (!GameplayEngine.active) return;
            // Draw everything
            crosshair.Draw();

            for (int i = 0; i < GetGameplayEngine().keys; i++)
            {
                foreach (KeyValuePair<long, HitObject> pair in GetGameplayEngine().columns[i].updateHitObjects)
                {
                    if (pair.Value.IsSeen())
                    {
                        pair.Value.Draw();
                    }
                    if (pair.Key - GetGameplayEngine().msIgnore > GetGameplayEngine().getElapsed())
                    {
                        break; // not nice ik
                    }
                }
            }

            accuracyDisplay.Draw();
            scoreDisplay.Draw();
            comboDisplay.Draw();
            judgeBox.Draw();
            accMeter.Draw();
        }

        public bool isActive()
        {
            return GameplayEngine.active;
        }

        public override void Destroy()
        {
            GetGameplayEngine().Reset();
        }
    }
}
