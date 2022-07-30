namespace FaceDetect.FormsApp.Converters;

using System;
using System.Collections.Generic;
using System.Globalization;

using Xamarin.Forms;

public class StringJoinConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IEnumerable<string> ie)
        {
            return String.Join(", ", ie);
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
