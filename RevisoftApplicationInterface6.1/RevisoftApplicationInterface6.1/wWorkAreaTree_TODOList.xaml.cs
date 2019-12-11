//----------------------------------------------------------------------------+
//                       wWorkAreaTree_TODOList.xaml.cs                       |
//----------------------------------------------------------------------------+
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
using System.Data;

namespace RevisoftApplication
{

  public partial class WindowWorkAreaTree_TODOList : System.Windows.Window
  {
    public string SelectedTreeSource = "";
    public string SelectedDataSource = "";
    public string SelectedSessioneSource = "";

    public string Data = DateTime.Now.ToShortDateString();

    private string _cliente = "";
    private App.TipoAttivita _TipoAttivita = App.TipoAttivita.Sconosciuto;
    private bool firsttime = true;

    public string TitoloSessione = "";
    public string ImportFileName = "";

    public string IDTree = "-1";
    public string IDCliente = "-1";
    public string IDSessione = "-1";

    //XmlDataProviderManager _x; // non viene mai usato
    public XmlDataProviderManager _xTXP;
    XmlDataProvider TreeXmlProvider;
    XmlDocument xmlTMP = new XmlDocument();

    ArrayList ALXTPP = new ArrayList();

    Hashtable htComboID = new Hashtable();
    bool _isModified = false;

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

    private void GeneraTitolo()
    {
      txtTitoloRagioneSociale.Text = "Cliente: " + _cliente;
    }

    public WindowWorkAreaTree_TODOList()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      labelAttivita.Content = "";

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

    //----------------------------------------------------------------------------+
    //                               LoadTreeSource                               |
    //----------------------------------------------------------------------------+
   
