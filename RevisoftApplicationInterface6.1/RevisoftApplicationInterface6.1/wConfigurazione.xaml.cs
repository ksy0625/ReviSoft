using System;
using System.Windows;
using System.Windows.Input;
using System.Collections;

namespace RevisoftApplication
{

  public partial class wConfigurazione : Window
  {

    private bool _open;
    private bool _magicKeyCombination = false;

    public wConfigurazione()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      //Interfaccia
      _open = true;
      checkBoxMostraAllAvvio.Visibility = System.Windows.Visibility.Collapsed;
      checkBoxMostraAllAvvio.IsChecked = App.AppSetupBenvenuto;
      checkBoxMostraIstruzioniAutomatiche.IsChecked = App.AppSetupIstruzioniAutomatiche;
      checkBoxMostraAlertSuCompletato.IsChecked = App.AppSetupAlertSuCompletato;
      //4.6
      checkBoxUserBackupPath.IsChecked = App.AppSetupBackupPersonalizzato;
      textBoxUserBackupPath.Text = App.AppUserBackupFolder;
      _open = false;

      //nascondo info di sistema
      tabItemFunzionalita.Visibility = System.Windows.Visibility.Collapsed;
      tabItemConfigurazioneLicenza.Visibility = System.Windows.Visibility.Collapsed;

      //licenza GUEST: disabilito reset archivio
      //if (App.TipoLicenza == App.TipologieLicenze.Guest)
      if (App.Guest == true)
      {
        buttonResetMasterFile.Visibility = System.Windows.Visibility.Collapsed;
      }

