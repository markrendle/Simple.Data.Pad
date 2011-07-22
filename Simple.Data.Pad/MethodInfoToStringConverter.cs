using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Data.Pad
{
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Data;

    public class MethodInfoToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var methodInfo = value as MethodInfo;
            if (methodInfo == null)
            {
                return string.Empty;
            }

            return string.Format("{0}({1})", methodInfo.Name,
                                 string.Join(", ", methodInfo.GetParameters().Select(p => p.Name)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
