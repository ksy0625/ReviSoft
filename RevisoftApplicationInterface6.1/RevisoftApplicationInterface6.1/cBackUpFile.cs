using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Linq;
using System.Data.SqlClient;
using System.Data;

namespace RevisoftApplication
{
    class BackUpFile
    {
        private WindowGestioneMessaggi message = new WindowGestioneMessaggi();
		private XmlManager x = new XmlManager();	
        private XmlDocument document = new XmlDocument();
        //4.6
        //private string file = string.Empty;

        //4.6 tipologia backup
        private static bool _BackupPersonalizzato;

        //4.6
        string cartellaBackUp = App.AppBackupFolder + "\\";
        string filebackup = App.AppBackUpDataFile;

        //4.12.3
        private int MaxOutputSegmentSize = 1024 * 1024 * 500; //500 megabyte


        public bool BackupPersonalizzato
        {
            get { return (bool)_BackupPersonalizzato; }
            set {
                    _BackupPersonalizzato = value;
                    if (_BackupPersonalizzato)
                    {
                        cartellaBackUp = App.AppBackupFolderUser + "\\";
                        filebackup = App.AppBackUpDataFileUser;
                    }
                }
        }

    //----------------------------------------------------------------------------+
    //                                 BackUpFile                                 |
    //----------------------------------------------------------------------------+
    public void BackUpFile_old()
        {
            App.ErrorLevel = App.ErrorTypes.Nessuno;
      //4.6
      //file = App.AppBackUpDataFile;
      x.TipoCodifica = XmlManager.TipologiaCodifica.Normale;	

            //controllo directory di backup
			DirectoryInfo di = new DirectoryInfo(cartellaBackUp);
			if (!di.Exists)
			{
				di.Create();
			}

            //4.6 controllo file di backup
            if (App.AppSetupBackupPersonalizzato)
            {
                if (!File.Exists(filebackup))
                {
                    string s = "";
                    s += "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                    s += "<ROOT>";
                    s += "<REVISOFT ID=\"10\" ChiaveServer=\"\" DataLicenzaProva=\"\" DataLicenza=\"\" />";
                    s += "<BACKUPS LastID=\"0\">";
                    s += "</BACKUPS>";
                    s += "</ROOT >";

                    //salvo dati
                    RevisoftApplication.XmlManager xBKIndex = new XmlManager();
                    xBKIndex.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                    xBKIndex.SaveEncodedFile(filebackup, s);
                }
            }
        }
    public BackUpFile()
    {
#if (!DBG_TEST)
      BackUpFile_old();return;
#endif
      App.ErrorLevel = App.ErrorTypes.Nessuno;
      x.TipoCodifica = XmlManager.TipologiaCodifica.Normale;
      DirectoryInfo di = new DirectoryInfo(cartellaBackUp);
      if (!di.Exists) di.Create();
      if (App.AppSetupBackupPersonalizzato)
      {
        if (!File.Exists(filebackup))
        {
          string s = "";
          s += "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
          s += "<ROOT>";
          s += "<REVISOFT ID=\"10\" ChiaveServer=\"\" DataLicenzaProva=\"\" DataLicenza=\"\" />";
          s += "<BACKUPS LastID=\"0\">";
          s += "</BACKUPS>";
          s += "</ROOT >";
          RevisoftApplication.XmlManager xBKIndex = new XmlManager();
          xBKIndex.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
          xBKIndex.SaveEncodedFile_old(filebackup, s);
        }
      }
    }


#region Funzioni Base
    //----------------------------------------------------------------------------+
    //                                   Check                                    |
    //----------------------------------------------------------------------------+
    private bool Check_old()
    {
        //controllo presenza File master
        if (!File.Exists(filebackup))
        {
            //4.6
            if (_BackupPersonalizzato)
            {
                string s = "";
                s += "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                s += "<ROOT>";
                s += "<REVISOFT ID=\"10\" ChiaveServer=\"\" DataLicenzaProva=\"\" DataLicenza=\"\" />";
                s += "<BACKUPS LastID=\"0\">";
                s += "</BACKUPS>";
                s += "</ROOT >";

                //salvo dati
                RevisoftApplication.XmlManager xBKIndex = new XmlManager();
                xBKIndex.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                xBKIndex.SaveEncodedFile(filebackup, s);
                return true;
            }
            else
            {
                ErrorCritical(WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.MancaFileBackUp);
                return false;
            }
        }

        return true;
    }
    private bool Check()
    {
#if (!DBG_TEST)
      return Check_old();
#endif
      if (!File.Exists(filebackup))
      {
        string s = "";
        s += "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        s += "<ROOT>";
        s += "<REVISOFT ID=\"10\" ChiaveServer=\"\" DataLicenzaProva=\"\" DataLicenza=\"\" />";
        s += "<BACKUPS LastID=\"0\">";
        s += "</BACKUPS>";
        s += "</ROOT >";
        RevisoftApplication.XmlManager xBKIndex = new XmlManager();
        xBKIndex.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
        xBKIndex.SaveEncodedFile_old(filebackup, s);
        //if (_BackupPersonalizzato)
        //{
        //  string s = "";
        //  s += "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        //  s += "<ROOT>";
        //  s += "<REVISOFT ID=\"10\" ChiaveServer=\"\" DataLicenzaProva=\"\" DataLicenza=\"\" />";
        //  s += "<BACKUPS LastID=\"0\">";
        //  s += "</BACKUPS>";
        //  s += "</ROOT >";
        //  RevisoftApplication.XmlManager xBKIndex = new XmlManager();
        //  xBKIndex.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
        //  xBKIndex.SaveEncodedFile_old(filebackup, s);
        //  return true;
        //}
        //else
        //{
        //  ErrorCritical(WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.MancaFileBackUp);
        //  return false;
        //}
      }
      return true;
    }

