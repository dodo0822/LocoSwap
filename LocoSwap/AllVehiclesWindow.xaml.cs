using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
