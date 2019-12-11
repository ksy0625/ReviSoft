using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Windows;
using NLog;
using System.Collections;
using System.Xml.Linq;
using System.Xml;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Threading;

namespace RevisoftApplication
{

  public class Accettazionedelrischio_6_1
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string rischio { get; set; }
  }


  public class DichiarazioneRedditi_6_1_Rate
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string ID { get; set; }
    public string scadenze { get; set; }
    public string rata { get; set; }
    public string pagatoil { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }

  }

  public class DichiarazioneRedditi_6_1
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string periodo_imposta { get; set; }
    public string H1 { get; set; }
    public string H2 { get; set; }
    public string H3 { get; set; }
    public string H4 { get; set; }
    public string C1R1 { get; set; }
    public string C2R1 { get; set; }
    public string C3R1 { get; set; }
    public string C4R1 { get; set; }
    public string C1R2 { get; set; }
    public string C2R2 { get; set; }
    public string C3R2 { get; set; }
    public string C4R2 { get; set; }
    public string C1R3 { get; set; }
    public string C2R3 { get; set; }
    public string C3R3 { get; set; }
    public string C4R3 { get; set; }
    public string C1R4 { get; set; }
    public string C2R4 { get; set; }
    public string C3R4 { get; set; }
    public string C4R4 { get; set; }
    public double C1R5 { get; set; }
    public double C2R5 { get; set; }
    public double C3R5 { get; set; }
    public double C4R5 { get; set; }
    public double C1ACC1 { get; set; }
    public double C2ACC1 { get; set; }
    public double C3ACC1 { get; set; }
    public double C4ACC1 { get; set; }
    public double compensazione1 { get; set; }
    public double C1ACC2 { get; set; }
    public double C2ACC2 { get; set; }
    public double C3ACC2 { get; set; }
    public double C4ACC2 { get; set; }
    public double compensazione2 { get; set; }
    public string datapagamento { get; set; }

  }

  public class StatoNodi
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Stato { get; set; }
  }

  public class BilancioVerifica
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string ID { get; set; }
    public string esercizio { get; set; }
    public string codice { get; set; }
    public string titolo { get; set; }
    public string valore { get; set; }
    public string opened { get; set; }
  }


  public class PianificazioneNewSingolo
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int ID { get; set; }
    public string Voce { get; set; }
    public string Titolo { get; set; }
    public string Testo { get; set; }
    public string Esecutore { get; set; }
    public string Nota { get; set; }
    public string cmbRI { get; set; }
    public string EsameFisico { get; set; }
    public string Ispezione { get; set; }
    public string Indagine { get; set; }
    public string Osservazione { get; set; }
    public string Ricalcolo { get; set; }
    public string Riesecuzione { get; set; }
    public string Conferma { get; set; }
    public string PianificazioneNewSingle { get; set; }


  }

  public class Campionamento
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Finaldata { get; set; }
    public string Rawdata { get; set; }
    public string NomeFile { get; set; }
    
  }

   public class CampionamentoValori
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string NomeAttributo { get; set; }
    public string ValoreAttributo { get; set; }
    
    
  }

  public class ArchivioDocumenti
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int ID { get; set; }
    public string Tree { get; set; }
    public string Tipo { get; set; }
    public string Titolo { get; set; }
    public string Descrizione { get; set; }
    public string File { get; set; }
    public string Visualizza { get; set; }
    public string ClienteExtended { get; set; }
    public string TreeExtended { get; set; }
    public string SessioneExtended { get; set; }
    public string NodoExtended { get; set; }
    public string FileExtended { get; set; }
    public string TipoExtended { get; set; }

  }


  public class PianificazioneNew
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string ID { get; set; }
    public string Voce { get; set; }
    public string Titolo { get; set; }
    public string Testo { get; set; }
    public string EsameFisico { get; set; }
    public string Ispezione { get; set; }
    public string Indagine { get; set; }
    public string Osservazione { get; set; }
    public string Ricalcolo { get; set; }
    public string Riesecuzione { get; set; }
    public string Conferma { get; set; }
    public string Comparazioni { get; set; }
    public string Esecutore { get; set; }
    public string Nota { get; set; }
    public string cmbRI { get; set; }

  }


  public class PianificazioneNewWD_Node
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int ID { get; set; }
    public string Chiuso { get; set; }
    public string cmbRI { get; set; }
    public string cmbRI_Proposto { get; set; }
    public string Esecutore { get; set; }
    public string Nota { get; set; }
    public string Titolo { get; set; }
    public string Voce { get; set; }
    public string xaml { get; set; }


  }
  public class PianificazioneNewWD_Valore
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int ID { get; set; }
    public string Chiuso { get; set; }
    public string Codice { get; set; }
    public string somma { get; set; }
    public string CONTROLLO { get; set; }
    public string EA { get; set; }
    public string ET { get; set; }
    public string MO { get; set; }
    public string refEA { get; set; }
    public string Tipo { get; set; }
    public string Titolo { get; set; }
    public string EsameFisico { get; set; }
    public string Ispezione { get; set; }
    public string Indagine { get; set; }
    public string Osservazione { get; set; }
    public string Ricalcolo { get; set; }
    public string Riesecuzione { get; set; }
    public string Conferma { get; set; }
    public string NoteNumber { get; set; }
    public string NoteRealRow { get; set; }
    public string Note { get; set; }


  }
  public class PianificazioneNewWD_ValoreRighe
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int ID { get; set; }
    public int row { get; set; }
    public string EA { get; set; }
    public string Codice { get; set; }
    public string Titolo { get; set; }

  }

  public class Esecutore_Reviewer
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Esecutore { get; set; }
    public string Reviewer { get; set; }

  }

  public class Osservazioni
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string OsservazioniTxt { get; set; }
    public string OsservazioniOldTxt { get; set; }

  }
  public class TabellaSospesi
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string SospesiTxt { get; set; }
    public string SospesiOldTxt { get; set; }
    public string TitoloAttivita { get; set; }


  }

  public class Excel_BilancioRiclassificato
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Titolo { get; set; }
    public int row { get; set; }
    public string tipo { get; set; }
    public string name { get; set; }
    public string EA { get; set; }
    public string EP { get; set; }
    public string DIFF { get; set; }
    public string PERCENT_EA { get; set; }
    public string PERCENT_EP { get; set; }

  }

  public class DestinatariEBilancio
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string RagioneSociale { get; set; }
    public string Indirizzo { get; set; }
    public string REA { get; set; }
    public string CapitaleSociale { get; set; }
    public string txtValoreProduzione { get; set; }
    public string txtCostiProduzione { get; set; }
    public string txtRisultatoGestione { get; set; }
    public string txtRettifiche { get; set; }
    public string txtRisultatoExtragestione { get; set; }
    public string txtImposte { get; set; }
    public string txtUtilePerditaEconomico { get; set; }
    public string txtAttivita { get; set; }
    public string txtPassivita { get; set; }
    public string txtPatrimonioNetto { get; set; }
    public string txtUtilePerditaPatrimoniale { get; set; }
    public string cmbBilancio { get; set; }
    public string cmbDestinatari { get; set; }

  }



  public class Excel_ErroriRilevati_riepilogo
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string row { get; set; }
    public string txtCodice { get; set; }
    public string txtName { get; set; }
    public string txtIMPOSTE { get; set; }
    public string txtIMPOSTEPN { get; set; }
    public string txtAP { get; set; }
    public string txtEA { get; set; }
    public string chkIrrilevante { get; set; }
    public string txtPN { get; set; }
    public string txtDIFF { get; set; }
    public string txtNETTOPN { get; set; }
    public string txtSP { get; set; }
    public string txtNETTOCE { get; set; }
    public string txtCE { get; set; }
    public string rowTOT { get; set; }
    public string txtTotEA { get; set; }
    public string txtTotAP { get; set; }
    public string txtTotPN { get; set; }
    public string txtTotTotPN { get; set; }
    public string txtTotTotCE { get; set; }
    public string txtTotDIFF { get; set; }
    public string txtTotIMPOSTE { get; set; }
    public string txtTotIMPOSTEPN { get; set; }
    public string txtTotMaterialitaSP { get; set; }
    public string txtTotMaterialitaCE { get; set; }
    public string txtTotEccedenzaSP { get; set; }
    public string txtTotEccedenzaCE { get; set; }
    public string txtErroreTollerabileSP { get; set; }
    public string txtErroreTollerabileCE { get; set; }


  }


  public class Excel_LimiteMaterialitaSPCE
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string ID { get; set; }
    public string name { get; set; }
    public string ma { get; set; }
    public string et { get; set; }
    public string value { get; set; }
  }

  public class ValutazioneAmbiente
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string ID { get; set; }
    public string name { get; set; }
    public string Alto { get; set; }
    public string Medio { get; set; }
    public string Basso { get; set; }


  }

  public class Excel_Rifiuti
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }
    public string rif { get; set; }
    public string caricoscarico { get; set; }
    public string data { get; set; }
    public string pagina { get; set; }
    public string protocollo { get; set; }
    public string formulario { get; set; }

  }
  public class Excel_Rifiuti_Note
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }
    public string rif { get; set; }
    public string note { get; set; }

  }


  public class Excel_BilancioIndici
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string txtEA_1 { get; set; }
    public string txtEP_1 { get; set; }
    public string txtEA_2 { get; set; }
    public string txtEP_2 { get; set; }
    public string txtEA_3 { get; set; }
    public string txtEP_3 { get; set; }
    public string txtEA_4 { get; set; }
    public string txtEP_4 { get; set; }
    public string txtEA_5 { get; set; }
    public string txtEP_5 { get; set; }
    public string txtEA_6 { get; set; }
    public string txtEP_6 { get; set; }
    public string txtEA_7 { get; set; }
    public string txtEP_7 { get; set; }
    public string txtEA_8 { get; set; }
    public string txtEP_8 { get; set; }
    public string txtEA_9 { get; set; }
    public string txtEP_9 { get; set; }
    public string txtEA_10 { get; set; }
    public string txtEP_10 { get; set; }
    public string txtEA_11 { get; set; }
    public string txtEP_11 { get; set; }
    public string txtEA_12 { get; set; }
    public string txtEP_12 { get; set; }
    public string txtEA_13 { get; set; }
    public string txtEP_13 { get; set; }
  }


  public class ConfrontoMaterialita
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int txtErroreTollerabileSP { get; set; }
    public string txtErroreTollerabileCE { get; set; }


  }


  public class Leads
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int ID { get; set; }
    public string Tipo { get; set; }
    public string Titolo { get; set; }
    public string refEA { get; set; }
    public string refEP { get; set; }
    public string EA { get; set; }
    public string EP { get; set; }
    public string incdec { get; set; }
    public string somma { get; set; }

  }

  public class Excel_Bilancio_Testata
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Esecutore { get; set; }
    public string Osservazioni { get; set; }
    public string OsservazioniOLD { get; set; }
    public string Reviewer { get; set; }
    public string Sospesi { get; set; }
    public string TestoDaStampare { get; set; }
    public string tipoBilancio { get; set; }
    public string nodeModified { get; set; }
    public string TitoloEA { get; set; }
    public string TitoloEP { get; set; }
    public string opened { get; set; }
    public string stato { get; set; }

  }

  public class Excel_Bilancio
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string template { get; set; }
    public string ID { get; set; }
    public string Codice { get; set; }
    public string DIFF { get; set; }
    public double EA { get; set; }
    public double EP { get; set; }
    public string name { get; set; }
    public int paddingCodice { get; set; }
    public string Titolo { get; set; }
    public string rigaVuota { get; set; }
    public string bg { get; set; }
    public string noData { get; set; }
    public string opened { get; set; }
  }

  public class Excel_VersamentoImposteContributi
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string periodo { get; set; }
    public string name { get; set; }
    public string codice { get; set; }
    public double importoPagato { get; set; }
    public double importoCompensato { get; set; }
    public string PeriodoDiRiferimento { get; set; }
    public string DataDiPagamento { get; set; }
    public string AMezzo { get; set; }
    public string ProtocolloTelematico { get; set; }

  }

  public class Excel_Compensazioni
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string periodo { get; set; }
    public string name { get; set; }
    public string codice { get; set; }
    public double importoPagato { get; set; }
    public string txtfinder { get; set; }
    public string txtCreditoEsistente { get; set; }
    public string isnew { get; set; }

  }

  public class IndipendenzaFinanziaria
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string txtCT { get; set; }
    public string txtC { get; set; }
    public string txtS { get; set; }
    public string txtSCCT { get; set; }
    public string txtSSC { get; set; }
    public string txtFascia { get; set; }
    public string txtValutazione { get; set; }
  }


  public class TempiCorrispettiviVigilanza
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string professionista { get; set; }
    public string qualifica { get; set; }
    public double onorario { get; set; }

  }

  public class TempiCorrispettivi
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string codice { get; set; }
    public string name { get; set; }
    public int ore { get; set; }
    public double tariffaoraria { get; set; }
    public double onorario { get; set; }
    public string Incipit { get; set; }

  }


  public class NodoMultiplo
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string ID { get; set; }
    public string Tab { get; set; }
    public string Tipologia { get; set; }
  }


  public class Excel_F24
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }

    public string rif { get; set; }
    public string codicetributo { get; set; }
    public string competenza { get; set; }
    public double importopagato { get; set; }
    public double importocompensato { get; set; }
    public string datapagamento { get; set; }
    public string datascadenza { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }
  }


  public class Excel_F24Note
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }
    public int rif { get; set; }
    public string note { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }
  }



  public class Excel_ErroriRilevatiNN
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string numero { get; set; }
    public string name { get; set; }
    public string corretto { get; set; }
  }
  public class Excel_ErroriRilevatiMR
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string rif { get; set; }
    public string name { get; set; }
    public string contoimputato { get; set; }
    public string contoproposto { get; set; }
    public double importo { get; set; }
    public string corretto { get; set; }
  }


  public class Excel_ErroriRilevati_Note
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string rif { get; set; }
    public string name { get; set; }
  }

  public class RelazioneErroriRilevati
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int ID { get; set; }
    public string titolo { get; set; }
    public string testo { get; set; }
    public string chkInserireRelazione { get; set; }
    public string chk1 { get; set; }
    public string chk2 { get; set; }
    public string chk3 { get; set; }

  }


  public class Excel_ErroriRilevati
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string name { get; set; }
    public double importo { get; set; }
    public double importoAP { get; set; }
    public double impattofiscale { get; set; }
    public double impattofiscalePN { get; set; }
    public double suutileattuale { get; set; }
    public double suPNattuale { get; set; }
    public string rif { get; set; }
    public string corretto { get; set; }
    public string txtV_IRES { get; set; }
    public string txtP_IRES { get; set; }
    public string txtV_IRAP { get; set; }
    public string txtP_IRAP { get; set; }
    public string txt1 { get; set; }
    public string txtV_1 { get; set; }
    public string txtP_1 { get; set; }
    public string txt2 { get; set; }
    public string txtV_2 { get; set; }
    public string txtP_2 { get; set; }
    public string txt3 { get; set; }
    public string txtV_3 { get; set; }
    public string txtP_3 { get; set; }
    public string txt4 { get; set; }
    public string txtV_4 { get; set; }
    public string txtP_4 { get; set; }
    public string txt5 { get; set; }
    public string txtV_5 { get; set; }
    public string txtP_5 { get; set; }

    public string txtV_IRESPN { get; set; }
    public string txtP_IRESPN { get; set; }
    public string txtV_IRAPPN { get; set; }
    public string txtP_IRAPPN { get; set; }
    public string txt1PN { get; set; }
    public string txtV_1PN { get; set; }
    public string txtP_1PN { get; set; }
    public string txt2PN { get; set; }
    public string txtV_2PN { get; set; }
    public string txtP_2PN { get; set; }
    public string txt3PN { get; set; }
    public string txtV_3PN { get; set; }
    public string txtP_3PN { get; set; }
    public string txt4PN { get; set; }
    public string txtV_4PN { get; set; }
    public string txtP_4PN { get; set; }
    public string txt5PN { get; set; }
    public string txtV_5PN { get; set; }
    public string txtP_5PN { get; set; }

  }


  public class Excel_CapitaleSociale_CapitaleSociale
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string name { get; set; }
    public double deliberato { get; set; }
    public double sottoscritto { get; set; }
    public double versato { get; set; }
  }
  public class Excel_CapitaleSociale_TipiAzioni
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string name { get; set; }
    public double valnom { get; set; }
    public double numero { get; set; }
    public double totale { get; set; }
  }
  public class Excel_CapitaleSociale_Ripartizione
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string name { get; set; }
    public double valnom { get; set; }
    public double numero { get; set; }
    public double totale { get; set; }
    public double percentuale { get; set; }
    public string tiporipartizione { get; set; }
  }

  public class Excel_CapitaleSociale_RipartizioneAN
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string name { get; set; }
    public double valnom { get; set; }
    public double numero { get; set; }
    public double totale { get; set; }
    public double percentuale { get; set; }
    public string tiporipartizione { get; set; }
  }






  public class Excel_NumeriCasuali
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int txt1 { get; set; }
    public int txt2 { get; set; }
    public int txt3 { get; set; }
    public int txt4 { get; set; }
    public int txt5 { get; set; }
    public int txt6 { get; set; }
    public int txt7 { get; set; }
    public int txt8 { get; set; }
    public int txt9 { get; set; }
    public int txt10 { get; set; }
    public int txt11 { get; set; }
    public int txt12 { get; set; }
    public int txt13 { get; set; }
    public int txt14 { get; set; }
  }

  public class Excel_COGENote
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }
    public string rif { get; set; }
    public string note { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }
  }

  public class Excel_COGE
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }
    public string rif { get; set; }
    public string contocoge { get; set; }
    public string descrizionecoge { get; set; }
    public double importocoge { get; set; }
    public double importof24 { get; set; }
    public double delta { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }
  }

  public class clsAltoMedioBasso
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int value { get; set; }
  }

  public class TestoPropostoMultiplo
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int ID { get; set; }
    public string value { get; set; }
    public string strchecked { get; set; }
    public string name { get; set; }
  }

  public class DiscussioniTeam
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string data { get; set; }
    public string Chiuso { get; set; }
    public int ID { get; set; }
    public string name { get; set; }
  }


  public class Excel_SospesiDiCassa
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string PeriodoDiRiferimento { get; set; }
    public string name { get; set; }
    public string codice { get; set; }
    public string data_prelievo { get; set; }
    public double importoCompensato { get; set; }
  }

  public class TempiRevisione
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string fase { get; set; }
    public string attivita { get; set; }
    public string esecutore { get; set; }
    public double ore { get; set; }
  }

  public class Excel_ISQC_TempiLavoro_Riepilogo
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string titolo { get; set; }
    public double previste { get; set; }
    public double effettive { get; set; }
    public double scostamento { get; set; }
    public string percentuale { get; set; }

  }

  public class Excel_ScrittureMagazzino
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }
    public string rif { get; set; }
    public string descrizione { get; set; }
    public string data { get; set; }
    public string pagina { get; set; }
    public string protocollo { get; set; }

  }


  public class Excel_ScrittureMagazzino_note
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }
    public string rif { get; set; }
    public string note { get; set; }
  }


  public class Excel_Riconciliazioni
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string CreditoEsistente { get; set; }
    public string banca { get; set; }
    public string ccn { get; set; }
    public double saldocontabile { get; set; }
    public double saldoec { get; set; }
    public double differenza { get; set; }
    public double riconciliato { get; set; }
    public double importoconriconciliato { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }

  }

  public class Excel_ISQC_TempiLavoro
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }
    public string data { get; set; }
    public string esecutore { get; set; }
    public int previste { get; set; }
    public int effettive { get; set; }
    public int scostamento { get; set; }
    public double percentuale { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }

  }

  public class Excel_CassaContanteNew
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string CreditoEsistente { get; set; }
    public string unitario { get; set; }
    public string euro { get; set; }
    public int numeropezzi { get; set; }
    public double txtTotaleComplessivo { get; set; }
    public double txtDifferenza { get; set; }
    public double txtSaldoSchedaContabile { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }

  }



  public class Excel_CassaContanteAltreValute
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string CreditoEsistente { get; set; }
    public string unitario { get; set; }
    public string valuta { get; set; }
    public int numeropezzi { get; set; }

    public double txtTotaleComplessivo { get; set; }
    public double txtDifferenza { get; set; }
    public double txtSaldoSchedaContabile { get; set; }
    public double txtTassoDiCambio { get; set; }
    public double txtControvaloreInEuro { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }

  }


  public class Excel_ISQC_Incaricati
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }
    public string incaricato { get; set; }
    public string incarico { get; set; }
    public string previste { get; set; }
    public string effettive { get; set; }
    public string scostamento { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }


  }

  public class Excel_RitenuteLavoratoriAutonomi
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }
    public int rif { get; set; }
    public string fornitore { get; set; }
    public double importo { get; set; }
    public string datadocumento { get; set; }
    public string numerodocumento { get; set; }
    public string datapagamento { get; set; }
    public string codicetributo { get; set; }
  }

  public class Excel_RitenuteLavoratoriAutonomiNote
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }
    public int rif { get; set; }
    public string note { get; set; }
  }


  public class Excel_Affidamenti
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string banca { get; set; }
    public string tipoaffidamento { get; set; }
    public string inizio { get; set; }
    public string dataverifica { get; set; }
    public string utilizzo { get; set; }
    public string scadenza { get; set; }

  }


  public class Excel_CUD
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }
    public int rif { get; set; }
    public string periodo { get; set; }
    public string scadenza { get; set; }
    public string datapresentaz { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }


  }

  public class Excel_CUD_Note
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }
    public int rif { get; set; }
    public string note { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }

  }





  public class LuogoDataFirma
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string cmbFirma { get; set; }
    public string txtData { get; set; }
    public string txtLuogo { get; set; }
  }

  public class ConsolidatoReportistica
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int riga { get; set; }
    public string titolo { get; set; }
    public string testo { get; set; }
    public string name { get; set; }
    public string chk1 { get; set; }
    public string chk2 { get; set; }
    public string chk3 { get; set; }

  }

  public class ConsolidatoIstruzioni
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int riga { get; set; }
    public string titolo { get; set; }
    public string testo { get; set; }
  }



  public class Incarico_Pagamenti
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Incipit { get; set; }
    public string name { get; set; }
    public string codice { get; set; }

  }
  public class Incarico_Personale
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Incipit { get; set; }
    public string name { get; set; }
    public string codice { get; set; }
    public string note { get; set; }
  }

  public class Pianificazione
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string field_80 { get; set; }
    public string field_81 { get; set; }
    public string field_82 { get; set; }
    public string field_83 { get; set; }
    public string field_85 { get; set; }
    public string field_86 { get; set; }
    public string field_87 { get; set; }
    public string field_88 { get; set; }
    public string field_89 { get; set; }
    public string field_90 { get; set; }
    public string field_91 { get; set; }
    public string field_93 { get; set; }
    public string field_94 { get; set; }
    public string field_95 { get; set; }
    public string field_96 { get; set; }
    public string field_97 { get; set; }
    public string field_98 { get; set; }
    public string field_99 { get; set; }
    public string field_100 { get; set; }
    public string field_101 { get; set; }
    public string field_102 { get; set; }

  }

  public class RischioGlobale
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string txt1 { get; set; }
    public string txt2 { get; set; }
    public string txt3 { get; set; }
    public string txt4 { get; set; }
    public string txt5 { get; set; }
    public string txt6 { get; set; }
    public string txt1c { get; set; }
    public string txt2c { get; set; }
    public string txt3c { get; set; }
    public string txt4c { get; set; }
    public string txt5c { get; set; }
    public string txt6c { get; set; }

  }

  public class Excel_Uniemens
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }
    public string rif { get; set; }
    public string periodo { get; set; }
    public double importo { get; set; }
    public string scadenza { get; set; }
    public string datapresentaz { get; set; }
    public string numeroprotocollo { get; set; }
    public string cfintermediario { get; set; }
    public string datapag { get; set; }
  }

  public class Excel_Uniemens_Note
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }
    public string rif { get; set; }
    public string note { get; set; }

  }


  public class CassaContante
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public double unitario { get; set; }
    public int numeropezzi { get; set; }
    public double euro { get; set; }
    public double txtSaldoSchedaContabile { get; set; }
    public double CreditoEsistente { get; set; }
    public double txtTotaleComplessivo { get; set; }
    public double txtDifferenza { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }


  }


  public class ContributiAgenti
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }
    public string rif { get; set; }
    public string periodo { get; set; }
    public string scadenza { get; set; }
    public string datapresentaz { get; set; }
    public double importo { get; set; }
    public string datapag { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }


  }


  public class ContributiAgenti_Note
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string Header { get; set; }
    public string rif { get; set; }
    public string note { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }
  }


  public class Excel_Consolidato
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string name { get; set; }
    public double risultatonetto { get; set; }
    public double produzionesenzacosti { get; set; }
    public double valoreproduzione { get; set; }
    public double patrimonionetto { get; set; }
    public double passivo { get; set; }
    public double attivo { get; set; }
    public double risultatonettoCHECK { get; set; }
    public double risultatoanteimposteCHECK { get; set; }
    public double produzionesenzacostiCHECK { get; set; }
    public double valoreproduzioneCHECK { get; set; }
    public double patrimonionettoCHECK { get; set; }
    public double passivoCHECK { get; set; }
    public double attivoCHECK { get; set; }

    public double risultatonetto2 { get; set; }
    public double risultatoanteimposte2 { get; set; }
    public double produzionesenzacosti2 { get; set; }
    public double valoreproduzione2 { get; set; }
    public double patrimonionetto2 { get; set; }
    public double passivo2 { get; set; }
    public double attivo2 { get; set; }

    public double risultatonettoCHECK2 { get; set; }
    public double risultatoanteimposteCHECK2 { get; set; }
    public double produzionesenzacostiCHECK2 { get; set; }
    public double valoreproduzioneCHECK2 { get; set; }
    public double patrimonionettoCHECK2 { get; set; }
    public double passivoCHECK2 { get; set; }
    public double attivoCHECK2 { get; set; }

    public double risultatonettoTOT { get; set; }
    public double produzionesenzacostiTOT { get; set; }
    public double risultatoanteimposteTOT { get; set; }
    public double valoreproduzioneTOT { get; set; }
    public double patrimonionettoTOT { get; set; }
    public double passivoTOT { get; set; }
    public double attivoTOT { get; set; }
    public double risultatoanteimposte { get; set; }
    public string scope { get; set; }
    public string metodoconsolidamento { get; set; }
    public string revisore { get; set; }
    public string sede { get; set; }
    public string CF { get; set; }
    public string denominazione { get; set; }

  }


  public class CompensiERisorse_EsecutoriRevisione
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string nome { get; set; }
    public string qualifica { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }
  }


  public class CompensiERisorse_CompensoRevisione_Stimaore_6_1
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public double Totale_Attivo { get; set; }
    public double Totale_Ricavi { get; set; }
    public double txtMedia { get; set; }
    public string txtNumeroOre { get; set; }
    public string txtSettore { get; set; }
    public string txtPercMaggRid { get; set; }
    public string txtRischio { get; set; }
    public string txtPercRischioMaggRid { get; set; }
    public string txtTotaleOre { get; set; }

  }

  public class CompensiERisorse_CompensoRevisione_6_1
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string fase { get; set; }
    public string attivita { get; set; }
    public string esecutore { get; set; }
    public string qualifica { get; set; }
    public string data_termine { get; set; }
    public double ore { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }
  }

  public class CompensiERisorse_Totali_6_1
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string qualifica { get; set; }
    public double ore { get; set; }
    public double tariffa { get; set; }
    public double compenso { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }
  }



  public class CompensiERisorse_CompensoRevisione
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string fase { get; set; }
    public string attivita { get; set; }
    public string esecutore { get; set; }
    public double ore { get; set; }
    public string termini { get; set; }
    public double txtTotale { get; set; }
    public double txtTariffaOraria { get; set; }
    public double txtCompenso { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }
  }

  public class CompensiERisorse_TerminiEsecuzione
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string fase { get; set; }
    public string attivita { get; set; }
    public string termini { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }
  }


  public class CassaValoriBollati_Francobolli
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int numeropezzi { get; set; }
    public double unitario { get; set; }
    public double euro { get; set; }
    public string txtfinder { get; set; }
    public string CreditoEsistente { get; set; }
    public double txtSaldoSchedaContabile { get; set; }
    public double txtTotaleComplessivo { get; set; }
    public double txtDifferenza { get; set; }
    public string isnew { get; set; }
  }
  public class CassaValoriBollati_Marche
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int numeropezzi { get; set; }
    public double unitario { get; set; }
    public double euro { get; set; }
    public string txtfinder { get; set; }
    public string isnew { get; set; }
  }

  /*
    public class CassaContanteAltreValute
   {
          public int ID_SCHEDA { get; set; }
          public int ID_CLIENTE { get; set; }
          public int ID_SESSIONE { get; set; }
          public double CreditoEsistente { get; set; }
      public int numeropezzi { get; set; }
      public int unitario { get; set; }
      public int valuta { get; set; }
    }
    */

  // codice ok manca store e udtt non testato
  public class ProspettoIVA
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string nomecampo { get; set; }
    public string valore { get; set; }

  }

  public class Testi
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int ID { get; set; }
    public string name { get; set; }
    public string value { get; set; }
  }

  public class Tabella
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int ID { get; set; }
    public string name { get; set; }
    public string value { get; set; }
    public string tab { get; set; }
  }


  public class CheckListPlus
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int ID { get; set; }
    public string Codice { get; set; }
    public string name { get; set; }
    public string value { get; set; }
    public string Nota { get; set; }
    public string opzione1 { get; set; }
    public string opzione2 { get; set; }
    public string opzione3 { get; set; }

  }


  public class CheckList
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int ID { get; set; }
    public string Codice { get; set; }
    public string name { get; set; }
    public string value { get; set; }
    public string Nota { get; set; }
    public string risultato { get; set; }
  }



  public class CassaTitoli
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string name { get; set; }
    public string codice { get; set; }
    public double importoPagato { get; set; }
    public double importoCompensato { get; set; }
    public string txtfinder { get; set; }
    public string CreditoEsistente { get; set; }
    public string isnew { get; set; }

  }

  // ok udtt  e store testato
  public class CassaAssegni
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string name { get; set; }
    public string codice { get; set; }
    public double importoPagato { get; set; }
    public double importoCompensato { get; set; }
    public string txtfinder { get; set; }
    public string PeriodoDiRiferimento { get; set; }
    public string isnew { get; set; }

  }
  public class PianificazioneVerifiche
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public int ID { get; set; }
    public int NODE_ID { get; set; }
    public string PianificazioneID { get; set; }
    public string Codice { get; set; }
    public string Titolo { get; set; }
    public string Father { get; set; }
    public string Checked { get; set; }

  }

  public class PianificazioneVerificheTestata
  {
    public int ID_SCHEDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public int ID_SESSIONE { get; set; }
    public string ID { get; set; }
    public string Data { get; set; }
    public string PianificazioneChecked { get; set; }
  }


  public static class cBusinessObjects
  {
   public static  bool SessioneIsNew=false;
    public static Dictionary<string, FrameworkElement> uc_controls = new Dictionary<string, FrameworkElement>();
    public static Logger logger = LogManager.GetCurrentClassLogger();
    public static int idcliente;
    public static int idsessione;
    public static Hashtable ht_diff_tra_uc_bilancio = new Hashtable();
    

    public const string empty_rtf = @"\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 MS Sans Serif;}";
    public static string connection_db;
    private static wLavoriInCorso pd = null;
    public static string TitoloAttivita; // viene impostato in wWorkAreaTree per essere inserito nei sospesi della scheda come titolo sezione 


    public const string url_ticket = "";
    public const string url_script_db = "";

    public static XmlDocument xmldocument = new XmlDocument(); // usato in condividi dati

    public static bool ReadOnlyControls = false;



    public static string GetCapitaleSociale()
    {

       
      string CapitaleSociale = "";
      DataTable dati = GetData(39, typeof(Excel_CapitaleSociale_CapitaleSociale), -1, -1, 1);
      foreach (DataRow item in dati.Rows)
      {

        if (item["name"].ToString() == "Totale")
        {
          if (ConvertInteger(item["sottoscritto"].ToString()) == ConvertInteger(item["versato"].ToString()))
          {
            return "Capitale Sociale euro " + ConvertInteger(item["versato"].ToString()) + " I.V.";
          }
          else
          {
            if (item["versato"].ToString() != "")
            {
              if (item["versato"].ToString() != "0")
                return "Capitale Sociale sottoscritto euro " + ConvertInteger(item["sottoscritto"].ToString()) + " versato " + ConvertInteger(item["versato"].ToString());
              else
                return "Capitale Sociale sottoscritto euro " + ConvertInteger(item["sottoscritto"].ToString());
            }
            else
            {
              return "Capitale Sociale sottoscritto euro " + ConvertInteger(item["sottoscritto"].ToString());
            }

          }
        }
      }

      foreach (DataRow item in dati.Rows)
      {

        if (ConvertInteger(item["sottoscritto"].ToString()) == ConvertInteger(item["versato"].ToString()))
        {
          return "Capitale Sociale euro " + ConvertInteger(item["versato"].ToString()) + " I.V.";
        }
        else
        {
          return "Capitale Sociale sottoscritto euro " + ConvertInteger(item["sottoscritto"].ToString()) + " versato " + ConvertInteger(item["versato"].ToString());
        }

      }

      return CapitaleSociale;
    }



    public static string GetREA()
    {
      string REA = "";
      DataTable dati = GetData(35, typeof(Tabella), -1, -1, 1);
      bool cciaa = false;
      bool piva = false;

      foreach (DataRow item in dati.Rows)
      {
        if (item["value"].ToString() == "")
          continue;
        if (!cciaa && item["name"].ToString() == "CCIA - REA (luogo e numero)")
        {
          REA = "C.C.I.A.A " + item["value"].ToString() + " - ";
          cciaa = true;
        }

        if (!piva && item["name"].ToString() == "Partita IVA")
        {
          REA += "Registro imprese e Codice Fiscale " + item["value"].ToString();
          piva = true;
        }
      }
      return REA;
    }


    public static string GetIndirizzo()
    {
      string Indirizzo = "";
      DataTable dati = GetData(108, typeof(Tabella), -1, -1, 1);
      bool ind = false;
      bool CAP = false;
      bool citta = false;
      bool provincia = false;

      foreach (DataRow item in dati.Rows)
      {
        if (item["value"].ToString() == "")
          continue;

        if (!ind && item["name"].ToString() == "Indirizzo")
        {
          Indirizzo += " " + item["value"].ToString();
          ind = true;
        }
        if (!CAP && item["name"].ToString() == "CAP")
        {
          Indirizzo += " " + item["value"].ToString();
          CAP = true;
        }
        if (!citta && item["name"].ToString() == "Città / Località")
        {
          Indirizzo += " " + item["value"].ToString();
          citta = true;
        }
        if (!provincia && item["name"].ToString() == "Provincia")
        {
          Indirizzo += " " + item["value"].ToString();
          provincia = true;
        }

      }
      if (Indirizzo == "")
        Indirizzo = "Dato Mancante: Compilare Carta di Lavoro 2.1.1";

      return Indirizzo;
    }

    public static string GetRagioneSociale()
    {
      DataTable dati = GetData(108, typeof(Tabella), cBusinessObjects.idcliente, -1, 1);
      foreach (DataRow dd in dati.Rows)
      {
        if (dd["value"].ToString() == "")
          continue;
        if (dd["name"].ToString() == "Ragione Sociale")
        {
          return dd["value"].ToString();
        }
      }
      return "Dato Mancante: Compilare Carta di Lavoro 2.1.1";
    }




    public static int GetLastIDCliente()
    {
      int lastid;
      SqlConnection conn = new SqlConnection(getconnectiondb());
      conn.Open();
      SqlCommand cmd = new SqlCommand("UPDATE Last_ID_Cliente set LAST_ID_CLIENTE= LAST_ID_CLIENTE+1", conn);
      cmd.ExecuteNonQuery();
      cmd = new SqlCommand("SELECT Last_ID_Cliente FROM LAST_ID_CLIENTE", conn);
      lastid = int.Parse(cmd.ExecuteScalar().ToString());
      conn.Close();
      return lastid;
    }


    public static void DeleteCliente(int id)
    {

      try
      {
        SqlConnection conn = new SqlConnection(getconnectiondb());
        conn.Open();
        DataTable schemaTable = conn.GetSchema("Columns");
        foreach (DataRow row in schemaTable.Rows)
        {
          if (row["COLUMN_NAME"].ToString() == "ID_CLIENTE")
          {
            try
            {
              string nomeclasse = "RevisoftApplication." + row["TABLE_NAME"].ToString() + ", RevisoftApplication";
              DataTable dataTable = CreateDataTable(Type.GetType(cBusinessObjects.getfullnomeclass(row["TABLE_NAME"].ToString())));

              SqlCommand cmd = new SqlCommand("DELETE from " + row["TABLE_NAME"].ToString() + " WHERE ID_CLIENTE=" + id.ToString(), conn);
              cmd.ExecuteNonQuery();

            }
            catch (Exception exception)
            {
              logger.Error(exception, "DeleteCliente exception");

            }

          }

        }
        conn.Close();

      }
      catch (Exception exception)
      {
        logger.Error(exception, "DeleteCliente exception");

      }
      return;
    }

    public static string GetStato(int id, string idcliente, string idsessione,string idt="")
    {
      try
      {
         int id_tree;
         if(idt!="")
            id_tree = GetIDTree(id,int.Parse(idt));
         else
          id_tree = GetIDTree(id);

        SqlConnection conn = new SqlConnection(getconnectiondb());
        conn.Open();
        SqlCommand cmd = new SqlCommand("SELECT Stato FROM StatoNodi WHERE ID_SCHEDA=" + id_tree.ToString() + " AND ID_SESSIONE=" + idsessione + " AND ID_CLIENTE=" + idcliente, conn);

        string st = cmd.ExecuteScalar().ToString().Trim();
        conn.Close();
        return st;
      }
      catch (Exception)
      {
        return "";
      }
    }


    public static void DeleteSessione(string area, int id, string idcliente)
    {
      
     

      DeleteSessionData(area, id, idcliente);
      cBusinessObjects.logger.Info("cBusinessObjects >> DeleteSessione: {0} ", id);
      try
      {
        SqlConnection conn = new SqlConnection(getconnectiondb());
        conn.Open();
        SqlCommand cmd = new SqlCommand("DELETE FROM TabellaSessioni WHERE Area='" + area + "' AND ID_SESSIONE=" + id.ToString() + " AND ID_CLIENTE=" + idcliente, conn);
        cmd.ExecuteNonQuery();
        conn.Close();

      }
      catch (Exception exception)
      {
        logger.Error(exception, "DeleteTree exception");

      }

    }

    //------------------------------------------------------------------------+
    //                           DeleteSessionData                            |
    //------------------------------------------------------------------------+
    public static void DeleteSessionData(string area,int idsessione,string idcliente)
    {
      string str;
      DataTable dtSchema;
      int idTree,index;
       string[] arrAree =
      {
        "Bilancio", "Conclusione", "Incarico", "IncaricoCS", "IncaricoSU", "IncaricoREV", "ISQC",
        "PianificazioniVerifica", "PianificazioniVigilanza", "RelazioneB",
        "RelazioneBC", "RelazioneBV", "RelazioneV", "RelazioneVC", "Revisione",
        "Verifica", "Vigilanza"
      };
      int[] arrIdTrees = { 4, 19, 3,71,72,73, 28, 26, 27, 21, 31, 23, 22, 32, 1, 2, 18 };

      str = string.Format("cBusinessObjects >> DeleteSessionData({0},{1},{2})",
        area, idsessione, idcliente);
      logger.Info(str);
      index = Array.IndexOf(arrAree, area);
      if (index < 0)
      {
        str=string.Format("DeleteSessionData(): area '{0}' non trovata", area);
        logger.Error("errore",str);
        return;
      }
      idTree = arrIdTrees[index];
      try
      {
        using (SqlConnection conn=new SqlConnection(getconnectiondb()))
        {
          conn.Open();
          dtSchema = conn.GetSchema("Columns");
          using (SqlCommand cmd = new SqlCommand())
          {
            cmd.Connection = conn;
            foreach (DataRow dr in dtSchema.Rows)
            {
              // cancellare dati da questa tabella
              if (dr["COLUMN_NAME"].ToString() == "ID_SCHEDA")
              {
                cmd.CommandText = string.Format(
                  "delete from {0} " +
                  "where (ID_SCHEDA/10000000={1}) " +
                    "and (ID_CLIENTE={2}) " +
                    "and (ID_SESSIONE={3})",
                  dr["TABLE_NAME"].ToString(), idTree, idcliente, idsessione);
                cmd.ExecuteNonQuery();
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        logger.Error(ex, "DeleteSessionData() --> errore");
      }
    }

    public static string CercaSessione(string areafrom, string areato, string idsessionefrom, int idcliente)
    {

      cBusinessObjects.logger.Info("cBusinessObjects >> CercaSessione");
            string data = "";
      string sessioneto = "-9";
      try
      {
        SqlConnection conn = new SqlConnection(getconnectiondb());
        SqlCommand cmd;
        conn.Open();
        if (areafrom == "")
          cmd = new SqlCommand("SELECT Data FROM TabellaSessioni  WHERE ID_SESSIONE=" + idsessionefrom + " AND ID_CLIENTE=" + idcliente.ToString(), conn);
        else
          cmd = new SqlCommand("SELECT Data FROM TabellaSessioni  WHERE Area='" + areafrom + "' AND ID_SESSIONE=" + idsessionefrom + " AND ID_CLIENTE=" + idcliente.ToString(), conn);

        data = (String)cmd.ExecuteScalar();

        if (data != null)
        {
          SqlCommand cmd2;
          cmd2 = new SqlCommand("SELECT ID_SESSIONE FROM TabellaSessioni  WHERE Area='" + areato + "'  AND Data='" + data + "' AND ID_CLIENTE=" + idcliente.ToString(), conn);
          sessioneto = cmd2.ExecuteScalar().ToString();
        }
        conn.Close();

      }
      catch (Exception exception)
      {
        logger.Error(exception, "CercaSessione exception");

      }
      return sessioneto;
    }

    public static string VerificaSessione(string area, string datavalue, int idsessionefrom, int idclientefrom)
    {

      cBusinessObjects.logger.Info("cBusinessObjects >> CercaSessione");
      string data = "";
      string sessioneto = "-1";
      try
      {
        SqlConnection conn = new SqlConnection(getconnectiondb());
        SqlCommand cmd;
        conn.Open();
        cmd = new SqlCommand("SELECT Data FROM TabellaSessioni  WHERE Area='" + area + "' AND ID_SESSIONE=" + idsessionefrom.ToString() + " AND ID_CLIENTE=" + idclientefrom.ToString(), conn);

        data = (String)cmd.ExecuteScalar();

        if (data == null)
        {

        string versione_sessione="";  
        if (area != "IncaricoCS" && area != "Incarico" && area != "IncaricoSU" && area != "IncaricoREV")
            versione_sessione = App.AppVersione;

          cmd = new SqlCommand("INSERT INTO TabellaSessioni VALUES(" + idclientefrom.ToString() + "," + idsessionefrom.ToString() + ",'" + area + "','" + datavalue.Trim(' ') + "','"+versione_sessione+"')", conn);
          cmd.ExecuteNonQuery();
        }
        conn.Close();

      }
      catch (Exception exception)
      {
        logger.Error(exception, "VerificaSessione exception");

      }
      return sessioneto;
    }


    public static void AddSessione(string area, string datavalue, int idsessione, int idcliente)
    {

     string versione_sessione = "";
      cBusinessObjects.logger.Info("cBusinessObjects >> AddSessione: {0} ", datavalue);
      try
      {
        SqlConnection conn = new SqlConnection(getconnectiondb());
        SqlCommand cmd;
        conn.Open();
       
   
       
        if(!SessioneIsNew)
        {
            string tmp_versione_sessione = "";
            cmd = new SqlCommand("SELECT Versione FROM TabellaSessioni  WHERE  Area='" + area + "' AND ID_SESSIONE=" + idsessione.ToString()  + " AND ID_CLIENTE=" + idcliente.ToString(), conn);  
            try
              {
                tmp_versione_sessione = (String)cmd.ExecuteScalar();
              }
              catch (Exception aa)
              {

              }
           
            if (tmp_versione_sessione != "")
            {
                versione_sessione = tmp_versione_sessione;
            }
        }
        if(versione_sessione=="")
        {
               versione_sessione = App.AppVersione;
        }

        SessioneIsNew = false;

        if (area != "IncaricoCS" && area != "Incarico" && area != "IncaricoSU" && area != "IncaricoREV")
            versione_sessione = App.AppVersione;
        cmd = new SqlCommand("DELETE FROM TabellaSessioni WHERE Area='" + area + "' AND ID_SESSIONE=" + idsessione.ToString() + " AND ID_CLIENTE=" + idcliente.ToString(), conn);
        cmd.ExecuteNonQuery();
        cmd = new SqlCommand("INSERT INTO TabellaSessioni VALUES(" + idcliente.ToString() + "," 
        + idsessione.ToString() + ",'" 
        + area + "','" 
        + datavalue.Trim(' ')  + "','" 
        + versione_sessione
        + "')", conn);
        cmd.ExecuteNonQuery();
        conn.Close();

      }
      catch (Exception exception)
      {
        logger.Error(exception, "AddSessione exception");

      }

    }

    public static void DeleteTree(string id, string idcliente)
    {
      cBusinessObjects.logger.Info("cBusinessObjects >> DeleteTree: {0} ", id);
      int id_tree = GetIDTree(0, int.Parse(id));
      try
      {
        SqlConnection conn = new SqlConnection(getconnectiondb());
        SqlCommand cmd;
        conn.Open();
        if (id == "-1")
          cmd = new SqlCommand("DELETE FROM SessioniXMLData WHERE ID_CLIENTE=" + idcliente, conn);
        else
          cmd = new SqlCommand("DELETE FROM SessioniXMLData WHERE ID_TREE=" + id_tree + " AND ID_CLIENTE=" + idcliente, conn);
        cmd.ExecuteNonQuery();
        conn.Close();

      }
      catch (Exception exception)
      {
        logger.Error(exception, "DeleteTree exception");

      }

    }
    

    

    public static void show_workinprogress(string messaggio)
    {

      return;

      Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,new Action(delegate { }));
      if (pd != null)
        return;
      pd = new wLavoriInCorso(messaggio);
      pd.Show();
    }

    public static void hide_workinprogress()
    {
      Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,new Action(delegate { }));
      if (pd != null)
        pd.Close();
      pd = null;
    }



    public static int CheckAggiornamenti()
    {

      string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\upgrades_db";
      bool checkfiles = true;
      int currentscript = 1;
      System.IO.Directory.CreateDirectory(path);

      while (checkfiles)
      {
        try
        {
          string fileName = "script_" + currentscript.ToString() + ".sql";
          var request = (HttpWebRequest)WebRequest.Create(url_script_db + "/" + fileName);
          request.Method = "GET";
          using (var response = request.GetResponse())
          {
            using (var responseStream = response.GetResponseStream())
            {

              if (!File.Exists(path + "\\" + fileName))
              {
                using (var fileToDownload = new System.IO.FileStream(path + "\\" + fileName, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
                {
                  responseStream.CopyTo(fileToDownload);
                }
                string contents = File.ReadAllText(path + "\\" + fileName);
                ExecuteSqlScript(contents);
              }
            }
          }
          currentscript++;
        }
        catch (Exception)
        {
          checkfiles = false;
        }
      }

      return 0;
    }

    private static Regex _sqlScriptSplitRegEx = new Regex(@"^\s*GO\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

    public static void ExecuteSqlScript(string scriptText)
    {
      if (string.IsNullOrEmpty(scriptText))
        return;

      var scripts = _sqlScriptSplitRegEx.Split(scriptText);

      foreach (var scriptLet in scripts)
      {
        if (scriptLet.Trim().Length == 0)
          continue;
        Executesql(scriptLet);
      }

    }


    public static DataTable ClassToDataTable(Type T)
    {
      DataTable dt = new DataTable();

      foreach (PropertyInfo info in T.GetProperties())
      {
        dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
      }
      return dt;

    }

    public static DataTable CreateDataTable(Type T)
    {
      cBusinessObjects.logger.Info("NOME TABELLA"+T.Name);
      DataTable dataTable = new DataTable(T.Name);
      try
      {

        foreach (PropertyInfo info in T.GetProperties())
        {
       
          dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
        }


      }
      catch (Exception exception)
      {
        cBusinessObjects.logger.Error(exception, "cBusinessObjects.CreateDataTable exception");
        return dataTable;
      }

      return dataTable;
    }

  

    public static List<string> FindTablesById(int id, int idt = -1)
    {

      int id_tree = GetIDTree(id);
      if (idt != -1)
        id_tree = GetIDTree(id, idt);

      List<string> listaschede = new List<string>();
      try
      {
        SqlConnection conn = new SqlConnection(getconnectiondb());
        conn.Open();
        DataTable schemaTable = conn.GetSchema("Columns");


        foreach (DataRow row in schemaTable.Rows)
        {
          if (row["COLUMN_NAME"].ToString() == "ID_SCHEDA")
          {
            try
            {
              string nomeclasse = "RevisoftApplication." + row["TABLE_NAME"].ToString() + ", RevisoftApplication";
           
              DataTable dataTable = null;

              logger.Info( "FindTablesById "+getfullnomeclass(row["TABLE_NAME"].ToString()));
              dataTable = CreateDataTable(Type.GetType(getfullnomeclass(row["TABLE_NAME"].ToString())));

              SqlCommand cmd = new SqlCommand("select * from " + row["TABLE_NAME"].ToString() + " WHERE ID_SCHEDA=" + id_tree.ToString() + " AND ID_SESSIONE=" + idsessione.ToString() + " AND ID_CLIENTE=" + idcliente.ToString(), conn);
              using (SqlDataReader dr = cmd.ExecuteReader())
              {
                dataTable.Load(dr);
                if (dataTable.Rows.Count > 0)
                  listaschede.Add(row["TABLE_NAME"].ToString());
              }
            }
            catch (Exception exception)
            {
              logger.Error(exception, "FindTablesById exception");

            }

          }

        }
        conn.Close();

      }
      catch (Exception exception)
      {
        logger.Error(exception, "FindTablesById exception");

      }
      return listaschede;
    }

    public static void Executesql(string query)
    {
      cBusinessObjects.logger.Info("cBusinessObjects >> Executesql query: {0} ", query);
      try
      {
        SqlConnection conn = new SqlConnection(getconnectiondb());
        conn.Open();
        SqlCommand cmd = new SqlCommand(query, conn);
        cmd.ExecuteNonQuery();
        conn.Close();

      }
      catch (Exception exception)
      {
        logger.Error(exception, "Executesql exception");

      }

    }

    public static DataTable ExecutesqlDataTable(string query)
    {
      cBusinessObjects.logger.Info("cBusinessObjects >> ExecutesqlDataTable query: {0} ", query);
      DataTable dataTable = new DataTable();
      try
      {
        SqlConnection conn = new SqlConnection(getconnectiondb());
        conn.Open();
        SqlCommand cmd = new SqlCommand(query, conn);
        using (SqlDataReader dr = cmd.ExecuteReader())
        {

          dataTable.Load(dr);
          foreach (DataRow row in dataTable.Rows)
          {
            foreach (DataColumn col in dataTable.Columns)
            {
              if (row.IsNull(col) && col.DataType == typeof(string))
                row.SetField(col, String.Empty);
            }
          }
        }
        conn.Close();
        return dataTable;
      }
      catch (Exception exception)
      {
        logger.Error(exception, "ExecutesqlDataTable exception");
      }
      return dataTable;
    }

    public static int Gest_ID_SCHEDA(string id, int idtree)
    {

      return int.Parse(id) - idtree * 10000000;

    }

   public static int GetIDTree(int id, int idtree = -10)
    {

      if (idtree != -10)
        return idtree * 10000000 + id;

      int id_tree = 0;
      foreach (var Window in App.Current.Windows)
      {
        if (Window.GetType().Name == "WindowWorkAreaTree")
        {
          id_tree = int.Parse(((RevisoftApplication.WindowWorkAreaTree)Window).IDTree);
        }
      }
    
      string versione=  CercaVersione(id_tree.ToString());
      if(versione=="")
      {       
        if(id_tree==71 || id_tree==72 || id_tree==73)
            id_tree=3;
      }
      
       return id_tree * 10000000 + id;

    }




    public static DataTable GetData(int id, Type T, int idcli = -1, int idsess = -10, int idtree = -10)
    {

      int id_tree = GetIDTree(id, idtree);


      cBusinessObjects.logger.Info("cBusinessObjects >> GetData ID: {0} Type: {1}", id, T.Name);
      DataTable dataTable = CreateDataTable(T);
      try
      {

        SqlConnection conn = new SqlConnection(getconnectiondb());
        conn.Open();

        SqlCommand cmd = new SqlCommand("getDataTable", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        SqlParameter param1 = new SqlParameter();
        param1.ParameterName = "@ID_SCHEDA";
        if (id != -1)
          param1.Value = id_tree;
        else
          param1.Value = id;

        cmd.Parameters.Add(param1);
        SqlParameter param2 = new SqlParameter();
        param2.ParameterName = "@ID_CLIENTE";
        if (idcli != -1)
          param2.Value = idcli;
        else
          param2.Value = idcliente;
        cmd.Parameters.Add(param2);
        SqlParameter param3 = new SqlParameter();
        param3.ParameterName = "@ID_SESSIONE";
        if (idsess != -10)
          param3.Value = idsess;
        else
          param3.Value = idsessione;
        cmd.Parameters.Add(param3);
        SqlParameter param4 = new SqlParameter();
        param4.ParameterName = "@TABELLA";
        param4.Value = T.Name;
        cmd.Parameters.Add(param4);

        using (SqlDataReader dr = cmd.ExecuteReader())
        {

          dataTable.Load(dr);
          foreach (DataRow row in dataTable.Rows)
          {
            foreach (DataColumn col in dataTable.Columns)
            {
              if (row.IsNull(col) && col.DataType == typeof(string))
                row.SetField(col, String.Empty);
            }
          }

        }
        DataColumnCollection columns = dataTable.Columns;
        if (columns.Contains("ID"))
        {

          DataView dv = dataTable.DefaultView;
          dv.Sort = "ID";
          DataTable sortedDT = dv.ToTable();
          conn.Close();
          return sortedDT;

        }
        conn.Close();
        return dataTable;

      }
      catch (Exception exception)
      {
        logger.Error(exception, "cBusinessObjects.GetData exception");
        return dataTable;
      }


    }

    public static DataTable SetDataFiltered(DataTable dt, DataTable datiT, string head, string headcolumn)
    {
      bool first = true;
      DataTable datiC = datiT.Clone();
      foreach (DataRow dtrow in datiT.Rows)
      {
        if (dtrow[headcolumn] != null)
        {
          if (dtrow[headcolumn].ToString() != head)
          {
            DataRow firstNewRow = datiC.NewRow();
            firstNewRow.ItemArray = dtrow.ItemArray;
            datiC.Rows.Add(firstNewRow);
          }
          else
          {
            if (first)
            {
              foreach (DataRow dtrow2 in dt.Rows)
              {
                DataRow firstNewRow = datiC.NewRow();
                firstNewRow.ItemArray = dtrow2.ItemArray;
                datiC.Rows.Add(firstNewRow);
              }
              first = false;
            }

          }
        }
      }
      if (first)
      {
        foreach (DataRow dtrow2 in dt.Rows)
        {
          DataRow firstNewRow = datiC.NewRow();
          firstNewRow.ItemArray = dtrow2.ItemArray;
          datiC.Rows.Add(firstNewRow);
        }
      }
      return datiC;
    }



    public static DataTable GetDataFiltered(DataTable dt, string head, string headcolumn)
    {
      DataTable datiC = dt.Clone();
      foreach (DataRow dtrow in dt.Rows)
      {
        if (dtrow[headcolumn] != null)
        {
          if (dtrow[headcolumn].ToString() == head)
          {
            DataRow firstNewRow = datiC.NewRow();
            firstNewRow.ItemArray = dtrow.ItemArray;
            datiC.Rows.Add(firstNewRow);
          }
        }
      }
      DataColumnCollection columns = datiC.Columns;
      if (columns.Contains("ID"))
      {
        DataView dv = datiC.DefaultView;
        dv.Sort = "ID";
        DataTable sortedDT = dv.ToTable();
        return sortedDT;
      }
      return datiC;
    }


    public static string GetVersioneSessione(string ids,string area)
        {
        string versione_sessione = "";
        string tmp_versione_sessione = "";
        try
        {
          SqlConnection conn = new SqlConnection(getconnectiondb());
          SqlCommand cmd;
          conn.Open();
          cmd = new SqlCommand("SELECT Versione FROM TabellaSessioni  WHERE Area='" + area + "' AND ID_SESSIONE=" + ids + " AND ID_CLIENTE=" + idcliente.ToString(), conn);
          try
          {
            tmp_versione_sessione = (String)cmd.ExecuteScalar();
          }
          catch (Exception aa)
          {
        
          }

          if (tmp_versione_sessione != null)
            {
                versione_sessione = tmp_versione_sessione;
            }
        }
        catch(Exception ee)
          {
            logger.Error(ee, "cBusinessObjects.GetVersioneSessione exception");
          }
       
          if (area != "IncaricoCS" && area != "Incarico" && area != "IncaricoSU" && area != "IncaricoREV")
            versione_sessione = App.AppVersione;

           return versione_sessione;
        }
    
   public static string CercaVersione(string idt)
        {
              string tmp_versione_sessione= "";
              try
              {
               string[] arrAree =
               {
                "Bilancio", "Conclusione", "Incarico", "IncaricoCS", "IncaricoSU", "IncaricoREV", "ISQC",
                "PianificazioniVerifica", "PianificazioniVigilanza", "RelazioneB",
                "RelazioneBC", "RelazioneBV", "RelazioneV", "RelazioneVC", "Revisione",
                "Verifica", "Vigilanza"
                };
               int[] arrIdTrees = { 4, 19, 3,71,72,73, 28, 26, 27, 21, 31, 23, 22, 32, 1, 2, 18 };
               int index = Array.IndexOf(arrIdTrees, int.Parse(idt));
               string area = arrAree[index];
               SqlConnection conn = new SqlConnection(getconnectiondb());
               SqlCommand cmd;
               conn.Open();
               cmd = new SqlCommand("SELECT Versione FROM TabellaSessioni  WHERE Area='" + area  + "' AND ID_SESSIONE=" + idsessione.ToString()  + " AND ID_CLIENTE=" + idcliente.ToString(), conn);
               logger.Info("SELECT Versione FROM TabellaSessioni  WHERE Area='" + area + "' AND ID_SESSIONE=" + idsessione.ToString() + " AND ID_CLIENTE=" + idcliente.ToString());
               try
               {
                  tmp_versione_sessione = (String)cmd.ExecuteScalar();
               }
               catch(Exception aa)
               {
                        tmp_versione_sessione = "";
               }
           
                if (tmp_versione_sessione!=null && tmp_versione_sessione != ""  )
                {
                    return tmp_versione_sessione;
                }
              }
               catch(Exception aa)
               {
                    
               }
              return "";
       }

   public static XmlDocument NewLoadEncodedFile(string xFile, string idtree = "")
     {
        XmlManager x = new XmlManager();
        XmlDocument doctree = null;

        string item = xFile.Split('\\').Last();
        string versione_sessione = "";
     
          x.CheckXmlCache();
          if(idtree!="")
          {    
              
            versione_sessione = CercaVersione(idtree);
           
          }

          if(versione_sessione!="")
          {
          
                if (App.m_xmlCache.ContainsKey(versione_sessione+item))
                {
                  doctree = ((XMLELEMENT)App.m_xmlCache[versione_sessione+item]).doc;
                }
                else
                {
                 doctree = GetAlbero(item, idtree);

                 if (doctree != null && doctree.OuterXml != "")
                  {
                    App.m_xmlCache.Add(versione_sessione+item, new XMLELEMENT(doctree, false));
                  }
                 }
          }
          else
          {
            if (App.m_xmlCache.ContainsKey(item))
            {
               doctree = ((XMLELEMENT)App.m_xmlCache[item]).doc;
            }
            else
            {
                XmlNode nn = null;
                doctree = StaticUtilities.BuildXML(item);
                if (doctree != null)
                  App.m_xmlCache.Add(item, new XMLELEMENT(doctree, false));
            }
          
          }
            

      

      return doctree;

    }
  
    public static int SaveData(int id, DataTable dt, Type T, int id_scheda = -1, int idt = -1)
    {
      logger.Info("SaveData ID: {0} Type: {1} Rows:{2}", id, T.Name, dt.Rows.Count);
      try
      {

        int id_tree = GetIDTree(id);
        if (idt != -1)
        {
          id_tree = GetIDTree(id, idt);
        }
        if (id != -1) // ArchivioDocumenti mette id =-1 
        {
          foreach (DataRow dtrow in dt.Rows)
          {
            if (id_scheda != -1)
              dtrow["ID_SCHEDA"] = id_scheda;
            else
              dtrow["ID_SCHEDA"] = id_tree;

            dtrow["ID_CLIENTE"] = idcliente;
            dtrow["ID_SESSIONE"] = idsessione;
          }
        }
        SqlConnection conn = new SqlConnection(getconnectiondb());
        conn.Open();
        SqlCommand cmd = new SqlCommand("putDataTable" + T.Name, conn);
        cmd.CommandType = CommandType.StoredProcedure;
        SqlParameter param1 = new SqlParameter();
        param1.ParameterName = "@ID_SCHEDA";
        if (id != -1) // ArchivioDocumenti mette id =-1 
        {
          if (id_scheda != -1)
            param1.Value = id_scheda;
          else
            param1.Value = id_tree;
        }
        else
        {
          param1.Value = -1;
        }

        cmd.Parameters.Add(param1);
        SqlParameter param2 = new SqlParameter();
        param2.ParameterName = "@ID_CLIENTE";
        param2.Value = idcliente;
        cmd.Parameters.Add(param2);
        SqlParameter param3 = new SqlParameter();
        param3.ParameterName = "@ID_SESSIONE";
        if (id != -1) // ArchivioDocumenti mette id =-1 
        {
          param3.Value = idsessione;
        }
        else
        {
          param3.Value = -1;
        }

        cmd.Parameters.Add(param3);
        SqlParameter sqlParam2 = cmd.Parameters.AddWithValue("@OBJDATATABLE", dt);
        sqlParam2.SqlDbType = SqlDbType.Structured;
        cmd.ExecuteNonQuery();
        conn.Close();

      }
      catch (Exception exception)
      {
        logger.Error(exception, "cBusinessObjects.SaveData exception");
        return -1;
      }

      return 0;
    }

    public static string getfullnomeclass(string nomec)
    {

      var t = typeof(Tabella); //una classe qualsiasi

      return t.AssemblyQualifiedName.Replace("Tabella", nomec);

    }
    public static Hashtable GetDataSessioniHT(string tipoHT,string versione,int idc = -1, int idt = -1)
    {
      int id_tree = GetIDTree(0);
      if (idt != -1)
      {
        id_tree = GetIDTree(0, idt);
      }

      cBusinessObjects.logger.Info("cBusinessObjects >> GetDataSessioniHT tipoHT: {0}", tipoHT);
      DataTable myDataTable = new DataTable();
      myDataTable.Columns.Add(new DataColumn("Key", typeof(int)));
      myDataTable.Columns.Add(new DataColumn("Value", typeof(string)));

      Hashtable htOut = new Hashtable();
      try
      {

        SqlConnection conn = new SqlConnection(getconnectiondb());
        conn.Open();

        SqlCommand cmd = new SqlCommand("getDataSessioniHT", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        SqlParameter param1 = new SqlParameter();
        param1.ParameterName = "@ID_TREE";
        param1.Value = id_tree;
        cmd.Parameters.Add(param1);
        SqlParameter param2 = new SqlParameter();
        param2.ParameterName = "@ID_CLIENTE";

        if (idc != -1)
          param2.Value = idc;
        else
          param2.Value = idcliente;

        cmd.Parameters.Add(param2);
        SqlParameter param3 = new SqlParameter();
        param3.ParameterName = "@ID_TIPO";
        param3.Value = tipoHT;
        cmd.Parameters.Add(param3);

       SqlParameter param4 = new SqlParameter();
        param4.ParameterName = "@VERSIONE";
        param4.Value = versione;
        cmd.Parameters.Add(param4);

        using (SqlDataReader dr = cmd.ExecuteReader())
        {

          myDataTable.Load(dr);

        }
        conn.Close();
        foreach (DataRow drIn in myDataTable.Rows)
        {
          htOut.Add(int.Parse(drIn["Key"].ToString()), drIn["Value"].ToString());
        }


      }
      catch (Exception exception)
      {
        logger.Error(exception, "cBusinessObjects.GetDataSessioniHT exception");
        return null;
      }

      return htOut;
    }


    public static int SaveSessioniHT(Hashtable ht, string tipoHT,string versione)
    {
      int id_tree = GetIDTree(0);

      logger.Info("SaveSessioniHT");
      try
      {

        DataTable myDataTable = new DataTable();
        myDataTable.Columns.Add(new DataColumn("ID_TREE", typeof(int)));
        myDataTable.Columns.Add(new DataColumn("ID_CLIENTE", typeof(int)));
        myDataTable.Columns.Add(new DataColumn("ID_TIPO", typeof(string)));
        myDataTable.Columns.Add(new DataColumn("Key", typeof(int)));
        myDataTable.Columns.Add(new DataColumn("Value", typeof(string)));
        myDataTable.Columns.Add(new DataColumn("versione", typeof(string)));
        foreach (DictionaryEntry item in ht)
        {
          DataRow myRow = myDataTable.NewRow();
          myRow[0] = id_tree;
          myRow[1] = idcliente;
          myRow[2] = tipoHT;
          myRow[3] = item.Key.ToString();
          myRow[4] = item.Value.ToString();
          myRow[5] = versione;
          myDataTable.Rows.Add(myRow);
        }

        SqlConnection conn = new SqlConnection(getconnectiondb());
        conn.Open();
        SqlCommand cmd = new SqlCommand("SaveSessioniHTData", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        SqlParameter param1 = new SqlParameter();
        param1.ParameterName = "@ID_TREE";
        param1.Value = id_tree;
        cmd.Parameters.Add(param1);
        SqlParameter param2 = new SqlParameter();
        param2.ParameterName = "@ID_CLIENTE";
        param2.Value = idcliente;
        cmd.Parameters.Add(param2);
        SqlParameter param3 = new SqlParameter();
        param3.ParameterName = "@ID_TIPO";
        param3.Value = tipoHT;
        cmd.Parameters.Add(param3);

        SqlParameter param4 = new SqlParameter();
        param4.ParameterName = "@VERSIONE";
        param4.Value = versione;
        cmd.Parameters.Add(param4);
    
        SqlParameter sqlParam2 = cmd.Parameters.AddWithValue("@OBJDATATABLE", myDataTable);
        sqlParam2.SqlDbType = SqlDbType.Structured;
        cmd.ExecuteNonQuery();
        conn.Close();

      }
      catch (Exception exception)
      {
        logger.Error(exception, "cBusinessObjects.SaveSessioniHT exception");
        return -1;
      }

      return 0;
    }

    public static XmlDocument GetDataSessioniFile(string versione,int idt = -1, int idcli = -1)
    {
      int id_tree = GetIDTree(0);

      if (idt != -1)
      {
        id_tree = GetIDTree(0, idt);
      }

      cBusinessObjects.logger.Info("cBusinessObjects >> GetDataSessioniFile ");

      XmlDocument doc = new XmlDocument();
      try
      {

        SqlConnection conn = new SqlConnection(getconnectiondb());
        conn.Open();

        SqlCommand cmd = new SqlCommand("getDataSessioniFile", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        SqlParameter param1 = new SqlParameter();
        param1.ParameterName = "@ID_TREE";
        param1.Value = id_tree;
        cmd.Parameters.Add(param1);
        SqlParameter param2 = new SqlParameter();
        param2.ParameterName = "@ID_CLIENTE";
        if (idcli != -1)
        {
          param2.Value = idcli;
        }
        else
        {
          param2.Value = idcliente;
        }

        cmd.Parameters.Add(param2);

       SqlParameter param4 = new SqlParameter();
        param4.ParameterName = "@VERSIONE";
        param4.Value = versione;
        cmd.Parameters.Add(param4);
        using (var rdr = cmd.ExecuteReader())
        {
          while (rdr.Read())
          {
            var xr = rdr.GetSqlXml(0);
            doc.LoadXml(xr.Value);


          }
        }

        conn.Close();

      }
      catch (Exception exception)
      {
        logger.Error(exception, "cBusinessObjects.GetDataSessioniFile exception");
        return null;
      }

      return doc;
    }

    public static int SaveAlbero(string xmlstring, int idtree)
    {

      var doc = XDocument.Parse(xmlstring);
      logger.Info("SaveAlbero");
      try
      {
        SqlConnection conn = new SqlConnection(getconnectiondb());
        conn.Open();
        SqlCommand cmd = new SqlCommand("SaveAlbero", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        var pDoc = cmd.Parameters.Add("@XMLSTRING", System.Data.SqlDbType.Xml);
        pDoc.Value = doc.CreateReader();
        SqlParameter param4 = new SqlParameter();
        param4.ParameterName = "@ID_TREE";
        param4.Value = idtree;
        cmd.Parameters.Add(param4);
        cmd.ExecuteNonQuery();
        conn.Close();

      }
      catch (Exception exception)
      {
        logger.Error(exception, "cBusinessObjects.SaveAlbero exception");
        return -1;
      }

      return 0;
    }


    public static XmlDocument GetAlbero(string guid, string idt = "")
    {


      cBusinessObjects.logger.Info("cBusinessObjects >> GetAlbero ");

      XmlDocument doc = new XmlDocument();
      try
      {

        SqlConnection conn = new SqlConnection(getconnectiondb());
        conn.Open();

        SqlCommand cmd = new SqlCommand("getAlbero", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        SqlParameter param1 = new SqlParameter();

        param1.ParameterName = "@ID_TREE";
        if (idt == "")
          param1.Value = -1;
        else
          param1.Value = int.Parse(idt);
        cmd.Parameters.Add(param1);

        SqlParameter param2 = new SqlParameter();

        param2.ParameterName = "@XMLGUID";
        if (idt == "")
          param2.Value = guid;
        else
          param2.Value = "";

        cmd.Parameters.Add(param2);

        using (var rdr = cmd.ExecuteReader())
        {
          while (rdr.Read())
          {
            var xr = rdr.GetSqlXml(0);
            doc.LoadXml(xr.Value);
          }
        }

        conn.Close();

      }
      catch (Exception exception)
      {
        logger.Error(exception, "cBusinessObjects.getAlbero exception");
        return null;
      }

      return doc;
    }

    public static int SaveSessioniFile(string xmlstring, string xmlguid,string versione)
    {
      int id_tree = GetIDTree(0);
      var doc = XDocument.Parse(xmlstring);
      XmlDocument xmlToSave = new XmlDocument();
      xmlToSave.LoadXml(xmlstring);

   
      logger.Info("SaveSessioniFile");
      try
      {
        SqlConnection conn = new SqlConnection(getconnectiondb());
        conn.Open();
        SqlCommand cmd = new SqlCommand("SaveSessioniFile", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        SqlParameter param1 = new SqlParameter();
        param1.ParameterName = "@ID_TREE";
        param1.Value = id_tree;
        cmd.Parameters.Add(param1);
        SqlParameter param2 = new SqlParameter();
        param2.ParameterName = "@ID_CLIENTE";
        param2.Value = idcliente;
        cmd.Parameters.Add(param2);
        var pDoc = cmd.Parameters.Add("@XMLSTRING", System.Data.SqlDbType.Xml);
        //  pDoc.Value = doc.CreateReader();
        pDoc.Value = new System.Data.SqlTypes.SqlXml(new XmlTextReader(xmlToSave.InnerXml, XmlNodeType.Document, null));
        SqlParameter param4 = new SqlParameter();
        param4.ParameterName = "@XMLGUID";
        param4.Value = xmlguid;
        cmd.Parameters.Add(param4);

       SqlParameter param5 = new SqlParameter();
        param5.ParameterName = "@VERSIONE";
        param5.Value = versione;
        cmd.Parameters.Add(param5);
        cmd.ExecuteNonQuery();
        conn.Close();

      }
      catch (Exception exception)
      {
        logger.Error(exception, "cBusinessObjects.SaveSessioniFile exception");
        return -1;
      }

      return 0;
    }


    public static string ConvertNumber(string valore)
    {
      double dblValore = 0.0;

      double.TryParse(valore, out dblValore);

      if (dblValore == 0.0)
      {
        return "";
      }
      else
      {
        return String.Format("{0:#,#.00}", dblValore);
      }
    }
    public static string ConvertNumber(string valore, int dec)
    {
      double dblValore = 0.0;

      double.TryParse(valore, out dblValore);

      if (dblValore == 0.0)
      {
        return "";
      }
      else
      {
        if (dec == 0)
          return String.Format("{0:#}", dblValore);
        if (dec == 1)
          return String.Format("{0:#,0}", dblValore);
        if (dec == 2)
          return String.Format("{0:#,00}", dblValore);

        return String.Format("{0:#,00}", dblValore);
      }
    }

    public static string ConvertInteger(string valore)
    {
      double dblValore = 0.0;

      double.TryParse(valore, out dblValore);

      if (dblValore == 0.0)
      {
        return "0";
      }
      else
      {
        return String.Format("{0:#,0}", dblValore);
      }
    }

    public static string getconnectiondb()
    {
      if (!string.IsNullOrEmpty(connection_db))
        return connection_db;

      string user, catalog, server, iniFilePath, connString;

      iniFilePath = System.IO.Path.GetFullPath(System.Reflection.Assembly.GetEntryAssembly().Location);
      iniFilePath = iniFilePath.Replace(".exe", ".ini");
      INIFile iniFile = new INIFile(iniFilePath);
      user = Environment.UserName;
      server = iniFile.Read(user, "server");

      if (server == "")
      {
        IEnumerable<string> lista = SqlHelper.ListLocalSqlInstances();

        foreach (string istanza in lista)
        {
          if (istanza.IndexOf("SQLREVISOFT") != -1)
          {
            server = istanza;
            break;
          }
        }

      }
      catalog = iniFile.Read(user, "catalog");
      if (catalog == "") catalog = "Revisoft_" + user;
      iniFile.Write(user, "server", server);
      iniFile.Write(user, "catalog", catalog);
      //----------------------------------------------------------------------------+
      //                   verifica connessione a database utente                   |
      //----------------------------------------------------------------------------+
      connString = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True", server, catalog);
      using (SqlConnection conn = new SqlConnection(connString))
      {
        try { conn.Open(); }
        catch (Exception ex)
        {
          logger.Error(ex, "cBusinessObjects.SaveData exception");
          MessageBox.Show(ex.Message);
        }
        conn.Close();
      }
      connection_db = connString;
      return connString;
    }

    public static string striptagsfromrtf(string rtftext)
    {

        System.Windows.Forms.RichTextBox rtBox = new System.Windows.Forms.RichTextBox();
        try{
          rtBox.Rtf = rtftext;
          return rtBox.Text.ToString();
        }
        catch(Exception aa)
        {
           return "";
        }
      
    }

  }
}
