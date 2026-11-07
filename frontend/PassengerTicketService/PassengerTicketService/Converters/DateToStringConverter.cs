using System.Globalization;

namespace PassengerTicketService.Converters
{
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                // Als de datum vandaag is, toon "vandaag", anders alleen de datum (dd/MM/yyyy)
                return date.Date == DateTime.Today ? "vandaag" : date.ToString("dd/MM/yyyy");
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}