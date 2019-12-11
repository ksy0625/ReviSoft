using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using static RevisoftApplication.XmlManager;

namespace RevisoftApplication
{
  public class XmlManager
  {
    //------------------------------------------------------------------- CRIPT
    private const string REVISOFT_CRIPT_OLD_KEY = "ciao";
    private const string REVISOFT_CRIPT_NORMAL_KEY = "datalaboristhebest";
    private const string REVISOFT_CRIPT_STRONG_KEY = "ambrarabacicicocotrecivettesulcomechefacevanolamoreconlafigliadeldottore";
    //--------------------------------------------------------------- variabili
    private TipologiaCodifica _TipoCodifica;
    //-------------------------------------------------------------------- enum
    public enum TipologiaCodifica { Nessuna = 0, Normale = 1, Avanzata = 2, Old = 9 };

    //--------------------------------------------------------------- proprietà
    public TipologiaCodifica TipoCodifica
    {
      get { return (TipologiaCodifica)_TipoCodifica; }
      set { _TipoCodifica = value; }
    }

    //------------------------------------------------------------------ metodi
    public XmlManager()
    {
      _TipoCodifica = TipologiaCodifica.Normale;
    }

    #region CODIFICA_DECODIFICA

    //----------------------------------------------------------------------------+
    //                                 EncodeFile                                 |
    //----------------------------------------------------------------------------+
    private void EncodeFile(string Path, string Testo)
    {
      //---------------------------------------------------- tipologia codifica
      string Pwd = "";
      switch (_TipoCodifica)
      {
        case TipologiaCodifica.Old:
          Pwd = REVISOFT_CRIPT_OLD_KEY;
          break;
        case TipologiaCodifica.Normale:
          Pwd = REVISOFT_CRIPT_NORMAL_KEY;
          break;
        case TipologiaCodifica.Avanzata:
          Pwd = REVISOFT_CRIPT_STRONG_KEY;
          break;
      }
      string temporaryfileforcheck =
        //Path.Replace('.' + Path.Split('.').Last(), '.' + "tmp");
        Path + ".tmp"; // E.B. nome file potrebbe non avere estensione
      System.IO.File.WriteAllText(temporaryfileforcheck, Testo);
      byte[] keyBytes;
      keyBytes = Encoding.Unicode.GetBytes(Pwd);
      Rfc2898DeriveBytes derivedKey = new Rfc2898DeriveBytes(Pwd, keyBytes);
      RijndaelManaged rijndaelCSP = new RijndaelManaged();
      rijndaelCSP.Key = derivedKey.GetBytes(rijndaelCSP.KeySize / 8);
      rijndaelCSP.IV = derivedKey.GetBytes(rijndaelCSP.BlockSize / 8);
      ICryptoTransform encryptor = rijndaelCSP.CreateEncryptor();
      byte[] encrypted = Encoding.Unicode.GetBytes(Testo);
      FileStream outputFileStream =
        new FileStream(Path, FileMode.Create, FileAccess.Write);
      CryptoStream encryptStream =
        new CryptoStream(outputFileStream, encryptor, CryptoStreamMode.Write);
      encryptStream.Write(encrypted, 0, encrypted.Length);
      encryptStream.FlushFinalBlock();
      rijndaelCSP.Clear();
      encryptStream.Close();
      outputFileStream.Close();
      //---------------------------------------------------- test di decodifica
      string testTesto = DecodeFile(Path);
      if (testTesto.Replace("\0", "") == Testo.Replace("\0", ""))
      {
        FileInfo fitmp = new FileInfo(temporaryfileforcheck);
        fitmp.Delete();
      }
      else
      {
        throw new Exception();
      }
    }

