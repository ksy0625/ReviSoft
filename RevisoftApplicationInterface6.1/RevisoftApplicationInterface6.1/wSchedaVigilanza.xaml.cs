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
using System.Threading;
using System.Data;
using System.Windows.Threading;

namespace RevisoftApplication
{

  public partial class wSchedaVigilanza : Window
  {
    private App.TipoAttivitaScheda _tipologiaAttivita;
    private App.TipoAttivitaScheda oldTipologiaAttivita = App.TipoAttivitaScheda.View;

    public int IDVigilanza;

    private bool _InCaricamento;
    private bool _DatiCambiati;
    public bool RegistrazioneEffettuata;

    private bool firsttime = true;

    private bool _cmbInCaricamento = false;
    private int OldSelectedCmbClienti = -1;

    public string IDClienteImport = "-1";

    Hashtable htClienti = new Hashtable();
    Hashtable htDate = new Hashtable();

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
              if (mf.GetVigilanze(item["ID"].ToString()).Count == 0)
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

    public wSchedaVigilanza()
    {
      if (ALreadyDone) { }
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

    public void ConfiguraMaschera()
    {
      //inibisco tutto i controlli
      dtpDataNomina.IsHitTestVisible = false;
      dtpDataEsecuzione.IsHitTestVisible = false;
      dtpDataEsecuzione_Fine.IsHitTestVisible = false;
      cmbInizio.IsReadOnly = true;
      cmbFine.IsReadOnly = true;
      txtLuogo.IsReadOnly = true;
      txtSindacoRevisore.IsReadOnly = true;
      txtPresidente.IsReadOnly = true;
      txtSindacoEffettivo1.IsReadOnly = true;
      txtSindacoEffettivo2.IsReadOnly = true;
      txtAssistitoDa.IsReadOnly = true;
      rdbCollegioSindacale.IsHitTestVisible = false;
      rdbRevisore.IsHitTestVisible = false;
      rdbSindacoUnico.IsHitTestVisible = false;

      //nascondo testo help - non + usato
      textBlockDescrizione.Text = "";
      textBlockDescrizione.Visibility = System.Windows.Visibility.Collapsed;

      switch (TipologiaAttivita)
      {
        case App.TipoAttivitaScheda.New:
          labelTitolo.Content = "Nuova Sessione";
          buttonComando.Content = "Crea";
          txtLuogo.Text = "la sede della società";
          buttonComando.Visibility = System.Windows.Visibility.Visible;
          dtpDataNomina.IsHitTestVisible = true;
          dtpDataEsecuzione.IsHitTestVisible = true;
          dtpDataEsecuzione_Fine.IsHitTestVisible = true;
          cmbInizio.IsReadOnly = false;
          cmbFine.IsReadOnly = false;
          txtLuogo.IsReadOnly = false;
          txtSindacoRevisore.IsReadOnly = false;
          txtPresidente.IsReadOnly = false;
          txtSindacoEffettivo1.IsReadOnly = false;
          txtSindacoEffettivo2.IsReadOnly = false;
          txtAssistitoDa.IsReadOnly = false;
          rdbCollegioSindacale.IsHitTestVisible = true;
          rdbRevisore.IsHitTestVisible = true;
          rdbSindacoUnico.IsHitTestVisible = true;
          GridComboData.Visibility = System.Windows.Visibility.Collapsed;
          buttonComando.Visibility = System.Windows.Visibility.Visible;
          buttonApri.Visibility = System.Windows.Visibility.Hidden;
          break;
        case App.TipoAttivitaScheda.Edit:
          labelTitolo.Content = "Modifica Sessione";
          buttonComando.Content = "Salva";
          buttonComando.Visibility = System.Windows.Visibility.Collapsed;
          dtpDataNomina.IsHitTestVisible = true;
          dtpDataEsecuzione.IsHitTestVisible = true;
          dtpDataEsecuzione_Fine.IsHitTestVisible = true;
          cmbInizio.IsReadOnly = false;
          cmbFine.IsReadOnly = false;
          txtLuogo.IsReadOnly = false;
          txtSindacoRevisore.IsReadOnly = false;
          txtPresidente.IsReadOnly = false;
          txtSindacoEffettivo1.IsReadOnly = false;
          txtSindacoEffettivo2.IsReadOnly = false;
          txtAssistitoDa.IsReadOnly = false;
          rdbCollegioSindacale.IsHitTestVisible = true;
          rdbRevisore.IsHitTestVisible = true;
          rdbSindacoUnico.IsHitTestVisible = true;
          buttonComando.Visibility = System.Windows.Visibility.Collapsed;
          buttonApri.Visibility = System.Windows.Visibility.Visible;
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
          buttonApri.Visibility = System.Windows.Visibility.Visible;
          break;
        case App.TipoAttivitaScheda.View:
        default:
          labelTitolo.Content = "Apri Sessione in sola lettura";
          cmbData.Visibility = System.Windows.Visibility.Visible;
          buttonApri.Visibility = System.Windows.Visibility.Visible;
          //buttonApri.Margin = buttonComando.Margin;
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

    private void functionCmbClientiChanged(ComboBox cmb)
    {

      MasterFile mf = MasterFile.Create();

      if (_cmbInCaricamento)
        return;

      if (oldTipologiaAttivita != App.TipoAttivitaScheda.View)
      {
        TipologiaAttivita = oldTipologiaAttivita;
      }

      _InCaricamento = true;
      cmbData.SelectedIndex = -1;
      dtpDataNomina.Text = "";
      dtpDataEsecuzione.Text = "";
      dtpDataEsecuzione_Fine.Text = "";
      cmbInizio.SelectedValue = "00:00";
      cmbFine.SelectedValue = "00:00";
      txtLuogo.Text = "";
      txtSindacoRevisore.Text = "";
      txtPresidente.Text = "";
      txtSindacoEffettivo1.Text = "";
      txtSindacoEffettivo2.Text = "";
      txtAssistitoDa.Text = "";
      rdbCollegioSindacale.IsChecked = false;
      rdbRevisore.IsChecked = false;
      rdbSindacoUnico.IsChecked = false;

      if (cmb.SelectedIndex != -1)
      {
        try
        {
          string IDCliente = htClienti[cmb.SelectedIndex].ToString();

          Hashtable daticliente = mf.GetAnagrafica(Convert.ToInt32(IDCliente));

          if (daticliente["OrganoDiControllo"] != null)
          {
            switch (daticliente["OrganoDiControllo"].ToString())
            {
              case "1": //rdbOrganoControlloSindaco
                rdbCollegioSindacale.IsChecked = true;
                txtPresidente.Text = ((daticliente["Presidente"] != null) ? daticliente["Presidente"].ToString() : "");
                txtSindacoEffettivo1.Text = ((daticliente["MembroEffettivo"] != null) ? daticliente["MembroEffettivo"].ToString() : "");
                txtSindacoEffettivo2.Text = ((daticliente["MembroEffettivo2"] != null) ? daticliente["MembroEffettivo2"].ToString() : "");
                break;
              case "2"://rdbOrganoControlloCollegio
                rdbSindacoUnico.IsChecked = true;
                txtSindacoRevisore.Text = ((daticliente["Presidente"] != null) ? daticliente["Presidente"].ToString() : "");
                break;
              default: //non si applica
                rdbRevisore.IsChecked = true;
                txtSindacoRevisore.Text = ((daticliente["RevisoreAutonomo"] != null) ? daticliente["RevisoreAutonomo"].ToString() : "");
                break;
            }
          }



          if (_tipologiaAttivita == App.TipoAttivitaScheda.New)
          {
            ArrayList al = mf.GetPianificazioniVigilanze(IDCliente);
            List<DateTime> alX = new List<DateTime>();

            List<string> alS = new List<string>();

            foreach (Hashtable item in mf.GetVigilanze(IDCliente))
            {

              alS.Add(item["Data"].ToString());
            }

            foreach (Hashtable itemHT in al)
                        {
                            DataTable pianificazione = cBusinessObjects.GetData(100003, typeof(PianificazioneVerificheTestata), int.Parse(IDCliente), int.Parse(itemHT["ID"].ToString()), 27);

                            foreach (DataRow itemXPP in pianificazione.Rows)
                            {
                                DateTime dt = Convert.ToDateTime(itemXPP["Data"].ToString());
                                bool giapresente = false;
                                foreach (Hashtable item in mf.GetVerifiche(IDCliente))
                                {
                                     DateTime dtp = Convert.ToDateTime(item["Data"].ToString());
                                    if (dtp==dt)
                                         giapresente = true;
                                }
                  
                                if (!giapresente && !alX.Contains(dt))
                                {
                                    alX.Add(dt);
                                }
                            }

                        }

            if (alX.Count > 0)
            {
              alX.Sort();

              foreach (DateTime item in alX)
              {
                cmbPianificate.Items.Add(item.ToShortDateString());
              }

              grdPianificazione.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
              grdPianificazione.Visibility = System.Windows.Visibility.Collapsed;
            }
          }

          OldSelectedCmbClienti = Convert.ToInt32(IDCliente);

          int index = 0;
          htDate.Clear();
          cmbData.Items.Clear();

          List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>();

          foreach (Hashtable item in mf.GetVigilanze(IDCliente))
          {

            myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), item["Data"].ToString()));
          }

          myList.Sort
          (
            delegate (KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
            {
              return Convert.ToDateTime(nextPair.Value).CompareTo(Convert.ToDateTime(firstPair.Value));
            }
          );

          foreach (KeyValuePair<string, string> item in myList)
          {
            cmbData.Items.Add(item.Value);
            htDate.Add(index, item.Key);
            index++;
          }

          //stato
          //               App.TipoAnagraficaStato anaStato = mf.GetAnafraficaStato(Convert.ToInt32(IDCliente));

          ////non disponibile: configuro interfaccia
          //if (anaStato != App.TipoAnagraficaStato.Disponibile)
          //{
          //	oldTipologiaAttivita = TipologiaAttivita;
          //	TipologiaAttivita = App.TipoAttivitaScheda.View;
          //}


          //stato
          if (IDClienteImport == "-1")
          {
            App.TipoAnagraficaStato anaStato = mf.GetAnafraficaStato(Convert.ToInt32(IDCliente));

            //non disponibile: configuro interfaccia
            //  if (anaStato != App.TipoAnagraficaStato.Disponibile)
            //   {
            //   oldTipologiaAttivita = TipologiaAttivita;
            //   TipologiaAttivita = App.TipoAttivitaScheda.View;
            // }
          }


          if (TipologiaAttivita != App.TipoAttivitaScheda.New)
          {
            cmbData.IsEnabled = true;
            dtpDataNomina.IsEnabled = false;
            dtpDataEsecuzione.IsEnabled = false;
            dtpDataEsecuzione_Fine.IsEnabled = false;
            cmbInizio.IsEnabled = false;
            cmbFine.IsEnabled = false;
            txtLuogo.IsEnabled = false;
            txtSindacoRevisore.IsEnabled = false;
            txtPresidente.IsEnabled = false;
            txtSindacoEffettivo1.IsEnabled = false;
            txtSindacoEffettivo2.IsEnabled = false;
            txtAssistitoDa.IsEnabled = false;
            rdbCollegioSindacale.IsEnabled = false;
            rdbRevisore.IsEnabled = false;
            rdbSindacoUnico.IsEnabled = false;
          }
          else
          {
            cmbData.IsEnabled = false;
            dtpDataNomina.IsEnabled = true;
            dtpDataEsecuzione.IsEnabled = true;
            dtpDataEsecuzione_Fine.IsEnabled = true;
            cmbInizio.IsEnabled = true;
            cmbFine.IsEnabled = true;
            txtLuogo.IsEnabled = true;
            txtSindacoRevisore.IsEnabled = true;
            txtPresidente.IsEnabled = true;
            txtSindacoEffettivo1.IsEnabled = true;
            txtSindacoEffettivo2.IsEnabled = true;
            txtAssistitoDa.IsEnabled = true;
            rdbCollegioSindacale.IsEnabled = true;
            rdbRevisore.IsEnabled = true;
            rdbSindacoUnico.IsEnabled = true;
          }
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wSchedaVigilanza.functionCmbClientiChanged exception");
          string log = ex.Message;

          cmbData.IsEnabled = false;
          dtpDataNomina.IsEnabled = false;

          dtpDataEsecuzione.IsEnabled = false;
          dtpDataEsecuzione_Fine.IsEnabled = false;
          cmbInizio.IsEnabled = false;
          cmbFine.IsEnabled = false;
          txtLuogo.IsEnabled = false;
          txtSindacoRevisore.IsEnabled = false;
          txtPresidente.IsEnabled = false;
          txtSindacoEffettivo1.IsEnabled = false;
          txtSindacoEffettivo2.IsEnabled = false;
          txtAssistitoDa.IsEnabled = false;
          rdbCollegioSindacale.IsEnabled = false;
          rdbRevisore.IsEnabled = false;
          rdbSindacoUnico.IsEnabled = false;
        }
      }
    }

    private void cmbData_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      functionCmbDataChanged(((ComboBox)sender));
      ConfiguraMaschera();
    }

