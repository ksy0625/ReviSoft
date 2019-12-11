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
	public partial class ucExcel_RitenuteLavoratoriAutonomi : UserControl
    {
        public int id;
        private DataTable dati = null;
        private DataTable datiT = null;
        private DataTable dati_note = null;
        private DataTable dati_noteT = null;
        private int CurrentTabSelectedIndex = 0;

        private int idsessione;
        private string _ID = "";
        private int numeroattuale = 1;
		private bool _ReadOnly = false;

        public ucExcel_RitenuteLavoratoriAutonomi()
        {
          
            InitializeComponent();
            try
            {
                FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
               new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            }
            catch (Exception e)
            {

            }
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
            idsessione = cBusinessObjects.idsessione;
            _ID = ID;
           
			ArrayList Al = new ArrayList();


            string head = "";
            datiT = cBusinessObjects.GetData(id, typeof(Excel_RitenuteLavoratoriAutonomi));
            foreach (DataRow dtrow in datiT.Rows)
            {
                if (dtrow["Header"] != null)
                {
                    head = dtrow["Header"].ToString();
                    break;
                }
            }
            dati = cBusinessObjects.GetDataFiltered(datiT, head, "Header");
            dati_noteT = cBusinessObjects.GetData(id, typeof(Excel_RitenuteLavoratoriAutonomiNote));

            dati_note = cBusinessObjects.GetDataFiltered(dati_noteT, head, "Header");

           

            foreach (DataRow dtrow in datiT.Rows)
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
                Al.Add( "Ritenute" );
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
		
		public int Save()
		{
            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["Header"] = ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString();
            }

            foreach (DataRow dtrow in dati_note.Rows)
            {
                dtrow["Header"] = ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString();
            }
            datiT = cBusinessObjects.SetDataFiltered(dati, datiT, ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString(), "Header");
            cBusinessObjects.SaveData(id, datiT, typeof(Excel_RitenuteLavoratoriAutonomi));
            dati_noteT = cBusinessObjects.SetDataFiltered(dati_note, dati_noteT, ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString(), "Header");
            return cBusinessObjects.SaveData(id, dati_noteT, typeof(Excel_RitenuteLavoratoriAutonomiNote));

            

        }

        private void AggiungiNodo(string Alias, string ID, string Codice)
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

		
            numeroattuale = 0;
         
            if (tabControl.SelectedItem != null)
            {
              
                foreach (DataRow dtrow in dati.Rows)
                {
                    
                        if (dtrow["Header"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString())
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

                numeroattuale = numeroattuale + 1;
                dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, ((TabItem)(tabControl.SelectedItem)).Header, numeroattuale);


            }

        }

        private void AggiungiNodoNote( string Alias, string ID, string Codice )
        {
            if ( _ReadOnly )
            {
                MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione" );
                return;
            }

            if ( tabControl.SelectedItem == null )
            {
                return;
            }

            dati_note.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, ((TabItem)(tabControl.SelectedItem)).Header);

        }

        private void DeleteTotal()
        {

            for (int i = dati.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dtrow = dati.Rows[i];
                if (dtrow["Header"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                    if (dtrow["fornitore"].ToString() == "Totale")
                        dtrow.Delete();
            }

            this.dati.AcceptChanges();
          
        }

      
    //----------------------------------------------------------------------------+
    //                               GenerateTotal                                |
    //----------------------------------------------------------------------------+
    private void GenerateTotal()
    {
      if (tabControl.SelectedItem == null) return;
      DeleteTotal();
      double totaleimporto = 0.0;
      bool trovato = false;

      if (this.dati.Rows.Count == 0)
         return;


        foreach (DataRow dtrow in this.dati.Rows)
        {
          if (dtrow["Header"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString())
                {
                    double importo = 0.0;
                    double.TryParse(dtrow["importo"].ToString(), out importo);
                    totaleimporto += importo;
                }
        }

        DataRow dd=    dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, ((TabItem)(tabControl.SelectedItem)).Header);
         
        dd["fornitore"]= "Totale";
        dd["importo"] = totaleimporto;

    }

    private void dtgErroriRilevati_Loaded(object sender, RoutedEventArgs e)
		{
			GenerateTotal();
		}

		private void dtgErroriRilevati_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			GenerateTotal();
		}

		private void AddRowErroriRilevati(object sender, RoutedEventArgs e)
		{
			AggiungiNodo("", _ID, "");

			GenerateTotal();
		}

		private void DeleteRowErroriRilevati(object sender, RoutedEventArgs e)
		{
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}

			if (MessageBox.Show("Si è sicuri di procedere con l'eliminazione?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				

				if (dtgRitenuteLavoratoriAutonomi.SelectedCells.Count >= 1)
				{
					
				}
				else
				{
					MessageBox.Show("Selezionare una riga");
					return;
				}

				try
				{
                    int k = 0;
                    foreach (DataRow dtrow in this.dati.Rows)
                    {
                        if (k == dtgRitenuteLavoratoriAutonomi.Items.IndexOf(dtgRitenuteLavoratoriAutonomi.SelectedCells[0].Item))
                        {
                            dtrow.Delete();
                            break;
                        }

                        k++;
                    }
                    this.dati.AcceptChanges();
                    GenerateTotal();
					return;
				}
				catch (Exception ex)
				{
					string log = ex.Message;

					MessageBox.Show("Solo le righe inserite dall'utente possono essere cancellate");
				}
			}
		}

        private void AddRowNote( object sender, RoutedEventArgs e )
        {
            AggiungiNodoNote( "", _ID, "" );
        }

        private void DeleteRowNote( object sender, RoutedEventArgs e )
        {
            if ( _ReadOnly )
            {
                MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione" );
                return;
            }

            if ( MessageBox.Show( "Si è sicuri di procedere con l'eliminazione?", "Attenzione", MessageBoxButton.YesNo ) == MessageBoxResult.Yes )
            {
               

                if ( dtgRitenuteLavoratoriAutonomiNote.SelectedCells.Count >= 1 )
                {
                  
                }
                else
                {
                    MessageBox.Show( "Selezionare una riga" );
                    return;
                }

                try
                {
                    int k = 0;
                    foreach (DataRow dtrow in this.dati_note.Rows)
                    {
                        if (k == dtgRitenuteLavoratoriAutonomiNote.Items.IndexOf(dtgRitenuteLavoratoriAutonomiNote.SelectedCells[0].Item))
                        {
                            dtrow.Delete();
                            break;
                        }

                        k++;
                       
                    }
                    this.dati_note.AcceptChanges();
                
                }

                catch ( Exception ex )
                {
                    string log = ex.Message;

                    MessageBox.Show( "Solo le righe inserite dall'utente possono essere cancellate" );
                }
            }
        }

		

		private void DataGrid_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            DataGrid grd = (DataGrid)sender;
            grd.CommitEdit(DataGridEditingUnit.Cell, true);
        }

        public T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                string controlName = child.GetValue(Control.NameProperty) as string;

                if (controlName == name)
                {
                    return child as T;
                }

                else
                {
                    T result = FindVisualChildByName<T>(child, name);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        { 
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
            }
        }

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				e.Cancel = true;
				return;
			}
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

            string head = "";
            if (e.RemovedItems.Count > 0)
            {

                head = ((TabItem)tabControl.Items[CurrentTabSelectedIndex]).Header.ToString();
                foreach (DataRow dtrow in dati.Rows)
                {
                    dtrow["Header"] = head;
                }
                datiT = cBusinessObjects.SetDataFiltered(dati, datiT, head, "Header");
                foreach (DataRow dtrow in dati_note.Rows)
                {
                    dtrow["Header"] = head;
                }
                dati_noteT = cBusinessObjects.SetDataFiltered(dati_note, dati_noteT, head, "Header");
            }


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

				
				}
				else
				{
                    dati = cBusinessObjects.GetDataFiltered(datiT, ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString(), "Header");
                   
                    Binding b = new Binding();
                    b.Source = dati;
                    dtgRitenuteLavoratoriAutonomi.SetBinding(ItemsControl.ItemsSourceProperty, b);

                    dati_note = cBusinessObjects.GetDataFiltered(dati_noteT, ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString(), "Header");
                    b = new Binding();
                    b.Source = dati_note;
                    dtgRitenuteLavoratoriAutonomiNote.SetBinding(ItemsControl.ItemsSourceProperty, b);

				}
			}
            CurrentTabSelectedIndex = tabControl.SelectedIndex;
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

                dati.Clear();
                datiT = cBusinessObjects.SetDataFiltered(dati, datiT, newHeader, "Header");

                dati_note.Clear();
                dati_noteT = cBusinessObjects.SetDataFiltered(dati_note, dati_noteT, newHeader, "Header");



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

			Binding b = new Binding();
			b.Source =dati;
			dtgRitenuteLavoratoriAutonomi.SetBinding(ItemsControl.ItemsSourceProperty, b);

            b = new Binding();
            b.Source = dati_note;
            dtgRitenuteLavoratoriAutonomiNote.SetBinding( ItemsControl.ItemsSourceProperty, b );
            
			((TabItem)(tabControl.SelectedItem)).Header = newHeader;
			//txtHeader.Text = newHeader;
		}
        
        //public string ReplaceXml(string valore)
        //{
        //    string returnvalue = valore;

        //    returnvalue = returnvalue.Replace(" ", "").Replace("'", "").Replace("<", "").Replace("/", "").Replace("\\", "").Replace(">", "").Replace("\"", "");

        //    return returnvalue;

        //}

		private void ChangeNameTab(string newname, string oldheader)
		{
            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["Header"].ToString() == oldheader)
                {
                    dtrow["Header"] = newname;
                }
            }
            datiT = cBusinessObjects.SetDataFiltered(dati, datiT, oldheader, "Header");

            foreach (DataRow dtrow in dati_note.Rows)
            {
                if (dtrow["Header"].ToString() == oldheader)
                {
                    dtrow["Header"] = newname;
                }
            }
            dati_noteT = cBusinessObjects.SetDataFiltered(dati_note, dati_noteT, oldheader, "Header");
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

                foreach (DataRow dtrow in dati.Rows)
                {
                    dtrow["Header"] = "temp";
                }
                datiT = cBusinessObjects.SetDataFiltered(dati, datiT, sourceHeader, "Header");

                DataTable dati2 = cBusinessObjects.GetDataFiltered(datiT, targetHeader, "Header");
                foreach (DataRow dtrow in dati.Rows)
                {
                    dtrow["Header"] = sourceHeader;
                }
                datiT = cBusinessObjects.SetDataFiltered(dati, datiT, targetHeader, "Header");

                datiT = cBusinessObjects.SetDataFiltered(dati2, datiT, "temp", "Header");
                dati = cBusinessObjects.GetDataFiltered(datiT, targetHeader, "Header");

                foreach (DataRow dtrow in dati_note.Rows)
                {
                    dtrow["Header"] = "temp";
                }
                dati_noteT = cBusinessObjects.SetDataFiltered(dati_note, dati_noteT, sourceHeader, "Header");

                dati2 = cBusinessObjects.GetDataFiltered(dati_noteT, targetHeader, "Header");
                foreach (DataRow dtrow in dati_note.Rows)
                {
                    dtrow["Header"] = sourceHeader;
                }
                dati_noteT = cBusinessObjects.SetDataFiltered(dati_note, dati_noteT, targetHeader, "Header");

                dati_noteT = cBusinessObjects.SetDataFiltered(dati2, dati_noteT, "temp", "Header");
                dati_note = cBusinessObjects.GetDataFiltered(dati_noteT, targetHeader, "Header");

                tabItemTarget.Header = sourceHeader;
                tabItemSource.Header = targetHeader;
                Binding b = new Binding();
                b.Source = dati;
                dtgRitenuteLavoratoriAutonomi.SetBinding(ItemsControl.ItemsSourceProperty, b);


                b = new Binding();
                b.Source = dati_note;
                dtgRitenuteLavoratoriAutonomiNote.SetBinding(ItemsControl.ItemsSourceProperty, b);

            }
        }

        private void tabControl_PreviewMouseDown( object sender, MouseButtonEventArgs e )
        {
            dtgRitenuteLavoratoriAutonomi.Focus();
        }
    }
}
