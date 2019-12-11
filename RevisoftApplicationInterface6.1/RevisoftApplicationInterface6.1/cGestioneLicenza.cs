using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows;
using System.Collections;
using RevisoftApplication.it.revisoft.ws;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;

namespace RevisoftApplication
{
    class GestioneLicenza
    {
        //Costanti
        private const int PERIODO_PROVA = 10;
        private const int AVVISO_GIORNI_ALLA_SCADENZA_PROVA = 2;
        private const string INTESTATARIO_PROVA = "Intestatario di prova";
        private const string UTENTE_PROVA = "Utente di prova";
        private const string CODICE_MACCHINA_PRIMORDIALE = "MADE_IN_DATALABOR";
        private const string DATA_PRIMORDIALE = "01/01/2000";
        private const string DATAORA_PRIMORDIALE = "01/01/2000 00:00:00";
        private const int DURATA_LICENZA = 365;
        private const int AVVISO_GIORNI_ALLA_SCADENZA = 30;
        public const int TOT_ANAGRAFICHE_LICENZA_ENTRY = 1;
        public const int TOT_ANAGRAFICHE_LICENZA_GUEST = 1;


        //variabili: info app
        private static DateTime _Info_DataInstallazione;
        private static DateTime _Info_DataCreazioneLicenzaProva;
        private static DateTime _Info_DataAttivazioneLicenza;
        private static DateTime _Info_DataScadenzaLicenza;
        private static DateTime _Info_DataUltimoUtilizzo;

        private static string _Info_CodiceMacchina;
        private static string _Info_CodiceMacchinaServer;

        private static bool _Info_Benvenuto;
        private static bool _Info_IstruzioniAutomatiche;
        private static bool _Info_AlertSuCompletato;        
        private static string _Info_TipoGestioneArchivio;
        private static string _Info_PathArchivioRemoto;
        private static string _Info_TipoGestioneBackup;
        private static string _Info_PathBackup;
        private static bool _Info_AddioBackupRevisoft;
        private static bool _Info_AddioBackupUtente;


        //variabili: Licenza
        private static string _Intestatario;
        private static string _Utente;

        private static bool _Multilicenza;
        private static bool _Sigillo;
        private static bool _LicenzaProva;
        private static bool _Guest;
        private static bool _Server;
        private static bool _Cloud;
        private static bool _RemoteDesktop;
        private static bool _Client;

        private static int _NumeroLicenze;
        private static int _NumeroAnagrafiche;

        private static bool _LicenzaProvaDisponibile;
        private static bool _ForzaAttivazioneLicenzaProva;
        private static bool _LicenzaDisponibile;

        private static DateTime _Scadenza;
        private static bool _ScadenzaVicina;
        private static int _GiorniUtilizzati;
        private static int _GiorniAllaScadenza;
        private static int _GiorniUtilizzatiPercentuale;
        private static bool _StatoLicenza;


        private static string _CodiceMacchina;
        private static string _CodiceMacchinaServer;
        private static DateTime _DataInstallazione;
        private static DateTime _DataCreazioneLicenzaProva;
        private static DateTime _DataAttivazioneLicenza;
        private static DateTime _DataScadenzaLicenza;
        private static int _DurataLicenza;
        private static DateTime _DataUltimoUtilizzo;

        //private static App.TipologieLicenze     _TipoLicenza;
        //private static bool         _StatoLicenzaCambiato;

        public Hashtable DatiMultiLicenza = new Hashtable();
        private static bool _StatoLicenzaMultipla;




        //Proprietà: Info app
        public DateTime Info_DataInstallazione
        {
            get { return (DateTime)_Info_DataInstallazione; }
        }
        public DateTime Info_DataCreazioneLicenzaProva
        {
            get { return (DateTime)_Info_DataCreazioneLicenzaProva; }
        }
        public DateTime Info_DataAttivazioneLicenza
        {
            get { return (DateTime)_Info_DataAttivazioneLicenza; }
        }
        public DateTime Info_DataScadenzaLicenza
        {
            get { return (DateTime)_Info_DataScadenzaLicenza; }
        }
        public DateTime Info_DataUltimoUtilizzo
        {
            get { return (DateTime)_Info_DataUltimoUtilizzo; }
        }
        public string Info_CodiceMacchina
        {
            get { return (string)(_Info_CodiceMacchina); }
        }



        //Proprietà: Licenza
        public string Intestatario
        {
            get { return (string)_Intestatario; }
        }
        public string Utente
        {
            get { return (string)_Utente; }
        }
        public bool LicenzaProvaDisponibile
        {
            get { return (bool)_LicenzaProvaDisponibile; }
            set { _LicenzaProvaDisponibile = value; }
        }
        public bool LicenzaDisponibile
        {
            get { return (bool)_LicenzaDisponibile; }
        }
        public bool ScadenzaVicina
        {
            get { return (bool)_ScadenzaVicina; }
        }
        public int DurataLicenza
        {
            get { return (int)_DurataLicenza; }
        }
        public int GiorniUtilizzati
        {
            get { return (int)_GiorniUtilizzati; }
        }
        public int GiorniUtilizzatiPercentuale
        {
            get { return (int)_GiorniUtilizzatiPercentuale; }
        }
        public int GiorniAllaScadenza
        {
            get { return (int)_GiorniAllaScadenza; }
        }
        public bool StatoLicenza
        {
            get { return (bool)_StatoLicenza; }
        }
        //public bool StatoLicenzaCambiato
        //{
        //    get { return (bool)_StatoLicenzaCambiato; }
        //}
        public string CodiceMacchina
        {
            get { return (string)(_CodiceMacchina.Split('-')[0]); }
        }
        public string CodiceMacchinaServer
        {
            get { return (string)(_CodiceMacchinaServer.Split('-')[0]); }
        }
        //public bool StatoLicenzaSigillo
        //{
        //    get { return (bool)_StatoLicenzaSigillo; }
        //}
        public bool StatoLicenzaMultipla
        {
            get { return (bool)_StatoLicenzaMultipla; }
        }

        //Proprietà: Licenza DATE
        public DateTime DataInstallazione
        {
            get { return (DateTime)_DataInstallazione; }
        }
        public DateTime DataCreazioneLicenzaProva
        {
            get { return (DateTime)_Info_DataCreazioneLicenzaProva; }
        }
        public DateTime DataAttivazioneLicenza
        {
            get { return (DateTime)_Info_DataAttivazioneLicenza; }
        }
        public DateTime DataScadenzaLicenza
        {
            get { return (DateTime)_Info_DataScadenzaLicenza; }
        }
        public DateTime DataUltimoUtilizzo
        {
            get { return (DateTime)_Info_DataUltimoUtilizzo; }
        }








        //Metodi
        public void Inizializza()
        {
      RevisoftApplication.Utilities u = new Utilities();
#if (DBG_TEST)
    _Info_DataInstallazione = new DateTime(2018, 10, 17, 0, 0, 0);
    _Info_DataCreazioneLicenzaProva = _Info_DataInstallazione;
    _Info_DataAttivazioneLicenza = _Info_DataInstallazione;
    _DataUltimoUtilizzo = DateTime.Now;
    //_Info_CodiceMacchina = "DUMMY-LICENCE-EB";
    _Info_CodiceMacchina = "RDP license-"+Environment.MachineName;
    _Info_CodiceMacchinaServer = _Info_CodiceMacchina;
    _Info_Benvenuto = true;
    //_Intestatario = "DUMMY"; _Utente = "DUMMY";
    _Intestatario = Environment.UserName; _Utente = _Intestatario;
    _NumeroAnagrafiche = 10000;

    _StatoLicenza = true;
    _StatoLicenzaMultipla = false;
    _LicenzaProvaDisponibile = true;
    _LicenzaDisponibile = true;
    _Scadenza = Convert.ToDateTime(DateTime.Now + new TimeSpan(365, 0, 0, 0));
    _Info_DataScadenzaLicenza = _Scadenza;
    _CodiceMacchina = _Info_CodiceMacchina;
    _CodiceMacchinaServer = _Info_CodiceMacchina;
    App.NumeroanAgrafiche = 10000;
    App.CodiceMacchinaServer = _Info_CodiceMacchina;
    App.CodiceMacchina = _Info_CodiceMacchina;
    App.AppSetupBenvenuto = true;

    App.AppConsentiImportaEsporta = true;
    App.AppConsentiCreazioneAnagrafica = true;
    App.AppConsentiAccessoArchivioLocale = true;
    App.AppConsentiAccessoArchivioRemoto = true;
    App.AppConsentiBackUp = true;
    App.AppConsentiGestioneBackUp = true;

    u.ConfiguraApplicazione();
    return;
#endif
            //set
            //_TipoLicenza = App.TipologieLicenze.Ignota;
            //_TipoLicenzaSigillo = App.TipologieLicenze.Ignota;
            //_Info_TipoLicenza = App.TipologieLicenze.Ignota;
#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
            _StatoLicenza = false;
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
      _StatoLicenzaMultipla = false;
            _LicenzaProvaDisponibile = true;
            _LicenzaDisponibile = true;
            _Scadenza = Convert.ToDateTime(DateTime.Now);

            //Migrazione Info file dalla 4.4 alla 4.5
            MigrazioneInfoFile();

            //4.6 Eventuale aggiornamento Info file
            AggiornaInfoFile();

            //Info Revisoft
            if (!LeggiInfoRevisoft())
                return;

            //Info macchina
            //RevisoftApplication.Utilities u = new Utilities();
            u.LeggiInfoMacchina();

            //Info Licenza di prova
            VerificaAttivazioneLicenzaProva();

            //Prima attivazione, forzo creazione licenza di prova
            if (_LicenzaProvaDisponibile && _ForzaAttivazioneLicenzaProva)
                return;

            //Migrazione licenza dalla 4.4 alla 4.5
            MigrazioneLicenza();


            //Dati licenza
            LeggiDatiLicenza();

            //Verifica licenza
            VerificaLicenza();

            //andrea disattivata provvisoriamente 4.5
            //Dati licenza MULTIPLA
            //LeggiMultilicenza();

            //Configura licenza
            ConfiguraLicenza();

            //Leggi configurazione da registro
            u.ConfiguraApplicazione();
        }





        //INFO APPLICAZIONE *******************************************************************************************************
        /*  con installazione copio file info vuoto, se manca esco e non consento utilizzo sw
        *  quando attivo licenza aggiorno dati info
        *  uso interno
        */
        //creo file INFO primordiale
        private void InizializzaInfoRevisoft()
        {
            //configurazione ai valori iniziali
            _DataInstallazione = Convert.ToDateTime(DATA_PRIMORDIALE);
            _DataCreazioneLicenzaProva = Convert.ToDateTime(DATA_PRIMORDIALE);
            _DataAttivazioneLicenza = Convert.ToDateTime(DATA_PRIMORDIALE);
            _DataScadenzaLicenza = Convert.ToDateTime(DATA_PRIMORDIALE);
            _DataUltimoUtilizzo = Convert.ToDateTime(DATA_PRIMORDIALE);
            _CodiceMacchina = CODICE_MACCHINA_PRIMORDIALE;
            _CodiceMacchinaServer = CODICE_MACCHINA_PRIMORDIALE;
            //creo file
            CreaInfoRevisoft();
        }