    public void LoadTreeSource()
    {

      if (Data == "") Data = DateTime.Now.ToShortDateString();
      ArrayList pianificatehere = new ArrayList();
      txtData.Text = Data;
      RevisoftApplication.XmlManager x = new XmlManager();
      x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
      MasterFile mf = MasterFile.Create();
      ArrayList files = new ArrayList();
      switch (TipoAttivita)
      {
        case App.TipoAttivita.Incarico:
          SelectedTreeSource = App.AppTemplateTreeIncarico;
          files = mf.GetIncarichi(IDCliente);
          break;
        case App.TipoAttivita.ISQC:
          SelectedTreeSource = App.AppTemplateTreeISQC;
          files = mf.GetISQCs(IDCliente);
          break;
        case App.TipoAttivita.Revisione:
          SelectedTreeSource = App.AppTemplateTreeRevisione;
          files = mf.GetRevisioni(IDCliente);
          break;
        case App.TipoAttivita.Bilancio:
          SelectedTreeSource = App.AppTemplateTreeBilancio;
          files = mf.GetBilanci(IDCliente);
          break;
        case App.TipoAttivita.Conclusione:
          SelectedTreeSource = App.AppTemplateTreeConclusione;
          files = mf.GetConclusioni(IDCliente);
          break;
        case App.TipoAttivita.Verifica:
          files = mf.GetVerifiche(IDCliente);
          SelectedTreeSource = App.AppTemplateTreeVerifica;
          ArrayList al = mf.GetPianificazioniVerifiche(IDCliente);
          foreach (Hashtable itemHT in al)
          {
            ALXTPP.Add(itemHT["ID"].ToString());
        }
          break;
        case App.TipoAttivita.Vigilanza:
          files = mf.GetVigilanze(IDCliente);
          SelectedTreeSource = App.AppTemplateTreeVigilanza;
          ArrayList al2 = mf.GetPianificazioniVigilanze(IDCliente);
          foreach (Hashtable itemHT in al2)
          {
           
            ALXTPP.Add(itemHT["ID"].ToString());
          }
          break;
        default:
          return;
      }
      if (files.Count > 0)
      {
        string maxID = "0", id;
        DateTime lastdate = Convert.ToDateTime("01/01/1900");
        string tobeused = "";
        foreach (Hashtable itemHT in files)
        {
          if (itemHT.Contains("ID"))
          {
            id = itemHT["ID"].ToString();
            if (Convert.ToInt32(id) > Convert.ToInt32(maxID))
            {
              maxID = id; tobeused = itemHT["File"].ToString();
            }
          }
        }
        _xTXP = new XmlDataProviderManager(tobeused);
        TreeXmlProvider.Document = x.LoadEncodedFile(tobeused);
      }
      else
      {
        _xTXP = new XmlDataProviderManager(SelectedTreeSource);
        TreeXmlProvider.Document = x.LoadEncodedFile(SelectedTreeSource);
      }
      if (firsttime)
      {
        firsttime = false;
        foreach (XmlNode item in _xTXP.Document.SelectNodes("//Node"))
        {
          if (item.Attributes["WidthNota"] == null)
          {
            XmlAttribute attr = _xTXP.Document.CreateAttribute("WidthNota");
            item.Attributes.Append(attr);
          }
          if (item.SelectNodes("Node").Count > 0)
          {
            item.Attributes["WidthNota"].Value = "0";
          }
          else
          {
            item.Attributes["WidthNota"].Value = "Auto";
          }
          if (item.Attributes["Checked"] == null)
          {
            XmlAttribute attr = _xTXP.Document.CreateAttribute("Checked");
            item.Attributes.Append(attr);
            item.Attributes["Checked"].Value = "False";
          }
          if (item.Attributes["NotaTDL"] == null)
          {
            XmlAttribute attr = _xTXP.Document.CreateAttribute("NotaTDL");
            item.Attributes.Append(attr);
            item.Attributes["NotaTDL"].Value = "";
          }
          item.Attributes["Expanded"].Value = "True";
          item.Attributes["Selected"].Value = "False";
          if (item.Attributes["Pianificato"] == null)
          {
            XmlAttribute attr = item.OwnerDocument.CreateAttribute("Pianificato");
            attr.Value = "";
            item.Attributes.Append(attr);
          }
          DataTable pianificazione = null;
          DataTable pianificazioneTestata = null;
          foreach (string ALitemXTPP in ALXTPP)
            {
                bool donehere = false;
                string IDPHERE = "";
                string datac = "";

                if (TipoAttivita == App.TipoAttivita.Verifica)
                {
                        IDPHERE = "100013";
                        pianificazioneTestata = cBusinessObjects.GetData(int.Parse(IDPHERE), typeof(PianificazioneVerificheTestata), int.Parse(IDCliente), int.Parse(ALitemXTPP), 26);
                        pianificazione = cBusinessObjects.GetData(int.Parse(IDPHERE), typeof(PianificazioneVerifiche),int.Parse(IDCliente), int.Parse(ALitemXTPP), 26);
                 }
                else
                {
                        IDPHERE = "100003";
                        pianificazioneTestata = cBusinessObjects.GetData(int.Parse(IDPHERE), typeof(PianificazioneVerificheTestata), int.Parse(IDCliente), int.Parse(ALitemXTPP), 27);

                        pianificazione = cBusinessObjects.GetData(int.Parse(IDPHERE), typeof(PianificazioneVerifiche), int.Parse(IDCliente), int.Parse(ALitemXTPP), 27);
                }
                foreach (DataRow itemXPP in pianificazione.Rows)
                {
                    if (itemXPP["NODE_ID"].ToString() != item.Attributes["ID"].Value)
                                continue;
                    foreach (DataRow dd in pianificazioneTestata.Rows)
                    {
                        if (dd["ID"].ToString() == itemXPP["PianificazioneID"].ToString())
                            datac = dd["Data"].ToString();
                    }
                    if (datac != Data)
                                continue;
                    if (itemXPP["Checked"].ToString() == "True")
                    {
                        item.Attributes["Pianificato"].Value = "P";
                        item.Attributes["Checked"].Value = "True";
                        StaticUtilities.MarkNodeAsModified(item, App.OBJ_MOD); _isModified = true;
                        pianificatehere.Add(item.Attributes["ID"].Value);
                        break;
                    }
                           
                }
                if (donehere)
                {
                    break;
                }
            }
      
        }
        _xTXP.Save();
      
        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("//Node"))
        {
          if (item.Attributes["NotaTDL"] == null)
          {
            XmlAttribute attr = TreeXmlProvider.Document.CreateAttribute("NotaTDL");
            item.Attributes.Append(attr);
            item.Attributes["NotaTDL"].Value = "";
          }
          if (item.Attributes["Checked"] == null)
          {
            XmlAttribute attr = TreeXmlProvider.Document.CreateAttribute("Checked");
            item.Attributes.Append(attr);
            item.Attributes["Checked"].Value = "False";
          }
          if (item.Attributes["Pianificato"] == null)
          {
            XmlAttribute attr = TreeXmlProvider.Document.CreateAttribute("Pianificato");
            item.Attributes.Append(attr);
            item.Attributes["Pianificato"].Value = "";
          }
          if (pianificatehere.Contains(item.Attributes["ID"].Value))
          {
            item.Attributes["Pianificato"].Value = "P";
          }
          if (item.Attributes["WidthNota"] == null)
          {
            XmlAttribute attr = TreeXmlProvider.Document.CreateAttribute("WidthNota");
            item.Attributes.Append(attr);
          }
          if (item.SelectNodes("Node").Count > 0)
          {
            item.Attributes["WidthNota"].Value = "0";
          }
          else
          {
            item.Attributes["WidthNota"].Value = "Auto";
          }
          if (item.Attributes["Pianificato"].Value == "P")
          {
            item.Attributes["Checked"].Value = "True";
            //item.Attributes["NotaTDL"].Value = "";
          }
          StaticUtilities.MarkNodeAsModified(item, App.OBJ_MOD);_isModified = true;
        }
      }
      Utilities u = new Utilities();
      labelAttivita.Content = u.TitoloAttivita(_TipoAttivita);
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

