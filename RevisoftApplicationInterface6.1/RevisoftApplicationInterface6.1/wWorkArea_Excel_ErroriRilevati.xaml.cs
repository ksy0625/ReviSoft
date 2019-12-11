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
	public partial class uc_Excel_ErroriRilevati : UserControl
    {

        public int id;
        private DataTable dati = null;
     

		private bool _ReadOnly = false;

		public uc_Excel_ErroriRilevati()
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
       
            dati= cBusinessObjects.GetData(id, typeof(Excel_ErroriRilevati));
          

            Binding b = new Binding();
            b.Source = dati;
            dtgErroriRilevati.SetBinding(ItemsControl.ItemsSourceProperty, b);
        }

		public int Save()
		{
            return cBusinessObjects.SaveData(id, dati, typeof(Excel_ErroriRilevati));
        }

        private void AggiungiNodo(string Alias)
        {
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}
            bool trovata = false;
            if (Alias != "")
            {
               
                foreach (DataRow dtrow in dati.Rows)
                {
                    if (dtrow["name"].ToString() == Alias)
                        trovata = true;
                }
            }

            if (trovata)
                return;            
             dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Alias);
        }

        private void DeleteTotal()
        {
            foreach (DataRow dtrow in this.dati.Rows)
            {
                if ("Totale" == dtrow["name"].ToString())
                {
                    dtrow.Delete();
                    break;
                }
            }
            this.dati.AcceptChanges();
        }

        private void GenerateTotal()
        {
            DeleteTotal();

            double importo = 0.0;
            double impattofiscale = 0.0;

           

            foreach (DataRow dtrow in this.dati.Rows)
            {
                importo += Convert.ToDouble(dtrow["importo"].ToString());
                impattofiscale += Convert.ToDouble(((dtrow["impattofiscale"] != null) ? dtrow["impattofiscale"].ToString() : "0.0"));

            }
            dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, "Totale", importo, impattofiscale);

         
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
            AggiungiNodo("");

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
				

				if (dtgErroriRilevati.SelectedCells.Count >= 1)
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
                        if (k == dtgErroriRilevati.Items.IndexOf(dtgErroriRilevati.SelectedCells[0].Item))
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
                    cBusinessObjects.logger.Error(ex, "wWorkArea_Excel_ErroriRilevati.DeleteRowErroriRilevati exception");
                    string log = ex.Message;

					MessageBox.Show("Solo le righe inserite dall'utente possono essere cancellate");
				}
			}
        }

        private void dtgErroriRilevati_KeyUp( object sender, KeyEventArgs e )
        {
            if ( e.Key == Key.Enter )
            {
                GenerateTotal();
            }
        }
    }
}
