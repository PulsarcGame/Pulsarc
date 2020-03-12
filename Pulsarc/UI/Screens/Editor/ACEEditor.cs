using Pulsarc.Beatmaps;
using Pulsarc.Beatmaps.Events;
using Pulsarc.UI.Screens.BaseEngine;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Utils;
using System.Collections.Generic;
using Wobble.Screens;
using System;
using Pulsarc.Utils.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.UI.Screens.Editor.UI;
using System.Diagnostics;

namespace Pulsarc.UI.Screens.Editor
{
    public class ACEEditor : ArcCrosshairEngine, IEditor
    {
        private ACEEditorView GetEditorView() => (ACEEditorView)View;
        protected override ScreenView SetView() => new ACEEditorView(this);

        private Stopwatch LastKeyPressTimer = new Stopwatch();

        internal List<BeatCircle> BeatCircles { get; private set; } = new List<BeatCircle>();

        // Adds a buffer of 5ms between keypresses to reduce irresponsive presses.
        private bool ReadyForNewInput
        {
            get => LastKeyPressTimer.ElapsedMilliseconds >= 5;
            set
            {
                if (!value)
                {
                    LastKeyPressTimer.Restart();
                }
            }
        }

        public new float Rate
        {
            get => Editor.Rate;
            set => Editor.Rate = value;
        }

        public ACEEditor() { }

        public override void Init(Beatmap beatmap)
        {
            AudioManager.Stop();

            base.Init(beatmap);

            Editor.Active = true;
            AudioManager.StartAudioPlayer();

            // Wait for AudioManager to finish loading, we're dependent on GetSongDuration()
            while (!AudioManager.Active) { }

            // Don't run the music while loading
            AudioManager.Pause();
            AudioManager.Seek(0);

            // Load the beat circles
            LoadBeatCircles(beatmap);
            Editor.BeatSnapInterval = Beat.Eighth;

            GC.Collect();

            Init();
        }

        #region Init Methods
        protected override void InitializeVariables(in Beatmap beatmap)
        {
            AudioManager.AudioRate = 1;

            CurrentSpeedMultiplier = Config.GetInt("Gameplay", "ApproachSpeed") / 5f / Rate;
            // TODO: Relative with scale???
            CurrentArcsSpeed = 1;

            LastKeyPressTimer.Start();
        }

        private void LoadBeatCircles(in Beatmap beatmap)
        {
            for (int i = 0; i < beatmap.TimingPoints.Count; i++)
            {
                double bpm = beatmap.TimingPoints[i].Bpm;
                double currentTime = beatmap.TimingPoints[i].Time;

                double endTime = beatmap.TimingPoints.Count > i + 1 
                    ? beatmap.TimingPoints[i].Time
                    : AudioManager.GetSongDuration();

                bool timingPointMade = false;

                while (currentTime < endTime)
                {
                    if (timingPointMade)
                    {
                        BeatCircles.Add(new BeatCircle(
                            Beat.Whole, (int)currentTime, CurrentSpeedMultiplier));
                    }
                    else
                    {
                        BeatCircles.Add(new TimingPointCircle(beatmap.TimingPoints[i], CurrentSpeedMultiplier));
                        timingPointMade = true;
                    }

                    double wholeStep = bpm / 60d * 1000d;

                    MakeHalfBeats(currentTime, wholeStep, endTime);

                    MakeThirdBeats(currentTime, wholeStep, endTime);

                    currentTime += bpm / 60d * 1000d;
                }
            }

            BeatCircles.Sort((x, y) => x.Time.CompareTo(y.Time));
        }

        private void MakeHalfBeats(in double currentTime, in double wholeStep, in double endTime)
        {
            double halfStep = wholeStep / 2d;

            if (currentTime + halfStep < endTime)
            {
                BeatCircles.Add(new BeatCircle(
                    Beat.Half, (int)(currentTime + halfStep), CurrentSpeedMultiplier));
            }

            MakeAFourthBeat(currentTime, halfStep, endTime);
            MakeAFourthBeat(currentTime + halfStep, halfStep, endTime);
        }

        private void MakeAFourthBeat(in double currentTime, in double halfStep, in double endTime)
        {
            double fourthStep = halfStep / 2d;
            
            if (currentTime + halfStep < endTime)
            {
                BeatCircles.Add(new BeatCircle(
                    Beat.Fourth, (int)(currentTime + fourthStep), CurrentSpeedMultiplier));
            }

            MakeAnEighthBeat(currentTime, fourthStep, endTime);
            MakeAnEighthBeat(currentTime + fourthStep, fourthStep, endTime);
        }