    //----------------------------------------------------------------------------+
    //                               Window_Closing                               |
    //----------------------------------------------------------------------------+
    private void Window_Closing_old(object sender, System.ComponentModel.CancelEventArgs e)
    {
      _xTXP.Save();
    }
    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
#if (!DBG_TEST)
      Window_Closing_old(sender,e);return;
#endif
      _xTXP.isModified = true;
      _xTXP.Save(true);
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

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

    public string Convert2RTFChar(string carattere)
    {
      string newChar = "";

      switch (carattere)
      {
        //case "!":
        //    newChar = "\\'21";
        //    break;
        case "\"":
          newChar = "\\'22";
          break;
        //case "#":
        //    newChar = "\\'23";
        //    break;
        case "$":
          newChar = "\\'24";
          break;
        case "%":
          newChar = "\\'25";
          break;
        case "&":
          newChar = "\\'26";
          break;
        case "'":
          newChar = "\\'27";
          break;
        //case "(":
        //    newChar = "\\'28";
        //    break;
        //case ")":
        //    newChar = "\\'29";
        //    break;
        //case "*":
        //    newChar = "\\'2a";
        //    break;
        //case "+":
        //    newChar = "\\'2b";
        //    break;
        //case ",":
        //    newChar = "\\'2c";
        //    break;
        //case "-":
        //    newChar = "\\'2d";
        //    break;
        //case ".":
        //    newChar = "\\'2e";
        //    break;
        //case "/":
        //    newChar = "\\'2f";
        //    break;
        //case ":":
        //    newChar = "\\'3a";
        //    break;
        //case ";":
        //    newChar = "\\'3b";
        //    break;
        //case "<":
        //    newChar = "\\'3c";
        //    break;
        //case "=":
        //    newChar = "\\'3d";
        //    break;
        //case ">":
        //    newChar = "\\'3e";
        //    break;
        //case "?":
        //    newChar = "\\'3f";
        //    break;
        //case "@":
        //    newChar = "\\'40";
        //    break;
        //case "[":
        //    newChar = "\\'5b";
        //    break;
        //case "\\":
        //    newChar = "\\'5c";
        //    break;
        //case "]":
        //    newChar = "\\'5d";
        //    break;
        //case "^":
        //    newChar = "\\'5e";
        //    break;
        //case "_":
        //    newChar = "\\'5f";
        //    break;
        //case "`":
        //    newChar = "\\'60";
        //    break;
        //case "{":
        //    newChar = "\\'7b";
        //    break;
        //case "|":
        //    newChar = "\\'7c";
        //    break;
        //case "}":
        //    newChar = "\\'7d";
        //    break;
        //case "~":
        //    newChar = "\\'7e";
        //    break;
        case "€":
          newChar = "\\'80";
          break;
        //case "͵":
        //    newChar = "\\'82";
        //    break;
        //case "ƒ":
        //    newChar = "\\'83";
        //    break;
        //case ""
        //    newChar = "\\'84";
        //    break;
        case "…":
          newChar = "\\'85";
          break;
        //case "†":
        //    newChar = "\\'86";
        //case "‡":
        //    newChar = "\\'87";
        //    break;
        case "∘":
          newChar = "\\'88";
          break;
        //case "‰":
        //    newChar = "\\'89";
        //    break;
        //case "Š":
        //    newChar = "\\'8a";
        //    break;
        //case "‹":
        //    newChar = "\\'8b";
        //    break;
        //case "Œ":
        //    newChar = "\\'8c";
        //    break;
        //case "Ž":
        //    newChar = "\\'8e";
        //    break;
        //case "‘":
        //    newChar = "\\'91";
        //    break;
        case "’":
          newChar = "\\'92";
          break;
        case "“":
          newChar = "\\'93";
          break;
        case "”":
          newChar = "\\'94";
          break;
        //case "•":
        //    newChar = "\\'95";
        //    break;
        //case "–":
        //    newChar = "\\'96";
        //    break;
        //case "—":
        //    newChar = "\\'97";
        //    break;
        //case "~":
        //    newChar = "\\'98";
        //    break;
        //case "™":
        //    newChar = "\\'99";
        //    break;
        //case "š":
        //    newChar = "\\'9a";
        //    break;
        //case "›":
        //    newChar = "\\'9b";
        //    break;
        //case "œ":
        //    newChar = "\\'9c";
        //    break;
        //case "ž":
        //    newChar = "\\'9e";
        //    break;
        //case "Ÿ":
        //    newChar = "\\'9f";
        //    break;
        //case "¡":
        //    newChar = "\\'a1";
        //    break;
        //case "¢":
        //    newChar = "\\'a2";
        //    break;
        //case "£":
        //    newChar = "\\'a3";
        //    break;
        //case "¤":
        //    newChar = "\\'a4";
        //    break;
        //case "¥":
        //    newChar = "\\'a5";
        //    break;
        //case "¦":
        //    newChar = "\\'a6";
        //    break;
        //case "§":
        //    newChar = "\\'a7";
        //    break;
        //case "¨":
        //    newChar = "\\'a8";
        //    break;
        case "©":
          newChar = "\\'a9";
          break;
        //case "ª":
        //    newChar = "\\'aa";
        //    break;
        //case "«":
        //    newChar = "\\'ab";
        //    break;
        //case "¬":
        //    newChar = "\\'ac";
        //    break;
        //case "®":
        //    newChar = "\\'ae";
        //    break;
        //case "¯":
        //    newChar = "\\'af";
        //    break;
        case "°":
          newChar = "\\'b0";
          break;
        case "±":
          newChar = "\\'b1";
          break;
        case "²":
          newChar = "\\'b2";
          break;
        case "³":
          newChar = "\\'b3";
          break;
        //case "´":
        //    newChar = "\\'b4";
        //    break;
        case "µ":
          newChar = "\\'b5";
          break;
        //case "¶":
        //    newChar = "\\'b6";
        //    break;
        //case "•":
        //  newChar = "\\'b7";
        //break;
        //case "¸":
        //    newChar = "\\'b8";
        //    break;
        //case "¹":
        //    newChar = "\\'b9";
        //    break;
        //case "º":
        //    newChar = "\\'ba";
        //    break;
        //case "»":
        //    newChar = "\\'bb";
        //    break;
        //case "¼":
        //    newChar = "\\'bc";
        //    break;
        //case "½":
        //    newChar = "\\'bd";
        //    break;
        //case "¾":
        //    newChar = "\\'be";
        //    break;
        //case "¿":
        //    newChar = "\\'bf";
        //    break;
        case "À":
          newChar = "\\'c0";
          break;
        case "Á":
          newChar = "\\'c1";
          break;
        case "Â":
          newChar = "\\'c2";
          break;
        case "Ã":
          newChar = "\\'c3";
          break;
        case "Ä":
          newChar = "\\'c4";
          break;
        case "Å":
          newChar = "\\'c5";
          break;
        case "Æ":
          newChar = "\\'c6";
          break;
        case "Ç":
          newChar = "\\'c7";
          break;
        case "È":
          newChar = "\\'c8";
          break;
        case "É":
          newChar = "\\'c9";
          break;
        case "Ê":
          newChar = "\\'ca";
          break;
        case "Ë":
          newChar = "\\'cb";
          break;
        case "Ì":
          newChar = "\\'cc";
          break;
        case "Í":
          newChar = "\\'cd";
          break;
        case "Î":
          newChar = "\\'ce";
          break;
        case "Ï":
          newChar = "\\'cf";
          break;
        case "Ð":
          newChar = "\\'d0";
          break;
        case "Ñ":
          newChar = "\\'d1";
          break;
        case "Ò":
          newChar = "\\'d2";
          break;
        case "Ó":
          newChar = "\\'d3";
          break;
        case "Ô":
          newChar = "\\'d4";
          break;
        case "Õ":
          newChar = "\\'d5";
          break;
        case "Ö":
          newChar = "\\'d6";
          break;
        //case "×":
        //    newChar = "\\'d7";
        //    break;
        case "Ø":
          newChar = "\\'d8";
          break;
        case "Ù":
          newChar = "\\'d9";
          break;
        case "Ú":
          newChar = "\\'da";
          break;
        case "Û":
          newChar = "\\'db";
          break;
        case "Ü":
          newChar = "\\'dc";
          break;
        case "Ý":
          newChar = "\\'dd";
          break;
        case "Þ":
          newChar = "\\'de";
          break;
        case "ß":
          newChar = "\\'df";
          break;
        case "à":
          newChar = "\\'e0";
          break;
        case "á":
          newChar = "\\'e1";
          break;
        case "â":
          newChar = "\\'e2";
          break;
        case "ã":
          newChar = "\\'e3";
          break;
        case "ä":
          newChar = "\\'e4";
          break;
        case "å":
          newChar = "\\'e5";
          break;
        case "æ":
          newChar = "\\'e6";
          break;
        case "ç":
          newChar = "\\'e7";
          break;
        case "è":
          newChar = "\\'e8";
          break;
        case "é":
          newChar = "\\'e9";
          break;
        case "ê":
          newChar = "\\'ea";
          break;
        case "ë":
          newChar = "\\'eb";
          break;
        case "ì":
          newChar = "\\'ec";
          break;
        case "í":
          newChar = "\\'ed";
          break;
        case "î":
          newChar = "\\'ee";
          break;
        case "ï":
          newChar = "\\'ef";
          break;
        case "ð":
          newChar = "\\'f0";
          break;
        case "ñ":
          newChar = "\\'f1";
          break;
        case "ò":
          newChar = "\\'f2";
          break;
        case "ó":
          newChar = "\\'f3";
          break;
        case "ô":
          newChar = "\\'f4";
          break;
        case "õ":
          newChar = "\\'f5";
          break;
        case "ö":
          newChar = "\\'f6";
          break;
        case "÷":
          newChar = "\\'f7";
          break;
        case "ø":
          newChar = "\\'f8";
          break;
        case "ù":
          newChar = "\\'f9";
          break;
        case "ú":
          newChar = "\\'fa";
          break;
        case "û":
          newChar = "\\'fb";
          break;
        case "ü":
          newChar = "\\'fc";
          break;
        case "ý":
          newChar = "\\'fd";
          break;
        case "þ":
          newChar = "\\'fe";
          break;
        case "ÿ":
          newChar = "\\'ff";
          break;
      }

      return newChar;
    }

