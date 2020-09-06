using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocoSwap
{
    public class ScenarioVehicle : Vehicle
    {
        private int _idx;
        private string _number;
        private bool _flipped;
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
            this.Provider = string.Copy(from.Provider);
            this.Product = string.Copy(from.Product);
            this.BlueprintId = string.Copy(from.BlueprintId);
            this.Name = string.Copy(from.Name);
            this.Exists = VehicleExistance.Replaced;
            this.Type = from.Type;
            this.DisplayName = string.Copy(from.DisplayName);

            if (from.NumberingList.Count > 0)
            {
                var index = Utilities.StaticRandom.Instance.Next(from.NumberingList.Count);
                Number = from.NumberingList[index];
            }
        }

    }
}