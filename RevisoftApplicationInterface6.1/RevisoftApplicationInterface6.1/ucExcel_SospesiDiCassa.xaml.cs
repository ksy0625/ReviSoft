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
	public partial class ucExcel_SospesiDiCassa : UserControl
    {
        public int id;
        private DataTable dati = null; 
    

		private bool _ReadOnly = false;

		public ucExcel_SospesiDiCassa()
        {
          
            InitializeComponent();
            try
            {
                FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
               new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            }
            catch (Exception)
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
        
          
            dati = cBusinessObjects.GetData(id, typeof(Excel_SospesiDiCassa));

            foreach (DataRow dtrow in dati.Rows)
            {
                      
				if (dtrow["PeriodoDiRiferimento"] != null)
				{
					txtPeriodoDiRiferimento.Text = dtrow["PeriodoDiRiferimento"].ToString();
				}
			}

            Binding b = new Binding();
            b.Source = dati;
			dtgSospesiDiCassa.SetBinding(ItemsControl.ItemsSourceProperty, b);
        }

		public int Save()
		{

            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["PeriodoDiRiferimento"] = txtPeriodoDiRiferimento.Text;
            }
            return cBusinessObjects.SaveData(id, dati, typeof(Excel_SospesiDiCassa));
        }

        private void AggiungiNodo(string Alias,  string Codice)
        {
          

			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}
            
                dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione,"", Alias, Codice);
   
        }

        private void DeleteTotal()
        {
           
            for (int i = dati.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dtrow = dati.Rows[i];
                if (dtrow["name"].ToString() == "Totale")
                    dtrow.Delete();
            }
            this.dati.AcceptChanges();

        }

        private void GenerateTotal()
        {
            if (dati.Rows.Count == 0)
                return;
            DeleteTotal();

			//double importoPagato = 0.0;
			double importoCompensato = 0.0;

            
            foreach (DataRow dtrow in dati.Rows)
            {
                double ic = 0;
                if(dtrow["importoCompensato"]!=System.DBNull.Value)
                     double.TryParse(dtrow["importoCompensato"].ToString(), out ic);
                importoCompensato += ic;


            }
           DataRow dd= dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione,"", "Totale");
           dd["importoCompensato"] = importoCompensato;
        }

        private void DataGrid_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            //   DataGrid grd = (DataGrid)sender;
            //    grd.CommitEdit(DataGridEditingUnit.Cell, true);
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
                    //        DataGrid grd = (DataGrid)sender;
                    //       grd.BeginEdit(e);
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
				

				if (dtgSospesiDiCassa.SelectedCells.Count >= 1)
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
                        if (k == dtgSospesiDiCassa.Items.IndexOf(dtgSospesiDiCassa.SelectedCells[0].Item))
                        {
                            dtrow.Delete();
                            break;
                        }

                        k++;
                    }
                    dati.AcceptChanges();
                    GenerateTotal();

					//_x.Save();

					
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

		private void txtPeriodoDiRiferimento_LostFocus(object sender, RoutedEventArgs e)
		{
			
            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["PeriodoDiRiferimento"] = txtPeriodoDiRiferimento.Text;
            }
         
		}
    }
}
