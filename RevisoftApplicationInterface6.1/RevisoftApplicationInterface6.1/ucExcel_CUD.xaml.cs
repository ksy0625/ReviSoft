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
	public partial class ucExcel_CUD : UserControl
    {
        public int id;
        private DataTable dati = null;
        private DataTable dati_note = null;

       
		private bool _ReadOnly = false;
        int numeroattuale = 1;

        GenericTable gtCUD = null;
        GenericTable gtCUDNote = null;

        public ucExcel_CUD()
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

          
      
            

            dati = cBusinessObjects.GetData(id, typeof(Excel_CUD));
            dati_note = cBusinessObjects.GetData(id, typeof(Excel_CUD_Note));


            ArrayList Al = new ArrayList();
            
            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["Header"]!=null)
                {
                    if (!Al.Contains(dtrow["Header"].ToString()))
                    {
                        Al.Add(dtrow["Header"].ToString());
                    }
                  
                }
            }
          
            if (Al.Count == 0)
            {
                Al.Add("CUD");
            }


            gtCUD = new GenericTable( tblCUD, _ReadOnly);

            gtCUD.ColumnsAlias = new string[] { "Rif", "Descrizione", "Scadenza", "Data Invio" };
            gtCUD.ColumnsValues = new string[] { "rif", "periodo", "scadenza", "datapresentaz" };
            gtCUD.ColumnsWidth = new double[] { 1.0, 4.0, 2.0, 2.0 };
            gtCUD.ColumnsMinWidth = new double[] { 50.0, 0.0, 0.0, 0.0 };
            gtCUD.ColumnsTypes = new string[] { "string", "string", "string", "string" };
            gtCUD.ColumnsAlignment = new string[] { "left", "left", "right", "right" };
            gtCUD.ConditionalReadonly = new bool[] { true, true, false, false };
            gtCUD.ConditionalAttribute = "new";
            gtCUD.ColumnsHasTotal = new bool[] { false, false, false, false };
            gtCUD.AliasTotale = "Totale";
            gtCUD.dati = dati;
            gtCUD.xml = false;
            gtCUD.ColumnAliasTotale = 1;

            gtCUDNote = new GenericTable( tblCUDNote, _ReadOnly);

            gtCUDNote.ColumnsAlias = new string[] { "Rif", "Note" };
            gtCUDNote.ColumnsValues = new string[] { "rif", "note" };
            gtCUDNote.ColumnsWidth = new double[] { 1.0, 8.0 };
            gtCUDNote.ColumnsMinWidth = new double[] { 50.0, 0.0 };
            gtCUDNote.ColumnsTypes = new string[] { "string", "string" };
            gtCUDNote.ColumnsAlignment = new string[] { "left", "left" };
            gtCUDNote.ConditionalReadonly = new bool[] { false, false };
            gtCUDNote.ConditionalAttribute = "new";
            gtCUDNote.ColumnsHasTotal = new bool[] { false, false };
            gtCUDNote.AliasTotale = "Totale";
            gtCUDNote.dati = dati_note;
            gtCUDNote.xml = false;
            gtCUDNote.ColumnAliasTotale = 1;

            if (Al.Count == 0)
			{
				
                if ( _ReadOnly )
                {
                   
                }
                else
                {

                    DataRow dd = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, ((tabControl == null || tabControl.SelectedItem == null) ? "CUD" : ((TabItem)(tabControl.SelectedItem)).Header),1, "Certificazioni su ritenute acc.to operate");
                    dd["isnew"] = 1;
                    dd = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione,((tabControl == null || tabControl.SelectedItem == null) ? "CUD" : ((TabItem)(tabControl.SelectedItem)).Header),2, "Consegna CUD");
                    dd["isnew"] = 1;
                    dd = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, ((tabControl == null || tabControl.SelectedItem == null) ? "CUD" : ((TabItem)(tabControl.SelectedItem)).Header),3, "Certificazione CUPE");
                    dd["isnew"] = 1;
                    gtCUD.GenerateTable();

                }
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
        }

    //------------------------------------------------------------------------+
    //                                  Save                                  |
    //------------------------------------------------------------------------+
    public int Save()
    {
      //----------------------------------------------------------------------+
      //      4.7.15 - Quando si crea una nuova riga, al momento del          |
      //      salvataggio esistono record con campo Header null. Per          |
      //      questo alla riapertura si vede un TAB in più senza nome.        |
      //      Non importa se la riga aggiunta viene cancellata prima          |
      //      di salvare. Un rimedio è eliminare tutti i record con           |
      //      Header null qui, un attimo prima di salvare. La soluzione       |
      //      corretta sarebbe però capire come sono generati questi          |
      //      record estranei.                                                |
      //----------------------------------------------------------------------+
      DataRow[] arrRows = dati.Select("header is null");
      foreach (DataRow dr in arrRows) dati.Rows.Remove(dr);
      cBusinessObjects.SaveData(id, dati_note, typeof(Excel_CUD_Note));
      return cBusinessObjects.SaveData(id, dati, typeof(Excel_CUD));
    }

    private void AddRowErroriRilevati(object sender, RoutedEventArgs e)
		{
            numeroattuale = 0;
            foreach (DataRow dtrow in dati.Rows)
            {
                if ((dtrow["Header"] !=null)&&(dtrow["Header"].ToString()== ((tabControl == null || tabControl.SelectedItem == null) ? "CUD" : ((TabItem)(tabControl.SelectedItem)).Header).ToString()))
                 if (numeroattuale < int.Parse(dtrow["rif"].ToString()))
                    numeroattuale = int.Parse(dtrow["rif"].ToString());
            }
            numeroattuale++;

            DataRow dd=dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, ((tabControl == null || tabControl.SelectedItem == null) ? "CUD" : ((TabItem)(tabControl.SelectedItem)).Header), (numeroattuale).ToString(), "");
            dd["isnew"] = 1;
            gtCUD.AddRow();
            return;
		}

		private void DeleteRowErroriRilevati(object sender, RoutedEventArgs e)
		{
            gtCUD.DeleteRow();
         
		}

        private void AddRowNote( object sender, RoutedEventArgs e )
        {
            DataRow dd = dati_note.Rows.Add(id,  cBusinessObjects.idcliente, cBusinessObjects.idsessione, ((tabControl == null || tabControl.SelectedItem == null) ? "CUD" : ((TabItem)(tabControl.SelectedItem)).Header));
            dd["isnew"] = 1;
            gtCUDNote.AddRow();
           
        }

        private void DeleteRowNote( object sender, RoutedEventArgs e )
        {
            gtCUDNote.DeleteRow();
            
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
                   
                    DataRow dd = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione,newHeader.ToString(), 1, "Certificazioni su ritenute acc.to operate");
                    dd["isnew"] = 1;
                    dd = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, newHeader.ToString(), 2, "Consegna CUD");
                    dd["isnew"] = 1;
                    dd = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione,newHeader.ToString(), 3, "Certificazione CUPE");
                    dd["isnew"] = 1;

                    gtCUD.filtercolumn = "Header";
                    gtCUD.filtervalue = newHeader;
                    gtCUD.GenerateTable();
                    gtCUDNote.filtercolumn = "Header";
                    gtCUDNote.filtervalue = newHeader;
                    gtCUDNote.GenerateTable();
                  
                }
				else
				{
                    gtCUD.filtercolumn = "Header";
                    gtCUD.filtervalue = ((string)(((TabItem)(e.AddedItems[0])).Header));
                    gtCUD.GenerateTable();
                    gtCUDNote.filtercolumn = "Header";
                    gtCUDNote.filtervalue = ((string)(((TabItem)(e.AddedItems[0])).Header));
                    gtCUDNote.GenerateTable();
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
                for (int i = dati_note.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dtrow = dati_note.Rows[i];
                    if (dtrow["Header"].ToString() == newHeader)
                        dtrow.Delete();
                }

                
                dati.AcceptChanges();
                dati_note.AcceptChanges();



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

            ((TabItem)(tabControl.SelectedItem)).Header = newHeader;
            gtCUD.filtercolumn = "Header";
            gtCUD.filtervalue = newHeader;
            gtCUD.GenerateTable();
            gtCUDNote.filtercolumn = "Header";
            gtCUDNote.filtervalue = newHeader;
            gtCUDNote.GenerateTable();

        }

		private void ChangeNameTab(string newname, string oldheader)
		{
            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["Header"] != null)
                {
                    if (dtrow["Header"].ToString() == oldheader)
                    {
                        dtrow["Header"] = newname;
                    }
                }

            }
            foreach (DataRow dtrow in dati_note.Rows)
            {
                if (dtrow["Header"] != null)
                {
                    if (dtrow["Header"].ToString() == oldheader)
                    {
                        dtrow["Header"] = newname;
                    }
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

			

				tabItemTarget.Header = sourceHeader;
				tabItemSource.Header = targetHeader;

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

                DataTable datiCNote = dati_note.Clone();
                foreach (TabItem tb in tabControl.Items)
                {
                    foreach (DataRow dtrow in dati_note.Rows)
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
                dati_note = datiCNote;

                gtCUD.filtercolumn = "Header";
                gtCUD.filtervalue = targetHeader;
                gtCUD.GenerateTable();
                gtCUDNote.filtercolumn = "Header";
                gtCUDNote.filtervalue = targetHeader;
                gtCUDNote.GenerateTable();

                
            }
		}

    }
}
