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

namespace Pulsarc.UI.Screens.Editor
{
    public class ACEEditor : ArcCrosshairEngine, IEditor
    {
        private ACEEditorView GetEditorView() => (ACEEditorView)View;
        protected override ScreenView SetView() => new ACEEditorView(this);

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

            GC.Collect();

            Init();
        }

        protected override void InitializeVariables(in Beatmap beatmap)
        {
            AudioManager.AudioRate = 1;

            CurrentSpeedMultiplier = Config.GetInt("Gameplay", "ApproachSpeed") / 5f / Rate;
            // TODO: Relative with scale???
            CurrentArcsSpeed = 1;
            
        }

        private bool pausedAtStartYet = false;

        public override void Update(GameTime gameTime)
        {
            // If not active, don't update
            if (!Editor.Active) { return; }

            // Pause the audio and set time to 0 on the first active update frame.
            if (!pausedAtStartYet && AudioManager.Active)
            {
                AudioManager.Pause();
                Time = 0;
                pausedAtStartYet = true;
            }

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
            Keys[] keyPresses = Keyboard.GetState().GetPressedKeys();

            if (keyPresses.Length <= 0) { return; }

            for (int i = 0; i < keyPresses.Length; i++)
            {
                if (keyPresses[i] == Keys.Space)
                {
                    Resume();
                }
            }
        }

        private int lastScrollValue = 0;

        private void HandleMouseInput()
        {
            MouseState ms = Mouse.GetState();

            if (ms.ScrollWheelValue < lastScrollValue)
            {
                ScrollTime(100);
            }
            else if (ms.ScrollWheelValue > lastScrollValue)
            {
                ScrollTime(-100);
            }
            lastScrollValue = ms.ScrollWheelValue;
        }

        private void UpdateEditor()
        {
            if (CurrentBeatmap == null) { return; }

            UpdateArcs();

            //UpdateBeatCircles();
        }

        private void UpdateArcs()
        {
            for (int i = 0; i < KeyCount; i++)
            {
                //PulsarcLogger.Debug($"\nColumn: {i}");

                bool updatedAll = false;

                ref Column currentColumn = ref Columns[i];

                int startIndex = Math.Max(currentColumn.UpdateHitObjects.FindIndex(x => x.ZLocation < 8000), 0);

                for (int k = startIndex;
                    k < currentColumn.UpdateHitObjects.Count && !updatedAll;
                    k++)
                {
                    //PulsarcLogger.Debug($"{CurrentSpeedMultiplier}");

                    HitObject currentHitObject = currentColumn.UpdateHitObjects[k];

                    // Process new position of this object
                    currentHitObject.RecalcPos((int)Time, CurrentSpeedMultiplier,
                        Crosshair.GetZLocation());

                    // Ignore following objects if we have reached the ignore distance.
                    if (currentHitObject
                        .IsSeenAt(CurrentSpeedMultiplier, Crosshair.GetZLocation()) 
                        - IgnoreTime > Time)
                    {
                        //PulsarcLogger.Debug("AllDone!");
                        updatedAll = true;
                    }
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
    }
}
