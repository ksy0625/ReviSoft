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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using RevisoftApplication;
using System.Collections;
using System.IO;
using System.Data;

namespace UserControls
{
  public partial class ucTesto : UserControl
  {

    public int id;
    private DataTable dati = null;

    //private XmlDataProviderManager _x;
    private string _ID = "-1";
    private bool firsttime = true;

    private bool _ReadOnly = false;

    public ucTesto()
    {
      InitializeComponent();

  //    mainRTB.Focus();
    }

    public void FocusNow()
    {
   //   mainRTB.Focus();
    }

    public bool ReadOnly
    {
      set
      {
        _ReadOnly = value;

        mainRTB.IsReadOnly = _ReadOnly;
        txtValore.IsReadOnly = _ReadOnly;
      }
    }

    public void Load(string ID, string IDCliente, string IDSessione)
    {

        id = int.Parse(ID.ToString());
        cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
        cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());


         _ID = ID;
    

      dati = cBusinessObjects.GetData(id, typeof(Testi));
      if(dati.Rows.Count==0)
      {
                dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione,0,"",cBusinessObjects.empty_rtf);
      }

    
      string testo = "";
      foreach (DataRow dtrow in dati.Rows)
      {
        testo = dtrow["value"].ToString();
        txtTitolo.Text = dtrow["name"].ToString();
      }


      this.mainRTB.Selection.ApplyPropertyValue(FlowDocument.TextAlignmentProperty, TextAlignment.Justify);

      try
      {
        MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(testo));

        this.mainRTB.Selection.Load(stream, DataFormats.Rtf);

        this.mainRTB.ScrollToEnd();

        TextRange tr = new TextRange(mainRTB.Document.ContentStart,
                   mainRTB.Document.ContentEnd);
        MemoryStream ms = new MemoryStream();
        tr.Save(ms, DataFormats.Text);
        txtValore.Text = ASCIIEncoding.Default.GetString(ms.ToArray());
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        txtValore.Text = "";
      }

      //txtValore.Focus();

      mainRTB.PreviewKeyDown += OnClearClipboard;
    }

    private void OnClearClipboard(object sender, KeyEventArgs keyEventArgs)
    {
      if (keyEventArgs.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
      {
        if (Clipboard.ContainsImage())
        {
          Clipboard.Clear();
        }

        if (Clipboard.ContainsText())
        {
          string valueclipboard = Clipboard.GetText(TextDataFormat.Text).Trim();
          Clipboard.SetText(valueclipboard, TextDataFormat.Text);

          MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(valueclipboard));

          this.mainRTB.Selection.Load(stream, DataFormats.Rtf);

          this.mainRTB.ScrollToEnd();
        }
      }
    }

    public int Save()
    {

      TextRange tr = new TextRange(mainRTB.Document.ContentStart,
                      mainRTB.Document.ContentEnd);

      MemoryStream ms = new MemoryStream();
      tr.Save(ms, DataFormats.Rtf);
      string xamlText = ASCIIEncoding.Default.GetString(ms.ToArray());
      foreach (DataRow dtrow in dati.Rows)
      {
        dtrow["name"]=txtTitolo.Text;
        dtrow["value"] = xamlText.Replace("\\f1", "\\f0").Replace("\\f2", "\\f0").Replace("{\\f0\\fcharset0 Times New Roman;}", "{\\f0 Arial;\\f1 Wingdings 2;\\f2 Wingdings;}");

      }

      tr.Save(ms, DataFormats.Text);

      txtValore.Text = ASCIIEncoding.Default.GetString(ms.ToArray());

     
      return cBusinessObjects.SaveData(id, dati, typeof(Testi));

    }

   

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (firsttime)
      {
        firsttime = false;
        return;
      }
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

      //if ( !txtValore.IsFocused )
      //{
      //    FocusNow();
      //}
    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      double tmp = e.NewSize.Width - 80.0;

      if (tmp <= 20)
      {
        return;
      }

      txtTitolo.Width = tmp - 20;
      txtValore.Width = tmp - 20;
      grdMainContainer.Width = tmp;

      FocusNow();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      FocusNow();
      //if ( _ID == "37" || _ID == "38" || _ID == "40" || _ID == "41" || _ID == "42" || _ID == "46" || _ID == "58" || _ID == "59" || _ID == "60" || _ID == "63" )
      //{
      //    MessageBox.Show( "Per disporre di strumenti di FORMATTAZIONE usare i COMMENTI anziché lo spazio di default" );
      //}
    }
  }
}
