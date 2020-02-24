using System;
using System.Diagnostics;
using System.IO;

namespace Patcher
{
    class Program
    {
        static void Main()
        {
            // First check makes sure we're in the same directory as Pulsarc.exe
            if (File.Exists("Pulsarc.exe") && MoveDownloads())
            {
                Process.Start("Pulsarc.exe");
            }
            /*
            else
            {
                Console.WriteLine("Something went wrong.");
            }

            Console.ReadLine();*/
        }

        /// <summary>
        /// Moves all the files from the Downloads directory to the Pulsarc directory.
        /// </summary>
        /// <returns>Returns true if there was no files or all files were moved
        /// without error. Returns false if an error happens.</returns>
        private static bool MoveDownloads()
        {
            string[] files = Directory.GetFiles("Downloads");

            foreach (string file in files)
            {
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
                        File.Delete(relativePath);
                    }

                    File.Move(file, relativePath);
                    //Console.WriteLine($"{file} was moved to {relativePath}");
                }
                catch
                {
                    //Console.WriteLine($"Something went wrong with moving {file}");
                    return false;
                }
            }

            return true;
        }
    }
}
