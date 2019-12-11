using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RevisoftApplication.BRL;

namespace RevisoftApplication.Converter
{
	public class RuoloConverter : System.Windows.Data.IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Ruolo ruo = (Ruolo)value;
			return ruo.Id;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			short val = (short)value;
			return val;
		}
	}
}
