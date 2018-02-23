using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MineSweeper.Converters
{
    public class TileDisplayConverter : IValueConverter
    {
        private static TileDisplayConverter instance;
        public static TileDisplayConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new TileDisplayConverter();

                return instance;
            }
        }
        private TileDisplayConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is Bitmap)
            {
                BitmapImage tmp = (BitmapImage)DrawingBitmapToImageConverter.Instance.Convert(value, targetType, parameter, culture);
                return new System.Windows.Controls.Image() { Source = tmp,Stretch=System.Windows.Media.Stretch.Fill};
            }
            else if (value is string number)
            {
                return number;
            }
            else
                return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
