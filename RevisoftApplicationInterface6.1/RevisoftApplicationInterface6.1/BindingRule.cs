using System;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;

namespace BindValidation
{
    public class GGMMRule : ValidationRule
    {
		public GGMMRule()
        {
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
			string tmp = (string)value;

            try
            {
				if (tmp.Length > 0)
				{
					int i = Convert.ToInt32(tmp.Replace("/", ""));
				}
            }
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
            catch (Exception e)
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata
            {
				return new ValidationResult(false, "Formato 'GG/MM'");
            }

			string[] splitted = tmp.Split('/');

			if (splitted.Length != 2)
			{
				return new ValidationResult(false, "Formato 'GG/MM'");
			}

			try
            {
				DateTime dt = Convert.ToDateTime(tmp + "/2012");
			}
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
			catch (Exception e)
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata
			{
				return new ValidationResult(false, "data non valida");
			}

			return new ValidationResult(true, null);
        }
    }

	public class HHMMRule : ValidationRule
	{
		public HHMMRule()
		{
		}

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			string tmp = (string)value;

			if (tmp != "")
			{
				try
				{
					if (tmp.Length > 0)
					{
						int i = Convert.ToInt32(tmp.Replace(":", ""));
					}
				}
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
				catch (Exception e)
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata
				{
					return new ValidationResult(false, "Formato 'HH:MM'");
				}

				string[] splitted = tmp.Split(':');

				if (splitted.Length != 2)
				{
					return new ValidationResult(false, "Formato 'HH:MM'");
				}

				try
				{
					DateTime dt = Convert.ToDateTime("01/01/2012 " + tmp);
				}
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
				catch (Exception e)
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata
				{
					return new ValidationResult(false, "orario non valido");
				}
			}

			return new ValidationResult(true, null);
		}
	}

	public class MyDataSource
	{
		private string valoreDal;
		private string valoreAl;
		private string inizioSeduta;
		private string fineSeduta;

		public MyDataSource()
		{
		}

		public string ValoreDal
		{
			get { return valoreDal; }
			set { valoreDal = value; }
		}

		public string ValoreAl
		{
			get { return valoreAl; }
			set { valoreAl = value; }
		}

		public string InizioSeduta
		{
			get { return inizioSeduta; }
			set { inizioSeduta = value; }
		}

		public string FineSeduta
		{
			get { return fineSeduta; }
			set { fineSeduta = value; }
		}
	}
}
