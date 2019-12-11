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
using System.Xml;
using RevisoftApplication;
using System.Collections;
using System.Data;


namespace UserControls
{

    public partial class ucIndipendenzaFinanziaria : UserControl
    {
        public int id;
        private DataTable dati = null;
   
	
		public ucIndipendenzaFinanziaria()
        {
            InitializeComponent();            
        }

        private bool _ReadOnly = false;

        public bool ReadOnly 
        {
            set
            {
                _ReadOnly = value;
            }
        }
        public void LoadDataSource(string ID, string IDCliente, string IDSessione)
        {
            id = int.Parse(ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
    

            dati = cBusinessObjects.GetData(id, typeof(IndipendenzaFinanziaria));

            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["txtCT"] != null)
				{
					txtCT.Text = ConvertNumber(dtrow["txtCT"].ToString());
				}

				if (dtrow["txtC"] != null)
				{
					txtC.Text = ConvertNumber(dtrow["txtC"].ToString());
				}

				if (dtrow["txtS"] != null)
				{
					txtS.Text = ConvertNumber(dtrow["txtS"].ToString());
				}
			}

			UpdateData();
        }

		public int Save()
		{
            if(dati.Rows.Count==0)
            {
                dati.Rows.Add(id,cBusinessObjects.idcliente,cBusinessObjects.idsessione,0,0,0,0,0);
            }
            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["txtCT"] = txtCT.Text;
                dtrow["txtC"] = txtC.Text;
                dtrow["txtS"] = txtS.Text;
                dtrow["txtSCCT"] = txtSCCT.Text;
                dtrow["txtSSC"] = txtSSC.Text;
                dtrow["txtFascia"] = txtFascia.Text;
                dtrow["txtValutazione"] = txtValutazione.Text;
            }

            return cBusinessObjects.SaveData(id, dati, typeof(IndipendenzaFinanziaria));
        }

		private void UpdateData()
		{
			double dblC = 0.0;
			double.TryParse(txtC.Text, out dblC);

			double dblS = 0.0;
			double.TryParse(txtS.Text, out dblS);

			double dblCT = 0.0;
			double.TryParse(txtCT.Text, out dblCT);

			double dblSCCT = ((dblS + dblC) / dblCT) * 100.0;
			double dblSSC = (dblS / (dblS + dblC)) * 100.0;

			txtSCCT.Text = ConvertPercent(dblSCCT.ToString());
			txtSSC.Text = ConvertPercent(dblSSC.ToString());

			Uri uriSource = null;

			if (dblSCCT > 15)
			{
				if (dblSSC < 66)
				{
					txtFascia.Text = "1 - B";
					txtValutazione.Text = "Dipendente";
					uriSource = new Uri(".\\Images\\Dipendente.png", UriKind.Relative);
					imgDipendenzaIndipendenza.Source = new BitmapImage(uriSource);
				}
				else
				{
					txtFascia.Text = "1 - A";
					txtValutazione.Text = "Indipendente";
					uriSource = new Uri(".\\Images\\Indipendente.png", UriKind.Relative);
					imgDipendenzaIndipendenza.Source = new BitmapImage(uriSource);
				}
			}
			else if (dblSCCT <= 5)
			{
				txtFascia.Text = "3";
				txtValutazione.Text = "Indipendente";
				uriSource = new Uri(".\\Images\\Indipendente.png", UriKind.Relative);
				imgDipendenzaIndipendenza.Source = new BitmapImage(uriSource);
			}
			else
			{
				if (dblSSC < 50)
				{
					txtFascia.Text = "2 - B";
					txtValutazione.Text = "Dipendente";
					uriSource = new Uri(".\\Images\\Dipendente.png", UriKind.Relative);
					imgDipendenzaIndipendenza.Source = new BitmapImage(uriSource);
				}
				else
				{
					txtFascia.Text = "2 - A";
					txtValutazione.Text = "Indipendente";
					uriSource = new Uri(".\\Images\\Indipendente.png", UriKind.Relative);
					imgDipendenzaIndipendenza.Source = new BitmapImage(uriSource);
				}
			}
		}
		
		private string ConvertNumber(string valore)
		{
			double dblValore = 0.0;

			double.TryParse(valore, out dblValore);

			if (dblValore == 0.0)
			{
				return "";
			}
			else
			{
				return String.Format("{0:#,#}", dblValore);
			}
		}

		private string ConvertPercent(string valore)
		{
			double dblValore = 0.0;

			valore = valore.Replace("%", "");

			double.TryParse(valore, out dblValore);

			if (dblValore == 0.0)
			{
				return "";
			}
			else
			{
				return String.Format("{0:0.00}", dblValore) + "%";
			}
		}

		private string ConvertPercentIntero(string valore)
		{
			double dblValore = 0.0;

			valore = valore.Replace("%", "");

			double.TryParse(valore, out dblValore);

			if (dblValore == 0.0)
			{
				return "";
			}
			else
			{
				return String.Format("{0:0}", dblValore) + "%";
			}
		}

		private double ConvertFromPercent(string valore)
		{
			double dblValore = 0.0;

			valore = valore.Replace("%", "");

			double.TryParse(valore, out dblValore);

			return dblValore;
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double newsize = e.NewSize.Width - 30.0;

			try
			{				
				foreach (UIElement item in stack.Children)
				{
					((UserControl)(((Grid)(((Border)(item)).Child)).Children[2])).Width = newsize - 30;
				}

				stack.Width = Convert.ToDouble(newsize);
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}
		}

		private void txt_TextChanged(object sender, TextChangedEventArgs e)
		{
			;
		}

		private void obj_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}
		}

		private void obj_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}
		}

		private void txt_LostFocus(object sender, RoutedEventArgs e)
		{
			((TextBox)(sender)).Text = ConvertNumber(((TextBox)(sender)).Text);
			UpdateData();
		}

		private void txt_LostFocus_perc(object sender, RoutedEventArgs e)
		{
			((TextBox)(sender)).Text = ConvertPercent(((TextBox)(sender)).Text);
			UpdateData();
		}

		private void txt_LostFocus_perc_int(object sender, RoutedEventArgs e)
		{
			((TextBox)(sender)).Text = ConvertPercentIntero(((TextBox)(sender)).Text);
			UpdateData();
		}
    }
}
