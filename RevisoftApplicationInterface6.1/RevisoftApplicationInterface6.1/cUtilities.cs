using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.Management;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;
using RevisoftApplication.it.revisoft.ws;
using System.Net;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using System.Reflection;

namespace RevisoftApplication
{
  static class StaticUtilities
  {
    static public string ReplaceXml(string valore)
    {
      string returnvalue = valore;

      returnvalue = returnvalue.Replace(" ", "").Replace("'", "").Replace("<", "").Replace("/", "").Replace("\\", "").Replace(">", "").Replace("\"", "").Replace("&", "").Replace(":", "");

      return returnvalue;
    }

    static public string ReplaceXmlKeepSpaces(string valore)
    {
      string returnvalue = valore;

      returnvalue = returnvalue.Replace("'", "").Replace("<", "").Replace("/", "").Replace("\\", "").Replace(">", "").Replace("\"", "").Replace("&", "").Replace(":", "");

      return returnvalue;
    }

    // E.B. nuovi metodi ..
    static public XmlDocument BuildXML(string source)
    {
      XmlDocument xmlDoc = new XmlDocument();
      string str;
      string guidStr = source.Split('\\').Last();
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("dbo.BuildXML", conn);
        cmd.Parameters.AddWithValue("@guid", guidStr);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try
        {
          using (XmlReader reader = cmd.ExecuteXmlReader())
          {
            if (reader.Read())
            {
              xmlDoc.Load(reader); reader.Close();
            }
          }
        }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "BuildXML(): errore\n" + ex.Message;
            System.Windows.MessageBox.Show(msg);
          }
          return null;
        }
      }
      if (!xmlDoc.HasChildNodes)
      {
        str = string.Format("{0}.ok", guidStr);
        File.Delete(str);
        str = string.Format("{0}.bad", guidStr);
        File.WriteAllText(str, "");
        return null;
      }
      else
      {
        str = string.Format("{0}.bad", guidStr);
        File.Delete(str);
        str = string.Format("{0}.ok", guidStr);
        File.WriteAllText(str, "");
      }
      return xmlDoc;
    }
    static public void ClearXmlCache()
    {
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("dbo.ClearXmlCache", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try
        {
          cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "ClearXmlCache(): errore\n" + ex.Message;
            System.Windows.MessageBox.Show(msg);
          }
        }
      }
    }
    static public void PurgeXML(string source = null)
    {
      if (source == null) return;
      if (App.m_xmlCache.Contains(source)) App.m_xmlCache.Remove(source);
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("dbo.PurgeXML", conn);
        cmd.Parameters.AddWithValue("@guid", source);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "PurgeXML(): errore\n" + ex.Message;
            System.Windows.MessageBox.Show(msg);
          }
        }
      }
    }
    static public void MarkNodeAsModified(XmlNode node, string value)
    {
      if (node == null || value == null) return;
      if (node.Attributes[App.MOD_ATTRIB] != null) return;
      XmlAttribute attr = node.OwnerDocument.CreateAttribute(App.MOD_ATTRIB);
      attr.Value = value;
      node.Attributes.Append(attr);
    }
    static public void DumpModifiedCache()
    {
      int count, i, saved;
      string str;
      XMLELEMENT e;

      count = App.m_xmlCache.Count; saved = 0;
      string[] keys = new string[count];
      App.m_xmlCache.Keys.CopyTo(keys, 0);
      for (i = 0; i < count; i++)
      {
        e = (XMLELEMENT)App.m_xmlCache[keys[i]];
        if (e.isModified)
        {
          str = string.Format("{0}.mod", keys[i].Split('\\').Last());
          e.doc.Save(str);
          SaveToSql(keys[i], ref e.doc);
          saved++;
        }
      }
      //if (saved > 1)
      //{
      //  str = string.Format("modifiche: {0}", saved);
      //  MessageBox.Show(str);
      //}
    }
    static public void SaveToSql(string key, ref XmlDocument d)
    {
      XmlDocument doc = new XmlDocument();
      if (key == null || d == null) return;
      doc = ExtractMasterFileMod(d);
      if (doc == null) doc = ExtractTreeMod(d);
      if (doc == null) doc = ExtractDatiMod(d);
      if (doc == null) doc = ExtractFlussiMod(d);
      if (doc == null) return;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        string query, str;
        str = doc.OuterXml.Replace("'", "''");
        query = string.Format(
          "insert into xmlSaveTest (guid,data)\n" +
          "values ('{0}','{1}')", key.Split('\\').Last(), str);
        conn.Open();
        SqlCommand cmd = new SqlCommand(query, conn);
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "SaveToSql(): errore\n" + ex.Message;
            System.Windows.MessageBox.Show(msg);
          }
        }
      }
    }
    static public XmlDocument ExtractTreeMod(XmlDocument d)
    {
      string str, idFather;
      XmlDocument mod = new XmlDocument();
      XmlNode n1, n2;
      XmlAttribute attrFather;

      if (d == null) return null;
      n1 = d.SelectSingleNode("Tree"); if (n1 == null) return null;
      // estrae tutti nodi e sessioni modificati
      str = "Tree//Node[@" + App.MOD_ATTRIB + "]";
      XmlNodeList nodiMod = d.SelectNodes(str);
      str = "Tree/Sessioni[@" + App.MOD_ATTRIB + "]";
      XmlNodeList sessioniMod = d.SelectNodes(str);
      if (nodiMod.Count == 0 && sessioniMod.Count == 0) return null;
      // importazione root
      n1 = d.SelectSingleNode("/Tree"); n2 = mod.ImportNode(n1, false); mod.AppendChild(n2);
      // importazione nodi modificati
      if (nodiMod.Count > 0)
      {
        foreach (XmlNode n in nodiMod)
        {
          idFather = (n.ParentNode.Attributes["ID"] == null) ? "" : n.ParentNode.Attributes["ID"].Value;
          n2 = mod.ImportNode(n, true);
          attrFather = mod.CreateAttribute("idFather");
          attrFather.Value = idFather;
          n2.Attributes.Append(attrFather);
          mod.DocumentElement.AppendChild(n2);
        }
      }
      // importazione sessioni modificate
      if (sessioniMod.Count > 0)
      {
        foreach (XmlNode n in sessioniMod)
        {
          n2 = mod.ImportNode(n, true);
          mod.DocumentElement.AppendChild(n2);
        }
      }
      return mod;
    }
    static public XmlDocument ExtractDatiMod(XmlDocument d)
    {
      string str, idFather;
      XmlDocument mod = new XmlDocument();
      XmlNode n1, n2;
      XmlAttribute attrFather;

      if (d == null) return null;
      n1 = d.SelectSingleNode("Dati"); if (n1 == null) return null;
      // estrae tutti nodi e sessioni modificati
      str = "Dati//Dato[@" + App.MOD_ATTRIB + "]";
      XmlNodeList datiMod = d.SelectNodes(str);
      if (datiMod.Count == 0) return null;
      // importazione root
      n1 = d.SelectSingleNode("/Dati"); n2 = mod.ImportNode(n1, false); mod.AppendChild(n2);
      // importazione nodi modificati
      foreach (XmlNode n in datiMod)
      {
        idFather = (n.ParentNode.Attributes["ID"] == null) ? "" : n.ParentNode.Attributes["ID"].Value;
        n2 = mod.ImportNode(n, true);
        attrFather = mod.CreateAttribute("idFather");
        attrFather.Value = idFather;
        n2.Attributes.Append(attrFather);
        mod.DocumentElement.AppendChild(n2);
      }
      return mod;
    }
    static public XmlDocument ExtractFlussiMod(XmlDocument d)
    {
      string str, datiTipo, datoTab, valoreId, valoreGruppo;
      XmlDocument mod = new XmlDocument();
      XmlNode n1, n2, datiParent, datoParent, valoreParent;
      XmlNodeList lDato, lValore, lAllegato;

      if (d == null) return null;
      n1 = d.SelectSingleNode("FLUSSI"); if (n1 == null) return null;
      // estrae tutti dati, valori e allegati modificati
      str = "FLUSSI//Dato[@" + App.MOD_ATTRIB + "]"; lDato = d.SelectNodes(str);
      str = "FLUSSI//Valore[@" + App.MOD_ATTRIB + "]"; lValore = d.SelectNodes(str);
      str = "FLUSSI//Allegato[@" + App.MOD_ATTRIB + "]"; lAllegato = d.SelectNodes(str);
      // importazione root
      n1 = d.SelectSingleNode("/FLUSSI"); n2 = mod.ImportNode(n1, false); mod.AppendChild(n2);
      if (lDato.Count < 1 && lValore.Count < 1 && lAllegato.Count < 1) return mod;
      // Dato
      if (lDato.Count > 0)
      {
        foreach (XmlNode n in lDato)
        {
          datiParent = n.ParentNode; datiTipo = datiParent.Attributes["TIPO"].Value;
          n1 = mod.SelectSingleNode("//Dati[@TIPO=" + datiTipo + "]");
          if (n1 == null)
          {
            n2 = mod.ImportNode(datiParent, false);
            mod.DocumentElement.AppendChild(n2);
            n1 = mod.SelectSingleNode("//Dati[@TIPO=" + datiTipo + "]");
          }
          n2 = mod.ImportNode(n, false); n1.AppendChild(n2);
        }
      }
      // Valore
      if (lValore.Count > 0)
      {
        foreach (XmlNode n in lValore)
        {
          datoParent = n.ParentNode; datoTab = datoParent.Attributes["TAB"].Value;
          datiParent = datoParent.ParentNode; datiTipo = datiParent.Attributes["TIPO"].Value;
          n1 = mod.SelectSingleNode("//Dati[@TIPO=" + datiTipo + "]/Dato[@TAB=" + datoTab + "]");
          if (n1 == null) break;
          n2 = mod.ImportNode(n, false); n1.AppendChild(n2);
        }
      }
      // Allegato
      if (lAllegato.Count > 0)
      {
        foreach (XmlNode n in lAllegato)
        {
          valoreParent = n.ParentNode;
          valoreId = valoreParent.Attributes["ID"].Value;
          valoreGruppo = valoreParent.Attributes["GRUPPO"].Value;
          datoParent = valoreParent.ParentNode; datoTab = datoParent.Attributes["TAB"].Value;
          datiParent = datoParent.ParentNode; datiTipo = datiParent.Attributes["TIPO"].Value;
          n1 = mod.SelectSingleNode(
            "//Dati[@TIPO=" + datiTipo + "]/Dato[@TAB=" + datoTab + "]/Valore[@ID=" + valoreId + "][@GRUPPO=" + valoreGruppo + "]");
          if (n1 == null) break;
          n2 = mod.ImportNode(n, false); n1.AppendChild(n2);
        }
      }
      return mod;
    }
    static public XmlDocument ExtractMasterFileMod(XmlDocument d)
    {
      string str;
      XmlDocument mod = new XmlDocument();
      XmlNode n1, n2;
      XmlNodeList list;

      if (d == null) return null;
      // importazione root
      n1 = d.SelectSingleNode("ROOT"); if (n1 == null) return null;
      n2 = mod.ImportNode(n1, false); mod.AppendChild(n2);
      // valutazione presenza modifiche
      n1 = d.SelectSingleNode("//*[@" + App.MOD_ATTRIB + "]");
      if (n1 == null) return null;
      // estrae tutti dati, valori e allegati modificati
      // ROOT/REVISOFT
      str = "//REVISOFT[@" + App.MOD_ATTRIB + "]"; list = d.SelectNodes(str);
      if (list.Count > 0)
      {
        foreach (XmlNode n in list)
        {
          n1 = mod.ImportNode(n, false);
          mod.DocumentElement.AppendChild(n1);
        }
      }
      // ROOT/CLIENTI/CLIENTE
      str = "//CLIENTI//CLIENTE[@" + App.MOD_ATTRIB + "]"; list = d.SelectNodes(str);
      if (list.Count > 0)
      {
        foreach (XmlNode n in list)
        {
          n1 = mod.ImportNode(n, false);
          mod.DocumentElement.AppendChild(n1);
        }
      }
      return mod;
    }
    static public DataTable DataTableFromQuery(string query)
    {
      DataTable dt;
      SqlCommand cmd;
      string qry;

      //---------------------------------------------------- verifica argomenti
      if (query == null) return null;
      qry = query.Trim();
      if (string.IsNullOrEmpty(qry)) return null;
      //------------------------------------------------------ esecuzione query
      dt = new DataTable();
      try
      {
        using (var conn = new SqlConnection(App.connString))
        {
          conn.Open(); cmd = new SqlCommand(qry, conn);
          using (SqlDataReader dr = cmd.ExecuteReader()) { dt.Load(dr); }
          conn.Close();
        }
      }
      catch (Exception) { return null; }
      return dt;
    }

    //------------------------------------------------------------------------+
    //                            ImportEstraiDati                            |
    //------------------------------------------------------------------------+
    static public bool ImportEstraiDati(string nomefileimport, bool verbose)
    {

      DirectoryInfo di = null;
      try
      {
        string cartellaTmp = "", str;
        string impCodMacchinaServer, impCodMacchina;
        bool ok = true, isDecoded;
        XmlManager x = new XmlManager { TipoCodifica = XmlManager.TipologiaCodifica.Normale };
        XmlDocument doc = new XmlDocument();

        isDecoded = (nomefileimport.Split('\\').Last().StartsWith(App.BK_DECODED_PREFIX));

        //----------------------------------------------------------------------------+
        //                 estrazione file ZIP in cartella temporanea                 |
        //----------------------------------------------------------------------------+
        try
        {
          cartellaTmp = App.TMP_FOLDER;
          if (!cartellaTmp.EndsWith(@"\")) cartellaTmp += @"\";
          str = cartellaTmp + Guid.NewGuid().ToString();
          di = new DirectoryInfo(str);
          while (di.Exists)
          {
            str = cartellaTmp + Guid.NewGuid().ToString();
            di = new DirectoryInfo(str);
          }
          cartellaTmp = str;
          di.Create();
          //apro lo zip
          Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(nomefileimport);
          if (!isDecoded) zip.Password = App.ZipFilePassword;
          zip.ExtractAll(cartellaTmp);
        }
        catch (Exception ex)
        {
          ok = false;

          di.Delete(true);
          if (!App.m_bNoExceptionMsg)
          {
            str = "ImportEstraiDati(): errore estrazione dati\n" + ex.Message;
            System.Windows.MessageBox.Show(str);
          }
          //App.GestioneLog(str);
        }
        if (!ok)
        {

          di.Delete(true);
          return false;
        }
        //----------------------------------------------------------------------------+
        //                         eventuale verifica licenza                         |
        //----------------------------------------------------------------------------+
        if (App.m_xmlCache.Contains("all.xml")) App.m_xmlCache.Remove("all.xml");
        XmlDataProviderManager _d = new XmlDataProviderManager(cartellaTmp + @"\all.xml", false);
        XmlNode licenza = _d.Document.SelectSingleNode("/ROOT/LICENZA");
        if (licenza != null)
        {
          impCodMacchinaServer = licenza.Attributes["CodiceMacchinaServer"].Value.ToString().Split('-')[0];
          impCodMacchina = licenza.Attributes["CodiceMacchina"].Value.ToString().Split('-')[0];
          RevisoftApplication.GestioneLicenza l = new GestioneLicenza();

          if (!l.VerificaCodiceMacchinaFileImportato(
            impCodMacchinaServer.Split('-')[0], impCodMacchina.Split('-')[0]))
          {

            return false;
          }
        }
        else
        {

          if (System.Windows.MessageBox.Show(
            "ATTENZIONE: vengono acquisiti tutti i dati (Revisione e Verifiche) " +
            "e verranno sovrascritti sull’unità di destinazione. Per importare " +
            "una sola parte dei dati utilizzare il CONDIVIDI DATI presente nelle " +
            "aree specifiche. Procedere?", "ATTENZIONE",
            MessageBoxButton.YesNo) == MessageBoxResult.No)
            return false;

        }
        //----------------------------------------------------------------------------+
        //                 decodifica di ogni file con nome tipo GUID                 |
        //----------------------------------------------------------------------------+
        string dataFile;
        string[] files = Directory.GetFiles(
          cartellaTmp, @"*-*-*-*-*.*", SearchOption.TopDirectoryOnly);
        foreach (string s in files)
        {
          dataFile = s;
          if (isDecoded)
          {
            if (s == "all.xml" || s.EndsWith(".xaml")) continue;
            dataFile = dataFile.Replace(".xml", "");
          }
          else
          {
            if (s.EndsWith(".xml") || s.EndsWith(".xaml")) continue;
          }
          if (isDecoded) doc.Load(s);
          else doc = x.LoadEncodedFile_old(s);
          str = doc.InnerXml.Replace("&#x8;", "");
          doc.InnerXml = str;
          doc.Save(dataFile);
        }
        //----------------------------------------------------------------------------+
        //                    preparazione dati cliente importato                     |
        //----------------------------------------------------------------------------+
        MasterFile.ForceRecreate();
        MasterFile mf = MasterFile.Create();
        XmlNode nodoClienteImportato = _d.Document.SelectSingleNode("/ROOT/CLIENTE");
        Hashtable ht = new Hashtable();
        foreach (XmlAttribute item in nodoClienteImportato.Attributes)
        {
          ht.Add(item.Name, item.Value);
        }
        //----------------------------------------------------------------------------+
        //                              verifica cliente                              |
        //----------------------------------------------------------------------------+
        int IDCliente = mf.CheckEsistenzaCliente(ht);
        if (IDCliente == -1) // esiste gia'
        {

          if (verbose)
          {
            if (System.Windows.MessageBox.Show(
              "Esiste già un cliente " +
                ((nodoClienteImportato.Attributes["RagioneSociale"] == null)
                  ? "" : nodoClienteImportato.Attributes["RagioneSociale"].Value) + ". " +
              "Si vuole sovrascrivere completamente?", "Attenzione",
              MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
              if (mf.DeleteAnagrafica(
                ((nodoClienteImportato.Attributes["RagioneSociale"] == null)
                  ? "" : nodoClienteImportato.Attributes["RagioneSociale"].Value)) == false)
              {
                di.Delete(true); return false;
              }
              //---------------------------------- si ricicla IDCliente attuale
              //IDCliente = mf.CheckEsistenzaCliente(ht);
              IDCliente = mf.GetCliente(ht);
            }
            else { di.Delete(true); return false; }
          }
          else
          {
            if (mf.DeleteAnagrafica(
              ((nodoClienteImportato.Attributes["RagioneSociale"] == null)
                ? "" : nodoClienteImportato.Attributes["RagioneSociale"].Value)) == false)
            {
              di.Delete(true); return false;
            }
            //------------------------------------ si ricicla IDCliente attuale
            //IDCliente = mf.CheckEsistenzaCliente(ht);
            IDCliente = mf.GetCliente(ht);
          }

        }
        // se non esisteva, e' stato aggiunto in mf.CheckEsistenzaCliente()
        /*-----------------------------------------------------------------------------
          else
          {
            mf.InsertClientChild(IDCliente, nodoClienteImportato);
          }
        -----------------------------------------------------------------------------*/
        //----------------------------------------------------------------------------+
        //                       aggiornamento BilancioVerifica                       |
        //----------------------------------------------------------------------------+
        XmlNode associazioniBilancio =
          _d.Document.SelectSingleNode("/ROOT/CLIENTE/BilancioVerifica");
        if (associazioniBilancio != null)
          mf.SetAnagraficaBV(IDCliente, associazioniBilancio);
        //----------------------------------------------------------------------------+
        //                    preparazione tabelle corrispondenza                     |
        //----------------------------------------------------------------------------+
        FileInfo fi;
        Hashtable IncaricoOldNew = new Hashtable();
        Hashtable ISQCOldNew = new Hashtable();
        Hashtable RevisioneOldNew = new Hashtable();
        Hashtable BilancioOldNew = new Hashtable();
        Hashtable ConclusioniOldNew = new Hashtable();
        Hashtable VerificaOldNew = new Hashtable();
        Hashtable VigilanzaOldNew = new Hashtable();
        Hashtable RelazioniBOldNew = new Hashtable();
        Hashtable RelazioniVOldNew = new Hashtable();
        Hashtable RelazioniBCOldNew = new Hashtable();
        Hashtable RelazioniVCOldNew = new Hashtable();
        Hashtable RelazioniBVOldNew = new Hashtable();
        Hashtable PianificazioniVerificaOldNew = new Hashtable();
        Hashtable PianificazioniVigilanzaOldNew = new Hashtable();
        FileInfo fnew;

        //----------------------------------------------------------------------------+
        //                           importazione INCARICO                            |
        //----------------------------------------------------------------------------+
        foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/INCARICO"))
        {
          node.Attributes["Cliente"].Value = IDCliente.ToString();
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["File"].Value);
          if (!fi.Exists) continue;
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["FileData"].Value);
          if (!fi.Exists) continue;
          string vecchio = node.Attributes["ID"].Value;
          string nuovo = mf.AddIncarico(node);
          IncaricoOldNew.Add(vecchio, nuovo);
          //------------------------------------------------- importazione alberi
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.impGuidFiles", conn);
            cmd.Parameters.AddWithValue("@parCliente", IDCliente.ToString());
            cmd.Parameters.AddWithValue("@parID", nuovo);
            cmd.Parameters.AddWithValue("@parFile", node.Attributes["File"].Value);
            cmd.Parameters.AddWithValue("@parFileData", node.Attributes["FileData"].Value);
            cmd.Parameters.AddWithValue("@parFolder", cartellaTmp);
            cmd.Parameters.AddWithValue("@parDocFolder", App.AppDocumentiFolder);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "importazione INCARICO: errore\n" + ex.Message;
                System.Windows.MessageBox.Show(msg);
              }
            }
          }
          if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        }

        //----------------------------------------------------------------------------+
        //                             importazione ISQC                              |
        //----------------------------------------------------------------------------+
        foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/ISQC"))
        {
          node.Attributes["Cliente"].Value = IDCliente.ToString();
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["File"].Value);
          if (!fi.Exists) continue;
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["FileData"].Value);
          if (!fi.Exists) continue;
          string vecchio = node.Attributes["ID"].Value;
          string nuovo = mf.AddISQC(node);
          ISQCOldNew.Add(vecchio, nuovo);
          //------------------------------------------------- importazione alberi
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.impGuidFiles", conn);
            cmd.Parameters.AddWithValue("@parCliente", IDCliente.ToString());
            cmd.Parameters.AddWithValue("@parID", nuovo);
            cmd.Parameters.AddWithValue("@parFile", node.Attributes["File"].Value);
            cmd.Parameters.AddWithValue("@parFileData", node.Attributes["FileData"].Value);
            cmd.Parameters.AddWithValue("@parFolder", cartellaTmp);
            cmd.Parameters.AddWithValue("@parDocFolder", App.AppDocumentiFolder);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "importazione ISQC: errore\n" + ex.Message;
                System.Windows.MessageBox.Show(msg);
              }
            }
          }
          if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        }

        //----------------------------------------------------------------------------+
        //                           importazione REVISIONE                           |
        //----------------------------------------------------------------------------+
        foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/REVISIONE"))
        {
          node.Attributes["Cliente"].Value = IDCliente.ToString();
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["File"].Value);
          if (!fi.Exists) continue;
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["FileData"].Value);
          if (!fi.Exists) continue;
          string vecchio = node.Attributes["ID"].Value;
          string nuovo = mf.AddRevisione(node);
          RevisioneOldNew.Add(vecchio, nuovo);
          XmlDataProviderManager _xaml = new XmlDataProviderManager(cartellaTmp + "\\" + node.Attributes["FileData"].Value, false);
          if (_xaml != null && _xaml.Document.SelectSingleNode("/Dati//Dato[@ID='274']") != null)
          {
            foreach (XmlNode tmpnode in _xaml.Document.SelectSingleNode("/Dati//Dato[@ID='274']").SelectNodes("Node[@xaml]"))
            {
              FileInfo fxamlhere = new FileInfo(cartellaTmp + tmpnode.Attributes["xaml"].Value.Replace("XAML\\", ""));
              if (!fxamlhere.Exists) tmpnode.Attributes.Remove(tmpnode.Attributes["xaml"]);
              else
              {
                DirectoryInfo dixaml = new DirectoryInfo(App.AppDataDataFolder + "\\XAML");
                if (!dixaml.Exists) dixaml.Create();
                fxamlhere.CopyTo(App.AppDataDataFolder + "\\XAML\\" + fxamlhere.Name, true);
              }
            }
            _xaml.Save_old();
          }
          //------------------------------------------------- importazione alberi
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.impGuidFiles", conn);
            cmd.Parameters.AddWithValue("@parCliente", IDCliente.ToString());
            cmd.Parameters.AddWithValue("@parID", nuovo);
            cmd.Parameters.AddWithValue("@parFile", node.Attributes["File"].Value);
            cmd.Parameters.AddWithValue("@parFileData", node.Attributes["FileData"].Value);
            cmd.Parameters.AddWithValue("@parFolder", cartellaTmp);
            cmd.Parameters.AddWithValue("@parDocFolder", App.AppDocumentiFolder);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "importazione REVISIONE: errore\n" + ex.Message;
                System.Windows.MessageBox.Show(msg);
              }
            }
          }
          if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        }

        //----------------------------------------------------------------------------+
        //                           importazione BILANCIO                            |
        //----------------------------------------------------------------------------+
        foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/BILANCIO"))
        {
          node.Attributes["Cliente"].Value = IDCliente.ToString();
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["File"].Value);
          if (!fi.Exists) continue;
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["FileData"].Value);
          if (!fi.Exists) continue;
          string vecchio = node.Attributes["ID"].Value;
          string nuovo = mf.AddBilancio(node);
          BilancioOldNew.Add(vecchio, nuovo);
          //------------------------------------------------- importazione alberi
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.impGuidFiles", conn);
            cmd.Parameters.AddWithValue("@parCliente", IDCliente.ToString());
            cmd.Parameters.AddWithValue("@parID", nuovo);
            cmd.Parameters.AddWithValue("@parFile", node.Attributes["File"].Value);
            cmd.Parameters.AddWithValue("@parFileData", node.Attributes["FileData"].Value);
            cmd.Parameters.AddWithValue("@parFolder", cartellaTmp);
            cmd.Parameters.AddWithValue("@parDocFolder", App.AppDocumentiFolder);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "importazione BILANCIO: errore\n" + ex.Message;
                System.Windows.MessageBox.Show(msg);
              }
            }
          }
          if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        }

        //----------------------------------------------------------------------------+
        //                          importazione CONCLUSIONE                          |
        //----------------------------------------------------------------------------+
        foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/CONCLUSIONE"))
        {
          node.Attributes["Cliente"].Value = IDCliente.ToString();
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["File"].Value);
          if (!fi.Exists) continue;
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["FileData"].Value);
          if (!fi.Exists) continue;
          string vecchio = node.Attributes["ID"].Value;
          string nuovo = mf.AddConclusione(node);
          ConclusioniOldNew.Add(vecchio, nuovo);
          //------------------------------------------------- importazione alberi
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.impGuidFiles", conn);
            cmd.Parameters.AddWithValue("@parCliente", IDCliente.ToString());
            cmd.Parameters.AddWithValue("@parID", nuovo);
            cmd.Parameters.AddWithValue("@parFile", node.Attributes["File"].Value);
            cmd.Parameters.AddWithValue("@parFileData", node.Attributes["FileData"].Value);
            cmd.Parameters.AddWithValue("@parFolder", cartellaTmp);
            cmd.Parameters.AddWithValue("@parDocFolder", App.AppDocumentiFolder);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "importazione CONCLUSIONE: errore\n" + ex.Message;
                System.Windows.MessageBox.Show(msg);
              }
            }
          }
          if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        }

        //----------------------------------------------------------------------------+
        //                            importazione FLUSSO                             |
        //----------------------------------------------------------------------------+
        foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/FLUSSO"))
        {
          node.Attributes["Cliente"].Value = IDCliente.ToString();
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["FileData"].Value);
          if (!fi.Exists) continue;
          XmlDataProviderManager _fa =
            new XmlDataProviderManager(cartellaTmp + "\\" + node.Attributes["FileData"].Value, false);
          string xpath = "//Allegato";
          string directory = App.AppDocumentiFolder + "\\Flussi";
          string directorytmp = cartellaTmp + "\\Flussi";
          foreach (XmlNode item in _fa.Document.SelectNodes(xpath))
          {
            FileInfo f_fa = new FileInfo(directorytmp + "\\" + item.Attributes["FILE"].Value);
            if (f_fa.Exists)
            {
              DirectoryInfo newdi = new DirectoryInfo(directory);
              if (newdi.Exists == false) newdi.Create();
              int HSIDHERE = Convert.ToInt32(item.Attributes["FILE"].Value.Split('.')[0]);
              string EXTENSIONHERE = item.Attributes["FILE"].Value.Split('.')[1];
              FileInfo f_d = new FileInfo(directory + "\\" + HSIDHERE.ToString() + "." + EXTENSIONHERE);
              while (f_d.Exists)
              {
                HSIDHERE++;
                f_d = new FileInfo(directory + "\\" + HSIDHERE.ToString() + "." + EXTENSIONHERE);
              }
              f_fa.CopyTo(directory + "\\" + HSIDHERE.ToString() + "." + EXTENSIONHERE, true);
              item.Attributes["FILE"].Value = HSIDHERE.ToString() + "." + EXTENSIONHERE;
            }
          }
          _fa.Save_old();
          //------------------------------------------------- importazione albero
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.impGuidFiles", conn);
            cmd.Parameters.AddWithValue("@parCliente", IDCliente.ToString());
            cmd.Parameters.AddWithValue("@parID", "flussi");
            cmd.Parameters.AddWithValue("@parFile", "");
            cmd.Parameters.AddWithValue("@parFileData", node.Attributes["FileData"].Value);
            cmd.Parameters.AddWithValue("@parFolder", cartellaTmp);
            cmd.Parameters.AddWithValue("@parDocFolder", App.AppDocumentiFolder);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "importazione FLUSSO: errore\n" + ex.Message;
                System.Windows.MessageBox.Show(msg);
              }
            }
          }
          if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        }

        //----------------------------------------------------------------------------+
        //                          importazione RELAZIONEB                           |
        //----------------------------------------------------------------------------+
        foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/RELAZIONEB"))
        {
          node.Attributes["Cliente"].Value = IDCliente.ToString();
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["File"].Value);
          if (!fi.Exists) continue;
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["FileData"].Value);
          if (!fi.Exists) continue;
          string vecchio = node.Attributes["ID"].Value;
          string nuovo = mf.AddRelazioneB(node);
          RelazioniBOldNew.Add(vecchio, nuovo);
          //------------------------------------------------- importazione alberi
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.impGuidFiles", conn);
            cmd.Parameters.AddWithValue("@parCliente", IDCliente.ToString());
            cmd.Parameters.AddWithValue("@parID", nuovo);
            cmd.Parameters.AddWithValue("@parFile", node.Attributes["File"].Value);
            cmd.Parameters.AddWithValue("@parFileData", node.Attributes["FileData"].Value);
            cmd.Parameters.AddWithValue("@parFolder", cartellaTmp);
            cmd.Parameters.AddWithValue("@parDocFolder", App.AppDocumentiFolder);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "importazione RELAZIONEB: errore\n" + ex.Message;
                System.Windows.MessageBox.Show(msg);
              }
            }
          }
          if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        }

        //----------------------------------------------------------------------------+
        //                          importazione RELAZIONEBC                          |
        //----------------------------------------------------------------------------+
        foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/RELAZIONEBC"))
        {
          node.Attributes["Cliente"].Value = IDCliente.ToString();
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["File"].Value);
          if (!fi.Exists) continue;
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["FileData"].Value);
          if (!fi.Exists) continue;
          string vecchio = node.Attributes["ID"].Value;
          string nuovo = mf.AddRelazioneBC(node);
          RelazioniBCOldNew.Add(vecchio, nuovo);
          //------------------------------------------------- importazione alberi
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.impGuidFiles", conn);
            cmd.Parameters.AddWithValue("@parCliente", IDCliente.ToString());
            cmd.Parameters.AddWithValue("@parID", nuovo);
            cmd.Parameters.AddWithValue("@parFile", node.Attributes["File"].Value);
            cmd.Parameters.AddWithValue("@parFileData", node.Attributes["FileData"].Value);
            cmd.Parameters.AddWithValue("@parFolder", cartellaTmp);
            cmd.Parameters.AddWithValue("@parDocFolder", App.AppDocumentiFolder);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "importazione RELAZIONEBC: errore\n" + ex.Message;
                System.Windows.MessageBox.Show(msg);
              }
            }
          }
          if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        }

        //----------------------------------------------------------------------------+
        //                          importazione RELAZIONEV                           |
        //----------------------------------------------------------------------------+
        foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/RELAZIONEV"))
        {
          node.Attributes["Cliente"].Value = IDCliente.ToString();
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["File"].Value);
          if (!fi.Exists) continue;
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["FileData"].Value);
          if (!fi.Exists) continue;
          string vecchio = node.Attributes["ID"].Value;
          string nuovo = mf.AddRelazioneV(node);
          RelazioniVOldNew.Add(vecchio, nuovo);
          //------------------------------------------------- importazione alberi
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.impGuidFiles", conn);
            cmd.Parameters.AddWithValue("@parCliente", IDCliente.ToString());
            cmd.Parameters.AddWithValue("@parID", nuovo);
            cmd.Parameters.AddWithValue("@parFile", node.Attributes["File"].Value);
            cmd.Parameters.AddWithValue("@parFileData", node.Attributes["FileData"].Value);
            cmd.Parameters.AddWithValue("@parFolder", cartellaTmp);
            cmd.Parameters.AddWithValue("@parDocFolder", App.AppDocumentiFolder);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "importazione RELAZIONEV: errore\n" + ex.Message;
                System.Windows.MessageBox.Show(msg);
              }
            }
          }
          if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        }

        //----------------------------------------------------------------------------+
        //                          importazione RELAZIONEVC                          |
        //----------------------------------------------------------------------------+
        foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/RELAZIONEVC"))
        {
          node.Attributes["Cliente"].Value = IDCliente.ToString();
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["File"].Value);
          if (!fi.Exists) continue;
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["FileData"].Value);
          if (!fi.Exists) continue;
          string vecchio = node.Attributes["ID"].Value;
          string nuovo = mf.AddRelazioneVC(node);
          RelazioniVCOldNew.Add(vecchio, nuovo);
          //------------------------------------------------- importazione alberi
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.impGuidFiles", conn);
            cmd.Parameters.AddWithValue("@parCliente", IDCliente.ToString());
            cmd.Parameters.AddWithValue("@parID", nuovo);
            cmd.Parameters.AddWithValue("@parFile", node.Attributes["File"].Value);
            cmd.Parameters.AddWithValue("@parFileData", node.Attributes["FileData"].Value);
            cmd.Parameters.AddWithValue("@parFolder", cartellaTmp);
            cmd.Parameters.AddWithValue("@parDocFolder", App.AppDocumentiFolder);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "importazione RELAZIONEVC: errore\n" + ex.Message;
                System.Windows.MessageBox.Show(msg);
              }
            }
          }
          if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        }

        //----------------------------------------------------------------------------+
        //                          importazione RELAZIONEBV                          |
        //----------------------------------------------------------------------------+
        foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/RELAZIONEBV"))
        {
          node.Attributes["Cliente"].Value = IDCliente.ToString();
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["File"].Value);
          if (!fi.Exists) continue;
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["FileData"].Value);
          if (!fi.Exists) continue;
          string vecchio = node.Attributes["ID"].Value;
          string nuovo = mf.AddRelazioneBV(node);
          RelazioniBVOldNew.Add(vecchio, nuovo);
          //------------------------------------------------- importazione alberi
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.impGuidFiles", conn);
            cmd.Parameters.AddWithValue("@parCliente", IDCliente.ToString());
            cmd.Parameters.AddWithValue("@parID", nuovo);
            cmd.Parameters.AddWithValue("@parFile", node.Attributes["File"].Value);
            cmd.Parameters.AddWithValue("@parFileData", node.Attributes["FileData"].Value);
            cmd.Parameters.AddWithValue("@parFolder", cartellaTmp);
            cmd.Parameters.AddWithValue("@parDocFolder", App.AppDocumentiFolder);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "importazione RELAZIONEBV: errore\n" + ex.Message;
                System.Windows.MessageBox.Show(msg);
              }
            }
          }
          if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        }

        //----------------------------------------------------------------------------+
        //                           importazione VERIFICA                            |
        //----------------------------------------------------------------------------+
        foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/VERIFICA"))
        {
          node.Attributes["Cliente"].Value = IDCliente.ToString();
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["File"].Value);
          if (!fi.Exists) continue;
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["FileData"].Value);
          if (!fi.Exists) continue;
          string vecchio = node.Attributes["ID"].Value;
          string nuovo = mf.AddVerifica(node);
          VerificaOldNew.Add(vecchio, nuovo);
          //------------------------------------------------- importazione alberi
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.impGuidFiles", conn);
            cmd.Parameters.AddWithValue("@parCliente", IDCliente.ToString());
            cmd.Parameters.AddWithValue("@parID", nuovo);
            cmd.Parameters.AddWithValue("@parFile", node.Attributes["File"].Value);
            cmd.Parameters.AddWithValue("@parFileData", node.Attributes["FileData"].Value);
            cmd.Parameters.AddWithValue("@parFolder", cartellaTmp);
            cmd.Parameters.AddWithValue("@parDocFolder", App.AppDocumentiFolder);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "importazione VERIFICA: errore\n" + ex.Message;
                System.Windows.MessageBox.Show(msg);
              }
            }
          }
          if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        }

        //----------------------------------------------------------------------------+
        //                           importazione VIGILANZA                           |
        //----------------------------------------------------------------------------+
        foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/VIGILANZA"))
        {
          node.Attributes["Cliente"].Value = IDCliente.ToString();
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["File"].Value);
          if (!fi.Exists) continue;
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["FileData"].Value);
          if (!fi.Exists) continue;
          string vecchio = node.Attributes["ID"].Value;
          string nuovo = mf.AddVigilanza(node);
          VigilanzaOldNew.Add(vecchio, nuovo);
          //------------------------------------------------- importazione alberi
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.impGuidFiles", conn);
            cmd.Parameters.AddWithValue("@parCliente", IDCliente.ToString());
            cmd.Parameters.AddWithValue("@parID", nuovo);
            cmd.Parameters.AddWithValue("@parFile", node.Attributes["File"].Value);
            cmd.Parameters.AddWithValue("@parFileData", node.Attributes["FileData"].Value);
            cmd.Parameters.AddWithValue("@parFolder", cartellaTmp);
            cmd.Parameters.AddWithValue("@parDocFolder", App.AppDocumentiFolder);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "importazione VIGILANZA: errore\n" + ex.Message;
                System.Windows.MessageBox.Show(msg);
              }
            }
          }
          if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        }

        //----------------------------------------------------------------------------+
        //                    importazione PIANIFICAZIONIVERIFICA                     |
        //----------------------------------------------------------------------------+
        foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICA"))
        {
          node.Attributes["Cliente"].Value = IDCliente.ToString();
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["File"].Value);
          if (!fi.Exists) continue;
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["FileData"].Value);
          if (!fi.Exists) continue;
          string vecchio = node.Attributes["ID"].Value;
          string nuovo = mf.AddPianificazioniVerifica(node);
          PianificazioniVerificaOldNew.Add(vecchio, nuovo);
          //------------------------------------------------- importazione alberi
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.impGuidFiles", conn);
            cmd.Parameters.AddWithValue("@parCliente", IDCliente.ToString());
            cmd.Parameters.AddWithValue("@parID", nuovo);
            cmd.Parameters.AddWithValue("@parFile", node.Attributes["File"].Value);
            cmd.Parameters.AddWithValue("@parFileData", node.Attributes["FileData"].Value);
            cmd.Parameters.AddWithValue("@parFolder", cartellaTmp);
            cmd.Parameters.AddWithValue("@parDocFolder", App.AppDocumentiFolder);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "importazione PIANIFICAZIONIVERIFICA: errore\n" + ex.Message;
                System.Windows.MessageBox.Show(msg);
              }
            }
          }
          if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        }

        //----------------------------------------------------------------------------+
        //                    importazione PIANIFICAZIONIVIGILANZA                    |
        //----------------------------------------------------------------------------+
        foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZA"))
        {
          node.Attributes["Cliente"].Value = IDCliente.ToString();
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["File"].Value);
          if (!fi.Exists) continue;
          fi = new FileInfo(cartellaTmp + "\\" + node.Attributes["FileData"].Value);
          if (!fi.Exists) continue;
          string vecchio = node.Attributes["ID"].Value;
          string nuovo = mf.AddPianificazioniVigilanza(node);
          PianificazioniVigilanzaOldNew.Add(vecchio, nuovo);
          //------------------------------------------------- importazione alberi
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("dbo.impGuidFiles", conn);
            cmd.Parameters.AddWithValue("@parCliente", IDCliente.ToString());
            cmd.Parameters.AddWithValue("@parID", nuovo);
            cmd.Parameters.AddWithValue("@parFile", node.Attributes["File"].Value);
            cmd.Parameters.AddWithValue("@parFileData", node.Attributes["FileData"].Value);
            cmd.Parameters.AddWithValue("@parFolder", cartellaTmp);
            cmd.Parameters.AddWithValue("@parDocFolder", App.AppDocumentiFolder);
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "importazione PIANIFICAZIONIVIGILANZA: errore\n" + ex.Message;
                System.Windows.MessageBox.Show(msg);
              }
            }
          }
          if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        }

        //----------------------------------------------------------------------------+
        //                           importazione DOCUMENTO                           |
        //----------------------------------------------------------------------------+
        XmlDataProviderManager _dd =
          new XmlDataProviderManager(App.AppDocumentiDataFile, true);
        bool tobesaved = false;
        foreach (XmlNode documento in _d.Document.SelectNodes("/ROOT/DOCUMENTO"))
        {
          XmlNode root = _dd.Document.SelectSingleNode("//DOCUMENTI");
          int newID = Convert.ToInt32(root.Attributes["LastID"].Value) + 1;
          // file di origine
          FileInfo ff = new FileInfo(cartellaTmp + @"\" + documento.Attributes["File"].Value);
          if (ff.Exists)
          {
            string nomefile = newID.ToString() + "." + documento.Attributes["File"].Value.Split('.').Last();
            // file di destinazione
            fnew = new FileInfo(App.AppDocumentiFolder + @"\" + nomefile);
            if (fnew.Exists) fnew.Delete();
            ff.CopyTo(App.AppDocumentiFolder + @"\" + nomefile);
            string trueSessione = (documento.Attributes["Sessione"] == null)
              ? "-1" : documento.Attributes["Sessione"].Value;
            if (trueSessione != "-1")
            {
              App.TipoFile Tree =
                ((App.TipoFile)(Convert.ToInt32(documento.Attributes["Tree"].Value)));
              switch (Tree)
              {
                case App.TipoFile.Incarico:
                case App.TipoFile.IncaricoCS:
                case App.TipoFile.IncaricoSU:
                case App.TipoFile.IncaricoREV:
                  if (IncaricoOldNew.Contains(trueSessione))
                    trueSessione = IncaricoOldNew[trueSessione].ToString();
                  break;
                case App.TipoFile.ISQC:
                  if (ISQCOldNew.Contains(trueSessione))
                    trueSessione = ISQCOldNew[trueSessione].ToString();
                  break;
                case App.TipoFile.Revisione:
                  if (RevisioneOldNew.Contains(trueSessione))
                    trueSessione = RevisioneOldNew[trueSessione].ToString();
                  break;
                case App.TipoFile.Bilancio:
                  if (BilancioOldNew.Contains(trueSessione))
                    trueSessione = BilancioOldNew[trueSessione].ToString();
                  break;
                case App.TipoFile.Conclusione:
                  if (ConclusioniOldNew.Contains(trueSessione))
                    trueSessione = ConclusioniOldNew[trueSessione].ToString();
                  break;
                case App.TipoFile.Verifica:
                  if (VerificaOldNew.Contains(trueSessione))
                    trueSessione = VerificaOldNew[trueSessione].ToString();
                  break;
                case App.TipoFile.Vigilanza:
                  if (VigilanzaOldNew.Contains(trueSessione))
                    trueSessione = VigilanzaOldNew[trueSessione].ToString();
                  break;
                case App.TipoFile.Flussi:
                  break;
                case App.TipoFile.PianificazioniVerifica:
                  if (PianificazioniVerificaOldNew.Contains(trueSessione))
                    trueSessione = PianificazioniVerificaOldNew[trueSessione].ToString();
                  break;
                case App.TipoFile.PianificazioniVigilanza:
                  if (PianificazioniVigilanzaOldNew.Contains(trueSessione))
                    trueSessione = PianificazioniVigilanzaOldNew[trueSessione].ToString();
                  break;
                case App.TipoFile.RelazioneB:
                  if (RelazioniBOldNew.Contains(trueSessione))
                    trueSessione = RelazioniBOldNew[trueSessione].ToString();
                  break;
                case App.TipoFile.RelazioneBC:
                  if (RelazioniBCOldNew.Contains(trueSessione))
                    trueSessione = RelazioniBCOldNew[trueSessione].ToString();
                  break;
                case App.TipoFile.RelazioneV:
                  if (RelazioniVOldNew.Contains(trueSessione))
                    trueSessione = RelazioniVOldNew[trueSessione].ToString();
                  break;
                case App.TipoFile.RelazioneVC:
                  if (RelazioniVCOldNew.Contains(trueSessione))
                    trueSessione = RelazioniVCOldNew[trueSessione].ToString();
                  break;
                case App.TipoFile.RelazioneBV:
                  if (RelazioniBVOldNew.Contains(trueSessione))
                    trueSessione = RelazioniBVOldNew[trueSessione].ToString();
                  break;
                default:
                  break;
              }
            }
            string xml = "<DOCUMENTO ID=\"" + newID.ToString() + "\" Cliente=\"" + IDCliente + "\" Sessione=\"" + trueSessione + "\" Tree=\"" + ((documento.Attributes["Tree"] == null) ? "" : documento.Attributes["Tree"].Value) + "\" Nodo=\"" + ((documento.Attributes["Nodo"] == null) ? "" : documento.Attributes["Nodo"].Value).Replace("&", "&amp;").Replace("\"", "'") + "\" Tipo=\"" + ((documento.Attributes["Tipo"] == null) ? "" : documento.Attributes["Tipo"].Value) + "\" Titolo=\"" + ((documento.Attributes["Titolo"] == null) ? "" : documento.Attributes["Titolo"].Value).ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Descrizione=\"" + ((documento.Attributes["Descrizione"] == null) ? "" : documento.Attributes["Descrizione"].Value).ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" File=\"" + nomefile + "\" Visualizza=\"True\" />";
            XmlDocument doctmp = new XmlDocument();
            doctmp.LoadXml(xml);
            XmlNode tmpNode = doctmp.SelectSingleNode("/DOCUMENTO");
            XmlNode node = _dd.Document.ImportNode(tmpNode, true);
            root.AppendChild(node);
            root.Attributes["LastID"].Value = newID.ToString();
            tobesaved = true;
            using (SqlConnection conn = new SqlConnection(App.connString))
            {
              conn.Open();
              SqlCommand cmd = new SqlCommand("doc.NewDocumento", conn);
              cmd.Parameters.AddWithValue("@rec", tmpNode.OuterXml);
              cmd.CommandType = CommandType.StoredProcedure;
              //cmd.CommandTimeout = App.m_CommandTimeout;
              try { cmd.ExecuteNonQuery(); }
              catch (Exception ex)
              {
                if (!App.m_bNoExceptionMsg)
                {
                  string msg = "SQL call 'doc.NewDocumento' failed: errore\n" + ex.Message;
                  System.Windows.MessageBox.Show(msg);
                }
              }
            }
          }
        }
        if (tobesaved)
        {
          _dd.Save();
          if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
        }
        _d.Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
        di.Delete(true);
        mf.SplitVerificheVigilanze();
        mf.UpdateTipoEsercisioSu239();

        return true;
      }
      catch (Exception ex)
      {

        di.Delete(true);
        System.Windows.MessageBox.Show(ex.Message);
        return false;
      }
    }

    //------------------------------------------------------------------------+
    //                             SetLockStatus                              |
    //------------------------------------------------------------------------+
    static public void SetLockStatus(string guid = null, string codice = null, bool toBlock = true)
    {
      bool argNull, argEmpty;

      argNull = guid == null || codice == null; if (argNull) return;
      argEmpty = string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(codice);
      if (argEmpty && toBlock) return;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand(
            toBlock ? "dbo.SetItemLock" : "dbo.SetItemUnlock",
            conn);
        cmd.Parameters.AddWithValue("@itemGuid", guid.Split('\\').Last());
        //cmd.Parameters.AddWithValue("@userGuid", Environment.UserName);
        cmd.Parameters.AddWithValue("@userGuid", (App.AppTipo == App.ModalitaApp.Team) ? App.AppUtente.Login : Environment.UserName);
        cmd.Parameters.AddWithValue("@codice", codice);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        var retVal = cmd.Parameters.Add("@retVal", SqlDbType.Int);
        retVal.Direction = ParameterDirection.ReturnValue;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = (toBlock ? "dbo.SetItemLock" : "dbo.SetItemUnlock") + ": errore\n" + ex.Message;
            System.Windows.MessageBox.Show(msg);
          }
        }
        if (!toBlock)
        {
          int res = (int)retVal.Value;
          if (res != 0)
          {
            if (App.m_xmlCache.Contains(guid)) App.m_xmlCache.Remove(guid);
          }
        }
      }
    }
  }
  //============================================================================+
  //                                  INIFile                                   |
  //============================================================================+
  class INIFile
  {
    private string filePath;

    [System.Runtime.InteropServices.DllImport("kernel32")]
    private static extern long WritePrivateProfileString(
      string section, string key, string val, string filePath);

    [System.Runtime.InteropServices.DllImport("kernel32")]
    private static extern int GetPrivateProfileString(
      string section, string key, string def, StringBuilder retVal,
      int size, string filePath);

    //----------------------------------------------------------------------------+
    //                                  INIFile                                   |
    //----------------------------------------------------------------------------+
    public INIFile(string filePath) { this.filePath = filePath; }

    //----------------------------------------------------------------------------+
    //                                   Write                                    |
    //----------------------------------------------------------------------------+
    public void Write(string section, string key, string value)
    {
      WritePrivateProfileString(section, key, value, filePath);
    }

    //----------------------------------------------------------------------------+
    //                                    Read                                    |
    //----------------------------------------------------------------------------+
    public string Read(string section, string key)
    {
      StringBuilder SB = new StringBuilder(255);
      int i = GetPrivateProfileString(section, key, "", SB, 255, filePath);
      return SB.ToString();
    }

    public string FilePath
    {
      get { return filePath; }
      set { filePath = value; }
    }
  } //----------------------------------------------------------- class INIFile
  static class SqlHelper
  {
    public static IEnumerable<string> ListLocalSqlInstances()
    {
      if (Environment.Is64BitOperatingSystem)
      {
        using (var hive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
        {
          foreach (string item in ListLocalSqlInstances(hive))
          {
            yield return item;
          }
        }

        using (var hive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
        {
          foreach (string item in ListLocalSqlInstances(hive))
          {
            yield return item;
          }
        }
      }
      else
      {
        foreach (string item in ListLocalSqlInstances(Registry.LocalMachine))
        {
          yield return item;
        }
      }
    }

    private static IEnumerable<string> ListLocalSqlInstances(RegistryKey hive)
    {
      const string keyName = @"Software\Microsoft\Microsoft SQL Server";
      const string valueName = "InstalledInstances";
      const string defaultName = "MSSQLSERVER";

      using (var key = hive.OpenSubKey(keyName, false))
      {
        if (key == null) return Enumerable.Empty<string>();

        var value = key.GetValue(valueName) as string[];
        if (value == null) return Enumerable.Empty<string>();

        for (int index = 0; index < value.Length; index++)
        {
          if (string.Equals(value[index], defaultName, StringComparison.OrdinalIgnoreCase))
          {
            value[index] = ".";
          }
          else
          {
            value[index] = @".\" + value[index];
          }
        }

        return value;
      }
    }
  }

  

  class Utilities
  {
    revisoftWS rw = new revisoftWS();

    //Costanti
    private const string PC_INFO_SCONOSCIUTA = "000000000000";

    //variabili: PC
    private static string _IdProcessore;
    private static string _MacAddress;
    private static string _HDSerial;

    //variabili: Registro
    private static bool _PrimaInstallazione;

    //variabili: Aggiornamento software
    private static string _NomeNuovaVeresione;
    private static string _NomeComandoAggiornamento;



    //Gestione file system
    private string[] _TipoEstensioniFile = { ".rlf", ".rrdf", ".rvdf", ".ridf", ".rbdf", ".rmdf", ".rif", ".rmf", ".rief", ".riet", ".rbkf", ".rfdf", ".rmodf", ".rdocf", ".rsdf", ".rdoct", ".rxml", ".xbrl", ".radf", ".rcdf", ".rslf", ".redf", ".redf", ".redf", ".rlogf", ".rixtf", ".rpdf", ".rpdf", ".rqdf", ".xls", ".redf", ".redf", ".redf" };
    private string[] _TipoEstensioniFile_Filtri = { "Licenza (.rlf)|*.rlf",
                                                                "Revisione (.rrdf)|*.rrdf",
                                                                "Verifica (.rvdf)|*.rvdf",
                                                                "Incarico (.ridf)|*.ridf",
                                "Bilancio (.rbdf)|*.rbdf",
                                                                "Archivio Master (.rmdf)|*.rmdf",
                                                                "Configurazione (.rif)|*.rif",
                                                                "Messaggi (.rmf)|*.rmf",
                                "Importazione/Espostazione (.rief)|*.rief",
                                                                "Importazione template (.riet)|*.riet",
                                                                "BackUp (.rbkf)|*.rbkf",
                                                                "Formulario (.rfdf)|*.rfdf",
                                                                "Modelli predefiniti (.rmodf)|*.rmodf",
                                                                "Documenti associati (.rdocf)|*.rdocf",
                                                                "Scambio dati (.rsdf)|*.rsdf",
                                                                "Template Doc (.rdoct)|*.rdoct",
                                "Revisoft XML (.rxml)|*.rxml",
                                "XBRL (.xbrl)|*.xbrl",
                                                                "Vigilanza (.radf)|*.radf",
                                                                "Conclusioni (.rcdf)|*.rcdf",
                                                                "Sigillo (.rslf)|*.rslf",
                                                                "Relazione Bilancio (.redf)|*.redf",
                                                                "Relazione Vigilanza (.redf)|*.redf",
                                                                "Relazione Bilancio e Vigilanza (.redf)|*.redf",
                                                                "Log (.rlogf)|*.rlogf",
                                                                "Indice Template (.rixtf)|*.rixtf",
                                                                "Pianificazione Verifica (.rpdf)|*.rpdf",
                                                                "Pianificazione Vigilanza (.rpdf)|*.rpdf",
                                                                "ISQC (.rqdf)|*.rqdf",
                                                                "Excel|*.xls;*.xlsx",
                                                                "Relazione Bilancio Consolidato (.redf)|*.redf",
                                                                "Relazione Vigilanza Consolidato (.redf)|*.redf"
                                                                };


    //Proprietà: Info PC
    public string IdProcessore
    {
      get { return (string)_IdProcessore; }
    }
    public string MacAddress
    {
      get { return (string)_MacAddress; }
    }
    public string HDSerial
    {
      get { return (string)_HDSerial; }
    }
    //Proprietà registro
    public bool PrimaInstallazione
    {
      get { return (bool)_PrimaInstallazione; }
    }
    //Proprietà aggiornamento software
    public string NomeNuovaVeresione
    {
      get { return (string)_NomeNuovaVeresione; }
    }
    public string NomeComandoAggiornamento
    {
      get { return (string)_NomeComandoAggiornamento; }
    }


    #region USCITA_APPLICAZIONE
    //Metodi
    public bool ConfermaUscita()
    {

      bool ApplicazioneInChiusura = System.Windows.MessageBox.Show("Confermi l'uscita?", "Revisoft", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
      return ApplicazioneInChiusura;
    }


    public void ChiudiApplicazioneSuErrore()
    {
      //Esci
      System.Windows.Application.Current.Shutdown();
      System.Environment.Exit(0);
      Environment.Exit(0);
      Process.GetCurrentProcess().Kill();
    }

    public void ChiudiApplicazione()
    {
      //LOG
      SalvaLog();

      //Esci
      System.Windows.Application.Current.Shutdown();
      System.Environment.Exit(0);
      Environment.Exit(0);
      Process.GetCurrentProcess().Kill();
    }

    public void ChiudiApplicazioneConAggiornamento(string cmd)
    {
      //eseguo comando di aggiornamento
      //System.Diagnostics.Process.Start(cmd);
      ProcessStartInfo p = new ProcessStartInfo(cmd);
      p.WindowStyle = ProcessWindowStyle.Minimized;
      Process.Start(p);

      //Esci
      System.Windows.Application.Current.Shutdown();
      System.Environment.Exit(0);
      Environment.Exit(0);
      Process.GetCurrentProcess().Kill();
    }

    public void ChiudiApplicazioneConConferma()
    {
      if (ConfermaUscita())
        ChiudiApplicazione();
    }

    public bool ChiudiApplicazioneConBackup()
    {
      bool returnvalue = false;

      if (true) { ChiudiApplicazione(); return false; }

      //4.5
      ////vers. 3.3 - consento backup se archivio locale
      //App.AppConsentiBackUp = (App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Locale);


      //licenza abilitata alla gestione dei BackUp
      //if (App.AppConsentiBackUp)
      //{
      //CheckConsistenzaDocumenti();

      //Conferma
      Process thisProc = Process.GetCurrentProcess();
      if (Process.GetProcessesByName(thisProc.ProcessName).Length <= 1)
      {
        //4.6
        wAddio saluti = new wAddio();
        saluti.ShowDialog();

        if (saluti.ChiudiRevisoft)
        {
          ChiudiApplicazione();
        }
        else
        {
          returnvalue = true;
        }

        //4.6
        //switch ( MessageBox.Show( "E' vivamente consigliabile effettuare un backup del sistema prima di uscire.\n\nProcedo con il salvataggio?", "Attenzione", MessageBoxButton.YesNoCancel ) )
        //{
        //    case MessageBoxResult.Yes:
        //        ProgressWindow pw = new ProgressWindow();
        //        //Backup
        //        BackUpFile bkf = new BackUpFile();
        //        Hashtable ht = new Hashtable();
        //        bkf.SetBackUp( ht, -1 );
        //        //Process wait - STOP
        //        pw.Close();
        //        ChiudiApplicazione();
        //        break;
        //    case MessageBoxResult.No:
        //        ChiudiApplicazione();
        //        break;
        //    case MessageBoxResult.Cancel:
        //    default:
        //        returnvalue = true;
        //        break;
        //}
      }
      //}
      //else
      //    //andrea - licenze non abilitate al backup (satellite e client rete)
      //    if (ConfermaUscita())
      //        ChiudiApplicazione();
      //    else
      //        returnvalue = true;

      return returnvalue;
    }

    //----------------------------------------------------------------------------+
    //                         CheckConsistenzaDocumenti                          |
    //----------------------------------------------------------------------------+
    private void CheckConsistenzaDocumenti()
    {
      bool isModified = false; // E.B.
      XmlDataProviderManager _x = new XmlDataProviderManager(App.AppDocumentiDataFile, true);
      XmlNodeList nodelist = _x.Document.SelectNodes("//DOCUMENTI//DOCUMENTO");
      ArrayList NodiDaCancellare = new ArrayList();
      foreach (XmlNode node in nodelist)
      {
        MasterFile mf = MasterFile.Create();
        Hashtable ht = new Hashtable();
        string Sessione = node.Attributes["Sessione"].Value;
        string Tree = node.Attributes["Tree"].Value;
        try
        {
          switch ((App.TipoFile)(System.Convert.ToInt32(Tree)))
          {
            case App.TipoFile.Revisione:
              ht = mf.GetRevisione(Sessione);
              break;
            case App.TipoFile.Verifica:
              ht = mf.GetVerifica(Sessione);
              break;
            case App.TipoFile.Incarico:
            case App.TipoFile.IncaricoCS:
            case App.TipoFile.IncaricoSU:
            case App.TipoFile.IncaricoREV:
              ht = mf.GetIncarico(Sessione);
              break;
            case App.TipoFile.ISQC:
              ht = mf.GetISQC(Sessione);
              break;
            case App.TipoFile.Bilancio:
              ht = mf.GetBilancio(Sessione);
              break;
            case App.TipoFile.Vigilanza:
              ht = mf.GetVigilanza(Sessione);
              break;
            default:
              break;
          }
        }
        catch (Exception ex)
        {
          string lof = ex.Message;
        }
        if (ht == null || (ht != null && ht.Count == 0))
        {
          NodiDaCancellare.Add(node);
        }
      }
      string directory = App.AppDocumentiFolder;
      foreach (XmlNode item in NodiDaCancellare)
      {
        var file = directory + "\\" + item.Attributes["File"].Value;
        FileInfo fitmp = new FileInfo(file);
        if (fitmp.Exists)
        {
          fitmp.Delete();
        }
        item.ParentNode.RemoveChild(item);
        StaticUtilities.MarkNodeAsModified(item, App.OBJ_MOD); isModified = true; // E.B.
      }
      _x.isModified = isModified; // E.B.
      _x.Save();
    }

    #endregion

    #region REGISTRO_DI_SISTEMA

    public void ConfiguraRegistroAttivazioneLicenzaProva()
    {
      //string regFolder = "SOFTWARE\\Microsoft\\DRDF";
      ////Chiave applicazione HKEY_LOCAL_MACHINE\SOFTWARE\Revisoft\Revisoft
      //RegistryKey key = Registry.LocalMachine.CreateSubKey(regFolder);
      //key.Close();
      ////Salvo il valore in  HKEY_LOCAL_MACHINE\SOFTWARE\Revisoft\Revisoft -> DL_Info
      //key = Registry.LocalMachine.OpenSubKey(regFolder, true);
      //Gestione Cripto dato
      RevisoftApplication.XmlManager x = new XmlManager();
      x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Avanzata;
      //DataInstallazione applicazione
      string dataInstallazione = DateTime.Now.ToShortDateString();
      string dataInstallazioneEncoded = x.EncodeString(dataInstallazione).ToString();

      GestioneLicenza gl = new GestioneLicenza();
      gl.SetFromInfo("DLDataPath", dataInstallazioneEncoded);

      //Creo valore HKEY_LOCAL_MACHINE\SOFTWARE\Revisoft\Revisoft -> DL_Info
      // key.SetValue("DLDataPath", dataInstallazioneEncoded);
      //Setto proprietà prima installazione: consente attivazione licenza di prova
      _PrimaInstallazione = true;
    }

    public void ConfiguraRegistroAttivazioneLicenzaProvaReset()
    {
      GestioneLicenza gl = new GestioneLicenza();
      gl.SetFromInfo("DLDataPath", "");
      _PrimaInstallazione = false;

      //string regFolder = "SOFTWARE\\Microsoft\\DRDF";
      ////verifico presenza e cancello chiave
      //RegistryKey key = Registry.LocalMachine.OpenSubKey(regFolder, true);
      //if (key != null)
      //    Registry.LocalMachine.DeleteSubKey(regFolder);
    }

    public void VerificaRegistroAttivazioneLicenzaProva()
    {
      string setupData = "";
      GestioneLicenza gl = new GestioneLicenza();
      setupData = gl.GetFromInfo("DataCreazioneLicenzaProva");

      //verifico valore, se prima installazione se già effettuato non consento attivazione licenza di prova
      if (setupData == "")
        _PrimaInstallazione = true;
      else
        _PrimaInstallazione = false;


      //string regFolder = "SOFTWARE\\Microsoft\\DRDF";

      ////Verifico valore  
      //RegistryKey key = Registry.LocalMachine.OpenSubKey(regFolder, true);

      //string setupData = "";
      //if (key != null)
      //{
      //    //leggo il valore
      //    setupData = key.GetValue("DLDataPath", "").ToString();
      //}
    }

    //ANDREA 4.5 ELIMINARE
    public void ConfiguraRegistroApplicazione()
    {
      ////SETUP - Archivio locale
      //SetRegistroChiaveApplicazione(App.Registry_TipoGestioneArchivio.ToString(), App.TipoGestioneArchivio.Locale.ToString());
      ////Benvenuto
      //SetRegistroChiaveApplicazione(App.Registry_Benvenuto.ToString(), true.ToString());
      ////Istruzioni automatiche
      //SetRegistroChiaveApplicazione(App.Registry_IstruzioniAutomatiche.ToString(), true.ToString());
    }

    //ANDREA 4.5 ELIMINARE
    public void ConfiguraRegistroApplicazioneEstensioni()
    {
      ////Estensione LICENZA - .RLF
      //RegistryKey key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Licenza));
      //key.Close();
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Licenza) + "\\DefaultIcon");
      //key.SetValue("", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft-Licenza.ico");
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Licenza) + "\\shell\\Open\\command");
      //key.SetValue("", "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft.exe" + "\" \"%1\"");
      //key.Close();

      ////Estensione Import/Export File - .RIEF
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.ImportExport));
      //key.Close();
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.ImportExport) + "\\DefaultIcon");
      //key.SetValue("", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft-ImportExport.ico");
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.ImportExport) + "\\shell\\Open\\command");
      //key.SetValue("", "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft.exe" + "\" \"%1\"");
      //key.Close();

      ////Estensione Import Template - .RIET
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.ImportTemplate));
      //key.Close();
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.ImportTemplate) + "\\DefaultIcon");
      //key.SetValue("", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft-Documento.ico");
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.ImportTemplate) + "\\shell\\Open\\command");
      //key.SetValue("", "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft.exe" + "\" \"%1\"");
      //key.Close();

      ////Estensione Scambio Dati - .RSDT
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.ScambioDati));
      //key.Close();
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.ScambioDati) + "\\DefaultIcon");
      //key.SetValue("", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft-Documento.ico");
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.ScambioDati) + "\\shell\\Open\\command");
      //key.SetValue("", "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft.exe" + "\" \"%1\"");
      //key.Close();

      ////Estensione Backup su file - .RBKF
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.BackUp));
      //key.Close();
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.BackUp) + "\\DefaultIcon");
      //key.SetValue("", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft-BackUp.ico");
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.BackUp) + "\\shell\\Open\\command");
      //key.SetValue("", "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft.exe" + "\" \"%1\"");
      //key.Close();

      ////Estensione documenti - Revisione rrdf
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Revisione));
      //key.Close();
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Revisione) + "\\DefaultIcon");
      //key.SetValue("", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft-Documento.ico");
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Revisione) + "\\shell\\Open\\command");
      //key.SetValue("", "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft.exe" + "\" \"%1\"");
      //key.Close();

      ////Estensione documenti - Verifica rvdf
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Verifica));
      //key.Close();
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Verifica) + "\\DefaultIcon");
      //key.SetValue("", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft-Documento.ico");
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Verifica) + "\\shell\\Open\\command");
      //key.SetValue("", "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft.exe" + "\" \"%1\"");
      //key.Close();

      ////Estensione documenti - Vigilanza radf
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Vigilanza));
      //key.Close();
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Vigilanza) + "\\DefaultIcon");
      //key.SetValue("", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft-Documento.ico");
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Vigilanza) + "\\shell\\Open\\command");
      //key.SetValue("", "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft.exe" + "\" \"%1\"");
      //key.Close();

      ////Estensione documenti - Incarico ridf
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Incarico));
      //key.Close();
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Incarico) + "\\DefaultIcon");
      //key.SetValue("", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft-Documento.ico");
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Incarico) + "\\shell\\Open\\command");
      //key.SetValue("", "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft.exe" + "\" \"%1\"");
      //key.Close();

      ////Estensione documenti - ISQC rqdf
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.ISQC));
      //key.Close();
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.ISQC) + "\\DefaultIcon");
      //key.SetValue("", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft-Documento.ico");
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.ISQC) + "\\shell\\Open\\command");
      //key.SetValue("", "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft.exe" + "\" \"%1\"");
      //key.Close();

      ////Estensione documenti - Bilancio rbdf
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Bilancio));
      //key.Close();
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Bilancio) + "\\DefaultIcon");
      //key.SetValue("", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft-Documento.ico");
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Bilancio) + "\\shell\\Open\\command");
      //key.SetValue("", "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft.exe" + "\" \"%1\"");
      //key.Close();

      ////Estensione documenti - Vigilanza radf
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Vigilanza));
      //key.Close();
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Vigilanza) + "\\DefaultIcon");
      //key.SetValue("", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft-Documento.ico");
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Vigilanza) + "\\shell\\Open\\command");
      //key.SetValue("", "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft.exe" + "\" \"%1\"");
      //key.Close();

      ////Estensione documenti - Conclusioni rcdf
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Conclusione));
      //key.Close();
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Conclusione) + "\\DefaultIcon");
      //key.SetValue("", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft-Documento.ico");
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Conclusione) + "\\shell\\Open\\command");
      //key.SetValue("", "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft.exe" + "\" \"%1\"");
      //key.Close();

      ////Estensione LICENZA - Sigillo rslf
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Sigillo));
      //key.Close();
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Sigillo) + "\\DefaultIcon");
      //key.SetValue("", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft-Licenza.ico");
      //key = Registry.ClassesRoot.CreateSubKey(EstensioneFile(App.TipoFile.Sigillo) + "\\shell\\Open\\command");
      //key.SetValue("", "\"" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Revisoft\\Revisoft\\Revisoft.exe" + "\" \"%1\"");
      //key.Close();
    }


    //public void SetRegistroChiaveApplicazione(string chiave, string valore)
    //{
    //    GestioneLicenza gl = new GestioneLicenza();
    //    gl.SetFromInfo(chiave, valore);

    //    //string regFolder = "SOFTWARE\\" + App.ApplicationFolder + "\\" + App.ApplicationFolder;

    //    ////Chiave applicazione HKEY_LOCAL_MACHINE\SOFTWARE\Revisoft\Revisoft
    //    //RegistryKey key = Registry.LocalMachine.CreateSubKey(regFolder);
    //    //key.Close();
    //    ////Salvo il valore in  HKEY_LOCAL_MACHINE\SOFTWARE\Revisoft\Revisoft
    //    //key = Registry.LocalMachine.OpenSubKey(regFolder, true);
    //    ////Creo valore HKEY_LOCAL_MACHINE\SOFTWARE\Revisoft\Revisoft -> valore
    //    //key.SetValue(chiave, valore);
    //}

    public string GetRegistroChiaveApplicazione(string chiave)
    {
      //4.5 mantenere utilizzata da migrazione info file

      string regFolder = "SOFTWARE\\" + App.ApplicationFolder + "\\" + App.ApplicationFolder;

      //Verifico valore  HKEY_LOCAL_MACHINE\SOFTWARE\Revisoft\Revisoft -> DL_Info
      RegistryKey key = Registry.LocalMachine.OpenSubKey(regFolder, true);

      //leggo valore
      return key.GetValue(chiave, "").ToString();
    }

    #endregion

    #region CONFIGURAZIONE


    public App.TipoGestioneArchivio TipologiaArchivio(string val)
    {
      if (val == "")
        return App.TipoGestioneArchivio.Locale;

      if (val == App.TipoGestioneArchivio.Locale.ToString())
        return App.TipoGestioneArchivio.Locale;
      else if (val == App.TipoGestioneArchivio.Remoto.ToString())
        return App.TipoGestioneArchivio.Remoto;
      else if (val == App.TipoGestioneArchivio.Cloud.ToString())
        return App.TipoGestioneArchivio.Cloud;
      else if (val == App.TipoGestioneArchivio.LocaleImportExport.ToString())
        return App.TipoGestioneArchivio.LocaleImportExport;


      return App.TipoGestioneArchivio.Locale;
    }


    public void ConfiguraApplicazione()
    {
      RevisoftApplication.Utilities u = new Utilities();

      //Finestra di benvenuto
      //string val = u.GetRegistroChiaveApplicazione(App.Registry_Benvenuto.ToString());
      //if (val != "")
      //    App.AppSetupBenvenuto = Convert.ToBoolean(u.GetRegistroChiaveApplicazione(App.Registry_Benvenuto));
      //else
      //    App.AppSetupBenvenuto = true;

      //Istruzioni automatiche
      //string val2 = u.GetRegistroChiaveApplicazione(App.Registry_IstruzioniAutomatiche.ToString());
      //if (val2 != "")
      //    App.AppSetupIstruzioniAutomatiche = Convert.ToBoolean( u.GetRegistroChiaveApplicazione( App.Registry_IstruzioniAutomatiche ) );
      //else
      //    App.AppSetupIstruzioniAutomatiche = false;

      //Tipo archivio
      //val = u.GetRegistroChiaveApplicazione(App.Registry_TipoGestioneArchivio);

      //andrea 4.5
      //App.AppSetupTipoGestioneArchivio = TipologiaArchivio(val);
      //if (val != "")
      //{
      //    if (val == App.TipoGestioneArchivio.Locale.ToString())
      //        App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Locale;
      //    else if (val == App.TipoGestioneArchivio.Remoto.ToString())
      //        App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Remoto;
      //    else if (val == App.TipoGestioneArchivio.Cloud.ToString())
      //        App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Cloud;
      //    else if (val == App.TipoGestioneArchivio.LocaleImportExport.ToString())
      //        App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.LocaleImportExport;
      //}
      //else
      //    App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Locale;
      //path archivio remoto
      //App.AppPathArchivioRemoto = u.GetRegistroChiaveApplicazione(App.Registry_PathArchivioRemoto);


      //Configurazione client lan
      //if ((App.TipoLicenza == App.TipologieLicenze.ClientLan || App.TipoLicenza == App.TipologieLicenze.ClientLanMulti || App.TipoLicenza == App.TipologieLicenze.Server) && App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Remoto)
      //if(App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Remoto)
      //{
      //    //Imposto percorso archivo remoto
      //    //App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Remoto;
      //    App.AppPathArchivioRemoto = u.GetRegistroChiaveApplicazione(App.Registry_PathArchivioRemoto);
      //}

      //Log
      App.AppInizioSessione = DateTime.Now;

      //Software Distribution
      App.AppSetupNuovaVersione = false;
      App.AppSetupScaricaNuovaVersione = false;
      App.AppNome = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
      App.AppVersione = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

      ConfiguraPercorsi();

    }

    public void ConfiguraPercorsi()
    {
      //PATH DATI 
      switch (App.AppSetupTipoGestioneArchivio)
      {
        case App.TipoGestioneArchivio.Remoto:
          App.AppDataFolder = App.AppPathArchivioRemoto;
          //versione 3.0 - verifico formato path e converto in UNC
          if (App.AppDataFolder.IndexOf(':') == 1)
          {
            App.AppDataFolder = GetRealPathFile(App.AppDataFolder);
          }
          break;
        case App.TipoGestioneArchivio.Locale:
        case App.TipoGestioneArchivio.LocaleImportExport:
        //per ora non sviluppato, ma app datafolder deve comunque contenere un dato
        case App.TipoGestioneArchivio.Cloud:
        default:
          App.AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + App.ApplicationFolder + "\\" + App.ApplicationFolder;
          break;
      }

      //Cartella locale (usata per log utente)
      App.AppLocalDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + App.ApplicationFolder + "\\" + App.ApplicationFolder;

      if (!string.IsNullOrEmpty(App.AppTeamDataFolder)) App.AppDataFolder = App.AppTeamDataFolder;

      //FOLDER DATI
      App.AppDataDataFolder = App.AppDataFolder + "\\" + App.DataFolder;
      //FOLDER BACKUP
      App.AppBackupFolder = App.AppDataFolder + "\\" + App.BackUpFolder;
      //FOLDER DOCUMENTI UTENTE
      App.AppDocumentiFolder = App.AppDataFolder + "\\" + App.UserFileFolder;
      //FOLDER DOCUMENTI FLUSSI UTENTE
      App.AppDocumentiFlussiFolder = App.AppDataFolder + "\\" + App.UserFileFolder + "\\" + App.UserFileFlussiFolder;
      //FOLDER TEMPLATE
      App.AppTemplateFolder = App.AppDataFolder + "\\" + App.TemplateFolder;


      //FOLDER MODELLI PREDEFINITI
      App.AppModelliFolder = App.AppTemplateFolder + "\\" + App.ModelliFolder;
      //FOLDER FORMULARIO
      App.AppFormularioFolder = App.AppTemplateFolder + "\\" + App.FormularioFolder;

      //SYSTEM FILE: FORMULARIO
      App.AppFormularioFile = App.AppTemplateFolder + "\\" + App.DocNameFormulario + EstensioneFile(App.TipoFile.Formulario);
      App.AppFormularioFileDati = App.AppTemplateFolder + "\\" + App.DocNameFormularioDati + EstensioneFile(App.TipoFile.Formulario);

      //SYSTEM FILE: MODELLI PREDEFINITI
      App.AppModelliFile = App.AppTemplateFolder + "\\" + App.DocNameModelli + EstensioneFile(App.TipoFile.ModellPredefiniti);


      //USER FILE: MASTER
      App.AppMasterDataFile = App.AppDataFolder + "\\" + App.ApplicationFileName + EstensioneFile(App.TipoFile.Master);
      //USER FILE: BACKUP
      App.AppBackUpDataFile = App.AppDataFolder + "\\" + App.ApplicationFileName + EstensioneFile(App.TipoFile.BackUp);
      //USER FILE: DOCUMENTI ASSOCIATI
      App.AppDocumentiDataFile = App.AppDataFolder + "\\" + App.ApplicationFileName + EstensioneFile(App.TipoFile.DocumentiAssociati);


      //4.6 FOLDER BACKUP PERSONALIZZATO
      App.AppBackupFolderUser = App.AppUserBackupFolder + "\\" + App.BackUpFolder;
      App.AppBackUpDataFileUser = App.AppUserBackupFolder + "\\" + App.ApplicationFileName + EstensioneFile(App.TipoFile.BackUp);


      //FILE TEMPLATE Incarico
      App.AppTemplateTreeIncarico = App.AppTemplateFolder + "\\" + App.DocNameIncarico + EstensioneFile(App.TipoFile.Incarico);

      //FILE TEMPLATE ISQC
      App.AppTemplateTreeISQC = App.AppTemplateFolder + "\\" + App.DocNameISQC + EstensioneFile(App.TipoFile.ISQC);

      //FILE TEMPLATE Verifica
      App.AppTemplateTreeVerifica = App.AppTemplateFolder + "\\" + App.DocNameVerifica + EstensioneFile(App.TipoFile.Verifica);
      App.AppTemplateTreePianificazioniVerifica = App.AppTemplateFolder + "\\" + App.DocNamePianificazioniVerifica + EstensioneFile(App.TipoFile.PianificazioniVerifica);
      //FILE TEMPLATE Revisione
      App.AppTemplateTreeRevisione = App.AppTemplateFolder + "\\" + App.DocNameRevisione + EstensioneFile(App.TipoFile.Revisione);
      //FILE TEMPLATE Bilancio
      App.AppTemplateTreeBilancio = App.AppTemplateFolder + "\\" + App.DocNameBilancio + EstensioneFile(App.TipoFile.Bilancio);
      //FILE TEMPLATE Conclusione
      App.AppTemplateTreeConclusione = App.AppTemplateFolder + "\\" + App.DocNameConclusione + EstensioneFile(App.TipoFile.Conclusione);
      //FILE TEMPLATE Vigilanza
      App.AppTemplateTreeVigilanza = App.AppTemplateFolder + "\\" + App.DocNameVigilanza + EstensioneFile(App.TipoFile.Vigilanza);
      App.AppTemplateTreePianificazioniVigilanza = App.AppTemplateFolder + "\\" + App.DocNamePianificazioniVigilanza + EstensioneFile(App.TipoFile.PianificazioniVigilanza);
      //FILE TEMPLATE RelazioneB
      App.AppTemplateTreeRelazioneB = App.AppTemplateFolder + "\\" + App.DocNameRelazioneB + EstensioneFile(App.TipoFile.RelazioneB);
      //FILE TEMPLATE RelazioneV
      App.AppTemplateTreeRelazioneV = App.AppTemplateFolder + "\\" + App.DocNameRelazioneV + EstensioneFile(App.TipoFile.RelazioneV);


      //FILE TEMPLATE RelazioneBC
      App.AppTemplateTreeRelazioneBC = App.AppTemplateFolder + "\\" + App.DocNameRelazioneBC + EstensioneFile(App.TipoFile.RelazioneBC);
      //FILE TEMPLATE RelazioneVC
      App.AppTemplateTreeRelazioneVC = App.AppTemplateFolder + "\\" + App.DocNameRelazioneVC + EstensioneFile(App.TipoFile.RelazioneVC);


      //FILE TEMPLATE RelazioneBV
      App.AppTemplateTreeRelazioneBV = App.AppTemplateFolder + "\\" + App.DocNameRelazioneBV + EstensioneFile(App.TipoFile.RelazioneBV);

      //FILE TEMPLATE DATA Incarico
      App.AppTemplateDataIncarico = App.AppTemplateFolder + "\\" + App.DocNameIncaricoDati + EstensioneFile(App.TipoFile.Incarico);

      //FILE TEMPLATE DATA ISQC
      App.AppTemplateDataISQC = App.AppTemplateFolder + "\\" + App.DocNameISQCDati + EstensioneFile(App.TipoFile.ISQC);

      //FILE TEMPLATE DATA Verifica
      App.AppTemplateDataVerifica = App.AppTemplateFolder + "\\" + App.DocNameVerificaDati + EstensioneFile(App.TipoFile.Verifica);
      App.AppTemplateDataPianificazioniVerifica = App.AppTemplateFolder + "\\" + App.DocNamePianificazioniVerificaDati + EstensioneFile(App.TipoFile.PianificazioniVerifica);
      //FILE TEMPLATE DATA Revisione
      App.AppTemplateDataRevisione = App.AppTemplateFolder + "\\" + App.DocNameRevisioneDati + EstensioneFile(App.TipoFile.Revisione);
      //FILE TEMPLATE DATA Bilancio
      App.AppTemplateDataBilancio = App.AppTemplateFolder + "\\" + App.DocNameBilancioDati + EstensioneFile(App.TipoFile.Bilancio);
      //FILE TEMPLATE DATA Bilancio
      App.AppTemplateDataConclusione = App.AppTemplateFolder + "\\" + App.DocNameConclusioneDati + EstensioneFile(App.TipoFile.Conclusione);
      //FILE TEMPLATE DATA Vigilanza
      App.AppTemplateDataVigilanza = App.AppTemplateFolder + "\\" + App.DocNameVigilanzaDati + EstensioneFile(App.TipoFile.Vigilanza);
      App.AppTemplateDataPianificazioniVigilanza = App.AppTemplateFolder + "\\" + App.DocNamePianificazioniVigilanzaDati + EstensioneFile(App.TipoFile.PianificazioniVigilanza);
      //FILE TEMPLATE DATA RelazioneB
      App.AppTemplateDataRelazioneB = App.AppTemplateFolder + "\\" + App.DocNameRelazioneBDati + EstensioneFile(App.TipoFile.RelazioneB);
      //FILE TEMPLATE DATA RelazioneV
      App.AppTemplateDataRelazioneV = App.AppTemplateFolder + "\\" + App.DocNameRelazioneVDati + EstensioneFile(App.TipoFile.RelazioneV);


      //FILE TEMPLATE DATA RelazioneBC
      App.AppTemplateDataRelazioneBC = App.AppTemplateFolder + "\\" + App.DocNameRelazioneBCDati + EstensioneFile(App.TipoFile.RelazioneBC);
      //FILE TEMPLATE DATA RelazioneVC
      App.AppTemplateDataRelazioneVC = App.AppTemplateFolder + "\\" + App.DocNameRelazioneVCDati + EstensioneFile(App.TipoFile.RelazioneVC);


      //FILE TEMPLATE DATA RelazioneB
      App.AppTemplateDataRelazioneBV = App.AppTemplateFolder + "\\" + App.DocNameRelazioneBVDati + EstensioneFile(App.TipoFile.RelazioneBV);

      //FILE TEMPLATE STAMPA
      App.AppTemplateStampa = App.AppTemplateFolder + "\\" + App.DocNameModelloStampa + EstensioneFile(App.TipoFile.DocTemplate);
      App.AppTemplateStampaNoLogo = App.AppTemplateFolder + "\\" + App.DocNameModelloStampaNoLogo + EstensioneFile(App.TipoFile.DocTemplate);
      App.AppTemplateStampaBilancio = App.AppTemplateFolder + "\\" + App.DocNameModelloStampaBilancio + EstensioneFile(App.TipoFile.DocTemplate);

      //FILE TEMPLATE BILANCI
      App.AppTemplateBilancio_Attivo = App.AppTemplateFolder + "\\" + App.Bilancio_Attivo + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancio_ContoEconomico = App.AppTemplateFolder + "\\" + App.Bilancio_ContoEconomico + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancio_Passivo = App.AppTemplateFolder + "\\" + App.Bilancio_Passivo + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancio_Riclassificato = App.AppTemplateFolder + "\\" + App.Bilancio_Riclassificato + EstensioneFile(App.TipoFile.RevisoftXML);

      App.AppTemplateBilancioAbbreviato_Attivo = App.AppTemplateFolder + "\\" + App.BilancioAbbreviato_Attivo + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancioAbbreviato_ContoEconomico = App.AppTemplateFolder + "\\" + App.BilancioAbbreviato_ContoEconomico + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancioAbbreviato_Passivo = App.AppTemplateFolder + "\\" + App.BilancioAbbreviato_Passivo + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancioAbbreviato_Riclassificato = App.AppTemplateFolder + "\\" + App.BilancioAbbreviato_Riclassificato + EstensioneFile(App.TipoFile.RevisoftXML);

      App.AppXBRL = App.AppTemplateFolder + "\\" + App.XBRL + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppLEAD = App.AppTemplateFolder + "\\" + App.Lead + EstensioneFile(App.TipoFile.RevisoftXML);

      App.AppTemplateBilancio_Attivo2016 = App.AppTemplateFolder + "\\" + App.Bilancio_Attivo2016 + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancio_ContoEconomico2016 = App.AppTemplateFolder + "\\" + App.Bilancio_ContoEconomico2016 + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancio_Passivo2016 = App.AppTemplateFolder + "\\" + App.Bilancio_Passivo2016 + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancio_Riclassificato2016 = App.AppTemplateFolder + "\\" + App.Bilancio_Riclassificato2016 + EstensioneFile(App.TipoFile.RevisoftXML);

      App.AppTemplateBilancio_Attivo2016_Consolidato = App.AppTemplateFolder + "\\" + App.Bilancio_Attivo2016_Consolidato + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancio_ContoEconomico2016_Consolidato = App.AppTemplateFolder + "\\" + App.Bilancio_ContoEconomico2016_Consolidato + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancio_Passivo2016_Consolidato = App.AppTemplateFolder + "\\" + App.Bilancio_Passivo2016_Consolidato + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancio_Riclassificato2016_Consolidato = App.AppTemplateFolder + "\\" + App.Bilancio_Riclassificato2016_Consolidato + EstensioneFile(App.TipoFile.RevisoftXML);


      App.AppTemplateBilancioAbbreviato_Attivo2016 = App.AppTemplateFolder + "\\" + App.BilancioAbbreviato_Attivo2016 + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancioAbbreviato_ContoEconomico2016 = App.AppTemplateFolder + "\\" + App.BilancioAbbreviato_ContoEconomico2016 + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancioAbbreviato_Passivo2016 = App.AppTemplateFolder + "\\" + App.BilancioAbbreviato_Passivo2016 + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancioAbbreviato_Riclassificato2016 = App.AppTemplateFolder + "\\" + App.BilancioAbbreviato_Riclassificato2016 + EstensioneFile(App.TipoFile.RevisoftXML);

      App.AppTemplateBilancioMicro_Attivo2016 = App.AppTemplateFolder + "\\" + App.BilancioMicro_Attivo2016 + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancioMicro_ContoEconomico2016 = App.AppTemplateFolder + "\\" + App.BilancioMicro_ContoEconomico2016 + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancioMicro_Passivo2016 = App.AppTemplateFolder + "\\" + App.BilancioMicro_Passivo2016 + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppTemplateBilancioMicro_Riclassificato2016 = App.AppTemplateFolder + "\\" + App.BilancioMicro_Riclassificato2016 + EstensioneFile(App.TipoFile.RevisoftXML);


      App.AppXBRL2016 = App.AppTemplateFolder + "\\" + App.XBRL2016 + EstensioneFile(App.TipoFile.RevisoftXML);
      App.AppLEAD2016 = App.AppTemplateFolder + "\\" + App.Lead2016 + EstensioneFile(App.TipoFile.RevisoftXML);

      //LOG - Sempre cartella locale
      App.AppLogFolder = App.AppLocalDataFolder + "\\" + App.LogFolder;
      App.AppOldLogFolder = App.AppLocalDataFolder + "\\" + App.LogFolder + "\\" + App.OldLogFolder;

      //4.5 andrea
      //verifico presenza file archivio
      //if ((!File.Exists(App.AppMasterDataFile)) || (App.RemoteDesktop && App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Locale))
      //Patrizia - rimosso il controllo sull'esistenza del file rdmf
      if (App.RemoteDesktop && App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Locale)
      {
        App.AppAutoExec = true;
        App.AppAutoExecFunzione = App.TipoFunzioniAutoexec.SetupLan;
      }

    }

    #endregion

    #region INFO_MACCHINA
    public bool LeggiInfoMacchina()
    {
      try
      {
        //Leggo informazioni di sistema
        ManagementObjectCollection mbsList = null;
        //Id Processore
        ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_processor");
        mbsList = mbs.Get();
        foreach (ManagementObject mo in mbsList)
        {
          _IdProcessore = mo["ProcessorID"].ToString();
        }
        //MAC address ATTIVO
        mbs = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'");
        mbsList = mbs.Get();
        foreach (ManagementObject mo in mbsList)
        {
          _MacAddress = mo["MacAddress"].ToString();
        }
        //MAC address NON ATTIVO
        if (_MacAddress == null)
        {
          mbs = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
          mbsList = mbs.Get();
          foreach (ManagementObject mo in mbsList)
          {
            if (mo["MacAddress"] != null)
              _MacAddress = mo["MacAddress"].ToString();
          }
        }

        //mbs = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");


        //foreach (ManagementObject wmi_HD in mbs.Get())
        //{
        //    // get the hard drive from collection
        //    // using index
        //    //HardDrive hd = (HardDrive)hdCollection[i];

        //    // get the hardware serial no.
        //    if (wmi_HD["Removable"] != null && !((Boolean)wmi_HD["Removable"]) && wmi_HD["Replaceable"] != null && !((Boolean)wmi_HD["Replaceable"]) && wmi_HD["SerialNumber"] != null)
        //    {
        //        _HDSerial = wmi_HD["SerialNumber"].ToString();
        //        break;
        //    }                    
        //}


        //mbs = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk");

        //foreach (ManagementObject drive in mbs.Get())
        //{
        //    Console.WriteLine("-------");
        //    Console.WriteLine(string.Format("VolumeName: {0}", drive["VolumeName"]));
        //    Console.WriteLine(string.Format("VolumeSerialNumber: {0}", drive["VolumeSerialNumber"]));
        //    Console.WriteLine(string.Format("MediaType: {0}", drive["MediaType"]));
        //    Console.WriteLine(string.Format("FileSystem: {0}", drive["FileSystem"]));
        //}

        _HDSerial = GetHardSerial();

        //Creo codice macchina
        CreaCodiceMacchina();
        //esco
        return true;
      }
      catch (Exception)
      {
        App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
        RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
        m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.NoCodiceMacchina;
        m.VisualizzaMessaggio();
        return false;
      }
    }

    public string GetHardSerial()
    {
      //string SerialNumber = PC_INFO_SCONOSCIUTA;
      //string tempLogFile = @"seriallog.txt";
      //StringBuilder microlog = new StringBuilder();
      //microlog.AppendLine("Log Seriale");
      //try
      //{

      //    ManagementObjectSearcher Finder = new ManagementObjectSearcher("Select * from Win32_OperatingSystem");
      //    string Name = "";

      //    foreach (ManagementObject OS in Finder.Get()) {
      //        Name = OS["Name"].ToString();
      //        microlog.AppendLine(String.Format("Name: {0}",Name));
      //    }
      //    int ind = 0;
      //    int HardIndex = 0;
      //    //Name = "Microsoft Windows XP Professional|C:\WINDOWS|\Device\Harddisk0\Partition1"
      //    if (Name.IndexOf("Harddisk") > 0)
      //    {
      //        microlog.AppendLine("HD Trovato");
      //        ind = Name.IndexOf("Harddisk") + 8;
      //     HardIndex = Convert.ToInt16(Name.Substring(ind, 1));
      //    }
      //    microlog.AppendLine(String.Format("Harddisk: {0}", HardIndex));
      //    Finder = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE Index=" + HardIndex);
      //    foreach (ManagementObject HardDisks in Finder.Get())
      //    {
      //        SerialNumber =  String.Format("{0}|{1}|{2}|{3}",HardDisks["Model"], HardDisks["Manufacturer"], HardDisks["TotalHeads"], HardDisks["SerialNumber"]).Trim();
      //    }

      //    SerialNumber = GetHash(SerialNumber).Replace("-","");

      //    //return SerialNumber;
      //}
      //catch (Exception ex)
      //{
      //    microlog.AppendLine("ERRORE:");
      //    microlog.AppendLine(ex.ToString());
      //    //throw;
      //}
      //microlog.AppendLine("************************************");
      //microlog.AppendLine("************************************");
      //System.IO.File.AppendAllText(tempLogFile, microlog.ToString());
      //return SerialNumber;

      string SerialNumber = PC_INFO_SCONOSCIUTA;
#pragma warning disable CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato
      string tempLogFile = @"seriallog.txt";
#pragma warning restore CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato
      StringBuilder microlog = new StringBuilder();
      microlog.AppendLine("Log Seriale");
      try
      {
        SerialNumber = GetHash(
            "CPU >> " + cpuId()
            + "\nBIOS >> " + biosId()
            + "\nBASE >> " + baseId()
            + "\nDISK >> " + diskId()
            + "\nVIDEO >> " + videoId()
                             );
        SerialNumber = SerialNumber.Replace("-", "");
      }
      catch (Exception ex)
      {
        microlog.AppendLine("ERRORE:");
        microlog.AppendLine(ex.ToString());
        //throw;
      }

      microlog.AppendLine("************************************");
      microlog.AppendLine("************************************");
      //System.IO.File.AppendAllText(tempLogFile, microlog.ToString());
      return SerialNumber;



    }

    private string GetHash(string s)
    {
      MD5 sec = new MD5CryptoServiceProvider();
      ASCIIEncoding enc = new ASCIIEncoding();
      byte[] bt = enc.GetBytes(s);
      return GetHexString(sec.ComputeHash(bt));
    }
    private string GetHexString(byte[] bt)
    {
      string s = string.Empty;
      for (int i = 0; i < bt.Length; i++)
      {
        byte b = bt[i];
        int n, n1, n2;
        n = (int)b;
        n1 = n & 15;
        n2 = (n >> 4) & 15;
        if (n2 > 9)
          s += ((char)(n2 - 10 + (int)'A')).ToString();
        else
          s += n2.ToString();
        if (n1 > 9)
          s += ((char)(n1 - 10 + (int)'A')).ToString();
        else
          s += n1.ToString();
        if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "-";
      }
      return s;
    }

    #region Original Device ID Getting Code
    //Return a hardware identifier
    private string identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
    {
      string result = "";
      System.Management.ManagementClass mc =
  new System.Management.ManagementClass(wmiClass);
      System.Management.ManagementObjectCollection moc = mc.GetInstances();
      foreach (System.Management.ManagementObject mo in moc)
      {
        if (mo[wmiMustBeTrue].ToString() == "True")
        {
          //Only get the first one
          if (result == "")
          {
            try
            {
              result = mo[wmiProperty].ToString();
              break;
            }
            catch
            {
            }
          }
        }
      }
      return result;
    }
    //Return a hardware identifier
    private string identifier(string wmiClass, string wmiProperty)
    {
      string result = "";
      System.Management.ManagementClass mc =
  new System.Management.ManagementClass(wmiClass);
      System.Management.ManagementObjectCollection moc = mc.GetInstances();
      foreach (System.Management.ManagementObject mo in moc)
      {
        //Only get the first one
        if (result == "")
        {
          try
          {
            result = mo[wmiProperty].ToString();
            break;
          }
          catch
          {
          }
        }
      }
      return result;
    }
    private string cpuId()
    {
      //Uses first CPU identifier available in order of preference
      //Don't get all identifiers, as it is very time consuming
      string retVal = identifier("Win32_Processor", "UniqueId");
      if (retVal == "") //If no UniqueID, use ProcessorID
      {
        retVal = identifier("Win32_Processor", "ProcessorId");
        if (retVal == "") //If no ProcessorId, use Name
        {
          retVal = identifier("Win32_Processor", "Name");
          if (retVal == "") //If no Name, use Manufacturer
          {
            retVal = identifier("Win32_Processor", "Manufacturer");
          }
          //Add clock speed for extra security
          retVal += identifier("Win32_Processor", "MaxClockSpeed");
        }
      }
      return retVal;
    }
    //BIOS Identifier
    private string biosId()
    {
      return identifier("Win32_BIOS", "Manufacturer")
      + identifier("Win32_BIOS", "SMBIOSBIOSVersion")
      + identifier("Win32_BIOS", "IdentificationCode")
      + identifier("Win32_BIOS", "SerialNumber")
      + identifier("Win32_BIOS", "ReleaseDate")
      + identifier("Win32_BIOS", "Version");
    }
    //Main physical hard drive ID
    private string diskId()
    {
      return identifier("Win32_DiskDrive", "Model")
      + identifier("Win32_DiskDrive", "Manufacturer")
      + identifier("Win32_DiskDrive", "SerialNumber")
      + identifier("Win32_DiskDrive", "TotalHeads");
    }
    //Motherboard ID
    private string baseId()
    {
      return identifier("Win32_BaseBoard", "Model")
      + identifier("Win32_BaseBoard", "Manufacturer")
      + identifier("Win32_BaseBoard", "Name")
      + identifier("Win32_BaseBoard", "SerialNumber");
    }
    //Primary video controller ID
    private string videoId()
    {
      return identifier("Win32_VideoController", "DriverVersion")
      + identifier("Win32_VideoController", "Name");
    }
    //First enabled network card ID

    #endregion



    public void CreaCodiceMacchina()
    {
      //prima chiave: id processore
      if (_IdProcessore == null)
        _IdProcessore = PC_INFO_SCONOSCIUTA;

      //seconda chiave: mac address
      if (_MacAddress == null)
        _MacAddress = PC_INFO_SCONOSCIUTA;

      if (_HDSerial == null)
        _HDSerial = PC_INFO_SCONOSCIUTA;

      //Codice macchina
      //App.CodiceMacchina = _IdProcessore + "-" + _MacAddress.Replace(":", "");
      //App.CodiceMacchina = _IdProcessore;
      App.CodiceMacchina = _IdProcessore + "-" + _HDSerial;
    }

    #endregion

    #region GESTIONE_FILES_FILESYSTEM

    public string EstensioneFile(App.TipoFile i)
    {
      return (string)_TipoEstensioniFile[(int)i];
    }

    public string EstensioneFile_Filtro(App.TipoFile i)
    {
      return (string)_TipoEstensioniFile_Filtri[(int)i];
    }

    public string sys_OpenDirectoryDialog()
    {
      var dialog = new System.Windows.Forms.FolderBrowserDialog();
      System.Windows.Forms.DialogResult result = dialog.ShowDialog();

      //Gestione valore di ritorno
      if (result == System.Windows.Forms.DialogResult.OK)
      {
        return dialog.SelectedPath;
      }

      return "";
    }

    public string sys_OpenFileDialog(string nomefile, App.TipoFile tipoEstensione, string filter = "")
    {
      // Configuro dialog box
      Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
      dlg.FileName = nomefile;
      if (filter != "")
        dlg.Filter = filter;
      dlg.DefaultExt = EstensioneFile(tipoEstensione);
      dlg.Filter = EstensioneFile_Filtro(tipoEstensione);

      //Apro dialog box
      Nullable<bool> result = dlg.ShowDialog();

      //Gestione valore di ritorno
      if (result == true)
      {
        return dlg.FileName;
      }

      return null;
    }

    public string sys_SaveFileDialog(string nomefile, App.TipoFile tipoEstensione)
    {
      // Configuro dialog box
      Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
      dlg.FileName = nomefile;
      dlg.DefaultExt = EstensioneFile(tipoEstensione);
      dlg.Filter = EstensioneFile_Filtro(tipoEstensione);

      //Apro dialog box
      Nullable<bool> result = dlg.ShowDialog();

      //Gestione valore di ritorno
      if (result == true)
      {
        return dlg.FileName;
      }

      return null;
    }

    public bool CheckXmlDocument(XmlDocument doc, App.TipoFile tf)
    {
      bool returnvalue = false;

      try
      {
        XmlNode node = doc.SelectSingleNode("//REVISOFT");

        if (node != null)
        {
          if (((App.TipoFile)(Convert.ToInt32(node.Attributes["ID"].Value))) == tf)
          {
            returnvalue = true;
          }
        }
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }

      return returnvalue;
    }

    public bool CheckXmlDocument(XmlDocument doc, App.TipoFile tf, string Tipo)
    {
      bool returnvalue = false;

      try
      {
        XmlNode node = doc.SelectSingleNode("//REVISOFT");

        if (node != null)
        {
          string tmptf = ((App.TipoFile)(Convert.ToInt32(node.Attributes["ID"].Value))).ToString().Substring(0, 8);
          if (((tmptf == tf.ToString().Substring(0, 8)) || (tf == App.TipoFile.Vigilanza)) && node.Attributes["Tipo"].Value == Tipo)
          {
            returnvalue = true;
          }
        }
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        returnvalue = true;
      }

      return returnvalue;
    }


    [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int WNetGetConnection(
        [MarshalAs(UnmanagedType.LPTStr)] string localName,
        [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
        ref int length);

    private string GetUNCPath(string originalPath)
    {
      StringBuilder sb = new StringBuilder(512);
      int size = sb.Capacity;

      // look for the {LETTER}: combination ...
      if (originalPath.Length > 2 && originalPath[1] == ':')
      {
        // don't use char.IsLetter here - as that can be misleading
        // the only valid drive letters are a-z && A-Z.
        char c = originalPath[0];
        if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
        {
          int error = WNetGetConnection(originalPath.Substring(0, 2),
              sb, ref size);

          //if ( error == 0 )
          {
            DirectoryInfo dir = new DirectoryInfo(originalPath);

            string path = Path.GetFullPath(originalPath)
                .Substring(Path.GetPathRoot(originalPath).Length);
            return Path.Combine(sb.ToString().TrimEnd(), path);
          }
        }
      }

      return originalPath;
    }

    public string GetRealPathFile(string file)
    {
      FileInfo fi = new FileInfo(file);
      if (fi.Exists)
      {
        return file;
      }

      string disckpath = file.Split('\\')[0];
      string uncpath = GetUNCPath(disckpath + '\\');

      return file.Replace(disckpath, uncpath);
    }
    #endregion

    #region CONVALIDA_DATI_INTERFACCIA

    public bool ConvalidaDatiInterfaccia(DependencyObject obj, string msg)
    {
      return ConvalidaDatiInterfaccia(obj, msg, "");
    }

    public bool ConvalidaDatiInterfaccia(DependencyObject obj, string msg, string msg2)
    {
      switch (obj.GetType().Name)
      {
        case "TextBox":
          if (((System.Windows.Controls.TextBox)obj).Text.Trim().Length == 0)
          {
            System.Windows.MessageBox.Show("Attenzione, dato obbligatorio\n\n" + msg);
            ((System.Windows.Controls.TextBox)obj).Focus();
            return false;
          }
          else
          {
            if (Validation.GetHasError(((System.Windows.Controls.TextBox)obj)) == true)
            {
              System.Windows.MessageBox.Show("Attenzione, contenuto non valido\n\n" + msg2);
              ((System.Windows.Controls.TextBox)obj).Focus();
              return false;
            }
          }
          break;
        case "ComboBox":
          if (((System.Windows.Controls.ComboBox)obj).SelectedIndex == -1)
          {
            System.Windows.MessageBox.Show("Attenzione, dato obbligatorio\n\n" + msg);
            ((System.Windows.Controls.ComboBox)obj).Focus();
            return false;
          }
          break;
        case "DatePicker":
          if (((DatePicker)obj).Text.Trim().Length == 0)
          {
            System.Windows.MessageBox.Show("Attenzione, dato obbligatorio\n\n" + msg);
            ((DatePicker)obj).Focus();
            return false;
          }
          break;
        case "RadioButton":
          //valido solo se i radiobutton si trovano sotto lo steso padre
          string group = ((System.Windows.Controls.RadioButton)obj).GroupName;

          obj = VisualTreeHelper.GetParent(obj);

          for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
          {
            DependencyObject child = VisualTreeHelper.GetChild(obj, i);
            if (child.GetType().Name == "RadioButton" && group == ((System.Windows.Controls.RadioButton)child).GroupName)
            {
              if (((System.Windows.Controls.RadioButton)child).IsChecked == true)
              {
                return true;
              }
            }
          }
          System.Windows.MessageBox.Show("Attenzione, dato obbligatorio\n\n" + msg);
          return false;
        default:
          return false;
      }

      return true;
    }

    #endregion

    #region GESTIONE_MESSAGGI_SEMPLICI


    public MessageBoxResult AvvisoPerditaDati()
    {
      string msg = "Alcuni dati sono stati modificati, confermi l'uscita?";
      MessageBoxResult result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      return result;
    }
    public MessageBoxResult AvvisoPerditaDati(string msg)
    {
      MessageBoxResult result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      return result;
    }


    public MessageBoxResult ConfermaCancellazione()
    {
      string msg = "Confermi la cancellazione?";
      MessageBoxResult result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      return result;
    }


    public MessageBoxResult ConfermaBackUp()
    {
      string msg = "Confermi esecuzione copia di salvataggio?";
      MessageBoxResult result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      return result;
    }

    public MessageBoxResult ConfermaRestore()
    {
      string msg = "Procedere con il riprino della copia di salvataggio selezionata?";
      MessageBoxResult result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      return result;
    }

    public MessageBoxResult ConfermaTrasferimentoArchivio()
    {
      string msg = "Procedere con il trasferimento dell'archivio dati Revisoft?";
      MessageBoxResult result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      return result;
    }

    public MessageBoxResult ConfermaScambioArchivio()
    {
      string msg = "Procedere con lo scambio dell'archivio dati Revisoft da Locale a Remoto o viceversa?";
      MessageBoxResult result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      return result;
    }

    public MessageBoxResult ConfermaAggiornamentoModelli()
    {
      string msg = "Procedere con l'aggiornamento dei modelli sull'archivio di rete condiviso?";
      MessageBoxResult result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      return result;
    }

    public MessageBoxResult ConfermaSettaggioArchivio()
    {
      string msg = "Confermi la riconfigurazione dell'archivio dati Revisoft?";
      MessageBoxResult result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      return result;
    }

    public MessageBoxResult ConfermaSbloccoUtenti()
    {
      string msg = "Confermi lo sblocco dello stato di tutti i clienti?";
      MessageBoxResult result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      return result;
    }

    public MessageBoxResult ConfermaSbloccoUtente()
    {
      string msg = "Confermi lo sblocco dello stato del cliente?";
      MessageBoxResult result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      return result;
    }

    public MessageBoxResult ConfermaResetArchivio()
    {
      string msg = "Confermi il reset dell'archivio dati Revisoft?\nAttenzione tutti i dati saranno persi.";
      MessageBoxResult result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      return result;
    }

    public MessageBoxResult ConfermaImportazione()
    {
      string msg = "Confermi l'importazione del cliente.";
      MessageBoxResult result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      return result;
    }

    public MessageBoxResult ConfermaEsportazione()
    {
      string msg = "Confermi l'esportazione del cliente.";
      MessageBoxResult result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      return result;
    }

    public MessageBoxResult ConfermaCondivisione()
    {
      string msg = "Confermi la condivisione del cliente.";
      MessageBoxResult result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      return result;
    }


    public string TitoloAttivita(App.TipoAttivita t)
    {
      string titolo = "";
      //Titolo attivita
      switch (t)
      {
        case App.TipoAttivita.Incarico:
          titolo = "Accettazione dell'incarico e indipendenza";
          break;
        case App.TipoAttivita.ISQC:
          titolo = "ISQC";
          break;
        case App.TipoAttivita.Revisione:
          titolo = "Comprensione - rischio - pianificazione";
          break;
        case App.TipoAttivita.Bilancio:
          titolo = "Controllo del bilancio";
          break;
        case App.TipoAttivita.Conclusione:
          titolo = "Conclusioni";
          break;
        case App.TipoAttivita.Verifica:
          titolo = "Controllo contabile";
          break;
        case App.TipoAttivita.Vigilanza:
          titolo = "Attività di vigilanza";
          break;
        case App.TipoAttivita.RelazioneB:
          titolo = "Relazione di Bilancio";
          break;
        case App.TipoAttivita.RelazioneV:
          titolo = "Relazione Vigilanza";
          break;
        case App.TipoAttivita.RelazioneBC:
          titolo = "Relazione di Bilancio Consolidato";
          break;
        case App.TipoAttivita.RelazioneVC:
          titolo = "Relazione Vigilanza Consolidato";
          break;
        case App.TipoAttivita.RelazioneBV:
          titolo = "Relazione Bilancio e Vigilanza";
          break;
        case App.TipoAttivita.PianificazioniVerifica:
          titolo = "Pianificazione verifiche periodiche";
          break;
        case App.TipoAttivita.PianificazioniVigilanza:
          titolo = "Pianificazione attività di vigilanza";
          break;
      }

      return titolo;
    }


    public MessageBoxResult ConfermiCambioMultiLicenza(string utente)
    {
      string msg = "Confermi l'attivazione della nuova Licenza intestata a \"" + utente + "\"";
      MessageBoxResult result = System.Windows.MessageBox.Show(msg, "Attenzione", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      return result;
    }

    #endregion

    #region LOG

    public void SalvaLog()
    {
      //genero file XML
      XmlManager x = new XmlManager();
      XmlDocument document = new XmlDocument();
      string logFile = string.Empty;

      App.ErrorLevel = App.ErrorTypes.Nessuno;
      logFile = App.AppLogFolder + "\\" + DateTime.Now.ToShortDateString().Replace('/', '-') + "-" + DateTime.Now.ToShortTimeString().Replace(':', '.') + EstensioneFile(App.TipoFile.Log);
      x.TipoCodifica = XmlManager.TipologiaCodifica.Normale;

      //Dati licenza
      RevisoftApplication.GestioneLicenza l = new GestioneLicenza();

      //Dati Masterfile
      MasterFile m = MasterFile.Create();

      //Durata sessione in minuti
      System.TimeSpan sessioneDurata = DateTime.Now - App.AppInizioSessione;

      string s = "";
      s += "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
      s += "<RevisoftLogFile>";
      s += "   <Data>" + DateTime.Now.ToShortDateString() + "</Data>";
      s += "   <Ora>" + DateTime.Now.ToShortTimeString() + "</Ora>";
      s += "   <Versione>" + App.AppVersione + "</Versione>";
      s += "   <LicenzaCodiceMacchina>" + l.Info_CodiceMacchina + "</LicenzaCodiceMacchina>";
      //s += "   <LicenzaTipo>" + l.NomeLicenza() + "</LicenzaTipo>";
      s += "   <LicenzaIntestatario>" + l.Intestatario + "</LicenzaIntestatario>";
      s += "   <LicenzaUtente>" + l.Utente + "</LicenzaUtente>";
      s += "   <LicenzaDataInizio>" + l.DataAttivazioneLicenza + "</LicenzaDataInizio>";
      s += "   <LicenzaDataFine>" + l.DataScadenzaLicenza + "</LicenzaDataFine>";
      s += "   <SessioneInizio>" + App.AppInizioSessione.ToString() + "</SessioneInizio>";
      s += "   <SessioneFine>" + DateTime.Now.ToString() + "</SessioneFine>";
      s += "   <SessioneDurata>" + sessioneDurata.TotalMinutes.ToString() + "</SessioneDurata>";
      s += "   <TotAnagrafiche>" + m.GetAnagraficheCount().ToString() + "</TotAnagrafiche>";
      s += "   <TotIncarichi>" + m.GetIncarichiCount().ToString() + "</TotIncarichi>";
      s += "   <TotRevisioni>" + m.GetRevisioneCount().ToString() + "</TotRevisioni>";
      s += "   <TotBilanci>" + m.GetBilanciCount().ToString() + "</TotBilanci>";
      s += "   <TotConclusioni>" + m.GetConclusioniCount().ToString() + "</TotConclusioni>";
      s += "   <TotVerifiche>" + m.GetVerificheCount().ToString() + "</TotVerifiche>";
      s += "   <TotVigilanze>" + m.GetVigilanzeCount().ToString() + "</TotVigilanze>";
      s += "   <TotRelazioniV>" + m.GetRelazioniVCount().ToString() + "</TotRelazioniV>";
      s += "   <TotRelazioniB>" + m.GetRelazioniBCount().ToString() + "</TotRelazioniB>";
      s += "   <TotRelazioniBV>" + m.GetRelazioniBVCount().ToString() + "</TotRelazioniBV>";
      s += "   <TotFlussi>" + m.GetFlussiCount().ToString() + "</TotFlussi>";
      s += "   <TotPianificazioniVerifiche>" + m.GetPianificazionePianificazioniVerificheCount().ToString() + "</TotPianificazioniVerifiche>";
      s += "   <TotPianificazioniVigilanza>" + m.GetPianificazionePianificazioniVigilanzeCount().ToString() + "</TotPianificazioniVigilanza>";
      s += "</RevisoftLogFile>";

      //salvo file
      x.SaveEncodedFile(logFile, s);
    }


    public bool InviaLog()
    {
      //MessageBox.Show("log check:" + App.urlCheckConnection);
      if (!CheckConnection(App.urlCheckConnection))
      {
        return false;
      }

      //genero file XML
      XmlManager x = new XmlManager();
      XmlDocument document = new XmlDocument();
      string logFile = string.Empty;


      //Origine
      string sourceFolder = App.AppLogFolder;
      // MessageBox.Show(sourceFolder);
      DirectoryInfo source = new DirectoryInfo(sourceFolder);

      string buff;

      // gestione file
      foreach (FileInfo fi in source.GetFiles())
      {
        //verifica formato
        if (fi.Extension == EstensioneFile(App.TipoFile.Log))
        {
          //decritto file
          logFile = App.AppLogFolder + "\\" + fi.Name;
          x.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
          document = x.LoadEncodedFile(logFile);
          buff = string.Empty;
          buff = document.InnerXml;

          //chiamata al servizio revisoft
          try
          {

            //MessageBox.Show(buff);

            string outputstring = rw.statisticheWS(buff);
            //MessageBox.Show(outputstring);
          }
          catch (Exception ex)
          {
            //MessageBox.Show(ex.Message);
            string logerror = ex.Message;
            return false;
          }

          //sposto file in cartella OLD
          logFile = App.AppOldLogFolder + "\\" + fi.Name;
          //elimino file se già presente
          if (File.Exists(logFile))
            File.Delete(logFile);
          fi.MoveTo(logFile);
        }

      }

      return true;
    }

    public bool CheckConnection(String URL)
    {
      try
      {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
        request.Timeout = 2000;
        //request.Credentials = CredentialCache.DefaultNetworkCredentials;
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        if (response.StatusCode == HttpStatusCode.OK)
        {
          response.Close();
          return true;
        }
        else
        {
          return false;
        }
      }
      catch
      {
        return false;
      }
    }



    #endregion


    #region AGGIORNAMENTO

    public bool VerificaAggiornamenti()
    {
      //MessageBox.Show(App.urlCheckConnection);
      if (!CheckConnection(App.urlCheckConnection))
      {
        return false;
      }

      App.AppSetupNuovaVersione = false;

      //Dati di invio
      string s = "";
      s += "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
      s += "<RevisoftUploader>";
      s += "   <Versione>" + "4.0.0.0" + "</Versione>";
      //s += "   <Versione>" + App.AppVersione + "</Versione>";
      //s += "   <Versione>4.9.0.0</Versione>";
      s += "   <Test>" + (App.AppTestDownload ? "1" : "0") + "</Test>";
      s += "</RevisoftUploader>";

      //MessageBox.Show(s);
      string returnstring = "";
      try
      {
        returnstring = rw.versioniWS(s);

        //MessageBox.Show(returnstring);
        int intresult = 0;

        if (int.TryParse(returnstring, out intresult))
        {
          _NomeNuovaVeresione = "";
        }
        else
        {
          _NomeNuovaVeresione = returnstring;
          App.AppSetupNuovaVersione = true;
        }

        // MessageBox.Show("after load");
      }
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
      catch (Exception e)
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata
      {
        //MessageBox.Show(e.Message);
        return false;
      }

      return true;
    }

    public void ScaricaAggiornamento()
    {
      if (_NomeNuovaVeresione != "")
      {
        using (WebClient client = new WebClient())
        {
          client.DownloadFile(_NomeNuovaVeresione, App.AppTempFolder + "\\" + _NomeNuovaVeresione.Split('/').Last());
        }
      }
    }





    public void CreaComandoAggiornamento()
    {
      string cmdFileName = _NomeNuovaVeresione.Substring(1 + _NomeNuovaVeresione.LastIndexOf('/'));
      _NomeComandoAggiornamento = App.AppTempFolder + App.UpdateCommand;

      //scrivo file
      System.IO.StreamWriter f = new System.IO.StreamWriter(_NomeComandoAggiornamento);
      f.WriteLine("@ECHO OFF");
      //lancio setup
      f.WriteLine("start /wait " + NomeFileBreve(App.AppTempFolder) + cmdFileName);
      //lancio nuovo revisoft
      //f.WriteLine("\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"");
      f.WriteLine("start " + NomeFileBreve(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
      f.Close();
    }



    #endregion


    #region ALTRO


    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern int GetShortPathName([MarshalAs(UnmanagedType.LPTStr)] string path, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder shortPath, int shortPathLength);

    public string NomeFileBreve(string filename)
    {
      StringBuilder shortPath = new StringBuilder(512);
      GetShortPathName(@filename, shortPath, shortPath.Capacity);

      return shortPath.ToString();
    }





    public void ConfiguraComboEsercizioFiscale(System.Windows.Controls.ComboBox cb)
    {
      int anno = DateTime.Now.Year;
      //aggiungo un anno
      anno++;
      for (int i = anno; i >= 2009; i--)
        cb.Items.Add(i.ToString());

    }

    //andrea 2.8.1 - errore su w7 in attivazione licenza
    public DateTime StringToDateTime(string text)
    {
      DateTime convertedDate;
      try
      {
        convertedDate = Convert.ToDateTime(text);
      }
      catch (FormatException)
      {
        text = text.Substring(0, 10);
        convertedDate = Convert.ToDateTime(text);
      }

      return convertedDate;
    }



    public void StringToImage(string text, string filename)
    {
      //canvas
      Bitmap canvas = new Bitmap(900, 20, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      //.Format64bppArgb); andrea
      Graphics gph1 = Graphics.FromImage(canvas);
      gph1.Clear(System.Drawing.Color.Transparent);

      //formato stringa
      //StringFormat sf = new StringFormat();
      //sf.LineAlignment = StringAlignment.Center;
      //sf.Alignment = StringAlignment.Center;

      //testo
      Font font = new Font("Calibri", 11, GraphicsUnit.Point);
      //16, GraphicsUnit.Pixel); andrea
      //SolidBrush brush = new SolidBrush(System.Drawing.Color.Black);

      //rotazione testo
      //gph1.RotateTransform(-45);

      //info box
      //SizeF txt = gph1.MeasureString(text, font);
      //SizeF sz = gph1.VisibleClipBounds.Size;
      //RectangleF box = new RectangleF(0, 0, sz.Width, sz.Height);

      ////centro pagina
      //string newtext = "";
      //for (int i = 0; i < text.Length; i++)
      //{
      //    if (text[i] == ' ')
      //    {
      //        newtext += text[i];
      //    }
      //    else
      //    {
      //        newtext += text[i] + " ";
      //    }
      //}

      gph1.DrawString(text, font, System.Drawing.Brushes.DarkGray, 0, 0);


      ////colonna sinistra
      //box.X = -400;
      //box.Y = -675;
      //gph1.DrawString(text, font, brush, box, sf);
      //box.X = -400;
      //box.Y = -225;
      //gph1.DrawString(text, font, brush, box, sf);
      //box.X = -400;
      //box.Y = 225;
      //gph1.DrawString(text, font, brush, box, sf);
      //box.X = -400;
      //box.Y = 675;
      //gph1.DrawString(text, font, brush, box, sf);

      ////colonna destra
      //box.X = 400;
      //box.Y = -675;
      //gph1.DrawString(text, font, brush, box, sf);
      //box.X = 400;
      //box.Y = -225;
      //gph1.DrawString(text, font, brush, box, sf);
      //box.X = 400;
      //box.Y = 225;
      //gph1.DrawString(text, font, brush, box, sf);
      //box.X = 400;
      //box.Y = 675;
      //gph1.DrawString(text, font, brush, box, sf);

      //salvataggio
      canvas.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
      canvas.Dispose();
    }


    public void CopyFolderContent(DirectoryInfo source, DirectoryInfo target)
    {
      if (source.FullName.ToLower() == target.FullName.ToLower())
      {
        return;
      }

      // Check if the target directory exists, if not, create it.
      if (Directory.Exists(target.FullName) == false)
      {
        Directory.CreateDirectory(target.FullName);
      }

      // Copy each file into it's new directory.
      foreach (FileInfo fi in source.GetFiles())
      {
        //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
        fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
      }

      // Copy each subdirectory using recursion.
      foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
      {
        DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
        CopyFolderContent(diSourceSubDir, nextTargetSubDir);
      }
    }


    public bool IsAdministrator()
    {
      return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
              .IsInRole(WindowsBuiltInRole.Administrator);
    }


    public bool VerificaIstanzeUtente()
    {
      int counter = 0;
      int sessID = System.Diagnostics.Process.GetCurrentProcess().SessionId;

      foreach (Process clsProcess in Process.GetProcesses())
      {
        if (clsProcess.ProcessName.StartsWith("Revisoft") && clsProcess.SessionId == sessID)
        {
          counter++;
        }
      }
      //process not found, return false
      return counter >= 2;
    }


    public bool VerificaAggiornamentoTemplateRemoto()
    {
      //Cartella di verifica
      string destFolder = App.AppPathArchivioRemoto + "\\" + App.TemplateFolder + "\\" + App.TemplateFolderVersioni + "\\" + App.AppVersionePrecedente;

      //Verifica esistenza
      DirectoryInfo destinazione = new DirectoryInfo(destFolder);

      return destinazione.Exists;
    }

    public bool AggiornaTemplateRemoto()
    {


      //Origine
      string sourceFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + App.ApplicationFolder + "\\" + App.ApplicationFolder + "\\" + App.TemplateFolder;

      //Destinazione
      string destFolder = App.AppPathArchivioRemoto + "\\" + App.TemplateFolder;

      //ORIGINE: Verifica esistenza
      DirectoryInfo origine = new DirectoryInfo(sourceFolder);
      if (!origine.Exists)
      {
        System.Windows.MessageBox.Show("Cartella di origine non trovata.\n\n" + sourceFolder);
        return false;
      }

      //DESTINAZIONE: Verifica esistenza
      DirectoryInfo destinazione = new DirectoryInfo(destFolder);
      if (!destinazione.Exists)
      {
        System.Windows.MessageBox.Show("Cartella di destinazione non trovata.\n\n" + destFolder);
        return false;
      }

      //Copio intero contenuto in cartella di destinazione
      CopyFolderContent(origine, destinazione);


      //andrea - v. 4.0
      string destFlussi = App.AppPathArchivioRemoto + "\\" + App.UserFileFolder + "\\" + App.UserFileFlussiFolder;
      if (!File.Exists(destFlussi))
        Directory.CreateDirectory(destFlussi);




      MasterFile.ForceRecreate();

      return true;
    }


    #endregion

  }
}
