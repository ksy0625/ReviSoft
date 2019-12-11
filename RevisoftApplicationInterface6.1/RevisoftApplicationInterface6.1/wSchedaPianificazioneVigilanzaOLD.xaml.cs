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
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using System.Collections;
using System.ComponentModel;

namespace RevisoftApplication
{

    public partial class wSchedaPianificazioniVigilanzaOLD : Window
    {
        private App.TipoAttivitaScheda _tipologiaAttivita;
		private App.TipoAttivitaScheda oldTipologiaAttivita = App.TipoAttivitaScheda.View;

        public int IDPianificazioniVigilanza;
        private bool _InCaricamento;
        private bool _DatiCambiati;
        public bool RegistrazioneEffettuata;

		private bool firsttime = true;

		private bool _cmbInCaricamento = false;
        private int OldSelectedCmbClienti = -1;

        public string IDClienteImport = "-1";

        Hashtable htClienti = new Hashtable();
        Hashtable htDate = new Hashtable();

        public bool noopenaftercreate = false;

		public App.TipoAttivitaScheda TipologiaAttivita
		{
			get { return _tipologiaAttivita; }
			set 
			{
				if (!firsttime && _tipologiaAttivita == value)
				{
					return;
				}

				firsttime = false;

				_tipologiaAttivita = value;

				MasterFile mf = MasterFile.Create();

				int index = 0;

				int selectedIndex = -1;
				if (cmbClienti.Items.Count != 0)
				{
					_cmbInCaricamento = true;
					selectedIndex = cmbClienti.SelectedIndex;
					cmbClienti.Items.Clear();
					htClienti.Clear();
				}

				List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>();

				foreach (Hashtable item in mf.GetAnagrafiche())
				{
                    if ( IDClienteImport == "-1" || IDClienteImport == item["ID"].ToString() )
                    {
                        if ( _tipologiaAttivita != App.TipoAttivitaScheda.New )
                        {
                            if ( mf.GetPianificazioniVigilanze( item["ID"].ToString() ).Count == 0 )
                            {
                                continue;
                            }
                        }

                        string cliente = item["RagioneSociale"].ToString();
                        if ( IDClienteImport == "-1" )
                        {
                            switch ( ( (App.TipoAnagraficaStato)( Convert.ToInt32( item["Stato"].ToString() ) ) ) )
                            {
                               // case App.TipoAnagraficaStato.InUso:
                               //     cliente += " (In Uso)";
                                //    break;
                                case App.TipoAnagraficaStato.Bloccato:
                                    cliente += " (Bloccato)";
                                    break;
                                case App.TipoAnagraficaStato.Esportato:
                                    cliente += " (Esportato)";
                                    break;
                                case App.TipoAnagraficaStato.Disponibile:
                                case App.TipoAnagraficaStato.Sconosciuto:
                                default:
                                    break;
                            }
                        }

                        myList.Add( new KeyValuePair<string, string>( item["ID"].ToString(), cliente ) );
                    }
				}

				myList.Sort
				(
					delegate(KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
					{
						return firstPair.Value.CompareTo(nextPair.Value);
					}
				);

				foreach (KeyValuePair<string, string> item in myList)
				{
					cmbClienti.Items.Add(item.Value);
					htClienti.Add(index, item.Key);
					index++;
				}

				cmbClienti.SelectedIndex = selectedIndex;
				_cmbInCaricamento = false;
			}
		}

        public wSchedaPianificazioniVigilanzaOLD()
        {
            InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];

      //var
      _InCaricamento = true;
            _DatiCambiati = false;
            RegistrazioneEffettuata = false;

            //interfaccia 
            buttonComando.Visibility = System.Windows.Visibility.Hidden;
            cmbClienti.Focus();
        }

