using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
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
        private Queue<string> patchFileLocations;

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
            if (!File.Exists(tempFilePath) 
                || Hasher.HashFile(tempFilePath, HashType.MD5) != update.MD5)
            {
                // If we already tried once, stop trying.
                if (retry) { return Result.HashFailed; }

                // Try one more time
                return DeleteFileAndTryAgain();
            }

            // Otherwise add it to the queue of patches
            patchFileLocations.Enqueue(tempFilePath);

            UpdateXML nextUpdate;
            bool anotherOne = updates.TryPop(out nextUpdate);
            // If the Pop was successful, handle it, otherwise we are done and have suceeded!
            return anotherOne ? HandleUpdateXML(nextUpdate) : Result.DownloadSucceeded;

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
            // If there's no more updates to handle, prepare the patches and cancel.
            if (updates.Count <= 0)
            {
                webClient.Dispose();
                PreparePatches();
                CancelAsync();
            }
        }

        private void PreparePatches()
        {
            while (patchFileLocations.Count > 0)
            {
                string patchFile = patchFileLocations.Dequeue();
                using (FileStream input = File.OpenRead(patchFile))
                using (ZipFile zipFile = new ZipFile(input))
                {
                    foreach (ZipEntry entry in zipFile)
                    {
                        // Ignore non-files
                        if (!entry.IsFile) { continue; }

                        // entry.Name includes the full path relative to the .zip file
                        string newPath = Path.Combine( UpdateInfo.GetPathToAssembly() + "Downloads/",
                                                       entry.Name );

                        // Make sure the directory is created.
                        string directoryName = Path.GetDirectoryName(newPath);
                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(directoryName);
                        }

                        // According to SharpZipLib 4KB is optimum
                        byte[] buffer = new byte[4096];

                        // Unzip the file in buffered chunks.
                        // According to SharpZipLib this uses less memory than unzipping the whole
                        // thing At once.
                        using (Stream zipStream = zipFile.GetInputStream(entry))
                        using (Stream output = File.Create(newPath))
                        {
                            // Queue is organized from oldest to newest so newer files will
                            // overwrite older files.
                            StreamUtils.Copy(zipStream, output, buffer);

                            if (ShouldHideFile(entry.Name))
                            {
                                File.SetAttributes(newPath,
                                    File.GetAttributes(newPath) | FileAttributes.Hidden);
                            }
                        }
                    }
                }

                // Delete the original file
                try
                {
                    File.Delete(patchFile);
                }
                catch
                {
                    System.Console.WriteLine($"DEBUG: Couldn't delete temp file {patchFile}, ignoring.");
                }
            }

            // At this point, all the patch files should be in Downloads, the patcher program is ready
            // To go.
            Updater.LaunchPathcer();

            bool ShouldHideFile(in string name)
                => hiddenFileTypes.Contains(name.Substring(name.LastIndexOf('.')));
        }

        // For ShouldHideFile() above
        private List<string> hiddenFileTypes = new List<string>()
            { ".dll", ".so", ".dylib", ".config", ".json", };
    }
}