        private void CreaInfoRevisoft()
        {
            //xml
            string s = "";
            s += "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
            s += "<RevisoftInfoFile>";
            s += "   <DataInstallazione>" + _DataInstallazione + "</DataInstallazione>";
            s += "   <DataCreazioneLicenzaProva>" + _DataCreazioneLicenzaProva + "</DataCreazioneLicenzaProva>";
            s += "   <DataAttivazioneLicenza>" + _DataAttivazioneLicenza + "</DataAttivazioneLicenza>";
            s += "   <DataScadenzaLicenza>" + _DataScadenzaLicenza + "</DataScadenzaLicenza>";
            s += "   <DataUltimoUtilizzo>" + _DataUltimoUtilizzo + "</DataUltimoUtilizzo>";
            s += "   <CodiceMacchina>" + _CodiceMacchina.Split('-')[0] + "</CodiceMacchina>";
            s += "   <CodiceMacchinaServer>" + _CodiceMacchinaServer.Split('-')[0] + "</CodiceMacchinaServer>";
            //4.5
            s += "   <Benvenuto>True</Benvenuto>";
            s += "   <IstruzioniAutomatiche>True</IstruzioniAutomatiche>";
            s += "   <TipoGestioneArchivio>" + App.TipoGestioneArchivio.Locale.ToString() + "</TipoGestioneArchivio>";
            s += "   <PathArchivioRemoto></PathArchivioRemoto>";
            //4.6
            s += "   <BackupPersonalizzato>False</BackupPersonalizzato>";
            s += "   <PathBackupUtente></PathBackupUtente>";
            s += "   <AddioBackupRevisoft>True</AddioBackupRevisoft>";
            s += "   <AddioBackupUtente>False</AddioBackupUtente>";
            s += "</RevisoftInfoFile>";
            //file
            SalvaInfoRevisoft(s);
        }

        public void ResetInfoRevisoft()
        {
            //configurazione ai valori iniziali
            InizializzaInfoRevisoft();
            //cancello file licenza
            if (File.Exists(App.AppLicenseFile))
                File.Delete(App.AppLicenseFile);

            if (File.Exists(App.AppLicenseFile_OLD))
                File.Delete(App.AppLicenseFile_OLD);

            //rimuovo chiave registro licenza prova
            Utilities u = new Utilities();
            u.ConfiguraRegistroAttivazioneLicenzaProvaReset();
            return;
        }
        //***************************************************************************************************************************


        private void SalvaInfoRevisoft(string s)
        {
            //controllo presenza file
            if (File.Exists(App.AppInfoFile))
            {
                File.Delete(App.AppInfoFile);
            }
            //salvo dati
            RevisoftApplication.XmlManager x = new XmlManager();
            x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Avanzata;
            x.SaveEncodedFile(App.AppInfoFile, s);
        }

        public void SalvaInfoDataUltimoUtilizzo()
        {
            _DataUltimoUtilizzo = DateTime.Now;

            string s = "";
            s += "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
            s += "<RevisoftInfoFile>";
            s += "   <DataInstallazione>" + _Info_DataInstallazione + "</DataInstallazione>";
            s += "   <DataCreazioneLicenzaProva>" + _Info_DataCreazioneLicenzaProva + "</DataCreazioneLicenzaProva>";
            s += "   <DataAttivazioneLicenza>" + _Info_DataAttivazioneLicenza + "</DataAttivazioneLicenza>";
            s += "   <DataScadenzaLicenza>" + _Info_DataScadenzaLicenza + "</DataScadenzaLicenza>";
            s += "   <DataUltimoUtilizzo>" + _DataUltimoUtilizzo + "</DataUltimoUtilizzo>";                                 //modificiato
            s += "   <CodiceMacchina>" + _Info_CodiceMacchina.Split('-')[0] + "</CodiceMacchina>";
            s += "   <CodiceMacchinaServer>" + _Info_CodiceMacchinaServer.Split('-')[0] + "</CodiceMacchinaServer>";
            //4.5
            s += "   <Benvenuto>" + App.AppSetupBenvenuto + "</Benvenuto>";
            s += "   <IstruzioniAutomatiche>" + App.AppSetupIstruzioniAutomatiche + "</IstruzioniAutomatiche>";
            s += "   <AlertSuCompletato>" + App.AppSetupAlertSuCompletato + "</AlertSuCompletato>";            
            s += "   <TipoGestioneArchivio>" + App.AppSetupTipoGestioneArchivio + "</TipoGestioneArchivio>";
            s += "   <PathArchivioRemoto>" + App.AppPathArchivioRemoto + "</PathArchivioRemoto>";
            //4.6
            s += "   <BackupPersonalizzato>" + App.AppSetupBackupPersonalizzato + "</BackupPersonalizzato>";
            s += "   <PathBackupUtente>" + App.AppUserBackupFolder + "</PathBackupUtente>";
            s += "   <AddioBackupRevisoft>" + App.AppSetupAddioBackupRevisoft + "</AddioBackupRevisoft>";
            s += "   <AddioBackupUtente>" + App.AppSetupAddioBackupUtente + "</AddioBackupUtente>";
            s += "</RevisoftInfoFile>";
            //salvo file
            SalvaInfoRevisoft(s);
        }

        public string GetFromInfo(string element)
        {
            string returnstring = "";

            if (!File.Exists(App.AppInfoFile))
            {
                if (!File.Exists(App.AppInfoFile_OLD))
                {
                    App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                    RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                    m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.MancaFileInfo;
                    m.VisualizzaMessaggio();
                    return "";
                }
                else
                {
                    FileInfo fi = new FileInfo(App.AppInfoFile_OLD);
                    fi.CopyTo(App.AppInfoFile, true);
                }
            }

            RevisoftApplication.XmlManager x = new XmlManager();
            XmlDocument f = new XmlDocument();
            x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Avanzata;
            f = x.LoadEncodedFile(App.AppInfoFile);

            XmlNode n = f.SelectSingleNode("/RevisoftInfoFile/" + element);
            if (n != null)
            {
                returnstring = n.InnerText;
            }

            return returnstring;
        }

        public string SetFromInfo(string element, string value)
        {
            string returnstring = "";

            if (!File.Exists(App.AppInfoFile))
            {
                if (!File.Exists(App.AppInfoFile_OLD))
                {
                    App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                    RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                    m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.MancaFileInfo;
                    m.VisualizzaMessaggio();
                    return "";
                }
                else
                {
                    FileInfo fi = new FileInfo(App.AppInfoFile_OLD);
                    fi.CopyTo(App.AppInfoFile, true);
                }
            }

            RevisoftApplication.XmlManager x = new XmlManager();
            XmlDocument f = new XmlDocument();
            x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Avanzata;
            f = x.LoadEncodedFile(App.AppInfoFile);

            XmlNode n = f.SelectSingleNode("/RevisoftInfoFile/" + element);
            if (n == null)
            {
                XmlNode xn = f.CreateNode(XmlNodeType.Element, element, "");
                f.SelectSingleNode("/RevisoftInfoFile").AppendChild(xn);

                n = f.SelectSingleNode("/RevisoftInfoFile/" + element);

                n.InnerText = value;
            }

            n.InnerText = value;

            x.SaveEncodedFile(App.AppInfoFile, f.OuterXml);

            returnstring = n.InnerText;

            return returnstring;
        }


        private void MigrazioneInfoFile()
        {
            //Verifico presenza nuovo info file
            if (File.Exists(App.AppInfoFile))
                return;

            //Verifico presenza vecchio info file
            if (!File.Exists(App.AppInfoFile_OLD))
                return;

            //verifico se admin
            Utilities u = new Utilities();
            if (!u.IsAdministrator())
            {
                App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckDirittiAdmin;
                m.VisualizzaMessaggio();
            }



            //copio vecchio info file in appdata
            FileInfo fi = new FileInfo(App.AppInfoFile_OLD);
            fi.CopyTo(App.AppInfoFile, true);

            //leggo info file
            try
            {
                //leggo file info
                RevisoftApplication.XmlManager x = new XmlManager();
                XmlDocument f = new XmlDocument();
                x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Avanzata;
                f = x.LoadEncodedFile(App.AppInfoFile);
                //DataInstallazione
                XmlNode n = f.SelectSingleNode("/RevisoftInfoFile/DataInstallazione");
                _Info_DataInstallazione = Convert.ToDateTime(n.InnerText);
                //DataCreazioneLicenzaProva
                n = f.SelectSingleNode("/RevisoftInfoFile/DataCreazioneLicenzaProva");
                _Info_DataCreazioneLicenzaProva = Convert.ToDateTime(n.InnerText);
                //DataAttivazioneLicenza
                n = f.SelectSingleNode("/RevisoftInfoFile/DataAttivazioneLicenza");
                _Info_DataAttivazioneLicenza = Convert.ToDateTime(n.InnerText);
                //DataScadenzaLicenza
                n = f.SelectSingleNode("/RevisoftInfoFile/DataScadenzaLicenza");
                _Info_DataScadenzaLicenza = Convert.ToDateTime(n.InnerText);
                //DataUltimoUtilizzo
                n = f.SelectSingleNode("/RevisoftInfoFile/DataUltimoUtilizzo");
                _Info_DataUltimoUtilizzo = Convert.ToDateTime(n.InnerText);
                //CodiceMacchina
                n = f.SelectSingleNode("/RevisoftInfoFile/CodiceMacchina");
                _Info_CodiceMacchina = n.InnerText.Split('-')[0];
                //CodiceMacchinaServer
                n = f.SelectSingleNode("/RevisoftInfoFile/CodiceMacchinaServer");
                _Info_CodiceMacchinaServer = n.InnerText.Split('-')[0];

                //4.5 - Aggiungo nuovi campi
                XmlNode root = f.SelectSingleNode("/RevisoftInfoFile");


                //// lettura registro


                //Benvenuto
                XmlNode e = f.CreateNode(XmlNodeType.Element, "Benvenuto", "");
                _Info_Benvenuto = Convert.ToBoolean(u.GetRegistroChiaveApplicazione(App.Registry_Benvenuto.ToString()));
                e.InnerText = _Info_Benvenuto.ToString();
                root.AppendChild(e);
                //IstruzioniAutomatiche
                e = f.CreateNode(XmlNodeType.Element, "IstruzioniAutomatiche", "");
                _Info_IstruzioniAutomatiche = Convert.ToBoolean(u.GetRegistroChiaveApplicazione(App.Registry_IstruzioniAutomatiche.ToString()));
                e.InnerText = _Info_IstruzioniAutomatiche.ToString();
                root.AppendChild(e);

                e = f.CreateNode(XmlNodeType.Element, "AlertSuCompletato", "");
                _Info_AlertSuCompletato = Convert.ToBoolean(u.GetRegistroChiaveApplicazione(App.Registry_AlertSuCompletato.ToString()));
                e.InnerText = _Info_AlertSuCompletato.ToString();
                root.AppendChild(e);
                


                //TipoGestioneArchivio
                e = f.CreateNode(XmlNodeType.Element, "TipoGestioneArchivio", "");
                _Info_TipoGestioneArchivio = u.GetRegistroChiaveApplicazione(App.Registry_TipoGestioneArchivio);
                e.InnerText = _Info_TipoGestioneArchivio;
                root.AppendChild(e);
                //TipoGestioneArchivio
                e = f.CreateNode(XmlNodeType.Element, "PathArchivioRemoto", "");
                _Info_PathArchivioRemoto = u.GetRegistroChiaveApplicazione(App.Registry_PathArchivioRemoto);
                e.InnerText = _Info_PathArchivioRemoto;
                root.AppendChild(e);

                //salvo nuovo info file
                x.SaveEncodedFile(App.AppInfoFile, f.OuterXml);

            }
            catch (Exception)
            {
                App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.ErroreInFileInfo;
                m.VisualizzaMessaggio();
            }

        }