        public void ConfiguraMaschera()
        {
            //inibisco tutto i controlli
            dtpDataInizio.IsHitTestVisible = false;
            dtpDataFine.IsHitTestVisible = false;
            
            switch (TipologiaAttivita)
            {
                case App.TipoAttivitaScheda.New:
                    //labelTitolo.Content = "Nuova Sessione";
                    buttonComando.Content = "Crea";

                    buttonComando.Visibility = System.Windows.Visibility.Visible;
                    dtpDataInizio.IsHitTestVisible = true;
                    dtpDataFine.IsHitTestVisible = true;                    
                    GridComboData.Visibility = System.Windows.Visibility.Collapsed;
                    buttonComando.Visibility = System.Windows.Visibility.Visible;
                    buttonApri.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case App.TipoAttivitaScheda.Edit:
                    //labelTitolo.Content = "Modifica Sessione";
                    buttonComando.Content = "Salva";
                    buttonComando.Visibility = System.Windows.Visibility.Collapsed;
                    dtpDataInizio.IsHitTestVisible = true;
                    dtpDataFine.IsHitTestVisible = true;                 
                    buttonComando.Visibility = System.Windows.Visibility.Collapsed;
                    buttonApri.Visibility = System.Windows.Visibility.Visible;
                    break;
                case App.TipoAttivitaScheda.Delete:
                    //labelTitolo.Content = "Elimina Sessione";
                    buttonComando.Content = "Elimina";
                    buttonComando.Visibility = System.Windows.Visibility.Visible;
                    buttonApri.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case App.TipoAttivitaScheda.Export:
                    //labelTitolo.Content = "Esporta Sessione";
                    buttonComando.Content = "Esporta";
                    buttonComando.Visibility = System.Windows.Visibility.Visible;
                    buttonApri.Visibility = System.Windows.Visibility.Visible;
                    break;
                case App.TipoAttivitaScheda.View:
                default:
                    //labelTitolo.Content = "Apri Sessione in sola lettura";
					cmbData.Visibility = System.Windows.Visibility.Visible;
                    buttonApri.Visibility = System.Windows.Visibility.Visible;
                    //buttonApri.Margin = buttonComando.Margin;
                    break;
            }
        }

        private void cmbClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			//if (OldSelectedCmbClienti != -1)
			//{
			//    MasterFile mf = MasterFile.Create();
			//    mf.SetAnafraficaStato(Convert.ToInt32(OldSelectedCmbClienti), App.TipoAnagraficaStato.Disponibile);
			//}

            //interfaccia
            functionCmbClientiChanged(((ComboBox)sender));
            ConfiguraMaschera();
            if (TipologiaAttivita == App.TipoAttivitaScheda.New)
                dtpDataInizio.Focus();
            else
                cmbData.Focus();
        }

        private void functionCmbClientiChanged(ComboBox cmb)
        {
			if (_cmbInCaricamento)
				return;

			if (oldTipologiaAttivita != App.TipoAttivitaScheda.View)
			{
				TipologiaAttivita = oldTipologiaAttivita;
			}

            _InCaricamento = true;
            cmbData.SelectedIndex = -1;
            dtpDataInizio.Text = "";
            dtpDataFine.Text = "";
            
            if (cmb.SelectedIndex != -1)
            {
                try
                {
                    string IDCliente = htClienti[cmb.SelectedIndex].ToString();

                    OldSelectedCmbClienti = Convert.ToInt32(IDCliente);

                    MasterFile mf = MasterFile.Create();

                    int index = 0;
                    htDate.Clear();
                    cmbData.Items.Clear();

                   List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>();

                   foreach ( Hashtable item in mf.GetPianificazioniVigilanze( IDCliente ) )
					{

                        myList.Add( new KeyValuePair<string, string>( item["ID"].ToString(), item["DataInizio"].ToString() + " - " + item["DataFine"].ToString() ) );
					}

					myList.Sort
					(
						delegate(KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
						{
                            return Convert.ToDateTime( nextPair.Value.Split( ' ' )[0] ).CompareTo( Convert.ToDateTime( firstPair.Value.Split( ' ' )[0] ) );
						}
					);

					foreach (KeyValuePair<string, string> item in myList)
					{
						cmbData.Items.Add(item.Value);
						htDate.Add(index, item.Key);
						index++;
					}

                    //stato
                    if ( IDClienteImport == "-1" )
                    {
                        App.TipoAnagraficaStato anaStato = mf.GetAnafraficaStato( Convert.ToInt32( IDCliente ) );

                        //non disponibile: configuro interfaccia
                        if ( anaStato != App.TipoAnagraficaStato.Disponibile )
                        {
                            oldTipologiaAttivita = TipologiaAttivita;
                            TipologiaAttivita = App.TipoAttivitaScheda.View;
                        }
                    }

                    if (TipologiaAttivita != App.TipoAttivitaScheda.New)
                    {
                        cmbData.IsEnabled = true;
                        dtpDataInizio.IsEnabled = false;
                        dtpDataFine.IsEnabled = false;                      
                    }
                    else
                    {
                        cmbData.IsEnabled = false;
                        dtpDataInizio.IsEnabled = true;
                        dtpDataFine.IsEnabled = true;                       
                    }
                }
                catch (Exception ex)
                {
                    cBusinessObjects.logger.Error(ex, "wSchedaPianificazioneVigilanzaOLD.functionCmbClientiChanged exception");
                    string log = ex.Message;

                    cmbData.IsEnabled = false;
                    dtpDataInizio.IsEnabled = false;
                    dtpDataFine.IsEnabled = false;
                }
            }
        }

        private void cmbData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            functionCmbDataChanged(((ComboBox)sender));
            ConfiguraMaschera();
        }

