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
using System.Globalization;
using System.Data;


namespace RevisoftApplication
{
  public partial class wSchedaTODOList : Window
  {
    Hashtable htClienti = new Hashtable();
    Hashtable htAree = new Hashtable();

    public wSchedaTODOList()
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

    private void functionCmbClientiChanged(ComboBox cmb)
    {
      int index = 0;

      cmbData.SelectedIndex = -1;

      cmbData.Items.Clear();
      htAree.Clear();

      Utilities u = new Utilities();

      string value = u.TitoloAttivita(App.TipoAttivita.Verifica);
      htAree.Add(index, App.TipoAttivita.Verifica);
      index++;
      cmbData.Items.Add("" + value);

      value = u.TitoloAttivita(App.TipoAttivita.Vigilanza);
      htAree.Add(index, App.TipoAttivita.Vigilanza);
      index++;
      cmbData.Items.Add("" + value);
    }

    private void cmbPianificate_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (((ComboBox)(sender)).SelectedValue != null)
      {
        dtpDataNomina.Text = ((ComboBox)(sender)).SelectedValue.ToString();
        dtpDataNomina.IsEnabled = false;
      }
      else
      {
        dtpDataNomina.Text = "";
      }
    }

    private void cmbArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      MasterFile mf = MasterFile.Create();

      string IDCliente = htClienti[cmbClienti.SelectedIndex].ToString();

      cmbPianificate.SelectedIndex = -1;
      cmbPianificate.Items.Clear();

      if (cmbData.SelectedIndex == 0)
      {
        ArrayList al = mf.GetPianificazioniVerifiche(IDCliente);
        List<DateTime> alX = new List<DateTime>();

        foreach (Hashtable itemHT in al)
        {
          DataTable pianificazione = cBusinessObjects.GetData(100013, typeof(PianificazioneVerificheTestata), int.Parse(IDCliente), int.Parse(itemHT["ID"].ToString()), 26);

          foreach (DataRow itemXPP in pianificazione.Rows)
          {
            DateTime dt = Convert.ToDateTime(itemXPP["Data"].ToString());

            if (!alX.Contains(dt))
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
      else if (cmbData.SelectedIndex == 1)
      {
        ArrayList al = mf.GetPianificazioniVigilanze(IDCliente);
        List<DateTime> alX = new List<DateTime>();

        foreach (Hashtable itemHT in al)
        {


          DataTable pianificazione = cBusinessObjects.GetData(100003, typeof(PianificazioneVerificheTestata), int.Parse(IDCliente), int.Parse(itemHT["ID"].ToString()), 27);

          foreach (DataRow itemXPP in pianificazione.Rows)
          {

            DateTime dt = Convert.ToDateTime(itemXPP["Data"].ToString());

            if (!alX.Contains(dt))
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

    }

    private void buttonTODOList_Click(object sender, RoutedEventArgs e)
    {
      //controllo selezione clienti
      if (cmbClienti.SelectedIndex == -1)
      {
        MessageBox.Show("selezionare un cliente");
        return;
      }

      if (cmbData.SelectedIndex == -1)
      {
        MessageBox.Show("selezionare un'area");
        return;
      }

      string IDCliente = htClienti[cmbClienti.SelectedIndex].ToString();

      App.TipoAttivita Area = (App.TipoAttivita)(htAree[cmbData.SelectedIndex]);

      WindowWorkAreaTree_TODOList TODOList = new WindowWorkAreaTree_TODOList();
      TODOList.Owner = this;

      if (cmbData.SelectedIndex == 0)
      {
        TODOList.TipoAttivita = App.TipoAttivita.Verifica;
      }
      else
      {
        TODOList.TipoAttivita = App.TipoAttivita.Vigilanza;
      }

      TODOList.IDCliente = IDCliente;
      TODOList.Data = dtpDataNomina.Text;
      TODOList.IDCliente = IDCliente;
      TODOList.Cliente = cmbClienti.SelectedValue.ToString();
      TODOList.LoadTreeSource();

      try
      {
        TODOList.ShowDialog();
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaTODOList.buttonTODOList_Click exception");
        string log = ex.Message;
      }

    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }
  }
}
