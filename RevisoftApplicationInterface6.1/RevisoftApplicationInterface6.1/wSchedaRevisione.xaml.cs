//----------------------------------------------------------------------------+
//                          wSchedaRevisione.xaml.cs                          |
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
  public partial class wSchedaRevisione : Window
  {
    private App.TipoAttivitaScheda _tipologiaAttivita;
    private App.TipoAttivitaScheda oldTipologiaAttivita = App.TipoAttivitaScheda.View;

    private bool _InCaricamento;
    private bool _DatiCambiati;

    private bool _cmbInCaricamento = false;
    public bool RegistrazioneEffettuata;

    public string IDRevisione;

    private bool firsttime = true;


    public string IDClienteImport = "-1";

    private int OldSelectedCmbClienti = -1;

    Hashtable htClienti = new Hashtable();
    Hashtable htDate = new Hashtable();

    Hashtable htSelectedDate = new Hashtable();

    public bool noopenaftercreate = false;

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
          if (IDClienteImport == "-1" || IDClienteImport == item["ID"].ToString())
          {
            if (_tipologiaAttivita != App.TipoAttivitaScheda.New)
            {
              if (mf.GetRevisioni(item["ID"].ToString()).Count == 0)
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
                                //     cliente += " (In Uso)";
                                //     break;
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
    //                              wSchedaRevisione                              |
    //----------------------------------------------------------------------------+
    public wSchedaRevisione()
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

    //----------------------------------------------------------------------------+
    //                             ConfiguraMaschera                              |
    //----------------------------------------------------------------------------+
    public void ConfiguraMaschera()
    {
      //inibisco tutto i controlli
      dtpDataNomina.IsEnabled = false;
      txtNota.IsReadOnly = true;
      //nascondo testo help - non + usato
      textBlockDescrizione.Text = "";
      textBlockDescrizione.Visibility = System.Windows.Visibility.Collapsed;
      switch (TipologiaAttivita)
      {
        case App.TipoAttivitaScheda.New:
          labelTitolo.Content = "Nuova Sessione";
          buttonComando.Content = "Crea";
          GridComboData.Visibility = System.Windows.Visibility.Collapsed;
          buttonComando.Visibility = System.Windows.Visibility.Visible;
          buttonApri.Visibility = System.Windows.Visibility.Hidden;
          //abilito controlli
          dtpDataNomina.IsEnabled = true;
          txtNota.IsReadOnly = false;
          break;
        case App.TipoAttivitaScheda.Edit:
          labelTitolo.Content = "Modifica Sessione";
          buttonComando.Content = "Salva";
          buttonComando.Visibility = System.Windows.Visibility.Collapsed;
          //abilito controlli
          dtpDataNomina.IsEnabled = true;
          txtNota.IsReadOnly = false;
          break;
        case App.TipoAttivitaScheda.Delete:
          labelTitolo.Content = "Elimina Sessione";
          buttonComando.Content = "Elimina";
          buttonComando.Visibility = System.Windows.Visibility.Visible;
          buttonApri.Visibility = System.Windows.Visibility.Collapsed;
          break;
        case App.TipoAttivitaScheda.Export:
          labelTitolo.Content = "Esporta Sessione";
          buttonComando.Content = "Esporta";
          buttonComando.Visibility = System.Windows.Visibility.Visible;
          break;
        case App.TipoAttivitaScheda.View:
        default:
          labelTitolo.Content = "Apri Sessione in sola lettura";
          cmbData.Visibility = System.Windows.Visibility.Visible;
          buttonApri.Visibility = System.Windows.Visibility.Visible;
          buttonApri.Margin = buttonComando.Margin;
          break;
      }
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
    //                           ConvertDataToEsercizio                           |
    //----------------------------------------------------------------------------+
    private string ConvertDataToEsercizio(string data)
    {
      string returnvalue = "";
      int IDCliente = Convert.ToInt32(htClienti[cmbClienti.SelectedIndex].ToString());
      MasterFile mf = MasterFile.Create();
      Hashtable clientetmp = mf.GetAnagrafica(IDCliente);
      switch ((App.TipoAnagraficaEsercizio)(Convert.ToInt32(clientetmp["Esercizio"].ToString())))
      {
        case App.TipoAnagraficaEsercizio.ACavallo:
          returnvalue = Convert.ToDateTime(data).Year.ToString() + " - " + (Convert.ToDateTime(data).Year + 1).ToString();
          break;
        case App.TipoAnagraficaEsercizio.AnnoSolare:
        case App.TipoAnagraficaEsercizio.Sconosciuto:
        default:
          returnvalue = Convert.ToDateTime(data).Year.ToString();
          break;
      }
      return returnvalue;
    }

    //----------------------------------------------------------------------------+
    //                           ConvertDataToEsercizio                           |
    //----------------------------------------------------------------------------+
    private string ConvertDataToEsercizio(string data, Hashtable ht)
    {
      string returnvalue = "";
      if (ht.Contains("Intermedio") && ht.Contains("EsercizioDal") && ht.Contains("EsercizioAl"))
      {
        returnvalue = "dal " + ht["EsercizioDal"].ToString() + " al " + ht["EsercizioAl"].ToString();
      }
      else
      {
        switch ((App.TipoAnagraficaEsercizio)(Convert.ToInt32(ht["Esercizio"].ToString())))
        {
          case App.TipoAnagraficaEsercizio.ACavallo:
            returnvalue = Convert.ToDateTime(data).Year.ToString() + " - " + (Convert.ToDateTime(data).Year + 1).ToString();
            break;
          case App.TipoAnagraficaEsercizio.AnnoSolare:
          case App.TipoAnagraficaEsercizio.Sconosciuto:
          default:
            returnvalue = Convert.ToDateTime(data).Year.ToString();
            break;
        }
      }
      return returnvalue;
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
      dtpDataNomina.SelectedIndex = -1;
      txtNota.Text = "";
      imageStato.Visibility = System.Windows.Visibility.Hidden;
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
          List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>();
          foreach (Hashtable item in mf.GetRevisioni(IDCliente))
          {
            myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), ConvertDataToEsercizio(item["Data"].ToString(), item)));
          }
          myList.Sort
          (
            delegate (KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
            {
              try
              {
                return Convert.ToInt32(nextPair.Value).CompareTo(Convert.ToInt32(firstPair.Value));
              }
              catch (Exception ex)
              {
                cBusinessObjects.logger.Error(ex, "wSchedaRevisione.functionCmbClientiChanged1 exception");
                string log = ex.Message;
                return 1;
              }
            }
          );
          foreach (KeyValuePair<string, string> item in myList)
          {
            cmbData.Items.Add(item.Value);
            htDate.Add(index, item.Key);
            index++;
          }
          index = 0;
          htSelectedDate.Clear();
          dtpDataNomina.Items.Clear();
          for (int i = 2009; i <= (DateTime.Now.Year + 1); i++)
          {
            Hashtable clientetmp = mf.GetAnagrafica(Convert.ToInt32(IDCliente));
            string datatmp = "";
            switch ((App.TipoAnagraficaEsercizio)(Convert.ToInt32(clientetmp["Esercizio"].ToString())))
            {
              case App.TipoAnagraficaEsercizio.ACavallo:
                datatmp = "31/12/" + i.ToString();
                break;
              case App.TipoAnagraficaEsercizio.AnnoSolare:
              case App.TipoAnagraficaEsercizio.Sconosciuto:
              default:
                datatmp = "01/01/" + i.ToString();
                break;
            }
            if (!cmbData.Items.Contains(ConvertDataToEsercizio(datatmp)))
            {
              dtpDataNomina.Items.Add(ConvertDataToEsercizio(datatmp));
              htSelectedDate.Add(index, datatmp);
              index++;
            }
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
            label2.Visibility = System.Windows.Visibility.Collapsed;
            dtpDataNomina.Visibility = System.Windows.Visibility.Collapsed;
            label3.Visibility = System.Windows.Visibility.Visible;
            cmbData.Visibility = System.Windows.Visibility.Visible;
            txtNota.IsEnabled = false;
          }
          else
          {
            cmbData.IsEnabled = false;
            dtpDataNomina.IsEnabled = true;
            label2.Visibility = System.Windows.Visibility.Visible;
            dtpDataNomina.Visibility = System.Windows.Visibility.Visible;
            label3.Visibility = System.Windows.Visibility.Collapsed;
            cmbData.Visibility = System.Windows.Visibility.Collapsed;
            txtNota.IsEnabled = true;
          }
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wSchedaRevisione.functionCmbClientiChanged2 exception");
          string log = ex.Message;
          cmbData.IsEnabled = false;
          dtpDataNomina.IsEnabled = false;
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
          IDRevisione = htDate[cmb.SelectedIndex].ToString();
          MasterFile mf = MasterFile.Create();
          Hashtable htRevisione = new Hashtable();
          htRevisione = mf.GetRevisione(IDRevisione);
          dtpDataNomina.IsEnabled = true;
          txtNota.IsEnabled = true;
          foreach (DictionaryEntry item in htSelectedDate)
          {
            if (item.Value.ToString() == ConvertDataToEsercizio(htRevisione["Data"].ToString(), htRevisione))
            {
              dtpDataNomina.SelectedIndex = Convert.ToInt32(item.Key.ToString());
            }
          }
          txtNota.Text = htRevisione["Note"].ToString();
          _InCaricamento = false;
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wSchedaRevisione.functionCmbDataChanged exception");
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
      int IDRevisione = App.MasterFile_NewID;
      try
      {
        IDRevisione = Convert.ToInt32(htDate[cmbData.SelectedIndex].ToString());
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaRevisione.buttonComando_Click exception");
        string log = ex.Message;
      }
      if (TipologiaAttivita == App.TipoAttivitaScheda.Delete && IDRevisione == -1)
      {
        MessageBox.Show("selezionare una sessione");
        return;
      }
      App.TipoAttivitaScheda oldTipo = TipologiaAttivita;
      switch (TipologiaAttivita)
      {
        //Nuovo e salva
        case App.TipoAttivitaScheda.New:
        case App.TipoAttivitaScheda.Edit:
          //Campi Obbligatorio
          if (dtpDataNomina.SelectedIndex == -1)// !u.ConvalidaDatiInterfaccia(dtpDataNomina, "Data mancante."))
          {
            MessageBox.Show("Selezionare un Esercizio.");
            return;
          }
          //Controllo che questa data non sia già stata presa
          if (!mf.CheckDoppio_Revisione(IDRevisione, IDCliente, htSelectedDate[dtpDataNomina.SelectedIndex].ToString()))
          {
            MessageBox.Show("Data già presente per questo cliente");
            return;
          }
                    //setto dati
                    tabControl1.IsEnabled = false;
            gridButtons.IsEnabled = false;
            loading.Visibility = Visibility;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,new Action(delegate { }));
                    //cBusinessObjects.show_workinprogress("Prima creazione dell'albero in corso...");
                    Hashtable ht = new Hashtable();
          ht.Add("Cliente", IDCliente);
          ht.Add("Data", htSelectedDate[dtpDataNomina.SelectedIndex].ToString());
          ht.Add("Note", txtNota.Text.Trim());
          IDRevisione = mf.SetRevisione(ht, IDRevisione, IDCliente);
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
              if (Convert.ToInt32(item.Value.ToString()) == IDRevisione)
              {
                cmbData.SelectedIndex = Convert.ToInt32(item.Key.ToString());
              }
            }
            functionCmbDataChanged(cmbData);
                cBusinessObjects.AddSessione("Revisione",cmbData.SelectedValue.ToString(), IDRevisione, IDCliente);

            }
          _DatiCambiati = false;
          break;
        case App.TipoAttivitaScheda.Delete:
          //richiesta conferma
          if (MessageBoxResult.No == u.ConfermaCancellazione()) return;
          //cancellazione
          mf.DeleteRevisione(IDRevisione, IDCliente.ToString());
                   
          RegistrazioneEffettuata = true;
          base.Close();
          break;
        case App.TipoAttivitaScheda.Export:
          break;
      }
      //apro tree appena creato
      if (oldTipo == App.TipoAttivitaScheda.New)
      {
        //MessageBox.Show("apro tree appena creato");
        Accedi_Click(IDRevisione.ToString(), false);
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
      int IDVerifica = App.MasterFile_NewID;
      try
      {
        IDVerifica = Convert.ToInt32(htDate[cmbData.SelectedIndex].ToString());
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaRevisione.ButtonApri_Click exception");
        string log = ex.Message;
      }
      if (IDVerifica == -1)
      {
        MessageBox.Show("selezionare una sessione");
      }
      else
      {
#if (false)
        string str = String.Format("cliente: {0}, revisione: {1}", IDCliente, IDVerifica);
        MessageBox.Show(str);
#endif
        bool isSchedaReadOnly = TipologiaAttivita == App.TipoAttivitaScheda.View || anaStato == App.TipoAnagraficaStato.InUso;
       cBusinessObjects.VerificaSessione("Revisione",cmbData.SelectedValue.ToString(), IDVerifica, IDCliente);
     
        Accedi_Click(IDVerifica.ToString(), isSchedaReadOnly);
      }
    }

    //----------------------------------------------------------------------------+
    //                                   accedi                                   |
    //----------------------------------------------------------------------------+
    public void accedi(string ID, bool ReadOnly)
    {
      MasterFile mf = MasterFile.Create();
      Hashtable htSelected = mf.GetRevisione(ID);
      if (htSelected.Count == 0) return;
      WindowWorkAreaTree wWorkArea = new WindowWorkAreaTree();
      //Prisc
      try
      {
        wWorkArea.Owner = this;
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaRevisione.accedi exception");
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
      wWorkArea.TipoAttivita = App.TipoAttivita.Revisione;
      wWorkArea.Cliente = (((Hashtable)(mf.GetAnagrafica(
        Convert.ToInt32(htSelected["Cliente"].ToString()))))
        ["RagioneSociale"].ToString()) + " (C.F. "
        + (((Hashtable)(mf.GetAnagrafica(Convert.ToInt32(htSelected["Cliente"].ToString()))))
        ["CodiceFiscale"].ToString()) + ")";
      wWorkArea.SessioneAlias = "";
      wWorkArea.SessioneFile = "";
      wWorkArea.SessioneSigillo = null;
      wWorkArea.SessioneSigilloData = null;
      wWorkArea.SessioneSigilloPassword = null;
      wWorkArea.IDTree = (Convert.ToInt32(App.TipoFile.Revisione)).ToString();
      wWorkArea.IDCliente = htSelected["Cliente"].ToString();
      wWorkArea.IDSessione = ID;
      foreach (Hashtable item in ((ArrayList)(mf.GetRevisioni(htSelected["Cliente"].ToString()))))
      {
        wWorkArea.SessioneFile += ((wWorkArea.SessioneFile == "") ? "" : "|") + App.AppDataDataFolder + "\\" + item["FileData"].ToString();
        wWorkArea.SessioneAlias += ((wWorkArea.SessioneAlias == "") ? "" : "|") + item["Data"].ToString();// ConvertDataToEsercizio(item["Data"].ToString());
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
        cBusinessObjects.logger.Error(ex, "wSchedaRevisione.Accedi_Click exception");
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
  } //-------------------------- public partial class wSchedaRevisione : Window
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
	public partial class wSchedaRevisione : Window
	{
		private App.TipoAttivitaScheda _tipologiaAttivita;
		private App.TipoAttivitaScheda oldTipologiaAttivita = App.TipoAttivitaScheda.View;

		private bool _InCaricamento;
		private bool _DatiCambiati;

		private bool _cmbInCaricamento = false;
        public bool RegistrazioneEffettuata;

        public string IDRevisione;

		private bool firsttime = true;


        public string IDClienteImport = "-1";

		private int OldSelectedCmbClienti = -1;

		Hashtable htClienti = new Hashtable();
		Hashtable htDate = new Hashtable();

        Hashtable htSelectedDate = new Hashtable();

        public bool noopenaftercreate = false;

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
                            if ( mf.GetRevisioni( item["ID"].ToString() ).Count == 0 )
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

		public wSchedaRevisione()
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

		public void ConfiguraMaschera()
		{
            //inibisco tutto i controlli
            dtpDataNomina.IsEnabled = false;
            txtNota.IsReadOnly = true;

            //nascondo testo help - non + usato
            textBlockDescrizione.Text = "";
            textBlockDescrizione.Visibility = System.Windows.Visibility.Collapsed;

			switch (TipologiaAttivita)
			{
				case App.TipoAttivitaScheda.New:
					labelTitolo.Content = "Nuova Sessione";
					buttonComando.Content = "Crea";
                    GridComboData.Visibility = System.Windows.Visibility.Collapsed;
					buttonComando.Visibility = System.Windows.Visibility.Visible;
                    buttonApri.Visibility = System.Windows.Visibility.Hidden;
                    //abilito controlli
                    dtpDataNomina.IsEnabled = true;
                    txtNota.IsReadOnly = false;                    
					break;
				case App.TipoAttivitaScheda.Edit:
					labelTitolo.Content = "Modifica Sessione";
					buttonComando.Content = "Salva";
					buttonComando.Visibility = System.Windows.Visibility.Collapsed;
                    //abilito controlli
					dtpDataNomina.IsEnabled = true;
                    txtNota.IsReadOnly = false; 
					break;
				case App.TipoAttivitaScheda.Delete:
					labelTitolo.Content = "Elimina Sessione";
					buttonComando.Content = "Elimina";
					buttonComando.Visibility = System.Windows.Visibility.Visible;
                    buttonApri.Visibility = System.Windows.Visibility.Collapsed;
					break;
				case App.TipoAttivitaScheda.Export:
					labelTitolo.Content = "Esporta Sessione";
					buttonComando.Content = "Esporta";
					buttonComando.Visibility = System.Windows.Visibility.Visible;
					break;
				case App.TipoAttivitaScheda.View:
				default:
                    labelTitolo.Content = "Apri Sessione in sola lettura";
					cmbData.Visibility = System.Windows.Visibility.Visible;
                    buttonApri.Visibility = System.Windows.Visibility.Visible;
                    buttonApri.Margin = buttonComando.Margin;
                    break;
			}

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

		private string ConvertDataToEsercizio(string data)
		{
			string returnvalue = "";

			int IDCliente = Convert.ToInt32(htClienti[cmbClienti.SelectedIndex].ToString());
			MasterFile mf = MasterFile.Create();
			Hashtable clientetmp = mf.GetAnagrafica(IDCliente);

			switch ((App.TipoAnagraficaEsercizio)(Convert.ToInt32(clientetmp["Esercizio"].ToString())))
            {
                case App.TipoAnagraficaEsercizio.ACavallo:
					returnvalue = Convert.ToDateTime(data).Year.ToString() + " - " + (Convert.ToDateTime(data).Year + 1).ToString();
                    break;
				case App.TipoAnagraficaEsercizio.AnnoSolare:
                case App.TipoAnagraficaEsercizio.Sconosciuto:
                default:
					returnvalue = Convert.ToDateTime(data).Year.ToString();
                    break;
            }

			return returnvalue;
		}

        private string ConvertDataToEsercizio(string data, Hashtable ht)
        {
            string returnvalue = "";

            if (ht.Contains("Intermedio") && ht.Contains("EsercizioDal") && ht.Contains("EsercizioAl"))
            {
                returnvalue = "dal " + ht["EsercizioDal"].ToString() + " al " + ht["EsercizioAl"].ToString();
            }
            else
            {

                switch ((App.TipoAnagraficaEsercizio)(Convert.ToInt32(ht["Esercizio"].ToString())))
                {
                    case App.TipoAnagraficaEsercizio.ACavallo:
                        returnvalue = Convert.ToDateTime(data).Year.ToString() + " - " + (Convert.ToDateTime(data).Year + 1).ToString();
                        break;
                    case App.TipoAnagraficaEsercizio.AnnoSolare:
                    case App.TipoAnagraficaEsercizio.Sconosciuto:
                    default:
                        returnvalue = Convert.ToDateTime(data).Year.ToString();
                        break;
                }
            }

            return returnvalue;
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
			dtpDataNomina.SelectedIndex = -1;
			txtNota.Text = "";
			imageStato.Visibility = System.Windows.Visibility.Hidden;

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

					List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>();

					foreach (Hashtable item in mf.GetRevisioni(IDCliente))
					{

						myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), ConvertDataToEsercizio(item["Data"].ToString(), item)));
					}

                    myList.Sort
					(
						delegate(KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
						{
							try
							{
								return Convert.ToInt32( nextPair.Value ).CompareTo( Convert.ToInt32( firstPair.Value ) );
							}
							catch (Exception ex)
							{
								string log = ex.Message;
								return 1;
							}
							
						}
					);

					foreach (KeyValuePair<string, string> item in myList)
					{
						cmbData.Items.Add(item.Value);
						htDate.Add(index, item.Key);
						index++;
					}

					index = 0;
					htSelectedDate.Clear();
					dtpDataNomina.Items.Clear();

					for (int i = 2009; i <= (DateTime.Now.Year + 1); i++)
					{
						Hashtable clientetmp = mf.GetAnagrafica(Convert.ToInt32(IDCliente));

						string datatmp = "";

						switch ((App.TipoAnagraficaEsercizio)(Convert.ToInt32(clientetmp["Esercizio"].ToString())))
						{
							case App.TipoAnagraficaEsercizio.ACavallo:
								datatmp = "31/12/" + i.ToString();
								break;
							case App.TipoAnagraficaEsercizio.AnnoSolare:
							case App.TipoAnagraficaEsercizio.Sconosciuto:
							default:
								datatmp = "01/01/" + i.ToString();
								break;
						}

						if (!cmbData.Items.Contains(ConvertDataToEsercizio(datatmp)))
						{
							dtpDataNomina.Items.Add(ConvertDataToEsercizio(datatmp));
							htSelectedDate.Add(index, datatmp);
							index++;
						}
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
						label2.Visibility = System.Windows.Visibility.Collapsed;
						dtpDataNomina.Visibility = System.Windows.Visibility.Collapsed;
						label3.Visibility = System.Windows.Visibility.Visible;
						cmbData.Visibility = System.Windows.Visibility.Visible;
						txtNota.IsEnabled = false;
					}
					else
					{
						cmbData.IsEnabled = false;
						dtpDataNomina.IsEnabled = true;
						label2.Visibility = System.Windows.Visibility.Visible;
						dtpDataNomina.Visibility = System.Windows.Visibility.Visible;
						label3.Visibility = System.Windows.Visibility.Collapsed;
						cmbData.Visibility = System.Windows.Visibility.Collapsed;
						txtNota.IsEnabled = true;
					}
				}
				catch (Exception ex)
				{
					string log = ex.Message;

					cmbData.IsEnabled = false;
					dtpDataNomina.IsEnabled = false;
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

					IDRevisione = htDate[cmb.SelectedIndex].ToString();

					MasterFile mf = MasterFile.Create();
					Hashtable htRevisione = new Hashtable();

					htRevisione = mf.GetRevisione(IDRevisione);
					dtpDataNomina.IsEnabled = true;
					txtNota.IsEnabled = true;

					foreach (DictionaryEntry item in htSelectedDate)
					{
						if (item.Value.ToString() == ConvertDataToEsercizio(htRevisione["Data"].ToString(), htRevisione))
						{
							dtpDataNomina.SelectedIndex = Convert.ToInt32(item.Key.ToString());
						}
					}

					txtNota.Text = htRevisione["Note"].ToString();

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
			int IDRevisione = App.MasterFile_NewID;

			try
			{
				IDRevisione = Convert.ToInt32(htDate[cmbData.SelectedIndex].ToString());
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}

			if (TipologiaAttivita == App.TipoAttivitaScheda.Delete && IDRevisione == -1)
            {
                MessageBox.Show("selezionare una sessione");
                return;
            }

			App.TipoAttivitaScheda oldTipo = TipologiaAttivita;

			switch (TipologiaAttivita)
			{
				//Nuovo e salva
				case App.TipoAttivitaScheda.New:
				case App.TipoAttivitaScheda.Edit:
					//Campi Obbligatorio
					if (dtpDataNomina.SelectedIndex == -1)// !u.ConvalidaDatiInterfaccia(dtpDataNomina, "Data mancante."))
					{
						MessageBox.Show("Selezionare un Esercizio.");
						return;
					}

					//Controllo che questa data non sia già stata presa
					if (!mf.CheckDoppio_Revisione(IDRevisione, IDCliente, htSelectedDate[dtpDataNomina.SelectedIndex].ToString()))
					{
						MessageBox.Show("Data già presente per questo cliente");
						return;
					}

					//setto dati
					Hashtable ht = new Hashtable();

					ht.Add("Cliente", IDCliente);
					ht.Add("Data", htSelectedDate[dtpDataNomina.SelectedIndex].ToString());
					ht.Add("Note", txtNota.Text.Trim());

					IDRevisione = mf.SetRevisione(ht, IDRevisione, IDCliente);

					RegistrazioneEffettuata = true;

					if (TipologiaAttivita == App.TipoAttivitaScheda.New)
					{
                        if ( IDClienteImport != "-1" )
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
							if (Convert.ToInt32(item.Value.ToString()) == IDRevisione)
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
					mf.DeleteRevisione(IDRevisione);
					RegistrazioneEffettuata = true;
					base.Close();
					break;
				case App.TipoAttivitaScheda.Export:
					break;
			}

            //apro tree appena creato
            if (oldTipo == App.TipoAttivitaScheda.New)
            {
                //MessageBox.Show("apro tree appena creato");
				Accedi_Click(IDRevisione.ToString(), false);
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
			int IDVerifica = App.MasterFile_NewID;

			try
			{
				IDVerifica = Convert.ToInt32(htDate[cmbData.SelectedIndex].ToString());
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}

			if (IDVerifica == -1)
			{
				MessageBox.Show("selezionare una sessione");
			}
			else
			{
#if (false)
        string str = String.Format("cliente: {0}, revisione: {1}", IDCliente, IDVerifica);
        MessageBox.Show(str);
#endif
        bool isSchedaReadOnly = TipologiaAttivita == App.TipoAttivitaScheda.View || anaStato == App.TipoAnagraficaStato.InUso;
        Accedi_Click(IDVerifica.ToString(), isSchedaReadOnly);
			}
		}

    public void accedi( string ID, bool ReadOnly )
    {
      MasterFile mf = MasterFile.Create();
      Hashtable htSelected = mf.GetRevisione(ID);

      if (htSelected.Count == 0) return;
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
      wWorkArea.TipoAttivita = App.TipoAttivita.Revisione;
      wWorkArea.Cliente = (((Hashtable)(mf.GetAnagrafica(
        Convert.ToInt32( htSelected["Cliente"].ToString()))))
        ["RagioneSociale"].ToString() ) + " (C.F. "
        + (((Hashtable)(mf.GetAnagrafica(Convert.ToInt32(htSelected["Cliente"].ToString()))))
        ["CodiceFiscale"].ToString()) + ")";
      wWorkArea.SessioneAlias = "";
      wWorkArea.SessioneFile = "";
      wWorkArea.SessioneSigillo = null;
      wWorkArea.SessioneSigilloData = null;
      wWorkArea.SessioneSigilloPassword = null;

      wWorkArea.IDTree = (Convert.ToInt32(App.TipoFile.Revisione)).ToString();
      wWorkArea.IDCliente = htSelected["Cliente"].ToString();
      wWorkArea.IDSessione = ID;

      foreach (Hashtable item in ((ArrayList)(mf.GetRevisioni(htSelected["Cliente"].ToString()))))
      {
        wWorkArea.SessioneFile += ( ( wWorkArea.SessioneFile == "" ) ? "" : "|" ) + App.AppDataDataFolder + "\\" + item["FileData"].ToString();
        wWorkArea.SessioneAlias += ( ( wWorkArea.SessioneAlias == "" ) ? "" : "|" ) + item["Data"].ToString();// ConvertDataToEsercizio(item["Data"].ToString());
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
      Show();
      //Close();
  }

		private void Accedi_Click(string ID, bool ReadOnly)
		{
			try
			{
        if (noopenaftercreate) return;
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

	}
}
*/
