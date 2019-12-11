//----------------------------------------------------------------------------+
//                            wSchedaISQC.xaml.cs                             |
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
using System.Collections;
using System.ComponentModel;
using System.Windows.Threading;

namespace RevisoftApplication
{
  public partial class wSchedaISQC : Window
  {
    private App.TipoAttivitaScheda _tipologiaAttivita;
    private App.TipoAttivitaScheda oldTipologiaAttivita = App.TipoAttivitaScheda.View;
    private bool _riesame;
    private bool firsttime = true;

    public string IDISQC;

    private bool _InCaricamento;
    private bool _DatiCambiati;
    public bool RegistrazioneEffettuata;

    public bool noopenaftercreate = false;

    private bool _cmbInCaricamento = false;
    private int OldSelectedCmbClienti = -1;

    public string IDClienteImport = "-1";

    Hashtable htClienti = new Hashtable();
    Hashtable htDate = new Hashtable();

    public App.TipoAttivitaScheda TipologiaAttivita
    {
      get { return _tipologiaAttivita; }
      set
      {
        if (!firsttime && _tipologiaAttivita == value) return;
        firsttime = false;
        _tipologiaAttivita = value;
        MasterFile mf = MasterFile.Create();
        int index = 0;
        int selectedIndex = -1;
        if (cmbClienti.Items.Count != 0)
        {
          _cmbInCaricamento = true;
          selectedIndex = cmbClienti.SelectedIndex;
          cmbClienti.Items.Clear();
          htClienti.Clear();
        }
        List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>();
        foreach (Hashtable item in mf.GetAnagrafiche())
        {
          if (IDClienteImport == "-1" || IDClienteImport == item["ID"].ToString())
          {
            if (_tipologiaAttivita != App.TipoAttivitaScheda.New)
            {
              if (mf.GetISQCs(item["ID"].ToString()).Count == 0)
              {
                continue;
              }
            }
            string cliente = item["RagioneSociale"].ToString();
            if (IDClienteImport == "-1")
            {
              switch (((App.TipoAnagraficaStato)(Convert.ToInt32(item["Stato"].ToString()))))
              {
              //MM  case App.TipoAnagraficaStato.InUso:
              //    cliente += " (In Uso)";
              //   break;
                case App.TipoAnagraficaStato.Bloccato:
                  cliente += " (Bloccato)";
                  break;
                case App.TipoAnagraficaStato.Esportato:
                  cliente += " (Esportato)";
                  break;
                case App.TipoAnagraficaStato.Disponibile:
                case App.TipoAnagraficaStato.Sconosciuto:
                default:
                  break;
              }
            }
            myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), cliente));
          }
        }
        myList.Sort
        (
          delegate (KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
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
        _cmbInCaricamento = false;
      }
    }

