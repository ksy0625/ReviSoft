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
using UserControls;
using System.Media;
using System.Threading;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;

namespace RevisoftApplication
{
  public partial class wSchedaBilancioVerifica : Window
  {
    public int id;
    public DataTable datiBV = null;

    public ExcelPackage excelworkBook;
    public ExcelWorksheet excelSheet;

    public string Nomefile = "";
    public string IDCLiente = "";
    public string tipoBilancio = "";
    public string IDB_Padre = "";

    bool alternate = false;

    bool checkTotaleAZero = false;

    string PerditaUtileID = "";
    string ArrotondamentoID = "";

    private string left = "./Images/icone/wa_nav_sess_prev.png";
    private string right = "./Images/icone/wa_nav_sess_next.png";

    public string esercizioinesame = "EA"; //"EP";

    private XmlNode nodeBV = null;
    public DataRow nodehere = null;
    public DataTable datibilanciohere = null;



    int lastRowIncludeFormulas = 0;

    MasterFile mf = MasterFile.Create();

    bool esistealmenounavocenonassociata = false;
    bool esistealmenounavoce = false;

    string templateA = "";
    string templateB = "";
    string templateC = "";

    Hashtable valorisomma = new Hashtable();
    Hashtable valoridareavere = new Hashtable();

    Hashtable valorilabel = new Hashtable();

    int rowintestazione = 1;

    DataTable RawData = null;

    List<string> ContoEconomico = new List<string>();
    List<string> PatAttivo = new List<string>();
    List<string> PatPassivo = new List<string>();

    public class ComboboxItem
    {
      public string Text { get; set; }
      public string Value { get; set; }

      public override string ToString()
      {
        return Text;
      }
    }

    public wSchedaBilancioVerifica(string _id)
    {
      id = int.Parse(_id);
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
    }

    public bool Load()
    {
      if (IDCLiente == "")
      {
        return false;
      }


      if (IDB_Padre == "227" || IDB_Padre == "134" || IDB_Padre == "2016134" || IDB_Padre == "166" || IDB_Padre == "321")
      {
        switch (tipoBilancio)
        {
          case "consolidato":
            templateA = App.AppTemplateBilancio_Attivo2016_Consolidato;
            templateB = App.AppTemplateBilancio_Passivo2016_Consolidato;
            templateC = App.AppTemplateBilancio_ContoEconomico2016_Consolidato;

            valorisomma = new Hashtable();
            valorisomma.Add("31", "32|33");
            valorisomma.Add("34", "35|36");
            valorisomma.Add("37", "38|39");
            valorisomma.Add("40", "41|42");
            valorisomma.Add("201637", "201638|201639");
            valorisomma.Add("201676", "201677|201678");
            valorisomma.Add("58", "59|60");
            valorisomma.Add("61", "62|63");
            valorisomma.Add("64", "65|66");
            valorisomma.Add("67", "68|69");
            valorisomma.Add("70", "71|72");
            valorisomma.Add("76", "77|78");
            valorisomma.Add("132", "133|134");
            valorisomma.Add("135", "136|137");
            valorisomma.Add("138", "139|140");
            valorisomma.Add("141", "142|143");
            valorisomma.Add("144", "145|146");
            valorisomma.Add("147", "148|149");
            valorisomma.Add("150", "151|152");
            valorisomma.Add("153", "154|155");
            valorisomma.Add("156", "157|158");
            valorisomma.Add("159", "160|161");
            valorisomma.Add("162", "163|164");
            valorisomma.Add("2016162", "2016163|2016164");
            valorisomma.Add("165", "166|167");
            valorisomma.Add("168", "169|170");
            valorisomma.Add("171", "172|173");

            valoridareavere = new Hashtable();
            valoridareavere.Add("Crediti e debiti verso imprese controllate", "61|156");
            valoridareavere.Add("Crediti e debiti verso imprese collegate", "64|159");
            valoridareavere.Add("Crediti e debiti verso imprese controllanti", "67|162");
            valoridareavere.Add("Crediti e debiti verso imprese sott. Contr. Controllanti", "201676|2016162");
            valoridareavere.Add("Crediti e debiti tributari", "70|165");
            valoridareavere.Add("Debiti e crediti previdenza sociale", "76|168");
            valoridareavere.Add("Debiti e crediti verso altri", "76|171");
            valoridareavere.Add("Crediti e debiti verso banche e poste", "90|141");

            PerditaUtileID = "11611";
            ArrotondamentoID = "20161143";

            break;
          case "2016":
            templateA = App.AppTemplateBilancio_Attivo2016;
            templateB = App.AppTemplateBilancio_Passivo2016;
            templateC = App.AppTemplateBilancio_ContoEconomico2016;

            valorisomma = new Hashtable();
            valorisomma.Add("31", "32|33");
            valorisomma.Add("34", "35|36");
            valorisomma.Add("37", "38|39");
            valorisomma.Add("40", "41|42");
            valorisomma.Add("201637", "201638|201639");
            valorisomma.Add("201676", "201677|201678");
            valorisomma.Add("58", "59|60");
            valorisomma.Add("61", "62|63");
            valorisomma.Add("64", "65|66");
            valorisomma.Add("67", "68|69");
            valorisomma.Add("70", "71|72");
            valorisomma.Add("76", "77|78");
            valorisomma.Add("132", "133|134");
            valorisomma.Add("135", "136|137");
            valorisomma.Add("138", "139|140");
            valorisomma.Add("141", "142|143");
            valorisomma.Add("144", "145|146");
            valorisomma.Add("147", "148|149");
            valorisomma.Add("150", "151|152");
            valorisomma.Add("153", "154|155");
            valorisomma.Add("156", "157|158");
            valorisomma.Add("159", "160|161");
            valorisomma.Add("162", "163|164");
            valorisomma.Add("2016162", "2016163|2016164");
            valorisomma.Add("165", "166|167");
            valorisomma.Add("168", "169|170");
            valorisomma.Add("171", "172|173");

            valoridareavere = new Hashtable();
            valoridareavere.Add("Crediti e debiti verso imprese controllate", "61|156");
            valoridareavere.Add("Crediti e debiti verso imprese collegate", "64|159");
            valoridareavere.Add("Crediti e debiti verso imprese controllanti", "67|162");
            valoridareavere.Add("Crediti e debiti verso imprese sott. Contr. Controllanti", "201676|2016162");
            valoridareavere.Add("Crediti e debiti tributari", "70|165");
            valoridareavere.Add("Debiti e crediti previdenza sociale", "76|168");
            valoridareavere.Add("Debiti e crediti verso altri", "76|171");
            valoridareavere.Add("Crediti e debiti verso banche e poste", "90|141");

            PerditaUtileID = "11611";
            ArrotondamentoID = "20161143";

            break;
          default:
            templateA = App.AppTemplateBilancio_Attivo;
            templateB = App.AppTemplateBilancio_Passivo;
            templateC = App.AppTemplateBilancio_ContoEconomico;

            valorisomma = new Hashtable();
            valorisomma.Add("31", "32|33");
            valorisomma.Add("34", "35|36");
            valorisomma.Add("37", "38|39");
            valorisomma.Add("40", "41|42");
            valorisomma.Add("58", "59|60");
            valorisomma.Add("61", "62|63");
            valorisomma.Add("64", "65|66");
            valorisomma.Add("67", "68|69");
            valorisomma.Add("70", "71|72");
            valorisomma.Add("73", "74|75");
            valorisomma.Add("76", "77|78");
            valorisomma.Add("132", "133|134");
            valorisomma.Add("135", "136|137");
            valorisomma.Add("138", "139|140");
            valorisomma.Add("141", "142|143");
            valorisomma.Add("144", "145|146");
            valorisomma.Add("147", "148|149");
            valorisomma.Add("150", "151|152");
            valorisomma.Add("153", "154|155");
            valorisomma.Add("156", "157|158");
            valorisomma.Add("159", "160|161");
            valorisomma.Add("162", "163|164");
            valorisomma.Add("165", "166|167");
            valorisomma.Add("168", "169|170");
            valorisomma.Add("171", "172|173");

            valoridareavere = new Hashtable();
            valoridareavere.Add("Crediti e debiti verso imprese controllate", "61|156");
            valoridareavere.Add("Crediti e debiti verso imprese collegate", "64|159");
            valoridareavere.Add("Crediti e debiti verso imprese controllanti", "67|162");
            valoridareavere.Add("Crediti e debiti tributari", "70|165");
            valoridareavere.Add("Debiti e crediti previdenza sociale", "76|168");
            valoridareavere.Add("Debiti e crediti verso altri", "76|171");
            valoridareavere.Add("Crediti e debiti verso banche e poste", "90|141");

            PerditaUtileID = "11611";
            ArrotondamentoID = "118";

            break;
        }
      }
      else
      {
        switch (tipoBilancio)
        {
          case "Micro":
            templateA = App.AppTemplateBilancioMicro_Attivo2016;
            templateB = App.AppTemplateBilancioMicro_Passivo2016;
            templateC = App.AppTemplateBilancioMicro_ContoEconomico2016;

            valorisomma = new Hashtable();
            valorisomma.Add("57", "1059|1060");
            valorisomma.Add("131", "133|134");

            valoridareavere = new Hashtable();
            valoridareavere.Add("Disponibilità liquide/debiti bancari", "89|131");
            valoridareavere.Add("crediti/debiti", "57|131");

            PerditaUtileID = "11611";
            ArrotondamentoID = "100114";

            break;
          case "2016":
            templateA = App.AppTemplateBilancioAbbreviato_Attivo2016;
            templateB = App.AppTemplateBilancioAbbreviato_Passivo2016;
            templateC = App.AppTemplateBilancioAbbreviato_ContoEconomico2016;

            valorisomma = new Hashtable();
            valorisomma.Add("57", "1059|1060");
            valorisomma.Add("131", "133|134");

            valoridareavere = new Hashtable();
            valoridareavere.Add("Disponibilità liquide/debiti bancari", "89|131");
            valoridareavere.Add("crediti/debiti", "57|131");

            PerditaUtileID = "11611";
            ArrotondamentoID = "100114";

            break;
          default:
            templateA = App.AppTemplateBilancioAbbreviato_Attivo;
            templateB = App.AppTemplateBilancioAbbreviato_Passivo;
            templateC = App.AppTemplateBilancioAbbreviato_ContoEconomico;

            valorisomma = new Hashtable();
            valorisomma.Add("57", "1059|1060");
            valorisomma.Add("131", "133|134");
            valorisomma.Add("10091", "10092|10093");

            valoridareavere = new Hashtable();
            valoridareavere.Add("Disponibilità liquide/debiti bancari", "89|131");
            valoridareavere.Add("crediti/debiti", "57|131");

            PerditaUtileID = "120";
            ArrotondamentoID = "100114";

            break;
        }
      }

      nodeBV = mf.GetAnagraficaBV(Convert.ToInt32(IDCLiente));


      if (nodeBV == null)
      {
        string xmlBV = "<BilancioVerifica rowintestazione=\"0\" codice=\"0\" descrizione=\"0\" saldo=\"0\" saldod=\"0\" saldoa=\"0\" />";
        XmlDocument doctmpBV = new XmlDocument();
        doctmpBV.LoadXml(xmlBV);

        nodeBV = doctmpBV.SelectSingleNode("/BilancioVerifica");
      }

      if (Nomefile != "")
      {

        //excel.Calculation = Microsoft.Office.Interop.Excel.XlCalculation.xlCalculationManual;
        var file = new FileInfo(Nomefile);
        excelworkBook = new ExcelPackage(file);

        if (excelworkBook.Compatibility.IsWorksheets1Based)
          excelSheet = excelworkBook.Workbook.Worksheets[1];
        else
          excelSheet = excelworkBook.Workbook.Worksheets[0];

      }

      CreateInterface();

      if (Nomefile == "")
      {
        GetDataSenzaExcel(true);
        if (esistealmenounavoce == false)
        {
          MessageBox.Show("Non è mai stata fatta una importazione BV di associazione. Selezionare un file XLS di esempio.");
          return false;
        }

        StackPanel stpIntestazione = (StackPanel)this.FindName("stpIntestazione");
        stpIntestazione.Visibility = Visibility.Collapsed;

        StackPanel stpScelte = (StackPanel)this.FindName("stpScelte");
        stpScelte.Visibility = Visibility.Collapsed;

        if (esistealmenounavocenonassociata)
        {
          StackPanel stpBV = (StackPanel)this.FindName("stpCestino");
          stpBV.Visibility = Visibility.Visible;

          StackPanel stpBVCollector = (StackPanel)this.FindName("stpBVCollector");
          stpBVCollector.Visibility = Visibility.Collapsed;

          StackPanel stpLastControls = (StackPanel)this.FindName("stpLastControls");
          stpLastControls.Visibility = Visibility.Collapsed;

          VisualizzaListaDaAssociare_Cestino();
          VisualizzaListaAssociate_Cestino();
        }
        else
        {
          StackPanel stpBV = (StackPanel)this.FindName("stpCestino");
          stpBV.Visibility = Visibility.Collapsed;

          StackPanel stpBVCollector = (StackPanel)this.FindName("stpBVCollector");
          stpBVCollector.Visibility = Visibility.Collapsed;

          StackPanel stpLastControls = (StackPanel)this.FindName("stpLastControls");
          stpLastControls.Visibility = Visibility.Visible;

          VisualizzaListaFinal();
        }
      }
      else
      {
        try
        {
          GetDataFromExcel(true);

          if (esistealmenounavoce == false)
          {
            return true;
          }

          StackPanel stpIntestazione = (StackPanel)this.FindName("stpIntestazione");
          stpIntestazione.Visibility = Visibility.Collapsed;

          StackPanel stpScelte = (StackPanel)this.FindName("stpScelte");
          stpScelte.Visibility = Visibility.Collapsed;

          if (esistealmenounavocenonassociata)
          {
            StackPanel stpBV = (StackPanel)this.FindName("stpCestino");
            stpBV.Visibility = Visibility.Visible;

            StackPanel stpBVCollector = (StackPanel)this.FindName("stpBVCollector");
            stpBVCollector.Visibility = Visibility.Collapsed;

            StackPanel stpLastControls = (StackPanel)this.FindName("stpLastControls");
            stpLastControls.Visibility = Visibility.Collapsed;

            VisualizzaListaDaAssociare_Cestino();
            VisualizzaListaAssociate_Cestino();
          }
          else
          {
            StackPanel stpBV = (StackPanel)this.FindName("stpCestino");
            stpBV.Visibility = Visibility.Collapsed;

            StackPanel stpBVCollector = (StackPanel)this.FindName("stpBVCollector");
            stpBVCollector.Visibility = Visibility.Collapsed;

            StackPanel stpLastControls = (StackPanel)this.FindName("stpLastControls");
            stpLastControls.Visibility = Visibility.Visible;

            VisualizzaListaFinal();
          }
        }
        catch (Exception ex)
        {
          string log = ex.Message;
        }
      }

      return true;
    }

