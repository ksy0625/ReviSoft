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

    public partial class ucExcel_CassaAssegni : UserControl
    {
        public int id;
    

        private DataTable dati = null;
        //private XmlDataProviderManager _x = null;
        private string _ID = "";

		    private bool _ReadOnly = false;

        GenericTable gtCassaAssegni = null;

        public ucExcel_CassaAssegni()
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("it-IT"); 
            InitializeComponent();
        }

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

            if (ID != "-1" && ID != "")
            {
                _ID = "=" + ID + "";
            }

            dati = cBusinessObjects.GetData(id, typeof(CassaAssegni));
       
            foreach (DataRow dtrow in dati.Rows)
            {
                if (String.IsNullOrEmpty(dtrow["PeriodoDiRiferimento"].ToString()))
                    txtPeriodoDiRiferimento.Text = "";
                else
                    txtPeriodoDiRiferimento.Text = dtrow["PeriodoDiRiferimento"].ToString();
            }


            gtCassaAssegni = new GenericTable( tblCassaAssegni, _ReadOnly);

            gtCassaAssegni.ColumnsAlias = new string[] { "Traente", "Banca", "Piazza", "Importo" };
            gtCassaAssegni.ColumnsValues = new string[] { "name", "codice", "importoPagato", "importoCompensato" };
            gtCassaAssegni.ColumnsWidth = new double[] {3.0, 1.0, 1.0, 1.0 };
            gtCassaAssegni.ColumnsMinWidth = new double[] { 0.0, 0.0, 0.0, 0.0 };
            gtCassaAssegni.ColumnsTypes = new string[] { "string", "string", "money", "money" };
            gtCassaAssegni.ColumnsAlignment = new string[] { "left", "left", "right", "right" };
            gtCassaAssegni.ConditionalReadonly = new bool[] { false, false, false, false };
            gtCassaAssegni.ConditionalAttribute = "new";
            gtCassaAssegni.ColumnsHasTotal = new bool[] { false, false, true, true };
            gtCassaAssegni.AliasTotale = "Totale";
            gtCassaAssegni.ColumnAliasTotale = 1;

        
            gtCassaAssegni.dati = dati;
            gtCassaAssegni.xml = false;

            gtCassaAssegni.Xpath = "/Dati/Dato[@ID" + _ID + "]/Valore[@tipo='CassaAssegni']";
            
            gtCassaAssegni.GenerateTable();
        }

	   

        public int Save()
        {
            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["PeriodoDiRiferimento"] = txtPeriodoDiRiferimento.Text;
            }
            return cBusinessObjects.SaveData(id, dati, typeof(CassaAssegni));
        }
        

        private void AddRowErroriRilevati(object sender, RoutedEventArgs e)
        {
            DataRow dd= dati.Rows.Add(id,cBusinessObjects.idcliente,cBusinessObjects.idsessione);
            dd["isnew"] = 1;
            gtCassaAssegni.GenerateTable();
        }

        private void DeleteRowErroriRilevati(object sender, RoutedEventArgs e)
        {
            gtCassaAssegni.DeleteRow();
            return;
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

		private void txtPeriodoDiRiferimento_LostFocus(object sender, RoutedEventArgs e)
		{
            foreach (DataRow dtrow in dati.Rows)
            {
                 dtrow["PeriodoDiRiferimento"]= txtPeriodoDiRiferimento.Text;
            }
            
		}

    }
}