        private string CorreggiData(string data)
        {
            string returnstring = "00:00";

            if(data.Trim() != "" && data.Contains(':'))
            {
                data = data.Replace(":", "");
                int intdata = 0;
                
                int.TryParse(data, out intdata);

                intdata = intdata - (intdata % 100 % 15);

                data = intdata.ToString().PadLeft( 4, '0' );
                returnstring = data.Substring( 0, 2 ) + ":" + data.Substring( 2, 2 );
            }

            return returnstring;
        }

        private void functionCmbDataChanged(ComboBox cmb)
        {
            if (cmb.SelectedIndex != -1)
            {
                try
                {
                    _InCaricamento = true;

                    string IDPianificazioniVigilanza = htDate[cmb.SelectedIndex].ToString();

                    MasterFile mf = MasterFile.Create();
                    Hashtable htVigilanza = new Hashtable();

                    htVigilanza = mf.GetPianificazioniVigilanza( IDPianificazioniVigilanza );
                    dtpDataInizio.IsEnabled = true;
                    dtpDataFine.IsEnabled = true;

                    dtpDataInizio.Text = htVigilanza["DataInizio"].ToString();
                    dtpDataFine.Text = ( htVigilanza["DataFine"] != null ) ? htVigilanza["DataFine"].ToString() : htVigilanza["DataInizio"].ToString();
                                     
                    _InCaricamento = false;
                }
                catch (Exception ex)
                {
                    cBusinessObjects.logger.Error(ex, "wSchedaPianificazioneVigilanzaOLD.functionCmbDataChanged exception");
                    string log = ex.Message;
                }
            }
        }

