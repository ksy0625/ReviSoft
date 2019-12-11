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

using System.Data.SqlClient;
using System.Data;

namespace RevisoftApplication
{
  public partial class wFlussi : System.Windows.Window
  {
    private string _cliente = "";
    public int IDCliente = -1;
    private bool _ReadOnly = true;
    private XmlDataProviderManager _x = null;
    public RevisoftApplication.wSchedaSceltaFlussi.TipoFlusso tipo =
      new RevisoftApplication.wSchedaSceltaFlussi.TipoFlusso();
    Hashtable htTabs = new Hashtable();

    //----------------------------------------------------------------------------+
    //                             proprieta' Cliente                             |
    //----------------------------------------------------------------------------+
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

    //----------------------------------------------------------------------------+
    //                                  wFlussi                                   |
    //----------------------------------------------------------------------------+
    public wFlussi()
    {
      InitializeComponent();
      lblTitolo.Foreground = App._arrBrushes[0];
    }

    //----------------------------------------------------------------------------+
    //                            proprieta' ReadOnly                             |
    //----------------------------------------------------------------------------+
    public bool ReadOnly
    {
      set
      {
        _ReadOnly = value;
      }
    }

    //----------------------------------------------------------------------------+
    //                                GeneraTitolo                                |
    //----------------------------------------------------------------------------+
    public void GeneraTitolo()
    {
      ;
    }

    //----------------------------------------------------------------------------+
    //                                    Load                                    |
    //----------------------------------------------------------------------------+
    public void Load()
    {
      tabControlFlussi.Items.Clear();
      MasterFile mf = MasterFile.Create();
      if (mf.CheckDoppio_Flussi(IDCliente))
      {
        // setto dati
        Hashtable ht = new Hashtable();
        ht.Add("Cliente", IDCliente);
        mf.SetFlussi(ht, IDCliente);
      }
      Hashtable htSelected = mf.GetFlussi(IDCliente.ToString());
      _x = new XmlDataProviderManager(App.AppDataDataFolder + "\\" +
        htSelected["FileData"].ToString());
      switch (tipo)
      {
        case wSchedaSceltaFlussi.TipoFlusso.ISQC:
          lblTitolo.Content = "Comunicazioni per controllo di qualità interno (ISQC)";
          break;
        case wSchedaSceltaFlussi.TipoFlusso.Societa:
          lblTitolo.Content = "Comunicazioni fra Organi della società";
          break;
        case wSchedaSceltaFlussi.TipoFlusso.Gruppo:
          lblTitolo.Content = "Comunicazioni fra organi della società del gruppo";
          break;
        case wSchedaSceltaFlussi.TipoFlusso.Terzi:
          lblTitolo.Content = "Comunicazioni con terzi";
          break;
        default:
          break;
      }
      TabItem ti;
      string xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato";
      foreach (XmlNode item in _x.Document.SelectNodes(xpath))
      {
        string xpathinternal = "//Dati[@TIPO=" + Convert.ToInt32(tipo) +
          "]/Dato[@TAB=" + item.Attributes["TAB"].Value + "]/Valore";
        foreach (XmlNode items in item.SelectNodes("Valore"))
        {
          items.Attributes["ALLEGATI"].Value =
            ((items.HasChildNodes == true) ? "PRESENTI" : "");
        }
        ucTabellaFlussi t = new ucTabellaFlussi();
        t.ReadOnly = _ReadOnly;
        t.Load(_x, xpathinternal);
        ti = new TabItem();
        ti.Header = item.Attributes["TABNAME"].Value;
        if (!htTabs.Contains(item.Attributes["TAB"].Value))
        {
          htTabs.Add(item.Attributes["TAB"].Value, item.Attributes["TABNAME"].Value);
        }
        ti.Content = t;
        tabControlFlussi.Items.Add(ti);
      }
    }

    //----------------------------------------------------------------------------+
    //                          UserControl_SizeChanged                           |
    //----------------------------------------------------------------------------+
    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
    }

