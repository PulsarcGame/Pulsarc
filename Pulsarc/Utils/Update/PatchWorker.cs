using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace Pulsarc.Utils.Update
{
    internal class PatchWorker : BackgroundWorker
    {
        private Queue<string> filePaths;

        private bool started = false;

        public bool ReadyToStart => !started && filePaths.Count > 0;

        public bool AddingToQueue = true;

        public PatchWorker()
        {
            WorkerSupportsCancellation = true;
            DoWork += Work;
            RunWorkerCompleted += OnComplete;
        }

        private void Work(object sender, DoWorkEventArgs e)
        {
            started = true;

            while(AddingToQueue)
            {
                // If there's currently nothing in queue, wait a bit then check again.
                if (filePaths.Count <= 0)
                {
                    Thread.Sleep(300);
                    continue;
                }

                e.Result = Install(filePaths.Dequeue());

                if ((Result)e.Result != Result.SinglePatchSucceeded) { return; }
            }

            e.Result = Result.AllPatchesSucceeded;
        }
        
        private Result Install(in string zipFilePath)
        {
            using (FileStream input = File.OpenRead(zipFilePath))
            using (ZipFile zipFile = new ZipFile(input))
            {
                foreach (ZipEntry entry in zipFile)
                {
                    // Ignore uneeded entries
                    if (!KeepEntry(entry)) { continue; }

                    // entry.Name includes the full path relative to the .zip file
                    // Add that path ontop of Pulsarc's directory to move the files accordingly.
                    string newPath = Path.Combine(UpdateInfo.GetPathToAssembly(), entry.Name);

                    // Create a new directory if needed.
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
                        StreamUtils.Copy(zipStream, output, buffer);

                        if (ShouldHideFile(entry.Name))
                        {
                            File.SetAttributes(newPath,
                                File.GetAttributes(newPath) | FileAttributes.Hidden);
                        }
                    }
                }
            }

            DeleteExtraFile(zipFilePath);
            return Result.SinglePatchSucceeded;

            // If the entry is a file without the .pdb extension and is good for the
            // current platform, return true.
            bool KeepEntry(ZipEntry entry)
                => entry.IsFile && !entry.Name.Contains(".pdb") && ForRightPlatform(entry.Name);

            bool ForRightPlatform(in string name)
            {
                if (UpdateInfo.IsOnWindows)
                {
                    // Windows doesn't use .dylibs or .sos
                    if (name.Contains(".dylib") || name.Contains(".so"))
                    {
                        return false;
                    }
                }
                else if (UpdateInfo.IsOnMac)
                {
                    // Mac doesn't use .dlls
                    // Might not use .sos either
                    if (name.Contains(".dll"))
                    {
                        return false;
                    }
                }
                else if (UpdateInfo.IsOnLinux)
                {
                    // Linux doesn't use .dlls or .dylibs
                    if (name.Contains(".dll") || name.Contains(".dylib"))
                    {
                        return false;
                    }
                }

                return true;
            }

            bool ShouldHideFile(in string name)
                => hiddenFileTypes.Contains(name.Substring(name.LastIndexOf('.')));
        }

        // For ShouldHideFile() above
        private List<string> hiddenFileTypes = new List<string>()
            { ".dll", ".so", ".dylib", ".config", ".json", };

        private void DeleteExtraFile(in string path)
        {
            // Try to delete the files twice.
            try { File.Delete(path); }
            catch
            {
                try { File.Delete(path); }
                catch { }
            }
        }

        private void OnComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            bool errorHappened = (Result)e.Result != Result.AllPatchesSucceeded;

            if (errorHappened || (!AddingToQueue && filePaths.Count <= 0))
            {
                CancelAsync();
            }
            // If there's a mistake somehow, restart to finish the rest of the queue
            else
            {
                started = false;
            }
        }

        public void Enqueue(string path) => filePaths.Enqueue(path);
    }
}
