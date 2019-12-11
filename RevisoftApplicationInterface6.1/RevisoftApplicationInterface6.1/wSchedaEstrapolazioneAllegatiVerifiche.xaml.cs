//----------------------------------------------------------------------------+
//                 wSchedaEstrapolazioneAllegatiVerifiche.cs                  |
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
  //               class wSchedaEstrapolazioneAllegatiVerifiche               |
  //==========================================================================+
  public partial class wSchedaEstrapolazioneAllegatiVerifiche : Window
  {
    ArrayList alVerifiche = new ArrayList();
    ArrayList alVigilanze = new ArrayList();
    ArrayList Esercizi = new ArrayList();
    ArrayList Sessioni = new ArrayList();
    DataTable _dtDox; //------------------- tutti i dox del cliente selezionato
    Hashtable htClienti = new Hashtable();

    //------------------------------------------------------------------------+
    //                 wSchedaEstrapolazioneAllegatiVerifiche                 |
    //------------------------------------------------------------------------+
    public wSchedaEstrapolazioneAllegatiVerifiche()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
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
      string cliente, IDCliente;

      mf = MasterFile.Create();
      index = 0;
      selectedIndex = -1;
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
        myList.Add(
          new KeyValuePair<string, string>(item["ID"].ToString(), cliente));
      }
      myList.Sort
      (
        delegate (
          KeyValuePair<string, string> firstPair,
          KeyValuePair<string, string> nextPair)
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
      IDCliente = mf.GetClienteFissato();
      foreach (DictionaryEntry item in htClienti)
      {
        if (item.Value.ToString() == IDCliente)
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
      Hashtable ht;
      MasterFile mf;
      string file, returnvalue;
      XmlDataProviderManager _y;
      XmlNode node;

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
      _y = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + file);
      node = _y.Document.SelectSingleNode("//Tree//Node[@ID='" + nodo + "']");
      if (node != null)
      {
        returnvalue = node.Attributes["Codice"].Value + " "
          + node.Attributes["Titolo"].Value;
      }
      return returnvalue;
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
    //                          buttonTODOList_Click                          |
    //------------------------------------------------------------------------+
    private void buttonTODOList_Click(object sender, RoutedEventArgs e)
    {
      ArrayList VerificheTBD, VigilanzeTBD;
      bool atLeastOne;
      string str,extension,folder,titolo,dstFile,IDCliente, selectedDIR, selectedDIR_TMP;
      Utilities u;
      int i;

      //------------------------------------------- controllo selezione clienti
      if (cmbClienti.SelectedIndex == -1)
      {
        MessageBox.Show("selezionare un Cliente"); return;
      }
      VerificheTBD = new ArrayList(); VigilanzeTBD = new ArrayList();
      //---------------- verifica almeno una sessione con documenti selezionata
      atLeastOne = false;
      foreach (object item in stpSessioniVerifiche.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)(item)).Tag.ToString() == "-1") continue;
          if (((CheckBox)(item)).IsChecked == true)
          {
            atLeastOne = true;
            VerificheTBD.Add(((CheckBox)(item)).Tag.ToString());
          }
        }
      }
      foreach (object item in stpSessioniVigilanze.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)(item)).Tag.ToString() == "-1") continue;
          if (((CheckBox)(item)).IsChecked == true)
          {
            atLeastOne = true;
            VigilanzeTBD.Add(((CheckBox)(item)).Tag.ToString());
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

      //di = new DirectoryInfo(selectedDIR);
      //if (!di.Exists) di.Create();
      //selectedDIR += "\\EstrapolazioneDocumentiVerifiche_" + cmbClienti.SelectedValue;
      //di = new DirectoryInfo(selectedDIR);
      //if (!di.Exists) di.Create();
      //selectedDIR_TMP = "";

      if (selectedDIR == "") return;
      selectedDIR += "\\EstrapolazioneDocumentiVerifiche_" + cmbClienti.SelectedValue;
      Directory.CreateDirectory(selectedDIR);
      selectedDIR_TMP = string.Empty;

      foreach (Hashtable htVerifica in alVerifiche)
      {
        if (VerificheTBD.Contains(htVerifica["ID"].ToString()))
        {
          selectedDIR_TMP = selectedDIR + "\\"
            + ToFileString(u.TitoloAttivita(App.TipoAttivita.Verifica)) + "\\"
            + ToFileString(((htVerifica["Data"] != null) ?
            htVerifica["Data"].ToString() : Guid.NewGuid().ToString()));

          //di = new DirectoryInfo(selectedDIR_TMP);
          //if (!di.Exists) di.Create();
          Directory.CreateDirectory(selectedDIR_TMP);
          //------------------------------------------------------------------+
          //                       trasferimento files                        |
          //------------------------------------------------------------------+

          //xpath = "//DOCUMENTI//DOCUMENTO[@Cliente='" + IDCliente + "']";
          //xpath += "[@Tree='" + (Convert.ToInt32(App.TipoFile.Verifica)).ToString() + "']";
          //xpath += "[@Sessione='" + htVerifica["ID"].ToString() + "']";
          //_x = new XmlDataProviderManager(App.AppDocumentiDataFile, true);
          //foreach (XmlNode item in _x.Document.SelectNodes(xpath))
          //{
          //  if (item.Attributes["File"] == null) continue;
          //  file = App.AppDocumentiFolder + "\\" + item.Attributes["File"].Value;
          //  fi = new FileInfo(file);
          //  if (fi.Exists)
          //  {
          //    newdir = Guid.NewGuid().ToString();
          //    try
          //    {
          //      newdir = GetNodeString(
          //        item.Attributes["Tree"].Value,
          //        item.Attributes["Sessione"].Value,
          //        item.Attributes["Nodo"].Value);
          //    }
          //    catch (Exception ex)
          //    {
          //      cBusinessObjects.logger.Error(ex, "wSchedaEstrapolazioneAllegatiVerifiche.buttonTODOList_Click1 exception");
          //    }
          //    di = new DirectoryInfo(selectedDIR_TMP + "\\" + newdir);
          //    if (!di.Exists) di.Create();
          //    titolohere = ReplaceChar(
          //      ToFileString(((item.Attributes["Titolo"] == null) ?
          //      "Nessun Titolo" : item.Attributes["Titolo"].Value)));
          //    if (titolohere.Length > 50)
          //      titolohere = titolohere.Substring(0, 50) + "__";
          //    newfile = selectedDIR_TMP + "\\" + newdir + "\\" + titolohere;
          //    while ((new FileInfo(newfile + fi.Extension)).Exists)
          //      newfile += "(1)";
          //    try { fi.CopyTo(newfile + fi.Extension, true); }
          //    catch (Exception ex)
          //    {
          //      cBusinessObjects.logger.Error(ex, "wSchedaEstrapolazioneAllegatiVerifiche.buttonTODOList_Click2 exception");
          //    }
          //  }
          //}
          str = string.Format("(ID_CLIENTE={0}) and (Tree='{1}') and (ID_SESSIONE={2})",
            IDCliente, Convert.ToInt32(App.TipoFile.Verifica), htVerifica["ID"].ToString());
          foreach (DataRow dr in _dtDox.Select(str))
          {
            str = App.AppDocumentiFolder + "\\" + dr["File"].ToString();
            if (!File.Exists(str)) continue;
            extension = System.IO.Path.GetExtension(str);
            folder = selectedDIR_TMP + "\\" + dr["NodoExtended"].ToString();
            Directory.CreateDirectory(folder);
            titolo = ReplaceChar(ToFileString(dr["Titolo"].ToString()));
            if (string.IsNullOrEmpty(titolo)) titolo = "senza_titolo";
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

      foreach (Hashtable htVigilanza in alVigilanze)
      {
        if (VigilanzeTBD.Contains(htVigilanza["ID"].ToString()))
        {
          selectedDIR_TMP = selectedDIR + "\\"
            + ToFileString(u.TitoloAttivita(App.TipoAttivita.Vigilanza))
            + "\\" + ToFileString(((htVigilanza["Data"] != null) ?
            htVigilanza["Data"].ToString() : Guid.NewGuid().ToString()));

          //di = new DirectoryInfo(selectedDIR_TMP);
          //if (!di.Exists) di.Create();
          Directory.CreateDirectory(selectedDIR_TMP);
          //------------------------------------------------------------------+
          //                       trasferimento files                        |
          //------------------------------------------------------------------+

          //xpath = "//DOCUMENTI//DOCUMENTO[@Cliente='" + IDCliente + "']";
          //xpath += "[@Tree='" + (Convert.ToInt32(App.TipoFile.Vigilanza)).ToString() + "']";
          //xpath += "[@Sessione='" + htVigilanza["ID"].ToString() + "']";
          //_x = new XmlDataProviderManager(App.AppDocumentiDataFile, true);
          //foreach (XmlNode item in _x.Document.SelectNodes(xpath))
          //{
          //  file = App.AppDocumentiFolder + "\\" + item.Attributes["File"].Value;
          //  fi = new FileInfo(file);
          //  if (fi.Exists)
          //  {
          //    newdir = Guid.NewGuid().ToString();
          //    try
          //    {
          //      newdir = GetNodeString(
          //        item.Attributes["Tree"].Value,
          //        item.Attributes["Sessione"].Value,
          //        item.Attributes["Nodo"].Value);
          //    }
          //    catch (Exception ex)
          //    {
          //      cBusinessObjects.logger.Error(ex, "wSchedaEstrapolazioneAllegatiVerifiche.buttonTODOList_Click3 exception");
          //    }
          //    di = new DirectoryInfo(selectedDIR_TMP + "\\" + newdir);
          //    if (!di.Exists) di.Create();
          //    titolohere = ReplaceChar(
          //      ToFileString(((item.Attributes["Titolo"] == null) ?
          //      "Nessun Titolo" : item.Attributes["Titolo"].Value)));
          //    if (titolohere.Length > 50)
          //      titolohere = titolohere.Substring(0, 50) + "__";
          //    newfile = selectedDIR_TMP + "\\" + newdir + "\\" + titolohere;
          //    while ((new FileInfo(newfile + fi.Extension)).Exists)
          //      newfile += "(1)";
          //    try { fi.CopyTo(newfile + fi.Extension, true); }
          //    catch (Exception ex)
          //    {
          //      cBusinessObjects.logger.Error(ex, "wSchedaEstrapolazioneAllegatiVerifiche.buttonTODOList_Click4 exception");
          //    }
          //  }
          //}
          str = string.Format("(ID_CLIENTE={0}) and (Tree='{1}') and (ID_SESSIONE={2})",
            IDCliente, Convert.ToInt32(App.TipoFile.Vigilanza), htVigilanza["ID"].ToString());
          foreach (DataRow dr in _dtDox.Select(str))
          {
            str = App.AppDocumentiFolder + "\\" + dr["File"].ToString();
            if (!File.Exists(str)) continue;
            extension = System.IO.Path.GetExtension(str);
            folder = selectedDIR_TMP + "\\" + dr["NodoExtended"].ToString();
            Directory.CreateDirectory(folder);
            titolo = ReplaceChar(ToFileString(dr["Titolo"].ToString()));
            if (string.IsNullOrEmpty(titolo)) titolo = "senza_titolo";
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
      FileInfo fi;
      int counter;
      string file, IDCliente, str, xpath;
      XmlDataProviderManager _x;

      IDCliente = htClienti[((ComboBox)sender).SelectedIndex].ToString();
      stpSessioniVerifiche.Children.Clear();
      stpSessioniVigilanze.Children.Clear();
      MasterFile mf = MasterFile.Create();
      alVerifiche = mf.GetVerifiche(IDCliente);
      alVigilanze = mf.GetVigilanze(IDCliente);

      str = string.Format(
        "select * from dbo.ArchivioDocumenti where (ID_CLIENTE={0})", IDCliente);
      _dtDox = StaticUtilities.DataTableFromQuery(str);

      // VERIFICHE
      TextBlock txtVerifiche = new TextBlock();
      txtVerifiche.HorizontalAlignment =
        System.Windows.HorizontalAlignment.Center;
      txtVerifiche.Text = "Controllo Contabile";
      txtVerifiche.FontWeight = FontWeights.Bold;
      txtVerifiche.Margin = new Thickness(0, 0, 0, 5);
      stpSessioniVerifiche.Children.Add(txtVerifiche);

      CheckBox chkSessioneTuttoVerifiche = new CheckBox();
      chkSessioneTuttoVerifiche.Tag = "-1";
      chkSessioneTuttoVerifiche.Content = "Tutte";
      chkSessioneTuttoVerifiche.Margin = new Thickness(0, 0, 0, 5);
      chkSessioneTuttoVerifiche.Checked += chkSessioneTuttoVerifiche_Checked;
      chkSessioneTuttoVerifiche.Unchecked += chkSessioneTuttoVerifiche_Unchecked;
      stpSessioniVerifiche.Children.Add(chkSessioneTuttoVerifiche);

      //_x = new XmlDataProviderManager(App.AppDocumentiDataFile, true);
      foreach (Hashtable htVerifica in alVerifiche)
      {
        counter = 0;
        //xpath = "//DOCUMENTI//DOCUMENTO[@Cliente='" + IDCliente + "']";
        //xpath += "[@Tree='" + (Convert.ToInt32(App.TipoFile.Verifica)).ToString() + "']";
        //xpath += "[@Sessione='" + htVerifica["ID"].ToString() + "']";
        //foreach (XmlNode item in _x.Document.SelectNodes(xpath))
        //{
        //  file = App.AppDocumentiFolder + "\\" + item.Attributes["File"].Value;
        //  fi = new FileInfo(file);
        //  if (fi.Exists) counter++;
        //}
        str = string.Format("(Tree='{0}') and (ID_SESSIONE={1})",
          Convert.ToInt32(App.TipoFile.Verifica), htVerifica["ID"].ToString());
        foreach (DataRow dr in _dtDox.Select(str))
        {
          str = App.AppDocumentiFolder + "\\" + dr["File"].ToString();
          if (File.Exists(str)) counter++;
        }

        if (counter > 0)
        {
          CheckBox chkSessione = new CheckBox();
          chkSessione.Tag = htVerifica["ID"].ToString();
          chkSessione.Content = htVerifica["Data"].ToString()
            + " (" + counter.ToString() + ")";
          stpSessioniVerifiche.Children.Add(chkSessione);
        }
      }

      // VIGILANZE
      TextBlock txtVigilanze = new TextBlock();
      txtVigilanze.HorizontalAlignment =
        System.Windows.HorizontalAlignment.Center;
      txtVigilanze.Text = "Vigilanza";
      txtVigilanze.FontWeight = FontWeights.Bold;
      txtVigilanze.Margin = new Thickness(0, 0, 0, 5);
      stpSessioniVigilanze.Children.Add(txtVigilanze);

      CheckBox chkSessioneTuttoVigilanze = new CheckBox();
      chkSessioneTuttoVigilanze.Tag = "-1";
      chkSessioneTuttoVigilanze.Content = "Tutte";
      chkSessioneTuttoVigilanze.Margin = new Thickness(0, 0, 0, 10);
      chkSessioneTuttoVigilanze.Checked += chkSessioneTuttoVigilanze_Checked;
      chkSessioneTuttoVigilanze.Unchecked += chkSessioneTuttoVigilanze_Unchecked;
      stpSessioniVigilanze.Children.Add(chkSessioneTuttoVigilanze);

      foreach (Hashtable htVigilanza in alVigilanze)
      {
        counter = 0;
        //xpath = "//DOCUMENTI//DOCUMENTO[@Cliente='" + IDCliente + "']";
        //xpath += "[@Tree='" + (Convert.ToInt32(App.TipoFile.Vigilanza)).ToString() + "']";
        //xpath += "[@Sessione='" + htVigilanza["ID"].ToString() + "']";
        //foreach (XmlNode item in _x.Document.SelectNodes(xpath))
        //{
        //  file = App.AppDocumentiFolder + "\\" + item.Attributes["File"].Value;
        //  fi = new FileInfo(file);
        //  if (fi.Exists) counter++;
        //}
        str = string.Format("(Tree='{0}') and (ID_SESSIONE={1})",
          Convert.ToInt32(App.TipoFile.Vigilanza), htVigilanza["ID"].ToString());
        foreach (DataRow dr in _dtDox.Select(str))
        {
          str = App.AppDocumentiFolder + "\\" + dr["File"].ToString();
          if (File.Exists(str)) counter++;
        }

        if (counter > 0)
        {
          CheckBox chkSessione = new CheckBox();
          chkSessione.Tag = htVigilanza["ID"].ToString();
          chkSessione.Content = htVigilanza["Data"].ToString()
            + " (" + counter.ToString() + ")";
          stpSessioniVigilanze.Children.Add(chkSessione);
        }
      }
    }

    //------------------------------------------------------------------------+
    //                  chkSessioneTuttoVerifiche_Unchecked                   |
    //------------------------------------------------------------------------+
    void chkSessioneTuttoVerifiche_Unchecked(object sender, RoutedEventArgs e)
    {
      foreach (object item in stpSessioniVerifiche.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)item).Tag.ToString() == "-1") continue;
          ((CheckBox)item).IsChecked = false;
        }
      }
    }

    //------------------------------------------------------------------------+
    //                   chkSessioneTuttoVerifiche_Checked                    |
    //------------------------------------------------------------------------+
    void chkSessioneTuttoVerifiche_Checked(object sender, RoutedEventArgs e)
    {
      foreach (object item in stpSessioniVerifiche.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)item).Tag.ToString() == "-1") continue;
          ((CheckBox)item).IsChecked = true;
        }
      }
    }

    //------------------------------------------------------------------------+
    //                  chkSessioneTuttoVigilanze_Unchecked                   |
    //------------------------------------------------------------------------+
    void chkSessioneTuttoVigilanze_Unchecked(object sender, RoutedEventArgs e)
    {
      foreach (object item in stpSessioniVigilanze.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)item).Tag.ToString() == "-1") continue;
          ((CheckBox)item).IsChecked = false;
        }
      }
    }

    //------------------------------------------------------------------------+
    //                   chkSessioneTuttoVigilanze_Checked                    |
    //------------------------------------------------------------------------+
    void chkSessioneTuttoVigilanze_Checked(object sender, RoutedEventArgs e)
    {
      foreach (object item in stpSessioniVigilanze.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)item).Tag.ToString() == "-1") continue;
          ((CheckBox)item).IsChecked = true;
        }
      }
    }
  } // class wSchedaEstrapolazioneAllegatiVerifiche
} // namespace RevisoftApplication
