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



namespace RevisoftApplication
{
  public partial class wSchedaRotazioneScorte : System.Windows.Window
  {
    public enum TipologieCampionamento { Sconosciuto, Clienti, Fornitori, Magazzino };
    public TipologieCampionamento _tipologia = TipologieCampionamento.Sconosciuto;
    public XmlDataProviderManager RevisioneAssociata = null;

    DataSet RawData = null;

    ArrayList ALrowsstratificate_scelte = new ArrayList();
    ArrayList ALrowsstratificate = new ArrayList();
    ArrayList ALrowsIntermediatevalue = new ArrayList();
    ArrayList CompleteListStratification = new ArrayList();
    ArrayList ALtxtTipoCampionamento_Info = new ArrayList();
    ArrayList ALtxtTotaleSaldiCampione = new ArrayList();
    ArrayList ALtxtTotaleSaldo = new ArrayList();

    bool esistealmenounavoce = false;

    DataSet FinalData = null;
    ExcelPackage excelworkBook = null;
    ExcelWorksheet excelSheet = null;
    int lastColIncludeFormulas = 0;
    int lastRowIncludeFormulas = 0;

    Hashtable valorilabel = new Hashtable();

    List<string> colonne = new List<string>();

    int rowintestazione = 1;

    string indexcolumnsaldo = "0";
    double intervalloMIN = 0.0;
    double intervalloMAX = 0.0;
    List<double> intervalli = new List<double>();

    private string left = "./Images/icone/wa_nav_sess_prev.png";
    private string right = "./Images/icone/wa_nav_sess_next.png";
    private string addimg = "./Images/icone/add2.png";
    private string deleteimg = "./Images/icone/close.png";

    public class ComboboxItem
    {
      public string Text { get; set; }
      public string Value { get; set; }

      public override string ToString()
      {
        return Text;
      }
    }

    public XmlNode node
    {
      get
      {
        return ((XmlNode)(((WindowWorkArea)(this.Owner))._x.Document.SelectSingleNode("/Dati//Dato[@ID='" + ((XmlNode)(((WindowWorkArea)(this.Owner)).Nodes[((WindowWorkArea)(this.Owner)).NodeNow])).Attributes["ID"].Value + "']")));
      }
    }

    public wSchedaRotazioneScorte()
    {
      if (esistealmenounavoce || excelworkBook == null || indexcolumnsaldo.Equals("")
          || intervalloMIN == 0.0 || intervalloMAX == 0.0 || left.Equals("") || right.Equals("")
          || addimg.Equals("") || deleteimg.Equals("")) { }
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
    }

    public bool Load()
    {
      colonne.Clear();

      labelTitolo.Content = "Tasso di rotazione delle scorte";

      colonne.Add("Codice");
      colonne.Add("Descrizione");
      colonne.Add("Unità di misura");
      colonne.Add("Totale CARICO");
      colonne.Add("Totale SCARICO");
      colonne.Add("Quantità GIACENTE");
      colonne.Add("VALORE quantità giacente");
      colonne.Add("Attributo di STRATIFICAZIONE");

      indexcolumnsaldo = "6";

      if (node != null && node.Attributes["TassoRotazione"] != null && node.Attributes["TassoRotazione"].Value != "<NewDataSet />")
      {
        using (StringReader sw = new StringReader(node.Attributes["TassoRotazione"].Value))
        {
          FinalData = new DataSet();
          FinalData.ReadXml(sw);
        }

        if (node != null && node.Attributes["RawData"] != null)
        {
          using (StringReader sw = new StringReader(node.Attributes["RawData"].Value))
          {
            RawData = new DataSet();
            RawData.ReadXml(sw);
          }
        }
      }
      else
      {
        FinalData = null;

        if (node != null && node.Attributes["RawData"] != null)
        {
          using (StringReader sw = new StringReader(node.Attributes["RawData"].Value))
          {
            RawData = new DataSet();
            RawData.ReadXml(sw);
          }
        }
        else
        {
          MessageBox.Show("Caricare i dati attraverso il pulsante di campionamento.");
          this.Close();
          return false;

          //RawData = null;

          //Utilities u = new Utilities();
          //string Nomefile = u.sys_OpenFileDialog("", App.TipoFile.BilancioDiVerifica);

          //if (Nomefile == "")
          //{
          //    return false;
          //}

          //if (node.Attributes["Nomefile"] == null)
          //{
          //    XmlAttribute attr = node.OwnerDocument.CreateAttribute("Nomefile");
          //    node.Attributes.Append(attr);
          //}

          //node.Attributes["Nomefile"].Value = Nomefile;

          //excel = new Microsoft.Office.Interop.Excel.Application();

          //excel.Visible = false;
          //excel.DisplayAlerts = false;
          //excel.ScreenUpdating = false;
          ////excel.Calculation = Microsoft.Office.Interop.Excel.XlCalculation.xlCalculationManual;

          //excelworkBook = excel.Workbooks.Open(Nomefile, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

          //excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelworkBook.Worksheets.get_Item(1);
        }
      }

      CreateInterface();

      return true;
    }

