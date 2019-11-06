using Microsoft.Xna.Framework;
using Pulsarc.Beatmaps;
using Pulsarc.Beatmaps.Events;
using Pulsarc.Utils;
using System;
using System.Collections.Generic;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Editor
{
    abstract class EditorEngine : PulsarcScreen, IEditor
    {
        public override ScreenView View { get; protected set; }

        protected List<object> editorClipboard = new List<object>();
        protected List<object> selectedItems = new List<object>();

        // A collection of each action that has happened.
        // Used for Undo/Redo functions.
        protected List<EditorAction> actions = new List<EditorAction>();

        // The current index in actions.
        private int _actionIndex;
        protected int ActionIndex
        {
            get => _actionIndex;

            // If the value is outside the range of possible indexes,
            // change it to minimum or maximum values
            set => _actionIndex = value < 0 ? 0 : value >= actions.Count ? actions.Count - 1 : value;
        }

        // The current action state, determined by the ActionIndex
        private List<EditorAction> CurrentActionState => actions.GetRange(0, actions.Count - 1 - ActionIndex);

        // Current beatmap being edited
        public Beatmap Beatmap { get; protected set; }

        // Current Time
        public double Time
        {
            get => AudioManager.GetTime();
            set => AudioManager.Seek(value);
        }

        // The currently selected rate
        public float Rate
        {
            get => AudioManager.AudioRate;
            set => AudioManager.AudioRate = value;
        }

        // The current scale
        // The lower the scale, the closer the arcs are from each other
        // The higher the scale, the farther the arcs are from each other
        public float Scale { get; protected set; }

        public EditorEngine(Beatmap beatmap)
        {
            Init(beatmap);
        }

        public void Init(Beatmap beatmap)
        {

        }

        public override void Update(GameTime gameTime)
        {

        }

        public void SetFirstOffset(TimingPoint timingPoint)
        {
            // If there's a special window/tool used to find the
            // first offset, it will be here.
            // ...
            
            AddTimingPoint(timingPoint);
        }

        public void SetFirstOffset(int time, double bpm = 120)
        {
            SetFirstOffset(new TimingPoint(time, bpm));
        }

        #region Time Navigation Methods
        public void SetTime(double time)
        {
            Time = time;
        }

        public void ScrollTime(double delta)
        {
            SetTime(Time + delta);
        }

        public void Resume()
        {
            AudioManager.Resume();
        }

        public void Pause()
        {
            AudioManager.Pause();
        }
        #endregion

        #region Add Item Methods
        public abstract void AddEvent(Event evnt);

        public abstract void AddEvent(int time, EventType eventType);

        public abstract void AddHitObject(EditorHitObject hitObject);

        public abstract void AddHitObject(int time, double angle, int keys);

        public abstract void AddTimingPoint(TimingPoint timingPoint);

        public abstract void AddTimingPoint(int time, double bpm);

        public abstract void AddZoomEvent(ZoomEvent zoomEvent);

        public abstract void AddZoomEvent(int time, ZoomType zoomType, float zoomLevel, int endTime);
        #endregion

        #region Shortcutable Methods

        public void DeleteSelection()
        {
            // Remove Selection from the editor.
            // ...

            actions.Add(new EditorAction(selectedItems, false));

            selectedItems = new List<object>();
        }

        public void Copy()
        {
            editorClipboard = selectedItems;
        }

        public void Cut()
        {
            Copy();
            DeleteSelection();
        }

        public void Paste(int time)
        {
            // Paste the items at time provided.
            // ...

            actions.Add(new EditorAction(selectedItems, true));
        }

        public void Undo()
        {
            ActionIndex++;
        }

        public void Redo()
        {
            ActionIndex--;
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
        public void Select(object item)
        {
            selectedItems = new List<object>() { item };
        }

        public void AddToSelection(object item)
        {
            selectedItems.Add(item);
        }

        public void RemoveFromSelection(object item)
        {
            selectedItems.Remove(item);
        }
        #endregion
    }
}