        private void AggiornaInfoFile()
        {
            //leggo info file
            try
            {
                //leggo file info
                RevisoftApplication.XmlManager x = new XmlManager();
                XmlDocument f = new XmlDocument();
                x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Avanzata;
                f = x.LoadEncodedFile(App.AppInfoFile);
                //DataInstallazione
                XmlNode n = f.SelectSingleNode("/RevisoftInfoFile/BackupPersonalizzato");
                //4.6 integro nuovo campi
                if (f.SelectSingleNode("/RevisoftInfoFile/BackupPersonalizzato") == null)
                {
                    //setto var di ambiente
                    App.AppSetupBackupPersonalizzato = false;
                    App.AppUserBackupFolder = "";
                    App.AppSetupAddioBackupRevisoft = true;
                    App.AppSetupAddioBackupUtente = false;

                    XmlNode root = f.SelectSingleNode("/RevisoftInfoFile");
                    //BackupPersonalizzato
                    XmlNode e = f.CreateNode(XmlNodeType.Element, "BackupPersonalizzato", "");
                    e.InnerText = App.AppSetupBackupPersonalizzato.ToString();
                    root.AppendChild(e);
                }

                if (f.SelectSingleNode("/RevisoftInfoFile/PathBackupUtente") == null)
                {
                    App.AppSetupBackupPersonalizzato = false;
                    App.AppUserBackupFolder = "";
                    App.AppSetupAddioBackupRevisoft = true;
                    App.AppSetupAddioBackupUtente = false;

                    XmlNode root = f.SelectSingleNode("/RevisoftInfoFile");
                    //Path BackupPersonalizzato
                    XmlNode e = f.CreateNode(XmlNodeType.Element, "PathBackupUtente", "");
                    e.InnerText = App.AppUserBackupFolder;
                    root.AppendChild(e);
                }

                if (f.SelectSingleNode("/RevisoftInfoFile/AddioBackupRevisoft") == null)
                {
                    App.AppSetupBackupPersonalizzato = false;
                    App.AppUserBackupFolder = "";
                    App.AppSetupAddioBackupRevisoft = true;
                    App.AppSetupAddioBackupUtente = false;

                    XmlNode root = f.SelectSingleNode("/RevisoftInfoFile");
                    //Addio backup Revisoft
                    XmlNode e = f.CreateNode(XmlNodeType.Element, "AddioBackupRevisoft", "");
                    e.InnerText = "True";
                    root.AppendChild(e);
                }

                if (f.SelectSingleNode("/RevisoftInfoFile/AddioBackupUtente") == null)
                {
                    App.AppSetupBackupPersonalizzato = false;
                    App.AppUserBackupFolder = "";
                    App.AppSetupAddioBackupRevisoft = true;
                    App.AppSetupAddioBackupUtente = false;

                    XmlNode root = f.SelectSingleNode("/RevisoftInfoFile");
                    //Addio backup personalizzato
                    XmlNode e = f.CreateNode(XmlNodeType.Element, "AddioBackupUtente", "");
                    e.InnerText = "False";
                    root.AppendChild(e);
                }

                //salvo nuovo info file
                x.SaveEncodedFile(App.AppInfoFile, f.OuterXml);

            }
            catch (Exception)
            {
                App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.ErroreInFileInfo;
                m.VisualizzaMessaggio();
            }
        }


        private bool LeggiInfoRevisoft()
        {
            //controllo presenza file: se manca esco
            if (!File.Exists(App.AppInfoFile))
            {
                App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.MancaFileInfo;
                m.VisualizzaMessaggio();
                return false;
            }

            try
            {
                //leggo file info
                RevisoftApplication.XmlManager x = new XmlManager();
                XmlDocument f = new XmlDocument();
                x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Avanzata;
                f = x.LoadEncodedFile(App.AppInfoFile);
                //DataInstallazione
                XmlNode n = f.SelectSingleNode("/RevisoftInfoFile/DataInstallazione");
                _Info_DataInstallazione = Convert.ToDateTime(n.InnerText);
                //DataCreazioneLicenzaProva
                n = f.SelectSingleNode("/RevisoftInfoFile/DataCreazioneLicenzaProva");
                _Info_DataCreazioneLicenzaProva = Convert.ToDateTime(n.InnerText);
                //DataAttivazioneLicenza
                n = f.SelectSingleNode("/RevisoftInfoFile/DataAttivazioneLicenza");
                _Info_DataAttivazioneLicenza = Convert.ToDateTime(n.InnerText);
                //DataScadenzaLicenza
                n = f.SelectSingleNode("/RevisoftInfoFile/DataScadenzaLicenza");
                _Info_DataScadenzaLicenza = Convert.ToDateTime(n.InnerText);
                //DataUltimoUtilizzo
                n = f.SelectSingleNode("/RevisoftInfoFile/DataUltimoUtilizzo");
                _Info_DataUltimoUtilizzo = Convert.ToDateTime(n.InnerText);
                //CodiceMacchina
                n = f.SelectSingleNode("/RevisoftInfoFile/CodiceMacchina");
                _Info_CodiceMacchina = n.InnerText.Split('-')[0];
                //CodiceMacchinaServer
                n = f.SelectSingleNode("/RevisoftInfoFile/CodiceMacchinaServer");
                _Info_CodiceMacchinaServer = n.InnerText.Split('-')[0];
                //4.5
                //Benvenuto
                n = f.SelectSingleNode("/RevisoftInfoFile/Benvenuto");
                _Info_Benvenuto = Convert.ToBoolean(n.InnerText);
                //IstruzioniAutomatiche
                n = f.SelectSingleNode("/RevisoftInfoFile/IstruzioniAutomatiche");
                _Info_IstruzioniAutomatiche = Convert.ToBoolean(n.InnerText);
                //Alert su completato
                n = f.SelectSingleNode("/RevisoftInfoFile/AlertSuCompletato");
                if(n != null)
                {
                    _Info_AlertSuCompletato = Convert.ToBoolean(n.InnerText);
                }
                else
                {
                    _Info_AlertSuCompletato = true;
                }
                //TipoGestioneArchivio
                n = f.SelectSingleNode("/RevisoftInfoFile/TipoGestioneArchivio");
                _Info_TipoGestioneArchivio = n.InnerText;
                //TipoGestioneArchivio
                n = f.SelectSingleNode("/RevisoftInfoFile/PathArchivioRemoto");
                _Info_PathArchivioRemoto = n.InnerText;

                //4.6 TipoGestioneBackup
                n = f.SelectSingleNode("/RevisoftInfoFile/BackupPersonalizzato");
                _Info_TipoGestioneBackup = n.InnerText;
                //Perecorso Backup personalizzato
                n = f.SelectSingleNode("/RevisoftInfoFile/PathBackupUtente");
                _Info_PathBackup = n.InnerText;
                //Addio Backup Revisoft
                n = f.SelectSingleNode("/RevisoftInfoFile/AddioBackupRevisoft");
                _Info_AddioBackupRevisoft = Convert.ToBoolean(n.InnerText);
                //Addio Backup personalizzato
                n = f.SelectSingleNode("/RevisoftInfoFile/AddioBackupUtente");
                _Info_AddioBackupUtente = Convert.ToBoolean(n.InnerText);


                //configuro variabili d'ambiente
                Utilities u = new Utilities();
                App.AppSetupBenvenuto = _Info_Benvenuto;
                App.AppSetupIstruzioniAutomatiche = _Info_IstruzioniAutomatiche;
                App.AppSetupAlertSuCompletato = _Info_AlertSuCompletato;
                App.AppSetupTipoGestioneArchivio = u.TipologiaArchivio(_Info_TipoGestioneArchivio);
                //App.AppPathArchivioRemoto = _Info_PathArchivioRemoto;
                //4.6 percorso backup personalizzato
                if (_Info_TipoGestioneBackup != null)
                {
                    App.AppSetupBackupPersonalizzato = Convert.ToBoolean(_Info_TipoGestioneBackup);
                    //verifico percorso
                    if (App.AppSetupBackupPersonalizzato)
                    {
                        if (_Info_PathBackup.Trim() != "")
                            App.AppUserBackupFolder = _Info_PathBackup;
                        else
                            App.AppSetupBackupPersonalizzato = false;
                    }

                }
                App.AppSetupAddioBackupRevisoft = _Info_AddioBackupRevisoft;
                App.AppSetupAddioBackupUtente = _Info_AddioBackupUtente;

                return true;
            }
            catch (Exception)
            {
                App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.ErroreInFileInfo;
                m.VisualizzaMessaggio();
                return false;
            }

        }

        //LICENZA DI PROVA ********************************************************************************************************

        public void VerificaAttivazioneLicenzaProva()
        {
            //string setupData = "";
            //setupData = GetFromInfo("DataCreazioneLicenzaProva");

            //verifico valore, se prima installazione già effettuata non consento attivazione licenza di prova
            if (_Info_DataCreazioneLicenzaProva == Convert.ToDateTime(DATAORA_PRIMORDIALE))
                _LicenzaProvaDisponibile = true;
            else
                _LicenzaProvaDisponibile = false;

            if (!File.Exists(App.AppLicenseFile) && !File.Exists(App.AppLicenseFile_OLD))
                _ForzaAttivazioneLicenzaProva = true;
        }

