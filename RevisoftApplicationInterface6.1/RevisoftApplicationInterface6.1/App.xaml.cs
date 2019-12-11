//----------------------------------------------------------------------------+
//                                App.xaml.cs                                 |
//----------------------------------------------------------------------------+
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using System.Xml;
using RevisoftApplication.BRL;
namespace RevisoftApplication
{
  // E.B. nuova struttura per cache XML
  public struct XMLELEMENT
  {
    public XmlDocument doc;
    public bool isModified;
    public void Reset() { doc = null; isModified = false; }
    public XMLELEMENT(XmlDocument d, bool isMod) { doc = d; isModified = isMod; }
  }
  public partial class App : Application
  {
    //---------------------------------- personalizzazione colori (se presente)
    public static Brush[] _arrBrushes = new Brush[14];
    public static string _logoPath = string.Empty;
    public const string DEFAULT_COLOR_SESSION_SELECTED = "#AA82BDE4";
    public static string COLOR_SESSION_SELECTED = "#AA82BDE4";

    #region constants

    // costanti
    public const string ApplicationFileName = "RevisoftApp";
    public const string ApplicationFolder = "Revisoft";
    public const string BackUpFolder = "Backup";
    public const string Bilancio_Attivo = "Bilancio_Attivo";
    public const string Bilancio_Attivo2016 = "Bilancio_Attivo_2016";
    public const string Bilancio_Attivo2016_Consolidato = "Bilancio_Attivo_2016_Consolidato";
    public const string Bilancio_ContoEconomico = "Bilancio_ContoEconomico";
    public const string Bilancio_ContoEconomico2016 = "Bilancio_ContoEconomico_2016";
    public const string Bilancio_ContoEconomico2016_Consolidato = "Bilancio_ContoEconomico_2016_Consolidato";
    public const string Bilancio_Passivo = "Bilancio_Passivo";
    public const string Bilancio_Passivo2016 = "Bilancio_Passivo_2016";
    public const string Bilancio_Passivo2016_Consolidato = "Bilancio_Passivo_2016_Consolidato";
    public const string Bilancio_Riclassificato = "BilancioRiclassificato";
    public const string Bilancio_Riclassificato2016 = "BilancioRiclassificato_2016";
    public const string Bilancio_Riclassificato2016_Consolidato = "BilancioRiclassificato_2016_Consolidato";
    public const string BilancioAbbreviato_Attivo = "BilancioAbbreviato_Attivo";
    public const string BilancioAbbreviato_Attivo2016 = "BilancioAbbreviato_Attivo_2016";
    public const string BilancioAbbreviato_ContoEconomico = "BilancioAbbreviato_ContoEconomico";
    public const string BilancioAbbreviato_ContoEconomico2016 = "BilancioAbbreviato_ContoEconomico_2016";
    public const string BilancioAbbreviato_Passivo = "BilancioAbbreviato_Passivo";
    public const string BilancioAbbreviato_Passivo2016 = "BilancioAbbreviato_Passivo_2016";
    public const string BilancioAbbreviato_Riclassificato = "BilancioAbbreviatoRiclassificato";
    public const string BilancioAbbreviato_Riclassificato2016 = "BilancioAbbreviatoRiclassificato_2016";
    public const string BilancioMicro_Attivo2016 = "BilancioMicro_Attivo_2016";
    public const string BilancioMicro_ContoEconomico2016 = "BilancioMicro_ContoEconomico_2016";
    public const string BilancioMicro_Passivo2016 = "BilancioMicro_Passivo_2016";
    public const string BilancioMicro_Riclassificato2016 = "BilancioMicroRiclassificato_2016";
    public const string ClientiEsportatiFolder = "ClientiEsportati";
    public const string DataFolder = "DataFile";
    public const string DocNameBilancio = "Bilancio";
    public const string DocNameBilancioDati = "DatiBilancio";
    public const string DocNameConclusione = "Conclusioni";
    public const string DocNameConclusioneDati = "DatiConclusioni";
    public const string DocNameFlussi = "Flussi";
    public const string DocNameFormulario = "Formulario";
    public const string DocNameFormularioDati = "DatiFormulario";
    public const string DocNameIncarico = "Incarico";
    public const string DocNameIncaricoDati = "DatiIncarico";
    public const string DocNameISQC = "ISQC";
    public const string DocNameISQCDati = "DatiISQC";
    public const string DocNameModelli = "ModelliPredefiniti";
    public const string DocNameModelloStampa = "PrintTemplate";
    public const string DocNameModelloStampaBilancio = "PrintTemplateBilancio";
    public const string DocNameModelloStampaNoLogo = "PrintTemplateNoLogo";
    public const string DocNamePianificazioniVerifica = "PianificazioniVerifiche";
    public const string DocNamePianificazioniVerificaDati = "DatiPianificazioniVerifiche";
    public const string DocNamePianificazioniVigilanza = "PianificazioniVigilanze";
    public const string DocNamePianificazioniVigilanzaDati = "DatiPianificazioniVigilanze";
    public const string DocNameRelazioneB = "RelazioneB";
    public const string DocNameRelazioneBC = "RelazioneBC";
    public const string DocNameRelazioneBCDati = "DatiRelazioneBC";
    public const string DocNameRelazioneBDati = "DatiRelazioneB";
    public const string DocNameRelazioneBV = "RelazioneBV";
    public const string DocNameRelazioneBVDati = "DatiRelazioneBV";
    public const string DocNameRelazioneV = "RelazioneV";
    public const string DocNameRelazioneVC = "RelazioneVC";
    public const string DocNameRelazioneVCDati = "DatiRelazioneVC";
    public const string DocNameRelazioneVDati = "DatiRelazioneV";
    public const string DocNameRevisione = "Revisione";
    public const string DocNameRevisioneDati = "DatiRevisione";
    public const string DocNameVerifica = "Verifica";
    public const string DocNameVerificaDati = "DatiVerifica";
    public const string DocNameVigilanza = "Vigilanza";
    public const string DocNameVigilanzaDati = "DatiVigilanza";
    public const string FormularioFolder = "Formulario";
    public const string IndiceTemplateFileName = "Template";
    public const string Lead = "lead";
    public const string Lead2016 = "lead_2016";
    public const string LicenseFolder = "RLF";
    public const string LogFolder = "Log";
    public const int MasterFile_NewID = -1;
    public const string ModelliFolder = "Modelli";
    public const string NewTabHeaderText = " + ";
    public const string OldLogFolder = "Old";
    public const string TemplateFolder = "Template";
    public const string TemplateFolderVersioni = "Versioni";
    public const string UpdateCommand = "RevisoftUpdate.bat";
    public const string urlCheckConnection = "http://www.revisoft.it/";
    public const string urlNoteRilascio = "http://www.revisoft.it/versioni.pdf";
    public const string UserFileFlussiFolder = "Flussi";
    public const string UserFileFolder = "UserDoc";
    public const string XBRL = "xbrl";
    public const string XBRL2016 = "xbrl_2016";
    public const string ZipFilePassword = "Datalabor.com";
    // costanti registro
    public const string Registry_AlertSuCompletato = "AlertSuCompletato";
    public const string Registry_Benvenuto = "Benvenuto";
    public const string Registry_IstruzioniAutomatiche = "IstruzioniAutomatiche";
    public const string Registry_PathArchivioRemoto = "PathArchivioRemoto";
    public const string Registry_TipoGestioneArchivio = "TipoGestioneArchivio";
    // 4.6 Versione x about e aggiornamento archivi remoto
    public const string AppVersioneAbout = "6.1.0";
    public const string AppVersionePrecedente = "6.0.0";
    #endregion

    #region variables

    //Variabili
    public static string MessaggioSolaScrittura = "Occorre selezionare Sblocca Stato per modificare il contenuto.";
    public static string MessaggioSolaScritturaStato = "Sessione in sola lettura, impossibile modificare lo stato.";
    public static ErrorTypes ErrorLevel;
    public static string CodiceMacchina;
    public static string CodiceMacchinaServer;
    public static bool Scaduta;

