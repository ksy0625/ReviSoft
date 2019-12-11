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
using System.Data;

namespace UserControls
{
  public partial class ucCicli : UserControl
  {
    public int id;
    private DataTable dati = null;

    private string check = "./Images/icone/Stato/check2.png";
    private string uncheck = "./Images/icone/Stato/nothing.png";

    private XmlDataProviderManager _x;
    private string _ID = "-1";

    Hashtable Sessioni = new Hashtable();
    Hashtable SessioniTitoli = new Hashtable();
    Hashtable SessioniID = new Hashtable();
    int SessioneNow;
    string IDTree;
    string IDCliente;
    string IDSessione;

    public ucCicli()
    {
      InitializeComponent();
    }

    //----------------------------------------------------------------------------+
    //                                    Load                                    |
    //----------------------------------------------------------------------------+
    public void Load(ref XmlDataProviderManager x, string ID, string FileRevisione,
      Hashtable _Sessioni, Hashtable _SessioniTitoli, Hashtable _SessioniID, int
      _SessioneNow, string _IDTree, string _IDCliente, string _IDSessione)
    {
      AltoMedioBasso valore;
      DataTable dt;
      Image img;
      int i, id_scheda, j, k, row;
      string qry, val, xml;
      TextBlock txt;
      Uri uriSource;

      //----------------------------------------------- recupero dati da 2.9.1 .. 5
      id = int.Parse(ID.ToString());
      id_scheda = cBusinessObjects.GetIDTree(id);
      cBusinessObjects.idcliente = int.Parse(_IDCliente.ToString());
      cBusinessObjects.idsessione = int.Parse(_IDSessione.ToString());
      qry = string.Format(
        "select ID_SCHEDA,value from clsAltoMedioBasso " +
        "where (ID_SCHEDA in (10000205,10000217,10000218,10000219,10000220)) " +
          "and (ID_CLIENTE={0}) and (ID_SESSIONE={1}) " +
        "order by ID_SCHEDA ",
        cBusinessObjects.idcliente, cBusinessObjects.idsessione);
      dt = cBusinessObjects.ExecutesqlDataTable(qry);

      Sessioni = _Sessioni;
      SessioniTitoli = _SessioniTitoli;
      SessioniID = _SessioniID;
      SessioneNow = _SessioneNow;
      IDTree = _IDTree;
      IDCliente = _IDCliente;
      IDSessione = _IDSessione;

      _x = x.Clone();
      _ID = ID;

      ArrayList Nodi = new ArrayList();
      Nodi.Add("205");
      Nodi.Add("217");
      Nodi.Add("218");
      Nodi.Add("219");
      Nodi.Add("220");

      Hashtable NodiAlias = new Hashtable();
      NodiAlias.Add("205", "204");
      NodiAlias.Add("217", "213");
      NodiAlias.Add("218", "214");
      NodiAlias.Add("219", "215");
      NodiAlias.Add("220", "216");

      #region xaml_objects1

      row = 1;

      Grid grd = new Grid();
      ColumnDefinition cd = new ColumnDefinition();
      cd.Width = GridLength.Auto;
      grd.ColumnDefinitions.Add(cd);
      cd = new ColumnDefinition();
      cd.Width = new GridLength(1, GridUnitType.Star);
      grd.ColumnDefinitions.Add(cd);
      cd = new ColumnDefinition();
      cd.Width = new GridLength(1, GridUnitType.Star);
      grd.ColumnDefinitions.Add(cd);
      cd = new ColumnDefinition();
      cd.Width = new GridLength(1, GridUnitType.Star);
      grd.ColumnDefinitions.Add(cd);

      RowDefinition rd = new RowDefinition();
      grd.RowDefinitions.Add(rd);

      txt = new TextBlock();
      grd.Children.Add(txt);
      Grid.SetRow(txt, 0);
      Grid.SetColumn(txt, 0);

      Border brd = new Border();
      brd.BorderThickness = new Thickness(1.0);
      brd.BorderBrush = Brushes.LightGray;
      brd.Background = Brushes.LightGray;
      brd.Padding = new Thickness(2.0);

      txt = new TextBlock();
      txt.Text = "Alto";
      txt.FontSize = 14;
      txt.TextAlignment = TextAlignment.Center;
      txt.FontWeight = FontWeights.Bold;
      txt.Margin = new Thickness(0, 0, 0, 10);

      brd.Child = txt;

      grd.Children.Add(brd);
      Grid.SetRow(brd, 0);
      Grid.SetColumn(brd, 1);

      brd = new Border();
      brd.BorderThickness = new Thickness(1.0);
      brd.BorderBrush = Brushes.LightGray;
      brd.Background = Brushes.LightGray;
      brd.Padding = new Thickness(2.0);

      txt = new TextBlock();
      txt.Text = "Medio";
      txt.FontSize = 14;
      txt.TextAlignment = TextAlignment.Center;
      txt.FontWeight = FontWeights.Bold;
      txt.Margin = new Thickness(0, 0, 0, 10);

      brd.Child = txt;

      grd.Children.Add(brd);
      Grid.SetRow(brd, 0);
      Grid.SetColumn(brd, 2);

      brd = new Border();
      brd.BorderThickness = new Thickness(1.0);
      brd.BorderBrush = Brushes.LightGray;
      brd.Background = Brushes.LightGray;
      brd.Padding = new Thickness(2.0);

      txt = new TextBlock();
      txt.Text = "Basso";
      txt.FontSize = 14;
      txt.TextAlignment = TextAlignment.Center;
      txt.FontWeight = FontWeights.Bold;
      txt.Margin = new Thickness(0, 0, 0, 10);

      brd.Child = txt;

      grd.Children.Add(brd);
      Grid.SetRow(brd, 0);
      Grid.SetColumn(brd, 3);

      #endregion

      //---------------------------------------------------------- scansione valori
      for (i = 0; i < 5; i++)
      {
        //---------------------------------------------------------- lettura valore
        k = -1; valore = AltoMedioBasso.Sconosciuto;
        for (j = 0; j < dt.Rows.Count && k < 0; j++)
        {
          if (dt.Rows[j].ItemArray[0].ToString().EndsWith(Nodi[i].ToString()))
            k = j;
        }
        if (k > -1)
        {
          val = dt.Rows[k].ItemArray[1].ToString();
          if (!string.IsNullOrEmpty(val))
            valore = (AltoMedioBasso)Convert.ToInt32(val);
        }

        //---------------------------------------------- impostazione prima colonna
        rd = new RowDefinition();
        grd.RowDefinitions.Add(rd);

        RevisoftApplication.XmlManager xt = new XmlManager();
        xt.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
        XmlDataProvider TreeXmlProvider = new XmlDataProvider();
        TreeXmlProvider.Document = xt.LoadEncodedFile(App.AppTemplateTreeRevisione);
        XmlNode tnode = TreeXmlProvider.Document.SelectSingleNode(
          "/Tree//Node[@ID=" + NodiAlias[Nodi[i]].ToString() + "]");

        brd = new Border();
        brd.BorderThickness = new Thickness(1.0);
        brd.BorderBrush = Brushes.LightGray;
        brd.Background = (row % 2 == 0) ?
          new SolidColorBrush(Color.FromArgb(126, 241, 241, 241)) : Brushes.White;
        brd.Padding = new Thickness(2.0);

        txt = new TextBlock();
        txt.Text = tnode.Attributes["Codice"].Value + "\t"
          + tnode.Attributes["Titolo"].Value;
        txt.ToolTip = "Fare Doppio CLick per aprire la Carta di lavoro "
          + tnode.Attributes["Codice"].Value;
        txt.MouseDown += new MouseButtonEventHandler(txt_MouseDownCicli);
        txt.FontSize = 13;

                //---------------------------------------------- impostazione nodo "Valore"
                XmlNode nodeNodo = null;
     //   XmlNode nodeNodo = _x.Document.SelectSingleNode(
       //   "/Dati//Dato[@ID='" + _ID + "']/Valore[@ID='" + Nodi[i] + "']");

        if (nodeNodo == null)
        {
          xml = "<Valore ID='" + Nodi[i].ToString() + "'/>";
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode_int = doctmp.SelectSingleNode("Valore");
          XmlNode node_imp = _x.Document.ImportNode(tmpNode_int, true);
          _x.Document.SelectSingleNode(
            "/Dati//Dato[@ID='" + _ID + "']").AppendChild(node_imp);
          nodeNodo = _x.Document.SelectSingleNode(
            "/Dati//Dato[@ID='" + _ID + "']/Valore[@ID='"
              + Nodi[i].ToString() + "']");
        }

        if (nodeNodo.Attributes["name"] == null)
        {
          XmlAttribute attr = _x.Document.CreateAttribute("name");
          nodeNodo.Attributes.Append(attr);
        }
        nodeNodo.Attributes["name"].Value = tnode.Attributes["Codice"].Value
          + " " + tnode.Attributes["Titolo"].Value;

        if (nodeNodo.Attributes["value"] == null)
        {
          XmlAttribute attr = _x.Document.CreateAttribute("value");
          nodeNodo.Attributes.Append(attr);
        }
        nodeNodo.Attributes["value"].Value = (Convert.ToInt32(valore)).ToString();

        #region xaml_objects2

        brd.Child = txt;
        grd.Children.Add(brd);
        Grid.SetRow(brd, row);
        Grid.SetColumn(brd, 0);

        brd = new Border();
        brd.BorderThickness = new Thickness(1.0);
        brd.BorderBrush = Brushes.LightGray;
        brd.Background = (row % 2 == 0) ?
          new SolidColorBrush(Color.FromArgb(126, 241, 241, 241)) : Brushes.White;
        brd.Padding = new Thickness(2.0);

        img = new Image();
        uriSource = (valore == AltoMedioBasso.Alto) ?
          new Uri(check, UriKind.Relative) : new Uri(uncheck, UriKind.Relative);
        img.Source = new BitmapImage(uriSource);
        img.Width = 16.0;
        brd.Child = img;
        grd.Children.Add(brd);
        Grid.SetRow(brd, row);
        Grid.SetColumn(brd, 1);

        brd = new Border();
        brd.BorderThickness = new Thickness(1.0);
        brd.BorderBrush = Brushes.LightGray;
        brd.Background = (row % 2 == 0) ?
          new SolidColorBrush(Color.FromArgb(126, 241, 241, 241)) : Brushes.White;
        brd.Padding = new Thickness(2.0);

        img = new Image();
        uriSource = (valore == AltoMedioBasso.Medio) ?
          new Uri(check, UriKind.Relative) : new Uri(uncheck, UriKind.Relative);
        img.Source = new BitmapImage(uriSource);
        img.Width = 16.0;
        brd.Child = img;
        grd.Children.Add(brd);
        Grid.SetRow(brd, row);
        Grid.SetColumn(brd, 2);

        brd = new Border();
        brd.BorderThickness = new Thickness(1.0);
        brd.BorderBrush = Brushes.LightGray;
        brd.Background = (row % 2 == 0) ?
          new SolidColorBrush(Color.FromArgb(126, 241, 241, 241)) : Brushes.White;
        brd.Padding = new Thickness(2.0);

        img = new Image();
        uriSource = (valore == AltoMedioBasso.Basso) ?
          new Uri(check, UriKind.Relative) : new Uri(uncheck, UriKind.Relative);
        img.Source = new BitmapImage(uriSource);
        img.Width = 16.0;
        brd.Child = img;
        grd.Children.Add(brd);
        Grid.SetRow(brd, row);
        Grid.SetColumn(brd, 3);

        #endregion

        row++;
      }
      brdMain.Child = grd;
    }