        public void AttivaLicenzaProva()
        {
            //setup
            DateTime Adesso = DateTime.Now;
            _Intestatario = INTESTATARIO_PROVA;
            _Utente = UTENTE_PROVA;
            //_TipoLicenza = App.TipologieLicenze.Prova;
            _DataInstallazione = Adesso;
            _DataCreazioneLicenzaProva = Adesso;
            _Scadenza = Convert.ToDateTime(Adesso.AddDays(PERIODO_PROVA));
            _DataUltimoUtilizzo = Adesso;
            //_CodiceMacchina = App.CodiceMacchina.Split('-')[0];
            //_CodiceMacchinaServer = App.CodiceMacchina.Split('-')[0];
            _CodiceMacchina = App.CodiceMacchina;
            _CodiceMacchinaServer = App.CodiceMacchina;
            _GiorniUtilizzati = 1;
            _GiorniAllaScadenza = PERIODO_PROVA;
            _DurataLicenza = PERIODO_PROVA;

            //LICENZA
            string s = "";
            s += "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
            s += "<RevisoftLicenseFile>";
            s += "   <Intestatario>" + _Intestatario + "</Intestatario>";
            s += "   <Utente>" + _Utente + "</Utente>";
            s += "   <DataAttivazioneLicenza>" + Convert.ToDateTime(_DataCreazioneLicenzaProva) + "</DataAttivazioneLicenza>";
            s += "   <DataScadenzaLicenza>" + Convert.ToDateTime(_Scadenza) + "</DataScadenzaLicenza>";
            s += "   <DurataLicenza>" + PERIODO_PROVA + "</DurataLicenza>";
            s += "   <CodiceMacchina>" + _CodiceMacchina.Split('-')[0] + "</CodiceMacchina>";
            s += "   <CodiceMacchinaServer>" + _CodiceMacchinaServer.Split('-')[0] + "</CodiceMacchinaServer>";
            //4.5
            s += "   <Multilicenza>False</Multilicenza>";
            s += "   <Sigillo>False</Sigillo>";
            s += "   <LicenzaProva>True</LicenzaProva>";
            s += "   <Guest>False</Guest>";
            s += "   <Server>False</Server>";
            s += "   <Cloud>False</Cloud>";
            s += "   <RemoteDesktop>False</RemoteDesktop>";
            s += "   <Client>False</Client>";
            s += "   <NumeroLicenze>1</NumeroLicenze>";
            s += "   <NumeroAnagrafiche>1</NumeroAnagrafiche>";
            s += "</RevisoftLicenseFile>";

            //salvo file licenza prova
            RevisoftApplication.XmlManager x = new XmlManager();
            x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Avanzata;
            x.SaveEncodedFile(App.AppLicenseFile, s);

            //set Info var
            _Info_DataInstallazione = _DataInstallazione;
            _Info_DataCreazioneLicenzaProva = _DataCreazioneLicenzaProva;
            _Info_DataScadenzaLicenza = _Scadenza;
            _Info_DataUltimoUtilizzo = _DataUltimoUtilizzo;
            //_Info_CodiceMacchina = App.CodiceMacchina.Split('-')[0];
            //_Info_CodiceMacchinaServer = App.CodiceMacchina.Split('-')[0];
            _Info_CodiceMacchina = App.CodiceMacchina;
            _Info_CodiceMacchinaServer = App.CodiceMacchina;
            //_Info_TipoLicenza = _TipoLicenza;

            //INFO FILE
            s = "";
            s += "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
            s += "<RevisoftInfoFile versione=\"4.6\" >";
            s += "   <DataInstallazione>" + _Info_DataInstallazione + "</DataInstallazione>";                            //modificato
            s += "   <DataCreazioneLicenzaProva>" + _Info_DataCreazioneLicenzaProva + "</DataCreazioneLicenzaProva>";    //modificato
            s += "   <DataAttivazioneLicenza>" + _Info_DataAttivazioneLicenza + "</DataAttivazioneLicenza>";
            s += "   <DataScadenzaLicenza>" + _Info_DataScadenzaLicenza + "</DataScadenzaLicenza>";                      //modificiato
            s += "   <DataUltimoUtilizzo>" + _Info_DataUltimoUtilizzo + "</DataUltimoUtilizzo>";                         //modificiato
            s += "   <CodiceMacchina>" + _Info_CodiceMacchina.Split('-')[0] + "</CodiceMacchina>";
            s += "   <CodiceMacchinaServer>" + _Info_CodiceMacchinaServer.Split('-')[0] + "</CodiceMacchinaServer>";
            //4.5
            s += "   <Benvenuto>" + App.AppSetupBenvenuto + "</Benvenuto>";
            s += "   <IstruzioniAutomatiche>" + App.AppSetupIstruzioniAutomatiche + "</IstruzioniAutomatiche>";
            s += "   <AlertSuCompletato>" + App.AppSetupAlertSuCompletato + "</AlertSuCompletato>";
            s += "   <TipoGestioneArchivio>" + App.AppSetupTipoGestioneArchivio + "</TipoGestioneArchivio>";
            s += "   <PathArchivioRemoto>" + App.AppPathArchivioRemoto + "</PathArchivioRemoto>";
            //4.6
            s += "   <TipoGestioneArchivio>" + App.AppSetupBackupPersonalizzato + "</TipoGestioneArchivio>";
            s += "   <PathBackupUtente>" + App.AppUserBackupFolder + "</PathBackupUtente>";
            s += "   <AddioBackupRevisoft>" + App.AppSetupAddioBackupRevisoft + "</AddioBackupRevisoft>";
            s += "   <AddioBackupUtente>" + App.AppSetupAddioBackupUtente + "</AddioBackupUtente>";
            s += "</RevisoftInfoFile>";
            //salvo file
            SalvaInfoRevisoft(s);

            //settaggi
            //App.TipoLicenza = App.TipologieLicenze.Prova;
            App.CodiceMacchinaServer = _CodiceMacchinaServer.Split('-')[0];
            _StatoLicenza = true;
            //_StatoLicenzaCambiato = true;
            _LicenzaProvaDisponibile = false;

            //Configura licenza
            ConfiguraLicenza();

            //Configuro applicazione
            RevisoftApplication.Utilities u = new Utilities();
            u.ConfiguraApplicazione();
            //Registro - impedisco creazione di una licenza di prova
            u.ConfiguraRegistroAttivazioneLicenzaProva();
            //Salvo info in master file
            MasterFile m = MasterFile.Create();
            //m.SetChiaveServer(App.CodiceMacchina.Split('-')[0]);
            m.SetChiaveServer(App.CodiceMacchina);
            m.SetDataLicenzaProva(_DataCreazioneLicenzaProva.ToString());
        }

        public void CreaLicenzaDaWS()
        {
            //Utilities u = new Utilities();

            //if (!u.CheckConnection(App.urlCheckConnection))
            //{
            //    return;
            //}

            //Datalabor.Revisoft rw = new Datalabor.Revisoft();

            //u.LeggiInfoMacchina();

            //string newxml = rw.GetLicenzaXML(App.CodiceMacchina);

            //RevisoftApplication.XmlManager x = new XmlManager();
            //x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Avanzata;
            //x.SaveEncodedFile(App.AppLicenseFile, newxml);

            return;
        }

        public bool VerificaLicenzaWS()
        {
            //Utilities u = new Utilities();

            //if (!u.CheckConnection(App.urlCheckConnection))
            //{
            //    return true;
            //}

            ////verifica dei dati della licenza online
            //Datalabor.Revisoft rw = new Datalabor.Revisoft();

            //u.LeggiInfoMacchina();

            //string datascadenza = rw.GetScadenza(App.CodiceMacchina);

            //if(u.StringToDateTime(datascadenza) < DateTime.Now)
            //{
            //    return false;
            //}

            return true;
        }

        public void MigrazioneLicenza()
        {
            //verifico presenza nuova licenza, se esiste esco
            if (File.Exists(App.AppLicenseFile))
                return;

            //Verifico presenza vecchia licenza
            if (!File.Exists(App.AppLicenseFile_OLD))
                return;

            //Copio vecchia licenza in appdata
            FileInfo fi = new FileInfo(App.AppLicenseFile_OLD);
            fi.CopyTo(App.AppLicenseFile, true);

            //instanzio Utiliti x controllo data - andrea 2.8.1
            RevisoftApplication.Utilities u = new Utilities();

            //leggo vecchia licenza
            RevisoftApplication.XmlManager x = new XmlManager();
            x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Avanzata;
            XmlDocument f = new XmlDocument();
            f = x.LoadEncodedFile(App.AppLicenseFile);

            //Intestatario
            XmlNode n = f.SelectSingleNode("/RevisoftLicenseFile/Intestatario");
            _Intestatario = n.InnerText;
            //Utente
            n = f.SelectSingleNode("/RevisoftLicenseFile/Utente");
            _Utente = n.InnerText;
            //DataAttivazioneLicenza
            n = f.SelectSingleNode("/RevisoftLicenseFile/DataAttivazioneLicenza");
            //_DataAttivazioneLicenza = Convert.ToDateTime(n.InnerText);
            _DataAttivazioneLicenza = u.StringToDateTime(n.InnerText);
            //DataScadenzaLicenza
            n = f.SelectSingleNode("/RevisoftLicenseFile/DataScadenzaLicenza");
            //_DataScadenzaLicenza = Convert.ToDateTime(n.InnerText);
            _DataScadenzaLicenza = u.StringToDateTime(n.InnerText);
            //DurataLicenza
            n = f.SelectSingleNode("/RevisoftLicenseFile/DurataLicenza");
            _DurataLicenza = Convert.ToInt32(n.InnerText);
            //CodiceMacchina
            n = f.SelectSingleNode("/RevisoftLicenseFile/CodiceMacchina");
            _CodiceMacchina = n.InnerText.Split('-')[0];
            //CodiceMacchinaServer
            n = f.SelectSingleNode("/RevisoftLicenseFile/CodiceMacchinaServer");
            _CodiceMacchinaServer = n.InnerText.Split('-')[0];
            //TipoLicenza
            n = f.SelectSingleNode("/RevisoftLicenseFile/TipoLicenza");
            App.TipologieLicenze _TipoLicenza = (App.TipologieLicenze)(Enum.Parse(typeof(App.TipologieLicenze), n.InnerText, true));
            TrascodificaVecchiaLicenza(_TipoLicenza);

            //4.5 - Aggiungo nuovi campi
            XmlNode root = f.SelectSingleNode("/RevisoftLicenseFile");
            //Multilicenza
            XmlNode e = f.CreateNode(XmlNodeType.Element, "Multilicenza", "");
            e.InnerText = "False";
            root.AppendChild(e);
            //Sigillo
            e = f.CreateNode(XmlNodeType.Element, "Sigillo", "");
            e.InnerText = "True";
            root.AppendChild(e);
            //Licenza Prova
            e = f.CreateNode(XmlNodeType.Element, "LicenzaProva", "");
            e.InnerText = App.Prova.ToString();
            root.AppendChild(e);
            //Guest
            e = f.CreateNode(XmlNodeType.Element, "Guest", "");
            e.InnerText = App.Guest.ToString();
            root.AppendChild(e);
            //Server
            e = f.CreateNode(XmlNodeType.Element, "Server", "");
            e.InnerText = App.Server.ToString();
            root.AppendChild(e);
            //Cloud
            e = f.CreateNode(XmlNodeType.Element, "Cloud", "");
            e.InnerText = "False";
            root.AppendChild(e);
            //RemoteDesktop
            e = f.CreateNode(XmlNodeType.Element, "RemoteDesktop", "");
            e.InnerText = "False";
            root.AppendChild(e);
            //Client
            e = f.CreateNode(XmlNodeType.Element, "Client", "");
            e.InnerText = App.Client.ToString();
            root.AppendChild(e);
            //NumeroLicenze
            e = f.CreateNode(XmlNodeType.Element, "NumeroLicenze", "");
            e.InnerText = "1";
            root.AppendChild(e);
            //NumeroLicenze
            e = f.CreateNode(XmlNodeType.Element, "NumeroAnagrafiche", "");
            e.InnerText = App.NumeroanAgrafiche.ToString();
            root.AppendChild(e);

            //salvo nuova licenza
            x.SaveEncodedFile(App.AppLicenseFile, f.OuterXml);
        }


