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
  public partial class wMultiLicenza : Window
  {

    //Gestione licenza
    RevisoftApplication.GestioneLicenza l = new GestioneLicenza();


    public wMultiLicenza()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      //interfaccia 
      ConfiguraMaschera();
      cmbMultiLicenze.Focus();
    }


    public void ConfiguraMaschera()
    {
      string utente;

      //Resetto interfaccia
      if (cmbMultiLicenze.Items.Count != 0)
      {
        cmbMultiLicenze.Items.Clear();
        cmbMultiLicenze.SelectedIndex = -1;
      }

      //Utiliti
      RevisoftApplication.Utilities u = new Utilities();

      //Leggo dati
      if (l.LeggiMultilicenza())
      {
        foreach (DictionaryEntry item in l.DatiMultiLicenza)
        {
          utente = item.Key.ToString();

          //scadenza
          DateTime scadenza = u.StringToDateTime(((Hashtable)(item.Value))["DataScadenza"].ToString());
          //Licenza
          //App.TipologieLicenze licenza = (App.TipologieLicenze)(Enum.Parse(typeof(App.TipologieLicenze), ((Hashtable)(item.Value))["TipoLicenza"].ToString(), true));

          if (scadenza >= DateTime.Now)// && App.TipologieLicenze.ClientLanMulti == licenza)
          {
            //alimento combo
            cmbMultiLicenze.Items.Add(utente);
          }
        }
      }

    }


    private void buttonApplica_Click(object sender, RoutedEventArgs e)
    {
      //controllo selezione clienti
      if (cmbMultiLicenze.SelectedIndex == -1)
      {
        MessageBox.Show("Selezionare una Licenza");
        return;
      }

      //Utiliti
      RevisoftApplication.Utilities u = new Utilities();

      //carico nuova licenza
      if (MessageBoxResult.No == u.ConfermiCambioMultiLicenza(cmbMultiLicenze.SelectedValue.ToString()))
        return;

      //interfaccia - chiudo maschera
      base.Close();



      //acquisisco file name
      string mlFileName = ((Hashtable)(l.DatiMultiLicenza[cmbMultiLicenze.SelectedValue.ToString()]))["FileName"].ToString();

      //attivo licenza
      l.AttivaMultiLicenzaDaFile(mlFileName);

      //interfaccia
      ((MainWindow)(this.Owner)).ReloadMainWindow();



      //fine
      MessageBox.Show("Nuova licenza caricata.");
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

  }
}
