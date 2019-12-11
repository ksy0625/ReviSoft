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
	public partial class ucExcel_CassaTitoli : UserControl
    {
        public int id;
        private DataTable dati=null;
        //private XmlDataProviderManager _x = null;
        private string _ID = "";

		private bool _ReadOnly = false;
        
        GenericTable gtCassaTitoli = null;

        public ucExcel_CassaTitoli()
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
            dati = cBusinessObjects.GetData(id, typeof(CassaTitoli));

            foreach (DataRow dtrow in dati.Rows)
            {
                if (String.IsNullOrEmpty(dtrow["CreditoEsistente"].ToString()))
                    txtCreditoEsistente.Text = "";
                else
                    txtCreditoEsistente.Text = dtrow["CreditoEsistente"].ToString();
            }

           

            gtCassaTitoli = new GenericTable( tblCassaTitoli, _ReadOnly);

            gtCassaTitoli.ColumnsAlias = new string[] { "Titolo", "Scadenza", "Euro" };
            gtCassaTitoli.ColumnsValues = new string[] { "name", "codice", "importoPagato" };
            gtCassaTitoli.ColumnsWidth = new double[] { 3.0, 1.0, 1.0 };
            gtCassaTitoli.ColumnsMinWidth = new double[] { 0.0, 0.0, 0.0 };
            gtCassaTitoli.ColumnsTypes = new string[] { "string", "string", "money" };
            gtCassaTitoli.ColumnsAlignment = new string[] { "left", "right", "right" };
            gtCassaTitoli.ColumnsReadOnly = new bool[] { false, false, false };
            gtCassaTitoli.ConditionalReadonly = new bool[] { false, false, false };
            gtCassaTitoli.ConditionalAttribute = "new";
            gtCassaTitoli.ColumnsHasTotal = new bool[] { false, false, true };
            gtCassaTitoli.AliasTotale = "Totale";
            gtCassaTitoli.ColumnAliasTotale = 1;

            gtCassaTitoli.dati = dati;
            gtCassaTitoli.xml = false;
            gtCassaTitoli.GenerateTable();
            
        }
        public int Save()
        {
            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["CreditoEsistente"] = txtCreditoEsistente.Text;
            }
            return cBusinessObjects.SaveData(id, dati, typeof(CassaTitoli));
        }
        

        private void AddRowErroriRilevati(object sender, RoutedEventArgs e)
        {
            dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            gtCassaTitoli.GenerateTable();
        }

        private void DeleteRowErroriRilevati(object sender, RoutedEventArgs e)
        {
            gtCassaTitoli.DeleteRow();
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
                dtrow["CreditoEsistente"] = txtCreditoEsistente.Text;
            }

		}
    }
}
