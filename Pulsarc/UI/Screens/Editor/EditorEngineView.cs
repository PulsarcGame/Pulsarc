using Microsoft.Xna.Framework;
using Pulsarc.UI.Common;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Editor
{
    abstract class EditorEngineView : ScreenView
    {
        private Background background;

        private EditorEngine GetEditorEngine() { return (EditorEngine)Screen; }

        public EditorEngineView(Screen screen) : base(screen) { }

        public void Init()
        {
            background = GetEditorEngine().Background;
        }

        public override void Draw(GameTime gameTime)
        {
            if (background.Dimmed && background.DimTexture.Opacity != 1f || !background.Dimmed)
                background.Draw();

            DrawEditorHitObjects();
        }

        private void DrawEditorHitObjects()
        {
            // Go through each key
            for (int i = 0; i < GetEditorEngine().KeyCount; i++)
            {
                // Go through the arcs in each column
                for (int k = 0; k < GetEditorEngine().Columns[i].UpdateHitObjects.Count; k++)
                {
                    // If the arc is on screen, draw it.
                    if (GetEditorEngine().Columns[i].UpdateHitObjects[k].Value.IsSeen())
                        GetEditorEngine().Columns[i].UpdateHitObjects[k].Value.Draw();

                    // If the arc is inside the "IgnoreTime" window, stop bothering to
                    // look at the rest of the arcs in this column.
                    if (GetEditorEngine().Columns[i].UpdateHitObjects[k].Key - GetEditorEngine().IgnoreTime > GetEditorEngine().Time)
                        break;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Destroy()
        {
            
        }
    }
}
