using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ProductsDishes
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, System.Globalization.CultureInfo c)
            => value != null ? Visibility.Visible : Visibility.Collapsed;
        public object ConvertBack(object v, Type t, object p, System.Globalization.CultureInfo c)
            => throw new NotImplementedException();
    }
}
