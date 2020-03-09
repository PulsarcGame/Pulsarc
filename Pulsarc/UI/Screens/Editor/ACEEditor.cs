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
            AudioManager.Pause();

            base.Init(beatmap);

            Init();
        }

        protected override void InitializeVariables(in Beatmap beatmap)
        {
            Rate = 1;

            CurrentSpeedMultiplier = UserSpeed;
            // TODO: Relative with scale???
            CurrentArcsSpeed = 1;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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
