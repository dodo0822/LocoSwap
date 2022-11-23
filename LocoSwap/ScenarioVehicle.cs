using System;

namespace LocoSwap
{
    public class ScenarioVehicle : Vehicle
    {
        private int _idx;
        private string _number;
        private bool _flipped;
        private string _possibleSubstitutionDisplayName;

        public int Idx
        {
            get => _idx;
            set => SetProperty(ref _idx, value);
        }
        public string Number
        {
            get => _number;
            set => SetProperty(ref _number, value);
        }
        public bool Flipped
        {
            get => _flipped;
            set => SetProperty(ref _flipped, value);
        }
        public string PossibleSubstitutionDisplayName
        {
            get => _possibleSubstitutionDisplayName;
            set => SetProperty(ref _possibleSubstitutionDisplayName, value);
        }
        public ScenarioVehicle() : base()
        {
            Idx = -1;
            Number = "";
            Flipped = false;
        }

        public ScenarioVehicle(int idx, string provider, string product, string blueprintId, string name, string number, bool flipped)
            : base(provider, product, blueprintId, name)
        {
            Idx = idx;
            Number = number;
            Flipped = flipped;
        }
        public void CopyFrom(AvailableVehicle from)
        {
            this.Provider = from.Provider;
            this.Product = from.Product;
            this.BlueprintId = from.BlueprintId;
            this.Name = from.Name;
            this.Exists = VehicleExistance.Replaced;
            this.Type = from.Type;
            this.DisplayName = from.DisplayName;

            if (from.IsReskin)
            {
                this.IsReskin = true;
                this.ReskinBlueprintId = from.ReskinBlueprintId;
                this.ReskinProvider = from.ReskinProvider;
                this.ReskinProduct = from.ReskinProduct;
            }
            else
            {
                this.IsReskin = false;
                this.ReskinBlueprintId = "";
                this.ReskinProvider = "";
                this.ReskinProduct = "";
            }

            if (from.NumberingList.Count > 0)
            {
                var index = Utilities.StaticRandom.Instance.Next(from.NumberingList.Count);
                Number = from.NumberingList[index];
            }
            else
            {
                Number = (new Guid()).ToString();
            }
        }

    }
}