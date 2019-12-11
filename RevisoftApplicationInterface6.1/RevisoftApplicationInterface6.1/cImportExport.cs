using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.IO;
using System.Windows;

namespace RevisoftApplication
{
	static class cImportExport
	{

    static public bool Export(string nomefileexport, int idAnagrafica, bool Condividi)
        {
		      	MasterFile mf = MasterFile.Create();

            //andrea 2.8
            //if (mf.GetAllXmlCliente(idAnagrafica, nomefileexport))
            //{
            //    MessageBox.Show("Esportazione avvenuta con successo");
            //}

            return mf.GetAllXmlCliente( idAnagrafica, nomefileexport, Condividi );
        }

        //andrea
        static public bool ExportNoVerbose(string nomefileexport, int idAnagrafica, bool Condividi)
        {
            MasterFile mf = MasterFile.Create();
            return mf.GetAllXmlCliente(idAnagrafica, nomefileexport, Condividi);
        }


        //versione precedente
		static public void Export(string nomefileexport, XmlNode cliente, XmlNode nodo, ArrayList rdf, ArrayList uuf)
		{
			//genero cartella temporanea
			string cartellatmp = App.AppDataFolder + "\\" + Guid.NewGuid().ToString();
			DirectoryInfo di = new DirectoryInfo(cartellatmp);
			if(di.Exists)
			{
				//errore directory già esistente aspettare processo terminato da parte da altro utente
				return;
			}

			di.Create();

			//nuove cartelle per i file necessari
			DirectoryInfo d_rdf = new DirectoryInfo(cartellatmp + "\\RDF");
			d_rdf.Create();

			DirectoryInfo d_uuff = new DirectoryInfo(cartellatmp + "\\UserUF");
			d_uuff.Create();

			//raccolgo i file in una cartella temporanea
			FileInfo fi;
			
			foreach (string file in rdf)
			{
				fi = new FileInfo(file);
                fi.IsReadOnly = false;
				fi.CopyTo(d_rdf.FullName + "\\" + fi.Name);
			}

			foreach (string file in uuf)
			{
				fi = new FileInfo(file);
                fi.IsReadOnly = false;
				fi.CopyTo(d_uuff.FullName + "\\" + fi.Name);
			}

			//file cliente e nodo
			XmlDocument doc = new XmlDocument();
			XmlElement root = doc.CreateElement("ROOT");
			root.AppendChild(doc.ImportNode(cliente, true));
			root.AppendChild(doc.ImportNode(nodo, true));
			doc.AppendChild(root);
			doc.Save(cartellatmp + "\\file.xml");

			//creo lo zip
			Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile();
			zip.AddDirectory(di.FullName);
			zip.Save(nomefileexport);

			//Cancello i temporanei
			di.Delete(true);
		}

		static public void ImportTemplate(string nomefileimport)
		{
			//apro lo zip
			try
			{
            

				Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(nomefileimport);
				zip.Password = App.ZipFilePassword;
				zip.ExtractAll(App.AppTemplateFolder, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);

               
            }
			catch (Exception ex)
			{
				string log = ex.Message;
				MessageBox.Show("Impossibile comprendere il file in ingresso");
			}			
		}

        static public bool Import(string nomefileimport)
        {
            //versione 3.0 - verifico formato path e converto in UNC
            if (nomefileimport.IndexOf(':') == 1)
            {
                RevisoftApplication.Utilities u = new Utilities();
                nomefileimport = u.GetRealPathFile(nomefileimport);
            }
            //importo file
            return Import(nomefileimport, true);
        }

