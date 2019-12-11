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
  public partial class OsservazioniConclusive : Window
  {
    public int id;
    private DataTable dati = null;
    private bool _DatiCambiati;

    private bool _ReadOnly = false;
    public App.TipoTreeNodeStato Stato = App.TipoTreeNodeStato.Sconosciuto;

    public OsservazioniConclusive()
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

    private void btn_TestoDaStampare_Click(object sender, RoutedEventArgs e)
    {

      TestoDaStampare o = new TestoDaStampare();
      o.Owner = ((WindowWorkArea)(Owner));

      o.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
      o.Top = this.Top + 30.0;
      o.Left = this.Left + 30.0;

      if (System.Windows.SystemParameters.PrimaryScreenWidth < 1100 || System.Windows.SystemParameters.PrimaryScreenHeight < 600)
      {
        o.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 80.0 / 100.0;
        o.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 80.0 / 100.0;
        o.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 80.0 / 100.0;
        o.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 80.0 / 100.0;
        o.MinHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 80.0 / 100.0;
        o.MinWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 80.0 / 100.0;
      }
      else
      {
        o.Width = 1100;
        o.Height = 600;
      }

      o.ReadOnly = _ReadOnly;

      o.Stato = Stato;



      o.Load();

      o.ShowDialog();


    }

    public void Load(string ID, string IDCliente, string IDSessione)
    {
      id = int.Parse(ID.ToString());
      cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
      cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
      dati = cBusinessObjects.GetData(id, typeof(Osservazioni));
      if (dati.Rows.Count == 0)
      {
        dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
      }

      string osservazioni = "";
      foreach (DataRow dtrow in dati.Rows)
      {
        osservazioni = dtrow["OsservazioniTxt"].ToString();
      }

      btn_TestoDaStampare.Visibility = System.Windows.Visibility.Collapsed;

      switch ((App.TipoFile)(System.Convert.ToInt32(((WindowWorkArea)(Owner)).IDTree)))
      {
        case App.TipoFile.Verifica:
          btn_TestoDaStampare.Visibility = System.Windows.Visibility.Visible;
          break;
        default:
          break;
      }

      if (Stato == App.TipoTreeNodeStato.Completato)
      {
        buttonElimina.Visibility = System.Windows.Visibility.Collapsed;
      }

      this.mainRTB.Selection.ApplyPropertyValue(FlowDocument.TextAlignmentProperty, TextAlignment.Justify);

      try
      {
        MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(osservazioni));
        this.mainRTB.Selection.Load(stream, DataFormats.Rtf);

        TextRange tr = new TextRange(mainRTB.Document.ContentStart,
                    mainRTB.Document.ContentEnd);
        MemoryStream ms = new MemoryStream();
        tr.Save(ms, DataFormats.Text);
        txtNote.Text = ASCIIEncoding.Default.GetString(ms.ToArray());
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wOsservazioni.Load exception");
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
      dati.Clear();
      cBusinessObjects.SaveData(id, dati, typeof(Osservazioni));

      _DatiCambiati = false;

      base.Close();
    }

    //----------------------------------------------------------------------------+
    //                             buttonSalva_Click                              |
    //----------------------------------------------------------------------------+
    private void buttonSalva_Click(object sender, RoutedEventArgs e)
    {

      foreach (DataRow dtrow in dati.Rows)
      {

        if (dtrow["OsservazioniOLDTxt"].ToString() == "")
        {
          dtrow["OsservazioniOLDTxt"] = dtrow["OsservazioniTxt"];
        }
        TextRange tr = new TextRange(
        mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);
        MemoryStream ms = new MemoryStream();
        tr.Save(ms, DataFormats.Rtf);
        string xamlText = ASCIIEncoding.Default.GetString(ms.ToArray());
        dtrow["OsservazioniTxt"] =
          xamlText.Replace("\\f1", "\\f0").Replace(
            "\\f2", "\\f0").Replace(
              "{\\f0\\fcharset0 Times New Roman;}", "{\\f0 Arial;\\f1 Wingdings 2;\\f2 Wingdings;}");

        tr.Save(ms, DataFormats.Text);
        txtNote.Text = ASCIIEncoding.Default.GetString(ms.ToArray());
      }

      cBusinessObjects.SaveData(id, dati, typeof(Osservazioni));

      _DatiCambiati = false;
      if (btn_TestoDaStampare.Visibility == System.Windows.Visibility.Collapsed)
      {
        base.Close();
      }

    }
  }
}
