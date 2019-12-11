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
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;

namespace RevisoftApplication
{
  public partial class wSchedaImmissioneFlussi : Window
  {
    public RevisoftApplication.wSchedaSceltaFlussi.TipoFlusso tipo =
      new RevisoftApplication.wSchedaSceltaFlussi.TipoFlusso();
    public int IDComunicazione = -1;
    public int IDGruppoComunicazione = -1;
    public int IDTab = -1;
    public int IDCliente = -1;
    private int newIDTab = -1;
    public string oldIR = "";
    private Hashtable htMD = new Hashtable();
    private Hashtable htAllegati = new Hashtable();
    public XmlDataProviderManager _x;

    //----------------------------------------------------------------------------+
    //                          wSchedaImmissioneFlussi                           |
    //----------------------------------------------------------------------------+
    public wSchedaImmissioneFlussi()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
    }

    //----------------------------------------------------------------------------+
    //                                    Load                                    |
    //----------------------------------------------------------------------------+
    public void Load()
    {
      cmbMD.Items.Clear();
      htMD.Clear();
      htAllegati.Clear();
      lstAllegati.Items.Clear();
      string xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato";
      foreach (XmlNode item in _x.Document.SelectNodes(xpath))
      {
        cmbMD.Items.Add(item.Attributes["TABNAME"].Value.ToString());
        if (IDTab == Convert.ToInt32(item.Attributes["TAB"].Value.ToString()))
        {
          cmbMD.SelectedValue = item.Attributes["TABNAME"].Value.ToString();
          lblNuovo.Visibility = System.Windows.Visibility.Collapsed;
          txtMD.Visibility = System.Windows.Visibility.Collapsed;
        }
        if (!htMD.Contains(item.Attributes["TAB"].Value.ToString()))
        {
          htMD.Add(item.Attributes["TAB"].Value.ToString(), item.Attributes["TABNAME"].Value.ToString());
        }
        if (Convert.ToInt32(item.Attributes["TAB"].Value.ToString()) > newIDTab)
        {
          newIDTab = Convert.ToInt32(item.Attributes["TAB"].Value.ToString());
        }
      }
      newIDTab++;
      if (oldIR != "")
      {
        if (oldIR == "inviata")
        {
          rdbInviata.IsChecked = false;
          rdbRicevuta.IsChecked = true;
        }
        else
        {
          rdbInviata.IsChecked = true;
          rdbRicevuta.IsChecked = false;
        }
      }
      xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" +
        IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "][@ID=" +
        IDComunicazione + "]";
      XmlNode tmpNode = _x.Document.SelectSingleNode(xpath);
      if (tmpNode != null)
      {
        if (tmpNode.Attributes["IR"].Value.ToString() == "inviata")
        {
          rdbInviata.IsChecked = true;
          rdbRicevuta.IsChecked = false;
        }
        else
        {
          rdbInviata.IsChecked = false;
          rdbRicevuta.IsChecked = true;
        }
        txtNota.Text = tmpNode.Attributes["NOTE"].Value.ToString();
        dtpData.SelectedDate = Convert.ToDateTime(tmpNode.Attributes["DATA"].Value.ToString());
        cmbMD.IsEnabled = false;
        xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" +
          IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "][@ID=" +
          IDComunicazione + "]/Allegato";
        foreach (XmlNode item in _x.Document.SelectNodes(xpath))
        {
          htAllegati.Add(item.Attributes["TITOLO"].Value.ToString(), item.Attributes["FILE"].Value.ToString());
          ListBoxItem itm = new ListBoxItem();
          itm.Content = item.Attributes["TITOLO"].Value.ToString();
          lstAllegati.Items.Add(itm);
        }
      }
    }

    //----------------------------------------------------------------------------+
    //                         RadioButtonInviata_Checked                         |
    //----------------------------------------------------------------------------+
    private void RadioButtonInviata_Checked(object sender, RoutedEventArgs e)
    {
      labelMittenteDestinatario.Content = "Destinatario";
    }

    //----------------------------------------------------------------------------+
    //                        RadioButtonRicevuta_Checked                         |
    //----------------------------------------------------------------------------+
    private void RadioButtonRicevuta_Checked(object sender, RoutedEventArgs e)
    {
      labelMittenteDestinatario.Content = "Mittente";
    }

