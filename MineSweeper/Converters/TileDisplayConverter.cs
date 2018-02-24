using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
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
                return new System.Windows.Controls.Image() { Source = tmp, Stretch = System.Windows.Media.Stretch.Fill };
            }
            else if (value is string number)
            {
                switch (number)
                {
                    case "1":
                        return new TextBlock{ Text = number, Foreground = new SolidColorBrush(Colors.Black) };
                    case "2":
                        return new TextBlock { Text = number, Foreground = new SolidColorBrush(Colors.DarkBlue) };
                    case "3":
                        return new TextBlock { Text = number, Foreground = new SolidColorBrush(Colors.DarkSeaGreen) };
                    case "4":
                        return new TextBlock { Text = number, Foreground = new SolidColorBrush(Colors.DarkCyan) };
                    case "5":
                        return new TextBlock { Text = number, Foreground = new SolidColorBrush(Colors.DarkKhaki) };
                    case "6":
                        return new TextBlock { Text = number, Foreground = new SolidColorBrush(Colors.SaddleBrown) };
                    case "7":
                        return new TextBlock { Text = number, Foreground = new SolidColorBrush(Colors.DarkGray) };
                    case "8":
                        return new TextBlock { Text = number, Foreground = new SolidColorBrush(Colors.DarkOrange) };
                    default:
                        return number;
                }
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
