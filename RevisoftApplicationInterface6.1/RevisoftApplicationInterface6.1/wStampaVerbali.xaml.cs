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
  public partial class wStampaVerbali : Window
  {

    private int OldSelectedCmbClienti = -1;

    Hashtable htClienti = new Hashtable();
    Hashtable htDate = new Hashtable();

    public string selectedCliente = "";
    public string selectedSession = "";


    public wStampaVerbali()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
    }

    public void inizializza()
    {
      //interfaccia 
      ConfiguraMaschera();
      cmbClienti.Focus();
    }

    public void ConfiguraMaschera()
    {
      MasterFile mf = MasterFile.Create();

      int index = 0;

      int selectedIndex = -1;
      if (cmbClienti.Items.Count != 0)
      {
        selectedIndex = cmbClienti.SelectedIndex;
        cmbClienti.Items.Clear();
        htClienti.Clear();
      }

      List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>();

      foreach (Hashtable item in mf.GetAnagrafiche())
      {
        bool tbe = false;

        if (selectedCliente != "")
        {
          if (item["ID"].ToString() == selectedCliente)
          {
            myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), item["RagioneSociale"].ToString()));

            foreach (Hashtable item2 in mf.GetVerifiche(selectedCliente))
            {
              if (item2["ID"].ToString() == selectedSession)
              {
                cmbData.Items.Add(item2["Data"].ToString());
                cmbData.SelectedIndex = 0;
                htDate.Add(0, item2["ID"].ToString());
                tbe = true;
                break;
              }
            }

            if (tbe)
            {
              break;
            }

            foreach (Hashtable item2 in mf.GetVigilanze(selectedCliente))
            {
              if (item2["ID"].ToString() == selectedSession)
              {
                cmbData.Items.Add(item2["Data"].ToString());
                cmbData.SelectedIndex = 0;
                htDate.Add(0, item2["ID"].ToString());
                break;
              }
            }

            if (tbe)
            {
              break;
            }
          }
        }
        else
        {
          if (mf.GetVerifiche(item["ID"].ToString()).Count == 0 && mf.GetVigilanze(item["ID"].ToString()).Count == 0)
          {
            continue;
          }

          string cliente = item["RagioneSociale"].ToString();

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

      string IDCliente = ((selectedCliente != "") ? selectedCliente : mf.GetClienteFissato());
      foreach (DictionaryEntry item in htClienti)
      {
        if (item.Value.ToString() == IDCliente)
        {
          cmbClienti.SelectedIndex = Convert.ToInt32(item.Key.ToString());
        }
      }
    }

    private void cmbClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (selectedCliente != "")
      {
        return;
      }

      //interfaccia
      functionCmbClientiChanged(((ComboBox)sender));
      cmbData.Focus();
    }

    private void functionCmbClientiChanged(ComboBox cmb)
    {
      if (selectedCliente != "")
      {
        return;
      }

      cmbData.SelectedIndex = -1;

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

          ArrayList alreadydone = new ArrayList();

          foreach (Hashtable item in mf.GetVerifiche(IDCliente))
          {
            myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), (item["Data"].ToString())));
            alreadydone.Add((item["Data"].ToString()));
          }

          foreach (Hashtable item in mf.GetVigilanze(IDCliente))
          {
            if (!alreadydone.Contains((item["Data"].ToString())))
            {
              myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), (item["Data"].ToString())));
              alreadydone.Add((item["Data"].ToString()));
            }
          }

          myList.Sort
          (
            delegate (KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
            {
              try
              {

                return Convert.ToDateTime(nextPair.Value).CompareTo(Convert.ToDateTime(firstPair.Value));
                //return nextPair.Value.ToString().CompareTo(firstPair.Value.ToString());
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
        }
        catch (Exception ex)
        {
          string log = ex.Message;
          cmbData.IsEnabled = false;
        }
      }
    }

    private string ConvertInteger(string valore)
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

    private void buttonStampa_Click(object sender, RoutedEventArgs e)
    {
      //controllo selezione clienti
      if (cmbClienti.SelectedIndex == -1)
      {
        MessageBox.Show("selezionare un cliente");
        return;
      }

      if (cmbData.SelectedIndex == -1)
      {
        MessageBox.Show("selezionare una sessione");
        return;
      }

      if (rdbVerbale.IsChecked == false && rdbMemorandum.IsChecked == false)
      {
        MessageBox.Show("selezionare verbale o memorandum");
        return;
      }

      if (rdbDescrittiva.IsChecked != true && rdbCompleta.IsChecked != true)
      {
        MessageBox.Show("Scegliere il tipo di stampa");
        return;
      }

      //Process wait - START
      //ProgressWindow pw = new ProgressWindow();

      MasterFile mf = MasterFile.Create();

      string IDCliente = htClienti[cmbClienti.SelectedIndex].ToString();
      
      this.selectedCliente = IDCliente;


      string anno = cmbData.SelectedValue.ToString().Split('/')[2];

      Hashtable cliente = mf.GetAnagrafica(Convert.ToInt32(IDCliente));

      XmlDataProviderManager _t = null;
 
      Hashtable valueshere = new Hashtable();

      //WordLib wl = new WordLib();
      RTFLib wl = new RTFLib();

      //wl.Verbali = true;

      #region Dati da revisione

      //XmlDataProviderManager _y = null;

      string FileRevisione = mf.GetRevisioneFromEsercizio(IDCliente, anno);



      string RagioneSociale = "Dato Mancante: Compilare Carta di Lavoro 2.1.1";
      string Indirizzo = "";
      string REA = "";
      string CapitaleSociale = "Dato Mancante: Compilare Carta di Lavoro 2.1.5";
      string ids = "";
      foreach (Hashtable item in mf.GetVerifiche(IDCliente))
      {
        if (cmbData.SelectedValue.ToString() == (item["Data"].ToString()))
        {
          ids = item["ID"].ToString();
        }
      }
      this.selectedSession = ids;

      cBusinessObjects.idcliente = int.Parse(IDCliente);
      RagioneSociale = cBusinessObjects.GetRagioneSociale();
      Indirizzo = cBusinessObjects.GetIndirizzo();
      REA = cBusinessObjects.GetREA();
      CapitaleSociale = cBusinessObjects.GetCapitaleSociale();

      #endregion

      wl.Watermark = false;
      wl.TabelleSenzaRigheVuote = true;
      wl.SenzaStampareTitoli = true;
      wl.TitoloVerbale = false;

      if (rdbDescrittiva.IsChecked == true)
      {
        wl.StampaDescrittiva = true;
      }

      wl.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
      wl.Open(new Hashtable(), cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), cmbData.SelectedValue.ToString(), "", true, true, IDCliente);

      if (rdbVerbale.IsChecked == true)
      {
        //if ( chk4.IsChecked == false && Chk5.IsChecked == false )
        //{
        //    MessageBox.Show( "selezionare almeno una tra l'area 4 o 5" );
        //    pw.Close();
        //    return;
        //}

        //if ( chk4.IsChecked == true)
        //{
        string partial_rtf_text = "";

        if (rdbMemorandum.IsChecked == false)
        {
          foreach (Hashtable item in mf.GetVerifiche(IDCliente))
          {
            if (cmbData.SelectedValue.ToString() == (item["Data"].ToString()))
            {
              valueshere = item;
              _t = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["File"].ToString());
     
            }
          }

          bool notfoundv = false;
          if (valueshere.Count == 0)
          {
            notfoundv = true;
          }
          else
          {
            valueshere.Add("PeriodoPianificato", "");
            DateTime dti_o = Convert.ToDateTime(valueshere["Data"]);

            foreach (Hashtable item in mf.GetPianificazioniVerifiche(IDCliente))
            {
              DateTime dti = Convert.ToDateTime(item["DataInizio"]);
              DateTime dtf = Convert.ToDateTime(item["DataFine"]);

              if (dti_o.CompareTo(dti) > 0 && dti_o.CompareTo(dtf) < 0)
              {
                valueshere["PeriodoPianificato"] = item["DataInizio"] + " - " + item["DataFine"];
                break;
              }
            }

            partial_rtf_text = "\\pard\\keepn\\f0 ";

            string titolohere = "VERBALE DELLA VERIFICA PERIODICA DEL ";
            try
            {
              //if (_t.Document.SelectSingleNode("/Tree//Node[@ID='614']/Sessioni/Sessione[@Selected='#AA82BDE4']").Attributes["Stato"].Value == "2" || _t.Document.SelectSingleNode("/Tree//Node[@ID='615']/Sessioni/Sessione[@Selected='#AA82BDE4']").Attributes["Stato"].Value == "2" || _t.Document.SelectSingleNode("/Tree//Node[@ID='616']/Sessioni/Sessione[@Selected='#AA82BDE4']").Attributes["Stato"].Value == "2" || _t.Document.SelectSingleNode("/Tree//Node[@ID='617']/Sessioni/Sessione[@Selected='#AA82BDE4']").Attributes["Stato"].Value == "2" || _t.Document.SelectSingleNode("/Tree//Node[@ID='618']/Sessioni/Sessione[@Selected='#AA82BDE4']").Attributes["Stato"].Value == "2")
              //{
              //  titolohere = "VERBALE DI INSEDIAMENTO DEL ";
              //}
              XmlNode node6;
              string selection,strNode;
              int i;

              selection = "/Tree//Node[@ID='<idnode>']/Sessioni/Sessione[@Selected='#AA82BDE4']";
              for (i = 0; i < 5; i++)
              {
                strNode = string.Format("{0}", 614 + i);
                strNode = selection.Replace("<idnode>", strNode);
                node6 = _t.Document.SelectSingleNode(strNode);
                if (node6 == null) continue;
                if (node6.Attributes["Stato"].Value == "2")
                {
                  titolohere = "VERBALE DI INSEDIAMENTO DEL ";
                  break;
                }
              }
            }
            catch (Exception ex)
            {
              string log = ex.Message;
            }

            wl.SetIntestazione(RagioneSociale, Indirizzo, CapitaleSociale, REA, titolohere, valueshere);

            partial_rtf_text += "\\trowd\\cellx9900 \\fs24 \\qc\\b ATTIVITA' DI CONTROLLO CONTABILE \\b0\\cell\\row";

            string TestoHere = "L'organo di revisione procede alla verifica secondo la previsione dell'art.14, primo comma, lettera b), del D. Lgs. 27 Gennaio 2010 n° 39, in conformità al Principio di revisione SA Italia 250B.";

            partial_rtf_text += "\\trowd\\cellx9900 \\qj " + TestoHere + " \\cell\\row";

            TestoHere = "Per lo svolgimento delle attività di revisione e controllo viene utilizzata una procedura informatica che produce carte di lavoro, nelle quali vengono inseriti i dati tabellari raccolti nel corso della sessione, i commenti e le osservazioni annotati, nonché vengono allegati documenti in qualsiasi formato immateriale.";
            partial_rtf_text += "\\trowd\\cellx9900 \\qj " + TestoHere + " \\cell\\row";

            bool esistepianificazione = false;
            ArrayList al = mf.GetPianificazioniVerifiche(IDCliente);
            foreach (Hashtable itemHT in al)
            {
              string filedata = App.AppDataDataFolder + "\\" + itemHT["FileData"].ToString();
              //if ((new FileInfo(filedata)).Exists)
              //{
              //  XmlDataProviderManager ALXTPP = new XmlDataProviderManager(filedata);

              //  foreach (XmlNode itemXPP in ALXTPP.Document.SelectNodes("//Dato[@ID=\"100013\"]/Valore/Pianificazione"))
              //  {
              //    if (itemXPP.Attributes["Data"].Value == cmbData.SelectedValue.ToString())
              //    {
              //      esistepianificazione = true;
              //      break;
              //    }
              //  }
              //}
              XmlDataProviderManager ALXTPP = new XmlDataProviderManager(filedata);
              if (ALXTPP.Document != null)
              {
                foreach (XmlNode itemXPP in ALXTPP.Document.SelectNodes("//Dato[@ID=\"100013\"]/Valore/Pianificazione"))
                {
                  if (itemXPP.Attributes["Data"].Value == cmbData.SelectedValue.ToString())
                  {
                    esistepianificazione = true;
                    break;
                  }
                }
              }

              if (esistepianificazione == true)
              {
                break;
              }
            }

            if (esistepianificazione == true)
            {
              //Inserisco il testo per la pianificazione
              TestoHere = "L'attività di controllo è stata pianificata con la carta di lavoro denominata Pianificazione; la sessione avviata rientra fra quelle pianificate.";
              partial_rtf_text += "\\trowd\\cellx9900 \\qj " + TestoHere + " \\line \\cell\\row";
            }

            if (rdbDescrittiva.IsChecked == true)
            {
              TestoHere = "Per economia di lavoro nel presente verbale verrà dato conto delle carte di lavoro utilizzate dalle quali verranno estrapolati le sole osservazioni e i commenti; per i dati raccolti ed inseriti anche in forma tabellare si farà riferimento alle carte di lavoro, conservate con modalità informatica, che si intendono parte integrante del presente verbale. Parimenti i documenti raccolti vengono associati alle carte di lavoro e conservati in modalità informatiche.";
              partial_rtf_text += "\\trowd\\cellx9900 \\qj " + TestoHere + " \\line \\cell\\row";
            }

            TestoHere = "Vengono eseguite le seguenti verifiche: \\line ";
            partial_rtf_text += "\\trowd\\cellx9900 \\qj " + TestoHere + " \\cell\\row";

           // partial_rtf_text += "\\par\n";

            wl.InsertRtf(partial_rtf_text);
            
          

            printsingle(cliente, _t,  (Convert.ToInt32(App.TipoFile.Verifica)).ToString(), wl, valueshere["ID"].ToString(), IDCliente, "", valueshere["FileData"].ToString());

          }

          valueshere.Clear();

          foreach (Hashtable item in mf.GetVigilanze(IDCliente))
          {
            if (cmbData.SelectedValue.ToString() == (item["Data"].ToString()))
            {
              valueshere = item;
              _t = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["File"].ToString());
           
            }
          }

          if (valueshere.Count > 0)
          {
            if (notfoundv == false)
            {
              partial_rtf_text = "\\pard\\keepn\\f0 ";
              partial_rtf_text += "\\trowd\\cellx9900 \\fs24 \\qc\\b ATTIVITA' DI VIGILANZA \\b0\\cell\\row";

              string TestoHere = "L’organo di controllo di cui agli artt. 2397 e seguenti Cod. Civ. procede alla verifica prescritta dall'articolo 2404 Cod. Civ.; l'attività da svolgere concernerà i doveri indicati nel primo comma dell'articolo 2403 Cod. Civ., esercitando i poteri di cui all'art. 2404 bis Cod. Civ.";

              partial_rtf_text += "\\trowd\\cellx9900 \\qj " + TestoHere + " \\cell\\row";
              partial_rtf_text += "\\trowd\\cellx9900 \\qj \\line \\cell\\row";

              partial_rtf_text += "\n";
            }
            else
            {
              string titolohere2 = "VERBALE DELLA VERIFICA DEL ";
              try
              {
                if (_t.Document.SelectSingleNode("/Tree//Node[@ID='614']/Sessioni/Sessione[@Selected='#AA82BDE4']").Attributes["Stato"].Value == "2")
                {
                  titolohere2 = "VERBALE DI INSEDIAMENTO DEL ";
                }
              }
              catch (Exception ex)
              {
                string log = ex.Message;
              }

              wl.SetIntestazione(RagioneSociale, Indirizzo, CapitaleSociale, REA, titolohere2, valueshere);

              partial_rtf_text += "\\trowd\\cellx9900 \\fs24 \\qc\\b ATTIVITA' DI VIGILANZA \\b0\\cell\\row";

              string TestoHere = "L’organo di controllo di cui agli artt. 2397 e seguenti Cod. Civ. procede alla verifica prescritta dall'articolo 2404 Cod. Civ.; l'attività da svolgere concernerà i doveri indicati nel primo comma dell'articolo 2403 Cod. Civ., esercitando i poteri di cui all'art. 2404 bis Cod. Civ.";

              partial_rtf_text += "\\trowd\\cellx9900 \\qj " + TestoHere + " \\cell\\row";
              partial_rtf_text += "\\trowd\\cellx9900 \\qj \\line \\cell\\row";

              partial_rtf_text += "\n";
            }


            wl.InsertRtf(partial_rtf_text);

            printsingle(cliente, _t, (Convert.ToInt32(App.TipoFile.Vigilanza)).ToString(), wl, valueshere["ID"].ToString(), IDCliente, "", valueshere["FileData"].ToString());
          }
          else
          {
            MessageBox.Show("Verifica per attività di vigilanza non trovata");
          }
        }
        else
        {
          foreach (Hashtable item in mf.GetVigilanze(IDCliente))
          {
            if (cmbData.SelectedValue.ToString() == (item["Data"].ToString()))
            {
              valueshere = item;
              _t = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["File"].ToString());
     
            }
          }

          if (valueshere.Count > 0)
          {
            valueshere.Add("PeriodoPianificato", "");
            DateTime dti_o = Convert.ToDateTime(valueshere["Data"]);

            foreach (Hashtable item in mf.GetPianificazioniVigilanze(IDCliente))
            {
              DateTime dti = Convert.ToDateTime(item["DataInizio"]);
              DateTime dtf = Convert.ToDateTime(item["DataFine"]);

              if (dti_o.CompareTo(dti) > 0 && dti_o.CompareTo(dtf) < 0)
              {
                valueshere["PeriodoPianificato"] = item["DataInizio"] + " - " + item["DataFine"];
                break;
              }
            }

            partial_rtf_text = "\\pard\\keepn\\f0 ";

            if (valueshere.Count == 0)
            {
              MessageBox.Show("Verifica per attività di vigilanza non trovata");
            }
            else
            {
              string titolohere3 = "VERBALE DELLA VERIFICA DEL ";
              try
              {
                if (_t.Document.SelectSingleNode("/Tree//Node[@ID='614']/Sessioni/Sessione[@Selected='#AA82BDE4']").Attributes["Stato"].Value == "2")
                {
                  titolohere3 = "VERBALE DI INSEDIAMENTO DEL ";
                }
              }
              catch (Exception ex)
              {
                string log = ex.Message;
              }

              wl.SetIntestazione(RagioneSociale, Indirizzo, CapitaleSociale, REA, titolohere3, valueshere);

              partial_rtf_text += "\\trowd\\cellx9900 \\fs24 \\qc\\b ATTIVITA' DI VIGILANZA \\b0\\cell\\row";

              string TestoHere = "L’organo di controllo di cui agli artt. 2397 e seguenti Cod. Civ. procede alla verifica prescritta dall'articolo 2404 Cod. Civ.; l'attività da svolgere concernerà i doveri indicati nel primo comma dell'articolo 2403 Cod. Civ., esercitando i poteri di cui all'art. 2404 bis Cod. Civ.";

              partial_rtf_text += "\\trowd\\cellx9900 \\qj " + TestoHere + " \\cell\\row";
              partial_rtf_text += "\\trowd\\cellx9900 \\qj \\line \\cell\\row";

              bool esistepianificazione = false;
              ArrayList al = mf.GetPianificazioniVigilanze(IDCliente);
              foreach (Hashtable itemHT in al)
              {
                string filedata = App.AppDataDataFolder + "\\" + itemHT["FileData"].ToString();
                if ((new FileInfo(filedata)).Exists)
                {
                  XmlDataProviderManager ALXTPP = new XmlDataProviderManager(filedata);

                  foreach (XmlNode itemXPP in ALXTPP.Document.SelectNodes("//Dato[@ID=\"100003\"]/Valore/Pianificazione"))
                  {
                    if (itemXPP.Attributes["Data"].Value == cmbData.SelectedValue.ToString())
                    {
                      esistepianificazione = true;
                      break;
                    }
                  }
                }

                if (esistepianificazione == true)
                {
                  break;
                }
              }

              //partial_rtf_text += "\\trowd\\cellx9900 \\qj " + TestoHere + " \\cell\\row";

              TestoHere = "Vengono eseguite le seguenti verifiche: \\line ";
              partial_rtf_text += "\\trowd\\cellx9900 \\qj " + TestoHere + " \\cell\\row";

           //   partial_rtf_text += "\\par\n";

              wl.InsertRtf(partial_rtf_text);

              printsingle(cliente, _t,  (Convert.ToInt32(App.TipoFile.Vigilanza)).ToString(), wl, valueshere["ID"].ToString(), IDCliente, "", valueshere["FileData"].ToString());
            }
          }

          #region memorandum contemporaneo
          RTFLib wl2 = new RTFLib();
          wl2.Watermark = false;
          wl2.TabelleSenzaRigheVuote = true;
          wl2.SenzaStampareTitoli = true;
          wl2.TitoloVerbale = false;

          if (rdbDescrittiva.IsChecked == true)
          {
            wl2.StampaDescrittiva = true;
          }

          wl2.TemplateFileCompletePath = App.AppTemplateStampaNoLogo;
          wl2.Open(new Hashtable(), cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), cmbData.SelectedValue.ToString(), "", true, true, IDCliente);
          foreach (Hashtable item in mf.GetVerifiche(IDCliente))
          {
            if (cmbData.SelectedValue.ToString() == (item["Data"].ToString()))
            {
              valueshere = item;
              _t = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["File"].ToString());
  
            }
          }

          if (!valueshere.Contains("PeriodoPianificato"))
          {
            valueshere.Add("PeriodoPianificato", "");
          }
          DateTime dti_o2 = Convert.ToDateTime(valueshere["Data"]);

          foreach (Hashtable item in mf.GetPianificazioniVerifiche(IDCliente))
          {
            DateTime dti = Convert.ToDateTime(item["DataInizio"]);
            DateTime dtf = Convert.ToDateTime(item["DataFine"]);

            if (dti_o2.CompareTo(dti) > 0 && dti_o2.CompareTo(dtf) < 0)
            {
              valueshere["PeriodoPianificato"] = item["DataInizio"] + " - " + item["DataFine"];
              break;
            }
          }

          if (valueshere.Count == 0)
          {
            MessageBox.Show("Verifica per controllo contabile non trovata");
          }
          else
          {
            wl2.SetIntestazione(RagioneSociale, Indirizzo, CapitaleSociale, REA, "MEMORANDUM DELLA VERIFICA PERIODICA DEL ", valueshere);

            string partial_rtf_text2 = "\\pard\\keepn\\f0 ";

            partial_rtf_text2 += "\\trowd\\cellx9900 \\fs24 \\qc\\b ATTIVITA' DI CONTROLLO CONTABILE \\b0\\cell\\row";

            string TestoHere2 = "L'organo di revisione procede alla verifica secondo la previsione dell'art.14, primo comma, lettera b), del D. Lgs. 27 Gennaio 2010 n° 39, in conformità al Principio di revisione SA Italia 250B.";

            partial_rtf_text2 += "\\trowd\\cellx9900 \\qj " + TestoHere2 + " \\cell\\row";

            TestoHere2 = "Per lo svolgimento delle attività di revisione e controllo viene utilizzata una procedura informatica che produce carte di lavoro, nelle quali vengono inseriti i dati tabellari raccolti nel corso della sessione, i commenti e le osservazioni annotati, nonché vengono allegati documenti in qualsiasi formato immateriale.";
            partial_rtf_text2 += "\\trowd\\cellx9900 \\qj " + TestoHere2 + " \\cell\\row";

            bool esistepianificazione2 = false;
            ArrayList al2 = mf.GetPianificazioniVerifiche(IDCliente);
            foreach (Hashtable itemHT in al2)
            {
              string filedata = App.AppDataDataFolder + "\\" + itemHT["FileData"].ToString();
              if ((new FileInfo(filedata)).Exists)
              {
                XmlDataProviderManager ALXTPP = new XmlDataProviderManager(filedata);

                foreach (XmlNode itemXPP in ALXTPP.Document.SelectNodes("//Dato[@ID=\"100013\"]/Valore/Pianificazione"))
                {
                  if (itemXPP.Attributes["Data"].Value == cmbData.SelectedValue.ToString())
                  {
                    esistepianificazione2 = true;
                    break;
                  }
                }
              }

              if (esistepianificazione2 == true)
              {
                break;
              }
            }

            if (esistepianificazione2 == true)
            {
              //Inserisco il testo per la pianificazione
              TestoHere2 = "L'attività di controllo è stata pianificata con la carta di lavoro denominata Pianificazione; la sessione avviata rientra fra quelle pianificate.";
              partial_rtf_text2 += "\\trowd\\cellx9900 \\qj " + TestoHere2 + " \\line \\cell\\row";
            }

            if (rdbDescrittiva.IsChecked == true)
            {
              TestoHere2 = "Per economia di lavoro nel presente memorandum verrà dato conto delle carte di lavoro utilizzate dalle quali verranno estrapolati le sole osservazioni e i commenti; per i dati raccolti ed inseriti anche in forma tabellare si farà riferimento alle carte di lavoro, conservate con modalità informatica, che si intendono parte integrante del presente verbale. Parimenti i documenti raccolti vengono associati alle carte di lavoro e conservati in modalità informatiche.";
              partial_rtf_text2 += "\\trowd\\cellx9900 \\qj " + TestoHere2 + " \\line \\cell\\row";
            }

            TestoHere2 = "Vengono eseguite le seguenti verifiche: \\line ";
            partial_rtf_text2 += "\\trowd\\cellx9900 \\qj " + TestoHere2 + " \\cell\\row";

         //   partial_rtf_text2 += "\\par\n";

            wl2.InsertRtf(partial_rtf_text2);

            printsingle(cliente, _t,  (Convert.ToInt32(App.TipoFile.Verifica)).ToString(), wl2, valueshere["ID"].ToString(), IDCliente, "", valueshere["FileData"].ToString());
          }

          wl2.LastParagraph(valueshere);
          wl2.SetFilename(App.AppTempFolder + "\\" + cliente["RagioneSociale"].ToString() + "_Memorandum_" + cmbData.SelectedValue.ToString().Replace("/", ""));
          wl2.Save("");
          wl2.Close();
          #endregion
        }

      }
      else if (rdbMemorandum.IsChecked == true)
      {
        foreach (Hashtable item in mf.GetVerifiche(IDCliente))
        {
          if (cmbData.SelectedValue.ToString() == (item["Data"].ToString()))
          {
            valueshere = item;
            _t = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["File"].ToString());
 
          }
        }

        valueshere.Add("PeriodoPianificato", "");
        DateTime dti_o = Convert.ToDateTime(valueshere["Data"]);

        foreach (Hashtable item in mf.GetPianificazioniVerifiche(IDCliente))
        {
          DateTime dti = Convert.ToDateTime(item["DataInizio"]);
          DateTime dtf = Convert.ToDateTime(item["DataFine"]);

          if (dti_o.CompareTo(dti) > 0 && dti_o.CompareTo(dtf) < 0)
          {
            valueshere["PeriodoPianificato"] = item["DataInizio"] + " - " + item["DataFine"];
            break;
          }
        }

        if (valueshere.Count == 0)
        {
          MessageBox.Show("Verifica per controllo contabile non trovata");
        }
        else
        {
          wl.SetIntestazione(RagioneSociale, Indirizzo, CapitaleSociale, REA, "MEMORANDUM DELLA VERIFICA PERIODICA DEL ", valueshere);

          string partial_rtf_text = "\\pard\\keepn\\f0 ";

          partial_rtf_text += "\\trowd\\cellx9900 \\fs24 \\qc\\b ATTIVITA' DI CONTROLLO CONTABILE \\b0\\cell\\row";

          string TestoHere = "L'organo di revisione procede alla verifica secondo la previsione dell'art.14, primo comma, lettera b), del D. Lgs. 27 Gennaio 2010 n° 39, in conformità al Principio di revisione SA Italia 250B.";

          partial_rtf_text += "\\trowd\\cellx9900 \\qj " + TestoHere + " \\cell\\row";

          TestoHere = "Per lo svolgimento delle attività di revisione e controllo viene utilizzata una procedura informatica che produce carte di lavoro, nelle quali vengono inseriti i dati tabellari raccolti nel corso della sessione, i commenti e le osservazioni annotati, nonché vengono allegati documenti in qualsiasi formato immateriale.";
          partial_rtf_text += "\\trowd\\cellx9900 \\qj " + TestoHere + " \\cell\\row";

          bool esistepianificazione = false;
          ArrayList al = mf.GetPianificazioniVerifiche(IDCliente);
          foreach (Hashtable itemHT in al)
          {
            string filedata = App.AppDataDataFolder + "\\" + itemHT["FileData"].ToString();
            if ((new FileInfo(filedata)).Exists)
            {
              XmlDataProviderManager ALXTPP = new XmlDataProviderManager(filedata);

              foreach (XmlNode itemXPP in ALXTPP.Document.SelectNodes("//Dato[@ID=\"100013\"]/Valore/Pianificazione"))
              {
                if (itemXPP.Attributes["Data"].Value == cmbData.SelectedValue.ToString())
                {
                  esistepianificazione = true;
                  break;
                }
              }
            }

            if (esistepianificazione == true)
            {
              break;
            }
          }

          if (esistepianificazione == true)
          {
            //Inserisco il testo per la pianificazione
            TestoHere = "L'attività di controllo è stata pianificata con la carta di lavoro denominata Pianificazione; la sessione avviata rientra fra quelle pianificate.";
            partial_rtf_text += "\\trowd\\cellx9900 \\qj " + TestoHere + " \\line \\cell\\row";
          }

          if (rdbDescrittiva.IsChecked == true)
          {
            TestoHere = "Per economia di lavoro nel presente memorandum verrà dato conto delle carte di lavoro utilizzate dalle quali verranno estrapolati le sole osservazioni e i commenti; per i dati raccolti ed inseriti anche in forma tabellare si farà riferimento alle carte di lavoro, conservate con modalità informatica, che si intendono parte integrante del presente verbale. Parimenti i documenti raccolti vengono associati alle carte di lavoro e conservati in modalità informatiche.";
            partial_rtf_text += "\\trowd\\cellx9900 \\qj " + TestoHere + " \\line \\cell\\row";
          }

          TestoHere = "Vengono eseguite le seguenti verifiche: \\line ";
          partial_rtf_text += "\\trowd\\cellx9900 \\qj " + TestoHere + " \\cell\\row";

         // partial_rtf_text += "\\par\n";

          wl.InsertRtf(partial_rtf_text);

          printsingle(cliente, _t, (Convert.ToInt32(App.TipoFile.Verifica)).ToString(), wl, valueshere["ID"].ToString(), IDCliente, "", valueshere["FileData"].ToString());
        }
      }
      else
      {
        //pw.Close();
        return;
      }

      wl.LastParagraph(valueshere);
      string str;
      str = App.AppTempFolder + "\\" + cliente["RagioneSociale"].ToString() + "_VerbaleVerificaPeriodica_" + cmbData.SelectedValue.ToString().Replace("/", "");
      str = str.Replace("*", "_");
      wl.SetFilename(str);
      wl.Save("");
      wl.Close();

      //Process wait - STOP
      //pw.Close();
    }

    //private void printsingle(Hashtable cliente, XmlDataProviderManager TreeXmlProvider, XmlDataProviderManager _x, string IDTree, WordLib wl, string IDSessione, string IDCliente)
    private void printsingle(Hashtable cliente, XmlDataProviderManager TreeXmlProvider,  string IDTree, RTFLib wl, string IDSessione, string IDCliente, string AdditivaTitolo, string nomefile)
    {
     this.selectedSession = IDSessione;
      RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl,  IDTree, IDSessione, IDCliente, AdditivaTitolo, nomefile);

      istobecompleteforprinting = false;

      if (RecursiveCheck(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"),IDTree))
      {
        wl.AddTitleDaCompletare();

        RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl,  IDTree, IDSessione, IDCliente, AdditivaTitolo, nomefile);
      }

      istobecompleteforprinting = true;
    }

    //private bool isreporttobeprinted = true;

    bool istobecompleteforprinting = true;
    public bool printall = false;

  
    private bool RecursiveCheck(XmlNode node, string IDTree)
    {
      bool returnvalue = false;
      if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
      {
        if (node.Attributes["ID"] != null)
        {
          if (printall) return true;

          string stato= cBusinessObjects.GetStato(int.Parse(node.Attributes["ID"].Value), selectedCliente, selectedSession,IDTree);

          if (node.Attributes["Report"].Value == "True" || (stato != "" && stato == ((istobecompleteforprinting) ? ((Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString()) : ((Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString()))))
            {
                return true;
            }

                    //    XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");
                    //  if (node.Attributes["Report"] != null && NodoDato != null && NodoDato.Attributes["Stato"] != null)
                    //        {
                    //        if (node.Attributes["Report"].Value == "True" || (NodoDato.Attributes["Stato"] != null && NodoDato.Attributes["Stato"].Value == ((istobecompleteforprinting) ? ((Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString()) : ((Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString()))))
                    // {
                    //        return true;
                    //      }
                    //      }
                }
            }
      else
      {
        if (node.ParentNode.Name != "Tree")
        {
          foreach (XmlNode item in node.ChildNodes)
          {
            if (item.Name == "Node")
            {
              returnvalue = RecursiveCheck(item,IDTree);
              if (returnvalue)
              {
                return true;
              }
            }
          }
        }
      }
      return returnvalue;
    }

   
    //private void RecursiveNode(XmlNode node, WordLib wl, XmlDataProviderManager _x, string IDTree, string IDSessione, string IDCliente)
    private void RecursiveNode(XmlNode node, RTFLib wl, string IDTree, string IDSessione, string IDCliente, string AdditivaTitolo, string nomefile)
    {
      if (node == null)
      {
        return;
      }

      if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
      {
        if (RecursiveCheck(node,IDTree))
        {

          wl.Add(node, IDCliente, IDTree, IDSessione, nomefile);
        }
      }
      else
      {
        if (node.ParentNode.Name == "Tree" || RecursiveCheck(node,IDTree))
        {
          wl.AddTitle(node.Attributes["Codice"].Value + " " + node.Attributes["Titolo"].Value + AdditivaTitolo, node.ParentNode.Name != "Tree");

          foreach (XmlNode item in node.ChildNodes)
          {
            if (item.Name == "Node")
            {
              RecursiveNode(item, wl,  IDTree, IDSessione, IDCliente, "", nomefile);
            }
          }
        }
      }
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }
  }
}
