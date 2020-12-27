using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace LocoSwap
{
    /// <summary>
    /// Interaction logic for VehicleNumberSelectionWindow.xaml
    /// </summary>
    public partial class VehicleNumberSelectionWindow : Window
    {
        public enum WindowType
        {
            Selection,
            List
        }
        public class ViewModel : ModelBase
        {
            private bool _isSelection = false;
            private string _number = "";
            public ObservableCollection<string> CandidateNumbers { get; set; } = new ObservableCollection<string>();

            public bool IsSelection
            {
                get => _isSelection;
                set => SetProperty(ref _isSelection, value);
            }

            public string Number
            {
                get => _number;
                set => SetProperty(ref _number, value);
            }
        }

        private readonly ViewModel Model;

        public string SelectedNumber
        {
            get => Model.Number;
        }
        public VehicleNumberSelectionWindow(IEnumerable<string> numbers, WindowType type, string previousNumber = "")
        {
            Model = new ViewModel();
            InitializeComponent();
            this.DataContext = Model;

            Model.IsSelection = type == WindowType.Selection;
            foreach (string number in numbers)
            {
                Model.CandidateNumbers.Add(number);
            }
            Model.Number = previousNumber;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Model.IsSelection)
            {
                string number = (string)NumberListBox.SelectedItem;
                Model.Number = number;
            }
        }
    }
}
