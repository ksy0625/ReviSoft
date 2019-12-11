//----------------------------------------------------------------------------+
//                             MainWindow.xaml.cs                             |
//----------------------------------------------------------------------------+
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
using System.Xml;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;

namespace RevisoftApplication
{
  public partial class MainWindow : Window
  {
    bool showallclienti = true;
    //dati anagrafiche
    int counterClienti = 0;
    Hashtable htClienti_ID = new Hashtable();
    Hashtable htClienti_RS = new Hashtable();
    Hashtable htClienti_CF = new Hashtable();
    Hashtable htClienti_PIVA = new Hashtable();
    Hashtable htClienti_Ese = new Hashtable();
    Hashtable htClienti_Stato = new Hashtable();
    Hashtable htClienti_StatoDesc = new Hashtable();
    Hashtable htClienti_StatoIcon = new Hashtable();
    //costanti Colori
    //Brush GridAlternateColorOdd = new SolidColorBrush(Color.FromArgb(126, 241, 241, 241));
    //Brush GridAlternateColorEven = new SolidColorBrush(Color.FromArgb(126, 211, 211, 211));
    //Brush GridSelectedColor = new SolidColorBrush(Color.FromArgb(126, 130, 189, 228));
    //Brush GridHoverColor = new SolidColorBrush(Color.FromArgb(126, 123, 225, 72));
    //Brush GridHoverColor = new SolidColorBrush(Color.FromArgb(126, 245, 164, 28));
    //Brush ComboColor = new SolidColorBrush(Color.FromArgb(255, 82, 101, 115));
    //Brush ComboSelectedColor = new SolidColorBrush(Color.FromArgb(255, 247, 168, 39));
    //selezione cliente
    Brush GridOldBackground = null;
    Brush GridSelectedBackground = null;
    Grid gridSelected = null;
    int IndexSelected = -1;

    //----------------------------------------------------------------------------+
    //                                 MainWindow                                 |
    //----------------------------------------------------------------------------+
    public MainWindow()
    {
      InitializeComponent();
      System.Windows.Application.Current.MainWindow = this;
#if (DBG_TEST)
      int i;
      string[] exts = new string[] { "ok", "mod", "bad" };

      StaticUtilities.ClearXmlCache();
      foreach (string ext in exts)
      {
        string[] files = System.IO.Directory.GetFiles(@".\", "*." + ext,
          System.IO.SearchOption.TopDirectoryOnly);
        for (i = 0; i < files.Length; i++) File.Delete(files[i]);
      }
      StaticUtilities.SetLockStatus("", "", false);
#endif
      //creo obj utilities
      RevisoftApplication.Utilities u = new Utilities();
      //Gestione licenza
      RevisoftApplication.GestioneLicenza l = new GestioneLicenza();

      // TEAM
      // si verifica se nella tabella RUOLI è presente il ruolo RUO_ID=100	e RUO_DESCR=NO_TEAM
      // se è presente l'applicazione parte in modalità standalone senza necessità di login
      if (RevisoftApplication.BRL.cRuoli.IsStandAlone())
      {
        App.AppTipo = App.ModalitaApp.StandAlone;
        App.AppRuolo = App.RuoloDesc.StandAlone;
        App.AppUtente = new BRL.Utente();
        App.AppUtente.RuoId = (int)App.RuoloDesc.StandAlone;
      }
      else
      {
        // l'utente deve eseguire il log in
        wLogin loginWindow = new wLogin();
        loginWindow.ShowDialog();
        if (!loginWindow.loginOk)
          u.ChiudiApplicazioneSuErrore();
      }


      l.Inizializza();
      //Licenza NON attiva
      if (!l.StatoLicenza)
      {
        //Configuro registro di sistema
        //u.ConfiguraRegistroApplicazione();
        //u.ConfiguraRegistroApplicazioneEstensioni();
        //Attivazione prima licenza
        PermettiAttivazioneLicenza(l);
        //Finestra messaggio dopo creazione licenza di prova
        if (App.Prova == true)
        {
          App.ErrorLevel = App.ErrorTypes.Segnalazione;
          RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
          m.TipoMessaggioSegnalazione = WindowGestioneMessaggi.TipologieMessaggiSegnalazioni.LicenzaProvaCreata;
          m.VisualizzaMessaggio();
        }
      }
      //Licenza NON attiva BIS
      //if (App.TipoLicenza == App.TipologieLicenze.Ignota)
      if (!l.StatoLicenza) u.ChiudiApplicazione();
      //Licenza SCADUTA
      //if (App.TipoLicenza == App.TipologieLicenze.Scaduta)
      if (App.Scaduta == true) PermettiAttivazioneLicenza(l);
      //4.8
      //4.5/4.6 esco se richieste App.AppAutoExec
      if (App.AppAutoExec) return;
      //Verifico chiave file dati 
      l.VerificaInfoMasterFile();
      //Setto Info file
      l.SalvaInfoDataUltimoUtilizzo();
      //Carico dati
      MasterFile mf = MasterFile.Create();
      //Attivo applicazione x diritti licenza e numero anagrafiche
      if (l.StatoLicenza)
      {
        //conteggio anagrafiche
        counterClienti = mf.GetAnagraficheCount();
        //interfaccia
        StatusBarDatiUtente(l);
        ConfiguraInterfacciaClientiPerStato();
        ConfiguraInterfaccia();
        //apro maschera clienti se archivio vuoto
        // TEAM - procedimento modificato in funzione della modalità applicazione
        if (App.AppTipo == App.ModalitaApp.StandAlone || App.AppTipo == App.ModalitaApp.Administrator)
        {
          if (counterClienti == 0 && !App.AppAutoExec) // && App.TipoLicenza != App.TipologieLicenze.Viewer)
          {
            App.AppAutoExec = true;
            App.AppAutoExecFunzione = App.TipoFunzioniAutoexec.NuovoCliente;
          }
        }

      }
      mf.SplitVerificheVigilanze();

      // Team - Pat
      if (App.AppTipo != App.ModalitaApp.Administrator)
      {
        // la gestione degli utenti e l'associazione dei clienti al team è visibile al solo utente administrator
        btn_Apri_Utenti.Visibility = Visibility.Collapsed;
        btn_AssociaClientiLeader.Visibility = Visibility.Collapsed;

        // associazione utenti possibile al solo utente team leader
        if (App.AppTipo == App.ModalitaApp.Team)
        {
          if (App.AppUtente.RuoId == (int)App.RuoloDesc.TeamLeader)
          {
            txtTipoUtenteLoggato.Text = "Team Leader:";
            txtUtenteLoggato.Text = $" {App.AppUtente.Nome} {App.AppUtente.Cognome}";
            btn_Associa_Utenti.Visibility = Visibility.Visible;
            btn_AssegnaIncarico.Visibility = Visibility.Visible;
            btn_Riepilogo_Utenti.Visibility = Visibility.Collapsed;
          }
          else
          {
            // esecutore o revisore
            if (App.AppUtente.RuoId == (int)App.RuoloDesc.Reviewer)
            {
              txtTipoUtenteLoggato.Text = "Reviewer:";
              txtUtenteLoggato.Text = $" {App.AppUtente.Nome} {App.AppUtente.Cognome}";
            }
            else
            {
              txtTipoUtenteLoggato.Text = "Esecutore:";
              txtUtenteLoggato.Text = $" {App.AppUtente.Nome} {App.AppUtente.Cognome}";
            }
            btn_Associa_Utenti.Visibility = Visibility.Collapsed;
            btn_Riepilogo_Utenti.Visibility = Visibility.Visible;
          }

        }
        else
        {
          btn_Riepilogo_Utenti.Visibility = Visibility.Collapsed;
          btn_Associa_Utenti.Visibility = Visibility.Collapsed;
        }

      }
      else
      {
        // administrator

        txtUtenteLoggato.Text = $"Administrator";
        btn_Apri_Utenti.Visibility = Visibility.Visible;
        btn_Riepilogo_Utenti.Visibility = Visibility.Collapsed;
        btn_AssociaClientiLeader.Visibility = Visibility.Visible;
        btn_Associa_Utenti.Visibility = Visibility.Collapsed;
        txtTipoUtenteLoggato.Visibility = Visibility.Collapsed;
      }


      //adesso vado a OnContentRendered
      //mf.CheckAndNormalizeDocuments();
      mf.UpdateTipoEsercisioSu239();

      Expander_Clienti.IsExpanded = false;
      btn_Cliente_Sblocca_Start.Visibility = Visibility.Collapsed;
      btn_Cliente_Fissa_Start.Visibility = Visibility.Visible;
      Expander_Revisione.IsEnabled = true;
      Expander_Verifiche.IsEnabled = true;
      Expander_Accettazione.IsEnabled = true;
      Expander_Relazioni.IsEnabled = true;
      if (mf.GetClienteFissato() != "-1")
      {
        Expander_Revisione.IsEnabled = true;
        Expander_Verifiche.IsEnabled = true;
        Expander_Accettazione.IsEnabled = true;
        Expander_Relazioni.IsEnabled = true;

      }


    }

    void ImpostaColori()
    {
      int[] arrIndexes = { 2, 4, 6, 8 };
      TextBlock tb;
      TextBlock[] tbs =
      {
        tbClienti,tbAccettazione,tbRevisione,tbBilancio,
        tbRelazioni,tbFlussi,tbTeamRev,tbVerifiche,tbStrumenti
      };
      RadioButton[] rdbs =
      {
        Area1CS, Area1REV, Area1SU, rdb_createam, rdb_Flusso_Gruppo,
        rdb_Flusso_ISQC, rdb_Flusso_Societa, rdb_Flusso_Terzi, rdbBilancio,
        rdbConclusioni, rdbISQC, rdbPianificazioniVerifica,
        rdbPianificazioniVigilanza, rdbRelazioneB, rdbRelazioneBC,
        rdbRelazioneBV, rdbRelazioneV, rdbRelazioneVC, rdbRevisione,
        rdbVerifica, rdbVigilanza
      };

      // testo expanders
      foreach (TextBlock t in tbs) t.Foreground = App._arrBrushes[0];
      // testo radio buttons
      foreach (RadioButton r in rdbs) r.Foreground= App._arrBrushes[2];
      // intestazioni lista clienti
      foreach (int i in arrIndexes)
      {
        tb = (TextBlock)((Grid)stpClienti_RagioneSociale.Parent).Children[i];
        tb.Background = App._arrBrushes[3];
        tb.Foreground = App._arrBrushes[4];
      }
      // logo
      if (!string.IsNullOrEmpty(App._logoPath))
        logo.Source = new BitmapImage(new Uri(App._logoPath));
      // barra pulsanti
      ButtonBar.Background = App._arrBrushes[12];
      SolidColorBrush tmpBrush = (SolidColorBrush)Resources["buttonHover"];
      tmpBrush.Color = ((SolidColorBrush)App._arrBrushes[13]).Color;
    }

    //----------------------------------------------------------------------------+
    //                             MainWindow_Closed                              |
    //----------------------------------------------------------------------------+
    void MainWindow_Closed(object sender, EventArgs e)
    {
      MasterFile mf = MasterFile.Create();
      mf.SetClienteFissato("-1");
#if (DBG_TEST)
      StaticUtilities.SetLockStatus("", "", false);
      //StaticUtilities.DumpModifiedCache();
      //      //StaticUtilities.ClearXmlCache();
      //      using (SqlConnection conn = new SqlConnection(App.connString))
      //      {
      //          conn.Open();
      //          SqlCommand cmd = new SqlCommand("dbo.SaveModified", conn);
      //          cmd.CommandType = CommandType.StoredProcedure;
      //          cmd.CommandTimeout = App.m_CommandTimeout;
      //          try { cmd.ExecuteNonQuery(); }
      //          catch (Exception ex)
      //          {
      //              if (!App.m_bNoExceptionMsg)
      //              {
      //                  string msg = "SQL call 'dbo.SaveModified' failed: errore\n" + ex.Message;
      //                  MessageBox.Show(msg);
      //              }
      //          }
      //      }
#endif
      RevisoftApplication.Utilities u = new Utilities();
      ((System.ComponentModel.CancelEventArgs)e).Cancel = u.ChiudiApplicazioneConBackup();
    }

    //----------------------------------------------------------------------------+
    //                             RepeatRevisoftInit                             |
    //                funzione di sistema per passaggio di licenza                |
    //----------------------------------------------------------------------------+
    public void RepeatRevisoftInit()
    {
      //Gestione licenza
      RevisoftApplication.GestioneLicenza l = new GestioneLicenza();
      l.ConfiguraLicenza();
      if (!File.Exists(App.AppMasterDataFile) && File.Exists(App.AppMasterDataFile + ".example"))
      {
        System.IO.File.Move(App.AppMasterDataFile + ".example", App.AppMasterDataFile);
      }


      //Configurazione path di rete
      // E.B.+L.C. - commentato per by-passare verifica percorso di rete
      ////if (((App.TipoLicenza == App.TipologieLicenze.ClientLan || App.TipoLicenza == App.TipologieLicenze.ClientLanMulti || App.TipoLicenza == App.TipologieLicenze.Server) && (App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Locale || App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Remoto)) || (App.TipoLicenza == App.TipologieLicenze.ClientLan && !File.Exists(App.AppMasterDataFile)))
      //if (!File.Exists(App.AppMasterDataFile))
      //{
      //  //App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Remoto;
      //  wGestioneArchivio w = new wGestioneArchivio();
      //  w.Owner = this;
      //  w.tabControl1.SelectedIndex = 1;
      //  w.ShowDialog();
      //}


      //Verifico chiave file dati
      l.VerificaInfoMasterFile();
      //Attivo applicazione x diritti licenza e numero anagrafiche
      if (l.StatoLicenza)
      {
        //conteggio anagrafiche
        MasterFile m = MasterFile.Create();
        counterClienti = m.GetAnagraficheCount();
        //interfaccia
        StatusBarDatiUtente(l);
        ConfiguraInterfacciaClientiPerStato();
        ConfiguraInterfaccia();
      }
    }

    //----------------------------------------------------------------------------+
    //                              ReloadMainWindow                              |
    //----------------------------------------------------------------------------+
    public void ReloadMainWindow()
    {
      //Ricarico dati
      CaricaClienti();
      //Configura interfaccia x licenza e numero anagrafiche *** non invertire ordine
      ConfiguraInterfacciaClientiPerStato();
      ConfiguraInterfaccia();
      //configura status bar 2.8.1
      RevisoftApplication.GestioneLicenza l = new GestioneLicenza();
      StatusBarDatiUtente(l);
    }

    #region MENU

