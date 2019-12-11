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
	public partial class ucExcel_Affidamenti : UserControl
    {
        public int id;
        private DataTable dati = null;
        private DataTable datiT = null;
        private int CurrentTabSelectedIndex = 0;
        private int indexselcell =-1;
        
     

		private bool _ReadOnly = false;

		public ucExcel_Affidamenti()
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
            string head = "";
            datiT = cBusinessObjects.GetData(id, typeof(Excel_Affidamenti));
            datiT.Columns.Add("bold", typeof(bool));  
            foreach (DataRow dtrow in datiT.Rows)
            {
                if (dtrow["banca"].ToString() != "")
                {
                    head = dtrow["banca"].ToString();
                    break;
                }
            }

            dati = cBusinessObjects.GetDataFiltered(datiT, head, "banca");

            bool trovata = false;

            foreach (DataRow dtrow in datiT.Rows)
            {
                if (dtrow["banca"] !=null)
                {
                    if (!Al.Contains(dtrow["banca"].ToString()))
                    {
                        Al.Add(dtrow["banca"].ToString());
                        trovata = true;
                    }
                }
            }
            if(!trovata)
            {
                Al.Add("Sconosciuta");

            }
           

			if (Al.Count == 0)
			{
				Al.Add("Sconosciuta");
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
                 dtrow["banca"] = ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString();
            }
            datiT = cBusinessObjects.SetDataFiltered(dati, datiT, ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString(), "banca");
            datiT.Columns.Remove("bold");
            return cBusinessObjects.SaveData(id, datiT, typeof(Excel_Affidamenti));
        }

		public void LoadRiepilogo()
		{
			int row = 0;

			Border brd;
			TextBlock txt;
			RowDefinition rd;

			rd = new RowDefinition();
			grdRiepilogo.RowDefinitions.Add(rd);
			row++;

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.BorderBrush = Brushes.LightGray;
			brd.Background = Brushes.LightGray;
			brd.Padding = new Thickness(2.0);

			txt = new TextBlock();
			txt.Text = "Tipo Affidamento";
			txt.TextAlignment = TextAlignment.Left;
			txt.TextWrapping = TextWrapping.Wrap;

			brd.Child = txt;

			grdRiepilogo.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 0);

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.BorderBrush = Brushes.LightGray;
			brd.Background = Brushes.LightGray;
			brd.Padding = new Thickness(2.0);

			txt = new TextBlock();
			txt.Text = "ad inizio esercizio";
			txt.TextAlignment = TextAlignment.Left;
			txt.TextWrapping = TextWrapping.Wrap;

			brd.Child = txt;

			grdRiepilogo.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 1);

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.BorderBrush = Brushes.LightGray;
			brd.Background = Brushes.LightGray;
			brd.Padding = new Thickness(2.0);

			txt = new TextBlock();
			txt.Text = "alla data verifica";
			txt.TextAlignment = TextAlignment.Left;
			txt.TextWrapping = TextWrapping.Wrap;

			brd.Child = txt;

			grdRiepilogo.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 2);

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.BorderBrush = Brushes.LightGray;
			brd.Background = Brushes.LightGray;
			brd.Padding = new Thickness(2.0);

			txt = new TextBlock();
			txt.Text = "utilizzo";
			txt.TextAlignment = TextAlignment.Left;
			txt.TextWrapping = TextWrapping.Wrap;

			brd.Child = txt;

			grdRiepilogo.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 3);

			List<string> Alias = new List<string>();
			
			Alias.Add("a");
			Alias.Add("b");
			Alias.Add("c");
			Alias.Add("d");
			Alias.Add("e");
			Alias.Add("f");
			Alias.Add("g");
			Alias.Add("h");
			Alias.Add("i");
			Alias.Add("l");
			Alias.Add("m");
			Alias.Add("n");

			Hashtable htAlias = new Hashtable();
			htAlias.Add("a", "conto corrente");
			htAlias.Add("b", "sbf - riba");
			htAlias.Add("c", "anticipo fatture");
			htAlias.Add("d", "anticipo export");
			htAlias.Add("e", "anticipo import");
			htAlias.Add("f", "chirografario");
			htAlias.Add("g", "mutui ipotecari o similari");
			htAlias.Add("h", "operazioni a termine");
			htAlias.Add("i", "finanza derivata");
			htAlias.Add("l", "garanzie prestate");
			htAlias.Add("m", "altro 1");
			htAlias.Add("n", "altro 2");

			Hashtable htInizio = new Hashtable();
			Hashtable htFine = new Hashtable();
			Hashtable htUtilizzo = new Hashtable();


            foreach (DataRow dtrow in dati.Rows)
            {
				if (dtrow["tipoaffidamento"] == null)
				{
					continue;
				}

				if (dtrow["tipoaffidamento"].ToString() == "")
				{
					continue;
				}

				if (!htInizio.Contains(dtrow["tipoaffidamento"].ToString()))
				{
					htInizio.Add(dtrow["tipoaffidamento"].ToString(), 0.0);
				}

				if (dtrow["inizio"] != null)
				{
					double value = 0.0;
					double.TryParse(dtrow["inizio"].ToString(), out value);
					htInizio[dtrow["tipoaffidamento"].ToString()] = (double)htInizio[dtrow["tipoaffidamento"].ToString()] + value;
				}

				if (!htFine.Contains(dtrow["tipoaffidamento"].ToString()))
				{
					htFine.Add(dtrow["tipoaffidamento"].ToString(), 0.0);
				}

				if (dtrow["dataverifica"] != null)
				{
					double value = 0.0;
					double.TryParse(dtrow["dataverifica"].ToString(), out value);
					htFine[dtrow["tipoaffidamento"].ToString()] = (double)htFine[dtrow["tipoaffidamento"].ToString()] + value;
				}

				if (!htUtilizzo.Contains(dtrow["tipoaffidamento"].ToString()))
				{
					htUtilizzo.Add(dtrow["tipoaffidamento"].ToString(), 0.0);
				}

				if (dtrow["utilizzo"] != null)
				{
					double value = 0.0;
					double.TryParse(dtrow["utilizzo"].ToString(), out value);
					htUtilizzo[dtrow["tipoaffidamento"].ToString()] = (double)htUtilizzo[dtrow["tipoaffidamento"].ToString()] + value;
				}
			}
            foreach (DataRow dtrow in datiT.Rows)
            {
                if (dtrow["banca"].ToString() == ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString())
                    continue;

				if (dtrow["tipoaffidamento"] == null)
				{
					continue;
				}

				if (dtrow["tipoaffidamento"].ToString() == "")
				{
					continue;
				}

				if (!htInizio.Contains(dtrow["tipoaffidamento"].ToString()))
				{
					htInizio.Add(dtrow["tipoaffidamento"].ToString(), 0.0);
				}

				if (dtrow["inizio"] != null)
				{
					double value = 0.0;
					double.TryParse(dtrow["inizio"].ToString(), out value);
					htInizio[dtrow["tipoaffidamento"].ToString()] = (double)htInizio[dtrow["tipoaffidamento"].ToString()] + value;
				}

				if (!htFine.Contains(dtrow["tipoaffidamento"].ToString()))
				{
					htFine.Add(dtrow["tipoaffidamento"].ToString(), 0.0);
				}

				if (dtrow["dataverifica"] != null)
				{
					double value = 0.0;
					double.TryParse(dtrow["dataverifica"].ToString(), out value);
					htFine[dtrow["tipoaffidamento"].ToString()] = (double)htFine[dtrow["tipoaffidamento"].ToString()] + value;
				}

				if (!htUtilizzo.Contains(dtrow["tipoaffidamento"].ToString()))
				{
					htUtilizzo.Add(dtrow["tipoaffidamento"].ToString(), 0.0);
				}

				if (dtrow["utilizzo"] != null)
				{
					double value = 0.0;
					double.TryParse(dtrow["utilizzo"].ToString(), out value);
					htUtilizzo[dtrow["tipoaffidamento"].ToString()] = (double)htUtilizzo[dtrow["tipoaffidamento"].ToString()] + value;
				}
			}



			foreach (string item in Alias)
			{
				rd = new RowDefinition();
				grdRiepilogo.RowDefinitions.Add(rd);
				row++;

				brd = new Border();
				brd.BorderThickness = new Thickness(1.0);
				brd.BorderBrush = Brushes.LightGray;
				if (row % 2 == 0)
				{
					brd.Background = new SolidColorBrush(Color.FromArgb(126, 241, 241, 241));
				}
				else
				{
					brd.Background = Brushes.White;
				}

				brd.Padding = new Thickness(2.0);

				txt = new TextBlock();
				txt.Text = htAlias[item].ToString();
				txt.TextAlignment = TextAlignment.Left;
				txt.TextWrapping = TextWrapping.Wrap;

				brd.Child = txt;

				grdRiepilogo.Children.Add(brd);
				Grid.SetRow(brd, row);
				Grid.SetColumn(brd, 0);

				brd = new Border();
				brd.BorderThickness = new Thickness(1.0);
				brd.BorderBrush = Brushes.LightGray;
				if (row % 2 == 0)
				{
					brd.Background = new SolidColorBrush(Color.FromArgb(126, 241, 241, 241));
				}
				else
				{
					brd.Background = Brushes.White;
				}

				brd.Padding = new Thickness(2.0);

				txt = new TextBlock();
				double valore = 0.0;
				if (htInizio.Contains(item))
				{
					valore = (double)htInizio[item];
				}
				txt.Text = cBusinessObjects.ConvertNumber(valore.ToString());
				txt.TextAlignment = TextAlignment.Right;
				txt.TextWrapping = TextWrapping.Wrap;

				brd.Child = txt;

				grdRiepilogo.Children.Add(brd);
				Grid.SetRow(brd, row);
				Grid.SetColumn(brd, 1);

				brd = new Border();
				brd.BorderThickness = new Thickness(1.0);
				brd.BorderBrush = Brushes.LightGray;
				if (row % 2 == 0)
				{
					brd.Background = new SolidColorBrush(Color.FromArgb(126, 241, 241, 241));
				}
				else
				{
					brd.Background = Brushes.White;
				}

				brd.Padding = new Thickness(2.0);

				txt = new TextBlock();
				valore = 0.0;
				if (htFine.Contains(item))
				{
					valore = (double)htFine[item];
				}
				txt.Text = cBusinessObjects.ConvertNumber(valore.ToString());
                txt.TextAlignment = TextAlignment.Right;
				txt.TextWrapping = TextWrapping.Wrap;

				brd.Child = txt;

				grdRiepilogo.Children.Add(brd);
				Grid.SetRow(brd, row);
				Grid.SetColumn(brd, 2);

				brd = new Border();
				brd.BorderThickness = new Thickness(1.0);
				brd.BorderBrush = Brushes.LightGray;
				if (row % 2 == 0)
				{
					brd.Background = new SolidColorBrush(Color.FromArgb(126, 241, 241, 241));
				}
				else
				{
					brd.Background = Brushes.White;
				}

				brd.Padding = new Thickness(2.0);

				txt = new TextBlock();
				valore = 0.0;
				if (htUtilizzo.Contains(item))
				{
					valore = (double)htUtilizzo[item];
				}
				txt.Text = cBusinessObjects.ConvertNumber(valore.ToString());
                txt.TextAlignment = TextAlignment.Right;
				txt.TextWrapping = TextWrapping.Wrap;

				brd.Child = txt;

				grdRiepilogo.Children.Add(brd);
				Grid.SetRow(brd, row);
				Grid.SetColumn(brd, 3);
			}

			rd = new RowDefinition();
			grdRiepilogo.RowDefinitions.Add(rd);
			row++;

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.BorderBrush = Brushes.LightGray;
			brd.Background = Brushes.LightGray;
			brd.Padding = new Thickness(2.0);

			txt = new TextBlock();
			txt.Text = "Totale";
			txt.TextAlignment = TextAlignment.Left;
			txt.TextWrapping = TextWrapping.Wrap;

			brd.Child = txt;

			grdRiepilogo.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 0);

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.BorderBrush = Brushes.LightGray;
			brd.Background = Brushes.LightGray;
			brd.Padding = new Thickness(2.0);

			txt = new TextBlock();

			double somma = 0.0;
			foreach (DictionaryEntry item in htInizio)
			{
				somma += (double)(item.Value);
			}
			txt.Text = cBusinessObjects.ConvertNumber(somma.ToString());
			txt.TextAlignment = TextAlignment.Right;
			txt.TextWrapping = TextWrapping.Wrap;

			brd.Child = txt;

			grdRiepilogo.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 1);

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.BorderBrush = Brushes.LightGray;
			brd.Background = Brushes.LightGray;
			brd.Padding = new Thickness(2.0);

			txt = new TextBlock();
			somma = 0.0;
			foreach (DictionaryEntry item in htFine)
			{
				somma += (double)(item.Value);
			}
			txt.Text = cBusinessObjects.ConvertNumber(somma.ToString());
            txt.TextAlignment = TextAlignment.Right;
			txt.TextWrapping = TextWrapping.Wrap;

			brd.Child = txt;

			grdRiepilogo.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 2);

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.BorderBrush = Brushes.LightGray;
			brd.Background = Brushes.LightGray;
			brd.Padding = new Thickness(2.0);

			txt = new TextBlock();
			somma = 0.0;
			foreach (DictionaryEntry item in htUtilizzo)
			{
				somma += (double)(item.Value);
			}
			txt.Text = cBusinessObjects.ConvertNumber(somma.ToString());
            txt.TextAlignment = TextAlignment.Right;
			txt.TextWrapping = TextWrapping.Wrap;

			brd.Child = txt;

			grdRiepilogo.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 3);
		}

		private void AggiungiNodo(string Alias, string Codice)
        {
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}

            if ( tabControl.SelectedItem == null)
            {
                return;
            }
            dati.Rows.Add(id, cBusinessObjects.idcliente,cBusinessObjects.idsessione, ((TabItem)(tabControl.SelectedItem)).Header);

			
        }



        private void GenerateTotal()
        {
            if (tabControl.SelectedItem == null)
            {
                return;
            }

            if (dtgAffidamenti.SelectedCells.Count>0)
                indexselcell= dtgAffidamenti.Items.IndexOf(dtgAffidamenti.SelectedCells[0].Item);

            ((TabItem)(tabControl.SelectedItem)).Header = ((TabItem)(tabControl.SelectedItem)).Header.ToString();



            double totaleinizio = 0.0;
            double totaledataverifica = 0.0;
            double totaleutilizzo = 0.0;

            DeleteTotal();


            foreach (DataRow dtrow in this.dati.Rows)
            {
                if ((dtrow["banca"] != null) && (dtrow["banca"].ToString() == (string)((TabItem)(tabControl.SelectedItem)).Header))
                {
                    double inizio = 0.0;
                    double dataverifica = 0.0;
                    double utilizzo = 0.0;

                    double.TryParse(dtrow["inizio"].ToString(), out inizio);
                    double.TryParse(dtrow["dataverifica"].ToString(), out dataverifica);
                    double.TryParse(dtrow["utilizzo"].ToString(), out utilizzo);

                    totaleinizio += inizio;
                    totaledataverifica += dataverifica;
                    totaleutilizzo += utilizzo;
                }
            }
           
            DataRow dd = dati.Rows.Add(id,cBusinessObjects.idcliente,cBusinessObjects.idsessione);
            dd["scadenza"] = "Totale";
            dd["inizio"] = cBusinessObjects.ConvertNumber(totaleinizio.ToString());
            dd["dataverifica"] = cBusinessObjects.ConvertNumber(totaledataverifica.ToString());
            dd["utilizzo"] = cBusinessObjects.ConvertNumber(totaleutilizzo.ToString());
            dd["bold"] = true;
            dd["banca"] = ((TabItem)(tabControl.SelectedItem)).Header;
	
			grdRiepilogo.Children.Clear();

			LoadRiepilogo();
        }

         private void DeleteTotal()
            {
    
                foreach (DataRow dtrow in this.dati.Rows)
                {
                            if ("Totale" == dtrow["scadenza"].ToString())
                            {
                                dtrow.Delete();
                                break;
                            }
                }
                dati.AcceptChanges();

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
			AggiungiNodo("",  "");

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
				

				if (indexselcell <0)
				{
					
					MessageBox.Show("Selezionare una riga");
					return;
				}

				try
				{
                    int k = 0;
                    foreach (DataRow dtrow in this.dati.Rows)
                    {
                       if (k ==indexselcell)
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

		private void txtDataDiPagamento_LostFocus(object sender, RoutedEventArgs e)
		{
			string newHeader = txtBancaAgenzia.Text;
            txtBancaAgenzia.Text = newHeader;

            if ( tabControl.SelectedItem == null )
            {
                return;
            }

			if (((TabItem)(tabControl.SelectedItem)).Header.ToString() != newHeader)
			{
				foreach (TabItem item in tabControl.Items)
				{
					if (((string)(item.Header)) == newHeader)
					{
						MessageBox.Show("Attenzione, Nome già esistente");
						tabControl.SelectedIndex = 0;
						return;
					}
				}

                if ( tabControl.SelectedItem == null )
                {
                    return;
                }

				((TabItem)(tabControl.SelectedItem)).Header = newHeader;


               
                foreach (DataRow dtrow in dati.Rows)
     			{
				 dtrow["banca"] = newHeader;
				}
            

                Binding b = new Binding();
				b.Source = dati;
				dtgAffidamenti.SetBinding(ItemsControl.ItemsSourceProperty, b);

				
			}
		}

		private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            

         
            dtgAffidamenti.Focus();
            string head = "";
            if (e.RemovedItems.Count > 0)
            {
             
                head = ((TabItem)tabControl.Items[CurrentTabSelectedIndex]).Header.ToString();
                foreach (DataRow dtrow in dati.Rows)
                {
                    dtrow["banca"] = head;
                }
                datiT = cBusinessObjects.SetDataFiltered(dati, datiT, head, "banca");
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

					var dialog = new wInputBox("Inserire Nome della nuova Banca / Agenzia");
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

                    dati = cBusinessObjects.GetDataFiltered(datiT, ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString(), "banca");
                    if (dati.Rows.Count == 0)
                    {
                        AggiungiNodo("", "");

                    }
                    Binding b = new Binding();
					b.Source = dati;
					dtgAffidamenti.SetBinding(ItemsControl.ItemsSourceProperty, b);
					txtBancaAgenzia.Text = ((string)(((TabItem)(e.AddedItems[0])).Header));
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

                if (MessageBox.Show("La tabella verrà cancellata. Procedere?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    string newHeader = txtBancaAgenzia.Text;
                    dati.Clear();
                    datiT = cBusinessObjects.SetDataFiltered(dati, datiT, newHeader, "banca");

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

			Binding b = new Binding();
			b.Source = dati;
			dtgAffidamenti.SetBinding(ItemsControl.ItemsSourceProperty, b);


			((TabItem)(tabControl.SelectedItem)).Header = newHeader;
			txtBancaAgenzia.Text = newHeader;
		}

		private void ChangeNameTab(string newname, string oldheader)
		{

            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["banca"].ToString() == oldheader)
                {
                    dtrow["banca"] = newname;
                }
            }
            datiT = cBusinessObjects.SetDataFiltered(dati, datiT, oldheader, "banca");
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
                    dtrow["banca"] = "temp";
                }
                datiT = cBusinessObjects.SetDataFiltered(dati, datiT, sourceHeader, "banca");

                DataTable dati2 = cBusinessObjects.GetDataFiltered(datiT, targetHeader, "banca");
                foreach (DataRow dtrow in dati.Rows)
                {
                    dtrow["banca"] = sourceHeader;
                }
                datiT = cBusinessObjects.SetDataFiltered(dati, datiT, targetHeader, "banca");

                datiT = cBusinessObjects.SetDataFiltered(dati2, datiT, "temp", "banca");
                dati = cBusinessObjects.GetDataFiltered(datiT, targetHeader, "banca");
                tabItemTarget.Header = sourceHeader;
                tabItemSource.Header = targetHeader;
                Binding b = new Binding();
                b.Source = dati;
                dtgAffidamenti.SetBinding(ItemsControl.ItemsSourceProperty, b);
              

            }

        }

    }
}