        private void MakeAnEighthBeat(in double currentTime, in double fourthStep, in double endTime)
        {
            double eighthStep = fourthStep / 2d;

            if (currentTime + eighthStep < endTime)
            {
                BeatCircles.Add(new BeatCircle(
                    Beat.Eighth, (int)(currentTime + eighthStep), CurrentSpeedMultiplier));
            }

            MakeASixteenthBeat(currentTime, eighthStep, endTime);
            MakeASixteenthBeat(currentTime + eighthStep, eighthStep, endTime);
        }

        private void MakeASixteenthBeat(in double currentTime, in double eighthStep, in double endTime)
        {
            double sixteenthStep = eighthStep / 2d;

            if (currentTime + sixteenthStep < endTime)
            {
                BeatCircles.Add(new BeatCircle(
                    Beat.Sixteenth, (int)(currentTime + sixteenthStep), CurrentSpeedMultiplier));
            }
        }

        private void MakeThirdBeats(in double currentTime, in double wholeStep, in double endTime)
        {
            double thirdStep = wholeStep / 3d;

            if (currentTime + thirdStep < endTime)
            {
                BeatCircles.Add(new BeatCircle(
                    Beat.Third, (int)(currentTime + thirdStep), CurrentSpeedMultiplier));
            }

            MakeASixthBeat(currentTime, thirdStep, endTime);
            MakeASixthBeat(currentTime + thirdStep, thirdStep, endTime);
            MakeASixthBeat(currentTime + (thirdStep * 2d), thirdStep, endTime);
        }

        private void MakeASixthBeat(in double currentTime, in double thirdStep, in double endTime)
        {
            double sixthStep = thirdStep / 2d;

            if (currentTime + sixthStep < endTime)
            {
                BeatCircles.Add(new BeatCircle(
                    Beat.Sixth, (int)(currentTime + sixthStep), CurrentSpeedMultiplier));
            }

            MakeATwelvethBeat(currentTime, sixthStep, endTime);
            MakeATwelvethBeat(currentTime, sixthStep, endTime);
        }

        private void MakeATwelvethBeat(in double currentTime, in double sixthStep, in double endTime)
        {
            double twelvethStep = sixthStep / 2d;

            if (currentTime + twelvethStep < endTime)
            {
                BeatCircles.Add(new BeatCircle(
                    Beat.Twelveth, (int)(currentTime + twelvethStep), CurrentSpeedMultiplier));
            }
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            // If not active, don't update
            if (!Editor.Active) { return; }

            HandleKeyPresses();

            HandleMouseInput();

            UpdateEditor();

            // Handle visuals
            View.Update(gameTime);
        }

        private void HandleKeyPresses()
        {
            // Handle multi-key shortcuts (CTRL+Z, CTRL+Y, etc.)
            HandleShortcuts();

            // Handle single-key actions
            HandleOtherPresses();
        }

        private void HandleShortcuts()
        {

        }

        private void HandleOtherPresses()
        {
            if (!ReadyForNewInput) { return; }

            if (InputManager.KeyPressed(Keys.Space))
            {
                if (AudioManager.Paused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }

                ReadyForNewInput = false;
            }
        }

        private int lastScrollValue = 0;

        private void HandleMouseInput()
        {
            MouseState ms = InputManager.MouseState;

            if (ms.ScrollWheelValue < lastScrollValue)
            {
                ScrollToLastBeatDisplay();
            }
            else if (ms.ScrollWheelValue > lastScrollValue)
            {
                ScrollToNextBeatDisplay();
            }
            lastScrollValue = ms.ScrollWheelValue;
        }

        private void UpdateEditor()
        {
            if (CurrentBeatmap == null) { return; }

            UpdateArcs();

            UpdateBeatCircles();
        }

        private void UpdateArcs()
        {
            for (int i = 0; i < KeyCount; i++)
            {
                bool updatedAll = false;

                ref Column currentColumn = ref Columns[i];

                int startIndex = Math.Max(currentColumn.UpdateHitObjects.FindIndex(x => x.ZLocation < 8000), 0);

                for (int k = startIndex;
                    k < currentColumn.UpdateHitObjects.Count && !updatedAll;
                    k++)
                {
                    HitObject currentHitObject = currentColumn.UpdateHitObjects[k];

                    // Process new position of this object
                    currentHitObject.RecalcPos((int)Time, CurrentSpeedMultiplier,
                        Crosshair.GetZLocation());

                    // Ignore following objects if we have reached the ignore distance.
                    if (currentHitObject
                        .IsSeenAt(CurrentSpeedMultiplier, Crosshair.GetZLocation()) - IgnoreTime
                            > Time)
                    {
                        updatedAll = true;
                    }
                }
            }
        }