		static public bool Import(string nomefileimport, bool verbose)
		{
#if (DBG_TEST)
      return StaticUtilities.ImportEstraiDati(nomefileimport,verbose);
#else
            try
            {
                string cartellatmp = App.AppTempFolder + Guid.NewGuid().ToString();

                DirectoryInfo di = new DirectoryInfo(cartellatmp);

                if (di.Exists)
                {
                    //errore directory già esistente aspettare processo terminato da parte di altro utente
                    return false;
                }

                //apro lo zip
                Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(nomefileimport);
                zip.Password = App.ZipFilePassword;

                zip.ExtractAll(cartellatmp);

                XmlDataProviderManager _d = new XmlDataProviderManager(cartellatmp + "\\all.xml", false);

                //andrea - versione 3.0 - import/export limitato a livello di licenza
                XmlNode licenza = _d.Document.SelectSingleNode("/ROOT/LICENZA");
                if (licenza != null)
                {
                    string importCodiceMacchinaServer = licenza.Attributes["CodiceMacchinaServer"].Value.ToString().Split('-')[0];
                    string importCodiceMacchina = licenza.Attributes["CodiceMacchina"].Value.ToString().Split('-')[0];

                    //Gestione licenza
                    RevisoftApplication.GestioneLicenza l = new GestioneLicenza();

                    if (!l.VerificaCodiceMacchinaFileImportato(importCodiceMacchinaServer.Split('-')[0], importCodiceMacchina.Split('-')[0]))
                        return false;

                }
                else
                {
                    if (MessageBox.Show("ATTENZIONE: vengono acquisiti tutti i dati (Revisione e Verifiche) e verranno sovrascritti sull’unità di destinazione. Per importare una sola parte dei dati utilizzare il CONDIVIDI DATI presente nelle aree specifiche. Procedere?", "ATTENZIONE", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    {
                        return false;
                    }
                }

                XmlNode cliente_importato = _d.Document.SelectSingleNode("/ROOT/CLIENTE");

                //aggiorno MasterDataFile
                MasterFile.ForceRecreate();
                MasterFile mf = MasterFile.Create();

#region CLIENTE
                Hashtable ht = new Hashtable();
                foreach (XmlAttribute item in cliente_importato.Attributes)
                {
                    ht.Add(item.Name, item.Value);
                }
                //ht.Add("RagioneSociale", cliente_importato.Attributes["RagioneSociale"].Value);
                //ht.Add("CodiceFiscale", cliente_importato.Attributes["CodiceFiscale"].Value);
                //ht.Add("PartitaIVA", cliente_importato.Attributes["PartitaIVA"].Value);
                //ht.Add("Note", cliente_importato.Attributes["Note"].Value);
                //ht.Add("EsercizioDal", cliente_importato.Attributes["EsercizioDal"].Value);
                //ht.Add("EsercizioAl", cliente_importato.Attributes["EsercizioAl"].Value);
                //ht.Add("Esercizio", cliente_importato.Attributes["Esercizio"].Value);

                int IDCliente = mf.CheckEsistenzaCliente(ht);
                
                if (IDCliente == -1)
                {
                    if (verbose)
                    {
                        if (MessageBox.Show("Esiste già un cliente " + ((cliente_importato.Attributes["RagioneSociale"] == null)? "" : cliente_importato.Attributes["RagioneSociale"].Value) + ". Si vuole sovrascrivere completamente?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            if(mf.DeleteAnagrafica(((cliente_importato.Attributes["RagioneSociale"] == null) ? "" : cliente_importato.Attributes["RagioneSociale"].Value)) == false)
                            {
                                di.Delete(true);
                                return false;
                            }
                            IDCliente = mf.CheckEsistenzaCliente(ht);
                        }
                        else
                        {
                            di.Delete(true);
                            return false;
                        }
                    }
                    else
                    {
                        //no verbose
                        if(mf.DeleteAnagrafica(((cliente_importato.Attributes["RagioneSociale"] == null) ? "" : cliente_importato.Attributes["RagioneSociale"].Value)) == false)
                        {
                            di.Delete(true);
                            return false;
                        }
                        IDCliente = mf.CheckEsistenzaCliente(ht);
                    }

                }
                else
                {
                    mf.InsertClientChild(IDCliente, cliente_importato);

                    
                }

                //Aggiorno associazione bilancio di verifica
                XmlNode associazioni_bilancio = _d.Document.SelectSingleNode("/ROOT/CLIENTE/BilancioVerifica");
                if (associazioni_bilancio != null)
                {
                    mf.SetAnagraficaBV(IDCliente, associazioni_bilancio);
                }

#endregion

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

                //andrea
                FileInfo fnew;

#region INCARICO
                foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/INCARICO"))
                {
                    node.Attributes["Cliente"].Value = IDCliente.ToString();

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    //andrea
                    //string nuovonomefile = node.Attributes["File"].Value.Split('.').First() + "(Copia)." + node.Attributes["File"].Value.Split('.').Last();
                    //node.Attributes["File"].Value = nuovonomefile;
                    //nuovonomefile = node.Attributes["FileData"].Value.Split('.').First() + "(Copia)." + node.Attributes["FileData"].Value.Split('.').Last();
                    //node.Attributes["FileData"].Value = nuovonomefile;

                    string vecchio = node.Attributes["ID"].Value;

                    string nuovo = mf.AddIncarico(node);

                    IncaricoOldNew.Add(vecchio, nuovo);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);


                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                }
#endregion

                //MessageBox.Show("incarico");

#region ISQC
                foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/ISQC"))
                {
                    node.Attributes["Cliente"].Value = IDCliente.ToString();

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }
                    
                    string vecchio = node.Attributes["ID"].Value;

                    string nuovo = mf.AddISQC(node);

                    ISQCOldNew.Add(vecchio, nuovo);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);


                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                }
#endregion

                //MessageBox.Show("isqc");

#region REVISIONE
                foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/REVISIONE"))
                {
                    node.Attributes["Cliente"].Value = IDCliente.ToString();

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    //andrea
                    //string nuovonomefile = node.Attributes["File"].Value.Split('.').First() + "(Copia)." + node.Attributes["File"].Value.Split('.').Last();
                    //node.Attributes["File"].Value = nuovonomefile;
                    //nuovonomefile = node.Attributes["FileData"].Value.Split('.').First() + "(Copia)." + node.Attributes["FileData"].Value.Split('.').Last();
                    //node.Attributes["FileData"].Value = nuovonomefile;

                    string vecchio = node.Attributes["ID"].Value;

                    string nuovo = mf.AddRevisione(node);

                    RevisioneOldNew.Add(vecchio, nuovo);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);

                    XmlDataProviderManager _xaml = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);

                    if (_xaml != null && _xaml.Document.SelectSingleNode("/Dati//Dato[@ID='274']") != null)
                    {
                        foreach (XmlNode tmpnode in _xaml.Document.SelectSingleNode("/Dati//Dato[@ID='274']").SelectNodes("Node[@xaml]"))
                        {
                            FileInfo fxamlhere = new FileInfo(cartellatmp + tmpnode.Attributes["xaml"].Value.Replace("XAML\\", ""));

                            if (!fxamlhere.Exists)
                            {
                                tmpnode.Attributes.Remove(tmpnode.Attributes["xaml"]);
                            }
                            else
                            {
                                DirectoryInfo dixaml = new DirectoryInfo(App.AppDataDataFolder + "\\XAML");

                                if (!dixaml.Exists)
                                {
                                    dixaml.Create();
                                }

                                fxamlhere.CopyTo(App.AppDataDataFolder + "\\XAML\\" + fxamlhere.Name, true);
                            }
                        }

                        _xaml.Save();
                    }
                }
#endregion