        public void TrascodificaVecchiaLicenza(App.TipologieLicenze _TipoLicenza)
        {
            switch (_TipoLicenza)
            {
                //PROVA
                case App.TipologieLicenze.Prova:
                    App.NumeroanAgrafiche = 1;
                    App.Prova = true;
                    break;
                //DESKTOP - standard revisoft
                case App.TipologieLicenze.DeskTop:
                    App.NumeroanAgrafiche = 1000; //infinite
                    break;
                //SERVER - rete revisoft
                case App.TipologieLicenze.Server:
                    App.NumeroanAgrafiche = 1000; //infinite
                    App.Server = true;
                    break;
                //CLIENT LAN
                case App.TipologieLicenze.ClientLan:
                case App.TipologieLicenze.ClientLanMulti:
                    App.NumeroanAgrafiche = 1000; //infinite
                    App.Client = true;
                    break;
                //ENTRY LEVEL
                case App.TipologieLicenze.EntryLevel:
                    App.NumeroanAgrafiche = 1;
                    break;
                //VIEWER
                case App.TipologieLicenze.Viewer:
                    App.NumeroanAgrafiche = 1;
                    break;
                case App.TipologieLicenze.Guest:
                    App.Guest = true;
                    break;
            }

        }



        public void LeggiDatiLicenza()
        {
            //controllo presenza file: se manca esco
            if (!File.Exists(App.AppLicenseFile))
            {
                App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.MancaFileLicenza;
                m.VisualizzaMessaggio();
                return;
            }

            //instanzio Utiliti x controllo data - andrea 2.8.1
            RevisoftApplication.Utilities u = new Utilities();

            //leggo licenza
            RevisoftApplication.XmlManager x = new XmlManager();
            x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Avanzata;
            XmlDocument f = new XmlDocument();
            f = x.LoadEncodedFile(App.AppLicenseFile);

            //Intestatario
            XmlNode n = f.SelectSingleNode("/RevisoftLicenseFile/Intestatario");
            _Intestatario = n.InnerText;
            //Utente
            n = f.SelectSingleNode("/RevisoftLicenseFile/Utente");
            _Utente = n.InnerText;
            //DataAttivazioneLicenza
            n = f.SelectSingleNode("/RevisoftLicenseFile/DataAttivazioneLicenza");
            //_DataAttivazioneLicenza = Convert.ToDateTime(n.InnerText);
            _DataAttivazioneLicenza = u.StringToDateTime(n.InnerText);
            //DataScadenzaLicenza
            n = f.SelectSingleNode("/RevisoftLicenseFile/DataScadenzaLicenza");
            //_DataScadenzaLicenza = Convert.ToDateTime(n.InnerText);
            _DataScadenzaLicenza = u.StringToDateTime(n.InnerText);
            //DurataLicenza
            n = f.SelectSingleNode("/RevisoftLicenseFile/DurataLicenza");
            _DurataLicenza = Convert.ToInt32(n.InnerText);
            //CodiceMacchina
            n = f.SelectSingleNode("/RevisoftLicenseFile/CodiceMacchina");
            _CodiceMacchina = n.InnerText.Split('-')[0];
            //CodiceMacchinaServer
            n = f.SelectSingleNode("/RevisoftLicenseFile/CodiceMacchinaServer");
            _CodiceMacchinaServer = n.InnerText.Split('-')[0];
            //4.5
            //Multilicenza
            n = f.SelectSingleNode("/RevisoftLicenseFile/Multilicenza");
            _Multilicenza = Convert.ToBoolean(n.InnerText);
            //Sigillo
            n = f.SelectSingleNode("/RevisoftLicenseFile/Sigillo");
            _Sigillo = Convert.ToBoolean(n.InnerText);
            //LicenzaProva
            n = f.SelectSingleNode("/RevisoftLicenseFile/LicenzaProva");
            _LicenzaProva = Convert.ToBoolean(n.InnerText);
            //Guest
            n = f.SelectSingleNode("/RevisoftLicenseFile/Guest");
            _Guest = Convert.ToBoolean(n.InnerText);
            //Server
            n = f.SelectSingleNode("/RevisoftLicenseFile/Server");
            _Server = Convert.ToBoolean(n.InnerText);
            //Cloud
            n = f.SelectSingleNode("/RevisoftLicenseFile/Cloud");
            _Cloud = Convert.ToBoolean(n.InnerText);
            //RemoteDesktop
            n = f.SelectSingleNode("/RevisoftLicenseFile/RemoteDesktop");
            _RemoteDesktop = Convert.ToBoolean(n.InnerText);
            //Client
            n = f.SelectSingleNode("/RevisoftLicenseFile/Client");
            _Client = Convert.ToBoolean(n.InnerText);
            //NumeroLicenze
            n = f.SelectSingleNode("/RevisoftLicenseFile/NumeroLicenze");
            _NumeroLicenze = Convert.ToInt32(n.InnerText);
            //NumeroAnagrafiche
            n = f.SelectSingleNode("/RevisoftLicenseFile/NumeroAnagrafiche");
            _NumeroAnagrafiche = Convert.ToInt32(n.InnerText);
        }


        public bool VerificaLicenza()
        {

            if (!VerificaLicenzaWS())
            {
                return false;
            }

            //Gestione messaggi
            App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
            RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();

            //Corrispondenze INFO/LICENZA *************************************************
            //Scadenza
            if (_DataScadenzaLicenza != _Info_DataScadenzaLicenza)
            {
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckInfoScadenza;
                m.VisualizzaMessaggio();
                return false;
            }

            //Codice macchina PC/CLIENT
            if (_CodiceMacchina != _Info_CodiceMacchina)
            {
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckInfoCodiceMacchina;
                m.VisualizzaMessaggio();
                return false;
            }
            //Codice macchina SERVER
            if (_CodiceMacchinaServer.Split('-')[0] != _Info_CodiceMacchinaServer.Split('-')[0])
            {
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckInfoCodiceMacchinaServer;
                m.VisualizzaMessaggio();
                return false;
            }

            //Data attivazione Licenza prova 
            //if (_TipoLicenza == App.TipologieLicenze.Prova)
            if (_LicenzaProva == true)
            {
                if (_Info_DataCreazioneLicenzaProva != _DataAttivazioneLicenza)
                {
                    m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckInfoDataAttivazione;
                    m.VisualizzaMessaggio();
                    return false;
                }
                if (PERIODO_PROVA != _DurataLicenza)
                {
                    m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckInfoDurataLicenza;
                    m.VisualizzaMessaggio();
                    return false;
                }
            }

            else

            //Data attivazione Licenza regolare
            //if (_TipoLicenza == App.TipologieLicenze.Server || _TipoLicenza == App.TipologieLicenze.DeskTop || _TipoLicenza == App.TipologieLicenze.EntryLevel || _TipoLicenza == App.TipologieLicenze.ClientLan || _TipoLicenza == App.TipologieLicenze.ClientLanMulti || _TipoLicenza == App.TipologieLicenze.Viewer || _TipoLicenza == App.TipologieLicenze.Guest)
            {
                if (_Info_DataAttivazioneLicenza != _DataAttivazioneLicenza)
                {
                    m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckInfoDataAttivazione;
                    m.VisualizzaMessaggio();
                    return false;
                }
                if (DURATA_LICENZA != _DurataLicenza)
                {
                    m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckInfoDurataLicenza;
                    m.VisualizzaMessaggio();
                    return false;
                }
            }

            //CONTROLLI DATE **************************************************************
            //Controllo scadenza
            if (DateTime.Now > _DataScadenzaLicenza)
            {
                App.Scaduta = true;
                //App.TipoLicenza = App.TipologieLicenze.Scaduta;
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckLicenzaScaduta;
                m.VisualizzaMessaggio();
                App.Scaduta = true;
                //_TipoLicenza = App.TipologieLicenze.Scaduta;
                return false;
            }
            //Controllo data ultimo utilizzo
            if (_Info_DataUltimoUtilizzo > DateTime.Now)
            {
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckLicenzaUltimoUso;
                m.VisualizzaMessaggio();
                return false;
            }

            //CONTROLLI CODICE MACCHINA ****************************************************
            if ((App.CodiceMacchina != CodiceMacchina) && App.CodiceMacchina.Split('-')[0] != CodiceMacchina.Split('-')[0])
            {
                ////verifica singola corrispondenza
                //string[] cmReale = App.CodiceMacchina.Split('-');
                //string[] cmLicenza = CodiceMacchina.Split('-');

                ////almeno una chiave corretta
                //if (cmReale[0] != cmLicenza[0] && cmReale[1] == cmLicenza[1] || cmReale[0] == cmLicenza[0] && cmReale[1] != cmLicenza[1])
                //{
                //    App.ErrorLevel = App.ErrorTypes.Avviso;
                //    m.TipoMessaggioAvviso = WindowGestioneMessaggi.TipologieMessaggiAvvisi.CodiceMacchinaParziale;
                //    m.VisualizzaMessaggio();
                //}
                //else
                //{
                //chiave diversa
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckLicenzaCodiceMacchina;
                m.VisualizzaMessaggio();
                return false;
                //}
            }


            //LICENZA VALIDA ****************************************************************************************************
            //controlli su chiave server rimandati su main window
            //Giorni utilizzati
            TimeSpan u = DateTime.Now - _DataAttivazioneLicenza;
            _GiorniUtilizzati = Convert.ToInt32(u.TotalDays);
            //Giorni alla scadenza
            TimeSpan g = _DataScadenzaLicenza - DateTime.Now;
            _GiorniAllaScadenza = Convert.ToInt32(g.TotalDays);

            if (_GiorniUtilizzati == 0)
                _GiorniUtilizzati = 1;

            //Percentuale giorni
            _GiorniUtilizzatiPercentuale = Convert.ToInt32(100 * _GiorniUtilizzati / _DurataLicenza);
            if (_GiorniUtilizzatiPercentuale <= 0)
                _GiorniUtilizzatiPercentuale = 1;
            if (_GiorniUtilizzatiPercentuale >= 365)
                _GiorniUtilizzatiPercentuale = 100;

            //Settaggi di interfaccia
            if (_LicenzaProva == true)
            //if (_TipoLicenza == App.TipologieLicenze.Prova)
            {
                _LicenzaProvaDisponibile = false;
                _LicenzaDisponibile = true;

                if (_GiorniAllaScadenza < AVVISO_GIORNI_ALLA_SCADENZA_PROVA)
                    _ScadenzaVicina = true;
            }

            else
            //if (_TipoLicenza == App.TipologieLicenze.Server || _TipoLicenza == App.TipologieLicenze.DeskTop || _TipoLicenza == App.TipologieLicenze.EntryLevel || _TipoLicenza == App.TipologieLicenze.ClientLan || _TipoLicenza == App.TipologieLicenze.ClientLanMulti || _TipoLicenza == App.TipologieLicenze.Viewer || _TipoLicenza == App.TipologieLicenze.Guest)
            {
                _LicenzaProvaDisponibile = false;
                _LicenzaDisponibile = false;

                if (_GiorniAllaScadenza < AVVISO_GIORNI_ALLA_SCADENZA)
                {
                    _ScadenzaVicina = true;
                    _LicenzaDisponibile = true;
                }
            }




            //4.5
            //Setto tipolicanza di App
            //App.TipoLicenza = _TipoLicenza;
            App.CodiceMacchinaServer = _CodiceMacchinaServer;

            //4.5 nuove proprietà
            App.Multilicenza = _Multilicenza;
            App.Sigillo = _Sigillo;
            App.Prova = _LicenzaProva;
            App.Guest = _Guest;
            App.Server = _Server;
            App.Cloud = _Cloud;
            App.RemoteDesktop = _RemoteDesktop;
            App.Client = _Client;
            App.NumeroLicenze = _NumeroLicenze;
            App.NumeroanAgrafiche = _NumeroAnagrafiche;


            //Controlli istanze Revisoft e Remote Desktop
            if (App.AppIstanzeAttive > App.NumeroLicenze)
            {
                if (SystemInformation.TerminalServerSession)
                {
                    m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckLimiteUtenti;
                    m.VisualizzaMessaggio();
                    return false;
                }
                else
                {
                    System.Windows.MessageBox.Show("Revisoft è già in esecuzione.\nImpossibile eseguire il programma nuovamente.");
                    Utilities u2 = new Utilities();
                    u2.ChiudiApplicazioneSuErrore();
                    return false;
                }
            }
            else
            {
                //verifica doppia esecuzione da parte dello stesso utente in terminal server
                if (SystemInformation.TerminalServerSession)
                {
                    Utilities u2 = new Utilities();
                   
                    if (u2.VerificaIstanzeUtente())
                    {
                        System.Windows.MessageBox.Show("Revisoft è già in esecuzione.\nImpossibile eseguire il programma nuovamente.");
                        u2.ChiudiApplicazioneSuErrore();
                        return false;
                    }
                }
            }

            //resetto stato errore
            App.ErrorLevel = App.ErrorTypes.Nessuno;

            //Licenza corretta
            _StatoLicenza = true;
            return _StatoLicenza;
        }