    //----------------------------------------------------------------------------+
    //                                wSchedaISQC                                 |
    //----------------------------------------------------------------------------+
    public wSchedaISQC()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      //var
      _InCaricamento = true;
      _DatiCambiati = false;
      RegistrazioneEffettuata = false;
      //interfaccia
      buttonComando.Visibility = System.Windows.Visibility.Hidden;
      cmbClienti.Focus();
    }

    public bool Riesame
    {
      get
      {
        return _riesame;
      }
      set
      {
        _riesame = value;
        if (value)
        {
          labelTitolo.Content = labelTitolo.Content.ToString().Split('(')[0] + "Riesame___Incarico";
          rdbAttivitaNomina.IsChecked = false;
          rdbAttivitaRiesame.IsChecked = true;
        }
        else
        {
          labelTitolo.Content = labelTitolo.Content.ToString().Split('(')[0] + "ISQC";
          rdbAttivitaNomina.IsChecked = true;
          rdbAttivitaRiesame.IsChecked = false;
        }
      }
    }

    //----------------------------------------------------------------------------+
    //                             ConfiguraMaschera                              |
    //----------------------------------------------------------------------------+
    public void ConfiguraMaschera()
    {
      //inibisco tutto i controlli
      dtpDataNomina.IsHitTestVisible = false;
      dtpDataFine.IsHitTestVisible = false;
      rdbAttivitaNomina.IsHitTestVisible = false;
      rdbAttivitaRiesame.IsHitTestVisible = false;
      rdbCollegioSindacale.IsHitTestVisible = false;
      rdbRevisoreSincoUnico.IsHitTestVisible = false;
      txtNota.IsReadOnly = true;
      //nascondo testo help - non + usato
      textBlockDescrizione.Text = "";
      textBlockDescrizione.Visibility = System.Windows.Visibility.Collapsed;
      switch (TipologiaAttivita)
      {
        case App.TipoAttivitaScheda.New:
          labelTitolo.Content = "Nuovo ";
          Riesame = _riesame;
          buttonComando.Content = "Crea";
          GridComboData.Visibility = System.Windows.Visibility.Collapsed;
          buttonComando.Visibility = System.Windows.Visibility.Visible;
          buttonApri.Visibility = System.Windows.Visibility.Hidden;
          //abilito controlli
          dtpDataNomina.IsHitTestVisible = true;
          dtpDataFine.IsHitTestVisible = true;
          rdbAttivitaNomina.IsHitTestVisible = true;
          rdbAttivitaRiesame.IsHitTestVisible = true;
          rdbCollegioSindacale.IsHitTestVisible = true;
          rdbRevisoreSincoUnico.IsHitTestVisible = true;
          txtNota.IsReadOnly = false;
          break;
        case App.TipoAttivitaScheda.Edit:
          labelTitolo.Content = "Modifica ";
          Riesame = _riesame;
          buttonComando.Content = "Salva";
          buttonComando.Visibility = System.Windows.Visibility.Collapsed;
          buttonApri.Visibility = System.Windows.Visibility.Visible;
          //abilito controlli
          dtpDataNomina.IsHitTestVisible = true;
          dtpDataFine.IsHitTestVisible = true;
          rdbAttivitaNomina.IsHitTestVisible = true;
          rdbAttivitaRiesame.IsHitTestVisible = true;
          rdbCollegioSindacale.IsHitTestVisible = true;
          rdbRevisoreSincoUnico.IsHitTestVisible = true;
          txtNota.IsReadOnly = false;
          break;
        case App.TipoAttivitaScheda.Delete:
          labelTitolo.Content = "Elimina ";
          Riesame = _riesame;
          buttonComando.Content = "Elimina";
          buttonComando.Visibility = System.Windows.Visibility.Visible;
          buttonApri.Visibility = System.Windows.Visibility.Collapsed;
          break;
        case App.TipoAttivitaScheda.Export:
          labelTitolo.Content = "Esporta ";
          Riesame = _riesame;
          buttonComando.Content = "Esporta";
          buttonComando.Visibility = System.Windows.Visibility.Visible;
          buttonApri.Visibility = System.Windows.Visibility.Visible;
          break;
        case App.TipoAttivitaScheda.View:
        default:
          labelTitolo.Content = "Apri ISQC/Riesame Incarico in sola lettura";
          //Riesame = _riesame;
          cmbData.Visibility = System.Windows.Visibility.Visible;
          buttonApri.Visibility = System.Windows.Visibility.Visible;
          buttonApri.Margin = buttonComando.Margin;
          break;
      }
      _InCaricamento = false;
      MasterFile mf = MasterFile.Create();
      string IDCliente = mf.GetClienteFissato();
      foreach (DictionaryEntry item in htClienti)
      {
        if (item.Value.ToString() == IDCliente)
        {
          cmbClienti.SelectedIndex = Convert.ToInt32(item.Key.ToString());
          return;
        }
      }
    }

    //----------------------------------------------------------------------------+
    //                        cmbClienti_SelectionChanged                         |
    //----------------------------------------------------------------------------+
    private void cmbClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      //rilascio blocco su selezione precedente
      //if (OldSelectedCmbClienti != -1)
      //{
      //    MasterFile mf = MasterFile.Create();
      //    mf.SetAnafraficaStato(Convert.ToInt32(OldSelectedCmbClienti), App.TipoAnagraficaStato.Disponibile);
      //}
      //interfaccia
      functionCmbClientiChanged(((ComboBox)sender));
      ConfiguraMaschera();
      if (TipologiaAttivita == App.TipoAttivitaScheda.New)
        dtpDataNomina.Focus();
      else
        cmbData.Focus();
    }

    //----------------------------------------------------------------------------+
    //                         functionCmbClientiChanged                          |
    //----------------------------------------------------------------------------+
    private void functionCmbClientiChanged(ComboBox cmb)
    {
      if (_cmbInCaricamento) return;
      if (oldTipologiaAttivita != App.TipoAttivitaScheda.View)
      {
        TipologiaAttivita = oldTipologiaAttivita;
      }
      _InCaricamento = true;
      cmbData.SelectedIndex = -1;
      dtpDataNomina.Text = "";
      dtpDataFine.Text = "";
      rdbAttivitaNomina.IsChecked = false;
      rdbAttivitaRiesame.IsChecked = false;
      rdbCollegioSindacale.IsChecked = false;
      rdbRevisoreSincoUnico.IsChecked = false;
      txtNota.Text = "";
      if (cmb.SelectedIndex != -1)
      {
        try
        {
          string IDCliente = htClienti[cmb.SelectedIndex].ToString();
          OldSelectedCmbClienti = Convert.ToInt32(IDCliente);
          MasterFile mf = MasterFile.Create();
          int index = 0;
          htDate.Clear();
          cmbData.Items.Clear();
          Hashtable attivita = new Hashtable();
          List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>();
          foreach (Hashtable item in mf.GetISQCs(IDCliente))
          {
            switch ((App.TipoISQCAttivita)(Convert.ToInt32(item["Attivita"].ToString())))
            {
              case App.TipoISQCAttivita.Nomina:
                attivita.Add(item["ID"].ToString(), "");
                break;
              case App.TipoISQCAttivita.Riesame:
                attivita.Add(item["ID"].ToString(), "");
                break;
              case App.TipoISQCAttivita.Sconosciuto:
              default:
                attivita.Add(item["ID"].ToString(), "");
                break;
            }
            myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), item["DataNomina"].ToString() + " - " + ((item["DataFine"] == null) ? "" : item["DataFine"].ToString())));
          }
          myList.Sort
          (
            delegate (KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
            {
              return Convert.ToDateTime((nextPair.Value.ToString().Split('-'))[0].Trim()).CompareTo(Convert.ToDateTime((firstPair.Value.ToString().Split('-'))[0].Trim()));
            }
          );
          foreach (KeyValuePair<string, string> item in myList)
          {
            cmbData.Items.Add(item.Value + attivita[item.Key].ToString());
            htDate.Add(index, item.Key);
            index++;
          }
          //stato
          if (IDClienteImport == "-1")
          {
            App.TipoAnagraficaStato anaStato = mf.GetAnafraficaStato(Convert.ToInt32(IDCliente));
            //non disponibile: configuro interfaccia
            if (anaStato != App.TipoAnagraficaStato.Disponibile)
            {
              oldTipologiaAttivita = TipologiaAttivita;
              TipologiaAttivita = App.TipoAttivitaScheda.View;
            }
          }
          if (TipologiaAttivita != App.TipoAttivitaScheda.New)
          {
            cmbData.IsEnabled = true;
            dtpDataNomina.IsEnabled = false;
            dtpDataFine.IsEnabled = false;
            rdbAttivitaNomina.IsEnabled = false;
            rdbAttivitaRiesame.IsEnabled = false;
            rdbCollegioSindacale.IsEnabled = false;
            rdbRevisoreSincoUnico.IsEnabled = false;
            txtNota.IsEnabled = false;
          }
          else
          {
            cmbData.IsEnabled = false;
            dtpDataNomina.IsEnabled = true;
            dtpDataFine.IsEnabled = true;
            rdbAttivitaNomina.IsEnabled = true;
            rdbAttivitaRiesame.IsEnabled = true;
            rdbCollegioSindacale.IsEnabled = true;
            rdbRevisoreSincoUnico.IsEnabled = true;
            txtNota.IsEnabled = true;
          }
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wSchedaISQC.functionCmbClientiChanged exception");
          string log = ex.Message;
          cmbData.IsEnabled = false;
          dtpDataNomina.IsEnabled = false;
          dtpDataFine.IsEnabled = false;
          rdbAttivitaNomina.IsEnabled = false;
          rdbAttivitaRiesame.IsEnabled = false;
          rdbCollegioSindacale.IsEnabled = false;
          rdbRevisoreSincoUnico.IsEnabled = false;
          txtNota.IsEnabled = false;
        }
      }
    }

    //----------------------------------------------------------------------------+
    //                          cmbData_SelectionChanged                          |
    //----------------------------------------------------------------------------+
    private void cmbData_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      functionCmbDataChanged(((ComboBox)sender));
      ConfiguraMaschera();
    }

    //----------------------------------------------------------------------------+
    //                           functionCmbDataChanged                           |
    //----------------------------------------------------------------------------+
    private void functionCmbDataChanged(ComboBox cmb)
    {
      if (cmb.SelectedIndex != -1)
      {
        try
        {
          _InCaricamento = true;
          IDISQC = htDate[cmb.SelectedIndex].ToString();
          MasterFile mf = MasterFile.Create();
          Hashtable htISQC = new Hashtable();
          htISQC = mf.GetISQC(IDISQC);
          dtpDataNomina.IsEnabled = true;
          dtpDataFine.IsEnabled = true;
          rdbAttivitaNomina.IsEnabled = true;
          rdbAttivitaRiesame.IsEnabled = true;
          rdbCollegioSindacale.IsEnabled = true;
          rdbRevisoreSincoUnico.IsEnabled = true;
          txtNota.IsEnabled = true;
          dtpDataNomina.Text = htISQC["DataNomina"].ToString();
          dtpDataFine.Text = ((htISQC["DataFine"] == null) ? "" : htISQC["DataFine"].ToString());
          txtNota.Text = htISQC["Note"].ToString();
          switch ((App.TipoISQCComposizione)(Convert.ToInt32(htISQC["Composizione"].ToString())))
          {
            case App.TipoISQCComposizione.CollegioSindacale:
              rdbCollegioSindacale.IsChecked = true;
              rdbRevisoreSincoUnico.IsChecked = false;
              break;
            case App.TipoISQCComposizione.SindacoUnico:
              rdbCollegioSindacale.IsChecked = false;
              rdbRevisoreSincoUnico.IsChecked = true;
              break;
            case App.TipoISQCComposizione.Sconosciuto:
            default:
              break;
          }
          switch ((App.TipoISQCAttivita)(Convert.ToInt32(htISQC["Attivita"].ToString())))
          {
            case App.TipoISQCAttivita.Nomina:
              Riesame = false;
              rdbAttivitaNomina.IsChecked = true;
              rdbAttivitaRiesame.IsChecked = false;
              break;
            case App.TipoISQCAttivita.Riesame:
              Riesame = true;
              rdbAttivitaNomina.IsChecked = false;
              rdbAttivitaRiesame.IsChecked = true;
              break;
            case App.TipoISQCAttivita.Sconosciuto:
            default:
              break;
          }
          _InCaricamento = false;
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wSchedaISQC.functionCmbDataChanged exception");
          string log = ex.Message;
        }
      }
    }

    //----------------------------------------------------------------------------+
    //                            buttonComando_Click                             |
    //----------------------------------------------------------------------------+
    private void buttonComando_Click(object sender, RoutedEventArgs e)
    {
      //controllo selezione clienti
      if (cmbClienti.SelectedIndex == -1)
      {
        MessageBox.Show("selezionare un cliente");
        return;
      }
      MasterFile mf = MasterFile.Create();
      Utilities u = new Utilities();
      int IDCliente = Convert.ToInt32(htClienti[cmbClienti.SelectedIndex].ToString());
      int IDISQC = App.MasterFile_NewID;
      try
      {
        IDISQC = Convert.ToInt32(htDate[cmbData.SelectedIndex].ToString());
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaISQC.buttonComando_Click exception");
        string log = ex.Message;
      }
      if (TipologiaAttivita == App.TipoAttivitaScheda.Delete && IDISQC == -1)
      {
        MessageBox.Show("selezionare un ISQC");
        return;
      }
      App.TipoAttivitaScheda oldTipo = TipologiaAttivita;
      switch (TipologiaAttivita)
      {
        //Nuovo e salva
        case App.TipoAttivitaScheda.New:
        case App.TipoAttivitaScheda.Edit:
          //convalida dati
          //Campi Obbligatorio
          if (!u.ConvalidaDatiInterfaccia(dtpDataNomina, "Data mancante."))
            return;
          if (!u.ConvalidaDatiInterfaccia(dtpDataFine, "Data mancante."))
            return;
          //if (!u.ConvalidaDatiInterfaccia(rdbCollegioSindacale, "Selezionare Collegio o Revisore."))
          //    return;
          //if (!u.ConvalidaDatiInterfaccia(rdbAttivitaNomina, "Selezionare tipologia Attività.") && Riesame == null)
          //    return;
          //Controllo che questa data non sia già stata presa
          if (!mf.CheckDoppio_ISQC(IDISQC, IDCliente, dtpDataNomina.SelectedDate.Value.ToShortDateString()))
          {
            MessageBox.Show("Data già presente per questo cliente");
            return;
          }
          if(TipologiaAttivita== App.TipoAttivitaScheda.New)
          {
            tabControl1.IsEnabled = false;
            gridButtons.IsEnabled = false;
            loading.Visibility = Visibility;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,new Action(delegate { }));
                    //cBusinessObjects.show_workinprogress("Prima creazione dell'albero in corso...");
          }
            

                    //setto dati
                    Hashtable ht = new Hashtable();
          ht.Add("Cliente", IDCliente);
          ht.Add("DataNomina", dtpDataNomina.SelectedDate.Value.ToShortDateString());
          ht.Add("DataFine", dtpDataFine.SelectedDate.Value.ToShortDateString());
          ht.Add("Note", txtNota.Text.Trim());
          if (rdbCollegioSindacale.IsChecked == false && rdbRevisoreSincoUnico.IsChecked == false)
          {
            ht.Add("Composizione", (int)(App.TipoISQCComposizione.Sconosciuto));
          }
          else
          {
            if (rdbCollegioSindacale.IsChecked == true)
            {
              ht.Add("Composizione", (int)(App.TipoISQCComposizione.CollegioSindacale));
            }
            if (rdbRevisoreSincoUnico.IsChecked == true)
            {
              ht.Add("Composizione", (int)(App.TipoISQCComposizione.SindacoUnico));
            }
          }
          //if (rdbAttivitaNomina.IsChecked == false && rdbAttivitaRiesame.IsChecked == false)
          //{
          //    ht.Add("Attivita", (int)(App.TipoISQCAttivita.Sconosciuto));
          //}
          //else
          {
            if (rdbAttivitaNomina.IsChecked == true || Riesame == false)
            {
              ht.Add("Attivita", (int)(App.TipoISQCAttivita.Nomina));
            }
            if (rdbAttivitaRiesame.IsChecked == true || Riesame == true)
            {
              ht.Add("Attivita", (int)(App.TipoISQCAttivita.Riesame));
            }
          }
          IDISQC = mf.SetISQC(ht, IDISQC, IDCliente);
          RegistrazioneEffettuata = true;
          if (TipologiaAttivita == App.TipoAttivitaScheda.New)
          {
            if (IDClienteImport != "-1")
                        {
                            cBusinessObjects.hide_workinprogress();
                            this.Close();
                        }
                           
             cBusinessObjects.SessioneIsNew = true;          
             TipologiaAttivita = App.TipoAttivitaScheda.Edit;
            mf.SetAnafraficaStato(Convert.ToInt32(IDCliente), App.TipoAnagraficaStato.Disponibile);
            ConfiguraMaschera();
            foreach (DictionaryEntry item in htClienti)
            {
              if (Convert.ToInt32(item.Value.ToString()) == IDCliente)
              {
                cmbClienti.SelectedIndex = Convert.ToInt32(item.Key.ToString());
              }
            }
            functionCmbClientiChanged(cmbClienti);
            cmbData.IsEnabled = true;
            foreach (DictionaryEntry item in htDate)
            {
              if (Convert.ToInt32(item.Value.ToString()) == IDISQC)
              {
                cmbData.SelectedIndex = Convert.ToInt32(item.Key.ToString());
              }
            }
            functionCmbDataChanged(cmbData);
                        cBusinessObjects.AddSessione("ISQC",cmbData.SelectedValue.ToString(), IDISQC, IDCliente);


                    }
          _DatiCambiati = false;
          break;
        case App.TipoAttivitaScheda.Delete:
          //richiesta conferma
          if (MessageBoxResult.No == u.ConfermaCancellazione())
            return;
          //cancellazione
          mf.DeleteISQC(IDISQC, IDCliente.ToString());
                    RegistrazioneEffettuata = true;
          break;
        case App.TipoAttivitaScheda.Export:
          break;
      }
      //apro tree appena creato
      if (oldTipo == App.TipoAttivitaScheda.New)
      {
        Accedi_Click(IDISQC.ToString(), false);
        //MessageBox.Show("apro tree appena creato");
      }
      //chiudo maschera
      if (TipologiaAttivita != App.TipoAttivitaScheda.Edit)
        base.Close();
    }

    //----------------------------------------------------------------------------+
    //                              ButtonApri_Click                              |
    //----------------------------------------------------------------------------+
    private void ButtonApri_Click(object sender, RoutedEventArgs e)
    {
      //controllo selezione clienti
      if (cmbClienti.SelectedIndex == -1)
      {
        MessageBox.Show("selezionare un cliente");
        return;
      }
      //dati modificati
      if (_DatiCambiati)
      {
        Utilities u = new Utilities();
        if (MessageBoxResult.No == u.AvvisoPerditaDati("Alcuni dati sono stati modificati, confermi apertura?"))
          return;
      }
      //disponibile: blocco cliente
      int IDCliente = Convert.ToInt32(htClienti[cmbClienti.SelectedIndex].ToString());
      MasterFile mf = MasterFile.Create();
      App.TipoAnagraficaStato anaStato = mf.GetAnafraficaStato(IDCliente);
      if (anaStato == App.TipoAnagraficaStato.Disponibile && TipologiaAttivita != App.TipoAttivitaScheda.View)
        mf.SetAnafraficaStato(Convert.ToInt32(IDCliente), App.TipoAnagraficaStato.InUso);
      //apre treee
      int IDISQC = App.MasterFile_NewID;
      try
      {
        IDISQC = Convert.ToInt32(htDate[cmbData.SelectedIndex].ToString());
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaISQC.ButtonApri_Click exception");
        string log = ex.Message;
      }
      if (IDISQC == -1)
      {
        MessageBox.Show("selezionare un ISQC");
      }
      else
      {
#if (false)
        string str = String.Format("cliente: {0}, IDISQC: {1}", IDCliente, IDISQC);
        MessageBox.Show(str);
#endif
        bool isSchedaReadOnly = TipologiaAttivita == App.TipoAttivitaScheda.View || anaStato == App.TipoAnagraficaStato.InUso;
        cBusinessObjects.VerificaSessione("ISQC",cmbData.SelectedValue.ToString(), IDISQC, IDCliente);
      
        Accedi_Click(IDISQC.ToString(), isSchedaReadOnly);
      }
    }

    //----------------------------------------------------------------------------+
    //                                   accedi                                   |
    //----------------------------------------------------------------------------+
    public void accedi(string ID, bool ReadOnly)
    {
      MasterFile mf = MasterFile.Create();
      Hashtable htSelected = mf.GetISQC(ID);
      if (htSelected.Count == 0) return;
      WindowWorkAreaTree wWorkArea = new WindowWorkAreaTree();
      //Prisc
      try
      {
        wWorkArea.Owner = this;
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaISQC.accedi exception");
        string log = ex.Message;
      }
      wWorkArea.SelectedTreeSource = App.AppDataDataFolder + "\\" + htSelected["File"].ToString();
      wWorkArea.SelectedDataSource = App.AppDataDataFolder + "\\" + htSelected["FileData"].ToString();
      if (htSelected["Sigillo"] != null && htSelected["Sigillo"].ToString() != "")
      {
        wWorkArea.ReadOnly = true;
      }
      else
      {
        wWorkArea.ReadOnly = ReadOnly;
      }
      wWorkArea.TipoAttivita = App.TipoAttivita.ISQC;
      wWorkArea.Cliente = (((Hashtable)(mf.GetAnagrafica(Convert.ToInt32(htSelected["Cliente"].ToString()))))["RagioneSociale"].ToString()) + " (C.F. " + (((Hashtable)(mf.GetAnagrafica(Convert.ToInt32(htSelected["Cliente"].ToString()))))["CodiceFiscale"].ToString()) + ")";
      wWorkArea.SessioneAlias = "";
      wWorkArea.SessioneFile = "";
      wWorkArea.SessioneSigillo = null;
      wWorkArea.SessioneSigilloData = null;
      wWorkArea.SessioneSigilloPassword = null;
      wWorkArea.IDTree = (Convert.ToInt32(App.TipoFile.ISQC)).ToString();
      wWorkArea.IDCliente = htSelected["Cliente"].ToString();
      wWorkArea.IDSessione = ID;
      foreach (Hashtable item in ((ArrayList)(mf.GetISQCs(htSelected["Cliente"].ToString()))))
      {
        wWorkArea.SessioneFile += ((wWorkArea.SessioneFile == "") ? "" : "|") + App.AppDataDataFolder + "\\" + item["FileData"].ToString();
        switch ((App.TipoISQCAttivita)(Convert.ToInt32(item["Attivita"].ToString())))
        {
          case App.TipoISQCAttivita.Nomina:
            wWorkArea.SessioneAliasAdditivo += ((wWorkArea.SessioneAliasAdditivo == "") ? "" : "|") + ((item["DataFine"] == null) ? "" : item["DataFine"].ToString());
            break;
          case App.TipoISQCAttivita.Riesame:
            wWorkArea.SessioneAliasAdditivo += ((wWorkArea.SessioneAliasAdditivo == "") ? "" : "|") + ((item["DataFine"] == null) ? "" : item["DataFine"].ToString());
            break;
          case App.TipoISQCAttivita.Sconosciuto:
          default:
            wWorkArea.SessioneAliasAdditivo += ((wWorkArea.SessioneAliasAdditivo == "") ? "" : "|") + ((item["DataFine"] == null) ? "" : item["DataFine"].ToString());
            break;
        }
        //wWorkArea.SessioneAlias += ((wWorkArea.SessioneAlias == "") ? "" : "|") + ((item["DataNomina"].ToString().Split('/')[2].Length == 2) ? item["DataNomina"].ToString().Split('/')[0] + "/" + item["DataNomina"].ToString().Split('/')[1] + "/20" + item["DataNomina"].ToString().Split('/')[2] : item["DataNomina"].ToString());
        wWorkArea.SessioneAlias += ((wWorkArea.SessioneAlias == "") ? "" : "|") + item["DataNomina"].ToString();
        wWorkArea.SessioneID += ((wWorkArea.SessioneID == "") ? "" : "|") + item["ID"].ToString();
        wWorkArea.SessioneSigillo += ((wWorkArea.SessioneSigillo == null) ? "" : "|") + ((item["Sigillo"] != null) ? item["Sigillo"].ToString() : "");
        wWorkArea.SessioneSigilloData += ((wWorkArea.SessioneSigilloData == null) ? "" : "|") + ((item["Sigillo_Data"] != null) ? item["Sigillo_Data"].ToString() : "");
        wWorkArea.SessioneSigilloPassword += ((wWorkArea.SessioneSigilloPassword == null) ? "" : "|") + ((item["Sigillo_Password"] != null) ? item["Sigillo_Password"].ToString() : "");
      }
      //aperto in sola lettura
      wWorkArea.ApertoInSolaLettura = TipologiaAttivita == App.TipoAttivitaScheda.View;
      wWorkArea.LoadTreeSource();
      Hide();
      wWorkArea.ShowDialog();
      if (TipologiaAttivita != App.TipoAttivitaScheda.View)
      {
        int IDCliente = Convert.ToInt32(htClienti[cmbClienti.SelectedIndex].ToString());
        mf.SetAnafraficaStato(Convert.ToInt32(IDCliente), App.TipoAnagraficaStato.Disponibile);
      }
      Close();
      
    }

    //----------------------------------------------------------------------------+
    //                                Accedi_Click                                |
    //----------------------------------------------------------------------------+
    private void Accedi_Click(string ID, bool ReadOnly)
    {
      try
      {
        if (noopenaftercreate) return;
        accedi(ID, ReadOnly);
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaISQC.Accedi_Click exception");
        string log = ex.Message;
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
    //                         GestoreEvento_DatiCambiati                         |
    //----------------------------------------------------------------------------+
    private void GestoreEvento_DatiCambiati(object sender, RoutedEventArgs e)
    {
      if (rdbAttivitaNomina.IsChecked == true)
      {
        _riesame = false;
      }
      if (rdbAttivitaRiesame.IsChecked == true)
      {
        _riesame = true;
      }
      if (_InCaricamento) return;
      _DatiCambiati = true;
    }

    //----------------------------------------------------------------------------+
    //                       GestoreEvento_ChiusuraFinestra                       |
    //----------------------------------------------------------------------------+
    private void GestoreEvento_ChiusuraFinestra(object sender, CancelEventArgs e)
    {
      if (IDClienteImport != "-1")
      {
        ;// MessageBox.Show("La Sessione viene adesso generata\r\nDovrà essere selezionata nella tendina Destinazione della finestra di Import\r\nScegliere le CdL da Importare e premere Importa\r\nPer accedere alla sessione appena importata bisognerà chiudere la Sessione in corso e aprire quella nuova.", "Attenzione");
        return;
      }
      //Configuro stato
      if (TipologiaAttivita != App.TipoAttivitaScheda.View && cmbClienti.SelectedIndex != -1)
      {
        string IDCliente = htClienti[cmbClienti.SelectedIndex].ToString();
        MasterFile mf = MasterFile.Create();
        mf.SetAnafraficaStato(Convert.ToInt32(IDCliente), App.TipoAnagraficaStato.Disponibile);
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
    //                         GestoreEvento_DataCambiata                         |
    //----------------------------------------------------------------------------+
    private void GestoreEvento_DataCambiata(object sender, RoutedEventArgs e)
    {
      if (cmbData.SelectedValue == null) return;

            if (dtpDataNomina.ToString().Substring(0, 10) == cmbData.SelectedValue.ToString().Substring(0, 10)) return;
     
      MasterFile mf = MasterFile.Create();
      Utilities u = new Utilities();
      int IDCliente = Convert.ToInt32(htClienti[cmbClienti.SelectedIndex].ToString());
      int IDISQC = Convert.ToInt32(htDate[cmbData.SelectedIndex].ToString());
      if (!u.ConvalidaDatiInterfaccia(dtpDataNomina, "Data mancante."))
        return;
      if (!u.ConvalidaDatiInterfaccia(dtpDataFine, "Data mancante."))
        return;
      //Controllo che questa data non sia già stata presa
      if (!mf.CheckDoppio_ISQC(IDISQC, IDCliente, dtpDataNomina.SelectedDate.Value.ToShortDateString()))
      {
        MessageBox.Show("Data già presente per questo cliente");
        dtpDataNomina.Text = cmbData.SelectedValue.ToString();
        return;
      }
      Hashtable ht = new Hashtable();
      ht = mf.GetISQC(IDISQC.ToString());
      ht["DataNomina"] = dtpDataNomina.SelectedDate.Value.ToShortDateString();
      ht["DataFine"] = dtpDataFine.SelectedDate.Value.ToShortDateString();
      //ht.Add("Cliente", IDCliente);
      //ht.Add("DataNomina", dtpDataNomina.SelectedDate.Value.ToShortDateString());
      //ht.Add("Note", txtNota.Text.Trim());
      //if (rdbCollegioSindacale.IsChecked == false && rdbRevisoreSincoUnico.IsChecked == false)
      //{
      //    ht.Add("Composizione", (int)(App.TipoISQCComposizione.Sconosciuto));
      //}
      //else
      //{
      //    if (rdbCollegioSindacale.IsChecked == true)
      //    {
      //        ht.Add("Composizione", (int)(App.TipoISQCComposizione.CollegioSindacale));
      //    }
      //    if (rdbRevisoreSincoUnico.IsChecked == true)
      //    {
      //        ht.Add("Composizione", (int)(App.TipoISQCComposizione.SindacoUnico));
      //    }
      //}
      ////if (rdbAttivitaNomina.IsChecked == false && rdbAttivitaRiesame.IsChecked == false)
      ////{
      ////    ht.Add("Attivita", (int)(App.TipoISQCAttivita.Sconosciuto));
      ////}
      ////else
      //{
      //    if (rdbAttivitaNomina.IsChecked == true || Riesame == false)
      //    {
      //        ht.Add("Attivita", (int)(App.TipoISQCAttivita.Nomina));
      //    }
      //    if (rdbAttivitaRiesame.IsChecked == true || Riesame == true)
      //    {
      //        ht.Add("Attivita", (int)(App.TipoISQCAttivita.Riesame));
      //    }
      //}
      IDISQC = mf.SetISQC(ht, IDISQC, IDCliente);
      //GestoreEvento_DatiCambiati(sender, e);
    }
  } //------------------------------- public partial class wSchedaISQC : Window
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
using System.Collections;
using System.ComponentModel;

namespace RevisoftApplication
{

    public partial class wSchedaISQC : Window
    {
		private App.TipoAttivitaScheda _tipologiaAttivita;
		private App.TipoAttivitaScheda oldTipologiaAttivita = App.TipoAttivitaScheda.View;
		private bool _riesame;
		private bool firsttime = true;

        public string IDISQC;

        private bool _InCaricamento;
        private bool _DatiCambiati;
        public bool RegistrazioneEffettuata;

        public bool noopenaftercreate = false;

		private bool _cmbInCaricamento = false;
		private int OldSelectedCmbClienti = -1;

        public string IDClienteImport = "-1";

		Hashtable htClienti = new Hashtable();
		Hashtable htDate = new Hashtable();

		public App.TipoAttivitaScheda TipologiaAttivita
		{
			get { return _tipologiaAttivita; }
			set
			{
				if (!firsttime && _tipologiaAttivita == value)
				{
					return;
				}

				firsttime = false;

				_tipologiaAttivita = value;
								
				MasterFile mf = MasterFile.Create();

				int index = 0;

				int selectedIndex = -1;
				if (cmbClienti.Items.Count != 0)
				{
					_cmbInCaricamento = true;
					selectedIndex = cmbClienti.SelectedIndex;
					cmbClienti.Items.Clear();
					htClienti.Clear();
				}

				List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>();

				foreach (Hashtable item in mf.GetAnagrafiche())
				{
                    if ( IDClienteImport == "-1" || IDClienteImport == item["ID"].ToString() )
                    {
                        if ( _tipologiaAttivita != App.TipoAttivitaScheda.New )
                        {
                            if ( mf.GetISQCs( item["ID"].ToString() ).Count == 0 )
                            {
                                continue;
                            }
                        }

                        string cliente = item["RagioneSociale"].ToString();
                        if ( IDClienteImport == "-1" )
                        {
                            switch ( ( (App.TipoAnagraficaStato)( Convert.ToInt32( item["Stato"].ToString() ) ) ) )
                            {
                                case App.TipoAnagraficaStato.InUso:
                                    cliente += " (In Uso)";
                                    break;
                                case App.TipoAnagraficaStato.Bloccato:
                                    cliente += " (Bloccato)";
                                    break;
                                case App.TipoAnagraficaStato.Esportato:
                                    cliente += " (Esportato)";
                                    break;
                                case App.TipoAnagraficaStato.Disponibile:
                                case App.TipoAnagraficaStato.Sconosciuto:
                                default:
                                    break;
                            }
                        }

                        myList.Add( new KeyValuePair<string, string>( item["ID"].ToString(), cliente ) );
                    }
				}

				myList.Sort
				(
					delegate(KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
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
				_cmbInCaricamento = false;
			}
		}

        public wSchedaISQC()
		{
			InitializeComponent();

			 //var
            _InCaricamento = true;
            _DatiCambiati = false;
            RegistrazioneEffettuata = false;
						
            //interfaccia 
            buttonComando.Visibility = System.Windows.Visibility.Hidden;
            cmbClienti.Focus();

           
        }

		public bool Riesame
		{
			get
			{
				return _riesame;
			}
			set
			{
				_riesame = value;
				
				if(value)
				{
					labelTitolo.Content = labelTitolo.Content.ToString().Split('(')[0] + "Riesame___Incarico";
					rdbAttivitaNomina.IsChecked = false;
					rdbAttivitaRiesame.IsChecked = true;
				}
				else
				{
					labelTitolo.Content = labelTitolo.Content.ToString().Split('(')[0] + "ISQC";
					rdbAttivitaNomina.IsChecked = true;
					rdbAttivitaRiesame.IsChecked = false;
				}
			}
		}

        public void ConfiguraMaschera()
        {
            //inibisco tutto i controlli
			dtpDataNomina.IsHitTestVisible = false;
            dtpDataFine.IsHitTestVisible = false;
            rdbAttivitaNomina.IsHitTestVisible = false;
			rdbAttivitaRiesame.IsHitTestVisible = false;
			rdbCollegioSindacale.IsHitTestVisible = false;
			rdbRevisoreSincoUnico.IsHitTestVisible = false;
			txtNota.IsReadOnly = true;

            //nascondo testo help - non + usato
            textBlockDescrizione.Text = "";
            textBlockDescrizione.Visibility = System.Windows.Visibility.Collapsed;

            switch (TipologiaAttivita)
            {
                case App.TipoAttivitaScheda.New:
                    labelTitolo.Content = "Nuovo ";
					Riesame = _riesame;
                    buttonComando.Content = "Crea";
                    GridComboData.Visibility = System.Windows.Visibility.Collapsed;
                    buttonComando.Visibility = System.Windows.Visibility.Visible;
                    buttonApri.Visibility = System.Windows.Visibility.Hidden;
                    //abilito controlli
                    dtpDataNomina.IsHitTestVisible = true;
                    dtpDataFine.IsHitTestVisible = true;
                    rdbAttivitaNomina.IsHitTestVisible = true;
                    rdbAttivitaRiesame.IsHitTestVisible = true;
                    rdbCollegioSindacale.IsHitTestVisible = true;
                    rdbRevisoreSincoUnico.IsHitTestVisible = true;
                    txtNota.IsReadOnly = false;			
                    break;
                case App.TipoAttivitaScheda.Edit:
					labelTitolo.Content = "Modifica ";
					Riesame = _riesame;
                    buttonComando.Content = "Salva";
                    buttonComando.Visibility = System.Windows.Visibility.Collapsed;
                    buttonApri.Visibility = System.Windows.Visibility.Visible;
                    //abilito controlli
                    dtpDataNomina.IsHitTestVisible = true;
                    dtpDataFine.IsHitTestVisible = true;
                    rdbAttivitaNomina.IsHitTestVisible = true;
                    rdbAttivitaRiesame.IsHitTestVisible = true;
                    rdbCollegioSindacale.IsHitTestVisible = true;
                    rdbRevisoreSincoUnico.IsHitTestVisible = true;
                    txtNota.IsReadOnly = false;			
                    break;
                case App.TipoAttivitaScheda.Delete:
					labelTitolo.Content = "Elimina ";
					Riesame = _riesame;
                    buttonComando.Content = "Elimina";
                    buttonComando.Visibility = System.Windows.Visibility.Visible;
                    buttonApri.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case App.TipoAttivitaScheda.Export:
					labelTitolo.Content = "Esporta ";
					Riesame = _riesame;
                    buttonComando.Content = "Esporta";
                    buttonComando.Visibility = System.Windows.Visibility.Visible;
                    buttonApri.Visibility = System.Windows.Visibility.Visible;
                    break;
                case App.TipoAttivitaScheda.View:
				default:
                    labelTitolo.Content = "Apri ISQC/Riesame Incarico in sola lettura";
                    //Riesame = _riesame;
					cmbData.Visibility = System.Windows.Visibility.Visible;
                    buttonApri.Visibility = System.Windows.Visibility.Visible;
                    buttonApri.Margin = buttonComando.Margin;
                    break;
            }
            _InCaricamento = false;

            MasterFile mf = MasterFile.Create();
            string IDCliente = mf.GetClienteFissato();
            foreach (DictionaryEntry item in htClienti)
            {
                if (item.Value.ToString() == IDCliente)
                {
                    cmbClienti.SelectedIndex = Convert.ToInt32(item.Key.ToString());
                    return;
                }
            }
        }

		private void cmbClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            //rilascio blocco su selezione precedente
			//if (OldSelectedCmbClienti != -1)
			//{
			//    MasterFile mf = MasterFile.Create();
			//    mf.SetAnafraficaStato(Convert.ToInt32(OldSelectedCmbClienti), App.TipoAnagraficaStato.Disponibile);
			//}

            //interfaccia
			functionCmbClientiChanged(((ComboBox)sender));

			ConfiguraMaschera();

            if (TipologiaAttivita == App.TipoAttivitaScheda.New)
                dtpDataNomina.Focus();
            else
                cmbData.Focus();
		}

		private void functionCmbClientiChanged(ComboBox cmb)
		{
			if (_cmbInCaricamento)
				return;

			if (oldTipologiaAttivita != App.TipoAttivitaScheda.View)
			{
				TipologiaAttivita = oldTipologiaAttivita;
			}

			_InCaricamento = true;
			cmbData.SelectedIndex = -1;
			dtpDataNomina.Text = "";
            dtpDataFine.Text = "";
            rdbAttivitaNomina.IsChecked = false;
			rdbAttivitaRiesame.IsChecked = false;
			rdbCollegioSindacale.IsChecked = false;
			rdbRevisoreSincoUnico.IsChecked = false;
			txtNota.Text = "";

			if (cmb.SelectedIndex != -1)
			{
				try
				{
					string IDCliente = htClienti[cmb.SelectedIndex].ToString();

					OldSelectedCmbClienti = Convert.ToInt32(IDCliente);

					MasterFile mf = MasterFile.Create();

					int index = 0;
					htDate.Clear();
					cmbData.Items.Clear();

					Hashtable attivita = new Hashtable();
					
					List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>();

					foreach (Hashtable item in mf.GetISQCs(IDCliente))
					{
						switch ((App.TipoISQCAttivita)(Convert.ToInt32(item["Attivita"].ToString())))
						{
							case App.TipoISQCAttivita.Nomina:
								attivita.Add(item["ID"].ToString(), "");
								break;
							case App.TipoISQCAttivita.Riesame:
								attivita.Add(item["ID"].ToString(), "");
								break;
							case App.TipoISQCAttivita.Sconosciuto:
							default:
								attivita.Add(item["ID"].ToString(), "");
								break;
						}

						myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), item["DataNomina"].ToString() + " - " + ((item["DataFine"] == null)? "" : item["DataFine"].ToString())));
					}

					myList.Sort
					(
						delegate(KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
						{
                            return Convert.ToDateTime( (nextPair.Value.ToString().Split('-'))[0].Trim()).CompareTo( Convert.ToDateTime( (firstPair.Value.ToString().Split('-'))[0].Trim()));
						}
					);

					foreach (KeyValuePair<string, string> item in myList)
					{
						cmbData.Items.Add(item.Value + attivita[item.Key].ToString());
						htDate.Add(index, item.Key);
						index++;
					}	

					//stato
                    if ( IDClienteImport == "-1" )
                    {
                        App.TipoAnagraficaStato anaStato = mf.GetAnafraficaStato( Convert.ToInt32( IDCliente ) );

                        //non disponibile: configuro interfaccia
                        if ( anaStato != App.TipoAnagraficaStato.Disponibile )
                        {
                            oldTipologiaAttivita = TipologiaAttivita;
                            TipologiaAttivita = App.TipoAttivitaScheda.View;
                        }
                    }   

					if (TipologiaAttivita != App.TipoAttivitaScheda.New)
					{
						cmbData.IsEnabled = true;
						dtpDataNomina.IsEnabled = false;
                        dtpDataFine.IsEnabled = false;
                        rdbAttivitaNomina.IsEnabled = false;
						rdbAttivitaRiesame.IsEnabled = false;
						rdbCollegioSindacale.IsEnabled = false;
						rdbRevisoreSincoUnico.IsEnabled = false;
						txtNota.IsEnabled = false;
					}
					else
					{
						cmbData.IsEnabled = false;
						dtpDataNomina.IsEnabled = true;
                        dtpDataFine.IsEnabled = true;
                        rdbAttivitaNomina.IsEnabled = true;
						rdbAttivitaRiesame.IsEnabled = true;
						rdbCollegioSindacale.IsEnabled = true;
						rdbRevisoreSincoUnico.IsEnabled = true;
						txtNota.IsEnabled = true;
					}
				}
				catch (Exception ex)
				{
					string log = ex.Message;
					cmbData.IsEnabled = false;
					dtpDataNomina.IsEnabled = false;
                    dtpDataFine.IsEnabled = false;
                    rdbAttivitaNomina.IsEnabled = false;
					rdbAttivitaRiesame.IsEnabled = false;
					rdbCollegioSindacale.IsEnabled = false;
					rdbRevisoreSincoUnico.IsEnabled = false;
					txtNota.IsEnabled = false;
				}
			}
		}

		private void cmbData_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			functionCmbDataChanged(((ComboBox)sender));
			ConfiguraMaschera();
		}

		private void functionCmbDataChanged(ComboBox cmb)
		{
			if (cmb.SelectedIndex != -1)
			{
				try
				{
                    _InCaricamento = true;

                    IDISQC = htDate[cmb.SelectedIndex].ToString();

					MasterFile mf = MasterFile.Create();

					Hashtable htISQC = new Hashtable();

					htISQC = mf.GetISQC(IDISQC);

					dtpDataNomina.IsEnabled = true;
                    dtpDataFine.IsEnabled = true;
                    rdbAttivitaNomina.IsEnabled = true;
					rdbAttivitaRiesame.IsEnabled = true;
					rdbCollegioSindacale.IsEnabled = true;
					rdbRevisoreSincoUnico.IsEnabled = true;
					txtNota.IsEnabled = true;

					dtpDataNomina.Text = htISQC["DataNomina"].ToString();
                    dtpDataFine.Text = ((htISQC["DataFine"] == null)?"": htISQC["DataFine"].ToString());
                    txtNota.Text = htISQC["Note"].ToString();

					switch ((App.TipoISQCComposizione)(Convert.ToInt32(htISQC["Composizione"].ToString())))
					{
						case App.TipoISQCComposizione.CollegioSindacale:
							rdbCollegioSindacale.IsChecked = true;
							rdbRevisoreSincoUnico.IsChecked = false;
							break;
						case App.TipoISQCComposizione.SindacoUnico:
							rdbCollegioSindacale.IsChecked = false;
							rdbRevisoreSincoUnico.IsChecked = true;
							break;
						case App.TipoISQCComposizione.Sconosciuto:
						default:
							break;
					}

					switch ((App.TipoISQCAttivita)(Convert.ToInt32(htISQC["Attivita"].ToString())))
					{
						case App.TipoISQCAttivita.Nomina:
							Riesame = false;
							rdbAttivitaNomina.IsChecked = true;
							rdbAttivitaRiesame.IsChecked = false;
							break;
						case App.TipoISQCAttivita.Riesame:
							Riesame = true;
							rdbAttivitaNomina.IsChecked = false;
							rdbAttivitaRiesame.IsChecked = true;
							break;
						case App.TipoISQCAttivita.Sconosciuto:
						default:
							break;
					}

					_InCaricamento = false;
				}
				catch (Exception ex)
				{
					string log = ex.Message;
				}
			}
		}

        private void buttonComando_Click(object sender, RoutedEventArgs e)
        {
            //controllo selezione clienti
            if (cmbClienti.SelectedIndex == -1)
            {
                MessageBox.Show("selezionare un cliente");
                return;
            }

            MasterFile mf = MasterFile.Create();
            Utilities u = new Utilities();

			int IDCliente = Convert.ToInt32(htClienti[cmbClienti.SelectedIndex].ToString());
			int IDISQC = App.MasterFile_NewID;

			try
			{
				IDISQC = Convert.ToInt32(htDate[cmbData.SelectedIndex].ToString());
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}

            if (TipologiaAttivita == App.TipoAttivitaScheda.Delete && IDISQC == -1)
            {
                MessageBox.Show("selezionare un ISQC");
                return;
            }

			App.TipoAttivitaScheda oldTipo = TipologiaAttivita;

            switch (TipologiaAttivita)
            {
                //Nuovo e salva
                case App.TipoAttivitaScheda.New:
                case App.TipoAttivitaScheda.Edit:
                    //convalida dati
                    //Campi Obbligatorio
					if (!u.ConvalidaDatiInterfaccia(dtpDataNomina, "Data mancante."))
						return;
                    if (!u.ConvalidaDatiInterfaccia(dtpDataFine, "Data mancante."))
                        return;
                    //if (!u.ConvalidaDatiInterfaccia(rdbCollegioSindacale, "Selezionare Collegio o Revisore."))
                    //    return;
                    //if (!u.ConvalidaDatiInterfaccia(rdbAttivitaNomina, "Selezionare tipologia Attività.") && Riesame == null)
                    //    return;

                    //Controllo che questa data non sia già stata presa
                    if (!mf.CheckDoppio_ISQC(IDISQC, IDCliente, dtpDataNomina.SelectedDate.Value.ToShortDateString()))
					{
						MessageBox.Show("Data già presente per questo cliente");
						return;
					}

                    //setto dati
                    Hashtable ht = new Hashtable();

					ht.Add("Cliente", IDCliente);
					ht.Add("DataNomina", dtpDataNomina.SelectedDate.Value.ToShortDateString());
                    ht.Add("DataFine", dtpDataFine.SelectedDate.Value.ToShortDateString());
                    ht.Add("Note", txtNota.Text.Trim());
					if (rdbCollegioSindacale.IsChecked == false && rdbRevisoreSincoUnico.IsChecked == false)
					{
						ht.Add("Composizione", (int)(App.TipoISQCComposizione.Sconosciuto));
					}
					else
					{
						if (rdbCollegioSindacale.IsChecked == true)
						{
							ht.Add("Composizione", (int)(App.TipoISQCComposizione.CollegioSindacale));
						}

						if (rdbRevisoreSincoUnico.IsChecked == true)
						{
							ht.Add("Composizione", (int)(App.TipoISQCComposizione.SindacoUnico));
						}

					}

					//if (rdbAttivitaNomina.IsChecked == false && rdbAttivitaRiesame.IsChecked == false)
					//{
					//    ht.Add("Attivita", (int)(App.TipoISQCAttivita.Sconosciuto));
					//}
					//else
					{
						if (rdbAttivitaNomina.IsChecked == true || Riesame == false)
						{
							ht.Add("Attivita", (int)(App.TipoISQCAttivita.Nomina));
						}

						if (rdbAttivitaRiesame.IsChecked == true || Riesame == true)
						{
							ht.Add("Attivita", (int)(App.TipoISQCAttivita.Riesame));
						}

					}

					IDISQC = mf.SetISQC(ht, IDISQC, IDCliente);

                    RegistrazioneEffettuata = true;

                    if (TipologiaAttivita == App.TipoAttivitaScheda.New)
                    {
                        if(IDClienteImport != "-1")
                        {
                            this.Close();
                        }

                        //Process wait - START
                        ProgressWindow pw = new ProgressWindow();

						TipologiaAttivita = App.TipoAttivitaScheda.Edit;
						mf.SetAnafraficaStato(Convert.ToInt32(IDCliente), App.TipoAnagraficaStato.Disponibile);

                        ConfiguraMaschera();

						foreach (DictionaryEntry item in htClienti)
						{
							if (Convert.ToInt32(item.Value.ToString()) == IDCliente)
							{
								cmbClienti.SelectedIndex = Convert.ToInt32(item.Key.ToString());
							}
						}

						functionCmbClientiChanged(cmbClienti);

						cmbData.IsEnabled = true;

						foreach (DictionaryEntry item in htDate)
						{
							if (Convert.ToInt32(item.Value.ToString()) == IDISQC)
							{
								cmbData.SelectedIndex = Convert.ToInt32(item.Key.ToString());
							}
						}

						functionCmbDataChanged(cmbData);

                        //Process wait - STOP
                        pw.Close();
                    }

					_DatiCambiati = false;
                    break;
                case App.TipoAttivitaScheda.Delete:
                    //richiesta conferma
                    if (MessageBoxResult.No == u.ConfermaCancellazione())
                        return;
                    //cancellazione
					mf.DeleteISQC(IDISQC);
					RegistrazioneEffettuata = true;
                    break;
                case App.TipoAttivitaScheda.Export:
                    break;
            }

            //apro tree appena creato
            if (oldTipo == App.TipoAttivitaScheda.New)
            {
				Accedi_Click(IDISQC.ToString(), false);
                //MessageBox.Show("apro tree appena creato");
            }

            //chiudo maschera
            if (TipologiaAttivita != App.TipoAttivitaScheda.Edit)
                base.Close();
        }
		        
		private void ButtonApri_Click(object sender, RoutedEventArgs e)
		{

            //controllo selezione clienti
            if (cmbClienti.SelectedIndex == -1)
            {
                MessageBox.Show("selezionare un cliente");
                return;
            }


            //dati modificati
            if (_DatiCambiati)
            {
                Utilities u = new Utilities();
                if (MessageBoxResult.No == u.AvvisoPerditaDati("Alcuni dati sono stati modificati, confermi apertura?"))
                    return;
            }

			//disponibile: blocco cliente
			int IDCliente = Convert.ToInt32(htClienti[cmbClienti.SelectedIndex].ToString());
			MasterFile mf = MasterFile.Create();
			App.TipoAnagraficaStato anaStato = mf.GetAnafraficaStato(IDCliente);

            if (anaStato == App.TipoAnagraficaStato.Disponibile && TipologiaAttivita != App.TipoAttivitaScheda.View)
                mf.SetAnafraficaStato(Convert.ToInt32(IDCliente), App.TipoAnagraficaStato.InUso);

            //apre treee
			int IDISQC = App.MasterFile_NewID;

			try
			{
                IDISQC = Convert.ToInt32( htDate[cmbData.SelectedIndex].ToString() );
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}

            if ( IDISQC == -1 )
			{
				MessageBox.Show("selezionare un ISQC");
			}
			else
			{
#if (false)
                string str = String.Format("cliente: {0}, IDISQC: {1}", IDCliente, IDISQC);
                MessageBox.Show(str);
#endif
                bool isSchedaReadOnly = TipologiaAttivita == App.TipoAttivitaScheda.View || anaStato == App.TipoAnagraficaStato.InUso;
                Accedi_Click( IDISQC.ToString(), isSchedaReadOnly );
			}
		}

        public void accedi( string ID, bool ReadOnly )
        {
            MasterFile mf = MasterFile.Create();

            Hashtable htSelected = mf.GetISQC( ID );

            if ( htSelected.Count == 0 )
            {
                return;
            }

            WindowWorkAreaTree wWorkArea = new WindowWorkAreaTree();
            //Prisc
            try
            {
                wWorkArea.Owner = this;
            }
            catch (Exception ex)
            {
                string log = ex.Message;
            }

            wWorkArea.SelectedTreeSource = App.AppDataDataFolder + "\\" + htSelected["File"].ToString();
            wWorkArea.SelectedDataSource = App.AppDataDataFolder + "\\" + htSelected["FileData"].ToString();
            if ( htSelected["Sigillo"] != null && htSelected["Sigillo"].ToString() != "" )
            {
                wWorkArea.ReadOnly = true;
            }
            else
            {
                wWorkArea.ReadOnly = ReadOnly;
            }
            wWorkArea.TipoAttivita = App.TipoAttivita.ISQC;
            wWorkArea.Cliente = ( ( (Hashtable)( mf.GetAnagrafica( Convert.ToInt32( htSelected["Cliente"].ToString() ) ) ) )["RagioneSociale"].ToString() ) + " (C.F. " + ( ( (Hashtable)( mf.GetAnagrafica( Convert.ToInt32( htSelected["Cliente"].ToString() ) ) ) )["CodiceFiscale"].ToString() ) + ")";
            wWorkArea.SessioneAlias = "";
            wWorkArea.SessioneFile = "";
            wWorkArea.SessioneSigillo = null;
            wWorkArea.SessioneSigilloData = null;
            wWorkArea.SessioneSigilloPassword = null;

            wWorkArea.IDTree = ( Convert.ToInt32( App.TipoFile.ISQC ) ).ToString();
            wWorkArea.IDCliente = htSelected["Cliente"].ToString();
            wWorkArea.IDSessione = ID;

            foreach ( Hashtable item in ( (ArrayList)( mf.GetISQCs( htSelected["Cliente"].ToString() ) ) ) )
            {
                wWorkArea.SessioneFile += ( ( wWorkArea.SessioneFile == "" ) ? "" : "|" ) + App.AppDataDataFolder + "\\" + item["FileData"].ToString();

                switch ( (App.TipoISQCAttivita)( Convert.ToInt32( item["Attivita"].ToString() ) ) )
                {
                    case App.TipoISQCAttivita.Nomina:
                        wWorkArea.SessioneAliasAdditivo += ( ( wWorkArea.SessioneAliasAdditivo == "" ) ? "" : "|" ) + ((item["DataFine"] == null)? "" : item["DataFine"].ToString());
                        break;
                    case App.TipoISQCAttivita.Riesame:
                        wWorkArea.SessioneAliasAdditivo += ( ( wWorkArea.SessioneAliasAdditivo == "" ) ? "" : "|" ) + ((item["DataFine"] == null) ? "" : item["DataFine"].ToString());
                        break;
                    case App.TipoISQCAttivita.Sconosciuto:
                    default:
                        wWorkArea.SessioneAliasAdditivo += ( ( wWorkArea.SessioneAliasAdditivo == "" ) ? "" : "|" ) + ((item["DataFine"] == null) ? "" : item["DataFine"].ToString());
                        break;
                }

                //wWorkArea.SessioneAlias += ( ( wWorkArea.SessioneAlias == "" ) ? "" : "|" ) + ( ( item["DataNomina"].ToString().Split( '/' )[2].Length == 2 ) ? item["DataNomina"].ToString().Split( '/' )[0] + "/" + item["DataNomina"].ToString().Split( '/' )[1] + "/20" + item["DataNomina"].ToString().Split( '/' )[2] : item["DataNomina"].ToString() );
                
                wWorkArea.SessioneAlias += ( ( wWorkArea.SessioneAlias == "" ) ? "" : "|" ) + item["DataNomina"].ToString();
                wWorkArea.SessioneID += ( ( wWorkArea.SessioneID == "" ) ? "" : "|" ) + item["ID"].ToString();
                wWorkArea.SessioneSigillo += ( ( wWorkArea.SessioneSigillo == null ) ? "" : "|" ) + ( ( item["Sigillo"] != null ) ? item["Sigillo"].ToString() : "" );
                wWorkArea.SessioneSigilloData += ( ( wWorkArea.SessioneSigilloData == null ) ? "" : "|" ) + ( ( item["Sigillo_Data"] != null ) ? item["Sigillo_Data"].ToString() : "" );
                wWorkArea.SessioneSigilloPassword += ( ( wWorkArea.SessioneSigilloPassword == null ) ? "" : "|" ) + ( ( item["Sigillo_Password"] != null ) ? item["Sigillo_Password"].ToString() : "" );
            }

            //aperto in sola lettura
            wWorkArea.ApertoInSolaLettura = TipologiaAttivita == App.TipoAttivitaScheda.View;

            wWorkArea.LoadTreeSource();

            Hide();
            wWorkArea.ShowDialog();
            
            if ( TipologiaAttivita != App.TipoAttivitaScheda.View )
            {
                int IDCliente = Convert.ToInt32( htClienti[cmbClienti.SelectedIndex].ToString() );
                mf.SetAnafraficaStato( Convert.ToInt32( IDCliente ), App.TipoAnagraficaStato.Disponibile );
            }

            Close();
        }

		private void Accedi_Click(string ID, bool ReadOnly)
		{
			try
			{
                if(noopenaftercreate)
                {
                    return;
                }

                accedi( ID, ReadOnly );
				
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}
		}

        private void buttonChiudi_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void GestoreEvento_DatiCambiati(object sender, RoutedEventArgs e)
        {
			if (rdbAttivitaNomina.IsChecked == true)
			{
				_riesame = false;
			}

			if (rdbAttivitaRiesame.IsChecked == true)
			{
				_riesame = true;
			}

            if (_InCaricamento)
                return;
            _DatiCambiati = true;
        }

        private void GestoreEvento_ChiusuraFinestra(object sender, CancelEventArgs e)
        { 
            if ( IDClienteImport != "-1" )
            {
                ;// MessageBox.Show( "La Sessione viene adesso generata\r\nDovrà essere selezionata nella tendina Destinazione della finestra di Import\r\nScegliere le CdL da Importare e premere Importa\r\nPer accedere alla sessione appena importata bisognerà chiudere la Sessione in corso e aprire quella nuova.", "Attenzione" );
                return;
            }

            //Configuro stato
            if (TipologiaAttivita != App.TipoAttivitaScheda.View && cmbClienti.SelectedIndex != -1)
            {
                string IDCliente = htClienti[cmbClienti.SelectedIndex].ToString();
                MasterFile mf = MasterFile.Create();
                mf.SetAnafraficaStato(Convert.ToInt32(IDCliente), App.TipoAnagraficaStato.Disponibile);
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

        private void GestoreEvento_DataCambiata( object sender, RoutedEventArgs e )
        {
            if ( cmbData.SelectedValue == null )
            {
                return;
            }

            try
            {
                if (Convert.ToDateTime(dtpDataNomina.Text) == Convert.ToDateTime(cmbData.SelectedValue.ToString().Split('-')[0].Trim()))
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                string log = ex.Message;
                return;
            }
            

            MasterFile mf = MasterFile.Create();
            Utilities u = new Utilities();

            int IDCliente = Convert.ToInt32( htClienti[cmbClienti.SelectedIndex].ToString() );
            int IDISQC = Convert.ToInt32( htDate[cmbData.SelectedIndex].ToString() );

            if ( !u.ConvalidaDatiInterfaccia( dtpDataNomina, "Data mancante." ) )
                return;

            if (!u.ConvalidaDatiInterfaccia(dtpDataFine, "Data mancante."))
                return;

            //Controllo che questa data non sia già stata presa
            if ( !mf.CheckDoppio_ISQC( IDISQC, IDCliente, dtpDataNomina.SelectedDate.Value.ToShortDateString() ) )
            {
                MessageBox.Show( "Data già presente per questo cliente" );
                dtpDataNomina.Text = cmbData.SelectedValue.ToString();
                return;
            }

            Hashtable ht = new Hashtable();
            ht = mf.GetISQC( IDISQC.ToString() );
            ht["DataNomina"] = dtpDataNomina.SelectedDate.Value.ToShortDateString();
            ht["DataFine"] = dtpDataFine.SelectedDate.Value.ToShortDateString();

            //ht.Add( "Cliente", IDCliente );
            //ht.Add( "DataNomina", dtpDataNomina.SelectedDate.Value.ToShortDateString() );
            //ht.Add( "Note", txtNota.Text.Trim() );
            //if ( rdbCollegioSindacale.IsChecked == false && rdbRevisoreSincoUnico.IsChecked == false )
            //{
            //    ht.Add( "Composizione", (int)( App.TipoISQCComposizione.Sconosciuto ) );
            //}
            //else
            //{
            //    if ( rdbCollegioSindacale.IsChecked == true )
            //    {
            //        ht.Add( "Composizione", (int)( App.TipoISQCComposizione.CollegioSindacale ) );
            //    }

            //    if ( rdbRevisoreSincoUnico.IsChecked == true )
            //    {
            //        ht.Add( "Composizione", (int)( App.TipoISQCComposizione.SindacoUnico ) );
            //    }

            //}

            ////if (rdbAttivitaNomina.IsChecked == false && rdbAttivitaRiesame.IsChecked == false)
            ////{
            ////    ht.Add("Attivita", (int)(App.TipoISQCAttivita.Sconosciuto));
            ////}
            ////else
            //{
            //    if ( rdbAttivitaNomina.IsChecked == true || Riesame == false )
            //    {
            //        ht.Add( "Attivita", (int)( App.TipoISQCAttivita.Nomina ) );
            //    }

            //    if ( rdbAttivitaRiesame.IsChecked == true || Riesame == true )
            //    {
            //        ht.Add( "Attivita", (int)( App.TipoISQCAttivita.Riesame ) );
            //    }

            //}

            IDISQC = mf.SetISQC( ht, IDISQC, IDCliente );

            //GestoreEvento_DatiCambiati( sender, e );
        }
    }
}
*/