                //MessageBox.Show("revisione");

#region BILANCIO
                foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/BILANCIO"))
                {
                    node.Attributes["Cliente"].Value = IDCliente.ToString();

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    //andrea
                    //string nuovonomefile = node.Attributes["File"].Value.Split('.').First() + "(Copia)." + node.Attributes["File"].Value.Split('.').Last();
                    //node.Attributes["File"].Value = nuovonomefile;
                    //nuovonomefile = node.Attributes["FileData"].Value.Split('.').First() + "(Copia)." + node.Attributes["FileData"].Value.Split('.').Last();
                    //node.Attributes["FileData"].Value = nuovonomefile;

                    string vecchio = node.Attributes["ID"].Value;

                    string nuovo = mf.AddBilancio(node);

                    BilancioOldNew.Add(vecchio, nuovo);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (fnew.Exists)
                        fnew.Delete();


                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                }
#endregion

                //MessageBox.Show("bilancio");

#region CONCLUSIONE
                foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/CONCLUSIONE"))
                {
                    node.Attributes["Cliente"].Value = IDCliente.ToString();

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    //andrea
                    //string nuovonomefile = node.Attributes["File"].Value.Split('.').First() + "(Copia)." + node.Attributes["File"].Value.Split('.').Last();
                    //node.Attributes["File"].Value = nuovonomefile;
                    //nuovonomefile = node.Attributes["FileData"].Value.Split('.').First() + "(Copia)." + node.Attributes["FileData"].Value.Split('.').Last();
                    //node.Attributes["FileData"].Value = nuovonomefile;

                    string vecchio = node.Attributes["ID"].Value;

                    string nuovo = mf.AddConclusione(node);

                    ConclusioniOldNew.Add(vecchio, nuovo);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (fnew.Exists)
                        fnew.Delete();


                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                }
#endregion

