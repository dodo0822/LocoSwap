using LocoSwap.Properties;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;

namespace LocoSwap
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SetLanguageDictionary();

            if (!Directory.Exists(Utilities.GetTempDir()))
            {
                Directory.CreateDirectory(Utilities.GetTempDir());
            }

            string[] files = Directory.GetFiles(Utilities.GetTempDir());
            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            if (Settings.Default.Preset == null) Settings.Default.Preset = new SwapPreset();

            while (Settings.Default.TsPath == "")
            {
                MessageBox.Show(Language.Resources.msg_first_time, Language.Resources.msg_message, MessageBoxButton.OK, MessageBoxImage.Information);
                var selected = Utilities.ChangeTsPath();
                if(!selected)
                {
                    MessageBox.Show(Language.Resources.msg_ts_path_required, Language.Resources.msg_message, MessageBoxButton.OK, MessageBoxImage.Information);
                    Current.Shutdown();
                    return;
                }
            }

            Debug.Print("SwapPreset has {0} items", Settings.Default.Preset.List.Count);

            //ScenarioEditWindow scenarioEditWindow = new ScenarioEditWindow("00000035-0000-0000-0000-000000002012", "a198cf63-bcd3-473b-8b31-731e6515a023");
            //scenarioEditWindow.Show();
            new MainWindow().Show();
            //new SwapPresetWindow().Show();
            //new SettingsWindow().Show();
        }

        public void SetLanguageDictionary()
        {
            var lang = Settings.Default.Language;
            if (lang == "") lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            Debug.Print("Set language to {0}", lang);
            switch (lang)
            {
                case "de":
                    Language.Resources.Culture = new System.Globalization.CultureInfo("de-DE");
                    break;
                case "en":
                default:
                    Language.Resources.Culture = new System.Globalization.CultureInfo("en-US");
                    break;
            }
        }
    }
}
