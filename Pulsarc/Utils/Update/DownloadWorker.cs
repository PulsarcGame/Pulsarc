using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;

namespace Pulsarc.Utils.Update
{
    internal class DownloadWorker : BackgroundWorker
    {
        private WebClient webClient = new WebClient();

        private string downloadedFilePath = "";
        public string DownloadedFilePath
        {
            // Reset this when we get it to signal we're ready to download another.
            get
            {
                string toReturn = downloadedFilePath;
                downloadedFilePath = "";
                PathWaitingToBeGot = false;
                return toReturn;
            }
            // Set PathWaitingToBeGot to true to let others know we're ready to be got.
            private set
            {
                if (!PathWaitingToBeGot)
                {
                    downloadedFilePath = value;
                    PathWaitingToBeGot = true;
                }
            }
        }

        private Stack<UpdateXML> updates;

        public bool ReadyForAnother => !CancellationPending
                                        && !PathWaitingToBeGot
                                        && !IsBusy;

        public bool PathWaitingToBeGot { get; private set; } = false;

        public DownloadWorker(Stack<UpdateXML> updates)
        {
            WorkerSupportsCancellation = true;
            DoWork += Work;
            RunWorkerCompleted += OnComplete;

            this.updates = updates;
        }

        private void Work(object sender, DoWorkEventArgs e) => e.Result = HandleUpdateXML(updates.Pop());

        private Result HandleUpdateXML(UpdateXML update, bool retry = false)
        {
            string tempFilePath = Path.GetRandomFileName();
            using (FileStream stream = File.Create(Path.Combine(Path.GetTempPath(), $"{tempFilePath}.zip")))
            {
                tempFilePath = stream.Name;
            }

            try
            {
                using (webClient)
                {
                    // Not using Async since we're already in a worker thread, and we need to wait
                    // for the file to finish before continuing.
                    webClient.DownloadFile(update.DownloadUri, tempFilePath);
                }
            }
            catch
            {
                // If we already tried once, stop trying.
                if (retry) { return Result.DownloadFailed; }

                // Try one more time.
                return DeleteFileAndTryAgain();
            }

            // Checksum with md5, try again if there's an issue
            if (Hasher.HashFile(tempFilePath, HashType.MD5) != update.MD5 || !File.Exists(tempFilePath))
            {
                // If we already tried once, stop trying.
                if (retry) { return Result.HashFailed; }

                // Try one more time
                return DeleteFileAndTryAgain();
            }

            DownloadedFilePath = tempFilePath;

            return Result.DownloadSucceeded;

            Result DeleteFileAndTryAgain()
            {
                // Give everything a break
                Thread.Sleep(333);

                // Try to delete the temp file twice
                try { File.Delete(tempFilePath); }
                catch
                {
                    try { File.Delete(tempFilePath); }
                    catch { }
                }

                return HandleUpdateXML(update, true);
            }
        }

        private void OnComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            // If there's no more updates to handle, cancel.
            if (updates.Count <= 0)
            {
                webClient.Dispose();
                CancelAsync();
            }
        }
    }
}
