using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Xml;
using System.Collections;
using RevisoftApplication;
using System.Data;


namespace UserControls
{
    public partial class ucExcel_ISQC_TempiLavoro : UserControl
    {
        public int id;
        public int idcliente;
        public int idsessione;

        private DataTable dati = null;
 

        private bool _ReadOnly = false;

        GenericTable gtCOGE = null;

        public ucExcel_ISQC_TempiLavoro()
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

        public void LoadDataSource(string ID, string IDCliente, string IDSessione,string titolo)
        {

            id = int.Parse(ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
            txtTitolo.Text = titolo;
        

            ArrayList Al = new ArrayList();
            dati = cBusinessObjects.GetData(id, typeof(Excel_ISQC_TempiLavoro));
            foreach (DataRow dtrow in dati.Rows)
            {
                if (!Al.Contains(dtrow["Header"].ToString()))
                {
                    Al.Add(dtrow["Header"].ToString());
                }
            }
       

			if (Al.Count == 0)
			{
                Al.Add("ISQC");
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

            gtCOGE = new GenericTable(tblISQC, _ReadOnly);

            gtCOGE.ColumnsAlias = new string[] { "Data", "Descrizione Attività", "Ore Previste", "Ore Effettive", "Scostamento", "%" };
            gtCOGE.ColumnsValues = new string[] { "data", "esecutore", "previste", "effettive", "scostamento", "percentuale" };
            gtCOGE.ColumnsWidth = new double[] { 2.0, 10.0, 2.0, 2.0, 2.0, 1.0 };
            gtCOGE.ColumnsMinWidth = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            gtCOGE.ColumnsTypes = new string[] { "string", "string", "int", "int", "int", "percent" };
            gtCOGE.ColumnsAlignment = new string[] { "left", "left", "right", "right", "right", "right" };
            gtCOGE.ColumnsReadOnly = new bool[] { false, false, false, false, true, true };
            gtCOGE.ConditionalReadonly = new bool[] { false, false, false, false, false, false };
            gtCOGE.ConditionalAttribute = "new";
            gtCOGE.ColumnsHasTotal = new bool[] { false, false, true, true, true, false };
            gtCOGE.AliasTotale = "Totale";
            gtCOGE.ColumnAliasTotale = 1;
            gtCOGE.xml = false;
            gtCOGE.dati = dati;
            gtCOGE.TotalToBeCalculated += GtCOGE_TotalToBeCalculated;
        }


        public int Save()
        {
            return cBusinessObjects.SaveData(id,dati, typeof(Excel_ISQC_TempiLavoro));
        }


        private void AggiungiNodo(string Alias,string Codice)
        {
            dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, ((TabItem)(tabControl.SelectedItem)).Header);
            gtCOGE.AddRow();
        }

        private void GtCOGE_TotalToBeCalculated_Intrernal()
        {
            for (int i = 0; i < gtCOGE.row; i++)
            {
                double previste = 0.0;
                double effettive = 0.0;
                double scostamento = 0.0;
                double percentuale = 0.0;

                double.TryParse(gtCOGE.GetValue("2", i.ToString()), out previste);
                double.TryParse(gtCOGE.GetValue("3", i.ToString()), out effettive);

                scostamento = effettive - previste;
                percentuale = ((previste != 0) ? (effettive - previste) / previste * 100.0 : 0);

                gtCOGE.SetValue("4", i.ToString(), ConvertNumber(scostamento.ToString()));
                gtCOGE.SetValue("5", i.ToString(), ConvertNumberWithDecimal(percentuale.ToString()));
            }
        }

        private void GtCOGE_TotalToBeCalculated(object sendername, EventArgs e)
        {
            GtCOGE_TotalToBeCalculated_Intrernal();

            //if (((string)sendername).Split('_').Count() < 2)
            //{
            //    return;
            //}

            //string idcolumn = ((string)sendername).Split('_')[1];
            //string idrow = ((string)sendername).Split('_')[2];

            //if (idcolumn == "2" || idcolumn == "3")
            //{
            //    double previste = 0.0;
            //    double effettive = 0.0;
            //    double scostamento = 0.0;
            //    double percentuale = 0.0;

            //    double.TryParse(gtCOGE.GetValue("2", idrow), out previste);
            //    double.TryParse(gtCOGE.GetValue("3", idrow), out effettive);

            //    scostamento = effettive - previste;
            //    percentuale = ((previste != 0) ? (effettive - previste) / previste * 100.0 : 0);

            //    gtCOGE.SetValue("4", idrow, ConvertNumber(scostamento.ToString()));
            //    gtCOGE.SetValue("5", idrow, ConvertNumberWithDecimal(percentuale.ToString()));
            //}
        }
        
		private void AddRowErroriRilevati(object sender, RoutedEventArgs e)
		{
			AggiungiNodo("",  "");
		}

		private void DeleteRowErroriRilevati(object sender, RoutedEventArgs e)
		{
            gtCOGE.DeleteRow();
		}
        
		private string ConvertNumber(string valore)
		{
			double dblValore = 0.0;

			double.TryParse(valore, out dblValore);

			if (dblValore == 0.0)
			{
				return "";
			}
			else
			{
				return String.Format("{0:#,#}", dblValore);
			}
		}

        private string ConvertNumberWithDecimal(string valore)
        {
            double dblValore = 0.0;

            double.TryParse(valore, out dblValore);

            if (dblValore == 0.0)
            {
                return "";
            }
            else
            {
                return String.Format("{0:#,#.00}", dblValore);
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
                    gtCOGE.filtervalue = newHeader;
                    gtCOGE.GenerateTable();
                    AggiungiNodo("", "");
				}
				else
				{
                    gtCOGE.filtercolumn = "Header";
                    gtCOGE.filtervalue = ((string)(((TabItem)(e.AddedItems[0])).Header));
                    gtCOGE.GenerateTable();
                    GtCOGE_TotalToBeCalculated_Intrernal();

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

            if ( tabControl.SelectedItem == null)
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

       
            gtCOGE.filtercolumn = "Header";
            gtCOGE.filtervalue = newHeader;
            gtCOGE.GenerateTable();
            GtCOGE_TotalToBeCalculated_Intrernal();

            ((TabItem)(tabControl.SelectedItem)).Header = newHeader;
		}
       
		private void ChangeNameTab(string newname, string oldheader)
		{
            foreach (DataRow dtrow in dati.Rows)
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

                gtCOGE.filtercolumn = "Header";
                gtCOGE.filtervalue = targetHeader;
                gtCOGE.GenerateTable();
                GtCOGE_TotalToBeCalculated_Intrernal();

            }
		}

        private void tabControl_PreviewMouseDown( object sender, MouseButtonEventArgs e )
        {
            ;
        }
    }
}
