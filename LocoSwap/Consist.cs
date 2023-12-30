﻿using System.Collections.Generic;

namespace LocoSwap
{
    public enum ConsistVehicleExistance
    {
        Found,
        Missing,
        MissingWithRules,
        MissingWithRulesPartiallyReplaced,
        PartiallyReplaced,
        FullyReplaced
    }
    public class Consist : ModelBase
    {
        private int _idx;
        private string _name;
        private ConsistVehicleExistance _isComplete;
        private List<ScenarioVehicle> _vehicles;
        private bool _isPlayerConsist;
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
        public ConsistVehicleExistance IsComplete
        {
            get => _isComplete;
            set => SetProperty(ref _isComplete, value);
        }
        public List<ScenarioVehicle> Vehicles
        {
            get => _vehicles;
            set => SetProperty(ref _vehicles, value);
        }
        public bool IsPlayerConsist
        {
            get => _isPlayerConsist;
            set => SetProperty(ref _isPlayerConsist, value);
        }

        public Consist()
        {
            Idx = -1;
            Name = "";
            IsComplete = ConsistVehicleExistance.Found;
            Vehicles = new List<ScenarioVehicle>();
            IsPlayerConsist = false;
        }

        public Consist(int idx, string name)
        {
            Idx = idx;
            Name = name;
            IsComplete = ConsistVehicleExistance.Found;
            Vehicles = new List<ScenarioVehicle>();
            IsPlayerConsist = false;
        }

        public void DetermineCompletenessAfterReplace()
        {
            int countNotReplacedVehicles = 0;
            foreach (var vehicle in Vehicles)
            {
                if (vehicle.Exists == VehicleExistance.Missing)
                {
                    IsComplete = ConsistVehicleExistance.PartiallyReplaced;
                    return;
                }
                if (vehicle.Exists == VehicleExistance.MissingWithRule)
                {
                    countNotReplacedVehicles++;
                    IsComplete = ConsistVehicleExistance.MissingWithRulesPartiallyReplaced;
                }
            }
            if (countNotReplacedVehicles == 0)
            {
            IsComplete = ConsistVehicleExistance.FullyReplaced;
        }
    }
}
}
