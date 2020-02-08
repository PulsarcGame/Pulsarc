using Pulsarc.Utils.Update;
using System;

namespace Pulsarc
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Checks for updates and applies them if found.
            Updater.CheckForUpdates();

            using (var game = new Pulsarc())
            {
                game.Run();
            }
        }
    }
}
