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
    public partial class ucExcel_ISQC_TempiLavoro_Riepilogo : UserControl
    {
        public int id;
       

        private DataTable dati = null;


        private bool _ReadOnly = false;
        
        public ucExcel_ISQC_TempiLavoro_Riepilogo()
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
        public void LoadDataSource(string ID, string IDCliente, string IDSessione, string titolo)
        {

            id = int.Parse(ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
            txtTitolo.Text = titolo;

       
            dati = cBusinessObjects.GetData(id, typeof(Excel_ISQC_TempiLavoro_Riepilogo));
            DeleteAll();

            AggiungiNodo("Comprensione - rischio - pianificazione", "181");
            AggiungiNodo("Controllo del Bilancio", "182");
            AggiungiNodo("Canclusioni - review - Relazione","183");
            AggiungiNodo("Altre attività", "184");
            AggiungiNodo("Verifiche periodiche",  "185");

            GenerateTotal();

            Binding b = new Binding();
            b.Source =dati;
            dtgISQC.SetBinding(ItemsControl.ItemsSourceProperty, b);
        }

        public int Save()
        {
            return cBusinessObjects.SaveData(id, dati, typeof(Excel_ISQC_TempiLavoro_Riepilogo));
        }


        private void AggiungiNodo(string Alias,  string ID_Riferimento)
        {
        
            string spercentuale = "0,00%";

            double totalepreviste = 0.0;
            double totaleeffettive = 0.0;
            double totalescostamento = 0.0;
            double totalepercentuale = 0.0;
            DataTable datirif = cBusinessObjects.GetData(int.Parse(ID_Riferimento), typeof(Excel_ISQC_TempiLavoro));
            foreach (DataRow dtrow in datirif.Rows)
            {
                
                double previste = 0.0;
                double effettive = 0.0;
                double scostamento = 0.0;
                double percentuale = 0.0;
                if(dtrow["previste"]!=null)
                   double.TryParse(dtrow["previste"].ToString(), out previste);
                if (dtrow["effettive"] != null)
                    double.TryParse(dtrow["effettive"].ToString(), out effettive);

                scostamento = effettive - previste;
                percentuale = ((previste != 0) ? (effettive - previste) / previste * 100.0  : 0);

             //   dtrow["scostamento"] = cBusinessObjects.ConvertNumber(scostamento.ToString());
             //  dtrow["percentuale"]= ConvertNumberWithDecimal(percentuale.ToString());

                totalepreviste += previste;
                totaleeffettive += effettive;
                totalescostamento += scostamento;
            }

            totalepercentuale = ((totalepreviste != 0) ? (totaleeffettive - totalepreviste) / totalepreviste * 100.0 : 0) ;

  
            spercentuale = totalepercentuale.ToString();

            dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Alias, totalepreviste, totaleeffettive, totalescostamento, spercentuale);
          


    }

        private void DeleteAll()
        {
            
            for (int i = dati.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dtrow = dati.Rows[i];
                dtrow.Delete();
            }
            this.dati.AcceptChanges();
        }


        private void DeleteTotal()
        {
            for (int i = dati.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dtrow = dati.Rows[i];
                if (dtrow["titolo"].ToString() == "Totale")
                    dtrow.Delete();
            }
            this.dati.AcceptChanges();
        }

        private void GenerateTotal()
		{
			DeleteTotal();

            double totalepreviste = 0.0;
            double totaleeffettive = 0.0;
            double totalescostamento = 0.0;
            double totalepercentuale = 0.0;

            if (!_ReadOnly && dati.Rows.Count == 0)
            {
                AggiungiNodo("", "");
            }

            foreach (DataRow dtrow in this.dati.Rows)
            {
                double previste = 0.0;
                double effettive = 0.0;
                double scostamento= 0.0;
                double percentuale = 0.0;
                if(dtrow["previste"]!=null)
                   double.TryParse(dtrow["previste"].ToString(), out previste);
                if (dtrow["effettive"] != null)
                    double.TryParse(dtrow["effettive"].ToString(), out effettive);

                scostamento = effettive - previste;
                percentuale = ((previste != 0)? (effettive - previste) / previste * 100.0 : 0);
                //percentuale = previste / effettive;

                dtrow["scostamento"] =scostamento;
                dtrow["percentuale"] = ConvertNumberWithDecimal(percentuale.ToString());

                totalepreviste += previste;
                totaleeffettive += effettive;
                totalescostamento += scostamento;
			}

            //totalepercentuale = totalepreviste / totaleeffettive;
            totalepercentuale = ((totalepreviste != 0) ? (totaleeffettive - totalepreviste) / totalepreviste * 100.0 : 0);

            dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, "Totale", totalepreviste, totaleeffettive, totalescostamento, totalepercentuale.ToString());


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
				

				if (dtgISQC.SelectedCells.Count >= 1)
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
                        if (k == dtgISQC.Items.IndexOf(dtgISQC.SelectedCells[0].Item))
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
        
        private void tabControl_PreviewMouseDown( object sender, MouseButtonEventArgs e )
        {
            dtgISQC.Focus();
        }
    }
}
