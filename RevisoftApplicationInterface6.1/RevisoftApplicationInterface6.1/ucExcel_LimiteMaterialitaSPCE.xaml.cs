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
  public enum IpotesiMaterialita { Prima, Seconda, Terza };


  public partial class ucExcel_LimiteMaterialitaSPCE : UserControl
  {
    public int id;
    private DataTable dati = null;
    private DataTable datiCONSOLIDATO = null;


    private string down = "./Images/icone/navigate_down.png";
    private string left = "./Images/icone/navigate_left.png";

    XmlDataProviderManager _x;
    //XmlDataProviderManager _d;
    string _ID;
    string IDTree;
    private string IDB_Padre = "227";
    private string IDBA_Padre = "229";

    Hashtable valoreEA = new Hashtable();

    Hashtable SommeDaExcel = new Hashtable();
    Hashtable ValoriDaExcelEA = new Hashtable();

    bool canbecalculated = false;

    private IpotesiMaterialita _ipotesi = IpotesiMaterialita.Prima;

    private bool _isUpdatingXMLData = false;
    private bool _isUpdatingData = false;


    public ucExcel_LimiteMaterialitaSPCE(string _IDTree)
    {
      InitializeComponent();
      IDTree =_IDTree;
    }

    private bool _ReadOnly = false;

    public bool ReadOnly
    {
      set
      {
        _ReadOnly = value;
      }
    }

    public void CALCULATECONSOLIDATO()
    {




      Grid g = new Grid();

      ColumnDefinition cd = new ColumnDefinition();
      cd.Width = new GridLength(15.0);
      g.ColumnDefinitions.Add(cd);

      cd = new ColumnDefinition();
      cd.Width = GridLength.Auto;
      g.ColumnDefinitions.Add(cd);

      g.RowDefinitions.Add(new RowDefinition());
      g.RowDefinitions.Add(new RowDefinition());

      Image i = new Image();
      i.SetValue(Grid.RowProperty, 0);
      i.SetValue(Grid.ColumnProperty, 0);

      var uriSource = new Uri(left, UriKind.Relative);
      i.Source = new BitmapImage(uriSource);
      i.Height = 10.0;
      i.Width = 10.0;

      g.Children.Add(i);

      TextBlock tb = new TextBlock();
      tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
      tb.Text = "Ripartizione delle Materialità fa le COMPONENTI";

      tb.FontSize = 13;
      tb.FontWeight = FontWeights.Bold;
      tb.Margin = new Thickness(5.0);
      tb.Foreground = Brushes.Gray;

      tb.SetValue(Grid.RowProperty, 0);
      tb.SetValue(Grid.ColumnProperty, 1);

      g.Children.Add(tb);

      StackPanel sp = new StackPanel();
      sp.Orientation = Orientation.Vertical;

      StackPanel spheader = new StackPanel();
      spheader.Orientation = Orientation.Horizontal;

      TextBox t = new TextBox();
      t.Width = 300;
      t.FontWeight = FontWeights.Bold;
      t.TextAlignment = TextAlignment.Center;
      t.IsReadOnly = true;
      t.Background = Brushes.LightGray;
      t.BorderBrush = Brushes.DarkGray;
      t.BorderThickness = new Thickness(1);
      t.Text = "Componenti";
      spheader.Children.Add(t);

      t = new TextBox();
      t.Width = 300;
      t.FontWeight = FontWeights.Bold;
      t.TextAlignment = TextAlignment.Center;
      t.IsReadOnly = true;
      t.Background = Brushes.LightGray;
      t.BorderBrush = Brushes.DarkGray;
      t.BorderThickness = new Thickness(1);
      t.Text = "Materialità Operativa";
      spheader.Children.Add(t);

      t = new TextBox();
      t.Width = 300;
      t.FontWeight = FontWeights.Bold;
      t.TextAlignment = TextAlignment.Center;
      t.IsReadOnly = true;
      t.Background = Brushes.LightGray;
      t.BorderBrush = Brushes.DarkGray;
      t.BorderThickness = new Thickness(1);
      t.Text = "Errore Tollerabile";
      spheader.Children.Add(t);

      sp.Children.Add(spheader);

      int rowhere = 0;

      double totma = 0;
      double totet = 0;
      foreach (DataRow dtrow in datiCONSOLIDATO.Rows)
      {
        if (dtrow["name"].ToString() == "")
        {
          continue;
        }

        DataRow node = null;
        foreach (DataRow dt in dati.Rows)
        {
          if (dt["ID"].ToString() == rowhere.ToString())
          {
            node = dt;
          }
        }

        if (node == null)
          node = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, rowhere.ToString());

        StackPanel sprow = new StackPanel();
        sprow.Orientation = Orientation.Horizontal;

        t = new TextBox();
        t.Width = 300;
        t.IsReadOnly = true;
        t.BorderBrush = Brushes.DarkGray;
        t.BorderThickness = new Thickness(1);
        t.Text = dtrow["name"].ToString();
        sprow.Children.Add(t);


        node["name"] = t.Text;

        t = new TextBox();
        t.Tag = rowhere.ToString();
        t.Width = 300;
        t.TextAlignment = TextAlignment.Right;
        t.BorderBrush = Brushes.DarkGray;
        t.BorderThickness = new Thickness(1);
        t.LostFocus += T_LostFocus;
        t.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDown;
        t.PreviewKeyDown += obj_PreviewKeyDown;
        t.Text = ConvertNumber(((node["ma"].ToString() == "") ? "0" : node["ma" + rowhere.ToString()].ToString()));
        sprow.Children.Add(t);

        node["ma"] = t.Text;
        double tmpd = 0;

        double.TryParse(t.Text, out tmpd);
        totma += tmpd;

        t = new TextBox();
        t.Tag = rowhere.ToString();
        t.Width = 300;
        t.TextAlignment = TextAlignment.Right;
        t.BorderBrush = Brushes.DarkGray;
        t.BorderThickness = new Thickness(1);
        t.LostFocus += T_LostFocus1;
        t.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDown;
        t.PreviewKeyDown += obj_PreviewKeyDown;
        t.Text = ConvertNumber(((node["et"].ToString() == "") ? "0" : node["et"].ToString()));
        sprow.Children.Add(t);

        node["et"] = t.Text;

        tmpd = 0;

        double.TryParse(t.Text, out tmpd);

        totet += tmpd;

        sp.Children.Add(sprow);

        rowhere++;

      }

      StackPanel spfooter = new StackPanel();
      spfooter.Orientation = Orientation.Horizontal;

      t = new TextBox();
      t.Width = 300;
      t.IsReadOnly = true;
      t.BorderBrush = Brushes.DarkGray;
      t.BorderThickness = new Thickness(1);
      t.FontWeight = FontWeights.Bold;
      t.Text = "Totale";
      t.Background = Brushes.LightYellow;
      spfooter.Children.Add(t);

      DataRow nodef = null;
      foreach (DataRow dt in dati.Rows)
      {
        if (dt["ID"].ToString() == rowhere.ToString())
        {
          nodef = dt;
        }
      }

      if (nodef == null)
        nodef = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, rowhere.ToString());


      nodef["name"] = t.Text;

      t = new TextBox();
      t.Width = 300;
      t.TextAlignment = TextAlignment.Right;
      t.IsReadOnly = true;
      t.BorderBrush = Brushes.DarkGray;
      t.BorderThickness = new Thickness(1);
      t.FontWeight = FontWeights.Bold;
      t.Background = Brushes.LightYellow;
      t.Text = ConvertNumber(totma.ToString());
      spfooter.Children.Add(t);

      nodef["ma"] = t.Text;

      t = new TextBox();
      t.Width = 300;
      t.TextAlignment = TextAlignment.Right;
      t.IsReadOnly = true;
      t.BorderBrush = Brushes.DarkGray;
      t.BorderThickness = new Thickness(1);
      t.FontWeight = FontWeights.Bold;
      t.Background = Brushes.LightYellow;
      t.Text = ConvertNumber(totet.ToString());
      spfooter.Children.Add(t);


      nodef["et"] = t.Text;

      sp.Children.Add(spfooter);

      sp.SetValue(Grid.RowProperty, 1);
      sp.SetValue(Grid.ColumnProperty, 1);

      sp.Visibility = System.Windows.Visibility.Visible;
      uriSource = new Uri(left, UriKind.Relative);
      ((Image)(g.Children[0])).Source = new BitmapImage(uriSource);

      g.Children.Add(sp);

      brdCONSOLIDATO.Child = g;

      // _d.Save();
    }

    //----------------------------------------------------------------------------+
    //                                    Load                                    |
    //----------------------------------------------------------------------------+
    public void Load(string ID, string FileDataRevisione, IpotesiMaterialita ipotesi, string IDCliente, string IDSessione)
    {

      id = int.Parse(ID.ToString());
      cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
      cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
      dati = cBusinessObjects.GetData(id, typeof(Excel_LimiteMaterialitaSPCE));
      datiCONSOLIDATO = cBusinessObjects.GetData(313, typeof(Excel_Consolidato));


      _ID = ID;
      _ipotesi = ipotesi;
      switch (ipotesi)
      {
        case IpotesiMaterialita.Prima:
          brdLimiti.Visibility = System.Windows.Visibility.Visible;
          brdDottrina.Visibility = System.Windows.Visibility.Visible;
          brdPrima.Visibility = System.Windows.Visibility.Visible;
          brdSeconda.Visibility = System.Windows.Visibility.Visible;
          brdTerza.Visibility = System.Windows.Visibility.Collapsed;
          brdLimitiBILANCIO.Visibility = System.Windows.Visibility.Visible;
          brdDottrinaBILANCIO.Visibility = System.Windows.Visibility.Visible;
          brdPrimaBILANCIO.Visibility = System.Windows.Visibility.Visible;
          brdSecondaBILANCIO.Visibility = System.Windows.Visibility.Visible;
          brdTerzaBILANCIO.Visibility = System.Windows.Visibility.Collapsed;
          gridContoEcValoreProd.Visibility = System.Windows.Visibility.Collapsed;
          gridContoEcValoreProdBILANCIO.Visibility = Visibility.Collapsed;
          txt1_p_minBILANCIO.IsReadOnly = true;
          txt2_p_minBILANCIO.IsReadOnly = true;
          txt3_p_minBILANCIO.IsReadOnly = true;
          txt4_p_minBILANCIO.IsReadOnly = true;
          txt14_p_minBILANCIO.IsReadOnly = true;
          txtContoEcValoreProdTotaleBILANCIO_p_min.IsReadOnly = true;
          txt1_p_maxBILANCIO.IsReadOnly = true;
          txt2_p_maxBILANCIO.IsReadOnly = true;
          txt3_p_maxBILANCIO.IsReadOnly = true;
          txt4_p_maxBILANCIO.IsReadOnly = true;
          txt14_p_maxBILANCIO.IsReadOnly = true;
          break;
        case IpotesiMaterialita.Seconda:
          brdLimiti.Visibility = System.Windows.Visibility.Visible;
          brdDottrina.Visibility = System.Windows.Visibility.Visible;
          brdPrima.Visibility = System.Windows.Visibility.Collapsed;
          brdSeconda.Visibility = System.Windows.Visibility.Visible;
          brdTerza.Visibility = System.Windows.Visibility.Collapsed;
          brdLimitiBILANCIO.Visibility = System.Windows.Visibility.Visible;
          brdDottrinaBILANCIO.Visibility = System.Windows.Visibility.Visible;
          brdPrimaBILANCIO.Visibility = System.Windows.Visibility.Collapsed;
          brdSecondaBILANCIO.Visibility = System.Windows.Visibility.Visible;
          brdTerzaBILANCIO.Visibility = System.Windows.Visibility.Collapsed;
          gridContoEcValoreProd.Visibility = System.Windows.Visibility.Collapsed;
          gridContoEcValoreProdBILANCIO.Visibility = Visibility.Collapsed;
          brdRadioPrimaPianificata.Visibility = Visibility.Collapsed;
          brdRadioSecondaPianificata.Visibility = Visibility.Hidden;
          brdRadioPrimaBILANCIO.Visibility = Visibility.Collapsed;
          brdRadioSecondaBILANCIO.Visibility = Visibility.Collapsed;
          break;
        case IpotesiMaterialita.Terza:
          brdLimiti.Visibility = System.Windows.Visibility.Visible;
          brdDottrina.Visibility = System.Windows.Visibility.Visible;
          brdPrima.Visibility = System.Windows.Visibility.Visible;
          brdSeconda.Visibility = System.Windows.Visibility.Visible;
          brdTerza.Visibility = System.Windows.Visibility.Collapsed;
          brdLimitiBILANCIO.Visibility = System.Windows.Visibility.Visible;
          brdDottrinaBILANCIO.Visibility = System.Windows.Visibility.Visible;
          brdPrimaBILANCIO.Visibility = System.Windows.Visibility.Visible;
          brdSecondaBILANCIO.Visibility = System.Windows.Visibility.Visible;
          brdTerzaBILANCIO.Visibility = System.Windows.Visibility.Collapsed;
          gridContoEcValoreProd.Visibility = System.Windows.Visibility.Visible;
          gridContoEcValoreProdBILANCIO.Visibility = Visibility.Visible;
          //tabBILANCIO.Visibility = System.Windows.Visibility.Collapsed;
          break;
        default:
          break;
      }
      MasterFile mf = MasterFile.Create();

      DataRow nodeCONSOLIDATO = null;

      if (ID == "314")
      {
        brdCONSOLIDATO.Visibility = Visibility.Visible;
        ((TabItem)(Tabcontrolhere.Items[0])).Header = "Materialità CONSOLIDATO";
        ((TabItem)(Tabcontrolhere.Items[0])).Visibility = Visibility.Collapsed;
        foreach (DataRow dtrow in datiCONSOLIDATO.Rows)
        {
          nodeCONSOLIDATO = dtrow;
        }

        if (nodeCONSOLIDATO != null)
        {
          txt1.Text = nodeCONSOLIDATO["attivo"].ToString();
          txt2.Text = nodeCONSOLIDATO["patrimonionetto"].ToString();
          txt3.Text = nodeCONSOLIDATO["valoreproduzione"].ToString();
          txt4.Text = nodeCONSOLIDATO["risultatoanteimposte"].ToString();
        }
        CALCULATECONSOLIDATO();
        //((TabItem)(Tabcontrolhere.Items[0])).Header = "Materialità TOTALE AGGREGATO";
        //((TabItem)(Tabcontrolhere.Items[1])).Header = "Materialità CONSOLIDATO";
        //XmlNode nodeCONSOLIDATO = d.Document.SelectSingleNode("//Dato[@ID='313']");
        //if (nodeCONSOLIDATO != null)
        //{
        //    txt1BILANCIO.Text = ((nodeCONSOLIDATO.Attributes["attivo"] == null)? "" : nodeCONSOLIDATO.Attributes["attivo"].ToString().ToString());
        //    txt2BILANCIO.Text = ((nodeCONSOLIDATO.Attributes["patrimonionetto"] == null) ? "" : nodeCONSOLIDATO.Attributes["patrimonionetto"].ToString().ToString());
        //    txt3BILANCIO.Text = ((nodeCONSOLIDATO.Attributes["valoreproduzione"] == null) ? "" : nodeCONSOLIDATO.Attributes["valoreproduzione"].ToString().ToString());
        //    txt4BILANCIO.Text = ((nodeCONSOLIDATO.Attributes["risultatoanteimposte"] == null) ? "" : nodeCONSOLIDATO.Attributes["risultatoanteimposte"].ToString().ToString());
        //    txt1.Text = ((nodeCONSOLIDATO.Attributes["attivoTOT"] == null) ? "" : nodeCONSOLIDATO.Attributes["attivoTOT"].ToString().ToString());
        //    txt2.Text = ((nodeCONSOLIDATO.Attributes["patrimonionettoTOT"] == null) ? "" : nodeCONSOLIDATO.Attributes["patrimonionettoTOT"].ToString().ToString());
        //    txt3.Text = ((nodeCONSOLIDATO.Attributes["valoreproduzioneTOT"] == null) ? "" : nodeCONSOLIDATO.Attributes["valoreproduzioneTOT"].ToString().ToString());
        //    txt4.Text = ((nodeCONSOLIDATO.Attributes["risultatoanteimposteTOT"] == null) ? "" : nodeCONSOLIDATO.Attributes["risultatoanteimposteTOT"].ToString().ToString());
        //}
      }
      else
      {
        string FileBilancio = mf.GetBilancioAssociatoFromRevisioneFile(FileDataRevisione);
        //if (FileBilancio != "" && (new FileInfo(FileBilancio)).Exists)
        if (!string.IsNullOrEmpty(FileBilancio))
        {
          _x = new XmlDataProviderManager(FileBilancio);
        }
        else
        {
          _x = null;
        }
        #region Dati da bilancio
        string tipoBilancio = "";
        string idsessionebilancio = "";
                
        if(IDTree=="1")
          idsessionebilancio = cBusinessObjects.CercaSessione("Revisione", "Bilancio", cBusinessObjects.idsessione.ToString(), cBusinessObjects.idcliente);
        else
          idsessionebilancio = cBusinessObjects.CercaSessione("Conclusione", "Bilancio", cBusinessObjects.idsessione.ToString(), cBusinessObjects.idcliente);
  
        DataTable bilanciotestata = cBusinessObjects.GetData(int.Parse(IDB_Padre), typeof(Excel_Bilancio_Testata), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);


        if (bilanciotestata.Rows.Count > 0)
        {
          foreach (DataRow dtrow in bilanciotestata.Rows)
          {
            tipoBilancio = dtrow["tipoBilancio"].ToString();
          }
        }

        switch (tipoBilancio)
        {
          case "2016":
            SommeDaExcel.Clear();
            SommeDaExcel.Add("TotaleAttivitaBILANCIO", "3|4|8|9|10|11|12|13|14|17|18|19|20|21|25|26|27|28|32|33|35|36|38|39|41|42|43|44|51|52|53|54|55|59|60|62|63|65|66|68|69|71|72|73|77|78|81|82|83|84|85|86|90|91|92|98|201655|201627|201638|201639|201677|201678|201651|201683");
            SommeDaExcel.Add("PatrimonionettoBILANCIO", "108|109|110|111|112|113|117|11600|11601|11602|11603|11604|11605|115|11606|11607|116|11608|11609|11610|118|119|120|1160|11700|11701|114|20161131|20161132|20161133|20161134|20161135|20161136|20161137|20161138|20161139|20161140|20161141|20171142|20161142|20161143|2016114|2016998|11611");
            SommeDaExcel.Add("RicaviEsercizioBILANCIO", "189|190|191|192|194|195");
            SommeDaExcel.Add("RisultatoImposteBILANCIO", "247|248|249|251|252|253|2016249|20162491|20162492|20162493|198|199|200|202|203|204|205|206|208|209|210|211|212|213|214|215|189|190|191|192|194|195|222|223|224|2016224|20162241|235|236|237|234|232|231|228|229|230|227|2016237|2016229|240|241|242|239|2016242|243");
            SommeDaExcel.Add("DiffFraValoreECostiProdBILANCIO", "189|190|191|192|194|195|198|199|200|202|203|204|205|206|208|209|210|211|212|213|214|215");
            SommeDaExcel.Add("A1BILANCIO", "189");
            SommeDaExcel.Add("A2BILANCIO", "190");
            SommeDaExcel.Add("A3BILANCIO", "191");
            SommeDaExcel.Add("A4BILANCIO", "192");
            SommeDaExcel.Add("A5BILANCIO", "195");
            SommeDaExcel.Add("A5_2BILANCIO", "193");
            break;
          default:
            SommeDaExcel.Clear();
            SommeDaExcel.Add("TotaleAttivitaBILANCIO", "3|4|8|9|10|11|12|13|14|17|18|19|20|21|25|26|27|28|32|33|35|36|38|39|41|42|43|44|51|52|53|54|55|59|60|62|63|65|66|68|69|71|72|74|75|77|78|81|82|83|84|85|86|90|91|92|98|99"); //102
            SommeDaExcel.Add("PatrimonionettoBILANCIO", "108|109|110|111|112|113|115|116|117|118|119|120"); // Modifica 3.0 Borelli //3|4|8|9|10|11|12|13|14|17|18|19|20|21|25|26|27|28|32|33|35|36|38|39|41|42|43|44|51|52|53|54|55|59|60|62|63|65|66|68|69|71|72|74|75|77|78|81|82|83|84|85|86|90|91|92|98|99|-108|-109|-110|-111|-112|-113|-115|-116|-117|-118|-119|-120|-124|-125|-126|-129|-133|-134|-136|-137|-139|-140|-142|-143|-145|-146|-148|-149|-151|-152|-154|-155|-157|-158|-160|-161|-163|-164|-166|-167|-169|-170|-172|-173|-176|-177
            SommeDaExcel.Add("RicaviEsercizioBILANCIO", "189|190|191|192|194|195");// Modifica 3.0 Borelli //196
            SommeDaExcel.Add("RisultatoImposteBILANCIO", "189|190|191|192|194|195|198|199|200|202|203|204|205|206|208|209|210|211|212|213|214|215|222|223|224|227|228|229|230|231|232|234|235|236|237|239|240|241|242|243|247|248|249|251|252|253|257|258|260|261|262"); //265
            SommeDaExcel.Add("DiffFraValoreECostiProdBILANCIO", "189|190|191|192|194|195|198|199|200|202|203|204|205|206|208|209|210|211|212|213|214|215");
            SommeDaExcel.Add("A1BILANCIO", "189");
            SommeDaExcel.Add("A2BILANCIO", "190");
            SommeDaExcel.Add("A3BILANCIO", "191");
            SommeDaExcel.Add("A4BILANCIO", "192");
            SommeDaExcel.Add("A5BILANCIO", "195");
            SommeDaExcel.Add("A5_2BILANCIO", "193");
            break;
        }
        RetrieveData(IDB_Padre);
        if (valoreEA.Count == 0)
        {
                
        if(IDTree=="1")
          idsessionebilancio = cBusinessObjects.CercaSessione("Revisione", "Bilancio", cBusinessObjects.idsessione.ToString(), cBusinessObjects.idcliente);
        else
          idsessionebilancio = cBusinessObjects.CercaSessione("Conclusione", "Bilancio", cBusinessObjects.idsessione.ToString(), cBusinessObjects.idcliente);
  
          bilanciotestata = cBusinessObjects.GetData(int.Parse(IDBA_Padre), typeof(Excel_Bilancio_Testata), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);

          if (bilanciotestata.Rows.Count > 0)
          {
            foreach (DataRow dtrow in bilanciotestata.Rows)
            {
              tipoBilancio = dtrow["tipoBilancio"].ToString();
            }
          }
          switch (tipoBilancio)
          {
            case "Micro":
              SommeDaExcel.Clear();
              SommeDaExcel.Add("TotaleAttivitaBILANCIO", "2|7|16|1009|50|1059|1060|80|89|201655|98");
              SommeDaExcel.Add("PatrimonionettoBILANCIO", "108|109|110|111|112|100114|119|11611|2016114|2016998");
              SommeDaExcel.Add("RicaviEsercizioBILANCIO", "189|190|191|192|194|195");
              SommeDaExcel.Add("RisultatoImposteBILANCIO", "189|2016190|190|191|192|194|195|212|213|214|215|2016208|208|209|210|211|202|203|204|205|206|2016204|200|199|198|222|223|224|2016224|20162241|235|236|237|234|232|231|228|229|230|227|2016237|2016231|2016229|240|241|242|239|2016242|243|247|248|249|251|252|253|2016249|20162491|20162492|20162493");
              SommeDaExcel.Add("DiffFraValoreECostiProdBILANCIO", "189|2016190|190|191|192|194|195|212|213|214|215|2016208|208|209|210|211|202|203|204|205|206|2016204|200|199|198");
              SommeDaExcel.Add("A1BILANCIO", "189");
              SommeDaExcel.Add("A2BILANCIO", "190");
              SommeDaExcel.Add("A3BILANCIO", "191");
              SommeDaExcel.Add("A4BILANCIO", "192");
              SommeDaExcel.Add("A5BILANCIO", "195");
              SommeDaExcel.Add("A5_2BILANCIO", "193");
              break;
            case "2016":
              SommeDaExcel.Clear();
              SommeDaExcel.Add("TotaleAttivitaBILANCIO", "2|7|16|1009|50|1059|1060|80|89|201655|98");
              SommeDaExcel.Add("PatrimonionettoBILANCIO", "108|109|110|111|114|112|100114|119|11611|2016114|2016998");
              SommeDaExcel.Add("RicaviEsercizioBILANCIO", "189|190|191|192|194|195");
              SommeDaExcel.Add("RisultatoImposteBILANCIO", "189|2016190|190|191|192|194|195|212|213|214|215|2016208|208|209|210|211|202|203|204|205|206|2016204|200|199|198|222|223|224|2016224|20162241|235|236|237|234|232|231|228|229|230|227|2016237|2016231|2016229|240|241|242|239|2016242|243|247|248|249|251|252|253|2016249|20162491|20162492|20162493");
              SommeDaExcel.Add("DiffFraValoreECostiProdBILANCIO", "189|190|191|192|194|195|198|199|200|202|203|204|205|206|208|209|210|211|212|213|214|215");
              SommeDaExcel.Add("A1BILANCIO", "189");
              SommeDaExcel.Add("A2BILANCIO", "190");
              SommeDaExcel.Add("A3BILANCIO", "191");
              SommeDaExcel.Add("A4BILANCIO", "192");
              SommeDaExcel.Add("A5BILANCIO", "195");
              SommeDaExcel.Add("A5_2BILANCIO", "193");
              break;
            default:
              SommeDaExcel.Clear();
              SommeDaExcel.Add("TotaleAttivitaBILANCIO", "3|4|10071|10072|10073|10081|10082|10083|10092|10093|23|50|1059|1060|80|89|97");
              SommeDaExcel.Add("PatrimonionettoBILANCIO", "108|109|110|111|112|113|100114|119|120");
              SommeDaExcel.Add("RicaviEsercizioBILANCIO", "189|190|192|194|195");
              SommeDaExcel.Add("RisultatoImposteBILANCIO", "189|190|192|194|195|198|199|200|202|203|204|208|209|212|213|214|215|210|211|222|223|224|227|228|229|230|231|232|234|235|236|237|239|240|241|242|243|246|250|256|259");
              SommeDaExcel.Add("DiffFraValoreECostiProdBILANCIO", "189|190|191|192|194|195|198|199|200|202|203|204|205|206|208|209|210|211|212|213|214|215");
              SommeDaExcel.Add("A1BILANCIO", "189");
              SommeDaExcel.Add("A2BILANCIO", "190");
              SommeDaExcel.Add("A3BILANCIO", "191");
              SommeDaExcel.Add("A4BILANCIO", "192");
              SommeDaExcel.Add("A5BILANCIO", "195");
              SommeDaExcel.Add("A5_2BILANCIO", "193");
              break;
          }
          RetrieveData(IDBA_Padre);
        }
        #endregion
        ValoriDaExcelEA.Add("TotaleAttivitaBILANCIO", GetValoreEA("TotaleAttivitaBILANCIO"));
        ValoriDaExcelEA.Add("PatrimonionettoBILANCIO", GetValoreEA("PatrimonionettoBILANCIO"));
        ValoriDaExcelEA.Add("RicaviEsercizioBILANCIO", GetValoreEA("RicaviEsercizioBILANCIO"));
        ValoriDaExcelEA.Add("RisultatoImposteBILANCIO", GetValoreEA("RisultatoImposteBILANCIO"));
        ValoriDaExcelEA.Add("DiffFraValoreECostiProdBILANCIO", GetValoreEA("DiffFraValoreECostiProdBILANCIO"));
        ValoriDaExcelEA.Add("A1BILANCIO", GetValoreEA("A1BILANCIO"));
        ValoriDaExcelEA.Add("A2BILANCIO", GetValoreEA("A2BILANCIO"));
        ValoriDaExcelEA.Add("A3BILANCIO", GetValoreEA("A3BILANCIO"));
        ValoriDaExcelEA.Add("A4BILANCIO", GetValoreEA("A4BILANCIO"));
        ValoriDaExcelEA.Add("A5BILANCIO", GetValoreEA("A5BILANCIO"));
        ValoriDaExcelEA.Add("A5_2BILANCIO", GetValoreEA("A5_2BILANCIO"));
        ValoriDaExcelEA.Add("TotaleAttivita", 0.0);
        ValoriDaExcelEA.Add("Patrimonionetto", 0.0);
        ValoriDaExcelEA.Add("RicaviEsercizio", 0.0);
        ValoriDaExcelEA.Add("RisultatoImposte", 0.0);
        ValoriDaExcelEA.Add("DiffFraValoreECostiProd", 0.0);
        ValoriDaExcelEA.Add("A1", 0.0);
        ValoriDaExcelEA.Add("A2", 0.0);
        ValoriDaExcelEA.Add("A3", 0.0);
        ValoriDaExcelEA.Add("A4", 0.0);
        ValoriDaExcelEA.Add("A5", 0.0);
        ValoriDaExcelEA.Add("A5_2", 0.0);
        txt1BILANCIO.Text = ConvertNumber(ValoriDaExcelEA["TotaleAttivitaBILANCIO"].ToString());
        txt2BILANCIO.Text = ConvertNumber(ValoriDaExcelEA["PatrimonionettoBILANCIO"].ToString());
        txt3BILANCIO.Text = ConvertNumber(ValoriDaExcelEA["RicaviEsercizioBILANCIO"].ToString());
        txt4BILANCIO.Text = ConvertNumber(ValoriDaExcelEA["RisultatoImposteBILANCIO"].ToString());
        txt14BILANCIO.Text = ConvertNumber(ValoriDaExcelEA["DiffFraValoreECostiProdBILANCIO"].ToString());
        txtContoEcValoreProdA1BILANCIO.Text = ConvertNumber(ValoriDaExcelEA["A1BILANCIO"].ToString());
        txtContoEcValoreProdA2BILANCIO.Text = ConvertNumber(ValoriDaExcelEA["A2BILANCIO"].ToString());
        txtContoEcValoreProdA3BILANCIO.Text = ConvertNumber(ValoriDaExcelEA["A3BILANCIO"].ToString());
        txtContoEcValoreProdA4BILANCIO.Text = ConvertNumber(ValoriDaExcelEA["A4BILANCIO"].ToString());
        txtContoEcValoreProdA5BILANCIO.Text = ConvertNumber(ValoriDaExcelEA["A5BILANCIO"].ToString());
        txtContoEcValoreProdA5_2BILANCIO.Text = ConvertNumber(ValoriDaExcelEA["A5_2BILANCIO"].ToString());
      }

      //  XmlNode tmpNode = _d.Document.SelectSingleNode("/Dati//Dato[@ID='" + _ID + "']");



      /* Iuri 4.12.4 Aggiunti per nuovi campi materialità */
      if (getval("rbtTipoMaterialitaPianificata1").ToString() != "")
      {
        rbtTipoMaterialitaPianificata1.IsChecked = Convert.ToBoolean(getval("rbtTipoMaterialitaPianificata1").ToString());
      }
      if (getval("rbtTipoMaterialitaPianificata2").ToString() != "")
      {
        rbtTipoMaterialitaPianificata2.IsChecked = Convert.ToBoolean(getval("rbtTipoMaterialitaPianificata2").ToString());
      }
      if (getval("rbtTipoMaterialitaBilancio1").ToString() != "")
      {
        rbtTipoMaterialitaBilancio1.IsChecked = Convert.ToBoolean(getval("rbtTipoMaterialitaBilancio1").ToString());
      }
      if (getval("rbtTipoMaterialitaBilancio2").ToString() != "")
      {
        rbtTipoMaterialitaBilancio2.IsChecked = Convert.ToBoolean(getval("rbtTipoMaterialitaBilancio2").ToString());
      }
      /* Fine Iuri 4.12.4 Aggiunti per nuovi campi materialità */
      if (getval("chk1").ToString() != "")
      {
        chk1.IsChecked = Convert.ToBoolean(getval("chk1").ToString());
      }
      if (getval("chk2").ToString() != "")
      {
        chk2.IsChecked = Convert.ToBoolean(getval("chk2").ToString());
      }
      if (getval("chk3").ToString() != "")
      {
        chk3.IsChecked = Convert.ToBoolean(getval("chk3").ToString());
      }
      if (getval("chk4").ToString() != "")
      {
        chk4.IsChecked = Convert.ToBoolean(getval("chk4").ToString());
      }
      if (getval("chk14").ToString() != "")
      {
        chk14.IsChecked = Convert.ToBoolean(getval("chk14").ToString());
      }
      if (getval("chkContoEcValoreProdTotale").ToString() != "")
      {
        chkContoEcValoreProdTotale.IsChecked = Convert.ToBoolean(getval("chkContoEcValoreProdTotale").ToString());
      }
      if (getval("chk1BILANCIO").ToString() != "")
      {
        chk1BILANCIO.IsChecked = Convert.ToBoolean(getval("chk1BILANCIO").ToString());
      }
      if (getval("chk2BILANCIO").ToString() != "")
      {
        chk2BILANCIO.IsChecked = Convert.ToBoolean(getval("chk2BILANCIO").ToString());
      }
      if (getval("chk3BILANCIO").ToString() != "")
      {
        chk3BILANCIO.IsChecked = Convert.ToBoolean(getval("chk3BILANCIO").ToString());
      }
      if (getval("chk4BILANCIO").ToString() != "")
      {
        chk4BILANCIO.IsChecked = Convert.ToBoolean(getval("chk4BILANCIO").ToString());
      }
      if (getval("chk14BILANCIO").ToString() != "")
      {
        chk14BILANCIO.IsChecked = Convert.ToBoolean(getval("chk14BILANCIO").ToString());
      }
      if (getval("chkContoEcValoreProdTotaleBILANCIO").ToString() != "")
      {
        chkContoEcValoreProdTotaleBILANCIO.IsChecked = Convert.ToBoolean(getval("chkContoEcValoreProdTotaleBILANCIO").ToString());
      }
      if (ID != "314")
      {
        if (getval("txt1").ToString() != "")
        {
          txt1.Text = ConvertNumber(getval("txt1").ToString());
        }
        if (getval("txt2").ToString() != "")
        {
          txt2.Text = ConvertNumber(getval("txt2").ToString());
        }
        if (getval("txt3").ToString() != "")
        {
          txt3.Text = ConvertNumber(getval("txt3").ToString());
        }
        if (getval("txt4").ToString() != "")
        {
          txt4.Text = ConvertNumber(getval("txt4").ToString());
        }
        if (getval("txt14").ToString() != "")
        {
          txt14.Text = ConvertNumber(getval("txt14").ToString());
        }
        if (getval("txtContoEcValoreProdA1").ToString() != "")
        {
          txtContoEcValoreProdA1.Text = ConvertNumber(getval("txtContoEcValoreProdA1").ToString());
        }
        if (getval("txtContoEcValoreProdA2").ToString() != "")
        {
          txtContoEcValoreProdA2.Text = ConvertNumber(getval("txtContoEcValoreProdA2").ToString());
        }
        if (getval("txtContoEcValoreProdA3").ToString() != "")
        {
          txtContoEcValoreProdA3.Text = ConvertNumber(getval("txtContoEcValoreProdA3").ToString());
        }
        if (getval("txtContoEcValoreProdA4").ToString() != "")
        {
          txtContoEcValoreProdA4.Text = ConvertNumber(getval("txtContoEcValoreProdA4").ToString());
        }
        if (getval("txtContoEcValoreProdA5").ToString() != "")
        {
          txtContoEcValoreProdA5.Text = ConvertNumber(getval("txtContoEcValoreProdA5").ToString());
        }
        if (getval("txtContoEcValoreProdA5_2").ToString() != "")
        {
          txtContoEcValoreProdA5_2.Text = ConvertNumber(getval("txtContoEcValoreProdA5_2").ToString());
        }
        if (getval("txtContoEcValoreProdTotale").ToString() != "")
        {
          txtContoEcValoreProdTotale.Text = ConvertNumber(getval("txtContoEcValoreProdTotale").ToString());
        }
      }
      if (getval("txt8").ToString() != "")
      {
        txt8.Text = ConvertPercentIntero(getval("txt8").ToString());
      }
      if (getval("txt10").ToString() != "")
      {
        txt10.Text = ConvertPercentIntero(getval("txt10").ToString());
      }
      if (getval("txt11").ToString() != "")
      {
        txt11.Text = ConvertPercentIntero(getval("txt11").ToString());
      }
      if (getval("txt7_3sp").ToString() != "")
      {
        txt7_3sp.Text = ConvertNumber(getval("txt7_3sp").ToString());
      }
      if (getval("txt7_3ec").ToString() != "")
      {
        txt7_3ec.Text = ConvertNumber(getval("txt7_3ec").ToString());
      }
      if (getval("txt1_p_min").ToString() != "")
      {
        txt1_p_min.Text = ConvertPercent(getval("txt1_p_min").ToString());
      }
      if (getval("txt2_p_min").ToString() != "")
      {
        txt2_p_min.Text = ConvertPercent(getval("txt2_p_min").ToString());
      }
      if (getval("txt3_p_min").ToString() != "")
      {
        txt3_p_min.Text = ConvertPercent(getval("txt3_p_min").ToString());
      }
      if (getval("txt4_p_min").ToString() != "")
      {
        txt4_p_min.Text = ConvertPercent(getval("txt4_p_min").ToString());
      }
      if (getval("txt14_p_min").ToString() != "")
      {
        txt14_p_min.Text = ConvertPercent(getval("txt14_p_min").ToString());
      }
      if (getval("txtContoEcValoreProdTotale_p_min").ToString() != "")
      {
        txtContoEcValoreProdTotale_p_min.Text = ConvertPercent(getval("txtContoEcValoreProdTotale_p_min").ToString());
      }
      if (getval("txt1_p_max").ToString() != "")
      {
        txt1_p_max.Text = ConvertPercent(getval("txt1_p_max").ToString());
      }
      if (getval("txt2_p_max").ToString() != "")
      {
        txt2_p_max.Text = ConvertPercent(getval("txt2_p_max").ToString());
      }
      if (getval("txt3_p_max").ToString() != "")
      {
        txt3_p_max.Text = ConvertPercent(getval("txt3_p_max").ToString());
      }
      if (getval("txt4_p_max").ToString() != "")
      {
        txt4_p_max.Text = ConvertPercent(getval("txt4_p_max").ToString());
      }
      if (getval("txt14_p_max").ToString() != "")
      {
        txt14_p_max.Text = ConvertPercent(getval("txt14_p_max").ToString());
      }
      if (getval("txtContoEcValoreProdTotale_p_max").ToString() != "")
      {
        txtContoEcValoreProdTotale_p_max.Text = ConvertPercent(getval("txtContoEcValoreProdTotale_p_max").ToString());
      }
      if (getval("txt8_2sp").ToString() != "")
      {
        txt8_2sp.Text = ConvertPercentIntero(getval("txt8_2sp").ToString());
      }
      if (getval("txt10_2sp").ToString() != "")
      {
        txt10_2sp.Text = ConvertPercentIntero(getval("txt10_2sp").ToString());
      }
      if (getval("txt11_2sp").ToString() != "")
      {
        txt11_2sp.Text = ConvertPercentIntero(getval("txt11_2sp").ToString());
      }
      if (getval("txt8_2ec").ToString() != "")
      {
        txt8_2ce.Text = ConvertPercentIntero(getval("txt8_2ec").ToString());
      }
      if (getval("txt10_2ec").ToString() != "")
      {
        txt10_2ce.Text = ConvertPercentIntero(getval("txt10_2ec").ToString());
      }
      if (getval("txt11_2ec").ToString() != "")
      {
        txt11_2ce.Text = ConvertPercentIntero(getval("txt11_2ec").ToString());
      }
      if (getval("txt8_3sp").ToString() != "")
      {
        txt8_3sp.Text = ConvertPercentIntero(getval("txt8_3sp").ToString());
      }
      if (getval("txt10_3sp").ToString() != "")
      {
        txt10_3sp.Text = ConvertPercentIntero(getval("txt10_3sp").ToString());
      }
      if (getval("txt11_3sp").ToString() != "")
      {
        txt11_3sp.Text = ConvertPercentIntero(getval("txt11_3sp").ToString());
      }
      if (getval("txt8_3ec").ToString() != "")
      {
        txt8_3ec.Text = ConvertPercentIntero(getval("txt8_3ec").ToString());
      }
      if (getval("txt10_3ec").ToString() != "")
      {
        txt10_3ec.Text = ConvertPercentIntero(getval("txt10_3ec").ToString());
      }
      if (getval("txt11_3ec").ToString() != "")
      {
        txt11_3ec.Text = ConvertPercentIntero(getval("txt11_3ec").ToString());
      }
      //DA BILANCIO
      if (ID != "314")
      {
        if (getval("txt1BILANCIO").ToString() != "" && txt1BILANCIO.Text == "")
        {
          txt1BILANCIO.Text = ConvertNumber(getval("txt1BILANCIO").ToString());
        }
        if (getval("txt2BILANCIO").ToString() != "" && txt2BILANCIO.Text == "")
        {
          txt2BILANCIO.Text = ConvertNumber(getval("txt2BILANCIO").ToString());
        }
        if (getval("txt3BILANCIO").ToString() != "" && txt3BILANCIO.Text == "")
        {
          txt3BILANCIO.Text = ConvertNumber(getval("txt3BILANCIO").ToString());
        }
        if (getval("txt4BILANCIO").ToString() != "" && txt4BILANCIO.Text == "")
        {
          txt4BILANCIO.Text = ConvertNumber(getval("txt4BILANCIO").ToString());
        }
        if (getval("txt14BILANCIO").ToString() != "" && txt14BILANCIO.Text == "")
        {
          txt14BILANCIO.Text = ConvertNumber(getval("txt14BILANCIO").ToString());
        }
        if (getval("txtContoEcValoreProdA1BILANCIO").ToString() != "")
        {
          txtContoEcValoreProdA1BILANCIO.Text = ConvertNumber(getval("txtContoEcValoreProdA1BILANCIO").ToString());
        }
        if (getval("txtContoEcValoreProdA2BILANCIO").ToString() != "")
        {
          txtContoEcValoreProdA2.Text = ConvertNumber(getval("txtContoEcValoreProdA2").ToString());
        }
        if (getval("txtContoEcValoreProdA3BILANCIO").ToString() != "")
        {
          txtContoEcValoreProdA3BILANCIO.Text = ConvertNumber(getval("txtContoEcValoreProdA3BILANCIO").ToString());
        }
        if (getval("txtContoEcValoreProdA4BILANCIO").ToString() != "")
        {
          txtContoEcValoreProdA4BILANCIO.Text = ConvertNumber(getval("txtContoEcValoreProdA4BILANCIO").ToString());
        }
        if (getval("txtContoEcValoreProdA5BILANCIO").ToString() != "")
        {
          txtContoEcValoreProdA5BILANCIO.Text = ConvertNumber(getval("txtContoEcValoreProdA5BILANCIO").ToString());
        }
        if (getval("txtContoEcValoreProdA5_2BILANCIO").ToString() != "")
        {
          txtContoEcValoreProdA5_2BILANCIO.Text = ConvertNumber(getval("txtContoEcValoreProdA5_2BILANCIO").ToString());
        }
        if (getval("txtContoEcValoreProdTotaleBILANCIO").ToString() != "")
        {
          txtContoEcValoreProdTotaleBILANCIO.Text = ConvertNumber(getval("txtContoEcValoreProdTotaleBILANCIO").ToString());
        }
      }
      if (getval("txt8").ToString() != "")
      {
        txt8BILANCIO.Text = ConvertPercentIntero(getval("txt8").ToString());
      }
      if (getval("txt10").ToString() != "")
      {
        txt10BILANCIO.Text = ConvertPercentIntero(getval("txt10").ToString());
      }
      if (getval("txt11").ToString() != "")
      {
        txt11BILANCIO.Text = ConvertPercentIntero(getval("txt11").ToString());
      }
      if (getval("txt7_3sp").ToString() != "")
      {
        txt7_3spBILANCIO.Text = ConvertNumber(getval("txt7_3sp").ToString());
      }
      if (getval("txt7_3ec").ToString() != "")
      {
        txt7_3ecBILANCIO.Text = ConvertNumber(getval("txt7_3ec").ToString());
      }
      if (getval("txt1_p_min").ToString() != "")
      {
        txt1_p_minBILANCIO.Text = ConvertPercent(getval("txt1_p_min").ToString());
      }
      if (getval("txt2_p_min").ToString() != "")
      {
        txt2_p_minBILANCIO.Text = ConvertPercent(getval("txt2_p_min").ToString());
      }
      if (getval("txt3_p_min").ToString() != "")
      {
        txt3_p_minBILANCIO.Text = ConvertPercent(getval("txt3_p_min").ToString());
      }
      if (getval("txt4_p_min").ToString() != "")
      {
        txt4_p_minBILANCIO.Text = ConvertPercent(getval("txt4_p_min").ToString());
      }
      if (getval("txt14_p_min").ToString() != "")
      {
        txt14_p_minBILANCIO.Text = ConvertPercent(getval("txt14_p_min").ToString());
      }
      if (getval("txtContoEcValoreProdTotaleBILANCIO_p_min").ToString() != "")
      {
        txtContoEcValoreProdTotaleBILANCIO_p_min.Text = ConvertPercent(getval("txtContoEcValoreProdTotaleBILANCIO_p_min").ToString());
      }
      if (getval("txt1_p_max").ToString() != "")
      {
        txt1_p_maxBILANCIO.Text = ConvertPercent(getval("txt1_p_max").ToString());
      }
      if (getval("txt2_p_max").ToString() != "")
      {
        txt2_p_maxBILANCIO.Text = ConvertPercent(getval("txt2_p_max").ToString());
      }
      if (getval("txt3_p_max").ToString() != "")
      {
        txt3_p_maxBILANCIO.Text = ConvertPercent(getval("txt3_p_max").ToString());
      }
      if (getval("txt4_p_max").ToString() != "")
      {
        txt4_p_maxBILANCIO.Text = ConvertPercent(getval("txt4_p_max").ToString());
      }
      if (getval("txt14_p_max").ToString() != "")
      {
        txt14_p_maxBILANCIO.Text = ConvertPercent(getval("txt14_p_max").ToString());
      }
      if (getval("txtContoEcValoreProdTotaleBILANCIO_p_max").ToString() != "")
      {
        txtContoEcValoreProdTotaleBILANCIO_p_max.Text = ConvertPercent(getval("txtContoEcValoreProdTotaleBILANCIO_p_max").ToString());
      }
      if (getval("txt8_2sp").ToString() != "")
      {
        txt8_2spBILANCIO.Text = ConvertPercentIntero(getval("txt8_2sp").ToString());
      }
      if (getval("txt10_2sp").ToString() != "")
      {
        txt10_2spBILANCIO.Text = ConvertPercentIntero(getval("txt10_2sp").ToString());
      }
      if (getval("txt11_2sp").ToString() != "")
      {
        txt11_2spBILANCIO.Text = ConvertPercentIntero(getval("txt11_2sp").ToString());
      }
      if (getval("txt8_2ec").ToString() != "")
      {
        txt8_2ceBILANCIO.Text = ConvertPercentIntero(getval("txt8_2ec").ToString());
      }
      if (getval("txt10_2ec").ToString() != "")
      {
        txt10_2ceBILANCIO.Text = ConvertPercentIntero(getval("txt10_2ec").ToString());
      }
      if (getval("txt11_2ec").ToString() != "")
      {
        txt11_2ceBILANCIO.Text = ConvertPercentIntero(getval("txt11_2ec").ToString());
      }
      if (getval("txt8_3sp").ToString() != "")
      {
        txt8_3spBILANCIO.Text = ConvertPercentIntero(getval("txt8_3sp").ToString());
      }
      if (getval("txt10_3sp").ToString() != "")
      {
        txt10_3spBILANCIO.Text = ConvertPercentIntero(getval("txt10_3sp").ToString());
      }
      if (getval("txt11_3sp").ToString() != "")
      {
        txt11_3spBILANCIO.Text = ConvertPercentIntero(getval("txt11_3sp").ToString());
      }
      if (getval("txt8_3ec").ToString() != "")
      {
        txt8_3ecBILANCIO.Text = ConvertPercentIntero(getval("txt8_3ec").ToString());
      }
      if (getval("txt10_3ec").ToString() != "")
      {
        txt10_3ecBILANCIO.Text = ConvertPercentIntero(getval("txt10_3ec").ToString());
      }
      if (getval("txt11_3ec").ToString() != "")
      {
        txt11_3ecBILANCIO.Text = ConvertPercentIntero(getval("txt11_3ec").ToString());
      }

      canbecalculated = true;
      if (!_isUpdatingData)
      {
        _isUpdatingData = true;
        UpdateData();
        _isUpdatingData = false;
      }
    }

    private object getval(string fname)
    {

      bool trovato = false;
      foreach (DataRow dtrow in dati.Rows)
      {
        if (dtrow["ID"].ToString() == fname)
        {
          return dtrow["value"];
        }
      }
      if (!trovato)
      {
        DataRow node = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, fname);
      }
      return "";

    }

    private void T_LostFocus(object sender, RoutedEventArgs e)
    {
      TextBox t = (TextBox)sender;


      foreach (DataRow dtrow in dati.Rows)
      {
        if (dtrow["ID"].ToString() == t.Tag.ToString())
        {
          dtrow["ma"] = t.Text;
        }
      }

      CALCULATECONSOLIDATO();
    }

    private void T_LostFocus1(object sender, RoutedEventArgs e)
    {
      TextBox t = (TextBox)sender;

      foreach (DataRow dtrow in dati.Rows)
      {
        if (dtrow["ID"].ToString() == t.Tag.ToString())
        {
          dtrow["et"] = t.Text;
        }
      }

      CALCULATECONSOLIDATO();
    }


    private void setval(string fname, string valore)
    {
      bool trovato = false;
      foreach (DataRow dtrow in dati.Rows)
      {
        if (dtrow["ID"].ToString() == fname)
        {
          dtrow["value"] = valore;
          trovato = true;
        }
      }
      if (!trovato)
      {
        DataRow node = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, fname);
        node["value"] = valore;
      }

    }


    public void UpdateXMLData()
    {


      /* Iuri 4.12.4 Aggiunti per nuovi campi materialità */

      setval("rbtTipoMaterialitaPianificata1", rbtTipoMaterialitaPianificata1.IsChecked.ToString());
      setval("rbtTipoMaterialitaPianificata2", rbtTipoMaterialitaPianificata2.IsChecked.ToString());

      setval("rbtTipoMaterialitaBilancio1", rbtTipoMaterialitaBilancio1.IsChecked.ToString());
      setval("rbtTipoMaterialitaBilancio2", rbtTipoMaterialitaBilancio2.IsChecked.ToString());

      /* Fine Iuri 4.12.4 Aggiunti per nuovi campi materialità */

      setval("chk1", chk1.IsChecked.ToString());
      setval("chk2", chk2.IsChecked.ToString());
      setval("chk3", chk3.IsChecked.ToString());
      setval("chk4", chk4.IsChecked.ToString());
      setval("chk14", chk14.IsChecked.ToString());
      setval("chkContoEcValoreProdTotale", chkContoEcValoreProdTotale.IsChecked.ToString());

      //setval("chk1BILANCIO",chk1BILANCIO.IsChecked.ToString());
      //setval("chk2BILANCIO",chk2BILANCIO.IsChecked.ToString());
      //setval("chk3BILANCIO",chk3BILANCIO.IsChecked.ToString());
      //setval("chk4BILANCIO",chk4BILANCIO.IsChecked.ToString());

      setval("chk1BILANCIO", chk1.IsChecked.ToString());
      setval("chk2BILANCIO", chk2.IsChecked.ToString());
      setval("chk3BILANCIO", chk3.IsChecked.ToString());
      setval("chk4BILANCIO", chk4.IsChecked.ToString());
      setval("chk14BILANCIO", chk14.IsChecked.ToString());
      setval("chkContoEcValoreProdTotaleBILANCIO", chkContoEcValoreProdTotale.IsChecked.ToString());


      if (_ipotesi == IpotesiMaterialita.Prima)
      {
        chk1BILANCIO.IsChecked = chk1.IsChecked;
        chk2BILANCIO.IsChecked = chk2.IsChecked;
        chk3BILANCIO.IsChecked = chk3.IsChecked;
        chk4BILANCIO.IsChecked = chk4.IsChecked;
        chk14BILANCIO.IsChecked = chk14.IsChecked;
        chkContoEcValoreProdTotaleBILANCIO.IsChecked = chkContoEcValoreProdTotale.IsChecked;
      }

      setval("txt1", txt1.Text);
      setval("txt2", txt2.Text);
      setval("txt3", txt3.Text);
      setval("txt4", txt4.Text);
      setval("txt14", txt14.Text);

      setval("txtContoEcValoreProdA1", txtContoEcValoreProdA1.Text);
      setval("txtContoEcValoreProdA2", txtContoEcValoreProdA2.Text);
      setval("txtContoEcValoreProdA3", txtContoEcValoreProdA3.Text);
      setval("txtContoEcValoreProdA4", txtContoEcValoreProdA4.Text);
      setval("txtContoEcValoreProdA5", txtContoEcValoreProdA5.Text);
      setval("txtContoEcValoreProdA5_2", txtContoEcValoreProdA5_2.Text);
      setval("txtContoEcValoreProdTotale", txtContoEcValoreProdTotale.Text);

      setval("txt8", txt8.Text);
      setval("txt10", txt10.Text);
      setval("txt11", txt11.Text);

      setval("txt7_3sp", txt7_3sp.Text);
      setval("txt7_3ec", txt7_3ec.Text);

      setval("txt1_p_min", txt1_p_min.Text);
      setval("txt2_p_min", txt2_p_min.Text);
      setval("txt3_p_min", txt3_p_min.Text);
      setval("txt4_p_min", txt4_p_min.Text);
      setval("txt14_p_min", txt14_p_min.Text);
      setval("txtContoEcValoreProdTotale_p_min", txtContoEcValoreProdTotale_p_min.Text);

      setval("txt1_p_max", txt1_p_max.Text);
      setval("txt2_p_max", txt2_p_max.Text);
      setval("txt3_p_max", txt3_p_max.Text);
      setval("txt4_p_max", txt4_p_max.Text);
      setval("txt14_p_max", txt14_p_max.Text);
      setval("txtContoEcValoreProdTotale_p_max", txtContoEcValoreProdTotale_p_max.Text);




      setval("txt8_2sp", txt8_2sp.Text);
      setval("txt10_2sp", txt10_2sp.Text);
      setval("txt11_2sp", txt11_2sp.Text);

      setval("txt8_2ec", txt8_2ce.Text);
      setval("txt10_2ec", txt10_2ce.Text);
      setval("txt11_2ec", txt11_2ce.Text);





      setval("txt8_3sp", txt8_3sp.Text);
      setval("txt10_3sp", txt10_3sp.Text);
      setval("txt11_3sp", txt11_3sp.Text);

      setval("txt8_3ec", txt8_3ec.Text);
      setval("txt10_3ec", txt10_3ec.Text);
      setval("txt11_3ec", txt11_3ec.Text);



      setval("txt1BILANCIO", txt1BILANCIO.Text);
      setval("txt2BILANCIO", txt2BILANCIO.Text);
      setval("txt3BILANCIO", txt3BILANCIO.Text);
      setval("txt4BILANCIO", txt4BILANCIO.Text);
      setval("txt14BILANCIO", txt14BILANCIO.Text);

      setval("txtContoEcValoreProdA1BILANCIO", txtContoEcValoreProdA1BILANCIO.Text);
      setval("txtContoEcValoreProdA2BILANCIO", txtContoEcValoreProdA2BILANCIO.Text);
      setval("txtContoEcValoreProdA3BILANCIO", txtContoEcValoreProdA3BILANCIO.Text);
      setval("txtContoEcValoreProdA4BILANCIO", txtContoEcValoreProdA4BILANCIO.Text);
      setval("txtContoEcValoreProdA5BILANCIO", txtContoEcValoreProdA5BILANCIO.Text);
      setval("txtContoEcValoreProdA5_2BILANCIO", txtContoEcValoreProdA5_2BILANCIO.Text);
      setval("txtContoEcValoreProdTotaleBILANCIO", txtContoEcValoreProdTotaleBILANCIO.Text);

      setval("txt8BILANCIO", txt8.Text);
      setval("txt10BILANCIO", txt10.Text);
      setval("txt11BILANCIO", txt11.Text);

      setval("txt7_3spBILANCIO", txt7_3spBILANCIO.Text);
      setval("txt7_3ecBILANCIO", txt7_3ecBILANCIO.Text);

      setval("txt1_p_minBILANCIO", txt1_p_min.Text);
      setval("txt2_p_minBILANCIO", txt2_p_min.Text);
      setval("txt3_p_minBILANCIO", txt3_p_min.Text);
      setval("txt4_p_minBILANCIO", txt4_p_min.Text);
      setval("txt14_p_minBILANCIO", txt4_p_min.Text);
      setval("txtContoEcValoreProdTotaleBILANCIO_p_min", txtContoEcValoreProdTotaleBILANCIO_p_min.Text);

      setval("txt1_p_maxBILANCIO", txt1_p_max.Text);
      setval("txt2_p_maxBILANCIO", txt2_p_max.Text);
      setval("txt3_p_maxBILANCIO", txt3_p_max.Text);
      setval("txt4_p_maxBILANCIO", txt4_p_max.Text);
      setval("txt14_p_maxBILANCIO", txt4_p_max.Text);
      setval("txtContoEcValoreProdTotaleBILANCIO_p_max", txtContoEcValoreProdTotaleBILANCIO_p_max.Text);




      setval("txt8_2spBILANCIO", txt8_2sp.Text);
      setval("txt10_2spBILANCIO", txt10_2sp.Text);
      setval("txt11_2spBILANCIO", txt11_2sp.Text);

      setval("txt8_2ecBILANCIO", txt8_2ce.Text);
      setval("txt10_2ecBILANCIO", txt10_2ce.Text);
      setval("txt11_2ecBILANCIO", txt11_2ce.Text);





      setval("txt8_3spBILANCIO", txt8_3sp.Text);
      setval("txt10_3spBILANCIO", txt10_3sp.Text);
      setval("txt11_3spBILANCIO", txt11_3sp.Text);

      setval("txt8_3ecBILANCIO", txt8_3ec.Text);
      setval("txt10_3ecBILANCIO", txt10_3ec.Text);
      setval("txt11_3ecBILANCIO", txt11_3ec.Text);



      setval("txt1min", txt1min.Text);

      setval("txt1lmax", txt1lmax.Text);

      setval("txt1lmaxdn", txt1lmaxdn.Text);

      setval("txt1lmindn", txt1lmindn.Text);

      setval("txt2lmin", txt2lmin.Text);

      setval("txt2lmax", txt2lmax.Text);

      setval("txt2lmaxdn", txt2lmaxdn.Text);

      setval("txt2lmindn", txt2lmindn.Text);

      setval("txt3lmin", txt3lmin.Text);


      setval("txt3lmax", txt3lmax.Text);

      setval("txt3lmaxdn", txt3lmaxdn.Text);

      setval("txt3lmindn", txt3lmindn.Text);

      setval("txt4lmin", txt4lmin.Text);

      setval("txt4lmax", txt4lmax.Text);

      setval("txt4lmaxdn", txt4lmaxdn.Text);

      setval("txt4lmindn", txt4lmindn.Text);

      setval("txt14lmin", txt14lmin.Text);

      setval("txt14lmax", txt14lmax.Text);

      setval("txt14lmaxdn", txt14lmaxdn.Text);

      setval("txt14lmindn", txt14lmindn.Text);

      setval("txtContoEcValoreProdTotalelmin", txtContoEcValoreProdTotalelmin.Text);

      setval("txtContoEcValoreProdTotalelmax", txtContoEcValoreProdTotalelmax.Text);

      setval("txtContoEcValoreProdTotalelmaxdn", txtContoEcValoreProdTotalelmaxdn.Text);

      setval("txtContoEcValoreProdTotalelmindn", txtContoEcValoreProdTotalelmindn.Text);

      setval("txt5", txt5.Text);

      setval("txt6", txt6.Text);

      setval("txt7", txt7.Text);

      setval("txt9", txt9.Text);

      setval("txt12", txt12.Text);

      setval("txt13", txt13.Text);

      setval("txt5_2sp", txt5_2sp.Text);

      setval("txt6_2sp", txt6_2sp.Text);

      setval("txt7_2sp", txt7_2sp.Text);

      setval("txt9_2sp", txt9_2sp.Text);

      setval("txt12_2sp", txt12_2sp.Text);

      setval("txt13_2sp", txt13_2sp.Text);

      setval("txt5_2ce", txt5_2ce.Text);

      setval("txt6_2ce", txt6_2ce.Text);

      setval("txt7_2ce", txt7_2ce.Text);

      setval("txt9_2ce", txt9_2ce.Text);

      setval("txt12_2ce", txt12_2ce.Text);

      setval("txt13_2ce", txt13_2ce.Text);

      setval("txt9_3sp", txt9_3sp.Text);

      setval("txt12_3sp", txt12_3sp.Text);

      setval("txt13_3sp", txt13_3sp.Text);

      setval("txt9_3ec", txt9_3ec.Text);

      setval("txt12_3ec", txt12_3ec.Text);

      setval("txt13_3ec", txt13_3ec.Text);

      setval("txt1minBILANCIO", txt1minBILANCIO.Text);

      setval("txt1lmaxBILANCIO", txt1lmaxBILANCIO.Text);

      setval("txt1lmaxdnBILANCIO", txt1lmaxdnBILANCIO.Text);

      setval("txt1lmindnBILANCIO", txt1lmindnBILANCIO.Text);

      setval("txt2lminBILANCIO", txt2lminBILANCIO.Text);

      setval("txt2lmaxBILANCIO", txt2lmaxBILANCIO.Text);

      setval("txt2lmaxdnBILANCIO", txt2lmaxdnBILANCIO.Text);

      setval("txt2lmindnBILANCIO", txt2lmindnBILANCIO.Text);

      setval("txt3lminBILANCIO", txt3lminBILANCIO.Text);

      setval("txt3lmaxBILANCIO", txt3lmaxBILANCIO.Text);

      setval("txt3lmaxdnBILANCIO", txt3lmaxdnBILANCIO.Text);

      setval("txt3lmindnBILANCIO", txt3lmindnBILANCIO.Text);

      setval("txt4lminBILANCIO", txt4lminBILANCIO.Text);

      setval("txt4lmaxBILANCIO", txt4lmaxBILANCIO.Text);

      setval("txt4lmaxdnBILANCIO", txt4lmaxdnBILANCIO.Text);

      setval("txt4lmindnBILANCIO", txt4lmindnBILANCIO.Text);

      setval("txt5BILANCIO", txt5BILANCIO.Text);

      setval("txt14lminBILANCIO", txt14lminBILANCIO.Text);

      setval("txt14lmaxBILANCIO", txt14lmaxBILANCIO.Text);

      setval("txt14lmaxdnBILANCIO", txt14lmaxdnBILANCIO.Text);

      setval("txt14lmindnBILANCIO", txt14lmindnBILANCIO.Text);

      setval("txtContoEcValoreProdTotalelminBILANCIO", txtContoEcValoreProdTotalelminBILANCIO.Text);

      setval("txtContoEcValoreProdTotalelmaxBILANCIO", txtContoEcValoreProdTotalelmaxBILANCIO.Text);

      setval("txtContoEcValoreProdTotalelmaxdnBILANCIO", txtContoEcValoreProdTotalelmaxdnBILANCIO.Text);

      setval("txtContoEcValoreProdTotalelmindnBILANCIO", txtContoEcValoreProdTotalelmindnBILANCIO.Text);

      setval("txt6BILANCIO", txt6BILANCIO.Text);

      setval("txt7BILANCIO", txt7BILANCIO.Text);

      setval("txt9BILANCIO", txt9BILANCIO.Text);

      setval("txt12BILANCIO", txt12BILANCIO.Text);

      setval("txt13BILANCIO", txt13BILANCIO.Text);

      setval("txt5_2spBILANCIO", txt5_2spBILANCIO.Text);

      setval("txt6_2spBILANCIO", txt6_2spBILANCIO.Text);

      setval("txt7_2spBILANCIO", txt7_2spBILANCIO.Text);

      setval("txt9_2spBILANCIO", txt9_2spBILANCIO.Text);

      setval("txt12_2spBILANCIO", txt12_2spBILANCIO.Text);

      setval("txt13_2spBILANCIO", txt13_2spBILANCIO.Text);

      setval("txt5_2ceBILANCIO", txt5_2ceBILANCIO.Text);

      setval("txt6_2ceBILANCIO", txt6_2ceBILANCIO.Text);

      setval("txt7_2ceBILANCIO", txt7_2ceBILANCIO.Text);

      setval("txt9_2ceBILANCIO", txt9_2ceBILANCIO.Text);

      setval("txt12_2ceBILANCIO", txt12_2ceBILANCIO.Text);

      setval("txt13_2ceBILANCIO", txt13_2ceBILANCIO.Text);

      setval("txt9_3spBILANCIO", txt9_3spBILANCIO.Text);

      setval("txt12_3spBILANCIO", txt12_3spBILANCIO.Text);

      setval("txt13_3spBILANCIO", txt13_3spBILANCIO.Text);

      setval("txt9_3ecBILANCIO", txt9_3ecBILANCIO.Text);

      setval("txt12_3ecBILANCIO", txt12_3ecBILANCIO.Text);

      setval("txt13_3ecBILANCIO", txt13_3ecBILANCIO.Text);



    }

    public int Save()
    {

      if (!_isUpdatingXMLData)
      {
        _isUpdatingXMLData = true;
         UpdateData();
        UpdateXMLData();
        _isUpdatingXMLData = false;
      }


      return cBusinessObjects.SaveData(id, dati, typeof(Excel_LimiteMaterialitaSPCE));


    }

    private void UpdateData()
    {
      if (canbecalculated == false)
      {
        return;
      }

      if (!_isUpdatingXMLData)
      {
        _isUpdatingXMLData = true;
        UpdateXMLData();
        _isUpdatingXMLData = false;
      }

      if (_ipotesi == IpotesiMaterialita.Prima)
      {
        chk1BILANCIO.IsChecked = chk1.IsChecked;
        chk2BILANCIO.IsChecked = chk2.IsChecked;
        chk3BILANCIO.IsChecked = chk3.IsChecked;
        chk4BILANCIO.IsChecked = chk4.IsChecked;
        chk14BILANCIO.IsChecked = chk14.IsChecked;
        chkContoEcValoreProdTotaleBILANCIO.IsChecked = chkContoEcValoreProdTotale.IsChecked;


        foreach (DataRow dtrow in dati.Rows)
        {
          if (getval("txt8").ToString() != "")
          {
            txt8BILANCIO.Text = ConvertPercentIntero(getval("txt8").ToString());
          }

          if (getval("txt10").ToString() != "")
          {
            txt10BILANCIO.Text = ConvertPercentIntero(getval("txt10").ToString());
          }

          if (getval("txt11").ToString() != "")
          {
            txt11BILANCIO.Text = ConvertPercentIntero(getval("txt11").ToString());
          }

          if (getval("txt7_3sp").ToString() != "")
          {
            txt7_3spBILANCIO.Text = ConvertNumber(getval("txt7_3sp").ToString());
          }

          if (getval("txt7_3ec").ToString() != "")
          {
            txt7_3ecBILANCIO.Text = ConvertNumber(getval("txt7_3ec").ToString());
          }

          if (getval("txt1_p_min").ToString() != "")
          {
            txt1_p_minBILANCIO.Text = ConvertPercent(getval("txt1_p_min").ToString());
          }

          if (getval("txt2_p_min").ToString() != "")
          {
            txt2_p_minBILANCIO.Text = ConvertPercent(getval("txt2_p_min").ToString());
          }

          if (getval("txt3_p_min").ToString() != "")
          {
            txt3_p_minBILANCIO.Text = ConvertPercent(getval("txt3_p_min").ToString());
          }

          if (getval("txt4_p_min").ToString() != "")
          {
            txt4_p_minBILANCIO.Text = ConvertPercent(getval("txt4_p_min").ToString());
          }

          if (getval("txt1_p_max").ToString() != "")
          {
            txt1_p_maxBILANCIO.Text = ConvertPercent(getval("txt1_p_max").ToString());
          }

          if (getval("txt2_p_max").ToString() != "")
          {
            txt2_p_maxBILANCIO.Text = ConvertPercent(getval("txt2_p_max").ToString());
          }

          if (getval("txt3_p_max").ToString() != "")
          {
            txt3_p_maxBILANCIO.Text = ConvertPercent(getval("txt3_p_max").ToString());
          }

          if (getval("txt4_p_max").ToString() != "")
          {
            txt4_p_maxBILANCIO.Text = ConvertPercent(getval("txt4_p_max").ToString());
          }

          if (getval("txt8_2sp").ToString() != "")
          {
            txt8_2spBILANCIO.Text = ConvertPercentIntero(getval("txt8_2sp").ToString());
          }

          if (getval("txt10_2sp").ToString() != "")
          {
            txt10_2spBILANCIO.Text = ConvertPercentIntero(getval("txt10_2sp").ToString());
          }

          if (getval("txt11_2sp").ToString() != "")
          {
            txt11_2spBILANCIO.Text = ConvertPercentIntero(getval("txt11_2sp").ToString());
          }

          if (getval("txt8_2ec").ToString() != "")
          {
            txt8_2ceBILANCIO.Text = ConvertPercentIntero(getval("txt8_2ec").ToString());
          }

          if (getval("txt10_2ec").ToString() != "")
          {
            txt10_2ceBILANCIO.Text = ConvertPercentIntero(getval("txt10_2ec").ToString());
          }

          if (getval("txt11_2ec").ToString() != "")
          {
            txt11_2ceBILANCIO.Text = ConvertPercentIntero(getval("txt11_2ec").ToString());
          }

          if (getval("txt8_3sp").ToString() != "")
          {
            txt8_3spBILANCIO.Text = ConvertPercentIntero(getval("txt8_3sp").ToString());
          }

          if (getval("txt10_3sp").ToString() != "")
          {
            txt10_3spBILANCIO.Text = ConvertPercentIntero(getval("txt10_3sp").ToString());
          }

          if (getval("txt11_3sp").ToString() != "")
          {
            txt11_3spBILANCIO.Text = ConvertPercentIntero(getval("txt11_3sp").ToString());
          }

          if (getval("txt8_3ec").ToString() != "")
          {
            txt8_3ecBILANCIO.Text = ConvertPercentIntero(getval("txt8_3ec").ToString());
          }

          if (getval("txt10_3ec").ToString() != "")
          {
            txt10_3ecBILANCIO.Text = ConvertPercentIntero(getval("txt10_3ec").ToString());
          }

          if (getval("txt11_3ec").ToString() != "")
          {
            txt11_3ecBILANCIO.Text = ConvertPercentIntero(getval("txt11_3ec").ToString());
          }

        }
      }


      //txtContoEcValoreProdTotale

      double dblValore = 0.0;

      double TotMin = 0.0;
      double TotMax = 0.0;

      double TotMinSP = 0.0;
      double TotMaxSP = 0.0;
      double TotMinEC = 0.0;
      double TotMaxEC = 0.0;
      double TotValoreProdEC = 0.0;




      double numchecked = 0;
      double numchecked1 = 0;
      double numchecked2 = 0;

      #region Valore Produzione


      if (double.TryParse(txtContoEcValoreProdA1.Text, out dblValore))
      {
        TotValoreProdEC += dblValore;
      }

      if (double.TryParse(txtContoEcValoreProdA2.Text, out dblValore))
      {
        TotValoreProdEC += dblValore;
      }

      if (double.TryParse(txtContoEcValoreProdA3.Text, out dblValore))
      {
        TotValoreProdEC += dblValore;
      }

      if (double.TryParse(txtContoEcValoreProdA4.Text, out dblValore))
      {
        TotValoreProdEC += dblValore;
      }

      if (double.TryParse(txtContoEcValoreProdA5.Text, out dblValore))
      {
        TotValoreProdEC += dblValore;
      }

      if (double.TryParse(txtContoEcValoreProdA5_2.Text, out dblValore))
      {
        TotValoreProdEC += dblValore;
      }

      txtContoEcValoreProdTotale.Text = ConvertNumber(TotValoreProdEC.ToString());

      dblValore = 0.0;
      double.TryParse(txtContoEcValoreProdTotale.Text, out dblValore);

      txtContoEcValoreProdTotalelmin.Text = ConvertNumber((dblValore * ConvertFromPercent(txtContoEcValoreProdTotale_p_min.Text) / 100.0).ToString());
      txtContoEcValoreProdTotalelmindn.Text = ConvertNumber((dblValore * 1.0 / 100.0).ToString());
      if (chkContoEcValoreProdTotale.IsChecked == true)
      {
        TotMinSP += dblValore * ConvertFromPercent(txtContoEcValoreProdTotale_p_min.Text) / 100.0;
        txtContoEcValoreProdTotalelmin.Foreground = Brushes.Black;
        numchecked++;
        numchecked1++;
      }
      else
      {
        txtContoEcValoreProdTotalelmin.Foreground = Brushes.Transparent;
      }
      txtContoEcValoreProdTotalelmax.Text = ConvertNumber((dblValore * ConvertFromPercent(txtContoEcValoreProdTotale_p_max.Text) / 100.0).ToString());
      txtContoEcValoreProdTotalelmaxdn.Text = ConvertNumber((dblValore * 5.0 / 100.0).ToString());
      if (chkContoEcValoreProdTotale.IsChecked == true)
      {
        TotMaxSP += dblValore * ConvertFromPercent(txtContoEcValoreProdTotale_p_max.Text) / 100.0;
        txtContoEcValoreProdTotalelmax.Foreground = Brushes.Black;
      }
      else
      {
        txtContoEcValoreProdTotalelmax.Foreground = Brushes.Transparent;
      }


      #endregion

      dblValore = 0.0;
      double.TryParse(txt1.Text, out dblValore);

      txt1min.Text = ConvertNumber((dblValore * ConvertFromPercent(txt1_p_min.Text) / 100.0).ToString());
      txt1lmindn.Text = ConvertNumber((dblValore * 0.5 / 100.0).ToString());
      if (chk1.IsChecked == true)
      {
        TotMinSP += dblValore * ConvertFromPercent(txt1_p_min.Text) / 100.0;
        txt1min.Foreground = Brushes.Black;
        numchecked++;
        numchecked1++;
      }
      else
      {
        txt1min.Foreground = Brushes.Transparent;
      }
      txt1lmax.Text = ConvertNumber((dblValore * ConvertFromPercent(txt1_p_max.Text) / 100.0).ToString());
      txt1lmaxdn.Text = ConvertNumber((dblValore * 1.0 / 100.0).ToString());
      if (chk1.IsChecked == true)
      {
        TotMaxSP += dblValore * ConvertFromPercent(txt1_p_max.Text) / 100.0;
        txt1lmax.Foreground = Brushes.Black;
      }
      else
      {
        txt1lmax.Foreground = Brushes.Transparent;
      }

      dblValore = 0.0;
      double.TryParse(txt2.Text, out dblValore);

      txt2lmin.Text = ConvertNumber((dblValore * ConvertFromPercent(txt2_p_min.Text) / 100.0).ToString());
      txt2lmindn.Text = ConvertNumber((dblValore * 1.0 / 100.0).ToString());
      if (chk2.IsChecked == true)
      {
        TotMinSP += dblValore * ConvertFromPercent(txt2_p_min.Text) / 100.0;
        txt2lmin.Foreground = Brushes.Black;
        numchecked++;
        numchecked1++;
      }
      else
      {
        txt2lmin.Foreground = Brushes.Transparent;
      }
      txt2lmax.Text = ConvertNumber((dblValore * ConvertFromPercent(txt2_p_max.Text) / 100.0).ToString());
      txt2lmaxdn.Text = ConvertNumber((dblValore * 5.0 / 100.0).ToString());
      if (chk2.IsChecked == true)
      {
        TotMaxSP += dblValore * ConvertFromPercent(txt2_p_max.Text) / 100.0;
        txt2lmax.Foreground = Brushes.Black;
      }
      else
      {
        txt2lmax.Foreground = Brushes.Transparent;
      }

      dblValore = 0.0;
      double.TryParse(txt3.Text, out dblValore);

      txt3lmin.Text = ConvertNumber((dblValore * ConvertFromPercent(txt3_p_min.Text) / 100.0).ToString());
      txt3lmindn.Text = ConvertNumber((dblValore * 0.5 / 100.0).ToString());
      if (chk3.IsChecked == true)
      {
        TotMinEC += dblValore * ConvertFromPercent(txt3_p_min.Text) / 100.0;
        txt3lmin.Foreground = Brushes.Black;
        numchecked++;
        numchecked2++;
      }
      else
      {
        txt3lmin.Foreground = Brushes.Transparent;
      }
      txt3lmax.Text = ConvertNumber((dblValore * ConvertFromPercent(txt3_p_max.Text) / 100.0).ToString());
      txt3lmaxdn.Text = ConvertNumber((dblValore * 1.0 / 100.0).ToString());
      if (chk3.IsChecked == true)
      {
        TotMaxEC += dblValore * ConvertFromPercent(txt3_p_max.Text) / 100.0;
        txt3lmax.Foreground = Brushes.Black;
      }
      else
      {
        txt3lmax.Foreground = Brushes.Transparent;
      }

      dblValore = 0.0;
      double.TryParse(txt4.Text, out dblValore);

      txt4lmin.Text = ConvertNumber((dblValore * ConvertFromPercent(txt4_p_min.Text) / 100.0).ToString());
      txt4lmindn.Text = ConvertNumber((dblValore * 5.0 / 100.0).ToString());
      if (chk4.IsChecked == true)
      {
        TotMinEC += dblValore * ConvertFromPercent(txt4_p_min.Text) / 100.0;
        txt4lmin.Foreground = Brushes.Black;
        numchecked++;
        numchecked2++;
      }
      else
      {
        txt4lmin.Foreground = Brushes.Transparent;
      }
      txt4lmax.Text = ConvertNumber((dblValore * ConvertFromPercent(txt4_p_max.Text) / 100.0).ToString());
      txt4lmaxdn.Text = ConvertNumber((dblValore * 10.0 / 100.0).ToString());
      if (chk4.IsChecked == true)
      {
        TotMaxEC += dblValore * ConvertFromPercent(txt4_p_max.Text) / 100.0;
        txt4lmax.Foreground = Brushes.Black;
      }
      else
      {
        txt4lmax.Foreground = Brushes.Transparent;
      }

      dblValore = 0.0;
      double.TryParse(txt14.Text, out dblValore);

      txt14lmin.Text = ConvertNumber((dblValore * ConvertFromPercent(txt14_p_min.Text) / 100.0).ToString());
      txt14lmindn.Text = ConvertNumber((dblValore * 5.0 / 100.0).ToString());
      if (chk14.IsChecked == true)
      {
        TotMinEC += dblValore * ConvertFromPercent(txt14_p_min.Text) / 100.0;
        txt14lmin.Foreground = Brushes.Black;
        numchecked++;
        numchecked2++;
      }
      else
      {
        txt14lmin.Foreground = Brushes.Transparent;
      }
      txt14lmax.Text = ConvertNumber((dblValore * ConvertFromPercent(txt14_p_max.Text) / 100.0).ToString());
      txt14lmaxdn.Text = ConvertNumber((dblValore * 10.0 / 100.0).ToString());
      if (chk14.IsChecked == true)
      {
        TotMaxEC += dblValore * ConvertFromPercent(txt14_p_max.Text) / 100.0;
        txt14lmax.Foreground = Brushes.Black;
      }
      else
      {
        txt14lmax.Foreground = Brushes.Transparent;
      }

      TotMin = TotMinSP + TotMinEC;
      TotMax = TotMaxSP + TotMaxEC;

      txt5.Text = ConvertNumber(TotMin.ToString());
      txt6.Text = ConvertNumber(TotMax.ToString());

      txt7.Text = ConvertNumber(((TotMax + TotMin) / (numchecked * 2.0)).ToString());



      //txt12.Text = ConvertNumber(((TotMax + TotMin) / (numchecked * 2) * ConvertFromPercent(txt8.Text) / 100.0 * ConvertFromPercent(txt10.Text) / 100.0).ToString());
      //txt13.Text = ConvertNumber(((TotMax + TotMin) / (numchecked * 2)* ConvertFromPercent(txt8.Text) / 100.0 * ConvertFromPercent(txt11.Text) / 100.0).ToString());

      txt12.Text = ConvertNumber(((TotMax + TotMin) / (numchecked * 2.0) * ConvertFromPercent(txt10.Text) / 100.0).ToString());

      double dblValore12 = 0;
      double.TryParse(txt12.Text, out dblValore12);
      txt9.Text = ConvertNumber((dblValore12 * ConvertFromPercent(txt8.Text) / 100.0).ToString());

      txt13.Text = ConvertNumber(((TotMax + TotMin) / (numchecked * 2.0) * ConvertFromPercent(txt11.Text) / 100.0).ToString());

      //Seconda ipotesi
      txt5_2sp.Text = ConvertNumber(TotMinSP.ToString());
      txt6_2sp.Text = ConvertNumber(TotMaxSP.ToString());

      txt7_2sp.Text = ConvertNumber(((TotMaxSP + TotMinSP) / (numchecked1 * 2.0)).ToString());

      //txt9_2sp.Text = ConvertNumber(((TotMaxSP + TotMinSP) / (numchecked1 * 2.0) * ConvertFromPercent(txt8_2sp.Text) / 100.0).ToString());

      //txt12_2sp.Text = ConvertNumber(((TotMaxSP + TotMinSP) / 4.0 * ConvertFromPercent(txt8_2sp.Text) / 100.0 * ConvertFromPercent(txt10_2sp.Text) / 100.0).ToString());
      //txt13_2sp.Text = ConvertNumber(((TotMaxSP + TotMinSP) / 4.0 * ConvertFromPercent(txt8_2sp.Text) / 100.0 * ConvertFromPercent(txt11_2sp.Text) / 100.0).ToString());

      txt12_2sp.Text = ConvertNumber(((TotMaxSP + TotMinSP) / (numchecked1 * 2.0) * ConvertFromPercent(txt10_2sp.Text) / 100.0).ToString());

      dblValore12 = 0;
      double.TryParse(txt12_2sp.Text, out dblValore12);
      txt9_2sp.Text = ConvertNumber((dblValore12 * ConvertFromPercent(txt8_2sp.Text) / 100.0).ToString());

      txt13_2sp.Text = ConvertNumber(((TotMaxSP + TotMinSP) / (numchecked1 * 2.0) * ConvertFromPercent(txt11_2sp.Text) / 100.0).ToString());


      txt5_2ce.Text = ConvertNumber(TotMinEC.ToString());
      txt6_2ce.Text = ConvertNumber(TotMaxEC.ToString());

      txt7_2ce.Text = ConvertNumber(((TotMaxEC + TotMinEC) / (numchecked2 * 2.0)).ToString());

      //txt9_2ce.Text = ConvertNumber(((TotMaxEC + TotMinEC) / (numchecked2 * 2.0) * ConvertFromPercent(txt8_2ce.Text) / 100.0).ToString());

      //txt12_2ce.Text = ConvertNumber(((TotMaxEC + TotMinEC) / 4.0 * ConvertFromPercent(txt8_2ce.Text) / 100.0 * ConvertFromPercent(txt10_2ce.Text) / 100.0).ToString());
      //txt13_2ce.Text = ConvertNumber(((TotMaxEC + TotMinEC) / 4.0 * ConvertFromPercent(txt8_2ce.Text) / 100.0 * ConvertFromPercent(txt11_2ce.Text) / 100.0).ToString());

      txt12_2ce.Text = ConvertNumber(((TotMaxEC + TotMinEC) / (numchecked2 * 2.0) * ConvertFromPercent(txt10_2ce.Text) / 100.0).ToString());

      dblValore12 = 0;
      double.TryParse(txt12_2ce.Text, out dblValore12);
      txt9_2ce.Text = ConvertNumber((dblValore12 * ConvertFromPercent(txt8_2ce.Text) / 100.0).ToString());

      txt13_2ce.Text = ConvertNumber(((TotMaxEC + TotMinEC) / (numchecked2 * 2.0) * ConvertFromPercent(txt11_2ce.Text) / 100.0).ToString());

      //Terza ipotesi
      dblValore = 0.0;
      double.TryParse(txt7_3sp.Text, out dblValore);

      //txt9_3sp.Text = ConvertNumber((dblValore * ConvertFromPercent(txt8_3sp.Text) / 100.0).ToString());
      //txt12_3sp.Text = ConvertNumber((dblValore * ConvertFromPercent(txt8_3sp.Text) / 100.0 * ConvertFromPercent(txt10_3sp.Text) / 100.0).ToString());
      //txt13_3sp.Text = ConvertNumber((dblValore * ConvertFromPercent(txt8_3sp.Text) / 100.0 * ConvertFromPercent(txt11_3sp.Text) / 100.0).ToString());


      txt12_3sp.Text = ConvertNumber((dblValore * ConvertFromPercent(txt10_3sp.Text) / 100.0).ToString());

      dblValore12 = 0;
      double.TryParse(txt12_3sp.Text, out dblValore12);
      txt9_3sp.Text = ConvertNumber((dblValore12 * ConvertFromPercent(txt8_3sp.Text) / 100.0).ToString());

      txt13_3sp.Text = ConvertNumber((dblValore * ConvertFromPercent(txt11_3sp.Text) / 100.0).ToString());

      dblValore = 0.0;
      double.TryParse(txt7_3ec.Text, out dblValore);

      //txt9_3ec.Text = ConvertNumber((dblValore * ConvertFromPercent(txt8_3ec.Text) / 100.0).ToString());
      //txt12_3ec.Text = ConvertNumber((dblValore * ConvertFromPercent(txt8_3ec.Text) / 100.0 * ConvertFromPercent(txt10_3ec.Text) / 100.0).ToString());
      //txt13_3ec.Text = ConvertNumber((dblValore * ConvertFromPercent(txt8_3ec.Text) / 100.0 * ConvertFromPercent(txt11_3ec.Text) / 100.0).ToString());

      txt12_3ec.Text = ConvertNumber((dblValore * ConvertFromPercent(txt10_3ec.Text) / 100.0).ToString());

      dblValore12 = 0;
      double.TryParse(txt12_3ec.Text, out dblValore12);
      txt9_3ec.Text = ConvertNumber((dblValore12 * ConvertFromPercent(txt8_3ec.Text) / 100.0).ToString());

      txt13_3ec.Text = ConvertNumber((dblValore * ConvertFromPercent(txt11_3ec.Text) / 100.0).ToString());

      //DA BILANCIO
      double dblValoreBILANCIO = 0.0;

      double TotMinBILANCIO = 0.0;
      double TotMaxBILANCIO = 0.0;

      double TotMinSPBILANCIO = 0.0;
      double TotMaxSPBILANCIO = 0.0;
      double TotMinECBILANCIO = 0.0;
      double TotMaxECBILANCIO = 0.0;
      double TotValoreProdECBILANCIO = 0.0;

      dblValoreBILANCIO = 0.0;
      double.TryParse(txt1BILANCIO.Text, out dblValoreBILANCIO);

      numchecked = 0.0;






      #region Valore Produzione


      if (double.TryParse(txtContoEcValoreProdA1BILANCIO.Text, out dblValoreBILANCIO))
      {
        TotValoreProdECBILANCIO += dblValoreBILANCIO;
      }

      if (double.TryParse(txtContoEcValoreProdA2BILANCIO.Text, out dblValoreBILANCIO))
      {
        TotValoreProdECBILANCIO += dblValoreBILANCIO;
      }

      if (double.TryParse(txtContoEcValoreProdA3BILANCIO.Text, out dblValoreBILANCIO))
      {
        TotValoreProdECBILANCIO += dblValoreBILANCIO;
      }

      if (double.TryParse(txtContoEcValoreProdA4BILANCIO.Text, out dblValoreBILANCIO))
      {
        TotValoreProdECBILANCIO += dblValoreBILANCIO;
      }

      if (double.TryParse(txtContoEcValoreProdA5BILANCIO.Text, out dblValoreBILANCIO))
      {
        TotValoreProdECBILANCIO += dblValoreBILANCIO;
      }

      if (double.TryParse(txtContoEcValoreProdA5_2BILANCIO.Text, out dblValoreBILANCIO))
      {
        TotValoreProdECBILANCIO += dblValoreBILANCIO;
      }

      txtContoEcValoreProdTotaleBILANCIO.Text = ConvertNumber(TotValoreProdECBILANCIO.ToString());

      dblValoreBILANCIO = 0.0;


      double.TryParse(txtContoEcValoreProdTotaleBILANCIO.Text, out dblValoreBILANCIO);

      txtContoEcValoreProdTotalelminBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * ConvertFromPercent(txtContoEcValoreProdTotaleBILANCIO_p_min.Text) / 100.0).ToString());
      txtContoEcValoreProdTotalelmindnBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * 1.0 / 100.0).ToString());
      if (chkContoEcValoreProdTotaleBILANCIO.IsChecked == true)
      {
        TotMinSPBILANCIO += dblValoreBILANCIO * ConvertFromPercent(txtContoEcValoreProdTotaleBILANCIO_p_min.Text) / 100.0;
        txtContoEcValoreProdTotalelminBILANCIO.Foreground = Brushes.Black;
        numchecked++;
        numchecked1++;
      }
      else
      {
        txtContoEcValoreProdTotalelminBILANCIO.Foreground = Brushes.Transparent;
      }
      txtContoEcValoreProdTotalelmaxBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * ConvertFromPercent(txtContoEcValoreProdTotaleBILANCIO_p_max.Text) / 100.0).ToString());
      txtContoEcValoreProdTotalelmaxdnBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * 5.0 / 100.0).ToString());
      //if (chkContoEcValoreProdTotale.IsChecked == true) // bug segnalato da Leandro, risolto con riga seguente
      if (chkContoEcValoreProdTotaleBILANCIO.IsChecked == true)
      {
        TotMaxSPBILANCIO += dblValoreBILANCIO * ConvertFromPercent(txtContoEcValoreProdTotaleBILANCIO_p_max.Text) / 100.0;
        txtContoEcValoreProdTotalelmaxBILANCIO.Foreground = Brushes.Black;
      }
      else
      {
        txtContoEcValoreProdTotalelmaxBILANCIO.Foreground = Brushes.Transparent;
      }






      #endregion

      dblValoreBILANCIO = 0.0;
      double.TryParse(txt1BILANCIO.Text, out dblValoreBILANCIO);

      txt1minBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * ConvertFromPercent(txt1_p_minBILANCIO.Text) / 100.0).ToString());
      txt1lmindnBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * 0.5 / 100.0).ToString());
      if (chk1BILANCIO.IsChecked == true)
      {
        TotMinSPBILANCIO += dblValoreBILANCIO * ConvertFromPercent(txt1_p_minBILANCIO.Text) / 100.0;
        txt1minBILANCIO.Foreground = Brushes.Black;
        numchecked++;
      }
      else
      {
        txt1minBILANCIO.Foreground = Brushes.Transparent;
      }
      txt1lmaxBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * ConvertFromPercent(txt1_p_maxBILANCIO.Text) / 100.0).ToString());
      txt1lmaxdnBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * 1.0 / 100.0).ToString());
      if (chk1BILANCIO.IsChecked == true)
      {
        TotMaxSPBILANCIO += dblValoreBILANCIO * ConvertFromPercent(txt1_p_maxBILANCIO.Text) / 100.0;
        txt1lmaxBILANCIO.Foreground = Brushes.Black;
      }
      else
      {
        txt1lmaxBILANCIO.Foreground = Brushes.Transparent;
      }

      dblValoreBILANCIO = 0.0;
      double.TryParse(txt2BILANCIO.Text, out dblValoreBILANCIO);

      txt2lminBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * ConvertFromPercent(txt2_p_minBILANCIO.Text) / 100.0).ToString());
      txt2lmindnBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * 1.0 / 100.0).ToString());
      if (chk2BILANCIO.IsChecked == true)
      {
        TotMinSPBILANCIO += dblValoreBILANCIO * ConvertFromPercent(txt2_p_minBILANCIO.Text) / 100.0;
        txt2lminBILANCIO.Foreground = Brushes.Black;
        numchecked++;
      }
      else
      {
        txt2lminBILANCIO.Foreground = Brushes.Transparent;
      }
      txt2lmaxBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * ConvertFromPercent(txt2_p_maxBILANCIO.Text) / 100.0).ToString());
      txt2lmaxdnBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * 5.0 / 100.0).ToString());
      if (chk2BILANCIO.IsChecked == true)
      {
        TotMaxSPBILANCIO += dblValoreBILANCIO * ConvertFromPercent(txt2_p_maxBILANCIO.Text) / 100.0;
        txt2lmaxBILANCIO.Foreground = Brushes.Black;
      }
      else
      {
        txt2lmaxBILANCIO.Foreground = Brushes.Transparent;
      }

      dblValoreBILANCIO = 0.0;
      double.TryParse(txt3BILANCIO.Text, out dblValoreBILANCIO);

      txt3lminBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * ConvertFromPercent(txt3_p_minBILANCIO.Text) / 100.0).ToString());
      txt3lmindnBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * 0.5 / 100.0).ToString());
      if (chk3BILANCIO.IsChecked == true)
      {
        TotMinECBILANCIO += dblValoreBILANCIO * ConvertFromPercent(txt3_p_minBILANCIO.Text) / 100.0;
        txt3lminBILANCIO.Foreground = Brushes.Black;
        numchecked++;
      }
      else
      {
        txt3lminBILANCIO.Foreground = Brushes.Transparent;
      }
      txt3lmaxBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * ConvertFromPercent(txt3_p_maxBILANCIO.Text) / 100.0).ToString());
      txt3lmaxdnBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * 1.0 / 100.0).ToString());
      if (chk3BILANCIO.IsChecked == true)
      {
        TotMaxECBILANCIO += dblValoreBILANCIO * ConvertFromPercent(txt3_p_maxBILANCIO.Text) / 100.0;
        txt3lmaxBILANCIO.Foreground = Brushes.Black;
      }
      else
      {
        txt3lmaxBILANCIO.Foreground = Brushes.Transparent;
      }

      dblValoreBILANCIO = 0.0;
      double.TryParse(txt4BILANCIO.Text, out dblValoreBILANCIO);

      txt4lminBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * ConvertFromPercent(txt4_p_minBILANCIO.Text) / 100.0).ToString());
      txt4lmindnBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * 5.0 / 100.0).ToString());
      if (chk4BILANCIO.IsChecked == true)
      {
        TotMinECBILANCIO += dblValoreBILANCIO * ConvertFromPercent(txt4_p_minBILANCIO.Text) / 100.0;
        txt4lminBILANCIO.Foreground = Brushes.Black;
        numchecked++;
      }
      else
      {
        txt4lminBILANCIO.Foreground = Brushes.Transparent;
      }
      txt4lmaxBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * ConvertFromPercent(txt4_p_maxBILANCIO.Text) / 100.0).ToString());
      txt4lmaxdnBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * 10.0 / 100.0).ToString());
      if (chk4BILANCIO.IsChecked == true)
      {
        TotMaxECBILANCIO += dblValoreBILANCIO * ConvertFromPercent(txt4_p_maxBILANCIO.Text) / 100.0;
        txt4lmaxBILANCIO.Foreground = Brushes.Black;
      }
      else
      {
        txt4lmaxBILANCIO.Foreground = Brushes.Transparent;
      }

      dblValoreBILANCIO = 0.0;
      double.TryParse(txt14BILANCIO.Text, out dblValoreBILANCIO);

      txt14lminBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * ConvertFromPercent(txt14_p_minBILANCIO.Text) / 100.0).ToString());
      txt14lmindnBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * 5.0 / 100.0).ToString());
      if (chk14BILANCIO.IsChecked == true)
      {
        TotMinECBILANCIO += dblValoreBILANCIO * ConvertFromPercent(txt14_p_minBILANCIO.Text) / 100.0;
        txt14lminBILANCIO.Foreground = Brushes.Black;
        numchecked++;
      }
      else
      {
        txt14lminBILANCIO.Foreground = Brushes.Transparent;
      }
      txt14lmaxBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * ConvertFromPercent(txt14_p_maxBILANCIO.Text) / 100.0).ToString());
      txt14lmaxdnBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * 10.0 / 100.0).ToString());
      if (chk14BILANCIO.IsChecked == true)
      {
        TotMaxECBILANCIO += dblValoreBILANCIO * ConvertFromPercent(txt14_p_maxBILANCIO.Text) / 100.0;
        txt14lmaxBILANCIO.Foreground = Brushes.Black;
      }
      else
      {
        txt14lmaxBILANCIO.Foreground = Brushes.Transparent;
      }

      TotMinBILANCIO = TotMinSPBILANCIO + TotMinECBILANCIO;
      TotMaxBILANCIO = TotMaxSPBILANCIO + TotMaxECBILANCIO;

      txt5BILANCIO.Text = ConvertNumber(TotMinBILANCIO.ToString());
      txt6BILANCIO.Text = ConvertNumber(TotMaxBILANCIO.ToString());

      txt7BILANCIO.Text = ConvertNumber(((TotMaxBILANCIO + TotMinBILANCIO) / (numchecked * 2.0)).ToString());

      //txt9BILANCIO.Text = ConvertNumber( ((TotMaxBILANCIO + TotMinBILANCIO) / (numchecked * 2.0) * ConvertFromPercent( txt8BILANCIO.Text ) / 100.0).ToString() );

      //txt12BILANCIO.Text = ConvertNumber( ((TotMaxBILANCIO + TotMinBILANCIO) / 8.0 * ConvertFromPercent( txt8BILANCIO.Text ) / 100.0 * ConvertFromPercent( txt10BILANCIO.Text ) / 100.0).ToString() );
      //txt13BILANCIO.Text = ConvertNumber( ((TotMaxBILANCIO + TotMinBILANCIO) / 8.0 * ConvertFromPercent( txt8BILANCIO.Text ) / 100.0 * ConvertFromPercent( txt11BILANCIO.Text ) / 100.0).ToString() );

      txt12BILANCIO.Text = ConvertNumber(((TotMaxBILANCIO + TotMinBILANCIO) / (numchecked * 2.0) * ConvertFromPercent(txt10.Text) / 100.0).ToString());

      dblValore12 = 0;
      double.TryParse(txt12BILANCIO.Text, out dblValore12);
      txt9BILANCIO.Text = ConvertNumber((dblValore12 * ConvertFromPercent(txt8.Text) / 100.0).ToString());

      txt13BILANCIO.Text = ConvertNumber(((TotMaxBILANCIO + TotMinBILANCIO) / (numchecked * 2.0) * ConvertFromPercent(txt11.Text) / 100.0).ToString());


      //Seconda ipotesi
      txt5_2spBILANCIO.Text = ConvertNumber(TotMinSPBILANCIO.ToString());
      txt6_2spBILANCIO.Text = ConvertNumber(TotMaxSPBILANCIO.ToString());

      txt7_2spBILANCIO.Text = ConvertNumber(((TotMaxSPBILANCIO + TotMinSPBILANCIO) / (numchecked1 * 2.0)).ToString());

      //txt9_2spBILANCIO.Text = ConvertNumber( ((TotMaxSPBILANCIO + TotMinSPBILANCIO) / (numchecked1 * 2.0) * ConvertFromPercent( txt8_2spBILANCIO.Text ) / 100.0).ToString() );

      //txt12_2spBILANCIO.Text = ConvertNumber( ((TotMaxSPBILANCIO + TotMinSPBILANCIO) / 4.0 * ConvertFromPercent( txt8_2spBILANCIO.Text ) / 100.0 * ConvertFromPercent( txt10_2spBILANCIO.Text ) / 100.0).ToString() );
      //txt13_2spBILANCIO.Text = ConvertNumber( ((TotMaxSPBILANCIO + TotMinSPBILANCIO) / 4.0 * ConvertFromPercent( txt8_2spBILANCIO.Text ) / 100.0 * ConvertFromPercent( txt11_2spBILANCIO.Text ) / 100.0).ToString() );

      txt12_2spBILANCIO.Text = ConvertNumber(((TotMaxSPBILANCIO + TotMinSPBILANCIO) / (numchecked1 * 2.0) * ConvertFromPercent(txt10_2sp.Text) / 100.0).ToString());

      dblValore12 = 0;
      double.TryParse(txt12_2spBILANCIO.Text, out dblValore12);
      txt9_2spBILANCIO.Text = ConvertNumber((dblValore12 * ConvertFromPercent(txt8_2sp.Text) / 100.0).ToString());

      txt13_2spBILANCIO.Text = ConvertNumber(((TotMaxSPBILANCIO + TotMinSPBILANCIO) / (numchecked1 * 2.0) * ConvertFromPercent(txt11_2sp.Text) / 100.0).ToString());

      txt5_2ceBILANCIO.Text = ConvertNumber(TotMinECBILANCIO.ToString());
      txt6_2ceBILANCIO.Text = ConvertNumber(TotMaxECBILANCIO.ToString());

      txt7_2ceBILANCIO.Text = ConvertNumber(((TotMaxECBILANCIO + TotMinECBILANCIO) / (numchecked2 * 2.0)).ToString());

      //txt9_2ceBILANCIO.Text = ConvertNumber( ((TotMaxECBILANCIO + TotMinECBILANCIO) / (numchecked2 * 2.0) * ConvertFromPercent( txt8_2ceBILANCIO.Text ) / 100.0).ToString() );

      //txt12_2ceBILANCIO.Text = ConvertNumber( ((TotMaxECBILANCIO + TotMinECBILANCIO) / 4.0 * ConvertFromPercent( txt8_2ceBILANCIO.Text ) / 100.0 * ConvertFromPercent( txt10_2ceBILANCIO.Text ) / 100.0).ToString() );
      //txt13_2ceBILANCIO.Text = ConvertNumber( ((TotMaxECBILANCIO + TotMinECBILANCIO) / 4.0 * ConvertFromPercent( txt8_2ceBILANCIO.Text ) / 100.0 * ConvertFromPercent( txt11_2ceBILANCIO.Text ) / 100.0).ToString() );

      txt12_2ceBILANCIO.Text = ConvertNumber(((TotMaxECBILANCIO + TotMinECBILANCIO) / (numchecked2 * 2.0) * ConvertFromPercent(txt10_2ce.Text) / 100.0).ToString());

      dblValore12 = 0;
      double.TryParse(txt12_2ceBILANCIO.Text, out dblValore12);
      txt9_2ceBILANCIO.Text = ConvertNumber((dblValore12 * ConvertFromPercent(txt8_2ce.Text) / 100.0).ToString());

      txt13_2ceBILANCIO.Text = ConvertNumber(((TotMaxECBILANCIO + TotMinECBILANCIO) / (numchecked2 * 2.0) * ConvertFromPercent(txt11_2ce.Text) / 100.0).ToString());


      //Terza ipotesi
      dblValoreBILANCIO = 0.0;
      double.TryParse(txt7_3spBILANCIO.Text, out dblValoreBILANCIO);

      txt9_3spBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * ConvertFromPercent(txt8_3sp.Text) / 100.0).ToString());
      //txt12_3spBILANCIO.Text = ConvertNumber( (dblValoreBILANCIO * ConvertFromPercent( txt8_3spBILANCIO.Text ) / 100.0 * ConvertFromPercent( txt10_3spBILANCIO.Text ) / 100.0).ToString() );
      //txt13_3spBILANCIO.Text = ConvertNumber( (dblValoreBILANCIO * ConvertFromPercent( txt8_3spBILANCIO.Text ) / 100.0 * ConvertFromPercent( txt11_3spBILANCIO.Text ) / 100.0).ToString() );

      txt12_3spBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * ConvertFromPercent(txt10_3sp.Text) / 100.0).ToString());

      dblValore12 = 0;
      double.TryParse(txt12_3spBILANCIO.Text, out dblValore12);
      txt9_3spBILANCIO.Text = ConvertNumber((dblValore12 * ConvertFromPercent(txt8_3spBILANCIO.Text) / 100.0).ToString());

      txt13_3spBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * ConvertFromPercent(txt11_3sp.Text) / 100.0).ToString());

      dblValoreBILANCIO = 0.0;
      double.TryParse(txt7_3ecBILANCIO.Text, out dblValoreBILANCIO);

      //txt9_3ecBILANCIO.Text = ConvertNumber( (dblValoreBILANCIO * ConvertFromPercent( txt8_3ecBILANCIO.Text ) / 100.0).ToString() );
      //txt12_3ecBILANCIO.Text = ConvertNumber( (dblValoreBILANCIO * ConvertFromPercent( txt8_3ecBILANCIO.Text ) / 100.0 * ConvertFromPercent( txt10_3ecBILANCIO.Text ) / 100.0).ToString() );
      //txt13_3ecBILANCIO.Text = ConvertNumber( (dblValoreBILANCIO * ConvertFromPercent( txt8_3ecBILANCIO.Text ) / 100.0 * ConvertFromPercent( txt11_3ecBILANCIO.Text ) / 100.0).ToString() );
      txt12_3ecBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * ConvertFromPercent(txt10_3ec.Text) / 100.0).ToString());

      dblValore12 = 0;
      double.TryParse(txt12_3ecBILANCIO.Text, out dblValore12);
      txt9_3ecBILANCIO.Text = ConvertNumber((dblValore12 * ConvertFromPercent(txt8_3ec.Text) / 100.0).ToString());

      txt13_3ecBILANCIO.Text = ConvertNumber((dblValoreBILANCIO * ConvertFromPercent(txt11_3ec.Text) / 100.0).ToString());


      if (_ipotesi == IpotesiMaterialita.Prima)
      {
        txt1_p_minBILANCIO.Text = txt1_p_min.Text;
        txt2_p_minBILANCIO.Text = txt2_p_min.Text;
        txt3_p_minBILANCIO.Text = txt3_p_min.Text;
        txt4_p_minBILANCIO.Text = txt4_p_min.Text;
        txt14_p_minBILANCIO.Text = txt14_p_min.Text;

        txtContoEcValoreProdTotaleBILANCIO_p_min.Text = txtContoEcValoreProdTotale_p_min.Text;

        txt1_p_maxBILANCIO.Text = txt1_p_max.Text;
        txt2_p_maxBILANCIO.Text = txt2_p_max.Text;
        txt3_p_maxBILANCIO.Text = txt3_p_max.Text;
        txt4_p_maxBILANCIO.Text = txt4_p_max.Text;
        txt14_p_maxBILANCIO.Text = txt14_p_max.Text;

        txtContoEcValoreProdTotaleBILANCIO_p_max.Text = txtContoEcValoreProdTotale_p_max.Text;

        rbtTipoMaterialitaBilancio1.IsChecked = rbtTipoMaterialitaPianificata1.IsChecked;
        rbtTipoMaterialitaBilancio2.IsChecked = rbtTipoMaterialitaPianificata2.IsChecked;


      }

      if (rbtTipoMaterialitaPianificata1.IsChecked.HasValue && rbtTipoMaterialitaPianificata1.IsChecked.Value)
      {
        brdPrima.BorderBrush = Brushes.Green;
        brdRadioPrimaPianificata.BorderBrush = Brushes.Green;
      }
      else
      {
        brdPrima.BorderBrush = App._arrBrushes[0];
        brdRadioPrimaPianificata.BorderBrush = App._arrBrushes[0];
      }

      if (rbtTipoMaterialitaPianificata2.IsChecked.HasValue && rbtTipoMaterialitaPianificata2.IsChecked.Value)
      {
        brdSeconda.BorderBrush = Brushes.Green;
        brdRadioSecondaPianificata.BorderBrush = Brushes.Green;

      }
      else
      {
        brdSeconda.BorderBrush = App._arrBrushes[0];
        brdRadioSecondaPianificata.BorderBrush = App._arrBrushes[0];
      }

      if (rbtTipoMaterialitaBilancio1.IsChecked.HasValue && rbtTipoMaterialitaBilancio1.IsChecked.Value)
      {
        brdPrimaBILANCIO.BorderBrush = Brushes.Green;
        brdRadioPrimaBILANCIO.BorderBrush = Brushes.Green;
      }
      else
      {
        brdPrimaBILANCIO.BorderBrush = App._arrBrushes[0];
        brdRadioPrimaBILANCIO.BorderBrush = App._arrBrushes[0];
      }

      if (rbtTipoMaterialitaBilancio2.IsChecked.HasValue && rbtTipoMaterialitaBilancio2.IsChecked.Value)
      {
        brdSecondaBILANCIO.BorderBrush = Brushes.Green;
        brdRadioSecondaBILANCIO.BorderBrush = Brushes.Green;
      }
      else
      {
        brdSecondaBILANCIO.BorderBrush = App._arrBrushes[0];
        brdRadioSecondaBILANCIO.BorderBrush = App._arrBrushes[0];
      }
    }

    private double GetValoreEA(string Cella)
    {
      double returnvalue = 0.0;

      if (SommeDaExcel.Contains(Cella))
      {
        foreach (string ID in SommeDaExcel[Cella].ToString().Split('|'))
        {
          double dblValore = 0.0;

          if (valoreEA.Contains(ID))
          {
            double.TryParse(valoreEA[ID].ToString(), out dblValore);
          }

          returnvalue += dblValore;
        }
      }

      return returnvalue;
    }

    private void RetrieveData(string ID)
    {
      #region originale
      //string idsessionebilancio = "";
      //if(IDTree=="1")
      //idsessionebilancio = cBusinessObjects.CercaSessione("Revisione", "Bilancio", cBusinessObjects.idsessione.ToString(), cBusinessObjects.idcliente);
      //else
      //idsessionebilancio = cBusinessObjects.CercaSessione("Conclusione", "Bilancio", cBusinessObjects.idsessione.ToString(), cBusinessObjects.idcliente);

      //DataTable datibilancio = cBusinessObjects.GetData(int.Parse(ID), typeof(Excel_Bilancio), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);



      //if (datibilancio.Rows.Count > 0)
      //{
      //  foreach (DataRow dtrow in datibilancio.Rows)
      //  {
      //    //Calcolo valori attuali

      //    if (dtrow["EA"].ToString() != "")
      //    {
      //      valoreEA.Add(dtrow["ID"].ToString(), dtrow["EA"].ToString());
      //    }
      //    else
      //    {
      //      valoreEA.Add(dtrow["ID"].ToString(), "0");
      //    }
      //  }
      //}
      #endregion

      //----------------------------------------------------------------------+
      //         il codice della region precedente è stato sostituito         |
      //                    da questo - Enrico 2019.09.12                     |
      //----------------------------------------------------------------------+
      DataTable datiBilancio;
      int idSessioneBilancio;

      idSessioneBilancio = int.Parse(
        cBusinessObjects.CercaSessione(
          (IDTree == "1") ? "Revisione" : "Conclusione",
          "Bilancio", cBusinessObjects.idsessione.ToString(),
          cBusinessObjects.idcliente));
      if (idSessioneBilancio < 0) return;
      datiBilancio = cBusinessObjects.GetData(int.Parse(ID),
        typeof(Excel_Bilancio), cBusinessObjects.idcliente,
        idSessioneBilancio, 4);
      if (datiBilancio.Rows.Count < 1) return;
      foreach (DataRow dtrow in datiBilancio.Rows)
      {
        if (dtrow["EA"].ToString() != "")
        {
          valoreEA.Add(dtrow["ID"].ToString(), dtrow["EA"].ToString());
        }
        else
        {
          valoreEA.Add(dtrow["ID"].ToString(), "0");
        }
      }
    }

    private string ConvertNumber(string valore)
    {
      double dblValore = 0.0;

      double.TryParse(valore, out dblValore);

      if (dblValore == 0.0)
      {
        return "";
      }
      else
      {
        return String.Format("{0:#,#}", dblValore);
      }
    }

    private string ConvertPercent(string valore)
    {
      double dblValore = 0.0;

      valore = valore.Replace("%", "");

      double.TryParse(valore, out dblValore);

      if (dblValore == 0.0)
      {
        return "";
      }
      else
      {
        return String.Format("{0:0.00}", dblValore) + "%";
      }
    }

    private string ConvertPercentIntero(string valore)
    {
      double dblValore = 0.0;

      valore = valore.Replace("%", "");

      double.TryParse(valore, out dblValore);

      if (dblValore == 0.0)
      {
        return "";
      }
      else
      {
        return String.Format("{0:0}", dblValore) + "%";
      }
    }

    private double ConvertFromPercent(string valore)
    {
      double dblValore = 0.0;

      valore = valore.Replace("%", "");

      double.TryParse(valore, out dblValore);

      return dblValore;
    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      double newsize = e.NewSize.Width - 30.0;

      try
      {
        foreach (UIElement item in stack.Children)
        {
          ((UserControl)(((Grid)(((Border)(item)).Child)).Children[2])).Width = newsize - 30;
        }

        stack.Width = Convert.ToDouble(newsize);
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
    }

    private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      Image i = ((Image)sender);
      UserControl u = ((UserControl)(((Grid)(i.Parent)).Children[2]));

      if (u.Visibility == System.Windows.Visibility.Collapsed)
      {
        u.Visibility = System.Windows.Visibility.Visible;
        var uriSource = new Uri(down, UriKind.Relative);
        i.Source = new BitmapImage(uriSource);
      }
      else
      {
        u.Visibility = System.Windows.Visibility.Collapsed;
        var uriSource = new Uri(left, UriKind.Relative);
        i.Source = new BitmapImage(uriSource);
      }
    }

    private void txt_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (!_isUpdatingData)
      {
        _isUpdatingData = true;
        UpdateData();
        _isUpdatingData = false;
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
    }

    private void txt_LostFocus(object sender, RoutedEventArgs e)
    {
      ((TextBox)(sender)).Text = ConvertNumber(((TextBox)(sender)).Text);
    }

    private void txt_LostFocus_perc(object sender, RoutedEventArgs e)
    {
      ((TextBox)(sender)).Text = ConvertPercent(((TextBox)(sender)).Text);
      if (!_isUpdatingData)
      {
        _isUpdatingData = true;
        UpdateData();
        _isUpdatingData = false;
      }
    }

    private void txt_LostFocus_perc_int(object sender, RoutedEventArgs e)
    {
      ((TextBox)(sender)).Text = ConvertPercentIntero(((TextBox)(sender)).Text);
      if (!_isUpdatingData)
      {
        _isUpdatingData = true;
        UpdateData();
        _isUpdatingData = false;
      }
    }

    private void chk_CheckedChanged(object sender, RoutedEventArgs e)
    {
      CheckBox chk = sender as CheckBox;
      if (chk != null)
      {
        switch (chk.Name)
        {
          case "chk4":
            if (chk.IsChecked.HasValue && chk.IsChecked.Value)
            {
              if (chk14 != null)
                chk14.IsChecked = false;
              //if(chk14 != null && chk14.IsChecked.HasValue && chk14.IsChecked.Value)
              //{
              //    //MessageBox.Show("Il risultato operativo e il risultato ante imposte non possono essere selezionati contemporaneamente!");
              //    //e.Handled = true;
              //    //chk.IsChecked = false;

              //    return;
              //}
            }
            break;
          case "chk14":
            if (chk.IsChecked.HasValue && chk.IsChecked.Value)
            {
              if (chk4 != null)
                chk4.IsChecked = false;

              //if (chk4 != null && chk4.IsChecked.HasValue && chk4.IsChecked.Value)
              //{
              //    chk4.IsChecked = false;
              //    //MessageBox.Show("Il risultato operativo e il risultato ante imposte non possono essere selezionati contemporaneamente!");
              //    //e.Handled = true;
              //    //chk.IsChecked = false;
              //    return;
              //}
            }
            break;
          case "chkContoEcValoreProdTotale":
            if (chk.IsChecked.HasValue && chk.IsChecked.Value)
            {
              if (chk3 != null)
                chk3.IsChecked = false;
              //if(chk14 != null && chk14.IsChecked.HasValue && chk14.IsChecked.Value)
              //{
              //    //MessageBox.Show("Il risultato operativo e il risultato ante imposte non possono essere selezionati contemporaneamente!");
              //    //e.Handled = true;
              //    //chk.IsChecked = false;

              //    return;
              //}
            }
            break;
          case "chk3":
            if (chk.IsChecked.HasValue && chk.IsChecked.Value)
            {
              if (chkContoEcValoreProdTotale != null)
                chkContoEcValoreProdTotale.IsChecked = false;

              //if (chk4 != null && chk4.IsChecked.HasValue && chk4.IsChecked.Value)
              //{
              //    chk4.IsChecked = false;
              //    //MessageBox.Show("Il risultato operativo e il risultato ante imposte non possono essere selezionati contemporaneamente!");
              //    //e.Handled = true;
              //    //chk.IsChecked = false;
              //    return;
              //}
            }
            break;
          //case "chk4BILANCIO":
          //    if (chk.IsChecked.HasValue && chk.IsChecked.Value)
          //    {
          //        if (chk14BILANCIO != null)
          //            chk14BILANCIO.IsChecked = false;
          //        //if(chk14 != null && chk14.IsChecked.HasValue && chk14.IsChecked.Value)
          //        //{
          //        //    //MessageBox.Show("Il risultato operativo e il risultato ante imposte non possono essere selezionati contemporaneamente!");
          //        //    //e.Handled = true;
          //        //    //chk.IsChecked = false;

          //        //    return;
          //        //}
          //    }
          //    break;
          //case "chk14BILANCIO":
          //    if (chk.IsChecked.HasValue && chk.IsChecked.Value)
          //    {
          //        if (chk4BILANCIO != null)
          //            chk4BILANCIO.IsChecked = false;

          //        //if (chk4 != null && chk4.IsChecked.HasValue && chk4.IsChecked.Value)
          //        //{
          //        //    chk4.IsChecked = false;
          //        //    //MessageBox.Show("Il risultato operativo e il risultato ante imposte non possono essere selezionati contemporaneamente!");
          //        //    //e.Handled = true;
          //        //    //chk.IsChecked = false;
          //        //    return;
          //        //}
          //    }
          //break;
          default:
            break;
        }


        if (_ipotesi == IpotesiMaterialita.Terza)
        {
          switch (chk.Name)
          {
            case "chk4BILANCIO":
              if (chk.IsChecked.HasValue && chk.IsChecked.Value)
              {
                if (chk14BILANCIO != null)
                  chk14BILANCIO.IsChecked = false;

              }
              break;
            case "chk14BILANCIO":
              if (chk.IsChecked.HasValue && chk.IsChecked.Value)
              {
                if (chk4BILANCIO != null)
                  chk4BILANCIO.IsChecked = false;


              }
              break;
            case "chkContoEcValoreProdTotaleBILANCIO":
              if (chk.IsChecked.HasValue && chk.IsChecked.Value)
              {
                if (chk3BILANCIO != null)
                  chk3BILANCIO.IsChecked = false;

              }
              break;
            case "chk3BILANCIO":
              if (chk.IsChecked.HasValue && chk.IsChecked.Value)
              {
                if (chkContoEcValoreProdTotaleBILANCIO != null)
                  chkContoEcValoreProdTotaleBILANCIO.IsChecked = false;


              }
              break;

            default:
              break;
          }
        }

      }




      if (!_isUpdatingData)
      {
        _isUpdatingData = true;
        UpdateData();
        _isUpdatingData = false;
      }
    }

    private void rbtTipo_Checked(object sender, RoutedEventArgs e)
    {
      if (!_isUpdatingData)
      {
        _isUpdatingData = true;
        UpdateData();
        _isUpdatingData = false;
      }
    }

    private void rbtTipo_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (_ReadOnly)
      {
        e.Handled = true;
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");

        return;
      }
    }
  }
}
