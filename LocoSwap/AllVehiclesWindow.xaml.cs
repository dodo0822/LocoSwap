using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace LocoSwap
{
    /// <summary>
    /// Interaction logic for AllVehiclesWindow.xaml
    /// </summary>
    public partial class AllVehiclesWindow : Window
    {
        public class VehiclesViewModel : ModelBase
        {
            public ObservableCollection<ScenarioVehicle> Vehicles { get; set; } = new ObservableCollection<ScenarioVehicle>();
        }

        private VehiclesViewModel ViewModel;

        public AllVehiclesWindow(List<ScenarioVehicle> _vehicles)
        {
            InitializeComponent();

            ViewModel = new VehiclesViewModel();
            ViewModel.Vehicles = new ObservableCollection<ScenarioVehicle>(_vehicles);

            DataContext = ViewModel;
        }
    }
}
