using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace GameOfLife.Converters
{
    class BoolToBrushConverter : IValueConverter
    {
        public Brush TrueBrush { get; set; } = Brushes.Black;
        public Brush FalseBrush { get; set; } = Brushes.White;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Console.WriteLine(value.GetType() );
            bool valBool = (bool)value;
            Brush brush = null;
            if(targetType == typeof(Brush))
            {
                 brush = valBool ? TrueBrush : FalseBrush;
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Brush)value).Equals(TrueBrush);
        }
    }
}