        private void buttonComando_Click(object sender, RoutedEventArgs e)
        {
            //controllo selezione clienti
            if (cmbClienti.SelectedIndex == -1)
            {
                MessageBox.Show("selezionare un cliente");
                return;
            }

            MasterFile mf = MasterFile.Create();
            Utilities u = new Utilities();

            int IDCliente = Convert.ToInt32(htClienti[cmbClienti.SelectedIndex].ToString());
            IDPianificazioniVigilanza = App.MasterFile_NewID;

            try
            {
                IDPianificazioniVigilanza = Convert.ToInt32(htDate[cmbData.SelectedIndex].ToString());
            }
            catch (Exception ex)
            {
                cBusinessObjects.logger.Error(ex, "wSchedaPianificazioneVigilanzaOLD.buttonComando_Click exception");
                string log = ex.Message;
            }

            if (TipologiaAttivita == App.TipoAttivitaScheda.Delete && IDPianificazioniVigilanza == -1)
            {
                MessageBox.Show("selezionare una sessione");
                return;
            }

			App.TipoAttivitaScheda oldTipo = TipologiaAttivita;

            switch (TipologiaAttivita)
            {
                //Nuovo e salva
                case App.TipoAttivitaScheda.New:
                case App.TipoAttivitaScheda.Edit:
                    //convalida dati
                    //Campi Obbligatorio
                    if (!u.ConvalidaDatiInterfaccia(dtpDataInizio, "Data Inizio periodo mancante."))
                        return;

                    if ( !u.ConvalidaDatiInterfaccia( dtpDataFine, "Data Fine periodo mancante." ) )
                        return;

					//Controllo che questa data non sia già stata presa
                    if ( !mf.CheckDoppio_PianificazioniVigilanza( IDPianificazioniVigilanza, IDCliente, dtpDataInizio.SelectedDate.Value.ToShortDateString(), dtpDataFine.SelectedDate.Value.ToShortDateString() ) )
					{
						MessageBox.Show("Data già pianificata");
						return;
					}

                    //setto dati
                    Hashtable ht = new Hashtable();
                    ht.Add("Cliente", IDCliente);
                    ht.Add( "DataInizio", dtpDataInizio.SelectedDate.Value.ToShortDateString() );
                    ht.Add( "DataFine", ( ( dtpDataFine.SelectedDate != null ) ? dtpDataFine.SelectedDate.Value.ToShortDateString() : dtpDataInizio.SelectedDate.Value.ToShortDateString() ) );


                    IDPianificazioniVigilanza = mf.SetPianificazioniVigilanza( ht, IDPianificazioniVigilanza, IDCliente );

                    RegistrazioneEffettuata = true;

                    if (TipologiaAttivita == App.TipoAttivitaScheda.New)
                    {
                        if ( IDClienteImport != "-1" )
                        {
                            this.Close();
                        }

                        cBusinessObjects.SessioneIsNew = true;  
                        TipologiaAttivita = App.TipoAttivitaScheda.Edit;
                        mf.SetAnafraficaStato(Convert.ToInt32(IDCliente), App.TipoAnagraficaStato.Disponibile);

                        ConfiguraMaschera();

                        foreach (DictionaryEntry item in htClienti)
                        {
                            if (Convert.ToInt32(item.Value.ToString()) == IDCliente)
                            {
                                cmbClienti.SelectedIndex = Convert.ToInt32(item.Key.ToString());
                            }
                        }

                        functionCmbClientiChanged(cmbClienti);

                        cmbData.IsEnabled = true;

                        foreach (DictionaryEntry item in htDate)
                        {
                            if (Convert.ToInt32(item.Value.ToString()) == IDPianificazioniVigilanza)
                            {
                                cmbData.SelectedIndex = Convert.ToInt32(item.Key.ToString());
                            }
                        }


                        functionCmbDataChanged(cmbData);
                        cBusinessObjects.AddSessione("PianificazioniVigilanza",cmbData.SelectedValue.ToString(), IDPianificazioniVigilanza, IDCliente);

                    }
                    _DatiCambiati = false;
                    break;
                case App.TipoAttivitaScheda.Delete:
                    //richiesta conferma
                    if (MessageBoxResult.No == u.ConfermaCancellazione())
                        return;
                    //cancellazione
                    mf.DeletePianificazioniVigilanza( IDPianificazioniVigilanza, IDCliente.ToString());
                    RegistrazioneEffettuata = true;
                    base.Close();
                    break;
                case App.TipoAttivitaScheda.Export:
                    break;
            }

            //apro tree appena creato
			if (oldTipo == App.TipoAttivitaScheda.New)
            {
                //MessageBox.Show("apro tree appena creato");
				AccediVigilanza_Click(IDPianificazioniVigilanza.ToString(), false);
            }

            //chiudo maschera
            if (TipologiaAttivita != App.TipoAttivitaScheda.Edit)
                base.Close();
        }


        private void AccediVigilanza_Click( string ID, bool ReadOnly )
        {
            try
            {
                if ( noopenaftercreate )
                {
                    return;
                }

                accedi( ID, ReadOnly, false );
            }
            catch ( Exception ex )
            {
                cBusinessObjects.logger.Error(ex, "wSchedaPianificazioneVigilanzaOLD.AccediVigilanza_Click exception");
                string log = ex.Message;
            }
        }

