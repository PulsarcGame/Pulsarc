using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;

namespace Pulsarc.Utils.Update
{
    internal static class UpdateInfo
    {
        // Windows
        public static bool IsOnWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool Is64Bit => IsOnWindows && Environment.Is64BitProcess;
        private const string WINDOWS_FILE_EXTENSION = ".exe";

        // Mac / OSX
        public static bool IsOnMac => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        private const string MAC_FILE_EXTENSION = "";

        // Linux
        public static bool IsOnLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        private const string LINUX_FILE_EXTENSION = "TODO";

        // Invalid Platform Exception
        private static Exception invalidPlat = new Exception("Invalid Platform");

        public static string ExecutableExtension =>  IsOnWindows ? WINDOWS_FILE_EXTENSION
                                                   : IsOnMac ? MAC_FILE_EXTENSION
                                                   : IsOnLinux ? LINUX_FILE_EXTENSION
                                                   : throw invalidPlat;

        public static string Platform = IsOnWindows ?
                                            Is64Bit ? "win64" : "win32"
                                      : IsOnMac ? "osx"
                                      : IsOnLinux ? "linux"
                                      : throw invalidPlat;

        // The address to the xml file for installation
        private const string RELEASE_PATH = "https://pulsarc.net/Releases/";

        // The full path to the xml file for installation. Formatted as so:
        // "https://pulsarc.net/Releases/CurrentVersion-{platform}.xml"
        public static string ServerDownloadPath => $"{RELEASE_PATH}CurrentVersion-{Platform}.xml";

        /// <summary>
        /// Sees if a Pulsarc executable is in the path provided.
        /// </summary>
        /// <param name="pathToCheck">The path to check for Pulsarc.</param>
        /// <returns></returns>
        public static bool PulsarcExistsIn(string pathToCheck)
            => File.Exists($"{pathToCheck}/Pulsarc{ExecutableExtension}");

        /// <summary>
        /// Determine whether or not the Pulsarc Directory Exists.
        /// </summary>
        /// <returns>True if Pulsarc has been installed and there is a directory.
        /// or False if it can't be found.</returns>
        public static bool PulsarcExists() => PulsarcExistsIn(GetPathToAssembly());

        /// <summary>
        /// Get the folder of the currently executing assembly.
        /// </summary>
        /// <returns></returns>
        public static string GetPathToAssembly()
            => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)
                // Change windows' "\" to "/" for consistent format.
                .Replace("\\", "/");

        /// <summary>
        /// Finds what the xml file of the provided version would be called if hosted on pulsarc.net.
        /// The format is "https://pulsarc.net/Release/Version-{version}-{platform}.xml"
        /// </summary>
        /// <param name="version">The game version to find the xml for.</param>
        /// <returns></returns>
        internal static string GetPreviousVersionXMLPath(Version version)
        {
            string ver = version.ToString();
            ver = ver.Substring(0, ver.LastIndexOf('.'));

            return $"{RELEASE_PATH}Version-{ver}-{Platform}.xml";
        }
    }
}