        public bool VerificaInfoMasterFile_old()
        {
            MasterFile mf = MasterFile.Create();

            #if DEBUG
                _StatoLicenza = true;
                _LicenzaProva = false;
            #else
            //chiave server
            string lanChiaveServer = mf.GetChiaveServer();
            if (App.CodiceMacchinaServer.Trim().Split('-')[0] != "" && App.CodiceMacchina.Split('-')[0] != App.CodiceMacchinaServer.Split('-')[0] && App.CodiceMacchina != App.CodiceMacchinaServer.Split('-')[0])
            {
                if (App.CodiceMacchina.Split('-')[0] != CodiceMacchina.Split('-')[0] && App.CodiceMacchinaServer.Split('-')[0] != lanChiaveServer.Split('-')[0])
                {
                    //mancanza di corrispondenza chiave server / masterfile
                    App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                    RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                    m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckChiaveServerMasterFile;
                    m.VisualizzaMessaggio();
                    _StatoLicenza = false;
                    return _StatoLicenza;
                }
            }
            else
            {
                if (App.CodiceMacchinaServer.Split('-')[0] != lanChiaveServer.Split('-')[0])
                {
                    //mancanza di corrispondenza chiave server / masterfile
                    App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                    RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                    m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckChiaveServerMasterFile;
                    m.VisualizzaMessaggio();
                    _StatoLicenza = false;
                    return _StatoLicenza;
                }
            }
            //    if ((App.TipoLicenza != App.TipologieLicenze.ClientLan && App.TipoLicenza != App.TipologieLicenze.ClientLanMulti &&  App.CodiceMacchinaServer != lanChiaveServer) ||
            //    (App.TipoLicenza != App.TipologieLicenze.ClientLan && App.TipoLicenza != App.TipologieLicenze.ClientLanMulti && App.CodiceMacchina != CodiceMacchina && App.CodiceMacchinaServer != lanChiaveServer))
            //{
            //    //mancanza di corrispondenza chiave server / masterfile
            //    App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
            //    RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
            //    m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckChiaveServerMasterFile;
            //    m.VisualizzaMessaggio();
            //    _StatoLicenza = false;
            //    return _StatoLicenza;
            //}
            //else
            {
                //corrispondenza corretta
                _StatoLicenza = true;
            }
#endif

            //data licenza prova
            if (_LicenzaProva == true)// App.TipoLicenza == App.TipologieLicenze.Prova)
            {
                string dataLicenzaProva = mf.GetDataLicenzaProva();
                if (_Info_DataCreazioneLicenzaProva.ToString() != dataLicenzaProva)
                {
                    //mancanza di corrispondenza data licenza prova
                    App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                    RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                    m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckDataLicenzaProvaMasterFile;
                    m.VisualizzaMessaggio();
                    _StatoLicenza = false;
                    return _StatoLicenza;
                }
                else
                {
                    //corrispondenza corretta
                    _StatoLicenza = true;
                }
            }

/*
            //data tutte le altre licenze
            if (App.TipoLicenza != App.TipologieLicenze.Prova)
            {
                string dataLicenza = mf.GetDataLicenza();
                if (_Info_DataAttivazioneLicenza.ToString() != dataLicenza)
                {
                    //mancanza di corrispondenza data licenza prova
                    App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                    RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                    m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckDataLicenzaMasterFile;
                    m.VisualizzaMessaggio();
                    _StatoLicenza = false;
                    return _StatoLicenza;
                }
                else
                {
                    //corrispondenza corretta
                    _StatoLicenza = true;
                }
            }
*/
            return _StatoLicenza;
        }
        public bool VerificaInfoMasterFile()
        {
#if (!DBG_TEST)
      return VerificaInfoMasterFile_old();
#endif
      MasterFile mf = MasterFile.Create();

                _StatoLicenza = true;
                _LicenzaProva = false;

            //data licenza prova
            if (_LicenzaProva == true)// App.TipoLicenza == App.TipologieLicenze.Prova)
            {
                string dataLicenzaProva = mf.GetDataLicenzaProva();
                if (_Info_DataCreazioneLicenzaProva.ToString() != dataLicenzaProva)
                {
                    //mancanza di corrispondenza data licenza prova
                    App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                    RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                    m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckDataLicenzaProvaMasterFile;
                    m.VisualizzaMessaggio();
                    _StatoLicenza = false;
                    return _StatoLicenza;
                }
                else
                {
                    //corrispondenza corretta
                    _StatoLicenza = true;
                }
            }

/*
            //data tutte le altre licenze
            if (App.TipoLicenza != App.TipologieLicenze.Prova)
            {
                string dataLicenza = mf.GetDataLicenza();
                if (_Info_DataAttivazioneLicenza.ToString() != dataLicenza)
                {
                    //mancanza di corrispondenza data licenza prova
                    App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                    RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                    m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckDataLicenzaMasterFile;
                    m.VisualizzaMessaggio();
                    _StatoLicenza = false;
                    return _StatoLicenza;
                }
                else
                {
                    //corrispondenza corretta
                    _StatoLicenza = true;
                }
            }
*/
            return _StatoLicenza;
        }


        public void ConfiguraLicenza()
        {
            //Funzionalità da proprietà licenza
            App.AppConsentiSigillo = App.Sigillo;
            App.AppConsentiMultiLicenza = App.Multilicenza;
            App.AppConsentiAccessoArchivioCloud = App.Cloud;
            App.AppConsentiSigillo = App.Sigillo;

            //Attivo le variabili funzionali di base
            App.AppConsentiImportaEsporta = true;
            App.AppConsentiCreazioneAnagrafica = true;
            App.AppConsentiAccessoArchivioLocale = true;
            App.AppConsentiAccessoArchivioRemoto = true;
            App.AppConsentiBackUp = true;
            //4.5.1
            App.AppConsentiGestioneBackUp = true;


            //Disattivo le variabili funzionali evolute
            App.AppConsentiGestioneArchivioRemoto = false;
            App.AppConsentiImportazioneEsportazioneLan = false;


            //configura funzionalità evolute in base alle proprietà della licenza
            if (App.Server)
            {
                App.AppConsentiGestioneArchivioRemoto = true;
                App.AppConsentiImportazioneEsportazioneLan = true;
                App.AppConsentiBackUp = true;
            }

            if (App.Client)
            {
                App.AppConsentiImportazioneEsportazioneLan = true;
                App.AppConsentiBackUp = (App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Locale || App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.LocaleImportExport);
            }



            //andrea 4.5
            //if (App.CodiceMacchinaServer.Trim().Split('-')[0] != "" && App.CodiceMacchina.Split('-')[0] != App.CodiceMacchinaServer.Split('-')[0])
            //{
            //    App.AppConsentiImportazioneEsportazioneLan = true;
            //    //App.AppConsentiAccessoArchivioRemoto = true;
            //    //App.AppConsentiGestioneArchivioRemoto = true;
            //    App.AppConsentiBackUp = false;
            //}



            //    //Configuro funzionalità
            //    switch (App.TipoLicenza)
            //{
            //    //PROVA
            //    case App.TipologieLicenze.Prova:
            //        App.AppConsentiAccessoArchivioLocale = true;
            //        App.AppConsentiCreazioneAnagrafica = true;
            //        break;
            //    //DESKTOP - standard revisoft
            //    case App.TipologieLicenze.DeskTop:
            //        App.AppConsentiAccessoArchivioLocale = true;
            //        App.AppConsentiCreazioneAnagrafica = true;
            //        App.AppConsentiImportaEsporta = true;
            //        App.AppConsentiBackUp = true;
            //        break;
            //    //SERVER - rete revisoft
            //    case App.TipologieLicenze.Server:
            //        App.AppConsentiAccessoArchivioLocale = true;
            //        App.AppConsentiAccessoArchivioRemoto = true;
            //        App.AppConsentiGestioneArchivioRemoto = true;
            //        App.AppConsentiCreazioneAnagrafica = true;
            //        App.AppConsentiImportaEsporta = true;
            //        App.AppConsentiBackUp = true;
            //        break;
            //    //CLIENT LAN
            //    case App.TipologieLicenze.ClientLan:
            //    case App.TipologieLicenze.ClientLanMulti:
            //        App.AppConsentiAccessoArchivioRemoto = true;
            //        App.AppConsentiCreazioneAnagrafica = true;
            //        App.AppConsentiImportazioneEsportazioneLan = true;
            //        App.AppConsentiMultiLicenza = (App.TipoLicenza == App.TipologieLicenze.ClientLanMulti);

            //        //vers. 3.3 - consento backup se archivio locale
            //        App.AppConsentiBackUp = (App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Locale);

            //        break;
            //    //ENTRY LEVEL
            //    case App.TipologieLicenze.EntryLevel:
            //        App.AppConsentiAccessoArchivioLocale = true;
            //        App.AppConsentiCreazioneAnagrafica = true;
            //        //App.AppConsentiImportaEsporta = true; modifica con versione 2.1
            //        App.AppConsentiBackUp = true;
            //        break;
            //    //VIEWER + GUEST
            //    case App.TipologieLicenze.Viewer:
            //    case App.TipologieLicenze.Guest:
            //        App.AppConsentiAccessoArchivioLocale = true;
            //        //App.AppConsentiCreazioneAnagrafica = true; modifica con versione 2.5
            //        App.AppConsentiImportaEsporta = true;
            //        break;
            //}


        }




