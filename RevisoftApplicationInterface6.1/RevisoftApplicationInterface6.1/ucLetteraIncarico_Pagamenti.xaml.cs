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
	public partial class ucLetteraIncarico_Pagamenti : UserControl
    {
        public int id;
        private DataTable dati = null;
     
    

		private bool _ReadOnly = false;

        public ucLetteraIncarico_Pagamenti()
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

          

            dati = cBusinessObjects.GetData(id, typeof(Incarico_Pagamenti));
            foreach (DataRow dtrow in dati.Rows)
            {
                if (String.IsNullOrEmpty(dtrow["Incipit"].ToString()))
                    txtIncipit.Text = "";
                else
                    txtIncipit.Text = dtrow["Incipit"].ToString();
            }


            Binding b = new Binding();
            b.Source = dati;
			dtgPagamenti.SetBinding(ItemsControl.ItemsSourceProperty, b);

          
        }

		public int Save()
		{
            return cBusinessObjects.SaveData(id, dati, typeof(Incarico_Pagamenti));
        }


        private void AggiungiNodo(string Alias, string Codice)
        {
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}
            dati.Rows.Add(id,cBusinessObjects.idcliente,cBusinessObjects.idsessione);

            
                //_x.Save();

                //dtgCapitaleSociale.Items.Refresh();
           
        }

        private void DeleteTotal()
        {
            ;
        }

        private void GenerateTotal()
        {
            ;
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
                //ComboBox comboBox = FindVisualChildByName<ComboBox>(((DataGridCell)(e.OriginalSource)), "cmb");

                //if (comboBox != null)
                //{
                //    comboBox.Focus();
                //}
                //else
                {
                    DataGrid grd = (DataGrid)sender;
                    grd.BeginEdit(e);
                }
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

            //_x.Save();
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
			

				if (dtgPagamenti.SelectedCells.Count >= 1)
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
                        if (k == dtgPagamenti.Items.IndexOf(dtgPagamenti.SelectedCells[0].Item))
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

		private void txtIncipit_LostFocus(object sender, RoutedEventArgs e)
		{

            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["Incipit"] = txtIncipit.Text;
            }
        }

    }
}
