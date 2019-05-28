using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Data;

namespace LocalFileSharing.DesktopUI.Converters {
    public class IPAddressToStringConverter
        : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is IPAddress ipAddress) {
                return ipAddress.ToString();
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            bool result = IPAddress.TryParse(value.ToString(), out IPAddress ipAddress);

            if (!result) {
                return DependencyProperty.UnsetValue;
            }

            return ipAddress;
        }
    }
}
