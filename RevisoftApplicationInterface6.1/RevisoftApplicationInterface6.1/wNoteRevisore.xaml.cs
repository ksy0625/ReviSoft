using RevisoftApplication.BRL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace RevisoftApplication
{
  /// <summary>
  /// Interaction logic for wNoteRevisore.xaml
  /// </summary>
  public partial class wNoteRevisore : Window
  {
    private bool datiCambiati;
    private NoteXRevisore nota;

    public wNoteRevisore(string idCliente, string codice)
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      nota = cNote.GetNote(App.AppUtente.Id, idCliente, codice);
      if (nota == null)
        nota = new NoteXRevisore { NXR_UTE_ID = App.AppUtente.Id, NXR_CLI_ID = idCliente, NXR_COD_ID = codice, NXR_NOTE = string.Empty };
      if (!string.IsNullOrEmpty(nota.NXR_NOTE))
        SetXaml(mainRTB, nota.NXR_NOTE);
      datiCambiati = false;
    }

    private void buttonElimina_Click(object sender, RoutedEventArgs e)
    {
      SetXaml(mainRTB, string.Empty);
      datiCambiati = true;
    }

    private void buttonSalva_Click(object sender, RoutedEventArgs e)
    {
      SalvaNota();
      Close();
    }

    private void GestoreEvento_ChiusuraFinestra(object sender, CancelEventArgs e) {
      if (!datiCambiati)
        return;
      var messageBoxResult = MessageBox.Show("Vuoi salvare i cambiamenti?", "Note Revisore", MessageBoxButton.YesNo);
      if (messageBoxResult == MessageBoxResult.Yes)
        SalvaNota();
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void SalvaNota()
    {
      if (!datiCambiati)
        return;
      nota.NXR_NOTE = GetXaml(mainRTB);
      cNote.UpsertNote(nota.NXR_UTE_ID.Value, nota.NXR_CLI_ID, nota.NXR_COD_ID, nota.NXR_NOTE);
      datiCambiati = false;
    }

    private void GestoreEvento_DatiCambiati(object sender, RoutedEventArgs e)
    {
      datiCambiati = true;
    }

    static string GetXaml(RichTextBox rt)
    {
      TextRange range = new TextRange(rt.Document.ContentStart, rt.Document.ContentEnd);
      MemoryStream stream = new MemoryStream();
      range.Save(stream, DataFormats.Xaml);
      string xamlText = Encoding.UTF8.GetString(stream.ToArray());
      return xamlText;
    }

    static void SetXaml(RichTextBox rt, string xamlString)
    {
      try
      {
        StringReader stringReader = new StringReader(xamlString);
        XmlReader xmlReader = XmlReader.Create(stringReader);
        Section sec = XamlReader.Load(xmlReader) as Section;
        FlowDocument doc = new FlowDocument();
        while (sec.Blocks.Count > 0)
          doc.Blocks.Add(sec.Blocks.FirstBlock);
        rt.Document = doc;
      }
      catch(Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wNoteRevisore.SetXaml exception");
        rt.Document.Blocks.Clear();
        rt.Document.Blocks.Add(new Paragraph(new Run(xamlString)));
      }
    }

  }
}