    //----------------------------------------------------------------------------+
    //                                 DecodeFile                                 |
    //----------------------------------------------------------------------------+
    private string DecodeFile(string Path)
    {
            bool ok=true;
            string returnvalue="";
      //---------------------------------------------------- tipologia codifica
      string Pwd = "";
      switch (_TipoCodifica)
      {
        case TipologiaCodifica.Old:
          Pwd = REVISOFT_CRIPT_OLD_KEY;
          break;
        case TipologiaCodifica.Normale:
          Pwd = REVISOFT_CRIPT_NORMAL_KEY;
          break;
        case TipologiaCodifica.Avanzata:
          Pwd = REVISOFT_CRIPT_STRONG_KEY;
          break;
      }
      byte[] keyBytes = Encoding.Unicode.GetBytes(Pwd);
      Rfc2898DeriveBytes derivedKey = new Rfc2898DeriveBytes(Pwd, keyBytes);
      RijndaelManaged rijndaelCSP = new RijndaelManaged();
      rijndaelCSP.Key = derivedKey.GetBytes(rijndaelCSP.KeySize / 8);
      rijndaelCSP.IV = derivedKey.GetBytes(rijndaelCSP.BlockSize / 8);
      ICryptoTransform decryptor = rijndaelCSP.CreateDecryptor();
      FileStream inputFileStream =
        new FileStream(Path, FileMode.Open, FileAccess.Read);
      if (inputFileStream == null) return null;
      using (CryptoStream decryptStream= new CryptoStream(inputFileStream, decryptor, CryptoStreamMode.Read))
            {
      //CryptoStream decryptStream =
      //  new CryptoStream(inputFileStream, decryptor, CryptoStreamMode.Read);
                byte[] inputFileData = new byte[(int)inputFileStream.Length];
            try
            {
                decryptStream.Read(inputFileData, 0, (int)inputFileStream.Length);
            }
            catch (Exception)
                {
                    ok = false;
                decryptStream.Dispose();
              
            }
                if (ok)
            {
                returnvalue = Encoding.Unicode.GetString(inputFileData);
                decryptStream.Close();
            }
         }
            rijndaelCSP.Clear();
      inputFileStream.Close();
            if (!ok) File.Delete(Path);
            return returnvalue;
    }

    //----------------------------------------------------------------------------+
    //                              LoadEncodedFile                               |
    //----------------------------------------------------------------------------+
    public XmlDocument LoadEncodedFile_old(string xFile)
    {
      XmlDocument d = new XmlDocument();
      if (_TipoCodifica == TipologiaCodifica.Nessuna)
      {
        //------------------------------------------------- dati non codificati
        d.Load(xFile);
      }
      else
      {
        //----------------------------------------------------- dati codificati
        string Testo = DecodeFile(xFile);
        XmlDataProvider p = new XmlDataProvider();
        //p.Document = new System.Xml.XmlDocument();
        //p.Document.LoadXml(Testo);
        //p.Refresh();
        try
        {
          d.LoadXml(Testo);
        }
        catch (Exception ex)
        {
          string log = ex.Message;
          Testo = Testo.ToString().Replace("&", "&amp;").Replace("\"", "'");
          d.LoadXml(Testo);
        }
      }
      return d;
    }
    public XmlDocument LoadEncodedFile(string xFile)
    {
            return cBusinessObjects.NewLoadEncodedFile(xFile);

                    try{
                   XmlDocument d=null;
                      string item = xFile.Split('\\').Last();
                      if (App.m_bxmlCacheEnable)
                      {
                        if (App.m_xmlCache.ContainsKey(item))
                         d = ((XMLELEMENT) App.m_xmlCache[item]).doc;
                        else
                        {
                          CheckXmlCache();
                          d = StaticUtilities.BuildXML(item);
                          if (d!=null) App.m_xmlCache.Add(item, new XMLELEMENT(d,false));
                        }
                      }
                      else
                      {
                           d = StaticUtilities.BuildXML(item);
                      }
               
                      if (d == null) d = LoadEncodedFile_old(xFile);
                      return d;
                    }
                    catch(Exception aa)
                    {

                    }
                            return null;

                    }
                    // E.B. - nuovo metodo
                    public void CheckXmlCache()
                    {
                      int i,count,deleted;
                      count = App.m_xmlCache.Count;
                      if (count < App.m_cacheMax) return;
                      // se la cache e' piena, prova a liberare spazi eliminando gli elementi non modificati
                      string[] keys = new string[count];
                      App.m_xmlCache.Keys.CopyTo(keys, 0);
                      deleted = 0;count = keys.Length;
                      for (i = 0; i < count && deleted<count/5; i++)
                      {
                        if (!((XMLELEMENT)(App.m_xmlCache[keys[i]])).isModified)
                        {
                          App.m_xmlCache.Remove(keys[i]);deleted++;
                        }
                      }
    }
    //----------------------------------------------------------------------------+
    //                          EncodedFileToDecodedFile                          |
    //----------------------------------------------------------------------------+
    public void EncodedFileToDecodedFile(string eFile, string dFile)
    {
      if (_TipoCodifica == TipologiaCodifica.Nessuna)
      {
        //------------------------------------------------- dati non codificati
        FileInfo fi = new FileInfo(eFile);
        fi.CopyTo(dFile);
      }
      else
      {
        //----------------------------------------------------- dati codificati
        string Testo = DecodeFile(eFile);
        StreamWriter sw = new StreamWriter(dFile);
        sw.Write(Testo);
        sw.Close();
      }
    }

