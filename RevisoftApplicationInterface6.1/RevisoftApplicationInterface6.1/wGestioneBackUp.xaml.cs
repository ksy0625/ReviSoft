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
using System.Collections;
using System.IO;

namespace RevisoftApplication
{

  public partial class wGestioneBackUp : Window
  {
    Hashtable htBU = new Hashtable();
    BackUpFile bf = new BackUpFile();

    public wGestioneBackUp()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      //Interfaccia x autorizzazioni - backup
      btn_BackUp.IsEnabled = App.AppConsentiGestioneBackUp;
      btn_Restore.IsEnabled = App.AppConsentiGestioneBackUp;
      btn_Elimina.IsEnabled = App.AppConsentiGestioneBackUp;

      //Carico dati
      reload_lstRestore();
    }

    private void reload_lstRestore()
    {
      lstRestore.Items.Clear();
      htBU.Clear();

      int index = 0;

      List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>();

      foreach (Hashtable item in bf.GetBackUps())
      {
        myList.Add(new KeyValuePair<string, string>(item["ID"].ToString(), item["Data"].ToString() + " " + item["Ora"].ToString()));
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
        htBU.Add(index, item.Key);
        lstRestore.Items.Add("Backup del giorno " + (Convert.ToDateTime(item.Value)).ToShortDateString() + " ore " + (Convert.ToDateTime(item.Value)).ToShortTimeString());

        index++;
      }
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

    private void btn_BackUp_Click(object sender, RoutedEventArgs e)
    {
      //richiesta conferma
      Utilities u = new Utilities();
      if (MessageBoxResult.No == u.ConfermaBackUp())
        return;


      //backup
      Hashtable ht = new Hashtable();
      bf.SetBackUp(ht, -1);
      reload_lstRestore();


      MessageBox.Show("Salvataggio avvenuto con successo");
    }

    private void btn_Restore_Click(object sender, RoutedEventArgs e)
    {
      if (lstRestore.SelectedIndex != -1)
      {
        //richiesta conferma
        Utilities u = new Utilities();
        if (MessageBoxResult.No == u.ConfermaRestore())
          return;



        Hashtable ht = new Hashtable();
        bf.SetBackUp(ht, -1);
        bf.Restore(htBU[lstRestore.SelectedIndex].ToString());
        reload_lstRestore();



        MessageBox.Show("Ripristino avvenuto con successo");
      }
      else
      {
        MessageBox.Show("Selezionare un punto di ripristino dalla lista");
      }
    }

    private void btn_BackUpFile_Click(object sender, RoutedEventArgs e)
    {
      //File di backup
      string nomefile = "";
      Utilities u = new Utilities();
      nomefile = u.sys_SaveFileDialog("", App.TipoFile.BackUp);
      //Annullo backup
      if (nomefile == null)
      {
        return;
      }



      //backup
      bf.SetBackUpFile(nomefile);



      //interfaccia
      MessageBox.Show("Salvataggio archivio Revisoft avvenuto con successo");
    }

    private void btn_RestoreFile_Click(object sender, RoutedEventArgs e)
    {
      //File di restore
      string nomefile = "";
      Utilities u = new Utilities();
      nomefile = u.sys_OpenFileDialog("", App.TipoFile.BackUp);
      //Annullo restore
      if (nomefile == null)
      {
        return;
      }

      //controllo esistenza file
      FileInfo fi = new FileInfo(nomefile);
      if (!fi.Exists)
      {
        return;
      }

      //restore
      bf.RestoreFile(nomefile);


      //interfaccia
      MessageBox.Show("Ripristino archivio Revisoft avvenuto con successo");
    }

    private void lstRestore_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (lstRestore.SelectedIndex != -1)
      {
        btn_Restore.IsEnabled = true;
        btn_Elimina.IsEnabled = true;
      }
      else
      {
        btn_Restore.IsEnabled = false;
        btn_Elimina.IsEnabled = false;
      }
    }

    private void btn_Elimina_Click(object sender, RoutedEventArgs e)
    {
      if (lstRestore.SelectedIndex != -1)
      {
        //richiesta conferma
        Utilities u = new Utilities();
        if (MessageBoxResult.No == u.ConfermaCancellazione())
          return;

        bf.DeleteBackUp(htBU[lstRestore.SelectedIndex].ToString());
        reload_lstRestore();
        MessageBox.Show("Punto di ripristino eliminato con successo");
      }
      else
      {
        MessageBox.Show("Selezionare un punto di ripristino dalla lista");
      }
    }

  }
}
