using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Pulsarc.Utils.Update
{
    public static class Updater
    {
        public static void CheckForUpdates()
        {
            if (!UpdateExistsAndIsNewerThanClient(out UpdateXML updateXML, UpdateInfo.ServerDownloadPath))
            { return; }

            DownloadUpdates(FindNeededUpdates(updateXML));
        }

        private static bool UpdateExistsAndIsNewerThanClient(out UpdateXML updateXML, string xmlPath)
        {
            // updateXML is expected to be changed.
            updateXML = null;

            //Console.WriteLine("Checking if updateXML location exists.");

            // If the xml doesn't exist on the server, return false.
            Uri xmlLocation = new Uri(xmlPath);
            if (!UpdateXML.Exists(xmlLocation)) { return false; }

            //Console.WriteLine("It exists!");

            // If it does exist, assign updateXML to it and compare to the client version.
            updateXML = UpdateXML.Parse(xmlLocation);
            return updateXML != null && updateXML.PreviousVersion >= Pulsarc.CurrentVersion;
        }

        private static Stack<UpdateXML> FindNeededUpdates(UpdateXML firstUpdateToStack)
        {
            //Console.WriteLine("Finding what updates to download");

            Stack<UpdateXML> updateStack = new Stack<UpdateXML>();
            updateStack.Push(firstUpdateToStack);

            UpdateXML currentUpdate = firstUpdateToStack;
            while (true)
            {
                // Look at the previous version provided by the current UpdateXML.
                // If the previous version is not greater than this client's version, or parsing
                // failed, stop stacking and get ready for updating.
                if (!UpdateExistsAndIsNewerThanClient(out UpdateXML lastUpdate,
                    UpdateInfo.GetPreviousVersionXMLPath(currentUpdate.PreviousVersion)))
                    { break; }

                // Otherwise, add it to the stack
                updateStack.Push(lastUpdate);

                // Prepare for next loop
                currentUpdate = lastUpdate;
            }

            return updateStack;
        }

        private static void DownloadUpdates(Stack<UpdateXML> updates)
        {
            //Console.WriteLine("Downloading Updates");

            bool success = false;

            using (DownloadWorker downloader = new DownloadWorker(updates))
            {
                downloader.RunWorkerAsync();
                success = downloader.PatchSucceeded;
            }

            if (success)
            {
                LaunchPathcer();
            }
        }

        internal static void LaunchPathcer()
        {
            bool patcherChecksumPassed = true; // TODO: checksum Patcher.exe

            //Console.WriteLine("Seeing if Patcher exists.");

            // If Patcher.exe is not the correct version of patcher, download it
            if (!patcherChecksumPassed || !File.Exists("Downloads/Patcher.exe"))
            {
                // TODO: Download the newest version of patcher.exe
            }

            //Console.WriteLine("It does! shutting down");

            // Launch Patcher
            Process.Start("Downloads/Patcher.exe");

            // Quit Pulsarc so Patcher can do shit
            Environment.Exit(0);
        }
    }
}