    //public static TipologieLicenze                    TipoLicenza; //settata da classe cGestionelicenza
    //public static TipologieLicenze                    TipoLicenzaSigillo; //SIGILLO

    //Variabili funzionalità - vers. 4.5
    public static bool Multilicenza;
    public static bool Sigillo;
    public static bool Prova;
    public static bool Guest;
    public static bool Server;
    public static bool Cloud;
    public static bool RemoteDesktop;
    public static bool Client;
    public static int NumeroanAgrafiche;
    public static int NumeroLicenze;

    //Variabili di configurazione
    public static bool AppSetupBenvenuto;
    public static bool AppSetupIstruzioniAutomatiche;
    public static bool AppSetupAlertSuCompletato;
    public static TipoGestioneArchivio AppSetupTipoGestioneArchivio;
    public static string AppPathArchivioRemoto;
    public static DateTime AppInizioSessione;
    public static string AppNome;
    public static string AppVersione;
    public static bool AppSetupNuovaVersione;
    public static bool AppSetupScaricaNuovaVersione;
    public static int AppIstanzeAttive;
    public static bool AppSetupBackupPersonalizzato;
    public static bool AppSetupAddioBackupRevisoft;
    public static bool AppSetupAddioBackupUtente;

    //Variabili attivazioni funzioanlità/versioni licenza ***** ricordarsi di settarle in GestioneLicenza.Configuralicenza() ********
    public static bool AppConsentiImportaEsporta;
    public static bool AppConsentiCreazioneAnagrafica;
    public static bool AppConsentiImportazioneEsportazioneLan;
    public static bool AppConsentiAccessoArchivioLocale;
    public static bool AppConsentiAccessoArchivioRemoto;
    public static bool AppConsentiAccessoArchivioCloud;
    public static bool AppConsentiGestioneArchivioRemoto;
    public static bool AppConsentiGestioneBackUp;
    public static bool AppConsentiBackUp;
    public static bool AppConsentiMultiLicenza;
    public static bool AppConsentiSigillo;

    //Path di sistema
    public static string AppProgramFolder;         //Program file \ revisoft \ revisoft
    public static string AppLicenseFolder;         //Program file \ revisoft \ revisoft \ Licenze
    public static string AppDataFolder;            //User \ AppData \ Roaming \ revisoft \ revisoft
    public static string AppTeamDataFolder;        // opzione da INI per override di AppDataFolder e AppLocalDataFolder
    public static string AppDataDataFolder;        //User \ AppData \ Roaming \ revisoft \ revisoft \ RDF
    public static string AppTemplateFolder;        //User \ AppData \ Roaming \ revisoft \ revisoft \ template
    public static string AppBackupFolder;          //User \ AppData \ Roaming \ revisoft \ revisoft \ Backup
    public static string AppFormularioFolder;      //User \ AppData \ Roaming \ revisoft \ revisoft \ Formulario
    public static string AppModelliFolder;         //User \ AppData \ Roaming \ revisoft \ revisoft \ Modelli
    public static string AppDocumentiFolder;       //User \ AppData \ Roaming \ revisoft \ revisoft \ UserDoc
    public static string AppDocumentiFlussiFolder; //User \ AppData \ Roaming \ revisoft \ revisoft \ UserDoc \ Flussi
    public static string AppLocalDataFolder;       //User \ AppData \ Roaming \ revisoft \ revisoft
    public static string AppLogFolder;             //User \ AppData \ Roaming \ revisoft \ revisoft \ Log
    public static string AppOldLogFolder;          //User \ AppData \ Roaming \ revisoft \ revisoft \ Log \ Old
    public static string AppTempFolder;            //cartella temp di sistema
    public static string AppUserBackupFolder;      //User \ AppData \ Roaming \ revisoft \ revisoft \ Backup
    //4.6 percorsi backup personalizzato
    public static string AppBackupFolderUser;
    public static string AppBackUpDataFileUser;

    //Autoexec - per le attività che richiedono interfaccia
    public static bool AppAutoExec;
    public static TipoFile AppAutoExecTipoFile;
    public static string AppAutoExecFileName;
    public static TipoFunzioniAutoexec AppAutoExecFunzione;
    public static bool AppForzaAttivazioneLicenza;
    public static bool AppTestDownload;

    //File di sistema
    public static string AppInfoFile;
    public static string AppInfoFile_OLD;
    public static string AppLicenseFile;
    public static string AppLicenseFile_OLD;
    public static string AppLicenseSigilloFile;
    public static string AppLicenseSigilloFile_OLD;
    public static string AppHelpFile;
    public static string AppMessageFile;

    //File dati di sistema
    public static string AppFormularioFile;
    public static string AppFormularioFileDati;
    public static string AppModelliFile;
    public static string AppLogDataFile;

    //File template - utilizzati quanto creo nuovo doc
    public static string AppTemplateTreeIncarico;
    public static string AppTemplateTreeISQC;
    public static string AppTemplateTreeVerifica;
    public static string AppTemplateTreePianificazioniVerifica;
    public static string AppTemplateTreeRevisione;
    public static string AppTemplateTreeBilancio;
    public static string AppTemplateTreeConclusione;
    public static string AppTemplateDataIncarico;
    public static string AppTemplateDataISQC;
    public static string AppTemplateDataVerifica;
    public static string AppTemplateDataPianificazioniVerifica;
    public static string AppTemplateDataRevisione;
    public static string AppTemplateDataBilancio;
    public static string AppTemplateDataConclusione;
    public static string AppTemplateTreeVigilanza;
    public static string AppTemplateDataVigilanza;
    public static string AppTemplateTreePianificazioniVigilanza;
    public static string AppTemplateDataPianificazioniVigilanza;

    public static string AppTemplateTreeRelazioneB;
    public static string AppTemplateDataRelazioneB;
    public static string AppTemplateTreeRelazioneV;
    public static string AppTemplateDataRelazioneV;

    public static string AppTemplateTreeRelazioneBC;
    public static string AppTemplateDataRelazioneBC;
    public static string AppTemplateTreeRelazioneVC;
    public static string AppTemplateDataRelazioneVC;

    public static string AppTemplateTreeRelazioneBV;
    public static string AppTemplateDataRelazioneBV;

    public static string AppTemplateDataFlussi;

    //File template - Stampa
    public static string AppTemplateStampa;
    public static string AppTemplateStampaNoLogo;
    public static string AppTemplateStampaBilancio;

    //File template - Bilanci
    public static string AppTemplateBilancio_Attivo;
    public static string AppTemplateBilancio_ContoEconomico;
    public static string AppTemplateBilancio_Passivo;
    public static string AppTemplateBilancio_Riclassificato;

    public static string AppTemplateBilancioAbbreviato_Attivo;
    public static string AppTemplateBilancioAbbreviato_ContoEconomico;
    public static string AppTemplateBilancioAbbreviato_Passivo;
    public static string AppTemplateBilancioAbbreviato_Riclassificato;

    public static string AppTemplateBilancio_Attivo2016;
    public static string AppTemplateBilancio_ContoEconomico2016;
    public static string AppTemplateBilancio_Passivo2016;
    public static string AppTemplateBilancio_Riclassificato2016;

    public static string AppTemplateBilancio_Attivo2016_Consolidato;
    public static string AppTemplateBilancio_ContoEconomico2016_Consolidato;
    public static string AppTemplateBilancio_Passivo2016_Consolidato;
    public static string AppTemplateBilancio_Riclassificato2016_Consolidato;

    public static string AppTemplateBilancioAbbreviato_Attivo2016;
    public static string AppTemplateBilancioAbbreviato_ContoEconomico2016;
    public static string AppTemplateBilancioAbbreviato_Passivo2016;
    public static string AppTemplateBilancioAbbreviato_Riclassificato2016;

    public static string AppTemplateBilancioMicro_Attivo2016;
    public static string AppTemplateBilancioMicro_ContoEconomico2016;
    public static string AppTemplateBilancioMicro_Passivo2016;
    public static string AppTemplateBilancioMicro_Riclassificato2016;

    public static string AppXBRL;
    public static string AppLEAD;

