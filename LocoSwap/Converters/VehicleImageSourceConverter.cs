using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LocoSwap.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class VehicleImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            var xmlPath = (string)value;
            var vehicleDirectory = new FileInfo(Path.Combine(Properties.Settings.Default.TsPath, "Assets", xmlPath)).Directory.FullName;
            var imagePath = Path.Combine(vehicleDirectory, "Locoinformation", "image.png");
            if (File.Exists(imagePath)) return imagePath;
            else return "/LocoSwap;component/Resources/PreviewNotAvailable.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
