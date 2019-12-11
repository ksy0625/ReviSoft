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
	public partial class ucExcel_Riconciliazioni : UserControl
    {
        public int id;
    

        private DataTable dati = null;

		private bool _ReadOnly = false;

        GenericTable gtRiconciliazioni = null;

        public ucExcel_Riconciliazioni()
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

            ArrayList Al = new ArrayList();

            dati = cBusinessObjects.GetData(id, typeof(Excel_Riconciliazioni));
            foreach (DataRow dtrow in dati.Rows)
            {
               
                if (!Al.Contains(dtrow["CreditoEsistente"].ToString()))
                {
                    Al.Add(dtrow["CreditoEsistente"].ToString());
                }
            }

			if (Al.Count == 0)
			{
                Al.Add( "Conti Ordinari" );
			}

			foreach (string item in Al)
			{
				TabItem ti = new TabItem();
				ti.Header = item;

				tabControl.Items.Add(ti);
			}

			TabItem tiout = new TabItem();
			tiout.Header = App.NewTabHeaderText;

			tabControl.Items.Add(tiout);

            gtRiconciliazioni = new GenericTable( tblRiconciliazioni, _ReadOnly);

            gtRiconciliazioni.ColumnsAlias = new string[] { "Banca", "c/c n°", "Saldo contabile", "Saldo e/c banca", "Differenza", "Riconciliato", "imp.non ric." };
            gtRiconciliazioni.ColumnsValues = new string[] { "banca", "ccn", "saldocontabile", "saldoec", "differenza", "riconciliato", "importoconriconciliato" };
            gtRiconciliazioni.ColumnsWidth = new double[] { 2.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };
            gtRiconciliazioni.ColumnsMinWidth = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            gtRiconciliazioni.ColumnsTypes = new string[] { "string", "string", "money", "money", "money", "money", "money" };
            gtRiconciliazioni.ColumnsAlignment = new string[] { "left", "left", "right", "right", "right", "right", "right" };
            gtRiconciliazioni.ColumnsReadOnly = new bool[] { false, false, false, false, true, false, true };
            gtRiconciliazioni.ConditionalReadonly = new bool[] { false, false, false, false, false, false, false };
            gtRiconciliazioni.ConditionalAttribute = "new";
            gtRiconciliazioni.ColumnsHasTotal = new bool[] { false, false, true, true, true, true, true };
            gtRiconciliazioni.AliasTotale = "Totale";
            gtRiconciliazioni.ColumnAliasTotale = 1;
            gtRiconciliazioni.xml = false;
            gtRiconciliazioni.dati = dati;
            gtRiconciliazioni.TotalToBeCalculated += GtCOGE_TotalToBeCalculated;
        }
		
        public int Save()
        {
           
            for (int i = dati.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dtrow = dati.Rows[i];
                if (dtrow["CreditoEsistente"].ToString() == "")
                    dtrow.Delete();
            }
            this.dati.AcceptChanges();
            return cBusinessObjects.SaveData(id, dati, typeof(Excel_Riconciliazioni));
        }

        private void AggiungiNodo(string Alias, string Codice)
        {
          
            dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, ((TabItem)(tabControl.SelectedItem)).Header);
            gtRiconciliazioni.AddRow();
        }

        private void GtCOGE_TotalToBeCalculated(object sendername, EventArgs e)
        {
            if (((string)sendername).Split('_').Count() < 2)
            {
                return;
            }

            string idcolumn = ((string)sendername).Split('_')[1];
            string idrow = ((string)sendername).Split('_')[2];

            if (idcolumn == "3" || idcolumn == "2" || idcolumn == "5")
            {
                double saldocontabile = 0.0;
                double saldoec = 0.0;
                double riconciliato = 0.0;

                double.TryParse(gtRiconciliazioni.GetValue("2", idrow), out saldocontabile);
                double.TryParse(gtRiconciliazioni.GetValue("3", idrow), out saldoec);
                double.TryParse(gtRiconciliazioni.GetValue("5", idrow), out riconciliato);

                double differenza = 0.0;
                double importoconriconciliato = 0.0;

                differenza = saldocontabile - saldoec;
                gtRiconciliazioni.SetValue("4", idrow, cBusinessObjects.ConvertNumber(differenza.ToString()));

                importoconriconciliato = saldocontabile - saldoec - riconciliato;
                gtRiconciliazioni.SetValue("6", idrow, cBusinessObjects.ConvertNumber(importoconriconciliato.ToString()));
            }
        }
        
		private void AddRowErroriRilevati(object sender, RoutedEventArgs e)
		{
			AggiungiNodo("",  "");
		}

		private void DeleteRowErroriRilevati(object sender, RoutedEventArgs e)
		{
            gtRiconciliazioni.DeleteRow();
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
            if ( tabControl.SelectedItem == null )
            {
                return;
            }

            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["CreditoEsistente"].ToString()== ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                {
                   dtrow["CreditoEsistente"]= txtCreditoEsistente.Text;
                }
            }
            ((TabItem)(tabControl.SelectedItem)).Header = txtCreditoEsistente.Text;


        }

		private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0 && (e.AddedItems[0]).GetType().Name == "TabItem")
			{
				if (((string)(((TabItem)(e.AddedItems[0])).Header)) == App.NewTabHeaderText)
				{
					if (_ReadOnly)
					{
						MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
						return;
					}

					var dialog = new wInputBox("Inserire Nome della nuova Tabella");
					dialog.ShowDialog();
                    if (!dialog.diagres)
                    {
                        return;
                    }
                    string newHeader = dialog.ResponseText;

					if (newHeader == "")
					{
						MessageBox.Show("Attenzione, Nome non valido");
						tabControl.SelectedIndex = 0;
						return;
					}

					foreach (TabItem item in tabControl.Items)
					{
						if (((string)(item.Header)) == newHeader)
						{
							MessageBox.Show("Attenzione, Nome già esistente");
							tabControl.SelectedIndex = 0;
							return;
						}
					}

					TabItem ti = new TabItem();
					ti.Header = newHeader;

		        

					tabControl.Items.Insert(tabControl.Items.Count - 1, ti);
					tabControl.SelectedIndex = tabControl.Items.Count - 2;
                    gtRiconciliazioni.filtercolumn = "CreditoEsistente";
                    gtRiconciliazioni.filtervalue = newHeader;
                    txtCreditoEsistente.Text = newHeader;
                    gtRiconciliazioni.GenerateTable();
                    AggiungiNodo("", "");
				}
				else
				{
					
                    foreach (DataRow dtrow in dati.Rows)
                    {
                        if (dtrow["CreditoEsistente"].ToString() == ((TabItem)(e.AddedItems[0])).Header.ToString())
                        {
                            txtCreditoEsistente.Text = dtrow["CreditoEsistente"].ToString();    
                        }
                    }
                    gtRiconciliazioni.filtercolumn = "CreditoEsistente";
                    gtRiconciliazioni.filtervalue = ((TabItem)(e.AddedItems[0])).Header.ToString();
                    gtRiconciliazioni.GenerateTable();
                        
				}
			}			
		}

		private void btnDeleteTable_Click(object sender, RoutedEventArgs e)
		{
            if ( tabControl.Items.Count <= 2 )
            {
                MessageBox.Show( "L'unica tabella presente non è cancellabile; usa il comando Cancella Contenuto.", "Attenzione" );
                return;
            }

			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}

			if (MessageBox.Show("La tabella verrà cancellata. Procedere?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				string newHeader = ((TabItem)(tabControl.SelectedItem)).Header.ToString();

              
                for (int i = dati.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dtrow = dati.Rows[i];
                    if (dtrow["CreditoEsistente"].ToString() == newHeader)
                        dtrow.Delete();
                }
                this.dati.AcceptChanges();


                TabItem oldSelected = ((TabItem)(tabControl.SelectedItem));

				if (tabControl.Items.Count > 0)
				{
					tabControl.SelectedIndex = 0;
				}
				else
				{
					tabControl.SelectedIndex = -1;
				}

				tabControl.Items.Remove(oldSelected);
			}
		}

		private void btnRenameTable_Click(object sender, RoutedEventArgs e)
		{
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}

            if ( tabControl.SelectedItem == null )
            {
                return;
            }

			var dialog = new wInputBox("Inserire Titolo della nuova Tabella");
            dialog.ResponseText = ( (TabItem)( tabControl.SelectedItem ) ).Header.ToString();
			dialog.ShowDialog();
            if (!dialog.diagres)
            {
                return;
            }
            string newHeader =dialog.ResponseText;

			if (newHeader == "")
			{
				MessageBox.Show("Attenzione, Titolo non valido");
				tabControl.SelectedIndex = 0;
				return;
			}

			foreach (TabItem item in tabControl.Items)
			{
				if (((string)(item.Header)) == newHeader)
				{
					MessageBox.Show("Attenzione, Titolo già esistente");
					tabControl.SelectedIndex = 0;
					return;
				}
			}

			ChangeNameTab(newHeader, ((TabItem)(tabControl.SelectedItem)).Header.ToString());


            gtRiconciliazioni.filtercolumn = "CreditoEsistente";
            gtRiconciliazioni.filtervalue = newHeader;
            gtRiconciliazioni.GenerateTable();
            
			((TabItem)(tabControl.SelectedItem)).Header = newHeader;
		}
        
		private void ChangeNameTab(string newname, string oldheader)
		{
            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["CreditoEsistente"].ToString() == oldheader)
                {
                    dtrow["CreditoEsistente"] = newname;
                }
            }
            txtCreditoEsistente.Text = newname;
        }

		private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			var tabItem = e.Source as TabItem;

			if (tabItem == null)
				return;

			if (tabItem.Header.ToString() == App.NewTabHeaderText)
			{
				return;
			}

			if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
			{
				DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
			}
		}


		private void TabItem_Drop(object sender, DragEventArgs e)
		{
			if (_ReadOnly)
			{
				return;
			}

			var tabItemTarget = e.Source as TabItem;

			if (tabItemTarget.Header.ToString() == App.NewTabHeaderText)
			{
				return;
			}

			var tabItemSource = e.Data.GetData(typeof(TabItem)) as TabItem;

			if (!tabItemTarget.Equals(tabItemSource))
			{
				string sourceHeader = tabItemSource.Header.ToString();
				string targetHeader = tabItemTarget.Header.ToString();


				tabItemTarget.Header = sourceHeader;
				tabItemSource.Header = targetHeader;

                DataTable datiC = dati.Clone();
                foreach (TabItem tb in tabControl.Items)
                {
                    foreach (DataRow dtrow in dati.Rows)
                    {
                        if (dtrow["CreditoEsistente"] != null)
                        {
                            if (dtrow["CreditoEsistente"].ToString() == tb.Header.ToString())
                            {
                                DataRow firstNewRow = datiC.NewRow();
                                firstNewRow.ItemArray = dtrow.ItemArray;
                                datiC.Rows.Add(firstNewRow);
                            }
                        }
                    }
                }
                dati = datiC;

                gtRiconciliazioni.filtercolumn = "CreditoEsistente";
                gtRiconciliazioni.filtervalue = targetHeader;
                gtRiconciliazioni.GenerateTable();
			}
		}

        private void tabControl_PreviewMouseDown( object sender, MouseButtonEventArgs e )
        {
            ;
        }
    }
}
