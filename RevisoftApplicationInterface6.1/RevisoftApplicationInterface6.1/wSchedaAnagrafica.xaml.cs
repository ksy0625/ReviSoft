//----------------------------------------------------------------------------+
//                         wSchedaAnagrafica.xaml.cs                          |
//----------------------------------------------------------------------------+
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using System.ComponentModel;
using System.Collections;
using RevisoftApplication.BRL;

namespace RevisoftApplication
{
  public partial class wSchedaAnafrafica : Window
  {
    public App.TipoAttivitaScheda TipologiaAttivita;
    public int idRecord = 0;
    private bool _InCaricamento;
    private bool _DatiCambiati;
    public bool RegistrazioneEffettuata;
    private bool annulla = false;
    //Team
    private Dictionary<int, Utente> _teamList;
    private int _TeamLeadeOld = -1;

    //----------------------------------------------------------------------------+
    //                             wSchedaAnafrafica                              |
    //----------------------------------------------------------------------------+
    public wSchedaAnafrafica()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      //var
      idRecord = App.MasterFile_NewID;
      _InCaricamento = true;
      _DatiCambiati = false;
      RegistrazioneEffettuata = false;

      //interfaccia
      txtRagioneSociale.Focus();
      buttonComando.Visibility = System.Windows.Visibility.Hidden;


      _TeamLeadeOld = -1;
    }

