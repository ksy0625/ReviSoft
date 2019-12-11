using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Xml;
using System.Collections;
using UserControls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Data;

namespace RevisoftApplication
{

  public partial class WindowWorkAreaTree_ScambioDati : Window
  {
    public string SelectedTreeSource = "";
    public string SelectedDataSource = "";
    public string SelectedSessioneSource = "";

    private string cartellatmpImportazione = "";

    private string _cliente = "";
    private App.TipoScambioDati _tipo;
    private App.TipoAttivita _TipoAttivita = App.TipoAttivita.Sconosciuto;
    private bool firsttime = true;

    public string TitoloSessione = "";
    public string ImportFileName = "";
    private string importCodiceFiscale = "";
    private string importCliente = "";

    public string IDTree = "-1";
    public string IDCliente = "-1";
    public string IDSessione = "-1";

    XmlDataProviderManager _x;
    XmlDataProvider TreeXmlProvider;
    DataSet dsimport = null;

    XmlDocument xmlTMP = new XmlDocument();

    Hashtable htComboID = new Hashtable();

    public string Cliente
    {
      get
      {
        return _cliente;
      }
      set
      {
        _cliente = value;
        GeneraTitolo();
      }
    }

    public App.TipoAttivita TipoAttivita
    {
      get
      {
        return _TipoAttivita;
      }
      set
      {
        _TipoAttivita = value;
      }
    }

    public App.TipoScambioDati Tipo
    {
      get
      {
        return _tipo;
      }
      set
      {
        _tipo = value;
        switch (value)
        {
          case App.TipoScambioDati.Esporta:
            buttonEsporta.Visibility = System.Windows.Visibility.Visible;
            buttonImporta.Visibility = System.Windows.Visibility.Collapsed;
            gridSceltaSessione.Visibility = System.Windows.Visibility.Collapsed;
            labelSessione.Content = "Sessione da Esportare: " + TitoloSessione.Replace("01/01/", "");
            break;
          case App.TipoScambioDati.Importa:
            buttonEsporta.Visibility = System.Windows.Visibility.Collapsed;
            buttonImporta.Visibility = System.Windows.Visibility.Visible;
            gridSceltaSessione.Visibility = System.Windows.Visibility.Visible;
            labelSessione.Content = "Sessione da Importare: " + TitoloSessione.Replace("01/01/", "");
            break;
          default:
            break;
        }

        GeneraTitolo();
      }
    }




    private void GeneraTitolo()
    {
      txtTitoloRagioneSociale.Text = _cliente;
    }

    public WindowWorkAreaTree_ScambioDati()
    {
      InitializeComponent();

      //htComboID.Add(0, "-1");//Nuovo
      ((Label)(((Grid)txtTitoloRagioneSociale.Parent).Children[0])).Foreground = App._arrBrushes[0];
      ((Label)(((Grid)txtTitoloRagioneSociale.Parent).Children[1])).Foreground = App._arrBrushes[9];
      ((Label)(((Grid)txtTitoloRagioneSociale.Parent).Children[2])).Foreground = App._arrBrushes[9];
      txtTitoloRagioneSociale.Foreground = App._arrBrushes[9];
      labelAttivita.Content = "";

      MasterFile mf = MasterFile.Create();

      //string date = mf.GetData();

      //try
      //{
      //    if (Convert.ToDateTime(date) < DateTime.Now)
      //    {
      //        MessageBox.Show("Licenza scaduta");
      //        this.Close();
      //        return;
      //    }
      //}
      //catch (Exception ex)
      //{
      //    string log = ex.Message;
      //    this.Close();
      //    return;
      //}


      TreeXmlProvider = this.FindResource("xdpTree") as XmlDataProvider;
    }

    #region TreeDataSource

    private void SaveTreeSource()
    {
      if (TreeXmlProvider.Document != null)
      {
        RevisoftApplication.XmlManager x = new XmlManager();
        x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
        x.SaveEncodedFile(SelectedTreeSource, TreeXmlProvider.Document.OuterXml);
      }
    }

    public void LoadTreeSource()
    {


      RevisoftApplication.XmlManager x = new XmlManager();
      x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
      TreeXmlProvider.Document = cBusinessObjects.NewLoadEncodedFile(SelectedTreeSource, IDTree);
      cBusinessObjects.logger.Info(">>>> Passo2");
      if (firsttime)
      {
        firsttime = false;

        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("//Node"))
        {
          try
          {
            item.Attributes["Checked"].Value = "False";
          }
          catch (Exception ex)
          {
            string log = ex.Message;
            XmlAttribute attr = TreeXmlProvider.Document.CreateAttribute("Checked");
            item.Attributes.Append(attr);
            item.Attributes["Checked"].Value = "False";
          }

          item.Attributes["Expanded"].Value = "True";
          item.Attributes["Selected"].Value = "False";
        }
      }

      //interfaccia
      Utilities u = new Utilities();
      labelAttivita.Content = u.TitoloAttivita(_TipoAttivita);
      labelSessione.Content = (_tipo == App.TipoScambioDati.Importa ? "Sessione da Importare: " : "Sessione da Esportare: ") + TitoloSessione.Replace("01/01/", "");


      TreeXmlProvider.Refresh();
      LoadDataSource();



    }

    #endregion

    #region DataDataSource

    private void LoadDataSource()
    {
      ;
    }

    #endregion

