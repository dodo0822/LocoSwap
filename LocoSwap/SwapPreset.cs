using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;

namespace LocoSwap
{
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class SwapPresetItem : ModelBase
    {
        public string TargetName { get; set; }
        public string TargetXmlPath { get; set; }
        public string NewName { get; set; }
        public string NewXmlPath { get; set; }
        public float NewLength { get; set; }
    }

    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class SwapPreset
    {
        public ObservableCollection<SwapPresetItem> List { get; } = new ObservableCollection<SwapPresetItem>();

        public bool Contains(string targetXmlPath)
        {
            return List.Any((item) => item.TargetXmlPath == targetXmlPath);
        }

        public SwapPresetItem Find(string targetXmlPath)
        {
            return List.Where((item) => item.TargetXmlPath == targetXmlPath).FirstOrDefault();
        }
    }
}
