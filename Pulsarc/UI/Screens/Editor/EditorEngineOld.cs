using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pulsarc.Beatmaps;
using Pulsarc.Beatmaps.Events;
using Pulsarc.UI.Common;
using Pulsarc.UI.Screens.Editor.UI;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Utils;
using Pulsarc.Utils.Audio;
using Pulsarc.Utils.Graphics;
using System;
using System.Collections.Generic;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Editor
{
    public abstract class EditorEngineOld : PulsarcScreen, IEditor
    {
        public override ScreenView View { get; protected set; }
        private EditorEngineViewOld GetEditorView() { return (EditorEngineViewOld)View; }

        // Whether or not an Editor engine is currently running
        public static bool Active { get; protected set; } = false;

        // A list of objects that were copy or cut onto the clipboard
        // TODO: Make it copy to the System Clipboard too. Could use the ToString() of
        // each object so it matches the .psc format.
        protected List<Drawable> editorClipboard = new List<Drawable>();

        // The currently selected items in the editor.
        protected List<Drawable> selectedItems = new List<Drawable>();

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

        // All the "tracks" or "directions" EditorHitObjects can come from
        public Column[] Columns { get; protected set; }

        // Used to store the key-style of the current map (4k, 7k, etc.)
        public int KeyCount { get; protected set; }

        // Background
        public Background Background { get; protected set; }

        // Unsure if needed, keeping just in case
        protected int EventIndex;
        public Event NextEvent { get; protected set; }

        // Events that are currently being handled
        public List<Event> ActiveEvents { get; private set; } = new List<Event>();

        // Toggle Events on or off
        public virtual bool EventsOn { get; set; }

        // Current Event modifiers. Effects can be toggled on or off
        // These need to be tracked even if not in a Gameplay Style editor
        public virtual double CurrentSpeedMultiplier { get; set; }
        public virtual double CurrentArcSpeed { get; set; }

        public virtual double CurrentZoomLevel { get; set; }

        // Whether time is paused. This doesn't prevent moving through time.
        public static bool Paused
        {
            get => AudioManager.paused;
            set
            {
                if (AudioManager.paused && value == false)
                    AudioManager.Resume();
                else if (!AudioManager.paused && value == true)
                    AudioManager.Pause();
            }
        }

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

        // Current interval for Beat Snapping (1/1, 1/2, 1/3, 1/4, etc.)
        public Beat BeatSnapInterval { get; protected set; } = Beat.Whole;

        // Whether added notes should snap to the closest BeatDisplay or not.
        public bool BeatLocked { get; protected set; } = true;

        // Whether the Editor is in the state to add HitObjects with a mouse click.
        public bool CanAddHitObjects { get; protected set; } = true;

        // Time distance (in ms) from which hitobjects are neither updated nor drawn.
        public int IgnoreTime { get; private set; } = 500;

        protected int LastScrollValue { get; set; } = 0;

        public EditorEngineOld() => View = CreateEditorView();

        protected abstract EditorEngineViewOld CreateEditorView();

        public void Init(Beatmap beatmap)
        {
            if (beatmap == null) { return; }

            if (!beatmap.FullyLoaded)
            {
                beatmap = BeatmapHelper.Load(beatmap.Path, beatmap.FileName);
            }

            EventIndex = 0;
            NextEvent = beatmap.Events.Count > 0 ? beatmap.Events[EventIndex] : null;

            KeyCount = 4;
            Columns = new Column[KeyCount];

            Background = new Background(Config.GetInt("Editor", "BackgroundDim") / 100f);
            Background.ChangeBackground(GraphicsUtils.LoadFileTexture($"{beatmap.Path}/{beatmap.Background}"));

            AudioManager.songPath = beatmap.GetFullAudioPath();

            Beatmap = beatmap;

            CreateColumns();

            foreach (Column col in Columns)
            {
                col.SortUpdateHitObjects();
            }

            GetEditorView().Init();

            StartEditor();

            Init();
        }

        #region Init Methods
        // How columns are created depends on the style of Editor we're using.
        protected abstract void CreateColumns();

        protected void StartEditor()
        {
            AudioManager.StartEditorPlayer();
            EditorEngineOld.Active = true;
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            // If not active, don't update.
            if (!Active)
                return;

            HandleInputs();

            HandleEvents();
        }

        #region Update Methods
        protected virtual void HandleInputs()
        {
            HandleKeyboard();
            HandleMouse();
        }

        protected virtual void HandleKeyboard()
        {
            // TODO: Add a timer that prevents endless actions when holding shortcuts down.

            KeyboardState ks = Keyboard.GetState();

            // Pause/Resume
            if (ks.IsKeyDown(Keys.Space))
                if (Paused)
                    Resume();
                else
                    Pause();

            // Move time with arrow keys
            // Currently moves the current scale per second
            if (ks.IsKeyDown(Keys.Left))
                ScrollTime(-Scale / 1000 * PulsarcTime.DeltaTime);
            else if (ks.IsKeyDown(Keys.Right))
                ScrollTime(Scale / 1000 * PulsarcTime.DeltaTime);

            // Delete with delete or backspace
            if (ks.IsKeyDown(Keys.Delete) || ks.IsKeyDown(Keys.Back))
                DeleteSelection();

            // Copy/Cut/Paste
            if (ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl))
                if (ks.IsKeyDown(Keys.C))
                    Copy();
                else if (ks.IsKeyDown(Keys.X))
                    Cut();
                else if (ks.IsKeyDown(Keys.V))
                    Paste(FindTime(Mouse.GetState().Position));

            // Undo/Redo
            if (ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl))
                if (ks.IsKeyDown(Keys.Z))
                    Undo();
                else if (ks.IsKeyDown(Keys.Y))
                    Redo();

            // Save/Save As/Open
            if (ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl))
                if (ks.IsKeyDown(Keys.S))
                    Save();
                else if (ks.IsKeyDown(Keys.S) &&
                        (ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift)))
                    SaveAs();
                else if (ks.IsKeyDown(Keys.O))
                    Open();
        }

        protected virtual void HandleMouse()
        {
            MouseState ms = Mouse.GetState();

            // Move time with mouse wheel
            // TODO: Find X and change to "ScrollTime(Scale * X)"
            if (ms.ScrollWheelValue < LastScrollValue)
                ScrollTime(-Scale);
            else if (ms.ScrollWheelValue > LastScrollValue)
                ScrollTime(Scale);
            LastScrollValue = ms.ScrollWheelValue;

            // Add HitObject
            if (ms.LeftButton == ButtonState.Pressed)
            {
                Drawable clickedItem;
                if (ClickedAnObject(ms.Position, out clickedItem))
                    Select(clickedItem);
                else if (CanAddHitObjects)
                    AddHitObject(FindTime(ms.Position), FindClosestColumn(ms.Position));
            }
        }

        protected virtual void HandleEvents()
        {
            if (Beatmap == null || !EventsOn)
                return;

            ActivateNextEvent();

            HandleActiveEvents();
        }

        protected virtual void ActivateNextEvent()
        {
            // If the current event Index is within range, and the next event time is less than or equal to the current time
            if (Beatmap.Events.Count > EventIndex && NextEvent.Time <= Time)
            {
                // Start handling the current event, and increase the event index
                NextEvent.Active = true;
                ActiveEvents.Add(NextEvent);
                EventIndex++;

                // Get ready for the next event, if it exists
                if (Beatmap.Events.Count > EventIndex)
                    NextEvent = Beatmap.Events[EventIndex];
            }
        }

        protected virtual void HandleActiveEvents()
        {
            for (int i = 0; i < ActiveEvents.Count; i++)
                // If the event is active, handle it
                if (ActiveEvents[i].Active)
                    ActiveEvents[i].Handle(this);
                // Otherwise, add remove it
                else
                    ActiveEvents.RemoveAt(i--);
        }
        #endregion

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
        public abstract int FindTime(Point mousePosition);
        public abstract int FindClosestColumn(Point mousePos);

        public abstract void AddEvent(Event evnt);

        public abstract void AddEvent(int time, EventType eventType);

        public abstract void AddHitObject(int time, int columnIndex);

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

            selectedItems = new List<Drawable>();
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
        /// <summary>
        /// Returns true if the mouse clicked an object on screen.
        /// </summary>
        /// <param name="mousePos">The position of the mouse (assumed mouse already clicked)</param>
        /// <param name="clicked">If this method returns true, clicked will be changed to the object clicked.</param>
        /// <returns></returns>
        protected abstract bool ClickedAnObject(Point mousePos, out Drawable clicked);

        public void Select(Drawable item)
        {
            selectedItems = new List<Drawable>() { item };
        }

        public void Select(List<Drawable> items)
        {
            selectedItems = items;
        }

        public void AddToSelection(Drawable item)
        {
            selectedItems.Add(item);
        }

        public void AddToSelection(List<Drawable> items)
        {
            selectedItems.AddRange(items);
        }

        public void RemoveFromSelection(Drawable item)
        {
            if (selectedItems.Contains(item))
                selectedItems.Remove(item);
        }

        public void RemoveFromSelection(List<Drawable> items)
        {
            for (int i = 0; i < items.Count; i++)
                RemoveFromSelection(items[i]);
        }
        #endregion

        #region IEventHandleable Methods
        public Beatmap GetCurrentBeatmap()
        {
            return Beatmap;
        }

        public double GetCurrentTime()
        {
            return Time;
        }

        // Depending on the Editor style, there may not be a crosshair
        // for zoom events to handle.
        public abstract bool HasCrosshair();
        public abstract Crosshair GetCrosshair();
        #endregion

        public override void UpdateDiscord() => PulsarcDiscord.SetStatus("", "Editing maps.");
    }
}
