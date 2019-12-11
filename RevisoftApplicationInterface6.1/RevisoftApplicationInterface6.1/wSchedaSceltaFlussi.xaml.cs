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
using UserControls;

namespace RevisoftApplication
{
  public partial class wSchedaSceltaFlussi : Window
  {

    Hashtable htClienti = new Hashtable();
    Hashtable htAree = new Hashtable();

    public TipoFlusso tipo = new TipoFlusso();

    public enum TipoFlusso
    {
      ISQC,
      Societa,
      Gruppo,
      Terzi
    }

    public wSchedaSceltaFlussi()
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

      string IDCliente = htClienti[cmbClienti.SelectedIndex].ToString();


      wFlussi FLUSSI = new wFlussi();
      FLUSSI.Owner = this;
      FLUSSI.IDCliente = Convert.ToInt32(IDCliente);
      FLUSSI.Cliente = cmbClienti.SelectedValue.ToString();
      FLUSSI.tipo = tipo;

      FLUSSI.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

      FLUSSI.Load();
      Hide();
      FLUSSI.ShowDialog();
      Close();
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }
  }
}