      //gestore evento keypress
      AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler)HandleKeyDownEvent);
    }

    private void VisualizzaStatoFunzionalita()
    {
      //funzionalità
      checkBoxConsentiAccessoArchivioLocale.IsChecked = App.AppConsentiAccessoArchivioLocale;
      checkBoxConsentiAccessoArchivioRemoto.IsChecked = App.AppConsentiAccessoArchivioRemoto;
      checkBoxConsentiAccessoArchivioCloud.IsChecked = App.AppConsentiAccessoArchivioCloud;
      checkBoxConsentiCreazioneAnagrafica.IsChecked = App.AppConsentiCreazioneAnagrafica;
      checkBoxConsentiImportaEsporta.IsChecked = App.AppConsentiImportaEsporta;
      checkBoxConsentiImportazioneEsportazioneLan.IsChecked = App.AppConsentiImportazioneEsportazioneLan;
      checkBoxConsentiGestioneArchivioRemoto.IsChecked = App.AppConsentiGestioneArchivioRemoto;
      checkBoxConsentiBackUp.IsChecked = App.AppConsentiBackUp;
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

    private void checkBoxMostraAllAvvio_Check(object sender, RoutedEventArgs e)
    {
      if (!_open)
      {
        App.AppSetupBenvenuto = (bool)checkBoxMostraAllAvvio.IsChecked;
        //salvo nuova configurazione
        GestioneLicenza l = new GestioneLicenza();
        l.SalvaInfoDataUltimoUtilizzo();
      }
    }


    private void checkBoxUserBackupPath_Checked(object sender, RoutedEventArgs e)
    {
      //attivo percorso personalizzato
      if (((bool)checkBoxUserBackupPath.IsChecked))
      {
        textBoxUserBackupPath.IsEnabled = true;
        buttonSelezionaPathBackupUtente.IsEnabled = true;
      }
      else
      //imposto percorso default
      {
        textBoxUserBackupPath.IsEnabled = true;
        buttonSelezionaPathBackupUtente.IsEnabled = true;
        textBoxUserBackupPath.Text = "";
        App.AppSetupBackupPersonalizzato = false;
        App.AppUserBackupFolder = "";
        GestioneLicenza l = new GestioneLicenza();
        l.SalvaInfoDataUltimoUtilizzo();
      }

    }

    private void buttonSelezionaCartellaBackup_Click(object sender, RoutedEventArgs e)
    {
      RevisoftApplication.Utilities u = new Utilities();
      string ret = u.sys_OpenDirectoryDialog();

      if (ret.Trim() != "")
      {
        try
        {
          textBoxUserBackupPath.Text = ret;
          string d = textBoxUserBackupPath.Text + "\\" + App.BackUpFolder + "\\" + App.ClientiEsportatiFolder;
          if (!System.IO.Directory.Exists(d))
          {
            //creo sub cartella di sistema
            System.IO.Directory.CreateDirectory(d);
          }
          //setto var applicazione
          App.AppSetupBackupPersonalizzato = true;
          App.AppUserBackupFolder = ret;
          App.AppBackupFolderUser = App.AppUserBackupFolder + "\\" + App.BackUpFolder;
          App.AppBackUpDataFileUser = App.AppUserBackupFolder + "\\" + App.ApplicationFileName + u.EstensioneFile(App.TipoFile.BackUp);
          GestioneLicenza l = new GestioneLicenza();
          l.SalvaInfoDataUltimoUtilizzo();
        }
        //in caso di errore torno alla configurazione di default
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wConfigurazione.buttonSelezionaCartellaBackup_Click exception");
          checkBoxUserBackupPath.IsChecked = false;
          textBoxUserBackupPath.Text = "";
          textBoxUserBackupPath.IsEnabled = false;
          buttonSelezionaPathBackupUtente.IsEnabled = false;
        }
      }
      //cartella non selezionata
      else
      {
        checkBoxUserBackupPath.IsChecked = false;
        textBoxUserBackupPath.Text = "";
        textBoxUserBackupPath.IsEnabled = false;
        buttonSelezionaPathBackupUtente.IsEnabled = false;
      }


    }


    private void buttonSbloccaUtenti_Click(object sender, RoutedEventArgs e)
    {
      //richiesta conferma
      Utilities u = new Utilities();
      if (MessageBoxResult.Yes == u.ConfermaSbloccoUtenti())
      {
        MasterFile mf = MasterFile.Create();
        bool esportatipresenti = false;

        foreach (Hashtable item in mf.GetAnagrafiche())
        {
          if (mf.GetAnafraficaStato(Convert.ToInt32(item["ID"].ToString())) != App.TipoAnagraficaStato.Esportato)
          {
            mf.SetAnafraficaStato(Convert.ToInt32(item["ID"].ToString()), App.TipoAnagraficaStato.Disponibile);
          }
          else
          {
            esportatipresenti = true;
          }
        }

          ((RevisoftApplication.MainWindow)(this.Owner)).CaricaClienti();

        if (esportatipresenti == true)
        {
          MessageBox.Show("Sblocco Utenti Esportati disponibile solo da HelpDesk");
        }
        else
        {
          MessageBox.Show("Sblocco Utenti Avvenuto con successo");
        }
      }
    }

    private void buttonResetMasterFile_Click(object sender, RoutedEventArgs e)
    {
      //richiesta conferma
      Utilities u = new Utilities();
      if (MessageBoxResult.Yes == u.ConfermaResetArchivio())
      {
        MasterFile mf = MasterFile.Create();
        mf.ResetMasterFile();
        ((MainWindow)(this.Owner)).ReloadMainWindow();
        MessageBox.Show("Reset Avvenuto con successo");
      }
    }

    private void MostraInfoSistema(object sender, RoutedEventArgs e)
    {
      if (_magicKeyCombination)
      {
        tabItemFunzionalita.Visibility = System.Windows.Visibility.Visible;
        tabItemConfigurazioneLicenza.Visibility = System.Windows.Visibility.Visible;
        //funzionalità
        VisualizzaStatoFunzionalita();
      }

    }

    private void HandleKeyDownEvent(object sender, KeyEventArgs e)
    {
      if (Keyboard.IsKeyToggled(Key.LeftCtrl) && Keyboard.IsKeyToggled(Key.LeftShift) && Keyboard.IsKeyToggled(Key.R))
      {
        _magicKeyCombination = true;
      }

    }

    //configurazione licenza ********************************************
    private void buttonLicenzaProva_Click(object sender, RoutedEventArgs e)
    {
      //App.TipoLicenza = App.TipologieLicenze.Prova;
      //App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Locale;
      //((MainWindow)(this.Owner)).RepeatRevisoftInit();
      ////funzionalità
      //VisualizzaStatoFunzionalita();
    }

    private void buttonLicenzaServer_Click(object sender, RoutedEventArgs e)
    {
      //App.TipoLicenza = App.TipologieLicenze.Server;
      //App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Locale;
      //((MainWindow)(this.Owner)).RepeatRevisoftInit();
      ////funzionalità
      //VisualizzaStatoFunzionalita();
    }

    private void buttonLicenzaDeskTop_Click(object sender, RoutedEventArgs e)
    {
      //App.TipoLicenza = App.TipologieLicenze.DeskTop;
      //App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Locale;
      //((MainWindow)(this.Owner)).RepeatRevisoftInit();
      ////funzionalità
      //VisualizzaStatoFunzionalita();
    }

    private void buttonLicenzaClient_Click(object sender, RoutedEventArgs e)
    {
      //App.TipoLicenza = App.TipologieLicenze.ClientLan;
      //App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Locale;
      //((MainWindow)(this.Owner)).RepeatRevisoftInit();
      ////funzionalità
      //VisualizzaStatoFunzionalita();
    }

    private void buttonLicenzaEntry_Click(object sender, RoutedEventArgs e)
    {
      //App.TipoLicenza = App.TipologieLicenze.EntryLevel;
      //App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Locale;
      //((MainWindow)(this.Owner)).RepeatRevisoftInit();
      ////funzionalità
      //VisualizzaStatoFunzionalita();
    }

    private void buttonLicenzaViewer_Click(object sender, RoutedEventArgs e)
    {
      //App.TipoLicenza = App.TipologieLicenze.Viewer;
      //App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Locale;
      //((MainWindow)(this.Owner)).RepeatRevisoftInit();
      ////funzionalità
      //VisualizzaStatoFunzionalita();
    }

    private void checkBoxMostraAlertSuCompletato_Checked(object sender, RoutedEventArgs e)
    {
      if (!_open)
      {
        App.AppSetupAlertSuCompletato = (bool)checkBoxMostraAlertSuCompletato.IsChecked;
        //salvo nuova configurazione
        GestioneLicenza l = new GestioneLicenza();
        l.SalvaInfoDataUltimoUtilizzo();
      }
    }


    private void checkBoxMostraIstruzioniAutomatiche_Checked(object sender, RoutedEventArgs e)
    {
      if (!_open)
      {
        App.AppSetupIstruzioniAutomatiche = (bool)checkBoxMostraIstruzioniAutomatiche.IsChecked;
        //salvo nuova configurazione
        GestioneLicenza l = new GestioneLicenza();
        l.SalvaInfoDataUltimoUtilizzo();
      }
    }

  }
}
