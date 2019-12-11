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
	public partial class ucExcel_CassaValoriBollati : UserControl
    {
          public int id;
          private DataTable dati = null;
          private DataTable dati2 = null;

          //private XmlDataProviderManager _x = null;
          private string _ID = "";

	     	private bool _ReadOnly = false;


        GenericTable gtCassaValoriBollati = null;
        GenericTable gtCassaValoriBollati2 = null;

        public ucExcel_CassaValoriBollati()
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

           _ID =  ID;
         

          dati = cBusinessObjects.GetData(id, typeof(CassaValoriBollati_Francobolli));

          foreach (DataRow dtrow in dati.Rows)
            {
              if (String.IsNullOrEmpty(dtrow["CreditoEsistente"].ToString()))
                txtCreditoEsistente.Text = "";
                    else
                txtCreditoEsistente.Text = dtrow["CreditoEsistente"].ToString();
              if (String.IsNullOrEmpty(dtrow["txtSaldoSchedaContabile"].ToString()))
                txtSaldoSchedaContabile.Text = "";
                    else
                txtSaldoSchedaContabile.Text = dtrow["txtSaldoSchedaContabile"].ToString();
            }
          

            gtCassaValoriBollati = new GenericTable( tblCassaValoriBollati, _ReadOnly);

            gtCassaValoriBollati.ColumnsAlias = new string[] { "N° Pezzi", "Unitario", "Euro" };
            gtCassaValoriBollati.ColumnsValues = new string[] { "numeropezzi", "unitario", "euro" };
            gtCassaValoriBollati.ColumnsWidth = new double[] { 1.0, 1.0, 1.0 };
            gtCassaValoriBollati.ColumnsMinWidth = new double[] { 0.0, 0.0, 0.0 };
            gtCassaValoriBollati.ColumnsTypes = new string[] { "int", "money", "money" };
            gtCassaValoriBollati.ColumnsAlignment = new string[] { "right", "right", "right" };
            gtCassaValoriBollati.ColumnsReadOnly = new bool[] { false, false, true };
            gtCassaValoriBollati.ConditionalReadonly = new bool[] { false, false, true };
            gtCassaValoriBollati.ConditionalAttribute = "new";
            gtCassaValoriBollati.ColumnsHasTotal = new bool[] { false, false, true };
            gtCassaValoriBollati.AliasTotale = "Totale";
            gtCassaValoriBollati.ColumnAliasTotale = 1;
            gtCassaValoriBollati.TotalToBeCalculated += GtCassaValoriBollati_TotalToBeCalculated;

            gtCassaValoriBollati.dati = dati;
            gtCassaValoriBollati.xml = false;
            gtCassaValoriBollati.Xpath = "/Dati/Dato[@ID" + _ID + "]/Valore[@tipo='CassaValoriBollati_Francobolli']";
            gtCassaValoriBollati.GenerateTable();

            dati2 = cBusinessObjects.GetData(id, typeof(CassaValoriBollati_Marche));

            gtCassaValoriBollati2 = new GenericTable( tblCassaValoriBollati2, _ReadOnly);

            gtCassaValoriBollati2.ColumnsAlias = new string[] { "N° Pezzi", "Unitario", "Euro" };
            gtCassaValoriBollati2.ColumnsValues = new string[] { "numeropezzi", "unitario", "euro" };
            gtCassaValoriBollati2.ColumnsWidth = new double[] { 1.0, 1.0, 1.0 };
            gtCassaValoriBollati2.ColumnsMinWidth = new double[] { 0.0, 0.0, 0.0 };
            gtCassaValoriBollati2.ColumnsTypes = new string[] { "int", "money", "money" };
            gtCassaValoriBollati2.ColumnsAlignment = new string[] { "right", "right", "right" };
            gtCassaValoriBollati2.ColumnsReadOnly = new bool[] { false, false, true };
            gtCassaValoriBollati2.ConditionalReadonly = new bool[] { false, false, true };
            gtCassaValoriBollati2.ConditionalAttribute = "new";
            gtCassaValoriBollati2.ColumnsHasTotal = new bool[] { false, false, true };
            gtCassaValoriBollati2.AliasTotale = "Totale";
            gtCassaValoriBollati2.ColumnAliasTotale = 1;
            gtCassaValoriBollati2.dati = dati2;
            gtCassaValoriBollati2.xml = false;
            gtCassaValoriBollati2.TotalToBeCalculated += GtCassaValoriBollati2_TotalToBeCalculated;

            gtCassaValoriBollati2.Xpath = "/Dati/Dato[@ID" + _ID + "]/Valore[@tipo='CassaValoriBollati_Marche']";
            gtCassaValoriBollati2.GenerateTable();

            GenerateGrandTotal();
        }
        
        private void GtCassaValoriBollati_TotalToBeCalculated(object sendername, EventArgs e)
        {
            if (((string)sendername).Split('_').Count() < 2)
            {
                return;
            }

            string idcolumn = ((string)sendername).Split('_')[1];
            string idrow = ((string)sendername).Split('_')[2];

            if (idcolumn == "0" || idcolumn == "1")
            {
                double unitario = 0.0;
                double numeropezzi = 0.0;
                double euro = 0.0;

                double.TryParse(gtCassaValoriBollati.GetValue("1", idrow), out unitario);
                double.TryParse(gtCassaValoriBollati.GetValue("0", idrow), out numeropezzi);

                euro = unitario * numeropezzi;

                gtCassaValoriBollati.SetValue("2", idrow, cBusinessObjects.ConvertNumber(euro.ToString()));
            }

            GenerateGrandTotal();
        }

        private void GtCassaValoriBollati2_TotalToBeCalculated(object sendername, EventArgs e)
        {
            if (((string)sendername).Split('_').Count() < 2)
            {
                return;
            }

            string idcolumn = ((string)sendername).Split('_')[1];
            string idrow = ((string)sendername).Split('_')[2];

            if (idcolumn == "0" || idcolumn == "1")
            {
                double unitario = 0.0;
                double numeropezzi = 0.0;
                double euro = 0.0;

                double.TryParse(gtCassaValoriBollati2.GetValue("1", idrow), out unitario);
                double.TryParse(gtCassaValoriBollati2.GetValue("0", idrow), out numeropezzi);

                euro = unitario * numeropezzi;

                gtCassaValoriBollati2.SetValue("2", idrow, cBusinessObjects.ConvertNumber(euro.ToString()));
            }

            GenerateGrandTotal();
        }

    public int Save()
    {
      GenerateGrandTotal();
      foreach (DataRow dtrow in dati.Rows)
      {
                dtrow["CreditoEsistente"] = txtCreditoEsistente.Text;
                dtrow["txtTotaleComplessivo"] = txtTotaleComplessivo.Text;
                dtrow["txtDifferenza"] = txtDifferenza.Text;
      }


      cBusinessObjects.SaveData(id, dati, typeof(CassaValoriBollati_Francobolli));
      return cBusinessObjects.SaveData(id, dati2, typeof(CassaValoriBollati_Marche));

    }


 
		private void GenerateGrandTotal()
		{
            double totale = 0.0;
            double.TryParse(gtCassaValoriBollati.GenerateSpecificTotal("2"), out totale);

            double totale2 = 0.0;
            double.TryParse(gtCassaValoriBollati2.GenerateSpecificTotal("2"), out totale2);

			txtTotaleComplessivo.Text = cBusinessObjects.ConvertNumber((totale + totale2).ToString());

			double saldo = 0.0;
			double.TryParse(txtSaldoSchedaContabile.Text, out saldo);

			txtDifferenza.Text = cBusinessObjects.ConvertNumber((totale + totale2 - saldo).ToString());
		}

		#region francobolli
		private void AggiungiNodo(string Alias, string ID, string Codice)
        {
         //   gtCassaValoriBollati.Xpathparentnode = "//Dati/Dato[@ID" + _ID + "]";
        //    gtCassaValoriBollati.TemplateNewNode = "<Valore tipo=\"CassaValoriBollati_Francobolli\" " + ((Alias == "") ? " new=\"true\" " : " ") + " numeropezzi=\"\" unitario=\"\" euro=\"\" />";
            DataRow dd= dati.Rows.Add(id,cBusinessObjects.idcliente,cBusinessObjects.idsessione);
            dd["isnew"] = 1;
            gtCassaValoriBollati.GenerateTable();
        }
        
		private void AddRowErroriRilevati(object sender, RoutedEventArgs e)
		{
			AggiungiNodo("", _ID, "");
		}

		private void DeleteRowErroriRilevati(object sender, RoutedEventArgs e)
		{
            gtCassaValoriBollati.DeleteRow();
		}
		#endregion

		#region marche
		private void AggiungiNodo2(string Alias, string ID, string Codice)
		{
            //      gtCassaValoriBollati2.Xpathparentnode = "//Dati/Dato[@ID" + _ID + "]";
            //     gtCassaValoriBollati2.TemplateNewNode = "<Valore tipo=\"CassaValoriBollati_Marche\" " + ((Alias == "") ? " new=\"true\" " : " ") + " numeropezzi=\"\" unitario=\"\" euro=\"\" />";
          
            DataRow dd = dati2.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            dd["isnew"] = 1;
            gtCassaValoriBollati2.GenerateTable();
        }
     
		private void AddRowErroriRilevati2(object sender, RoutedEventArgs e)
		{
			AggiungiNodo2("", _ID, "");
		}

		private void DeleteRowErroriRilevati2(object sender, RoutedEventArgs e)
		{
            gtCassaValoriBollati2.DeleteRow();
    }
		#endregion

		
        
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

		private void txtSaldoSchedaContabile_LostFocus(object sender, RoutedEventArgs e)
		{
			txtSaldoSchedaContabile.Text = cBusinessObjects.ConvertNumber(txtSaldoSchedaContabile.Text);
            foreach (DataRow dtrow in dati.Rows)
              {
                dtrow["txtSaldoSchedaContabile"] = txtSaldoSchedaContabile.Text;
              }
 
	        	GenerateGrandTotal();
			
		}

    }
}
