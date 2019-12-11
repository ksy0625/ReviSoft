using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using RevisoftApplication;
using System.Data;

namespace UserControls
{
	public partial class uc_Excel_TariffaProfessionale : UserControl
    {
        public int id;
        //private DataTable dati = null;
        private string _ID = "-1";
        CultureInfo culture = CultureInfo.CreateSpecificCulture("it-IT");

		public uc_Excel_TariffaProfessionale()
        {   
            InitializeComponent();
        }
        public void Load(string ID, string IDCliente,  string IDSessione)
        {
            id = int.Parse(ID);
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
            _ID = ID;

            //Binding b = new Binding();
            //b.Source = _x.xdp;
            //b.XPath = "/Dati/Dato[@ID=" + ID + "]/Valore/@value";
            //txtTitolo.SetBinding(TextBlock.TextProperty, b);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ;
            //if (txtValore.Text == "Titolo")
            //{
            //    txtValore.Text = "";
            //}

            //try
            //{
            //    XmlNode tmp = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]/Valore");
            //    tmp.Attributes["name"].Value = txtValore.Text;

            //    _x.Save();
            //}
            //catch (Exception ex)
            //{
            //    string log = ex.Message;
            //}

        }

        private void txtLetteraA_TextChanged(object sender, TextChangedEventArgs e)
        {
            double value = 0.0;

            try 
	        {	        
		        value = Convert.ToDouble(((TextBox)(sender)).Text);
	        }
	        catch (Exception ex)
	        {
                cBusinessObjects.logger.Error(ex, "wWorkArea_Excel_TariffaProfessionale.txtLetteraA_TextChanged exception");
                string log = ex.Message;
                value = 0.0;
	        }  

            double scaglione1 = 258228.44; 
            double scaglione2 = 2582284.49; 
            double scaglione3 = 25822844.94;

            double scaglione1Da = 929.63;
            double scaglione2Da = 929.64;
            double scaglione3Da = 1859.26;	

#pragma warning disable CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato
            double scaglione1A = 929.63;
#pragma warning restore CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato
            double scaglione2A = 1859.25;
            double scaglione3A = 3718.49;

            int fascia = 0;

            if (value <= scaglione1)
            {
                fascia = 1;
            }
            else if (value <= scaglione2 && value > scaglione1)
            {
                fascia = 2;
            }
            else if (value <= scaglione3 && value > scaglione2)
            {
                fascia = 3;
            }
            else if (value > scaglione3)
            {
                fascia = 4;
            }

            double frazioni = 0.0;

            if (fascia == 4)
            {
                frazioni = 799.99 * Math.Ceiling((value - 25822844.94) / 10000000.0);
            }

            double amin = 0;

            if (fascia == 1)
            {
                amin = scaglione1;
            }
            else if (fascia == 2)
            {
                amin = scaglione1;
            }
            else if (fascia == 3)
            {
                amin = scaglione2;
            }

            double amax = 0;

            if (fascia == 1)
            {
                amax = scaglione1;
            }
            else if (fascia == 2)
            {
                amax = scaglione2;
            }
            else if (fascia == 3)
            {
                amax = scaglione3;
            }

            double tmin = 0;

            if (fascia == 1)
            {
                tmin = 0;
            }
            else if (fascia == 2)
            {
                tmin = scaglione2Da;
            }
            else if (fascia == 3)
            {
                tmin = scaglione3Da;
            }

            double tmax = 0;

            if (fascia == 1)
            {
                tmax = 0;
            }
            else if (fascia == 2)
            {
                tmax = scaglione2A;
            }
            else if (fascia == 3)
            {
                tmax = scaglione3A;
            }

            double c1 = 0.0;

            if (fascia == 1 || fascia == 4)
            {
                c1 = 0;
            }
            else
            {
                c1 = (amax - amin) / (tmax - tmin);
            }

            double c2 = 0.0;

            if (fascia == 1 || fascia == 4)
            {
                c2 = 0;
            }
            else
            {
                c2 = (amax - value) / c1;
            }

            double valorefinale = 0.0;

            if (fascia == 1)
            {
                valorefinale = scaglione1Da;
            }
            else if (fascia == 4)
            {
                valorefinale = scaglione3A + frazioni;
            }
            else
            {
                valorefinale = -(c2 - tmax);
            }

            txtCompensoA.Text = String.Format("{0:0,0.00}", valorefinale);
        }

