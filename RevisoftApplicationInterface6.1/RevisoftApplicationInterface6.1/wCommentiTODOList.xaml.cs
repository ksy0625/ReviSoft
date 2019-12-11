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
  public partial class wCommentiTODOList : Window
  {
    private bool _DatiCambiati;
    public string Nodo = "-1";
    public bool NotEmpty = false;
    private bool _ReadOnly = false;

    public wCommentiTODOList()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      txtNote.Focus();
      _DatiCambiati = false;
    }

    public bool ReadOnly
    {
      set
      {
        _ReadOnly = value;

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
      XmlNode node = ((WindowWorkAreaTree_TODOList)(Owner))._xTXP.Document.SelectSingleNode("//Node[@ID='" + Nodo + "']");


      this.mainRTB.Selection.ApplyPropertyValue(FlowDocument.TextAlignmentProperty, TextAlignment.Justify);

      if (node != null)
      {
        try
        {
          MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(node.Attributes["NotaTDL"].Value));
          this.mainRTB.Selection.Load(stream, DataFormats.Rtf);

          TextRange tr = new TextRange(mainRTB.Document.ContentStart,
                     mainRTB.Document.ContentEnd);
          MemoryStream ms = new MemoryStream();
          tr.Save(ms, DataFormats.Text);
          txtNote.Text = ASCIIEncoding.Default.GetString(ms.ToArray());

          if (txtNote.Text.Trim() != "")
          {
            NotEmpty = true;
          }
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wCommentiTODOList.Load exception");
          string log = ex.Message;
          txtNote.Text = "";
        }
      }

      txtNote.Focus();
      _DatiCambiati = false;
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
      XmlNode node = ((WindowWorkAreaTree_TODOList)(Owner))._xTXP.Document.SelectSingleNode("//Node[@ID='" + Nodo + "']");

      if (node != null)
      {
        try
        {
          node.Attributes["NotaTDL"].Value = "";

          NotEmpty = false;
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wCommentiTODOList.buttonElimina_Click exception");
          string log = ex.Message;
          XmlAttribute attr = node.OwnerDocument.CreateAttribute("NotaTDL");
          attr.Value = "";
          node.Attributes.Append(attr);
          NotEmpty = false;
        }
      }

        ((WindowWorkAreaTree_TODOList)(Owner))._xTXP.Save();

      _DatiCambiati = false;

      base.Close();
    }

    private void buttonSalva_Click(object sender, RoutedEventArgs e)
    {
      XmlNode node = ((WindowWorkAreaTree_TODOList)(Owner))._xTXP.Document.SelectSingleNode("//Node[@ID='" + Nodo + "']");

      if (node != null)
      {
        if (node.Attributes["NotaTDL"] == null)
        {
          XmlAttribute attr = node.OwnerDocument.CreateAttribute("NotaTDL");
          node.Attributes.Append(attr);
        }

        TextRange tr = new TextRange(mainRTB.Document.ContentStart,
                    mainRTB.Document.ContentEnd);
        MemoryStream ms = new MemoryStream();
        tr.Save(ms, DataFormats.Rtf);
        string xamlText = ASCIIEncoding.Default.GetString(ms.ToArray());

        node.Attributes["NotaTDL"].Value = xamlText;

        tr.Save(ms, DataFormats.Text);
        txtNote.Text = ASCIIEncoding.Default.GetString(ms.ToArray());

        if (txtNote.Text.Trim() != "")
        {
          NotEmpty = true;
        }
        else
        {
          NotEmpty = false;
        }
      }

            ((WindowWorkAreaTree_TODOList)(Owner))._xTXP.Save();

      _DatiCambiati = false;

      base.Close();
    }
  }
}
