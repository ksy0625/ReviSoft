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
  public partial class wSchedaSelezionaCliente : Window
  {
    Hashtable htClienti = new Hashtable();
    Hashtable htRagioniSociali = new Hashtable();
    Hashtable htAree = new Hashtable();

    public string IDCliente = "-1";
    public string RagioneSociale = "";

    public wSchedaSelezionaCliente()
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
        htRagioniSociali.Clear();
        htClienti.Clear();
      }

      List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>();

      foreach (Hashtable item in mf.GetAnagrafiche(true))
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
        htRagioniSociali.Add(index, item.Value);
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
    }

    private void functionCmbClientiChanged(ComboBox cmb)
    {

    }

    private void buttonSOSPESI_Click(object sender, RoutedEventArgs e)
    {
      //controllo selezione clienti
      if (cmbClienti.SelectedIndex == -1)
      {
        MessageBox.Show("selezionare un cliente");
        return;
      }

      IDCliente = htClienti[cmbClienti.SelectedIndex].ToString();
      RagioneSociale = htRagioniSociali[cmbClienti.SelectedIndex].ToString();

      base.Close();
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }
  }
}
