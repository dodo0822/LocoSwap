using LocoSwap.Properties;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using Serilog;

namespace LocoSwap
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public class LanguageListItem
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public ObservableCollection<LanguageListItem> LanguageList { get; } = new ObservableCollection<LanguageListItem>();

        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = this;
            LanguageList.Add(new LanguageListItem
            {
                Name = LocoSwap.Language.Resources.use_system_language,
                Value = ""
            });
            LanguageList.Add(new LanguageListItem
            {
                Name = "Deutsch",
                Value = "de"
            });
            LanguageList.Add(new LanguageListItem
            {
                Name = "English",
                Value = "en"
            });
            LanguageList.Add(new LanguageListItem
            {
                Name = "Русский",
                Value = "ru"
            });
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TsPathBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Utilities.ChangeTsPath();
        }

        private void LanguageComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Log.Debug("Change language to {0}", Settings.Default.Language);
            var app = (App)Application.Current;
            app.SetLanguageDictionary();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
