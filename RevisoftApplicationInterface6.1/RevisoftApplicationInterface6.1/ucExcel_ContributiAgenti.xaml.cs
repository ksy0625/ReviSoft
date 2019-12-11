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
	public partial class ucExcel_ContributiAgenti : UserControl
    {
        public int id;
        private DataTable dati = null;
        private DataTable dati_Note = null;
        
        private int numeroattuale = 1;
		private bool _ReadOnly = false;


        GenericTable gtCOGE = null;
        GenericTable gtCOGENote = null;

        public ucExcel_ContributiAgenti()
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
            dati = cBusinessObjects.GetData(id, typeof(ContributiAgenti));
            dati_Note = cBusinessObjects.GetData(id, typeof(ContributiAgenti_Note));
 
           foreach (DataRow dtrow in dati.Rows)
            {
				if (dtrow["Header"].ToString() != null)
				{
					if (!Al.Contains(dtrow["Header"].ToString()))
					{
						Al.Add(dtrow["Header"].ToString());
					}
				}
				else
				{
                    // dati.Rows.Add(id, "Contributi Agenti");
				}
			}

			if (Al.Count == 0)
			{
                Al.Add( "Contributi Agenti" );
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

            gtCOGE = new GenericTable( tblCOGE, _ReadOnly);

            gtCOGE.ColumnsAlias = new string[] { "Rif", "Periodo", "Scadenza", "Data presentaz.", "Importo", "Data Pag." };
            gtCOGE.ColumnsValues = new string[] { "rif", "periodo", "scadenza", "datapresentaz", "importo", "datapag" };
            gtCOGE.ColumnsWidth = new double[] { 1.0, 2.0, 2.0, 2.0, 2.0, 2.0 };
            gtCOGE.ColumnsMinWidth = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            gtCOGE.ColumnsTypes = new string[] { "string", "string", "string", "string", "money", "string" };
            gtCOGE.ColumnsAlignment = new string[] { "left", "left", "right", "right", "right", "right" };
            gtCOGE.ColumnsReadOnly = new bool[] { false, false, false, false, false, false };
            gtCOGE.ConditionalReadonly = new bool[] { false, false, false, false, false, false };
            gtCOGE.ConditionalAttribute = "new";
            gtCOGE.dati = dati;
            gtCOGE.xml = false;
            gtCOGE.ColumnsHasTotal = new bool[] { false, false, false, false, true, false };
            gtCOGE.AliasTotale = "Totale";
            gtCOGE.ColumnAliasTotale = 1;

            gtCOGENote = new GenericTable( tblCOGENote, _ReadOnly);

            gtCOGENote.ColumnsAlias = new string[] { "Rif", "Noteo" };
            gtCOGENote.ColumnsValues = new string[] { "rif", "note" };
            gtCOGENote.ColumnsWidth = new double[] { 1.0, 22.0 };
            gtCOGENote.ColumnsMinWidth = new double[] { 0.0, 0.0 };
            gtCOGENote.ColumnsTypes = new string[] { "string", "string" };
            gtCOGENote.ColumnsAlignment = new string[] { "left", "left" };
            gtCOGENote.ColumnsReadOnly = new bool[] { false, false };
            gtCOGENote.ConditionalReadonly = new bool[] { false, false };
            gtCOGENote.ConditionalAttribute = "new";
            gtCOGENote.dati = dati_Note;
            gtCOGENote.xml = false;
            gtCOGENote.ColumnsHasTotal = new bool[] { false, false };
            gtCOGENote.AliasTotale = "Totale";
            gtCOGENote.ColumnAliasTotale = 1;
        }

        public int Save()
        {
            cBusinessObjects.SaveData(id, dati, typeof(ContributiAgenti));
            return cBusinessObjects.SaveData(id, dati_Note, typeof(ContributiAgenti_Note));
        }

		
		private void AggiungiNodo(string Alias,  string Codice)
        {
            
                numeroattuale =1;
                if (tabControl.SelectedItem != null)
                {
                    foreach (DataRow dtrow in dati.Rows)
                    {
                    if (dtrow["Header"].ToString() != ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                        continue;
                    if (dtrow["rif"] != null)
                    {
                        int valorehere = 0;

                        int.TryParse(dtrow["rif"].ToString(), out valorehere);

                        if (valorehere > numeroattuale)
                        {
                        numeroattuale = valorehere;
                        }
                    }              
                    numeroattuale = numeroattuale + 1;
                    }
                }

            DataRow dd = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, ((TabItem)(tabControl.SelectedItem)).Header, (numeroattuale).ToString());
            dd["isnew"] = 1;
            gtCOGE.GenerateTable();
        }

        private void AggiungiNodoNote( string Alias, string Codice )
        {
                numeroattuale = 1;
                if (tabControl.SelectedItem != null)
                {
                    foreach (DataRow dtrow in dati_Note.Rows)
                    {
                    if (dtrow["Header"].ToString() != ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                        continue;
                    if (dtrow["rif"] != null)
                    {
                        int valorehere = 0;

                        int.TryParse(dtrow["rif"].ToString(), out valorehere);

                        if (valorehere > numeroattuale)
                        {
                        numeroattuale = valorehere;
                        }
                    }              
                    numeroattuale = numeroattuale + 1;
                    }
                }

            DataRow dd=dati_Note.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, ((TabItem)(tabControl.SelectedItem)).Header, (numeroattuale).ToString());
            dd["isnew"] = 1;
            gtCOGENote.GenerateTable();
        }

		private void AddRowErroriRilevati(object sender, RoutedEventArgs e)
		{
			AggiungiNodo("",  "");
		}

		private void DeleteRowErroriRilevati(object sender, RoutedEventArgs e)
		{
            gtCOGE.DeleteRow();            
		}

        private void AddRowNote( object sender, RoutedEventArgs e )
        {
            AggiungiNodoNote( "",  "" );
        }

        private void DeleteRowNote( object sender, RoutedEventArgs e )
        {
            gtCOGENote.DeleteRow();
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


                    gtCOGE.filtercolumn = "Header";
                    gtCOGE.filtervalue = ((TabItem)(tabControl.SelectedItem)).Header.ToString();
                    gtCOGE.GenerateTable();
                    gtCOGENote.filtercolumn = "Header";
                    gtCOGENote.filtervalue = ((TabItem)(tabControl.SelectedItem)).Header.ToString();
                    gtCOGENote.GenerateTable();

                }
				else
				{
                    gtCOGE.filtercolumn = "Header";
                    gtCOGE.filtervalue = ((string)(((TabItem)(e.AddedItems[0])).Header));
                    gtCOGE.GenerateTable();
                    gtCOGENote.filtercolumn = "Header";
                    gtCOGENote.filtervalue = ((string)(((TabItem)(e.AddedItems[0])).Header));
                    gtCOGENote.GenerateTable();
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

            if ( tabControl.SelectedItem == null )
            {
                return;
            }

			if (MessageBox.Show("La tabella verrà cancellata. Procedere?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
                string newHeader = ( (TabItem)( tabControl.SelectedItem ) ).Header.ToString();
                for (int i = dati.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dtrow = dati.Rows[i];
                    if (dtrow["Header"].ToString() == newHeader)
                        dtrow.Delete();
                }

                for (int i = dati_Note.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dtrow = dati_Note.Rows[i];
                    if (dtrow["Header"].ToString() == newHeader)
                        dtrow.Delete();
                }

                dati.AcceptChanges();
                dati_Note.AcceptChanges();

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
            string newHeader = dialog.ResponseText;

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

            ((TabItem)(tabControl.SelectedItem)).Header = newHeader;
            gtCOGE.filtercolumn = "Header";
            gtCOGE.filtervalue = newHeader;
            gtCOGE.GenerateTable();
            gtCOGENote.filtercolumn = "Header";
            gtCOGENote.filtervalue = newHeader;
            gtCOGENote.GenerateTable();


        
		}
        
		private void ChangeNameTab(string newname, string oldheader)
		{
            foreach (DataRow dtrow in dati.Rows)
            {
                 if(dtrow["Header"].ToString()== oldheader)
                 {
                   dtrow["Header"] = newname;
                 }
            }

            foreach (DataRow dtrow in dati_Note.Rows)
            {
                if (dtrow["Header"].ToString() == oldheader)
                {
                dtrow["Header"] = newname;
                }
            }

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

                DataTable datiC = dati.Clone();
                foreach (TabItem tb in tabControl.Items)
                {
                    foreach (DataRow dtrow in dati.Rows)
                    {
                        if (dtrow["Header"] != null)
                        {
                            if (dtrow["Header"].ToString() == tb.Header.ToString())
                            {
                                DataRow firstNewRow = datiC.NewRow();
                                firstNewRow.ItemArray = dtrow.ItemArray;
                                datiC.Rows.Add(firstNewRow);
                            }
                        }
                    }
                }
                dati = datiC;

                DataTable datiCNote = dati_Note.Clone();
                foreach (TabItem tb in tabControl.Items)
                {
                    foreach (DataRow dtrow in dati_Note.Rows)
                    {
                        if (dtrow["Header"] != null)
                        {
                            if (dtrow["Header"].ToString() == tb.Header.ToString())
                            {
                                DataRow firstNewRow = datiCNote.NewRow();
                                firstNewRow.ItemArray = dtrow.ItemArray;
                                datiCNote.Rows.Add(firstNewRow);
                            }
                        }
                    }
                }
                dati_Note = datiCNote;

                gtCOGE.filtercolumn = "Header";
                gtCOGE.filtervalue = targetHeader;
                gtCOGE.GenerateTable();
                gtCOGENote.filtercolumn = "Header";
                gtCOGENote.filtervalue = targetHeader;
                gtCOGENote.GenerateTable();

              
			}
		}

        private void tabControl_PreviewMouseDown( object sender, MouseButtonEventArgs e )
        {
            ;
        }
    }
}