        public void accedi( string ID, bool ReadOnly, bool tobeclosed )
        {
            MasterFile mf = MasterFile.Create();
            Hashtable htSelected = mf.GetPianificazioniVigilanza( ID );

            if(htSelected.Count == 0)
            {
                return;
            }

            WindowWorkAreaTree wWorkArea = new WindowWorkAreaTree();
            //Prisc
            try
            {
                wWorkArea.Owner = this;
            }
            catch (Exception ex)
            {
                cBusinessObjects.logger.Error(ex, "wSchedaPianificazioneVigilanzaOLD.accedi exception");
                string log = ex.Message;
            }
            wWorkArea.SelectedTreeSource = App.AppDataDataFolder + "\\" + htSelected["File"].ToString();
            wWorkArea.SelectedDataSource = App.AppDataDataFolder + "\\" + htSelected["FileData"].ToString();
            wWorkArea.ReadOnly = ReadOnly;
            wWorkArea.TipoAttivita = App.TipoAttivita.PianificazioniVigilanza;
            wWorkArea.Cliente = ( ( (Hashtable)( mf.GetAnagrafica( Convert.ToInt32( htSelected["Cliente"].ToString() ) ) ) )["RagioneSociale"].ToString() ) + " (C.F. " + ( ( (Hashtable)( mf.GetAnagrafica( Convert.ToInt32( htSelected["Cliente"].ToString() ) ) ) )["CodiceFiscale"].ToString() ) + ")";
            wWorkArea.SessioneAlias = "";
            wWorkArea.SessioneFile = "";

            wWorkArea.IDTree = ( Convert.ToInt32( App.TipoFile.PianificazioniVigilanza ) ).ToString();
            wWorkArea.IDCliente = htSelected["Cliente"].ToString();
            wWorkArea.IDSessione = ID;           

            foreach ( Hashtable item in ( (ArrayList)( mf.GetPianificazioniVigilanze( htSelected["Cliente"].ToString() ) ) ) )
            {
                wWorkArea.SessioneAliasAdditivo += ( ( wWorkArea.SessioneAliasAdditivo == "" ) ? "" : "|" ) + item["DataFine"].ToString();
                wWorkArea.SessioneFile += ( ( wWorkArea.SessioneFile == "" ) ? "" : "|" ) + App.AppDataDataFolder + "\\" + item["FileData"].ToString();
                wWorkArea.SessioneAlias += ( ( wWorkArea.SessioneAlias == "" ) ? "" : "|" ) + item["DataInizio"].ToString();
                wWorkArea.SessioneID += ( ( wWorkArea.SessioneID == "" ) ? "" : "|" ) + item["ID"].ToString();
            }


            //aperto in sola lettura
            wWorkArea.ApertoInSolaLettura = TipologiaAttivita == App.TipoAttivitaScheda.View;

            wWorkArea.LoadTreeSource();
            wWorkArea.ShowDialog();

            //setto dati
            Hashtable ht = new Hashtable();
            ht.Add( "Cliente", Convert.ToInt32( htSelected["Cliente"].ToString() ) );


            ht.Add( "DataInizio", dtpDataInizio.SelectedDate.Value.ToShortDateString() );
            ht.Add( "DataFine", dtpDataFine.SelectedDate.Value.ToShortDateString() );

            mf.SetPianificazioniVigilanza( ht, Convert.ToInt32( ID ), Convert.ToInt32( htSelected["Cliente"].ToString() ) );

            if ( TipologiaAttivita != App.TipoAttivitaScheda.View )
            {
                int IDCliente = Convert.ToInt32( htClienti[cmbClienti.SelectedIndex].ToString() );
                mf.SetAnafraficaStato( Convert.ToInt32( IDCliente ), App.TipoAnagraficaStato.Disponibile );
            }

            functionCmbDataChanged( cmbData );

            if ( tobeclosed == true )
            {
                base.Close();
            }
        }