    private void CreateInterface()
    {
      stpFinal.Visibility = Visibility.Collapsed;

      valorilabel = new Hashtable();

      #region riga intestazione

      StackPanel stpIntestazione = new StackPanel();
      if (RawData == null && FinalData == null)
      {
        stpIntestazione.Visibility = Visibility.Visible;

        stpIntestazione.Margin = new Thickness(10);
        stpIntestazione.Orientation = Orientation.Vertical;
        stpIntestazione.Name = "stpIntestazione";
        try
        {
          this.UnregisterName(stpIntestazione.Name);
        }
        catch (Exception ex)
        {
          string log = ex.Message;
        }
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
        lbl.Text = "dovrà contenere un solo foglio.";
        stptext.Children.Add(lbl);

        lbl = new TextBlock();
        lbl.Text = "Tutte le righe dovranno essere in soluzione di continuità.";
        stptext.Children.Add(lbl);

        stpRiga_label.Children.Add(stptext);

        lbl = new TextBlock();
        lbl.Text = "Qualora fossero già presenti delle suddivisioni, queste dovranno essere messe esplicitamente in una apposita colonna, in modo da unire le suddivisioni.";
        stpRiga_label.Children.Add(lbl);

        lbl = new TextBlock();
        lbl.Margin = new Thickness(0, 15, 0, 0);
        lbl.FontWeight = FontWeights.Bold;
        lbl.Text = "Fase 1) Selezionare la riga contenente l'intestazione.";
        stpRiga_label.Children.Add(lbl);

        lbl = new TextBlock();
        lbl.Text = "Selezionare la riga che indica il contenuto delle colonne del file XLS da importare (";
        for (int i = 0; i < colonne.Count; i++)
        {
          if (i != 0)
          {
            lbl.Text += ", ";
          }
          lbl.Text += colonne[i];
        }
        lbl.Text += ")";

        stpRiga_label.Children.Add(lbl);

        lbl = new TextBlock();
        lbl.Text = "Il software non importerà le righe che precedono quella selezionata.";
        stpRiga_label.Children.Add(lbl);

        stpRiga_1.Children.Add(stpRiga_label);

        int irow = 1;
        lastColIncludeFormulas = 1;
        lastRowIncludeFormulas = 20;


        irow = excelSheet.Dimension.End.Row;

        lastColIncludeFormulas = excelSheet.Cells.Where(cell => !cell.Value.ToString().Equals("")).Last().End.Column;
        lastRowIncludeFormulas = excelSheet.Cells.Where(cell => !cell.Value.ToString().Equals("")).Last().End.Row;


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

        try
        {
          this.UnregisterName(lst.Name);
        }
        catch (Exception ex)
        {
          string log = ex.Message;
        }
        this.RegisterName(lst.Name, lst);

        ScrollViewer sw = new ScrollViewer();
        sw.MaxWidth = 1000.0;
        sw.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

        StackPanel stpRiga_2 = new StackPanel();
        stpRiga_2.Name = "Riga2";
        stpRiga_2.Orientation = Orientation.Horizontal;

        try
        {
          this.UnregisterName(stpRiga_2.Name);
        }
        catch (Exception ex)
        {
          string log = ex.Message;
        }
        this.RegisterName(stpRiga_2.Name, stpRiga_2);

        sw.Content = stpRiga_2;

        stpRiga_1.Children.Add(lst);

        stpIntestazione.Children.Add(stpRiga_1);
        stpIntestazione.Children.Add(sw);

        stackPanel1.Children.Add(stpIntestazione);
      }
      else
      {
        stpIntestazione.Visibility = Visibility.Collapsed;
      }

      #endregion

      #region riga scelta colonne

      StackPanel stpScelte = new StackPanel();
      if (RawData == null && FinalData == null)
      {
        stpScelte.Visibility = Visibility.Visible;

        stpScelte.Margin = new Thickness(10);
        stpScelte.Name = "stpScelte";

        try
        {
          this.UnregisterName(stpScelte.Name);
        }
        catch (Exception ex)
        {
          string log = ex.Message;
        }
        this.RegisterName(stpScelte.Name, stpScelte);

        StackPanel stpRiga_label = new StackPanel();
        stpRiga_label.Orientation = Orientation.Vertical;

        TextBlock lbl = new TextBlock();
        lbl.Text = "Fase 2) Selezionare le colonne.";
        lbl.FontWeight = FontWeights.Bold;
        stpRiga_label.Children.Add(lbl);

        lbl = new TextBlock();
        lbl.Text = "Indicare a fianco di ciascuna voce sottostante, la LETTERA della COLONNA del file XLS contenente i dati da importare con dell’apposita tendina.";
        stpRiga_label.Children.Add(lbl);

        stpScelte.Children.Add(stpRiga_label);

        for (int i = 0; i < colonne.Count; i++)
        {
          StackPanel stpRiga_colonna = new StackPanel();
          stpRiga_colonna.Margin = new Thickness(0, 10, 0, 0);
          stpRiga_colonna.Orientation = Orientation.Horizontal;
          lbl = new TextBlock();
          lbl.Text = colonne[i];
          lbl.Width = 200;
          stpRiga_colonna.Children.Add(lbl);

          ComboBox lst_colonna = new ComboBox();
          lst_colonna.Name = "lst_" + i.ToString();
          lst_colonna.SelectionChanged += Lst_Colonna_SelectionChanged;
          lst_colonna.Width = 200;
          lst_colonna.Margin = new Thickness(10, 0, 0, 0);

          try
          {
            this.UnregisterName(lst_colonna.Name);
          }
          catch (Exception ex)
          {
            string log = ex.Message;
          }
          this.RegisterName(lst_colonna.Name, lst_colonna);
          lst_colonna.Items.Clear();
          stpRiga_colonna.Children.Add(lst_colonna);

          if (i == colonne.Count - 1)
          {
            ComboBox lst_attr = new ComboBox();
            lst_attr.Name = "lst_attr";
            lst_attr.Width = 150;
            lst_attr.Margin = new Thickness(10, 0, 0, 0);

            try
            {
              this.UnregisterName(lst_attr.Name);
            }
            catch (Exception ex)
            {
              string log = ex.Message;
            }
            this.RegisterName(lst_attr.Name, lst_attr);
            lst_attr.Items.Clear();
            lst_attr.Items.Add("Aree geografiche");
            lst_attr.Items.Add("Canali distributivi");
            lst_attr.Items.Add("Settori merceologici");
            lst_attr.Items.Add("Vetustà del credito");
            lst_attr.Items.Add("Unità di misura");
            lst_attr.Items.Add("Altri");

            stpRiga_colonna.Children.Add(lst_attr);

            TextBox txt_attr = new TextBox();
            txt_attr.Name = "txt_attr";
            txt_attr.Width = 250;
            txt_attr.Margin = new Thickness(10, 0, 0, 0);

            try
            {
              this.UnregisterName(txt_attr.Name);
            }
            catch (Exception ex)
            {
              string log = ex.Message;
            }
            this.RegisterName(txt_attr.Name, txt_attr);

            stpRiga_colonna.Children.Add(txt_attr);
          }

          stpScelte.Children.Add(stpRiga_colonna);
        }

        StackPanel stpBottoni = new StackPanel();
        stpBottoni.Orientation = Orientation.Horizontal;

        Button btn = new Button();
        btn.HorizontalAlignment = HorizontalAlignment.Right;
        btn.Width = 100.0;
        btn.Margin = new Thickness(20);
        btn.Padding = new Thickness(10);
        btn.Content = "Avanti";
        btn.Click += Btn_Next_SceltaColonne_Click;

        stpBottoni.Children.Add(btn);

        stpScelte.Children.Add(stpBottoni);

        stackPanel1.Children.Add(stpScelte);
      }
      else
      {
        stpScelte.Visibility = Visibility.Collapsed;
      }
      #endregion

      if (RawData != null)
      {
        CalculatefinalTable();
      }

      if (FinalData != null)
      {
        if (node != null && node.Attributes["TassoRotazioneSelected"] != null)
        {
          CreateFinalFinal();
        }
        else
        {
          CreateFinal();
        }
      }
    }

    #region Bottoni Next / Prev


    private void ButtonNext_Click(object sender, RoutedEventArgs e)
    {
      //if (node != null)
      //{
      //    if (node.Attributes["TassoRotazioneSelected"] == null)
      //    {
      //        XmlAttribute attr = node.OwnerDocument.CreateAttribute("TassoRotazioneSelected");
      //        node.Attributes.Append(attr);
      //    }

      //    string result = "";

      //    for (int i = 0; i < FinalData.Tables[0].Rows.Count; i++)
      //    {
      //        if(FinalData.Tables[0].Rows[i][FinalData.Tables[0].Columns.Count - 1].ToString() == "True")
      //        {
      //            result += ((result == "") ? "" : "|") + FinalData.Tables[0].Rows[i][0].ToString();
      //        }
      //    }

      //    node.Attributes["TassoRotazioneSelected"].Value = result;
      //}

      //((WindowWorkArea)(this.Owner))._x.Save();

      if (node == null || node.Attributes["TassoRotazioneSelected"] == null || node.Attributes["TassoRotazioneSelected"].Value == "" || node.Attributes["TassoRotazioneSelected"].Value == "|")
      {
        MessageBox.Show("Selezionare almeno una riga");
        return;
      }

      CreateFinalFinal();
    }

    private void ButtonBack_Click(object sender, RoutedEventArgs e)
    {
      if (node.Attributes["TassoRotazioneMotivazione"] == null)
      {
        XmlAttribute attr = node.OwnerDocument.CreateAttribute("TassoRotazioneMotivazione");
        attr.Value = "";
        node.Attributes.Append(attr);
      }

      node.Attributes["TassoRotazioneMotivazione"].Value = txtMotivazione.Text;

      ((WindowWorkArea)(this.Owner))._x.Save();

      CreateFinal();
    }

