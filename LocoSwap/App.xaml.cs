using LocoSwap.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;

using Serilog;

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

            try
            {
                if (File.Exists("debug.log"))
                {
                    File.Delete("debug.log");
                }
            }
            catch (Exception)
            {
                Debug.Print("Can not delete existing log file");
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("debug.log",
                    rollingInterval: RollingInterval.Infinite,
                    rollOnFileSizeLimit: false)
                .CreateLogger();

            Log.Debug("LocoSwap version {0} starting up..", Assembly.GetEntryAssembly().GetName().Version.ToString());

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

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

            Log.Debug("SwapPreset has {0} items", Settings.Default.Preset.List.Count);

            new MainWindow().Show();
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            Log.Warning("Uncaught exception: {0} - {1}\n{2}", e.Exception.GetType().ToString(), e.Exception.Message, e.Exception.StackTrace.ToString());
            Current.Shutdown();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = (Exception)e.ExceptionObject;
            Log.Warning("Uncaught exception: {0}", exception.Message);
            Current.Shutdown();
        }

        public void SetLanguageDictionary()
        {
            var lang = Settings.Default.Language;
            if (lang == "") lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            Log.Debug("Set language to {0}", lang);
            switch (lang)
            {
                case "de":
                    Language.Resources.Culture = new System.Globalization.CultureInfo("de-DE");
                    break;
                case "ru":
                    Language.Resources.Culture = new System.Globalization.CultureInfo("ru-RU");
                    break;
                case "en":
                default:
                    Language.Resources.Culture = new System.Globalization.CultureInfo("en-US");
                    break;
            }
        }
    }
}
