using System;
using System.Diagnostics;
using System.IO;

namespace Patcher
{
    class Program
    {
        static void Main()
        {
            if (File.Exists("Pulsarc.exe") && MoveDownloads())
            {
                Process.Start("Pulsarc.exe");
            }
        }

        private static bool MoveDownloads()
        {
            string[] files = Directory.GetFiles("Downloads");

            bool allFilesMoveSuccessful = true;

            foreach (string file in files)
            {
                try
                {
                    string relativePath = Path.GetRelativePath("Downloads", file);

                    // Ensure that the target doesn't exist
                    if (File.Exists(relativePath))
                    {
                        File.Delete(relativePath);
                    }

                    File.Move(file, relativePath);
                    Console.WriteLine($"{file} was moved to {relativePath}");
                }
                catch
                {
                    Console.WriteLine("Well, something went wrong.");
                    //allFilesMoveSuccessful = false;
                }
            }

            return allFilesMoveSuccessful;
        }
    }
}