    private void Btn_Next_SceltaColonne_Click(object sender, RoutedEventArgs e)
    {
      StackPanel stpIntestazione = (StackPanel)this.FindName("stpIntestazione");
      stpIntestazione.Visibility = Visibility.Collapsed;

      StackPanel stpScelte = (StackPanel)this.FindName("stpScelte");
      stpScelte.Visibility = Visibility.Collapsed;

      StackPanel stpIntervalliMonetari = (StackPanel)this.FindName("stpFinal");
      stpIntervalliMonetari.Visibility = Visibility.Visible;

      for (int i = 0; i < colonne.Count - 1; i++)
      {
        ComboBox lst = (ComboBox)this.FindName("lst_" + i.ToString());
        if (lst.SelectedIndex == 0)
        {
          MessageBox.Show("Selezionare " + colonne[i]);
          return;
        }
      }

      GetDataFromExcel(true);
      CalculatefinalTable();
      CreateFinal();
    }

    void CalculatefinalTable()
    {
      stpFinal.Visibility = Visibility.Visible;

      FinalData = null;

      FinalData = new DataSet();

      System.Data.DataTable dataTable = new System.Data.DataTable();
      dataTable.TableName = "dataTable";

      dataTable.Columns.Add("Codice");
      dataTable.Columns.Add("Descrizione");
      dataTable.Columns.Add("Scarico(Qta)");
      dataTable.Columns.Add("Inventario(Qta)");
      dataTable.Columns.Add("Valore");
      dataTable.Columns.Add("IR");
      dataTable.Columns.Add("IR in mesi", typeof(int));
      dataTable.Columns.Add("Selezione", typeof(bool));

      if (RawData != null)
      {
        for (int i = 0; i < RawData.Tables[0].Rows.Count; i++)
        {
          List<object> tmparray = new List<object>();

          tmparray.Add(RawData.Tables[0].Rows[i][0].ToString()); //Codice
          tmparray.Add(RawData.Tables[0].Rows[i][1].ToString()); //Descrizione
          tmparray.Add(RawData.Tables[0].Rows[i][4].ToString()); //Scarico
          tmparray.Add(RawData.Tables[0].Rows[i][5].ToString()); //Inventario

          double scarico = 0;
          double.TryParse(RawData.Tables[0].Rows[i][4].ToString(), out scarico);

          double inventario = 0;
          double.TryParse(RawData.Tables[0].Rows[i][5].ToString(), out inventario);

          double TR = 0;
          if (scarico != 0)
          {
            TR = inventario / scarico;
          }

          if (TR < 0)
          {
            TR = TR * -1;
          }

          tmparray.Add(RawData.Tables[0].Rows[i][6].ToString());

          tmparray.Add(ConvertNumber(TR.ToString()));

          tmparray.Add(Math.Ceiling(TR * 12));

          tmparray.Add(false);

          dataTable.Rows.Add(tmparray.ToArray());
        }
      }

      FinalData.Tables.Add(dataTable);

      string result;
      using (StringWriter sw = new StringWriter())
      {
        FinalData.WriteXml(sw);
        result = sw.ToString();
      }

      if (node != null)
      {
        if (node.Attributes["TassoRotazione"] == null)
        {
          XmlAttribute attr = node.OwnerDocument.CreateAttribute("TassoRotazione");
          node.Attributes.Append(attr);
        }

        node.Attributes["TassoRotazione"].Value = result;
      }

       ((WindowWorkArea)(this.Owner))._x.Save();
      CreateFinal();
    }
    #endregion

    #region Riga Intestazione

    private void Lst_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      rowintestazione = Convert.ToInt32(((string)(((ComboBox)sender).SelectedItem)));

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

      for (int i = 0; i < colonne.Count; i++)
      {
        ComboBox lsthere = (ComboBox)this.FindName("lst_" + i.ToString());
        lsthere.Items.Clear();
        lsthere.Items.Add(item);
      }

      for (int i = 1; i <= 100; i++)
      {
        string valuehere = "";

        try
        {
          ExcelRange objRange = null;
          objRange = excelSheet.Cells[rowintestazione, i];

          if (objRange.Merge)
          {
            valuehere = Convert.ToString((excelSheet.Cells[1, 1]).Text).Trim();
          }
          else
          {
            valuehere = Convert.ToString(objRange.Text).Trim();
          }

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

        for (int jj = 0; jj < colonne.Count; jj++)
        {
          ComboBox lsthere = (ComboBox)this.FindName("lst_" + jj.ToString());
          lsthere.Items.Add(item);
        }

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
    }

    #endregion

    #region Scelta Colonne

    private void Lst_Colonna_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
    }
    #endregion

    #region final