    #region FILE
    //----------------------------------------------------------------------------+
    //                             menuFileEsci_Click                             |
    //                            Gestione menù: FILE                             |
    //----------------------------------------------------------------------------+
    private void menuFileEsci_Click(object sender, RoutedEventArgs e)
    {
      MasterFile mf = MasterFile.Create();
      mf.SetClienteFissato("-1");
#if (DBG_TEST)
      StaticUtilities.SetLockStatus("", "", false);
      //StaticUtilities.DumpModifiedCache();
      //      //StaticUtilities.ClearXmlCache();
      //      using (SqlConnection conn = new SqlConnection(App.connString))
      //      {
      //          conn.Open();
      //          SqlCommand cmd = new SqlCommand("dbo.SaveModified", conn);
      //          cmd.CommandType = CommandType.StoredProcedure;
      //          cmd.CommandTimeout = App.m_CommandTimeout;
      //          try { cmd.ExecuteNonQuery(); }
      //          catch (Exception ex)
      //          {
      //              if (!App.m_bNoExceptionMsg)
      //              {
      //                  string msg = "SQL call 'dbo.SaveModified' failed: errore\n" + ex.Message;
      //                  MessageBox.Show(msg);
      //              }
      //          }
      //      }
#endif
      RevisoftApplication.Utilities u = new Utilities();
      u.ChiudiApplicazioneConBackup();
    }
    #endregion //------------------------------------------------------------- FILE

    #region CLIENTI
    //Gestione menù: CLIENTI ********************************************

