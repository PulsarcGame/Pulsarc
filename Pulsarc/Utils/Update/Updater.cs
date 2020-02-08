using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Pulsarc.Utils.Update
{
    public static class Updater
    {
        public static void CheckForUpdates()
        {
            UpdateXML updateXML;

            if (!UpdateAvailable(out updateXML)) { return; }

            ApplyUpdates(FindNeededUpdates(updateXML));
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
                UpdateXML lastUpdate;

                // Look at the previous version provided by the current UpdateXML.
                // If the previous version is not greater than this client's version, or parsing
                // failed, break
                if (!UpdateExistsAndIsNewerThanClient(out lastUpdate,
                    UpdateInfo.GetPreviousVersionXMLPath(currentUpdate.PreviousVersion)))
                { break; }

                // Otherwise, add it to the stack
                updateStack.Push(lastUpdate);

                // Prepare for next loop
                currentUpdate = lastUpdate;
            }

            return updateStack;
        }

        private static void ApplyUpdates(Stack<UpdateXML> updates)
        {

            using (DownloadWorker downloader = new DownloadWorker(updates))
            using (PatchWorker patcher = new PatchWorker())
            {
                // We have data we want handled by downloader that we will sent to installer, this loop
                // Lets those two workers work together but in async until both are done.
                while (!downloader.CancellationPending || !patcher.CancellationPending)
                {
                    // Downloader will do work when its ready to start again.
                    if (downloader.ReadyForAnother)
                    {
                        downloader.RunWorkerAsync();
                    }
                    // If the worker is ready to give us the TempFile details enqueue them
                    // to the installer worker's queue.
                    else if (downloader.PathWaitingToBeGot)
                    {
                        patcher.Enqueue(downloader.DownloadedFilePath);
                    }
                    // When downloader is done, let installer know it doesn't need to wait for anymore
                    // files
                    else if (downloader.CancellationPending)
                    {
                        patcher.AddingToQueue = false;
                    }

                    // Wait until at least thing has been queued before starting.
                    if (patcher.ReadyToStart)
                    {
                        patcher.RunWorkerAsync();
                    }

                    // Sleep for a bit so we aren't looping this a shitton.
                    Thread.Sleep(300);
                }
            }
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