    public static string AppXBRL2016;
    public static string AppLEAD2016;

    //File dati utente
    public static string AppMasterDataFile;
    public static string AppBackUpDataFile;
    public static string AppDocumentiDataFile;

    //Versione Team
    public static ModalitaApp AppTipo;
    public static RuoloDesc AppRuolo;
    public static Utente AppUtente;
    #endregion

    #region enum's

    //Enum
    public enum TipologieLicenze { Ignota = 0, Prova = 1, Server = 2, DeskTop = 3, EntryLevel = 4, ClientLan = 5, Viewer = 6, Guest = 7, ClientLanMulti = 8, Sigillo = 9, Scaduta = 99 };
    public enum TipoGestioneArchivio { Locale = 0, Remoto = 1, Cloud = 2, LocaleImportExport = 3 };
    public enum ErrorTypes { Nessuno = 0, Segnalazione = 1, Avviso = 2, Errore = 3, ErroreBloccante = 4 };
    public enum TipoFile { Licenza = 0, Revisione = 1, Verifica = 2, Incarico = 3, IncaricoCS = 71, IncaricoSU = 72, IncaricoREV = 73, Bilancio = 4, Master = 5, Info = 6, Messagi = 7, ImportExport = 8, ImportTemplate = 9, BackUp = 10, Formulario = 11, ModellPredefiniti = 12, DocumentiAssociati = 13, ScambioDati = 14, DocTemplate = 15, RevisoftXML = 16, XBRL = 17, Vigilanza = 18, Conclusione = 19, Sigillo = 20, RelazioneB = 21, RelazioneV = 22, RelazioneBV = 23, Log = 24, IndiceTemplate = 25, PianificazioniVerifica = 26, PianificazioniVigilanza = 27, ISQC = 28, BilancioDiVerifica = 29, RelazioneBC = 31, RelazioneVC = 32, Flussi = 99 };
    public enum TipoAttivitaScheda { New = 0, Edit = 1, View = 2, Delete = 3, Export = 4, Condividi = 5 }
    public enum TipoAnagraficaStato { Sconosciuto = -1, Disponibile = 0, InUso = 1, Bloccato = 2, Esportato = 3 }
    public enum TipoAnagraficaEsercizio { Sconosciuto = -1, AnnoSolare = 0, ACavallo = 1 }
    public enum TipoIncaricoComposizione { Sconosciuto = -1, CollegioSindacale = 0, Revisore = 1, SindacoUnico = 2 }
    public enum TipoIncaricoAttivita { Sconosciuto = -1, Nomina = 0, Riesame = 1 }
    public enum TipoISQCComposizione { Sconosciuto = -1, CollegioSindacale = 0, Revisore = 1, SindacoUnico = 2 }
    public enum TipoISQCAttivita { Sconosciuto = -1, Nomina = 0, Riesame = 1 }
    public enum TipoSessioneStato { Sconosciuto = -1, Disponibile = 0, InUso = 1, Bloccato = 2, Esportato = 3 }
    public enum TipoTreeNodeStato { SigilloRotto = -6, Sigillo = -5, SolaLettura = -4, VociCompilate = -3, NodoFazzoletto = -2, Sconosciuto = -1, NonApplicabile = 0, DaCompletare = 1, Completato = 2, CancellaDati = 3, Scrittura = 4, Report = 5, NonApplicabileBucoTemplate = 6, CompletatoBloccoEsecutore = 7 }
    public enum TipoAttivita { Sconosciuto = -1, Incarico = 0, IncaricoCS = 71, IncaricoSU = 72, IncaricoREV = 73, Revisione = 1, Bilancio = 2, Verifica = 3, Vigilanza = 18, Conclusione = 19, RelazioneB = 21, RelazioneV = 22, RelazioneBV = 23, PianificazioniVerifica = 26, PianificazioniVigilanza = 27, ISQC = 28, RelazioneBC = 31, RelazioneVC = 32, Flussi = 99 }
    public enum TipoScambioDati { Sconosciuto = -1, Esporta = 0, Importa = 1 };
    public enum TipoFunzioniAutoexec { Sconosciuto = -1, ScambioDati = 0, NuovoCliente = 1, SetupLan = 2, ImportExport = 3, ImportTemplate = 4, Restore = 5 };

    //Versione Team
    public enum ModalitaApp { StandAlone = 0, Team = 1, Administrator = 2 };
    public enum RuoloDesc { NessunRuolo = 0, Administrator = 1, TeamLeader = 2, Reviewer = 3, Esecutore = 4, StandAlone = 5, RevisoreAutonomo = 6 };
    public enum TipoAbilitazioneWindow { TuttoAbilitato = 0, TuttoDisabilitato = 1, AbilitaPerTeamLeader = 2, AbilitaPerReviewer = 3, AbilitaPerReviewerBloccato = 4, AbilitaPerEsecutore = 5, TuttoDisabilitatoPerReviewer = 6 };
    #endregion

    // sql - nuove definizioni - inizio
    public enum TipiOggetto
    {
      BILANCIO = 1, CONCLUSIONE = 2, INCARICO = 3, ISQC = 4, PIANIFICAZIONIVERIFICA = 5,
      PIANIFICAZIONIVIGILANZA = 6, RELAZIONEB = 7, RELAZIONEBV = 8, RELAZIONEV = 9,
      REVISIONE = 10, VERIFICA = 11, VIGILANZA = 12
    }
    public static string connString = ""; // valorizzato in App()
    //public static SqlConnection sqlConnection;
    //public static string[] clientiFields =
    //{
    //  "ID","Stato","Note","EsercizioAl","EsercizioDal","Esercizio",
    //  "CodiceFiscale","PartitaIVA","RagioneSociale","Presidente",
    //  "MembroEffettivo","MembroEffettivo2","RevisoreAutonomo",
    //  "OrganoDiControllo","OrganoDiRevisione","SindacoSupplente",
    //  "SindacoSupplente2","DataModificaStato","UtenteModificaStato"
    //};
    //public static string[] incarichiFields =
    //{
    //  "ID","Cliente","Stato","File","FileData","Note","DataNomina","Composizione","Attivita"
    //};
    public static bool m_bxmlCacheEnable = true;
    public static bool m_bNoExceptionMsg = true;
    public static int m_cacheMax = 200;
    public static Hashtable m_xmlCache = new Hashtable(); // contiene XMLELEMENT
    public static int m_CommandTimeout = 0; // in secondi, 0=no limit
    public const string MOD_ATTRIB = "nodeModified";
    public const string OBJ_NEW = "new";
    public const string OBJ_MOD = "mod";
    public const string TMP_FOLDER = @"c:\";
    public const string BK_DECODED_PREFIX = "_decoded_";
    // sql - nuove definizioni - fine

