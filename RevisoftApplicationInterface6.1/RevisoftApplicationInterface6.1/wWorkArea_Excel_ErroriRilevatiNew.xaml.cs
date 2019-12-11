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
	public partial class uc_Excel_ErroriRilevatiNew : UserControl
    {
        public int id;
        private DataTable dati_ErroriRilevati = null;
        private DataTable dati_ErroriRilevatiNN = null;
        private DataTable dati_ErroriRilevatiMR = null;
        private DataTable dati_ErroriRilevati_Note = null;
        
        private string _ID = "";
        private int indexselcell_ErroriRilevati =-1;
        private int indexselcell_ErroriRilevatiNN =-1; 
         private int indexselcell_ErroriRilevatiMR =-1;
       

		private bool _ReadOnly = false;

        public WindowWorkArea Owner;

        public uc_Excel_ErroriRilevatiNew()
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

            _ID = ID;

            dati_ErroriRilevati = cBusinessObjects.GetData(id, typeof(Excel_ErroriRilevati));
            dati_ErroriRilevati.Columns.Add("bold", typeof(String));
           
            dati_ErroriRilevatiNN = cBusinessObjects.GetData(id, typeof(Excel_ErroriRilevatiNN));
            dati_ErroriRilevatiMR = cBusinessObjects.GetData(id, typeof(Excel_ErroriRilevatiMR));
            dati_ErroriRilevati_Note = cBusinessObjects.GetData(id, typeof(Excel_ErroriRilevati_Note));


            Binding b = new Binding();
            b.Source = dati_ErroriRilevati;
            dtgErroriRilevati.SetBinding(ItemsControl.ItemsSourceProperty, b);

            Binding bnote = new Binding();
            bnote.Source = dati_ErroriRilevati_Note;
            dtgNote.SetBinding( ItemsControl.ItemsSourceProperty, bnote);

            Binding b2 = new Binding();
            b2.Source = dati_ErroriRilevatiNN;
            dtgErroriRilevatiNN.SetBinding( ItemsControl.ItemsSourceProperty, b2 );

            Binding b3 = new Binding();
            b3.Source = dati_ErroriRilevatiMR;
            dtgErroriRilevatiMR.SetBinding( ItemsControl.ItemsSourceProperty, b3 );
        }

		public int Save()
		{
            cBusinessObjects.SaveData(id, dati_ErroriRilevatiNN, typeof(Excel_ErroriRilevatiNN));
            cBusinessObjects.SaveData(id, dati_ErroriRilevatiMR, typeof(Excel_ErroriRilevatiMR));
            cBusinessObjects.SaveData(id, dati_ErroriRilevati_Note, typeof(Excel_ErroriRilevati_Note));
            try
            {
                dati_ErroriRilevati.Columns.Remove("bold");
            }
            catch(Exception)
            {

            }
            
            foreach(DataRow ddN in dati_ErroriRilevati_Note.Rows)
            {
                foreach (DataRow dd in dati_ErroriRilevati.Rows)
                {
                    if(ddN["rif"].ToString()== ddN["rif"].ToString())
                    {
                        if (dd["name"].ToString() == "Totale")
                            continue;
                        dd["name"] = ddN["name"].ToString();
                    }
                }
            }

            return cBusinessObjects.SaveData(id, dati_ErroriRilevati, typeof(Excel_ErroriRilevati));
        }

        private void AggiungiNodo(string Alias, string ID)
        {
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}
         
            bool trovata = false;
            if (Alias != "")
            {

                foreach (DataRow dtrow in dati_ErroriRilevati.Rows)
                {
                    if (dtrow["name"].ToString() == Alias)
                        trovata = true;
                }
            }

            if (trovata)
                return;
            int numeroattuale = 0;
            foreach (DataRow dtrow in dati_ErroriRilevati.Rows)
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

            DataRow tmp=dati_ErroriRilevati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            tmp["rif"] = numeroattuale.ToString();
            tmp["name"] = Alias;
            tmp["corretto"] = "False";
         

            DataRow tmp2 = dati_ErroriRilevati_Note.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            tmp2["rif"] = numeroattuale.ToString();
           
        }


        private void AggiungiNodoNN( string Alias, string ID )
        {
            if ( _ReadOnly )
            {
                MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione" );
                return;
            }
            bool trovata = false;
            if (Alias != "")
            {

                foreach (DataRow dtrow in dati_ErroriRilevatiNN.Rows)
                {
                    if (dtrow["name"].ToString() == Alias)
                        trovata = true;
                }
            }

            if (trovata)
                return;
            int numeroattuale = 0;
            foreach (DataRow dtrow in dati_ErroriRilevatiNN.Rows)
            {
                if (dtrow["numero"] != null)
                {
                    int valorehere = 0;

                    int.TryParse(dtrow["numero"].ToString(), out valorehere);

                    if (valorehere > numeroattuale)
                    {
                        numeroattuale = valorehere;
                    }
                }
            }
         

            numeroattuale = numeroattuale + 1;

          
         
            DataRow tmp=dati_ErroriRilevatiNN.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            tmp["name"] = Alias;
            tmp["corretto"] = false;
            tmp["numero"] = numeroattuale.ToString();

        }

        private void AggiungiNodoMR( string Alias, string ID )
        {
            if ( _ReadOnly )
            {
                MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione" );
                return;
            }
            bool trovata = false;
            if (Alias != "")
            {

                foreach (DataRow dtrow in dati_ErroriRilevatiMR.Rows)
                {
                    if (dtrow["name"].ToString() == Alias)
                        trovata = true;
                }
            }

            if (trovata)
                return;
             int numeroattuale = 0;
            foreach (DataRow dtrow in dati_ErroriRilevatiMR.Rows)
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

          
            DataRow dtrowtmp=dati_ErroriRilevatiMR.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            dtrowtmp["name"] = Alias;
            dtrowtmp["corretto"] = false;
            dtrowtmp["rif"] = numeroattuale.ToString();


        }

        private void DeleteTotal()
        {

            for (int i = dati_ErroriRilevati.Rows.Count - 1; i >= 0; i--)
                {
                  DataRow dtrow = dati_ErroriRilevati.Rows[i];
                  if ("Totale" == dtrow["name"].ToString())
                    dtrow.Delete();
                }
                dati_ErroriRilevati.AcceptChanges();
        }

       

        private void DeleteTotalMR()
        {
 
               for (int i = dati_ErroriRilevatiMR.Rows.Count - 1; i >= 0; i--)
                {
                  DataRow dtrow = dati_ErroriRilevatiMR.Rows[i];
                  if ("Totale" == dtrow["name"].ToString())
                    dtrow.Delete();
                }
                dati_ErroriRilevatiMR.AcceptChanges();

        }

        private void GenerateTotal()
        {
         
            if (dtgErroriRilevati.SelectedCells.Count>0)
                indexselcell_ErroriRilevati = dtgErroriRilevati.Items.IndexOf(dtgErroriRilevati.SelectedCells[0].Item);

            DeleteTotal();

            double importo = 0.0;
            double importoap = 0.0;
            double impattofiscale = 0.0;
            double impattofiscalePN = 0.0;            

            double suPNattuale  = 0.0;
            double suutileattuale = 0.0;
            foreach (DataRow dtrow in this.dati_ErroriRilevati.Rows)
             {
                if(dtrow["importo"].ToString()!="")
                  importo += Convert.ToDouble(dtrow["importo"].ToString());
                if (dtrow["importoAP"].ToString() != "")
                    importoap += Convert.ToDouble(dtrow["importoAP"].ToString());

                if (dtrow["corretto"].ToString() == "False")// && item.Attributes["importo"].Value != "0" && item.Attributes["importo"].Value != "0,00" && item.Attributes["importo"].Value != "")
                {
                    wEffettoFiscaleCalcolo wef = new wEffettoFiscaleCalcolo(dtrow, "");
                    wef.getTotal();
                    if ((dtrow["impattofiscale"].ToString() != "") && (dtrow["impattofiscalePN"].ToString() != ""))
                        impattofiscale += Convert.ToDouble((( dtrow["impattofiscale"].ToString() != "") ? dtrow["impattofiscale"].ToString() : "0.0"));

                    wef = new wEffettoFiscaleCalcolo(dtrow, "PN");
                    wef.getTotal();
                    if ((dtrow["impattofiscalePN"].ToString() != "") && (dtrow["impattofiscalePN"].ToString() != ""))
                        impattofiscalePN += Convert.ToDouble((( dtrow["impattofiscalePN"].ToString() != "") ? dtrow["impattofiscalePN"].ToString() : "0.0"));

                    if (dtrow["importo"].ToString() != "")
                        dtrow["suPNattuale"] = ( Convert.ToDouble(dtrow["importo"].ToString() ) ).ToString();
                    //item.Attributes["suutileattuale"].Value = ( Convert.ToDouble( item.Attributes["importo"].Value ) - Convert.ToDouble( item.Attributes["importoAP"].Value ) > 0 ) ? ( Convert.ToDouble( item.Attributes["importo"].Value ) - Convert.ToDouble( item.Attributes["importoAP"].Value ) ).ToString() : "0";
                    if ((dtrow["importo"].ToString() != "")&& (dtrow["importoAP"].ToString() != ""))
                        dtrow["suutileattuale"] = ( Convert.ToDouble(dtrow["importo"].ToString()) - Convert.ToDouble(dtrow["importoAP"].ToString() ) ).ToString();
                }
                else
                {
                    //Eliminati valori su non corretto per richiesta borelli
                    //wEffettoFiscaleCalcolo wef = new wEffettoFiscaleCalcolo(item, "");
                    //wef.setEmpty();

                    //Eliminati valori su non corretto per richiesta borelli
                    //wef = new wEffettoFiscaleCalcolo(item, "PN");
                    //wef.setEmpty();

                    dtrow["suPNattuale"] = "0";
                    dtrow["suutileattuale"] = "0";
                    dtrow["impattofiscale"] = "0";
                    dtrow["impattofiscalePN"] = "0";
                }
                if (dtrow["suPNattuale"].ToString() != "")
                    suPNattuale += Convert.ToDouble(dtrow["suPNattuale"].ToString());
                if (dtrow["suutileattuale"].ToString() != "")
                    suutileattuale += Convert.ToDouble(dtrow["suutileattuale"].ToString());
            }
            DataRow tmpd = dati_ErroriRilevati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            tmpd["name"] = "Totale";
            tmpd["importo"] = importo;
            tmpd["importoAP"] = importoap;
            tmpd["impattofiscale"] = impattofiscale;
            tmpd["impattofiscalePN"] = impattofiscalePN;
            tmpd["suutileattuale"] = suutileattuale;
            tmpd["suPNattuale"] = suPNattuale;
            tmpd["bold"] = "True";

          
          

        }

        private string ConvertNumberNoDecimal( string valore )
        {
            double dblValore = 0.0;

            double.TryParse( valore, out dblValore );

            if ( dblValore == 0.0 )
            {
                return "0";
            }
            else
            {
                return String.Format( "{0:#,#}", dblValore );
            }
        }
     

        private void GenerateTotalMR()
        {
          if (dtgErroriRilevatiMR.SelectedCells.Count>0)
                indexselcell_ErroriRilevatiMR = dtgErroriRilevatiMR.Items.IndexOf(dtgErroriRilevatiMR.SelectedCells[0].Item);

            DeleteTotalMR();

            double importo = 0.0;
            foreach (DataRow dtrow in this.dati_ErroriRilevatiMR.Rows)
            {
                if(dtrow["importo"].ToString()!="")
                  importo += Convert.ToDouble(dtrow["importo"].ToString() );
            }
            DataRow tmp=dati_ErroriRilevatiMR.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            tmp["name"] = "Totale";
            tmp["importo"] = importo;
            tmp["corretto"] = "False";

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
                 DataGrid dg = (DataGrid)sender;
                 e.Handled = true;
                 dg.BeginEdit(e);
                
              
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

            if (e.Column.Header.ToString().Contains("Fiscale"))
            {
                if (e.Column.Header.ToString().Contains("P.N."))
                {
                    wEffettoFiscaleCalcolo wef = new wEffettoFiscaleCalcolo((DataRow)((DataRowView)e.Row.DataContext).Row, "PN");
                    wef.ShowDialog();

                    GenerateTotal();
                    return;
                }
                else
                {
                    wEffettoFiscaleCalcolo wef = new wEffettoFiscaleCalcolo((DataRow)((DataRowView)e.Row.DataContext).Row, "");
                    wef.ShowDialog();

                    GenerateTotal();
                    return;
                }
            }
        }

        private void dtgErroriRilevati_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateTotal();
        }

        private void dtgErroriRilevatiNN_Loaded( object sender, RoutedEventArgs e )
        {
           
        }

        private void dtgErroriRilevatiMR_Loaded( object sender, RoutedEventArgs e )
        {
            GenerateTotalMR();
        }

        private void dtgErroriRilevati_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
           GenerateTotal();
        }

        private void dtgErroriRilevatiNN_CellEditEnding( object sender, DataGridCellEditEndingEventArgs e )
        {
          
        }

        private void dtgErroriRilevatiMR_CellEditEnding( object sender, DataGridCellEditEndingEventArgs e )
        {
            GenerateTotalMR();
        }

        private void AddRowErroriRilevati(object sender, RoutedEventArgs e)
        {
            AggiungiNodo("", _ID);

            GenerateTotal();
        }

        private void AddRowErroriRilevatiNN( object sender, RoutedEventArgs e )
        {
            AggiungiNodoNN( "", _ID );

       
        }

        private void AddRowErroriRilevatiMR( object sender, RoutedEventArgs e )
        {
            AggiungiNodoMR( "", _ID );

            GenerateTotalMR();
        }

        private void RowErroriRilevatiPeriodoPrecedente(object sender, RoutedEventArgs e)
        {
            if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}

            if ( Owner.btn_NavBar_SessioneNext.IsEnabled == false )
            {
                MessageBox.Show( "Non esiste nessuna sessione precedente. impossibile importare dati.", "Attenzione" );
                return;
            }

            if ( MessageBox.Show( "Si è sicuri di procedere con l'importazione dei dati dall'anno precedente?", "Attenzione", MessageBoxButton.YesNo ) == MessageBoxResult.Yes )
            {
                int ids = cBusinessObjects.idsessione;
                cBusinessObjects.idsessione = int.Parse(Owner.Sessioni[Owner.SessioneNow + 1].ToString());
                dati_ErroriRilevati = cBusinessObjects.GetData(id, typeof(Excel_ErroriRilevati));
                cBusinessObjects.idsessione = ids;
       
            }
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
			

				if (indexselcell_ErroriRilevati >=0)
				{
					
				}
				else
				{
					MessageBox.Show("Selezionare una riga");
					return;
				}

				try
				{

                    //int k = 0;
                    
                   for (int i = dati_ErroriRilevati.Rows.Count - 1; i >= 0; i--)
                    {
                      DataRow dtrow = dati_ErroriRilevati.Rows[i];
                       if (i ==indexselcell_ErroriRilevati)
                        dtrow.Delete();
                    }
                    dati_ErroriRilevati.AcceptChanges();
                    try
                    {
                       
                        for (int i = dati_ErroriRilevati_Note.Rows.Count - 1; i >= 0; i--)
                        {
                          DataRow dtrow = dati_ErroriRilevati_Note.Rows[i];
                           if (i ==indexselcell_ErroriRilevati)
                            dtrow.Delete();
                        }
                        dati_ErroriRilevati_Note.AcceptChanges();
                    }
                    catch(Exception a)
                    {

                    }
                   

                    GenerateTotal();

					return;
				}
				catch (Exception ex)
				{
                    cBusinessObjects.logger.Error(ex, "wWorkArea_Excel_ErroriRilevatiNew.DeleteRowErroriRilevati exception");
                    string log = ex.Message;

					MessageBox.Show("Solo le righe inserite dall'utente possono essere cancellate");
				}
			}
        }

        private void DeleteRowErroriRilevatiNN( object sender, RoutedEventArgs e )
        {
            if ( _ReadOnly )
            {
                MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione" );
                return;
            }

            if ( MessageBox.Show( "Si è sicuri di procedere con l'eliminazione?", "Attenzione", MessageBoxButton.YesNo ) == MessageBoxResult.Yes )
            {
           

                if ( dtgErroriRilevatiNN.SelectedCells.Count >0 )
                {
                  
                }
                else
                {
                    MessageBox.Show( "Selezionare una riga" );
                    return;
                }

                try
                {
                
                  
                    for (int i = dati_ErroriRilevatiNN.Rows.Count - 1; i >= 0; i--)
                    {
                      DataRow dtrow = dati_ErroriRilevatiNN.Rows[i];
                       if (i == dtgErroriRilevatiNN.Items.IndexOf(dtgErroriRilevatiNN.SelectedCells[0].Item))
                        dtrow.Delete();
                    }
                    dati_ErroriRilevatiNN.AcceptChanges();
                    
                  
                    return;
                }
                catch ( Exception ex )
                {
              
                }
            }
        }

        private void DeleteRowErroriRilevatiMR( object sender, RoutedEventArgs e )
        {
            if ( _ReadOnly )
            {
                MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione" );
                return;
            }

            if ( MessageBox.Show( "Si è sicuri di procedere con l'eliminazione?", "Attenzione", MessageBoxButton.YesNo ) == MessageBoxResult.Yes )
            {
               

                if (indexselcell_ErroriRilevatiMR >=0)
                {
                   
                }
                else
                {
                    MessageBox.Show( "Selezionare una riga" );
                    return;
                }

                try
                {
                  
                    for (int i = dati_ErroriRilevatiMR.Rows.Count - 1; i >= 0; i--)
                    {
                      DataRow dtrow = dati_ErroriRilevatiMR.Rows[i];
                       if (i == indexselcell_ErroriRilevatiMR)
                        dtrow.Delete();
                    }
                    dati_ErroriRilevatiMR.AcceptChanges();
                    GenerateTotalMR();

                    return;
                }
                catch ( Exception ex )
                {
                    cBusinessObjects.logger.Error(ex, "wWorkArea_Excel_ErroriRilevatiNew.DeleteRowErroriRilevatiMR exception");
                    string log = ex.Message;

                    MessageBox.Show( "Solo le righe inserite dall'utente possono essere cancellate" );
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

        private void dtgErroriRilevatiNN_KeyUp( object sender, KeyEventArgs e )
        {
            
        }

        private void dtgErroriRilevatiMR_KeyUp( object sender, KeyEventArgs e )
        {
            if ( e.Key == Key.Enter )
            {
                GenerateTotalMR();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

         
        if((((DataRow)((DataRowView)(((CheckBox)(sender)).DataContext)).Row)["corretto"].ToString()=="True" 
        && ((CheckBox)(sender)).IsChecked == false)||
        (((DataRow)((DataRowView)(((CheckBox)(sender)).DataContext)).Row)["corretto"].ToString()=="False" 
        && ((CheckBox)(sender)).IsChecked == true))
        {
          ((DataRow)((DataRowView)(((CheckBox)(sender)).DataContext)).Row)["corretto"] = (((CheckBox)(sender)).IsChecked == true) ? "True" : "False";
          GenerateTotal();
        }
           


        }
    }
}
