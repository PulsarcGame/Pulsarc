using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
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

        private Stack<UpdateXML> updates;
        private Queue<string> patchFileLocations = new Queue<string>();

        public bool PatchSucceeded = false;

        public DownloadWorker(Stack<UpdateXML> updates)
        {
            WorkerSupportsCancellation = true;
            DoWork += Work;
            RunWorkerCompleted += OnComplete;

            this.updates = updates;
        }

        private void Work(object sender, DoWorkEventArgs e)
            => HandleUpdateXML(updates.Pop());

        private void HandleUpdateXML(UpdateXML update, bool retry = false)
        {
            string tempFilePath = Path.GetTempFileName();

            try
            {
                using (webClient)
                {
                    webClient.DownloadFile(update.DownloadUri, tempFilePath);
                }
            }
            catch
            {
                // If we already tried once, stop trying.
                if (retry) { return; }

                // Try one more time.
                DeleteFileAndTryAgain();
            }

            // Checksum with md5, try again if there's an issue
            if (!File.Exists(tempFilePath) 
                || Hasher.HashFile(tempFilePath, HashType.MD5) != update.MD5)
            {
                // If we already tried once, stop trying.
                if (retry) { return; }

                // Try one more time
                DeleteFileAndTryAgain();
            }

            // Otherwise add it to the queue of patches
            patchFileLocations.Enqueue(tempFilePath);

            bool anotherOne = updates.TryPop(out UpdateXML nextUpdate);

            // If the Pop was successful, handle it, otherwise we are done and have suceeded!
            if (anotherOne)
            {
                HandleUpdateXML(nextUpdate);
            }

            void DeleteFileAndTryAgain()
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

                HandleUpdateXML(update, true);
            }
        }

        private void OnComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            // If there's no more updates to handle, prepare the patches and cancel.
            if (updates.Count <= 0)
            {
                webClient.Dispose();
                PreparePatches();
                CancelAsync();

                if (PatchSucceeded)
                {
                    Updater.LaunchPathcer();
                }
            }
        }

        private void PreparePatches()
        {
            while (patchFileLocations.Count > 0)
            {
                string oldFile = patchFileLocations.Dequeue();
                string patchFile = Path.ChangeExtension(oldFile, ".zip");

                try
                {
                    File.Copy(oldFile, patchFile);
                }
                catch
                {
                    //Console.WriteLine("Copying Failed");
                    return;
                }

                //Console.WriteLine($"patchFile is dequeud! The path is: {patchFile}" +
                    //$"\nWaiting 333 ms to give some breathing room.");

                Thread.Sleep(333);

                using (FileStream input = File.OpenRead(patchFile))
                using (ZipFile zipFile = new ZipFile(input))
                {
                    //Console.WriteLine("Ready To start moving files!");

                    foreach (ZipEntry entry in zipFile)
                    {
                        // Ignore non-files
                        if (!entry.IsFile) { continue; }

                        // entry.Name includes the full path relative to the .zip file
                        string newPath = Path.Combine(UpdateInfo.DownloadsPath, entry.Name);

                        // Make sure the directory is created.
                        string directoryName = Path.GetDirectoryName(newPath);
                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(directoryName);
                        }

                        // According to SharpZipLib 4KB is optimum
                        byte[] buffer = new byte[4096];

                        // If the downloads folder already has this file, delete it
                        // Queue is organized from oldest to newest, so newer files
                        // will "overwrite" older ones.
                        if (File.Exists(newPath))
                        {
                            //Console.WriteLine($"Deleting {newPath}");

                            File.Delete(newPath);
                        }

                        // Unzip the file in buffered chunks.
                        // According to SharpZipLib this uses less memory than unzipping the whole
                        // thing At once.
                        using (Stream zipStream = zipFile.GetInputStream(entry))
                        using (Stream output = File.Create(newPath))
                        {
                            StreamUtils.Copy(zipStream, output, buffer);

                            if (ShouldHideFile(entry.Name))
                            {
                                File.SetAttributes(newPath,
                                    File.GetAttributes(newPath) | FileAttributes.Hidden);
                            }
                        }
                    }
                }

                // Delete the temp file
                try { File.Delete(oldFile); }
                catch
                {
                    //Console.WriteLine($"DEBUG: Couldn't delete temp file {oldFile}. Ignoring.");
                }

                PatchSucceeded = true;
            }

            bool ShouldHideFile(in string name)
                => hiddenFileTypes.Contains(name.Substring(name.LastIndexOf('.')));
        }

        // For ShouldHideFile() above
        private List<string> hiddenFileTypes = new List<string>()
            { ".dll", ".so", ".dylib", ".config", ".json", };
    }
}
