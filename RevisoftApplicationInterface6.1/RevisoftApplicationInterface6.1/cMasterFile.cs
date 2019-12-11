// commento di prova per verificare correttezza ramo sorgente    ok!

//----------------------------------------------------------------------------+
//                               cMasterFile.cs                               |
//----------------------------------------------------------------------------+
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;
using System.Windows.Data;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using static RevisoftApplication.XmlManager;
using RevisoftApplication.BRL;

namespace RevisoftApplication
{
  class MasterFile
  {
    private WindowGestioneMessaggi message = new WindowGestioneMessaggi();
    private XmlManager x = new XmlManager();
    private XmlDocument document = new XmlDocument();
    private string file = string.Empty;

    // sqlignore
    public MasterFile()
    {
      App.ErrorLevel = App.ErrorTypes.Nessuno;
      file = App.AppMasterDataFile;
      x.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
      //Open();
      //XmlNode root = document.SelectSingleNode("/ROOT/CONCLUSIONI");
      //if (root == null)
      //{
      //    root = document.SelectSingleNode("/ROOT");
      //    string xml2 = "<CONCLUSIONI LastID=\"1\"/>";
      //    XmlDocument doctmp2 = new XmlDocument();
      //    doctmp2.LoadXml(xml2);
      //    XmlNode tmpNode2 = doctmp2.SelectSingleNode("/CONCLUSIONI");
      //    XmlNode cliente2 = document.ImportNode(tmpNode2, true);
      //    root.AppendChild(cliente2);
      //    root = document.SelectSingleNode("/ROOT/CONCLUSIONI");
      //    Save();
      //}
      //Close();
      //Open();
      //root = document.SelectSingleNode("/ROOT/VIGILANZE");
      //if (root == null)
      //{
      //    root = document.SelectSingleNode("/ROOT");
      //    string xml2 = "<VIGILANZE LastID=\"1\"/>";
      //    XmlDocument doctmp2 = new XmlDocument();
      //    doctmp2.LoadXml(xml2);
      //    XmlNode tmpNode2 = doctmp2.SelectSingleNode("/VIGILANZE");
      //    XmlNode cliente2 = document.ImportNode(tmpNode2, true);
      //    root.AppendChild(cliente2);
      //    root = document.SelectSingleNode("/ROOT/VIGILANZE");
      //    Save();
      //}
      //Close();
      //Open();
      //root = document.SelectSingleNode("/ROOT/FLUSSI");
      //if (root == null)
      //{
      //    root = document.SelectSingleNode("/ROOT");
      //    string xml2 = "<FLUSSI/>";
      //    XmlDocument doctmp2 = new XmlDocument();
      //    doctmp2.LoadXml(xml2);
      //    XmlNode tmpNode2 = doctmp2.SelectSingleNode("/FLUSSI");
      //    XmlNode cliente2 = document.ImportNode(tmpNode2, true);
      //    root.AppendChild(cliente2);
      //    root = document.SelectSingleNode("/ROOT/FLUSSI");
      //    Save();
      //}
      //Close();
    }

    private static MasterFile _instance = new MasterFile();

    public XmlDocument GetDocument() // E.B. nuovo metodo
    {
      XmlDocument d;
      Open();d = document;
      Close();
      return d;
    }

    //----------------------------------------------------------------------------+
    //                                   Create                                   |
    //----------------------------------------------------------------------------+
    // sqlignore
    public static MasterFile Create()
    {
      return _instance;
    }

    //----------------------------------------------------------------------------+
    //                               ForceRecreate                                |
    //----------------------------------------------------------------------------+
    // sqlignore
    public static void ForceRecreate()
    {
      _instance = new MasterFile();
    }

    #region Funzioni Base

    //----------------------------------------------------------------------------+
    //                              ResetMasterFile                               |
    //----------------------------------------------------------------------------+
    //----------------------------------------------------------------------------+
    //         cancella tutti gli oggetti seguenti impostando a 0 LastID          |
    //       per i flussi, vengono cancellati anche tutti i files allegati        |
    //          e il parametro LastID viene ignorato e rimane sempre a 0          |
    //                                                                            |
    //     BILANCIO, CLIENTE, CONCLUSIONE, DOCUMENTO, FLUSSO, INCARICO, ISQC,     |
    // PIANIFICAZIONIVERIFICA, PIANIFICAZIONIVIGILANZA, RELAZIONEB, RELAZIONEBC,  |
    //    RELAZIONEBV, RELAZIONEV, RELAZIONEVC, REVISIONE, VERIFICA, VIGILANZA    |
    //----------------------------------------------------------------------------+
    public void ResetMasterFile_old()
    {
      //try
      //{
      Open();
      //resetto contenuti
      XmlNodeList xNodes = document.SelectNodes("/ROOT/VERIFICHE/VERIFICA");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
        if (fi.Exists)
        {
          fi.Delete();
        }
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
        if (fd.Exists)
        {
          fd.Delete();
        }
        node.ParentNode.RemoveChild(node);
      }
      document.SelectSingleNode("/ROOT/VERIFICHE").Attributes["LastID"].Value = "0";
      xNodes = document.SelectNodes("/ROOT/VIGILANZE/VIGILANZA");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        node.ParentNode.RemoveChild(node);
      }
      document.SelectSingleNode("/ROOT/VIGILANZE").Attributes["LastID"].Value = "0";
      xNodes = document.SelectNodes("/ROOT/BILANCI/BILANCIO");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        node.ParentNode.RemoveChild(node);
      }
      document.SelectSingleNode("/ROOT/BILANCI").Attributes["LastID"].Value = "0";
      xNodes = document.SelectNodes("/ROOT/CONCLUSIONI/CONCLUSIONE");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        node.ParentNode.RemoveChild(node);
      }
      document.SelectSingleNode("/ROOT/CONCLUSIONI").Attributes["LastID"].Value = "0";
      xNodes = document.SelectNodes("/ROOT/REVISIONI/REVISIONE");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        node.ParentNode.RemoveChild(node);
      }
      document.SelectSingleNode("/ROOT/REVISIONI").Attributes["LastID"].Value = "0";
      xNodes = document.SelectNodes("/ROOT/INCARICHI/INCARICO");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        node.ParentNode.RemoveChild(node);
      }
      document.SelectSingleNode("/ROOT/INCARICHI").Attributes["LastID"].Value = "0";
      xNodes = document.SelectNodes("/ROOT/ISQCs/ISQC");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        node.ParentNode.RemoveChild(node);
      }
      document.SelectSingleNode("/ROOT/ISQCs").Attributes["LastID"].Value = "0";
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIV");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<RELAZIONIV LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIV");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
      }
      xNodes = document.SelectNodes("/ROOT/RELAZIONIV/RELAZIONEV");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        node.ParentNode.RemoveChild(node);
      }
      document.SelectSingleNode("/ROOT/RELAZIONIV").Attributes["LastID"].Value = "0";
      xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<RELAZIONIVC LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIVC");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
      }
      xNodes = document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        node.ParentNode.RemoveChild(node);
      }
      document.SelectSingleNode("/ROOT/RELAZIONIVC").Attributes["LastID"].Value = "0";
      xNode = document.SelectSingleNode("/ROOT/RELAZIONIB");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<RELAZIONIB LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIB");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
      }
      xNodes = document.SelectNodes("/ROOT/RELAZIONIB/RELAZIONEB");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        node.ParentNode.RemoveChild(node);
      }
      document.SelectSingleNode("/ROOT/RELAZIONIB").Attributes["LastID"].Value = "0";
      xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<RELAZIONIBC LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIBC");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
      }
      xNodes = document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        node.ParentNode.RemoveChild(node);
      }
      document.SelectSingleNode("/ROOT/RELAZIONIBC").Attributes["LastID"].Value = "0";
      xNode = document.SelectSingleNode("/ROOT/RELAZIONIBV");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<RELAZIONIBV LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIBV");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
      }
      xNodes = document.SelectNodes("/ROOT/RELAZIONIBV/RELAZIONEBV");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        node.ParentNode.RemoveChild(node);
      }
      document.SelectSingleNode("/ROOT/RELAZIONIBV").Attributes["LastID"].Value = "0";
      xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVERIFICHE");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<PIANIFICAZIONIVERIFICHE LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/PIANIFICAZIONIVERIFICHE");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
      }
      xNodes = document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        node.ParentNode.RemoveChild(node);
      }
      document.SelectSingleNode("/ROOT/PIANIFICAZIONIVERIFICHE").Attributes["LastID"].Value = "0";
      xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVIGILANZE");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<PIANIFICAZIONIVIGILANZE LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/PIANIFICAZIONIVIGILANZE");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
      }
      xNodes = document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        node.ParentNode.RemoveChild(node);
      }
      document.SelectSingleNode("/ROOT/PIANIFICAZIONIVIGILANZE").Attributes["LastID"].Value = "0";
      xNode = document.SelectSingleNode("/ROOT/FLUSSI");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<FLUSSI />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/FLUSSI");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
      }
      xNodes = document.SelectNodes("/ROOT/FLUSSI/FLUSSO");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
        if (fd.Exists)
        {
          //andrea - elimino allegati ai flussi
          XmlManager xf = new XmlManager();
          XmlDocument xfDoc = xf.LoadEncodedFile(fd.FullName);
          string xpath = "//Allegato";
          XmlNodeList tmpNodeList = xfDoc.SelectNodes(xpath);
          string f = "";
          foreach (XmlNode item in tmpNodeList)
          {
            f = App.AppDocumentiFlussiFolder + "\\" + item.Attributes["FILE"].Value;
            if (File.Exists(f)) File.Delete(f);
          }
          fd.Delete();
        }
        node.ParentNode.RemoveChild(node);
      }
      xNodes = document.SelectNodes("/ROOT/CLIENTI/CLIENTE");
      foreach (XmlNode node in xNodes)
      {
        node.ParentNode.RemoveChild(node);
      }
      document.SelectSingleNode("/ROOT/CLIENTI").Attributes["LastID"].Value = "0";
      Save();
      Close();
      //cancello dati
      XmlManager xdoc = new XmlManager();
      xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
      //----------------------------------------------------------------------------+
      //    App.AppDocumentiDataFile = App.AppDataFolder + "\\"
      //      + App.ApplicationFileName
      //      + EstensioneFile(App.TipoFile.DocumentiAssociati);
      //    AppDataFolder=<user>\AppData\Roaming\Revisoft\Revisoft
      //    ApplicationFileName="RevisoftApp"
      //    EstensioneFile(13)=".rdocf"
      //    --> <user>\AppData\Roaming\Revisoft\Revisoft\RevisoftApp.rdocf
      //----------------------------------------------------------------------------+
      XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
      xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fi = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        node.ParentNode.RemoveChild(node);
      }
      xdoc_doc.SelectSingleNode("//DOCUMENTI").Attributes["LastID"].Value = "0";
      xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);
      //         }
      //catch (Exception ex)
      //{
      //  string log = ex.Message;
      //  Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      //}
    }
    public void ResetMasterFile()
    {
#if (!DBG_TEST)
      ResetMasterFile_old();return;
#endif
      MessageBox.Show("funzione non disponibile");
      return;
#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
      XmlNodeList xNodes;
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
      Open();
      // cancellazione documenti allegati ai flussi
      xNodes = document.SelectNodes("/ROOT/FLUSSI/FLUSSO");
      foreach (XmlNode node in xNodes)
      {
        XmlManager xf = new XmlManager();
        XmlDocument xfDoc = xf.LoadEncodedFile(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
        string xpath = "//Allegato";
        XmlNodeList tmpNodeList = xfDoc.SelectNodes(xpath);
        string f = "";
        foreach (XmlNode item in tmpNodeList)
        {
          f = App.AppDocumentiFlussiFolder + "\\" + item.Attributes["FILE"].Value;
          if (File.Exists(f)) File.Delete(f);
        }
      }
      //Save();
      Close();
      XmlManager xdoc = new XmlManager();
      xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
      //----------------------------------------------------------------------------+
      //    App.AppDocumentiDataFile = App.AppDataFolder + "\\"
      //      + App.ApplicationFileName
      //      + EstensioneFile(App.TipoFile.DocumentiAssociati);
      //    AppDataFolder=<user>\AppData\Roaming\Revisoft\Revisoft
      //    ApplicationFileName="RevisoftApp"
      //    EstensioneFile(13)=".rdocf"
      //    --> <user>\AppData\Roaming\Revisoft\Revisoft\RevisoftApp.rdocf
      //----------------------------------------------------------------------------+
      // cancellazione documenti
      XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
      xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fi = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
      }
      //xdoc_doc.SelectSingleNode("//DOCUMENTI").Attributes["LastID"].Value = "0";
      //xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);
      //         }
      //catch (Exception ex)
      //{
      //  string log = ex.Message;
      //  Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      //}
      // cancellazione di tutti i dati nel database
    }

    public string GetClienteFissato()
    {
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/REVISOFT");
        if (xNode != null && xNode.Attributes["ClienteFissato"] != null)
        {
          Close();
          return xNode.Attributes["ClienteFissato"].Value;
        }
        else
        {
          Close();
          return null;
        }
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);
        return null;
      }
    }

    public void SetClienteFissato_old(string ID)
    {
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/REVISOFT");
        if (xNode.Attributes["ClienteFissato"] == null)
        {
          XmlAttribute attr = xNode.OwnerDocument.CreateAttribute("ClienteFissato");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["ClienteFissato"].Value = ID;
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);
      }
    }
    public void SetClienteFissato(string ID)
    {
#if (!DBG_TEST)
      SetClienteFissato_old(ID);return;
#else
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/REVISOFT");
        if (xNode.Attributes["ClienteFissato"] == null)
        {
          XmlAttribute attr = xNode.OwnerDocument.CreateAttribute("ClienteFissato");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["ClienteFissato"].Value = ID;
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.SetClienteFissato", conn);
          cmd.Parameters.AddWithValue("@ClienteFissato", ID);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SetClienteFissato(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);
      }
#endif
    }

    //----------------------------------------------------------------------------+
    //                                   Check                                    |
    //----------------------------------------------------------------------------+
    // sqlignore
    private bool Check()
    {
      if (!File.Exists(file) && File.Exists(file + ".example"))
      {
        System.IO.File.Move(file + ".example", file);
      }
      //controllo presenza File master
      if (!File.Exists(file))
      {
        ErrorCritical(WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.MancaFileMaster);
        return false;
      }
      return true;
    }

    DateTime _lastWrite = DateTime.MinValue;

    //----------------------------------------------------------------------------+
    //                               IsFileChanged                                |
    //----------------------------------------------------------------------------+
    // sqlignore
    private bool IsFileChanged()
    {
      //return true;
      if (document == null) return true;
      FileInfo fileInfo = new FileInfo(file);
      bool isCahged = fileInfo.LastWriteTime > _lastWrite;
      if (isCahged)
      {
        _lastWrite = fileInfo.LastWriteTime;
        return true;
      }
      return false;
    }

    //----------------------------------------------------------------------------+
    //                                    Open                                    |
    //----------------------------------------------------------------------------+
    private void Open_old()
    {
      if (Check())
      {
        //carico file
        if (!IsFileChanged()) return;
        try
        {
          document = x.LoadEncodedFile(file);
          Utilities u = new Utilities();
          if (!u.CheckXmlDocument(document, App.TipoFile.Master))
          {
            throw new Exception("Documento non valido. ID diverso da standard TipoFile");
          }
        }
        catch (Exception ex)
        {
          string log = ex.Message;
          document = null;
          Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);
        }
      }
    }
    private void Open()
    {
#if (!DBG_TEST)
      Open_old();return;
#endif
      //carico file
      try
      {
        document = x.LoadEncodedFile(file);
        Utilities u = new Utilities();
        if (!u.CheckXmlDocument(document, App.TipoFile.Master))
        {
          throw new Exception("Documento non valido. ID diverso da standard TipoFile");
        }
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        document = null;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);
      }
    }

    //----------------------------------------------------------------------------+
    //                                   Close                                    |
    //----------------------------------------------------------------------------+
    // sqlignore
    private void Close()
    {
      //document = new XmlDocument();
    }

    //----------------------------------------------------------------------------+
    //                                    Save                                    |
    //----------------------------------------------------------------------------+
    // sqlignore
    private void Save_old()
    {
      if (Check())
      {
        //salvo file
        try
        {
          x.SaveEncodedFile(file, document.OuterXml);
        }
        catch (Exception ex)
        {
          string log = ex.Message;
          Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
        }
      }
    }
    private void Save(bool isMod=false)
    {
      //if (Check())
      //{
        //salvo file
        try
        {
          x.SaveEncodedFile(file, document.OuterXml,isMod,isMod);
        }
        catch (Exception ex)
        {
          string log = ex.Message;
          Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
        }
      //}
    }

    //----------------------------------------------------------------------------+
    //                                   Error                                    |
    //----------------------------------------------------------------------------+
    // sqlignore
    private void Error(WindowGestioneMessaggi.TipologieMessaggiErrore wgmtme)
    {
      App.ErrorLevel = App.ErrorTypes.Errore;
      message.TipoMessaggioErrore = wgmtme;
      message.VisualizzaMessaggio();
    }

    //----------------------------------------------------------------------------+
    //                               ErrorCritical                                |
    //----------------------------------------------------------------------------+
    // sqlignore
    private void ErrorCritical(WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti wgmtme)
    {
      App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
      message.TipoMessaggioErroreBloccante = wgmtme;
      message.VisualizzaMessaggio();
    }

#endregion //---------------------------------------------------- Funzioni Base

#region Codice Macchina

    public string GetCodiceMacchinaServer()
    {
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/REVISOFT");
        return xNode.Attributes["CodiceMacchinaServer"].Value.ToString().Split('-')[0];
#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
        Close();
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);
        return null;
      }
    }

    public bool SetCodiceMacchinaServer_old(string CodiceMacchina)
    {
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/REVISOFT");
        xNode.Attributes["CodiceMacchinaServer"].Value = CodiceMacchina.Split('-')[0];
        Save();
        Close();
        return true;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);
        return false;
      }
#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
      return false;
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
    }
    public bool SetCodiceMacchinaServer(string CodiceMacchina)
    {
#if (!DBG_TEST)
      return SetCodiceMacchinaServer_old(CodiceMacchina);
#else
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/REVISOFT");
        xNode.Attributes["CodiceMacchinaServer"].Value = CodiceMacchina.Split('-')[0];
        Save();
        Close();
        return true;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);
        return false;
      }
#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
      return false;
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
#endif
    }

#endregion //-------------------------------------------------- Codice Macchina

#region Revisoft

    //----------------------------------------------------------------------------+
    //                        GetTreeAssociatoFromFileData                        |
    //----------------------------------------------------------------------------+
    public string GetTreeAssociatoFromFileData(string file)
    {
      string returnstring = "";
      Open();
      if (document.SelectSingleNode("ROOT/RELAZIONIB/RELAZIONEB[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/RELAZIONIB/RELAZIONEB[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/RELAZIONIBC/RELAZIONEBC[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/RELAZIONIBC/RELAZIONEBC[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/RELAZIONIV/RELAZIONEV[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/RELAZIONIV/RELAZIONEV[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/RELAZIONIVC/RELAZIONEVC[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/RELAZIONIVC/RELAZIONEVC[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/RELAZIONIBV/RELAZIONEBV[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/RELAZIONIBV/RELAZIONEBV[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/REVISIONI/REVISIONE[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/REVISIONI/REVISIONE[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/REVISIONI/REVISIONE[@Sigillo1_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/REVISIONI/REVISIONE[@Sigillo1_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo1_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/REVISIONI/REVISIONE[@Sigillo2_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/REVISIONI/REVISIONE[@Sigillo2_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo2_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/REVISIONI/REVISIONE[@Sigillo3_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/REVISIONI/REVISIONE[@Sigillo3_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo3_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/INCARICHI/INCARICO[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/INCARICHI/INCARICO[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/INCARICHI/INCARICO[@Sigillo1_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/INCARICHI/INCARICO[@Sigillo1_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo1_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/INCARICHI/INCARICO[@Sigillo2_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/INCARICHI/INCARICO[@Sigillo2_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo2_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/INCARICHI/INCARICO[@Sigillo3_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/INCARICHI/INCARICO[@Sigillo3_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo3_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/ISQCs/ISQC[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/ISQCs/ISQC[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/ISQCs/ISQC[@Sigillo1_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/ISQCs/ISQC[@Sigillo1_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo1_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/ISQCs/ISQC[@Sigillo2_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/ISQCs/ISQC[@Sigillo2_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo2_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/ISQCs/ISQC[@Sigillo3_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/ISQCs/ISQC[@Sigillo3_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo3_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/VERIFICHE/VERIFICA[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/VERIFICHE/VERIFICA[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/VERIFICHE/VERIFICA[@Sigillo1_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/VERIFICHE/VERIFICA[@Sigillo1_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo1_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/VERIFICHE/VERIFICA[@Sigillo2_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/VERIFICHE/VERIFICA[@Sigillo2_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo2_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/VERIFICHE/VERIFICA[@Sigillo3_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/VERIFICHE/VERIFICA[@Sigillo3_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo3_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/VIGILANZE/VIGILANZA[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/VIGILANZE/VIGILANZA[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/VIGILANZE/VIGILANZA[@Sigillo1_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/VIGILANZE/VIGILANZA[@Sigillo1_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo1_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/VIGILANZE/VIGILANZA[@Sigillo2_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/VIGILANZE/VIGILANZA[@Sigillo2_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo2_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/VIGILANZE/VIGILANZA[@Sigillo3_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/VIGILANZE/VIGILANZA[@Sigillo3_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo3_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/BILANCI/BILANCIO[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/BILANCI/BILANCIO[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/BILANCI/BILANCIO[@Sigillo1_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/BILANCI/BILANCIO[@Sigillo1_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo1_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/BILANCI/BILANCIO[@Sigillo2_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/BILANCI/BILANCIO[@Sigillo2_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo2_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/BILANCI/BILANCIO[@Sigillo3_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/BILANCI/BILANCIO[@Sigillo3_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo3_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/CONCLUSIONI/CONCLUSIONE[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/CONCLUSIONI/CONCLUSIONE[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/CONCLUSIONI/CONCLUSIONE[@Sigillo1_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/CONCLUSIONI/CONCLUSIONE[@Sigillo1_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo1_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/CONCLUSIONI/CONCLUSIONE[@Sigillo2_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/CONCLUSIONI/CONCLUSIONE[@Sigillo2_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo2_File"].Value.ToString();
      }
      if (document.SelectSingleNode("ROOT/CONCLUSIONI/CONCLUSIONE[@Sigillo3_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
      {
        returnstring = document.SelectSingleNode("ROOT/CONCLUSIONI/CONCLUSIONE[@Sigillo3_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo3_File"].Value.ToString();
      }
      Close();
      return returnstring;
    }

    //----------------------------------------------------------------------------+
    //                                GetRevisoft                                 |
    //----------------------------------------------------------------------------+
    private XmlNode GetRevisoft()
    {
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/REVISOFT");
        return xNode;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);
        return null;
      }
    }

    //----------------------------------------------------------------------------+
    //                              GetChiaveServer                               |
    //----------------------------------------------------------------------------+
    public string GetChiaveServer()
    {
      try
      {
        return GetRevisoft().Attributes["ChiaveServer"].Value;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);
        return "";
      }
    }

    public bool SetChiaveServer_old(string chiave)
    {
      try
      {
        XmlNode xNode = GetRevisoft();
        xNode.Attributes["ChiaveServer"].Value = chiave;
        Save();
        return true;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
        return false;
      }
#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
      return false;
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
    }
    public bool SetChiaveServer(string chiave)
    {
#if (!DBG_TEST)
      return SetChiaveServer_old(chiave);
#else
      bool res=true;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetChiaveServer", conn);
        cmd.Parameters.AddWithValue("@chiave", chiave);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          res = false;
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "SetChiaveServer(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      if (!res) Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      return res;
#endif
    }

    //----------------------------------------------------------------------------+
    //                                  GetData                                   |
    //----------------------------------------------------------------------------+
    public string GetData()
    {
      try
      {
        return GetRevisoft().Attributes["Data"].Value;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);
        return "";
      }
    }

    //----------------------------------------------------------------------------+
    //                                  SetData                                   |
    //----------------------------------------------------------------------------+
    public bool SetData(string chiave)
    {
      try
      {
        XmlNode xNode = GetRevisoft();
        xNode.Attributes["Data"].Value = chiave;
        Save();
        return true;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
        return false;
      }
#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
      return false;
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
    }

    public string GetDataLicenzaProva()
    {
      try
      {
        return GetRevisoft().Attributes["DataLicenzaProva"].Value;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);
        return "";
      }
    }

    public bool SetDataLicenzaProva_old(string chiave)
    {
      try
      {
        XmlNode xNode = GetRevisoft();
        xNode.Attributes["DataLicenzaProva"].Value = chiave;
        Save();
        return true;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
        return false;
      }
#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
      return false;
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
    }
    public bool SetDataLicenzaProva(string chiave)
    {
#if (!DBG_TEST)
      return SetDataLicenzaProva_old(chiave);
#else
      bool res=true;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetDataLicenzaProva", conn);
        cmd.Parameters.AddWithValue("@chiave", chiave);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          res = false;
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "SetDataLicenzaProva(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      if (!res) Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      return res;
#endif
    }

    //----------------------------------------------------------------------------+
    //                               GetDataLicenza                               |
    //----------------------------------------------------------------------------+
    public string GetDataLicenza()
    {
      try
      {
        return GetRevisoft().Attributes["DataLicenza"].Value;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);
        return "";
      }
    }

    public bool SetDataLicenza_old(string chiave)
    {
      try
      {
        XmlNode xNode = GetRevisoft();
        xNode.Attributes["DataLicenza"].Value = chiave;
        Save();
        return true;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
        return false;
      }
#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
      return false;
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
    }
    public bool SetDataLicenza(string chiave)
    {
#if (!DBG_TEST)
      return SetDataLicenza_old(chiave);
#else
      bool res = true;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetDataLicenza", conn);
        cmd.Parameters.AddWithValue("@chiave", chiave);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          res = false;
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "SetDataLicenza(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      if (!res) Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      return res;
#endif
    }

#endregion //--------------------------------------------------------- Revisoft

    public void UpdateTipoEsercisioSu239_old()
    {
      Open();
      foreach (XmlNode xNode in document.SelectNodes("/ROOT/CLIENTI/CLIENTE"))
      {
        if (xNode.Attributes["Esercizio"] == null || xNode.Attributes["EsercizioDal"] == null || xNode.Attributes["EsercizioAl"] == null)
        {
          continue;
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/REVISIONI/REVISIONE[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
        {
          if (node != null)
          {
            if (node.Attributes["Esercizio"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
              attr.Value = xNode.Attributes["Esercizio"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
              attr.Value = xNode.Attributes["EsercizioDal"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
              attr.Value = xNode.Attributes["EsercizioAl"].Value;
              node.Attributes.Append(attr);
            }
          }
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/BILANCI/BILANCIO[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
        {
          if (node != null)
          {
            if (node.Attributes["Esercizio"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
              attr.Value = xNode.Attributes["Esercizio"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
              attr.Value = xNode.Attributes["EsercizioDal"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
              attr.Value = xNode.Attributes["EsercizioAl"].Value;
              node.Attributes.Append(attr);
            }
          }
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/CONCLUSIONI/CONCLUSIONE[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
        {
          if (node != null)
          {
            if (node.Attributes["Esercizio"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
              attr.Value = xNode.Attributes["Esercizio"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
              attr.Value = xNode.Attributes["EsercizioDal"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
              attr.Value = xNode.Attributes["EsercizioAl"].Value;
              node.Attributes.Append(attr);
            }
          }
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIB/RELAZIONEB[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
        {
          if (node != null)
          {
            if (node.Attributes["Esercizio"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
              attr.Value = xNode.Attributes["Esercizio"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
              attr.Value = xNode.Attributes["EsercizioDal"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
              attr.Value = xNode.Attributes["EsercizioAl"].Value;
              node.Attributes.Append(attr);
            }
          }
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
        {
          if (node != null)
          {
            if (node.Attributes["Esercizio"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
              attr.Value = xNode.Attributes["Esercizio"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
              attr.Value = xNode.Attributes["EsercizioDal"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
              attr.Value = xNode.Attributes["EsercizioAl"].Value;
              node.Attributes.Append(attr);
            }
          }
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBV/RELAZIONEBV[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
        {
          if (node != null)
          {
            if (node.Attributes["Esercizio"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
              attr.Value = xNode.Attributes["Esercizio"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
              attr.Value = xNode.Attributes["EsercizioDal"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
              attr.Value = xNode.Attributes["EsercizioAl"].Value;
              node.Attributes.Append(attr);
            }
          }
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIV/RELAZIONEV[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
        {
          if (node != null)
          {
            if (node.Attributes["Esercizio"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
              attr.Value = xNode.Attributes["Esercizio"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
              attr.Value = xNode.Attributes["EsercizioDal"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
              attr.Value = xNode.Attributes["EsercizioAl"].Value;
              node.Attributes.Append(attr);
            }
          }
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
        {
          if (node != null)
          {
            if (node.Attributes["Esercizio"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
              attr.Value = xNode.Attributes["Esercizio"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
              attr.Value = xNode.Attributes["EsercizioDal"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
              attr.Value = xNode.Attributes["EsercizioAl"].Value;
              node.Attributes.Append(attr);
            }
          }
        }
      }
#region exists cliente
      List<XmlNode> toRemove = new List<XmlNode>();
      foreach (XmlNode node in document.SelectNodes("/ROOT/REVISIONI/REVISIONE"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/BILANCI/BILANCIO"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/CONCLUSIONI/CONCLUSIONE"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIB/RELAZIONEB"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIV/RELAZIONEV"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBV/RELAZIONEBV"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/VIGILANZE/VIGILANZA"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/VERIFICHE/VERIFICA"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode xmlElement in toRemove)
      {
        XmlNode node = xmlElement.ParentNode;
        node.RemoveChild(xmlElement);
      }
#endregion //--------------------------------------------------- exists cliente
      Save();
      Close();
    }
    public void UpdateTipoEsercisioSu239()
    {
#if (!DBG_TEST)
      UpdateTipoEsercisioSu239_old();return;
#else
      Open();
      foreach (XmlNode xNode in document.SelectNodes("/ROOT/CLIENTI/CLIENTE"))
      {
        if (xNode.Attributes["Esercizio"] == null || xNode.Attributes["EsercizioDal"] == null || xNode.Attributes["EsercizioAl"] == null)
        {
          continue;
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/REVISIONI/REVISIONE[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
        {
          if (node != null)
          {
            if (node.Attributes["Esercizio"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
              attr.Value = xNode.Attributes["Esercizio"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
              attr.Value = xNode.Attributes["EsercizioDal"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
              attr.Value = xNode.Attributes["EsercizioAl"].Value;
              node.Attributes.Append(attr);
              if (node.Attributes[App.MOD_ATTRIB] == null)
              {
                attr = node.OwnerDocument.CreateAttribute(App.MOD_ATTRIB);
                node.Attributes.Append(attr);
                node.Attributes[App.MOD_ATTRIB].Value = App.OBJ_MOD;
              }
            }
          }
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/BILANCI/BILANCIO[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
        {
          if (node != null)
          {
            if (node.Attributes["Esercizio"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
              attr.Value = xNode.Attributes["Esercizio"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
              attr.Value = xNode.Attributes["EsercizioDal"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
              attr.Value = xNode.Attributes["EsercizioAl"].Value;
              node.Attributes.Append(attr);
            }
          }
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/CONCLUSIONI/CONCLUSIONE[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
        {
          if (node != null)
          {
            if (node.Attributes["Esercizio"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
              attr.Value = xNode.Attributes["Esercizio"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
              attr.Value = xNode.Attributes["EsercizioDal"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
              attr.Value = xNode.Attributes["EsercizioAl"].Value;
              node.Attributes.Append(attr);
            }
          }
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIB/RELAZIONEB[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
        {
          if (node != null)
          {
            if (node.Attributes["Esercizio"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
              attr.Value = xNode.Attributes["Esercizio"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
              attr.Value = xNode.Attributes["EsercizioDal"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
              attr.Value = xNode.Attributes["EsercizioAl"].Value;
              node.Attributes.Append(attr);
            }
          }
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
        {
          if (node != null)
          {
            if (node.Attributes["Esercizio"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
              attr.Value = xNode.Attributes["Esercizio"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
              attr.Value = xNode.Attributes["EsercizioDal"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
              attr.Value = xNode.Attributes["EsercizioAl"].Value;
              node.Attributes.Append(attr);
            }
          }
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBV/RELAZIONEBV[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
        {
          if (node != null)
          {
            if (node.Attributes["Esercizio"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
              attr.Value = xNode.Attributes["Esercizio"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
              attr.Value = xNode.Attributes["EsercizioDal"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
              attr.Value = xNode.Attributes["EsercizioAl"].Value;
              node.Attributes.Append(attr);
            }
          }
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIV/RELAZIONEV[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
        {
          if (node != null)
          {
            if (node.Attributes["Esercizio"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
              attr.Value = xNode.Attributes["Esercizio"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
              attr.Value = xNode.Attributes["EsercizioDal"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
              attr.Value = xNode.Attributes["EsercizioAl"].Value;
              node.Attributes.Append(attr);
            }
          }
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
        {
          if (node != null)
          {
            if (node.Attributes["Esercizio"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
              attr.Value = xNode.Attributes["Esercizio"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
              attr.Value = xNode.Attributes["EsercizioDal"].Value;
              node.Attributes.Append(attr);
              attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
              attr.Value = xNode.Attributes["EsercizioAl"].Value;
              node.Attributes.Append(attr);
            }
          }
        }
      }
#region exists cliente
      List<XmlNode> toRemove = new List<XmlNode>();
      foreach (XmlNode node in document.SelectNodes("/ROOT/REVISIONI/REVISIONE"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/BILANCI/BILANCIO"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/CONCLUSIONI/CONCLUSIONE"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIB/RELAZIONEB"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIV/RELAZIONEV"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBV/RELAZIONEBV"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/VIGILANZE/VIGILANZA"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/VERIFICHE/VERIFICA"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode node in document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA"))
      {
        if (node != null)
        {
          if (node.Attributes["Cliente"] != null)
          {
            string IDCliente = node.Attributes["Cliente"].Value;
            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
            {
              toRemove.Add(node);
            }
          }
        }
      }
      foreach (XmlNode xmlElement in toRemove)
      {
        XmlNode node = xmlElement.ParentNode;
        node.RemoveChild(xmlElement);
      }
#endregion //--------------------------------------------------- exists cliente
      Save();
      Close();
#endif
    }

#region Anagrafica

    //----------------------------------------------------------------------------+
    //                            GetAnagraficaInterna                            |
    //        restituisce il nodo relativo al cliente con 'id' specificato        |
    //----------------------------------------------------------------------------+
    private XmlNode GetAnagraficaInterna(int id)
    {
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + id.ToString() + "']");
        return xNode;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);
        return null;
      }
    }

    public bool ClienteGiaPresente(Hashtable ht, int id)
    {
      Open();
      bool returnvalue = false;
      if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@PartitaIVA=\"" + ht["PartitaIVA"].ToString() + "\"][@ID!=\"" + id + "\"]") != null || document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@CodiceFiscale=\"" + ht["CodiceFiscale"].ToString() + "\"][@ID!=\"" + id + "\"]") != null)
      {
        returnvalue = true;
      }
      return returnvalue;
    }

    public int ClienteGiaPresente(Hashtable ht)
    {
      Open();
      int returnvalue = -1;
      if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@PartitaIVA=\"" + ht["PartitaIVA"].ToString() + "\"]") != null)
      {
        returnvalue = Convert.ToInt32(document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@PartitaIVA=\"" + ht["PartitaIVA"].ToString() + "\"]").Attributes["ID"].Value.ToString());
      }
      if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@CodiceFiscale=\"" + ht["CodiceFiscale"].ToString() + "\"]") != null)
      {
        returnvalue = Convert.ToInt32(document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@CodiceFiscale=\"" + ht["CodiceFiscale"].ToString() + "\"]").Attributes["ID"].Value.ToString());
      }
      return returnvalue;
    }

    //----------------------------------------------------------------------------+
    //                             InsertClientChild                              |
    //----------------------------------------------------------------------------+
    public void InsertClientChild(int ID, XmlNode node)
    {
      XmlNode xNode = GetAnagraficaInterna(ID);
      xNode.InnerText = "";
      foreach (XmlNode item in node.ChildNodes)
      {
        XmlNode xNode2 = xNode.OwnerDocument.ImportNode(item, true);
        xNode.AppendChild(xNode2);
      }
      Save();
    }

		//----------------------------------------------------------------------------+
		//                           CheckEsistenzaCliente                            |
		//----------------------------------------------------------------------------+
		public int GetCliente(Hashtable ht)
		{
			Open();
			int IDReal = -1;
			// TEAM - in caso di import il cliente viene cancellato dall'xml ma non dal db per mantenere lo stesso ID 
			// e non perdere le eventuali associazioni tra cliente e utenti  			 
			string ragioneSocialeCliente = ht["RagioneSociale"] == null ? "" : ht["RagioneSociale"].ToString();
			XmlNode xnode = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@RagioneSociale=\"" + ragioneSocialeCliente + "\"]");
			if (xnode == null) //non esiste questo cliente nell'xml
			{
				//if (cCliente.ExistCliente(Convert.ToInt32(ht["ID"]))) // esiste nel DB
				//{
				//	// i dati sul clinete si aggiornano e l'ID rimane lo stesso
				//	IDReal = SetAnagrafica(ht, Convert.ToInt32(ht["ID"]));
				//}
				//else
				//{
					//aggiungo nuovo senza avviso
					IDReal = SetAnagrafica(ht, App.MasterFile_NewID);
				//}

			}
			else
			{
				// lo stato del cliente deve ritornare a 0=disponibile quello esportato è esportto=3
				// nel file importato l'id cliente può essere diverso dal cliente presente nel db, allora se il cliente è già presente
				// si deve usare l'id presente
				int id = Convert.ToInt32(xnode.Attributes["ID"].Value);
				IDReal = SetAnagrafica(ht, id,true);
			}

			return IDReal;
		}
		public int CheckEsistenzaCliente(Hashtable ht)
		{
			Open();
			int IDReal = -1;
			//andrea
			//if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@RagioneSociale='" + ht["RagioneSociale"].ToString() + "']") == null) //non esiste questo cliente.
			if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@RagioneSociale=\"" + ((ht["RagioneSociale"] == null) ? "" : ht["RagioneSociale"].ToString()) + "\"]") == null) //non esiste questo cliente.
			{
				//aggiungo nuovo senza avviso
				IDReal = SetAnagrafica(ht, App.MasterFile_NewID);
			}
			//else if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@RagioneSociale='" + ht["RagioneSociale"].ToString() + "'][@CodiceFiscale='" + ht["CodiceFiscale"].ToString() + "']") != null)
			//{
			//    IDReal = Convert.ToInt32(document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@RagioneSociale='" + ht["RagioneSociale"].ToString() + "'][@CodiceFiscale='" + ht["CodiceFiscale"].ToString() + "']").Attributes["ID"].Value);
			//}
			return IDReal;
		}


		//----------------------------------------------------------------------------+
		//                              GetIDAnagrafica                               |
		//----------------------------------------------------------------------------+
		public string GetIDAnagrafica(string RagioneSociale)
    {
      Open();
      string IDReal = "-1";
      XmlNode node = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@RagioneSociale=\"" + ((RagioneSociale == null) ? "" : RagioneSociale) + "\"]");
      if (node != null) IDReal = node.Attributes["ID"].Value;
      return IDReal;
    }

    //----------------------------------------------------------------------------+
    //                              GetAllXmlCliente                              |
    //----------------------------------------------------------------------------+
    public bool GetAllXmlCliente_old(int id, string ret, bool Condividi)
    {
      try
      {
        Open();
        string cartellatmp = App.AppTempFolder + Guid.NewGuid().ToString();
        DirectoryInfo di = new DirectoryInfo(cartellatmp);
        if (di.Exists)
        {
          //errore directory già esistente aspettare processo terminato da parte di altro utente
          return false;
        }
        di.Create();
#pragma warning disable CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato
        bool directoryflussiesiste = false;
#pragma warning restore CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato
        string xml = "<ROOT>";
        if (!Condividi)
        {
          //andrea - versione 3.0 inserimento di codice macchina in file di esportazione - funzionalità disponibile a livello di licenza
          xml += "<LICENZA CodiceMacchinaServer=\"" + App.CodiceMacchinaServer.Split('-')[0] + "\" CodiceMacchina=\"" + App.CodiceMacchina.Split('-')[0] + "\" />";
        }
        XmlNode xNode = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + id.ToString() + "']");
        if (xNode == null) return false;
        xml += xNode.OuterXml;
        foreach (XmlNode node in document.SelectNodes("/ROOT/INCARICHI/INCARICO[@Cliente='" + id.ToString() + "']"))
        {
          FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
          }
          else
          {
            continue;
          }
          f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
          }
          else
          {
            continue;
          }
          xml += node.OuterXml;
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/ISQCs/ISQC[@Cliente='" + id.ToString() + "']"))
        {
          FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
          }
          else
          {
            continue;
          }
          f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
          }
          else
          {
            continue;
          }
          xml += node.OuterXml;
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/REVISIONI/REVISIONE[@Cliente='" + id.ToString() + "']"))
        {
          FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
          }
          else
          {
            continue;
          }
          f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
          if (f_d.Exists)
          {
            //XAML
            XmlDataProviderManager _xaml = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
            if (_xaml != null && _xaml.Document.SelectSingleNode("/Dati//Dato[@ID='274']") != null)
            {
              foreach (XmlNode tmpnode in _xaml.Document.SelectSingleNode("/Dati//Dato[@ID='274']").SelectNodes("Node[@xaml]"))
              {
                try
                {
                  FileInfo fxamlhere = new FileInfo(App.AppDataDataFolder + tmpnode.Attributes["xaml"].Value);
                  if (!fxamlhere.Exists)
                  {
                    tmpnode.Attributes.Remove(tmpnode.Attributes["xaml"]);
                  }
                  else
                  {
                    fxamlhere.CopyTo(di.FullName + "\\" + fxamlhere.Name, true);
                  }
                }
                catch (Exception ex2)
                {
                  string log = ex2.Message;
                  tmpnode.Attributes.Remove(tmpnode.Attributes["xaml"]);
                }
              }
              _xaml.Save();
            }
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
          }
          else
          {
            continue;
          }
          xml += node.OuterXml;
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/BILANCI/BILANCIO[@Cliente='" + id.ToString() + "']"))
        {
          FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
          }
          else
          {
            continue;
          }
          f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
          }
          else
          {
            continue;
          }
          xml += node.OuterXml;
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/CONCLUSIONI/CONCLUSIONE[@Cliente='" + id.ToString() + "']"))
        {
          FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
          }
          else
          {
            continue;
          }
          f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
          }
          else
          {
            continue;
          }
          xml += node.OuterXml;
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/VERIFICHE/VERIFICA[@Cliente='" + id.ToString() + "']"))
        {
          FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
          }
          else
          {
            continue;
          }
          f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
          }
          else
          {
            continue;
          }
          xml += node.OuterXml;
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/VIGILANZE/VIGILANZA[@Cliente='" + id.ToString() + "']"))
        {
          FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
          }
          else
          {
            continue;
          }
          f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
          }
          else
          {
            continue;
          }
          xml += node.OuterXml;
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/FLUSSI/FLUSSO[@Cliente='" + id.ToString() + "']"))
        {
          FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
            XmlDataProviderManager _fa = new XmlDataProviderManager(di.FullName + "\\" + node.Attributes["FileData"].Value, true);
            string xpath = "//Allegato";
            string directory = App.AppDocumentiFolder + "\\Flussi";
            foreach (XmlNode item in _fa.Document.SelectNodes(xpath))
            {
              FileInfo f_fa = new FileInfo(directory + "\\" + item.Attributes["FILE"].Value);
              if (f_fa.Exists)
              {
                DirectoryInfo newdi = new DirectoryInfo(di.FullName + "\\Flussi");
                if (newdi.Exists == false)
                {
                  newdi.Create();
                }
                directoryflussiesiste = true;
                f_fa.CopyTo(di.FullName + "\\Flussi\\" + item.Attributes["FILE"].Value, true);
              }
            }
          }
          else
          {
            continue;
          }
          xml += node.OuterXml;
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@Cliente='" + id.ToString() + "']"))
        {
          FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
          }
          else
          {
            continue;
          }
          f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
          }
          else
          {
            continue;
          }
          xml += node.OuterXml;
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@Cliente='" + id.ToString() + "']"))
        {
          FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
          }
          else
          {
            continue;
          }
          f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
          }
          else
          {
            continue;
          }
          xml += node.OuterXml;
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIB/RELAZIONEB[@Cliente='" + id.ToString() + "']"))
        {
          FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
          }
          else
          {
            continue;
          }
          f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
          }
          else
          {
            continue;
          }
          xml += node.OuterXml;
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC[@Cliente='" + id.ToString() + "']"))
        {
          FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
          }
          else
          {
            continue;
          }
          f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
          }
          else
          {
            continue;
          }
          xml += node.OuterXml;
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBV/RELAZIONEBV[@Cliente='" + id.ToString() + "']"))
        {
          FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
          }
          else
          {
            continue;
          }
          f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
          }
          else
          {
            continue;
          }
          xml += node.OuterXml;
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIV/RELAZIONEV[@Cliente='" + id.ToString() + "']"))
        {
          FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
          }
          else
          {
            continue;
          }
          f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
          }
          else
          {
            continue;
          }
          xml += node.OuterXml;
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC[@Cliente='" + id.ToString() + "']"))
        {
          FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
          }
          else
          {
            continue;
          }
          f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
          }
          else
          {
            continue;
          }
          xml += node.OuterXml;
        }
        XmlDataProviderManager _d = new XmlDataProviderManager(App.AppDocumentiDataFile, true);
        XmlNodeList nodelisttmp = _d.Document.SelectNodes("//DOCUMENTI//DOCUMENTO[@Cliente='" + id.ToString() + "']");
        int numtotdoc = 0;
        foreach (XmlNode nodetmp in nodelisttmp)
        {
          FileInfo f_d = new FileInfo(App.AppDocumentiFolder + "\\" + nodetmp.Attributes["File"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + nodetmp.Attributes["File"].Value, true);
            xml += nodetmp.OuterXml;
            numtotdoc++;
          }
        }
        xml += "</ROOT>";
        string path_fileX = di.FullName + "\\" + "all.xml";
        XmlDocument xmlTMP = new XmlDocument();
        xmlTMP.LoadXml(xml);
        XmlNodeList nodelisttmptest = xmlTMP.SelectNodes("//DOCUMENTO[@Cliente='" + id.ToString() + "']");
        //PRISCTBD
        if (numtotdoc != nodelisttmptest.Count) return false;
        foreach (XmlNode nodetmp in nodelisttmptest)
        {
          FileInfo f_d = new FileInfo(di.FullName + "\\" + nodetmp.Attributes["File"].Value);
          if (!f_d.Exists) return false;
        }
        xmlTMP.Save(path_fileX);
        Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
        zip.Password = App.ZipFilePassword;
        zip.AddDirectory(di.FullName);
        zip.Save(ret);
        FileInfo finew = new FileInfo(ret);
        char[] invalidChars = Path.GetInvalidFileNameChars();
        string RagioneSociale = new string
          (
            xNode.Attributes["RagioneSociale"].Value
            .Where(x => !invalidChars.Contains(x))
            .ToArray()
          );
        //3.6 andrea
        //string nuovofile = App.AppBackupFolder + "\\ClientiEsportati\\" + RagioneSociale + ".rief";
        //4.6 aggiungo BackUpFolder
        string nuovofile = App.AppBackupFolder + "\\" + App.ClientiEsportatiFolder + "\\" + RagioneSociale + " (" + DateTime.Now.ToShortDateString().Replace('/', '-') + "-" + DateTime.Now.ToShortTimeString().Replace(':', '.') + ").rief";
        //4.6
        DirectoryInfo ditmp = new DirectoryInfo(App.AppBackupFolder + "\\" + App.ClientiEsportatiFolder);
        if (!ditmp.Exists) ditmp.Create();
        finew.CopyTo(nuovofile, true); //Backup silenzioso dei dati cliente affidato il recupero all'help desk
        //Cancello i temporanei
        di.Delete(true);
        return true;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        return false;
      }
    }
    public bool GetAllXmlCliente(int id, string ret, bool Condividi)
    {
#if (!DBG_TEST)
  return GetAllXmlCliente_old(id, ret, Condividi);
#endif
      try
      {
        Open();
        string cartellatmp = App.AppTempFolder + Guid.NewGuid().ToString();
        DirectoryInfo di = new DirectoryInfo(cartellatmp);
        if (di.Exists)
        {
          // errore directory già esistente aspettare processo terminato da parte
          // di altro utente
          return false;
        }
        di.Create();
        string xml = "<ROOT>";
        if (!Condividi)
        {
          // versione 3.0 inserimento di codice macchina in file di esportazione
          // funzionalità disponibile a livello di licenza
          xml += "<LICENZA CodiceMacchinaServer=\"" +
          App.CodiceMacchinaServer.Split('-')[0] + "\" CodiceMacchina=\"" +
          App.CodiceMacchina.Split('-')[0] + "\" />";
        }
        XmlNode xNode = document.SelectSingleNode(
          "/ROOT/CLIENTI/CLIENTE[@ID='" + id.ToString() + "']");
        if (xNode == null) return false;
        xml += xNode.OuterXml;
        //-------------------------------------------------------------------------
        XmlDocument xmlData;
        string xmlFile, xmlFileData;
        XmlManager encoder = new XmlManager();
        encoder.TipoCodifica = TipologiaCodifica.Normale;
        //-------------------------------------------------------------------------
        //---------------------------------------------------------------- INCARICO
        foreach (XmlNode node in document.SelectNodes(
          "/ROOT/INCARICHI/INCARICO[@Cliente='" + id.ToString() + "']"))
        {
          xmlFile = node.Attributes["File"].Value; xmlFileData = node.Attributes["FileData"].Value;
          xmlData = StaticUtilities.BuildXML(xmlFile); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFile, xmlData.OuterXml);
          xmlData = StaticUtilities.BuildXML(xmlFileData); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFileData, xmlData.OuterXml);
          xml += node.OuterXml;
        }
        //-------------------------------------------------------------------- ISQC
        foreach (XmlNode node in document.SelectNodes(
          "/ROOT/ISQCs/ISQC[@Cliente='" + id.ToString() + "']"))
        {
          xmlFile = node.Attributes["File"].Value; xmlFileData = node.Attributes["FileData"].Value;
          xmlData = StaticUtilities.BuildXML(xmlFile); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFile, xmlData.OuterXml);
          xmlData = StaticUtilities.BuildXML(xmlFileData); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFileData, xmlData.OuterXml);
          xml += node.OuterXml;
        }
        //--------------------------------------------------------------- REVISIONE
        foreach (XmlNode node in document.SelectNodes(
          "/ROOT/REVISIONI/REVISIONE[@Cliente='" + id.ToString() + "']"))
        {
          xmlFile = node.Attributes["File"].Value; xmlFileData = node.Attributes["FileData"].Value;
          xmlData = StaticUtilities.BuildXML(xmlFile); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFile, xmlData.OuterXml);
          xmlData = StaticUtilities.BuildXML(xmlFileData); if (xmlData == null) continue;
          // XAML
          XmlDataProviderManager _xaml = new XmlDataProviderManager(
            App.AppDataDataFolder + "\\" + xmlFileData);
          if (_xaml != null && _xaml.Document.SelectSingleNode("/Dati//Dato[@ID='274']") != null)
          {
            foreach (XmlNode tmpnode in _xaml.Document.SelectSingleNode(
              "/Dati//Dato[@ID='274']").SelectNodes("Node[@xaml]"))
            {
              try
              {
                FileInfo fxamlhere = new FileInfo(
                  App.AppDataDataFolder + tmpnode.Attributes["xaml"].Value);
                if (!fxamlhere.Exists) tmpnode.Attributes.Remove(tmpnode.Attributes["xaml"]);
                else fxamlhere.CopyTo(di.FullName + "\\" + fxamlhere.Name, true);
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
                tmpnode.Attributes.Remove(tmpnode.Attributes["xaml"]);
              }
            }
            _xaml.isModified = true; _xaml.Save(true);
          }
          xmlData = StaticUtilities.BuildXML(xmlFileData);
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFileData, xmlData.OuterXml);
          xml += node.OuterXml;
        }
        //---------------------------------------------------------------- BILANCIO
        foreach (XmlNode node in document.SelectNodes(
          "/ROOT/BILANCI/BILANCIO[@Cliente='" + id.ToString() + "']"))
        {
          xmlFile = node.Attributes["File"].Value; xmlFileData = node.Attributes["FileData"].Value;
          xmlData = StaticUtilities.BuildXML(xmlFile); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFile, xmlData.OuterXml);
          xmlData = StaticUtilities.BuildXML(xmlFileData); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFileData, xmlData.OuterXml);
          xml += node.OuterXml;
        }
        //------------------------------------------------------------- CONCLUSIONE
        foreach (XmlNode node in document.SelectNodes(
          "/ROOT/CONCLUSIONI/CONCLUSIONE[@Cliente='" + id.ToString() + "']"))
        {
          xmlFile = node.Attributes["File"].Value; xmlFileData = node.Attributes["FileData"].Value;
          xmlData = StaticUtilities.BuildXML(xmlFile); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFile, xmlData.OuterXml);
          xmlData = StaticUtilities.BuildXML(xmlFileData); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFileData, xmlData.OuterXml);
          xml += node.OuterXml;
        }
        //---------------------------------------------------------------- VERIFICA
        foreach (XmlNode node in document.SelectNodes(
          "/ROOT/VERIFICHE/VERIFICA[@Cliente='" + id.ToString() + "']"))
        {
          xmlFile = node.Attributes["File"].Value; xmlFileData = node.Attributes["FileData"].Value;
          xmlData = StaticUtilities.BuildXML(xmlFile); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFile, xmlData.OuterXml);
          xmlData = StaticUtilities.BuildXML(xmlFileData); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFileData, xmlData.OuterXml);
          xml += node.OuterXml;
        }
        //--------------------------------------------------------------- VIGILANZA
        foreach (XmlNode node in document.SelectNodes(
          "/ROOT/VIGILANZE/VIGILANZA[@Cliente='" + id.ToString() + "']"))
        {
          xmlFile = node.Attributes["File"].Value; xmlFileData = node.Attributes["FileData"].Value;
          xmlData = StaticUtilities.BuildXML(xmlFile); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFile, xmlData.OuterXml);
          xmlData = StaticUtilities.BuildXML(xmlFileData); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFileData, xmlData.OuterXml);
          xml += node.OuterXml;
        }
        //------------------------------------------------------------------ FLUSSO
        foreach (XmlNode node in document.SelectNodes(
          "/ROOT/FLUSSI/FLUSSO[@Cliente='" + id.ToString() + "']"))
        {
          xmlFileData = node.Attributes["FileData"].Value;
          xmlData = StaticUtilities.BuildXML(xmlFileData); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFileData, xmlData.OuterXml);
          XmlDataProviderManager _fa = new XmlDataProviderManager(
            di.FullName + "\\" + xmlFileData, true);
          string xpath = "//Allegato";
          string directory = App.AppDocumentiFolder + "\\Flussi";
          foreach (XmlNode item in _fa.Document.SelectNodes(xpath))
          {
            FileInfo f_fa = new FileInfo(directory + "\\" + item.Attributes["FILE"].Value);
            if (f_fa.Exists)
            {
              DirectoryInfo newdi = new DirectoryInfo(di.FullName + "\\Flussi");
              if (newdi.Exists == false) newdi.Create();
              f_fa.CopyTo(di.FullName + "\\Flussi\\" + item.Attributes["FILE"].Value, true);
            }
          }
          xml += node.OuterXml;
        }
        //-------------------------------------------------- PIANIFICAZIONIVERIFICA
        foreach (XmlNode node in document.SelectNodes(
          "/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@Cliente='" + id.ToString() + "']"))
        {
          xmlFile = node.Attributes["File"].Value; xmlFileData = node.Attributes["FileData"].Value;
          xmlData = StaticUtilities.BuildXML(xmlFile); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFile, xmlData.OuterXml);
          xmlData = StaticUtilities.BuildXML(xmlFileData); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFileData, xmlData.OuterXml);
          xml += node.OuterXml;
        }
        //------------------------------------------------- PIANIFICAZIONIVIGILANZA
        foreach (XmlNode node in document.SelectNodes(
          "/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@Cliente='" + id.ToString() + "']"))
        {
          xmlFile = node.Attributes["File"].Value; xmlFileData = node.Attributes["FileData"].Value;
          xmlData = StaticUtilities.BuildXML(xmlFile); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFile, xmlData.OuterXml);
          xmlData = StaticUtilities.BuildXML(xmlFileData); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFileData, xmlData.OuterXml);
          xml += node.OuterXml;
        }
        //-------------------------------------------------------------- RELAZIONEB
        foreach (XmlNode node in document.SelectNodes(
          "/ROOT/RELAZIONIB/RELAZIONEB[@Cliente='" + id.ToString() + "']"))
        {
          xmlFile = node.Attributes["File"].Value; xmlFileData = node.Attributes["FileData"].Value;
          xmlData = StaticUtilities.BuildXML(xmlFile); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFile, xmlData.OuterXml);
          xmlData = StaticUtilities.BuildXML(xmlFileData); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFileData, xmlData.OuterXml);
          xml += node.OuterXml;
        }
        //------------------------------------------------------------- RELAZIONEBC
        foreach (XmlNode node in document.SelectNodes(
          "/ROOT/RELAZIONIBC/RELAZIONEBC[@Cliente='" + id.ToString() + "']"))
        {
          xmlFile = node.Attributes["File"].Value; xmlFileData = node.Attributes["FileData"].Value;
          xmlData = StaticUtilities.BuildXML(xmlFile); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFile, xmlData.OuterXml);
          xmlData = StaticUtilities.BuildXML(xmlFileData); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFileData, xmlData.OuterXml);
          xml += node.OuterXml;
        }
        //------------------------------------------------------------- RELAZIONEBV
        foreach (XmlNode node in document.SelectNodes(
          "/ROOT/RELAZIONIBV/RELAZIONEBV[@Cliente='" + id.ToString() + "']"))
        {
          xmlFile = node.Attributes["File"].Value; xmlFileData = node.Attributes["FileData"].Value;
          xmlData = StaticUtilities.BuildXML(xmlFile); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFile, xmlData.OuterXml);
          xmlData = StaticUtilities.BuildXML(xmlFileData); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFileData, xmlData.OuterXml);
          xml += node.OuterXml;
        }
        //-------------------------------------------------------------- RELAZIONEV
        foreach (XmlNode node in document.SelectNodes(
          "/ROOT/RELAZIONIV/RELAZIONEV[@Cliente='" + id.ToString() + "']"))
        {
          xmlFile = node.Attributes["File"].Value; xmlFileData = node.Attributes["FileData"].Value;
          xmlData = StaticUtilities.BuildXML(xmlFile); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFile, xmlData.OuterXml);
          xmlData = StaticUtilities.BuildXML(xmlFileData); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFileData, xmlData.OuterXml);
          xml += node.OuterXml;
        }
        //------------------------------------------------------------- RELAZIONEVC
        foreach (XmlNode node in document.SelectNodes(
          "/ROOT/RELAZIONIVC/RELAZIONEVC[@Cliente='" + id.ToString() + "']"))
        {
          xmlFile = node.Attributes["File"].Value; xmlFileData = node.Attributes["FileData"].Value;
          xmlData = StaticUtilities.BuildXML(xmlFile); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFile, xmlData.OuterXml);
          xmlData = StaticUtilities.BuildXML(xmlFileData); if (xmlData == null) continue;
          encoder.SaveEncodedFile_old(di.FullName + @"\" + xmlFileData, xmlData.OuterXml);
          xml += node.OuterXml;
        }
        //--------------------------------------------------------------- DOCUMENTO
        XmlDataProviderManager _d = new XmlDataProviderManager(App.AppDocumentiDataFile, true);
        XmlNodeList nodelisttmp = _d.Document.SelectNodes(
          "//DOCUMENTI//DOCUMENTO[@Cliente='" + id.ToString() + "']");
        int numtotdoc = 0;
        foreach (XmlNode nodetmp in nodelisttmp)
        {
          FileInfo f_d = new FileInfo(App.AppDocumentiFolder + "\\" + nodetmp.Attributes["File"].Value);
          if (f_d.Exists)
          {
            f_d.CopyTo(di.FullName + "\\" + nodetmp.Attributes["File"].Value, true);
            xml += nodetmp.OuterXml;
            numtotdoc++;
          }
        }
        xml += "</ROOT>";
        string path_fileX = di.FullName + "\\" + "all.xml";
        XmlDocument xmlTMP = new XmlDocument();
        xmlTMP.LoadXml(xml);
        XmlNodeList nodelisttmptest = xmlTMP.SelectNodes("//DOCUMENTO[@Cliente='" + id.ToString() + "']");
        //PRISCTBD
        if (numtotdoc != nodelisttmptest.Count) return false;
        foreach (XmlNode nodetmp in nodelisttmptest)
        {
          FileInfo f_d = new FileInfo(di.FullName + "\\" + nodetmp.Attributes["File"].Value);
          if (!f_d.Exists) return false;
        }
        xmlTMP.Save(path_fileX);
        Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
        zip.Password = App.ZipFilePassword;
        zip.AddDirectory(di.FullName);
        zip.Save(ret);
        FileInfo finew = new FileInfo(ret);
        char[] invalidChars = Path.GetInvalidFileNameChars();
        string RagioneSociale = new string
          (
            xNode.Attributes["RagioneSociale"].Value
            .Where(x => !invalidChars.Contains(x))
            .ToArray()
          );
        string nuovofile = App.AppBackupFolder + "\\" + App.ClientiEsportatiFolder +
          "\\" + RagioneSociale + " (" +
          DateTime.Now.ToShortDateString().Replace('/', '-') + "-" +
          DateTime.Now.ToShortTimeString().Replace(':', '.') + ").rief";
        DirectoryInfo ditmp = new DirectoryInfo(
          App.AppBackupFolder + "\\" + App.ClientiEsportatiFolder);
        if (!ditmp.Exists) ditmp.Create();
        // Backup silenzioso dei dati cliente affidato il recupero all'help desk
        finew.CopyTo(nuovofile, true);
        // Cancello i temporanei
        di.Delete(true);
        return true;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        return false;
      }
    }

    public Hashtable GetAnagrafica(int id)
    {
      Hashtable results = new Hashtable();
      try
      {
        XmlNode xNode = GetAnagraficaInterna(id);
        foreach (XmlAttribute item in xNode.Attributes)
        {
          results.Add(item.Name, item.Value);
        }
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return results;
    }

    //----------------------------------------------------------------------------+
    //                              GetAnagraficaBV                               |
    //----------------------------------------------------------------------------+
    public XmlNode GetAnagraficaBV(int id)
    {
      try
      {
        XmlNode xNode = GetAnagraficaInterna(id);
        return xNode.SelectSingleNode("BilancioVerifica");
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return null;
    }

    //----------------------------------------------------------------------------+
    //                              SetAnagraficaBV                               |
    //----------------------------------------------------------------------------+
    public bool SetAnagraficaBV_old(int id, XmlNode BVNode)
    {
      try
      {
        XmlNode xNode = GetAnagraficaInterna(id);
        XmlNode xNode2 = xNode.SelectSingleNode("BilancioVerifica");
        if (xNode2 != null)
        {
          xNode2.ParentNode.RemoveChild(xNode2);
        }
        xNode2 = xNode.OwnerDocument.ImportNode(BVNode, true);
        xNode.AppendChild(xNode2);
        Save();
        return true;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return false;
    }
    public bool SetAnagraficaBV(int id, XmlNode BVNode)
    {
#if (!DBG_TEST)
      return SetAnagraficaBV_old(id, BVNode);
#endif
      bool res = true;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetAnagraficaBV", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@BVNode", BVNode.OuterXml);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          res = false;
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "SetAnagraficaBV(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      StaticUtilities.PurgeXML("RevisoftApp.rmdf");
      return res;
    }

    public App.TipoAnagraficaStato GetAnafraficaStato(int id)
    {
      try
      {
        XmlNode xNode = GetAnagraficaInterna(id);
        return (App.TipoAnagraficaStato)Convert.ToInt32(xNode.Attributes["Stato"].Value.ToString());
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        return App.TipoAnagraficaStato.Sconosciuto;
      }
    }

    public bool SetAnafraficaStato_old(int id, App.TipoAnagraficaStato stato)
    {
      if (stato == App.TipoAnagraficaStato.Sconosciuto) return false;
      try
      {
        XmlNode xNode = GetAnagraficaInterna(id);
        if (xNode == null) return false;
        if (xNode.Attributes["Stato"] == null)
        {
          xNode.OwnerDocument.Attributes.Append(xNode.OwnerDocument.CreateAttribute("Stato"));
        }
        xNode.Attributes["Stato"].Value = ((int)(stato)).ToString();
        if (xNode.Attributes["DataModificaStato"] == null)
        {
          xNode.Attributes.Append(xNode.OwnerDocument.CreateAttribute("DataModificaStato"));
        }
        xNode.Attributes["DataModificaStato"].Value = DateTime.Now.ToShortDateString();
        if (xNode.Attributes["UtenteModificaStato"] == null)
        {
          xNode.Attributes.Append(xNode.OwnerDocument.CreateAttribute("UtenteModificaStato"));
        }
        RevisoftApplication.GestioneLicenza l = new GestioneLicenza();
        xNode.Attributes["UtenteModificaStato"].Value = l.Utente;
        Save();
        return true;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
        return false;
      }
    }
    public bool SetAnafraficaStato(int id, App.TipoAnagraficaStato stato)
    {
#if (!DBG_TEST)
      return SetAnafraficaStato_old(id, stato);
#else
      if (stato == App.TipoAnagraficaStato.Sconosciuto) return false;
      try
      {
        XmlNode xNode = GetAnagraficaInterna(id);
        if (xNode == null) return false;
        if (xNode.Attributes["Stato"] == null)
        {
          xNode.OwnerDocument.Attributes.Append(xNode.OwnerDocument.CreateAttribute("Stato"));
        }
        xNode.Attributes["Stato"].Value = ((int)(stato)).ToString();
        if (xNode.Attributes["DataModificaStato"] == null)
        {
          xNode.Attributes.Append(xNode.OwnerDocument.CreateAttribute("DataModificaStato"));
        }
        xNode.Attributes["DataModificaStato"].Value = DateTime.Now.ToShortDateString();
        if (xNode.Attributes["UtenteModificaStato"] == null)
        {
          xNode.Attributes.Append(xNode.OwnerDocument.CreateAttribute("UtenteModificaStato"));
        }
        RevisoftApplication.GestioneLicenza l = new GestioneLicenza();
        xNode.Attributes["UtenteModificaStato"].Value = l.Utente;
        XmlDocument doctmp = new XmlDocument();
        XmlNode tmpNode = doctmp.ImportNode(xNode, false);
        doctmp.AppendChild(tmpNode);
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.SetAnagrafica", conn);
          cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SetAnafraficaStato(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        return true;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
        return false;
      }
#endif
    }

    //----------------------------------------------------------------------------+
    //                      GetAnagraficaNumeroSigilliTotali                      |
    //----------------------------------------------------------------------------+
    public int GetAnagraficaNumeroSigilliTotali(int id)
    {
      int numeroSigilli = 0;
/*
#if (DBG_TEST)
      Hashtable ht = new Hashtable();
      try
      {
        ht = GetAnagrafica(id);
        if (ht["RevisoreAutonomo"].ToString() != "") { numeroSigilli++; return numeroSigilli; }
        else
        {
          if (ht["Presidente"].ToString() != "") numeroSigilli++;
          if (ht["MembroEffettivo"].ToString() != "") numeroSigilli++;
          if (ht["MembroEffettivo2"].ToString() != "") numeroSigilli++;
        }
      }
#else
*/
      try
      {
        XmlNode cliente = GetAnagraficaInterna(id);
        if (cliente["RevisoreAutonomo"] != null)
        {
          numeroSigilli = 1;
          return numeroSigilli;
        }
        else
        {
          if (cliente["Presidente"] != null)
          {
            numeroSigilli++;
          }
          if (cliente["MembroEffettivo"] != null)
          {
            numeroSigilli++;
          }
          if (cliente["MembroEffettivo2"] != null)
          {
            numeroSigilli++;
          }
        }
      }
//#endif
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return numeroSigilli;
    }

    //----------------------------------------------------------------------------+
    //                            GetAnagraficheCount                             |
    //----------------------------------------------------------------------------+
    public int GetAnagraficheCount()
    {
      ArrayList Clienti = GetAnagrafiche();
      return Clienti.Count;
    }

    public ArrayList GetAnagrafiche(bool what=false)
    {
      ArrayList results = new ArrayList();
      try
      {
			List<Cliente> clienti = null;
			ClienteComparer comp = null;


		  Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/CLIENTI/CLIENTE");
        XmlNode xNodeCF = document.SelectSingleNode("/ROOT/REVISOFT");
        foreach (XmlNode node in xNodes)
        {
          if (xNodeCF.Attributes["ClienteFissato"] != null
            && xNodeCF.Attributes["ClienteFissato"].Value != "-1"
            && xNodeCF.Attributes["ClienteFissato"].Value != ""
            && xNodeCF.Attributes["ClienteFissato"].Value != node.Attributes["ID"].Value)
          {
            if(!what)
               continue;
          }
          Hashtable result = new Hashtable();
			 // TEAM
			 // si devono vedere i soli clienti associati
			 if (App.AppTipo == App.ModalitaApp.Team || (App.AppTipo == App.ModalitaApp.StandAlone && App.AppUtente.RuoId == (int)App.RuoloDesc.RevisoreAutonomo) )
			 {
				 if (clienti == null)
				{
					clienti = cCliente.GetClientiByIdUtente(App.AppUtente.Id, App.AppRuolo);
					comp = new ClienteComparer();
				}
				Cliente cli = new Cliente() { ID = node.Attributes["ID"].Value };
				if (!clienti.Contains(cli, comp))
					continue;
			 }

          foreach (XmlAttribute item in node.Attributes)
          {
            result.Add(item.Name, item.Value);
          }
          results.Add(result);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return results;
    }

    public bool DeleteAnagrafica(string RagioneSociale)
    {
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@RagioneSociale=\"" + RagioneSociale + "\"]");
      if (xNode == null) return true;
      int id = Convert.ToInt32(xNode.Attributes["ID"].Value);
      Close();
		return DeleteAnagrafica(id, false);
    }

    //----------------------------------------------------------------------------+
    //                              DeleteAnagrafica                              |
    //----------------------------------------------------------------------------+
    public bool DeleteAnagrafica_old(int id)
    {
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + id.ToString() + "']");
        xNode.ParentNode.RemoveChild(xNode);
        foreach (XmlNode node in document.SelectNodes("/ROOT/INCARICHI/INCARICO[@Cliente='" + id.ToString() + "']"))
        {
          if (node.Attributes["File"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          if (node.Attributes["FileData"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          node.ParentNode.RemoveChild(node);
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/ISQCs/ISQC[@Cliente='" + id.ToString() + "']"))
        {
          if (node.Attributes["File"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          if (node.Attributes["FileData"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          node.ParentNode.RemoveChild(node);
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/REVISIONI/REVISIONE[@Cliente='" + id.ToString() + "']"))
        {
          if (node.Attributes["File"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          if (node.Attributes["FileData"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          node.ParentNode.RemoveChild(node);
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIV/RELAZIONEV[@Cliente='" + id.ToString() + "']"))
        {
          if (node.Attributes["File"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          if (node.Attributes["FileData"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          node.ParentNode.RemoveChild(node);
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC[@Cliente='" + id.ToString() + "']"))
        {
          if (node.Attributes["File"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          if (node.Attributes["FileData"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          node.ParentNode.RemoveChild(node);
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIB/RELAZIONEB[@Cliente='" + id.ToString() + "']"))
        {
          if (node.Attributes["File"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          if (node.Attributes["FileData"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          node.ParentNode.RemoveChild(node);
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC[@Cliente='" + id.ToString() + "']"))
        {
          if (node.Attributes["File"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          if (node.Attributes["FileData"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          node.ParentNode.RemoveChild(node);
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBV/RELAZIONEBV[@Cliente='" + id.ToString() + "']"))
        {
          if (node.Attributes["File"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          if (node.Attributes["FileData"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          node.ParentNode.RemoveChild(node);
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/BILANCI/BILANCIO[@Cliente='" + id.ToString() + "']"))
        {
          if (node.Attributes["File"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          if (node.Attributes["FileData"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          node.ParentNode.RemoveChild(node);
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/VERIFICHE/VERIFICA[@Cliente='" + id.ToString() + "']"))
        {
          if (node.Attributes["File"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          if (node.Attributes["FileData"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          node.ParentNode.RemoveChild(node);
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/VIGILANZE/VIGILANZA[@Cliente='" + id.ToString() + "']"))
        {
          if (node.Attributes["File"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          if (node.Attributes["FileData"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          node.ParentNode.RemoveChild(node);
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/CONCLUSIONI/CONCLUSIONE[@Cliente='" + id.ToString() + "']"))
        {
          if (node.Attributes["File"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          if (node.Attributes["FileData"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          node.ParentNode.RemoveChild(node);
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/FLUSSI/FLUSSO[@Cliente='" + id.ToString() + "']"))
        {
          FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
          if (f_d.Exists)
          {
            XmlDataProviderManager _fa = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value, true);
            string xpath = "//Allegato";
            string directory = App.AppDocumentiFolder + "\\Flussi";
            foreach (XmlNode item in _fa.Document.SelectNodes(xpath))
            {
              FileInfo f_fa = new FileInfo(App.AppDataDataFolder + "\\Flussi\\" + item.Attributes["FILE"].Value);
              if (f_fa.Exists)
              {
                try
                {
                  f_fa.Delete();
                }
                catch (Exception ex2)
                {
                  string log = ex2.Message;
                }
              }
            }
            try
            {
              f_d.Delete();
            }
            catch (Exception ex2)
            {
              string log = ex2.Message;
            }
          }
          node.ParentNode.RemoveChild(node);
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@Cliente='" + id.ToString() + "']"))
        {
          if (node.Attributes["File"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          if (node.Attributes["FileData"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          node.ParentNode.RemoveChild(node);
        }
        foreach (XmlNode node in document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@Cliente='" + id.ToString() + "']"))
        {
          if (node.Attributes["File"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          if (node.Attributes["FileData"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          node.ParentNode.RemoveChild(node);
        }
        XmlDataProviderManager _d = new XmlDataProviderManager(App.AppDocumentiDataFile, true);
        //XmlNodeList nodelisttmp = ;
        foreach (XmlNode nodetmp in _d.Document.SelectNodes("//DOCUMENTI//DOCUMENTO[@Cliente='" + id.ToString() + "']"))
        {
          if (nodetmp.Attributes["File"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDocumentiFolder + "\\" + nodetmp.Attributes["File"].Value);
            if (f_d.Exists)
            {
              try
              {
                f_d.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
          nodetmp.ParentNode.RemoveChild(nodetmp);
        }
        _d.Save();
        //while (nodelisttmp.Count > 0)
        //{
        //    nodelisttmp[0].ParentNode.RemoveChild(nodelisttmp[0]);
        //    nodelisttmp = _d.Document.SelectNodes("//DOCUMENTI//DOCUMENTO[@Cliente='" + id.ToString() + "']");
        //}
        Save();
        Close();
        return true;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Close();
        //Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
        return false;
      }
    }
    public bool DeleteAnagrafica(int id, bool cacellaClienteDB = true)
    {
            cBusinessObjects.DeleteCliente(id);
#if (!DBG_TEST)
      return DeleteAnagrafica_old(id);
#else
            try
      {
        XmlDataProviderManager _d = new XmlDataProviderManager(App.AppDocumentiDataFile, true);
        foreach (XmlNode nodetmp in _d.Document.SelectNodes("//DOCUMENTI//DOCUMENTO[@Cliente='" + id.ToString() + "']"))
        {
          if (nodetmp.Attributes["File"] != null)
          {
            FileInfo f_d = new FileInfo(App.AppDocumentiFolder + "\\" + nodetmp.Attributes["File"].Value);
            if (f_d.Exists)
            {
              try { f_d.Delete(); }
              catch (Exception) {}
            }
          }
        }
        _d.isModified = true;_d.Save(true);
        Open();
        foreach (XmlNode node in document.SelectNodes("/ROOT/FLUSSI/FLUSSO[@Cliente='" + id.ToString() + "']"))
        {
          XmlDataProviderManager _fa = new XmlDataProviderManager(
            App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value, true);
          string directory = App.AppDocumentiFolder + @"\Flussi\";
          foreach (XmlNode item in _fa.Document.SelectNodes("//Allegato"))
          {
            FileInfo f_fa = new FileInfo(directory + item.Attributes["FILE"].Value);
            if (f_fa.Exists)
            {
              try
              {
                f_fa.Delete();
              }
              catch (Exception ex2)
              {
                string log = ex2.Message;
              }
            }
          }
        }
        Close();
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
					SqlCommand cmd;
			// TEAM
			if (cacellaClienteDB)
				cmd = new SqlCommand("mf.DeleteAnagrafica", conn);
			 else
				cmd = new SqlCommand("mf.DeleteAnagraficaNOCliente", conn);

			 cmd.Parameters.AddWithValue("@ID", id.ToString());
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "DeleteAnagrafica(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
        return true;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Close();
        return false;
      }
#endif
    }

    public int SetAnagrafica_old(Hashtable values, int id)
    {
      int returnID = id;
      Open();
      if (id == App.MasterFile_NewID)
      {
        XmlNode root = document.SelectSingleNode(" / ROOT/CLIENTI");
        if (root.Attributes["LastID"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("LastID");
          attr.Value = "0";
          root.Attributes.Append(attr);
        }
        returnID = (Convert.ToInt32(((root.Attributes["LastID"] == null)? "0" : root.Attributes["LastID"].Value)) + 1);
        string lastindex = returnID.ToString();
        string xml = "<CLIENTE ID=\"" + lastindex + "\" Stato=\"" + ((int)(App.TipoAnagraficaStato.Disponibile)).ToString() + "\" Note=\"" + ((!values.Contains("Note")) ? "" : values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'")) + "\" EsercizioAl=\"" + ((!values.Contains("EsercizioAl")) ? "" : values["EsercizioAl"].ToString()) + "\" EsercizioDal=\"" + ((!values.Contains("EsercizioDal")) ? "" : values["EsercizioDal"].ToString()) + "\" Esercizio=\"" + ((!values.Contains("Esercizio")) ? "" : values["Esercizio"].ToString()) + "\" CodiceFiscale=\"" + ((!values.Contains("CodiceFiscale")) ? "" : values["CodiceFiscale"].ToString()) + "\" PartitaIVA=\"" + ((!values.Contains("PartitaIVA")) ? "" : values["PartitaIVA"].ToString()) + "\" RagioneSociale=\"" + ((!values.Contains("RagioneSociale")) ? "" : values["RagioneSociale"].ToString().ToString().Replace("&", "&amp;").Replace("\"", "'")) + "\" Presidente=\"" + ((!values.Contains("Presidente")) ? "" : values["Presidente"].ToString().ToString().Replace("&", "&amp;").Replace("\"", "'")) + "\" MembroEffettivo=\"" + ((!values.Contains("MembroEffettivo")) ? "" : values["MembroEffettivo"].ToString().ToString().Replace("&", "&amp;").Replace("\"", "'")) + "\" MembroEffettivo2=\"" + ((!values.Contains("MembroEffettivo2")) ? "" : values["MembroEffettivo2"].ToString().ToString().Replace("&", "&amp;").Replace("\"", "'")) + "\" RevisoreAutonomo=\"" + ((!values.Contains("RevisoreAutonomo")) ? "" : values["RevisoreAutonomo"].ToString().ToString().Replace("&", "&amp;").Replace("\"", "'")) + "\" OrganoDiControllo=\"" + ((!values.Contains("OrganoDiControllo")) ? "" : values["OrganoDiControllo"].ToString()) + "\" OrganoDiRevisione=\"" + ((!values.Contains("OrganoDiRevisione")) ? "" : values["OrganoDiRevisione"].ToString()) + "\" SindacoSupplente=\"" + ((!values.Contains("SindacoSupplente")) ? "" : values["SindacoSupplente"].ToString().ToString().Replace("&", "&amp;").Replace("\"", "'")) + "\" SindacoSupplente2=\"" + ((!values.Contains("SindacoSupplente2")) ? "" : values["SindacoSupplente2"].ToString().ToString().Replace("&", "&amp;").Replace("\"", "'")) + "\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/CLIENTE");
        XmlNode cliente = document.ImportNode(tmpNode, true);
        root.AppendChild(cliente);
        root.Attributes["LastID"].Value = lastindex;
      }
      else
      {
        XmlNode xNode = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + returnID.ToString() + "']");
        if (xNode.Attributes["Note"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("Note");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["Note"].Value = ((!values.Contains("Note")) ? "" : values["Note"].ToString());
        if (xNode.Attributes["EsercizioAl"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("EsercizioAl");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["EsercizioAl"].Value = ((!values.Contains("EsercizioAl")) ? "" : values["EsercizioAl"].ToString());
        if (xNode.Attributes["EsercizioDal"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("EsercizioDal");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["EsercizioDal"].Value = ((!values.Contains("EsercizioDal")) ? "" : values["EsercizioDal"].ToString());
        if (xNode.Attributes["Esercizio"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("Esercizio");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["Esercizio"].Value = ((!values.Contains("Esercizio")) ? "" : values["Esercizio"].ToString());
        if (xNode.Attributes["CodiceFiscale"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("CodiceFiscale");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["CodiceFiscale"].Value = ((!values.Contains("CodiceFiscale")) ? "" : values["CodiceFiscale"].ToString());
        if (xNode.Attributes["PartitaIVA"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("PartitaIVA");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["PartitaIVA"].Value = ((!values.Contains("PartitaIVA")) ? "" : values["PartitaIVA"].ToString());
        if (xNode.Attributes["RagioneSociale"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("RagioneSociale");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["RagioneSociale"].Value = ((!values.Contains("RagioneSociale")) ? "" : values["RagioneSociale"].ToString());
        if (xNode.Attributes["OrganoDiControllo"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("OrganoDiControllo");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["OrganoDiControllo"].Value = ((!values.Contains("OrganoDiControllo")) ? "" : values["OrganoDiControllo"].ToString());
        if (xNode.Attributes["OrganoDiRevisione"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("OrganoDiRevisione");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["OrganoDiRevisione"].Value = ((!values.Contains("OrganoDiRevisione")) ? "" : values["OrganoDiRevisione"].ToString());
        if (xNode.Attributes["Presidente"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("Presidente");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["Presidente"].Value = ((!values.Contains("Presidente")) ? "" : values["Presidente"].ToString());
        if (xNode.Attributes["MembroEffettivo"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("MembroEffettivo");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["MembroEffettivo"].Value = ((!values.Contains("MembroEffettivo")) ? "" : values["MembroEffettivo"].ToString());
        if (xNode.Attributes["MembroEffettivo2"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("MembroEffettivo2");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["MembroEffettivo2"].Value = ((!values.Contains("MembroEffettivo2")) ? "" : values["MembroEffettivo2"].ToString());
        if (xNode.Attributes["SindacoSupplente"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("SindacoSupplente");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["SindacoSupplente"].Value = ((!values.Contains("SindacoSupplente")) ? "" : values["SindacoSupplente"].ToString());
        if (xNode.Attributes["SindacoSupplente2"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("SindacoSupplente2");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["SindacoSupplente2"].Value = ((!values.Contains("SindacoSupplente2")) ? "" : values["SindacoSupplente2"].ToString());
        if (xNode.Attributes["RevisoreAutonomo"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("RevisoreAutonomo");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["RevisoreAutonomo"].Value = ((!values.Contains("RevisoreAutonomo")) ? "" : values["RevisoreAutonomo"].ToString());
      }
      Save();
      Close();
      return returnID;
    }
    public int SetAnagrafica(Hashtable values, int id, bool updateStato = false)
    {
           

      XmlDocument doctmp = new XmlDocument();
      XmlNode tmpNode;
      int returnID = id;
      Open();
      // nuovo cliente
      if (id == App.MasterFile_NewID)
      {
        XmlNode root = document.SelectSingleNode(" / ROOT/CLIENTI");
        if (root.Attributes["LastID"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("LastID");
          attr.Value = "0";
          root.Attributes.Append(attr);
        }

        returnID =  cBusinessObjects.GetLastIDCliente();
        string lastindex = returnID.ToString();
        string xml = "<CLIENTE ID=\"" + lastindex + "\" Stato=\"" +
          ((int)(App.TipoAnagraficaStato.Disponibile)).ToString() +
          "\" Note=\"" + ((!values.Contains("Note")) ? "" : values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'")) +
          "\" EsercizioAl=\"" + ((!values.Contains("EsercizioAl")) ? "" : values["EsercizioAl"].ToString()) +
          "\" EsercizioDal=\"" + ((!values.Contains("EsercizioDal")) ? "" : values["EsercizioDal"].ToString()) +
          "\" Esercizio=\"" + ((!values.Contains("Esercizio")) ? "" : values["Esercizio"].ToString()) +
          "\" CodiceFiscale=\"" + ((!values.Contains("CodiceFiscale")) ? "" : values["CodiceFiscale"].ToString()) +
          "\" PartitaIVA=\"" + ((!values.Contains("PartitaIVA")) ? "" : values["PartitaIVA"].ToString()) +
          "\" RagioneSociale=\"" + ((!values.Contains("RagioneSociale"))
            ? "" : values["RagioneSociale"].ToString().ToString().Replace("&", "&amp;").Replace("\"", "'")) +
          "\" Presidente=\"" + ((!values.Contains("Presidente"))
            ? "" : values["Presidente"].ToString().ToString().Replace("&", "&amp;").Replace("\"", "'")) +
          "\" MembroEffettivo=\"" + ((!values.Contains("MembroEffettivo"))
            ? "" : values["MembroEffettivo"].ToString().ToString().Replace("&", "&amp;").Replace("\"", "'")) +
          "\" MembroEffettivo2=\"" + ((!values.Contains("MembroEffettivo2"))
            ? "" : values["MembroEffettivo2"].ToString().ToString().Replace("&", "&amp;").Replace("\"", "'")) +
          "\" RevisoreAutonomo=\"" + ((!values.Contains("RevisoreAutonomo"))
            ? "" : values["RevisoreAutonomo"].ToString().ToString().Replace("&", "&amp;").Replace("\"", "'")) +
          "\" OrganoDiControllo=\"" + ((!values.Contains("OrganoDiControllo")) ? "" : values["OrganoDiControllo"].ToString()) +
          "\" OrganoDiRevisione=\"" + ((!values.Contains("OrganoDiRevisione")) ? "" : values["OrganoDiRevisione"].ToString()) +
          "\" SindacoSupplente=\"" + ((!values.Contains("SindacoSupplente"))
            ? "" : values["SindacoSupplente"].ToString().ToString().Replace("&", "&amp;").Replace("\"", "'")) +
          "\" SindacoSupplente2=\"" + ((!values.Contains("SindacoSupplente2"))
            ? "" : values["SindacoSupplente2"].ToString().ToString().Replace("&", "&amp;").Replace("\"", "'")) +"\" />";
        doctmp.LoadXml(xml);
        tmpNode = doctmp.SelectSingleNode("/CLIENTE");
        XmlNode cliente = document.ImportNode(tmpNode, true);
        root.AppendChild(cliente);
        root.Attributes["LastID"].Value = lastindex;
      }
      else
      {
        XmlNode xNode = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + returnID.ToString() + "']");
        if (xNode.Attributes["Note"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("Note");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["Note"].Value = ((!values.Contains("Note")) ? "" : values["Note"].ToString());
        if (xNode.Attributes["EsercizioAl"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("EsercizioAl");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["EsercizioAl"].Value = ((!values.Contains("EsercizioAl")) ? "" : values["EsercizioAl"].ToString());
        if (xNode.Attributes["EsercizioDal"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("EsercizioDal");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["EsercizioDal"].Value = ((!values.Contains("EsercizioDal")) ? "" : values["EsercizioDal"].ToString());
        if (xNode.Attributes["Esercizio"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("Esercizio");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["Esercizio"].Value = ((!values.Contains("Esercizio")) ? "" : values["Esercizio"].ToString());
        if (xNode.Attributes["CodiceFiscale"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("CodiceFiscale");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["CodiceFiscale"].Value = ((!values.Contains("CodiceFiscale")) ? "" : values["CodiceFiscale"].ToString());
        if (xNode.Attributes["PartitaIVA"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("PartitaIVA");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["PartitaIVA"].Value = ((!values.Contains("PartitaIVA")) ? "" : values["PartitaIVA"].ToString());
        if (xNode.Attributes["RagioneSociale"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("RagioneSociale");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["RagioneSociale"].Value = ((!values.Contains("RagioneSociale")) ? "" : values["RagioneSociale"].ToString());
        if (xNode.Attributes["OrganoDiControllo"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("OrganoDiControllo");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["OrganoDiControllo"].Value = ((!values.Contains("OrganoDiControllo")) ? "" : values["OrganoDiControllo"].ToString());
        if (xNode.Attributes["OrganoDiRevisione"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("OrganoDiRevisione");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["OrganoDiRevisione"].Value = ((!values.Contains("OrganoDiRevisione")) ? "" : values["OrganoDiRevisione"].ToString());
        if (xNode.Attributes["Presidente"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("Presidente");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["Presidente"].Value = ((!values.Contains("Presidente")) ? "" : values["Presidente"].ToString());
        if (xNode.Attributes["MembroEffettivo"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("MembroEffettivo");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["MembroEffettivo"].Value = ((!values.Contains("MembroEffettivo")) ? "" : values["MembroEffettivo"].ToString());
        if (xNode.Attributes["MembroEffettivo2"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("MembroEffettivo2");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["MembroEffettivo2"].Value = ((!values.Contains("MembroEffettivo2")) ? "" : values["MembroEffettivo2"].ToString());
        if (xNode.Attributes["SindacoSupplente"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("SindacoSupplente");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["SindacoSupplente"].Value = ((!values.Contains("SindacoSupplente")) ? "" : values["SindacoSupplente"].ToString());
        if (xNode.Attributes["SindacoSupplente2"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("SindacoSupplente2");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["SindacoSupplente2"].Value = ((!values.Contains("SindacoSupplente2")) ? "" : values["SindacoSupplente2"].ToString());
        if (xNode.Attributes["RevisoreAutonomo"] == null)
        {
          XmlAttribute attr = document.CreateAttribute("RevisoreAutonomo");
          xNode.Attributes.Append(attr);
        }
        xNode.Attributes["RevisoreAutonomo"].Value = ((!values.Contains("RevisoreAutonomo")) ? "" : values["RevisoreAutonomo"].ToString());
		  if (updateStato)
		  {
				xNode.Attributes["Stato"].Value = ((int)(App.TipoAnagraficaStato.Disponibile)).ToString();
		  }
		  doctmp = new XmlDocument();
        tmpNode=doctmp.ImportNode(xNode, false);
        doctmp.AppendChild(tmpNode);
      }
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
			// la store procedure mf.SetAnagrafica se il cliente non esiste lo inserisce, mentre se esiste lo aggiorna
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetAnagrafica", conn);
        cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "SetAnagrafica(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      Save();
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      Close();
      return returnID;

    }

#endregion //------------------------------------------------------- Anagrafica

#region incarico

    public int GetIncarichiCount()
    {
      int result = 0;
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/INCARICHI/INCARICO");
        if (xNodes != null) result = xNodes.Count;
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    public ArrayList GetIncarichi(string IDCliente,string area1="")
    {
      ArrayList results = new ArrayList();
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/INCARICHI/INCARICO[@Cliente='" + IDCliente + "']");
        foreach (XmlNode node in xNodes)
        {
         if( node.Attributes["Area1"] ==null || node.Attributes["Area1"].Value.Trim() == area1)
         {
                  Hashtable result = new Hashtable();
                  foreach (XmlAttribute item in node.Attributes)
                  {
                    result.Add(item.Name, item.Value);
                  }
                  results.Add(result);
         }  
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return results;
    }

    public Hashtable GetIncarico(string IDIncarico)
    {
      Hashtable result = new Hashtable();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/INCARICHI/INCARICO[@ID='" + IDIncarico + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                                AddIncarico                                 |
    //----------------------------------------------------------------------------+
    public string AddIncarico_old(XmlNode node)
    {
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/INCARICHI");
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      XmlNode xtmp = document.ImportNode(node, true);
      xNode.AppendChild(xtmp);
      xNode.Attributes["LastID"].Value = ID;
      Save();
      Close();
      return ID;
    }
    public string AddIncarico(XmlNode node)
    {
#if (!DBG_TEST)
      return AddIncarico_old(node);
#endif
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/INCARICHI");
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetIncarico", conn);
        cmd.Parameters.AddWithValue("@IDIncarico", ID);
        cmd.Parameters.AddWithValue("@rec", node.OuterXml);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "AddIncarico(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      Save();
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      Close();
      return ID;
    }

    //----------------------------------------------------------------------------+
    //                               DeleteIncarico                               |
    //----------------------------------------------------------------------------+
    public void DeleteIncarico_old(int IDIncarico)
    {
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/INCARICHI/INCARICO[@ID='" + IDIncarico.ToString() + "']");
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        xNode.ParentNode.RemoveChild(xNode);
        Save();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDIncarico + "'][@Tree='" + (Convert.ToInt32(App.TipoFile.Incarico)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
          node.ParentNode.RemoveChild(node);
        }
        xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
    }
    public void DeleteIncarico(int IDIncarico, string idcliente,string area1)
     {

      try
      {
         if(area1.Trim()=="")
             cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.Incarico)).ToString(), idcliente);
           if(area1.Trim()=="CS")
            cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.IncaricoCS)).ToString(), idcliente);
          if(area1.Trim()=="SU")
            cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.IncaricoSU)).ToString(), idcliente);
           if(area1.Trim()=="REV")
           cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.IncaricoREV)).ToString(), idcliente);

       
        cBusinessObjects.DeleteSessione("Incarico"+area1,IDIncarico, idcliente);
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/INCARICHI/INCARICO[@ID='" + IDIncarico.ToString() + "']");
        if (App.m_xmlCache.Contains(xNode.Attributes["File"].Value)) App.m_xmlCache.Remove(xNode.Attributes["File"].Value);
        if (App.m_xmlCache.Contains(xNode.Attributes["FileData"].Value)) App.m_xmlCache.Remove(xNode.Attributes["FileData"].Value);
        Close();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes(
          "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDIncarico + "'][@Tree='" +
          (Convert.ToInt32(App.TipoFile.Incarico)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.DeleteIncarico", conn);
          cmd.Parameters.AddWithValue("@ID", IDIncarico.ToString());
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
                            cBusinessObjects.hide_workinprogress();
                            string msg = "DeleteIncarico(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
      }
      catch (Exception ex)
      {
                cBusinessObjects.hide_workinprogress();
                string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
            cBusinessObjects.hide_workinprogress();
        }

    //----------------------------------------------------------------------------+
    //                            CheckDoppio_incarico                            |
    //----------------------------------------------------------------------------+
    public bool CheckDoppio_incarico(int ID, int IDCliente, string Data, string area1)
    {
      Open();
      XmlNodeList xNodes = document.SelectNodes("/ROOT/INCARICHI/INCARICO");
      foreach (XmlNode node in xNodes)
      {
        if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString()
          && node.Attributes["DataNomina"].Value == Data && (node.Attributes["Area1"] ==null || node.Attributes["Area1"].Value.Trim() == area1))
        {
          Close();
          return false;
        }
      }
      Close();
      return true;
    }


        //----------------------------------------------------------------------------+
        //                                SetIncarico                                 |
        //----------------------------------------------------------------------------+

        public int SetIncarico(Hashtable values, int IDIncarico, int IDCliente)
    {
      


            string newNametree, newNamedati;
      try
      {
        Open();
        if (IDIncarico == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/INCARICHI");
          IDIncarico = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDIncarico.ToString();
          newNametree = App.AppTemplateTreeIncarico; newNametree = newNametree.Split('\\').Last();
          newNamedati = App.AppTemplateDataIncarico; newNamedati= newNamedati.Split('\\').Last();
          string xml = "<INCARICO ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" +
            ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
            "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" DataNomina=\"" + values["DataNomina"].ToString() +
            "\" Area1=\"" + values["Area1"].ToString() + "\" Composizione=\"" +
            values["Composizione"].ToString() + "\" Attivita=\"" + values["Attivita"].ToString() + "\"  />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.NewIncarico", conn);
            cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetIncarico(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        else
        {
        
          XmlNode xNode = document.SelectSingleNode("/ROOT/INCARICHI/INCARICO[@ID='" + IDIncarico.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          if(xNode.Attributes["DataNomina"].Value!=values["DataNomina"].ToString())
          {
           if(values["Area1"].ToString().Trim()=="")
             cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.Incarico)).ToString(), IDCliente.ToString());
           if(values["Area1"].ToString().Trim()=="CS")
             cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.IncaricoCS)).ToString(), IDCliente.ToString());
          if(values["Area1"].ToString().Trim()=="SU")
             cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.IncaricoSU)).ToString(), IDCliente.ToString());
           if(values["Area1"].ToString().Trim()=="REV")
             cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.IncaricoREV)).ToString(), IDCliente.ToString());
              
          }
          

          xNode.Attributes["DataNomina"].Value = values["DataNomina"].ToString();
          xNode.Attributes["Composizione"].Value = values["Composizione"].ToString();
          xNode.Attributes["Attivita"].Value = values["Attivita"].ToString();
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.SetIncarico", conn);
            cmd.Parameters.AddWithValue("@IDIncarico", IDIncarico.ToString());
            cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetIncarico(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDIncarico;
    }

    //----------------------------------------------------------------------------+
    //                             SetSigilloIncarico                             |
    //----------------------------------------------------------------------------+
    public int SetSigilloIncarico_old(int IDIncarico, string revisore, string password)
    {
      try
      {
        Open();
        if (IDIncarico == App.MasterFile_NewID)
        {
          ;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/INCARICHI/INCARICO[@ID='" + IDIncarico.ToString() + "']");
          if (xNode.Attributes["Sigillo"] == null)
          {
            xNode.Attributes.Append(document.CreateAttribute("Sigillo"));
          }
          if (xNode.Attributes["Sigillo_Password"] == null)
          {
            xNode.Attributes.Append(document.CreateAttribute("Sigillo_Password"));
          }
          if (xNode.Attributes["Sigillo_Data"] == null)
          {
            xNode.Attributes.Append(document.CreateAttribute("Sigillo_Data"));
          }
          xNode.Attributes["Sigillo"].Value = revisore;
          xNode.Attributes["Sigillo_Password"].Value = password;
          xNode.Attributes["Sigillo_Data"].Value = DateTime.Now.ToShortDateString();
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDIncarico;
    }
    public int SetSigilloIncarico(int IDIncarico, string revisore, string password)
    {
#if (!DBG_TEST)
      return SetSigilloIncarico_old(IDIncarico, revisore, password);
#endif
      if (IDIncarico == App.MasterFile_NewID) return IDIncarico;
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/INCARICHI/INCARICO[@ID='" + IDIncarico.ToString() + "']");
        if (xNode.Attributes["Sigillo"] == null)
        {
          xNode.Attributes.Append(document.CreateAttribute("Sigillo"));
        }
        if (xNode.Attributes["Sigillo_Password"] == null)
        {
          xNode.Attributes.Append(document.CreateAttribute("Sigillo_Password"));
        }
        if (xNode.Attributes["Sigillo_Data"] == null)
        {
          xNode.Attributes.Append(document.CreateAttribute("Sigillo_Data"));
        }
        xNode.Attributes["Sigillo"].Value = revisore;
        xNode.Attributes["Sigillo_Password"].Value = password;
        xNode.Attributes["Sigillo_Data"].Value = DateTime.Now.ToShortDateString();
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.SetIncarico", conn);
          cmd.Parameters.AddWithValue("@IDIncarico", IDIncarico.ToString());
          cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SetSigilloIncarico(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDIncarico;
    }

    //----------------------------------------------------------------------------+
    //                           RemoveSigilloIncarico                            |
    //----------------------------------------------------------------------------+
    public int RemoveSigilloIncarico_old(int IDIncarico)
    {
      try
      {
        Open();
        if (IDIncarico == App.MasterFile_NewID)
        {
          ;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/INCARICHI/INCARICO[@ID='" + IDIncarico.ToString() + "']");
          if (xNode.Attributes["Sigillo"] != null)
          {
            xNode.Attributes.Remove(xNode.Attributes["Sigillo"]);
          }
          if (xNode.Attributes["Sigillo_Password"] != null)
          {
            xNode.Attributes.Remove(xNode.Attributes["Sigillo_Password"]);
          }
          if (xNode.Attributes["Sigillo_Data"] != null)
          {
            xNode.Attributes.Remove(xNode.Attributes["Sigillo_Data"]);
          }
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDIncarico;
    }
    public int RemoveSigilloIncarico(int IDIncarico)
    {
#if (!DBG_TEST)
      return RemoveSigilloIncarico_old(IDIncarico);
#endif
      if (IDIncarico == App.MasterFile_NewID) return IDIncarico;
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/INCARICHI/INCARICO[@ID='" + IDIncarico.ToString() + "']");
        if (xNode.Attributes["Sigillo"] != null)
        {
          xNode.Attributes.Remove(xNode.Attributes["Sigillo"]);
        }
        if (xNode.Attributes["Sigillo_Password"] != null)
        {
          xNode.Attributes.Remove(xNode.Attributes["Sigillo_Password"]);
        }
        if (xNode.Attributes["Sigillo_Data"] != null)
        {
          xNode.Attributes.Remove(xNode.Attributes["Sigillo_Data"]);
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.SetIncarico", conn);
          cmd.Parameters.AddWithValue("@IDIncarico", IDIncarico.ToString());
          cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "RemoveSigilloIncarico(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDIncarico;
    }

#endregion //--------------------------------------------------------- incarico

#region ISQC

    //----------------------------------------------------------------------------+
    //                               GetISQCsCount                                |
    //----------------------------------------------------------------------------+
    public int GetISQCsCount()
    {
      int result = 0;
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/ISQCs/ISQC");
        if (xNodes != null) result = xNodes.Count;
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                                  GetISQCs                                  |
    //----------------------------------------------------------------------------+
    public ArrayList GetISQCs(string IDCliente)
    {
      ArrayList results = new ArrayList();
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/ISQCs/ISQC[@Cliente='" + IDCliente + "']");
        foreach (XmlNode node in xNodes)
        {
          Hashtable result = new Hashtable();
          foreach (XmlAttribute item in node.Attributes)
          {
            result.Add(item.Name, item.Value);
          }
          results.Add(result);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return results;
    }

    //----------------------------------------------------------------------------+
    //                                  GetISQC                                   |
    //----------------------------------------------------------------------------+
    public Hashtable GetISQC(string IDISQC)
    {
      Hashtable result = new Hashtable();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/ISQCs/ISQC[@ID='" + IDISQC + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                                  AddISQC                                   |
    //----------------------------------------------------------------------------+
    public string AddISQC_old(XmlNode node)
    {
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/ISQCs");
      if (document.SelectNodes("/ROOT/ISQCs") == null || document.SelectNodes("/ROOT/ISQCs").Count == 0)
      {
        string xmlISQC = "<ISQCs LastID=\"1\" />";
        XmlDocument doctmpISQC = new XmlDocument();
        doctmpISQC.LoadXml(xmlISQC);
        XmlNode tmpNodeISQC = doctmpISQC.SelectSingleNode("/ISQCs");
        XmlNode clienteISQC = document.ImportNode(tmpNodeISQC, true);
        document.SelectSingleNode("/ROOT").AppendChild(clienteISQC);
        xNode = document.SelectSingleNode("/ROOT/ISQCs");
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      XmlNode xtmp = document.ImportNode(node, true);
      xNode.AppendChild(xtmp);
      xNode.Attributes["LastID"].Value = ID;
      Save();
      Close();
      return ID;
    }
    public string AddISQC(XmlNode node)
    {
#if (!DBG_TEST)
      return AddISQC_old(node);
#endif
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/ISQCs");
      if (document.SelectNodes("/ROOT/ISQCs") == null || document.SelectNodes("/ROOT/ISQCs").Count == 0)
      {
        string xmlISQC = "<ISQCs LastID=\"1\" />";
        XmlDocument doctmpISQC = new XmlDocument();
        doctmpISQC.LoadXml(xmlISQC);
        XmlNode tmpNodeISQC = doctmpISQC.SelectSingleNode("/ISQCs");
        XmlNode clienteISQC = document.ImportNode(tmpNodeISQC, true);
        document.SelectSingleNode("/ROOT").AppendChild(clienteISQC);
        xNode = document.SelectSingleNode("/ROOT/ISQCs");
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetISQC", conn);
        cmd.Parameters.AddWithValue("@IDISQC", ID);
        cmd.Parameters.AddWithValue("@rec", node.OuterXml);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "AddISQC(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      Save();
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      Close();
      return ID;
    }

    //----------------------------------------------------------------------------+
    //                                 DeleteISQC                                 |
    //----------------------------------------------------------------------------+
    public void DeleteISQC_old(int IDISQC)
    {
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/ISQCs/ISQC[@ID='" + IDISQC.ToString() + "']");
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        xNode.ParentNode.RemoveChild(xNode);
        Save();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDISQC + "'][@Tree='" + (Convert.ToInt32(App.TipoFile.ISQC)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
          node.ParentNode.RemoveChild(node);
        }
        xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
    }
    public void DeleteISQC(int IDISQC, string idcliente)
        {

      try
      {
                cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.ISQC)).ToString(), idcliente);
                cBusinessObjects.DeleteSessione("ISQC",IDISQC, idcliente);
                Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/ISQCs/ISQC[@ID='" + IDISQC.ToString() + "']");
        if (App.m_xmlCache.Contains(xNode.Attributes["File"].Value)) App.m_xmlCache.Remove(xNode.Attributes["File"].Value);
        if (App.m_xmlCache.Contains(xNode.Attributes["FileData"].Value)) App.m_xmlCache.Remove(xNode.Attributes["FileData"].Value);
        Close();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes(
          "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDISQC + "'][@Tree='" +
          (Convert.ToInt32(App.TipoFile.ISQC)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.DeleteISQC", conn);
          cmd.Parameters.AddWithValue("@IDISQC", IDISQC.ToString());
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
                            cBusinessObjects.hide_workinprogress();
                            string msg = "DeleteISQC(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
      }
      catch (Exception ex)
      {
        string log = ex.Message;
                cBusinessObjects.hide_workinprogress();

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
            cBusinessObjects.hide_workinprogress();
        }

    //----------------------------------------------------------------------------+
    //                              CheckDoppio_ISQC                              |
    //----------------------------------------------------------------------------+
    public bool CheckDoppio_ISQC(int ID, int IDCliente, string Data)
    {
      Open();
      XmlNodeList xNodes = document.SelectNodes("/ROOT/ISQCs/ISQC");
      foreach (XmlNode node in xNodes)
      {
        if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["DataNomina"].Value == Data)
        {
          Close();
          return false;
        }
      }
      Close();
      return true;
    }

    //----------------------------------------------------------------------------+
    //                                  SetISQC                                   |
    //----------------------------------------------------------------------------+
    
    public int SetISQC(Hashtable values, int IDISQC, int IDCliente)
    {
         cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.ISQC)).ToString(), IDCliente.ToString());

           
            string newNametree, newNamedati;
      try
      {
        Open();
        if (IDISQC == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/ISQCs");
          if (document.SelectNodes("/ROOT/ISQCs") == null || document.SelectNodes("/ROOT/ISQCs").Count == 0)
          {
            string xmlISQC = "<ISQCs LastID=\"1\" />";
            XmlDocument doctmpISQC = new XmlDocument();
            doctmpISQC.LoadXml(xmlISQC);
            XmlNode tmpNodeISQC = doctmpISQC.SelectSingleNode("/ISQCs");
            XmlNode clienteISQC = document.ImportNode(tmpNodeISQC, true);
            document.SelectSingleNode("/ROOT").AppendChild(clienteISQC);
            root = document.SelectSingleNode("/ROOT/ISQCs");
          }
          IDISQC = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDISQC.ToString();
          newNametree = App.AppTemplateTreeISQC; newNametree = newNametree.Split('\\').Last();
          newNamedati = App.AppTemplateDataISQC; newNamedati = newNamedati.Split('\\').Last();
          string xml = "<ISQC ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" +
            ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
            "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" DataNomina=\"" + values["DataNomina"].ToString() + "\" DataFine=\"" +
            ((values["DataFine"] != null) ? values["DataFine"].ToString() : "") + "\" Composizione=\"" +
            values["Composizione"].ToString() + "\" Attivita=\"" + values["Attivita"].ToString() + "\"  />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.NewISQC", conn);
            cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetISQC(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/ISQCs/ISQC[@ID='" + IDISQC.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          xNode.Attributes["DataNomina"].Value = values["DataNomina"].ToString();
          if (values["DataFine"] != null)
          {
            if (xNode.Attributes["DataFine"] == null)
            {
              xNode.Attributes.Append(document.CreateAttribute("DataFine"));
            }
            xNode.Attributes["DataFine"].Value = values["DataFine"].ToString();
          }
          xNode.Attributes["Composizione"].Value = values["Composizione"].ToString();
          xNode.Attributes["Attivita"].Value = values["Attivita"].ToString(); ;
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.SetISQC", conn);
            cmd.Parameters.AddWithValue("@IDISQC", IDISQC.ToString());
            cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetISQC(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDISQC;
    }

    //----------------------------------------------------------------------------+
    //                               SetSigilloISQC                               |
    //----------------------------------------------------------------------------+
    public int SetSigilloISQC_old(int IDISQC, string revisore, string password)
    {
      try
      {
        Open();
        if (IDISQC == App.MasterFile_NewID)
        {
          ;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/ISQCs/ISQC[@ID='" + IDISQC.ToString() + "']");
          if (xNode.Attributes["Sigillo"] == null)
          {
            xNode.Attributes.Append(document.CreateAttribute("Sigillo"));
          }
          if (xNode.Attributes["Sigillo_Password"] == null)
          {
            xNode.Attributes.Append(document.CreateAttribute("Sigillo_Password"));
          }
          if (xNode.Attributes["Sigillo_Data"] == null)
          {
            xNode.Attributes.Append(document.CreateAttribute("Sigillo_Data"));
          }
          xNode.Attributes["Sigillo"].Value = revisore;
          xNode.Attributes["Sigillo_Password"].Value = password;
          xNode.Attributes["Sigillo_Data"].Value = DateTime.Now.ToShortDateString();
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDISQC;
    }
    public int SetSigilloISQC(int IDISQC, string revisore, string password)
    {
#if (!DBG_TEST)
      return SetSigilloISQC_old(IDISQC, revisore, password);
#endif
      if (IDISQC == App.MasterFile_NewID) return IDISQC;
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/ISQCs/ISQC[@ID='" + IDISQC.ToString() + "']");
        if (xNode.Attributes["Sigillo"] == null)
        {
          xNode.Attributes.Append(document.CreateAttribute("Sigillo"));
        }
        if (xNode.Attributes["Sigillo_Password"] == null)
        {
          xNode.Attributes.Append(document.CreateAttribute("Sigillo_Password"));
        }
        if (xNode.Attributes["Sigillo_Data"] == null)
        {
          xNode.Attributes.Append(document.CreateAttribute("Sigillo_Data"));
        }
        xNode.Attributes["Sigillo"].Value = revisore;
        xNode.Attributes["Sigillo_Password"].Value = password;
        xNode.Attributes["Sigillo_Data"].Value = DateTime.Now.ToShortDateString();
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.SetISQC", conn);
          cmd.Parameters.AddWithValue("@IDISQC", IDISQC.ToString());
          cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SetSigilloISQC(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDISQC;
    }

    //----------------------------------------------------------------------------+
    //                             RemoveSigilloISQC                              |
    //----------------------------------------------------------------------------+
    public int RemoveSigilloISQC_old(int IDISQC)
    {
      try
      {
        Open();
        if (IDISQC == App.MasterFile_NewID)
        {
          ;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/ISQCs/ISQC[@ID='" + IDISQC.ToString() + "']");
          if (xNode.Attributes["Sigillo"] != null)
          {
            xNode.Attributes.Remove(xNode.Attributes["Sigillo"]);
          }
          if (xNode.Attributes["Sigillo_Password"] != null)
          {
            xNode.Attributes.Remove(xNode.Attributes["Sigillo_Password"]);
          }
          if (xNode.Attributes["Sigillo_Data"] != null)
          {
            xNode.Attributes.Remove(xNode.Attributes["Sigillo_Data"]);
          }
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDISQC;
    }
    public int RemoveSigilloISQC(int IDISQC)
    {
#if (!DBG_TEST)
      return RemoveSigilloISQC_old(IDISQC);
#endif
      if (IDISQC == App.MasterFile_NewID) return IDISQC;
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/ISQCs/ISQC[@ID='" + IDISQC.ToString() + "']");
        if (xNode.Attributes["Sigillo"] != null)
        {
          xNode.Attributes.Remove(xNode.Attributes["Sigillo"]);
        }
        if (xNode.Attributes["Sigillo_Password"] != null)
        {
          xNode.Attributes.Remove(xNode.Attributes["Sigillo_Password"]);
        }
        if (xNode.Attributes["Sigillo_Data"] != null)
        {
          xNode.Attributes.Remove(xNode.Attributes["Sigillo_Data"]);
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.SetISQC", conn);
          cmd.Parameters.AddWithValue("@IDISQC", IDISQC.ToString());
          cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "RemoveSigilloISQC(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDISQC;
    }

#endregion //------------------------------------------------------------- ISQC

#region Revisione

    //----------------------------------------------------------------------------+
    //                             GetRevisioneCount                              |
    //----------------------------------------------------------------------------+
    public int GetRevisioneCount()
    {
      int result = 0;
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/REVISIONI/REVISIONE");
        if (xNodes != null)
        {
          result = xNodes.Count;
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                                GetRevisioni                                |
    //----------------------------------------------------------------------------+
    public ArrayList GetRevisioni(string IDCliente)
    {
      ArrayList results = new ArrayList();
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/REVISIONI/REVISIONE[@Cliente='" + IDCliente + "']");
        foreach (XmlNode node in xNodes)
        {
          Hashtable result = new Hashtable();
          foreach (XmlAttribute item in node.Attributes)
          {
            result.Add(item.Name, item.Value);
          }
          results.Add(result);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return results;
    }

    //----------------------------------------------------------------------------+
    //                                GetRevisione                                |
    //----------------------------------------------------------------------------+
    public Hashtable GetRevisione(string IDRevisione)
    {
      Hashtable result = new Hashtable();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@ID='" + IDRevisione + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                          GetRevisioneFromFileData                          |
    //----------------------------------------------------------------------------+
    public Hashtable GetRevisioneFromFileData(string FileSessione)
    {
      Hashtable result = new Hashtable();
      FileSessione = FileSessione.Split('\\').Last();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@FileData='" + FileSessione + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                                AddRevisione                                |
    //----------------------------------------------------------------------------+
    public string AddRevisione_old(XmlNode node)
    {
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI");
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      XmlNode xtmp = document.ImportNode(node, true);
      xNode.AppendChild(xtmp);
      xNode.Attributes["LastID"].Value = ID;
      Save();
      Close();
      return ID;
    }
    public string AddRevisione(XmlNode node)
    {
#if (!DBG_TEST)
      return AddRevisione_old(node);
#endif
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI");
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetRevisione", conn);
        cmd.Parameters.AddWithValue("@IDRevisione", ID);
        cmd.Parameters.AddWithValue("@rec", node.OuterXml);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "AddRevisione(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      Save();
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      Close();
      return ID;
    }

    //----------------------------------------------------------------------------+
    //                              DeleteRevisione                               |
    //----------------------------------------------------------------------------+
  
    public void DeleteRevisione(int IDRevisione, string idcliente)
        {

      try
      {
        cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.Revisione)).ToString(),  idcliente);
                cBusinessObjects.DeleteSessione("Revisione",IDRevisione, idcliente);
                Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@ID='" + IDRevisione.ToString() + "']");
        if (App.m_xmlCache.Contains(xNode.Attributes["File"].Value)) App.m_xmlCache.Remove(xNode.Attributes["File"].Value);
        if (App.m_xmlCache.Contains(xNode.Attributes["FileData"].Value)) App.m_xmlCache.Remove(xNode.Attributes["FileData"].Value);
        Close();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDRevisione + "'][@Tree='" + (Convert.ToInt32(App.TipoFile.Revisione)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.DeleteRevisione", conn);
          cmd.Parameters.AddWithValue("@IDRevisione", IDRevisione.ToString());
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
                            cBusinessObjects.hide_workinprogress();
                            string msg = "DeleteRevisione(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
      }
      catch (Exception ex)
      {
        string log = ex.Message;
                cBusinessObjects.hide_workinprogress();
                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
            cBusinessObjects.hide_workinprogress();
        }

    //----------------------------------------------------------------------------+
    //                           CheckDoppio_Revisione                            |
    //----------------------------------------------------------------------------+
    public bool CheckDoppio_Revisione(int ID, int IDCliente, string Data)
    {
      Open();
      XmlNodeList xNodes = document.SelectNodes("/ROOT/REVISIONI/REVISIONE");
      foreach (XmlNode node in xNodes)
      {
        if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data)
        {
          Close();
          return false;
        }
      }
      Close();
      return true;
    }

    //----------------------------------------------------------------------------+
    //                           SetRevisioneIntermedio                           |
    //----------------------------------------------------------------------------+
    public int SetRevisioneIntermedio_old(Hashtable values, int IDCliente, string dal, string al)
    {
      int IDRevisione = -1;

      try
      {
        Open();

        XmlNode root = document.SelectSingleNode("/ROOT/REVISIONI");

        IDRevisione = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

        string lastindex = IDRevisione.ToString();

        //Template TREE
        FileInfo fitree = new FileInfo(App.AppTemplateTreeRevisione);
        string estensione = "." + App.AppTemplateTreeRevisione.Split('.').Last();
        string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
        FileInfo fnewtree = new FileInfo(newNametree);

        while (fnewtree.Exists)
        {
          newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          fnewtree = new FileInfo(newNametree);
        }

        fitree.CopyTo(newNametree);
        newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

        //Template Dati
        FileInfo fidati = new FileInfo(App.AppTemplateDataRevisione);
        string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
        FileInfo fnewdati = new FileInfo(newNamedati);

        while (fnewdati.Exists)
        {
          newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          fnewdati = new FileInfo(newNamedati);
        }

        fidati.CopyTo(newNamedati);
        newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");

        XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

        string xml = "<REVISIONE ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);

        XmlNode tmpNode = doctmp.SelectSingleNode("/REVISIONE");
        XmlNode cliente = document.ImportNode(tmpNode, true);

        root.AppendChild(cliente);

        root.Attributes["LastID"].Value = lastindex;

        Save();

        Close();

      }
      catch (Exception ex)
      {
        string log = ex.Message;

        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }

      return IDRevisione;
    }
    public int SetRevisioneIntermedio(Hashtable values, int IDCliente, string dal, string al)
    {
#if (!DBG_TEST)
      return SetRevisioneIntermedio_old(values, IDCliente, dal, al);
#endif
      int IDRevisione = -1;
      string newNametree, newNamedati;
      try
      {
        Open();
        XmlNode root = document.SelectSingleNode("/ROOT/REVISIONI");
        IDRevisione = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
        string lastindex = IDRevisione.ToString();
        newNametree = App.AppTemplateTreeRevisione; newNametree = newNametree.Split('\\').Last();
        newNamedati = App.AppTemplateDataRevisione; newNamedati = newNamedati.Split('\\').Last();
        XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
        string xml = "<REVISIONE ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente +
          "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
          "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
          "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value +
          "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.NewRevisione", conn);
          cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SetRevisioneIntermedio(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRevisione;
    }

    //----------------------------------------------------------------------------+
    //                                SetRevisione                                |
    //----------------------------------------------------------------------------+
    public int SetRevisione_old(Hashtable values, int IDRevisione, int IDCliente)
    {
      try
      {
        Open();
        if (IDRevisione == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/REVISIONI");
          IDRevisione = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDRevisione.ToString();
          //Template TREE
          FileInfo fitree = new FileInfo(App.AppTemplateTreeRevisione);
          string estensione = "." + App.AppTemplateTreeRevisione.Split('.').Last();
          string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewtree = new FileInfo(newNametree);
          while (fnewtree.Exists)
          {
            newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewtree = new FileInfo(newNametree);
          }
          fitree.CopyTo(newNametree);
          newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");
          //Template Dati
          FileInfo fidati = new FileInfo(App.AppTemplateDataRevisione);
          string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewdati = new FileInfo(newNamedati);
          while (fnewdati.Exists)
          {
            newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewdati = new FileInfo(newNamedati);
          }
          fidati.CopyTo(newNamedati);
          newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
          XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
          string xml = "<REVISIONE ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/REVISIONE");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          root.AppendChild(cliente);
          root.Attributes["LastID"].Value = lastindex;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@ID='" + IDRevisione.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          xNode.Attributes["Data"].Value = values["Data"].ToString();
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRevisione;
    }
    public int SetRevisione(Hashtable values, int IDRevisione, int IDCliente)
    {
#if (!DBG_TEST)
      return SetRevisione_old(values, IDRevisione, IDCliente);
#endif
      string newNametree, newNamedati;
      try
      {
        Open();
        if (IDRevisione == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/REVISIONI");
          IDRevisione = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDRevisione.ToString();
          newNametree = App.AppTemplateTreeRevisione; newNametree = newNametree.Split('\\').Last();
          newNamedati = App.AppTemplateDataRevisione; newNamedati = newNamedati.Split('\\').Last();
          XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
          string xml = "<REVISIONE ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" +
            ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
            "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" +
            XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" +
            XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" +
            XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.NewRevisione", conn);
            cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetRevisione(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@ID='" + IDRevisione.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          xNode.Attributes["Data"].Value = values["Data"].ToString();
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.SetRevisione", conn);
            cmd.Parameters.AddWithValue("@IDRevisione", IDRevisione.ToString());
            cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetRevisione(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRevisione;
    }

    //----------------------------------------------------------------------------+
    //                  GetRevisioneAssociataFromConclusioneFile                  |
    //----------------------------------------------------------------------------+
    public string GetRevisioneAssociataFromConclusioneFile(string FileConclusione)
    {
      string FileRevisione = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI/CONCLUSIONE[@FileData='" + FileConclusione.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetRevisioni(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileRevisione = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileRevisione;
    }

    //----------------------------------------------------------------------------+
    //                         GetRevisioneFromEsercizio                          |
    //----------------------------------------------------------------------------+
    public string GetRevisioneFromEsercizio(string Cliente, string Esercizio)
    {
      string FileRevisione = "";
      Open();
      ArrayList al = GetRevisioni(Cliente);
      foreach (Hashtable item in al)
      {
        if (item["Data"].ToString() == "01/01/" + Esercizio || item["Data"].ToString() == "31/12/" + Esercizio)
        {
          FileRevisione = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
          break;
        }
      }
      Close();
      return FileRevisione;
    }

    //----------------------------------------------------------------------------+
    //                   GetRevisioneAssociataFromBilancioFile                    |
    //----------------------------------------------------------------------------+
    public string GetRevisioneAssociataFromBilancioFile(string FileBilancio)
    {
      string FileRevisione = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@FileData='" + FileBilancio.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetRevisioni(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileRevisione = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileRevisione;
    }

    //----------------------------------------------------------------------------+
    //                   GetBilancioAssociatoFromRevisioneFile                    |
    //----------------------------------------------------------------------------+
    public string GetBilancioAssociatoFromRevisioneFile(string FileRevisione)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@FileData='" + FileRevisione.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                 GetBilancioTreeAssociatoFromRevisioneFile                  |
    //----------------------------------------------------------------------------+
    public string GetBilancioTreeAssociatoFromRevisioneFile(string FileRevisione)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@FileData='" + FileRevisione.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = App.AppDataDataFolder + "\\" + item["File"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                  GetBilancioIDAssociatoFromRevisioneFile                   |
    //----------------------------------------------------------------------------+
    public string GetBilancioIDAssociatoFromRevisioneFile(string FileRevisione)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@FileData='" + FileRevisione.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = item["ID"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                  GetBilancioAssociatoFromConclusioneFile                   |
    //----------------------------------------------------------------------------+
    public string GetBilancioAssociatoFromConclusioneFile(string FileConclusione)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI/CONCLUSIONE[@FileData='" + FileConclusione.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                GetBilancioTreeAssociatoFromConclusioneFile                 |
    //----------------------------------------------------------------------------+
    public string GetBilancioTreeAssociatoFromConclusioneFile(string FileConclusione)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI/CONCLUSIONE[@FileData='" + FileConclusione.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = App.AppDataDataFolder + "\\" + item["File"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                 GetBilancioIDAssociatoFromConclusioneFile                  |
    //----------------------------------------------------------------------------+
    public string GetBilancioIDAssociatoFromConclusioneFile(string FileConclusione)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI/CONCLUSIONE[@FileData='" + FileConclusione.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = item["ID"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                  GetAllRevisioneAssociataFromBilancioFile                  |
    //----------------------------------------------------------------------------+
    public Hashtable GetAllRevisioneAssociataFromBilancioFile(string FileBilancio)
    {
      Hashtable Revisione = null;
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@FileData='" + FileBilancio.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetRevisioni(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              Revisione = item;
              break;
            }
          }
        }
      }
      Close();
      return Revisione;
    }

    //----------------------------------------------------------------------------+
    //                  GetAllBilancioAssociatoFromRevisioneFile                  |
    //----------------------------------------------------------------------------+
    public Hashtable GetAllBilancioAssociatoFromRevisioneFile(string FileRevisione)
    {
      Hashtable Bilancio = null;
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@FileData='" + FileRevisione.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              Bilancio = item;
              break;
            }
          }
        }
      }
      Close();
      return Bilancio;
    }

    //----------------------------------------------------------------------------+
    //                            SetSigilloRevisione                             |
    //----------------------------------------------------------------------------+
    public int SetSigilloRevisione_old(int IDRevisione, string revisore, string password)
    {
      try
      {
        Open();
        if (IDRevisione == App.MasterFile_NewID)
        {
          ;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@ID='" + IDRevisione.ToString() + "']");
          if (xNode.Attributes["Sigillo"] == null)
          {
            xNode.Attributes.Append(document.CreateAttribute("Sigillo"));
          }
          if (xNode.Attributes["Sigillo_Password"] == null)
          {
            xNode.Attributes.Append(document.CreateAttribute("Sigillo_Password"));
          }
          if (xNode.Attributes["Sigillo_Data"] == null)
          {
            xNode.Attributes.Append(document.CreateAttribute("Sigillo_Data"));
          }
          xNode.Attributes["Sigillo"].Value = revisore;
          xNode.Attributes["Sigillo_Password"].Value = password;
          xNode.Attributes["Sigillo_Data"].Value = DateTime.Now.ToShortDateString();
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRevisione;
    }
    public int SetSigilloRevisione(int IDRevisione, string revisore, string password)
    {
#if (!DBG_TEST)
      return SetSigilloRevisione_old(IDRevisione, revisore, password);
#endif
      if (IDRevisione == App.MasterFile_NewID) return IDRevisione;
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@ID='" + IDRevisione.ToString() + "']");
        if (xNode.Attributes["Sigillo"] == null)
        {
          xNode.Attributes.Append(document.CreateAttribute("Sigillo"));
        }
        if (xNode.Attributes["Sigillo_Password"] == null)
        {
          xNode.Attributes.Append(document.CreateAttribute("Sigillo_Password"));
        }
        if (xNode.Attributes["Sigillo_Data"] == null)
        {
          xNode.Attributes.Append(document.CreateAttribute("Sigillo_Data"));
        }
        xNode.Attributes["Sigillo"].Value = revisore;
        xNode.Attributes["Sigillo_Password"].Value = password;
        xNode.Attributes["Sigillo_Data"].Value = DateTime.Now.ToShortDateString();
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.SetRevisione", conn);
          cmd.Parameters.AddWithValue("@IDRevisione", IDRevisione.ToString());
          cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SetSigilloRevisione(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRevisione;
    }

    //----------------------------------------------------------------------------+
    //                           RemoveSigilloRevisione                           |
    //----------------------------------------------------------------------------+
    public int RemoveSigilloRevisione_old(int IDRevisione)
    {
      try
      {
        Open();
        if (IDRevisione == App.MasterFile_NewID)
        {
          ;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@ID='" + IDRevisione.ToString() + "']");
          if (xNode.Attributes["Sigillo"] != null)
          {
            xNode.Attributes.Remove(xNode.Attributes["Sigillo"]);
          }
          if (xNode.Attributes["Sigillo_Password"] != null)
          {
            xNode.Attributes.Remove(xNode.Attributes["Sigillo_Password"]);
          }
          if (xNode.Attributes["Sigillo_Data"] != null)
          {
            xNode.Attributes.Remove(xNode.Attributes["Sigillo_Data"]);
          }
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRevisione;
    }
    public int RemoveSigilloRevisione(int IDRevisione)
    {
#if (!DBG_TEST)
      return RemoveSigilloRevisione_old(IDRevisione);
#endif
      if (IDRevisione == App.MasterFile_NewID) return IDRevisione;
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@ID='" + IDRevisione.ToString() + "']");
        if (xNode.Attributes["Sigillo"] != null)
        {
          xNode.Attributes.Remove(xNode.Attributes["Sigillo"]);
        }
        if (xNode.Attributes["Sigillo_Password"] != null)
        {
          xNode.Attributes.Remove(xNode.Attributes["Sigillo_Password"]);
        }
        if (xNode.Attributes["Sigillo_Data"] != null)
        {
          xNode.Attributes.Remove(xNode.Attributes["Sigillo_Data"]);
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.SetRevisione", conn);
          cmd.Parameters.AddWithValue("@IDRevisione", IDRevisione.ToString());
          cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "RemoveSigilloRevisione(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRevisione;
    }

#endregion //-------------------------------------------------------- Revisione

#region Bilancio

    //----------------------------------------------------------------------------+
    //                              GetBilanciCount                               |
    //----------------------------------------------------------------------------+
    public int GetBilanciCount()
    {
      int result = 0;
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/BILANCI/BILANCIO");
        if (xNodes != null)
        {
          result = xNodes.Count;
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                                 GetBilanci                                 |
    //----------------------------------------------------------------------------+
    public ArrayList GetBilanci(string IDCliente)
    {
      ArrayList results = new ArrayList();
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/BILANCI/BILANCIO[@Cliente='" + IDCliente + "']");
        foreach (XmlNode node in xNodes)
        {
          Hashtable result = new Hashtable();
          foreach (XmlAttribute item in node.Attributes)
          {
            result.Add(item.Name, item.Value);
          }
          results.Add(result);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return results;
    }

    //----------------------------------------------------------------------------+
    //                                GetBilancio                                 |
    //----------------------------------------------------------------------------+
    public Hashtable GetBilancio(string IDBilancio)
    {
      Hashtable result = new Hashtable();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@ID='" + IDBilancio + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                                AddBilancio                                 |
    //----------------------------------------------------------------------------+
    public string AddBilancio_old(XmlNode node)
    {
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI");
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      XmlNode xtmp = document.ImportNode(node, true);
      xNode.AppendChild(xtmp);
      xNode.Attributes["LastID"].Value = ID;
      Save();
      Close();
      return ID;
    }
    public string AddBilancio(XmlNode node)
    {
#if (!DBG_TEST)
      return AddBilancio_old(node);
#endif
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI");
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetBilancio", conn);
        cmd.Parameters.AddWithValue("@IDBilancio", ID);
        cmd.Parameters.AddWithValue("@rec", node.OuterXml);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "AddBilancio(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      Save();
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      Close();
      return ID;
    }

    //----------------------------------------------------------------------------+
    //                               DeleteBilancio                               |
    //----------------------------------------------------------------------------+
   
    public void DeleteBilancio(int IDBilancio,string idcliente)
    {

      try
      {
        cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.Bilancio)).ToString(), idcliente);
        cBusinessObjects.DeleteSessione("Bilancio",IDBilancio, idcliente);
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@ID='" + IDBilancio.ToString() + "']");
        if (App.m_xmlCache.Contains(xNode.Attributes["File"].Value)) App.m_xmlCache.Remove(xNode.Attributes["File"].Value);
        if (App.m_xmlCache.Contains(xNode.Attributes["FileData"].Value)) App.m_xmlCache.Remove(xNode.Attributes["FileData"].Value);
        Close();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes(
          "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDBilancio + "'][@Tree='" +
          (Convert.ToInt32(App.TipoFile.Bilancio)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.DeleteBilancio", conn);
          cmd.Parameters.AddWithValue("@IDBilancio", IDBilancio.ToString());
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "DeleteBilancio(): errore\n" + ex.Message;
                            cBusinessObjects.hide_workinprogress();
                            MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
      }
      catch (Exception ex)
      {
        string log = ex.Message;
                cBusinessObjects.hide_workinprogress();
                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
            cBusinessObjects.hide_workinprogress();
   }

    //----------------------------------------------------------------------------+
    //                            CheckDoppio_Bilancio                            |
    //----------------------------------------------------------------------------+
    public bool CheckDoppio_Bilancio(int ID, int IDCliente, string Data)
    {
      Open();
      XmlNodeList xNodes = document.SelectNodes("/ROOT/BILANCI/BILANCIO");
      foreach (XmlNode node in xNodes)
      {
        if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data)
        {
          Close();
          return false;
        }
      }
      Close();
      return true;
    }

    public int SetBilancioIntermedio_old(Hashtable values, int IDCliente, string dal, string al)
    {
      int IDBilancio = -1;
      try
      {
        Open();

        XmlNode root = document.SelectSingleNode("/ROOT/BILANCI");

        IDBilancio = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

        string lastindex = IDBilancio.ToString();

        //Template TREE
        FileInfo fitree = new FileInfo(App.AppTemplateTreeBilancio);
        string estensione = "." + App.AppTemplateTreeBilancio.Split('.').Last();
        string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
        FileInfo fnewtree = new FileInfo(newNametree);

        while (fnewtree.Exists)
        {
          newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          fnewtree = new FileInfo(newNametree);
        }

        fitree.CopyTo(newNametree);
        newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

        //Template Dati
        FileInfo fidati = new FileInfo(App.AppTemplateDataBilancio);
        string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
        FileInfo fnewdati = new FileInfo(newNamedati);

        while (fnewdati.Exists)
        {
          newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          fnewdati = new FileInfo(newNamedati);
        }

        fidati.CopyTo(newNamedati);
        newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");

        XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

        string xml = "<BILANCIO ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\" Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\"  />";

        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);

        XmlNode tmpNode = doctmp.SelectSingleNode("/BILANCIO");
        XmlNode cliente = document.ImportNode(tmpNode, true);

        root.AppendChild(cliente);

        root.Attributes["LastID"].Value = lastindex;


        Save();

        Close();

      }
      catch (Exception ex)
      {
        string log = ex.Message;

        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }

      return IDBilancio;
    }
    //----------------------------------------------------------------------------+
    //                           SetBilancioIntermedio                            |
    //----------------------------------------------------------------------------+
    public int SetBilancioIntermedio(Hashtable values, int IDCliente, string dal, string al)
    {
#if (!DBG_TEST)
      return SetBilancioIntermedio_old(values, IDCliente, dal, al);
#endif
      int IDBilancio = -1;
      string newNametree, newNamedati;
      try
      {
        Open();
        XmlNode root = document.SelectSingleNode("/ROOT/BILANCI");
        IDBilancio = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
        string lastindex = IDBilancio.ToString();
        newNametree = App.AppTemplateTreeBilancio; newNametree = newNametree.Split('\\').Last();
        newNamedati = App.AppTemplateDataBilancio; newNamedati = newNamedati.Split('\\').Last();
        XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
        string xml = "<BILANCIO ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente +
          "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
          "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
          "\" Data=\"" + values["Data"].ToString() + "\" Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value +
          "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\"  />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.NewBilancio", conn);
          cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SetBilancioIntermedio(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDBilancio;
    }

    //----------------------------------------------------------------------------+
    //                                SetBilancio                                 |
    //----------------------------------------------------------------------------+
    public int SetBilancio_old(Hashtable values, int IDBilancio, int IDCliente)
    {
      try
      {
        Open();
        if (IDBilancio == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/BILANCI");
          IDBilancio = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDBilancio.ToString();
          //Template TREE
          FileInfo fitree = new FileInfo(App.AppTemplateTreeBilancio);
          string estensione = "." + App.AppTemplateTreeBilancio.Split('.').Last();
          string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewtree = new FileInfo(newNametree);
          while (fnewtree.Exists)
          {
            newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewtree = new FileInfo(newNametree);
          }
          fitree.CopyTo(newNametree);
          newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");
          //Template Dati
          FileInfo fidati = new FileInfo(App.AppTemplateDataBilancio);
          string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewdati = new FileInfo(newNamedati);
          while (fnewdati.Exists)
          {
            newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewdati = new FileInfo(newNamedati);
          }
          fidati.CopyTo(newNamedati);
          newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
          XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
          string xml = "<BILANCIO ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\" Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\"  />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/BILANCIO");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          root.AppendChild(cliente);
          root.Attributes["LastID"].Value = lastindex;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@ID='" + IDBilancio.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          xNode.Attributes["Data"].Value = values["Data"].ToString();
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDBilancio;
    }
    public int SetBilancio(Hashtable values, int IDBilancio, int IDCliente)
    {
#if (!DBG_TEST)
      return SetBilancio_old(values, IDBilancio, IDCliente);
#endif
      string newNametree, newNamedati;
      try
      {
        Open();
        if (IDBilancio == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/BILANCI");
          IDBilancio = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDBilancio.ToString();
          newNametree = App.AppTemplateTreeBilancio; newNametree = newNametree.Split('\\').Last();
          newNamedati = App.AppTemplateDataBilancio; newNamedati = newNamedati.Split('\\').Last();
          XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
          string xml = "<BILANCIO ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" +
            ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
            "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Data=\"" + values["Data"].ToString() + "\" Esercizio=\"" +
            XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" +
            XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" +
            XmlNodeCliente.Attributes["EsercizioAl"].Value + "\"  />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.NewBilancio", conn);
            cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetBilancio(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@ID='" + IDBilancio.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          xNode.Attributes["Data"].Value = values["Data"].ToString();
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.SetBilancio", conn);
            cmd.Parameters.AddWithValue("@IDBilancio", IDBilancio.ToString());
            cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetBilancio(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDBilancio;
    }

    //----------------------------------------------------------------------------+
    //                             SetSigilloBilancio                             |
    //----------------------------------------------------------------------------+
    public int SetSigilloBilancio_old(int IDBilancio, string revisore, string password)
    {
      try
      {
        Open();
        if (IDBilancio == App.MasterFile_NewID)
        {
          ;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@ID='" + IDBilancio.ToString() + "']");
          if (xNode.Attributes["Sigillo"] == null)
          {
            xNode.Attributes.Append(document.CreateAttribute("Sigillo"));
          }
          if (xNode.Attributes["Sigillo_Password"] == null)
          {
            xNode.Attributes.Append(document.CreateAttribute("Sigillo_Password"));
          }
          if (xNode.Attributes["Sigillo_Data"] == null)
          {
            xNode.Attributes.Append(document.CreateAttribute("Sigillo_Data"));
          }
          xNode.Attributes["Sigillo"].Value = revisore;
          xNode.Attributes["Sigillo_Password"].Value = password;
          xNode.Attributes["Sigillo_Data"].Value = DateTime.Now.ToShortDateString();
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDBilancio;
    }
    public int SetSigilloBilancio(int IDBilancio, string revisore, string password)
    {
#if (!DBG_TEST)
      return SetSigilloBilancio_old(IDBilancio, revisore, password);
#endif
      if (IDBilancio == App.MasterFile_NewID) return IDBilancio;
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@ID='" + IDBilancio.ToString() + "']");
        if (xNode.Attributes["Sigillo"] == null)
        {
          xNode.Attributes.Append(document.CreateAttribute("Sigillo"));
        }
        if (xNode.Attributes["Sigillo_Password"] == null)
        {
          xNode.Attributes.Append(document.CreateAttribute("Sigillo_Password"));
        }
        if (xNode.Attributes["Sigillo_Data"] == null)
        {
          xNode.Attributes.Append(document.CreateAttribute("Sigillo_Data"));
        }
        xNode.Attributes["Sigillo"].Value = revisore;
        xNode.Attributes["Sigillo_Password"].Value = password;
        xNode.Attributes["Sigillo_Data"].Value = DateTime.Now.ToShortDateString();
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.SetBilancio", conn);
          cmd.Parameters.AddWithValue("@IDBilancio", IDBilancio.ToString());
          cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SetSigilloBilancio(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDBilancio;
    }

    //----------------------------------------------------------------------------+
    //                           RemoveSigilloBilancio                            |
    //----------------------------------------------------------------------------+
    public int RemoveSigilloBilancio_old(int IDBilancio)
    {
      try
      {
        Open();
        if (IDBilancio == App.MasterFile_NewID)
        {
          ;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@ID='" + IDBilancio.ToString() + "']");
          if (xNode.Attributes["Sigillo"] != null)
          {
            xNode.Attributes.Remove(xNode.Attributes["Sigillo"]);
          }
          if (xNode.Attributes["Sigillo_Password"] != null)
          {
            xNode.Attributes.Remove(xNode.Attributes["Sigillo_Password"]);
          }
          if (xNode.Attributes["Sigillo_Data"] != null)
          {
            xNode.Attributes.Remove(xNode.Attributes["Sigillo_Data"]);
          }
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDBilancio;
    }
    public int RemoveSigilloBilancio(int IDBilancio)
    {
#if (!DBG_TEST)
      if (IDBilancio == App.MasterFile_NewID) return IDBilancio;
#endif
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@ID='" + IDBilancio.ToString() + "']");
        if (xNode.Attributes["Sigillo"] != null)
        {
          xNode.Attributes.Remove(xNode.Attributes["Sigillo"]);
        }
        if (xNode.Attributes["Sigillo_Password"] != null)
        {
          xNode.Attributes.Remove(xNode.Attributes["Sigillo_Password"]);
        }
        if (xNode.Attributes["Sigillo_Data"] != null)
        {
          xNode.Attributes.Remove(xNode.Attributes["Sigillo_Data"]);
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.SetBilancio", conn);
          cmd.Parameters.AddWithValue("@IDBilancio", IDBilancio.ToString());
          cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "RemoveSigilloBilancio(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDBilancio;
    }

#endregion //--------------------------------------------------------- Bilancio

#region Conclusioni

    //----------------------------------------------------------------------------+
    //                            GetConclusioniCount                             |
    //----------------------------------------------------------------------------+
    public int GetConclusioniCount()
    {
      int result = 0;
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/CONCLUSIONI/CONCLUSIONE");
        if (xNodes != null)
        {
          result = xNodes.Count;
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                               GetConclusioni                               |
    //----------------------------------------------------------------------------+
    public ArrayList GetConclusioni(string IDCliente)
    {
      ArrayList results = new ArrayList();
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/CONCLUSIONI/CONCLUSIONE[@Cliente='" + IDCliente + "']");
        foreach (XmlNode node in xNodes)
        {
          Hashtable result = new Hashtable();
          foreach (XmlAttribute item in node.Attributes)
          {
            result.Add(item.Name, item.Value);
          }
          results.Add(result);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return results;
    }

    //----------------------------------------------------------------------------+
    //                               GetConclusione                               |
    //----------------------------------------------------------------------------+
    public Hashtable GetConclusione(string IDConclusione)
    {
      Hashtable result = new Hashtable();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI/CONCLUSIONE[@ID='" + IDConclusione + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                               AddConclusione                               |
    //----------------------------------------------------------------------------+
    public string AddConclusione_old(XmlNode node)
    {
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI");
      if (xNode == null)
      {
        xNode = document.SelectSingleNode("/ROOT");
        string xml2 = "<CONCLUSIONI LastID=\"1\"/>";
        XmlDocument doctmp2 = new XmlDocument();
        doctmp2.LoadXml(xml2);
        XmlNode tmpNode2 = doctmp2.SelectSingleNode("/CONCLUSIONI");
        XmlNode cliente2 = document.ImportNode(tmpNode2, true);
        xNode.AppendChild(cliente2);
        xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI");
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      XmlNode xtmp = document.ImportNode(node, true);
      xNode.AppendChild(xtmp);
      xNode.Attributes["LastID"].Value = ID;
      Save();
      Close();
      return ID;
    }
    public string AddConclusione(XmlNode node)
    {
#if (!DBG_TEST)
      return AddConclusione_old(node);
#endif
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI");
      if (xNode == null)
      {
        xNode = document.SelectSingleNode("/ROOT");
        string xml2 = "<CONCLUSIONI LastID=\"1\"/>";
        XmlDocument doctmp2 = new XmlDocument();
        doctmp2.LoadXml(xml2);
        XmlNode tmpNode2 = doctmp2.SelectSingleNode("/CONCLUSIONI");
        XmlNode cliente2 = document.ImportNode(tmpNode2, true);
        xNode.AppendChild(cliente2);
        xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI");
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetConclusione", conn);
        cmd.Parameters.AddWithValue("@IDConclusione", ID);
        cmd.Parameters.AddWithValue("@rec", node.OuterXml);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "AddConclusione(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      Save();
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      Close();
      return ID;
    }

    //----------------------------------------------------------------------------+
    //                             DeleteConclusione                              |
    //----------------------------------------------------------------------------+
    public void DeleteConclusione_old(int IDConclusione)
    {
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI/CONCLUSIONE[@ID='" + IDConclusione.ToString() + "']");
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        xNode.ParentNode.RemoveChild(xNode);
        Save();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDConclusione + "'][@Tree='" + (Convert.ToInt32(App.TipoFile.Conclusione)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
          node.ParentNode.RemoveChild(node);
        }
        xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
    }
    public void DeleteConclusione(int IDConclusione,string idcliente)
    {

      try
      {
                cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.Conclusione)).ToString(), idcliente);
                cBusinessObjects.DeleteSessione("Conclusione",IDConclusione, idcliente);
                Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI/CONCLUSIONE[@ID='" + IDConclusione.ToString() + "']");
        if (App.m_xmlCache.Contains(xNode.Attributes["File"].Value)) App.m_xmlCache.Remove(xNode.Attributes["File"].Value);
        if (App.m_xmlCache.Contains(xNode.Attributes["FileData"].Value)) App.m_xmlCache.Remove(xNode.Attributes["FileData"].Value);
        Close();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes(
          "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDConclusione + "'][@Tree='" +
          (Convert.ToInt32(App.TipoFile.Conclusione)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.DeleteConclusione", conn);
          cmd.Parameters.AddWithValue("@IDConclusione", IDConclusione.ToString());
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
                            cBusinessObjects.hide_workinprogress();
                            string msg = "DeleteConclusione(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
      }
      catch (Exception ex)
      {
        string log = ex.Message;
                cBusinessObjects.hide_workinprogress();
                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
            cBusinessObjects.hide_workinprogress();
        }

    //----------------------------------------------------------------------------+
    //                          CheckDoppio_Conclusione                           |
    //----------------------------------------------------------------------------+
    public bool CheckDoppio_Conclusione(int ID, int IDCliente, string Data)
    {
      Open();
      XmlNodeList xNodes = document.SelectNodes("/ROOT/CONCLUSIONI/CONCLUSIONE");
      foreach (XmlNode node in xNodes)
      {
        if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data)
        {
          Close();
          return false;
        }
      }
      Close();
      return true;
    }

    //----------------------------------------------------------------------------+
    //                          SetConclusioneIntermedio                          |
    //----------------------------------------------------------------------------+
    public int SetConclusioneIntermedio_old(Hashtable values, int IDCliente, string dal, string al)
    {
      int IDConclusione = -1;
      try
      {
        Open();
        XmlNode root = document.SelectSingleNode("/ROOT/CONCLUSIONI");
        if (root == null)
        {
          root = document.SelectSingleNode("/ROOT");
          string xml2 = "<CONCLUSIONI LastID=\"1\"/>";
          XmlDocument doctmp2 = new XmlDocument();
          doctmp2.LoadXml(xml2);
          XmlNode tmpNode2 = doctmp2.SelectSingleNode("/CONCLUSIONI");
          XmlNode cliente2 = document.ImportNode(tmpNode2, true);
          root.AppendChild(cliente2);
          root = document.SelectSingleNode("/ROOT/CONCLUSIONI");
        }
        IDConclusione = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
        string lastindex = IDConclusione.ToString();
        //Template TREE
        FileInfo fitree = new FileInfo(App.AppTemplateTreeConclusione);
        string estensione = "." + App.AppTemplateTreeConclusione.Split('.').Last();
        string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
        FileInfo fnewtree = new FileInfo(newNametree);
        while (fnewtree.Exists)
        {
          newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          fnewtree = new FileInfo(newNametree);
        }
        fitree.CopyTo(newNametree);
        newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");
        //Template Dati
        FileInfo fidati = new FileInfo(App.AppTemplateDataConclusione);
        string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
        FileInfo fnewdati = new FileInfo(newNamedati);
        while (fnewdati.Exists)
        {
          newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          fnewdati = new FileInfo(newNamedati);
        }
        fidati.CopyTo(newNamedati);
        newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
        XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
        string xml = "<CONCLUSIONE ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/CONCLUSIONE");
        XmlNode cliente = document.ImportNode(tmpNode, true);
        root.AppendChild(cliente);
        root.Attributes["LastID"].Value = lastindex;
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDConclusione;
    }
    public int SetConclusioneIntermedio(Hashtable values, int IDCliente, string dal, string al)
    {
#if (!DBG_TEST)
      return SetConclusioneIntermedio_old(values, IDCliente, dal, al);
#endif
      int IDConclusione = -1;
      string newNametree, newNamedati;
      try
      {
        Open();
        XmlNode root = document.SelectSingleNode("/ROOT/CONCLUSIONI");
        if (root == null)
        {
          root = document.SelectSingleNode("/ROOT");
          string xml2 = "<CONCLUSIONI LastID=\"1\"/>";
          XmlDocument doctmp2 = new XmlDocument();
          doctmp2.LoadXml(xml2);
          XmlNode tmpNode2 = doctmp2.SelectSingleNode("/CONCLUSIONI");
          XmlNode cliente2 = document.ImportNode(tmpNode2, true);
          root.AppendChild(cliente2);
          root = document.SelectSingleNode("/ROOT/CONCLUSIONI");
        }
        IDConclusione = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
        string lastindex = IDConclusione.ToString();
        newNametree = App.AppTemplateTreeConclusione; newNametree = newNametree.Split('\\').Last();
        newNamedati = App.AppTemplateDataConclusione; newNamedati = newNamedati.Split('\\').Last();
        XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
        string xml = "<CONCLUSIONE ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente +
          "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
          "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
          "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value +
          "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.NewConclusione", conn);
          cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SetConclusioneIntermedio(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDConclusione;
    }

    //----------------------------------------------------------------------------+
    //                               SetConclusione                               |
    //----------------------------------------------------------------------------+
    public int SetConclusione_old(Hashtable values, int IDConclusione, int IDCliente)
    {
      try
      {
        Open();
        if (IDConclusione == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/CONCLUSIONI");
          if (root == null)
          {
            root = document.SelectSingleNode("/ROOT");
            string xml2 = "<CONCLUSIONI LastID=\"1\"/>";
            XmlDocument doctmp2 = new XmlDocument();
            doctmp2.LoadXml(xml2);
            XmlNode tmpNode2 = doctmp2.SelectSingleNode("/CONCLUSIONI");
            XmlNode cliente2 = document.ImportNode(tmpNode2, true);
            root.AppendChild(cliente2);
            root = document.SelectSingleNode("/ROOT/CONCLUSIONI");
          }
          IDConclusione = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDConclusione.ToString();
          //Template TREE
          FileInfo fitree = new FileInfo(App.AppTemplateTreeConclusione);
          string estensione = "." + App.AppTemplateTreeConclusione.Split('.').Last();
          string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewtree = new FileInfo(newNametree);
          while (fnewtree.Exists)
          {
            newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewtree = new FileInfo(newNametree);
          }
          fitree.CopyTo(newNametree);
          newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");
          //Template Dati
          FileInfo fidati = new FileInfo(App.AppTemplateDataConclusione);
          string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewdati = new FileInfo(newNamedati);
          while (fnewdati.Exists)
          {
            newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewdati = new FileInfo(newNamedati);
          }
          fidati.CopyTo(newNamedati);
          newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
          XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
          string xml = "<CONCLUSIONE ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/CONCLUSIONE");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          root.AppendChild(cliente);
          root.Attributes["LastID"].Value = lastindex;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/CONSLUSIONI/CONCLUSIONE[@ID='" + IDConclusione.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          xNode.Attributes["Data"].Value = values["Data"].ToString();
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDConclusione;
    }
    public int SetConclusione(Hashtable values, int IDConclusione, int IDCliente)
    {
#if (!DBG_TEST)
      return SetConclusione_old(values, IDConclusione, IDCliente);
#endif
      string newNametree, newNamedati;
      try
      {
        Open();
        if (IDConclusione == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/CONCLUSIONI");
          if (root == null)
          {
            root = document.SelectSingleNode("/ROOT");
            string xml2 = "<CONCLUSIONI LastID=\"1\"/>";
            XmlDocument doctmp2 = new XmlDocument();
            doctmp2.LoadXml(xml2);
            XmlNode tmpNode2 = doctmp2.SelectSingleNode("/CONCLUSIONI");
            XmlNode cliente2 = document.ImportNode(tmpNode2, true);
            root.AppendChild(cliente2);
            root = document.SelectSingleNode("/ROOT/CONCLUSIONI");
          }
          IDConclusione = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDConclusione.ToString();
          newNametree = App.AppTemplateTreeConclusione; newNametree = newNametree.Split('\\').Last();
          newNamedati = App.AppTemplateDataConclusione; newNamedati = newNamedati.Split('\\').Last();
          XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
          string xml = "<CONCLUSIONE ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" +
            ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
            "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" +
            XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" +
            XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" +
            XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.NewConclusione", conn);
            cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetConclusione(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/CONSLUSIONI/CONCLUSIONE[@ID='" + IDConclusione.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          xNode.Attributes["Data"].Value = values["Data"].ToString();
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.SetConclusione", conn);
            cmd.Parameters.AddWithValue("@IDConclusione", IDConclusione.ToString());
            cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetConclusione(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDConclusione;
    }

    //----------------------------------------------------------------------------+
    //                           SetSigilloConclusione                            |
    //----------------------------------------------------------------------------+
    public int SetSigilloConclusione_old(int IDConclusione, string revisore, string password)
    {
      try
      {
        Open();
        if (IDConclusione == App.MasterFile_NewID)
        {
          ;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI/CONCLUSIONE[@ID='" + IDConclusione.ToString() + "']");
          if (xNode.Attributes["Sigillo"] == null)
          {
            xNode.Attributes.Append(document.CreateAttribute("Sigillo"));
          }
          if (xNode.Attributes["Sigillo_Password"] == null)
          {
            xNode.Attributes.Append(document.CreateAttribute("Sigillo_Password"));
          }
          if (xNode.Attributes["Sigillo_Data"] == null)
          {
            xNode.Attributes.Append(document.CreateAttribute("Sigillo_Data"));
          }
          xNode.Attributes["Sigillo"].Value = revisore;
          xNode.Attributes["Sigillo_Password"].Value = password;
          xNode.Attributes["Sigillo_Data"].Value = DateTime.Now.ToShortDateString();
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDConclusione;
    }
    public int SetSigilloConclusione(int IDConclusione, string revisore, string password)
    {
#if (!DBG_TEST)
      return SetSigilloConclusione_old(IDConclusione, revisore, password);
#endif
      if (IDConclusione == App.MasterFile_NewID) return IDConclusione;
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI/CONCLUSIONE[@ID='" + IDConclusione.ToString() + "']");
        if (xNode.Attributes["Sigillo"] == null)
        {
          xNode.Attributes.Append(document.CreateAttribute("Sigillo"));
        }
        if (xNode.Attributes["Sigillo_Password"] == null)
        {
          xNode.Attributes.Append(document.CreateAttribute("Sigillo_Password"));
        }
        if (xNode.Attributes["Sigillo_Data"] == null)
        {
          xNode.Attributes.Append(document.CreateAttribute("Sigillo_Data"));
        }
        xNode.Attributes["Sigillo"].Value = revisore;
        xNode.Attributes["Sigillo_Password"].Value = password;
        xNode.Attributes["Sigillo_Data"].Value = DateTime.Now.ToShortDateString();
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.SetConclusione", conn);
          cmd.Parameters.AddWithValue("@IDConclusione", IDConclusione.ToString());
          cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SetSigilloConclusione(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDConclusione;
    }

    //----------------------------------------------------------------------------+
    //                          RemoveSigilloConclusione                          |
    //----------------------------------------------------------------------------+
    public int RemoveSigilloConclusione_old(int IDConclusione)
    {
      try
      {
        Open();
        if (IDConclusione == App.MasterFile_NewID)
        {
          ;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI/CONCLUSIONE[@ID='" + IDConclusione.ToString() + "']");
          if (xNode.Attributes["Sigillo"] != null)
          {
            xNode.Attributes.Remove(xNode.Attributes["Sigillo"]);
          }
          if (xNode.Attributes["Sigillo_Password"] != null)
          {
            xNode.Attributes.Remove(xNode.Attributes["Sigillo_Password"]);
          }
          if (xNode.Attributes["Sigillo_Data"] != null)
          {
            xNode.Attributes.Remove(xNode.Attributes["Sigillo_Data"]);
          }
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDConclusione;
    }
    public int RemoveSigilloConclusione(int IDConclusione)
    {
#if (!DBG_TEST)
      return RemoveSigilloConclusione_old(IDConclusione);
#endif
      if (IDConclusione == App.MasterFile_NewID) return IDConclusione;
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI/CONCLUSIONE[@ID='" + IDConclusione.ToString() + "']");
        if (xNode.Attributes["Sigillo"] != null)
        {
          xNode.Attributes.Remove(xNode.Attributes["Sigillo"]);
        }
        if (xNode.Attributes["Sigillo_Password"] != null)
        {
          xNode.Attributes.Remove(xNode.Attributes["Sigillo_Password"]);
        }
        if (xNode.Attributes["Sigillo_Data"] != null)
        {
          xNode.Attributes.Remove(xNode.Attributes["Sigillo_Data"]);
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.SetConclusione", conn);
          cmd.Parameters.AddWithValue("@IDConclusione", IDConclusione.ToString());
          cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "RemoveSigilloConclusione(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDConclusione;
    }

#endregion //------------------------------------------------------ Conclusioni

#region Pianificazione PianificazioniVerifica

    //----------------------------------------------------------------------------+
    //               GetPianificazionePianificazioniVerificheCount                |
    //----------------------------------------------------------------------------+
    public int GetPianificazionePianificazioniVerificheCount()
    {
      int result = 0;
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA");
        if (xNodes != null) result = xNodes.Count;
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                         GetPianificazioniVerifiche                         |
    //----------------------------------------------------------------------------+
    public ArrayList GetPianificazioniVerifiche(string IDCliente)
    {
      ArrayList results = new ArrayList();
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@Cliente='" + IDCliente + "']");
        foreach (XmlNode node in xNodes)
        {
          Hashtable result = new Hashtable();
          foreach (XmlAttribute item in node.Attributes)
          {
            result.Add(item.Name, item.Value);
          }
          results.Add(result);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return results;
    }

    //----------------------------------------------------------------------------+
    //                         GetPianificazioniVerifica                          |
    //----------------------------------------------------------------------------+
    public Hashtable GetPianificazioniVerifica(string IDPianificazioniVerifica)
    {
      Hashtable result = new Hashtable();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@ID='" + IDPianificazioniVerifica + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                         AddPianificazioniVerifica                          |
    //----------------------------------------------------------------------------+
    public string AddPianificazioniVerifica_old(XmlNode node)
    {
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVERIFICHE");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<PIANIFICAZIONIVERIFICHE LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/PIANIFICAZIONIVERIFICHE");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
        xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVERIFICHE");
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      XmlNode xtmp = document.ImportNode(node, true);
      xNode.AppendChild(xtmp);
      xNode.Attributes["LastID"].Value = ID;
      Save();
      Close();
      return ID;
    }
    public string AddPianificazioniVerifica(XmlNode node)
    {
#if (!DBG_TEST)
      return AddPianificazioniVerifica_old(node);
#endif
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVERIFICHE");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<PIANIFICAZIONIVERIFICHE LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/PIANIFICAZIONIVERIFICHE");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
        xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVERIFICHE");
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetPianificazioniVerifica", conn);
        cmd.Parameters.AddWithValue("@IDPianificazioniVerifica", ID);
        cmd.Parameters.AddWithValue("@rec", node.OuterXml);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "AddPianificazioniVerifica(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      Save();
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      Close();
      return ID;
    }

    //----------------------------------------------------------------------------+
    //                        DeletePianificazioniVerifica                        |
    //----------------------------------------------------------------------------+

    public void DeletePianificazioniVerifica(int IDPianificazioniVerifica, string idcliente)
        {

      try
      {
                cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.PianificazioniVerifica)).ToString(), idcliente);
                cBusinessObjects.DeleteSessione("PianificazioniVerifica",IDPianificazioniVerifica, idcliente);
                Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@ID='" + IDPianificazioniVerifica.ToString() + "']");
        if (App.m_xmlCache.Contains(xNode.Attributes["File"].Value)) App.m_xmlCache.Remove(xNode.Attributes["File"].Value);
        if (App.m_xmlCache.Contains(xNode.Attributes["FileData"].Value)) App.m_xmlCache.Remove(xNode.Attributes["FileData"].Value);
        Close();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDPianificazioniVerifica + "'][@Tree='" + (Convert.ToInt32(App.TipoFile.PianificazioniVerifica)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.DeletePianificazioniVerifica", conn);
          cmd.Parameters.AddWithValue("@IDPianificazioniVerifica", IDPianificazioniVerifica.ToString());
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
                            cBusinessObjects.hide_workinprogress();
                            string msg = "DeletePianificazioniVerifica(): errore\n" + ex.Message;

              MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
      }
      catch (Exception ex)
      {
        string log = ex.Message;
                cBusinessObjects.hide_workinprogress();
                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
            cBusinessObjects.hide_workinprogress();
        }

    //----------------------------------------------------------------------------+
    //                     CheckDoppio_PianificazioniVerifica                     |
    //----------------------------------------------------------------------------+
    public bool CheckDoppio_PianificazioniVerifica(int ID, int IDCliente, string DataInizio, string DataFine)
    {
      Open();
      XmlNodeList xNodes = document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA");
      DateTime dti_o = Convert.ToDateTime(DataInizio);
      DateTime dtf_o = Convert.ToDateTime(DataFine);
      foreach (XmlNode node in xNodes)
      {
        //controllo standard
        if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && (node.Attributes["DataInizio"].Value == DataInizio || node.Attributes["DataFine"].Value == DataFine))
        {
          Close();
          return false;
        }
        if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString())
        {
          //controllo di accavallamento date
          DateTime dti = Convert.ToDateTime(node.Attributes["DataInizio"].Value);
          DateTime dtf = Convert.ToDateTime(node.Attributes["DataFine"].Value);
          if ((dti_o.CompareTo(dti) > 0 && dti_o.CompareTo(dtf) < 0) || (dtf_o.CompareTo(dti) > 0 && dtf_o.CompareTo(dtf) < 0))
          {
            Close();
            return false;
          }
        }
      }
      Close();
      return true;
    }

    //----------------------------------------------------------------------------+
    //                       SetDataPianificazioniVerifica                        |
    //----------------------------------------------------------------------------+
    public void SetDataPianificazioniVerifica(string olddata_s, string newdata_s, int IDPianificazioniVerifica, int IDCliente)
    {
            cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.PianificazioniVerifica)).ToString(), IDCliente.ToString());

            MessageBox.Show("Dati sessione aggiornati");

            string olddata = olddata_s;
      string newdata = newdata_s;
      try
      {
        olddata = olddata.Substring(0, 5) + "&#xD;&#xA;" + olddata.Substring(6, 4);
        newdata = newdata.Substring(0, 5) + "&#xD;&#xA;" + newdata.Substring(6, 4);
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      ArrayList al = GetPianificazioniVerifiche(IDCliente.ToString());
      foreach (Hashtable item in al)
      {
        XmlManager x2 = new XmlManager();
        x2.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
        XmlDataProviderManager _test = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["File"].ToString());
        x2.SaveEncodedFile(App.AppDataDataFolder + "\\" + item["File"].ToString(), _test.Document.OuterXml.Replace(olddata, newdata));
      }
    }

    //----------------------------------------------------------------------------+
    //                         SetPianificazioniVerifica                          |
    //----------------------------------------------------------------------------+
    public int SetPianificazioniVerifica_old(Hashtable values, int IDPianificazioniVerifica, int IDCliente)
    {
      try
      {
        string olddata = "";
        string newdata = "";
        bool changedatatbd = false;

        Open();

        if (document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICHE") == null || document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICHE").Count == 0)
        {
          string xml = "<PIANIFICAZIONIVERIFICHE LastID=\"1\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);

          XmlNode tmpNode = doctmp.SelectSingleNode("/PIANIFICAZIONIVERIFICHE");
          XmlNode cliente = document.ImportNode(tmpNode, true);

          document.SelectSingleNode("/ROOT").AppendChild(cliente);
        }

        if (IDPianificazioniVerifica == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVERIFICHE");

          IDPianificazioniVerifica = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

          string lastindex = IDPianificazioniVerifica.ToString();

          //Template TREE
          FileInfo fitree = new FileInfo(App.AppTemplateTreePianificazioniVerifica);
          string estensione = "." + App.AppTemplateTreePianificazioniVerifica.Split('.').Last();
          string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewtree = new FileInfo(newNametree);

          while (fnewtree.Exists)
          {
            newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewtree = new FileInfo(newNametree);
          }

          fitree.CopyTo(newNametree);
          newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

          //Template Dati
          FileInfo fidati = new FileInfo(App.AppTemplateDataPianificazioniVerifica);
          string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewdati = new FileInfo(newNamedati);

          while (fnewdati.Exists)
          {
            newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewdati = new FileInfo(newNamedati);
          }

          fidati.CopyTo(newNamedati);
          newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");


          string xml = "<PIANIFICAZIONIVERIFICA ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" DataInizio=\"" + values["DataInizio"].ToString() + "\" DataFine=\"" + values["DataFine"].ToString() + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);

          XmlNode tmpNode = doctmp.SelectSingleNode("/PIANIFICAZIONIVERIFICA");
          XmlNode cliente = document.ImportNode(tmpNode, true);

          root.AppendChild(cliente);

          root.Attributes["LastID"].Value = lastindex;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@ID='" + IDPianificazioniVerifica.ToString() + "']");


          if (xNode.Attributes["DataInizio"].Value != values["DataInizio"].ToString())
          {
            olddata = xNode.Attributes["DataInizio"].Value;
            newdata = values["DataInizio"].ToString();
            changedatatbd = true;

            xNode.Attributes["DataInizio"].Value = values["DataInizio"].ToString();
          }

          if (xNode.Attributes["DataFine"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("DataFine");
            xNode.Attributes.Append(attr);
          }

          xNode.Attributes["DataFine"].Value = values["DataFine"].ToString();
        }

        Save();

        Close();

        if (changedatatbd)
        {
          SetDataPianificazioniVerifica(olddata, newdata, IDPianificazioniVerifica, IDCliente);
        }

      }
      catch (Exception ex)
      {
        string log = ex.Message;

        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }

      return IDPianificazioniVerifica;
    }
    public int SetPianificazioniVerifica(Hashtable values, int IDPianificazioniVerifica, int IDCliente,bool cancellatree=true)
    {
        if(cancellatree)
         cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.PianificazioniVerifica)).ToString(), IDCliente.ToString());
            string newNametree, newNamedati;
      try
      {
        //string str;
        string olddata = "";
        string newdata = "";
        bool changedatatbd = false;
        Open();
        if (document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICHE") == null || document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICHE").Count == 0)
        {
          string xml = "<PIANIFICAZIONIVERIFICHE LastID=\"1\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/PIANIFICAZIONIVERIFICHE");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          document.SelectSingleNode("/ROOT").AppendChild(cliente);
        }
        if (IDPianificazioniVerifica == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVERIFICHE");
          IDPianificazioniVerifica = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDPianificazioniVerifica.ToString();
          newNametree = App.AppTemplateTreePianificazioniVerifica; newNametree = newNametree.Split('\\').Last();
          newNamedati = App.AppTemplateDataPianificazioniVerifica; newNamedati = newNamedati.Split('\\').Last();
          string xml = "<PIANIFICAZIONIVERIFICA ID=\"" + lastindex + "\" Cliente=\"" + IDCliente +
            "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" +
            newNametree + "\" FileData=\"" + newNamedati + "\" DataInizio=\"" + values["DataInizio"].ToString() +
            "\" DataFine=\"" + values["DataFine"].ToString() + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.NewPianificazioniVerifica", conn);
            cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetPianificazioniVerifica(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@ID='" + IDPianificazioniVerifica.ToString() + "']");
          if (xNode.Attributes["DataInizio"].Value != values["DataInizio"].ToString())
          {
            olddata = xNode.Attributes["DataInizio"].Value;
            newdata = values["DataInizio"].ToString();
            changedatatbd = true;
            xNode.Attributes["DataInizio"].Value = values["DataInizio"].ToString();
          }
          if (xNode.Attributes["DataFine"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("DataFine");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["DataFine"].Value = values["DataFine"].ToString();
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.SetPianificazioniVerifica", conn);
            cmd.Parameters.AddWithValue("@IDPianificazioniVerifica", IDPianificazioniVerifica.ToString());
            cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetPianificazioniVerifica(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
            //str = xNode.Attributes["File"].Value;
            //if (str!=null)
            //  if (App.m_xmlCache.Contains(str)) App.m_xmlCache.Remove(str);
            //str = xNode.Attributes["FileData"].Value;
            //if (str != null)
            //  if (App.m_xmlCache.Contains(str)) App.m_xmlCache.Remove(str);
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
        if (changedatatbd)
        {
          SetDataPianificazioniVerifica(olddata, newdata, IDPianificazioniVerifica, IDCliente);
        }
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDPianificazioniVerifica;
    }

#endregion //---------------------------- Pianificazione PianificazioniVerifica

#region Pianificazione PianificazioniVigilanza

    //----------------------------------------------------------------------------+
    //               GetPianificazionePianificazioniVigilanzeCount                |
    //----------------------------------------------------------------------------+
    public int GetPianificazionePianificazioniVigilanzeCount()
    {
      int result = 0;
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA");
        if (xNodes != null)
        {
          result = xNodes.Count;
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                         GetPianificazioniVigilanze                         |
    //----------------------------------------------------------------------------+
    public ArrayList GetPianificazioniVigilanze(string IDCliente)
    {
      ArrayList results = new ArrayList();
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@Cliente='" + IDCliente + "']");
        foreach (XmlNode node in xNodes)
        {
          Hashtable result = new Hashtable();
          foreach (XmlAttribute item in node.Attributes)
          {
            result.Add(item.Name, item.Value);
          }
          results.Add(result);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return results;
    }

    //----------------------------------------------------------------------------+
    //                         GetPianificazioniVigilanza                         |
    //----------------------------------------------------------------------------+
    public Hashtable GetPianificazioniVigilanza(string IDPianificazioniVigilanza)
    {
      Hashtable result = new Hashtable();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@ID='" + IDPianificazioniVigilanza + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                         AddPianificazioniVigilanza                         |
    //----------------------------------------------------------------------------+
    public string AddPianificazioniVigilanza_old(XmlNode node)
    {
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVIGILANZE");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<PIANIFICAZIONIVIGILANZE LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/PIANIFICAZIONIVIGILANZE");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
        xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVIGILANZE");
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      XmlNode xtmp = document.ImportNode(node, true);
      xNode.AppendChild(xtmp);
      xNode.Attributes["LastID"].Value = ID;
      Save();
      Close();
      return ID;
    }
    public string AddPianificazioniVigilanza(XmlNode node)
    {
#if (!DBG_TEST)
      return AddPianificazioniVigilanza_old(node);
#endif
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVIGILANZE");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<PIANIFICAZIONIVIGILANZE LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/PIANIFICAZIONIVIGILANZE");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
        xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVIGILANZE");
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetPianificazioniVigilanza", conn);
        cmd.Parameters.AddWithValue("@IDPianificazioniVigilanza", ID);
        cmd.Parameters.AddWithValue("@rec", node.OuterXml);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "AddPianificazioniVigilanza(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      Save();
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      Close();
      return ID;
    }

    //----------------------------------------------------------------------------+
    //                       DeletePianificazioniVigilanza                        |
    //----------------------------------------------------------------------------+
    public void DeletePianificazioniVigilanza_old(int IDPianificazioniVigilanza)
    {
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@ID='" + IDPianificazioniVigilanza.ToString() + "']");
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        xNode.ParentNode.RemoveChild(xNode);
        Save();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDPianificazioniVigilanza + "'][@Tree='" + (Convert.ToInt32(App.TipoFile.PianificazioniVigilanza)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
          node.ParentNode.RemoveChild(node);
        }
        xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
    }
    public void DeletePianificazioniVigilanza(int IDPianificazioniVigilanza, string idcliente)
        {

      try
      {
                cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.PianificazioniVigilanza)).ToString(), idcliente);
                cBusinessObjects.DeleteSessione("PianificazioniVigilanza",IDPianificazioniVigilanza, idcliente);
                Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@ID='" + IDPianificazioniVigilanza.ToString() + "']");
        if (App.m_xmlCache.Contains(xNode.Attributes["File"].Value)) App.m_xmlCache.Remove(xNode.Attributes["File"].Value);
        if (App.m_xmlCache.Contains(xNode.Attributes["FileData"].Value)) App.m_xmlCache.Remove(xNode.Attributes["FileData"].Value);
        Close();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes(
          "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDPianificazioniVigilanza + "'][@Tree='" +
          (Convert.ToInt32(App.TipoFile.PianificazioniVigilanza)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.DeletePianificazioniVigilanza", conn);
          cmd.Parameters.AddWithValue("@IDPianificazioniVigilanza", IDPianificazioniVigilanza.ToString());
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
                            cBusinessObjects.hide_workinprogress();
                            string msg = "DeletePianificazioniVigilanza(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
      }
      catch (Exception ex)
      {
        string log = ex.Message;
                cBusinessObjects.hide_workinprogress();
                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
            cBusinessObjects.hide_workinprogress();
        }

    //----------------------------------------------------------------------------+
    //                    CheckDoppio_PianificazioniVigilanza                     |
    //----------------------------------------------------------------------------+
    public bool CheckDoppio_PianificazioniVigilanza(int ID, int IDCliente, string DataInizio, string DataFine)
    {
      Open();
      DateTime dti_o = Convert.ToDateTime(DataInizio);
      DateTime dtf_o = Convert.ToDateTime(DataFine);
      XmlNodeList xNodes = document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA");
      foreach (XmlNode node in xNodes)
      {
        if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && (node.Attributes["DataInizio"].Value == DataInizio || node.Attributes["DataFine"].Value == DataFine))
        {
          Close();
          return false;
        }
        if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString())
        {
          //controllo di accavallamento date
          DateTime dti = Convert.ToDateTime(node.Attributes["DataInizio"].Value);
          DateTime dtf = Convert.ToDateTime(node.Attributes["DataFine"].Value);
          if ((dti_o.CompareTo(dti) > 0 && dti_o.CompareTo(dtf) < 0) || (dtf_o.CompareTo(dti) > 0 && dtf_o.CompareTo(dtf) < 0))
          {
            Close();
            return false;
          }
        }
      }
      Close();
      return true;
    }

    //----------------------------------------------------------------------------+
    //                       SetDataPianificazioniVigilanza                       |
    //----------------------------------------------------------------------------+
    public void SetDataPianificazioniVigilanza(string olddata_s, string newdata_s, int IDPianificazioniVigilanza, int IDCliente)
    {
            cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.PianificazioniVigilanza)).ToString(), IDCliente.ToString());

            MessageBox.Show("Dati sessione aggiornati");
            string olddata = olddata_s;
      string newdata = newdata_s;
      try
      {
        olddata = olddata.Substring(0, 5) + "&#xD;&#xA;" + olddata.Substring(6, 4);
        newdata = newdata.Substring(0, 5) + "&#xD;&#xA;" + newdata.Substring(6, 4);
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      ArrayList al = GetPianificazioniVigilanze(IDCliente.ToString());
      foreach (Hashtable item in al)
      {
        XmlManager x2 = new XmlManager();
        x2.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
        XmlDataProviderManager _test = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["File"].ToString());
        x2.SaveEncodedFile(App.AppDataDataFolder + "\\" + item["File"].ToString(), _test.Document.OuterXml.Replace(olddata, newdata));
      }
    }

    //----------------------------------------------------------------------------+
    //                         SetPianificazioniVigilanza                         |
    //----------------------------------------------------------------------------+
    public int SetPianificazioniVigilanza_old(Hashtable values, int IDPianificazioniVigilanza, int IDCliente)
    {
      try
      {
        string olddata = "";
        string newdata = "";
        bool changedatatbd = false;
        Open();
        if (document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZE") == null || document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZE").Count == 0)
        {
          string xml = "<PIANIFICAZIONIVIGILANZE LastID=\"1\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/PIANIFICAZIONIVIGILANZE");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          document.SelectSingleNode("/ROOT").AppendChild(cliente);
        }
        if (IDPianificazioniVigilanza == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVIGILANZE");
          IDPianificazioniVigilanza = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDPianificazioniVigilanza.ToString();
          //Template TREE
          FileInfo fitree = new FileInfo(App.AppTemplateTreePianificazioniVigilanza);
          string estensione = "." + App.AppTemplateTreePianificazioniVigilanza.Split('.').Last();
          string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewtree = new FileInfo(newNametree);
          while (fnewtree.Exists)
          {
            newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewtree = new FileInfo(newNametree);
          }
          fitree.CopyTo(newNametree);
          newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");
          //Template Dati
          FileInfo fidati = new FileInfo(App.AppTemplateDataPianificazioniVigilanza);
          string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewdati = new FileInfo(newNamedati);
          while (fnewdati.Exists)
          {
            newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewdati = new FileInfo(newNamedati);
          }
          fidati.CopyTo(newNamedati);
          newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
          string xml = "<PIANIFICAZIONIVIGILANZA ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" DataInizio=\"" + values["DataInizio"].ToString() + "\" DataFine=\"" + values["DataFine"].ToString() + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/PIANIFICAZIONIVIGILANZA");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          root.AppendChild(cliente);
          root.Attributes["LastID"].Value = lastindex;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@ID='" + IDPianificazioniVigilanza.ToString() + "']");
          if (xNode.Attributes["DataInizio"].Value != values["DataInizio"].ToString())
          {
            olddata = xNode.Attributes["DataInizio"].Value;
            newdata = values["DataInizio"].ToString();
            changedatatbd = true;
            xNode.Attributes["DataInizio"].Value = values["DataInizio"].ToString();
          }
          if (xNode.Attributes["DataFine"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("DataFine");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["DataFine"].Value = values["DataFine"].ToString();
        }
        Save();
        Close();
        if (changedatatbd)
        {
          SetDataPianificazioniVigilanza(olddata, newdata, IDPianificazioniVigilanza, IDCliente);
        }
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDPianificazioniVigilanza;
    }
    public int SetPianificazioniVigilanza(Hashtable values, int IDPianificazioniVigilanza, int IDCliente,bool cancellatree=true)
    {
            if (cancellatree)
                cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.PianificazioniVigilanza)).ToString(), IDCliente.ToString());
            string newNametree, newNamedati;
      try
      {
        string olddata = "";
        string newdata = "";
        bool changedatatbd = false;
        Open();
        if (document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZE") == null || document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZE").Count == 0)
        {
          string xml = "<PIANIFICAZIONIVIGILANZE LastID=\"1\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/PIANIFICAZIONIVIGILANZE");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          document.SelectSingleNode("/ROOT").AppendChild(cliente);
        }
        if (IDPianificazioniVigilanza == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVIGILANZE");
          IDPianificazioniVigilanza = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDPianificazioniVigilanza.ToString();
          newNametree = App.AppTemplateTreePianificazioniVigilanza; newNametree = newNametree.Split('\\').Last();
          newNamedati = App.AppTemplateDataPianificazioniVigilanza; newNamedati = newNamedati.Split('\\').Last();
          string xml = "<PIANIFICAZIONIVIGILANZA ID=\"" + lastindex + "\" Cliente=\"" + IDCliente +
            "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" +
            newNametree + "\" FileData=\"" + newNamedati + "\" DataInizio=\"" + values["DataInizio"].ToString() +
            "\" DataFine=\"" + values["DataFine"].ToString() + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.NewPianificazioniVigilanza", conn);
            cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetPianificazioniVigilanza(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
            if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
          }
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@ID='" + IDPianificazioniVigilanza.ToString() + "']");
          if (xNode.Attributes["DataInizio"].Value != values["DataInizio"].ToString())
          {
            olddata = xNode.Attributes["DataInizio"].Value;
            newdata = values["DataInizio"].ToString();
            changedatatbd = true;
            xNode.Attributes["DataInizio"].Value = values["DataInizio"].ToString();
          }
          if (xNode.Attributes["DataFine"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("DataFine");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["DataFine"].Value = values["DataFine"].ToString();
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.SetPianificazioniVigilanza", conn);
            cmd.Parameters.AddWithValue("@IDPianificazioniVigilanza", IDPianificazioniVigilanza.ToString());
            cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetPianificazioniVigilanza(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        Save(true);
        StaticUtilities.PurgeXML("RevisoftApp.rmdf");
        Close();
        if (changedatatbd)
        {
          SetDataPianificazioniVigilanza(olddata, newdata, IDPianificazioniVigilanza, IDCliente);
        }
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDPianificazioniVigilanza;
    }

#endregion //--------------------------- Pianificazione PianificazioniVigilanza

#region Verifica

    //----------------------------------------------------------------------------+
    //                             GetVerificheCount                              |
    //----------------------------------------------------------------------------+
    public int GetVerificheCount()
    {
      int result = 0;
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/VERIFICHE/VERIFICA");
        if (xNodes != null)
        {
          result = xNodes.Count;
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                                GetVerifiche                                |
    //----------------------------------------------------------------------------+
    public ArrayList GetVerifiche(string IDCliente)
    {
      ArrayList results = new ArrayList();
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/VERIFICHE/VERIFICA[@Cliente='" + IDCliente + "']");
        foreach (XmlNode node in xNodes)
        {
          Hashtable result = new Hashtable();
          foreach (XmlAttribute item in node.Attributes)
          {
            result.Add(item.Name, item.Value);
          }
          results.Add(result);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return results;
    }

    //----------------------------------------------------------------------------+
    //                                GetVerifica                                 |
    //----------------------------------------------------------------------------+
    public Hashtable GetVerifica(string IDVerifica)
    {
      Hashtable result = new Hashtable();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/VERIFICHE/VERIFICA[@ID='" + IDVerifica + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        if (!result.ContainsKey("DataEsecuzione_Fine"))
        {
          result.Add("DataEsecuzione_Fine", result["DataEsecuzione"].ToString());
        }
        if (!result.ContainsKey("DataOggetto_Inizio"))
        {
          result.Add("DataOggetto_Inizio", result["DataEsecuzione"].ToString());
        }
        if (!result.ContainsKey("DataOggetto_Fine"))
        {
          result.Add("DataOggetto_Fine", result["DataEsecuzione"].ToString());
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                                AddVerifica                                 |
    //----------------------------------------------------------------------------+
    public string AddVerifica_old(XmlNode node)
    {
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/VERIFICHE");
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      XmlNode xtmp = document.ImportNode(node, true);
      xNode.AppendChild(xtmp);
      xNode.Attributes["LastID"].Value = ID;
      Save();
      Close();
      return ID;
    }
    public string AddVerifica(XmlNode node)
    {
#if (!DBG_TEST)
      return AddVerifica_old(node);
#endif
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/VERIFICHE");
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetVerifica", conn);
        cmd.Parameters.AddWithValue("@IDVerifica", ID);
        cmd.Parameters.AddWithValue("@rec", node.OuterXml);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "AddVerifica(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      Save();
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      Close();
      return ID;
    }

    //----------------------------------------------------------------------------+
    //                               DeleteVerifica                               |
    //----------------------------------------------------------------------------+
    public void DeleteVerifica_old(int IDVerifica)
    {
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/VERIFICHE/VERIFICA[@ID='" + IDVerifica.ToString() + "']");
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        xNode.ParentNode.RemoveChild(xNode);
        Save();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDVerifica + "'][@Tree='" + (Convert.ToInt32(App.TipoFile.Verifica)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
          node.ParentNode.RemoveChild(node);
        }
        xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
    }
    public void DeleteVerifica(int IDVerifica, string idcliente)
        {

      try
      {
                cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.Verifica)).ToString(), idcliente);
                cBusinessObjects.DeleteSessione("Verifica",IDVerifica,idcliente);
                Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/VERIFICHE/VERIFICA[@ID='" + IDVerifica.ToString() + "']");
        if (App.m_xmlCache.Contains(xNode.Attributes["File"].Value)) App.m_xmlCache.Remove(xNode.Attributes["File"].Value);
        if (App.m_xmlCache.Contains(xNode.Attributes["FileData"].Value)) App.m_xmlCache.Remove(xNode.Attributes["FileData"].Value);
        Close();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes(
          "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDVerifica + "'][@Tree='" +
          (Convert.ToInt32(App.TipoFile.Verifica)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.DeleteVerifica", conn);
          cmd.Parameters.AddWithValue("@IDVerifica", IDVerifica.ToString());
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "DeleteVerifica(): errore\n" + ex.Message;
                            cBusinessObjects.hide_workinprogress();
                            MessageBox.Show(msg);

            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
      }
      catch (Exception ex)
      {
        string log = ex.Message;
                cBusinessObjects.hide_workinprogress();
                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
            cBusinessObjects.hide_workinprogress();
        }

    //----------------------------------------------------------------------------+
    //                            CheckDoppio_Verifica                            |
    //----------------------------------------------------------------------------+
    public bool CheckDoppio_Verifica(int ID, int IDCliente, string Data)
    {
      Open();
      XmlNodeList xNodes = document.SelectNodes("/ROOT/VERIFICHE/VERIFICA");
      foreach (XmlNode node in xNodes)
      {
        if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data)
        {
          Close();
          return false;
        }
      }
      Close();
      return true;
    }

    //----------------------------------------------------------------------------+
    //                              SetDataVerifica                               |
    //----------------------------------------------------------------------------+
    public void SetDataVerifica(string olddata_s, string newdata_s, int IDVerifica, int IDCliente)
    {
           cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.Verifica)).ToString(), IDCliente.ToString());

    
      MessageBox.Show("Dati sessione aggiornati");
    
      string olddata = olddata_s;
      string newdata = newdata_s;
      try
      {
        olddata = olddata.Substring(0, 5) + "&#xD;&#xA;" + olddata.Substring(6, 4);
        newdata = newdata.Substring(0, 5) + "&#xD;&#xA;" + newdata.Substring(6, 4);
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      ArrayList al = GetVerifiche(IDCliente.ToString());
      foreach (Hashtable item in al)
      {
        XmlManager x2 = new XmlManager();
        x2.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
        XmlDataProviderManager _test = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["File"].ToString());
        x2.SaveEncodedFile(App.AppDataDataFolder + "\\" + item["File"].ToString(), _test.Document.OuterXml.Replace(olddata, newdata));
      }
    }

    //----------------------------------------------------------------------------+
    //                                SetVerifica                                 |
    //----------------------------------------------------------------------------+
   
    public int SetVerifica(Hashtable values, int IDVerifica, int IDCliente,bool cancellaretree=true)
    {
       if(cancellaretree)
         cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.Verifica)).ToString(), IDCliente.ToString());

       string newNametree, newNamedati;

      try
      {
        string olddata = "";
        string newdata = "";
        bool changedatatbd = false;
        Open();
        if (IDVerifica == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/VERIFICHE");
          IDVerifica = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDVerifica.ToString();
          newNametree = App.AppTemplateTreeVerifica; newNametree = newNametree.Split('\\').Last();
          newNamedati = App.AppTemplateDataVerifica; newNamedati = newNamedati.Split('\\').Last();
          string xml = "<VERIFICA ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" +
            ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
            "\" FileData=\"" + newNamedati + "\" Composizione=\"" + values["Composizione"].ToString() +
            "\" Inizio=\"" + values["Inizio"].ToString() + "\" Fine=\"" + values["Fine"].ToString() +
            "\" Luogo=\"" + values["Luogo"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Revisore=\"" + values["Revisore"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Presidente=\"" + values["Presidente"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Sindaco1=\"" + values["Sindaco1"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Sindaco2=\"" + values["Sindaco2"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Collaboratore=\"" + values["Collaboratore"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" AssisitoDa=\"" + values["AssisitoDa"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Data=\"" + values["Data"].ToString() + "\" DataEsecuzione=\"" + values["DataEsecuzione"].ToString() +
            "\" DataEsecuzione_Fine=\"" + values["DataEsecuzione_Fine"].ToString() + "\" DataOggetto_Inizio= \"" +
            ((values.Contains("DataOggetto_Inizio")) ? values["DataOggetto_Inizio"].ToString() : values["DataEsecuzione"].ToString()) +
            "\" DataOggetto_Fine= \"" +
            ((values.Contains("DataOggetto_Fine")) ? values["DataOggetto_Fine"].ToString() : values["DataEsecuzione"].ToString()) + "\"/>";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.NewVerifica", conn);
            cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetVerifica(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/VERIFICHE/VERIFICA[@ID='" + IDVerifica.ToString() + "']");
          xNode.Attributes["Inizio"].Value = values["Inizio"].ToString();
          xNode.Attributes["Fine"].Value = values["Fine"].ToString();
          xNode.Attributes["Luogo"].Value = values["Luogo"].ToString();
          xNode.Attributes["Revisore"].Value = values["Revisore"].ToString();
          xNode.Attributes["Presidente"].Value = values["Presidente"].ToString();
          xNode.Attributes["Sindaco1"].Value = values["Sindaco1"].ToString();
          xNode.Attributes["Sindaco2"].Value = values["Sindaco2"].ToString();
          if (xNode.Attributes["Collaboratore"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("Collaboratore");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Collaboratore"].Value = ((values["Collaboratore"] == null) ? "" : values["Collaboratore"].ToString());
          xNode.Attributes["AssisitoDa"].Value = values["AssisitoDa"].ToString();
          xNode.Attributes["Composizione"].Value = values["Composizione"].ToString();
          if (xNode.Attributes["Data"].Value != values["Data"].ToString())
          {
            olddata = xNode.Attributes["Data"].Value;
            newdata = values["Data"].ToString();
            changedatatbd = true;
            xNode.Attributes["Data"].Value = values["Data"].ToString();
          }
          if (xNode.Attributes["DataEsecuzione"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("DataEsecuzione");
            xNode.Attributes.Append(attr);
          }
          if (xNode.Attributes["DataEsecuzione_Fine"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("DataEsecuzione_Fine");
            xNode.Attributes.Append(attr);
          }
          if (xNode.Attributes["DataOggetto_Inizio"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("DataOggetto_Inizio");
            xNode.Attributes.Append(attr);
          }
          if (xNode.Attributes["DataOggetto_Fine"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("DataOggetto_Fine");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["DataEsecuzione"].Value = ((values["DataEsecuzione"] == null) ? values["Data"].ToString() : values["DataEsecuzione"].ToString());
          xNode.Attributes["DataEsecuzione_Fine"].Value = ((values["DataEsecuzione_Fine"] == null) ? values["Data"].ToString() : values["DataEsecuzione_Fine"].ToString());
          xNode.Attributes["DataOggetto_Inizio"].Value = ((values["DataOggetto_Inizio"] == null) ? values["Data"].ToString() : values["DataOggetto_Inizio"].ToString());
          xNode.Attributes["DataOggetto_Fine"].Value = ((values["DataOggetto_Fine"] == null) ? values["Data"].ToString() : values["DataOggetto_Fine"].ToString());
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.SetVerifica", conn);
            cmd.Parameters.AddWithValue("@IDVerifica", IDVerifica.ToString());
            cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetVerifica(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
        if (changedatatbd)
        {
          SetDataVerifica(olddata, newdata, IDVerifica, IDCliente);
        }
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDVerifica;
    }

    //----------------------------------------------------------------------------+
    //                     GetVigilanzaAssociataFromVerifica                      |
    //----------------------------------------------------------------------------+
    public Hashtable GetVigilanzaAssociataFromVerifica(string ID)
    {
      Hashtable result = new Hashtable();
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/VERIFICHE/VERIFICA[@ID='" + ID + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetVigilanze(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              result = item;
              break;
            }
          }
        }
      }
      Close();
      return result;
    }

#endregion //--------------------------------------------------------- Verifica

#region Vigilanza

    //----------------------------------------------------------------------------+
    //                          SplitVerificheVigilanze                           |
    //----------------------------------------------------------------------------+
    public void SplitVerificheVigilanze_old()
    {
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/VERIFICHE/VERIFICA");
        foreach (XmlNode node in xNodes)
        {
          if (node.Attributes["AlreadySplitted"] != null)
          {
            continue;
          }
          //Controllo se l'albero comprende vigilanze
          FileInfo fileVerifica = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
          FileInfo fileVerificaDati = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
          if (fileVerifica.Exists && fileVerificaDati.Exists)
          {
            XmlDataProviderManager xdocVerifica = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
            XmlDocument docVerifica = xdocVerifica.Document;
            XmlDataProviderManager xdocVerificaDati = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
            XmlDocument docVerificaDati = xdocVerificaDati.Document;
            if (docVerifica.SelectNodes("//Node[@ID>=500]").Count > 0)
            {
              //Creo nuovi file vigilanze
              string estensione = "." + App.AppTemplateTreeVigilanza.Split('.').Last();
              string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
              FileInfo fnewtree = new FileInfo(newNametree);
              while (fnewtree.Exists)
              {
                newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                fnewtree = new FileInfo(newNametree);
              }
              fileVerifica.CopyTo(newNametree);
              XmlDataProviderManager xdocVigilanza = new XmlDataProviderManager(newNametree);
              XmlDocument docVigilanza = xdocVigilanza.Document;
              XmlNode noderevisoft = docVigilanza.SelectSingleNode("//REVISOFT");
              noderevisoft.Attributes["ID"].Value = (Convert.ToInt32(App.TipoFile.Vigilanza)).ToString();
              XmlNode firstnodeToBeDeleted = docVigilanza.SelectSingleNode("//Node[@ID=1]");
              firstnodeToBeDeleted.ParentNode.ReplaceChild(firstnodeToBeDeleted.ChildNodes[1], firstnodeToBeDeleted);
              foreach (XmlNode item in docVigilanza.SelectNodes("//Node[@ID<500]"))
              {
                if (item.Attributes["ID"].Value != "1")
                {
                  item.ParentNode.RemoveChild(item);
                }
              }
              xdocVigilanza.Save();
              noderevisoft = docVerifica.SelectSingleNode("//REVISOFT");
              noderevisoft.Attributes["ID"].Value = (Convert.ToInt32(App.TipoFile.Verifica)).ToString();
              firstnodeToBeDeleted = docVerifica.SelectSingleNode("//Node[@ID=1]");
              firstnodeToBeDeleted.ParentNode.ReplaceChild(firstnodeToBeDeleted.ChildNodes[0], firstnodeToBeDeleted);
              foreach (XmlNode item in docVerifica.SelectNodes("//Node[@ID>=500]"))
              {
                item.ParentNode.RemoveChild(item);
              }
              xdocVerifica.Save();
              newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");
              string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
              FileInfo fnewdati = new FileInfo(newNamedati);
              while (fnewdati.Exists)
              {
                newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                fnewdati = new FileInfo(newNamedati);
              }
              fileVerificaDati.CopyTo(newNamedati);
              XmlDataProviderManager xdocVigilanzaData = new XmlDataProviderManager(newNamedati);
              XmlDocument docVigilanzaData = xdocVigilanzaData.Document;
              noderevisoft = docVigilanzaData.SelectSingleNode("//REVISOFT");
              noderevisoft.Attributes["ID"].Value = (Convert.ToInt32(App.TipoFile.Vigilanza)).ToString();
              foreach (XmlNode item in docVigilanzaData.SelectNodes("//Dato[@ID<500]"))
              {
                if (item.Attributes["ID"].Value != "1")
                {
                  item.ParentNode.RemoveChild(item);
                }
              }
              xdocVigilanzaData.Save();
              noderevisoft = docVerificaDati.SelectSingleNode("//REVISOFT");
              noderevisoft.Attributes["ID"].Value = (Convert.ToInt32(App.TipoFile.Verifica)).ToString();
              foreach (XmlNode item in docVerificaDati.SelectNodes("//Dato[@ID>=500]"))
              {
                item.ParentNode.RemoveChild(item);
              }
              xdocVerificaDati.Save();
              newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
              if (document.SelectNodes("/ROOT/VIGILANZE") == null || document.SelectNodes("/ROOT/VIGILANZE").Count == 0)
              {
                string xml = "<VIGILANZE LastID=\"1\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);
                XmlNode tmpNode = doctmp.SelectSingleNode("/VIGILANZE");
                XmlNode cliente = document.ImportNode(tmpNode, true);
                document.SelectSingleNode("/ROOT").AppendChild(cliente);
              }
              XmlNode root = document.SelectSingleNode("/ROOT/VIGILANZE");
              int IDVigilanza = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
              string lastindex = IDVigilanza.ToString();
              root.Attributes["LastID"].Value = lastindex;
              string xmlVigilanza = "<VIGILANZA ID=\"" + lastindex + "\" Cliente=\"" + node.Attributes["Cliente"].Value + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\"  Data=\"" + node.Attributes["Data"].Value + "\"  />";
              XmlDocument doctmp2 = new XmlDocument();
              doctmp2.LoadXml(xmlVigilanza);
              XmlNode tmpNode2 = doctmp2.SelectSingleNode("/VIGILANZA");
              XmlNode cliente2 = document.ImportNode(tmpNode2, true);
              root.AppendChild(cliente2);
              Save();
              //aggiorno allegati
              XmlManager xdoc = new XmlManager();
              xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
              XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
              XmlNodeList xNodesDocuments = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + node.Attributes["ID"].Value + "'][@Tree='" + (Convert.ToInt32(App.TipoFile.Verifica)).ToString() + "'][@Nodo>=500]");
              foreach (XmlNode nodeD in xNodesDocuments)
              {
                nodeD.Attributes["Tree"].Value = (Convert.ToInt32(App.TipoFile.Vigilanza)).ToString();
              }
              xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);
            }
            else
            {
              XmlAttribute attr = document.CreateAttribute("AlreadySplitted");
              attr.Value = "True";
              node.Attributes.Append(attr);
            }
          }
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return;
    }
    public void SplitVerificheVigilanze()
    {
#if (!DBG_TEST)
      SplitVerificheVigilanze_old();return;
#endif
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("dbo.SplitVerificheVigilanze", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "SplitVerificheVigilanze(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
    }

    //----------------------------------------------------------------------------+
    //                             GetVigilanzeCount                              |
    //----------------------------------------------------------------------------+
    public int GetVigilanzeCount()
    {
      int result = 0;
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/VIGILANZE/VIGILANZA");
        if (xNodes != null)
        {
          result = xNodes.Count;
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                                GetVigilanze                                |
    //----------------------------------------------------------------------------+
    public ArrayList GetVigilanze(string IDCliente)
    {
      ArrayList results = new ArrayList();
      try
      {
        Open();
        if (document.SelectNodes("/ROOT/VIGILANZE") == null || document.SelectNodes("/ROOT/VIGILANZE").Count == 0)
        {
          string xml = "<VIGILANZE LastID=\"1\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/VIGILANZE");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          document.SelectSingleNode("/ROOT").AppendChild(cliente);
        }
        XmlNodeList xNodes = document.SelectNodes("/ROOT/VIGILANZE/VIGILANZA[@Cliente='" + IDCliente + "']");
        foreach (XmlNode node in xNodes)
        {
          Hashtable result = new Hashtable();
          foreach (XmlAttribute item in node.Attributes)
          {
            result.Add(item.Name, item.Value);
          }
          results.Add(result);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return results;
    }

    //----------------------------------------------------------------------------+
    //                                GetVigilanza                                |
    //----------------------------------------------------------------------------+
    public Hashtable GetVigilanza(string IDVigilanza)
    {
      Hashtable result = new Hashtable();
      try
      {
        Open();
        if (document.SelectNodes("/ROOT/VIGILANZE") == null || document.SelectNodes("/ROOT/VIGILANZE").Count == 0)
        {
          string xml = "<VIGILANZE LastID=\"1\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/VIGILANZE");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          document.SelectSingleNode("/ROOT").AppendChild(cliente);
        }
        XmlNode xNode = document.SelectSingleNode("/ROOT/VIGILANZE/VIGILANZA[@ID='" + IDVigilanza + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                                AddVigilanza                                |
    //----------------------------------------------------------------------------+
    public string AddVigilanza_old(XmlNode node)
    {
      Open();
      if (document.SelectNodes("/ROOT/VIGILANZE") == null || document.SelectNodes("/ROOT/VIGILANZE").Count == 0)
      {
        string xml = "<VIGILANZE LastID=\"1\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/VIGILANZE");
        XmlNode cliente = document.ImportNode(tmpNode, true);
        document.SelectSingleNode("/ROOT").AppendChild(cliente);
      }
      XmlNode xNode = document.SelectSingleNode("/ROOT/VIGILANZE");
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      XmlNode xtmp = document.ImportNode(node, true);
      xNode.AppendChild(xtmp);
      xNode.Attributes["LastID"].Value = ID;
      Save();
      Close();
      return ID;
    }
    public string AddVigilanza(XmlNode node)
    {
#if (!DBG_TEST)
      return AddVigilanza_old(node);
#endif
      Open();
      if (document.SelectNodes("/ROOT/VIGILANZE") == null || document.SelectNodes("/ROOT/VIGILANZE").Count == 0)
      {
        string xml = "<VIGILANZE LastID=\"1\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/VIGILANZE");
        XmlNode cliente = document.ImportNode(tmpNode, true);
        document.SelectSingleNode("/ROOT").AppendChild(cliente);
      }
      XmlNode xNode = document.SelectSingleNode("/ROOT/VIGILANZE");
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetVigilanza", conn);
        cmd.Parameters.AddWithValue("@IDVigilanza", ID);
        cmd.Parameters.AddWithValue("@rec", node.OuterXml);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "AddVigilanza(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      Save();
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      Close();
      return ID;
    }

    //----------------------------------------------------------------------------+
    //                              DeleteVigilanza                               |
    //----------------------------------------------------------------------------+
    public void DeleteVigilanza_old(int IDVigilanza)
    {
      try
      {
        Open();
        if (document.SelectNodes("/ROOT/VIGILANZE") == null || document.SelectNodes("/ROOT/VIGILANZE").Count == 0)
        {
          string xml = "<VIGILANZE LastID=\"1\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/VIGILANZE");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          document.SelectSingleNode("/ROOT").AppendChild(cliente);
        }
        XmlNode xNode = document.SelectSingleNode("/ROOT/VIGILANZE/VIGILANZA[@ID='" + IDVigilanza.ToString() + "']");
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        xNode.ParentNode.RemoveChild(xNode);
        Save();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDVigilanza + "'][@Tree='" + (Convert.ToInt32(App.TipoFile.Vigilanza)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
          node.ParentNode.RemoveChild(node);
        }
        xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
    }
    public void DeleteVigilanza(int IDVigilanza, string idcliente)
        {

      try
      {
                cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.Vigilanza)).ToString(), idcliente);
                cBusinessObjects.DeleteSessione("Vigilanza",IDVigilanza, idcliente);
                Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/VIGILANZE/VIGILANZA[@ID='" + IDVigilanza.ToString() + "']");
        if (App.m_xmlCache.Contains(xNode.Attributes["File"].Value)) App.m_xmlCache.Remove(xNode.Attributes["File"].Value);
        if (App.m_xmlCache.Contains(xNode.Attributes["FileData"].Value)) App.m_xmlCache.Remove(xNode.Attributes["FileData"].Value);
        Close();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes(
          "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDVigilanza + "'][@Tree='" +
          (Convert.ToInt32(App.TipoFile.Vigilanza)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.DeleteVigilanza", conn);
          cmd.Parameters.AddWithValue("@IDVigilanza", IDVigilanza.ToString());
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
                            cBusinessObjects.hide_workinprogress();
                            string msg = "DeleteVigilanza(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
      }
      catch (Exception ex)
      {
        string log = ex.Message;
                cBusinessObjects.hide_workinprogress();
                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
            cBusinessObjects.hide_workinprogress();
        }


    //----------------------------------------------------------------------------+
    //                           CheckDoppio_Vigilanza                            |
    //----------------------------------------------------------------------------+
    public bool CheckDoppio_Vigilanza(int ID, int IDCliente, string Data)
    {
      Open();
      if (document.SelectNodes("/ROOT/VIGILANZE") == null || document.SelectNodes("/ROOT/VIGILANZE").Count == 0)
      {
        string xml = "<VIGILANZE LastID=\"1\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/VIGILANZE");
        XmlNode cliente = document.ImportNode(tmpNode, true);
        document.SelectSingleNode("/ROOT").AppendChild(cliente);
      }
      XmlNodeList xNodes = document.SelectNodes("/ROOT/VIGILANZE/VIGILANZA");
      foreach (XmlNode node in xNodes)
      {
        if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data)
        {
          Close();
          return false;
        }
      }
      Close();
      return true;
    }

    //----------------------------------------------------------------------------+
    //                              SetDataVigilanza                              |
    //----------------------------------------------------------------------------+
    public void SetDataVigilanza(string olddata_s, string newdata_s, int IDVerifica, int IDCliente)
    {
     
     cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.Vigilanza)).ToString(), IDCliente.ToString());

           
      MessageBox.Show("Dati sessione aggiornati");

            string olddata = olddata_s;
      string newdata = newdata_s;
      try
      {
        olddata = olddata.Substring(0, 5) + "&#xD;&#xA;" + olddata.Substring(6, 4);
        newdata = newdata.Substring(0, 5) + "&#xD;&#xA;" + newdata.Substring(6, 4);
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      ArrayList al = GetVigilanze(IDCliente.ToString());
      foreach (Hashtable item in al)
      {
        XmlManager x2 = new XmlManager();
        x2.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
        XmlDataProviderManager _test = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["File"].ToString());
        x2.SaveEncodedFile(App.AppDataDataFolder + "\\" + item["File"].ToString(), _test.Document.OuterXml.Replace(olddata, newdata));
      }
    }

    //----------------------------------------------------------------------------+
    //                                SetVigilanza                                |
    //----------------------------------------------------------------------------+
    public int SetVigilanza_old(Hashtable values, int IDVigilanza, int IDCliente)
    {
      try
      {
        string olddata = "";
        string newdata = "";
        bool changedatatbd = false;
        Open();
        if (document.SelectNodes("/ROOT/VIGILANZE") == null || document.SelectNodes("/ROOT/VIGILANZE").Count == 0)
        {
          string xml = "<VIGILANZE LastID=\"1\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/VIGILANZE");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          document.SelectSingleNode("/ROOT").AppendChild(cliente);
        }
        if (IDVigilanza == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/VIGILANZE");
          IDVigilanza = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDVigilanza.ToString();
          //Template TREE
          FileInfo fitree = new FileInfo(App.AppTemplateTreeVigilanza);
          string estensione = "." + App.AppTemplateTreeVigilanza.Split('.').Last();
          string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewtree = new FileInfo(newNametree);
          while (fnewtree.Exists)
          {
            newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewtree = new FileInfo(newNametree);
          }
          fitree.CopyTo(newNametree);
          newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");
          //Template Dati
          FileInfo fidati = new FileInfo(App.AppTemplateDataVigilanza);
          string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewdati = new FileInfo(newNamedati);
          while (fnewdati.Exists)
          {
            newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewdati = new FileInfo(newNamedati);
          }
          fidati.CopyTo(newNamedati);
          newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
          string xml = "<VIGILANZA ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Composizione=\"" + values["Composizione"].ToString() + "\" Inizio=\"" + values["Inizio"].ToString() + "\" Fine=\"" + values["Fine"].ToString() + "\" Luogo=\"" + values["Luogo"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Revisore=\"" + values["Revisore"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Presidente=\"" + values["Presidente"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Sindaco1=\"" + values["Sindaco1"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Sindaco2=\"" + values["Sindaco2"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" AssisitoDa=\"" + values["AssisitoDa"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\" DataEsecuzione=\"" + values["DataEsecuzione"].ToString() + "\" DataEsecuzione_Fine=\"" + values["DataEsecuzione_Fine"].ToString() + "\" />";
          //string xml = "<VIGILANZA ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\"  Data=\"" + values["Data"].ToString() + "\"  />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/VIGILANZA");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          root.AppendChild(cliente);
          root.Attributes["LastID"].Value = lastindex;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/VIGILANZE/VIGILANZA[@ID='" + IDVigilanza.ToString() + "']");
          if (xNode.Attributes["Inizio"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("Inizio");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Inizio"].Value = ((values["Inizio"] == null) ? "" : values["Inizio"].ToString());
          if (xNode.Attributes["Fine"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("Fine");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Fine"].Value = ((values["Fine"] == null) ? "" : values["Fine"].ToString());
          if (xNode.Attributes["Luogo"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("Luogo");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Luogo"].Value = ((values["Luogo"] == null) ? "" : values["Luogo"].ToString());
          if (xNode.Attributes["Revisore"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("Revisore");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Revisore"].Value = ((values["Revisore"] == null) ? "" : values["Revisore"].ToString());
          if (xNode.Attributes["Presidente"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("Presidente");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Presidente"].Value = ((values["Presidente"] == null) ? "" : values["Presidente"].ToString());
          if (xNode.Attributes["Sindaco1"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("Sindaco1");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Sindaco1"].Value = ((values["Sindaco1"] == null) ? "" : values["Sindaco1"].ToString());
          if (xNode.Attributes["Sindaco2"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("Sindaco2");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Sindaco2"].Value = ((values["Sindaco2"] == null) ? "" : values["Sindaco2"].ToString());
          if (xNode.Attributes["AssisitoDa"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("AssisitoDa");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["AssisitoDa"].Value = ((values["AssisitoDa"] == null) ? "" : values["AssisitoDa"].ToString());
          if (xNode.Attributes["Composizione"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("Composizione");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Composizione"].Value = ((values["Composizione"] == null) ? "" : values["Composizione"].ToString());
          if (xNode.Attributes["Data"].Value != values["Data"].ToString())
          {
            olddata = xNode.Attributes["Data"].Value;
            newdata = values["Data"].ToString();
            changedatatbd = true;
            xNode.Attributes["Data"].Value = values["Data"].ToString();
          }
          if (xNode.Attributes["DataEsecuzione"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("DataEsecuzione");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["DataEsecuzione"].Value = ((values["DataEsecuzione"] == null) ? values["Data"].ToString() : values["DataEsecuzione"].ToString());
          if (xNode.Attributes["DataEsecuzione_Fine"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("DataEsecuzione_Fine");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["DataEsecuzione_Fine"].Value = ((values["DataEsecuzione_Fine"] == null) ? values["Data"].ToString() : values["DataEsecuzione_Fine"].ToString());
        }
        Save();
        Close();
        if (changedatatbd)
        {
          SetDataVigilanza(olddata, newdata, IDVigilanza, IDCliente);
        }
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDVigilanza;
    }
    public int SetVigilanza(Hashtable values, int IDVigilanza, int IDCliente,bool cancellaretree=true)
    {
      if(cancellaretree)
          cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.Vigilanza)).ToString(), IDCliente.ToString());
      string newNametree, newNamedati;
      try
      {
        string olddata = "";
        string newdata = "";
        bool changedatatbd = false;
        Open();
        if (IDVigilanza == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/VIGILANZE");
          IDVigilanza = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDVigilanza.ToString();
          newNametree = App.AppTemplateTreeVigilanza; newNametree = newNametree.Split('\\').Last();
          newNamedati = App.AppTemplateDataVigilanza; newNamedati = newNamedati.Split('\\').Last();
          string xml = "<VIGILANZA ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" +
            ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
            "\" FileData=\"" + newNamedati + "\" Composizione=\"" + values["Composizione"].ToString() +
            "\" Inizio=\"" + values["Inizio"].ToString() + "\" Fine=\"" + values["Fine"].ToString() +
            "\" Luogo=\"" + values["Luogo"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Revisore=\"" + values["Revisore"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Presidente=\"" + values["Presidente"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Sindaco1=\"" + values["Sindaco1"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Sindaco2=\"" + values["Sindaco2"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" AssisitoDa=\"" + values["AssisitoDa"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Data=\"" + values["Data"].ToString() + "\" DataEsecuzione=\"" + values["DataEsecuzione"].ToString() +
            "\" DataEsecuzione_Fine=\"" + values["DataEsecuzione_Fine"].ToString() + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.NewVigilanza", conn);
            cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetVigilanza(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/VIGILANZE/VIGILANZA[@ID='" + IDVigilanza.ToString() + "']");
          if (xNode.Attributes["Inizio"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("Inizio");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Inizio"].Value = ((values["Inizio"] == null) ? "" : values["Inizio"].ToString());
          if (xNode.Attributes["Fine"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("Fine");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Fine"].Value = ((values["Fine"] == null) ? "" : values["Fine"].ToString());
          if (xNode.Attributes["Luogo"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("Luogo");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Luogo"].Value = ((values["Luogo"] == null) ? "" : values["Luogo"].ToString());
          if (xNode.Attributes["Revisore"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("Revisore");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Revisore"].Value = ((values["Revisore"] == null) ? "" : values["Revisore"].ToString());
          if (xNode.Attributes["Presidente"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("Presidente");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Presidente"].Value = ((values["Presidente"] == null) ? "" : values["Presidente"].ToString());
          if (xNode.Attributes["Sindaco1"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("Sindaco1");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Sindaco1"].Value = ((values["Sindaco1"] == null) ? "" : values["Sindaco1"].ToString());
          if (xNode.Attributes["Sindaco2"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("Sindaco2");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Sindaco2"].Value = ((values["Sindaco2"] == null) ? "" : values["Sindaco2"].ToString());
          if (xNode.Attributes["AssisitoDa"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("AssisitoDa");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["AssisitoDa"].Value = ((values["AssisitoDa"] == null) ? "" : values["AssisitoDa"].ToString());
          if (xNode.Attributes["Composizione"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("Composizione");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["Composizione"].Value = ((values["Composizione"] == null) ? "" : values["Composizione"].ToString());
          if (xNode.Attributes["Data"].Value != values["Data"].ToString())
          {
            olddata = xNode.Attributes["Data"].Value;
            newdata = values["Data"].ToString();
            changedatatbd = true;
            xNode.Attributes["Data"].Value = values["Data"].ToString();
          }
          if (xNode.Attributes["DataEsecuzione"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("DataEsecuzione");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["DataEsecuzione"].Value = ((values["DataEsecuzione"] == null) ? values["Data"].ToString() : values["DataEsecuzione"].ToString());
          if (xNode.Attributes["DataEsecuzione_Fine"] == null)
          {
            XmlAttribute attr = document.CreateAttribute("DataEsecuzione_Fine");
            xNode.Attributes.Append(attr);
          }
          xNode.Attributes["DataEsecuzione_Fine"].Value = ((values["DataEsecuzione_Fine"] == null) ? values["Data"].ToString() : values["DataEsecuzione_Fine"].ToString());
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.SetVigilanza", conn);
            cmd.Parameters.AddWithValue("@IDVigilanza", IDVigilanza.ToString());
            cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetVigilanza(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
        if (changedatatbd)
        {
          SetDataVigilanza(olddata, newdata, IDVigilanza, IDCliente);
        }
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDVigilanza;
    }

    //----------------------------------------------------------------------------+
    //                     GetVerificaAssociataFromVigilanza                      |
    //----------------------------------------------------------------------------+
    public Hashtable GetVerificaAssociataFromVigilanza(string ID)
    {
      Hashtable result = new Hashtable();
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/VIGILANZE/VIGILANZA[@ID='" + ID + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetVerifiche(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              result = item;
              break;
            }
          }
        }
      }
      Close();
      return result;
    }

#endregion //-------------------------------------------------------- Vigilanza

#region Flussi

    //----------------------------------------------------------------------------+
    //                               GetFlussiCount                               |
    //----------------------------------------------------------------------------+
    public int GetFlussiCount()
    {
      int result = 0;
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/FLUSSI/FLUSSO");
        if (xNodes != null)
        {
          result = xNodes.Count;
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                                 GetFlussi                                  |
    //----------------------------------------------------------------------------+
    public Hashtable GetFlussi(string IDCliente)
    {
      Hashtable result = new Hashtable();
      try
      {
        Open();
        XmlNode node = document.SelectSingleNode("/ROOT/FLUSSI/FLUSSO[@Cliente='" + IDCliente + "']");
        foreach (XmlAttribute item in node.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                           GetFlussiFromFileData                            |
    //----------------------------------------------------------------------------+
    public Hashtable GetFlussiFromFileData(string FileSessione)
    {
      Hashtable result = new Hashtable();
      FileSessione = FileSessione.Split('\\').Last();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/FLUSSI/FLUSSO[@FileData='" + FileSessione + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                                 AddFlussi                                  |
    //----------------------------------------------------------------------------+
    public void AddFlussi(XmlNode node)
    {
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/FLUSSI");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<FLUSSI/>";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/FLUSSI");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
        xNode = document.SelectSingleNode("/ROOT/FLUSSI");
      }
      XmlNode xtmp = document.ImportNode(node, true);
      xNode.AppendChild(xtmp);
      Save();
      Close();
      return;
    }

    //----------------------------------------------------------------------------+
    //                             CheckDoppio_Flussi                             |
    //----------------------------------------------------------------------------+
    public bool CheckDoppio_Flussi(int IDCliente)
    {
      Open();
      XmlNodeList xNodes = document.SelectNodes("/ROOT/FLUSSI/FLUSSO");
      foreach (XmlNode node in xNodes)
      {
        if (node.Attributes["Cliente"].Value == IDCliente.ToString())
        {
          Close();
          return false;
        }
      }
      Close();
      return true;
    }

    //----------------------------------------------------------------------------+
    //                                 SetFlussi                                  |
    //----------------------------------------------------------------------------+
    public void SetFlussi_old(Hashtable values, int IDCliente)
    {
      try
      {
        Open();
        XmlNode root = document.SelectSingleNode("/ROOT/FLUSSI");
        if (root == null)
        {
          XmlNode xroot = document.SelectSingleNode("/ROOT");
          string xmll = "<FLUSSI LastID=\"0\" />";
          XmlDocument doctmpl = new XmlDocument();
          doctmpl.LoadXml(xmll);
          XmlNode tmpNodel = doctmpl.SelectSingleNode("/FLUSSI");
          XmlNode xxtmp = document.ImportNode(tmpNodel, true);
          xroot.AppendChild(xxtmp);
          root = document.SelectSingleNode("/ROOT/FLUSSI");
        }
        //Template Dati
        string estensione = ".rflf";
        string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
        FileInfo fnewdati = new FileInfo(newNamedati);
        while (fnewdati.Exists)
        {
          newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          fnewdati = new FileInfo(newNamedati);
        }
        string xmlflussi = "<FLUSSI><Dati TIPO=\"0\"></Dati><Dati TIPO=\"1\"></Dati><Dati TIPO=\"2\"></Dati><Dati TIPO=\"3\"></Dati></FLUSSI>";
        XmlManager xf = new XmlManager();
        xf.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        xf.SaveEncodedFile(newNamedati, xmlflussi);
        newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
        string xml = "<FLUSSO Cliente=\"" + IDCliente + "\" FileData=\"" + newNamedati + "\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/FLUSSO");
        XmlNode cliente = document.ImportNode(tmpNode, true);
        root.AppendChild(cliente);
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return;
    }
    public void SetFlussi(Hashtable values, int IDCliente)
    {
#if (!DBG_TEST)
      SetFlussi_old(values, IDCliente);return;
#endif
      try
      {
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.SetFlussi", conn);
          cmd.Parameters.AddWithValue("@IDCliente", IDCliente.ToString());
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SetFlussi(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return;
    }

#endregion //----------------------------------------------------------- Flussi

#region RelazioneV

    //----------------------------------------------------------------------------+
    //                             GetRelazioniVCount                             |
    //----------------------------------------------------------------------------+
    public int GetRelazioniVCount()
    {
      int result = 0;
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIV/RELAZIONEV");
        if (xNodes != null)
        {
          result = xNodes.Count;
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                               GetRelazioniV                                |
    //----------------------------------------------------------------------------+
    public ArrayList GetRelazioniV(string IDCliente)
    {
      ArrayList results = new ArrayList();
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIV/RELAZIONEV[@Cliente='" + IDCliente + "']");
        foreach (XmlNode node in xNodes)
        {
          Hashtable result = new Hashtable();
          foreach (XmlAttribute item in node.Attributes)
          {
            result.Add(item.Name, item.Value);
          }
          results.Add(result);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return results;
    }

    //----------------------------------------------------------------------------+
    //                               GetRelazioneV                                |
    //----------------------------------------------------------------------------+
    public Hashtable GetRelazioneV(string IDRelazioneV)
    {
      Hashtable result = new Hashtable();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIV/RELAZIONEV[@ID='" + IDRelazioneV + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                         GetRelazioneVFromFileData                          |
    //----------------------------------------------------------------------------+
    public Hashtable GetRelazioneVFromFileData(string FileSessione)
    {
      Hashtable result = new Hashtable();
      FileSessione = FileSessione.Split('\\').Last();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIV/RELAZIONEV[@FileData='" + FileSessione + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                          GetBilancioFromFileData                           |
    //----------------------------------------------------------------------------+
    public Hashtable GetBilancioFromFileData(string FileSessione)
    {
      Hashtable result = new Hashtable();
      FileSessione = FileSessione.Split('\\').Last();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@FileData='" + FileSessione + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                         GetConclusioneFromFileData                         |
    //----------------------------------------------------------------------------+
    public Hashtable GetConclusioneFromFileData(string FileSessione)
    {
      Hashtable result = new Hashtable();
      FileSessione = FileSessione.Split('\\').Last();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI/CONCLUSIONE[@FileData='" + FileSessione + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                               AddRelazioneV                                |
    //----------------------------------------------------------------------------+
    public string AddRelazioneV_old(XmlNode node)
    {
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIV");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<RELAZIONIV LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIV");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
        xNode = document.SelectSingleNode("/ROOT/RELAZIONIV");
      }
      if (xNode.Attributes["LastID"].Value == "")
      {
        xNode.Attributes["LastID"].Value = "0";
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      XmlNode xtmp = document.ImportNode(node, true);
      xNode.AppendChild(xtmp);
      xNode.Attributes["LastID"].Value = ID;
      Save();
      Close();
      return ID;
    }
    public string AddRelazioneV(XmlNode node)
    {
#if (!DBG_TEST)
      return AddRelazioneV_old(node);
#endif
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIV");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<RELAZIONIV LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIV");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
        xNode = document.SelectSingleNode("/ROOT/RELAZIONIV");
      }
      if (xNode.Attributes["LastID"].Value == "")
      {
        xNode.Attributes["LastID"].Value = "0";
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetRelazioneV", conn);
        cmd.Parameters.AddWithValue("@IDRelazioneV", ID);
        cmd.Parameters.AddWithValue("@rec", node.OuterXml);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "AddRelazioneV(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      Save();
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      Close();
      return ID;
    }

    //----------------------------------------------------------------------------+
    //                              DeleteRelazioneV                              |
    //----------------------------------------------------------------------------+
    public void DeleteRelazioneV_old(int IDRelazioneV)
    {
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIV/RELAZIONEV[@ID='" + IDRelazioneV.ToString() + "']");
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        xNode.ParentNode.RemoveChild(xNode);
        Save();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDRelazioneV + "'][@Tree='" + (Convert.ToInt32(App.TipoFile.RelazioneV)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
          node.ParentNode.RemoveChild(node);
        }
        xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
    }
    public void DeleteRelazioneV(int IDRelazioneV, string idcliente)
        {

      try
      {
                cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.RelazioneV)).ToString(), idcliente);
                cBusinessObjects.DeleteSessione("RelazioneV",IDRelazioneV, idcliente);
                Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIV/RELAZIONEV[@ID='" + IDRelazioneV.ToString() + "']");
        if (App.m_xmlCache.Contains(xNode.Attributes["File"].Value)) App.m_xmlCache.Remove(xNode.Attributes["File"].Value);
        if (App.m_xmlCache.Contains(xNode.Attributes["FileData"].Value)) App.m_xmlCache.Remove(xNode.Attributes["FileData"].Value);
        Close();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDRelazioneV +
          "'][@Tree='" + (Convert.ToInt32(App.TipoFile.RelazioneV)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.DeleteRelazioneV", conn);
          cmd.Parameters.AddWithValue("@IDRelazioneV", IDRelazioneV.ToString());
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
                            cBusinessObjects.hide_workinprogress();
                            string msg = "DeleteRelazioneV(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
      }
      catch (Exception ex)
      {
        string log = ex.Message;
                cBusinessObjects.hide_workinprogress();
                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
            cBusinessObjects.hide_workinprogress();
        }

    //----------------------------------------------------------------------------+
    //                           CheckDoppio_RelazioneV                           |
    //----------------------------------------------------------------------------+
    public bool CheckDoppio_RelazioneV(int ID, int IDCliente, string Data)
    {
      Open();
      XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIV/RELAZIONEV");
      foreach (XmlNode node in xNodes)
      {
        if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data)
        {
          Close();
          return false;
        }
      }
      Close();
      return true;
    }

    //----------------------------------------------------------------------------+
    //                          SetRelazioneVIntermedio                           |
    //----------------------------------------------------------------------------+
    public int SetRelazioneVIntermedio_old(Hashtable values, int IDCliente, string dal, string al)
    {
      int IDRelazioneV = -1;
      try
      {
        Open();
        XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIV");
        if (root == null)
        {
          XmlNode xroot = document.SelectSingleNode("/ROOT");
          string xmll = "<RELAZIONIV LastID=\"0\" />";
          XmlDocument doctmpl = new XmlDocument();
          doctmpl.LoadXml(xmll);
          XmlNode tmpNodel = doctmpl.SelectSingleNode("/RELAZIONIV");
          XmlNode xxtmp = document.ImportNode(tmpNodel, true);
          xroot.AppendChild(xxtmp);
          root = document.SelectSingleNode("/ROOT/RELAZIONIV");
        }
        IDRelazioneV = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
        string lastindex = IDRelazioneV.ToString();
        //Template TREE
        FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneV);
        string estensione = "." + App.AppTemplateTreeRelazioneV.Split('.').Last();
        string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
        FileInfo fnewtree = new FileInfo(newNametree);
        while (fnewtree.Exists)
        {
          newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          fnewtree = new FileInfo(newNametree);
        }
        fitree.CopyTo(newNametree);
        newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");
        //Template Dati
        FileInfo fidati = new FileInfo(App.AppTemplateDataRelazioneV);
        string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
        FileInfo fnewdati = new FileInfo(newNamedati);
        while (fnewdati.Exists)
        {
          newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          fnewdati = new FileInfo(newNamedati);
        }
        fidati.CopyTo(newNamedati);
        newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
        XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
        string xml = "<RELAZIONEV ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONEV");
        XmlNode cliente = document.ImportNode(tmpNode, true);
        root.AppendChild(cliente);
        root.Attributes["LastID"].Value = lastindex;
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneV;
    }
    public int SetRelazioneVIntermedio(Hashtable values, int IDCliente, string dal, string al)
    {
#if (!DBG_TEST)
      return SetRelazioneVIntermedio_old(values, IDCliente, dal, al);
#endif
      int IDRelazioneV = -1;
      string newNametree, newNamedati;
      try
      {
        Open();
        XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIV");
        if (root == null)
        {
          XmlNode xroot = document.SelectSingleNode("/ROOT");
          string xmll = "<RELAZIONIV LastID=\"0\" />";
          XmlDocument doctmpl = new XmlDocument();
          doctmpl.LoadXml(xmll);
          XmlNode tmpNodel = doctmpl.SelectSingleNode("/RELAZIONIV");
          XmlNode xxtmp = document.ImportNode(tmpNodel, true);
          xroot.AppendChild(xxtmp);
          root = document.SelectSingleNode("/ROOT/RELAZIONIV");
        }
        IDRelazioneV = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
        string lastindex = IDRelazioneV.ToString();
        newNametree = App.AppTemplateTreeRelazioneV; newNametree = newNametree.Split('\\').Last();
        newNamedati = App.AppTemplateDataRelazioneV; newNamedati = newNamedati.Split('\\').Last();
        XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
        string xml = "<RELAZIONEV ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente +
          "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
          "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
          "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value +
          "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.NewRelazioneV", conn);
          cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SetRelazioneVIntermedio(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneV;
    }

    //----------------------------------------------------------------------------+
    //                               SetRelazioneV                                |
    //----------------------------------------------------------------------------+
    public int SetRelazioneV_old(Hashtable values, int IDRelazioneV, int IDCliente)
    {
      try
      {
        Open();
        if (IDRelazioneV == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIV");
          if (root == null)
          {
            XmlNode xroot = document.SelectSingleNode("/ROOT");
            string xmll = "<RELAZIONIV LastID=\"0\" />";
            XmlDocument doctmpl = new XmlDocument();
            doctmpl.LoadXml(xmll);
            XmlNode tmpNodel = doctmpl.SelectSingleNode("/RELAZIONIV");
            XmlNode xxtmp = document.ImportNode(tmpNodel, true);
            xroot.AppendChild(xxtmp);
            root = document.SelectSingleNode("/ROOT/RELAZIONIV");
          }
          IDRelazioneV = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDRelazioneV.ToString();
          //Template TREE
          FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneV);
          string estensione = "." + App.AppTemplateTreeRelazioneV.Split('.').Last();
          string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewtree = new FileInfo(newNametree);
          while (fnewtree.Exists)
          {
            newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewtree = new FileInfo(newNametree);
          }
          fitree.CopyTo(newNametree);
          newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");
          //Template Dati
          FileInfo fidati = new FileInfo(App.AppTemplateDataRelazioneV);
          string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewdati = new FileInfo(newNamedati);
          while (fnewdati.Exists)
          {
            newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewdati = new FileInfo(newNamedati);
          }
          fidati.CopyTo(newNamedati);
          newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
          XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
          string xml = "<RELAZIONEV ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONEV");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          root.AppendChild(cliente);
          root.Attributes["LastID"].Value = lastindex;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIV/RELAZIONEV[@ID='" + IDRelazioneV.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          xNode.Attributes["Data"].Value = values["Data"].ToString();
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneV;
    }
    public int SetRelazioneV(Hashtable values, int IDRelazioneV, int IDCliente)
    {
#if (!DBG_TEST)
      return SetRelazioneV_old(values, IDRelazioneV, IDCliente);
#endif
      string newNametree, newNamedati;
      try
      {
        Open();
        if (IDRelazioneV == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIV");
          if (root == null)
          {
            XmlNode xroot = document.SelectSingleNode("/ROOT");
            string xmll = "<RELAZIONIV LastID=\"0\" />";
            XmlDocument doctmpl = new XmlDocument();
            doctmpl.LoadXml(xmll);
            XmlNode tmpNodel = doctmpl.SelectSingleNode("/RELAZIONIV");
            XmlNode xxtmp = document.ImportNode(tmpNodel, true);
            xroot.AppendChild(xxtmp);
            root = document.SelectSingleNode("/ROOT/RELAZIONIV");
          }
          IDRelazioneV = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDRelazioneV.ToString();
          newNametree = App.AppTemplateTreeRelazioneV; newNametree = newNametree.Split('\\').Last();
          newNamedati = App.AppTemplateDataRelazioneV; newNamedati = newNamedati.Split('\\').Last();
          XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
          string xml = "<RELAZIONEV ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" +
            ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
            "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" +
            XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" +
            XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" +
            XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.NewRelazioneV", conn);
            cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetRelazioneV(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIV/RELAZIONEV[@ID='" + IDRelazioneV.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          xNode.Attributes["Data"].Value = values["Data"].ToString();
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.SetRelazioneV", conn);
            cmd.Parameters.AddWithValue("@IDRelazioneV", IDRelazioneV.ToString());
            cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetRelazioneV(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneV;
    }

    //----------------------------------------------------------------------------+
    //                  GetRevisioneAssociataFromRelazioneVFile                   |
    //----------------------------------------------------------------------------+
    public string GetRevisioneAssociataFromRelazioneVFile(string FileRelazioneV)
    {
      string FileRevisione = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIV/RELAZIONEV[@FileData='" + FileRelazioneV.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetRevisioni(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileRevisione = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileRevisione;
    }

    //----------------------------------------------------------------------------+
    //                   GetBilancioAssociatoFromRelazioneVFile                   |
    //----------------------------------------------------------------------------+
    public string GetBilancioAssociatoFromRelazioneVFile(string FileRelazioneV)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIV/RELAZIONEV[@FileData='" + FileRelazioneV.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                  GetBilancioAssociatoFromConclusioniFile                   |
    //----------------------------------------------------------------------------+
    public string GetBilancioAssociatoFromConclusioniFile(string FileConclusioni)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI/CONCLUSIONE[@FileData='" + FileConclusioni.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                 GetBilancioTreeAssociatoFromRelazioneVFile                 |
    //----------------------------------------------------------------------------+
    public string GetBilancioTreeAssociatoFromRelazioneVFile(string FileRelazioneV)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIV/RELAZIONEV[@FileData='" + FileRelazioneV.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = App.AppDataDataFolder + "\\" + item["File"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                  GetBilancioIDAssociatoFromRelazioneVFile                  |
    //----------------------------------------------------------------------------+
    public string GetBilancioIDAssociatoFromRelazioneVFile(string FileRelazioneV)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIV/RELAZIONEV[@FileData='" + FileRelazioneV.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = item["ID"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                 GetAllRelazioneVAssociataFromBilancioFile                  |
    //----------------------------------------------------------------------------+
    public Hashtable GetAllRelazioneVAssociataFromBilancioFile(string FileBilancio)
    {
      Hashtable RelazioneV = null;
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@FileData='" + FileBilancio.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetRelazioniV(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              RelazioneV = item;
              break;
            }
          }
        }
      }
      Close();
      return RelazioneV;
    }

    //----------------------------------------------------------------------------+
    //                 GetAllBilancioAssociatoFromRelazioneVFile                  |
    //----------------------------------------------------------------------------+
    public Hashtable GetAllBilancioAssociatoFromRelazioneVFile(string FileRelazioneV)
    {
      Hashtable Bilancio = null;
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIV/RELAZIONEV[@FileData='" + FileRelazioneV.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              Bilancio = item;
              break;
            }
          }
        }
      }
      Close();
      return Bilancio;
    }

#endregion //------------------------------------------------------- RelazioneV

#region RelazioneB

    //----------------------------------------------------------------------------+
    //                             GetRelazioniBCount                             |
    //----------------------------------------------------------------------------+
    public int GetRelazioniBCount()
    {
      int result = 0;
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIB/RELAZIONEB");
        if (xNodes != null)
        {
          result = xNodes.Count;
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                               GetRelazioniB                                |
    //----------------------------------------------------------------------------+
    public ArrayList GetRelazioniB(string IDCliente)
    {
      ArrayList results = new ArrayList();
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIB/RELAZIONEB[@Cliente='" + IDCliente + "']");
        foreach (XmlNode node in xNodes)
        {
          Hashtable result = new Hashtable();
          foreach (XmlAttribute item in node.Attributes)
          {
            result.Add(item.Name, item.Value);
          }
          results.Add(result);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return results;
    }

    //----------------------------------------------------------------------------+
    //                               GetRelazioneB                                |
    //----------------------------------------------------------------------------+
    public Hashtable GetRelazioneB(string IDRelazioneB)
    {
      Hashtable result = new Hashtable();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIB/RELAZIONEB[@ID='" + IDRelazioneB + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                         GetRelazioneBFromFileData                          |
    //----------------------------------------------------------------------------+
    public Hashtable GetRelazioneBFromFileData(string FileSessione)
    {
      Hashtable result = new Hashtable();
      FileSessione = FileSessione.Split('\\').Last();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIB/RELAZIONEB[@FileData='" + FileSessione + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                               AddRelazioneB                                |
    //----------------------------------------------------------------------------+
    public string AddRelazioneB_old(XmlNode node)
    {
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIB");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<RELAZIONIB LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIB");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
        xNode = document.SelectSingleNode("/ROOT/RELAZIONIB");
      }
      if (xNode.Attributes["LastID"].Value == "")
      {
        xNode.Attributes["LastID"].Value = "0";
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      XmlNode xtmp = document.ImportNode(node, true);
      xNode.AppendChild(xtmp);
      xNode.Attributes["LastID"].Value = ID;
      Save();
      Close();
      return ID;
    }
    public string AddRelazioneB(XmlNode node)
    {
#if (!DBG_TEST)
      return AddRelazioneB_old(node);
#endif
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIB");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<RELAZIONIB LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIB");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
        xNode = document.SelectSingleNode("/ROOT/RELAZIONIB");
      }
      if (xNode.Attributes["LastID"].Value == "")
      {
        xNode.Attributes["LastID"].Value = "0";
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetRelazioneB", conn);
        cmd.Parameters.AddWithValue("@IDRelazioneB", ID);
        cmd.Parameters.AddWithValue("@rec", node.OuterXml);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "AddRelazioneB(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      Save();
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      Close();
      return ID;
    }

    //----------------------------------------------------------------------------+
    //                              DeleteRelazioneB                              |
    //----------------------------------------------------------------------------+
    public void DeleteRelazioneB_old(int IDRelazioneB)
    {
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIB/RELAZIONEB[@ID='" + IDRelazioneB.ToString() + "']");
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        xNode.ParentNode.RemoveChild(xNode);
        Save();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDRelazioneB + "'][@Tree='" + (Convert.ToInt32(App.TipoFile.RelazioneB)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
          node.ParentNode.RemoveChild(node);
        }
        xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
    }
    public void DeleteRelazioneB(int IDRelazioneB, string idcliente)
        {

      try
      {
                cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.RelazioneB)).ToString(), idcliente);
                cBusinessObjects.DeleteSessione("RelazioneB",IDRelazioneB, idcliente);
                Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIB/RELAZIONEB[@ID='" + IDRelazioneB.ToString() + "']");
        if (App.m_xmlCache.Contains(xNode.Attributes["File"].Value)) App.m_xmlCache.Remove(xNode.Attributes["File"].Value);
        if (App.m_xmlCache.Contains(xNode.Attributes["FileData"].Value)) App.m_xmlCache.Remove(xNode.Attributes["FileData"].Value);
        Close();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDRelazioneB +
          "'][@Tree='" + (Convert.ToInt32(App.TipoFile.RelazioneB)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.DeleteRelazioneB", conn);
          cmd.Parameters.AddWithValue("@IDRelazioneB", IDRelazioneB.ToString());
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
                            cBusinessObjects.hide_workinprogress();
                            string msg = "DeleteRelazioneB(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
      }
      catch (Exception ex)
      {
        string log = ex.Message;
                cBusinessObjects.hide_workinprogress();
                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
            cBusinessObjects.hide_workinprogress();
        }

    //----------------------------------------------------------------------------+
    //                           CheckDoppio_RelazioneB                           |
    //----------------------------------------------------------------------------+
    public bool CheckDoppio_RelazioneB(int ID, int IDCliente, string Data)
    {
      Open();
      XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIB/RELAZIONEB");
      foreach (XmlNode node in xNodes)
      {
        if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data)
        {
          Close();
          return false;
        }
      }
      Close();
      return true;
    }

    //----------------------------------------------------------------------------+
    //                          SetRelazioneBIntermedio                           |
    //----------------------------------------------------------------------------+
    public int SetRelazioneBIntermedio_old(Hashtable values, int IDCliente, string dal, string al)
    {
      int IDRelazioneB = -1;
      try
      {
        Open();
        XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIB");
        if (root == null)
        {
          XmlNode xroot = document.SelectSingleNode("/ROOT");
          string xmla = "<RELAZIONIB LastID=\"0\" />";
          XmlDocument doctmpa = new XmlDocument();
          doctmpa.LoadXml(xmla);
          XmlNode tmpNodea = doctmpa.SelectSingleNode("/RELAZIONIB");
          XmlNode xxtmp = document.ImportNode(tmpNodea, true);
          xroot.AppendChild(xxtmp);
          root = document.SelectSingleNode("/ROOT/RELAZIONIB");
        }
        IDRelazioneB = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
        string lastindex = IDRelazioneB.ToString();
        //Template TREE
        FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneB);
        string estensione = "." + App.AppTemplateTreeRelazioneB.Split('.').Last();
        string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
        FileInfo fnewtree = new FileInfo(newNametree);
        while (fnewtree.Exists)
        {
          newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          fnewtree = new FileInfo(newNametree);
        }
        fitree.CopyTo(newNametree);
        newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");
        //Template Dati
        FileInfo fidati = new FileInfo(App.AppTemplateDataRelazioneB);
        string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
        FileInfo fnewdati = new FileInfo(newNamedati);
        while (fnewdati.Exists)
        {
          newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          fnewdati = new FileInfo(newNamedati);
        }
        fidati.CopyTo(newNamedati);
        newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
        XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
        string xml = "<RELAZIONEB ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONEB");
        XmlNode cliente = document.ImportNode(tmpNode, true);
        root.AppendChild(cliente);
        root.Attributes["LastID"].Value = lastindex;
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneB;
    }
    public int SetRelazioneBIntermedio(Hashtable values, int IDCliente, string dal, string al)
    {
#if (!DBG_TEST)
      return SetRelazioneBIntermedio_old(values, IDCliente, dal, al);
#endif
      int IDRelazioneB = -1;
      string newNametree, newNamedati;
      try
      {
        Open();
        XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIB");
        if (root == null)
        {
          XmlNode xroot = document.SelectSingleNode("/ROOT");
          string xmla = "<RELAZIONIB LastID=\"0\" />";
          XmlDocument doctmpa = new XmlDocument();
          doctmpa.LoadXml(xmla);
          XmlNode tmpNodea = doctmpa.SelectSingleNode("/RELAZIONIB");
          XmlNode xxtmp = document.ImportNode(tmpNodea, true);
          xroot.AppendChild(xxtmp);
          root = document.SelectSingleNode("/ROOT/RELAZIONIB");
        }
        IDRelazioneB = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
        string lastindex = IDRelazioneB.ToString();
        newNametree = App.AppTemplateTreeRelazioneB; newNametree = newNametree.Split('\\').Last();
        newNamedati = App.AppTemplateDataRelazioneB; newNamedati = newNamedati.Split('\\').Last();
        XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
        string xml = "<RELAZIONEB ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente +
          "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
          "\" FileData=\"" + newNamedati + "\" Note=\"" +
          values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() +
          "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" +
          dal + "\" EsercizioAl=\"" + al + "\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.NewRelazioneB", conn);
          cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SetRelazioneBIntermedio(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneB;
    }

    //----------------------------------------------------------------------------+
    //                               SetRelazioneB                                |
    //----------------------------------------------------------------------------+
    public int SetRelazioneB_old(Hashtable values, int IDRelazioneB, int IDCliente)
    {
      try
      {
        Open();
        if (IDRelazioneB == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIB");
          if (root == null)
          {
            XmlNode xroot = document.SelectSingleNode("/ROOT");
            string xmla = "<RELAZIONIB LastID=\"0\" />";
            XmlDocument doctmpa = new XmlDocument();
            doctmpa.LoadXml(xmla);
            XmlNode tmpNodea = doctmpa.SelectSingleNode("/RELAZIONIB");
            XmlNode xxtmp = document.ImportNode(tmpNodea, true);
            xroot.AppendChild(xxtmp);
            root = document.SelectSingleNode("/ROOT/RELAZIONIB");
          }
          IDRelazioneB = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDRelazioneB.ToString();
          //Template TREE
          FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneB);
          string estensione = "." + App.AppTemplateTreeRelazioneB.Split('.').Last();
          string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewtree = new FileInfo(newNametree);
          while (fnewtree.Exists)
          {
            newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewtree = new FileInfo(newNametree);
          }
          fitree.CopyTo(newNametree);
          newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");
          //Template Dati
          FileInfo fidati = new FileInfo(App.AppTemplateDataRelazioneB);
          string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewdati = new FileInfo(newNamedati);
          while (fnewdati.Exists)
          {
            newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewdati = new FileInfo(newNamedati);
          }
          fidati.CopyTo(newNamedati);
          newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
          XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
          string xml = "<RELAZIONEB ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONEB");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          root.AppendChild(cliente);
          root.Attributes["LastID"].Value = lastindex;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIB/RELAZIONEB[@ID='" + IDRelazioneB.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          xNode.Attributes["Data"].Value = values["Data"].ToString();
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneB;
    }
    public int SetRelazioneB(Hashtable values, int IDRelazioneB, int IDCliente)
    {
#if (!DBG_TEST)
      return SetRelazioneB_old(values, IDRelazioneB, IDCliente);
#endif
      string newNametree, newNamedati;
      try
      {
        Open();
        if (IDRelazioneB == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIB");
          if (root == null)
          {
            XmlNode xroot = document.SelectSingleNode("/ROOT");
            string xmla = "<RELAZIONIB LastID=\"0\" />";
            XmlDocument doctmpa = new XmlDocument();
            doctmpa.LoadXml(xmla);
            XmlNode tmpNodea = doctmpa.SelectSingleNode("/RELAZIONIB");
            XmlNode xxtmp = document.ImportNode(tmpNodea, true);
            xroot.AppendChild(xxtmp);
            root = document.SelectSingleNode("/ROOT/RELAZIONIB");
          }
          IDRelazioneB = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDRelazioneB.ToString();
          newNametree = App.AppTemplateTreeRelazioneB; newNametree = newNametree.Split('\\').Last();
          newNamedati = App.AppTemplateDataRelazioneB; newNamedati = newNamedati.Split('\\').Last();
          XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
          string xml = "<RELAZIONEB ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" +
            ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
            "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value +
            "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value +
            "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.NewRelazioneB", conn);
            cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetRelazioneB(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIB/RELAZIONEB[@ID='" + IDRelazioneB.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          xNode.Attributes["Data"].Value = values["Data"].ToString();
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.SetRelazioneB", conn);
            cmd.Parameters.AddWithValue("@IDRelazioneB", IDRelazioneB.ToString());
            cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetRelazioneB(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneB;
    }

    //----------------------------------------------------------------------------+
    //                  GetRevisioneAssociataFromRelazioneBFile                   |
    //----------------------------------------------------------------------------+
    public string GetRevisioneAssociataFromRelazioneBFile(string FileRelazioneV)
    {
      string FileRevisione = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIB/RELAZIONEB[@FileData='" + FileRelazioneV.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetRevisioni(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileRevisione = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileRevisione;
    }

    //----------------------------------------------------------------------------+
    //                   GetBilancioAssociatoFromRelazioneBFile                   |
    //----------------------------------------------------------------------------+
    public string GetBilancioAssociatoFromRelazioneBFile(string FileRelazioneB)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIB/RELAZIONEB[@FileData='" + FileRelazioneB.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                 GetBilancioTreeAssociatoFromRelazioneBFile                 |
    //----------------------------------------------------------------------------+
    public string GetBilancioTreeAssociatoFromRelazioneBFile(string FileRelazioneB)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIB/RELAZIONEB[@FileData='" + FileRelazioneB.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = App.AppDataDataFolder + "\\" + item["File"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                  GetBilancioIDAssociatoFromRelazioneBFile                  |
    //----------------------------------------------------------------------------+
    public string GetBilancioIDAssociatoFromRelazioneBFile(string FileRelazioneB)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIB/RELAZIONEB[@FileData='" + FileRelazioneB.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = item["ID"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                 GetAllRelazioneBAssociataFromBilancioFile                  |
    //----------------------------------------------------------------------------+
    public Hashtable GetAllRelazioneBAssociataFromBilancioFile(string FileBilancio)
    {
      Hashtable RelazioneB = null;
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@FileData='" + FileBilancio.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetRelazioniB(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              RelazioneB = item;
              break;
            }
          }
        }
      }
      Close();
      return RelazioneB;
    }

    //----------------------------------------------------------------------------+
    //                 GetAllBilancioAssociatoFromRelazioneBFile                  |
    //----------------------------------------------------------------------------+
    public Hashtable GetAllBilancioAssociatoFromRelazioneBFile(string FileRelazioneB)
    {
      Hashtable Bilancio = null;
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIB/RELAZIONEB[@FileData='" + FileRelazioneB.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              Bilancio = item;
              break;
            }
          }
        }
      }
      Close();
      return Bilancio;
    }

#endregion //------------------------------------------------------- RelazioneB

#region RelazioneVC

    //----------------------------------------------------------------------------+
    //                            GetRelazioniVCCount                             |
    //----------------------------------------------------------------------------+
    public int GetRelazioniVCCount()
    {
      int result = 0;
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC");
        if (xNodes != null)
        {
          result = xNodes.Count;
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                               GetRelazioniVC                               |
    //----------------------------------------------------------------------------+
    public ArrayList GetRelazioniVC(string IDCliente)
    {
      ArrayList results = new ArrayList();
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC[@Cliente='" + IDCliente + "']");
        foreach (XmlNode node in xNodes)
        {
          Hashtable result = new Hashtable();
          foreach (XmlAttribute item in node.Attributes)
          {
            result.Add(item.Name, item.Value);
          }
          results.Add(result);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return results;
    }

    //----------------------------------------------------------------------------+
    //                               GetRelazioneVC                               |
    //----------------------------------------------------------------------------+
    public Hashtable GetRelazioneVC(string IDRelazioneVC)
    {
      Hashtable result = new Hashtable();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@ID='" + IDRelazioneVC + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                         GetRelazioneVCFromFileData                         |
    //----------------------------------------------------------------------------+
    public Hashtable GetRelazioneVCFromFileData(string FileSessione)
    {
      Hashtable result = new Hashtable();
      FileSessione = FileSessione.Split('\\').Last();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@FileData='" + FileSessione + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                               AddRelazioneVC                               |
    //----------------------------------------------------------------------------+
    public string AddRelazioneVC_old(XmlNode node)
    {
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<RELAZIONIVC LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIVC");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
        xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC");
      }
      if (xNode.Attributes["LastID"].Value == "")
      {
        xNode.Attributes["LastID"].Value = "0";
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      XmlNode xtmp = document.ImportNode(node, true);
      xNode.AppendChild(xtmp);
      xNode.Attributes["LastID"].Value = ID;
      Save();
      Close();
      return ID;
    }
    public string AddRelazioneVC(XmlNode node)
    {
#if (!DBG_TEST)
      return AddRelazioneVC_old(node);
#endif
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<RELAZIONIVC LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIVC");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
        xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC");
      }
      if (xNode.Attributes["LastID"].Value == "")
      {
        xNode.Attributes["LastID"].Value = "0";
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetRelazioneVC", conn);
        cmd.Parameters.AddWithValue("@IDRelazioneVC", ID);
        cmd.Parameters.AddWithValue("@rec", node.OuterXml);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "AddRelazioneVC(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      Save();
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      Close();
      return ID;
    }

    //----------------------------------------------------------------------------+
    //                             DeleteRelazioneVC                              |
    //----------------------------------------------------------------------------+
    public void DeleteRelazioneVC_old(int IDRelazioneVC)
    {
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@ID='" + IDRelazioneVC.ToString() + "']");
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        xNode.ParentNode.RemoveChild(xNode);
        Save();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDRelazioneVC + "'][@Tree='" + (Convert.ToInt32(App.TipoFile.RelazioneVC)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
          node.ParentNode.RemoveChild(node);
        }
        xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
    }
    public void DeleteRelazioneVC(int IDRelazioneVC, string idcliente)
        {

      try
      {
                cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.RelazioneVC)).ToString(), idcliente);
                cBusinessObjects.DeleteSessione("RelazioneVC",IDRelazioneVC, idcliente);
                Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@ID='" + IDRelazioneVC.ToString() + "']");
        if (App.m_xmlCache.Contains(xNode.Attributes["File"].Value)) App.m_xmlCache.Remove(xNode.Attributes["File"].Value);
        if (App.m_xmlCache.Contains(xNode.Attributes["FileData"].Value)) App.m_xmlCache.Remove(xNode.Attributes["FileData"].Value);
        Close();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes(
          "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDRelazioneVC + "'][@Tree='" +
          (Convert.ToInt32(App.TipoFile.RelazioneVC)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.DeleteRelazioneVC", conn);
          cmd.Parameters.AddWithValue("@IDRelazioneVC", IDRelazioneVC.ToString());
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
                            cBusinessObjects.hide_workinprogress();
                            string msg = "DeleteRelazioneVC(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
      }
      catch (Exception ex)
      {
        string log = ex.Message;
                cBusinessObjects.hide_workinprogress();
                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
            cBusinessObjects.hide_workinprogress();
        }

    //----------------------------------------------------------------------------+
    //                          CheckDoppio_RelazioneVC                           |
    //----------------------------------------------------------------------------+
    public bool CheckDoppio_RelazioneVC(int ID, int IDCliente, string Data)
    {
      Open();
      XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC");
      foreach (XmlNode node in xNodes)
      {
        if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data)
        {
          Close();
          return false;
        }
      }
      Close();
      return true;
    }

    //----------------------------------------------------------------------------+
    //                          SetRelazioneVCIntermedio                          |
    //----------------------------------------------------------------------------+
    public int SetRelazioneVCIntermedio_old(Hashtable values, int IDCliente, string dal, string al)
    {
      int IDRelazioneVC = -1;
      try
      {
        Open();
        XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIVC");
        if (root == null)
        {
          XmlNode xroot = document.SelectSingleNode("/ROOT");
          string xmll = "<RELAZIONIVC LastID=\"0\" />";
          XmlDocument doctmpl = new XmlDocument();
          doctmpl.LoadXml(xmll);
          XmlNode tmpNodel = doctmpl.SelectSingleNode("/RELAZIONIVC");
          XmlNode xxtmp = document.ImportNode(tmpNodel, true);
          xroot.AppendChild(xxtmp);
          root = document.SelectSingleNode("/ROOT/RELAZIONIVC");
        }
        IDRelazioneVC = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
        string lastindex = IDRelazioneVC.ToString();
        //Template TREE
        FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneVC);
        string estensione = "." + App.AppTemplateTreeRelazioneVC.Split('.').Last();
        string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
        FileInfo fnewtree = new FileInfo(newNametree);
        while (fnewtree.Exists)
        {
          newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          fnewtree = new FileInfo(newNametree);
        }
        fitree.CopyTo(newNametree);
        newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");
        //Template Dati
        FileInfo fidati = new FileInfo(App.AppTemplateDataRelazioneVC);
        string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
        FileInfo fnewdati = new FileInfo(newNamedati);
        while (fnewdati.Exists)
        {
          newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          fnewdati = new FileInfo(newNamedati);
        }
        fidati.CopyTo(newNamedati);
        newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
        XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
        string xml = "<RELAZIONEVC ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONEVC");
        XmlNode cliente = document.ImportNode(tmpNode, true);
        root.AppendChild(cliente);
        root.Attributes["LastID"].Value = lastindex;
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneVC;
    }
    public int SetRelazioneVCIntermedio(Hashtable values, int IDCliente, string dal, string al)
    {
#if (!DBG_TEST)
      return SetRelazioneVCIntermedio_old(values, IDCliente, dal, al);
#endif
      int IDRelazioneVC = -1;
      string newNametree, newNamedati;
      try
      {
        Open();
        XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIVC");
        if (root == null)
        {
          XmlNode xroot = document.SelectSingleNode("/ROOT");
          string xmll = "<RELAZIONIVC LastID=\"0\" />";
          XmlDocument doctmpl = new XmlDocument();
          doctmpl.LoadXml(xmll);
          XmlNode tmpNodel = doctmpl.SelectSingleNode("/RELAZIONIVC");
          XmlNode xxtmp = document.ImportNode(tmpNodel, true);
          xroot.AppendChild(xxtmp);
          root = document.SelectSingleNode("/ROOT/RELAZIONIVC");
        }
        IDRelazioneVC = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
        string lastindex = IDRelazioneVC.ToString();
        newNametree = App.AppTemplateTreeRelazioneVC; newNametree = newNametree.Split('\\').Last();
        newNamedati = App.AppTemplateDataRelazioneVC; newNamedati = newNamedati.Split('\\').Last();
        XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
        string xml = "<RELAZIONEVC ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente +
          "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
          "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
          "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value +
          "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.NewRelazioneVC", conn);
          cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SetRelazioneVCIntermedio(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneVC;
    }

    //----------------------------------------------------------------------------+
    //                               SetRelazioneVC                               |
    //----------------------------------------------------------------------------+
    public int SetRelazioneVC_old(Hashtable values, int IDRelazioneVC, int IDCliente)
    {
      try
      {
        Open();
        if (IDRelazioneVC == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIVC");
          if (root == null)
          {
            XmlNode xroot = document.SelectSingleNode("/ROOT");
            string xmll = "<RELAZIONIVC LastID=\"0\" />";
            XmlDocument doctmpl = new XmlDocument();
            doctmpl.LoadXml(xmll);
            XmlNode tmpNodel = doctmpl.SelectSingleNode("/RELAZIONIVC");
            XmlNode xxtmp = document.ImportNode(tmpNodel, true);
            xroot.AppendChild(xxtmp);
            root = document.SelectSingleNode("/ROOT/RELAZIONIVC");
          }
          IDRelazioneVC = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDRelazioneVC.ToString();
          //Template TREE
          FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneVC);
          string estensione = "." + App.AppTemplateTreeRelazioneVC.Split('.').Last();
          string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewtree = new FileInfo(newNametree);
          while (fnewtree.Exists)
          {
            newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewtree = new FileInfo(newNametree);
          }
          fitree.CopyTo(newNametree);
          newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");
          //Template Dati
          FileInfo fidati = new FileInfo(App.AppTemplateDataRelazioneVC);
          string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewdati = new FileInfo(newNamedati);
          while (fnewdati.Exists)
          {
            newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewdati = new FileInfo(newNamedati);
          }
          fidati.CopyTo(newNamedati);
          newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
          XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
          string xml = "<RELAZIONEVC ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONEVC");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          root.AppendChild(cliente);
          root.Attributes["LastID"].Value = lastindex;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@ID='" + IDRelazioneVC.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          xNode.Attributes["Data"].Value = values["Data"].ToString();
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneVC;
    }
    public int SetRelazioneVC(Hashtable values, int IDRelazioneVC, int IDCliente)
    {
#if (!DBG_TEST)
      return SetRelazioneVC_old(values, IDRelazioneVC, IDCliente);
#endif
      string newNametree, newNamedati;
      try
      {
        Open();
        if (IDRelazioneVC == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIVC");
          if (root == null)
          {
            XmlNode xroot = document.SelectSingleNode("/ROOT");
            string xmll = "<RELAZIONIVC LastID=\"0\" />";
            XmlDocument doctmpl = new XmlDocument();
            doctmpl.LoadXml(xmll);
            XmlNode tmpNodel = doctmpl.SelectSingleNode("/RELAZIONIVC");
            XmlNode xxtmp = document.ImportNode(tmpNodel, true);
            xroot.AppendChild(xxtmp);
            root = document.SelectSingleNode("/ROOT/RELAZIONIVC");
          }
          IDRelazioneVC = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDRelazioneVC.ToString();
          newNametree = App.AppTemplateTreeRelazioneVC; newNametree = newNametree.Split('\\').Last();
          newNamedati = App.AppTemplateDataRelazioneVC; newNamedati = newNamedati.Split('\\').Last();
          XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
          string xml = "<RELAZIONEVC ID=\"" + lastindex + "\" Cliente=\"" + IDCliente +
            "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
            "\" FileData=\"" + newNamedati + "\" Note=\"" +
            values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Data=\"" + values["Data"].ToString() +
            "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value +
            "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value +
            "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.NewRelazioneVC", conn);
            cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetRelazioneVC(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@ID='" + IDRelazioneVC.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          xNode.Attributes["Data"].Value = values["Data"].ToString();
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.SetRelazioneVC", conn);
            cmd.Parameters.AddWithValue("@IDRelazioneVC", IDRelazioneVC.ToString());
            cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetRelazioneVC(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneVC;
    }

    //----------------------------------------------------------------------------+
    //                  GetRevisioneAssociataFromRelazioneVCFile                  |
    //----------------------------------------------------------------------------+
    public string GetRevisioneAssociataFromRelazioneVCFile(string FileRelazioneVC)
    {
      string FileRevisione = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@FileData='" + FileRelazioneVC.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetRevisioni(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileRevisione = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileRevisione;
    }

    //----------------------------------------------------------------------------+
    //                  GetBilancioAssociatoFromRelazioneVCFile                   |
    //----------------------------------------------------------------------------+
    public string GetBilancioAssociatoFromRelazioneVCFile(string FileRelazioneVC)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@FileData='" + FileRelazioneVC.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                GetBilancioTreeAssociatoFromRelazioneVCFile                 |
    //----------------------------------------------------------------------------+
    public string GetBilancioTreeAssociatoFromRelazioneVCFile(string FileRelazioneVC)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@FileData='" + FileRelazioneVC.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = App.AppDataDataFolder + "\\" + item["File"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                 GetBilancioIDAssociatoFromRelazioneVCFile                  |
    //----------------------------------------------------------------------------+
    public string GetBilancioIDAssociatoFromRelazioneVCFile(string FileRelazioneVC)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@FileData='" + FileRelazioneVC.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = item["ID"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                 GetAllRelazioneVCAssociataFromBilancioFile                 |
    //----------------------------------------------------------------------------+
    public Hashtable GetAllRelazioneVCAssociataFromBilancioFile(string FileBilancio)
    {
      Hashtable RelazioneVC = null;
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@FileData='" + FileBilancio.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetRelazioniVC(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              RelazioneVC = item;
              break;
            }
          }
        }
      }
      Close();
      return RelazioneVC;
    }

    //----------------------------------------------------------------------------+
    //                 GetAllBilancioAssociatoFromRelazioneVCFile                 |
    //----------------------------------------------------------------------------+
    public Hashtable GetAllBilancioAssociatoFromRelazioneVCFile(string FileRelazioneVC)
    {
      Hashtable Bilancio = null;
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@FileData='" + FileRelazioneVC.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              Bilancio = item;
              break;
            }
          }
        }
      }
      Close();
      return Bilancio;
    }

#endregion //------------------------------------------------------ RelazioneVC

#region RelazioneBC

    //----------------------------------------------------------------------------+
    //                            GetRelazioniBCCount                             |
    //----------------------------------------------------------------------------+
    public int GetRelazioniBCCount()
    {
      int result = 0;
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC");
        if (xNodes != null)
        {
          result = xNodes.Count;
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                               GetRelazioniBC                               |
    //----------------------------------------------------------------------------+
    public ArrayList GetRelazioniBC(string IDCliente)
    {
      ArrayList results = new ArrayList();
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC[@Cliente='" + IDCliente + "']");
        foreach (XmlNode node in xNodes)
        {
          Hashtable result = new Hashtable();
          foreach (XmlAttribute item in node.Attributes)
          {
            result.Add(item.Name, item.Value);
          }
          results.Add(result);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return results;
    }

    //----------------------------------------------------------------------------+
    //                               GetRelazioneBC                               |
    //----------------------------------------------------------------------------+
    public Hashtable GetRelazioneBC(string IDRelazioneBC)
    {
      Hashtable result = new Hashtable();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@ID='" + IDRelazioneBC + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                         GetRelazioneBCFromFileData                         |
    //----------------------------------------------------------------------------+
    public Hashtable GetRelazioneBCFromFileData(string FileSessione)
    {
      Hashtable result = new Hashtable();
      FileSessione = FileSessione.Split('\\').Last();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@FileData='" + FileSessione + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                               AddRelazioneBC                               |
    //----------------------------------------------------------------------------+
    public string AddRelazioneBC_old(XmlNode node)
    {
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<RELAZIONIBC LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIBC");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
        xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC");
      }
      if (xNode.Attributes["LastID"].Value == "")
      {
        xNode.Attributes["LastID"].Value = "0";
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      XmlNode xtmp = document.ImportNode(node, true);
      xNode.AppendChild(xtmp);
      xNode.Attributes["LastID"].Value = ID;
      Save();
      Close();
      return ID;
    }
    public string AddRelazioneBC(XmlNode node)
    {
#if (!DBG_TEST)
      return AddRelazioneBC_old(node);
#endif
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<RELAZIONIBC LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIBC");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
        xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC");
      }
      if (xNode.Attributes["LastID"].Value == "")
      {
        xNode.Attributes["LastID"].Value = "0";
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetRelazioneBC", conn);
        cmd.Parameters.AddWithValue("@IDRelazioneBC", ID);
        cmd.Parameters.AddWithValue("@rec", node.OuterXml);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "AddRelazioneBC(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      Save();
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      Close();
      return ID;
    }

    //----------------------------------------------------------------------------+
    //                             DeleteRelazioneBC                              |
    //----------------------------------------------------------------------------+
    public void DeleteRelazioneBC_old(int IDRelazioneBC)
    {
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@ID='" + IDRelazioneBC.ToString() + "']");
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        xNode.ParentNode.RemoveChild(xNode);
        Save();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDRelazioneBC + "'][@Tree='" + (Convert.ToInt32(App.TipoFile.RelazioneBC)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
          node.ParentNode.RemoveChild(node);
        }
        xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
    }
    public void DeleteRelazioneBC(int IDRelazioneBC, string idcliente)
        {

      try
      {
                cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.RelazioneBC)).ToString(), idcliente);
                cBusinessObjects.DeleteSessione("RelazioneBC",IDRelazioneBC, idcliente);
                Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@ID='" + IDRelazioneBC.ToString() + "']");
        if (App.m_xmlCache.Contains(xNode.Attributes["File"].Value)) App.m_xmlCache.Remove(xNode.Attributes["File"].Value);
        if (App.m_xmlCache.Contains(xNode.Attributes["FileData"].Value)) App.m_xmlCache.Remove(xNode.Attributes["FileData"].Value);
        Close();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes(
          "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDRelazioneBC +
          "'][@Tree='" + (Convert.ToInt32(App.TipoFile.RelazioneBC)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.DeleteRelazioneBC", conn);
          cmd.Parameters.AddWithValue("@IDRelazioneBC", IDRelazioneBC.ToString());
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
                            cBusinessObjects.hide_workinprogress();
                            string msg = "DeleteRelazioneBC(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
      }
      catch (Exception ex)
      {
        string log = ex.Message;
                cBusinessObjects.hide_workinprogress();
                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
            cBusinessObjects.hide_workinprogress();
        }

    //----------------------------------------------------------------------------+
    //                          CheckDoppio_RelazioneBC                           |
    //----------------------------------------------------------------------------+
    public bool CheckDoppio_RelazioneBC(int ID, int IDCliente, string Data)
    {
      Open();
      XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC");
      foreach (XmlNode node in xNodes)
      {
        if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data)
        {
          Close();
          return false;
        }
      }
      Close();
      return true;
    }

    //----------------------------------------------------------------------------+
    //                          SetRelazioneBCIntermedio                          |
    //----------------------------------------------------------------------------+
    public int SetRelazioneBCIntermedio_old(Hashtable values, int IDCliente, string dal, string al)
    {
      int IDRelazioneBC = -1;
      try
      {
        Open();
        XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIBC");
        if (root == null)
        {
          XmlNode xroot = document.SelectSingleNode("/ROOT");
          string xmla = "<RELAZIONIBC LastID=\"0\" />";
          XmlDocument doctmpa = new XmlDocument();
          doctmpa.LoadXml(xmla);
          XmlNode tmpNodea = doctmpa.SelectSingleNode("/RELAZIONIBC");
          XmlNode xxtmp = document.ImportNode(tmpNodea, true);
          xroot.AppendChild(xxtmp);
          root = document.SelectSingleNode("/ROOT/RELAZIONIBC");
        }
        IDRelazioneBC = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
        string lastindex = IDRelazioneBC.ToString();
        //Template TREE
        FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneBC);
        string estensione = "." + App.AppTemplateTreeRelazioneBC.Split('.').Last();
        string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
        FileInfo fnewtree = new FileInfo(newNametree);
        while (fnewtree.Exists)
        {
          newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          fnewtree = new FileInfo(newNametree);
        }
        fitree.CopyTo(newNametree);
        newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");
        //Template Dati
        FileInfo fidati = new FileInfo(App.AppTemplateDataRelazioneBC);
        string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
        FileInfo fnewdati = new FileInfo(newNamedati);
        while (fnewdati.Exists)
        {
          newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          fnewdati = new FileInfo(newNamedati);
        }
        fidati.CopyTo(newNamedati);
        newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
        XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
        string xml = "<RELAZIONEBC ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONEBC");
        XmlNode cliente = document.ImportNode(tmpNode, true);
        root.AppendChild(cliente);
        root.Attributes["LastID"].Value = lastindex;
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneBC;
    }
    public int SetRelazioneBCIntermedio(Hashtable values, int IDCliente, string dal, string al)
    {
#if (!DBG_TEST)
      return SetRelazioneBCIntermedio_old(values, IDCliente, dal, al);
#endif
      int IDRelazioneBC = -1;
      string newNametree, newNamedati;
      try
      {
        Open();
        XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIBC");
        if (root == null)
        {
          XmlNode xroot = document.SelectSingleNode("/ROOT");
          string xmla = "<RELAZIONIBC LastID=\"0\" />";
          XmlDocument doctmpa = new XmlDocument();
          doctmpa.LoadXml(xmla);
          XmlNode tmpNodea = doctmpa.SelectSingleNode("/RELAZIONIBC");
          XmlNode xxtmp = document.ImportNode(tmpNodea, true);
          xroot.AppendChild(xxtmp);
          root = document.SelectSingleNode("/ROOT/RELAZIONIBC");
        }
        IDRelazioneBC = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
        string lastindex = IDRelazioneBC.ToString();
        newNametree = App.AppTemplateTreeRelazioneBC; newNametree = newNametree.Split('\\').Last();
        newNamedati = App.AppTemplateDataRelazioneBC; newNamedati = newNamedati.Split('\\').Last();
        XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
        string xml = "<RELAZIONEBC ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente +
          "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
          "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
          "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value +
          "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.NewRelazioneBC", conn);
          cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SetRelazioneBCIntermedio(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneBC;
    }

    //----------------------------------------------------------------------------+
    //                               SetRelazioneBC                               |
    //----------------------------------------------------------------------------+
    public int SetRelazioneBC_old(Hashtable values, int IDRelazioneBC, int IDCliente)
    {
      try
      {
        Open();
        if (IDRelazioneBC == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIBC");
          if (root == null)
          {
            XmlNode xroot = document.SelectSingleNode("/ROOT");
            string xmla = "<RELAZIONIBC LastID=\"0\" />";
            XmlDocument doctmpa = new XmlDocument();
            doctmpa.LoadXml(xmla);
            XmlNode tmpNodea = doctmpa.SelectSingleNode("/RELAZIONIBC");
            XmlNode xxtmp = document.ImportNode(tmpNodea, true);
            xroot.AppendChild(xxtmp);
            root = document.SelectSingleNode("/ROOT/RELAZIONIBC");
          }
          IDRelazioneBC = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDRelazioneBC.ToString();
          //Template TREE
          FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneBC);
          string estensione = "." + App.AppTemplateTreeRelazioneBC.Split('.').Last();
          string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewtree = new FileInfo(newNametree);
          while (fnewtree.Exists)
          {
            newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewtree = new FileInfo(newNametree);
          }
          fitree.CopyTo(newNametree);
          newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");
          //Template Dati
          FileInfo fidati = new FileInfo(App.AppTemplateDataRelazioneBC);
          string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewdati = new FileInfo(newNamedati);
          while (fnewdati.Exists)
          {
            newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewdati = new FileInfo(newNamedati);
          }
          fidati.CopyTo(newNamedati);
          newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
          XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
          string xml = "<RELAZIONEBC ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONEBC");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          root.AppendChild(cliente);
          root.Attributes["LastID"].Value = lastindex;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@ID='" + IDRelazioneBC.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          xNode.Attributes["Data"].Value = values["Data"].ToString();
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneBC;
    }
    public int SetRelazioneBC(Hashtable values, int IDRelazioneBC, int IDCliente)
    {
#if (!DBG_TEST)
      return SetRelazioneBC_old(values, IDRelazioneBC, IDCliente);
#endif
      string newNametree, newNamedati;
      try
      {
        Open();
        if (IDRelazioneBC == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIBC");
          if (root == null)
          {
            XmlNode xroot = document.SelectSingleNode("/ROOT");
            string xmla = "<RELAZIONIBC LastID=\"0\" />";
            XmlDocument doctmpa = new XmlDocument();
            doctmpa.LoadXml(xmla);
            XmlNode tmpNodea = doctmpa.SelectSingleNode("/RELAZIONIBC");
            XmlNode xxtmp = document.ImportNode(tmpNodea, true);
            xroot.AppendChild(xxtmp);
            root = document.SelectSingleNode("/ROOT/RELAZIONIBC");
          }
          IDRelazioneBC = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDRelazioneBC.ToString();
          newNametree = App.AppTemplateTreeRelazioneBC; newNametree = newNametree.Split('\\').Last();
          newNamedati = App.AppTemplateDataRelazioneBC; newNamedati = newNamedati.Split('\\').Last();
          XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
          string xml = "<RELAZIONEBC ID=\"" + lastindex + "\" Cliente=\"" + IDCliente +
            "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree +
            "\" FileData=\"" + newNamedati +
            "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value +
            "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value +
            "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.NewRelazioneBC", conn);
            cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetRelazioneBC(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@ID='" + IDRelazioneBC.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          xNode.Attributes["Data"].Value = values["Data"].ToString();
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.SetRelazioneBC", conn);
            cmd.Parameters.AddWithValue("@IDRelazioneBC", IDRelazioneBC.ToString());
            cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetRelazioneBC(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneBC;
    }

    //----------------------------------------------------------------------------+
    //                  GetRevisioneAssociataFromRelazioneBCFile                  |
    //----------------------------------------------------------------------------+
    public string GetRevisioneAssociataFromRelazioneBCFile(string FileRelazioneV)
    {
      string FileRevisione = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@FileData='" + FileRelazioneV.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetRevisioni(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileRevisione = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileRevisione;
    }

    //----------------------------------------------------------------------------+
    //                  GetBilancioAssociatoFromRelazioneBCFile                   |
    //----------------------------------------------------------------------------+
    public string GetBilancioAssociatoFromRelazioneBCFile(string FileRelazioneBC)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@FileData='" + FileRelazioneBC.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                GetBilancioTreeAssociatoFromRelazioneBCFile                 |
    //----------------------------------------------------------------------------+
    public string GetBilancioTreeAssociatoFromRelazioneBCFile(string FileRelazioneBC)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@FileData='" + FileRelazioneBC.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = App.AppDataDataFolder + "\\" + item["File"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                 GetBilancioIDAssociatoFromRelazioneBCFile                  |
    //----------------------------------------------------------------------------+
    public string GetBilancioIDAssociatoFromRelazioneBCFile(string FileRelazioneBC)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@FileData='" + FileRelazioneBC.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = item["ID"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                 GetAllRelazioneBCAssociataFromBilancioFile                 |
    //----------------------------------------------------------------------------+
    public Hashtable GetAllRelazioneBCAssociataFromBilancioFile(string FileBilancio)
    {
      Hashtable RelazioneBC = null;
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@FileData='" + FileBilancio.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetRelazioniBC(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              RelazioneBC = item;
              break;
            }
          }
        }
      }
      Close();
      return RelazioneBC;
    }

    //----------------------------------------------------------------------------+
    //                 GetAllBilancioAssociatoFromRelazioneBCFile                 |
    //----------------------------------------------------------------------------+
    public Hashtable GetAllBilancioAssociatoFromRelazioneBCFile(string FileRelazioneBC)
    {
      Hashtable Bilancio = null;
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@FileData='" + FileRelazioneBC.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              Bilancio = item;
              break;
            }
          }
        }
      }
      Close();
      return Bilancio;
    }

#endregion //------------------------------------------------------ RelazioneBC

#region RelazioneBVV

    //----------------------------------------------------------------------------+
    //                            GetRelazioniBVCount                             |
    //----------------------------------------------------------------------------+
    public int GetRelazioniBVCount()
    {
      int result = 0;
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIBV/RELAZIONEBV");
        if (xNodes != null)
        {
          result = xNodes.Count;
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                               GetRelazioniBV                               |
    //----------------------------------------------------------------------------+
    public ArrayList GetRelazioniBV(string IDCliente)
    {
      ArrayList results = new ArrayList();
      try
      {
        Open();
        XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIBV/RELAZIONEBV[@Cliente='" + IDCliente + "']");
        foreach (XmlNode node in xNodes)
        {
          Hashtable result = new Hashtable();
          foreach (XmlAttribute item in node.Attributes)
          {
            result.Add(item.Name, item.Value);
          }
          results.Add(result);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return results;
    }

    //----------------------------------------------------------------------------+
    //                               GetRelazioneBV                               |
    //----------------------------------------------------------------------------+
    public Hashtable GetRelazioneBV(string IDRelazioneBV)
    {
      Hashtable result = new Hashtable();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBV/RELAZIONEBV[@ID='" + IDRelazioneBV + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                         GetRelazioneBVFromFileData                         |
    //----------------------------------------------------------------------------+
    public Hashtable GetRelazioneBVFromFileData(string FileSessione)
    {
      Hashtable result = new Hashtable();
      FileSessione = FileSessione.Split('\\').Last();
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBV/RELAZIONEBV[@FileData='" + FileSessione + "']");
        foreach (XmlAttribute item in xNode.Attributes)
        {
          result.Add(item.Name, item.Value);
        }
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
      return result;
    }

    //----------------------------------------------------------------------------+
    //                               AddRelazioneBV                               |
    //----------------------------------------------------------------------------+
    public string AddRelazioneBV_old(XmlNode node)
    {
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBV");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<RELAZIONIBV LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIBV");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
        xNode = document.SelectSingleNode("/ROOT/RELAZIONIBV");
      }
      if (xNode.Attributes["LastID"].Value == "")
      {
        xNode.Attributes["LastID"].Value = "0";
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      XmlNode xtmp = document.ImportNode(node, true);
      xNode.AppendChild(xtmp);
      xNode.Attributes["LastID"].Value = ID;
      Save();
      Close();
      return ID;
    }
    public string AddRelazioneBV(XmlNode node)
    {
#if (!DBG_TEST)
      return AddRelazioneBV_old(node);
#endif
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBV");
      if (xNode == null)
      {
        XmlNode xroot = document.SelectSingleNode("/ROOT");
        string xml = "<RELAZIONIBV LastID=\"0\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIBV");
        XmlNode xxtmp = document.ImportNode(tmpNode, true);
        xroot.AppendChild(xxtmp);
        xNode = document.SelectSingleNode("/ROOT/RELAZIONIBV");
      }
      if (xNode.Attributes["LastID"].Value == "")
      {
        xNode.Attributes["LastID"].Value = "0";
      }
      string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();
      node.Attributes["ID"].Value = ID;
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("mf.SetRelazioneBV", conn);
        cmd.Parameters.AddWithValue("@IDRelazioneBV", ID);
        cmd.Parameters.AddWithValue("@rec", node.OuterXml);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "AddRelazioneBV(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      Save();
      if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
      Close();
      return ID;
    }

    //----------------------------------------------------------------------------+
    //                             DeleteRelazioneBV                              |
    //----------------------------------------------------------------------------+
    public void DeleteRelazioneBV_old(int IDRelazioneBV)
    {
      try
      {
        Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBV/RELAZIONEBV[@ID='" + IDRelazioneBV.ToString() + "']");
        FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
        if (fi.Exists) fi.Delete();
        FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
        if (fd.Exists) fd.Delete();
        xNode.ParentNode.RemoveChild(xNode);
        Save();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDRelazioneBV + "'][@Tree='" + (Convert.ToInt32(App.TipoFile.RelazioneBV)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
          node.ParentNode.RemoveChild(node);
        }
        xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
    }
    public void DeleteRelazioneBV(int IDRelazioneBV, string idcliente)
        {

      try
      {
                cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.RelazioneBV)).ToString(), idcliente);
                cBusinessObjects.DeleteSessione("RelazioneBV",IDRelazioneBV, idcliente);
                Open();
        XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBV/RELAZIONEBV[@ID='" + IDRelazioneBV.ToString() + "']");
        if (App.m_xmlCache.Contains(xNode.Attributes["File"].Value)) App.m_xmlCache.Remove(xNode.Attributes["File"].Value);
        if (App.m_xmlCache.Contains(xNode.Attributes["FileData"].Value)) App.m_xmlCache.Remove(xNode.Attributes["FileData"].Value);
        Close();
        //cancello allegati
        XmlManager xdoc = new XmlManager();
        xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
        XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
        XmlNodeList xNodes = xdoc_doc.SelectNodes(
          "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDRelazioneBV + "'][@Tree='" +
          (Convert.ToInt32(App.TipoFile.RelazioneBV)).ToString() + "']");
        foreach (XmlNode node in xNodes)
        {
          FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
          if (fis.Exists) fis.Delete();
        }
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.DeleteRelazioneBV", conn);
          cmd.Parameters.AddWithValue("@IDRelazioneBV", IDRelazioneBV.ToString());
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
                            cBusinessObjects.hide_workinprogress();
                            string msg = "DeleteRelazioneBV(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");
      }
      catch (Exception ex)
      {
        string log = ex.Message;
                cBusinessObjects.hide_workinprogress();
                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
      }
            cBusinessObjects.hide_workinprogress();
        }

    //----------------------------------------------------------------------------+
    //                          CheckDoppio_RelazioneBV                           |
    //----------------------------------------------------------------------------+
    public bool CheckDoppio_RelazioneBV(int ID, int IDCliente, string Data)
    {
      Open();
      XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIBV/RELAZIONEBV");
      foreach (XmlNode node in xNodes)
      {
        if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data)
        {
          Close();
          return false;
        }
      }
      Close();
      return true;
    }

    //----------------------------------------------------------------------------+
    //                          SetRelazioneBVIntermedio                          |
    //----------------------------------------------------------------------------+
    public int SetRelazioneBVIntermedio_old(Hashtable values, int IDCliente, string dal, string al)
    {
      int IDRelazioneBV = -1;
      try
      {
        Open();
        XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIBV");
        if (root == null)
        {
          XmlNode xroot = document.SelectSingleNode("/ROOT");
          string xmla = "<RELAZIONIBV LastID=\"0\" />";
          XmlDocument doctmpa = new XmlDocument();
          doctmpa.LoadXml(xmla);
          XmlNode tmpNodea = doctmpa.SelectSingleNode("/RELAZIONIBV");
          XmlNode xxtmp = document.ImportNode(tmpNodea, true);
          xroot.AppendChild(xxtmp);
          root = document.SelectSingleNode("/ROOT/RELAZIONIBV");
        }
        IDRelazioneBV = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
        string lastindex = IDRelazioneBV.ToString();
        //Template TREE
        FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneBV);
        string estensione = "." + App.AppTemplateTreeRelazioneBV.Split('.').Last();
        string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
        FileInfo fnewtree = new FileInfo(newNametree);
        while (fnewtree.Exists)
        {
          newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          fnewtree = new FileInfo(newNametree);
        }
        fitree.CopyTo(newNametree);
        newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");
        //Template Dati
        FileInfo fidati = new FileInfo(App.AppTemplateDataRelazioneBV);
        string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
        FileInfo fnewdati = new FileInfo(newNamedati);
        while (fnewdati.Exists)
        {
          newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          fnewdati = new FileInfo(newNamedati);
        }
        fidati.CopyTo(newNamedati);
        newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
        XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
        string xml = "<RELAZIONEBV ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONEBV");
        XmlNode cliente = document.ImportNode(tmpNode, true);
        root.AppendChild(cliente);
        root.Attributes["LastID"].Value = lastindex;
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneBV;
    }
    public int SetRelazioneBVIntermedio(Hashtable values, int IDCliente, string dal, string al)
    {
#if (!DBG_TEST)
      return SetRelazioneBVIntermedio_old(values, IDCliente, dal, al);
#endif
      int IDRelazioneBV = -1;
      string newNametree, newNamedati;
      try
      {
        Open();
        XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIBV");
        if (root == null)
        {
          XmlNode xroot = document.SelectSingleNode("/ROOT");
          string xmla = "<RELAZIONIBV LastID=\"0\" />";
          XmlDocument doctmpa = new XmlDocument();
          doctmpa.LoadXml(xmla);
          XmlNode tmpNodea = doctmpa.SelectSingleNode("/RELAZIONIBV");
          XmlNode xxtmp = document.ImportNode(tmpNodea, true);
          xroot.AppendChild(xxtmp);
          root = document.SelectSingleNode("/ROOT/RELAZIONIBV");
        }
        IDRelazioneBV = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
        string lastindex = IDRelazioneBV.ToString();
        newNametree = App.AppTemplateTreeRelazioneBV; newNametree = newNametree.Split('\\').Last();
        newNamedati = App.AppTemplateDataRelazioneBV; newNamedati = newNamedati.Split('\\').Last();
        XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
        string xml = "<RELAZIONEBV ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente +
          "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() +
          "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati +
          "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
          "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value +
          "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
        XmlDocument doctmp = new XmlDocument();
        doctmp.LoadXml(xml);
        using (SqlConnection conn = new SqlConnection(App.connString))
        {
          conn.Open();
          SqlCommand cmd = new SqlCommand("mf.NewRelazioneBV", conn);
          cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = App.m_CommandTimeout;
          try { cmd.ExecuteNonQuery(); }
          catch (Exception ex)
          {
            if (!App.m_bNoExceptionMsg)
            {
              string msg = "SetRelazioneBVIntermedio(): errore\n" + ex.Message;
              MessageBox.Show(msg);
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneBV;
    }

    //----------------------------------------------------------------------------+
    //                               SetRelazioneBV                               |
    //----------------------------------------------------------------------------+
    public int SetRelazioneBV_old(Hashtable values, int IDRelazioneBV, int IDCliente)
    {
      try
      {
        Open();
        if (IDRelazioneBV == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIBV");
          if (root == null)
          {
            XmlNode xroot = document.SelectSingleNode("/ROOT");
            string xmla = "<RELAZIONIBV LastID=\"0\" />";
            XmlDocument doctmpa = new XmlDocument();
            doctmpa.LoadXml(xmla);
            XmlNode tmpNodea = doctmpa.SelectSingleNode("/RELAZIONIBV");
            XmlNode xxtmp = document.ImportNode(tmpNodea, true);
            xroot.AppendChild(xxtmp);
            root = document.SelectSingleNode("/ROOT/RELAZIONIBV");
          }
          IDRelazioneBV = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDRelazioneBV.ToString();
          //Template TREE
          FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneBV);
          string estensione = "." + App.AppTemplateTreeRelazioneBV.Split('.').Last();
          string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewtree = new FileInfo(newNametree);
          while (fnewtree.Exists)
          {
            newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewtree = new FileInfo(newNametree);
          }
          fitree.CopyTo(newNametree);
          newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");
          //Template Dati
          FileInfo fidati = new FileInfo(App.AppTemplateDataRelazioneBV);
          string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
          FileInfo fnewdati = new FileInfo(newNamedati);
          while (fnewdati.Exists)
          {
            newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
            fnewdati = new FileInfo(newNamedati);
          }
          fidati.CopyTo(newNamedati);
          newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
          XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
          string xml = "<RELAZIONEBV ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONEBV");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          root.AppendChild(cliente);
          root.Attributes["LastID"].Value = lastindex;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBV/RELAZIONEBV[@ID='" + IDRelazioneBV.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          xNode.Attributes["Data"].Value = values["Data"].ToString();
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneBV;
    }
    public int SetRelazioneBV(Hashtable values, int IDRelazioneBV, int IDCliente)
    {
#if (!DBG_TEST)
      return SetRelazioneBV_old(values, IDRelazioneBV, IDCliente);
#endif
      string newNametree, newNamedati;
      try
      {
        Open();
        if (IDRelazioneBV == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIBV");
          if (root == null)
          {
            XmlNode xroot = document.SelectSingleNode("/ROOT");
            string xmla = "<RELAZIONIBV LastID=\"0\" />";
            XmlDocument doctmpa = new XmlDocument();
            doctmpa.LoadXml(xmla);
            XmlNode tmpNodea = doctmpa.SelectSingleNode("/RELAZIONIBV");
            XmlNode xxtmp = document.ImportNode(tmpNodea, true);
            xroot.AppendChild(xxtmp);
            root = document.SelectSingleNode("/ROOT/RELAZIONIBV");
          }
          IDRelazioneBV = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string lastindex = IDRelazioneBV.ToString();
          newNametree = App.AppTemplateTreeRelazioneBV; newNametree = newNametree.Split('\\').Last();
          newNamedati = App.AppTemplateDataRelazioneBV; newNamedati = newNamedati.Split('\\').Last();
          XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");
          string xml = "<RELAZIONEBV ID=\"" + lastindex + "\" Cliente=\"" + IDCliente +
            "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() +
            "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati +
            "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") +
            "\" Data=\"" + values["Data"].ToString() +
            "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value +
            "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value +
            "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.NewRelazioneBV", conn);
            cmd.Parameters.AddWithValue("@rec", doctmp.InnerXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetRelazioneBV(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBV/RELAZIONEBV[@ID='" + IDRelazioneBV.ToString() + "']");
          xNode.Attributes["Note"].Value = values["Note"].ToString();
          xNode.Attributes["Data"].Value = values["Data"].ToString();
          using (SqlConnection conn = new SqlConnection(App.connString))
          {
            conn.Open();
            SqlCommand cmd = new SqlCommand("mf.SetRelazioneBV", conn);
            cmd.Parameters.AddWithValue("@IDRelazioneBV", IDRelazioneBV.ToString());
            cmd.Parameters.AddWithValue("@rec", xNode.OuterXml);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = App.m_CommandTimeout;
            try { cmd.ExecuteNonQuery(); }
            catch (Exception ex)
            {
              if (!App.m_bNoExceptionMsg)
              {
                string msg = "SetRelazioneBV(): errore\n" + ex.Message;
                MessageBox.Show(msg);
              }
            }
          }
        }
        Save();
        if (App.m_xmlCache.Contains("RevisoftApp.rmdf")) App.m_xmlCache.Remove("RevisoftApp.rmdf");
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDRelazioneBV;
    }

    //----------------------------------------------------------------------------+
    //                  GetRevisioneAssociataFromRelazioneBVFile                  |
    //----------------------------------------------------------------------------+
    public string GetRevisioneAssociataFromRelazioneBVFile(string FileRelazioneV)
    {
      string FileRevisione = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBV/RELAZIONEBV[@FileData='" + FileRelazioneV.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetRevisioni(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileRevisione = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileRevisione;
    }

    //----------------------------------------------------------------------------+
    //                  GetBilancioAssociatoFromRelazioneBVFile                   |
    //----------------------------------------------------------------------------+
    public string GetBilancioAssociatoFromRelazioneBVFile(string FileRelazioneBV)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBV/RELAZIONEBV[@FileData='" + FileRelazioneBV.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                GetBilancioTreeAssociatoFromRelazioneBVFile                 |
    //----------------------------------------------------------------------------+
    public string GetBilancioTreeAssociatoFromRelazioneBVFile(string FileRelazioneBV)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBV/RELAZIONEBV[@FileData='" + FileRelazioneBV.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = App.AppDataDataFolder + "\\" + item["File"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                 GetBilancioIDAssociatoFromRelazioneBVFile                  |
    //----------------------------------------------------------------------------+
    public string GetBilancioIDAssociatoFromRelazioneBVFile(string FileRelazioneBV)
    {
      string FileBilancio = "";
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBV/RELAZIONEBV[@FileData='" + FileRelazioneBV.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              FileBilancio = item["ID"].ToString();
              break;
            }
          }
        }
      }
      Close();
      return FileBilancio;
    }

    //----------------------------------------------------------------------------+
    //                 GetAllRelazioneBVAssociataFromBilancioFile                 |
    //----------------------------------------------------------------------------+
    public Hashtable GetAllRelazioneBVAssociataFromBilancioFile(string FileBilancio)
    {
      Hashtable RelazioneBV = null;
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@FileData='" + FileBilancio.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetRelazioniBV(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              RelazioneBV = item;
              break;
            }
          }
        }
      }
      Close();
      return RelazioneBV;
    }

    //----------------------------------------------------------------------------+
    //                 GetAllBilancioAssociatoFromRelazioneBVFile                 |
    //----------------------------------------------------------------------------+
    public Hashtable GetAllBilancioAssociatoFromRelazioneBVFile(string FileRelazioneBV)
    {
      Hashtable Bilancio = null;
      Open();
      XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBV/RELAZIONEBV[@FileData='" + FileRelazioneBV.Split('\\').Last() + "']");
      if (xNode != null)
      {
        if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
        {
          ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);
          foreach (Hashtable item in al)
          {
            if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
            {
              Bilancio = item;
              break;
            }
          }
        }
      }
      Close();
      return Bilancio;
    }

#endregion //----------------------------------------------------- RelazioneBVV

#region DOCUMENTI

    //----------------------------------------------------------------------------+
    //                         CheckAndNormalizeDocuments                         |
    //----------------------------------------------------------------------------+
    public void CheckAndNormalizeDocuments()
    {
      //cancello allegati
      XmlManager xdoc = new XmlManager();
      xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
      XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);
      XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTO");
      foreach (XmlNode node in xNodes)
      {
        FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
        if (!fis.Exists)
        {
          node.ParentNode.RemoveChild(node);
          continue;
        }
        if (node.Attributes["ClienteExtended"] == null && node.Attributes["Cliente"] != null)
        {
          XmlAttribute attr = node.OwnerDocument.CreateAttribute("ClienteExtended");
          try
          {
            attr.Value = wDocumenti.GetClienteString(node.Attributes["Cliente"].Value);
          }
          catch (Exception ex)
          {
            attr.Value = "";
            string log = ex.Message;
          }
          node.Attributes.Append(attr);
        }
        if (node.Attributes["SessioneExtended"] == null && node.Attributes["Sessione"] != null)
        {
          XmlAttribute attr = node.OwnerDocument.CreateAttribute("SessioneExtended");
          try
          {
            attr.Value = wDocumenti.GetSessioneString(node.Attributes["Tree"].Value, node.Attributes["Sessione"].Value);
          }
          catch (Exception ex)
          {
            attr.Value = "";
            string log = ex.Message;
          }
          node.Attributes.Append(attr);
        }
        if (node.Attributes["NodoExtended"] == null && node.Attributes["Tree"] != null && node.Attributes["Sessione"] != null && node.Attributes["Nodo"] != null)
        {
          XmlAttribute attr = node.OwnerDocument.CreateAttribute("NodoExtended");
          try
          {
            attr.Value = wDocumenti.GetNodeString(node.Attributes["Tree"].Value, node.Attributes["Sessione"].Value, node.Attributes["Nodo"].Value);
          }
          catch (Exception ex)
          {
            attr.Value = "";
            string log = ex.Message;
          }
          node.Attributes.Append(attr);
        }
      }
      xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);
      DirectoryInfo dis_lost = new DirectoryInfo(App.AppDocumentiFolder + "\\Lost");
      if (!dis_lost.Exists) dis_lost.Create();
      DirectoryInfo dis = new DirectoryInfo(App.AppDocumentiFolder);
      foreach (FileInfo item in dis.GetFiles())
      {
        XmlNodeList xNodeshere = xdoc_doc.SelectNodes("//DOCUMENTO[@File=\"" + item.Name + "\"]");
        if (xNodeshere.Count == 0)
        {
          item.MoveTo(App.AppDocumentiFolder + "\\Lost\\" + item.Name);
        }
      }
    }

#endregion //-------------------------------------------------------- DOCUMENTI

  } //-------------------------------------------------------- class MasterFile
} //--------------------------------------------- namespace RevisoftApplication

/*
// srcOld
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;
using System.Windows.Data;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace RevisoftApplication
{
    class MasterFile
    {
        private WindowGestioneMessaggi message = new WindowGestioneMessaggi();
		private XmlManager x = new XmlManager();					
        private XmlDocument document = new XmlDocument();
        private string file = string.Empty;

        public MasterFile()
        {
            App.ErrorLevel = App.ErrorTypes.Nessuno;
            file = App.AppMasterDataFile;
			x.TipoCodifica = XmlManager.TipologiaCodifica.Normale;

            //Open();

            //XmlNode root = document.SelectSingleNode( "/ROOT/CONCLUSIONI" );

            //if ( root == null )
            //{
            //    root = document.SelectSingleNode( "/ROOT" );

            //    string xml2 = "<CONCLUSIONI LastID=\"1\"/>";
            //    XmlDocument doctmp2 = new XmlDocument();
            //    doctmp2.LoadXml( xml2 );

            //    XmlNode tmpNode2 = doctmp2.SelectSingleNode( "/CONCLUSIONI" );
            //    XmlNode cliente2 = document.ImportNode( tmpNode2, true );

            //    root.AppendChild( cliente2 );

            //    root = document.SelectSingleNode( "/ROOT/CONCLUSIONI" );

            //    Save();
            //}
            
            //Close();
            
            //Open();

            //root = document.SelectSingleNode( "/ROOT/VIGILANZE" );

            //if ( root == null )
            //{
            //    root = document.SelectSingleNode( "/ROOT" );

            //    string xml2 = "<VIGILANZE LastID=\"1\"/>";
            //    XmlDocument doctmp2 = new XmlDocument();
            //    doctmp2.LoadXml( xml2 );

            //    XmlNode tmpNode2 = doctmp2.SelectSingleNode( "/VIGILANZE" );
            //    XmlNode cliente2 = document.ImportNode( tmpNode2, true );

            //    root.AppendChild( cliente2 );

            //    root = document.SelectSingleNode( "/ROOT/VIGILANZE" );

            //    Save();
            //}
            
            //Close();

            //Open();

            //root = document.SelectSingleNode( "/ROOT/FLUSSI" );

            //if ( root == null )
            //{
            //    root = document.SelectSingleNode( "/ROOT" );

            //    string xml2 = "<FLUSSI/>";
            //    XmlDocument doctmp2 = new XmlDocument();
            //    doctmp2.LoadXml( xml2 );

            //    XmlNode tmpNode2 = doctmp2.SelectSingleNode( "/FLUSSI" );
            //    XmlNode cliente2 = document.ImportNode( tmpNode2, true );

            //    root.AppendChild( cliente2 );

            //    root = document.SelectSingleNode( "/ROOT/FLUSSI" );

            //    Save();
            //}

            //Close();
        }


        private static MasterFile _instance = new MasterFile();

        public static MasterFile Create()
        {
            return _instance;
        }

        public static void ForceRecreate()
        {
            _instance = new MasterFile();
        }



#region Funzioni Base
        public void ResetMasterFile()
		{
			//try
			//{
			Open();

            //resetto contenuti
			XmlNodeList xNodes = document.SelectNodes("/ROOT/VERIFICHE/VERIFICA");

			foreach (XmlNode node in xNodes)
			{
				FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
				if (fi.Exists)
				{
					fi.Delete();
				}

				FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
				if (fd.Exists)
				{
					fd.Delete();
				}

				node.ParentNode.RemoveChild(node);
			}

			document.SelectSingleNode("/ROOT/VERIFICHE").Attributes["LastID"].Value = "0";

            xNodes = document.SelectNodes( "/ROOT/VIGILANZE/VIGILANZA" );

            foreach ( XmlNode node in xNodes )
            {
                FileInfo fi = new FileInfo( App.AppDataDataFolder + "\\" + node.Attributes["File"].Value );
                if ( fi.Exists )
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo( App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value );
                if ( fd.Exists )
                {
                    fd.Delete();
                }

                node.ParentNode.RemoveChild( node );
            }

            document.SelectSingleNode( "/ROOT/VIGILANZE" ).Attributes["LastID"].Value = "0";

            xNodes = document.SelectNodes("/ROOT/BILANCI/BILANCIO");

			foreach (XmlNode node in xNodes)
			{
				FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
				if (fi.Exists)
				{
					fi.Delete();
				}

				FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
				if (fd.Exists)
				{
					fd.Delete();
				}

				node.ParentNode.RemoveChild(node);
			}

			document.SelectSingleNode("/ROOT/BILANCI").Attributes["LastID"].Value = "0";

            xNodes = document.SelectNodes( "/ROOT/CONCLUSIONI/CONCLUSIONE" );

            foreach ( XmlNode node in xNodes )
            {
                FileInfo fi = new FileInfo( App.AppDataDataFolder + "\\" + node.Attributes["File"].Value );
                if ( fi.Exists )
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo( App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value );
                if ( fd.Exists )
                {
                    fd.Delete();
                }

                node.ParentNode.RemoveChild( node );
            }

            document.SelectSingleNode( "/ROOT/CONCLUSIONI" ).Attributes["LastID"].Value = "0";

            xNodes = document.SelectNodes("/ROOT/REVISIONI/REVISIONE");

			foreach (XmlNode node in xNodes)
			{
				FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
				if (fi.Exists)
				{
					fi.Delete();
				}

				FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
				if (fd.Exists)
				{
					fd.Delete();
				}

				node.ParentNode.RemoveChild(node);
			}

		    document.SelectSingleNode("/ROOT/REVISIONI").Attributes["LastID"].Value = "0";

            xNodes = document.SelectNodes("/ROOT/INCARICHI/INCARICO");

			foreach (XmlNode node in xNodes)
			{
				FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
				if (fi.Exists)
				{
					fi.Delete();
				}

				FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
				if (fd.Exists)
				{
					fd.Delete();
				}

				node.ParentNode.RemoveChild(node);
			}

			document.SelectSingleNode("/ROOT/INCARICHI").Attributes["LastID"].Value = "0";





            xNodes = document.SelectNodes("/ROOT/ISQCs/ISQC");

            foreach (XmlNode node in xNodes)
            {
                FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                if (fi.Exists)
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                if (fd.Exists)
                {
                    fd.Delete();
                }

                node.ParentNode.RemoveChild(node);
            }

            document.SelectSingleNode("/ROOT/ISQCs").Attributes["LastID"].Value = "0";




            XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIV");

            if (xNode == null)
            {
                XmlNode xroot = document.SelectSingleNode("/ROOT");

                string xml = "<RELAZIONIV LastID=\"0\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);
                XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIV");
                XmlNode xxtmp = document.ImportNode(tmpNode, true);
                xroot.AppendChild(xxtmp);
            }

            xNodes = document.SelectNodes( "/ROOT/RELAZIONIV/RELAZIONEV" );

            foreach ( XmlNode node in xNodes )
            {
                FileInfo fi = new FileInfo( App.AppDataDataFolder + "\\" + node.Attributes["File"].Value );
                if ( fi.Exists )
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo( App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value );
                if ( fd.Exists )
                {
                    fd.Delete();
                }

                node.ParentNode.RemoveChild( node );
            }

            document.SelectSingleNode( "/ROOT/RELAZIONIV" ).Attributes["LastID"].Value = "0";







            xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC");

            if (xNode == null)
            {
                XmlNode xroot = document.SelectSingleNode("/ROOT");

                string xml = "<RELAZIONIVC LastID=\"0\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);
                XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIVC");
                XmlNode xxtmp = document.ImportNode(tmpNode, true);
                xroot.AppendChild(xxtmp);
            }

            xNodes = document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC");

            foreach (XmlNode node in xNodes)
            {
                FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                if (fi.Exists)
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                if (fd.Exists)
                {
                    fd.Delete();
                }

                node.ParentNode.RemoveChild(node);
            }

            document.SelectSingleNode("/ROOT/RELAZIONIVC").Attributes["LastID"].Value = "0";





            xNode = document.SelectSingleNode("/ROOT/RELAZIONIB");

            if (xNode == null)
            {
                XmlNode xroot = document.SelectSingleNode("/ROOT");

                string xml = "<RELAZIONIB LastID=\"0\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);
                XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIB");
                XmlNode xxtmp = document.ImportNode(tmpNode, true);
                xroot.AppendChild(xxtmp);
            }

            xNodes = document.SelectNodes( "/ROOT/RELAZIONIB/RELAZIONEB" );

            foreach ( XmlNode node in xNodes )
            {
                FileInfo fi = new FileInfo( App.AppDataDataFolder + "\\" + node.Attributes["File"].Value );
                if ( fi.Exists )
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo( App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value );
                if ( fd.Exists )
                {
                    fd.Delete();
                }

                node.ParentNode.RemoveChild( node );
            }

            document.SelectSingleNode( "/ROOT/RELAZIONIB" ).Attributes["LastID"].Value = "0";







            xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC");

            if (xNode == null)
            {
                XmlNode xroot = document.SelectSingleNode("/ROOT");

                string xml = "<RELAZIONIBC LastID=\"0\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);
                XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIBC");
                XmlNode xxtmp = document.ImportNode(tmpNode, true);
                xroot.AppendChild(xxtmp);
            }

            xNodes = document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC");

            foreach (XmlNode node in xNodes)
            {
                FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                if (fi.Exists)
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                if (fd.Exists)
                {
                    fd.Delete();
                }

                node.ParentNode.RemoveChild(node);
            }

            document.SelectSingleNode("/ROOT/RELAZIONIBC").Attributes["LastID"].Value = "0";







            xNode = document.SelectSingleNode("/ROOT/RELAZIONIBV");

            if (xNode == null)
            {
                XmlNode xroot = document.SelectSingleNode("/ROOT");

                string xml = "<RELAZIONIBV LastID=\"0\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);
                XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIBV");
                XmlNode xxtmp = document.ImportNode(tmpNode, true);
                xroot.AppendChild(xxtmp);
            }

            xNodes = document.SelectNodes( "/ROOT/RELAZIONIBV/RELAZIONEBV" );

            foreach ( XmlNode node in xNodes )
            {
                FileInfo fi = new FileInfo( App.AppDataDataFolder + "\\" + node.Attributes["File"].Value );
                if ( fi.Exists )
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo( App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value );
                if ( fd.Exists )
                {
                    fd.Delete();
                }

                node.ParentNode.RemoveChild( node );
            }

            document.SelectSingleNode( "/ROOT/RELAZIONIBV" ).Attributes["LastID"].Value = "0";
            
            xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVERIFICHE");

            if (xNode == null)
            {
                XmlNode xroot = document.SelectSingleNode("/ROOT");

                string xml = "<PIANIFICAZIONIVERIFICHE LastID=\"0\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);
                XmlNode tmpNode = doctmp.SelectSingleNode("/PIANIFICAZIONIVERIFICHE");
                XmlNode xxtmp = document.ImportNode(tmpNode, true);
                xroot.AppendChild(xxtmp);
            }

            xNodes = document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA");
            foreach (XmlNode node in xNodes)
            {
                FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                if (fi.Exists)
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                if (fd.Exists)
                {
                    fd.Delete();
                }
                node.ParentNode.RemoveChild(node);
            }

            document.SelectSingleNode("/ROOT/PIANIFICAZIONIVERIFICHE").Attributes["LastID"].Value = "0";

            xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVIGILANZE");

            if (xNode == null)
            {
                XmlNode xroot = document.SelectSingleNode("/ROOT");

                string xml = "<PIANIFICAZIONIVIGILANZE LastID=\"0\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);
                XmlNode tmpNode = doctmp.SelectSingleNode("/PIANIFICAZIONIVIGILANZE");
                XmlNode xxtmp = document.ImportNode(tmpNode, true);
                xroot.AppendChild(xxtmp);
            }

            xNodes = document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA");
            foreach (XmlNode node in xNodes)
            {
                FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                if (fi.Exists)
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                if (fd.Exists)
                {
                    fd.Delete();
                }
                node.ParentNode.RemoveChild(node);
            }
            document.SelectSingleNode("/ROOT/PIANIFICAZIONIVIGILANZE").Attributes["LastID"].Value = "0";

            xNode = document.SelectSingleNode("/ROOT/FLUSSI");

            if (xNode == null)
            {
                XmlNode xroot = document.SelectSingleNode("/ROOT");

                string xml = "<FLUSSI />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);
                XmlNode tmpNode = doctmp.SelectSingleNode("/FLUSSI");
                XmlNode xxtmp = document.ImportNode(tmpNode, true);
                xroot.AppendChild(xxtmp);
            }

            xNodes = document.SelectNodes("/ROOT/FLUSSI/FLUSSO");
            foreach (XmlNode node in xNodes)
            {
                FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                if (fd.Exists)
                {
                    //andrea - elimino allegati ai flussi
                    XmlManager xf = new XmlManager();
                    XmlDocument xfDoc = xf.LoadEncodedFile(fd.FullName);

                    string xpath = "//Allegato";
                    XmlNodeList tmpNodeList = xfDoc.SelectNodes(xpath);

                    string f = "";
                    foreach (XmlNode item in tmpNodeList)
                    {
                        f = App.AppDocumentiFlussiFolder + "\\" + item.Attributes["FILE"].Value;
                        if (File.Exists(f))
                            File.Delete(f);
                    }

                    fd.Delete();
                }
                node.ParentNode.RemoveChild(node);
            }

			xNodes = document.SelectNodes("/ROOT/CLIENTI/CLIENTE");
			foreach (XmlNode node in xNodes)
			{
				node.ParentNode.RemoveChild(node);
			}

			document.SelectSingleNode("/ROOT/CLIENTI").Attributes["LastID"].Value = "0";

			Save();

			Close();

            //cancello dati
			XmlManager xdoc = new XmlManager();
			xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
			XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);

			xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO");

			foreach (XmlNode node in xNodes)
			{
				FileInfo fi = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
				if (fi.Exists)
				{
					fi.Delete();
				}
					
				node.ParentNode.RemoveChild(node);
			}

			xdoc_doc.SelectSingleNode("//DOCUMENTI").Attributes["LastID"].Value = "0";
            xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);

   //         }
			//catch (Exception ex)
			//{
			//	string log = ex.Message;

			//	Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
			//}
		}

        public string GetClienteFissato()
        {
#if (DBG_TEST)
      string query;
      int i;

      query = String.Format(@"select clienteFissato from Revisoft");
      SqlDataAdapter da = new SqlDataAdapter(query, App.connString);
      DataTable dataTable = new DataTable(); da.Fill(dataTable);
      i = dataTable.Rows.Count;if (i < 1) return null;
      return dataTable.Rows[0].ItemArray[0].ToString();
#else
            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode("/ROOT/REVISOFT");
                if (xNode != null && xNode.Attributes["ClienteFissato"] != null)
                {
                    Close();
                    return xNode.Attributes["ClienteFissato"].Value;
                }
                else
                {
                    Close();
                    return null;
                }                
            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);

                return null;
            }
#endif
        }

        public void SetClienteFissato(string ID)
        {
#if (DBG_TEST)
      string query;

      query = String.Format("update [Revisoft] set [clienteFissato]={0}",ID);
      App.sqlConnection.Open();
      SqlCommand cmd = new SqlCommand(query, App.sqlConnection);
      try { cmd.ExecuteNonQuery(); }
      catch (SqlException e) { MessageBox.Show(e.Message); }
      finally { App.sqlConnection.Close(); }
#else
            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode("/ROOT/REVISOFT");
                if(xNode.Attributes["ClienteFissato"] == null)
                {
                    XmlAttribute attr = xNode.OwnerDocument.CreateAttribute("ClienteFissato");
                    xNode.Attributes.Append(attr);
                }

                xNode.Attributes["ClienteFissato"].Value = ID;

                Save();
                Close();
            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);
            }
#endif
        }

        private bool Check()
        {
            if (!File.Exists(file) && File.Exists(file+".example"))
            {
                System.IO.File.Move(file + ".example", file);
            }


                //controllo presenza File master
            if (!File.Exists(file))
            {
                ErrorCritical(WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.MancaFileMaster);

                return false;
            }

            return true;
        }

        DateTime _lastWrite = DateTime.MinValue;

        private bool IsFileChanged()
        {
            //return true;
            if (document == null) return true;
            FileInfo fileInfo = new FileInfo(file);
            bool isCahged = fileInfo.LastWriteTime > _lastWrite;
            if (isCahged)
            {
                _lastWrite = fileInfo.LastWriteTime;
                return true;
            }
            return false;
        }

        private void Open()
        {            
            if(Check())
            {
                //carico file  
                if (!IsFileChanged()) return;

                try
                {					
					document = x.LoadEncodedFile(file);
					Utilities u = new Utilities();
					if (!u.CheckXmlDocument(document, App.TipoFile.Master))
					{
						throw new Exception("Documento non valido. ID diverso da standard TipoFile");
					}
                    //x.EncodedFileToDecodedFile(file, file + ".decoded");
                }
                catch (Exception ex)
                {
                    string log = ex.Message;
					document = null;

                    Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);
                }
            }
        }

        private void Close()
        {
			//document = new XmlDocument();
        }

        private void Save()
        {
            if(Check())
            {
                //salvo file
                try
                {
					x.SaveEncodedFile(file, document.OuterXml);
                }
                catch (Exception ex)
                {
                    string log = ex.Message;

                    Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
                }
            }
        }

        private void Error(WindowGestioneMessaggi.TipologieMessaggiErrore wgmtme)
        {
            App.ErrorLevel = App.ErrorTypes.Errore;
            message.TipoMessaggioErrore = wgmtme;
            message.VisualizzaMessaggio();
        }

        private void ErrorCritical(WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti wgmtme)
        {
            App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
            message.TipoMessaggioErroreBloccante = wgmtme;
            message.VisualizzaMessaggio();
        }
#endregion

#region Codice Macchina
        public string GetCodiceMacchinaServer()
        {
            try 
            {	        
                Open();

                XmlNode xNode = document.SelectSingleNode("/ROOT/REVISOFT");
                return xNode.Attributes["CodiceMacchinaServer"].Value.ToString().Split('-')[0];

#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
                Close();
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);
                
                return null;
            }
        }
        
        public bool SetCodiceMacchinaServer(string CodiceMacchina)
        {
            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode("/ROOT/REVISOFT");

                xNode.Attributes["CodiceMacchinaServer"].Value = CodiceMacchina.Split('-')[0];
               
                Save();

                Close();

                return true;
            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);

                return false;
            }

#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
            return false;
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
        }
#endregion

#region Revisoft
		public string GetTreeAssociatoFromFileData(string file)
		{
			string returnstring = "";

			Open();

            if ( document.SelectSingleNode( "ROOT/RELAZIONIB/RELAZIONEB[@FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/RELAZIONIB/RELAZIONEB[@FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["File"].Value.ToString();
            }


            if (document.SelectSingleNode("ROOT/RELAZIONIBC/RELAZIONEBC[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
            {
                returnstring = document.SelectSingleNode("ROOT/RELAZIONIBC/RELAZIONEBC[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/RELAZIONIV/RELAZIONEV[@FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/RELAZIONIV/RELAZIONEV[@FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["File"].Value.ToString();
            }

            if (document.SelectSingleNode("ROOT/RELAZIONIVC/RELAZIONEVC[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
            {
                returnstring = document.SelectSingleNode("ROOT/RELAZIONIVC/RELAZIONEVC[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/RELAZIONIBV/RELAZIONEBV[@FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/RELAZIONIBV/RELAZIONEBV[@FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["File"].Value.ToString();
            }

			if (document.SelectSingleNode("ROOT/REVISIONI/REVISIONE[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
			{
				returnstring = document.SelectSingleNode("ROOT/REVISIONI/REVISIONE[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
			}

            if ( document.SelectSingleNode( "ROOT/REVISIONI/REVISIONE[@Sigillo1_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/REVISIONI/REVISIONE[@Sigillo1_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo1_File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/REVISIONI/REVISIONE[@Sigillo2_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/REVISIONI/REVISIONE[@Sigillo2_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo2_File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/REVISIONI/REVISIONE[@Sigillo3_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/REVISIONI/REVISIONE[@Sigillo3_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo3_File"].Value.ToString();
            }

			if (document.SelectSingleNode("ROOT/INCARICHI/INCARICO[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
			{
				returnstring = document.SelectSingleNode("ROOT/INCARICHI/INCARICO[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
			}

            if ( document.SelectSingleNode( "ROOT/INCARICHI/INCARICO[@Sigillo1_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/INCARICHI/INCARICO[@Sigillo1_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo1_File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/INCARICHI/INCARICO[@Sigillo2_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/INCARICHI/INCARICO[@Sigillo2_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo2_File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/INCARICHI/INCARICO[@Sigillo3_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/INCARICHI/INCARICO[@Sigillo3_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo3_File"].Value.ToString();
            }






            if (document.SelectSingleNode("ROOT/ISQCs/ISQC[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
            {
                returnstring = document.SelectSingleNode("ROOT/ISQCs/ISQC[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
            }

            if (document.SelectSingleNode("ROOT/ISQCs/ISQC[@Sigillo1_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
            {
                returnstring = document.SelectSingleNode("ROOT/ISQCs/ISQC[@Sigillo1_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo1_File"].Value.ToString();
            }

            if (document.SelectSingleNode("ROOT/ISQCs/ISQC[@Sigillo2_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
            {
                returnstring = document.SelectSingleNode("ROOT/ISQCs/ISQC[@Sigillo2_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo2_File"].Value.ToString();
            }

            if (document.SelectSingleNode("ROOT/ISQCs/ISQC[@Sigillo3_FileData=\"" + file.Split('\\').Last() + "\"]") != null)
            {
                returnstring = document.SelectSingleNode("ROOT/ISQCs/ISQC[@Sigillo3_FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["Sigillo3_File"].Value.ToString();
            }






            if ( document.SelectSingleNode( "ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["File"].Value.ToString();
            }

			if (document.SelectSingleNode("ROOT/VERIFICHE/VERIFICA[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
			{
				returnstring = document.SelectSingleNode("ROOT/VERIFICHE/VERIFICA[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
			}

            if ( document.SelectSingleNode( "ROOT/VERIFICHE/VERIFICA[@Sigillo1_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/VERIFICHE/VERIFICA[@Sigillo1_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo1_File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/VERIFICHE/VERIFICA[@Sigillo2_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/VERIFICHE/VERIFICA[@Sigillo2_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo2_File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/VERIFICHE/VERIFICA[@Sigillo3_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/VERIFICHE/VERIFICA[@Sigillo3_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo3_File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/VIGILANZE/VIGILANZA[@FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/VIGILANZE/VIGILANZA[@FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/VIGILANZE/VIGILANZA[@Sigillo1_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/VIGILANZE/VIGILANZA[@Sigillo1_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo1_File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/VIGILANZE/VIGILANZA[@Sigillo2_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/VIGILANZE/VIGILANZA[@Sigillo2_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo2_File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/VIGILANZE/VIGILANZA[@Sigillo3_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/VIGILANZE/VIGILANZA[@Sigillo3_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo3_File"].Value.ToString();
            }

			if (document.SelectSingleNode("ROOT/BILANCI/BILANCIO[@FileData=\"" + file.Split('\\').Last() + "\"]") != null)
			{
				returnstring = document.SelectSingleNode("ROOT/BILANCI/BILANCIO[@FileData=\"" + file.Split('\\').Last() + "\"]").Attributes["File"].Value.ToString();
			}

            if ( document.SelectSingleNode( "ROOT/BILANCI/BILANCIO[@Sigillo1_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/BILANCI/BILANCIO[@Sigillo1_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo1_File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/BILANCI/BILANCIO[@Sigillo2_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/BILANCI/BILANCIO[@Sigillo2_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo2_File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/BILANCI/BILANCIO[@Sigillo3_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/BILANCI/BILANCIO[@Sigillo3_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo3_File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/CONCLUSIONI/CONCLUSIONE[@FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/CONCLUSIONI/CONCLUSIONE[@FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/CONCLUSIONI/CONCLUSIONE[@Sigillo1_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/CONCLUSIONI/CONCLUSIONE[@Sigillo1_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo1_File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/CONCLUSIONI/CONCLUSIONE[@Sigillo2_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/CONCLUSIONI/CONCLUSIONE[@Sigillo2_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo2_File"].Value.ToString();
            }

            if ( document.SelectSingleNode( "ROOT/CONCLUSIONI/CONCLUSIONE[@Sigillo3_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ) != null )
            {
                returnstring = document.SelectSingleNode( "ROOT/CONCLUSIONI/CONCLUSIONE[@Sigillo3_FileData=\"" + file.Split( '\\' ).Last() + "\"]" ).Attributes["Sigillo3_File"].Value.ToString();
            }

			Close();

			return returnstring;
		}

		private XmlNode GetRevisoft()
		{
			try
			{
				Open();

				XmlNode xNode = document.SelectSingleNode("/ROOT/REVISOFT");

				return xNode;
			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);

				return null;
			}
		}

		public string GetChiaveServer()
		{
			try
			{
				return GetRevisoft().Attributes["ChiaveServer"].Value;
			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);

				return "";
			}
		}

		public bool SetChiaveServer(string chiave)
		{
			try
			{
				XmlNode xNode = GetRevisoft();

				xNode.Attributes["ChiaveServer"].Value = chiave;

				Save();

				return true;
			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);

				return false;
			}

#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
			return false;
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
		}

		public string GetData()
		{
			try
			{
				return GetRevisoft().Attributes["Data"].Value;
			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);

				return "";
			}
		}

		public bool SetData(string chiave)
		{
			try
			{
				XmlNode xNode = GetRevisoft();

				xNode.Attributes["Data"].Value = chiave;

				Save();

				return true;
			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);

				return false;
			}

#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
			return false;
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
		}

		public string GetDataLicenzaProva()
		{
			try
			{
				return GetRevisoft().Attributes["DataLicenzaProva"].Value;
			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);

				return "";
			}
		}

		public bool SetDataLicenzaProva(string chiave)
		{
			try
			{
				XmlNode xNode = GetRevisoft();

				xNode.Attributes["DataLicenzaProva"].Value = chiave;

				Save();

				return true;
			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);

				return false;
			}

#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
			return false;
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
		}

		public string GetDataLicenza()
		{
			try
			{
				return GetRevisoft().Attributes["DataLicenza"].Value;
			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);

				return "";
			}
		}

		public bool SetDataLicenza(string chiave)
		{
			try
			{
				XmlNode xNode = GetRevisoft();

				xNode.Attributes["DataLicenza"].Value = chiave;

				Save();

				return true;
			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);

				return false;
			}

#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
			return false;
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
		}
#endregion

        public void UpdateTipoEsercisioSu239()
        {
            Open();

            foreach (XmlNode xNode in document.SelectNodes("/ROOT/CLIENTI/CLIENTE"))
            {
                if(xNode.Attributes["Esercizio"] == null || xNode.Attributes["EsercizioDal"] == null || xNode.Attributes["EsercizioAl"] == null)
                {
                    continue;
                }
                
                foreach (XmlNode node in document.SelectNodes("/ROOT/REVISIONI/REVISIONE[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
                {
                    if (node != null)
                    {
                        if (node.Attributes["Esercizio"] == null)
                        {
                            XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
                            attr.Value = xNode.Attributes["Esercizio"].Value;
                            node.Attributes.Append(attr);

                            attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
                            attr.Value = xNode.Attributes["EsercizioDal"].Value;
                            node.Attributes.Append(attr);

                            attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
                            attr.Value = xNode.Attributes["EsercizioAl"].Value;
                            node.Attributes.Append(attr);
                        }                        
                    }
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/BILANCI/BILANCIO[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
                {   
                    if (node != null)
                    {
                        if (node.Attributes["Esercizio"] == null)
                        {
                            XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
                            attr.Value = xNode.Attributes["Esercizio"].Value;
                            node.Attributes.Append(attr);

                            attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
                            attr.Value = xNode.Attributes["EsercizioDal"].Value;
                            node.Attributes.Append(attr);

                            attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
                            attr.Value = xNode.Attributes["EsercizioAl"].Value;
                            node.Attributes.Append(attr);
                        }
                    }
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/CONCLUSIONI/CONCLUSIONE[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
                {
                    if (node != null)
                    {
                        if (node.Attributes["Esercizio"] == null)
                        {
                            XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
                            attr.Value = xNode.Attributes["Esercizio"].Value;
                            node.Attributes.Append(attr);

                            attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
                            attr.Value = xNode.Attributes["EsercizioDal"].Value;
                            node.Attributes.Append(attr);

                            attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
                            attr.Value = xNode.Attributes["EsercizioAl"].Value;
                            node.Attributes.Append(attr);
                        }
                    }
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIB/RELAZIONEB[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
                {
                    if (node != null)
                    {
                        if (node.Attributes["Esercizio"] == null)
                        {
                            XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
                            attr.Value = xNode.Attributes["Esercizio"].Value;
                            node.Attributes.Append(attr);

                            attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
                            attr.Value = xNode.Attributes["EsercizioDal"].Value;
                            node.Attributes.Append(attr);

                            attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
                            attr.Value = xNode.Attributes["EsercizioAl"].Value;
                            node.Attributes.Append(attr);
                        }
                    }
                }


                foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
                {
                    if (node != null)
                    {
                        if (node.Attributes["Esercizio"] == null)
                        {
                            XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
                            attr.Value = xNode.Attributes["Esercizio"].Value;
                            node.Attributes.Append(attr);

                            attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
                            attr.Value = xNode.Attributes["EsercizioDal"].Value;
                            node.Attributes.Append(attr);

                            attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
                            attr.Value = xNode.Attributes["EsercizioAl"].Value;
                            node.Attributes.Append(attr);
                        }
                    }
                }


                foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBV/RELAZIONEBV[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
                {
                    if (node != null)
                    {
                        if (node.Attributes["Esercizio"] == null)
                        {
                            XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
                            attr.Value = xNode.Attributes["Esercizio"].Value;
                            node.Attributes.Append(attr);

                            attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
                            attr.Value = xNode.Attributes["EsercizioDal"].Value;
                            node.Attributes.Append(attr);

                            attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
                            attr.Value = xNode.Attributes["EsercizioAl"].Value;
                            node.Attributes.Append(attr);
                        }
                    }
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIV/RELAZIONEV[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
                {
                    if (node != null)
                    {
                        if (node.Attributes["Esercizio"] == null)
                        {
                            XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
                            attr.Value = xNode.Attributes["Esercizio"].Value;
                            node.Attributes.Append(attr);

                            attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
                            attr.Value = xNode.Attributes["EsercizioDal"].Value;
                            node.Attributes.Append(attr);

                            attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
                            attr.Value = xNode.Attributes["EsercizioAl"].Value;
                            node.Attributes.Append(attr);
                        }
                    }
                }


                foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC[@Cliente='" + xNode.Attributes["ID"].Value + "']"))
                {
                    if (node != null)
                    {
                        if (node.Attributes["Esercizio"] == null)
                        {
                            XmlAttribute attr = node.OwnerDocument.CreateAttribute("Esercizio");
                            attr.Value = xNode.Attributes["Esercizio"].Value;
                            node.Attributes.Append(attr);

                            attr = node.OwnerDocument.CreateAttribute("EsercizioDal");
                            attr.Value = xNode.Attributes["EsercizioDal"].Value;
                            node.Attributes.Append(attr);

                            attr = node.OwnerDocument.CreateAttribute("EsercizioAl");
                            attr.Value = xNode.Attributes["EsercizioAl"].Value;
                            node.Attributes.Append(attr);
                        }
                    }
                }
            }

#region exists cliente
            List<XmlNode> toRemove = new List<XmlNode>();

            foreach (XmlNode node in document.SelectNodes("/ROOT/REVISIONI/REVISIONE"))
            {
                if (node != null)
                {
                    if (node.Attributes["Cliente"] != null)
                    {
                        string IDCliente = node.Attributes["Cliente"].Value;
                        if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
                        {
                            toRemove.Add(node);
                        }
                    }
                }
            }

            foreach (XmlNode node in document.SelectNodes("/ROOT/BILANCI/BILANCIO"))
            {
                if (node != null)
                {
                    if (node.Attributes["Cliente"] != null)
                    {
                        string IDCliente = node.Attributes["Cliente"].Value;
                        if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
                        {
                            toRemove.Add(node);
                        }
                    }
                }
            }

            foreach (XmlNode node in document.SelectNodes("/ROOT/CONCLUSIONI/CONCLUSIONE"))
            {
                if (node != null)
                {
                    if (node.Attributes["Cliente"] != null)
                    {
                        string IDCliente = node.Attributes["Cliente"].Value;
                        if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
                        {
                            toRemove.Add(node);
                        }
                    }
                }
            }

            foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIB/RELAZIONEB"))
            {
                if (node != null)
                {
                    if (node.Attributes["Cliente"] != null)
                    {
                        string IDCliente = node.Attributes["Cliente"].Value;
                        if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
                        {
                            toRemove.Add(node);
                        }
                    }
                }
            }


            foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC"))
            {
                if (node != null)
                {
                    if (node.Attributes["Cliente"] != null)
                    {
                        string IDCliente = node.Attributes["Cliente"].Value;
                        if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
                        {
                            toRemove.Add(node);
                        }
                    }
                }
            }

            foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIV/RELAZIONEV"))
            {
                if (node != null)
                {
                    if (node.Attributes["Cliente"] != null)
                    {
                        string IDCliente = node.Attributes["Cliente"].Value;
                        if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
                        {
                            toRemove.Add(node);
                        }
                    }
                }
            }

            foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC"))
            {
                if (node != null)
                {
                    if (node.Attributes["Cliente"] != null)
                    {
                        string IDCliente = node.Attributes["Cliente"].Value;
                        if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
                        {
                            toRemove.Add(node);
                        }
                    }
                }
            }

            foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBV/RELAZIONEBV"))
            {
                if (node != null)
                {
                    if (node.Attributes["Cliente"] != null)
                    {
                        string IDCliente = node.Attributes["Cliente"].Value;
                        if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
                        {
                            toRemove.Add(node);
                        }
                    }
                }
            }

            foreach (XmlNode node in document.SelectNodes("/ROOT/VIGILANZE/VIGILANZA"))
            {
                if (node != null)
                {
                    if (node.Attributes["Cliente"] != null)
                    {
                        string IDCliente = node.Attributes["Cliente"].Value;
                        if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
                        {
                            toRemove.Add(node);
                        }
                    }
                }
            }

            foreach (XmlNode node in document.SelectNodes("/ROOT/VERIFICHE/VERIFICA"))
            {
                if (node != null)
                {
                    if (node.Attributes["Cliente"] != null)
                    {
                        string IDCliente = node.Attributes["Cliente"].Value;
                        if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
                        {
                            toRemove.Add(node);
                        }
                    }
                }
            }

            foreach (XmlNode node in document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA"))
            {
                if (node != null)
                {
                    if (node.Attributes["Cliente"] != null)
                    {
                        string IDCliente = node.Attributes["Cliente"].Value;
                        if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
                        {
                            toRemove.Add(node);
                        }
                    }
                }
            }

            foreach (XmlNode node in document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA"))
            {
                if (node != null)
                {
                    if (node.Attributes["Cliente"] != null)
                    {
                        string IDCliente = node.Attributes["Cliente"].Value;
                        if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + IDCliente + "']") == null)
                        {
                            toRemove.Add(node);
                        }
                    }
                }
            }

            foreach (XmlNode xmlElement in toRemove)
            {
                XmlNode node = xmlElement.ParentNode;
                node.RemoveChild(xmlElement);
            }

#endregion

            Save();
            Close();
        }

#region Anagrafica
        // restituisce il nodo relativo al cliente con 'id' specificato
        private XmlNode GetAnagraficaInterna(int id)
        {
            try
            {	        
                Open();

                XmlNode xNode = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + id.ToString() + "']");

                return xNode;
            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileMaster);

                return null;
            }
        }

        public bool ClienteGiaPresente( Hashtable ht, int id )
        {
#if (DBG_TEST)
            string query;

            query = String.Format(@"select idCliente from[AnagraficaClienti] where " +
              "(partitaIva='{0}') or (codiceFiscale='{1}') or (idCliente={2})",
              ht["PartitaIVA"].ToString(), ht["CodiceFiscale"].ToString(), id);
            SqlDataAdapter da = new SqlDataAdapter(query, App.connString);
            DataTable dataTable = new DataTable(); da.Fill(dataTable);
            return (dataTable.Rows.Count > 0);
#else
            Open();

            bool returnvalue = false;

            if ( document.SelectSingleNode( "/ROOT/CLIENTI/CLIENTE[@PartitaIVA=\"" + ht["PartitaIVA"].ToString() + "\"][@ID!=\"" + id + "\"]" ) != null || document.SelectSingleNode( "/ROOT/CLIENTI/CLIENTE[@CodiceFiscale=\"" + ht["CodiceFiscale"].ToString() + "\"][@ID!=\"" + id + "\"]" ) != null )
            {
                returnvalue = true;
            }
            return returnvalue;
#endif
        }

        public int ClienteGiaPresente(Hashtable ht)
        {
#if (DBG_TEST)
            string query;

            query = String.Format( @"select idCliente from[AnagraficaClienti] where "+
                "(partitaIva='{0}') or (codiceFiscale='{1}')", ht["PartitaIVA"].ToString(), ht["CodiceFiscale"].ToString());
            SqlDataAdapter da = new SqlDataAdapter(query,App.connString);
            DataTable dataTable = new DataTable();da.Fill(dataTable);
            return (dataTable.Rows.Count > 0) ? Convert.ToInt32(dataTable.Rows[0].ItemArray[0].ToString()) : -1;
#else
            Open();

            int returnvalue = -1;

            if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@PartitaIVA=\"" + ht["PartitaIVA"].ToString() + "\"]") != null )
            {
                returnvalue = Convert.ToInt32(document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@PartitaIVA=\"" + ht["PartitaIVA"].ToString() + "\"]").Attributes["ID"].Value.ToString());
            }
            
            if(document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@CodiceFiscale=\"" + ht["CodiceFiscale"].ToString() + "\"]") != null)
            {
                returnvalue = Convert.ToInt32( document.SelectSingleNode( "/ROOT/CLIENTI/CLIENTE[@CodiceFiscale=\"" + ht["CodiceFiscale"].ToString() + "\"]" ).Attributes["ID"].Value.ToString() );
            }

            return returnvalue;
#endif
        }

        public void InsertClientChild( int ID, XmlNode node)
        {
            XmlNode xNode = GetAnagraficaInterna(ID);

            xNode.InnerText = "";

            foreach (XmlNode item in node.ChildNodes)
            {
                XmlNode xNode2 = xNode.OwnerDocument.ImportNode(item, true);
                xNode.AppendChild(xNode2);
            }
           
            Save();
        }

		public int CheckEsistenzaCliente(Hashtable ht)
		{
			Open();

			int IDReal = -1;

            //andrea
			//if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@RagioneSociale='" + ht["RagioneSociale"].ToString() + "']") == null) //non esiste questo cliente.

            if ( document.SelectSingleNode( "/ROOT/CLIENTI/CLIENTE[@RagioneSociale=\"" + ((ht["RagioneSociale"] == null)? "" : ht["RagioneSociale"].ToString()) + "\"]" ) == null ) //non esiste questo cliente.
			{
				//aggiungo nuovo senza avviso
				IDReal = SetAnagrafica(ht, App.MasterFile_NewID);
			}
			//else if (document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@RagioneSociale='" + ht["RagioneSociale"].ToString() + "'][@CodiceFiscale='" + ht["CodiceFiscale"].ToString() + "']") != null)
			//{
			//    IDReal = Convert.ToInt32(document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@RagioneSociale='" + ht["RagioneSociale"].ToString() + "'][@CodiceFiscale='" + ht["CodiceFiscale"].ToString() + "']").Attributes["ID"].Value);
			//}

			return IDReal;
		}

        public string GetIDAnagrafica(string RagioneSociale)
        {
            Open();

            string IDReal = "-1";

            XmlNode node = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@RagioneSociale=\"" + ((RagioneSociale == null) ? "" : RagioneSociale) + "\"]");
            if (node != null) 
            {
                IDReal = node.Attributes["ID"].Value;
            }

            return IDReal;
        }

        public bool GetAllXmlCliente(int id, string ret, bool Condividi)
		{
            try
            {
                Open();

                string cartellatmp = App.AppTempFolder + Guid.NewGuid().ToString();
                DirectoryInfo di = new DirectoryInfo(cartellatmp);
                if (di.Exists)
                {
                    //errore directory già esistente aspettare processo terminato da parte di altro utente
                    return false;
                }

                di.Create();

#pragma warning disable CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato
                bool directoryflussiesiste = false;
#pragma warning restore CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato

                string xml = "<ROOT>";

                if (!Condividi)
                {
                    //andrea - versione 3.0 inserimento di codice macchina in file di esportazione - funzionalità disponibile a livello di licenza
                    xml += "<LICENZA CodiceMacchinaServer=\"" + App.CodiceMacchinaServer.Split('-')[0] + "\" CodiceMacchina=\"" + App.CodiceMacchina.Split('-')[0] + "\" />";
                }

                XmlNode xNode = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + id.ToString() + "']");

                if (xNode == null)
                {
                    return false;
                }

                xml += xNode.OuterXml;

                foreach (XmlNode node in document.SelectNodes("/ROOT/INCARICHI/INCARICO[@Cliente='" + id.ToString() + "']"))
                {
                    FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    xml += node.OuterXml;
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/ISQCs/ISQC[@Cliente='" + id.ToString() + "']"))
                {
                    FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    xml += node.OuterXml;
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/REVISIONI/REVISIONE[@Cliente='" + id.ToString() + "']"))
                {
                    FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (f_d.Exists)
                    {
                        //XAML
                        XmlDataProviderManager _xaml = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);

                        if (_xaml != null && _xaml.Document.SelectSingleNode("/Dati//Dato[@ID='274']") != null)
                        {
                            foreach (XmlNode tmpnode in _xaml.Document.SelectSingleNode("/Dati//Dato[@ID='274']").SelectNodes("Node[@xaml]"))
                            {
                                try
                                {
                                    FileInfo fxamlhere = new FileInfo(App.AppDataDataFolder + tmpnode.Attributes["xaml"].Value);

                                    if (!fxamlhere.Exists)
                                    {
                                        tmpnode.Attributes.Remove(tmpnode.Attributes["xaml"]);
                                    }
                                    else
                                    {
                                        fxamlhere.CopyTo(di.FullName + "\\" + fxamlhere.Name, true);
                                    }
                                }
                                catch (Exception ex2)
                                {
                                    string log = ex2.Message;
                                    tmpnode.Attributes.Remove(tmpnode.Attributes["xaml"]);
                                }
                                
                            }

                            _xaml.Save();
                        }                      
                   
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    xml += node.OuterXml;
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/BILANCI/BILANCIO[@Cliente='" + id.ToString() + "']"))
                {
                    FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    xml += node.OuterXml;
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/CONCLUSIONI/CONCLUSIONE[@Cliente='" + id.ToString() + "']"))
                {
                    FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    xml += node.OuterXml;
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/VERIFICHE/VERIFICA[@Cliente='" + id.ToString() + "']"))
                {
                    FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    xml += node.OuterXml;
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/VIGILANZE/VIGILANZA[@Cliente='" + id.ToString() + "']"))
                {
                    FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    xml += node.OuterXml;
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/FLUSSI/FLUSSO[@Cliente='" + id.ToString() + "']"))
                {
                    FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);

                        XmlDataProviderManager _fa = new XmlDataProviderManager(di.FullName + "\\" + node.Attributes["FileData"].Value, true);

                        string xpath = "//Allegato";
                        string directory = App.AppDocumentiFolder + "\\Flussi";

                        foreach (XmlNode item in _fa.Document.SelectNodes(xpath))
                        {
                            FileInfo f_fa = new FileInfo(directory + "\\" + item.Attributes["FILE"].Value);

                            if (f_fa.Exists)
                            {
                                DirectoryInfo newdi = new DirectoryInfo(di.FullName + "\\Flussi");
                                if (newdi.Exists == false)
                                {
                                    newdi.Create();
                                }

                                directoryflussiesiste = true;
                                f_fa.CopyTo(di.FullName + "\\Flussi\\" + item.Attributes["FILE"].Value, true);
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }

                    xml += node.OuterXml;
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@Cliente='" + id.ToString() + "']"))
                {
                    FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    xml += node.OuterXml;
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@Cliente='" + id.ToString() + "']"))
                {
                    FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    xml += node.OuterXml;
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIB/RELAZIONEB[@Cliente='" + id.ToString() + "']"))
                {
                    FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    xml += node.OuterXml;
                }




                foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC[@Cliente='" + id.ToString() + "']"))
                {
                    FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    xml += node.OuterXml;
                }


                foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBV/RELAZIONEBV[@Cliente='" + id.ToString() + "']"))
                {
                    FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    xml += node.OuterXml;
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIV/RELAZIONEV[@Cliente='" + id.ToString() + "']"))
                {
                    FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    xml += node.OuterXml;
                }


                foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC[@Cliente='" + id.ToString() + "']"))
                {
                    FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["File"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + node.Attributes["FileData"].Value);
                    }
                    else
                    {
                        continue;
                    }

                    xml += node.OuterXml;
                }



                XmlDataProviderManager _d = new XmlDataProviderManager(App.AppDocumentiDataFile, true);

                XmlNodeList nodelisttmp = _d.Document.SelectNodes("//DOCUMENTI//DOCUMENTO[@Cliente='" + id.ToString() + "']");

                int numtotdoc = 0;

                foreach (XmlNode nodetmp in nodelisttmp)
                {
                    FileInfo f_d = new FileInfo(App.AppDocumentiFolder + "\\" + nodetmp.Attributes["File"].Value);
                    if (f_d.Exists)
                    {
                        f_d.CopyTo(di.FullName + "\\" + nodetmp.Attributes["File"].Value, true);
                        xml += nodetmp.OuterXml;
                        numtotdoc++;
                    }
                }

                xml += "</ROOT>";

                string path_fileX = di.FullName + "\\" + "all.xml";

                XmlDocument xmlTMP = new XmlDocument();
                xmlTMP.LoadXml(xml);

                XmlNodeList nodelisttmptest = xmlTMP.SelectNodes("//DOCUMENTO[@Cliente='" + id.ToString() + "']");

                //PRISCTBD
                if(numtotdoc != nodelisttmptest.Count)
                {
                    return false;
                }

                foreach (XmlNode nodetmp in nodelisttmptest)
                {
                    FileInfo f_d = new FileInfo(di.FullName + "\\" + nodetmp.Attributes["File"].Value);
                    if (!f_d.Exists)
                    {
                        return false;
                    }
                }

                xmlTMP.Save(path_fileX);

                Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
                zip.Password = App.ZipFilePassword;

                zip.AddDirectory(di.FullName);
                zip.Save(ret);

                FileInfo finew = new FileInfo(ret);

                char[] invalidChars = Path.GetInvalidFileNameChars();
                string RagioneSociale = new string
                                                (
                                                    xNode.Attributes["RagioneSociale"].Value
                                                        .Where(x => !invalidChars.Contains(x))
                                                        .ToArray()
                                                );

                //3.6 andrea
                //string nuovofile = App.AppBackupFolder + "\\ClientiEsportati\\" + RagioneSociale + ".rief";
                //4.6 aggiungo BackUpFolder
                string nuovofile = App.AppBackupFolder + "\\" + App.ClientiEsportatiFolder + "\\" + RagioneSociale + " (" + DateTime.Now.ToShortDateString().Replace('/', '-') + "-" + DateTime.Now.ToShortTimeString().Replace(':', '.') + ").rief";
                //4.6
                DirectoryInfo ditmp = new DirectoryInfo(App.AppBackupFolder + "\\" + App.ClientiEsportatiFolder);
                if (!ditmp.Exists)
                {
                    ditmp.Create();
                }

                finew.CopyTo(nuovofile, true); //Backup silenzioso dei dati cliente affidato il recupero all'help desk

                //Cancello i temporanei
                di.Delete(true);

                return true;
            }
            catch (Exception ex)
            {
                string log = ex.Message;
                return false;
            }
        }

        public Hashtable GetAnagrafica(int id)
        {
            Hashtable results = new Hashtable();
#if (DBG_TEST)
            string query,str,str2;
            query = String.Format("select * from [AnagraficaClienti] where (idCliente={0})", id);
            SqlDataAdapter da = new SqlDataAdapter(query, App.connString);
            DataTable dataTable = new DataTable();
            try
            {
                da.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        str = dataTable.Rows[0].ItemArray[col].ToString();
                        if (str.Contains("1900 00:00:00")) { str2 = str.Remove(5); str = str2; }
                        if (str.Contains("00:00:00")) { str2 = str.Remove(10); str = str2; }
                        results.Add(App.clientiFields[col], str);
                    }
                }
            }
#else
            try
            {	        
                XmlNode xNode = GetAnagraficaInterna(id);

                foreach (XmlAttribute item in xNode.Attributes)
	            {
                    results.Add(item.Name, item.Value);
	            }

            }
#endif
            catch (Exception ex)
            {
                string log = ex.Message;
            }

            return results;
        }

        public XmlNode GetAnagraficaBV(int id)
        {  
            try
            {
                XmlNode xNode = GetAnagraficaInterna(id);

                return xNode.SelectSingleNode("BilancioVerifica");
            }
            catch (Exception ex)
            {
                string log = ex.Message;
            }

            return null;
        }

        public bool SetAnagraficaBV(int id, XmlNode BVNode)
        {
            try
            {
                XmlNode xNode = GetAnagraficaInterna(id);

                XmlNode xNode2 = xNode.SelectSingleNode("BilancioVerifica");

                if(xNode2 != null)
                {
                    xNode2.ParentNode.RemoveChild(xNode2);
                }

                xNode2 = xNode.OwnerDocument.ImportNode(BVNode, true);

                xNode.AppendChild(xNode2);

                Save();

                return true;
            }
            catch (Exception ex)
            {
                string log = ex.Message;
            }

            return false;
        }

        public App.TipoAnagraficaStato GetAnafraficaStato(int id)
        {
#if (DBG_TEST)
            try
            {
                Hashtable ht = new Hashtable();
                ht = GetAnagrafica(id);
                return (App.TipoAnagraficaStato) Convert.ToInt32(ht["Stato"].ToString());
            }
#else
            try 
            {
                XmlNode xNode = GetAnagraficaInterna(id);
                                
                return (App.TipoAnagraficaStato)Convert.ToInt32(xNode.Attributes["Stato"].Value.ToString());
            }
#endif
            catch (Exception ex)
            {
                string log = ex.Message;

                return App.TipoAnagraficaStato.Sconosciuto;
            }
        }

        public bool SetAnafraficaStato(int id, App.TipoAnagraficaStato stato)
        {
            if(stato == App.TipoAnagraficaStato.Sconosciuto)
            {
                return false;
            }
#if (DBG_TEST)
            string query;

            query = String.Format(
                "update [AnagraficaClienti] set [idTipoAnagraficaStato]={0}\n" +
                "where (idCliente={1})", (int) stato, id);
            App.sqlConnection.Open();
            SqlCommand cmd = new SqlCommand(query, App.sqlConnection);
            try { cmd.ExecuteNonQuery();return true; }
            catch (SqlException e) { MessageBox.Show(e.Message);return false; }
            finally { App.sqlConnection.Close(); }
#else
            try
            {
                XmlNode xNode = GetAnagraficaInterna(id);

                if(xNode == null)
                {
                    return false;
                }

				if(xNode.Attributes["Stato"] == null)
				{
					xNode.OwnerDocument.Attributes.Append(xNode.OwnerDocument.CreateAttribute("Stato"));
				}

                xNode.Attributes["Stato"].Value = ((int)(stato)).ToString();

				if(xNode.Attributes["DataModificaStato"] == null)
				{
					xNode.Attributes.Append(xNode.OwnerDocument.CreateAttribute("DataModificaStato"));
				}

				xNode.Attributes["DataModificaStato"].Value = DateTime.Now.ToShortDateString();

                if ( xNode.Attributes["UtenteModificaStato"] == null )
                {
                    xNode.Attributes.Append( xNode.OwnerDocument.CreateAttribute( "UtenteModificaStato" ) );
                }

                RevisoftApplication.GestioneLicenza l = new GestioneLicenza();
                xNode.Attributes["UtenteModificaStato"].Value = l.Utente;

                Save();

                return true;
            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
               
                return false;
            }
#endif
        }

        public int GetAnagraficaNumeroSigilliTotali( int id )
        {
            int numeroSigilli = 0;
#if (DBG_TEST)
            Hashtable ht = new Hashtable();
            try
            {
                ht = GetAnagrafica(id);
                if (ht["RevisoreAutonomo"].ToString() != "") { numeroSigilli++;return numeroSigilli; }
                else
                {
                    if (ht["Presidente"].ToString() != "") numeroSigilli++;
                    if (ht["MembroEffettivo"].ToString() != "") numeroSigilli++;
                    if (ht["MembroEffettivo2"].ToString() != "") numeroSigilli++;
                }
            }
#else
            try
            {
                XmlNode cliente = GetAnagraficaInterna( id );

                if(cliente["RevisoreAutonomo"] != null)
                {
                    numeroSigilli = 1;
                    return numeroSigilli;
                }
                else
                {
                    if(cliente["Presidente"] != null)
                    {
                        numeroSigilli++;
                    }

                    if(cliente["MembroEffettivo"] != null)
                    {
                        numeroSigilli++;
                    }

                    if(cliente["MembroEffettivo2"] != null)
                    {
                        numeroSigilli++;
                    }
                }
            }
#endif
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return numeroSigilli;
        }

        public int GetAnagraficheCount()
        {
            ArrayList Clienti = GetAnagrafiche();

            return Clienti.Count;
        }

        public ArrayList GetAnagrafiche()
        {
            ArrayList results = new ArrayList();
            try
            {
#if (DBG_TEST)
                SqlDataAdapter da = new SqlDataAdapter("select * from [AnagraficaClienti] order by [ragioneSociale],[idCliente]", App.connString);
                DataTable dataTable = new DataTable();
                string str2;
                da.Fill(dataTable);
                for (int row=0;row<dataTable.Rows.Count;row++)
                {
                    Hashtable result = new Hashtable();
                    for (int col = 0; col < dataTable.Columns.Count; col++)
                    {
                        string str = dataTable.Rows[row].ItemArray[col].ToString();
                        if (str.Contains("1900 00:00:00")) { str2=str.Remove(5);str = str2; }
                        if (str.Contains("00:00:00")) { str2 = str.Remove(10); str = str2; }
                        result.Add(App.clientiFields[col], str);
                    }
                    results.Add(result);
                }
#else
                Open();
                XmlNodeList xNodes = document.SelectNodes("/ROOT/CLIENTI/CLIENTE");
                XmlNode xNodeCF = document.SelectSingleNode("/ROOT/REVISOFT");
                foreach (XmlNode node in xNodes)
	            {
                    if (xNodeCF.Attributes["ClienteFissato"] != null
                        && xNodeCF.Attributes["ClienteFissato"].Value != "-1"
                        && xNodeCF.Attributes["ClienteFissato"].Value != ""
                        && xNodeCF.Attributes["ClienteFissato"].Value != node.Attributes["ID"].Value)
                    {
                        continue;
                    }
                    Hashtable result = new Hashtable();
                    foreach (XmlAttribute item in node.Attributes)
	                {
                        result.Add(item.Name, item.Value);
	                }
                    results.Add(result);
	            }
                Close();
#endif
            }
            catch (Exception ex)
            {
                string log = ex.Message;
            }
            return results;
        }

		public bool DeleteAnagrafica(string RagioneSociale)
		{
			Open();

			XmlNode xNode = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@RagioneSociale=\"" + RagioneSociale + "\"]");

			if (xNode == null)
			{
				return true;
			}

			int id = Convert.ToInt32(xNode.Attributes["ID"].Value);

			Close();

			return DeleteAnagrafica(id);
		}

        public bool DeleteAnagrafica(int id)
        {
      //string query;

      //query = String.Format(
      //    "update [AnagraficaClienti] set [idTipoAnagraficaStato]={0}\n" +
      //    "where (idCliente={1})", (int)stato, id);
      //App.sqlConnection.Open();
      //SqlCommand cmd = new SqlCommand(query, App.sqlConnection);
      //try { cmd.ExecuteNonQuery(); return true; }
      //catch (SqlException e) { MessageBox.Show(e.Message); return false; }
      //finally { App.sqlConnection.Close(); }
            try 
            {	        
                Open();

                XmlNode xNode = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + id.ToString() + "']");
                
		        xNode.ParentNode.RemoveChild(xNode);

				foreach (XmlNode node in document.SelectNodes("/ROOT/INCARICHI/INCARICO[@Cliente='" + id.ToString() + "']"))
				{
                    if (node.Attributes["File"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    if (node.Attributes["FileData"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    node.ParentNode.RemoveChild(node);
				}






                foreach (XmlNode node in document.SelectNodes("/ROOT/ISQCs/ISQC[@Cliente='" + id.ToString() + "']"))
                {
                    if (node.Attributes["File"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    if (node.Attributes["FileData"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    node.ParentNode.RemoveChild(node);
                }



                foreach (XmlNode node in document.SelectNodes("/ROOT/REVISIONI/REVISIONE[@Cliente='" + id.ToString() + "']"))
				{
                    if (node.Attributes["File"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    if (node.Attributes["FileData"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    node.ParentNode.RemoveChild(node);
				}

                foreach ( XmlNode node in document.SelectNodes( "/ROOT/RELAZIONIV/RELAZIONEV[@Cliente='" + id.ToString() + "']" ) )
                {
                    if (node.Attributes["File"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    if (node.Attributes["FileData"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    node.ParentNode.RemoveChild( node );
                }




                foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC[@Cliente='" + id.ToString() + "']"))
                {
                    if (node.Attributes["File"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    if (node.Attributes["FileData"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    node.ParentNode.RemoveChild(node);
                }


                foreach ( XmlNode node in document.SelectNodes( "/ROOT/RELAZIONIB/RELAZIONEB[@Cliente='" + id.ToString() + "']" ) )
                {
                    if (node.Attributes["File"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    if (node.Attributes["FileData"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    node.ParentNode.RemoveChild( node );
                }




                foreach (XmlNode node in document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC[@Cliente='" + id.ToString() + "']"))
                {
                    if (node.Attributes["File"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    if (node.Attributes["FileData"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    node.ParentNode.RemoveChild(node);
                }



                foreach ( XmlNode node in document.SelectNodes( "/ROOT/RELAZIONIBV/RELAZIONEBV[@Cliente='" + id.ToString() + "']" ) )
                {
                    if (node.Attributes["File"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    if (node.Attributes["FileData"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    node.ParentNode.RemoveChild( node );
                }

				foreach (XmlNode node in document.SelectNodes("/ROOT/BILANCI/BILANCIO[@Cliente='" + id.ToString() + "']"))
				{
                    if (node.Attributes["File"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    if (node.Attributes["FileData"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    node.ParentNode.RemoveChild(node);
				}

				foreach (XmlNode node in document.SelectNodes("/ROOT/VERIFICHE/VERIFICA[@Cliente='" + id.ToString() + "']"))
				{
                    if (node.Attributes["File"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    if (node.Attributes["FileData"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    node.ParentNode.RemoveChild(node);
				}

                foreach (XmlNode node in document.SelectNodes("/ROOT/VIGILANZE/VIGILANZA[@Cliente='" + id.ToString() + "']"))
                {
                    if (node.Attributes["File"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    if (node.Attributes["FileData"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    node.ParentNode.RemoveChild(node);
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/CONCLUSIONI/CONCLUSIONE[@Cliente='" + id.ToString() + "']"))
                {
                    if (node.Attributes["File"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    if (node.Attributes["FileData"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    node.ParentNode.RemoveChild(node);
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/FLUSSI/FLUSSO[@Cliente='" + id.ToString() + "']"))
                {
                    FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (f_d.Exists)
                    {
                        XmlDataProviderManager _fa = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value, true);

                        string xpath = "//Allegato";
                        string directory = App.AppDocumentiFolder + "\\Flussi";

                        foreach (XmlNode item in _fa.Document.SelectNodes(xpath))
                        {
                            FileInfo f_fa = new FileInfo(App.AppDataDataFolder + "\\Flussi\\" + item.Attributes["FILE"].Value);

                            if (f_fa.Exists)
                            {
                                try
                                {
                                    f_fa.Delete();
                                }
                                catch (Exception ex2)
                                {
                                    string log = ex2.Message;
                                }
                            }
                        }

                        try
                        {
                            f_d.Delete();
                        }
                        catch (Exception ex2)
                        {
                            string log = ex2.Message;
                        }
                    }

                    node.ParentNode.RemoveChild(node);
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@Cliente='" + id.ToString() + "']"))
                {
                    if (node.Attributes["File"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    if (node.Attributes["FileData"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    node.ParentNode.RemoveChild(node);
                }

                foreach (XmlNode node in document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@Cliente='" + id.ToString() + "']"))
                {
                    if (node.Attributes["File"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    if (node.Attributes["FileData"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }
                        }
                    }

                    node.ParentNode.RemoveChild(node);
                }

                XmlDataProviderManager _d = new XmlDataProviderManager(App.AppDocumentiDataFile, true);

				//XmlNodeList nodelisttmp = ;

				foreach (XmlNode nodetmp in _d.Document.SelectNodes("//DOCUMENTI//DOCUMENTO[@Cliente='" + id.ToString() + "']"))
				{
                    if (nodetmp.Attributes["File"] != null)
                    {
                        FileInfo f_d = new FileInfo(App.AppDocumentiFolder + "\\" + nodetmp.Attributes["File"].Value);
                        if (f_d.Exists)
                        {
                            try
                            {
                                f_d.Delete();
                            }
                            catch (Exception ex2)
                            {
                                string log = ex2.Message;
                            }                            
                        }
                    }

                    nodetmp.ParentNode.RemoveChild(nodetmp);
                }

                _d.Save();
                
                //while ( nodelisttmp.Count > 0 )
                //{
                //    nodelisttmp[0].ParentNode.RemoveChild( nodelisttmp[0] );

                //    nodelisttmp = _d.Document.SelectNodes( "//DOCUMENTI//DOCUMENTO[@Cliente='" + id.ToString() + "']" );
                //}

                Save();

                Close();

                return true;
            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Close();

                //Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);

                return false;
            }
    }

        public int SetAnagrafica(Hashtable values, int id)
        {
#if (DBG_TEST)
            string query, note, esercizioAl, esercizioDal, codiceFiscale,partitaIva,ragioneSociale, presidente;
            string membroEffettivo, membroEffettivo2, revisoreAutonomo, sindacoSupplente, sindacoSupplente2;
            int retIdCliente=id, idTipoAnagraficaEsercizio, idTipoOrganoDiControllo, idTipoOrganoDiRevisione;
            int idTipoAnagraficaStato,res;

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("", App.connString);
            DataTable dataTable = new DataTable();
            DataSet dataSet=new DataSet();

            // preparazione dati cliente
#region preparazione dati cliente
            idTipoAnagraficaStato = (!values.Contains("Stato")) ? -1 : Convert.ToInt32(values["Stato"].ToString());
            note = (!values.Contains("Note")) ? "" : values["Note"].ToString(); note.Trim();
            if (note.Length < 1) note = "null";
            else { note.Replace("'", "''"); note = "'" + note + "'"; }
            esercizioAl = (!values.Contains("EsercizioAl")) ? "" : values["EsercizioAl"].ToString(); esercizioAl.Trim();
            if (esercizioAl.Length < 1) esercizioAl = "null";
            else { esercizioAl += @"/1900"; esercizioAl = "'" + esercizioAl + "'"; }
            esercizioDal = (!values.Contains("EsercizioDal")) ? "" : values["EsercizioDal"].ToString(); esercizioDal.Trim();
            if (esercizioDal.Length < 1) esercizioDal = "null";
            else { esercizioDal += @"/1900"; esercizioDal = "'" + esercizioDal + "'"; }
            idTipoAnagraficaEsercizio = (!values.Contains("Esercizio")) ? -1 : Convert.ToInt32(values["Esercizio"].ToString());
            codiceFiscale = (!values.Contains("CodiceFiscale")) ? "" : values["CodiceFiscale"].ToString(); codiceFiscale.Trim();
            if (codiceFiscale.Length < 1) codiceFiscale = "null";
            else { codiceFiscale.Replace("'", "''"); codiceFiscale = "'" + codiceFiscale + "'"; }
            partitaIva = (!values.Contains("PartitaIVA")) ? "" : values["PartitaIVA"].ToString(); partitaIva.Trim();
            if (partitaIva.Length < 1) partitaIva = "null";
            else { partitaIva.Replace("'", "''"); partitaIva = "'" + partitaIva + "'"; }
            ragioneSociale = (!values.Contains("RagioneSociale")) ? "" : values["RagioneSociale"].ToString(); ragioneSociale.Trim();
            if (ragioneSociale.Length < 1) ragioneSociale = "null";
            else { ragioneSociale.Replace("'", "''"); ragioneSociale = "'" + ragioneSociale + "'"; }
            presidente = (!values.Contains("Presidente")) ? "" : values["Presidente"].ToString(); presidente.Trim();
            if (presidente.Length < 1) presidente = "null";
            else { presidente.Replace("'", "''"); presidente = "'" + presidente + "'"; }
            membroEffettivo = (!values.Contains("MembroEffettivo")) ? "" : values["MembroEffettivo"].ToString(); membroEffettivo.Trim();
            if (membroEffettivo.Length < 1) membroEffettivo = "null";
            else { membroEffettivo.Replace("'", "''"); membroEffettivo = "'" + membroEffettivo + "'"; }
            membroEffettivo2 = (!values.Contains("MembroEffettivo2")) ? "" : values["MembroEffettivo2"].ToString(); membroEffettivo2.Trim();
            if (membroEffettivo2.Length < 1) membroEffettivo2 = "null";
            else { membroEffettivo2.Replace("'", "''"); membroEffettivo2 = "'" + membroEffettivo2 + "'"; }
            revisoreAutonomo = (!values.Contains("RevisoreAutonomo")) ? "" : values["RevisoreAutonomo"].ToString(); revisoreAutonomo.Trim();
            if (revisoreAutonomo.Length < 1) revisoreAutonomo = "null";
            else { revisoreAutonomo.Replace("'", "''"); revisoreAutonomo = "'" + revisoreAutonomo + "'"; }
            idTipoOrganoDiControllo = (!values.Contains("OrganoDiControllo")) ? 3 : Convert.ToInt32(values["OrganoDiControllo"].ToString());
            idTipoOrganoDiRevisione = (!values.Contains("OrganoDiRevisione")) ? 1 : Convert.ToInt32(values["OrganoDiRevisione"].ToString());
            sindacoSupplente = (!values.Contains("SindacoSupplente")) ? "" : values["SindacoSupplente"].ToString(); sindacoSupplente.Trim();
            if (sindacoSupplente.Length < 1) sindacoSupplente = "null";
            else { sindacoSupplente.Replace("'", "''"); sindacoSupplente = "'" + sindacoSupplente + "'"; }
            sindacoSupplente2 = (!values.Contains("SindacoSupplente2")) ? "" : values["SindacoSupplente2"].ToString(); sindacoSupplente2.Trim();
            if (sindacoSupplente2.Length < 1) sindacoSupplente2 = "null";
            else { sindacoSupplente2.Replace("'", "''"); sindacoSupplente2 = "'" + sindacoSupplente2 + "'"; }
#endregion
            if (id == App.MasterFile_NewID) // nuovo cliente
            {
                // generazione nuovo idCliente
                query = "select max(idCliente) from [AnagraficaClienti]";
                sqlDataAdapter.SelectCommand = new SqlCommand(query,App.sqlConnection);
                sqlDataAdapter.Fill(dataTable);
                retIdCliente= (dataTable.Rows.Count < 1) ? 1 : Convert.ToInt32(dataTable.Rows[0].ItemArray[0]) + 1;
                // inserimento nuovo cliente
                query = String.Format(
                  "insert into [AnagraficaClienti] (\n" +
                      "[idCliente],[idTipoAnagraficaStato],[note],[esercizioAl],[esercizioDal],\n" +
                      "[idTipoAnagraficaEsercizio],[codiceFiscale],[partitaIva],[ragioneSociale],\n" +
                      "[presidente],[membroEffettivo],[membroEffettivo2],[revisoreAutonomo],\n" +
                      "[idTipoOrganoDiControllo],[idTipoOrganoDiRevisione],[sindacoSupplente],\n" +
                      "[sindacoSupplente2],[dataModificaStato],[utenteModificaStato])\n" +
                  "values (\n" +
                      "{0},{1},{2},{3},{4},\n" +
                      "{5},{6},{7},{8},\n" +
                      "{9},{10},{11},{12},\n" +
                      "{13},{14},{15},\n" +
                      "{16},null,null)",
                  retIdCliente, 0, note, esercizioAl, esercizioDal,
                  idTipoAnagraficaEsercizio, codiceFiscale, partitaIva, ragioneSociale, presidente,
                  membroEffettivo, membroEffettivo2, revisoreAutonomo, idTipoOrganoDiControllo,
                  idTipoOrganoDiRevisione, sindacoSupplente, sindacoSupplente2);
                App.sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(query, App.sqlConnection);
                try { res=cmd.ExecuteNonQuery(); }
                catch (SqlException e) { MessageBox.Show(e.Message); }
                finally { App.sqlConnection.Close(); }
            } // nuovo cliente
            else // aggiornamento dati cliente esistente
            {
                // aggiornamento cliente
                query = String.Format(
                    "update [AnagraficaClienti] set\n" +
                    "  [note]={0},[esercizioAl]={1},\n" +
                    "  [esercizioDal]={2},[idTipoAnagraficaEsercizio]={3},[codiceFiscale]={4},\n" +
                    "  [partitaIva]={5},[ragioneSociale]={6},[presidente]={7},\n" +
                    "  [membroEffettivo]={8},[membroEffettivo2]={9},\n" +
                    "  [revisoreAutonomo]={10},[idTipoOrganoDiControllo]={11},\n" +
                    "  [idTipoOrganoDiRevisione]={12},[sindacoSupplente]={13},\n" +
                    "  [sindacoSupplente2]={14}\n" +
                    "where (idCliente={15})",
                    note, esercizioAl,
                    esercizioDal, idTipoAnagraficaEsercizio, codiceFiscale,
                    partitaIva, ragioneSociale, presidente,
                    membroEffettivo, membroEffettivo2,
                    revisoreAutonomo, idTipoOrganoDiControllo,
                    idTipoOrganoDiRevisione, sindacoSupplente,
                    sindacoSupplente2, retIdCliente);
                App.sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(query, App.sqlConnection);
                try { res = cmd.ExecuteNonQuery(); }
                catch (SqlException e) { MessageBox.Show(e.Message); }
                finally { App.sqlConnection.Close(); }
            } // aggiornamento dati cliente esistente
            return retIdCliente;
#else
            int returnID = id;

            Open();				

            if (id == App.MasterFile_NewID)
            {
                XmlNode root = document.SelectSingleNode(" / ROOT/CLIENTI");

                if (root.Attributes["LastID"] == null)
                {
                    XmlAttribute attr = document.CreateAttribute("LastID");
                    attr.Value = "0";
                    root.Attributes.Append(attr);
                }

                returnID = (Convert.ToInt32(((root.Attributes["LastID"] == null)? "0" : root.Attributes["LastID"].Value)) + 1);

				string lastindex = returnID.ToString();


                string xml = "<CLIENTE ID=\"" + lastindex + "\" Stato=\"" + ( (int)( App.TipoAnagraficaStato.Disponibile ) ).ToString() + "\" Note=\"" + ( ( !values.Contains( "Note" ) ) ? "" : values["Note"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) ) + "\" EsercizioAl=\"" + ( ( !values.Contains( "EsercizioAl" ) ) ? "" : values["EsercizioAl"].ToString() ) + "\" EsercizioDal=\"" + ( ( !values.Contains( "EsercizioDal" ) ) ? "" : values["EsercizioDal"].ToString() ) + "\" Esercizio=\"" + ( ( !values.Contains( "Esercizio" ) ) ? "" : values["Esercizio"].ToString() ) + "\" CodiceFiscale=\"" + ( ( !values.Contains( "CodiceFiscale" ) ) ? "" : values["CodiceFiscale"].ToString() ) + "\" PartitaIVA=\"" + ( ( !values.Contains( "PartitaIVA" ) ) ? "" : values["PartitaIVA"].ToString() ) + "\" RagioneSociale=\"" + ( ( !values.Contains( "RagioneSociale" ) ) ? "" : values["RagioneSociale"].ToString().ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) ) + "\" Presidente=\"" + ( ( !values.Contains( "Presidente" ) ) ? "" : values["Presidente"].ToString().ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) ) + "\" MembroEffettivo=\"" + ( ( !values.Contains( "MembroEffettivo" ) ) ? "" : values["MembroEffettivo"].ToString().ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) ) + "\" MembroEffettivo2=\"" + ( ( !values.Contains( "MembroEffettivo2" ) ) ? "" : values["MembroEffettivo2"].ToString().ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) ) + "\" RevisoreAutonomo=\"" + ( ( !values.Contains( "RevisoreAutonomo" ) ) ? "" : values["RevisoreAutonomo"].ToString().ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) ) + "\" OrganoDiControllo=\"" + ( ( !values.Contains( "OrganoDiControllo" ) ) ? "" : values["OrganoDiControllo"].ToString() ) + "\" OrganoDiRevisione=\"" + ( ( !values.Contains( "OrganoDiRevisione" ) ) ? "" : values["OrganoDiRevisione"].ToString() ) + "\" SindacoSupplente=\"" + ((!values.Contains("SindacoSupplente")) ? "" : values["SindacoSupplente"].ToString().ToString().Replace("&", "&amp;").Replace("\"", "'")) + "\" SindacoSupplente2=\"" + ((!values.Contains("SindacoSupplente2")) ? "" : values["SindacoSupplente2"].ToString().ToString().Replace("&", "&amp;").Replace("\"", "'")) + "\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);
                    
                XmlNode tmpNode = doctmp.SelectSingleNode("/CLIENTE");
                XmlNode cliente = document.ImportNode(tmpNode, true);
                    
                root.AppendChild(cliente);
                    
                root.Attributes["LastID"].Value = lastindex;
            }
            else
            {
				XmlNode xNode = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID='" + returnID.ToString() + "']");

                if (xNode.Attributes["Note"] == null)
                {
                    XmlAttribute attr = document.CreateAttribute("Note");
                    xNode.Attributes.Append(attr);
                }

                xNode.Attributes["Note"].Value = ((!values.Contains( "Note" )) ? "" : values["Note"].ToString());

                if (xNode.Attributes["EsercizioAl"] == null)
                {
                    XmlAttribute attr = document.CreateAttribute("EsercizioAl");
                    xNode.Attributes.Append(attr);
                }

                xNode.Attributes["EsercizioAl"].Value = ((!values.Contains( "EsercizioAl" )) ? "" : values["EsercizioAl"].ToString());

                if (xNode.Attributes["EsercizioDal"] == null)
                {
                    XmlAttribute attr = document.CreateAttribute("EsercizioDal");
                    xNode.Attributes.Append(attr);
                }

                xNode.Attributes["EsercizioDal"].Value = ((!values.Contains( "EsercizioDal" )) ? "" : values["EsercizioDal"].ToString());

                if (xNode.Attributes["Esercizio"] == null)
                {
                    XmlAttribute attr = document.CreateAttribute("Esercizio");
                    xNode.Attributes.Append(attr);
                }

                xNode.Attributes["Esercizio"].Value = ((!values.Contains( "Esercizio" )) ? "" : values["Esercizio"].ToString());

                if (xNode.Attributes["CodiceFiscale"] == null)
                {
                    XmlAttribute attr = document.CreateAttribute("CodiceFiscale");
                    xNode.Attributes.Append(attr);
                }

                xNode.Attributes["CodiceFiscale"].Value = ((!values.Contains( "CodiceFiscale" )) ? "" : values["CodiceFiscale"].ToString());

                if (xNode.Attributes["PartitaIVA"] == null)
                {
                    XmlAttribute attr = document.CreateAttribute("PartitaIVA");
                    xNode.Attributes.Append(attr);
                }

                xNode.Attributes["PartitaIVA"].Value = ((!values.Contains( "PartitaIVA" )) ? "" : values["PartitaIVA"].ToString());

                if (xNode.Attributes["RagioneSociale"] == null)
                {
                    XmlAttribute attr = document.CreateAttribute("RagioneSociale");
                    xNode.Attributes.Append(attr);
                }

                xNode.Attributes["RagioneSociale"].Value = ((!values.Contains( "RagioneSociale" )) ? "" : values["RagioneSociale"].ToString());

                if(xNode.Attributes["OrganoDiControllo"] == null)
                {
                    XmlAttribute attr = document.CreateAttribute("OrganoDiControllo");
                    xNode.Attributes.Append(attr);
                }

                xNode.Attributes["OrganoDiControllo"].Value = ((!values.Contains( "OrganoDiControllo" )) ? "" : values["OrganoDiControllo"].ToString());

                if(xNode.Attributes["OrganoDiRevisione"] == null)
                {
                    XmlAttribute attr = document.CreateAttribute("OrganoDiRevisione");
                    xNode.Attributes.Append(attr);
                }

                xNode.Attributes["OrganoDiRevisione"].Value = ((!values.Contains( "OrganoDiRevisione" )) ? "" : values["OrganoDiRevisione"].ToString());
                
                if(xNode.Attributes["Presidente"] == null)
                {
                    XmlAttribute attr = document.CreateAttribute("Presidente");
                    xNode.Attributes.Append(attr);
                }

                xNode.Attributes["Presidente"].Value = ((!values.Contains( "Presidente" )) ? "" : values["Presidente"].ToString());

                if(xNode.Attributes["MembroEffettivo"] == null)
                {
                    XmlAttribute attr = document.CreateAttribute("MembroEffettivo");
                    xNode.Attributes.Append(attr);
                }

                xNode.Attributes["MembroEffettivo"].Value = ((!values.Contains( "MembroEffettivo" )) ? "" : values["MembroEffettivo"].ToString());

                if(xNode.Attributes["MembroEffettivo2"] == null)
                {
                    XmlAttribute attr = document.CreateAttribute("MembroEffettivo2");
                    xNode.Attributes.Append(attr);
                }

                xNode.Attributes["MembroEffettivo2"].Value = ((!values.Contains( "MembroEffettivo2" )) ? "" : values["MembroEffettivo2"].ToString());

                if (xNode.Attributes["SindacoSupplente"] == null)
                {
                    XmlAttribute attr = document.CreateAttribute("SindacoSupplente");
                    xNode.Attributes.Append(attr);
                }

                xNode.Attributes["SindacoSupplente"].Value = ((!values.Contains("SindacoSupplente")) ? "" : values["SindacoSupplente"].ToString());

                if (xNode.Attributes["SindacoSupplente2"] == null)
                {
                    XmlAttribute attr = document.CreateAttribute("SindacoSupplente2");
                    xNode.Attributes.Append(attr);
                }

                xNode.Attributes["SindacoSupplente2"].Value = ((!values.Contains("SindacoSupplente2")) ? "" : values["SindacoSupplente2"].ToString());

                if (xNode.Attributes["RevisoreAutonomo"] == null)
                {
                    XmlAttribute attr = document.CreateAttribute("RevisoreAutonomo");
                    xNode.Attributes.Append(attr);
                }

                xNode.Attributes["RevisoreAutonomo"].Value = ((!values.Contains( "RevisoreAutonomo" )) ? "" : values["RevisoreAutonomo"].ToString());
            }

            Save();

            Close();            

			return returnID;
#endif
        }

#endregion

#region incarico

        public int GetIncarichiCount()
        {
            int result = 0;

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/INCARICHI/INCARICO" );

                if ( xNodes != null )
                {
                    result = xNodes.Count;
                }

                Close();
            }

            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }



		public ArrayList GetIncarichi(string IDCliente)
		{
			ArrayList results = new ArrayList();

			try
			{
				Open();

				XmlNodeList xNodes = document.SelectNodes("/ROOT/INCARICHI/INCARICO[@Cliente='" + IDCliente + "']");

				foreach (XmlNode node in xNodes)
				{
					Hashtable result = new Hashtable();

					foreach (XmlAttribute item in node.Attributes)
					{
						result.Add(item.Name, item.Value);
					}

					results.Add(result);
				}

				Close();
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}

			return results;
		}

		public Hashtable GetIncarico(string IDIncarico)
		{
			Hashtable result = new Hashtable();

			try
			{
				Open();

				XmlNode xNode = document.SelectSingleNode("/ROOT/INCARICHI/INCARICO[@ID='" + IDIncarico + "']");

				foreach (XmlAttribute item in xNode.Attributes)
				{
					result.Add(item.Name, item.Value);
				}

				Close();
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}

			return result;
		}

		public string AddIncarico(XmlNode node)
		{
			Open();

			XmlNode xNode = document.SelectSingleNode("/ROOT/INCARICHI");

			string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();

			node.Attributes["ID"].Value = ID;

			XmlNode xtmp = document.ImportNode(node, true);

			xNode.AppendChild(xtmp);

			xNode.Attributes["LastID"].Value = ID;

			Save();

			Close();

			return ID;
		}

		public void DeleteIncarico(int IDIncarico)
		{
			try
			{
				Open();

				XmlNode xNode = document.SelectSingleNode("/ROOT/INCARICHI/INCARICO[@ID='" + IDIncarico.ToString() + "']");

				FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
				if (fi.Exists)
				{
					fi.Delete();
				}

				FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
				if (fd.Exists)
				{
					fd.Delete();
				}

				xNode.ParentNode.RemoveChild(xNode);

				Save();

				//cancello allegati
				XmlManager xdoc = new XmlManager();
				xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
				XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);

                XmlNodeList xNodes = xdoc_doc.SelectNodes( "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDIncarico + "'][@Tree='" + (Convert.ToInt32( App.TipoFile.Incarico )).ToString() + "']" );

				foreach (XmlNode node in xNodes)
				{
					FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
					if (fis.Exists)
					{
						fis.Delete();
					}

					node.ParentNode.RemoveChild(node);
				}

				xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);
				
				Close();
			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
			}
		}

		public bool CheckDoppio_incarico(int ID, int IDCliente, string Data)
		{
#if (DBG_TEST)
      string query;

      query = String.Format(
        @"select idOggetto from Oggetti "
        + @"where (idCliente={0}) and (idTipoFile=3) and (dataNomina='{1}') "
          + @"and (idOggetto<>{2})",IDCliente,Data,ID);
      SqlDataAdapter da = new SqlDataAdapter(query, App.connString);
      DataTable dataTable = new DataTable(); da.Fill(dataTable);
      return dataTable.Rows.Count<1;
#else
			Open();

			XmlNodeList xNodes = document.SelectNodes("/ROOT/INCARICHI/INCARICO");
			foreach (XmlNode node in xNodes)
			{
				if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString()
          && node.Attributes["DataNomina"].Value == Data)
				{
					Close();
					return false;
				}
			}

			Close();
			return true;
#endif
		}

		public int SetIncarico(Hashtable values, int IDIncarico, int IDCliente)
		{
#if (DBG_TEST)
      string query;
      if (IDIncarico == App.MasterFile_NewID)
      {
        // IDIncarico = nuovo id valido
        query = string.Format(
          @"select max(idOggetto) as max_idOggetto "+
          @"from Oggetti where(idCliente = {0}) and(idTipoFile = {1})",
          IDCliente, App.TipiOggetto.INCARICO);
        SqlDataAdapter da = new SqlDataAdapter(query, App.connString);
        DataTable dataTable = new DataTable(); da.Fill(dataTable);
        IDIncarico = 1+Convert.ToInt32(dataTable.Rows[0].ItemArray[0].ToString());
        //----------------------------------------------------------------------------+
        //   genera un nuovo file per l' albero eseguendo la procedura seguente:      |
        //   [1] costruzione percorso completo template tree incarico                 |
        //       --> App.AppTemplateTreeIncarico =                                    |
        //         <User\AppData\Roaming\revisoft\revisoft>\Template\Incarico.ridf    |
        //   [2] generazione nome nuovo file                                          |
        //       --> newNametree=                                                     |
        //         <User\AppData\Roaming\revisoft\revisoft>\<newGUID>.ridf            |
        //       se il file esiste già, si riprova con un newGUID diverso             |
        //   [3] copia del template su nuovo file con nome calcolato al punto [2]     |
        //       <User\AppData\Roaming\revisoft\revisoft>\Template\Incarico.ridf      |
        //       copiato su                                                           |
        //       <User\AppData\Roaming\revisoft\revisoft>\<newGUID>.ridf              |
        //   [4] rimozione prefisso                                                   |
        //       <User\AppData\Roaming\revisoft\revisoft>\<newGUID>.ridf              |
        //       --> <newGUID>.ridf                                                   |
        //----------------------------------------------------------------------------+
        //----------------------------------------------------------------------------+
        //  genera un nuovo file per i dati così:                                     |
        //  [1] costruzione percorso completo template dati incarico                  |
        //      --> App.AppTemplateDataIncarico =                                     |
        //        <User\AppData\Roaming\revisoft\revisoft\template>\DatiIncarico.ridf |
        //  [2] generazione nome nuovo file                                           |
        //      --> newNamedati =                                                     |
        //        <User\AppData\Roaming\revisoft\revisoft\RDF>\<newGUID>.ridf         |
        //        se il file esiste già, si riprova con un newGUID diverso            |
        //  [3] copia del template su nuovo file con nome calcolato al punto [2]      |
        //      <User\AppData\Roaming\revisoft\revisoft\template>\DatiIncarico.ridf   |
        //      copiato su                                                            |
        //      <User\AppData\Roaming\revisoft\revisoft\RDF>\<newGUID>.ridf           |
        //  [4] rimozione prefisso                                                    |
        //      <User\AppData\Roaming\revisoft\revisoft\RDF>\<newGUID>.ridf           |
        //      --> <newGUID>.ridf                                                    |
        //----------------------------------------------------------------------------+
      }
      //string query;
      //int i;

      //query = String.Format(@"select clienteFissato from Revisoft");
      //SqlDataAdapter da = new SqlDataAdapter(query, App.connString);
      //DataTable dataTable = new DataTable(); da.Fill(dataTable);
      //i = dataTable.Rows.Count; if (i < 1) return null;
      //return dataTable.Rows[0].ItemArray[0].ToString();

      //string query;

      //query = String.Format("update [Revisoft] set [clienteFissato]={0}", ID);
      //App.sqlConnection.Open();
      //SqlCommand cmd = new SqlCommand(query, App.sqlConnection);
      //try { cmd.ExecuteNonQuery(); }
      //catch (SqlException e) { MessageBox.Show(e.Message); }
      //finally { App.sqlConnection.Close(); }
      return IDIncarico;
#else
			try
			{
				Open();

				if (IDIncarico == App.MasterFile_NewID)
				{
					XmlNode root = document.SelectSingleNode("/ROOT/INCARICHI");

					IDIncarico = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

					string lastindex = IDIncarico.ToString();

					//Template TREE
					FileInfo fitree = new FileInfo(App.AppTemplateTreeIncarico);
					string estensione = "." + App.AppTemplateTreeIncarico.Split('.').Last();
					string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
					FileInfo fnewtree = new FileInfo(newNametree);

					while (fnewtree.Exists)
					{
						newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
						fnewtree = new FileInfo(newNametree);
					}

					fitree.CopyTo(newNametree);
					newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

					//Template Dati
					FileInfo fidati = new FileInfo(App.AppTemplateDataIncarico);
					string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
					FileInfo fnewdati = new FileInfo(newNamedati);

					while (fnewdati.Exists)
					{
						newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
						fnewdati = new FileInfo(newNamedati);
					}

					fidati.CopyTo(newNamedati);
					newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");


          string xml = "<INCARICO ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ( (int)( App.TipoSessioneStato.Disponibile ) ).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" DataNomina=\"" + values["DataNomina"].ToString() + "\" Composizione=\"" + values["Composizione"].ToString() + "\" Attivita=\"" + values["Attivita"].ToString() + "\"  />";
					XmlDocument doctmp = new XmlDocument();
					doctmp.LoadXml(xml);

					XmlNode tmpNode = doctmp.SelectSingleNode("/INCARICO");
					XmlNode cliente = document.ImportNode(tmpNode, true);

					root.AppendChild(cliente);

					root.Attributes["LastID"].Value = lastindex;
				}
				else
				{
					XmlNode xNode = document.SelectSingleNode("/ROOT/INCARICHI/INCARICO[@ID='" + IDIncarico.ToString() + "']");

					xNode.Attributes["Note"].Value = values["Note"].ToString();
					xNode.Attributes["DataNomina"].Value = values["DataNomina"].ToString();
					xNode.Attributes["Composizione"].Value = values["Composizione"].ToString();
					xNode.Attributes["Attivita"].Value = values["Attivita"].ToString(); ;
				}

				Save();

				Close();

			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
			}

			return IDIncarico;
#endif
		}

        public int SetSigilloIncarico( int IDIncarico, string revisore, string password )
        {
            try
            {
                Open();

                if ( IDIncarico == App.MasterFile_NewID )
                {
                    ;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode( "/ROOT/INCARICHI/INCARICO[@ID='" + IDIncarico.ToString() + "']" );

                    if ( xNode.Attributes["Sigillo"] == null )
                    {
                        xNode.Attributes.Append( document.CreateAttribute( "Sigillo" ) );
                    }
                    
                    if ( xNode.Attributes["Sigillo_Password"] == null )
                    {
                        xNode.Attributes.Append( document.CreateAttribute( "Sigillo_Password" ) );
                    }

                    if ( xNode.Attributes["Sigillo_Data"] == null )
                    {
                        xNode.Attributes.Append( document.CreateAttribute( "Sigillo_Data" ) );
                    }

                    xNode.Attributes["Sigillo"].Value = revisore;
                    xNode.Attributes["Sigillo_Password"].Value = password;
                    xNode.Attributes["Sigillo_Data"].Value = DateTime.Now.ToShortDateString();
                }

                Save();

                Close();

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster );
            }

            return IDIncarico;
        }

        public int RemoveSigilloIncarico( int IDIncarico)
        {
            try
            {
                Open();

                if ( IDIncarico == App.MasterFile_NewID )
                {
                    ;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode( "/ROOT/INCARICHI/INCARICO[@ID='" + IDIncarico.ToString() + "']" );

                    if ( xNode.Attributes["Sigillo"] != null )
                    {
                        xNode.Attributes.Remove( xNode.Attributes["Sigillo"] );
                    }

                    if ( xNode.Attributes["Sigillo_Password"] != null )
                    {
                        xNode.Attributes.Remove( xNode.Attributes["Sigillo_Password"] );
                    }

                    if ( xNode.Attributes["Sigillo_Data"] != null )
                    {
                        xNode.Attributes.Remove( xNode.Attributes["Sigillo_Data"] );
                    }
                }

                Save();

                Close();

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster );
            }

            return IDIncarico;
        }
#endregion

#region ISQC

        public int GetISQCsCount()
        {
            int result = 0;

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes("/ROOT/ISQCs/ISQC");

                if (xNodes != null)
                {
                    result = xNodes.Count;
                }

                Close();
            }

            catch (Exception ex)
            {
                string log = ex.Message;
            }

            return result;
        }



        public ArrayList GetISQCs(string IDCliente)
        {
            ArrayList results = new ArrayList();

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes("/ROOT/ISQCs/ISQC[@Cliente='" + IDCliente + "']");

                foreach (XmlNode node in xNodes)
                {
                    Hashtable result = new Hashtable();

                    foreach (XmlAttribute item in node.Attributes)
                    {
                        result.Add(item.Name, item.Value);
                    }

                    results.Add(result);
                }

                Close();
            }
            catch (Exception ex)
            {
                string log = ex.Message;
            }

            return results;
        }

        public Hashtable GetISQC(string IDISQC)
        {
            Hashtable result = new Hashtable();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode("/ROOT/ISQCs/ISQC[@ID='" + IDISQC + "']");

                foreach (XmlAttribute item in xNode.Attributes)
                {
                    result.Add(item.Name, item.Value);
                }

                Close();
            }
            catch (Exception ex)
            {
                string log = ex.Message;
            }

            return result;
        }

        public string AddISQC(XmlNode node)
        {
            Open();

            XmlNode xNode = document.SelectSingleNode("/ROOT/ISQCs");

            if (document.SelectNodes("/ROOT/ISQCs") == null || document.SelectNodes("/ROOT/ISQCs").Count == 0)
            {
                string xmlISQC = "<ISQCs LastID=\"1\" />";
                XmlDocument doctmpISQC = new XmlDocument();
                doctmpISQC.LoadXml(xmlISQC);

                XmlNode tmpNodeISQC = doctmpISQC.SelectSingleNode("/ISQCs");
                XmlNode clienteISQC = document.ImportNode(tmpNodeISQC, true);

                document.SelectSingleNode("/ROOT").AppendChild(clienteISQC);

                xNode = document.SelectSingleNode("/ROOT/ISQCs");
            }

            string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();

            node.Attributes["ID"].Value = ID;

            XmlNode xtmp = document.ImportNode(node, true);

            xNode.AppendChild(xtmp);

            xNode.Attributes["LastID"].Value = ID;

            Save();

            Close();

            return ID;
        }

        public void DeleteISQC(int IDISQC)
        {
            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode("/ROOT/ISQCs/ISQC[@ID='" + IDISQC.ToString() + "']");

                FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
                if (fi.Exists)
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
                if (fd.Exists)
                {
                    fd.Delete();
                }

                xNode.ParentNode.RemoveChild(xNode);

                Save();

                //cancello allegati
                XmlManager xdoc = new XmlManager();
                xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
                XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);

                XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDISQC + "'][@Tree='" + (Convert.ToInt32(App.TipoFile.ISQC)).ToString() + "']");

                foreach (XmlNode node in xNodes)
                {
                    FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
                    if (fis.Exists)
                    {
                        fis.Delete();
                    }

                    node.ParentNode.RemoveChild(node);
                }

                xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);

                Close();
            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
            }
        }

        public bool CheckDoppio_ISQC(int ID, int IDCliente, string Data)
        {
            Open();

            XmlNodeList xNodes = document.SelectNodes("/ROOT/ISQCs/ISQC");
            foreach (XmlNode node in xNodes)
            {
                if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["DataNomina"].Value == Data)
                {
                    Close();
                    return false;
                }
            }

            Close();
            return true;
        }

        public int SetISQC(Hashtable values, int IDISQC, int IDCliente)
        {
            try
            {
                Open();

                if (IDISQC == App.MasterFile_NewID)
                {
                    XmlNode root = document.SelectSingleNode("/ROOT/ISQCs");

                    if (document.SelectNodes("/ROOT/ISQCs") == null || document.SelectNodes("/ROOT/ISQCs").Count == 0)
                    {
                        string xmlISQC = "<ISQCs LastID=\"1\" />";
                        XmlDocument doctmpISQC = new XmlDocument();
                        doctmpISQC.LoadXml(xmlISQC);

                        XmlNode tmpNodeISQC = doctmpISQC.SelectSingleNode("/ISQCs");
                        XmlNode clienteISQC = document.ImportNode(tmpNodeISQC, true);

                        document.SelectSingleNode("/ROOT").AppendChild(clienteISQC);

                        root = document.SelectSingleNode("/ROOT/ISQCs");
                    }

                    IDISQC = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

                    string lastindex = IDISQC.ToString();

                    //Template TREE
                    FileInfo fitree = new FileInfo(App.AppTemplateTreeISQC);
                    string estensione = "." + App.AppTemplateTreeISQC.Split('.').Last();
                    string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewtree = new FileInfo(newNametree);

                    while (fnewtree.Exists)
                    {
                        newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewtree = new FileInfo(newNametree);
                    }

                    fitree.CopyTo(newNametree);
                    newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

                    //Template Dati
                    FileInfo fidati = new FileInfo(App.AppTemplateDataISQC);
                    string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewdati = new FileInfo(newNamedati);

                    while (fnewdati.Exists)
                    {
                        newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewdati = new FileInfo(newNamedati);
                    }

                    fidati.CopyTo(newNamedati);
                    newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");


                    string xml = "<ISQC ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" DataNomina=\"" + values["DataNomina"].ToString() + "\" DataFine=\"" + ((values["DataFine"] != null)? values["DataFine"].ToString() : "") + "\" Composizione=\"" + values["Composizione"].ToString() + "\" Attivita=\"" + values["Attivita"].ToString() + "\"  />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml(xml);

                    XmlNode tmpNode = doctmp.SelectSingleNode("/ISQC");
                    XmlNode cliente = document.ImportNode(tmpNode, true);

                    root.AppendChild(cliente);

                    root.Attributes["LastID"].Value = lastindex;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode("/ROOT/ISQCs/ISQC[@ID='" + IDISQC.ToString() + "']");

                    xNode.Attributes["Note"].Value = values["Note"].ToString();
                    xNode.Attributes["DataNomina"].Value = values["DataNomina"].ToString();
                    if (values["DataFine"] != null)
                    {
                        if (xNode.Attributes["DataFine"] == null)
                        {
                            xNode.Attributes.Append(document.CreateAttribute("DataFine"));
                        }
                        xNode.Attributes["DataFine"].Value = values["DataFine"].ToString();
                    }
                    xNode.Attributes["Composizione"].Value = values["Composizione"].ToString();
                    xNode.Attributes["Attivita"].Value = values["Attivita"].ToString(); ;
                }

                Save();

                Close();

            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
            }

            return IDISQC;
        }

        public int SetSigilloISQC(int IDISQC, string revisore, string password)
        {
            try
            {
                Open();

                if (IDISQC == App.MasterFile_NewID)
                {
                    ;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode("/ROOT/ISQCs/ISQC[@ID='" + IDISQC.ToString() + "']");

                    if (xNode.Attributes["Sigillo"] == null)
                    {
                        xNode.Attributes.Append(document.CreateAttribute("Sigillo"));
                    }

                    if (xNode.Attributes["Sigillo_Password"] == null)
                    {
                        xNode.Attributes.Append(document.CreateAttribute("Sigillo_Password"));
                    }

                    if (xNode.Attributes["Sigillo_Data"] == null)
                    {
                        xNode.Attributes.Append(document.CreateAttribute("Sigillo_Data"));
                    }

                    xNode.Attributes["Sigillo"].Value = revisore;
                    xNode.Attributes["Sigillo_Password"].Value = password;
                    xNode.Attributes["Sigillo_Data"].Value = DateTime.Now.ToShortDateString();
                }

                Save();

                Close();

            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
            }

            return IDISQC;
        }

        public int RemoveSigilloISQC(int IDISQC)
        {
            try
            {
                Open();

                if (IDISQC == App.MasterFile_NewID)
                {
                    ;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode("/ROOT/ISQCs/ISQC[@ID='" + IDISQC.ToString() + "']");

                    if (xNode.Attributes["Sigillo"] != null)
                    {
                        xNode.Attributes.Remove(xNode.Attributes["Sigillo"]);
                    }

                    if (xNode.Attributes["Sigillo_Password"] != null)
                    {
                        xNode.Attributes.Remove(xNode.Attributes["Sigillo_Password"]);
                    }

                    if (xNode.Attributes["Sigillo_Data"] != null)
                    {
                        xNode.Attributes.Remove(xNode.Attributes["Sigillo_Data"]);
                    }
                }

                Save();

                Close();

            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
            }

            return IDISQC;
        }
#endregion

#region Revisione

        public int GetRevisioneCount()
        {
            int result = 0;

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/REVISIONI/REVISIONE" );

                if ( xNodes != null )
                {
                    result = xNodes.Count;
                }

                Close();
            }

            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }


		public ArrayList GetRevisioni(string IDCliente)
		{
			ArrayList results = new ArrayList();

			try
			{
				Open();

				XmlNodeList xNodes = document.SelectNodes("/ROOT/REVISIONI/REVISIONE[@Cliente='" + IDCliente + "']");

				foreach (XmlNode node in xNodes)
				{
					Hashtable result = new Hashtable();

					foreach (XmlAttribute item in node.Attributes)
					{
						result.Add(item.Name, item.Value);
					}

					results.Add(result);
				}

				Close();
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}

			return results;
		}

		public Hashtable GetRevisione(string IDRevisione)
		{
			Hashtable result = new Hashtable();

			try
			{
				Open();

				XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@ID='" + IDRevisione + "']");

				foreach (XmlAttribute item in xNode.Attributes)
				{
					result.Add(item.Name, item.Value);
				}

				Close();
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}

			return result;
		}

        public Hashtable GetRevisioneFromFileData( string FileSessione )
        {
            Hashtable result = new Hashtable();

            FileSessione = FileSessione.Split( '\\' ).Last();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode( "/ROOT/REVISIONI/REVISIONE[@FileData='" + FileSessione + "']" );

                foreach ( XmlAttribute item in xNode.Attributes )
                {
                    result.Add( item.Name, item.Value );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

		public string AddRevisione(XmlNode node)
		{
			Open();

			XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI");

			string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();

			node.Attributes["ID"].Value = ID;

			XmlNode xtmp = document.ImportNode(node, true);

			xNode.AppendChild(xtmp);

			xNode.Attributes["LastID"].Value = ID;

			Save();

			Close();

			return ID;
		}

		public void DeleteRevisione(int IDRevisione)
		{
			try
			{
				Open();

				XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@ID='" + IDRevisione.ToString() + "']");

				FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
				if (fi.Exists)
				{
					fi.Delete();
				}

				FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
				if (fd.Exists)
				{
					fd.Delete();
				}

				xNode.ParentNode.RemoveChild(xNode);

				Save();

				//cancello allegati
				XmlManager xdoc = new XmlManager();
				xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
				XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);

                XmlNodeList xNodes = xdoc_doc.SelectNodes( "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDRevisione + "'][@Tree='" + (Convert.ToInt32( App.TipoFile.Revisione )).ToString() + "']" );

				foreach (XmlNode node in xNodes)
				{
					FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
					if (fis.Exists)
					{
						fis.Delete();
					}

					node.ParentNode.RemoveChild(node);
				}

				xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);

				Close();

			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
			}
		}

		public bool CheckDoppio_Revisione(int ID, int IDCliente, string Data)
		{
			Open();

			XmlNodeList xNodes = document.SelectNodes("/ROOT/REVISIONI/REVISIONE");
			foreach (XmlNode node in xNodes)
			{
				if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data)
				{
					Close();
					return false;
				}
			}

			Close();
			return true;
		}

        public int SetRevisioneIntermedio(Hashtable values, int IDCliente, string dal, string al)
        {
            int IDRevisione = -1;

            try
            {
                Open();

                XmlNode root = document.SelectSingleNode("/ROOT/REVISIONI");

                IDRevisione = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

                string lastindex = IDRevisione.ToString();

                //Template TREE
                FileInfo fitree = new FileInfo(App.AppTemplateTreeRevisione);
                string estensione = "." + App.AppTemplateTreeRevisione.Split('.').Last();
                string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                FileInfo fnewtree = new FileInfo(newNametree);

                while (fnewtree.Exists)
                {
                    newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    fnewtree = new FileInfo(newNametree);
                }

                fitree.CopyTo(newNametree);
                newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

                //Template Dati
                FileInfo fidati = new FileInfo(App.AppTemplateDataRevisione);
                string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                FileInfo fnewdati = new FileInfo(newNamedati);

                while (fnewdati.Exists)
                {
                    newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    fnewdati = new FileInfo(newNamedati);
                }

                fidati.CopyTo(newNamedati);
                newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");

                XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

                string xml = "<REVISIONE ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);

                XmlNode tmpNode = doctmp.SelectSingleNode("/REVISIONE");
                XmlNode cliente = document.ImportNode(tmpNode, true);

                root.AppendChild(cliente);

                root.Attributes["LastID"].Value = lastindex;

                Save();

                Close();

            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
            }

            return IDRevisione;
        }

        public int SetRevisione(Hashtable values, int IDRevisione, int IDCliente)
		{
			try
			{
				Open();

				if (IDRevisione == App.MasterFile_NewID)
				{
					XmlNode root = document.SelectSingleNode("/ROOT/REVISIONI");

					IDRevisione = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

					string lastindex = IDRevisione.ToString();

					//Template TREE
					FileInfo fitree = new FileInfo(App.AppTemplateTreeRevisione);
					string estensione = "." + App.AppTemplateTreeRevisione.Split('.').Last();
					string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
					FileInfo fnewtree = new FileInfo(newNametree);

					while (fnewtree.Exists)
					{
						newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
						fnewtree = new FileInfo(newNametree);
					}

					fitree.CopyTo(newNametree);
					newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

					//Template Dati
					FileInfo fidati = new FileInfo(App.AppTemplateDataRevisione);
					string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
					FileInfo fnewdati = new FileInfo(newNamedati);

					while (fnewdati.Exists)
					{
						newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
						fnewdati = new FileInfo(newNamedati);
					}

					fidati.CopyTo(newNamedati);
					newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");
                    
                    XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

                    string xml = "<REVISIONE ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ( (int)( App.TipoSessioneStato.Disponibile ) ).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
					XmlDocument doctmp = new XmlDocument();
					doctmp.LoadXml(xml);

					XmlNode tmpNode = doctmp.SelectSingleNode("/REVISIONE");
					XmlNode cliente = document.ImportNode(tmpNode, true);

					root.AppendChild(cliente);

					root.Attributes["LastID"].Value = lastindex;
				}
				else
				{
					XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@ID='" + IDRevisione.ToString() + "']");

					xNode.Attributes["Note"].Value = values["Note"].ToString();
					xNode.Attributes["Data"].Value = values["Data"].ToString();
				}

				Save();

				Close();

			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
			}

			return IDRevisione;
		}

        public string GetRevisioneAssociataFromConclusioneFile( string FileConclusione )
        {
            string FileRevisione = "";

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/CONCLUSIONI/CONCLUSIONE[@FileData='" + FileConclusione.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetRevisioni( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            FileRevisione = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileRevisione;
        }

        public string GetRevisioneFromEsercizio( string Cliente, string Esercizio )
        {
            string FileRevisione = "";

            Open();

            ArrayList al = GetRevisioni( Cliente );

            foreach ( Hashtable item in al )
            {
                if ( item["Data"].ToString() == "01/01/" + Esercizio || item["Data"].ToString() == "31/12/" + Esercizio)
                {
                    FileRevisione = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
                    break;
                }
            }

            Close();

            return FileRevisione;
        }

		public string GetRevisioneAssociataFromBilancioFile(string FileBilancio)
		{
			string FileRevisione = "";

			Open();

			XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@FileData='" + FileBilancio.Split('\\').Last() + "']");

			if (xNode != null)
			{
				if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
				{
					ArrayList al = GetRevisioni(xNode.Attributes["Cliente"].Value);

					foreach (Hashtable item in al)
					{
						if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
						{
							FileRevisione = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
							break;
						}
					}
				}
			}

			Close();

			return FileRevisione;
		}

		public string GetBilancioAssociatoFromRevisioneFile(string FileRevisione)
		{
			string FileBilancio = "";

			Open();

			XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@FileData='" + FileRevisione.Split('\\').Last() + "']");

			if (xNode != null)
			{
				if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
				{
					ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);

					foreach (Hashtable item in al)
					{
						if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
						{
							FileBilancio = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
							break;
						}
					}
				}
			}

			Close();

			return FileBilancio;
		}

        public string GetBilancioTreeAssociatoFromRevisioneFile( string FileRevisione )
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/REVISIONI/REVISIONE[@FileData='" + FileRevisione.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetBilanci( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            FileBilancio = App.AppDataDataFolder + "\\" + item["File"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public string GetBilancioIDAssociatoFromRevisioneFile( string FileRevisione )
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/REVISIONI/REVISIONE[@FileData='" + FileRevisione.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetBilanci( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            FileBilancio =  item["ID"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public string GetBilancioAssociatoFromConclusioneFile(string FileConclusione)
		{
			string FileBilancio = "";

			Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/CONCLUSIONI/CONCLUSIONE[@FileData='" + FileConclusione.Split( '\\' ).Last() + "']" );

			if (xNode != null)
			{
				if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
				{
					ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);

					foreach (Hashtable item in al)
					{
						if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
						{
							FileBilancio = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
							break;
						}
					}
				}
			}

			Close();

			return FileBilancio;
		}

        public string GetBilancioTreeAssociatoFromConclusioneFile( string FileConclusione )
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/CONCLUSIONI/CONCLUSIONE[@FileData='" + FileConclusione.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetBilanci( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            FileBilancio = App.AppDataDataFolder + "\\" + item["File"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public string GetBilancioIDAssociatoFromConclusioneFile( string FileConclusione )
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/CONCLUSIONI/CONCLUSIONE[@FileData='" + FileConclusione.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetBilanci( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            FileBilancio = item["ID"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

		public Hashtable GetAllRevisioneAssociataFromBilancioFile(string FileBilancio)
		{
			Hashtable Revisione = null;

			Open();

			XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@FileData='" + FileBilancio.Split('\\').Last() + "']");

			if (xNode != null)
			{
				if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
				{
					ArrayList al = GetRevisioni(xNode.Attributes["Cliente"].Value);

					foreach (Hashtable item in al)
					{
						if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
						{
							Revisione = item;
							break;
						}
					}
				}
			}

			Close();

			return Revisione;
		}

		public Hashtable GetAllBilancioAssociatoFromRevisioneFile(string FileRevisione)
		{
			Hashtable Bilancio = null;

			Open();

			XmlNode xNode = document.SelectSingleNode("/ROOT/REVISIONI/REVISIONE[@FileData='" + FileRevisione.Split('\\').Last() + "']");

			if (xNode != null)
			{
				if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
				{
					ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);

					foreach (Hashtable item in al)
					{
						if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
						{
							Bilancio = item;
							break;
						}
					}
				}
			}

			Close();

			return Bilancio;
		}

        public int SetSigilloRevisione( int IDRevisione, string revisore, string password )
        {
            try
            {
                Open();

                if ( IDRevisione == App.MasterFile_NewID )
                {
                    ;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode( "/ROOT/REVISIONI/REVISIONE[@ID='" + IDRevisione.ToString() + "']" );

                    if ( xNode.Attributes["Sigillo"] == null )
                    {
                        xNode.Attributes.Append( document.CreateAttribute( "Sigillo" ) );
                    }

                    if ( xNode.Attributes["Sigillo_Password"] == null )
                    {
                        xNode.Attributes.Append( document.CreateAttribute( "Sigillo_Password" ) );
                    }

                    if ( xNode.Attributes["Sigillo_Data"] == null )
                    {
                        xNode.Attributes.Append( document.CreateAttribute( "Sigillo_Data" ) );
                    }

                    xNode.Attributes["Sigillo"].Value = revisore;
                    xNode.Attributes["Sigillo_Password"].Value = password;
                    xNode.Attributes["Sigillo_Data"].Value = DateTime.Now.ToShortDateString();
                }

                Save();

                Close();

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster );
            }

            return IDRevisione;
        }

        public int RemoveSigilloRevisione( int IDRevisione )
        {
            try
            {
                Open();

                if ( IDRevisione == App.MasterFile_NewID )
                {
                    ;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode( "/ROOT/REVISIONI/REVISIONE[@ID='" + IDRevisione.ToString() + "']" );

                    if ( xNode.Attributes["Sigillo"] != null )
                    {
                        xNode.Attributes.Remove(xNode.Attributes["Sigillo"]);
                    }

                    if ( xNode.Attributes["Sigillo_Password"] != null )
                    {
                        xNode.Attributes.Remove(xNode.Attributes["Sigillo_Password"]);
                    }

                    if ( xNode.Attributes["Sigillo_Data"] != null )
                    {
                        xNode.Attributes.Remove(xNode.Attributes["Sigillo_Data"]);
                    }
                }

                Save();

                Close();

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster );
            }

            return IDRevisione;
        }
#endregion

#region Bilancio

        public int GetBilanciCount()
        {
            int result = 0;

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/BILANCI/BILANCIO" );

                if ( xNodes != null )
                {
                    result = xNodes.Count;
                }

                Close();
            }

            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

		public ArrayList GetBilanci(string IDCliente)
		{
			ArrayList results = new ArrayList();

			try
			{
				Open();

				XmlNodeList xNodes = document.SelectNodes("/ROOT/BILANCI/BILANCIO[@Cliente='" + IDCliente + "']");

				foreach (XmlNode node in xNodes)
				{
					Hashtable result = new Hashtable();

					foreach (XmlAttribute item in node.Attributes)
					{
						result.Add(item.Name, item.Value);
					}

					results.Add(result);
				}

				Close();
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}

			return results;
		}

		public Hashtable GetBilancio(string IDBilancio)
		{
			Hashtable result = new Hashtable();

			try
			{
				Open();

				XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@ID='" + IDBilancio + "']");

				foreach (XmlAttribute item in xNode.Attributes)
				{
					result.Add(item.Name, item.Value);
				}

				Close();
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}

			return result;
		}

		public string AddBilancio(XmlNode node)
		{
			Open();

			XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI");

			string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();

			node.Attributes["ID"].Value = ID;

			XmlNode xtmp = document.ImportNode(node, true);

			xNode.AppendChild(xtmp);

			xNode.Attributes["LastID"].Value = ID;

			Save();

			Close();

			return ID;
		}

		public void DeleteBilancio(int IDBilancio)
		{
			try
			{
				Open();

				XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@ID='" + IDBilancio.ToString() + "']");

				FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
				if (fi.Exists)
				{
					fi.Delete();
				}

				FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
				if (fd.Exists)
				{
					fd.Delete();
				}

				xNode.ParentNode.RemoveChild(xNode);

				Save();

				//cancello allegati
				XmlManager xdoc = new XmlManager();
				xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
				XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);

                XmlNodeList xNodes = xdoc_doc.SelectNodes( "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDBilancio + "'][@Tree='" + (Convert.ToInt32( App.TipoFile.Bilancio )).ToString() + "']" );

				foreach (XmlNode node in xNodes)
				{
					FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
					if (fis.Exists)
					{
						fis.Delete();
					}

					node.ParentNode.RemoveChild(node);
				}

				xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);

				Close();
			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
			}
		}

		public bool CheckDoppio_Bilancio(int ID, int IDCliente, string Data)
		{
			Open();

			XmlNodeList xNodes = document.SelectNodes("/ROOT/BILANCI/BILANCIO");
			foreach (XmlNode node in xNodes)
			{
				if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data)
				{
					Close();
					return false;
				}
			}

			Close();
			return true;
		}
        
        public int SetBilancioIntermedio(Hashtable values, int IDCliente, string dal, string al)
        {
            int IDBilancio = -1;
            try
            {
                Open();

                 XmlNode root = document.SelectSingleNode("/ROOT/BILANCI");

                    IDBilancio = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

                    string lastindex = IDBilancio.ToString();

                    //Template TREE
                    FileInfo fitree = new FileInfo(App.AppTemplateTreeBilancio);
                    string estensione = "." + App.AppTemplateTreeBilancio.Split('.').Last();
                    string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewtree = new FileInfo(newNametree);

                    while (fnewtree.Exists)
                    {
                        newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewtree = new FileInfo(newNametree);
                    }

                    fitree.CopyTo(newNametree);
                    newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

                    //Template Dati
                    FileInfo fidati = new FileInfo(App.AppTemplateDataBilancio);
                    string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewdati = new FileInfo(newNamedati);

                    while (fnewdati.Exists)
                    {
                        newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewdati = new FileInfo(newNamedati);
                    }

                    fidati.CopyTo(newNamedati);
                    newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");

                    XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

                    string xml = "<BILANCIO ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\" Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\"  />";

                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml(xml);

                    XmlNode tmpNode = doctmp.SelectSingleNode("/BILANCIO");
                    XmlNode cliente = document.ImportNode(tmpNode, true);

                    root.AppendChild(cliente);

                    root.Attributes["LastID"].Value = lastindex;
               

                Save();

                Close();

            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
            }

            return IDBilancio;
        }


        public int SetBilancio(Hashtable values, int IDBilancio, int IDCliente)
		{
			try
			{
				Open();

				if (IDBilancio == App.MasterFile_NewID)
				{
					XmlNode root = document.SelectSingleNode("/ROOT/BILANCI");

					IDBilancio = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

					string lastindex = IDBilancio.ToString();

					//Template TREE
					FileInfo fitree = new FileInfo(App.AppTemplateTreeBilancio);
					string estensione = "." + App.AppTemplateTreeBilancio.Split('.').Last();
					string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
					FileInfo fnewtree = new FileInfo(newNametree);

					while (fnewtree.Exists)
					{
						newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
						fnewtree = new FileInfo(newNametree);
					}

					fitree.CopyTo(newNametree);
					newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

					//Template Dati
					FileInfo fidati = new FileInfo(App.AppTemplateDataBilancio);
					string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
					FileInfo fnewdati = new FileInfo(newNamedati);

					while (fnewdati.Exists)
					{
						newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
						fnewdati = new FileInfo(newNamedati);
					}

					fidati.CopyTo(newNamedati);
					newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");

                    XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

                    string xml = "<BILANCIO ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\" Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\"  />";

					XmlDocument doctmp = new XmlDocument();
					doctmp.LoadXml(xml);

					XmlNode tmpNode = doctmp.SelectSingleNode("/BILANCIO");
					XmlNode cliente = document.ImportNode(tmpNode, true);

					root.AppendChild(cliente);

					root.Attributes["LastID"].Value = lastindex;
				}
				else
				{
					XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@ID='" + IDBilancio.ToString() + "']");

					xNode.Attributes["Note"].Value = values["Note"].ToString();
					xNode.Attributes["Data"].Value = values["Data"].ToString();
				}

				Save();

				Close();

			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
			}

			return IDBilancio;
		}

        public int SetSigilloBilancio( int IDBilancio, string revisore, string password )
        {
            try
            {
                Open();

                if ( IDBilancio == App.MasterFile_NewID )
                {
                    ;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode( "/ROOT/BILANCI/BILANCIO[@ID='" + IDBilancio.ToString() + "']" );

                    if ( xNode.Attributes["Sigillo"] == null )
                    {
                        xNode.Attributes.Append( document.CreateAttribute( "Sigillo" ) );
                    }

                    if ( xNode.Attributes["Sigillo_Password"] == null )
                    {
                        xNode.Attributes.Append( document.CreateAttribute( "Sigillo_Password" ) );
                    }

                    if ( xNode.Attributes["Sigillo_Data"] == null )
                    {
                        xNode.Attributes.Append( document.CreateAttribute( "Sigillo_Data" ) );
                    }

                    xNode.Attributes["Sigillo"].Value = revisore;
                    xNode.Attributes["Sigillo_Password"].Value = password;
                    xNode.Attributes["Sigillo_Data"].Value = DateTime.Now.ToShortDateString();
                }

                Save();

                Close();

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster );
            }

            return IDBilancio;
        }

        public int RemoveSigilloBilancio( int IDBilancio )
        {
            try
            {
                Open();

                if ( IDBilancio == App.MasterFile_NewID )
                {
                    ;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode( "/ROOT/BILANCI/BILANCIO[@ID='" + IDBilancio.ToString() + "']" );

                    if ( xNode.Attributes["Sigillo"] != null )
                    {
                        xNode.Attributes.Remove(xNode.Attributes["Sigillo"]);
                    }

                    if ( xNode.Attributes["Sigillo_Password"] != null )
                    {
                        xNode.Attributes.Remove(xNode.Attributes["Sigillo_Password"]);
                    }

                    if ( xNode.Attributes["Sigillo_Data"] != null )
                    {
                        xNode.Attributes.Remove(xNode.Attributes["Sigillo_Data"]);
                    }
                }

                Save();

                Close();

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster );
            }

            return IDBilancio;
        }
#endregion

#region Conclusioni

        public int GetConclusioniCount()
        {
            int result = 0;

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/CONCLUSIONI/CONCLUSIONE" );

                if ( xNodes != null )
                {
                    result = xNodes.Count;
                }

                Close();
            }

            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }


        public ArrayList GetConclusioni( string IDCliente )
        {
            ArrayList results = new ArrayList();

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/CONCLUSIONI/CONCLUSIONE[@Cliente='" + IDCliente + "']" );

                foreach ( XmlNode node in xNodes )
                {
                    Hashtable result = new Hashtable();

                    foreach ( XmlAttribute item in node.Attributes )
                    {
                        result.Add( item.Name, item.Value );
                    }

                    results.Add( result );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return results;
        }

        public Hashtable GetConclusione( string IDConclusione )
        {
            Hashtable result = new Hashtable();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode( "/ROOT/CONCLUSIONI/CONCLUSIONE[@ID='" + IDConclusione + "']" );

                foreach ( XmlAttribute item in xNode.Attributes )
                {
                    result.Add( item.Name, item.Value );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public string AddConclusione( XmlNode node )
        {
            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/CONCLUSIONI" );

            if ( xNode == null )
            {
                xNode = document.SelectSingleNode( "/ROOT" );

                string xml2 = "<CONCLUSIONI LastID=\"1\"/>";
                XmlDocument doctmp2 = new XmlDocument();
                doctmp2.LoadXml( xml2 );

                XmlNode tmpNode2 = doctmp2.SelectSingleNode( "/CONCLUSIONI" );
                XmlNode cliente2 = document.ImportNode( tmpNode2, true );

                xNode.AppendChild( cliente2 );

                xNode = document.SelectSingleNode( "/ROOT/CONCLUSIONI" );
            }
            
            string ID = (Convert.ToInt32( xNode.Attributes["LastID"].Value ) + 1).ToString();

            node.Attributes["ID"].Value = ID;

            XmlNode xtmp = document.ImportNode( node, true );

            xNode.AppendChild( xtmp );

            xNode.Attributes["LastID"].Value = ID;

            Save();

            Close();

            return ID;
        }

        public void DeleteConclusione( int IDConclusione )
        {
            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode( "/ROOT/CONCLUSIONI/CONCLUSIONE[@ID='" + IDConclusione.ToString() + "']" );

                FileInfo fi = new FileInfo( App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value );
                if ( fi.Exists )
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo( App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value );
                if ( fd.Exists )
                {
                    fd.Delete();
                }

                xNode.ParentNode.RemoveChild( xNode );

                Save();

                //cancello allegati
                XmlManager xdoc = new XmlManager();
                xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
                XmlDocument xdoc_doc = xdoc.LoadEncodedFile( App.AppDocumentiDataFile );

                XmlNodeList xNodes = xdoc_doc.SelectNodes( "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDConclusione + "'][@Tree='" + (Convert.ToInt32( App.TipoFile.Conclusione )).ToString() + "']" );

                foreach ( XmlNode node in xNodes )
                {
                    FileInfo fis = new FileInfo( App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value );
                    if ( fis.Exists )
                    {
                        fis.Delete();
                    }

                    node.ParentNode.RemoveChild( node );
                }

                xdoc.SaveEncodedFile( App.AppDocumentiDataFile, xdoc_doc.InnerXml );

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster );
            }
        }

        public bool CheckDoppio_Conclusione( int ID, int IDCliente, string Data )
        {
            Open();

            XmlNodeList xNodes = document.SelectNodes( "/ROOT/CONCLUSIONI/CONCLUSIONE" );
            foreach ( XmlNode node in xNodes )
            {
                if ( node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data )
                {
                    Close();
                    return false;
                }
            }

            Close();
            return true;
        }
        
        public int SetConclusioneIntermedio(Hashtable values, int IDCliente, string dal, string al)
        {
            int IDConclusione = -1;
            try
            {
                Open();

                 XmlNode root = document.SelectSingleNode("/ROOT/CONCLUSIONI");

                    if (root == null)
                    {
                        root = document.SelectSingleNode("/ROOT");

                        string xml2 = "<CONCLUSIONI LastID=\"1\"/>";
                        XmlDocument doctmp2 = new XmlDocument();
                        doctmp2.LoadXml(xml2);

                        XmlNode tmpNode2 = doctmp2.SelectSingleNode("/CONCLUSIONI");
                        XmlNode cliente2 = document.ImportNode(tmpNode2, true);

                        root.AppendChild(cliente2);

                        root = document.SelectSingleNode("/ROOT/CONCLUSIONI");
                    }

                    IDConclusione = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

                    string lastindex = IDConclusione.ToString();

                    //Template TREE
                    FileInfo fitree = new FileInfo(App.AppTemplateTreeConclusione);
                    string estensione = "." + App.AppTemplateTreeConclusione.Split('.').Last();
                    string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewtree = new FileInfo(newNametree);

                    while (fnewtree.Exists)
                    {
                        newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewtree = new FileInfo(newNametree);
                    }

                    fitree.CopyTo(newNametree);
                    newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

                    //Template Dati
                    FileInfo fidati = new FileInfo(App.AppTemplateDataConclusione);
                    string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewdati = new FileInfo(newNamedati);

                    while (fnewdati.Exists)
                    {
                        newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewdati = new FileInfo(newNamedati);
                    }

                    fidati.CopyTo(newNamedati);
                    newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");

                    XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

                    string xml = "<CONCLUSIONE ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml(xml);

                    XmlNode tmpNode = doctmp.SelectSingleNode("/CONCLUSIONE");
                    XmlNode cliente = document.ImportNode(tmpNode, true);

                    root.AppendChild(cliente);

                    root.Attributes["LastID"].Value = lastindex;

                Save();

                Close();

            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
            }

            return IDConclusione;
        }

        public int SetConclusione( Hashtable values, int IDConclusione, int IDCliente )
        {
            try
            {
                Open();

                if ( IDConclusione == App.MasterFile_NewID )
                {
                    XmlNode root = document.SelectSingleNode( "/ROOT/CONCLUSIONI" );

                    if ( root == null )
                    {
                        root = document.SelectSingleNode( "/ROOT" );

                        string xml2 = "<CONCLUSIONI LastID=\"1\"/>";
                        XmlDocument doctmp2 = new XmlDocument();
                        doctmp2.LoadXml( xml2 );

                        XmlNode tmpNode2 = doctmp2.SelectSingleNode( "/CONCLUSIONI" );
                        XmlNode cliente2 = document.ImportNode( tmpNode2, true );

                        root.AppendChild( cliente2 );

                        root = document.SelectSingleNode( "/ROOT/CONCLUSIONI" );
                    }

                    IDConclusione = (Convert.ToInt32( root.Attributes["LastID"].Value ) + 1);

                    string lastindex = IDConclusione.ToString();

                    //Template TREE
                    FileInfo fitree = new FileInfo( App.AppTemplateTreeConclusione );
                    string estensione = "." + App.AppTemplateTreeConclusione.Split( '.' ).Last();
                    string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewtree = new FileInfo( newNametree );

                    while ( fnewtree.Exists )
                    {
                        newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewtree = new FileInfo( newNametree );
                    }

                    fitree.CopyTo( newNametree );
                    newNametree = newNametree.Replace( App.AppDataDataFolder + "\\", "" );

                    //Template Dati
                    FileInfo fidati = new FileInfo( App.AppTemplateDataConclusione );
                    string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewdati = new FileInfo( newNamedati );

                    while ( fnewdati.Exists )
                    {
                        newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewdati = new FileInfo( newNamedati );
                    }

                    fidati.CopyTo( newNamedati );
                    newNamedati = newNamedati.Replace( App.AppDataDataFolder + "\\", "" );
                    
                    XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

                    string xml = "<CONCLUSIONE ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ( (int)( App.TipoSessioneStato.Disponibile ) ).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml( xml );

                    XmlNode tmpNode = doctmp.SelectSingleNode( "/CONCLUSIONE" );
                    XmlNode cliente = document.ImportNode( tmpNode, true );

                    root.AppendChild( cliente );

                    root.Attributes["LastID"].Value = lastindex;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode( "/ROOT/CONSLUSIONI/CONCLUSIONE[@ID='" + IDConclusione.ToString() + "']" );

                    xNode.Attributes["Note"].Value = values["Note"].ToString();
                    xNode.Attributes["Data"].Value = values["Data"].ToString();
                }

                Save();

                Close();

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster );
            }

            return IDConclusione;
        }

        public int SetSigilloConclusione( int IDConclusione, string revisore, string password )
        {
            try
            {
                Open();

                if ( IDConclusione == App.MasterFile_NewID )
                {
                    ;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode( "/ROOT/CONCLUSIONI/CONCLUSIONE[@ID='" + IDConclusione.ToString() + "']" );

                    if ( xNode.Attributes["Sigillo"] == null )
                    {
                        xNode.Attributes.Append( document.CreateAttribute( "Sigillo" ) );
                    }

                    if ( xNode.Attributes["Sigillo_Password"] == null )
                    {
                        xNode.Attributes.Append( document.CreateAttribute( "Sigillo_Password" ) );
                    }

                    if ( xNode.Attributes["Sigillo_Data"] == null )
                    {
                        xNode.Attributes.Append( document.CreateAttribute( "Sigillo_Data" ) );
                    }

                    xNode.Attributes["Sigillo"].Value = revisore;
                    xNode.Attributes["Sigillo_Password"].Value = password;
                    xNode.Attributes["Sigillo_Data"].Value = DateTime.Now.ToShortDateString();
                }

                Save();

                Close();

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster );
            }

            return IDConclusione;
        }

        public int RemoveSigilloConclusione( int IDConclusione)
        {
            try
            {
                Open();

                if ( IDConclusione == App.MasterFile_NewID )
                {
                    ;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode( "/ROOT/CONCLUSIONI/CONCLUSIONE[@ID='" + IDConclusione.ToString() + "']" );

                    if ( xNode.Attributes["Sigillo"] != null )
                    {
                        xNode.Attributes.Remove(xNode.Attributes["Sigillo"]);
                    }

                    if ( xNode.Attributes["Sigillo_Password"] != null )
                    {
                        xNode.Attributes.Remove(xNode.Attributes["Sigillo_Password"]);
                    }

                    if ( xNode.Attributes["Sigillo_Data"] != null )
                    {
                        xNode.Attributes.Remove(xNode.Attributes["Sigillo_Data"] );
                    }
                }

                Save();

                Close();

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster );
            }

            return IDConclusione;
        }
#endregion


#region Pianificazione PianificazioniVerifica

        public int GetPianificazionePianificazioniVerificheCount()
        {
            int result = 0;

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA" );

                if ( xNodes != null )
                {
                    result = xNodes.Count;
                }

                Close();
            }

            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public ArrayList GetPianificazioniVerifiche( string IDCliente )
        {
            ArrayList results = new ArrayList();

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@Cliente='" + IDCliente + "']" );

                foreach ( XmlNode node in xNodes )
                {
                    Hashtable result = new Hashtable();

                    foreach ( XmlAttribute item in node.Attributes )
                    {
                        result.Add( item.Name, item.Value );
                    }

                    results.Add( result );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return results;
        }

        public Hashtable GetPianificazioniVerifica( string IDPianificazioniVerifica )
        {
            Hashtable result = new Hashtable();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode( "/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@ID='" + IDPianificazioniVerifica + "']" );

                foreach ( XmlAttribute item in xNode.Attributes )
                {
                    result.Add( item.Name, item.Value );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public string AddPianificazioniVerifica( XmlNode node )
        {
            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/PIANIFICAZIONIVERIFICHE" );

            if (xNode == null)
            {
                XmlNode xroot = document.SelectSingleNode("/ROOT");

                string xml = "<PIANIFICAZIONIVERIFICHE LastID=\"0\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);
                XmlNode tmpNode = doctmp.SelectSingleNode("/PIANIFICAZIONIVERIFICHE");
                XmlNode xxtmp = document.ImportNode(tmpNode, true);
                xroot.AppendChild(xxtmp);

                xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVERIFICHE");
            }

            string ID = ( Convert.ToInt32( xNode.Attributes["LastID"].Value ) + 1 ).ToString();

            node.Attributes["ID"].Value = ID;

            XmlNode xtmp = document.ImportNode( node, true );

            xNode.AppendChild( xtmp );

            xNode.Attributes["LastID"].Value = ID;

            Save();

            Close();

            return ID;
        }

        public void DeletePianificazioniVerifica( int IDPianificazioniVerifica )
        {
            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode( "/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@ID='" + IDPianificazioniVerifica.ToString() + "']" );

                FileInfo fi = new FileInfo( App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value );
                if ( fi.Exists )
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo( App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value );
                if ( fd.Exists )
                {
                    fd.Delete();
                }

                xNode.ParentNode.RemoveChild( xNode );

                Save();

                //cancello allegati
                XmlManager xdoc = new XmlManager();
                xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
                XmlDocument xdoc_doc = xdoc.LoadEncodedFile( App.AppDocumentiDataFile );

                XmlNodeList xNodes = xdoc_doc.SelectNodes( "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDPianificazioniVerifica + "'][@Tree='" + ( Convert.ToInt32( App.TipoFile.PianificazioniVerifica ) ).ToString() + "']" );

                foreach ( XmlNode node in xNodes )
                {
                    FileInfo fis = new FileInfo( App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value );
                    if ( fis.Exists )
                    {
                        fis.Delete();
                    }

                    node.ParentNode.RemoveChild( node );
                }

                xdoc.SaveEncodedFile( App.AppDocumentiDataFile, xdoc_doc.InnerXml );

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster );
            }
        }

        public bool CheckDoppio_PianificazioniVerifica( int ID, int IDCliente, string DataInizio, string DataFine )
        {
            Open();

            XmlNodeList xNodes = document.SelectNodes( "/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA" );

            DateTime dti_o = Convert.ToDateTime(DataInizio);
            DateTime dtf_o = Convert.ToDateTime(DataFine);

            foreach ( XmlNode node in xNodes )
            {
                //controllo standard
                if ( node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && ( node.Attributes["DataInizio"].Value == DataInizio || node.Attributes["DataFine"].Value == DataFine ) )
                {
                    Close();
                    return false;
                }

                if ( node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() )
                {
                    //controllo di accavallamento date
                    DateTime dti = Convert.ToDateTime( node.Attributes["DataInizio"].Value );
                    DateTime dtf = Convert.ToDateTime( node.Attributes["DataFine"].Value );

                    if ( ( dti_o.CompareTo( dti ) > 0 && dti_o.CompareTo( dtf ) < 0 ) || ( dtf_o.CompareTo( dti ) > 0 && dtf_o.CompareTo( dtf ) < 0 ) )
                    {
                        Close();
                        return false;
                    }
                }
            }

            Close();
            return true;
        }

        public void SetDataPianificazioniVerifica( string olddata_s, string newdata_s, int IDPianificazioniVerifica, int IDCliente )
        {
            string olddata = olddata_s;
            string newdata = newdata_s;

            try
            {
                olddata = olddata.Substring( 0, 5 ) + "&#xD;&#xA;" + olddata.Substring( 6, 4 );
                newdata = newdata.Substring( 0, 5 ) + "&#xD;&#xA;" + newdata.Substring( 6, 4 );
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }


            ArrayList al = GetPianificazioniVerifiche( IDCliente.ToString() );

            foreach ( Hashtable item in al )
            {
                XmlManager x2 = new XmlManager();
                x2.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                XmlDataProviderManager _test = new XmlDataProviderManager( App.AppDataDataFolder + "\\" + item["File"].ToString() );

                x2.SaveEncodedFile( App.AppDataDataFolder + "\\" + item["File"].ToString(), _test.Document.OuterXml.Replace( olddata, newdata ) );

            }
        }

        public int SetPianificazioniVerifica( Hashtable values, int IDPianificazioniVerifica, int IDCliente )
        {
            try
            {
                string olddata = "";
                string newdata = "";
                bool changedatatbd = false;

                Open();

                if ( document.SelectNodes( "/ROOT/PIANIFICAZIONIVERIFICHE" ) == null || document.SelectNodes( "/ROOT/PIANIFICAZIONIVERIFICHE" ).Count == 0 )
                {
                    string xml = "<PIANIFICAZIONIVERIFICHE LastID=\"1\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml( xml );

                    XmlNode tmpNode = doctmp.SelectSingleNode( "/PIANIFICAZIONIVERIFICHE" );
                    XmlNode cliente = document.ImportNode( tmpNode, true );

                    document.SelectSingleNode( "/ROOT" ).AppendChild( cliente );
                }

                if ( IDPianificazioniVerifica == App.MasterFile_NewID )
                {
                    XmlNode root = document.SelectSingleNode( "/ROOT/PIANIFICAZIONIVERIFICHE" );

                    IDPianificazioniVerifica = ( Convert.ToInt32( root.Attributes["LastID"].Value ) + 1 );

                    string lastindex = IDPianificazioniVerifica.ToString();

                    //Template TREE
                    FileInfo fitree = new FileInfo( App.AppTemplateTreePianificazioniVerifica );
                    string estensione = "." + App.AppTemplateTreePianificazioniVerifica.Split( '.' ).Last();
                    string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewtree = new FileInfo( newNametree );

                    while ( fnewtree.Exists )
                    {
                        newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewtree = new FileInfo( newNametree );
                    }

                    fitree.CopyTo( newNametree );
                    newNametree = newNametree.Replace( App.AppDataDataFolder + "\\", "" );

                    //Template Dati
                    FileInfo fidati = new FileInfo( App.AppTemplateDataPianificazioniVerifica );
                    string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewdati = new FileInfo( newNamedati );

                    while ( fnewdati.Exists )
                    {
                        newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewdati = new FileInfo( newNamedati );
                    }

                    fidati.CopyTo( newNamedati );
                    newNamedati = newNamedati.Replace( App.AppDataDataFolder + "\\", "" );


                    string xml = "<PIANIFICAZIONIVERIFICA ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ( (int)( App.TipoSessioneStato.Disponibile ) ).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" DataInizio=\"" + values["DataInizio"].ToString() + "\" DataFine=\"" + values["DataFine"].ToString() + "\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml( xml );

                    XmlNode tmpNode = doctmp.SelectSingleNode( "/PIANIFICAZIONIVERIFICA" );
                    XmlNode cliente = document.ImportNode( tmpNode, true );

                    root.AppendChild( cliente );

                    root.Attributes["LastID"].Value = lastindex;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode( "/ROOT/PIANIFICAZIONIVERIFICHE/PIANIFICAZIONIVERIFICA[@ID='" + IDPianificazioniVerifica.ToString() + "']" );

        
                    if ( xNode.Attributes["DataInizio"].Value != values["DataInizio"].ToString() )
                    {
                        olddata = xNode.Attributes["DataInizio"].Value;
                        newdata = values["DataInizio"].ToString();
                        changedatatbd = true;

                        xNode.Attributes["DataInizio"].Value = values["DataInizio"].ToString();
                    }

                    if ( xNode.Attributes["DataFine"] == null )
                    {
                        XmlAttribute attr = document.CreateAttribute( "DataFine" );
                        xNode.Attributes.Append( attr );
                    }

                    xNode.Attributes["DataFine"].Value = values["DataFine"].ToString();
                }

                Save();

                Close();

                if ( changedatatbd )
                {
                    SetDataPianificazioniVerifica( olddata, newdata, IDPianificazioniVerifica, IDCliente );
                }

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster );
            }

            return IDPianificazioniVerifica;
        }

#endregion

#region Pianificazione PianificazioniVigilanza

        public int GetPianificazionePianificazioniVigilanzeCount()
        {
            int result = 0;

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA" );

                if ( xNodes != null )
                {
                    result = xNodes.Count;
                }

                Close();
            }

            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public ArrayList GetPianificazioniVigilanze( string IDCliente )
        {
            ArrayList results = new ArrayList();

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@Cliente='" + IDCliente + "']" );

                foreach ( XmlNode node in xNodes )
                {
                    Hashtable result = new Hashtable();

                    foreach ( XmlAttribute item in node.Attributes )
                    {
                        result.Add( item.Name, item.Value );
                    }

                    results.Add( result );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return results;
        }

        public Hashtable GetPianificazioniVigilanza( string IDPianificazioniVigilanza )
        {
            Hashtable result = new Hashtable();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode( "/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@ID='" + IDPianificazioniVigilanza + "']" );

                foreach ( XmlAttribute item in xNode.Attributes )
                {
                    result.Add( item.Name, item.Value );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public string AddPianificazioniVigilanza( XmlNode node )
        {
            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/PIANIFICAZIONIVIGILANZE" );

            if (xNode == null)
            {
                XmlNode xroot = document.SelectSingleNode("/ROOT");

                string xml = "<PIANIFICAZIONIVIGILANZE LastID=\"0\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);
                XmlNode tmpNode = doctmp.SelectSingleNode("/PIANIFICAZIONIVIGILANZE");
                XmlNode xxtmp = document.ImportNode(tmpNode, true);
                xroot.AppendChild(xxtmp);

                xNode = document.SelectSingleNode("/ROOT/PIANIFICAZIONIVIGILANZE");
            }

            string ID = ( Convert.ToInt32( xNode.Attributes["LastID"].Value ) + 1 ).ToString();

            node.Attributes["ID"].Value = ID;

            XmlNode xtmp = document.ImportNode( node, true );

            xNode.AppendChild( xtmp );

            xNode.Attributes["LastID"].Value = ID;

            Save();

            Close();

            return ID;
        }

        public void DeletePianificazioniVigilanza( int IDPianificazioniVigilanza )
        {
            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode( "/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@ID='" + IDPianificazioniVigilanza.ToString() + "']" );

                FileInfo fi = new FileInfo( App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value );
                if ( fi.Exists )
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo( App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value );
                if ( fd.Exists )
                {
                    fd.Delete();
                }

                xNode.ParentNode.RemoveChild( xNode );

                Save();

                //cancello allegati
                XmlManager xdoc = new XmlManager();
                xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
                XmlDocument xdoc_doc = xdoc.LoadEncodedFile( App.AppDocumentiDataFile );

                XmlNodeList xNodes = xdoc_doc.SelectNodes( "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDPianificazioniVigilanza + "'][@Tree='" + ( Convert.ToInt32( App.TipoFile.PianificazioniVigilanza ) ).ToString() + "']" );

                foreach ( XmlNode node in xNodes )
                {
                    FileInfo fis = new FileInfo( App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value );
                    if ( fis.Exists )
                    {
                        fis.Delete();
                    }

                    node.ParentNode.RemoveChild( node );
                }

                xdoc.SaveEncodedFile( App.AppDocumentiDataFile, xdoc_doc.InnerXml );

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster );
            }
        }

        public bool CheckDoppio_PianificazioniVigilanza( int ID, int IDCliente, string DataInizio, string DataFine )
        {
            Open();

            DateTime dti_o = Convert.ToDateTime( DataInizio );
            DateTime dtf_o = Convert.ToDateTime( DataFine );

            XmlNodeList xNodes = document.SelectNodes( "/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA" );
            foreach ( XmlNode node in xNodes )
            {
                if ( node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && ( node.Attributes["DataInizio"].Value == DataInizio || node.Attributes["DataFine"].Value == DataFine ) )
                {
                    Close();
                    return false;
                }

                if ( node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() )
                {
                    //controllo di accavallamento date
                    DateTime dti = Convert.ToDateTime( node.Attributes["DataInizio"].Value );
                    DateTime dtf = Convert.ToDateTime( node.Attributes["DataFine"].Value );

                    if ( ( dti_o.CompareTo( dti ) > 0 && dti_o.CompareTo( dtf ) < 0 ) || ( dtf_o.CompareTo( dti ) > 0 && dtf_o.CompareTo( dtf ) < 0 ) )
                    {
                        Close();
                        return false;
                    }
                }
            }

            Close();
            return true;
        }

        public void SetDataPianificazioniVigilanza( string olddata_s, string newdata_s, int IDPianificazioniVigilanza, int IDCliente )
        {
            string olddata = olddata_s;
            string newdata = newdata_s;

            try
            {
                olddata = olddata.Substring( 0, 5 ) + "&#xD;&#xA;" + olddata.Substring( 6, 4 );
                newdata = newdata.Substring( 0, 5 ) + "&#xD;&#xA;" + newdata.Substring( 6, 4 );
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }


            ArrayList al = GetPianificazioniVigilanze( IDCliente.ToString() );

            foreach ( Hashtable item in al )
            {
                XmlManager x2 = new XmlManager();
                x2.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                XmlDataProviderManager _test = new XmlDataProviderManager( App.AppDataDataFolder + "\\" + item["File"].ToString() );

                x2.SaveEncodedFile( App.AppDataDataFolder + "\\" + item["File"].ToString(), _test.Document.OuterXml.Replace( olddata, newdata ) );

            }
        }

        public int SetPianificazioniVigilanza( Hashtable values, int IDPianificazioniVigilanza, int IDCliente )
        {
            try
            {
                string olddata = "";
                string newdata = "";
                bool changedatatbd = false;

                Open();

                if ( document.SelectNodes( "/ROOT/PIANIFICAZIONIVIGILANZE" ) == null || document.SelectNodes( "/ROOT/PIANIFICAZIONIVIGILANZE" ).Count == 0 )
                {
                    string xml = "<PIANIFICAZIONIVIGILANZE LastID=\"1\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml( xml );

                    XmlNode tmpNode = doctmp.SelectSingleNode( "/PIANIFICAZIONIVIGILANZE" );
                    XmlNode cliente = document.ImportNode( tmpNode, true );

                    document.SelectSingleNode( "/ROOT" ).AppendChild( cliente );
                }

                if ( IDPianificazioniVigilanza == App.MasterFile_NewID )
                {
                    XmlNode root = document.SelectSingleNode( "/ROOT/PIANIFICAZIONIVIGILANZE" );

                    IDPianificazioniVigilanza = ( Convert.ToInt32( root.Attributes["LastID"].Value ) + 1 );

                    string lastindex = IDPianificazioniVigilanza.ToString();

                    //Template TREE
                    FileInfo fitree = new FileInfo( App.AppTemplateTreePianificazioniVigilanza );
                    string estensione = "." + App.AppTemplateTreePianificazioniVigilanza.Split( '.' ).Last();
                    string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewtree = new FileInfo( newNametree );

                    while ( fnewtree.Exists )
                    {
                        newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewtree = new FileInfo( newNametree );
                    }

                    fitree.CopyTo( newNametree );
                    newNametree = newNametree.Replace( App.AppDataDataFolder + "\\", "" );

                    //Template Dati
                    FileInfo fidati = new FileInfo( App.AppTemplateDataPianificazioniVigilanza );
                    string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewdati = new FileInfo( newNamedati );

                    while ( fnewdati.Exists )
                    {
                        newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewdati = new FileInfo( newNamedati );
                    }

                    fidati.CopyTo( newNamedati );
                    newNamedati = newNamedati.Replace( App.AppDataDataFolder + "\\", "" );


                    string xml = "<PIANIFICAZIONIVIGILANZA ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ( (int)( App.TipoSessioneStato.Disponibile ) ).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" DataInizio=\"" + values["DataInizio"].ToString() + "\" DataFine=\"" + values["DataFine"].ToString() + "\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml( xml );

                    XmlNode tmpNode = doctmp.SelectSingleNode( "/PIANIFICAZIONIVIGILANZA" );
                    XmlNode cliente = document.ImportNode( tmpNode, true );

                    root.AppendChild( cliente );

                    root.Attributes["LastID"].Value = lastindex;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode( "/ROOT/PIANIFICAZIONIVIGILANZE/PIANIFICAZIONIVIGILANZA[@ID='" + IDPianificazioniVigilanza.ToString() + "']" );


                    if ( xNode.Attributes["DataInizio"].Value != values["DataInizio"].ToString() )
                    {
                        olddata = xNode.Attributes["DataInizio"].Value;
                        newdata = values["DataInizio"].ToString();
                        changedatatbd = true;

                        xNode.Attributes["DataInizio"].Value = values["DataInizio"].ToString();
                    }

                    if ( xNode.Attributes["DataFine"] == null )
                    {
                        XmlAttribute attr = document.CreateAttribute( "DataFine" );
                        xNode.Attributes.Append( attr );
                    }

                    xNode.Attributes["DataFine"].Value = values["DataFine"].ToString();
                }

                Save();

                Close();

                if ( changedatatbd )
                {
                    SetDataPianificazioniVigilanza( olddata, newdata, IDPianificazioniVigilanza, IDCliente );
                }

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster );
            }

            return IDPianificazioniVigilanza;
        }

#endregion

#region Verifica

        public int GetVerificheCount()
        {
            int result = 0;

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/VERIFICHE/VERIFICA" );

                if ( xNodes != null )
                {
                    result = xNodes.Count;
                }

                Close();
            }

            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }
                
		public ArrayList GetVerifiche(string IDCliente)
		{
			ArrayList results = new ArrayList();

			try
			{
				Open();

				XmlNodeList xNodes = document.SelectNodes("/ROOT/VERIFICHE/VERIFICA[@Cliente='" + IDCliente + "']");

				foreach (XmlNode node in xNodes)
				{
					Hashtable result = new Hashtable();

					foreach (XmlAttribute item in node.Attributes)
					{
						result.Add(item.Name, item.Value);
					}

					results.Add(result);
				}

				Close();
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}

			return results;
		}

		public Hashtable GetVerifica(string IDVerifica)
		{
			Hashtable result = new Hashtable();

			try
			{
				Open();

				XmlNode xNode = document.SelectSingleNode("/ROOT/VERIFICHE/VERIFICA[@ID='" + IDVerifica + "']");

				foreach (XmlAttribute item in xNode.Attributes)
				{
					result.Add(item.Name, item.Value);
				}

                if (!result.ContainsKey("DataEsecuzione_Fine"))
                {
                    result.Add("DataEsecuzione_Fine", result["DataEsecuzione"].ToString());
                }
                if (!result.ContainsKey("DataOggetto_Inizio"))
                {
                    result.Add("DataOggetto_Inizio", result["DataEsecuzione"].ToString());
                }
                if (!result.ContainsKey("DataOggetto_Fine"))
                {
                    result.Add("DataOggetto_Fine", result["DataEsecuzione"].ToString());
                }

                Close();
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}

			return result;
		}

		public string AddVerifica(XmlNode node)
		{
			Open();

			XmlNode xNode = document.SelectSingleNode("/ROOT/VERIFICHE");

			string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();

			node.Attributes["ID"].Value = ID;

			XmlNode xtmp = document.ImportNode(node, true);

			xNode.AppendChild(xtmp);

			xNode.Attributes["LastID"].Value = ID;

			Save();

			Close();

			return ID;
		}

		public void DeleteVerifica(int IDVerifica)
		{
			try
			{
				Open();

				XmlNode xNode = document.SelectSingleNode("/ROOT/VERIFICHE/VERIFICA[@ID='" + IDVerifica.ToString() + "']");

				FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
				if (fi.Exists)
				{
					fi.Delete();
				}

				FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
				if (fd.Exists)
				{
					fd.Delete();
				}

				xNode.ParentNode.RemoveChild(xNode);

				Save();

				//cancello allegati
				XmlManager xdoc = new XmlManager();
				xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
				XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);

                XmlNodeList xNodes = xdoc_doc.SelectNodes( "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDVerifica + "'][@Tree='" + (Convert.ToInt32( App.TipoFile.Verifica )).ToString() + "']" );

				foreach (XmlNode node in xNodes)
				{
					FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
					if (fis.Exists)
					{
						fis.Delete();
					}

					node.ParentNode.RemoveChild(node);
				}

				xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);

				Close();
			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
			}
		}

		public bool CheckDoppio_Verifica(int ID, int IDCliente, string Data)
		{
			Open();

			XmlNodeList xNodes = document.SelectNodes("/ROOT/VERIFICHE/VERIFICA");
			foreach (XmlNode node in xNodes)
			{
				if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data)
				{
					Close();
					return false;
				}
			}

			Close();
			return true;
		}

        public void SetDataVerifica( string olddata_s, string newdata_s, int IDVerifica, int IDCliente )
        {
            string olddata = olddata_s;
            string newdata = newdata_s;

            try
            {
                olddata = olddata.Substring( 0, 5 ) + "&#xD;&#xA;" + olddata.Substring( 6, 4 );
                newdata = newdata.Substring( 0, 5 ) + "&#xD;&#xA;" + newdata.Substring( 6, 4 );
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }


            ArrayList al = GetVerifiche( IDCliente.ToString() );

            foreach ( Hashtable item in al )
            {
                XmlManager x2 = new XmlManager();
                x2.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                XmlDataProviderManager _test = new XmlDataProviderManager( App.AppDataDataFolder + "\\" + item["File"].ToString() );

                x2.SaveEncodedFile( App.AppDataDataFolder + "\\" + item["File"].ToString(), _test.Document.OuterXml.Replace( olddata, newdata ) );

            }
        }

		public int SetVerifica(Hashtable values, int IDVerifica, int IDCliente)
		{
			try
			{
                string olddata = "";
                string newdata = "";
                bool changedatatbd = false;

				Open();

				if (IDVerifica == App.MasterFile_NewID)
				{
					XmlNode root = document.SelectSingleNode("/ROOT/VERIFICHE");

					IDVerifica = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

					string lastindex = IDVerifica.ToString();

					//Template TREE
					FileInfo fitree = new FileInfo(App.AppTemplateTreeVerifica);
					string estensione = "." + App.AppTemplateTreeVerifica.Split('.').Last();
					string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
					FileInfo fnewtree = new FileInfo(newNametree);

					while (fnewtree.Exists)
					{
						newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
						fnewtree = new FileInfo(newNametree);
					}

					fitree.CopyTo(newNametree);
					newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

					//Template Dati
					FileInfo fidati = new FileInfo(App.AppTemplateDataVerifica);
					string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
					FileInfo fnewdati = new FileInfo(newNamedati);

					while (fnewdati.Exists)
					{
						newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
						fnewdati = new FileInfo(newNamedati);
					}

					fidati.CopyTo(newNamedati);
					newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");

                    string xml = "<VERIFICA ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ( (int)( App.TipoSessioneStato.Disponibile ) ).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Composizione=\"" + values["Composizione"].ToString() + "\" Inizio=\"" + values["Inizio"].ToString() + "\" Fine=\"" + values["Fine"].ToString() + "\" Luogo=\"" + values["Luogo"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" Revisore=\"" + values["Revisore"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" Presidente=\"" + values["Presidente"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" Sindaco1=\"" + values["Sindaco1"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" Sindaco2=\"" + values["Sindaco2"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" Collaboratore=\"" + values["Collaboratore"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" AssisitoDa=\"" + values["AssisitoDa"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" Data=\"" + values["Data"].ToString() + "\" DataEsecuzione=\"" + values["DataEsecuzione"].ToString() + "\" DataEsecuzione_Fine=\"" + values["DataEsecuzione_Fine"].ToString() + "\" DataOggetto_Inizio= \"" + ((values.Contains("DataOggetto_Inizio"))? values["DataOggetto_Inizio"].ToString() : values["DataEsecuzione"].ToString()) + "\" DataOggetto_Fine= \"" + ((values.Contains("DataOggetto_Fine")) ? values["DataOggetto_Fine"].ToString() : values["DataEsecuzione"].ToString()) + "\"/>"; 
                    XmlDocument doctmp = new XmlDocument();
					doctmp.LoadXml(xml);

					XmlNode tmpNode = doctmp.SelectSingleNode("/VERIFICA");
					XmlNode cliente = document.ImportNode(tmpNode, true);

					root.AppendChild(cliente);

					root.Attributes["LastID"].Value = lastindex;
				}
				else
				{
					XmlNode xNode = document.SelectSingleNode("/ROOT/VERIFICHE/VERIFICA[@ID='" + IDVerifica.ToString() + "']");

					xNode.Attributes["Inizio"].Value = values["Inizio"].ToString();
					xNode.Attributes["Fine"].Value = values["Fine"].ToString();
					xNode.Attributes["Luogo"].Value = values["Luogo"].ToString();
					xNode.Attributes["Revisore"].Value = values["Revisore"].ToString();
					xNode.Attributes["Presidente"].Value = values["Presidente"].ToString();
					xNode.Attributes["Sindaco1"].Value = values["Sindaco1"].ToString();
					xNode.Attributes["Sindaco2"].Value = values["Sindaco2"].ToString();
                    if ( xNode.Attributes["Collaboratore"] == null )
                    {
                        XmlAttribute attr = document.CreateAttribute( "Collaboratore" );
                        xNode.Attributes.Append( attr );
                    }
                    xNode.Attributes["Collaboratore"].Value = ((values["Collaboratore"] == null)? "" : values["Collaboratore"].ToString());
					xNode.Attributes["AssisitoDa"].Value = values["AssisitoDa"].ToString();

					xNode.Attributes["Composizione"].Value = values["Composizione"].ToString();

                    if ( xNode.Attributes["Data"].Value != values["Data"].ToString() )
                    {
                        olddata = xNode.Attributes["Data"].Value;
                        newdata = values["Data"].ToString();
                        changedatatbd = true;

                        xNode.Attributes["Data"].Value = values["Data"].ToString();
                    }

                    if ( xNode.Attributes["DataEsecuzione"] == null )
                    {
                        XmlAttribute attr = document.CreateAttribute( "DataEsecuzione" );
                        xNode.Attributes.Append( attr );
                    }

                    if (xNode.Attributes["DataEsecuzione_Fine"] == null)
                    {
                        XmlAttribute attr = document.CreateAttribute("DataEsecuzione_Fine");
                        xNode.Attributes.Append(attr);
                    }

                    if (xNode.Attributes["DataOggetto_Inizio"] == null)
                    {
                        XmlAttribute attr = document.CreateAttribute("DataOggetto_Inizio");
                        xNode.Attributes.Append(attr);
                    }

                    if (xNode.Attributes["DataOggetto_Fine"] == null)
                    {
                        XmlAttribute attr = document.CreateAttribute("DataOggetto_Fine");
                        xNode.Attributes.Append(attr);
                    }

                    xNode.Attributes["DataEsecuzione"].Value = ((values["DataEsecuzione"] == null) ? values["Data"].ToString() : values["DataEsecuzione"].ToString());
                    xNode.Attributes["DataEsecuzione_Fine"].Value = ((values["DataEsecuzione_Fine"] == null) ? values["Data"].ToString() : values["DataEsecuzione_Fine"].ToString());

                    xNode.Attributes["DataOggetto_Inizio"].Value = ((values["DataOggetto_Inizio"] == null) ? values["Data"].ToString() : values["DataOggetto_Inizio"].ToString());
                    xNode.Attributes["DataOggetto_Fine"].Value = ((values["DataOggetto_Fine"] == null) ? values["Data"].ToString() : values["DataOggetto_Fine"].ToString());
                }

				Save();

				Close();

                if(changedatatbd)
                {
                    SetDataVerifica( olddata, newdata, IDVerifica, IDCliente );
                }

			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
			}

			return IDVerifica;
		}

        public Hashtable GetVigilanzaAssociataFromVerifica( string ID )
        {
            Hashtable result = new Hashtable();

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/VERIFICHE/VERIFICA[@ID='" + ID + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetVigilanze( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            result = item;
                            break;
                        }
                    }
                }
            }

            Close();

            return result;
        }
#endregion

#region Vigilanza

        public void SplitVerificheVigilanze()
        {
            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/VERIFICHE/VERIFICA" );

                foreach ( XmlNode node in xNodes )
                {
                    if(node.Attributes["AlreadySplitted"] != null)
                    {
                        continue;
                    }

                    //Controllo se l'albero comprende vigilanze
                    FileInfo fileVerifica = new FileInfo( App.AppDataDataFolder + "\\" + node.Attributes["File"].Value );
                    FileInfo fileVerificaDati = new FileInfo( App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value );
                    if ( fileVerifica.Exists && fileVerificaDati.Exists)
                    {
                        XmlDataProviderManager xdocVerifica = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                        XmlDocument docVerifica = xdocVerifica.Document;

                        XmlDataProviderManager xdocVerificaDati = new XmlDataProviderManager( App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value );
                        XmlDocument docVerificaDati = xdocVerificaDati.Document;

                        if ( docVerifica.SelectNodes( "//Node[@ID>=500]" ).Count > 0 )
                        {
                            //Creo nuovi file vigilanze
                            string estensione = "." + App.AppTemplateTreeVigilanza.Split( '.' ).Last();
                            string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                            FileInfo fnewtree = new FileInfo( newNametree );

                            while ( fnewtree.Exists )
                            {
                                newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                                fnewtree = new FileInfo( newNametree );
                            }

                            fileVerifica.CopyTo( newNametree );

                            XmlDataProviderManager xdocVigilanza = new XmlDataProviderManager(newNametree);
                            XmlDocument docVigilanza = xdocVigilanza.Document;

                            XmlNode noderevisoft = docVigilanza.SelectSingleNode( "//REVISOFT" );
                            noderevisoft.Attributes["ID"].Value = (Convert.ToInt32(App.TipoFile.Vigilanza)).ToString();

                            XmlNode firstnodeToBeDeleted = docVigilanza.SelectSingleNode( "//Node[@ID=1]" );
                            firstnodeToBeDeleted.ParentNode.ReplaceChild( firstnodeToBeDeleted.ChildNodes[1], firstnodeToBeDeleted );

                            foreach ( XmlNode item in docVigilanza.SelectNodes( "//Node[@ID<500]" ) )
                            {
                                if ( item.Attributes["ID"].Value != "1" )
                                {
                                    item.ParentNode.RemoveChild( item );
                                }
                            }

                            xdocVigilanza.Save();

                            noderevisoft = docVerifica.SelectSingleNode( "//REVISOFT" );
                            noderevisoft.Attributes["ID"].Value = ( Convert.ToInt32( App.TipoFile.Verifica ) ).ToString();

                            firstnodeToBeDeleted = docVerifica.SelectSingleNode( "//Node[@ID=1]" );
                            firstnodeToBeDeleted.ParentNode.ReplaceChild( firstnodeToBeDeleted.ChildNodes[0], firstnodeToBeDeleted );

                            foreach ( XmlNode item in docVerifica.SelectNodes( "//Node[@ID>=500]" ) )
                            {
                                item.ParentNode.RemoveChild( item );
                            }

                            xdocVerifica.Save();

                            newNametree = newNametree.Replace( App.AppDataDataFolder + "\\", "" );

                            string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                            FileInfo fnewdati = new FileInfo( newNamedati );

                            while ( fnewdati.Exists )
                            {
                                newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                                fnewdati = new FileInfo( newNamedati );
                            }

                            fileVerificaDati.CopyTo( newNamedati );

                            XmlDataProviderManager xdocVigilanzaData = new XmlDataProviderManager( newNamedati );
                            XmlDocument docVigilanzaData = xdocVigilanzaData.Document;

                            noderevisoft = docVigilanzaData.SelectSingleNode( "//REVISOFT" );
                            noderevisoft.Attributes["ID"].Value = ( Convert.ToInt32( App.TipoFile.Vigilanza ) ).ToString();

                            foreach ( XmlNode item in docVigilanzaData.SelectNodes( "//Dato[@ID<500]" ) )
                            {
                                if ( item.Attributes["ID"].Value != "1" )
                                {
                                    item.ParentNode.RemoveChild( item );
                                }
                            }

                            xdocVigilanzaData.Save();

                            noderevisoft = docVerificaDati.SelectSingleNode( "//REVISOFT" );
                            noderevisoft.Attributes["ID"].Value = ( Convert.ToInt32( App.TipoFile.Verifica ) ).ToString();

                            foreach ( XmlNode item in docVerificaDati.SelectNodes( "//Dato[@ID>=500]" ) )
                            {
                                item.ParentNode.RemoveChild( item );
                            }

                            xdocVerificaDati.Save();

                            newNamedati = newNamedati.Replace( App.AppDataDataFolder + "\\", "" );

                            if ( document.SelectNodes( "/ROOT/VIGILANZE" ) == null || document.SelectNodes( "/ROOT/VIGILANZE" ).Count == 0 )
                            {
                                string xml = "<VIGILANZE LastID=\"1\" />";
                                XmlDocument doctmp = new XmlDocument();
                                doctmp.LoadXml( xml );

                                XmlNode tmpNode = doctmp.SelectSingleNode( "/VIGILANZE" );
                                XmlNode cliente = document.ImportNode( tmpNode, true );

                                document.SelectSingleNode( "/ROOT" ).AppendChild( cliente );
                            }

                            XmlNode root = document.SelectSingleNode( "/ROOT/VIGILANZE" );

                            int IDVigilanza = ( Convert.ToInt32( root.Attributes["LastID"].Value ) + 1 );

                            string lastindex = IDVigilanza.ToString();

                            root.Attributes["LastID"].Value = lastindex;

                            string xmlVigilanza = "<VIGILANZA ID=\"" + lastindex + "\" Cliente=\"" + node.Attributes["Cliente"].Value + "\" Stato=\"" + ( (int)( App.TipoSessioneStato.Disponibile ) ).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\"  Data=\"" + node.Attributes["Data"].Value + "\"  />";
                            XmlDocument doctmp2 = new XmlDocument();
                            doctmp2.LoadXml( xmlVigilanza );

                            XmlNode tmpNode2 = doctmp2.SelectSingleNode( "/VIGILANZA" );
                            XmlNode cliente2 = document.ImportNode( tmpNode2, true );

                            root.AppendChild( cliente2 );
                            
                            Save();

                            //aggiorno allegati
                            XmlManager xdoc = new XmlManager();
                            xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
                            XmlDocument xdoc_doc = xdoc.LoadEncodedFile( App.AppDocumentiDataFile );

                            XmlNodeList xNodesDocuments = xdoc_doc.SelectNodes( "//DOCUMENTI/DOCUMENTO[@Sessione='" + node.Attributes["ID"].Value + "'][@Tree='" + ( Convert.ToInt32( App.TipoFile.Verifica ) ).ToString() + "'][@Nodo>=500]" );

                            foreach ( XmlNode nodeD in xNodesDocuments )
                            {
                                nodeD.Attributes["Tree"].Value = ( Convert.ToInt32( App.TipoFile.Vigilanza ) ).ToString();
                            }

                            xdoc.SaveEncodedFile( App.AppDocumentiDataFile, xdoc_doc.InnerXml );
                        }
                        else
                        {                            
							XmlAttribute attr = document.CreateAttribute("AlreadySplitted");
                            attr.Value = "True";
							node.Attributes.Append(attr);
                        }
                    }    
                }
                
                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return;
        }

        public int GetVigilanzeCount()
        {
            int result = 0;

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/VIGILANZE/VIGILANZA" );

                if ( xNodes != null )
                {
                    result = xNodes.Count;
                }

                Close();
            }

            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public ArrayList GetVigilanze( string IDCliente )
        {
            ArrayList results = new ArrayList();

            try
            {
                Open();

                if ( document.SelectNodes( "/ROOT/VIGILANZE" ) == null || document.SelectNodes( "/ROOT/VIGILANZE" ).Count == 0 )
                {
                    string xml = "<VIGILANZE LastID=\"1\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml( xml );

                    XmlNode tmpNode = doctmp.SelectSingleNode( "/VIGILANZE" );
                    XmlNode cliente = document.ImportNode( tmpNode, true );

                    document.SelectSingleNode( "/ROOT" ).AppendChild( cliente );
                }

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/VIGILANZE/VIGILANZA[@Cliente='" + IDCliente + "']" );

                foreach ( XmlNode node in xNodes )
                {
                    Hashtable result = new Hashtable();

                    foreach ( XmlAttribute item in node.Attributes )
                    {
                        result.Add( item.Name, item.Value );
                    }

                    results.Add( result );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return results;
        }

        public Hashtable GetVigilanza( string IDVigilanza )
        {
            Hashtable result = new Hashtable();

            try
            {
                Open();

                if ( document.SelectNodes( "/ROOT/VIGILANZE" ) == null || document.SelectNodes( "/ROOT/VIGILANZE" ).Count == 0 )
                {
                    string xml = "<VIGILANZE LastID=\"1\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml( xml );

                    XmlNode tmpNode = doctmp.SelectSingleNode( "/VIGILANZE" );
                    XmlNode cliente = document.ImportNode( tmpNode, true );

                    document.SelectSingleNode( "/ROOT" ).AppendChild( cliente );
                }

                XmlNode xNode = document.SelectSingleNode( "/ROOT/VIGILANZE/VIGILANZA[@ID='" + IDVigilanza + "']" );

                foreach ( XmlAttribute item in xNode.Attributes )
                {
                    result.Add( item.Name, item.Value );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public string AddVigilanza( XmlNode node )
        {
            Open();

            if ( document.SelectNodes( "/ROOT/VIGILANZE" ) == null || document.SelectNodes( "/ROOT/VIGILANZE" ).Count == 0 )
            {
                string xml = "<VIGILANZE LastID=\"1\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml( xml );

                XmlNode tmpNode = doctmp.SelectSingleNode( "/VIGILANZE" );
                XmlNode cliente = document.ImportNode( tmpNode, true );

                document.SelectSingleNode( "/ROOT" ).AppendChild( cliente );
            }

            XmlNode xNode = document.SelectSingleNode( "/ROOT/VIGILANZE" );

            string ID = (Convert.ToInt32( xNode.Attributes["LastID"].Value ) + 1).ToString();

            node.Attributes["ID"].Value = ID;

            XmlNode xtmp = document.ImportNode( node, true );

            xNode.AppendChild( xtmp );

            xNode.Attributes["LastID"].Value = ID;

            Save();

            Close();

            return ID;
        }

        public void DeleteVigilanza( int IDVigilanza )
        {
            try
            {
                Open();

                if ( document.SelectNodes( "/ROOT/VIGILANZE" ) == null || document.SelectNodes( "/ROOT/VIGILANZE" ).Count == 0 )
                {
                    string xml = "<VIGILANZE LastID=\"1\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml( xml );

                    XmlNode tmpNode = doctmp.SelectSingleNode( "/VIGILANZE" );
                    XmlNode cliente = document.ImportNode( tmpNode, true );

                    document.SelectSingleNode( "/ROOT" ).AppendChild( cliente );
                }

                XmlNode xNode = document.SelectSingleNode( "/ROOT/VIGILANZE/VIGILANZA[@ID='" + IDVigilanza.ToString() + "']" );

                FileInfo fi = new FileInfo( App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value );
                if ( fi.Exists )
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo( App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value );
                if ( fd.Exists )
                {
                    fd.Delete();
                }

                xNode.ParentNode.RemoveChild( xNode );

                Save();

                //cancello allegati
                XmlManager xdoc = new XmlManager();
                xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
                XmlDocument xdoc_doc = xdoc.LoadEncodedFile( App.AppDocumentiDataFile );

                XmlNodeList xNodes = xdoc_doc.SelectNodes( "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDVigilanza + "'][@Tree='" + (Convert.ToInt32( App.TipoFile.Vigilanza )).ToString() + "']" );

                foreach ( XmlNode node in xNodes )
                {
                    FileInfo fis = new FileInfo( App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value );
                    if ( fis.Exists )
                    {
                        fis.Delete();
                    }

                    node.ParentNode.RemoveChild( node );
                }

                xdoc.SaveEncodedFile( App.AppDocumentiDataFile, xdoc_doc.InnerXml );

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster );
            }
        }

        public bool CheckDoppio_Vigilanza( int ID, int IDCliente, string Data )
        {
            Open();

            if ( document.SelectNodes( "/ROOT/VIGILANZE" ) == null || document.SelectNodes( "/ROOT/VIGILANZE" ).Count == 0 )
            {
                string xml = "<VIGILANZE LastID=\"1\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml( xml );

                XmlNode tmpNode = doctmp.SelectSingleNode( "/VIGILANZE" );
                XmlNode cliente = document.ImportNode( tmpNode, true );

                document.SelectSingleNode( "/ROOT" ).AppendChild( cliente );
            }

            XmlNodeList xNodes = document.SelectNodes( "/ROOT/VIGILANZE/VIGILANZA" );
            foreach ( XmlNode node in xNodes )
            {
                if ( node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data )
                {
                    Close();
                    return false;
                }
            }

            Close();
            return true;
        }

        public void SetDataVigilanza( string olddata_s, string newdata_s, int IDVerifica, int IDCliente )
        {
            string olddata = olddata_s;
            string newdata = newdata_s;

            try
            {
                olddata = olddata.Substring( 0, 5 ) + "&#xD;&#xA;" + olddata.Substring( 6, 4 );
                newdata = newdata.Substring( 0, 5 ) + "&#xD;&#xA;" + newdata.Substring( 6, 4 );
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }


            ArrayList al = GetVigilanze( IDCliente.ToString() );

            foreach ( Hashtable item in al )
            {
                XmlManager x2 = new XmlManager();
                x2.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                XmlDataProviderManager _test = new XmlDataProviderManager( App.AppDataDataFolder + "\\" + item["File"].ToString() );

                x2.SaveEncodedFile( App.AppDataDataFolder + "\\" + item["File"].ToString(), _test.Document.OuterXml.Replace( olddata, newdata ) );

            }
        }

        public int SetVigilanza( Hashtable values, int IDVigilanza, int IDCliente )
        {
            try
            {
                string olddata = "";
                string newdata = "";
                bool changedatatbd = false;

                Open();

                if ( document.SelectNodes( "/ROOT/VIGILANZE" ) == null || document.SelectNodes( "/ROOT/VIGILANZE" ).Count == 0 )
                {
                    string xml = "<VIGILANZE LastID=\"1\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml( xml );

                    XmlNode tmpNode = doctmp.SelectSingleNode( "/VIGILANZE" );
                    XmlNode cliente = document.ImportNode( tmpNode, true );

                    document.SelectSingleNode( "/ROOT" ).AppendChild( cliente );
                }

                if ( IDVigilanza == App.MasterFile_NewID )
                {
                    XmlNode root = document.SelectSingleNode( "/ROOT/VIGILANZE" );

                    IDVigilanza = (Convert.ToInt32( root.Attributes["LastID"].Value ) + 1);

                    string lastindex = IDVigilanza.ToString();

                    //Template TREE
                    FileInfo fitree = new FileInfo( App.AppTemplateTreeVigilanza );
                    string estensione = "." + App.AppTemplateTreeVigilanza.Split( '.' ).Last();
                    string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewtree = new FileInfo( newNametree );

                    while ( fnewtree.Exists )
                    {
                        newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewtree = new FileInfo( newNametree );
                    }

                    fitree.CopyTo( newNametree );
                    newNametree = newNametree.Replace( App.AppDataDataFolder + "\\", "" );

                    //Template Dati
                    FileInfo fidati = new FileInfo( App.AppTemplateDataVigilanza );
                    string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewdati = new FileInfo( newNamedati );

                    while ( fnewdati.Exists )
                    {
                        newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewdati = new FileInfo( newNamedati );
                    }

                    fidati.CopyTo( newNamedati );
                    newNamedati = newNamedati.Replace( App.AppDataDataFolder + "\\", "" );

                    string xml = "<VIGILANZA ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ( (int)( App.TipoSessioneStato.Disponibile ) ).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Composizione=\"" + values["Composizione"].ToString() + "\" Inizio=\"" + values["Inizio"].ToString() + "\" Fine=\"" + values["Fine"].ToString() + "\" Luogo=\"" + values["Luogo"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" Revisore=\"" + values["Revisore"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" Presidente=\"" + values["Presidente"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" Sindaco1=\"" + values["Sindaco1"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" Sindaco2=\"" + values["Sindaco2"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" AssisitoDa=\"" + values["AssisitoDa"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" Data=\"" + values["Data"].ToString() + "\" DataEsecuzione=\"" + values["DataEsecuzione"].ToString() + "\" DataEsecuzione_Fine=\"" + values["DataEsecuzione_Fine"].ToString() + "\" />";

                    //string xml = "<VIGILANZA ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\"  Data=\"" + values["Data"].ToString() + "\"  />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml( xml );

                    XmlNode tmpNode = doctmp.SelectSingleNode( "/VIGILANZA" );
                    XmlNode cliente = document.ImportNode( tmpNode, true );

                    root.AppendChild( cliente );

                    root.Attributes["LastID"].Value = lastindex;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode( "/ROOT/VIGILANZE/VIGILANZA[@ID='" + IDVigilanza.ToString() + "']" );

                    if ( xNode.Attributes["Inizio"] == null )
                    {
                        XmlAttribute attr = document.CreateAttribute( "Inizio" );
                        xNode.Attributes.Append( attr );
                    }
                    xNode.Attributes["Inizio"].Value = ((values["Inizio"] == null) ? "" : values["Inizio"].ToString());

                    if ( xNode.Attributes["Fine"] == null )
                    {
                        XmlAttribute attr = document.CreateAttribute( "Fine" );
                        xNode.Attributes.Append( attr );
                    }
                    xNode.Attributes["Fine"].Value = ((values["Fine"] == null) ? "" : values["Fine"].ToString());

                    if ( xNode.Attributes["Luogo"] == null )
                    {
                        XmlAttribute attr = document.CreateAttribute( "Luogo" );
                        xNode.Attributes.Append( attr );
                    }
                    xNode.Attributes["Luogo"].Value = ((values["Luogo"] == null) ? "" : values["Luogo"].ToString());

                    if ( xNode.Attributes["Revisore"] == null )
                    {
                        XmlAttribute attr = document.CreateAttribute( "Revisore" );
                        xNode.Attributes.Append( attr );
                    }
                    xNode.Attributes["Revisore"].Value = ((values["Revisore"] == null) ? "" : values["Revisore"].ToString());

                    if ( xNode.Attributes["Presidente"] == null )
                    {
                        XmlAttribute attr = document.CreateAttribute( "Presidente" );
                        xNode.Attributes.Append( attr );
                    }
                    xNode.Attributes["Presidente"].Value = ((values["Presidente"] == null) ? "" : values["Presidente"].ToString());

                    if ( xNode.Attributes["Sindaco1"] == null )
                    {
                        XmlAttribute attr = document.CreateAttribute( "Sindaco1" );
                        xNode.Attributes.Append( attr );
                    }
                    xNode.Attributes["Sindaco1"].Value = ((values["Sindaco1"] == null) ? "" : values["Sindaco1"].ToString());

                    if ( xNode.Attributes["Sindaco2"] == null )
                    {
                        XmlAttribute attr = document.CreateAttribute( "Sindaco2" );
                        xNode.Attributes.Append( attr );
                    }
                    xNode.Attributes["Sindaco2"].Value = ((values["Sindaco2"] == null) ? "" : values["Sindaco2"].ToString());

                    if ( xNode.Attributes["AssisitoDa"] == null )
                    {
                        XmlAttribute attr = document.CreateAttribute( "AssisitoDa" );
                        xNode.Attributes.Append( attr );
                    }
                    xNode.Attributes["AssisitoDa"].Value = ((values["AssisitoDa"] == null) ? "" : values["AssisitoDa"].ToString());

                    if ( xNode.Attributes["Composizione"] == null )
                    {
                        XmlAttribute attr = document.CreateAttribute( "Composizione" );
                        xNode.Attributes.Append( attr );
                    }
                    xNode.Attributes["Composizione"].Value = ((values["Composizione"] == null) ? "" : values["Composizione"].ToString());

                    if ( xNode.Attributes["Data"].Value != values["Data"].ToString() )
                    {
                        olddata = xNode.Attributes["Data"].Value;
                        newdata = values["Data"].ToString();
                        changedatatbd = true;

                        xNode.Attributes["Data"].Value = values["Data"].ToString();
                    }

                    if (xNode.Attributes["DataEsecuzione"] == null)
                    {
                        XmlAttribute attr = document.CreateAttribute("DataEsecuzione");
                        xNode.Attributes.Append(attr);
                    }

                    xNode.Attributes["DataEsecuzione"].Value = ((values["DataEsecuzione"] == null) ? values["Data"].ToString() : values["DataEsecuzione"].ToString());


                    if ( xNode.Attributes["DataEsecuzione_Fine"] == null )
                    {
                        XmlAttribute attr = document.CreateAttribute( "DataEsecuzione_Fine" );
                        xNode.Attributes.Append( attr );
                    }

                    xNode.Attributes["DataEsecuzione_Fine"].Value = ((values["DataEsecuzione_Fine"] == null) ? values["Data"].ToString() : values["DataEsecuzione_Fine"].ToString()); 
                }

                Save();

                Close();

                if ( changedatatbd )
                {
                    SetDataVigilanza( olddata, newdata, IDVigilanza, IDCliente );
                }

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster );
            }

            return IDVigilanza;
        }

        public Hashtable GetVerificaAssociataFromVigilanza( string ID )
        {
            Hashtable result = new Hashtable();
            
            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/VIGILANZE/VIGILANZA[@ID='" + ID + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetVerifiche( xNode.Attributes["Cliente"].Value );
                    
                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            result = item;
                            break;
                        }
                    }
                }
            }

            Close();

            return result;
        }

#endregion

#region Flussi

        public int GetFlussiCount()
        {
            int result = 0;

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/FLUSSI/FLUSSO" );

                if ( xNodes != null )
                {
                    result = xNodes.Count;
                }

                Close();
            }

            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public Hashtable GetFlussi( string IDCliente )
        {
            Hashtable result = new Hashtable();

            try
            {
                Open();

                XmlNode node = document.SelectSingleNode( "/ROOT/FLUSSI/FLUSSO[@Cliente='" + IDCliente + "']" );

                foreach ( XmlAttribute item in node.Attributes )
                {
                    result.Add( item.Name, item.Value );
                }
                
                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public Hashtable GetFlussiFromFileData( string FileSessione )
        {
            Hashtable result = new Hashtable();

            FileSessione = FileSessione.Split( '\\' ).Last();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode( "/ROOT/FLUSSI/FLUSSO[@FileData='" + FileSessione + "']" );

                foreach ( XmlAttribute item in xNode.Attributes )
                {
                    result.Add( item.Name, item.Value );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public void AddFlussi( XmlNode node )
        {
            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/FLUSSI" );

            if ( xNode == null )
            {
                XmlNode xroot = document.SelectSingleNode( "/ROOT" );

                string xml = "<FLUSSI/>";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml( xml );
                XmlNode tmpNode = doctmp.SelectSingleNode( "/FLUSSI" );
                XmlNode xxtmp = document.ImportNode( tmpNode, true );
                xroot.AppendChild( xxtmp );

                xNode = document.SelectSingleNode( "/ROOT/FLUSSI" );
            }

            XmlNode xtmp = document.ImportNode( node, true );

            xNode.AppendChild( xtmp );

            Save();

            Close();

            return;
        }
        
        public bool CheckDoppio_Flussi( int IDCliente )
        {
            Open();

            XmlNodeList xNodes = document.SelectNodes( "/ROOT/FLUSSI/FLUSSO" );
            foreach ( XmlNode node in xNodes )
            {
                if ( node.Attributes["Cliente"].Value == IDCliente.ToString() )
                {
                    Close();
                    return false;
                }
            }

            Close();
            return true;
        }

        public void SetFlussi( Hashtable values, int IDCliente )
        {
            try
            {
                Open();

                XmlNode root = document.SelectSingleNode( "/ROOT/FLUSSI" );

                if ( root == null )
                {
                    XmlNode xroot = document.SelectSingleNode( "/ROOT" );

                    string xmll = "<FLUSSI LastID=\"0\" />";
                    XmlDocument doctmpl = new XmlDocument();
                    doctmpl.LoadXml( xmll );
                    XmlNode tmpNodel = doctmpl.SelectSingleNode( "/FLUSSI" );
                    XmlNode xxtmp = document.ImportNode( tmpNodel, true );
                    xroot.AppendChild( xxtmp );

                    root = document.SelectSingleNode( "/ROOT/FLUSSI" );
                }
                               

                //Template Dati
                string estensione = ".rflf";
                string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                FileInfo fnewdati = new FileInfo( newNamedati );

                while ( fnewdati.Exists )
                {
                    newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    fnewdati = new FileInfo( newNamedati );
                }
                
                string xmlflussi = "<FLUSSI><Dati TIPO=\"0\"></Dati><Dati TIPO=\"1\"></Dati><Dati TIPO=\"2\"></Dati><Dati TIPO=\"3\"></Dati></FLUSSI>";
                
                XmlManager xf = new XmlManager();
                xf.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
                xf.SaveEncodedFile(newNamedati, xmlflussi);

                newNamedati = newNamedati.Replace( App.AppDataDataFolder + "\\", "" );

                string xml = "<FLUSSO Cliente=\"" + IDCliente + "\" FileData=\"" + newNamedati + "\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml( xml );

                XmlNode tmpNode = doctmp.SelectSingleNode( "/FLUSSO" );
                XmlNode cliente = document.ImportNode( tmpNode, true );

                root.AppendChild( cliente );
                
                Save();

                Close();

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster );
            }

            return;
        }

#endregion

#region RelazioneV

        public int GetRelazioniVCount()
        {
            int result = 0;

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/RELAZIONIV/RELAZIONEV" );

                if ( xNodes != null )
                {
                    result = xNodes.Count;
                }

                Close();
            }

            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public ArrayList GetRelazioniV( string IDCliente )
        {
            ArrayList results = new ArrayList();

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/RELAZIONIV/RELAZIONEV[@Cliente='" + IDCliente + "']" );

                foreach ( XmlNode node in xNodes )
                {
                    Hashtable result = new Hashtable();

                    foreach ( XmlAttribute item in node.Attributes )
                    {
                        result.Add( item.Name, item.Value );
                    }

                    results.Add( result );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return results;
        }

        public Hashtable GetRelazioneV( string IDRelazioneV )
        {
            Hashtable result = new Hashtable();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIV/RELAZIONEV[@ID='" + IDRelazioneV + "']" );

                foreach ( XmlAttribute item in xNode.Attributes )
                {
                    result.Add( item.Name, item.Value );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public Hashtable GetRelazioneVFromFileData( string FileSessione )
        {
            Hashtable result = new Hashtable();

            FileSessione = FileSessione.Split( '\\' ).Last();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIV/RELAZIONEV[@FileData='" + FileSessione + "']" );

                foreach ( XmlAttribute item in xNode.Attributes )
                {
                    result.Add( item.Name, item.Value );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public Hashtable GetBilancioFromFileData(string FileSessione)
        {
            Hashtable result = new Hashtable();

            FileSessione = FileSessione.Split('\\').Last();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@FileData='" + FileSessione + "']");

                foreach (XmlAttribute item in xNode.Attributes)
                {
                    result.Add(item.Name, item.Value);
                }

                Close();
            }
            catch (Exception ex)
            {
                string log = ex.Message;
            }

            return result;
        }

        public Hashtable GetConclusioneFromFileData(string FileSessione)
        {
            Hashtable result = new Hashtable();

            FileSessione = FileSessione.Split('\\').Last();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI/CONCLUSIONE[@FileData='" + FileSessione + "']");

                foreach (XmlAttribute item in xNode.Attributes)
                {
                    result.Add(item.Name, item.Value);
                }

                Close();
            }
            catch (Exception ex)
            {
                string log = ex.Message;
            }

            return result;
        }

        public string AddRelazioneV( XmlNode node )
        {
            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIV" );

            if(xNode == null)
            {
                XmlNode xroot = document.SelectSingleNode( "/ROOT" );

                string xml = "<RELAZIONIV LastID=\"0\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml( xml );
                XmlNode tmpNode = doctmp.SelectSingleNode( "/RELAZIONIV" );
                XmlNode xxtmp = document.ImportNode( tmpNode, true );
                xroot.AppendChild( xxtmp );

                xNode = document.SelectSingleNode( "/ROOT/RELAZIONIV" );
            }

            if ( xNode.Attributes["LastID"].Value == "" )
            {
                xNode.Attributes["LastID"].Value = "0";
            }

            string ID = ( Convert.ToInt32( xNode.Attributes["LastID"].Value ) + 1 ).ToString();

            node.Attributes["ID"].Value = ID;

            XmlNode xtmp = document.ImportNode( node, true );

            xNode.AppendChild( xtmp );

            xNode.Attributes["LastID"].Value = ID;

            Save();

            Close();

            return ID;
        }

        public void DeleteRelazioneV( int IDRelazioneV )
        {
            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIV/RELAZIONEV[@ID='" + IDRelazioneV.ToString() + "']" );

                FileInfo fi = new FileInfo( App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value );
                if ( fi.Exists )
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo( App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value );
                if ( fd.Exists )
                {
                    fd.Delete();
                }

                xNode.ParentNode.RemoveChild( xNode );

                Save();

                //cancello allegati
                XmlManager xdoc = new XmlManager();
                xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
                XmlDocument xdoc_doc = xdoc.LoadEncodedFile( App.AppDocumentiDataFile );

                XmlNodeList xNodes = xdoc_doc.SelectNodes( "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDRelazioneV + "'][@Tree='" + ( Convert.ToInt32( App.TipoFile.RelazioneV ) ).ToString() + "']" );

                foreach ( XmlNode node in xNodes )
                {
                    FileInfo fis = new FileInfo( App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value );
                    if ( fis.Exists )
                    {
                        fis.Delete();
                    }

                    node.ParentNode.RemoveChild( node );
                }

                xdoc.SaveEncodedFile( App.AppDocumentiDataFile, xdoc_doc.InnerXml );

                Close();

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster );
            }
        }

        public bool CheckDoppio_RelazioneV( int ID, int IDCliente, string Data )
        {
            Open();

            XmlNodeList xNodes = document.SelectNodes( "/ROOT/RELAZIONIV/RELAZIONEV" );
            foreach ( XmlNode node in xNodes )
            {
                if ( node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data )
                {
                    Close();
                    return false;
                }
            }

            Close();
            return true;
        }
               
        public int SetRelazioneVIntermedio(Hashtable values, int IDCliente, string dal, string al)
        {
            int IDRelazioneV = -1;
            try
            {
                Open();

                XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIV");

                    if (root == null)
                    {
                        XmlNode xroot = document.SelectSingleNode("/ROOT");

                        string xmll = "<RELAZIONIV LastID=\"0\" />";
                        XmlDocument doctmpl = new XmlDocument();
                        doctmpl.LoadXml(xmll);
                        XmlNode tmpNodel = doctmpl.SelectSingleNode("/RELAZIONIV");
                        XmlNode xxtmp = document.ImportNode(tmpNodel, true);
                        xroot.AppendChild(xxtmp);

                        root = document.SelectSingleNode("/ROOT/RELAZIONIV");
                    }

                    IDRelazioneV = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

                    string lastindex = IDRelazioneV.ToString();

                    //Template TREE
                    FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneV);
                    string estensione = "." + App.AppTemplateTreeRelazioneV.Split('.').Last();
                    string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewtree = new FileInfo(newNametree);

                    while (fnewtree.Exists)
                    {
                        newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewtree = new FileInfo(newNametree);
                    }

                    fitree.CopyTo(newNametree);
                    newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

                    //Template Dati
                    FileInfo fidati = new FileInfo(App.AppTemplateDataRelazioneV);
                    string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewdati = new FileInfo(newNamedati);

                    while (fnewdati.Exists)
                    {
                        newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewdati = new FileInfo(newNamedati);
                    }

                    fidati.CopyTo(newNamedati);
                    newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");

                    XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

                    string xml = "<RELAZIONEV ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml(xml);

                    XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONEV");
                    XmlNode cliente = document.ImportNode(tmpNode, true);

                    root.AppendChild(cliente);

                    root.Attributes["LastID"].Value = lastindex;
               

                Save();

                Close();

            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
            }

            return IDRelazioneV;
        }

        public int SetRelazioneV( Hashtable values, int IDRelazioneV, int IDCliente )
        {
            try
            {
                Open();

                if ( IDRelazioneV == App.MasterFile_NewID )
                {
                    XmlNode root = document.SelectSingleNode( "/ROOT/RELAZIONIV" );

                    if ( root == null )
                    {
                        XmlNode xroot = document.SelectSingleNode( "/ROOT" );

                        string xmll = "<RELAZIONIV LastID=\"0\" />";
                        XmlDocument doctmpl = new XmlDocument();
                        doctmpl.LoadXml( xmll );
                        XmlNode tmpNodel = doctmpl.SelectSingleNode( "/RELAZIONIV" );
                        XmlNode xxtmp = document.ImportNode( tmpNodel, true );
                        xroot.AppendChild( xxtmp );

                        root = document.SelectSingleNode( "/ROOT/RELAZIONIV" );
                    }

                    IDRelazioneV = ( Convert.ToInt32( root.Attributes["LastID"].Value ) + 1 );

                    string lastindex = IDRelazioneV.ToString();

                    //Template TREE
                    FileInfo fitree = new FileInfo( App.AppTemplateTreeRelazioneV );
                    string estensione = "." + App.AppTemplateTreeRelazioneV.Split( '.' ).Last();
                    string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewtree = new FileInfo( newNametree );

                    while ( fnewtree.Exists )
                    {
                        newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewtree = new FileInfo( newNametree );
                    }

                    fitree.CopyTo( newNametree );
                    newNametree = newNametree.Replace( App.AppDataDataFolder + "\\", "" );

                    //Template Dati
                    FileInfo fidati = new FileInfo( App.AppTemplateDataRelazioneV );
                    string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewdati = new FileInfo( newNamedati );

                    while ( fnewdati.Exists )
                    {
                        newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewdati = new FileInfo( newNamedati );
                    }

                    fidati.CopyTo( newNamedati );
                    newNamedati = newNamedati.Replace( App.AppDataDataFolder + "\\", "" );

                    XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

                    string xml = "<RELAZIONEV ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ( (int)( App.TipoSessioneStato.Disponibile ) ).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml( xml );

                    XmlNode tmpNode = doctmp.SelectSingleNode( "/RELAZIONEV" );
                    XmlNode cliente = document.ImportNode( tmpNode, true );

                    root.AppendChild( cliente );

                    root.Attributes["LastID"].Value = lastindex;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIV/RELAZIONEV[@ID='" + IDRelazioneV.ToString() + "']" );

                    xNode.Attributes["Note"].Value = values["Note"].ToString();
                    xNode.Attributes["Data"].Value = values["Data"].ToString();
                }

                Save();

                Close();

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster );
            }

            return IDRelazioneV;
        }


        public string GetRevisioneAssociataFromRelazioneVFile( string FileRelazioneV )
        {
            string FileRevisione = "";

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIV/RELAZIONEV[@FileData='" + FileRelazioneV.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetRevisioni( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            FileRevisione = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileRevisione;
        }

        public string GetBilancioAssociatoFromRelazioneVFile( string FileRelazioneV )
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIV/RELAZIONEV[@FileData='" + FileRelazioneV.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetBilanci( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            FileBilancio = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public string GetBilancioAssociatoFromConclusioniFile(string FileConclusioni)
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode("/ROOT/CONCLUSIONI/CONCLUSIONE[@FileData='" + FileConclusioni.Split('\\').Last() + "']");

            if (xNode != null)
            {
                if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
                {
                    ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);

                    foreach (Hashtable item in al)
                    {
                        if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
                        {
                            FileBilancio = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public string GetBilancioTreeAssociatoFromRelazioneVFile( string FileRelazioneV )
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIV/RELAZIONEV[@FileData='" + FileRelazioneV.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetBilanci( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            FileBilancio = App.AppDataDataFolder + "\\" + item["File"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public string GetBilancioIDAssociatoFromRelazioneVFile( string FileRelazioneV )
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIV/RELAZIONEV[@FileData='" + FileRelazioneV.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetBilanci( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            FileBilancio = item["ID"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public Hashtable GetAllRelazioneVAssociataFromBilancioFile( string FileBilancio )
        {
            Hashtable RelazioneV = null;

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/BILANCI/BILANCIO[@FileData='" + FileBilancio.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetRelazioniV( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            RelazioneV = item;
                            break;
                        }
                    }
                }
            }

            Close();

            return RelazioneV;
        }

        public Hashtable GetAllBilancioAssociatoFromRelazioneVFile( string FileRelazioneV )
        {
            Hashtable Bilancio = null;

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIV/RELAZIONEV[@FileData='" + FileRelazioneV.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetBilanci( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            Bilancio = item;
                            break;
                        }
                    }
                }
            }

            Close();

            return Bilancio;
        }
#endregion

#region RelazioneB

        public int GetRelazioniBCount()
        {
            int result = 0;

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/RELAZIONIB/RELAZIONEB" );

                if ( xNodes != null )
                {
                    result = xNodes.Count;
                }

                Close();
            }

            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }


        public ArrayList GetRelazioniB( string IDCliente )
        {
            ArrayList results = new ArrayList();

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/RELAZIONIB/RELAZIONEB[@Cliente='" + IDCliente + "']" );

                foreach ( XmlNode node in xNodes )
                {
                    Hashtable result = new Hashtable();

                    foreach ( XmlAttribute item in node.Attributes )
                    {
                        result.Add( item.Name, item.Value );
                    }

                    results.Add( result );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return results;
        }

        public Hashtable GetRelazioneB( string IDRelazioneB )
        {
            Hashtable result = new Hashtable();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIB/RELAZIONEB[@ID='" + IDRelazioneB + "']" );

                foreach ( XmlAttribute item in xNode.Attributes )
                {
                    result.Add( item.Name, item.Value );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public Hashtable GetRelazioneBFromFileData( string FileSessione )
        {
            Hashtable result = new Hashtable();

            FileSessione = FileSessione.Split( '\\' ).Last();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIB/RELAZIONEB[@FileData='" + FileSessione + "']" );

                foreach ( XmlAttribute item in xNode.Attributes )
                {
                    result.Add( item.Name, item.Value );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public string AddRelazioneB( XmlNode node )
        {
            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIB" );

            if ( xNode == null )
            {
                XmlNode xroot = document.SelectSingleNode( "/ROOT" );

                string xml = "<RELAZIONIB LastID=\"0\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml( xml );
                XmlNode tmpNode = doctmp.SelectSingleNode( "/RELAZIONIB" );
                XmlNode xxtmp = document.ImportNode( tmpNode, true );
                xroot.AppendChild( xxtmp );

                xNode = document.SelectSingleNode( "/ROOT/RELAZIONIB" );
            }

            if(xNode.Attributes["LastID"].Value == "")
            {
                xNode.Attributes["LastID"].Value = "0";
            }

            string ID = ( Convert.ToInt32( xNode.Attributes["LastID"].Value ) + 1 ).ToString();

            node.Attributes["ID"].Value = ID;

            XmlNode xtmp = document.ImportNode( node, true );

            xNode.AppendChild( xtmp );

            xNode.Attributes["LastID"].Value = ID;

            Save();

            Close();

            return ID;
        }

        public void DeleteRelazioneB( int IDRelazioneB )
        {
            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIB/RELAZIONEB[@ID='" + IDRelazioneB.ToString() + "']" );

                FileInfo fi = new FileInfo( App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value );
                if ( fi.Exists )
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo( App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value );
                if ( fd.Exists )
                {
                    fd.Delete();
                }

                xNode.ParentNode.RemoveChild( xNode );

                Save();

                //cancello allegati
                XmlManager xdoc = new XmlManager();
                xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
                XmlDocument xdoc_doc = xdoc.LoadEncodedFile( App.AppDocumentiDataFile );

                XmlNodeList xNodes = xdoc_doc.SelectNodes( "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDRelazioneB + "'][@Tree='" + ( Convert.ToInt32( App.TipoFile.RelazioneB ) ).ToString() + "']" );

                foreach ( XmlNode node in xNodes )
                {
                    FileInfo fis = new FileInfo( App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value );
                    if ( fis.Exists )
                    {
                        fis.Delete();
                    }

                    node.ParentNode.RemoveChild( node );
                }

                xdoc.SaveEncodedFile( App.AppDocumentiDataFile, xdoc_doc.InnerXml );

                Close();

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster );
            }
        }

        public bool CheckDoppio_RelazioneB( int ID, int IDCliente, string Data )
        {
            Open();

            XmlNodeList xNodes = document.SelectNodes( "/ROOT/RELAZIONIB/RELAZIONEB" );
            foreach ( XmlNode node in xNodes )
            {
                if ( node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data )
                {
                    Close();
                    return false;
                }
            }

            Close();
            return true;
        }
        
        public int SetRelazioneBIntermedio(Hashtable values, int IDCliente, string dal, string al)
        {
            int IDRelazioneB = -1;
            try
            {
                Open();

                
                    XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIB");

                    if (root == null)
                    {
                        XmlNode xroot = document.SelectSingleNode("/ROOT");

                        string xmla = "<RELAZIONIB LastID=\"0\" />";
                        XmlDocument doctmpa = new XmlDocument();
                        doctmpa.LoadXml(xmla);
                        XmlNode tmpNodea = doctmpa.SelectSingleNode("/RELAZIONIB");
                        XmlNode xxtmp = document.ImportNode(tmpNodea, true);
                        xroot.AppendChild(xxtmp);

                        root = document.SelectSingleNode("/ROOT/RELAZIONIB");
                    }

                    IDRelazioneB = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

                    string lastindex = IDRelazioneB.ToString();

                    //Template TREE
                    FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneB);
                    string estensione = "." + App.AppTemplateTreeRelazioneB.Split('.').Last();
                    string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewtree = new FileInfo(newNametree);

                    while (fnewtree.Exists)
                    {
                        newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewtree = new FileInfo(newNametree);
                    }

                    fitree.CopyTo(newNametree);
                    newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

                    //Template Dati
                    FileInfo fidati = new FileInfo(App.AppTemplateDataRelazioneB);
                    string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewdati = new FileInfo(newNamedati);

                    while (fnewdati.Exists)
                    {
                        newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewdati = new FileInfo(newNamedati);
                    }

                    fidati.CopyTo(newNamedati);
                    newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");

                    XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

                    string xml = "<RELAZIONEB ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml(xml);

                    XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONEB");
                    XmlNode cliente = document.ImportNode(tmpNode, true);

                    root.AppendChild(cliente);

                    root.Attributes["LastID"].Value = lastindex;
               

                Save();

                Close();

            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
            }

            return IDRelazioneB;
        }

        public int SetRelazioneB( Hashtable values, int IDRelazioneB, int IDCliente )
        {
            try
            {
                Open();

                if ( IDRelazioneB == App.MasterFile_NewID )
                {
                    XmlNode root = document.SelectSingleNode( "/ROOT/RELAZIONIB" );

                    if ( root == null )
                    {
                        XmlNode xroot = document.SelectSingleNode( "/ROOT" );

                        string xmla = "<RELAZIONIB LastID=\"0\" />";
                        XmlDocument doctmpa = new XmlDocument();
                        doctmpa.LoadXml( xmla );
                        XmlNode tmpNodea = doctmpa.SelectSingleNode( "/RELAZIONIB" );
                        XmlNode xxtmp = document.ImportNode( tmpNodea, true );
                        xroot.AppendChild( xxtmp );

                        root = document.SelectSingleNode( "/ROOT/RELAZIONIB" );
                    }

                    IDRelazioneB = ( Convert.ToInt32( root.Attributes["LastID"].Value ) + 1 );

                    string lastindex = IDRelazioneB.ToString();

                    //Template TREE
                    FileInfo fitree = new FileInfo( App.AppTemplateTreeRelazioneB );
                    string estensione = "." + App.AppTemplateTreeRelazioneB.Split( '.' ).Last();
                    string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewtree = new FileInfo( newNametree );

                    while ( fnewtree.Exists )
                    {
                        newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewtree = new FileInfo( newNametree );
                    }

                    fitree.CopyTo( newNametree );
                    newNametree = newNametree.Replace( App.AppDataDataFolder + "\\", "" );

                    //Template Dati
                    FileInfo fidati = new FileInfo( App.AppTemplateDataRelazioneB );
                    string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewdati = new FileInfo( newNamedati );

                    while ( fnewdati.Exists )
                    {
                        newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewdati = new FileInfo( newNamedati );
                    }

                    fidati.CopyTo( newNamedati );
                    newNamedati = newNamedati.Replace( App.AppDataDataFolder + "\\", "" );

                    XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

                    string xml = "<RELAZIONEB ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ( (int)( App.TipoSessioneStato.Disponibile ) ).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml( xml );

                    XmlNode tmpNode = doctmp.SelectSingleNode( "/RELAZIONEB" );
                    XmlNode cliente = document.ImportNode( tmpNode, true );

                    root.AppendChild( cliente );

                    root.Attributes["LastID"].Value = lastindex;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIB/RELAZIONEB[@ID='" + IDRelazioneB.ToString() + "']" );

                    xNode.Attributes["Note"].Value = values["Note"].ToString();
                    xNode.Attributes["Data"].Value = values["Data"].ToString();
                }

                Save();

                Close();

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster );
            }

            return IDRelazioneB;
        }

        public string GetRevisioneAssociataFromRelazioneBFile( string FileRelazioneV )
        {
            string FileRevisione = "";

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIB/RELAZIONEB[@FileData='" + FileRelazioneV.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetRevisioni( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            FileRevisione = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileRevisione;
        }

        public string GetBilancioAssociatoFromRelazioneBFile( string FileRelazioneB )
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIB/RELAZIONEB[@FileData='" + FileRelazioneB.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetBilanci( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            FileBilancio = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public string GetBilancioTreeAssociatoFromRelazioneBFile( string FileRelazioneB )
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIB/RELAZIONEB[@FileData='" + FileRelazioneB.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetBilanci( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            FileBilancio = App.AppDataDataFolder + "\\" + item["File"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public string GetBilancioIDAssociatoFromRelazioneBFile( string FileRelazioneB )
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIB/RELAZIONEB[@FileData='" + FileRelazioneB.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetBilanci( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            FileBilancio = item["ID"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public Hashtable GetAllRelazioneBAssociataFromBilancioFile( string FileBilancio )
        {
            Hashtable RelazioneB = null;

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/BILANCI/BILANCIO[@FileData='" + FileBilancio.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetRelazioniB( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            RelazioneB = item;
                            break;
                        }
                    }
                }
            }

            Close();

            return RelazioneB;
        }

        public Hashtable GetAllBilancioAssociatoFromRelazioneBFile( string FileRelazioneB )
        {
            Hashtable Bilancio = null;

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIB/RELAZIONEB[@FileData='" + FileRelazioneB.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetBilanci( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            Bilancio = item;
                            break;
                        }
                    }
                }
            }

            Close();

            return Bilancio;
        }
#endregion





#region RelazioneVC

        public int GetRelazioniVCCount()
        {
            int result = 0;

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC");

                if (xNodes != null)
                {
                    result = xNodes.Count;
                }

                Close();
            }

            catch (Exception ex)
            {
                string log = ex.Message;
            }

            return result;
        }

        public ArrayList GetRelazioniVC(string IDCliente)
        {
            ArrayList results = new ArrayList();

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC[@Cliente='" + IDCliente + "']");

                foreach (XmlNode node in xNodes)
                {
                    Hashtable result = new Hashtable();

                    foreach (XmlAttribute item in node.Attributes)
                    {
                        result.Add(item.Name, item.Value);
                    }

                    results.Add(result);
                }

                Close();
            }
            catch (Exception ex)
            {
                string log = ex.Message;
            }

            return results;
        }

        public Hashtable GetRelazioneVC(string IDRelazioneVC)
        {
            Hashtable result = new Hashtable();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@ID='" + IDRelazioneVC + "']");

                foreach (XmlAttribute item in xNode.Attributes)
                {
                    result.Add(item.Name, item.Value);
                }

                Close();
            }
            catch (Exception ex)
            {
                string log = ex.Message;
            }

            return result;
        }

        public Hashtable GetRelazioneVCFromFileData(string FileSessione)
        {
            Hashtable result = new Hashtable();

            FileSessione = FileSessione.Split('\\').Last();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@FileData='" + FileSessione + "']");

                foreach (XmlAttribute item in xNode.Attributes)
                {
                    result.Add(item.Name, item.Value);
                }

                Close();
            }
            catch (Exception ex)
            {
                string log = ex.Message;
            }

            return result;
        }

        public string AddRelazioneVC(XmlNode node)
        {
            Open();

            XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC");

            if (xNode == null)
            {
                XmlNode xroot = document.SelectSingleNode("/ROOT");

                string xml = "<RELAZIONIVC LastID=\"0\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);
                XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIVC");
                XmlNode xxtmp = document.ImportNode(tmpNode, true);
                xroot.AppendChild(xxtmp);

                xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC");
            }

            if (xNode.Attributes["LastID"].Value == "")
            {
                xNode.Attributes["LastID"].Value = "0";
            }

            string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();

            node.Attributes["ID"].Value = ID;

            XmlNode xtmp = document.ImportNode(node, true);

            xNode.AppendChild(xtmp);

            xNode.Attributes["LastID"].Value = ID;

            Save();

            Close();

            return ID;
        }

        public void DeleteRelazioneVC(int IDRelazioneVC)
        {
            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@ID='" + IDRelazioneVC.ToString() + "']");

                FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
                if (fi.Exists)
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
                if (fd.Exists)
                {
                    fd.Delete();
                }

                xNode.ParentNode.RemoveChild(xNode);

                Save();

                //cancello allegati
                XmlManager xdoc = new XmlManager();
                xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
                XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);

                XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDRelazioneVC + "'][@Tree='" + (Convert.ToInt32(App.TipoFile.RelazioneVC)).ToString() + "']");

                foreach (XmlNode node in xNodes)
                {
                    FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
                    if (fis.Exists)
                    {
                        fis.Delete();
                    }

                    node.ParentNode.RemoveChild(node);
                }

                xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);

                Close();

            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
            }
        }

        public bool CheckDoppio_RelazioneVC(int ID, int IDCliente, string Data)
        {
            Open();

            XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIVC/RELAZIONEVC");
            foreach (XmlNode node in xNodes)
            {
                if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data)
                {
                    Close();
                    return false;
                }
            }

            Close();
            return true;
        }

        public int SetRelazioneVCIntermedio(Hashtable values, int IDCliente, string dal, string al)
        {
            int IDRelazioneVC = -1;
            try
            {
                Open();

                XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIVC");

                if (root == null)
                {
                    XmlNode xroot = document.SelectSingleNode("/ROOT");

                    string xmll = "<RELAZIONIVC LastID=\"0\" />";
                    XmlDocument doctmpl = new XmlDocument();
                    doctmpl.LoadXml(xmll);
                    XmlNode tmpNodel = doctmpl.SelectSingleNode("/RELAZIONIVC");
                    XmlNode xxtmp = document.ImportNode(tmpNodel, true);
                    xroot.AppendChild(xxtmp);

                    root = document.SelectSingleNode("/ROOT/RELAZIONIVC");
                }

                IDRelazioneVC = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

                string lastindex = IDRelazioneVC.ToString();

                //Template TREE
                FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneVC);
                string estensione = "." + App.AppTemplateTreeRelazioneVC.Split('.').Last();
                string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                FileInfo fnewtree = new FileInfo(newNametree);

                while (fnewtree.Exists)
                {
                    newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    fnewtree = new FileInfo(newNametree);
                }

                fitree.CopyTo(newNametree);
                newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

                //Template Dati
                FileInfo fidati = new FileInfo(App.AppTemplateDataRelazioneVC);
                string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                FileInfo fnewdati = new FileInfo(newNamedati);

                while (fnewdati.Exists)
                {
                    newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    fnewdati = new FileInfo(newNamedati);
                }

                fidati.CopyTo(newNamedati);
                newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");

                XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

                string xml = "<RELAZIONEVC ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);

                XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONEVC");
                XmlNode cliente = document.ImportNode(tmpNode, true);

                root.AppendChild(cliente);

                root.Attributes["LastID"].Value = lastindex;


                Save();

                Close();

            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
            }

            return IDRelazioneVC;
        }

        public int SetRelazioneVC(Hashtable values, int IDRelazioneVC, int IDCliente)
        {
            try
            {
                Open();

                if (IDRelazioneVC == App.MasterFile_NewID)
                {
                    XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIVC");

                    if (root == null)
                    {
                        XmlNode xroot = document.SelectSingleNode("/ROOT");

                        string xmll = "<RELAZIONIVC LastID=\"0\" />";
                        XmlDocument doctmpl = new XmlDocument();
                        doctmpl.LoadXml(xmll);
                        XmlNode tmpNodel = doctmpl.SelectSingleNode("/RELAZIONIVC");
                        XmlNode xxtmp = document.ImportNode(tmpNodel, true);
                        xroot.AppendChild(xxtmp);

                        root = document.SelectSingleNode("/ROOT/RELAZIONIVC");
                    }

                    IDRelazioneVC = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

                    string lastindex = IDRelazioneVC.ToString();

                    //Template TREE
                    FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneVC);
                    string estensione = "." + App.AppTemplateTreeRelazioneVC.Split('.').Last();
                    string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewtree = new FileInfo(newNametree);

                    while (fnewtree.Exists)
                    {
                        newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewtree = new FileInfo(newNametree);
                    }

                    fitree.CopyTo(newNametree);
                    newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

                    //Template Dati
                    FileInfo fidati = new FileInfo(App.AppTemplateDataRelazioneVC);
                    string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewdati = new FileInfo(newNamedati);

                    while (fnewdati.Exists)
                    {
                        newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewdati = new FileInfo(newNamedati);
                    }

                    fidati.CopyTo(newNamedati);
                    newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");

                    XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

                    string xml = "<RELAZIONEVC ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml(xml);

                    XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONEVC");
                    XmlNode cliente = document.ImportNode(tmpNode, true);

                    root.AppendChild(cliente);

                    root.Attributes["LastID"].Value = lastindex;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@ID='" + IDRelazioneVC.ToString() + "']");

                    xNode.Attributes["Note"].Value = values["Note"].ToString();
                    xNode.Attributes["Data"].Value = values["Data"].ToString();
                }

                Save();

                Close();

            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
            }

            return IDRelazioneVC;
        }


        public string GetRevisioneAssociataFromRelazioneVCFile(string FileRelazioneVC)
        {
            string FileRevisione = "";

            Open();

            XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@FileData='" + FileRelazioneVC.Split('\\').Last() + "']");

            if (xNode != null)
            {
                if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
                {
                    ArrayList al = GetRevisioni(xNode.Attributes["Cliente"].Value);

                    foreach (Hashtable item in al)
                    {
                        if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
                        {
                            FileRevisione = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileRevisione;
        }

        public string GetBilancioAssociatoFromRelazioneVCFile(string FileRelazioneVC)
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@FileData='" + FileRelazioneVC.Split('\\').Last() + "']");

            if (xNode != null)
            {
                if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
                {
                    ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);

                    foreach (Hashtable item in al)
                    {
                        if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
                        {
                            FileBilancio = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public string GetBilancioTreeAssociatoFromRelazioneVCFile(string FileRelazioneVC)
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@FileData='" + FileRelazioneVC.Split('\\').Last() + "']");

            if (xNode != null)
            {
                if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
                {
                    ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);

                    foreach (Hashtable item in al)
                    {
                        if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
                        {
                            FileBilancio = App.AppDataDataFolder + "\\" + item["File"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public string GetBilancioIDAssociatoFromRelazioneVCFile(string FileRelazioneVC)
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@FileData='" + FileRelazioneVC.Split('\\').Last() + "']");

            if (xNode != null)
            {
                if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
                {
                    ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);

                    foreach (Hashtable item in al)
                    {
                        if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
                        {
                            FileBilancio = item["ID"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public Hashtable GetAllRelazioneVCAssociataFromBilancioFile(string FileBilancio)
        {
            Hashtable RelazioneVC = null;

            Open();

            XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@FileData='" + FileBilancio.Split('\\').Last() + "']");

            if (xNode != null)
            {
                if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
                {
                    ArrayList al = GetRelazioniVC(xNode.Attributes["Cliente"].Value);

                    foreach (Hashtable item in al)
                    {
                        if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
                        {
                            RelazioneVC = item;
                            break;
                        }
                    }
                }
            }

            Close();

            return RelazioneVC;
        }

        public Hashtable GetAllBilancioAssociatoFromRelazioneVCFile(string FileRelazioneVC)
        {
            Hashtable Bilancio = null;

            Open();

            XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIVC/RELAZIONEVC[@FileData='" + FileRelazioneVC.Split('\\').Last() + "']");

            if (xNode != null)
            {
                if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
                {
                    ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);

                    foreach (Hashtable item in al)
                    {
                        if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
                        {
                            Bilancio = item;
                            break;
                        }
                    }
                }
            }

            Close();

            return Bilancio;
        }
#endregion

#region RelazioneBC

        public int GetRelazioniBCCount()
        {
            int result = 0;

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC");

                if (xNodes != null)
                {
                    result = xNodes.Count;
                }

                Close();
            }

            catch (Exception ex)
            {
                string log = ex.Message;
            }

            return result;
        }


        public ArrayList GetRelazioniBC(string IDCliente)
        {
            ArrayList results = new ArrayList();

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC[@Cliente='" + IDCliente + "']");

                foreach (XmlNode node in xNodes)
                {
                    Hashtable result = new Hashtable();

                    foreach (XmlAttribute item in node.Attributes)
                    {
                        result.Add(item.Name, item.Value);
                    }

                    results.Add(result);
                }

                Close();
            }
            catch (Exception ex)
            {
                string log = ex.Message;
            }

            return results;
        }

        public Hashtable GetRelazioneBC(string IDRelazioneBC)
        {
            Hashtable result = new Hashtable();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@ID='" + IDRelazioneBC + "']");

                foreach (XmlAttribute item in xNode.Attributes)
                {
                    result.Add(item.Name, item.Value);
                }

                Close();
            }
            catch (Exception ex)
            {
                string log = ex.Message;
            }

            return result;
        }

        public Hashtable GetRelazioneBCFromFileData(string FileSessione)
        {
            Hashtable result = new Hashtable();

            FileSessione = FileSessione.Split('\\').Last();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@FileData='" + FileSessione + "']");

                foreach (XmlAttribute item in xNode.Attributes)
                {
                    result.Add(item.Name, item.Value);
                }

                Close();
            }
            catch (Exception ex)
            {
                string log = ex.Message;
            }

            return result;
        }

        public string AddRelazioneBC(XmlNode node)
        {
            Open();

            XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC");

            if (xNode == null)
            {
                XmlNode xroot = document.SelectSingleNode("/ROOT");

                string xml = "<RELAZIONIBC LastID=\"0\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);
                XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONIBC");
                XmlNode xxtmp = document.ImportNode(tmpNode, true);
                xroot.AppendChild(xxtmp);

                xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC");
            }

            if (xNode.Attributes["LastID"].Value == "")
            {
                xNode.Attributes["LastID"].Value = "0";
            }

            string ID = (Convert.ToInt32(xNode.Attributes["LastID"].Value) + 1).ToString();

            node.Attributes["ID"].Value = ID;

            XmlNode xtmp = document.ImportNode(node, true);

            xNode.AppendChild(xtmp);

            xNode.Attributes["LastID"].Value = ID;

            Save();

            Close();

            return ID;
        }

        public void DeleteRelazioneBC(int IDRelazioneBC)
        {
            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@ID='" + IDRelazioneBC.ToString() + "']");

                FileInfo fi = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value);
                if (fi.Exists)
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo(App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value);
                if (fd.Exists)
                {
                    fd.Delete();
                }

                xNode.ParentNode.RemoveChild(xNode);

                Save();

                //cancello allegati
                XmlManager xdoc = new XmlManager();
                xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
                XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);

                XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTI/DOCUMENTO[@Sessione='" + IDRelazioneBC + "'][@Tree='" + (Convert.ToInt32(App.TipoFile.RelazioneBC)).ToString() + "']");

                foreach (XmlNode node in xNodes)
                {
                    FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
                    if (fis.Exists)
                    {
                        fis.Delete();
                    }

                    node.ParentNode.RemoveChild(node);
                }

                xdoc.SaveEncodedFile(App.AppDocumentiDataFile, xdoc_doc.InnerXml);

                Close();

            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
            }
        }

        public bool CheckDoppio_RelazioneBC(int ID, int IDCliente, string Data)
        {
            Open();

            XmlNodeList xNodes = document.SelectNodes("/ROOT/RELAZIONIBC/RELAZIONEBC");
            foreach (XmlNode node in xNodes)
            {
                if (node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data)
                {
                    Close();
                    return false;
                }
            }

            Close();
            return true;
        }

        public int SetRelazioneBCIntermedio(Hashtable values, int IDCliente, string dal, string al)
        {
            int IDRelazioneBC = -1;
            try
            {
                Open();


                XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIBC");

                if (root == null)
                {
                    XmlNode xroot = document.SelectSingleNode("/ROOT");

                    string xmla = "<RELAZIONIBC LastID=\"0\" />";
                    XmlDocument doctmpa = new XmlDocument();
                    doctmpa.LoadXml(xmla);
                    XmlNode tmpNodea = doctmpa.SelectSingleNode("/RELAZIONIBC");
                    XmlNode xxtmp = document.ImportNode(tmpNodea, true);
                    xroot.AppendChild(xxtmp);

                    root = document.SelectSingleNode("/ROOT/RELAZIONIBC");
                }

                IDRelazioneBC = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

                string lastindex = IDRelazioneBC.ToString();

                //Template TREE
                FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneBC);
                string estensione = "." + App.AppTemplateTreeRelazioneBC.Split('.').Last();
                string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                FileInfo fnewtree = new FileInfo(newNametree);

                while (fnewtree.Exists)
                {
                    newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    fnewtree = new FileInfo(newNametree);
                }

                fitree.CopyTo(newNametree);
                newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

                //Template Dati
                FileInfo fidati = new FileInfo(App.AppTemplateDataRelazioneBC);
                string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                FileInfo fnewdati = new FileInfo(newNamedati);

                while (fnewdati.Exists)
                {
                    newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    fnewdati = new FileInfo(newNamedati);
                }

                fidati.CopyTo(newNamedati);
                newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");

                XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

                string xml = "<RELAZIONEBC ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);

                XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONEBC");
                XmlNode cliente = document.ImportNode(tmpNode, true);

                root.AppendChild(cliente);

                root.Attributes["LastID"].Value = lastindex;


                Save();

                Close();

            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
            }

            return IDRelazioneBC;
        }

        public int SetRelazioneBC(Hashtable values, int IDRelazioneBC, int IDCliente)
        {
            try
            {
                Open();

                if (IDRelazioneBC == App.MasterFile_NewID)
                {
                    XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIBC");

                    if (root == null)
                    {
                        XmlNode xroot = document.SelectSingleNode("/ROOT");

                        string xmla = "<RELAZIONIBC LastID=\"0\" />";
                        XmlDocument doctmpa = new XmlDocument();
                        doctmpa.LoadXml(xmla);
                        XmlNode tmpNodea = doctmpa.SelectSingleNode("/RELAZIONIBC");
                        XmlNode xxtmp = document.ImportNode(tmpNodea, true);
                        xroot.AppendChild(xxtmp);

                        root = document.SelectSingleNode("/ROOT/RELAZIONIBC");
                    }

                    IDRelazioneBC = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

                    string lastindex = IDRelazioneBC.ToString();

                    //Template TREE
                    FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneBC);
                    string estensione = "." + App.AppTemplateTreeRelazioneBC.Split('.').Last();
                    string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewtree = new FileInfo(newNametree);

                    while (fnewtree.Exists)
                    {
                        newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewtree = new FileInfo(newNametree);
                    }

                    fitree.CopyTo(newNametree);
                    newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

                    //Template Dati
                    FileInfo fidati = new FileInfo(App.AppTemplateDataRelazioneBC);
                    string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewdati = new FileInfo(newNamedati);

                    while (fnewdati.Exists)
                    {
                        newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewdati = new FileInfo(newNamedati);
                    }

                    fidati.CopyTo(newNamedati);
                    newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");

                    XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

                    string xml = "<RELAZIONEBC ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml(xml);

                    XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONEBC");
                    XmlNode cliente = document.ImportNode(tmpNode, true);

                    root.AppendChild(cliente);

                    root.Attributes["LastID"].Value = lastindex;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@ID='" + IDRelazioneBC.ToString() + "']");

                    xNode.Attributes["Note"].Value = values["Note"].ToString();
                    xNode.Attributes["Data"].Value = values["Data"].ToString();
                }

                Save();

                Close();

            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
            }

            return IDRelazioneBC;
        }

        public string GetRevisioneAssociataFromRelazioneBCFile(string FileRelazioneV)
        {
            string FileRevisione = "";

            Open();

            XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@FileData='" + FileRelazioneV.Split('\\').Last() + "']");

            if (xNode != null)
            {
                if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
                {
                    ArrayList al = GetRevisioni(xNode.Attributes["Cliente"].Value);

                    foreach (Hashtable item in al)
                    {
                        if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
                        {
                            FileRevisione = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileRevisione;
        }

        public string GetBilancioAssociatoFromRelazioneBCFile(string FileRelazioneBC)
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@FileData='" + FileRelazioneBC.Split('\\').Last() + "']");

            if (xNode != null)
            {
                if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
                {
                    ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);

                    foreach (Hashtable item in al)
                    {
                        if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
                        {
                            FileBilancio = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public string GetBilancioTreeAssociatoFromRelazioneBCFile(string FileRelazioneBC)
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@FileData='" + FileRelazioneBC.Split('\\').Last() + "']");

            if (xNode != null)
            {
                if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
                {
                    ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);

                    foreach (Hashtable item in al)
                    {
                        if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
                        {
                            FileBilancio = App.AppDataDataFolder + "\\" + item["File"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public string GetBilancioIDAssociatoFromRelazioneBCFile(string FileRelazioneBC)
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@FileData='" + FileRelazioneBC.Split('\\').Last() + "']");

            if (xNode != null)
            {
                if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
                {
                    ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);

                    foreach (Hashtable item in al)
                    {
                        if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
                        {
                            FileBilancio = item["ID"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public Hashtable GetAllRelazioneBCAssociataFromBilancioFile(string FileBilancio)
        {
            Hashtable RelazioneBC = null;

            Open();

            XmlNode xNode = document.SelectSingleNode("/ROOT/BILANCI/BILANCIO[@FileData='" + FileBilancio.Split('\\').Last() + "']");

            if (xNode != null)
            {
                if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
                {
                    ArrayList al = GetRelazioniBC(xNode.Attributes["Cliente"].Value);

                    foreach (Hashtable item in al)
                    {
                        if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
                        {
                            RelazioneBC = item;
                            break;
                        }
                    }
                }
            }

            Close();

            return RelazioneBC;
        }

        public Hashtable GetAllBilancioAssociatoFromRelazioneBCFile(string FileRelazioneBC)
        {
            Hashtable Bilancio = null;

            Open();

            XmlNode xNode = document.SelectSingleNode("/ROOT/RELAZIONIBC/RELAZIONEBC[@FileData='" + FileRelazioneBC.Split('\\').Last() + "']");

            if (xNode != null)
            {
                if (xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null)
                {
                    ArrayList al = GetBilanci(xNode.Attributes["Cliente"].Value);

                    foreach (Hashtable item in al)
                    {
                        if (item["Data"].ToString() == xNode.Attributes["Data"].Value)
                        {
                            Bilancio = item;
                            break;
                        }
                    }
                }
            }

            Close();

            return Bilancio;
        }
#endregion




#region RelazioneBVV

        public int GetRelazioniBVCount()
        {
            int result = 0;

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/RELAZIONIBV/RELAZIONEBV" );

                if ( xNodes != null )
                {
                    result = xNodes.Count;
                }

                Close();
            }

            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public ArrayList GetRelazioniBV( string IDCliente )
        {
            ArrayList results = new ArrayList();

            try
            {
                Open();

                XmlNodeList xNodes = document.SelectNodes( "/ROOT/RELAZIONIBV/RELAZIONEBV[@Cliente='" + IDCliente + "']" );

                foreach ( XmlNode node in xNodes )
                {
                    Hashtable result = new Hashtable();

                    foreach ( XmlAttribute item in node.Attributes )
                    {
                        result.Add( item.Name, item.Value );
                    }

                    results.Add( result );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return results;
        }

        public Hashtable GetRelazioneBV( string IDRelazioneBV )
        {
            Hashtable result = new Hashtable();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIBV/RELAZIONEBV[@ID='" + IDRelazioneBV + "']" );

                foreach ( XmlAttribute item in xNode.Attributes )
                {
                    result.Add( item.Name, item.Value );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public Hashtable GetRelazioneBVFromFileData( string FileSessione )
        {
            Hashtable result = new Hashtable();

            FileSessione = FileSessione.Split( '\\' ).Last();

            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIBV/RELAZIONEBV[@FileData='" + FileSessione + "']" );

                foreach ( XmlAttribute item in xNode.Attributes )
                {
                    result.Add( item.Name, item.Value );
                }

                Close();
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

            return result;
        }

        public string AddRelazioneBV( XmlNode node )
        {
            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIBV" );

            if ( xNode == null )
            {
                XmlNode xroot = document.SelectSingleNode( "/ROOT" );

                string xml = "<RELAZIONIBV LastID=\"0\" />";
                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml( xml );
                XmlNode tmpNode = doctmp.SelectSingleNode( "/RELAZIONIBV" );
                XmlNode xxtmp = document.ImportNode( tmpNode, true );
                xroot.AppendChild( xxtmp );

                xNode = document.SelectSingleNode( "/ROOT/RELAZIONIBV" );
            }

            if ( xNode.Attributes["LastID"].Value == "" )
            {
                xNode.Attributes["LastID"].Value = "0";
            }

            string ID = ( Convert.ToInt32( xNode.Attributes["LastID"].Value ) + 1 ).ToString();

            node.Attributes["ID"].Value = ID;

            XmlNode xtmp = document.ImportNode( node, true );

            xNode.AppendChild( xtmp );

            xNode.Attributes["LastID"].Value = ID;

            Save();

            Close();

            return ID;
        }

        public void DeleteRelazioneBV( int IDRelazioneBV )
        {
            try
            {
                Open();

                XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIBV/RELAZIONEBV[@ID='" + IDRelazioneBV.ToString() + "']" );

                FileInfo fi = new FileInfo( App.AppDataDataFolder + "\\" + xNode.Attributes["File"].Value );
                if ( fi.Exists )
                {
                    fi.Delete();
                }

                FileInfo fd = new FileInfo( App.AppDataDataFolder + "\\" + xNode.Attributes["FileData"].Value );
                if ( fd.Exists )
                {
                    fd.Delete();
                }

                xNode.ParentNode.RemoveChild( xNode );

                Save();

                //cancello allegati
                XmlManager xdoc = new XmlManager();
                xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
                XmlDocument xdoc_doc = xdoc.LoadEncodedFile( App.AppDocumentiDataFile );

                XmlNodeList xNodes = xdoc_doc.SelectNodes( "//DOCUMENTI/DOCUMENTO[@Sessione='" + IDRelazioneBV + "'][@Tree='" + ( Convert.ToInt32( App.TipoFile.RelazioneBV ) ).ToString() + "']" );

                foreach ( XmlNode node in xNodes )
                {
                    FileInfo fis = new FileInfo( App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value );
                    if ( fis.Exists )
                    {
                        fis.Delete();
                    }

                    node.ParentNode.RemoveChild( node );
                }

                xdoc.SaveEncodedFile( App.AppDocumentiDataFile, xdoc_doc.InnerXml );

                Close();

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster );
            }
        }

        public bool CheckDoppio_RelazioneBV( int ID, int IDCliente, string Data )
        {
            Open();

            XmlNodeList xNodes = document.SelectNodes( "/ROOT/RELAZIONIBV/RELAZIONEBV" );
            foreach ( XmlNode node in xNodes )
            {
                if ( node.Attributes["ID"].Value != ID.ToString() && node.Attributes["Cliente"].Value == IDCliente.ToString() && node.Attributes["Data"].Value == Data )
                {
                    Close();
                    return false;
                }
            }

            Close();
            return true;
        }
        
        public int SetRelazioneBVIntermedio(Hashtable values, int IDCliente, string dal, string al)
        {
            int IDRelazioneBV = -1;

            try
            {
                Open();

                    XmlNode root = document.SelectSingleNode("/ROOT/RELAZIONIBV");

                    if (root == null)
                    {
                        XmlNode xroot = document.SelectSingleNode("/ROOT");

                        string xmla = "<RELAZIONIBV LastID=\"0\" />";
                        XmlDocument doctmpa = new XmlDocument();
                        doctmpa.LoadXml(xmla);
                        XmlNode tmpNodea = doctmpa.SelectSingleNode("/RELAZIONIBV");
                        XmlNode xxtmp = document.ImportNode(tmpNodea, true);
                        xroot.AppendChild(xxtmp);

                        root = document.SelectSingleNode("/ROOT/RELAZIONIBV");
                    }

                    IDRelazioneBV = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);

                    string lastindex = IDRelazioneBV.ToString();

                    //Template TREE
                    FileInfo fitree = new FileInfo(App.AppTemplateTreeRelazioneBV);
                    string estensione = "." + App.AppTemplateTreeRelazioneBV.Split('.').Last();
                    string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewtree = new FileInfo(newNametree);

                    while (fnewtree.Exists)
                    {
                        newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewtree = new FileInfo(newNametree);
                    }

                    fitree.CopyTo(newNametree);
                    newNametree = newNametree.Replace(App.AppDataDataFolder + "\\", "");

                    //Template Dati
                    FileInfo fidati = new FileInfo(App.AppTemplateDataRelazioneBV);
                    string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewdati = new FileInfo(newNamedati);

                    while (fnewdati.Exists)
                    {
                        newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewdati = new FileInfo(newNamedati);
                    }

                    fidati.CopyTo(newNamedati);
                    newNamedati = newNamedati.Replace(App.AppDataDataFolder + "\\", "");

                    XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

                    string xml = "<RELAZIONEBV ID=\"" + lastindex + "\" Intermedio=\"True\" Cliente=\"" + IDCliente + "\" Stato=\"" + ((int)(App.TipoSessioneStato.Disponibile)).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + dal + "\" EsercizioAl=\"" + al + "\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml(xml);

                    XmlNode tmpNode = doctmp.SelectSingleNode("/RELAZIONEBV");
                    XmlNode cliente = document.ImportNode(tmpNode, true);

                    root.AppendChild(cliente);

                    root.Attributes["LastID"].Value = lastindex;


                Save();

                Close();

            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
            }

            return IDRelazioneBV;
        }

        public int SetRelazioneBV( Hashtable values, int IDRelazioneBV, int IDCliente )
        {
            try
            {
                Open();

                if ( IDRelazioneBV == App.MasterFile_NewID )
                {
                    XmlNode root = document.SelectSingleNode( "/ROOT/RELAZIONIBV" );

                    if ( root == null )
                    {
                        XmlNode xroot = document.SelectSingleNode( "/ROOT" );

                        string xmla = "<RELAZIONIBV LastID=\"0\" />";
                        XmlDocument doctmpa = new XmlDocument();
                        doctmpa.LoadXml( xmla );
                        XmlNode tmpNodea = doctmpa.SelectSingleNode( "/RELAZIONIBV" );
                        XmlNode xxtmp = document.ImportNode( tmpNodea, true );
                        xroot.AppendChild( xxtmp );

                        root = document.SelectSingleNode( "/ROOT/RELAZIONIBV" );
                    }

                    IDRelazioneBV = ( Convert.ToInt32( root.Attributes["LastID"].Value ) + 1 );

                    string lastindex = IDRelazioneBV.ToString();

                    //Template TREE
                    FileInfo fitree = new FileInfo( App.AppTemplateTreeRelazioneBV );
                    string estensione = "." + App.AppTemplateTreeRelazioneBV.Split( '.' ).Last();
                    string newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewtree = new FileInfo( newNametree );

                    while ( fnewtree.Exists )
                    {
                        newNametree = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewtree = new FileInfo( newNametree );
                    }

                    fitree.CopyTo( newNametree );
                    newNametree = newNametree.Replace( App.AppDataDataFolder + "\\", "" );

                    //Template Dati
                    FileInfo fidati = new FileInfo( App.AppTemplateDataRelazioneBV );
                    string newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                    FileInfo fnewdati = new FileInfo( newNamedati );

                    while ( fnewdati.Exists )
                    {
                        newNamedati = App.AppDataDataFolder + "\\" + Guid.NewGuid().ToString() + estensione;
                        fnewdati = new FileInfo( newNamedati );
                    }

                    fidati.CopyTo( newNamedati );
                    newNamedati = newNamedati.Replace( App.AppDataDataFolder + "\\", "" );

                    XmlNode XmlNodeCliente = document.SelectSingleNode("/ROOT/CLIENTI/CLIENTE[@ID= " + IDCliente + "]");

                    string xml = "<RELAZIONEBV ID=\"" + lastindex + "\" Cliente=\"" + IDCliente + "\" Stato=\"" + ( (int)( App.TipoSessioneStato.Disponibile ) ).ToString() + "\" File=\"" + newNametree + "\" FileData=\"" + newNamedati + "\" Note=\"" + values["Note"].ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" ) + "\" Data=\"" + values["Data"].ToString() + "\"  Esercizio=\"" + XmlNodeCliente.Attributes["Esercizio"].Value + "\" EsercizioDal=\"" + XmlNodeCliente.Attributes["EsercizioDal"].Value + "\" EsercizioAl=\"" + XmlNodeCliente.Attributes["EsercizioAl"].Value + "\" />";
                    XmlDocument doctmp = new XmlDocument();
                    doctmp.LoadXml( xml );

                    XmlNode tmpNode = doctmp.SelectSingleNode( "/RELAZIONEBV" );
                    XmlNode cliente = document.ImportNode( tmpNode, true );

                    root.AppendChild( cliente );

                    root.Attributes["LastID"].Value = lastindex;
                }
                else
                {
                    XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIBV/RELAZIONEBV[@ID='" + IDRelazioneBV.ToString() + "']" );

                    xNode.Attributes["Note"].Value = values["Note"].ToString();
                    xNode.Attributes["Data"].Value = values["Data"].ToString();
                }

                Save();

                Close();

            }
            catch ( Exception ex )
            {
                string log = ex.Message;

                Error( WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster );
            }

            return IDRelazioneBV;
        }

        public string GetRevisioneAssociataFromRelazioneBVFile( string FileRelazioneV )
        {
            string FileRevisione = "";

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIBV/RELAZIONEBV[@FileData='" + FileRelazioneV.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetRevisioni( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            FileRevisione = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileRevisione;
        }

        public string GetBilancioAssociatoFromRelazioneBVFile( string FileRelazioneBV )
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIBV/RELAZIONEBV[@FileData='" + FileRelazioneBV.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetBilanci( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            FileBilancio = App.AppDataDataFolder + "\\" + item["FileData"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public string GetBilancioTreeAssociatoFromRelazioneBVFile( string FileRelazioneBV )
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIBV/RELAZIONEBV[@FileData='" + FileRelazioneBV.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetBilanci( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            FileBilancio = App.AppDataDataFolder + "\\" + item["File"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public string GetBilancioIDAssociatoFromRelazioneBVFile( string FileRelazioneBV )
        {
            string FileBilancio = "";

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIBV/RELAZIONEBV[@FileData='" + FileRelazioneBV.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetBilanci( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            FileBilancio = item["ID"].ToString();
                            break;
                        }
                    }
                }
            }

            Close();

            return FileBilancio;
        }

        public Hashtable GetAllRelazioneBVAssociataFromBilancioFile( string FileBilancio )
        {
            Hashtable RelazioneBV = null;

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/BILANCI/BILANCIO[@FileData='" + FileBilancio.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetRelazioniBV( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            RelazioneBV = item;
                            break;
                        }
                    }
                }
            }

            Close();

            return RelazioneBV;
        }

        public Hashtable GetAllBilancioAssociatoFromRelazioneBVFile( string FileRelazioneBV )
        {
            Hashtable Bilancio = null;

            Open();

            XmlNode xNode = document.SelectSingleNode( "/ROOT/RELAZIONIBV/RELAZIONEBV[@FileData='" + FileRelazioneBV.Split( '\\' ).Last() + "']" );

            if ( xNode != null )
            {
                if ( xNode.Attributes["Cliente"] != null && xNode.Attributes["Data"] != null )
                {
                    ArrayList al = GetBilanci( xNode.Attributes["Cliente"].Value );

                    foreach ( Hashtable item in al )
                    {
                        if ( item["Data"].ToString() == xNode.Attributes["Data"].Value )
                        {
                            Bilancio = item;
                            break;
                        }
                    }
                }
            }

            Close();

            return Bilancio;
        }
#endregion

#region DOCUMENTI
        public void CheckAndNormalizeDocuments()
        { 
            //cancello allegati
            XmlManager xdoc = new XmlManager();
            xdoc.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
            XmlDocument xdoc_doc = xdoc.LoadEncodedFile(App.AppDocumentiDataFile);

            XmlNodeList xNodes = xdoc_doc.SelectNodes("//DOCUMENTO");

            foreach ( XmlNode node in xNodes )
            {
                FileInfo fis = new FileInfo(App.AppDocumentiFolder + "\\" + node.Attributes["File"].Value);
                if (!fis.Exists)
                {
                    node.ParentNode.RemoveChild(node);
                    continue;
                }

                if (node.Attributes["ClienteExtended"] == null && node.Attributes["Cliente"] != null)
                {
                    XmlAttribute attr = node.OwnerDocument.CreateAttribute("ClienteExtended");

                    try
                    {
                        attr.Value = wDocumenti.GetClienteString(node.Attributes["Cliente"].Value);
                    }
                    catch (Exception ex)
                    {
                        attr.Value = "";
                        string log = ex.Message;
                    }

                    node.Attributes.Append(attr);
                }

                if (node.Attributes["SessioneExtended"] == null && node.Attributes["Sessione"] != null)
                {
                    XmlAttribute attr = node.OwnerDocument.CreateAttribute("SessioneExtended");
                    
                    try
                    {
                        attr.Value = wDocumenti.GetSessioneString(node.Attributes["Tree"].Value, node.Attributes["Sessione"].Value);
                    }
                    catch (Exception ex)
                    {
                        attr.Value = "";
                        string log = ex.Message;
                    }

                    node.Attributes.Append(attr);
                }

                if (node.Attributes["NodoExtended"] == null && node.Attributes["Tree"] != null && node.Attributes["Sessione"] != null && node.Attributes["Nodo"] != null)
                {
                    XmlAttribute attr = node.OwnerDocument.CreateAttribute("NodoExtended");

                    try
                    {
                        attr.Value = wDocumenti.GetNodeString(node.Attributes["Tree"].Value, node.Attributes["Sessione"].Value, node.Attributes["Nodo"].Value);
                    }
                    catch (Exception ex)
                    {
                        attr.Value = "";
                        string log = ex.Message;
                    }
                    
                    node.Attributes.Append(attr);
                }
            }

            xdoc.SaveEncodedFile( App.AppDocumentiDataFile, xdoc_doc.InnerXml );

            DirectoryInfo dis_lost = new DirectoryInfo(App.AppDocumentiFolder + "\\Lost");
            if (!dis_lost.Exists)
            {
                dis_lost.Create();
            }

            DirectoryInfo dis = new DirectoryInfo(App.AppDocumentiFolder);
            foreach (FileInfo item in dis.GetFiles())
            {
                XmlNodeList xNodeshere = xdoc_doc.SelectNodes("//DOCUMENTO[@File=\"" + item.Name + "\"]");
                if (xNodeshere.Count == 0)
                {
                    item.MoveTo(App.AppDocumentiFolder + "\\Lost\\" + item.Name);
                }
            }
        }
#endregion
    }
}
*/
