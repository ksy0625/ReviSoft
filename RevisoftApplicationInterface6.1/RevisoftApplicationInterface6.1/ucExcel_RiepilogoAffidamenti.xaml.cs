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
using System.Collections;
using System.Threading;
using RevisoftApplication;
using System.Data;

namespace UserControls
{
	public partial class ucExcel_RiepilogoAffidamenti : UserControl
    {
        public int id;
        private DataTable dati = null;
       
        //private string _ID = "";

		public ucExcel_RiepilogoAffidamenti()
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("it-IT"); 
            InitializeComponent();
        }

        public void LoadDataSource(string ID, string IDCliente, string IDSessione)
        {

            id = int.Parse(ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
         
            
            dati = cBusinessObjects.GetData(id, typeof(Excel_Affidamenti));
            int row = 0;

			Border brd;
			TextBlock txt;
			RowDefinition rd;

			Hashtable htAlias = new Hashtable();
			htAlias.Add("a", "conto corrente");
			htAlias.Add("b", "sbf - riba");
			htAlias.Add("c", "anticipo fatture");
			htAlias.Add("d", "anticipo export");
			htAlias.Add("e", "anticipo import");
			htAlias.Add("f", "chirografario");
			htAlias.Add("g", "mutui ipotecari o similari");
			htAlias.Add("h", "operazioni a termine");
			htAlias.Add("i", "finanza derivata");
			htAlias.Add("l", "garanzie prestate");
			htAlias.Add("m", "altro 1");
			htAlias.Add("n", "altro 2");

			Hashtable htInizio = new Hashtable();
			Hashtable htFine = new Hashtable();
			Hashtable htUtilizzo = new Hashtable();

            foreach (DataRow dtrow in dati.Rows)
            {
				if (dtrow["tipoaffidamento"] == null)
				{
					continue;
				}

				if (dtrow["tipoaffidamento"].ToString() == "")
				{
					continue;
				}

				if (!htInizio.Contains(dtrow["tipoaffidamento"].ToString()))
				{
					htInizio.Add(dtrow["tipoaffidamento"].ToString(), 0.0);
				}

				if (dtrow["inizio"] != null)
				{
					double value = 0.0;
					double.TryParse(dtrow["inizio"].ToString(), out value);
					htInizio[dtrow["tipoaffidamento"].ToString()] = (double)htInizio[dtrow["tipoaffidamento"].ToString()] + value;
				}

				if (!htFine.Contains(dtrow["tipoaffidamento"].ToString()))
				{
					htFine.Add(dtrow["tipoaffidamento"].ToString(), 0.0);
				}

				if (dtrow["dataverifica"] != null)
				{
					double value = 0.0;
					double.TryParse(dtrow["dataverifica"].ToString(), out value);
					htFine[dtrow["tipoaffidamento"].ToString()] = (double)htFine[dtrow["tipoaffidamento"].ToString()] + value;
				}

				if (!htUtilizzo.Contains(dtrow["tipoaffidamento"].ToString()))
				{
					htUtilizzo.Add(dtrow["tipoaffidamento"].ToString(), 0.0);
				}

				if (dtrow["utilizzo"] != null)
				{
					double value = 0.0;
					double.TryParse(dtrow["utilizzo"].ToString(), out value);
					htUtilizzo[dtrow["tipoaffidamento"].ToString()] = (double)htUtilizzo[dtrow["tipoaffidamento"].ToString()] + value;
				}
			}

			foreach (DictionaryEntry item in htAlias)
			{
				rd = new RowDefinition();
				grdRiepilogo.RowDefinitions.Add(rd);
				row++;

				brd = new Border();
				brd.BorderThickness = new Thickness(1.0);

				brd.BorderBrush = Brushes.Black;

				txt = new TextBlock();
				txt.Text = item.Value.ToString();
				txt.TextAlignment = TextAlignment.Left;
				txt.TextWrapping = TextWrapping.Wrap;

				brd.Child = txt;

				grdRiepilogo.Children.Add(brd);
				Grid.SetRow(brd, row);
				Grid.SetColumn(brd, 0);

				brd = new Border();
				brd.BorderThickness = new Thickness(1.0);

				brd.BorderBrush = Brushes.Black;

				txt = new TextBlock();
				double valore = 0.0;
				if (htInizio.Contains(item.Key.ToString()))
				{
					valore = (double)htInizio[item.Key.ToString()];
				}
				txt.Text = cBusinessObjects.ConvertNumber(valore.ToString());
				txt.TextAlignment = TextAlignment.Left;
				txt.TextWrapping = TextWrapping.Wrap;

				brd.Child = txt;

				grdRiepilogo.Children.Add(brd);
				Grid.SetRow(brd, row);
				Grid.SetColumn(brd, 1);

				brd = new Border();
				brd.BorderThickness = new Thickness(1.0);

				brd.BorderBrush = Brushes.Black;

				txt = new TextBlock();
				valore = 0.0;
				if (htFine.Contains(item.Key.ToString()))
				{
					valore = (double)htFine[item.Key.ToString()];
				}
				txt.Text = cBusinessObjects.ConvertNumber(valore.ToString());
				txt.TextAlignment = TextAlignment.Left;
				txt.TextWrapping = TextWrapping.Wrap;

				brd.Child = txt;

				grdRiepilogo.Children.Add(brd);
				Grid.SetRow(brd, row);
				Grid.SetColumn(brd, 2);

				brd = new Border();
				brd.BorderThickness = new Thickness(1.0);

				brd.BorderBrush = Brushes.Black;

				txt = new TextBlock();
				valore = 0.0;
				if (htUtilizzo.Contains(item.Key.ToString()))
				{
					valore = (double)htUtilizzo[item.Key.ToString()];
				}
				txt.Text = cBusinessObjects.ConvertNumber(valore.ToString());
				txt.TextAlignment = TextAlignment.Left;
				txt.TextWrapping = TextWrapping.Wrap;

				brd.Child = txt;

				grdRiepilogo.Children.Add(brd);
				Grid.SetRow(brd, row);
				Grid.SetColumn(brd, 3);
			}

			rd = new RowDefinition();
			grdRiepilogo.RowDefinitions.Add(rd);
			row++;

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.Background = Brushes.Gray;
			brd.BorderBrush = Brushes.Black;

			txt = new TextBlock();
			txt.Text = "Totale";
			txt.TextAlignment = TextAlignment.Left;
			txt.TextWrapping = TextWrapping.Wrap;

			brd.Child = txt;

			grdRiepilogo.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 0);

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.Background = Brushes.Gray;
			brd.BorderBrush = Brushes.Black;

			txt = new TextBlock();

			double somma = 0.0;
			foreach (DictionaryEntry item in htInizio)
			{
				somma += (double)(item.Value);
			}
			txt.Text = cBusinessObjects.ConvertNumber(somma.ToString());
			txt.TextAlignment = TextAlignment.Left;
			txt.TextWrapping = TextWrapping.Wrap;

			brd.Child = txt;

			grdRiepilogo.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 1);

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.Background = Brushes.Gray;
			brd.BorderBrush = Brushes.Black;

			txt = new TextBlock();
			somma = 0.0;
			foreach (DictionaryEntry item in htFine)
			{
				somma += (double)(item.Value);
			}
			txt.Text = cBusinessObjects.ConvertNumber(somma.ToString());
			txt.TextAlignment = TextAlignment.Left;
			txt.TextWrapping = TextWrapping.Wrap;

			brd.Child = txt;

			grdRiepilogo.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 2);

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.Background = Brushes.Gray;
			brd.BorderBrush = Brushes.Black;

			txt = new TextBlock();
			somma = 0.0;
			foreach (DictionaryEntry item in htUtilizzo)
			{
				somma += (double)(item.Value);
			}
			txt.Text = cBusinessObjects.ConvertNumber(somma.ToString());
			txt.TextAlignment = TextAlignment.Left;
			txt.TextWrapping = TextWrapping.Wrap;

			brd.Child = txt;

			grdRiepilogo.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 3);
        }

		
    }
}