    private void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      ;
    }

    private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      string SearchFor = ((TextBox)sender).Text.ToUpper();
      int foundID = -1;
      bool found = false;

      if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
      {
        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
        {
          if (item.Attributes["Selected"] != null)
          {
            if (item.Attributes["Selected"].Value == "True")
            {
              foundID = Convert.ToInt32(item.Attributes["ID"].Value);
            }

            item.Attributes["Selected"].Value = "False";
          }
        }

        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
        {
          if (found == false /*&& foundID != Convert.ToInt32(item.Attributes["ID"].Value)*/ && (item.Attributes["Titolo"].Value.ToUpper().Contains(SearchFor) || item.Attributes["Codice"].Value.ToUpper().Contains(SearchFor)))
          {
            found = true;
            item.Attributes["Selected"].Value = "True";

            if (item.ParentNode != null)
            {
              XmlNode parent = item.ParentNode;

              while (parent != null && parent.GetType().Name == "XmlElement" && parent.Name == "Node")
              {
                parent.Attributes["Expanded"].Value = "True";
                parent = parent.ParentNode;
              }
            }
          }
        }
      }

      if (found == false)
      {
        MessageBox.Show("Nessuna Carta di Lavoro presente per il testo ricercato");
      }
    }

    private void ItemsControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      ;
    }

    private void searchTextBox_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter || e.Key == Key.Tab)
      {
        string SearchFor = ((TextBox)sender).Text.ToUpper();
        int foundID = -1;
        bool found = false;

        if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
        {
          foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
          {
            if (item.Attributes["Selected"] != null)
            {
              if (item.Attributes["Selected"].Value == "True")
              {
                foundID = Convert.ToInt32(item.Attributes["ID"].Value);
              }

              item.Attributes["Selected"].Value = "False";
            }
          }

          foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
          {
            if (found == false /*&& foundID != Convert.ToInt32(item.Attributes["ID"].Value)*/ && (item.Attributes["Titolo"].Value.ToUpper().Contains(SearchFor) || item.Attributes["Codice"].Value.ToUpper().Contains(SearchFor)))
            {
              found = true;
              item.Attributes["Selected"].Value = "True";

              if (item.ParentNode != null)
              {
                XmlNode parent = item.ParentNode;

                while (parent != null && parent.GetType().Name == "XmlElement" && parent.Name == "Node")
                {
                  parent.Attributes["Expanded"].Value = "True";
                  parent = parent.ParentNode;
                }
              }
            }
          }
        }

        if (found == false)
        {
          MessageBox.Show("Nessuna Carta di Lavoro presente per il testo ricercato");
        }
      }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      ;
    }

    private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
    {
      ;
    }

    private void OnItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      ;
    }

    private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (grdMainContainer.Visibility == System.Windows.Visibility.Collapsed)
      {
        grdMainContainer.Visibility = System.Windows.Visibility.Visible;
        //brdSearch.Visibility = System.Windows.Visibility.Visible; *** andrea
        var uriSource = new Uri("./Images/icone/navigate_up.png", UriKind.Relative);
        ((Image)sender).Source = new BitmapImage(uriSource);
      }
      else
      {
        grdMainContainer.Visibility = System.Windows.Visibility.Collapsed;
        //brdSearch.Visibility = System.Windows.Visibility.Collapsed; *** andrea
        var uriSource = new Uri("./Images/icone/navigate_down.png", UriKind.Relative);
        ((Image)sender).Source = new BitmapImage(uriSource);
      }
    }

    private void buttonCreaNuova_Click(object sender, RoutedEventArgs e)
    {
      switch (_TipoAttivita)
      {
        case App.TipoAttivita.Incarico:
        case App.TipoAttivita.IncaricoCS:
        case App.TipoAttivita.IncaricoSU:
        case App.TipoAttivita.IncaricoREV:
          wSchedaIncarico wi = new wSchedaIncarico();
          wi.IDClienteImport = IDCliente;

          wi.TipologiaAttivita = App.TipoAttivitaScheda.New;
          wi.Riesame = false;
          wi.noopenaftercreate = true;
          wi.ConfiguraMaschera();
          wi.Owner = this;
          wi.ShowDialog();
          if (wi.IDIncarico != null && wi.IDIncarico.ToString() != "-1" && wi.IDIncarico.ToString() != "0")
          {
            Import(wi.IDIncarico.ToString());
            if (this.Owner != null && this.Owner.Owner != null)
            {
              this.Owner.Owner.Close();
            }
            wi.accedi(wi.IDIncarico.ToString(), false);
          }
          break;
        case App.TipoAttivita.ISQC:
          wSchedaISQC wiISQC = new wSchedaISQC();
          wiISQC.IDClienteImport = IDCliente;
          wiISQC.TipologiaAttivita = App.TipoAttivitaScheda.New;
          wiISQC.Riesame = false;
          wiISQC.noopenaftercreate = true;
          wiISQC.ConfiguraMaschera();
          wiISQC.Owner = this;
          wiISQC.ShowDialog();
          if (wiISQC.IDISQC != null && wiISQC.IDISQC.ToString() != "-1" && wiISQC.IDISQC.ToString() != "0")
          {
            Import(wiISQC.IDISQC.ToString());
            if (this.Owner != null && this.Owner.Owner != null)
            {
              this.Owner.Owner.Close();
            }
            wiISQC.accedi(wiISQC.IDISQC.ToString(), false);
          }
          break;
        case App.TipoAttivita.Revisione:
          wSchedaRevisione wr = new wSchedaRevisione();
          wr.IDClienteImport = IDCliente;
          wr.TipologiaAttivita = App.TipoAttivitaScheda.New;
          wr.ConfiguraMaschera();
          wr.noopenaftercreate = true;
          wr.Owner = this;
          wr.ShowDialog();
          if (wr.IDRevisione != null && wr.IDRevisione.ToString() != "-1" && wr.IDRevisione.ToString() != "0")
          {
            Import(wr.IDRevisione.ToString());
            if (this.Owner != null && this.Owner.Owner != null)
            {
              this.Owner.Owner.Close();
            }
            wr.accedi(wr.IDRevisione.ToString(), false);
          }
          break;

        case App.TipoAttivita.RelazioneB:
          wSchedaRelazioneB wrb = new wSchedaRelazioneB();
          wrb.IDClienteImport = IDCliente;
          wrb.TipologiaAttivita = App.TipoAttivitaScheda.New;
          wrb.ConfiguraMaschera();
          wrb.noopenaftercreate = true;
          wrb.Owner = this;
          wrb.ShowDialog();
          if (wrb.IDRelazioneB != null && wrb.IDRelazioneB.ToString() != "-1" && wrb.IDRelazioneB.ToString() != "0")
          {
            Import(wrb.IDRelazioneB.ToString());
            if (this.Owner != null && this.Owner.Owner != null)
            {
              this.Owner.Owner.Close();
            }
            wrb.accedi(wrb.IDRelazioneB.ToString(), false);
          }
          break;


        case App.TipoAttivita.RelazioneBC:
          wSchedaRelazioneBC wrbc = new wSchedaRelazioneBC();
          wrbc.IDClienteImport = IDCliente;
          wrbc.TipologiaAttivita = App.TipoAttivitaScheda.New;
          wrbc.ConfiguraMaschera();
          wrbc.noopenaftercreate = true;
          wrbc.Owner = this;
          wrbc.ShowDialog();
          if (wrbc.IDRelazioneBC != null && wrbc.IDRelazioneBC.ToString() != "-1" && wrbc.IDRelazioneBC.ToString() != "0")
          {
            Import(wrbc.IDRelazioneBC.ToString());
            if (this.Owner != null && this.Owner.Owner != null)
            {
              this.Owner.Owner.Close();
            }
            wrbc.accedi(wrbc.IDRelazioneBC.ToString(), false);
          }
          break;

        case App.TipoAttivita.RelazioneBV:
          wSchedaRelazioneBV wrbv = new wSchedaRelazioneBV();
          wrbv.IDClienteImport = IDCliente;
          wrbv.TipologiaAttivita = App.TipoAttivitaScheda.New;
          wrbv.ConfiguraMaschera();
          wrbv.noopenaftercreate = true;
          wrbv.Owner = this;
          wrbv.ShowDialog();
          if (wrbv.IDRelazioneBV != null && wrbv.IDRelazioneBV.ToString() != "-1" && wrbv.IDRelazioneBV.ToString() != "0")
          {
            Import(wrbv.IDRelazioneBV.ToString());
            if (this.Owner != null && this.Owner.Owner != null)
            {
              this.Owner.Owner.Close();
            }
            wrbv.accedi(wrbv.IDRelazioneBV.ToString(), false);
          }
          break;

        case App.TipoAttivita.RelazioneV:
          wSchedaRelazioneV wrv = new wSchedaRelazioneV();
          wrv.IDClienteImport = IDCliente;
          wrv.TipologiaAttivita = App.TipoAttivitaScheda.New;
          wrv.ConfiguraMaschera();
          wrv.noopenaftercreate = true;
          wrv.Owner = this;
          wrv.ShowDialog();
          if (wrv.IDRelazioneV != null && wrv.IDRelazioneV.ToString() != "-1" && wrv.IDRelazioneV.ToString() != "0")
          {
            Import(wrv.IDRelazioneV.ToString());
            if (this.Owner != null && this.Owner.Owner != null)
            {
              this.Owner.Owner.Close();
            }
            wrv.accedi(wrv.IDRelazioneV.ToString(), false);
          }
          break;


        case App.TipoAttivita.RelazioneVC:
          wSchedaRelazioneVC wrvc = new wSchedaRelazioneVC();
          wrvc.IDClienteImport = IDCliente;
          wrvc.TipologiaAttivita = App.TipoAttivitaScheda.New;
          wrvc.ConfiguraMaschera();
          wrvc.noopenaftercreate = true;
          wrvc.Owner = this;
          wrvc.ShowDialog();
          if (wrvc.IDRelazioneVC != null && wrvc.IDRelazioneVC.ToString() != "-1" && wrvc.IDRelazioneVC.ToString() != "0")
          {
            Import(wrvc.IDRelazioneVC.ToString());
            if (this.Owner != null && this.Owner.Owner != null)
            {
              this.Owner.Owner.Close();
            }
            wrvc.accedi(wrvc.IDRelazioneVC.ToString(), false);
          }
          break;

        case App.TipoAttivita.Bilancio:
          wSchedaBilancio wb = new wSchedaBilancio();
          wb.IDClienteImport = IDCliente;
          wb.TipologiaAttivita = App.TipoAttivitaScheda.New;
          wb.ConfiguraMaschera();
          wb.noopenaftercreate = true;
          wb.Owner = this;
          wb.ShowDialog();
          if (wb.IDBilancio != 0 && wb.IDBilancio.ToString() != "-1" && wb.IDBilancio.ToString() != "0")
          {
            Import(wb.IDBilancio.ToString());
            if (this.Owner != null && this.Owner.Owner != null)
            {
              this.Owner.Owner.Close();
            }
            wb.accedi(wb.IDBilancio.ToString(), false);
          }
          break;
        case App.TipoAttivita.Verifica:
          wSchedaVerifica wv = new wSchedaVerifica();
          wv.IDClienteImport = IDCliente;
          wv.TipologiaAttivita = App.TipoAttivitaScheda.New;
          wv.ConfiguraMaschera();
          wv.noopenaftercreate = true;
          wv.Owner = this;
          wv.ShowDialog();
          if (wv.IDVerifica != 0 && wv.IDVerifica.ToString() != "-1" && wv.IDVerifica.ToString() != "0")
          {
            Import(wv.IDVerifica.ToString());
            if (this.Owner != null && this.Owner.Owner != null)
            {
              this.Owner.Owner.Close();
            }
            wv.accedi(wv.IDVerifica.ToString(), false, true);
          }
          break;
        case App.TipoAttivita.Conclusione:
          wSchedaConclusioni wc = new wSchedaConclusioni();
          wc.IDClienteImport = IDCliente;
          wc.TipologiaAttivita = App.TipoAttivitaScheda.New;
          wc.ConfiguraMaschera();
          wc.noopenaftercreate = true;
          wc.Owner = this;
          wc.ShowDialog();
          if (wc.IDConclusione != 0 && wc.IDConclusione.ToString() != "-1" && wc.IDConclusione.ToString() != "0")
          {
            Import(wc.IDConclusione.ToString());
            if (this.Owner != null && this.Owner.Owner != null)
            {
              this.Owner.Owner.Close();
            }
            wc.accedi(wc.IDConclusione.ToString(), false);
          }
          break;
        case App.TipoAttivita.Vigilanza:
          wSchedaVigilanza wvv = new wSchedaVigilanza();
          wvv.IDClienteImport = IDCliente;
          wvv.TipologiaAttivita = App.TipoAttivitaScheda.New;
          wvv.ConfiguraMaschera();
          wvv.TipologiaAttivita = App.TipoAttivitaScheda.New;
          wvv.noopenaftercreate = true;
          wvv.Owner = this;
          wvv.ShowDialog();
          if (wvv.IDVigilanza != 0 && wvv.IDVigilanza.ToString() != "-1" && wvv.IDVigilanza.ToString() != "0")
          {
            Import(wvv.IDVigilanza.ToString());
            if (this.Owner != null && this.Owner.Owner != null)
            {
              this.Owner.Owner.Close();
            }
            wvv.accedi(wvv.IDVigilanza.ToString(), false, true);
          }
          break;
        case App.TipoAttivita.PianificazioniVerifica:
          wSchedaPianificazioniVerifica wpv = new wSchedaPianificazioniVerifica();
          wpv.IDClienteImport = IDCliente;
          wpv.TipologiaAttivita = App.TipoAttivitaScheda.New;
          wpv.ConfiguraMaschera();
          wpv.noopenaftercreate = true;
          wpv.Owner = this;
          wpv.ShowDialog();
          if (wpv.IDPianificazioniVerifica != 0 && wpv.IDPianificazioniVerifica.ToString() != "-1" && wpv.IDPianificazioniVerifica.ToString() != "0")
          {
            Import(wpv.IDPianificazioniVerifica.ToString());
            if (this.Owner != null && this.Owner.Owner != null)
            {
              this.Owner.Owner.Close();
            }
            wpv.accedi(wpv.IDPianificazioniVerifica.ToString(), false, true);
          }
          break;
        case App.TipoAttivita.PianificazioniVigilanza:
          wSchedaPianificazioniVigilanza wpvi = new wSchedaPianificazioniVigilanza();
          wpvi.IDClienteImport = IDCliente;
          wpvi.TipologiaAttivita = App.TipoAttivitaScheda.New;
          wpvi.ConfiguraMaschera();
          wpvi.noopenaftercreate = true;
          wpvi.Owner = this;
          wpvi.ShowDialog();
          if (wpvi.IDPianificazioniVigilanza != 0 && wpvi.IDPianificazioniVigilanza.ToString() != "-1" && wpvi.IDPianificazioniVigilanza.ToString() != "0")
          {
            Import(wpvi.IDPianificazioniVigilanza.ToString());
            if (this.Owner != null && this.Owner.Owner != null)
            {
              this.Owner.Owner.Close();
            }
            wpvi.accedi(wpvi.IDPianificazioniVigilanza.ToString(), false, true);
          }
          break;
        case App.TipoAttivita.Sconosciuto:
        default:
          break;
      }

      this.Close();
      //CaricaInfoFileDaImportare();
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

    private void buttonEsporta_Click(object sender, RoutedEventArgs e)
    {
      cBusinessObjects.show_workinprogress("Elaborazione in corso...");
      bool tobedone = false;
      foreach (XmlNode item in TreeXmlProvider.Document.SelectSingleNode("/Tree").SelectNodes("//Node"))
      {
        try
        {
          if (item.Attributes["Checked"].Value == "True")
          {
            tobedone = true;
          }
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wWorkAreaTree_ScambioDati.buttonEsporta_Click1 exception");
          string log = ex.Message;
        }
      }

      if (tobedone == false)
      {
        cBusinessObjects.hide_workinprogress();
        MessageBox.Show("Selezionare almeno una voce.", "Attenzione");
        return;
      }

      string titolo = "";

      if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
      {
        titolo = TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value;
      }

      if (titolo == "")
      {
        return;
      }

      Utilities u = new Utilities();

      string fnSuggerito = "Revisoft Condivisione dati (" + TitoloSessione.Replace("01/01/", "").Replace('/', '-') + " " + _cliente.Substring(0, _cliente.IndexOf('(')) + ")";

      bool nofilesaved = false;
      string ret = "";
      FileInfo fi = null;

      //    MessageBoxResult resultmsg = MessageBox.Show( "Vuoi copiare subito in un'altra sessione del cliente, anziché esportare il file?\n\nSpiegazione:\nSI: copia in altra sessione (presente o da creare) all'interno di questo software.\nNO: crea un file da esportare e copiare in un software Revisoft di altro utente.", "Attenzione", MessageBoxButton.YesNoCancel );

      //      if ( resultmsg == MessageBoxResult.Yes )
      //     {
      nofilesaved = true;
      ret = App.AppDataFolder + "\\tmpimportexport.rsdf";
      fi = new FileInfo(ret);
      if (fi.Exists)
      {
        fi.Delete();
      }
      /*   }
      else if ( resultmsg == MessageBoxResult.No )
      {
          ret = u.sys_SaveFileDialog(fnSuggerito, App.TipoFile.ScambioDati);

    if (ret == null || ret == "")
    {
      return;
    }
      }
      else
      {
          return;
      }
      */
      string cartellatmp = App.AppDataFolder + "\\" + Guid.NewGuid().ToString();
      DirectoryInfo di = new DirectoryInfo(cartellatmp);
      if (di.Exists)
      {
        //errore directory già esistente aspettare processo terminato da parte di altro utente
        return;
      }

      di.Create();

      string path_fileX = cartellatmp + "\\" + "tree.xml";
      FileInfo fileX = new FileInfo(path_fileX);

      _x = new XmlDataProviderManager(SelectedDataSource);

      RevisoftApplication.XmlManager x = new XmlManager();
      x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
      XmlDocument dataDoc = x.LoadEncodedFile(SelectedDataSource);


      XmlDataProviderManager _d = new XmlDataProviderManager(App.AppDocumentiDataFile, true);
      DataSet ds = new DataSet();
      DataTable dt = new DataTable();
      dt.Clear();
      dt.TableName = "DatiClienteSessioneAttivita";
      dt.Columns.Add("Attivita");
      dt.Columns.Add("IDSessione");
      dt.Columns.Add("IDCliente");
      dt.Columns.Add("CodiceFiscale");
      dt.Columns.Add("Cliente");
      dt.Columns.Add("Sessione");
      DataRow _ravi = dt.NewRow();
      _ravi["Attivita"] = Convert.ToInt32(_TipoAttivita).ToString();
      _ravi["IDSessione"] = IDSessione.ToString();
      _ravi["IDCliente"] = IDCliente.ToString();
      _ravi["CodiceFiscale"] = _cliente.Substring(_cliente.IndexOf("(C.F. ") + 6, _cliente.Length - (_cliente.IndexOf("(C.F. ") + 7));
      _ravi["Sessione"] = TitoloSessione.Replace("01/01/", "");
      _ravi["Cliente"] = _cliente.Substring(0, _cliente.IndexOf('(')).Trim().Replace("&", "&amp;").Replace("\"", "'");
      dt.Rows.Add(_ravi);
      ds.Tables.Add(dt);
      ds.Tables[ds.Tables.Count - 1].TableName = "DatiClienteSessioneAttivita";


      cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
      cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
      int tbnum = 0;
      foreach (XmlNode item in TreeXmlProvider.Document.SelectSingleNode("/Tree").SelectNodes("//Node"))
      {
        try
        {
          int id = int.Parse(item.Attributes["ID"].Value);
          if (item.Attributes["Checked"].Value == "True")
          {
            List<string> tableslist = cBusinessObjects.FindTablesById(id);
            foreach (string tb in tableslist)
            {
              string nomeclasse = "RevisoftApplication." + tb + ", RevisoftApplication";
              DataTable dati = cBusinessObjects.GetData(id, Type.GetType(cBusinessObjects.getfullnomeclass(tb)));
              ds.Tables.Add(dati);
              ds.Tables[ds.Tables.Count - 1].TableName = tb + "|" + tbnum.ToString();
              tbnum++;
            }

          }
        }
        catch (Exception ex)
        {
          cBusinessObjects.hide_workinprogress();
          cBusinessObjects.logger.Error(ex, "wWorkAreaTree_ScambioDati.buttonEsporta_Click2 exception");
          string log = ex.Message;
          item.ParentNode.RemoveChild(item);
        }
      }


      ds.WriteXml(path_fileX);


      //creo lo zip
      Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
      zip.Password = App.ZipFilePassword;

      zip.AddDirectory(di.FullName);
      zip.Save(ret);
      cBusinessObjects.xmldocument = TreeXmlProvider.Document;
      //Cancello i temporanei
      di.Delete(true);

      if (nofilesaved)
      {

        WindowWorkAreaTree_ScambioDati wWorkAreaSD = new WindowWorkAreaTree_ScambioDati();
        wWorkAreaSD.Owner = this;
        wWorkAreaSD.Tipo = App.TipoScambioDati.Importa;
        wWorkAreaSD.ImportFileName = ret;
        wWorkAreaSD.IDCliente = IDCliente;
        wWorkAreaSD.IDTree = IDTree;
        wWorkAreaSD.SelectedTreeSource = SelectedTreeSource;
        wWorkAreaSD.SelectedTreeSource = SelectedTreeSource;
        wWorkAreaSD.SelectedDataSource = SelectedDataSource;
        wWorkAreaSD.Cliente = _cliente;
        //andrea
        wWorkAreaSD.TipoAttivita = _TipoAttivita;
        wWorkAreaSD.IDSessione = IDSessione;
        wWorkAreaSD.CaricaInfoFileDaImportare();



        cBusinessObjects.hide_workinprogress();
        wWorkAreaSD.ShowDialog();

        try
        {
          fi.Delete();
        }
        catch (Exception ex)
        {
          cBusinessObjects.hide_workinprogress();
          cBusinessObjects.logger.Error(ex, "wWorkAreaTree_ScambioDati.buttonEsporta_Click3 exception");
          string log = ex.Message;
        }
      }
      else
      {
        cBusinessObjects.hide_workinprogress();
        MessageBox.Show("Salvataggio avvenuto con successo.");
      }

      base.Close();
    }

    private void buttonImporta_Click(object sender, RoutedEventArgs e)
    {

      if (comboSessioni.SelectedIndex == -1)
      {
        MessageBox.Show("Selezionare una Carta di Lavoro dal menu a tendina");
        return;
      }

      if (MessageBox.Show("Si è sicuri di voler sovrascrivere gli attuali dati con quelli che si stanno importando?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
      {
        return;
      }

      string IDSessioneTmp = htComboID[comboSessioni.SelectedIndex].ToString();
      cBusinessObjects.show_workinprogress("Elaborazione in corso ...");
      Import(IDSessioneTmp);
      cBusinessObjects.hide_workinprogress();
    }

    private void Import(string IDSessioneTmp)
    {
      MasterFile mf = MasterFile.Create();

      if (IDSessioneTmp == "-1")
      {
        //Nuovo

        //Controllo Cliente se esiste già
        ArrayList clienti = mf.GetAnagrafiche();

        string IDCliente = "-1";
        foreach (Hashtable item in clienti)
        {
          if (item["CodiceFiscale"].ToString() == importCodiceFiscale)
          {
            IDCliente = item["ID"].ToString();
            break;
          }
        }

        if (IDCliente == "-1")
        {
          //TBD
          //Controllo del numero massimo di clienti

          //Cliente non esiste ne creo uno nuovo
          Hashtable values = new Hashtable();

          values.Add("Note", "");
          values.Add("EsercizioAl", "");
          values.Add("EsercizioDal", "");
          values.Add("Esercizio", "");
          values.Add("CodiceFiscale", importCodiceFiscale);
          values.Add("PartitaIVA", "");
          values.Add("RagioneSociale", importCliente);

          IDCliente = mf.SetAnagrafica(values, -1).ToString();
        }

        Hashtable ht;

        switch (_TipoAttivita)
        {
          case App.TipoAttivita.Incarico:
          case App.TipoAttivita.IncaricoCS:
          case App.TipoAttivita.IncaricoREV:
          case App.TipoAttivita.IncaricoSU:

            //setto dati
            ht = new Hashtable();

            ht.Add("Cliente", IDCliente);
            ht.Add("DataNomina", TitoloSessione);
            ht.Add("Note", "");
            ht.Add("Composizione", (int)(App.TipoIncaricoComposizione.Sconosciuto));
            ht.Add("Attivita", (int)(App.TipoIncaricoAttivita.Sconosciuto));

            IDSessioneTmp = mf.SetIncarico(ht, Convert.ToInt32(IDSessioneTmp), Convert.ToInt32(IDCliente)).ToString();
         
            Hashtable htsessione = mf.GetIncarico(IDSessioneTmp);
            SelectedDataSource = htsessione["FileData"].ToString();
            break;
          case App.TipoAttivita.ISQC:
            //setto dati
            ht = new Hashtable();

            ht.Add("Cliente", IDCliente);
            ht.Add("DataNomina", TitoloSessione);
            ht.Add("Note", "");
            ht.Add("Composizione", (int)(App.TipoIncaricoComposizione.Sconosciuto));
            ht.Add("Attivita", (int)(App.TipoIncaricoAttivita.Sconosciuto));

            IDSessioneTmp = mf.SetISQC(ht, Convert.ToInt32(IDSessioneTmp), Convert.ToInt32(IDCliente)).ToString();

            Hashtable htsessioneISQC = mf.GetISQC(IDSessioneTmp);
            SelectedDataSource = htsessioneISQC["FileData"].ToString();
            break;
          case App.TipoAttivita.Revisione:
            ht = new Hashtable();

            ht.Add("Cliente", IDCliente);
            ht.Add("Data", TitoloSessione);
            ht.Add("Note", "");

            IDSessioneTmp = mf.SetRevisione(ht, Convert.ToInt32(IDSessioneTmp), Convert.ToInt32(IDCliente)).ToString();
            break;
          case App.TipoAttivita.Bilancio:
            ht = new Hashtable();

            ht.Add("Cliente", IDCliente);
            ht.Add("Data", TitoloSessione);
            ht.Add("Note", "");

            IDSessioneTmp = mf.SetBilancio(ht, Convert.ToInt32(IDSessioneTmp), Convert.ToInt32(IDCliente)).ToString();
            break;

          case App.TipoAttivita.RelazioneB:
            ht = new Hashtable();

            ht.Add("Cliente", IDCliente);
            ht.Add("Data", TitoloSessione);
            ht.Add("Note", "");

            IDSessioneTmp = mf.SetRelazioneB(ht, Convert.ToInt32(IDSessioneTmp), Convert.ToInt32(IDCliente)).ToString();
            break;

          case App.TipoAttivita.RelazioneV:
            ht = new Hashtable();

            ht.Add("Cliente", IDCliente);
            ht.Add("Data", TitoloSessione);
            ht.Add("Note", "");

            IDSessioneTmp = mf.SetRelazioneV(ht, Convert.ToInt32(IDSessioneTmp), Convert.ToInt32(IDCliente)).ToString();
            break;


          case App.TipoAttivita.RelazioneBC:
            ht = new Hashtable();

            ht.Add("Cliente", IDCliente);
            ht.Add("Data", TitoloSessione);
            ht.Add("Note", "");

            IDSessioneTmp = mf.SetRelazioneBC(ht, Convert.ToInt32(IDSessioneTmp), Convert.ToInt32(IDCliente)).ToString();
            break;

          case App.TipoAttivita.RelazioneVC:
            ht = new Hashtable();

            ht.Add("Cliente", IDCliente);
            ht.Add("Data", TitoloSessione);
            ht.Add("Note", "");

            IDSessioneTmp = mf.SetRelazioneVC(ht, Convert.ToInt32(IDSessioneTmp), Convert.ToInt32(IDCliente)).ToString();
            break;

          case App.TipoAttivita.RelazioneBV:
            ht = new Hashtable();

            ht.Add("Cliente", IDCliente);
            ht.Add("Data", TitoloSessione);
            ht.Add("Note", "");

            IDSessioneTmp = mf.SetRelazioneBV(ht, Convert.ToInt32(IDSessioneTmp), Convert.ToInt32(IDCliente)).ToString();
            break;

          case App.TipoAttivita.Conclusione:
            ht = new Hashtable();

            ht.Add("Cliente", IDCliente);
            ht.Add("Data", TitoloSessione);
            ht.Add("Note", "");

            IDSessioneTmp = mf.SetConclusione(ht, Convert.ToInt32(IDSessioneTmp), Convert.ToInt32(IDCliente)).ToString();
            break;
          case App.TipoAttivita.Verifica:
            ht = new Hashtable();
            ht.Add("Cliente", IDCliente);
            ht.Add("Data", TitoloSessione);
            ht.Add("DataEsecuzione", TitoloSessione);
            ht.Add("DataEsecuzione_Fine", TitoloSessione);
            ht.Add("Inizio", "");
            ht.Add("Fine", "");
            ht.Add("Luogo", "");
            ht.Add("Revisore", "");
            ht.Add("Presidente", "");
            ht.Add("Sindaco1", "");
            ht.Add("Sindaco2", "");
            ht.Add("Collaboratore", "");
            ht.Add("AssisitoDa", "");
            ht.Add("Composizione", (int)(App.TipoIncaricoComposizione.Sconosciuto));

            IDSessioneTmp = mf.SetVerifica(ht, Convert.ToInt32(IDSessioneTmp), Convert.ToInt32(IDCliente)).ToString();
            break;
          case App.TipoAttivita.Vigilanza:
            ht = new Hashtable();
            ht.Add("Cliente", IDCliente);
            ht.Add("Data", TitoloSessione);
            ht.Add("DataEsecuzione", TitoloSessione);
            ht.Add("DataEsecuzione_Fine", TitoloSessione);
            ht.Add("Inizio", "");
            ht.Add("Fine", "");
            ht.Add("Luogo", "");
            ht.Add("Revisore", "");
            ht.Add("Presidente", "");
            ht.Add("Sindaco1", "");
            ht.Add("Sindaco2", "");
            ht.Add("AssisitoDa", "");
            ht.Add("Composizione", (int)(App.TipoIncaricoComposizione.Sconosciuto));

            IDSessioneTmp = mf.SetVigilanza(ht, Convert.ToInt32(IDSessioneTmp), Convert.ToInt32(IDCliente)).ToString();
            break;
          case App.TipoAttivita.PianificazioniVerifica:
            ht = new Hashtable();
            ht.Add("Cliente", IDCliente);
            ht.Add("DataInizio", TitoloSessione);
            ht.Add("DataFine", TitoloSessione);
            IDSessioneTmp = mf.SetPianificazioniVerifica(ht, Convert.ToInt32(IDSessioneTmp), Convert.ToInt32(IDCliente)).ToString();
            break;
          case App.TipoAttivita.PianificazioniVigilanza:
            ht = new Hashtable();
            ht.Add("Cliente", IDCliente);
            ht.Add("DataInizio", TitoloSessione);
            ht.Add("DataFine", TitoloSessione);
            IDSessioneTmp = mf.SetPianificazioniVigilanza(ht, Convert.ToInt32(IDSessioneTmp), Convert.ToInt32(IDCliente)).ToString();
            break;
          case App.TipoAttivita.Sconosciuto:
          default:
            break;
        }
      }
      else
      {
        Hashtable htanagrafica = null;

        switch (_TipoAttivita)
        {
          case App.TipoAttivita.Incarico:
          case App.TipoAttivita.IncaricoSU:
          case App.TipoAttivita.IncaricoCS:
          case App.TipoAttivita.IncaricoREV:

            if (_TipoAttivita == App.TipoAttivita.Incarico)
              IDTree = (Convert.ToInt32(App.TipoFile.Incarico)).ToString();
            if (_TipoAttivita == App.TipoAttivita.IncaricoSU)
              IDTree = (Convert.ToInt32(App.TipoFile.IncaricoSU)).ToString();
            if (_TipoAttivita == App.TipoAttivita.IncaricoCS)
              IDTree = (Convert.ToInt32(App.TipoFile.IncaricoCS)).ToString();
            if (_TipoAttivita == App.TipoAttivita.IncaricoREV)
              IDTree = (Convert.ToInt32(App.TipoFile.IncaricoREV)).ToString();

            Hashtable htsessionei = mf.GetIncarico(IDSessioneTmp);

            htanagrafica = mf.GetAnagrafica(Convert.ToInt32(htsessionei["Cliente"].ToString()));

            if (htanagrafica["RagioneSociale"].ToString() != importCliente)
            {
              cBusinessObjects.hide_workinprogress();
              if (MessageBox.Show("Il cliente sul quale si sta scrivendo è il diverso da quello dal quale son stati condivisi i dati. CONTINUARE?", "ATTENZIONE", MessageBoxButton.YesNo) == MessageBoxResult.No)
              {
                return;
              }

            }
            if (htsessionei["FileData"] == null)
            {
              return;
            }

            if (htsessionei["Sigillo"] != null && htsessionei["Sigillo"].ToString() != "")
            {
              cBusinessObjects.hide_workinprogress();
              MessageBox.Show("Impossibile importare i dati. Alla sessione risulta essere applicato un Sigillo.");
              return;
            }

            SelectedDataSource = htsessionei["FileData"].ToString();
            IDCliente = htsessionei["Cliente"].ToString();
            break;
          case App.TipoAttivita.ISQC:
            IDTree = (Convert.ToInt32(App.TipoFile.ISQC)).ToString();
            Hashtable htsessioneiISQC = mf.GetISQC(IDSessioneTmp);

            htanagrafica = mf.GetAnagrafica(Convert.ToInt32(htsessioneiISQC["Cliente"].ToString()));

            if (htanagrafica["RagioneSociale"].ToString() != importCliente)
            {
              cBusinessObjects.hide_workinprogress();
              if (MessageBox.Show("Il cliente sul quale si sta scrivendo è il diverso da quello dal quale son stati condivisi i dati. CONTINUARE?", "ATTENZIONE", MessageBoxButton.YesNo) == MessageBoxResult.No)
              {
                return;
              }
            }
            if (htsessioneiISQC["FileData"] == null)
            {
              return;
            }

            if (htsessioneiISQC["Sigillo"] != null && htsessioneiISQC["Sigillo"].ToString() != "")
            {
              cBusinessObjects.hide_workinprogress();
              MessageBox.Show("Impossibile importare i dati. Alla sessione risulta essere applicato un Sigillo.");
              return;
            }

            SelectedDataSource = htsessioneiISQC["FileData"].ToString();
            IDCliente = htsessioneiISQC["Cliente"].ToString();
            break;
          case App.TipoAttivita.Revisione:
            IDTree = (Convert.ToInt32(App.TipoFile.Revisione)).ToString();
            Hashtable htsessioner = mf.GetRevisione(IDSessioneTmp);

            htanagrafica = mf.GetAnagrafica(Convert.ToInt32(htsessioner["Cliente"].ToString()));

            if (htanagrafica["RagioneSociale"].ToString() != importCliente)
            {
              cBusinessObjects.hide_workinprogress();
              if (MessageBox.Show("Il cliente sul quale si sta scrivendo è il diverso da quello dal quale son stati condivisi i dati. CONTINUARE?", "ATTENZIONE", MessageBoxButton.YesNo) == MessageBoxResult.No)
              {
                return;
              }
            }


            if (htsessioner["FileData"] == null)
            {
              return;
            }

            if (htsessioner["Sigillo"] != null && htsessioner["Sigillo"].ToString() != "")
            {
              cBusinessObjects.hide_workinprogress();
              MessageBox.Show("Impossibile importare i dati. Alla sessione risulta essere applicato un Sigillo.");
              return;
            }

            SelectedDataSource = htsessioner["FileData"].ToString();
            IDCliente = htsessioner["Cliente"].ToString();
            break;
          case App.TipoAttivita.Bilancio:
            IDTree = (Convert.ToInt32(App.TipoFile.Bilancio)).ToString();
            Hashtable htsessioneb = mf.GetBilancio(IDSessioneTmp);

            htanagrafica = mf.GetAnagrafica(Convert.ToInt32(htsessioneb["Cliente"].ToString()));

            if (htanagrafica["RagioneSociale"].ToString() != importCliente)
            {
              cBusinessObjects.hide_workinprogress();
              if (MessageBox.Show("Il cliente sul quale si sta scrivendo è il diverso da quello dal quale son stati condivisi i dati. CONTINUARE?", "ATTENZIONE", MessageBoxButton.YesNo) == MessageBoxResult.No)
              {
                return;
              }

            }


            if (htsessioneb["FileData"] == null)
            {
              return;
            }

            if (htsessioneb["Sigillo"] != null && htsessioneb["Sigillo"].ToString() != "")
            {
              cBusinessObjects.hide_workinprogress();
              MessageBox.Show("Impossibile importare i dati. Alla sessione risulta essere applicato un Sigillo.");
              return;
            }

            SelectedDataSource = htsessioneb["FileData"].ToString();
            IDCliente = htsessioneb["Cliente"].ToString();
            break;
          case App.TipoAttivita.Conclusione:
            IDTree = (Convert.ToInt32(App.TipoFile.Conclusione)).ToString();
            Hashtable htsessionec = mf.GetConclusione(IDSessioneTmp);

            htanagrafica = mf.GetAnagrafica(Convert.ToInt32(htsessionec["Cliente"].ToString()));

            if (htanagrafica["RagioneSociale"].ToString() != importCliente)
            {
              cBusinessObjects.hide_workinprogress();

              if (MessageBox.Show("Il cliente sul quale si sta scrivendo è il diverso da quello dal quale son stati condivisi i dati. CONTINUARE?", "ATTENZIONE", MessageBoxButton.YesNo) == MessageBoxResult.No)
              {
                return;
              }
            }
            if (htsessionec["Sigillo"] != null && htsessionec["Sigillo"].ToString() != "")
            {
              cBusinessObjects.hide_workinprogress();
              MessageBox.Show("Impossibile importare i dati. Alla sessione risulta essere applicato un Sigillo.");
              return;
            }

            if (htsessionec["FileData"] == null)
            {
              return;
            }

            SelectedDataSource = htsessionec["FileData"].ToString();
            IDCliente = htsessionec["Cliente"].ToString();
            break;

          case App.TipoAttivita.RelazioneB:
            IDTree = (Convert.ToInt32(App.TipoFile.RelazioneB)).ToString();
            Hashtable htsessionerb = mf.GetRelazioneB(IDSessioneTmp);

            htanagrafica = mf.GetAnagrafica(Convert.ToInt32(htsessionerb["Cliente"].ToString()));

            if (htanagrafica["RagioneSociale"].ToString() != importCliente)
            {
              cBusinessObjects.hide_workinprogress();
              if (MessageBox.Show("Il cliente sul quale si sta scrivendo è il diverso da quello dal quale son stati condivisi i dati. CONTINUARE?", "ATTENZIONE", MessageBoxButton.YesNo) == MessageBoxResult.No)
              {
                return;
              }
            }
            if (htsessionerb["FileData"] == null)
            {
              return;
            }

            if (htsessionerb["Sigillo"] != null && htsessionerb["Sigillo"].ToString() != "")
            {
              cBusinessObjects.hide_workinprogress();
              MessageBox.Show("Impossibile importare i dati. Alla sessione risulta essere applicato un Sigillo.");
              return;
            }

            SelectedDataSource = htsessionerb["FileData"].ToString();
            IDCliente = htsessionerb["Cliente"].ToString();
            break;

          case App.TipoAttivita.RelazioneV:
            IDTree = (Convert.ToInt32(App.TipoFile.RelazioneV)).ToString();
            Hashtable htsessionerv = mf.GetRelazioneV(IDSessioneTmp);

            htanagrafica = mf.GetAnagrafica(Convert.ToInt32(htsessionerv["Cliente"].ToString()));

            if (htanagrafica["RagioneSociale"].ToString() != importCliente)
            {
              cBusinessObjects.hide_workinprogress();
              if (MessageBox.Show("Il cliente sul quale si sta scrivendo è il diverso da quello dal quale son stati condivisi i dati. CONTINUARE?", "ATTENZIONE", MessageBoxButton.YesNo) == MessageBoxResult.No)
              {
                return;
              }
            }
            if (htsessionerv["FileData"] == null)
            {
              return;
            }

            if (htsessionerv["Sigillo"] != null && htsessionerv["Sigillo"].ToString() != "")
            {
              cBusinessObjects.hide_workinprogress();
              MessageBox.Show("Impossibile importare i dati. Alla sessione risulta essere applicato un Sigillo.");
              return;
            }

            SelectedDataSource = htsessionerv["FileData"].ToString();
            IDCliente = htsessionerv["Cliente"].ToString();
            break;





          case App.TipoAttivita.RelazioneBC:
            IDTree = (Convert.ToInt32(App.TipoFile.RelazioneBC)).ToString();
            Hashtable htsessionerbc = mf.GetRelazioneB(IDSessioneTmp);

            htanagrafica = mf.GetAnagrafica(Convert.ToInt32(htsessionerbc["Cliente"].ToString()));

            if (htanagrafica["RagioneSociale"].ToString() != importCliente)
            {
              cBusinessObjects.hide_workinprogress();
              if (MessageBox.Show("Il cliente sul quale si sta scrivendo è il diverso da quello dal quale son stati condivisi i dati. CONTINUARE?", "ATTENZIONE", MessageBoxButton.YesNo) == MessageBoxResult.No)
              {
                return;
              }
            }
            if (htsessionerbc["FileData"] == null)
            {
              return;
            }

            if (htsessionerbc["Sigillo"] != null && htsessionerbc["Sigillo"].ToString() != "")
            {
              cBusinessObjects.hide_workinprogress();
              MessageBox.Show("Impossibile importare i dati. Alla sessione risulta essere applicato un Sigillo.");
              return;
            }

            SelectedDataSource = htsessionerbc["FileData"].ToString();
            IDCliente = htsessionerbc["Cliente"].ToString();
            break;

          case App.TipoAttivita.RelazioneVC:
            IDTree = (Convert.ToInt32(App.TipoFile.RelazioneVC)).ToString();
            Hashtable htsessionervc = mf.GetRelazioneVC(IDSessioneTmp);

            htanagrafica = mf.GetAnagrafica(Convert.ToInt32(htsessionervc["Cliente"].ToString()));

            if (htanagrafica["RagioneSociale"].ToString() != importCliente)
            {
              cBusinessObjects.hide_workinprogress();
              if (MessageBox.Show("Il cliente sul quale si sta scrivendo è il diverso da quello dal quale son stati condivisi i dati. CONTINUARE?", "ATTENZIONE", MessageBoxButton.YesNo) == MessageBoxResult.No)
              {
                return;
              }
            }

            if (htsessionervc["FileData"] == null)
            {
              return;
            }

            if (htsessionervc["Sigillo"] != null && htsessionervc["Sigillo"].ToString() != "")
            {
              cBusinessObjects.hide_workinprogress();
              MessageBox.Show("Impossibile importare i dati. Alla sessione risulta essere applicato un Sigillo.");
              return;
            }

            SelectedDataSource = htsessionervc["FileData"].ToString();
            IDCliente = htsessionervc["Cliente"].ToString();
            break;






          case App.TipoAttivita.RelazioneBV:
            IDTree = (Convert.ToInt32(App.TipoFile.RelazioneBV)).ToString();
            Hashtable htsessionerbv = mf.GetRelazioneBV(IDSessioneTmp);

            htanagrafica = mf.GetAnagrafica(Convert.ToInt32(htsessionerbv["Cliente"].ToString()));

            if (htanagrafica["RagioneSociale"].ToString() != importCliente)
            {
              cBusinessObjects.hide_workinprogress();
              if (MessageBox.Show("Il cliente sul quale si sta scrivendo è il diverso da quello dal quale son stati condivisi i dati. CONTINUARE?", "ATTENZIONE", MessageBoxButton.YesNo) == MessageBoxResult.No)
              {
                return;
              }
            }

            if (htsessionerbv["FileData"] == null)
            {
              return;
            }

            if (htsessionerbv["Sigillo"] != null && htsessionerbv["Sigillo"].ToString() != "")
            {
              cBusinessObjects.hide_workinprogress();
              MessageBox.Show("Impossibile importare i dati. Alla sessione risulta essere applicato un Sigillo.");
              return;
            }

            SelectedDataSource = htsessionerbv["FileData"].ToString();
            IDCliente = htsessionerbv["Cliente"].ToString();
            break;

          case App.TipoAttivita.Verifica:
            IDTree = (Convert.ToInt32(App.TipoFile.Verifica)).ToString();
            Hashtable htsessionev = mf.GetVerifica(IDSessioneTmp);

            htanagrafica = mf.GetAnagrafica(Convert.ToInt32(htsessionev["Cliente"].ToString()));

            if (htanagrafica["RagioneSociale"].ToString() != importCliente)
            {
              cBusinessObjects.hide_workinprogress();
              if (MessageBox.Show("Il cliente sul quale si sta scrivendo è il diverso da quello dal quale son stati condivisi i dati. CONTINUARE?", "ATTENZIONE", MessageBoxButton.YesNo) == MessageBoxResult.No)
              {
                return;
              }
            }
            if (htsessionev["FileData"] == null)
            {
              return;
            }

            if (htsessionev["Sigillo"] != null && htsessionev["Sigillo"].ToString() != "")
            {
              cBusinessObjects.hide_workinprogress();
              MessageBox.Show("Impossibile importare i dati. Alla sessione risulta essere applicato un Sigillo.");
              return;
            }

            SelectedDataSource = htsessionev["FileData"].ToString();
            IDCliente = htsessionev["Cliente"].ToString();
            break;
          case App.TipoAttivita.Vigilanza:
            IDTree = (Convert.ToInt32(App.TipoFile.Vigilanza)).ToString();
            Hashtable htsessionea = mf.GetVigilanza(IDSessioneTmp);

            htanagrafica = mf.GetAnagrafica(Convert.ToInt32(htsessionea["Cliente"].ToString()));

            if (htanagrafica["RagioneSociale"].ToString() != importCliente)
            {
              cBusinessObjects.hide_workinprogress();
              if (MessageBox.Show("Il cliente sul quale si sta scrivendo è il diverso da quello dal quale son stati condivisi i dati. CONTINUARE?", "ATTENZIONE", MessageBoxButton.YesNo) == MessageBoxResult.No)
              {
                return;
              }
            }

            if (htsessionea["FileData"] == null)
            {
              return;
            }

            if (htsessionea["Sigillo"] != null && htsessionea["Sigillo"].ToString() != "")
            {
              cBusinessObjects.hide_workinprogress();
              MessageBox.Show("Impossibile importare i dati. Alla sessione risulta essere applicato un Sigillo.");
              return;
            }

            SelectedDataSource = htsessionea["FileData"].ToString();
            IDCliente = htsessionea["Cliente"].ToString();
            break;
          case App.TipoAttivita.PianificazioniVerifica:
            IDTree = (Convert.ToInt32(App.TipoFile.PianificazioniVerifica)).ToString();
            Hashtable htsessionepv = mf.GetPianificazioniVerifica(IDSessioneTmp);

            htanagrafica = mf.GetAnagrafica(Convert.ToInt32(htsessionepv["Cliente"].ToString()));

            if (htanagrafica["RagioneSociale"].ToString() != importCliente)
            {
              cBusinessObjects.hide_workinprogress();
              if (MessageBox.Show("Il cliente sul quale si sta scrivendo è il diverso da quello dal quale son stati condivisi i dati. CONTINUARE?", "ATTENZIONE", MessageBoxButton.YesNo) == MessageBoxResult.No)
              {
                return;
              }
            }

            if (htsessionepv["FileData"] == null)
            {
              return;
            }

            SelectedDataSource = htsessionepv["FileData"].ToString();
            IDCliente = htsessionepv["Cliente"].ToString();
            break;
          case App.TipoAttivita.PianificazioniVigilanza:
            IDTree = (Convert.ToInt32(App.TipoFile.PianificazioniVigilanza)).ToString();
            Hashtable htsessionepvi = mf.GetPianificazioniVigilanza(IDSessioneTmp);

            htanagrafica = mf.GetAnagrafica(Convert.ToInt32(htsessionepvi["Cliente"].ToString()));

            if (htanagrafica["RagioneSociale"].ToString() != importCliente)
            {
              cBusinessObjects.hide_workinprogress();
              if (MessageBox.Show("Il cliente sul quale si sta scrivendo è il diverso da quello dal quale son stati condivisi i dati. CONTINUARE?", "ATTENZIONE", MessageBoxButton.YesNo) == MessageBoxResult.No)
              {
                return;
              }
            }
            if (htsessionepvi["FileData"] == null)
            {
              return;
            }

            SelectedDataSource = htsessionepvi["FileData"].ToString();
            IDCliente = htsessionepvi["Cliente"].ToString();
            break;
          case App.TipoAttivita.Sconosciuto:
          default:
            IDTree = "-1";
            break;
        }
      }
      int oldIDCliente = cBusinessObjects.idcliente;
      int oldIDSessione = cBusinessObjects.idsessione;
      cBusinessObjects.idcliente = int.Parse(IDCliente);
      cBusinessObjects.idsessione = int.Parse(IDSessioneTmp);
      int lastid = 1;
      cBusinessObjects.DeleteTree("-1", IDCliente);
      DataTable tempdt = cBusinessObjects.ExecutesqlDataTable("SELECT MAX(ID) AS LASTID FROM ArchivioDocumenti");
      foreach (DataRow dd in tempdt.Rows)
      {
        if (dd["LASTID"].ToString() != "")
          lastid = int.Parse(dd["LASTID"].ToString()) + 1;
      }




      foreach (DataTable dt in dsimport.Tables)
      {
        if (dt.TableName.Contains("ArchivioDocumenti"))
        {
          for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dtrow = dt.Rows[i];
               if (dtrow["Tipo"].ToString() != (Convert.ToInt32(TipoDocumento.Permanente)).ToString())
                    dtrow.Delete();
            }
          dt.AcceptChanges();

          foreach (DataRow dtro in dt.Rows)
          {
          
            string oldfile = dtro["file"].ToString();
            dtro["file"] = dtro["file"].ToString().Replace(dtro["ID"].ToString(), lastid.ToString());
            File.Copy(App.AppDocumentiFolder + "\\" + oldfile, App.AppDocumentiFolder + "\\" + dtro["file"].ToString());
            dtro["ID"] = lastid;
            lastid++;

          }
        }


        if (dt.TableName == "DatiClienteSessioneAttivita")
          continue;


        int id = -1;
        foreach (DataRow dtrow in dt.Rows)
        {
          dtrow["ID_CLIENTE"] = cBusinessObjects.idcliente;
          dtrow["ID_SESSIONE"] = cBusinessObjects.idsessione;
          id = int.Parse(dtrow["ID_SCHEDA"].ToString());
        }
        string[] tokens = dt.TableName.Split('|');
        string nomeclasse = "RevisoftApplication." + tokens[0] + ", RevisoftApplication";

        cBusinessObjects.SaveData(0, dt, Type.GetType(cBusinessObjects.getfullnomeclass(tokens[0])), id);
      }



      cBusinessObjects.idcliente = oldIDCliente;
      cBusinessObjects.idsessione = oldIDSessione;


      /*MM MANCA DI VERIFICARE LA QUESTIONE DOCUMENTI
                  _x = new XmlDataProviderManager( App.AppDataDataFolder + "\\" + SelectedDataSource );

                  RevisoftApplication.XmlManager x = new XmlManager();
                  x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                  XmlDocument dataDoc = x.LoadEncodedFile( App.AppDataDataFolder + "\\" + SelectedDataSource );

                  XmlDataProviderManager _d = new XmlDataProviderManager( App.AppDocumentiDataFile, true );

                  bool tobesaved = false;

                  foreach ( XmlNode item in TreeXmlProvider.Document.SelectSingleNode( "/Tree" ).SelectNodes( "//Node" ) )
                  {
                      if ( item.Attributes["Checked"] != null && item.Attributes["Checked"].Value == "True" && xmlTMP.SelectSingleNode( "//Dato[@ID='" + item.Attributes["ID"].Value + "']" ) != null )
                      {
                          XmlNode removable = dataDoc.SelectSingleNode( "/Dati//Dato[@ID='" + item.Attributes["ID"].Value + "']" );
                          XmlNode imported = dataDoc.ImportNode( xmlTMP.SelectSingleNode( "//Dato[@ID='" + item.Attributes["ID"].Value + "']" ), true );
                          if ( removable != null )
                          {
                              removable.ParentNode.ReplaceChild( imported, removable );
                          }
                          else
                          {
                              dataDoc.SelectSingleNode( "/Dati" ).AppendChild( imported );
                          }

                          //eliminazione di tutti i vacchi documenti
                          XmlNode root = _d.Document.SelectSingleNode( "//DOCUMENTI" );
                          foreach ( XmlNode documento in root.SelectNodes( "//DOCUMENTO[@Nodo='" + item.Attributes["ID"].Value + "'][@Sessione='" + IDSessioneTmp + "'][@Cliente='" + IDCliente + "'][@Tree='" + IDTree + "']") )
                          {
                              FileInfo ff = new FileInfo( App.AppDocumentiFolder + "\\" + documento.Attributes["File"].Value );
                              if ( ff.Exists )
                              {
                                  ff.Delete();
                              }

                              documento.ParentNode.RemoveChild( documento );
                          }

                          //inserimento di tutti i nuovi documenti
                          foreach ( XmlNode documento in xmlTMP.SelectNodes( "//DOCUMENTO[@Nodo='" + item.Attributes["ID"].Value + "']" ) )
                          {
                              int newID = Convert.ToInt32( root.Attributes["LastID"].Value ) + 1;
                              FileInfo ff = new FileInfo( cartellatmpImportazione + "\\" + documento.Attributes["File"].Value );

                              if ( ff.Exists )
                              {
                                  string nomefile = newID.ToString() + "." + documento.Attributes["File"].Value.Split( '.' ).Last();
                                  ff.CopyTo( App.AppDocumentiFolder + "\\" + nomefile, true );

                                  string xml = "<DOCUMENTO ID=\"" + newID.ToString() + "\" Cliente=\"" + IDCliente + "\" Sessione=\"" + IDSessioneTmp + "\" Tree=\"" + IDTree + "\" Nodo=\"" + item.Attributes["ID"].Value + "\" Tipo=\"" + documento.Attributes["Tipo"].Value + "\" Titolo=\"" + documento.Attributes["Titolo"].Value.Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" Descrizione=\"" + documento.Attributes["Descrizione"].Value.Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" File=\"" + nomefile + "\" Visualizza=\"True\" />";

                                  XmlDocument doctmp = new XmlDocument();
                                  doctmp.LoadXml( xml );
                                  XmlNode tmpNode = doctmp.SelectSingleNode( "/DOCUMENTO" );

                                  XmlNode node = _d.Document.ImportNode( tmpNode, true );

                                  root.AppendChild( node );
                                  root.Attributes["LastID"].Value = newID.ToString();

                                  tobesaved = true;
                              }
                          }
                      }
                  }

                  if ( tobesaved )
                  {
                      _d.Save();
                  }

                  x.SaveEncodedFile( App.AppDataDataFolder + "\\" + SelectedDataSource, dataDoc.OuterXml );
      */

      DirectoryInfo di = new DirectoryInfo(cartellatmpImportazione);
      if (di.Exists)
      {
        di.Delete(true);
      }

      foreach (var Window in App.Current.Windows)
      {
        if (Window.GetType().Name == "WindowWorkAreaTree")
        {
          ((RevisoftApplication.WindowWorkAreaTree)Window).Close();
        }
      }
      cBusinessObjects.DeleteTree("-1", IDCliente); // NB CANCELLO CACHE DOPO CHE HO CHIUSO LA FINESTRA PERCHE NELLA CHIUSURA SALVA LA CACHE
      cBusinessObjects.hide_workinprogress();
      MessageBox.Show("Importazione avvenuta con successo,");
      base.Close();

      //if ( MessageBox.Show( "Importazione avvenuta con successo. Si vuole importare su un'altra sessione?", "Attenzione", MessageBoxButton.YesNo ) == MessageBoxResult.No )
      //{
      //    base.Close();
      //}	
    }

    private string ConvertDate(string date)
    {

      date = date.ToString().Replace("01/01/", "");

      date = date.ToString().Contains("31/12/") ? date.ToString().Replace("31/12/", "") + " / " + (Convert.ToInt32(date.ToString().Replace("31/12/", "")) + 1).ToString() : date;

      return date;
    }

    public void CaricaInfoFileDaImportare()
    {
      //esco se manca file
      //if (ImportFileName == null || ImportFileName == "")
      //{
      //    base.Close();
      //}

      //versione 3.0 - verifico formato path e converto in UNC
      if (ImportFileName.IndexOf(':') == 1)
      {
        RevisoftApplication.Utilities u2 = new Utilities();
        ImportFileName = u2.GetRealPathFile(ImportFileName);
      }


      cartellatmpImportazione = App.AppTempFolder + "\\" + Guid.NewGuid().ToString();
      DirectoryInfo di = new DirectoryInfo(cartellatmpImportazione);
      if (di.Exists)
      {
        //errore directory già esistente aspettare processo terminato da parte di altro utente
        return;
      }

      Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(ImportFileName);
      zip.Password = App.ZipFilePassword;

      zip.ExtractAll(cartellatmpImportazione);

      WindowGestioneMessaggi m = new WindowGestioneMessaggi();

      //	xmlTMP.Load(cartellatmpImportazione + "\\tree.xml");
      //    XmlNode xNode = xmlTMP.SelectSingleNode("/ROOT/REVISOFT");
      dsimport = new DataSet();
      dsimport.ReadXml(cartellatmpImportazione + "\\tree.xml");

      //controllo formato file
      if (dsimport.Tables.Count == 0)
      {
        //mancanza di corrispondenza chiave server / masterfile
        App.ErrorLevel = App.ErrorTypes.Errore;
        m.TipoMessaggioErrore = WindowGestioneMessaggi.TipologieMessaggiErrore.FormatoFileErrato;
        m.VisualizzaMessaggio();
        //esco da importazione
        base.Close();
      }


      foreach (DataRow dtrow in dsimport.Tables["DatiClienteSessioneAttivita"].Rows)
      {
        //attività
        _TipoAttivita = (App.TipoAttivita)(Convert.ToInt32(dtrow["Attivita"].ToString()));
        //sessione
        TitoloSessione = dtrow["Sessione"].ToString();
        //Cliente
        importCliente = dtrow["Cliente"].ToString();
        //Codice fiscale
        importCodiceFiscale = dtrow["CodiceFiscale"].ToString();
        //cliente completo
        _cliente = importCliente + " (C.F. " + importCodiceFiscale + ")";
      }


      //interfaccia
      RevisoftApplication.Utilities u = new Utilities();
      labelAttivita.Content = u.TitoloAttivita(_TipoAttivita);
      labelSessione.Content = "Sessione da Importare: " + TitoloSessione.Replace("01/01/", "");
      txtTitoloRagioneSociale.Text = _cliente;

      MasterFile mf = MasterFile.Create();
      ArrayList clienti = mf.GetAnagrafiche();

      int index = -1;

      htComboID.Clear();
      comboSessioni.Items.Clear();
      string versionesessione = "";
      switch (_TipoAttivita)
      {
        case App.TipoAttivita.Incarico:
        case App.TipoAttivita.IncaricoSU:
        case App.TipoAttivita.IncaricoCS:
        case App.TipoAttivita.IncaricoREV:


          LoadTreeSource();
          string strincarico = "Incarico";
          if(_TipoAttivita== App.TipoAttivita.IncaricoSU)
             strincarico = "Incarico";
         if(_TipoAttivita== App.TipoAttivita.IncaricoCS)
             strincarico = "IncaricoCS";
         if(_TipoAttivita== App.TipoAttivita.IncaricoREV)
             strincarico = "IncaricoREV";
          versionesessione = cBusinessObjects.GetVersioneSessione(IDSessione,strincarico);
          foreach (Hashtable item in clienti)
          {
            //if ( IDCliente == "-1" || item["ID"].ToString() == IDCliente )
            {
              string area1 = "";
              if (IDTree == "71")
                area1 = "CS";
              if (IDTree == "72")
                area1 = "SU";
              if (IDTree == "73")
                area1 = "REV";
              ArrayList lista = mf.GetIncarichi(item["ID"].ToString(), area1);
              foreach (Hashtable item2 in lista)
              {
                if (item2["ID"].ToString() != IDSessione && versionesessione==cBusinessObjects.GetVersioneSessione(item2["ID"].ToString(),strincarico))
                {
                  htComboID.Add(++index, item2["ID"].ToString());
                  comboSessioni.Items.Add(item["RagioneSociale"].ToString() + " - " + u.TitoloAttivita(_TipoAttivita) + " - Sessione del " + ConvertDate(item2["DataNomina"].ToString()));
                }
              }
            }
          }
          break;
        case App.TipoAttivita.ISQC:
          SelectedTreeSource = App.AppTemplateTreeISQC;
          LoadTreeSource();
          versionesessione = cBusinessObjects.GetVersioneSessione(IDSessione,"ISQC");
          foreach (Hashtable item in clienti)
          {
            //if (IDCliente == "-1" || item["ID"].ToString() == IDCliente)
            {
              ArrayList lista = mf.GetISQCs(item["ID"].ToString());
              foreach (Hashtable item2 in lista)
              {
                if (item2["ID"].ToString() != IDSessione && versionesessione==cBusinessObjects.GetVersioneSessione(item2["ID"].ToString(),"ISQC"))
                {
                  htComboID.Add(++index, item2["ID"].ToString());
                  comboSessioni.Items.Add(item["RagioneSociale"].ToString() + " - " + u.TitoloAttivita(_TipoAttivita) + " - Sessione del " + ConvertDate(item2["DataNomina"].ToString()));
                  //comboSessioni.Items.Add("Sessione del " + ConvertDate(item2["DataNomina"].ToString()) + " (" + u.TitoloAttivita(_TipoAttivita) + " - " + item["RagioneSociale"].ToString() + ")");
                }
              }
            }
          }
          break;
        case App.TipoAttivita.Revisione:
          SelectedTreeSource = App.AppTemplateTreeRevisione;
          LoadTreeSource();
          versionesessione = cBusinessObjects.GetVersioneSessione(IDSessione,"Revisione");
          foreach (Hashtable item in clienti)
          {
            //if ( IDCliente == "-1" || item["ID"].ToString() == IDCliente )
            {
              ArrayList lista = mf.GetRevisioni(item["ID"].ToString());
              foreach (Hashtable item2 in lista)
              {
                if (item2["ID"].ToString() != IDSessione && versionesessione==cBusinessObjects.GetVersioneSessione(item2["ID"].ToString(),"Revisione"))
                {
                  htComboID.Add(++index, item2["ID"].ToString());
                  comboSessioni.Items.Add(item["RagioneSociale"].ToString() + " - " + u.TitoloAttivita(_TipoAttivita) + " - Sessione del " + ConvertDate(item2["Data"].ToString()).Replace("01/01/", ""));
                  //comboSessioni.Items.Add( "Sessione del " + ConvertDate( item2["Data"].ToString() ).Replace("01/01/", "") + " (" + u.TitoloAttivita( _TipoAttivita ) + " - " + item["RagioneSociale"].ToString() + ")" );
                }
              }
            }
          }
          break;
        case App.TipoAttivita.Bilancio:
          SelectedTreeSource = App.AppTemplateTreeBilancio;
          LoadTreeSource();
          versionesessione = cBusinessObjects.GetVersioneSessione(IDSessione,"Bilancio");
          foreach (Hashtable item in clienti)
          {
            //if ( IDCliente == "-1" || item["ID"].ToString() == IDCliente )
            {
              ArrayList lista = mf.GetBilanci(item["ID"].ToString());
              foreach (Hashtable item2 in lista)
              {
                if (item2["ID"].ToString() != IDSessione && versionesessione==cBusinessObjects.GetVersioneSessione(item2["ID"].ToString(),"Bilancio"))
                {
                  htComboID.Add(++index, item2["ID"].ToString());
                  comboSessioni.Items.Add(item["RagioneSociale"].ToString() + " - " + u.TitoloAttivita(_TipoAttivita) + " - Sessione del " + ConvertDate(item2["Data"].ToString()).Replace("01/01/", ""));
                  //comboSessioni.Items.Add( "Sessione del " + ConvertDate( item2["Data"].ToString() ).Replace( "01/01/", "" ) + " (" + u.TitoloAttivita( _TipoAttivita ) + " - " + item["RagioneSociale"].ToString() + ")" );
                }
              }
            }
          }
          break;
        case App.TipoAttivita.Conclusione:
          SelectedTreeSource = App.AppTemplateTreeConclusione;
          LoadTreeSource();
          versionesessione = cBusinessObjects.GetVersioneSessione(IDSessione,"Conclusione");

          foreach (Hashtable item in clienti)
          {
            //if ( IDCliente == "-1" || item["ID"].ToString() == IDCliente )
            {
              ArrayList lista = mf.GetConclusioni(item["ID"].ToString());
              foreach (Hashtable item2 in lista)
              {
                if (item2["ID"].ToString() != IDSessione && versionesessione==cBusinessObjects.GetVersioneSessione(item2["ID"].ToString(),"Conclusione"))
                {
                  htComboID.Add(++index, item2["ID"].ToString());
                  comboSessioni.Items.Add(item["RagioneSociale"].ToString() + " - " + u.TitoloAttivita(_TipoAttivita) + " - Sessione del " + ConvertDate(item2["Data"].ToString()).Replace("01/01/", ""));
                  //comboSessioni.Items.Add( "Sessione del " + ConvertDate( item2["Data"].ToString() ).Replace( "01/01/", "" ) + " (" + u.TitoloAttivita( _TipoAttivita ) + " - " + item["RagioneSociale"].ToString() + ")" );
                }
              }
            }
          }
          break;


        case App.TipoAttivita.RelazioneB:
          SelectedTreeSource = App.AppTemplateTreeRelazioneB;
          LoadTreeSource();
          versionesessione = cBusinessObjects.GetVersioneSessione(IDSessione,"RelazioneB");

          foreach (Hashtable item in clienti)
          {
            //if ( IDCliente == "-1" || item["ID"].ToString() == IDCliente )
            {
              ArrayList lista = mf.GetRelazioniB(item["ID"].ToString());
              foreach (Hashtable item2 in lista)
              {
                if (item2["ID"].ToString() != IDSessione && versionesessione==cBusinessObjects.GetVersioneSessione(item2["ID"].ToString(),"RelazioneB"))
                {
                  htComboID.Add(++index, item2["ID"].ToString());
                  comboSessioni.Items.Add(item["RagioneSociale"].ToString() + " - " + u.TitoloAttivita(_TipoAttivita) + " - Sessione del " + ConvertDate(item2["Data"].ToString()).Replace("01/01/", ""));
                  //comboSessioni.Items.Add( "Sessione del " + ConvertDate( item2["Data"].ToString() ) + " (" + u.TitoloAttivita( _TipoAttivita ) + " - " + item["RagioneSociale"].ToString() + ")" );
                }
              }
            }
          }
          break;

        case App.TipoAttivita.RelazioneV:
          SelectedTreeSource = App.AppTemplateTreeRelazioneV;
          LoadTreeSource();
          versionesessione = cBusinessObjects.GetVersioneSessione(IDSessione,"RelazioneV");


          foreach (Hashtable item in clienti)
          {
            //if ( IDCliente == "-1" || item["ID"].ToString() == IDCliente )
            {
              ArrayList lista = mf.GetRelazioniV(item["ID"].ToString());
              foreach (Hashtable item2 in lista)
              {
                if (item2["ID"].ToString() != IDSessione && versionesessione==cBusinessObjects.GetVersioneSessione(item2["ID"].ToString(),"RelazioneV"))
                {
                  htComboID.Add(++index, item2["ID"].ToString());
                  comboSessioni.Items.Add(item["RagioneSociale"].ToString() + " - " + u.TitoloAttivita(_TipoAttivita) + " - Sessione del " + ConvertDate(item2["Data"].ToString()).Replace("01/01/", ""));
                  //comboSessioni.Items.Add( "Sessione del " + ConvertDate( item2["Data"].ToString() ) + " (" + u.TitoloAttivita( _TipoAttivita ) + " - " + item["RagioneSociale"].ToString() + ")" );
                }
              }
            }
          }
          break;


        case App.TipoAttivita.RelazioneBC:
          SelectedTreeSource = App.AppTemplateTreeRelazioneBC;
          LoadTreeSource();
          versionesessione = cBusinessObjects.GetVersioneSessione(IDSessione,"RelazioneBC");

          foreach (Hashtable item in clienti)
          {
            //if ( IDCliente == "-1" || item["ID"].ToString() == IDCliente )
            {
              ArrayList lista = mf.GetRelazioniBC(item["ID"].ToString());
              foreach (Hashtable item2 in lista)
              {
                if (item2["ID"].ToString() != IDSessione && versionesessione==cBusinessObjects.GetVersioneSessione(item2["ID"].ToString(),"RelazioneBC"))
                {
                  htComboID.Add(++index, item2["ID"].ToString());
                  comboSessioni.Items.Add(item["RagioneSociale"].ToString() + " - " + u.TitoloAttivita(_TipoAttivita) + " - Sessione del " + ConvertDate(item2["Data"].ToString()).Replace("01/01/", ""));
                  //comboSessioni.Items.Add( "Sessione del " + ConvertDate( item2["Data"].ToString() ) + " (" + u.TitoloAttivita( _TipoAttivita ) + " - " + item["RagioneSociale"].ToString() + ")" );
                }
              }
            }
          }
          break;

        case App.TipoAttivita.RelazioneVC:
          SelectedTreeSource = App.AppTemplateTreeRelazioneVC;
          LoadTreeSource();
          versionesessione = cBusinessObjects.GetVersioneSessione(IDSessione,"RelazioneVC");
          foreach (Hashtable item in clienti)
          {
            //if ( IDCliente == "-1" || item["ID"].ToString() == IDCliente )
            {
              ArrayList lista = mf.GetRelazioniVC(item["ID"].ToString());
              foreach (Hashtable item2 in lista)
              {
                if (item2["ID"].ToString() != IDSessione && versionesessione==cBusinessObjects.GetVersioneSessione(item2["ID"].ToString(),"RelazioneVC"))
                {
                  htComboID.Add(++index, item2["ID"].ToString());
                  comboSessioni.Items.Add(item["RagioneSociale"].ToString() + " - " + u.TitoloAttivita(_TipoAttivita) + " - Sessione del " + ConvertDate(item2["Data"].ToString()).Replace("01/01/", ""));
                  //comboSessioni.Items.Add( "Sessione del " + ConvertDate( item2["Data"].ToString() ) + " (" + u.TitoloAttivita( _TipoAttivita ) + " - " + item["RagioneSociale"].ToString() + ")" );
                }
              }
            }
          }
          break;

        case App.TipoAttivita.RelazioneBV:
          SelectedTreeSource = App.AppTemplateTreeRelazioneBV;
          LoadTreeSource();
          versionesessione = cBusinessObjects.GetVersioneSessione(IDSessione,"RelazioneBV");

          foreach (Hashtable item in clienti)
          {
            //if ( IDCliente == "-1" || item["ID"].ToString() == IDCliente )
            {
              ArrayList lista = mf.GetRelazioniBV(item["ID"].ToString());
              foreach (Hashtable item2 in lista)
              {
                if (item2["ID"].ToString() != IDSessione && versionesessione==cBusinessObjects.GetVersioneSessione(item2["ID"].ToString(),"RelazioneBV"))
                {
                  htComboID.Add(++index, item2["ID"].ToString());
                  comboSessioni.Items.Add(item["RagioneSociale"].ToString() + " - " + u.TitoloAttivita(_TipoAttivita) + " - Sessione del " + ConvertDate(item2["Data"].ToString()).Replace("01/01/", ""));
                  //comboSessioni.Items.Add( "Sessione del " + ConvertDate( item2["Data"].ToString() ) + " (" + u.TitoloAttivita( _TipoAttivita ) + " - " + item["RagioneSociale"].ToString() + ")" );
                }
              }
            }
          }
          break;

        case App.TipoAttivita.Verifica:
          SelectedTreeSource = App.AppTemplateTreeVerifica;
          LoadTreeSource();
          versionesessione = cBusinessObjects.GetVersioneSessione(IDSessione,"Verifica");

          foreach (Hashtable item in clienti)
          {
            //if ( IDCliente == "-1" || item["ID"].ToString() == IDCliente )
            {
              ArrayList lista = mf.GetVerifiche(item["ID"].ToString());
              foreach (Hashtable item2 in lista)
              {
                if (item2["ID"].ToString() != IDSessione && versionesessione==cBusinessObjects.GetVersioneSessione(item2["ID"].ToString(),"Verifica"))
                {
                  htComboID.Add(++index, item2["ID"].ToString());
                  comboSessioni.Items.Add(item["RagioneSociale"].ToString() + " - " + u.TitoloAttivita(_TipoAttivita) + " - Sessione del " + ConvertDate(item2["Data"].ToString()).Replace("01/01/", ""));
                  //comboSessioni.Items.Add( "Sessione del " + ConvertDate( item2["Data"].ToString() ) + " (" + u.TitoloAttivita( _TipoAttivita ) + " - " + item["RagioneSociale"].ToString() + ")" );
                }
              }
            }
          }
          break;
        case App.TipoAttivita.Vigilanza:
          SelectedTreeSource = App.AppTemplateTreeVigilanza;
          LoadTreeSource();
          versionesessione = cBusinessObjects.GetVersioneSessione(IDSessione,"Vigilanza");
          foreach (Hashtable item in clienti)
          {
            //if ( IDCliente == "-1" || item["ID"].ToString() == IDCliente )
            {
              ArrayList lista = mf.GetVigilanze(item["ID"].ToString());
              foreach (Hashtable item2 in lista)
              {
                if (item2["ID"].ToString() != IDSessione && versionesessione==cBusinessObjects.GetVersioneSessione(item2["ID"].ToString(),"Vigilanza"))
                {
                  htComboID.Add(++index, item2["ID"].ToString());
                  comboSessioni.Items.Add(item["RagioneSociale"].ToString() + " - " + u.TitoloAttivita(_TipoAttivita) + " - Sessione del " + ConvertDate(item2["Data"].ToString()).Replace("01/01/", ""));
                  //comboSessioni.Items.Add( "Sessione del " + ConvertDate( item2["Data"].ToString() ) + " (" + u.TitoloAttivita( _TipoAttivita ) + " - " + item["RagioneSociale"].ToString() + ")" );
                }
              }
            }
          }
          break;
        case App.TipoAttivita.PianificazioniVerifica:
          SelectedTreeSource = App.AppTemplateTreePianificazioniVerifica;
          LoadTreeSource();
          versionesessione = cBusinessObjects.GetVersioneSessione(IDSessione,"PianificazioniVerifica");

          foreach (Hashtable item in clienti)
          {
            //if ( IDCliente == "-1" || item["ID"].ToString() == IDCliente )
            {
              ArrayList lista = mf.GetPianificazioniVerifiche(item["ID"].ToString());
              foreach (Hashtable item2 in lista)
              {
                if (item2["ID"].ToString() != IDSessione && versionesessione==cBusinessObjects.GetVersioneSessione(item2["ID"].ToString(),"PianificazioniVerifica"))
                {
                  htComboID.Add(++index, item2["ID"].ToString());
                  comboSessioni.Items.Add(item["RagioneSociale"].ToString() + " - " + u.TitoloAttivita(_TipoAttivita) + " - Sessione del " + ConvertDate(item2["DataInizio"].ToString()).Replace("01/01/", ""));
                  //comboSessioni.Items.Add( "Sessione del " + ConvertDate( item2["Data"].ToString() ) + " (" + u.TitoloAttivita( _TipoAttivita ) + " - " + item["RagioneSociale"].ToString() + ")" );
                }
              }
            }
          }
          break;
        case App.TipoAttivita.PianificazioniVigilanza:
          SelectedTreeSource = App.AppTemplateTreePianificazioniVigilanza;
          LoadTreeSource();
          versionesessione = cBusinessObjects.GetVersioneSessione(IDSessione,"PianificazioniVigilanza");

          foreach (Hashtable item in clienti)
          {
            //if ( IDCliente == "-1" || item["ID"].ToString() == IDCliente )
            {
              ArrayList lista = mf.GetPianificazioniVigilanze(item["ID"].ToString());
              foreach (Hashtable item2 in lista)
              {
                if (item2["ID"].ToString() != IDSessione && versionesessione==cBusinessObjects.GetVersioneSessione(item2["ID"].ToString(),"PianificazioniVigilanza"))
                {
                  htComboID.Add(++index, item2["ID"].ToString());
                  comboSessioni.Items.Add(item["RagioneSociale"].ToString() + " - " + u.TitoloAttivita(_TipoAttivita) + " - Sessione del " + ConvertDate(item2["DataInizio"].ToString()).Replace("01/01/", ""));
                  //comboSessioni.Items.Add( "Sessione del " + ConvertDate( item2["Data"].ToString() ) + " (" + u.TitoloAttivita( _TipoAttivita ) + " - " + item["RagioneSociale"].ToString() + ")" );
                }
              }
            }
          }
          break;
        case App.TipoAttivita.Sconosciuto:
        default:
          break;
      }


      if (cBusinessObjects.xmldocument != null && cBusinessObjects.xmldocument.SelectSingleNode("/Tree") != null)
      {

        foreach (XmlNode item2 in TreeXmlProvider.Document.SelectSingleNode("/Tree").SelectNodes("//Node"))
        {

          foreach (XmlNode item in cBusinessObjects.xmldocument.SelectNodes("//Node"))
          {
            if (item.Attributes["ID"].Value == item2.Attributes["ID"].Value)
            {
              item2.Attributes["Checked"].Value = item.Attributes["Checked"].Value;

              break;
            }
          }

        }

      }
      else
      {
        base.Close();
      }
    }

    private void buttonEsportazione_Click(object sender, RoutedEventArgs e)
    {
      LoadTreeSource();
      Tipo = App.TipoScambioDati.Esporta;
    }

    private void buttonImportazione_Click(object sender, RoutedEventArgs e)
    {
      LoadTreeSource();
      Tipo = App.TipoScambioDati.Importa;
    }

    private void CheckBox_SourceUpdated(object sender, DataTransferEventArgs e)
    {
      XmlNode root = ((XmlNode)(((XmlAttribute)(((CheckBox)(sender)).Tag)).OwnerElement));
      foreach (XmlNode item in root.SelectNodes(".//Node"))
      {
        item.Attributes["Checked"].Value = root.Attributes["Checked"].Value;
      }
    }
  }
}