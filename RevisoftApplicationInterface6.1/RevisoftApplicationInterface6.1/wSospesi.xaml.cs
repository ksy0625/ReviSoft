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
  public partial class Sospesi : Window
  {
    public int id;
    public int idt;
    private DataTable dati = null;
    private bool _DatiCambiati;

    public string filedata = "";
    private bool _ReadOnly = false;
    public bool Changed = false;
    public App.TipoTreeNodeStato Stato = App.TipoTreeNodeStato.Sconosciuto;

    public string SospesiValue = "";

    public Sospesi()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      mainRTB.Focus();
      mainRTB.CaretPosition = mainRTB.Document.ContentEnd;
      mainRTB.ScrollToEnd();

      _DatiCambiati = false;
    }

    public bool ReadOnly
    {
      set
      {
        _ReadOnly = value;

        mainRTB.IsReadOnly = _ReadOnly;
        txtNote.IsReadOnly = _ReadOnly;

        if (_ReadOnly)
        {
          buttonSalva.Visibility = System.Windows.Visibility.Collapsed;
        }
        else
        {
          buttonSalva.Visibility = System.Windows.Visibility.Visible;
        }
      }
    }

    public void Load(string ID, string IDCliente, string IDSessione, int _idt = -1)
    {

      id = int.Parse(ID.ToString());
      idt = _idt;
      cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
      cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
      if (_idt == -1)
        dati = cBusinessObjects.GetData(id, typeof(TabellaSospesi));
      else
        dati = cBusinessObjects.GetData(id, typeof(TabellaSospesi), -1, -10, _idt);

      if (dati.Rows.Count == 0)
      {
        dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
      }
      foreach (DataRow dtrow in dati.Rows)
      {
        SospesiValue = (dtrow["SospesiTxt"].ToString() != "") ? dtrow["SospesiTxt"].ToString() : "";
      }

      if (Stato == App.TipoTreeNodeStato.Completato)
      {
        buttonElimina.Visibility = System.Windows.Visibility.Collapsed;
      }


      this.mainRTB.Selection.ApplyPropertyValue(FlowDocument.TextAlignmentProperty, TextAlignment.Justify);

      try
      {
        MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(SospesiValue));
        this.mainRTB.Selection.Load(stream, DataFormats.Rtf);

        TextRange tr = new TextRange(mainRTB.Document.ContentStart,
                    mainRTB.Document.ContentEnd);
        MemoryStream ms = new MemoryStream();
        tr.Save(ms, DataFormats.Text);
        txtNote.Text = ASCIIEncoding.Default.GetString(ms.ToArray());
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSospesi.Load exception");
        string log = ex.Message;
        txtNote.Text = "";
      }

      mainRTB.Focus();
      mainRTB.CaretPosition = mainRTB.Document.ContentEnd;
      mainRTB.ScrollToEnd();

      _DatiCambiati = false;
    }

    private void obj_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }
    }

    private void obj_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }
    }

    private void GestoreEvento_DatiCambiati(object sender, RoutedEventArgs e)
    {
      _DatiCambiati = true;
    }

    private void GestoreEvento_ChiusuraFinestra(object sender, CancelEventArgs e)
    {
      if (!_DatiCambiati)
        return;

      Utilities u = new Utilities();
      if (MessageBoxResult.No == u.AvvisoPerditaDati())
        e.Cancel = true;
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

    private void buttonElimina_Click(object sender, RoutedEventArgs e)
    {
      if (MessageBox.Show("Sicuri di voler eliminare questo sospeso?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
      {
        return;
      }
      dati.Clear();


      Changed = true;
      if (idt != -1)
        cBusinessObjects.SaveData(id, dati, typeof(TabellaSospesi), -1, idt);
      else
        cBusinessObjects.SaveData(id, dati, typeof(TabellaSospesi));

      _DatiCambiati = false;

      base.Close();
    }

    private void buttonSalva_Click(object sender, RoutedEventArgs e)
    {
      foreach (DataRow dtrow in dati.Rows)
      {
        if (dtrow["SospesiOLDTxt"].ToString() == "")
        {
          dtrow["SospesiOLDTxt"] = dtrow["SospesiTxt"];
        }
        TextRange tr = new TextRange(
        mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);
        MemoryStream ms = new MemoryStream();
        tr.Save(ms, DataFormats.Rtf);
        string xamlText = ASCIIEncoding.Default.GetString(ms.ToArray());
        dtrow["SospesiTxt"] =
          xamlText.Replace("\\f1", "\\f0").Replace(
            "\\f2", "\\f0").Replace(
              "{\\f0\\fcharset0 Times New Roman;}", "{\\f0 Arial;\\f1 Wingdings 2;\\f2 Wingdings;}");

        tr.Save(ms, DataFormats.Text);
        txtNote.Text = ASCIIEncoding.Default.GetString(ms.ToArray());
        dtrow["TitoloAttivita"] = cBusinessObjects.TitoloAttivita;

      }

      if (idt != -1)
        cBusinessObjects.SaveData(id, dati, typeof(TabellaSospesi), -1, idt);
      else
        cBusinessObjects.SaveData(id, dati, typeof(TabellaSospesi));



      Changed = true;


      _DatiCambiati = false;

      base.Close();
    }
  }
}
