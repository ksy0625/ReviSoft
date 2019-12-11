//----------------------------------------------------------------------------+
//                           wWorkAreaTree.xaml.cs                            |
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
using System.Xml.Linq;
using System.Xml.Schema;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using RevisoftApplication.BRL;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;

namespace RevisoftApplication
{
  public static class MyExtensions
  {
    public static string ToStringAlignAttributes(this XDocument document)
    {
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Indent = true;
      settings.OmitXmlDeclaration = true;
      settings.NewLineOnAttributes = true;
      StringBuilder stringBuilder = new StringBuilder();
      using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
        document.WriteTo(xmlWriter);
      return stringBuilder.ToString();
    }
  }

  class Program
  {
    private static class Xsi
    {
      public static XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
      public static XName schemaLocation = xsi + "schemaLocation";
      public static XName noNamespaceSchemaLocation = xsi + "noNamespaceSchemaLocation";
    }



    public static XDocument Normalize(XDocument source, XmlSchemaSet schema)
    {
      bool havePSVI = false;

      // validate, throw errors, add PSVI information
      if (schema != null)
      {
        source.Validate(schema, null, true);
        havePSVI = true;
      }
      return new XDocument(
        source.Declaration,
        source.Nodes().Select(n =>
        {
          // Remove comments, processing instructions, and text nodes that are
          // children of XDocument. Only white space text nodes are allowed as
          // children of a document, so we can remove all text nodes.
          if (n is XComment || n is XProcessingInstruction || n is XText)
            return null;
          XElement e = n as XElement;
          if (e != null) return NormalizeElement(e, havePSVI);
          return n;
        }
        )
      );
    }

    public static bool DeepEqualsWithNormalization(XDocument doc1, XDocument doc2,
      XmlSchemaSet schemaSet)
    {
      XDocument d1 = Normalize(doc1, schemaSet);
      XDocument d2 = Normalize(doc2, schemaSet);
      return XNode.DeepEquals(d1, d2);
    }

    private static IEnumerable<XAttribute> NormalizeAttributes(XElement element,
      bool havePSVI)
    {
      return element.Attributes()
        .Where(a => !a.IsNamespaceDeclaration
          && a.Name != Xsi.schemaLocation
          && a.Name != Xsi.noNamespaceSchemaLocation)
        .OrderBy(a => a.Name.NamespaceName)
        .ThenBy(a => a.Name.LocalName)
        .Select(a =>
        {
          if (havePSVI)
          {
            var dt = a.GetSchemaInfo().SchemaType.TypeCode;
            switch (dt)
            {
              case XmlTypeCode.Boolean:
                return new XAttribute(a.Name, (bool)a);
              case XmlTypeCode.DateTime:
                return new XAttribute(a.Name, (DateTime)a);
              case XmlTypeCode.Decimal:
                return new XAttribute(a.Name, (decimal)a);
              case XmlTypeCode.Double:
                return new XAttribute(a.Name, (double)a);
              case XmlTypeCode.Float:
                return new XAttribute(a.Name, (float)a);
              case XmlTypeCode.HexBinary:
              case XmlTypeCode.Language:
                return new XAttribute(a.Name, ((string)a).ToLower());
            }
          }
          return a;
        });
    }

    private static XNode NormalizeNode(XNode node, bool havePSVI)
    {
      // trim comments and processing instructions from normalized tree
      if (node is XComment || node is XProcessingInstruction) return null;
      XElement e = node as XElement;
      if (e != null) return NormalizeElement(e, havePSVI);
      // Only thing left is XCData and XText, so clone them
      return node;
    }

    private static XElement NormalizeElement(XElement element, bool havePSVI)
    {
      if (havePSVI)
      {
        var dt = element.GetSchemaInfo();
        switch (dt.SchemaType.TypeCode)
        {
          case XmlTypeCode.Boolean:
            return new XElement(element.Name,
              NormalizeAttributes(element, havePSVI),
              (bool)element);
          case XmlTypeCode.DateTime:
            return new XElement(element.Name,
              NormalizeAttributes(element, havePSVI),
              (DateTime)element);
          case XmlTypeCode.Decimal:
            return new XElement(element.Name,
              NormalizeAttributes(element, havePSVI),
              (decimal)element);
          case XmlTypeCode.Double:
            return new XElement(element.Name,
              NormalizeAttributes(element, havePSVI),
              (double)element);
          case XmlTypeCode.Float:
            return new XElement(element.Name,
              NormalizeAttributes(element, havePSVI),
              (float)element);
          case XmlTypeCode.HexBinary:
          case XmlTypeCode.Language:
            return new XElement(element.Name,
              NormalizeAttributes(element, havePSVI),
              ((string)element).ToLower());
          default:
            return new XElement(element.Name,
              NormalizeAttributes(element, havePSVI),
              element.Nodes().Select(n => NormalizeNode(n, havePSVI)));
        }
      }
      else
      {
        return new XElement(element.Name,
          NormalizeAttributes(element, havePSVI),
          element.Nodes().Select(n => NormalizeNode(n, havePSVI)));
      }
    }
  }

  public partial class WindowWorkAreaTree : Window
  {
    public bool tobereopened = false;
    public bool CheckCompleto = false;

    public string SelectedTreeSource = "";
    public string SelectedDataSource = "";
    public string SelectedSessioneSource = "";

    public bool ReadOnly = true;
    public bool ApertoInSolaLettura = true;

    public string _cliente = "";
    public string Esercizio = "";

    public string SessioneAlias = "";
    public string SessioneAliasAdditivo = "";
    public string SessioneFile = "";
    public string SessioneID = "";
    public string SessioneSigillo = "";
    public string SessioneSigilloData = "";
    public string SessioneSigilloPassword = "";

    private string selectedAlias = "";
    private string selectedAliasCodificato = "";
    private App.TipoAttivita _TipoAttivita = App.TipoAttivita.Sconosciuto;

    public string __IDTree = "-1";
    public string IDCliente = "-1";
    public string IDSessione = "-1";

    private bool firsttime = true;
    public bool StampaTemporanea = false;

    public XmlDataProviderManager _x;
    public XmlDataProvider TreeXmlProvider;

    Hashtable YearColor = new Hashtable();
    private Hashtable htStati = new Hashtable();
    Hashtable htSessioni = new Hashtable();
    Hashtable htSessioniAlias = new Hashtable();
    Hashtable htSessioniID = new Hashtable();
    Hashtable htSessioneSigillo = new Hashtable();
    Hashtable htSessioneSigilloData = new Hashtable();
    Hashtable htSessioneSigilloPassword = new Hashtable();

    ArrayList ALXTPP = new ArrayList();

    public string IDTree
    {
      get
      {
        return __IDTree;
      }
      set
      {
        __IDTree = value;
      }
    }

    public string Cliente
    {
      get
      {
        return _cliente;
      }
      set
      {
        _cliente = value;
        txtTitoloRagioneSociale.Text = _cliente;
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

    public Delegate EmptyDelegate { get; private set; }

    private bool m_isModified = false; // E.B.

    public WindowWorkAreaTree()
    {
      InitializeComponent();
      txtTitoloAttivita.Foreground = App._arrBrushes[0];
      txtTitoloRagioneSociale.Foreground = App._arrBrushes[9];
      ButtonBar.Background = App._arrBrushes[12];
      SolidColorBrush tmpBrush = (SolidColorBrush)Resources["buttonHover"];
      tmpBrush.Color = ((SolidColorBrush)App._arrBrushes[13]).Color;
      //andrea 2.9
      this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
      this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;


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

      //Colonna selezionata
      YearColor.Add(-1, "82BDE4");
      //Colori colonne di sezione
      YearColor.Add(2000, "F1F1F1");
      YearColor.Add(2001, "D3D3D3");
      YearColor.Add(2002, "F1F1F1");
      YearColor.Add(2003, "D3D3D3");
      YearColor.Add(2004, "F1F1F1");
      YearColor.Add(2005, "D3D3D3");
      YearColor.Add(2006, "F1F1F1");
      YearColor.Add(2007, "D3D3D3");
      YearColor.Add(2008, "F1F1F1");
      YearColor.Add(2009, "D3D3D3");
      YearColor.Add(2010, "F1F1F1");
      YearColor.Add(2011, "D3D3D3");
      YearColor.Add(2012, "F1F1F1");
      YearColor.Add(2013, "D3D3D3");
      YearColor.Add(2014, "F1F1F1");
      YearColor.Add(2015, "D3D3D3");
      YearColor.Add(2016, "F1F1F1");
      YearColor.Add(2017, "D3D3D3");
      YearColor.Add(2018, "F1F1F1");
      YearColor.Add(2019, "D3D3D3");
      YearColor.Add(2020, "F1F1F1");
      YearColor.Add(2021, "D3D3D3");
      YearColor.Add(2022, "F1F1F1");
      YearColor.Add(2023, "D3D3D3");
      YearColor.Add(2024, "F1F1F1");
      YearColor.Add(2025, "D3D3D3");
      YearColor.Add(2026, "F1F1F1");
      YearColor.Add(2027, "D3D3D3");
      YearColor.Add(2028, "F1F1F1");

    }

    #region TreeDataSource

    private void SaveTreeSource(bool isMod = false)
    {
      string str, sessionColor;
      SolidColorBrush tmpBrush;

      if (TreeXmlProvider.Document != null)
      {
        tmpBrush = (SolidColorBrush)App._arrBrushes[10];
        sessionColor = "#" + tmpBrush.Color.A.ToString("X2") + tmpBrush.Color.R.ToString("X2") + tmpBrush.Color.G.ToString("X2") + tmpBrush.Color.B.ToString("X2");
        str = string.Format("//Sessione[@Selected='{0}']", sessionColor);
        foreach (XmlNode n in TreeXmlProvider.Document.SelectNodes(str))
        {
          n.Attributes["Selected"].Value = App.DEFAULT_COLOR_SESSION_SELECTED;
        }
        RevisoftApplication.XmlManager x = new XmlManager();
        x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
        x.SaveEncodedFile(SelectedTreeSource, TreeXmlProvider.Document.OuterXml, isMod, true);
        TreeXmlProvider.Document = cBusinessObjects.NewLoadEncodedFile(SelectedTreeSource,IDTree);

        string versione = cBusinessObjects.GetVersioneSessione(IDSessione, ((App.TipoFile)Enum.Parse(typeof(App.TipoFile), IDTree)).ToString());
              
        cBusinessObjects.idcliente = int.Parse(IDCliente);
        cBusinessObjects.SaveSessioniFile(TreeXmlProvider.Document.OuterXml, "",versione);
        cBusinessObjects.SaveSessioniHT(htSessioniAlias, "htSessioniAlias",versione);
        cBusinessObjects.SaveSessioniHT(htSessioniID, "htSessioniID",versione);
        cBusinessObjects.SaveSessioniHT(htSessioni, "htSessioni",versione);
        Hashtable ht = new Hashtable();
        ht.Add("0", selectedAliasCodificato);
        cBusinessObjects.SaveSessioniHT(ht, "AliasCodificato",versione);

      }


      ReloadStatoNodiPadre();
    }

    //----------------------------------------------------------------------------+
    //                           SaveTreeSourceNoReload                           |
    //----------------------------------------------------------------------------+

    private void SaveTreeSourceNoReload(bool isMod = false)
    {
      string str, sessionColor;
      SolidColorBrush tmpBrush;

      if (TreeXmlProvider.Document != null)
      {
        tmpBrush = (SolidColorBrush)App._arrBrushes[10];
        sessionColor = "#" + tmpBrush.Color.A.ToString("X2") + tmpBrush.Color.R.ToString("X2") + tmpBrush.Color.G.ToString("X2") + tmpBrush.Color.B.ToString("X2");
        str = string.Format("//Sessione[@Selected='{0}']", sessionColor);
        foreach (XmlNode n in TreeXmlProvider.Document.SelectNodes(str))
        {
          n.Attributes["Selected"].Value = App.DEFAULT_COLOR_SESSION_SELECTED;
        }
        RevisoftApplication.XmlManager x = new XmlManager();
        x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
        x.SaveEncodedFile(SelectedTreeSource, TreeXmlProvider.Document.OuterXml, isMod, true);
        cBusinessObjects.idcliente = int.Parse(IDCliente);
        string versione = cBusinessObjects.GetVersioneSessione(IDSessione, ((App.TipoFile)Enum.Parse(typeof(App.TipoFile), IDTree)).ToString());
        
        cBusinessObjects.SaveSessioniFile(TreeXmlProvider.Document.OuterXml, "",versione);
        cBusinessObjects.SaveSessioniHT(htSessioniAlias, "htSessioniAlias",versione);
        cBusinessObjects.SaveSessioniHT(htSessioniID, "htSessioniID",versione);
        cBusinessObjects.SaveSessioniHT(htSessioni, "htSessioni",versione);
        if (selectedAliasCodificato != "")
        {
          Hashtable ht = new Hashtable();
          ht.Add("0", selectedAliasCodificato);
          cBusinessObjects.SaveSessioniHT(ht, "AliasCodificato",versione);
        }
      }
    }

    //----------------------------------------------------------------------------+
    //                               LoadTreeSource                               |
    //----------------------------------------------------------------------------+


    public void LoadTreeSource()
    {
      cBusinessObjects.idcliente = int.Parse(IDCliente);
      cBusinessObjects.idsessione = int.Parse(IDSessione); 

      bool isCached = true;

      string fileName;

      //------------------------------------------------------- titolo attivita
      Utilities u = new Utilities();
      txtTitoloAttivita.Text = u.TitoloAttivita(_TipoAttivita);
      cBusinessObjects.TitoloAttivita = u.TitoloAttivita(_TipoAttivita);
      //-------------------------------------------------- visibilita' pulsanti
      btn_StampaVerbale.Visibility = System.Windows.Visibility.Collapsed;
      btn_CopiaLibroSociale.Visibility = System.Windows.Visibility.Collapsed;
      switch (TipoAttivita)
      {
        case App.TipoAttivita.ISQC:
        case App.TipoAttivita.Incarico:
        case App.TipoAttivita.IncaricoCS:
        case App.TipoAttivita.IncaricoREV:
        case App.TipoAttivita.IncaricoSU:
        case App.TipoAttivita.Revisione:
        case App.TipoAttivita.Bilancio:
        case App.TipoAttivita.Conclusione:
          break;
        case App.TipoAttivita.Verifica:
        case App.TipoAttivita.Vigilanza:
          btn_StampaVerbale.Visibility = System.Windows.Visibility.Visible;
          if (TipoAttivita == App.TipoAttivita.Verifica)
          {
            btn_CopiaLibroSociale.ToolTip = "Copia da Vigilanza";
            TextBlock_Btn_CopiaLibroSociale.Text = "Copia da Vigilanza";
          }
          else
          {
            btn_CopiaLibroSociale.ToolTip = "Copia da Controllo Contabile";
            TextBlock_Btn_CopiaLibroSociale.Text = "Copia da Controllo Contabile";
          }
          Uri uriSource = null;
          uriSource = new Uri("/RevisoftApplication;component/Images/icone/printer3.png", UriKind.Relative);
          img_StampaPDF.Source = new BitmapImage(uriSource);
          break;
        case App.TipoAttivita.RelazioneB:
        case App.TipoAttivita.RelazioneV:
        case App.TipoAttivita.RelazioneBC:
        case App.TipoAttivita.RelazioneVC:
        case App.TipoAttivita.RelazioneBV:
          btn_ArchivioAllegati.Visibility = System.Windows.Visibility.Collapsed;
          break;
        case App.TipoAttivita.Sconosciuto:
        default:
          break;
      }
      //-------------------------------- eventuale visualizzazione process wait
      if (App.m_bxmlCacheEnable)
      {
        //-------------------------------------- valuta se XML e' gia' in cache
        fileName = SelectedTreeSource.Split('\\').Last();
        isCached = App.m_xmlCache.ContainsKey(fileName);
      }



      //------------------------------------------------------ caricamento dati
      RevisoftApplication.XmlManager x = new XmlManager();
      x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
      TreeXmlProvider.Document = cBusinessObjects.NewLoadEncodedFile(SelectedTreeSource, IDTree);
      // se non corrisponde al tipo di file, lo cancella, lo inizializza al
      // template corretto e lo ricarica
      if (!u.CheckXmlDocument(
        TreeXmlProvider.Document, ((App.TipoFile)(Convert.ToInt32(IDTree))), "Tree"))
      {
        if (((App.TipoFile)(Convert.ToInt32(IDTree))) == App.TipoFile.RelazioneB)
        {

          TreeXmlProvider.Document = cBusinessObjects.NewLoadEncodedFile(SelectedTreeSource, IDTree);
        }
        else
        {

          this.Close();
          return;
        }
        if (((App.TipoFile)(Convert.ToInt32(IDTree))) == App.TipoFile.RelazioneBC)
        {

          TreeXmlProvider.Document = cBusinessObjects.NewLoadEncodedFile(SelectedTreeSource, IDTree);
        }
        else
        {

          this.Close();
          return;
        }
      }

      if (firsttime)
      {
        firsttime = false;
        MasterFile mf = MasterFile.Create();
        if (IDTree == "2")
        {
          ArrayList al = mf.GetPianificazioniVerifiche(IDCliente);
          foreach (Hashtable itemHT in al)
          {
            ALXTPP.Add(itemHT["ID"].ToString());
          }
        }
        if (IDTree == "18")
        {
          ArrayList al = mf.GetPianificazioniVigilanze(IDCliente);
          foreach (Hashtable itemHT in al)
          {
            ALXTPP.Add(itemHT["ID"].ToString());
          }
        }
        try
        {


          foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("//Node"))
          {
            if (item.ParentNode.Name == "Tree" || IDTree == "26" || IDTree == "27")
            {
              item.Attributes["Expanded"].Value = "True";
            }
            else
            {
              item.Attributes["Expanded"].Value = "False";
            }
            if (item.Attributes["Bold"] == null)
            {
              XmlAttribute attr = item.OwnerDocument.CreateAttribute("Bold");
              attr.Value = "False";
              item.Attributes.Append(attr);
            }
            if (item.ParentNode.Name == "Tree")
            {
              item.Attributes["Bold"].Value = "True";
            }
            item.Attributes["Selected"].Value = "False";
            if (item.Attributes["MinWidth"] == null)
            {
              XmlAttribute attr = item.OwnerDocument.CreateAttribute("MinWidth");
              item.Attributes.Append(attr);
            }
            if (IDTree == "26" || IDTree == "27")
            {
              item.Attributes["MinWidth"].Value = "100";
            }
            else
            {
              item.Attributes["MinWidth"].Value = "Auto";
            }
            if (item.Attributes["HighLighted"] == null)
            {
              XmlAttribute attr = item.OwnerDocument.CreateAttribute("HighLighted");
              attr.Value = "Black";
              item.Attributes.Append(attr);
            }
            item.Attributes["HighLighted"].Value = "Black";
            if (item.Attributes["Visible"] == null)
            {
              XmlAttribute attr = item.OwnerDocument.CreateAttribute("Visible");
              item.Attributes.Append(attr);
            }
            item.Attributes["Visible"].Value = "True";
            if (item.Attributes["Codice"].Value == "4")
            {
              item.Attributes["Expanded"].Value = "True";
            }
            if (item.Attributes["Codice"].Value == "5")
            {
              item.Attributes["Expanded"].Value = "True";
            }
          }
        }
        catch (Exception aa)
        {

        }
      }

      TreeXmlProvider.Refresh();

      LoadDataSource();





    }

    #endregion //--------------------------------------------------- TreeDataSource

    #region DataDataSource

    //----------------------------------------------------------------------------+
    //                           ConvertDataToEsercizio                           |
    //----------------------------------------------------------------------------+
    private string ConvertDataToEsercizio(string anno)
    {
      string returnvalue = "";
      MasterFile mf = MasterFile.Create();
      Hashtable clientetmp = mf.GetAnagrafica(Convert.ToInt32(IDCliente));

      switch ((App.TipoAnagraficaEsercizio)(Convert.ToInt32(clientetmp["Esercizio"].ToString())))
      {
        case App.TipoAnagraficaEsercizio.ACavallo:
          returnvalue = anno + "/" + ((Convert.ToInt32(anno) % 100) + 1).ToString();
          break;
        case App.TipoAnagraficaEsercizio.AnnoSolare:
        case App.TipoAnagraficaEsercizio.Sconosciuto:
        default:
          returnvalue = anno;
          break;
      }
      return returnvalue;
    }

    //----------------------------------------------------------------------------+
    //                           ConvertDataToEsercizio                           |
    //----------------------------------------------------------------------------+
    private string ConvertDataToEsercizio(string anno, Hashtable ht)
    {
      string returnvalue = "";

      if (!ht.Contains("Esercizio")) return anno;
      if (ht.Contains("Intermedio") && ht.Contains("EsercizioDal")
        && ht.Contains("EsercizioAl"))
      {
        returnvalue = ht["EsercizioDal"].ToString() + "\r\n"
          + ht["EsercizioAl"].ToString();
      }
      else
      {
        switch ((App.TipoAnagraficaEsercizio)(Convert.ToInt32(ht["Esercizio"].ToString())))
        {
          case App.TipoAnagraficaEsercizio.ACavallo:
            returnvalue = anno + "/" + ((Convert.ToInt32(anno) % 100) + 1).ToString();
            break;
          case App.TipoAnagraficaEsercizio.AnnoSolare:
          case App.TipoAnagraficaEsercizio.Sconosciuto:
          default:
            returnvalue = anno;
            break;
        }
      }
      return returnvalue;
    }

    bool nodialreadyloader = false;

    //----------------------------------------------------------------------------+
    //                               LoadDataSource                               |
    //----------------------------------------------------------------------------+
    private void LoadDataSource()
    { 

      cBusinessObjects.idcliente = int.Parse(IDCliente);
      cBusinessObjects.idsessione = int.Parse(IDSessione);

      SolidColorBrush scb = (SolidColorBrush)Resources["nodeSelected"];
      scb.Color = ((SolidColorBrush)App._arrBrushes[11]).Color;
      string versione = cBusinessObjects.GetVersioneSessione(IDSessione, ((App.TipoFile)Enum.Parse(typeof(App.TipoFile), IDTree)).ToString());

      XmlDocument xmldata = cBusinessObjects.GetDataSessioniFile(versione);
      htSessioni.Clear();
      htSessioniAlias.Clear();
      htSessioniID.Clear();
      Hashtable temphtSessioni = cBusinessObjects.GetDataSessioniHT("htSessioni",versione);
      Hashtable temphtSessioniAlias = cBusinessObjects.GetDataSessioniHT("htSessioniAlias",versione);
      Hashtable temphtSessioniID = cBusinessObjects.GetDataSessioniHT("htSessioniID",versione);
      SolidColorBrush tmpBrush = (SolidColorBrush)App._arrBrushes[10];
      string sessionColor = "#" +
        tmpBrush.Color.A.ToString("X2") +
        tmpBrush.Color.R.ToString("X2") +
        tmpBrush.Color.G.ToString("X2") +
        tmpBrush.Color.B.ToString("X2");

      if ((xmldata != null) && (temphtSessioni != null) && (temphtSessioniAlias != null) && (temphtSessioniID != null)
         && (xmldata.OuterXml != "") && (temphtSessioni.Count > 0) && (temphtSessioniAlias.Count > 0) && (temphtSessioniID.Count > 0))
      {
        htSessioni = temphtSessioni;
        htSessioniAlias = temphtSessioniAlias;
        htSessioniID = temphtSessioniID;

        if (htSessioniID.ContainsValue(IDSessione))
        {

          TreeXmlProvider.Document = xmldata;
          // setta sessione corrente
          int contsess = 0;

          foreach (XmlNode nodeTree in TreeXmlProvider.Document.SelectSingleNode("/Tree/Sessioni"))
          {
            if (htSessioniID[contsess].ToString() == IDSessione)
            {
              nodeTree.Attributes["Selected"].Value = sessionColor;
              selectedAliasCodificato = nodeTree.Attributes["Alias"].Value;
            }
            else
            {
              nodeTree.Attributes["Selected"].Value = "White";
            }
            contsess++;
          }

          XmlNodeList elemList = TreeXmlProvider.Document.GetElementsByTagName("Node");
          for (int i = 0; i < elemList.Count; i++)
          {
            contsess = 0;
            if (elemList[i].SelectSingleNode("Sessioni") != null)
            {
              foreach (XmlNode sess in elemList[i].SelectSingleNode("Sessioni"))
              {
                if (htSessioniID[contsess].ToString() == IDSessione)
                  sess.Attributes["Selected"].Value = sessionColor;
                else
                  sess.Attributes["Selected"].Value = "White";
                contsess++;
              }
            }


          }

          _x = new XmlDataProviderManager(SelectedDataSource);
          cBusinessObjects.hide_workinprogress();
          return;
        }
      }

      //cBusinessObjects.show_workinprogress("Prima creazione dell'albero in corso...");
      // membro IDCliente --> idCliente
      // membro IDSessione --> idOggetto
      // membro IDTree --> idTipoFile
      _x = new XmlDataProviderManager(SelectedDataSource);
      Utilities u = new Utilities();
      if (!u.CheckXmlDocument(
        _x.Document, ((App.TipoFile)(Convert.ToInt32(IDTree))), "Data"))
      {
        this.Close();
        cBusinessObjects.hide_workinprogress();
        return;
      }

      if (nodialreadyloader == false)
      {

        ReloadNodi();

        // codice aggiunto per impostare i colori di una sessione appena creata - inizio
        string str;
        str = string.Format("/Tree/Sessioni/Sessione[@Selected='{0}']", App.DEFAULT_COLOR_SESSION_SELECTED);
        XmlNode nodeSessione = TreeXmlProvider.Document.SelectSingleNode(str);
        if (nodeSessione != null) nodeSessione.Attributes["Selected"].Value = sessionColor;
        XmlNodeList elemList = TreeXmlProvider.Document.GetElementsByTagName("Node");
        str = string.Format("Sessioni/Sessione[@Selected='{0}']", App.DEFAULT_COLOR_SESSION_SELECTED);
        for (int i = 0; i < elemList.Count; i++)
        {
          nodeSessione = elemList[i].SelectSingleNode(str);
          if (nodeSessione != null) nodeSessione.Attributes["Selected"].Value = sessionColor;
        }
        // codice aggiunto per impostare i colori di una sessione appena creata - fine

        ScrollForced();
        nodialreadyloader = true;

      }

      //cBusinessObjects.hide_workinprogress();
    }

    private void AggiustaSessioniNodi()
    {
      return;


      XmlNodeList elemList = TreeXmlProvider.Document.GetElementsByTagName("Node");
      for (int i = 0; i < elemList.Count; i++)
      {
        foreach (XmlNode nodeTree in TreeXmlProvider.Document.SelectSingleNode("/Tree/Sessioni"))
        {

          foreach (XmlNode node in elemList[i].SelectNodes("Sessioni"))
          {

          }

          XmlNode nodeSessione = elemList[i].SelectSingleNode("Sessioni");
          if (nodeSessione == null)
          {
            //  / Sessione[@Alias =\"" + nodeTree.Attributes["Alias"].Value + "\"]");
            elemList[i].AppendChild(nodeTree);
          }
        }

      }
      SaveTreeSource(true);


    }



    //----------------------------------------------------------------------------+
    //                              CheckIfAllDates                               |
    //----------------------------------------------------------------------------+
    private bool CheckIfAllDates(ref Hashtable ht, ref Hashtable htID,
ref Hashtable htAliasAdditivo, ref List<DateTime> dates, ref List<string> strings)
    {
      bool alldates = true;
      DateTime data;

            // ELIMINA LE SESSIONI CHE SI RIFERISCONO A VERSIONI DIVERSE DALLA SESSIONE CORRENTE
       
      string area =((App.TipoFile)Enum.Parse(typeof(App.TipoFile), IDTree)).ToString();


      string versione=cBusinessObjects.GetVersioneSessione(IDSessione,area);
      string tmpSessioneFile = SessioneFile;
      string tmpSessioneAlias = SessioneAlias;
      string tmpSessioneSigillo = SessioneSigillo;
      string tmpSessioneSigilloData = SessioneSigilloData;
      string tmpSessioneSigilloPassword = SessioneSigilloPassword;
      string tmpSessioneID = SessioneID;
      string tmpSessioneAliasAdditivo = SessioneAliasAdditivo;
   

      for (int i = 0; i < SessioneFile.Split('|').Count(); i++)
        {
            string tmp_versione=cBusinessObjects.GetVersioneSessione(SessioneID.Split('|')[i].Replace("S1_", "").Replace("S2_", "").Replace("S3_", ""),area);
            if(tmp_versione!=versione)
            {
                 if (SessioneFile.Split('|').Count() > i)
                    if(SessioneFile.Split('|')[i]!="")
                    {
                    tmpSessioneFile=tmpSessioneFile.Replace(SessioneFile.Split('|')[i], "");
                    tmpSessioneFile= tmpSessioneFile.Replace("||", "|").TrimStart('|');
                    }
                 if (SessioneAlias.Split('|').Count() > i)                 
                    if(SessioneAlias.Split('|')[i]!="")
                    {
                    tmpSessioneAlias=tmpSessioneAlias.Replace(SessioneAlias.Split('|')[i], "");
                    tmpSessioneAlias=tmpSessioneAlias.Replace("||", "|").TrimStart('|');
                    }
                 if (SessioneSigillo.Split('|').Count() > i)                 
                    if(SessioneSigillo.Split('|')[i]!="")
                    {
                    tmpSessioneSigillo=tmpSessioneSigillo.Replace(SessioneSigillo.Split('|')[i], "");
                    tmpSessioneSigillo=tmpSessioneSigillo.Replace("||", "|").TrimStart('|');
                    }
                 if (SessioneSigilloData.Split('|').Count() > i)
                    if(SessioneSigilloData.Split('|')[i]!="")
                    {
                    tmpSessioneSigilloData=tmpSessioneSigilloData.Replace(SessioneSigilloData.Split('|')[i], "");
                    tmpSessioneSigilloData=tmpSessioneSigilloData.Replace("||", "|").TrimStart('|');
                    }
                 if (SessioneSigilloPassword.Split('|').Count() > i)
                    if(SessioneSigilloPassword.Split('|')[i]!="")
                    {
                    tmpSessioneSigilloPassword=tmpSessioneSigilloPassword.Replace(SessioneSigilloPassword.Split('|')[i], "");
                    tmpSessioneSigilloPassword=tmpSessioneSigilloPassword.Replace("||", "|").TrimStart('|');
                    }
                 if (SessioneID.Split('|').Count() > i)
                    if(SessioneID.Split('|')[i]!="")
                    {
                    tmpSessioneID=tmpSessioneID.Replace(SessioneID.Split('|')[i], "");
                    tmpSessioneID=tmpSessioneID.Replace("||", "|").TrimStart('|');
                    }
                 if (SessioneAliasAdditivo.Split('|').Count() > i)
                    if(SessioneAliasAdditivo.Split('|')[i]!="")
                    {
                    tmpSessioneAliasAdditivo=tmpSessioneAliasAdditivo.Replace(SessioneAliasAdditivo.Split('|')[i], "");
                    tmpSessioneAliasAdditivo=tmpSessioneAliasAdditivo.Replace("||", "|").TrimStart('|');
                    }

            }
        }

         SessioneFile = tmpSessioneFile.TrimEnd('|');
         SessioneAlias = tmpSessioneAlias.TrimEnd('|');
         SessioneSigillo = tmpSessioneSigillo.TrimEnd('|');
         SessioneSigilloData = tmpSessioneSigilloData.TrimEnd('|');
         SessioneSigilloPassword = tmpSessioneSigilloPassword.TrimEnd('|');
         SessioneID = tmpSessioneID.TrimEnd('|');
         SessioneAliasAdditivo = tmpSessioneAliasAdditivo.TrimEnd('|');
   
      for (int i = 0; i < SessioneFile.Split('|').Count(); i++)
      {
        ht.Add(SessioneAlias.Split('|')[i], SessioneFile.Split('|')[i]);
        if (SessioneSigillo.Split('|').Count() > i)
        {
          htSessioneSigillo.Add(SessioneAlias.Split('|')[i], SessioneSigillo.Split('|')[i]);
        }
        if (SessioneSigilloData.Split('|').Count() > i)
        {
          htSessioneSigilloData.Add(SessioneAlias.Split('|')[i], SessioneSigilloData.Split('|')[i]);
        }
        if (SessioneSigilloPassword.Split('|').Count() > i)
        {
          htSessioneSigilloPassword.Add(SessioneAlias.Split('|')[i], SessioneSigilloPassword.Split('|')[i]);
        }
        htID.Add(SessioneAlias.Split('|')[i], SessioneID.Split('|')[i].Replace("S1_", "").Replace("S2_", "").Replace("S3_", ""));
        if (SessioneAliasAdditivo.Split('|').Count() > i)
        {
          htAliasAdditivo.Add(SessioneAlias.Split('|')[i], SessioneAliasAdditivo.Split('|')[i]);
        }
        else
        {
          htAliasAdditivo.Add(SessioneAlias.Split('|')[i], "");
        }
        string aliastmp = SessioneAlias.Split('|')[i];
        strings.Add(aliastmp);
        if (aliastmp == "")
        {
          aliastmp = "31/12/" + DateTime.Now.Year.ToString();
        }
        if (DateTime.TryParse(aliastmp.Replace("S1_", "").Replace("S2_", "").Replace("S3_", ""), out data))
        {
          if (aliastmp.Contains("S1_"))
          {
            data = data.AddDays(1);
          }
          if (aliastmp.Contains("S2_"))
          {
            data = data.AddDays(2);
          }
          if (aliastmp.Contains("S3_"))
          {
            data = data.AddDays(3);
          }
          dates.Add(data);
        }
        else
        {
          alldates = false;
        }
      }
      return alldates;
    }

    bool GetCompletezzaNodiAlreadyDone = false;

    private void GetCompletezzaNodi(ref bool alldates, ref List<string> strings,
      ref List<DateTime> dates, ref Hashtable ht, ref Hashtable htID,
      ref ArrayList alCheckCompletezzaNodi, ref XmlManager x)
    {
      int daycounter = -1;

      if (GetCompletezzaNodiAlreadyDone == true) return;
      GetCompletezzaNodiAlreadyDone = true;

      for (int i = 0; i < strings.Count; i++)
      {


        string alias;
        if (alldates)
        {
          alias = dates[i].ToShortDateString();
          if (alias == "31/12" + DateTime.Now.Year.ToString())
          {
            alias = "";
          }
        }
        else
        {
          alias = strings[i];
        }
        if (!ht.Contains(alias))
        {
          //compatibilità con MAC
          if (alias.Split('/')[2].Length == 4)
          {
            alias = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2].Substring(2, 2);
          }
        }
        if (!ht.Contains(alias))
        {
          //compatibilità con MAC
          if (alias.Split('/')[2].Length == 2)
          {
            alias = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/20" + alias.Split('/')[2];
          }
        }
        while (!ht.Contains(alias))
        {
          if (daycounter == -1)
          {
            alias = "S1_" + dates[i].AddDays(daycounter).ToShortDateString();
          }
          if (daycounter == -2)
          {
            alias = "S2_" + dates[i].AddDays(daycounter).ToShortDateString();
          }
          if (daycounter == -3)
          {
            alias = "S3_" + dates[i].AddDays(daycounter).ToShortDateString();
            break;
          }
          daycounter -= 1;
        }
        if (!htSessioni.ContainsKey(i))
        { htSessioni.Add(i, ht[alias].ToString()); }
        if (!htSessioniID.ContainsKey(i))
        { htSessioniID.Add(i, htID[alias].ToString()); }

        //    XmlDocument tmpDoc = x.LoadEncodedFile(ht[alias].ToString());
        //   GetTemplateVersioning(ht[alias].ToString());
        XmlNodeList elemList = TreeXmlProvider.Document.GetElementsByTagName("Node");
        for (int iv = 0; iv < elemList.Count; iv++)
        {
          if (elemList[iv].SelectSingleNode("//Sessioni") != null)
          {
            if (!alCheckCompletezzaNodi.Contains(elemList[iv].Attributes["ID"].Value))
            {
              alCheckCompletezzaNodi.Add(elemList[iv].Attributes["ID"].Value);
            }
          }
        }
      }
    }

    bool GetTemplateVersioningAlreadyDone = false;

    //----------------------------------------------------------------------------+
    //                           GetTemplateVersioning                            |
    //----------------------------------------------------------------------------+
    private void GetTemplateVersioning(string fileData)
    {
      if (GetTemplateVersioningAlreadyDone == true) return;
      GetTemplateVersioningAlreadyDone = true;
      // Check su altri template
      ArrayList TemplateVersions = new ArrayList();
      Utilities u = new Utilities();
      XmlDocument doctmp = new XmlDocument();
      //luigi
      //doctmp.Load(App.AppTemplateFolder + "\\TranscodificaTemplate.xml");
      //doctmp.Load(App.AppTemplateFolder + App.IndiceTemplateFileName + u.EstensioneFile(App.TipoFile.IndiceTemplate));
      //apro file XML
      XmlManager x = new XmlManager();
      string tFile = string.Empty;
      App.ErrorLevel = App.ErrorTypes.Nessuno;
      tFile = App.AppTemplateFolder + "\\" + App.IndiceTemplateFileName + u.EstensioneFile(App.TipoFile.IndiceTemplate);
      x.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
      doctmp = x.LoadEncodedFile(tFile);
      foreach (XmlNode item in doctmp.SelectNodes("/TEMPLATES/TEMPLATE"))
      {
        TemplateVersions.Add(item.Attributes["VERSION"].Value);
      }
      XmlManager xTreeParagone = new XmlManager();
      if (fileData == SelectedDataSource)
      {
        if (TreeXmlProvider.Document.SelectSingleNode("/Tree/REVISOFT").Attributes["Template"] == null)
        {
          //ALBERO DI PARTENZA
          XmlNode tmpNodeTreehere = TreeXmlProvider.Document.SelectSingleNode("/Tree").Clone();
          XmlAttribute attr = TreeXmlProvider.Document.CreateAttribute("Template");
          TreeXmlProvider.Document.SelectSingleNode("/Tree/REVISOFT").Attributes.Append(attr);
          TreeXmlProvider.Document.SelectSingleNode("/Tree/REVISOFT").Attributes["Template"].Value = (string)(TemplateVersions[TemplateVersions.Count - 1]);
          App.TipoFile TipoTree = ((App.TipoFile)(Convert.ToInt32(TreeXmlProvider.Document.SelectSingleNode("/Tree/REVISOFT").Attributes["ID"].Value)));
          foreach (XmlNode item in tmpNodeTreehere.SelectNodes("//Sessioni"))
          {
            item.ParentNode.RemoveChild(item);
          }
          foreach (XmlNode item in tmpNodeTreehere.SelectNodes("//Node"))
          {
            if (item.Attributes["Report"] != null)
              item.Attributes.Remove(item.Attributes["Report"]);
            if (item.Attributes["Nota"] != null)
              item.Attributes.Remove(item.Attributes["Nota"]);
            if (item.Attributes["Chiuso"] != null)
              item.Attributes.Remove(item.Attributes["Chiuso"]);
            if (item.Attributes["HighLighted"] != null)
              item.Attributes.Remove(item.Attributes["HighLighted"]);
            if (item.Attributes["Visible"] != null)
              item.Attributes.Remove(item.Attributes["Visible"]);
            if (item.Attributes["Selected"] != null)
              item.Attributes.Remove(item.Attributes["Selected"]);
            if (item.Attributes["MinWidth"] != null)
              item.Attributes.Remove(item.Attributes["MinWidth"]);
            if (item.Attributes["Expanded"] != null)
              item.Attributes.Remove(item.Attributes["Expanded"]);
          }
          //ALBERO DI PARAGONE
          for (int i = 0; i < (TemplateVersions.Count - 1); i++)
          {
            XmlNode templateParagoneNode = null;
            string filepathhere = "";
            FileInfo fihere = null;
            switch (TipoTree)
            {
              case App.TipoFile.Revisione:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRevisione + (new Utilities()).EstensioneFile(App.TipoFile.Revisione);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                break;
              case App.TipoFile.PianificazioniVerifica:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNamePianificazioniVerifica + (new Utilities()).EstensioneFile(App.TipoFile.PianificazioniVerifica);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                break;
              case App.TipoFile.Verifica:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameVerifica + (new Utilities()).EstensioneFile(App.TipoFile.Verifica);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameVerifica + (new Utilities()).EstensioneFile(App.TipoFile.Verifica))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.Incarico:
              case App.TipoFile.IncaricoCS:
              case App.TipoFile.IncaricoSU:
              case App.TipoFile.IncaricoREV:

                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameIncarico + (new Utilities()).EstensioneFile(App.TipoFile.Incarico);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameIncarico + (new Utilities()).EstensioneFile(App.TipoFile.Incarico))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.ISQC:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameISQC + (new Utilities()).EstensioneFile(App.TipoFile.ISQC);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameIncarico + (new Utilities()).EstensioneFile(App.TipoFile.Incarico))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.Bilancio:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameBilancio + (new Utilities()).EstensioneFile(App.TipoFile.Bilancio);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameBilancio + (new Utilities()).EstensioneFile(App.TipoFile.Bilancio))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.PianificazioniVigilanza:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNamePianificazioniVigilanza + (new Utilities()).EstensioneFile(App.TipoFile.PianificazioniVigilanza);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                break;
              case App.TipoFile.Vigilanza:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameVigilanza + (new Utilities()).EstensioneFile(App.TipoFile.Vigilanza);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameVigilanza + (new Utilities()).EstensioneFile(App.TipoFile.Vigilanza))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.Conclusione:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameConclusione + (new Utilities()).EstensioneFile(App.TipoFile.Conclusione);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameConclusione + (new Utilities()).EstensioneFile(App.TipoFile.Conclusione))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.RelazioneB:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneB + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneB);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneB + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneB))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.RelazioneV:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneV + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneV);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneV + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneV))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.RelazioneBC:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneBC + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneBC);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneB + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneB))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.RelazioneVC:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneVC + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneVC);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneV + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneV))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.RelazioneBV:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneBV + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneBV);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneBV + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneBV))).SelectSingleNode("/Tree").Clone();
                break;
              default:
                break;
            }
            if (templateParagoneNode != null)
            {
              foreach (XmlNode item in templateParagoneNode.SelectNodes("//Node"))
              {
                if (item.Attributes["Report"] != null)
                  item.Attributes.Remove(item.Attributes["Report"]);
                if (item.Attributes["Nota"] != null)
                  item.Attributes.Remove(item.Attributes["Nota"]);
                if (item.Attributes["Chiuso"] != null)
                  item.Attributes.Remove(item.Attributes["Chiuso"]);
                if (item.Attributes["HighLighted"] != null)
                  item.Attributes.Remove(item.Attributes["HighLighted"]);
                if (item.Attributes["Visible"] != null)
                  item.Attributes.Remove(item.Attributes["Visible"]);
                if (item.Attributes["Selected"] != null)
                  item.Attributes.Remove(item.Attributes["Selected"]);
                if (item.Attributes["MinWidth"] != null)
                  item.Attributes.Remove(item.Attributes["MinWidth"]);
                if (item.Attributes["Expanded"] != null)
                  item.Attributes.Remove(item.Attributes["Expanded"]);
              }
              bool result = Program.DeepEqualsWithNormalization(XDocument.Parse(tmpNodeTreehere.OuterXml), XDocument.Parse(templateParagoneNode.OuterXml), null);
              //XNode.DeepEquals
              if (result)
              {
                TreeXmlProvider.Document.SelectSingleNode("/Tree/REVISOFT").Attributes["Template"].Value = (string)(TemplateVersions[i]);
                TreeXmlProvider.Refresh();
                break;
              }
            }
          }
        }
      }
      else
      {
        XmlDataProviderManager xTree =
          new XmlDataProviderManager(
            App.AppDataDataFolder + "\\"
            + (new MasterFile()).GetTreeAssociatoFromFileData(fileData), true);
        XmlDocument tmpTree = xTree.Document;
        if (tmpTree.SelectSingleNode("/Tree/REVISOFT").Attributes["Template"] == null)
        {
          //ALBERO DI PARTENZA
          XmlNode tmpNodeTreehere = tmpTree.SelectSingleNode("/Tree").Clone();
          XmlAttribute attr = tmpTree.CreateAttribute("Template");
          tmpTree.SelectSingleNode("/Tree/REVISOFT").Attributes.Append(attr);
          tmpTree.SelectSingleNode("/Tree/REVISOFT").Attributes["Template"].Value = (string)(TemplateVersions[TemplateVersions.Count - 1]);
          App.TipoFile TipoTree = ((App.TipoFile)(Convert.ToInt32(tmpTree.SelectSingleNode("/Tree/REVISOFT").Attributes["ID"].Value)));
          foreach (XmlNode item in tmpNodeTreehere.SelectNodes("//Sessioni"))
          {
            item.ParentNode.RemoveChild(item);
          }
          foreach (XmlNode item in tmpNodeTreehere.SelectNodes("//Node"))
          {
            if (item.Attributes["Report"] != null)
              item.Attributes.Remove(item.Attributes["Report"]);
            if (item.Attributes["Nota"] != null)
              item.Attributes.Remove(item.Attributes["Nota"]);
            if (item.Attributes["Chiuso"] != null)
              item.Attributes.Remove(item.Attributes["Chiuso"]);
            if (item.Attributes["HighLighted"] != null)
              item.Attributes.Remove(item.Attributes["HighLighted"]);
            if (item.Attributes["Visible"] != null)
              item.Attributes.Remove(item.Attributes["Visible"]);
            if (item.Attributes["Selected"] != null)
              item.Attributes.Remove(item.Attributes["Selected"]);
            if (item.Attributes["MinWidth"] != null)
              item.Attributes.Remove(item.Attributes["MinWidth"]);
            if (item.Attributes["Expanded"] != null)
              item.Attributes.Remove(item.Attributes["Expanded"]);
          }
          //ALBERO DI PARAGONE
          for (int i = 0; i < (TemplateVersions.Count - 1); i++)
          {
            XmlNode templateParagoneNode = null;
            string filepathhere = "";
            FileInfo fihere = null;
            switch (TipoTree)
            {
              case App.TipoFile.Revisione:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRevisione + (new Utilities()).EstensioneFile(App.TipoFile.Revisione);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                break;
              case App.TipoFile.PianificazioniVerifica:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNamePianificazioniVerifica + (new Utilities()).EstensioneFile(App.TipoFile.PianificazioniVerifica);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameVerifica + (new Utilities()).EstensioneFile(App.TipoFile.Verifica))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.Verifica:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameVerifica + (new Utilities()).EstensioneFile(App.TipoFile.Verifica);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameVerifica + (new Utilities()).EstensioneFile(App.TipoFile.Verifica))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.Incarico:
              case App.TipoFile.IncaricoCS:
              case App.TipoFile.IncaricoSU:
              case App.TipoFile.IncaricoREV:

                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameIncarico + (new Utilities()).EstensioneFile(App.TipoFile.Incarico);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameIncarico + (new Utilities()).EstensioneFile(App.TipoFile.Incarico))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.ISQC:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameISQC + (new Utilities()).EstensioneFile(App.TipoFile.ISQC);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameIncarico + (new Utilities()).EstensioneFile(App.TipoFile.Incarico))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.Bilancio:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameBilancio + (new Utilities()).EstensioneFile(App.TipoFile.Bilancio);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameBilancio + (new Utilities()).EstensioneFile(App.TipoFile.Bilancio))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.PianificazioniVigilanza:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNamePianificazioniVigilanza + (new Utilities()).EstensioneFile(App.TipoFile.PianificazioniVigilanza);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameVigilanza + (new Utilities()).EstensioneFile(App.TipoFile.Vigilanza))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.Vigilanza:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameVigilanza + (new Utilities()).EstensioneFile(App.TipoFile.Vigilanza);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameVigilanza + (new Utilities()).EstensioneFile(App.TipoFile.Vigilanza))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.Conclusione:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameConclusione + (new Utilities()).EstensioneFile(App.TipoFile.Conclusione);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameConclusione + (new Utilities()).EstensioneFile(App.TipoFile.Conclusione))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.RelazioneB:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneB + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneB);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneB + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneB))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.RelazioneV:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneV + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneV);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneV + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneV))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.RelazioneBC:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneBC + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneBC);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneB + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneB))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.RelazioneVC:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneVC + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneVC);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneV + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneV))).SelectSingleNode("/Tree").Clone();
                break;
              case App.TipoFile.RelazioneBV:
                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneBV + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneBV);
                fihere = new FileInfo(filepathhere);
                if (fihere.Exists)
                {
                  templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                }
                //templateParagoneNode = (xTreeParagone.LoadEncodedFile(App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneBV + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneBV))).SelectSingleNode("/Tree").Clone();
                break;
              default:
                break;
            }
            if (templateParagoneNode != null)
            {
              foreach (XmlNode item in templateParagoneNode.SelectNodes("//Node"))
              {
                if (item.Attributes["Report"] != null)
                  item.Attributes.Remove(item.Attributes["Report"]);
                if (item.Attributes["Nota"] != null)
                  item.Attributes.Remove(item.Attributes["Nota"]);
                if (item.Attributes["Chiuso"] != null)
                  item.Attributes.Remove(item.Attributes["Chiuso"]);
                if (item.Attributes["HighLighted"] != null)
                  item.Attributes.Remove(item.Attributes["HighLighted"]);
                if (item.Attributes["Visible"] != null)
                  item.Attributes.Remove(item.Attributes["Visible"]);
                if (item.Attributes["Selected"] != null)
                  item.Attributes.Remove(item.Attributes["Selected"]);
                if (item.Attributes["MinWidth"] != null)
                  item.Attributes.Remove(item.Attributes["MinWidth"]);
                if (item.Attributes["Expanded"] != null)
                  item.Attributes.Remove(item.Attributes["Expanded"]);
              }
              bool result = Program.DeepEqualsWithNormalization(XDocument.Parse(tmpNodeTreehere.OuterXml), XDocument.Parse(templateParagoneNode.OuterXml), null);
              //XNode.DeepEquals
              if (result)
              {
                tmpTree.SelectSingleNode("/Tree/REVISOFT").Attributes["Template"].Value = (string)(TemplateVersions[i]);
                xTree.Save();
                break;
              }
            }
          }
          xTree.Save();
        }
      }
    }

    //----------------------------------------------------------------------------+
    //                               VerificaStati                                |
    //----------------------------------------------------------------------------+

    private void VerificaStati(ref bool alldates, ref List<string> strings,
      ref List<DateTime> dates, ref Hashtable ht, ref XmlManager x,
      ref Hashtable htAliasAdditivo, ref ArrayList alCheckCompletezzaNodi)
    {
      Hashtable chkNA = new Hashtable();
      int daycounter = -1;
      string aliastocheck = "";
      bool isModified = false; // E.B.

      for (int i = 0; i < strings.Count; i++)
      {

        ((MainWindow)System.Windows.Application.Current.MainWindow).UpdateLayout();
        string alias;
        if (alldates)
        {
          alias = dates[i].ToShortDateString();
          if (alias == "31/12" + DateTime.Now.Year.ToString())
          {
            alias = "";
          }
        }
        else
        {
          alias = strings[i];
        }
        if (!ht.Contains(alias))
        {
          //compatibilità con MAC
          if (alias.Split('/')[2].Length == 4)
          {
            alias = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2].Substring(2, 2);
          }
        }
        if (!ht.Contains(alias))
        {
          //compatibilità con MAC
          if (alias.Split('/')[2].Length == 2)
          {
            alias = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/20" + alias.Split('/')[2];
          }
        }
        while (!ht.Contains(alias))
        {
          if (daycounter == -1)
          {
            alias = "S1_" + dates[i].AddDays(daycounter).ToShortDateString();
          }
          if (daycounter == -2)
          {
            alias = "S2_" + dates[i].AddDays(daycounter).ToShortDateString();
          }
          if (daycounter == -3)
          {
            alias = "S3_" + dates[i].AddDays(daycounter).ToShortDateString();
            break;
          }
          daycounter -= 1;
        }
        switch ((App.TipoFile)(System.Convert.ToInt32(IDTree)))
        {
          case App.TipoFile.Bilancio:
            if (alias.Split('/').Count() > 2)
            {
              MasterFile mf = MasterFile.Create();
              Hashtable hthere = mf.GetBilancioFromFileData(ht[alias].ToString());
              if (hthere != null && hthere["Esercizio"] != null)
              {
                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
              }
              else
              {
                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2]);
              }
            }
            break;
          case App.TipoFile.Conclusione:
            if (alias.Split('/').Count() > 2)
            {
              MasterFile mf = MasterFile.Create();
              Hashtable hthere = mf.GetConclusioneFromFileData(ht[alias].ToString());
              if (hthere != null && hthere["Esercizio"] != null)
              {
                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
              }
              else
              {
                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2]);
              }
            }
            break;
          case App.TipoFile.Revisione:
            if (alias.Split('/').Count() > 2)
            {
              MasterFile mf = MasterFile.Create();
              Hashtable hthere = mf.GetRevisioneFromFileData(ht[alias].ToString());
              if (hthere != null && hthere["Esercizio"] != null)
              {
                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
              }
              else
              {
                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2]);
              }
            }
            break;
          case App.TipoFile.RelazioneB:
            if (alias.Split('/').Count() > 2)
            {
              MasterFile mf = MasterFile.Create();
              Hashtable hthere = mf.GetRelazioneBFromFileData(ht[alias].ToString());
              if (hthere != null && hthere["Esercizio"] != null)
              {
                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
              }
              else
              {
                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2]);
              }
            }
            break;
          case App.TipoFile.RelazioneV:
            if (alias.Split('/').Count() > 2)
            {
              MasterFile mf = MasterFile.Create();
              Hashtable hthere = mf.GetRelazioneVFromFileData(ht[alias].ToString());
              if (hthere != null && hthere["Esercizio"] != null)
              {
                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
              }
              else
              {
                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2]);
              }
            }
            break;
          case App.TipoFile.RelazioneBC:
            if (alias.Split('/').Count() > 2)
            {
              MasterFile mf = MasterFile.Create();
              Hashtable hthere = mf.GetRelazioneBCFromFileData(ht[alias].ToString());
              if (hthere != null && hthere["Esercizio"] != null)
              {
                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
              }
              else
              {
                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2]);
              }
            }
            break;
          case App.TipoFile.RelazioneVC:
            if (alias.Split('/').Count() > 2)
            {
              MasterFile mf = MasterFile.Create();
              Hashtable hthere = mf.GetRelazioneVCFromFileData(ht[alias].ToString());
              if (hthere != null && hthere["Esercizio"] != null)
              {
                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
              }
              else
              {
                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2]);
              }
            }
            break;
          case App.TipoFile.RelazioneBV:
            if (alias.Split('/').Count() > 2)
            {
              MasterFile mf = MasterFile.Create();
              Hashtable hthere = mf.GetRelazioneBVFromFileData(ht[alias].ToString());
              if (hthere != null && hthere["Esercizio"] != null)
              {
                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
              }
              else
              {
                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2]);
              }
            }
            break;
          case App.TipoFile.PianificazioniVigilanza:
          case App.TipoFile.PianificazioniVerifica:
            if (alias.Split('/').Count() > 2 && htAliasAdditivo[alias] != null)
            {
              aliastocheck = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
            }
            break;
          case App.TipoFile.Vigilanza:
          case App.TipoFile.Verifica:
          case App.TipoFile.Incarico:
          case App.TipoFile.IncaricoCS:
          case App.TipoFile.IncaricoSU:
          case App.TipoFile.IncaricoREV:
            if (alias.Split('/').Count() > 2 && htAliasAdditivo[alias] != null)
            {
              aliastocheck = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "\r\n" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
            }
            break;
          case App.TipoFile.ISQC:
            if (alias.Split('/').Count() > 2 && htAliasAdditivo[alias] != null)
            {
              aliastocheck = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
            }
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


        XmlNode nodeTree = TreeXmlProvider.Document.SelectSingleNode("/Tree");
        if (nodeTree != null)
        {
          XmlNode nodeSessioni = nodeTree.SelectSingleNode("Sessioni");
          //PRISC DA CONTROLLARE PER SIGILLO -- inizio
          if (i == 0 && nodeSessioni != null && !alias.Contains("S1") && !alias.Contains("S2") && !alias.Contains("S3"))
          {
            nodeSessioni.ParentNode.RemoveChild(nodeSessioni);
            nodeSessioni = null;
          }
          //PRISC DA CONTROLLARE PER SIGILLO -- fine
          if (nodeSessioni == null)
          {
            nodeSessioni = nodeTree.OwnerDocument.CreateNode(XmlNodeType.Element, "Sessioni", "");
            nodeTree.AppendChild(nodeSessioni);
            nodeSessioni = nodeTree.SelectSingleNode("Sessioni");
            StaticUtilities.MarkNodeAsModified(nodeSessioni, App.OBJ_MOD); isModified = true; // E.B.
          }
          XmlNode nodeSessione = nodeTree.SelectSingleNode("Sessioni/Sessione[@Alias=\"" + aliastocheck + "\"]");
          if (nodeSessione != null)
          {
            switch ((App.TipoFile)(System.Convert.ToInt32(IDTree)))
            {
              case App.TipoFile.Bilancio:
                if (alias.Split('/').Count() > 2)
                {
                  MasterFile mf = MasterFile.Create();
                  Hashtable hthere = mf.GetBilancioFromFileData(ht[alias].ToString());
                  if (hthere != null && hthere["Esercizio"] != null)
                  {
                    htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                  }
                  else
                  {
                    htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                  }
                }
                break;
              case App.TipoFile.Conclusione:
                if (alias.Split('/').Count() > 2)
                {
                  MasterFile mf = MasterFile.Create();
                  Hashtable hthere = mf.GetConclusioneFromFileData(ht[alias].ToString());
                  if (hthere != null && hthere["Esercizio"] != null)
                  {
                    htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                  }
                  else
                  {
                    htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                  }
                }
                break;
              case App.TipoFile.Revisione:
                if (alias.Split('/').Count() > 2)
                {
                  MasterFile mf = MasterFile.Create();
                  Hashtable hthere = mf.GetRevisioneFromFileData(ht[alias].ToString());
                  if (hthere != null && hthere["Esercizio"] != null)
                  {
                    htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                  }
                  else
                  {
                    htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                  }
                }
                break;
              case App.TipoFile.RelazioneB:
                if (alias.Split('/').Count() > 2)
                {
                  MasterFile mf = MasterFile.Create();
                  Hashtable hthere = mf.GetRelazioneBFromFileData(ht[alias].ToString());
                  if (hthere != null && hthere["Esercizio"] != null)
                  {
                    htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                  }
                  else
                  {
                    htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                  }
                }
                break;
              case App.TipoFile.RelazioneV:
                if (alias.Split('/').Count() > 2)
                {
                  MasterFile mf = MasterFile.Create();
                  Hashtable hthere = mf.GetRelazioneVFromFileData(ht[alias].ToString());
                  if (hthere != null && hthere["Esercizio"] != null)
                  {
                    htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                  }
                  else
                  {
                    htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                  }
                }
                break;
              case App.TipoFile.RelazioneBC:
                if (alias.Split('/').Count() > 2)
                {
                  MasterFile mf = MasterFile.Create();
                  Hashtable hthere = mf.GetRelazioneBCFromFileData(ht[alias].ToString());
                  if (hthere != null && hthere["Esercizio"] != null)
                  {
                    htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                  }
                  else
                  {
                    htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                  }
                }
                break;
              case App.TipoFile.RelazioneVC:
                if (alias.Split('/').Count() > 2)
                {
                  MasterFile mf = MasterFile.Create();
                  Hashtable hthere = mf.GetRelazioneVCFromFileData(ht[alias].ToString());
                  if (hthere != null && hthere["Esercizio"] != null)
                  {
                    htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                  }
                  else
                  {
                    htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                  }
                }
                break;
              case App.TipoFile.RelazioneBV:
                if (alias.Split('/').Count() > 2)
                {
                  MasterFile mf = MasterFile.Create();
                  Hashtable hthere = mf.GetRelazioneBVFromFileData(ht[alias].ToString());
                  if (hthere != null && hthere["Esercizio"] != null)
                  {
                    htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                  }
                  else
                  {
                    htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                  }
                }
                break;
              case App.TipoFile.PianificazioniVigilanza:
              case App.TipoFile.PianificazioniVerifica:
              case App.TipoFile.Vigilanza:
              case App.TipoFile.Verifica:
              case App.TipoFile.Incarico:
              case App.TipoFile.IncaricoCS:
              case App.TipoFile.IncaricoSU:
              case App.TipoFile.IncaricoREV:

                htSessioniAlias.Add(i, alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : " - " + htAliasAdditivo[alias].ToString()));
                break;
              case App.TipoFile.ISQC:
                htSessioniAlias.Add(i, alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : " - " + htAliasAdditivo[alias].ToString()));
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
            if (SelectedDataSource == ht[alias].ToString())
            {
              if (nodeSessione.Attributes["Stato"] != null && nodeSessione.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Sigillo)).ToString())
              {
                ReadOnly = true;
              }
              if (nodeSessione.Attributes["Stato"] != null && nodeSessione.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.SigilloRotto)).ToString())
              {
                ReadOnly = false;
              }
              switch ((App.TipoFile)(System.Convert.ToInt32(IDTree)))
              {
                case App.TipoFile.Bilancio:
                  if (alias.Split('/').Count() > 2)
                  {
                    MasterFile mf = MasterFile.Create();
                    Hashtable hthere = mf.GetBilancioFromFileData(ht[alias].ToString());
                    if (hthere != null && hthere["Esercizio"] != null)
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                    }
                    else
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                    }
                  }
                  break;
                case App.TipoFile.Conclusione:
                  if (alias.Split('/').Count() > 2)
                  {
                    MasterFile mf = MasterFile.Create();
                    Hashtable hthere = mf.GetConclusioneFromFileData(ht[alias].ToString());
                    if (hthere != null && hthere["Esercizio"] != null)
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                    }
                    else
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                    }
                  }
                  break;
                case App.TipoFile.Revisione:
                  if (alias.Split('/').Count() > 2)
                  {
                    MasterFile mf = MasterFile.Create();
                    Hashtable hthere = mf.GetRevisioneFromFileData(ht[alias].ToString());
                    if (hthere != null && hthere["Esercizio"] != null)
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                    }
                    else
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                    }
                  }
                  break;
                case App.TipoFile.RelazioneB:
                  if (alias.Split('/').Count() > 2)
                  {
                    MasterFile mf = MasterFile.Create();
                    Hashtable hthere = mf.GetRelazioneBFromFileData(ht[alias].ToString());
                    if (hthere != null && hthere["Esercizio"] != null)
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                    }
                    else
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                    }
                  }
                  break;
                case App.TipoFile.RelazioneV:
                  if (alias.Split('/').Count() > 2)
                  {
                    MasterFile mf = MasterFile.Create();
                    Hashtable hthere = mf.GetRelazioneVFromFileData(ht[alias].ToString());
                    if (hthere != null && hthere["Esercizio"] != null)
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                    }
                    else
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                    }
                  }
                  break;
                case App.TipoFile.RelazioneBC:
                  if (alias.Split('/').Count() > 2)
                  {
                    MasterFile mf = MasterFile.Create();
                    Hashtable hthere = mf.GetRelazioneBCFromFileData(ht[alias].ToString());
                    if (hthere != null && hthere["Esercizio"] != null)
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                    }
                    else
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                    }
                  }
                  break;
                case App.TipoFile.RelazioneVC:
                  if (alias.Split('/').Count() > 2)
                  {
                    MasterFile mf = MasterFile.Create();
                    Hashtable hthere = mf.GetRelazioneVCFromFileData(ht[alias].ToString());
                    if (hthere != null && hthere["Esercizio"] != null)
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                    }
                    else
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                    }
                  }
                  break;
                case App.TipoFile.RelazioneBV:
                  if (alias.Split('/').Count() > 2)
                  {
                    MasterFile mf = MasterFile.Create();
                    Hashtable hthere = mf.GetRelazioneBVFromFileData(ht[alias].ToString());
                    if (hthere != null && hthere["Esercizio"] != null)
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                    }
                    else
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                    }
                  }
                  break;
                case App.TipoFile.PianificazioniVigilanza:
                case App.TipoFile.PianificazioniVerifica:
                  selectedAliasCodificato = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
                  break;
                case App.TipoFile.Vigilanza:
                case App.TipoFile.Verifica:
                case App.TipoFile.Incarico:
                case App.TipoFile.IncaricoCS:
                case App.TipoFile.IncaricoSU:
                case App.TipoFile.IncaricoREV:

                  selectedAliasCodificato = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "\r\n" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
                  break;
                case App.TipoFile.ISQC:
                  selectedAliasCodificato = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
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
              selectedAlias = alias;
            }
          }
          else
          {
            nodeSessione = nodeSessioni.OwnerDocument.CreateNode(XmlNodeType.Element, "Sessione", "");
            XmlAttribute attr = nodeSessioni.OwnerDocument.CreateAttribute("Alias");
            switch ((App.TipoFile)(System.Convert.ToInt32(IDTree)))
            {
              case App.TipoFile.Bilancio:
                if (alias.Split('/').Count() > 2)
                {
                  MasterFile mf = MasterFile.Create();
                  Hashtable hthere = mf.GetBilancioFromFileData(ht[alias].ToString());
                  if (hthere != null && hthere["Esercizio"] != null)
                  {
                    if (alias.Split('/').Count() > 2)
                    {
                      attr.Value = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                      htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                    }
                    else
                    {
                      attr.Value = strings[i];
                    }
                  }
                  else
                  {
                    if (alias.Split('/').Count() > 2)
                    {
                      attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                      htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                    }
                    else
                    {
                      attr.Value = strings[i];
                    }
                  }
                }
                break;
              case App.TipoFile.Conclusione:
                if (alias.Split('/').Count() > 2)
                {
                  MasterFile mf = MasterFile.Create();
                  Hashtable hthere = mf.GetConclusioneFromFileData(ht[alias].ToString());
                  if (hthere != null && hthere["Esercizio"] != null)
                  {
                    if (alias.Split('/').Count() > 2)
                    {
                      attr.Value = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                      htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                    }
                    else
                    {
                      attr.Value = strings[i];
                    }
                  }
                  else
                  {
                    if (alias.Split('/').Count() > 2)
                    {
                      attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                      htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                    }
                    else
                    {
                      attr.Value = strings[i];
                    }
                  }
                }
                break;
              case App.TipoFile.Revisione:
                if (alias.Split('/').Count() > 2)
                {
                  MasterFile mf = MasterFile.Create();
                  Hashtable hthere = mf.GetRevisioneFromFileData(ht[alias].ToString());
                  if (hthere != null && hthere["Esercizio"] != null)
                  {
                    if (alias.Split('/').Count() > 2)
                    {
                      attr.Value = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                      htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                    }
                    else
                    {
                      attr.Value = strings[i];
                    }
                  }
                  else
                  {
                    if (alias.Split('/').Count() > 2)
                    {
                      attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                      htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                    }
                    else
                    {
                      attr.Value = strings[i];
                    }
                  }
                }
                break;
              case App.TipoFile.RelazioneB:
                if (alias.Split('/').Count() > 2)
                {
                  MasterFile mf = MasterFile.Create();
                  Hashtable hthere = mf.GetRelazioneBFromFileData(ht[alias].ToString());
                  if (hthere != null && hthere["Esercizio"] != null)
                  {
                    if (alias.Split('/').Count() > 2)
                    {
                      attr.Value = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                      htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                    }
                    else
                    {
                      attr.Value = strings[i];
                    }
                  }
                  else
                  {
                    if (alias.Split('/').Count() > 2)
                    {
                      attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                      htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                    }
                    else
                    {
                      attr.Value = strings[i];
                    }
                  }
                }
                break;
              case App.TipoFile.RelazioneV:
                if (alias.Split('/').Count() > 2)
                {
                  MasterFile mf = MasterFile.Create();
                  Hashtable hthere = mf.GetRelazioneVFromFileData(ht[alias].ToString());
                  if (hthere != null && hthere["Esercizio"] != null)
                  {
                    if (alias.Split('/').Count() > 2)
                    {
                      attr.Value = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                      htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                    }
                    else
                    {
                      attr.Value = strings[i];
                    }
                  }
                  else
                  {
                    if (alias.Split('/').Count() > 2)
                    {
                      attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                      htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                    }
                    else
                    {
                      attr.Value = strings[i];
                    }
                  }
                }
                break;
              case App.TipoFile.RelazioneBC:
                if (alias.Split('/').Count() > 2)
                {
                  MasterFile mf = MasterFile.Create();
                  Hashtable hthere = mf.GetRelazioneBCFromFileData(ht[alias].ToString());
                  if (hthere != null && hthere["Esercizio"] != null)
                  {
                    if (alias.Split('/').Count() > 2)
                    {
                      attr.Value = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                      htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                    }
                    else
                    {
                      attr.Value = strings[i];
                    }
                  }
                  else
                  {
                    if (alias.Split('/').Count() > 2)
                    {
                      attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                      htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                    }
                    else
                    {
                      attr.Value = strings[i];
                    }
                  }
                }
                break;
              case App.TipoFile.RelazioneVC:
                if (alias.Split('/').Count() > 2)
                {
                  MasterFile mf = MasterFile.Create();
                  Hashtable hthere = mf.GetRelazioneVCFromFileData(ht[alias].ToString());
                  if (hthere != null && hthere["Esercizio"] != null)
                  {
                    if (alias.Split('/').Count() > 2)
                    {
                      attr.Value = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                      htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                    }
                    else
                    {
                      attr.Value = strings[i];
                    }
                  }
                  else
                  {
                    if (alias.Split('/').Count() > 2)
                    {
                      attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                      htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                    }
                    else
                    {
                      attr.Value = strings[i];
                    }
                  }
                }
                break;
              case App.TipoFile.RelazioneBV:
                if (alias.Split('/').Count() > 2)
                {
                  MasterFile mf = MasterFile.Create();
                  Hashtable hthere = mf.GetRelazioneBVFromFileData(ht[alias].ToString());
                  if (hthere != null && hthere["Esercizio"] != null)
                  {
                    if (alias.Split('/').Count() > 2)
                    {
                      attr.Value = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                      htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                    }
                    else
                    {
                      attr.Value = strings[i];
                    }
                  }
                  else
                  {
                    if (alias.Split('/').Count() > 2)
                    {
                      attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                      htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                    }
                    else
                    {
                      attr.Value = strings[i];
                    }
                  }
                }
                break;
              case App.TipoFile.PianificazioniVigilanza:
              case App.TipoFile.PianificazioniVerifica:
                if (alias.Split('/').Count() > 2 && htAliasAdditivo[alias] != null)
                {
                  attr.Value = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
                  if (!htSessioniAlias.ContainsKey(i))
                  {
                    htSessioniAlias.Add(i, alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : " - " + htAliasAdditivo[alias].ToString()));
                  }
                }
                else
                {
                  attr.Value = strings[i];
                }
                break;
              case App.TipoFile.Vigilanza:
              case App.TipoFile.Verifica:
              case App.TipoFile.Incarico:
              case App.TipoFile.IncaricoCS:
              case App.TipoFile.IncaricoSU:
              case App.TipoFile.IncaricoREV:

                if (alias.Split('/').Count() > 2 && htAliasAdditivo[alias] != null)
                {
                  attr.Value = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "\r\n" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
                  if (!htSessioniAlias.ContainsKey(i))
                  {
                    htSessioniAlias.Add(i, alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : " - " + htAliasAdditivo[alias].ToString()));
                  }
                }
                else
                {
                  attr.Value = strings[i];
                }
                break;
              case App.TipoFile.ISQC:
                if (alias.Split('/').Count() > 2 && htAliasAdditivo[alias] != null)
                {
                  attr.Value = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
                  if (!htSessioniAlias.ContainsKey(i))
                  {
                    htSessioniAlias.Add(i, alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : " - " + htAliasAdditivo[alias].ToString()));
                  }
                }
                else
                {
                  attr.Value = strings[i];
                }
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
            nodeSessione.Attributes.Append(attr);
            attr = nodeSessioni.OwnerDocument.CreateAttribute("Selected");
            if (SelectedDataSource == ht[alias].ToString())
            {
              switch ((App.TipoFile)(System.Convert.ToInt32(IDTree)))
              {
                case App.TipoFile.Bilancio:
                  if (alias.Split('/').Count() > 2)
                  {
                    MasterFile mf = MasterFile.Create();
                    Hashtable hthere = mf.GetBilancioFromFileData(ht[alias].ToString());
                    if (hthere != null && hthere["Esercizio"] != null)
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                    }
                    else
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                    }
                  }
                  break;
                case App.TipoFile.Conclusione:
                  if (alias.Split('/').Count() > 2)
                  {
                    MasterFile mf = MasterFile.Create();
                    Hashtable hthere = mf.GetConclusioneFromFileData(ht[alias].ToString());
                    if (hthere != null && hthere["Esercizio"] != null)
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                    }
                    else
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                    }
                  }
                  break;
                case App.TipoFile.Revisione:
                  if (alias.Split('/').Count() > 2)
                  {
                    MasterFile mf = MasterFile.Create();
                    Hashtable hthere = mf.GetRevisioneFromFileData(ht[alias].ToString());
                    if (hthere != null && hthere["Esercizio"] != null)
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                    }
                    else
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                    }
                  }
                  break;
                case App.TipoFile.RelazioneB:
                  if (alias.Split('/').Count() > 2)
                  {
                    MasterFile mf = MasterFile.Create();
                    Hashtable hthere = mf.GetRelazioneBFromFileData(ht[alias].ToString());
                    if (hthere != null && hthere["Esercizio"] != null)
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                    }
                    else
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                    }
                  }
                  break;
                case App.TipoFile.RelazioneV:
                  if (alias.Split('/').Count() > 2)
                  {
                    MasterFile mf = MasterFile.Create();
                    Hashtable hthere = mf.GetRelazioneVFromFileData(ht[alias].ToString());
                    if (hthere != null && hthere["Esercizio"] != null)
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                    }
                    else
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                    }
                  }
                  break;
                case App.TipoFile.RelazioneBC:
                  if (alias.Split('/').Count() > 2)
                  {
                    MasterFile mf = MasterFile.Create();
                    Hashtable hthere = mf.GetRelazioneBCFromFileData(ht[alias].ToString());
                    if (hthere != null && hthere["Esercizio"] != null)
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                    }
                    else
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                    }
                  }
                  break;
                case App.TipoFile.RelazioneVC:
                  if (alias.Split('/').Count() > 2)
                  {
                    MasterFile mf = MasterFile.Create();
                    Hashtable hthere = mf.GetRelazioneVCFromFileData(ht[alias].ToString());
                    if (hthere != null && hthere["Esercizio"] != null)
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                    }
                    else
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                    }
                  }
                  break;
                case App.TipoFile.RelazioneBV:
                  if (alias.Split('/').Count() > 2)
                  {
                    MasterFile mf = MasterFile.Create();
                    Hashtable hthere = mf.GetRelazioneBVFromFileData(ht[alias].ToString());
                    if (hthere != null && hthere["Esercizio"] != null)
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                    }
                    else
                    {
                      selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                    }
                  }
                  break;
                case App.TipoFile.PianificazioniVigilanza:
                case App.TipoFile.PianificazioniVerifica:
                  selectedAliasCodificato = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
                  break;
                case App.TipoFile.Vigilanza:
                case App.TipoFile.Verifica:
                case App.TipoFile.Incarico:
                case App.TipoFile.IncaricoCS:
                case App.TipoFile.IncaricoSU:
                case App.TipoFile.IncaricoREV:
                  selectedAliasCodificato = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "\r\n" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
                  break;
                case App.TipoFile.ISQC:
                  selectedAliasCodificato = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
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
              selectedAlias = alias;
              attr.Value = "#AA" + YearColor[-1].ToString();
            }
            else
            {
              attr.Value = "White";
            }
            nodeSessione.Attributes.Append(attr);
            nodeSessioni.AppendChild(nodeSessione);
            StaticUtilities.MarkNodeAsModified(nodeSessioni, App.OBJ_MOD); isModified = true; // E.B.
          }
        }

        ArrayList alCheckCompletezzaNodiNOMORE = new ArrayList();
        foreach (string ID in alCheckCompletezzaNodi)
        {
          nodeTree = TreeXmlProvider.Document.SelectSingleNode("/Tree//Node[@ID=" + ID + "]");
          if (nodeTree == null)
          {
            if (!alCheckCompletezzaNodiNOMORE.Contains(ID))
            {
              alCheckCompletezzaNodiNOMORE.Add(ID);
            }
          }
          else
          {

            XmlNode nodeSessioni = nodeTree.SelectSingleNode("Sessioni");
            //PRISC DA CONTROLLARE PER SIGILLO -- inizio
            if (i == 0 && nodeSessioni != null && !alias.Contains("S1") && !alias.Contains("S2") && !alias.Contains("S3"))
            {
              nodeSessioni.ParentNode.RemoveChild(nodeSessioni);
              nodeSessioni = null;
            }
            //PRISC DA CONTROLLARE PER SIGILLO -- fine
            if (nodeSessioni == null)
            {
              XmlNode newElemOut = nodeTree.OwnerDocument.CreateNode(XmlNodeType.Element, "Sessioni", "");
              nodeTree.AppendChild(newElemOut);
              nodeSessioni = nodeTree.SelectSingleNode("Sessioni");
            }
            XmlNode nodeSessione = nodeTree.SelectSingleNode("Sessioni/Sessione[@Alias=\"" + aliastocheck + "\"]");
            if (nodeSessione == null)
            {
              nodeSessione = nodeSessioni.OwnerDocument.CreateNode(XmlNodeType.Element, "Sessione", "");
              XmlAttribute attr = nodeSessioni.OwnerDocument.CreateAttribute("Alias");
              if (aliastocheck != "")
              {
                attr.Value = aliastocheck;
              }
              else
              {
                attr.Value = strings[i];
              }
              nodeSessione.Attributes.Append(attr);
              attr = nodeSessioni.OwnerDocument.CreateAttribute("Stato");
              if (nodeTree != null && nodeTree.ParentNode != null && nodeTree.ParentNode.Name == "Tree")
              {
                if (attr.Value == (Convert.ToInt32(App.TipoTreeNodeStato.Sigillo)).ToString()
                  || attr.Value == (Convert.ToInt32(App.TipoTreeNodeStato.SigilloRotto)).ToString())
                {
                  ;
                }
                else
                {
                  if (SelectedDataSource == ht[alias].ToString()
                    && nodeTree.Attributes["Osservazioni"] != null
                    && nodeTree.Attributes["Osservazioni"].Value.Trim() != "")
                  {
                    attr.Value = (Convert.ToInt32(App.TipoTreeNodeStato.NodoFazzoletto)).ToString();
                  }
                  else
                  {
                    attr.Value = (Convert.ToInt32(App.TipoTreeNodeStato.NodoFazzoletto)).ToString();
                  }
                }
              }
              else
              {
                string ids = "-1";
                foreach (DictionaryEntry s in htSessioniAlias)
                {

                  if (aliastocheck.Replace("\r\n", "/") == s.Value.ToString().Replace(" - ", "/"))
                  {
                    ids = htSessioniID[s.Key].ToString();

                  }
                }

                attr.Value = getStato(nodeTree, ids);



                if (chkNA.Contains(ID) && i == 1 && attr.Value == (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile)).ToString())
                {
                  TreeXmlProvider.Document.SelectSingleNode(chkNA[ID].ToString()).Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile)).ToString();
                  XmlNode xtbdh = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + ID + "']");
                  if (xtbdh.Attributes["Stato"] == null)
                  {
                    XmlAttribute attrh = _x.Document.CreateAttribute("Stato");
                    xtbdh.Attributes.Append(attrh);
                  }
                  xtbdh.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile)).ToString();
                }
                else
                {
                  if (nodeTree.Attributes["Report"].Value == "True")
                  {
                    attr.Value = (Convert.ToInt32(App.TipoTreeNodeStato.SolaLettura)).ToString();
                  }
                }
              }
              nodeSessione.Attributes.Append(attr);
              if (i == 0 && attr.Value == "-1")
              {
                if (!chkNA.ContainsKey(ID))
                {
                  chkNA.Add(ID, "/Tree//Node[@ID=" + ID + "]/Sessioni/Sessione[@Alias=\"" + aliastocheck + "\"]");
                }
              }
              nodeSessioni.AppendChild(nodeSessione);
            }
            nodeSessione = nodeTree.SelectSingleNode("Sessioni/Sessione[@Alias=\"" + aliastocheck + "\"]");
            DataTable pianificazione = null;
            DataTable pianificazioneTestata = null;
            if (IDTree == "2" || IDTree == "18")
            {
              foreach (string ALitemXTPP in ALXTPP)
              {
                bool donehere = false;
                string IDPHERE = "";
                if (IDTree == "2")
                {
                  IDPHERE = "100013";
                  pianificazioneTestata = cBusinessObjects.GetData(int.Parse(IDPHERE), typeof(PianificazioneVerificheTestata), cBusinessObjects.idcliente, int.Parse(ALitemXTPP), 26);

                  pianificazione = cBusinessObjects.GetData(int.Parse(IDPHERE), typeof(PianificazioneVerifiche), cBusinessObjects.idcliente, int.Parse(ALitemXTPP), 26);
                }
                else
                {
                  IDPHERE = "100003";
                  pianificazioneTestata = cBusinessObjects.GetData(int.Parse(IDPHERE), typeof(PianificazioneVerificheTestata), cBusinessObjects.idcliente, int.Parse(ALitemXTPP), 27);

                  pianificazione = cBusinessObjects.GetData(int.Parse(IDPHERE), typeof(PianificazioneVerifiche), cBusinessObjects.idcliente, int.Parse(ALitemXTPP), 27);
                }
                string datac = "";
                foreach (DataRow itemXPP in pianificazione.Rows)
                {

                  if (itemXPP["NODE_ID"].ToString() != ID)
                    continue;
                  if (itemXPP["PianificazioneID"].ToString() == "0")
                    continue;

                  foreach (DataRow dd in pianificazioneTestata.Rows)
                  {
                    if (dd["ID"].ToString() == itemXPP["PianificazioneID"].ToString())
                      datac = dd["Data"].ToString();
                  }
                  if (nodeSessione.Attributes["Pianificato"] == null)
                  {
                    XmlAttribute attr = nodeSessione.OwnerDocument.CreateAttribute("Pianificato");
                    attr.Value = "";
                    nodeSessione.Attributes.Append(attr);
                  }
                  if (nodeSessione.Attributes["Alias"].Value.Replace("\r\n", "/") == datac && itemXPP["Checked"].ToString() == "True")
                  {
                    nodeSessione.Attributes["Pianificato"].Value = "P";
                    XmlNode AppoggioNode = nodeTree.ParentNode;
                    while (AppoggioNode != null && AppoggioNode.ParentNode.Name != "Tree")
                    {
                      XmlNode AppoggioNodeHere = AppoggioNode.SelectSingleNode("Sessioni/Sessione[@Alias=\"" + aliastocheck + "\"]");
                      if (AppoggioNodeHere != null)
                      {
                        if (AppoggioNodeHere.Attributes["Pianificato"] == null)
                        {
                          XmlAttribute attr = AppoggioNodeHere.OwnerDocument.CreateAttribute("Pianificato");
                          attr.Value = "";
                          AppoggioNodeHere.Attributes.Append(attr);
                        }
                        AppoggioNodeHere.Attributes["Pianificato"].Value = "P";
                        AppoggioNode = AppoggioNode.ParentNode;
                      }
                      else
                      {
                        AppoggioNode = null;
                      }
                    }
                    donehere = true;
                    break;
                  }
                  else
                  {
                    nodeSessione.Attributes["Pianificato"].Value = "";
                  }
                }
                if (donehere)
                {
                  break;
                }
              }
            }
            if (nodeSessione.Attributes["Selected"] == null)
            {
              XmlAttribute attr2 = nodeSessioni.OwnerDocument.CreateAttribute("Selected");
              nodeSessione.Attributes.Append(attr2);
            }
            if (SelectedDataSource == ht[alias].ToString())
            {
              nodeSessione.Attributes["Selected"].Value = "#AA" + YearColor[-1].ToString();
            }
            else
            {
              int anno = Convert.ToInt32(alias.Substring(alias.Length - 4, 4));
              if (i % 2 == 0)
              {
                nodeSessione.Attributes["Selected"].Value = "#80" + YearColor[anno].ToString();
              }
              else
              {
                nodeSessione.Attributes["Selected"].Value = "#AA" + YearColor[anno].ToString();
              }
            }
            StaticUtilities.MarkNodeAsModified(nodeSessione, App.OBJ_MOD); isModified = true;
          }
        }
        nodeTree = TreeXmlProvider.Document.SelectSingleNode("/Tree/Node/Sessioni/Sessione[@Alias=\"" + aliastocheck + "\"]");
        if (nodeTree != null && htSessioneSigillo[alias] != null && htSessioneSigillo[alias].ToString() != "")
        {
          if (nodeTree.Attributes["Stato"] == null)
          {
            XmlAttribute attrnew = nodeTree.OwnerDocument.CreateAttribute("Stato");
            nodeTree.Attributes.Append(attrnew);
          }
          nodeTree.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.Sigillo)).ToString();
          if (nodeTree.Attributes["ToolTip"] == null)
          {
            XmlAttribute attrnew = nodeTree.OwnerDocument.CreateAttribute("ToolTip");
            nodeTree.Attributes.Append(attrnew);
          }
          nodeTree.Attributes["ToolTip"].Value = "Applicato da " + htSessioneSigillo[alias].ToString() + " il " + htSessioneSigilloData[alias].ToString();
          if (nodeTree.Attributes["Revisore"] == null)
          {
            XmlAttribute attrnew = nodeTree.OwnerDocument.CreateAttribute("Revisore");
            nodeTree.Attributes.Append(attrnew);
          }
          nodeTree.Attributes["Revisore"].Value = htSessioneSigillo[alias].ToString();
          if (nodeTree.Attributes["Password"] == null)
          {
            XmlAttribute attrnew = nodeTree.OwnerDocument.CreateAttribute("Password");
            nodeTree.Attributes.Append(attrnew);
          }
          nodeTree.Attributes["Password"].Value = htSessioneSigilloPassword[alias].ToString();
          StaticUtilities.MarkNodeAsModified(nodeTree, App.OBJ_MOD); isModified = true; // E.B.
        }
        foreach (string itemTOBEDELETED in alCheckCompletezzaNodiNOMORE)
        {
          alCheckCompletezzaNodi.Remove(itemTOBEDELETED);
        }
        _x.isModified = isModified;

        _x.Save();
      }
      //SaveTreeSource();


      SaveTreeSource(isModified);
    }

    //----------------------------------------------------------------------------+
    //                                 ReloadNodi                                 |
    //----------------------------------------------------------------------------+
    private void ReloadNodi()
    {
      Hashtable ht = new Hashtable();
      Hashtable htID = new Hashtable();
      Hashtable htAliasAdditivo = new Hashtable();
      ArrayList alCheckCompletezzaNodi = new ArrayList();

      List<DateTime> dates = new List<DateTime>();
      List<string> strings = new List<string>();
      bool alldates = true;



      htSessioni.Clear();
      htSessioniAlias.Clear();
      htSessioniID.Clear();
      htSessioneSigillo.Clear();
      htSessioneSigilloData.Clear();
      htSessioneSigilloPassword.Clear();
      RevisoftApplication.XmlManager x = new XmlManager();
      x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;



      alldates = CheckIfAllDates(ref ht, ref htID, ref htAliasAdditivo, ref dates, ref strings);
      if (alldates)
      {
        dates.Sort();
        dates.Reverse();
      }

      //if (CheckCompleto == true)
      {
        GetCompletezzaNodi(ref alldates, ref strings, ref dates, ref ht, ref htID, ref alCheckCompletezzaNodi, ref x);
      }


      VerificaStati(ref alldates, ref strings, ref dates, ref ht, ref x, ref htAliasAdditivo, ref alCheckCompletezzaNodi);
    }

    //----------------------------------------------------------------------------+
    //                            ReloadStatoNodiPadre                            |
    //----------------------------------------------------------------------------+
    private void ReloadStatoNodiPadre()
    {
            
      foreach (XmlNode nodeTree in TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"))
      {
        if (nodeTree.ChildNodes.Count > 1 && nodeTree.Name == "Node")//nodeTree != null)
        {
          XmlNode nodeSessione = nodeTree.SelectSingleNode("Sessioni/Sessione[@Selected='#AA82BDE4']");

          string ids = "-1";
          if (nodeSessione != null)
          {
            foreach (DictionaryEntry s in htSessioniAlias)
            {
              if (nodeSessione.Attributes["Alias"].Value.Replace("\r\n", "/") == s.Value.ToString().Replace(" - ", "/"))
              {
                ids = htSessioniID[s.Key].ToString();

              }
            }
          }


          if (nodeTree.ParentNode.Name == "Tree")
          {
            if (nodeSessione.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Sigillo)).ToString() || nodeSessione.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.SigilloRotto)).ToString())
            {
              ;
            }
            else
            {
              if (nodeTree.Attributes["Osservazioni"] != null && nodeTree.Attributes["Osservazioni"].Value.Trim() != "")
              {
                if (nodeSessione != null && nodeSessione.Attributes["Stato"] != null)
                {
                  nodeSessione.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.NodoFazzoletto)).ToString();
                }
              }
              else
              {
                if (nodeSessione != null && nodeSessione.Attributes["Stato"] != null)
                {
                  nodeSessione.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.NodoFazzoletto)).ToString();
                }
              }
            }
          }
          else
          {
            if (nodeSessione != null && nodeSessione.Attributes["Stato"] != null)
            {
              foreach (XmlNode nodeTreeSecondoLivello in nodeTree.ChildNodes)
              {
                if (nodeTreeSecondoLivello.ChildNodes.Count > 1 && nodeTreeSecondoLivello.Name == "Node")
                {
                  XmlNode nodeSessioneSecondoLivello = nodeTreeSecondoLivello.SelectSingleNode("Sessioni/Sessione[@Selected='#AA82BDE4']");
                  if (nodeSessioneSecondoLivello != null && nodeSessioneSecondoLivello.Attributes["Stato"] != null)
                  {
                    nodeSessioneSecondoLivello.Attributes["Stato"].Value = getStato(nodeTreeSecondoLivello, ids);
                  }
                }
              }
              nodeSessione.Attributes["Stato"].Value = getStato(nodeTree, ids);
            }
          }
        }
      }
      _x.Save();
    }

    //----------------------------------------------------------------------------+
    //                                  getStato                                  |
    //----------------------------------------------------------------------------+
    private string getStato(XmlNode nodeTree, string ids = "-1")
    {
      string returnvalue = "";
      string statotmp = (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString();

      if (nodeTree.ChildNodes.Count > 1 && !(nodeTree.Attributes["Tipologia"].Value == "Nodo Multiplo") && !(nodeTree.Attributes["Report"].Value == "True"))
      {
        foreach (XmlNode nodesStati in nodeTree.ChildNodes)
        {
          if (nodesStati.Name == "Node")
          {
            if (returnvalue != (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString()
              // ANDREA && returnvalue != (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString()
              )
            {
              statotmp = getStato(nodesStati, ids);
              if (statotmp == (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString())
              {
                returnvalue = statotmp;
              }
              else
              {
                if (statotmp == (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString())
                {
                  if (returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString() || returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.VociCompilate)).ToString())
                  {
                    returnvalue = (Convert.ToInt32(App.TipoTreeNodeStato.VociCompilate)).ToString();
                  }
                  else
                  {
                    returnvalue = statotmp;
                  }
                }
                else
                {
                  if ((statotmp == (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile)).ToString() || statotmp == (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabileBucoTemplate)).ToString()) && ((returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile)).ToString() || returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabileBucoTemplate)).ToString()) || returnvalue == ""))
                  {
                    returnvalue = (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString();
                  }
                  if (statotmp == (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString() || statotmp == (Convert.ToInt32(App.TipoTreeNodeStato.VociCompilate)).ToString())
                  {
                    if (returnvalue == "")
                    {
                      returnvalue = statotmp;
                    }
                    else
                    {
                      if (returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.VociCompilate)).ToString() || returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString())
                      {
                        returnvalue = (Convert.ToInt32(App.TipoTreeNodeStato.VociCompilate)).ToString();
                      }
                    }
                  }
                }
              }
            }
            else
            {
              break;
            }
          }
        }
      }
      else
      {
        if (nodeTree.Attributes["Report"].Value == "True")
        {
          returnvalue = (Convert.ToInt32(App.TipoTreeNodeStato.Report)).ToString();
        }
        else
        {
          cBusinessObjects.idcliente = int.Parse(IDCliente);
          DataTable dati = null;
          if (ids != "-1")
            dati = cBusinessObjects.GetData(int.Parse(nodeTree.Attributes["ID"].Value), typeof(StatoNodi), cBusinessObjects.idcliente, int.Parse(ids));
          else
            dati = cBusinessObjects.GetData(int.Parse(nodeTree.Attributes["ID"].Value), typeof(StatoNodi), cBusinessObjects.idcliente, cBusinessObjects.idsessione);

          if (dati.Rows.Count == 0)
          {
            /*
            //MM TO DO DA METTERE A POSTO E DECOMMENTARE SI DOVREBBE TRATTARE DEL BILANCIO RICLASSIFICATO
            if (nodeTree.Attributes["ID"].Value == "138")
            {
              node = tmpDoc.SelectSingleNode("/Dati//Dato[@ID='84']");
            }
            if (nodeTree.Attributes["ID"].Value == "84")
            {
              node = tmpDoc.SelectSingleNode("/Dati//Dato[@ID='138']");
            }
            */
          }
          if (dati.Rows.Count > 0)
          {
            foreach (DataRow dt in dati.Rows)
            {
              if (dt["Stato"].ToString() != "")
              {
                returnvalue = dt["Stato"].ToString().Trim(' ');
              }
              else
              {
                returnvalue = (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString();
              }
            }
          }
          else
          {
            //returnvalue = "-1";
            returnvalue = (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString();
          }
        }
      }
      return returnvalue;
    }

    #endregion //--------------------------------------------------- DataDataSource

    //----------------------------------------------------------------------------+
    //                          Tree_SelectedItemChanged                          |
    //----------------------------------------------------------------------------+
    private void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      ;
    }

    //----------------------------------------------------------------------------+
    //                         searchTextBox_TextChanged                          |
    //----------------------------------------------------------------------------+
    private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      string SearchFor = ((TextBox)sender).Text.ToUpper();
      //int foundID = -1;
      bool found = false;

      if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
      {
        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
        {
          if (item.Attributes["Selected"] != null)
          {
            //if (item.Attributes["Selected"].Value == "True")
            //{
            //    foundID = Convert.ToInt32(item.Attributes["ID"].Value);
            //}
            item.Attributes["Selected"].Value = "False";
          }
          if (item.Attributes["HighLighted"] != null)
          {
            item.Attributes["HighLighted"].Value = "Black";
          }
        }
        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
        {
          if (
            //found == false && foundID != Convert.ToInt32(item.Attributes["ID"].Value) &&
            (item.Attributes["Titolo"].Value.ToUpper().Contains(SearchFor)
              || item.Attributes["Codice"].Value.ToUpper().Contains(SearchFor)))
          {
            found = true;
            item.Attributes["HighLighted"].Value = "Red";
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

    //----------------------------------------------------------------------------+
    //                      ItemsControl_MouseLeftButtonDown                      |
    //----------------------------------------------------------------------------+
    private void ItemsControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      ;
    }

    #region RICERCA TESTO

    //----------------------------------------------------------------------------+
    //                            searchTextBox_KeyUp                             |
    //----------------------------------------------------------------------------+
    private void searchTextBox_KeyUp()
    {
      string SearchFor = searchTextBox.Text.Trim().ToUpper();
      bool found = false;

      //if (e.Key == Key.Enter || e.Key == Key.Tab)
      //{
      //int foundID = -1;
      if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
      {
        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
        {
          if (item.Attributes["Selected"] != null)
          {
            //if (item.Attributes["Selected"].Value == "True")
            //{
            //    foundID = Convert.ToInt32(item.Attributes["ID"].Value);
            //}
            item.Attributes["Selected"].Value = "False";
          }
          if (item.Attributes["Expanded"] != null)
          {
            if (item.ParentNode.Name == "Tree")
            {
              item.Attributes["Expanded"].Value = "True";
            }
            else
            {
              item.Attributes["Expanded"].Value = "False";
            }
          }
          if (item.Attributes["HighLighted"] != null)
          {
            item.Attributes["HighLighted"].Value = "Black";
          }
        }
        if (SearchFor == "")
        {
          return;
        }
        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
        {
          if (
            //found == false && foundID != Convert.ToInt32(item.Attributes["ID"].Value) &&
            (item.Attributes["Titolo"].Value.ToUpper().Contains(SearchFor)
            || item.Attributes["Codice"].Value.ToUpper().Contains(SearchFor)))
          {
            found = true;
            item.Attributes["HighLighted"].Value = "Red";
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
      //}
    }

    //----------------------------------------------------------------------------+
    //                             buttonCerca_Click                              |
    //----------------------------------------------------------------------------+
    private void buttonCerca_Click(object sender, RoutedEventArgs e)
    {
      searchTextBox_KeyUp();
    }

    //----------------------------------------------------------------------------+
    //                          buttonCercaAnnulla_Click                          |
    //----------------------------------------------------------------------------+
    private void buttonCercaAnnulla_Click(object sender, RoutedEventArgs e)
    {
      searchTextBox.Text = "";
      searchTextBox_KeyUp();
    }

    #endregion //---------------------------------------------------- RICERCA TESTO


    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
#if (!DBG_TEST)
      Window_Closing_old(sender,e);return;
#endif
      if (_x != null)
      {
        _x.isModified = m_isModified;
        _x.Save();
      }
      SaveTreeSourceNoReload(m_isModified);
    }

    Brush ButtonStatoSelectedColor = new SolidColorBrush(Color.FromArgb(255, 247, 168, 39));
    Color ButtonToolBarSelectedColor = Color.FromArgb(126, 130, 189, 228);
    Color ButtonToolBarPulseColor = Color.FromArgb(126, 82, 101, 115);

    //----------------------------------------------------------------------------+
    //                           AnimateBackgroundColor                           |
    //----------------------------------------------------------------------------+
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

    //----------------------------------------------------------------------------+
    //                           TreeViewItem_Selected                            |
    //----------------------------------------------------------------------------+
    private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
    {
      try
      {
        if (((XmlNode)(tvMain.SelectedItem)).ChildNodes.Count > 1 && !(((XmlNode)(tvMain.SelectedItem)).Attributes["Tipologia"].Value == "Nodo Multiplo"))
        {
          btn_NonApplicabile.IsEnabled = false;
        }
        else
        {
          btn_NonApplicabile.IsEnabled = true;
          //try
          //{
          XmlNode node = ((XmlNode)(tvMain.SelectedItem));
          string nota = node.Attributes["Nota"].Value;
          if (nota != "" && nota != "<P align=left>&nbsp;</P>" && nota != "<P class=MsoNormal style=\"MARGIN: 0cm 0cm 0pt; LINE-HEIGHT: normal\" align=left>&nbsp;</P>")
          {
            AnimateBackgroundColor(btn_GuidaRevisoft, ButtonToolBarSelectedColor, ButtonToolBarPulseColor, 1);
          }
          else
          {
            btn_GuidaRevisoft.Background = btn_ArchivioAllegati.Background;
          }
          //}
          //catch (Exception ex)
          //{
          //    string log = ex.Message;
          //}
        }
        btn_CopiaLibroSociale.Visibility = System.Windows.Visibility.Collapsed;
        string IDHere = ((XmlNode)(tvMain.SelectedItem)).Attributes["ID"].Value;
        if ((IDTree == "2" && (IDHere == "20" || IDHere == "21" || IDHere == "22" || IDHere == "23" || IDHere == "24" || IDHere == "146")) || (IDTree == "18" && (IDHere == "600" || IDHere == "601" || IDHere == "602" || IDHere == "603" || IDHere == "604" || IDHere == "605")))
        {
          btn_CopiaLibroSociale.Visibility = System.Windows.Visibility.Visible;
        }
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wWorkAreaTree.TreeViewItem_Selected exception");
        string log = ex.Message;
      }
    }


    //----------------------------------------------------------------------------+
    //                           OnItemMouseDoubleClick                           |
    //----------------------------------------------------------------------------+
    private void OnItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
#if (!DBG_TEST)
      OnItemMouseDoubleClick_old(sender, e);return;
#endif
      XmlNode nodeSessione = null;
      //XmlNode removable = null;
      //XmlNode imported = null;
      XmlNode node1 = null;
      XmlNode node;

      if (e.ClickCount != 2) return;

      e.Handled = true;


      try { node = ((XmlNode)(tvMain.SelectedItem)); }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wWorkAreaTree.OnItemMouseDoubleClick1 exception");
        string log = ex.Message;
        e.Handled = true;
        return;
      }
      if (node == null)
      {
        e.Handled = true;
        return;
      }
      if (node.ParentNode == null)
      {
        e.Handled = true;
        return;
      }

      string res, str, itemGuid;
      bool ok;
      SqlParameter retPar;

      itemGuid = SelectedTreeSource.Split('\\').Last();
      using (System.Data.SqlClient.SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("dbo.CheckItemLock", conn);
        cmd.Parameters.AddWithValue("@itemGuid", itemGuid);
        //cmd.Parameters.AddWithValue("@userGuid", Environment.UserName);
        cmd.Parameters.AddWithValue("@userGuid", (App.AppTipo == App.ModalitaApp.Team) ? App.AppUtente.Login : Environment.UserName);
        cmd.Parameters.AddWithValue("@codice", node.Attributes["Codice"].Value);
        retPar = new SqlParameter("@res", SqlDbType.VarChar, 50);
        retPar.Direction = ParameterDirection.Output;
        cmd.Parameters.Add(retPar);
        cmd.CommandType = CommandType.StoredProcedure;
        res = ""; ok = true;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wWorkAreaTree.OnItemMouseDoubleClick2 exception");
          ok = false;
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "SQL call 'dbo.CheckItemLock' failed: errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      if (ok) res = retPar.SqlValue.ToString();
      str = (App.AppTipo == App.ModalitaApp.Team) ? App.AppUtente.Login : Environment.UserName;
      if (res != "" && res != str)
      {
        if (App.m_xmlCache.Contains(itemGuid)) App.m_xmlCache.Remove(itemGuid);
        str = SelectedDataSource.Split('\\').Last();
        if (App.m_xmlCache.Contains(str)) App.m_xmlCache.Remove(str);
        str = string.Format("La carta di lavoro {0} è attualmente in uso dall'utente: \"{1}\"",
          node.Attributes["Codice"].Value, res);
        MessageBox.Show(str, "ATTENZIONE", MessageBoxButton.OK, MessageBoxImage.Stop);
        e.Handled = true;
        return;

      }

      StaticUtilities.SetLockStatus(SelectedTreeSource, node.Attributes["Codice"].Value, true);

      if (node.ParentNode.Name == "Tree")
      {
        XmlNode nodeTree = TreeXmlProvider.Document.SelectSingleNode("/Tree/Node");
        nodeSessione = nodeTree.SelectSingleNode("Sessioni/Sessione[@Selected='#AA82BDE4']");
        XmlNode nodeTreePadre = TreeXmlProvider.Document.SelectSingleNode("/Tree");
        XmlNode nodeTreeSessione = nodeTreePadre.SelectSingleNode("Sessioni/Sessione[@Selected='#AA82BDE4']");
        if (nodeSessione.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Sigillo)).ToString())
        {
          wSigilloSbloccoBlocco sb = new wSigilloSbloccoBlocco();
          sb.Titolo = "Sblocca Sigillo";
          sb.Nodo = nodeSessione;
          sb.NodoTree = nodeTreeSessione;
          sb.IDCliente = IDCliente;
          MasterFile mf = MasterFile.Create();
          Hashtable hthere = mf.GetBilancioFromFileData(SessioneFile);
          if (hthere != null && hthere["Esercizio"] != null)
          {
            sb.AliasSessione = ConvertDataToEsercizio(selectedAlias, hthere);
          }
          else
          {
            sb.AliasSessione = ConvertDataToEsercizio(selectedAlias);
          }
          sb.Owner = this;
          sb.Load();
          sb.ShowDialog();
        }
        else
        {
          NodoFazzoletto o = new NodoFazzoletto();
          o.Owner = this;
          //MM   o.ApertoInSolaLettura = ApertoInSolaLettura;
          o.ReadOnly = ReadOnly;
          o.ApertoInSolaLettura = false;
          //   o.ReadOnly = ReadOnly;
          o.Nodo = node.Attributes["ID"].Value;
          o.Load(IDCliente);
          o.ShowDialog();
          if (nodeTree.Attributes["Osservazioni"] != null && nodeTree.Attributes["Osservazioni"].Value.Trim() != "")
          {
            if (nodeSessione.Attributes["Stato"] != null)
            {
              nodeSessione.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.NodoFazzoletto)).ToString();
              StaticUtilities.MarkNodeAsModified(nodeTree, App.OBJ_MOD);
              StaticUtilities.MarkNodeAsModified(nodeSessione, App.OBJ_MOD);
              m_isModified = true;
            }
          }
          else
          {
            if (nodeSessione.Attributes["Stato"] != null)
            {
              nodeSessione.Attributes["Stato"].Value =
              (Convert.ToInt32(App.TipoTreeNodeStato.NodoFazzoletto)).ToString();
              StaticUtilities.MarkNodeAsModified(nodeSessione, App.OBJ_MOD); m_isModified = true;
            }
          }
        }
        SaveTreeSource(m_isModified);
        e.Handled = true;
        StaticUtilities.SetLockStatus(SelectedTreeSource, node.Attributes["Codice"].Value, false);
        return;
      }
      //  try
      //  {
      if (node.Attributes["Titolo"].Value.Contains("Utilizzata sino a ver. 4.1"))
      {
        e.Handled = true;
        StaticUtilities.SetLockStatus(SelectedTreeSource, node.Attributes["Codice"].Value, false);
        return;
      }
      if (node.Attributes["ID"].Value == "278" && IDTree == "4")
      {
        wCampionamento wcnn = new wCampionamento(
          node.SelectSingleNode("Sessioni/Sessione[@Alias=\"" + selectedAliasCodificato + "\"]"),
          IDCliente, Cliente, Esercizio, IDSessione, IDTree);
        wcnn.ShowDialog();
        RevisoftApplication.XmlManager xx = new XmlManager();
        xx.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
        TreeXmlProvider.Document = cBusinessObjects.NewLoadEncodedFile(SelectedTreeSource,IDTree);
        TreeXmlProvider.Refresh();
        XmlNode nodehere = TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=" + wcnn.changedID + "]");
        XmlNode nodeSessionehere = nodehere.SelectSingleNode("Sessioni/Sessione[@Alias=\"" + selectedAliasCodificato + "\"]");
        XmlNode paretntnodeherehere = nodehere.ParentNode;
        while (paretntnodeherehere != null && paretntnodeherehere.Attributes["Expanded"] != null)
        {
          paretntnodeherehere.Attributes["Expanded"].Value = "True";
          StaticUtilities.MarkNodeAsModified(paretntnodeherehere, App.OBJ_MOD); m_isModified = true;
          paretntnodeherehere = paretntnodeherehere.ParentNode;
        }
        if (nodeSessionehere != null && nodeSessionehere.Attributes["Stato"].Value != (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString())
        {
          nodeSessionehere.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString();
          StaticUtilities.MarkNodeAsModified(nodeSessionehere, App.OBJ_MOD); m_isModified = true;
        }
        if (_x.Document.SelectSingleNode("//Dati//Dato[@ID='" + wcnn.changedID + "']").Attributes["Stato"] == null)
        {
          XmlAttribute attr = _x.Document.CreateAttribute("Stato");
          _x.Document.SelectSingleNode("//Dati//Dato[@ID='" + wcnn.changedID + "']").Attributes.Append(attr);
        }
        node1 = _x.Document.SelectSingleNode("//Dati//Dato[@ID='" + wcnn.changedID + "']");
        node1.Attributes["Stato"].Value = nodeSessionehere.Attributes["Stato"].Value;
        StaticUtilities.MarkNodeAsModified(node1, App.OBJ_MOD); m_isModified = true;
        _x.Save();
        ReloadStatoNodiPadre();
        SaveTreeSource(m_isModified);
        e.Handled = true;
        StaticUtilities.SetLockStatus(SelectedTreeSource, node.Attributes["Codice"].Value, false);
        return;
      }
      if (node.Attributes["ID"].Value == "100013" && IDTree == "26")
      {
        cBusinessObjects.idsessione = int.Parse(IDSessione);
        cBusinessObjects.idcliente = int.Parse(IDCliente);
        DataTable datipianificazioniTestata = cBusinessObjects.GetData(100013, typeof(PianificazioneVerificheTestata));

        Hashtable htSessioniP = new Hashtable();
        string lastData = "";
        string lastKey = "";
        foreach (DataRow pianificazioneNode in datipianificazioniTestata.Rows)
        {
          if (!htSessioniP.Contains(pianificazioneNode["ID"].ToString()))
          {
            htSessioniP.Add(pianificazioneNode["ID"].ToString(), pianificazioneNode["Data"].ToString());
            lastData = pianificazioneNode["Data"].ToString();
            lastKey = pianificazioneNode["ID"].ToString();
          }
        }
        MasterFile mf = MasterFile.Create();
        Hashtable htmf = mf.GetPianificazioniVerifica(IDSessione);
        wSchedaSessioniPianificazioniVerifiche sspve = new wSchedaSessioniPianificazioniVerifiche();
        sspve.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        switch (base.WindowState)
        {
          case System.Windows.WindowState.Normal:
            sspve.Width = ActualWidth * 97 / 100;
            sspve.Height = ActualHeight * 97 / 100;
            break;
          case System.Windows.WindowState.Maximized:
            sspve.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 97 / 100;
            sspve.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 97 / 100;
            break;
        }
        nodeSessione = node.SelectSingleNode("Sessioni/Sessione[@Alias=\"" + selectedAliasCodificato + "\"]");
        if (nodeSessione != null)
        {
          sspve.Stato = ((App.TipoTreeNodeStato)(Convert.ToInt32(nodeSessione.Attributes["Stato"].Value)));
          sspve.OldStatoNodo = sspve.Stato;
        }
        XmlNode nodeNota = ((XmlNode)(tvMain.SelectedItem));
        sspve.nota = node.Attributes["Nota"].Value;
        sspve.lastData = lastData;
        sspve.lastKey = lastKey;
        sspve.DataInizio = htmf["DataInizio"].ToString();
        sspve.DataFine = htmf["DataFine"].ToString();
        sspve.htSessioni = htSessioniP;
        sspve.Cliente = Cliente;
        sspve.IDCliente = IDCliente;
        sspve._x = _x;
        sspve.ReadOnly = ReadOnly;
        sspve.IDTree = IDTree;
        sspve.IDSessione = IDSessione;

        sspve.ConfiguraMaschera();
        sspve.Activate();
        sspve.ShowDialog();
        if (!m_isModified) m_isModified = sspve.m_isModified;

        if (nodeSessione != null
          && (sspve.Stato != App.TipoTreeNodeStato.Sconosciuto
            || (sspve.Stato == App.TipoTreeNodeStato.Sconosciuto
              && nodeSessione.Attributes["Stato"].Value != (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString())))
        {
          if (nodeSessione.Attributes["Stato"].Value != (Convert.ToInt32(App.TipoTreeNodeStato.SolaLettura)).ToString())
          {
            nodeSessione.Attributes["Stato"].Value = (Convert.ToInt32(sspve.Stato)).ToString();
            StaticUtilities.MarkNodeAsModified(nodeSessione, App.OBJ_MOD); m_isModified = true;
          }
        }

        DataTable dstati = cBusinessObjects.GetData(100013, typeof(StatoNodi));
        if (dstati.Rows.Count == 0)
          dstati.Rows.Add(100013, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
        foreach (DataRow dt in dstati.Rows)
        {
          dt["Stato"] = nodeSessione.Attributes["Stato"].Value;
        }
        cBusinessObjects.SaveData(100013, dstati, typeof(StatoNodi));

        ReloadStatoNodiPadre();
        SaveTreeSource(m_isModified);
        e.Handled = true;
        StaticUtilities.SetLockStatus(SelectedTreeSource, node.Attributes["Codice"].Value, false);
        return;
      }
      if (node.Attributes["ID"].Value == "100003" && IDTree == "27")
      {
        cBusinessObjects.idsessione = int.Parse(IDSessione);
        cBusinessObjects.idcliente = int.Parse(IDCliente);
        DataTable datipianificazioniTestata = cBusinessObjects.GetData(100003, typeof(PianificazioneVerificheTestata));

        Hashtable htSessioniP = new Hashtable();
        string lastData = "";
        string lastKey = "";
        foreach (DataRow pianificazioneNode in datipianificazioniTestata.Rows)
        {
          if (!htSessioniP.Contains(pianificazioneNode["ID"].ToString()))
          {
            htSessioniP.Add(pianificazioneNode["ID"].ToString(), pianificazioneNode["Data"].ToString());
            lastData = pianificazioneNode["Data"].ToString();
            lastKey = pianificazioneNode["ID"].ToString();
          }
        }
        MasterFile mf = MasterFile.Create();
        Hashtable htmf = mf.GetPianificazioniVigilanza(IDSessione);
        wSchedaSessioniPianificazioniVigilanze sspve = new wSchedaSessioniPianificazioniVigilanze();
        sspve.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        switch (base.WindowState)
        {
          case System.Windows.WindowState.Normal:
            sspve.Width = ActualWidth * 97 / 100;
            sspve.Height = ActualHeight * 97 / 100;
            break;
          case System.Windows.WindowState.Maximized:
            sspve.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 97 / 100;
            sspve.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 97 / 100;
            break;
        }
        nodeSessione = node.SelectSingleNode("Sessioni/Sessione[@Alias=\"" + selectedAliasCodificato + "\"]");
        if (nodeSessione != null)
        {
          sspve.Stato = ((App.TipoTreeNodeStato)(Convert.ToInt32(nodeSessione.Attributes["Stato"].Value)));
          sspve.OldStatoNodo = sspve.Stato;
        }
        XmlNode nodeNota = ((XmlNode)(tvMain.SelectedItem));
        sspve.nota = node.Attributes["Nota"].Value;
        sspve.lastData = lastData;
        sspve.lastKey = lastKey;
        sspve.DataInizio = htmf["DataInizio"].ToString();
        sspve.DataFine = htmf["DataFine"].ToString();
        sspve.htSessioni = htSessioniP;
        sspve.Cliente = Cliente;
        sspve.IDCliente = IDCliente;
        sspve._x = _x;
        sspve.ReadOnly = ReadOnly;
        sspve.IDTree = IDTree;
        sspve.IDSessione = IDSessione;

        sspve.ConfiguraMaschera();
        sspve.Activate();
        sspve.ShowDialog();
        if (!m_isModified) m_isModified = sspve.m_isModified;

        if (nodeSessione != null
          && (sspve.Stato != App.TipoTreeNodeStato.Sconosciuto
            || (sspve.Stato == App.TipoTreeNodeStato.Sconosciuto
              && nodeSessione.Attributes["Stato"].Value != (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString())))
        {
          if (nodeSessione.Attributes["Stato"].Value != (Convert.ToInt32(App.TipoTreeNodeStato.SolaLettura)).ToString())
          {
            nodeSessione.Attributes["Stato"].Value = (Convert.ToInt32(sspve.Stato)).ToString();
            StaticUtilities.MarkNodeAsModified(nodeSessione, App.OBJ_MOD); m_isModified = true;
          }
        }
        DataTable dstati = cBusinessObjects.GetData(100013, typeof(StatoNodi));
        if (dstati.Rows.Count == 0)
          dstati.Rows.Add(100013, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
        foreach (DataRow dt in dstati.Rows)
        {
          dt["Stato"] = nodeSessione.Attributes["Stato"].Value;
        }
        cBusinessObjects.SaveData(100013, dstati, typeof(StatoNodi));


        ReloadStatoNodiPadre();
        SaveTreeSource(m_isModified);
        e.Handled = true;
        StaticUtilities.SetLockStatus(SelectedTreeSource, node.Attributes["Codice"].Value, false);
        return;
      }
      if (_x.Document != null)
      {
        XmlNodeList tmpnode2 = _x.Document.SelectNodes("//Node[@xaml]");
        foreach (XmlNode item in tmpnode2)
        {
          if (!(item.Attributes["xaml"].Value.Contains("\\XAML\\")))
          {
            DirectoryInfo di = new DirectoryInfo(App.AppDataDataFolder + "\\XAML");
            if (!di.Exists) di.Create();
            string newXamlFile = "\\XAML\\" + Guid.NewGuid().ToString() + ".xaml";
            FileInfo fxaml = new FileInfo(App.AppDataDataFolder + newXamlFile);
            while (fxaml.Exists)
            {
              newXamlFile = "\\XAML\\" + Guid.NewGuid().ToString() + ".xaml";
              fxaml = new FileInfo(App.AppDataDataFolder + newXamlFile);
            }
            StreamWriter sw = fxaml.CreateText();
            sw.WriteLine(item.Attributes["xaml"].Value);
            sw.Flush();
            sw.Close();
            item.Attributes["xaml"].Value = newXamlFile;
            StaticUtilities.MarkNodeAsModified(item, App.OBJ_MOD); m_isModified = true;
          }
          else break;
        }
        _x.Save();
      }

      WindowWorkArea wa = new WindowWorkArea(ref _x);

      // Nodi
      int index = -1;
      wa.NodeHome = -1;
      if (TreeXmlProvider.Document != null
        && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
      {
        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
        {
          if (item.Attributes["Tipologia"].Value == "Nodo Multiplo"
            || item.ChildNodes.Count == 1)
          {
            index++;
            if (item.Attributes["ID"].Value == node.Attributes["ID"].Value)
            {
              wa.NodeHome = index;
            }
            if (!wa.Nodes.ContainsKey(index))
            {
              wa.Nodes.Add(index, item);
            }
          }
        }
      }
      if (wa.NodeHome == -1)
      {
        e.Handled = true;
        StaticUtilities.SetLockStatus(SelectedTreeSource, node.Attributes["Codice"].Value, false);
        return;
      }
      wa.NodeNow = wa.NodeHome;
      wa.Owner = Window.GetWindow(this);
      // posizione e dimensioni finestra

      wa.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

      var graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
      var pixelWidth = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
      var pixelHeight = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
      var pixelToDPI = 96.0 / graphics.DpiX;
      this.Width = pixelWidth * pixelToDPI;
      this.Height = pixelHeight * pixelToDPI;
      this.Left = 0;
      this.Top = 0;
      this.WindowState = WindowState.Normal;

      /*
                  switch (base.WindowState)
              {
                case System.Windows.WindowState.Normal:
                  wa.Width = ActualWidth * 97 / 100;
                  wa.Height = ActualHeight * 97 / 100;
                  break;
                case System.Windows.WindowState.Maximized:
                  wa.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 100 / 100;
                  wa.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 97 / 100;
                  break;
              }
      */

      // Sessioni
      wa.Sessioni = htSessioni;
      wa.SessioniTitoli = htSessioniAlias;
      wa.SessioniID = htSessioniID;
      foreach (DictionaryEntry item in htSessioni)
      {
        if (item.Value.ToString() == _x.File)
        {
          wa.SessioneHome = Convert.ToInt32(item.Key.ToString());
          wa.SessioneNow = wa.SessioneHome;
          break;
        }
      }
      // Variabili
      ReadOnly = false; // E.B.
      wa.ReadOnly = ReadOnly;
      wa.ReadOnlyOLD = ReadOnly;
      wa.ApertoInSolaLettura = ApertoInSolaLettura;
      nodeSessione = node.SelectSingleNode(
        "Sessioni/Sessione[@Alias=\"" + selectedAliasCodificato + "\"]");
      if (nodeSessione != null)
      {
        wa.Stato = ((App.TipoTreeNodeStato)(Convert.ToInt32(nodeSessione.Attributes["Stato"].Value)));
        if (wa.Stato == App.TipoTreeNodeStato.Scrittura)
        {
          wa.Stato = App.TipoTreeNodeStato.DaCompletare;
          nodeSessione.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString();
          StaticUtilities.MarkNodeAsModified(nodeSessione, App.OBJ_MOD); m_isModified = true;
        }
        wa.OldStatoNodo = wa.Stato;
      }


      // TEAM
      // a seconda del ruolo dell'utente loggato si abilitano/disabilitano i bottoni o si impedisce l'apertura della cartella
      string cartella = node.Attributes["Codice"].Value;
      switch (App.AppRuolo)
      {
        case App.RuoloDesc.TeamLeader:
          switch (wa.Stato)
          {
            case App.TipoTreeNodeStato.SolaLettura:
              wa._enableTeam = App.TipoAbilitazioneWindow.TuttoDisabilitato;
              break;
            default:
              // l'utente team leader può modificare le Aree: Areaa1 e Area ISQC, tutte le altre in sola lettura
              if (cartella != "1" && !cartella.StartsWith("1.") && cartella.ToUpper() != "ISQC" && !cartella.ToUpper().StartsWith("ISQC"))
                wa._enableTeam = App.TipoAbilitazioneWindow.TuttoDisabilitato;
              else
                wa._enableTeam = App.TipoAbilitazioneWindow.AbilitaPerTeamLeader;
              break;
          }
          break;
        case App.RuoloDesc.Reviewer:
          // l'utente Revisore può visionare le cartelle assegnate ai suoi esecutori:
          // se la cartella è in stato "Da Completare" può completare e bloccare
          // se la cartella è in stato "Completato" può impostare Da Completare e bloccare
          if (cCartelle.EsisteCartellaPerEsecutoreDiRevisore(Convert.ToInt32(IDCliente), App.AppUtente.Id, cartella))
          {
            // la cartella è assegnata ad uno dei "suoi" esecutori
            switch (wa.Stato)
            {
              case App.TipoTreeNodeStato.SolaLettura:
                wa._enableTeam = App.TipoAbilitazioneWindow.TuttoDisabilitatoPerReviewer;
                break;
              case App.TipoTreeNodeStato.Completato:
              case App.TipoTreeNodeStato.DaCompletare:
              case App.TipoTreeNodeStato.Sconosciuto:
              case App.TipoTreeNodeStato.CompletatoBloccoEsecutore:
                if (cCartelle.IsCartellaBloccata(cartella, App.AppUtente.Id, IDCliente, true))
                  wa._enableTeam = App.TipoAbilitazioneWindow.AbilitaPerReviewerBloccato;
                else
                  wa._enableTeam = App.TipoAbilitazioneWindow.AbilitaPerReviewer;
                break;
              default:
                wa._enableTeam = App.TipoAbilitazioneWindow.TuttoDisabilitatoPerReviewer;
                break;
            }
          }
          else
            wa._enableTeam = App.TipoAbilitazioneWindow.TuttoDisabilitatoPerReviewer;
          break;
        case App.RuoloDesc.Esecutore:

          // l'utente esecutore può visionare le cartelle che gli sono state assegnate
          if (!cCartelle.EsisteCartellaPerEsecutore(Convert.ToInt32(IDCliente), App.AppUtente.Id, cartella))
          {
            MessageBox.Show($"{App.AppUtente.Login} non autorizzato su questa carta di lavoro", "Non autorizzato", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
            StaticUtilities.SetLockStatus(SelectedTreeSource, node.Attributes["Codice"].Value, false);
            return;
          }
          // l'esecutore può modificare la cartella solo se il revisore non ha bloccato la cartella
          bool isBloccata = cCartelle.IsCartellaBloccata(cartella, App.AppUtente.Id, IDCliente, false);
          if (isBloccata)
            wa._enableTeam = App.TipoAbilitazioneWindow.TuttoDisabilitato;
          else
            switch (wa.Stato)
            {
              case App.TipoTreeNodeStato.SolaLettura:
                wa._enableTeam = App.TipoAbilitazioneWindow.TuttoDisabilitato;
                break;
              default:
                wa._enableTeam = App.TipoAbilitazioneWindow.AbilitaPerEsecutore;
                break;
            }

          break;
        default:
          // non si disabilitano i bottoni
          wa._enableTeam = App.TipoAbilitazioneWindow.TuttoAbilitato;
          break;
      }
      wa._cartellaxTeam = cartella;

      // passaggio dati
      wa.IDTree = IDTree;
      wa.IDSessione = IDSessione;
      wa.IDCliente = IDCliente;

      //APERTURA
      // apertura
      wa.Load();

      wa.ConfiguraStatoNodo(App.TipoTreeNodeStato.Scrittura, false);
      try
      {
        wa.ShowDialog();
      }
      catch (Exception a)
      {
        cBusinessObjects.logger.Error(a, "wWorkAreaTree Errore dopo ritorno a da wa.ShowDialog() ddella carta di lavoro");
        return;
      }


      if (!m_isModified)
        m_isModified = wa.m_isModified;
      if (m_isModified)
      {
        node = ((XmlNode)(tvMain.SelectedItem));
        StaticUtilities.MarkNodeAsModified(node, App.OBJ_MOD);
      }
      if (nodeSessione != null && (wa.Stato != App.TipoTreeNodeStato.Sconosciuto
        || (wa.Stato == App.TipoTreeNodeStato.Sconosciuto
        && nodeSessione.Attributes["Stato"].Value != (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString())))
      {
        if (nodeSessione.Attributes["Stato"].Value != (Convert.ToInt32(App.TipoTreeNodeStato.SolaLettura)).ToString())
        {
          nodeSessione.Attributes["Stato"].Value = (Convert.ToInt32(wa.Stato)).ToString();
          StaticUtilities.MarkNodeAsModified(nodeSessione, App.OBJ_MOD); m_isModified = true;
        }
      }
      string IDNodeList = node.Attributes["ID"].Value;
      foreach (XmlNode child in TreeXmlProvider.Document.SelectSingleNode("//Tree//Node[@ID='" + node.Attributes["ID"].Value + "']").ChildNodes)
      {
        if (child.Attributes["ID"] != null)
        {
          IDNodeList += "|" + child.Attributes["ID"].Value;
        }
      }
      foreach (string nodeID in IDNodeList.Split('|'))
      {
        DataTable dstati = cBusinessObjects.GetData(int.Parse(nodeID), typeof(StatoNodi));
        if (dstati.Rows.Count == 0)
          dstati.Rows.Add(int.Parse(nodeID), cBusinessObjects.idcliente, cBusinessObjects.idsessione);
        foreach (DataRow dt in dstati.Rows)
        {
          //  dt["Stato"]= nodeSessione.Attributes["Stato"].Value;
          dt["Stato"] = (Convert.ToInt32(wa.Stato)).ToString();
        }
        cBusinessObjects.SaveData(int.Parse(nodeID), dstati, typeof(StatoNodi));
      }
      ReloadStatoNodiPadre();
      /*
       * CODICE VECCHIO DI GESTIONE DELLO STATO DEL SINGOLO NODO
              foreach (string nodeID in IDNodeList.Split('|'))
              {
                removable = _x.Document.SelectSingleNode("//Dati//Dato[@ID='" + nodeID + "']");
                imported = _x.Document.ImportNode(
                  wa._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + nodeID + "']"), true);
                removable.ParentNode.ReplaceChild(imported, removable);
                StaticUtilities.MarkNodeAsModified(imported, App.OBJ_MOD); m_isModified = true;
              }
              if (_x.Document.SelectSingleNode(
                "//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']").Attributes["Stato"] == null)
              {
                XmlAttribute attr = _x.Document.CreateAttribute("Stato");
                node1 = _x.Document.SelectSingleNode("//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");
                node1.Attributes.Append(attr);
                StaticUtilities.MarkNodeAsModified(node1, App.OBJ_MOD); m_isModified = true;
              }
              node1 = _x.Document.SelectSingleNode("//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");
              node1.Attributes["Stato"].Value = nodeSessione.Attributes["Stato"].Value;
              StaticUtilities.MarkNodeAsModified(node1, App.OBJ_MOD); m_isModified = true;
              _x.isModified = true;
              _x.Save(true);
              SaveTreeSource(true); m_isModified = false;
        */
      e.Handled = true;
      StaticUtilities.SetLockStatus(SelectedTreeSource, node.Attributes["Codice"].Value, false);
    }




    //----------------------------------------------------------------------------+
    //                         Image_MouseLeftButtonDown                          |
    //----------------------------------------------------------------------------+
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

    //----------------------------------------------------------------------------+
    //                          btn_NonApplicabile_Click                          |
    //----------------------------------------------------------------------------+
    private void btn_NonApplicabile_Click(object sender, RoutedEventArgs e)
    {
      //    if (ReadOnly)
     //    {
     //    MessageBox.Show("Sessione in sola lettura", "Attenzione");
      //   return;
     //  }
      XmlNode node = ((XmlNode)(tvMain.SelectedItem));
      if (node.Attributes["Report"].Value == "True") return;
      XmlNode SelectedNode = node.SelectSingleNode("Sessioni/Sessione[@Selected='#AA82BDE4']");
      if (SelectedNode.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString() || SelectedNode.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString())
      {
        MessageBox.Show("Questa Carta di Lavoro ha già uno stato assegnato, non è possibile renderlo Non Applicabile.", "Attenzione");
        return;
      }
      if (SelectedNode.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile)).ToString())
      {
        SelectedNode.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString();
      }
      else
      {
        SelectedNode.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile)).ToString();
      }



      try
      {
        DataTable dstati = cBusinessObjects.GetData(int.Parse(node.Attributes["ID"].Value), typeof(StatoNodi));
        if (dstati.Rows.Count == 0)
          dstati.Rows.Add(int.Parse(node.Attributes["ID"].Value), cBusinessObjects.idcliente, cBusinessObjects.idsessione);
        foreach (DataRow dt in dstati.Rows)
        {
          dt["Stato"] = SelectedNode.Attributes["Stato"].Value;
        }
        cBusinessObjects.SaveData(int.Parse(node.Attributes["ID"].Value), dstati, typeof(StatoNodi));
      }
      catch (Exception ez)
      {

      }



      ReloadStatoNodiPadre();
    }

    //----------------------------------------------------------------------------+
    //                             buttonChiudi_Click                             |
    //----------------------------------------------------------------------------+
    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      //_x.isModified = m_isModified;
      //if (_x != null) _x.Save();
      //SaveTreeSourceNoReload();
      base.Close();
    }

    //----------------------------------------------------------------------------+
    //                         buttonApriFormulario_Click                         |
    //----------------------------------------------------------------------------+
    private void buttonApriFormulario_Click(object sender, RoutedEventArgs e)
    {
      Formulario formulario = new Formulario();

      formulario.Owner = this;
      formulario.LoadTreeSource();
      formulario.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                         buttonApriDocumenti_Click                          |
    //----------------------------------------------------------------------------+
    private void buttonApriDocumenti_Click(object sender, RoutedEventArgs e)
    {
      wDocumenti documenti = new wDocumenti();

      documenti.ReadOnly = ReadOnly;
      documenti.Titolo = "Indice Documenti per Cliente";
      documenti.Tipologia = TipoVisualizzazione.Documenti;
      documenti.Tree = IDTree;
      documenti.Cliente = IDCliente;
      documenti.Sessione = "-1"; //IDSessione;
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

    //----------------------------------------------------------------------------+
    //                           btn_ScambioDati_Click                            |
    //----------------------------------------------------------------------------+
    private void btn_ScambioDati_Click(object sender, RoutedEventArgs e)
    {
      WindowWorkAreaTree_ScambioDati wWorkAreaSD = new WindowWorkAreaTree_ScambioDati();
      cBusinessObjects.show_workinprogress("Elaborazione in corso ...");

      wWorkAreaSD.Owner = this;
      wWorkAreaSD.SelectedTreeSource = SelectedTreeSource;
      wWorkAreaSD.SelectedDataSource = SelectedDataSource;
      wWorkAreaSD.Cliente = _cliente;
      wWorkAreaSD.IDTree = IDTree;
      wWorkAreaSD.IDCliente = IDCliente;
      wWorkAreaSD.IDSessione = IDSessione;
      //andrea
      wWorkAreaSD.TitoloSessione = selectedAlias;
      wWorkAreaSD.Tipo = App.TipoScambioDati.Esporta;
      wWorkAreaSD.TipoAttivita = _TipoAttivita;
      //carico dati
      wWorkAreaSD.LoadTreeSource();
      cBusinessObjects.hide_workinprogress();
      wWorkAreaSD.ShowDialog();
      //this.LoadTreeSource();
    }

    //----------------------------------------------------------------------------+
    //                            menuInfoGuida_Click                             |
    //----------------------------------------------------------------------------+
    private void menuInfoGuida_Click(object sender, RoutedEventArgs e)
    {
      //file
      //System.Diagnostics.Process.Start(App.AppHelpFile);
      //web
      System.Diagnostics.Process.Start(RevisoftApplication.Properties.Settings.Default["RevisoftApplicationGuide"].ToString());
    }

    //----------------------------------------------------------------------------+
    //                          menuCampionamento_Click                           |
    //----------------------------------------------------------------------------+
    private void menuCampionamento_Click(object sender, RoutedEventArgs e)
    {
      XmlNode nodeTreePadre = TreeXmlProvider.Document.SelectSingleNode("/Tree");
      XmlNode nodeTreeSessione = nodeTreePadre.SelectSingleNode("Sessioni/Sessione[@Selected='#AA82BDE4']");
      wCampionamento wcnn = new wCampionamento(nodeTreeSessione, IDCliente, Cliente, Esercizio, IDSessione, IDTree);
      wcnn.ShowDialog();
      if (!wcnn.diagres)
      {
        e.Handled = true;
        return;
      }

         
 return;


      RevisoftApplication.XmlManager xx = new XmlManager();
      xx.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
      TreeXmlProvider.Document = cBusinessObjects.NewLoadEncodedFile(SelectedTreeSource,IDTree);
      TreeXmlProvider.Refresh();
      XmlNode nodehere = TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=" + wcnn.changedID + "]");
      XmlNode nodeSessionehere = nodehere.SelectSingleNode("Sessioni/Sessione[@Alias=\"" + selectedAliasCodificato + "\"]");
      XmlNode paretntnodeherehere = nodehere.ParentNode;
      while (paretntnodeherehere != null && paretntnodeherehere.Attributes["Expanded"] != null)
      {
        paretntnodeherehere.Attributes["Expanded"].Value = "True";
        paretntnodeherehere = paretntnodeherehere.ParentNode;
      }
      if (nodeSessionehere != null && nodeSessionehere.Attributes["Stato"].Value != (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString())
      {
        nodeSessionehere.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString();
      }
      //   if (_x.Document.SelectSingleNode("//Dati//Dato[@ID='" + wcnn.changedID + "']").Attributes["Stato"] == null)
      //   {
      //         XmlAttribute attr = _x.Document.CreateAttribute("Stato");
      //         _x.Document.SelectSingleNode("//Dati//Dato[@ID='" + wcnn.changedID + "']").Attributes.Append(attr);
      //     }
      //         _x.Document.SelectSingleNode("//Dati//Dato[@ID='" + wcnn.changedID + "']").Attributes["Stato"].Value = nodeSessionehere.Attributes["Stato"].Value;
      //          _x.Save();


      ReloadStatoNodiPadre();
      SaveTreeSource();
      e.Handled = true;
      return;
    }

    //----------------------------------------------------------------------------+
    //                          btn_GuidaRevisoft_Click                           |
    //----------------------------------------------------------------------------+
    private void btn_GuidaRevisoft_Click(object sender, RoutedEventArgs e)
    {
      XmlNode node;
      string nota = "";

      try
      {
        node = ((XmlNode)(tvMain.SelectedItem));
        nota = node.Attributes["Nota"].Value;
        string fileguida = AppDomain.CurrentDomain.BaseDirectory + "/guida/" + node.Attributes["Codice"].Value + ".htm";
        if (File.Exists(fileguida))
          nota = File.ReadAllText(fileguida);

      }
      catch (Exception ex)
      {

      }
      wGuidaRevisoft w = new wGuidaRevisoft();
      w.Owner = Window.GetWindow(this);
      w.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
      //w.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
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
      //Point p = Mouse.GetPosition(this);
      //switch (base.WindowState)
      //{
      //    case System.Windows.WindowState.Normal:
      //        w.Top = this.Top + p.Y;
      //        w.Left = this.Left + p.X;
      //        break;
      //    case System.Windows.WindowState.Maximized:
      //        w.Top = p.Y;
      //        w.Left = p.X;
      //        break;
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

    bool StampaLetteraAttestazione = false;

    //----------------------------------------------------------------------------+
    //                    btn_StampaLetteraAttestazione_Click                     |
    //----------------------------------------------------------------------------+
    public void btn_StampaLetteraAttestazione_Click(object sender, RoutedEventArgs e)
    {
      if (TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=\"261\"]") == null)
      {
        MessageBox.Show("documento non disponibile");
        return;
      }

      MasterFile mf = MasterFile.Create();
      Hashtable cliente = mf.GetAnagrafica(Convert.ToInt32(IDCliente));
      Hashtable dati = new Hashtable();
      //WordLib wl = new WordLib();
      RTFLib wl = new RTFLib();
      string Intestazione = "";
      dati = mf.GetConclusione(IDSessione);
      wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
      wl.StampaLetteraAttestazione = true;
      wl.StampaTemporanea = StampaTemporanea;
      wl.htCliente = cliente;
      wl.Open(
        dati,
        cliente["RagioneSociale"].ToString(),
        cliente["CodiceFiscale"].ToString(),
        selectedAliasCodificato,
        TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value,
        false, true, IDCliente);
      StampaLetteraAttestazione = true;
      RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=\"261\"]"), wl, SelectedDataSource);
      Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " "
        + cliente["CodiceFiscale"].ToString() + " Esercizio: "
        + ConvertDate(dati["Data"].ToString());

      wl.Save(Intestazione);
      StampaTemporanea = false;
      wl.Close();
    }

    bool StampaManagementLetter = false;

    //----------------------------------------------------------------------------+
    //                      btn_StampaManagementLetter_Click                      |
    //----------------------------------------------------------------------------+
    public void btn_StampaManagementLetter_Click(object sender, RoutedEventArgs e)
    {
      if (TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=\"281\"]") == null)
      {
        MessageBox.Show("documento non disponibile", "operazione impossibile", MessageBoxButton.OK);
        return;
      }
      //ProgressWindow pw = new ProgressWindow();
      MasterFile mf = MasterFile.Create();
      Hashtable cliente = mf.GetAnagrafica(Convert.ToInt32(IDCliente));
      Hashtable dati = new Hashtable();
      //WordLib wl = new WordLib();
      RTFLib wl = new RTFLib();
      string Intestazione = "";
      dati = mf.GetConclusione(IDSessione);
      wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
      wl.StampaManagementLetter = true;
      wl.StampaTemporanea = StampaTemporanea;
      wl.htCliente = cliente;
      wl.Open(
        dati,
        cliente["RagioneSociale"].ToString(),
        cliente["CodiceFiscale"].ToString(),
        selectedAliasCodificato,
        TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value,
        false, true, IDCliente);
      StampaManagementLetter = true;
      RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=\"281\"]").ParentNode, wl, SelectedDataSource);
      Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " "
        + cliente["CodiceFiscale"].ToString() + " Esercizio: "
        + ConvertDate(dati["Data"].ToString());
      //if (pw != null) { pw.Close(); pw = null; }
      wl.Save(Intestazione);
      StampaTemporanea = false;
      wl.Close();
    }

    bool StampaLetteraIncarico = false;

    //----------------------------------------------------------------------------+
    //                      btn_StampaLetteraIncarico_Click                       |
    //----------------------------------------------------------------------------+
    public void btn_StampaLetteraIncarico_Click(object sender, RoutedEventArgs e)
    {
      if (TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=\"142\"]") == null)
      {
        MessageBox.Show("documento non disponibile", "operazione impossibile", MessageBoxButton.OK);
        return;
      }



      MasterFile mf = MasterFile.Create();
      Hashtable cliente = mf.GetAnagrafica(Convert.ToInt32(IDCliente));
      Hashtable dati = new Hashtable();
      //WordLib wl = new WordLib();
      RTFLib wl = new RTFLib();
      string Intestazione = "";

      dati = mf.GetIncarico(IDSessione);
      wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
      wl.StampaLetteraIncarico = true;
      wl.StampaTemporanea = StampaTemporanea;
      wl.htCliente = cliente;
      wl.Open(
        dati, cliente["RagioneSociale"].ToString(),
        cliente["CodiceFiscale"].ToString(), selectedAliasCodificato,
        TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true, IDCliente);
      StampaLetteraIncarico = true;
      RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=\"142\"]"), wl, SelectedDataSource);
      Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " "
        + cliente["CodiceFiscale"].ToString() + " Esercizio: "
        + ConvertDate(dati["DataNomina"].ToString());

      wl.Save(Intestazione);
      StampaTemporanea = false;
      wl.Close();
    }

    //----------------------------------------------------------------------------+
    //                  btn_StampaLetteraIncaricoCollegio_Click                   |
    //----------------------------------------------------------------------------+
    public void btn_StampaLetteraIncaricoCollegio_Click(object sender, RoutedEventArgs e)
    {
      if (TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=\"2016142\"]") == null)
      {
        MessageBox.Show("documento non disponibile", "operazione impossibile", MessageBoxButton.OK);
        return;
      }



      MasterFile mf = MasterFile.Create();
      Hashtable cliente = mf.GetAnagrafica(Convert.ToInt32(IDCliente));
      Hashtable dati = new Hashtable();
      //WordLib wl = new WordLib();
      RTFLib wl = new RTFLib();
      string Intestazione = "";

      dati = mf.GetIncarico(IDSessione);
      wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
      wl.StampaLetteraIncarico = true;
      wl.StampaTemporanea = StampaTemporanea;
      wl.htCliente = cliente;
      wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true, IDCliente);
      StampaLetteraIncarico = true;
      RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=\"2016142\"]"), wl, SelectedDataSource);
      Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["DataNomina"].ToString());

      wl.Save(Intestazione);
      StampaTemporanea = false;
      wl.Close();
    }

    bool StampaCodiceEtico = false;

    //----------------------------------------------------------------------------+
    //                        btn_StampaCodiceEtico_Click                         |
    //----------------------------------------------------------------------------+
    public void btn_StampaCodiceEtico_Click(object sender, RoutedEventArgs e)
    {
      if (TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=\"142\"]") == null)
      {
        MessageBox.Show("documento non disponibile", "operazione impossibile", MessageBoxButton.OK);
        return;
      }


      MasterFile mf = MasterFile.Create();
      Hashtable cliente = mf.GetAnagrafica(Convert.ToInt32(IDCliente));
      Hashtable dati = new Hashtable();
      //WordLib wl = new WordLib();
      RTFLib wl = new RTFLib();
      string Intestazione = "";

      dati = mf.GetISQC(IDSessione);
      wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
      wl.StampaCodiceEtico = true;
      wl.StampaTemporanea = StampaTemporanea;
      wl.htCliente = cliente;
      wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true, IDCliente);
      StampaCodiceEtico = true;
      RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=\"142\"]"), wl, SelectedDataSource);
      Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["DataNomina"].ToString());

      wl.Save(Intestazione);
      StampaTemporanea = false;
      wl.Close();
    }

    string IDB_Padre = "227";
    string IDBA_Padre = "229";
    string tipologiaBilancio = "";
    string tipoBilancio = "";

    //----------------------------------------------------------------------------+
    //                           btn_StampaReport_Click                           |
    //----------------------------------------------------------------------------+
    public void btn_StampaReport_Click(object sender, RoutedEventArgs e)
    {
      //string str = string.Format(
      //    "SelectedTreeSource: {0}\n" +
      //    "SelectedDataSource: {1}\n" +
      //    "SelectedSessioneSource: {2}\n",
      //    SelectedTreeSource, SelectedDataSource, SelectedSessioneSource);
      //MessageBox.Show(str);
      //

      wSceltaStampa st = new wSceltaStampa();
      switch ((App.TipoFile)(System.Convert.ToInt32(IDTree)))
      {
        case App.TipoFile.Incarico:
        case App.TipoFile.IncaricoCS:
        case App.TipoFile.IncaricoSU:
        case App.TipoFile.IncaricoREV:

          st.StampePossibili.Add("Stampa Fascicolo");
          st.StampePossibili.Add("Stampa Lettera di Incarico Collegio");
          st.StampePossibili.Add("Stampa Lettera di Incarico Soggetto Unico");
          break;
        case App.TipoFile.ISQC:
          st.StampePossibili.Add("Stampa Fascicolo");
          st.StampePossibili.Add("Stampa Codice Etico");
          break;
        case App.TipoFile.Conclusione:
          st.StampePossibili.Add("Stampa Fascicolo");
          st.StampePossibili.Add("Stampa Lettera di Attestazione");
          st.StampePossibili.Add("Stampa Management Letter");
          break;
        case App.TipoFile.Revisione:
        case App.TipoFile.Bilancio:
          st.StampePossibili.Add("Stampa Fascicolo");
          break;
        case App.TipoFile.Verifica:
          st.StampePossibili.Add("Stampa Anteprima");
          st.StampePossibili.Add("Stampa Carte di Lavoro Vuote");
          break;
        case App.TipoFile.Vigilanza:
          st.StampePossibili.Add("Stampa Anteprima");
          TextBlock_Btn_Stampa.Text = "Anteprima";
          break;
        case App.TipoFile.RelazioneB:
        case App.TipoFile.RelazioneV:
        case App.TipoFile.RelazioneBC:
        case App.TipoFile.RelazioneVC:
        case App.TipoFile.RelazioneBV:
          st.StampePossibili.Add("Stampa Relazione");
          break;
        case App.TipoFile.PianificazioniVerifica:
        case App.TipoFile.PianificazioniVigilanza:
          st.StampePossibili.Add("Stampa Pianificazione");
          break;
        default:
          break;
      }
      st.Load();
      if (st.reallychosen == false)
      {

        st.ShowDialog();
        if (st.reallychosen == false)
        {

          return;
        }
      }
      foreach (RadioButton item in st.collectorRadiobutton.Children)
      {
        if (item.IsChecked == true)
        {
          MasterFile mf = MasterFile.Create();
          Hashtable cliente = mf.GetAnagrafica(Convert.ToInt32(IDCliente));
          Hashtable dati = new Hashtable();
          //WordLib wl = new WordLib();
          RTFLib wl = new RTFLib();
          string Intestazione = "";
          switch ((App.TipoFile)(System.Convert.ToInt32(IDTree)))
          {
            case App.TipoFile.Revisione:
              if (item.Content.ToString() == "Stampa Fascicolo")
              {

                dati = mf.GetRevisione(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampa;
                wl.Fascicolo = true;
                wl.Open(dati, cliente["RagioneSociale"].ToString(),
                  cliente["CodiceFiscale"].ToString(),
                  selectedAliasCodificato,
                  TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value,
                  false, true, IDCliente);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                Intestazione = "Società: " + cliente["RagioneSociale"].ToString()
                  + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: "
                  + ConvertDate(dati["Data"].ToString());

                wl.SavePDF(Intestazione, this);
              }
              break;
            case App.TipoFile.RelazioneB:
              string FileBilancioB = mf.GetBilancioAssociatoFromRelazioneBFile(SelectedDataSource);
              if (FileBilancioB != "" && (new FileInfo(FileBilancioB)).Exists)
              {
                XmlDataProviderManager _xBV = new XmlDataProviderManager(FileBilancioB);
                if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']") != null
                  && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"] != null)
                {
                  tipologiaBilancio = "Ordinario";
                  tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"].Value;
                }
                else
                {
                  if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']") != null
                    && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"] != null)
                  {
                    tipologiaBilancio = "Abbreviato";
                    tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"].Value;
                  }
                }
              }
              if (item.Content.ToString() == "Stampa Relazione")
              {
                if (StampaTemporanea == false
                  && RecursiveCheckDaCompletarePresenti(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node")) == true)
                {
                  if (MessageBox.Show("Risultano paragrafi non completati o non esaminati. "
                    + "Procedo ugualmente con la stampa?", "Attenzione",
                    MessageBoxButton.YesNo) == MessageBoxResult.No)
                  {
                    Show(); Activate(); return;
                  }
                }
                dati = mf.GetRelazioneB(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                wl.StampaRelazioneGenerica = true;
                wl.StampaRelazioneBilancio = true;
                wl.StampaTemporanea = StampaTemporanea;
                wl.tipologiaBilancio = tipologiaBilancio;
                wl.tipoBilancio = tipoBilancio;
                wl.htCliente = cliente;
                //pw = new ProgressWindow();
                wl.Open(dati,
                  cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(),
                  selectedAliasCodificato,
                  TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value,
                  false, true, IDCliente);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " "
                  + cliente["CodiceFiscale"].ToString() + " Esercizio: "
                  + ConvertDate(dati["Data"].ToString());
                //if (pw != null) { pw.Close(); pw = null; }
                wl.Save(Intestazione);
                StampaTemporanea = false;
              }
              break;
            case App.TipoFile.RelazioneV:
              string FileBilancioV = mf.GetBilancioAssociatoFromRelazioneVFile(SelectedDataSource);
              if (FileBilancioV != "" && (new FileInfo(FileBilancioV)).Exists)
              {
                XmlDataProviderManager _xBV = new XmlDataProviderManager(FileBilancioV);
                if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']") != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"] != null)
                {
                  tipologiaBilancio = "Ordinario";
                  tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"].Value;
                }
                else
                {
                  if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']") != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"] != null)
                  {
                    tipologiaBilancio = "Abbreviato";
                    tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"].Value;
                  }
                }
              }
              if (item.Content.ToString() == "Stampa Relazione")
              {
                if (StampaTemporanea == false && RecursiveCheckDaCompletarePresenti(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node")) == true)
                {
                  if (MessageBox.Show("Risultano paragrafi non completati o non esaminati. Procedo ugualmente con la stampa?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
                  {
                    Show();
                    Activate();
                    return;
                  }
                }
                dati = mf.GetRelazioneV(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                wl.StampaRelazioneGenerica = true;
                wl.StampaRelazioneVigilanza = true;
                wl.StampaTemporanea = StampaTemporanea;
                wl.tipologiaBilancio = tipologiaBilancio;
                wl.tipoBilancio = tipoBilancio;
                wl.htCliente = cliente;
                wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true, IDCliente);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["Data"].ToString());
                wl.Save(Intestazione);
                StampaTemporanea = false;
              }
              break;
            case App.TipoFile.RelazioneBC:
              string FileBilancioBC = mf.GetBilancioAssociatoFromRelazioneBCFile(SelectedDataSource);
              if (FileBilancioBC != "" && (new FileInfo(FileBilancioBC)).Exists)
              {
                XmlDataProviderManager _xBV = new XmlDataProviderManager(FileBilancioBC);
                if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']") != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"] != null)
                {
                  tipologiaBilancio = "Ordinario";
                  tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"].Value;
                }
                else
                {
                  if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']") != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"] != null)
                  {
                    tipologiaBilancio = "Abbreviato";
                    tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"].Value;
                  }
                }
              }
              if (item.Content.ToString() == "Stampa Relazione")
              {
                if (StampaTemporanea == false && RecursiveCheckDaCompletarePresenti(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node")) == true)
                {
                  if (MessageBox.Show("Risultano paragrafi non completati o non esaminati. Procedo ugualmente con la stampa?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
                  {
                    Show();
                    Activate();
                    return;
                  }
                }
                dati = mf.GetRelazioneBC(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                wl.StampaRelazioneGenerica = true;
                wl.StampaRelazioneBilancio = true;
                wl.StampaTemporanea = StampaTemporanea;
                wl.tipologiaBilancio = tipologiaBilancio;
                wl.tipoBilancio = tipoBilancio;
                wl.htCliente = cliente;
                wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true, IDCliente);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["Data"].ToString());
                wl.Save(Intestazione);
                StampaTemporanea = false;
              }
              break;
            case App.TipoFile.RelazioneVC:
              string FileBilancioVC = mf.GetBilancioAssociatoFromRelazioneVCFile(SelectedDataSource);
              if (FileBilancioVC != "" && (new FileInfo(FileBilancioVC)).Exists)
              {
                XmlDataProviderManager _xBV = new XmlDataProviderManager(FileBilancioVC);
                if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']") != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"] != null)
                {
                  tipologiaBilancio = "Ordinario";
                  tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"].Value;
                }
                else
                {
                  if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']") != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"] != null)
                  {
                    tipologiaBilancio = "Abbreviato";
                    tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"].Value;
                  }
                }
              }
              if (item.Content.ToString() == "Stampa Relazione")
              {
                if (StampaTemporanea == false && RecursiveCheckDaCompletarePresenti(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node")) == true)
                {
                  if (MessageBox.Show("Risultano paragrafi non completati o non esaminati. Procedo ugualmente con la stampa?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
                  {
                    Show();
                    Activate();
                    return;
                  }
                }
                dati = mf.GetRelazioneVC(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                wl.StampaRelazioneGenerica = true;
                wl.StampaRelazioneVigilanza = true;
                wl.StampaTemporanea = StampaTemporanea;
                wl.tipologiaBilancio = tipologiaBilancio;
                wl.tipoBilancio = tipoBilancio;
                wl.htCliente = cliente;
                wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true, IDCliente);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["Data"].ToString());
                wl.Save(Intestazione);
                StampaTemporanea = false;
              }
              break;
            case App.TipoFile.RelazioneBV:
              string FileBilancioBV = mf.GetBilancioAssociatoFromRelazioneBVFile(SelectedDataSource);
              if (FileBilancioBV != "" && (new FileInfo(FileBilancioBV)).Exists)
              {
                XmlDataProviderManager _xBV = new XmlDataProviderManager(FileBilancioBV);
                if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']") != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"] != null)
                {
                  tipologiaBilancio = "Ordinario";
                  tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"].Value;
                }
                else
                {
                  if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']") != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"] != null)
                  {
                    tipologiaBilancio = "Abbreviato";
                    tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"].Value;
                  }
                }
              }
              if (item.Content.ToString() == "Stampa Relazione")
              {
                if (StampaTemporanea == false && RecursiveCheckDaCompletarePresenti(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node")) == true)
                {
                  if (MessageBox.Show("Risultano paragrafi non completati o non esaminati. Procedo ugualmente con la stampa?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
                  {
                    Show();
                    Activate();
                    return;
                  }
                }
                XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='3']");
                if (NodoDato != null && (NodoDato.Attributes["Stato"] == null || NodoDato.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString()))
                {
                  if (NodoDato.Attributes["Stato"] == null)
                  {
                    XmlAttribute attr2 = NodoDato.OwnerDocument.CreateAttribute("Stato");
                    attr2.Value = "-1";
                    NodoDato.Attributes.Append(attr2);
                  }
                  NodoDato.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString();
                }
                NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='4']");
                if (NodoDato != null && (NodoDato.Attributes["Stato"] == null || NodoDato.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString()))
                {
                  if (NodoDato.Attributes["Stato"] == null)
                  {
                    XmlAttribute attr2 = NodoDato.OwnerDocument.CreateAttribute("Stato");
                    attr2.Value = "-1";
                    NodoDato.Attributes.Append(attr2);
                  }
                  NodoDato.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString();
                }
                _x.Save();
                dati = mf.GetRelazioneBV(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                wl.StampaRelazioneGenerica = true;
                wl.StampaRelazioneBilancioeVigilanza = true;
                wl.StampaTemporanea = StampaTemporanea;
                wl.tipologiaBilancio = tipologiaBilancio;
                wl.tipoBilancio = tipoBilancio;
                wl.htCliente = cliente;
                wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true, IDCliente);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["Data"].ToString());
                wl.Save(Intestazione);
                StampaTemporanea = false;
              }
              break;
            case App.TipoFile.PianificazioniVerifica:
              if (item.Content.ToString() == "Stampa Pianificazione")
              {
                dati = mf.GetPianificazioniVerifica(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                wl.Watermark = false;
                wl.TitoloVerbale = false;
                wl.TitoloPianificazione = true;
                //wl.StampaTemporanea = true;
                wl.StampaTemporanea = false; // così si può scegliere la cartella di destinazione
                wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true, IDCliente);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                wl.Save("");
              }
              break;
            case App.TipoFile.Verifica:
              if (item.Content.ToString() == "Stampa Anteprima")
              {
                dati = mf.GetVerifica(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                wl.Watermark = false;
                wl.TabelleSenzaRigheVuote = true;
                wl.SenzaStampareTitoli = true;
                wl.StampaTemporanea = true;
                wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true, IDCliente);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                istobecompleteforprinting = false;
                if (RecursiveCheckComplete(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node")))
                {
                  wl.AddTitleDaCompletare();
                  RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                }
                istobecompleteforprinting = true;
                //ArrayList alVigilanze = mf.GetVigilanze(IDCliente);
                //foreach (Hashtable datiVigilanza in alVigilanze)
                //{
                //    if (datiVigilanza["Data"].ToString() == dati["Data"].ToString())
                //    {
                //        RevisoftApplication.XmlManager x = new XmlManager();
                //        x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                //        XmlDataProvider TreeXmlProviderVigilanza = new XmlDataProvider();
                //        string SelectedTreeSourceVigilanza = App.AppDataDataFolder + "\\" + datiVigilanza["File"].ToString();
                //        string SelectedDataSourceVigilanza = App.AppDataDataFolder + "\\" + datiVigilanza["FileData"].ToString();
                //        TreeXmlProviderVigilanza.Document = x.LoadEncodedFile(SelectedTreeSourceVigilanza);
                //        if (TreeXmlProviderVigilanza.Document.SelectSingleNode("/Tree/Node") != null)
                //        {
                //            RecursiveNode(TreeXmlProviderVigilanza.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSourceVigilanza);
                //        }
                //    }
                //}
                //wl.LastParagraph(dati);
                wl.Save("");
              }
              else if (item.Content.ToString() == "Stampa Carte di Lavoro Vuote")
              {
                printall = true;
                printall_excludednodes.Clear();
                printall_excludednodes.Add("4.2.16");
                printall_excludednodes.Add("4.2.17");
                printall_excludednodes.Add("4.2.21");
                printall_excludednodes.Add("4.3.4");
                printall_excludednodes.Add("4.5.11");
                printall_excludednodes.Add("4.7.1");
                printall_excludednodes.Add("4.7.2");
                printall_excludednodes.Add("4.9.1");
                printall_excludednodes.Add("4.9.2");
                printall_excludednodes.Add("4.9.3");
                printall_excludednodes.Add("4.9.4");
                printall_excludednodes.Add("4.9.5");
                printall_excludednodes.Add("4.10.1");
                printall_excludednodes.Add("4.10.8");
                printall_excludednodes.Add("4.11.1");
                printall_excludednodes.Add("4.11.2");
                printall_excludednodes.Add("4.12.1");
                printall_excludednodes.Add("4.12.2");
                printall_excludednodes.Add("4.13.1");
                printall_excludednodes.Add("4.14.1");
                printall_excludednodes.Add("4.14.6");
                printall_excludednodes.Add("4.15");
                printall_excludednodes.Add("4.31.1");
                printall_excludednodes.Add("4.31.2");
                printall_excludednodes.Add("4.97");
                printall_excludednodes.Add("4.98");
                printall_excludednodes.Add("4.99");
                printall_nodesnow = new List<string>();
                RecursiveNodeOnlyCodes(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"));
                wSceltaNodiStampaVuota snlv = new wSceltaNodiStampaVuota(printall_nodesnow);
                snlv.ShowDialog();
                if (snlv.isok == false)
                {
                  Show();
                  Activate();
                  return;
                }
                printall_excludednodes.AddRange(snlv.listahere);
                dati = mf.GetVerifica(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                wl.Watermark = false;
                wl.printall = true;
                wl.TabelleSenzaRigheVuote = true;
                wl.SenzaStampareTitoli = true;
                //wl.StampaTemporanea = true;
                wl.StampaTemporanea = false;
                wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true, IDCliente);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                wl.Save("");
                printall = false;
              }
              break;
            case App.TipoFile.PianificazioniVigilanza:
              if (item.Content.ToString() == "Stampa Pianificazione")
              {
                dati = mf.GetPianificazioniVigilanza(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                wl.Watermark = false;
                wl.TitoloVerbale = false;
                wl.TitoloPianificazione = true;
                //wl.StampaTemporanea = true;
                wl.StampaTemporanea = false; // così si può scegliere la cartella di destinazione
                wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true, IDCliente);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                wl.Save("");
              }
              break;
            case App.TipoFile.Vigilanza:
              if (item.Content.ToString() == "Stampa Anteprima")
              {
                dati = mf.GetVigilanza(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                wl.Watermark = false;
                wl.TabelleSenzaRigheVuote = true;
                wl.SenzaStampareTitoli = true;
                //wl.StampaTemporanea = true;
                wl.StampaTemporanea = false; // così si può scegliere la cartella di destinazione
                wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true, IDCliente);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                istobecompleteforprinting = false;
                if (RecursiveCheckComplete(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node")))
                {
                  wl.AddTitleDaCompletare();
                  RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                }
                istobecompleteforprinting = true;
                //ArrayList alVerifiche = mf.GetVerifiche(IDCliente);
                //foreach (Hashtable datiVerifica in alVerifiche)
                //{
                //    if (datiVerifica["Data"].ToString() == dati["Data"].ToString())
                //    {
                //        RevisoftApplication.XmlManager x = new XmlManager();
                //        x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                //        XmlDataProvider TreeXmlProviderVerifica = new XmlDataProvider();
                //        string SelectedTreeSourceVerifica = App.AppDataDataFolder + "\\" + datiVerifica["File"].ToString();
                //        string SelectedDataSourceVerifica = App.AppDataDataFolder + "\\" + datiVerifica["FileData"].ToString();
                //        TreeXmlProviderVerifica.Document = x.LoadEncodedFile(SelectedTreeSourceVerifica);
                //        if (TreeXmlProviderVerifica.Document.SelectSingleNode("/Tree/Node") != null)
                //        {
                //            RecursiveNode(TreeXmlProviderVerifica.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSourceVerifica);
                //        }
                //    }
                //}
                //wl.LastParagraph(dati);
                wl.Save("");
              }
              break;
            case App.TipoFile.Incarico:
            case App.TipoFile.IncaricoCS:
            case App.TipoFile.IncaricoSU:
            case App.TipoFile.IncaricoREV:

              if (item.Content.ToString() == "Stampa Fascicolo")
              {

                StampaLetteraIncarico = false;
                dati = mf.GetIncarico(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampa;
                wl.Fascicolo = true;
                wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true, IDCliente);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["DataNomina"].ToString());

                wl.SavePDF(Intestazione, this);
              }
              if (item.Content.ToString() == "Stampa Lettera di Incarico Collegio")
              {
                btn_StampaLetteraIncaricoCollegio_Click(sender, e);
              }
              if (item.Content.ToString() == "Stampa Lettera di Incarico Soggetto Unico")
              {
                btn_StampaLetteraIncarico_Click(sender, e);
              }
              break;
            case App.TipoFile.ISQC:
              if (item.Content.ToString() == "Stampa Fascicolo")
              {

                StampaCodiceEtico = false;
                dati = mf.GetISQC(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampa;
                wl.Fascicolo = true;
                wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true, IDCliente);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["DataNomina"].ToString());

                wl.SavePDF(Intestazione, this);
              }
              if (item.Content.ToString() == "Stampa Codice Etico")
              {
                btn_StampaCodiceEtico_Click(sender, e);
              }
              break;
            case App.TipoFile.Bilancio:
              if (item.Content.ToString() == "Stampa Fascicolo")
              {

                dati = mf.GetBilancio(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampa;
                wl.Fascicolo = true;
                wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true, IDCliente);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["Data"].ToString());

                wl.SavePDF(Intestazione, this);
              }
              break;
            case App.TipoFile.Conclusione:
              if (item.Content.ToString() == "Stampa Fascicolo")
              {

                StampaLetteraAttestazione = false;
                StampaManagementLetter = false;
                dati = mf.GetConclusione(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampa;
                wl.Fascicolo = true;
                wl.Open(
                  dati,
                  cliente["RagioneSociale"].ToString(),
                  cliente["CodiceFiscale"].ToString(),
                  selectedAliasCodificato,
                  TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value,
                  false, true, IDCliente);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["Data"].ToString());

                wl.SavePDF(Intestazione, this);
              }
              if (item.Content.ToString() == "Stampa Lettera di Attestazione")
              {
                btn_StampaLetteraAttestazione_Click(sender, e);
              }
              if (item.Content.ToString() == "Stampa Management Letter")
              {
                btn_StampaManagementLetter_Click(sender, e);
              }
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
            default: break;
          }
          wl.Close();

        }
      }
      Show();
    }

    //----------------------------------------------------------------------------+
    //                                ConvertDate                                 |
    //----------------------------------------------------------------------------+
    private string ConvertDate(string date)
    {
      date = date.ToString().Replace("01/01/", "");
      date = date.ToString().Contains("31/12/") ? date.ToString().Replace("31/12/", "") + " / " + (Convert.ToInt32(date.ToString().Replace("31/12/", "")) + 1).ToString() : date;
      return date;
    }

    bool istobecompleteforprinting = true;

    //----------------------------------------------------------------------------+
    //                           RecursiveCheckComplete                           |
    //----------------------------------------------------------------------------+
    private bool RecursiveCheckComplete(XmlNode node)
    {
      bool returnvalue = false;
      if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
      {
        if (node.Attributes["ID"] != null)
        {
          XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");
          if (node.Attributes["Report"] != null && NodoDato != null && NodoDato.Attributes["Stato"] != null)
          {
            if (node.Attributes["Report"].Value == "True" || (NodoDato.Attributes["Stato"] != null && NodoDato.Attributes["Stato"].Value == ((istobecompleteforprinting) ? ((Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString()) : ((Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString()))))
            {
              return true;
            }
          }
        }
      }
      else
      {
        // if (node.ParentNode.Name != "Tree")
        {
          foreach (XmlNode item in node.ChildNodes)
          {
            if (item.Name == "Node")
            {
              returnvalue = RecursiveCheck(item);
              if (returnvalue)
              {
                return true;
              }
            }
          }
        }
      }
      return returnvalue;
    }

    //----------------------------------------------------------------------------+
    //                               RecursiveCheck                               |
    //----------------------------------------------------------------------------+
    private bool RecursiveCheck(XmlNode node)
    {
      bool returnvalue = false;
      if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
      {
        if (node.Attributes["ID"] != null)
        {
          if (printall) return true;

          string stato = cBusinessObjects.GetStato(int.Parse(node.Attributes["ID"].Value), IDCliente, IDSessione);

          if (node.Attributes["Report"].Value == "True" || (stato != "" && stato == ((istobecompleteforprinting) ? ((Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString()) : ((Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString()))))
          {
            return true;
          }

          //    XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");
          //  if (node.Attributes["Report"] != null && NodoDato != null && NodoDato.Attributes["Stato"] != null)
          //        {
          //        if (node.Attributes["Report"].Value == "True" || (NodoDato.Attributes["Stato"] != null && NodoDato.Attributes["Stato"].Value == ((istobecompleteforprinting) ? ((Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString()) : ((Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString()))))
          // {
          //        return true;
          //      }
          //      }
        }
      }
      else
      {
        if (node.ParentNode.Name != "Tree")
        {
          foreach (XmlNode item in node.ChildNodes)
          {
            if (item.Name == "Node")
            {
              returnvalue = RecursiveCheck(item);
              if (returnvalue)
              {
                return true;
              }
            }
          }
        }
      }
      return returnvalue;
    }

    //----------------------------------------------------------------------------+
    //                     RecursiveCheckDaCompletarePresenti                     |
    //----------------------------------------------------------------------------+
    private bool RecursiveCheckDaCompletarePresenti(XmlNode node)
    {
      bool returnvalue = false;
      if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
      {
        if (node.Attributes["ID"] != null)
        {
          if (printall)
          {
            return true;
          }
          XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");
          if (NodoDato != null && NodoDato.Attributes["Stato"] != null)
          {
            if ((NodoDato.Attributes["Stato"] != null && (NodoDato.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString() || NodoDato.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString())))
            {
              if ((IDTree == "21" && NodoDato.Attributes["ID"].Value == "17" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='18']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='19']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='20']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "18" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='17']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='19']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='20']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "19" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='17']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='18']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='20']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "20" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='17']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='18']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='19']").Attributes["Stato"].Value == "2")))
              {
                return false;
              }
              if ((IDTree == "21" && NodoDato.Attributes["ID"].Value == "5" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='30']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='31']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "30" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='5']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='31']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "31" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='5']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='30']").Attributes["Stato"].Value == "2")))
              {
                return false;
              }
              if ((IDTree == "21" && NodoDato.Attributes["ID"].Value == "10" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='11']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='27']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='33']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "11" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='10']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='27']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='33']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "27" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='10']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='11']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "28" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='10']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='11']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "33" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='10']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='11']").Attributes["Stato"].Value == "2")))
              {
                return false;
              }
              if ((IDTree == "21" && NodoDato.Attributes["ID"].Value == "28" && _x.Document.SelectSingleNode("/Dati//Dato[@ID='33']").Attributes["Stato"].Value == "2") || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "33" && _x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2"))
              {
                return false;
              }
              if ((IDTree == "22" && NodoDato.Attributes["ID"].Value == "28" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='34']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='35']").Attributes["Stato"].Value == "2")) || (IDTree == "22" && NodoDato.Attributes["ID"].Value == "34" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='35']").Attributes["Stato"].Value == "2")) || (IDTree == "22" && NodoDato.Attributes["ID"].Value == "35" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='34']").Attributes["Stato"].Value == "2")))
              {
                return false;
              }
              //Albero B + V
              if ((IDTree == "23" && NodoDato.Attributes["ID"].Value == "117" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='118']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='119']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='120']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "118" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='117']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='119']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='120']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "119" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='117']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='118']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='120']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "120" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='117']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='118']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='119']").Attributes["Stato"].Value == "2")))
              {
                return false;
              }
              if ((IDTree == "23" && NodoDato.Attributes["ID"].Value == "105" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='130']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='131']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "130" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='105']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='131']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "131" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='105']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='130']").Attributes["Stato"].Value == "2")))
              {
                return false;
              }
              if ((IDTree == "23" && NodoDato.Attributes["ID"].Value == "110" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='111']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='127']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='128']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='133']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "111" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='110']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='127']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='128']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='133']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "127" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='110']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='111']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "128" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='110']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='111']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "133" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='110']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='111']").Attributes["Stato"].Value == "2")))
              {
                return false;
              }
              if ((IDTree == "23" && NodoDato.Attributes["ID"].Value == "128" && _x.Document.SelectSingleNode("/Dati//Dato[@ID='133']").Attributes["Stato"].Value == "2") || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "133" && _x.Document.SelectSingleNode("/Dati//Dato[@ID='128']").Attributes["Stato"].Value == "2"))
              {
                return false;
              }
              if ((IDTree == "23" && NodoDato.Attributes["ID"].Value == "228" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='234']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='235']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "234" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='228']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='235']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "235" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='228']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='234']").Attributes["Stato"].Value == "2")))
              {
                return false;
              }
              //if ((IDTree == "3" && NodoDato.Attributes["ID"].Value == "161" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='164']").Attributes["Stato"].Value == "2")) || (IDTree == "3" && NodoDato.Attributes["ID"].Value == "164" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='161']").Attributes["Stato"].Value == "2")))
              //{
              //    return false;
              //}
              //if ((IDTree == "21" && NodoDato.Attributes["ID"].Value == "17" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='18']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='19']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='20']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "18" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='17']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='19']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='20']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "19" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='17']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='18']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='20']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "20" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='17']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='18']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='19']").Attributes["Stato"].Value == "2")))
              //{
              //    return false;
              //}
              //if ((IDTree == "21" && NodoDato.Attributes["ID"].Value == "5" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='30']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='31']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "30" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='5']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='31']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "31" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='5']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='30']").Attributes["Stato"].Value == "2")))
              //{
              //    return false;
              //}
              //if ((IDTree == "21" && NodoDato.Attributes["ID"].Value == "10" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='11']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='27']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='33']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "11" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='10']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='27']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='23']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "27" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='10']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='11']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "28" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='10']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='11']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "33" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='10']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='11']").Attributes["Stato"].Value == "2")))
              //{
              //    return false;
              //}
              //if ((IDTree == "21" && NodoDato.Attributes["ID"].Value == "28" && _x.Document.SelectSingleNode("/Dati//Dato[@ID='33']").Attributes["Stato"].Value == "2") || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "33" && _x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2"))
              //{
              //    return false;
              //}
              //if ((IDTree == "22" && NodoDato.Attributes["ID"].Value == "28" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='34']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='35']").Attributes["Stato"].Value == "2")) || (IDTree == "22" && NodoDato.Attributes["ID"].Value == "34" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='35']").Attributes["Stato"].Value == "2")) || (IDTree == "22" && NodoDato.Attributes["ID"].Value == "35" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='34']").Attributes["Stato"].Value == "2")))
              //{
              //    return false;
              //}
              return true;
            }
          }
        }
      }
      else
      {
        //if (node.ParentNode.Name != "Tree")
        {
          foreach (XmlNode item in node.ChildNodes)
          {
            if (item.Name == "Node")
            {
              returnvalue = RecursiveCheckDaCompletarePresenti(item);
              if (returnvalue)
              {
                return true;
              }
            }
          }
        }
      }
      return returnvalue;
    }

    //----------------------------------------------------------------------------+
    //                          RecursiveNodeCheckFigli                           |
    //----------------------------------------------------------------------------+
    private bool RecursiveNodeCheckFigli(XmlNode node, RTFLib wl)
    {
      bool returnvalue = false;
      if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
      {
        if (RecursiveCheck(node))
        {

          returnvalue = wl.AddCheck(node);
        }
      }
      else
      {
        if (node.ParentNode.Name == "Tree" || RecursiveCheck(node))
        {
          foreach (XmlNode item in node.ChildNodes)
          {
            if (item.Name == "Node")
            {
              returnvalue = RecursiveNodeCheckFigli(item, wl);
              if (returnvalue == true)
              {
                break;
              }
            }
          }
        }
      }
      return returnvalue;
    }

    //----------------------------------------------------------------------------+
    //                          RecursiveNodeCheckFigli                           |
    //----------------------------------------------------------------------------+
    private bool RecursiveNodeCheckFigli(XmlNode node)
    {
      bool returnvalue = false;
      if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
      {
        if (RecursiveCheck(node))
        {
          XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");
          returnvalue = true;
        }
      }
      else
      {
        if (node.ParentNode.Name == "Tree" || RecursiveCheck(node))
        {
          foreach (XmlNode item in node.ChildNodes)
          {
            if (item.Name == "Node")
            {
              returnvalue = RecursiveNodeCheckFigli(item);
              if (returnvalue == true)
              {
                break;
              }
            }
          }
        }
      }
      return returnvalue;
    }

    public bool printall = false;
    public List<string> printall_excludednodes = new List<string>();
    public List<string> printall_nodesnow = new List<string>();

    //----------------------------------------------------------------------------+
    //                           RecursiveNodeOnlyCodes                           |
    //----------------------------------------------------------------------------+
    private void RecursiveNodeOnlyCodes(XmlNode node)
    {
      if (printall)
      {
        if (printall_excludednodes.Contains(node.Attributes["Codice"].Value))
        {
          return;
        }
      }
      if (StampaLetteraIncarico == false && (node.Attributes["ID"].Value == "142" || node.Attributes["ID"].Value == "2016142") && IDTree == "3")
      {
        return;
      }
      if (StampaCodiceEtico == false && node.Attributes["ID"].Value == "142" && IDTree == "28")
      {
        return;
      }
      if (StampaLetteraAttestazione == false && node.Attributes["ID"].Value == "261" && IDTree == "19")
      {
        return;
      }
      if (StampaManagementLetter == false && node.Attributes["ID"].Value == "281" && IDTree == "19")
      {
        return;
      }
      if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
      {
        if (RecursiveCheck(node) || (node.Attributes["ID"].Value == "100013" && IDTree == "26") || (node.Attributes["ID"].Value == "100003" && IDTree == "27"))
        {
          XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");
          if (NodoDato != null)
          {
            printall_nodesnow.Add(((node.Attributes["Codice"] != null) ? node.Attributes["Codice"].Value : "") + "|" + ((node.Attributes["Titolo"] != null) ? node.Attributes["Titolo"].Value : ""));
          }
        }
      }
      else
      {
        if (node.ParentNode.Name == "Tree" || RecursiveCheck(node))
        {
          if (node.ParentNode.Name != "Tree" && (RecursiveNodeCheckFigli(node) == true || node.Attributes["Codice"].Value == "95.311"))
          {
            if (StampaLetteraIncarico == true && (node.Attributes["ID"].Value == "150" || node.Attributes["ID"].Value == "154"))
            {
              ;
            }
            else if (StampaLetteraAttestazione == true && node.Attributes["ID"].Value == "269")
            {
              ;
            }
            else if (StampaManagementLetter == true && node.Attributes["ID"].Value == "269")
            {
              ;
            }
            else
            {
              ;// printall_nodesnow.Add(((node.Attributes["Codice"] != null) ? node.Attributes["Codice"].Value : "") + "|" + ((node.Attributes["Titolo"] != null) ? node.Attributes["Titolo"].Value : ""));
            }
          }
          foreach (XmlNode item in node.ChildNodes)
          {
            if (item.Name == "Node")
            {
              RecursiveNodeOnlyCodes(item);
            }
          }
        }
      }
    }

    //----------------------------------------------------------------------------+
    //                               RecursiveNode                                |
    //----------------------------------------------------------------------------+
    private void RecursiveNode(XmlNode node, RTFLib wl, string nomefile)
    {
      if (printall)
      {
        if (printall_excludednodes.Contains(node.Attributes["Codice"].Value))
        {
          return;
        }
      }
      if (StampaLetteraIncarico == false && (node.Attributes["ID"].Value == "142" || node.Attributes["ID"].Value == "2016142") && IDTree == "3")
      {
        return;
      }
      if (StampaCodiceEtico == false && node.Attributes["ID"].Value == "142" && IDTree == "28")
      {
        return;
      }
      if (StampaLetteraAttestazione == false && node.Attributes["ID"].Value == "261" && IDTree == "19")
      {
        return;
      }
      if (StampaManagementLetter == false && node.Attributes["ID"].Value == "281" && IDTree == "19")
      {
        return;
      }
      if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
      {
        if (RecursiveCheck(node) || (node.Attributes["ID"].Value == "100013" && IDTree == "26") || (node.Attributes["ID"].Value == "100003" && IDTree == "27"))
        {

          wl.Add(node, IDCliente, IDTree, IDSessione, nomefile);

        }
      }
      else
      {
        if (node.ParentNode.Name == "Tree" || RecursiveCheck(node))
        {
          if (node.ParentNode.Name != "Tree" && (RecursiveNodeCheckFigli(node, wl) == true || node.Attributes["Codice"].Value == "95.311"))
          {
            if (StampaLetteraIncarico == true && (node.Attributes["ID"].Value == "150" || node.Attributes["ID"].Value == "154"))
            {
              wl.AddTitleLetteraIncarico(node.Attributes["Titolo"].Value, true);
            }
            else if (StampaLetteraAttestazione == true && node.Attributes["ID"].Value == "269")
            {
              wl.AddTitleLetteraAttestazione(node.Attributes["Titolo"].Value, true);
            }
            else if (StampaManagementLetter == true && node.Attributes["ID"].Value == "269")
            {
              wl.AddTitleLetteraAttestazione(node.Attributes["Titolo"].Value, true);
            }
            else
            {
              wl.AddTitle(node.Attributes["Codice"].Value + " " + node.Attributes["Titolo"].Value, true);
            }
          }
          foreach (XmlNode item in node.ChildNodes)
          {
            if (item.Name == "Node")
            {
              RecursiveNode(item, wl, nomefile);
            }
          }
        }
      }
    }

    //----------------------------------------------------------------------------+
    //                               tvMain_KeyDown                               |
    //----------------------------------------------------------------------------+
    private void tvMain_KeyDown(object sender, KeyEventArgs e)
    {
      e.Handled = true;
    }

    public int scrlollingid = 0;
    public int OLDscrlollingid = -1;

    //----------------------------------------------------------------------------+
    //                            buttonIndietro_Click                            |
    //----------------------------------------------------------------------------+
    private void buttonIndietro_Click(object sender, RoutedEventArgs e)
    {
      if (scrlollingid > 0)
      {
        scrlollingid--;
        ScrollForced();
      }
    }

    //----------------------------------------------------------------------------+
    //                     TreeViewItem_RequestBringIntoView                      |
    //----------------------------------------------------------------------------+
    private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
    {
      e.Handled = true;
    }

    //----------------------------------------------------------------------------+
    //                             buttonAvanti_Click                             |
    //----------------------------------------------------------------------------+
    private void buttonAvanti_Click(object sender, RoutedEventArgs e)
    {
      if (TreeXmlProvider.Document.SelectNodes("//Sessioni/Sessione[position()>" + (scrlollingid + 1).ToString() + "]").Count > 0)
      {
        scrlollingid++;
        ScrollForced();
      }
    }

    //----------------------------------------------------------------------------+
    //                                ScrollForced                                |
    //----------------------------------------------------------------------------+
    private void ScrollForced()
    {
      if (OLDscrlollingid != scrlollingid)
      {
        foreach (XmlNode xNode in TreeXmlProvider.Document.SelectNodes("//Sessioni/Sessione[position()>" + scrlollingid.ToString() + "]"))
        {
          if (xNode.Attributes["Visible"] == null)
          {
            XmlAttribute attr = xNode.OwnerDocument.CreateAttribute("Visible");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Visible"].Value = "True";
        }
        foreach (XmlNode xNode in TreeXmlProvider.Document.SelectNodes("//Sessioni/Sessione[position()<=" + scrlollingid.ToString() + "]"))
        {
          if (xNode.Attributes["Visible"] == null)
          {
            XmlAttribute attr = xNode.OwnerDocument.CreateAttribute("Visible");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Visible"].Value = "False";
        }
        //SaveTreeSource();
        if (TreeXmlProvider.Document != null)
        {
          RevisoftApplication.XmlManager x = new XmlManager();
          x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
          x.SaveEncodedFile(SelectedTreeSource, TreeXmlProvider.Document.OuterXml);
        }
        OLDscrlollingid = scrlollingid;
      }
    }

    //----------------------------------------------------------------------------+
    //                             Window_SizeChanged                             |
    //----------------------------------------------------------------------------+
    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      SVTreeFixed.Width = 490;
      SVTreeFixed.Height = e.NewSize.Height - 180;
      SVTreeFixed.Margin = new Thickness(0, -18, 0, 0);
      SVTree.Width = e.NewSize.Width - 135 - 490;
      SVTree.Height = e.NewSize.Height - 180;
      SVTree.Margin = new Thickness(-20, -18, 0, 0);
      SVTreeHeader.Width = e.NewSize.Width - 135 - 490;
      // SVTreeHeader.Margin = new Thickness(-50, 0, 0, 0);
      // SVTreeHeader.Padding = new Thickness(51, 0, 0, 0);
      SVTreeHeader.Margin = new Thickness(-20, 0, 0, 0);
      SVTreeHeader.Padding = new Thickness(21, 0, 0, 0);
      gridTV.Width = tvMain.Width;
      gridTVFixed.Width = tvMainFixed.Width;
      SVTree.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
      SVTreeFixed.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
      SVTree.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
      SVTreeHeader.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
    }

    //----------------------------------------------------------------------------+
    //                            SVTree_ScrollChanged                            |
    //----------------------------------------------------------------------------+
    private void SVTree_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      SVTreeHeader.ScrollToHorizontalOffset((sender as ScrollViewer).HorizontalOffset);
      SVTreeFixed.ScrollToVerticalOffset((sender as ScrollViewer).VerticalOffset);
    }

    //----------------------------------------------------------------------------+
    //                              Grid_MouseWheel                               |
    //----------------------------------------------------------------------------+
    private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      SVTree.ScrollToVerticalOffset(SVTree.VerticalOffset - e.Delta);
      e.Handled = true;
    }

    //----------------------------------------------------------------------------+
    //                      menuStrumentiStampaVerbali_Click                      |
    //----------------------------------------------------------------------------+
    private void menuStrumentiStampaVerbali_Click(object sender, RoutedEventArgs e)
    {
      wStampaVerbali wSF = new wStampaVerbali();
      wSF.selectedCliente = IDCliente;
      wSF.selectedSession = IDSessione;
      wSF.inizializza();
      wSF.Owner = this;
      wSF.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                             btn_ISQCTdL_Click                              |
    //----------------------------------------------------------------------------+
    private void btn_ISQCTdL_Click(object sender, RoutedEventArgs e)
    {
      wSceltaISCQ sq = new wSceltaISCQ(IDCliente, IDTree);
      sq.ShowDialog();
      e.Handled = true;
    }

    //----------------------------------------------------------------------------+
    //                         menuStrumentiCopiaDa_Click                         |
    //----------------------------------------------------------------------------+

    private void menuStrumentiCopiaDa_Click(object sender, RoutedEventArgs e)
    {


      DataTable datifrom = null;

      if (MessageBox.Show("Sicuri di voler importare i valori? I dati " +
        "attualmente presenti verranno cancellati.", "Attenzione",
        MessageBoxButton.YesNo) == MessageBoxResult.No)
      {
        return;
      }
      string IDHere = ((XmlNode)(tvMain.SelectedItem)).Attributes["ID"].Value;
      string nodeID = "";

      if ((IDTree == "2" && (IDHere == "20" || IDHere == "21" || IDHere == "22"
        || IDHere == "23" || IDHere == "24" || IDHere == "146")))
      {
        nodeID = "";
        switch (IDHere)
        {
          case "20":
            nodeID = "600";
            break;
          case "21":
            nodeID = "601";
            break;
          case "22":
            nodeID = "602";
            break;
          case "23":
            nodeID = "603";
            break;
          case "24":
            nodeID = "604";
            break;
          case "146":
            nodeID = "605";
            break;
          default:
            return;
        }

        datifrom = cBusinessObjects.GetData(int.Parse(nodeID), typeof(Tabella), -1, -1, 18);
        cBusinessObjects.SaveData(int.Parse(IDHere), datifrom, typeof(Tabella));

        int lastid = 1;

        DataTable tempdt = cBusinessObjects.ExecutesqlDataTable("SELECT MAX(ID) AS LASTID FROM ArchivioDocumenti");
        foreach (DataRow dd in tempdt.Rows)
        {
          if (dd["LASTID"].ToString() != "")
            lastid = int.Parse(dd["LASTID"].ToString()) + 1;
        }
        DataTable documentifrom = cBusinessObjects.GetData(int.Parse(nodeID), typeof(ArchivioDocumenti), -1, -1, 18);


        foreach (DataRow dtro in documentifrom.Rows)
        {
          if (dtro["Tipo"].ToString() != (Convert.ToInt32(TipoDocumento.Permanente)).ToString())
            continue;
          string oldfile = dtro["file"].ToString();
          dtro["file"] = dtro["file"].ToString().Replace(dtro["ID"].ToString(), lastid.ToString());
          File.Copy(App.AppDocumentiFolder + "\\" + oldfile, App.AppDocumentiFolder + "\\" + lastid + "_" + dtro["file"].ToString());
          dtro["ID"] = lastid;
          lastid++;

        }
        cBusinessObjects.SaveData(int.Parse(IDHere), documentifrom, typeof(ArchivioDocumenti));

      }

      if ((IDTree == "18" && (IDHere == "600" || IDHere == "601" || IDHere == "602"
       || IDHere == "603" || IDHere == "604" || IDHere == "605")))
      {
        nodeID = "";
        switch (IDHere)
        {
          case "600":
            nodeID = "20";
            break;
          case "601":
            nodeID = "21";
            break;
          case "602":
            nodeID = "22";
            break;
          case "603":
            nodeID = "23";
            break;
          case "604":
            nodeID = "24";
            break;
          case "605":
            nodeID = "146";
            break;
          default:
            return;
        }
        datifrom = cBusinessObjects.GetData(int.Parse(nodeID), typeof(Tabella), -1, -1, 2);
        cBusinessObjects.SaveData(int.Parse(IDHere), datifrom, typeof(Tabella));
        int lastid = 1;

        DataTable tempdt = cBusinessObjects.ExecutesqlDataTable("SELECT MAX(ID) AS LASTID FROM ArchivioDocumenti");
        foreach (DataRow dd in tempdt.Rows)
        {
          if (dd["LASTID"].ToString() != "")
            lastid = int.Parse(dd["LASTID"].ToString()) + 1;
        }
        DataTable documentifrom = cBusinessObjects.GetData(int.Parse(nodeID), typeof(ArchivioDocumenti), -1, -1, 2);


        foreach (DataRow dtro in documentifrom.Rows)
        {
          if (dtro["Tipo"].ToString() != (Convert.ToInt32(TipoDocumento.Permanente)).ToString())
            continue;
          string oldfile = dtro["file"].ToString();
          dtro["file"] = dtro["file"].ToString().Replace(dtro["ID"].ToString(), lastid.ToString());
          File.Copy(App.AppDocumentiFolder + "\\" + oldfile, App.AppDocumentiFolder + "\\" + lastid + "_" + dtro["file"].ToString());
          dtro["ID"] = lastid;
          lastid++;

        }
        cBusinessObjects.SaveData(int.Parse(IDHere), documentifrom, typeof(ArchivioDocumenti));
      }
      MessageBox.Show("Dati Importati con sucesso");
    }

    //----------------------------------------------------------------------------+
    //                             btn_Espandi_Click                              |
    //----------------------------------------------------------------------------+
    private void btn_Espandi_Click(object sender, RoutedEventArgs e)
    {
      if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
      {
        if (txt_Espandi.Text == "Espandi")
        {
          txt_Espandi.Text = "Chiudi";
          var uriSource = new Uri("./Images/icone/navigate_open.png", UriKind.Relative);
          img_Espandi.Source = new BitmapImage(uriSource);
          foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
          {
            if (item.Attributes["Selected"] != null)
            {
              item.Attributes["Selected"].Value = "False";
            }
            if (item.Attributes["Expanded"] != null)
            {
              item.Attributes["Expanded"].Value = "True";
            }
            if (item.Attributes["HighLighted"] != null)
            {
              item.Attributes["HighLighted"].Value = "Black";
            }
          }
        }
        else
        {
          txt_Espandi.Text = "Espandi";
          var uriSource = new Uri("./Images/icone/navigate_close.png", UriKind.Relative);
          img_Espandi.Source = new BitmapImage(uriSource);
          foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
          {
            if (item.Attributes["Selected"] != null)
            {
              item.Attributes["Selected"].Value = "False";
            }
            if (item.Attributes["Expanded"] != null)
            {
              if (item.ParentNode.Name == "Tree")
              {
                item.Attributes["Expanded"].Value = "True";
              }
              else
              {
                item.Attributes["Expanded"].Value = "False";
              }
            }
            if (item.Attributes["HighLighted"] != null)
            {
              item.Attributes["HighLighted"].Value = "Black";
            }
          }
        }
      }
    }
  } //------------------------ public partial class WindowWorkAreaTree : Window
} //--------------------------------------------- namespace RevisoftApplication

namespace ConvNS
{
  [ValueConversion(typeof(string), typeof(string))]
  public class TypeVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

      string tipo = (string)value;

      if (tipo != "Nodo Multiplo")
      {
        return "Visible";
      }
      else
      {
        return "Collapsed";
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class convertwithnewline : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

      string temp = (string)value;
      string[] temparr = temp.Split('|');
      if (temparr.Count() == 1)
        return value;
      string res = "";
      foreach (string a in temparr)
      {
        res = res + a + Environment.NewLine;
      }
      return res;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class TypeVisibilityConverterifempty : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ((value == null) || (value == System.DBNull.Value))
        return "Visible";
      string tipo = (string)value;

      if (tipo != "")
      {
        return "Visible";
      }
      else
      {
        return "Hidden";
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class boolVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ((value == null) || (value == System.DBNull.Value))
        return "Visible";

      string tipo = (string)value;


      if (tipo == "True")
      {
        return "Visible";
      }
      else
      {
        return "Collapsed";
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class FontWeightConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ((value != null) && (value != System.DBNull.Value))
      {
        if (value.ToString() == "True")
        {
          return "Bold";
        }
        else
        {
          return "Regular";
        }
      }
      else
      {
        return "Regular";
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class IsTabStopConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ((value != null) && (value != System.DBNull.Value))
      {
        return "False";
      }
      else
      {
        return "True";
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class VisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ((value != null) && (value != System.DBNull.Value))
      {
        return "Hidden";
      }
      else
      {
        return "Visible";
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class Money : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      culture = CultureInfo.CreateSpecificCulture("it-IT");
      double tmpvalue = 0.0;

      double.TryParse(value.ToString(), out tmpvalue);
      if (tmpvalue == 0.0)
      {
        return "";
      }
      else
      {
        return String.Format("{0:#,0.00}", tmpvalue);
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      culture = CultureInfo.CreateSpecificCulture("it-IT");
      double tmpvalue = 0.0;

      if (!value.ToString().Contains(','))
      {
        if (value.ToString().Split('.').Length == 2 && value.ToString().Split('.')[1].Length != 3)
        {
          value = value.ToString().Replace('.', ',');
        }
      }
      double.TryParse(value.ToString(), out tmpvalue);
      //tmpvalue = System.Convert.ToDouble(System.Convert.ToString(value).Replace("€ ", ""));
      return String.Format("{0:#,0.00}", tmpvalue);
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class Money2 : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

      double tmpvalue = 0.0;
      double.TryParse(value.ToString(), out tmpvalue);
      if (tmpvalue == 0.0)
      {
        return "0";
      }
      else
      {
        return String.Format("{0:#,0.00000}", tmpvalue);
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {

      double tmpvalue = 0.0;
      if (!value.ToString().Contains(','))
      {
        if (value.ToString().Split('.').Length == 2 && value.ToString().Split('.')[1].Length != 3)
        {
          value = value.ToString().Replace('.', ',');
        }
      }
      double.TryParse(value.ToString(), out tmpvalue);
      //tmpvalue = System.Convert.ToDouble(System.Convert.ToString(value).Replace("€ ", ""));
      return String.Format("{0:#,0.00000}", tmpvalue);
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class MoneyWithZero : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

      double tmpvalue = 0.0;
      double.TryParse(value.ToString(), out tmpvalue);
      return String.Format("{0:#,0.00}", tmpvalue);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {

      double tmpvalue = 0.0;
      if (!value.ToString().Contains(','))
      {
        if (value.ToString().Split('.').Length == 2 && value.ToString().Split('.')[1].Length != 3)
        {
          value = value.ToString().Replace('.', ',');
        }
      }
      double.TryParse(value.ToString(), out tmpvalue);
      //tmpvalue = System.Convert.ToDouble(System.Convert.ToString(value).Replace("€ ", ""));
      return String.Format("{0:#,0.00}", tmpvalue);
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class MoneyNodecimal : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      culture = CultureInfo.CreateSpecificCulture("it-IT");
      double tmpvalue = 0.0;
      double.TryParse(value.ToString(), out tmpvalue);
      if (tmpvalue == 0.0)
      {
        return "";
      }
      else
      {
        return String.Format("{0:#,0}", tmpvalue);
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {

      double tmpvalue = 0.0;
      if (!value.ToString().Contains(','))
      {
        if (value.ToString().Split('.').Length == 2 && value.ToString().Split('.')[1].Length != 3)
        {
          value = value.ToString().Replace('.', ',');
        }
      }
      double.TryParse(value.ToString(), out tmpvalue);
      //tmpvalue = System.Convert.ToDouble(System.Convert.ToString(value).Replace("€ ", ""));
      return String.Format("{0:#,0}", tmpvalue);
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class Integer : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

      double tmpvalue = 0.0;
      double.TryParse(value.ToString(), out tmpvalue);
      if (tmpvalue == 0.0)
      {
        return "";
      }
      else
      {
        return String.Format("{0:#,0}", tmpvalue);
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {

      double tmpvalue = 0.0;
      if (!value.ToString().Contains(','))
      {
        if (value.ToString().Split('.').Length == 2 && value.ToString().Split('.')[1].Length != 3)
        {
          value = value.ToString().Replace('.', ',');
        }
      }
      double.TryParse(value.ToString(), out tmpvalue);
      //tmpvalue = System.Convert.ToDouble(System.Convert.ToString(value).Replace("€ ", ""));
      return String.Format("{0:#,0.00}", tmpvalue);
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class Percent : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

      double tmpvalue = 0.0;
      double.TryParse(value.ToString(), out tmpvalue);
      tmpvalue = tmpvalue * 100.0;
      if (tmpvalue == 0.0)
      {
        return "";
      }
      else
      {
        return String.Format("{0:0.00} %", tmpvalue);
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {

      double tmpvalue = 0.0;
      value = value.ToString().Replace(" %", "");
      double.TryParse(value.ToString(), out tmpvalue);
      return String.Format("{0:0.00}", tmpvalue);
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class BackgroundConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ((value != null) && (value != System.DBNull.Value))
      {
        return Brushes.LightGray;
      }
      else
      {
        return Brushes.Transparent;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class BackgroundColorConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      int alternateCount = (int)value;
      if (alternateCount % 2 == 0)
      {
        return "#ffffff";
      }
      else
      {
        return "#ccccff";
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class IconeSospesiConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value.ToString() != "")
      {
        return ".\\Images\\icone\\Stato\\sospesi.png";
      }
      else
      {
        return ".\\Images\\icone\\Stato\\nothing.png";
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class IconeStatoConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      switch (value.ToString())
      {
        case "6":
        case "0":
          return ".\\Images\\icone\\Stato\\nonapp_small.png";
        case "1":
          return ".\\Images\\icone\\Stato\\parziale.png";
        case "2":
          return ".\\Images\\icone\\Stato\\completo.png";
        case "3":
          return ".\\Images\\icone\\Stato\\warning.png";
        case "4":
          return ".\\Images\\icone\\Stato\\check2.png";
        case "-2":
          return ".\\Images\\icone\\Stato\\nothing.png"; //return ".\\Images\\icone\\Stato\\note_pinned.png";
        case "-3":
          return ".\\Images\\icone\\Stato\\nothing.png";
        case "-5":
          return ".\\Images\\icone\\Stato\\Sigillo.png";
        case "-6":
          return ".\\Images\\icone\\Stato\\SigilloRotto.png";
        case "7":
          return ".\\Images\\icone\\Stato\\DoppiaSpunta.png";
        case "-4":
        case "-1":
        default:
          return ".\\Images\\icone\\Stato\\nothing.png";
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class TooltipStatoConverter : IMultiValueConverter
  {
    public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
    {
      //attenzione abbiamo RevisoftApplication.App.NomeTipoTreeNodeStato((RevisoftApplication.App.TipoTreeNodeStato)
      switch (((XmlAttribute)(value[0])).Value.ToString())
      {
        case "6":
        case "0":
          return "Non Applicabile";
        case "1":
          return "Da Completare";
        case "2":
          return "Completato";
        case "3":
          return "Resettato";
        case "4":
          return "In scrittura";
        case "-2":
          return "Promemoria";
        case "-3":
          return "Voci Compilate";
        case "-4":
          return "Voce in Sola Lettura";
        case "-5":
          return ((XmlAttribute)(value[1])).Value.ToString();
        case "-6":
          return ((XmlAttribute)(value[1])).Value.ToString();
        case "-1":
        default:
          return "Nessuno stato assegnato";
      }
    }

    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }


  [ValueConversion(typeof(string), typeof(string))]
  public class RadioButtonConverter_6_1 : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null || parameter == null) return false;
      switch (value.ToString())
      {
        case "Alto":
          return (parameter.ToString() == "Alto");
        case "Medio":
          return (parameter.ToString() == "Medio");
        case "Basso":
          return (parameter.ToString() == "Basso");
        case "NA":
        default:
          return (parameter.ToString() == "NA");
      }

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null || parameter == null) return null;
      bool useValue = (bool)value;
      if (useValue)
      {
        return parameter.ToString();
      }
      return null;
    }
  }




  [ValueConversion(typeof(string), typeof(string))]
  public class RadioButtonConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null || parameter == null) return false;
      switch (value.ToString())
      {
        case "Si":
          return (parameter.ToString() == "Si");
        case "No":
          return (parameter.ToString() == "No");
        case "NA":
        default:
          return (parameter.ToString() == "NA");
      }

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null || parameter == null) return null;
      bool useValue = (bool)value;
      if (useValue)
      {
        return parameter.ToString();
      }
      return null;
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class ImageNoteConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string down = "./Images/icone/navigate_down.png";
      string up = "./Images/icone/navigate_up.png";

      if ((value != null) && ((string)value == string.Empty))
      {
        return down;
      }
      else
      {
        return up;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class ImageNoteVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if ((string)value == string.Empty)
      {
        return "Collapsed";
      }
      else
      {
        return "Visible";
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }
} //---------------------------------------------------------- namespace ConvNS

/*
// srcold
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
using System.Xml.Linq;
using System.Xml.Schema;
using System.Text;
using System.IO;

namespace RevisoftApplication
{
  public static class MyExtensions
  {
    public static string ToStringAlignAttributes( this XDocument document )
    {
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Indent = true;
      settings.OmitXmlDeclaration = true;
      settings.NewLineOnAttributes = true;
      StringBuilder stringBuilder = new StringBuilder();
      using ( XmlWriter xmlWriter = XmlWriter.Create( stringBuilder, settings ) )
          document.WriteTo( xmlWriter );
      return stringBuilder.ToString();
    }
  }

    class Program
    {
        private static class Xsi
        {
            public static XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

            public static XName schemaLocation = xsi + "schemaLocation";
            public static XName noNamespaceSchemaLocation = xsi + "noNamespaceSchemaLocation";
        }

        public static XDocument Normalize( XDocument source, XmlSchemaSet schema )
        {
            bool havePSVI = false;
            // validate, throw errors, add PSVI information
            if ( schema != null )
            {
                source.Validate( schema, null, true );
                havePSVI = true;
            }
            return new XDocument(
                source.Declaration,
                source.Nodes().Select( n =>
                {
                    // Remove comments, processing instructions, and text nodes that are
                    // children of XDocument.  Only white space text nodes are allowed as
                    // children of a document, so we can remove all text nodes.
                    if ( n is XComment || n is XProcessingInstruction || n is XText )
                        return null;
                    XElement e = n as XElement;
                    if ( e != null )
                        return NormalizeElement( e, havePSVI );
                    return n;
                }
                )
            );
        }

        public static bool DeepEqualsWithNormalization( XDocument doc1, XDocument doc2,
            XmlSchemaSet schemaSet )
        {
            XDocument d1 = Normalize( doc1, schemaSet );
            XDocument d2 = Normalize( doc2, schemaSet );
            return XNode.DeepEquals( d1, d2 );
        }

        private static IEnumerable<XAttribute> NormalizeAttributes( XElement element,
            bool havePSVI )
        {
            return element.Attributes()
                    .Where( a => !a.IsNamespaceDeclaration &&
                        a.Name != Xsi.schemaLocation &&
                        a.Name != Xsi.noNamespaceSchemaLocation )
                    .OrderBy( a => a.Name.NamespaceName )
                    .ThenBy( a => a.Name.LocalName )
                    .Select(
                        a =>
                        {
                            if ( havePSVI )
                            {
                                var dt = a.GetSchemaInfo().SchemaType.TypeCode;
                                switch ( dt )
                                {
                                    case XmlTypeCode.Boolean:
                                        return new XAttribute( a.Name, (bool)a );
                                    case XmlTypeCode.DateTime:
                                        return new XAttribute( a.Name, (DateTime)a );
                                    case XmlTypeCode.Decimal:
                                        return new XAttribute( a.Name, (decimal)a );
                                    case XmlTypeCode.Double:
                                        return new XAttribute( a.Name, (double)a );
                                    case XmlTypeCode.Float:
                                        return new XAttribute( a.Name, (float)a );
                                    case XmlTypeCode.HexBinary:
                                    case XmlTypeCode.Language:
                                        return new XAttribute( a.Name,
                                            ( (string)a ).ToLower() );
                                }
                            }
                            return a;
                        }
                    );
        }

        private static XNode NormalizeNode( XNode node, bool havePSVI )
        {
            // trim comments and processing instructions from normalized tree
            if ( node is XComment || node is XProcessingInstruction )
                return null;
            XElement e = node as XElement;
            if ( e != null )
                return NormalizeElement( e, havePSVI );
            // Only thing left is XCData and XText, so clone them
            return node;
        }

        private static XElement NormalizeElement( XElement element, bool havePSVI )
        {
            if ( havePSVI )
            {
                var dt = element.GetSchemaInfo();
                switch ( dt.SchemaType.TypeCode )
                {
                    case XmlTypeCode.Boolean:
                        return new XElement( element.Name,
                            NormalizeAttributes( element, havePSVI ),
                            (bool)element );
                    case XmlTypeCode.DateTime:
                        return new XElement( element.Name,
                            NormalizeAttributes( element, havePSVI ),
                            (DateTime)element );
                    case XmlTypeCode.Decimal:
                        return new XElement( element.Name,
                            NormalizeAttributes( element, havePSVI ),
                            (decimal)element );
                    case XmlTypeCode.Double:
                        return new XElement( element.Name,
                            NormalizeAttributes( element, havePSVI ),
                            (double)element );
                    case XmlTypeCode.Float:
                        return new XElement( element.Name,
                            NormalizeAttributes( element, havePSVI ),
                            (float)element );
                    case XmlTypeCode.HexBinary:
                    case XmlTypeCode.Language:
                        return new XElement( element.Name,
                            NormalizeAttributes( element, havePSVI ),
                            ( (string)element ).ToLower() );
                    default:
                        return new XElement( element.Name,
                            NormalizeAttributes( element, havePSVI ),
                            element.Nodes().Select( n => NormalizeNode( n, havePSVI ) )
                        );
                }
            }
            else
            {
                return new XElement( element.Name,
                    NormalizeAttributes( element, havePSVI ),
                    element.Nodes().Select( n => NormalizeNode( n, havePSVI ) )
                );
            }
        }
    }

    public partial class WindowWorkAreaTree : Window
    {
        public bool tobereopened = false;
        public bool CheckCompleto = false;

        public string SelectedTreeSource = "";
        public string SelectedDataSource = "";
        public string SelectedSessioneSource = "";

        public bool ReadOnly = true;
        public bool ApertoInSolaLettura = true;

        public string _cliente = "";
        public string Esercizio = "";

        public string SessioneAlias = "";
		public string SessioneAliasAdditivo = "";		
        public string SessioneFile = "";
		public string SessioneID = "";
        public string SessioneSigillo = "";
        public string SessioneSigilloData = "";
        public string SessioneSigilloPassword = "";

		private string selectedAlias = "";
		private string selectedAliasCodificato = "";
        private App.TipoAttivita _TipoAttivita = App.TipoAttivita.Sconosciuto;

		public string __IDTree = "-1";
		public string IDCliente = "-1";
		public string IDSessione = "-1";

		private bool firsttime = true;
        public bool StampaTemporanea = false;

        public XmlDataProviderManager _x;
        public XmlDataProvider TreeXmlProvider;

		Hashtable YearColor = new Hashtable();
		Hashtable htStati = new Hashtable();
		Hashtable htSessioni = new Hashtable();
		Hashtable htSessioniAlias = new Hashtable();
		Hashtable htSessioniID = new Hashtable();
        Hashtable htSessioneSigillo = new Hashtable();
        Hashtable htSessioneSigilloData = new Hashtable();
        Hashtable htSessioneSigilloPassword = new Hashtable();

        ArrayList ALXTPP = new ArrayList();

        public string IDTree 
		{       
			get 
			{ 
				return __IDTree; 
			}
			set 
			{ 
				__IDTree = value;               
			}
		}

        

		public string Cliente 
		{       
			get 
			{ 
				return _cliente; 
			}
			set 
			{ 
				_cliente = value;
				txtTitoloRagioneSociale.Text = _cliente;
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

		public WindowWorkAreaTree()
        {   
            InitializeComponent();


            //andrea 2.9
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

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

			//Colonna selezionata
			YearColor.Add(-1, "82BDE4");
            //Colori colonne di sezione
			YearColor.Add(2000, "F1F1F1");
			YearColor.Add(2001, "D3D3D3");
			YearColor.Add(2002, "F1F1F1");
			YearColor.Add(2003, "D3D3D3");
			YearColor.Add(2004, "F1F1F1");
			YearColor.Add(2005, "D3D3D3");
			YearColor.Add(2006, "F1F1F1");
			YearColor.Add(2007, "D3D3D3");
			YearColor.Add(2008, "F1F1F1");
			YearColor.Add(2009, "D3D3D3");
			YearColor.Add(2010, "F1F1F1");
			YearColor.Add(2011, "D3D3D3");
			YearColor.Add(2012, "F1F1F1");
			YearColor.Add(2013, "D3D3D3");
			YearColor.Add(2014, "F1F1F1");
			YearColor.Add(2015, "D3D3D3");
			YearColor.Add(2016, "F1F1F1");
			YearColor.Add(2017, "D3D3D3");
			YearColor.Add(2018, "F1F1F1");
			YearColor.Add(2019, "D3D3D3");
			YearColor.Add(2020, "F1F1F1");
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

			ReloadStatoNodiPadre();
        }

        private void SaveTreeSourceNoReload()
        {
            if ( TreeXmlProvider.Document != null )
            {
                RevisoftApplication.XmlManager x = new XmlManager();
                x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                x.SaveEncodedFile( SelectedTreeSource, TreeXmlProvider.Document.OuterXml );
            }
        }

        public void LoadTreeSource()
        {
            //Process wait - START
            //ProgressWindow pw = new ProgressWindow();

            //Titolo attivita
            Utilities u = new Utilities();
            txtTitoloAttivita.Text = u.TitoloAttivita(_TipoAttivita);

            btn_StampaVerbale.Visibility = System.Windows.Visibility.Collapsed;
            btn_CopiaLibroSociale.Visibility = System.Windows.Visibility.Collapsed;

            switch ( TipoAttivita )
            {
                case App.TipoAttivita.ISQC:
                case App.TipoAttivita.Incarico:
                case App.TipoAttivita.Revisione:
                case App.TipoAttivita.Bilancio:
                case App.TipoAttivita.Conclusione:
                    //TextBlock_Btn_Stampa.Text = "Stampa Fascicolo";
                    break;
                case App.TipoAttivita.Verifica:
                case App.TipoAttivita.Vigilanza:
                    //TextBlock_Btn_Stampa.Text = "Stampa Anteprima";

                    btn_StampaVerbale.Visibility = System.Windows.Visibility.Visible;

                    if (TipoAttivita == App.TipoAttivita.Verifica)
                    {
                        btn_CopiaLibroSociale.ToolTip = "Copia da Vigilanza";
                        TextBlock_Btn_CopiaLibroSociale.Text = "Copia da Vigilanza";
                    }
                    else
                    {
                        btn_CopiaLibroSociale.ToolTip = "Copia da Controllo Contabile";
                        TextBlock_Btn_CopiaLibroSociale.Text = "Copia da Controllo Contabile";
                    }
                    
                    Uri uriSource = null;
                    //andrea 4.10
                    //uriSource = new Uri( "/RevisoftApplication;component/Images/icone/document_view.png", UriKind.Relative );
                    uriSource = new Uri("/RevisoftApplication;component/Images/icone/printer3.png", UriKind.Relative);
                    img_StampaPDF.Source = new BitmapImage( uriSource );
                    break;                
                case App.TipoAttivita.RelazioneB:
                case App.TipoAttivita.RelazioneV:
                case App.TipoAttivita.RelazioneBC:
                case App.TipoAttivita.RelazioneVC:
                case App.TipoAttivita.RelazioneBV:
                    //TextBlock_Btn_Stampa.Text = "Stampa Relazione";
                    btn_ArchivioAllegati.Visibility = System.Windows.Visibility.Collapsed;
                    break;                    
                case App.TipoAttivita.Sconosciuto:
                default:
                    //TextBlock_Btn_Stampa.Text = "Stampa";
                    break;
            }

            //carico dati
            RevisoftApplication.XmlManager x = new XmlManager();
            x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
            TreeXmlProvider.Document = x.LoadEncodedFile(SelectedTreeSource);

			if (!u.CheckXmlDocument(TreeXmlProvider.Document, ((App.TipoFile)(Convert.ToInt32(IDTree))), "Tree"))
			{
                if (((App.TipoFile)(Convert.ToInt32(IDTree))) == App.TipoFile.RelazioneB)
                {
                    FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneB);
                    FileInfo oldfi = new FileInfo(SelectedTreeSource);
                    oldfi.Delete();
                    fitree.CopyTo(SelectedTreeSource);
                    TreeXmlProvider.Document = x.LoadEncodedFile(SelectedTreeSource);
                }
                else
                {
                    this.Close();
                    return;
                }


                if (((App.TipoFile)(Convert.ToInt32(IDTree))) == App.TipoFile.RelazioneBC)
                {
                    FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneBC);
                    FileInfo oldfi = new FileInfo(SelectedTreeSource);
                    oldfi.Delete();
                    fitree.CopyTo(SelectedTreeSource);
                    TreeXmlProvider.Document = x.LoadEncodedFile(SelectedTreeSource);
                }
                else
                {
                    this.Close();
                    return;
                }
            }

			if (firsttime)
			{
				firsttime = false;

                MasterFile mf = MasterFile.Create();
                if ( IDTree == "2" )
                {
                    ArrayList al = mf.GetPianificazioniVerifiche( IDCliente );
                    foreach ( Hashtable itemHT in al )
                    {
                        string filedata = App.AppDataDataFolder + "\\" + itemHT["FileData"].ToString();
                        if ( ( new FileInfo( filedata ) ).Exists )
                        {
                            ALXTPP.Add( new XmlDataProviderManager( filedata ) );
                        }
                    }
                }

                if ( IDTree == "18" )
                {
                    ArrayList al = mf.GetPianificazioniVigilanze( IDCliente );
                    foreach ( Hashtable itemHT in al )
                    {
                        string filedata = App.AppDataDataFolder + "\\" + itemHT["FileData"].ToString();
                        if ( ( new FileInfo( filedata ) ).Exists )
                        {
                            ALXTPP.Add( new XmlDataProviderManager( filedata ) );
                        }
                    }
                }

                //if(IDTree =="4")
                //{
                //    string[] sessionisplitted = SessioneFile.Split('|');
                //    string[] aliasplitted = SessioneAlias.Split('|');

                //    for (int i = 0; i < sessionisplitted.Count(); i++)
                //    {
                //        if ( sessionisplitted[i] == SelectedDataSource )
                //        {
                //            XmlNodeList xnl = TreeXmlProvider.Document.SelectNodes( "///Tree//Node[@ID]/Sessioni/Sessione[@Alias=\"" + aliasplitted[i] + "\"]" );
                //            int c= xnl.Count;
                //            break;
                //        }
                //    } 

                //    //

                //    //_x.Document.SelectSingleNode( "/Dati//Dato[@ID='246']/Node[@Voce='3.4.9']" )

                        

                //    //            if ( !chkNABilancioDaPianificazione.ContainsKey( ID ) )
                //    //            {
                //    //                chkNABilancioDaPianificazione.Add( ID, "" );
                //    //            }

                //}

				foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("//Node"))
				{
                    //string[] splittedID = SessioneID.Split('|');
                    //string[] splittedAlias = SessioneAlias.Split('|');

                    //for (int ii = 0; ii < splittedID.Count(); ii++)
                    //{
                    //    if(splittedID[ii] == IDSessione)
                    //    {
                    //        selectedAlias = splittedAlias[ii].Split( '/' )[2];
                    //    }
                    //}

                    //XmlNode nodeTree = item.SelectSingleNode("Sessioni/Sessione[@Alias=\"" + ConvertDataToEsercizio(selectedAlias, IDSessione) + "\"]");

                    //if (nodeTree.Attributes["Stato"] == null)
                    //{
                    //    XmlAttribute attr2 = nodeTree.OwnerDocument.CreateAttribute("Stato");
                    //    attr2.Value = "-1";
                    //    nodeTree.Attributes.Append(attr2);
                    //}

                    //if (nodeTree.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString() || nodeTree.Attributes["Status"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile).ToString()))
                    //{
                    //    string ID = nodeTree.Attributes["ID"].Value;

                    //    if (IDTree == "4" && (
                    //                        ID == "80"
                    //                        || ID == "81"
                    //                        || ID == "82"
                    //                        || ID == "83"
                    //                        || ID == "85"
                    //                        || ID == "86"
                    //                        || ID == "87"
                    //                        || ID == "88"
                    //                        || ID == "89"
                    //                        || ID == "90"
                    //                        || ID == "91"
                    //                        || ID == "92"
                    //                        || ID == "93"
                    //                        || ID == "94"
                    //                        || ID == "95"
                    //                        || ID == "96"
                    //                        || ID == "97"
                    //                        || ID == "98"
                    //                        || ID == "99"
                    //                        || ID == "100"
                    //                        || ID == "101"
                    //                        || ID == "102"
                    //                        ))
                    //    {
                    //        string FileDataRevisione = mf.GetRevisioneAssociataFromBilancioFile(SessioneFile);
                    //        if (FileDataRevisione != null && FileDataRevisione != "")
                    //        {
                    //            XmlDataProviderManager _x_x = new XmlDataProviderManager(FileDataRevisione);
                    //            if (_x_x != null)
                    //            {
                    //                XmlNode pianificazionenode = _x_x.Document.SelectSingleNode("/Dati//Dato[@ID='" + "274" + "']");
                    //                if (pianificazionenode != null)
                    //                {
                    //                    pianificazionenode = pianificazionenode.SelectSingleNode("Node[@ID='" + ID + "']");
                    //                    if (pianificazionenode != null)
                    //                    {   
                    //                        if (pianificazionenode.Attributes["cmbRI"] != null && pianificazionenode.Attributes["cmbRI"].Value == "NA")
                    //                        {
                    //                            nodeTree.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile)).ToString();
                    //                        }
                    //                        else
                    //                        {
                    //                            nodeTree.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString();
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}











                    if (item.ParentNode.Name == "Tree" || IDTree == "26" || IDTree == "27")
					{
						item.Attributes["Expanded"].Value = "True";
					}
					else
					{
						item.Attributes["Expanded"].Value = "False";
					}

                    if ( item.Attributes["Bold"] == null )
					{
                        XmlAttribute attr = item.OwnerDocument.CreateAttribute( "Bold" );
						attr.Value = "False";
						item.Attributes.Append(attr);
					}
                    
                    if (item.ParentNode.Name == "Tree")
                    {
                        item.Attributes["Bold"].Value = "True";
                    }

					item.Attributes["Selected"].Value = "False";


                    if ( item.Attributes["MinWidth"] == null )
                    {
                        XmlAttribute attr = item.OwnerDocument.CreateAttribute( "MinWidth" );
                        item.Attributes.Append( attr );
                    }

                    if ( IDTree == "26" || IDTree == "27" )
                    {
                        item.Attributes["MinWidth"].Value = "100";
                    }
                    else
                    {
                        item.Attributes["MinWidth"].Value = "Auto";
                    }


					if (item.Attributes["HighLighted"] == null)
					{
						XmlAttribute attr = item.OwnerDocument.CreateAttribute("HighLighted");
						attr.Value = "Black";
						item.Attributes.Append(attr);
					}

					item.Attributes["HighLighted"].Value = "Black";

                    if ( item.Attributes["Visible"] == null )
					{
                        XmlAttribute attr = item.OwnerDocument.CreateAttribute( "Visible" );
						item.Attributes.Append(attr);                        
					}

                    item.Attributes["Visible"].Value = "True";

                    if ( item.Attributes["Codice"].Value == "4" )
                    {
                        item.Attributes["Expanded"].Value = "True";
                    }

                    if ( item.Attributes["Codice"].Value == "5" )
                    {
                        item.Attributes["Expanded"].Value = "True";
                    }

                    //if ( TipoAttivita == App.TipoAttivita.Vigilanza && Convert.ToInt32( item.Attributes["ID"].Value ) < 500 )
                    //{
                    //    item.Attributes["Visible"].Value = "False";
                    //}

                    //if ( TipoAttivita == App.TipoAttivita.Verifica && Convert.ToInt32( item.Attributes["ID"].Value ) >= 500 )
                    //{
                    //    item.Attributes["Visible"].Value = "False";
                    //}
			        
				}
			}
            
            TreeXmlProvider.Refresh();
            
            LoadDataSource();

            //Process wait - STOP
            //pw.Close();
        }

#endregion

#region DataDataSource

		private string ConvertDataToEsercizio(string anno)
		{
			string returnvalue = "";

			MasterFile mf = MasterFile.Create();
			Hashtable clientetmp = mf.GetAnagrafica(Convert.ToInt32(IDCliente));

            switch ((App.TipoAnagraficaEsercizio)(Convert.ToInt32(clientetmp["Esercizio"].ToString())))
            {
                case App.TipoAnagraficaEsercizio.ACavallo:
                    returnvalue = anno + "/" + ((Convert.ToInt32(anno) % 100) + 1).ToString();
                    break;
                case App.TipoAnagraficaEsercizio.AnnoSolare:
                case App.TipoAnagraficaEsercizio.Sconosciuto:
                default:
                    returnvalue = anno;
                    break;
            }
			return returnvalue;
		}

        private string ConvertDataToEsercizio(string anno, Hashtable ht)
        {
            string returnvalue = "";

            if(!ht.Contains("Esercizio"))
            {
                return anno;
            }

            if (ht.Contains("Intermedio") && ht.Contains("EsercizioDal") && ht.Contains("EsercizioAl"))
            {
                returnvalue = ht["EsercizioDal"].ToString() + "\r\n" + ht["EsercizioAl"].ToString();
            }
            else
            {
                switch ((App.TipoAnagraficaEsercizio)(Convert.ToInt32(ht["Esercizio"].ToString())))
                {
                    case App.TipoAnagraficaEsercizio.ACavallo:
                        returnvalue = anno + "/" + ((Convert.ToInt32(anno) % 100) + 1).ToString();
                        break;
                    case App.TipoAnagraficaEsercizio.AnnoSolare:
                    case App.TipoAnagraficaEsercizio.Sconosciuto:
                    default:
                        returnvalue = anno;
                        break;
                }
            }

            return returnvalue;
        }

        bool nodialreadyloader = false;
        private void LoadDataSource()
        {
            _x = new XmlDataProviderManager(SelectedDataSource);
			
			Utilities u = new Utilities();
			if (!u.CheckXmlDocument(_x.Document, ((App.TipoFile)(Convert.ToInt32(IDTree))), "Data"))
			{
				this.Close();
				return;
			}

            if(nodialreadyloader == false)
            {

                ReloadNodi();

                ScrollForced();

                nodialreadyloader = true;
            }

		}

        private bool CheckIfAllDates( ref Hashtable ht, ref Hashtable htID , ref Hashtable htAliasAdditivo, ref List<DateTime> dates, ref List<string> strings)
        {
            bool alldates = true;

            for ( int i = 0; i < SessioneFile.Split( '|' ).Count(); i++ )
            {
                ht.Add( SessioneAlias.Split( '|' )[i], SessioneFile.Split( '|' )[i] );

                if ( SessioneSigillo.Split( '|' ).Count() > i )
                {
                    htSessioneSigillo.Add( SessioneAlias.Split( '|' )[i], SessioneSigillo.Split( '|' )[i] );
                }

                if ( SessioneSigilloData.Split( '|' ).Count() > i )
                {
                    htSessioneSigilloData.Add( SessioneAlias.Split( '|' )[i], SessioneSigilloData.Split( '|' )[i] );
                }

                if ( SessioneSigilloPassword.Split( '|' ).Count() > i )
                {
                    htSessioneSigilloPassword.Add( SessioneAlias.Split( '|' )[i], SessioneSigilloPassword.Split( '|' )[i] );
                }

                htID.Add( SessioneAlias.Split( '|' )[i], SessioneID.Split( '|' )[i].Replace( "S1_", "" ).Replace( "S2_", "" ).Replace( "S3_", "" ) );

                if(SessioneAliasAdditivo.Split( '|' ).Count() > i)
                {
                    htAliasAdditivo.Add( SessioneAlias.Split( '|' )[i], SessioneAliasAdditivo.Split( '|' )[i] );
                }
                else
                {
                    htAliasAdditivo.Add( SessioneAlias.Split( '|' )[i], "" );
                }

                string aliastmp = SessioneAlias.Split( '|' )[i];

                strings.Add( aliastmp );

                if ( aliastmp == "" )
                {
                    aliastmp = "31/12/" + DateTime.Now.Year.ToString();
                }

                DateTime data;
                
                if (DateTime.TryParse( aliastmp.Replace( "S1_", "" ).Replace( "S2_", "" ).Replace( "S3_", "" ) , out data))
                {
                    if ( aliastmp.Contains( "S1_" ) )
                    {
                        data = data.AddDays( 1 );
                    }

                    if ( aliastmp.Contains( "S2_" ) )
                    {
                        data = data.AddDays( 2 );
                    }

                    if ( aliastmp.Contains( "S3_" ) )
                    {
                        data = data.AddDays( 3 );
                    }
                    dates.Add( data );
                }
                else
                {
                    alldates = false;
                }
            }

            return alldates;
        }

        bool GetCompletezzaNodiAlreadyDone = false;
        private void GetCompletezzaNodi( ref bool alldates, ref List<string> strings, ref List<DateTime> dates, ref Hashtable ht, ref Hashtable htID, ref ArrayList alCheckCompletezzaNodi, ref XmlManager x )
        {
            if(GetCompletezzaNodiAlreadyDone == true)
            {
                return;
            }

            GetCompletezzaNodiAlreadyDone = true;

            for ( int i = 0; i < strings.Count; i++ )
            {
                string alias;
                if ( alldates )
                {
                    alias = dates[i].ToShortDateString();
                    if ( alias == "31/12" + DateTime.Now.Year.ToString() )
                    {
                        alias = "";
                    }
                }
                else
                {
                    alias = strings[i];
                }

                int daycounter = -1;

                if ( !ht.Contains( alias ) )
                {
                    //compatibilità con MAC
                    if(alias.Split('/')[2].Length == 4)
                    {
                        alias = alias.Split( '/' )[0] + "/" + alias.Split( '/' )[1] + "/" + alias.Split( '/' )[2].Substring(2,2) ;
                    }
                }

                if ( !ht.Contains( alias ) )
                {
                    //compatibilità con MAC
                    if ( alias.Split( '/' )[2].Length == 2 )
                    {
                        alias = alias.Split( '/' )[0] + "/" + alias.Split( '/' )[1] + "/20" + alias.Split( '/' )[2];
                    }
                }

                while ( !ht.Contains( alias ) )
                {
                    if ( daycounter == -1 )
                    {
                        alias = "S1_" + dates[i].AddDays( daycounter ).ToShortDateString();
                    }

                    if ( daycounter == -2 )
                    {
                        alias = "S2_" + dates[i].AddDays( daycounter ).ToShortDateString();
                    }

                    if ( daycounter == -3 )
                    {
                        alias = "S3_" + dates[i].AddDays( daycounter ).ToShortDateString();
                        break;
                    }

                    daycounter -= 1;
                }

                if ( !htSessioni.ContainsKey( i ) )
                { htSessioni.Add( i, ht[alias].ToString() ); }
                if ( !htSessioniID.ContainsKey( i ) )
                { htSessioniID.Add( i, htID[alias].ToString() ); }

                XmlDocument tmpDoc = x.LoadEncodedFile( ht[alias].ToString() );
                
                GetTemplateVersioning( ht[alias].ToString() );

                foreach ( XmlNode node in tmpDoc.SelectNodes( "/Dati//Dato" ) )
                {
                    if ( !alCheckCompletezzaNodi.Contains( node.Attributes["ID"].Value ) )
                    {
                        alCheckCompletezzaNodi.Add( node.Attributes["ID"].Value );
                    }
                }
            }
        }

        bool GetTemplateVersioningAlreadyDone = false;

        private void GetTemplateVersioning( string fileData)
        {
            if (GetTemplateVersioningAlreadyDone == true)
            {
                return;
            }

            GetTemplateVersioningAlreadyDone = true;

            //Check su altri template
            ArrayList TemplateVersions = new ArrayList();

            Utilities u = new Utilities();
            XmlDocument doctmp = new XmlDocument();

            //luigi
            //doctmp.Load( App.AppTemplateFolder + "\\TranscodificaTemplate.xml" );
            //doctmp.Load(App.AppTemplateFolder + App.IndiceTemplateFileName + u.EstensioneFile(App.TipoFile.IndiceTemplate));


            //apro file XML
            XmlManager x = new XmlManager();
            string tFile = string.Empty;
            App.ErrorLevel = App.ErrorTypes.Nessuno;
            tFile = App.AppTemplateFolder + "\\" + App.IndiceTemplateFileName + u.EstensioneFile(App.TipoFile.IndiceTemplate);
            x.TipoCodifica = XmlManager.TipologiaCodifica.Normale;

            doctmp = x.LoadEncodedFile(tFile);


            foreach ( XmlNode item in doctmp.SelectNodes( "/TEMPLATES/TEMPLATE" ) )
            {
                TemplateVersions.Add( item.Attributes["VERSION"].Value );
            } 

            XmlManager xTreeParagone = new XmlManager();

            if(fileData == SelectedDataSource)
            {
                if ( TreeXmlProvider.Document.SelectSingleNode( "/Tree/REVISOFT" ).Attributes["Template"] == null )
                {
                    //ALBERO DI PARTENZA

                    XmlNode tmpNodeTreehere = TreeXmlProvider.Document.SelectSingleNode( "/Tree" ).Clone();

                    XmlAttribute attr = TreeXmlProvider.Document.CreateAttribute( "Template" );
                    TreeXmlProvider.Document.SelectSingleNode( "/Tree/REVISOFT" ).Attributes.Append( attr );

                    TreeXmlProvider.Document.SelectSingleNode( "/Tree/REVISOFT" ).Attributes["Template"].Value = (string)( TemplateVersions[TemplateVersions.Count - 1] );

                    App.TipoFile TipoTree = ( (App.TipoFile)( Convert.ToInt32( TreeXmlProvider.Document.SelectSingleNode( "/Tree/REVISOFT" ).Attributes["ID"].Value ) ) );

                    foreach ( XmlNode item in tmpNodeTreehere.SelectNodes( "//Sessioni" ) )
                    {
                        item.ParentNode.RemoveChild( item );
                    }

                    foreach ( XmlNode item in tmpNodeTreehere.SelectNodes( "//Node" ) )
                    {
                        if ( item.Attributes["Report"] != null )
                            item.Attributes.Remove( item.Attributes["Report"] );

                        if ( item.Attributes["Nota"] != null )
                            item.Attributes.Remove( item.Attributes["Nota"] );

                        if ( item.Attributes["Chiuso"] != null )
                            item.Attributes.Remove( item.Attributes["Chiuso"] );

                        if ( item.Attributes["HighLighted"] != null )
                            item.Attributes.Remove( item.Attributes["HighLighted"] );

                        if ( item.Attributes["Visible"] != null )
                            item.Attributes.Remove( item.Attributes["Visible"] );

                        if ( item.Attributes["Selected"] != null )
                            item.Attributes.Remove( item.Attributes["Selected"] );

                        if ( item.Attributes["MinWidth"] != null )
                            item.Attributes.Remove( item.Attributes["MinWidth"] );

                        if ( item.Attributes["Expanded"] != null )
                            item.Attributes.Remove( item.Attributes["Expanded"] );

                    }

                    //ALBERO DI PARAGONE

                    for ( int i = 0; i < (TemplateVersions.Count - 1); i++ )
                    {
                        XmlNode templateParagoneNode = null;
                        string filepathhere = "";
                        FileInfo fihere = null;

                        switch ( TipoTree )
                        {
                            case App.TipoFile.Revisione:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRevisione + ( new Utilities() ).EstensioneFile( App.TipoFile.Revisione );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }                                
                                break;
                            case App.TipoFile.PianificazioniVerifica:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNamePianificazioniVerifica + ( new Utilities() ).EstensioneFile( App.TipoFile.PianificazioniVerifica );
                                fihere = new FileInfo( filepathhere );
                                if ( fihere.Exists )
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere ).SelectSingleNode( "/Tree" ).Clone() );
                                }
                                break;
                            case App.TipoFile.Verifica:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameVerifica + ( new Utilities() ).EstensioneFile( App.TipoFile.Verifica );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }   
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameVerifica + ( new Utilities() ).EstensioneFile( App.TipoFile.Verifica ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.Incarico:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameIncarico + ( new Utilities() ).EstensioneFile( App.TipoFile.Incarico );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }   
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameIncarico + ( new Utilities() ).EstensioneFile( App.TipoFile.Incarico ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;                           
                            case App.TipoFile.ISQC:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameISQC + (new Utilities()).EstensioneFile(App.TipoFile.ISQC);
                                fihere = new FileInfo(filepathhere);
                                if (fihere.Exists)
                                {
                                    templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                                }
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameIncarico + ( new Utilities() ).EstensioneFile( App.TipoFile.Incarico ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.Bilancio:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameBilancio + ( new Utilities() ).EstensioneFile( App.TipoFile.Bilancio );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }   
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameBilancio + ( new Utilities() ).EstensioneFile( App.TipoFile.Bilancio ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.PianificazioniVigilanza:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNamePianificazioniVigilanza + ( new Utilities() ).EstensioneFile( App.TipoFile.PianificazioniVigilanza );
                                fihere = new FileInfo( filepathhere );
                                if ( fihere.Exists )
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere ).SelectSingleNode( "/Tree" ).Clone() );
                                }                                break;
                            case App.TipoFile.Vigilanza:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameVigilanza + ( new Utilities() ).EstensioneFile( App.TipoFile.Vigilanza );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }   
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameVigilanza + ( new Utilities() ).EstensioneFile( App.TipoFile.Vigilanza ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.Conclusione:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameConclusione + ( new Utilities() ).EstensioneFile( App.TipoFile.Conclusione );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }   
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameConclusione + ( new Utilities() ).EstensioneFile( App.TipoFile.Conclusione ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.RelazioneB:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneB + ( new Utilities() ).EstensioneFile( App.TipoFile.RelazioneB );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }   
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneB + ( new Utilities() ).EstensioneFile( App.TipoFile.RelazioneB ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.RelazioneV:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneV + ( new Utilities() ).EstensioneFile( App.TipoFile.RelazioneV );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }   
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneV + ( new Utilities() ).EstensioneFile( App.TipoFile.RelazioneV ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;

                            case App.TipoFile.RelazioneBC:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneBC + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneBC);
                                fihere = new FileInfo(filepathhere);
                                if (fihere.Exists)
                                {
                                    templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                                }
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneB + ( new Utilities() ).EstensioneFile( App.TipoFile.RelazioneB ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.RelazioneVC:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneVC + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneVC);
                                fihere = new FileInfo(filepathhere);
                                if (fihere.Exists)
                                {
                                    templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                                }
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneV + ( new Utilities() ).EstensioneFile( App.TipoFile.RelazioneV ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.RelazioneBV:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneBV + ( new Utilities() ).EstensioneFile( App.TipoFile.RelazioneBV );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }   
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneBV + ( new Utilities() ).EstensioneFile( App.TipoFile.RelazioneBV ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            default:
                                break;
                        }                   

                        if(templateParagoneNode != null)
                        {
                            foreach ( XmlNode item in templateParagoneNode.SelectNodes( "//Node" ) )
                            {
                                if ( item.Attributes["Report"] != null )
                                    item.Attributes.Remove( item.Attributes["Report"] );

                                if ( item.Attributes["Nota"] != null )
                                    item.Attributes.Remove( item.Attributes["Nota"] );

                                if ( item.Attributes["Chiuso"] != null )
                                    item.Attributes.Remove( item.Attributes["Chiuso"] );

                                if ( item.Attributes["HighLighted"] != null )
                                    item.Attributes.Remove( item.Attributes["HighLighted"] );

                                if ( item.Attributes["Visible"] != null )
                                    item.Attributes.Remove( item.Attributes["Visible"] );

                                if ( item.Attributes["Selected"] != null )
                                    item.Attributes.Remove( item.Attributes["Selected"] );

                                if ( item.Attributes["MinWidth"] != null )
                                    item.Attributes.Remove( item.Attributes["MinWidth"] );

                                if ( item.Attributes["Expanded"] != null )
                                    item.Attributes.Remove( item.Attributes["Expanded"] );

                            }

                            bool result = Program.DeepEqualsWithNormalization( XDocument.Parse( tmpNodeTreehere.OuterXml ), XDocument.Parse( templateParagoneNode.OuterXml ), null);

                            //XNode.DeepEquals
                            if ( result )
                            {
                                TreeXmlProvider.Document.SelectSingleNode( "/Tree/REVISOFT" ).Attributes["Template"].Value = (string)( TemplateVersions[i] );
                                TreeXmlProvider.Refresh();
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                XmlDataProviderManager xTree = new XmlDataProviderManager( App.AppDataDataFolder + "\\" + ( new MasterFile() ).GetTreeAssociatoFromFileData( fileData ) , true);
                            
                XmlDocument tmpTree = xTree.Document;

                if ( tmpTree.SelectSingleNode( "/Tree/REVISOFT" ).Attributes["Template"] == null )
                {
                    //ALBERO DI PARTENZA

                    XmlNode tmpNodeTreehere = tmpTree.SelectSingleNode( "/Tree" ).Clone();
                
                    XmlAttribute attr = tmpTree.CreateAttribute( "Template" );
                    tmpTree.SelectSingleNode( "/Tree/REVISOFT" ).Attributes.Append( attr );

                    tmpTree.SelectSingleNode( "/Tree/REVISOFT" ).Attributes["Template"].Value = (string)( TemplateVersions[TemplateVersions.Count - 1] );
                                
                    App.TipoFile TipoTree = ( (App.TipoFile)( Convert.ToInt32( tmpTree.SelectSingleNode( "/Tree/REVISOFT" ).Attributes["ID"].Value ) ) );

                    foreach ( XmlNode item in tmpNodeTreehere.SelectNodes( "//Sessioni" ) )
                    {
                        item.ParentNode.RemoveChild( item );
                    }

                    foreach ( XmlNode item in tmpNodeTreehere.SelectNodes( "//Node" ) )
                    {
                        if ( item.Attributes["Report"] != null )
                            item.Attributes.Remove( item.Attributes["Report"] );

                        if ( item.Attributes["Nota"] != null )
                            item.Attributes.Remove( item.Attributes["Nota"] );

                        if ( item.Attributes["Chiuso"] != null )
                            item.Attributes.Remove( item.Attributes["Chiuso"] );

                        if ( item.Attributes["HighLighted"] != null )
                            item.Attributes.Remove( item.Attributes["HighLighted"] );

                        if ( item.Attributes["Visible"] != null )
                            item.Attributes.Remove( item.Attributes["Visible"] );

                        if ( item.Attributes["Selected"] != null )
                            item.Attributes.Remove( item.Attributes["Selected"] );

                        if ( item.Attributes["MinWidth"] != null )
                            item.Attributes.Remove( item.Attributes["MinWidth"] );

                        if ( item.Attributes["Expanded"] != null )
                            item.Attributes.Remove( item.Attributes["Expanded"] );

                    }

                    //ALBERO DI PARAGONE

                    for ( int i = 0; i < (TemplateVersions.Count - 1); i++ )
                    {
                        XmlNode templateParagoneNode = null;
                        string filepathhere = "";
                        FileInfo fihere = null;

                        switch ( TipoTree )
                        {
                            case App.TipoFile.Revisione:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRevisione + ( new Utilities() ).EstensioneFile( App.TipoFile.Revisione );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }                                
                                break;
                            case App.TipoFile.PianificazioniVerifica:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNamePianificazioniVerifica + ( new Utilities() ).EstensioneFile( App.TipoFile.PianificazioniVerifica );
                                fihere = new FileInfo( filepathhere );
                                if ( fihere.Exists )
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere ).SelectSingleNode( "/Tree" ).Clone() );
                                }
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameVerifica + ( new Utilities() ).EstensioneFile( App.TipoFile.Verifica ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.Verifica:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameVerifica + ( new Utilities() ).EstensioneFile( App.TipoFile.Verifica );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }   
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameVerifica + ( new Utilities() ).EstensioneFile( App.TipoFile.Verifica ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.Incarico:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameIncarico + ( new Utilities() ).EstensioneFile( App.TipoFile.Incarico );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }   
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameIncarico + ( new Utilities() ).EstensioneFile( App.TipoFile.Incarico ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.ISQC:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameISQC + (new Utilities()).EstensioneFile(App.TipoFile.ISQC);
                                fihere = new FileInfo(filepathhere);
                                if (fihere.Exists)
                                {
                                    templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                                }
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameIncarico + ( new Utilities() ).EstensioneFile( App.TipoFile.Incarico ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.Bilancio:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameBilancio + ( new Utilities() ).EstensioneFile( App.TipoFile.Bilancio );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }   
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameBilancio + ( new Utilities() ).EstensioneFile( App.TipoFile.Bilancio ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.PianificazioniVigilanza:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNamePianificazioniVigilanza + ( new Utilities() ).EstensioneFile( App.TipoFile.PianificazioniVigilanza );
                                fihere = new FileInfo( filepathhere );
                                if ( fihere.Exists )
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere ).SelectSingleNode( "/Tree" ).Clone() );
                                }
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameVigilanza + ( new Utilities() ).EstensioneFile( App.TipoFile.Vigilanza ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.Vigilanza:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameVigilanza + ( new Utilities() ).EstensioneFile( App.TipoFile.Vigilanza );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }   
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameVigilanza + ( new Utilities() ).EstensioneFile( App.TipoFile.Vigilanza ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.Conclusione:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameConclusione + ( new Utilities() ).EstensioneFile( App.TipoFile.Conclusione );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }   
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameConclusione + ( new Utilities() ).EstensioneFile( App.TipoFile.Conclusione ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.RelazioneB:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneB + ( new Utilities() ).EstensioneFile( App.TipoFile.RelazioneB );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }   
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneB + ( new Utilities() ).EstensioneFile( App.TipoFile.RelazioneB ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.RelazioneV:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneV + ( new Utilities() ).EstensioneFile( App.TipoFile.RelazioneV );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }   
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneV + ( new Utilities() ).EstensioneFile( App.TipoFile.RelazioneV ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.RelazioneBC:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneBC + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneBC);
                                fihere = new FileInfo(filepathhere);
                                if (fihere.Exists)
                                {
                                    templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                                }
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneB + ( new Utilities() ).EstensioneFile( App.TipoFile.RelazioneB ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.RelazioneVC:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneVC + (new Utilities()).EstensioneFile(App.TipoFile.RelazioneVC);
                                fihere = new FileInfo(filepathhere);
                                if (fihere.Exists)
                                {
                                    templateParagoneNode = (xTreeParagone.LoadEncodedFile(filepathhere).SelectSingleNode("/Tree").Clone());
                                }
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneV + ( new Utilities() ).EstensioneFile( App.TipoFile.RelazioneV ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            case App.TipoFile.RelazioneBV:
                                filepathhere = App.AppTemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneBV + ( new Utilities() ).EstensioneFile( App.TipoFile.RelazioneBV );
                                fihere = new FileInfo(filepathhere);
                                if(fihere.Exists)
                                {
                                    templateParagoneNode = ( xTreeParagone.LoadEncodedFile( filepathhere  ).SelectSingleNode( "/Tree" ).Clone());
                                }   
                                //templateParagoneNode = ( xTreeParagone.LoadEncodedFile( App.AppTemplateFolder + "\\Versioni\\" + TemplateVersions[i] + "\\" + App.DocNameRelazioneBV + ( new Utilities() ).EstensioneFile( App.TipoFile.RelazioneBV ) ) ).SelectSingleNode( "/Tree" ).Clone();
                                break;
                            default:
                                break;
                        }

                        if ( templateParagoneNode != null )
                        {
                            foreach ( XmlNode item in templateParagoneNode.SelectNodes( "//Node" ) )
                            {
                                if ( item.Attributes["Report"] != null )
                                    item.Attributes.Remove( item.Attributes["Report"] );

                                if ( item.Attributes["Nota"] != null )
                                    item.Attributes.Remove( item.Attributes["Nota"] );

                                if ( item.Attributes["Chiuso"] != null )
                                    item.Attributes.Remove( item.Attributes["Chiuso"] );

                                if ( item.Attributes["HighLighted"] != null )
                                    item.Attributes.Remove( item.Attributes["HighLighted"] );

                                if ( item.Attributes["Visible"] != null )
                                    item.Attributes.Remove( item.Attributes["Visible"] );

                                if ( item.Attributes["Selected"] != null )
                                    item.Attributes.Remove( item.Attributes["Selected"] );

                                if ( item.Attributes["MinWidth"] != null )
                                    item.Attributes.Remove( item.Attributes["MinWidth"] );

                                if ( item.Attributes["Expanded"] != null )
                                    item.Attributes.Remove( item.Attributes["Expanded"] );

                            }

                            bool result = Program.DeepEqualsWithNormalization( XDocument.Parse( tmpNodeTreehere.OuterXml ), XDocument.Parse( templateParagoneNode.OuterXml ), null );

                            //XNode.DeepEquals
                            if ( result )
                            {
                                tmpTree.SelectSingleNode( "/Tree/REVISOFT" ).Attributes["Template"].Value = (string)( TemplateVersions[i] );
                                xTree.Save();
                                break;
                            }
                        }
                    }

                    xTree.Save();
                }
            }
        }


        private void VerificaStati( ref bool alldates, ref List<string> strings, ref List<DateTime> dates, ref Hashtable ht, ref XmlManager x, ref Hashtable htAliasAdditivo, ref ArrayList alCheckCompletezzaNodi )
        {
            Hashtable chkNA = new Hashtable();            

            for ( int i = 0; i < strings.Count; i++ )
            {
                string alias;
                if ( alldates )
                {
                    alias = dates[i].ToShortDateString();
                    if ( alias == "31/12" + DateTime.Now.Year.ToString() )
                    {
                        alias = "";
                    }
                }
                else
                {
                    alias = strings[i];
                }

                if ( !ht.Contains( alias ) )
                {
                    //compatibilità con MAC
                    if ( alias.Split( '/' )[2].Length == 4 )
                    {
                        alias = alias.Split( '/' )[0] + "/" + alias.Split( '/' )[1] + "/" + alias.Split( '/' )[2].Substring( 2, 2 );
                    }
                }

                if ( !ht.Contains( alias ) )
                {
                    //compatibilità con MAC
                    if ( alias.Split( '/' )[2].Length == 2 )
                    {
                        alias = alias.Split( '/' )[0] + "/" + alias.Split( '/' )[1] + "/20" + alias.Split( '/' )[2];
                    }
                }

                int daycounter = -1;

                while ( !ht.Contains( alias ) )
                {
                    if ( daycounter == -1 )
                    {
                        alias = "S1_" + dates[i].AddDays( daycounter ).ToShortDateString();
                    }

                    if ( daycounter == -2 )
                    {
                        alias = "S2_" + dates[i].AddDays( daycounter ).ToShortDateString();
                    }

                    if ( daycounter == -3 )
                    {
                        alias = "S3_" + dates[i].AddDays( daycounter ).ToShortDateString();
                        break;
                    }

                    daycounter -= 1;
                }

                string aliastocheck = "";

                switch ( (App.TipoFile)( System.Convert.ToInt32( IDTree ) ) )
                {
                    case App.TipoFile.Bilancio:
                        if (alias.Split('/').Count() > 2)
                        {
                            MasterFile mf = MasterFile.Create();
                            Hashtable hthere = mf.GetBilancioFromFileData(ht[alias].ToString());

                            if(hthere != null && hthere["Esercizio"] != null)
                            {
                                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                            }
                            else
                            {
                                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2]);
                            }                            
                        }
                        break;
                    case App.TipoFile.Conclusione:
                        if (alias.Split('/').Count() > 2)
                        {
                            MasterFile mf = MasterFile.Create();
                            Hashtable hthere = mf.GetConclusioneFromFileData(ht[alias].ToString());

                            if (hthere != null && hthere["Esercizio"] != null)
                            {
                                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                            }
                            else
                            {
                                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2]);
                            }
                        }
                        break;
                    case App.TipoFile.Revisione:
                        if (alias.Split('/').Count() > 2)
                        {
                            MasterFile mf = MasterFile.Create();
                            Hashtable hthere = mf.GetRevisioneFromFileData(ht[alias].ToString());

                            if (hthere != null && hthere["Esercizio"] != null)
                            {
                                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                            }
                            else
                            {
                                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2]);
                            }
                        }
                        break;
                    case App.TipoFile.RelazioneB:
                        if (alias.Split('/').Count() > 2)
                        {
                            MasterFile mf = MasterFile.Create();
                            Hashtable hthere = mf.GetRelazioneBFromFileData(ht[alias].ToString());

                            if (hthere != null && hthere["Esercizio"] != null)
                            {
                                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                            }
                            else
                            {
                                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2]);
                            }
                        }
                        break;
                    case App.TipoFile.RelazioneV:
                        if (alias.Split('/').Count() > 2)
                        {
                            MasterFile mf = MasterFile.Create();
                            Hashtable hthere = mf.GetRelazioneVFromFileData(ht[alias].ToString());

                            if (hthere != null && hthere["Esercizio"] != null)
                            {
                                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                            }
                            else
                            {
                                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2]);
                            }
                        }
                        break;

                    case App.TipoFile.RelazioneBC:
                        if (alias.Split('/').Count() > 2)
                        {
                            MasterFile mf = MasterFile.Create();
                            Hashtable hthere = mf.GetRelazioneBCFromFileData(ht[alias].ToString());

                            if (hthere != null && hthere["Esercizio"] != null)
                            {
                                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                            }
                            else
                            {
                                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2]);
                            }
                        }
                        break;
                    case App.TipoFile.RelazioneVC:
                        if (alias.Split('/').Count() > 2)
                        {
                            MasterFile mf = MasterFile.Create();
                            Hashtable hthere = mf.GetRelazioneVCFromFileData(ht[alias].ToString());

                            if (hthere != null && hthere["Esercizio"] != null)
                            {
                                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                            }
                            else
                            {
                                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2]);
                            }
                        }
                        break;

                    case App.TipoFile.RelazioneBV:
                        if (alias.Split('/').Count() > 2)
                        {
                            MasterFile mf = MasterFile.Create();
                            Hashtable hthere = mf.GetRelazioneBVFromFileData(ht[alias].ToString());

                            if (hthere != null && hthere["Esercizio"] != null)
                            {
                                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                            }
                            else
                            {
                                aliastocheck = ConvertDataToEsercizio(alias.Split('/')[2]);
                            }
                        }
                        break;

                    case App.TipoFile.PianificazioniVigilanza:
                    case App.TipoFile.PianificazioniVerifica:
                        if ( alias.Split( '/' ).Count() > 2 && htAliasAdditivo[alias] != null )
                        {
                            aliastocheck = alias.Split( '/' )[0] + "/" + alias.Split( '/' )[1] + "/" + alias.Split( '/' )[2] + ( ( htAliasAdditivo[alias].ToString() == "" ) ? "" : "\r\n" + htAliasAdditivo[alias].ToString() );
                        }
                        break;
                    case App.TipoFile.Vigilanza:
                    case App.TipoFile.Verifica:
                    case App.TipoFile.Incarico:
                        if ( alias.Split( '/' ).Count() > 2 && htAliasAdditivo[alias] != null )
                        {
                            aliastocheck = alias.Split( '/' )[0] + "/" + alias.Split( '/' )[1] + "\r\n" + alias.Split( '/' )[2] + ( ( htAliasAdditivo[alias].ToString() == "" ) ? "" : "\r\n" + htAliasAdditivo[alias].ToString() );
                        }
                        break;
                    case App.TipoFile.ISQC:
                        if (alias.Split('/').Count() > 2 && htAliasAdditivo[alias] != null)
                        {
                            aliastocheck = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
                        }
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

                XmlDocument tmpDoc = x.LoadEncodedFile( ht[alias].ToString() );

                XmlNode nodeTree = TreeXmlProvider.Document.SelectSingleNode( "/Tree" );
                if ( nodeTree != null )
                {
                    XmlNode nodeSessioni = nodeTree.SelectSingleNode( "Sessioni" );
                    //PRISC DA CONTROLLARE PER SIGILLO -- inizio
                    if ( i == 0 && nodeSessioni != null && !alias.Contains( "S1" ) && !alias.Contains( "S2" ) && !alias.Contains( "S3" ) )
                    {
                        nodeSessioni.ParentNode.RemoveChild( nodeSessioni );
                        nodeSessioni = null;
                    }
                    //PRISC DA CONTROLLARE PER SIGILLO -- fine

                    if ( nodeSessioni == null )
                    {
                        nodeSessioni = nodeTree.OwnerDocument.CreateNode( XmlNodeType.Element, "Sessioni", "" );
                        nodeTree.AppendChild( nodeSessioni );
                        nodeSessioni = nodeTree.SelectSingleNode( "Sessioni" );
                    }

                    XmlNode nodeSessione = nodeTree.SelectSingleNode( "Sessioni/Sessione[@Alias=\"" + aliastocheck + "\"]" );

                    if ( nodeSessione != null )
                    {
                        switch ( (App.TipoFile)( System.Convert.ToInt32( IDTree ) ) )
                        {
                            case App.TipoFile.Bilancio:
                                if (alias.Split('/').Count() > 2)
                                {
                                    MasterFile mf = MasterFile.Create();
                                    Hashtable hthere = mf.GetBilancioFromFileData(ht[alias].ToString());

                                    if (hthere != null && hthere["Esercizio"] != null)
                                    {
                                        htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                                    }
                                    else
                                    {
                                        htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                                    }
                                }
                                break;
                            case App.TipoFile.Conclusione:
                                if (alias.Split('/').Count() > 2)
                                {
                                    MasterFile mf = MasterFile.Create();
                                    Hashtable hthere = mf.GetConclusioneFromFileData(ht[alias].ToString());

                                    if (hthere != null && hthere["Esercizio"] != null)
                                    {
                                        htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                                    }
                                    else
                                    {
                                        htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                                    }
                                }
                                break;
                            case App.TipoFile.Revisione:
                                if (alias.Split('/').Count() > 2)
                                {
                                    MasterFile mf = MasterFile.Create();
                                    Hashtable hthere = mf.GetRevisioneFromFileData(ht[alias].ToString());

                                    if (hthere != null && hthere["Esercizio"] != null)
                                    {
                                        htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                                    }
                                    else
                                    {
                                        htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                                    }
                                }
                                break;
                            case App.TipoFile.RelazioneB:
                                if (alias.Split('/').Count() > 2)
                                {
                                    MasterFile mf = MasterFile.Create();
                                    Hashtable hthere = mf.GetRelazioneBFromFileData(ht[alias].ToString());

                                    if (hthere != null && hthere["Esercizio"] != null)
                                    {
                                        htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                                    }
                                    else
                                    {
                                        htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                                    }
                                }
                                break;
                            case App.TipoFile.RelazioneV:
                                if (alias.Split('/').Count() > 2)
                                {
                                    MasterFile mf = MasterFile.Create();
                                    Hashtable hthere = mf.GetRelazioneVFromFileData(ht[alias].ToString());

                                    if (hthere != null && hthere["Esercizio"] != null)
                                    {
                                        htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                                    }
                                    else
                                    {
                                        htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                                    }
                                }
                                break;

                            case App.TipoFile.RelazioneBC:
                                if (alias.Split('/').Count() > 2)
                                {
                                    MasterFile mf = MasterFile.Create();
                                    Hashtable hthere = mf.GetRelazioneBCFromFileData(ht[alias].ToString());

                                    if (hthere != null && hthere["Esercizio"] != null)
                                    {
                                        htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                                    }
                                    else
                                    {
                                        htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                                    }
                                }
                                break;
                            case App.TipoFile.RelazioneVC:
                                if (alias.Split('/').Count() > 2)
                                {
                                    MasterFile mf = MasterFile.Create();
                                    Hashtable hthere = mf.GetRelazioneVCFromFileData(ht[alias].ToString());

                                    if (hthere != null && hthere["Esercizio"] != null)
                                    {
                                        htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                                    }
                                    else
                                    {
                                        htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                                    }
                                }
                                break;

                            case App.TipoFile.RelazioneBV:
                                if (alias.Split('/').Count() > 2)
                                {
                                    MasterFile mf = MasterFile.Create();
                                    Hashtable hthere = mf.GetRelazioneBVFromFileData(ht[alias].ToString());

                                    if (hthere != null && hthere["Esercizio"] != null)
                                    {
                                        htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                                    }
                                    else
                                    {
                                        htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                                    }
                                }
                                break;
                            case App.TipoFile.PianificazioniVigilanza:
                            case App.TipoFile.PianificazioniVerifica:
                            case App.TipoFile.Vigilanza:
                            case App.TipoFile.Verifica:
                            case App.TipoFile.Incarico:
                                htSessioniAlias.Add( i, alias.Split( '/' )[0] + "/" + alias.Split( '/' )[1] + "/" + alias.Split( '/' )[2] + ( ( htAliasAdditivo[alias].ToString() == "" ) ? "" : " - " + htAliasAdditivo[alias].ToString() ) );
                                break;
                            case App.TipoFile.ISQC:
                                htSessioniAlias.Add(i, alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : " - " + htAliasAdditivo[alias].ToString()));
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

                        if ( SelectedDataSource == ht[alias].ToString() )
                        {
                            if ( nodeSessione.Attributes["Stato"] != null && nodeSessione.Attributes["Stato"].Value == ( Convert.ToInt32( App.TipoTreeNodeStato.Sigillo ) ).ToString() )
                            {
                                ReadOnly = true;
                            }

                            if ( nodeSessione.Attributes["Stato"] != null && nodeSessione.Attributes["Stato"].Value == ( Convert.ToInt32( App.TipoTreeNodeStato.SigilloRotto ) ).ToString() )
                            {
                                ReadOnly = false;
                            }

                            switch ( (App.TipoFile)( System.Convert.ToInt32( IDTree ) ) )
                            {
                                case App.TipoFile.Bilancio:
                                    if (alias.Split('/').Count() > 2)
                                    {
                                        MasterFile mf = MasterFile.Create();
                                        Hashtable hthere = mf.GetBilancioFromFileData(ht[alias].ToString());

                                        if (hthere != null && hthere["Esercizio"] != null)
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                        }
                                        else
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                                        }
                                    }
                                    break;
                                case App.TipoFile.Conclusione:
                                    if (alias.Split('/').Count() > 2)
                                    {
                                        MasterFile mf = MasterFile.Create();
                                        Hashtable hthere = mf.GetConclusioneFromFileData(ht[alias].ToString());

                                        if (hthere != null && hthere["Esercizio"] != null)
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                        }
                                        else
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                                        }
                                    }
                                    break;
                                case App.TipoFile.Revisione:
                                    if (alias.Split('/').Count() > 2)
                                    {
                                        MasterFile mf = MasterFile.Create();
                                        Hashtable hthere = mf.GetRevisioneFromFileData(ht[alias].ToString());

                                        if (hthere != null && hthere["Esercizio"] != null)
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                        }
                                        else
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                                        }
                                    }
                                    break;
                                case App.TipoFile.RelazioneB:
                                    if (alias.Split('/').Count() > 2)
                                    {
                                        MasterFile mf = MasterFile.Create();
                                        Hashtable hthere = mf.GetRelazioneBFromFileData(ht[alias].ToString());

                                        if (hthere != null && hthere["Esercizio"] != null)
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                        }
                                        else
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                                        }
                                    }
                                    break;
                                case App.TipoFile.RelazioneV:
                                    if (alias.Split('/').Count() > 2)
                                    {
                                        MasterFile mf = MasterFile.Create();
                                        Hashtable hthere = mf.GetRelazioneVFromFileData(ht[alias].ToString());

                                        if (hthere != null && hthere["Esercizio"] != null)
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                        }
                                        else
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                                        }
                                    }
                                    break;


                                case App.TipoFile.RelazioneBC:
                                    if (alias.Split('/').Count() > 2)
                                    {
                                        MasterFile mf = MasterFile.Create();
                                        Hashtable hthere = mf.GetRelazioneBCFromFileData(ht[alias].ToString());

                                        if (hthere != null && hthere["Esercizio"] != null)
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                        }
                                        else
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                                        }
                                    }
                                    break;
                                case App.TipoFile.RelazioneVC:
                                    if (alias.Split('/').Count() > 2)
                                    {
                                        MasterFile mf = MasterFile.Create();
                                        Hashtable hthere = mf.GetRelazioneVCFromFileData(ht[alias].ToString());

                                        if (hthere != null && hthere["Esercizio"] != null)
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                        }
                                        else
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                                        }
                                    }
                                    break;

                                case App.TipoFile.RelazioneBV:
                                    if (alias.Split('/').Count() > 2)
                                    {
                                        MasterFile mf = MasterFile.Create();
                                        Hashtable hthere = mf.GetRelazioneBVFromFileData(ht[alias].ToString());

                                        if (hthere != null && hthere["Esercizio"] != null)
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                        }
                                        else
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                                        }
                                    }
                                    break;
                                case App.TipoFile.PianificazioniVigilanza:
                                case App.TipoFile.PianificazioniVerifica:
                                    selectedAliasCodificato = alias.Split( '/' )[0] + "/" + alias.Split( '/' )[1] + "/" + alias.Split( '/' )[2] + ( ( htAliasAdditivo[alias].ToString() == "" ) ? "" : "\r\n" + htAliasAdditivo[alias].ToString() );
                                    break;
                                case App.TipoFile.Vigilanza:
                                case App.TipoFile.Verifica:
                                case App.TipoFile.Incarico:
                                    selectedAliasCodificato = alias.Split( '/' )[0] + "/" + alias.Split( '/' )[1] + "\r\n" + alias.Split( '/' )[2] + ( ( htAliasAdditivo[alias].ToString() == "" ) ? "" : "\r\n" + htAliasAdditivo[alias].ToString() );
                                    break;
                                case App.TipoFile.ISQC:
                                    selectedAliasCodificato = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
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

                            selectedAlias = alias;
                        }
                    }
                    else
                    {
                        nodeSessione = nodeSessioni.OwnerDocument.CreateNode( XmlNodeType.Element, "Sessione", "" );

                        XmlAttribute attr = nodeSessioni.OwnerDocument.CreateAttribute( "Alias" );

                        switch ( (App.TipoFile)( System.Convert.ToInt32( IDTree ) ) )
                        {
                            case App.TipoFile.Bilancio:
                                if (alias.Split('/').Count() > 2)
                                {
                                    MasterFile mf = MasterFile.Create();
                                    Hashtable hthere = mf.GetBilancioFromFileData(ht[alias].ToString());

                                    if (hthere != null && hthere["Esercizio"] != null)
                                    {
                                        if (alias.Split('/').Count() > 2)
                                        {
                                            attr.Value = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                            htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                                        }
                                        else
                                        {
                                            attr.Value = strings[i];
                                        }
                                    }
                                    else
                                    {
                                        if (alias.Split('/').Count() > 2)
                                        {
                                            attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                                            htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                                        }
                                        else
                                        {
                                            attr.Value = strings[i];
                                        }
                                    }
                                }
                                break;
                            case App.TipoFile.Conclusione:
                                if (alias.Split('/').Count() > 2)
                                {
                                    MasterFile mf = MasterFile.Create();
                                    Hashtable hthere = mf.GetConclusioneFromFileData(ht[alias].ToString());

                                    if (hthere != null && hthere["Esercizio"] != null)
                                    {
                                        if (alias.Split('/').Count() > 2)
                                        {
                                            attr.Value = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                            htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                                        }
                                        else
                                        {
                                            attr.Value = strings[i];
                                        }
                                    }
                                    else
                                    {
                                        if (alias.Split('/').Count() > 2)
                                        {
                                            attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                                            htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                                        }
                                        else
                                        {
                                            attr.Value = strings[i];
                                        }
                                    }
                                }
                                break;
                            case App.TipoFile.Revisione:
                                if (alias.Split('/').Count() > 2)
                                {
                                    MasterFile mf = MasterFile.Create();
                                    Hashtable hthere = mf.GetRevisioneFromFileData(ht[alias].ToString());

                                    if (hthere != null && hthere["Esercizio"] != null)
                                    {
                                        if (alias.Split('/').Count() > 2)
                                        {
                                            attr.Value = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                            htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                                        }
                                        else
                                        {
                                            attr.Value = strings[i];
                                        }
                                    }
                                    else
                                    {
                                        if (alias.Split('/').Count() > 2)
                                        {
                                            attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                                            htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                                        }
                                        else
                                        {
                                            attr.Value = strings[i];
                                        }
                                    }
                                }
                                break;
                            case App.TipoFile.RelazioneB:
                                if (alias.Split('/').Count() > 2)
                                {
                                    MasterFile mf = MasterFile.Create();
                                    Hashtable hthere = mf.GetRelazioneBFromFileData(ht[alias].ToString());

                                    if (hthere != null && hthere["Esercizio"] != null)
                                    {
                                        if (alias.Split('/').Count() > 2)
                                        {
                                            attr.Value = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                            htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                                        }
                                        else
                                        {
                                            attr.Value = strings[i];
                                        }
                                    }
                                    else
                                    {
                                        if (alias.Split('/').Count() > 2)
                                        {
                                            attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                                            htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                                        }
                                        else
                                        {
                                            attr.Value = strings[i];
                                        }
                                    }
                                }
                                break;
                            case App.TipoFile.RelazioneV:
                                if (alias.Split('/').Count() > 2)
                                {
                                    MasterFile mf = MasterFile.Create();
                                    Hashtable hthere = mf.GetRelazioneVFromFileData(ht[alias].ToString());

                                    if (hthere != null && hthere["Esercizio"] != null)
                                    {
                                        if (alias.Split('/').Count() > 2)
                                        {
                                            attr.Value = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                            htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                                        }
                                        else
                                        {
                                            attr.Value = strings[i];
                                        }
                                    }
                                    else
                                    {
                                        if (alias.Split('/').Count() > 2)
                                        {
                                            attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                                            htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                                        }
                                        else
                                        {
                                            attr.Value = strings[i];
                                        }
                                    }
                                }
                                break;


                            case App.TipoFile.RelazioneBC:
                                if (alias.Split('/').Count() > 2)
                                {
                                    MasterFile mf = MasterFile.Create();
                                    Hashtable hthere = mf.GetRelazioneBCFromFileData(ht[alias].ToString());

                                    if (hthere != null && hthere["Esercizio"] != null)
                                    {
                                        if (alias.Split('/').Count() > 2)
                                        {
                                            attr.Value = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                            htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                                        }
                                        else
                                        {
                                            attr.Value = strings[i];
                                        }
                                    }
                                    else
                                    {
                                        if (alias.Split('/').Count() > 2)
                                        {
                                            attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                                            htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                                        }
                                        else
                                        {
                                            attr.Value = strings[i];
                                        }
                                    }
                                }
                                break;
                            case App.TipoFile.RelazioneVC:
                                if (alias.Split('/').Count() > 2)
                                {
                                    MasterFile mf = MasterFile.Create();
                                    Hashtable hthere = mf.GetRelazioneVCFromFileData(ht[alias].ToString());

                                    if (hthere != null && hthere["Esercizio"] != null)
                                    {
                                        if (alias.Split('/').Count() > 2)
                                        {
                                            attr.Value = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                            htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                                        }
                                        else
                                        {
                                            attr.Value = strings[i];
                                        }
                                    }
                                    else
                                    {
                                        if (alias.Split('/').Count() > 2)
                                        {
                                            attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                                            htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                                        }
                                        else
                                        {
                                            attr.Value = strings[i];
                                        }
                                    }
                                }
                                break;


                            case App.TipoFile.RelazioneBV:
                                if (alias.Split('/').Count() > 2)
                                {
                                    MasterFile mf = MasterFile.Create();
                                    Hashtable hthere = mf.GetRelazioneBVFromFileData(ht[alias].ToString());

                                    if (hthere != null && hthere["Esercizio"] != null)
                                    {
                                        if (alias.Split('/').Count() > 2)
                                        {
                                            attr.Value = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                            htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2], hthere));
                                        }
                                        else
                                        {
                                            attr.Value = strings[i];
                                        }
                                    }
                                    else
                                    {
                                        if (alias.Split('/').Count() > 2)
                                        {
                                            attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                                            htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                                        }
                                        else
                                        {
                                            attr.Value = strings[i];
                                        }
                                    }
                                }
                                break;
                            case App.TipoFile.PianificazioniVigilanza:
                            case App.TipoFile.PianificazioniVerifica:
                                if ( alias.Split( '/' ).Count() > 2 && htAliasAdditivo[alias] != null )
                                {
                                    attr.Value = alias.Split( '/' )[0] + "/" + alias.Split( '/' )[1] + "/" + alias.Split( '/' )[2] + ( ( htAliasAdditivo[alias].ToString() == "" ) ? "" : "\r\n" + htAliasAdditivo[alias].ToString() );
                                    if ( !htSessioniAlias.ContainsKey( i ) )
                                    {
                                        htSessioniAlias.Add( i, alias.Split( '/' )[0] + "/" + alias.Split( '/' )[1] + "/" + alias.Split( '/' )[2] + ( ( htAliasAdditivo[alias].ToString() == "" ) ? "" : " - " + htAliasAdditivo[alias].ToString() ) );
                                    }
                                }
                                else
                                {
                                    attr.Value = strings[i];
                                }
                                break;
                            case App.TipoFile.Vigilanza:
                            case App.TipoFile.Verifica:
                            case App.TipoFile.Incarico:
                                if ( alias.Split( '/' ).Count() > 2 && htAliasAdditivo[alias] != null )
                                {
                                    attr.Value = alias.Split( '/' )[0] + "/" + alias.Split( '/' )[1] + "\r\n" + alias.Split( '/' )[2] + ( ( htAliasAdditivo[alias].ToString() == "" ) ? "" : "\r\n" + htAliasAdditivo[alias].ToString() );
                                    if ( !htSessioniAlias.ContainsKey( i ) )
                                    {
                                        htSessioniAlias.Add( i, alias.Split( '/' )[0] + "/" + alias.Split( '/' )[1] + "/" + alias.Split( '/' )[2] + ( ( htAliasAdditivo[alias].ToString() == "" ) ? "" : " - " + htAliasAdditivo[alias].ToString() ) );
                                    }
                                }
                                else
                                {
                                    attr.Value = strings[i];
                                }
                                break;
                            case App.TipoFile.ISQC:
                                if (alias.Split('/').Count() > 2 && htAliasAdditivo[alias] != null)
                                {
                                    attr.Value = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
                                    if (!htSessioniAlias.ContainsKey(i))
                                    {
                                        htSessioniAlias.Add(i, alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : " - " + htAliasAdditivo[alias].ToString()));
                                    }
                                }
                                else
                                {
                                    attr.Value = strings[i];
                                }
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

                        nodeSessione.Attributes.Append( attr );

                        attr = nodeSessioni.OwnerDocument.CreateAttribute( "Selected" );
                        if ( SelectedDataSource == ht[alias].ToString() )
                        {
                            switch ( (App.TipoFile)( System.Convert.ToInt32( IDTree ) ) )
                            {
                                case App.TipoFile.Bilancio:
                                    if (alias.Split('/').Count() > 2)
                                    {
                                        MasterFile mf = MasterFile.Create();
                                        Hashtable hthere = mf.GetBilancioFromFileData(ht[alias].ToString());

                                        if (hthere != null && hthere["Esercizio"] != null)
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                        }
                                        else
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                                        }
                                    }
                                    break;
                                case App.TipoFile.Conclusione:
                                    if (alias.Split('/').Count() > 2)
                                    {
                                        MasterFile mf = MasterFile.Create();
                                        Hashtable hthere = mf.GetConclusioneFromFileData(ht[alias].ToString());

                                        if (hthere != null && hthere["Esercizio"] != null)
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                        }
                                        else
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                                        }
                                    }
                                    break;
                                case App.TipoFile.Revisione:
                                    if (alias.Split('/').Count() > 2)
                                    {
                                        MasterFile mf = MasterFile.Create();
                                        Hashtable hthere = mf.GetRevisioneFromFileData(ht[alias].ToString());

                                        if (hthere != null && hthere["Esercizio"] != null)
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                        }
                                        else
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                                        }
                                    }
                                    break;
                                case App.TipoFile.RelazioneB:
                                    if (alias.Split('/').Count() > 2)
                                    {
                                        MasterFile mf = MasterFile.Create();
                                        Hashtable hthere = mf.GetRelazioneBFromFileData(ht[alias].ToString());

                                        if (hthere != null && hthere["Esercizio"] != null)
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                        }
                                        else
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                                        }
                                    }
                                    break;
                                case App.TipoFile.RelazioneV:
                                    if (alias.Split('/').Count() > 2)
                                    {
                                        MasterFile mf = MasterFile.Create();
                                        Hashtable hthere = mf.GetRelazioneVFromFileData(ht[alias].ToString());

                                        if (hthere != null && hthere["Esercizio"] != null)
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                        }
                                        else
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                                        }
                                    }
                                    break;


                                case App.TipoFile.RelazioneBC:
                                    if (alias.Split('/').Count() > 2)
                                    {
                                        MasterFile mf = MasterFile.Create();
                                        Hashtable hthere = mf.GetRelazioneBCFromFileData(ht[alias].ToString());

                                        if (hthere != null && hthere["Esercizio"] != null)
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                        }
                                        else
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                                        }
                                    }
                                    break;
                                case App.TipoFile.RelazioneVC:
                                    if (alias.Split('/').Count() > 2)
                                    {
                                        MasterFile mf = MasterFile.Create();
                                        Hashtable hthere = mf.GetRelazioneVCFromFileData(ht[alias].ToString());

                                        if (hthere != null && hthere["Esercizio"] != null)
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                        }
                                        else
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                                        }
                                    }
                                    break;

                                case App.TipoFile.RelazioneBV:
                                    if (alias.Split('/').Count() > 2)
                                    {
                                        MasterFile mf = MasterFile.Create();
                                        Hashtable hthere = mf.GetRelazioneBVFromFileData(ht[alias].ToString());

                                        if (hthere != null && hthere["Esercizio"] != null)
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2], hthere);
                                        }
                                        else
                                        {
                                            selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                                        }
                                    }
                                    break;
                                case App.TipoFile.PianificazioniVigilanza:
                                case App.TipoFile.PianificazioniVerifica:
                                    selectedAliasCodificato = alias.Split( '/' )[0] + "/" + alias.Split( '/' )[1] + "/" + alias.Split( '/' )[2] + ( ( htAliasAdditivo[alias].ToString() == "" ) ? "" : "\r\n" + htAliasAdditivo[alias].ToString() );
                                    break;
                                case App.TipoFile.Vigilanza:
                                case App.TipoFile.Verifica:
                                case App.TipoFile.Incarico:
                                    selectedAliasCodificato = alias.Split( '/' )[0] + "/" + alias.Split( '/' )[1] + "\r\n" + alias.Split( '/' )[2] + ( ( htAliasAdditivo[alias].ToString() == "" ) ? "" : "\r\n" + htAliasAdditivo[alias].ToString() );
                                    break;
                                case App.TipoFile.ISQC:
                                    selectedAliasCodificato = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
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

                            selectedAlias = alias;
                            attr.Value = "#AA" + YearColor[-1].ToString();
                        }
                        else
                        {
                            attr.Value = "White";
                        }
                        nodeSessione.Attributes.Append( attr );

                        nodeSessioni.AppendChild( nodeSessione );
                    }
                }

                ArrayList alCheckCompletezzaNodiNOMORE = new ArrayList();

                foreach ( string ID in alCheckCompletezzaNodi )
                {
                    nodeTree = TreeXmlProvider.Document.SelectSingleNode( "/Tree//Node[@ID=" + ID + "]" );

                    if ( nodeTree == null )
                    {
                        if ( !alCheckCompletezzaNodiNOMORE.Contains( ID ) )
                        {
                            alCheckCompletezzaNodiNOMORE.Add( ID );
                        }
                    }
                    else
                    {
                        XmlNode node = tmpDoc.SelectSingleNode( "/Dati//Dato[@ID='" + ID + "']" );

                        if ( node != null && !htStati.ContainsKey( node.Attributes["ID"].Value ) )
                        {
                            htStati.Add( node.Attributes["ID"].Value, node );
                        }

                        XmlNode nodeSessioni = nodeTree.SelectSingleNode( "Sessioni" );
                        //PRISC DA CONTROLLARE PER SIGILLO -- inizio
                        if ( i == 0 && nodeSessioni != null && !alias.Contains( "S1" ) && !alias.Contains( "S2" ) && !alias.Contains( "S3" ) )
                        {
                            nodeSessioni.ParentNode.RemoveChild( nodeSessioni );
                            nodeSessioni = null;
                        }
                        //PRISC DA CONTROLLARE PER SIGILLO -- fine

                        if ( nodeSessioni == null )
                        {
                            XmlNode newElemOut = nodeTree.OwnerDocument.CreateNode( XmlNodeType.Element, "Sessioni", "" );
                            nodeTree.AppendChild( newElemOut );
                            nodeSessioni = nodeTree.SelectSingleNode( "Sessioni" );
                        }

                        XmlNode nodeSessione = nodeTree.SelectSingleNode( "Sessioni/Sessione[@Alias=\"" + aliastocheck + "\"]" );

                        if ( nodeSessione == null )
                        {

                            nodeSessione = nodeSessioni.OwnerDocument.CreateNode( XmlNodeType.Element, "Sessione", "" );

                            XmlAttribute attr = nodeSessioni.OwnerDocument.CreateAttribute( "Alias" );

                            if ( aliastocheck != "" )
                            {
                                attr.Value = aliastocheck;
                            }
                            else
                            {
                                attr.Value = strings[i];
                            }

                            nodeSessione.Attributes.Append( attr );

                            attr = nodeSessioni.OwnerDocument.CreateAttribute( "Stato" );

                            if ( nodeTree != null && nodeTree.ParentNode != null && nodeTree.ParentNode.Name == "Tree" )
                            {
                                if ( attr.Value == ( Convert.ToInt32( App.TipoTreeNodeStato.Sigillo ) ).ToString() || attr.Value == ( Convert.ToInt32( App.TipoTreeNodeStato.SigilloRotto ) ).ToString() )
                                {
                                    ;
                                }
                                else
                                {
                                    if ( SelectedDataSource == ht[alias].ToString() && nodeTree.Attributes["Osservazioni"] != null && nodeTree.Attributes["Osservazioni"].Value.Trim() != "" )
                                    {
                                        attr.Value = ( Convert.ToInt32( App.TipoTreeNodeStato.NodoFazzoletto ) ).ToString();
                                    }
                                    else
                                    {
                                        attr.Value = ( Convert.ToInt32( App.TipoTreeNodeStato.NodoFazzoletto ) ).ToString();
                                    }
                                }
                            }
                            else
                            {
                                attr.Value = getStato( nodeTree, tmpDoc );
                                                                
                                if (chkNA.Contains(ID) && i == 1 && attr.Value == (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile)).ToString())
                                {
                                    TreeXmlProvider.Document.SelectSingleNode(chkNA[ID].ToString()).Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile)).ToString();
                                    XmlNode xtbdh = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + ID + "']");
                                    if (xtbdh.Attributes["Stato"] == null)
                                    {
                                        XmlAttribute attrh = _x.Document.CreateAttribute("Stato");
                                        xtbdh.Attributes.Append(attrh);
                                    }
                                    xtbdh.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile)).ToString();
                                }
                                else
                                {
                                    if (nodeTree.Attributes["Report"].Value == "True")
                                    {
                                        attr.Value = (Convert.ToInt32(App.TipoTreeNodeStato.SolaLettura)).ToString();
                                    }
                                }
                            }

                            nodeSessione.Attributes.Append( attr );

                            if ( i == 0 && attr.Value == "-1" )
                            {
                                if ( !chkNA.ContainsKey( ID ) )
                                {
                                    chkNA.Add( ID, "/Tree//Node[@ID=" + ID + "]/Sessioni/Sessione[@Alias=\"" + aliastocheck + "\"]" );
                                }
                            }

                            nodeSessioni.AppendChild( nodeSessione );
                        }

                        nodeSessione = nodeTree.SelectSingleNode( "Sessioni/Sessione[@Alias=\"" + aliastocheck + "\"]" );

                        if ( IDTree == "2" || IDTree == "18" )
                        {
                            foreach ( XmlDataProviderManager ALitemXTPP in ALXTPP )
                            {
                                bool donehere = false;

                                string IDPHERE = "";

                                if ( IDTree == "2" )
                                {
                                    IDPHERE = "100013";
                                }
                                else
                                {
                                    IDPHERE = "100003";
                                }

                                foreach ( XmlNode itemXPP in ALitemXTPP.Document.SelectNodes( "//Dato[@ID=\"" + IDPHERE + "\"]/Valore[@ID=\"" + ID + "\"]/Pianificazione" ) )
                                {
                                    if ( nodeSessione.Attributes["Pianificato"] == null )
                                    {
                                        XmlAttribute attr = nodeSessione.OwnerDocument.CreateAttribute( "Pianificato" );
                                        attr.Value = "";
                                        nodeSessione.Attributes.Append( attr );
                                    }

                                    if ( nodeSessione.Attributes["Alias"].Value.Replace( "\r\n", "/" ) == itemXPP.Attributes["Data"].Value && itemXPP.Attributes["Checked"] != null && itemXPP.Attributes["Checked"].Value == "True" )
                                    {
                                        nodeSessione.Attributes["Pianificato"].Value = "P";

                                        XmlNode AppoggioNode = nodeTree.ParentNode;
                                        while ( AppoggioNode != null && AppoggioNode.ParentNode.Name != "Tree")
                                        {
                                            XmlNode AppoggioNodeHere = AppoggioNode.SelectSingleNode( "Sessioni/Sessione[@Alias=\"" + aliastocheck + "\"]" );
                                            if ( AppoggioNodeHere != null )
                                            {
                                                if ( AppoggioNodeHere.Attributes["Pianificato"] == null )
                                                {
                                                    XmlAttribute attr = AppoggioNodeHere.OwnerDocument.CreateAttribute( "Pianificato" );
                                                    attr.Value = "";
                                                    AppoggioNodeHere.Attributes.Append( attr );
                                                }

                                                AppoggioNodeHere.Attributes["Pianificato"].Value = "P";
                                                AppoggioNode = AppoggioNode.ParentNode;
                                            }
                                            else
                                            {
                                                AppoggioNode = null;
                                            }
                                        }

                                        donehere = true;
                                        break;
                                    }
                                    else
                                    {
                                        nodeSessione.Attributes["Pianificato"].Value = "";
                                    }
                                }

                                if(donehere)
                                {
                                    break;
                                }
                            }
                        }

                        if ( nodeSessione.Attributes["Selected"] == null )
                        {
                            XmlAttribute attr2 = nodeSessioni.OwnerDocument.CreateAttribute( "Selected" );
                            nodeSessione.Attributes.Append( attr2 );
                        }

                        if ( SelectedDataSource == ht[alias].ToString() )
                        {
                            nodeSessione.Attributes["Selected"].Value = "#AA" + YearColor[-1].ToString();
                        }
                        else
                        {
                            int anno = Convert.ToInt32( alias.Substring( alias.Length - 4, 4 ) );

                            if ( i % 2 == 0 )
                            {
                                nodeSessione.Attributes["Selected"].Value = "#80" + YearColor[anno].ToString();
                            }
                            else
                            {
                                nodeSessione.Attributes["Selected"].Value = "#AA" + YearColor[anno].ToString();
                            }
                        }

                    }
                }

                nodeTree = TreeXmlProvider.Document.SelectSingleNode( "/Tree/Node/Sessioni/Sessione[@Alias=\"" + aliastocheck + "\"]" );

                if ( nodeTree != null && htSessioneSigillo[alias] != null && htSessioneSigillo[alias].ToString() != "" )
                {
                    if ( nodeTree.Attributes["Stato"] == null )
                    {
                        XmlAttribute attrnew = nodeTree.OwnerDocument.CreateAttribute( "Stato" );
                        nodeTree.Attributes.Append( attrnew );
                    }
                    nodeTree.Attributes["Stato"].Value = ( Convert.ToInt32( App.TipoTreeNodeStato.Sigillo ) ).ToString();

                    if ( nodeTree.Attributes["ToolTip"] == null )
                    {
                        XmlAttribute attrnew = nodeTree.OwnerDocument.CreateAttribute( "ToolTip" );
                        nodeTree.Attributes.Append( attrnew );
                    }
                    nodeTree.Attributes["ToolTip"].Value = "Applicato da " + htSessioneSigillo[alias].ToString() + " il " + htSessioneSigilloData[alias].ToString();

                    if ( nodeTree.Attributes["Revisore"] == null )
                    {
                        XmlAttribute attrnew = nodeTree.OwnerDocument.CreateAttribute( "Revisore" );
                        nodeTree.Attributes.Append( attrnew );
                    }
                    nodeTree.Attributes["Revisore"].Value = htSessioneSigillo[alias].ToString();

                    if ( nodeTree.Attributes["Password"] == null )
                    {
                        XmlAttribute attrnew = nodeTree.OwnerDocument.CreateAttribute( "Password" );
                        nodeTree.Attributes.Append( attrnew );
                    }
                    nodeTree.Attributes["Password"].Value = htSessioneSigilloPassword[alias].ToString();

                }

                foreach ( string itemTOBEDELETED in alCheckCompletezzaNodiNOMORE )
                {
                    alCheckCompletezzaNodi.Remove( itemTOBEDELETED );
                }

                _x.Save();
            }

            SaveTreeSource();
        }

        

        

		private void ReloadStatoNodiPadre()
		{
			foreach (XmlNode nodeTree in TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"))
			{

				if (nodeTree.ChildNodes.Count > 1 && nodeTree.Name == "Node")//nodeTree != null)
                {
					XmlNode nodeSessione = nodeTree.SelectSingleNode("Sessioni/Sessione[@Selected='#AA82BDE4']");

					if (nodeTree.ParentNode.Name == "Tree")
					{
                        if ( nodeSessione.Attributes["Stato"].Value == (Convert.ToInt32( App.TipoTreeNodeStato.Sigillo )).ToString() || nodeSessione.Attributes["Stato"].Value == (Convert.ToInt32( App.TipoTreeNodeStato.SigilloRotto )).ToString() )
                        {
                            ;
                        }
                        else
                        {
                            if ( nodeTree.Attributes["Osservazioni"] != null && nodeTree.Attributes["Osservazioni"].Value.Trim() != "" )
                            {
                                if ( nodeSessione != null && nodeSessione.Attributes["Stato"] != null )
                                {
                                    nodeSessione.Attributes["Stato"].Value = (Convert.ToInt32( App.TipoTreeNodeStato.NodoFazzoletto )).ToString();
                                }
                            }
                            else
                            {
                                if ( nodeSessione != null && nodeSessione.Attributes["Stato"] != null )
                                {
                                    nodeSessione.Attributes["Stato"].Value = ( Convert.ToInt32( App.TipoTreeNodeStato.NodoFazzoletto ) ).ToString();
                                }
                            }
                        }
					}
					else
					{
						if (nodeSessione != null && nodeSessione.Attributes["Stato"] != null)
						{
							foreach (XmlNode nodeTreeSecondoLivello in nodeTree.ChildNodes)
							{
								if (nodeTreeSecondoLivello.ChildNodes.Count > 1 && nodeTreeSecondoLivello.Name == "Node")
								{
									XmlNode nodeSessioneSecondoLivello = nodeTreeSecondoLivello.SelectSingleNode("Sessioni/Sessione[@Selected='#AA82BDE4']");

									if (nodeSessioneSecondoLivello != null && nodeSessioneSecondoLivello.Attributes["Stato"] != null)
									{
										nodeSessioneSecondoLivello.Attributes["Stato"].Value = getStato(nodeTreeSecondoLivello, _x.Document);
									}
								}
							}

							nodeSessione.Attributes["Stato"].Value = getStato(nodeTree, _x.Document);
						}
					}                 
                }
            }

			_x.Save();
		}

		private string getStato(XmlNode nodeTree, XmlDocument tmpDoc)
		{
			string returnvalue = "";
			string statotmp = (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString();

			if (nodeTree.ChildNodes.Count > 1 && !(nodeTree.Attributes["Tipologia"].Value == "Nodo Multiplo") && !(nodeTree.Attributes["Report"].Value == "True"))
			{
				foreach (XmlNode nodesStati in nodeTree.ChildNodes)
				{
					if (nodesStati.Name == "Node")
					{
						if (returnvalue != (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString()
            // ANDREA && returnvalue != (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString()
            )
						{
							statotmp = getStato(nodesStati, tmpDoc);

							if (statotmp == (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString())
							{
								returnvalue = statotmp;
							}
							else
							{
								if (statotmp == (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString())
								{
									if (returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString() || returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.VociCompilate)).ToString())
									{
										returnvalue = (Convert.ToInt32(App.TipoTreeNodeStato.VociCompilate)).ToString();
									}
									else
									{
										returnvalue = statotmp;
									}
								}
								else
								{
									if ((statotmp == (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile)).ToString() || statotmp == (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabileBucoTemplate)).ToString()) && ((returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile)).ToString() || returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabileBucoTemplate)).ToString()) || returnvalue == ""))
									{
										returnvalue = (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString();
									}

									if (statotmp == (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString() || statotmp == (Convert.ToInt32(App.TipoTreeNodeStato.VociCompilate)).ToString())
									{
										if(returnvalue == "")
										{
											returnvalue = statotmp;
										}
										else
										{
											if (returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.VociCompilate)).ToString() || returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString())
											{
												returnvalue = (Convert.ToInt32(App.TipoTreeNodeStato.VociCompilate)).ToString();
											}
										}
									}
								}
							}
						}
						else
						{
							break;
						}						
					}
				}
			}
			else
			{
				if (nodeTree.Attributes["Report"].Value == "True")
				{
					returnvalue = (Convert.ToInt32(App.TipoTreeNodeStato.Report)).ToString();
				}
				else
				{
					XmlNode node = tmpDoc.SelectSingleNode("/Dati//Dato[@ID='" + nodeTree.Attributes["ID"].Value + "']");

                    if ( node == null)
                    {
                        if(nodeTree.Attributes["ID"].Value == "138")
                        {
                            node = tmpDoc.SelectSingleNode("/Dati//Dato[@ID='84']");
                        }

                        if ( nodeTree.Attributes["ID"].Value == "84" )
                        {
                            node = tmpDoc.SelectSingleNode( "/Dati//Dato[@ID='138']" );
                        }
                    }


					if (node != null)
					{
						if (node.Attributes["Stato"] != null)
						{
							returnvalue = node.Attributes["Stato"].Value;
						}
						else
						{
							XmlAttribute attr2 = node.OwnerDocument.CreateAttribute("Stato");
							attr2.Value = "-1";
							node.Attributes.Append(attr2);

							returnvalue = (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString();
						}
					}
					else
					{
						//returnvalue = "-1";
                        returnvalue = ( Convert.ToInt32( App.TipoTreeNodeStato.NonApplicabileBucoTemplate ) ).ToString();
					}
				}
			}

            return returnvalue;
		}

#endregion
        
        private void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ;
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string SearchFor = ((TextBox)sender).Text.ToUpper();
            //int foundID = -1;
            bool found = false;

            if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
            {
                foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
                {
                    if (item.Attributes["Selected"] != null)
                    {
						//if (item.Attributes["Selected"].Value == "True")
						//{
						//    foundID = Convert.ToInt32(item.Attributes["ID"].Value);
						//}

                        item.Attributes["Selected"].Value = "False";
                    }

					if (item.Attributes["HighLighted"] != null)
					{
						item.Attributes["HighLighted"].Value = "Black";
					}
                }

                foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
                {
					if (
          //found == false && foundID != Convert.ToInt32(item.Attributes["ID"].Value) &&
          (item.Attributes["Titolo"].Value.ToUpper().Contains(SearchFor) || item.Attributes["Codice"].Value.ToUpper().Contains(SearchFor)))
                    {
                        found = true;
                        item.Attributes["HighLighted"].Value = "Red";

                        if(item.ParentNode != null)
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
                MessageBox.Show( "Nessuna Carta di Lavoro presente per il testo ricercato" );
            }
        }

        private void ItemsControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
			;
        }
		
#region RICERCA_TESTO

        private void searchTextBox_KeyUp()
        {
            //if (e.Key == Key.Enter || e.Key == Key.Tab)
            //{
                string SearchFor = searchTextBox.Text.Trim().ToUpper();
                //int foundID = -1;
                bool found = false;

                if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
                {
                    foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
                    {
                        if (item.Attributes["Selected"] != null)
                        {
							//if (item.Attributes["Selected"].Value == "True")
							//{
							//    foundID = Convert.ToInt32(item.Attributes["ID"].Value);
							//}

                            item.Attributes["Selected"].Value = "False";
                        }

						if (item.Attributes["Expanded"] != null)
						{
							if (item.ParentNode.Name == "Tree")
							{
								item.Attributes["Expanded"].Value = "True";
							}
							else
							{
								item.Attributes["Expanded"].Value = "False";
							}
						}

						if (item.Attributes["HighLighted"] != null)
						{
							item.Attributes["HighLighted"].Value = "Black";
						}
                    }

					if (SearchFor == "")
					{
						return;
					}

                    foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
                    {
                        if (
                        //found == false && foundID != Convert.ToInt32(item.Attributes["ID"].Value) &&
                        (item.Attributes["Titolo"].Value.ToUpper().Contains(SearchFor) || item.Attributes["Codice"].Value.ToUpper().Contains(SearchFor)))
                        {
                            found = true;
							item.Attributes["HighLighted"].Value = "Red";

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
                    MessageBox.Show( "Nessuna Carta di Lavoro presente per il testo ricercato" );
                }
            //}
        }

        private void buttonCerca_Click(object sender, RoutedEventArgs e)
        {
            searchTextBox_KeyUp();
        }

        private void buttonCercaAnnulla_Click(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = "";
            searchTextBox_KeyUp();
        }

#endregion
		
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //MessageBoxResult result = MessageBox.Show("Si vuole lasciare un promemoria inerente a questa sessione?", "Attenzione", MessageBoxButton.YesNoCancel);
            //if ( result == MessageBoxResult.Yes )
            //{
            //    NodoFazzoletto o = new NodoFazzoletto();
            //    o.Owner = this;
            //    o.ApertoInSolaLettura = ApertoInSolaLettura;
            //    o.Nodo = TreeXmlProvider.Document.SelectSingleNode("/Tree/Node").Attributes["ID"].Value;
            //    o.Load();
            //    o.ShowDialog();
            //}
            //else if ( result == MessageBoxResult.Cancel )
            //{
            //    e.Cancel = true;
            //    return;
            //}


			_x.Save();
            SaveTreeSourceNoReload();
        }

        Brush ButtonStatoSelectedColor = new SolidColorBrush( Color.FromArgb( 255, 247, 168, 39 ) );
        Color ButtonToolBarSelectedColor = Color.FromArgb( 126, 130, 189, 228 );
        Color ButtonToolBarPulseColor = Color.FromArgb( 126, 82, 101, 115 );

        private void AnimateBackgroundColor( Button btn, Color from, Color to, int seconds )
        {
            SolidColorBrush brush = new SolidColorBrush( from );

            btn.Background = brush;
            System.Windows.Media.Animation.ColorAnimation a = new System.Windows.Media.Animation.ColorAnimation();
            a.From = from;
            a.To = to;
            a.Duration = new Duration( TimeSpan.FromSeconds( seconds ) );
            a.AutoReverse = true;
            btn.Background.BeginAnimation( SolidColorBrush.ColorProperty, a );
        }

		private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
		{
			try
			{
				if (((XmlNode)(tvMain.SelectedItem)).ChildNodes.Count > 1 && !(((XmlNode)(tvMain.SelectedItem)).Attributes["Tipologia"].Value == "Nodo Multiplo"))
				{
					btn_NonApplicabile.IsEnabled = false;
				}
				else
				{
					btn_NonApplicabile.IsEnabled = true;

                    //try
                    //{
                        XmlNode node = ((XmlNode)(tvMain.SelectedItem));
                        string nota = node.Attributes["Nota"].Value;
                        if ( nota != "" && nota != "<P align=left>&nbsp;</P>" && nota != "<P class=MsoNormal style=\"MARGIN: 0cm 0cm 0pt; LINE-HEIGHT: normal\" align=left>&nbsp;</P>" )
                        {
                            AnimateBackgroundColor( btn_GuidaRevisoft, ButtonToolBarSelectedColor, ButtonToolBarPulseColor, 1 );
                        }
                        else
                        {
                            btn_GuidaRevisoft.Background = btn_ArchivioAllegati.Background;
                        }
                    //}
                    //catch ( Exception ex )
                    //{
                    //    string log = ex.Message;
                    //}
				}

                btn_CopiaLibroSociale.Visibility = System.Windows.Visibility.Collapsed;

                string IDHere = ((XmlNode)(tvMain.SelectedItem)).Attributes["ID"].Value;
                if ((IDTree == "2" && (IDHere == "20" || IDHere == "21" || IDHere == "22" || IDHere == "23" || IDHere == "24" || IDHere == "146")) || (IDTree == "18" && (IDHere == "600" || IDHere == "601" || IDHere == "602" || IDHere == "603" || IDHere == "604" || IDHere == "605")))
                {
                    btn_CopiaLibroSociale.Visibility = System.Windows.Visibility.Visible;
                }

            }
			catch (Exception ex)
			{
				string log = ex.Message;
			}	
		}

		private void OnItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
            XmlNode nodeSessione = null;
            XmlNode removable = null;
            XmlNode imported = null;

			if (e.ClickCount != 2)
			{
				return;
			}

			XmlNode node;

			try
			{
				node = ((XmlNode)(tvMain.SelectedItem));
				//node = TreeXmlProvider.Document.SelectSingleNode("/Tree//Node[@ID='" + node.Attributes["ID"].Value + "']");
			}
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

			if (node.ParentNode == null)
			{
				e.Handled = true;
				return;
			}

			if (node.ParentNode.Name == "Tree")
			{
                XmlNode nodeTree = TreeXmlProvider.Document.SelectSingleNode( "/Tree/Node" );
                nodeSessione = nodeTree.SelectSingleNode( "Sessioni/Sessione[@Selected='#AA82BDE4']" );

                XmlNode nodeTreePadre = TreeXmlProvider.Document.SelectSingleNode( "/Tree" );
                XmlNode nodeTreeSessione = nodeTreePadre.SelectSingleNode( "Sessioni/Sessione[@Selected='#AA82BDE4']" );

                if ( nodeSessione.Attributes["Stato"].Value == (Convert.ToInt32( App.TipoTreeNodeStato.Sigillo )).ToString() )
                {
                    wSigilloSbloccoBlocco sb = new wSigilloSbloccoBlocco();
                    sb.Titolo = "Sblocca Sigillo";
                    sb.Nodo = nodeSessione;
                    sb.NodoTree = nodeTreeSessione;
                    sb.IDCliente = IDCliente;

                    MasterFile mf = MasterFile.Create();
                    Hashtable hthere = mf.GetBilancioFromFileData(SessioneFile);

                    if (hthere != null && hthere["Esercizio"] != null)
                    {
                        sb.AliasSessione = ConvertDataToEsercizio(selectedAlias, hthere);
                    }
                    else
                    {
                        sb.AliasSessione = ConvertDataToEsercizio(selectedAlias);
                    }
                        
                    sb.Owner = this;
                    sb.Load();
                    sb.ShowDialog();
                }
                //else if ( nodeSessione.Attributes["Stato"].Value == (Convert.ToInt32( App.TipoTreeNodeStato.SigilloRotto )).ToString() )
                //{
                //    wSigilloSbloccoBlocco sb = new wSigilloSbloccoBlocco();
                //    sb.Titolo = "Ri Applica Sigillo";
                //    sb.Nodo = nodeSessione;
                //    sb.NodoTree = nodeTreeSessione;
                //    sb.Owner = this;
                //    sb.Load();
                //    sb.ShowDialog();
                //}
                else
                {
                    NodoFazzoletto o = new NodoFazzoletto();
                    o.Owner = this;
                    o.ApertoInSolaLettura = ApertoInSolaLettura;
                    o.ReadOnly = ReadOnly;
                    o.Nodo = node.Attributes["ID"].Value;
                    o.Load();
                    o.ShowDialog();

                    if ( nodeTree.Attributes["Osservazioni"] != null && nodeTree.Attributes["Osservazioni"].Value.Trim() != "" )
                    {
                        if ( nodeSessione.Attributes["Stato"] != null )
                        {
                            nodeSessione.Attributes["Stato"].Value = (Convert.ToInt32( App.TipoTreeNodeStato.NodoFazzoletto )).ToString();
                        }
                    }
                    else
                    {
                        if ( nodeSessione.Attributes["Stato"] != null )
                        {
                            nodeSessione.Attributes["Stato"].Value = ( Convert.ToInt32( App.TipoTreeNodeStato.NodoFazzoletto ) ).ToString();
                        }
                    }
                }

				SaveTreeSource();			
				e.Handled = true;
				return;
			}

			try
			{
                if(node.Attributes["Titolo"].Value.Contains("Utilizzata sino a ver. 4.1"))
                {
                    e.Handled = true;
                    return;
                }

                if (node.Attributes["ID"].Value == "278" && IDTree == "4")
                {
                    wCampionamento wcnn = new wCampionamento(node.SelectSingleNode("Sessioni/Sessione[@Alias=\"" + selectedAliasCodificato + "\"]"), IDCliente, Cliente, Esercizio, IDSessione, IDTree);
                    wcnn.ShowDialog();

                    RevisoftApplication.XmlManager xx = new XmlManager();
                    xx.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                    TreeXmlProvider.Document = xx.LoadEncodedFile(SelectedTreeSource);

                    TreeXmlProvider.Refresh();

                    XmlNode nodehere = TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=" + wcnn.changedID + "]");
                    XmlNode nodeSessionehere = nodehere.SelectSingleNode("Sessioni/Sessione[@Alias=\"" + selectedAliasCodificato + "\"]");

                    XmlNode paretntnodeherehere = nodehere.ParentNode;
                    while (paretntnodeherehere != null && paretntnodeherehere.Attributes["Expanded"] != null)
                    {
                        paretntnodeherehere.Attributes["Expanded"].Value = "True";
                        paretntnodeherehere = paretntnodeherehere.ParentNode;
                    }

                    if (nodeSessionehere != null && nodeSessionehere.Attributes["Stato"].Value != (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString())
                    {
                        nodeSessionehere.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString();
                    }

                    if (_x.Document.SelectSingleNode("//Dati//Dato[@ID='" + wcnn.changedID + "']").Attributes["Stato"] == null)
                    {
                        XmlAttribute attr = _x.Document.CreateAttribute("Stato");
                        _x.Document.SelectSingleNode("//Dati//Dato[@ID='" + wcnn.changedID + "']").Attributes.Append(attr);
                    }
                    _x.Document.SelectSingleNode("//Dati//Dato[@ID='" + wcnn.changedID + "']").Attributes["Stato"].Value = nodeSessionehere.Attributes["Stato"].Value;
                    _x.Save();

                    ReloadStatoNodiPadre();

                    SaveTreeSource();

                    e.Handled = true;
                    
                    return;
                }

                if (node.Attributes["ID"].Value == "100013" && IDTree == "26")
                {
                    XmlNode pianificazioniNode = _x.Document.SelectSingleNode( "//Dato[@ID=\"100013\"]" );

                    Hashtable htSessioniP = new Hashtable();
                    string lastData = "";
                    string lastKey = "";

                    foreach ( XmlNode pianificazioneNode in pianificazioniNode.SelectNodes( "//Pianificazione" ) )
                    {
                        if ( !htSessioniP.Contains( pianificazioneNode.Attributes["ID"].Value ) )
                        {
                            htSessioniP.Add( pianificazioneNode.Attributes["ID"].Value, pianificazioneNode.Attributes["Data"].Value );
                            lastData = pianificazioneNode.Attributes["Data"].Value;
                            lastKey = pianificazioneNode.Attributes["ID"].Value;
                        }
                    }

                    MasterFile mf = MasterFile.Create();
                    Hashtable htmf = mf.GetPianificazioniVerifica( IDSessione );

                    wSchedaSessioniPianificazioniVerifiche sspve = new wSchedaSessioniPianificazioniVerifiche();

                    sspve.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    switch ( base.WindowState )
                    {
                        case System.Windows.WindowState.Normal:
                            sspve.Width = ActualWidth * 97 / 100;
                            sspve.Height = ActualHeight * 97 / 100;
                            break;
                        case System.Windows.WindowState.Maximized:
                            sspve.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 97 / 100;
                            sspve.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 97 / 100;
                            break;
                    }

                    nodeSessione = node.SelectSingleNode( "Sessioni/Sessione[@Alias=\"" + selectedAliasCodificato + "\"]" );
                    if ( nodeSessione != null )
                    {
                        sspve.Stato = ( (App.TipoTreeNodeStato)( Convert.ToInt32( nodeSessione.Attributes["Stato"].Value ) ) );
                        sspve.OldStatoNodo = sspve.Stato;
                    }


                    XmlNode nodeNota = ( (XmlNode)( tvMain.SelectedItem ) );
                    sspve.nota = node.Attributes["Nota"].Value;

                    sspve.lastData = lastData;
                    sspve.lastKey = lastKey; 
                    sspve.DataInizio = htmf["DataInizio"].ToString();
                    sspve.DataFine = htmf["DataFine"].ToString();
                    sspve.htSessioni = htSessioniP;
                    sspve.Cliente = Cliente;
                    sspve.IDCliente = IDCliente;
                    sspve._x = _x;
                    sspve.ReadOnly = ReadOnly;

                    sspve.IDTree = IDTree;
                    sspve.IDSessione = IDSessione;                    

                    if(htSessioniP.Count == 0)
                    {
                        sspve.generateTree();
                    }

                    sspve.ConfiguraMaschera();
                    sspve.ShowDialog();
                    sspve.Activate();

                    _x.Save();

                    if ( nodeSessione != null && ( sspve.Stato != App.TipoTreeNodeStato.Sconosciuto || ( sspve.Stato == App.TipoTreeNodeStato.Sconosciuto && nodeSessione.Attributes["Stato"].Value != ( Convert.ToInt32( App.TipoTreeNodeStato.Sconosciuto ) ).ToString() ) ) )
                    {
                        if ( nodeSessione.Attributes["Stato"].Value != ( Convert.ToInt32( App.TipoTreeNodeStato.SolaLettura ) ).ToString() )
                        {
                            nodeSessione.Attributes["Stato"].Value = ( Convert.ToInt32( sspve.Stato ) ).ToString();
                        }
                    }

                    removable = _x.Document.SelectSingleNode( "//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']" );
                    imported = _x.Document.ImportNode( sspve._x.Document.SelectSingleNode( "//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']" ), true );
                    removable.ParentNode.ReplaceChild( imported, removable );

                    //_x = wa._x;

                    if ( _x.Document.SelectSingleNode( "//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']" ).Attributes["Stato"] == null )
                    {
                        XmlAttribute attr = _x.Document.CreateAttribute( "Stato" );
                        _x.Document.SelectSingleNode( "//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']" ).Attributes.Append( attr );
                    }
                    _x.Document.SelectSingleNode( "//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']" ).Attributes["Stato"].Value = nodeSessione.Attributes["Stato"].Value;
                    _x.Save();

                    ReloadStatoNodiPadre();

                    SaveTreeSource();

                    e.Handled = true;
                    return;
                }

                if ( node.Attributes["ID"].Value == "100003" && IDTree == "27" )
                {
                    XmlNode pianificazioniNode = _x.Document.SelectSingleNode( "//Dato[@ID=\"100003\"]" );

                    Hashtable htSessioniP = new Hashtable();
                    string lastData = "";
                    string lastKey = "";

                    foreach ( XmlNode pianificazioneNode in pianificazioniNode.SelectNodes( "//Pianificazione" ) )
                    {
                        if ( !htSessioniP.Contains( pianificazioneNode.Attributes["ID"].Value ) )
                        {
                            htSessioniP.Add( pianificazioneNode.Attributes["ID"].Value, pianificazioneNode.Attributes["Data"].Value );
                            lastData = pianificazioneNode.Attributes["Data"].Value;
                            lastKey = pianificazioneNode.Attributes["ID"].Value;
                        }
                    }

                    MasterFile mf = MasterFile.Create();
                    Hashtable htmf = mf.GetPianificazioniVigilanza( IDSessione );

                    wSchedaSessioniPianificazioniVigilanze sspve = new wSchedaSessioniPianificazioniVigilanze();

                    sspve.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    switch ( base.WindowState )
                    {
                        case System.Windows.WindowState.Normal:
                            sspve.Width = ActualWidth * 97 / 100;
                            sspve.Height = ActualHeight * 97 / 100;
                            break;
                        case System.Windows.WindowState.Maximized:
                            sspve.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 97 / 100;
                            sspve.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 97 / 100;
                            break;
                    }

                    nodeSessione = node.SelectSingleNode( "Sessioni/Sessione[@Alias=\"" + selectedAliasCodificato + "\"]" );
                    if ( nodeSessione != null )
                    {
                        sspve.Stato = ( (App.TipoTreeNodeStato)( Convert.ToInt32( nodeSessione.Attributes["Stato"].Value ) ) );
                        sspve.OldStatoNodo = sspve.Stato;
                    }


                    XmlNode nodeNota = ( (XmlNode)( tvMain.SelectedItem ) );
                    sspve.nota = node.Attributes["Nota"].Value;

                    sspve.lastData = lastData;
                    sspve.lastKey = lastKey;
                    sspve.DataInizio = htmf["DataInizio"].ToString();
                    sspve.DataFine = htmf["DataFine"].ToString();
                    sspve.htSessioni = htSessioniP;
                    sspve.Cliente = Cliente;
                    sspve.IDCliente = IDCliente;
                    sspve._x = _x;
                    sspve.ReadOnly = ReadOnly;

                    sspve.IDTree = IDTree;
                    sspve.IDSessione = IDSessione;

                    if ( htSessioniP.Count == 0 )
                    {
                        sspve.generateTree();
                    }

                    sspve.ConfiguraMaschera();
                    sspve.ShowDialog();
                    sspve.Activate();

                    _x.Save();

                    if ( nodeSessione != null && ( sspve.Stato != App.TipoTreeNodeStato.Sconosciuto || ( sspve.Stato == App.TipoTreeNodeStato.Sconosciuto && nodeSessione.Attributes["Stato"].Value != ( Convert.ToInt32( App.TipoTreeNodeStato.Sconosciuto ) ).ToString() ) ) )
                    {
                        if ( nodeSessione.Attributes["Stato"].Value != ( Convert.ToInt32( App.TipoTreeNodeStato.SolaLettura ) ).ToString() )
                        {
                            nodeSessione.Attributes["Stato"].Value = ( Convert.ToInt32( sspve.Stato ) ).ToString();
                        }
                    }

                    removable = _x.Document.SelectSingleNode( "//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']" );
                    imported = _x.Document.ImportNode( sspve._x.Document.SelectSingleNode( "//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']" ), true );
                    removable.ParentNode.ReplaceChild( imported, removable );

                    //_x = wa._x;

                    if ( _x.Document.SelectSingleNode( "//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']" ).Attributes["Stato"] == null )
                    {
                        XmlAttribute attr = _x.Document.CreateAttribute( "Stato" );
                        _x.Document.SelectSingleNode( "//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']" ).Attributes.Append( attr );
                    }
                    _x.Document.SelectSingleNode( "//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']" ).Attributes["Stato"].Value = nodeSessione.Attributes["Stato"].Value;
                    _x.Save();

                    ReloadStatoNodiPadre();

                    SaveTreeSource();

                    e.Handled = true;
                    return;
                    //XmlNode pianificazioniNode = _x.Document.SelectSingleNode( "//Dato[@ID=\"100003\"]" );

                    //Hashtable htSessioniP = new Hashtable();
                    //string lastData = "";
                    //string lastKey = "";

                    //foreach ( XmlNode pianificazioneNode in pianificazioniNode.SelectNodes( "//Pianificazione" ) )
                    //{
                    //    if ( !htSessioniP.Contains( pianificazioneNode.Attributes["ID"].Value ) )
                    //    {
                    //        htSessioniP.Add( pianificazioneNode.Attributes["ID"].Value, pianificazioneNode.Attributes["Data"].Value );
                    //        lastData = pianificazioneNode.Attributes["Data"].Value;
                    //        lastKey = pianificazioneNode.Attributes["ID"].Value;
                    //    }
                    //}

                    //MasterFile mf = MasterFile.Create();
                    //Hashtable htmf = mf.GetPianificazioniVigilanza( IDSessione );

                    //wSchedaSessioniPianificazioniVigilanze sspvi = new wSchedaSessioniPianificazioniVigilanze();

                    //nodeSessione = node.SelectSingleNode( "Sessioni/Sessione[@Alias=\"" + selectedAliasCodificato + "\"]" );
                    //if ( nodeSessione != null )
                    //{
                    //    sspvi.Stato = ( (App.TipoTreeNodeStato)( Convert.ToInt32( nodeSessione.Attributes["Stato"].Value ) ) );
                    //    sspvi.OldStatoNodo = sspvi.Stato;
                    //}

                    //XmlNode nodeNota = ( (XmlNode)( tvMain.SelectedItem ) );
                    //sspvi.nota = node.Attributes["Nota"].Value;

                    //sspvi.lastData = lastData;
                    //sspvi.lastKey = lastKey;
                    //sspvi.DataInizio = htmf["DataInizio"].ToString();
                    //sspvi.DataFine = htmf["DataFine"].ToString();
                    //sspvi.htSessioni = htSessioniP;
                    //sspvi.Cliente = Cliente;
                    //sspvi.IDCliente = IDCliente;
                    //sspvi._x = _x;
                    //sspvi.ReadOnly = ReadOnly;

                    ////sspvi.IDTree = IDTree;
                    ////sspvi.IDSessione = IDSessione;     

                    //if ( htSessioniP.Count == 0 )
                    //{
                    //    sspvi.generateTree();
                    //}

                    //sspvi.ConfiguraMaschera();
                    //sspvi.ShowDialog();
                    //sspvi.Activate();

                    //_x.Save();

                    //if ( nodeSessione != null && ( sspvi.Stato != App.TipoTreeNodeStato.Sconosciuto || ( sspvi.Stato == App.TipoTreeNodeStato.Sconosciuto && nodeSessione.Attributes["Stato"].Value != ( Convert.ToInt32( App.TipoTreeNodeStato.Sconosciuto ) ).ToString() ) ) )
                    //{
                    //    if ( nodeSessione.Attributes["Stato"].Value != ( Convert.ToInt32( App.TipoTreeNodeStato.SolaLettura ) ).ToString() )
                    //    {
                    //        nodeSessione.Attributes["Stato"].Value = ( Convert.ToInt32( sspvi.Stato ) ).ToString();
                    //    }
                    //}

                    //removable = _x.Document.SelectSingleNode( "//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']" );
                    //imported = _x.Document.ImportNode( sspvi._x.Document.SelectSingleNode( "//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']" ), true );
                    //removable.ParentNode.ReplaceChild( imported, removable );

                    ////_x = wa._x;

                    //if ( _x.Document.SelectSingleNode( "//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']" ).Attributes["Stato"] == null )
                    //{
                    //    XmlAttribute attr = _x.Document.CreateAttribute( "Stato" );
                    //    _x.Document.SelectSingleNode( "//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']" ).Attributes.Append( attr );
                    //}
                    //_x.Document.SelectSingleNode( "//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']" ).Attributes["Stato"].Value = nodeSessione.Attributes["Stato"].Value;
                    //_x.Save();

                    //ReloadStatoNodiPadre();

                    //SaveTreeSource();


                    //e.Handled = true;
                    //return;
                }

                XmlNodeList tmpnode2 = _x.Document.SelectNodes("//Node[@xaml]");

                foreach (XmlNode item in tmpnode2)
                {
                    if (!(item.Attributes["xaml"].Value.Contains("\\XAML\\")))
                    {
                        DirectoryInfo di = new DirectoryInfo(App.AppDataDataFolder + "\\XAML");
                        if (!di.Exists)
                        {
                            di.Create();
                        }

                        string newXamlFile = "\\XAML\\" + Guid.NewGuid().ToString() + ".xaml";
                        FileInfo fxaml = new FileInfo(App.AppDataDataFolder + newXamlFile);

                        while (fxaml.Exists)
                        {
                            newXamlFile = "\\XAML\\" + Guid.NewGuid().ToString() + ".xaml";
                            fxaml = new FileInfo(App.AppDataDataFolder + newXamlFile);
                        }

                        StreamWriter sw = fxaml.CreateText();
                        sw.WriteLine(item.Attributes["xaml"].Value);
                        sw.Flush();
                        sw.Close();

                        item.Attributes["xaml"].Value = newXamlFile;
                    }
                    else
                    {
                        break;
                    }
                }

                _x.Save();

                WindowWorkArea wa = new WindowWorkArea(ref _x);

				//Nodi
				int index = -1;
				wa.NodeHome = -1;
                
                //PRISC VELOCIZZAZIONE
                //SaveTreeSource();

				if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
				{
					foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
					{
						if (item.Attributes["Tipologia"].Value == "Nodo Multiplo" || item.ChildNodes.Count == 1)
						{
							index++;

							if (item.Attributes["ID"].Value == node.Attributes["ID"].Value)
							{
								wa.NodeHome = index;
							}

                            if ( !wa.Nodes.ContainsKey( index ) )
                            {
                                wa.Nodes.Add( index, item );
                            }
						}
					}
				}

				if (wa.NodeHome == -1)
				{
					e.Handled = true;
					return;
				}
                
				wa.NodeNow = wa.NodeHome;

                wa.Owner = Window.GetWindow(this);

                //posizione e dimensioni finestra
                wa.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                switch (base.WindowState)
                {
                    case System.Windows.WindowState.Normal:
                        wa.Width = ActualWidth * 97 / 100;
                        wa.Height = ActualHeight * 97 / 100;
                        break;
                    case System.Windows.WindowState.Maximized:
						wa.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 97 / 100;
                        wa.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 97 / 100;
                        break;
                }

				//Sessioni
				wa.Sessioni = htSessioni;
				wa.SessioniTitoli = htSessioniAlias;
				wa.SessioniID = htSessioniID;

				foreach (DictionaryEntry item in htSessioni)
				{
					if (item.Value.ToString() == _x.File)
					{
						wa.SessioneHome = Convert.ToInt32(item.Key.ToString());
						wa.SessioneNow = wa.SessioneHome;
						break;
					}
				}

				//Variabili
				wa.ReadOnly = ReadOnly;
				wa.ReadOnlyOLD = ReadOnly;
                wa.ApertoInSolaLettura = ApertoInSolaLettura;
				
				nodeSessione = node.SelectSingleNode("Sessioni/Sessione[@Alias=\"" + selectedAliasCodificato + "\"]");
				if (nodeSessione != null)
				{
					wa.Stato = ((App.TipoTreeNodeStato)(Convert.ToInt32(nodeSessione.Attributes["Stato"].Value)));
                    if(wa.Stato == App.TipoTreeNodeStato.Scrittura)
                    {
                        wa.Stato = App.TipoTreeNodeStato.DaCompletare;
                        nodeSessione.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString();
                    }
					wa.OldStatoNodo = wa.Stato;
				}				

                //passaggio dati
				wa.IDTree = IDTree;
				wa.IDSessione = IDSessione;
				wa.IDCliente = IDCliente;
                
                //apertura
                wa.Load();

                wa.ShowDialog();

                //RevisoftApplication.XmlManager x = new XmlManager();
                //x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                //TreeXmlProvider.Document = x.LoadEncodedFile(SelectedTreeSource);

                //TreeXmlProvider.Refresh();

                //node = TreeXmlProvider.Document.SelectSingleNode( "//Node[@ID=" + node.Attributes["ID"].Value + "]" );
                //nodeSessione = node.SelectSingleNode( "Sessioni/Sessione[@Alias=\"" + selectedAliasCodificato + "\"]" );
                   
                //XmlNode paretntnodehere = node.ParentNode;
                //while ( paretntnodehere != null && paretntnodehere.Attributes["Expanded"] != null )
                //{
                //    paretntnodehere.Attributes["Expanded"].Value = "True";
                //    paretntnodehere = paretntnodehere.ParentNode;
                //}

                ////if(nodeSessione != null)
                ////{
                ////    if ( nodeSessione.Attributes["Sospesi"] == null)
                ////    {
                ////        XmlAttribute attr = nodeSessione.OwnerDocument.CreateAttribute( "Sospesi" );
                ////        nodeSessione.Attributes.Append( attr );
                ////    }

                ////    nodeSessione.Attributes["Sospesi"].Value = wa.sospesi;
                ////}

				if (nodeSessione != null && (wa.Stato != App.TipoTreeNodeStato.Sconosciuto || (wa.Stato == App.TipoTreeNodeStato.Sconosciuto && nodeSessione.Attributes["Stato"].Value != (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString())))
				{
					if (nodeSessione.Attributes["Stato"].Value != (Convert.ToInt32(App.TipoTreeNodeStato.SolaLettura)).ToString())
					{
						nodeSessione.Attributes["Stato"].Value = (Convert.ToInt32(wa.Stato)).ToString();
					}
				}

                string IDNodeList = node.Attributes["ID"].Value;

                foreach (XmlNode child in TreeXmlProvider.Document.SelectSingleNode("//Tree//Node[@ID='" + node.Attributes["ID"].Value + "']").ChildNodes)
                {
                    if (child.Attributes["ID"] != null)
                    {
                        IDNodeList += "|" + child.Attributes["ID"].Value;
                    }
                }

                foreach (string nodeID in IDNodeList.Split('|'))
                {
                    removable = _x.Document.SelectSingleNode("//Dati//Dato[@ID='" + nodeID + "']");
                    imported = _x.Document.ImportNode(wa._x.Document.SelectSingleNode("//Dati//Dato[@ID='" + nodeID + "']"), true);
                    removable.ParentNode.ReplaceChild(imported, removable);
                }               

				//_x = wa._x;

				if (_x.Document.SelectSingleNode("//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']").Attributes["Stato"] == null)
				{
				    XmlAttribute attr = _x.Document.CreateAttribute("Stato");
					_x.Document.SelectSingleNode("//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']").Attributes.Append(attr);
				}
				_x.Document.SelectSingleNode("//Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']").Attributes["Stato"].Value = nodeSessione.Attributes["Stato"].Value;
				_x.Save();
                
				SaveTreeSource();
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}

			e.Handled = true;
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

		private void btn_NonApplicabile_Click(object sender, RoutedEventArgs e)
		{
			if (ReadOnly)
			{
				MessageBox.Show("Sessione in sola lettura", "Attenzione");
				return;
			}

			XmlNode node = ((XmlNode)(tvMain.SelectedItem));

            if ( node.Attributes["Report"].Value == "True" )
            {
                return;
            }

			XmlNode SelectedNode = node.SelectSingleNode("Sessioni/Sessione[@Selected='#AA82BDE4']");

			if (SelectedNode.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString() || SelectedNode.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString())
			{
                MessageBox.Show( "Questa Carta di Lavoro ha già uno stato assegnato, non è possibile renderlo Non Applicabile.", "Attenzione" );
				return;
			}

			if (SelectedNode.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile)).ToString())
			{
				SelectedNode.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString();
 			}
			else
			{
                SelectedNode.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile)).ToString();
			}

			XmlNode nodetmp = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");// ((XmlNode)(htStati[node.Attributes["ID"].Value]));

			try 
			{
				nodetmp.Attributes["Stato"].Value = SelectedNode.Attributes["Stato"].Value;
			}
			catch (Exception ex)
			{
				string log = ex.Message;
				XmlAttribute attr = nodetmp.OwnerDocument.CreateAttribute("Stato");
				attr.Value = SelectedNode.Attributes["Stato"].Value;
				nodetmp.Attributes.Append(attr);
			}

			_x.Save();

			ReloadStatoNodiPadre();
		}

		private void buttonChiudi_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

        private void buttonApriFormulario_Click(object sender, RoutedEventArgs e)
        {
            Formulario formulario = new Formulario();
            formulario.Owner = this;
            formulario.LoadTreeSource();
            formulario.ShowDialog();
        }

		private void buttonApriDocumenti_Click(object sender, RoutedEventArgs e)
		{
			wDocumenti documenti = new wDocumenti();

			documenti.ReadOnly = ReadOnly;
			documenti.Titolo = "Indice Documenti per Cliente";
			documenti.Tipologia = TipoVisualizzazione.Documenti;
			documenti.Tree = IDTree;
			documenti.Cliente = IDCliente;
			documenti.Sessione = "-1"; //IDSessione;
			documenti.Owner = this;

            if ( System.Windows.SystemParameters.PrimaryScreenWidth < 1100 || System.Windows.SystemParameters.PrimaryScreenHeight < 600 )
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

		private void btn_ScambioDati_Click(object sender, RoutedEventArgs e)
		{
			WindowWorkAreaTree_ScambioDati wWorkAreaSD = new WindowWorkAreaTree_ScambioDati();

			wWorkAreaSD.Owner = this;
			wWorkAreaSD.SelectedTreeSource =  SelectedTreeSource;
			wWorkAreaSD.SelectedDataSource = SelectedDataSource;
			wWorkAreaSD.Cliente =  _cliente;

			wWorkAreaSD.IDTree = IDTree;
			wWorkAreaSD.IDCliente = IDCliente;
			wWorkAreaSD.IDSessione = IDSessione;

            //andrea
            wWorkAreaSD.TitoloSessione = selectedAlias; 
			wWorkAreaSD.Tipo = App.TipoScambioDati.Esporta;
            wWorkAreaSD.TipoAttivita = _TipoAttivita;
            
            //carico dati
			wWorkAreaSD.LoadTreeSource();
			wWorkAreaSD.ShowDialog();

            //this.LoadTreeSource();
		}

        private void menuInfoGuida_Click(object sender, RoutedEventArgs e)
        {
            //file
            //System.Diagnostics.Process.Start(App.AppHelpFile);
            //web
            System.Diagnostics.Process.Start(RevisoftApplication.Properties.Settings.Default["RevisoftApplicationGuide"].ToString());
        }


        

        private void menuCampionamento_Click(object sender, RoutedEventArgs e)
        {
            XmlNode nodeTreePadre = TreeXmlProvider.Document.SelectSingleNode("/Tree");
            XmlNode nodeTreeSessione = nodeTreePadre.SelectSingleNode("Sessioni/Sessione[@Selected='#AA82BDE4']");
            wCampionamento wcnn = new wCampionamento(nodeTreeSessione, IDCliente, Cliente, Esercizio, IDSessione, IDTree);
            wcnn.ShowDialog();

            RevisoftApplication.XmlManager xx = new XmlManager();
            xx.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
            TreeXmlProvider.Document = xx.LoadEncodedFile(SelectedTreeSource);

            TreeXmlProvider.Refresh();

            XmlNode nodehere = TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=" + wcnn.changedID + "]");
            XmlNode nodeSessionehere = nodehere.SelectSingleNode("Sessioni/Sessione[@Alias=\"" + selectedAliasCodificato + "\"]");

            XmlNode paretntnodeherehere = nodehere.ParentNode;
            while (paretntnodeherehere != null && paretntnodeherehere.Attributes["Expanded"] != null)
            {
                paretntnodeherehere.Attributes["Expanded"].Value = "True";
                paretntnodeherehere = paretntnodeherehere.ParentNode;
            }

            if (nodeSessionehere != null && nodeSessionehere.Attributes["Stato"].Value != (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString())
            {
                nodeSessionehere.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString();
            }

            if (_x.Document.SelectSingleNode("//Dati//Dato[@ID='" + wcnn.changedID + "']").Attributes["Stato"] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("Stato");
                _x.Document.SelectSingleNode("//Dati//Dato[@ID='" + wcnn.changedID + "']").Attributes.Append(attr);
            }
            _x.Document.SelectSingleNode("//Dati//Dato[@ID='" + wcnn.changedID + "']").Attributes["Stato"].Value = nodeSessionehere.Attributes["Stato"].Value;
            _x.Save();

            ReloadStatoNodiPadre();

            SaveTreeSource();

            e.Handled = true;

            return;
        }


        private void btn_GuidaRevisoft_Click(object sender, RoutedEventArgs e)
        {
			XmlNode node;
			string nota = "";

			try
			{
				node = ((XmlNode)(tvMain.SelectedItem));
				nota = node.Attributes["Nota"].Value;
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}

			
			wGuidaRevisoft w = new wGuidaRevisoft();
			w.Owner = Window.GetWindow(this);

            w.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
			//w.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;

            if ( System.Windows.SystemParameters.PrimaryScreenWidth < 1100 || System.Windows.SystemParameters.PrimaryScreenHeight < 600 )
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

            //Point p = Mouse.GetPosition(this);
            //switch (base.WindowState)
            //{
            //    case System.Windows.WindowState.Normal:
            //        w.Top = this.Top + p.Y;
            //        w.Left = this.Left + p.X;
            //        break;
            //    case System.Windows.WindowState.Maximized:
            //        w.Top = p.Y;
            //        w.Left = p.X;
            //        break;
            //}

            if ( nota != "" && nota != "<P align=left>&nbsp;</P>" && nota != "<P class=MsoNormal style=\"MARGIN: 0cm 0cm 0pt; LINE-HEIGHT: normal\" align=left>&nbsp;</P>" )
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

        bool StampaLetteraAttestazione = false;

    public void btn_StampaLetteraAttestazione_Click( object sender, RoutedEventArgs e )
    {
      if (TreeXmlProvider.Document.SelectSingleNode( "//Node[@ID=\"261\"]" ) == null)
      {
        MessageBox.Show("documento non disponibile");
        return;
      }

      ProgressWindow pw = new ProgressWindow();

      MasterFile mf = MasterFile.Create();
      Hashtable cliente = mf.GetAnagrafica( Convert.ToInt32( IDCliente ) );
      Hashtable dati = new Hashtable();
      //WordLib wl = new WordLib();
      RTFLib wl = new RTFLib();
      string Intestazione = "";

      dati = mf.GetConclusione(IDSessione);
      wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
      wl.StampaLetteraAttestazione = true;
      wl.StampaTemporanea = StampaTemporanea;
      wl.htCliente = cliente;

      wl.Open(
        dati,
        cliente["RagioneSociale"].ToString(),
        cliente["CodiceFiscale"].ToString(),
        selectedAliasCodificato,
        TreeXmlProvider.Document.SelectSingleNode( "/Tree" ).ChildNodes[0].Attributes["Titolo"].Value,
        false,true);

      StampaLetteraAttestazione = true;
      RecursiveNode(TreeXmlProvider.Document.SelectSingleNode( "//Node[@ID=\"261\"]" ),wl,SelectedDataSource);
      Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " "
        + cliente["CodiceFiscale"].ToString() + " Esercizio: "
        + ConvertDate( dati["Data"].ToString() );
      if (pw != null) { pw.Close(); pw = null; }
      wl.Save(Intestazione);
      StampaTemporanea = false;
      wl.Close();
    }


        bool StampaManagementLetter = false;

    public void btn_StampaManagementLetter_Click(object sender, RoutedEventArgs e)
    {
      if (TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=\"281\"]") == null)
      {
        MessageBox.Show("documento non disponibile", "operazione impossibile", MessageBoxButton.OK);
        return;
      }

      ProgressWindow pw = new ProgressWindow();

      MasterFile mf = MasterFile.Create();
      Hashtable cliente = mf.GetAnagrafica(Convert.ToInt32(IDCliente));
      Hashtable dati = new Hashtable();
      //WordLib wl = new WordLib();
      RTFLib wl = new RTFLib();
      string Intestazione = "";

      dati = mf.GetConclusione(IDSessione);
      wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
      wl.StampaManagementLetter = true;
      wl.StampaTemporanea = StampaTemporanea;
      wl.htCliente = cliente;
      wl.Open(
        dati,
        cliente["RagioneSociale"].ToString(),
        cliente["CodiceFiscale"].ToString(),
        selectedAliasCodificato,
        TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value,
        false,true);

      StampaManagementLetter = true;
      RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=\"281\"]").ParentNode,wl,SelectedDataSource);
      Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " "
        + cliente["CodiceFiscale"].ToString() + " Esercizio: "
        + ConvertDate(dati["Data"].ToString());
      if (pw != null) { pw.Close();pw = null; }
      wl.Save(Intestazione);
      StampaTemporanea = false;
      wl.Close();
    }

        bool StampaLetteraIncarico = false;

        public void btn_StampaLetteraIncarico_Click( object sender, RoutedEventArgs e )
        {
            if(TreeXmlProvider.Document.SelectSingleNode( "//Node[@ID=\"142\"]" ) == null)
            {
                return;
            }

            //Process wait - START
            //ProgressWindow pw = new ProgressWindow();

            MasterFile mf = MasterFile.Create();
            Hashtable cliente = mf.GetAnagrafica( Convert.ToInt32( IDCliente ) );
            Hashtable dati = new Hashtable();

            //WordLib wl = new WordLib();
            RTFLib wl = new RTFLib();
            string Intestazione = "";

            dati = mf.GetIncarico( IDSessione );
            wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
            wl.StampaLetteraIncarico = true;
            wl.StampaTemporanea = StampaTemporanea;

            wl.htCliente = cliente;

            wl.Open(
              dati, cliente["RagioneSociale"].ToString(),
              cliente["CodiceFiscale"].ToString(), selectedAliasCodificato,
              TreeXmlProvider.Document.SelectSingleNode( "/Tree" ).ChildNodes[0].Attributes["Titolo"].Value, false, true );

            StampaLetteraIncarico = true;
            RecursiveNode( TreeXmlProvider.Document.SelectSingleNode( "//Node[@ID=\"142\"]" ), wl, SelectedDataSource );

            Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " "
              + cliente["CodiceFiscale"].ToString() + " Esercizio: "
              + ConvertDate( dati["DataNomina"].ToString() );
            //if (pw != null) { pw.Close(); pw = null; }
            wl.Save( Intestazione );
            StampaTemporanea = false;
            wl.Close();

            //Process wait - STOP
            //pw.Close();
        }

        public void btn_StampaLetteraIncaricoCollegio_Click(object sender, RoutedEventArgs e)
        {
            if (TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=\"2016142\"]") == null)
            {
                return;
            }

            //Process wait - START
            //ProgressWindow pw = new ProgressWindow();

            MasterFile mf = MasterFile.Create();
            Hashtable cliente = mf.GetAnagrafica(Convert.ToInt32(IDCliente));
            Hashtable dati = new Hashtable();

            //WordLib wl = new WordLib();
            RTFLib wl = new RTFLib();
            string Intestazione = "";

            dati = mf.GetIncarico(IDSessione);
            wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
            wl.StampaLetteraIncarico = true;
            wl.StampaTemporanea = StampaTemporanea;

            wl.htCliente = cliente;

            wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true);

            StampaLetteraIncarico = true;
            RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=\"2016142\"]"), wl, SelectedDataSource);

            Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["DataNomina"].ToString());
            //if (pw != null) { pw.Close();pw = null; }
            wl.Save(Intestazione);
            StampaTemporanea = false;
            wl.Close();

            //Process wait - STOP
            //pw.Close();
        }

        bool StampaCodiceEtico = false;

        public void btn_StampaCodiceEtico_Click(object sender, RoutedEventArgs e)
        {
            if (TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=\"142\"]") == null)
            {
                return;
            }

            //Process wait - START
            //ProgressWindow pw = new ProgressWindow();

            MasterFile mf = MasterFile.Create();
            Hashtable cliente = mf.GetAnagrafica(Convert.ToInt32(IDCliente));
            Hashtable dati = new Hashtable();

            //WordLib wl = new WordLib();
            RTFLib wl = new RTFLib();
            string Intestazione = "";

            dati = mf.GetISQC(IDSessione);
            wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
            wl.StampaCodiceEtico = true;
            wl.StampaTemporanea = StampaTemporanea;

            wl.htCliente = cliente;

            wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true);

            StampaCodiceEtico = true;
            RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("//Node[@ID=\"142\"]"), wl, SelectedDataSource);

            Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["DataNomina"].ToString());
            //if (pw != null) { pw.Close();pw = null; }
            wl.Save(Intestazione);
            StampaTemporanea = false;
            wl.Close();

            //Process wait - STOP
            //pw.Close();
        }

        string IDB_Padre = "227";
        string IDBA_Padre = "229";
        string tipologiaBilancio = "";
        string tipoBilancio = "";
        
    public void btn_StampaReport_Click(object sender, RoutedEventArgs e)
		{
      Hide();
      ProgressWindow pw = null;

      wSceltaStampa st = new wSceltaStampa();
      switch ((App.TipoFile)(System.Convert.ToInt32(IDTree)))
      {
        case App.TipoFile.Incarico:
            st.StampePossibili.Add("Stampa Fascicolo");
            st.StampePossibili.Add("Stampa Lettera di Incarico Collegio");
            st.StampePossibili.Add("Stampa Lettera di Incarico Soggetto Unico");
            break;
        case App.TipoFile.ISQC:
            st.StampePossibili.Add("Stampa Fascicolo");
            st.StampePossibili.Add("Stampa Codice Etico");
            break;
        case App.TipoFile.Conclusione:
            st.StampePossibili.Add("Stampa Fascicolo");
            st.StampePossibili.Add("Stampa Lettera di Attestazione");
            st.StampePossibili.Add("Stampa Management Letter");
            break;
        case App.TipoFile.Revisione:
        case App.TipoFile.Bilancio:
            st.StampePossibili.Add("Stampa Fascicolo");
            break;
        case App.TipoFile.Verifica:
            st.StampePossibili.Add("Stampa Anteprima");
            st.StampePossibili.Add("Stampa Carte di Lavoro Vuote");
            break;
        case App.TipoFile.Vigilanza:
            st.StampePossibili.Add("Stampa Anteprima");
            TextBlock_Btn_Stampa.Text = "Anteprima";
            break;
        case App.TipoFile.RelazioneB:
        case App.TipoFile.RelazioneV:
        case App.TipoFile.RelazioneBC:
        case App.TipoFile.RelazioneVC:
        case App.TipoFile.RelazioneBV:
            st.StampePossibili.Add("Stampa Relazione");
            break;
        case App.TipoFile.PianificazioniVerifica:
        case App.TipoFile.PianificazioniVigilanza:
            st.StampePossibili.Add("Stampa Pianificazione");
            break;
        default:
            break;
      }

      st.Load();
      if (st.reallychosen == false)
      {
        st.ShowDialog();

        if (st.reallychosen == false)
        {
          Show();
          Activate();
          return;
        }
      }
               
      foreach (RadioButton item in st.collectorRadiobutton.Children)
      {
        if (item.IsChecked == true)
        {                    
          MasterFile mf = MasterFile.Create();
          Hashtable cliente = mf.GetAnagrafica(Convert.ToInt32(IDCliente));
          Hashtable dati = new Hashtable();

          //WordLib wl = new WordLib();
          RTFLib wl = new RTFLib();
          string Intestazione = "";

          switch ((App.TipoFile)(System.Convert.ToInt32(IDTree)))
          {
            case App.TipoFile.Revisione:
              if (item.Content.ToString() == "Stampa Fascicolo")
              {
                pw = new ProgressWindow();
                dati = mf.GetRevisione(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampa;
                wl.Fascicolo = true;
                wl.Open(dati, cliente["RagioneSociale"].ToString(),
                  cliente["CodiceFiscale"].ToString(),
                  selectedAliasCodificato,
                  TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value,
                  false,true);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"),wl,SelectedDataSource);
                Intestazione = "Società: " + cliente["RagioneSociale"].ToString()
                  + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: "
                  + ConvertDate(dati["Data"].ToString());
                if (pw!=null) { pw.Close();pw = null; }
                wl.SavePDF(Intestazione, this);
              }
              break;

            case App.TipoFile.RelazioneB:
              string FileBilancioB = mf.GetBilancioAssociatoFromRelazioneBFile(SelectedDataSource);
              if (FileBilancioB != "" && (new FileInfo(FileBilancioB)).Exists)
              {
                XmlDataProviderManager _xBV = new XmlDataProviderManager(FileBilancioB);

                if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']") != null
                  && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"] != null)
                {
                  tipologiaBilancio = "Ordinario";
                  tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"].Value;
                }
                else
                {
                  if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']") != null
                    && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"] != null)
                  {
                    tipologiaBilancio = "Abbreviato";
                    tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"].Value;
                  }
                }
              }
              if (item.Content.ToString() == "Stampa Relazione")
              {
                if (StampaTemporanea == false
                  && RecursiveCheckDaCompletarePresenti(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node")) == true)
                {
                  if (MessageBox.Show("Risultano paragrafi non completati o non esaminati. "
                    +"Procedo ugualmente con la stampa?", "Attenzione",
                    MessageBoxButton.YesNo) == MessageBoxResult.No)
                  {
                    Show();Activate();return;
                  }
                }
                dati = mf.GetRelazioneB(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                wl.StampaRelazioneGenerica = true;
                wl.StampaRelazioneBilancio = true;
                wl.StampaTemporanea = StampaTemporanea;
                wl.tipologiaBilancio = tipologiaBilancio;
                wl.tipoBilancio = tipoBilancio;
                wl.htCliente = cliente;
                pw = new ProgressWindow();
                wl.Open(dati,
                  cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(),
                  selectedAliasCodificato,
                  TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value,
                  false, true);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"),wl,SelectedDataSource);
                Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " "
                  + cliente["CodiceFiscale"].ToString() + " Esercizio: "
                  + ConvertDate(dati["Data"].ToString());
                if (pw != null) { pw.Close();pw = null; }
                wl.Save(Intestazione);
                StampaTemporanea = false;
              }
              break;

                  case App.TipoFile.RelazioneV:
                      //Process wait - START
                      //pw = new ProgressWindow();


                      string FileBilancioV = mf.GetBilancioAssociatoFromRelazioneVFile(SelectedDataSource);

                      if (FileBilancioV != "" && (new FileInfo(FileBilancioV)).Exists)
                      {
                          XmlDataProviderManager _xBV = new XmlDataProviderManager(FileBilancioV);

                          if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']") != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"] != null)
                          {
                              tipologiaBilancio = "Ordinario";
                              tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"].Value;
                          }
                          else
                          {
                              if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']") != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"] != null)
                              {
                                  tipologiaBilancio = "Abbreviato";
                                  tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"].Value;
                              }
                          }
                      }

                      if (item.Content.ToString() == "Stampa Relazione")
                      {
                          if (StampaTemporanea == false && RecursiveCheckDaCompletarePresenti(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node")) == true)
                          {
                              if (MessageBox.Show("Risultano paragrafi non completati o non esaminati. Procedo ugualmente con la stampa?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
                              {
                                //pw.Close();
                                Show();
                                Activate();
                                return;
                              }
                          }

                          dati = mf.GetRelazioneV(IDSessione);
                          wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                          wl.StampaRelazioneGenerica = true;
                          wl.StampaRelazioneVigilanza = true;
                          wl.StampaTemporanea = StampaTemporanea;
                          wl.tipologiaBilancio = tipologiaBilancio;
                          wl.tipoBilancio = tipoBilancio;

                          wl.htCliente = cliente;

                          wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true);
                          RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);

                          Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["Data"].ToString());
                          wl.Save(Intestazione);
                          StampaTemporanea = false;
                      }
                      break;

                  case App.TipoFile.RelazioneBC:
                      //Process wait - START
                      //pw = new ProgressWindow();


                      string FileBilancioBC = mf.GetBilancioAssociatoFromRelazioneBCFile(SelectedDataSource);

                      if (FileBilancioBC != "" && (new FileInfo(FileBilancioBC)).Exists)
                      {
                          XmlDataProviderManager _xBV = new XmlDataProviderManager(FileBilancioBC);

                          if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']") != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"] != null)
                          {
                              tipologiaBilancio = "Ordinario";
                              tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"].Value;
                          }
                          else
                          {
                              if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']") != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"] != null)
                              {
                                  tipologiaBilancio = "Abbreviato";
                                  tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"].Value;
                              }
                          }
                      }

                      if (item.Content.ToString() == "Stampa Relazione")
                      {
                          if (StampaTemporanea == false && RecursiveCheckDaCompletarePresenti(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node")) == true)
                          {
                              if (MessageBox.Show("Risultano paragrafi non completati o non esaminati. Procedo ugualmente con la stampa?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
                              {
                                  //pw.Close();
                                  Show();
                                  Activate();
                                  return;
                              }
                          }

                          dati = mf.GetRelazioneBC(IDSessione);
                          wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                          wl.StampaRelazioneGenerica = true;
                          wl.StampaRelazioneBilancio = true;
                          wl.StampaTemporanea = StampaTemporanea;
                          wl.tipologiaBilancio = tipologiaBilancio;
                          wl.tipoBilancio = tipoBilancio;

                          wl.htCliente = cliente;

                          wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true);
                          RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);

                          Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["Data"].ToString());
                          wl.Save(Intestazione);
                          StampaTemporanea = false;
                      }
                      break;

                  case App.TipoFile.RelazioneVC:
                      //Process wait - START
                      //pw = new ProgressWindow();


                      string FileBilancioVC = mf.GetBilancioAssociatoFromRelazioneVCFile(SelectedDataSource);

                      if (FileBilancioVC != "" && (new FileInfo(FileBilancioVC)).Exists)
                      {
                          XmlDataProviderManager _xBV = new XmlDataProviderManager(FileBilancioVC);

                          if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']") != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"] != null)
                          {
                              tipologiaBilancio = "Ordinario";
                              tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"].Value;
                          }
                          else
                          {
                              if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']") != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"] != null)
                              {
                                  tipologiaBilancio = "Abbreviato";
                                  tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"].Value;
                              }
                          }
                      }

                      if (item.Content.ToString() == "Stampa Relazione")
                      {
                          if (StampaTemporanea == false && RecursiveCheckDaCompletarePresenti(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node")) == true)
                          {
                              if (MessageBox.Show("Risultano paragrafi non completati o non esaminati. Procedo ugualmente con la stampa?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
                              {
                                //pw.Close();
                                Show();
                                Activate();
                                return;
                              }
                          }

                          dati = mf.GetRelazioneVC(IDSessione);
                          wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                          wl.StampaRelazioneGenerica = true;
                          wl.StampaRelazioneVigilanza = true;
                          wl.StampaTemporanea = StampaTemporanea;
                          wl.tipologiaBilancio = tipologiaBilancio;
                          wl.tipoBilancio = tipoBilancio;

                          wl.htCliente = cliente;

                          wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true);
                          RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);

                          Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["Data"].ToString());
                          wl.Save(Intestazione);
                          StampaTemporanea = false;
                      }
                      break;

                  case App.TipoFile.RelazioneBV:
                      //Process wait - START
                      //pw = new ProgressWindow();


                      string FileBilancioBV = mf.GetBilancioAssociatoFromRelazioneBVFile(SelectedDataSource);

                      if (FileBilancioBV != "" && (new FileInfo(FileBilancioBV)).Exists)
                      {
                          XmlDataProviderManager _xBV = new XmlDataProviderManager(FileBilancioBV);
                                
                          if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']") != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"] != null)
                          {
                              tipologiaBilancio = "Ordinario";
                              tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDB_Padre + "']").Attributes["tipoBilancio"].Value;
                          }
                          else
                          {                                    
                              if (_xBV != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']") != null && _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"] != null)
                              {
                                  tipologiaBilancio = "Abbreviato";
                                  tipoBilancio = _xBV.Document.SelectSingleNode("//Dato[@ID='" + IDBA_Padre + "']").Attributes["tipoBilancio"].Value;
                              }
                          }                                
                      }

                      if (item.Content.ToString() == "Stampa Relazione")
                      {
                          if (StampaTemporanea == false && RecursiveCheckDaCompletarePresenti(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node")) == true)
                          {
                              if (MessageBox.Show("Risultano paragrafi non completati o non esaminati. Procedo ugualmente con la stampa?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
                              {
                                  //pw.Close();
                                  Show();
                                  Activate();
                                  return;
                              }
                          }

                          XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='3']");
                          if (NodoDato != null && (NodoDato.Attributes["Stato"] == null || NodoDato.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString()))
                          {
                              if (NodoDato.Attributes["Stato"] == null)
                              {
                                  XmlAttribute attr2 = NodoDato.OwnerDocument.CreateAttribute("Stato");
                                  attr2.Value = "-1";
                                  NodoDato.Attributes.Append(attr2);
                              }

                              NodoDato.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString();
                          }

                          NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='4']");
                          if (NodoDato != null && (NodoDato.Attributes["Stato"] == null || NodoDato.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString()))
                          {
                              if (NodoDato.Attributes["Stato"] == null)
                              {
                                  XmlAttribute attr2 = NodoDato.OwnerDocument.CreateAttribute("Stato");
                                  attr2.Value = "-1";
                                  NodoDato.Attributes.Append(attr2);
                              }

                              NodoDato.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString();
                          }

                          _x.Save();

                          dati = mf.GetRelazioneBV(IDSessione);
                          wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                          wl.StampaRelazioneGenerica = true;
                          wl.StampaRelazioneBilancioeVigilanza = true;
                          wl.StampaTemporanea = StampaTemporanea;
                          wl.tipologiaBilancio = tipologiaBilancio;
                          wl.tipoBilancio = tipoBilancio;

                          wl.htCliente = cliente;

                          wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true);
                          RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);

                          Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["Data"].ToString());
                          wl.Save(Intestazione);
                          StampaTemporanea = false;
                      }
                      break;

                  case App.TipoFile.PianificazioniVerifica:
                      //Process wait - START
                      //pw = new ProgressWindow();

                      if (item.Content.ToString() == "Stampa Pianificazione")
                      {
                          dati = mf.GetPianificazioniVerifica(IDSessione);
                          wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                          wl.Watermark = false;
                          wl.TitoloVerbale = false;
                          wl.TitoloPianificazione = true;
                          wl.StampaTemporanea = true;

                          wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true);

                          RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);

                          wl.Save("");
                      }
                      break;

                  case App.TipoFile.Verifica:
                      //Process wait - START
                      //pw = new ProgressWindow();

                      if (item.Content.ToString() == "Stampa Anteprima")
                      {
                          dati = mf.GetVerifica(IDSessione);
                          wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                          wl.Watermark = false;

                          wl.TabelleSenzaRigheVuote = true;
                          wl.SenzaStampareTitoli = true;
                          wl.StampaTemporanea = true;

                          wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true);

                          RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);

                          istobecompleteforprinting = false;

                          if (RecursiveCheckComplete(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node")))
                          {

                              wl.AddTitleDaCompletare();

                              RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                          }

                          istobecompleteforprinting = true;
                          //ArrayList alVigilanze = mf.GetVigilanze( IDCliente );

                          //foreach ( Hashtable datiVigilanza in alVigilanze )
                          //{
                          //    if ( datiVigilanza["Data"].ToString() == dati["Data"].ToString() )
                          //    {
                          //        RevisoftApplication.XmlManager x = new XmlManager();
                          //        x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                          //        XmlDataProvider TreeXmlProviderVigilanza = new XmlDataProvider();
                          //        string SelectedTreeSourceVigilanza = App.AppDataDataFolder + "\\" + datiVigilanza["File"].ToString();
                          //        string SelectedDataSourceVigilanza = App.AppDataDataFolder + "\\" + datiVigilanza["FileData"].ToString();
                          //        TreeXmlProviderVigilanza.Document = x.LoadEncodedFile( SelectedTreeSourceVigilanza );
                          //        if ( TreeXmlProviderVigilanza.Document.SelectSingleNode( "/Tree/Node" ) != null )
                          //        {
                          //            RecursiveNode( TreeXmlProviderVigilanza.Document.SelectSingleNode( "/Tree/Node" ), wl, SelectedDataSourceVigilanza );
                          //        }
                          //    }
                          //}

                          //wl.LastParagraph(dati);
                          wl.Save("");
                      }
                      else if (item.Content.ToString() == "Stampa Carte di Lavoro Vuote")
                      {
                          //Process wait - START
                          //pw = new ProgressWindow();

                          printall = true;

                          printall_excludednodes.Clear();
                          printall_excludednodes.Add("4.2.16");
                          printall_excludednodes.Add("4.2.17");
                          printall_excludednodes.Add("4.2.21");
                          printall_excludednodes.Add("4.3.4");
                          printall_excludednodes.Add("4.5.11");
                          printall_excludednodes.Add("4.7.1");
                          printall_excludednodes.Add("4.7.2");
                          printall_excludednodes.Add("4.9.1");
                          printall_excludednodes.Add("4.9.2");
                          printall_excludednodes.Add("4.9.3");
                          printall_excludednodes.Add("4.9.4");
                          printall_excludednodes.Add("4.9.5");
                          printall_excludednodes.Add("4.10.1");
                          printall_excludednodes.Add("4.10.8");
                          printall_excludednodes.Add("4.11.1");
                          printall_excludednodes.Add("4.11.2");
                          printall_excludednodes.Add("4.12.1");
                          printall_excludednodes.Add("4.12.2");
                          printall_excludednodes.Add("4.13.1");
                          printall_excludednodes.Add("4.14.1");
                          printall_excludednodes.Add("4.14.6");
                          printall_excludednodes.Add("4.15");
                          printall_excludednodes.Add("4.31.1");
                          printall_excludednodes.Add("4.31.2");
                          printall_excludednodes.Add("4.97");
                          printall_excludednodes.Add("4.98");
                          printall_excludednodes.Add("4.99");

                          printall_nodesnow = new List<string>();

                          RecursiveNodeOnlyCodes(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"));
                                
                          wSceltaNodiStampaVuota snlv = new wSceltaNodiStampaVuota(printall_nodesnow);

                          snlv.ShowDialog();

                          if (snlv.isok == false)
                          {
                              Show();
                              Activate();
                              return;
                          }

                          printall_excludednodes.AddRange(snlv.listahere);
                                
                          dati = mf.GetVerifica(IDSessione);
                          wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                          wl.Watermark = false;

                          wl.printall = true;

                          wl.TabelleSenzaRigheVuote = true;
                          wl.SenzaStampareTitoli = true;
                          //wl.StampaTemporanea = true;
                          wl.StampaTemporanea = false;

                          wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true);

                          RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                                
                          wl.Save("");

                          printall = false;
                      }
                      break;
                  case App.TipoFile.PianificazioniVigilanza:
                      //Process wait - START
                      //pw = new ProgressWindow();

                      if (item.Content.ToString() == "Stampa Pianificazione")
                      {
                          dati = mf.GetPianificazioniVigilanza(IDSessione);
                          wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                          wl.Watermark = false;
                          wl.TitoloVerbale = false;
                          wl.TitoloPianificazione = true;
                          wl.StampaTemporanea = true;

                          wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true);

                          RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);

                          wl.Save("");
                      }
                      break;
                  case App.TipoFile.Vigilanza:
                      //Process wait - START
                      //pw = new ProgressWindow();

                      if (item.Content.ToString() == "Stampa Anteprima")
                      {
                          dati = mf.GetVigilanza(IDSessione);
                          wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
                          wl.Watermark = false;

                          wl.TabelleSenzaRigheVuote = true;
                          wl.SenzaStampareTitoli = true;
                          wl.StampaTemporanea = true;

                          wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true);

                          RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);

                          istobecompleteforprinting = false;

                          if (RecursiveCheckComplete(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node")))
                          {
                              wl.AddTitleDaCompletare();

                              RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                          }

                          istobecompleteforprinting = true;

                          //ArrayList alVerifiche = mf.GetVerifiche( IDCliente );

                          //foreach ( Hashtable datiVerifica in alVerifiche )
                          //{
                          //    if ( datiVerifica["Data"].ToString() == dati["Data"].ToString() )
                          //    {
                          //        RevisoftApplication.XmlManager x = new XmlManager();
                          //        x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                          //        XmlDataProvider TreeXmlProviderVerifica = new XmlDataProvider();
                          //        string SelectedTreeSourceVerifica = App.AppDataDataFolder + "\\" + datiVerifica["File"].ToString();
                          //        string SelectedDataSourceVerifica = App.AppDataDataFolder + "\\" + datiVerifica["FileData"].ToString();
                          //        TreeXmlProviderVerifica.Document = x.LoadEncodedFile( SelectedTreeSourceVerifica );
                          //        if ( TreeXmlProviderVerifica.Document.SelectSingleNode( "/Tree/Node" ) != null )
                          //        {
                          //            RecursiveNode( TreeXmlProviderVerifica.Document.SelectSingleNode( "/Tree/Node" ), wl, SelectedDataSourceVerifica );
                          //        }
                          //    }
                          //}

                          //wl.LastParagraph(dati);
                          wl.Save("");
                      }
                      break;
                  case App.TipoFile.Incarico:
                      if (item.Content.ToString() == "Stampa Fascicolo")
                      {
                          //pw = new ProgressWindow();
                          StampaLetteraIncarico = false;
                          dati = mf.GetIncarico(IDSessione);
                          wl.TemplateFileCompletePath = App.AppTemplateStampa;
                          wl.Fascicolo = true;

                          wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true);
                          RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);

                          Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["DataNomina"].ToString());
                          //pw.Close();pw = null;
                          wl.SavePDF(Intestazione, this);
                      }
                            
                      if (item.Content.ToString() == "Stampa Lettera di Incarico Collegio")
                      {
                          btn_StampaLetteraIncaricoCollegio_Click(sender, e);
                      }

                      if (item.Content.ToString() == "Stampa Lettera di Incarico Soggetto Unico")
                      {
                          btn_StampaLetteraIncarico_Click(sender, e);
                      }
                      break;
                  case App.TipoFile.ISQC:
                      if (item.Content.ToString() == "Stampa Fascicolo")
                      {
                          //pw = new ProgressWindow();
                          StampaCodiceEtico = false;
                          dati = mf.GetISQC(IDSessione);
                          wl.TemplateFileCompletePath = App.AppTemplateStampa;
                          wl.Fascicolo = true;

                          wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true);
                          RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);

                          Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["DataNomina"].ToString());
                          //if (pw != null) { pw.Close(); pw = null; }
                          wl.SavePDF(Intestazione, this);
                      }
                      if (item.Content.ToString() == "Stampa Codice Etico")
                      {
                          btn_StampaCodiceEtico_Click(sender, e);
                      }
                      break;
                  case App.TipoFile.Bilancio:
                      if (item.Content.ToString() == "Stampa Fascicolo")
                      {
                          //pw = new ProgressWindow();
                          dati = mf.GetBilancio(IDSessione);
                          wl.TemplateFileCompletePath = App.AppTemplateStampa;
                          wl.Fascicolo = true;

                          wl.Open(dati, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), selectedAliasCodificato, TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value, false, true);
                          RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);

                          Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["Data"].ToString());
                          //if (pw != null) { pw.Close(); pw = null; }
                          wl.SavePDF(Intestazione, this);
                      }
                      break;
            case App.TipoFile.Conclusione:
              if (item.Content.ToString() == "Stampa Fascicolo")
              {
                pw = new ProgressWindow();
                StampaLetteraAttestazione = false;
                StampaManagementLetter = false;
                dati = mf.GetConclusione(IDSessione);
                wl.TemplateFileCompletePath = App.AppTemplateStampa;
                wl.Fascicolo = true;
                wl.Open(
                  dati,
                  cliente["RagioneSociale"].ToString(),
                  cliente["CodiceFiscale"].ToString(),
                  selectedAliasCodificato,
                  TreeXmlProvider.Document.SelectSingleNode("/Tree").ChildNodes[0].Attributes["Titolo"].Value,
                  false,true);
                RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, SelectedDataSource);
                Intestazione = "Società: " + cliente["RagioneSociale"].ToString() + " " + cliente["CodiceFiscale"].ToString() + " Esercizio: " + ConvertDate(dati["Data"].ToString());
                if (pw != null) { pw.Close(); pw = null; }
                wl.SavePDF(Intestazione, this);
              }
              if (item.Content.ToString() == "Stampa Lettera di Attestazione")
              {
                btn_StampaLetteraAttestazione_Click(sender, e);
              }
              if (item.Content.ToString() == "Stampa Management Letter")
              {
                btn_StampaManagementLetter_Click(sender, e);
              }
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
            default:break;
          }
          wl.Close();
          if (pw != null) { pw.Close(); pw = null; }
        }
      }
      Show();Activate();
    }

        private string ConvertDate( string date )
        {
            date = date.ToString().Replace( "01/01/", "" );

            date = date.ToString().Contains( "31/12/" ) ? date.ToString().Replace( "31/12/", "" ) + " / " + ( Convert.ToInt32( date.ToString().Replace( "31/12/", "" ) ) + 1 ).ToString() : date;

            return date;
        }

        bool istobecompleteforprinting = true;

        private bool RecursiveCheckComplete(XmlNode node)
        {
            bool returnvalue = false;

            if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
            {
                if (node.Attributes["ID"] != null)
                {
                    XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");
                    if (node.Attributes["Report"] != null && NodoDato != null && NodoDato.Attributes["Stato"] != null)
                    {
                        if (node.Attributes["Report"].Value == "True" || (NodoDato.Attributes["Stato"] != null && NodoDato.Attributes["Stato"].Value == ((istobecompleteforprinting) ? ((Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString()) : ((Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString()))))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
               // if (node.ParentNode.Name != "Tree")
                {
                    foreach (XmlNode item in node.ChildNodes)
                    {
                        if (item.Name == "Node")
                        {
                            returnvalue = RecursiveCheck(item);
                            if (returnvalue)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return returnvalue;
        }

        private bool RecursiveCheck(XmlNode node)
		{
			bool returnvalue = false;

			if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
			{
				if(node.Attributes["ID"] != null)
				{
                    if(printall)
                    {
                        return true;
                    }

					XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");
                    if ( node.Attributes["Report"] != null && NodoDato != null && NodoDato.Attributes["Stato"] != null )
                    {
                        if ( node.Attributes["Report"].Value == "True" || (NodoDato.Attributes["Stato"] != null && NodoDato.Attributes["Stato"].Value == ((istobecompleteforprinting)?((Convert.ToInt32( App.TipoTreeNodeStato.Completato )).ToString()) : ((Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString()))))
                        {
                            return true;
                        }
                    }
				}
			}
			else
			{
				if (node.ParentNode.Name != "Tree")
				{
					foreach (XmlNode item in node.ChildNodes)
					{
						if (item.Name == "Node")
						{
							returnvalue = RecursiveCheck(item);
							if (returnvalue)
							{
								return true;
							}
						}
					}
				}
			}

			return returnvalue;
		}

        private bool RecursiveCheckDaCompletarePresenti(XmlNode node)
        {
            bool returnvalue = false;

            if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
            {
                if (node.Attributes["ID"] != null)
                {
                    if (printall)
                    {
                        return true;
                    }

                    XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");
                    if (NodoDato != null && NodoDato.Attributes["Stato"] != null)
                    {

                        if ( (NodoDato.Attributes["Stato"] != null && (NodoDato.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString() || NodoDato.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString())))
                        {

                            if ((IDTree == "21" && NodoDato.Attributes["ID"].Value == "17" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='18']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='19']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='20']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "18" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='17']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='19']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='20']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "19" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='17']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='18']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='20']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "20" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='17']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='18']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='19']").Attributes["Stato"].Value == "2")))
                            {
                                return false;
                            }

                            if ((IDTree == "21" && NodoDato.Attributes["ID"].Value == "5" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='30']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='31']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "30" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='5']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='31']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "31" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='5']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='30']").Attributes["Stato"].Value == "2")))
                            {
                                return false;
                            }

                            if ((IDTree == "21" && NodoDato.Attributes["ID"].Value == "10" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='11']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='27']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='33']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "11" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='10']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='27']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='33']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "27" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='10']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='11']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "28" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='10']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='11']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "33" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='10']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='11']").Attributes["Stato"].Value == "2")))
                            {
                                return false;
                            }

                            if ((IDTree == "21" && NodoDato.Attributes["ID"].Value == "28" && _x.Document.SelectSingleNode("/Dati//Dato[@ID='33']").Attributes["Stato"].Value == "2") || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "33" && _x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2"))
                            {
                                return false;
                            }

                            if ((IDTree == "22" && NodoDato.Attributes["ID"].Value == "28" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='34']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='35']").Attributes["Stato"].Value == "2")) || (IDTree == "22" && NodoDato.Attributes["ID"].Value == "34" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='35']").Attributes["Stato"].Value == "2")) || (IDTree == "22" && NodoDato.Attributes["ID"].Value == "35" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='34']").Attributes["Stato"].Value == "2")))
                            {
                                return false;
                            }

                            //Albero B + V

                            if ((IDTree == "23" && NodoDato.Attributes["ID"].Value == "117" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='118']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='119']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='120']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "118" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='117']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='119']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='120']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "119" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='117']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='118']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='120']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "120" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='117']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='118']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='119']").Attributes["Stato"].Value == "2")))
                            {
                                return false;
                            }

                            if ((IDTree == "23" && NodoDato.Attributes["ID"].Value == "105" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='130']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='131']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "130" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='105']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='131']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "131" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='105']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='130']").Attributes["Stato"].Value == "2")))
                            {
                                return false;
                            }

                            if ((IDTree == "23" && NodoDato.Attributes["ID"].Value == "110" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='111']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='127']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='128']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='133']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "111" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='110']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='127']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='128']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='133']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "127" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='110']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='111']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "128" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='110']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='111']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "133" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='110']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='111']").Attributes["Stato"].Value == "2")))
                            {
                                return false;
                            }

                            if ((IDTree == "23" && NodoDato.Attributes["ID"].Value == "128" && _x.Document.SelectSingleNode("/Dati//Dato[@ID='133']").Attributes["Stato"].Value == "2") || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "133" && _x.Document.SelectSingleNode("/Dati//Dato[@ID='128']").Attributes["Stato"].Value == "2"))
                            {
                                return false;
                            }

                            if ((IDTree == "23" && NodoDato.Attributes["ID"].Value == "228" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='234']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='235']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "234" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='228']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='235']").Attributes["Stato"].Value == "2")) || (IDTree == "23" && NodoDato.Attributes["ID"].Value == "235" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='228']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='234']").Attributes["Stato"].Value == "2")))
                            {
                                return false;
                            }







                            //if ((IDTree == "3" && NodoDato.Attributes["ID"].Value == "161" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='164']").Attributes["Stato"].Value == "2")) || (IDTree == "3" && NodoDato.Attributes["ID"].Value == "164" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='161']").Attributes["Stato"].Value == "2")))
                            //{
                            //    return false;
                            //}

                            //if ((IDTree == "21" && NodoDato.Attributes["ID"].Value == "17" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='18']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='19']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='20']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "18" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='17']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='19']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='20']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "19" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='17']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='18']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='20']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "20" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='17']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='18']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='19']").Attributes["Stato"].Value == "2")))
                            //{
                            //    return false;
                            //}

                            //if ((IDTree == "21" && NodoDato.Attributes["ID"].Value == "5" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='30']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='31']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "30" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='5']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='31']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "31" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='5']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='30']").Attributes["Stato"].Value == "2")))
                            //{
                            //    return false;
                            //}

                            //if ((IDTree == "21" && NodoDato.Attributes["ID"].Value == "10" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='11']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='27']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='33']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "11" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='10']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='27']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='23']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "27" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='10']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='11']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "28" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='10']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='11']").Attributes["Stato"].Value == "2")) || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "33" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='10']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='11']").Attributes["Stato"].Value == "2")))
                            //{
                            //    return false;
                            //}

                            //if ((IDTree == "21" && NodoDato.Attributes["ID"].Value == "28" && _x.Document.SelectSingleNode("/Dati//Dato[@ID='33']").Attributes["Stato"].Value == "2") || (IDTree == "21" && NodoDato.Attributes["ID"].Value == "33" && _x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2"))
                            //{
                            //    return false;
                            //}

                            //if ((IDTree == "22" && NodoDato.Attributes["ID"].Value == "28" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='34']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='35']").Attributes["Stato"].Value == "2")) || (IDTree == "22" && NodoDato.Attributes["ID"].Value == "34" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='35']").Attributes["Stato"].Value == "2")) || (IDTree == "22" && NodoDato.Attributes["ID"].Value == "35" && (_x.Document.SelectSingleNode("/Dati//Dato[@ID='28']").Attributes["Stato"].Value == "2" || _x.Document.SelectSingleNode("/Dati//Dato[@ID='34']").Attributes["Stato"].Value == "2")))
                            //{
                            //    return false;
                            //}

                            return true;
                        }
                    }
                }
            }
            else
            {
                //if (node.ParentNode.Name != "Tree")
                {
                    foreach (XmlNode item in node.ChildNodes)
                    {
                        if (item.Name == "Node")
                        {
                            returnvalue = RecursiveCheckDaCompletarePresenti(item);
                            if (returnvalue)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return returnvalue;
        }

        private bool RecursiveNodeCheckFigli(XmlNode node, RTFLib wl)
		{
			bool returnvalue = false;

			if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
			{
				if (RecursiveCheck(node))
				{
					XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");

					returnvalue = wl.AddCheck(node, NodoDato);
				}
			}
			else
			{
				if (node.ParentNode.Name == "Tree" || RecursiveCheck(node))
				{
					foreach (XmlNode item in node.ChildNodes)
					{
						if (item.Name == "Node")
						{
							returnvalue = RecursiveNodeCheckFigli(item, wl);

							if (returnvalue == true)
							{
								break;
							}
						}
					}
				}
			}

			return returnvalue;
		}

        private bool RecursiveNodeCheckFigli(XmlNode node)
        {
            bool returnvalue = false;

            if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
            {
                if (RecursiveCheck(node))
                {
                    XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");

                    returnvalue = true;
                }
            }
            else
            {
                if (node.ParentNode.Name == "Tree" || RecursiveCheck(node))
                {
                    foreach (XmlNode item in node.ChildNodes)
                    {
                        if (item.Name == "Node")
                        {
                            returnvalue = RecursiveNodeCheckFigli(item);

                            if (returnvalue == true)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return returnvalue;
        }

        public bool printall = false;
        public List<string> printall_excludednodes = new List<string>();
        public List<string> printall_nodesnow = new List<string>();

        //private void RecursiveNode(XmlNode node, WordLib wl)

        private void RecursiveNodeOnlyCodes(XmlNode node)
        {
            if (printall)
            {
                if (printall_excludednodes.Contains(node.Attributes["Codice"].Value))
                {
                    return;
                }
            }

            if (StampaLetteraIncarico == false && (node.Attributes["ID"].Value == "142" || node.Attributes["ID"].Value == "2016142") && IDTree == "3")
            {
                return;
            }

            if (StampaCodiceEtico == false && node.Attributes["ID"].Value == "142" && IDTree == "28")
            {
                return;
            }

            if (StampaLetteraAttestazione == false && node.Attributes["ID"].Value == "261" && IDTree == "19")
            {
                return;
            }

            if (StampaManagementLetter == false && node.Attributes["ID"].Value == "281" && IDTree == "19")
            {
                return;
            }
            
            if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
            {
                if (RecursiveCheck(node) || (node.Attributes["ID"].Value == "100013" && IDTree == "26") || (node.Attributes["ID"].Value == "100003" && IDTree == "27"))
                {
                    XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");
                    if (NodoDato != null)
                    {
                        printall_nodesnow.Add(((node.Attributes["Codice"] != null)?node.Attributes["Codice"].Value:"") + "|" + ((node.Attributes["Titolo"] != null)? node.Attributes["Titolo"].Value:""));
                    }
                }
            }
            else
            {
                if (node.ParentNode.Name == "Tree" || RecursiveCheck(node))
                {
                    if (node.ParentNode.Name != "Tree" && (RecursiveNodeCheckFigli(node) == true || node.Attributes["Codice"].Value == "95.311"))
                    {
                        if (StampaLetteraIncarico == true && (node.Attributes["ID"].Value == "150" || node.Attributes["ID"].Value == "154"))
                        {
                            ;
                        }
                        else if (StampaLetteraAttestazione == true && node.Attributes["ID"].Value == "269")
                        {
                            ;
                        }
                        else if (StampaManagementLetter == true && node.Attributes["ID"].Value == "269")
                        {
                            ;
                        }
                        else
                        {
                            ;// printall_nodesnow.Add(((node.Attributes["Codice"] != null) ? node.Attributes["Codice"].Value : "") + "|" + ((node.Attributes["Titolo"] != null) ? node.Attributes["Titolo"].Value : ""));      
                        }
                    }

                    foreach (XmlNode item in node.ChildNodes)
                    {
                        if (item.Name == "Node")
                        {
                            RecursiveNodeOnlyCodes(item);
                        }
                    }
                }
            }
        }

        private void RecursiveNode( XmlNode node, RTFLib wl, string nomefile )
		{
            if(printall)
            {
                if(printall_excludednodes.Contains(node.Attributes["Codice"].Value))
                {
                    return;
                }
            }

            if ( StampaLetteraIncarico == false && (node.Attributes["ID"].Value == "142" || node.Attributes["ID"].Value == "2016142") && IDTree == "3" )
            {
                return;
            }

            if (StampaCodiceEtico == false && node.Attributes["ID"].Value == "142" && IDTree == "28")
            {
                return;
            }

            if ( StampaLetteraAttestazione == false && node.Attributes["ID"].Value == "261" && IDTree == "19" )
            {
                return;
            }

            if (StampaManagementLetter == false && node.Attributes["ID"].Value == "281" && IDTree == "19")
            {
                return;
            }

            if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
			{
                if ( RecursiveCheck( node ) || ( node.Attributes["ID"].Value == "100013" && IDTree == "26" ) || ( node.Attributes["ID"].Value == "<" && IDTree == "27" ) )
				{
					XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");
                    if ( NodoDato != null )
                    {
                        wl.Add( node, NodoDato, IDCliente, IDTree, IDSessione, nomefile );
                    }
				}
			}
			else
			{
				if (node.ParentNode.Name == "Tree" || RecursiveCheck(node))
				{
					if (node.ParentNode.Name != "Tree" && (RecursiveNodeCheckFigli(node, wl) == true || node.Attributes["Codice"].Value == "95.311"))
					{
                        if ( StampaLetteraIncarico == true && ( node.Attributes["ID"].Value == "150" || node.Attributes["ID"].Value == "154" ) )
                        {
                            wl.AddTitleLetteraIncarico( node.Attributes["Titolo"].Value, true );
                        }
                        else if ( StampaLetteraAttestazione == true && node.Attributes["ID"].Value == "269" )
                        {
                            wl.AddTitleLetteraAttestazione( node.Attributes["Titolo"].Value, true );
                        }
                        else if (StampaManagementLetter == true && node.Attributes["ID"].Value == "269")
                        {
                            wl.AddTitleLetteraAttestazione(node.Attributes["Titolo"].Value, true);
                        }                     
                        else
                        {
                            wl.AddTitle( node.Attributes["Codice"].Value + " " + node.Attributes["Titolo"].Value, true );
                        }			
					}

					foreach (XmlNode item in node.ChildNodes)
					{
						if (item.Name == "Node")
						{
                            RecursiveNode( item, wl, nomefile );
						}
					}
				}
			}
		}

        private void tvMain_KeyDown( object sender, KeyEventArgs e )
        {
            e.Handled = true;
        }

        public int scrlollingid = 0;
        public int OLDscrlollingid = -1;

        private void buttonIndietro_Click(object sender, RoutedEventArgs e)
        {
            if ( scrlollingid > 0 )
            {
                scrlollingid--;
                ScrollForced();
            }
        }

        private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
        
        private void buttonAvanti_Click(object sender, RoutedEventArgs e)
        {
            if ( TreeXmlProvider.Document.SelectNodes( "//Sessioni/Sessione[position()>" + (scrlollingid + 1).ToString() + "]" ).Count > 0 )
            {
                scrlollingid++;
                ScrollForced();
            }
        }

        private void ScrollForced()
        {
            if ( OLDscrlollingid != scrlollingid )
            {
                //ProgressWindow pw = new ProgressWindow();

                foreach ( XmlNode xNode in TreeXmlProvider.Document.SelectNodes( "//Sessioni/Sessione[position()>" + scrlollingid.ToString() + "]" ) )
                {

                    if ( xNode.Attributes["Visible"] == null )
                    {
                        XmlAttribute attr = xNode.OwnerDocument.CreateAttribute( "Visible" );
                        xNode.Attributes.Append( attr );
                    }

                    xNode.Attributes["Visible"].Value = "True";
                }

                foreach ( XmlNode xNode in TreeXmlProvider.Document.SelectNodes( "//Sessioni/Sessione[position()<=" + scrlollingid.ToString() + "]" ) )
                {

                    if ( xNode.Attributes["Visible"] == null )
                    {
                        XmlAttribute attr = xNode.OwnerDocument.CreateAttribute( "Visible" );
                        xNode.Attributes.Append( attr );
                    }

                    xNode.Attributes["Visible"].Value = "False";
                }

                //SaveTreeSource();
                if ( TreeXmlProvider.Document != null )
                {
                    RevisoftApplication.XmlManager x = new XmlManager();
                    x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                    x.SaveEncodedFile( SelectedTreeSource, TreeXmlProvider.Document.OuterXml );
                }

                //pw.Close();
                OLDscrlollingid = scrlollingid;
	        }
        }

        private void Window_SizeChanged( object sender, SizeChangedEventArgs e )
        {
            SVTreeFixed.Width = 490;
            SVTreeFixed.Height = e.NewSize.Height - 180;
            SVTreeFixed.Margin = new Thickness( 0, -18, 0, 0 );

            SVTree.Width = e.NewSize.Width - 135 - 490;
            SVTree.Height = e.NewSize.Height - 180;
            SVTree.Margin = new Thickness( -20, -18, 0, 0 );

            SVTreeHeader.Width = e.NewSize.Width - 135 - 490;
           // SVTreeHeader.Margin = new Thickness( -50, 0, 0, 0 );
           // SVTreeHeader.Padding = new Thickness( 51, 0, 0, 0 );
            SVTreeHeader.Margin = new Thickness( -20, 0, 0, 0 );
            SVTreeHeader.Padding = new Thickness( 21, 0, 0, 0 );

            gridTV.Width = tvMain.Width;
            gridTVFixed.Width = tvMainFixed.Width;

            SVTree.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            SVTreeFixed.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

            SVTree.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            SVTreeHeader.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void SVTree_ScrollChanged( object sender, ScrollChangedEventArgs e )
        {
            SVTreeHeader.ScrollToHorizontalOffset( ( sender as ScrollViewer ).HorizontalOffset );

            SVTreeFixed.ScrollToVerticalOffset( ( sender as ScrollViewer ).VerticalOffset );
        }

        private void Grid_MouseWheel( object sender, MouseWheelEventArgs e )
        {
            SVTree.ScrollToVerticalOffset( SVTree.VerticalOffset - e.Delta );
            e.Handled = true;
        }

        private void menuStrumentiStampaVerbali_Click(object sender, RoutedEventArgs e)
        {
            wStampaVerbali wSF = new wStampaVerbali();

            wSF.selectedCliente = IDCliente;
            wSF.selectedSession = IDSessione;

            wSF.inizializza();
            
            wSF.Owner = this;
            wSF.ShowDialog();
        }

        private void btn_ISQCTdL_Click(object sender, RoutedEventArgs e)
        {
            wSceltaISCQ sq = new wSceltaISCQ(IDCliente, IDTree);
            sq.ShowDialog();
            e.Handled = true;
        }

        private void menuStrumentiCopiaDa_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Sicuri di voler importare i valori? I dati attualmente presenti verranno cancellati.","Attenzione",MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

            string IDHere = ((XmlNode)(tvMain.SelectedItem)).Attributes["ID"].Value;

            MasterFile mf = MasterFile.Create();
            XmlDataProviderManager _xnew = null;

            if ((IDTree == "2" && (IDHere == "20" || IDHere == "21" || IDHere == "22" || IDHere == "23" || IDHere == "24" || IDHere == "146")) )
            {
                string nodeID = "";

                switch (IDHere)
                {
                    case "20":
                        nodeID = "600";
                        break;
                    case "21":
                        nodeID = "601";
                        break;
                    case "22":
                        nodeID = "602";
                        break;
                    case "23":
                        nodeID = "603";
                        break;
                    case "24":
                        nodeID = "604";
                        break;
                    case "146":
                        nodeID = "605";
                        break;
                    default:
                        return;
                }

                Hashtable mfg = (mf.GetVigilanzaAssociataFromVerifica(IDSessione));

                _xnew = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + mfg["FileData"].ToString());

                XmlNode NodoDaImportare = _xnew.Document.SelectSingleNode("/Dati//Dato[@ID='" + nodeID + "']");
                NodoDaImportare.Attributes["ID"].Value = IDHere;

                XmlNode NodoDaSostituire = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + IDHere + "']");
                XmlNode NodoImportato = _x.Document.ImportNode(NodoDaImportare, true);

                NodoDaSostituire.ParentNode.AppendChild(NodoImportato);
                NodoDaSostituire.ParentNode.RemoveChild(NodoDaSostituire);

                _x.Save();

                //Copia documenti permanenti
                XmlDataProviderManager _d = new XmlDataProviderManager(App.AppDocumentiDataFile, true);

                XmlNodeList nodelisttmp = _d.Document.SelectNodes("//DOCUMENTI//DOCUMENTO[@Cliente='" + IDCliente + "'][@Sessione='" + mfg["ID"] + "'][@Tree='18'][@Nodo='" + nodeID + "']");

                bool tobesaved = false;

                foreach (XmlNode nodetmp in nodelisttmp)
                {
                    FileInfo f_d = new FileInfo(App.AppDocumentiFolder + "\\" + nodetmp.Attributes["File"].Value);
                    if (f_d.Exists)
                    {
                        tobesaved = true;


                        XmlNode root = _d.Document.SelectSingleNode("//DOCUMENTI");
                        int newID = Convert.ToInt32(root.Attributes["LastID"].Value) + 1;

                        string nuovonomefile = nodetmp.Attributes["File"].Value.Split('.').First() + newID.ToString() + "(Copia)." + nodetmp.Attributes["File"].Value.Split('.').Last();


                        XmlNode newNode = nodetmp.CloneNode(true);

                        newNode.Attributes["ID"].Value = newID.ToString();
                        newNode.Attributes["File"].Value = nuovonomefile;
                        newNode.Attributes["Sessione"].Value = IDSessione;
                        newNode.Attributes["Tree"].Value = IDTree;
                        newNode.Attributes["Nodo"].Value = IDHere;
                        newNode.Attributes["SessioneExtended"].Value = wDocumenti.GetSessioneString(IDTree, IDSessione);
                        newNode.Attributes["NodoExtended"].Value = wDocumenti.GetNodeString(IDTree, IDSessione, IDHere);
                        newNode.Attributes["TreeExtended"].Value = "4";

                        XmlNode nodoImportato = _d.Document.ImportNode(newNode, true);

                        root.AppendChild(nodoImportato);
                        root.Attributes["LastID"].Value = newID.ToString();

                        f_d.CopyTo(App.AppDocumentiFolder + "\\" + nuovonomefile);
                    }
                }

                if (tobesaved)
                {
                    _d.Save();
                }
            }

            if ((IDTree == "18" && (IDHere == "600" || IDHere == "601" || IDHere == "602" || IDHere == "603" || IDHere == "604" || IDHere == "605")))
            {
                string nodeID = "";

                switch (IDHere)
                {
                    case "600":
                        nodeID = "20";
                        break;
                    case "601":
                        nodeID = "21";
                        break;
                    case "602":
                        nodeID = "22";
                        break;
                    case "603":
                        nodeID = "23";
                        break;
                    case "604":
                        nodeID = "24";
                        break;
                    case "605":
                        nodeID = "146";
                        break;
                    default:
                        return;
                }

                Hashtable mfg = (mf.GetVerificaAssociataFromVigilanza(IDSessione));

                _xnew = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + mfg["FileData"].ToString());
                
                XmlNode NodoDaImportare = _xnew.Document.SelectSingleNode("/Dati//Dato[@ID='" + nodeID + "']");
                NodoDaImportare.Attributes["ID"].Value = IDHere;

                XmlNode NodoDaSostituire = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + IDHere + "']");
                XmlNode NodoImportato = _x.Document.ImportNode(NodoDaImportare, true);

                NodoDaSostituire.ParentNode.AppendChild(NodoImportato);
                NodoDaSostituire.ParentNode.RemoveChild(NodoDaSostituire);

                _x.Save();

                //Copia documenti permanenti
                XmlDataProviderManager _d = new XmlDataProviderManager(App.AppDocumentiDataFile, true);

                XmlNodeList nodelisttmp = _d.Document.SelectNodes("//DOCUMENTI//DOCUMENTO[@Cliente='" + IDCliente + "'][@Sessione='" + mfg["ID"] + "'][@Tree='2'][@Nodo='" + nodeID + "']");

                bool tobesaved = false;

                foreach (XmlNode nodetmp in nodelisttmp)
                {
                    FileInfo f_d = new FileInfo(App.AppDocumentiFolder + "\\" + nodetmp.Attributes["File"].Value);
                    if (f_d.Exists)
                    {
                        tobesaved = true;

                        
                        XmlNode root = _d.Document.SelectSingleNode("//DOCUMENTI");
                        int newID = Convert.ToInt32(root.Attributes["LastID"].Value) + 1;

                        string nuovonomefile = nodetmp.Attributes["File"].Value.Split('.').First() + newID.ToString() + "(Copia)." + nodetmp.Attributes["File"].Value.Split('.').Last();


                        XmlNode newNode = nodetmp.CloneNode(true);

                        newNode.Attributes["ID"].Value = newID.ToString();
                        newNode.Attributes["File"].Value = nuovonomefile;
                        newNode.Attributes["Sessione"].Value = IDSessione;
                        newNode.Attributes["Tree"].Value = IDTree;
                        newNode.Attributes["Nodo"].Value = IDHere;
                        newNode.Attributes["SessioneExtended"].Value = wDocumenti.GetSessioneString(IDTree, IDSessione);
                        newNode.Attributes["NodoExtended"].Value = wDocumenti.GetNodeString(IDTree, IDSessione, IDHere);
                        newNode.Attributes["TreeExtended"].Value = "5";

                        XmlNode nodoImportato = _d.Document.ImportNode(newNode, true);

                        root.AppendChild(nodoImportato);
                        root.Attributes["LastID"].Value = newID.ToString();

                        f_d.CopyTo(App.AppDocumentiFolder + "\\" + nuovonomefile);
                    }
                }

                if (tobesaved)
                {
                    _d.Save();
                }

            }

            MessageBox.Show("Dati Importati con sucesso");
        }

        private void btn_Espandi_Click(object sender, RoutedEventArgs e)
        {
            if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
            {
                if (txt_Espandi.Text == "Espandi")
                {
                    txt_Espandi.Text = "Chiudi";

                    var uriSource = new Uri("./Images/icone/navigate_open.png", UriKind.Relative);
                    img_Espandi.Source = new BitmapImage(uriSource);

                    foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
                    {
                        if (item.Attributes["Selected"] != null)
                        {
                            item.Attributes["Selected"].Value = "False";
                        }

                        if (item.Attributes["Expanded"] != null)
                        {
                            item.Attributes["Expanded"].Value = "True";
                        }

                        if (item.Attributes["HighLighted"] != null)
                        {
                            item.Attributes["HighLighted"].Value = "Black";
                        }
                    }
                }
                else
                {
                    txt_Espandi.Text = "Espandi";
                    
                    var uriSource = new Uri("./Images/icone/navigate_close.png", UriKind.Relative);
                    img_Espandi.Source = new BitmapImage(uriSource);

                    foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
                    {
                        if (item.Attributes["Selected"] != null)
                        {
                            item.Attributes["Selected"].Value = "False";
                        }

                        if (item.Attributes["Expanded"] != null)
                        {
                            if (item.ParentNode.Name == "Tree")
                            {
                                item.Attributes["Expanded"].Value = "True";
                            }
                            else
                            {
                                item.Attributes["Expanded"].Value = "False";
                            }
                        }

                        if (item.Attributes["HighLighted"] != null)
                        {
                            item.Attributes["HighLighted"].Value = "Black";
                        }
                    }
                }

            }
        }
    
    }
}

namespace ConvNS
{
    [ValueConversion(typeof(string), typeof(string))]
    public class TypeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string tipo = (string)value;

            if (tipo != "Nodo Multiplo")
            {
                return "Visible";
            }
            else
            {
                return "Collapsed";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class TypeVisibilityConverterifempty : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string tipo = (string)value;

            if (tipo != "")
            {
                return "Visible";
            }
            else
            {
                return "Hidden";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion( typeof( string ), typeof( string ) )]
    public class boolVisibilityConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            string tipo = (string)value;

            if ( tipo == "True" )
            {
                return "Visible";
            }
            else
            {
                return "Collapsed";
            }
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return null;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
	public class FontWeightConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				if (value.ToString() == "True")
				{
					return "Bold";
				}
				else
				{
					return "Regular";
				}
			}
			else
			{
				return "Regular";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
        
    [ValueConversion(typeof(string), typeof(string))]
    public class IsTabStopConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return "False";
            }
            else
            {
                return "True";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return "Hidden";
            }
            else
            {
                return "Visible";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class Money : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            culture = CultureInfo.CreateSpecificCulture("it-IT"); 
            double tmpvalue =  0.0;

			double.TryParse(value.ToString(), out tmpvalue);

            if (tmpvalue == 0.0)
            {
                return "";
            }
            else
            {
                return String.Format("{0:#,0.00}", tmpvalue);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            culture = CultureInfo.CreateSpecificCulture("it-IT"); 

            double tmpvalue = 0.0;

            if (!value.ToString().Contains(','))
            {
				if (value.ToString().Split('.').Length == 2 && value.ToString().Split('.')[1].Length != 3)
                {
                    value = value.ToString().Replace('.', ',');
                }
            }

			double.TryParse(value.ToString(), out tmpvalue);

            //tmpvalue = System.Convert.ToDouble(System.Convert.ToString(value).Replace("€ ", ""));

            return String.Format("{0:#,0.00}", tmpvalue);
        }
    }


    [ValueConversion(typeof(string), typeof(string))]
    public class Money2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            culture = CultureInfo.CreateSpecificCulture("it-IT");
            double tmpvalue = 0.0;

            double.TryParse(value.ToString(), out tmpvalue);

            if (tmpvalue == 0.0)
            {
                return "0";
            }
            else
            {
                return String.Format("{0:#,0.00000}", tmpvalue);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            culture = CultureInfo.CreateSpecificCulture("it-IT");

            double tmpvalue = 0.0;

            if (!value.ToString().Contains(','))
            {
                if (value.ToString().Split('.').Length == 2 && value.ToString().Split('.')[1].Length != 3)
                {
                    value = value.ToString().Replace('.', ',');
                }
            }

            double.TryParse(value.ToString(), out tmpvalue);

            //tmpvalue = System.Convert.ToDouble(System.Convert.ToString(value).Replace("€ ", ""));

            return String.Format("{0:#,0.00000}", tmpvalue);
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
	public class MoneyWithZero : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			double tmpvalue = 0.0;

			double.TryParse(value.ToString(), out tmpvalue);

			return String.Format("{0:#,0.00}", tmpvalue);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			culture = CultureInfo.CreateSpecificCulture("it-IT");

			double tmpvalue = 0.0;

			if (!value.ToString().Contains(','))
			{
				if (value.ToString().Split('.').Length == 2 && value.ToString().Split('.')[1].Length != 3)
				{
					value = value.ToString().Replace('.', ',');
				}
			}

			double.TryParse(value.ToString(), out tmpvalue);
			//tmpvalue = System.Convert.ToDouble(System.Convert.ToString(value).Replace("€ ", ""));

			return String.Format("{0:#,0.00}", tmpvalue);
		}
	}

	[ValueConversion(typeof(string), typeof(string))]
	public class MoneyNodecimal : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			double tmpvalue = 0.0;

			double.TryParse(value.ToString(), out tmpvalue);

			if (tmpvalue == 0.0)
			{
				return "";
			}
			else
			{
				return String.Format("{0:#,0}", tmpvalue);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			culture = CultureInfo.CreateSpecificCulture("it-IT");

			double tmpvalue = 0.0;

			if (!value.ToString().Contains(','))
			{
				if (value.ToString().Split('.').Length == 2 && value.ToString().Split('.')[1].Length != 3)
				{
					value = value.ToString().Replace('.', ',');
				}
			}

			double.TryParse(value.ToString(), out tmpvalue);

			//tmpvalue = System.Convert.ToDouble(System.Convert.ToString(value).Replace("€ ", ""));

			return String.Format("{0:#,0}", tmpvalue);
		}
	}

	[ValueConversion(typeof(string), typeof(string))]
	public class Integer : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			culture = CultureInfo.CreateSpecificCulture("it-IT");
			double tmpvalue = 0.0;

			double.TryParse(value.ToString(), out tmpvalue);

			if (tmpvalue == 0.0)
			{
				return "";
			}
			else
			{
				return String.Format("{0:#,0}", tmpvalue);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			culture = CultureInfo.CreateSpecificCulture("it-IT");

			double tmpvalue = 0.0;

			if (!value.ToString().Contains(','))
			{
				if (value.ToString().Split('.').Length == 2 && value.ToString().Split('.')[1].Length != 3)
				{
					value = value.ToString().Replace('.', ',');
				}
			}

			double.TryParse(value.ToString(), out tmpvalue);

			//tmpvalue = System.Convert.ToDouble(System.Convert.ToString(value).Replace("€ ", ""));

			return String.Format("{0:#,0.00}", tmpvalue);
		}
	}

    [ValueConversion(typeof(string), typeof(string))]
    public class Percent : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            culture = CultureInfo.CreateSpecificCulture("it-IT");
            double tmpvalue = 0.0;

			double.TryParse(value.ToString(), out tmpvalue);

			tmpvalue = tmpvalue * 100.0;

            if (tmpvalue == 0.0)
            {
                return "";
            }
            else
            {
                return String.Format("{0:0.00} %", tmpvalue);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            culture = CultureInfo.CreateSpecificCulture("it-IT");

            double tmpvalue = 0.0;

            value = value.ToString().Replace(" %", "");

			double.TryParse(value.ToString(), out tmpvalue);

            return String.Format("{0:0.00}", tmpvalue);
        }
    } 

    [ValueConversion(typeof(string), typeof(string))]
    public class BackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null) 
            {
                return Brushes.LightGray;
            }
            else
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }    

    [ValueConversion(typeof(string), typeof(string))]
    public class BackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int alternateCount = (int)value;

            if (alternateCount % 2 == 0)
            {
                return "#ffffff";
            }
            else
            {
                return "#ccccff";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion( typeof( string ), typeof( string ) )]
    public class IconeSospesiConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if ( value.ToString() != "")
            {
                return ".\\Images\\icone\\Stato\\sospesi.png";
            }
            else
            {
                return ".\\Images\\icone\\Stato\\nothing.png";
            }
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return null;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class IconeStatoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value.ToString())
            {
                case "6":
                case "0":
					return ".\\Images\\icone\\Stato\\nonapp_small.png";
                case "1":
					return ".\\Images\\icone\\Stato\\parziale.png";
				case "2":
					return ".\\Images\\icone\\Stato\\completo.png";
				case "3":
					return ".\\Images\\icone\\Stato\\warning.png";
				case "4":
					return ".\\Images\\icone\\Stato\\check2.png";
				case "-2":
                    return ".\\Images\\icone\\Stato\\note_pinned.png";
				case "-3":
					return ".\\Images\\icone\\Stato\\VociCompilate.png";
                case "-5":
                    return ".\\Images\\icone\\Stato\\Sigillo.png";
                case "-6":
                    return ".\\Images\\icone\\Stato\\SigilloRotto.png";
				case "-4":
                case "-1":
                default:
                    return ".\\Images\\icone\\Stato\\nothing.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
        
    [ValueConversion(typeof(string), typeof(string))]
    public class TooltipStatoConverter : IMultiValueConverter
    {
        public object Convert(object [] value, Type targetType, object parameter, CultureInfo culture)
        {
            //attenzione abbiamo RevisoftApplication.App.NomeTipoTreeNodeStato((RevisoftApplication.App.TipoTreeNodeStato)
            switch (((XmlAttribute)(value[0])).Value.ToString())
            {
                case "6":
                case "0":
                    return "Non Applicabile";
                case "1":
                    return "Da Completare";
                case "2":
                    return "Completato";
                case "3":
                    return "Resettato";
                case "4":
                    return "In scrittura";
                case "-2":
                    return "Promemoria";
				case "-3":
					return "Voci Compilate";
				case "-4":
					return "Voce in Sola Lettura";
                case "-5":
                    return ((XmlAttribute)(value[1])).Value.ToString();
                case "-6":
                    return ((XmlAttribute)(value[1])).Value.ToString();
                case "-1":
                default:
                    return "Nessuno stato assegnato";
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class RadioButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            switch (value.ToString())
            {
                case "Si":
                    return (parameter.ToString() == "Si");                    
                case "No":
                    return (parameter.ToString() == "No");
                case "NA":
                default:
                    return (parameter.ToString() == "NA");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            bool useValue = (bool)value;
            if (useValue)
            {
                return parameter.ToString();
            }

            return null;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class ImageNoteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string down = "./Images/icone/navigate_down.png";
            string up = "./Images/icone/navigate_up.png";

            if ((string)value == string.Empty)
            {
                return down;
            }
            else
            {
                return up;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class ImageNoteVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == string.Empty)
            {
                return "Collapsed";
            }
            else
            {
                return "Visible";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
*/