    public string Convert2RTFString(string buff, string replaceChar)
    {
      return buff.Replace(replaceChar, Convert2RTFChar(replaceChar));
    }

    private string Convert2RTF(string buff)
    {
      buff = buff.Replace("\\'", "\\#");
      buff = Convert2RTFString(buff, "'"); //va messo per primo o causa problemi
      buff = buff.Replace("\\#", "\\'");

      //for (char c = '!'; c <= 'ÿ'; c++)
      //{
      //    buff = Convert2RTFString(buff, c.ToString() );
      //}

      buff = Convert2RTFString(buff, "%");
      buff = Convert2RTFString(buff, "ì");
      buff = Convert2RTFString(buff, "è");
      buff = Convert2RTFString(buff, "é");
      buff = Convert2RTFString(buff, "ò");
      buff = Convert2RTFString(buff, "à");
      buff = Convert2RTFString(buff, "ù");
      buff = Convert2RTFString(buff, "°");
      buff = Convert2RTFString(buff, "€");
      buff = Convert2RTFString(buff, "\"");
      buff = Convert2RTFString(buff, "’");
      buff = Convert2RTFString(buff, "”");
      buff = Convert2RTFString(buff, "“");

      return buff;
    }

    private void buttonCreaPDF_Click(object sender, RoutedEventArgs e)
    {

     

      string rtf_text = "";
      rtf_text += "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1040\\deflangfe1040\\deftab709";
      rtf_text += "{\\fonttbl{\\f0 Cambria}}";
      rtf_text += "{\\colortbl;\\red0\\green255\\blue255;\\red204\\green204\\blue204;\\red255\\green255\\blue255;\\red230\\green230\\blue230;}";
      rtf_text += "\\viewkind4\\uc1";

      rtf_text += "\\fs28 \\qc " + Cliente + " \\ql \\fs28 \\line \\line ";

      rtf_text += "\\fs24 Siamo ad informarvi che nella sessione prevista per il giorno " + Data + " l'organo di controllo eseguirà i seguenti controlli: \\fs24 \\line \\line ";

      foreach (XmlNode item in _xTXP.Document.SelectSingleNode("/Tree").SelectNodes("//Node"))
      {
        try
        {
          if (item.Attributes["NotaTDL"] != null && item.Attributes["Checked"] != null && item.Attributes["Checked"].Value == "True")
          {
            rtf_text += "\\b " + item.Attributes["Codice"].Value + " " + item.Attributes["Titolo"].Value + ": \\b0  ";

            {
              string istruzione = item.Attributes["NotaTDL"].Value;

              if (istruzione.Trim() != "")
              {
                rtf_text += "\\pard\\keepn \\i " + istruzione + " \\i0\\par";
              }
            }

            rtf_text += " \\line \\line ";
          }
        }
        catch (Exception ex)
        {
          string log = ex.Message;
        }

      }

      rtf_text += "}";

      rtf_text = Convert2RTF(rtf_text);

      string filename = App.AppTempFolder + Guid.NewGuid().ToString();

      TextWriter tw = new StreamWriter(filename + ".rtf");
      tw.Write(rtf_text);
      tw.Close();

      //MM
      cDocNet wrdDoc = new cDocNet();
      wrdDoc.PageSetupPaperSize = "A4";
      wrdDoc.PageSetupOrientation = WdOrientation.wdOrientLandscape;
      wrdDoc.SaveAs(filename + ".pdf", filename + ".rtf", "WdSaveFormat.wdFormatPDF");
      //MM



      FileInfo fi = new FileInfo(filename + ".rtf");
      fi.Delete();

      //System.Diagnostics.Process process = new System.Diagnostics.Process();
      //process.Refresh();
      //process.StartInfo.FileName = filename + ".doc";
      //process.StartInfo.ErrorDialog = false;
      //process.StartInfo.Verb = "open";
      //process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
      //process.Start();

      System.Diagnostics.Process.Start(filename + ".pdf");

    
    }

