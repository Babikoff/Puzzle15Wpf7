using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Puzzle15.Interfaces;

namespace Puzzle15.Wpf.Converters
{
    public class ShowCellModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {   //targetType - type of WPF control prop
            if (!(value is ShowCellModeEnum)) return 0;
            //if (!(targetType.Equals(typeof(int)))) return 0;

            ShowCellModeEnum showCellMode = (ShowCellModeEnum)value;
            return (int)showCellMode;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {   // targetType - type of source (model) object prop
            if (!(targetType.Equals(typeof(ShowCellModeEnum)))) return ShowCellModeEnum.DigitsAndPictures;
            int intValue = 0;
            if (!int.TryParse(value.ToString(), out intValue)) return ShowCellModeEnum.DigitsAndPictures;

            object enumObj = Enum.ToObject(typeof(ShowCellModeEnum), intValue);
            if (enumObj == null) return ShowCellModeEnum.DigitsAndPictures;

            return (ShowCellModeEnum)enumObj;
        }
    }
}
