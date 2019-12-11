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

  public partial class wWorkAreaTree_SOSPESI : System.Windows.Window
  {
    int idtree = 0;
    XmlDocument doctree = null;
    public string SelectedTreeSource = "";
    public string SelectedDataSource = "";
    public string SelectedSessioneSource = "";

    private string _cliente = "";
    private App.TipoAttivita _TipoAttivita = App.TipoAttivita.Sconosciuto;
    private bool firsttime = true;

    public string TitoloSessione = "";
    public string ImportFileName = "";

    public string IDTree = "-1";
    public string IDCliente = "-1";
    public string IDSessione = "-1";

    public XmlDataProviderManager _xTXP;
    XmlDataProvider TreeXmlProvider;
    XmlDocument xmlTMP = new XmlDocument();

    string fileattuale = "";

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

    private void GeneraTitolo()
    {
      txtTitoloRagioneSociale.Text = "Cliente: " + _cliente;
    }

  
    public wWorkAreaTree_SOSPESI()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      labelAttivita.Content = "";

      TreeXmlProvider = this.FindResource("xdpTree") as XmlDataProvider;

      Utilities u = new Utilities();

            string value = u.TitoloAttivita( App.TipoAttivita.Incarico);
            TabItem ti = new TabItem();
           
         //   ti.Background = new SolidColorBrush( Colors.LightBlue );
            ti.Header = "1) " + value;
            tabControlSospesi.Items.Insert(0, ti );

            ti = new TabItem();    
         //   ti.Background = new SolidColorBrush( Colors.LightBlue );
            ti.Header = "1 CS) " + value ;
            tabControlSospesi.Items.Insert(1, ti );
            
            ti = new TabItem();    
         //   ti.Background = new SolidColorBrush( Colors.LightBlue );
              ti.Header = "1 SU) " + value ;
            tabControlSospesi.Items.Insert(2, ti );

              ti = new TabItem();    
         //   ti.Background = new SolidColorBrush( Colors.LightBlue );
            ti.Header = "1 REV) " + value ;
            tabControlSospesi.Items.Insert(3, ti );

            value = u.TitoloAttivita(App.TipoAttivita.ISQC);
            ti = new TabItem();

            //    ti.Background = new SolidColorBrush(Colors.LightSteelBlue);
            ti.Header = "ISQC";
            tabControlSospesi.Items.Insert(4, ti);

      value = u.TitoloAttivita(App.TipoAttivita.Revisione);
      ti = new TabItem();

            //   ti.Background = new SolidColorBrush( Colors.LightCoral );
            ti.Header = "2) " + value;
            tabControlSospesi.Items.Insert( 5, ti ); 

      value = u.TitoloAttivita(App.TipoAttivita.Bilancio);
      ti = new TabItem();

            //  ti.Background = new SolidColorBrush( Colors.LightCyan );
            ti.Header = "3) " + value;
            tabControlSospesi.Items.Insert( 6, ti );

      value = u.TitoloAttivita(App.TipoAttivita.Conclusione);
      ti = new TabItem();

            //    ti.Background = new SolidColorBrush( Colors.LightGoldenrodYellow );
            ti.Header = "9) " + value;
            tabControlSospesi.Items.Insert( 7, ti ); 

            value = u.TitoloAttivita( App.TipoAttivita.Verifica );
            ti = new TabItem();
            //      ti.Background = new SolidColorBrush( Colors.LightGray );
            ti.Header = "4) " + value;
            tabControlSospesi.Items.Insert( 8, ti );

            value = u.TitoloAttivita(App.TipoAttivita.PianificazioniVerifica);
            ti = new TabItem();
            //     ti.Background = new SolidColorBrush(Colors.LightPink);
            ti.Header = value;
            tabControlSospesi.Items.Insert(9, ti);

            value = u.TitoloAttivita( App.TipoAttivita.Vigilanza );
            ti = new TabItem();
         //   ti.Background = new SolidColorBrush( Colors.LightGreen );
            ti.Header = "5) " + value;
            tabControlSospesi.Items.Insert( 10, ti );            

            value = u.TitoloAttivita( App.TipoAttivita.PianificazioniVigilanza );
            ti = new TabItem();
            //   ti.Background = new SolidColorBrush( Colors.LightSeaGreen );
            ti.Header = value;
            tabControlSospesi.Items.Insert( 11, ti );
        }

    #region TreeDataSource

    private void SaveTreeSource()
    {
      //if (TreeXmlProvider.Document != null)
      //{
      //    RevisoftApplication.XmlManager x = new XmlManager();
      //    x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
      //    x.SaveEncodedFile(SelectedTreeSource, TreeXmlProvider.Document.OuterXml);
      //}
    }

    //----------------------------------------------------------------------------+
    //                               LoadTreeSource                               |
    //----------------------------------------------------------------------------+

    public void LoadTreeSource()
    {



      IsEnabled = false;
      if (firsttime)
      {
        firsttime = false;

        switch (TipoAttivita)
        {
          case App.TipoAttivita.Incarico:

            idtree = 3; //App.TipoFile.Incarico;

            break;
          case App.TipoAttivita.IncaricoCS:

            idtree = 71; //App.TipoFile.Incarico;

            break;
         case App.TipoAttivita.IncaricoSU:

            idtree = 72; //App.TipoFile.Incarico;

            break;
        case App.TipoAttivita.IncaricoREV:

            idtree = 73; //App.TipoFile.Incarico;

            break;
          case App.TipoAttivita.ISQC:

            idtree = 28; // App.TipoFile.ISQC.ToString();

            break;
          case App.TipoAttivita.Revisione:

            idtree = 1; // App.TipoFile.Revisione.ToString();

            break;
          case App.TipoAttivita.Bilancio:

            idtree = 4; // App.TipoFile.Bilancio.ToString();

            break;
          case App.TipoAttivita.Conclusione:

            idtree = 19; //App.TipoFile.Conclusione.ToString();

            break;
          case App.TipoAttivita.Verifica:

            idtree = 2; //App.TipoFile.Verifica.ToString();

            break;
          case App.TipoAttivita.Vigilanza:

            idtree = 18; // App.TipoFile.Vigilanza.ToString();

            break;
          case App.TipoAttivita.PianificazioniVerifica:
            idtree = 26; //App.TipoFile.PianificazioniVerifica.ToString();

            break;
          case App.TipoAttivita.PianificazioniVigilanza:
            idtree = 27; //App.TipoFile.PianificazioniVigilanza.ToString();

            break;
          default:
            return;
        }
   
   //    string area =((App.TipoFile)Enum.Parse(typeof(App.TipoFile), idtree.ToString())).ToString();

   //    doctree =  cBusinessObjects.NewLoadEncodedFile("",(idtree.ToString());
    
        doctree = cBusinessObjects.GetDataSessioniFile(App.AppVersione,idtree, int.Parse(IDCliente));

        if (doctree.InnerXml == "")
        {
          TreeXmlProvider.Document = new XmlDocument();
          TreeXmlProvider.Refresh();
          IsEnabled = true;
          return;
        }

        Hashtable temphtSessioniAlias = cBusinessObjects.GetDataSessioniHT("htSessioniAlias",App.AppVersione, int.Parse(IDCliente), idtree);
        Hashtable temphtSessioniID = cBusinessObjects.GetDataSessioniHT("htSessioniID",App.AppVersione, int.Parse(IDCliente), idtree);
        string ids = "";
        Hashtable _xHT = new Hashtable();
        foreach (XmlNode item in doctree.SelectNodes("//Node"))
        {
          item.Attributes["Expanded"].Value = "True";
          item.Attributes["Selected"].Value = "False";
          bool hasospesi = false;
          foreach (XmlNode itemSessione in item.SelectNodes("Sessioni/Sessione"))
          {

            if (!_xHT.Contains(itemSessione.Attributes["Alias"].Value.ToString()))
            {
              foreach (DictionaryEntry pair in temphtSessioniAlias)
              {
             
                if (pair.Value.ToString() == itemSessione.Attributes["Alias"].Value.ToString().Replace("\r\n", "/").Replace("/Incarico", "").Replace("/Riesame", ""))
                {
                  ids = temphtSessioniID[pair.Key].ToString();
                }
              }
              if (ids == "")
                continue;
              DataTable datisospesi = cBusinessObjects.GetData(int.Parse(item.Attributes["ID"].Value), typeof(TabellaSospesi), int.Parse(IDCliente), int.Parse(ids), idtree);
              foreach (DataRow dd in datisospesi.Rows)
              {
                if (itemSessione.Attributes["Sospesi"] == null)
                {
                  XmlAttribute attr = doctree.CreateAttribute("Sospesi");
                  itemSessione.Attributes.Append(attr);
                }
                if (itemSessione.Attributes["idsessione"] == null)
                {
                  XmlAttribute attr = doctree.CreateAttribute("idsessione");
                  itemSessione.Attributes.Append(attr);
                }
                itemSessione.Attributes["Sospesi"].Value = dd["SospesiTxt"].ToString();
                itemSessione.Attributes["idsessione"].Value = ids;
                hasospesi = true;
              }

            }
          }
          if (hasospesi == false)
          {
            if (item.ChildNodes.Count <= 1 && item.ParentNode != null && item.ParentNode.Attributes["HaSospesi"] == null)
            {
              XmlNode itemhere = item;
              while (item.ChildNodes.Count == 1 && itemhere.ParentNode != null && itemhere.ParentNode.ChildNodes.Count == 2)
              {
                itemhere = itemhere.ParentNode;
              }
              if (itemhere.ParentNode != null) itemhere.ParentNode.RemoveChild(itemhere);
            }
          }
          else
          {
            if (item.Attributes["HaSospesi"] == null)
            {
              XmlAttribute attr = doctree.CreateAttribute("HaSospesi");
              item.Attributes.Append(attr);
            }
            item.Attributes["HaSospesi"].Value = "true";
          }
        }
      }
      TreeXmlProvider.Document = doctree;
      // interfaccia
      Utilities u = new Utilities();
      labelAttivita.Content = u.TitoloAttivita(_TipoAttivita);
      TreeXmlProvider.Refresh();
      LoadDataSource();
      IsEnabled = true;
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


    private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
    {
      ;
    }
    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      StaticUtilities.PurgeXML(fileattuale + "tmp");
    }
    //----------------------------------------------------------------------------+
    //                           OnItemMouseDoubleClick                           |
    //----------------------------------------------------------------------------+

    private void OnItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {

      if (e.ClickCount != 2) return;
      XmlNode node;
      try { node = ((XmlNode)(tvMain.SelectedItem)); }
      catch (Exception ex)
      {
        string log = ex.Message;
        e.Handled = true;
        return;
      }
      if (node == null)
      {
        e.Handled = true;
        return;
      }
      string aliasnodo = ((XmlAttribute)(((Image)(sender)).ToolTip)).Value;
      string idnodo = node.Attributes["ID"].Value;

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
      o.ReadOnly = false;


      o.Load(idnodo, IDCliente, aliasnodo, idtree);
      o.ShowDialog();
      if (o.Changed == true)
      {
        firsttime = true;
        LoadTreeSource();
      }
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
      rtf_text += "{\\fonttbl{\\f0 Arial}}";
      rtf_text += "{\\colortbl;\\red0\\green255\\blue255;\\red204\\green204\\blue204;\\red255\\green255\\blue255;\\red230\\green230\\blue230;}";
      rtf_text += "\\viewkind4\\uc1";

      rtf_text += "\\fs30 " + _cliente + " \\fs20 \\line \\line ";

      rtf_text += "\\fs30 " + "SOSPESI" + " \\fs20 \\line \\line ";



      string sessione = "";
      int[] artab = new int[8];
      artab[0] = 0;
      artab[1] = 2;
      artab[2] = 3;
      artab[3] = 5;
      artab[4] = 7;
      artab[5] = 4;
      artab[6] = 6;
      artab[7] = 1;

      for (int k = 0; k < artab.Length; k++)
      {
        tabControlSospesi.SelectedIndex = artab[k];
        //    rtf_text += "\\fs30 " + labelAttivita.Content + " \\fs20 \\line \\line ";
        rtf_text += "\\fs30 " + ((System.Windows.Controls.HeaderedContentControl)tabControlSospesi.Items[tabControlSospesi.SelectedIndex]).Header.ToString() + " \\fs20 \\line \\line ";
        if (doctree.InnerXml != "")
          foreach (XmlNode item in doctree.SelectSingleNode("/Tree").SelectNodes("//Node"))
          {
            try
            {
              foreach (XmlNode itemSessione in item.SelectNodes("Sessioni/Sessione"))
              {
                if (itemSessione.Attributes["Sospesi"] != null && itemSessione.Attributes["Sospesi"].Value != "")
                {
                  if (sessione != itemSessione.Attributes["Alias"].Value.Replace("\r\n", " "))
                  {
                    sessione = itemSessione.Attributes["Alias"].Value.Replace("\r\n", " ");
                    rtf_text += "\\fs26 " + itemSessione.Attributes["Alias"].Value.Replace("\r\n", " ") + " \\fs20 \\line \\line ";
                  }

                  rtf_text += "\\b " + item.Attributes["Codice"].Value + " " + item.Attributes["Titolo"].Value + " \\b0 \\line ";

                  {
                    string istruzione = itemSessione.Attributes["Sospesi"].Value;

                    if (istruzione.Trim() != "")
                    {
                      //    rtf_text += "\\fs20 " + istruzione + " \\fs20 \\line ";

                    }
                  }

                  rtf_text += " ";
                }
              }
            }
            catch (Exception ex)
            {
              string log = ex.Message;
            }

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
      wrdDoc.SaveAs(filename + ".doc", filename + ".rtf", "WdSaveFormat.wdFormatDocument");
      //MM



      FileInfo fi = new FileInfo(filename + ".rtf");
      fi.Delete();

      System.Diagnostics.Process process = new System.Diagnostics.Process();
      process.Refresh();
      process.StartInfo.FileName = filename + ".doc";
      process.StartInfo.ErrorDialog = false;
      process.StartInfo.Verb = "open";
      process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
      process.Start();

      //System.Diagnostics.Process.Start(filename + ".doc");


    }

    private void CheckBox_SourceUpdated(object sender, RoutedEventArgs e)
    {
      wCommentiTODOList o = new wCommentiTODOList();

      o.Owner = this;

      o.ReadOnly = false;

      o.Nodo = ((XmlAttribute)(((System.Windows.Controls.CheckBox)(sender)).Tag)).Value;

      o.Load();

      o.ShowDialog();

      if (o.NotEmpty)
      {
        ((System.Windows.Controls.CheckBox)(sender)).IsChecked = true;
      }
      else
      {
        ((System.Windows.Controls.CheckBox)(sender)).IsChecked = false;
      }
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      switch ( tabControlSospesi.SelectedIndex )
            {
                default:
                case 0:
                    TipoAttivita = App.TipoAttivita.Incarico;
                    break;
                case 1:
                    TipoAttivita = App.TipoAttivita.IncaricoCS;
                    break;
                case 2:
                    TipoAttivita = App.TipoAttivita.IncaricoSU;
                    break;
                case 3:
                    TipoAttivita = App.TipoAttivita.IncaricoREV;
                    break;
                case 4:
                    TipoAttivita = App.TipoAttivita.ISQC;
                    break;
                case 5:
                    TipoAttivita = App.TipoAttivita.Revisione;
                    break;
                case 6:
                    TipoAttivita = App.TipoAttivita.Bilancio;
                    break;
                case 7:
                    TipoAttivita = App.TipoAttivita.Conclusione;
                    break;
                case 8:
                    TipoAttivita = App.TipoAttivita.Verifica;
                    break;
                case 9:
                    TipoAttivita = App.TipoAttivita.PianificazioniVerifica;
                    break;
                case 10:
                    TipoAttivita = App.TipoAttivita.Vigilanza;
                    break;                
                case 11:
                    TipoAttivita = App.TipoAttivita.PianificazioniVigilanza;
                    break;

            }

      firsttime = true;
      LoadTreeSource();
    }
  }
}