    private void CreateInterface()
    {
      valorilabel = new Hashtable();

      #region riga intestazione

      StackPanel stpIntestazione = new StackPanel();
      stpIntestazione.Margin = new Thickness(10);
      stpIntestazione.Orientation = Orientation.Vertical;
      stpIntestazione.Name = "stpIntestazione";
      this.RegisterName(stpIntestazione.Name, stpIntestazione);

      StackPanel stpRiga_1 = new StackPanel();
      stpRiga_1.Orientation = Orientation.Vertical;

      StackPanel stpRiga_label = new StackPanel();
      stpRiga_label.Orientation = Orientation.Vertical;

      StackPanel stptext = new StackPanel();
      stptext.Orientation = Orientation.Horizontal;

      TextBlock lbl = new TextBlock();
      lbl.Text = "Il file XLS ";
      stptext.Children.Add(lbl);

      lbl = new TextBlock();
      lbl.FontWeight = FontWeights.Bold;
      lbl.Text = "non dovrà esporre il risultato economico.";
      stptext.Children.Add(lbl);

      lbl = new TextBlock();
      lbl.Text = " che verrà calcolato ed allocato da Revisoft.";
      stptext.Children.Add(lbl);

      stpRiga_label.Children.Add(stptext);

      lbl = new TextBlock();
      lbl.Text = "Qualora fossero esposte le righe corrispondenti dovranno essere cestinate (Fase 3).";
      stpRiga_label.Children.Add(lbl);

      lbl = new TextBlock();
      lbl.Margin = new Thickness(0, 15, 0, 0);
      lbl.Text = "Tutte le impostazioni nelle fasi descritte verranno memorizzate; l’importazione successiva richiederà il collegamento dei soli eventuali conti non presenti in precedenza, che il software evidenzierà.";
      stpRiga_label.Children.Add(lbl);

      lbl = new TextBlock();
      lbl.Margin = new Thickness(0, 15, 0, 0);
      lbl.FontWeight = FontWeights.Bold;
      lbl.Text = "Fase 1) Selezionare la riga contenente l'intestazione.";
      stpRiga_label.Children.Add(lbl);

      lbl = new TextBlock();
      lbl.Text = "Selezionare la riga che indica il contenuto delle colonne del file XLS da importare (codice cliente, descrizione, saldo).";
      stpRiga_label.Children.Add(lbl);

      lbl = new TextBlock();
      lbl.Text = "Il software non importerà le righe che precedono quella selezionata.";
      stpRiga_label.Children.Add(lbl);

      stpRiga_1.Children.Add(stpRiga_label);



      lastRowIncludeFormulas = 20;

      if (Nomefile != "")
      {


        lastRowIncludeFormulas = excelSheet.Dimension.End.Row;

      }

      ComboBox lst = new ComboBox();
      lst.VerticalAlignment = VerticalAlignment.Center;
      lst.Margin = new Thickness(10);
      lst.Items.Clear();
      lst.Name = "lst_Intestazione";
      for (int i = 1; i <= lastRowIncludeFormulas; i++)
      {
        lst.Items.Add(i.ToString());
      }
      lst.SelectionChanged += Lst_SelectionChanged;
      this.RegisterName(lst.Name, lst);

      ScrollViewer sw = new ScrollViewer();
      sw.MaxWidth = 1000.0;
      sw.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

      StackPanel stpRiga_2 = new StackPanel();
      stpRiga_2.Name = "Riga2";
      stpRiga_2.Orientation = Orientation.Horizontal;
      this.RegisterName(stpRiga_2.Name, stpRiga_2);

      sw.Content = stpRiga_2;

      stpRiga_1.Children.Add(lst);

      stpIntestazione.Children.Add(stpRiga_1);
      stpIntestazione.Children.Add(sw);

      //Button btn = new Button();
      //btn.HorizontalAlignment = HorizontalAlignment.Right;
      //btn.Width = 100.0;
      //btn.Content = "Avanti";
      //btn.Click += Btn_Next_SceltaRigaIntestazione_Click;
      //stpIntestazione.Children.Add(btn);

      stackPanel1.Children.Add(stpIntestazione);

      #endregion

      #region riga scelta colonne

      StackPanel stpScelte = new StackPanel();
      stpScelte.Margin = new Thickness(10);
      stpScelte.Name = "stpScelte";
      this.RegisterName(stpScelte.Name, stpScelte);

      stpRiga_label = new StackPanel();
      stpRiga_label.Orientation = Orientation.Vertical;

      lbl = new TextBlock();
      lbl.Text = "Fase 2) Selezionare le colonne.";
      lbl.FontWeight = FontWeights.Bold;
      stpRiga_label.Children.Add(lbl);

      lbl = new TextBlock();
      lbl.Text = "Indicare a fianco di ciascuna voce sottostante, la LETTERA della COLONNA del file XLS contenente i dati da importare con dell’apposita tendina.";
      stpRiga_label.Children.Add(lbl);

      lbl = new TextBlock();
      lbl.Margin = new Thickness(0, 10, 0, 0);
      lbl.Text = "In particolare:";
      stpRiga_label.Children.Add(lbl);

      stptext = new StackPanel();
      stptext.Margin = new Thickness(0, 10, 0, 0);
      stptext.Orientation = Orientation.Horizontal;

      lbl = new TextBlock();
      lbl.Text = "SALDO: ";
      lbl.FontWeight = FontWeights.Bold;
      stptext.Children.Add(lbl);

      lbl = new TextBlock();
      lbl.Text = "dovrà essere indicata la lettera dell’unica colonna del file XLS contenente i saldi dei conti, nel bilancio di verifica, con segno + / - per Dare / Avere";
      stptext.Children.Add(lbl);

      stpRiga_label.Children.Add(stptext);

      stptext = new StackPanel();
      stptext.Margin = new Thickness(0, 10, 0, 0);
      stptext.Orientation = Orientation.Horizontal;

      lbl = new TextBlock();
      lbl.Text = "Saldo DARE – Saldo AVERE: ";
      lbl.FontWeight = FontWeights.Bold;
      stptext.Children.Add(lbl);

      lbl = new TextBlock();
      lbl.Text = "se i saldi dei conti nel bilancio di verifica sono esposti su due colonne per DARE e AVERE, dovranno essere indicate le lettere delle colonne corrispondenti del file XLS.";
      stptext.Children.Add(lbl);

      stpRiga_label.Children.Add(stptext);

      stpScelte.Children.Add(stpRiga_label);

      StackPanel stpRiga_Codice = new StackPanel();
      stpRiga_Codice.Margin = new Thickness(0, 10, 0, 0);
      stpRiga_Codice.Orientation = Orientation.Horizontal;
      lbl = new TextBlock();
      lbl.Text = "Codice";
      lbl.Width = 100;
      stpRiga_Codice.Children.Add(lbl);

      ComboBox lst2 = new ComboBox();
      lst2.Name = "lst2_Codice";
      lst2.Width = 200;
      lst2.Margin = new Thickness(10, 0, 0, 0);
      this.RegisterName(lst2.Name, lst2);
      lst2.Items.Clear();
      stpRiga_Codice.Children.Add(lst2);

      stpScelte.Children.Add(stpRiga_Codice);

      StackPanel stpRiga_Descrizione = new StackPanel();
      stpRiga_Descrizione.Orientation = Orientation.Horizontal;
      lbl = new TextBlock();
      lbl.Text = "Descrizione";
      lbl.Width = 100;
      stpRiga_Descrizione.Children.Add(lbl);

      lst2 = new ComboBox();
      lst2.Name = "lst2_Descrizione";
      lst2.Width = 200;
      lst2.Margin = new Thickness(10, 0, 0, 0);
      this.RegisterName(lst2.Name, lst2);
      lst2.Items.Clear();
      stpRiga_Descrizione.Children.Add(lst2);

      stpScelte.Children.Add(stpRiga_Descrizione);

      StackPanel stpRiga_Saldo = new StackPanel();
      stpRiga_Saldo.Orientation = Orientation.Horizontal;
      lbl = new TextBlock();
      lbl.Text = "Saldo";
      lbl.Width = 100;
      stpRiga_Saldo.Children.Add(lbl);

      lst2 = new ComboBox();
      lst2.Name = "lst2_Saldo";
      lst2.Width = 200;
      lst2.Margin = new Thickness(10, 0, 0, 0);
      this.RegisterName(lst2.Name, lst2);
      lst2.SelectionChanged += Lst_Saldo_SelectionChanged;
      lst2.Items.Clear();
      stpRiga_Saldo.Children.Add(lst2);

      stpScelte.Children.Add(stpRiga_Saldo);

      StackPanel stpRiga_SaldoD = new StackPanel();
      stpRiga_SaldoD.Orientation = Orientation.Horizontal;
      lbl = new TextBlock();
      lbl.Text = "Saldo Dare";
      lbl.Width = 100;
      stpRiga_SaldoD.Children.Add(lbl);

      lst2 = new ComboBox();
      lst2.Name = "lst2_SaldoD";
      lst2.Width = 200;
      lst2.Margin = new Thickness(10, 0, 0, 0);
      this.RegisterName(lst2.Name, lst2);
      lst2.SelectionChanged += Lst_SaldoDA_SelectionChanged;
      lst2.Items.Clear();
      stpRiga_SaldoD.Children.Add(lst2);

      stpScelte.Children.Add(stpRiga_SaldoD);

      StackPanel stpRiga_SaldoA = new StackPanel();
      stpRiga_SaldoA.Orientation = Orientation.Horizontal;
      lbl = new TextBlock();
      lbl.Text = "Saldo Avere";
      lbl.Width = 100;
      stpRiga_SaldoA.Children.Add(lbl);

      lst2 = new ComboBox();
      lst2.Name = "lst2_SaldoA";
      lst2.Width = 200;
      lst2.Margin = new Thickness(10, 0, 0, 0);
      lst2.SelectionChanged += Lst_SaldoDA_SelectionChanged;
      this.RegisterName(lst2.Name, lst2);
      lst2.Items.Clear();
      stpRiga_SaldoA.Children.Add(lst2);

      stpScelte.Children.Add(stpRiga_SaldoA);

      lst.SelectedIndex = Convert.ToInt32(nodeBV.Attributes["rowintestazione"].Value);

      StackPanel stpBottoni = new StackPanel();
      stpBottoni.Orientation = Orientation.Horizontal;

      //btn = new Button();
      //btn.HorizontalAlignment = HorizontalAlignment.Left;
      //btn.Width = 100.0;
      //btn.Margin = new Thickness(10);
      //btn.Content = "Indietro";
      //btn.Click += Btn_Back_SceltaColonne_Click;

      //stpBottoni.Children.Add(btn);

      Button btn = new Button();
      btn.HorizontalAlignment = HorizontalAlignment.Right;
      btn.Width = 100.0;
      btn.Margin = new Thickness(20);
      btn.Padding = new Thickness(10);
      btn.Content = "Avanti";
      btn.Click += Btn_Next_SceltaColonne_Click;

      stpBottoni.Children.Add(btn);

      stpScelte.Children.Add(stpBottoni);

      //stpScelte.Visibility = Visibility.Collapsed;

      stackPanel1.Children.Add(stpScelte);

      #endregion

      #region bilancio verifica - Cestino
      StackPanel stpCestino = new StackPanel();
      stpCestino.Width = 1100.0;
      stpCestino.Height = 680.0;
      stpCestino.Orientation = Orientation.Vertical;
      stpCestino.Name = "stpCestino";
      stpCestino.Margin = new Thickness(10);
      this.RegisterName(stpCestino.Name, stpCestino);

      lbl = new TextBlock();
      lbl.Text = "Fase 3) Righe da non importare.";
      lbl.Margin = new Thickness(5, 5, 5, 0);
      lbl.FontWeight = FontWeights.Bold;
      lbl.HorizontalAlignment = HorizontalAlignment.Center;
      stpCestino.Children.Add(lbl);

      lbl = new TextBlock();
      lbl.MaxWidth = 1000.0;
      lbl.TextAlignment = TextAlignment.Left;
      lbl.TextWrapping = TextWrapping.Wrap;
      lbl.HorizontalAlignment = HorizontalAlignment.Left;
      lbl.Text = "Nell'apposita finestra di sinistra appariranno le righe del file XLS da importare.";
      lbl.Margin = new Thickness(5, 5, 5, 0);
      stpCestino.Children.Add(lbl);

      lbl = new TextBlock();
      lbl.Text = "Con un tick nell'apposita check box, anche con selezione multipla, dovranno essere selezionate le righe contenenti dati da non importare (totali, sub totali, e tutto ciò che non costituisce un conto) e trasferite nel cestino, agendo sull'apposito comando.";
      lbl.MaxWidth = 1000.0;
      lbl.TextAlignment = TextAlignment.Left;
      lbl.TextWrapping = TextWrapping.Wrap;
      lbl.HorizontalAlignment = HorizontalAlignment.Left;
      lbl.Margin = new Thickness(5, 5, 5, 0);
      stpCestino.Children.Add(lbl);

      lbl = new TextBlock();
      lbl.Text = "In caso di errore si potrà selezionare la riga dal cestino e reimmetterla nell'elenco originario.";
      lbl.MaxWidth = 1000.0;
      lbl.TextAlignment = TextAlignment.Left;
      lbl.TextWrapping = TextWrapping.Wrap;
      lbl.HorizontalAlignment = HorizontalAlignment.Left;
      lbl.Margin = new Thickness(5, 5, 5, 0);
      stpCestino.Children.Add(lbl);

      StackPanel stpBVCestino = new StackPanel();
      stpBVCestino.HorizontalAlignment = HorizontalAlignment.Center;
      stpBVCestino.Orientation = Orientation.Horizontal;

      StackPanel stp = new StackPanel();
      stp.Margin = new Thickness(0, 0, 10, 0);
      stp.Orientation = Orientation.Vertical;

      lbl = new TextBlock();
      lbl.Text = "Righe del file XlS - Bilancio di Verifica.";
      lbl.Margin = new Thickness(5, 5, 5, 0);
      lbl.FontWeight = FontWeights.Bold;
      stp.Children.Add(lbl);

      System.Windows.Controls.Border brd = new System.Windows.Controls.Border();
      brd.Margin = new Thickness(5);
      brd.BorderThickness = new Thickness(1);
      brd.BorderBrush = Brushes.Black;
      brd.Background = Brushes.White;

      ScrollViewer sw2 = new ScrollViewer();
      sw2.BorderBrush = Brushes.Black;
      sw2.BorderThickness = new Thickness(1);
      sw2.Name = "sw_ElencoBVCestino";
      this.RegisterName(sw2.Name, sw2);
      sw2.Width = 500;
      sw2.Height = 440;

      brd.Child = sw2;

      stp.Children.Add(brd);
      stpBVCestino.Children.Add(stp);

      stp = new StackPanel();
      stp.Orientation = Orientation.Vertical;
      stp.VerticalAlignment = VerticalAlignment.Center;

      Button btnpassaggio = new Button();
      Image img = new Image();
      btnpassaggio.Margin = new Thickness(5);
      var uriSource = new Uri(right, UriKind.Relative);
      btnpassaggio.ToolTip = "Porta Le voci Selezionate a SINISTRA nel CESTINO.";
      img.Source = new BitmapImage(uriSource);
      btnpassaggio.Content = img;
      btnpassaggio.Click += BtnpassaggioVersoCestino_Click;

      stp.Children.Add(btnpassaggio);

      btnpassaggio = new Button();
      btnpassaggio.Margin = new Thickness(5);
      img = new Image();
      uriSource = new Uri(left, UriKind.Relative);
      btnpassaggio.ToolTip = "Porta Le voci Selezionate a DESTRA fuori dal CESTINO.";
      img.Source = new BitmapImage(uriSource);
      btnpassaggio.Content = img;
      btnpassaggio.Click += BtnpassaggioDaCestino_Click;

      stp.Children.Add(btnpassaggio);

      stpBVCestino.Children.Add(stp);

      stp = new StackPanel();
      stp.Orientation = Orientation.Vertical;

      lbl = new TextBlock();
      lbl.Text = "Cestino - Righe non importate.";
      lbl.Margin = new Thickness(5);
      lbl.FontWeight = FontWeights.Bold;
      stp.Children.Add(lbl);

      brd = new System.Windows.Controls.Border();
      brd.Margin = new Thickness(5);
      brd.BorderThickness = new Thickness(1);
      brd.BorderBrush = Brushes.Black;
      brd.Background = Brushes.White;

      ScrollViewer sw4 = new ScrollViewer();
      sw4.Name = "sw_ElencoAssociazioniCestino";
      this.RegisterName(sw4.Name, sw4);
      sw4.Height = 440;
      sw4.Width = 500;

      brd.Child = sw4;

      stp.Children.Add(brd);
      stpBVCestino.Children.Add(stp);

      stpCestino.Children.Add(stpBVCestino);

      stp = new StackPanel();
      stp.Orientation = Orientation.Horizontal;

      if (Nomefile != "")
      {
        btn = new Button();
        btn.HorizontalAlignment = HorizontalAlignment.Left;
        btn.Width = 100.0;
        btn.Margin = new Thickness(20);
        btn.Padding = new Thickness(10);
        btn.Content = "Indietro";
        btn.Click += Btn_Back_Cestino_Click;
        stp.Children.Add(btn);
      }

      btn = new Button();
      btn.Content = "Avanti";
      btn.HorizontalAlignment = HorizontalAlignment.Right;
      btn.Click += Btn_Next_Cestino_Click;
      btn.Width = 100.0;
      btn.Margin = new Thickness(20);
      btn.Padding = new Thickness(10);
      stp.Children.Add(btn);

      stpCestino.Children.Add(stp);

      stpCestino.Visibility = Visibility.Collapsed;

      stackPanel1.Children.Add(stpCestino);
      #endregion

      #region bilancio verifica

      StackPanel stpBVCollector = new StackPanel();
      stpBVCollector.Orientation = Orientation.Vertical;
      stpBVCollector.Width = 1100.0;
      stpBVCollector.Height = 680.0;
      stpBVCollector.Name = "stpBVCollector";
      this.RegisterName(stpBVCollector.Name, stpBVCollector);

      lbl = new TextBlock();
      lbl.Text = "Fase 4) Associazione Conti a Voci di Bilancio.";
      lbl.Margin = new Thickness(5, 5, 5, 0);
      lbl.FontWeight = FontWeights.Bold;
      lbl.HorizontalAlignment = HorizontalAlignment.Center;
      stpBVCollector.Children.Add(lbl);

      lbl = new TextBlock();
      lbl.MaxWidth = 1100.0;
      lbl.TextAlignment = TextAlignment.Left;
      lbl.TextWrapping = TextWrapping.Wrap;
      lbl.HorizontalAlignment = HorizontalAlignment.Left;
      lbl.Text = "Nell’apposita finestra di sinistra appariranno i conti del bilancio di verifica (dal file XLS) da collegare alle voci di bilancio elencate nella finestra centrale; con un tick nell’apposita check box, anche con selezione multipla, dovranno essere selezionati i conti da correlare alla voce di bilancio, che si selezionerà con un altro tick nella sua check box. L’apposito comando trasferirà i conti alla voce di bilancio scelta, eliminandoli dall’elenco di sinistra e collocandoli nella finestra di destra, che evidenzierà i conti contabili correlati ad ogni voce.";
      lbl.Margin = new Thickness(5, 5, 5, 0);
      stpBVCollector.Children.Add(lbl);

      lbl = new TextBlock();
      lbl.MaxWidth = 1100.0;
      lbl.TextAlignment = TextAlignment.Left;
      lbl.TextWrapping = TextWrapping.Wrap;
      lbl.HorizontalAlignment = HorizontalAlignment.Left;
      lbl.Text = "Le voci di bilancio sono suddivise in QUATTRO GRUPPI: Attivo – Passivo - Attivo / Passivo - Conto economico. Nel Gruppo Attivo/Passivo sono elencate le voci di bilancio che normalmente esprimono saldi attivi e passivi: banche, crediti tributari, previdenziali, ecc. A queste voci dovranno essere collegati tutti i conti contabili a loro riconducibili: sarà il software ad esporre i saldi fra le attività o passività dello Stato Patrimoniale, secondo il segno Dare o Avere.";
      lbl.Margin = new Thickness(5, 5, 5, 0);
      stpBVCollector.Children.Add(lbl);

      StackPanel stpBV = new StackPanel();
      stpBV.Orientation = Orientation.Horizontal;
      stpBV.Margin = new Thickness(10);

      stp = new StackPanel();
      stp.Margin = new Thickness(0, 0, 10, 0);
      stp.Orientation = Orientation.Vertical;

      lbl = new TextBlock();
      lbl.Text = "Conti del Bilancio di Verifica da associare";
      lbl.Margin = new Thickness(5);
      lbl.FontWeight = FontWeights.Bold;
      stp.Children.Add(lbl);

      brd = new System.Windows.Controls.Border();
      brd.Margin = new Thickness(5);
      brd.BorderThickness = new Thickness(1);
      brd.BorderBrush = Brushes.Black;
      brd.Background = Brushes.White;

      sw2 = new ScrollViewer();
      sw2.BorderBrush = Brushes.Black;
      sw2.BorderThickness = new Thickness(1);
      sw2.Name = "sw_ElencoBV";
      this.RegisterName(sw2.Name, sw2);
      sw2.Height = 440;
      sw2.Width = 300;

      brd.Child = sw2;

      stp.Children.Add(brd);
      stpBV.Children.Add(stp);

      stp = new StackPanel();
      stp.Margin = new Thickness(0, 0, 10, 0);
      stp.Orientation = Orientation.Vertical;
      stp.VerticalAlignment = VerticalAlignment.Center;

      btnpassaggio = new Button();
      img = new Image();
      uriSource = new Uri(right, UriKind.Relative);
      btnpassaggio.ToolTip = "Associa Le voci Selezionate a SINISTRA nella voce di Bilancio selezionata a DESTRA.";
      img.Source = new BitmapImage(uriSource);
      btnpassaggio.Content = img;
      btnpassaggio.Click += BtnpassaggioVersoBilancio_Click;

      stp.Children.Add(btnpassaggio);

      lbl = new TextBlock();
      lbl.Text = "ASSOCIA";
      lbl.FontWeight = FontWeights.Bold;
      lbl.Margin = new Thickness(5);

      stp.Children.Add(lbl);

      stpBV.Children.Add(stp);

      stp = new StackPanel();
      stp.Margin = new Thickness(0, 0, 10, 0);
      stp.Orientation = Orientation.Vertical;

      lbl = new TextBlock();
      lbl.Text = "Voci di Bilancio";
      lbl.FontWeight = FontWeights.Bold;
      lbl.Margin = new Thickness(5);
      stp.Children.Add(lbl);

      TabControl tabBilanci = new TabControl();
      tabBilanci.Name = "tabBilanci";
      this.RegisterName(tabBilanci.Name, tabBilanci);
      tabBilanci.Visibility = Visibility.Visible;

      #region dare / avere
      TabItem ti = new TabItem();
      ti.Header = "Attivo / Passivo";

      StackPanel tw = new StackPanel();
      tw.Orientation = Orientation.Vertical;

      foreach (DictionaryEntry item in valoridareavere)
      {
        StackPanel twriga = new StackPanel();
        twriga.Orientation = Orientation.Horizontal;

        CheckBox chk = new CheckBox();
        chk.Name = "chk_da_" + item.Value.ToString().Replace("|", "_");
        chk.Checked += ChkBilancio_Checked;
        chk.Unchecked += ChkBilancio_Unchecked;
        twriga.Children.Add(chk);

        lbl = new TextBlock();
        lbl.ToolTip = item.Key.ToString();
        lbl.Text = item.Key.ToString();
        twriga.Children.Add(lbl);

        tw.Children.Add(twriga);
      }

      ti.Content = tw;

      tabBilanci.Items.Add(ti);
      #endregion

      XmlDataProviderManager _y = null;

      #region Patrimoniale Attivo
      _y = new XmlDataProviderManager(templateA, true);

      ti = new TabItem();
      ti.Header = "Attivo";

      ScrollViewer swtw = new ScrollViewer();
      swtw.BorderBrush = Brushes.Black;
      swtw.BorderThickness = new Thickness(1);

      tw = new StackPanel();
      tw.Orientation = Orientation.Vertical;

      foreach (XmlNode item in _y.Document.SelectNodes("/Dato/MacroGruppo/Bilancio"))
      {
        if (item.Attributes["ID"] == null || item.Attributes["somma"] != null)
        {
          continue;
        }

        string valuetitolo = ((item.Attributes["Codice"] == null) ? "" : item.Attributes["Codice"].Value + " - ") + ((item.Attributes["name"] == null) ? "" : item.Attributes["name"].Value);

        if (valuetitolo == "")
        {
          continue;
        }

        PatAttivo.Add(item.Attributes["ID"].Value);

        valorilabel.Add(item.Attributes["ID"].Value, valuetitolo);

        bool valoridareaverefound = false;

        foreach (DictionaryEntry vda in valoridareavere)
        {
          foreach (string vda_id in vda.Value.ToString().Split('|'))
          {
            if (item.Attributes["ID"].Value == vda_id)
            {
              valoridareaverefound = true;
              break;
            }
          }

          if (valoridareaverefound == true)
          {
            break;
          }
        }

        if (valoridareaverefound == true)
        {
          continue;
        }

        bool valorisommafound = false;

        foreach (DictionaryEntry vs in valorisomma)
        {
          foreach (string vss in vs.Value.ToString().Split('|'))
          {
            if (vss == item.Attributes["ID"].Value)
            {
              valorisommafound = true;
              break;
            }
          }

          if (valorisommafound)
          {
            break;
          }
        }

        if (valorisommafound)
        {
          continue;
        }

        StackPanel twriga = new StackPanel();
        twriga.Orientation = Orientation.Horizontal;

        int paddingadditive = 0;

        if (item.Attributes["noData"] == null || item.Attributes["noData"].Value != "1" || valorisomma.Contains(item.Attributes["ID"].Value))
        {
          CheckBox chk = new CheckBox();
          chk.Name = "chk_" + item.Attributes["ID"].Value;
          chk.Checked += ChkBilancio_Checked;
          chk.Unchecked += ChkBilancio_Unchecked;
          twriga.Children.Add(chk);
        }
        else
        {
          paddingadditive = 20;
        }

        lbl = new TextBlock();
        if (item.Attributes["paddingCodice"] != null)
        {
          int padding = Convert.ToInt32(item.Attributes["paddingCodice"].Value) / 3 + paddingadditive;
          lbl.Margin = new Thickness(padding, 0, 0, 0);
        }
        else if (item.Attributes["Codice"] == null)
        {
          lbl.Margin = new Thickness(20 + paddingadditive, 0, 0, 0);
        }
        else
        {
          lbl.Margin = new Thickness(paddingadditive, 0, 0, 0);
        }

        lbl.Text = valuetitolo;
        twriga.Children.Add(lbl);

        tw.Children.Add(twriga);
      }

      swtw.Content = tw;

      swtw.Height = 410;
      swtw.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
      swtw.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

      ti.Content = swtw;
      tabBilanci.Items.Add(ti);
      #endregion

      #region Patrimoniale Passivo
      _y = new XmlDataProviderManager(templateB, true);

      ti = new TabItem();
      ti.Header = "Passivo";

      swtw = new ScrollViewer();
      swtw.BorderBrush = Brushes.Black;
      swtw.BorderThickness = new Thickness(1);

      tw = new StackPanel();
      tw.Orientation = Orientation.Vertical;

      foreach (XmlNode item in _y.Document.SelectNodes("/Dato/MacroGruppo/Bilancio"))
      {
        if (item.Attributes["ID"] == null || item.Attributes["somma"] != null)
        {
          continue;
        }

        string valuetitolo = ((item.Attributes["Codice"] == null) ? "" : item.Attributes["Codice"].Value + " - ") + ((item.Attributes["name"] == null) ? "" : item.Attributes["name"].Value);

        if (valuetitolo == "")
        {
          continue;
        }

        PatPassivo.Add(item.Attributes["ID"].Value);

        valorilabel.Add(item.Attributes["ID"].Value, valuetitolo);

        bool valoridareaverefound = false;

        foreach (DictionaryEntry vda in valoridareavere)
        {
          foreach (string vda_id in vda.Value.ToString().Split('|'))
          {
            if (item.Attributes["ID"].Value == vda_id)
            {
              valoridareaverefound = true;
              break;
            }
          }

          if (valoridareaverefound == true)
          {
            break;
          }
        }

        if (valoridareaverefound == true)
        {
          continue;
        }

        bool valorisommafound = false;

        foreach (DictionaryEntry vs in valorisomma)
        {
          foreach (string vss in vs.Value.ToString().Split('|'))
          {
            if (vss == item.Attributes["ID"].Value)
            {
              valorisommafound = true;
              break;
            }
          }

          if (valorisommafound)
          {
            break;
          }
        }

        if (valorisommafound)
        {
          continue;
        }

        StackPanel twriga = new StackPanel();
        twriga.Orientation = Orientation.Horizontal;

        int paddingadditive = 0;

        if (item.Attributes["noData"] == null || item.Attributes["noData"].Value != "1" || valorisomma.Contains(item.Attributes["ID"].Value))
        {
          CheckBox chk = new CheckBox();
          chk.Name = "chk_" + item.Attributes["ID"].Value;

          if (PerditaUtileID != item.Attributes["ID"].Value)
          {
            chk.Checked += ChkBilancio_Checked;
            chk.Unchecked += ChkBilancio_Unchecked;
          }
          else
          {
            chk.IsEnabled = false;
          }

          twriga.Children.Add(chk);
        }
        else
        {
          paddingadditive = 20;
        }

        lbl = new TextBlock();
        if (item.Attributes["paddingCodice"] != null)
        {
          int padding = Convert.ToInt32(item.Attributes["paddingCodice"].Value) / 3 + paddingadditive;
          lbl.Margin = new Thickness(padding, 0, 0, 0);
        }
        else if (item.Attributes["Codice"] == null)
        {
          lbl.Margin = new Thickness(20 + paddingadditive, 0, 0, 0);
        }
        else
        {
          lbl.Margin = new Thickness(paddingadditive, 0, 0, 0);
        }
        lbl.Text = valuetitolo;
        twriga.Children.Add(lbl);

        tw.Children.Add(twriga);
      }

      swtw.Content = tw;

      swtw.Height = 390;
      swtw.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
      swtw.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

      ti.Content = swtw;
      tabBilanci.Items.Add(ti);
      #endregion

      #region Conto Economico
      _y = new XmlDataProviderManager(templateC, true);

      ti = new TabItem();
      ti.Header = "Conto Economico";

      swtw = new ScrollViewer();
      swtw.BorderBrush = Brushes.Black;
      swtw.BorderThickness = new Thickness(1);

      tw = new StackPanel();
      tw.Orientation = Orientation.Vertical;

      foreach (XmlNode item in _y.Document.SelectNodes("/Dato/MacroGruppo/Bilancio"))
      {
        if (item.Attributes["ID"] == null || item.Attributes["somma"] != null)
        {
          continue;
        }

        string valuetitolo = ((item.Attributes["Codice"] == null) ? "" : item.Attributes["Codice"].Value + " - ") + ((item.Attributes["name"] == null) ? "" : item.Attributes["name"].Value);

        if (valuetitolo == "")
        {
          continue;
        }

        ContoEconomico.Add(item.Attributes["ID"].Value);

        valorilabel.Add(item.Attributes["ID"].Value, valuetitolo);

        bool valoridareaverefound = false;

        foreach (DictionaryEntry vda in valoridareavere)
        {
          foreach (string vda_id in vda.Value.ToString().Split('|'))
          {
            if (item.Attributes["ID"].Value == vda_id)
            {
              valoridareaverefound = true;
              break;
            }
          }

          if (valoridareaverefound == true)
          {
            break;
          }
        }

        if (valoridareaverefound == true)
        {
          continue;
        }

        bool valorisommafound = false;

        foreach (DictionaryEntry vs in valorisomma)
        {
          foreach (string vss in vs.Value.ToString().Split('|'))
          {
            if (vss == item.Attributes["ID"].Value)
            {
              valorisommafound = true;
              break;
            }
          }

          if (valorisommafound)
          {
            break;
          }
        }

        if (valorisommafound)
        {
          continue;
        }

        StackPanel twriga = new StackPanel();
        twriga.Orientation = Orientation.Horizontal;

        int paddingadditive = 0;

        if (item.Attributes["noData"] == null || item.Attributes["noData"].Value != "1" || valorisomma.Contains(item.Attributes["ID"].Value))
        {
          CheckBox chk = new CheckBox();
          chk.Name = "chk_" + item.Attributes["ID"].Value;
          chk.Checked += ChkBilancio_Checked;
          chk.Unchecked += ChkBilancio_Unchecked;
          twriga.Children.Add(chk);
        }
        else
        {
          paddingadditive = 20;
        }

        lbl = new TextBlock();
        if (item.Attributes["paddingCodice"] != null)
        {
          int padding = Convert.ToInt32(item.Attributes["paddingCodice"].Value) / 3 + paddingadditive;
          lbl.Margin = new Thickness(padding, 0, 0, 0);
        }
        else if (item.Attributes["Codice"] == null)
        {
          lbl.Margin = new Thickness(20 + paddingadditive, 0, 0, 0);
        }
        else
        {
          lbl.Margin = new Thickness(paddingadditive, 0, 0, 0);
        }
        lbl.Text = valuetitolo;
        twriga.Children.Add(lbl);

        tw.Children.Add(twriga);
      }

      swtw.Content = tw;

      swtw.Height = 410;
      swtw.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
      swtw.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

      ti.Content = swtw;
      tabBilanci.Items.Add(ti);
      #endregion

      tabBilanci.Height = 440;
      tabBilanci.Width = 300;

      stp.Children.Add(tabBilanci);
      stpBV.Children.Add(stp);

      stp = new StackPanel();
      stp.Margin = new Thickness(0, 0, 10, 0);
      stp.Orientation = Orientation.Vertical;
      stp.VerticalAlignment = VerticalAlignment.Center;

      btnpassaggio = new Button();
      img = new Image();
      uriSource = new Uri(left, UriKind.Relative);
      btnpassaggio.ToolTip = "Porta Le voci Selezionate a DESTRA nelle voci ancora da associare.";
      img.Source = new BitmapImage(uriSource);
      btnpassaggio.Content = img;
      btnpassaggio.Click += BtnpassaggioDaBilancio_Click;

      stp.Children.Add(btnpassaggio);

      lbl = new TextBlock();
      lbl.Text = "DISSOCIA";
      lbl.FontWeight = FontWeights.Bold;
      lbl.Margin = new Thickness(5);

      stp.Children.Add(lbl);

      stpBV.Children.Add(stp);

      stp = new StackPanel();
      stp.Orientation = Orientation.Vertical;

      lbl = new TextBlock();
      lbl.Text = "Conti associati alla voce selezionata";
      lbl.Margin = new Thickness(5);
      lbl.FontWeight = FontWeights.Bold;
      stp.Children.Add(lbl);

      brd = new System.Windows.Controls.Border();
      brd.Margin = new Thickness(5);
      brd.BorderThickness = new Thickness(1);
      brd.BorderBrush = Brushes.Black;
      brd.Background = Brushes.White;

      sw4 = new ScrollViewer();
      sw4.BorderBrush = Brushes.Black;
      sw4.BorderThickness = new Thickness(1);
      sw4.Name = "sw_ElencoAssociazioni";
      this.RegisterName(sw4.Name, sw4);
      sw4.Width = 290;
      sw4.Height = 440;

      brd.Child = sw4;

      stp.Children.Add(brd);
      stpBV.Children.Add(stp);

      stpBVCollector.Children.Add(stpBV);

      stp = new StackPanel();
      stp.Orientation = Orientation.Horizontal;

      btn = new Button();
      btn.HorizontalAlignment = HorizontalAlignment.Left;
      btn.Width = 100.0;
      btn.Margin = new Thickness(20, 0, 20, 0);
      btn.Padding = new Thickness(10);
      btn.Content = "Indietro";
      btn.Click += Btn_Back_BV_Click;
      stp.Children.Add(btn);

      if (Nomefile != "")
      {
        btn = new Button();
        btn.Content = "Avanti";
        btn.HorizontalAlignment = HorizontalAlignment.Right;
        btn.Click += Btn_Next_BV_Click;
        btn.Width = 100.0;
        btn.Margin = new Thickness(20, 0, 20, 0);
        btn.Padding = new Thickness(10);
        stp.Children.Add(btn);

        lbl = new TextBlock();
        lbl.Text = "In caso di uscita, le associazioni fino ad ora eseguite verranno salvate";
        lbl.FontWeight = FontWeights.Bold;
        lbl.Margin = new Thickness(5);

        stp.Children.Add(lbl);

      }
      else
      {
        btn = new Button();
        btn.Content = "Esci";
        btn.HorizontalAlignment = HorizontalAlignment.Right;
        btn.Click += Btn_Click;
        btn.Width = 100.0;
        btn.Margin = new Thickness(20, 0, 20, 0);
        btn.Padding = new Thickness(10);
        stp.Children.Add(btn);

        TextBlock txtb = new TextBlock();
        txtb.Text = "Dopo aver modificato l'associazione dei conti, occorre importare nuovamente il file xls per allocare i dati alle nuove voci.";
        txtb.Margin = new Thickness(20, 0, 0, 0);
        stp.Children.Add(txtb);
      }

      stpBVCollector.Children.Add(stp);

      stpBVCollector.Visibility = Visibility.Collapsed;

      stackPanel1.Children.Add(stpBVCollector);
      #endregion

      #region bilancio verifica ultimi controlli
      StackPanel stpLastControls = new StackPanel();
      stpLastControls.Width = 1100.0;
      stpLastControls.Height = 680.0;
      stpLastControls.Orientation = Orientation.Vertical;
      stpLastControls.Margin = new Thickness(10);
      stpLastControls.Name = "stpLastControls";
      this.RegisterName(stpLastControls.Name, stpLastControls);

      stp = new StackPanel();
      stp.Margin = new Thickness(0, 0, 10, 0);
      stp.Orientation = Orientation.Vertical;

      lbl = new TextBlock();
      lbl.Text = "Fase 5)";
      lbl.Margin = new Thickness(5);
      lbl.FontWeight = FontWeights.Bold;
      stp.Children.Add(lbl);


      lbl = new TextBlock();
      lbl.MaxWidth = 1000.0;
      lbl.TextAlignment = TextAlignment.Left;
      lbl.TextWrapping = TextWrapping.Wrap;
      lbl.HorizontalAlignment = HorizontalAlignment.Left;
      lbl.Text = "Il software elencherà le voci di bilancio che espongono crediti o debiti entro ed oltre l’esercizio successivo. Il valore viene esposto come ENTRO L’ESERCIZIO; indicando nell’apposito spazio la QUOTA OLTRE il software regolerà il valore ENTRO ed i dati verranno caricati sul bilancio.";
      lbl.Margin = new Thickness(5, 5, 5, 0);
      stp.Children.Add(lbl);


      sw2 = new ScrollViewer();
      sw2.BorderBrush = Brushes.Black;
      sw2.BorderThickness = new Thickness(1);
      sw2.Name = "sw_ElencoLastControls";
      this.RegisterName(sw2.Name, sw2);
      sw2.Height = 480;
      sw2.Width = 1000;

      stp.Children.Add(sw2);
      stpLastControls.Children.Add(stp);

      stp = new StackPanel();
      stp.Orientation = Orientation.Horizontal;

      btn = new Button();
      btn.Margin = new Thickness(5);
      btn.Content = "Modifica Impostazioni Precedenti";
      btn.Width = 200.0;
      btn.Margin = new Thickness(20);
      btn.Padding = new Thickness(10);
      btn.VerticalAlignment = VerticalAlignment.Center;
      btn.Click += Btn_Back_LastCheck_Click;
      stp.Children.Add(btn);

      Button btnFinalizza = new Button();
      btn.Margin = new Thickness(5);
      btnFinalizza.Content = "Conferma Importazione";
      btnFinalizza.Width = 200.0;
      btnFinalizza.Margin = new Thickness(20);
      btnFinalizza.Padding = new Thickness(10);
      btnFinalizza.VerticalAlignment = VerticalAlignment.Center;
      btnFinalizza.Click += BtnFinalizza_Click;
      stp.Children.Add(btnFinalizza);

      stpLastControls.Children.Add(stp);

      stpLastControls.Visibility = Visibility.Collapsed;

      stackPanel1.Children.Add(stpLastControls);
      #endregion
    }

