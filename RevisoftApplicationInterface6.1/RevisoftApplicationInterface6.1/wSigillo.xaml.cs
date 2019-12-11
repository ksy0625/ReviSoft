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
  public partial class wSigillo : Window
  {

    private int OldSelectedCmbClienti = -1;

    Hashtable htClienti = new Hashtable();
    Hashtable htDate = new Hashtable();


    public wSigillo()
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
        if (mf.GetBilanci(item["ID"].ToString()).Count == 0 && mf.GetRevisioni(item["ID"].ToString()).Count == 0 && mf.GetISQCs(item["ID"].ToString()).Count == 0 && mf.GetIncarichi(item["ID"].ToString()).Count == 0 && mf.GetConclusioni(item["ID"].ToString()).Count == 0)
        {
          continue;
        }

        string cliente = item["RagioneSociale"].ToString();

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
      //interfaccia
      functionCmbClientiChanged(((ComboBox)sender));
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

    private void functionCmbClientiChanged(ComboBox cmb)
    {
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

          foreach (Hashtable item in mf.GetIncarichi(IDCliente))
          {
            myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), ConvertDataToEsercizio(item["DataNomina"].ToString())));
            alreadydone.Add(ConvertDataToEsercizio(item["DataNomina"].ToString()));
          }

          foreach (Hashtable item in mf.GetISQCs(IDCliente))
          {
            myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), ConvertDataToEsercizio(item["DataNomina"].ToString())));
            alreadydone.Add(ConvertDataToEsercizio(item["DataNomina"].ToString()));
          }

          foreach (Hashtable item in mf.GetConclusioni(IDCliente))
          {
            if (!alreadydone.Contains(ConvertDataToEsercizio(item["Data"].ToString())))
            {
              myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), ConvertDataToEsercizio(item["Data"].ToString())));
            }
            alreadydone.Add(ConvertDataToEsercizio(item["Data"].ToString()));
          }

          foreach (Hashtable item in mf.GetBilanci(IDCliente))
          {
            if (!alreadydone.Contains(ConvertDataToEsercizio(item["Data"].ToString())))
            {
              myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), ConvertDataToEsercizio(item["Data"].ToString())));
            }
            alreadydone.Add(ConvertDataToEsercizio(item["Data"].ToString()));
          }

          foreach (Hashtable item in mf.GetRevisioni(IDCliente))
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
                cBusinessObjects.logger.Error(ex, "wSigillo.functionCmbClientiChanged1 exception");
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
          cBusinessObjects.logger.Error(ex, "wSigillo.functionCmbClientiChanged2 exception");
          string log = ex.Message;
          cmbData.IsEnabled = false;
        }
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
        MessageBox.Show("selezionare un esercizio");
        return;
      }

      MasterFile mf = MasterFile.Create();

      string IDCliente = htClienti[cmbClienti.SelectedIndex].ToString();

      string anno = cmbData.SelectedValue.ToString().Split(' ')[0];

      Hashtable cliente = mf.GetAnagrafica(Convert.ToInt32(IDCliente));

      ArrayList NodiDaCompletare = new ArrayList();

      foreach (Hashtable item in mf.GetIncarichi(IDCliente))
      {
        if (cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["DataNomina"].ToString()))
        {
          XmlDataProviderManager _t = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["File"].ToString());
          XmlDataProviderManager _x = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["FileData"].ToString());

          XmlNodeList NodeList = _x.Document.SelectNodes("/Dati//Dato");

          foreach (XmlNode nodo in NodeList)
          {
            if (nodo.Attributes["Stato"] != null && nodo.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString())
            {
              XmlNode nodotree = _t.Document.SelectSingleNode("/Tree//Node[@ID='" + nodo.Attributes["ID"].Value + "']");
              NodiDaCompletare.Add(nodotree.Attributes["Codice"].Value);
            }
          }
        }
      }

      foreach (Hashtable item in mf.GetISQCs(IDCliente))
      {
        if (cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["DataNomina"].ToString()))
        {
          XmlDataProviderManager _t = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["File"].ToString());
          XmlDataProviderManager _x = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["FileData"].ToString());

          XmlNodeList NodeList = _x.Document.SelectNodes("/Dati//Dato");

          foreach (XmlNode nodo in NodeList)
          {
            if (nodo.Attributes["Stato"] != null && nodo.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString())
            {
              XmlNode nodotree = _t.Document.SelectSingleNode("/Tree//Node[@ID='" + nodo.Attributes["ID"].Value + "']");
              NodiDaCompletare.Add(nodotree.Attributes["Codice"].Value);
            }
          }
        }
      }

      foreach (Hashtable item in mf.GetRevisioni(IDCliente))
      {
        if (cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["Data"].ToString()))
        {
          XmlDataProviderManager _t = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["File"].ToString());
          XmlDataProviderManager _x = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["FileData"].ToString());

          XmlNodeList NodeList = _x.Document.SelectNodes("/Dati//Dato");

          foreach (XmlNode nodo in NodeList)
          {
            if (nodo.Attributes["Stato"] != null && nodo.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString() && nodo.Attributes["ID"].Value != "22")
            {
              XmlNode nodotree = _t.Document.SelectSingleNode("/Tree//Node[@ID='" + nodo.Attributes["ID"].Value + "']");
              NodiDaCompletare.Add(nodotree.Attributes["Codice"].Value);
            }
          }
        }
      }

      foreach (Hashtable item in mf.GetBilanci(IDCliente))
      {
        if (cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["Data"].ToString()))
        {
          XmlDataProviderManager _t = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["File"].ToString());
          XmlDataProviderManager _x = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["FileData"].ToString());

          XmlNodeList NodeList = _x.Document.SelectNodes("/Dati//Dato");

          foreach (XmlNode nodo in NodeList)
          {
            if (nodo.Attributes["Stato"] != null && nodo.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString())
            {
              XmlNode nodotree = _t.Document.SelectSingleNode("/Tree//Node[@ID='" + nodo.Attributes["ID"].Value + "']");
              NodiDaCompletare.Add(nodotree.Attributes["Codice"].Value);
            }
          }
        }
      }

      foreach (Hashtable item in mf.GetConclusioni(IDCliente))
      {
        if (cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["Data"].ToString()))
        {
          XmlDataProviderManager _t = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["File"].ToString());
          XmlDataProviderManager _x = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + item["FileData"].ToString());

          XmlNodeList NodeList = _x.Document.SelectNodes("/Dati//Dato");

          foreach (XmlNode nodo in NodeList)
          {
            if (nodo.Attributes["Stato"] != null && nodo.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString())
            {
              XmlNode nodotree = _t.Document.SelectSingleNode("/Tree//Node[@ID='" + nodo.Attributes["ID"].Value + "']");
              NodiDaCompletare.Add(nodotree.Attributes["Codice"].Value);
            }
          }
        }
      }

      if (NodiDaCompletare.Count > 0)
      {
        string daCompletare = "Attenzione, per questo esercizio risultano 'DA COMPLETARE' le seguenti voci: ";
        foreach (string ndc in NodiDaCompletare)
        {
          daCompletare += ndc + ", ";
        }

        MessageBox.Show(daCompletare);
        return;
      }
      else
      {
        string sigillo = "";

        GestioneLicenza gl = new GestioneLicenza();
        string intestatario = gl.Utente;// UtenteSigillo;
        foreach (Hashtable item in mf.GetIncarichi(IDCliente))
        {
          if (cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["DataNomina"].ToString()))
          {
            sigillo = (item["Sigillo"] == null) ? "" : item["Sigillo"].ToString();

            if (sigillo != "")
            {
              MessageBox.Show("ATTENZIONE: sigillo già applicato a questa sessione");
              return;
            }
          }
        }

        foreach (Hashtable item in mf.GetISQCs(IDCliente))
        {
          if (cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["DataNomina"].ToString()))
          {
            sigillo = (item["Sigillo"] == null) ? "" : item["Sigillo"].ToString();

            if (sigillo != "")
            {
              MessageBox.Show("ATTENZIONE: sigillo già applicato a questa sessione");
              return;
            }
          }
        }

        foreach (Hashtable item in mf.GetBilanci(IDCliente))
        {
          if (cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["Data"].ToString()))
          {
            sigillo = (item["Sigillo"] == null) ? "" : item["Sigillo"].ToString();

            if (sigillo != "")
            {
              MessageBox.Show("ATTENZIONE: sigillo già applicato a questa sessione");
              return;
            }
          }
        }

        foreach (Hashtable item in mf.GetConclusioni(IDCliente))
        {
          if (cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["Data"].ToString()))
          {
            sigillo = (item["Sigillo"] == null) ? "" : item["Sigillo"].ToString();

            if (sigillo != "")
            {
              MessageBox.Show("ATTENZIONE: sigillo già applicato a questa sessione");
              return;
            }
          }
        }

        foreach (Hashtable item in mf.GetRevisioni(IDCliente))
        {
          if (cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["Data"].ToString()))
          {
            sigillo = (item["Sigillo"] == null) ? "" : item["Sigillo"].ToString();

            if (sigillo != "")
            {
              MessageBox.Show("ATTENZIONE: sigillo già applicato a questa sessione");
              return;
            }
          }
        }

        if ((cliente["Presidente"] == null || cliente["Presidente"].ToString() != intestatario) &&
            (cliente["MembroEffettivo"] == null || cliente["MembroEffettivo"].ToString() != intestatario) &&
            (cliente["MembroEffettivo2"] == null || cliente["MembroEffettivo2"].ToString() != intestatario) &&
            (cliente["RevisoreAutonomo"] == null || cliente["RevisoreAutonomo"].ToString() != intestatario))
        {
          MessageBox.Show("ATTENZIONE: l'intestatario di questa licenza non risulta uno dei revisori abilitati per questo cliente");
          return;
        }

        wSigilloPassword SP = new wSigilloPassword();
        SP.ShowDialog();
        SP.Activate();

        if (!SP.PasswordOK)
        {
          return;
        }

        string password = SP.Password;

        foreach (Hashtable item in mf.GetIncarichi(IDCliente))
        {
          if (cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["DataNomina"].ToString()))
          {
            mf.SetSigilloIncarico(Convert.ToInt32(item["ID"].ToString()), intestatario, password);
          }
        }

        foreach (Hashtable item in mf.GetISQCs(IDCliente))
        {
          if (cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["DataNomina"].ToString()))
          {
            mf.SetSigilloISQC(Convert.ToInt32(item["ID"].ToString()), intestatario, password);
          }
        }

        foreach (Hashtable item in mf.GetRevisioni(IDCliente))
        {
          if (cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["Data"].ToString()))
          {
            mf.SetSigilloRevisione(Convert.ToInt32(item["ID"].ToString()), intestatario, password);
          }
        }

        foreach (Hashtable item in mf.GetBilanci(IDCliente))
        {
          if (cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["Data"].ToString()))
          {
            mf.SetSigilloBilancio(Convert.ToInt32(item["ID"].ToString()), intestatario, password);
          }
        }

        foreach (Hashtable item in mf.GetConclusioni(IDCliente))
        {
          if (cmbData.SelectedValue.ToString() == ConvertDataToEsercizio(item["Data"].ToString()))
          {
            mf.SetSigilloConclusione(Convert.ToInt32(item["ID"].ToString()), intestatario, password);
          }
        }

        MessageBox.Show("Sigillo applicato con successo");
        base.Close();
      }
    }

    private void buttonLicenzaSigillo_Click(object sender, RoutedEventArgs e)
    {
      //RevisoftApplication.WindowGestioneLicenzaSigilloUtente wLS = new RevisoftApplication.WindowGestioneLicenzaSigilloUtente();
      //wLS.Owner = this;
      //wLS.ShowDialog();
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }
  }
}
