using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MineSweeper.Converters
{
    public class NotBitmapConverter: IValueConverter
    {
        private static NotBitmapConverter instance;
        public static NotBitmapConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new NotBitmapConverter();
                return instance;
            }
        }

        private NotBitmapConverter() { }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is System.Drawing.Bitmap);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
