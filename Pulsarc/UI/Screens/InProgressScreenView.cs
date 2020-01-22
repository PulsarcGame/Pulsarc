using Microsoft.Xna.Framework;
using Wobble.Screens;

namespace Pulsarc.UI.Screens
{
    /// <summary>
    /// A placeholder ScreenView for not-yet-implemented screens.
    /// </summary>
    class InProgressScreenView : ScreenView
    {
        public InProgressScreenView(InProgressScreen screen) : base(screen)
        { }

        public override void Destroy()
        { }

        public override void Draw(GameTime gameTime)
        { }

        public override void Update(GameTime gameTime)
        { }
    }
}