    private void obj_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if ((((WindowWorkArea)(this.Owner)).ReadOnly))
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }
    }

    private void obj_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if ((((WindowWorkArea)(this.Owner)).ReadOnly))
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }
    }

    private void GestoreEvento_DatiCambiati(object sender, RoutedEventArgs e)
    {

    }

    private void CreateFinal()
    {
      ScrollFinal.Height = 450.0;

      labelTitoloAdditive.Visibility = Visibility.Collapsed;

      Motivazioni.Visibility = Visibility.Collapsed;

      ButtonNext.Visibility = Visibility.Visible;
      ButtonBack.Visibility = Visibility.Collapsed;
      ButtonDatiFinal.Visibility = Visibility.Collapsed;

      stpButtons.Visibility = Visibility.Visible;
      stpFinal.Visibility = Visibility.Visible;

      grdFinalHeader.Children.Clear();
      grdFinalHeader.ColumnDefinitions.Clear();
      grdFinalHeader.RowDefinitions.Clear();
      grdFinalHeader.Width = 1070;
      grdFinalHeader.MaxWidth = 1070;
      grdFinalHeader.MinWidth = 1070;
      grdFinal.Children.Clear();
      grdFinal.ColumnDefinitions.Clear();
      grdFinal.RowDefinitions.Clear();
      grdFinal.Width = 1070;
      grdFinal.MaxWidth = 1070;
      grdFinal.MinWidth = 1070;

      ColumnDefinition cd;

      int real_i = -1;

      DataView dv = FinalData.Tables[0].DefaultView;
      dv.Sort = "[IR in mesi] DESC";

      for (int i = 0; i < dv.ToTable().Columns.Count; i++)
      {
        real_i++;

        cd = new ColumnDefinition();
        if (i == 1)
        {
          cd.Width = new GridLength(6, GridUnitType.Star);
        }
        else
        {
          cd.Width = new GridLength(2, GridUnitType.Star);
        }

        grdFinalHeader.ColumnDefinitions.Add(cd);

        cd = new ColumnDefinition();
        if (i == 1)
        {
          cd.Width = new GridLength(6, GridUnitType.Star);
        }
        else
        {
          cd.Width = new GridLength(2, GridUnitType.Star);
        }

        grdFinal.ColumnDefinitions.Add(cd);
      }

      /*HEADERS*/
      int row = 0;
      RowDefinition rd;
      System.Windows.Controls.Border brd;
      TextBlock lbl;

      real_i = -1;

#pragma warning disable CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato
      double totalErroriRilavati = 0;
#pragma warning restore CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato

      for (int i = 0; i < dv.ToTable().Columns.Count; i++)
      {
        rd = new RowDefinition();
        grdFinalHeader.RowDefinitions.Add(rd);

        real_i++;

        brd = new System.Windows.Controls.Border();
        brd.BorderThickness = new Thickness(1.0);
        brd.BorderBrush = Brushes.LightGray;
        brd.Background = Brushes.LightGray;
        brd.Padding = new Thickness(2.0);

        lbl = new TextBlock();
        lbl.Text = dv.ToTable().Columns[i].ColumnName;
        lbl.TextAlignment = TextAlignment.Center;
        lbl.TextWrapping = TextWrapping.Wrap;
        lbl.FontWeight = FontWeights.Bold;

        brd.Child = lbl;

        grdFinalHeader.Children.Add(brd);
        Grid.SetRow(brd, row);
        Grid.SetColumn(brd, real_i);
      }

      for (int j = 0; j < dv.ToTable().Rows.Count; j++)
      {
        real_i = -1;

        for (int i = 0; i < dv.ToTable().Columns.Count; i++)
        {
          rd = new RowDefinition();
          grdFinal.RowDefinitions.Add(rd);

          real_i++;

          brd = new System.Windows.Controls.Border();
          brd.BorderThickness = new Thickness(1.0);
          brd.BorderBrush = Brushes.LightGray;
          brd.Padding = new Thickness(2.0);

          if (i == dv.ToTable().Columns.Count - 1)
          {
            System.Windows.Controls.CheckBox chk = new System.Windows.Controls.CheckBox();
            chk.HorizontalAlignment = HorizontalAlignment.Center;
            chk.Tag = dv.ToTable().Rows[j][0].ToString();
            chk.Checked += Chk_Checked;
            chk.Unchecked += Chk_Unchecked;
            if (node.Attributes["TassoRotazioneSelected"] != null && node.Attributes["TassoRotazioneSelected"].Value != "" && node.Attributes["TassoRotazioneSelected"].Value.Split('|').Contains(dv.ToTable().Rows[j][0].ToString()))
            {
              chk.IsChecked = true;
            }
            else
            {
              chk.IsChecked = false;
            }

            brd.Child = chk;
          }
          else
          {
            lbl = new TextBlock();
            lbl.Text = dv.ToTable().Rows[j][i].ToString();
            if (i == 1)
            {
              lbl.TextAlignment = TextAlignment.Left;
            }
            else if (i == 2 || i == 3)
            {
              lbl.TextAlignment = TextAlignment.Right;
            }
            else
            {
              lbl.TextAlignment = TextAlignment.Center;
            }
            lbl.TextWrapping = TextWrapping.Wrap;

            brd.Child = lbl;
          }

          grdFinal.Children.Add(brd);
          Grid.SetRow(brd, row);
          Grid.SetColumn(brd, real_i);
        }

        row++;
      }

    }

    private void Chk_Unchecked(object sender, RoutedEventArgs e)
    {
      System.Windows.Controls.CheckBox chk = (System.Windows.Controls.CheckBox)(sender);

      if (node.Attributes["TassoRotazioneSelected"] == null)
      {
        XmlAttribute attr = node.OwnerDocument.CreateAttribute("TassoRotazioneSelected");
        attr.Value = "";
        node.Attributes.Append(attr);
      }

      node.Attributes["TassoRotazioneSelected"].Value = node.Attributes["TassoRotazioneSelected"].Value.Replace(chk.Tag.ToString() + "|", "");
      node.Attributes["TassoRotazioneSelected"].Value = node.Attributes["TassoRotazioneSelected"].Value.Replace("|" + chk.Tag.ToString(), "");
      node.Attributes["TassoRotazioneSelected"].Value = node.Attributes["TassoRotazioneSelected"].Value.Replace(chk.Tag.ToString(), "");

      ((WindowWorkArea)(this.Owner))._x.Save();

    }

    private void Chk_Checked(object sender, RoutedEventArgs e)
    {
      System.Windows.Controls.CheckBox chk = (System.Windows.Controls.CheckBox)(sender);

      if (node.Attributes["TassoRotazioneSelected"] == null)
      {
        XmlAttribute attr = node.OwnerDocument.CreateAttribute("TassoRotazioneSelected");
        attr.Value = "";
        node.Attributes.Append(attr);
      }

      node.Attributes["TassoRotazioneSelected"].Value += ((node.Attributes["TassoRotazioneSelected"].Value == "") ? "" : "|") + chk.Tag.ToString();
      ((WindowWorkArea)(this.Owner))._x.Save();
    }

    private void CreateFinalFinal()
    {
      try
      {
        MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(""));
        this.mainRTB.Selection.Load(stream, DataFormats.Rtf);

        TextRange tr = new TextRange(mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);
        MemoryStream ms = new MemoryStream();
        tr.Save(ms, DataFormats.Text);
        txtMotivazione.Text = ASCIIEncoding.Default.GetString(ms.ToArray());
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        txtMotivazione.Text = "";
      }

      this.mainRTB.Selection.ApplyPropertyValue(FlowDocument.TextAlignmentProperty, TextAlignment.Justify);

      if (node.Attributes["TassoRotazioneMotivazione"] != null)
      {
        try
        {
          MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(node.Attributes["TassoRotazioneMotivazione"].Value));
          this.mainRTB.Selection.Load(stream, DataFormats.Rtf);

          TextRange tr = new TextRange(mainRTB.Document.ContentStart,
                     mainRTB.Document.ContentEnd);
          MemoryStream ms = new MemoryStream();
          tr.Save(ms, DataFormats.Text);
          txtMotivazione.Text = ASCIIEncoding.Default.GetString(ms.ToArray());
        }
        catch (Exception ex)
        {
          string log = ex.Message;
          txtMotivazione.Text = "";
        }
      }
      else
      {
        txtMotivazione.Text = "";
      }

      mainRTB.Focus();
      mainRTB.CaretPosition = mainRTB.Document.ContentEnd;
      mainRTB.ScrollToEnd();

      ScrollFinal.Height = 250.0;

      labelTitoloAdditive.Visibility = Visibility.Visible;
      Motivazioni.Visibility = Visibility.Visible;

      if (node == null || node.Attributes["TassoRotazioneSelected"] == null)
      {
        return;
      }

      ButtonBack.Visibility = Visibility.Visible;
      ButtonNext.Visibility = Visibility.Collapsed;
      ButtonDatiFinal.Visibility = Visibility.Visible;

      stpButtons.Visibility = Visibility.Visible;
      stpFinal.Visibility = Visibility.Visible;

      grdFinalHeader.Children.Clear();
      grdFinalHeader.ColumnDefinitions.Clear();
      grdFinalHeader.RowDefinitions.Clear();
      grdFinalHeader.Width = 1070;
      grdFinalHeader.MaxWidth = 1070;
      grdFinalHeader.MinWidth = 1070;
      grdFinal.Children.Clear();
      grdFinal.ColumnDefinitions.Clear();
      grdFinal.RowDefinitions.Clear();
      grdFinal.Width = 1070;
      grdFinal.MaxWidth = 1070;
      grdFinal.MinWidth = 1070;

      ColumnDefinition cd;

      int real_i = -1;

      DataView dv = FinalData.Tables[0].DefaultView;
      dv.Sort = "[IR in mesi] DESC";

      for (int i = 0; i < dv.ToTable().Columns.Count - 1; i++)
      {
        real_i++;

        cd = new ColumnDefinition();
        if (i == 1)
        {
          cd.Width = new GridLength(6, GridUnitType.Star);
        }
        else
        {
          cd.Width = new GridLength(2, GridUnitType.Star);
        }

        grdFinalHeader.ColumnDefinitions.Add(cd);

        cd = new ColumnDefinition();
        if (i == 1)
        {
          cd.Width = new GridLength(6, GridUnitType.Star);
        }
        else
        {
          cd.Width = new GridLength(2, GridUnitType.Star);
        }

        grdFinal.ColumnDefinitions.Add(cd);
      }

      /*HEADERS*/
      int row = 0;
      RowDefinition rd;
      System.Windows.Controls.Border brd;
      TextBlock lbl;

      real_i = -1;