    private void Btn_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    #region Bottoni Next / Prev
    private void Btn_Next_SceltaRigaIntestazione_Click(object sender, RoutedEventArgs e)
    {
      StackPanel stpIntestazione = (StackPanel)this.FindName("stpIntestazione");
      stpIntestazione.Visibility = Visibility.Collapsed;

      StackPanel stpScelte = (StackPanel)this.FindName("stpScelte");
      stpScelte.Visibility = Visibility.Visible;
    }

    private void Btn_Back_SceltaColonne_Click(object sender, RoutedEventArgs e)
    {
      StackPanel stpIntestazione = (StackPanel)this.FindName("stpIntestazione");
      stpIntestazione.Visibility = Visibility.Visible;

      StackPanel stpScelte = (StackPanel)this.FindName("stpScelte");
      stpScelte.Visibility = Visibility.Collapsed;
    }

    private void Btn_Next_SceltaColonne_Click(object sender, RoutedEventArgs e)
    {
      StackPanel stpIntestazione = (StackPanel)this.FindName("stpIntestazione");
      stpIntestazione.Visibility = Visibility.Collapsed;

      StackPanel stpScelte = (StackPanel)this.FindName("stpScelte");
      stpScelte.Visibility = Visibility.Collapsed;

      StackPanel stpBV = (StackPanel)this.FindName("stpCestino");
      stpBV.Visibility = Visibility.Visible;

      ComboBox lst2_Codice = (ComboBox)this.FindName("lst2_Codice");
      if (lst2_Codice.SelectedIndex == 0)
      {
        MessageBox.Show("Selezionare Codice");
        return;
      }

      ComboBox lst2_Descrizione = (ComboBox)this.FindName("lst2_Descrizione");
      if (lst2_Descrizione.SelectedIndex == 0)
      {
        MessageBox.Show("Selezionare Descrizione");
        return;
      }

      ComboBox lst2_Saldo = (ComboBox)this.FindName("lst2_Saldo");
      ComboBox lst2_SaldoD = (ComboBox)this.FindName("lst2_SaldoD");
      ComboBox lst2_SaldoA = (ComboBox)this.FindName("lst2_SaldoA");
      if (lst2_Saldo.SelectedIndex == 0)
      {
        if (lst2_SaldoD.SelectedIndex == 0 || lst2_SaldoA.SelectedIndex == 0)
        {
          MessageBox.Show("Selezionare Colonna Saldo, oppure Saldo Dare e Saldo Avere");
          return;
        }
      }
      else
      {
        if (lst2_SaldoD.SelectedIndex != 0 || lst2_SaldoA.SelectedIndex != 0)
        {
          MessageBox.Show("Selezionare Colonna Saldo, oppure Saldo Dare e Saldo Avere. Non possono essere contemporanee.");
          return;
        }
      }

      nodeBV.Attributes["codice"].Value = ((ComboboxItem)(lst2_Codice.SelectedItem)).Value;
      nodeBV.Attributes["descrizione"].Value = ((ComboboxItem)(lst2_Descrizione.SelectedItem)).Value;
      nodeBV.Attributes["saldo"].Value = ((ComboboxItem)(lst2_Saldo.SelectedItem)).Value;
      nodeBV.Attributes["saldod"].Value = ((ComboboxItem)(lst2_SaldoD.SelectedItem)).Value;
      nodeBV.Attributes["saldoa"].Value = ((ComboboxItem)(lst2_SaldoA.SelectedItem)).Value;

      mf.SetAnagraficaBV(Convert.ToInt32(IDCLiente), nodeBV);

      GetDataFromExcel(true);

      VisualizzaListaDaAssociare_Cestino();
      VisualizzaListaAssociate_Cestino();
    }

