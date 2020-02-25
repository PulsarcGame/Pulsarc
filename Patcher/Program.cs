using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Patcher
{
    class Program
    {
        //static bool oof = false;

        static void Main()
        {
            // First check makes sure we're in the same directory as Pulsarc.exe, MoveDownloads()
            // Handles moving the files as well. If the move was successful, launch Pulsarc.
            if (File.Exists("Pulsarc.exe") && MoveDownloads())
            {
                // Wait some time, then launch Pulsarc.
                Thread.Sleep(333);

                Process.Start("Pulsarc.exe");
            }
            /*
            else
            {
                if (!oof)
                    Console.WriteLine("I can't find Pulsarc lol!");
            }*/
        }

        /// <summary>
        /// Moves all the files from the Downloads directory to the Pulsarc directory.
        /// </summary>
        /// <returns>Returns true if there was no files or all files were moved
        /// without error. Returns false if an error happens.</returns>
        private static bool MoveDownloads()
        {
            //oof = true;

            string[] files = Directory.GetFiles("Downloads");

            foreach (string file in files)
            {
                if (file.Contains("Patcher.exe")) { continue; }

                try
                {
                    // If downloads had an update for *every* file in Pulsarc, Downloads
                    // Would look like a new pulsarc directory, finding the relative path
                    // from Downloads should work to place files in the actual pulsarc directory.
                    string relativePath = Path.GetRelativePath("Downloads", file);

                    //Console.WriteLine($"Moving {file} to {relativePath}");
                        
                    // Delete the original file to overwrite it
                    if (File.Exists(relativePath))
                    {
                        //Console.WriteLine($"Deleting File {relativePath}");

                        File.Delete(relativePath);
                    }

                    File.Move(file, relativePath);
                    //Console.WriteLine($"{file} was moved to {relativePath}");
                }
                catch //(Exception e)
                {
                    //Console.WriteLine(e);

                    //Console.WriteLine($"Something went wrong with moving {file}");
                    return false;
                }
            }

            return true;
        }
    }
}