        private void UpdateBeatCircles()
        {
            int startIndex = Math.Max(BeatCircles.FindIndex(x => x.ZLocation < 8000), 0);

            bool updatedAll = false;

            for (int i = startIndex; i < BeatCircles.Count && !updatedAll; i++)
            {
                BeatCircle currentBeatCircle = BeatCircles[i];

                // process new position of this object
                currentBeatCircle.RecalcPos((int)Time, CurrentSpeedMultiplier,
                    Crosshair.GetZLocation());

                // Ignore following objects if we have reached the ignore distance.
                if (currentBeatCircle.
                    IsSeenAt(CurrentSpeedMultiplier, Crosshair.GetZLocation()) - IgnoreTime
                        > Time)
                {
                    updatedAll = true;
                }
            }
        }

        protected override void AddHitObjectToColumn(Arc arc, int colIndex)
        {
            Columns[colIndex].AddHitObject
            (
                new EditorArcHitObject
                (
                    arc.Time,
                    (int)(colIndex / (float)KeyCount * 360),
                    KeyCount,
                    CurrentArcsSpeed
                ),
                CurrentArcsSpeed * CurrentSpeedMultiplier,
                Crosshair.GetZLocation()
            );
        }

        //These methods may not be needed
        public void SetFirstOffset(TimingPoint timingPoint)
        {
            throw new NotImplementedException();
        }

        public void SetFirstOffset(int time, double bpm = 120)
        {
            throw new NotImplementedException();
        }

        #region Time Navigation Methods
        public void SetTime(double time) => AudioManager.Seek(time);

        public void ScrollTime(double delta) => AudioManager.DeltaTime(delta);

        public void Resume() => Editor.Paused = false;

        public void Pause() => Editor.Paused = true;

        public void ScrollToNextBeatDisplay()
            => Time = BeatCircles.Find(x => x.IsActive() && x.Time > Time)?.Time ?? Time;

        public void ScrollToLastBeatDisplay()
            => Time = BeatCircles.FindLast(x => x.IsActive() && x.Time < Time)?.Time ?? Time;

        public void ScrollToTheNextWholeBeat() => ScrollToNextOccurenceOf(Beat.Whole);

        public void ScrollToTheLastWholeBeat() => ScrollToLastOccurenceOf(Beat.Whole);

        public void ScrollToNextOccurenceOf(Beat beat)
            => Time = BeatCircles.Find(x => x.Time > Time && x.Beat == beat).Time;

        public void ScrollToLastOccurenceOf(Beat beat)
            => Time = BeatCircles.FindLast(x => x.Time > Time && x.Beat == beat).Time;

        public void ScrollToFirstBeat() => Time = BeatCircles.Find(x => x.IsActive()).Time;

        public void ScrollToLastBeat() => Time = BeatCircles.FindLast(x => x.IsActive()).Time;
        #endregion

        #region Add Item Methods
        public void AddTimingPoint(TimingPoint timingPoint)
        {
            throw new NotImplementedException();
        }

        public void AddTimingPoint(int time, double bpm)
        {
            throw new NotImplementedException();
        }

        public void AddEvent(Event evnt)
        {
            throw new NotImplementedException();
        }

        public void AddEvent(int time, EventType eventType)
        {
            throw new NotImplementedException();
        }

        public void AddZoomEvent(ZoomEvent zoomEvent)
        {
            //...

            AddEvent(zoomEvent);
        }

        public void AddZoomEvent(int time, ZoomType zoomType, float zoomLevel, int endTime)
            => AddZoomEvent(new ZoomEvent(time, zoomType, zoomLevel, endTime));

        public void AddHitObject(int time, int columnIndex)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Shortcutable Methods
        public void DeleteSelection()
        {
            throw new NotImplementedException();
        }

        public void Copy()
        {
            throw new NotImplementedException();
        }

        public void Cut()
        {
            throw new NotImplementedException();
        }

        public void Paste(int time)
        {
            throw new NotImplementedException();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }

        public void Redo()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void SaveAs()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Selection Methods
        public void Select(Drawable item)
        {
            throw new NotImplementedException();
        }

        public void Select(List<Drawable> items)
        {
            throw new NotImplementedException();
        }

        public void AddToSelection(Drawable item)
        {
            throw new NotImplementedException();
        }

        public void AddToSelection(List<Drawable> items)
        {
            throw new NotImplementedException();
        }

        public void RemoveFromSelection(Drawable item)
        {
            throw new NotImplementedException();
        }

        public void RemoveFromSelection(List<Drawable> items)
        {
            throw new NotImplementedException();
        }
        #endregion

        public EditorStyle GetEditorStyle() => EditorStyle.ACEStyle;

        public override void UpdateDiscord()
            => PulsarcDiscord.SetStatus("Editing Map: ", CurrentBeatmap.Title);

        public void ChangeBeatInterval(Beat beat) => Editor.BeatSnapInterval = beat;
    }
}