    private void Btn_Back_Cestino_Click(object sender, RoutedEventArgs e)
    {

      StackPanel stpIntestazione = (StackPanel)this.FindName("stpIntestazione");
      stpIntestazione.Visibility = Visibility.Visible;

      StackPanel stpScelte = (StackPanel)this.FindName("stpScelte");
      stpScelte.Visibility = Visibility.Visible;

      StackPanel stpBV = (StackPanel)this.FindName("stpCestino");
      stpBV.Visibility = Visibility.Collapsed;
    }

    private void Btn_Next_Cestino_Click(object sender, RoutedEventArgs e)
    {
      StackPanel stpIntestazione = (StackPanel)this.FindName("stpCestino");
      stpIntestazione.Visibility = Visibility.Collapsed;

      StackPanel stpScelte = (StackPanel)this.FindName("stpBVCollector");
      stpScelte.Visibility = Visibility.Visible;

      VisualizzaListaDaAssociare();
      VisualizzaListaAssociate("");
    }

    private void Btn_Back_BV_Click(object sender, RoutedEventArgs e)
    {
      StackPanel stpIntestazione = (StackPanel)this.FindName("stpBVCollector");
      stpIntestazione.Visibility = Visibility.Collapsed;

      StackPanel stpScelte = (StackPanel)this.FindName("stpCestino");
      stpScelte.Visibility = Visibility.Visible;

      VisualizzaListaDaAssociare_Cestino();
      VisualizzaListaAssociate_Cestino();
    }

