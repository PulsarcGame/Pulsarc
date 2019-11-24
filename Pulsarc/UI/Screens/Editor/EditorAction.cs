using System.Collections.Generic;

namespace Pulsarc.UI.Screens.Editor
{
    public class EditorAction
    {
        // The states of the items affected during this change before and after.
        public List<Drawable> OriginalStates { get; private set; } = new List<Drawable>();
        public List<Drawable> ChangedStates { get; private set; } = new List<Drawable>();

        public EditorAction(List<Drawable> added, List<Drawable> removed)
        {
            OriginalStates = added;
            ChangedStates = removed;
        }

        public EditorAction(List<Drawable> items, bool added)
        {
            // OriginalStates is a blank list, meaning these items were added.
            if (added)
                ChangedStates = items;
            // ChangedStates is a blank list, meaning these items were removed.
            else
                OriginalStates = items;
        }

        /// <summary>
        /// When we undo, we revert the states of items to what they were
        /// Before this EditorAction. These are stored in OriginalStates
        /// </summary>
        /// <returns></returns>
        public List<Drawable> Undo()
        {
            return OriginalStates;
        }

        /// <summary>
        /// When we redo, we reapply the states of items determined by this
        /// EditorAction. These are stored in ChangedStates.
        /// </summary>
        /// <returns></returns>
        public List<Drawable> Redo()
        {
            return ChangedStates;
        }
    }
}