                //MessageBox.Show("conclusione");

#region FLUSSI
                foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/FLUSSO"))
                {
                    node.Attributes["Cliente"].Value = IDCliente.ToString();

                    mf.AddFlussi(node);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    XmlDataProviderManager _fa = new XmlDataProviderManager(cartellatmp + "\\" + node.Attributes["FileData"].Value, true);

                    string xpath = "//Allegato";
                    string directory = App.AppDocumentiFolder + "\\Flussi";
                    string directorytmp = cartellatmp + "\\Flussi";

                    foreach (XmlNode item in _fa.Document.SelectNodes(xpath))
                    {
                        FileInfo f_fa = new FileInfo(directorytmp + "\\" + item.Attributes["FILE"].Value);

                        if (f_fa.Exists)
                        {
                            DirectoryInfo newdi = new DirectoryInfo(directory);
                            if (newdi.Exists == false)
                            {
                                newdi.Create();
                            }

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

                    _fa.Save();

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                }
#endregion

                //MessageBox.Show("flussi");

#region RELAZIONEB
                foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/RELAZIONEB"))
                {
                    node.Attributes["Cliente"].Value = IDCliente.ToString();

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    string vecchio = node.Attributes["ID"].Value;

                    string nuovo = mf.AddRelazioneB(node);


                    RelazioniBOldNew.Add(vecchio, nuovo);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (fnew.Exists)
                        fnew.Delete();


                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                }
#endregion

                //MessageBox.Show("relaz b");

#region RELAZIONEBC
                foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/RELAZIONEBC"))
                {
                    node.Attributes["Cliente"].Value = IDCliente.ToString();

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    string vecchio = node.Attributes["ID"].Value;

                    string nuovo = mf.AddRelazioneBC(node);


                    RelazioniBCOldNew.Add(vecchio, nuovo);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (fnew.Exists)
                        fnew.Delete();


                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                }
#endregion

                //MessageBox.Show("relaz bc");

#region RELAZIONEV
                foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/RELAZIONEV"))
                {
                    node.Attributes["Cliente"].Value = IDCliente.ToString();

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    string vecchio = node.Attributes["ID"].Value;

                    string nuovo = mf.AddRelazioneV(node);

                    RelazioniVOldNew.Add(vecchio, nuovo);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (fnew.Exists)
                        fnew.Delete();


                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                }
#endregion

                //MessageBox.Show("relaz v");

#region RELAZIONEVC
                foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/RELAZIONEVC"))
                {
                    node.Attributes["Cliente"].Value = IDCliente.ToString();

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    string vecchio = node.Attributes["ID"].Value;

                    string nuovo = mf.AddRelazioneVC(node);

                    RelazioniVCOldNew.Add(vecchio, nuovo);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (fnew.Exists)
                        fnew.Delete();


                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                }
#endregion

                //MessageBox.Show("relaz vc");

#region RELAZIONEBV
                foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/RELAZIONEBV"))
                {
                    node.Attributes["Cliente"].Value = IDCliente.ToString();

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    string vecchio = node.Attributes["ID"].Value;

                    string nuovo = mf.AddRelazioneBV(node);

                    RelazioniBVOldNew.Add(vecchio, nuovo);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (fnew.Exists)
                        fnew.Delete();
                    
                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                }
#endregion

                //MessageBox.Show("relaz bv");

#region VERIFICA
                foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/VERIFICA"))
                {
                    node.Attributes["Cliente"].Value = IDCliente.ToString();

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    //andrea
                    //string nuovonomefile = node.Attributes["File"].Value.Split('.').First() + "(Copia)." + node.Attributes["File"].Value.Split('.').Last();
                    //node.Attributes["File"].Value = nuovonomefile;
                    //nuovonomefile = node.Attributes["FileData"].Value.Split('.').First() + "(Copia)." + node.Attributes["FileData"].Value.Split('.').Last();
                    //node.Attributes["FileData"].Value = nuovonomefile;

                    string vecchio = node.Attributes["ID"].Value;

                    string nuovo = mf.AddVerifica(node);

                    VerificaOldNew.Add(vecchio, nuovo);


                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                }
#endregion

                //MessageBox.Show("verifica");

#region VIGILANZA
                foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/VIGILANZA"))
                {
                    node.Attributes["Cliente"].Value = IDCliente.ToString();

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    //andrea
                    //string nuovonomefile = node.Attributes["File"].Value.Split('.').First() + "(Copia)." + node.Attributes["File"].Value.Split('.').Last();
                    //node.Attributes["File"].Value = nuovonomefile;
                    //nuovonomefile = node.Attributes["FileData"].Value.Split('.').First() + "(Copia)." + node.Attributes["FileData"].Value.Split('.').Last();
                    //node.Attributes["FileData"].Value = nuovonomefile;

                    string vecchio = node.Attributes["ID"].Value;

                    string nuovo = mf.AddVigilanza(node);

                    VigilanzaOldNew.Add(vecchio, nuovo);


                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                }
#endregion

                //MessageBox.Show("vigilanza");

#region PIANIFICAZIONIVERIFICA
                foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/PIANIFICAZIONIVERIFICA"))
                {
                    node.Attributes["Cliente"].Value = IDCliente.ToString();

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    string vecchio = node.Attributes["ID"].Value;

                    string nuovo = mf.AddPianificazioniVerifica(node);

                    PianificazioniVerificaOldNew.Add(vecchio, nuovo);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                }
#endregion

                //MessageBox.Show("pian ve");

#region PIANIFICAZIONIVIGILANZA
                foreach (XmlNode node in _d.Document.SelectNodes("/ROOT/PIANIFICAZIONIVIGILANZA"))
                {
                    node.Attributes["Cliente"].Value = IDCliente.ToString();

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    string vecchio = node.Attributes["ID"].Value;

                    string nuovo = mf.AddPianificazioniVigilanza(node);

                    PianificazioniVigilanzaOldNew.Add(vecchio, nuovo);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["File"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["File"].Value);

                    fi = new FileInfo(cartellatmp + "\\" + node.Attributes["FileData"].Value);

                    //file di destinazione -- ANDREA
                    fnew = new FileInfo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                    if (fnew.Exists)
                        fnew.Delete();

                    fi.CopyTo(App.AppDataDataFolder + "\\" + node.Attributes["FileData"].Value);
                }
#endregion

                //MessageBox.Show("pian vi");

#region DOCUMENTI
                XmlDataProviderManager _dd = new XmlDataProviderManager(App.AppDocumentiDataFile, true);

                bool tobesaved = false;

                foreach (XmlNode documento in _d.Document.SelectNodes("/ROOT/DOCUMENTO"))
                {
                    XmlNode root = _dd.Document.SelectSingleNode("//DOCUMENTI");
                    int newID = Convert.ToInt32(root.Attributes["LastID"].Value) + 1;

                    //file di origine
                    FileInfo ff = new FileInfo(cartellatmp + "\\" + documento.Attributes["File"].Value);

                    if (ff.Exists)
                    {
                        string nomefile = newID.ToString() + "." + documento.Attributes["File"].Value.Split('.').Last();

                        //file di destinazione -- ANDREA
                        fnew = new FileInfo(App.AppDocumentiFolder + "\\" + nomefile);
                        if (fnew.Exists)
                        {
                            fnew.Delete();
                        }


                        ff.CopyTo(App.AppDocumentiFolder + "\\" + nomefile);

                        string trueSessione = (documento.Attributes["Sessione"] == null)? "-1" : documento.Attributes["Sessione"].Value;

                        if (trueSessione != "-1")
                        {
                            App.TipoFile Tree = ((App.TipoFile)(Convert.ToInt32(documento.Attributes["Tree"].Value)));

                            switch (Tree)
                            {
                                case App.TipoFile.Incarico:
                                case App.TipoFile.IncaricoCS:
                                case App.TipoFile.IncaricoSU:
                                case App.TipoFile.IncaricoREV:
                                    if (IncaricoOldNew.Contains(trueSessione))
                                    {
                                        trueSessione = IncaricoOldNew[trueSessione].ToString();
                                    }
                                    break;
                                case App.TipoFile.ISQC:

                                    if (ISQCOldNew.Contains(trueSessione))
                                    {
                                        trueSessione = ISQCOldNew[trueSessione].ToString();
                                    }
                                    break;
                                case App.TipoFile.Revisione:

                                    if (RevisioneOldNew.Contains(trueSessione))
                                    {
                                        trueSessione = RevisioneOldNew[trueSessione].ToString();
                                    }
                                    break;
                                case App.TipoFile.Bilancio:

                                    if (BilancioOldNew.Contains(trueSessione))
                                    {
                                        trueSessione = BilancioOldNew[trueSessione].ToString();
                                    }
                                    break;
                                case App.TipoFile.Conclusione:

                                    if (ConclusioniOldNew.Contains(trueSessione))
                                    {
                                        trueSessione = ConclusioniOldNew[trueSessione].ToString();
                                    }
                                    break;
                                case App.TipoFile.Verifica:

                                    if (VerificaOldNew.Contains(trueSessione))
                                    {
                                        trueSessione = VerificaOldNew[trueSessione].ToString();
                                    }
                                    break;
                                case App.TipoFile.Vigilanza:

                                    if (VigilanzaOldNew.Contains(trueSessione))
                                    {
                                        trueSessione = VigilanzaOldNew[trueSessione].ToString();
                                    }
                                    break;
                                case App.TipoFile.Flussi:
                                    ;
                                    break;
                                case App.TipoFile.PianificazioniVerifica:

                                    if (PianificazioniVerificaOldNew.Contains(trueSessione))
                                    {
                                        trueSessione = PianificazioniVerificaOldNew[trueSessione].ToString();
                                    }
                                    break;
                                case App.TipoFile.PianificazioniVigilanza:

                                    if (PianificazioniVigilanzaOldNew.Contains(trueSessione))
                                    {
                                        trueSessione = PianificazioniVigilanzaOldNew[trueSessione].ToString();
                                    }
                                    break;
                                case App.TipoFile.RelazioneB:
                                    if (RelazioniBOldNew.Contains(trueSessione))
                                    {
                                        trueSessione = RelazioniBOldNew[trueSessione].ToString();
                                    }
                                    break;
                                case App.TipoFile.RelazioneBC:
                                    if (RelazioniBCOldNew.Contains(trueSessione))
                                    {
                                        trueSessione = RelazioniBCOldNew[trueSessione].ToString();
                                    }
                                    break;
                                case App.TipoFile.RelazioneV:
                                    if (RelazioniVOldNew.Contains(trueSessione))
                                    {
                                        trueSessione = RelazioniVOldNew[trueSessione].ToString();
                                    }
                                    break;
                                case App.TipoFile.RelazioneVC:
                                    if (RelazioniVCOldNew.Contains(trueSessione))
                                    {
                                        trueSessione = RelazioniVCOldNew[trueSessione].ToString();
                                    }
                                    break;
                                case App.TipoFile.RelazioneBV:
                                    if (RelazioniBVOldNew.Contains(trueSessione))
                                    {
                                        trueSessione = RelazioniBVOldNew[trueSessione].ToString();
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        //switch (((App.TipoAttivita)(Convert.ToInt32(documento.Attributes["Tree"].Value))))
                        //{
                        //    case App.TipoAttivita.Incarico:
                        //        IDTree = (Convert.ToInt32(App.TipoFile.Incarico)).ToString();
                        //        break;
                        //    case App.TipoAttivita.Revisione:
                        //        IDTree = (Convert.ToInt32(App.TipoFile.Revisione)).ToString();
                        //        break;
                        //    case App.TipoAttivita.Bilancio:
                        //        IDTree = (Convert.ToInt32(App.TipoFile.Bilancio)).ToString();
                        //        break;
                        //    case App.TipoAttivita.Verifica:
                        //        IDTree = (Convert.ToInt32(App.TipoFile.Verifica)).ToString();
                        //        break;
                        //    case App.TipoAttivita.Sconosciuto:
                        //    default:
                        //        break;
                        //}

                        string xml = "<DOCUMENTO ID=\"" + newID.ToString() + "\" Cliente=\"" + IDCliente + "\" Sessione=\"" + trueSessione + "\" Tree=\"" + ((documento.Attributes["Tree"] == null)? "" : documento.Attributes["Tree"].Value) + "\" Nodo=\"" + ((documento.Attributes["Nodo"] == null) ? "" : documento.Attributes["Nodo"].Value).Replace("&", "&amp;").Replace("\"", "'") + "\" Tipo=\"" + ((documento.Attributes["Tipo"] == null) ? "" : documento.Attributes["Tipo"].Value) + "\" Titolo=\"" + ((documento.Attributes["Titolo"] == null) ? "" : documento.Attributes["Titolo"].Value).ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Descrizione=\"" + ((documento.Attributes["Descrizione"] == null) ? "" : documento.Attributes["Descrizione"].Value).ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" File=\"" + nomefile + "\" Visualizza=\"True\" />";

                        XmlDocument doctmp = new XmlDocument();
                        doctmp.LoadXml(xml);
                        XmlNode tmpNode = doctmp.SelectSingleNode("/DOCUMENTO");

                        XmlNode node = _dd.Document.ImportNode(tmpNode, true);

                        root.AppendChild(node);
                        root.Attributes["LastID"].Value = newID.ToString();

                        tobesaved = true;
                    }
                }

                if (tobesaved)
                {
                    _dd.Save();
                }
#endregion

                //MessageBox.Show("doc");

                _d.Save();
                di.Delete(true);

                mf.SplitVerificheVigilanze();

                //MessageBox.Show("split");
                mf.UpdateTipoEsercisioSu239();
                return true;
            }
            catch (Exception ex)
            {
                string log = ex.Message;
                return false;
            }
#endif
        }
	}
}
