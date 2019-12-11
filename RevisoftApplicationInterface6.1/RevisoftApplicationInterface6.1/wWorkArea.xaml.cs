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
using UserControls2;
using System.IO;
using System.Text.RegularExpressions;
using RevisoftApplication.BRL;
using System.Data.SqlClient;
using System.Data;

namespace RevisoftApplication
{
  public partial class WindowWorkArea : Window
  {
      
    public string SelectedTreeSource = "";
    public string SelectedDataSource = "";
    public string SelectedSessioneSource = "";
    public string Cliente = "";
    public string Esercizio = "";
    public string SessioneAlias = "";
    public string SessioneFile = "";
    public App.TipoTreeNodeStato OldStatoNodo;

    public App.TipoTreeNodeStato StatoHome = App.TipoTreeNodeStato.Sconosciuto;
    public bool ReadOnlyHome = false;
    public bool StatoHomeDone = false;

    public bool ReadOnly = false;
    public bool ReadOnlyOLD = false;
    public bool ApertoInSolaLettura = true;

    public XmlDataProviderManager _x;
    public XmlDataProviderManager _xHome;
    XmlDataProviderManager xdpm;

    public string IDTree = "-1";
    public string IDCliente = "-1";
    public string IDSessione = "-1";

    public bool ConsentiChiusuraFinestra;

    public Hashtable Nodes = new Hashtable();
    public Hashtable Sessioni = new Hashtable();
    public Hashtable SessioniTitoli = new Hashtable();
    public Hashtable SessioniID = new Hashtable();

    //selectd color
    Brush ButtonStatoSelectedColor = new SolidColorBrush(Color.FromArgb(255, 247, 168, 39));
    Color ButtonToolBarSelectedColor = Color.FromArgb(126, 130, 189, 228);
    Color ButtonToolBarPulseColor = Color.FromArgb(126, 82, 101, 115);

    public int NodeHome;
    public int NodeNow;
    public int SessioneHome;
    public int SessioneNow;

    string _Tipologia = "";
    string _IDNodo = "";

    public App.TipoTreeNodeStato Stato = App.TipoTreeNodeStato.Sconosciuto;
    public App.TipoTreeNodeStato StatoBeforeSblocco = App.TipoTreeNodeStato.Sconosciuto;

    private bool istobeshowedGuida = false;
    private bool firsttime = true;
    private bool NodoSolaLettura = false;

    private string OLDTemplate = "";
    private string IDHere = "";

    public string sospesi = "";

    private XmlDocument doctmp = new XmlDocument();
    public bool m_isModified = false;

    // TEAM		
    public App.TipoAbilitazioneWindow _enableTeam = App.TipoAbilitazioneWindow.TuttoAbilitato;
    public string _cartellaxTeam = "";
    //private bool bloccaCartellaOn = false;

    public WindowWorkArea(ref XmlDataProviderManager x)
    {
      InitializeComponent();
      //bloccaCartellaOn = false;
      txtTitoloNodo.Foreground = App._arrBrushes[0];
      txtTitoloSessione.Foreground = App._arrBrushes[9];
      txtAlert.Foreground = App._arrBrushes[9];
      ButtonBarLeft.Background = App._arrBrushes[12];
      SolidColorBrush tmpBrush = (SolidColorBrush)Resources["buttonHover"];
      tmpBrush.Color = ((SolidColorBrush)App._arrBrushes[13]).Color;

      ConsentiChiusuraFinestra = false;
      m_isModified = false;

      _x = x;

      //apro file XML
      Utilities uh = new Utilities();
      XmlManager xh = new XmlManager();
      string tFile = string.Empty;
      App.ErrorLevel = App.ErrorTypes.Nessuno;
      tFile = App.AppTemplateFolder + "\\" + App.IndiceTemplateFileName + uh.EstensioneFile(App.TipoFile.IndiceTemplate);
      xh.TipoCodifica = XmlManager.TipologiaCodifica.Normale;

      doctmp = xh.LoadEncodedFile(tFile);
    }

    #region TOOLS
    public string FindIDGivenTamplate(string ID, string TemplateFrom, string TemplateTO)
    {
      //luigi
      //doctmp.Load( App.AppTemplateFolder + "\\TranscodificaTemplate.xml" );
      //doctmp.Load(App.AppTemplateFolder + App.IndiceTemplateFileName + u.EstensioneFile(App.TipoFile.IndiceTemplate));

      string IDTO = ID;

      XmlNode nodeFrom = doctmp.SelectSingleNode("/TEMPLATES/TEMPLATE[@VERSION=\"" + TemplateFrom + "\"]/TRANSCODE[@HERE=\"" + ID + "\"][@TREE=\"" + IDTree + "\"]");
      if (nodeFrom != null && nodeFrom.Attributes["ID"] != null)
      {
        string IDTRANSCODE = nodeFrom.Attributes["ID"].Value;
        XmlNode nodeTo = doctmp.SelectSingleNode("/TEMPLATES/TEMPLATE[@VERSION=\"" + TemplateTO + "\"]/TRANSCODE[@ID=\"" + IDTRANSCODE + "\"]");
        if (nodeTo != null && nodeTo.Attributes["HERE"] != null)
        {
          IDTO = nodeTo.Attributes["HERE"].Value;
        }
      }

      return IDTO;
    }

    //----------------------------------------------------------------------------+
    //                                    Load                                    |
    //----------------------------------------------------------------------------+
   
