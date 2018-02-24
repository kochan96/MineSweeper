using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MineSweeper.Converters
{
    public class IsIntegerOrEmptyConverter: IValueConverter
    {
        private static IsIntegerOrEmptyConverter instance;
        public static IsIntegerOrEmptyConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new IsIntegerOrEmptyConverter();
                return instance;
            }
        }

        private IsIntegerOrEmptyConverter() { }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is int || int.TryParse(value.ToString(), out int result) || value.ToString()==String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