        private void ButtonApri_Click(object sender, RoutedEventArgs e)
        {
            //controllo selezione clienti
            if (cmbClienti.SelectedIndex == -1)
            {
                MessageBox.Show("selezionare un cliente");
                return;
            }

            //dati modificati
            if (_DatiCambiati)
            {
				//Utilities u = new Utilities();
				//if (MessageBoxResult.No == u.AvvisoPerditaDati("Alcuni dati sono stati modificati, confermi apertura?"))
				//    return;

				//Salvataggio automatico come richiesto da 2.3
				App.TipoAttivitaScheda OLDTipologiaAttivita = TipologiaAttivita;
				TipologiaAttivita = App.TipoAttivitaScheda.Edit;
				buttonComando_Click(sender, e);
				TipologiaAttivita = OLDTipologiaAttivita;
            }

			//disponibile: blocco cliente
			int IDCliente = Convert.ToInt32(htClienti[cmbClienti.SelectedIndex].ToString());
			MasterFile mf = MasterFile.Create();
			App.TipoAnagraficaStato anaStato = mf.GetAnafraficaStato(IDCliente);

            if (anaStato == App.TipoAnagraficaStato.Disponibile && TipologiaAttivita != App.TipoAttivitaScheda.View)
                mf.SetAnafraficaStato(Convert.ToInt32(IDCliente), App.TipoAnagraficaStato.InUso);

            //apre treee
            int IDPianificazioniVigilanza = App.MasterFile_NewID;

            try
            {
                IDPianificazioniVigilanza = Convert.ToInt32( htDate[cmbData.SelectedIndex].ToString() );
            }
            catch ( Exception ex )
            {
                cBusinessObjects.logger.Error(ex, "wSchedaPianificazioneVigilanzaOLD.ButtonApri_Click exception");
                string log = ex.Message;
            }

            if ( IDPianificazioniVigilanza == -1 )
            {
                MessageBox.Show( "selezionare una sessione" );
            }
            else
            {
                bool isSchedaReadOnly = TipologiaAttivita == App.TipoAttivitaScheda.View || anaStato == App.TipoAnagraficaStato.InUso;
                Accedi_Click( IDPianificazioniVigilanza.ToString(), isSchedaReadOnly );
            }
        }

        private void Accedi_Click( string ID, bool ReadOnly )
        {
            try
            {
                if ( noopenaftercreate )
                {
                    return;
                }

                accedi( ID, ReadOnly, true );

            }
            catch ( Exception ex )
            {
                cBusinessObjects.logger.Error(ex, "wSchedaPianificazioneVigilanzaOLD.Accedi_Click exception");
                string log = ex.Message;
            }
        }

        private void buttonChiudi_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void GestoreEvento_DatiCambiati(object sender, RoutedEventArgs e)
        {
            if (_InCaricamento)
                return;
            _DatiCambiati = true;
        }
        
        private void GestoreEvento_ChiusuraFinestra(object sender, CancelEventArgs e)
        {
            if ( IDClienteImport != "-1" )
            {
                return;
            }

			if (cmbClienti.SelectedIndex != -1 && TipologiaAttivita == App.TipoAttivitaScheda.Edit)
			{
				App.TipoAttivitaScheda OLDTipologiaAttivita = TipologiaAttivita;
				//TipologiaAttivita = App.TipoAttivitaScheda.Edit;
				buttonComando_Click(sender, new RoutedEventArgs());
				TipologiaAttivita = OLDTipologiaAttivita;
			}

			return;
        }

        private void GestoreEvento_ComboEsercizio_Checked(object sender, CancelEventArgs e)
        {
            _DatiCambiati = true;
        }

        private void dtpDataInizio_MouseDown( object sender, MouseButtonEventArgs e )
        {
            //if(dtpDataInizio.IsHitTestVisible == false)
            //{
            //    MessageBox.Show( "..." );
            //}
        }


        private void dtpDataFine_MouseDown( object sender, MouseButtonEventArgs e )
        {
            //if(dtpDataFine.IsHitTestVisible == false)
            //{
            //    MessageBox.Show( "..." );
            //}
        }
    }
}
