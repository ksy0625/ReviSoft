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
	public partial class uc_Excel_CapitaleSociale : UserControl
    {
        public int id;
        private DataTable datiCapitaleSociale = null;
        private DataTable datiTipiAzioni = null;
        private DataTable datiRipartizione = null;
        private DataTable datiRipartizioneAN = null;
      
        private string _ID = "-1";

		private bool _ReadOnly = false;

		public uc_Excel_CapitaleSociale()
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
            _ID = ID;

            datiCapitaleSociale = cBusinessObjects.GetData(id, typeof(Excel_CapitaleSociale_CapitaleSociale));
            datiTipiAzioni = cBusinessObjects.GetData(id, typeof(Excel_CapitaleSociale_TipiAzioni));
            datiRipartizione = cBusinessObjects.GetData(id, typeof(Excel_CapitaleSociale_Ripartizione));
            datiRipartizioneAN = cBusinessObjects.GetData(id, typeof(Excel_CapitaleSociale_RipartizioneAN));
 
            AggiungiNodo("ORDINARIE","CapitaleSociale", true);
			//AggiungiNodo("PRIVILEGIATE", _ID, "CapitaleSociale", true);


			AggiungiNodo("ORDINARIE / QUOTE SRL",  "TipiAzioni", true);
            //AggiungiNodo("PRIVILEGIATE", _ID, "TipiAzioni", true);


            //AggiungiNodo("", "Ripartizione", true);
            // AggiungiNodo( "",  "RipartizioneAN", true );

            Binding b = new Binding();
            b.Source = datiCapitaleSociale;
            dtgCapitaleSociale.SetBinding(ItemsControl.ItemsSourceProperty, b);

            Binding b1 = new Binding();
            b1.Source = datiTipiAzioni;
            dtgTipiAzioni.SetBinding(ItemsControl.ItemsSourceProperty, b1);

            Binding b2 = new Binding();
            b2.Source = datiRipartizione;
            dtgRipartizione.SetBinding(ItemsControl.ItemsSourceProperty, b2);

            Binding b3 = new Binding();
            b3.Source = datiRipartizioneAN;
            dtgRipartizioneAN.SetBinding( ItemsControl.ItemsSourceProperty, b3 );
        }

		public int Save()
		{
            cBusinessObjects.SaveData(id, datiCapitaleSociale, typeof(Excel_CapitaleSociale_CapitaleSociale));
            cBusinessObjects.SaveData(id, datiTipiAzioni, typeof(Excel_CapitaleSociale_TipiAzioni));
            cBusinessObjects.SaveData(id, datiRipartizione, typeof(Excel_CapitaleSociale_Ripartizione));
            return cBusinessObjects.SaveData(id, datiRipartizioneAN, typeof(Excel_CapitaleSociale_RipartizioneAN));
        }

        private void AggiungiNodo(string Alias, string tipo, bool NoReadOnlyCheck)
        {
			if (!NoReadOnlyCheck && _ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}
            bool trovata = false;
            if (Alias != "")
            {
                switch (tipo)
                {
                    case "CapitaleSociale":
                        foreach (DataRow dtrow in datiCapitaleSociale.Rows)
                        {
                            if (dtrow["name"].ToString() == Alias)
                                trovata = true;
                        }
                        break;
                    case "TipiAzioni":
                        foreach (DataRow dtrow in datiTipiAzioni.Rows)
                        {
                            if (dtrow["name"].ToString() == Alias)
                                trovata = true;
                        }
                        break;
                    case "Ripartizione":
                        foreach (DataRow dtrow in datiRipartizione.Rows)
                        {
                            if (dtrow["name"].ToString() == Alias)
                                trovata = true;
                        }
                        break;
                    case "RipartizioneAN":
                        foreach (DataRow dtrow in datiRipartizioneAN.Rows)
                        {
                            if (dtrow["name"].ToString() == Alias)
                                trovata = true;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (trovata)
                return;
            switch (tipo)
            {
                case "CapitaleSociale":
                    datiCapitaleSociale.Rows.Add(id,cBusinessObjects.idcliente,cBusinessObjects.idsessione,Alias);
                    break;
                case "TipiAzioni":
                    datiTipiAzioni.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Alias);
                    break;
                case "Ripartizione":
                    datiRipartizione.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Alias);
                    break;
                case "RipartizioneAN":
                    datiRipartizioneAN.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Alias);
                    break;
                default:
                    break;
            }

        }

        private void DeleteTotal(string tipo)
        {
            switch (tipo)
            {
                case "CapitaleSociale":
                    foreach (DataRow dtrow in this.datiCapitaleSociale.Rows)
                    {
                        if ("Totale" == dtrow["name"].ToString())
                        {
                            dtrow.Delete();
                            break;
                        }   
                    }
                    this.datiCapitaleSociale.AcceptChanges();
                    break;
                case "TipiAzioni":
                    foreach (DataRow dtrow in this.datiTipiAzioni.Rows)
                    {
                        if ("Totale" == dtrow["name"].ToString())
                        {
                            dtrow.Delete();
                            break;
                        }
                    }
                    this.datiTipiAzioni.AcceptChanges();
                    break;
                case "Ripartizione":
                    foreach (DataRow dtrow in this.datiRipartizione.Rows)
                    {
                        if ("Totale" == dtrow["name"].ToString())
                        {
                            dtrow.Delete();
                            break;
                        }
                    }
                    this.datiRipartizione.AcceptChanges();
                    break;
                case "RipartizioneAN":
                    foreach (DataRow dtrow in this.datiRipartizioneAN.Rows)
                    {
                        if ("Totale" == dtrow["name"].ToString())
                        {
                            dtrow.Delete();
                            break;
                        }
                    }
                    this.datiRipartizioneAN.AcceptChanges();
                    break;
                default:
                    break;
            }
          
        }

        private string ConvertNumber(string valore)
        {
            double dblValore = 0.0;

            double.TryParse(valore, out dblValore);

            if (dblValore == 0.0)
            {
                return "0";
            }
            else
            {
                //  return String.Format("{0:#,#.00000}", dblValore);
                return dblValore.ToString();
            }
        }

        private void GenerateTotal(string tipo)
        {
            switch (tipo)
            {
                case "CapitaleSociale":
                    if (this.datiCapitaleSociale.Rows.Count == 0)
                        return;
                    break;
                case "TipiAzioni":
                    if (this.datiTipiAzioni.Rows.Count == 0)
                        return;
                    break;
                case "Ripartizione":
                    if (this.datiRipartizione.Rows.Count == 0)
                        return;
                    break;
                case "RipartizioneAN":
                    if (this.datiRipartizioneAN.Rows.Count == 0)
                        return;
                    break;
                default:
                    break;

            }

           DeleteTotal(tipo);

            switch (tipo)
            {
                case "CapitaleSociale":
                    double deliberato = 0.0;
                    double sottoscritto = 0.0;
                    double versato = 0.0;
                    foreach (DataRow dtrow in this.datiCapitaleSociale.Rows)
                    {
                        if(dtrow["deliberato"].ToString()!="")
                          deliberato += Convert.ToDouble(dtrow["deliberato"].ToString());
                        if (dtrow["sottoscritto"].ToString() != "")
                            sottoscritto += Convert.ToDouble(dtrow["sottoscritto"].ToString());
                        if (dtrow["versato"].ToString() != "")
                            versato += Convert.ToDouble(dtrow["versato"].ToString());
                    }
                    datiCapitaleSociale.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, "Totale", deliberato, sottoscritto, versato);
                    break;
                case "TipiAzioni":
                    double valnom = 0.0;
                    double numero = 0.0;
                    double totale = 0.0;
                    foreach (DataRow dtrow in this.datiTipiAzioni.Rows)
                    {
                        double valnomtmp = 0;
                        if (dtrow["valnom"].ToString() != "")
                             valnomtmp = Convert.ToDouble(dtrow["valnom"].ToString());
                        double numerotmp = 0;
                        if (dtrow["numero"].ToString() != "")
                             numerotmp = Convert.ToDouble(dtrow["numero"].ToString());
                        double totaltmp = valnomtmp * numerotmp;
                        dtrow["totale"] = ConvertNumber(totaltmp.ToString());
                        valnom += valnomtmp;
                        numero += numerotmp;
                        totale += totaltmp;
                    }
                    datiTipiAzioni.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, "Totale", valnom, numero, totale);
                    break;

                case "Ripartizione":
                    double valnomr = 0.0;
                    double numeror = 0.0;
                    double totaler = 0.0;

                    string tiporipartizione = "Div";
                    foreach (DataRow dtrow in this.datiRipartizione.Rows)
                    {
                        double valnomtmp = 0;
                        if (dtrow["valnom"].ToString() != "")
                             valnomtmp = Convert.ToDouble(dtrow["valnom"].ToString());
                        double numerotmp = 0;
                        if (dtrow["numero"].ToString() != "")
                             numerotmp = Convert.ToDouble(dtrow["numero"].ToString());
                        double totaltmp = 0;
                        if (dtrow["totale"].ToString() != "")
                             totaltmp = (valnomtmp == 0) ? Convert.ToDouble(dtrow["totale"].ToString()) : valnomtmp * numerotmp;
                        else
                            totaltmp = (valnomtmp == 0) ? 0 : valnomtmp * numerotmp;
                        dtrow["totale"] = ConvertNumber(totaltmp.ToString());
                        valnomr += valnomtmp;
                        numeror += numerotmp;
                        totaler += totaltmp;
                    }

                    //foreach (XmlNode item in dtgRipartizione.ItemsSource)
                    foreach (DataRow dtrow in this.datiRipartizione.Rows)
                    {
                        if (totaler == 0.0)
                        {
                            dtrow["percentuale"] = (0.0).ToString();
                        }
                        else
                        {
                            dtrow["percentuale"] = ConvertNumber((Convert.ToDouble(dtrow["totale"].ToString()) / totaler).ToString());
                        }
                    }
                    datiRipartizione.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, "Totale", valnomr, numeror, totaler,1, tiporipartizione);
                    break;
                case "RipartizioneAN":
                    double valnomrAN = 0.0;
                    double numerorAN = 0.0;
                    double totalerAN = 0.0;

                    string tiporipartizioneAN = "Div";
                    foreach (DataRow dtrow in this.datiRipartizioneAN.Rows)
                    {
                        double totaltmp = 0;
                        if (dtrow["totale"].ToString() != "")
                             totaltmp = Convert.ToDouble(dtrow["totale"].ToString());
                        double numerotmp = 0;
                        if (dtrow["numero"].ToString() != "")
                            numerotmp = Convert.ToDouble(dtrow["numero"].ToString());

                        double valnomtmp = 0;
                        if(numerotmp!=0)
                        {
                        if (dtrow["valnom"].ToString() != "")
                             valnomtmp = (totaltmp == 0) ? Convert.ToDouble(dtrow["totale"].ToString()) : totaltmp / numerotmp;
                        else
                            valnomtmp = (valnomtmp == 0) ? 0 : totaltmp / numerotmp;
                        }
                        
                        dtrow["valnom"] = ConvertNumber(valnomtmp.ToString());
                        valnomrAN += valnomtmp;
                        numerorAN += numerotmp;
                        totalerAN += totaltmp;
                    }
                    foreach (DataRow dtrow in this.datiRipartizioneAN.Rows)
                    {
                        if (totalerAN == 0.0)
                        {
                            dtrow["percentuale"] = 0;
                        }
                        else
                        {
                           
                            if (dtrow["totale"].ToString() != "")
                                dtrow["percentuale"] = ConvertNumber((Convert.ToDouble(dtrow["totale"].ToString()) / totalerAN).ToString());
                            else
                               dtrow["percentuale"] =0;
                        }
                    }
                    datiRipartizioneAN.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, "Totale", valnomrAN, numerorAN, totalerAN, 1, tiporipartizioneAN);

                    break;
                default:
                    break;
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

        private void dtgCapitaleSociale_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateTotal("CapitaleSociale");
        }

        private void dtgCapitaleSociale_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
          
            double deliberato = 0.0;
            double sottoscritto = 0.0;
            double versato = 0.0;
            foreach (DataRow dtrow in this.datiCapitaleSociale.Rows)
            {
                if (dtrow["name"].ToString() == "Totale")
                    continue;

                    if (dtrow["deliberato"].ToString() != "")
                    deliberato += Convert.ToDouble(dtrow["deliberato"].ToString());
                if (dtrow["sottoscritto"].ToString() != "")
                    sottoscritto += Convert.ToDouble(dtrow["sottoscritto"].ToString());
                if (dtrow["versato"].ToString() != "")
                    versato += Convert.ToDouble(dtrow["versato"].ToString());
            }
            foreach (DataRow dtrow in this.datiCapitaleSociale.Rows)
            {
                if (dtrow["name"].ToString()=="Totale")
                {
                    dtrow["deliberato"] = deliberato;
                    dtrow["sottoscritto"] = sottoscritto;
                    dtrow["versato"] = versato;
                    break;
                }

            }

        }

        private void dtgTipiAzioni_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateTotal("TipiAzioni");
        }

        private void dtgTipiAzioni_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            double valnom = 0.0;
            double numero = 0.0;
            double totale = 0.0;
            foreach (DataRow dtrow in this.datiTipiAzioni.Rows)
            {
                if (dtrow["name"].ToString() == "Totale")
                    continue;

                double valnomtmp = 0;
                if (dtrow["valnom"].ToString() != "")
                    valnomtmp = Convert.ToDouble(dtrow["valnom"].ToString());
                double numerotmp = 0;
                if (dtrow["numero"].ToString() != "")
                    numerotmp = Convert.ToDouble(dtrow["numero"].ToString());
                double totaltmp = valnomtmp * numerotmp;
                dtrow["totale"] = ConvertNumber(totaltmp.ToString());
                valnom += valnomtmp;
                numero += numerotmp;
                totale += totaltmp;
            }
           GenerateTotal("TipiAzioni");
        }

        private void dtgRipartizione_Loaded(object sender, RoutedEventArgs e)
        {
              GenerateTotal("Ripartizione");
        }

        private void dtgRipartizione_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            double valnomr = 0.0;
            double numeror = 0.0;
            double totaler = 0.0;

            
            foreach (DataRow dtrow in this.datiRipartizione.Rows)
            {
                if (dtrow["name"].ToString() == "Totale")
                    continue;

                double valnomtmp = 0;
                if (dtrow["valnom"].ToString() != "")
                    valnomtmp = Convert.ToDouble(dtrow["valnom"].ToString());
                double numerotmp = 0;
                if (dtrow["numero"].ToString() != "")
                    numerotmp = Convert.ToDouble(dtrow["numero"].ToString());
                double totaltmp = 0;
                if (dtrow["totale"].ToString() != "")
                    totaltmp = (valnomtmp == 0) ? Convert.ToDouble(dtrow["totale"].ToString()) : valnomtmp * numerotmp;
                else
                    totaltmp = (valnomtmp == 0) ? 0 : valnomtmp * numerotmp;
                dtrow["totale"] = ConvertNumber(totaltmp.ToString());
                valnomr += valnomtmp;
                numeror += numerotmp;
                totaler += totaltmp;
            }

            //foreach (XmlNode item in dtgRipartizione.ItemsSource)
            foreach (DataRow dtrow in this.datiRipartizione.Rows)
            {
                if (totaler == 0.0)
                {
                    dtrow["percentuale"] = (0.0).ToString();
                }
                else
                {
                    dtrow["percentuale"] = ConvertNumber((Convert.ToDouble(dtrow["totale"].ToString()) / totaler).ToString());
                }
            }
            GenerateTotal("Ripartizione");

        }

        private void dtgRipartizioneAN_Loaded( object sender, RoutedEventArgs e )
        {
            GenerateTotal( "RipartizioneAN" );
        }

        private void dtgRipartizioneAN_CellEditEnding( object sender, DataGridCellEditEndingEventArgs e )
        {
            double valnomrAN = 0.0;
            double numerorAN = 0.0;
            double totalerAN = 0.0;

          
            foreach (DataRow dtrow in this.datiRipartizioneAN.Rows)
            {
                if (dtrow["name"].ToString() == "Totale")
                    continue;

                double valnomtmp = 0;
           
                double numerotmp = 0;
                if (dtrow["numero"].ToString() != "")
                    numerotmp = Convert.ToDouble(dtrow["numero"].ToString());
                double totaltmp = 0;
                if (dtrow["totale"].ToString() != "")
                    totaltmp = (valnomtmp == 0) ? Convert.ToDouble(dtrow["totale"].ToString()) : valnomtmp * numerotmp;
                else
                    totaltmp = (valnomtmp == 0) ? 0 : valnomtmp * numerotmp;

                if(numerotmp!=0)
                   dtrow["valnom"] = ConvertNumber((totaltmp/numerotmp).ToString());
                dtrow["totale"] = ConvertNumber(totaltmp.ToString());
                valnomrAN += valnomtmp;
                numerorAN += numerotmp;
                totalerAN += totaltmp;
            }
            foreach (DataRow dtrow in this.datiRipartizioneAN.Rows)
            {
                if (totalerAN == 0.0)
                {
                    dtrow["percentuale"] = 0;
                }
                else
                {

                    if (dtrow["totale"].ToString() != "")
                        dtrow["percentuale"] = ConvertNumber((Convert.ToDouble(dtrow["totale"].ToString()) / totalerAN).ToString());
                    else
                        dtrow["percentuale"] = 0;
                }
            }
            GenerateTotal( "RipartizioneAN" );
           
        }

        private void AddRowCapitaleSociale(object sender, RoutedEventArgs e)
        {
			AggiungiNodo("", "CapitaleSociale", false);

            GenerateTotal("CapitaleSociale");
        }

        private void DeleteRowCapitaleSociale(object sender, RoutedEventArgs e)
        {
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}

			if (MessageBox.Show("Si è sicuri di procedere con l'eliminazione?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				

				if (dtgCapitaleSociale.SelectedCells.Count < 1)
				{
			
					MessageBox.Show("Selezionare una riga");
					return;
				}

				try
				{
                    int k = 0;
                    foreach (DataRow dtrow in this.datiCapitaleSociale.Rows)
                    {
                        if (k == dtgCapitaleSociale.Items.IndexOf(dtgCapitaleSociale.SelectedCells[0].Item))
                        {
                            dtrow.Delete();
                            break;
                        }

                        k++;

                    }
                    this.datiCapitaleSociale.AcceptChanges();
					GenerateTotal("CapitaleSociale");
					return;
				}
				catch (Exception ex)
				{
                    cBusinessObjects.logger.Error(ex, "wWorkArea_Excel_CapitaleSociale.DeleteRowCapitaleSociale exception");
                    string log = ex.Message;

					MessageBox.Show("Solo le righe inserite dall'utente possono essere cancellate");
				}
			}
        }

        private void AddRowTipiAzioni(object sender, RoutedEventArgs e)
        {
			AggiungiNodo("",  "TipiAzioni", false);

            GenerateTotal("TipiAzioni");
        }

        private void DeleteRowTipiAzioni(object sender, RoutedEventArgs e)
        {
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}

			if (MessageBox.Show("Si è sicuri di procedere con l'eliminazione?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				

				if (dtgTipiAzioni.SelectedCells.Count < 1)
				{
				
					MessageBox.Show("Selezionare una riga");
					return;
				}

				try
				{
                    int k = 0;
                    foreach (DataRow dtrow in this.datiTipiAzioni.Rows)
                    {
                        if (k == dtgTipiAzioni.Items.IndexOf(dtgTipiAzioni.SelectedCells[0].Item))
                        {
                            dtrow.Delete();
                            break;
                        }
                        k++;
                    }
                    this.datiTipiAzioni.AcceptChanges();
                    GenerateTotal("TipiAzioni");
                    return;
				}
				catch (Exception ex)
				{
                    cBusinessObjects.logger.Error(ex, "wWorkArea_Excel_CapitaleSociale.DeleteRowTipiAzioni exception");
                    string log = ex.Message;
					MessageBox.Show("Solo le righe inserite dall'utente possono essere cancellate");
				}
			}
        }

        private void AddRowRipartizione(object sender, RoutedEventArgs e)
        {
            AggiungiNodo("", "Ripartizione", false);

            GenerateTotal("Ripartizione");
        }

        private void AddRowRipartizioneAN( object sender, RoutedEventArgs e )
        {
            AggiungiNodo( "",  "RipartizioneAN", false );

            GenerateTotal( "RipartizioneAN" );
        }

        private void DeleteRowRipartizione(object sender, RoutedEventArgs e)
        {
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}

			if (MessageBox.Show("Si è sicuri di procedere con l'eliminazione?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{

				if (dtgRipartizione.SelectedCells.Count < 1)
				{
				
					MessageBox.Show("Selezionare una riga");
					return;
				}

				try
				{
                    int k = 0;
                    foreach (DataRow dtrow in this.datiRipartizione.Rows)
                    {
                        if (k == dtgRipartizione.Items.IndexOf(dtgRipartizione.SelectedCells[0].Item))
                        {
                            dtrow.Delete();
                            break;
                        }
                        k++;
                    }
                    this.datiRipartizione.AcceptChanges();
                    GenerateTotal("Ripartizione");
                    return;					
				}
				catch (Exception ex)
				{
                    cBusinessObjects.logger.Error(ex, "wWorkArea_Excel_CapitaleSociale.DeleteRowRipartizione exception");
                    string log = ex.Message;

					MessageBox.Show("Solo le righe inserite dall'utente possono essere cancellate");
				}
			}
        }

        private void DeleteRowRipartizioneAN( object sender, RoutedEventArgs e )
        {
            if ( _ReadOnly )
            {
                MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione" );
                return;
            }

            if ( MessageBox.Show( "Si è sicuri di procedere con l'eliminazione?", "Attenzione", MessageBoxButton.YesNo ) == MessageBoxResult.Yes )
            {
                

                if ( dtgRipartizioneAN.SelectedCells.Count < 1 )
                {
               
                    MessageBox.Show( "Selezionare una riga" );
                    return;
                }

                try
                {
                    int k = 0;
                    foreach (DataRow dtrow in this.datiRipartizioneAN.Rows)
                    {
                        if (k == dtgRipartizione.Items.IndexOf(dtgRipartizioneAN.SelectedCells[0].Item))
                        {
                            dtrow.Delete();
                            break;
                        }
                        k++;
                    }
                    this.datiRipartizioneAN.AcceptChanges();
                    GenerateTotal("RipartizioneAN");
                    return;
                }
                catch ( Exception ex )
                {
                    cBusinessObjects.logger.Error(ex, "wWorkArea_Excel_CapitaleSociale.DeleteRowRipartizioneAN exception");
                    string log = ex.Message;

                    MessageBox.Show( "Solo le righe inserite dall'utente possono essere cancellate" );
                }
            }
        }

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			try
			{
				double tmp = e.NewSize.Width - 30.0;

				stpMain.Width = tmp;
			}
			catch (Exception ex)
			{
                cBusinessObjects.logger.Error(ex, "wWorkArea_Excel_CapitaleSociale.UserControl_SizeChanged exception");
                string log = ex.Message;
			}
		}

    }
}