    //------------------------------------------------------------------------+
    //                     buttonbuttonAzzeraValori_Click                     |
    //------------------------------------------------------------------------+
    private void buttonbuttonAzzeraValori_Click(object sender, RoutedEventArgs e)
    {
      //_xTXP.Save();
      foreach (XmlNode item in _xTXP.Document.SelectNodes("//Node"))
      {
        if (item.Attributes["Checked"] == null)
        {
          XmlAttribute attr = _xTXP.Document.CreateAttribute("Checked");
          item.Attributes.Append(attr);
        }
        item.Attributes["Checked"].Value = "False";
        if (item.Attributes["NotaTDL"] == null)
        {
          XmlAttribute attr = _xTXP.Document.CreateAttribute("NotaTDL");
          item.Attributes.Append(attr);
        }
        item.Attributes["NotaTDL"].Value = "";
        StaticUtilities.MarkNodeAsModified(item,App.OBJ_MOD); _isModified = true;
      }
      RevisoftApplication.XmlManager x = new XmlManager();
      x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
      _xTXP.isModified = _isModified;_xTXP.Save(_isModified);
      _isModified = false;
      TreeXmlProvider.Document = x.LoadEncodedFile(_xTXP.File);
      TreeXmlProvider.Refresh();
      LoadDataSource();
    }