    //----------------------------------------------------------------------------+
    //                        tabControl_SelectionChanged                         |
    //----------------------------------------------------------------------------+
    private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      tabControlFlussi.Focus();
    }

    //----------------------------------------------------------------------------+
    //                               btnNuova_Click                               |
    //----------------------------------------------------------------------------+
    private void btnNuova_Click(object sender, RoutedEventArgs e)
    {
      wSchedaImmissioneFlussi immissioneFLUSSI = new wSchedaImmissioneFlussi();
      immissioneFLUSSI.Owner = this;
      immissioneFLUSSI.tipo = tipo;
      immissioneFLUSSI.IDCliente = IDCliente;
      immissioneFLUSSI.IDComunicazione = -1;
      immissioneFLUSSI.IDGruppoComunicazione = -1;
      immissioneFLUSSI._x = _x;
      immissioneFLUSSI.Load();
      immissioneFLUSSI.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                             btnRisposta_Click                              |
    //----------------------------------------------------------------------------+
    private void btnRisposta_Click(object sender, RoutedEventArgs e)
    {
      if (tabControlFlussi.SelectedItem == null
        || ((UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent).dtgMain.SelectedItem == null
        || ((XmlNode)(((UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent).dtgMain.SelectedItem)).Attributes["GRUPPO"] == null)
      {
        MessageBox.Show("Occorre selezionare una comunicazione per creare una risposta");
        return;
      }
      int IDTabSelected = -1;
      if (tabControlFlussi.SelectedItem == null) return;
      foreach (DictionaryEntry item in htTabs)
      {
        if (item.Value.ToString() == ((TabItem)(tabControlFlussi.SelectedItem)).Header.ToString())
        {
          IDTabSelected = Convert.ToInt32(item.Key.ToString());
        }
      }
      int IDSelected = -1;
      int IDGruppoComunicazioneSelected = Convert.ToInt32(
        ((XmlNode)(((UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent).dtgMain.SelectedItem))
          .Attributes["GRUPPO"].Value);
      string oldIR = ((XmlNode)(((UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent)
        .dtgMain.SelectedItem)).Attributes["IR"].Value;
      wSchedaImmissioneFlussi immissioneFLUSSI = new wSchedaImmissioneFlussi();
      immissioneFLUSSI.Owner = this;
      immissioneFLUSSI.tipo = tipo;
      immissioneFLUSSI.IDCliente = IDCliente;
      immissioneFLUSSI.IDComunicazione = IDSelected;
      immissioneFLUSSI.IDGruppoComunicazione = IDGruppoComunicazioneSelected;
      immissioneFLUSSI.IDTab = IDTabSelected;
      immissioneFLUSSI.oldIR = oldIR;
      immissioneFLUSSI._x = _x;
      immissioneFLUSSI.Load();
      immissioneFLUSSI.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                             btnModifica_Click                              |
    //----------------------------------------------------------------------------+
    private void btnModifica_Click(object sender, RoutedEventArgs e)
    {
      if (tabControlFlussi.SelectedItem == null
        || ((UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent).dtgMain.SelectedItem == null
        || ((XmlNode)(((UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent)
          .dtgMain.SelectedItem)).Attributes["ID"] == null)
      {
        MessageBox.Show("Occorre selezionare una comunicazione per modificarla");
        return;
      }
      int IDTabSelected = -1;
      if (tabControlFlussi.SelectedItem == null) return;
      foreach (DictionaryEntry item in htTabs)
      {
        if (item.Value.ToString() == ((TabItem)(tabControlFlussi.SelectedItem)).Header.ToString())
        {
          IDTabSelected = Convert.ToInt32(item.Key.ToString());
        }
      }
      int IDSelected = Convert.ToInt32(
        ((XmlNode)(((UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent)
          .dtgMain.SelectedItem)).Attributes["ID"].Value);
      int IDGruppoComunicazioneSelected = Convert.ToInt32(
        ((XmlNode)(((UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent)
          .dtgMain.SelectedItem)).Attributes["GRUPPO"].Value);
      wSchedaImmissioneFlussi immissioneFLUSSI = new wSchedaImmissioneFlussi();
      immissioneFLUSSI.Owner = this;
      immissioneFLUSSI.tipo = tipo;
      immissioneFLUSSI.IDCliente = IDCliente;
      immissioneFLUSSI.IDComunicazione = IDSelected;
      immissioneFLUSSI.IDGruppoComunicazione = IDGruppoComunicazioneSelected;
      immissioneFLUSSI.IDTab = IDTabSelected;
      immissioneFLUSSI._x = _x;
      immissioneFLUSSI.Load();
      immissioneFLUSSI.ShowDialog();
    }

    //----------------------------------------------------------------------------+
    //                              btnElimina_Click                              |
    //----------------------------------------------------------------------------+
    private void btnElimina_Click_old(object sender, RoutedEventArgs e)
    {
      if (tabControlFlussi.SelectedItem == null
        || ((UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent).dtgMain.SelectedItem == null
        || ((XmlNode)(((UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent)
          .dtgMain.SelectedItem)).Attributes["ID"] == null)
      {
        MessageBox.Show("Occorre selezionare una comunicazione per eliminarla");
        return;
      }
      if (MessageBox.Show("Sicuri di voler eliminare la comunicazione?",
        "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
      int IDTabSelected = -1;
      if (tabControlFlussi.SelectedItem == null) return;
      foreach (DictionaryEntry item in htTabs)
      {
        if (item.Value.ToString() == ((TabItem)(tabControlFlussi.SelectedItem)).Header.ToString())
        {
          IDTabSelected = Convert.ToInt32(item.Key.ToString());
        }
      }
      int IDSelected = Convert.ToInt32(
        ((XmlNode)(((UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent)
          .dtgMain.SelectedItem)).Attributes["ID"].Value);
      int IDGruppoComunicazioneSelected = Convert.ToInt32(
        ((XmlNode)(((UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent)
          .dtgMain.SelectedItem)).Attributes["GRUPPO"].Value);
      string xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" +
        IDTabSelected + "]/Valore[@GRUPPO=" + IDGruppoComunicazioneSelected +
        "][@ID=" + IDSelected + "]";
      XmlNode tmpNode = _x.Document.SelectSingleNode(xpath);
      if (tmpNode.ParentNode.ChildNodes.Count > 1)
      {
        if (tmpNode != null)
        {
          tmpNode.ParentNode.RemoveChild(tmpNode);
        }
        _x.Save();
        Load();
        foreach (TabItem item in tabControlFlussi.Items)
        {
          if (item.Header.ToString() == htTabs[IDTabSelected.ToString()].ToString())
          {
            tabControlFlussi.SelectedItem = item;
          }
        }
        tabControlFlussi.Focus();
      }
      else
      {
        tmpNode.ParentNode.ParentNode.RemoveChild(tmpNode.ParentNode);
        _x.Save();
        Load();
        tabControlFlussi.Items.MoveCurrentToLast();
        tabControlFlussi.Focus();
      }
    }
    private void btnElimina_Click(object sender, RoutedEventArgs e)
    {
#if (!DBG_TEST)
      btnElimina_Click_old(sender, e);return;
#endif
      if (tabControlFlussi.SelectedItem == null
        || ((UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent).dtgMain.SelectedItem == null
        || ((XmlNode)(((UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent)
          .dtgMain.SelectedItem)).Attributes["ID"] == null)
      {
        MessageBox.Show("Occorre selezionare una comunicazione per eliminarla");
        return;
      }
      if (MessageBox.Show("Sicuri di voler eliminare la comunicazione?",
        "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
      int IDTabSelected = -1;
      if (tabControlFlussi.SelectedItem == null) return;
      foreach (DictionaryEntry item in htTabs)
      {
        if (item.Value.ToString() == ((TabItem)(tabControlFlussi.SelectedItem)).Header.ToString())
        {
          IDTabSelected = Convert.ToInt32(item.Key.ToString());
        }
      }
      int IDSelected = Convert.ToInt32(
        ((XmlNode)(((UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent)
          .dtgMain.SelectedItem)).Attributes["ID"].Value);
      int IDGruppoComunicazioneSelected = Convert.ToInt32(
        ((XmlNode)(((UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent)
          .dtgMain.SelectedItem)).Attributes["GRUPPO"].Value);
      string xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" +
        IDTabSelected + "]/Valore[@GRUPPO=" + IDGruppoComunicazioneSelected +
        "][@ID=" + IDSelected + "]";
      XmlNode tmpNode = _x.Document.SelectSingleNode(xpath);
      //----------------------------------------------------------------------------+
      //                   selezionare il nodo 'Dati/Dato/Valore'                   |
      //                   per avere tutte le chiavi --> nodoDati                   |
      //         passare nodoDati alla sp. Nella sp, cancellare il valore.          |
      //          Se era l' ultimo, cancellare anche il Dato. Restituire 1          |
      //          se e' stato cancellato il Dato, 0 se solo il valore, -1           |
      //                             in caso di errore.                             |
      //----------------------------------------------------------------------------+
      string guid, nodoDati;
      int res;

      MasterFile mf = MasterFile.Create();
      Hashtable htSelected = mf.GetFlussi(IDCliente.ToString());
      guid = htSelected["FileData"].ToString();
      nodoDati = tmpNode.ParentNode.ParentNode.OuterXml;
      using (System.Data.SqlClient.SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("flussi.EliminaComunicazione", conn);
        cmd.Parameters.AddWithValue("@IDCliente", IDCliente.ToString());
        cmd.Parameters.AddWithValue("@rec", nodoDati);
        cmd.Parameters.AddWithValue("@guid", guid);
        var retPar = cmd.Parameters.Add("@retVal", SqlDbType.Int);
        retPar.Direction = ParameterDirection.ReturnValue;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        res = -1;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wFlussi.btnElimina_Click exception");
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "SQL call 'flussi.EliminaComunicazione' failed: errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
        res = Convert.ToInt32(retPar.Value.ToString());
      }
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
      if (App.m_xmlCache.Contains(guid)) App.m_xmlCache.Remove(guid);
      if (res < 0) return;
      _x.Load(); Load();
      if (res == 0)
      {
        foreach (TabItem item in tabControlFlussi.Items)
        {
          if (item.Header.ToString() == htTabs[IDTabSelected.ToString()].ToString())
          {
            tabControlFlussi.SelectedItem = item;
          }
        }
        tabControlFlussi.Focus();
      }
      else
      {
        tabControlFlussi.Items.MoveCurrentToLast();
        tabControlFlussi.Focus();
      }
      // attenzione: i documenti allegati non vengono cancellati!!!
    }

    //----------------------------------------------------------------------------+
    //                            btnModificaMD_Click                             |
    //----------------------------------------------------------------------------+
    private void btnModificaMD_Click_old(object sender, RoutedEventArgs e)
    {
      if (tabControlFlussi.SelectedItem == null) return;
      var dialog = new wInputBox("Inserire testo Modifica");
      dialog.ResponseText = ((TabItem)(tabControlFlussi.SelectedItem)).Header.ToString();
      dialog.ShowDialog();
      if (dialog.ResponseText.Trim() == "")
      {
        MessageBox.Show("Nessun valore inserito");
        return;
      }
      string titolo = dialog.ResponseText.Replace("&", "&amp;").Replace("\"", "'");
      int IDTabSelected = -1;
      foreach (DictionaryEntry item in htTabs)
      {
        if (item.Value.ToString() == ((TabItem)(tabControlFlussi.SelectedItem)).Header.ToString())
        {
          IDTabSelected = Convert.ToInt32(item.Key.ToString());
        }
      }
      string xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" + IDTabSelected + "]";
      XmlNode tmpNode = _x.Document.SelectSingleNode(xpath);
      if (tmpNode != null)
      {
        tmpNode.Attributes["TABNAME"].Value = titolo;
        htTabs[IDTabSelected.ToString()] = titolo;
      }
      _x.Save();
      Load();
      foreach (TabItem item in tabControlFlussi.Items)
      {
        if (item.Header.ToString() == htTabs[IDTabSelected.ToString()].ToString())
        {
          tabControlFlussi.SelectedItem = item;
        }
      }
      tabControlFlussi.Focus();
    }
    private void btnModificaMD_Click(object sender, RoutedEventArgs e)
    {
#if (!DBG_TEST)
      btnModificaMD_Click_old(sender, e);return;
#endif
      if (tabControlFlussi.SelectedItem == null) return;
      var dialog = new wInputBox("Inserire testo Modifica");
      dialog.ResponseText = ((TabItem)(tabControlFlussi.SelectedItem)).Header.ToString();
      dialog.ShowDialog();
      if (dialog.ResponseText.Trim() == "")
      {
        MessageBox.Show("Nessun valore inserito");
        return;
      }
      string titolo = dialog.ResponseText.Replace("&", "&amp;").Replace("\"", "'");
      int IDTabSelected = -1;
      foreach (DictionaryEntry item in htTabs)
      {
        if (item.Value.ToString() == ((TabItem)(tabControlFlussi.SelectedItem)).Header.ToString())
        {
          IDTabSelected = Convert.ToInt32(item.Key.ToString());
        }
      }
      string xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" + IDTabSelected + "]";
      XmlNode tmpNode = _x.Document.SelectSingleNode(xpath);
      if (tmpNode != null)
      {
        tmpNode.Attributes["TABNAME"].Value = titolo;
        htTabs[IDTabSelected.ToString()] = titolo;
      }

      string guid, nodoDati;

      MasterFile mf = MasterFile.Create();
      Hashtable htSelected = mf.GetFlussi(IDCliente.ToString());
      guid = htSelected["FileData"].ToString();
      nodoDati = tmpNode.ParentNode.OuterXml;
      using (System.Data.SqlClient.SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("flussi.RinominaTab", conn);
        cmd.Parameters.AddWithValue("@IDCliente", IDCliente.ToString());
        cmd.Parameters.AddWithValue("@rec", nodoDati);
        cmd.Parameters.AddWithValue("@guid", guid);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wFlussi.btnModificaMD_Click exception");
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "SQL call 'flussi.RinominaTab' failed: errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
      if (App.m_xmlCache.Contains(guid)) App.m_xmlCache.Remove(guid);
      _x.Load(); Load();


      foreach (TabItem item in tabControlFlussi.Items)
      {
        if (item.Header.ToString() == htTabs[IDTabSelected.ToString()].ToString())
        {
          tabControlFlussi.SelectedItem = item;
        }
      }
      tabControlFlussi.Focus();
    }

    //----------------------------------------------------------------------------+
    //                             buttonChiudi_Click                             |
    //----------------------------------------------------------------------------+
    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

    //----------------------------------------------------------------------------+
    //                          btn_GuidaRevisoft_Click                           |
    //----------------------------------------------------------------------------+
    private void btn_GuidaRevisoft_Click(object sender, RoutedEventArgs e)
    {
      GuidaRevisoft(true);
    }

    //----------------------------------------------------------------------------+
    //                               GuidaRevisoft                                |
    //----------------------------------------------------------------------------+
    private void GuidaRevisoft(bool posizioneMouse)
    {
      wGuidaRevisoft w = new wGuidaRevisoft();
      w.Owner = this;
      string nota = "<P class=MsoNormal style=\"TEXT-ALIGN: justify; MARGIN: 0cm 0cm 8pt\">" +
        "<SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%'>" +
        "Questo modulo si prefigge di correlare lo scambio di comunicazioni ed informazioni " +
        "degli organi della società fra di loro ed i terzi." +
        "<?xml:namespace prefix = \"o\" ns = \"urn:schemas-microsoft-com:office:office\" />" +
        "<o:p></o:p></SPAN></P><P class=MsoNormal style=\"TEXT-ALIGN: justify; " +
        "MARGIN: 0cm 0cm 8pt\"><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; " +
        "LINE-HEIGHT: 107%'>Il modulo è diviso in quattro aree destinate agli scambi fra " +
        "appartenenti alla struttura del revisore, agli organi della società soggetta a " +
        "revisione, del gruppo al quale appartiene e da terzi.<o:p></o:p></SPAN></P>" +
        "<P class=MsoNormal style=\"TEXT-ALIGN: justify; MARGIN: 0cm 0cm 8pt\">" +
        "<SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%'>" +
        "Revisoft creerà, per ogni mittente/destinatario – identificato con una etichetta – " +
        "una tabella nella quale ogni riga indicherà una comunicazione son il soggetto indicato." +
        "<o:p></o:p></SPAN></P><P class=MsoNormal style=\"TEXT-ALIGN: justify; " +
        "MARGIN: 0cm 0cm 8pt\"><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; " +
        "LINE-HEIGHT: 107%'>Ogni riga evidenzierà se la comunicazione è stata inviata o ricevuta, " +
        "la data, un campo per le annotazioni ed il simbolo della presenza di allegati.<o:p></o:p>" +
        "</SPAN></P><P class=MsoNormal style=\"TEXT-ALIGN: justify; MARGIN: 0cm 0cm 8pt\">" +
        "<SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%'>" +
        "Accedendo ad un’area con il tasto APRI FLUSSI e dopo aver scelto il cliente, Revisoft " +
        "apre una finestra che:<o:p></o:p></SPAN></P><P class=MsoListParagraphCxSpFirst " +
        "style=\"TEXT-ALIGN: justify; MARGIN: 0cm 0cm 0pt 21.3pt; TEXT-INDENT: -18pt; " +
        "mso-add-space: auto; mso-list: l0 level1 lfo1\"><SPAN style='FONT-SIZE: 12pt; " +
        "FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%; mso-fareast-font-family: Cambria; " +
        "mso-bidi-font-family: Cambria'><SPAN style=\"mso-list: Ignore\">-" +
        "<SPAN style='FONT: 7pt \"Times New Roman\"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
        "&nbsp;&nbsp; </SPAN></SPAN></SPAN><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\"," +
        "serif; LINE-HEIGHT: 107%'>Sulla barra di sinistra espone i tasti di funzione per creare " +
        "una nuova comunicazione, rispondere, modificarla ed eliminarla. E’ anche presente un " +
        "tasto per modificare l’etichetta del soggetto col quale avviene lo scambio.<o:p></o:p>" +
        "</SPAN></P><P class=MsoListParagraphCxSpMiddle style=\"TEXT-ALIGN: justify; " +
        "MARGIN: 0cm 0cm 0pt 21.3pt; TEXT-INDENT: -18pt; mso-add-space: auto; mso-list: l0 " +
        "level1 lfo1\"><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; " +
        "LINE-HEIGHT: 107%; mso-fareast-font-family: Cambria; mso-bidi-font-family: Cambria'>" +
        "<SPAN style=\"mso-list: Ignore\">-<SPAN style='FONT: 7pt \"Times New Roman\"'>" +
        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </SPAN></SPAN></SPAN>" +
        "<SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%'>" +
        "Con il tasto NUOVA si apre una finestra per l’immissione dei dati che verranno " +
        "esposti nella riga della comunicazione e per caricare gli allegati.<o:p></o:p></SPAN>" +
        "</P><P class=MsoListParagraphCxSpMiddle style=\"TEXT-ALIGN: justify; " +
        "MARGIN: 0cm 0cm 0pt 21.3pt; TEXT-INDENT: -18pt; mso-add-space: auto; " +
        "mso-list: l0 level1 lfo1\"><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\"," +
        "serif; LINE-HEIGHT: 107%; mso-fareast-font-family: Cambria; mso-bidi-font-family: " +
        "Cambria'><SPAN style=\"mso-list: Ignore\">-<SPAN style='FONT: 7pt \"Times New Roman\"'>" +
        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </SPAN></SPAN></SPAN>" +
        "<SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%'>" +
        "Per collegare una comunicazione con un’altra già presente occorre SELEZIONARE " +
        "la riga della comunicazione presente ed usare il tasto RISPOSTA (si intende " +
        "risposta sia al mittente di una comunicazione ricevuta, che del destinatario di " +
        "una nostra comunicazione).<o:p></o:p></SPAN></P><P class=MsoListParagraphCxSpLast " +
        "style=\"TEXT-ALIGN: justify; MARGIN: 0cm 0cm 8pt 21.3pt; TEXT-INDENT: -18pt; " +
        "mso-add-space: auto; mso-list: l0 level1 lfo1\"><SPAN style='FONT-SIZE: 12pt; " +
        "FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%; mso-fareast-font-family: Cambria; " +
        "mso-bidi-font-family: Cambria'><SPAN style=\"mso-list: Ignore\">-" +
        "<SPAN style='FONT: 7pt \"Times New Roman\"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
        "&nbsp;&nbsp;&nbsp; </SPAN></SPAN></SPAN><SPAN style='FONT-SIZE: 12pt; " +
        "FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%'>Le varie comunicazioni verranno " +
        "collegate fra di loro e le righe che le rappresentano avranno lo stesso colore di " +
        "fondo, che sarà diverso da quelle non collegate o che sono correlate in altri " +
        "raggruppamenti.<o:p></o:p></SPAN></P><P class=MsoNormal style=\"TEXT-ALIGN: justify; " +
        "MARGIN: 0cm 0cm 8pt\"><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; " +
        "LINE-HEIGHT: 107%'>I tasti MODIFICA ed ELIMINA sono riferiti ad ogni singola " +
        "comunicazione, mentre il tasto MODIFICA NOME MITTENTE/DESTINATARIO interviene sul " +
        "testo dellìetichetta/linguetta.<o:p></o:p></SPAN></P>";
      if (System.Windows.SystemParameters.PrimaryScreenWidth < 1100
        || System.Windows.SystemParameters.PrimaryScreenHeight < 600)
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
      if (nota != "" && nota != "<P align=left>&nbsp;</P>"
        && nota != "<P class=MsoNormal style=\"MARGIN: 0cm 0cm 0pt; LINE-HEIGHT: normal\" align=left>&nbsp;</P>")
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
    //                              Convert2RTFChar                               |
    //----------------------------------------------------------------------------+
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
        //case "?":
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

    //----------------------------------------------------------------------------+
    //                             Convert2RTFString                              |
    //----------------------------------------------------------------------------+
    public string Convert2RTFString(string buff, string replaceChar)
    {
      return buff.Replace(replaceChar, Convert2RTFChar(replaceChar));
    }

    //----------------------------------------------------------------------------+
    //                                Convert2RTF                                 |
    //----------------------------------------------------------------------------+
    private string Convert2RTF(string buff)
    {
      buff = buff.Replace("\\'", "\\#");
      buff = Convert2RTFString(buff, "'"); //va messo per primo o causa problemi
      buff = buff.Replace("\\#", "\\'");
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

    //----------------------------------------------------------------------------+
    //                              btn_Stampa_Click                              |
    //----------------------------------------------------------------------------+
    private void btn_Stampa_Click(object sender, RoutedEventArgs e)
    {
     
      string rtf_text = "";
      rtf_text += "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1040\\deflangfe1040\\deftab709";
      rtf_text += "{\\fonttbl{\\f0 Cambria}}";
      rtf_text += "{\\colortbl;\\red0\\green255\\blue255;\\red204\\green204\\blue204;\\red255\\green255\\blue255;\\red230\\green230\\blue230;}";
      rtf_text += "\\viewkind4\\uc1";
      rtf_text += "\\trowd\\cellx9900 \\fs28 \\qc " + Cliente + " \\line \\line \\cell\\row";
      rtf_text += "\\trowd\\cellx9900 \\fs24 \\qc " + lblTitolo.Content + " \\line \\line \\cell\\row";
      string xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato";
      foreach (XmlNode item in _x.Document.SelectNodes(xpath))
      {
        rtf_text += "\\trowd\\cellx9900 \\fs24 \\ql \\b " + item.Attributes["TABNAME"].Value + ": \\b0 \\line \\cell\\row";
        string xpathinternal = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" + item.Attributes["TAB"].Value + "]/Valore";
        string gruppo = "0";
        foreach (XmlNode items in _x.Document.SelectNodes(xpathinternal))
        {
          if (gruppo != items.Attributes["GRUPPO"].Value)
          {
            gruppo = items.Attributes["GRUPPO"].Value;
            rtf_text += "\\trowd\\clbrdrb\\brdrw10\\brdrs\\cellx9000  \\cell\\row";
            rtf_text += "\\trowd\\cellx9000  \\cell\\row";
          }
          rtf_text += "\\trowd\\cellx9900 \\qj \\i " + items.Attributes["IR"].Value + " il " + items.Attributes["DATA"].Value + " \\i0 \\cell\\row";
          rtf_text += "\\trowd\\cellx9900 \\qj " + items.Attributes["NOTE"].Value + " \\cell\\row";
          rtf_text += "\\trowd\\cellx9000  \\cell\\row";
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
      wrdDoc.SaveAs(filename + ".pdf", filename + ".rtf", "WdSaveFormat.wdFormatPDF");
      //MM
          


      FileInfo fi = new FileInfo(filename + ".rtf");
      fi.Delete();
      System.Diagnostics.Process process = new System.Diagnostics.Process();
      process.Refresh();
      process.StartInfo.FileName = filename + ".pdf";
      process.StartInfo.ErrorDialog = false;
      process.StartInfo.Verb = "open";
      process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
      process.Start();
  
    }
  } // class wFlussi
} // namespace RevisoftApplication

// srcOld
/*
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
using Microsoft.Office.Interop.Word;


namespace RevisoftApplication
{

    public partial class wFlussi : System.Windows.Window
    {
        private string _cliente = "";
        public int IDCliente = -1;
        private bool _ReadOnly = true;

        private XmlDataProviderManager _x = null;
        public RevisoftApplication.wSchedaSceltaFlussi.TipoFlusso tipo = new RevisoftApplication.wSchedaSceltaFlussi.TipoFlusso();

        Hashtable htTabs = new Hashtable();

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

        public wFlussi()
        {
            InitializeComponent();
        }

        public bool ReadOnly
        {
            set
            {
                _ReadOnly = value;
            }
        }

        public void GeneraTitolo()
        {
            ;
        }

        public void Load()
        {
            tabControlFlussi.Items.Clear();

            MasterFile mf = MasterFile.Create();

            if(mf.CheckDoppio_Flussi(IDCliente))
            {
                //setto dati
                Hashtable ht = new Hashtable();

                ht.Add( "Cliente", IDCliente );

                mf.SetFlussi( ht, IDCliente );
            }

            Hashtable htSelected = mf.GetFlussi( IDCliente.ToString() );
            _x = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + htSelected["FileData"].ToString());

            switch ( tipo )
            {
                case wSchedaSceltaFlussi.TipoFlusso.ISQC:
                    lblTitolo.Content = "Comunicazioni per controllo di qualità interno (ISQC)";
                    break;
                case wSchedaSceltaFlussi.TipoFlusso.Societa:
                    lblTitolo.Content = "Comunicazioni fra Organi della società";
                    break;
                case wSchedaSceltaFlussi.TipoFlusso.Gruppo:
                    lblTitolo.Content = "Comunicazioni fra organi della società del gruppo";
                    break;
                case wSchedaSceltaFlussi.TipoFlusso.Terzi:
                    lblTitolo.Content = "Comunicazioni con terzi";
                    break;
                default:
                    break;
            }            

            TabItem ti;

            string xpath = "//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]/Dato";

            foreach ( XmlNode item in _x.Document.SelectNodes( xpath ) )
            {
                string xpathinternal = "//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]/Dato[@TAB=" + item.Attributes["TAB"].Value + "]/Valore";

                foreach ( XmlNode items in item.SelectNodes( "Valore" ) )
                {
                    items.Attributes["ALLEGATI"].Value = ( ( items.HasChildNodes == true ) ? "PRESENTI" : "" );
                }                

                ucTabellaFlussi t = new ucTabellaFlussi();
                t.ReadOnly = _ReadOnly;
                t.Load( _x, xpathinternal );

                ti = new TabItem();
                ti.Header = item.Attributes["TABNAME"].Value;

                if (! htTabs.Contains(item.Attributes["TAB"].Value))
                {
                    htTabs.Add( item.Attributes["TAB"].Value, item.Attributes["TABNAME"].Value );
                }

                ti.Content = t;
                tabControlFlussi.Items.Add( ti );   
            }
        }
	
		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
		}

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tabControlFlussi.Focus();
        }

        private void btnNuova_Click( object sender, RoutedEventArgs e )
        {
            //if ( tabControlFlussi.SelectedItem == null || ( (UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent ).dtgMain.SelectedItem == null || ( (XmlNode)( ( (UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent ).dtgMain.SelectedItem ) ).Attributes["GRUPPO"] == null )
            {
                wSchedaImmissioneFlussi immissioneFLUSSI = new wSchedaImmissioneFlussi();
                immissioneFLUSSI.Owner = this;
                immissioneFLUSSI.tipo = tipo;
                immissioneFLUSSI.IDCliente = IDCliente;
                immissioneFLUSSI.IDComunicazione = -1;
                immissioneFLUSSI.IDGruppoComunicazione = -1;
                immissioneFLUSSI._x = _x;
                immissioneFLUSSI.Load();
                immissioneFLUSSI.ShowDialog();
            }
            //else
            //{
            //    MessageBox.Show( "Occorre non avere nessuna comunicazione selezionata" );
            //    return;
            //}
        }

        private void btnRisposta_Click( object sender, RoutedEventArgs e )
        {
            if ( tabControlFlussi.SelectedItem == null || ( (UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent ).dtgMain.SelectedItem == null || ( (XmlNode)( ( (UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent ).dtgMain.SelectedItem ) ).Attributes["GRUPPO"] == null )
            {
                MessageBox.Show( "Occorre selezionare una comunicazione per creare una risposta" );
                return;
            }

            int IDTabSelected = -1;

            if ( tabControlFlussi.SelectedItem == null )
            {
                return;
            }

            foreach ( DictionaryEntry item in htTabs )
	        {
                if ( item.Value.ToString() == ( (TabItem)( tabControlFlussi.SelectedItem ) ).Header.ToString() )
                {
                    IDTabSelected = Convert.ToInt32( item.Key.ToString() );
                }
	        }

            int IDSelected = -1;
            int IDGruppoComunicazioneSelected = Convert.ToInt32( ( (XmlNode)( ( (UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent ).dtgMain.SelectedItem ) ).Attributes["GRUPPO"].Value );
            string oldIR = ( (XmlNode)( ( (UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent ).dtgMain.SelectedItem ) ).Attributes["IR"].Value;
            
            wSchedaImmissioneFlussi immissioneFLUSSI = new wSchedaImmissioneFlussi();
            immissioneFLUSSI.Owner = this;
            immissioneFLUSSI.tipo = tipo;
            immissioneFLUSSI.IDCliente = IDCliente;
            immissioneFLUSSI.IDComunicazione = IDSelected;
            immissioneFLUSSI.IDGruppoComunicazione = IDGruppoComunicazioneSelected;
            immissioneFLUSSI.IDTab = IDTabSelected;
            immissioneFLUSSI.oldIR = oldIR;
            immissioneFLUSSI._x = _x;
            immissioneFLUSSI.Load();
            immissioneFLUSSI.ShowDialog();
        }

        private void btnModifica_Click( object sender, RoutedEventArgs e )
        {
            if ( tabControlFlussi.SelectedItem == null || ( (UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent ).dtgMain.SelectedItem == null || ( (XmlNode)( ( (UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent ).dtgMain.SelectedItem ) ).Attributes["ID"] == null )
            {
                MessageBox.Show( "Occorre selezionare una comunicazione per modificarla" );
                return;
            }

            int IDTabSelected = -1;

            if ( tabControlFlussi.SelectedItem == null )
            {
                return;
            }

            foreach ( DictionaryEntry item in htTabs )
            {
                if ( item.Value.ToString() == ( (TabItem)( tabControlFlussi.SelectedItem ) ).Header.ToString() )
                {
                    IDTabSelected = Convert.ToInt32( item.Key.ToString() );
                }
            }  

            int IDSelected = Convert.ToInt32( ( (XmlNode)( ( (UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent ).dtgMain.SelectedItem ) ).Attributes["ID"].Value );
            int IDGruppoComunicazioneSelected = Convert.ToInt32( ( (XmlNode)( ( (UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent ).dtgMain.SelectedItem ) ).Attributes["GRUPPO"].Value );

            wSchedaImmissioneFlussi immissioneFLUSSI = new wSchedaImmissioneFlussi();
            immissioneFLUSSI.Owner = this;
            immissioneFLUSSI.tipo = tipo;
            immissioneFLUSSI.IDCliente = IDCliente;
            immissioneFLUSSI.IDComunicazione = IDSelected;
            immissioneFLUSSI.IDGruppoComunicazione = IDGruppoComunicazioneSelected; 
            immissioneFLUSSI.IDTab = IDTabSelected;
            immissioneFLUSSI._x = _x;
            immissioneFLUSSI.Load();
            immissioneFLUSSI.ShowDialog();
        }

        private void btnElimina_Click( object sender, RoutedEventArgs e )
        {
            if ( tabControlFlussi.SelectedItem == null || ( (UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent ).dtgMain.SelectedItem == null || ( (XmlNode)( ( (UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent ).dtgMain.SelectedItem ) ).Attributes["ID"] == null )
            {
                MessageBox.Show( "Occorre selezionare una comunicazione per eliminarla" );
                return;
            }

            if ( MessageBox.Show( "Sicuri di voler eliminare la comunicazione?", "Attenzione", MessageBoxButton.YesNo ) == MessageBoxResult.No )
            {
                return;
            }

            int IDTabSelected = -1;

            if ( tabControlFlussi.SelectedItem == null )
            {
                return;
            }

            foreach ( DictionaryEntry item in htTabs )
            {
                if ( item.Value.ToString() == ( (TabItem)( tabControlFlussi.SelectedItem ) ).Header.ToString() )
                {
                    IDTabSelected = Convert.ToInt32( item.Key.ToString() );
                }
            }

            int IDSelected = Convert.ToInt32( ( (XmlNode)( ( (UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent ).dtgMain.SelectedItem ) ).Attributes["ID"].Value );
            int IDGruppoComunicazioneSelected = Convert.ToInt32( ( (XmlNode)( ( (UserControls.ucTabellaFlussi)tabControlFlussi.SelectedContent ).dtgMain.SelectedItem ) ).Attributes["GRUPPO"].Value );

            string xpath = "//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]/Dato[@TAB=" + IDTabSelected + "]/Valore[@GRUPPO=" + IDGruppoComunicazioneSelected + "][@ID=" + IDSelected + "]";
            XmlNode tmpNode = _x.Document.SelectSingleNode( xpath );

            if ( tmpNode.ParentNode.ChildNodes.Count > 1 )
            {
                if ( tmpNode != null )
                {
                    tmpNode.ParentNode.RemoveChild( tmpNode );
                }

                _x.Save();
                Load();

                foreach ( TabItem item in tabControlFlussi.Items )
                {
                    if ( item.Header.ToString() == htTabs[IDTabSelected.ToString()].ToString() )
                    {
                        tabControlFlussi.SelectedItem = item;
                    }
                }

                tabControlFlussi.Focus();
            }
            else
            {
                tmpNode.ParentNode.ParentNode.RemoveChild( tmpNode.ParentNode );

                _x.Save();
                Load();

                tabControlFlussi.Items.MoveCurrentToLast();
                tabControlFlussi.Focus();
            }            
        }

        private void btnModificaMD_Click( object sender, RoutedEventArgs e )
        {
            if ( tabControlFlussi.SelectedItem == null )
            {
                return;
            }

            var dialog = new wInputBox( "Inserire testo Modifica" );
            dialog.ResponseText = ( (TabItem)( tabControlFlussi.SelectedItem ) ).Header.ToString();
            dialog.ShowDialog();
            
            if ( dialog.ResponseText.Trim() == "" )
            {
                MessageBox.Show( "Nessun valore inserito" );
                return;
            }

            string titolo = dialog.ResponseText.Replace( "&", "&amp;" ).Replace( "\"", "'" );

            int IDTabSelected = -1;

            foreach ( DictionaryEntry item in htTabs )
            {
                if ( item.Value.ToString() == ( (TabItem)( tabControlFlussi.SelectedItem ) ).Header.ToString() )
                {
                    IDTabSelected = Convert.ToInt32( item.Key.ToString() );
                }
            }

            string xpath = "//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]/Dato[@TAB=" + IDTabSelected + "]";
            XmlNode tmpNode = _x.Document.SelectSingleNode( xpath );

            if ( tmpNode != null )
            {
                tmpNode.Attributes["TABNAME"].Value = titolo;
                htTabs[IDTabSelected.ToString()] = titolo;
            }

            _x.Save();
            Load();

            foreach ( TabItem item in tabControlFlussi.Items )
            {
                if ( item.Header.ToString() == htTabs[IDTabSelected.ToString()].ToString() )
                {
                    tabControlFlussi.SelectedItem = item;
                }
            }

            tabControlFlussi.Focus();
        }

        private void buttonChiudi_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void btn_GuidaRevisoft_Click( object sender, RoutedEventArgs e )
        {
            GuidaRevisoft( true );
        }

        private void GuidaRevisoft( bool posizioneMouse )
        {
            wGuidaRevisoft w = new wGuidaRevisoft();
            w.Owner = this;

            string nota = "<P class=MsoNormal style=\"TEXT-ALIGN: justify; MARGIN: 0cm 0cm 8pt\"><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%'>Questo modulo si prefigge di correlare lo scambio di comunicazioni ed informazioni degli organi della società fra di loro ed i terzi.<?xml:namespace prefix = \"o\" ns = \"urn:schemas-microsoft-com:office:office\" /><o:p></o:p></SPAN></P><P class=MsoNormal style=\"TEXT-ALIGN: justify; MARGIN: 0cm 0cm 8pt\"><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%'>Il modulo è diviso in quattro aree destinate agli scambi fra appartenenti alla struttura del revisore, agli organi della società soggetta a revisione, del gruppo al quale appartiene e da terzi.<o:p></o:p></SPAN></P><P class=MsoNormal style=\"TEXT-ALIGN: justify; MARGIN: 0cm 0cm 8pt\"><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%'>Revisoft creerà, per ogni mittente/destinatario – identificato con una etichetta – una tabella nella quale ogni riga indicherà una comunicazione son il soggetto indicato.<o:p></o:p></SPAN></P><P class=MsoNormal style=\"TEXT-ALIGN: justify; MARGIN: 0cm 0cm 8pt\"><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%'>Ogni riga evidenzierà se la comunicazione è stata inviata o ricevuta, la data, un campo per le annotazioni ed il simbolo della presenza di allegati.<o:p></o:p></SPAN></P><P class=MsoNormal style=\"TEXT-ALIGN: justify; MARGIN: 0cm 0cm 8pt\"><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%'>Accedendo ad un’area con il tasto APRI FLUSSI e dopo aver scelto il cliente, Revisoft apre una finestra che:<o:p></o:p></SPAN></P><P class=MsoListParagraphCxSpFirst style=\"TEXT-ALIGN: justify; MARGIN: 0cm 0cm 0pt 21.3pt; TEXT-INDENT: -18pt; mso-add-space: auto; mso-list: l0 level1 lfo1\"><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%; mso-fareast-font-family: Cambria; mso-bidi-font-family: Cambria'><SPAN style=\"mso-list: Ignore\">-<SPAN style='FONT: 7pt \"Times New Roman\"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </SPAN></SPAN></SPAN><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%'>Sulla barra di sinistra espone i tasti di funzione per creare una nuova comunicazione, rispondere, modificarla ed eliminarla. E’ anche presente un tasto per modificare l’etichetta del soggetto col quale avviene lo scambio.<o:p></o:p></SPAN></P><P class=MsoListParagraphCxSpMiddle style=\"TEXT-ALIGN: justify; MARGIN: 0cm 0cm 0pt 21.3pt; TEXT-INDENT: -18pt; mso-add-space: auto; mso-list: l0 level1 lfo1\"><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%; mso-fareast-font-family: Cambria; mso-bidi-font-family: Cambria'><SPAN style=\"mso-list: Ignore\">-<SPAN style='FONT: 7pt \"Times New Roman\"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </SPAN></SPAN></SPAN><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%'>Con il tasto NUOVA si apre una finestra per l’immissione dei dati che verranno esposti nella riga della comunicazione e per caricare gli allegati.<o:p></o:p></SPAN></P><P class=MsoListParagraphCxSpMiddle style=\"TEXT-ALIGN: justify; MARGIN: 0cm 0cm 0pt 21.3pt; TEXT-INDENT: -18pt; mso-add-space: auto; mso-list: l0 level1 lfo1\"><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%; mso-fareast-font-family: Cambria; mso-bidi-font-family: Cambria'><SPAN style=\"mso-list: Ignore\">-<SPAN style='FONT: 7pt \"Times New Roman\"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </SPAN></SPAN></SPAN><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%'>Per collegare una comunicazione con un’altra già presente occorre SELEZIONARE la riga della comunicazione presente ed usare il tasto RISPOSTA (si intende risposta sia al mittente di una comunicazione ricevuta, che del destinatario di una nostra comunicazione).<o:p></o:p></SPAN></P><P class=MsoListParagraphCxSpLast style=\"TEXT-ALIGN: justify; MARGIN: 0cm 0cm 8pt 21.3pt; TEXT-INDENT: -18pt; mso-add-space: auto; mso-list: l0 level1 lfo1\"><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%; mso-fareast-font-family: Cambria; mso-bidi-font-family: Cambria'><SPAN style=\"mso-list: Ignore\">-<SPAN style='FONT: 7pt \"Times New Roman\"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </SPAN></SPAN></SPAN><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%'>Le varie comunicazioni verranno collegate fra di loro e le righe che le rappresentano avranno lo stesso colore di fondo, che sarà diverso da quelle non collegate o che sono correlate in altri raggruppamenti.<o:p></o:p></SPAN></P><P class=MsoNormal style=\"TEXT-ALIGN: justify; MARGIN: 0cm 0cm 8pt\"><SPAN style='FONT-SIZE: 12pt; FONT-FAMILY: \"Cambria\",serif; LINE-HEIGHT: 107%'>I tasti MODIFICA ed ELIMINA sono riferiti ad ogni singola comunicazione, mentre il tasto MODIFICA NOME MITTENTE/DESTINATARIO interviene sul testo dellìetichetta/linguetta.<o:p></o:p></SPAN></P>";
           
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

            w.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

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

        public string Convert2RTFChar( string carattere )
        {
            string newChar = "";

            switch ( carattere )
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

        public string Convert2RTFString( string buff, string replaceChar )
        {
            return buff.Replace( replaceChar, Convert2RTFChar( replaceChar ) );
        }

        private string Convert2RTF( string buff )
        {
            buff = buff.Replace( "\\'", "\\#" );
            buff = Convert2RTFString( buff, "'" ); //va messo per primo o causa problemi
            buff = buff.Replace( "\\#", "\\'" );

            //for (char c = '!'; c <= 'ÿ'; c++)
            //{
            //    buff = Convert2RTFString(buff, c.ToString() );
            //}

            buff = Convert2RTFString( buff, "%" );
            buff = Convert2RTFString( buff, "ì" );
            buff = Convert2RTFString( buff, "è" );
            buff = Convert2RTFString( buff, "é" );
            buff = Convert2RTFString( buff, "ò" );
            buff = Convert2RTFString( buff, "à" );
            buff = Convert2RTFString( buff, "ù" );
            buff = Convert2RTFString( buff, "°" );
            buff = Convert2RTFString( buff, "€" );
            buff = Convert2RTFString( buff, "\"" );
            buff = Convert2RTFString( buff, "’" );
            buff = Convert2RTFString( buff, "”" );
            buff = Convert2RTFString( buff, "“" );

            return buff;
        }

        private void btn_Stampa_Click( object sender, RoutedEventArgs e )
        {
            //Process wait - START
            ProgressWindow pw = new ProgressWindow();

            string rtf_text = "";
            rtf_text += "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1040\\deflangfe1040\\deftab709";
            rtf_text += "{\\fonttbl{\\f0 Cambria}}";
            rtf_text += "{\\colortbl;\\red0\\green255\\blue255;\\red204\\green204\\blue204;\\red255\\green255\\blue255;\\red230\\green230\\blue230;}";
            rtf_text += "\\viewkind4\\uc1";

            rtf_text += "\\trowd\\cellx9900 \\fs28 \\qc " + Cliente + " \\line \\line \\cell\\row";

            rtf_text += "\\trowd\\cellx9900 \\fs24 \\qc " + lblTitolo.Content + " \\line \\line \\cell\\row";
            
            string xpath = "//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]/Dato";

            foreach ( XmlNode item in _x.Document.SelectNodes( xpath ) )
            {
                rtf_text += "\\trowd\\cellx9900 \\fs24 \\ql \\b " + item.Attributes["TABNAME"].Value + ": \\b0 \\line \\cell\\row";

                string xpathinternal = "//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]/Dato[@TAB=" + item.Attributes["TAB"].Value + "]/Valore";

                string gruppo = "0";

                foreach ( XmlNode items in _x.Document.SelectNodes( xpathinternal ) )
                {
                    if ( gruppo != items.Attributes["GRUPPO"].Value )
                    {
                        gruppo = items.Attributes["GRUPPO"].Value;
                        rtf_text += "\\trowd\\clbrdrb\\brdrw10\\brdrs\\cellx9000  \\cell\\row";
                        rtf_text += "\\trowd\\cellx9000  \\cell\\row";
                    }

                    rtf_text += "\\trowd\\cellx9900 \\qj \\i " + items.Attributes["IR"].Value + " il " + items.Attributes["DATA"].Value + " \\i0 \\cell\\row";
                    rtf_text += "\\trowd\\cellx9900 \\qj " + items.Attributes["NOTE"].Value + " \\cell\\row";
    
                    rtf_text += "\\trowd\\cellx9000  \\cell\\row";
                }
            }

            rtf_text += "}";

            rtf_text = Convert2RTF( rtf_text );

            string filename = App.AppTempFolder + Guid.NewGuid().ToString();

            TextWriter tw = new StreamWriter( filename + ".rtf" );
            tw.Write( rtf_text );
            tw.Close();

            Microsoft.Office.Interop.Word.Application wrdApp;
            _Document wrdDoc;
            Object oMissing = System.Reflection.Missing.Value;
            Object oFalse = false;

            wrdApp = new Microsoft.Office.Interop.Word.Application();
            wrdApp.Visible = false;

            wrdDoc = wrdApp.Documents.Open( filename + ".rtf" );

            object fileFormat = WdSaveFormat.wdFormatPDF;

            wrdDoc.PageSetup.PaperSize = WdPaperSize.wdPaperA4;
            wrdDoc.SaveAs( filename + ".pdf", fileFormat );

            wrdDoc.Close( ref oFalse, ref oMissing, ref oMissing );
            wrdApp.Quit();

            FileInfo fi = new FileInfo( filename + ".rtf" );
            fi.Delete();

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.Refresh();
            process.StartInfo.FileName = filename + ".pdf";
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.Verb = "open";
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            process.Start();

            //System.Diagnostics.Process.Start( filename + ".pdf" );

            //Process wait - STOP
            pw.Close();
        }
    }
}
*/
