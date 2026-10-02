using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WinClickWpf.Converters
{
    public class CategoryNameToDnsVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (str.Equals("Cat_UsefulTweaks", StringComparison.OrdinalIgnoreCase) ||
                    str.Equals("Полезные твики", StringComparison.OrdinalIgnoreCase) ||
                    str.Equals("Useful Tweaks", StringComparison.OrdinalIgnoreCase))
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
