using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace SEH.Converts
{
    public class BoolToDashArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool b && b)
            {
                return new DoubleCollection { 3, 3 };
            }
            return new DoubleCollection(); //实线
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