    //costruttore
    public App()
    {
      String thisprocessname = Process.GetCurrentProcess().ProcessName;

      // if (Process.GetProcesses().Count(p => p.ProcessName == thisprocessname) > 1)
      //     System.Environment.Exit(1);

      string user, catalog, server, iniFilePath;

      iniFilePath =
        System.IO.Path.GetFullPath(
          System.Reflection.Assembly.GetEntryAssembly().Location);
      iniFilePath = iniFilePath.Replace(".exe", ".ini");
      INIFile iniFile = new INIFile(iniFilePath);
      user = Environment.UserName;
      server = iniFile.Read(user, "server");

      if (server == "")
        server = FindSQL();

      catalog = iniFile.Read(user, "catalog");
      if (catalog == "") catalog = "Revisoft_" + user;
      //catalog = "Revisoft_"+user;  // solo temporaneamente per fare i test
      iniFile.Write(user, "server", server);
      iniFile.Write(user, "catalog", catalog);
      // lettura combinazione colori da INI
      SetupColors(ref iniFile, user);

      //----------------------------------------------------------------------------+
      //                   verifica connessione a database utente                   |
      //----------------------------------------------------------------------------+
      connString = string.Format(
        "Data Source={0};Initial Catalog={1};Integrated Security=True", server, catalog);
      using (SqlConnection conn = new SqlConnection(connString))
      {
        try { conn.Open(); }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
          Current.Shutdown();
        }
      }
      RevisoftApplication.Utilities u = new Utilities();
      //Settaggi applicativi
      App.AppAutoExec = false;
      App.ErrorLevel = App.ErrorTypes.Nessuno;
      //andrea 2.9 modifica necessaria per XP
      if (Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) == "")
      {
        //PATH Programma - Win XP
        App.AppProgramFolder = Environment.GetEnvironmentVariable("ProgramFiles") + "\\" + ApplicationFolder + "\\" + ApplicationFolder;
      }
      else
      {
        //PATH Programma - Win 7
        App.AppProgramFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\" + ApplicationFolder + "\\" + ApplicationFolder;
      }
      App.AppLocalDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + App.ApplicationFolder + "\\" + App.ApplicationFolder;
      //andrea 3.0 - multilicenza
      App.AppLicenseFolder = App.AppLocalDataFolder + "\\" + App.LicenseFolder;
      //FILE INFO
      App.AppInfoFile = App.AppLocalDataFolder + "\\" + ApplicationFileName + u.EstensioneFile(TipoFile.Info);
      App.AppInfoFile_OLD = App.AppProgramFolder + "\\" + ApplicationFileName + u.EstensioneFile(TipoFile.Info);
      //FILE LICENZA
      App.AppLicenseFile = App.AppLocalDataFolder + "\\" + ApplicationFileName + u.EstensioneFile(TipoFile.Licenza);
      App.AppLicenseFile_OLD = App.AppProgramFolder + "\\" + ApplicationFileName + u.EstensioneFile(TipoFile.Licenza);
      //FILE LICENZA SIGILLO
      App.AppLicenseSigilloFile = App.AppLocalDataFolder + "\\" + ApplicationFileName + u.EstensioneFile(TipoFile.Sigillo);
      App.AppLicenseSigilloFile_OLD = App.AppProgramFolder + "\\" + ApplicationFileName + u.EstensioneFile(TipoFile.Sigillo);
      //FILE HELP
      App.AppHelpFile = RevisoftApplication.Properties.Settings.Default["RevisoftApplicationGuide"].ToString();
      //FILE MESSAGGI
      App.AppMessageFile = App.AppProgramFolder + "\\" + ApplicationFileName + u.EstensioneFile(TipoFile.Messagi);
      //Cartella TEMP
      AppTempFolder = System.IO.Path.GetTempPath();
      AppTeamDataFolder = iniFile.Read(user, "appdatafolder");
      if (!string.IsNullOrEmpty(AppTeamDataFolder))
      {
        App.AppDataFolder = AppTeamDataFolder;

        App.AppLocalDataFolder = AppDataFolder + "\\" + App.ApplicationFolder + "\\" + App.ApplicationFolder;
        App.AppLicenseFolder = AppDataFolder + "\\" + App.LicenseFolder;
        App.AppInfoFile = AppDataFolder + "\\" + ApplicationFileName + u.EstensioneFile(TipoFile.Info);
        App.AppLicenseFile = AppDataFolder + "\\" + ApplicationFileName + u.EstensioneFile(TipoFile.Licenza);
        App.AppLicenseSigilloFile = AppDataFolder + "\\" + ApplicationFileName + u.EstensioneFile(TipoFile.Sigillo);
        App.AppDocumentiFlussiFolder = AppDataFolder + "\\" + App.UserFileFolder + "\\" + App.UserFileFlussiFolder;
        AppDocumentiFolder = AppDataFolder + "\\" + App.UserFileFolder;
        AppPathArchivioRemoto = AppDataFolder;
      }
    }

    private void SetupColors(ref INIFile iniFile, string user)
    {
      string[] arrDefaults =
      {
        "#FFF5A41C","#FFF7A827","#FF526573","DarkGray","White",
        "#7EF1F1F1","#7ED3D3D3","#7E82BDE4","#7EF5A41C","Black",
        "#AA82BDE4","#7EF5A41C","#FF407192","#FF8BA2B2"
      };
      int i;
      string str, key;
      BrushConverter converter = new BrushConverter();

      if (iniFile == null || string.IsNullOrEmpty(user)) return;
      for (i = 0; i < _arrBrushes.Length; i++)
      {
        key = string.Format("Color{0}", (i + 1).ToString("D2"));
        str = iniFile.Read(user, key).Trim().Replace(" ", "");
        if (string.IsNullOrEmpty(str)) str = arrDefaults[i];
        str = str.ToUpper();
        _arrBrushes[i] = (Brush)converter.ConvertFromString(str);
        iniFile.Write(user, key, str);
        if (i == 10) COLOR_SESSION_SELECTED = str;
      }
      _logoPath = iniFile.Read(user, "logo");
      if (string.IsNullOrEmpty(_logoPath))
      {
        iniFile.Write(user, "logo", ""); return;
      }
      if (!File.Exists(_logoPath)) _logoPath = string.Empty;
    }

    private string FindSQL()
    {
      string str = string.Empty;
      IEnumerable<string> lista = SqlHelper.ListLocalSqlInstances();
      foreach (string istanza in lista)
      {
        if (istanza.IndexOf("SQLREVISOFT") != -1)
        {
          str = istanza;
          break;
        }
      }

      return str;
    }
    //Gestione parametri applicativi su riga di comando
    protected override void OnStartup(StartupEventArgs e)
    {
      //Conto le istanze attive Revisoft
      Process thisProc = Process.GetCurrentProcess();
      Process[] prx = Process.GetProcessesByName(thisProc.ProcessName);
      foreach (Process item in prx)
      {
        if (thisProc.MachineName == item.MachineName)
        {
          App.AppIstanzeAttive++;
        }
      }
      //// Verifico la presenza di una precedente istanza di Revisoft ed esco se presente
      //Process thisProc = Process.GetCurrentProcess();
      //if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
      //{
      //    MessageBox.Show("Revisoft è già in esecuzione.\nImpossibile eseguire il programma nuovamente.");
      //    Application.Current.Shutdown();
      //    return;
      //}
      RevisoftApplication.Utilities u = new Utilities();
      foreach (string arg in e.Args)
      {
        switch (arg)
        {
          case "/setup":
            u.ConfiguraRegistroApplicazione();
            break;
          case "/lan":
            App.AppAutoExec = true;
            App.AppAutoExecFunzione = TipoFunzioniAutoexec.SetupLan;
            break;
          case "/licenza":
            App.AppForzaAttivazioneLicenza = true;
            break;
          case "/key":
            App.AppAutoExec = true;
            if (u.LeggiInfoMacchina())
            {
              //copio codice macchina in clipboard
              Clipboard.Clear();
              Clipboard.SetText(App.CodiceMacchina);
              MessageBox.Show("Codice macchina:\t" + App.CodiceMacchina + "\n\nInformazione inserita in clipboard.");
            }
            u.ChiudiApplicazione();
            break;
          case "/test":
            App.AppTestDownload = true;
            break;
          case "/help":
            MessageBox.Show("Funzione da riga di comando:\n/setup\tConfigura registro\n/lan\tconfigura archivio di rete\n/licenza\tdisattiva controlli licenza\n/key\tVisualizza codice macchina");
            break;
          //case "/init":
          ////resetto info licenza
          //GestioneLicenza lreset = new GestioneLicenza();
          //lreset.ResetInfoRevisoft();
          ////resetto applicazione
          //u.ConfiguraApplicazione();
          //u.ConfiguraPercorsi();
          //MasterFile m = MasterFile.Create();
          //m.ResetMasterFile();
          //break;
          default:
            string ext = arg.ToString().Substring(arg.ToString().LastIndexOf("."));
            //gestione licenza - senza interfaccia
            if (ext.ToLower() == u.EstensioneFile(TipoFile.Licenza))
            {
              App.AppAutoExec = false;
              RevisoftApplication.GestioneLicenza l = new GestioneLicenza();
              l.AttivaLicenzaDaFile(arg);
            }
            ////gestione licenza SIGILLO - senza interfaccia
            //if (ext == u.EstensioneFile(TipoFile.Sigillo))
            //{
            //    App.AppAutoExec = false;
            //    RevisoftApplication.GestioneLicenza l = new GestioneLicenza();
            //    l.AttivaSigilloDaFile(arg);
            //}
            //gestione scambio dati - con interfaccia su OnContentRendered
            if (ext.ToLower() == u.EstensioneFile(TipoFile.ScambioDati))
            {
              App.AppAutoExec = true;
              App.AppAutoExecTipoFile = App.TipoFile.ScambioDati;
              App.AppAutoExecFileName = arg.ToString();
              App.AppAutoExecFunzione = TipoFunzioniAutoexec.ScambioDati;
            }
            //gestione import export
            if (ext.ToLower() == u.EstensioneFile(TipoFile.ImportExport))
            {
              App.AppAutoExec = true;
              App.AppAutoExecTipoFile = App.TipoFile.ImportExport;
              App.AppAutoExecFileName = arg.ToString();
              App.AppAutoExecFunzione = TipoFunzioniAutoexec.ImportExport;
            }
            //gestione import export template
            if (ext.ToLower() == u.EstensioneFile(TipoFile.ImportTemplate))
            {
              App.AppAutoExec = true;
              App.AppAutoExecTipoFile = App.TipoFile.ImportTemplate;
              App.AppAutoExecFileName = arg.ToString();
              App.AppAutoExecFunzione = TipoFunzioniAutoexec.ImportTemplate;
            }
            //gestione Backup/Restore
            if (ext.ToLower() == u.EstensioneFile(TipoFile.BackUp))
            {
              App.AppAutoExec = true;
              App.AppAutoExecTipoFile = App.TipoFile.BackUp;
              App.AppAutoExecFileName = arg.ToString();
              App.AppAutoExecFunzione = TipoFunzioniAutoexec.Restore;
            }
            break;
        }
      }

      base.OnStartup(e);
    }

    //----------------------------------------------------------------------------+
    //                           NomeTipoTreeNodeStato                            |
    //----------------------------------------------------------------------------+
    public static string NomeTipoTreeNodeStato(TipoTreeNodeStato stato)
    {
      string buff = "";
      switch (stato)
      {
        //NodoFazzoletto
        case App.TipoTreeNodeStato.NodoFazzoletto:
          buff = "Promemoria";
          break;
        //NonApplicabile
        case App.TipoTreeNodeStato.NonApplicabileBucoTemplate:
        case App.TipoTreeNodeStato.NonApplicabile:
          buff = "Non Applicabile";
          break;
        //DaCompletare
        case App.TipoTreeNodeStato.DaCompletare:
          buff = "Da Completare";
          break;
        //Completato
        case App.TipoTreeNodeStato.Completato:
          buff = "Completato";
          break;
        //CancellaDati
        case App.TipoTreeNodeStato.CancellaDati:
          buff = "Resettato";
          break;
        //Scrittura
        case App.TipoTreeNodeStato.Scrittura:
          buff = "In Scrittura";
          break;
        //Sconosciuto
        case App.TipoTreeNodeStato.Sconosciuto:
        default:
          buff = "Nessuno stato assegnato";
          break;
      }
      return buff;
    }
    public static void GestioneLog(string messageLog)
    {
      // TO DO Gestione Log
      MessageBox.Show(messageLog);
    }
  } //---------------------------------- public partial class App : Application
} //--------------------------------------------- namespace RevisoftApplication

