using Pulsarc.Beatmaps;
using Pulsarc.Beatmaps.Events;
using Pulsarc.UI.Screens.Editor.UI;
using Pulsarc.UI.Screens.Gameplay;
using Pulsarc.Utils.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulsarc.UI.Screens.Editor
{
    /// <summary>
    /// Singleton class for Editor of differing styles to utilize.
    /// </summary>
    public static class Editor
    {
        public static bool Active { get; internal set; } = false;

        // Whether time is paused. This doesn't prevent scrolling through time.
        // Set this to true to pause the audio if it's playing
        // Set this to false to resume the audio if it's paused
        public static bool Paused
        {
            get => AudioManager.Paused;
            set
            {
                if (!value)
                {
                    AudioManager.Resume();
                }
                else
                {
                    AudioManager.Pause();
                }
            }
        }

        // The currently selected rate
        public static float Rate
        {
            get => AudioManager.AudioRate;
            set => AudioManager.AudioRate = value;
        }

        // The current time
        public static double Time => AudioManager.GetTime();

        // The current beatmap we are editing
        internal static Beatmap WorkingBeatmap;

        internal static Crosshair Crosshair;

        // A list of objects that were copy or cut onto the clipboard
        // TODO: Make it copy to the System Clipboard too. Could use the ToString() of
        // each object so it matches the .psc format.
        internal static List<Drawable> EditorClipboard = new List<Drawable>();

        // The currently selected items in the editor.
        internal static List<Drawable> SelectedItems = new List<Drawable>();

        // A collection of each action that has happened.
        // Used for Undo/Redo functions.
        internal static List<EditorAction> Actions = new List<EditorAction>();

        // The current index in actions.
        internal static int _actionIndex;
        internal static int ActionIndex
        {
            get => _actionIndex;

            // If the value is outside the range of possible indexes,
            // change it to minimum or maximum values
            set => _actionIndex = value < 0 ? 0
                    : value >= Actions.Count
                    ? Actions.Count - 1 : value;
        }

        // The current action state, determined by the ActionIndex
        internal static List<EditorAction> CurrentActionState
            => Actions.GetRange(0, Actions.Count - 1 - ActionIndex);

        // Probably would need to be tracked even for non-ACE editors.
        internal static double CurrentZoomLevel;

        // The current scale
        // The lower the scale, the closer the arcs are from each other
        // The higher the scale, the farther the arcs are from each other
        internal static float Scale;

        // Current interval for Beat Snapping (1/1, 1/2, 1/3, 1/4, etc.)
        internal static Beat BeatSnapInterval = Beat.Whole;

        // Whether added notes should snap to the closest BeatDisplay or not.
        internal static bool BeatLocked = true;

        // Whether the Editor is in the state to add HitObjects with a mouse click.
        internal static bool CanAddHitObjects = true;

        internal static int LastScrollValue = 0;

        internal static void Init(Beatmap beatmap)
        {
            WorkingBeatmap = beatmap;
        }

        internal static void Close()
        {
            // Make everything null
            WorkingBeatmap = null;
        }
        internal static void Update()
        {
            CalculateLastFrameData();
        }

        internal static bool SameAsLastFrame(double time, double scale, double crosshairZLoc)
        {
            // The conditionals we're looking at
            bool timeSame = time == lastFrameTime;
            bool scaleSame = scale == lastFrameScale;
            bool crosshairZLocSame = crosshairZLoc == lastFrameCrosshairZLoc;

            // If all our conditionals were true, then this frame is the same.
            // If at least one of our conditionals were false, this frame is different.
            return timeSame && scaleSame && crosshairZLocSame;
        }

        private static void CalculateLastFrameData()
        {
            lastFrameTime = Time;
            lastFrameScale = Scale;
            lastFrameCrosshairZLoc = Crosshair.GetZLocation();
        }

        private static double lastFrameTime = 0;
        private static double lastFrameScale = 0;
        private static double lastFrameCrosshairZLoc = 0;
    }

    public enum EditorStyle
    {
        // Gameplay-style
        ACEStyle,
        // Mania-style
        //TrackStyle,
    }
}
