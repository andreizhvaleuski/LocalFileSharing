using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LocalFileSharing.DesktopUI.Converters {
    public class BytesToHumanReadableStringConverter : IValueConverter {
        private Dictionary<long, string> prefixes = new Dictionary<long, string>() {
            { 1024L, "B" },
            { 1024L * 1024L, "KB" },
            { 1024L * 1024L * 1024L, "MB" },
            { 1024L * 1024L * 1024L * 1024L, "GB" },
            { 1024L * 1024L * 1024L * 1024L * 1024L, "TB" },
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is long number) {
                foreach (var item in prefixes) {
                    if (number / item.Key == 0) {
                        return string.Format("{0:0.00} {1}", (double)number / item.Key * 1024, item.Value);
                    }
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