    public void Load()
    {

            // forzatura
     
      ReadOnly = false;
      ApertoInSolaLettura = false;
      txtAlert.Text = "";
      XmlDataProviderManager _x_AP = null;
      XmlNode node = ((XmlNode)(Nodes[NodeNow]));
      MasterFile mf = MasterFile.Create();
      if (IDHere == "") IDHere = ((XmlNode)(Nodes[NodeNow])).Attributes["ID"].Value;


      
      string file = mf.GetTreeAssociatoFromFileData(Sessioni[SessioneNow].ToString());
      file = App.AppDataDataFolder + "\\" + file;
      xdpm = new XmlDataProviderManager(file,IDTree);
    
      string templateHere = ((xdpm.Document.SelectSingleNode(
        "/Tree//REVISOFT").Attributes["Template"] != null)
          ? xdpm.Document.SelectSingleNode("/Tree//REVISOFT").Attributes["Template"].Value : "");
      if (OLDTemplate != "" && OLDTemplate != templateHere)
      {
        IDHere = FindIDGivenTamplate(IDHere, OLDTemplate, templateHere);
      }
      OLDTemplate = templateHere;
      

      node = xdpm.Document.SelectSingleNode("//Node[@ID=\"" + IDHere + "\"]");
     
      if (node == null) { stpAreaLavoro.Children.Clear(); return; }
      ConfiguraStatoNodo(Stato, false);
      AbilitaBottoniSessioni();
      AbilitaBottoniNodi();
      EvidenziaStato(true);
      AggiornaStatoBottoneOsservazioniConclusive();
      AggiornaStatoBottoneSospesi();
      AggiornaStatoBottoneModelliPredefiniti();
      AggiornaStatoBottoneDocumentiAssociat();
      AggiornaStatoBottoneIstruzioni();
      MasterFile tmpMF = MasterFile.Create();

      // TEAM 
      if (ReadOnly || _enableTeam == App.TipoAbilitazioneWindow.TuttoDisabilitato || _enableTeam == App.TipoAbilitazioneWindow.TuttoDisabilitatoPerReviewer)
      {
        base.Background = Brushes.Azure;
        Border_BoxContenuti.Background = Brushes.White;
        base.Title = "Revisoft - Area di lavoro - SESSIONE IN SOLA LETTURA";
      }
      else
      {
        base.Background = Brushes.WhiteSmoke;
        Border_BoxContenuti.Background = Brushes.White;
        base.Title = "Revisoft - Area di lavoro";
      }
      _Tipologia = node.Attributes["Tipologia"].Value;
      _IDNodo = node.Attributes["ID"].Value;
      if (SessioneNow == SessioneHome
        && (_IDNodo == "227" || _IDNodo == "229"
          || (_IDNodo == "321" && IDTree == "4")
          || (_IDNodo == "134" && (IDTree == "3" || IDTree == "71" || IDTree == "72" ||IDTree == "73"))
          || (_IDNodo == "139" && (IDTree == "3" || IDTree == "71" || IDTree == "72" ||IDTree == "73"))
          || _IDNodo == "2016174" || _IDNodo == "2016134"
          || _IDNodo == "2016186" || (_IDNodo == "166" && IDTree == "2")
          || (_IDNodo == "172" && IDTree == "2")) && ReadOnly == false && IDTree != "23")
      {
        btn_XBLR.Visibility = System.Windows.Visibility.Visible;
      }
      else btn_XBLR.Visibility = System.Windows.Visibility.Collapsed;
      if (SessioneNow == SessioneHome
        && ((_IDNodo == "134" && IDTree == "4") || _IDNodo == "131" || _IDNodo == "179"))
      {
        btn_Campionamento.Visibility = System.Windows.Visibility.Visible;
        if (_IDNodo == "131")
          btn_RotazioneScorte.Visibility = System.Windows.Visibility.Visible;
      }
      else
      {
        btn_Campionamento.Visibility = System.Windows.Visibility.Collapsed;
        btn_RotazioneScorte.Visibility = System.Windows.Visibility.Collapsed;
      }
      Uri uriSource = null;
      btn_StampaVerbale.Visibility = System.Windows.Visibility.Collapsed;
      switch ((App.TipoFile)(System.Convert.ToInt32(IDTree)))
      {
        case App.TipoFile.RelazioneB:
        case App.TipoFile.RelazioneV:
        case App.TipoFile.RelazioneBC:
        case App.TipoFile.RelazioneVC:
        case App.TipoFile.RelazioneBV:
          txt_StampaPDF.Text = "Stampa Anteprima";
          uriSource = new Uri("/RevisoftApplication;component/Images/icone/document_view.png", UriKind.Relative);
          img_StampaPDF.Source = new BitmapImage(uriSource);
          btn_DocumentiAssociati.Visibility = System.Windows.Visibility.Collapsed;
          btn_ModelliPredefiniti.Visibility = System.Windows.Visibility.Collapsed;
          btn_OsservazioniConclusive.Visibility = System.Windows.Visibility.Collapsed;
          btn_SOSPESI.Visibility = System.Windows.Visibility.Collapsed;
          break;
        case App.TipoFile.Incarico:
           case App.TipoFile.IncaricoCS:
            case App.TipoFile.IncaricoSU:
            case App.TipoFile.IncaricoREV:
          XmlNode nodehere =
            ((WindowWorkAreaTree)Owner).TreeXmlProvider.Document.
              SelectSingleNode("//Node[@ID='" + _IDNodo + "']");
          bool tobedonelettera = false;
          while (nodehere != null && nodehere.Attributes["ID"] != null)
          {
            if (nodehere.Attributes["ID"].Value == "142"
              || nodehere.Attributes["ID"].Value == "2016142") tobedonelettera = true;
            nodehere = nodehere.ParentNode;
          }
          if (tobedonelettera)
          {
            txt_StampaPDF.Text = "Stampa Anteprima";
            uriSource = new Uri("/RevisoftApplication;component/Images/icone/document_view.png", UriKind.Relative);
            img_StampaPDF.Source = new BitmapImage(uriSource);
            btn_DocumentiAssociati.Visibility = System.Windows.Visibility.Collapsed;
            btn_ModelliPredefiniti.Visibility = System.Windows.Visibility.Collapsed;
            btn_OsservazioniConclusive.Visibility = System.Windows.Visibility.Collapsed;
            btn_SOSPESI.Visibility = System.Windows.Visibility.Collapsed;
          }
          break;
        case App.TipoFile.ISQC:
          if (Owner.GetType().Name == "WindowWorkAreaTree")
          {
            XmlNode nodehereISQC =
              ((WindowWorkAreaTree)Owner).TreeXmlProvider.Document.
                SelectSingleNode("//Node[@ID='" + _IDNodo + "']");
            bool tobedoneletteraISQC = false;
            while (nodehereISQC != null && nodehereISQC.Attributes["ID"] != null)
            {
              if (nodehereISQC.Attributes["ID"].Value == "142")
                tobedoneletteraISQC = true;
              nodehereISQC = nodehereISQC.ParentNode;
            }
            if (tobedoneletteraISQC)
            {
              txt_StampaPDF.Text = "Stampa Anteprima";
              uriSource = new Uri("/RevisoftApplication;component/Images/icone/document_view.png", UriKind.Relative);
              img_StampaPDF.Source = new BitmapImage(uriSource);
              btn_DocumentiAssociati.Visibility = System.Windows.Visibility.Collapsed;
              btn_ModelliPredefiniti.Visibility = System.Windows.Visibility.Collapsed;
              btn_OsservazioniConclusive.Visibility = System.Windows.Visibility.Collapsed;
              btn_SOSPESI.Visibility = System.Windows.Visibility.Collapsed;
            }
          }
          break;
        case App.TipoFile.Conclusione:
          XmlNode nodehere2 =
            ((WindowWorkAreaTree)Owner).TreeXmlProvider.Document.
              SelectSingleNode("//Node[@ID='" + _IDNodo + "']");
          bool tobedonelettera2 = false;
          bool tobedonelettera3 = false;
          while (nodehere2 != null && nodehere2.Attributes["ID"] != null)
          {
            if (nodehere2.Attributes["ID"].Value == "261")
              tobedonelettera2 = true;
            if (nodehere2.Attributes["ID"].Value == "280")
              tobedonelettera3 = true;
            nodehere2 = nodehere2.ParentNode;
          }
          if (tobedonelettera2 || tobedonelettera3)
          {
            txt_StampaPDF.Text = "Stampa Anteprima";
            uriSource = new Uri("/RevisoftApplication;component/Images/icone/document_view.png", UriKind.Relative);
            img_StampaPDF.Source = new BitmapImage(uriSource);
            btn_DocumentiAssociati.Visibility = System.Windows.Visibility.Collapsed;
            btn_ModelliPredefiniti.Visibility = System.Windows.Visibility.Collapsed;
            btn_OsservazioniConclusive.Visibility = System.Windows.Visibility.Collapsed;
            btn_SOSPESI.Visibility = System.Windows.Visibility.Collapsed;
          }
          break;
        case App.TipoFile.Verifica:
        case App.TipoFile.Vigilanza:
          txt_StampaPDF.Text = "Stampa Anteprima";
          uriSource = new Uri("/RevisoftApplication;component/Images/icone/document_view.png", UriKind.Relative);
          img_StampaPDF.Source = new BitmapImage(uriSource);
          if (_IDNodo == "615" || _IDNodo == "616" || _IDNodo == "617" || _IDNodo == "618")
          {
            btn_DocumentiAssociati.Visibility = System.Windows.Visibility.Collapsed;
            btn_ModelliPredefiniti.Visibility = System.Windows.Visibility.Collapsed;
            btn_OsservazioniConclusive.Visibility = System.Windows.Visibility.Collapsed;
            btn_SOSPESI.Visibility = System.Windows.Visibility.Collapsed;
          }
          break;
        default:
          break;
      }
      if (node.Attributes["Report"] != null
        && node.Attributes["Report"].Value == "True")
      {
        NodoSolaLettura = true;
        ConfiguraStatoNodo(App.TipoTreeNodeStato.Completato, false);
        txtEsciSenzaSalvare.Text = "Esci";
        btn_EsciSenzaSalvare.Visibility = System.Windows.Visibility.Visible;
        btn_Stato_PrimaVisione.Visibility = System.Windows.Visibility.Collapsed;
        btn_SalvaTemporaneo.Visibility = System.Windows.Visibility.Collapsed;
        btn_DocumentiAssociati.Visibility = System.Windows.Visibility.Collapsed;
        btn_ModelliPredefiniti.Visibility = System.Windows.Visibility.Collapsed;
        btn_Stato_Completato.Visibility = System.Windows.Visibility.Collapsed;
        btn_Stato_DaCompletare.Visibility = System.Windows.Visibility.Collapsed;
        btn_Stato_NonApplicabile.Visibility = System.Windows.Visibility.Collapsed;
      //  btn_Stato_SbloccaNodo.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Collapsed;
        btn_OsservazioniConclusive.Visibility = System.Windows.Visibility.Collapsed;
        btn_SOSPESI.Visibility = System.Windows.Visibility.Collapsed;
      }
      else
      {
        NodoSolaLettura = false;
        btn_EsciSenzaSalvare.Visibility = System.Windows.Visibility.Visible;
        btn_Stato_PrimaVisione.Visibility = System.Windows.Visibility.Visible;
        if (_IDNodo == "128" || (IDTree == "2" && _IDNodo == "166")
          || (IDTree == "2" && _IDNodo == "172")
          || (IDTree != "2" && _IDNodo == "29")
          || (IDTree == "1" && _IDNodo == "70")
          || (IDTree == "1" && _IDNodo == "265")
          || (_IDNodo == "77" && IDTree != "2")
          || (_IDNodo == "78" && IDTree != "2")
          || _IDNodo == "199" || _IDNodo == "246"
          || (IDTree == "4"
            && (node.Attributes["ID"].Value == "227"
            || node.Attributes["ID"].Value == "229"
            || node.Attributes["ID"].Value == "206"
            || node.Attributes["ID"].Value == "207"
            || node.Attributes["ID"].Value == "219"
            || node.Attributes["ID"].Value == "220"))
          || ((IDTree == "3" || IDTree == "71" || IDTree == "72" ||IDTree == "73")
          && (node.Attributes["ID"].Value == "134"
            || node.Attributes["ID"].Value == "139"
            || node.Attributes["ID"].Value == "140"
            || node.Attributes["ID"].Value == "2016179"
            || node.Attributes["ID"].Value == "2016178"
            || node.Attributes["ID"].Value == "2016174"
            || node.Attributes["ID"].Value == "2016140"
            || node.Attributes["ID"].Value == "2016139"
            || node.Attributes["ID"].Value == "2016134"
            || node.Attributes["ID"].Value == "2016191"
            || node.Attributes["ID"].Value == "2016190"
            || node.Attributes["ID"].Value == "2016186")))
        {
          btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
          btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Collapsed;
        }
      }
      if (node.Attributes["ID"].Value == "9")
      {
        btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Collapsed;
        btn_NavBar_SessionePrev.Visibility = System.Windows.Visibility.Collapsed;
        btn_NavBar_SessioneNext.Visibility = System.Windows.Visibility.Collapsed;
        btn_NavBar_Home.Visibility = System.Windows.Visibility.Collapsed;
      }
      switch (_Tipologia)
      {
        case "Testo":
          if (node.Attributes["Titolo"].Value == "Discussioni del team"
            || node.Attributes["Titolo"].Value == "Discussioni tra sindaci")
          {
            _Tipologia = "DiscussioniTeam";
            ucDiscussioniTeam utxdt = new ucDiscussioniTeam();
            utxdt.Loaded += UserControl_Loaded;
            utxdt.ReadOnly = ReadOnly;
            utxdt.Load( node.Attributes["ID"].Value,IDCliente,IDSessione);
            stpAreaLavoro.Children.Clear();
            stpAreaLavoro.Children.Add(utxdt);
          }
          else
          {
            ucTesto utx = new ucTesto();
            utx.Loaded += UserControl_Loaded;
            utx.ReadOnly = ReadOnly;
            utx.Load( node.Attributes["ID"].Value,IDCliente,IDSessione);
            stpAreaLavoro.Children.Clear();
            stpAreaLavoro.Children.Add(utx);
            utx.FocusNow();
          }
          break;
        case "Relazione: Testo proposto a Scelta Multipla":
          ucTestoPropostoMultiplo utxpm = new ucTestoPropostoMultiplo();
          utxpm.Loaded += UserControl_Loaded;
          utxpm.ReadOnly = ReadOnly;
          utxpm.Load( node.Attributes["ID"].Value,IDCliente,IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(utxpm);
          break;
        case "Relazione: Testo proposto a Scelta Multipla non esclusiva":
          ucTestoPropostoMultiploNonEsclusivo utxpmne = new ucTestoPropostoMultiploNonEsclusivo();
          utxpmne.Loaded += UserControl_Loaded;
          utxpmne.ReadOnly = ReadOnly;
          utxpmne.Load( node.Attributes["ID"].Value, IDCliente, IDSessione);
                    stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(utxpmne);
          break;
        case "Relazione: Luogo Data e Firma":
          ucLuogoDataFirma utldf = new ucLuogoDataFirma();
          utldf.Loaded += UserControl_Loaded;
          utldf.ReadOnly = ReadOnly;
          utldf.Load( node.Attributes["ID"].Value, Sessioni[SessioneNow].ToString(), IDCliente, IDTree, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(utldf);
          break;
        case "Relazione: Destinatari e Bilancio":
          ucDestinatariEBilancio udeb = new ucDestinatariEBilancio();
          udeb.Loaded += UserControl_Loaded;
          udeb.ReadOnly = ReadOnly;
          udeb.Load( node.Attributes["ID"].Value, Sessioni[SessioneNow].ToString(), IDCliente, IDTree, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(udeb);
          break;
        case "Relazione: Errori Rilevati":
          ucRelazioneErroriRilevati uder = new ucRelazioneErroriRilevati();
          uder.Loaded += UserControl_Loaded;
          uder.ReadOnly = ReadOnly;
          uder.Load( node.Attributes["ID"].Value, Sessioni[SessioneNow].ToString(), IDCliente,IDSessione, IDTree);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uder);
          break;
        case "Tabella":
          ucTabella ut = new ucTabella();
          ut.Loaded += UserControl_Loaded;
          ut.ReadOnly = ReadOnly;
          ut.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
          ut.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
          ut.Load(node.Attributes["ID"].Value, "", IDTree, "",IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(ut);
          break;
        case "Tabella Replicabile":
          ucTabellaReplicata ur = new ucTabellaReplicata();
          ur.Loaded += UserControl_Loaded;
          //ur.ReadOnly = ReadOnly;
          ur.ReadOnly = false; // E.B.
          ur.Load( node.Attributes["ID"].Value, node.Attributes["Tab"].Value, IDTree,IDCliente,IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(ur);
          break;
        case "Check List":
          ucCheckList uc = new ucCheckList();
          uc.Loaded += UserControl_Loaded;
          uc.ReadOnly = ReadOnly;
          uc.Load( node.Attributes["ID"].Value,IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uc);
          break;
        case "Check List con Risultato":
          ucCheckList ucr = new ucCheckList();
          ucr.Loaded += UserControl_Loaded;
          ucr.Condizione = node.Attributes["Tab"].Value;
          ucr.ReadOnly = ReadOnly;
          ucr.Load(node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(ucr);
          break;
        case "Check List +":
          ucCheckListPlus ucp = new ucCheckListPlus();
          ucp.Loaded += UserControl_Loaded;
          ucp.ReadOnly = ReadOnly;
          ucp.Load(node.Attributes["ID"].Value,IDCliente, IDSessione);
                    stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(ucp);
          break;
        case "Check List + 6_1":
          ucCheckListPlus_6_1 ucp6_1 = new ucCheckListPlus_6_1();
          ucp6_1.Loaded += UserControl_Loaded;
          ucp6_1.ReadOnly = ReadOnly;
          ucp6_1.Load(node.Attributes["ID"].Value,IDCliente, IDSessione);
                    stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(ucp6_1);
          break;

        case "Nodo Multiplo":
          ucNodoMultiploVerticale unm = new ucNodoMultiploVerticale();
          unm.Loaded += UserControl_Loaded;
          unm.ReadOnly = ReadOnly;
          unm.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
          unm.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
          unm.Owner = this;
          unm.Load(ref _x, node.Attributes["ID"].Value, node.Attributes["Tab"].Value, node.ChildNodes, Sessioni, SessioneNow, IDTree, SessioniTitoli, SessioniID, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(unm);
          break;
        case "Nodo Multiplo Orizzontale":
          ucNodoMultiplo unmo = new ucNodoMultiplo();
          unmo.Loaded += UserControl_Loaded;
          unmo.ReadOnly = ReadOnly;
          try
          {
            unmo.Load(
              ref _x, node.Attributes["ID"].Value, node.Attributes["Tab"].Value,
              Sessioni, SessioneNow, node.Attributes["Tab"].Value,
              IDTree, IDCliente);
          }
          catch (Exception ex)
          {
            string log = ex.Message;
          }
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(unmo);
          break;
        case "Excel: Numeri Casuali":
          uc_Excel_NumeriCasuali uce_nc = new uc_Excel_NumeriCasuali();
          uce_nc.Loaded += UserControl_Loaded;
          uce_nc.LoadDataSource( node.Attributes["ID"].Value,IDCliente,IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_nc);
          break;
        case "Excel: Capitale Sociale":
          uc_Excel_CapitaleSociale uce_cs = new uc_Excel_CapitaleSociale();
          uce_cs.Loaded += UserControl_Loaded;
          uce_cs.ReadOnly = ReadOnly;
          uce_cs.LoadDataSource(node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_cs);
          break;
        case "Excel: Errori Rilevati":
          uc_Excel_ErroriRilevati uce_er = new uc_Excel_ErroriRilevati();
          uce_er.Loaded += UserControl_Loaded;
          uce_er.ReadOnly = ReadOnly;
          uce_er.LoadDataSource(node.Attributes["ID"].Value,IDCliente,IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_er);
          break;
        case "Excel: Errori Rilevati New":
          uc_Excel_ErroriRilevatiNew uce_ernew = new uc_Excel_ErroriRilevatiNew();
          uce_ernew.Loaded += UserControl_Loaded;
          uce_ernew.ReadOnly = ReadOnly;
          uce_ernew.LoadDataSource( node.Attributes["ID"].Value,IDCliente,IDSessione);
          uce_ernew.Owner = this;
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_ernew);
          break;
        case "Excel: Errori Rilevati Riepilogo":
        case "Excel: Sommario Rettifiche":
          ucErroriRilevatiRiepilogo uce_err = new ucErroriRilevatiRiepilogo();
          uce_err.Loaded += UserControl_Loaded;
          uce_err.ReadOnly = ReadOnly;
          bool result = uce_err.Load(
            ref _x, node.Attributes["ID"].Value,
            Sessioni[SessioneNow].ToString(), Sessioni, SessioniTitoli, SessioniID,
            SessioneNow, IDTree, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          if (result) stpAreaLavoro.Children.Add(uce_err);
          else
          {
            ConsentiChiusuraFinestra = true;
            this.Close();
          }
          break;
        case "Excel: Errori Rilevati Riepilogo New":
        case "Excel: Sommario Rettifiche New":
          ucErroriRilevatiRiepilogoNew uce_errnew = new ucErroriRilevatiRiepilogoNew();
          uce_errnew.Loaded += UserControl_Loaded;
          uce_errnew.ReadOnly = ReadOnly;
          bool resultnew = uce_errnew.Load(
           node.Attributes["ID"].Value, Sessioni[SessioneNow].ToString(),
            Sessioni, SessioniTitoli, SessioniID, SessioneNow, IDTree, IDCliente,
            IDSessione);
          stpAreaLavoro.Children.Clear();
          if (resultnew) stpAreaLavoro.Children.Add(uce_errnew);
          else
          {
            ConsentiChiusuraFinestra = true;
            this.Close();
          }
          break;
        case "Excel: Errori Rilevati Riepilogo NN":
          ucErroriRilevatiRiepilogoNN uce_errnn = new ucErroriRilevatiRiepilogoNN();
          uce_errnn.Loaded += UserControl_Loaded;
          uce_errnn.ReadOnly = ReadOnly;
          bool resultnn = uce_errnn.Load(
            ref _x, node.Attributes["ID"].Value, Sessioni[SessioneNow].ToString(),
            Sessioni, SessioniTitoli, SessioniID, SessioneNow, IDTree, IDCliente,
            IDSessione);
          stpAreaLavoro.Children.Clear();
          if (resultnn) stpAreaLavoro.Children.Add(uce_errnn);
          else
          {
            ConsentiChiusuraFinestra = true;
            this.Close();
          }
          break;
        case "Excel: Confronto Materialità":
          ucConfrontoMaterialita uce_cm = new ucConfrontoMaterialita();
          uce_cm.Loaded += UserControl_Loaded;
          uce_cm.ReadOnly = ReadOnly;
          bool result_cm = uce_cm.Load(
             node.Attributes["ID"].Value, Sessioni[SessioneNow].ToString(),
            Sessioni, SessioniTitoli, SessioniID, SessioneNow, IDTree, IDCliente,
            IDSessione);
          stpAreaLavoro.Children.Clear();
          if (result_cm) stpAreaLavoro.Children.Add(uce_cm);
          else
          {
            ConsentiChiusuraFinestra = true;
            this.Close();
          }
          break;
        case "Excel: Bilancio Riclassificato":
          ucExcel_BilancioRiclassificato uce_br = new ucExcel_BilancioRiclassificato();
          uce_br.Loaded += UserControl_Loaded;
                    uce_br.Load( _x_AP, node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_br);
          break;
         case "Dichiarazione_redditi_6_1":
          ucDichiarazioneRedditi_6_1 ucdichiar = new ucDichiarazioneRedditi_6_1();
          ucdichiar.Loaded += UserControl_Loaded;
          ucdichiar.LoadDataSource( node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(ucdichiar);
          break;
        case "Excel: Bilancio Abbreviato Riclassificato":
          ucExcel_BilancioRiclassificato uce_bar = new ucExcel_BilancioRiclassificato();
          uce_bar.Loaded += UserControl_Loaded;
          uce_bar.Abbreviato = true;
           uce_bar.Load(_x_AP, node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_bar);
          break;
        case "Excel: Bilancio Indici":
          ucExcel_BilancioIndici uce_bi = new ucExcel_BilancioIndici();
          uce_bi.Loaded += UserControl_Loaded;
          uce_bi.Load(ref _x, _x_AP, node.Attributes["ID"].Value, IDCliente, IDSessione);
                    stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_bi);
          break;
        case "Excel: Bilancio Abbreviato Indici":
          ucExcel_BilancioIndici uce_bai = new ucExcel_BilancioIndici();
          uce_bai.Loaded += UserControl_Loaded;
          uce_bai.Load(ref _x, _x_AP, node.Attributes["ID"].Value,IDCliente,IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_bai);
          break;
        case "Excel: Bilancio":
          uc_Excel_Bilancio uce_b = new uc_Excel_Bilancio(1);
          uce_b.Loaded += UserControl_Loaded;
          uce_b.ReadOnly = ReadOnly;
          uce_b.LoadDataSource(
            ref _x, node.Attributes["ID"].Value, _x_AP, App.AppDataFolder + "\\" +
            node.Attributes["Tab"].Value, IDCliente, IDSessione);
                    stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_b);
          break;
        case "Excel: Versamento imposte e contributi":
          ucExcel_VersamentoImposteContributi uce_vic = new ucExcel_VersamentoImposteContributi();
          uce_vic.Loaded += UserControl_Loaded;
          uce_vic.ReadOnly = ReadOnly;
          uce_vic.LoadDataSource( node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_vic);
          break;
        case "Excel: Compensazioni":
          ucExcel_Compensazioni uce_co = new ucExcel_Compensazioni();
          uce_co.Loaded += UserControl_Loaded;
          uce_co.ReadOnly = ReadOnly;
          uce_co.LoadDataSource(node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_co);
          break;
        case "Excel: Sospesi di Cassa":
          ucExcel_SospesiDiCassa uce_sc = new ucExcel_SospesiDiCassa();
          uce_sc.Loaded += UserControl_Loaded;
          uce_sc.ReadOnly = ReadOnly;
          uce_sc.LoadDataSource(node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_sc);
          break;
        case "Excel: Cassa Titoli":
          ucExcel_CassaTitoli uce_ct = new ucExcel_CassaTitoli();
          uce_ct.Loaded += UserControl_Loaded;
          uce_ct.ReadOnly = ReadOnly;
          uce_ct.LoadDataSource(node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_ct);
          break;
        case "Excel: Cassa Assegni":
          ucExcel_CassaAssegni uce_ca = new ucExcel_CassaAssegni();
          uce_ca.Loaded += UserControl_Loaded;
          uce_ca.ReadOnly = ReadOnly;
          uce_ca.LoadDataSource(node.Attributes["ID"].Value,IDCliente,IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_ca);
          break;
        case "Excel: Cassa Contante":
          ucExcel_CassaContante uce_cc = new ucExcel_CassaContante();
          uce_cc.Loaded += UserControl_Loaded;
          uce_cc.LoadDataSource(node.Attributes["ID"].Value, IDCliente, IDSessione);
          uce_cc.ReadOnly = ReadOnly;
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_cc);
          break;
        case "Excel: Cassa Valori Bollati":
          ucExcel_CassaValoriBollati uce_vb = new ucExcel_CassaValoriBollati();
          uce_vb.Loaded += UserControl_Loaded;
          uce_vb.ReadOnly = ReadOnly;
          uce_vb.LoadDataSource(node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_vb);
          break;
        case "Excel: Riconciliazioni Banche":
          ucExcel_Riconciliazioni uce_rb = new ucExcel_Riconciliazioni();
          uce_rb.Loaded += UserControl_Loaded;
          uce_rb.ReadOnly = ReadOnly;
          uce_rb.LoadDataSource(node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_rb);
          break;
        case "Excel: Materialità SP + CE":
          ucExcel_LimiteMaterialitaSPCE uce_lm = new ucExcel_LimiteMaterialitaSPCE(IDTree);
          uce_lm.Loaded += UserControl_Loaded;
          uce_lm.ReadOnly = ReadOnly;
          uce_lm.Load(
            node.Attributes["ID"].Value, Sessioni[SessioneNow].ToString(),
            IpotesiMaterialita.Prima, IDCliente, IDSessione);
                    stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_lm);
          break;
        case "Excel: Materialità SP e CE":
          ucExcel_LimiteMaterialitaSPCE uce_lm2 = new ucExcel_LimiteMaterialitaSPCE(IDTree);
          uce_lm2.Loaded += UserControl_Loaded;
          uce_lm2.ReadOnly = ReadOnly;
          uce_lm2.Load(
            node.Attributes["ID"].Value, Sessioni[SessioneNow].ToString(),
            IpotesiMaterialita.Seconda, IDCliente, IDSessione);
                    stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_lm2);
          break;
        case "Excel: Materialità Personalizzata":
          ucExcel_LimiteMaterialitaSPCE uce_lm3 = new ucExcel_LimiteMaterialitaSPCE(IDTree);
          uce_lm3.Loaded += UserControl_Loaded;
          uce_lm3.ReadOnly = ReadOnly;
          uce_lm3.Load( node.Attributes["ID"].Value, Sessioni[SessioneNow].ToString(),  IpotesiMaterialita.Terza, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_lm3);
          break;
        case "Excel: Affidamenti Bancari":
          ucExcel_Affidamenti uce_ab = new ucExcel_Affidamenti();
          uce_ab.Loaded += UserControl_Loaded;
          uce_ab.ReadOnly = ReadOnly;
          uce_ab.LoadDataSource( node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_ab);
          break;
        case "Excel: Dipendenza Finanziaria":
          ucIndipendenzaFinanziaria uce_if = new ucIndipendenzaFinanziaria();
          uce_if.Loaded += UserControl_Loaded;
          uce_if.ReadOnly = ReadOnly;
          uce_if.LoadDataSource( node.Attributes["ID"].Value,IDCliente,IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_if);
          break;
        case "Excel: Pianificazione":
          ucPianificazione uce_p = new ucPianificazione();
          uce_p.Loaded += UserControl_Loaded;
          uce_p.ReadOnly = ReadOnly;
          uce_p.Load(node.Attributes["ID"].Value,IDCliente,IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_p);
          break;
        case "Excel: PianificazioneNew":
          if (node.Attributes["ID"].Value == "274")
          {
            ucPianificazioneNewWithDetails uce_pNew = new ucPianificazioneNewWithDetails();
            uce_pNew.Loaded += UserControl_Loaded;
            uce_pNew.ReadOnly = ReadOnly;
            uce_pNew.Load(
              node.Attributes["ID"].Value,
              Sessioni[SessioneNow].ToString(), Sessioni, SessioniTitoli,
              SessioniID, SessioneNow, IDTree, IDCliente, IDSessione);
            stpAreaLavoro.Children.Clear();
            stpAreaLavoro.Children.Add(uce_pNew);
          }
          else
          {
            ucPianificazioneNew uce_pNew = new ucPianificazioneNew();
            uce_pNew.Loaded += UserControl_Loaded;
            uce_pNew.ReadOnly = ReadOnly;
            uce_pNew.Load(
              node.Attributes["ID"].Value,
              Sessioni[SessioneNow].ToString(), Sessioni, SessioniTitoli,
              SessioniID, SessioneNow, IDTree, IDCliente, IDSessione);
            stpAreaLavoro.Children.Clear();
            stpAreaLavoro.Children.Add(uce_pNew);
          }
          break;
        case "Excel: Compensi e Risorse":
          ucCompensiERisorse uce_cer = new ucCompensiERisorse();
          uce_cer.Loaded += UserControl_Loaded;
         
          uce_cer.ReadOnly = ReadOnly;
          uce_cer.Load(node.Attributes["ID"].Value,IDCliente,IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_cer);
          break;

        case "CompensiERisorse_6_1":
            ucCompensiERisorse_6_1 uce_cer_6_1 = new ucCompensiERisorse_6_1();
       
            uce_cer_6_1.Loaded += UserControl_Loaded;
            uce_cer_6_1.ReadOnly = ReadOnly;
            uce_cer_6_1.Load( node.Attributes["ID"].Value,IDCliente,IDSessione); 
            DataTable datirischio = null;
            if(cBusinessObjects.GetIDTree(int.Parse(node.Attributes["ID"].Value))==712016129)
               datirischio = cBusinessObjects.GetData(2016994, typeof(Accettazionedelrischio_6_1),cBusinessObjects.idcliente,cBusinessObjects.idsessione,71);
            if(cBusinessObjects.GetIDTree(int.Parse(node.Attributes["ID"].Value))==722016129)
               datirischio = cBusinessObjects.GetData(2016994, typeof(Accettazionedelrischio_6_1),cBusinessObjects.idcliente,cBusinessObjects.idsessione,72);
            if(cBusinessObjects.GetIDTree(int.Parse(node.Attributes["ID"].Value))==732016129)
               datirischio = cBusinessObjects.GetData(2016994, typeof(Accettazionedelrischio_6_1),cBusinessObjects.idcliente,cBusinessObjects.idsessione,73);
            if(datirischio.Rows.Count==0)
            {
                MessageBox.Show("Attenzione compilare la scheda accettazione del rischio prima di complilare la carta corrente");
                //ConsentiChiusuraFinestra = true;
                //this.Close();
                //return;
            }
            else
            {
                string rischio = "";
               foreach(DataRow dd in datirischio.Rows)
                {
                if (dd["Rischio"].ToString() == "BASSO")
                    rischio = dd["Rischio"].ToString();
                if (dd["Rischio"].ToString() == "ALTO")
                    rischio = dd["Rischio"].ToString();
                if (dd["Rischio"].ToString() == "MEDIO")
                    rischio = dd["Rischio"].ToString();
        
                }
                if(rischio=="")
                {
                    MessageBox.Show("Attenzione compilare la scheda accettazione del rischio prima di complilare la carta corrente");
                    //ConsentiChiusuraFinestra = true;
                    //this.Close();
                    //return;
                }


                foreach(DataRow dd in datirischio.Rows)
                {
                if(dd["Rischio"].ToString()=="BASSO")
                   uce_cer_6_1.txtRischio.Text = "Rischio preliminare \"Basso\"";
                if(dd["Rischio"].ToString()=="MEDIO")
                   uce_cer_6_1.txtRischio.Text = "Rischio preliminare \"Moderato\""; 
                if(dd["Rischio"].ToString()=="ALTO")
                   uce_cer_6_1.txtRischio.Text = "Rischio preliminare \"Alto\"";
                  uce_cer_6_1.txtRischio.IsEnabled = false;
                }
                uce_cer_6_1.RicalcolaStimaOre();
            }
       

            stpAreaLavoro.Children.Clear();
            stpAreaLavoro.Children.Add(uce_cer_6_1);
            break;

        case "Excel: Tempi di Revisione":
          if (IDTree == "28")
          {
            string titolo_tdr = "";
            if (node.Attributes["ID"].Value == "168" || node.Attributes["ID"].Value == "169"
              || node.Attributes["ID"].Value == "171")
            {
              ucExcel_ISQC_Incaricati uce_tdr = new ucExcel_ISQC_Incaricati();
              switch (node.Attributes["ID"].Value)
              {
                case "168":
                  titolo_tdr = "RESPONSABILE DELLA REVISIONE";
                  break;
                case "169":
                  titolo_tdr = "RESPONSABILE DEL RIESAME DELLA QUALITA'";
                  break;
                case "171":
                  titolo_tdr = "TEAM DI REVISIONE - COMPONENTI";
                  break;
                default:
                  break;
              }
              uce_tdr.Loaded += UserControl_Loaded;
              uce_tdr.ReadOnly = ReadOnly;
              uce_tdr.LoadDataSource( node.Attributes["ID"].Value, IDCliente, IDSessione, titolo_tdr);
              stpAreaLavoro.Children.Clear();
              stpAreaLavoro.Children.Add(uce_tdr);
            }
            else if (node.Attributes["ID"].Value == "186")
            {
              ucExcel_ISQC_TempiLavoro_Riepilogo uce_tdr = new ucExcel_ISQC_TempiLavoro_Riepilogo();
              uce_tdr.Loaded += UserControl_Loaded;
              uce_tdr.ReadOnly = ReadOnly;
              uce_tdr.LoadDataSource( node.Attributes["ID"].Value, IDCliente, IDSessione, "RIEPILOGO DEI TEMPI DI LAVORO");
              stpAreaLavoro.Children.Clear();
              stpAreaLavoro.Children.Add(uce_tdr);
            }
            else
            {
              switch (node.Attributes["ID"].Value)
              {
                case "181":
                  titolo_tdr = "Comprensione - rischio - pianificazione";
                  break;
                case "182":
                  titolo_tdr = "Controllo del Bilancio";
                  break;
                case "183":
                  titolo_tdr = "Conclusioni - Review - Relazione";
                  break;
                case "184":
                  titolo_tdr = "Altre attività";
                  break;
                case "185":
                  titolo_tdr = "Verifiche periodiche";
                  break;
                default:
                  break;
              }
              ucExcel_ISQC_TempiLavoro uce_tdr = new ucExcel_ISQC_TempiLavoro();
              uce_tdr.Loaded += UserControl_Loaded;
              uce_tdr.ReadOnly = ReadOnly;
              uce_tdr.LoadDataSource(node.Attributes["ID"].Value, IDCliente, IDSessione, titolo_tdr);
              stpAreaLavoro.Children.Clear();
              stpAreaLavoro.Children.Add(uce_tdr);
            }
          }
          else
          {
            ucTempiRevisione uce_tdr = new ucTempiRevisione();
            uce_tdr.Loaded += UserControl_Loaded;
            uce_tdr.ReadOnly = ReadOnly;
            uce_tdr.Load( node.Attributes["ID"].Value,IDCliente,IDSessione);
            stpAreaLavoro.Children.Clear();
            stpAreaLavoro.Children.Add(uce_tdr);
          }
          break;
        case "Excel: F24":
          try
          {
            ucExcel_F24 uce_f24 = new ucExcel_F24();
            uce_f24.Loaded += UserControl_Loaded;
            uce_f24.ReadOnly = ReadOnly;
            uce_f24.LoadDataSource( node.Attributes["ID"].Value, IDCliente, IDSessione);
            stpAreaLavoro.Children.Clear();
            stpAreaLavoro.Children.Add(uce_f24);
          }
          catch (Exception ex)
          {
            string log = ex.Message;
          }
          break;
        case "Excel: COGE":
          ucExcel_COGE uce_COGE = new ucExcel_COGE();
          uce_COGE.Loaded += UserControl_Loaded;
          uce_COGE.ReadOnly = ReadOnly;
          uce_COGE.LoadDataSource(node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_COGE);
          break;
        case "Excel: Uniemens":
          ucExcel_Uniemens uce_Uniemens = new ucExcel_Uniemens();
          uce_Uniemens.Loaded += UserControl_Loaded;
          uce_Uniemens.ReadOnly = ReadOnly;
          uce_Uniemens.LoadDataSource(node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_Uniemens);
          break;
        case "Excel: Contributi Agenti":
          ucExcel_ContributiAgenti uce_ContributiAgenti = new ucExcel_ContributiAgenti();
          uce_ContributiAgenti.Loaded += UserControl_Loaded;
          uce_ContributiAgenti.ReadOnly = ReadOnly;
          uce_ContributiAgenti.LoadDataSource(node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_ContributiAgenti);
          break;
        case "Excel: CUD":
          ucExcel_CUD uce_CUD = new ucExcel_CUD();
          uce_CUD.Loaded += UserControl_Loaded;
          uce_CUD.ReadOnly = ReadOnly;
          uce_CUD.LoadDataSource( node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_CUD);
          break;
        case "Ritenute Lavoratori Autonomi":
          ucExcel_RitenuteLavoratoriAutonomi uce_RLA = new ucExcel_RitenuteLavoratoriAutonomi();
          uce_RLA.Loaded += UserControl_Loaded;
          uce_RLA.ReadOnly = ReadOnly;
          uce_RLA.LoadDataSource( node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_RLA);
          break;
        case "Excel: Rifiuti":
          ucExcel_Rifiuti uce_R = new ucExcel_Rifiuti();
          uce_R.Loaded += UserControl_Loaded;
          uce_R.ReadOnly = ReadOnly;
          uce_R.LoadDataSource(node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_R);
          break;
        case "Excel: ScrittureMagazzino":
          ucExcel_ScrittureMagazzino uce_M = new ucExcel_ScrittureMagazzino();
          uce_M.Loaded += UserControl_Loaded;
          uce_M.ReadOnly = ReadOnly;
          uce_M.LoadDataSource( node.Attributes["ID"].Value, IDCliente, IDSessione);
          stpAreaLavoro.Children.Clear();
          stpAreaLavoro.Children.Add(uce_M);
          break;
        case "Excel":
          if (node.Attributes["ID"].Value == "327" && IDTree == "4")
          {
            ucConsolidatoScope uce_ab_consolidato = new ucConsolidatoScope();
            uce_ab_consolidato.Loaded += UserControl_Loaded;
            uce_ab_consolidato.ReadOnly = ReadOnly;
            uce_ab_consolidato.Load(ref _x, node.Attributes["ID"].Value);
            stpAreaLavoro.Children.Clear();
            stpAreaLavoro.Children.Add(uce_ab_consolidato);
          }
          else if (node.Attributes["ID"].Value == "316" && IDTree == "4")
          {
            ucConsolidatoReportistica uce_ab_consolidato = new ucConsolidatoReportistica();
            uce_ab_consolidato.Loaded += UserControl_Loaded;
            uce_ab_consolidato.ReadOnly = ReadOnly;
            uce_ab_consolidato.Load( node.Attributes["ID"].Value,
              Sessioni[SessioneNow].ToString(), IDCliente, IDTree);
            stpAreaLavoro.Children.Clear();
            stpAreaLavoro.Children.Add(uce_ab_consolidato);
          }
          else if (node.Attributes["ID"].Value == "315" && IDTree == "4")
          {
            ucConsolidatoIstruzioni uce_ab_consolidato = new ucConsolidatoIstruzioni();
            uce_ab_consolidato.Loaded += UserControl_Loaded;
            uce_ab_consolidato.ReadOnly = ReadOnly;
            uce_ab_consolidato.Load(node.Attributes["ID"].Value,IDCliente,IDSessione);
            stpAreaLavoro.Children.Clear();
            stpAreaLavoro.Children.Add(uce_ab_consolidato);
          }
          if (node.Attributes["ID"].Value == "313" && IDTree == "4")
          {
            ucExcel_Consolidato uce_ab_consolidato = new ucExcel_Consolidato();
            uce_ab_consolidato.Loaded += UserControl_Loaded;
            uce_ab_consolidato.ReadOnly = ReadOnly;
            uce_ab_consolidato.LoadDataSource( node.Attributes["ID"].Value, IDCliente, IDSessione);
            stpAreaLavoro.Children.Clear();
            stpAreaLavoro.Children.Add(uce_ab_consolidato);
          }
          else if (node.Attributes["ID"].Value == "202")
          {
            ucCicli uce_c = new ucCicli();
            uce_c.Loaded += UserControl_Loaded;
            uce_c.Load(
              ref _x, node.Attributes["ID"].Value,
              Sessioni[SessioneNow].ToString(), Sessioni, SessioniTitoli,
              SessioniID, SessioneNow, IDTree, IDCliente, IDSessione);
            stpAreaLavoro.Children.Clear();
            stpAreaLavoro.Children.Add(uce_c);
          }
          else if (node.Attributes["ID"].Value == "22" && IDTree != "2")
          {
            ucRischioGlobale uce_rg = new ucRischioGlobale();
            uce_rg.Owner = this;
            uce_rg.Loaded += UserControl_Loaded;
                        uce_rg.Load(node.Attributes["ID"].Value, Sessioni, SessioneNow, IDTree, IDCliente, IDSessione);
            stpAreaLavoro.Children.Clear();
            stpAreaLavoro.Children.Add(uce_rg);
          }
          else if ((node.Attributes["ID"].Value == "160" || node.Attributes["ID"].Value == "2016160") && (IDTree == "3" ||IDTree == "71" || IDTree == "72" || IDTree == "73")  )
          {
            ucLetteraIncarico_Personale ucli_pe = new ucLetteraIncarico_Personale();
            ucli_pe.Loaded += UserControl_Loaded;
            ucli_pe.ReadOnly = ReadOnly;
            ucli_pe.LoadDataSource( node.Attributes["ID"].Value, IDCliente, IDSessione);
            stpAreaLavoro.Children.Clear();
            stpAreaLavoro.Children.Add(ucli_pe);
          }
          else if ((node.Attributes["ID"].Value == "161" || node.Attributes["ID"].Value == "2016161") && (IDTree == "3" ||IDTree == "71" || IDTree == "72" || IDTree == "73"))
          {
            ucLetteraIncarico_TempiCorrispettivi ucli_tc = new ucLetteraIncarico_TempiCorrispettivi();
            ucli_tc.Owner = this;
            ucli_tc.Loaded += UserControl_Loaded;
            ucli_tc.ReadOnly = ReadOnly;
            ucli_tc.LoadDataSource(node.Attributes["ID"].Value, IDCliente, IDSessione);
            stpAreaLavoro.Children.Clear();
            stpAreaLavoro.Children.Add(ucli_tc);
          }
          else if ((node.Attributes["ID"].Value == "162"
            || node.Attributes["ID"].Value == "2016162") && (IDTree == "3" ||IDTree == "71" || IDTree == "72" || IDTree == "73"))
          {
            ucLetteraIncarico_Pagamenti ucli_pa = new ucLetteraIncarico_Pagamenti();
            ucli_pa.Loaded += UserControl_Loaded;
            ucli_pa.ReadOnly = ReadOnly;
            ucli_pa.LoadDataSource(node.Attributes["ID"].Value, IDCliente, IDSessione);
            stpAreaLavoro.Children.Clear();
            stpAreaLavoro.Children.Add(ucli_pa);
          }
          break;
        default:
          stpAreaLavoro.Children.Clear();
          break;
      }
      //forzatura
      ReadOnly = false;
      ApertoInSolaLettura = false;

      // TEAM
      EnableButtonForTeam();
    }

    // TEAM
    private void EnableButtonForTeam()
    {
      switch (_enableTeam)
      {
        case App.TipoAbilitazioneWindow.AbilitaPerTeamLeader:
          btn_Stato_Completato.IsEnabled = true;
          btn_Stato_DaCompletare.IsEnabled = true;
          btn_Stato_NonApplicabile.IsEnabled = true;
                    //btn_Stato_SbloccaNodo.IsEnabled = true;
                    btn_SalvaTemporaneo.IsEnabled = true;
          btn_Stato_PrimaVisione.IsEnabled = true;
          btn_CopiaDaAltraSessione2.IsEnabled = true;
          btn_CopiaInSessioneAttiva.IsEnabled = true;
          btn_Note.Visibility = Visibility.Collapsed;
          btn_Stato_BloccoEsecutore.Visibility = Visibility.Collapsed;
          break;
        case App.TipoAbilitazioneWindow.AbilitaPerReviewer:
        case App.TipoAbilitazioneWindow.AbilitaPerReviewerBloccato:
          btn_Stato_Completato.IsEnabled = true;
          btn_Stato_DaCompletare.IsEnabled = true;
          btn_Stato_BloccoEsecutore.IsEnabled = true;
          btn_Stato_NonApplicabile.IsEnabled = false;
                    //btn_Stato_SbloccaNodo.IsEnabled = false;
                    btn_SalvaTemporaneo.IsEnabled = false;
          btn_Stato_PrimaVisione.IsEnabled = false;
          btn_CopiaDaAltraSessione2.IsEnabled = false;
          btn_CopiaInSessioneAttiva.IsEnabled = false;
          btn_Note.Visibility = Visibility.Visible;
          break;
        case App.TipoAbilitazioneWindow.AbilitaPerEsecutore:
          if (cCartelle.IsCartellaBloccata(_cartellaxTeam, App.AppUtente.Id, IDCliente, false))
          {
            // l'esecutore non può modificare la cartella
            btn_Stato_Completato.IsEnabled = false;
            btn_Stato_DaCompletare.IsEnabled = false;
            btn_Stato_NonApplicabile.IsEnabled = false;
                        //btn_Stato_SbloccaNodo.IsEnabled = false;
                        btn_SalvaTemporaneo.IsEnabled = false;
            btn_Stato_PrimaVisione.IsEnabled = false;
            btn_CopiaDaAltraSessione2.IsEnabled = false;
            btn_CopiaInSessioneAttiva.IsEnabled = false;
          }
          else
          {
            btn_Stato_Completato.IsEnabled = true;
            btn_Stato_DaCompletare.IsEnabled = true;
            btn_Stato_NonApplicabile.IsEnabled = true;
                        //btn_Stato_SbloccaNodo.IsEnabled = true;
                        btn_SalvaTemporaneo.IsEnabled = true;
            btn_Stato_PrimaVisione.IsEnabled = true;
            btn_CopiaDaAltraSessione2.IsEnabled = true;
            btn_CopiaInSessioneAttiva.IsEnabled = true;

          }
          btn_Stato_BloccoEsecutore.Visibility = Visibility.Collapsed;
          btn_Note.Visibility = Visibility.Collapsed;
          break;
        case App.TipoAbilitazioneWindow.TuttoDisabilitatoPerReviewer:
          btn_Stato_BloccoEsecutore.IsEnabled = false;
          btn_Stato_Completato.IsEnabled = false;
          btn_Stato_DaCompletare.IsEnabled = false;
          btn_Stato_NonApplicabile.IsEnabled = false;
                    //btn_Stato_SbloccaNodo.IsEnabled = false;
                    btn_SalvaTemporaneo.IsEnabled = false;
          btn_Stato_PrimaVisione.IsEnabled = false;
          btn_CopiaDaAltraSessione2.IsEnabled = false;
          btn_CopiaInSessioneAttiva.IsEnabled = false;
          btn_Note.Visibility = Visibility.Visible;
          break;
        case App.TipoAbilitazioneWindow.TuttoAbilitato:
          btn_Stato_Completato.IsEnabled = true;
          btn_Stato_DaCompletare.IsEnabled = true;
          btn_Stato_NonApplicabile.IsEnabled = true;
                    //btn_Stato_SbloccaNodo.IsEnabled = true;
                    btn_SalvaTemporaneo.IsEnabled = true;
          btn_Stato_PrimaVisione.IsEnabled = true;
          btn_CopiaDaAltraSessione2.IsEnabled = true;
          btn_CopiaInSessioneAttiva.IsEnabled = true;
          btn_Note.Visibility = Visibility.Collapsed;
          btn_Stato_BloccoEsecutore.Visibility = Visibility.Collapsed;
          break;
        case App.TipoAbilitazioneWindow.TuttoDisabilitato:
          btn_Stato_Completato.IsEnabled = false;
          btn_Stato_DaCompletare.IsEnabled = false;
          btn_Stato_NonApplicabile.IsEnabled = false;
                    //btn_Stato_SbloccaNodo.IsEnabled = false;
                    btn_SalvaTemporaneo.IsEnabled = false;
          btn_Stato_PrimaVisione.IsEnabled = false;
          btn_CopiaDaAltraSessione2.IsEnabled = false;
          btn_CopiaInSessioneAttiva.IsEnabled = false;
          btn_Note.Visibility = Visibility.Collapsed;
          btn_Stato_BloccoEsecutore.Visibility = Visibility.Collapsed;
          break;
        default:
          btn_Stato_Completato.IsEnabled = true;
          btn_Stato_DaCompletare.IsEnabled = true;
          btn_Stato_NonApplicabile.IsEnabled = true;
                    //btn_Stato_SbloccaNodo.IsEnabled = true;
                    btn_SalvaTemporaneo.IsEnabled = true;
          btn_Stato_PrimaVisione.IsEnabled = true;
          btn_CopiaDaAltraSessione2.IsEnabled = true;
          btn_CopiaInSessioneAttiva.IsEnabled = true;
          btn_Note.Visibility = Visibility.Collapsed;
          btn_Stato_BloccoEsecutore.Visibility = Visibility.Collapsed;
          break;
      }
            if (cBusinessObjects.ReadOnlyControls)
            {
                btn_Stato_NonApplicabile.IsEnabled = false;
                btn_Stato_DaCompletare.IsEnabled = false;
                btn_Stato_Completato.IsEnabled = false;
                //btn_Stato_SbloccaNodo.IsEnabled = true;

                btn_SalvaTemporaneo.IsEnabled = false;
                btn_EsciSenzaSalvare.IsEnabled = false;
                btn_Stato_PrimaVisione.IsEnabled = false;
            }
        }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {

      ////luigi
      ////XmlDocument doctmp = new XmlDocument();
      ////doctmp.Load( App.AppTemplateFolder + "\\TranscodificaTemplate.xml" );
      ////doctmp.Load(App.AppTemplateFolder + App.IndiceTemplateFileName + u.EstensioneFile(App.TipoFile.IndiceTemplate));

      //Utilities u = new Utilities();
      //XmlDocument doctmp = new XmlDocument();

      ////apro file XML
      //XmlManager x = new XmlManager();
      //string tFile = string.Empty;
      //App.ErrorLevel = App.ErrorTypes.Nessuno;
      //tFile = App.AppTemplateFolder + "\\" + App.IndiceTemplateFileName + u.EstensioneFile(App.TipoFile.IndiceTemplate);
      //x.TipoCodifica = XmlManager.TipologiaCodifica.Normale;

      //doctmp = x.LoadEncodedFile(tFile);

      XmlNode nodeFrom = doctmp.SelectSingleNode("/TEMPLATES/TEMPLATE[@VERSION=\"" +
        OLDTemplate + "\"]/TRANSCODE[@HERE=\"" + IDHere + "\"][@TREE=\"" + IDTree + "\"]");
      if (nodeFrom != null && nodeFrom.Attributes["MESSAGE"] != null)
      {
        MessageBox.Show(nodeFrom.Attributes["MESSAGE"].Value);
      }

      if (stpAreaLavoro.Children[0].GetType().Name == "ucTesto")
      {
        ((ucTesto)(stpAreaLavoro.Children[0])).FocusNow();
      }
    }

    private void AbilitaBottoniNodi()
    {
      txtTitoloNodo.Text = ((XmlNode)(Nodes[NodeNow])).Attributes["Codice"].Value +
        " " + ((XmlNode)(Nodes[NodeNow])).Attributes["Titolo"].Value;

      btn_NavBar_NodoPrev.Visibility = System.Windows.Visibility.Collapsed;
      btn_NavBar_NodoNext.Visibility = System.Windows.Visibility.Collapsed;

      return;

      //codice vecchio per attivazione / disattivazione nodi

#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
      if (ReadOnly == false)// || NodeNow != NodeHome)
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
      {
        btn_NavBar_NodoPrev.Visibility = System.Windows.Visibility.Collapsed;
        btn_NavBar_NodoNext.Visibility = System.Windows.Visibility.Collapsed;
      }
      else
      {
        btn_NavBar_NodoPrev.Visibility = System.Windows.Visibility.Visible;
        btn_NavBar_NodoNext.Visibility = System.Windows.Visibility.Visible;

        if (NodeNow == 0)
        {
          btn_NavBar_NodoPrev.IsEnabled = false;
        }
        else
        {
          btn_NavBar_NodoPrev.IsEnabled = true;
        }

        if ((NodeNow + 1) == Nodes.Count)
        {
          btn_NavBar_NodoNext.IsEnabled = false;
        }
        else
        {
          btn_NavBar_NodoNext.IsEnabled = true;
        }
      }
    }

    private void AbilitaBottoniSessioni()
    {
      txtTitoloSessione.Text = SessioniTitoli[SessioneNow].ToString();// +"; Template: " + OLDTemplate + "; ID: " + IDHere;
            
      if (SessioneNow == 0)
      {
        btn_NavBar_SessionePrev.IsEnabled = false;
      }
      else
      {
        btn_NavBar_SessionePrev.IsEnabled = true;
      }

      if ((SessioneNow + 1) == Sessioni.Count)
      {
        btn_NavBar_SessioneNext.IsEnabled = false;
      }
      else
      {
        btn_NavBar_SessioneNext.IsEnabled = true;
      }
    }

    private void EvidenziaStato(bool updatestato)
    {
      if (updatestato)
      {
        UpdateStato();
      }

      switch (Stato)
      {
        case App.TipoTreeNodeStato.NonApplicabileBucoTemplate:
        case App.TipoTreeNodeStato.NonApplicabile:
          btn_Stato_NonApplicabile.Background = ButtonStatoSelectedColor;
          btn_Stato_DaCompletare.Background = btn_Stato_PrimaVisione.Background;
          btn_Stato_Completato.Background = btn_Stato_PrimaVisione.Background;
                    //btn_Stato_SbloccaNodo.Background = btn_Stato_PrimaVisione.Background;
                    break;
        case App.TipoTreeNodeStato.DaCompletare:
          btn_Stato_DaCompletare.Background = ButtonStatoSelectedColor;
          btn_Stato_NonApplicabile.Background = btn_Stato_PrimaVisione.Background;
          btn_Stato_Completato.Background = btn_Stato_PrimaVisione.Background;
                    // btn_Stato_SbloccaNodo.Background = btn_Stato_PrimaVisione.Background;
                    break;
        case App.TipoTreeNodeStato.Completato:
          btn_Stato_Completato.Background = ButtonStatoSelectedColor;
          btn_Stato_NonApplicabile.Background = btn_Stato_PrimaVisione.Background;
          btn_Stato_DaCompletare.Background = btn_Stato_PrimaVisione.Background;
                    //    btn_Stato_SbloccaNodo.Background = btn_Stato_PrimaVisione.Background;
                    break;
        case App.TipoTreeNodeStato.CompletatoBloccoEsecutore:
          btn_Stato_BloccoEsecutore.Background = ButtonStatoSelectedColor;
          btn_Stato_NonApplicabile.Background = btn_Stato_PrimaVisione.Background;
          btn_Stato_DaCompletare.Background = btn_Stato_PrimaVisione.Background;
                    //btn_Stato_SbloccaNodo.Background = btn_Stato_PrimaVisione.Background;
                    break;
        case App.TipoTreeNodeStato.Scrittura:
                    // btn_Stato_SbloccaNodo.Background = ButtonStatoSelectedColor;
                    btn_Stato_NonApplicabile.Background = btn_Stato_PrimaVisione.Background;
          btn_Stato_DaCompletare.Background = btn_Stato_PrimaVisione.Background;
          btn_Stato_Completato.Background = btn_Stato_PrimaVisione.Background;

          switch (StatoBeforeSblocco)
          {
            case App.TipoTreeNodeStato.NonApplicabileBucoTemplate:
            case App.TipoTreeNodeStato.NonApplicabile:
              AnimateBackgroundColor(btn_Stato_NonApplicabile, Color.FromArgb(255, 247, 168, 39), ButtonToolBarSelectedColor, 1);
              break;
            case App.TipoTreeNodeStato.Completato:
              AnimateBackgroundColor(btn_Stato_Completato, Color.FromArgb(255, 247, 168, 39), ButtonToolBarSelectedColor, 1);
              break;
            case App.TipoTreeNodeStato.DaCompletare:
              AnimateBackgroundColor(btn_Stato_DaCompletare, Color.FromArgb(255, 247, 168, 39), ButtonToolBarSelectedColor, 1);
              break;
            default:
              break;
          }
          break;
        case App.TipoTreeNodeStato.CancellaDati:
        case App.TipoTreeNodeStato.Sconosciuto:
        default:
          btn_Stato_NonApplicabile.Background = btn_Stato_PrimaVisione.Background;
          btn_Stato_DaCompletare.Background = btn_Stato_PrimaVisione.Background;
          btn_Stato_Completato.Background = btn_Stato_PrimaVisione.Background;
                    //btn_Stato_SbloccaNodo.Background = btn_Stato_PrimaVisione.Background;
                    break;
      }

      // TEAM
      if (_enableTeam == App.TipoAbilitazioneWindow.AbilitaPerReviewerBloccato)
      {
        btn_Stato_DaCompletare.Background = btn_Stato_PrimaVisione.Background;
        btn_Stato_Completato.Background = btn_Stato_PrimaVisione.Background;
        btn_Stato_BloccoEsecutore.Background = ButtonStatoSelectedColor;
      }
    }

    private void UpdateStato()
    {
      XmlNode node = ((XmlNode)(Nodes[NodeNow]));


            string tmpstato = cBusinessObjects.GetStato(int.Parse(node.Attributes["ID"].Value), IDCliente, IDSessione);
      if (tmpstato != "")
      {
        try
        {
          Stato = ((App.TipoTreeNodeStato)(Convert.ToInt32(tmpstato)));
        }
        catch (Exception ex)
        {
          Stato = App.TipoTreeNodeStato.Sconosciuto;

          string log = ex.Message;
        }
      }
    }

  
    private void ResetNodo()
    {

      bool isModified = false;
      if (NodeNow != NodeHome || SessioneNow != SessioneHome)
      {
        MessageBox.Show("Solo la Carta di Lavoro selezionata della sessione selezionata può essere resettata.");
        return;
      }

        XmlNode node = ((XmlNode)(Nodes[NodeHome]));

        List<string> tableslist = cBusinessObjects.FindTablesById(int.Parse(node.Attributes["ID"].Value));
        foreach (string tb in tableslist)
        {
            string nomeclasse = "RevisoftApplication." + tb + ", RevisoftApplication";

            cBusinessObjects.Executesql("DELETE FROM " + tb + " WHERE ID_SCHEDA=" + cBusinessObjects.GetIDTree(int.Parse(node.Attributes["ID"].Value)).ToString() + " AND ID_CLIENTE=" + cBusinessObjects.idcliente.ToString() + " AND ID_SESSIONE=" + cBusinessObjects.idsessione.ToString());
        }
        foreach (XmlNode item2 in node.ChildNodes)
        {
                if (item2.Name != "Node")
                    continue;
            tableslist = cBusinessObjects.FindTablesById(int.Parse(item2.Attributes["ID"].Value));
            foreach (string tb in tableslist)
            {
                string nomeclasse = "RevisoftApplication." + tb + ", RevisoftApplication";

                cBusinessObjects.Executesql("DELETE FROM " + tb + " WHERE ID_SCHEDA=" + cBusinessObjects.GetIDTree(int.Parse(item2.Attributes["ID"].Value)).ToString() + " AND ID_CLIENTE=" + cBusinessObjects.idcliente.ToString() + " AND ID_SESSIONE=" + cBusinessObjects.idsessione.ToString());
            }
         
        }

    
     
    }
    #endregion

    #region GESTIONE_STATI

    
    private bool SalvaDatiControllo(App.TipoTreeNodeStato IpotesiStato)
    {

      XmlNode node = ((XmlNode)(Nodes[NodeNow]));

            switch (_Tipologia)
            {
                case "DiscussioniTeam":
                    ((ucDiscussioniTeam)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Testo":
                    ((ucTesto)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Relazione: Testo proposto a Scelta Multipla":
                    ((ucTestoPropostoMultiplo)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Relazione: Testo proposto a Scelta Multipla non esclusiva":
                    ((ucTestoPropostoMultiploNonEsclusivo)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Relazione: Errori Rilevati":
                    ((ucRelazioneErroriRilevati)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Relazione: Luogo Data e Firma":
                    ((ucLuogoDataFirma)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Relazione: Destinatari e Bilancio":
                    ((ucDestinatariEBilancio)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Tabella":
                    ((ucTabella)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Tabella Replicabile":
                    ((ucTabellaReplicata)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Check List con Risultato":
                case "Check List":
                    ((ucCheckList)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Check List +":
                    ((ucCheckListPlus)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Check List + 6_1":
                    ((ucCheckListPlus_6_1)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Nodo Multiplo":
                    //PRISC VELOCIZZAZIONE
                    //xdpm.Load();
                    XmlDataProviderManager _xxx = ((ucNodoMultiploVerticale)(stpAreaLavoro.Children[0])).Save(IpotesiStato);

                    break;
                case "Nodo Multiplo Orizzontale":
                    //_x = ((ucNodoMultiplo)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Capitale Sociale":
                    ((uc_Excel_CapitaleSociale)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Numeri Casuali":
                    ((uc_Excel_NumeriCasuali)(stpAreaLavoro.Children[0])).Save();
                    break;
                //case "Excel: Tariffa Professionale":
                //    //_x = ((uc_Excel_TariffaProfessionale)(stpAreaLavoro.Children[0])).Save();
                //    break;
                case "Excel: Errori Rilevati":
                    ((uc_Excel_ErroriRilevati)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Errori Rilevati New":
                    ((uc_Excel_ErroriRilevatiNew)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Errori Rilevati Riepilogo":
                case "Excel: Sommario Rettifiche":
                    _x = ((ucErroriRilevatiRiepilogo)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Errori Rilevati Riepilogo New":
                case "Excel: Sommario Rettifiche New":
                     ((ucErroriRilevatiRiepilogoNew)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Bilancio":
                    //_x = ((uc_Excel_Bilancio)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Versamento imposte e contributi":
                    ((ucExcel_VersamentoImposteContributi)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Compensazioni":
                    ((ucExcel_Compensazioni)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Dichiarazione_redditi_6_1":
                    ((ucDichiarazioneRedditi_6_1)(stpAreaLavoro.Children[0])).Save();
                    break;

                 case "Excel: Sospesi di Cassa":
                    ((ucExcel_SospesiDiCassa)(stpAreaLavoro.Children[0])).Save();
                    break;
                    

                case "Excel: Cassa Titoli":
                    ((ucExcel_CassaTitoli)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Cassa Assegni":
                    ((ucExcel_CassaAssegni)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Cassa Contante":
                    ((ucExcel_CassaContante)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Cassa Valori Bollati":
                    ((ucExcel_CassaValoriBollati)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Riconciliazioni Banche":
                    ((ucExcel_Riconciliazioni)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Materialità SP + CE":
                case "Excel: Materialità SP e CE":
                case "Excel: Materialità Personalizzata":
                    ((ucExcel_LimiteMaterialitaSPCE)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Affidamenti Bancari":
                    ((ucExcel_Affidamenti)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Dipendenza Finanziaria":
                    ((ucIndipendenzaFinanziaria)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Pianificazione":
                    ((ucPianificazione)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: PianificazioneNew":
                    if (node.Attributes["ID"].Value == "274")
                    {
                        ((ucPianificazioneNewWithDetails)(stpAreaLavoro.Children[0])).Save();
                    }
                    else
                    {
                        ((ucPianificazioneNew)(stpAreaLavoro.Children[0])).Save();
                    }
                    break;
                case "Excel: Compensi e Risorse":
                    ((ucCompensiERisorse)(stpAreaLavoro.Children[0])).Save();

                    break;
                 case "CompensiERisorse_6_1":
                    ((ucCompensiERisorse_6_1)(stpAreaLavoro.Children[0])).Save();

                    break;
                case "Excel: Tempi di Revisione":
                    if (IDTree == "28")
                    {
                        if (node.Attributes["ID"].Value == "168" || node.Attributes["ID"].Value == "169" || node.Attributes["ID"].Value == "171")
                        {
                            ((ucExcel_ISQC_Incaricati)(stpAreaLavoro.Children[0])).Save();
                        }
                        else if (node.Attributes["ID"].Value == "186")
                        {
                            ((ucExcel_ISQC_TempiLavoro_Riepilogo)(stpAreaLavoro.Children[0])).Save();
                        }
                        else
                        {
                            ((ucExcel_ISQC_TempiLavoro)(stpAreaLavoro.Children[0])).Save();
                        }
                    }
                    else
                    {
                        ((ucTempiRevisione)(stpAreaLavoro.Children[0])).Save();
                    }
                    break;
                case "Excel: F24":
                    ((ucExcel_F24)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: COGE":
                    ((ucExcel_COGE)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Uniemens":
                    ((ucExcel_Uniemens)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Contributi Agenti":
                    ((ucExcel_ContributiAgenti)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: CUD":
                    ((ucExcel_CUD)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: Rifiuti":
                    ((ucExcel_Rifiuti)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel: ScrittureMagazzino":
                    ((ucExcel_ScrittureMagazzino)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Ritenute Lavoratori Autonomi":
                    ((ucExcel_RitenuteLavoratoriAutonomi)(stpAreaLavoro.Children[0])).Save();
                    break;
                case "Excel":
                    if (node.Attributes["ID"].Value == "327" && IDTree == "4")
                    {
                        _x = ((ucConsolidatoScope)(stpAreaLavoro.Children[0])).Save();
                    }
                    else if (node.Attributes["ID"].Value == "316" && IDTree == "4")
                    {
                        ((ucConsolidatoReportistica)(stpAreaLavoro.Children[0])).Save();
                    }
                    else if (node.Attributes["ID"].Value == "315" && IDTree == "4")
                    {
                        ((ucConsolidatoIstruzioni)(stpAreaLavoro.Children[0])).Save();
                    }
                    else if (node.Attributes["ID"].Value == "313" && IDTree == "4")
                    {
                        ((ucExcel_Consolidato)(stpAreaLavoro.Children[0])).Save();
                    }
                    else if (node.Attributes["ID"].Value == "202")
                    {
                        _x = ((ucCicli)(stpAreaLavoro.Children[0])).Save();
                    }
                    else if (node.Attributes["ID"].Value == "22" && IDTree != "2")
                    {
                        ((ucRischioGlobale)(stpAreaLavoro.Children[0])).Save();
                    }
                    else if ((node.Attributes["ID"].Value == "160" || node.Attributes["ID"].Value == "2016160") && (IDTree == "3" ||IDTree == "71" || IDTree == "72" || IDTree == "73"))
                    {
                        ((ucLetteraIncarico_Personale)(stpAreaLavoro.Children[0])).Save();
                    }
                    else if ((node.Attributes["ID"].Value == "161" || node.Attributes["ID"].Value == "2016161") && (IDTree == "3" ||IDTree == "71" || IDTree == "72" || IDTree == "73"))
                    {
                        ((ucLetteraIncarico_TempiCorrispettivi)(stpAreaLavoro.Children[0])).Save();
                    }
                    else if ((node.Attributes["ID"].Value == "162" || node.Attributes["ID"].Value == "2016162") && (IDTree == "3" ||IDTree == "71" || IDTree == "72" || IDTree == "73"))
                    {
                        ((ucLetteraIncarico_Pagamenti)(stpAreaLavoro.Children[0])).Save();
                    }
                    break;
                default:
                    break;
            }
      
      
    
      return true;
    }

    private void btn_Stato_NonApplicabile_Click(object sender, RoutedEventArgs e)
    {

      if (ApertoInSolaLettura)
      {
        MessageBox.Show(App.MessaggioSolaScritturaStato);
        return;
      }

      if (ReadOnly == true && ReadOnlyOLD != true && (Stato != App.TipoTreeNodeStato.NonApplicabile && Stato != App.TipoTreeNodeStato.NonApplicabileBucoTemplate))
      {
        MessageBox.Show("La Carta di Lavoro è nello stato " + App.NomeTipoTreeNodeStato(Stato).ToUpper() + ". Occorre selezionare Sblocca Stato per modificare il contenuto.");
        return;
      }

      if (MessageBox.Show("Lo stato Non Applicabile implica la cancellazione dei dati, si vuole procedere?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        ResetNodo();

   //     SalvaDatiControllo(App.TipoTreeNodeStato.Sconosciuto);

        StatoBeforeSblocco = App.TipoTreeNodeStato.Sconosciuto;

        ConfiguraStatoNodo(App.TipoTreeNodeStato.NonApplicabile, true);
      }
      

    }

    private void btn_Stato_DaCompletare_Click(object sender, RoutedEventArgs e)
    {
           
      // TEAM
      GestioneSbloccoCartellaPerEsecutore();

      if (Stato == App.TipoTreeNodeStato.DaCompletare)
      {
        ConfiguraStatoNodoExitNoChange(App.TipoTreeNodeStato.DaCompletare, true);
        return;
      }

      if (ApertoInSolaLettura)
      {
        MessageBox.Show(App.MessaggioSolaScritturaStato);
        return;
      }

      if (ReadOnly == true && ReadOnlyOLD != true && Stato != App.TipoTreeNodeStato.DaCompletare)
      {
        MessageBox.Show("La Carta di Lavoro è nello stato " + App.NomeTipoTreeNodeStato(Stato).ToUpper() +
          ". Occorre selezionare Sblocca Stato per modificare il contenuto.");
        return;
      }

      try
      {
        XmlNode node = ((XmlNode)(Nodes[NodeNow]));

        //if ( ( IDTree == "21" && node.Attributes["ID"].Value == "12" && ( _x.Document.SelectSingleNode( "/Dati//Dato[@ID='11']" ).Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode( "/Dati//Dato[@ID='10']" ).Attributes["Stato"].Value == "2" ) ) || ( IDTree == "21" && node.Attributes["ID"].Value == "11" && ( _x.Document.SelectSingleNode( "/Dati//Dato[@ID='10']" ).Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode( "/Dati//Dato[@ID='12']" ).Attributes["Stato"].Value == "2" ) ) || ( IDTree == "21" && node.Attributes["ID"].Value == "10" && ( _x.Document.SelectSingleNode( "/Dati//Dato[@ID='11']" ).Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode( "/Dati//Dato[@ID='12']" ).Attributes["Stato"].Value == "2" ) ) )
        //{
        //    MessageBox.Show( "Queste voci mutualmente esclusive. E' già presente un paragrafo COMPLETATO." );
        //    return;
        //}

        if (((IDTree == "3" ||IDTree == "71" || IDTree == "72" || IDTree == "73") && node.Attributes["ID"].Value == "161" && (
            cBusinessObjects.GetStato(164, IDCliente, IDSessione) == "2")) || 
            ((IDTree == "3" ||IDTree == "71" || IDTree == "72" || IDTree == "73") && node.Attributes["ID"].Value == "164" && 
            (cBusinessObjects.GetStato(161, IDCliente, IDSessione) == "2")))
        {
          MessageBox.Show("Queste voci mutualmente esclusive. E' già presente un paragrafo COMPLETATO.");
          return;
        }

        if ((IDTree == "21" && node.Attributes["ID"].Value == "17" && (
          cBusinessObjects.GetStato(18, IDCliente, IDSessione) == "2" ||
          cBusinessObjects.GetStato(19, IDCliente, IDSessione) == "2" ||
          cBusinessObjects.GetStato(20, IDCliente, IDSessione) == "2")) || 
          (IDTree == "21" && node.Attributes["ID"].Value == "18" && 
          (cBusinessObjects.GetStato(17, IDCliente, IDSessione) == "2" ||
          cBusinessObjects.GetStato(19, IDCliente, IDSessione) == "2" ||
          cBusinessObjects.GetStato(20, IDCliente, IDSessione) == "2")) || 
          (IDTree == "21" && node.Attributes["ID"].Value == "19" && 
          (cBusinessObjects.GetStato(17, IDCliente, IDSessione) == "2" ||
          cBusinessObjects.GetStato(18, IDCliente, IDSessione) == "2" ||
          cBusinessObjects.GetStato(20, IDCliente, IDSessione) == "2")) || 
          (IDTree == "21" && node.Attributes["ID"].Value == "20" && 
          (cBusinessObjects.GetStato(17, IDCliente, IDSessione) == "2" ||
          cBusinessObjects.GetStato(18, IDCliente, IDSessione) == "2" ||
          cBusinessObjects.GetStato(19, IDCliente, IDSessione) == "2")))
        {
          MessageBox.Show("Queste voci mutualmente esclusive. E' già presente un paragrafo COMPLETATO.");
          return;
        }

        if ((IDTree == "21" && node.Attributes["ID"].Value == "5" && (
            cBusinessObjects.GetStato(30, IDCliente, IDSessione) == "2" ||
            cBusinessObjects.GetStato(31, IDCliente, IDSessione) == "2")) || 
            (IDTree == "21" && node.Attributes["ID"].Value == "30" && 
            (cBusinessObjects.GetStato(5, IDCliente, IDSessione) == "2" ||
            cBusinessObjects.GetStato(31, IDCliente, IDSessione) == "2")) || 
            (IDTree == "21" && node.Attributes["ID"].Value == "31" && 
            (cBusinessObjects.GetStato(5, IDCliente, IDSessione) == "2" ||
           cBusinessObjects.GetStato(30, IDCliente, IDSessione) == "2")))
        {
          MessageBox.Show("Queste voci mutualmente esclusive. E' già presente un paragrafo COMPLETATO.");
          return;
        }

        if ((IDTree == "21" && node.Attributes["ID"].Value == "10" && 
           (cBusinessObjects.GetStato(11, IDCliente, IDSessione) == "2" ||
           cBusinessObjects.GetStato(27, IDCliente, IDSessione) == "2" ||
           cBusinessObjects.GetStato(28, IDCliente, IDSessione) == "2" ||
           cBusinessObjects.GetStato(33, IDCliente, IDSessione) == "2")) || 
           (IDTree == "21" && node.Attributes["ID"].Value == "11" && 
           (cBusinessObjects.GetStato(10, IDCliente, IDSessione) == "2" ||
           cBusinessObjects.GetStato(27, IDCliente, IDSessione) == "2" ||
           cBusinessObjects.GetStato(28, IDCliente, IDSessione) == "2" ||
           cBusinessObjects.GetStato(23, IDCliente, IDSessione) == "2")) || 
           (IDTree == "21" && node.Attributes["ID"].Value == "27" && 
           (cBusinessObjects.GetStato(10, IDCliente, IDSessione) == "2" ||
           cBusinessObjects.GetStato(11, IDCliente, IDSessione) == "2")) || 
           (IDTree == "21" && node.Attributes["ID"].Value == "28" && 
           (cBusinessObjects.GetStato(10, IDCliente, IDSessione) == "2" ||
           cBusinessObjects.GetStato(11, IDCliente, IDSessione) == "2")) || 
           (IDTree == "21" && node.Attributes["ID"].Value == "33" && 
           (cBusinessObjects.GetStato(10, IDCliente, IDSessione) == "2" ||
           cBusinessObjects.GetStato(11, IDCliente, IDSessione) == "2")))
        {
          MessageBox.Show("Queste voci mutualmente esclusive. E' già presente un paragrafo COMPLETATO.");
          return;
        }

        if ((IDTree == "21" && node.Attributes["ID"].Value == "28" &&
            cBusinessObjects.GetStato(33, IDCliente, IDSessione) == "2") || 
            (IDTree == "21" && node.Attributes["ID"].Value == "33" &&
            cBusinessObjects.GetStato(28, IDCliente, IDSessione) == "2"))
        {
          MessageBox.Show("Queste voci mutualmente esclusive. E' già presente un paragrafo COMPLETATO.");
          return;
        }

        if ((IDTree == "22" && node.Attributes["ID"].Value == "28" && 
        (cBusinessObjects.GetStato(34, IDCliente, IDSessione) == "2" ||
        cBusinessObjects.GetStato(35, IDCliente, IDSessione) == "2")) || 
        (IDTree == "22" && node.Attributes["ID"].Value == "34" && 
        (cBusinessObjects.GetStato(28, IDCliente, IDSessione) == "2" ||
        cBusinessObjects.GetStato(35, IDCliente, IDSessione) == "2")) || 
        (IDTree == "22" && node.Attributes["ID"].Value == "35" && 
        (cBusinessObjects.GetStato(28, IDCliente, IDSessione) == "2" ||
        cBusinessObjects.GetStato(34, IDCliente, IDSessione) == "2")))
        {
          MessageBox.Show("Queste voci mutualmente esclusive. E' già presente un paragrafo COMPLETATO.");
          return;
        }
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }

      if (SalvaDatiControllo(App.TipoTreeNodeStato.DaCompletare) == false)
      {
        return;
      }

      StatoBeforeSblocco = App.TipoTreeNodeStato.Sconosciuto;

      ConfiguraStatoNodo(App.TipoTreeNodeStato.DaCompletare, true);
    }

    public bool ShouldBEDaCompletare = false;

    private void btn_SalvaTemporaneo_Click(object sender, RoutedEventArgs e)
    {
      if (ApertoInSolaLettura)
      {
        MessageBox.Show(App.MessaggioSolaScritturaStato);
        return;
      }
      if (ReadOnly == true && ReadOnlyOLD != true)
      {
        MessageBox.Show("La Carta di Lavoro è nello stato " +
          App.NomeTipoTreeNodeStato(Stato).ToUpper() +
          ". Occorre selezionare Sblocca Stato per modificare il contenuto.");
        return;
      }
      if (SalvaDatiControllo(App.TipoTreeNodeStato.Sconosciuto) == false)
      {
        return;
      }
      ShouldBEDaCompletare = true;
      // forzatura
      //Load();
    }

    public bool ForzaturaADaCompletare = false;

    //----------------------------------------------------------------------------+
    //                         btn_Stato_Completato_Click                         |
    //----------------------------------------------------------------------------+
    private void btn_Stato_BloccoEsecutore_Click(object sender, RoutedEventArgs e)
    {
      // TEAM
      // se lo stato è già bloccato si ignora il click, non imposto il bottone a disabled perchè altrimenti non visualizza i colori
      if (Stato == App.TipoTreeNodeStato.CompletatoBloccoEsecutore && _enableTeam != App.TipoAbilitazioneWindow.AbilitaPerReviewerBloccato)
      {
        ConfiguraStatoNodoExitNoChange(App.TipoTreeNodeStato.CompletatoBloccoEsecutore, true);
        return;
      }


      // quando il revisore blocca inserisce nella tabella la cartella bloccata, ma solo se non è già bloccata	
      cCartelle.BloccaCartella(_cartellaxTeam, App.AppUtente.Id, IDCliente);

      // TODO
      // in seguito al blocco per l'esecutore si devono eseguire le modifiche per il completamento della cartella
      GestioneStatoCompletato(App.TipoTreeNodeStato.CompletatoBloccoEsecutore);


      //bloccaCartellaOn = true;
      //base.Close();
      //return;
    }


    private void GestioneStatoCompletato(App.TipoTreeNodeStato nuovo_stato)
    {

     

     if (Stato == nuovo_stato)
      {
        ConfiguraStatoNodoExitNoChange(nuovo_stato, true);
        return;
      }
      if (_Tipologia == "Relazione: Testo proposto a Scelta Multipla")
      {
        if (!((ucTestoPropostoMultiplo)(stpAreaLavoro.Children[0])).CheckifOK())
        {
          return;
        }
      }
      if (_Tipologia == "Relazione: Testo proposto a Scelta Multipla non esclusiva")
      {
        if (!((ucTestoPropostoMultiploNonEsclusivo)(stpAreaLavoro.Children[0])).CheckifOK())
        {
          return;
        }
      }
      if (ApertoInSolaLettura)
      {
        MessageBox.Show(App.MessaggioSolaScritturaStato);
        return;
      }
      if (ReadOnly == true && ReadOnlyOLD != true && Stato != App.TipoTreeNodeStato.Completato)
      {
        MessageBox.Show("La Carta di Lavoro è nello stato " +
          App.NomeTipoTreeNodeStato(Stato).ToUpper() +
          ". Occorre selezionare Sblocca Stato per modificare il contenuto.");
        return;
      }
      //----------------------------------------------- verifica nodi autoesclusivi
      try
      {
        XmlNode node = ((XmlNode)(Nodes[NodeNow]));
        if ( (IDTree == "3" && !(node.Attributes["Codice"].Value.Contains("1.1.20")))  ||
          (IDTree == "71" && !(node.Attributes["Codice"].Value.Contains("1.CS.20")) ) ||
          (IDTree == "72" && !(node.Attributes["Codice"].Value.Contains("1.SU.20")) )  ||
          (IDTree == "73" && !(node.Attributes["Codice"].Value.Contains("1.REV.20")) ) )
        {
          if ((node.Attributes["ID"].Value == "161"
            && cBusinessObjects.GetStato(164,IDCliente,IDSessione) == "2")
            || (node.Attributes["ID"].Value == "164"
            && cBusinessObjects.GetStato(161, IDCliente, IDSessione) == "2"))
          {
            MessageBox.Show("Il bilancio ordinario e il bilancio abbreviato non " +
              "possono essere COMPLETATI contemporaneamente.");
            return;
          }
        }
        if ( (IDTree == "3" && !(node.Attributes["Codice"].Value.Contains("1.1.20")))  ||
          (IDTree == "71" && !(node.Attributes["Codice"].Value.Contains("1.CS.20")) ) ||
          (IDTree == "72" && !(node.Attributes["Codice"].Value.Contains("1.SU.20")) )  ||
          (IDTree == "73" && !(node.Attributes["Codice"].Value.Contains("1.REV.20")) ) )
        {
          if (
            (node.Attributes["ID"].Value == "134"
             && cBusinessObjects.GetStato(2016174, IDCliente, IDSessione) == "2")
            ||
            (node.Attributes["ID"].Value == "2016174"
             && cBusinessObjects.GetStato(134, IDCliente, IDSessione) == "2"))
          {
            MessageBox.Show("Il bilancio ordinario e il bilancio abbreviato non " +
              "possono essere COMPLETATI contemporaneamente.");
            return;
          }
        }
         if ( (IDTree == "3" && !(node.Attributes["Codice"].Value.Contains("1.1.20")))  ||
          (IDTree == "71" && !(node.Attributes["Codice"].Value.Contains("1.CS.20")) ) ||
          (IDTree == "72" && !(node.Attributes["Codice"].Value.Contains("1.SU.20")) )  ||
          (IDTree == "73" && !(node.Attributes["Codice"].Value.Contains("1.REV.20")) ) )
        {
          if (
            (node.Attributes["ID"].Value == "2016134"
             && cBusinessObjects.GetStato(2016186, IDCliente, IDSessione) == "2")
            ||
            (node.Attributes["ID"].Value == "2016186"
             && cBusinessObjects.GetStato(2016134, IDCliente, IDSessione) == "2"))
          {
            MessageBox.Show("Il bilancio ordinario e il bilancio abbreviato non " +
              "possono essere COMPLETATI contemporaneamente.");
            return;
          }
        }
        if (IDTree != "23")
        {
          if (
            (IDTree != "1" && node.Attributes["ID"].Value == "227"
             && cBusinessObjects.GetStato(229, IDCliente, IDSessione)== "2")
            ||
            (IDTree != "1" && node.Attributes["ID"].Value == "229"
             && cBusinessObjects.GetStato(227, IDCliente, IDSessione) == "2"))
          {
            MessageBox.Show("Il bilancio ordinario e il bilancio abbreviato non " +
              "possono essere COMPLETATI contemporaneamente.");
            return;
          }
        }
        if (
          (IDTree == "1" && node.Attributes["ID"].Value == "77"
           && (
            cBusinessObjects.GetStato(78, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(199, IDCliente, IDSessione) == "2"))
          ||
          (IDTree == "1" && node.Attributes["ID"].Value == "78"
           && (cBusinessObjects.GetStato(78, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(199, IDCliente, IDSessione) == "2"))
          ||
          (IDTree == "1" && node.Attributes["ID"].Value == "199"
           && (cBusinessObjects.GetStato(78, IDCliente, IDSessione) == "2"
           || cBusinessObjects.GetStato(77, IDCliente, IDSessione) == "2")))
        {
          MessageBox.Show("Diversi tipi di MATERIALITA' non possono essere " +
            "COMPLETATI contemporaneamente.");
          return;
        }
        if (
          (IDTree == "1" && node.Attributes["ID"].Value == "254"
           && cBusinessObjects.GetStato(70, IDCliente, IDSessione) == "2")
          || (IDTree == "1" && node.Attributes["ID"].Value == "70"
           && cBusinessObjects.GetStato(254, IDCliente, IDSessione) == "2"))
        {
          MessageBox.Show("Il 2.8.7 o il 2.8.7 BIS non possono essere COMPLETATI " +
            "contemporaneamente.");
          return;
        }
        //if ( ( IDTree == "21" && node.Attributes["ID"].Value == "12" && ( _x.Document.SelectSingleNode( "/Dati//Dato[@ID='11']" ).Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode( "/Dati//Dato[@ID='10']" ).Attributes["Stato"].Value == "2" ) ) || ( IDTree == "21" && node.Attributes["ID"].Value == "11" && ( _x.Document.SelectSingleNode( "/Dati//Dato[@ID='10']" ).Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode( "/Dati//Dato[@ID='12']" ).Attributes["Stato"].Value == "2" ) ) || ( IDTree == "21" && node.Attributes["ID"].Value == "10" && ( _x.Document.SelectSingleNode( "/Dati//Dato[@ID='11']" ).Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode( "/Dati//Dato[@ID='12']" ).Attributes["Stato"].Value == "2" ) ) )
        //{
        //    MessageBox.Show( "Queste voci mutualmente esclusive. E' già presente un paragrafo COMPLETATO." );
        //    return;
        //}
        //------------------------------------ verifica solo un giudizio completato
        if (
          (IDTree == "21"
           && (node.Attributes["ID"].Value == "46" || node.Attributes["ID"].Value == "47")
           && (cBusinessObjects.GetStato(52, IDCliente, IDSessione)  == "2"
            || cBusinessObjects.GetStato(48, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(50, IDCliente, IDSessione) == "2"))
           || (IDTree == "21"
            && (node.Attributes["ID"].Value == "52" || node.Attributes["ID"].Value == "38"
              || node.Attributes["ID"].Value == "23" || node.Attributes["ID"].Value == "24"
              || node.Attributes["ID"].Value == "25")
            && (cBusinessObjects.GetStato(46, IDCliente, IDSessione) == "2"
              || cBusinessObjects.GetStato(48, IDCliente, IDSessione) == "2"
              || cBusinessObjects.GetStato(50, IDCliente, IDSessione) == "2"))
           || (IDTree == "21"
            && (node.Attributes["ID"].Value == "48" || node.Attributes["ID"].Value == "49")
            && (cBusinessObjects.GetStato(46, IDCliente, IDSessione) == "2"
              || cBusinessObjects.GetStato(52, IDCliente, IDSessione) == "2"
              || cBusinessObjects.GetStato(50, IDCliente, IDSessione) == "2"))
           || (IDTree == "21" && (node.Attributes["ID"].Value == "50" || node.Attributes["ID"].Value == "51")
            && (cBusinessObjects.GetStato(46, IDCliente, IDSessione) == "2"
              || cBusinessObjects.GetStato(52, IDCliente, IDSessione) == "2"
              || cBusinessObjects.GetStato(48, IDCliente, IDSessione) == "2")))
        {
          MessageBox.Show("Queste voci sono mutualmente esclusive. E' già presente " +
            "un giudizio COMPLETATO.");
          return;
        }
        if (
          (IDTree == "31"
           && (node.Attributes["ID"].Value == "37" || node.Attributes["ID"].Value == "38")
           && (cBusinessObjects.GetStato(48, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(41, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(44, IDCliente, IDSessione) == "2"))
          || (IDTree == "31"
           && (node.Attributes["ID"].Value == "48" || node.Attributes["ID"].Value == "22"
            || node.Attributes["ID"].Value == "23" || node.Attributes["ID"].Value == "24"
            || node.Attributes["ID"].Value == "25")
           && (cBusinessObjects.GetStato(37, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(41, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(44, IDCliente, IDSessione) == "2"))
          || (IDTree == "31"
           && (node.Attributes["ID"].Value == "41" || node.Attributes["ID"].Value == "42")
           && (cBusinessObjects.GetStato(37, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(48, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(44, IDCliente, IDSessione) == "2"))
          || (IDTree == "31"
           && (node.Attributes["ID"].Value == "44" || node.Attributes["ID"].Value == "45")
           && (cBusinessObjects.GetStato(37, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(41, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(48, IDCliente, IDSessione) == "2")))
        {
          MessageBox.Show("Queste voci sono mutualmente esclusive. E' già presente " +
            "un giudizio COMPLETATO.");
          return;
        }
        if (
          (IDTree == "23"
           && (node.Attributes["ID"].Value == "317" || node.Attributes["ID"].Value == "318")
           && (cBusinessObjects.GetStato(308, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(319, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(321, IDCliente, IDSessione) == "2"))
          || (IDTree == "23"
           && (node.Attributes["ID"].Value == "308" || node.Attributes["ID"].Value == "122"
            || node.Attributes["ID"].Value == "309" || node.Attributes["ID"].Value == "124")
           && (cBusinessObjects.GetStato(317, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(319, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(321, IDCliente, IDSessione) == "2"))
          || (IDTree == "23"
           && (node.Attributes["ID"].Value == "319" || node.Attributes["ID"].Value == "320")
           && (cBusinessObjects.GetStato(317, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(308, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(321, IDCliente, IDSessione) == "2"))
          || (IDTree == "23"
           && (node.Attributes["ID"].Value == "321" || node.Attributes["ID"].Value == "322")
           && (cBusinessObjects.GetStato(317, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(308, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(319, IDCliente, IDSessione)  == "2")))
        {
          MessageBox.Show("Queste voci sono mutualmente esclusive. E' già presente " +
            "un giudizio COMPLETATO.");
          return;
        }
        if (
          (IDTree == "21"
           && node.Attributes["ID"].Value == "17"
           && (cBusinessObjects.GetStato(18, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(19, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(20, IDCliente, IDSessione) == "2"))
          || (IDTree == "21" && node.Attributes["ID"].Value == "18"
           && (cBusinessObjects.GetStato(17, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(19, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(20, IDCliente, IDSessione) == "2"))
          || (IDTree == "21" && node.Attributes["ID"].Value == "19"
           && (cBusinessObjects.GetStato(17, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(18, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(20, IDCliente, IDSessione) == "2"))
          || (IDTree == "21" && node.Attributes["ID"].Value == "20"
           && (cBusinessObjects.GetStato(17, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(18, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(19, IDCliente, IDSessione) == "2")))
        {
          MessageBox.Show("Queste voci mutualmente esclusive. E' già presente un " +
            "paragrafo COMPLETATO.");
          return;
        }
        if (
          (IDTree == "21" && node.Attributes["ID"].Value == "5"
           && (cBusinessObjects.GetStato(30, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(31, IDCliente, IDSessione) == "2"))
          || (IDTree == "21" && node.Attributes["ID"].Value == "30"
           && (cBusinessObjects.GetStato(5, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(31, IDCliente, IDSessione) == "2"))
          || (IDTree == "21" && node.Attributes["ID"].Value == "31"
           && (cBusinessObjects.GetStato(5, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(30, IDCliente, IDSessione) == "2")))
        {
          MessageBox.Show("Queste voci mutualmente esclusive. E' già presente un " +
            "paragrafo COMPLETATO.");
          return;
        }
        if (
          (IDTree == "21" && node.Attributes["ID"].Value == "10"
           && (cBusinessObjects.GetStato(11, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(27, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(28, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(33, IDCliente, IDSessione) == "2"))
          || (IDTree == "21" && node.Attributes["ID"].Value == "11"
           && (cBusinessObjects.GetStato(10, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(27, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(28, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(33, IDCliente, IDSessione) == "2"))
          || (IDTree == "21" && node.Attributes["ID"].Value == "27"
           && (cBusinessObjects.GetStato(10, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(11, IDCliente, IDSessione) == "2"))
          || (IDTree == "21" && node.Attributes["ID"].Value == "28"
           && (cBusinessObjects.GetStato(10, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(11, IDCliente, IDSessione) == "2"))
          || (IDTree == "21" && node.Attributes["ID"].Value == "33"
           && (cBusinessObjects.GetStato(10, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(11, IDCliente, IDSessione) == "2")))
        {
          MessageBox.Show("Queste voci mutualmente esclusive. E' già presente un " +
            "paragrafo COMPLETATO.");
          return;
        }
        if (
          (IDTree == "21" && node.Attributes["ID"].Value == "28"
           && cBusinessObjects.GetStato(33, IDCliente, IDSessione) == "2")
          || (IDTree == "21" && node.Attributes["ID"].Value == "33"
           && cBusinessObjects.GetStato(28, IDCliente, IDSessione) == "2"))
        {
          MessageBox.Show("Queste voci mutualmente esclusive. E' già presente un " +
            "paragrafo COMPLETATO.");
          return;
        }
        if (
          (IDTree == "22" && node.Attributes["ID"].Value == "28"
           && (cBusinessObjects.GetStato(34, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(35, IDCliente, IDSessione) == "2"))
          || (IDTree == "22" && node.Attributes["ID"].Value == "34"
           && (cBusinessObjects.GetStato(28, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(35, IDCliente, IDSessione) == "2"))
          || (IDTree == "22" && node.Attributes["ID"].Value == "35"
           && (cBusinessObjects.GetStato(28, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(34, IDCliente, IDSessione) == "2")))
        {
          MessageBox.Show("Queste voci mutualmente esclusive. E' già presente un " +
            "paragrafo COMPLETATO.");
          return;
        }
        //------------------------------------------------------------ albero B + V
        if (
          (IDTree == "23" && node.Attributes["ID"].Value == "117"
           && (cBusinessObjects.GetStato(118, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(119, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(120, IDCliente, IDSessione) == "2"))
           || (IDTree == "23" && node.Attributes["ID"].Value == "118"
            && (cBusinessObjects.GetStato(117, IDCliente, IDSessione) == "2"
              || cBusinessObjects.GetStato(119, IDCliente, IDSessione) == "2"
              || cBusinessObjects.GetStato(120, IDCliente, IDSessione) == "2"))
           || (IDTree == "23" && node.Attributes["ID"].Value == "119"
            && (cBusinessObjects.GetStato(117, IDCliente, IDSessione) == "2"
              || cBusinessObjects.GetStato(118, IDCliente, IDSessione) == "2"
              || cBusinessObjects.GetStato(120, IDCliente, IDSessione) == "2"))
           || (IDTree == "23" && node.Attributes["ID"].Value == "120"
            && (cBusinessObjects.GetStato(117, IDCliente, IDSessione) == "2"
              || cBusinessObjects.GetStato(118, IDCliente, IDSessione) == "2"
              || cBusinessObjects.GetStato(119, IDCliente, IDSessione) == "2")))
        {
          MessageBox.Show("Queste voci mutualmente esclusive. E' già presente un " +
            "paragrafo COMPLETATO.");
          return;
        }
        if (
          IDTree == "23" && node.Attributes["ID"].Value == "117"
           && (cBusinessObjects.GetStato(122, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(123, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(124, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(125, IDCliente, IDSessione) == "2"))
        {
          if (MessageBox.Show("Sono presenti rilievi. Confermi il giudizio " +
            "positivo?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
          {
            ResetNodo();
            StatoBeforeSblocco = App.TipoTreeNodeStato.Sconosciuto;
            ConfiguraStatoNodo(App.TipoTreeNodeStato.Sconosciuto, false);
            //Load();
            if (SalvaDatiControllo(App.TipoTreeNodeStato.Sconosciuto) == false) return;
            ConsentiChiusuraFinestra = true;
            SessioneNow = SessioneHome;
            _x = new XmlDataProviderManager(Sessioni[SessioneHome].ToString());
            base.Close();
            return;
          }
        }
        if (IDTree == "21" && node.Attributes["ID"].Value == "17" && 
          (cBusinessObjects.GetStato(22, IDCliente, IDSessione) == "2" ||
          cBusinessObjects.GetStato(23, IDCliente, IDSessione) == "2" ||
          cBusinessObjects.GetStato(24, IDCliente, IDSessione) == "2" ||
          cBusinessObjects.GetStato(25, IDCliente, IDSessione) == "2"))
        {
          if (MessageBox.Show("Sono presenti rilievi. Confermi il giudizio positivo?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
          {
            ResetNodo();
            StatoBeforeSblocco = App.TipoTreeNodeStato.Sconosciuto;
            ConfiguraStatoNodo(App.TipoTreeNodeStato.Sconosciuto, false);
            //Load();
            if (SalvaDatiControllo(App.TipoTreeNodeStato.Sconosciuto) == false) return;
            ConsentiChiusuraFinestra = true;
            SessioneNow = SessioneHome;
            _x = new XmlDataProviderManager(Sessioni[SessioneHome].ToString());
            base.Close();
            return;
          }
        }
        if (
          (IDTree == "23" && node.Attributes["ID"].Value == "105"
           && (cBusinessObjects.GetStato(130, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(131, IDCliente, IDSessione) == "2"))
          || (IDTree == "23" && node.Attributes["ID"].Value == "130"
           && (cBusinessObjects.GetStato(105, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(131, IDCliente, IDSessione) == "2"))
          || (IDTree == "23" && node.Attributes["ID"].Value == "131"
           && (cBusinessObjects.GetStato(105, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(130, IDCliente, IDSessione) == "2")))
        {
          MessageBox.Show("Queste voci mutualmente esclusive. E' già presente un " +
            "paragrafo COMPLETATO.");
          return;
        }
        if (
          (IDTree == "23" && node.Attributes["ID"].Value == "110"
           && (cBusinessObjects.GetStato(111, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(127, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(128, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(133, IDCliente, IDSessione) == "2"))
          || (IDTree == "23" && node.Attributes["ID"].Value == "111"
           && (cBusinessObjects.GetStato(110, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(127, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(128, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(133, IDCliente, IDSessione) == "2"))
          || (IDTree == "23" && node.Attributes["ID"].Value == "127"
           && (cBusinessObjects.GetStato(110, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(111, IDCliente, IDSessione) == "2"))
          || (IDTree == "23" && node.Attributes["ID"].Value == "128"
           && (cBusinessObjects.GetStato(110, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(111, IDCliente, IDSessione) == "2"))
          || (IDTree == "23" && node.Attributes["ID"].Value == "133"
           && (cBusinessObjects.GetStato(110, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(111, IDCliente, IDSessione) == "2")))
        {
          MessageBox.Show("Queste voci mutualmente esclusive. E' già presente un " +
            "paragrafo COMPLETATO.");
          return;
        }
        if (
          (IDTree == "23" && node.Attributes["ID"].Value == "128"
           && cBusinessObjects.GetStato(133, IDCliente, IDSessione) == "2")
          || (IDTree == "23" && node.Attributes["ID"].Value == "133"
           && cBusinessObjects.GetStato(128, IDCliente, IDSessione) == "2"))
        {
          MessageBox.Show("Queste voci mutualmente esclusive. E' già presente un " +
            "paragrafo COMPLETATO.");
          return;
        }
        if (
          (IDTree == "23" && node.Attributes["ID"].Value == "228"
           && (cBusinessObjects.GetStato(234, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(235, IDCliente, IDSessione) == "2"))
          || (IDTree == "23" && node.Attributes["ID"].Value == "234"
           && (cBusinessObjects.GetStato(228, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(235, IDCliente, IDSessione) == "2"))
          || (IDTree == "23" && node.Attributes["ID"].Value == "235"
           && (cBusinessObjects.GetStato(228, IDCliente, IDSessione) == "2"
            || cBusinessObjects.GetStato(234, IDCliente, IDSessione) == "2")))
        {
          MessageBox.Show("Queste voci mutualmente esclusive. E' già presente un " +
            "paragrafo COMPLETATO.");
          return;
        }
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      

        if (SalvaDatiControllo(nuovo_stato) == false) return;
       
    
        StatoBeforeSblocco = App.TipoTreeNodeStato.Sconosciuto;
      if (ForzaturaADaCompletare == true)
      {
        ConfiguraStatoNodo(App.TipoTreeNodeStato.DaCompletare, true);
        ForzaturaADaCompletare = false;

      }
      else
      {
        ConfiguraStatoNodo(nuovo_stato, true);
      }

     

   }
        private void btn_Stato_Completato_Click(object sender, RoutedEventArgs e)
    {
        // TEAM
     
        GestioneSbloccoCartellaPerEsecutore();
       
      
        GestioneStatoCompletato(App.TipoTreeNodeStato.Completato);
     
        return;
    }

    private void btn_Stato_CancellaDati_Click(object sender, RoutedEventArgs e)
    {
      if (ApertoInSolaLettura)
      {
        MessageBox.Show(App.MessaggioSolaScritturaStato);
        return;
      }

      if (ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }

      if (MessageBox.Show("Si vuole davvero resettare completamente i dati?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        ResetNodo();

        StatoBeforeSblocco = App.TipoTreeNodeStato.Sconosciuto;

        ConfiguraStatoNodo(App.TipoTreeNodeStato.Sconosciuto, false);

        //Load();

        //if (SalvaDatiControllo(App.TipoTreeNodeStato.Sconosciuto) == false)
        //{
        //    return;
        //}

        ConsentiChiusuraFinestra = true;
        SessioneNow = SessioneHome;
        _x = new XmlDataProviderManager(Sessioni[SessioneHome].ToString());
        base.Close();
      }
    }

    private void btn_EsciSenzaSalvare_Click(object sender, RoutedEventArgs e)
    {
      SessioneNow = SessioneHome;
      NodeNow = NodeHome;

      //andrea
      if (NodoSolaLettura || ApertoInSolaLettura)
      {
        ConsentiChiusuraFinestra = true;

        base.Close();
        return;
      }

      if (Stato == App.TipoTreeNodeStato.Sconosciuto && OldStatoNodo == App.TipoTreeNodeStato.Sconosciuto)
      {
        ResetNodo();

        StatoBeforeSblocco = App.TipoTreeNodeStato.Sconosciuto;

        ConfiguraStatoNodo(App.TipoTreeNodeStato.Sconosciuto, false);

        //Load();

        //if (SalvaDatiControllo(App.TipoTreeNodeStato.Sconosciuto) == false)
        //{
        //    return;
        //}

        ConsentiChiusuraFinestra = true;
        SessioneNow = SessioneHome;
        _x = new XmlDataProviderManager(Sessioni[SessioneHome].ToString());
        base.Close();
      }

      _x = new XmlDataProviderManager(Sessioni[SessioneHome].ToString());

      //if (MessageBox.Show("Eventuali nuovi dati inseriti andranno persi, confermi?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
      //{
      //    return;
      //}

      XmlNode node = ((XmlNode)(Nodes[NodeNow]));
  
      //XmlDataProviderManager _d = new XmlDataProviderManager(App.AppDocumentiDataFile, true);

      //XmlNodeList xd = _d.Document.SelectNodes("//DOCUMENTI//DOCUMENTO[@Nuova='True']");

      //for (int i = 0; i < xd.Count; i++)
      //{
      //    xd[i].ParentNode.RemoveChild(xd[i]);
      //}

      //_d.Save();

      if (ShouldBEDaCompletare && OldStatoNodo != App.TipoTreeNodeStato.Completato && OldStatoNodo != App.TipoTreeNodeStato.NonApplicabile &&
        OldStatoNodo != App.TipoTreeNodeStato.NonApplicabileBucoTemplate && OldStatoNodo != App.TipoTreeNodeStato.CompletatoBloccoEsecutore)
      {
        ConfiguraStatoNodo(App.TipoTreeNodeStato.DaCompletare, true);
      }
      else
      {
        ConfiguraStatoNodo(OldStatoNodo, true);
      }
    }

    private void ConfiguraStatoNodoExitNoChange(App.TipoTreeNodeStato stato, bool uscita)
    {
      Stato = stato;

      if (Stato == App.TipoTreeNodeStato.Completato || Stato == App.TipoTreeNodeStato.DaCompletare || Stato == App.TipoTreeNodeStato.CompletatoBloccoEsecutore
        || Stato == App.TipoTreeNodeStato.NonApplicabile || Stato == App.TipoTreeNodeStato.NonApplicabileBucoTemplate)
      {
        ReadOnly = true;
      }
      else
      {
        if (SessioneNow == SessioneHome && NodeNow == NodeHome)
        {
          ReadOnly = ReadOnlyOLD;
        }
      }

      ConsentiChiusuraFinestra = true;
      SessioneNow = SessioneHome;
      //_x = new XmlDataProviderManager(Sessioni[SessioneHome].ToString());
      base.Close();
    }

  
    public void ConfiguraStatoNodo(App.TipoTreeNodeStato stato, bool uscita)
    {
          cBusinessObjects.logger.Info( " ConfiguraStatoNodo 1");
      


        if (Stato != stato)
      {
        Stato = stato;
        if (Stato == App.TipoTreeNodeStato.Completato || Stato == App.TipoTreeNodeStato.DaCompletare //|| Stato == App.TipoTreeNodeStato.BloccatoEsecutore
          || Stato == App.TipoTreeNodeStato.NonApplicabile || Stato == App.TipoTreeNodeStato.NonApplicabileBucoTemplate)
        {
          ReadOnly = true;
        }
        else
        {
          if (SessioneNow == SessioneHome && NodeNow == NodeHome)
          {
            ReadOnly = ReadOnlyOLD;
          }
        }
        if (SessioneNow == SessioneHome)
        {
      
        ArrayList ToBeChanged = new ArrayList();
          XmlNode node = ((XmlNode)(Nodes[NodeNow]));
        

          ToBeChanged.Add(node.Attributes["ID"].Value);
          foreach (XmlNode item in node.ChildNodes)
          {
            if (item.Attributes["ID"] != null)
            {
              ToBeChanged.Add(item.Attributes["ID"].Value);
            }
          }

          foreach (string item in ToBeChanged)
          {
                DataTable dstati = cBusinessObjects.GetData(int.Parse(item), typeof(StatoNodi));
                if (dstati.Rows.Count == 0)
                    dstati.Rows.Add(int.Parse(item), cBusinessObjects.idcliente, cBusinessObjects.idsessione);
                foreach (DataRow dt in dstati.Rows)
                {
                    dt["Stato"] = (Convert.ToInt32(stato)).ToString();
                }
                cBusinessObjects.SaveData(int.Parse(item), dstati, typeof(StatoNodi));
  
                m_isModified = true;
            }

        

        //_x.isModified = m_isModified;
        //_x.Save(uscita || m_isModified);
                }
      }
      else
      {
        if (Stato == App.TipoTreeNodeStato.Completato || Stato == App.TipoTreeNodeStato.DaCompletare //|| Stato == App.TipoTreeNodeStato.BloccatoEsecutore
          || Stato == App.TipoTreeNodeStato.NonApplicabile || Stato == App.TipoTreeNodeStato.NonApplicabileBucoTemplate)
        {
          ReadOnly = true;
        }
        else
        {
          if (SessioneNow == SessioneHome && NodeNow == NodeHome)
          {
            ReadOnly = ReadOnlyOLD;
          }
        }
      }
      if (uscita)
      {
        ConsentiChiusuraFinestra = true;
        SessioneNow = SessioneHome;
        //if (m_isModified)
        //{
        //  _x.isModified = true;
        //  _x.Save(true);m_isModified = false;
        //  StaticUtilities.PurgeXML(_x.File.Split('\\').Last());
        //}
        //_x = new XmlDataProviderManager(Sessioni[SessioneHome].ToString());
        base.Close();
         cBusinessObjects.logger.Info( " ConfiguraStatoNodo 6");
        
     
      }
      ReadOnly = false; // forzatura sblocca stato libero
    }

    // TEAM
    private void GestioneSbloccoCartellaPerEsecutore()
    {
      // se l'utente revisore clicca su completa o da completare per una cartella bloccata con la doppia spunta allora sblocca la cartella
      if (App.AppTipo != App.ModalitaApp.Team || App.AppUtente.RuoId != (int)App.RuoloDesc.Reviewer)
        return;
      if (cCartelle.IsCartellaBloccata(_cartellaxTeam, App.AppUtente.Id, IDCliente, true))
        cCartelle.SbloccaCartella(_cartellaxTeam, App.AppUtente.Id, IDCliente);
    }
    #endregion

    #region GESTIONE_EVENTI

    bool noload = false;

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
       
        // TEAM
        //if (bloccaCartellaOn)
        //{
        //	bloccaCartellaOn = false;
        //	m_isModified = true;
        //	return;
        //}

        if (SessioneNow != SessioneHome)
      {
        MessageBox.Show("Tornare alla sessione attiva per uscire.");
        e.Cancel = true;
        return;
      }

      if (!ReadOnly && !ConsentiChiusuraFinestra && Stato != App.TipoTreeNodeStato.Completato && Stato != App.TipoTreeNodeStato.CompletatoBloccoEsecutore
        && Stato != App.TipoTreeNodeStato.DaCompletare && Stato != App.TipoTreeNodeStato.NonApplicabile
              && Stato != App.TipoTreeNodeStato.NonApplicabileBucoTemplate)
      {
        if (ShouldBEDaCompletare == true)
        {
          ConfiguraStatoNodo(App.TipoTreeNodeStato.DaCompletare, false);
          OldStatoNodo = App.TipoTreeNodeStato.DaCompletare;
        }
        else
        {
          MessageBox.Show("Assegnare uno STATO per uscire.");
          e.Cancel = true;
          return;
        }
      }

      noload = true;
      if (SessioneNow != SessioneHome)
      {
        btn_NavBar_NodoHome_Click(sender, new RoutedEventArgs());
      }

      //if(SessioneNow != SessioneHome)
      //{
      //    SessioneNow = SessioneHome;
      //    NodeNow = NodeHome;

      //    _x = new XmlDataProviderManager(Sessioni[SessioneHome].ToString());

      //    Stato = App.TipoTreeNodeStato.Sconosciuto;
      //}
      if (m_isModified)
      {
        _x.isModified = true;
    //    _x.Save(true);
        m_isModified = false;
      }
       

     }

        #endregion

        #region TOOLBAR
      private void AnimateBackgroundColorSP(StackPanel btn, Color from, Color to, int seconds)
        {
            SolidColorBrush brush = new SolidColorBrush(from);

            btn.Background = brush;
            System.Windows.Media.Animation.ColorAnimation a = new System.Windows.Media.Animation.ColorAnimation();
            a.From = from;
            a.To = to;
            a.Duration = new Duration(TimeSpan.FromSeconds(seconds));
            a.AutoReverse = true;
            btn.Background.BeginAnimation(SolidColorBrush.ColorProperty, a);
        }


        private void AnimateBackgroundColor(Button btn, Color from, Color to, int seconds)
    {
      SolidColorBrush brush = new SolidColorBrush(from);

      btn.Background = brush;
      System.Windows.Media.Animation.ColorAnimation a = new System.Windows.Media.Animation.ColorAnimation();
      a.From = from;
      a.To = to;
      a.Duration = new Duration(TimeSpan.FromSeconds(seconds));
      a.AutoReverse = true;
      btn.Background.BeginAnimation(SolidColorBrush.ColorProperty, a);
    }

    private void AggiornaStatoBottoneSospesi()
    {
      XmlNode node = ((XmlNode)(Nodes[NodeNow]));
      DataTable datitmp = cBusinessObjects.GetData(int.Parse(node.Attributes["ID"].Value), typeof(TabellaSospesi));
      DataRow nodetmp = null;
      foreach (DataRow dd in datitmp.Rows)
        {
            nodetmp = dd;
        }
        if (nodetmp != null && nodetmp["SospesiTxt"].ToString() != "")
        {
            if (nodetmp["SospesiTxt"].ToString().Trim() != "")
            {
                  
          //btn_Stato_PrimaVisione.Focus();

          if (txtAlert.Text == "")
          {
            txtAlert.Text = "Presenza di: ";
          }

          if (!txtAlert.Text.Contains("Sospesi;"))
          {
            txtAlert.Text += "Sospesi; ";
          }

          AnimateBackgroundColorSP(btn_SOSPESISP, ButtonToolBarSelectedColor, ButtonToolBarPulseColor, 1);
          return;
        }
      }

      if (txtAlert.Text.Contains("Sospesi;"))
      {
        txtAlert.Text = txtAlert.Text.Replace("Sospesi; ", "");

        if (txtAlert.Text == "Presenza di: ")
        {
          txtAlert.Text = "";
        }
      }

      btn_SOSPESI.Background = btn_Stato_PrimaVisione.Background;
    }

    private void AggiornaStatoBottoneOsservazioniConclusive()
    {
         

            XmlNode node = ((XmlNode)(Nodes[NodeNow]));
     
      DataTable datitmp = cBusinessObjects.GetData(int.Parse(node.Attributes["ID"].Value), typeof(Osservazioni));
      DataRow nodetmp = null;
      foreach (DataRow dd in datitmp.Rows)
            {
                nodetmp = dd;
            }
      if (nodetmp != null && nodetmp["OsservazioniTxt"].ToString()!= "")
      {
        if (nodetmp["OsservazioniTxt"].ToString().Trim() != "")
        {
          //btn_Stato_PrimaVisione.Focus();

          if (txtAlert.Text == "")
          {
            txtAlert.Text = "Presenza di: ";
          }

          if (!txtAlert.Text.Contains("Commenti;"))
          {
            txtAlert.Text += "Commenti; ";
          }

         AnimateBackgroundColorSP(btn_OsservazioniConclusiveSP, ButtonToolBarSelectedColor, ButtonToolBarPulseColor, 1);
        return;
        }
      }

      if (txtAlert.Text.Contains("Commenti;"))
      {
        txtAlert.Text = txtAlert.Text.Replace("Commenti; ", "");

        if (txtAlert.Text == "Presenza di: ")
        {
          txtAlert.Text = "";
        }
      }

      btn_OsservazioniConclusive.Background = btn_Stato_PrimaVisione.Background;
    }

    private void btn_OsservazioniConclusive_Click(object sender, RoutedEventArgs e)
    {
      XmlNode node = ((XmlNode)(Nodes[NodeNow]));

      OsservazioniConclusive o = new OsservazioniConclusive();
      o.Owner = this;

      if (System.Windows.SystemParameters.PrimaryScreenWidth < 1100 || System.Windows.SystemParameters.PrimaryScreenHeight < 600)
      {
        o.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        o.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        o.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        o.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        o.MinHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        o.MinWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
      }
      else
      {
        o.Width = 1100;
        o.Height = 600;
      }

      o.ReadOnly = ReadOnly;

      o.Stato = Stato;

            
      o.Load(node.Attributes["ID"].Value, IDCliente,IDSessione);

      o.ShowDialog();

      AggiornaStatoBottoneOsservazioniConclusive();
    }

    private void AggiornaStatoBottoneModelliPredefiniti()
    {
      XmlNode node = ((XmlNode)(Nodes[NodeNow]));

      XmlDataProviderManager tmp_x = new XmlDataProviderManager(App.AppModelliFile, true);

      if (tmp_x.Document.SelectNodes("//MODELLI//MODELLO[@Tree='" + IDTree + "'][@Nodo='" + node.Attributes["ID"].Value + "']").Count > 0)
      {
        //btn_Stato_PrimaVisione.Focus();

        if (txtAlert.Text == "")
        {
          txtAlert.Text = "Presenza di: ";
        }

        if (!txtAlert.Text.Contains("Modelli;"))
        {
          txtAlert.Text += "Modelli; ";
        }

        AnimateBackgroundColor(btn_ModelliPredefiniti, ButtonToolBarSelectedColor, ButtonToolBarPulseColor, 1);

        btn_ModelliPredefiniti.IsEnabled = true;
      }
      else
      {
        btn_ModelliPredefiniti.Background = btn_Stato_PrimaVisione.Background;

        btn_ModelliPredefiniti.IsEnabled = false;
      }
    }

    private void btn_ModelliPredefiniti_Click(object sender, RoutedEventArgs e)
    {
      wDocumenti documenti = new wDocumenti();

      XmlNode node = ((XmlNode)(Nodes[NodeNow]));

      documenti.Titolo = "Modelli Predefiniti";
      documenti.Tipologia = TipoVisualizzazione.Modelli;
      documenti.Tree = IDTree;
      documenti.Cliente = IDCliente;
      documenti.Sessione = IDSessione;
      documenti.Nodo = node.Attributes["ID"].Value;

      documenti.Owner = this;

      if (System.Windows.SystemParameters.PrimaryScreenWidth < 1100 || System.Windows.SystemParameters.PrimaryScreenHeight < 600)
      {
        documenti.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        documenti.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        documenti.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        documenti.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        documenti.MinHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        documenti.MinWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
      }
      else
      {
        documenti.Width = 1100;
        documenti.Height = 600;
      }

      documenti.Load();
      documenti.ShowDialog();
    }

    private void AggiornaStatoBottoneDocumentiAssociat()
    {
        XmlNode node = ((XmlNode)(Nodes[NodeNow]));
        DataTable datitmp = cBusinessObjects.GetData(int.Parse(node.Attributes["ID"].Value), typeof(ArchivioDocumenti), cBusinessObjects.idcliente,cBusinessObjects.idsessione);
        if(datitmp.Rows.Count>0)
            {
                AnimateBackgroundColorSP(btn_DocumentiAssociatiSP, ButtonToolBarSelectedColor, ButtonToolBarPulseColor, 1);
                return;
            }
            else
            {
                btn_DocumentiAssociati.Background = btn_Stato_PrimaVisione.Background;
            }
    }

    private void AggiornaStatoBottoneIstruzioni()
    {
      XmlNode node = ((XmlNode)(Nodes[NodeNow]));
      string nota = "";

      if (node.Attributes["Nota"] != null)
      {
        nota = node.Attributes["Nota"].Value;
      }

      XmlDataProviderManager tmp_x = new XmlDataProviderManager(App.AppDocumentiDataFile, true);

      if (nota != "" && nota != "<P>&nbsp;</P>" && nota != "<P align=left>&nbsp;</P>" && nota != "<P class=MsoNormal style=\"MARGIN: 0cm 0cm 0pt; LINE-HEIGHT: normal\" align=left>&nbsp;</P>")
      {
        //btn_Stato_PrimaVisione.Focus();

        AnimateBackgroundColor(btn_NodoHelp, ButtonToolBarSelectedColor, ButtonToolBarPulseColor, 1);

        // E.B. disabilita visualizzazione iniziale della guida
        //istobeshowedGuida = true;

        return;
      }
      else
      {
        btn_NodoHelp.Background = btn_Stato_PrimaVisione.Background;
      }
    }

    private void btn_DocumentiAssociati_Click(object sender, RoutedEventArgs e)
    {
      //if (ReadOnly)
      //{
      //    MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione"); 
      //    return;
      //}

      wDocumenti documenti = new wDocumenti();

      XmlNode node = ((XmlNode)(Nodes[NodeNow]));

      documenti.ReadOnly = ReadOnly;
      documenti.Titolo = "Documenti Associati";
      documenti.Tipologia = TipoVisualizzazione.Documenti;
      documenti.Tree = IDTree;
      documenti.Cliente = IDCliente;
      documenti.Sessione = IDSessione;
      documenti.Nodo = node.Attributes["ID"].Value;
      documenti.Owner = this;

      if (System.Windows.SystemParameters.PrimaryScreenWidth < 1100 || System.Windows.SystemParameters.PrimaryScreenHeight < 600)
      {
        documenti.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        documenti.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        documenti.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        documenti.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        documenti.MinHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        documenti.MinWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
      }
      else
      {
        documenti.Width = 1100;
        documenti.Height = 600;
      }

      documenti.Load();
      documenti.ShowDialog();

      AggiornaStatoBottoneDocumentiAssociat();
    }

    //----------------------------------------------------------------------------+
    //                       btn_CopiaDaAltraSessione_Click                       |
    //----------------------------------------------------------------------------+
  
    private void btn_CopiaDaAltraSessione_Click(object sender, RoutedEventArgs e)
    {
      if (ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }
      // E.B.
      //--------------------------------------------------------------------------+
      //         Con la nuova logica è consentito modificare i dati anche         |
      //       nello stato "Completato" senza prima premere "Sblocca Stato"       |
      //--------------------------------------------------------------------------+
      //if (Stato == App.TipoTreeNodeStato.Completato)
      //{
      //  MessageBox.Show("La Carta di Lavoro è nello stato " +
      //    App.NomeTipoTreeNodeStato(Stato).ToUpper() +
      //    ". Occorre selezionare Sblocca Stato per modificare il contenuto.");
      //  return;
      //}
      try
      {
        IndiceSessioni o = new IndiceSessioni();
        XmlNode node = ((XmlNode)(Nodes[NodeNow]));

        o.Tree = IDTree;
        o.Cliente = IDCliente;
        o.node = node;
        o.Sessione = SessioniID[SessioneHome].ToString();
        o.Nodo = node.Attributes["ID"].Value;
        o.Owner = this;
        o.Load();
        o.ShowDialog();

        // E.B.
        //------------------------------------------------------------------------+
        //  La classe IndiceSessioni ha un nuovo campo "_isModified". Se i dati   |
        //   vengono effettivamente copiati da un' altra sessione, qui troviamo   |
        //           _isModified=true e bisogna quindi salvare i dati.            |
        //------------------------------------------------------------------------+
        if (o._isModified) { _x.isModified = true; _x.Save(true); }

        ConfiguraStatoNodo(App.TipoTreeNodeStato.Scrittura, false);
        Load();
        AggiornaStatoBottoneDocumentiAssociat();
        AggiornaStatoBottoneIstruzioni();
        AggiornaStatoBottoneModelliPredefiniti();
        // E.B.
        //------------------------------------------------------------------------+
        //   Non più necessario. Se i dati sono stati importati il salvataggio    |
        //        è già avvenuto. Altrimenti, non c' è niente da salvare.         |
        //------------------------------------------------------------------------+
        //btn_SalvaTemporaneo_Click(sender, e);
      }
      catch (Exception ex)
      {
        if (!App.m_bNoExceptionMsg)
        {
          string msg = "btn_CopiaDaAltraSessione_Click(): errore\n" + ex.Message;
          MessageBox.Show(msg);
        }
      }
    }


    //----------------------------------------------------------------------------+
    //                      btn_CopiaInSessioneAttiva_Click                       |
    //----------------------------------------------------------------------------+
   
    private void btn_CopiaInSessioneAttiva_Click(object sender, RoutedEventArgs e)
    {
      if (ReadOnlyHome)
      {
        MessageBox.Show(
          "La Carta di Lavoro nella quale si vuole copiare è in sola lettura.");
        return;
      }
      if (StatoHome == App.TipoTreeNodeStato.Completato)
      {
        MessageBox.Show("La Carta di Lavoro è nello stato " +
          App.NomeTipoTreeNodeStato(Stato).ToUpper() +
          ". Occorre selezionare Sblocca Stato per modificare il contenuto.");
        return;
      }
      if (MessageBox.Show("Sicuri di voler esportare i dati? i dati attualmente " +
        "presenti nella sessione attiva verranno sovrascritti",
        "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        XmlNode node = ((XmlNode)(Nodes[NodeNow]));
        string IDNodeList = node.Attributes["ID"].Value;
        if (node.Attributes["Tipologia"].Value == "Nodo Multiplo")
        {
          MasterFile mf = MasterFile.Create();
          Hashtable ht;
          string file = "";
          switch ((App.TipoFile)(System.Convert.ToInt32(IDTree)))
          {
            case App.TipoFile.Revisione:
              ht = mf.GetRevisione(SessioniID[SessioneNow].ToString());
              if (ht.Contains("File")) file = ht["File"].ToString();
              else file = App.AppTemplateTreeRevisione;
              break;
            case App.TipoFile.Verifica:
              ht = mf.GetVerifica(SessioniID[SessioneNow].ToString());
              if (ht.Contains("File")) file = ht["File"].ToString();
              else file = App.AppTemplateTreeVerifica;
              break;
            case App.TipoFile.Vigilanza:
              ht = mf.GetVigilanza(SessioniID[SessioneNow].ToString());
              if (ht.Contains("File")) file = ht["File"].ToString();
              else file = App.AppTemplateTreeVigilanza;
              break;
            case App.TipoFile.Incarico:
            case App.TipoFile.IncaricoCS:
            case App.TipoFile.IncaricoSU:
            case App.TipoFile.IncaricoREV:
              ht = mf.GetIncarico(SessioniID[SessioneNow].ToString());
              if (ht.Contains("File")) file = ht["File"].ToString();
              else file = App.AppTemplateTreeIncarico;
              break;
            case App.TipoFile.ISQC:
              ht = mf.GetISQC(SessioniID[SessioneNow].ToString());
              if (ht.Contains("File")) file = ht["File"].ToString();
              else file = App.AppTemplateTreeISQC;
              break;
            case App.TipoFile.Bilancio:
              ht = mf.GetBilancio(SessioniID[SessioneNow].ToString());
              if (ht.Contains("File")) file = ht["File"].ToString();
              else file = App.AppTemplateTreeBilancio;
              break;
            case App.TipoFile.Conclusione:
              ht = mf.GetConclusione(SessioniID[SessioneNow].ToString());
              if (ht.Contains("File")) file = ht["File"].ToString();
              else file = App.AppTemplateTreeConclusione;
              break;
            case App.TipoFile.Licenza:
            case App.TipoFile.Master:
            case App.TipoFile.Info:
            case App.TipoFile.Messagi:
            case App.TipoFile.ImportExport:
            case App.TipoFile.ImportTemplate:
            case App.TipoFile.BackUp:
            case App.TipoFile.Formulario:
            case App.TipoFile.ModellPredefiniti:
            case App.TipoFile.DocumentiAssociati:
            default:
              break;
          }
          XmlDataProviderManager _t =
            new XmlDataProviderManager(App.AppDataDataFolder + "\\" + file);
          foreach (XmlNode child in _t.Document.SelectSingleNode(
            "//Tree//Node[@ID='" + node.Attributes["ID"].Value + "']").ChildNodes)
          {
            if (child.Attributes["ID"] != null)
            {
              IDNodeList += "|" + child.Attributes["ID"].Value;
            }
          }
        }
        XmlDataProviderManager y = new XmlDataProviderManager(Sessioni[SessioneNow].ToString());
        XmlDataProviderManager z = new XmlDataProviderManager(Sessioni[SessioneHome].ToString());
        XmlNode NodoDaImportare, NodoDaSostituire, NodoImportato;
        z.isModified = false;
        foreach (string nodeID in IDNodeList.Split('|'))
        {
          NodoDaImportare = y.Document.SelectSingleNode("/Dati//Dato[@ID='" + nodeID + "']");
          NodoDaSostituire = z.Document.SelectSingleNode("/Dati//Dato[@ID='" + nodeID + "']");
          NodoImportato = z.Document.ImportNode(NodoDaImportare, true);
          NodoDaSostituire.ParentNode.AppendChild(NodoImportato);
          NodoDaSostituire.ParentNode.RemoveChild(NodoDaSostituire);
          StaticUtilities.MarkNodeAsModified(NodoImportato, App.OBJ_MOD);
          z.isModified = true;
        }
        if (z.isModified) z.Save(true);
        //------------------------------------------ copia documenti permanenti
        XmlDataProviderManager _d = new XmlDataProviderManager(
          App.AppDocumentiDataFile, true);
        XmlNodeList nodelisttmp;

        // cancellare documenti permanenti attuali in indice e in cartella documenti
        nodelisttmp = _d.Document.SelectNodes(
          "//DOCUMENTI//DOCUMENTO[@Cliente='" + IDCliente + "'][@Sessione='" +
          SessioniID[SessioneHome].ToString() + "'][@Tree='" +
          IDTree + "'][@Nodo='" + node.Attributes["ID"].Value + "'][@Tipo='1']");
        foreach (XmlNode nodetmp in nodelisttmp)
        {
          File.Delete(App.AppDocumentiFolder + "\\" + nodetmp.Attributes["File"].Value);
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("doc.DeleteDocumento", conn);
            cmd.Parameters.AddWithValue("@rec", nodetmp.OuterXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SQL call 'doc.DeleteDocumento ' failed: errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }

        nodelisttmp = _d.Document.SelectNodes(
        "//DOCUMENTI//DOCUMENTO[@Cliente='" + IDCliente + "'][@Sessione='" +
        SessioniID[SessioneNow].ToString() + "'][@Tree='" +
        IDTree + "'][@Nodo='" + node.Attributes["ID"].Value + "'][@Tipo='1']");
        int newID;
        XmlNode root, newNode;
        string nuovonomefile, ext;
        foreach (XmlNode nodetmp in nodelisttmp)
        {
          FileInfo f_d = new FileInfo(
            App.AppDocumentiFolder + "\\" + nodetmp.Attributes["File"].Value);
          if (f_d.Exists)
          {
            //string nuovonomefile = nodetmp.Attributes["File"].Value.Split('.').First() +
            //  "(Copia)." + nodetmp.Attributes["File"].Value.Split('.').Last();
            root = _d.Document.SelectSingleNode("//DOCUMENTI");
            newID = Convert.ToInt32(root.Attributes["LastID"].Value) + 1;

            ext = nodetmp.Attributes["File"].Value.Split('.').Last();
            nuovonomefile = newID.ToString();
            if (!string.IsNullOrEmpty(ext)) nuovonomefile += "." + ext;
            f_d.CopyTo(App.AppDocumentiFolder + "\\" + nuovonomefile);

            newNode = nodetmp.CloneNode(true);
            newNode.Attributes["ID"].Value = newID.ToString();
            newNode.Attributes["File"].Value = nuovonomefile;
            newNode.Attributes["Sessione"].Value = SessioniID[SessioneHome].ToString();

            using (SqlConnection conn = new SqlConnection(App.connString))
            {
              conn.Open();
              SqlCommand cmd = new SqlCommand("doc.NewDocumento", conn);
              cmd.Parameters.AddWithValue("@rec", newNode.OuterXml);
              cmd.CommandType = CommandType.StoredProcedure;
              cmd.CommandTimeout = App.m_CommandTimeout;
              try { cmd.ExecuteNonQuery(); }
              catch (Exception ex)
              {
                if (!App.m_bNoExceptionMsg)
                {
                  string msg = "SQL call 'doc.NewDocumento' failed: errore\n" + ex.Message;
                  MessageBox.Show(msg);
                }
              }
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
        NodeNow = NodeHome;
        SessioneNow = SessioneHome;
        if (SessioneNow == SessioneHome)
        {
          //copiadaaltrasessionedebug
          btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Visible;
          //btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
          btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Collapsed;
        }
        else
        {
          btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
          btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Visible;
        }
        if (_IDNodo == "128" || (IDTree != "2" && _IDNodo == "29") ||
          (IDTree == "1" && _IDNodo == "70") ||
          (IDTree == "1" && _IDNodo == "265") || _IDNodo == "77" ||
          _IDNodo == "78" || _IDNodo == "199")
        {
          btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
          btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Collapsed;
        }
        _x = new XmlDataProviderManager(Sessioni[SessioneNow].ToString());
        ReadOnly = true;
        btn_Stato_NonApplicabile.IsEnabled = true;
        btn_Stato_DaCompletare.IsEnabled = true;
        btn_Stato_Completato.IsEnabled = true;
                //btn_Stato_SbloccaNodo.IsEnabled = true;
                btn_SalvaTemporaneo.IsEnabled = true;

        btn_EsciSenzaSalvare.IsEnabled = true;

        btn_Stato_PrimaVisione.IsEnabled = true;
                if (cBusinessObjects.ReadOnlyControls)
                {
                    btn_Stato_NonApplicabile.IsEnabled = false;
                    btn_Stato_DaCompletare.IsEnabled = false;
                    btn_Stato_Completato.IsEnabled = false;
                    //btn_Stato_SbloccaNodo.IsEnabled = true;

                    btn_SalvaTemporaneo.IsEnabled = false;
                    btn_EsciSenzaSalvare.IsEnabled = false;
                    btn_Stato_PrimaVisione.IsEnabled = false;
                }
                Load();
        SalvaDatiControllo(App.TipoTreeNodeStato.Completato);
        AggiornaStatoBottoneDocumentiAssociat();
        AggiornaStatoBottoneIstruzioni();
        AggiornaStatoBottoneModelliPredefiniti();
      }
    }

    private void btn_StampaPDF_Click(object sender, RoutedEventArgs e)
    {

      try
      {
        if (Sessioni[SessioneHome] != null)
        {
          _x = new XmlDataProviderManager(Sessioni[SessioneHome].ToString());
        }
      }
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
      catch (Exception ex)
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata
      {

        // throw;
      }



      switch ((App.TipoFile)(System.Convert.ToInt32(IDTree)))
      {
        case App.TipoFile.RelazioneV:
        case App.TipoFile.RelazioneBV:
        case App.TipoFile.RelazioneB:
        case App.TipoFile.RelazioneVC:
        case App.TipoFile.RelazioneBC:

          App.TipoTreeNodeStato StatoBeforePrint = Stato;
          ConfiguraStatoNodo(App.TipoTreeNodeStato.Completato, false);

          SalvaDatiControllo(App.TipoTreeNodeStato.Sconosciuto);

          XmlNode removable = ((WindowWorkAreaTree)Owner)._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']");
          XmlNode imported = ((WindowWorkAreaTree)Owner)._x.Document.ImportNode(_x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']"), true);
          removable.ParentNode.ReplaceChild(imported, removable);
          if (((WindowWorkAreaTree)Owner)._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']").Attributes["Stato"] == null)
          {
            XmlAttribute xattr = ((WindowWorkAreaTree)Owner)._x.Document.CreateAttribute("Stato");
            ((WindowWorkAreaTree)Owner)._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']").Attributes.Append(xattr);
          }
              ((WindowWorkAreaTree)Owner)._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']").Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString();
          ((WindowWorkAreaTree)Owner)._x.Save();

          ((WindowWorkAreaTree)Owner).StampaTemporanea = true;
          ((WindowWorkAreaTree)Owner).btn_StampaReport_Click(sender, e);

          ConfiguraStatoNodo(StatoBeforePrint, false);
          return;
        case App.TipoFile.Incarico:
            case App.TipoFile.IncaricoCS:
            case App.TipoFile.IncaricoSU:
            case App.TipoFile.IncaricoREV:
          XmlNode nodehere = ((WindowWorkAreaTree)Owner).TreeXmlProvider.Document.SelectSingleNode("//Node[@ID='" + _IDNodo + "']"); ;
          bool tobedonelettera = false;
          bool tobedoneletteracollegio = false;
          while (nodehere != null && nodehere.Attributes["ID"] != null)
          {
            if (nodehere.Attributes["ID"].Value == "142")
            {
              tobedonelettera = true;
            }

            if (nodehere.Attributes["ID"].Value == "2016142")
            {
              tobedoneletteracollegio = true;
            }

            nodehere = nodehere.ParentNode;
          }

          if (tobedonelettera || tobedoneletteracollegio)
          {
            App.TipoTreeNodeStato StatoBeforePrint2 = Stato;
            ConfiguraStatoNodo(App.TipoTreeNodeStato.Completato, false);

            SalvaDatiControllo(App.TipoTreeNodeStato.Sconosciuto);

            XmlNode removable2 = ((WindowWorkAreaTree)Owner)._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']");
            XmlNode imported2 = ((WindowWorkAreaTree)Owner)._x.Document.ImportNode(_x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']"), true);
            removable2.ParentNode.ReplaceChild(imported2, removable2);
            if (((WindowWorkAreaTree)Owner)._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']").Attributes["Stato"] == null)
            {
              XmlAttribute xattr = ((WindowWorkAreaTree)Owner)._x.Document.CreateAttribute("Stato");
              ((WindowWorkAreaTree)Owner)._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']").Attributes.Append(xattr);
            }
              ((WindowWorkAreaTree)Owner)._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']").Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString();
            ((WindowWorkAreaTree)Owner)._x.Save();

            ((WindowWorkAreaTree)Owner).StampaTemporanea = true;
            if (tobedonelettera)
            {
              ((WindowWorkAreaTree)Owner).btn_StampaLetteraIncarico_Click(sender, e);
            }

            if (tobedoneletteracollegio)
            {
              ((WindowWorkAreaTree)Owner).btn_StampaLetteraIncaricoCollegio_Click(sender, e);
            }

            ConfiguraStatoNodo(StatoBeforePrint2, false);
            return;
          }
          break;
        case App.TipoFile.ISQC:
          XmlNode nodehereISQC = ((WindowWorkAreaTree)Owner).TreeXmlProvider.Document.SelectSingleNode("//Node[@ID='" + _IDNodo + "']"); ;
          bool tobedoneCodiceEtico = false;
          while (nodehereISQC != null && nodehereISQC.Attributes["ID"] != null)
          {
            if (nodehereISQC.Attributes["ID"].Value == "142")
            {
              tobedoneCodiceEtico = true;
            }
            nodehereISQC = nodehereISQC.ParentNode;
          }

          if (tobedoneCodiceEtico)
          {
            App.TipoTreeNodeStato StatoBeforePrint2 = Stato;
            ConfiguraStatoNodo(App.TipoTreeNodeStato.Completato, false);

            SalvaDatiControllo(App.TipoTreeNodeStato.Sconosciuto);

            XmlNode removable2 = ((WindowWorkAreaTree)Owner)._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']");
            XmlNode imported2 = ((WindowWorkAreaTree)Owner)._x.Document.ImportNode(_x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']"), true);
            removable2.ParentNode.ReplaceChild(imported2, removable2);

            if (((WindowWorkAreaTree)Owner)._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']").Attributes["Stato"] == null)
            {
              XmlAttribute xattr = ((WindowWorkAreaTree)Owner)._x.Document.CreateAttribute("Stato");
              ((WindowWorkAreaTree)Owner)._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']").Attributes.Append(xattr);
            }

              ((WindowWorkAreaTree)Owner)._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']").Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString();
            ((WindowWorkAreaTree)Owner)._x.Save();

            ((WindowWorkAreaTree)Owner).StampaTemporanea = true;
            ((WindowWorkAreaTree)Owner).btn_StampaCodiceEtico_Click(sender, e);

            ConfiguraStatoNodo(StatoBeforePrint2, false);
            return;
          }
          break;
        case App.TipoFile.Conclusione:
          XmlNode nodehere2 = ((WindowWorkAreaTree)Owner).TreeXmlProvider.Document.SelectSingleNode("//Node[@ID='" + _IDNodo + "']"); ;
          bool tobedonelettera2 = false;
          bool tobedonelettera3 = false;
          while (nodehere2 != null && nodehere2.Attributes["ID"] != null)
          {
            if (nodehere2.Attributes["ID"].Value == "261")
            {
              tobedonelettera2 = true;
            }
            else if (nodehere2.Attributes["ID"].Value == "281")
            {
              tobedonelettera3 = true;
            }
            nodehere2 = nodehere2.ParentNode;
          }

          if (tobedonelettera2 || tobedonelettera3)
          {
            App.TipoTreeNodeStato StatoBeforePrint2 = Stato;
            ConfiguraStatoNodo(App.TipoTreeNodeStato.Completato, false);

            SalvaDatiControllo(App.TipoTreeNodeStato.Sconosciuto);

            XmlNode removable2 = ((WindowWorkAreaTree)Owner)._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']");
            XmlNode imported2 = ((WindowWorkAreaTree)Owner)._x.Document.ImportNode(_x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']"), true);
            removable2.ParentNode.ReplaceChild(imported2, removable2);
            if (((WindowWorkAreaTree)Owner)._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']").Attributes["Stato"] == null)
            {
              XmlAttribute xattr = ((WindowWorkAreaTree)Owner)._x.Document.CreateAttribute("Stato");
              ((WindowWorkAreaTree)Owner)._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']").Attributes.Append(xattr);
            }
              ((WindowWorkAreaTree)Owner)._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + _IDNodo + "']").Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString();
            ((WindowWorkAreaTree)Owner)._x.Save();

            ((WindowWorkAreaTree)Owner).StampaTemporanea = true;

            if (tobedonelettera2)
            {
              ((WindowWorkAreaTree)Owner).btn_StampaLetteraAttestazione_Click(sender, e);
            }
            else if (tobedonelettera3)
            {
              ((WindowWorkAreaTree)Owner).btn_StampaManagementLetter_Click(sender, e);
            }

            ConfiguraStatoNodo(StatoBeforePrint2, false);
            return;
          }
          break;
        default:
          break;
      }

      if (!NodoSolaLettura && (Stato != App.TipoTreeNodeStato.Completato && Stato != App.TipoTreeNodeStato.DaCompletare && Stato != App.TipoTreeNodeStato.CompletatoBloccoEsecutore) && MessageBox.Show("La stampa richiede il salvataggio dei dati inseriti. Procedere?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
      {
        return;
      }

      //Process wait - START
      //ProgressWindow pw = new ProgressWindow();
     

      
      Nodes[NodeNow] = xdpm.Document.SelectSingleNode("//Node[@ID=" + ((XmlNode)(Nodes[NodeNow])).Attributes["ID"].Value + "]");
      XmlNode node = ((XmlNode)(Nodes[NodeNow]));

      if (SalvaDatiControllo(App.TipoTreeNodeStato.Sconosciuto) == false)
      {
        return;
      }

      if (Stato == App.TipoTreeNodeStato.Sconosciuto)
      {
        ConfiguraStatoNodo(App.TipoTreeNodeStato.DaCompletare, false);
        _x.Save();
        Load();
      }

      MasterFile mf = MasterFile.Create();
      Hashtable cliente = mf.GetAnagrafica(Convert.ToInt32(IDCliente));

      //WordLib wl = new WordLib();
      RTFLib wl = new RTFLib();

      wl.TemplateFileCompletePath = App.AppTemplateStampa;

      switch ((App.TipoFile)(System.Convert.ToInt32(IDTree)))
      {
        case App.TipoFile.Revisione:
          wl.TemplateFileCompletePath = App.AppTemplateStampa;
          break;
        case App.TipoFile.Verifica:
          wl.TemplateFileCompletePath = App.AppTemplateStampa;
          //wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
          //wl.Watermark = false;
          wl.TitoloVerbale = false;
          wl.TabelleSenzaRigheVuote = true;
          wl.SenzaStampareTitoli = true;
          wl.StampaTemporanea = true;
          break;
        case App.TipoFile.Vigilanza:
          wl.TemplateFileCompletePath = App.AppTemplateStampa;
          //wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
          //wl.Watermark = false;
          wl.TitoloVerbale = false;
          wl.TabelleSenzaRigheVuote = true;
          wl.SenzaStampareTitoli = true;
          wl.StampaTemporanea = true;
          break;
        case App.TipoFile.PianificazioniVerifica:
          wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
          wl.Watermark = false;
          wl.TabelleSenzaRigheVuote = true;
          wl.StampaTemporanea = true;
          break;
        case App.TipoFile.PianificazioniVigilanza:
          wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
          wl.Watermark = false;
          wl.TabelleSenzaRigheVuote = true;
          wl.StampaTemporanea = true;
          break;
        case App.TipoFile.Incarico:
            case App.TipoFile.IncaricoCS:
            case App.TipoFile.IncaricoSU:
            case App.TipoFile.IncaricoREV:
          wl.TemplateFileCompletePath = App.AppTemplateStampa;
          break;
        case App.TipoFile.ISQC:
          wl.TemplateFileCompletePath = App.AppTemplateStampa;
          break;
        case App.TipoFile.Bilancio:
          wl.TemplateFileCompletePath = App.AppTemplateStampa;
          break;
        case App.TipoFile.Licenza:
        case App.TipoFile.Master:
        case App.TipoFile.Info:
        case App.TipoFile.Messagi:
        case App.TipoFile.ImportExport:
        case App.TipoFile.ImportTemplate:
        case App.TipoFile.BackUp:
        case App.TipoFile.Formulario:
        case App.TipoFile.ModellPredefiniti:
        case App.TipoFile.DocumentiAssociati:
        default:
          break;
      }

      //wl.Open(new Hashtable(), cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), SessioniTitoli[SessioneNow].ToString(), node.OwnerDocument.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, false);
      wl.Open(cliente, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(),
        SessioniTitoli[SessioneNow].ToString(),
        node.OwnerDocument.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, false, IDCliente);
            wl.checkTBD = false;
      wl.Add(node, IDCliente, IDTree, IDSessione, Sessioni[SessioneNow].ToString());

      if (wl.Watermark == false)
      {
        wl.Save("");
      }
      else
      {
        string Intestazione = cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(SessioniTitoli[SessioneNow].ToString());
        wl.SavePDF(Intestazione, this);
      }

      wl.Close();

      //Process wait - STOP
      //pw.Close();
    }

    private string ConvertDate(string date)
    {
      if (date.ToString().Contains(" - "))
      {
        ;
      }
      else
      {
        date = date.ToString().Replace("01/01/", "");

        date = date.ToString().Contains("31/12/") ? date.ToString().Replace("31/12/", "") + " / " + (Convert.ToInt32(date.ToString().Replace("31/12/", "")) + 1).ToString() : date;
      }

      return date;
    }

    private void btn_GuidaRevisoft_Click(object sender, RoutedEventArgs e)
    {
      GuidaRevisoft(true);
    }

    private void GuidaRevisoft(bool posizioneMouse)
    {
      XmlNode node;
      string nota = "";

      try
      {
        node = ((XmlNode)(Nodes[NodeNow]));
        nota = node.Attributes["Nota"].Value;
        string fileguida = AppDomain.CurrentDomain.BaseDirectory + "/guida/" + node.Attributes["Codice"].Value + ".htm";
        if( File.Exists(fileguida))
           nota = File.ReadAllText( fileguida);
      }
      catch (Exception ex)
      {
       
      }


      wGuidaRevisoft w = new wGuidaRevisoft();
      w.Owner = Window.GetWindow(this);

      if (System.Windows.SystemParameters.PrimaryScreenWidth < 1100 || System.Windows.SystemParameters.PrimaryScreenHeight < 600)
      {
        w.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        w.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        w.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        w.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        w.MinHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        w.MinWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
      }
      else
      {
        w.Width = 1100;
        w.Height = 600;
      }

      w.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
      //if (posizioneMouse)
      //{
      //    w.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;

      //    Point p = Mouse.GetPosition(this);
      //    switch (base.WindowState)
      //    {
      //        case System.Windows.WindowState.Normal:
      //            w.Top = this.Top + p.Y;
      //            w.Left = this.Left + p.X;
      //            break;
      //        case System.Windows.WindowState.Maximized:
      //            w.Top = p.Y;
      //            w.Left = p.X;
      //            break;
      //    }
      //}
      //else
      //{
      //    w.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
      //}

      if (nota != "" && nota != "<P align=left>&nbsp;</P>" && nota != "<P class=MsoNormal style=\"MARGIN: 0cm 0cm 0pt; LINE-HEIGHT: normal\" align=left>&nbsp;</P>")
      {
        w.testoHtml = nota;
      }
      else
      {
        w.testoHtml = "<html><body>Nessun aiuto disponibile per la Carta di Lavoro selezionata</body></html>";
      }

      w.MostraGuida();
      w.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                         btn_NavBar_NodoHome_Click                          |
    //----------------------------------------------------------------------------+
  

    private void btn_NavBar_NodoHome_Click(object sender, RoutedEventArgs e)
    {
      stpAreaLavoro.IsEnabled = true;
      cBusinessObjects.ReadOnlyControls = false;

      NodeNow = NodeHome;
      SessioneNow = SessioneHome;
      IDSessione = SessioniID[SessioneNow].ToString();

      if (SessioneNow == SessioneHome)
      {
        //copiadaaltrasessionedebug
        btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Visible;
        //btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Collapsed;
      }
      else
      {
        btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Visible;
      }

      if (_IDNodo == "128" || (IDTree != "2" && _IDNodo == "29") ||
        (IDTree == "1" && _IDNodo == "70") ||
        (IDTree == "1" && _IDNodo == "265") || _IDNodo == "77" ||
        _IDNodo == "78" || _IDNodo == "199")
      {
        btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Collapsed;
      }

      _x = new XmlDataProviderManager(Sessioni[SessioneNow].ToString());

      ReadOnly = ReadOnlyHome;
      Stato = StatoHome;

      btn_Stato_NonApplicabile.IsEnabled = true;
      btn_Stato_DaCompletare.IsEnabled = true;
      btn_Stato_Completato.IsEnabled = true;
            //btn_Stato_SbloccaNodo.IsEnabled = true;

      btn_SalvaTemporaneo.IsEnabled = true;
      btn_EsciSenzaSalvare.IsEnabled = true;
      btn_Stato_PrimaVisione.IsEnabled = true;
      if(cBusinessObjects.ReadOnlyControls)
            {
                btn_Stato_NonApplicabile.IsEnabled = false;
                btn_Stato_DaCompletare.IsEnabled = false;
                btn_Stato_Completato.IsEnabled = false;
                //btn_Stato_SbloccaNodo.IsEnabled = true;

                btn_SalvaTemporaneo.IsEnabled = false;
                btn_EsciSenzaSalvare.IsEnabled = false;
                btn_Stato_PrimaVisione.IsEnabled = false;
            }

      if (noload == false) Load();
    }
    //----------------------------------------------------------------------------+
    //                         btn_NavBar_NodoPrev_Click                          |
    //----------------------------------------------------------------------------+
   

    private void btn_NavBar_NodoPrev_Click(object sender, RoutedEventArgs e)
    {
      if (SessioneNow == SessioneHome)
      {
                stpAreaLavoro.IsEnabled = true;
                cBusinessObjects.ReadOnlyControls = false;
                //copiadaaltrasessionedebug
                btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Visible;
        //btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Collapsed;
      }
      else
      {
                stpAreaLavoro.IsEnabled = false;
                cBusinessObjects.ReadOnlyControls = true;
                btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Visible;
      }

      if (_IDNodo == "128" || (IDTree != "2" && _IDNodo == "29") ||
        (IDTree == "1" && _IDNodo == "70") ||
        (IDTree == "1" && _IDNodo == "265") || _IDNodo == "77" ||
        _IDNodo == "78" || _IDNodo == "199")
      {
        btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Collapsed;
      }
      NodeNow--;
      ReadOnly = true;
      Load();
    }
    //----------------------------------------------------------------------------+
    //                         btn_NavBar_NodoNext_Click                          |
    //----------------------------------------------------------------------------+
   
    private void btn_NavBar_NodoNext_Click(object sender, RoutedEventArgs e)
    {
      if (SessioneNow == SessioneHome)
      {
        //copiadaaltrasessionedebug
        btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Visible;
        //btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Collapsed;
      }
      else
      {
        btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Visible;
      }

      if (_IDNodo == "128" || (IDTree != "2" && _IDNodo == "29") ||
        (IDTree == "1" && _IDNodo == "70") ||
        (IDTree == "1" && _IDNodo == "265") || _IDNodo == "77" ||
        _IDNodo == "78" || _IDNodo == "199")
      {
        btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Collapsed;
      }
      NodeNow++;
      ReadOnly = true;
      Load();
    }

    //----------------------------------------------------------------------------+
    //                       btn_NavBar_SessionePrev_Click                        |
    //----------------------------------------------------------------------------+
    

    private void btn_NavBar_SessionePrev_Click(object sender, RoutedEventArgs e)
    {

        if (SessioneNow == SessioneHome)
        {
                SalvaDatiControllo(App.TipoTreeNodeStato.Sconosciuto);
        }

        if (_xHome == null && SessioneNow == SessioneHome)
        {
            _xHome = _x.Clone();
        }
              
      SessioneNow--;
      _x = new XmlDataProviderManager(Sessioni[SessioneNow].ToString());
      if (StatoHomeDone == false)
      {
        ReadOnlyHome = ReadOnly;
        StatoHome = Stato;
        StatoHomeDone = true;
      }
      if (SessioneNow == SessioneHome)
      {
        _x = _xHome.Clone();
                stpAreaLavoro.IsEnabled = true;
                cBusinessObjects.ReadOnlyControls = false;
                //copiadaaltrasessionedebug
                btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Visible;
        //btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Collapsed;

        ReadOnly = ReadOnlyHome;
        Stato = StatoHome;

        btn_Stato_NonApplicabile.IsEnabled = true;
        btn_Stato_DaCompletare.IsEnabled = true;
        btn_Stato_Completato.IsEnabled = true;
                //btn_Stato_SbloccaNodo.IsEnabled = true;
                btn_SalvaTemporaneo.IsEnabled = true;
        btn_EsciSenzaSalvare.IsEnabled = true;
        btn_Stato_PrimaVisione.IsEnabled = true;
      }
      else
      {
                stpAreaLavoro.IsEnabled = false;
                cBusinessObjects.ReadOnlyControls = true;

                btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Visible;

        ReadOnly = true;

        btn_Stato_NonApplicabile.IsEnabled = false;
        btn_Stato_DaCompletare.IsEnabled = false;
        btn_Stato_Completato.IsEnabled = false;
                //btn_Stato_SbloccaNodo.IsEnabled = false;
                btn_SalvaTemporaneo.IsEnabled = false;
        btn_EsciSenzaSalvare.IsEnabled = false;
        btn_Stato_PrimaVisione.IsEnabled = false;
      }
            if (cBusinessObjects.ReadOnlyControls)
            {
                btn_Stato_NonApplicabile.IsEnabled = false;
                btn_Stato_DaCompletare.IsEnabled = false;
                btn_Stato_Completato.IsEnabled = false;
                //btn_Stato_SbloccaNodo.IsEnabled = true;

                btn_SalvaTemporaneo.IsEnabled = false;
                btn_EsciSenzaSalvare.IsEnabled = false;
                btn_Stato_PrimaVisione.IsEnabled = false;
            }

            if (_IDNodo == "128" || (IDTree != "2" && _IDNodo == "29") ||
        (IDTree == "1" && _IDNodo == "70") ||
        (IDTree == "1" && _IDNodo == "265") || _IDNodo == "77" ||
        _IDNodo == "78" || _IDNodo == "199")
      {
        btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Collapsed;
      }
      IDSessione = SessioniID[SessioneNow].ToString();
      Load();
      App.MessaggioSolaScrittura = "Carta in sola lettura, premere tasto ESCI";
      App.MessaggioSolaScritturaStato = "Carta in sola lettura, premere tasto ESCI";
      Hide();
      ShowDialog();
      App.MessaggioSolaScrittura =
        "Occorre selezionare Sblocca Stato per modificare il contenuto.";
      App.MessaggioSolaScritturaStato =
        "Sessione in sola lettura, impossibile modificare lo stato.";
    }
    //----------------------------------------------------------------------------+
    //                       btn_NavBar_SessioneNext_Click                        |
    //----------------------------------------------------------------------------+
   

    private void btn_NavBar_SessioneNext_Click(object sender, RoutedEventArgs e)
    {

        if (SessioneNow == SessioneHome)
        {
                SalvaDatiControllo(App.TipoTreeNodeStato.Sconosciuto);

        }


      if (_xHome == null && SessioneNow == SessioneHome)
      {
                _xHome = _x.Clone();
      }
      SessioneNow++;
      _x = new XmlDataProviderManager(Sessioni[SessioneNow].ToString());
      if (StatoHomeDone == false)
      {
        ReadOnlyHome = ReadOnly;
        StatoHome = Stato;
        StatoHomeDone = true;
      }
      if (SessioneNow == SessioneHome)
      {
        _x = _xHome.Clone();
        stpAreaLavoro.IsEnabled = true;
                cBusinessObjects.ReadOnlyControls = false;
                //copiadaaltrasessionedebug
                btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Visible;
        //btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Collapsed;

        ReadOnly = ReadOnlyHome;
        Stato = StatoHome;

        btn_Stato_NonApplicabile.IsEnabled = true;
        btn_Stato_DaCompletare.IsEnabled = true;
        btn_Stato_Completato.IsEnabled = true;
                //btn_Stato_SbloccaNodo.IsEnabled = true;

                btn_SalvaTemporaneo.IsEnabled = true;
        btn_EsciSenzaSalvare.IsEnabled = true;
        btn_Stato_PrimaVisione.IsEnabled = true;
      }
      else
      {
                stpAreaLavoro.IsEnabled = false;
                cBusinessObjects.ReadOnlyControls = true;
                btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Visible;

        ReadOnly = true;

        btn_Stato_NonApplicabile.IsEnabled = false;
        btn_Stato_DaCompletare.IsEnabled = false;
        btn_Stato_Completato.IsEnabled = false;
                //btn_Stato_SbloccaNodo.IsEnabled = false;

                btn_SalvaTemporaneo.IsEnabled = false;
        btn_EsciSenzaSalvare.IsEnabled = false;
        btn_Stato_PrimaVisione.IsEnabled = false;
      }
            if (cBusinessObjects.ReadOnlyControls)
            {
                btn_Stato_NonApplicabile.IsEnabled = false;
                btn_Stato_DaCompletare.IsEnabled = false;
                btn_Stato_Completato.IsEnabled = false;
                //btn_Stato_SbloccaNodo.IsEnabled = true;

                btn_SalvaTemporaneo.IsEnabled = false;
                btn_EsciSenzaSalvare.IsEnabled = false;
                btn_Stato_PrimaVisione.IsEnabled = false;
            }
            if (_IDNodo == "128" || (IDTree != "2" && _IDNodo == "29") ||
        (IDTree == "1" && _IDNodo == "70") ||
        (IDTree == "1" && _IDNodo == "265") || _IDNodo == "77" ||
        _IDNodo == "78" || _IDNodo == "199")
      {
        btn_CopiaDaAltraSessione2.Visibility = System.Windows.Visibility.Collapsed;
        btn_CopiaInSessioneAttiva.Visibility = System.Windows.Visibility.Collapsed;
      }
      IDSessione = SessioniID[SessioneNow].ToString();
      Load();
      App.MessaggioSolaScrittura = "Carta in sola lettura, premere tasto ESCI";
      App.MessaggioSolaScritturaStato = "Carta in sola lettura, premere tasto ESCI";
      Hide();
      ShowDialog();
      App.MessaggioSolaScrittura =
        "Occorre selezionare Sblocca Stato per modificare il contenuto.";
      App.MessaggioSolaScritturaStato =
        "Sessione in sola lettura, impossibile modificare lo stato.";
    }
    private void btn_AccediBilancio_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        MasterFile mf = MasterFile.Create();

        Hashtable htSelected = mf.GetAllBilancioAssociatoFromRevisioneFile(Sessioni[SessioneNow].ToString());

        wWorkAreaTreeLimited wWorkArea = new wWorkAreaTreeLimited();
        //Prisc
        try
        {
          wWorkArea.Owner = this;
        }
        catch (Exception ex)
        {
          string log = ex.Message;
        }

        wWorkArea.Width = this.Width * 90.0 / 100.0;
        wWorkArea.Height = this.Height * 90.0 / 100.0;

        wWorkArea.SelectedTreeSource = App.AppDataDataFolder + "\\" + htSelected["File"].ToString();
        wWorkArea.SelectedDataSource = App.AppDataDataFolder + "\\" + htSelected["FileData"].ToString();
        wWorkArea.ReadOnly = true;
        wWorkArea.TipoAttivita = App.TipoAttivita.Bilancio;
        wWorkArea.Cliente = (((Hashtable)(mf.GetAnagrafica(Convert.ToInt32(htSelected["Cliente"].ToString()))))["RagioneSociale"].ToString()) + " (C.F. " + (((Hashtable)(mf.GetAnagrafica(Convert.ToInt32(htSelected["Cliente"].ToString()))))["CodiceFiscale"].ToString()) + ")";
        wWorkArea.SessioneAlias = "";
        wWorkArea.SessioneFile = "";

        wWorkArea.IDTree = (Convert.ToInt32(App.TipoFile.Bilancio)).ToString();
        wWorkArea.IDCliente = htSelected["Cliente"].ToString();
        wWorkArea.IDSessione = htSelected["ID"].ToString();

        wWorkArea.SessioneFile = App.AppDataDataFolder + "\\" + htSelected["FileData"].ToString();
        wWorkArea.SessioneAlias = htSelected["Data"].ToString();// ConvertDataToEsercizio(item["Data"].ToString());
        wWorkArea.SessioneID = htSelected["ID"].ToString();

        wWorkArea.LoadTreeSource();
        wWorkArea.ShowDialog();

        //base.Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
    }

    private void btn_AccediRevisione_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        MasterFile mf = MasterFile.Create();

        Hashtable htSelected = mf.GetAllRevisioneAssociataFromBilancioFile(Sessioni[SessioneNow].ToString());

        wWorkAreaTreeLimited wWorkArea = new wWorkAreaTreeLimited();
        //Prisc
        try
        {
          wWorkArea.Owner = this;
        }
        catch (Exception ex)
        {
          string log = ex.Message;
        }

        wWorkArea.Width = this.Width * 90.0 / 100.0;
        wWorkArea.Height = this.Height * 90.0 / 100.0;

        wWorkArea.SelectedTreeSource = App.AppDataDataFolder + "\\" + htSelected["File"].ToString();
        wWorkArea.SelectedDataSource = App.AppDataDataFolder + "\\" + htSelected["FileData"].ToString();
        wWorkArea.ReadOnly = true;
        wWorkArea.TipoAttivita = App.TipoAttivita.Revisione;
        wWorkArea.Cliente = (((Hashtable)(mf.GetAnagrafica(Convert.ToInt32(htSelected["Cliente"].ToString()))))["RagioneSociale"].ToString()) + " (C.F. " + (((Hashtable)(mf.GetAnagrafica(Convert.ToInt32(htSelected["Cliente"].ToString()))))["CodiceFiscale"].ToString()) + ")";
        wWorkArea.SessioneAlias = "";
        wWorkArea.SessioneFile = "";

        wWorkArea.IDTree = (Convert.ToInt32(App.TipoFile.Revisione)).ToString();
        wWorkArea.IDCliente = htSelected["Cliente"].ToString();
        wWorkArea.IDSessione = htSelected["ID"].ToString();

        wWorkArea.SessioneFile = App.AppDataDataFolder + "\\" + htSelected["FileData"].ToString();
        wWorkArea.SessioneAlias = htSelected["Data"].ToString();// ConvertDataToEsercizio(item["Data"].ToString());
        wWorkArea.SessioneID = htSelected["ID"].ToString();

        wWorkArea.LoadTreeSource();
        wWorkArea.ShowDialog();

        //base.Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
    }

    private void btn_SOSPESI_Click(object sender, RoutedEventArgs e)
    {
      XmlNode node = ((XmlNode)(Nodes[NodeNow]));

      Sospesi o = new Sospesi();
      o.Owner = this;

      if (System.Windows.SystemParameters.PrimaryScreenWidth < 1100 || System.Windows.SystemParameters.PrimaryScreenHeight < 600)
      {
        o.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        o.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        o.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        o.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        o.MinHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        o.MinWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
      }
      else
      {
        o.Width = 1100;
        o.Height = 600;
      }

      o.ReadOnly = ReadOnly;

      o.Stato = Stato;

      o.Load(node.Attributes["ID"].Value,IDCliente,IDSessione);

      o.ShowDialog();

      AggiornaStatoBottoneSospesi();
    }

    //----------------------------------------------------------------------------+
    //                            btn_Esecutore_Click                             |
    //----------------------------------------------------------------------------+

    private void btn_Esecutore_Click(object sender, RoutedEventArgs e)
    {
        int id = 0;
        XmlNode node = ((XmlNode)(Nodes[NodeNow]));
        id = int.Parse(node.Attributes["ID"].Value);
        DataTable dati =cBusinessObjects.GetData(id, typeof(Esecutore_Reviewer));
        var dialog = new wInputBox2("", "", ReadOnly);
        if (dati.Rows.Count>0)
        {
                foreach (DataRow dtrow in dati.Rows)
                {
                   
                    dialog.ResponseText = dtrow["Esecutore"].ToString();
                    dialog.ResponseText2 = dtrow["Reviewer"].ToString();
                }
        }
        else
        {
            dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
        }

        dialog.ShowDialog();
        if (!ReadOnly)
        {

            foreach (DataRow dtrow in dati.Rows)
            {
                    dtrow["Esecutore"]=dialog.ResponseText;
                    dtrow["Reviewer"] =dialog.ResponseText2;
            }
          
            m_isModified = true;
            _x.isModified = true;
            cBusinessObjects.SaveData(id,dati, typeof(Esecutore_Reviewer));

            }
    }

    //private void btn_Reviewer_Click(object sender, RoutedEventArgs e)
    //{
    //    XmlNode node = ((XmlNode)(Nodes[NodeNow]));
    //    node = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");

    //    if (node.Attributes["Reviewer"] == null)
    //    {
    //        XmlAttribute attr = node.OwnerDocument.CreateAttribute("Reviewer");
    //        attr.Value = "";
    //        node.Attributes.Append(attr);
    //    }

    //    var dialog = new wInputBox("Inserire Reviewer", ReadOnly);

    //    dialog.ResponseText = ((node.Attributes["Reviewer"] == null) ? "" : node.Attributes["Reviewer"].Value);
    //    dialog.ShowDialog();

    //    if (!ReadOnly)
    //    {
    //        node.Attributes["Reviewer"].Value = dialog.ResponseText;
    //        _x.Save();
    //    }
    //}

    private void btn_BVCancella_Click()
    {
      string xmlBV = "<BilancioVerifica rowintestazione=\"0\" codice=\"0\" descrizione=\"0\" saldo=\"0\" saldod=\"0\" saldoa=\"0\" />";
      XmlDocument doctmpBV = new XmlDocument();
      doctmpBV.LoadXml(xmlBV);

      XmlNode nodeBV = doctmpBV.SelectSingleNode("/BilancioVerifica");

      MasterFile mf = MasterFile.Create();
      mf.SetAnagraficaBV(Convert.ToInt32(IDCliente), nodeBV);
    }

    private void btn_BV_Click(string esercizio)
    {
      if (SalvaDatiControllo(App.TipoTreeNodeStato.Sconosciuto) == false)
      {
        return;
      }

      string nomefile = "";

      if (esercizio != "")
      {
        Utilities u = new Utilities();
        nomefile = u.sys_OpenFileDialog("", App.TipoFile.BilancioDiVerifica, "Excel |*.xlsx");

        if (nomefile == null)
        {
          return;
        }
      }

      XmlNode node = ((XmlNode)(Nodes[NodeNow]));
     
      DataTable datibilanciotestata = cBusinessObjects.GetData(int.Parse(node.Attributes["ID"].Value), typeof(Excel_Bilancio_Testata));
      DataRow nodedata = null;
      foreach (DataRow dt in datibilanciotestata.Rows)
        {
                nodedata = dt;
        }
      if (nodedata == null)
        {
            return;
        }
      wSchedaBilancioVerifica sbv = new wSchedaBilancioVerifica(node.Attributes["ID"].Value);
      sbv.Nomefile = nomefile;
      sbv.IDCLiente = IDCliente;
      sbv.IDB_Padre = node.Attributes["ID"].Value;
      sbv.nodehere = nodedata;
      sbv.esercizioinesame = esercizio;

      if (nodedata != null && nodedata["tipoBilancio"].ToString() != "")
      {
        sbv.tipoBilancio = nodedata["tipoBilancio"].ToString();
            }
      else
      {
        sbv.tipoBilancio = "";
      }

      if (sbv.Load() == false)
      {
        return;
      }

      sbv.ShowDialog();

     

     
      ucNodoMultiploVerticale unm = new ucNodoMultiploVerticale();

      unm.ReadOnly = ReadOnly;

      unm.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
      unm.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;

      unm.Load(ref _x, node.Attributes["ID"].Value, node.Attributes["Tab"].Value, node.ChildNodes, Sessioni, SessioneNow, IDTree, SessioniTitoli, SessioniID, IDCliente, IDSessione);
      stpAreaLavoro.Children.Clear();
      stpAreaLavoro.Children.Add(unm);
    }

    private void XBRL()
    {
//      if (ReadOnly == false && MessageBox.Show("Tutti i dati attualmente presenti in bilancio verranno sovrascritti. Procedere?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
//      {
  //      return;
  //    }

      if (SalvaDatiControllo(App.TipoTreeNodeStato.Sconosciuto) == false)
      {
        return;
      }

      Utilities u = new Utilities();
      string nomefile = u.sys_OpenFileDialog("", App.TipoFile.XBRL);

      if (nomefile == null)
      {
        return;
      }

      var content = string.Empty;
      using (StreamReader reader = new StreamReader(nomefile))
      {
        content = reader.ReadToEnd();
        reader.Close();
      }

      content = Regex.Replace(content, "&euro;", "&#8364;");

      content = Regex.Replace(content, "&", " ");

      //content = Regex.Replace(content, "&nbsp;", " ");

      //using (StreamWriter writer = new StreamWriter(nomefile))
      //{
      //    writer.Write(content);
      //    writer.Close();
      //}


      XmlDocument xbrldoc = new XmlDocument();
      try
      {
        xbrldoc.LoadXml(content);
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        return;
      }

      XmlNode node = ((XmlNode)(Nodes[NodeNow]));
    
        DataRow tmpnodehere = null;
        DataTable datibilanciotestata3 = cBusinessObjects.GetData(int.Parse(node.Attributes["ID"].Value), typeof(Excel_Bilancio_Testata));
      
        foreach (DataRow dt in datibilanciotestata3.Rows)
        {
          tmpnodehere = dt; 
        }

          
      string filexbrl = "";
      if (tmpnodehere != null & tmpnodehere["tipoBilancio"].ToString() != "")
      {
        switch (tmpnodehere["tipoBilancio"].ToString())
        {
          case "2016":
          case "Micro":
            filexbrl = App.AppXBRL2016;
            break;
          default:
            filexbrl = App.AppXBRL;
            break;
        }
      }
      else
      {
        return;
      }


      cXBRL xbrl = new cXBRL(filexbrl);

      Hashtable htValueEAxbrl = new Hashtable();
      Hashtable htValueEPxbrl = new Hashtable();

      XmlNode RootXRBL = xbrldoc.SelectSingleNode("/").LastChild;

      //Controllo quali cntextRef sono presenti nel file

      XmlNodeList nl = RootXRBL.SelectNodes("//*[@contextRef]");
      ArrayList contextRef = new ArrayList();
      string contextRefEA = "";
      string contextRefEP = "";

      bool newXBRL = false;

      foreach (XmlNode item in nl)
      {
        string contextref = item.Attributes["contextRef"].Value.ToString();

        if (contextref.Substring(contextref.Length - 2) == "_i" || contextref.Substring(contextref.Length - 2) == "_d")
        {
          newXBRL = true;

          if (!contextRef.Contains(contextref.Substring(0, contextref.Length - 2)))
          {
            contextRef.Add(contextref.Substring(0, contextref.Length - 2));
          }
        }
        else
        {
          if (!contextRef.Contains(item.Attributes["contextRef"].Value.ToString().Substring(1)))
          {
            contextRef.Add(item.Attributes["contextRef"].Value.ToString().Substring(1));
          }
        }
      }

      if (contextRef.Count == 0)
      {
        MessageBox.Show("L'XBRL in esame non ha un contesto di riferimento");
        return;
      }

      if (contextRef.Count == 1)
      {
        contextRefEA = contextRef[0].ToString();
        contextRefEP = "";
      }
      else
      {
        wXBRLContestiRiferimento wxbrl = new wXBRLContestiRiferimento();
        wxbrl.ContestiRiferimento = contextRef;
        wxbrl.ConfiguraMaschera();
        wxbrl.ShowDialog();

        if (wxbrl.cmbEA.SelectedIndex == -1)
        {
          MessageBox.Show("E' necessario selezionare almeno il contesto dell'esercizio attuale.");
          return;
        }

        contextRefEA = wxbrl.cmbEA.SelectedValue.ToString();

        if (!contextRef.Contains(contextRefEA))
        {
          MessageBox.Show("E' necessario selezionare almeno il contesto dell'esercizio attuale.");
          return;
        }

        if (wxbrl.cmbEP.SelectedIndex != -1)
        {
          contextRefEP = wxbrl.cmbEP.SelectedValue.ToString();
        }

        if (!contextRef.Contains(contextRefEP))
        {
          contextRefEP = "";
        }

        if (contextRefEA == contextRefEP)
        {
          MessageBox.Show("Attenzione, il contesto Attuale e Precedente devono essere diversi.");
          return;
        }
      }

      foreach (DictionaryEntry item in xbrl.htXBRL)
      {
        int valoreEA = 0;
        int valoreEP = 0;
        bool isEP = false;

        if (newXBRL)
        {
          if (RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + contextRefEA.ToLower() + "_i']") != null || RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + contextRefEA.ToLower() + "_d']") != null)
          {
            if (RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + contextRefEA.ToLower() + "_i']") == null || !Int32.TryParse(RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + contextRefEA.ToLower() + "_i']").InnerText, out valoreEA))
            {
              Int32.TryParse(RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + contextRefEA.ToLower() + "_d']").InnerText, out valoreEA);
            }
          }

          if (contextRefEP != "" && (RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + contextRefEP.ToLower() + "_i']") != null || RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + contextRefEP.ToLower() + "_d']") != null))
          {
            if (RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + contextRefEP.ToLower() + "_i']") != null)
            {
              isEP = Int32.TryParse(RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + contextRefEP.ToLower() + "_i']").InnerText, out valoreEP);
            }

            if (RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + contextRefEP.ToLower() + "_d']") != null)
            {
              isEP = Int32.TryParse(RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='" + contextRefEP.ToLower() + "_d']").InnerText, out valoreEP);
            }
          }
        }
        else
        {
          if (RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='i" + contextRefEA.ToLower() + "']") != null || RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='d" + contextRefEA.ToLower() + "']") != null)
          {
            if (RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='i" + contextRefEA.ToLower() + "']") == null || !Int32.TryParse(RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='i" + contextRefEA.ToLower() + "']").InnerText, out valoreEA))
            {
              Int32.TryParse(RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='d" + contextRefEA.ToLower() + "']").InnerText, out valoreEA);
            }
          }

          if (contextRefEP != "" && (RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='i" + contextRefEP.ToLower() + "']") != null || RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='d" + contextRefEP.ToLower() + "']") != null))
          {
            if (RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='i" + contextRefEP.ToLower() + "']") != null)
            {
              isEP = Int32.TryParse(RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='i" + contextRefEP.ToLower() + "']").InnerText, out valoreEP);
            }

            if (RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='d" + contextRefEP.ToLower() + "']") != null)
            {
              isEP = Int32.TryParse(RootXRBL.SelectSingleNode("//*[local-name()='" + item.Key.ToString() + "'][translate(@contextRef, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') ='d" + contextRefEP.ToLower() + "']").InnerText, out valoreEP);
            }
          }
        }

        if (xbrl.htSegno.Contains(item.Key.ToString()))
        {
          valoreEA = valoreEA * -1;
          valoreEP = valoreEP * -1;
        }

        if (htValueEAxbrl.Contains(item.Value.ToString()))
        {
          htValueEAxbrl[item.Value.ToString()] = (Convert.ToInt32((htValueEAxbrl[item.Value.ToString()]).ToString()) + valoreEA).ToString();
        }
        else
        {
          htValueEAxbrl.Add(item.Value.ToString(), valoreEA.ToString());
        }

        if (isEP)
        {
          if (htValueEPxbrl.Contains(item.Value.ToString()))
          {
            htValueEPxbrl[item.Value.ToString()] = (Convert.ToInt32((htValueEPxbrl[item.Value.ToString()]).ToString()) + valoreEP).ToString();
          }
          else
          {
            htValueEPxbrl.Add(item.Value.ToString(), valoreEP.ToString());
          }
        }
      }

      DataTable datib = cBusinessObjects.GetData(int.Parse(node.Attributes["ID"].Value),typeof(Excel_Bilancio));
    

      foreach (DataRow item in datib.Rows)
      {
        if (item["EA"].ToString() != "")
        {
          item["EA"] =0;
        }

        if (item["EP"].ToString() != "")
        {
          item["EP"] = 0;
        }
      }

      foreach (DictionaryEntry item in htValueEAxbrl)
      {
       
        foreach (DataRow dd in datib.Rows)
        {
                    if (dd["ID"].ToString() == item.Key.ToString())
                    {
                        if (item.Key.ToString() != "")
                            dd["EA"] = item.Value.ToString();
                    }
        }

      }

      foreach (DictionaryEntry item in htValueEPxbrl)
      {
                foreach (DataRow dd in datib.Rows)
                {
                    if (dd["ID"].ToString() == item.Key.ToString())
                    {
                        if (item.Key.ToString() != "")
                            dd["EP"] = item.Value.ToString();
                    }
                }

               
       }


            cBusinessObjects.Executesql("DELETE FROM Excel_Bilancio WHERE ID_SCHEDA=" + cBusinessObjects.GetIDTree(int.Parse(node.Attributes["ID"].Value)).ToString() + " AND ID_CLIENTE=" + cBusinessObjects.idcliente.ToString() + " AND ID_SESSIONE=" + cBusinessObjects.idsessione.ToString());

            cBusinessObjects.SaveData(int.Parse(node.Attributes["ID"].Value),datib, typeof(Excel_Bilancio));

            ucNodoMultiploVerticale unm = new ucNodoMultiploVerticale();


      unm.ReadOnly = ReadOnly;

      unm.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
      unm.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;

      unm.Load(ref _x, node.Attributes["ID"].Value, node.Attributes["Tab"].Value, node.ChildNodes, Sessioni, SessioneNow, IDTree, SessioniTitoli, SessioniID, IDCliente, IDSessione);
      stpAreaLavoro.Children.Clear();
      stpAreaLavoro.Children.Add(unm);
    }

    private void btn_Campionamento_Click(object sender, RoutedEventArgs e)
    {
      wSchedaCampionamento stp = new wSchedaCampionamento();

      stp.Owner = this;
      stp.RevisioneAssociata = null;

      MasterFile tmpmf = MasterFile.Create();

      Hashtable revisioneNow = tmpmf.GetAllRevisioneAssociataFromBilancioFile(Sessioni[SessioneNow].ToString());
      if (revisioneNow != null && revisioneNow["FileData"] != null)
      {
        string revisioneAssociata = App.AppDataDataFolder + "\\" + revisioneNow["FileData"].ToString();

        if (revisioneAssociata != "")
        {
          stp.RevisioneAssociata = new XmlDataProviderManager(revisioneAssociata);
        }
      }

      if (IDTree!="pippo")
      {

      }

      string str;

      str = string.Format("IDTree={0}, IDCliente={1}, IDSessione={2}", IDTree, IDCliente, IDSessione);
      MessageBox.Show(str, "eccoli", MessageBoxButton.OK, MessageBoxImage.Information);

      switch (_IDNodo)
      {
        case "131":
          stp._tipologia = wSchedaCampionamento.TipologieCampionamento.Magazzino;
          break;
        case "134":
          stp._tipologia = wSchedaCampionamento.TipologieCampionamento.Clienti;
          break;
        case "179":
          stp._tipologia = wSchedaCampionamento.TipologieCampionamento.Fornitori;
          break;
        default:
          stp._tipologia = wSchedaCampionamento.TipologieCampionamento.Sconosciuto;
          break;
      }
      stp.nodeNumber = Convert.ToInt32(_IDNodo);

      if (stp.Load() == false)
      {
        return;
      }

      stp.ShowDialog();

      
    }


    private void btn_RotazioneScorte_Click(object sender, RoutedEventArgs e)
    {
      wSchedaRotazioneScorte stp = new wSchedaRotazioneScorte();

      stp.Owner = this;
      stp.RevisioneAssociata = null;

      if (stp.Load() == false)
      {
        return;
      }

      stp.ShowDialog();

    }

    private void btn_XBLR_Click(object sender, RoutedEventArgs e)
    {
      if (ApertoInSolaLettura)
      {
        MessageBox.Show(App.MessaggioSolaScritturaStato);
        return;
      }
      else if (ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }
      //Consolidato
      if (_IDNodo == "321")
      {
        XmlNode node = ((XmlNode)(Nodes[NodeNow]));
        XmlNode nodedata = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");

        if (nodedata != null)
        {
          if (nodedata.Attributes["tipoBilancio"] == null)
          {
            XmlAttribute attr = nodedata.OwnerDocument.CreateAttribute("tipoBilancio");
            nodedata.Attributes.Append(attr);
          }

          nodedata.Attributes["tipoBilancio"].Value = "consolidato";
        }

        wSceltaTipologiaCaricamentoDatiConsolidato stp = new wSceltaTipologiaCaricamentoDatiConsolidato();
        stp.ShowDialog();
        if (stp.typechosen == "")
        {
          return;
        }

        switch (stp.typechosen)
        {
          case "BVEA":
            btn_BV_Click("EA");
            break;
          case "BVEP":
            btn_BV_Click("EP");
            break;
          case "BV":
            btn_BV_Click("");
            break;
          case "Cancella":
            if (MessageBox.Show("Attenzione. tutte le associazioni verranno cancellate in modo irreversibile!", "Attenzione", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
              btn_BVCancella_Click();
            }
            break;
          default:
            break;
        }
      }
      else
      {
        wSceltaTipologiaCaricamentoDati stp = new wSceltaTipologiaCaricamentoDati(IDTree);
        stp.ShowDialog();
        if (stp.typechosen == "")
        {
          return;
        }

        switch (stp.typechosen)
        {
          case "XBRL":
            XBRL();
            break;
          case "BVEA":
            btn_BV_Click("EA");
            break;
          case "BVEP":
            btn_BV_Click("EP");
            break;
          case "BV":
            btn_BV_Click("");
            break;
          case "Cancella":
            if (MessageBox.Show("Attenzione. tutte le associazioni verranno cancellate in modo irreversibile!", "Attenzione", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
              btn_BVCancella_Click();
            }
            break;
          default:
            break;
        }
      }

    }

    #endregion

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      //XmlNode node = ((XmlNode)(Nodes[NodeNow]));

      //switch (node.Attributes["Tipologia"].Value)
      //{
      //    case "Nodo Multiplo":
      //        stpAreaLavoro.Width = Convert.ToInt32(e.NewSize.Width);
      //        ((ucNodoMultiploVerticale)(stpAreaLavoro.Children[0])).UserControl_SizeChanged(sender, e);
      //        break;
      //    default:
      //        break;
      //}
    }

    //bool done = false;

    private void stpAreaLavoro_GotFocus(object sender, RoutedEventArgs e)
    {
      //if (ReadOnly && !done)
      //{
      //    done = true;
      //    MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
      //}
    }

    private void Window_Activated(object sender, EventArgs e)
    {
      if (istobeshowedGuida && firsttime)
      {
        firsttime = false;

        istobeshowedGuida = false;

        XmlNode node;
        string nota = "";


        node = ((XmlNode)(Nodes[NodeNow]));

        if (node != null && node.Attributes["Nota"] != null)
        {
          nota = node.Attributes["Nota"].Value;
        }

        if (nota != "" && nota != "<P>&nbsp;</P>" && nota != "<P align=left>&nbsp;</P>" && nota != "<P class=MsoNormal style=\"MARGIN: 0cm 0cm 0pt; LINE-HEIGHT: normal\" align=left>&nbsp;</P>" && !App.AppSetupIstruzioniAutomatiche)
        {
          GuidaRevisoft(false);
        }
      }
    }

    private void menuStrumentiStampaVerbali_Click(object sender, RoutedEventArgs e)
    {
      wStampaVerbali wSF = new wStampaVerbali();
      wSF.inizializza();
      wSF.Owner = this;
      wSF.ShowDialog();
    }

    private void btn_ESPANDI_BV_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btn_Note_Click(object sender, RoutedEventArgs e)
    {
      wNoteRevisore w = new wNoteRevisore(IDCliente, _cartellaxTeam);
      w.ShowDialog();
    }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
