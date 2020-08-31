using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LocoSwap.Converters
{
    [ValueConversion(typeof(VehicleExistance), typeof(string))]
    public class StatusToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            var status = (VehicleExistance)value;
            string image;
            switch (status)
            {
                case VehicleExistance.Found:
                    image = "BulletGreen.png";
                    break;
                case VehicleExistance.Replaced:
                    image = "Replaced.png";
                    break;
                case VehicleExistance.Missing:
                default:
                    image = "BulletRed.png";
                    break;
            }
            string uri = String.Format("/LocoSwap;component/Resources/{0}", image);
            return uri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