    private void Btn_Next_BV_Click(object sender, RoutedEventArgs e)
    {
      StackPanel stpElencoBV = (StackPanel)this.FindName("stpElencoBV");

      if (stpElencoBV.Children.Count > 0)
      {
        MessageBox.Show("Esistono ancora dei conti da attribuire prima di finalizzare la procedura.");
        return;
      }

      GetDataFromExcel(true);

      VisualizzaListaFinal();

      StackPanel stpBV = (StackPanel)this.FindName("stpBVCollector");
      stpBV.Visibility = Visibility.Collapsed;

      StackPanel stpLastControls = (StackPanel)this.FindName("stpLastControls");
      stpLastControls.Visibility = Visibility.Visible;
    }

    private void Btn_Back_LastCheck_Click(object sender, RoutedEventArgs e)
    {
      StackPanel stpIntestazione = (StackPanel)this.FindName("stpLastControls");
      stpIntestazione.Visibility = Visibility.Collapsed;

      StackPanel stpScelte = (StackPanel)this.FindName("stpBVCollector");
      stpScelte.Visibility = Visibility.Visible;

      VisualizzaListaDaAssociare();
      VisualizzaListaAssociate("");
    }
    #endregion

    #region Riga Intestazione

    private void Lst_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      rowintestazione = Convert.ToInt32(((string)(((ComboBox)sender).SelectedItem)));
      nodeBV.Attributes["rowintestazione"].Value = ((ComboBox)sender).SelectedIndex.ToString();

      StackPanel stphere = (StackPanel)this.FindName("Riga2");
      if (stphere == null)
      {
        return;
      }

      stphere.Children.Clear();
      stphere.CanHorizontallyScroll = true;

      ComboboxItem item = new ComboboxItem();
      item.Text = "(Selezionare)";
      item.Value = "0";

      ComboBox lst2_Codice = (ComboBox)this.FindName("lst2_Codice");
      lst2_Codice.Items.Clear();
      lst2_Codice.Items.Add(item);

      ComboBox lst2_Descrizione = (ComboBox)this.FindName("lst2_Descrizione");
      lst2_Descrizione.Items.Clear();
      lst2_Descrizione.Items.Add(item);

      ComboBox lst2_Saldo = (ComboBox)this.FindName("lst2_Saldo");
      lst2_Saldo.Items.Clear();
      lst2_Saldo.Items.Add(item);

      ComboBox lst2_SaldoD = (ComboBox)this.FindName("lst2_SaldoD");
      lst2_SaldoD.Items.Clear();
      lst2_SaldoD.Items.Add(item);

      ComboBox lst2_SaldoA = (ComboBox)this.FindName("lst2_SaldoA");
      lst2_SaldoA.Items.Clear();
      lst2_SaldoA.Items.Add(item);

      TextBlock txthere = new TextBlock();
      txthere.Text = "Riga XLS selezionata > ";
      txthere.FontWeight = FontWeights.Bold;
      txthere.VerticalAlignment = VerticalAlignment.Center;
      stphere.Children.Add(txthere);

      for (int i = 1; i <= 100; i++)
      {
        string valuehere = "";

        try
        {

          ExcelRange objRange = excelSheet.Cells[rowintestazione, i];

          if (objRange.Merge)
          {
            valuehere = Convert.ToString((excelSheet.Cells[1, 1]).Text).Trim();
          }
          else
          {
            valuehere = Convert.ToString(objRange.Text).Trim();
          }



          //valuehere = excelSheet.Cells[rowintestazione, i].Value2.ToString();

          if (valuehere.Trim() == "")
          {
            continue;
          }
        }
        catch (Exception ex)
        {
          string loghere = ex.Message;
          continue;
        }

        StackPanel stp = new StackPanel();
        stp.Orientation = Orientation.Vertical;

        TextBlock txt = new TextBlock();

        txt.Text = ColumnIndexToColumnLetter(i);

        item = new ComboboxItem();
        item.Text = txt.Text + " - " + valuehere;
        item.Value = txt.Text;

        lst2_Codice.Items.Add(item);
        lst2_Descrizione.Items.Add(item);
        lst2_Saldo.Items.Add(item);
        lst2_SaldoD.Items.Add(item);
        lst2_SaldoA.Items.Add(item);

        txt.TextAlignment = TextAlignment.Center;
        txt.FontWeight = FontWeights.Bold;
        txt.Width = 100.0;
        txt.Margin = new Thickness(5, 5, 0, 0);
        stp.Children.Add(txt);

        txt = new TextBlock();
        txt.Text = valuehere;
        txt.TextAlignment = TextAlignment.Center;
        txt.Width = 100.0;
        txt.Margin = new Thickness(5, 0, 0, 5);
        stp.Children.Add(txt);

        stphere.Children.Add(stp);
      }

      int indexselected = 0;
      int index = 0;
      foreach (ComboboxItem cmb_item in lst2_Codice.Items)
      {
        if (nodeBV.Attributes["codice"].Value == cmb_item.Value)
        {
          indexselected = index;
          break;
        }

        index++;
      }

      lst2_Codice.SelectedIndex = indexselected;

      indexselected = 0;
      index = 0;
      foreach (ComboboxItem cmb_item in lst2_Descrizione.Items)
      {
        if (nodeBV.Attributes["descrizione"].Value == cmb_item.Value)
        {
          indexselected = index;
          break;
        }

        index++;
      }

      lst2_Descrizione.SelectedIndex = indexselected;

      indexselected = 0;
      index = 0;
      foreach (ComboboxItem cmb_item in lst2_Saldo.Items)
      {
        if (nodeBV.Attributes["saldo"].Value == cmb_item.Value)
        {
          indexselected = index;
          break;
        }

        index++;
      }

      lst2_Saldo.SelectedIndex = indexselected;

      indexselected = 0;
      index = 0;
      foreach (ComboboxItem cmb_item in lst2_SaldoD.Items)
      {
        if (nodeBV.Attributes["saldod"].Value == cmb_item.Value)
        {
          indexselected = index;
          break;
        }

        index++;
      }

      lst2_SaldoD.SelectedIndex = indexselected;

      indexselected = 0;
      index = 0;
      foreach (ComboboxItem cmb_item in lst2_SaldoA.Items)
      {
        if (nodeBV.Attributes["saldoa"].Value == cmb_item.Value)
        {
          indexselected = index;
          break;
        }

        index++;
      }

      lst2_SaldoA.SelectedIndex = indexselected;
    }

    #endregion

    #region Scelta Colonne

    private void Lst_Saldo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ComboBox lst2_Saldo = (ComboBox)this.FindName("lst2_Saldo");
      ComboBox lst2_SaldoD = (ComboBox)this.FindName("lst2_SaldoD");
      ComboBox lst2_SaldoA = (ComboBox)this.FindName("lst2_SaldoA");

