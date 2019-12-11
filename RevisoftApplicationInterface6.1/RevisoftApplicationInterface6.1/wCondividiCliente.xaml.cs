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
  public partial class wCondividiCliente : Window
  {

    private int OldSelectedCmbClienti = -1;

    Hashtable htClienti = new Hashtable();
    Hashtable htDate = new Hashtable();


    public wCondividiCliente()
    {
      if (OldSelectedCmbClienti == 0) { }
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
        if (mf.GetBilanci(item["ID"].ToString()).Count == 0 && mf.GetRevisioni(item["ID"].ToString()).Count == 0 && mf.GetISQCs(item["ID"].ToString()).Count == 0 && mf.GetIncarichi(item["ID"].ToString()).Count == 0 && mf.GetConclusioni(item["ID"].ToString()).Count == 0 && mf.GetVerifiche(item["ID"].ToString()).Count == 0)
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

    private void buttonStampa_Click(object sender, RoutedEventArgs e)
    {
      //controllo selezione clienti
      if (cmbClienti.SelectedIndex == -1)
      {
        MessageBox.Show("selezionare un cliente");
        return;
      }

      if (MessageBox.Show("ATTENZIONE: vengono acquisiti tutti i dati (Revisione e Verifiche) e verranno sovrascritti sull’unità di destinazione. Per importare una sola parte dei dati utilizzare il CONDIVIDI DATI presente nelle aree specifiche. Procedere?", "ATTENZIONE", MessageBoxButton.YesNo) == MessageBoxResult.No)
      {
        return;
      }

      //andrea - vecchia chiamata a scheda anagrafica
      ////carico dati
      //wSchedaAnafrafica w = new wSchedaAnafrafica();
      //w.TipologiaAttivita = App.TipoAttivitaScheda.Condividi;
      //w.idRecord = Convert.ToInt32( htClienti[cmbClienti.SelectedIndex].ToString() );
      //w.ConfiguraMaschera();
      //w.Owner = this;
      //w.ShowDialog();

      //esportazione su file
      int idRecord = Convert.ToInt32(htClienti[cmbClienti.SelectedIndex].ToString());

      char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
      string RagioneSociale = new string
                                      (
                                          ("Condivisione di " + cmbClienti.SelectedValue.ToString())
                                              .Where(x => !invalidChars.Contains(x))
                                              .ToArray()
                                      );

      //nome file di esportazione
      Utilities u = new Utilities();
      string nomeFile = RagioneSociale + u.EstensioneFile(App.TipoFile.ImportExport);
      string ret = u.sys_SaveFileDialog(nomeFile, App.TipoFile.ImportExport);
      if (ret != null)
      {

        base.Close();


        //ANDREA 2.8
        //backup file di importazione
        //string retBis = App.AppTempFolder + "{" + Guid.NewGuid().ToString() + "}";
        //cImportExport.ExportNoVerbose(retBis, idRecord, true);

        //esportazione
        cImportExport.Export(ret, idRecord, true);


        //interfaccia
        MessageBox.Show("Condivisione avvenuta con successo");
      }



    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }
  }
}