    //----------------------------------------------------------------------------+
    //                                    Open                                    |
    //----------------------------------------------------------------------------+
    private void Open_old()
        {            
            if(Check())
            {
                //carico file
                try
                {
					document = x.LoadEncodedFile(filebackup);
					Utilities u = new Utilities();
					if (!u.CheckXmlDocument(document, App.TipoFile.BackUp))
					{
						throw new Exception("Documento non valido. ID diverso da standard TipoFile");
					}
                }
                catch (Exception ex)
                {
                    string log = ex.Message;

                    Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileBackUp);
                }
            }
        }
    private void Open()
    {
#if (!DBG_TEST)
      Open_old();return;
#endif
      if (Check())
      {
        try
        {
          document = x.LoadEncodedFile_old(filebackup);
          Utilities u = new Utilities();
          if (!u.CheckXmlDocument(document, App.TipoFile.BackUp))
          {
            throw new Exception("Documento non valido. ID diverso da standard TipoFile");
          }
        }
        catch (Exception ex)
        {
          string log = ex.Message;
          Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCaricamentoFileBackUp);
        }
      }
    }

    private void Close()
        {
			document = new XmlDocument();
        }

    //----------------------------------------------------------------------------+
    //                                    Save                                    |
    //----------------------------------------------------------------------------+
    private void Save_old()
    {
        if(Check())
        {
            //salvo file
            try
            {
                //document.Save(file);
                x.SaveEncodedFile(filebackup, document.OuterXml);
            }
            catch (Exception ex)
            {
                string log = ex.Message;

                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileBackUp);
            }
        }
    }
    private void Save()
    {
#if (!DBG_TEST)
      Save_old();return;
#endif
      if (Check())
      {
        try { x.SaveEncodedFile_old(filebackup, document.OuterXml); }
        catch (Exception ex)
        {
          string log = ex.Message;
          Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileBackUp);
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

#region backup
		public ArrayList GetBackUps()
		{
			ArrayList results = new ArrayList();

			try
			{
				Open();

				XmlNodeList xNodes = document.SelectNodes("/ROOT/BACKUPS/BACKUP");

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

		public Hashtable GetBackUp(string IDBackUp)
		{
			Hashtable result = new Hashtable();

			try
			{
				Open();

				XmlNode xNode = document.SelectSingleNode("/ROOT/BACKUPS/BACKUP[@ID='" + IDBackUp + "']");

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

		public void DeleteBackUp(string IDBackUp)
		{
			try
			{
				Open();

				XmlNode xNode = document.SelectSingleNode("/ROOT/BACKUPS/BACKUP[@ID='" + IDBackUp + "']");

				if (xNode != null)
				{
					string nomefileBackUp = xNode.Attributes["File"].Value;

					FileInfo fi = new FileInfo(cartellaBackUp + nomefileBackUp);
					if (fi.Exists)
					{
						fi.Delete();
					}

					xNode.ParentNode.RemoveChild(xNode);

					Save();
				}

				Close();
			}
			catch (Exception ex)
			{
				string log = ex.Message;

				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInCancellazioneFileMaster);
			}
		}
    //----------------------------------------------------------------------------+
    //                                  Restore                                   |
    //----------------------------------------------------------------------------+
    public bool Restore_old(string IDBackUp)
		{
			string nomefileBackUp = "";

			Open();

			XmlNode xNode = document.SelectSingleNode("/ROOT/BACKUPS/BACKUP[@ID='" + IDBackUp + "']");

			if (xNode != null)
			{
				nomefileBackUp = xNode.Attributes["File"].Value;
			}

			Close();

			if (nomefileBackUp == "" || !(new FileInfo(cartellaBackUp + nomefileBackUp)).Exists)
			{
				return false;
			}

            DirectoryInfo rdf = new DirectoryInfo(App.AppDataDataFolder);
            DirectoryInfo uuf = new DirectoryInfo(App.AppDocumentiFolder);

			if (rdf.Exists)
			{
				rdf.Delete(true);
			}

			if (uuf.Exists)
			{
				uuf.Delete(true);
			}

			FileInfo fi = new FileInfo(App.AppMasterDataFile);

			if (fi.Exists)
			{
				fi.Delete();
			}

            fi = new FileInfo( App.AppDocumentiDataFile );

            if ( fi.Exists )
            {
                fi.Delete();
            }

			//apro lo zip
			Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(cartellaBackUp + nomefileBackUp);
			zip.Password = App.ZipFilePassword;
			zip.ExtractAll(App.AppDataFolder);

			return true;
		}
    public bool Restore(string IDBackUp)
    {
#if (!DBG_TEST)
      return Restore_old(IDBackUp);
#endif
      string nomefileBackUp = "";
      Open();
      XmlNode xNode = document.SelectSingleNode(
        "/ROOT/BACKUPS/BACKUP[@ID='" + IDBackUp + "']");
      if (xNode != null) nomefileBackUp = xNode.Attributes["File"].Value;
      Close();
      if (nomefileBackUp == ""
        || !(new FileInfo(cartellaBackUp + nomefileBackUp)).Exists)
      {
        return false;
      }
      DirectoryInfo rdf = new DirectoryInfo(App.AppDataDataFolder);
      DirectoryInfo uuf = new DirectoryInfo(App.AppDocumentiFolder);
      if (rdf.Exists) rdf.Delete(true);
      if (uuf.Exists) uuf.Delete(true);
      FileInfo fi = new FileInfo(App.AppMasterDataFile);
      if (fi.Exists) fi.Delete();
      fi = new FileInfo(App.AppDocumentiDataFile);
      if (fi.Exists) fi.Delete();
      return RestoreFile(cartellaBackUp + nomefileBackUp);
    }

    //----------------------------------------------------------------------------+
    //                                RestoreFile                                 |
    //----------------------------------------------------------------------------+
    public bool RestoreFile_old(string nomefileBackUp)
		{
            //controlli file
            if (nomefileBackUp == null || nomefileBackUp.Trim() == "")
            {
                return false;
            }

			//verifico il contenuto
            try
            {
			    Ionic.Zip.ZipFile z = new Ionic.Zip.ZipFile(nomefileBackUp);
			    z.Password = App.ZipFilePassword;
                z.ExtractAll(App.AppTempFolder, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                z.Dispose();
                z = null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

            //contenuto corretto, elimino cartelle vecchie
            //rimuovo cartelle
			DirectoryInfo rdf = new DirectoryInfo(App.AppDataDataFolder);
			if (rdf.Exists)
			{
				rdf.Delete(true);
			}
			DirectoryInfo uuf = new DirectoryInfo(App.AppDocumentiFolder);
			if (uuf.Exists)
			{
				uuf.Delete(true);
			}
            //rimuovo master file
            FileInfo fi = new FileInfo(App.AppMasterDataFile);
			if (fi.Exists)
			{
				fi.Delete();
			}
            //rimuovo indice documenti
            fi = new FileInfo(App.AppDocumentiDataFile);
            if (fi.Exists)
            {
                fi.Delete();
            }

            //copio file
            Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(nomefileBackUp);
            zip.Password = App.ZipFilePassword;
			zip.ExtractAll(App.AppDataFolder);

            //exit
			return true;
		}
    public bool RestoreFile(string nomefileBackUp)
    {
#if (!DBG_TEST)
      return RestoreFile_old(nomefileBackUp);
#endif
      string str, cartellatmp;
      XmlDocument doc = new XmlDocument();

      // controlli file
      if (nomefileBackUp == null || nomefileBackUp.Trim() == "") return false;
      if (nomefileBackUp.Split('\\').Last().StartsWith(App.BK_DECODED_PREFIX)) return RestoreFile_decoded(nomefileBackUp);
      cartellatmp = App.TMP_FOLDER;
      if (!cartellatmp.EndsWith(@"\")) cartellatmp += @"\";
      str = cartellatmp + Guid.NewGuid().ToString();
      DirectoryInfo di = new DirectoryInfo(str);
      if (di.Exists)
      {
        // errore directory già esistente aspettare processo terminato da parte
        // di altro utente
        return false;
      }
      cartellatmp = str;
      di.Create();
      // verifico il contenuto
      try
      {
        Ionic.Zip.ZipFile z = new Ionic.Zip.ZipFile(nomefileBackUp);
        z.Password = App.ZipFilePassword;
        z.ExtractAll(
          cartellatmp, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
        z.Dispose();
        z = null;
      }
      catch (Exception e)
      {
        di.Delete(true);
        MessageBox.Show(e.Message);
        return false;
      }
      // contenuto corretto, elimino cartelle vecchie
      DirectoryInfo rdf = new DirectoryInfo(App.AppDataDataFolder);
      if (rdf.Exists) rdf.Delete(true);
      DirectoryInfo uuf = new DirectoryInfo(App.AppDocumentiFolder);
      if (uuf.Exists) uuf.Delete(true);
      // rimuovo master file
      FileInfo fi = new FileInfo(App.AppMasterDataFile);
      if (fi.Exists) fi.Delete();
      // rimuovo indice documenti
      fi = new FileInfo(App.AppDocumentiDataFile);
      if (fi.Exists) fi.Delete();
      // decodifica di tutti i files

      // RevisoftApp.rmdf e RevisoftApp.rdocf
      XmlManager x = new XmlManager { TipoCodifica = XmlManager.TipologiaCodifica.Normale };
      string[] files = Directory.GetFiles(
        cartellatmp, @"*.*", SearchOption.TopDirectoryOnly);
      foreach (string s in files)
      {
        doc = x.LoadEncodedFile_old(s);
        str = doc.InnerXml.Replace("&#x8;", "");
        doc.InnerXml = str;
        doc.Save(s);
      }
      //MessageBox.Show("decodifica RevisoftApp.rmdf e RevisoftApp.rdocf terminata");

      // dati in DataFile
      files = Directory.GetFiles(
        cartellatmp + @"\" + App.DataFolder, @"*.*", SearchOption.TopDirectoryOnly);
      foreach (string s in files)
      {
        doc = x.LoadEncodedFile_old(s);
        str = doc.InnerXml.Replace("&#x8;", "");
        doc.InnerXml = str;
        doc.Save(s);
      }
      //MessageBox.Show("decodifica contenuto DataFile terminata");
      // importazione in SQL
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("dbo.impMasterFile", conn);
        cmd.Parameters.AddWithValue("@masterFileFolder", cartellatmp);
        cmd.Parameters.AddWithValue("@dataFolder", cartellatmp + @"\" + App.DataFolder);
        cmd.Parameters.AddWithValue("@docFolder", cartellatmp);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          str = ex.Message;
          if (!App.m_bNoExceptionMsg) MessageBox.Show(ex.Message);
        }
      }
      // ripristino cartelle
      DirectoryInfo d_rdf = new DirectoryInfo(App.AppDataDataFolder); // ex "\\RDF"
      if (!d_rdf.Exists) d_rdf.Create();
      DirectoryInfo d_uuff = new DirectoryInfo(App.AppDocumentiFolder); // ex "\\UserUF"
      if (!d_uuff.Exists) d_uuff.Create();
      DirectoryInfo d_ufl = new DirectoryInfo(App.AppDocumentiFlussiFolder); // ex "\\UserUF"
      if (!d_ufl.Exists) d_ufl.Create();
      // copia contenuto
      foreach (string item in Directory.GetFiles(cartellatmp,
        "*.*", SearchOption.AllDirectories))
        File.Copy(item, item.Replace(cartellatmp, App.AppDataFolder), true);
      Directory.Delete(cartellatmp, true);
      App.m_xmlCache.Clear();
      return true;
    }
    public bool RestoreFile_decoded(string nomefileBackUp)
    {
#if (!DBG_TEST)
      return RestoreFile_old(nomefileBackUp);
#endif
      string str, cartellatmp;
      XmlDocument doc = new XmlDocument();

      // controlli file
      cartellatmp = App.TMP_FOLDER;
      if (!cartellatmp.EndsWith(@"\")) cartellatmp += @"\";
      str = cartellatmp + Guid.NewGuid().ToString();
      DirectoryInfo di = new DirectoryInfo(str);
      if (di.Exists)
      {
        // errore directory già esistente aspettare processo terminato da parte
        // di altro utente
        return false;
      }
      cartellatmp = str;
      di.Create();
      // verifico il contenuto
      try
      {
        Ionic.Zip.ZipFile z = new Ionic.Zip.ZipFile(nomefileBackUp);
        z.ExtractAll(
          cartellatmp, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
        z.Dispose();
        z = null;
      }
      catch (Exception e)
      {
        di.Delete(true);
        MessageBox.Show(e.Message);
        return false;
      }
      // contenuto corretto, elimino cartelle vecchie
      DirectoryInfo rdf = new DirectoryInfo(App.AppDataDataFolder);
      if (rdf.Exists) rdf.Delete(true);
      DirectoryInfo uuf = new DirectoryInfo(App.AppDocumentiFolder);
      if (uuf.Exists) uuf.Delete(true);
      // rimuovo master file
      FileInfo fi = new FileInfo(App.AppMasterDataFile);
      if (fi.Exists) fi.Delete();
      // rimuovo indice documenti
      fi = new FileInfo(App.AppDocumentiDataFile);
      if (fi.Exists) fi.Delete();
      // decodifica di tutti i files

      // RevisoftApp.rmdf e RevisoftApp.rdocf
      XmlManager x = new XmlManager { TipoCodifica = XmlManager.TipologiaCodifica.Nessuna };
      string[] files = Directory.GetFiles(
        cartellatmp, @"*.*", SearchOption.TopDirectoryOnly);
      foreach (string s in files)
      {
        doc = x.LoadEncodedFile_old(s);
        str = doc.InnerXml.Replace("&#x8;", "");
        doc.InnerXml = str;
        doc.Save(s);
      }

      // dati in DataFile
      files = Directory.GetFiles(
        cartellatmp + @"\" + App.DataFolder, @"*.*", SearchOption.TopDirectoryOnly);
      foreach (string s in files)
      {
        doc = x.LoadEncodedFile_old(s);
        str = doc.InnerXml.Replace("&#x8;", "");
        doc.InnerXml = str;
        doc.Save(s);
      }
      // importazione in SQL
      using (SqlConnection conn = new SqlConnection(App.connString))
      {
        conn.Open();
        SqlCommand cmd = new SqlCommand("dbo.impMasterFile", conn);
        cmd.Parameters.AddWithValue("@masterFileFolder", cartellatmp);
        cmd.Parameters.AddWithValue("@dataFolder", cartellatmp + @"\" + App.DataFolder);
        cmd.Parameters.AddWithValue("@docFolder", cartellatmp);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = App.m_CommandTimeout;
        try { cmd.ExecuteNonQuery(); }
        catch (Exception ex)
        {
          str = ex.Message;
          if (!App.m_bNoExceptionMsg) MessageBox.Show(ex.Message);
        }
      }
      // ripristino cartelle
      DirectoryInfo d_rdf = new DirectoryInfo(App.AppDataDataFolder); // ex "\\RDF"
      if (!d_rdf.Exists) d_rdf.Create();
      str = App.AppDataDataFolder;if (!str.EndsWith(@"\")) str = str + @"\";
      str = str + "XAML";
      DirectoryInfo d_xaml = new DirectoryInfo(str);
      if (!d_xaml.Exists) d_xaml.Create();
      DirectoryInfo d_uuff = new DirectoryInfo(App.AppDocumentiFolder); // ex "\\UserUF"
      if (!d_uuff.Exists) d_uuff.Create();
      DirectoryInfo d_ufl = new DirectoryInfo(App.AppDocumentiFlussiFolder); // ex "\\UserUF"
      if (!d_ufl.Exists) d_ufl.Create();
      // copia contenuto
      foreach (string item in Directory.GetFiles(cartellatmp,
        "*.*", SearchOption.AllDirectories))
        File.Copy(item, item.Replace(cartellatmp, App.AppDataFolder), true);
      Directory.Delete(cartellatmp, true);
      App.m_xmlCache.Clear();
      return true;
    }

    //----------------------------------------------------------------------------+
    //                                 SetBackUp                                  |
    //----------------------------------------------------------------------------+
    public int SetBackUp_old(Hashtable values, int IDBackUp)
		{
			try
			{
				Open();

				if (IDBackUp == App.MasterFile_NewID)
				{
					XmlNode root = document.SelectSingleNode("/ROOT/BACKUPS");

					IDBackUp = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
										
					//string nomefileBackUp = "BackUp" + DateTime.Now.ToShortDateString().Replace("/", "") + DateTime.Now.ToShortTimeString().Replace(":", "");
                    string nomefileBackUp = "RevisoftBackUp_" + IDBackUp.ToString().PadLeft(3, '0');
                    //aggiungo estensione a file
                    Utilities u = new Utilities();
                    nomefileBackUp += u.EstensioneFile(App.TipoFile.BackUp);
					FileInfo buf = new FileInfo(cartellaBackUp + nomefileBackUp);

					int indice = 1;

					while (buf.Exists)
					{
						nomefileBackUp = nomefileBackUp.Split('(')[0] + "(" + indice.ToString() + ")";
						buf = new FileInfo(cartellaBackUp + nomefileBackUp);
						indice++;
					}

					string lastindex = IDBackUp.ToString();

                    string cartellatmp = App.AppTempFolder + Guid.NewGuid().ToString();
					DirectoryInfo di = new DirectoryInfo(cartellatmp);
					if (di.Exists)
					{
						//errore directory già esistente aspettare processo terminato da parte di altro utente
						return -1;
					}

					di.Create();

					//nuove cartelle per i file necessari
					DirectoryInfo d_rdf = new DirectoryInfo(cartellatmp + "\\" + App.DataFolder); // ex "\\RDF"
					d_rdf.Create();

                    DirectoryInfo d_uuff = new DirectoryInfo(cartellatmp + "\\" + App.UserFileFolder); // ex "\\UserUF"
					d_uuff.Create();

					//raccolgo i file in una cartella temporanea
                    DirectoryInfo rdf = new DirectoryInfo(App.AppDataDataFolder);
					foreach (FileInfo file in rdf.GetFiles())
					{
                        file.IsReadOnly = false;
						file.CopyTo(d_rdf.FullName + "\\" + file.Name);
					}

                    DirectoryInfo uuf = new DirectoryInfo(App.AppDocumentiFolder);
					foreach (FileInfo file in uuf.GetFiles())
					{
                        file.IsReadOnly = false;
						file.CopyTo(d_uuff.FullName + "\\" + file.Name);
					}

					//sposto il file masterfile
					FileInfo fi = new FileInfo(App.AppMasterDataFile);
                    fi.IsReadOnly = false;
					fi.CopyTo(di.FullName + "\\" + fi.Name);

                    fi = new FileInfo( App.AppDocumentiDataFile );
                    fi.IsReadOnly = false;
                    fi.CopyTo( di.FullName + "\\" + fi.Name );

					//creo lo zip
					Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
					zip.Password = App.ZipFilePassword;

					zip.AddDirectory(di.FullName);
                    zip.MaxOutputSegmentSize = MaxOutputSegmentSize;
                    //zip.ParallelDeflateThreshold = -1;
                    //zip.TempFileFolder = cartellaBackUp;
                    zip.Save(cartellaBackUp + nomefileBackUp);

					//Cancello i temporanei
                    try
                    {                        
					    di.Delete(true);
                    }
                    catch ( Exception ex)
                    {
                        string log = ex.Message;
                    }

					string xml = "<BACKUP ID=\"" + lastindex + "\" Data=\"" + DateTime.Now.ToShortDateString() + "\" Ora=\"" + DateTime.Now.ToShortTimeString() + "\" File=\"" + nomefileBackUp + "\" />";
					XmlDocument doctmp = new XmlDocument();
					doctmp.LoadXml(xml);

					XmlNode tmpNode = doctmp.SelectSingleNode("/BACKUP");
					XmlNode cliente = document.ImportNode(tmpNode, true);

					root.AppendChild(cliente);

					root.Attributes["LastID"].Value = lastindex;
				}
				else
				{
					XmlNode xNode = document.SelectSingleNode("/ROOT/BACKUPS/BACKUP[@ID='" + IDBackUp.ToString() + "']");

					xNode.Attributes["Data"].Value = values["Data"].ToString();
					xNode.Attributes["Ora"].Value = values["Ora"].ToString();
					xNode.Attributes["File"].Value = values["File"].ToString();
				}

				Save();

				Close();

			}
			catch (Exception ex)
			{
				string log = ex.ToString();
                //andrea x visualizzazione errore tester
                //MessageBox.Show(log);
                System.IO.File.WriteAllText(System.IO.Path.Combine(App.AppLogFolder, String.Format("{0:yyyyMMddHHmmss}_bklog.txt", DateTime.Now)),log);
                
				Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
			}

			return IDBackUp;
		}
    public int SetBackUp(Hashtable values, int IDBackUp)
    {
#if (!DBG_TEST)
      return SetBackUp_old(values, IDBackUp);
#endif
      try
      {
        Open();
        if (IDBackUp == App.MasterFile_NewID)
        {
          XmlNode root = document.SelectSingleNode("/ROOT/BACKUPS");
          IDBackUp = (Convert.ToInt32(root.Attributes["LastID"].Value) + 1);
          string nomefileBackUp = "RevisoftBackUp_" + IDBackUp.ToString().PadLeft(3, '0');
          // aggiungo estensione a file
          Utilities u = new Utilities();
          nomefileBackUp += u.EstensioneFile(App.TipoFile.BackUp);
          FileInfo buf = new FileInfo(cartellaBackUp + nomefileBackUp);
          int indice = 1;
          while (buf.Exists)
          {
            nomefileBackUp = nomefileBackUp.Split('(')[0] + "(" + indice.ToString() + ")";
            buf = new FileInfo(cartellaBackUp + nomefileBackUp);
            indice++;
          }
          string lastindex = IDBackUp.ToString();
          SetBackUpFile(cartellaBackUp + nomefileBackUp);
          string xml = "<BACKUP ID=\"" + lastindex + "\" Data=\"" +
            DateTime.Now.ToShortDateString() + "\" Ora=\"" +
            DateTime.Now.ToShortTimeString() + "\" File=\"" + nomefileBackUp + "\" />";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/BACKUP");
          XmlNode cliente = document.ImportNode(tmpNode, true);
          root.AppendChild(cliente);
          root.Attributes["LastID"].Value = lastindex;
        }
        else
        {
          XmlNode xNode = document.SelectSingleNode(
            "/ROOT/BACKUPS/BACKUP[@ID='" + IDBackUp.ToString() + "']");
          xNode.Attributes["Data"].Value = values["Data"].ToString();
          xNode.Attributes["Ora"].Value = values["Ora"].ToString();
          xNode.Attributes["File"].Value = values["File"].ToString();
        }
        Save();
        Close();
      }
      catch (Exception ex)
      {
        string log = ex.ToString();
        System.IO.File.WriteAllText(
          System.IO.Path.Combine(App.AppLogFolder,
          String.Format("{0:yyyyMMddHHmmss}_bklog.txt", DateTime.Now)), log);
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
      return IDBackUp;
    }

    //----------------------------------------------------------------------------+
    //                               SetBackUpFile                                |
    //----------------------------------------------------------------------------+
    public void SetBackUpFile_old(string nomefileBackUp)
		{
			try
			{
				FileInfo buf = new FileInfo(nomefileBackUp);

				string cartellatmp = App.AppTempFolder + Guid.NewGuid().ToString();
				DirectoryInfo di = new DirectoryInfo(cartellatmp);
				if (di.Exists)
				{
					//errore directory già esistente aspettare processo terminato da parte di altro utente
					return;
				}

				di.Create();

				//nuove cartelle per i file necessari
				DirectoryInfo d_rdf = new DirectoryInfo(cartellatmp + "\\" + App.DataFolder); // ex "\\RDF"
				d_rdf.Create();

				DirectoryInfo d_uuff = new DirectoryInfo(cartellatmp + "\\" + App.UserFileFolder); // ex "\\UserUF"
				d_uuff.Create();

				//raccolgo i file in una cartella temporanea
				DirectoryInfo rdf = new DirectoryInfo(App.AppDataDataFolder);
				foreach (FileInfo file in rdf.GetFiles())
				{
                    file.IsReadOnly = false;
					file.CopyTo(d_rdf.FullName + "\\" + file.Name);
				}

				DirectoryInfo uuf = new DirectoryInfo(App.AppDocumentiFolder);
				foreach (FileInfo file in uuf.GetFiles())
				{
                    file.IsReadOnly = false;
					file.CopyTo(d_uuff.FullName + "\\" + file.Name);
				}

				//sposto il file masterfile
				FileInfo fi = new FileInfo(App.AppMasterDataFile);
                fi.IsReadOnly = false;
				fi.CopyTo(di.FullName + "\\" + fi.Name);

				fi = new FileInfo(App.AppDocumentiDataFile);
                fi.IsReadOnly = false;
				fi.CopyTo(di.FullName + "\\" + fi.Name);

				//creo lo zip
				Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
				zip.Password = App.ZipFilePassword;

				zip.AddDirectory(di.FullName);

                zip.MaxOutputSegmentSize = MaxOutputSegmentSize;
                //zip.ParallelDeflateThreshold = -1;
                //zip.TempFileFolder = cartellatmp;
                zip.Save(nomefileBackUp);

				//Cancello i temporanei                
				di.Delete(true);

			}
			catch (Exception ex)
			{
				string log = ex.ToString();
                System.IO.File.WriteAllText(System.IO.Path.Combine(App.AppLogFolder, String.Format("{0:yyyyMMddHHmmss}_bklog.txt", DateTime.Now)), log);
                Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
			}
		}
    public void SetBackUpFile(string nomefileBackUp)
    {
#if (!DBG_TEST)
      SetBackUpFile_old(nomefileBackUp);return;
#endif
      try
      {
        FileInfo buf = new FileInfo(nomefileBackUp);
        string cartellatmp = App.TMP_FOLDER + Guid.NewGuid().ToString();
        DirectoryInfo di = new DirectoryInfo(cartellatmp);
        if (di.Exists)
        {
          // errore directory già esistente aspettare processo terminato da parte
          // di altro utente
          return;
        }
        di.Create();
        // nuove cartelle per i file necessari
        DirectoryInfo d_rdf = new DirectoryInfo(cartellatmp + "\\" + App.DataFolder); // ex "\\RDF"
        d_rdf.Create();
        DirectoryInfo d_uuff = new DirectoryInfo(cartellatmp + "\\" + App.UserFileFolder); // ex "\\UserUF"
        d_uuff.Create();
        //----------------------------------------------------------------------------+
        //                            lettura master file                             |
        //----------------------------------------------------------------------------+
        MasterFile mf = new MasterFile();
        XmlDocument mfDoc = mf.GetDocument();
        if (mfDoc == null) return;
        //----------------------------------------------------------------------------+
        //   scansione di tutti i nodi contenenti "FileData" e quindi anche "File"    |
        //----------------------------------------------------------------------------+
        string fName;
        XmlManager manager = new XmlManager();
        XmlDocument doc;
        string[] attrs = { "File", "FileData" };
        foreach (XmlNode node in mfDoc.SelectNodes("//*[@FileData]"))
        {
          foreach (string s in attrs)
          {
            // salvataggio albero se esiste
            if (node.Attributes[s]!=null)
            {
              fName = node.Attributes[s].Value;
              doc = manager.LoadEncodedFile(fName);
              if (doc != null)
              {
                manager.SaveEncodedFile_old(
                  cartellatmp + @"\" + App.DataFolder + @"\" + fName, doc.OuterXml);
              }
            }
          }
        }
        //----------------------------------------------------------------------------+
        //                         copia di tutti i documenti                         |
        //----------------------------------------------------------------------------+
        foreach (string dir in Directory.GetDirectories(App.AppDocumentiFolder,
          "*",SearchOption.AllDirectories))
          Directory.CreateDirectory(dir.Replace(App.AppDocumentiFolder, d_uuff.FullName));
        foreach (string item in Directory.GetFiles(App.AppDocumentiFolder,
          "*.*",SearchOption.AllDirectories))
          File.Copy(item, item.Replace(App.AppDocumentiFolder, d_uuff.FullName), true);
        //----------------------------------------------------------------------------+
        //                           scrittura master file                            |
        //----------------------------------------------------------------------------+
        manager.SaveEncodedFile_old(
          cartellatmp + @"\" + App.AppMasterDataFile.Split('\\').Last(), mfDoc.OuterXml);
        //----------------------------------------------------------------------------+
        //                          scrittura file documenti                          |
        //----------------------------------------------------------------------------+
        fName = "revisoftapp.rdocf";
        doc = manager.LoadEncodedFile(fName);
        if (doc != null)
        {
          manager.SaveEncodedFile_old(cartellatmp + @"\" + fName, doc.OuterXml);
        }

        //creo lo zip
        Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
        zip.Password = App.ZipFilePassword;
        zip.AddDirectory(di.FullName);
        zip.MaxOutputSegmentSize = MaxOutputSegmentSize;
        zip.Save(nomefileBackUp);
        // cancello i dati temporanei                
        di.Delete(true);
      }
      catch (Exception ex)
      {
        string log = ex.ToString();
        System.IO.File.WriteAllText(
          System.IO.Path.Combine(App.AppLogFolder,
          String.Format("{0:yyyyMMddHHmmss}_bklog.txt", DateTime.Now)), log);
        Error(WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioFileMaster);
      }
    }
#endregion
  }
}
