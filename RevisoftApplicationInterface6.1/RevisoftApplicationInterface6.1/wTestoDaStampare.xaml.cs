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

namespace RevisoftApplication
{
  public partial class TestoDaStampare : Window
  {
    private bool _DatiCambiati;
    public string Nodo = "-1";
    private bool _ReadOnly = false;
    public App.TipoTreeNodeStato Stato = App.TipoTreeNodeStato.Sconosciuto;

    public TestoDaStampare()
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

    public void Load()
    {
      XmlNode node = ((WindowWorkArea)(Owner))._x.Document.SelectSingleNode("/Dati//Dato[@ID='" + Nodo + "']");

      if (Stato == App.TipoTreeNodeStato.Completato)
      {
        buttonElimina.Visibility = System.Windows.Visibility.Collapsed;
      }

      this.mainRTB.Selection.ApplyPropertyValue(FlowDocument.TextAlignmentProperty, TextAlignment.Justify);

      if (node != null)
      {
        try
        {
          MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(node.Attributes["TestoDaStampare"].Value));
          this.mainRTB.Selection.Load(stream, DataFormats.Rtf);

          TextRange tr = new TextRange(mainRTB.Document.ContentStart,
                     mainRTB.Document.ContentEnd);
          MemoryStream ms = new MemoryStream();
          tr.Save(ms, DataFormats.Text);
          txtNote.Text = ASCIIEncoding.Default.GetString(ms.ToArray());
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wTestoDaStampare.Load exception");
          string log = ex.Message;
          txtNote.Text = "";
        }
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
      XmlNode node = ((WindowWorkArea)(Owner))._x.Document.SelectSingleNode("/Dati//Dato[@ID='" + Nodo + "']");

      if (node != null)
      {
        try
        {
          node.Attributes["TestoDaStampare"].Value = "";
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wTestoDaStampare.buttonElimina_Click exception");
          string log = ex.Message;
          XmlAttribute attr = node.OwnerDocument.CreateAttribute("TestoDaStampare");
          attr.Value = "";
          node.Attributes.Append(attr);
        }
      }

        ((WindowWorkArea)(Owner))._x.Save();

      _DatiCambiati = false;

      base.Close();
    }

    private void buttonCopia_Click(object sender, RoutedEventArgs e)
    {
      XmlNode node = ((WindowWorkArea)(Owner))._x.Document.SelectSingleNode("/Dati//Dato[@ID='" + Nodo + "']");
      if (node != null & node.Attributes["Osservazioni"] != null)
      {
        if (node.Attributes["TestoDaStampare"] == null)
        {
          XmlAttribute attr = node.OwnerDocument.CreateAttribute("TestoDaStampare");
          node.Attributes.Append(attr);
        }

        node.Attributes["TestoDaStampare"].Value = node.Attributes["Osservazioni"].Value;

        ((WindowWorkArea)(Owner))._x.Save();

        Load();

        _DatiCambiati = true;
      }
    }

    private void buttonSalva_Click(object sender, RoutedEventArgs e)
    {
      XmlNode node = ((WindowWorkArea)(Owner))._x.Document.SelectSingleNode("/Dati//Dato[@ID='" + Nodo + "']");

      if (node != null)
      {
        if (node.Attributes["TestoDaStampare"] == null)
        {
          XmlAttribute attr = node.OwnerDocument.CreateAttribute("TestoDaStampare");
          node.Attributes.Append(attr);
        }

        if (node.Attributes["TestoDaStampareOLD"] == null)
        {
          XmlAttribute attr = node.OwnerDocument.CreateAttribute("TestoDaStampareOLD");
          attr.Value = node.Attributes["TestoDaStampare"].Value;
          node.Attributes.Append(attr);
        }

        TextRange tr = new TextRange(mainRTB.Document.ContentStart,
                    mainRTB.Document.ContentEnd);
        MemoryStream ms = new MemoryStream();
        tr.Save(ms, DataFormats.Rtf);
        string xamlText = ASCIIEncoding.Default.GetString(ms.ToArray());

        node.Attributes["TestoDaStampare"].Value = xamlText.Replace("\\f1", "\\f0").Replace("\\f2", "\\f0").Replace("{\\f0\\fcharset0 Times New Roman;}", "{\\f0 Arial;\\f1 Wingdings 2;\\f2 Wingdings;}");

        tr.Save(ms, DataFormats.Text);
        txtNote.Text = ASCIIEncoding.Default.GetString(ms.ToArray());
      }

      ((WindowWorkArea)(Owner))._x.Save();

      _DatiCambiati = false;

      base.Close();
    }
  }
}
