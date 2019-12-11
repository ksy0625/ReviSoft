//----------------------------------------------------------------------------+
//               wSchedaEstrapolazioneAllegatiRevisione.xaml.cs               |
//----------------------------------------------------------------------------+
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.IO;
using System.Collections;
using System.Data;

namespace RevisoftApplication
{
  //==========================================================================+
  //               class wSchedaEstrapolazioneAllegatiRevisione               |
  //==========================================================================+
  public partial class wSchedaEstrapolazioneAllegatiRevisione : Window
  {
    Hashtable htClienti = new Hashtable();
    DataTable _dtDox; //------------------- tutti i dox del cliente selezionato

    ArrayList alIncarichi = new ArrayList();
    ArrayList alISQCs = new ArrayList();
    ArrayList alRevisioni = new ArrayList();
    ArrayList alBilanci = new ArrayList();
    ArrayList alConclusioni = new ArrayList();

    //------------------------------------------------------------------------+
    //                 wSchedaEstrapolazioneAllegatiRevisione                 |
    //------------------------------------------------------------------------+
    public wSchedaEstrapolazioneAllegatiRevisione()
    {
      InitializeComponent();
      ConfiguraMaschera();
      cmbClienti.Focus();
    }

    //------------------------------------------------------------------------+
    //                           ConfiguraMaschera                            |
    //------------------------------------------------------------------------+
    public void ConfiguraMaschera()
    {
      int index, selectedIndex;
      List<KeyValuePair<string, string>> myList;
      MasterFile mf;
      string cliente, idClienteFissato;

      mf = MasterFile.Create();
      index = 0; selectedIndex = -1;
      if (cmbClienti.Items.Count != 0)
      {
        selectedIndex = cmbClienti.SelectedIndex;
        cmbClienti.Items.Clear();
        htClienti.Clear();
      }
      myList = new List<KeyValuePair<string, string>>();
      foreach (Hashtable item in mf.GetAnagrafiche())
      {
        cliente = item["RagioneSociale"].ToString();
        myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), cliente));
      }
      myList.Sort
      (
        delegate (KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
        {
          return firstPair.Value.CompareTo(nextPair.Value);
        }
      );
      foreach (KeyValuePair<string, string> item in myList)
      {
        cmbClienti.Items.Add(item.Value);
        htClienti.Add(index, item.Key);
        index++;
      }
      cmbClienti.SelectedIndex = selectedIndex;
      idClienteFissato = mf.GetClienteFissato();
      foreach (DictionaryEntry item in htClienti)
      {
        if (item.Value.ToString() == idClienteFissato)
        {
          cmbClienti.SelectedIndex = Convert.ToInt32(item.Key.ToString());
          return;
        }
      }
    }

    //------------------------------------------------------------------------+
    //                              ToFileString                              |
    //------------------------------------------------------------------------+
    private string ToFileString(string name)
    {
      return name.Replace("\\", "-").Replace("/", "-").Replace(":", "-")
        .Replace("*", "-").Replace("?", "-").Replace("\"", "-")
        .Replace("<", "-").Replace(">", "-").Replace("|", "-");
    }

    //------------------------------------------------------------------------+
    //                             GetNodeString                              |
    //------------------------------------------------------------------------+
    static public string GetNodeString(string albero, string sessione, string nodo)
    {
      MasterFile mf;
      Hashtable ht;
      string file, returnvalue;

      mf = MasterFile.Create();
      file = string.Empty;
      returnvalue = string.Empty;
      switch ((App.TipoFile)(System.Convert.ToInt32(albero)))
      {
        case App.TipoFile.Revisione:
          ht = mf.GetRevisione(sessione);
          file = (ht.Contains("File")) ?
            ht["File"].ToString() : App.AppTemplateTreeRevisione;
          break;
        case App.TipoFile.Verifica:
          ht = mf.GetVerifica(sessione);
          file = (ht.Contains("File")) ?
            ht["File"].ToString() : App.AppTemplateTreeVerifica;
          break;
        case App.TipoFile.Vigilanza:
          ht = mf.GetVigilanza(sessione);
          file = (ht.Contains("File")) ?
            ht["File"].ToString() : App.AppTemplateTreeVigilanza;
          break;
        case App.TipoFile.Incarico:
        case App.TipoFile.IncaricoCS:
        case App.TipoFile.IncaricoSU:
        case App.TipoFile.IncaricoREV:
          ht = mf.GetIncarico(sessione);
          file = (ht.Contains("File")) ?
            ht["File"].ToString() : App.AppTemplateTreeIncarico;
          break;
        case App.TipoFile.ISQC:
          ht = mf.GetISQC(sessione);
          file = (ht.Contains("File")) ?
            ht["File"].ToString() : App.AppTemplateTreeISQC;
          break;
        case App.TipoFile.Bilancio:
          ht = mf.GetBilancio(sessione);
          file = (ht.Contains("File")) ?
            ht["File"].ToString() : App.AppTemplateTreeBilancio;
          break;
        case App.TipoFile.Conclusione:
          ht = mf.GetConclusione(sessione);
          file = (ht.Contains("File")) ?
            ht["File"].ToString() : App.AppTemplateTreeConclusione;
          break;
        default:
          break;
      }
      XmlDataProviderManager _y = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + file);
      XmlNode node = _y.Document.SelectSingleNode("//Tree//Node[@ID='" + nodo + "']");
      if (node != null)
      {
        returnvalue = node.Attributes["Codice"].Value + " " + node.Attributes["Titolo"].Value;
      }
      return returnvalue;
    }

    //------------------------------------------------------------------------+
    //                          buttonTODOList_Click                          |
    //------------------------------------------------------------------------+
    private void buttonTODOList_Click(object sender, RoutedEventArgs e)
    {
      ArrayList TBD1, TBD2, TBD3, TBD9, TBDISQC;
      bool atLeastOne;
      int i;
      string dstFile, extension, folder, IDCliente, selectedDIR, selectedDIR_TMP, str, titolo;
      Utilities u;

      //------------------------------------------- controllo selezione clienti
      if (cmbClienti.SelectedIndex == -1)
      {
        MessageBox.Show("selezionare un Cliente"); return;
      }
      TBD1 = new ArrayList(); TBD2 = new ArrayList(); TBD3 = new ArrayList();
      TBD9 = new ArrayList(); TBDISQC = new ArrayList();
      //---------------- verifica almeno una sessione con documenti selezionata
      atLeastOne = false;
      foreach (object item in stpSessioni1.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)item).Tag.ToString() == "-1") continue;
          if (((CheckBox)(item)).IsChecked == true)
          {
            atLeastOne = true;
            TBD1.Add(((CheckBox)(item)).Tag.ToString());
          }
        }
      }
      foreach (object item in stpSessioniISQC.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)(item)).Tag.ToString() == "-1") continue;
          if (((CheckBox)(item)).IsChecked == true)
          {
            atLeastOne = true;
            TBDISQC.Add(((CheckBox)(item)).Tag.ToString());
          }
        }
      }
      foreach (object item in stpSessioni2.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)(item)).Tag.ToString() == "-1") continue;
          if (((CheckBox)(item)).IsChecked == true)
          {
            atLeastOne = true;
            TBD2.Add(((CheckBox)(item)).Tag.ToString());
          }
        }
      }
      foreach (object item in stpSessioni3.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)(item)).Tag.ToString() == "-1") continue;
          if (((CheckBox)(item)).IsChecked == true)
          {
            atLeastOne = true;
            TBD3.Add(((CheckBox)(item)).Tag.ToString());
          }
        }
      }
      foreach (object item in stpSessioni9.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)(item)).Tag.ToString() == "-1") continue;
          if (((CheckBox)(item)).IsChecked == true)
          {
            atLeastOne = true;
            TBD9.Add(((CheckBox)(item)).Tag.ToString());
          }
        }
      }
      if (!atLeastOne)
      {
        MessageBox.Show("selezionare almeno una sessione"); return;
      }
      //------------------------------------------ scelta cartella destinazione
      IDCliente = htClienti[cmbClienti.SelectedIndex].ToString();
      u = new Utilities();
      selectedDIR = u.sys_OpenDirectoryDialog();
      if (selectedDIR == "") return;
      selectedDIR += "\\EstrapolazioneDocumentiRevisione_" + cmbClienti.SelectedValue;
      Directory.CreateDirectory(selectedDIR);
      selectedDIR_TMP = string.Empty;

      //----------------------------------------------------------------------+
      //                          documenti Incarico                          |
      //----------------------------------------------------------------------+
      foreach (Hashtable htTBD in alIncarichi)
      {
        if (TBD1.Contains(htTBD["ID"].ToString()))
        {
          selectedDIR_TMP = selectedDIR + "\\"
            + ToFileString(u.TitoloAttivita(App.TipoAttivita.Incarico))
            + "\\" + ToFileString(htTBD["DataNomina"].ToString());
          Directory.CreateDirectory(selectedDIR_TMP);
          //------------------------------------------------------------------+
          //                       trasferimento files                        |
          //------------------------------------------------------------------+
          str = string.Format("(ID_CLIENTE={0}) and (Tree='{1}') and (ID_SESSIONE={2})",
            IDCliente, Convert.ToInt32(App.TipoFile.Incarico), htTBD["ID"].ToString());
          foreach (DataRow dr in _dtDox.Select(str))
          {
            str = App.AppDocumentiFolder + "\\" + dr["File"].ToString();
            if (!File.Exists(str)) continue;
            extension = System.IO.Path.GetExtension(str);
            folder = selectedDIR_TMP + "\\" + dr["NodoExtended"].ToString();
            Directory.CreateDirectory(folder);
            titolo = ReplaceChar(ToFileString(dr["Titolo"].ToString()));
            if (titolo.Length > 50) titolo = titolo.Substring(0, 50) + "__";
            i = 1; dstFile = titolo;
            while (File.Exists(folder + @"\" + dstFile + extension))
            {
              dstFile = string.Format("{0}({1})", titolo, i);
            }
            File.Copy(str, folder + @"\" + dstFile + extension);
          }
        }
      }
      //----------------------------------------------------------------------+
      //                            documenti ISQC                            |
      //----------------------------------------------------------------------+
      foreach (Hashtable htTBD in alISQCs)
      {
        if (TBDISQC.Contains(htTBD["ID"].ToString()))
        {
          selectedDIR_TMP = selectedDIR + "\\"
            + ToFileString(u.TitoloAttivita(App.TipoAttivita.ISQC))
            + "\\" + ToFileString(htTBD["DataNomina"].ToString());
          Directory.CreateDirectory(selectedDIR_TMP);
          //------------------------------------------------------------------+
          //                       trasferimento files                        |
          //------------------------------------------------------------------+
          str = string.Format("(ID_CLIENTE={0}) and (Tree='{1}') and (ID_SESSIONE={2})",
            IDCliente, Convert.ToInt32(App.TipoFile.ISQC), htTBD["ID"].ToString());
          foreach (DataRow dr in _dtDox.Select(str))
          {
            str = App.AppDocumentiFolder + "\\" + dr["File"].ToString();
            if (!File.Exists(str)) continue;
            extension = System.IO.Path.GetExtension(str);
            folder = selectedDIR_TMP + "\\" + dr["NodoExtended"].ToString();
            Directory.CreateDirectory(folder);
            titolo = ReplaceChar(ToFileString(dr["Titolo"].ToString()));
            if (titolo.Length > 50) titolo = titolo.Substring(0, 50) + "__";
            i = 1; dstFile = titolo;
            while (File.Exists(folder + @"\" + dstFile + extension))
            {
              dstFile = string.Format("{0}({1})", titolo, i);
            }
            File.Copy(str, folder + @"\" + dstFile + extension);
          }
        }
      }
      //----------------------------------------------------------------------+
      //                         documenti Revisione                          |
      //----------------------------------------------------------------------+
      foreach (Hashtable htTBD in alRevisioni)
      {
        if (TBD2.Contains(htTBD["ID"].ToString()))
        {
          selectedDIR_TMP = selectedDIR + "\\"
            + ToFileString(u.TitoloAttivita(App.TipoAttivita.Revisione))
            + "\\" + ToFileString(htTBD["Data"].ToString().Replace("01/01/", "").Replace("31/12/", ""));
          Directory.CreateDirectory(selectedDIR_TMP);
          //------------------------------------------------------------------+
          //                       trasferimento files                        |
          //------------------------------------------------------------------+
          str = string.Format("(ID_CLIENTE={0}) and (Tree='{1}') and (ID_SESSIONE={2})",
            IDCliente, Convert.ToInt32(App.TipoFile.Revisione), htTBD["ID"].ToString());
          foreach (DataRow dr in _dtDox.Select(str))
          {
            str = App.AppDocumentiFolder + "\\" + dr["File"].ToString();
            if (!File.Exists(str)) continue;
            extension = System.IO.Path.GetExtension(str);
            folder = selectedDIR_TMP + "\\" + dr["NodoExtended"].ToString();
            Directory.CreateDirectory(folder);
            titolo = ReplaceChar(ToFileString(dr["Titolo"].ToString()));
            if (titolo.Length > 50) titolo = titolo.Substring(0, 50) + "__";
            i = 1; dstFile = titolo;
            while (File.Exists(folder + @"\" + dstFile + extension))
            {
              dstFile = string.Format("{0}({1})", titolo, i);
            }
            File.Copy(str, folder + @"\" + dstFile + extension);
          }
        }
      }
      //----------------------------------------------------------------------+
      //                          documenti Bilancio                          |
      //----------------------------------------------------------------------+
      foreach (Hashtable htTBD in alBilanci)
      {
        if (TBD3.Contains(htTBD["ID"].ToString()))
        {
          selectedDIR_TMP = selectedDIR + "\\"
            + ToFileString(u.TitoloAttivita(App.TipoAttivita.Bilancio))
            + "\\" + ToFileString(htTBD["Data"].ToString().Replace("01/01/", "").Replace("31/12/", ""));
          Directory.CreateDirectory(selectedDIR_TMP);
          //------------------------------------------------------------------+
          //                       trasferimento files                        |
          //------------------------------------------------------------------+
          str = string.Format("(ID_CLIENTE={0}) and (Tree='{1}') and (ID_SESSIONE={2})",
            IDCliente, Convert.ToInt32(App.TipoFile.Bilancio), htTBD["ID"].ToString());
          foreach (DataRow dr in _dtDox.Select(str))
          {
            str = App.AppDocumentiFolder + "\\" + dr["File"].ToString();
            if (!File.Exists(str)) continue;
            extension = System.IO.Path.GetExtension(str);
            folder = selectedDIR_TMP + "\\" + dr["NodoExtended"].ToString();
            Directory.CreateDirectory(folder);
            titolo = ReplaceChar(ToFileString(dr["Titolo"].ToString()));
            if (titolo.Length > 50) titolo = titolo.Substring(0, 50) + "__";
            i = 1; dstFile = titolo;
            while (File.Exists(folder + @"\" + dstFile + extension))
            {
              dstFile = string.Format("{0}({1})", titolo, i);
            }
            File.Copy(str, folder + @"\" + dstFile + extension);
          }
        }
      }
      //----------------------------------------------------------------------+
      //                        documenti Conclusione                         |
      //----------------------------------------------------------------------+
      foreach (Hashtable htTBD in alConclusioni)
      {
        if (TBD9.Contains(htTBD["ID"].ToString()))
        {
          selectedDIR_TMP = selectedDIR + "\\"
            + ToFileString(u.TitoloAttivita(App.TipoAttivita.Conclusione))
            + "\\" + ToFileString(htTBD["Data"].ToString().Replace("01/01/", "").Replace("31/12/", ""));
          Directory.CreateDirectory(selectedDIR_TMP);
          //------------------------------------------------------------------+
          //                       trasferimento files                        |
          //------------------------------------------------------------------+
          str = string.Format("(ID_CLIENTE={0}) and (Tree='{1}') and (ID_SESSIONE={2})",
            IDCliente, Convert.ToInt32(App.TipoFile.Conclusione), htTBD["ID"].ToString());
          foreach (DataRow dr in _dtDox.Select(str))
          {
            str = App.AppDocumentiFolder + "\\" + dr["File"].ToString();
            if (!File.Exists(str)) continue;
            extension = System.IO.Path.GetExtension(str);
            folder = selectedDIR_TMP + "\\" + dr["NodoExtended"].ToString();
            Directory.CreateDirectory(folder);
            titolo = ReplaceChar(ToFileString(dr["Titolo"].ToString()));
            if (titolo.Length > 50) titolo = titolo.Substring(0, 50) + "__";
            i = 1; dstFile = titolo;
            while (File.Exists(folder + @"\" + dstFile + extension))
            {
              dstFile = string.Format("{0}({1})", titolo, i);
            }
            File.Copy(str, folder + @"\" + dstFile + extension);
          }
        }
      }
      MessageBox.Show("Estrazione avvenuta con successo");
      Close();
      return;
    }

    //------------------------------------------------------------------------+
    //                              ReplaceChar                               |
    //------------------------------------------------------------------------+
    private string ReplaceChar(string strin)
    {
      return strin.Replace("/", "").Replace("\"", "").Replace("*", "")
        .Replace(":", "").Replace("?", "").Replace("<", "").Replace(">", "")
        .Replace("|", "").Replace(",", "").Replace(".", "").Replace("&", "")
        .Replace("'", "");
    }

    //------------------------------------------------------------------------+
    //                           buttonChiudi_Click                           |
    //------------------------------------------------------------------------+
    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    //------------------------------------------------------------------------+
    //                      cmbClienti_SelectionChanged                       |
    //------------------------------------------------------------------------+
    private void cmbClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      int counter;
      string IDCliente, str;

      IDCliente = htClienti[((ComboBox)sender).SelectedIndex].ToString();
      stpSessioni1.Children.Clear();
      stpSessioniISQC.Children.Clear();
      stpSessioni2.Children.Clear();
      stpSessioni3.Children.Clear();
      stpSessioni9.Children.Clear();

      MasterFile mf = MasterFile.Create();
      alIncarichi = mf.GetIncarichi(IDCliente);
      alISQCs = mf.GetISQCs(IDCliente);
      alRevisioni = mf.GetRevisioni(IDCliente);
      alBilanci = mf.GetBilanci(IDCliente);
      alConclusioni = mf.GetConclusioni(IDCliente);

      str = string.Format(
        "select * from dbo.ArchivioDocumenti where (ID_CLIENTE={0})", IDCliente);
      _dtDox = StaticUtilities.DataTableFromQuery(str);

      #region INCARICHI
      //----------------------------------------------------------------------+
      //                              INCARICHI                               |
      //----------------------------------------------------------------------+
      TextBlock txt1 = new TextBlock();
      txt1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
      txt1.Text = "Area 1";
      txt1.FontWeight = FontWeights.Bold;
      txt1.Margin = new Thickness(0, 0, 0, 5);
      stpSessioni1.Children.Add(txt1);

      CheckBox chkSessioneTutto1 = new CheckBox();
      chkSessioneTutto1.Tag = "-1";
      chkSessioneTutto1.Content = "Tutte";
      chkSessioneTutto1.Margin = new Thickness(0, 0, 0, 5);
      chkSessioneTutto1.Checked += chkSessioneTutto1_Checked;
      chkSessioneTutto1.Unchecked += chkSessioneTutto1_Unchecked;
      stpSessioni1.Children.Add(chkSessioneTutto1);

      foreach (Hashtable hthere in alIncarichi)
      {
        counter = 0;
        str = string.Format("(Tree='{0}') and (ID_SESSIONE={1})",
          Convert.ToInt32(App.TipoFile.Incarico), hthere["ID"].ToString());
        foreach (DataRow dr in _dtDox.Select(str))
        {
          str = App.AppDocumentiFolder + "\\" + dr["File"].ToString();
          if (File.Exists(str)) counter++;
        }
        if (counter > 0)
        {
          CheckBox chkSessione = new CheckBox();
          chkSessione.Tag = hthere["ID"].ToString();
          chkSessione.Content = hthere["DataNomina"].ToString() + " (" + counter.ToString() + ")";
          stpSessioni1.Children.Add(chkSessione);
        }
      }
      #endregion

      #region ISQC
      //----------------------------------------------------------------------+
      //                                 ISQC                                 |
      //----------------------------------------------------------------------+
      TextBlock txtISQC = new TextBlock();
      txtISQC.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
      txtISQC.Text = "ISQC";
      txtISQC.FontWeight = FontWeights.Bold;
      txtISQC.Margin = new Thickness(0, 0, 0, 5);
      stpSessioniISQC.Children.Add(txtISQC);

      CheckBox chkSessioneTuttoISQC = new CheckBox();
      chkSessioneTuttoISQC.Tag = "-1";
      chkSessioneTuttoISQC.Content = "Tutte";
      chkSessioneTuttoISQC.Margin = new Thickness(0, 0, 0, 5);
      chkSessioneTuttoISQC.Checked += chkSessioneTuttoISQC_Checked;
      chkSessioneTuttoISQC.Unchecked += chkSessioneTuttoISQC_Unchecked;
      stpSessioniISQC.Children.Add(chkSessioneTuttoISQC);

      foreach (Hashtable hthere in alISQCs)
      {
        counter = 0;
        str = string.Format("(Tree='{0}') and (ID_SESSIONE={1})",
          Convert.ToInt32(App.TipoFile.ISQC), hthere["ID"].ToString());
        foreach (DataRow dr in _dtDox.Select(str))
        {
          str = App.AppDocumentiFolder + "\\" + dr["File"].ToString();
          if (File.Exists(str)) counter++;
        }
        if (counter > 0)
        {
          CheckBox chkSessione = new CheckBox();
          chkSessione.Tag = hthere["ID"].ToString();
          chkSessione.Content = hthere["DataNomina"].ToString() + " (" + counter.ToString() + ")";
          stpSessioniISQC.Children.Add(chkSessione);
        }
      }
      #endregion

      #region REVISIONI
      //----------------------------------------------------------------------+
      //                              REVISIONI                               |
      //----------------------------------------------------------------------+
      txt1 = new TextBlock();
      txt1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
      txt1.Text = "Area 2";
      txt1.FontWeight = FontWeights.Bold;
      txt1.Margin = new Thickness(0, 0, 0, 5);
      stpSessioni2.Children.Add(txt1);

      chkSessioneTutto1 = new CheckBox();
      chkSessioneTutto1.Tag = "-1";
      chkSessioneTutto1.Content = "Tutte";
      chkSessioneTutto1.Margin = new Thickness(0, 0, 0, 5);
      chkSessioneTutto1.Checked += chkSessioneTutto2_Checked;
      chkSessioneTutto1.Unchecked += chkSessioneTutto2_Unchecked;
      stpSessioni2.Children.Add(chkSessioneTutto1);

      foreach (Hashtable hthere in alRevisioni)
      {
        counter = 0;
        str = string.Format("(Tree='{0}') and (ID_SESSIONE={1})",
          Convert.ToInt32(App.TipoFile.Revisione), hthere["ID"].ToString());
        foreach (DataRow dr in _dtDox.Select(str))
        {
          str = App.AppDocumentiFolder + "\\" + dr["File"].ToString();
          if (File.Exists(str)) counter++;
        }
        if (counter > 0)
        {
          CheckBox chkSessione = new CheckBox();
          chkSessione.Tag = hthere["ID"].ToString();
          chkSessione.Content = hthere["Data"].ToString().Replace("01/01/", "").Replace("31/12/", "")
            + " (" + counter.ToString() + ")";
          stpSessioni2.Children.Add(chkSessione);
        }
      }
      #endregion

      #region BILANCI
      //----------------------------------------------------------------------+
      //                               BILANCI                                |
      //----------------------------------------------------------------------+
      txt1 = new TextBlock();
      txt1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
      txt1.Text = "Area 3";
      txt1.FontWeight = FontWeights.Bold;
      txt1.Margin = new Thickness(0, 0, 0, 5);
      stpSessioni3.Children.Add(txt1);

      chkSessioneTutto1 = new CheckBox();
      chkSessioneTutto1.Tag = "-1";
      chkSessioneTutto1.Content = "Tutte";
      chkSessioneTutto1.Margin = new Thickness(0, 0, 0, 5);
      chkSessioneTutto1.Checked += chkSessioneTutto3_Checked;
      chkSessioneTutto1.Unchecked += chkSessioneTutto3_Unchecked;
      stpSessioni3.Children.Add(chkSessioneTutto1);

      foreach (Hashtable hthere in alBilanci)
      {
        counter = 0;
        str = string.Format("(Tree='{0}') and (ID_SESSIONE={1})",
          Convert.ToInt32(App.TipoFile.Bilancio), hthere["ID"].ToString());
        foreach (DataRow dr in _dtDox.Select(str))
        {
          str = App.AppDocumentiFolder + "\\" + dr["File"].ToString();
          if (File.Exists(str)) counter++;
        }
        if (counter > 0)
        {
          CheckBox chkSessione = new CheckBox();
          chkSessione.Tag = hthere["ID"].ToString();
          chkSessione.Content = hthere["Data"].ToString().Replace("01/01/", "").Replace("31/12/", "")
            + " (" + counter.ToString() + ")";
          stpSessioni3.Children.Add(chkSessione);
        }
      }
      #endregion

      #region CONCLUSIONI
      //----------------------------------------------------------------------+
      //                             CONCLUSIONI                              |
      //----------------------------------------------------------------------+
      txt1 = new TextBlock();
      txt1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
      txt1.Text = "Area 9";
      txt1.FontWeight = FontWeights.Bold;
      txt1.Margin = new Thickness(0, 0, 0, 5);
      stpSessioni9.Children.Add(txt1);

      chkSessioneTutto1 = new CheckBox();
      chkSessioneTutto1.Tag = "-1";
      chkSessioneTutto1.Content = "Tutte";
      chkSessioneTutto1.Margin = new Thickness(0, 0, 0, 5);
      chkSessioneTutto1.Checked += chkSessioneTutto9_Checked;
      chkSessioneTutto1.Unchecked += chkSessioneTutto9_Unchecked;
      stpSessioni9.Children.Add(chkSessioneTutto1);

      foreach (Hashtable hthere in alConclusioni)
      {
        counter = 0;
        str = string.Format("(Tree='{0}') and (ID_SESSIONE={1})",
          Convert.ToInt32(App.TipoFile.Conclusione), hthere["ID"].ToString());
        foreach (DataRow dr in _dtDox.Select(str))
        {
          str = App.AppDocumentiFolder + "\\" + dr["File"].ToString();
          if (File.Exists(str)) counter++;
        }
        if (counter > 0)
        {
          CheckBox chkSessione = new CheckBox();
          chkSessione.Tag = hthere["ID"].ToString();
          chkSessione.Content = hthere["Data"].ToString().Replace("01/01/", "").Replace("31/12/", "")
            + " (" + counter.ToString() + ")";
          stpSessioni9.Children.Add(chkSessione);
        }
      }
      #endregion
    }

    //------------------------------------------------------------------------+
    //                      chkSessioneTutto1_Unchecked                       |
    //------------------------------------------------------------------------+
    void chkSessioneTutto1_Unchecked(object sender, RoutedEventArgs e)
    {
      foreach (object item in stpSessioni1.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)(item)).Tag.ToString() == "-1") continue;
          ((CheckBox)(item)).IsChecked = false;
        }
      }
    }

    //------------------------------------------------------------------------+
    //                       chkSessioneTutto1_Checked                        |
    //------------------------------------------------------------------------+
    void chkSessioneTutto1_Checked(object sender, RoutedEventArgs e)
    {
      foreach (object item in stpSessioni1.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)(item)).Tag.ToString() == "-1") continue;
          ((CheckBox)(item)).IsChecked = true;
        }
      }
    }

    //------------------------------------------------------------------------+
    //                     chkSessioneTuttoISQC_Unchecked                     |
    //------------------------------------------------------------------------+
    void chkSessioneTuttoISQC_Unchecked(object sender, RoutedEventArgs e)
    {
      foreach (object item in stpSessioniISQC.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)(item)).Tag.ToString() == "-1") continue;
          ((CheckBox)(item)).IsChecked = false;
        }
      }
    }

    //------------------------------------------------------------------------+
    //                      chkSessioneTuttoISQC_Checked                      |
    //------------------------------------------------------------------------+
    void chkSessioneTuttoISQC_Checked(object sender, RoutedEventArgs e)
    {
      foreach (object item in stpSessioniISQC.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)(item)).Tag.ToString() == "-1") continue;
          ((CheckBox)(item)).IsChecked = true;
        }
      }
    }

    //------------------------------------------------------------------------+
    //                      chkSessioneTutto2_Unchecked                       |
    //------------------------------------------------------------------------+
    void chkSessioneTutto2_Unchecked(object sender, RoutedEventArgs e)
    {
      foreach (object item in stpSessioni2.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)(item)).Tag.ToString() == "-1") continue;
          ((CheckBox)(item)).IsChecked = false;
        }
      }
    }

    //------------------------------------------------------------------------+
    //                       chkSessioneTutto2_Checked                        |
    //------------------------------------------------------------------------+
    void chkSessioneTutto2_Checked(object sender, RoutedEventArgs e)
    {
      foreach (object item in stpSessioni2.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)(item)).Tag.ToString() == "-1") continue;
          ((CheckBox)(item)).IsChecked = true;
        }
      }
    }

    //------------------------------------------------------------------------+
    //                      chkSessioneTutto3_Unchecked                       |
    //------------------------------------------------------------------------+
    void chkSessioneTutto3_Unchecked(object sender, RoutedEventArgs e)
    {
      foreach (object item in stpSessioni3.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)(item)).Tag.ToString() == "-1") continue;
          ((CheckBox)(item)).IsChecked = false;
        }
      }
    }

    //------------------------------------------------------------------------+
    //                       chkSessioneTutto3_Checked                        |
    //------------------------------------------------------------------------+
    void chkSessioneTutto3_Checked(object sender, RoutedEventArgs e)
    {
      foreach (object item in stpSessioni3.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)(item)).Tag.ToString() == "-1") continue;
          ((CheckBox)(item)).IsChecked = true;
        }
      }
    }

    //------------------------------------------------------------------------+
    //                      chkSessioneTutto9_Unchecked                       |
    //------------------------------------------------------------------------+
    void chkSessioneTutto9_Unchecked(object sender, RoutedEventArgs e)
    {
      foreach (object item in stpSessioni9.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)(item)).Tag.ToString() == "-1") continue;
          ((CheckBox)(item)).IsChecked = false;
        }
      }
    }

    //------------------------------------------------------------------------+
    //                       chkSessioneTutto9_Checked                        |
    //------------------------------------------------------------------------+
    void chkSessioneTutto9_Checked(object sender, RoutedEventArgs e)
    {
      foreach (object item in stpSessioni9.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)(item)).Tag.ToString() == "-1") continue;
          ((CheckBox)(item)).IsChecked = true;
        }
      }
    }
  } // class wSchedaEstrapolazioneAllegatiRevisione
} // namespace RevisoftApplication
