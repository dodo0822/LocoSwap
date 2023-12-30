﻿using System;
using System.IO;

namespace LocoSwap
{
    public enum VehicleType
    {
        Unknown,
        Engine,
        Wagon,
        Tender
    }
    public enum VehicleExistance
    {
        Found,
        Missing,
        MissingWithRule,
        Replaced
    }
    public class Vehicle : ModelBase
    {
        private string _provider;
        private string _product;
        private string _blueprintId;

        private bool _isReskin;
        private string _reskinProvider;
        private string _reskinProduct;
        private string _reskinBlueprintId;

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
        public bool IsReskin
        {
            get => _isReskin;
            set => SetProperty(ref _isReskin, value);
        }
        public string ReskinProvider
        {
            get => _reskinProvider;
            set => SetProperty(ref _reskinProvider, value);
        }
        public string ReskinProduct
        {
            get => _reskinProduct;
            set => SetProperty(ref _reskinProduct, value);
        }
        public string ReskinBlueprintId
        {
            get => _reskinBlueprintId;
            set => SetProperty(ref _reskinBlueprintId, value);
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
        public float Length { get; set; }
        public VehicleExistance Exists
        {
            get => _exists;
            set => SetProperty(ref _exists, value);
        }
        public string XmlPath
        {
            get
            {
                return string.Format("{0}\\{1}\\{2}", Provider, Product, BlueprintId);
            }
        }
        public string FullXmlPath
        {
            get
            {
                return Path.Combine(Properties.Settings.Default.TsPath, "Assets", XmlPath);
            }
        }
        public string ReskinXmlPath
        {
            get
            {
                return string.Format("{0}\\{1}\\{2}", ReskinProvider, ReskinProduct, ReskinBlueprintId);
            }
        }
        public string ReskinFullXmlPath
        {
            get
            {
                return Path.Combine(Properties.Settings.Default.TsPath, "Assets", ReskinXmlPath);
            }
        }
        public string DisplayXmlPath
        {
            get
            {
                return IsReskin ? ReskinXmlPath : XmlPath;
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

        public Vehicle(string provider, string product, string blueprintId, string name, float length)
        {
            Provider = provider;
            Product = product;
            BlueprintId = blueprintId;
            Name = name;
            DisplayName = name;
            Exists = VehicleExistance.Found;
            Type = VehicleType.Unknown;
            Length = (float) Math.Round(length, 2);
        }
    }
}
