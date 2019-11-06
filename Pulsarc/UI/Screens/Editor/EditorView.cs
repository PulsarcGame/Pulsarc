using Microsoft.Xna.Framework;
using Wobble.Screens;

namespace Pulsarc.UI.Screens.Editor
{
    abstract class EditorView : ScreenView
    {
        private EditorEngine GetEditorEngine() { return (EditorEngine)Screen; }

        public EditorView(Screen screen) : base(screen) { }

        public override void Draw(GameTime gameTime)
        {
            throw new System.NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            throw new System.NotImplementedException();
        }

        public override void Destroy()
        {
            
        }
    }
}