    //----------------------------------------------------------------------------+
    //                              SaveEncodedFile                               |
    //----------------------------------------------------------------------------+
    public void SaveEncodedFile_old(string xFile, string xData)
    {
      if (_TipoCodifica == TipologiaCodifica.Nessuna)
      {
        //------------------------------------------------- dati non codificati
        XmlDocument d = new XmlDocument();
        try
        {
          d.LoadXml(xData);
        }
        catch (Exception ex)
        {
          string log = ex.Message;
          xData = xData.ToString().Replace("&", "&amp;").Replace("\"", "'");
          d.LoadXml(xData);
        }
        d.Save(xFile);
      }
      else
      {
        //----------------------------------------------------- dati codificati
        EncodeFile(xFile, xData);
      }
    }
    public void SaveEncodedFile(string xFile, string xData,bool isMod=false,bool saveNow=false)
    {
            // if (!App.m_bxmlCacheEnable || xFile== App.AppLicenseFile
            //   || xFile ==App.AppLicenseFile_OLD || xFile== App.AppInfoFile) {
            //  SaveEncodedFile_old(xFile, xData); return;
            //   }
      string item = xFile.Split('\\').Last();
      XmlDocument d = new XmlDocument();
      XMLELEMENT el;
      try { d.LoadXml(xData); }
      catch (Exception ex)
      {
        string log = ex.Message;
        xData = xData.ToString().Replace("&", "&amp;").Replace("\"", "'");
        d.LoadXml(xData);
      }
      if (App.m_xmlCache.ContainsKey(item))
      {
        el = (XMLELEMENT)(App.m_xmlCache[item]);
        el.doc = d; el.isModified = isMod; App.m_xmlCache[item] = el;
      }
      else { CheckXmlCache(); App.m_xmlCache.Add(item, new XMLELEMENT(d,isMod)); }
      //if (!isMod || !saveNow) return;
      if (!saveNow) return;
      // file xml in d, chiave in item
      // [1] creare albero ridotto con i soli nodi modificati
      XmlDocument doc = new XmlDocument();
      doc = StaticUtilities.ExtractMasterFileMod(d);
      if (doc == null) doc = StaticUtilities.ExtractTreeMod(d);
      if (doc == null) doc = StaticUtilities.ExtractDatiMod(d);
      if (doc == null) return; // just to be sure
      // [2] scrivere dati in dbo.xmlSaveTest
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        string query, str;
        str = doc.OuterXml.Replace("'", "''");
        query = string.Format(
          "insert into xmlSaveTest (guid,data)\n" + "values ('{0}','{1}')", item, str);
        conn.Open();
        SqlCommand cmd = new SqlCommand(query, conn);
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "SaveEncodedFile(): errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      // [3] invocare dbo.SaveSingleModified passando guid=item
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("dbo.SaveSingleModified", conn);
        cmd.Parameters.AddWithValue("@guid", item);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          if (!App.m_bNoExceptionMsg)
          {
            string msg = "dbo.SaveSingleModified: errore\n" + ex.Message;
            MessageBox.Show(msg);
          }
        }
      }
      // [4] eliminare flag di modifica dalla cache
      //     SQL elimina XML dalla sua cache
      XmlAttributeCollection attrColl = d.DocumentElement.Attributes;
      attrColl.RemoveNamedItem("nodeModified");
      attrColl.RemoveNamedItem("idFather");
      el = (XMLELEMENT)(App.m_xmlCache[item]);
      el.doc = d; el.isModified = false; App.m_xmlCache[item] = el;
    }

    //----------------------------------------------------------------------------+
    //                                EncodeString                                |
    //----------------------------------------------------------------------------+
    public string EncodeString(string testo)
    {
      //---------------------------------------------------- tipologia codifica
      string Pwd = "";
      switch (_TipoCodifica)
      {
        case TipologiaCodifica.Old:
          Pwd = REVISOFT_CRIPT_OLD_KEY;
          break;
        case TipologiaCodifica.Normale:
          Pwd = REVISOFT_CRIPT_NORMAL_KEY;
          break;
        case TipologiaCodifica.Avanzata:
          Pwd = REVISOFT_CRIPT_STRONG_KEY;
          break;
      }
      byte[] keyBytes;
      keyBytes = Encoding.Unicode.GetBytes(Pwd);
      Rfc2898DeriveBytes derivedKey = new Rfc2898DeriveBytes(Pwd, keyBytes);
      RijndaelManaged rijndaelCSP = new RijndaelManaged();
      rijndaelCSP.Key = derivedKey.GetBytes(rijndaelCSP.KeySize / 8);
      rijndaelCSP.IV = derivedKey.GetBytes(rijndaelCSP.BlockSize / 8);
      ICryptoTransform encryptor = rijndaelCSP.CreateEncryptor();
      MemoryStream memoryStream = new MemoryStream();
      CryptoStream cryptoStream =
        new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
      StreamWriter writer = new StreamWriter(cryptoStream);
      writer.Write(testo);
      writer.Flush();
      cryptoStream.FlushFinalBlock();
      writer.Flush();
      return Convert.ToBase64String(
        memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
    }

    //----------------------------------------------------------------------------+
    //                                DecodeString                                |
    //----------------------------------------------------------------------------+
    public string DecodeString(string testo)
    {
      //---------------------------------------------------- tipologia codifica
      string Pwd = "";
      switch (_TipoCodifica)
      {
        case TipologiaCodifica.Old:
          Pwd = REVISOFT_CRIPT_OLD_KEY;
          break;
        case TipologiaCodifica.Normale:
          Pwd = REVISOFT_CRIPT_NORMAL_KEY;
          break;
        case TipologiaCodifica.Avanzata:
          Pwd = REVISOFT_CRIPT_STRONG_KEY;
          break;
      }
      byte[] keyBytes = Encoding.Unicode.GetBytes(Pwd);
      Rfc2898DeriveBytes derivedKey = new Rfc2898DeriveBytes(Pwd, keyBytes);
      RijndaelManaged rijndaelCSP = new RijndaelManaged();
      rijndaelCSP.Key = derivedKey.GetBytes(rijndaelCSP.KeySize / 8);
      rijndaelCSP.IV = derivedKey.GetBytes(rijndaelCSP.BlockSize / 8);
      ICryptoTransform decryptor = rijndaelCSP.CreateDecryptor();
      MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(testo));
      CryptoStream cryptoStream =
        new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
      StreamReader reader = new StreamReader(cryptoStream);
      rijndaelCSP.Clear();
      return reader.ReadToEnd();
    }

    #endregion

  } // public class XmlManager

  public class XmlDataProviderManager
  {
    private XmlDataProvider XDP;
    private string nameFile;
    private string idt;
    private XmlManager x = new XmlManager();
    //private bool encoded = true;
    public bool isModified = false; // E.B. nuovo campo

    //----------------------------------------------------------------------------+
    //                           XmlDataProviderManager                           |
    //                          construttore originale:                           |
    //                 public XmlDataProviderManager(string file)                 |
    //            il nuovo costruttore permette di non caricare subito            |
    //                  il file - basta richiamare con loadNow=0                  |
    //----------------------------------------------------------------------------+
    public XmlDataProviderManager(string file,string IDtree="")
    {
       idt = IDtree;
      XDP = new XmlDataProvider();
      x.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
      File = file;
    }

    //----------------------------------------------------------------------------+
    //                                SetDocument                                 |
    //                             E.B. nuovo metodo                              |
    //            imposta il documento attuale senza accedere a disco             |
    //----------------------------------------------------------------------------+
    public void SetDocument(XmlDocument doc = null)
    {
      if (doc == null) return;
      XDP.Document = doc;
      Refresh();
    }

    //----------------------------------------------------------------------------+
    //                           XmlDataProviderManager                           |
    //----------------------------------------------------------------------------+
    public XmlDataProviderManager(string file, bool encoded)
    {
      XDP = new XmlDataProvider();
      if (encoded)
      {
        x.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
      }
      else
      {
        x.TipoCodifica = XmlManager.TipologiaCodifica.Nessuna;
      }
      File = file;
    }

    //----------------------------------------------------------------------------+
    //                                   Clone                                    |
    //----------------------------------------------------------------------------+
    public XmlDataProviderManager Clone()
    {
      XmlDataProviderManager newxdpm = new XmlDataProviderManager(nameFile);
      return newxdpm;
    }

    //----------------------------------------------------------------------------+
    //                               proprietà xdp                                |
    //----------------------------------------------------------------------------+
    public XmlDataProvider xdp
    {
      get { return XDP; }
    }

    //----------------------------------------------------------------------------+
    //                             proprietà Document                             |
    //----------------------------------------------------------------------------+
    public XmlDocument Document
    {
      get { return XDP.Document; }
    }

    //----------------------------------------------------------------------------+
    //                               proprietà File                               |
    //----------------------------------------------------------------------------+
    public string File
    {
      get { return nameFile; }
      set
      {
        nameFile = value;
        Load();
      }
    }

    //----------------------------------------------------------------------------+
    //                                    Save                                    |
    //----------------------------------------------------------------------------+
    public void Save2()
    {
      if (XDP.Document != null)
      {
        x.TipoCodifica = TipologiaCodifica.Nessuna;
        x.SaveEncodedFile_old(nameFile, XDP.Document.OuterXml);
    
      }
    }       
    public void Save_old()
    {
      if (XDP.Document != null)
      {
        x.TipoCodifica = TipologiaCodifica.Nessuna;
        x.SaveEncodedFile_old(nameFile, XDP.Document.OuterXml);
        Load();
      }
    }
    public void Save(bool saveNow=false)
    {
      
      if (XDP.Document != null)
      {
        x.SaveEncodedFile(nameFile, XDP.Document.OuterXml,isModified,saveNow);
      }
      isModified = false;
    }

    //----------------------------------------------------------------------------+
    //                                    Load                                    |
    //----------------------------------------------------------------------------+
    public void Load()
    {
   
      XDP.Document =cBusinessObjects.NewLoadEncodedFile(nameFile,idt);

      Refresh();
    }

    //----------------------------------------------------------------------------+
    //                                  Refresh                                   |
    //----------------------------------------------------------------------------+
    public void Refresh()
    {
      XDP.Refresh();
    }
  } // public class XmlDataProviderManager
} // namespace RevisoftApplication

