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
using System.ComponentModel;
using System.Collections;
using System.Data;

namespace RevisoftApplication
{
  public partial class NodoFazzoletto : Window
  {
    private bool _DatiCambiati;
    private int oldidsessione;
    int id = 0;
    DataTable dati = null;
    private int oldidcliente;

    public string Nodo = "-1";

    private bool _ReadOnly = false;
    public bool _ApertoInSolaLettura = true;


    public bool ReadOnly
    {
      set
      {
        _ReadOnly = value;

        if (value)
        {
          buttonSalva.Visibility = System.Windows.Visibility.Collapsed;
          buttonElimina.Visibility = System.Windows.Visibility.Collapsed;
          txtNote.IsEnabled = false;
        }
        else
        {
          buttonSalva.Visibility = System.Windows.Visibility.Visible;
          buttonElimina.Visibility = System.Windows.Visibility.Visible;
          txtNote.IsEnabled = true;
        }
      }
    }

    public bool ApertoInSolaLettura
    {
      set
      {
        _ApertoInSolaLettura = value;

        if (value)
        {
          buttonSalva.Visibility = System.Windows.Visibility.Collapsed;
          buttonElimina.Visibility = System.Windows.Visibility.Collapsed;
          txtNote.IsEnabled = false;
        }
        else
        {
          buttonSalva.Visibility = System.Windows.Visibility.Visible;
          buttonElimina.Visibility = System.Windows.Visibility.Visible;
          txtNote.IsEnabled = true;
        }
      }
    }


    public NodoFazzoletto()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      _DatiCambiati = false;
    }

    public void Load(string IDCliente)
    {
      id = int.Parse(Nodo);
      oldidcliente = cBusinessObjects.idcliente;
      oldidsessione = cBusinessObjects.idsessione;

      cBusinessObjects.idcliente = int.Parse(IDCliente);
      cBusinessObjects.idsessione = 0;

      dati = cBusinessObjects.GetData(id, typeof(Osservazioni));


      if (dati.Rows.Count > 0)
      {
        foreach (DataRow dtrow in dati.Rows)
        {
          txtNote.Text = dtrow["OsservazioniTxt"].ToString();

        }
      }
      else
      {
        dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
      }


      _DatiCambiati = false;

      txtNote.Focus();
    }

    private void GestoreEvento_DatiCambiati(object sender, RoutedEventArgs e)
    {
      _DatiCambiati = true;
    }

    private void GestoreEvento_ChiusuraFinestra(object sender, CancelEventArgs e)
    {
      if (_DatiCambiati)
      {
        buttonSalva_Click(sender, new RoutedEventArgs());
      }

      //Utilities u = new Utilities();
      //if (MessageBoxResult.No == u.AvvisoPerditaDati())
      //    e.Cancel = true;
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      if (_DatiCambiati)
      {
        buttonSalva_Click(sender, new RoutedEventArgs());
      }

      cBusinessObjects.idcliente = oldidcliente;
      cBusinessObjects.idsessione = oldidsessione;

      base.Close();
    }

    private void buttonSalva_Click(object sender, RoutedEventArgs e)
    {
      foreach (DataRow dtrow in dati.Rows)
      {
        dtrow["OsservazioniTxt"] = txtNote.Text;

      }

      cBusinessObjects.SaveData(id, dati, typeof(Osservazioni));


      _DatiCambiati = false;
    }

    private void buttonElimina_Click(object sender, RoutedEventArgs e)
    {
      //conferma cancellazione
      Utilities u = new Utilities();
      if (MessageBoxResult.No == u.ConfermaCancellazione())
        return;

      dati.Clear();
      cBusinessObjects.SaveData(id, dati, typeof(Osservazioni));

      txtNote.Text = "";
      _DatiCambiati = false;
      cBusinessObjects.idcliente = oldidcliente;
      cBusinessObjects.idsessione = oldidsessione;
      base.Close();
    }
  }
}