      if (lst2_Saldo.SelectedIndex == 0 || lst2_Saldo.SelectedIndex == -1)
      {
        lst2_SaldoD.IsEnabled = true;
        lst2_SaldoA.IsEnabled = true;
      }
      else
      {
        lst2_SaldoD.IsEnabled = false;
        lst2_SaldoA.IsEnabled = false;
      }
    }

    private void Lst_SaldoDA_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ComboBox lst2_Saldo = (ComboBox)this.FindName("lst2_Saldo");
      ComboBox lst2_SaldoD = (ComboBox)this.FindName("lst2_SaldoD");
      ComboBox lst2_SaldoA = (ComboBox)this.FindName("lst2_SaldoA");

      if ((lst2_SaldoD.SelectedIndex == 0 || lst2_SaldoD.SelectedIndex == -1) && (lst2_SaldoA.SelectedIndex == 0 || lst2_SaldoA.SelectedIndex == -1))
      {
        lst2_Saldo.IsEnabled = true;
      }
      else
      {
        lst2_Saldo.IsEnabled = false;
      }
    }

    private void GetDataSenzaExcel(bool erase)
    {
      esistealmenounavocenonassociata = false;
      esistealmenounavoce = false;

      int firstrow = rowintestazione + 1;
      erase = false;
      if (erase)
      {
        RawData = null;
      }

      if (RawData == null)
      {
        RawData = new DataTable();

        RawData.Columns.Add("Codice");
        RawData.Columns.Add("Descrizione");
        RawData.Columns.Add("Saldo");
        RawData.Columns.Add("ID");

        foreach (XmlNode nodeBVhere in nodeBV.SelectNodes("ASSOCIAZIONE[@tipobilancio=\"" + tipoBilancio + "\"][@codice][@ID]"))
        {
          esistealmenounavocenonassociata = true;

          string value_codice = nodeBVhere.Attributes["codice"].Value;
          string idassociato = nodeBVhere.Attributes["ID"].Value;
          string value_descrizione = ((nodeBVhere.Attributes["titolo"] != null) ? nodeBVhere.Attributes["titolo"].Value : "");

          bool rowalreadyexists = false;

          for (int w = 0; w < RawData.Rows.Count; w++)
          {
            if (RawData.Rows[w][0].ToString() == value_codice)
            {
              rowalreadyexists = true;
              break;
            }
          }

          if (rowalreadyexists == false)
          {
            List<string> tmparray = new List<string>();
            tmparray.Add(value_codice);
            tmparray.Add(value_descrizione);
            tmparray.Add("0");
            tmparray.Add(idassociato);

            RawData.Rows.Add(tmparray.ToArray());
            esistealmenounavoce = true;
          }
        }
      }
    }

    private void GetDataFromExcel(bool erase)
    {
      esistealmenounavocenonassociata = false;
      esistealmenounavoce = false;

      int firstrow = rowintestazione + 1;

      if (erase)
      {
        RawData = null;
      }


      ExcelRange objRange;

      if (RawData == null)
      {
        RawData = new DataTable();

        RawData.Columns.Add("Codice");
        RawData.Columns.Add("Descrizione");
        RawData.Columns.Add("Saldo");
        RawData.Columns.Add("ID");

        for (int i = firstrow; i <= lastRowIncludeFormulas; i++)
        {

          //valuenow = i;

          string value_codice = "";
          string value_descrizione = "";
          string value_saldo = "";
          string value_saldod = "";
          string value_saldoa = "";

          string value_saldofinal = "";

          if (excelSheet != null && nodeBV.Attributes["codice"].Value != "0")
          {
            string ce = nodeBV.Attributes["codice"].Value.ToString() + i.ToString() + ":" + nodeBV.Attributes["codice"].Value.ToString() + i.ToString();
            objRange = excelSheet.Cells[ce];

            if (objRange.Merge)
            {
              value_codice = Convert.ToString((excelSheet.Cells[1, 1]).Text).Trim();
            }
            else
            {
              value_codice = Convert.ToString(objRange.Text).Trim();
            }

            value_codice = value_codice.Replace("\"", "").Replace("&", "").Replace("<", "").Replace(">", "");

            if (value_codice.Trim() == "")
            {
              continue;
            }
            else
            {
              esistealmenounavoce = true;
            }
          }

          if (excelSheet != null && nodeBV.Attributes["descrizione"].Value != "0")
          {
            string ce = nodeBV.Attributes["descrizione"].Value.ToString() + i.ToString() + ":" + nodeBV.Attributes["descrizione"].Value.ToString() + i.ToString();

            objRange = excelSheet.Cells[ce];

            if (objRange.Merge)
            {
              value_descrizione = Convert.ToString((excelSheet.Cells[1, 1]).Text).Trim();
            }
            else
            {
              value_descrizione = Convert.ToString(objRange.Text).Trim();
            }

            value_descrizione = value_descrizione.Replace("\"", "").Replace("&", "").Replace("<", "").Replace(">", "");
          }

          if (excelSheet != null && nodeBV.Attributes["saldo"].Value != "0")
          {
            string ce = nodeBV.Attributes["saldo"].Value.ToString() + i.ToString() + ":" + nodeBV.Attributes["saldo"].Value.ToString() + i.ToString();
            objRange = excelSheet.Cells[ce];
            if (objRange.Merge)
            {
              value_saldo = Convert.ToString((excelSheet.Cells[1, 1]).Text).Trim();
            }
            else
            {
              value_saldo = Convert.ToString(objRange.Text).Trim();
            }

            //value_saldo = excelSheet.Cells[i, nodeBV.Attributes["saldo"].Value].Value2.ToString();
          }

          if (excelSheet != null && nodeBV.Attributes["saldod"].Value != "0")
          {
            string ce = nodeBV.Attributes["saldod"].Value.ToString() + i.ToString() + ":" + nodeBV.Attributes["saldod"].Value.ToString() + i.ToString();
            objRange = excelSheet.Cells[ce];
            if (objRange.Merge)
            {
              value_saldod = Convert.ToString((excelSheet.Cells[1, 1]).Text).Trim();
            }
            else
            {
              value_saldod = Convert.ToString(objRange.Text).Trim();
            }

            //value_saldod = excelSheet.Cells[i, nodeBV.Attributes["saldod"].Value].Value2.ToString();
          }

          if (excelSheet != null && nodeBV.Attributes["saldoa"].Value != "0")
          {
            string ce = nodeBV.Attributes["saldoa"].Value.ToString() + i.ToString() + ":" + nodeBV.Attributes["saldoa"].Value.ToString() + i.ToString();
            objRange = excelSheet.Cells[ce];
            if (objRange.Merge)
            {
              value_saldoa = Convert.ToString((excelSheet.Cells[1, 1]).Text).Trim();
            }
            else
            {
              value_saldoa = Convert.ToString(objRange.Text).Trim();
            }

            //value_saldoa = excelSheet.Cells[i, nodeBV.Attributes["saldoa"].Value].Value2.ToString();
          }

          if (cBusinessObjects.ConvertNumber(value_saldo) == "")
          {
            if (cBusinessObjects.ConvertNumber(value_saldod) == "")
            {
              if (cBusinessObjects.ConvertNumber(value_saldoa) == "")
              {
                continue;
              }
              else
              {
                value_saldofinal = ConvertNumberNeg(value_saldoa);
              }
            }
            else
            {
              value_saldofinal = cBusinessObjects.ConvertNumber(value_saldod);
            }
          }
          else
          {
            value_saldofinal = cBusinessObjects.ConvertNumber(value_saldo);
          }

          string idassociato = "";

          if (nodeBV.SelectSingleNode("ASSOCIAZIONE[@tipobilancio=\"" + tipoBilancio + "\"][@codice=\"" + value_codice + "\"][@ID]") != null)
          {
            idassociato = nodeBV.SelectSingleNode("ASSOCIAZIONE[@tipobilancio=\"" + tipoBilancio + "\"][@codice=\"" + value_codice + "\"]").Attributes["ID"].Value;

            if ((ContoEconomico.Contains(idassociato)) || (PatPassivo.Contains(idassociato))) // && (nodeBV.Attributes["saldoa"].Value == "0")
            {
              double valuehere = 0;
              double.TryParse(value_saldofinal, out valuehere);
              value_saldofinal = cBusinessObjects.ConvertNumber((-1.0 * valuehere).ToString());
            }
          }
          else
          {
            esistealmenounavocenonassociata = true;
          }

          bool rowalreadyexists = false;

          for (int w = 0; w < RawData.Rows.Count; w++)
          {
            if (RawData.Rows[w][0].ToString() == value_codice)
            {
              double olddata = 0.0;
              double newdata = 0.0;

              double.TryParse(RawData.Rows[w][2].ToString(), out olddata);
              double.TryParse(value_saldofinal, out newdata);

              RawData.Rows[w][2] = cBusinessObjects.ConvertNumber((olddata + newdata).ToString());
              rowalreadyexists = true;
              break;
            }
          }

          if (rowalreadyexists == false)
          {
            List<string> tmparray = new List<string>();
            tmparray.Add(value_codice);
            tmparray.Add(value_descrizione);
            tmparray.Add(value_saldofinal);
            tmparray.Add(idassociato);

            RawData.Rows.Add(tmparray.ToArray());
          }
        }
      }
        esistealmenounavocenonassociata = true;
    }

    #endregion

    #region Cestino

    private void VisualizzaListaDaAssociare_Cestino()
    {
      ScrollViewer sw_ElencoBV = (ScrollViewer)this.FindName("sw_ElencoBVCestino");
      sw_ElencoBV.BorderBrush = Brushes.Black;
      sw_ElencoBV.BorderThickness = new Thickness(1);

      StackPanel stpElencoBV = (StackPanel)this.FindName("stpElencoBVCestino");
      if (stpElencoBV == null)
      {
        stpElencoBV = new StackPanel();
        stpElencoBV.Name = "stpElencoBVCestino";
        this.RegisterName(stpElencoBV.Name, stpElencoBV);
      }
      else
      {
        stpElencoBV.Children.Clear();
      }

      stpElencoBV.Orientation = Orientation.Vertical;

      for (int i = 0; i < RawData.Rows.Count; i++)
      {
        if (nodeBV.SelectSingleNode("ASSOCIAZIONE[@tipobilancio=\"" + tipoBilancio + "\"][@codice=\"" + RawData.Rows[i][0].ToString() + "\"]") != null)
        {
          continue;
        }

        StackPanel stp = new StackPanel();
        stp.Orientation = Orientation.Horizontal;

        if (alternate)
        {
          stp.Background = Brushes.LightGray;
          alternate = false;
        }
        else
        {
          alternate = true;
        }

        TextBlock txt = new TextBlock();
        txt.Width = 100;
        txt.Margin = new Thickness(5, 0, 0, 0);
        txt.ToolTip = RawData.Rows[i][0].ToString();
        txt.Text = RawData.Rows[i][0].ToString();
        stp.Children.Add(txt);

        txt = new TextBlock();
        txt.Width = 200;
        txt.Margin = new Thickness(5, 0, 0, 0);
        txt.ToolTip = RawData.Rows[i][1].ToString();
        txt.Text = RawData.Rows[i][1].ToString();
        stp.Children.Add(txt);

        txt = new TextBlock();
        txt.Width = 150;
        txt.Margin = new Thickness(5, 0, 0, 0);
        txt.ToolTip = RawData.Rows[i][2].ToString();
        txt.Text = RawData.Rows[i][2].ToString();
        txt.TextAlignment = TextAlignment.Right;
        stp.Children.Add(txt);

        CheckBox chk = new CheckBox();
        stp.Children.Add(chk);

        stpElencoBV.Children.Add(stp);
      }

      sw_ElencoBV.Content = stpElencoBV;

      sw_ElencoBV.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
      sw_ElencoBV.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    }

    private void VisualizzaListaAssociate_Cestino()
    {
      ScrollViewer sw_ElencoAssociazioni = (ScrollViewer)this.FindName("sw_ElencoAssociazioniCestino");
      sw_ElencoAssociazioni.BorderBrush = Brushes.Black;
      sw_ElencoAssociazioni.BorderThickness = new Thickness(1);

      StackPanel stp = new StackPanel();

      string xmlpath = "";
      xmlpath = "ASSOCIAZIONE[@tipobilancio=\"" + tipoBilancio + "\"][@ID=\"0\"]";

      List<string> lsthere = new List<string>();
      Hashtable hsthere = new Hashtable();

      List<XmlNode> lsttobedeleted = new List<XmlNode>();

      foreach (XmlNode item in nodeBV.SelectNodes(xmlpath))
      {
        if (hsthere.Contains(item.Attributes["codice"].Value))
        {
          lsttobedeleted.Add(item);
        }
        else
        {
          lsthere.Add(item.Attributes["codice"].Value);
          hsthere.Add(item.Attributes["codice"].Value, item);
        }
      }

      try
      {
        for (int i = lsttobedeleted.Count - 1; i >= 0; i--)
        {
          XmlNode aaa = lsttobedeleted[i];
          aaa.ParentNode.RemoveChild(aaa);
        }
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }

      lsthere.Sort();

      foreach (string itemhere in lsthere)
      {
        XmlNode item = (XmlNode)(hsthere[itemhere]);

        StackPanel stpriga = new StackPanel();
        stpriga.Orientation = Orientation.Horizontal;

        if (alternate)
        {
          stpriga.Background = Brushes.LightGray;
          alternate = false;
        }
        else
        {
          alternate = true;
        }

        CheckBox chk = new CheckBox();
        chk.Tag = item.Attributes["codice"].Value;
        stpriga.Children.Add(chk);

        TextBlock txt = new TextBlock();
        txt.Margin = new Thickness(5, 0, 0, 0);
        txt.Width = 200;
        txt.ToolTip = item.Attributes["codice"].Value + " - " + item.Attributes["titolo"].Value;
        txt.Text = item.Attributes["codice"].Value + " - " + item.Attributes["titolo"].Value;
        stpriga.Children.Add(txt);

        stp.Children.Add(stpriga);
      }

      sw_ElencoAssociazioni.Content = stp;
    }

    private void BtnpassaggioVersoCestino_Click(object sender, RoutedEventArgs e)
    {
      StackPanel stpElencoBV = (StackPanel)this.FindName("stpElencoBVCestino");

      ArrayList tobeshown = new ArrayList();

      foreach (StackPanel item in stpElencoBV.Children)
      {
        if (((CheckBox)(item.Children[3])).IsChecked == true)
        {
          tobeshown.Add(((TextBlock)(item.Children[0])).Text + "|" + ((TextBlock)(item.Children[1])).Text);
        }
      }

      if (tobeshown.Count > 0)
      {
        foreach (string item in tobeshown)
        {
          string xmlBV = "<ASSOCIAZIONE tipobilancio=\"" + tipoBilancio + "\" codice=\"" + item.Split('|')[0] + "\" titolo=\"" + item.Split('|')[1] + "\" ID=\"0\" />";
          XmlDocument doctmpBV = new XmlDocument();
          doctmpBV.LoadXml(xmlBV);

          XmlNode tmpNode = doctmpBV.SelectSingleNode("/ASSOCIAZIONE");

          tmpNode = nodeBV.OwnerDocument.ImportNode(tmpNode, true);
          nodeBV.AppendChild(tmpNode);
        }

        mf.SetAnagraficaBV(Convert.ToInt32(IDCLiente), nodeBV);
      }

      VisualizzaListaDaAssociare_Cestino();
      VisualizzaListaAssociate_Cestino();
    }

    private void BtnpassaggioDaCestino_Click(object sender, RoutedEventArgs e)
    {
      ScrollViewer sw_ElencoAssociazioni = (ScrollViewer)this.FindName("sw_ElencoAssociazioniCestino");
      sw_ElencoAssociazioni.BorderBrush = Brushes.Black;
      sw_ElencoAssociazioni.BorderThickness = new Thickness(1);

      StackPanel stpElencoBV = (StackPanel)sw_ElencoAssociazioni.Content;

      ArrayList tobeshown = new ArrayList();

      foreach (StackPanel item in stpElencoBV.Children)
      {
        if (((CheckBox)(item.Children[0])).IsChecked == true)
        {
          tobeshown.Add(((CheckBox)(item.Children[0])).Tag);
        }
      }

      if (tobeshown.Count > 0)
      {
        foreach (string item in tobeshown)
        {
          XmlNode nodehere = nodeBV.SelectSingleNode("ASSOCIAZIONE[@tipobilancio=\"" + tipoBilancio + "\"][@ID=\"0\"][@codice=\"" + item + "\"]");
          if (nodehere != null)
          {
            nodehere.ParentNode.RemoveChild(nodehere);
          }
        }

        mf.SetAnagraficaBV(Convert.ToInt32(IDCLiente), nodeBV);
      }

      VisualizzaListaDaAssociare_Cestino();
      VisualizzaListaAssociate_Cestino();
    }

    #endregion

    #region Bilancio di Verifica

    private void VisualizzaListaDaAssociare()
    {
      ScrollViewer sw_ElencoBV = (ScrollViewer)this.FindName("sw_ElencoBV");
      sw_ElencoBV.BorderBrush = Brushes.Black;
      sw_ElencoBV.BorderThickness = new Thickness(1);

      StackPanel stpElencoBV = (StackPanel)this.FindName("stpElencoBV");
      if (stpElencoBV == null)
      {
        stpElencoBV = new StackPanel();
        stpElencoBV.Name = "stpElencoBV";
        this.RegisterName(stpElencoBV.Name, stpElencoBV);
      }
      else
      {
        stpElencoBV.Children.Clear();
      }
      stpElencoBV.Orientation = Orientation.Vertical;

      for (int i = 0; i < RawData.Rows.Count; i++)
      {
        if (nodeBV.SelectSingleNode("ASSOCIAZIONE[@tipobilancio=\"" + tipoBilancio + "\"][@codice=\"" + RawData.Rows[i][0].ToString() + "\"]") != null)
        {
          continue;
        }

        StackPanel stp = new StackPanel();
        stp.Orientation = Orientation.Horizontal;

        if (alternate)
        {
          stp.Background = Brushes.LightGray;
          alternate = false;
        }
        else
        {
          alternate = true;
        }

        TextBlock txt = new TextBlock();
        txt.Width = 100;
        txt.Margin = new Thickness(5, 0, 0, 0);
        txt.ToolTip = RawData.Rows[i][0].ToString();
        txt.Text = RawData.Rows[i][0].ToString();
        stp.Children.Add(txt);

        txt = new TextBlock();
        txt.Width = 150;
        txt.Margin = new Thickness(5, 0, 0, 0);
        txt.ToolTip = RawData.Rows[i][1].ToString();
        txt.Text = RawData.Rows[i][1].ToString();
        stp.Children.Add(txt);

        CheckBox chk = new CheckBox();
        stp.Children.Add(chk);

        stpElencoBV.Children.Add(stp);
      }


      sw_ElencoBV.Content = stpElencoBV;

      sw_ElencoBV.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
      sw_ElencoBV.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    }

    private void VisualizzaListaAssociate(string idhere)
    {
      ScrollViewer sw_ElencoAssociazioni = (ScrollViewer)this.FindName("sw_ElencoAssociazioni");
      sw_ElencoAssociazioni.BorderBrush = Brushes.Black;
      sw_ElencoAssociazioni.BorderThickness = new Thickness(1);

      StackPanel stp = new StackPanel();

      if (idhere == "")
      {
        ; //Borelli: tolta lista a dx in caso generico
      }
      else
      {
        string xmlpath = "";
        if (idhere == "")
        {
          xmlpath = "ASSOCIAZIONE[@tipobilancio=\"" + tipoBilancio + "\"][@ID!=\"0\"]";
        }
        else
        {
          xmlpath = "ASSOCIAZIONE[@tipobilancio=\"" + tipoBilancio + "\"][@ID=\"" + idhere + "\"]";
        }

        List<string> lsthere = new List<string>();
        Hashtable hsthere = new Hashtable();

        List<XmlNode> lsttobedeleted = new List<XmlNode>();

        foreach (XmlNode item in nodeBV.SelectNodes(xmlpath))
        {
          if (hsthere.Contains(item.Attributes["codice"].Value))
          {
            lsttobedeleted.Add(item);
          }
          else
          {
            lsthere.Add(item.Attributes["codice"].Value);
            hsthere.Add(item.Attributes["codice"].Value, item);
          }
        }

        try
        {
          for (int i = lsttobedeleted.Count - 1; i >= 0; i--)
          {
            XmlNode aaa = lsttobedeleted[i];
            aaa.ParentNode.RemoveChild(aaa);
          }
        }
        catch (Exception ex)
        {
          string log = ex.Message;
        }

        lsthere.Sort();

        foreach (string itemhere in lsthere)
        {
          XmlNode item = (XmlNode)(hsthere[itemhere]);

          StackPanel stpriga = new StackPanel();
          stpriga.Orientation = Orientation.Horizontal;

          if (alternate)
          {
            stpriga.Background = Brushes.LightGray;
            alternate = false;
          }
          else
          {
            alternate = true;
          }

          CheckBox chk = new CheckBox();
          chk.Tag = item.Attributes["ID"].Value + "|" + item.Attributes["codice"].Value;
          stpriga.Children.Add(chk);

          TextBlock txt = new TextBlock();
          txt.Margin = new Thickness(5, 0, 0, 0);
          txt.Width = 400;
          txt.ToolTip = item.Attributes["codice"].Value + " - " + item.Attributes["titolo"].Value;

          string additive = "";
          if (idhere == "")
          {
            if (!valorilabel.Contains(item.Attributes["ID"].Value))
            {
              foreach (DictionaryEntry vda in valoridareavere)
              {
                if ("da_" + vda.Value.ToString().Replace("|", "_") == item.Attributes["ID"].Value)
                {
                  additive = vda.Key.ToString();
                }
              }
            }
            else
            {
              additive = valorilabel[item.Attributes["ID"].Value].ToString();
            }

            additive += " <-- ";
          }

          txt.Text = additive + item.Attributes["codice"].Value + " - " + item.Attributes["titolo"].Value;
          stpriga.Children.Add(txt);

          stp.Children.Add(stpriga);
        }
      }

      sw_ElencoAssociazioni.Content = stp;
    }

    private void BtnpassaggioDaBilancio_Click(object sender, RoutedEventArgs e)
    {
      ScrollViewer sw_ElencoAssociazioni = (ScrollViewer)this.FindName("sw_ElencoAssociazioni");
      sw_ElencoAssociazioni.BorderBrush = Brushes.Black;
      sw_ElencoAssociazioni.BorderThickness = new Thickness(1);

      StackPanel stpElencoBV = (StackPanel)(sw_ElencoAssociazioni.Content);

      ArrayList tobeshown = new ArrayList();

      foreach (StackPanel item in stpElencoBV.Children)
      {
        if (((CheckBox)(item.Children[0])).IsChecked == true)
        {
          tobeshown.Add(((CheckBox)(item.Children[0])).Tag.ToString());
        }
      }

      string IDhere = "";

      if (tobeshown.Count > 0)
      {
        foreach (string item in tobeshown)
        {
          XmlNode nodehere = nodeBV.SelectSingleNode("ASSOCIAZIONE[@tipobilancio=\"" + tipoBilancio + "\"][@ID=\"" + item.Split('|')[0] + "\"][@codice=\"" + item.Split('|')[1] + "\"]");
          if (nodehere != null)
          {
            nodehere.ParentNode.RemoveChild(nodehere);
          }

          IDhere = item.Split('|')[0];
        }

        mf.SetAnagraficaBV(Convert.ToInt32(IDCLiente), nodeBV);
      }

      VisualizzaListaAssociate(IDhere);
      VisualizzaListaDaAssociare();
    }

    string OLDIDBilancioHere = "";

    private void BtnpassaggioVersoBilancio_Click(object sender, RoutedEventArgs e)
    {
      string IDBilancioHere = "0";

      TabControl tabBilanci = (TabControl)this.FindName("tabBilanci");
      foreach (TabItem item in tabBilanci.Items)
      {
        if (item.Content.GetType().Name == "ScrollViewer")
        {
          foreach (StackPanel item2 in ((StackPanel)((ScrollViewer)(item.Content)).Content).Children)
          {
            if ((item2.Children[0]).GetType().Name == "CheckBox")
            {
              if (((CheckBox)(item2.Children[0])).IsChecked == true)
              {
                IDBilancioHere = ((CheckBox)(item2.Children[0])).Name.Replace("chk_", "");
              }
            }
          }
        }
        else
        {
          if (item.Content.GetType().Name == "StackPanel")
          {
            foreach (StackPanel item2 in ((StackPanel)(item.Content)).Children)
            {
              if ((item2.Children[0]).GetType().Name == "CheckBox")
              {
                if (((CheckBox)(item2.Children[0])).IsChecked == true)
                {
                  IDBilancioHere = ((CheckBox)(item2.Children[0])).Name.Replace("chk_", "");
                }
              }
            }
          }
        }
      }

      if (IDBilancioHere == "0")
      {
        MessageBox.Show("Attenzione, Selezionare una voce di bilancio.");
        return;
      }

      if (OLDIDBilancioHere == IDBilancioHere)
      {
        if (MessageBox.Show("Voce appena utilizzata. Confermi la voce di bilancio scelta?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
        {
          return;
        }
      }

      OLDIDBilancioHere = IDBilancioHere;

      StackPanel stpElencoBV = (StackPanel)this.FindName("stpElencoBV");

      ArrayList tobeshown = new ArrayList();

      foreach (StackPanel item in stpElencoBV.Children)
      {
        if (((CheckBox)(item.Children[2])).IsChecked == true)
        {
          tobeshown.Add(((TextBlock)(item.Children[0])).Text + "|" + ((TextBlock)(item.Children[1])).Text);
        }
      }

      if (tobeshown.Count > 0)
      {
        foreach (string item in tobeshown)
        {
          string xmlBV = "<ASSOCIAZIONE tipobilancio=\"" + tipoBilancio + "\" codice=\"" + item.Split('|')[0] + "\" titolo=\"" + item.Split('|')[1] + "\" ID=\"" + IDBilancioHere + "\" />";
          XmlDocument doctmpBV = new XmlDocument();
          doctmpBV.LoadXml(xmlBV);

          XmlNode tmpNode = doctmpBV.SelectSingleNode("/ASSOCIAZIONE");

          tmpNode = nodeBV.OwnerDocument.ImportNode(tmpNode, true);
          nodeBV.AppendChild(tmpNode);
        }

        mf.SetAnagraficaBV(Convert.ToInt32(IDCLiente), nodeBV);
      }

      VisualizzaListaAssociate(IDBilancioHere);
      VisualizzaListaDaAssociare();
    }

    private void ChkBilancio_Unchecked(object sender, RoutedEventArgs e)
    {
      VisualizzaListaAssociate("");
    }

    private void ChkBilancio_Checked(object sender, RoutedEventArgs e)
    {
      TabControl tabBilanci = (TabControl)this.FindName("tabBilanci");
      foreach (TabItem item in tabBilanci.Items)
      {
        if (item.Content.GetType().Name == "ScrollViewer")
        {
          foreach (StackPanel item2 in ((StackPanel)((ScrollViewer)(item.Content)).Content).Children)
          {
            if ((item2.Children[0]).GetType().Name == "CheckBox")
            {
              if (((CheckBox)(item2.Children[0])).Name != ((CheckBox)sender).Name)
              {
                ((CheckBox)(item2.Children[0])).IsChecked = false;

                if ((item2.Children[1]).GetType().Name == "TextBlock")
                {
                  ((TextBlock)(item2.Children[1])).Background = Brushes.White;
                }
              }
              else
              {
                if ((item2.Children[1]).GetType().Name == "TextBlock")
                {
                  ((TextBlock)(item2.Children[1])).Background = App._arrBrushes[0];
                }
              }
            }
          }
        }
        else
        {
          if (item.Content.GetType().Name == "StackPanel")
          {
            foreach (StackPanel item2 in ((StackPanel)(item.Content)).Children)
            {
              if ((item2.Children[0]).GetType().Name == "CheckBox")
              {
                if (((CheckBox)(item2.Children[0])).Name != ((CheckBox)sender).Name)
                {
                  ((CheckBox)(item2.Children[0])).IsChecked = false;

                  if ((item2.Children[1]).GetType().Name == "TextBlock")
                  {
                    ((TextBlock)(item2.Children[1])).Background = Brushes.White;
                  }
                }
                else
                {
                  if ((item2.Children[1]).GetType().Name == "TextBlock")
                  {
                    ((TextBlock)(item2.Children[1])).Background = App._arrBrushes[0];
                  }
                }
              }
            }
          }
        }
      }

      VisualizzaListaAssociate(((CheckBox)sender).Name.Replace("chk_", ""));
    }

    #endregion

    #region final checks

    private void CalcolaListaFinal()
    {
      for (int i = 0; i < RawData.Rows.Count; i++)
      {
        if (RawData.Rows[i][3].ToString() == "")
        {
          foreach (XmlNode item in nodeBV.SelectNodes("ASSOCIAZIONE[@tipobilancio=\"" + tipoBilancio + "\"][@codice=\"" + RawData.Rows[i][0] + "\"]"))
          {
            RawData.Rows[i][3] = item.Attributes["ID"].Value;
          }
        }

        string IDREAL = RawData.Rows[i][3].ToString();
        string valoreREAL = RawData.Rows[i][2].ToString();

        double valuehere = 0;
        double.TryParse(valoreREAL, out valuehere);

        if (IDREAL.Contains("da_"))
        {
          IDREAL = IDREAL.Replace("da_", "");

          if (valuehere >= 0)
          {
            IDREAL = IDREAL.Split('_')[0];
          }
          else
          {
            valoreREAL = cBusinessObjects.ConvertNumber((valuehere * -1.0).ToString());
            IDREAL = IDREAL.Split('_')[1];
          }
        }

        RawData.Rows[i][2] = valoreREAL;
        RawData.Rows[i][3] = IDREAL;
      }
    }

    private void VisualizzaListaFinal()
    {
      CalcolaListaFinal();

      ScrollViewer sw_ElencoAssociazioni = (ScrollViewer)this.FindName("sw_ElencoLastControls");
      sw_ElencoAssociazioni.BorderBrush = Brushes.Black;
      sw_ElencoAssociazioni.BorderThickness = new Thickness(1);

      StackPanel stp = new StackPanel();

      foreach (DictionaryEntry vs in valorisomma)
      {
        bool foundavalue = false;

        for (int i = 0; i < RawData.Rows.Count; i++)
        {
          if (RawData.Rows[i][3].ToString() == vs.Key.ToString())
          {
            foundavalue = true;
            break;
          }
        }

        if (foundavalue == false)
        {
          continue;
        }

        TextBlock txt = new TextBlock();
        txt.FontWeight = FontWeights.Bold;
        txt.Text = valorilabel[vs.Key.ToString()].ToString();
        stp.Children.Add(txt);

        StackPanel stpriga = new StackPanel();
        stpriga.Orientation = Orientation.Horizontal;

        txt = new TextBlock();
        txt.Margin = new Thickness(5, 0, 0, 0);
        txt.Width = 200;
        txt.Text = "";
        stpriga.Children.Add(txt);

        txt = new TextBlock();
        txt.Margin = new Thickness(5, 0, 0, 0);
        txt.Width = 100;
        txt.FontWeight = FontWeights.Bold;
        txt.Text = "ENTRO";
        stpriga.Children.Add(txt);

        txt = new TextBlock();
        txt.Margin = new Thickness(5, 0, 0, 0);
        txt.Width = 100;
        txt.FontWeight = FontWeights.Bold;
        txt.Text = "OLTRE";
        stpriga.Children.Add(txt);

        txt = new TextBlock();
        txt.Margin = new Thickness(5, 0, 0, 0);
        txt.Width = 100;
        txt.FontWeight = FontWeights.Bold;
        txt.Text = "TOTALE";
        stpriga.Children.Add(txt);

        stp.Children.Add(stpriga);

        double totalvalue = 0;
        double outvalue = 0;

        for (int i = 0; i < RawData.Rows.Count; i++)
        {
          if (RawData.Rows[i][3].ToString() == vs.Key.ToString())
          {
            stpriga = new StackPanel();
            stpriga.Orientation = Orientation.Horizontal;

            txt = new TextBlock();
            txt.Margin = new Thickness(5, 0, 0, 0);
            txt.Width = 200;
            txt.Tag = RawData.Rows[i][0].ToString();
            txt.ToolTip = RawData.Rows[i][1].ToString();
            txt.Text = RawData.Rows[i][0].ToString() + " - " + RawData.Rows[i][1].ToString();
            stpriga.Children.Add(txt);

            outvalue = 0;
            double.TryParse(RawData.Rows[i][2].ToString(), out outvalue);
            totalvalue += outvalue;

            TextBox entro = new TextBox();
            entro.Tag = "entro_" + vs.Key.ToString();
            entro.Margin = new Thickness(5, 0, 0, 0);
            entro.Width = 100;
            entro.TextAlignment = TextAlignment.Right;
            entro.Text = RawData.Rows[i][2].ToString();
            entro.LostFocus += Entro_LostFocus;
            stpriga.Children.Add(entro);

            TextBox oltre = new TextBox();
            oltre.Tag = "oltre_" + vs.Key.ToString();
            oltre.Margin = new Thickness(5, 0, 0, 0);
            oltre.Width = 100;
            oltre.TextAlignment = TextAlignment.Right;
            oltre.LostFocus += Entro_LostFocus;
            stpriga.Children.Add(oltre);

            txt = new TextBlock();
            txt.Tag = "totale_" + vs.Key.ToString();
            txt.Margin = new Thickness(5, 0, 0, 0);
            txt.Width = 100;
            txt.TextAlignment = TextAlignment.Right;
            txt.Text = RawData.Rows[i][2].ToString();
            stpriga.Children.Add(txt);

            stp.Children.Add(stpriga);
          }
        }

        stpriga = new StackPanel();
        stpriga.Orientation = Orientation.Horizontal;

        txt = new TextBlock();
        txt.Margin = new Thickness(5, 0, 0, 0);
        txt.Width = 200;
        txt.Text = "";
        stpriga.Children.Add(txt);

        txt = new TextBlock();
        txt.Margin = new Thickness(5, 0, 0, 0);
        txt.Tag = "tot_entro_" + vs.Key.ToString();
        txt.Text = cBusinessObjects.ConvertNumber(totalvalue.ToString());
        txt.TextAlignment = TextAlignment.Right;
        txt.Width = 100;
        txt.FontWeight = FontWeights.Bold;
        stpriga.Children.Add(txt);

        txt = new TextBlock();
        txt.Margin = new Thickness(5, 0, 0, 0);
        txt.Tag = "tot_oltre_" + vs.Key.ToString();
        txt.TextAlignment = TextAlignment.Right;
        txt.Width = 100;
        txt.FontWeight = FontWeights.Bold;
        stpriga.Children.Add(txt);

        txt = new TextBlock();
        txt.Margin = new Thickness(5, 0, 0, 0);
        txt.Tag = "tot_totale_" + vs.Key.ToString();
        txt.TextAlignment = TextAlignment.Right;
        txt.Width = 100;
        txt.Text = cBusinessObjects.ConvertNumber(totalvalue.ToString());
        txt.FontWeight = FontWeights.Bold;
        stpriga.Children.Add(txt);

        stp.Children.Add(stpriga);
      }

      sw_ElencoAssociazioni.Content = stp;
    }

    private void Entro_LostFocus(object sender, RoutedEventArgs e)
    {
      StackPanel stp = (StackPanel)(((TextBox)sender).Parent);

      double outvalue = 0;
      double outvalue2 = 0;

      if (((TextBox)sender).Tag.ToString().Contains("entro"))
      {
        double.TryParse(((TextBlock)(stp.Children[3])).Text, out outvalue);
        double.TryParse(((TextBox)(stp.Children[1])).Text, out outvalue2);

        ((TextBox)(stp.Children[2])).Text = cBusinessObjects.ConvertNumber((outvalue - outvalue2).ToString());
      }
      else
      {
        double.TryParse(((TextBlock)(stp.Children[3])).Text, out outvalue);
        double.TryParse(((TextBox)(stp.Children[2])).Text, out outvalue2);

        ((TextBox)(stp.Children[1])).Text = cBusinessObjects.ConvertNumber((outvalue - outvalue2).ToString());
      }


      StackPanel stpparent = (StackPanel)(stp.Parent);

      double totalentro = 0;
      double totaloltre = 0;

      foreach (object item in stpparent.Children)
      {
        if (item.GetType().Name == "StackPanel")
        {
          if (((StackPanel)(item)).Children[1].GetType().Name == "TextBox")
          {
            outvalue = 0;
            double.TryParse(((TextBox)(((StackPanel)(item)).Children[1])).Text, out outvalue);
            totalentro += outvalue;

            outvalue = 0;
            double.TryParse(((TextBox)(((StackPanel)(item)).Children[2])).Text, out outvalue);
            totaloltre += outvalue;
          }
          else if (((StackPanel)(item)).Children[1].GetType().Name == "TextBlock")
          {
            if (((TextBlock)(((StackPanel)(item)).Children[1])).Tag != null && ((TextBlock)(((StackPanel)(item)).Children[1])).Tag.ToString().Contains("tot_"))
            {
              ((TextBlock)(((StackPanel)(item)).Children[1])).Text = cBusinessObjects.ConvertNumber(totalentro.ToString());
              ((TextBlock)(((StackPanel)(item)).Children[2])).Text = cBusinessObjects.ConvertNumber(totaloltre.ToString());
            }
          }
        }
        else
        {
          totalentro = 0;
          totaloltre = 0;
        }
      }

    }

    private string CheckDiBilancio()
    {
      double totEconomico = 0.0;
      double totPP = 0.0;
      double totPA = 0.0;

      for (int i = 0; i < RawData.Rows.Count; i++)
      {
        string codice = "";
        string titolo = "";
        string id1 = "";
        string id2 = "";
        string valore1 = "";
        string valore2 = "";

        if (valorisomma.Contains(RawData.Rows[i][3].ToString()))
        {
          ScrollViewer sw_ElencoAssociazioni = (ScrollViewer)this.FindName("sw_ElencoLastControls");
          sw_ElencoAssociazioni.BorderBrush = Brushes.Black;
          sw_ElencoAssociazioni.BorderThickness = new Thickness(1);

          foreach (object item in ((StackPanel)(sw_ElencoAssociazioni.Content)).Children)
          {
            if (item.GetType().Name == "StackPanel")
            {
              if (((StackPanel)(item)).Children[1].GetType().Name == "TextBox")
              {
                if (((TextBlock)(((StackPanel)(item)).Children[0])).Tag.ToString() == RawData.Rows[i][0].ToString())
                {
                  codice = ((TextBlock)(((StackPanel)(item)).Children[0])).Tag.ToString();
                  titolo = ((TextBlock)(((StackPanel)(item)).Children[0])).ToolTip.ToString();

                  valore1 = ((TextBox)(((StackPanel)(item)).Children[1])).Text;
                  valore2 = ((TextBox)(((StackPanel)(item)).Children[2])).Text;

                  id1 = valorisomma[RawData.Rows[i][3].ToString()].ToString().Split('|')[0];

                  id2 = valorisomma[RawData.Rows[i][3].ToString()].ToString().Split('|')[1];

                  break;
                }
              }
            }
          }
        }
        else
        {
          codice = RawData.Rows[i][0].ToString();
          titolo = RawData.Rows[i][1].ToString();
          valore1 = RawData.Rows[i][2].ToString();
          id1 = RawData.Rows[i][3].ToString();
          id2 = "";
          valore2 = "";
        }

        if (id1 != "")
        {
          double dblValoreNEW = 0.0;
          double.TryParse(valore1, out dblValoreNEW);

          if (ContoEconomico.Contains(id1))
          {
            totEconomico += dblValoreNEW;
          }

          if (PatAttivo.Contains(id1))
          {
            totPA += dblValoreNEW;
          }

          if (PatPassivo.Contains(id1))
          {
            totPP += dblValoreNEW;
          }
        }

        if (id2 != "")
        {
          double dblValoreNEW = 0.0;
          double.TryParse(valore2, out dblValoreNEW);

          if (ContoEconomico.Contains(id2))
          {
            totEconomico += dblValoreNEW;
          }

          if (PatAttivo.Contains(id2))
          {
            totPA += dblValoreNEW;
          }

          if (PatPassivo.Contains(id2))
          {
            totPP += dblValoreNEW;
          }
        }
      }

      if (totPP + totEconomico == totPA)
      {
        checkTotaleAZero = true;
      }
      else
      {
        checkTotaleAZero = false;
      }

      return ConvertNumberNoDecimal(totEconomico.ToString());
    }

    //------------------------------------------------------------------------+
    //                           BtnFinalizza_Click                           |
    //------------------------------------------------------------------------+
    private void BtnFinalizza_Click(object sender, RoutedEventArgs e)
    {
      DataRow tmpnode = null;
      double totEconomico = 0.0;
      string totUtilePerdita = CheckDiBilancio();
      string str = string.Empty;

      e.Handled = true;
      if (checkTotaleAZero == false)
      {
        ;// MessageBox.Show("Attenzione il bilancio non quadra, effettuare ulteriori controlli al \nbilancio di verifica importato ed eventualmente ripetere l'operazione \ndi importazione dopo aver corretto eventuali errori di assegnazione");
      }
      //-------------------------------------------- cancellazione dati attuali
      cBusinessObjects.Executesql(
        "DELETE FROM BilancioVerifica " +
        "WHERE esercizio='" + esercizioinesame + "' " +
          "AND ID_SCHEDA=" + cBusinessObjects.GetIDTree(id).ToString() + " " +
          "AND ID_CLIENTE=" + cBusinessObjects.idcliente.ToString() + " " +
          "AND ID_SESSIONE=" + cBusinessObjects.idsessione.ToString());
      //---------------------------------------------------------- lettura dati
      datiBV = cBusinessObjects.GetData(id, typeof(BilancioVerifica));
      datibilanciohere = cBusinessObjects.GetData(id, typeof(Excel_Bilancio));
    
      foreach (DataRow dt in datibilanciohere.Rows)
            {
                dt[esercizioinesame] = 0;
                dt["DIFF"] = 0;
            }

      //---------------------------------- RawData: member variable (DataTable)
      if (RawData != null)
      {
        for (int i = 0; i < RawData.Rows.Count; i++)
        {
          string codice = "";
          string titolo = "";
          string id1 = "";
          string id2 = "";
          string valore1 = "";
          string valore2 = "";
          if (valorisomma.Contains(RawData.Rows[i][3].ToString()))
          {
            ScrollViewer sw_ElencoAssociazioni = (ScrollViewer)this.FindName("sw_ElencoLastControls");
            sw_ElencoAssociazioni.BorderBrush = Brushes.Black;
            sw_ElencoAssociazioni.BorderThickness = new Thickness(1);
            foreach (object item in ((StackPanel)(sw_ElencoAssociazioni.Content)).Children)
            {
              if (item.GetType().Name == "StackPanel")
              {
                if (((StackPanel)(item)).Children[1].GetType().Name == "TextBox")
                {
                  if (((TextBlock)(((StackPanel)(item)).Children[0])).Tag.ToString() == RawData.Rows[i][0].ToString())
                  {
                    codice = ((TextBlock)(((StackPanel)(item)).Children[0])).Tag.ToString();
                    titolo = ((TextBlock)(((StackPanel)(item)).Children[0])).ToolTip.ToString();
                    valore1 = ((TextBox)(((StackPanel)(item)).Children[1])).Text;
                    valore2 = ((TextBox)(((StackPanel)(item)).Children[2])).Text;
                    id1 = valorisomma[RawData.Rows[i][3].ToString()].ToString().Split('|')[0];
                    id2 = valorisomma[RawData.Rows[i][3].ToString()].ToString().Split('|')[1];
                    break;
                  }
                }
              }
            }
          }
          else
          {
            codice = RawData.Rows[i][0].ToString();
            titolo = RawData.Rows[i][1].ToString();
            valore1 = RawData.Rows[i][2].ToString();
            id1 = RawData.Rows[i][3].ToString();
            id2 = "";
            valore2 = "";
          }
          if (id1 != "")
          {
            tmpnode = null;
            foreach (DataRow dt in datibilanciohere.Rows)
            {
              if (dt["ID"].ToString() == id1)
                tmpnode = dt;
            }
            if (tmpnode == null) continue;
            double dblValoreOLD = 0.0;
            double dblValoreNEW = 0.0;
            double.TryParse(tmpnode[esercizioinesame].ToString(), out dblValoreOLD);
            double.TryParse(valore1, out dblValoreNEW);
            try
            {
              tmpnode[esercizioinesame] = ConvertNumberNoDecimal(
                (dblValoreOLD + dblValoreNEW).ToString());
            }
            catch (Exception)
            {
              tmpnode[esercizioinesame] = "0";
            }
            DataRow tmpnodeBV = null;
            foreach (DataRow dt in datiBV.Rows)
            {
              if ((dt["ID"].ToString() == id1) && (dt["codice"].ToString() == codice)
                && (dt["esercizio"].ToString() == esercizioinesame))
                tmpnodeBV = dt;
            }
            if (tmpnodeBV == null)
              tmpnodeBV = datiBV.Rows.Add(
                id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            tmpnodeBV["esercizio"] = esercizioinesame;
            tmpnodeBV["codice"] = codice;
            tmpnodeBV["ID"] = id1;
            tmpnodeBV["valore"] = valore1;
            tmpnodeBV["titolo"] = titolo;
          }
          if (id2 != "")
          {
            tmpnode = null;
            foreach (DataRow dt in datibilanciohere.Rows)
            {
              if (dt["ID"].ToString() == id2) tmpnode = dt;
            }
            if (tmpnode == null) continue;
            double dblValoreOLD = 0.0;
            double dblValoreNEW = 0.0;
            double.TryParse(tmpnode[esercizioinesame].ToString(), out dblValoreOLD);
            double.TryParse(valore2, out dblValoreNEW);
            double dblValoreTemp1 = 0.0;
            double.TryParse((dblValoreOLD + dblValoreNEW).ToString(), out dblValoreTemp1);
            tmpnode[esercizioinesame] = dblValoreTemp1;
            DataRow tmpnodeBV = null;
            foreach (DataRow dt in datiBV.Rows)
            {
              if ((dt["ID"].ToString() == id2) && (dt["codice"].ToString() == codice)
                && (dt["esercizio"].ToString() == esercizioinesame))
                tmpnodeBV = dt;
            }
            if (tmpnodeBV == null)
              tmpnodeBV = datiBV.Rows.Add(
                id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            tmpnodeBV["esercizio"] = esercizioinesame;
            tmpnodeBV["codice"] = codice;
            tmpnodeBV["ID"] = id2;
            tmpnodeBV["valore"] = valore2;
            tmpnodeBV["titolo"] = titolo;
          }
        }
        totEconomico = 0.0;
        foreach (string ce in ContoEconomico)
        {
          foreach (DataRow dt in datibilanciohere.Rows)
          {
            if (dt["ID"].ToString() == ce)
            {
              double valoreeconomicohere = 0;
              try
              {
                str = ConvertNumberNoDecimal(dt[esercizioinesame].ToString());
              }
              catch (Exception) { str = "0"; }
              double.TryParse(str, out valoreeconomicohere);
              totEconomico += valoreeconomicohere;
            }
          }
        }
        double totPA = 0.0;
        foreach (string ce in PatAttivo)
        {
          foreach (DataRow dt in datibilanciohere.Rows)
          {
            if (dt["ID"].ToString() == ce)
            {
              double valoreeconomicohere = 0;
              try
              {
                str = ConvertNumberNoDecimal(dt[esercizioinesame].ToString());
              }
              catch (Exception) { str = "0"; }
              double.TryParse(str, out valoreeconomicohere);
              totPA += valoreeconomicohere;
            }
          }
        }
        double totPP = 0.0;
        foreach (string ce in PatPassivo)
        {
          foreach (DataRow dt in datibilanciohere.Rows)
          {
            if (dt["ID"].ToString() == ce)
            {
              double valoreeconomicohere = 0;
              try
              {
                str = ConvertNumberNoDecimal(dt[esercizioinesame].ToString());
              }
              catch (Exception) { str = "0"; }
              double.TryParse(str, out valoreeconomicohere);
              totPP += valoreeconomicohere;
            }
          }
        }
        DataRow tmprow = null;
        foreach (DataRow dt in datibilanciohere.Rows)
        {
          if (dt["ID"].ToString() == PerditaUtileID)
          {
            tmprow = dt;
          }
        }
        if (tmprow == null)
        {
          foreach (DataRow dt in datibilanciohere.Rows)
          {
            if (dt["ID"].ToString() == "120")
            {
              tmprow = dt;
            }
          }
        }
        if (tmprow == null)
        {
          tmprow = datibilanciohere.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
          tmprow["ID"] = "120";
        }
        tmprow[esercizioinesame] = double.Parse(totEconomico.ToString());
        tmprow = null;
        foreach (DataRow dt in datibilanciohere.Rows)
        {
          if (dt["ID"].ToString() == ArrotondamentoID)
          {
            tmprow = dt;
          }
        }
        if (tmprow == null)
        {
          tmprow = datibilanciohere.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
          tmprow["ID"] = ArrotondamentoID;
        }
        double valorearrotondamentohere = 0;
        try
        {
          str = ConvertNumberNoDecimal(tmprow[esercizioinesame].ToString());
        }
        catch (Exception) { str = "0"; }
        double.TryParse(str, out valorearrotondamentohere);
        double dblValoreTemp = 0.0;
        double.TryParse((valorearrotondamentohere + totPA - totPP - totEconomico).ToString(), out dblValoreTemp);
        tmprow[esercizioinesame] = dblValoreTemp;
        cBusinessObjects.SaveData(id, datiBV, typeof(BilancioVerifica));
        cBusinessObjects.Executesql(
          "DELETE FROM Excel_Bilancio " +
          "WHERE ID_SCHEDA=" + cBusinessObjects.GetIDTree(id).ToString() + " " +
            "AND ID_CLIENTE=" + cBusinessObjects.idcliente.ToString() + " " +
            "AND ID_SESSIONE=" + cBusinessObjects.idsessione.ToString());
        cBusinessObjects.SaveData(id, datibilanciohere, typeof(Excel_Bilancio));
        MessageBox.Show("Passaggio effettuato con successo");
        Close();
      } // if (RawData != null)
    }

    #endregion

    #region ALTRO
    private string ColumnIndexToColumnLetter(int colIndex)
    {
      int div = colIndex;
      string colLetter = String.Empty;
      int mod = 0;

      while (div > 0)
      {
        mod = (div - 1) % 26;
        colLetter = (char)(65 + mod) + colLetter;
        div = (int)((div - mod) / 26);
      }
      return colLetter;
    }


    private string ConvertNumberNeg(string valore)
    {
      double dblValore = 0.0;

      double.TryParse(valore, out dblValore);

      if (dblValore == 0.0)
      {
        return "";
      }
      else
      {
        return String.Format("{0:#,#.00}", dblValore * (-1.0));
      }
    }

    private string ConvertNumberNoDecimal(string valore)
    {
      double dblValore = 0.0;

      double.TryParse(valore, out dblValore);

      if (dblValore == 0.0)
      {
        return "";
      }
      else
      {
        return String.Format("{0:#,0}", Math.Round(dblValore));
      }
    }
    #endregion

  }
}
