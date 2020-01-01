using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MultiMediaPlayer
{
    class NameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var info = value as string;
            int pos = info.LastIndexOf('.');
            //var tokens = info.Split(new string[] { "." },
            //    StringSplitOptions.None);
            var ext = info.Substring(pos);
            var name = info.Substring(0, info.Length - ext.Length);
            return name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
