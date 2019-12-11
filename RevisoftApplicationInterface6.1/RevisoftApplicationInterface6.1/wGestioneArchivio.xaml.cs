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
using System.Collections;
using System.IO;

namespace RevisoftApplication
{

  public partial class wGestioneArchivio : Window
  {

    public wGestioneArchivio()
    {
      InitializeComponent();
      label9.Foreground = App._arrBrushes[0];

      RevisoftApplication.Utilities u = new Utilities();

      //interfaccia
      buttonSelezionaArchivioRemoto.IsEnabled = false;
      radioButtonArchivioLocale.IsChecked = App.AppSetupTipoGestioneArchivio.ToString() == App.TipoGestioneArchivio.Locale.ToString() || App.AppSetupTipoGestioneArchivio.ToString() == App.TipoGestioneArchivio.LocaleImportExport.ToString();
      radioButtonArchivioRemoto.IsChecked = App.AppSetupTipoGestioneArchivio.ToString() == App.TipoGestioneArchivio.Remoto.ToString();
      radioButtonArchivioCloud.IsChecked = App.AppSetupTipoGestioneArchivio.ToString() == App.TipoGestioneArchivio.Cloud.ToString();
      buttonApplica.IsEnabled = false;
      //Interfaccia x autorizzazioni - posizione archivio
      radioButtonArchivioLocale.IsEnabled = App.AppConsentiAccessoArchivioLocale;
      radioButtonArchivioRemoto.IsEnabled = App.AppConsentiAccessoArchivioRemoto;
      radioButtonArchivioCloud.IsEnabled = App.AppConsentiAccessoArchivioCloud;
      //Interfaccia x autorizzazioni - trasferimento archivio locale/lan
      btn_TrasferimentoArchivi.IsEnabled = App.AppConsentiGestioneArchivioRemoto;
      btn_TrasferimentoArchiviLocale.IsEnabled = App.AppConsentiGestioneArchivioRemoto;

      //Interfaccia x autorizzazioni - posizione archivio - REMOTO
      textBoxArchivioRemotoPath.Text = App.AppPathArchivioRemoto;
      if (App.AppConsentiAccessoArchivioRemoto && App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Remoto)
      {
        textBoxArchivioRemotoPath.IsEnabled = App.AppConsentiAccessoArchivioRemoto;
        buttonSelezionaArchivioRemoto.IsEnabled = false; //App.AppConsentiAccessoArchivioRemoto;
      }

      //modifile all'interfaccia dell'ultimo momento
      radioButtonArchivioLocale.IsEnabled = false;
      radioButtonArchivioRemoto.IsEnabled = false;

      if (App.AppConsentiGestioneArchivioRemoto)
      {
        btn_TrasferimentoArchivi.IsEnabled = App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Locale;
        btn_TrasferimentoArchiviLocale.IsEnabled = App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Remoto;
      }

      //forzatura x configurazione percorso di rete
      if (App.AppAutoExec && App.AppAutoExecFunzione == App.TipoFunzioniAutoexec.SetupLan)
      {
        radioButtonArchivioLocale.IsEnabled = true;
        radioButtonArchivioRemoto.IsEnabled = true;
        textBoxArchivioRemotoPath.Text = App.AppPathArchivioRemoto;
        buttonApplica.IsEnabled = true;
        buttonSelezionaArchivioRemoto.IsEnabled = true;
        btn_TrasferimentoArchiviLocale.IsEnabled = false;
        btn_TrasferimentoArchiviLocale.IsEnabled = false;
      }

      //if (App.CodiceMacchinaServer.Trim() != "" && App.CodiceMacchina != App.CodiceMacchinaServer)
      {
        //if (App.TipoLicenza == App.TipologieLicenze.ClientLan || App.TipoLicenza == App.TipologieLicenze.ClientLanMulti)
        //{
        radioButtonArchivioLocale.IsEnabled = true;
        radioButtonArchivioRemoto.IsEnabled = true;
      }

    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

    private void radioButtonArchivioLocale_Checked(object sender, RoutedEventArgs e)
    {
      textBoxArchivioRemotoPath.IsEnabled = false;
      buttonSelezionaArchivioRemoto.IsEnabled = false;
      textBoxArchivioRemotoPath.Text = "";
      buttonApplica.IsEnabled = true;
    }

    private void radioButtonArchivioRemoto_Checked(object sender, RoutedEventArgs e)
    {
      textBoxArchivioRemotoPath.IsEnabled = true;
      buttonSelezionaArchivioRemoto.IsEnabled = true;
      buttonApplica.IsEnabled = true;
    }

    private void buttonSelezionaArchivioRemoto_Click(object sender, RoutedEventArgs e)
    {
      RevisoftApplication.Utilities u = new Utilities();
      string ret = u.sys_OpenFileDialog("Archivio remoto", App.TipoFile.Master);

      if (ret != null)
        textBoxArchivioRemotoPath.Text = ret.Substring(0, ret.LastIndexOf("\\"));
      else
        textBoxArchivioRemotoPath.Text = "";
    }

    //ARCHIVIO MASTER: importa intero archivio
    private void menuStrumentiImportaArchivioRemoto(object sender, RoutedEventArgs e)
    {

      RevisoftApplication.Utilities u = new Utilities();
      string ret = u.sys_OpenFileDialog("Archivio remoto", App.TipoFile.Master);

      if (ret != null)
      {
        wFileImporta w1 = new wFileImporta();
        w1.textFileName.Text = ret;
        w1.Owner = this;
        w1.ShowDialog();
      }

    }

    //ARCHIVIO MASTER: esporta intero archivio
    private void menuStrumentiEsportaArchivioRemoto(object sender, RoutedEventArgs e)
    {

      RevisoftApplication.Utilities u = new Utilities();
      string ret = u.sys_OpenFileDialog("Archivio remoto", App.TipoFile.Master);

      if (ret != null)
      {

      }

    }

    private void buttonApplica_Click(object sender, RoutedEventArgs e)
    {


      //richiesta conferma
      Utilities u = new Utilities();
      if (MessageBoxResult.No == u.ConfermaSettaggioArchivio())
      {
        //ripristono interfaccia
        radioButtonArchivioLocale.IsChecked = App.AppSetupTipoGestioneArchivio.ToString() == App.TipoGestioneArchivio.Locale.ToString();
        radioButtonArchivioRemoto.IsChecked = App.AppSetupTipoGestioneArchivio.ToString() == App.TipoGestioneArchivio.Remoto.ToString();
        radioButtonArchivioCloud.IsChecked = App.AppSetupTipoGestioneArchivio.ToString() == App.TipoGestioneArchivio.Cloud.ToString();
        //buttonApplica.IsEnabled = false;
        return;
      }

      //locale
      if ((bool)radioButtonArchivioLocale.IsChecked)
      {
        //setto variabili globali
        App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Locale;

        //salvo nuova configurazione
        GestioneLicenza l = new GestioneLicenza();
        l.SalvaInfoDataUltimoUtilizzo();

        //Configuro path applicativi
        u.ConfiguraPercorsi();

        //setto funzione backup
        App.AppConsentiBackUp = true;
      }

      //remoto
      if ((bool)radioButtonArchivioRemoto.IsChecked)
      {
        //controllo presenza percorso remoto
        if (textBoxArchivioRemotoPath.Text.Trim().Length == 0)
        {
          App.ErrorLevel = App.ErrorTypes.Errore;
          RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
          m.TipoMessaggioErrore = WindowGestioneMessaggi.TipologieMessaggiErrore.MancaPercorsoArchivioRemoto;
          m.VisualizzaMessaggio();
          return;
        }

        //setto variabili globali
        App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Remoto;
        App.AppDataFolder = textBoxArchivioRemotoPath.Text;
        App.AppPathArchivioRemoto = App.AppDataFolder;

        //salvo nuova configurazione
        GestioneLicenza l = new GestioneLicenza();
        l.SalvaInfoDataUltimoUtilizzo();

        //Configuro path applicativi
        u.ConfiguraPercorsi();

        //setto funzione backup
        App.AppConsentiBackUp = true;
        if (App.Client)
        {
          App.AppConsentiBackUp = false;
        }

      }

      //interfaccia
      buttonApplica.IsEnabled = false;

      MasterFile.ForceRecreate();

      //ricarico main window
      ((MainWindow)(this.Owner)).ReloadMainWindow();

      MessageBox.Show("Procedura terminata con successo.");
    }


    private void btn_TrasferimentoArchivi_Click(object sender, RoutedEventArgs e)
    {

      //richiesta conferma
      Utilities u = new Utilities();
      if (MessageBoxResult.No == u.ConfermaTrasferimentoArchivio())
        return;

      //nuovo percorso
      string nuovaCartella = u.sys_OpenDirectoryDialog();
      if (nuovaCartella == "")
        return;


      //controllo percorso
      DirectoryInfo destinazione = new DirectoryInfo(nuovaCartella);
      if (!destinazione.Exists)
        return;

      DirectoryInfo origine = new DirectoryInfo(App.AppDataFolder);
      string tmpzipfile = App.AppTempFolder + "\\zip" + Guid.NewGuid().ToString();

      //Sposto archivio
      Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
      zip.AddDirectory(origine.FullName);
      zip.Save(tmpzipfile);
      zip.ExtractAll(destinazione.FullName, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);

      //Interfaccia
      textBoxArchivioRemotoPath.Text = destinazione.FullName;
      radioButtonArchivioRemoto.IsChecked = true;

      //Configuro applicazione
      App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Remoto;
      App.AppDataFolder = destinazione.FullName;
      App.AppPathArchivioRemoto = App.AppDataFolder;

      //salvo nuova configurazione
      GestioneLicenza l = new GestioneLicenza();
      l.SalvaInfoDataUltimoUtilizzo();

      //Configuro path applicativi
      u.ConfiguraPercorsi();


      MasterFile.ForceRecreate();

      //ricarico main window
      ((MainWindow)(this.Owner)).ReloadMainWindow();

      //interfaccia
      //modifile all'interfaccia dell'ultimo momento
      radioButtonArchivioLocale.IsEnabled = false;
      radioButtonArchivioRemoto.IsEnabled = false;
      buttonSelezionaArchivioRemoto.IsEnabled = false;
      buttonSelezionaArchivioRemoto.Visibility = System.Windows.Visibility.Collapsed;
      buttonApplica.IsEnabled = false;
      btn_TrasferimentoArchivi.IsEnabled = App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Locale;
      btn_TrasferimentoArchiviLocale.IsEnabled = App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Remoto;
      MessageBox.Show("Trasferimento archivio avvenuto con successo.");
    }

    private void btn_TrasferimentoArchiviLocale_Click(object sender, RoutedEventArgs e)
    {
      //richiesta conferma
      Utilities u = new Utilities();
      if (MessageBoxResult.No == u.ConfermaTrasferimentoArchivio())
        return;



      //controllo percorso
      DirectoryInfo origine = new DirectoryInfo(App.AppDataFolder);

      string tmpzipfile = App.AppTempFolder + "zip" + Guid.NewGuid().ToString();

      //Sposto archivio
      Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
      zip.AddDirectory(origine.FullName);
      zip.Save(tmpzipfile);

      //Interfaccia
      radioButtonArchivioLocale.IsChecked = true;

      //setto variabili globali
      App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Locale;

      //salvo nuova configurazione
      GestioneLicenza l = new GestioneLicenza();
      l.SalvaInfoDataUltimoUtilizzo();

      //Configuro path applicativi
      u.ConfiguraPercorsi();

      //trasferisco archivio
      DirectoryInfo destinazione = new DirectoryInfo(App.AppDataFolder);
      zip.ExtractAll(destinazione.FullName, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);



      MasterFile.ForceRecreate();

      //interfaccia
      //modifile all'interfaccia dell'ultimo momento
      radioButtonArchivioLocale.IsEnabled = false;
      radioButtonArchivioRemoto.IsEnabled = false;
      buttonSelezionaArchivioRemoto.IsEnabled = false;
      btn_TrasferimentoArchivi.IsEnabled = App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Locale;
      btn_TrasferimentoArchiviLocale.IsEnabled = App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Remoto;
      MessageBox.Show("Trasferimento archivio avvenuto con successo.");
    }



    private void btn_AggiornamentoArchivi_Click(object sender, RoutedEventArgs e)
    {
      //controll se archivio remoto
      if (App.AppSetupTipoGestioneArchivio != App.TipoGestioneArchivio.Remoto)
        MessageBox.Show("L'attuale configurazione non utilizza un archivio remoto.");

      //richiesta conferma
      Utilities u = new Utilities();
      if (MessageBoxResult.No == u.ConfermaAggiornamentoModelli())
        return;


      //4.6 spostato in utility
      ////Process wait - START
      //ProgressWindow pw = new ProgressWindow();

      ////Origine
      //string sourceFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + App.ApplicationFolder + "\\" + App.ApplicationFolder + "\\" + App.TemplateFolder;

      ////Destinazione
      //string destFolder = App.AppPathArchivioRemoto + "\\" + App.TemplateFolder;

      ////ORIGINE: Verifica esistenza
      //DirectoryInfo origine = new DirectoryInfo(sourceFolder);
      //if (!origine.Exists)
      //{
      //    MessageBox.Show("Cartella di origine non trovata.\n\n" + sourceFolder);
      //    return;
      //}

      ////DESTINAZIONE: Verifica esistenza
      //DirectoryInfo destinazione = new DirectoryInfo(destFolder);
      //if (!destinazione.Exists)
      //{
      //    MessageBox.Show("Cartella di destinazione non trovata.\n\n" + destFolder);
      //    return;
      //}

      ////Copio intero contenuto in cartella di destinazione
      //u.CopyFolderContent(origine, destinazione);


      ////andrea - v. 4.0
      //string destFlussi = App.AppPathArchivioRemoto + "\\" + App.UserFileFolder + "\\" + App.UserFileFlussiFolder;
      //if (!File.Exists(destFlussi))
      //    Directory.CreateDirectory(destFlussi);


      ////Process wait - STOP
      //pw.Close();


      //4.6
      if (u.AggiornaTemplateRemoto())
      {
        MasterFile.ForceRecreate();
        //interfaccia
        btn_AggiornamentoArchivi.IsEnabled = false;
        MessageBox.Show("Aggiornamento archivio avvenuto con successo.");
      }

    }


    private void btn_ScambiaArchivioLocaleRemoto_RemotoLocale_Click(object sender, RoutedEventArgs e)
    {

      if (App.AppPathArchivioRemoto == null || App.AppPathArchivioRemoto.ToString().Trim() == "")
      {
        MessageBox.Show("ATTENZIONE\nArchivio remoto non configurato.\nEseguire prima il trasferimento dell'archivio con apposita procedura.");
        return;
      }


      //richiesta conferma
      Utilities u = new Utilities();
      if (MessageBoxResult.Yes == u.ConfermaScambioArchivio())
      {

        //setto  archivio da  LOCALE A REMOTO
        if (App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.LocaleImportExport || App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Locale)
        {

          //setto variabile app
          App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Remoto;
          //setto funzione backup
          App.AppConsentiBackUp = true;
          if (App.Client)
          {
            App.AppConsentiBackUp = false;
          }
          //salvo nuova configurazione
          GestioneLicenza l = new GestioneLicenza();
          l.SalvaInfoDataUltimoUtilizzo();
          //Configuro path applicativi
          u.ConfiguraPercorsi();

        }
        //setto  archivio da  REMOTO A LOCALE 
        else if (App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Remoto)
        {

          //setto variabile app
          App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.LocaleImportExport;
          //setto funzione backup
          App.AppConsentiBackUp = true;
          //setto chiavi di registro
          //salvo nuova configurazione
          GestioneLicenza l = new GestioneLicenza();
          l.SalvaInfoDataUltimoUtilizzo();
          //Configuro path applicativi
          u.ConfiguraPercorsi();

        }

        MasterFile.ForceRecreate();
        //interfaccia
        ((MainWindow)(this.Owner)).ReloadMainWindow();
        //fine
        MessageBox.Show("Scambio archivio avvenuto con successo.");

        //4.6
        if (App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Remoto && !u.VerificaAggiornamentoTemplateRemoto())
        {
          if (u.AggiornaTemplateRemoto())
          {
            //interfaccia
            btn_AggiornamentoArchivi.IsEnabled = false;
            MessageBox.Show("Aggiornamento archivio avvenuto con successo.");
          }
        }
      }

    }


  }
}
