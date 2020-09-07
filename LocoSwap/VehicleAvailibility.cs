using Ionic.Zip;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LocoSwap
{
    public struct VehicleAvailibilityResult
    {
        public bool Available;
        public bool InApFile;
        public string ApPath;
        public string PathWithinAp;
    }
    static class VehicleAvailibility
    {
        private static Dictionary<string, VehicleAvailibilityResult> _vehicleTable;
        private static Dictionary<string, string> _vehicleDisplayNameTable;
        private static Dictionary<string, List<string>> _numberingListCache;

        static VehicleAvailibility()
        {
            _vehicleTable = new Dictionary<string, VehicleAvailibilityResult>();
            _vehicleDisplayNameTable = new Dictionary<string, string>();
            _numberingListCache = new Dictionary<string, List<string>>();
        }

        public static string GetVehicleDisplayName(Vehicle vehicle)
        {
            if (_vehicleDisplayNameTable.ContainsKey(vehicle.XmlPath)) return _vehicleDisplayNameTable[vehicle.XmlPath];
            var binPath = Path.ChangeExtension(vehicle.XmlPath, "bin");
            AvailableVehicle actualVehicle = new AvailableVehicle(binPath);
            _vehicleDisplayNameTable[vehicle.XmlPath] = string.Copy(actualVehicle.DisplayName);
            return _vehicleDisplayNameTable[vehicle.XmlPath];
        }

        public static List<string> GetNumberingList(string location)
        {
            if (_numberingListCache.ContainsKey(location))
            {
                return _numberingListCache[location];
            }
            var dcsvPath = Path.Combine(Properties.Settings.Default.TsPath, "Assets", location) + ".dcsv";
            if (!File.Exists(dcsvPath))
            {
                var components = location.Split('\\');
                if (components.Length < 3) throw new Exception("Numbering list not found");
                var apDirectory = Path.Combine(Properties.Settings.Default.TsPath, "Assets", components[0], components[1]);
                var apFiles = Directory.GetFiles(apDirectory, "*.ap", SearchOption.TopDirectoryOnly);
                bool found = false;
                foreach (var ap in apFiles)
                {
                    try
                    {
                        var zipFile = ZipFile.Read(ap);
                        var dcsvEntry = zipFile.Where(entry => entry.FileName == string.Join("/", components.Skip(2)) + ".dcsv").FirstOrDefault();
                        if (dcsvEntry == null) continue;
                        dcsvPath = Path.Combine(Utilities.GetTempDir(), Path.GetFileName(dcsvPath));
                        zipFile.FlattenFoldersOnExtract = true;
                        Utilities.RemoveFile(dcsvPath);
                        dcsvEntry.Extract(Utilities.GetTempDir());
                        found = true;
                        break;
                    }
                    catch (Exception)
                    {

                    }
                }
                if (!found) throw new Exception("Numbering list not found");
            }
            List<string> list = new List<string>();
            XDocument dcsv = XDocument.Load(dcsvPath);
            IEnumerable<XElement> cCSVItems = dcsv.Descendants("cCSVItem");
            foreach (XElement cCSVItem in cCSVItems)
            {
                if (cCSVItem.Element("Name") == null) continue;
                list.Add(cCSVItem.Element("Name").Value);
            }
            _numberingListCache[location] = list;
            return _numberingListCache[location];
        }

        public static VehicleAvailibilityResult IsVehicleAvailable(Vehicle vehicle)
        {
            VehicleAvailibilityResult ret = new VehicleAvailibilityResult
            {
                Available = false,
                InApFile = false,
                ApPath = null,
                PathWithinAp = null
            };
            if (vehicle.IsReskin)
            {
                // We should determine if the reskin itself exists first
                Vehicle reskinAsVehicle = new Vehicle(vehicle.ReskinProvider, vehicle.ReskinProduct, vehicle.ReskinBlueprintId, "Reskin");
                Log.Debug("IsVehicleAvailable: check for reskin {0}", reskinAsVehicle.XmlPath);
                VehicleAvailibilityResult reskinAvailability = IsVehicleAvailable(reskinAsVehicle);
                if (!reskinAvailability.Available)
                {
                    return ret;
                }
            }
            if (_vehicleTable.ContainsKey(vehicle.XmlPath))
            {
                return _vehicleTable[vehicle.XmlPath];
            }
            var xmlPath = vehicle.FullXmlPath;
            var binPath = Path.ChangeExtension(xmlPath, "bin");
            if (File.Exists(binPath))
            {
                ret.Available = true;
                _vehicleTable[vehicle.XmlPath] = ret;
                return ret;
            }

            var apDirectory = Path.Combine(Properties.Settings.Default.TsPath, "Assets", vehicle.Provider, vehicle.Product);
            if (Directory.Exists(apDirectory))
            {
                var apFiles = Directory.GetFiles(apDirectory, "*.ap");
                var found = false;
                string foundApPath = "";
                var binName = Path.ChangeExtension(vehicle.BlueprintId, "bin").Replace('\\', '/');
                foreach (var apPath in apFiles)
                {
                    var zipFile = ZipFile.Read(apPath);
                    var result = zipFile.Any(entry => entry.FileName.Equals(binName));
                    if (result)
                    {
                        found = true;
                        foundApPath = apPath;
                        break;
                    }
                }
                if(found)
                {
                    ret.Available = true;
                    ret.InApFile = true;
                    ret.ApPath = foundApPath;
                    ret.PathWithinAp = binName;
                    _vehicleTable[vehicle.XmlPath] = ret;
                    return ret;
                }
            }

            _vehicleTable[vehicle.XmlPath] = ret;
            return ret;
        }

        public static void ClearTable()
        {
            _vehicleTable.Clear();
            _vehicleDisplayNameTable.Clear();
            _numberingListCache.Clear();
        }
    }
}
