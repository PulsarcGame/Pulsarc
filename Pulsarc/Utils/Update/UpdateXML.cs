using System;
using System.Net;
using System.Xml;

namespace Pulsarc.Utils.Update
{
    internal class UpdateXML
    {
        public Version PreviousVersion { get; private set; }

        public Uri DownloadUri { get; private set; }

        public string MD5 { get; private set; }

        /// <summary>
        /// Represents the data found in the xml update file on the Pulsarc Server.
        /// Check assets/template.xml for reference.
        /// </summary>
        /// <param name="previousversion">The version of this update.</param>
        /// <param name="download">The link to the full download of this version. Mostly used
        /// by the updater portion. </param>
        /// <param name="md5">Used to checksum data integrity.</param>
        public UpdateXML(Version previousversion, Uri download, string md5)
        {
            PreviousVersion = previousversion;
            DownloadUri = download;
            MD5 = md5;
        }

        /// <summary>
        /// Checks to see if the provided URI is accessible.
        /// </summary>
        /// <param name="location">The URI to check.</param>
        /// <returns>True if we get an "OK" response from the URI. False in any other case.</returns>
        public static bool Exists(Uri location)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(location.AbsoluteUri);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                bool clear = response.StatusCode == HttpStatusCode.OK;

                response.Close();

                return clear;
            }
            catch { return false; }
        }

        /// <summary>
        /// Create an UpdateXML object by parsing the data found in the .xml file from the
        /// provided URI.
        /// </summary>
        /// <param name="location">The location of the .xml file to parse.</param>
        /// <returns>An UpdateXML object corresponding to the data parsed in the provided .xml.
        /// Null if there's an error or issue during parsing.</returns>
        public static UpdateXML Parse(Uri location)
        {
            Version previousVersion;
            string downloadURL, md5;

            try
            {
                // Load XML from the location provided
                XmlDocument doc = new XmlDocument();
                doc.Load(location.AbsoluteUri);

                // TODO: Change the .xml template to be more readable and update Installer
                // To use it.
                XmlNode node = doc.SelectSingleNode($"//install[@appId='Pulsarc']");

                if (node == null)
                {
                    return null;
                }

                // Get all the data from the root node
                previousVersion = Version.Parse(node["patch"]["previousVersion"].InnerText);
                downloadURL = node["patch"]["download"].InnerText;
                md5 = node["patch"]["md5"].InnerText;

                // Use that data to make and return a new UpdateXML
                return new UpdateXML(previousVersion, new Uri(downloadURL), md5);
            }
            catch //(Exception e)
            {
                //Console.WriteLine(e);

                return null;
            }
        }
    }
}