    //----------------------------------------------------------------------------+
    //                           buttonCompletato_Click                           |
    //----------------------------------------------------------------------------+
    private void buttonCompletato_Click_old(object sender, RoutedEventArgs e)
    {
      string xpath = "";
      string xml = "";
      XmlNode tmpNodeFather = null;
      XmlDocument doctmp = null;
      XmlNode tmpNodeToBeImported = null;
      XmlNode tmpNodeNew = null;
      if (dtpData.SelectedDate == null)
      {
        MessageBox.Show("Inserire una data");
        return;
      }
      if (rdbInviata.IsChecked == false && rdbRicevuta.IsChecked == false)
      {
        MessageBox.Show("Selezionare una voce tra Inviata e Ricevuta");
        return;
      }
      if (IDComunicazione == -1) // Nuova Comunicazione
      {
        if (IDGruppoComunicazione == -1) // Nuovo Gruppo Comunicazioni
        {
          if (IDTab == -1) // Nuovo Tab
          {
            if (txtMD.Text != "") // Nuovo Mittente/destinatario
            {
              xml = "<Dato TAB=\"" + newIDTab + "\" TABNAME=\"" +
                txtMD.Text.Replace("&", "&amp;").Replace("\"", "'") + "\"/>";
              IDTab = newIDTab;
              htMD.Add(newIDTab.ToString(), txtMD.Text.Replace("&", "&amp;").Replace("\"", "'"));
              newIDTab++;
              xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]";
              tmpNodeFather = _x.Document.SelectSingleNode(xpath);
              doctmp = new XmlDocument();
              doctmp.LoadXml(xml);
              tmpNodeToBeImported = doctmp.SelectSingleNode("/Dato");
              tmpNodeNew = _x.Document.ImportNode(tmpNodeToBeImported, true);
              tmpNodeFather.AppendChild(tmpNodeNew);
            }
            else
            {
              MessageBox.Show("Inserire il Mittente/Destinatario");
              return;
            }
          }
          xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" + IDTab + "]/Valore";
          foreach (XmlNode item in _x.Document.SelectNodes(xpath))
          {
            if (Convert.ToInt32(item.Attributes["GRUPPO"].Value.ToString()) > IDGruppoComunicazione)
            {
              IDGruppoComunicazione = Convert.ToInt32(item.Attributes["GRUPPO"].Value.ToString());
            }
          }
          IDGruppoComunicazione++;
        }
        xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" +
          IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "]";
        foreach (XmlNode item in _x.Document.SelectNodes(xpath))
        {
          if (Convert.ToInt32(item.Attributes["ID"].Value.ToString()) > IDComunicazione)
          {
            IDComunicazione = Convert.ToInt32(item.Attributes["ID"].Value.ToString());
          }
        }
        IDComunicazione++;
      }
      xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" +
        IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "][@ID=" +
        IDComunicazione + "]";
      tmpNodeFather = _x.Document.SelectSingleNode(xpath);
      if (tmpNodeFather == null)
      {
        xml = "<Valore ID=\"" + IDComunicazione + "\" GRUPPO=\"" +
          IDGruppoComunicazione + "\" GRUPPOCOLOR=\"" +
          ((IDGruppoComunicazione % 2 == 0) ? "AliceBlue" : "LightYellow") +
          "\" IR=\"" + ((rdbInviata.IsChecked == true) ? "inviata" : "ricevuta") +
          "\" DATA=\"" + dtpData.SelectedDate.Value.ToShortDateString() +
          "\"  NOTE=\"" +
          txtNota.Text.Replace("&", "&amp;").Replace("\"", "'").Replace("<", "&lt;").Replace(">", "&gt;") +
          "\" ALLEGATI=\"\"/>";
        doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        tmpNodeToBeImported = doctmp.SelectSingleNode("/Valore");
        tmpNodeNew = _x.Document.ImportNode(tmpNodeToBeImported, true);
        xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" + IDTab + "]";
        tmpNodeFather = _x.Document.SelectSingleNode(xpath);
        XmlNode LastNode = null;
        foreach (XmlNode item in _x.Document.SelectNodes(
          "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" + IDTab +
          "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "]"))
        {
          LastNode = item;
        }
        if (LastNode != null)
        {
          tmpNodeFather.InsertAfter(tmpNodeNew, LastNode);
        }
        else
        {
          tmpNodeFather.AppendChild(tmpNodeNew);
        }
      }
      else
      {
        tmpNodeFather.Attributes["IR"].Value = ((rdbInviata.IsChecked == true) ? "inviata" : "ricevuta");
        tmpNodeFather.Attributes["DATA"].Value = dtpData.SelectedDate.Value.ToShortDateString();
        tmpNodeFather.Attributes["NOTE"].Value = txtNota.Text.Replace("&", "&amp;").Replace("\"", "'");
        tmpNodeFather.Attributes["ALLEGATI"].Value = ((lstAllegati.Items.Count > 0) ? "PRESENTI" : "");
      }
      _x.Save();
    }
    private void buttonCompletato_Click(object sender, RoutedEventArgs e)
    {
#if (!DBG_TEST)
      buttonCompletato_Click_old(sender, e);return;
#endif
      string guid = "";
      string xpath = "";
      string xml = "";
      XmlNode tmpNodeFather = null;
      XmlDocument doctmp = null;
      XmlNode tmpNodeToBeImported = null;
      XmlNode tmpNodeNew = null;
      if (dtpData.SelectedDate == null)
      {
        MessageBox.Show("Inserire una data");
        return;
      }
      if (rdbInviata.IsChecked == false && rdbRicevuta.IsChecked == false)
      {
        MessageBox.Show("Selezionare una voce tra Inviata e Ricevuta");
        return;
      }
      MasterFile mf = MasterFile.Create();
      Hashtable htSelected = mf.GetFlussi(IDCliente.ToString());
      guid = htSelected["FileData"].ToString();
      if (IDComunicazione == -1) // Nuova Comunicazione
      {
        if (IDGruppoComunicazione == -1) // Nuovo Gruppo Comunicazioni
        {
          if (IDTab == -1) // Nuovo Tab
          {
            if (txtMD.Text != "") // Nuovo Mittente/destinatario
            {
              xml = "<Dato TAB=\"" + newIDTab + "\" TABNAME=\"" +
                txtMD.Text.Replace("&", "&amp;").Replace("\"", "'") + "\"/>";
              IDTab = newIDTab;
              htMD.Add(newIDTab.ToString(), txtMD.Text.Replace("&", "&amp;").Replace("\"", "'"));
              newIDTab++;
              xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]";
              tmpNodeFather = _x.Document.SelectSingleNode(xpath);
              doctmp = new XmlDocument();
              doctmp.LoadXml(xml);
              tmpNodeToBeImported = doctmp.SelectSingleNode("/Dato");
              tmpNodeNew = _x.Document.ImportNode(tmpNodeToBeImported, true);
              tmpNodeFather.AppendChild(tmpNodeNew);
              using (System.Data.SqlClient.SqlConnection conn = new SqlConnection(App.connString))
              {
                conn.Open();
                SqlCommand cmd = new SqlCommand("flussi.NewDato", conn);
                cmd.Parameters.AddWithValue("@ID", IDCliente.ToString());
                cmd.Parameters.AddWithValue("@TIPO", Convert.ToInt32(tipo));
                cmd.Parameters.AddWithValue("@rec", doctmp.OuterXml);
                cmd.Parameters.AddWithValue("@guid", guid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = App.m_CommandTimeout;
                try { cmd.ExecuteNonQuery(); }
                catch (Exception ex)
                {
                  cBusinessObjects.logger.Error(ex, "wSchedaImmissioneFlussi.buttonCompletato_Click1 exception");
                  if (!App.m_bNoExceptionMsg)
                  {
                    string msg = "SQL call 'flussi.NewDato' failed: errore\n" + ex.Message;
                    MessageBox.Show(msg);
                  }
                }
              }
              if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
              if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
              if (App.m_xmlCache.Contains(guid)) App.m_xmlCache.Remove(guid);
            }
            else
            {
              MessageBox.Show("Inserire il Mittente/Destinatario");
              return;
            }
          }
          xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" + IDTab + "]/Valore";
          foreach (XmlNode item in _x.Document.SelectNodes(xpath))
          {
            if (Convert.ToInt32(item.Attributes["GRUPPO"].Value.ToString()) > IDGruppoComunicazione)
            {
              IDGruppoComunicazione = Convert.ToInt32(item.Attributes["GRUPPO"].Value.ToString());
            }
          }
          IDGruppoComunicazione++;
        }
        xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" +
          IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "]";
        foreach (XmlNode item in _x.Document.SelectNodes(xpath))
        {
          if (Convert.ToInt32(item.Attributes["ID"].Value.ToString()) > IDComunicazione)
          {
            IDComunicazione = Convert.ToInt32(item.Attributes["ID"].Value.ToString());
          }
        }
        IDComunicazione++;
      }
      xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" +
        IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "][@ID=" +
        IDComunicazione + "]";
      tmpNodeFather = _x.Document.SelectSingleNode(xpath);
      if (tmpNodeFather == null)
      {
        xml = "<Valore ID=\"" + IDComunicazione + "\" GRUPPO=\"" +
          IDGruppoComunicazione + "\" GRUPPOCOLOR=\"" +
          ((IDGruppoComunicazione % 2 == 0) ? "AliceBlue" : "LightYellow") +
          "\" IR=\"" + ((rdbInviata.IsChecked == true) ? "inviata" : "ricevuta") +
          "\" DATA=\"" + dtpData.SelectedDate.Value.ToShortDateString() +
          "\"  NOTE=\"" +
          txtNota.Text.Replace("&", "&amp;").Replace("\"", "'").Replace("<", "&lt;").Replace(">", "&gt;") +
          "\" ALLEGATI=\"\"/>";
        doctmp = new XmlDocument();
        doctmp.LoadXml(xml);

        using (System.Data.SqlClient.SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("flussi.NewValore", conn);
          cmd.Parameters.AddWithValue("@ID", IDCliente.ToString());
          cmd.Parameters.AddWithValue("@TIPO", Convert.ToInt32(tipo));
          cmd.Parameters.AddWithValue("@TAB", IDTab);
          cmd.Parameters.AddWithValue("@rec", doctmp.OuterXml);
          cmd.Parameters.AddWithValue("@guid", guid);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            cBusinessObjects.logger.Error(ex, "wSchedaImmissioneFlussi.buttonCompletato_Click2 exception");
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SQL call 'flussi.NewValore' failed: errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
        if (App.m_xmlCache.Contains(guid)) App.m_xmlCache.Remove(guid);
      }
      else
      {
        tmpNodeFather.Attributes["IR"].Value = ((rdbInviata.IsChecked == true) ? "inviata" : "ricevuta");
        tmpNodeFather.Attributes["DATA"].Value = dtpData.SelectedDate.Value.ToShortDateString();
        tmpNodeFather.Attributes["NOTE"].Value = txtNota.Text.Replace("&", "&amp;").Replace("\"", "'");
        tmpNodeFather.Attributes["ALLEGATI"].Value = ((lstAllegati.Items.Count > 0) ? "PRESENTI" : "");
        using (System.Data.SqlClient.SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("flussi.UpdateValore", conn);
          cmd.Parameters.AddWithValue("@ID", IDCliente.ToString());
          cmd.Parameters.AddWithValue("@TIPO", Convert.ToInt32(tipo));
          cmd.Parameters.AddWithValue("@TAB", IDTab);
          cmd.Parameters.AddWithValue("@rec", tmpNodeFather.OuterXml);
          cmd.Parameters.AddWithValue("@guid", guid);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            cBusinessObjects.logger.Error(ex, "wSchedaImmissioneFlussi.buttonCompletato_Click3 exception");
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SQL call 'flussi.UpdateValore' failed: errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
        if (App.m_xmlCache.Contains(guid)) App.m_xmlCache.Remove(guid);
      }
      _x.Load();
    }

    //----------------------------------------------------------------------------+
    //                             buttonChiudi_Click                             |
    //----------------------------------------------------------------------------+
    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      if (htMD.Contains(IDTab.ToString()))
      {
        ((wFlussi)Owner).Load();
        foreach (TabItem item in ((wFlussi)Owner).tabControlFlussi.Items)
        {
          if (item.Header.ToString() == htMD[IDTab.ToString()].ToString())
          {
            ((wFlussi)Owner).tabControlFlussi.SelectedItem = item;
          }
        }
        ((wFlussi)Owner).tabControlFlussi.Focus();
      }
      this.Close();
    }

    //----------------------------------------------------------------------------+
    //                           cmbMD_SelectionChanged                           |
    //----------------------------------------------------------------------------+
    private void cmbMD_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      lblNuovo.Visibility = System.Windows.Visibility.Visible;
      txtMD.Visibility = System.Windows.Visibility.Visible;
      foreach (DictionaryEntry item in htMD)
      {
        if (item.Value == cmbMD.SelectedItem)
        {
          IDTab = Convert.ToInt32(item.Key.ToString());
          lblNuovo.Visibility = System.Windows.Visibility.Collapsed;
          txtMD.Visibility = System.Windows.Visibility.Collapsed;
          return;
        }
      }
    }

    public string InitialDirectory = "";

    private string dialogSaveFile()
    {
      string file = "";
      string newName = "";
      Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
      dlg.InitialDirectory = InitialDirectory;
      if (dlg.ShowDialog() == true)
      {
        FileInfo fi = new FileInfo(dlg.FileName);
        if (fi.Exists)
        {
          string pathnew = fi.FullName.Replace(fi.FullName.Split('\\').Last(), "");
          DirectoryInfo dinew = new DirectoryInfo(pathnew);
          if (dinew.Exists)
          {
            InitialDirectory = pathnew;
          }
          string directory = App.AppDocumentiFolder + "\\Flussi";
          DirectoryInfo dtest = new DirectoryInfo(directory);
          if (dtest.Exists == false)
          {
            dtest.Create();
          }
          int IDHere = -1;
          foreach (FileInfo item in dtest.GetFiles())
          {
            if (IDHere < Convert.ToInt32(item.Name.Replace(item.Extension, "")))
            {
              IDHere = Convert.ToInt32(item.Name.Replace(item.Extension, ""));
            }
          }
          IDHere++;
          newName = IDHere + "." + dlg.FileName.Split('.').Last();
          file = directory + "\\" + newName;
          FileInfo fitmp = new FileInfo(file);
          if (fitmp.Exists)
          {
            fitmp.Delete();
          }
          fi.IsReadOnly = false;
          fi.CopyTo(file);
        }
      }
      return file;
    }

    //----------------------------------------------------------------------------+
    //                        buttonAggiungiAllegato_Click                        |
    //----------------------------------------------------------------------------+
    private void buttonAggiungiAllegato_Click_old(object sender, RoutedEventArgs e)
    {
      if (IDGruppoComunicazione == -1 || IDComunicazione == -1 || IDTab == -1)
      {
        MessageBox.Show("Impossibile aggiungere allegati, senza prima aver creato il Flusso.");
        return;
      }
      var dialog = new wInputBox("Inserire Titolo del File");
      dialog.ResponseText = "";
      dialog.ShowDialog();
      string titolo = dialog.ResponseText.Replace("&", "&amp;").Replace("\"", "'");
      if (titolo == "") return;
      string xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" +
        IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "][@ID=" +
        IDComunicazione + "]/Allegato[@TITOLO=\"" + titolo + "\"]";
      XmlNode tmpNode = _x.Document.SelectSingleNode(xpath);
      if (tmpNode != null)
      {
        MessageBox.Show("Titolo già presente");
        return;
      }
      string file = dialogSaveFile();
      if (file == "" || file == null) return;
      string xml = "<Allegato ID=\"" + newIDTab + "\" FILE=\"" +
        file.Split('\\').Last() + "\" TITOLO=\"" + titolo + "\"/>";
      htAllegati.Add(titolo, file);
      xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" +
        IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "][@ID=" +
        IDComunicazione + "]";
      XmlNode tmpNodeFather = _x.Document.SelectSingleNode(xpath);
      XmlDocument doctmp = new XmlDocument();
      doctmp.LoadXml(xml);
      XmlNode tmpNodeToBeImported = doctmp.SelectSingleNode("/Allegato");
      XmlNode tmpNodeNew = _x.Document.ImportNode(tmpNodeToBeImported, true);
      tmpNodeFather.AppendChild(tmpNodeNew);
      _x.Save();
      Load();
    }
    private void buttonAggiungiAllegato_Click(object sender, RoutedEventArgs e)
    {
#if (!DBG_TEST)
      buttonAggiungiAllegato_Click_old(sender, e);return;
#endif
      if (IDGruppoComunicazione == -1 || IDComunicazione == -1 || IDTab == -1)
      {
        MessageBox.Show("Impossibile aggiungere allegati, senza prima aver creato il Flusso.");
        return;
      }
      var dialog = new wInputBox("Inserire Titolo del File");
      dialog.ResponseText = "";
      dialog.ShowDialog();
      string titolo = dialog.ResponseText.Replace("&", "&amp;").Replace("\"", "'");
      if (titolo == "") return;
      string xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" +
        IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "][@ID=" +
        IDComunicazione + "]/Allegato[@TITOLO=\"" + titolo + "\"]";
      XmlNode tmpNode = _x.Document.SelectSingleNode(xpath);
      if (tmpNode != null)
      {
        MessageBox.Show("Titolo già presente");
        return;
      }
      string file = dialogSaveFile();
      if (file == "" || file == null) return;
      string xml = "<Allegato ID=\"" + newIDTab + "\" FILE=\"" +
        file.Split('\\').Last() + "\" TITOLO=\"" + titolo + "\"/>";
      htAllegati.Add(titolo, file);
      XmlDocument doctmp = new XmlDocument();
      doctmp.LoadXml(xml);

      string guid;
      MasterFile mf = MasterFile.Create();
      Hashtable htSelected = mf.GetFlussi(IDCliente.ToString());
      guid = htSelected["FileData"].ToString();
      using (System.Data.SqlClient.SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("flussi.NewAllegato", conn);
        cmd.Parameters.AddWithValue("@IDCliente", IDCliente.ToString());
        cmd.Parameters.AddWithValue("@TIPO", Convert.ToInt32(tipo));
        cmd.Parameters.AddWithValue("@TAB", IDTab);
        cmd.Parameters.AddWithValue("@GRUPPO", IDGruppoComunicazione.ToString());
        cmd.Parameters.AddWithValue("@ID", IDComunicazione.ToString());
        cmd.Parameters.AddWithValue("@rec", doctmp.OuterXml);
        cmd.Parameters.AddWithValue("@guid", guid);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wSchedaImmissioneFlussi.buttonAggiungiAllegato_Click exception");
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "SQL call 'flussi.NewAllegato' failed: errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
      if (App.m_xmlCache.Contains(guid)) App.m_xmlCache.Remove(guid);
      _x.Load();
      Load();
    }

    //----------------------------------------------------------------------------+
    //                        buttonEliminaAllegato_Click                         |
    //----------------------------------------------------------------------------+
    private void buttonEliminaAllegato_Click_old(object sender, RoutedEventArgs e)
    {
      if (lstAllegati.SelectedIndex == -1)
      {
        MessageBox.Show("Selezionare un allegato");
        return;
      }
      if (MessageBox.Show("Sicuri di voler eliminare l'allegato?",
        "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
      string xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" +
        IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "][@ID=" +
        IDComunicazione + "]/Allegato[@FILE=\"" +
        htAllegati[((ListBoxItem)(lstAllegati.SelectedValue)).Content.ToString()].ToString() + "\"]";
      XmlNode tmpNode = _x.Document.SelectSingleNode(xpath);
      if (tmpNode != null)
      {
        tmpNode.ParentNode.RemoveChild(tmpNode);
      }
      string directory = "";
      directory = App.AppDocumentiFolder + "\\Flussi";
      string file = directory + "\\" + htAllegati[((ListBoxItem)(lstAllegati.SelectedValue)).Content.ToString()].ToString();
      FileInfo fi = new FileInfo(file);
      if (fi.Exists)
      {
        fi.Delete();
      }
      _x.Save();
      Load();
    }
    private void buttonEliminaAllegato_Click(object sender, RoutedEventArgs e)
    {
#if (!DBG_TEST)
      buttonEliminaAllegato_Click_old(sender, e);return;
#endif
      if (lstAllegati.SelectedIndex == -1)
      {
        MessageBox.Show("Selezionare un allegato");
        return;
      }
      if (MessageBox.Show("Sicuri di voler eliminare l'allegato?",
        "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
      string xpath = "//Dati[@TIPO=" + Convert.ToInt32(tipo) + "]/Dato[@TAB=" +
        IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "][@ID=" +
        IDComunicazione + "]/Allegato[@FILE=\"" +
        htAllegati[((ListBoxItem)(lstAllegati.SelectedValue)).Content.ToString()].ToString() + "\"]";
      XmlNode tmpNode = _x.Document.SelectSingleNode(xpath);
      if (tmpNode != null)
      {
        string guid;
        MasterFile mf = MasterFile.Create();
        Hashtable htSelected = mf.GetFlussi(IDCliente.ToString());
        guid = htSelected["FileData"].ToString();
        using (System.Data.SqlClient.SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("flussi.EliminaAllegato", conn);
          cmd.Parameters.AddWithValue("@IDCliente", IDCliente.ToString());
          cmd.Parameters.AddWithValue("@TIPO", Convert.ToInt32(tipo));
          cmd.Parameters.AddWithValue("@TAB", IDTab);
          cmd.Parameters.AddWithValue("@GRUPPO", IDGruppoComunicazione.ToString());
          cmd.Parameters.AddWithValue("@ID", IDComunicazione.ToString());
          cmd.Parameters.AddWithValue("@rec", tmpNode.OuterXml);
          cmd.Parameters.AddWithValue("@guid", guid);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              cBusinessObjects.logger.Error(ex, "wSchedaImmissioneFlussi.buttonEliminaAllegato_Click exception");
              string msg = "SQL call 'flussi.EliminaAllegato' failed: errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
        if (App.m_xmlCache.Contains(guid)) App.m_xmlCache.Remove(guid);
      }
      string directory = "";
      directory = App.AppDocumentiFolder + "\\Flussi";
      string file = directory + "\\" + htAllegati[((ListBoxItem)(lstAllegati.SelectedValue)).Content.ToString()].ToString();
      FileInfo fi = new FileInfo(file);
      if (fi.Exists) fi.Delete();
      _x.Load();
      Load();
    }

    //----------------------------------------------------------------------------+
    //                          buttonApriAllegato_Click                          |
    //----------------------------------------------------------------------------+
    private void buttonApriAllegato_Click(object sender, RoutedEventArgs e)
    {
      if (lstAllegati.SelectedIndex == -1)
      {
        MessageBox.Show("Selezionare un allegato");
        return;
      }
      try
      {
        string directory = "";
        directory = App.AppDocumentiFolder + "\\Flussi";
        string file = directory + "\\" +
          htAllegati[((ListBoxItem)(lstAllegati.SelectedValue)).Content.ToString()].ToString();
        FileInfo fi = new FileInfo(file);
        if (fi.Exists)
        {
          System.Diagnostics.Process process = new System.Diagnostics.Process();
          process.Refresh();
          process.StartInfo.FileName = file;
          process.StartInfo.ErrorDialog = false;
          process.StartInfo.Verb = "open";
          process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
          process.Start();
        }
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaImmissioneFlussi.buttonApriAllegato_Click exception");
        string log = ex.Message;
      }
    }
  } // class wSchedaImmissioneFlussi
} // namespace RevisoftApplication

// srcOld
/*
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
using System.IO;
using System.Collections;
using System.ComponentModel;

namespace RevisoftApplication
{

    public partial class wSchedaImmissioneFlussi : Window
    {
        public RevisoftApplication.wSchedaSceltaFlussi.TipoFlusso tipo = new RevisoftApplication.wSchedaSceltaFlussi.TipoFlusso();

        public int IDComunicazione = -1;
        public int IDGruppoComunicazione = -1;  
        public int IDTab = -1;
        public int IDCliente = -1;


        private int newIDTab = -1;
        public string oldIR = "";

		private Hashtable htMD = new Hashtable();
        private Hashtable htAllegati = new Hashtable();

        public XmlDataProviderManager _x;
        
        public wSchedaImmissioneFlussi()
		{
			InitializeComponent();            
        }

        public void Load()
        {
            cmbMD.Items.Clear();
            htMD.Clear();
            htAllegati.Clear();
            lstAllegati.Items.Clear();

            string xpath = "//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]/Dato";

            foreach (XmlNode item in _x.Document.SelectNodes(xpath))
	        {
                cmbMD.Items.Add( item.Attributes["TABNAME"].Value.ToString() );

                if(IDTab == Convert.ToInt32(item.Attributes["TAB"].Value.ToString()))
                {
                    cmbMD.SelectedValue = item.Attributes["TABNAME"].Value.ToString();
                    lblNuovo.Visibility = System.Windows.Visibility.Collapsed;
                    txtMD.Visibility = System.Windows.Visibility.Collapsed;
                }

                if ( !htMD.Contains( item.Attributes["TAB"].Value.ToString() ) )
                {
                    htMD.Add( item.Attributes["TAB"].Value.ToString(), item.Attributes["TABNAME"].Value.ToString() );
                }

                if ( Convert.ToInt32( item.Attributes["TAB"].Value.ToString() ) > newIDTab )
                {
                    newIDTab = Convert.ToInt32( item.Attributes["TAB"].Value.ToString() );
                }
            }

            newIDTab++;

            if(oldIR != "")
            {
                if(oldIR == "inviata")
                {
                    rdbInviata.IsChecked = false;
                    rdbRicevuta.IsChecked = true;
                }
                else
                {
                    rdbInviata.IsChecked = true;
                    rdbRicevuta.IsChecked = false;
                }
            }

            xpath = "//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]/Dato[@TAB=" + IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "][@ID=" + IDComunicazione + "]";
            XmlNode tmpNode = _x.Document.SelectSingleNode( xpath );

            if(tmpNode != null)
            {
                if ( tmpNode.Attributes["IR"].Value.ToString() == "inviata" )
                {
                    rdbInviata.IsChecked = true;
                    rdbRicevuta.IsChecked = false;
                }
                else
                {
                    rdbInviata.IsChecked = false;
                    rdbRicevuta.IsChecked = true;
                }

                txtNota.Text = tmpNode.Attributes["NOTE"].Value.ToString();
                dtpData.SelectedDate = Convert.ToDateTime( tmpNode.Attributes["DATA"].Value.ToString() );
                cmbMD.IsEnabled = false;

                xpath = "//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]/Dato[@TAB=" + IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "][@ID=" + IDComunicazione + "]/Allegato";
                foreach ( XmlNode item in _x.Document.SelectNodes( xpath ) )
                {
                    htAllegati.Add( item.Attributes["TITOLO"].Value.ToString(), item.Attributes["FILE"].Value.ToString() );
                    ListBoxItem itm = new ListBoxItem();
                    itm.Content = item.Attributes["TITOLO"].Value.ToString();

                    lstAllegati.Items.Add(itm);
                }
            }
        }
     
        private void RadioButtonInviata_Checked( object sender, RoutedEventArgs e )
        {
            labelMittenteDestinatario.Content = "Destinatario";
        }

        private void RadioButtonRicevuta_Checked( object sender, RoutedEventArgs e )
        {
            labelMittenteDestinatario.Content = "Mittente";
        }
        
        private void buttonCompletato_Click( object sender, RoutedEventArgs e )
        {
            string xpath = "";
            string xml = "";
            XmlNode tmpNodeFather = null;
            XmlDocument doctmp = null;
            XmlNode tmpNodeToBeImported = null;
            XmlNode tmpNodeNew  = null;

            if ( dtpData.SelectedDate == null)
            {
                MessageBox.Show( "Inserire una data" );
                return;
            }

            if(rdbInviata.IsChecked == false && rdbRicevuta.IsChecked == false)
            {
                MessageBox.Show("Selezionare una voce tra Inviata e Ricevuta");
                return;
            }

            if(IDComunicazione == -1)//Nuova Comunicazione
            {
                if(IDGruppoComunicazione == -1 )//Nuovo Gruppo Comunicazioni
                {
                    if(IDTab == -1) //Nuovo Tab
                    {
                        if(txtMD.Text != "") //Nuovo Mittente/destinatario
                        {
                            xml = "<Dato TAB=\"" + newIDTab + "\" TABNAME=\"" + txtMD.Text.Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\"/>";

                            IDTab = newIDTab;
                            htMD.Add( newIDTab.ToString(), txtMD.Text.Replace( "&", "&amp;" ).Replace( "\"", "'" ) );
                            newIDTab++;
                            xpath = "//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]";

                            tmpNodeFather = _x.Document.SelectSingleNode( xpath );

                            doctmp = new XmlDocument();
                            doctmp.LoadXml( xml );

                            tmpNodeToBeImported = doctmp.SelectSingleNode( "/Dato" );
                            tmpNodeNew = _x.Document.ImportNode( tmpNodeToBeImported, true );

                            tmpNodeFather.AppendChild( tmpNodeNew );
                        }
                        else
                        {
                            MessageBox.Show("Inserire il Mittente/Destinatario");
                            return;
                        }
                    }

                    xpath = "//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]/Dato[@TAB=" + IDTab + "]/Valore";

                    foreach (XmlNode item in _x.Document.SelectNodes(xpath))
	                {
                        if ( Convert.ToInt32( item.Attributes["GRUPPO"].Value.ToString() ) > IDGruppoComunicazione )
                        {
                            IDGruppoComunicazione = Convert.ToInt32( item.Attributes["GRUPPO"].Value.ToString() );
                        }
                    }

                    IDGruppoComunicazione++;                
                }

                xpath = "//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]/Dato[@TAB=" + IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "]";

                foreach (XmlNode item in _x.Document.SelectNodes(xpath))
	            {
                    if ( Convert.ToInt32( item.Attributes["ID"].Value.ToString() ) > IDComunicazione )
                    {
                        IDComunicazione = Convert.ToInt32( item.Attributes["ID"].Value.ToString() );
                    }
                }

                IDComunicazione++;
            }

            xpath = "//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]/Dato[@TAB=" + IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "][@ID=" + IDComunicazione + "]";
            tmpNodeFather = _x.Document.SelectSingleNode( xpath );

            if(tmpNodeFather == null)
            {
                xml = "<Valore ID=\"" + IDComunicazione + "\" GRUPPO=\"" + IDGruppoComunicazione + "\" GRUPPOCOLOR=\"" + ( ( IDGruppoComunicazione % 2 == 0 ) ? "AliceBlue" : "LightYellow" ) + "\" IR=\"" + ( ( rdbInviata.IsChecked == true ) ? "inviata" : "ricevuta" ) + "\" DATA=\"" + dtpData.SelectedDate.Value.ToShortDateString() + "\"  NOTE=\"" + txtNota.Text.Replace( "&", "&amp;" ).Replace( "\"", "'" ).Replace( "<", "&lt;" ).Replace( ">", "&gt;" ) + "\" ALLEGATI=\"\"/>";

                doctmp = new XmlDocument();
                doctmp.LoadXml( xml );

                tmpNodeToBeImported = doctmp.SelectSingleNode( "/Valore" );
                tmpNodeNew = _x.Document.ImportNode( tmpNodeToBeImported, true );

                xpath = "//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]/Dato[@TAB=" + IDTab + "]";
                tmpNodeFather = _x.Document.SelectSingleNode( xpath );

                XmlNode LastNode = null;

                foreach ( XmlNode item in _x.Document.SelectNodes("//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]/Dato[@TAB=" + IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "]") )
                {
                    //if ( DateTime.Compare( Convert.ToDateTime( item.Attributes["DATA"].Value.ToString() ), Convert.ToDateTime( dtpData.SelectedDate.Value.ToShortDateString() ) ) < 0 )
                    {
                        LastNode = item;
                    }
                }

                if ( LastNode != null )
                {
                    tmpNodeFather.InsertAfter( tmpNodeNew, LastNode );
                }
                else
                {
                    tmpNodeFather.AppendChild( tmpNodeNew );
                }
            }
            else
            {
                tmpNodeFather.Attributes["IR"].Value = ( ( rdbInviata.IsChecked == true ) ? "inviata" : "ricevuta" );
                tmpNodeFather.Attributes["DATA"].Value = dtpData.SelectedDate.Value.ToShortDateString();
                tmpNodeFather.Attributes["NOTE"].Value = txtNota.Text.Replace( "&", "&amp;" ).Replace( "\"", "'" );
                tmpNodeFather.Attributes["ALLEGATI"].Value = ((lstAllegati.Items.Count > 0)? "PRESENTI": "");
            }

            _x.Save();
        }

        private void buttonChiudi_Click( object sender, RoutedEventArgs e )
        {
            //buttonCompletato_Click( sender, e );

            if ( htMD.Contains( IDTab.ToString() ) )
            {
                ( (wFlussi)Owner ).Load();

                foreach ( TabItem item in ( (wFlussi)Owner ).tabControlFlussi.Items )
                {
                    if ( item.Header.ToString() == htMD[IDTab.ToString()].ToString() )
                    {
                        ( (wFlussi)Owner ).tabControlFlussi.SelectedItem = item;
                    }
                }
                ( (wFlussi)Owner ).tabControlFlussi.Focus();
            }
            this.Close();
        }

        private void cmbMD_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            lblNuovo.Visibility = System.Windows.Visibility.Visible;
            txtMD.Visibility = System.Windows.Visibility.Visible;

            foreach (DictionaryEntry item in htMD)
	        {
		        if(item.Value == cmbMD.SelectedItem)
                {
                    IDTab = Convert.ToInt32(item.Key.ToString());
                    lblNuovo.Visibility = System.Windows.Visibility.Collapsed;
                    txtMD.Visibility = System.Windows.Visibility.Collapsed;
                    return;
                }
	        } 
        }

        public string InitialDirectory = "";

        private string dialogSaveFile()
        {
            string file = "";
            string newName = "";

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = InitialDirectory;

            if ( dlg.ShowDialog() == true )
            {
                FileInfo fi = new FileInfo( dlg.FileName );
                if ( fi.Exists )
                {
                    string pathnew = fi.FullName.Replace( fi.FullName.Split( '\\' ).Last(), "" );
                    DirectoryInfo dinew = new DirectoryInfo( pathnew );

                    if ( dinew.Exists )
                    {
                        InitialDirectory = pathnew;
                    }

                    string directory = App.AppDocumentiFolder + "\\Flussi";

                    DirectoryInfo dtest = new DirectoryInfo(directory);
                    if(dtest.Exists == false)
                    {
                        dtest.Create();
                    }
                    
                    int IDHere = -1;

                    foreach (FileInfo item in dtest.GetFiles())
                    {
                        if (IDHere < Convert.ToInt32(item.Name.Replace(item.Extension, "")))
                        {
                            IDHere = Convert.ToInt32(item.Name.Replace(item.Extension, ""));
                        }
                    }
                    
                    IDHere++;

                    newName = IDHere + "." + dlg.FileName.Split( '.' ).Last();
                    file = directory + "\\" + newName;
                    FileInfo fitmp = new FileInfo( file );
                    if ( fitmp.Exists )
                    {
                        fitmp.Delete();
                    }

                    fi.IsReadOnly = false;
                    fi.CopyTo( file );
                }
            }

            return file;
        }

        private void buttonAggiungiAllegato_Click( object sender, RoutedEventArgs e )
        {
            if(IDGruppoComunicazione == -1 || IDComunicazione == -1 || IDTab == -1)
            {
                MessageBox.Show( "Impossibile aggiungere allegati, senza prima aver creato il Flusso." );
                return;
            }

            var dialog = new wInputBox( "Inserire Titolo del File" );
            dialog.ResponseText = "";
            dialog.ShowDialog();

            string titolo = dialog.ResponseText.Replace( "&", "&amp;" ).Replace( "\"", "'" );

            if(titolo == "")
            {
                return;
            }

            string xpath = "//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]/Dato[@TAB=" + IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "][@ID=" + IDComunicazione + "]/Allegato[@TITOLO=\"" + titolo + "\"]";
            XmlNode tmpNode = _x.Document.SelectSingleNode( xpath );

            if(tmpNode != null)
            {
                MessageBox.Show( "Titolo già presente" );
                return;
            }

            string file = dialogSaveFile();

            if(file == "" || file == null)
            {
                return;
            }

            string xml = "<Allegato ID=\"" + newIDTab + "\" FILE=\"" + file.Split('\\').Last() + "\" TITOLO=\"" + titolo + "\"/>";

            htAllegati.Add( titolo, file );

            xpath = "//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]/Dato[@TAB=" + IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "][@ID=" + IDComunicazione + "]";

            XmlNode tmpNodeFather = _x.Document.SelectSingleNode( xpath );

            XmlDocument doctmp = new XmlDocument();
            doctmp.LoadXml( xml );

            XmlNode tmpNodeToBeImported = doctmp.SelectSingleNode( "/Allegato" );
            XmlNode tmpNodeNew = _x.Document.ImportNode( tmpNodeToBeImported, true );

            tmpNodeFather.AppendChild( tmpNodeNew );

            _x.Save();
            Load();
        }

        private void buttonEliminaAllegato_Click( object sender, RoutedEventArgs e )
        {
            if ( lstAllegati.SelectedIndex == -1 )
            {
                MessageBox.Show( "Selezionare un allegato" );
                return;
            }

            if(MessageBox.Show( "Sicuri di voler eliminare l'allegato?", "Attenzione", MessageBoxButton.YesNo ) == MessageBoxResult.No)
            {
                return;
            }

            string xpath = "//Dati[@TIPO=" + Convert.ToInt32( tipo ) + "]/Dato[@TAB=" + IDTab + "]/Valore[@GRUPPO=" + IDGruppoComunicazione + "][@ID=" + IDComunicazione + "]/Allegato[@FILE=\"" + htAllegati[( (ListBoxItem)( lstAllegati.SelectedValue ) ).Content.ToString()].ToString() + "\"]";
            XmlNode tmpNode = _x.Document.SelectSingleNode( xpath );

            if ( tmpNode != null )
            {
                tmpNode.ParentNode.RemoveChild( tmpNode );
            }

            string directory = "";
            directory = App.AppDocumentiFolder + "\\Flussi";

            string file = directory + "\\" + htAllegati[( (ListBoxItem)( lstAllegati.SelectedValue ) ).Content.ToString()].ToString();
            FileInfo fi = new FileInfo( file );

            if ( fi.Exists )
            {
                fi.Delete();
            }

            _x.Save();
            Load();
        }

        private void buttonApriAllegato_Click( object sender, RoutedEventArgs e )
        {
			if (lstAllegati.SelectedIndex == -1)
			{
				MessageBox.Show("Selezionare un allegato");
                return;
			}

			try
			{
                string directory = "";
                directory = App.AppDocumentiFolder + "\\Flussi";

                string file = directory + "\\" + htAllegati[( (ListBoxItem)( lstAllegati.SelectedValue ) ).Content.ToString()].ToString();
			    FileInfo fi = new FileInfo(file);

			    if (fi.Exists)
			    {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.Refresh();
                    process.StartInfo.FileName = file;
                    process.StartInfo.ErrorDialog = false;
                    process.StartInfo.Verb = "open";
                    process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
                    process.Start();

                    //System.Diagnostics.Process.Start(file);
				}
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}            
        }

    }
}
*/
