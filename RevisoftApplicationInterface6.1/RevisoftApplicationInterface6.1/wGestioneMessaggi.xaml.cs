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
using System.ComponentModel;

namespace RevisoftApplication
{

  public partial class WindowGestioneMessaggi : Window
  {
    //Variabili
    private static TipologieMessaggiSegnalazioni _TipoMessaggioSegnalazione;
    private static TipologieMessaggiAvvisi _TipoMessaggioAvviso;
    private static TipologieMessaggiErrore _TipoMessaggioErrore;
    private static TipologieMessaggiErroriBloccanti _TipoMessaggioErroreBloccante;
    private bool _ChuisuraRegolare;


    //Segnalazioni
    public enum TipologieMessaggiSegnalazioni
    {
      LicenzaProvaCreata = 301
    };

    //Avvisi
    public enum TipologieMessaggiAvvisi
    {
      CodiceMacchinaParziale = 201,
      LicenzaSigilloNonDisponibile = 202,
      LicenzaMultiplaNonDisponibile = 203,
    };

    //Errori
    public enum TipologieMessaggiErrore
    {
      MancaPercorsoArchivioRemoto = 101,
      ErroreInFileMaster = 102,
      ErroreInCaricamentoFileMaster = 103,
      ErroreInSalvataggioFileMaster = 104,
      ErroreInCancellazioneFileMaster = 105,
      ErroreInCaricamentoFileBackUp = 106,
      ErroreInSalvataggioFileBackUp = 107,
      FormatoFileErrato = 108,
      MancaPrintTemplate = 109,
      ErroreInSalvataggioMultiLicenza = 110,
      ErroreInLetturaMultiLicenza = 111,
      ErroreCodiceMacchinaImportazioneFile = 112,
      ErroreCodiceMacchinaSigillo = 113,
      ErroreLicenzaSigillo = 114,
    };

    //Errori bloccanti
    public enum TipologieMessaggiErroriBloccanti
    {
      MancaFileInfo = 1,
      MancaFileLicenza = 2,
      ErroreInFileInfo = 3,
      NoCodiceMacchina = 4,
      CheckInfoScadenza = 5,
      CheckInfoLicenza = 6,
      CheckInfoCodiceMacchina = 7,
      CheckInfoCodiceMacchinaServer = 8,
      CheckInfoDataAttivazione = 9,
      CheckInfoDurataLicenza = 10,
      CheckLicenzaScaduta = 11,
      CheckLicenzaUltimoUso = 12,
      CheckLicenzaCodiceMacchina = 13,
      FileNonTrovato = 14,
      CheckLicenzaFormatoErrato = 15,
      FileTemplateNonTrovato = 16,
      MancaFileMaster = 17,
      MancaFileBackUp = 18,
      CheckChiaveServerMasterFile = 19,
      CheckDataLicenzaProvaMasterFile = 20,
      CheckDataLicenzaMasterFile = 21,
      CheckDirittiAdmin = 22,
      CheckLimiteUtenti = 23,
    };


    //Proprietà
    public TipologieMessaggiSegnalazioni TipoMessaggioSegnalazione
    {
      get { return _TipoMessaggioSegnalazione; }
      set { _TipoMessaggioSegnalazione = value; }
    }
    public TipologieMessaggiAvvisi TipoMessaggioAvviso
    {
      get { return _TipoMessaggioAvviso; }
      set { _TipoMessaggioAvviso = value; }
    }
    public TipologieMessaggiErrore TipoMessaggioErrore
    {
      get { return _TipoMessaggioErrore; }
      set { _TipoMessaggioErrore = value; }
    }
    public TipologieMessaggiErroriBloccanti TipoMessaggioErroreBloccante
    {
      get { return _TipoMessaggioErroreBloccante; }
      set { _TipoMessaggioErroreBloccante = value; }
    }




    //Metodi ******************************************************************************************************

    //Costruttore
    public WindowGestioneMessaggi()
    {
      InitializeComponent();
    }

