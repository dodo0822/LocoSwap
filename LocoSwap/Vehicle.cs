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
    public enum VehicleType
    {
        Unknown,
        Engine,
        Wagon
    }
    public enum VehicleExistance
    {
        Found,
        Missing,
        Replaced
    }
    public class Vehicle : ModelBase
    {
        private string _provider;
        private string _product;
        private string _blueprintId;
        private string _name;
        private string _displayName;
        private VehicleType _type;
        private VehicleExistance _exists;
        public string Provider
        {
            get => _provider;
            set => SetProperty(ref _provider, value);
        }
        public string Product
        {
            get => _product;
            set => SetProperty(ref _product, value);
        }
        public string BlueprintId
        {
            get => _blueprintId;
            set => SetProperty(ref _blueprintId, value);
        }
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }
        public VehicleType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }
        public VehicleExistance Exists
        {
            get => _exists;
            set => SetProperty(ref _exists, value);
        }
        public string XmlPath {
            get
            {
                return String.Format("{0}\\{1}\\{2}", Provider, Product, BlueprintId);
            }
        }
        public string FullXmlPath
        {
            get
            {
                return Path.Combine(Properties.Settings.Default.TsPath, "Assets", XmlPath);
            }
        }

        public Vehicle()
        {
            Provider = "";
            Product = "";
            BlueprintId = "";
            Name = "";
            DisplayName = "";
            Exists = VehicleExistance.Found;
            Type = VehicleType.Unknown;
        }

        public Vehicle(string provider, string product, string blueprintId, string name)
        {
            Provider = provider;
            Product = product;
            BlueprintId = blueprintId;
            Name = name;
            DisplayName = name;
            Exists = VehicleExistance.Found;
            Type = VehicleType.Unknown;
        }
    }
}