    private string CorreggiData(string data)
    {
      string returnstring = "00:00";

      if (data.Trim() != "" && data.Contains(':'))
      {
        data = data.Replace(":", "");
        int intdata = 0;

        int.TryParse(data, out intdata);

        intdata = intdata - (intdata % 100 % 15);

        data = intdata.ToString().PadLeft(4, '0');
        returnstring = data.Substring(0, 2) + ":" + data.Substring(2, 2);
      }

      return returnstring;
    }

    private void functionCmbDataChanged(ComboBox cmb)
    {
      if (cmb.SelectedIndex != -1)
      {
        try
        {
          _InCaricamento = true;

          IDVigilanza = Convert.ToInt32(htDate[cmb.SelectedIndex].ToString());

          MasterFile mf = MasterFile.Create();
          Hashtable htVigilanza = new Hashtable();

          htVigilanza = mf.GetVigilanza(IDVigilanza.ToString());
          dtpDataNomina.IsEnabled = true;

          dtpDataEsecuzione.IsEnabled = true;
          dtpDataEsecuzione_Fine.IsEnabled = true;
          cmbInizio.IsEnabled = true;
          cmbFine.IsEnabled = true;
          txtLuogo.IsEnabled = true;
          txtSindacoRevisore.IsEnabled = true;
          txtPresidente.IsEnabled = true;
          txtSindacoEffettivo1.IsEnabled = true;
          txtSindacoEffettivo2.IsEnabled = true;
          txtAssistitoDa.IsEnabled = true;
          rdbCollegioSindacale.IsEnabled = true;
          rdbRevisore.IsEnabled = true;
          rdbSindacoUnico.IsEnabled = true;

          dtpDataNomina.Text = htVigilanza["Data"].ToString();
          dtpDataEsecuzione.Text = (htVigilanza["DataEsecuzione"] != null) ? htVigilanza["DataEsecuzione"].ToString() : htVigilanza["Data"].ToString();
          dtpDataEsecuzione_Fine.Text = (htVigilanza["DataEsecuzione_Fine"] != null) ? htVigilanza["DataEsecuzione_Fine"].ToString() : htVigilanza["Data"].ToString();
          cmbInizio.SelectedValue = CorreggiData(htVigilanza["Inizio"].ToString());
          cmbFine.SelectedValue = CorreggiData(htVigilanza["Fine"].ToString());
          txtLuogo.Text = htVigilanza["Luogo"].ToString();
          txtSindacoRevisore.Text = htVigilanza["Revisore"].ToString();
          txtPresidente.Text = htVigilanza["Presidente"].ToString();
          txtSindacoEffettivo1.Text = htVigilanza["Sindaco1"].ToString();
          txtSindacoEffettivo2.Text = htVigilanza["Sindaco2"].ToString();
          txtAssistitoDa.Text = htVigilanza["AssisitoDa"].ToString();

          switch ((App.TipoIncaricoComposizione)(Convert.ToInt32(htVigilanza["Composizione"].ToString())))
          {
            case App.TipoIncaricoComposizione.CollegioSindacale:
              rdbCollegioSindacale.IsChecked = true;
              rdbRevisore.IsChecked = false;
              rdbSindacoUnico.IsChecked = false;
              break;
            case App.TipoIncaricoComposizione.Revisore:
              rdbCollegioSindacale.IsChecked = false;
              rdbRevisore.IsChecked = true;
              rdbSindacoUnico.IsChecked = false;
              break;
            case App.TipoIncaricoComposizione.SindacoUnico:
              rdbCollegioSindacale.IsChecked = false;
              rdbRevisore.IsChecked = false;
              rdbSindacoUnico.IsChecked = true;
              break;
            case App.TipoIncaricoComposizione.Sconosciuto:
            default:
              break;
          }

          _InCaricamento = false;
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wSchedaVigilanza.functionCmbDataChanged exception");
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
      IDVigilanza = App.MasterFile_NewID;

      try
      {
        IDVigilanza = Convert.ToInt32(htDate[cmbData.SelectedIndex].ToString());
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaVigilanza.buttonComando_Click exception");
        string log = ex.Message;
      }

      if (TipologiaAttivita == App.TipoAttivitaScheda.Delete && IDVigilanza == -1)
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
          //convalida dati
          //Campi Obbligatorio
          if (!u.ConvalidaDatiInterfaccia(dtpDataNomina, "Data mancante."))
            return;

          //if (!u.ConvalidaDatiInterfaccia(cmbInizio, "Ora inizio mancante."))
          //    return;

          //if (!u.ConvalidaDatiInterfaccia(txtLuogo, "Luogo mancante."))
          //    return;

          //Controllo che questa data non sia già stata presa
          if (!mf.CheckDoppio_Vigilanza(IDVigilanza, IDCliente, dtpDataNomina.SelectedDate.Value.ToShortDateString()))
          {
            MessageBox.Show("Data già presente per questo cliente");
            return;
          }

          //setto dati
          Hashtable ht = new Hashtable();
          ht.Add("Cliente", IDCliente);
          ht.Add("Data", dtpDataNomina.SelectedDate.Value.ToShortDateString());
          ht.Add("DataEsecuzione", ((dtpDataEsecuzione.SelectedDate != null) ? dtpDataEsecuzione.SelectedDate.Value.ToShortDateString() : dtpDataNomina.SelectedDate.Value.ToShortDateString()));
          ht.Add("DataEsecuzione_Fine", ((dtpDataEsecuzione_Fine.SelectedDate != null) ? dtpDataEsecuzione_Fine.SelectedDate.Value.ToShortDateString() : dtpDataNomina.SelectedDate.Value.ToShortDateString()));

          ComboBoxItem typeItem = (ComboBoxItem)(cmbInizio.SelectedItem);
          ht.Add("Inizio", typeItem.Content.ToString());

          ComboBoxItem typeItem2 = (ComboBoxItem)(cmbFine.SelectedItem);
          ht.Add("Fine", typeItem2.Content.ToString());

          ht.Add("Luogo", txtLuogo.Text.Trim());
          ht.Add("Revisore", txtSindacoRevisore.Text.Trim());
          ht.Add("Presidente", txtPresidente.Text.Trim());
          ht.Add("Sindaco1", txtSindacoEffettivo1.Text.Trim());
          ht.Add("Sindaco2", txtSindacoEffettivo2.Text.Trim());
          ht.Add("AssisitoDa", txtAssistitoDa.Text.Trim());

          if (rdbCollegioSindacale.IsChecked == false && rdbRevisore.IsChecked == false && rdbSindacoUnico.IsChecked == false)
          {
            ht.Add("Composizione", (int)(App.TipoIncaricoComposizione.Sconosciuto));
          }
          else
          {
            if (rdbCollegioSindacale.IsChecked == true)
            {
              ht.Add("Composizione", (int)(App.TipoIncaricoComposizione.CollegioSindacale));
            }

            if (rdbRevisore.IsChecked == true)
            {
              ht.Add("Composizione", (int)(App.TipoIncaricoComposizione.Revisore));
            }

            if (rdbSindacoUnico.IsChecked == true)
            {
              ht.Add("Composizione", (int)(App.TipoIncaricoComposizione.SindacoUnico));
            }
          }

          IDVigilanza = mf.SetVigilanza(ht, IDVigilanza, IDCliente, false);

          RegistrazioneEffettuata = true;

          if (TipologiaAttivita == App.TipoAttivitaScheda.New)
          {
            if (IDClienteImport != "-1")
            {
              this.Close();
            }
            stackPanel1.IsEnabled = false;
            gridButtons.IsEnabled = false;
            loading.Visibility = Visibility;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,new Action(delegate { }));
            //cBusinessObjects.show_workinprogress("Prima creazione dell'albero in corso...");
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
              if (Convert.ToInt32(item.Value.ToString()) == IDVigilanza)
              {
                cmbData.SelectedIndex = Convert.ToInt32(item.Key.ToString());
              }
            }

            functionCmbDataChanged(cmbData);
            cBusinessObjects.AddSessione("Vigilanza", cmbData.SelectedValue.ToString(), IDVigilanza, IDCliente);
          }
          _DatiCambiati = false;
          break;
        case App.TipoAttivitaScheda.Delete:
          //richiesta conferma
          if (MessageBoxResult.No == u.ConfermaCancellazione())
            return;
          //cancellazione
          mf.DeleteVigilanza(IDVigilanza, IDCliente.ToString());
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
        AccediVigilanza_Click(IDVigilanza.ToString(), false);
      }

