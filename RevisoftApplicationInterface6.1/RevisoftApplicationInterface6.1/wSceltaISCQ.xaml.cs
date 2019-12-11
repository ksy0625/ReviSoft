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
  public partial class wSceltaISCQ : Window
  {
    string _IDCliente = "-1";
    string _IDTree = "-1";

    ArrayList alISQCs = new ArrayList();

    public wSceltaISCQ(string IDCliente, string IDTree)
    {
      _IDCliente = IDCliente;
      _IDTree = IDTree;

      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];

      MasterFile mf = MasterFile.Create();

      alISQCs = mf.GetISQCs(IDCliente);

      //ISQC
      foreach (Hashtable hthere in alISQCs)
      {
        RadioButton chkSessione = new RadioButton();
        chkSessione.Tag = hthere["ID"].ToString();
        chkSessione.Content = hthere["DataNomina"].ToString() + " - " + ((hthere["DataFine"] == null) ? "" : hthere["DataFine"].ToString());
        chkSessione.GroupName = "ISQC";
        stpSessioniISQC.Children.Add(chkSessione);
      }
    }

    private void buttonApri_Click(object sender, RoutedEventArgs e)
    {
      MasterFile mf = MasterFile.Create();
      string selectedSession = "-1";
      string selectedSessionTitle = "";

      foreach (object item in stpSessioniISQC.Children)
      {
        if (item.GetType().Name == "RadioButton")
        {
          if (((RadioButton)(item)).IsChecked == true)
          {
            selectedSession = ((RadioButton)(item)).Tag.ToString();
            selectedSessionTitle = ((RadioButton)(item)).Content.ToString();
          }
        }
      }

      if (selectedSession == "-1")
      {
        e.Handled = true;
        return;
      }

      Hashtable ht = mf.GetISQC(selectedSession);

      XmlDataProviderManager _xNew = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + ht["FileData"].ToString());

      WindowWorkArea wa = new WindowWorkArea(ref _xNew);

      //Nodi
      wa.NodeHome = 0;

      RevisoftApplication.XmlManager xt = new XmlManager();
      xt.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
      XmlDataProvider TreeXmlProvider = new XmlDataProvider();
      TreeXmlProvider.Document = xt.LoadEncodedFile(App.AppDataDataFolder + "\\" + ht["File"].ToString());

      if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
      {
        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
        {
          switch (((App.TipoAttivita)(Convert.ToInt32(_IDTree))))
          {
            case App.TipoAttivita.Revisione:
              if (item.Attributes["ID"].Value == "181")
              {
                wa.Nodes.Add(0, item);
              }
              break;
            case App.TipoAttivita.Bilancio:
              if (item.Attributes["ID"].Value == "182")
              {
                wa.Nodes.Add(0, item);
              }
              break;
            case App.TipoAttivita.Conclusione:
              if (item.Attributes["ID"].Value == "183")
              {
                wa.Nodes.Add(0, item);
              }
              break;
            case App.TipoAttivita.Verifica:
              if (item.Attributes["ID"].Value == "185")
              {
                wa.Nodes.Add(0, item);
              }
              break;
            default:
              e.Handled = true;
              return;
          }
        }
      }

      if (wa.Nodes.Count == 0)
      {
        e.Handled = true;
        return;
      }

      wa.NodeNow = wa.NodeHome;

      wa.Owner = Window.GetWindow(this);

      //posizione e dimensioni finestra
      wa.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
      wa.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
      wa.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
      wa.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
      wa.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
      wa.MinHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
      wa.MinWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;

      //Sessioni
      wa.Sessioni.Clear();
      wa.Sessioni.Add(0, App.AppDataDataFolder + "\\" + ht["FileData"].ToString());

      wa.SessioniTitoli.Clear();
      wa.SessioniTitoli.Add(0, selectedSessionTitle);

      wa.SessioniID.Clear();
      wa.SessioniID.Add(0, selectedSession);

      wa.SessioneHome = 0;
      wa.SessioneNow = 0;

      //Variabili
      wa.ReadOnly = true;
      wa.ReadOnlyOLD = true;
      wa.ApertoInSolaLettura = true;

      //passaggio dati
      wa.IDTree = "28";
      wa.IDSessione = selectedSession;
      wa.IDCliente = _IDCliente;

      wa.Stato = App.TipoTreeNodeStato.Sconosciuto;
      wa.OldStatoNodo = wa.Stato;

      //apertura
      wa.Load();

      App.MessaggioSolaScrittura = "Carta in sola lettura, premere tasto ESCI";
      App.MessaggioSolaScritturaStato = "Carta in sola lettura, premere tasto ESCI";

      wa.ShowDialog();

      App.MessaggioSolaScrittura = "Occorre selezionare Sblocca Stato per modificare il contenuto.";
      App.MessaggioSolaScritturaStato = "Sessione in sola lettura, impossibile modificare lo stato.";

      base.Close();
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }
  }
}
