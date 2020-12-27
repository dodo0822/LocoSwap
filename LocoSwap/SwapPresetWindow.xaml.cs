using LocoSwap.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LocoSwap
{
    /// <summary>
    /// Interaction logic for SwapPresetWindow.xaml
    /// </summary>
    public partial class SwapPresetWindow : Window
    {
        public event EventHandler ApplyClicked;
        public List<SwapPresetItem> SelectedItems
        {
            get => PresetList.SelectedItems.Cast<SwapPresetItem>().ToList();
        }
        public SwapPresetWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = PresetList.SelectedItems.Cast<object>().ToArray();
            foreach (SwapPresetItem item in selected)
            {
                Settings.Default.Preset.List.Remove(item);
            }
            Settings.Default.Save();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyClicked?.Invoke(this, new EventArgs());
        }
    }
}
