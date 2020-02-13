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
            if (!UpdateAvailable(out UpdateXML updateXML)) { return; }

            DownloadUpdates(FindNeededUpdates(updateXML));
        }

        private static bool UpdateAvailable(out UpdateXML updateXML)
            => UpdateExistsAndIsNewerThanClient(out updateXML, UpdateInfo.ServerDownloadPath);

        private static bool UpdateExistsAndIsNewerThanClient(out UpdateXML updateXML, string xmlPath)
        {
            // updateXML is expected to be changed.
            updateXML = null;

            // If the xml doesn't exist on the server, return false.
            Uri xmlLocation = new Uri(xmlPath);
            if (!UpdateXML.Exists(xmlLocation)) { return false; }

            // If it does exist, assign updateXML to it and compare to the client version.
            updateXML = UpdateXML.Parse(xmlLocation);
            return updateXML != null && updateXML.PreviousVersion >= Pulsarc.CurrentVersion;
        }

        private static Stack<UpdateXML> FindNeededUpdates(UpdateXML firstUpdateToStack)
        {
            Stack<UpdateXML> updateStack = new Stack<UpdateXML>();
            updateStack.Push(firstUpdateToStack);

            UpdateXML currentUpdate = firstUpdateToStack;
            while (true)
            {
                // Look at the previous version provided by the current UpdateXML.
                // If the previous version is not greater than this client's version, or parsing
                // failed, break
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
            using (DownloadWorker downloader = new DownloadWorker(updates))
            {
                downloader.RunWorkerAsync();
            }
        }

        internal static void LaunchPathcer()
        {
            bool checksumPassed = true; // TODO: checksum Patcher.exe

            // If Patcher.exe is not the correct version of patcher, download it
            if (!checksumPassed || !File.Exists("Patcher.exe"))
            {
                // TODO: Download the newest version of patcher.exe
            }

            // Launch Patcher
            Process.Start("Patcher.exe");

            // Quit Pulsarc so Patcher can do shit
            Environment.Exit(0);
        }
    }

    internal enum Result
    {
        DownloadFailed,
        HashFailed,
        DownloadSucceeded,
        CopyingFailed,
        SinglePatchSucceeded,
        AllPatchesSucceeded
    }
}