    public void VisualizzaMessaggio()
    {
      _ChuisuraRegolare = false;



      string xKey = "/RevisoftMessageFile";
      string errMsg = "Undefined";

      switch (App.ErrorLevel)
      {
        case App.ErrorTypes.Segnalazione:
          labelTitolo.Content = "Segnalazione";
          labelTitolo.Foreground = Brushes.Gray;
          xKey += "/Segnalazione/Nodo[@Key='" + _TipoMessaggioSegnalazione.ToString() + "']";
          errMsg = _TipoMessaggioSegnalazione.ToString();
          break;
        case App.ErrorTypes.Avviso:
          labelTitolo.Content = "Avviso";
          labelTitolo.Foreground = App._arrBrushes[0];
          xKey += "/Avvisi/Nodo[@Key='" + _TipoMessaggioAvviso.ToString() + "']";
          errMsg = _TipoMessaggioAvviso.ToString();
          break;
        case App.ErrorTypes.Errore:
          labelTitolo.Content = "Errore";
          labelTitolo.Foreground = Brushes.Red;
          xKey += "/Errori/Nodo[@Key='" + _TipoMessaggioErrore.ToString() + "']";
          errMsg = _TipoMessaggioErrore.ToString();
          break;
        case App.ErrorTypes.ErroreBloccante:
          labelTitolo.Content = "Errore Bloccante";
          labelTitolo.Foreground = Brushes.Red;
          xKey += "/ErroriBloccanti/Nodo[@Key='" + _TipoMessaggioErroreBloccante.ToString() + "']";
          errMsg = _TipoMessaggioErroreBloccante.ToString();
          break;
      }


      //andrea 2.9
      //Verifico presenza file messaggio
      if (!System.IO.File.Exists(App.AppMessageFile))
      {
        MessageBox.Show("Message File mancante. Installazione incompleta\n\n" + App.AppMessageFile + "\n\nErrore: " + errMsg);
        RevisoftApplication.Utilities u = new Utilities();
        u.ChiudiApplicazione();
      }



      //leggo file messaggi
      RevisoftApplication.XmlManager x = new XmlManager();
      XmlDocument f = new XmlDocument();
      x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
      f = x.LoadEncodedFile(App.AppMessageFile);
      XmlNode n = f.SelectSingleNode(xKey);

      //nodo mancante
      if (n == null)
      {
        //Abstract
        textBlockAbstract.Text = "Messaggio non trovato";
        //Descrizione
        textBlockDescrizione.Text = "Tipo: " + App.ErrorLevel.ToString() + "\nCodice errore: " + errMsg;
      }
      else
      {
        //Abstract
        textBlockAbstract.Text = n.Attributes["Titolo"].InnerText;
        //Descrizione
        textBlockDescrizione.Text = n.Attributes["Descrizione"].InnerText;
      }

      //apro finestra
      base.ShowDialog();
    }

    private void buttonRinnovo_Click(object sender, RoutedEventArgs e)
    {
      //Apertura maschera
      WindowGestioneLicenza w1 = new WindowGestioneLicenza();
      w1.ShowDialog();
    }


    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      _ChuisuraRegolare = true;

      base.Close();
      if (App.ErrorLevel == App.ErrorTypes.ErroreBloccante && App.Scaduta == false) //App.TipoLicenza != App.TipologieLicenze.Scaduta)
      {
        RevisoftApplication.Utilities u = new Utilities();
        u.ChiudiApplicazioneSuErrore();
      }
    }

    private void GestoreEvento_ChiusuraFinestra(object sender, CancelEventArgs e)
    {
      if (_ChuisuraRegolare)
        return;

      //intercetto chiusura finestra e forzo chiusura applicazione
      if (App.ErrorLevel == App.ErrorTypes.ErroreBloccante && App.Scaduta == false) //App.TipoLicenza != App.TipologieLicenze.Scaduta)
      {
        RevisoftApplication.Utilities u = new Utilities();
        u.ChiudiApplicazione();
      }

    }


  }
}