    //----------------------------------------------------------------------------+
    //                             ConfiguraMaschera                              |
    //----------------------------------------------------------------------------+
    public void ConfiguraMaschera()
    {
      MasterFile mf = MasterFile.Create();
      //recupero dati e stato
      if (TipologiaAttivita != App.TipoAttivitaScheda.New)
      {
        Hashtable n = mf.GetAnagrafica(idRecord);
        //interfaccia
        if (App.ErrorLevel != App.ErrorTypes.Nessuno) return;
        //visualizzo bottone sblocca anagrafica
        btnSblocca_VisualizzaNascondi(mf.GetAnafraficaStato(idRecord));
        //
        txtRagioneSociale.Text = n["RagioneSociale"].ToString();
        txtPartitaIVA.Text = n["PartitaIVA"].ToString();
        txtCodiceFiscale.Text = n["CodiceFiscale"].ToString();
        switch ((App.TipoAnagraficaEsercizio)(Convert.ToInt32(n["Esercizio"].ToString())))
        {
          case App.TipoAnagraficaEsercizio.AnnoSolare:
            rdbEsercizioSolare.IsChecked = true;
            rdbEsercizioAcavallo.IsChecked = false;
            break;
          case App.TipoAnagraficaEsercizio.ACavallo:
            rdbEsercizioSolare.IsChecked = false;
            rdbEsercizioAcavallo.IsChecked = true;
            break;
          case App.TipoAnagraficaEsercizio.Sconosciuto:
          default:
            rdbEsercizioSolare.IsChecked = false;
            rdbEsercizioAcavallo.IsChecked = false;
            break;
        }
        //
        txtEsercizioDal.Text = n["EsercizioDal"].ToString();
        txtEsercizioAl.Text = n["EsercizioAl"].ToString();
        if (!n.Contains("OrganoDiControllo") || n["OrganoDiControllo"].ToString() == "" || Convert.ToInt32(n["OrganoDiControllo"].ToString()) == 1)
        {
          rdbOrganoControlloSindaco.IsChecked = false;
          rdbOrganoControlloCollegio.IsChecked = true;
          rdbOrganoControlloAssente.IsChecked = false;
        }
        else
        {
          if (!n.Contains("OrganoDiControllo") || n["OrganoDiControllo"].ToString() == "" || Convert.ToInt32(n["OrganoDiControllo"].ToString()) == 3)
          {
            rdbOrganoControlloSindaco.IsChecked = false;
            rdbOrganoControlloCollegio.IsChecked = false;
            rdbOrganoControlloAssente.IsChecked = true;
          }
          else
          {
            rdbOrganoControlloSindaco.IsChecked = true;
            rdbOrganoControlloCollegio.IsChecked = false;
            rdbOrganoControlloAssente.IsChecked = false;
          }
        }
        if (!n.Contains("OrganoDiRevisione") || n["OrganoDiRevisione"].ToString() == "" || Convert.ToInt32(n["OrganoDiRevisione"].ToString()) == 1)
        {
          rdbOrganoSocietaRevisione.IsChecked = false;
          rdbOrganoRevisioneAutonomo.IsChecked = false;
          rdbOrganoRevisioneControllo.IsChecked = true;
        }
        else if (Convert.ToInt32(n["OrganoDiRevisione"].ToString()) == 3)
        {
          rdbOrganoSocietaRevisione.IsChecked = true;
          rdbOrganoRevisioneAutonomo.IsChecked = false;
          rdbOrganoRevisioneControllo.IsChecked = false;
        }
        else
        {
          rdbOrganoSocietaRevisione.IsChecked = false;
          rdbOrganoRevisioneAutonomo.IsChecked = true;
          rdbOrganoRevisioneControllo.IsChecked = false;
        }
        if (n.Contains("Presidente"))
        {
          txtPresidente.Text = n["Presidente"].ToString();
        }
        if (n.Contains("MembroEffettivo"))
        {
          txtMembroEffettivo.Text = n["MembroEffettivo"].ToString();
        }
        if (n.Contains("MembroEffettivo2"))
        {
          txtMembroEffettivo2.Text = n["MembroEffettivo2"].ToString();
        }
        if (n.Contains("SindacoSupplente"))
        {
          txtSindacoSupplente.Text = n["SindacoSupplente"].ToString();
        }
        if (n.Contains("SindacoSupplente2"))
        {
          txtSindacoSupplente2.Text = n["SindacoSupplente2"].ToString();
        }
        if (n.Contains("RevisoreAutonomo"))
        {
          txtRevisoreAutonomo.Text = n["RevisoreAutonomo"].ToString();
        }
        txtNote.Text = n["Note"].ToString();
        _InCaricamento = false;
      }
      //inibisco tutti i controlli
      txtNote.IsReadOnly = true;
      txtRagioneSociale.IsReadOnly = true;
      txtCodiceFiscale.IsReadOnly = true;
      txtPartitaIVA.IsReadOnly = true;
      txtEsercizioDal.IsReadOnly = true;
      txtEsercizioAl.IsReadOnly = true;
      rdbEsercizioAcavallo.IsHitTestVisible = true;
      rdbEsercizioSolare.IsHitTestVisible = true;
      //nascondo testo help - non + usato
      textBlockDescrizione.Text = "";
      textBlockDescrizione.Visibility = System.Windows.Visibility.Collapsed;
      //interfaccia
      switch (TipologiaAttivita)
      {
        case App.TipoAttivitaScheda.New:
          labelTitolo.Content = "Nuova Anagrafica";
          buttonComando.Content = "Crea";
          buttonComando.Visibility = System.Windows.Visibility.Visible;
          //attivo controlli
          txtNote.IsReadOnly = false;
          txtRagioneSociale.IsReadOnly = false;
          txtCodiceFiscale.IsReadOnly = false;
          txtPartitaIVA.IsReadOnly = false;
          txtEsercizioDal.IsReadOnly = false;
          txtEsercizioAl.IsReadOnly = false;
          rdbEsercizioAcavallo.IsHitTestVisible = true;
          rdbEsercizioSolare.IsHitTestVisible = true;
          idRecord = App.MasterFile_NewID;
          break;
        case App.TipoAttivitaScheda.Edit:
          labelTitolo.Content = "Modifica Anagrafica";
          buttonComando.Content = "Salva";
          buttonComando.Visibility = System.Windows.Visibility.Visible;
          //attivo controlli
          txtNote.IsReadOnly = false;
          txtRagioneSociale.IsReadOnly = false;
          txtCodiceFiscale.IsReadOnly = false;
          txtPartitaIVA.IsReadOnly = false;
          txtEsercizioDal.IsReadOnly = false;
          txtEsercizioAl.IsReadOnly = false;
          rdbEsercizioAcavallo.IsHitTestVisible = true;
          rdbEsercizioSolare.IsHitTestVisible = true;
          //configurazione stato
          mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.InUso);
          break;
        case App.TipoAttivitaScheda.Delete:
          labelTitolo.Content = "Elimina Anagrafica";
          buttonComando.Content = "Elimina";
          buttonComando.Visibility = System.Windows.Visibility.Visible;
          //configurazione stato
          mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.InUso);
          break;
        case App.TipoAttivitaScheda.Export:
          labelTitolo.Content = "Esporta Anagrafica e documenti collegati";
          buttonComando.Content = "Esporta";
          buttonComando.Visibility = System.Windows.Visibility.Visible;
          //configurazione stato
          mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.InUso);
          break;
        case App.TipoAttivitaScheda.Condividi:
          labelTitolo.Content = "Condividi Anagrafica e documenti collegati";
          buttonComando.Content = "Condividi";
          buttonComando.Visibility = System.Windows.Visibility.Visible;
          break;
        case App.TipoAttivitaScheda.View:
        default:
          labelTitolo.Content = "Apri Anagrafica in sola lettura";
          break;
      }

      //Team
      ConfiguraMascheraTeam();

    }

    //----------------------------------------------------------------------------+
    //                       btnSblocca_VisualizzaNascondi                        |
    //----------------------------------------------------------------------------+
    private void btnSblocca_VisualizzaNascondi(App.TipoAnagraficaStato statoAnagrafica)
    {
      if (statoAnagrafica == App.TipoAnagraficaStato.Bloccato || statoAnagrafica == App.TipoAnagraficaStato.Esportato || statoAnagrafica == App.TipoAnagraficaStato.InUso || statoAnagrafica == App.TipoAnagraficaStato.Sconosciuto)
      {

      }
    }

    //----------------------------------------------------------------------------+
    //                            buttonComando_Click                             |
    //----------------------------------------------------------------------------+
    private void buttonComando_Click(object sender, RoutedEventArgs e)
    {
      MasterFile mf = MasterFile.Create();
      Utilities u = new Utilities();

      switch (TipologiaAttivita)
      {
        //Nuovo e salva
        case App.TipoAttivitaScheda.New:
        case App.TipoAttivitaScheda.Edit:
          //Campi Obbligatorio
          if (!u.ConvalidaDatiInterfaccia(txtRagioneSociale, "Ragione Sociale mancante."))
            return;
          if (!u.ConvalidaDatiInterfaccia(txtCodiceFiscale, "Codice Fiscale mancante."))
            return;
          if (!u.ConvalidaDatiInterfaccia(rdbEsercizioAcavallo, "Selezionare tipologia Esercizio."))
            return;
          if (!u.ConvalidaDatiInterfaccia(txtEsercizioDal, "Inizio Esercizio mancante.", "Formato GG/MM"))
            return;
          if (!u.ConvalidaDatiInterfaccia(txtEsercizioAl, "Fine Esercizio mancante.", "Formato GG/MM"))
            return;
          if (rdbEsercizioAcavallo.IsChecked == true)
          {
            try
            {
              //calcola la durata ipotetica del periodo in esame
              TimeSpan ts = Convert.ToDateTime(txtEsercizioAl.Text.Trim() + "/2013").Subtract(Convert.ToDateTime(txtEsercizioDal.Text.Trim() + "/2012"));
              if (ts.Days != 364)
              {
                MessageBox.Show("Attenzione, periodo a cavallo inferiore ai 365 giorni");
              }
            }
            catch (Exception ex)
            {
              cBusinessObjects.logger.Error(ex, "wSchedaAnagrafica.buttonComando_Click1 exception");
              string log = ex.Message;
            }
          }
          //setto dati
          Hashtable ht = new Hashtable();
          ht.Add("RagioneSociale", txtRagioneSociale.Text.Trim());
          ht.Add("CodiceFiscale", txtCodiceFiscale.Text.Trim());
          ht.Add("PartitaIVA", txtPartitaIVA.Text.Trim());
          ht.Add("Note", txtNote.Text.Trim());
          ht.Add("EsercizioDal", txtEsercizioDal.Text.Trim());
          ht.Add("EsercizioAl", txtEsercizioAl.Text.Trim());
          if (rdbEsercizioSolare.IsChecked == false && rdbEsercizioAcavallo.IsChecked == false)
          {
            ht.Add("Esercizio", (int)(App.TipoAnagraficaEsercizio.Sconosciuto));
          }
          else
          {
            if (rdbEsercizioSolare.IsChecked == true)
            {
              ht.Add("Esercizio", (int)(App.TipoAnagraficaEsercizio.AnnoSolare));
            }
            if (rdbEsercizioAcavallo.IsChecked == true)
            {
              ht.Add("Esercizio", (int)(App.TipoAnagraficaEsercizio.ACavallo));
            }
          }
          if (rdbOrganoControlloSindaco.IsChecked == true)
          {
            ht.Add("OrganoDiControllo", 2);
          }
          else if (rdbOrganoControlloCollegio.IsChecked == true)
          {
            ht.Add("OrganoDiControllo", 1);
          }
          else
          {
            ht.Add("OrganoDiControllo", 3);
          }
          if (rdbOrganoRevisioneAutonomo.IsChecked == true)
          {
            ht.Add("OrganoDiRevisione", 2);
          }
          else if (rdbOrganoSocietaRevisione.IsChecked == true)
          {
            ht.Add("OrganoDiRevisione", 3);
          }
          else
          {
            ht.Add("OrganoDiRevisione", 1);
          }
          ht.Add("Presidente", txtPresidente.Text.Trim());
          ht.Add("MembroEffettivo", txtMembroEffettivo.Text.Trim());
          ht.Add("MembroEffettivo2", txtMembroEffettivo2.Text.Trim());
          ht.Add("SindacoSupplente", txtSindacoSupplente.Text.Trim());
          ht.Add("SindacoSupplente2", txtSindacoSupplente2.Text.Trim());
          ht.Add("RevisoreAutonomo", txtRevisoreAutonomo.Text.Trim());
          if (mf.ClienteGiaPresente(ht, idRecord))
          {
            MessageBox.Show("Attenzione, Partita IVA o Codice Fiscale già presente per altro Cliente");
            return;
          }
          idRecord = mf.SetAnagrafica(ht, idRecord);

          // TEAM
          SalvaTeam();

          RegistrazioneEffettuata = true;
          _DatiCambiati = false;

          ((MainWindow)(Owner)).CaricaClienti();
          break;
        case App.TipoAttivitaScheda.Delete:
          //richiesta conferma
          if (MessageBoxResult.No == u.ConfermaCancellazione())
            return;
          //cancellazione
          mf.DeleteAnagrafica(idRecord);

          // TO DO x TEAM cancellare eventuale associazione con esecutori 

          RegistrazioneEffettuata = true;
          break;
        case App.TipoAttivitaScheda.Condividi:
          if (MessageBoxResult.No == u.ConfermaCondivisione())
            return;
          //esportazione su file
          char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
          string RagioneSociale = new string
            (
              ("Condivisione di " + txtRagioneSociale.Text)
              .Where(x => !invalidChars.Contains(x))
              .ToArray()
            );
          //nome file di esportazione
          string nomeFile = RagioneSociale + u.EstensioneFile(App.TipoFile.ImportExport);
          string ret = u.sys_SaveFileDialog(nomeFile, App.TipoFile.ImportExport);
          if (ret != null)
          {

            //ANDREA 2.8
            //backup file di importazione
            string retBis = App.AppTempFolder + "{" + Guid.NewGuid().ToString() + "}";
            cImportExport.ExportNoVerbose(retBis, idRecord, true);
            //esportazione
            //cImportExport.Export(ret, idRecord, true);


            //interfaccia
            MessageBox.Show("Condivisione avvenuta con successo");
            RegistrazioneEffettuata = true;
          }
          break;
        case App.TipoAttivitaScheda.Export:
          if (MessageBoxResult.No == u.ConfermaEsportazione())
            return;
          //esportazione via lan
          //if (App.CodiceMacchinaServer.Trim().Split('-')[0] != "" && App.CodiceMacchina.Split('-')[0] != App.CodiceMacchinaServer.Split('-')[0])
          //if (App.TipoLicenza == App.TipologieLicenze.ClientLan || App.TipoLicenza == App.TipologieLicenze.ClientLanMulti)
          //4.5.1
          if (App.AppConsentiImportazioneEsportazioneLan)
          {
            try
            {

              //backup file di importazione
              ret = App.AppTempFolder + "{" + Guid.NewGuid().ToString() + "}";
              bool returnvalue = cImportExport.ExportNoVerbose(ret, idRecord, false);
              if (returnvalue == false)
              {

                MessageBox.Show("Attenzione, rete discontinua, troppo lenta o diritti insufficienti sul server. Procedimento non portato a termine per evitare danni sui dati.");
                return;
              }
              //configurazione stato origine (pre 2.8)
              //mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.Esportato);
              //cancellazione richiesta del cliente sul clien (dalla 2.8)
              //imposto percorso archivi di destinazione
              if (App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Remoto)
              {
                //Imposto percorsi su archivio locale
                App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.LocaleImportExport;
                //salvo nuova configurazione
                GestioneLicenza l = new GestioneLicenza();
                l.SalvaInfoDataUltimoUtilizzo();
                //Setup path
                u.ConfiguraPercorsi();
              }
              else
              {
                //cancello in locale l'utente
                returnvalue = mf.DeleteAnagrafica(idRecord);
                if (returnvalue == false)
                {
                  //importo file in locale
                  cImportExport.Import(ret, false);

                  MessageBox.Show("Attenzione, rete discontinua, troppo lenta o diritti insufficienti sul server. Procedimento non portato a termine.");
                  return;
                }
                //Imposto percorsi su archivio remoto
                App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Remoto;
                //salvo nuova configurazione
                GestioneLicenza l = new GestioneLicenza();
                l.SalvaInfoDataUltimoUtilizzo();
                //setup path
                u.ConfiguraPercorsi();
              }
              //importo file in locale
              returnvalue = cImportExport.Import(ret, false);
              if (returnvalue == false)
              {
                //switcho nuovamente per rollback
                if (App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Remoto)
                {
                  App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.LocaleImportExport;
                  //salvo nuova configurazione
                  GestioneLicenza l = new GestioneLicenza();
                  l.SalvaInfoDataUltimoUtilizzo();
                  //setup path
                  u.ConfiguraPercorsi();
                }
                else
                {
                  //Imposto percorsi su archivio remoto
                  App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Remoto;
                  //salvo nuova configurazione
                  GestioneLicenza l = new GestioneLicenza();
                  l.SalvaInfoDataUltimoUtilizzo();
                  //setup path
                  u.ConfiguraPercorsi();
                }
                cImportExport.Import(ret, false);

                MessageBox.Show("Attenzione, rete discontinua, troppo lenta o diritti insufficienti sul server. Procedimento non portato a termine.");
                return;
              }

              RegistrazioneEffettuata = true;
              mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.Esportato);
              MasterFile.ForceRecreate();
              ////andrea 3.4 ********************************************************** ACCANTONATO ????
              ////verifico disponibilità del cliente su archivio remoto
              //int idDest = mf.ClienteGiaPresente(mf.GetAnagrafica(idRecord));
              //if (mf.GetAnafraficaStato(idDest) == App.TipoAnagraficaStato.Esportato || mf.GetAnafraficaStato(idDest) == App.TipoAnagraficaStato.Disponibile)
              //{
              //    //importo file in locale / esporto in remoto
              //    cImportExport.Import(ret, false);
              //    RegistrazioneEffettuata = true;
              //}
              //else
              //{
              //    RegistrazioneEffettuata = false;
              //}
              ////Process wait - STOP
              //pw.Close();
              ////andrea 3.4
              //if (!RegistrazioneEffettuata)
              //{
              //    MessageBox.Show("Attenzione, anagrafica in uso, impossibile esportare.\n\n");
              //}
            }
            catch (Exception ex)
            {
              cBusinessObjects.logger.Error(ex, "wSchedaAnagrafica.buttonComando_Click2 exception");

              string log = ex.Message;
              MessageBox.Show("Attenzione, rete discontinua, troppo lenta o diritti insufficienti sul server. Procedimento non portato a termine.");
              return;
            }
          }
          else
          {
            //esportazione su file
            invalidChars = System.IO.Path.GetInvalidFileNameChars();
            RagioneSociale = new string
              (
                txtRagioneSociale.Text
                .Where(x => !invalidChars.Contains(x))
                .ToArray()
              );
            //nome file di esportazione
            nomeFile = RagioneSociale + u.EstensioneFile(App.TipoFile.ImportExport);
            ret = u.sys_SaveFileDialog(nomeFile, App.TipoFile.ImportExport);
            if (ret != null)
            {

              //ANDREA 2.8
              //backup file di importazione
              string retBis = App.AppTempFolder + "{" + Guid.NewGuid().ToString() + "}";
              bool returnvalue = cImportExport.ExportNoVerbose(retBis, idRecord, false);
              if (returnvalue == false)
              {

                MessageBox.Show("Attenzione, Procedimento non portato a termine per evitare danni sui dati. Ritentare controllando che vi sia spazio sufficiente nella directory.");
                return;
              }
              //esportazione
              returnvalue = cImportExport.Export(ret, idRecord, false);
              if (returnvalue == false)
              {

                MessageBox.Show("Attenzione, Procedimento non portato a termine per evitare danni sui dati. Ritentare controllando che vi sia spazio sufficiente nella directory.");
                return;
              }

              //elimino anagrafica
              //if (App.TipoLicenza == App.TipologieLicenze.Viewer)
              //{
              //    //cancello in locale l'utente
              //    returnvalue = mf.DeleteAnagrafica(idRecord);
              //    if (returnvalue == false)
              //    {
              //        //importo file in locale
              //        cImportExport.Import(ret, false);
              //        //Process wait - STOP
              //        pw.Close();
              //        MessageBox.Show("Attenzione, rete discontinua, troppo lenta o diritti insufficienti sul server. Procedimento non portato a termine.");
              //        return;
              //    }
              //}
              //else
              //{
              //    //configurazione stato
              //    mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.Esportato);
              //}
              if (App.CodiceMacchina != App.CodiceMacchinaServer && App.NumeroanAgrafiche == 1)
              {
                mf.DeleteAnagrafica(idRecord);
              }
              else
              {
                //configurazione stato
                mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.Esportato);
              }
              //interfaccia
              MessageBox.Show("Esportazione avvenuta con successo");
              mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.Esportato);
              RegistrazioneEffettuata = true;
            }
          }
          break;
      }
      //chiudo maschera
      base.Close();
    }

    //----------------------------------------------------------------------------+
    //                         GestoreEvento_DatiCambiati                         |
    //----------------------------------------------------------------------------+
    private void GestoreEvento_DatiCambiati(object sender, RoutedEventArgs e)
    {
      //configura esercizio solare
      if (sender.GetType().Name == "RadioButton")
      {
        if (((RadioButton)sender).Name == "rdbEsercizioSolare")
        {
          txtEsercizioDal.Text = "01/01";
          txtEsercizioAl.Text = "31/12";
        }
        if (((RadioButton)sender).Name == "rdbOrganoRevisioneControllo")
        {
          lblRevisoreAutonomo.Visibility = System.Windows.Visibility.Collapsed;
          txtRevisoreAutonomo.Visibility = System.Windows.Visibility.Collapsed;
          txtRevisoreAutonomo.Text = "";
          txtRevisoreAutonomo.IsEnabled = false;
        }
        if (((RadioButton)sender).Name == "rdbOrganoRevisioneAutonomo" || ((RadioButton)sender).Name == "rdbOrganoSocietaRevisione")
        {
          lblRevisoreAutonomo.Visibility = System.Windows.Visibility.Visible;
          if (((RadioButton)sender).Name == "rdbOrganoRevisioneAutonomo")
          {
            lblRevisoreAutonomo.Text = "Revisore";
          }
          else
          {
            lblRevisoreAutonomo.Text = "Società di Revisione";
          }
          txtRevisoreAutonomo.Visibility = System.Windows.Visibility.Visible;
          txtRevisoreAutonomo.Text = "";
          txtRevisoreAutonomo.IsEnabled = true;
        }
        if (((RadioButton)sender).Name == "rdbOrganoControlloCollegio")
        {
          lblPresidenteSindacoUnico.Visibility = System.Windows.Visibility.Visible;
          lblPresidenteSindacoUnico.Text = "Presidente";
          txtPresidente.Visibility = System.Windows.Visibility.Visible;
          txtPresidente.IsEnabled = true;
          lblMembro.Visibility = System.Windows.Visibility.Visible;
          txtMembroEffettivo.Visibility = System.Windows.Visibility.Visible;
          txtMembroEffettivo.IsEnabled = true;
          lblMembro2.Visibility = System.Windows.Visibility.Visible;
          txtMembroEffettivo2.Visibility = System.Windows.Visibility.Visible;
          txtMembroEffettivo2.IsEnabled = true;
          lblSindacoSupplente.Visibility = System.Windows.Visibility.Visible;
          txtSindacoSupplente.Visibility = System.Windows.Visibility.Visible;
          txtSindacoSupplente.IsEnabled = true;
          lblSindacoSupplente2.Visibility = System.Windows.Visibility.Visible;
          txtSindacoSupplente2.Visibility = System.Windows.Visibility.Visible;
          txtSindacoSupplente2.IsEnabled = true;
        }
        if (((RadioButton)sender).Name == "rdbOrganoControlloSindaco")
        {
          lblPresidenteSindacoUnico.Visibility = System.Windows.Visibility.Visible;
          lblPresidenteSindacoUnico.Text = "Sindaco unico";
          txtPresidente.Visibility = System.Windows.Visibility.Visible;
          txtPresidente.IsEnabled = true;
          lblMembro.Visibility = System.Windows.Visibility.Collapsed;
          txtMembroEffettivo.Visibility = System.Windows.Visibility.Collapsed;
          txtMembroEffettivo.IsEnabled = false;
          lblMembro2.Visibility = System.Windows.Visibility.Collapsed;
          txtMembroEffettivo2.Visibility = System.Windows.Visibility.Collapsed;
          txtMembroEffettivo2.IsEnabled = false;
          lblSindacoSupplente.Visibility = System.Windows.Visibility.Collapsed;
          txtSindacoSupplente.Visibility = System.Windows.Visibility.Collapsed;
          txtSindacoSupplente.IsEnabled = false;
          lblSindacoSupplente2.Visibility = System.Windows.Visibility.Collapsed;
          txtSindacoSupplente2.Visibility = System.Windows.Visibility.Collapsed;
          txtSindacoSupplente2.IsEnabled = false;
        }
        if (((RadioButton)sender).Name == "rdbOrganoControlloAssente")
        {
          lblPresidenteSindacoUnico.Visibility = System.Windows.Visibility.Collapsed;
          lblPresidenteSindacoUnico.Text = "";
          txtPresidente.Visibility = System.Windows.Visibility.Collapsed;
          txtPresidente.IsEnabled = true;
          lblMembro.Visibility = System.Windows.Visibility.Collapsed;
          txtMembroEffettivo.Visibility = System.Windows.Visibility.Collapsed;
          txtMembroEffettivo.IsEnabled = true;
          lblMembro2.Visibility = System.Windows.Visibility.Collapsed;
          txtMembroEffettivo2.Visibility = System.Windows.Visibility.Collapsed;
          txtMembroEffettivo2.IsEnabled = true;
          lblSindacoSupplente.Visibility = System.Windows.Visibility.Collapsed;
          txtSindacoSupplente.Visibility = System.Windows.Visibility.Collapsed;
          txtSindacoSupplente.IsEnabled = false;
          lblSindacoSupplente2.Visibility = System.Windows.Visibility.Collapsed;
          txtSindacoSupplente2.Visibility = System.Windows.Visibility.Collapsed;
          txtSindacoSupplente2.IsEnabled = false;
        }
      }
      if (_InCaricamento) return;
      _DatiCambiati = true;
    }

    //----------------------------------------------------------------------------+
    //                       GestoreEvento_ChiusuraFinestra                       |
    //----------------------------------------------------------------------------+
    private void GestoreEvento_ChiusuraFinestra(object sender, CancelEventArgs e)
    {
      if (annulla) return;
      //Configuro stato
      if (TipologiaAttivita == App.TipoAttivitaScheda.Edit || (TipologiaAttivita == App.TipoAttivitaScheda.Export && !RegistrazioneEffettuata) || (TipologiaAttivita == App.TipoAttivitaScheda.Delete && !RegistrazioneEffettuata))
      {
        MasterFile mf = MasterFile.Create();
        mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.Disponibile);
      }
      //dati non modificati
      if (!_DatiCambiati) return;
      //dati modificati
      Utilities u = new Utilities();
      if (MessageBoxResult.No == u.AvvisoPerditaDati())
        e.Cancel = true;
    }

    //----------------------------------------------------------------------------+
    //                    GestoreEvento_ComboEsercizio_Checked                    |
    //----------------------------------------------------------------------------+
    private void GestoreEvento_ComboEsercizio_Checked(object sender, CancelEventArgs e)
    {
      _DatiCambiati = true;
    }

    //----------------------------------------------------------------------------+
    //                            buttonSblocca_Click                             |
    //----------------------------------------------------------------------------+
    private void buttonSblocca_Click(object sender, RoutedEventArgs e)
    {
      //richiesta conferma
      Utilities u = new Utilities();
      if (MessageBoxResult.Yes == u.ConfermaSbloccoUtente())
      {
        MasterFile mf = MasterFile.Create();
        if (mf.GetAnafraficaStato(idRecord) != App.TipoAnagraficaStato.Esportato)
        {
          mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.Disponibile);
          ((RevisoftApplication.MainWindow)(this.Owner)).CaricaClienti();
          MessageBox.Show("Sblocco Cliente Avvenuto con successo");
        }
        else
        {
          if (MessageBox.Show("Confermare sblocco del cliente attualmente esportato su altro PC? (ATTENZIONE operazione irreversibile.)", "Attenzione", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
          {
            mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.Disponibile);
            ((RevisoftApplication.MainWindow)(this.Owner)).CaricaClienti();
            MessageBox.Show("Sblocco Cliente Avvenuto con successo");
          }
        }
      }
    }

    //----------------------------------------------------------------------------+
    //                             buttonChiudi_Click                             |
    //----------------------------------------------------------------------------+
    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

    //----------------------------------------------------------------------------+
    //                            buttonAnnulla_Click                             |
    //----------------------------------------------------------------------------+
    private void buttonAnnulla_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

    //----------------------------------------------------------------------------+
    //                                Button_Click                                |
    //----------------------------------------------------------------------------+
    private void Button_Click(object sender, RoutedEventArgs e)
    {
      MasterFile mf = MasterFile.Create();
      Utilities u = new Utilities();
      wSchedaCambioEsercizio ece = new wSchedaCambioEsercizio();
      ece.Owner = this;
      ece.ShowDialog();
      //Campi Obbligatorio
      if (!u.ConvalidaDatiInterfaccia(txtRagioneSociale, "Ragione Sociale mancante."))
        return;
      if (!u.ConvalidaDatiInterfaccia(txtCodiceFiscale, "Codice Fiscale mancante."))
        return;
      if (!u.ConvalidaDatiInterfaccia(rdbEsercizioAcavallo, "Selezionare tipologia Esercizio."))
        return;
      if (!u.ConvalidaDatiInterfaccia(txtEsercizioDal, "Inizio Esercizio mancante.", "Formato GG/MM"))
        return;
      if (!u.ConvalidaDatiInterfaccia(txtEsercizioAl, "Fine Esercizio mancante.", "Formato GG/MM"))
        return;
      if (rdbEsercizioAcavallo.IsChecked == true)
      {
        try
        {
          //calcola la durata ipotetica del periodo in esame
          TimeSpan ts = Convert.ToDateTime(txtEsercizioAl.Text.Trim() + "/2013").Subtract(Convert.ToDateTime(txtEsercizioDal.Text.Trim() + "/2012"));
          if (ts.Days != 364)
          {
            MessageBox.Show("Attenzione, periodo a cavallo inferiore ai 365 giorni");
          }
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wSchedaAnagrafica.Button_Click exception");
          string log = ex.Message;
        }
      }
      //setto dati
      Hashtable ht = new Hashtable();
      ht.Add("RagioneSociale", txtRagioneSociale.Text.Trim());
      ht.Add("CodiceFiscale", txtCodiceFiscale.Text.Trim());
      ht.Add("PartitaIVA", txtPartitaIVA.Text.Trim());
      ht.Add("Note", txtNote.Text.Trim());
      ht.Add("EsercizioDal", txtEsercizioDal.Text.Trim());
      ht.Add("EsercizioAl", txtEsercizioAl.Text.Trim());
      if (rdbEsercizioSolare.IsChecked == false && rdbEsercizioAcavallo.IsChecked == false)
      {
        ht.Add("Esercizio", (int)(App.TipoAnagraficaEsercizio.Sconosciuto));
      }
      else
      {
        if (rdbEsercizioSolare.IsChecked == true)
        {
          ht.Add("Esercizio", (int)(App.TipoAnagraficaEsercizio.AnnoSolare));
        }
        if (rdbEsercizioAcavallo.IsChecked == true)
        {
          ht.Add("Esercizio", (int)(App.TipoAnagraficaEsercizio.ACavallo));
        }
      }
      if (rdbOrganoControlloSindaco.IsChecked == true)
      {
        ht.Add("OrganoDiControllo", 2);
      }
      else if (rdbOrganoControlloCollegio.IsChecked == true)
      {
        ht.Add("OrganoDiControllo", 1);
      }
      else
      {
        ht.Add("OrganoDiControllo", 3);
      }
      if (rdbOrganoRevisioneAutonomo.IsChecked == true)
      {
        ht.Add("OrganoDiRevisione", 2);
      }
      else if (rdbOrganoSocietaRevisione.IsChecked == true)
      {
        ht.Add("OrganoDiRevisione", 3);
      }
      else
      {
        ht.Add("OrganoDiRevisione", 1);
      }
      ht.Add("Presidente", txtPresidente.Text.Trim());
      if (rdbOrganoControlloSindaco.IsChecked == true)
      {
        ht.Add("MembroEffettivo", "");
        ht.Add("MembroEffettivo2", "");
        ht.Add("SindacoSupplente", "");
        ht.Add("SindacoSupplente2", "");
      }
      else
      {
        ht.Add("MembroEffettivo", txtMembroEffettivo.Text.Trim());
        ht.Add("MembroEffettivo2", txtMembroEffettivo2.Text.Trim());
        ht.Add("SindacoSupplente", txtSindacoSupplente.Text.Trim());
        ht.Add("SindacoSupplente2", txtSindacoSupplente2.Text.Trim());
      }
      ht.Add("RevisoreAutonomo", txtRevisoreAutonomo.Text.Trim());
      if (mf.ClienteGiaPresente(ht, idRecord))
      {
        MessageBox.Show("Attenzione, Partita IVA o Codice Fiscale già presente per altro Cliente");
        return;
      }
      idRecord = mf.SetAnagrafica(ht, idRecord);
    }


    //----------------------------------------------------------------------------+
    //                                Team                                        |
    //----------------------------------------------------------------------------+

    private void CaricaDatiTeamAdministrator()
    {
      try
      {
        _teamList = cUtenti.GetUtentiTeamLeader();

        int i = 0;
        if (_teamList != null)
        {
          for (i = 0; i < _teamList.Count; i++)
          {
            cmbTeamLeader.Items.Add(_teamList[i].Login);
          }
        }

        //cmbTeamLeader.Items.Add("nessuno");
        if (TipologiaAttivita == App.TipoAttivitaScheda.New)
          cmbTeamLeader.SelectedIndex = -1;
        else
        {
          _TeamLeadeOld = cUtenti.GetLaderIdAssociatoAlCliente(idRecord.ToString());
          if (_TeamLeadeOld >= 0)
          {
            for (int j = 0; j < _teamList.Count; j++)
            {
              if (_teamList[j].Id == _TeamLeadeOld)
              {
                cmbTeamLeader.SelectedIndex = j;
                break;
              }
            }
          }
        }
        if (TipologiaAttivita != App.TipoAttivitaScheda.New && App.TipoAttivitaScheda.New != App.TipoAttivitaScheda.Edit)
          cmbTeamLeader.IsEnabled = false;
        else
          cmbTeamLeader.IsEnabled = true;
        //switch (TipologiaAttivita)
        //{
        //	case App.TipoAttivitaScheda.New:
        //		cmbTeamLeader.SelectedIndex = -1;
        //		break;
        //	case App.TipoAttivitaScheda.Edit:
        //		// modifica	-- se esiste un team associato al cliente si posiziona
        //		 _TeamLeadeOld = cUtenti.GetLaderIdAssociatoAlCliente(idRecord.ToString());
        //		if (_TeamLeadeOld >= 0)
        //		{
        //			for (int j = 0; j < _teamList.Count; j++)
        //			{
        //				if (_teamList[j].Id == _TeamLeadeOld)
        //				{
        //					cmbTeamLeader.SelectedIndex = j;	 
        //					break;
        //				}
        //			}
        //		}
        //		break;
        //	case App.TipoAttivitaScheda.Delete:
        //		break;
        //	case App.TipoAttivitaScheda.Condividi:
        //	case App.TipoAttivitaScheda.Export:
        //	case App.TipoAttivitaScheda.View:
        //		break;
        //}	
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaAnagrafica.CaricaDatiTeamAdministrator exception");
        App.GestioneLog(ex.Message);
      }
    }

    private void ConfiguraMascheraTeam()
    {

      //Tean	 
      if (App.AppTipo == App.ModalitaApp.Administrator)
        CaricaDatiTeamAdministrator();

      // configurazione Tab di gestione Team
      if (App.AppTipo == App.ModalitaApp.StandAlone)
      {
        tabIteTeam.Visibility = Visibility.Collapsed;
        return;
      }

      if (App.AppTipo == App.ModalitaApp.Administrator)
      {
        tabIteTeam.Visibility = Visibility.Visible;
      }
      else
        tabIteTeam.Visibility = Visibility.Collapsed;

      switch (TipologiaAttivita)
      {
        case App.TipoAttivitaScheda.New:
        case App.TipoAttivitaScheda.Edit:
        case App.TipoAttivitaScheda.Delete:
          //if (App.AppTipo == App.ModalitaApp.Administrator)
          //{
          //	tabIteTeam.Visibility = Visibility.Visible;
          //}
          //else
          //	tabIteTeam.Visibility = Visibility.Collapsed;
          break;
        case App.TipoAttivitaScheda.Export:
        case App.TipoAttivitaScheda.Condividi:
        case App.TipoAttivitaScheda.View:

          break;
      }
    }

    private void SalvaTeam()
    {
      switch (App.AppTipo)
      {
        case App.ModalitaApp.StandAlone:
          return;
        case App.ModalitaApp.Administrator:

          // si salva l'associaione tra cliente e team leader
          if (cmbTeamLeader.SelectedIndex != -1)
          {
            int teamLeaderId = _teamList[cmbTeamLeader.SelectedIndex].Id;
            if (_TeamLeadeOld != teamLeaderId)
            {
              if (_TeamLeadeOld > 0)
              {
                cUtenti.UpsertClientiPerUtente(_TeamLeadeOld, "", idRecord.ToString());
              }
              cUtenti.UpsertClientiPerUtente(teamLeaderId, idRecord.ToString(), "");
            }
          }

          break;
        case App.ModalitaApp.Team:
          // si salvano le modifiche apportate dal team leader
          break;
      }

    }

  } //------------------------- public partial class wSchedaAnafrafica : Window
} //--------------------------------------------- namespace RevisoftApplication

