using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocoSwap
{
    public class Consist : ModelBase
    {
        private int _idx;
        private string _name;
        private VehicleExistance _isComplete;
        private List<ScenarioVehicle> _vehicles;
        public int Idx
        {
            get => _idx;
            set => SetProperty(ref _idx, value);
        }
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public VehicleExistance IsComplete
        {
            get => _isComplete;
            set => SetProperty(ref _isComplete, value);
        }
        public List<ScenarioVehicle> Vehicles
        {
            get => _vehicles;
            set => SetProperty(ref _vehicles, value);
        }

        public Consist()
        {
            Idx = -1;
            Name = "";
            IsComplete = VehicleExistance.Found;
            Vehicles = new List<ScenarioVehicle>();
        }

        public Consist(int idx, string name)
        {
            Idx = idx;
            Name = name;
            IsComplete = VehicleExistance.Found;
            Vehicles = new List<ScenarioVehicle>();
        }
    }
}