    //------------------------------------------------------------------------+
    //                         CheckBox_SourceUpdated                         |
    //------------------------------------------------------------------------+
    private void CheckBox_SourceUpdated(object sender, RoutedEventArgs e)
    {
      string IDNodo;
      XmlNode node;

      IDNodo =
        (
          (XmlAttribute)
            (
              (
                (System.Windows.Controls.CheckBox)(sender)
              ).Tag
            )
        ).Value.ToString();
      node = _xTXP.Document.SelectSingleNode("//Node[@ID='" + IDNodo + "']");
      if (((System.Windows.Controls.CheckBox)(sender)).IsChecked == true)
      {
        (
          (TextBox)
            (
              (
                (StackPanel)
                  (
                    (
                      (System.Windows.Controls.CheckBox)(sender)
                    ).Parent
                  )
              ).Children[2]
            )
        ).IsEnabled = true;
        if (node != null)
        {
          if (node.Attributes["Checked"] == null)
          {
            XmlAttribute attr = node.OwnerDocument.CreateAttribute("Checked");
            node.Attributes.Append(attr);
          }
          node.Attributes["Checked"].Value = "True";
          if (node.Attributes["NotaTDL"] == null)
          {
            XmlAttribute attr = node.OwnerDocument.CreateAttribute("NotaTDL");
            node.Attributes.Append(attr);
          }
          node.Attributes["NotaTDL"].Value = "";
          StaticUtilities.MarkNodeAsModified(node, App.OBJ_MOD);_isModified = true;
        }
        //_xTXP.Save();
      }
      else
      {
        (
          (TextBox)
            (
              (
                (StackPanel)
                  (
                    (
                      (System.Windows.Controls.CheckBox)(sender)
                    ).Parent
                  )
              ).Children[2]
            )
        ).IsEnabled = false;
        if (node != null)
        {
          if (node.Attributes["Checked"] == null)
          {
            XmlAttribute attr = node.OwnerDocument.CreateAttribute("Checked");
            node.Attributes.Append(attr);
          }
          node.Attributes["Checked"].Value = "False";
          if (node.Attributes["NotaTDL"] == null)
          {
            XmlAttribute attr = node.OwnerDocument.CreateAttribute("NotaTDL");
            node.Attributes.Append(attr);
          }
          node.Attributes["NotaTDL"].Value = "";
          StaticUtilities.MarkNodeAsModified(node, App.OBJ_MOD);_isModified = true;
        }
        //_xTXP.Save();
        ((TextBox)(((StackPanel)(((System.Windows.Controls.CheckBox)(sender)).Parent)).Children[2])).Text = "";
        ((System.Windows.Controls.CheckBox)(sender)).Focus();
      }
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      ;
    }