    //----------------------------------------------------------------------------+
    //                       menuClientiSuggerimenti_Click                        |
    //----------------------------------------------------------------------------+
    private void menuClientiSuggerimenti_Click(object sender, RoutedEventArgs e)
    {
      wSuggerimentiUsoCliente w = new wSuggerimentiUsoCliente();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                           menuClientiNuovo_Click                           |
    //----------------------------------------------------------------------------+
    private void menuClientiNuovo_Click(object sender, RoutedEventArgs e)
    {
      CaricaClienti();
      wSchedaAnafrafica w = new wSchedaAnafrafica();
      w.TipologiaAttivita = App.TipoAttivitaScheda.New;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
      if (w.RegistrazioneEffettuata) CaricaClienti();
    }

    //----------------------------------------------------------------------------+
    //                         menuClientiModifica_Click                          |
    //----------------------------------------------------------------------------+
    private void menuClientiModifica_Click(object sender, RoutedEventArgs e)
    {
      if (IndexSelected == -1)
      {
        MessageBox.Show("Selezionare un cliente per accedere alla sua scheda anagrafica.");
        return;
      }
      //carico dati
      wSchedaAnafrafica w = new wSchedaAnafrafica();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Edit;
      w.idRecord = Convert.ToInt32(((TextBlock)(((Border)(stpClienti_ID.Children[IndexSelected])).Child)).Text);
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
      if (w.RegistrazioneEffettuata) CaricaClienti();
    }

    //----------------------------------------------------------------------------+
    //                           menuClientiVedi_Click                            |
    //----------------------------------------------------------------------------+
    private void menuClientiVedi_Click(object sender, RoutedEventArgs e)
    {
      if (IndexSelected == -1)
      {
        MessageBox.Show("Selezionare un cliente per visualizzare la sua scheda anagrafica.");
        return;
      }
      //carico dati
      wSchedaAnafrafica w = new wSchedaAnafrafica();
      w.TipologiaAttivita = App.TipoAttivitaScheda.View;
      w.idRecord = Convert.ToInt32(((TextBlock)(((Border)(stpClienti_ID.Children[IndexSelected])).Child)).Text);
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
      if (w.RegistrazioneEffettuata) CaricaClienti();
    }

    //----------------------------------------------------------------------------+
    //                          menuClientiElimina_Click                          |
    //----------------------------------------------------------------------------+
    private void menuClientiElimina_Click(object sender, RoutedEventArgs e)
    {
      if (IndexSelected == -1)
      {
        MessageBox.Show("Selezionare un cliente per eliminare la sua scheda anagrafica.");
        return;
      }
      //carico dati
      wSchedaAnafrafica w = new wSchedaAnafrafica();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Delete;
      w.idRecord = Convert.ToInt32(((TextBlock)(((Border)(stpClienti_ID.Children[IndexSelected])).Child)).Text);
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
      if (w.RegistrazioneEffettuata) CaricaClienti();
    }

    //----------------------------------------------------------------------------+
    //                         menuClientiCondividi_Click                         |
    //----------------------------------------------------------------------------+
    private void menuClientiCondividi_Click(object sender, RoutedEventArgs e)
    {
      if (IndexSelected == -1)
      {
        MessageBox.Show("Selezionare un cliente per condividere la sua scheda anagrafica, le sessioni ed relativi documenti.");
        return;
      }
      //carico dati
      wSchedaAnafrafica w = new wSchedaAnafrafica();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Condividi;
      w.idRecord = Convert.ToInt32(((TextBlock)(((Border)(stpClienti_ID.Children[IndexSelected])).Child)).Text);
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
      if (w.RegistrazioneEffettuata) CaricaClienti();
    }

    //----------------------------------------------------------------------------+
    //                          menuClientiEsporta_Click                          |
    //----------------------------------------------------------------------------+
    private void menuClientiEsporta_Click(object sender, RoutedEventArgs e)
    {
      if (IndexSelected == -1)
      {
        MessageBox.Show("Selezionare un cliente per esportare la sua scheda anagrafica, le sessioni ed relativi documenti.");
        return;
      }
      //andrea - 4.7 controllo per esportazione
      if ((App.Server || App.Client) && App.AppPathArchivioRemoto == "")
      {
        MessageBox.Show("Percorso Archivio Remoto non configurato, impossibile esportare.");
        return;
      }
      //carico dati
      wSchedaAnafrafica w = new wSchedaAnafrafica();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Export;
      w.idRecord = Convert.ToInt32(((TextBlock)(((Border)(stpClienti_ID.Children[IndexSelected])).Child)).Text);
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
      if (w.RegistrazioneEffettuata) CaricaClienti();
    }

    //----------------------------------------------------------------------------+
    //                          menuClientiImporta_Click                          |
    //----------------------------------------------------------------------------+
    private void menuClientiImporta_Click(object sender, RoutedEventArgs e)
    {
      RevisoftApplication.Utilities u = new Utilities();
      string ret = u.sys_OpenFileDialog("Importazione Cliente", App.TipoFile.ImportExport);
      if (ret != null)
      {
        cImportExport.Import(ret);
        ReloadMainWindow();
      }
    }

    //----------------------------------------------------------------------------+
    //                       menuClientiDeFissa_Start_Click                       |
    //----------------------------------------------------------------------------+
    private void menuClientiDeFissa_Start_Click(object sender, RoutedEventArgs e)
    {
      MasterFile mf = MasterFile.Create();
      StatusBarItem_ClienteFisso.Content = "";
      mf.SetClienteFissato("-1");
      showallclienti = true;
      Imposta_Scegli_Esci_Cliente();
      CaricaClienti();
    }

    //----------------------------------------------------------------------------+
    //                        menuClientiFissa_Start_Click                        |
    //----------------------------------------------------------------------------+
    private void menuClientiFissa_Start_Click(object sender, RoutedEventArgs e)
    {
      wSchedaSelezionaCliente s = new wSchedaSelezionaCliente();
      s.Owner = this;
      s.ShowDialog();
      if (s.RagioneSociale != "")
      {
        MasterFile mf = MasterFile.Create();
        mf.SetClienteFissato(s.IDCliente);
        StatusBarItem_ClienteFisso.Content = "Cliente Scelto: " + s.RagioneSociale;
        showallclienti = false;
        ExpanderClientiExpanded();
        showallclienti = false;
        Expander_Revisione.IsEnabled = true;
        Expander_Verifiche.IsEnabled = true;
        Expander_Accettazione.IsEnabled = true;
        Expander_Relazioni.IsEnabled = true;
      }
    }

    //----------------------------------------------------------------------------+
    //                           menuClientiFissa_Click                           |
    //----------------------------------------------------------------------------+
    private void menuClientiFissa_Click(object sender, RoutedEventArgs e)
    {
      MasterFile mf = MasterFile.Create();
      string IDCliente = ((TextBlock)(((Border)(stpClienti_ID.Children[IndexSelected])).Child)).Text;
      if (IDCliente != mf.GetClienteFissato())
      {
        mf.SetClienteFissato(IDCliente);
        StatusBarItem_ClienteFisso.Content = "Cliente Scelto: " + ((TextBlock)(((Border)(stpClienti_RagioneSociale.Children[IndexSelected])).Child)).Text;
      }
      else
      {
        StatusBarItem_ClienteFisso.Content = "";
        mf.SetClienteFissato("-1");
      }
    }

    #endregion //---------------------------------------------------------- CLIENTI

    #region FLUSSI
    //Gestione menù: FLUSSI ********************************************

    //----------------------------------------------------------------------------+
    //                            menuFlussiApri_Click                            |
    //----------------------------------------------------------------------------+
    private void menuFlussiApri_Click(object sender, RoutedEventArgs e)
    {
      wSchedaSceltaFlussi w = new wSchedaSceltaFlussi();
      if (rdb_Flusso_ISQC.IsChecked == true)
      {
        w.tipo = wSchedaSceltaFlussi.TipoFlusso.ISQC;
      }
      if (rdb_Flusso_Societa.IsChecked == true)
      {
        w.tipo = wSchedaSceltaFlussi.TipoFlusso.Societa;
      }
      if (rdb_Flusso_Gruppo.IsChecked == true)
      {
        w.tipo = wSchedaSceltaFlussi.TipoFlusso.Gruppo;
      }
      if (rdb_Flusso_Terzi.IsChecked == true)
      {
        w.tipo = wSchedaSceltaFlussi.TipoFlusso.Terzi;
      }
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    #endregion //----------------------------------------------------------- FLUSSI

    #region INCARICHI
    //Gestione menù: INCARICHI ********************************************

    //----------------------------------------------------------------------------+
    //                          menuIncarichiNuovo_Click                          |
    //----------------------------------------------------------------------------+
    private void menuIncarichiNuovo_Click(object sender, RoutedEventArgs e)
    {
      wSchedaIncarico w = new wSchedaIncarico();
      w.TipologiaAttivita = App.TipoAttivitaScheda.New;
      w.Riesame = false;
      if (Area1CS.IsChecked == true)
        w.area1 = "CS";
      if (Area1SU.IsChecked == true)
        w.area1 = "SU";
      if (Area1REV.IsChecked == true)
        w.area1 = "REV";
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                         menuIncarichiRiesame_Click                         |
    //----------------------------------------------------------------------------+
    private void menuIncarichiRiesame_Click(object sender, RoutedEventArgs e)
    {
      wSchedaIncarico w = new wSchedaIncarico();

      if (Area1CS.IsChecked == true)
        w.area1 = "CS";
      if (Area1SU.IsChecked == true)
        w.area1 = "SU";
      if (Area1REV.IsChecked == true)
        w.area1 = "REV";
      w.TipologiaAttivita = App.TipoAttivitaScheda.New;
      w.Riesame = true;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                        menuIncarichiModifica_Click                         |
    //----------------------------------------------------------------------------+
    private void menuIncarichiModifica_Click(object sender, RoutedEventArgs e)
    {
      wSchedaIncarico w = new wSchedaIncarico();

      if (Area1CS.IsChecked == true)
        w.area1 = "CS";
      if (Area1SU.IsChecked == true)
        w.area1 = "SU";
      if (Area1REV.IsChecked == true)
        w.area1 = "REV";
      w.TipologiaAttivita = App.TipoAttivitaScheda.Edit;
      w.ConfiguraMaschera();
      w.Owner = this;
      //Hide();
      w.ShowDialog();
      Show(); //Activate();
    }

    //----------------------------------------------------------------------------+
    //                         menuIncarichiElimina_Click                         |
    //----------------------------------------------------------------------------+
    private void menuIncarichiElimina_Click(object sender, RoutedEventArgs e)
    {
      wSchedaIncarico w = new wSchedaIncarico();


      if (Area1CS.IsChecked == true)
        w.area1 = "CS";
      if (Area1SU.IsChecked == true)
        w.area1 = "SU";
      if (Area1REV.IsChecked == true)
        w.area1 = "REV";
      w.TipologiaAttivita = App.TipoAttivitaScheda.Delete;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();

    }

    //----------------------------------------------------------------------------+
    //                          menuIncarichiVedi_Click                           |
    //----------------------------------------------------------------------------+
    private void menuIncarichiVedi_Click(object sender, RoutedEventArgs e)
    {
      wSchedaIncarico w = new wSchedaIncarico();
      if (Area1CS.IsChecked == true)
        w.area1 = "CS";
      if (Area1SU.IsChecked == true)
        w.area1 = "SU";
      if (Area1REV.IsChecked == true)
        w.area1 = "REV";
      w.TipologiaAttivita = App.TipoAttivitaScheda.View;
      w.ConfiguraMaschera();

      w.Owner = this;
      w.ShowDialog();
    }

    #endregion //-------------------------------------------------------- INCARICHI

    #region ISQCs
    //Gestione menù: ISQCs ********************************************

    //----------------------------------------------------------------------------+
    //                            menuISQCsNuovo_Click                            |
    //----------------------------------------------------------------------------+
    private void menuISQCsNuovo_Click(object sender, RoutedEventArgs e)
    {
      wSchedaISQC w = new wSchedaISQC();
      w.TipologiaAttivita = App.TipoAttivitaScheda.New;
      w.Riesame = false;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                           menuISQCsRiesame_Click                           |
    //----------------------------------------------------------------------------+
    private void menuISQCsRiesame_Click(object sender, RoutedEventArgs e)
    {
      wSchedaISQC w = new wSchedaISQC();
      w.TipologiaAttivita = App.TipoAttivitaScheda.New;
      w.Riesame = true;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                          menuISQCsModifica_Click                           |
    //----------------------------------------------------------------------------+
    private void menuISQCsModifica_Click(object sender, RoutedEventArgs e)
    {
      wSchedaISQC w = new wSchedaISQC();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Edit;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                           menuISQCsElimina_Click                           |
    //----------------------------------------------------------------------------+
    private void menuISQCsElimina_Click(object sender, RoutedEventArgs e)
    {
      wSchedaISQC w = new wSchedaISQC();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Delete;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                            menuISQCsVedi_Click                             |
    //----------------------------------------------------------------------------+
    private void menuISQCsVedi_Click(object sender, RoutedEventArgs e)
    {
      wSchedaISQC w = new wSchedaISQC();
      w.TipologiaAttivita = App.TipoAttivitaScheda.View;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    #endregion //------------------------------------------------------------ ISQCs

    #region REVISIONI
    //Gestione menù: REVISIONI ************************************************

    //----------------------------------------------------------------------------+
    //                          menuRevisioniNuova_Click                          |
    //----------------------------------------------------------------------------+
    private void menuRevisioniNuova_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRevisione w = new wSchedaRevisione();
      w.TipologiaAttivita = App.TipoAttivitaScheda.New;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                        menuRevisioniModifica_Click                         |
    //----------------------------------------------------------------------------+
    private void menuRevisioniModifica_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRevisione w = new wSchedaRevisione();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Edit;
      w.ConfiguraMaschera();
      w.Owner = this;
      //Hide();
      w.ShowDialog();
      Show();
    }

    //----------------------------------------------------------------------------+
    //                         menuRevisioniElimina_Click                         |
    //----------------------------------------------------------------------------+
    private void menuRevisioniElimina_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRevisione w = new wSchedaRevisione();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Delete;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                          menuRevisioniVedi_Click                           |
    //----------------------------------------------------------------------------+
    private void menuRevisioniVedi_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRevisione w = new wSchedaRevisione();
      w.TipologiaAttivita = App.TipoAttivitaScheda.View;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    #endregion //-------------------------------------------------------- REVISIONI

    #region BILANCI

    //Gestione menù: BILANCI ********************************************

    //----------------------------------------------------------------------------+
    //                          menuBilancioNuova_Click                           |
    //----------------------------------------------------------------------------+
    private void menuBilancioNuova_Click(object sender, RoutedEventArgs e)
    {
      wSchedaBilancio w = new wSchedaBilancio();
      w.TipologiaAttivita = App.TipoAttivitaScheda.New;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                         menuBilancioModifica_Click                         |
    //----------------------------------------------------------------------------+
    private void menuBilancioModifica_Click(object sender, RoutedEventArgs e)
    {
      wSchedaBilancio w = new wSchedaBilancio();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Edit;
      w.ConfiguraMaschera();
      w.Owner = this;
      //Hide();
      w.ShowDialog();
      Show();
    }

    //----------------------------------------------------------------------------+
    //                         menuBilancioElimina_Click                          |
    //----------------------------------------------------------------------------+
    private void menuBilancioElimina_Click(object sender, RoutedEventArgs e)
    {
      wSchedaBilancio w = new wSchedaBilancio();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Delete;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                           menuBilancioVedi_Click                           |
    //----------------------------------------------------------------------------+
    private void menuBilancioVedi_Click(object sender, RoutedEventArgs e)
    {
      wSchedaBilancio w = new wSchedaBilancio();
      w.TipologiaAttivita = App.TipoAttivitaScheda.View;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    #endregion //---------------------------------------------------------- BILANCI

    #region CONCLUSIONI

    //Gestione menù: BILANCI ********************************************

    //----------------------------------------------------------------------------+
    //                         menuConclusioneNuova_Click                         |
    //----------------------------------------------------------------------------+
    private void menuConclusioneNuova_Click(object sender, RoutedEventArgs e)
    {
      wSchedaConclusioni w = new wSchedaConclusioni();
      w.TipologiaAttivita = App.TipoAttivitaScheda.New;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                       menuConclusioneModifica_Click                        |
    //----------------------------------------------------------------------------+
    private void menuConclusioneModifica_Click(object sender, RoutedEventArgs e)
    {
      wSchedaConclusioni w = new wSchedaConclusioni();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Edit;
      w.ConfiguraMaschera();
      w.Owner = this;
      //Hide();
      w.ShowDialog();
      Show();
    }

    //----------------------------------------------------------------------------+
    //                        menuConclusioneElimina_Click                        |
    //----------------------------------------------------------------------------+
    private void menuConclusioneElimina_Click(object sender, RoutedEventArgs e)
    {
      wSchedaConclusioni w = new wSchedaConclusioni();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Delete;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                         menuConclusioneVedi_Click                          |
    //----------------------------------------------------------------------------+
    private void menuConclusioneVedi_Click(object sender, RoutedEventArgs e)
    {
      wSchedaConclusioni w = new wSchedaConclusioni();
      w.TipologiaAttivita = App.TipoAttivitaScheda.View;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    #endregion //------------------------------------------------------ CONCLUSIONI

    #region RELAZIONI

    //Gestione menù: RelazioneB ************************************************

    //----------------------------------------------------------------------------+
    //                         menuRelazioneBNuova_Click                          |
    //----------------------------------------------------------------------------+
    private void menuRelazioneBNuova_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneB w = new wSchedaRelazioneB();
      w.TipologiaAttivita = App.TipoAttivitaScheda.New;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                        menuRelazioneBModifica_Click                        |
    //----------------------------------------------------------------------------+
    private void menuRelazioneBModifica_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneB w = new wSchedaRelazioneB();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Edit;
      w.ConfiguraMaschera();
      w.Owner = this;
      //Hide();
      w.ShowDialog();
      Show();
    }

    //----------------------------------------------------------------------------+
    //                        menuRelazioneBElimina_Click                         |
    //----------------------------------------------------------------------------+
    private void menuRelazioneBElimina_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneB w = new wSchedaRelazioneB();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Delete;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                          menuRelazioneBVedi_Click                          |
    //----------------------------------------------------------------------------+
    private void menuRelazioneBVedi_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneB w = new wSchedaRelazioneB();
      w.TipologiaAttivita = App.TipoAttivitaScheda.View;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //Gestione menù: RelazioneBC ************************************************

    //----------------------------------------------------------------------------+
    //                         menuRelazioneBCNuova_Click                         |
    //----------------------------------------------------------------------------+
    private void menuRelazioneBCNuova_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneBC w = new wSchedaRelazioneBC();
      w.TipologiaAttivita = App.TipoAttivitaScheda.New;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                       menuRelazioneBCModifica_Click                        |
    //----------------------------------------------------------------------------+
    private void menuRelazioneBCModifica_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneBC w = new wSchedaRelazioneBC();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Edit;
      w.ConfiguraMaschera();
      w.Owner = this;
      //Hide();
      w.ShowDialog();
      Show();
    }

    //----------------------------------------------------------------------------+
    //                        menuRelazioneBCElimina_Click                        |
    //----------------------------------------------------------------------------+
    private void menuRelazioneBCElimina_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneBC w = new wSchedaRelazioneBC();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Delete;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                         menuRelazioneBCVedi_Click                          |
    //----------------------------------------------------------------------------+
    private void menuRelazioneBCVedi_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneBC w = new wSchedaRelazioneBC();
      w.TipologiaAttivita = App.TipoAttivitaScheda.View;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //Gestione menù: RelazioneV ************************************************

    //----------------------------------------------------------------------------+
    //                         menuRelazioneVNuova_Click                          |
    //----------------------------------------------------------------------------+
    private void menuRelazioneVNuova_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneV w = new wSchedaRelazioneV();
      w.TipologiaAttivita = App.TipoAttivitaScheda.New;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                        menuRelazioneVModifica_Click                        |
    //----------------------------------------------------------------------------+
    private void menuRelazioneVModifica_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneV w = new wSchedaRelazioneV();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Edit;
      w.ConfiguraMaschera();
      w.Owner = this;
      //Hide();
      w.ShowDialog();
      Show();
    }

    //----------------------------------------------------------------------------+
    //                        menuRelazioneVElimina_Click                         |
    //----------------------------------------------------------------------------+
    private void menuRelazioneVElimina_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneV w = new wSchedaRelazioneV();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Delete;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                          menuRelazioneVVedi_Click                          |
    //----------------------------------------------------------------------------+
    private void menuRelazioneVVedi_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneV w = new wSchedaRelazioneV();
      w.TipologiaAttivita = App.TipoAttivitaScheda.View;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //Gestione menù: RelazioniVC ************************************************

    //----------------------------------------------------------------------------+
    //                         menuRelazioneVCNuova_Click                         |
    //----------------------------------------------------------------------------+
    private void menuRelazioneVCNuova_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneVC w = new wSchedaRelazioneVC();
      w.TipologiaAttivita = App.TipoAttivitaScheda.New;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                       menuRelazioneVCModifica_Click                        |
    //----------------------------------------------------------------------------+
    private void menuRelazioneVCModifica_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneVC w = new wSchedaRelazioneVC();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Edit;
      w.ConfiguraMaschera();
      w.Owner = this;
      //Hide();
      w.ShowDialog();
      Show();
    }

    //----------------------------------------------------------------------------+
    //                        menuRelazioneVCElimina_Click                        |
    //----------------------------------------------------------------------------+
    private void menuRelazioneVCElimina_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneVC w = new wSchedaRelazioneVC();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Delete;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                         menuRelazioneVCVedi_Click                          |
    //----------------------------------------------------------------------------+
    private void menuRelazioneVCVedi_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneVC w = new wSchedaRelazioneVC();
      w.TipologiaAttivita = App.TipoAttivitaScheda.View;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //Gestione menù: RelazioneBV ************************************************

    //----------------------------------------------------------------------------+
    //                         menuRelazioneBVNuova_Click                         |
    //----------------------------------------------------------------------------+
    private void menuRelazioneBVNuova_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneBV w = new wSchedaRelazioneBV();
      w.TipologiaAttivita = App.TipoAttivitaScheda.New;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                       menuRelazioneBVModifica_Click                        |
    //----------------------------------------------------------------------------+
    private void menuRelazioneBVModifica_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneBV w = new wSchedaRelazioneBV();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Edit;
      w.ConfiguraMaschera();
      w.Owner = this;
      //Hide();
      w.ShowDialog();
      Show();
    }

    //----------------------------------------------------------------------------+
    //                        menuRelazioneBVElimina_Click                        |
    //----------------------------------------------------------------------------+
    private void menuRelazioneBVElimina_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneBV w = new wSchedaRelazioneBV();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Delete;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                         menuRelazioneBVVedi_Click                          |
    //----------------------------------------------------------------------------+
    private void menuRelazioneBVVedi_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRelazioneBV w = new wSchedaRelazioneBV();
      w.TipologiaAttivita = App.TipoAttivitaScheda.View;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    #endregion //-------------------------------------------------------- RELAZIONI

    #region VERIFICHE

    //Gestione menù: VERIFICHE ********************************************

    //----------------------------------------------------------------------------+
    //                          menuVerificheNuova_Click                          |
    //----------------------------------------------------------------------------+
    private void menuVerificheNuova_Click(object sender, RoutedEventArgs e)
    {
      wSchedaVerifica w = new wSchedaVerifica();
      w.TipologiaAttivita = App.TipoAttivitaScheda.New;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                        menuVerificheModifica_Click                         |
    //----------------------------------------------------------------------------+
    private void menuVerificheModifica_Click(object sender, RoutedEventArgs e)
    {
      wSchedaVerifica w = new wSchedaVerifica();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Edit;
      w.ConfiguraMaschera();
      w.Owner = this;
      //Hide();
      w.ShowDialog();
      Show();
    }

    //----------------------------------------------------------------------------+
    //                         menuVerificheElimina_Click                         |
    //----------------------------------------------------------------------------+
    private void menuVerificheElimina_Click(object sender, RoutedEventArgs e)
    {
      wSchedaVerifica w = new wSchedaVerifica();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Delete;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                          menuVerificheVedi_Click                           |
    //----------------------------------------------------------------------------+
    private void menuVerificheVedi_Click(object sender, RoutedEventArgs e)
    {
      wSchedaVerifica w = new wSchedaVerifica();
      w.TipologiaAttivita = App.TipoAttivitaScheda.View;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    #endregion //-------------------------------------------------------- VERIFICHE

    #region VIGILANZA

    //Gestione menù: VERIFICHE ********************************************

    //----------------------------------------------------------------------------+
    //                          menuVigilanzaNuova_Click                          |
    //----------------------------------------------------------------------------+
    private void menuVigilanzaNuova_Click(object sender, RoutedEventArgs e)
    {
      wSchedaVigilanza w = new wSchedaVigilanza();
      w.TipologiaAttivita = App.TipoAttivitaScheda.New;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                        menuVigilanzaModifica_Click                         |
    //----------------------------------------------------------------------------+
    private void menuVigilanzaModifica_Click(object sender, RoutedEventArgs e)
    {
      wSchedaVigilanza w = new wSchedaVigilanza();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Edit;
      w.ConfiguraMaschera();
      w.Owner = this;
      //Hide();
      w.ShowDialog();
      Show();
    }

    //----------------------------------------------------------------------------+
    //                         menuVigilanzaElimina_Click                         |
    //----------------------------------------------------------------------------+
    private void menuVigilanzaElimina_Click(object sender, RoutedEventArgs e)
    {
      wSchedaVigilanza w = new wSchedaVigilanza();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Delete;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                          menuVigilanzaVedi_Click                           |
    //----------------------------------------------------------------------------+
    private void menuVigilanzaVedi_Click(object sender, RoutedEventArgs e)
    {
      wSchedaVigilanza w = new wSchedaVigilanza();
      w.TipologiaAttivita = App.TipoAttivitaScheda.View;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    #endregion //-------------------------------------------------------- VIGILANZA

    #region PIANIFICAZIONE VERIFICHE

    //Gestione menù: VERIFICHE ********************************************

    //----------------------------------------------------------------------------+
    //                   menuPianificazioniVerificheNuova_Click                   |
    //----------------------------------------------------------------------------+
    private void menuPianificazioniVerificheNuova_Click(object sender, RoutedEventArgs e)
    {
      wSchedaPianificazioniVerifica w = new wSchedaPianificazioniVerifica();
      w.TipologiaAttivita = App.TipoAttivitaScheda.New;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                 menuPianificazioniVerificheModifica_Click                  |
    //----------------------------------------------------------------------------+
    private void menuPianificazioniVerificheModifica_Click(object sender, RoutedEventArgs e)
    {
      wSchedaPianificazioniVerifica w = new wSchedaPianificazioniVerifica();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Edit;
      w.ConfiguraMaschera();
      w.Owner = this;
      //Hide();
      w.ShowDialog();
      Show();
    }

    //----------------------------------------------------------------------------+
    //                  menuPianificazioniVerificheElimina_Click                  |
    //----------------------------------------------------------------------------+
    private void menuPianificazioniVerificheElimina_Click(object sender, RoutedEventArgs e)
    {
      wSchedaPianificazioniVerifica w = new wSchedaPianificazioniVerifica();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Delete;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                   menuPianificazioniVerificheVedi_Click                    |
    //----------------------------------------------------------------------------+
    private void menuPianificazioniVerificheVedi_Click(object sender, RoutedEventArgs e)
    {
      wSchedaPianificazioniVerifica w = new wSchedaPianificazioniVerifica();
      w.TipologiaAttivita = App.TipoAttivitaScheda.View;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    #endregion //----------------------------------------- PIANIFICAZIONE VERIFICHE

    #region PIANIFICAZIONE VIGILANZA

    //Gestione menù: VERIFICHE ********************************************

    //----------------------------------------------------------------------------+
    //                   menuPianificazioniVigilanzaNuova_Click                   |
    //----------------------------------------------------------------------------+
    private void menuPianificazioniVigilanzaNuova_Click(object sender, RoutedEventArgs e)
    {
      wSchedaPianificazioniVigilanza w = new wSchedaPianificazioniVigilanza();
      w.TipologiaAttivita = App.TipoAttivitaScheda.New;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                 menuPianificazioniVigilanzaModifica_Click                  |
    //----------------------------------------------------------------------------+
    private void menuPianificazioniVigilanzaModifica_Click(object sender, RoutedEventArgs e)
    {
      wSchedaPianificazioniVigilanza w = new wSchedaPianificazioniVigilanza();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Edit;
      w.ConfiguraMaschera();
      w.Owner = this;
      //Hide();
      w.ShowDialog();
      Show();
    }

    //----------------------------------------------------------------------------+
    //                  menuPianificazioniVigilanzaElimina_Click                  |
    //----------------------------------------------------------------------------+
    private void menuPianificazioniVigilanzaElimina_Click(object sender, RoutedEventArgs e)
    {
      wSchedaPianificazioniVigilanza w = new wSchedaPianificazioniVigilanza();
      w.TipologiaAttivita = App.TipoAttivitaScheda.Delete;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                   menuPianificazioniVigilanzaVedi_Click                    |
    //----------------------------------------------------------------------------+
    private void menuPianificazioniVigilanzaVedi_Click(object sender, RoutedEventArgs e)
    {
      wSchedaPianificazioniVigilanza w = new wSchedaPianificazioniVigilanza();
      w.TipologiaAttivita = App.TipoAttivitaScheda.View;
      w.ConfiguraMaschera();
      w.Owner = this;
      w.ShowDialog();
    }

    #endregion //----------------------------------------- PIANIFICAZIONE VIGILANZA

    #region STRUMENTI

    //STRUMENTI: gestione backup
    private void menuStrumentiGestioneSalvataggi_Click(object sender, RoutedEventArgs e)
    {
      wGestioneBackUp w = new wGestioneBackUp();
      w.Owner = this;
      w.ShowDialog();
    }
    //STRUMENTI: gestione archivio
    private void menuStrumentiGestioneArchivio_Click(object sender, RoutedEventArgs e)
    {
      wGestioneArchivio w = new wGestioneArchivio();
      w.Owner = this;
      w.ShowDialog();
    }
    //STRUMENTI: dati licenza
    private void menuStrumentiDatiLicenza_Click(object sender, RoutedEventArgs e)
    {
      //Apertura maschera
      WindowGestioneLicenzaUtente w = new WindowGestioneLicenzaUtente();
      w.Owner = this;
      w.ShowDialog();
    }
    //STRUMENTI: gestione licenza
    private void menuStrumentiGestioneLicenza_Click(object sender, RoutedEventArgs e)
    {
      //Apertura maschera
      WindowGestioneLicenza w1 = new WindowGestioneLicenza();
      w1.Owner = this;
      w1.ShowDialog();
      //controllo licenza
      RevisoftApplication.GestioneLicenza l = new GestioneLicenza();
      //if (l.StatoLicenzaCambiato)
      StatusBarDatiUtente(l);
    }
    //STRUMENTI: configurazione
    private void menuStrumentiConfigurazione_Click(object sender, RoutedEventArgs e)
    {
      //Apertura maschera
      wConfigurazione w = new wConfigurazione();
      w.Owner = this;
      w.ShowDialog();
    }
    //STRUMENTI: Stampa Fascicolo
    private void menuStrumentiStampaFascicolo_Click(object sender, RoutedEventArgs e)
    {
      wStampaFascicolo wSF = new wStampaFascicolo();
      wSF.Owner = this;
      wSF.ShowDialog();
    }
    //STRUMENTI: Stampa Fascicolo
    private void menuStrumentiStampaVerbali_Click(object sender, RoutedEventArgs e)
    {
      wStampaVerbali wSF = new wStampaVerbali();
      wSF.inizializza();
      wSF.Owner = this;
      wSF.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                       menuStrumentiFormulario_Click                        |
    //----------------------------------------------------------------------------+
    private void menuStrumentiFormulario_Click(object sender, RoutedEventArgs e)
    {
      Formulario formulario = new Formulario();
      formulario.Owner = this;
      formulario.LoadTreeSource();
      formulario.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                         menuStrumentiSOSPESI_Click                         |
    //----------------------------------------------------------------------------+
    private void menuStrumentiSOSPESI_Click(object sender, RoutedEventArgs e)
    {
      wSchedaSOSPESI STDL = new wSchedaSOSPESI();
      STDL.Owner = this;
      STDL.ShowDialog();
      STDL.Activate();
    }

    //----------------------------------------------------------------------------+
    //                        menuStrumentiTODOList_Click                         |
    //----------------------------------------------------------------------------+
    private void menuStrumentiTODOList_Click(object sender, RoutedEventArgs e)
    {
      wSchedaTODOList STDL = new wSchedaTODOList();
      STDL.Owner = this;
      STDL.ShowDialog();
      STDL.Activate();
    }

    //----------------------------------------------------------------------------+
    //                    menuStrumentiEstraiDaRevisione_Click                    |
    //----------------------------------------------------------------------------+
    private void menuStrumentiEstraiDaRevisione_Click(object sender, RoutedEventArgs e)
    {
      wSchedaEstrapolazioneAllegatiRevisione STDL = new wSchedaEstrapolazioneAllegatiRevisione();
      STDL.Owner = this;
      STDL.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                    menuStrumentiEstraiDaVerifiche_Click                    |
    //----------------------------------------------------------------------------+
    private void menuStrumentiEstraiDaVerifiche_Click(object sender, RoutedEventArgs e)
    {
      wSchedaEstrapolazioneAllegatiVerifiche STDL = new wSchedaEstrapolazioneAllegatiVerifiche();
      STDL.Owner = this;
      STDL.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                        menuStrumentiPermanent_Click                        |
    //----------------------------------------------------------------------------+
    private void menuStrumentiPermanent_Click(object sender, RoutedEventArgs e)
    {
      wSchedaPermanent STDL = new wSchedaPermanent();
      STDL.Owner = this;
      STDL.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                        menuStrumentiDocumenti_Click                        |
    //----------------------------------------------------------------------------+
    private void menuStrumentiDocumenti_Click(object sender, RoutedEventArgs e)
    {
      wSchedaDocumenti STDL = new wSchedaDocumenti();
      STDL.Owner = this;
      STDL.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                     menuStrumentiResetMasterFile_Click                     |
    //----------------------------------------------------------------------------+
    private void menuStrumentiResetMasterFile_Click(object sender, RoutedEventArgs e)
    {
      //richiesta conferma
      Utilities u = new Utilities();
      if (MessageBoxResult.Yes == u.ConfermaResetArchivio())
      {
        MasterFile mf = MasterFile.Create();
        mf.ResetMasterFile();
        ReloadMainWindow();
        MessageBox.Show("Reset Avvenuto con successo");
      }
    }

    //----------------------------------------------------------------------------+
    //                      menuClientiCondividiScelta_Click                      |
    //----------------------------------------------------------------------------+
    private void menuClientiCondividiScelta_Click(object sender, RoutedEventArgs e)
    {
      wCondividiCliente wSF = new wCondividiCliente();
      wSF.Owner = this;
      wSF.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                         menuStrumentiSigillo_Click                         |
    //----------------------------------------------------------------------------+
    private void menuStrumentiSigillo_Click(object sender, RoutedEventArgs e)
    {
      //verifico licenza sigillo
      RevisoftApplication.GestioneLicenza l = new GestioneLicenza();
      //licenza disponibile
      if (App.Sigillo == true)// l.StatoLicenzaSigillo)
      {
        wSigillo wSF = new wSigillo();
        wSF.Owner = this;
        wSF.ShowDialog();
      }
      //licenza NON disponibile
      else
      {
        App.ErrorLevel = App.ErrorTypes.Avviso;
        RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
        m.TipoMessaggioAvviso = WindowGestioneMessaggi.TipologieMessaggiAvvisi.LicenzaSigilloNonDisponibile;
        m.VisualizzaMessaggio();
      }
    }

    //----------------------------------------------------------------------------+
    //                     menuStrumentiAssegnaIncarico_Click                     |
    //----------------------------------------------------------------------------+
    private void menuStrumentiAssegnaIncarico_Click(object sender, RoutedEventArgs e)
    {
      var wai = new wWorkAreaTree_AssegnazioneIncarichi();
      wai.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                      menuStrumentiMultilicenza_Click                       |
    //----------------------------------------------------------------------------+
    private void menuStrumentiMultilicenza_Click(object sender, RoutedEventArgs e)
    {
      //verifico licenza multipla
      RevisoftApplication.GestioneLicenza l = new GestioneLicenza();
      //licenza disponibile
      if (l.StatoLicenzaMultipla)
      {
        wMultiLicenza STDL = new wMultiLicenza();
        STDL.Owner = this;
        STDL.ShowDialog();
      }
      //licenza NON disponibile
      else
      {
        App.ErrorLevel = App.ErrorTypes.Avviso;
        RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
        m.TipoMessaggioAvviso = WindowGestioneMessaggi.TipologieMessaggiAvvisi.LicenzaMultiplaNonDisponibile;
        m.VisualizzaMessaggio();
      }
    }

    #endregion //-------------------------------------------------------- STRUMENTI

    #region INFO

    //Gestione menù: Doc Revisione
    private void menuInfoDocRevisione_Click(object sender, RoutedEventArgs e)
    {
      System.Diagnostics.Process.Start(RevisoftApplication.Properties.Settings.Default["RevisoftDocRevisione"].ToString());
    }

    //Gestione menù: Doc Vigilanza
    private void menuInfoDocVigilanza_Click(object sender, RoutedEventArgs e)
    {
      System.Diagnostics.Process.Start(RevisoftApplication.Properties.Settings.Default["RevisoftDocVigilanza"].ToString());
    }

    //Gestione menù: Note di Rilascio
    private void menuInfoNoteRilascio_Click(object sender, RoutedEventArgs e)
    {
      System.Diagnostics.Process.Start(App.urlNoteRilascio);
    }

    //Gestione menù: AIUTO
    private void menuInfoWeb_Click(object sender, RoutedEventArgs e)
    {
      System.Diagnostics.Process.Start(RevisoftApplication.Properties.Settings.Default["RevisoftWebSite"].ToString());
    }

    //Gestione menù: AIUTO
    private void menuProceduraRevisioneWeb_Click(object sender, RoutedEventArgs e)
    {
      System.Diagnostics.Process.Start(RevisoftApplication.Properties.Settings.Default["RevisoftWebProceduraRevisione"].ToString());
    }

    //----------------------------------------------------------------------------+
    //                            menuInfoGuida_Click                             |
    //----------------------------------------------------------------------------+
    private void menuInfoGuida_Click(object sender, RoutedEventArgs e)
    {
      //file
      System.Diagnostics.Process.Start(App.AppHelpFile);
      //web
      //System.Diagnostics.Process.Start(RevisoftApplication.Properties.Settings.Default["RevisoftApplicationGuide"].ToString());
    }

    //----------------------------------------------------------------------------+
    //                            menuInfoAbout_Click                             |
    //----------------------------------------------------------------------------+
    private void menuInfoAbout_Click(object sender, RoutedEventArgs e)
    {
      WindowAbout w1 = new WindowAbout();
      w1.Owner = this;
      w1.ShowDialog();
    }

    #endregion //------------------------------------------------------------- INFO

    #endregion //------------------------------------------------------------- MENU

    #region TOOLBAR

    //----------------------------------------------------------------------------+
    //                    ConfiguraInterfacciaClientiPerStato                     |
    //----------------------------------------------------------------------------+
    private void ConfiguraInterfacciaClientiPerStato()
    {
      //resetto toolbar
      if (IndexSelected == -1)
      {
        btn_Cliente_Importa.Visibility = System.Windows.Visibility.Visible;
        if (App.AppTipo == App.ModalitaApp.Team)
        {
          //  se TEAM, il revsore e l'esecutore non possono fare niente che modifiche i clienti, il team leade può modificare il cliente
          if (App.AppRuolo == App.RuoloDesc.Esecutore || App.AppRuolo == App.RuoloDesc.Reviewer || App.AppRuolo == App.RuoloDesc.TeamLeader)
          {
            btn_Cliente_Nuovo.Visibility = System.Windows.Visibility.Collapsed;
            //btn_Cliente_Vedi.Visibility = System.Windows.Visibility.Collapsed;
            btn_Cliente_Elimina.Visibility = System.Windows.Visibility.Collapsed;
            btn_Cliente_Esporta.Visibility = System.Windows.Visibility.Collapsed;
            //btn_Cliente_Condividi.Visibility = System.Windows.Visibility.Collapsed;
            btn_Cliente_Importa.Visibility = System.Windows.Visibility.Collapsed;

            if (App.AppRuolo == App.RuoloDesc.Esecutore || App.AppRuolo == App.RuoloDesc.Reviewer)
              btn_Cliente_Modifica.Visibility = System.Windows.Visibility.Collapsed;
            return;
          }
        }
        else if (App.AppTipo == App.ModalitaApp.StandAlone && App.AppUtente.RuoId == (int)App.RuoloDesc.RevisoreAutonomo)
        {
          btn_Cliente_Nuovo.Visibility = Visibility.Collapsed;
          btn_Cliente_Modifica.Visibility = Visibility.Collapsed;
          btn_Cliente_Elimina.Visibility = Visibility.Collapsed;
          return;
        }
        //menu
        //versione 3.0
        //menuClienteNuovo.Visibility = System.Windows.Visibility.Visible;
        //menuClienteModifica.Visibility = System.Windows.Visibility.Collapsed;
        //menuClienteVedi.Visibility = System.Windows.Visibility.Collapsed;
        //menuClienteElimina.Visibility = System.Windows.Visibility.Collapsed;
        //menuClientiEsporta.Visibility = System.Windows.Visibility.Collapsed;
        //menuClientiImporta.Visibility = System.Windows.Visibility.Visible;
        //toolbar
        btn_Cliente_Nuovo.Visibility = System.Windows.Visibility.Visible;
        btn_Cliente_Modifica.Visibility = System.Windows.Visibility.Collapsed;
        btn_Cliente_Vedi.Visibility = System.Windows.Visibility.Collapsed;
        btn_Cliente_Elimina.Visibility = System.Windows.Visibility.Collapsed;
        btn_Cliente_Esporta.Visibility = System.Windows.Visibility.Collapsed;
        //btn_Cliente_Condividi.Visibility = System.Windows.Visibility.Collapsed;
        btn_Cliente_Importa.Visibility = System.Windows.Visibility.Visible;

        ////licenza satellite e guest, no creazione clienti
        //if (App.TipoLicenza == App.TipologieLicenze.Viewer || App.TipoLicenza == App.TipologieLicenze.Guest)
        //{
        //    //menuClienteNuovo.Visibility = System.Windows.Visibility.Collapsed;
        //    btn_Cliente_Nuovo.Visibility = System.Windows.Visibility.Collapsed;
        //    //menuClientiImporta.Visibility = System.Windows.Visibility.Collapsed;
        //    btn_Cliente_Importa.Visibility = System.Windows.Visibility.Collapsed;
        //}

        //licenza entry level e prova con numero anagrafiche limitato
        //if ((App.TipoLicenza == App.TipologieLicenze.EntryLevel || App.TipoLicenza == App.TipologieLicenze.Prova) && counterClienti >= GestioneLicenza.TOT_ANAGRAFICHE_LICENZA_ENTRY)
        if (counterClienti >= App.NumeroanAgrafiche)
        {
          //menuClienteNuovo.Visibility = System.Windows.Visibility.Collapsed;
          btn_Cliente_Nuovo.Visibility = System.Windows.Visibility.Collapsed;
          //menuClientiImporta.Visibility = System.Windows.Visibility.Collapsed;
          btn_Cliente_Importa.Visibility = System.Windows.Visibility.Collapsed;
        }

        ////licenza client lan con numero anagrafiche limitato in modalità satellite
        //if (App.TipoLicenza == App.TipologieLicenze.ClientLan && App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.LocaleImportExport)
        //{
        //    //menuClienteNuovo.Visibility = System.Windows.Visibility.Collapsed;
        //    btn_Cliente_Nuovo.Visibility = System.Windows.Visibility.Collapsed;
        //    //menuClientiImporta.Visibility = System.Windows.Visibility.Collapsed;
        //    btn_Cliente_Importa.Visibility = System.Windows.Visibility.Collapsed;
        //}

        return;
      }

      btn_Cliente_Importa.Visibility = System.Windows.Visibility.Visible;
      if (App.AppTipo == App.ModalitaApp.Team)
      {
        if (App.AppRuolo == App.RuoloDesc.Esecutore || App.AppRuolo == App.RuoloDesc.Reviewer || App.AppRuolo == App.RuoloDesc.TeamLeader)
        {
          btn_Cliente_Nuovo.Visibility = System.Windows.Visibility.Collapsed;

          btn_Cliente_Vedi.Visibility = System.Windows.Visibility.Visible;
          btn_Cliente_Elimina.Visibility = System.Windows.Visibility.Collapsed;
          btn_Cliente_Esporta.Visibility = System.Windows.Visibility.Collapsed;
          //btn_Cliente_Condividi.Visibility = System.Windows.Visibility.Collapsed;
          btn_Cliente_Importa.Visibility = System.Windows.Visibility.Collapsed;
          if (App.AppRuolo == App.RuoloDesc.Esecutore || App.AppRuolo == App.RuoloDesc.Reviewer)
            btn_Cliente_Modifica.Visibility = System.Windows.Visibility.Collapsed;
          return;
        }

      }
      else if (App.AppTipo == App.ModalitaApp.StandAlone && App.AppUtente.RuoId == (int)App.RuoloDesc.RevisoreAutonomo)
      {
        btn_Cliente_Nuovo.Visibility = Visibility.Collapsed;
        btn_Cliente_Modifica.Visibility = Visibility.Collapsed;
        btn_Cliente_Elimina.Visibility = Visibility.Collapsed;
        return;
      }

      //configuro toolbar
      switch ((App.TipoAnagraficaStato)(Convert.ToInt32(htClienti_Stato[IndexSelected])))
      {
        case App.TipoAnagraficaStato.Disponibile:
          //menu
          //menuClienteNuovo.Visibility = System.Windows.Visibility.Visible;
          //menuClienteModifica.Visibility = System.Windows.Visibility.Visible;
          //menuClienteVedi.Visibility = System.Windows.Visibility.Visible;
          //menuClienteElimina.Visibility = System.Windows.Visibility.Visible;
          //menuClientiEsporta.Visibility = System.Windows.Visibility.Visible;
          //menuClientiImporta.Visibility = System.Windows.Visibility.Collapsed;
          //toolbar
          btn_Cliente_Nuovo.Visibility = System.Windows.Visibility.Visible;
          btn_Cliente_Modifica.Visibility = System.Windows.Visibility.Visible;
          btn_Cliente_Vedi.Visibility = System.Windows.Visibility.Visible;
          btn_Cliente_Elimina.Visibility = System.Windows.Visibility.Visible;
          btn_Cliente_Esporta.Visibility = System.Windows.Visibility.Visible;
          //btn_Cliente_Condividi.Visibility = System.Windows.Visibility.Visible;
          btn_Cliente_Importa.Visibility = System.Windows.Visibility.Collapsed;
          break;
        case App.TipoAnagraficaStato.Esportato:
          //menu
          //menuClienteNuovo.Visibility = System.Windows.Visibility.Visible;
          //menuClienteModifica.Visibility = System.Windows.Visibility.Collapsed;
          //menuClienteVedi.Visibility = System.Windows.Visibility.Visible;
          //menuClienteElimina.Visibility = System.Windows.Visibility.Collapsed;
          //menuClientiEsporta.Visibility = System.Windows.Visibility.Collapsed;
          //menuClientiImporta.Visibility = System.Windows.Visibility.Visible;
          //toolbar
          btn_Cliente_Nuovo.Visibility = System.Windows.Visibility.Visible;
          btn_Cliente_Modifica.Visibility = System.Windows.Visibility.Collapsed;
          btn_Cliente_Vedi.Visibility = System.Windows.Visibility.Visible;
          btn_Cliente_Elimina.Visibility = System.Windows.Visibility.Visible;
          btn_Cliente_Esporta.Visibility = System.Windows.Visibility.Collapsed;
          //btn_Cliente_Condividi.Visibility = System.Windows.Visibility.Visible;
          btn_Cliente_Importa.Visibility = System.Windows.Visibility.Visible;
          break;
        case App.TipoAnagraficaStato.InUso:
        case App.TipoAnagraficaStato.Sconosciuto:
        case App.TipoAnagraficaStato.Bloccato:
        default:
          //menu
          //menuClienteNuovo.Visibility = System.Windows.Visibility.Visible;
          //menuClienteModifica.Visibility = System.Windows.Visibility.Collapsed;
          //menuClienteVedi.Visibility = System.Windows.Visibility.Visible;
          //menuClienteElimina.Visibility = System.Windows.Visibility.Collapsed;
          //menuClientiEsporta.Visibility = System.Windows.Visibility.Collapsed;
          //menuClientiImporta.Visibility = System.Windows.Visibility.Collapsed;
          //toolbar
          btn_Cliente_Nuovo.Visibility = System.Windows.Visibility.Visible;
          btn_Cliente_Modifica.Visibility = System.Windows.Visibility.Collapsed;
          btn_Cliente_Vedi.Visibility = System.Windows.Visibility.Visible;
          btn_Cliente_Elimina.Visibility = System.Windows.Visibility.Collapsed;
          btn_Cliente_Esporta.Visibility = System.Windows.Visibility.Collapsed;
          //btn_Cliente_Condividi.Visibility = System.Windows.Visibility.Collapsed;
          btn_Cliente_Importa.Visibility = System.Windows.Visibility.Collapsed;
          break;
      }

      //licenza entry level e prova con numero anagrafiche limitato
      //if ((App.TipoLicenza == App.TipologieLicenze.EntryLevel || App.TipoLicenza == App.TipologieLicenze.Prova || App.TipoLicenza == App.TipologieLicenze.Viewer) && counterClienti >= GestioneLicenza.TOT_ANAGRAFICHE_LICENZA_ENTRY)
      if (counterClienti >= App.NumeroanAgrafiche)
      {
        //menuClienteNuovo.Visibility = System.Windows.Visibility.Collapsed;
        btn_Cliente_Nuovo.Visibility = System.Windows.Visibility.Collapsed;
        //menuClientiImporta.Visibility = System.Windows.Visibility.Collapsed;
        btn_Cliente_Importa.Visibility = System.Windows.Visibility.Collapsed;
      }

      //licenza guest 
      if (App.Guest == true)//App.TipoLicenza == App.TipologieLicenze.Guest)
      {
        //limitazione al numero anagrafiche 
        if (counterClienti >= GestioneLicenza.TOT_ANAGRAFICHE_LICENZA_GUEST)
        {
          //menuClienteNuovo.Visibility = System.Windows.Visibility.Collapsed;
          btn_Cliente_Nuovo.Visibility = System.Windows.Visibility.Collapsed;
          //menuClientiImporta.Visibility = System.Windows.Visibility.Collapsed;
          btn_Cliente_Importa.Visibility = System.Windows.Visibility.Collapsed;
        }
        //limitazione funzionalità
        //menuClienteElimina.Visibility = System.Windows.Visibility.Collapsed;
        btn_Cliente_Elimina.Visibility = System.Windows.Visibility.Collapsed;
      }

      //licenza client lan con numero anagrafiche limitato in modalità satellite
      //if (App.TipoLicenza == App.TipologieLicenze.ClientLan && App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.LocaleImportExport)
      //{
      //    //menuClienteNuovo.Visibility = System.Windows.Visibility.Collapsed;
      //    btn_Cliente_Nuovo.Visibility = System.Windows.Visibility.Collapsed;
      //    //menuClientiImporta.Visibility = System.Windows.Visibility.Collapsed;
      //    btn_Cliente_Importa.Visibility = System.Windows.Visibility.Collapsed;
      //}
    }

    #endregion //---------------------------------------------------------- TOOLBAR

    #region EVENTI

    //protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    //{
    //    RevisoftApplication.Utilities u = new Utilities();
    //    if (u.ConfermaUscita())
    //    {
    //        base.OnClosed(e);
    //        u.ChiudiApplicazione();
    //    }
    //    else
    //        e.Cancel = true;
    //}

    //----------------------------------------------------------------------------+
    //                             OnContentRendered                              |
    //----------------------------------------------------------------------------+


    protected override void OnContentRendered(EventArgs e)
    {
      base.OnContentRendered(e);
      ImpostaColori();



      //Benvenuto
      //  if (!App.AppAutoExec) // andrea 4.7 && App.RemoteDesktop == false
      if (false)
      {
        //3.6 messaggio visualizzato
        //&& App.AppSetupBenvenuto
        wBenvenuto w = new wBenvenuto();
        w.Owner = this;
        //w.Height = 515.0;
        //w.Width = 718.0;
        w.ShowDialog();
      }
      //4.6 Aggiornamento templete di rete
      if (!App.AppAutoExec && App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Remoto)
      {
        Utilities u = new Utilities();
        if (!u.VerificaAggiornamentoTemplateRemoto())
        {
          if (MessageBoxResult.Yes == u.ConfermaAggiornamentoModelli())
          {
            if (u.AggiornaTemplateRemoto())
            {
              MessageBox.Show("Aggiornamento archivio avvenuto con successo.");
            }
          }
        }
      }
      //Autoexec
      if (App.AppAutoExec)
      {
        switch (App.AppAutoExecFunzione)
        {
          case App.TipoFunzioniAutoexec.ScambioDati:
            WindowWorkAreaTree_ScambioDati wWorkAreaSD = new WindowWorkAreaTree_ScambioDati();
            wWorkAreaSD.Owner = this;
            wWorkAreaSD.Tipo = App.TipoScambioDati.Importa;
            wWorkAreaSD.ImportFileName = App.AppAutoExecFileName;

            wWorkAreaSD.CaricaInfoFileDaImportare();
            wWorkAreaSD.ShowDialog();
            break;
          case App.TipoFunzioniAutoexec.ImportExport:
            //controllo autorizzazioni / licenze
            //if (App.TipoLicenza == App.TipologieLicenze.Prova)
            if (App.Prova)
            {
              MessageBox.Show("Funzione non disponibile per la licenza Prova.");
              return;
            }
            if (counterClienti >= App.NumeroanAgrafiche)
            //if ((App.TipoLicenza == App.TipologieLicenze.Viewer || App.TipoLicenza == App.TipologieLicenze.EntryLevel) && counterClienti >= GestioneLicenza.TOT_ANAGRAFICHE_LICENZA_ENTRY)
            {
              MessageBox.Show("Si è raggiunto il limite massimo di anagrafiche per il tipo di licenza.\nEliminare un cliente e poi ripetere l'importazione.");
              return;
            }
            //if ((App.TipoLicenza == App.TipologieLicenze.Guest) && counterClienti >= GestioneLicenza.TOT_ANAGRAFICHE_LICENZA_GUEST)
            if ((App.Guest == true) && counterClienti >= GestioneLicenza.TOT_ANAGRAFICHE_LICENZA_GUEST)
            {
              MessageBox.Show("Si è raggiunto il limite massimo di anagrafiche consentito per la licenza GUEST.\nImpossibile importare una nuova anagrafica.");
              return;
            }
            //importo cliente
            Utilities u = new Utilities();
            if (u.ConfermaImportazione() == MessageBoxResult.Yes)
            {

              bool importresult = cImportExport.Import(App.AppAutoExecFileName);

              ReloadMainWindow();
              if (importresult == true)
              {
                MessageBox.Show("Importazione cliente completata.");
              }
            }
            break;
          case App.TipoFunzioniAutoexec.ImportTemplate:
            cImportExport.ImportTemplate(App.AppAutoExecFileName);
            MessageBox.Show("Template importato.");
            break;
          case App.TipoFunzioniAutoexec.Restore:
            //importo cliente
            Utilities u2 = new Utilities();
            if (u2.ConfermaRestore() == MessageBoxResult.Yes)
            {

              //faccio un backup di sistema
              BackUpFile bf = new BackUpFile();
              Hashtable ht = new Hashtable();
              bf.SetBackUp(ht, -1);
              //restore
              bool success = bf.RestoreFile(App.AppAutoExecFileName);

              //Interfaccia
              ReloadMainWindow();
              if (success)
              {
                MessageBox.Show("Ripristino archivio Revisoft avvenuto con successo.");
              }
            }
            break;
          case App.TipoFunzioniAutoexec.NuovoCliente:
            wSchedaAnafrafica w = new wSchedaAnafrafica();
            w.TipologiaAttivita = App.TipoAttivitaScheda.New;
            w.ConfiguraMaschera();
            w.Owner = this;
            w.ShowDialog();
            Expander_Clienti.IsExpanded = true;
            break;
          case App.TipoFunzioniAutoexec.SetupLan:
            RepeatRevisoftInit();
            App.AppAutoExec = false;
            break;
        }
      }
      Activate();
    }

    #endregion //----------------------------------------------------------- EVENTI

    #region EXPANDER

    //----------------------------------------------------------------------------+
    //                         Expander_Clienti_Expanded                          |
    //----------------------------------------------------------------------------+
    private void ExpanderClientiExpanded()
    {
      //Tool bar
      ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
      ButtonBar_Clienti.Visibility = System.Windows.Visibility.Visible;
      ButtonBar_Incarichi.Visibility = System.Windows.Visibility.Collapsed;
      ButtonBar_ISQCs.Visibility = System.Windows.Visibility.Collapsed;
      ButtonBar_Revisioni.Visibility = System.Windows.Visibility.Collapsed;
      ButtonBar_Bilanci.Visibility = System.Windows.Visibility.Collapsed;
      ButtonBar_Conclusioni.Visibility = System.Windows.Visibility.Collapsed;
      ButtonBar_RelazioneB.Visibility = System.Windows.Visibility.Collapsed;
      ButtonBar_RelazioneV.Visibility = System.Windows.Visibility.Collapsed;
      ButtonBar_RelazioneBC.Visibility = System.Windows.Visibility.Collapsed;
      ButtonBar_RelazioneVC.Visibility = System.Windows.Visibility.Collapsed;
      ButtonBar_RelazioneBV.Visibility = System.Windows.Visibility.Collapsed;
      ButtonBar_Verifiche.Visibility = System.Windows.Visibility.Collapsed;
      ButtonBar_Vigilanza.Visibility = System.Windows.Visibility.Collapsed;
      ButtonBar_Strumenti.Visibility = System.Windows.Visibility.Collapsed;
      ButtonBar_Flussi.Visibility = System.Windows.Visibility.Collapsed;
      ButtonBar_PianificazioniVerifiche.Visibility = System.Windows.Visibility.Collapsed;
      ButtonBar_PianificazioniVigilanza.Visibility = System.Windows.Visibility.Collapsed;

      //Expander: chiudo eventuali aperti
      if (Expander_Strumenti.IsExpanded) Expander_Strumenti.IsExpanded = false;
      if (Expander_Revisione.IsExpanded) Expander_Revisione.IsExpanded = false;
      if (Expander_Verifiche.IsExpanded) Expander_Verifiche.IsExpanded = false;
      if (Expander_Archivio.IsExpanded) Expander_Archivio.IsExpanded = false;
      if (Expander_Relazioni.IsExpanded) Expander_Relazioni.IsExpanded = false;
      if (Expander_Accettazione.IsExpanded) Expander_Accettazione.IsExpanded = false;
      if (Expander_Flussi.IsExpanded) Expander_Flussi.IsExpanded = false;
      if (Expander_Configurazione.IsExpanded) Expander_Configurazione.IsExpanded = false;

      //Carico clienti
      CaricaClienti();
      IndexSelected = -1;
      gridSelected = null;
      ConfiguraInterfacciaClientiPerStato();

      //configura status bar 2.8.1
      RevisoftApplication.GestioneLicenza l = new GestioneLicenza();
      StatusBarDatiUtente(l);
    }



    //----------------------------------------------------------------------------+
    //                         Expander_Expanded                         |
    //----------------------------------------------------------------------------+
    private void Expander_Expanded(object sender, RoutedEventArgs e)
    {

      //Tool bar
      Imposta_ExpanderChiusi(sender);
      Imposta_Bar_Visibility(sender);
      ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
      Imposta_Scegli_Esci_Cliente();

    }

    //----------------------------------------------------------------------------+
    //                           Expander_ALL_Collapsed                           |
    //----------------------------------------------------------------------------+
    private void Expander_ALL_Collapsed(object sender, RoutedEventArgs e)
    {
      //tutti gli expander chiuso, apro toolbar home page
      if (!Expander_Clienti.IsExpanded && !Expander_Revisione.IsExpanded && !Expander_Verifiche.IsExpanded && !Expander_Archivio.IsExpanded && !Expander_Strumenti.IsExpanded)
      {
        ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
        ButtonBar_Clienti.Visibility = System.Windows.Visibility.Collapsed;
        ButtonBar_Incarichi.Visibility = System.Windows.Visibility.Collapsed;
        ButtonBar_ISQCs.Visibility = System.Windows.Visibility.Collapsed;
        ButtonBar_Revisioni.Visibility = System.Windows.Visibility.Collapsed;
        ButtonBar_Bilanci.Visibility = System.Windows.Visibility.Collapsed;
        ButtonBar_Conclusioni.Visibility = System.Windows.Visibility.Collapsed;
        ButtonBar_RelazioneB.Visibility = System.Windows.Visibility.Collapsed;
        ButtonBar_RelazioneV.Visibility = System.Windows.Visibility.Collapsed;
        ButtonBar_RelazioneBC.Visibility = System.Windows.Visibility.Collapsed;
        ButtonBar_RelazioneVC.Visibility = System.Windows.Visibility.Collapsed;
        ButtonBar_RelazioneBV.Visibility = System.Windows.Visibility.Collapsed;
        ButtonBar_Verifiche.Visibility = System.Windows.Visibility.Collapsed;
        ButtonBar_Vigilanza.Visibility = System.Windows.Visibility.Collapsed;
        ButtonBar_Strumenti.Visibility = System.Windows.Visibility.Collapsed;
        ButtonBar_Flussi.Visibility = System.Windows.Visibility.Collapsed;
        ButtonBar_PianificazioniVerifiche.Visibility = System.Windows.Visibility.Collapsed;
        ButtonBar_PianificazioniVigilanza.Visibility = System.Windows.Visibility.Collapsed;

        //RadioButton

      }
      Imposta_rd_checked();
      Imposta_Scegli_Esci_Cliente();
    }

    private void Imposta_Scegli_Esci_Cliente()
    {
      if (showallclienti)
      {
        btn_Cliente_Sblocca_Start.Visibility = System.Windows.Visibility.Collapsed;
        btn_Cliente_Fissa_Start.Visibility = System.Windows.Visibility.Visible;
      }
      else
      {
        btn_Cliente_Sblocca_Start.Visibility = System.Windows.Visibility.Visible;
        btn_Cliente_Fissa_Start.Visibility = System.Windows.Visibility.Collapsed;

      }
    }

    #endregion //--------------------------------------------------------- EXPANDER

    #region GESTIONE_CLIENTI

    //----------------------------------------------------------------------------+
    //                               CaricaClienti                                |
    //----------------------------------------------------------------------------+
    public void CaricaClienti()
    {
      //carico dati
      GetClienti();
      counterClienti = 0;
      stpClienti_ID.Children.Clear();
      stpClienti_RagioneSociale.Children.Clear();
      stpClienti_CodiceFiscale.Children.Clear();
      stpClienti_PIVA.Children.Clear();
      stpClienti_Esercizio.Children.Clear();

      for (counterClienti = 0; counterClienti < htClienti_RS.Count; counterClienti++)
      {
        ColonnaTesto(stpClienti_ID, counterClienti, htClienti_ID[counterClienti].ToString());
        ColonnaTesto(stpClienti_RagioneSociale, counterClienti, htClienti_RS[counterClienti].ToString());
        ColonnaTesto(stpClienti_CodiceFiscale, counterClienti, htClienti_CF[counterClienti].ToString());
        ColonnaTesto(stpClienti_PIVA, counterClienti, htClienti_PIVA[counterClienti].ToString());
        ColonnaTesto(stpClienti_Esercizio, counterClienti, htClienti_Ese[counterClienti].ToString());

      }
      //andrea
      if (counterClienti == 0) IndexSelected = -1;
      //Configura interfaccia x licenza e numero anagrafiche
      //if (App.TipoLicenza == App.TipologieLicenze.EntryLevel || App.TipoLicenza == App.TipologieLicenze.Prova || App.TipoLicenza == App.TipologieLicenze.Viewer || App.TipoLicenza == App.TipologieLicenze.Guest) 
      {
        App.AppConsentiCreazioneAnagrafica = counterClienti < App.NumeroanAgrafiche; // GestioneLicenza.TOT_ANAGRAFICHE_LICENZA_ENTRY;
        ConfiguraInterfacciaClientiPerStato();
        ConfiguraInterfaccia();
      }
      //status bar
      StatusBarItem_Anagrafiche.Content = "Anagrafiche: " + counterClienti.ToString();
      //if (App.TipoLicenza == App.TipologieLicenze.ClientLan || App.TipoLicenza == App.TipologieLicenze.ClientLanMulti || App.TipoLicenza == App.TipologieLicenze.Server)
      //if (App.CodiceMacchinaServer.Trim().Split('-')[0] != "" && App.CodiceMacchina.Split('-')[0] != App.CodiceMacchinaServer.Split('-')[0])
      //4.5.1
      if (App.Server || App.Client)
      {
        if (App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Remoto)
        {
          StatusBarItem_Archivio.Content = "Archivio: REMOTO";
        }
        else
        {
          StatusBarItem_Archivio.Content = "Archivio: LOCALE";
        }
      }
      else
      {
        StatusBarItem_Archivio.Content = "Archivio: STANDARD";
      }
    }

    //----------------------------------------------------------------------------+
    //                                 GetClienti                                 |
    //----------------------------------------------------------------------------+
    private void GetClienti()
    {
      int counter = 0;
      htClienti_ID.Clear();
      htClienti_RS.Clear();
      htClienti_CF.Clear();
      htClienti_PIVA.Clear();
      htClienti_Ese.Clear();
      htClienti_Stato.Clear();
      htClienti_StatoDesc.Clear();
      htClienti_StatoIcon.Clear();

      //recupero dati
      MasterFile mf = MasterFile.Create();
      ArrayList risultati = mf.GetAnagrafiche(showallclienti);

      //interfaccia
      if (risultati.Count > 0 && App.ErrorLevel == App.ErrorTypes.Nessuno)
      {
        List<string> sorted = new List<string>();
        foreach (Hashtable item in risultati)
        {
          sorted.Add(item["RagioneSociale"].ToString());
        }

        sorted.Sort();

        foreach (string str in sorted)
        {
          foreach (Hashtable item in risultati)
          {
            if (str == item["RagioneSociale"].ToString())
            {
              htClienti_ID.Add(counter, item["ID"].ToString());
              htClienti_RS.Add(counter, item["RagioneSociale"].ToString());
              htClienti_CF.Add(counter, item["CodiceFiscale"].ToString());
              htClienti_PIVA.Add(counter, item["PartitaIVA"].ToString());

              string additive = string.Empty;
              if (item["EsercizioDal"].ToString() != "" && item["EsercizioAl"].ToString() != "")
              {
                additive = " (dal " + item["EsercizioDal"].ToString() + " al " + item["EsercizioAl"].ToString() + ")";
              }

              //tipo esercizio
              switch ((App.TipoAnagraficaEsercizio)(Convert.ToInt32(item["Esercizio"].ToString())))
              {
                case App.TipoAnagraficaEsercizio.Sconosciuto:
                  htClienti_Ese.Add(counter, "Sconosciuto");
                  break;
                case App.TipoAnagraficaEsercizio.AnnoSolare:
                  htClienti_Ese.Add(counter, "Anno Solare" + additive);
                  break;
                case App.TipoAnagraficaEsercizio.ACavallo:
                  htClienti_Ese.Add(counter, "A Cavallo" + additive);
                  break;
                default:
                  break;
              }

              //stato
              htClienti_Stato.Add(counter, (App.TipoAnagraficaStato)(Convert.ToInt32(item["Stato"].ToString())));

              string stringaAdditiva = "";

              //icona stato
              switch ((App.TipoAnagraficaStato)(Convert.ToInt32(item["Stato"].ToString())))
              {
                case App.TipoAnagraficaStato.Sconosciuto:
                  htClienti_StatoIcon.Add(counter, "./Images/icone/Stato/warning.png");
                  break;
                case App.TipoAnagraficaStato.Disponibile:
                  htClienti_StatoIcon.Add(counter, "./Images/icone/Stato/ana_stato_ok.png");
                  break;
                case App.TipoAnagraficaStato.InUso:
                  htClienti_StatoIcon.Add(counter, "./Images/icone/Stato/ana_stato_nok.png");
                  break;
                case App.TipoAnagraficaStato.Bloccato:
                  htClienti_StatoIcon.Add(counter, "./Images/icone/Stato/ana_stato_nok.png");
                  break;
                case App.TipoAnagraficaStato.Esportato:
                  htClienti_StatoIcon.Add(counter, "./Images/icone/Stato/ana_stato_nok.png");
                  if (item["DataModificaStato"] != null || item["UtenteModificaStato"] != null)
                  {
                    stringaAdditiva = " (";

                    if (item["DataModificaStato"] != null)
                    {
                      stringaAdditiva += " " + item["DataModificaStato"].ToString() + " ";
                    }

                    if (item["UtenteModificaStato"] != null)
                    {
                      stringaAdditiva += " " + item["UtenteModificaStato"].ToString() + " ";
                    }

                    stringaAdditiva += ")";
                  }
                  break;
                default:
                  break;
              }

              //descrizione stato
              htClienti_StatoDesc.Add(counter, ((App.TipoAnagraficaStato)(Convert.ToInt32(item["Stato"].ToString()))).ToString() + stringaAdditiva);
              counter++;
              break;
            }
          }
        }
      }
    }

    //----------------------------------------------------------------------------+
    //                                ColonnaTesto                                |
    //----------------------------------------------------------------------------+
    private void ColonnaTesto(StackPanel stp, int counter, string testo)
    {
      Border b = new Border();
      b.MinHeight = 20.0;
      b.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
      b.VerticalAlignment = System.Windows.VerticalAlignment.Center;

      if (counter < 0)
      {
        b.Background = Brushes.White;
      }
      else if (counter % 2 == 0)
      {
        b.Background = App._arrBrushes[5];
      }
      else
      {
        b.Background = App._arrBrushes[6];
      }

      TextBlock t = new TextBlock();
      t.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
      t.VerticalAlignment = System.Windows.VerticalAlignment.Center;
      t.FontSize = 13;
      t.FontWeight = FontWeights.Regular;
      t.Foreground = Brushes.Black;
      t.Margin = new Thickness(3, 0, 0, 0);
      t.Text = testo;

      b.Child = t;

      b.MouseEnter += new MouseEventHandler(Border_MouseEnter);
      b.MouseLeave += new MouseEventHandler(Border_MouseLeave);
      b.MouseLeftButtonDown += new MouseButtonEventHandler(Border_MouseCLick);

      stp.Children.Add(b);
    }

    //----------------------------------------------------------------------------+
    //                              ColonnaImmagine                               |
    //----------------------------------------------------------------------------+
    private void ColonnaImmagine(StackPanel stp, int counter, string uri)
    {
      Border b = new Border();
      b.Height = 20.0;
      b.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
      b.VerticalAlignment = System.Windows.VerticalAlignment.Center;
      if (counter < 0)
      {
        b.Background = Brushes.White;
      }
      else if (counter % 2 == 0)
      {
        b.Background = App._arrBrushes[5];
      }
      else
      {
        b.Background = App._arrBrushes[6];
      }

      //stackpanel
      StackPanel s = new StackPanel();
      s.Orientation = Orientation.Horizontal;
      b.Child = s;

      //image
      Image i = new Image();
      Uri uriSource = null;
      uriSource = new Uri(htClienti_StatoIcon[counter].ToString(), UriKind.Relative);
      i.Source = new BitmapImage(uriSource);
      i.Width = 16.0;
      i.Margin = new Thickness(5, 0, 0, 0);
      i.ToolTip = htClienti_StatoDesc[counter];

      //testo
      TextBlock t = new TextBlock();
      t.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
      t.VerticalAlignment = System.Windows.VerticalAlignment.Center;
      t.FontSize = 13;
      t.FontWeight = FontWeights.Regular;
      t.Foreground = Brushes.Black;
      t.Margin = new Thickness(5, 0, 0, 0);
      t.Text = htClienti_StatoDesc[counter].ToString();

      //aggiungo oggetti            
      s.Children.Add(i);
      s.Children.Add(t);

      //eventi mouse
      b.MouseEnter += new MouseEventHandler(Border_MouseEnter);
      b.MouseLeave += new MouseEventHandler(Border_MouseLeave);
      b.MouseLeftButtonDown += new MouseButtonEventHandler(Border_MouseCLick);

      stp.Children.Add(b);
    }


    //gestore eventi mouse
    private void Border_MouseEnter(object sender, MouseEventArgs e)
    {
      Border b = (Border)sender;
      int index = ((StackPanel)(b).Parent).Children.IndexOf(b);
      if (index == IndexSelected && gridSelected == ((Grid)((StackPanel)((Border)sender).Parent).Parent))
      {
        return;
      }
      GridOldBackground = b.Background;
      Grid g_ext = ((Grid)((StackPanel)((Border)sender).Parent).Parent);
      g_ext = ((Grid)(((Border)(g_ext.Parent)).Parent));
      foreach (UIElement item_ext in g_ext.Children)
      {
        if (item_ext.GetType().Name == "Border")
        {
          if (((Border)item_ext).Child.GetType().Name == "Grid")
          {
            Grid g = ((Grid)(((Border)item_ext).Child));
            foreach (UIElement item in g.Children)
            {
              if (item.GetType().Name == "StackPanel")
              {
                ((Border)(((StackPanel)item).Children[index])).Background = App._arrBrushes[8];
              }
            }
          }
        }
      }
    }

    //----------------------------------------------------------------------------+
    //                             Border_MouseLeave                              |
    //----------------------------------------------------------------------------+
    private void Border_MouseLeave(object sender, MouseEventArgs e)
    {
      Border b = (Border)sender;
      int index = -1;
      try
      {
        index = ((StackPanel)(b).Parent).Children.IndexOf(b);
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        return;
      }
      if (index == IndexSelected && gridSelected == ((Grid)((StackPanel)((Border)sender).Parent).Parent))
      {
        return;
      }
      Grid g_ext = ((Grid)((StackPanel)((Border)sender).Parent).Parent);
      g_ext = ((Grid)(((Border)(g_ext.Parent)).Parent));
      foreach (UIElement item_ext in g_ext.Children)
      {
        if (item_ext.GetType().Name == "Border")
        {
          if (((Border)item_ext).Child.GetType().Name == "Grid")
          {
            Grid g = ((Grid)(((Border)item_ext).Child));
            foreach (UIElement item in g.Children)
            {
              if (item.GetType().Name == "StackPanel")
              {
                ((Border)(((StackPanel)item).Children[index])).Background = GridOldBackground;
              }
            }
          }
        }
      }
    }

    //----------------------------------------------------------------------------+
    //                             Border_MouseCLick                              |
    //----------------------------------------------------------------------------+
    private void Border_MouseCLick(object sender, MouseButtonEventArgs e)
    {

      bool doubleclick = false;
      Imposta_Bar_Visibility();
      ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
      ButtonBar_Clienti.Visibility = System.Windows.Visibility.Visible;
      if (e.ClickCount > 1)
      {
        doubleclick = true;
      }
      Border b = (Border)sender;
      int index = ((StackPanel)(b).Parent).Children.IndexOf(b);
      Grid g_ext;
      if (IndexSelected != -1 && !doubleclick)
      {
        //trovo l'attuale index reale
        IndexSelected = -1;
        foreach (UIElement item in gridSelected.Children)
        {
          if (item.GetType().Name == "StackPanel")
          {
            StackPanel s = ((StackPanel)item);
            foreach (UIElement item_int in s.Children)
            {
              if (item_int.GetType().Name == "Border")
              {
                if (((Border)item_int).Background == App._arrBrushes[7])
                {
                  IndexSelected = s.Children.IndexOf(item_int);
                  break;
                }
              }
            }
          }
          if (IndexSelected != -1)
          {
            break;
          }
        }
        if (IndexSelected != -1)
        {
          MasterFile mf = MasterFile.Create();
          string IDCliente = ((TextBlock)(((Border)(stpClienti_ID.Children[IndexSelected])).Child)).Text;
          mf.SetClienteFissato(IDCliente);
          StatusBarItem_ClienteFisso.Content = "Cliente Scelto: " + ((TextBlock)(((Border)(stpClienti_RagioneSociale.Children[IndexSelected])).Child)).Text;

          // showallclienti = false;
          //   ExpanderClientiExpanded();
          //    showallclienti = false;
          //   Expander_Revisione.IsEnabled = true;
          //    Expander_Verifiche.IsEnabled = true;
          //    Expander_Accettazione.IsEnabled = true;
          //    Expander_Relazioni.IsEnabled = true;

          g_ext = gridSelected;
          g_ext = ((Grid)(((Border)(g_ext.Parent)).Parent));
          foreach (UIElement item_ext in g_ext.Children)
          {
            if (item_ext.GetType().Name == "Border")
            {
              if (((Border)item_ext).Child.GetType().Name == "Grid")
              {
                Grid g = ((Grid)(((Border)item_ext).Child));
                foreach (UIElement item in g.Children)
                {
                  if (item.GetType().Name == "StackPanel")
                  {
                    StackPanel s = ((StackPanel)item);
                    ((Border)(s.Children[IndexSelected])).Background = GridSelectedBackground;
                  }
                }
              }
            }
          }

        }
      }
      if (IndexSelected == index && gridSelected == ((Grid)((StackPanel)((Border)sender).Parent).Parent) && !doubleclick)
      {
        IndexSelected = -1;
        gridSelected = null;
        GridSelectedBackground = null;
        return;
      }
      IndexSelected = index;
      gridSelected = ((Grid)((StackPanel)((Border)sender).Parent).Parent);
      GridSelectedBackground = GridOldBackground;
      ConfiguraInterfacciaClientiPerStato();
      g_ext = gridSelected;
      g_ext = ((Grid)(((Border)(g_ext.Parent)).Parent));
      foreach (UIElement item_ext in g_ext.Children)
      {
        if (item_ext.GetType().Name == "Border")
        {
          if (((Border)item_ext).Child.GetType().Name == "Grid")
          {
            Grid g = ((Grid)(((Border)item_ext).Child));
            foreach (UIElement item in g.Children)
            {
              if (item.GetType().Name == "StackPanel")
              {
                ((Border)(((StackPanel)item).Children[IndexSelected])).Background = App._arrBrushes[7];
              }
            }
          }
        }
      }
      if (doubleclick)
      {

        if (IndexSelected != -1)
        {
          MasterFile mf = MasterFile.Create();
          string IDCliente = ((TextBlock)(((Border)(stpClienti_ID.Children[IndexSelected])).Child)).Text;
          if (IDCliente != mf.GetClienteFissato())
          {
            mf.SetClienteFissato(IDCliente);
          }
          //if (((TextBlock)(((Expander)(((Grid)(((Border)(gridSelected.Parent)).Parent)).Parent)).Header)).Text == "Revisioni")
          //{
          //    AccediRevisione_Click(false);
          //}
          //if (((TextBlock)(((Expander)(((Grid)(((Border)(gridSelected.Parent)).Parent)).Parent)).Header)).Text == "Verifiche Trimestrali")
          //{
          //    AccediVerifica_Click(false);
          //}
          //if (((TextBlock)(((Expander)(((Grid)(((Border)(gridSelected.Parent)).Parent)).Parent)).Header)).Text == "Incarichi")
          //{
          //    AccediIncarico_Click(false);
          //}
          //if (((TextBlock)(((Expander)(((Grid)(((Border)(gridSelected.Parent)).Parent)).Parent)).Header)).Text == "Clienti")
          //{
          //    menuClientiModifica_Click(sender, e);
          //}
        }
      }
    }

    #endregion //------------------------------------------------- GESTIONE_CLIENTI

    #region STATUS_BAR

    //Dati utente/licenza
    private void StatusBarDatiUtente(GestioneLicenza l)
    {
      //Label
      StatusBarItem_Intestatario.Content = "Intestatario: " + l.Intestatario;
      StatusBarItem_Utente.Content = "Utente: " + l.Utente; ;
      //StatusBarItem_Scadenza.Content = "Scadenza: " + l.DataScadenzaLicenza.ToString();
      //StatusBarItem_TipoLicenza.Content = "Tipo licenza: " + l.NomeLicenza();
      StatusBarItem_Giorni.Content = "Giorni di utilizzo: " + l.GiorniUtilizzati.ToString() + " di " + l.DurataLicenza.ToString();
      StatusBarItem_Anagrafiche.Content = "Anagrafiche: " + counterClienti.ToString();

      //if (App.CodiceMacchinaServer.Trim().Split('-')[0] != "" && App.CodiceMacchina.Split('-')[0] != App.CodiceMacchinaServer.Split('-')[0])
      //if (App.TipoLicenza == App.TipologieLicenze.ClientLan || App.TipoLicenza == App.TipologieLicenze.ClientLanMulti || App.TipoLicenza == App.TipologieLicenze.Server)
      if (App.Server || App.Client)
      {
        if (App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Remoto)
        {
          StatusBarItem_Archivio.Content = "Archivio: REMOTO";
        }
        else
        {
          StatusBarItem_Archivio.Content = "Archivio: LOCALE";
        }
      }
      else
      {
        StatusBarItem_Archivio.Content = "Archivio: STANDARD";
      }

      //progress bar
      progressBarLicenza.Maximum = l.DurataLicenza;
      progressBarLicenza.Value = l.GiorniUtilizzati;
      if (l.ScadenzaVicina) progressBarLicenza.Foreground = Brushes.Red;

      CaricaClienti();
      MasterFile mf = MasterFile.Create();
      int indexhere = 0;

      foreach (var item in stpClienti_ID.Children)
      {
        if (((TextBlock)(((Border)(item)).Child)).Text == mf.GetClienteFissato())
        {
          StatusBarItem_ClienteFisso.Content = "Cliente Scelto: " + ((TextBlock)(((Border)(stpClienti_RagioneSociale.Children[indexhere])).Child)).Text;
          return;
        }

        indexhere++;
      }
    }

    #endregion //------------------------------------------------------- STATUS_BAR

    #region AUTORIZZAZIONE_INTERFACCIA

    //----------------------------------------------------------------------------+
    //                            ConfiguraInterfaccia                            |
    //----------------------------------------------------------------------------+
    public void ConfiguraInterfaccia()
    {
      //gestione archivio
      //menuStrumentiGestioneArchivio.IsEnabled = false;
      //backup
      //    btn_ApriBackUp.Visibility = System.Windows.Visibility.Collapsed;
      //menuStrumentiGestioneSalvataggi.IsEnabled = false; ;
      //creazione anagrafiche
      //menuClienteNuovo.IsEnabled = false;
      btn_Cliente_Nuovo.IsEnabled = false;
      //importa / esporta
      //menuClientiEsporta.IsEnabled = false;
      //menuClientiImporta.IsEnabled = false;
      btn_Cliente_Esporta.IsEnabled = false;
      btn_Cliente_Importa.IsEnabled = false;
      //btn_Cliente_Condividi.IsEnabled = false;

      //accesso ad archivio locale
      //if (App.AppConsentiAccessoArchivioLocale)
      //{
      //  menuStrumentiGestioneArchivio.IsEnabled = false;
      //}

      //Accesso ad archivio remoto
      //if (App.AppConsentiAccessoArchivioRemoto)
      //{
      //  menuStrumentiGestioneArchivio.IsEnabled = true;
      //}

      //Accesso ad archivio cloud
      //if (App.AppConsentiAccessoArchivioCloud)
      //{
      //  menuStrumentiGestioneArchivio.IsEnabled = true;
      //}

      //Gesione archivio remoto
      //if (App.AppConsentiGestioneArchivioRemoto)
      //{
      //  menuStrumentiGestioneArchivio.IsEnabled = true;
      //}

      //Gestione backup
      //4.5.1
      if (App.AppConsentiGestioneBackUp)
      {
        //    btn_ApriBackUp.Visibility = App.AppConsentiGestioneBackUp ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        //menuStrumentiGestioneSalvataggi.IsEnabled = App.AppConsentiBackUp;
      }

      //creazione anagrafiche
      if (App.AppConsentiCreazioneAnagrafica)
      {
        //menuClienteNuovo.IsEnabled = App.AppConsentiCreazioneAnagrafica;
        btn_Cliente_Nuovo.IsEnabled = App.AppConsentiCreazioneAnagrafica;
      }

      //Passaggio dati via lan fra server e client
      if (App.AppConsentiImportazioneEsportazioneLan)
      {
        //menuClientiEsporta.IsEnabled = App.AppConsentiImportazioneEsportazioneLan;
        //menuClientiImporta.IsEnabled = App.AppConsentiImportazioneEsportazioneLan;
        btn_Cliente_Esporta.IsEnabled = App.AppConsentiImportazioneEsportazioneLan;
        btn_Cliente_Importa.IsEnabled = App.AppConsentiImportazioneEsportazioneLan;
        //btn_Cliente_Condividi.IsEnabled = App.AppConsentiImportazioneEsportazioneLan;
      }

      //Passaggio dati via lan fra server e client
      if (App.AppConsentiImportaEsporta)
      {
        //menuClientiEsporta.IsEnabled = App.AppConsentiImportaEsporta;
        //menuClientiImporta.IsEnabled = App.AppConsentiImportaEsporta;
        btn_Cliente_Esporta.IsEnabled = App.AppConsentiImportaEsporta;
        btn_Cliente_Importa.IsEnabled = App.AppConsentiImportaEsporta;
        //btn_Cliente_Condividi.IsEnabled = App.AppConsentiImportaEsporta;

        //andrea 2.8 ******************* CACCA
        //menuClientiEsporta.IsEnabled = true;
        //menuClientiImporta.IsEnabled = true;
        btn_Cliente_Esporta.IsEnabled = true;
        btn_Cliente_Importa.IsEnabled = true;
        //btn_Cliente_Condividi.IsEnabled = true;
        //menuClientiEsporta.Visibility = System.Windows.Visibility.Visible;
        //menuClientiImporta.Visibility = System.Windows.Visibility.Visible;
        /* *************************************************************** */
      }



      //Multilicenza
      if (App.AppConsentiMultiLicenza)
      {
        //btn_Multilicenza.IsEnabled = App.AppConsentiMultiLicenza;
      }

      //bottone reset archivio in strumenti
      //      btn_ResetMasterFile.Visibility = (App.Guest == true) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;// (App.TipoLicenza == App.TipologieLicenze.Viewer || App.TipoLicenza == App.TipologieLicenze.Guest) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
    }

    #endregion //--------------------------------------- AUTORIZZAZIONE_INTERFACCIA


    #region VARIE_ED_EVENTUALI

    //----------------------------------------------------------------------------+
    //                         PermettiAttivazioneLicenza                         |
    //----------------------------------------------------------------------------+
    private void PermettiAttivazioneLicenza(RevisoftApplication.GestioneLicenza l)
    {
      //Attivazione prima licenza
      WindowGestioneLicenza w1 = new WindowGestioneLicenza();
      w1.ShowDialog();
      //licenza non attivata
      if (!l.StatoLicenza)
      {
        System.Environment.Exit(0);
      }
    }

    #endregion //----------------------------------------------- VARIE_ED_EVENTUALI

    public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
    {
      if (depObj != null)
      {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
          DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
          if (child != null && child is T)
          {
            yield return (T)child;
          }

          foreach (T childOfChild in FindVisualChildren<T>(child))
          {
            yield return childOfChild;
          }
        }
      }
    }
    private void Imposta_rd_forecoolor()
    {
      foreach (RadioButton rdb in FindVisualChildren<RadioButton>(quadroprincipale))
      {
        rdb.Foreground = App._arrBrushes[2];
      }
    }

    private void Imposta_rd_checked()
    {
      foreach (RadioButton rdb in FindVisualChildren<RadioButton>(quadroprincipale))
      {
        rdb.IsChecked = false;
      }
    }

    private void Imposta_ExpanderChiusi(object sender)
    {

      foreach (Expander st in FindVisualChildren<Expander>(quadroprincipale))
      {
        if (st.Name == ((Expander)sender).Name)
          continue;
        //   if (st.Name == "Expander_Clienti")
        //       continue;
        if (st.Name.IndexOf("Expander_") >= 0)
          st.IsExpanded = false;
      }

         ((Expander)sender).IsExpanded = true;
    }

    private void Imposta_Bar_Visibility(object sender = null)
    {
      foreach (StackPanel st in FindVisualChildren<StackPanel>(ButtonBar))
      {
        if (st.Name.IndexOf("ButtonBar_") >= 0)
          st.Visibility = System.Windows.Visibility.Collapsed;
        if (sender != null && ((Expander)sender).Name == "Expander_Clienti" && st.Name == "ButtonBar_Clienti")
          st.Visibility = System.Windows.Visibility.Visible;

      }
    }


    //----------------------------------------------------------------------------+
    //                            RadioButton_Checked                             |
    //----------------------------------------------------------------------------+
    private void RadioButton_Checked(object sender, RoutedEventArgs e)
    {
      Imposta_rd_forecoolor();
      Imposta_Bar_Visibility();
      ButtonBar_AlwaysON.Visibility = System.Windows.Visibility.Visible;
      ((RadioButton)sender).Foreground = App._arrBrushes[1];

      switch (((RadioButton)sender).Name)
      {
        case "Area1CS":
        case "Area1SU":
        case "Area1REV":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
          ButtonBar_Incarichi.Visibility = System.Windows.Visibility.Visible;

          break;
        case "rdbISQC":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
          ButtonBar_ISQCs.Visibility = System.Windows.Visibility.Visible;

          break;
        case "rdbRevisione":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
          ButtonBar_Revisioni.Visibility = System.Windows.Visibility.Visible;

          break;

        case "rdbBilancio":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
          ButtonBar_Bilanci.Visibility = System.Windows.Visibility.Visible;

          break;
        case "rdbConclusioni":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
          ButtonBar_Conclusioni.Visibility = System.Windows.Visibility.Visible;

          break;
        case "rdbVerifica":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
          ButtonBar_Verifiche.Visibility = System.Windows.Visibility.Visible;
          break;
        case "rdbVigilanza":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
          ButtonBar_Vigilanza.Visibility = System.Windows.Visibility.Visible;

          break;
        case "rdbPianificazioniVerifica":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
          ButtonBar_PianificazioniVerifiche.Visibility = System.Windows.Visibility.Visible;

          break;
        case "rdbPianificazioniVigilanza":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
          ButtonBar_PianificazioniVigilanza.Visibility = System.Windows.Visibility.Visible;

          break;
        case "rdbRelazioneB":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
          ButtonBar_RelazioneB.Visibility = System.Windows.Visibility.Visible;

          break;
        case "rdbRelazioneBC":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
          ButtonBar_RelazioneBC.Visibility = System.Windows.Visibility.Visible;

          break;
        case "rdbRelazioneV":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
          ButtonBar_RelazioneV.Visibility = System.Windows.Visibility.Visible;

          break;
        case "rdbRelazioneVC":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
          ButtonBar_RelazioneVC.Visibility = System.Windows.Visibility.Visible;

          break;
        case "rdbRelazioneBV":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
          ButtonBar_RelazioneBV.Visibility = System.Windows.Visibility.Visible;

          break;
        case "rdb_Flusso_ISQC":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
          ButtonBar_Flussi.Visibility = System.Windows.Visibility.Visible;

          break;
        case "rdb_Flusso_Societa":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
          ButtonBar_Flussi.Visibility = System.Windows.Visibility.Visible;

          break;
        case "rdb_Flusso_Gruppo":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;

          ButtonBar_Flussi.Visibility = System.Windows.Visibility.Visible;

          break;
        case "rdb_Flusso_Terzi":

          ButtonBar_HomePage.Visibility = System.Windows.Visibility.Visible;
          ButtonBar_Flussi.Visibility = System.Windows.Visibility.Visible;

          break;
        default:
          break;
      }
      if (App.AppTipo == App.ModalitaApp.Team)
        RadioButton_Checked_Team(((RadioButton)sender).Name);
    }
    //TEAM
    private void RadioButton_Checked_Team(string radioButton)
    {
      switch (radioButton)
      {
        case "rdbIncarico":
          if (App.AppRuolo == App.RuoloDesc.Esecutore || App.AppRuolo == App.RuoloDesc.Reviewer)
          {
            btn_Incarico_Nuovo.IsEnabled = false;
            btn_Incarico_Riesame.IsEnabled = false;
            btn_Incarico_Elimina.IsEnabled = false;
            //btn_Incarico_Modifica.Visibility = Visibility.Collapsed;//
            //btn_Incarico_Sessione_Apri_ReadOnly.Visibility = Visibility.Visible;//
          }
          break;
        case "rdbISQC":
          if (App.AppRuolo == App.RuoloDesc.Esecutore || App.AppRuolo == App.RuoloDesc.Reviewer)
          {
            btn_ISQC_Elimina.IsEnabled = false;
            btn_ISQC_Nuovo.IsEnabled = false;
            //btn_ISQC_Modifica.Visibility = Visibility.Collapsed;//
            //btn_ISQC_Sessione_Apri_ReadOnly.Visibility = Visibility.Visible;//

          }
          break;
        case "rdbRevisione":
          if (App.AppRuolo == App.RuoloDesc.Reviewer)
          {
            btn_Revisione_Nuova.IsEnabled = false;
            btn_Revisione_Elimina.IsEnabled = false;
            //btn_Revisione_Modifica.Visibility = Visibility.Collapsed;//
            //btn_Revisione_Sessione_Apri_ReadOnly.Visibility = Visibility.Visible;//
          }
          break;
        case "rdbBilancio":
          if (App.AppRuolo == App.RuoloDesc.Reviewer)
          {
            btn_Bilancio_Elimina.IsEnabled = false;
            btn_Bilancio_Nuova.IsEnabled = false;
            //btn_Bilancio_Modifica.Visibility = Visibility.Collapsed;//
            //btn_Bilancio_Sessione_Apri_ReadOnly.Visibility = Visibility.Visible;//
          }
          break;
        case "rdbConclusioni":
          if (App.AppRuolo == App.RuoloDesc.Reviewer)
          {
            btn_Conclusione_Elimina.IsEnabled = false;
            btn_Conclusione_Nuova.IsEnabled = false;
            //btn_Conclusione_Modifica.Visibility = Visibility.Collapsed;//
            //btn_Conclusione_Sessione_Apri_ReadOnly.Visibility = Visibility.Visible;//
          }
          break;
        case "rdbVerifica":
          if (App.AppRuolo == App.RuoloDesc.Reviewer)
          {
            btn_Verifica_Elimina.IsEnabled = false;
            btn_Verifica_Nuova.IsEnabled = false;
            //btn_Verifica_Modifica.Visibility = Visibility.Collapsed;//
            //btn_Verifica_Sessione_Apri_ReadOnly.Visibility = Visibility.Visible;//
          }
          break;
        case "rdbVigilanza":
          if (App.AppRuolo == App.RuoloDesc.Reviewer)
          {
            btn_Vigilanza_Elimina.IsEnabled = false;
            btn_Vigilanza_Nuova.IsEnabled = false;
            //btn_Vigilanza_Modifica.Visibility = Visibility.Collapsed;//
            //btn_Vigilanza_Sessione_Apri_ReadOnly.Visibility = Visibility.Visible;//
          }
          break;
        case "rdbPianificazioniVerifica":
          if (App.AppRuolo == App.RuoloDesc.Reviewer)
          {
            btn_PianificazioniVerifica_Elimina.IsEnabled = false;
            btn_PianificazioniVerifica_Nuova.IsEnabled = false;
            //btn_PianificazioniVerifica_Modifica.Visibility = Visibility.Collapsed;//
            //btn_PianificazioniVerifica_Sessione_Apri_ReadOnly.Visibility = Visibility.Visible;//
          }
          break;
        case "rdbPianificazioniVigilanza":
          if (App.AppRuolo == App.RuoloDesc.Reviewer)
          {
            btn_PianificazioniVigilanza_Elimina.IsEnabled = false;
            btn_PianificazioniVigilanza_Nuova.IsEnabled = false;
            //btn_PianificazioniVigilanza_Modifica.Visibility = Visibility.Collapsed;//
            //btn_PianificazioniVigilanza_Sessione_Apri_ReadOnly.Visibility = Visibility.Visible;//
          }
          break;
        case "rdbRelazioneB":
          if (App.AppRuolo == App.RuoloDesc.Reviewer)
          {
            btn_RelazioneB_Elimina.IsEnabled = false;
            btn_RelazioneB_Nuova.IsEnabled = false;
            //btn_RelazioneB_Modifica.Visibility = Visibility.Collapsed;//
            //btn_RelazioneB_Sessione_Apri_ReadOnly.Visibility = Visibility.Visible;//
          }
          break;
        case "rdbRelazioneBC":
          if (App.AppRuolo == App.RuoloDesc.Reviewer)
          {
            btn_RelazioneBC_Elimina.IsEnabled = false;
            btn_RelazioneBC_Nuova.IsEnabled = false;
            //btn_RelazioneBC_Modifica.Visibility = Visibility.Collapsed;//
            //btn_RelazioneBC_Sessione_Apri_ReadOnly.Visibility = Visibility.Visible;//
          }
          break;
        case "rdbRelazioneV":
          if (App.AppRuolo == App.RuoloDesc.Reviewer)
          {
            btn_RelazioneV_Elimina.IsEnabled = false;
            btn_RelazioneV_Nuova.IsEnabled = false;
            //btn_RelazioneV_Modifica.Visibility = Visibility.Collapsed;//
            //btn_RelazioneV_Sessione_Apri_ReadOnly.Visibility = Visibility.Visible;//
          }
          break;
        case "rdbRelazioneVC":
          if (App.AppRuolo == App.RuoloDesc.Reviewer)
          {
            btn_RelazioneVC_Elimina.IsEnabled = false;
            btn_RelazioneVC_Nuova.IsEnabled = false;
            //btn_RelazioneVC_Modifica.Visibility = Visibility.Collapsed;//
            //btn_RelazioneVC_Sessione_Apri_ReadOnly.Visibility = Visibility.Visible;//
          }
          break;
        case "rdbRelazioneBV":
          if (App.AppRuolo == App.RuoloDesc.Reviewer)
          {
            btn_RelazioneBV_Elimina.IsEnabled = false;
            btn_RelazioneBV_Nuova.IsEnabled = false;
            //btn_RelazioneBV_Modifica.Visibility = Visibility.Collapsed;//
            //btn_RelazioneBV_Sessione_Apri_ReadOnly.Visibility = Visibility.Visible;//
          }
          break;
        case "rdb_Flusso_ISQC":
          break;
        case "rdb_Flusso_Societa":
          break;
        case "rdb_Flusso_Gruppo":
          break;
        case "rdb_Flusso_Terzi":
          break;
        default:
          break;
      }
    }

    //----------------------------------------------------------------------------+
    //                               Window_Closing                               |
    //----------------------------------------------------------------------------+
    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      Process thisProc = Process.GetCurrentProcess();

      if (Process.GetProcessesByName(thisProc.ProcessName).Length <= 1)
      {
        if (MessageBox.Show("E' vivamente consigliabile effettuare un backup del sistema prima di uscire, procedere?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
          BackUpFile bkf = new BackUpFile();
          Hashtable ht = new Hashtable();
          bkf.SetBackUp(ht, -1);
        }
      }
    }

    private void MainWindow_Closed(object sender, CancelEventArgs e)
    {
      MasterFile mf = MasterFile.Create();
      mf.SetClienteFissato("-1");
#if (DBG_TEST)
      StaticUtilities.SetLockStatus("", "", false);
      //StaticUtilities.DumpModifiedCache();
      //      //StaticUtilities.ClearXmlCache();
      //      using (SqlConnection conn = new SqlConnection(App.connString))
      //      {
      //          conn.Open();
      //          SqlCommand cmd = new SqlCommand("dbo.SaveModified", conn);
      //          cmd.CommandType = CommandType.StoredProcedure;
      //          cmd.CommandTimeout = App.m_CommandTimeout;
      //          try { cmd.ExecuteNonQuery(); }
      //          catch (Exception ex)
      //          {
      //              if (!App.m_bNoExceptionMsg)
      //              {
      //                  string msg = "SQL call 'dbo.SaveModified' failed: errore\n" + ex.Message;
      //                  MessageBox.Show(msg);
      //              }
      //          }
      //      }
#endif
      RevisoftApplication.Utilities u = new Utilities();
      ((System.ComponentModel.CancelEventArgs)e).Cancel = u.ChiudiApplicazioneConBackup();
    }
    private void btn_Apri_Utenti_Click(object sender, RoutedEventArgs e)
    {
      // apertura finestra di gestione utenti
      wUtenti w = new wUtenti();
      w.ShowDialog();

    }
    private void btn_Associa_Utenti_Click(object sender, RoutedEventArgs e)
    {
      wAssociaRuoliUtenti w = new wAssociaRuoliUtenti();
      w.ShowDialog();
    }

    private void btn_Riepilogo_Utenti_Click(object sender, RoutedEventArgs e)
    {
      wRiepilogoUtenti w = new wRiepilogoUtenti();
      w.ShowDialog();
    }

    private void btn_AssociaClientiLeader_Click(object sender, RoutedEventArgs e)
    {
      wAssociaTeamAiClienti wAssocia = new wAssociaTeamAiClienti();

      wAssocia._teamList = RevisoftApplication.BRL.cUtenti.GetUtentiTeamLeader();
      if (wAssocia._teamList == null)
      {
        MessageBox.Show("Attenzione: non è possibile eseguire associazioni poichè non sono presenti utenti con ruolo Team leader", "Assenza utenze con ruolo team leader", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }
      wAssocia.ShowDialog();
    }
    private void Btn_Ticket_Click(object sender, RoutedEventArgs e)
    {
      wticket tt = new wticket();
      tt.Owner = this;
      tt.ShowDialog();

    }

    private void menuClienti_elenco_Click(object sender, RoutedEventArgs e)
    {
      Expander_Clienti.IsExpanded = true;
      showallclienti = true;
      ExpanderClientiExpanded();
    }
  } //-------------------------------- public partial class MainWindow : Window
} //--------------------------------------------- namespace RevisoftApplication




