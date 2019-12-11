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
	public partial class ucExcel_F24 : UserControl
    {
        public int id;
        private DataTable dati = null;
        private DataTable dati_Note = null;
        

      
        private int numeroattuale = 1;

		private bool _ReadOnly = false;

        GenericTable gtF24 = null;
        GenericTable gtF24Note = null;

        public ucExcel_F24()
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
            dati = cBusinessObjects.GetData(id, typeof(Excel_F24));
            dati_Note = cBusinessObjects.GetData(id, typeof(Excel_F24Note));

            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["Header"] != null)
                {
                    if (!Al.Contains(dtrow["Header"].ToString()))
                    {
                        Al.Add(dtrow["Header"].ToString());     
                    }
                }
            }
       

			if (Al.Count == 0)
			{
                Al.Add( "F24" );
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

            gtF24 = new GenericTable( tblF24, _ReadOnly);

            gtF24.ColumnsAlias = new string[] { "Rif", "Codice Tributo", "Competenza", "Importo Pagato", "Importo Compensato", "Data Pagam.", "Data Scadenza" };
            gtF24.ColumnsValues = new string[] { "rif", "codicetributo", "competenza", "importopagato", "importocompensato", "datapagamento", "datascadenza" };
            gtF24.ColumnsWidth = new double[] { 1.0, 4.0, 2.0, 2.0, 2.0, 1.0, 1.0 };
            gtF24.ColumnsMinWidth = new double[] { 50.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            gtF24.ColumnsTypes = new string[] { "int", "string", "string", "money", "money", "string", "string" };
            gtF24.ColumnsAlignment = new string[] { "left", "left", "left", "right", "right", "right", "right" };
            gtF24.ConditionalReadonly = new bool[] { false, false, false, false, false, false, false };
            gtF24.ConditionalAttribute = "new";
            gtF24.ColumnsHasTotal = new bool[] { false, false, false, true, true, false, false};
            gtF24.AliasTotale = "Totale";
            gtF24.dati = dati;
            gtF24.xml = false;
            gtF24.ColumnAliasTotale = 1;

            gtF24Note = new GenericTable( tblF24Note, _ReadOnly);

            gtF24Note.ColumnsAlias = new string[] { "Rif", "Note" };
            gtF24Note.ColumnsValues = new string[] { "rif", "note" };
            gtF24Note.ColumnsWidth = new double[] { 1.0, 10.0 };
            gtF24Note.ColumnsMinWidth = new double[] { 50.0, 0.0};
            gtF24Note.ColumnsTypes = new string[] { "int", "string"};
            gtF24Note.ColumnsAlignment = new string[] { "left", "left" };
            gtF24Note.ConditionalReadonly = new bool[] { false, false };
            gtF24Note.ConditionalAttribute = "new";
            gtF24Note.ColumnsHasTotal = new bool[] { false, false };
            gtF24Note.AliasTotale = "Totale";
            gtF24Note.dati = dati_Note;
            gtF24Note.xml = false;
            gtF24Note.ColumnAliasTotale = 1;
        }
		
		public int Save()
		{
            cBusinessObjects.SaveData(id, dati_Note, typeof(Excel_F24Note));
            return cBusinessObjects.SaveData(id, dati, typeof(Excel_F24));

        }
		
		private void AddRowErroriRilevati(object sender, RoutedEventArgs e)
		{
            numeroattuale = 0;

            if (tabControl.SelectedItem != null)
            {
                foreach (DataRow dtrow in this.dati.Rows)
                {
                 if(dtrow["Header"].ToString()== ((TabItem)(tabControl.SelectedItem)).Header.ToString())    
                    {
                        if (dtrow["rif"] != null)
                        {
                            int valorehere = 0;

                            int.TryParse(dtrow["rif"].ToString(), out valorehere);

                            if (valorehere > numeroattuale)
                            {
                                numeroattuale = valorehere;
                            }
                        }
                    }
                }
            }
            numeroattuale = numeroattuale + 1;

            dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, ((TabItem)(tabControl.SelectedItem)).Header, numeroattuale);
            gtF24.GenerateTable();
            return;
		}

		private void DeleteRowErroriRilevati(object sender, RoutedEventArgs e)
		{
            gtF24.DeleteRow();
            return;

			//if (_ReadOnly)
			//{
			//	MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
			//	return;
			//}

			//if (MessageBox.Show("Si è sicuri di procedere con l'eliminazione?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			//{
			//	XmlNode node = null;

			//	if (dtgF24.SelectedCells.Count >= 1)
			//	{
			//		node = (XmlNode)(dtgF24.SelectedCells[0].Item);
			//	}
			//	else
			//	{
			//		MessageBox.Show("Selezionare una riga");
			//		return;
			//	}

			//	try
			//	{
			//		string ID = node.Attributes["new"].Value;

			//		node.ParentNode.RemoveChild(node);

			//		GenerateTotal();

			//		return;
			//	}
			//	catch (Exception ex)
			//	{
			//		string log = ex.Message;

			//		MessageBox.Show("Solo le righe inserite dall'utente possono essere cancellate");
			//	}
			//}
		}

        private void AddRowNote( object sender, RoutedEventArgs e )
        {

            dati_Note.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, ((TabItem)(tabControl.SelectedItem)).Header);
            gtF24Note.GenerateTable();

            return;

            //AggiungiNodoNote( "", _ID, "" );
        }

        private void DeleteRowNote( object sender, RoutedEventArgs e )
        {
            gtF24Note.DeleteRow();

            return;
        }
        
		private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            gtF24.SetFocus();
            
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
                    string newHeader =dialog.ResponseText;

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
                  
                 
               
				}
				else
				{
                    gtF24.filtercolumn = "Header";
                    gtF24.filtervalue = ((string)(((TabItem)(e.AddedItems[0])).Header)); ;
                    gtF24.GenerateTable();
                    gtF24Note.filtercolumn = "Header";
                    gtF24Note.filtervalue = ((string)(((TabItem)(e.AddedItems[0])).Header)); ;
                    gtF24Note.GenerateTable();
                                
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
               
                this.dati.AcceptChanges();
                for (int i = dati_Note.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dtrow = dati_Note.Rows[i];
                    if (dtrow["Header"].ToString() == newHeader)
                        dtrow.Delete();
                }
               
                this.dati_Note.AcceptChanges();


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
            gtF24.filtercolumn = "Header";
            gtF24.filtervalue = newHeader ;
            gtF24.GenerateTable();
            gtF24Note.filtercolumn = "Header";
            gtF24Note.filtervalue =newHeader ;
            gtF24Note.GenerateTable();

            ((TabItem)(tabControl.SelectedItem)).Header = newHeader;
			//txtHeader.Text = newHeader;
		}
 
		private void ChangeNameTab(string newname, string oldheader)
		{
            foreach (DataRow dtrow in this.dati.Rows)
            {
                if (dtrow["Header"].ToString() == oldheader)
                {
                    dtrow["Header"] = newname;
                }

            }
            foreach (DataRow dtrow in this.dati_Note.Rows)
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

                gtF24.filtercolumn = "Header";
                gtF24.filtervalue = targetHeader;
                gtF24.GenerateTable();
                gtF24Note.filtercolumn = "Header";
                gtF24Note.filtervalue = targetHeader;
                gtF24Note.GenerateTable();
              
            }
          
		}

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            gtF24.SetFocus();
        }
    }
}