/*
// srcOld
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace RevisoftApplication
{
    public partial class App : Application
    {
        //------------------------------------------------------------ costanti
        public const string ApplicationFileName                     = "RevisoftApp";
        public const string ApplicationFolder                       = "Revisoft";
        public const string BackUpFolder                            = "Backup";
        public const string Bilancio_Attivo                         = "Bilancio_Attivo";
        public const string Bilancio_Attivo2016                     = "Bilancio_Attivo_2016";
        public const string Bilancio_Attivo2016_Consolidato         = "Bilancio_Attivo_2016_Consolidato";
        public const string Bilancio_ContoEconomico                 = "Bilancio_ContoEconomico";
        public const string Bilancio_ContoEconomico2016             = "Bilancio_ContoEconomico_2016";
        public const string Bilancio_ContoEconomico2016_Consolidato = "Bilancio_ContoEconomico_2016_Consolidato";
        public const string Bilancio_Passivo                        = "Bilancio_Passivo";
        public const string Bilancio_Passivo2016                    = "Bilancio_Passivo_2016";
        public const string Bilancio_Passivo2016_Consolidato        = "Bilancio_Passivo_2016_Consolidato";
        public const string Bilancio_Riclassificato                 = "BilancioRiclassificato";
        public const string Bilancio_Riclassificato2016             = "BilancioRiclassificato_2016";
        public const string Bilancio_Riclassificato2016_Consolidato = "BilancioRiclassificato_2016_Consolidato";
        public const string BilancioAbbreviato_Attivo               = "BilancioAbbreviato_Attivo";
        public const string BilancioAbbreviato_Attivo2016           = "BilancioAbbreviato_Attivo_2016";
        public const string BilancioAbbreviato_ContoEconomico       = "BilancioAbbreviato_ContoEconomico";
        public const string BilancioAbbreviato_ContoEconomico2016   = "BilancioAbbreviato_ContoEconomico_2016";
        public const string BilancioAbbreviato_Passivo              = "BilancioAbbreviato_Passivo";
        public const string BilancioAbbreviato_Passivo2016          = "BilancioAbbreviato_Passivo_2016";
        public const string BilancioAbbreviato_Riclassificato       = "BilancioAbbreviatoRiclassificato";
        public const string BilancioAbbreviato_Riclassificato2016   = "BilancioAbbreviatoRiclassificato_2016";
        public const string BilancioMicro_Attivo2016                = "BilancioMicro_Attivo_2016";
        public const string BilancioMicro_ContoEconomico2016        = "BilancioMicro_ContoEconomico_2016";
        public const string BilancioMicro_Passivo2016               = "BilancioMicro_Passivo_2016";
        public const string BilancioMicro_Riclassificato2016        = "BilancioMicroRiclassificato_2016";
        public const string ClientiEsportatiFolder                  = "ClientiEsportati";
        public const string DataFolder                              = "DataFile";
        public const string DocNameBilancio                         = "Bilancio";
        public const string DocNameBilancioDati                     = "DatiBilancio";
        public const string DocNameConclusione                      = "Conclusioni";
        public const string DocNameConclusioneDati                  = "DatiConclusioni";
        public const string DocNameFlussi                           = "Flussi";
        public const string DocNameFormulario                       = "Formulario";
        public const string DocNameFormularioDati                   = "DatiFormulario";
        public const string DocNameIncarico                         = "Incarico";
        public const string DocNameIncaricoDati                     = "DatiIncarico";
        public const string DocNameISQC                             = "ISQC";
        public const string DocNameISQCDati                         = "DatiISQC";
        public const string DocNameModelli                          = "ModelliPredefiniti";
        public const string DocNameModelloStampa                    = "PrintTemplate";
        public const string DocNameModelloStampaBilancio            = "PrintTemplateBilancio";
        public const string DocNameModelloStampaNoLogo              = "PrintTemplateNoLogo";
        public const string DocNamePianificazioniVerifica           = "PianificazioniVerifiche";
        public const string DocNamePianificazioniVerificaDati       = "DatiPianificazioniVerifiche";
        public const string DocNamePianificazioniVigilanza          = "PianificazioniVigilanze";
        public const string DocNamePianificazioniVigilanzaDati      = "DatiPianificazioniVigilanze";
        public const string DocNameRelazioneB                       = "RelazioneB";
        public const string DocNameRelazioneBC                      = "RelazioneBC";
        public const string DocNameRelazioneBCDati                  = "DatiRelazioneBC";
        public const string DocNameRelazioneBDati                   = "DatiRelazioneB";
        public const string DocNameRelazioneBV                      = "RelazioneBV";
        public const string DocNameRelazioneBVDati                  = "DatiRelazioneBV";
        public const string DocNameRelazioneV                       = "RelazioneV";
        public const string DocNameRelazioneVC                      = "RelazioneVC";
        public const string DocNameRelazioneVCDati                  = "DatiRelazioneVC";
        public const string DocNameRelazioneVDati                   = "DatiRelazioneV";
        public const string DocNameRevisione                        = "Revisione";
        public const string DocNameRevisioneDati                    = "DatiRevisione";
        public const string DocNameVerifica                         = "Verifica";
        public const string DocNameVerificaDati                     = "DatiVerifica";
        public const string DocNameVigilanza                        = "Vigilanza";
        public const string DocNameVigilanzaDati                    = "DatiVigilanza";
        public const string FormularioFolder                        = "Formulario";
        public const string IndiceTemplateFileName                  = "Template";
        public const string Lead                                    = "lead";
        public const string Lead2016                                = "lead_2016";
        public const string LicenseFolder                           = "RLF";
        public const string LogFolder                               = "Log";
        public const int MasterFile_NewID                           = -1;
        public const string ModelliFolder                           = "Modelli";
        public const string NewTabHeaderText                        = " + ";
        public const string OldLogFolder                            = "Old";
        public const string TemplateFolder                          = "Template";
        public const string TemplateFolderVersioni                  = "Versioni";
        public const string UpdateCommand                           = "RevisoftUpdate.bat";
        public const string urlCheckConnection                      = "http://www.revisoft.it/";
        public const string urlNoteRilascio                         = "http://www.revisoft.it/versioni.pdf";
        public const string UserFileFlussiFolder                    = "Flussi";
        public const string UserFileFolder                          = "UserDoc";
        public const string XBRL                                    = "xbrl";
        public const string XBRL2016                                = "xbrl_2016";
        public const string ZipFilePassword                         = "Datalabor.com";

        //--------------------------------------------------- costanti registro
        public const string Registry_AlertSuCompletato              = "AlertSuCompletato";
        public const string Registry_Benvenuto                      = "Benvenuto";
        public const string Registry_IstruzioniAutomatiche          = "IstruzioniAutomatiche";
        public const string Registry_PathArchivioRemoto             = "PathArchivioRemoto";
        public const string Registry_TipoGestioneArchivio           = "TipoGestioneArchivio";

        //----------------- 4.6 Versione x about e aggiornamento archivi remoto
        public const string AppVersioneAbout                = "4.12.7";
        public const string AppVersionePrecedente           = "4.12.6";

        //Variabili
        public static string MessaggioSolaScrittura = "Occorre selezionare Sblocca Stato per modificare il contenuto.";
        public static string MessaggioSolaScritturaStato = "Sessione in sola lettura, impossibile modificare lo stato.";
        public static ErrorTypes                            ErrorLevel;
        public static string                                CodiceMacchina;
        public static string                                CodiceMacchinaServer;
        public static bool Scaduta;

        //public static TipologieLicenze                      TipoLicenza;        //settata da classe cGestionelicenza
        //public static TipologieLicenze                      TipoLicenzaSigillo; //SIGILLO

        //Variabili funzionalità - vers. 4.5
        public static bool Multilicenza;
        public static bool Sigillo;
        public static bool Prova;
        public static bool Guest;
        public static bool Server;
        public static bool Cloud;
        public static bool RemoteDesktop;
        public static bool Client;
        public static int NumeroanAgrafiche;
        public static int NumeroLicenze;

        //Variabili di configurazione
        public static bool AppSetupBenvenuto;
        public static bool AppSetupIstruzioniAutomatiche;
        public static bool AppSetupAlertSuCompletato;
        public static TipoGestioneArchivio AppSetupTipoGestioneArchivio;
		public static string AppPathArchivioRemoto;
        public static DateTime AppInizioSessione;
        public static string AppNome;
        public static string AppVersione;
        public static bool AppSetupNuovaVersione;
        public static bool AppSetupScaricaNuovaVersione;
        public static int AppIstanzeAttive;
        public static bool AppSetupBackupPersonalizzato;
        public static bool AppSetupAddioBackupRevisoft;
        public static bool AppSetupAddioBackupUtente;


        //Variabili attivazioni funzioanlità/versioni licenza ***** ricordarsi di settarle in GestioneLicenza.Configuralicenza() ********
        public static bool AppConsentiImportaEsporta;
        public static bool AppConsentiCreazioneAnagrafica;
        public static bool AppConsentiImportazioneEsportazioneLan;
        public static bool AppConsentiAccessoArchivioLocale;
        public static bool AppConsentiAccessoArchivioRemoto;
        public static bool AppConsentiAccessoArchivioCloud;
        public static bool AppConsentiGestioneArchivioRemoto;
        public static bool AppConsentiGestioneBackUp;
        public static bool AppConsentiBackUp;
        public static bool AppConsentiMultiLicenza;
        public static bool AppConsentiSigillo;


        //Path di sistema
        public static string AppProgramFolder;      //Program file \ revisoft \ revisoft
        public static string AppLicenseFolder;      //Program file \ revisoft \ revisoft \ Licenze
        public static string AppDataFolder;         //User \ AppData \ Roaming \ revisoft \ revisoft
        public static string AppDataDataFolder;     //User \ AppData \ Roaming \ revisoft \ revisoft \ RDF
        public static string AppTemplateFolder;     //User \ AppData \ Roaming \ revisoft \ revisoft \ template
        public static string AppBackupFolder;       //User \ AppData \ Roaming \ revisoft \ revisoft \ Backup
        public static string AppFormularioFolder;   //User \ AppData \ Roaming \ revisoft \ revisoft \ Formulario
        public static string AppModelliFolder;      //User \ AppData \ Roaming \ revisoft \ revisoft \ Modelli
        public static string AppDocumentiFolder;    //User \ AppData \ Roaming \ revisoft \ revisoft \ UserDoc
        public static string AppDocumentiFlussiFolder;    //User \ AppData \ Roaming \ revisoft \ revisoft \ UserDoc \ Flussi
        public static string AppLocalDataFolder;    //User \ AppData \ Roaming \ revisoft \ revisoft
        public static string AppLogFolder;          //User \ AppData \ Roaming \ revisoft \ revisoft \ Log
        public static string AppOldLogFolder;       //User \ AppData \ Roaming \ revisoft \ revisoft \ Log \ Old
        public static string AppTempFolder;         //cartella temp di sistema
        public static string AppUserBackupFolder;   //User \ AppData \ Roaming \ revisoft \ revisoft \ Backup
        //4.6 percorsi backup personalizzato
        public static string AppBackupFolderUser;
        public static string AppBackUpDataFileUser;


        //Autoexec - per le attività che richiedono interfaccia
        public static bool AppAutoExec;
        public static TipoFile AppAutoExecTipoFile;
        public static string AppAutoExecFileName;
        public static TipoFunzioniAutoexec AppAutoExecFunzione;
        public static bool AppForzaAttivazioneLicenza;
        public static bool AppTestDownload;

        //File di sistema
        public static string AppInfoFile;
        public static string AppInfoFile_OLD;        
        public static string AppLicenseFile;
        public static string AppLicenseFile_OLD;        
        public static string AppLicenseSigilloFile;
        public static string AppLicenseSigilloFile_OLD;
        public static string AppHelpFile;
        public static string AppMessageFile;

        //File dati di sistema
        public static string AppFormularioFile;
        public static string AppFormularioFileDati;
        public static string AppModelliFile;
        public static string AppLogDataFile;

        //File template - utilizzati quanto creo nuovo doc
        public static string AppTemplateTreeIncarico;
        public static string AppTemplateTreeISQC;
        public static string AppTemplateTreeVerifica;
        public static string AppTemplateTreePianificazioniVerifica;
        public static string AppTemplateTreeRevisione;
		public static string AppTemplateTreeBilancio;
        public static string AppTemplateTreeConclusione;
        public static string AppTemplateDataIncarico;
        public static string AppTemplateDataISQC;
        public static string AppTemplateDataVerifica;
        public static string AppTemplateDataPianificazioniVerifica;
        public static string AppTemplateDataRevisione;
		public static string AppTemplateDataBilancio;
        public static string AppTemplateDataConclusione;
        public static string AppTemplateTreeVigilanza;
        public static string AppTemplateDataVigilanza;
        public static string AppTemplateTreePianificazioniVigilanza;
        public static string AppTemplateDataPianificazioniVigilanza;

        public static string AppTemplateTreeRelazioneB;
        public static string AppTemplateDataRelazioneB;
        public static string AppTemplateTreeRelazioneV;
        public static string AppTemplateDataRelazioneV;

        public static string AppTemplateTreeRelazioneBC;
        public static string AppTemplateDataRelazioneBC;
        public static string AppTemplateTreeRelazioneVC;
        public static string AppTemplateDataRelazioneVC;

        public static string AppTemplateTreeRelazioneBV;
        public static string AppTemplateDataRelazioneBV;

        public static string AppTemplateDataFlussi;
        		
        //File template - Stampa
        public static string AppTemplateStampa;
        public static string AppTemplateStampaNoLogo;
        public static string AppTemplateStampaBilancio;


		//File template - Bilanci
		public static string AppTemplateBilancio_Attivo;			
		public static string AppTemplateBilancio_ContoEconomico;	
		public static string AppTemplateBilancio_Passivo;
		public static string AppTemplateBilancio_Riclassificato;

		public static string AppTemplateBilancioAbbreviato_Attivo;		
		public static string AppTemplateBilancioAbbreviato_ContoEconomico;
		public static string AppTemplateBilancioAbbreviato_Passivo;
		public static string AppTemplateBilancioAbbreviato_Riclassificato;

        public static string AppTemplateBilancio_Attivo2016;
        public static string AppTemplateBilancio_ContoEconomico2016;
        public static string AppTemplateBilancio_Passivo2016;
        public static string AppTemplateBilancio_Riclassificato2016;

        public static string AppTemplateBilancio_Attivo2016_Consolidato;
        public static string AppTemplateBilancio_ContoEconomico2016_Consolidato;
        public static string AppTemplateBilancio_Passivo2016_Consolidato;
        public static string AppTemplateBilancio_Riclassificato2016_Consolidato;

        public static string AppTemplateBilancioAbbreviato_Attivo2016;
        public static string AppTemplateBilancioAbbreviato_ContoEconomico2016;
        public static string AppTemplateBilancioAbbreviato_Passivo2016;
        public static string AppTemplateBilancioAbbreviato_Riclassificato2016;

        public static string AppTemplateBilancioMicro_Attivo2016;
        public static string AppTemplateBilancioMicro_ContoEconomico2016;
        public static string AppTemplateBilancioMicro_Passivo2016;
        public static string AppTemplateBilancioMicro_Riclassificato2016;

        public static string AppXBRL;
		public static string AppLEAD;

        public static string AppXBRL2016;
        public static string AppLEAD2016;

        //File dati utente
        public static string AppMasterDataFile;
		public static string AppBackUpDataFile;
        public static string AppDocumentiDataFile;


        //Enum
        public enum TipologieLicenze            { Ignota = 0, Prova = 1, Server = 2, DeskTop = 3, EntryLevel = 4, ClientLan = 5, Viewer = 6, Guest = 7, ClientLanMulti = 8, Sigillo = 9, Scaduta = 99 };
        public enum TipoGestioneArchivio        { Locale = 0, Remoto = 1, Cloud = 2, LocaleImportExport = 3 };
        public enum ErrorTypes                  { Nessuno = 0, Segnalazione = 1, Avviso = 2, Errore = 3, ErroreBloccante = 4 };
        public enum TipoFile                    { Licenza = 0, Revisione = 1, Verifica = 2, Incarico = 3, Bilancio = 4, Master = 5, Info = 6, Messagi = 7, ImportExport = 8, ImportTemplate = 9, BackUp = 10, Formulario = 11, ModellPredefiniti = 12, DocumentiAssociati = 13, ScambioDati = 14, DocTemplate = 15, RevisoftXML = 16, XBRL = 17, Vigilanza = 18, Conclusione = 19, Sigillo = 20, RelazioneB = 21, RelazioneV = 22, RelazioneBV = 23, Log = 24, IndiceTemplate = 25, PianificazioniVerifica = 26, PianificazioniVigilanza = 27, ISQC = 28, BilancioDiVerifica = 29, RelazioneBC = 31, RelazioneVC = 32, Flussi = 99 };
        public enum TipoAttivitaScheda          { New = 0, Edit = 1, View = 2, Delete = 3, Export = 4, Condividi = 5 }
        public enum TipoAnagraficaStato         { Sconosciuto = -1, Disponibile = 0, InUso= 1, Bloccato = 2, Esportato = 3 }
        public enum TipoAnagraficaEsercizio     { Sconosciuto = -1, AnnoSolare = 0, ACavallo = 1}
		public enum TipoIncaricoComposizione	{ Sconosciuto = -1, CollegioSindacale = 0, Revisore = 1, SindacoUnico = 2 }
		public enum TipoIncaricoAttivita		{ Sconosciuto = -1, Nomina = 0, Riesame = 1 }
        public enum TipoISQCComposizione        { Sconosciuto = -1, CollegioSindacale = 0, Revisore = 1, SindacoUnico = 2 }
        public enum TipoISQCAttivita            { Sconosciuto = -1, Nomina = 0, Riesame = 1 }
        public enum TipoSessioneStato			{ Sconosciuto = -1, Disponibile = 0, InUso = 1, Bloccato = 2, Esportato = 3 }
        public enum TipoTreeNodeStato           { SigilloRotto = -6, Sigillo = -5, SolaLettura = -4, VociCompilate = -3, NodoFazzoletto = -2, Sconosciuto = -1, NonApplicabile = 0, DaCompletare = 1, Completato = 2, CancellaDati = 3, Scrittura = 4, Report = 5, NonApplicabileBucoTemplate = 6 }
        public enum TipoAttivita                { Sconosciuto = -1, Incarico = 0, Revisione = 1, Bilancio = 2, Verifica = 3, Vigilanza = 18, Conclusione = 19, RelazioneB = 21, RelazioneV = 22, RelazioneBV = 23, PianificazioniVerifica = 26, PianificazioniVigilanza = 27, ISQC = 28, RelazioneBC = 31, RelazioneVC = 32, Flussi = 99 }
        public enum TipoScambioDati             { Sconosciuto = -1, Esporta = 0, Importa = 1 };
        public enum TipoFunzioniAutoexec        { Sconosciuto = -1, ScambioDati = 0, NuovoCliente = 1, SetupLan = 2, ImportExport = 3, ImportTemplate = 4, Restore = 5 };

#if (DBG_TEST)
    public enum TipiOggetto
    {
      BILANCIO = 1, CONCLUSIONE = 2, INCARICO = 3, ISQC = 4, PIANIFICAZIONIVERIFICA = 5,
      PIANIFICAZIONIVIGILANZA = 6, RELAZIONEB = 7, RELAZIONEBV = 8, RELAZIONEV = 9,
      REVISIONE = 10, VERIFICA = 11, VIGILANZA = 12
    }
        public static string connString = "Data Source=ENRICO-PC;Initial Catalog=Revisoft;Integrated Security=True";
        public static SqlConnection sqlConnection = new SqlConnection(connString);
        public static string[] clientiFields =
        {
            "ID","Stato","Note","EsercizioAl","EsercizioDal","Esercizio",
            "CodiceFiscale","PartitaIVA","RagioneSociale","Presidente",
            "MembroEffettivo","MembroEffettivo2","RevisoreAutonomo",
            "OrganoDiControllo","OrganoDiRevisione","SindacoSupplente",
            "SindacoSupplente2","DataModificaStato","UtenteModificaStato"
        };
#endif


        //costruttore
        public App()
        {
			RevisoftApplication.Utilities u = new Utilities();

			//Settaggi applicativi
            App.AppAutoExec = false;
            App.ErrorLevel = App.ErrorTypes.Nessuno;

            //andrea 2.9 modifica necessaria per XP 
            if (Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) == "")
            {
                //PATH Programma - Win XP
                App.AppProgramFolder = Environment.GetEnvironmentVariable("ProgramFiles") + "\\" + ApplicationFolder + "\\" + ApplicationFolder;
            }
            else
            {
                //PATH Programma - Win 7
                App.AppProgramFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\" + ApplicationFolder + "\\" + ApplicationFolder;
            }

            App.AppLocalDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + App.ApplicationFolder + "\\" + App.ApplicationFolder;

            //andrea 3.0 - multilicenza 
            App.AppLicenseFolder = App.AppLocalDataFolder + "\\" + App.LicenseFolder;

			//FILE INFO
			App.AppInfoFile = App.AppLocalDataFolder + "\\" + ApplicationFileName + u.EstensioneFile(TipoFile.Info);
            App.AppInfoFile_OLD = App.AppProgramFolder + "\\" + ApplicationFileName + u.EstensioneFile(TipoFile.Info);
            //FILE LICENZA
            App.AppLicenseFile = App.AppLocalDataFolder + "\\" + ApplicationFileName + u.EstensioneFile(TipoFile.Licenza);
            App.AppLicenseFile_OLD = App.AppProgramFolder + "\\" + ApplicationFileName + u.EstensioneFile(TipoFile.Licenza);
            //FILE LICENZA SIGILLO
            App.AppLicenseSigilloFile = App.AppLocalDataFolder + "\\" + ApplicationFileName + u.EstensioneFile(TipoFile.Sigillo);
            App.AppLicenseSigilloFile_OLD = App.AppProgramFolder + "\\" + ApplicationFileName + u.EstensioneFile(TipoFile.Sigillo);
            //FILE HELP
            App.AppHelpFile = App.AppProgramFolder + "\\" + RevisoftApplication.Properties.Settings.Default["RevisoftApplicationGuide"].ToString();
			//FILE MESSAGGI
			App.AppMessageFile = App.AppProgramFolder + "\\" + ApplicationFileName + u.EstensioneFile(TipoFile.Messagi);  
            //Cartella TEMP
            AppTempFolder = System.IO.Path.GetTempPath();
		}

        //Gestione parametri applicativi su riga di comando 
        protected override void OnStartup(StartupEventArgs e)
        {

            //Conto le istanze attive Revisoft
            Process thisProc = Process.GetCurrentProcess();
            Process[] prx = Process.GetProcessesByName(thisProc.ProcessName);
            foreach (Process item in prx)
            {
                if (thisProc.MachineName == item.MachineName)
                {
                    App.AppIstanzeAttive++;
                }
            }

            //// Verifico la presenza di una precedente istanza di Revisoft ed esco se presente
            //Process thisProc = Process.GetCurrentProcess();
            //if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
            //{
            //    MessageBox.Show("Revisoft è già in esecuzione.\nImpossibile eseguire il programma nuovamente.");
            //    Application.Current.Shutdown();
            //    return;
            //}


            RevisoftApplication.Utilities u = new Utilities();

            foreach (string arg in e.Args)
            {
                switch (arg)
                {
                    case "/setup":
                        u.ConfiguraRegistroApplicazione();
                        break;
                    case "/lan":
                        App.AppAutoExec = true;
                        App.AppAutoExecFunzione = TipoFunzioniAutoexec.SetupLan;
                        break;
                    case "/licenza":
                        App.AppForzaAttivazioneLicenza = true;
                        break;
                    case "/key":
                        App.AppAutoExec = true;
                        if (u.LeggiInfoMacchina())
                        {
                            //copio codice macchina in clipboard
                            Clipboard.Clear();
                            Clipboard.SetText(App.CodiceMacchina);
                            MessageBox.Show("Codice macchina:\t" + App.CodiceMacchina + "\n\nInformazione inserita in clipboard.");
                        }
                        u.ChiudiApplicazione();
                        break;
                    case "/test":
                        App.AppTestDownload = true;
                        break;
                    case "/help":
                        MessageBox.Show("Funzione da riga di comando:\n/setup\tConfigura registro\n/lan\tconfigura archivio di rete\n/licenza\tdisattiva controlli licenza\n/key\tVisualizza codice macchina");
                        break;
//                    case "/init":
//                        //resetto info licenza
//                        GestioneLicenza lreset = new GestioneLicenza();
//                        lreset.ResetInfoRevisoft();
//                        //resetto applicazione 
//                        u.ConfiguraApplicazione();
//	            					u.ConfiguraPercorsi();
//                        MasterFile m = MasterFile.Create();
//                        m.ResetMasterFile();
//                        break;
                    default:
                        string ext = arg.ToString().Substring(arg.ToString().LastIndexOf("."));

                        //gestione licenza - senza interfaccia
                        if (ext.ToLower() == u.EstensioneFile(TipoFile.Licenza))
                        {
                            App.AppAutoExec = false;
                            RevisoftApplication.GestioneLicenza l = new GestioneLicenza();
                            l.AttivaLicenzaDaFile(arg);
                        }

                        ////gestione licenza SIGILLO - senza interfaccia
                        //if (ext == u.EstensioneFile(TipoFile.Sigillo))
                        //{
                        //    App.AppAutoExec = false;
                        //    RevisoftApplication.GestioneLicenza l = new GestioneLicenza();
                        //    l.AttivaSigilloDaFile(arg);
                        //}



                        //gestione scambio dati - con interfaccia su OnContentRendered
                        if (ext.ToLower() == u.EstensioneFile(TipoFile.ScambioDati))
                        {
                            App.AppAutoExec = true;
                            App.AppAutoExecTipoFile = App.TipoFile.ScambioDati;
                            App.AppAutoExecFileName = arg.ToString();
                            App.AppAutoExecFunzione = TipoFunzioniAutoexec.ScambioDati;
                        }

                        //gestione import export
                        if (ext.ToLower() == u.EstensioneFile(TipoFile.ImportExport))
                        {
                            App.AppAutoExec = true;
                            App.AppAutoExecTipoFile = App.TipoFile.ImportExport;
                            App.AppAutoExecFileName = arg.ToString();
                            App.AppAutoExecFunzione = TipoFunzioniAutoexec.ImportExport;
                        }
					
                        //gestione import export template
                        if (ext.ToLower() == u.EstensioneFile(TipoFile.ImportTemplate))
                        {
                            App.AppAutoExec = true;
                            App.AppAutoExecTipoFile = App.TipoFile.ImportTemplate;
                            App.AppAutoExecFileName = arg.ToString();
                            App.AppAutoExecFunzione = TipoFunzioniAutoexec.ImportTemplate;
                        }

                        //gestione Backup/Restore
                        if (ext.ToLower() == u.EstensioneFile(TipoFile.BackUp))
                        {
                            App.AppAutoExec = true;
                            App.AppAutoExecTipoFile = App.TipoFile.BackUp;
                            App.AppAutoExecFileName = arg.ToString();
                            App.AppAutoExecFunzione = TipoFunzioniAutoexec.Restore;
                        }

                        break;
                }
            }
            base.OnStartup(e);
        }

        public static string NomeTipoTreeNodeStato(TipoTreeNodeStato stato)
        {
            string buff = "";

            switch (stato)
            {
                //NodoFazzoletto
                case App.TipoTreeNodeStato.NodoFazzoletto:
                    buff = "Promemoria";
                    break;
                //NonApplicabile
                case App.TipoTreeNodeStato.NonApplicabileBucoTemplate:
                case App.TipoTreeNodeStato.NonApplicabile:
                    buff = "Non Applicabile";
                    break;
                //DaCompletare
                case App.TipoTreeNodeStato.DaCompletare:
                    buff = "Da Completare";
                    break;
                //Completato
                case App.TipoTreeNodeStato.Completato:
                    buff = "Completato";
                    break;
                //CancellaDati
                case App.TipoTreeNodeStato.CancellaDati:
                    buff = "Resettato";
                    break;
                //Scrittura
                case App.TipoTreeNodeStato.Scrittura:
                    buff = "In Scrittura";
                    break;
                //Sconosciuto
                case App.TipoTreeNodeStato.Sconosciuto:
                default:
                    buff = "Nessuno stato assegnato";
                    break;
            }

            return buff;
        }


    }
}
*/
