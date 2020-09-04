using LocoSwap.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace LocoSwap
{
    static class Utilities
    {
        public static string GetTempDir()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "temp");
        }
        public static string GetSerzPath()
        {
            return Path.Combine(Properties.Settings.Default.TsPath, "serz.exe");
        }
        public static void RemoveFile(string path)
        {
            if (File.Exists(path))
            {
                File.SetAttributes(path, FileAttributes.Normal);
                File.Delete(path);
            }
        }
        public static bool ChangeTsPath()
        {
            var valid = false;
            while (!valid)
            {
                using (var dialog = new CommonOpenFileDialog())
                {
                    dialog.Title = Language.Resources.select_ts_path;
                    dialog.IsFolderPicker = true;

                    var result = dialog.ShowDialog();
                    if (result != CommonFileDialogResult.Ok)
                    {
                        return false;
                    }
                    var path = dialog.FileName;
                    var tsExe = Path.Combine(path, "RailWorks.exe");
                    if (!File.Exists(tsExe))
                    {
                        MessageBox.Show(Language.Resources.msg_ts_path_invalid, Language.Resources.msg_error, MessageBoxButton.OK, MessageBoxImage.Warning);
                        continue;
                    }
                    Settings.Default.TsPath = path;
                    Settings.Default.Save();
                    valid = true;
                }
            }
            return true;
        }

        public static void CopyUserLocalisedString(XElement dest, XElement orig)
        {
            if (dest == null || orig == null) return;
            var names = new string[]{ "English", "French", "Italian", "German", "Spanish", "Dutch", "Polish", "Russian", "Key" };
            foreach (var name in names)
            {
                XElement destName = dest.Element(name);
                XElement origName = orig.Element(name);
                if (destName != null && origName != null)
                {
                    destName.Value = origName.Value;
                }
            }
            dest.Element("Other").Elements().Remove();
        }

        public static class StaticRandom
        {
            private static int seed;

            private static ThreadLocal<Random> threadLocal = new ThreadLocal<Random>
                (() => new Random(Interlocked.Increment(ref seed)));

            static StaticRandom()
            {
                seed = Environment.TickCount;
            }

            public static Random Instance { get { return threadLocal.Value; } }
        }
    }
}
