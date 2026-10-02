using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WinClickWpf.Converters
{
    public class CategoryNameToUwpVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (str.Equals("Cat_Preinstalled", StringComparison.OrdinalIgnoreCase) ||
                    str.Equals("Предустановленные приложения", StringComparison.OrdinalIgnoreCase) ||
                    str.Equals("Preinstalled Applications", StringComparison.OrdinalIgnoreCase))
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
