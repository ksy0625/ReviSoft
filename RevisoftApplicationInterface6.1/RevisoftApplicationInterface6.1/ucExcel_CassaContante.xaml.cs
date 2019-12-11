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
	public partial class ucExcel_CassaContante : UserControl
    {
        public int id;
        private DataTable dati = null;
   


		private bool _ReadOnly = false;

        GenericTable gtCassaContante = null;

        public ucExcel_CassaContante()
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

            bool starternodestobeadded = false;

        

            dati = cBusinessObjects.GetData(id, typeof(CassaContante));


            foreach (DataRow dtrow in dati.Rows)
            {
                if (String.IsNullOrEmpty(dtrow["txtSaldoSchedaContabile"].ToString()))
                    txtSaldoSchedaContabile.Text = "";
                else
                    txtSaldoSchedaContabile.Text = dtrow["txtSaldoSchedaContabile"].ToString();

                if (String.IsNullOrEmpty(dtrow["CreditoEsistente"].ToString()))
                    txtCreditoEsistente.Text = "";
                else
                    txtCreditoEsistente.Text = dtrow["CreditoEsistente"].ToString();

                starternodestobeadded = true;
            }



            gtCassaContante = new GenericTable( tblCassaContante, _ReadOnly);

            gtCassaContante.ColumnsAlias = new string[] { "N° Pezzi", "Unitario", "Euro" };
            gtCassaContante.ColumnsValues = new string[] { "numeropezzi", "unitario", "euro" };
            gtCassaContante.ColumnsWidth = new double[] { 1.0, 1.0, 1.0 };
            gtCassaContante.ColumnsMinWidth = new double[] { 0.0, 0.0, 0.0 };
            gtCassaContante.ColumnsTypes = new string[] { "int", "money", "money" };
            gtCassaContante.ColumnsAlignment = new string[] { "right", "right", "right" };
            gtCassaContante.ConditionalReadonly = new bool[] { false, true, true };
            gtCassaContante.ConditionalAttribute = "";
            gtCassaContante.ColumnsHasTotal = new bool[] { false, false, true };
            gtCassaContante.AliasTotale = "Totale";
            gtCassaContante.ColumnAliasTotale = 1;
            gtCassaContante.dati = dati;
            gtCassaContante.xml = false;
            gtCassaContante.TotalToBeCalculated += GtCassaContante_TotalToBeCalculated;

          
            gtCassaContante.GenerateTable();

            if(starternodestobeadded)
            {

                addNode("500,00", false);
                addNode("200,00", false);
                addNode("100,00", false);
                addNode("50,00", false);
                addNode("20,00", false);
                addNode("10,00", false);
                addNode("5,00", false);
                addNode("2,00", false);
                addNode("1,00", false);
                addNode("0,50", false);
                addNode("0,20", false);
                addNode("0,10", false);
                addNode("0,05", false);
                addNode("0,02", false);
                addNode("0,01", false);
                addNode("Totale", true);

             
            }
        }


        public int Save()
        {

            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["txtTotaleComplessivo"] = txtTotaleComplessivo.Text;
                dtrow["txtDifferenza"] = txtDifferenza.Text;
            }
            return cBusinessObjects.SaveData(id, dati, typeof(CassaContante));
        }


        private void addNode(string unitario, bool bold)
		{
            dati.Rows.Add(id,cBusinessObjects.idcliente,cBusinessObjects.idsessione,unitario);
            gtCassaContante.AddRow();
        }

        private void GtCassaContante_TotalToBeCalculated(object sendername, EventArgs e)
        {
            if (((string)sendername).Split('_').Count() < 2)
            {
                return;
            }

            string idcolumn = ((string)sendername).Split('_')[1];
            string idrow = ((string)sendername).Split('_')[2];

            if (idcolumn == "0")
            {
                double unitario = 0.0;
                double numeropezzi = 0.0;
                double euro = 0.0;

                double.TryParse(gtCassaContante.GetValue("1", idrow), out unitario);
                double.TryParse(gtCassaContante.GetValue("0", idrow), out numeropezzi);

                euro = unitario * numeropezzi;

                gtCassaContante.SetValue("2", idrow, cBusinessObjects.ConvertNumber(euro.ToString()));                
            }

            GenerateTotal();
        }

        private void GenerateTotal()
        {
            txtTotaleComplessivo.Text = gtCassaContante.GenerateSpecificTotal("2");

            double saldo = 0.0;
            double.TryParse(txtSaldoSchedaContabile.Text, out saldo);

            double totale = 0.0;
            double.TryParse(txtTotaleComplessivo.Text, out totale);

            txtDifferenza.Text =cBusinessObjects.ConvertNumber((totale - saldo).ToString());
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
			
			if (dati.Rows != null)
			{
                foreach (DataRow dtrow in dati.Rows)
                {
                    dtrow["CreditoEsistente"] = txtCreditoEsistente.Text;
                }
                
                gtCassaContante.GenerateTable();
            }            
        }

		private void txtSaldoSchedaContabile_LostFocus(object sender, RoutedEventArgs e)
		{


			txtSaldoSchedaContabile.Text = cBusinessObjects.ConvertNumber(txtSaldoSchedaContabile.Text);
            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["txtSaldoSchedaContabile"] = txtSaldoSchedaContabile.Text;
            }
            GenerateTotal();

		}
    }
}