#pragma warning disable CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato
      double totalErroriRilavati = 0;
#pragma warning restore CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato

      for (int i = 0; i < dv.ToTable().Columns.Count - 1; i++)
      {
        rd = new RowDefinition();
        grdFinalHeader.RowDefinitions.Add(rd);

        real_i++;

        brd = new System.Windows.Controls.Border();
        brd.BorderThickness = new Thickness(1.0);
        brd.BorderBrush = Brushes.LightGray;
        brd.Background = Brushes.LightGray;
        brd.Padding = new Thickness(2.0);

        lbl = new TextBlock();
        lbl.Text = dv.ToTable().Columns[i].ColumnName;
        lbl.TextAlignment = TextAlignment.Center;
        lbl.TextWrapping = TextWrapping.Wrap;
        lbl.FontWeight = FontWeights.Bold;

        brd.Child = lbl;

        grdFinalHeader.Children.Add(brd);
        Grid.SetRow(brd, row);
        Grid.SetColumn(brd, real_i);
      }

      double TotaleValoreSelezionato = 0;

      for (int j = 0; j < dv.ToTable().Rows.Count; j++)
      {
        if (node.Attributes["TassoRotazioneSelected"] == null || node.Attributes["TassoRotazioneSelected"].Value == "" || !node.Attributes["TassoRotazioneSelected"].Value.Split('|').Contains(dv.ToTable().Rows[j][0].ToString()))
        {
          continue;
        }

        real_i = -1;

        for (int i = 0; i < dv.ToTable().Columns.Count - 1; i++)
        {


          rd = new RowDefinition();
          grdFinal.RowDefinitions.Add(rd);

          real_i++;

          brd = new System.Windows.Controls.Border();
          brd.BorderThickness = new Thickness(1.0);
          brd.BorderBrush = Brushes.LightGray;
          brd.Padding = new Thickness(2.0);

          if (i == dv.ToTable().Columns.Count - 1)
          {
            System.Windows.Controls.CheckBox chk = new System.Windows.Controls.CheckBox();
            if (dv.ToTable().Rows[j][i].ToString() == "True")
            {
              chk.IsChecked = true;
            }
            else
            {
              chk.IsChecked = false;
            }

            brd.Child = chk;
          }
          else
          {
            lbl = new TextBlock();
            lbl.Text = dv.ToTable().Rows[j][i].ToString();

            if (i == 4)
            {
              double trydouble = 0.0;

              double.TryParse(dv.ToTable().Rows[j][i].ToString(), out trydouble);
              TotaleValoreSelezionato += trydouble;
            }


            if (i == 1)
            {
              lbl.TextAlignment = TextAlignment.Left;
            }
            else if (i == 2 || i == 3 || i == 4)
            {
              lbl.TextAlignment = TextAlignment.Right;
            }
            else
            {
              lbl.TextAlignment = TextAlignment.Center;
            }
            lbl.TextWrapping = TextWrapping.Wrap;

            brd.Child = lbl;
          }

          grdFinal.Children.Add(brd);
          Grid.SetRow(brd, row);
          Grid.SetColumn(brd, real_i);
        }

        row++;
      }

      labelTitoloAdditive.Content = "Totale valore ITEM selezionati: " + ConvertNumber(TotaleValoreSelezionato.ToString());

      if (node.Attributes["TassoRotazione_labelTitoloAdditive"] == null)
      {
        XmlAttribute attr = node.OwnerDocument.CreateAttribute("TassoRotazione_labelTitoloAdditive");
        node.Attributes.Append(attr);
      }

      node.Attributes["TassoRotazione_labelTitoloAdditive"].Value = "Totale valore ITEM selezionati: " + ConvertNumber(TotaleValoreSelezionato.ToString());

      ((WindowWorkArea)(this.Owner))._x.Save();
    }
    #endregion

    #region ALTRO
    private void GetDataFromExcel(bool erase)
    {
      esistealmenounavoce = false;

      int firstrow = rowintestazione + 1;

      if (erase)
      {
        RawData = null;
      }

      ExcelRange objRange = null;

      if (RawData == null)
      {
        RawData = new DataSet();

        System.Data.DataTable dataTable = new System.Data.DataTable();
        dataTable.TableName = "dataTable";

        for (int i = 0; i < colonne.Count; i++)
        {
          dataTable.Columns.Add(colonne[i]);
        }

        for (int i = firstrow; i <= lastRowIncludeFormulas; i++)
        {
          List<string> tmparray = new List<string>();

          if (excelSheet != null)
          {
            esistealmenounavoce = true;

            for (int j = 0; j < colonne.Count; j++)
            {
              ComboBox lst = (ComboBox)this.FindName("lst_" + j.ToString());

              if (lst == null || lst.SelectedItem == null)
              {
                tmparray.Add("");
              }
              else
              {
                try
                {
                  objRange = excelSheet.Cells[i, int.Parse(((ComboboxItem)(lst.SelectedItem)).Value.ToString())];

                  if (objRange.Merge)
                  {
                    tmparray.Add(Convert.ToString((excelSheet.Cells[1, 1]).Text).Trim().Replace("\"", "").Replace("&", "").Replace("<", "").Replace(">", ""));
                  }
                  else
                  {
                    tmparray.Add(Convert.ToString(objRange.Text).Trim().Replace("\"", "").Replace("&", "").Replace("<", "").Replace(">", ""));
                  }
                }
                catch (Exception ex)
                {
                  string log = ex.Message;
                  tmparray.Add("");
                }

              }
            }

            dataTable.Rows.Add(tmparray.ToArray());
          }
        }

        RawData.Tables.Add(dataTable);

        string result;
        using (StringWriter sw = new StringWriter())
        {
          RawData.WriteXml(sw);
          result = sw.ToString();
        }

        if (node != null)
        {
          if (node.Attributes["RawData"] == null)
          {
            XmlAttribute attr = node.OwnerDocument.CreateAttribute("RawData");
            node.Attributes.Append(attr);
          }

          node.Attributes["RawData"].Value = result;

          ComboBox lst_Intestazione = (ComboBox)this.FindName("lst_Intestazione");

          if (lst_Intestazione != null)
          {
            if (node.Attributes["lst_Intestazione"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("lst_Intestazione");
              node.Attributes.Append(attr);
            }

            node.Attributes["lst_Intestazione"].Value = lst_Intestazione.SelectedValue.ToString();
          }

          for (int i = 0; i < colonne.Count; i++)
          {
            ComboBox lsthere = (ComboBox)this.FindName("lst_" + i.ToString());

            if (lsthere != null && lsthere.SelectedItem != null)
            {
              if (node.Attributes["lst_" + i.ToString()] == null)
              {
                XmlAttribute attr = node.OwnerDocument.CreateAttribute("lst_" + i.ToString());
                node.Attributes.Append(attr);
              }

              node.Attributes["lst_" + i.ToString()].Value = lsthere.SelectedValue.ToString();
            }
          }

          ComboBox lst_attr = (ComboBox)this.FindName("lst_attr");

          if (lst_attr != null && lst_attr.SelectedValue != null)
          {
            if (node.Attributes["lst_attr"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("lst_attr");
              node.Attributes.Append(attr);
            }

            node.Attributes["lst_attr"].Value = lst_attr.SelectedValue.ToString();
          }

          TextBox txt_attr = (TextBox)this.FindName("txt_attr");

          if (lst_attr != null)
          {
            if (node.Attributes["txt_attr"] == null)
            {
              XmlAttribute attr = node.OwnerDocument.CreateAttribute("txt_attr");
              node.Attributes.Append(attr);
            }

            node.Attributes["txt_attr"].Value = txt_attr.Text;
          }
        }
      }

        ((WindowWorkArea)(this.Owner))._x.Save();
    }

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

    private string ConvertNumber(string valore)
    {
      double dblValore = 0.0;

      double.TryParse(valore, out dblValore);

      if (dblValore == 0.0)
      {
        return "0,00";
      }
      else
      {
        return String.Format("{0:#,#.00}", dblValore);
      }
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
        return "0";
      }
      else
      {
        return String.Format("{0:#,0}", Math.Round(dblValore));
      }
    }
    #endregion

    private void ButtonDatiFinal_Click(object sender, RoutedEventArgs e)
    {
      if (FinalData == null)
      {
        return;
      }

      if (node.Attributes["TassoRotazioneMotivazione"] == null)
      {
        XmlAttribute attr = node.OwnerDocument.CreateAttribute("TassoRotazioneMotivazione");
        attr.Value = "";
        node.Attributes.Append(attr);
      }


      try
      {
        //MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(node.Attributes["TassoRotazioneMotivazione"].Value));
        //this.mainRTB.Selection.Load(stream, DataFormats.Rtf);

        TextRange tr = new TextRange(mainRTB.Document.ContentStart,
                   mainRTB.Document.ContentEnd);
        MemoryStream ms = new MemoryStream();
        tr.Save(ms, DataFormats.Text);
        txtMotivazione.Text = ASCIIEncoding.Default.GetString(ms.ToArray());
      }
      catch (Exception ex)
      {
        string log = ex.Message;
        txtMotivazione.Text = "";
      }

      node.Attributes["TassoRotazioneMotivazione"].Value = txtMotivazione.Text;

      ((WindowWorkArea)(this.Owner))._x.Save();

      string rtf_text = "";
      rtf_text += "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1040\\deflangfe1040\\deftab709";
      rtf_text += "{\\fonttbl{\\f0 Cambria}}";
      rtf_text += "{\\colortbl;\\red0\\green255\\blue255;\\red204\\green204\\blue204;\\red255\\green255\\blue255;\\red230\\green230\\blue230;}";
      rtf_text += "\\viewkind4\\uc1";

      if (node.Attributes["TassoRotazioneMotivazione"] != null)
      {
        rtf_text += "\\b Motivazioni scelta Rotazione Scorte: \\b0  ";

        string test = node.Attributes["TassoRotazioneMotivazione"].Value;
        if (test.Split('\n').Length > 12)
        {
          test = test.Replace(test.Split('\n')[0] + "\n", "");
          test = test.Substring(0, test.Length - 1);
          test = test.Replace("{\\f0 \\li0\\ri0\\sa0\\sb0\\fi0\\ql\\par}", "");
          test = test.Replace("\\fs21 ", "\\fs21\\f0 ");
          test = test.Replace("\\lang", "\\f0\\lang");
          test = test.Replace("\\f3", "");
        }
        else
        {
          test = test.Replace("{\\f0 \\li0\\ri0\\sa0\\sb0\\fi0\\ql\\par}", "").Replace("{\\f0 {\\ltrch }\\li0\\ri0\\sa0\\sb0\\fi0\\ql\\par}", "").Replace("{\\f0 {\\ltrch }\\li0\\ri0\\sa0\\sb0\\fi0\\ql\\par}", "").Replace("{\\f0 \\li0\\ri0\\sa0\\sb0\\fi0\\ql\\par}", "").Replace("{\\f0\\fcharset0 Segoe UI;}", "").Replace("\\f1", "\\f0").Replace("\\f2", "\\f0").Replace("\\f3", "\\f0").Replace("\\f4", "\\f0").Replace("{{\\pntext", "{\\f0{\\pntext").Replace("\\f1", "\\f0").Replace("\\f2", "\\f0").Replace("{\\f0\\fcharset0 Times New Roman;}{\\f0\\fcharset0 Tahoma;}", "{\\f0 Arial;\\f1 Wingdings 2;\\f2 Wingdings;}").Replace("\\f0 Wingdings 2", "\\f1 Wingdings 2").Replace("\\f0 Wingdings", "\\f2 Wingdings");
        }

        test = test.Replace("\\ql", "\\qj");

        while (test.Split('{').Length < test.Split('}').Length)
        {
          test = test.Remove(test.LastIndexOf("}"), 1);
        }

        rtf_text += "\\pard\\keepn\\f0\\qj\\li1440\\ri1440 " + test + "\\line \\par\n";

        rtf_text += " \\line ";
      }

      string inizioriga = "\\trowd\\trpaddl50\\trpaddt15\\trpaddr50\\trpaddb15\\trpaddfl3\\trpaddft3\\trpaddfr3\\trpaddfb3 ";
      string fineriga = "\\row ";
      string colore2 = "\\clcbpat2";
      string colore1 = "\\clcbpat3";
      string inizioriga2 = "\\pard\\intbl\\tx2291";
      string bordi = "\\clbrdrl\\brdrw10\\brdrs\\clbrdrt\\brdrw10\\brdrs\\clbrdrr\\brdrw10\\brdrs\\clbrdrb\\brdrw10\\brdrs"; //\\clpadt100

      string cell1CeR2 = "\\clvertalc\\cellx2500";
      string cell2CeR2 = "\\clvertalc\\cellx3200";
      string cell3CeR2 = "\\clvertalc\\cellx3800";
      string cell4CeR2 = "\\clvertalc\\cellx4600";
      string cell5CeR2 = "\\clvertalc\\cellx5500";
      string cell6CeR2 = "\\clvertalc\\cellx7900";

      rtf_text += inizioriga + "\n" + colore2 + bordi + cell1CeR2 + colore2 + bordi + cell2CeR2 + colore2 + bordi + cell3CeR2 + colore2 + bordi + cell4CeR2 + colore2 + bordi + cell5CeR2 + colore2 + bordi + cell6CeR2 + inizioriga2;

      rtf_text += " \\qc " + "Codice - Descrizione" + "\\cell";

      rtf_text += " \\qc " + "Scarico" + "\\cell";

      rtf_text += " \\qc " + "Inventario" + "\\cell";

      rtf_text += " \\qc " + "Valore" + "\\cell";

      rtf_text += " \\qc " + "IR" + "\\cell";

      rtf_text += " \\qc " + "IR in mesi" + "\\cell";

      rtf_text += fineriga;

      DataView dv = FinalData.Tables[0].DefaultView;
      dv.Sort = "[IR in mesi] DESC";

      for (int row = 0; row < dv.ToTable().Rows.Count; row++)
      {
        if (!node.Attributes["TassoRotazioneSelected"].Value.Split('|').Contains(dv.ToTable().Rows[row][0].ToString()))
        {
          continue;
        }

        rtf_text += inizioriga + "\n" + colore1 + bordi + cell1CeR2 + colore1 + bordi + cell2CeR2 + colore1 + bordi + cell3CeR2 + colore1 + bordi + cell4CeR2 + colore1 + bordi + cell5CeR2 + colore1 + bordi + cell6CeR2 + inizioriga2;

        rtf_text += " \\ql " + dv.ToTable().Rows[row][0].ToString() + " - " + dv.ToTable().Rows[row][1].ToString() + " \\cell";
        rtf_text += " \\qr " + dv.ToTable().Rows[row][2].ToString() + " \\cell";
        rtf_text += " \\qr " + dv.ToTable().Rows[row][3].ToString() + " \\cell";
        rtf_text += " \\qr " + dv.ToTable().Rows[row][4].ToString() + " \\cell";
        rtf_text += " \\qc " + dv.ToTable().Rows[row][5].ToString() + " \\cell";
        rtf_text += " \\qc " + dv.ToTable().Rows[row][6].ToString() + " \\cell";

        rtf_text += fineriga;
      }


      if (node.Attributes["TassoRotazione_labelTitoloAdditive"] != null)
      {
        rtf_text += "\\pard\\keepn\\f0\\qj\\li1440\\ri1440 " + " \\line \\b " + node.Attributes["TassoRotazione_labelTitoloAdditive"].Value + " \\b0 " + "\\line \\par\n";
      }

      rtf_text += "}";

      rtf_text = Convert2RTF(rtf_text);

      string filename = App.AppTempFolder + Guid.NewGuid().ToString();

      TextWriter tw = new StreamWriter(filename + ".rtf");
      tw.Write(rtf_text);
      tw.Close();

      //MM
      cDocNet wrdDoc = new cDocNet();
      wrdDoc.PageSetupPaperSize = "A4";
      wrdDoc.PageSetupOrientation = WdOrientation.wdOrientLandscape;
      wrdDoc.SaveAs(filename + ".pdf", filename + ".rtf", "WdSaveFormat.wdFormatPDF");
      //MM



      FileInfo fi = new FileInfo(filename + ".rtf");
      fi.Delete();

      //System.Diagnostics.Process process = new System.Diagnostics.Process();
      //process.Refresh();
      //process.StartInfo.FileName = filename + ".doc";
      //process.StartInfo.ErrorDialog = false;
      //process.StartInfo.Verb = "open";
      //process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
      //process.Start();

      System.Diagnostics.Process.Start(filename + ".pdf");

      //Process wait - STOP
      //pw.Close();
    }

    public string Convert2RTFChar(string carattere)
    {
      string newChar = "";

      switch (carattere)
      {
        //case "!":
        //    newChar = "\\'21";
        //    break;
        case "\"":
          newChar = "\\'22";
          break;
        //case "#":
        //    newChar = "\\'23";
        //    break;
        case "$":
          newChar = "\\'24";
          break;
        case "%":
          newChar = "\\'25";
          break;
        case "&":
          newChar = "\\'26";
          break;
        case "'":
          newChar = "\\'27";
          break;
        //case "(":
        //    newChar = "\\'28";
        //    break;
        //case ")":
        //    newChar = "\\'29";
        //    break;
        //case "*":
        //    newChar = "\\'2a";
        //    break;
        //case "+":
        //    newChar = "\\'2b";
        //    break;
        //case ",":
        //    newChar = "\\'2c";
        //    break;
        //case "-":
        //    newChar = "\\'2d";
        //    break;
        //case ".":
        //    newChar = "\\'2e";
        //    break;
        //case "/":
        //    newChar = "\\'2f";
        //    break;
        //case ":":
        //    newChar = "\\'3a";
        //    break;
        //case ";":
        //    newChar = "\\'3b";
        //    break;
        //case "<":
        //    newChar = "\\'3c";
        //    break;
        //case "=":
        //    newChar = "\\'3d";
        //    break;
        //case ">":
        //    newChar = "\\'3e";
        //    break;
        //case "?":
        //    newChar = "\\'3f";
        //    break;
        //case "@":
        //    newChar = "\\'40";
        //    break;
        //case "[":
        //    newChar = "\\'5b";
        //    break;
        //case "\\":
        //    newChar = "\\'5c";
        //    break;
        //case "]":
        //    newChar = "\\'5d";
        //    break;
        //case "^":
        //    newChar = "\\'5e";
        //    break;
        //case "_":
        //    newChar = "\\'5f";
        //    break;
        //case "`":
        //    newChar = "\\'60";
        //    break;
        //case "{":
        //    newChar = "\\'7b";
        //    break;
        //case "|":
        //    newChar = "\\'7c";
        //    break;
        //case "}":
        //    newChar = "\\'7d";
        //    break;
        //case "~":
        //    newChar = "\\'7e";
        //    break;
        case "€":
          newChar = "\\'80";
          break;
        //case "͵":
        //    newChar = "\\'82";
        //    break;
        //case "ƒ":
        //    newChar = "\\'83";
        //    break;
        //case ""
        //    newChar = "\\'84";
        //    break;
        case "…":
          newChar = "\\'85";
          break;
        //case "†":
        //    newChar = "\\'86";
        //case "‡":
        //    newChar = "\\'87";
        //    break;
        case "∘":
          newChar = "\\'88";
          break;
        //case "‰":
        //    newChar = "\\'89";
        //    break;
        //case "Š":
        //    newChar = "\\'8a";
        //    break;
        //case "‹":
        //    newChar = "\\'8b";
        //    break;
        //case "Œ":
        //    newChar = "\\'8c";
        //    break;
        //case "Ž":
        //    newChar = "\\'8e";
        //    break;
        //case "‘":
        //    newChar = "\\'91";
        //    break;
        case "’":
          newChar = "\\'92";
          break;
        case "“":
          newChar = "\\'93";
          break;
        case "”":
          newChar = "\\'94";
          break;
        //case "•":
        //    newChar = "\\'95";
        //    break;
        //case "–":
        //    newChar = "\\'96";
        //    break;
        //case "—":
        //    newChar = "\\'97";
        //    break;
        //case "~":
        //    newChar = "\\'98";
        //    break;
        //case "™":
        //    newChar = "\\'99";
        //    break;
        //case "š":
        //    newChar = "\\'9a";
        //    break;
        //case "›":
        //    newChar = "\\'9b";
        //    break;
        //case "œ":
        //    newChar = "\\'9c";
        //    break;
        //case "ž":
        //    newChar = "\\'9e";
        //    break;
        //case "Ÿ":
        //    newChar = "\\'9f";
        //    break;
        //case "¡":
        //    newChar = "\\'a1";
        //    break;
        //case "¢":
        //    newChar = "\\'a2";
        //    break;
        //case "£":
        //    newChar = "\\'a3";
        //    break;
        //case "¤":
        //    newChar = "\\'a4";
        //    break;
        //case "¥":
        //    newChar = "\\'a5";
        //    break;
        //case "¦":
        //    newChar = "\\'a6";
        //    break;
        //case "§":
        //    newChar = "\\'a7";
        //    break;
        //case "¨":
        //    newChar = "\\'a8";
        //    break;
        case "©":
          newChar = "\\'a9";
          break;
        //case "ª":
        //    newChar = "\\'aa";
        //    break;
        //case "«":
        //    newChar = "\\'ab";
        //    break;
        //case "¬":
        //    newChar = "\\'ac";
        //    break;
        //case "®":
        //    newChar = "\\'ae";
        //    break;
        //case "¯":
        //    newChar = "\\'af";
        //    break;
        case "°":
          newChar = "\\'b0";
          break;
        case "±":
          newChar = "\\'b1";
          break;
        case "²":
          newChar = "\\'b2";
          break;
        case "³":
          newChar = "\\'b3";
          break;
        //case "´":
        //    newChar = "\\'b4";
        //    break;
        case "µ":
          newChar = "\\'b5";
          break;
        //case "¶":
        //    newChar = "\\'b6";
        //    break;
        //case "•":
        //  newChar = "\\'b7";
        //break;
        //case "¸":
        //    newChar = "\\'b8";
        //    break;
        //case "¹":
        //    newChar = "\\'b9";
        //    break;
        //case "º":
        //    newChar = "\\'ba";
        //    break;
        //case "»":
        //    newChar = "\\'bb";
        //    break;
        //case "¼":
        //    newChar = "\\'bc";
        //    break;
        //case "½":
        //    newChar = "\\'bd";
        //    break;
        //case "¾":
        //    newChar = "\\'be";
        //    break;
        //case "¿":
        //    newChar = "\\'bf";
        //    break;
        case "À":
          newChar = "\\'c0";
          break;
        case "Á":
          newChar = "\\'c1";
          break;
        case "Â":
          newChar = "\\'c2";
          break;
        case "Ã":
          newChar = "\\'c3";
          break;
        case "Ä":
          newChar = "\\'c4";
          break;
        case "Å":
          newChar = "\\'c5";
          break;
        case "Æ":
          newChar = "\\'c6";
          break;
        case "Ç":
          newChar = "\\'c7";
          break;
        case "È":
          newChar = "\\'c8";
          break;
        case "É":
          newChar = "\\'c9";
          break;
        case "Ê":
          newChar = "\\'ca";
          break;
        case "Ë":
          newChar = "\\'cb";
          break;
        case "Ì":
          newChar = "\\'cc";
          break;
        case "Í":
          newChar = "\\'cd";
          break;
        case "Î":
          newChar = "\\'ce";
          break;
        case "Ï":
          newChar = "\\'cf";
          break;
        case "Ð":
          newChar = "\\'d0";
          break;
        case "Ñ":
          newChar = "\\'d1";
          break;
        case "Ò":
          newChar = "\\'d2";
          break;
        case "Ó":
          newChar = "\\'d3";
          break;
        case "Ô":
          newChar = "\\'d4";
          break;
        case "Õ":
          newChar = "\\'d5";
          break;
        case "Ö":
          newChar = "\\'d6";
          break;
        //case "×":
        //    newChar = "\\'d7";
        //    break;
        case "Ø":
          newChar = "\\'d8";
          break;
        case "Ù":
          newChar = "\\'d9";
          break;
        case "Ú":
          newChar = "\\'da";
          break;
        case "Û":
          newChar = "\\'db";
          break;
        case "Ü":
          newChar = "\\'dc";
          break;
        case "Ý":
          newChar = "\\'dd";
          break;
        case "Þ":
          newChar = "\\'de";
          break;
        case "ß":
          newChar = "\\'df";
          break;
        case "à":
          newChar = "\\'e0";
          break;
        case "á":
          newChar = "\\'e1";
          break;
        case "â":
          newChar = "\\'e2";
          break;
        case "ã":
          newChar = "\\'e3";
          break;
        case "ä":
          newChar = "\\'e4";
          break;
        case "å":
          newChar = "\\'e5";
          break;
        case "æ":
          newChar = "\\'e6";
          break;
        case "ç":
          newChar = "\\'e7";
          break;
        case "è":
          newChar = "\\'e8";
          break;
        case "é":
          newChar = "\\'e9";
          break;
        case "ê":
          newChar = "\\'ea";
          break;
        case "ë":
          newChar = "\\'eb";
          break;
        case "ì":
          newChar = "\\'ec";
          break;
        case "í":
          newChar = "\\'ed";
          break;
        case "î":
          newChar = "\\'ee";
          break;
        case "ï":
          newChar = "\\'ef";
          break;
        case "ð":
          newChar = "\\'f0";
          break;
        case "ñ":
          newChar = "\\'f1";
          break;
        case "ò":
          newChar = "\\'f2";
          break;
        case "ó":
          newChar = "\\'f3";
          break;
        case "ô":
          newChar = "\\'f4";
          break;
        case "õ":
          newChar = "\\'f5";
          break;
        case "ö":
          newChar = "\\'f6";
          break;
        case "÷":
          newChar = "\\'f7";
          break;
        case "ø":
          newChar = "\\'f8";
          break;
        case "ù":
          newChar = "\\'f9";
          break;
        case "ú":
          newChar = "\\'fa";
          break;
        case "û":
          newChar = "\\'fb";
          break;
        case "ü":
          newChar = "\\'fc";
          break;
        case "ý":
          newChar = "\\'fd";
          break;
        case "þ":
          newChar = "\\'fe";
          break;
        case "ÿ":
          newChar = "\\'ff";
          break;
      }

      return newChar;
    }

    public string Convert2RTFString(string buff, string replaceChar)
    {
      return buff.Replace(replaceChar, Convert2RTFChar(replaceChar));
    }

    private string Convert2RTF(string buff)
    {
      buff = buff.Replace("\\'", "\\#");
      buff = Convert2RTFString(buff, "'"); //va messo per primo o causa problemi
      buff = buff.Replace("\\#", "\\'");

      //for (char c = '!'; c <= 'ÿ'; c++)
      //{
      //    buff = Convert2RTFString(buff, c.ToString() );
      //}

      buff = Convert2RTFString(buff, "%");
      buff = Convert2RTFString(buff, "ì");
      buff = Convert2RTFString(buff, "è");
      buff = Convert2RTFString(buff, "é");
      buff = Convert2RTFString(buff, "ò");
      buff = Convert2RTFString(buff, "à");
      buff = Convert2RTFString(buff, "ù");
      buff = Convert2RTFString(buff, "°");
      buff = Convert2RTFString(buff, "€");
      buff = Convert2RTFString(buff, "\"");
      buff = Convert2RTFString(buff, "’");
      buff = Convert2RTFString(buff, "”");
      buff = Convert2RTFString(buff, "“");

      return buff;
    }

    private void Btn_Esci_Click(object sender, RoutedEventArgs e)
    {
      if (node.Attributes["TassoRotazioneMotivazione"] == null)
      {
        XmlAttribute attr = node.OwnerDocument.CreateAttribute("TassoRotazioneMotivazione");
        attr.Value = "";
        node.Attributes.Append(attr);
      }

      node.Attributes["TassoRotazioneMotivazione"].Value = txtMotivazione.Text;

      ((WindowWorkArea)(this.Owner))._x.Save();

      this.Close();
    }

    private void GestoreEvento_DatiCambiati(object sender, TextChangedEventArgs e)
    {
      if (node.Attributes["TassoRotazioneMotivazione"] == null)
      {
        XmlAttribute attr = node.OwnerDocument.CreateAttribute("TassoRotazioneMotivazione");
        attr.Value = "";
        node.Attributes.Append(attr);
      }

      node.Attributes["TassoRotazioneMotivazione"].Value = txtMotivazione.Text;

      ((WindowWorkArea)(this.Owner))._x.Save();
    }

    //private void ButtonDeleteRaw_Click(object sender, RoutedEventArgs e)
    //{
    //    if (node != null && node.Attributes["RawData"] != null)
    //    {
    //        node.Attributes.Remove(node.Attributes["RawData"]);
    //        RawData = null;
    //    }

    //    if (node != null && node.Attributes["TassoRotazione"] != null)
    //    {
    //        node.Attributes.Remove(node.Attributes["TassoRotazione"]);
    //        FinalData = null;
    //    }

    //    ((WindowWorkArea)(this.Owner))._x.Save();
    //    Load();
    //}
  }
}
