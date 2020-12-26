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
            if (value == null) return "/LocoSwap;component/Resources/PreviewNotAvailable.png";
            Vehicle vehicle = (Vehicle)value;
            return VehicleAvailibility.GetVehicleImage(vehicle);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
