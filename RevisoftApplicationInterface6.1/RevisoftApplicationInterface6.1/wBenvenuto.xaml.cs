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
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace RevisoftApplication
{


  public partial class wBenvenuto : Window
  {
    public const int GWL_STYLE = -16;
    public const int WS_SYSMENU = 0x80000;
    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    private bool _open;

    private BackgroundWorker bw = new BackgroundWorker();


    public wBenvenuto()
    {
      if (_open) { }
      InitializeComponent();
      label2.Foreground = App._arrBrushes[0];
      textBlockMessaggioScadenzaLicenza.Foreground = App._arrBrushes[0];
      label2_Copy.Foreground = App._arrBrushes[0];
      buttonAggiornaInSeguito.Visibility = System.Windows.Visibility.Hidden;
      //task in background - verifica aggiornamento
      bw.WorkerReportsProgress = true;
      bw.WorkerSupportsCancellation = true;
      bw.DoWork += new DoWorkEventHandler(bw_cloudCommunication);
      bw.ProgressChanged += new ProgressChangedEventHandler(bw_cloudCommunicationProgressChanged);
      bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_cloudCommunicationRunWorkerCompleted);

      //Interfaccia
      _open = true;
      checkBoxMostraAllAvvio.IsChecked = App.AppSetupBenvenuto;
      _open = false;

      textBlockMessaggioScadenzaLicenza.Visibility = System.Windows.Visibility.Collapsed;

      //3.6
      checkBoxMostraAllAvvio.Visibility = System.Windows.Visibility.Hidden;
      //aggiornamento software
      textAggiornamento.Visibility = System.Windows.Visibility.Hidden;
      buttonAggiornamento.Visibility = System.Windows.Visibility.Hidden;
      buttonChiudi.Visibility = System.Windows.Visibility.Hidden;
      textProgressCheck.Visibility = System.Windows.Visibility.Visible;
      ProgressDownload.Visibility = System.Windows.Visibility.Hidden;

      //visualizzo e ritardo
      base.Activate();
      //System.Threading.Thread.Sleep(500);

      //licenza in scadenza
      GestioneLicenza l = new GestioneLicenza();
      if (l.ScadenzaVicina)
      {
        textBlockMessaggioScadenzaLicenza.Text = "Attenzione, la sua licenza è prossima alla scadenza.\nRevisoft è attivo ancora per  " + l.GiorniAllaScadenza.ToString() + " giorni";
        textBlockMessaggioScadenzaLicenza.Visibility = System.Windows.Visibility.Visible;
      }

      //andrea - 4.7 - download inibito in remote desktop
      if (App.RemoteDesktop == true)
      {
        buttonAggiornamento.Visibility = System.Windows.Visibility.Hidden;
      }

      //lancio task in background
      if (bw.IsBusy != true)
      {
        bw.RunWorkerAsync();
      }

    }

    private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
      System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
      e.Handled = true;
    }

    void BenvenutoWindow_Closed(object sender, EventArgs e)
    {
      if (buttonChiudi.Visibility == System.Windows.Visibility.Hidden)
        ((System.ComponentModel.CancelEventArgs)e).Cancel = true;
    }

    private void buttonChiudi_Click_1(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

    private void checkBoxMostraAllAvvio_Check(object sender, RoutedEventArgs e)
    {
      //3.6
      //if (!_open)
      //{
      //    App.AppSetupBenvenuto = (bool)checkBoxMostraAllAvvio.IsChecked;
      //    RevisoftApplication.Utilities u = new Utilities();
      //    u.SetRegistroChiaveApplicazione(App.Registry_Benvenuto.ToString(), App.AppSetupBenvenuto.ToString());
      //}
    }

    private void buttonAggiorna_Click(object sender, RoutedEventArgs e)
    {
      //interfaccia
      buttonAggiornamento.Visibility = System.Windows.Visibility.Hidden;
      ProgressDownload.Visibility = System.Windows.Visibility.Visible;

      Utilities u = new Utilities();

      if (u.NomeNuovaVeresione != "")
      {
        WebClient webClient = new WebClient();
        webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(bw_DownloadRunWorkerCompleted);
        webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(bw_DownloadProgressChanged);
        webClient.DownloadFileAsync(new Uri(u.NomeNuovaVeresione), App.AppTempFolder + "\\" + u.NomeNuovaVeresione.Split('/').Last());
      }

    }


    private void bw_cloudCommunication(object sender, DoWorkEventArgs e)
    {
      BackgroundWorker worker = sender as BackgroundWorker;

      worker.ReportProgress(10);
      //System.Threading.Thread.Sleep(400);

      //3.6 - log ed aggiornamenti
      Utilities u = new Utilities();

      //MessageBox.Show("pre log");

      //invio log
      /*MM
       * bool result = u.InviaLog();

                  if (result == false)
                  {
                      throw new InvalidOperationException("Rete internet non disponibile.");
                  }
      */
      bool result = true;

      worker.ReportProgress(10);
      //System.Threading.Thread.Sleep(400);

      //MessageBox.Show("pre verifica");

      //verifica aggiornamenti

      // result = u.VerificaAggiornamenti();
      result = true;

      if (result == false)
      {
        throw new InvalidOperationException("Rete internet non disponibile.");
      }

      worker.ReportProgress(10);
      //System.Threading.Thread.Sleep(400);
    }

    private void bw_cloudCommunicationProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      textProgressCheck.Text += " .";
    }

    private void bw_cloudCommunicationRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      string testo = "Nessun aggiornamento rilevato.";
      //errore
      if (!(e.Error == null))
      {
        testo = e.Error.Message + " Impossibile verificare la presenza di aggiornamenti.";
        App.AppSetupNuovaVersione = false;
      }

      //interfaccia
      App.AppSetupNuovaVersione = false; // disabilita aggiornamento versione
      if (App.AppSetupNuovaVersione)
      {
        textVerificaAggiornamento.Visibility = System.Windows.Visibility.Hidden;
        textAggiornamento.Visibility = System.Windows.Visibility.Visible;
        buttonAggiornaInSeguito.Visibility = System.Windows.Visibility.Visible;
        buttonAggiornaInSeguito.Visibility = System.Windows.Visibility.Visible;
        buttonAggiornamento.Visibility = System.Windows.Visibility.Visible;
        textProgressCheck.Visibility = System.Windows.Visibility.Hidden;
        linkNoteRilascio.NavigateUri = new Uri(App.urlNoteRilascio);
      }
      else
      {
        textVerificaAggiornamento.Text = testo;
        textProgressCheck.Visibility = System.Windows.Visibility.Hidden;
        buttonAggiornaInSeguito.Visibility = System.Windows.Visibility.Hidden;
        buttonChiudi.Visibility = System.Windows.Visibility.Visible;
      }

    }


    private void bw_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
      ProgressDownload.Value = e.ProgressPercentage;

    }

    private void bw_DownloadRunWorkerCompleted(object sender, AsyncCompletedEventArgs e)
    {
      //errore
      if (!(e.Error == null))
      {
        MessageBox.Show("Errore nel processo di verifica aggiornamenti\n" + e.Error.Message);
      }

      Utilities u = new Utilities();

      //creo comando
      u.CreaComandoAggiornamento();

      //chiudo ed aggiorno
      u.ChiudiApplicazioneConAggiornamento(u.NomeComandoAggiornamento);

    }

    private void BenvenutoWindow_Closed(object sender, CancelEventArgs e)
    {

    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      var hwnd = new WindowInteropHelper(this).Handle;
      SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
    }
  }
}
