using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LocoSwap
{
    public class AvailableVehicle : Vehicle
    {
        private int _cargoCount;
        private int _entityCount;
        private List<string> _numberingList;
        private XElement _nameLocalisedString;
        public int CargoCount
        {
            get => _cargoCount;
            set => SetProperty(ref _cargoCount, value);
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

        public AvailableVehicle(string binPath)
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
                Debug.Print("Extract to {0}", actualBinPath);
            }

            XDocument document;
            document = TsSerializer.Load(actualBinPath);
            IEnumerable<XElement> blueprints = from item in document.Root.Descendants()
                                               where item.Name == "cEngineBlueprint" || item.Name == "cWagonBlueprint"
                                               select item;
            XElement blueprint = blueprints.FirstOrDefault();
            if (blueprint == null)
            {
                throw new Exception("The blueprint is not an engine or a wagon");
            }
            Name = blueprint.Element("Name").Value;

            DisplayName = Name;
            XElement displayNameNode = document.Root.Descendants("DisplayName").Elements("Localisation-cUserLocalisedString").First();
            _nameLocalisedString = document.Root.Descendants("DisplayName").Elements("Localisation-cUserLocalisedString").First();
            var preferredDisplayName = Utilities.DetermineDisplayName(displayNameNode);
            if (preferredDisplayName != "") DisplayName = preferredDisplayName;

            if (blueprint.Name == "cEngineBlueprint")
                Type = VehicleType.Engine;
            else
                Type = VehicleType.Wagon;

            EntityCount = document.Root.Descendants("cEntityContainerBlueprint-sChild").Count();

            XElement cargoDef = document.Root.Descendants("CargoDef").FirstOrDefault();
            if (cargoDef == null) CargoCount = 0;
            else
            {
                CargoCount = cargoDef.Elements().Count();
            }

            try
            {
                var location = document.Root.Descendants("NumberingList").FirstOrDefault().Element("cCSVContainer").Element("CsvFile").Value;
                NumberingList = VehicleAvailibility.GetNumberingList(location);
            }
            catch(Exception)
            {
                NumberingList = new List<string>();
            }
        }
    }
}
