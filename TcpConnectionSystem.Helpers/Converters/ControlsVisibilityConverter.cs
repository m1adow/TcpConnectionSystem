using System.Collections.ObjectModel;
using System.Globalization;
using TcpConnectionSystem.Models;

namespace TcpConnectionSystem.Helpers.Converters
{
    public sealed class ControlsVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = Visibility.Collapsed;

            if (value is ObservableCollection<User> collection)
            {
                result = Convert(collection);
            }
            else if (value is ObservableCollection<KeyValuePair<string, string>> collection)
            {
                result = Convert(collection);
            }

            return result;
        }

        private Visibility Convert<T>(ObservableCollection<T> collection) => collection.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

