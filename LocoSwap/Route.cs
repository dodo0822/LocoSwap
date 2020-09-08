using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace LocoSwap
{
    public class Route
    {
        private XDocument RouteProperties;
        public string Id { get; set; }
        public string Name { get; set; } = "Name not available";
        public string RouteDirectory
        {
            get
            {
                return GetRouteDirectory(Id);
            }
        }

        public Route()
        {
            Id = "";
            Name = "";
        }

        public Route(string id)
        {
            Load(id);
        }

        public void Load(string id)
        {
            Id = id;
            string xmlPath = Path.Combine(RouteDirectory, "RouteProperties.xml");
            if (!File.Exists(xmlPath))
            {
                bool found = false;
                xmlPath = Path.Combine(Utilities.GetTempDir(), "RouteProperties.xml");
                Utilities.RemoveFile(xmlPath);
                string[] apFiles = Directory.GetFiles(RouteDirectory);
                foreach (string apPath in apFiles)
                {
                    try
                    {
                        var zipFile = ZipFile.Read(apPath);
                        var apEntry = zipFile.Where(entry => entry.FileName == "RouteProperties.xml").FirstOrDefault();
                        if (apEntry == null) continue;
                        apEntry.Extract(Utilities.GetTempDir());
                        found = true;
                        break;
                    }
                    catch (Exception)
                    {

                    }
                }
                if (!found) throw new Exception("RouteProperties.xml not found for this route ID");
            }
            RouteProperties = XmlDocumentLoader.Load(xmlPath);

            XElement displayName = RouteProperties.XPathSelectElement("/cRouteProperties/DisplayName/Localisation-cUserLocalisedString");
            Name = Utilities.DetermineDisplayName(displayName);
        }

        public static string GetRoutesDirectory()
        {
            return Path.Combine(Properties.Settings.Default.TsPath, "Content", "Routes");
        }

        public static string GetRouteDirectory(string routeId)
        {
            return Path.Combine(Properties.Settings.Default.TsPath, "Content\\Routes", routeId);
        }

        public static string[] ListAllRoutes()
        {
            List<string> ret = new List<string>();
            var routeDirectories = Directory.GetDirectories(GetRoutesDirectory());
            foreach (var directory in routeDirectories)
            {
                string id = new DirectoryInfo(directory).Name;
                string xmlPath = Path.Combine(directory, "RouteProperties.xml");
                if (File.Exists(xmlPath))
                {
                    ret.Add(id);
                    continue;
                }
                string[] apFiles = Directory.GetFiles(directory, "*.ap", SearchOption.TopDirectoryOnly);
                bool found = false;
                foreach (string apPath in apFiles)
                {
                    try
                    {
                        var zipFile = ZipFile.Read(apPath);
                        var xmlEntry = zipFile.Where(entry => entry.FileName == "RouteProperties.xml").FirstOrDefault();
                        if (xmlEntry == null) continue;
                        found = true;
                        break;
                    }
                    catch (Exception)
                    {

                    }
                }
                if (found) ret.Add(id);
            }
            return ret.ToArray();
        }
    }
}