/*
// srcOld
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using System.ComponentModel;
using System.Collections;


namespace RevisoftApplication
{

    public partial class wSchedaAnafrafica : Window
    {
        public App.TipoAttivitaScheda TipologiaAttivita;
        public int idRecord = 0;
        private bool _InCaricamento;
        private bool _DatiCambiati;
        public bool RegistrazioneEffettuata;

        private bool annulla = false;


        public wSchedaAnafrafica()
        {
            InitializeComponent();

            //var
            idRecord = App.MasterFile_NewID;
            _InCaricamento = true;
            _DatiCambiati = false;
            RegistrazioneEffettuata = false;

            //interfaccia 
            txtRagioneSociale.Focus();
            buttonComando.Visibility = System.Windows.Visibility.Hidden;
            buttonSblocca.Visibility = System.Windows.Visibility.Hidden;
        }

        public void ConfiguraMaschera()
        {
            MasterFile mf = MasterFile.Create();

            //recupero dati e stato
            if (TipologiaAttivita != App.TipoAttivitaScheda.New)
            {
                Hashtable n = mf.GetAnagrafica(idRecord);


                //interfaccia
                if (App.ErrorLevel != App.ErrorTypes.Nessuno)
                {
					        return;
				        }

                //visualizzo bottone sblocca anagrafica
                btnSblocca_VisualizzaNascondi(mf.GetAnafraficaStato(idRecord));
                //
                txtRagioneSociale.Text = n["RagioneSociale"].ToString();
                txtPartitaIVA.Text = n["PartitaIVA"].ToString();
                txtCodiceFiscale.Text = n["CodiceFiscale"].ToString();
                switch ((App.TipoAnagraficaEsercizio)(Convert.ToInt32(n["Esercizio"].ToString())))
                {
                    case App.TipoAnagraficaEsercizio.AnnoSolare:
                        rdbEsercizioSolare.IsChecked = true;
                        rdbEsercizioAcavallo.IsChecked = false;
                        break;
                    case App.TipoAnagraficaEsercizio.ACavallo:
                        rdbEsercizioSolare.IsChecked = false;
                        rdbEsercizioAcavallo.IsChecked = true;
                        break;
                    case App.TipoAnagraficaEsercizio.Sconosciuto:
                    default:
                        rdbEsercizioSolare.IsChecked = false;
                        rdbEsercizioAcavallo.IsChecked = false;
                        break;
                }
                     
                //
                txtEsercizioDal.Text = n["EsercizioDal"].ToString();
                txtEsercizioAl.Text = n["EsercizioAl"].ToString();


                if ( !n.Contains( "OrganoDiControllo" ) || n["OrganoDiControllo"].ToString() == "" || Convert.ToInt32( n["OrganoDiControllo"].ToString() ) == 1 )
                {
                    rdbOrganoControlloSindaco.IsChecked = false;
                    rdbOrganoControlloCollegio.IsChecked = true;
                    rdbOrganoControlloAssente.IsChecked = false;
                }
                else
                {
                    if (!n.Contains("OrganoDiControllo") || n["OrganoDiControllo"].ToString() == "" || Convert.ToInt32(n["OrganoDiControllo"].ToString()) == 3)
                    {
                        rdbOrganoControlloSindaco.IsChecked = false;
                        rdbOrganoControlloCollegio.IsChecked = false;
                        rdbOrganoControlloAssente.IsChecked = true;
                    }
                    else
                    {
                        rdbOrganoControlloSindaco.IsChecked = true;
                        rdbOrganoControlloCollegio.IsChecked = false;
                        rdbOrganoControlloAssente.IsChecked = false;
                    }
                }

                if ( !n.Contains( "OrganoDiRevisione" ) || n["OrganoDiRevisione"].ToString() == "" || Convert.ToInt32( n["OrganoDiRevisione"].ToString() ) == 1 )
                {
                    rdbOrganoSocietaRevisione.IsChecked = false;
                    rdbOrganoRevisioneAutonomo.IsChecked = false;
                    rdbOrganoRevisioneControllo.IsChecked = true;
                }
                else if ( Convert.ToInt32( n["OrganoDiRevisione"].ToString() ) == 3 )
                {
                    rdbOrganoSocietaRevisione.IsChecked = true;
                    rdbOrganoRevisioneAutonomo.IsChecked = false;
                    rdbOrganoRevisioneControllo.IsChecked = false;
                }
                else
                {
                    rdbOrganoSocietaRevisione.IsChecked = false;
                    rdbOrganoRevisioneAutonomo.IsChecked = true;
                    rdbOrganoRevisioneControllo.IsChecked = false;
                }

                if ( n.Contains( "Presidente" ) )
                {
                    txtPresidente.Text = n["Presidente"].ToString();
                }

                if ( n.Contains( "MembroEffettivo" ) )
                {
                    txtMembroEffettivo.Text = n["MembroEffettivo"].ToString();
                }

                if ( n.Contains( "MembroEffettivo2" ) )
                {
                    txtMembroEffettivo2.Text = n["MembroEffettivo2"].ToString();
                }

                if (n.Contains("SindacoSupplente"))
                {
                    txtSindacoSupplente.Text = n["SindacoSupplente"].ToString();
                }

                if (n.Contains("SindacoSupplente2"))
                {
                    txtSindacoSupplente2.Text = n["SindacoSupplente2"].ToString();
                }

                if ( n.Contains( "RevisoreAutonomo" ) )
                {
                    txtRevisoreAutonomo.Text = n["RevisoreAutonomo"].ToString();
                }

                txtNote.Text = n["Note"].ToString();
                _InCaricamento = false;
            }

            //inibisco tutti i controlli
            txtNote.IsReadOnly = true;
            txtRagioneSociale.IsReadOnly = true;
            txtCodiceFiscale.IsReadOnly = true;
            txtPartitaIVA.IsReadOnly = true;
            txtEsercizioDal.IsReadOnly = true;
            txtEsercizioAl.IsReadOnly = true;
            rdbEsercizioAcavallo.IsHitTestVisible = true;
            rdbEsercizioSolare.IsHitTestVisible = true;

            //nascondo testo help - non + usato
            textBlockDescrizione.Text = "";
            textBlockDescrizione.Visibility = System.Windows.Visibility.Collapsed;


            //interfaccia
            switch (TipologiaAttivita)
            {
                case App.TipoAttivitaScheda.New:
                    labelTitolo.Content = "Nuova Anagrafica";
                    buttonComando.Content = "Crea";
                    buttonComando.Visibility = System.Windows.Visibility.Visible;
                    //attivo controlli
                    txtNote.IsReadOnly = false;
                    txtRagioneSociale.IsReadOnly = false;
                    txtCodiceFiscale.IsReadOnly = false;
                    txtPartitaIVA.IsReadOnly = false;
                    txtEsercizioDal.IsReadOnly = false;
                    txtEsercizioAl.IsReadOnly = false;
                    rdbEsercizioAcavallo.IsHitTestVisible = true;
                    rdbEsercizioSolare.IsHitTestVisible = true;
                    idRecord = App.MasterFile_NewID;
                    break;
                case App.TipoAttivitaScheda.Edit:
                    labelTitolo.Content = "Modifica Anagrafica";
                    buttonComando.Content = "Salva";
                    buttonComando.Visibility = System.Windows.Visibility.Visible;
                    //attivo controlli
                    txtNote.IsReadOnly = false;
                    txtRagioneSociale.IsReadOnly = false;
                    txtCodiceFiscale.IsReadOnly = false;
                    txtPartitaIVA.IsReadOnly = false;
                    txtEsercizioDal.IsReadOnly = false;
                    txtEsercizioAl.IsReadOnly = false;
                    rdbEsercizioAcavallo.IsHitTestVisible = true;
                    rdbEsercizioSolare.IsHitTestVisible = true;
                    //configurazione stato
                    mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.InUso);
                    break;
                case App.TipoAttivitaScheda.Delete:
                    labelTitolo.Content = "Elimina Anagrafica";
                    buttonComando.Content = "Elimina";
                    buttonComando.Visibility = System.Windows.Visibility.Visible;
                    //configurazione stato
                    mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.InUso);
                    break;
                case App.TipoAttivitaScheda.Export:
                    labelTitolo.Content = "Esporta Anagrafica e documenti collegati";
                    buttonComando.Content = "Esporta";
                    buttonComando.Visibility = System.Windows.Visibility.Visible;
                    //configurazione stato
                    mf.SetAnafraficaStato( idRecord, App.TipoAnagraficaStato.InUso );
                    break;
                case App.TipoAttivitaScheda.Condividi:
                    labelTitolo.Content = "Condividi Anagrafica e documenti collegati";
                    buttonComando.Content = "Condividi";
                    buttonComando.Visibility = System.Windows.Visibility.Visible;
                    break;
                case App.TipoAttivitaScheda.View:
        				default:
                    labelTitolo.Content = "Apri Anagrafica in sola lettura";
                    break;
            }
        }


        private void btnSblocca_VisualizzaNascondi(App.TipoAnagraficaStato statoAnagrafica)
        {
            if (statoAnagrafica == App.TipoAnagraficaStato.Bloccato || statoAnagrafica == App.TipoAnagraficaStato.Esportato || statoAnagrafica == App.TipoAnagraficaStato.InUso || statoAnagrafica == App.TipoAnagraficaStato.Sconosciuto)
            {
                buttonSblocca.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void buttonComando_Click(object sender, RoutedEventArgs e)
        {
            MasterFile mf = MasterFile.Create();
            Utilities u = new Utilities();

            switch (TipologiaAttivita)
            {
              //Nuovo e salva
              case App.TipoAttivitaScheda.New:
              case App.TipoAttivitaScheda.Edit:
                //Campi Obbligatorio
                if (!u.ConvalidaDatiInterfaccia(txtRagioneSociale, "Ragione Sociale mancante."))
                    return;
                if (!u.ConvalidaDatiInterfaccia(txtCodiceFiscale, "Codice Fiscale mancante."))
                    return;
                if (!u.ConvalidaDatiInterfaccia(rdbEsercizioAcavallo, "Selezionare tipologia Esercizio."))
						      return;
                if (!u.ConvalidaDatiInterfaccia(txtEsercizioDal, "Inizio Esercizio mancante.", "Formato GG/MM"))
						      return;
                if (!u.ConvalidaDatiInterfaccia(txtEsercizioAl, "Fine Esercizio mancante.", "Formato GG/MM"))
						      return;

					      if (rdbEsercizioAcavallo.IsChecked == true)
					      {
						      try 
						      {
                    //calcola la durata ipotetica del periodo in esame
                    TimeSpan ts = Convert.ToDateTime(txtEsercizioAl.Text.Trim() + "/2013").Subtract(Convert.ToDateTime(txtEsercizioDal.Text.Trim() + "/2012"));
							      if (ts.Days != 364)
							      {
								      MessageBox.Show("Attenzione, periodo a cavallo inferiore ai 365 giorni");
							      }
						      }
						      catch (Exception ex)
						      {
							      string log = ex.Message;
						      }						
					      }

                //setto dati
                Hashtable ht = new Hashtable();
                ht.Add("RagioneSociale", txtRagioneSociale.Text.Trim());
                ht.Add("CodiceFiscale", txtCodiceFiscale.Text.Trim());
                ht.Add("PartitaIVA", txtPartitaIVA.Text.Trim());
                ht.Add("Note", txtNote.Text.Trim());
                ht.Add("EsercizioDal", txtEsercizioDal.Text.Trim());
                ht.Add("EsercizioAl", txtEsercizioAl.Text.Trim());

                if(rdbEsercizioSolare.IsChecked == false && rdbEsercizioAcavallo.IsChecked == false)
                {
                    ht.Add("Esercizio", (int)(App.TipoAnagraficaEsercizio.Sconosciuto));
                }
                else
                {
                    if(rdbEsercizioSolare.IsChecked == true)
                    {
                        ht.Add("Esercizio", (int)(App.TipoAnagraficaEsercizio.AnnoSolare));
                    }
                        
                    if(rdbEsercizioAcavallo.IsChecked == true)
                    {
                        ht.Add("Esercizio", (int)(App.TipoAnagraficaEsercizio.ACavallo));
                    }

                }

                if ( rdbOrganoControlloSindaco.IsChecked == true )
                {
                    ht.Add( "OrganoDiControllo", 2 );
                }
                else if ( rdbOrganoControlloCollegio.IsChecked == true )
                {
                    ht.Add( "OrganoDiControllo", 1 );
                }
                else
                {
                    ht.Add("OrganoDiControllo", 3);
                }


                if ( rdbOrganoRevisioneAutonomo.IsChecked == true )
                {
                    ht.Add( "OrganoDiRevisione", 2 );
                }
                else if ( rdbOrganoSocietaRevisione.IsChecked == true )
                {
                    ht.Add( "OrganoDiRevisione", 3 );
                }
                else
                {
                    ht.Add( "OrganoDiRevisione", 1 );
                }

                    
                ht.Add( "Presidente", txtPresidente.Text.Trim() );
                ht.Add( "MembroEffettivo", txtMembroEffettivo.Text.Trim() );
                ht.Add( "MembroEffettivo2", txtMembroEffettivo2.Text.Trim() );

                ht.Add("SindacoSupplente", txtSindacoSupplente.Text.Trim());
                ht.Add("SindacoSupplente2", txtSindacoSupplente2.Text.Trim());

                    
                ht.Add( "RevisoreAutonomo", txtRevisoreAutonomo.Text.Trim() );

                if ( mf.ClienteGiaPresente( ht, idRecord ) )
                {
                    MessageBox.Show( "Attenzione, Partita IVA o Codice Fiscale già presente per altro Cliente" );
                    return;
                }

                idRecord = mf.SetAnagrafica(ht, idRecord);
                RegistrazioneEffettuata = true;
    					  _DatiCambiati = false;
                ((MainWindow)(Owner)).CaricaClienti();
                break;
              case App.TipoAttivitaScheda.Delete:
                  //richiesta conferma
                  if (MessageBoxResult.No == u.ConfermaCancellazione())
                      return;
                  //cancellazione
                  mf.DeleteAnagrafica(idRecord);
                  RegistrazioneEffettuata = true;
                  break;
              case App.TipoAttivitaScheda.Condividi:
                if ( MessageBoxResult.No == u.ConfermaCondivisione() )
                    return;

                //esportazione su file
                char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
                string RagioneSociale = new string
                                                (
                                                    ("Condivisione di " + txtRagioneSociale.Text)
                                                        .Where( x => !invalidChars.Contains( x ) )
                                                        .ToArray()
                                                );
                //nome file di esportazione
                string nomeFile = RagioneSociale + u.EstensioneFile( App.TipoFile.ImportExport );
                string ret = u.sys_SaveFileDialog( nomeFile, App.TipoFile.ImportExport );
                if ( ret != null )
                {
                    //Process wait - START
                    ProgressWindow pw = new ProgressWindow();

                    //ANDREA 2.8
                    //backup file di importazione
                    string retBis = App.AppTempFolder + "{" + Guid.NewGuid().ToString() + "}";
                    cImportExport.ExportNoVerbose( retBis, idRecord, true );

                    //esportazione
                    cImportExport.Export( ret, idRecord, true );

                    //Process wait - STOP
                    pw.Close();

                    //interfaccia
                    MessageBox.Show( "Condivisione avvenuta con successo" );
                    RegistrazioneEffettuata = true;
                }
                    
                break;
              case App.TipoAttivitaScheda.Export:
                if (MessageBoxResult.No == u.ConfermaEsportazione())
                    return;

                //esportazione via lan
                //if (App.CodiceMacchinaServer.Trim().Split('-')[0] != "" && App.CodiceMacchina.Split('-')[0] != App.CodiceMacchinaServer.Split('-')[0])
                //if (App.TipoLicenza == App.TipologieLicenze.ClientLan || App.TipoLicenza == App.TipologieLicenze.ClientLanMulti)
                //4.5.1
                if (App.AppConsentiImportazioneEsportazioneLan)
                {
                    try
                    {
                        //Process wait - START
                        ProgressWindow pw = new ProgressWindow();

                        //backup file di importazione
                        ret = App.AppTempFolder + "{" + Guid.NewGuid().ToString() + "}";
                        bool returnvalue = cImportExport.ExportNoVerbose(ret, idRecord, false);

                        if (returnvalue == false)
                        {
                            //Process wait - STOP                           
                            pw.Close();

                            MessageBox.Show("Attenzione, rete discontinua, troppo lenta o diritti insufficienti sul server. Procedimento non portato a termine per evitare danni sui dati.");
                            return;
                        }
                        //configurazione stato origine (pre 2.8)
                        //mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.Esportato);
                        //cancellazione richiesta del cliente sul clien (dalla 2.8)


                        //imposto percorso archivi di destinazione
                        if (App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Remoto)
                        {
                            //Imposto percorsi su archivio locale
                            App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.LocaleImportExport;
                            //salvo nuova configurazione
                            GestioneLicenza l = new GestioneLicenza();
                            l.SalvaInfoDataUltimoUtilizzo();
                            //Setup path
                            u.ConfiguraPercorsi();
                        }
                        else
                        {
                            //cancello in locale l'utente
                            returnvalue = mf.DeleteAnagrafica(idRecord);

                            if (returnvalue == false)
                            {
                                //importo file in locale
                                cImportExport.Import(ret, false);

                                //Process wait - STOP                           
                                pw.Close();

                                MessageBox.Show("Attenzione, rete discontinua, troppo lenta o diritti insufficienti sul server. Procedimento non portato a termine.");
                                return;
                            }

                            //Imposto percorsi su archivio remoto
                            App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Remoto;
                            //salvo nuova configurazione
                            GestioneLicenza l = new GestioneLicenza();
                            l.SalvaInfoDataUltimoUtilizzo();
                            //setup path
                            u.ConfiguraPercorsi();
                        }

                        //importo file in locale
                        returnvalue = cImportExport.Import(ret, false);

                        if (returnvalue == false)
                        {
                            //switcho nuovamente per rollback
                            if (App.AppSetupTipoGestioneArchivio == App.TipoGestioneArchivio.Remoto)
                            {
                                App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.LocaleImportExport;
                                //salvo nuova configurazione
                                GestioneLicenza l = new GestioneLicenza();
                                l.SalvaInfoDataUltimoUtilizzo();
                                //setup path
                                u.ConfiguraPercorsi();
                            }
                            else
                            {
                                //Imposto percorsi su archivio remoto
                                App.AppSetupTipoGestioneArchivio = App.TipoGestioneArchivio.Remoto;
                                //salvo nuova configurazione
                                GestioneLicenza l = new GestioneLicenza();
                                l.SalvaInfoDataUltimoUtilizzo();
                                //setup path
                                u.ConfiguraPercorsi();
                            }

                            cImportExport.Import(ret, false);

                            //Process wait - STOP                           
                            pw.Close();

                            MessageBox.Show("Attenzione, rete discontinua, troppo lenta o diritti insufficienti sul server. Procedimento non portato a termine.");
                            return;
                        }

                        //Process wait - STOP
                        pw.Close();

                        RegistrazioneEffettuata = true;

                        mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.Esportato);

                        MasterFile.ForceRecreate();


                        ////andrea 3.4 ********************************************************** ACCANTONATO ????
                        ////verifico disponibilità del cliente su archivio remoto
                        //int idDest = mf.ClienteGiaPresente( mf.GetAnagrafica( idRecord ) );
                        //if (mf.GetAnafraficaStato(idDest) == App.TipoAnagraficaStato.Esportato || mf.GetAnafraficaStato(idDest) == App.TipoAnagraficaStato.Disponibile)
                        //{
                        //    //importo file in locale / esporto in remoto
                        //    cImportExport.Import(ret, false);
                        //    RegistrazioneEffettuata = true;
                        //}
                        //else
                        //{
                        //    RegistrazioneEffettuata = false;
                        //}

                        ////Process wait - STOP
                        //pw.Close();

                        ////andrea 3.4
                        //if (!RegistrazioneEffettuata)
                        //{
                        //    MessageBox.Show("Attenzione, anagrafica in uso, impossibile esportare.\n\n");
                        //}
                    }
                    catch (Exception ex)
                    {
                        string log = ex.Message;
                        MessageBox.Show("Attenzione, rete discontinua, troppo lenta o diritti insufficienti sul server. Procedimento non portato a termine.");
                        return;
                    }
                       

                }
                else
                {
                    //esportazione su file
                    invalidChars = System.IO.Path.GetInvalidFileNameChars();
                    RagioneSociale = new string
                                                    (
                                                        txtRagioneSociale.Text
                                                            .Where(x => !invalidChars.Contains(x))
                                                            .ToArray()
                                                    );
                    //nome file di esportazione
                    nomeFile = RagioneSociale + u.EstensioneFile(App.TipoFile.ImportExport);
                    ret = u.sys_SaveFileDialog(nomeFile, App.TipoFile.ImportExport);
                    if (ret != null)
                    {
                        //Process wait - START
                        ProgressWindow pw = new ProgressWindow();

                        //ANDREA 2.8
                        //backup file di importazione
                        string retBis = App.AppTempFolder + "{" + Guid.NewGuid().ToString() + "}";

                        bool returnvalue = cImportExport.ExportNoVerbose(retBis, idRecord, false);

                        if (returnvalue == false)
                        {
                            //Process wait - STOP                           
                            pw.Close();

                            MessageBox.Show("Attenzione, Procedimento non portato a termine per evitare danni sui dati. Ritentare controllando che vi sia spazio sufficiente nella directory.");
                            return;
                        }

                        //esportazione
                        returnvalue = cImportExport.Export(ret, idRecord, false);

                        if (returnvalue == false)
                        {
                            //Process wait - STOP                           
                            pw.Close();

                            MessageBox.Show("Attenzione, Procedimento non portato a termine per evitare danni sui dati. Ritentare controllando che vi sia spazio sufficiente nella directory.");
                            return;
                        }

                        //Process wait - STOP
                        pw.Close();

                        //elimino anagrafica
                        //if (App.TipoLicenza == App.TipologieLicenze.Viewer)
                        //{
                        //    //cancello in locale l'utente
                        //    returnvalue = mf.DeleteAnagrafica(idRecord);

                        //    if (returnvalue == false)
                        //    {
                        //        //importo file in locale
                        //        cImportExport.Import(ret, false);

                        //        //Process wait - STOP                           
                        //        pw.Close();

                        //        MessageBox.Show("Attenzione, rete discontinua, troppo lenta o diritti insufficienti sul server. Procedimento non portato a termine.");
                        //        return;
                        //    }
                        //}
                        //else
                        //{
                        //    //configurazione stato
                        //    mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.Esportato);
                        //}


                        if (App.CodiceMacchina != App.CodiceMacchinaServer && App.NumeroanAgrafiche == 1)
                        {
                            mf.DeleteAnagrafica(idRecord);
                        }
                        else
                        {
                            //configurazione stato
                            mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.Esportato);
                        }

                        //interfaccia
                        MessageBox.Show("Esportazione avvenuta con successo");
                        mf.SetAnafraficaStato( idRecord, App.TipoAnagraficaStato.Esportato );
                        RegistrazioneEffettuata = true;
                    }
                }

                    
            
                break;
            }

            
            //chiudo maschera
            base.Close();
        }

        
        private void GestoreEvento_DatiCambiati(object sender, RoutedEventArgs e)
        {
            //configura esercizio solare
            if (sender.GetType().Name == "RadioButton")
            {
                if (((RadioButton)sender).Name == "rdbEsercizioSolare")
                {
                    txtEsercizioDal.Text = "01/01";
                    txtEsercizioAl.Text = "31/12";
                }

                if ( ((RadioButton)sender).Name == "rdbOrganoRevisioneControllo" )
                {
                    lblRevisoreAutonomo.Visibility = System.Windows.Visibility.Collapsed;
                    txtRevisoreAutonomo.Visibility = System.Windows.Visibility.Collapsed;
                    txtRevisoreAutonomo.Text = "";
                    txtRevisoreAutonomo.IsEnabled = false;
                }

                if ( ( (RadioButton)sender ).Name == "rdbOrganoRevisioneAutonomo" || ( (RadioButton)sender ).Name == "rdbOrganoSocietaRevisione" )
                {
                    lblRevisoreAutonomo.Visibility = System.Windows.Visibility.Visible;

                    if ( ( (RadioButton)sender ).Name == "rdbOrganoRevisioneAutonomo" )
                    {
                        lblRevisoreAutonomo.Text = "Revisore";
                    }
                    else
                    {
                        lblRevisoreAutonomo.Text = "Società di Revisione";
                    }

                    txtRevisoreAutonomo.Visibility = System.Windows.Visibility.Visible;
                    txtRevisoreAutonomo.Text = "";
                    txtRevisoreAutonomo.IsEnabled = true;
                }

                if ( ((RadioButton)sender).Name == "rdbOrganoControlloCollegio" )
                {
                    lblPresidenteSindacoUnico.Visibility = System.Windows.Visibility.Visible;
                    lblPresidenteSindacoUnico.Text = "Presidente";
                    txtPresidente.Visibility = System.Windows.Visibility.Visible;
                    txtPresidente.IsEnabled = true;

                    lblMembro.Visibility = System.Windows.Visibility.Visible;
                    txtMembroEffettivo.Visibility = System.Windows.Visibility.Visible;
                    txtMembroEffettivo.IsEnabled = true;

                    lblMembro2.Visibility = System.Windows.Visibility.Visible;
                    txtMembroEffettivo2.Visibility = System.Windows.Visibility.Visible;
                    txtMembroEffettivo2.IsEnabled = true;
                    
                    lblSindacoSupplente.Visibility = System.Windows.Visibility.Visible;
                    txtSindacoSupplente.Visibility = System.Windows.Visibility.Visible;
                    txtSindacoSupplente.IsEnabled = true;

                    lblSindacoSupplente2.Visibility = System.Windows.Visibility.Visible;
                    txtSindacoSupplente2.Visibility = System.Windows.Visibility.Visible;
                    txtSindacoSupplente2.IsEnabled = true;
                }

                if ( ((RadioButton)sender).Name == "rdbOrganoControlloSindaco" )
                {
                    lblPresidenteSindacoUnico.Visibility = System.Windows.Visibility.Visible;
                    lblPresidenteSindacoUnico.Text = "Sindaco unico";
                    txtPresidente.Visibility = System.Windows.Visibility.Visible;
                    txtPresidente.IsEnabled = true;

                    lblMembro.Visibility = System.Windows.Visibility.Collapsed;
                    txtMembroEffettivo.Visibility = System.Windows.Visibility.Collapsed;
                    txtMembroEffettivo.IsEnabled = false;

                    lblMembro2.Visibility = System.Windows.Visibility.Collapsed;
                    txtMembroEffettivo2.Visibility = System.Windows.Visibility.Collapsed;
                    txtMembroEffettivo2.IsEnabled = false;


                    lblSindacoSupplente.Visibility = System.Windows.Visibility.Collapsed;
                    txtSindacoSupplente.Visibility = System.Windows.Visibility.Collapsed;
                    txtSindacoSupplente.IsEnabled = false;

                    lblSindacoSupplente2.Visibility = System.Windows.Visibility.Collapsed;
                    txtSindacoSupplente2.Visibility = System.Windows.Visibility.Collapsed;
                    txtSindacoSupplente2.IsEnabled = false;
                }

                if (((RadioButton)sender).Name == "rdbOrganoControlloAssente")
                {
                    lblPresidenteSindacoUnico.Visibility = System.Windows.Visibility.Collapsed;
                    lblPresidenteSindacoUnico.Text = "";
                    txtPresidente.Visibility = System.Windows.Visibility.Collapsed;
                    txtPresidente.IsEnabled = true;

                    lblMembro.Visibility = System.Windows.Visibility.Collapsed;
                    txtMembroEffettivo.Visibility = System.Windows.Visibility.Collapsed;
                    txtMembroEffettivo.IsEnabled = true;

                    lblMembro2.Visibility = System.Windows.Visibility.Collapsed;
                    txtMembroEffettivo2.Visibility = System.Windows.Visibility.Collapsed;
                    txtMembroEffettivo2.IsEnabled = true;

                    lblSindacoSupplente.Visibility = System.Windows.Visibility.Collapsed;
                    txtSindacoSupplente.Visibility = System.Windows.Visibility.Collapsed;
                    txtSindacoSupplente.IsEnabled = false;

                    lblSindacoSupplente2.Visibility = System.Windows.Visibility.Collapsed;
                    txtSindacoSupplente2.Visibility = System.Windows.Visibility.Collapsed;
                    txtSindacoSupplente2.IsEnabled = false;
                }
            }

            if (_InCaricamento)
                return;

            _DatiCambiati = true;
        }

        private void GestoreEvento_ChiusuraFinestra(object sender, CancelEventArgs e)
        {
            if(annulla)
            {
                return;
            }

            //Configuro stato
            if (TipologiaAttivita == App.TipoAttivitaScheda.Edit || (TipologiaAttivita == App.TipoAttivitaScheda.Export && !RegistrazioneEffettuata) || (TipologiaAttivita == App.TipoAttivitaScheda.Delete && !RegistrazioneEffettuata))
			{
				MasterFile mf = MasterFile.Create();
				mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.Disponibile);
			}
            //dati non modificati
            if (!_DatiCambiati)
                return;
            //dati modificati
            Utilities u = new Utilities();
            if (MessageBoxResult.No == u.AvvisoPerditaDati())
                e.Cancel = true;
        }

        private void GestoreEvento_ComboEsercizio_Checked(object sender, CancelEventArgs e)
        {
            _DatiCambiati = true;
        }

        private void buttonSblocca_Click(object sender, RoutedEventArgs e)
        {
            //richiesta conferma
            Utilities u = new Utilities();
            if (MessageBoxResult.Yes == u.ConfermaSbloccoUtente())
            {
 
                MasterFile mf = MasterFile.Create();

                if ( mf.GetAnafraficaStato( idRecord ) != App.TipoAnagraficaStato.Esportato )
                {

                    mf.SetAnafraficaStato( idRecord, App.TipoAnagraficaStato.Disponibile );

                    ((RevisoftApplication.MainWindow)(this.Owner)).CaricaClienti();

                    MessageBox.Show( "Sblocco Cliente Avvenuto con successo" );
                }
                else
                {
                    if (MessageBox.Show("Confermare sblocco del cliente attualmente esportato su altro PC? ( ATTENZIONE operazione irreversibile. )", "Attenzione", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
                    {
                        mf.SetAnafraficaStato(idRecord, App.TipoAnagraficaStato.Disponibile);

                        ((RevisoftApplication.MainWindow)(this.Owner)).CaricaClienti();

                        MessageBox.Show("Sblocco Cliente Avvenuto con successo");
                    }
                }
            }
        }

        private void buttonChiudi_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }
                
        private void buttonAnnulla_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MasterFile mf = MasterFile.Create();
            Utilities u = new Utilities();
            
            wSchedaCambioEsercizio ece = new wSchedaCambioEsercizio();
            ece.Owner = this;
            ece.ShowDialog();


            //Campi Obbligatorio
            if (!u.ConvalidaDatiInterfaccia(txtRagioneSociale, "Ragione Sociale mancante."))
                return;
            if (!u.ConvalidaDatiInterfaccia(txtCodiceFiscale, "Codice Fiscale mancante."))
                return;
            if (!u.ConvalidaDatiInterfaccia(rdbEsercizioAcavallo, "Selezionare tipologia Esercizio."))
                return;
            if (!u.ConvalidaDatiInterfaccia(txtEsercizioDal, "Inizio Esercizio mancante.", "Formato GG/MM"))
                return;
            if (!u.ConvalidaDatiInterfaccia(txtEsercizioAl, "Fine Esercizio mancante.", "Formato GG/MM"))
                return;

            if (rdbEsercizioAcavallo.IsChecked == true)
            {
                try
                {
                    //calcola la durata ipotetica del periodo in esame
                    TimeSpan ts = Convert.ToDateTime(txtEsercizioAl.Text.Trim() + "/2013").Subtract(Convert.ToDateTime(txtEsercizioDal.Text.Trim() + "/2012"));
                    if (ts.Days != 364)
                    {
                        MessageBox.Show("Attenzione, periodo a cavallo inferiore ai 365 giorni");
                    }
                }
                catch (Exception ex)
                {
                    string log = ex.Message;
                }
            }

            //setto dati
            Hashtable ht = new Hashtable();
            ht.Add("RagioneSociale", txtRagioneSociale.Text.Trim());
            ht.Add("CodiceFiscale", txtCodiceFiscale.Text.Trim());
            ht.Add("PartitaIVA", txtPartitaIVA.Text.Trim());
            ht.Add("Note", txtNote.Text.Trim());
            ht.Add("EsercizioDal", txtEsercizioDal.Text.Trim());
            ht.Add("EsercizioAl", txtEsercizioAl.Text.Trim());

            if (rdbEsercizioSolare.IsChecked == false && rdbEsercizioAcavallo.IsChecked == false)
            {
                ht.Add("Esercizio", (int)(App.TipoAnagraficaEsercizio.Sconosciuto));
            }
            else
            {
                if (rdbEsercizioSolare.IsChecked == true)
                {
                    ht.Add("Esercizio", (int)(App.TipoAnagraficaEsercizio.AnnoSolare));
                }

                if (rdbEsercizioAcavallo.IsChecked == true)
                {
                    ht.Add("Esercizio", (int)(App.TipoAnagraficaEsercizio.ACavallo));
                }

            }

            if (rdbOrganoControlloSindaco.IsChecked == true)
            {
                ht.Add("OrganoDiControllo", 2);
            }
            else if (rdbOrganoControlloCollegio.IsChecked == true)
            {
                ht.Add("OrganoDiControllo", 1);
            }
            else
            {
                ht.Add("OrganoDiControllo", 3);
            }


            if (rdbOrganoRevisioneAutonomo.IsChecked == true)
            {
                ht.Add("OrganoDiRevisione", 2);
            }
            else if (rdbOrganoSocietaRevisione.IsChecked == true)
            {
                ht.Add("OrganoDiRevisione", 3);
            }
            else
            {
                ht.Add("OrganoDiRevisione", 1);
            }


            ht.Add("Presidente", txtPresidente.Text.Trim());
            if(rdbOrganoControlloSindaco.IsChecked == true)
            {
                ht.Add("MembroEffettivo", "");
                ht.Add("MembroEffettivo2", "");

                ht.Add("SindacoSupplente", "");
                ht.Add("SindacoSupplente2", "");

            }
            else
            {
                ht.Add("MembroEffettivo", txtMembroEffettivo.Text.Trim());
                ht.Add("MembroEffettivo2", txtMembroEffettivo2.Text.Trim());

                ht.Add("SindacoSupplente", txtSindacoSupplente.Text.Trim());
                ht.Add("SindacoSupplente2", txtSindacoSupplente2.Text.Trim());
            }
            
            ht.Add("RevisoreAutonomo", txtRevisoreAutonomo.Text.Trim());

            if (mf.ClienteGiaPresente(ht, idRecord))
            {
                MessageBox.Show("Attenzione, Partita IVA o Codice Fiscale già presente per altro Cliente");
                return;
            }

            idRecord = mf.SetAnagrafica(ht, idRecord);

        }
    }
}
*/