      //chiudo maschera
      if (TipologiaAttivita != App.TipoAttivitaScheda.Edit)
        base.Close();
    }



    private void AccediVigilanza_Click(string ID, bool ReadOnly)
    {
      try
      {
        if (noopenaftercreate)
        {
          return;
        }

        accedi(ID, ReadOnly, false);
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaVigilanza.AccediVigilanza_Click exception");
        string log = ex.Message;
      }
    }

    //----------------------------------------------------------------------------+
    //                                   accedi                                   |
    //----------------------------------------------------------------------------+
    public void accedi(string ID, bool ReadOnly, bool tobeclosed)
    {
      try

      {
        MasterFile mf = MasterFile.Create();
        Hashtable htSelected = mf.GetVigilanza(ID);

        if (htSelected.Count == 0) return;

        XmlDataProviderManager _test = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + htSelected["File"].ToString());

        WindowWorkAreaTree wWorkArea = new WindowWorkAreaTree();
        //wWorkArea.Owner = this;
        wWorkArea.SelectedTreeSource = App.AppDataDataFolder + "\\" + htSelected["File"].ToString();
        wWorkArea.SelectedDataSource = App.AppDataDataFolder + "\\" + htSelected["FileData"].ToString();
        wWorkArea.ReadOnly = ReadOnly;
        wWorkArea.TipoAttivita = App.TipoAttivita.Vigilanza;
        wWorkArea.Cliente = (((Hashtable)(mf.GetAnagrafica(Convert.ToInt32(htSelected["Cliente"].ToString()))))["RagioneSociale"].ToString()) + " (C.F. " + (((Hashtable)(mf.GetAnagrafica(Convert.ToInt32(htSelected["Cliente"].ToString()))))["CodiceFiscale"].ToString()) + ")";
        wWorkArea.SessioneAlias = "";
        wWorkArea.SessioneFile = "";

        wWorkArea.IDTree = (Convert.ToInt32(App.TipoFile.Vigilanza)).ToString();
        wWorkArea.IDCliente = htSelected["Cliente"].ToString();
        wWorkArea.IDSessione = ID;

        foreach (Hashtable item in ((ArrayList)(mf.GetVigilanze(htSelected["Cliente"].ToString()))))
        {
          wWorkArea.SessioneFile += ((wWorkArea.SessioneFile == "") ? "" : "|") + App.AppDataDataFolder + "\\" + item["FileData"].ToString();
          wWorkArea.SessioneAlias += ((wWorkArea.SessioneAlias == "") ? "" : "|") + item["Data"].ToString();
          wWorkArea.SessioneID += ((wWorkArea.SessioneID == "") ? "" : "|") + item["ID"].ToString();
        }

        //aperto in sola lettura
        wWorkArea.ApertoInSolaLettura = TipologiaAttivita == App.TipoAttivitaScheda.View;

        wWorkArea.LoadTreeSource();
        Hide();
        wWorkArea.ShowDialog();

        //setto dati
        Hashtable ht = new Hashtable();
        ht.Add("Cliente", Convert.ToInt32(htSelected["Cliente"].ToString()));
        ht.Add("Data", dtpDataNomina.SelectedDate.Value.ToShortDateString());
        ht.Add("DataEsecuzione", ((dtpDataEsecuzione.SelectedDate != null) ? dtpDataEsecuzione.SelectedDate.Value.ToShortDateString() : dtpDataNomina.SelectedDate.Value.ToShortDateString()));
        ht.Add("DataEsecuzione_Fine", ((dtpDataEsecuzione_Fine.SelectedDate != null) ? dtpDataEsecuzione_Fine.SelectedDate.Value.ToShortDateString() : dtpDataNomina.SelectedDate.Value.ToShortDateString()));

        ComboBoxItem typeItem = (ComboBoxItem)(cmbInizio.SelectedItem);
        ht.Add("Inizio", typeItem.Content.ToString());

        ComboBoxItem typeItem2 = (ComboBoxItem)(cmbFine.SelectedItem);
        ht.Add("Fine", typeItem2.Content.ToString());

        //ht.Add("Fine", DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Minute.ToString().PadLeft(2, '0'));
        ht.Add("Luogo", txtLuogo.Text.Trim());
        ht.Add("Revisore", txtSindacoRevisore.Text.Trim());
        ht.Add("Presidente", txtPresidente.Text.Trim());
        ht.Add("Sindaco1", txtSindacoEffettivo1.Text.Trim());
        ht.Add("Sindaco2", txtSindacoEffettivo2.Text.Trim());
        ht.Add("AssisitoDa", txtAssistitoDa.Text.Trim());

        if (rdbCollegioSindacale.IsChecked == false && rdbRevisore.IsChecked == false && rdbSindacoUnico.IsChecked == false)
        {
          ht.Add("Composizione", (int)(App.TipoIncaricoComposizione.Sconosciuto));
        }
        else
        {
          if (rdbCollegioSindacale.IsChecked == true)
          {
            ht.Add("Composizione", (int)(App.TipoIncaricoComposizione.CollegioSindacale));
          }

          if (rdbRevisore.IsChecked == true)
          {
            ht.Add("Composizione", (int)(App.TipoIncaricoComposizione.Revisore));
          }

          if (rdbSindacoUnico.IsChecked == true)
          {
            ht.Add("Composizione", (int)(App.TipoIncaricoComposizione.SindacoUnico));
          }

        }

        mf.SetVigilanza(ht, Convert.ToInt32(ID), Convert.ToInt32(htSelected["Cliente"].ToString()), false);

        if (TipologiaAttivita != App.TipoAttivitaScheda.View)
        {
          int IDCliente = Convert.ToInt32(htClienti[cmbClienti.SelectedIndex].ToString());
          mf.SetAnafraficaStato(Convert.ToInt32(IDCliente), App.TipoAnagraficaStato.Disponibile);
        }

        functionCmbDataChanged(cmbData);

        //Close();
        //Show();Activate();
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaVigilanza.accedi exception");
        string log = ex.Message;
      }
    }

    private void ButtonApri_Click(object sender, RoutedEventArgs e)
    {
      //controllo selezione clienti
      if (cmbClienti.SelectedIndex == -1)
      {
        MessageBox.Show("selezionare un cliente");
        return;
      }

      ComboBoxItem typeItem = (ComboBoxItem)(cmbInizio.SelectedItem);

      //if ( TipologiaAttivita != App.TipoAttivitaScheda.View && typeItem == null )
      //{
      //    MessageBox.Show("Inserire un'ora di inizio");
      //    return;
      //}

      //if (TipologiaAttivita != App.TipoAttivitaScheda.View && txtLuogo.Text.Trim() == "")
      //{
      //    MessageBox.Show("Inserire un luogo");
      //    return;
      //}

      //dati modificati
      if (_DatiCambiati)
      {
        //Utilities u = new Utilities();
        //if (MessageBoxResult.No == u.AvvisoPerditaDati("Alcuni dati sono stati modificati, confermi apertura?"))
        //    return;
        MasterFile mf2 = MasterFile.Create();

        int IDClientetmp = Convert.ToInt32(htClienti[cmbClienti.SelectedIndex].ToString());
        int IDVigilanzatmp = App.MasterFile_NewID;

        try
        {
          IDVigilanzatmp = Convert.ToInt32(htDate[cmbData.SelectedIndex].ToString());
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wSchedaVigilanza.buttonComando_Click exception");
          string log = ex.Message;
        }


        if (!mf2.CheckDoppio_Vigilanza(IDVigilanzatmp, IDClientetmp, dtpDataNomina.SelectedDate.Value.ToShortDateString()))
        {
          MessageBox.Show("Data già presente per questo cliente");
          return;
        }



        //Salvataggio automatico come richiesto da 2.3
        App.TipoAttivitaScheda OLDTipologiaAttivita = TipologiaAttivita;
        TipologiaAttivita = App.TipoAttivitaScheda.Edit;
        buttonComando_Click(sender, e);
        TipologiaAttivita = OLDTipologiaAttivita;
      }

      //disponibile: blocco cliente
      int IDCliente = Convert.ToInt32(htClienti[cmbClienti.SelectedIndex].ToString());
      MasterFile mf = MasterFile.Create();
      App.TipoAnagraficaStato anaStato = mf.GetAnafraficaStato(IDCliente);

      if (anaStato == App.TipoAnagraficaStato.Disponibile && TipologiaAttivita != App.TipoAttivitaScheda.View)
        mf.SetAnafraficaStato(Convert.ToInt32(IDCliente), App.TipoAnagraficaStato.InUso);

      //apre treee
      IDVigilanza = App.MasterFile_NewID;

      try
      {
        IDVigilanza = Convert.ToInt32(htDate[cmbData.SelectedIndex].ToString());
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaVigilanza.ButtonApri_Click exception");
        string log = ex.Message;
      }

      if (IDVigilanza == -1)
      {
        MessageBox.Show("selezionare una sessione");
      }
      else
      {
        bool isSchedaReadOnly = TipologiaAttivita == App.TipoAttivitaScheda.View || anaStato == App.TipoAnagraficaStato.InUso;
        cBusinessObjects.VerificaSessione("Vigilanza", cmbData.SelectedValue.ToString(), IDVigilanza, IDCliente);


        AccediVigilanza_Click(IDVigilanza.ToString(), isSchedaReadOnly);
      }
    }


    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      ComboBoxItem typeItem = (ComboBoxItem)(cmbInizio.SelectedItem);
      ComboBoxItem typeItem2 = (ComboBoxItem)(cmbFine.SelectedItem);

      //if ( TipologiaAttivita == App.TipoAttivitaScheda.Edit && typeItem != null && ( typeItem2 == null || ( typeItem2 != null && cmbFine.SelectedValue.ToString() == "00:00" ) ) )
      //{
      //    MessageBox.Show("Inserire un'ora di fine");
      //    return;
      //}

      base.Close();
    }

    private void GestoreEvento_DatiCambiati(object sender, RoutedEventArgs e)
    {
      if (_InCaricamento)
        return;
      _DatiCambiati = true;
    }

    private void AccediVigilanzaDaVigilanza_Click(string ID, bool ReadOnly)
    {
      try
      {
        MasterFile mf = MasterFile.Create();
        Hashtable htSelected = mf.GetVerificaAssociataFromVigilanza(ID);
        WindowWorkAreaTree wWorkArea = new WindowWorkAreaTree();
        //Prisc
        try
        {
          wWorkArea.Owner = this;
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wSchedaVigilanza.AccediVigilanzaDaVigilanza_Click1 exception");
          string log = ex.Message;
        }
        wWorkArea.SelectedTreeSource = App.AppDataDataFolder + "\\" + htSelected["File"].ToString();
        wWorkArea.SelectedDataSource = App.AppDataDataFolder + "\\" + htSelected["FileData"].ToString();
        wWorkArea.ReadOnly = ReadOnly;
        wWorkArea.TipoAttivita = App.TipoAttivita.Vigilanza;
        wWorkArea.Cliente = (((Hashtable)(mf.GetAnagrafica(Convert.ToInt32(htSelected["Cliente"].ToString()))))["RagioneSociale"].ToString()) + " (C.F. " + (((Hashtable)(mf.GetAnagrafica(Convert.ToInt32(htSelected["Cliente"].ToString()))))["CodiceFiscale"].ToString()) + ")";
        wWorkArea.SessioneAlias = "";
        wWorkArea.SessioneFile = "";

        wWorkArea.IDTree = (Convert.ToInt32(App.TipoFile.Vigilanza)).ToString();
        wWorkArea.IDCliente = htSelected["Cliente"].ToString();
        wWorkArea.IDSessione = ID;

        foreach (Hashtable item in ((ArrayList)(mf.GetVigilanze(htSelected["Cliente"].ToString()))))
        {
          wWorkArea.SessioneFile += ((wWorkArea.SessioneFile == "") ? "" : "|") + App.AppDataDataFolder + "\\" + item["FileData"].ToString();
          wWorkArea.SessioneAlias += ((wWorkArea.SessioneAlias == "") ? "" : "|") + item["Data"].ToString();
          wWorkArea.SessioneID += ((wWorkArea.SessioneID == "") ? "" : "|") + item["ID"].ToString();
        }


        //aperto in sola lettura
        wWorkArea.ApertoInSolaLettura = TipologiaAttivita == App.TipoAttivitaScheda.View;

        wWorkArea.LoadTreeSource();
        wWorkArea.ShowDialog();

        //setto dati
        Hashtable ht = new Hashtable();
        ht.Add("Cliente", Convert.ToInt32(htSelected["Cliente"].ToString()));
        ht.Add("Data", dtpDataNomina.SelectedDate.Value.ToShortDateString());
        ht.Add("DataEsecuzione", ((dtpDataEsecuzione.SelectedDate != null) ? dtpDataEsecuzione.SelectedDate.Value.ToShortDateString() : dtpDataNomina.SelectedDate.Value.ToShortDateString()));
        ht.Add("DataEsecuzione_Fine", ((dtpDataEsecuzione_Fine.SelectedDate != null) ? dtpDataEsecuzione_Fine.SelectedDate.Value.ToShortDateString() : dtpDataNomina.SelectedDate.Value.ToShortDateString()));

        ComboBoxItem typeItem = (ComboBoxItem)(cmbInizio.SelectedItem);
        ht.Add("Inizio", typeItem.Content.ToString());

        ComboBoxItem typeItem2 = (ComboBoxItem)(cmbFine.SelectedItem);
        ht.Add("Fine", typeItem2.Content.ToString());

        //ht.Add("Fine", DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Minute.ToString().PadLeft(2, '0'));
        ht.Add("Luogo", txtLuogo.Text.Trim());
        ht.Add("Revisore", txtSindacoRevisore.Text.Trim());
        ht.Add("Presidente", txtPresidente.Text.Trim());
        ht.Add("Sindaco1", txtSindacoEffettivo1.Text.Trim());
        ht.Add("Sindaco2", txtSindacoEffettivo2.Text.Trim());
        ht.Add("AssisitoDa", txtAssistitoDa.Text.Trim());

        if (rdbCollegioSindacale.IsChecked == false && rdbRevisore.IsChecked == false && rdbSindacoUnico.IsChecked == false)
        {
          ht.Add("Composizione", (int)(App.TipoIncaricoComposizione.Sconosciuto));
        }
        else
        {
          if (rdbCollegioSindacale.IsChecked == true)
          {
            ht.Add("Composizione", (int)(App.TipoIncaricoComposizione.CollegioSindacale));
          }

          if (rdbRevisore.IsChecked == true)
          {
            ht.Add("Composizione", (int)(App.TipoIncaricoComposizione.Revisore));
          }

          if (rdbSindacoUnico.IsChecked == true)
          {
            ht.Add("Composizione", (int)(App.TipoIncaricoComposizione.SindacoUnico));
          }

        }

        mf.SetVigilanza(ht, Convert.ToInt32(ID), Convert.ToInt32(htSelected["Cliente"].ToString()));

        if (TipologiaAttivita != App.TipoAttivitaScheda.View)
        {
          int IDCliente = Convert.ToInt32(htClienti[cmbClienti.SelectedIndex].ToString());
          mf.SetAnafraficaStato(Convert.ToInt32(IDCliente), App.TipoAnagraficaStato.Disponibile);
        }

        functionCmbDataChanged(cmbData);

        //base.Close();
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaVigilanza.AccediVigilanzaDaVigilanza_Click2 exception");
        string log = ex.Message;
      }
    }

    bool ALreadyDone = false;

    private void GestoreEvento_ChiusuraFinestra(object sender, CancelEventArgs e)
    {
      if (IDClienteImport != "-1")
      {
        ;// MessageBox.Show( "La Sessione viene adesso generata\r\nDovrà essere selezionata nella tendina Destinazione della finestra di Import\r\nScegliere le CdL da Importare e premere Importa\r\nPer accedere alla sessione appena importata bisognerà chiudere la Sessione in corso e aprire quella nuova.", "Attenzione" );
        return;
      }

      MasterFile mf = MasterFile.Create();

      //Configuro stato
      if (TipologiaAttivita != App.TipoAttivitaScheda.View && cmbClienti.SelectedIndex != -1)
      {
        string IDCliente = htClienti[cmbClienti.SelectedIndex].ToString();
        mf.SetAnafraficaStato(Convert.ToInt32(IDCliente), App.TipoAnagraficaStato.Disponibile);
      }

      //apre treee
      IDVigilanza = App.MasterFile_NewID;

      try
      {
        IDVigilanza = Convert.ToInt32(htDate[cmbData.SelectedIndex].ToString());
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaVigilanza.GestoreEvento_ChiusuraFinestra exception");
        string log = ex.Message;
        return;
      }

      if (IDVigilanza == -1)
      {
        return;
      }

      Hashtable htSelected = mf.GetVerificaAssociataFromVigilanza(IDVigilanza.ToString());

      if (htSelected == null || htSelected.Count == 0)
      {
        return;
      }

      //if ( ALreadyDone == false && ( TipologiaAttivita == App.TipoAttivitaScheda.View || TipologiaAttivita == App.TipoAttivitaScheda.Edit ) && MessageBox.Show( "Si desidera proseguire con la sessione collegata di '4) Controllo Contabile'?", "Proseguire", MessageBoxButton.YesNo ) == MessageBoxResult.Yes )
      //{
      //    ALreadyDone = true;

      //    //disponibile: blocco cliente
      //    int IDCliente = Convert.ToInt32( htClienti[cmbClienti.SelectedIndex].ToString() );

      //    App.TipoAnagraficaStato anaStato = mf.GetAnafraficaStato( IDCliente );

      //    if ( anaStato == App.TipoAnagraficaStato.Disponibile && TipologiaAttivita != App.TipoAttivitaScheda.View )
      //        mf.SetAnafraficaStato( Convert.ToInt32( IDCliente ), App.TipoAnagraficaStato.InUso );

      //    {
      //        bool isSchedaReadOnly = TipologiaAttivita == App.TipoAttivitaScheda.View || anaStato == App.TipoAnagraficaStato.InUso;
      //        AccediVigilanzaDaVigilanza_Click( IDVigilanza.ToString(), isSchedaReadOnly );
      //    }

      //    e.Cancel = true;
      //    return;
      //}

      ComboBoxItem typeItem = (ComboBoxItem)(cmbInizio.SelectedItem);
      ComboBoxItem typeItem2 = (ComboBoxItem)(cmbFine.SelectedItem);

      //if ( TipologiaAttivita == App.TipoAttivitaScheda.Edit && typeItem != null && ( typeItem2 == null || ( typeItem2 != null && cmbFine.SelectedValue.ToString() == "00:00" ) ) )
      //{
      //    MessageBox.Show("Inserire un'ora di fine");
      //    e.Cancel = true;
      //    return;
      //}

      if (cmbClienti.SelectedIndex != -1 && TipologiaAttivita == App.TipoAttivitaScheda.Edit)
      {
        App.TipoAttivitaScheda OLDTipologiaAttivita = TipologiaAttivita;
        //TipologiaAttivita = App.TipoAttivitaScheda.Edit;
        buttonComando_Click(sender, new RoutedEventArgs());
        TipologiaAttivita = OLDTipologiaAttivita;
      }

      return;

      ////dati non modificati
      //if (!_DatiCambiati)
      //    return;

      ////dati modificati
      //Utilities u = new Utilities();
      //if (MessageBoxResult.No == u.AvvisoPerditaDati())
      //    e.Cancel = true;
    }

    private void rdbCollegioSindacale_Checked(object sender, RoutedEventArgs e)
    {
      if (lblSR == null)
      {
        return;
      }

      //controllo dati cambiati
      GestoreEvento_DatiCambiati(sender, e);

      //interfaccia
      if (rdbCollegioSindacale.IsChecked == true)
      {
        lblSR.Visibility = System.Windows.Visibility.Collapsed;
        GridSindato.Visibility = System.Windows.Visibility.Visible;
        txtSindacoRevisore.Visibility = System.Windows.Visibility.Collapsed;
        lblP.Visibility = System.Windows.Visibility.Visible;
        txtPresidente.Visibility = System.Windows.Visibility.Visible;
        //lblSE1.Visibility = System.Windows.Visibility.Visible;
        //txtSindacoEffettivo1.Visibility = System.Windows.Visibility.Visible;
        //lblSE2.Visibility = System.Windows.Visibility.Visible;
        //txtSindacoEffettivo2.Visibility = System.Windows.Visibility.Visible;
      }
      else
      {
        lblSR.Visibility = System.Windows.Visibility.Visible;

        if (rdbRevisore.IsChecked == true)
        {
          lblSR.Content = "Revisore";
        }
        else
        {
          lblSR.Content = "Sindaco Unico";
        }

        GridSindato.Visibility = System.Windows.Visibility.Collapsed;
        txtSindacoRevisore.Visibility = System.Windows.Visibility.Visible;
        lblP.Visibility = System.Windows.Visibility.Collapsed;
        txtPresidente.Visibility = System.Windows.Visibility.Collapsed;
        //lblSE1.Visibility = System.Windows.Visibility.Collapsed;
        //txtSindacoEffettivo1.Visibility = System.Windows.Visibility.Collapsed;
        //lblSE2.Visibility = System.Windows.Visibility.Collapsed;
        //txtSindacoEffettivo2.Visibility = System.Windows.Visibility.Collapsed;
      }
    }
    private void GestoreEvento_ComboEsercizio_Checked(object sender, CancelEventArgs e)
    {
      _DatiCambiati = true;
    }

    private void dtpDataNomina_MouseDown(object sender, MouseButtonEventArgs e)
    {
      //if(dtpDataNomina.IsHitTestVisible == false)
      //{
      //    MessageBox.Show( "..." );
      //}
    }

    private void dtpDataEsecuzione_MouseDown(object sender, MouseButtonEventArgs e)
    {
      //if(dtpDataEsecuzione.IsHitTestVisible == false)
      //{
      //    MessageBox.Show( "..." );
      //}
    }

    private void cmbPianificate_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      dtpDataNomina.Text = ((ComboBox)(sender)).SelectedValue.ToString();
    }

    private void GestoreEvento_DataCambiata(object sender, RoutedEventArgs e)
    {
      if (cmbData.SelectedValue == null)
      {
        return;
      }

      if (dtpDataNomina.ToString().Substring(0, 10) == cmbData.SelectedValue.ToString().Substring(0, 10)) return;

      MasterFile mf = MasterFile.Create();
      Utilities u = new Utilities();

      int IDCliente = Convert.ToInt32(htClienti[cmbClienti.SelectedIndex].ToString());
      IDVigilanza = Convert.ToInt32(htDate[cmbData.SelectedIndex].ToString());

      if (!u.ConvalidaDatiInterfaccia(dtpDataNomina, "Data mancante."))
        return;

      //Controllo che questa data non sia già stata presa
      if (!mf.CheckDoppio_Vigilanza(IDVigilanza, IDCliente, dtpDataNomina.SelectedDate.Value.ToShortDateString()))
      {
        MessageBox.Show("Data già presente per questo cliente");
        dtpDataNomina.Text = cmbData.SelectedValue.ToString();
        return;
      }

      Hashtable ht = new Hashtable();
      ht = mf.GetVigilanza(IDVigilanza.ToString());
      ht["Data"] = dtpDataNomina.SelectedDate.Value.ToShortDateString();

      //ht.Add( "Cliente", IDCliente );
      //ht.Add( "Data", dtpDataNomina.SelectedDate.Value.ToShortDateString() );
      //ht.Add( "DataEsecuzione", ( ( dtpDataEsecuzione.SelectedDate != null ) ? dtpDataEsecuzione.SelectedDate.Value.ToShortDateString() : dtpDataNomina.SelectedDate.Value.ToShortDateString() ) );

      //ComboBoxItem typeItem = (ComboBoxItem)( cmbInizio.SelectedItem );
      //ht.Add( "Inizio", typeItem.Content.ToString() );

      //ComboBoxItem typeItem2 = (ComboBoxItem)( cmbFine.SelectedItem );
      //ht.Add( "Fine", typeItem2.Content.ToString() );

      //ht.Add( "Fine", DateTime.Now.Hour.ToString().PadLeft( 2, '0' ) + ":" + DateTime.Now.Minute.ToString().PadLeft( 2, '0' ) );
      //ht.Add( "Luogo", txtLuogo.Text.Trim() );
      //ht.Add( "Revisore", txtSindacoRevisore.Text.Trim() );
      //ht.Add( "Presidente", txtPresidente.Text.Trim() );
      //ht.Add( "Sindaco1", txtSindacoEffettivo1.Text.Trim() );
      //ht.Add( "Sindaco2", txtSindacoEffettivo2.Text.Trim() );
      //ht.Add( "AssisitoDa", txtAssistitoDa.Text.Trim() );

      //if ( rdbCollegioSindacale.IsChecked == false && rdbRevisore.IsChecked == false && rdbSindacoUnico.IsChecked == false )
      //{
      //    ht.Add( "Composizione", (int)( App.TipoIncaricoComposizione.Sconosciuto ) );
      //}
      //else
      //{
      //    if ( rdbCollegioSindacale.IsChecked == true )
      //    {
      //        ht.Add( "Composizione", (int)( App.TipoIncaricoComposizione.CollegioSindacale ) );
      //    }

      //    if ( rdbRevisore.IsChecked == true )
      //    {
      //        ht.Add( "Composizione", (int)( App.TipoIncaricoComposizione.Revisore ) );
      //    }

      //    if ( rdbSindacoUnico.IsChecked == true )
      //    {
      //        ht.Add( "Composizione", (int)( App.TipoIncaricoComposizione.SindacoUnico ) );
      //    }

      //}

      mf.SetVigilanza(ht, IDVigilanza, IDCliente);

      GestoreEvento_DatiCambiati(sender, e);
    }
  }
}
