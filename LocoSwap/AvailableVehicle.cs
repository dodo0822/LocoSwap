using Ionic.Zip;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace LocoSwap
{
    public class AvailableVehicle : Vehicle
    {
        private static XNamespace Namespace = "http://www.kuju.com/TnT/2003/Delta";

        private List<Tuple<string, string>> _cargoComponents;
        private int _entityCount;
        private List<string> _numberingList;
        private XElement _nameLocalisedString;
        public List<Tuple<string, string>> CargoComponents
        {
            get => _cargoComponents;
            set => SetProperty(ref _cargoComponents, value);
        }
        public int CargoCount
        {
            get => _cargoComponents.Count;
        }
        public int EntityCount
        {
            get => _entityCount;
            set => SetProperty(ref _entityCount, value);
        }
        public List<string> NumberingList
        {
            get => _numberingList;
            set => SetProperty(ref _numberingList, value);
        }
        public XElement NameLocalisedString
        {
            get => _nameLocalisedString;
            set => SetProperty(ref _nameLocalisedString, value);
        }

        public AvailableVehicle(string binPath, bool acceptReskin = true)
        {
            string[] binPathComponents = binPath.Split('\\');
            Provider = binPathComponents[0];
            Product = binPathComponents[1];
            BlueprintId = Path.ChangeExtension(string.Join("\\", binPathComponents.Skip(2)), "xml");
            Exists = VehicleExistance.Found;

            VehicleAvailibilityResult selfAvalibility = VehicleAvailibility.IsVehicleAvailable(this);
            if (!selfAvalibility.Available)
            {
                throw new Exception("Unable to load vehicle: bin file not found");
            }

            string actualBinPath = Path.Combine(Properties.Settings.Default.TsPath, "Assets", binPath);
            if (selfAvalibility.InApFile)
            {
                var zipFile = ZipFile.Read(selfAvalibility.ApPath);
                var binEntry = zipFile.Where(entry => entry.FileName == selfAvalibility.PathWithinAp).FirstOrDefault();
                if (binEntry == null)
                {
                    throw new Exception("Unable to load vehicle: bin file not found within .ap file");
                }
                var baseName = Path.GetFileNameWithoutExtension(selfAvalibility.PathWithinAp);
                var tempName = string.Format("{0}-{1}.bin", baseName, Utilities.StaticRandom.Instance.Next(10000, 99999));
                actualBinPath = Path.Combine(Utilities.GetTempDir(), tempName);
                Utilities.RemoveFile(actualBinPath);
                using (var fileStream = new FileStream(actualBinPath, FileMode.Create))
                {
                    binEntry.Extract(fileStream);
                    fileStream.Flush();
                    fileStream.Close();
                }
                Log.Debug("Extract to {0}", actualBinPath);
            }

            XDocument document;
            try
            {
                document = TsSerializer.Load(actualBinPath);
            }
            catch (Exception e)
            {
                Log.Debug("Failed to load vehicle blueprint: {0}", e);
                throw new Exception("Failed to load vehicle blueprint");
            }
            IEnumerable<XElement> blueprints = from item in document.Root.Descendants()
                                               where item.Name == "cEngineBlueprint" || item.Name == "cWagonBlueprint" || item.Name == "cReskinBlueprint" || item.Name == "cTenderBlueprint"
                                               select item;
            XElement blueprint = blueprints.FirstOrDefault();
            if (blueprint == null)
            {
                throw new Exception("The blueprint is not an engine, wagen or reskin");
            }
            Name = blueprint.Element("Name").Value;

            DisplayName = Name;
            XElement displayNameNode = document.Root.Descendants("DisplayName").Elements("Localisation-cUserLocalisedString").First();
            _nameLocalisedString = document.Root.Descendants("DisplayName").Elements("Localisation-cUserLocalisedString").First();
            var preferredDisplayName = Utilities.DetermineDisplayName(displayNameNode);
            if (preferredDisplayName != "") DisplayName = preferredDisplayName;

            if (blueprint.Name == "cEngineBlueprint")
                Type = VehicleType.Engine;
            else if (blueprint.Name == "cWagonBlueprint")
                Type = VehicleType.Wagon;
            else if (blueprint.Name == "cTenderBlueprint")
                Type = VehicleType.Tender;
            else
            {
                if (!acceptReskin)
                {
                    throw new Exception("Reskin found but not accepted!");
                }
                Log.Debug("{name} is a reskin! Trying to fill out rest of the info from the vehicle itself.", DisplayName);
                IsReskin = true;
                ReskinProvider = Provider;
                ReskinProduct = Product;
                ReskinBlueprintId = BlueprintId;

                try
                {
                    XElement reskinAssetBpId = blueprint.Element("ReskinAssetBpId");
                    Provider = reskinAssetBpId.Descendants("Provider").First().Value;
                    Product = reskinAssetBpId.Descendants("Product").First().Value;
                    BlueprintId = reskinAssetBpId.Descendants("BlueprintID").First().Value;
                }
                catch (Exception)
                {
                    Log.Debug("Cannot get main vehicle information!");
                    throw new Exception("Cannot get vehicle information from reskin blueprint.");
                }

                string mainVehicleBinPath = Path.ChangeExtension(XmlPath, "bin");
                try
                {
                    AvailableVehicle mainVehicle = new AvailableVehicle(mainVehicleBinPath, false);
                    Type = mainVehicle.Type;
                    EntityCount = mainVehicle.EntityCount;
                    CargoComponents = mainVehicle.CargoComponents;
                    NumberingList = mainVehicle.NumberingList;
                }
                catch (Exception e)
                {
                    Log.Debug("Exception caught loading main vehicle: {0}", e.Message);
                    throw e;
                }

                Log.Debug("After loading main vehicle: Type={0}, EntityCount={1}, CargoCount={2}", Type, EntityCount, CargoCount);

                return;
            }

            EntityCount = document.Root.Descendants("cEntityContainerBlueprint-sChild").Count();

            CargoComponents = new List<Tuple<string, string>>();
            XElement cargoDef = document.Root.Descendants("CargoDef").FirstOrDefault();
            if (cargoDef != null)
            {
                foreach (var cBulkCargoDef in cargoDef.Elements())
                {
                    var capacity = cBulkCargoDef.Element("Capacity");
                    var tuple = new Tuple<string, string>("0", "0000000000000000");
                    if (capacity != null)
                    {
                        tuple = new Tuple<string, string>(
                            capacity.Value,
                            capacity.Attribute(Namespace + "alt_encoding").Value);
                    }
                    CargoComponents.Add(tuple);
                }
            }

            try
            {
                var location = document.Root.Descendants("NumberingList").FirstOrDefault().Element("cCSVContainer").Element("CsvFile").Value;
                NumberingList = VehicleAvailibility.GetNumberingList(location);
            }
            catch (Exception)
            {
                NumberingList = new List<string>();
            }
        }
    }
}