    //------------------------------------------------------------------------+
    //                           TextBox_LostFocus                            |
    //------------------------------------------------------------------------+
    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
      string IDNodo;
      XmlNode node;

      IDNodo =
        (
          (XmlAttribute)
            (
              (
                (System.Windows.Controls.CheckBox)
                  (
                    (
                      (StackPanel)(((TextBox)(sender)).Parent)
                    ).Children[1]
                  )
              ).Tag
            )
        ).Value.ToString();
      node = _xTXP.Document.SelectSingleNode("//Node[@ID='" + IDNodo + "']");
      if (node != null)
      {
        if (node.Attributes["NotaTDL"] == null)
        {
          XmlAttribute attr = node.OwnerDocument.CreateAttribute("NotaTDL");
          node.Attributes.Append(attr);
        }
        node.Attributes["NotaTDL"].Value = ((TextBox)(sender)).Text;
        StaticUtilities.MarkNodeAsModified(node, App.OBJ_MOD);_isModified = true;
      }
      //_xTXP.Save();
    }

    private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
    {
      e.Handled = true;
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (e.NewSize.Width - 35 - 810 <= 0)
      {
        return;
      }

      SVTreeFixed.Width = 810;
      SVTreeFixed.Height = e.NewSize.Height - 190;
      SVTreeFixed.Margin = new Thickness(0, -20, 0, 0);

      SVTree.Width = e.NewSize.Width - 35 - 810;
      SVTree.Height = e.NewSize.Height - 190;
      SVTree.Margin = new Thickness(-20, -20, 0, 0);

      SVTreeHeader.Width = e.NewSize.Width - 5 - 710;
      SVTreeHeader.Margin = new Thickness(-20, 0, 0, 0);
      SVTreeHeader.Padding = new Thickness(151, 0, 0, 0);

      gridTV.Width = tvMain.Width;
      gridTVFixed.Width = tvMainFixed.Width;

      SVTree.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
      SVTreeFixed.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

      SVTree.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
      SVTreeHeader.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
    }

    private void SVTree_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      SVTreeHeader.ScrollToHorizontalOffset((sender as ScrollViewer).HorizontalOffset);

      SVTreeFixed.ScrollToVerticalOffset((sender as ScrollViewer).VerticalOffset);
    }

    private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      SVTree.ScrollToVerticalOffset(SVTree.VerticalOffset - e.Delta);
      e.Handled = true;
    }

    private void buttonNoteOperative_Click(object sender, RoutedEventArgs e)
    {
      wNoteOperativeTODOLIST nn = new wNoteOperativeTODOLIST();
      nn.ShowDialog();
    }
  }
}