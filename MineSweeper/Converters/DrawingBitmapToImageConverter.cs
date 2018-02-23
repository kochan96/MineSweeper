using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MineSweeper.Converters
{
    public class DrawingBitmapToImageConverter : IValueConverter
    {
        private static DrawingBitmapToImageConverter instance;
        public static DrawingBitmapToImageConverter Instance
        {
            get
            {
                if (instance == null)
                    instance = new DrawingBitmapToImageConverter();

                return instance;
            }
        }

        private DrawingBitmapToImageConverter(){ }
        

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is Bitmap bitmap)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    return bitmapImage;
                }
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