    public XmlDataProviderManager Save()
    {
      _x.Save();
      return _x;
    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      double newsize = e.NewSize.Width - 30.0;

      try
      {
        brdMain.Width = Convert.ToDouble(newsize);
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
    }

    void txt_MouseDownCicli(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount == 2)
      {
        MasterFile mf = MasterFile.Create();

        Hashtable revisioneNow = mf.GetRevisioneFromFileData(Sessioni[SessioneNow].ToString());
        string revisioneAssociata = App.AppDataDataFolder + "\\" + revisioneNow["FileData"].ToString();
        string revisioneTreeAssociata = App.AppDataDataFolder + "\\" + revisioneNow["File"].ToString();
        string revisioneIDAssociata = revisioneNow["ID"].ToString();
        string IDCliente = revisioneNow["Cliente"].ToString();

        if (revisioneAssociata == "")
        {
          e.Handled = true;
          return;
        }

        XmlDataProviderManager _xNew = new XmlDataProviderManager(revisioneAssociata);

        WindowWorkArea wa = new WindowWorkArea(ref _xNew);

        //Nodi
        wa.NodeHome = 0;

        RevisoftApplication.XmlManager xt = new XmlManager();
        xt.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
        XmlDataProvider TreeXmlProvider = new XmlDataProvider();
        TreeXmlProvider.Document = xt.LoadEncodedFile(revisioneTreeAssociata);

        if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
        {
          foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
          {
            if (item.Attributes["Codice"].Value == ((TextBlock)(sender)).ToolTip.ToString().Replace("Fare Doppio CLick per aprire la Carta di lavoro ", ""))
            {
              wa.Nodes.Add(0, item);
            }
          }
        }

        if (wa.Nodes.Count == 0)
        {
          e.Handled = true;
          return;
        }

        wa.NodeNow = wa.NodeHome;

        wa.Owner = Window.GetWindow(this);

        //posizione e dimensioni finestra
        wa.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        wa.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        wa.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        wa.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        wa.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        wa.MinHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        wa.MinWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;

        //Sessioni
        wa.Sessioni.Clear();
        wa.Sessioni.Add(0, revisioneAssociata);

        wa.SessioniTitoli.Clear();
        wa.SessioniTitoli.Add(0, "");

        wa.SessioniID.Clear();
        wa.SessioniID.Add(0, revisioneIDAssociata);

        wa.SessioneHome = 0;
        wa.SessioneNow = 0;

        //Variabili
        wa.ReadOnly = true;
        wa.ReadOnlyOLD = true;
        wa.ApertoInSolaLettura = true;

        //passaggio dati
        wa.IDTree = IDTree;
        wa.IDSessione = revisioneIDAssociata;
        wa.IDCliente = IDCliente;

        wa.Stato = App.TipoTreeNodeStato.Sconosciuto;
        wa.OldStatoNodo = wa.Stato;

        //apertura
        wa.Load();

        wa.ShowDialog();
      }
    }
  }
}