        public bool AttivaLicenzaDaFile(string fileName)
        {
            //verifico tipologia percorso file
            if (fileName.IndexOf(':') == 1)
            {
                RevisoftApplication.Utilities u2 = new Utilities();
                fileName = u2.GetRealPathFile(fileName);
            }

            //controllo presenza file: se manca esco
            if (!File.Exists(fileName))
            {
                App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.FileNonTrovato;
                m.VisualizzaMessaggio();
                return false;
            }

            //verifico se licenza è già stata attivata e trasferita nella cartella applicativa
            if (!App.AppForzaAttivazioneLicenza)
            {
                if (fileName.StartsWith(App.AppLocalDataFolder))
                {
                    return false;
                }
            }


            //info macchina
            RevisoftApplication.Utilities u = new Utilities();
            u.LeggiInfoMacchina();

            //apro licenza
            RevisoftApplication.XmlManager x = new XmlManager();
            x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Avanzata;
            XmlDocument f = new XmlDocument();
            f = x.LoadEncodedFile(fileName);

            //Controllo formato e codice macchina: se errato esco
            XmlNode n = f.SelectSingleNode("/RevisoftLicenseFile/CodiceMacchina");
            if (n == null)
            {
                App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckLicenzaFormatoErrato;
                m.VisualizzaMessaggio();
                return false;
            }

            if (n.InnerText.Split('-')[0].ToString() != App.CodiceMacchina.Split('-')[0].ToString())
            {
                App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckLicenzaCodiceMacchina;
                m.VisualizzaMessaggio();
                return false;
            }

            //Leggo file info
            if (!LeggiInfoRevisoft())
                return false;

            //Rinomino file licenza vecchio se presente
            if (File.Exists(App.AppLicenseFile))
            {
                string oldLic = App.AppLicenseFile + ".old";
                //elimino eventuale file prcedente
                if (File.Exists(oldLic))
                    File.Delete(oldLic);
                //Rinomino file
                File.Move(App.AppLicenseFile, oldLic);
            }

            //trasferisco nuova licenza nella cartella Revisoft
            File.Move(fileName, App.AppLicenseFile);

            //Leggo dati licenza
            LeggiDatiLicenza();
            //configuro tipo licenza e codice macchina server (recuperato da licenza)
            //App.TipoLicenza = _TipoLicenza;
            App.CodiceMacchinaServer = _CodiceMacchinaServer.Split('-')[0];

            _GiorniUtilizzati = 1;
            _GiorniAllaScadenza = _DurataLicenza;

            //setup
            DateTime Adesso = DateTime.Now;
            _DataInstallazione = Adesso;
            _DataUltimoUtilizzo = Adesso;

            //set Info var
            _Info_DataInstallazione = _DataInstallazione;
            _Info_DataAttivazioneLicenza = _DataAttivazioneLicenza;
            _Info_DataScadenzaLicenza = _DataScadenzaLicenza;
            _Info_DataUltimoUtilizzo = _DataUltimoUtilizzo;
            _Info_CodiceMacchina = App.CodiceMacchina.Split('-')[0];
            _Info_CodiceMacchinaServer = App.CodiceMacchinaServer.Split('-')[0];
            //_Info_TipoLicenza = App.TipoLicenza;

            //INFO FILE
            string s = "";
            s += "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
            s += "<RevisoftInfoFile versione=\"4.6\" >";
            s += "   <DataInstallazione>" + _Info_DataInstallazione + "</DataInstallazione>";                           //modificato
            s += "   <DataCreazioneLicenzaProva>" + _Info_DataCreazioneLicenzaProva + "</DataCreazioneLicenzaProva>";
            s += "   <DataAttivazioneLicenza>" + _Info_DataAttivazioneLicenza + "</DataAttivazioneLicenza>";            //modificiato
            s += "   <DataScadenzaLicenza>" + _Info_DataScadenzaLicenza + "</DataScadenzaLicenza>";                     //modificiato
            s += "   <DataUltimoUtilizzo>" + _Info_DataUltimoUtilizzo + "</DataUltimoUtilizzo>";                         //modificiato
            s += "   <CodiceMacchina>" + _Info_CodiceMacchina.Split('-')[0] + "</CodiceMacchina>";
            s += "   <CodiceMacchinaServer>" + _Info_CodiceMacchinaServer.Split('-')[0] + "</CodiceMacchinaServer>";
            //4.5
            s += "   <Benvenuto>" + App.AppSetupBenvenuto + "</Benvenuto>";
            s += "   <IstruzioniAutomatiche>" + App.AppSetupIstruzioniAutomatiche + "</IstruzioniAutomatiche>";
            s += "   <AlertSuCompletato>" + App.AppSetupAlertSuCompletato + "</AlertSuCompletato>";
            s += "   <TipoGestioneArchivio>" + App.AppSetupTipoGestioneArchivio + "</TipoGestioneArchivio>";
            s += "   <PathArchivioRemoto>" + App.AppPathArchivioRemoto + "</PathArchivioRemoto>";
            //4.6
            s += "   <TipoGestioneArchivio>" + App.AppSetupBackupPersonalizzato + "</TipoGestioneArchivio>";
            s += "   <PathBackupUtente>" + App.AppUserBackupFolder + "</PathBackupUtente>";
            s += "</RevisoftInfoFile>";
            //salvo file
            SalvaInfoRevisoft(s);

            //settaggi
            _StatoLicenza = true;
            //_StatoLicenzaCambiato = true;
            _LicenzaProvaDisponibile = false;

            //andrea 3.0 - multilicenza disponibile per licenza server
            if (_Multilicenza == true)// App.TipoLicenza == App.TipologieLicenze.ClientLanMulti)
                AggiungiMultiLicenza();

            //Configura licenza
            ConfiguraLicenza();

            //Configuro applicazione
            u.ConfiguraApplicazione();
            u.ConfiguraPercorsi();
            //Registro - impedisco creazione di una licenza di prova
            u.ConfiguraRegistroAttivazioneLicenzaProva();

            //Salvo info in master file
            //if (App.TipoLicenza != App.TipologieLicenze.ClientLan && App.TipoLicenza != App.TipologieLicenze.ClientLanMulti)
            {
                MasterFile mf = MasterFile.Create();
                mf.SetChiaveServer(App.CodiceMacchinaServer);
                mf.SetDataLicenza(_DataAttivazioneLicenza.ToString());
            }

            //esci
            return true;
        }




        public void AggiungiMultiLicenza()
        {
            string destFileName = "";
            string searchName = App.ApplicationFileName + " (*";
            int fileCount = 0;

            RevisoftApplication.Utilities u = new Utilities();

            try
            {
                //verifica esistenza cartella, la creo se manca
                if (!Directory.Exists(App.AppLicenseFolder))
                {
                    Directory.CreateDirectory(App.AppLicenseFolder);
                }

                //conteggio file
                fileCount = Directory.GetFiles(App.AppLicenseFolder, searchName).Length + 1;

                //File di destinazione
                destFileName = App.AppLicenseFolder + "\\" + App.ApplicationFileName + " (" + fileCount.ToString() + ")" + u.EstensioneFile(App.TipoFile.Licenza);
                File.Copy(App.AppLicenseFile, destFileName);
            }
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
            catch (Exception e)
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata
            {
                App.ErrorLevel = App.ErrorTypes.Errore;
                RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                m.TipoMessaggioErrore = WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInSalvataggioMultiLicenza;
                m.VisualizzaMessaggio();
            }
        }


        public bool LeggiMultilicenza()
        {
            string searchName = App.ApplicationFileName + " (*";
            int fileCount = 0;
            string mlFileName = "";

            RevisoftApplication.Utilities u = new Utilities();

            try
            {
                //verifica esistenza cartella
                if (!Directory.Exists(App.AppLicenseFolder))
                {
                    if (!Directory.Exists(App.AppProgramFolder + "\\" + App.LicenseFolder))
                    {
                        return false;
                    }
                    else
                    {
                        foreach (string item in Directory.GetFiles(App.AppProgramFolder + "\\" + App.LicenseFolder, searchName))
                        {
                            FileInfo fi = new FileInfo(App.AppProgramFolder + "\\" + App.LicenseFolder + "\\" + item);
                            fi.CopyTo(App.AppLicenseFolder + "\\" + item, true);
                        }
                    }
                }

                //verifico presenza files
                fileCount = Directory.GetFiles(App.AppLicenseFolder, searchName).Length;
                if (fileCount == 0)
                {
                    return false;
                }

                //Resetto array dati
                if (DatiMultiLicenza != null)
                    DatiMultiLicenza.Clear();

                //cliclo directory multilicenza
                for (int i = 1; i <= fileCount; i++)
                {
                    mlFileName = App.AppLicenseFolder + "\\" + App.ApplicationFileName + " (" + i.ToString() + ")" + u.EstensioneFile(App.TipoFile.Licenza);
                    //leggo dati licenza
                    LeggiDatiMultiLicenza(mlFileName);
                }

                return true;

            }
            catch (Exception e)
            {
                string log = e.Message;
                App.ErrorLevel = App.ErrorTypes.Errore;
                RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                m.TipoMessaggioErrore = WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreInLetturaMultiLicenza;
                m.VisualizzaMessaggio();

                return false;
            }


        }

