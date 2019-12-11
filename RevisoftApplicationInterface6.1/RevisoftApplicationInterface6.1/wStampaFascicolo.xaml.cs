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
  public partial class wStampaFascicolo : Window
  {

    private int OldSelectedCmbClienti = -1;

    Hashtable htClienti = new Hashtable();
    Hashtable htDate = new Hashtable();


    public string selectedCliente = "";
    public string selectedSession = "";


    ArrayList alIncarichi = new ArrayList();
    ArrayList alISQCs = new ArrayList();
    ArrayList alRevisioni = new ArrayList();
    ArrayList alBilanci = new ArrayList();
    ArrayList alConclusioni = new ArrayList();

    public wStampaFascicolo()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];

      //interfaccia 
      ConfiguraMaschera();
      cmbClienti.Focus();
    }

    public void ConfiguraMaschera()
    {
      MasterFile mf = MasterFile.Create();

      GestioneLicenza gl = new GestioneLicenza();

      txtRevisore.Text = gl.Utente;

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
        if (mf.GetBilanci(item["ID"].ToString()).Count == 0 && mf.GetRevisioni(item["ID"].ToString()).Count == 0)
        {
          continue;
        }

        string cliente = item["RagioneSociale"].ToString();
        //switch (((App.TipoAnagraficaStato)(Convert.ToInt32(item["Stato"].ToString()))))
        //{
        //    case App.TipoAnagraficaStato.InUso:
        //        cliente += " (In Uso)";
        //        break;
        //    case App.TipoAnagraficaStato.Bloccato:
        //        cliente += " (Bloccato)";
        //        break;
        //    case App.TipoAnagraficaStato.Esportato:
        //        cliente += " (Esportato)";
        //        break;
        //    case App.TipoAnagraficaStato.Disponibile:
        //    case App.TipoAnagraficaStato.Sconosciuto:
        //    default:
        //        break;
        //}

        myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), cliente));
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
      string IDCliente = htClienti[((ComboBox)sender).SelectedIndex].ToString();

      stpSessioni1.Children.Clear();

      MasterFile mf = MasterFile.Create();
      alIncarichi = mf.GetIncarichi(IDCliente);
      alISQCs = mf.GetISQCs(IDCliente);
      alRevisioni = mf.GetRevisioni(IDCliente);
      alBilanci = mf.GetBilanci(IDCliente);
      alConclusioni = mf.GetConclusioni(IDCliente);

      //INCARICHI
      TextBlock txt1 = new TextBlock();
      txt1.FontWeight = FontWeights.Bold;
      txt1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
      txt1.Text = "Incarichi";
      txt1.Margin = new Thickness(0, 0, 0, 5);
      stpSessioni1.Children.Add(txt1);

      foreach (Hashtable hthere in alIncarichi)
      {
        CheckBox chkSessione = new CheckBox();
        chkSessione.Tag = hthere["ID"].ToString();
        chkSessione.Content = hthere["DataNomina"].ToString();
        stpSessioni1.Children.Add(chkSessione);
      }

      //ISQCs
      TextBlock txtISQC = new TextBlock();
      txtISQC.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
      txtISQC.Text = "ISQC";
      txtISQC.FontWeight = FontWeights.Bold;
      txtISQC.Margin = new Thickness(0, 10, 0, 5);
      stpSessioni1.Children.Add(txtISQC);

      foreach (Hashtable hthere in alISQCs)
      {
        CheckBox chkSessione = new CheckBox();
        chkSessione.Width = 150;
        chkSessione.Tag = hthere["ID"].ToString();
        chkSessione.Content = hthere["DataNomina"].ToString();
        stpSessioni1.Children.Add(chkSessione);
      }

      //REVISIONI - BILANCI - CONCLUSIONI
      try
      {
        OldSelectedCmbClienti = Convert.ToInt32(IDCliente);

        int index = 0;
        htDate.Clear();
        cmbData.Items.Clear();
        stpSessioni239.Visibility = Visibility.Collapsed;

        List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>();

        ArrayList alreadydone = new ArrayList();

        foreach (Hashtable item in mf.GetBilanci(IDCliente))
        {
          myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), ConvertDataToEsercizio(item["Data"].ToString())));
          alreadydone.Add(ConvertDataToEsercizio(item["Data"].ToString()));
        }

        foreach (Hashtable item in mf.GetRevisioni(IDCliente))
        {
          if (!alreadydone.Contains(ConvertDataToEsercizio(item["Data"].ToString())))
          {
            myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), ConvertDataToEsercizio(item["Data"].ToString())));
            alreadydone.Add(ConvertDataToEsercizio(item["Data"].ToString()));
          }
        }

        foreach (Hashtable item in mf.GetConclusioni(IDCliente))
        {
          if (!alreadydone.Contains(ConvertDataToEsercizio(item["Data"].ToString())))
          {
            myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), ConvertDataToEsercizio(item["Data"].ToString())));
          }
        }

        myList.Sort
        (
            delegate (KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
            {
              try
              {
                return nextPair.Value.ToString().CompareTo(firstPair.Value.ToString());
              }
              catch (Exception ex)
              {
                cBusinessObjects.logger.Error(ex, "wStampaFascicolo.cmbClienti_SelectionChanged1 exception");
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
          stpSessioni239.Visibility = Visibility.Visible;
        }
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wStampaFascicolo.cmbClienti_SelectionChanged2 exception");
        string log = ex.Message;
        cmbData.IsEnabled = false;
      }
    }

    //      private void cmbClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //{
    //          //interfaccia
    //          functionCmbClientiChanged(((ComboBox)sender));
    //          cmbData.Focus();
    //      }

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

    //private void functionCmbClientiChanged(ComboBox cmb)
    //{
    //	cmbData.SelectedIndex = -1;

    //	if (cmb.SelectedIndex != -1)
    //	{
    //		try
    //		{
    //			string IDCliente = htClienti[cmb.SelectedIndex].ToString();

    //			OldSelectedCmbClienti = Convert.ToInt32(IDCliente);

    //			MasterFile mf = MasterFile.Create();

    //			int index = 0;
    //			htDate.Clear();
    //			cmbData.Items.Clear();

    //			List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>();

    //			ArrayList alreadydone = new ArrayList();

    //			foreach (Hashtable item in mf.GetBilanci(IDCliente))
    //			{
    //				myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), ConvertDataToEsercizio(item["Data"].ToString())));
    //				alreadydone.Add(ConvertDataToEsercizio(item["Data"].ToString()));
    //			}

    //			foreach (Hashtable item in mf.GetRevisioni(IDCliente))
    //			{
    //				if (!alreadydone.Contains(ConvertDataToEsercizio(item["Data"].ToString())))
    //				{
    //					myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), ConvertDataToEsercizio(item["Data"].ToString())));
    //                          alreadydone.Add( ConvertDataToEsercizio( item["Data"].ToString() ) );
    //				}
    //			}

    //                  foreach ( Hashtable item in mf.GetConclusioni( IDCliente ) )
    //                  {
    //                      if ( !alreadydone.Contains( ConvertDataToEsercizio( item["Data"].ToString() ) ) )
    //                      {
    //                          myList.Add( new KeyValuePair<string, string>( item["ID"].ToString(), ConvertDataToEsercizio( item["Data"].ToString() ) ) );
    //                      }
    //                  }

    //			myList.Sort
    //			(
    //				delegate(KeyValuePair<string, string> firstPair, KeyValuePair<string, string> nextPair)
    //				{
    //					try
    //					{
    //						return nextPair.Value.ToString().CompareTo(firstPair.Value.ToString());
    //					}
    //					catch (Exception ex)
    //					{
    //						string log = ex.Message;
    //						return 1;
    //					}
    //				}
    //			);

    //			foreach (KeyValuePair<string, string> item in myList)
    //			{
    //				cmbData.Items.Add(item.Value);
    //				htDate.Add(index, item.Key);
    //				index++;
    //			}
    //		}
    //		catch (Exception ex)
    //		{
    //			string log = ex.Message;
    //			cmbData.IsEnabled = false;
    //		}
    //	}
    //}

    private void buttonStampa_Click(object sender, RoutedEventArgs e)
    {

      //controllo selezione clienti
      if (cmbClienti.SelectedIndex == -1)
      {
        MessageBox.Show("selezionare un cliente");
        return;
      }

      //Adesso stampa selettivamente o 1/ISQC o 2/3/9 (Borelli 11/12/2017)
      //if (cmbData.SelectedIndex == -1)
      //{
      //    MessageBox.Show("selezionare un anno");
      //    return;
      //}
      Hide();
      ArrayList TBD1 = new ArrayList();
#pragma warning disable CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato
      bool atleastone = false;
#pragma warning restore CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato

      foreach (object item in stpSessioni1.Children)
      {
        if (item.GetType().Name == "CheckBox")
        {
          if (((CheckBox)(item)).Tag.ToString() == "-1")
          {
            continue;
          }

          if (((CheckBox)(item)).IsChecked == true)
          {
            atleastone = true;
            TBD1.Add(((CheckBox)(item)).Tag.ToString());
          }
        }
      }

      //if (cmbData.SelectedIndex == -1 && atleastone == false)
      //{
      //    MessageBox.Show("selezionare almeno un incarico o esercizio");
      //    return;
      //}




      try
      {
        MasterFile mf = MasterFile.Create();

        string IDCliente = htClienti[cmbClienti.SelectedIndex].ToString();

        string anno = "";
        if (cmbData.SelectedIndex != -1)
        {
          anno = cmbData.SelectedValue.ToString().Split(' ')[0];
        }
        selectedCliente = IDCliente;
        Hashtable cliente = mf.GetAnagrafica(Convert.ToInt32(IDCliente));

        //WordLib wl = new WordLib();
        RTFLib wl = new RTFLib();

        wl.Fascicolo = true;
        wl.StampaLetteraAttestazione = false;
        wl.StampaManagementLetter = false;
        wl.StampaLetteraIncarico = false;
        wl.StampaRelazioneBilancioeVigilanza = false;
        wl.StampaRelazioneBilancio = false;
        wl.StampaRelazioneVigilanza = false;
        wl.StampaRelazioneGenerica = false;
        wl.Utente = txtRevisore.Text;

        Hashtable hthere = new Hashtable();
        hthere.Add("ID", IDCliente);
        hthere.Add("anno", anno);

        wl.TemplateFileCompletePath = App.AppTemplateStampa;
        if (cmbData.SelectedIndex != -1)
        {
          wl.Open(hthere, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), cmbData.SelectedValue.ToString(), "Fascicolo della Revisione \\line\\line esercizio " + cmbData.SelectedValue.ToString() + " \\line\\line\\line\\line\\line\\line\\line\\line ", true, true, IDCliente);
        }
        else
        {
          wl.Open(hthere, cliente["RagioneSociale"].ToString(), cliente["CodiceFiscale"].ToString(), "", "Fascicolo della Revisione \\line\\line Accettazione Incarico \\line\\line\\line\\line\\line\\line\\line\\line ", true, true, IDCliente);
        }

        foreach (Hashtable htTBD in alIncarichi)
        {
          if (TBD1.Contains(htTBD["ID"].ToString()))
          {
            XmlDataProviderManager _t = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + htTBD["File"].ToString());
            XmlDataProviderManager _x = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + htTBD["FileData"].ToString());

            printsingle(cliente, _t, _x, (Convert.ToInt32(App.TipoFile.Incarico)).ToString(), wl, htTBD["ID"].ToString(), IDCliente, " ( Sessione: " + htTBD["DataNomina"].ToString() + " )", htTBD["FileData"].ToString());
          }
        }

        foreach (Hashtable htTBD in alISQCs)
        {
          if (TBD1.Contains(htTBD["ID"].ToString()))
          {
            XmlDataProviderManager _t = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + htTBD["File"].ToString());
            XmlDataProviderManager _x = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + htTBD["FileData"].ToString());

            printsingle(cliente, _t, _x, (Convert.ToInt32(App.TipoFile.ISQC)).ToString(), wl, htTBD["ID"].ToString(), IDCliente, " ( Sessione: " + htTBD["DataNomina"].ToString() + " )", htTBD["FileData"].ToString());
          }
        }

        if (cmbData.SelectedIndex == -1)// && atleastone == false)
        {
          ;
        }
        else
        {
          foreach (Hashtable item in mf.GetRevisioni(IDCliente))
          {
            try
            {
              if (cmbData.SelectedValue != null && item != null && item["Data"] != null && cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["Data"].ToString()))
              {
                XmlDataProviderManager _t = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["File"].ToString());
                XmlDataProviderManager _x = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["FileData"].ToString());

                printsingle(cliente, _t, _x, (Convert.ToInt32(App.TipoFile.Revisione)).ToString(), wl, item["ID"].ToString(), IDCliente, "", item["FileData"].ToString());
                break;
              }
            }
            catch (Exception ex)
            {
              cBusinessObjects.logger.Error(ex, "wStampaFascicolo.buttonStampa_Click1 exception");
              MessageBox.Show("Attenzione, errore nei dati sulle revisioni verificare la stampa");
              break;
            }

          }

          foreach (Hashtable item in mf.GetBilanci(IDCliente))
          {
            try
            {


              if (cmbData.SelectedValue != null && item != null && item["Data"] != null && cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["Data"].ToString()))
              {
                XmlDataProviderManager _t = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["File"].ToString());
                XmlDataProviderManager _x = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["FileData"].ToString());

                printsingle(cliente, _t, _x, (Convert.ToInt32(App.TipoFile.Bilancio)).ToString(), wl, item["ID"].ToString(), IDCliente, "", item["FileData"].ToString());
                break;
              }

            }
            catch (Exception ex)
            {
              cBusinessObjects.logger.Error(ex, "wStampaFascicolo.buttonStampa_Click2 exception");
              MessageBox.Show("Attenzione, errore nei dati sui bilanci verificare la stampa");
              break;
            }

          }

          foreach (Hashtable item in mf.GetConclusioni(IDCliente))
          {
            try
            {

              if (cmbData.SelectedValue != null && item != null && item["Data"] != null && cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["Data"].ToString()))
              {
                XmlDataProviderManager _t = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["File"].ToString());
                XmlDataProviderManager _x = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["FileData"].ToString());

                printsingle(cliente, _t, _x, (Convert.ToInt32(App.TipoFile.Conclusione)).ToString(), wl, item["ID"].ToString(), IDCliente, "", item["FileData"].ToString());
                break;
              }

            }
            catch (Exception ex)
            {
              cBusinessObjects.logger.Error(ex, "wStampaFascicolo.buttonStampa_Click3 exception");
              MessageBox.Show("Attenzione, errore nei dati sulle conclusioni verificare la stampa");
              break;
            }
          }
        }

        wl.SetFilename(App.AppTempFolder + "\\" + cliente["RagioneSociale"].ToString() + "_Fascicolo_" + anno);
        wl.SavePDF("", this);
        wl.Close();
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wStampaFascicolo.buttonStampa_Click4 exception");
        string log = ex.Message;
        MessageBox.Show("Attenzione, il processo ha riscontrato un errore: " + log);
      }



    }

    //private void printsingle(Hashtable cliente, XmlDataProviderManager TreeXmlProvider, XmlDataProviderManager _x, string IDTree, WordLib wl, string IDSessione, string IDCliente)
    private void printsingle(Hashtable cliente, XmlDataProviderManager TreeXmlProvider, XmlDataProviderManager _x, string IDTree, RTFLib wl, string IDSessione, string IDCliente, string AdditivaTitolo, string nomefile)
    {
      selectedSession = IDSessione;
      RecursiveNode(TreeXmlProvider.Document.SelectSingleNode("/Tree/Node"), wl, _x, IDTree, IDSessione, IDCliente, AdditivaTitolo, nomefile);
    }

    private bool isreporttobeprinted = true;

    private bool RecursiveCheck(XmlNode node, string IDTree)
    {
      bool returnvalue = false;

      if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
      {
        try
        {

          //XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");
          //	if ((isreporttobeprinted && node.Attributes["Report"].Value == "True") || (NodoDato.Attributes["Stato"] != null &&
          //  NodoDato.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString()))
          //if (NodoDato.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString() || node.Attributes["Report"].Value == "True")
          //{



          string stato = cBusinessObjects.GetStato(int.Parse(node.Attributes["ID"].Value), selectedCliente, selectedSession, IDTree);

          if (isreporttobeprinted && stato != "" && stato == ((Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString()))
          {
            return true;
          }
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wStampaFascicolo.RecursiveCheck exception");
          string log = ex.Message;
        }
      }
      else
      {
        if (node.ParentNode.Name != "Tree")
        {
          isreporttobeprinted = true;

          foreach (XmlNode item in node.ChildNodes)
          {
            if (item.Name == "Node")
            {
              returnvalue = RecursiveCheck(item, IDTree);

              //controllo nel caso che vi siano dei report che necessitano altri nodi come true all'interno dello stesso multinodo
              if (!returnvalue && (item.Attributes["ID"].Value == "227" || item.Attributes["ID"].Value == "229" || item.Attributes["ID"].Value == "134" || item.Attributes["ID"].Value == "2016174" || item.Attributes["ID"].Value == "2016134" || item.Attributes["ID"].Value == "2016186"))
              {
                isreporttobeprinted = false;
              }

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
    private void RecursiveNode(XmlNode node, RTFLib wl, XmlDataProviderManager _x, string IDTree, string IDSessione, string IDCliente, string AdditivaTitolo, string nomefile)
    {
      string str;

      try
      {
        if (node.Attributes["ID"].Value == "186")
        {
          str = "186";
        }
        if ((node.Attributes["ID"].Value == "142" || node.Attributes["ID"].Value == "2016142") && IDTree == "3")
        {
          return;
        }

        if (node.Attributes["ID"].Value == "142" && IDTree == "28")
        {
          return;
        }

        if (node.Attributes["ID"].Value == "261" && IDTree == "19")
        {
          return;
        }

        if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
        {
          if (RecursiveCheck(node, IDTree))
          {

            wl.Add(node, IDCliente, IDTree, IDSessione, nomefile);
          }
        }
        else
        {
          if (node.ParentNode.Name == "Tree" || RecursiveCheck(node, IDTree))
          {
            wl.AddTitle(node.Attributes["Codice"].Value + " " + node.Attributes["Titolo"].Value + AdditivaTitolo, node.ParentNode.Name != "Tree");

            foreach (XmlNode item in node.ChildNodes)
            {
              if (item.Name == "Node")
              {
                try
                {
                  RecursiveNode(item, wl, _x, IDTree, IDSessione, IDCliente, "", nomefile);
                }
                catch (Exception ex)
                {
                  cBusinessObjects.logger.Error(ex, "wStampaFascicolo.RecursiveNode1 exception");
                  str = ex.Message;
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wStampaFascicolo.RecursiveNode2 exception");
        str = ex.Message;
      }
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }
  }
}