        private void txt_LostFocus(object sender, RoutedEventArgs e)
        {
            double value = 0.0;

            try
            {
                value = Convert.ToDouble(((TextBox)(sender)).Text);
            }
            catch (Exception ex)
            {
                cBusinessObjects.logger.Error(ex, "wWorkArea_Excel_TariffaProfessionale.txt_LostFocus exception");
                string log = ex.Message;
                value = 0.0;
            }
            
            ((TextBox)(sender)).Text = String.Format("{0:0,0.00}", value);
        }

        private void txtLetteraB_TextChanged(object sender, TextChangedEventArgs e)
        {
            double value = 0.0;

            try
            {
                value = Convert.ToDouble(((TextBox)(sender)).Text);
            }
            catch (Exception ex)
            {
                cBusinessObjects.logger.Error(ex, "wWorkArea_Excel_TariffaProfessionale.txtLetteraB_TextChanged exception");
                string log = ex.Message;
                value = 0.0;
            }

            double scaglione1 = 10000.00;	  
            double scaglione2 = 119999.99; 	
            double scaglione3 = 516456.89; 	
            double scaglione4 = 2582284.49; 
            double scaglione5 = 10329137.97;

            double scaglione1Da = 774.69;
            double scaglione2Da = 1162.05;
            double scaglione3Da = 1936.72;
            double scaglione4Da = 3098.76;
            double scaglione5Da = 4648.12;

            double scaglione1A = 1162.04;
            double scaglione2A = 1936.71;
            double scaglione3A = 3098.75;
            double scaglione4A = 4648.11;

            int fascia = 0;

            if (value <= scaglione2 && value > scaglione1)
            {
                fascia = 1;
            }
            else if (value <= scaglione3 && value > scaglione2)
            {
                fascia = 2;
            }
            else if (value <= scaglione4 && value > scaglione3)
            {
                fascia = 3;
            }
            else if (value <= scaglione5 && value > scaglione4)
            {
                fascia = 4;
            }
            else if (value > scaglione5)
            {
                fascia = 5;
            }

            double frazioni = 0.0;

            if (fascia == 5)
            {
                frazioni = 774.69 * Math.Ceiling((value - 10392137.98) / 5164568.99);
            }

            double amin = 0;

            if (fascia == 1)
            {
                amin = scaglione1;
            }
            else if (fascia == 2)
            {
                amin = scaglione2;
            }
            else if (fascia == 3)
            {
                amin = scaglione3;
            }
            else if (fascia == 4)
            {
                amin = scaglione4;
            }
            else if (fascia == 5)
            {
                amin = scaglione5;
            }

            double amax = 0;

            if (fascia == 1)
            {
                amax = scaglione2;
            }
            else if (fascia == 2)
            {
                amax = scaglione3;
            }
            else if (fascia == 3)
            {
                amax = scaglione4;
            }
            else if (fascia == 4)
            {
                amax = scaglione5;
            }
            else if (fascia == 5)
            {
                amax = scaglione5;
            }

            double tmin = 0;

            if (fascia == 1)
            {
                tmin = scaglione1Da;
            }
            else if (fascia == 2)
            {
                tmin = scaglione2Da;
            }
            else if (fascia == 3)
            {
                tmin = scaglione3Da;
            }
            else if (fascia == 4)
            {
                tmin = scaglione4Da;
            }
            else if (fascia == 5)
            {
                tmin = scaglione5Da;
            }

            double tmax = 0;

            if (fascia == 1)
            {
                tmax = scaglione1A;
            }
            else if (fascia == 2)
            {
                tmax = scaglione2A;
            }
            else if (fascia == 3)
            {
                tmax = scaglione3A;
            }
            else if (fascia == 4)
            {
                tmax = scaglione4A;
            }

            double c1 = 0.0;

            if (fascia == 5)
            {
                c1 = 0;
            }
            else
            {
                c1 = (amax - amin) / (tmax - tmin);
            }

            double c2 = 0.0;

            if (fascia == 5)
            {
                c2 = 0;
            }
            else
            {
                c2 = (amax - value) / c1;
            }

            double importo = 0.0;

            if(fascia == 5)
            {
                importo = scaglione5Da + frazioni;
            }
            else
            {
                importo = -(c2 - tmax);
            }            

            double valorefinale = 0.0;

            if (value < 10000)
            {
                valorefinale = scaglione1Da;
            }
            else if (importo > 60000)
            {
                valorefinale = 60000;
            }
            else
            {
                valorefinale = importo;
            }

            txtCompensoB.Text = String.Format("{0:0,0.00}", valorefinale);
        }
    }
}
