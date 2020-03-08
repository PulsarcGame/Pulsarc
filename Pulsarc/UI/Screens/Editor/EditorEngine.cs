using Pulsarc.UI.Screens.Gameplay;
using System.Collections.Generic;

namespace Pulsarc.UI.Screens.Editor
{
    public class EditorEngine : GameplayEngine
    {
        private EditorEngineView GetEditorView() => (EditorEngineView)View;
    }
}
