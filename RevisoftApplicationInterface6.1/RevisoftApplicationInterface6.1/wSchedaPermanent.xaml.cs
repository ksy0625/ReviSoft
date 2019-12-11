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
  public partial class wSchedaPermanent : Window
  {
    Hashtable htClienti = new Hashtable();
    Hashtable htAree = new Hashtable();


    public wSchedaPermanent()
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

    private void buttonCerca_Click(object sender, RoutedEventArgs e)
    {
      //controllo selezione clienti
      if (cmbClienti.SelectedIndex == -1)
      {
        MessageBox.Show("selezionare un cliente");
        return;
      }


      //Process wait - START  andrea
      //ProgressWindow pw = new ProgressWindow();



      string IDCliente = htClienti[cmbClienti.SelectedIndex].ToString();

      wDocumenti documenti = new wDocumenti();

      documenti.ReadOnly = true;
      documenti.Permanente = "1";
      documenti.Titolo = "Indice Documenti per Cliente";
      documenti.Tipologia = TipoVisualizzazione.Documenti;
      documenti.Cliente = IDCliente;

      if (System.Windows.SystemParameters.PrimaryScreenWidth < 1100 || System.Windows.SystemParameters.PrimaryScreenHeight < 600)
      {
        documenti.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        documenti.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        documenti.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        documenti.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        documenti.MinHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        documenti.MinWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
      }
      else
      {
        documenti.Width = 1100;
        documenti.Height = 600;
      }




      documenti.Owner = this;
      documenti.Load();
      Hide();
      documenti.ShowDialog();
      Close();


      //Process wait - STOP
      //pw.Close();

    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }
  }
}
