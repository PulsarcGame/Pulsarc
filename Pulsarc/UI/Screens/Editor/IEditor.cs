using Pulsarc.Beatmaps;
using Pulsarc.Beatmaps.Events;
using Pulsarc.UI.Screens.BaseEngine;
using System.Collections.Generic;

namespace Pulsarc.UI.Screens.Editor
{
    public interface IEditor : IEventHandleable
    {
        // These methods may not be needed.
        void SetFirstOffset(TimingPoint timingPoint);
        void SetFirstOffset(int time, double bpm = 120);

        #region Time Navigation Methods
        void SetTime(double time);
        void ScrollTime(double delta);

        void Resume();
        void Pause();
        #endregion

        #region Add Item methods
        void AddTimingPoint(TimingPoint timingPoint);
        void AddTimingPoint(int time, double bpm);

        void AddEvent(Event evnt);
        void AddEvent(int time, EventType eventType);

        void AddZoomEvent(ZoomEvent zoomEvent);
        void AddZoomEvent(int time, ZoomType zoomType, float zoomLevel, int endTime);

        void AddHitObject(int time, int columnIndex);
        #endregion

        #region Shortcutable Methods
        void DeleteSelection();

        void Copy();
        void Cut();
        void Paste(int time);
        void Undo();
        void Redo();

        void Save();
        void SaveAs();
        void Open();
        #endregion

        #region Selection Methods
        void Select(Drawable item);
        void Select(List<Drawable> items);

        void AddToSelection(Drawable item);
        void AddToSelection(List<Drawable> items);
        
        void RemoveFromSelection(Drawable item);
        void RemoveFromSelection(List<Drawable> items);

        //void DragSelected();
        #endregion

        EditorStyle GetEditorStyle();
    }
}
