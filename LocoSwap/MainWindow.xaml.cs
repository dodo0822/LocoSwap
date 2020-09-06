using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LocoSwap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Route> Routes { get; } = new ObservableCollection<Route>();
        public ObservableCollection<Scenario> Scenarios { get; } = new ObservableCollection<Scenario>();
        public string WindowTitle { get; set; } = "LocoSwap";
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            this.WindowTitle = "LocoSwap " + Assembly.GetEntryAssembly().GetName().Version.ToString();

            var routeIds = Route.ListAllRoutes();

            foreach (var id in routeIds)
            {
                try
                {
                    Routes.Add(new Route(id));
                }
                catch(Exception)
                {

                }
            }
        }

        private void RouteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Scenarios.Clear();
            var routeId = ((Route)RouteList.SelectedItem).Id;
            var scenarioIds = Scenario.ListAllScenarios(routeId);
            foreach (var id in scenarioIds)
            {
                try
                {
                    Scenarios.Add(new Scenario(routeId, id));
                }
                catch (Exception)
                {

                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (RouteList.SelectedItem == null || ScenarioList.SelectedItem == null) return;
            var routeId = ((Route)RouteList.SelectedItem).Id;
            var scenarioId = ((Scenario)ScenarioList.SelectedItem).Id;
            var editWindow = new ScenarioEditWindow(routeId, scenarioId);
            editWindow.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (RouteList.SelectedItem == null || ScenarioList.SelectedItem == null) return;
            var scenario = (Scenario)ScenarioList.SelectedItem;
            Process.Start(scenario.ScenarioDirectory);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }

        private void ScenarioList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dataContext = ((FrameworkElement) e.OriginalSource).DataContext;
            if (dataContext is Scenario)
            {
                if (RouteList.SelectedItem == null) return;
                var routeId = ((Route)RouteList.SelectedItem).Id;
                var scenarioId = ((Scenario)dataContext).Id;
                var editWindow = new ScenarioEditWindow(routeId, scenarioId);
                editWindow.Show();
            }
        }
    }
}
