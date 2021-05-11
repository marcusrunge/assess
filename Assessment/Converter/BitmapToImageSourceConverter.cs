using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Assessment.Converter
{
    public class BitmapToImageSourceConverter : IValueConverter
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        /// <summary>
        /// <c>Convert</c> converts a <see cref="Bitmap"/> into a WPF <see cref="ImageSource"/>
        /// </summary>
        /// <param name="value">The bitmap</param>
        /// <param name="targetType">The target type</param>
        /// <param name="parameter">The converter parameter</param>
        /// <param name="culture">The culture</param>
        /// <returns>The <see cref="ImageSource"/></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case null:
                    return value;
                case Bitmap bitmap:
                    {
                        var handle = bitmap.GetHbitmap();
                        try
                        {
                            return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        }
                        finally { DeleteObject(handle); }
                    }

                default:
                    throw new FormatException("Value is not a bitmap");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
