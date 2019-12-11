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
	public partial class ucExcel_ScrittureMagazzino : UserControl
    {
        public int id;
  
        private DataTable dati = null;
        private DataTable datiT = null;
        private DataTable dati_Note = null;
        private DataTable dati_NoteT = null;
        private int CurrentTabSelectedIndex = 0;



        private bool _ReadOnly = false;
        int numeroattuale = 1;

        public ucExcel_ScrittureMagazzino()
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
           
			ArrayList Al = new ArrayList();

         
            datiT = cBusinessObjects.GetData(id, typeof(Excel_ScrittureMagazzino));
            dati_NoteT = cBusinessObjects.GetData(id, typeof(Excel_ScrittureMagazzino_note));

            foreach (DataRow dtrow in datiT.Rows)
            {
                if (!Al.Contains(dtrow["Header"].ToString()))
                {
                    Al.Add(dtrow["Header"].ToString());
                }
            }

            string head = "";
            foreach (DataRow dtrow in datiT.Rows)
            {
                if (dtrow["Header"] != null)
                {
                    head = dtrow["Header"].ToString();
                    break;
                }
            }
            dati = cBusinessObjects.GetDataFiltered(datiT, head, "Header");
            dati_Note = cBusinessObjects.GetDataFiltered(dati_NoteT, head, "Header");

            if (Al.Count == 0)
			{
                Al.Add( "Registro" );
                if ( _ReadOnly )
                {
                    ;
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

        public int Save()
        {
            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["Header"] = ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString();
            }
            foreach (DataRow dtrow in dati_Note.Rows)
            {
                dtrow["Header"] = ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString();
            }
            datiT = cBusinessObjects.SetDataFiltered(dati, datiT, ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString(), "Header");
            dati_NoteT = cBusinessObjects.SetDataFiltered(dati_Note, dati_NoteT, ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString(), "Header");
            cBusinessObjects.SaveData(id, dati_NoteT, typeof(Excel_ScrittureMagazzino_note));
            return cBusinessObjects.SaveData(id, datiT, typeof(Excel_ScrittureMagazzino));
           
        }

        private void AggiungiNodo(string Alias, string Codice)
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
                    numeroattuale = numeroattuale + 1;
                }
            }

            dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, ((TabItem)(tabControl.SelectedItem)).Header, (numeroattuale).ToString());
        }

        private void AggiungiNodoNote( string Alias,  string Codice )
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
            dati_Note.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, ((TabItem)(tabControl.SelectedItem)).Header);

        }

        private void DeleteTotal()
        {
            for (int i = dati.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dtrow = dati.Rows[i];
                if ((dtrow["Header"].ToString() == ((TabItem)(tabControl.SelectedItem)).Header.ToString()) && (dtrow["Header"].ToString() == "Totale"))
                    dtrow.Delete();
            }
            dati.AcceptChanges();
        }

        private void GenerateTotal()
		{
            ;
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
			AggiungiNodo("", "");

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
				
				if (dtgScrittureMagazzino.SelectedCells.Count >= 1)
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
                        if (k == dtgScrittureMagazzino.Items.IndexOf(dtgScrittureMagazzino.SelectedCells[0].Item))
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
            AggiungiNodoNote( "",  "" );
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
               
                if ( dtgScrittureMagazzinoNote.SelectedCells.Count >= 1 )
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
                    foreach (DataRow dtrow in this.dati_Note.Rows)
                    {
                        if (k == dtgScrittureMagazzino.Items.IndexOf(dtgScrittureMagazzino.SelectedCells[0].Item))
                        {
                            dtrow.Delete();
                            break;
                        }

                        k++;

                    }
                    this.dati_Note.AcceptChanges();

                    return;
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
                dati_NoteT = cBusinessObjects.SetDataFiltered(dati_Note, dati_NoteT, head, "Header");
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


					GenerateTotal();
				}
				else
				{
                    dati = cBusinessObjects.GetDataFiltered(datiT, ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString(), "Header");
                    dati_Note = cBusinessObjects.GetDataFiltered(dati_NoteT, ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString(), "Header");

                    Binding b = new Binding();
                    b.Source = dati;
                    dtgScrittureMagazzino.SetBinding(ItemsControl.ItemsSourceProperty, b);
                    
                    b = new Binding();
                    b.Source = dati_Note;
                    dtgScrittureMagazzinoNote.SetBinding(ItemsControl.ItemsSourceProperty, b);
               
				}
			}
            CurrentTabSelectedIndex = tabControl.SelectedIndex;
        }

		private void btnDeleteTable_Click(object sender, RoutedEventArgs e)
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

			if (MessageBox.Show("La tabella verrà cancellata. Procedere?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
                string newHeader = ( (TabItem)( tabControl.SelectedItem ) ).Header.ToString();
                dati.Clear();
                datiT = cBusinessObjects.SetDataFiltered(dati, datiT, newHeader, "Header");
                dati.Clear();
                dati_NoteT = cBusinessObjects.SetDataFiltered(dati_Note, dati_NoteT, newHeader, "Header");

                

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
            b.Source = dati;
            dtgScrittureMagazzinoNote.SetBinding(ItemsControl.ItemsSourceProperty, b);

            b = new Binding();
            b.Source = dati_Note;
            dtgScrittureMagazzinoNote.SetBinding(ItemsControl.ItemsSourceProperty, b);

            ChangeNameTab(newHeader, ((TabItem)(tabControl.SelectedItem)).Header.ToString());

		
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
            foreach (DataRow dtrow in dati_Note.Rows)
            {
                if (dtrow["Header"].ToString() == oldheader)
                {
                    dtrow["Header"] = newname;
                }
            }
            dati_NoteT = cBusinessObjects.SetDataFiltered(dati_Note, dati_NoteT, oldheader, "Header");

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

                foreach (DataRow dtrow in dati_Note.Rows)
                {
                    dtrow["Header"] = "temp";
                }
                dati_NoteT = cBusinessObjects.SetDataFiltered(dati_Note, dati_NoteT, sourceHeader, "Header");
                dati2 = cBusinessObjects.GetDataFiltered(dati_NoteT, targetHeader, "Header");
                foreach (DataRow dtrow in dati_Note.Rows)
                {
                    dtrow["Header"] = sourceHeader;
                }
                dati_NoteT = cBusinessObjects.SetDataFiltered(dati_Note, dati_NoteT, targetHeader, "Header");

                dati_NoteT = cBusinessObjects.SetDataFiltered(dati2, dati_NoteT, "temp", "Header");
                dati_Note = cBusinessObjects.GetDataFiltered(dati_NoteT, targetHeader, "Header");


                tabItemTarget.Header = sourceHeader;
                tabItemSource.Header = targetHeader;
                Binding b = new Binding();
                b.Source = dati;
                dtgScrittureMagazzino.SetBinding(ItemsControl.ItemsSourceProperty, b);
                b = new Binding();
                b.Source = dati_Note;
                dtgScrittureMagazzinoNote.SetBinding(ItemsControl.ItemsSourceProperty, b);
             
			}
		}

        private void tabControl_PreviewMouseDown( object sender, MouseButtonEventArgs e )
        {
            dtgScrittureMagazzino.Focus();
        }
    }
}