        private void LeggiDatiMultiLicenza(string mlFileName)
        {
            string htKey;

            //instanzio Utiliti
            RevisoftApplication.Utilities u = new Utilities();

            //leggo licenza
            RevisoftApplication.XmlManager x = new XmlManager();
            x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Avanzata;
            XmlDocument f = new XmlDocument();
            f = x.LoadEncodedFile(mlFileName);

            //Utente
            XmlNode n = f.SelectSingleNode("/RevisoftLicenseFile/Utente");
            htKey = n.InnerText;

            //Utente
            if (!DatiMultiLicenza.Contains(htKey))
            {
                DatiMultiLicenza.Add(htKey, new Hashtable());
                //Filename
                ((Hashtable)(DatiMultiLicenza[htKey])).Add("FileName", mlFileName);

                //DataScadenzaLicenza
                n = f.SelectSingleNode("/RevisoftLicenseFile/DataScadenzaLicenza");
                ((Hashtable)(DatiMultiLicenza[htKey])).Add("DataScadenza", n.InnerText);

                //TipoLicenza
                //n = f.SelectSingleNode("/RevisoftLicenseFile/TipoLicenza");
                //((Hashtable)(DatiMultiLicenza[htKey])).Add("TipoLicenza", n.InnerText);

                //setto vari globale
                _StatoLicenzaMultipla = true;
            }
        }



        public bool AttivaMultiLicenzaDaFile(string fileName)
        {
            //verifico tipologia percorso file
            if (fileName.IndexOf(':') == 1)
            {
                RevisoftApplication.Utilities u2 = new Utilities();
                fileName = u2.GetRealPathFile(fileName);
            }

            //controllo presenza file: se manca esco
            if (!File.Exists(fileName))
            {
                App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.FileNonTrovato;
                m.VisualizzaMessaggio();
                return false;
            }

            ////verifico se licenza è già stata attivata e trasferita nella cartella applicativa
            //if (!App.AppForzaAttivazioneLicenza)
            //{
            //    if (fileName.StartsWith(App.AppProgramFolder))
            //    {
            //        return false;
            //    }
            //}


            //info macchina
            RevisoftApplication.Utilities u = new Utilities();
            u.LeggiInfoMacchina();

            //apro licenza
            RevisoftApplication.XmlManager x = new XmlManager();
            x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Avanzata;
            XmlDocument f = new XmlDocument();
            f = x.LoadEncodedFile(fileName);

            //Controllo formato e codice macchina: se errato esco
            XmlNode n = f.SelectSingleNode("/RevisoftLicenseFile/CodiceMacchinaServer");
            if (n == null)
            {
                App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckLicenzaFormatoErrato;
                m.VisualizzaMessaggio();
                return false;
            }

            if (n.InnerText.Split('-')[0].ToString() != App.CodiceMacchinaServer.Split('-')[0].ToString())
            {
                App.ErrorLevel = App.ErrorTypes.ErroreBloccante;
                RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                m.TipoMessaggioErroreBloccante = WindowGestioneMessaggi.TipologieMessaggiErroriBloccanti.CheckLicenzaCodiceMacchina;
                m.VisualizzaMessaggio();
                return false;
            }

            //Leggo file info
            if (!LeggiInfoRevisoft())
                return false;

            //Rinomino file licenza vecchio se presente
            if (File.Exists(App.AppLicenseFile))
            {
                string oldLic = App.AppLicenseFile + ".old";
                //elimino eventuale file prcedente
                if (File.Exists(oldLic))
                    File.Delete(oldLic);
                //Rinomino file
                File.Move(App.AppLicenseFile, oldLic);
            }

            //trasferisco nuova licenza nella cartella Revisoft
            File.Copy(fileName, App.AppLicenseFile);

            //Leggo dati licenza
            LeggiDatiLicenza();
            //configuro tipo licenza e codice macchina server (recuperato da licenza)
            //App.TipoLicenza = _TipoLicenza;
            App.CodiceMacchinaServer = _CodiceMacchinaServer;//_CodiceMacchinaServer.Split('-')[0];

            _GiorniUtilizzati = 1;
            _GiorniAllaScadenza = _DurataLicenza;

            //setup
            DateTime Adesso = DateTime.Now;
            _DataInstallazione = Adesso;
            _DataUltimoUtilizzo = Adesso;

            //set Info var
            _Info_DataInstallazione = _DataInstallazione;
            _Info_DataAttivazioneLicenza = _DataAttivazioneLicenza;
            _Info_DataScadenzaLicenza = _DataScadenzaLicenza;
            _Info_DataUltimoUtilizzo = _DataUltimoUtilizzo;
            _Info_CodiceMacchina = App.CodiceMacchina.Split('-')[0];
            _Info_CodiceMacchinaServer = App.CodiceMacchinaServer.Split('-')[0];
            //_Info_TipoLicenza = App.TipoLicenza;

            //INFO FILE
            string s = "";
            s += "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
            s += "<RevisoftInfoFile versione=\"4.6\" >";
            s += "   <DataInstallazione>" + _Info_DataInstallazione + "</DataInstallazione>";                           //modificato
            s += "   <DataCreazioneLicenzaProva>" + _Info_DataCreazioneLicenzaProva + "</DataCreazioneLicenzaProva>";
            s += "   <DataAttivazioneLicenza>" + _Info_DataAttivazioneLicenza + "</DataAttivazioneLicenza>";            //modificiato
            s += "   <DataScadenzaLicenza>" + _Info_DataScadenzaLicenza + "</DataScadenzaLicenza>";                     //modificiato
            s += "   <DataUltimoUtilizzo>" + _Info_DataUltimoUtilizzo + "</DataUltimoUtilizzo>";                         //modificiato
            s += "   <CodiceMacchina>" + _Info_CodiceMacchina.Split('-')[0] + "</CodiceMacchina>";
            s += "   <CodiceMacchinaServer>" + _Info_CodiceMacchinaServer.Split('-')[0] + "</CodiceMacchinaServer>";
            //s += "   <TipoLicenza>" + _Info_TipoLicenza.ToString() + "</TipoLicenza>";                                   //modificato
            //4.5
            s += "   <Benvenuto>" + App.AppSetupBenvenuto + "</Benvenuto>";
            s += "   <IstruzioniAutomatiche>" + App.AppSetupIstruzioniAutomatiche + "</IstruzioniAutomatiche>";
            s += "   <AlertSuCompletato>" + App.AppSetupAlertSuCompletato + "</AlertSuCompletato>";
            s += "   <TipoGestioneArchivio>" + App.AppSetupTipoGestioneArchivio + "</TipoGestioneArchivio>";
            s += "   <PathArchivioRemoto>" + App.AppPathArchivioRemoto + "</PathArchivioRemoto>";
            //4.6
            s += "   <TipoGestioneArchivio>" + App.AppSetupBackupPersonalizzato + "</TipoGestioneArchivio>";
            s += "   <PathBackupUtente>" + App.AppUserBackupFolder + "</PathBackupUtente>";
            s += "</RevisoftInfoFile>";
            //salvo file
            SalvaInfoRevisoft(s);

            //settaggi
            _StatoLicenza = true;
            //_StatoLicenzaCambiato = true;
            _LicenzaProvaDisponibile = false;

            //Configura licenza
            ConfiguraLicenza();

            //Salvo info in master file
            //if (App.TipoLicenza != App.TipologieLicenze.ClientLan && App.TipoLicenza != App.TipologieLicenze.ClientLanMulti)
            {
                MasterFile mf = MasterFile.Create();
                mf.SetChiaveServer(App.CodiceMacchinaServer.Split('-')[0]);
                mf.SetDataLicenza(_DataAttivazioneLicenza.ToString());
            }

            //esci
            return true;
        }



        public bool VerificaCodiceMacchinaFileImportato(string importCodiceMacchinaServer, string importCodiceMacchina)
        {

            if (importCodiceMacchinaServer == App.CodiceMacchinaServer || importCodiceMacchinaServer.Split('-')[0] == App.CodiceMacchinaServer.Split('-')[0])
                return true;

            if ((importCodiceMacchinaServer != App.CodiceMacchinaServer && importCodiceMacchina == App.CodiceMacchina) || (importCodiceMacchinaServer.Split('-')[0] != App.CodiceMacchinaServer.Split('-')[0] && importCodiceMacchina.Split('-')[0] == App.CodiceMacchina.Split('-')[0]))
                return true;


            //ERRORE: codice macchina non corrispondente
            App.ErrorLevel = App.ErrorTypes.Errore;
            RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
            m.TipoMessaggioErrore = WindowGestioneMessaggi.TipologieMessaggiErrore.ErroreCodiceMacchinaImportazioneFile;
            m.VisualizzaMessaggio();
            return false;
        }


        public string GeneraFileFiligrana()
        {
            //info licenza
            string nf = App.AppTempFolder + "\\{" + Guid.NewGuid().ToString() + "}.png";
            string buff = "Licenziatario: " + _Intestatario + "  Utilizzatore: " + _Utente + "  Licenza numero: " + _Info_CodiceMacchina.Split('-')[0]; // "REVISOFT - Revisione Legale" +
            //genero file
            Utilities u = new Utilities();
            u.StringToImage(buff, nf);
            //ritorno nome file
            return nf;
        }


        public string NomeLicenza(App.TipologieLicenze licenza)
        {
            string buff = "";

            switch (licenza)
            {
                //PROVA
                case App.TipologieLicenze.Prova:
                    buff = "Prova";
                    break;
                //DESKTOP - standard revisoft
                case App.TipologieLicenze.DeskTop:
                    buff = "Standard";
                    break;
                //SERVER - rete revisoft
                case App.TipologieLicenze.Server:
                    buff = "Rete";
                    break;
                //CLIENT LAN
                case App.TipologieLicenze.ClientLan:
                case App.TipologieLicenze.ClientLanMulti:
                    buff = "Client";
                    break;
                //ENTRY LEVEL
                case App.TipologieLicenze.EntryLevel:
                    buff = "Entry Level";
                    break;
                //VIEWER
                case App.TipologieLicenze.Viewer:
                    buff = "Satellite";
                    break;
                //GUEST
                case App.TipologieLicenze.Guest:
                    buff = "Guest";
                    break;
                //SIGILLO
                case App.TipologieLicenze.Sigillo:
                    buff = "Sigillo";
                    break;
            }

            return buff;
        }


    }
}