/*
// srcOld
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Linq;

namespace RevisoftApplication
{
    public class XmlManager
    {
        //CRIPT
        private const string                REVISOFT_CRIPT_OLD_KEY              = "ciao";
        private const string                REVISOFT_CRIPT_NORMAL_KEY           = "datalaboristhebest";
        private const string                REVISOFT_CRIPT_STRONG_KEY           = "ambrarabacicicocotrecivettesulcomechefacevanolamoreconlafigliadeldottore";

        //Variabili
        private TipologiaCodifica           _TipoCodifica;

        //Enum
        public enum TipologiaCodifica        { Nessuna=0, Normale=1, Avanzata=2, Old=9 };
        
        //Proprietà
        public TipologiaCodifica TipoCodifica
        {
            get { return (TipologiaCodifica)_TipoCodifica; }
            set { _TipoCodifica = value; }
        }
        
        //Metodi
        public XmlManager()
        {
            _TipoCodifica = TipologiaCodifica.Normale;
        }
        

#region CODIFICA_DECODIFICA
        private void EncodeFile(string Path, string Testo)
        {
            //tipologia codifica
            string Pwd = "";
            switch (_TipoCodifica)
            {
                case TipologiaCodifica.Old:
                    Pwd = REVISOFT_CRIPT_OLD_KEY;
                    break;
                case TipologiaCodifica.Normale:
                    Pwd = REVISOFT_CRIPT_NORMAL_KEY;
                    break;
                case TipologiaCodifica.Avanzata:
                    Pwd = REVISOFT_CRIPT_STRONG_KEY;
                    break;
            }

            string temporaryfileforcheck = Path.Replace('.' + Path.Split('.').Last(), '.' + "tmp");

            System.IO.File.WriteAllText(temporaryfileforcheck, Testo);

            byte[] keyBytes;
            keyBytes = Encoding.Unicode.GetBytes(Pwd);

            Rfc2898DeriveBytes derivedKey = new Rfc2898DeriveBytes(Pwd, keyBytes);

            RijndaelManaged rijndaelCSP = new RijndaelManaged();
            rijndaelCSP.Key = derivedKey.GetBytes(rijndaelCSP.KeySize / 8);
            rijndaelCSP.IV = derivedKey.GetBytes(rijndaelCSP.BlockSize / 8);

            ICryptoTransform encryptor = rijndaelCSP.CreateEncryptor();

            byte[] encrypted = Encoding.Unicode.GetBytes(Testo);

            FileStream outputFileStream = new FileStream(Path, FileMode.Create, FileAccess.Write);

            CryptoStream encryptStream = new CryptoStream(outputFileStream, encryptor, CryptoStreamMode.Write);
            encryptStream.Write(encrypted, 0, encrypted.Length);
            encryptStream.FlushFinalBlock();

            rijndaelCSP.Clear();
            encryptStream.Close();
            outputFileStream.Close();

            //test di decodifica
            string testTesto = DecodeFile(Path);
            if (testTesto.Replace("\0", "") == Testo.Replace("\0", ""))
            {
                FileInfo fitmp = new FileInfo(temporaryfileforcheck);
                fitmp.Delete();
            }
            else
            {
                throw new Exception();
            }
        }

        private string DecodeFile(string Path)
        {
            //tipologia codifica
            string Pwd = "";
            switch (_TipoCodifica)
            {
                case TipologiaCodifica.Old:
                    Pwd = REVISOFT_CRIPT_OLD_KEY;
                    break;
                case TipologiaCodifica.Normale:
                    Pwd = REVISOFT_CRIPT_NORMAL_KEY;
                    break;
                case TipologiaCodifica.Avanzata:
                    Pwd = REVISOFT_CRIPT_STRONG_KEY;
                    break;
            } 
            
            byte[] keyBytes = Encoding.Unicode.GetBytes(Pwd);

            Rfc2898DeriveBytes derivedKey = new Rfc2898DeriveBytes(Pwd, keyBytes);

            RijndaelManaged rijndaelCSP = new RijndaelManaged();
            rijndaelCSP.Key = derivedKey.GetBytes(rijndaelCSP.KeySize / 8);
            rijndaelCSP.IV = derivedKey.GetBytes(rijndaelCSP.BlockSize / 8);
            ICryptoTransform decryptor = rijndaelCSP.CreateDecryptor();

            FileStream inputFileStream = new FileStream(Path, FileMode.Open, FileAccess.Read);

            if(inputFileStream == null)
            {
                return null;
            }

            CryptoStream decryptStream = new CryptoStream(inputFileStream, decryptor, CryptoStreamMode.Read);

            byte[] inputFileData = new byte[(int)inputFileStream.Length];
            decryptStream.Read(inputFileData, 0, (int)inputFileStream.Length);

            string returnvalue = Encoding.Unicode.GetString(inputFileData);

            rijndaelCSP.Clear();

            decryptStream.Close();
            inputFileStream.Close();

            return returnvalue;
        }

		public XmlDocument LoadEncodedFile(string xFile)
		{
			XmlDocument d = new XmlDocument();

			if (_TipoCodifica == TipologiaCodifica.Nessuna)
			{
				//dati non codificati
				d.Load(xFile);
			}
			else
			{
				//dati codificati
				string Testo = DecodeFile(xFile);
				XmlDataProvider p = new XmlDataProvider();
				//p.Document = new System.Xml.XmlDocument();
				//p.Document.LoadXml(Testo);
				//p.Refresh();
				try
				{
					d.LoadXml(Testo);
				}
				catch (Exception ex)
				{
					string log = ex.Message;

					Testo = Testo.ToString().Replace("&", "&amp;").Replace("\"", "'");
					d.LoadXml(Testo);
				}


			}
			return d;
		}

        public void EncodedFileToDecodedFile(string eFile, string dFile)
        {
            if (_TipoCodifica == TipologiaCodifica.Nessuna)
            {
                //dati non codificati
                FileInfo fi = new FileInfo(eFile);
                fi.CopyTo(dFile);
            }
            else
            {
                //dati codificati
                string Testo = DecodeFile(eFile);
                StreamWriter sw = new StreamWriter(dFile);
                sw.Write(Testo);
                sw.Close();
            }
        }



        public void SaveEncodedFile(string xFile, string xData)
        {
            if (_TipoCodifica == TipologiaCodifica.Nessuna)
            {
                //dati non codificati
                XmlDocument d = new XmlDocument();

                try
                {
                    d.LoadXml( xData );
                }
                catch ( Exception ex )
                {
                    string log = ex.Message;

                    xData = xData.ToString().Replace( "&", "&amp;" ).Replace( "\"", "'" );
                    d.LoadXml( xData );
                }

                d.Save(xFile);
            }
            else
            {
                //dati codificati
                EncodeFile(xFile, xData);
            }
        }


        public string EncodeString(string testo)
        {
            //tipologia codifica
            string Pwd = "";
            switch (_TipoCodifica)
            {
                case TipologiaCodifica.Old:
                    Pwd = REVISOFT_CRIPT_OLD_KEY;
                    break;
                case TipologiaCodifica.Normale:
                    Pwd = REVISOFT_CRIPT_NORMAL_KEY;
                    break;
                case TipologiaCodifica.Avanzata:
                    Pwd = REVISOFT_CRIPT_STRONG_KEY;
                    break;
            }

            byte[] keyBytes;
            keyBytes = Encoding.Unicode.GetBytes(Pwd);

            Rfc2898DeriveBytes derivedKey = new Rfc2898DeriveBytes(Pwd, keyBytes);

            RijndaelManaged rijndaelCSP = new RijndaelManaged();
            rijndaelCSP.Key = derivedKey.GetBytes(rijndaelCSP.KeySize / 8);
            rijndaelCSP.IV = derivedKey.GetBytes(rijndaelCSP.BlockSize / 8);

            ICryptoTransform encryptor = rijndaelCSP.CreateEncryptor();

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cryptoStream);
            writer.Write(testo);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);

        }

        public string DecodeString(string testo)
        {
            //tipologia codifica
            string Pwd = "";
            switch (_TipoCodifica)
            {
                case TipologiaCodifica.Old:
                    Pwd = REVISOFT_CRIPT_OLD_KEY;
                    break;
                case TipologiaCodifica.Normale:
                    Pwd = REVISOFT_CRIPT_NORMAL_KEY;
                    break;
                case TipologiaCodifica.Avanzata:
                    Pwd = REVISOFT_CRIPT_STRONG_KEY;
                    break;
            }

            byte[] keyBytes = Encoding.Unicode.GetBytes(Pwd);

            Rfc2898DeriveBytes derivedKey = new Rfc2898DeriveBytes(Pwd, keyBytes);

            RijndaelManaged rijndaelCSP = new RijndaelManaged();
            rijndaelCSP.Key = derivedKey.GetBytes(rijndaelCSP.KeySize / 8);
            rijndaelCSP.IV = derivedKey.GetBytes(rijndaelCSP.BlockSize / 8);
            ICryptoTransform decryptor = rijndaelCSP.CreateDecryptor();


            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(testo));
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            StreamReader reader = new StreamReader(cryptoStream);
            rijndaelCSP.Clear();
            return reader.ReadToEnd();

        }
#endregion
    }

    public class XmlDataProviderManager
    {
        private XmlDataProvider XDP;
        private string nameFile;
        private XmlManager x = new XmlManager();
    //private bool encoded = true;
    private bool loadOnFileChange = true; // E.B. nuovo campo

    //public XmlDataProviderManager(string file)
    public XmlDataProviderManager(string file,int loadNow=1)
    {
      XDP = new XmlDataProvider();
      x.TipoCodifica = XmlManager.TipologiaCodifica.Normale;

      loadOnFileChange = loadNow != 0; // E.B. eventuale inibizione caricamento automatico

      File = file;
    }

    // E.B. nuovo metodo
    public void SetDocument(XmlDocument doc = null)
    {
      if (doc == null) return;
      XDP.Document = doc;
      Refresh();
    }

		public XmlDataProviderManager(string file, bool encoded)
		{
			XDP = new XmlDataProvider();
			if (encoded)
			{
				x.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
			}
			else
			{
				x.TipoCodifica = XmlManager.TipologiaCodifica.Nessuna;
			}

			File = file;
		}

		public XmlDataProviderManager Clone()
		{
			XmlDataProviderManager newxdpm = new XmlDataProviderManager(nameFile);
			return newxdpm; 
		}

        public XmlDataProvider xdp
        {
            get { return XDP; }
        }

        public XmlDocument Document
        {
            get { return XDP.Document; }
        }

    public string File
    {
      get { return nameFile; }
      set 
      { 
        nameFile = value;
        //Load();
        if (loadOnFileChange) // E.B. sostituisce riga commentata
        {
          Load();
        }
        loadOnFileChange = true;
      }
    }

        public void Save()
        {
            if (XDP.Document != null)
            {
                x.SaveEncodedFile(nameFile, XDP.Document.OuterXml);
                Load();
            }
        }

        public void Load()
        {            
            XDP.Document = x.LoadEncodedFile(nameFile);

            Refresh();
        }

        public void Refresh()
        {
            XDP.Refresh();
        }
    }
}
*/
