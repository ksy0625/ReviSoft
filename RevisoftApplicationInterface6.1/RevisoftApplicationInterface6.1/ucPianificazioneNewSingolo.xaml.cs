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
  public partial class ucPianificazioneNewSingolo : UserControl
  {
    public int id;
    private DataTable dati = null;
    string idsessionerevisione = "";
    private string check = "./Images/icone/ana_stato_ok_blu.png";
    private string uncheck = "./Images/icone/check1-24x24.png";
    private string disabled = "./Images/icone/disabled.png";

    private string up = "./Images/icone/navigate_up.png";
    private string down = "./Images/icone/navigate_down.png";
    private string left = "./Images/icone/navigate_left.png";

    private XmlDataProviderManager _x;
    //private XmlDataProviderManager _y;
    private string _ID = "-1";
    private string IDRischioGlobale = "22";
    private string IDPianificazione = "246";

    Hashtable Sessioni = new Hashtable();
    Hashtable SessioniTitoli = new Hashtable();
    Hashtable SessioniID = new Hashtable();
    int SessioneNow;
    string IDTree;
    string IDCliente;

    string IDSessione;
    private bool _ReadOnly = true;

    public Hashtable visiblenode = new Hashtable();
    public ArrayList visiblenodeID = new ArrayList();
    public bool estesa = false;

    public bool ReadOnly
    {
      set
      {
        _ReadOnly = true;
      }
    }

    public ucPianificazioneNewSingolo()
    {
      InitializeComponent();
    }

    public void Load(string ID, string FileDataBilancio, Hashtable _Sessioni, Hashtable _SessioniTitoli, Hashtable _SessioniID, int _SessioneNow, string _IDTree, string _IDCliente, string _IDSessione)
    {
      id = int.Parse(ID);

      cBusinessObjects.idcliente = int.Parse(_IDCliente.ToString());
      cBusinessObjects.idsessione = int.Parse(_IDSessione.ToString());

      Sessioni = _Sessioni;
      SessioniTitoli = _SessioniTitoli;
      SessioniID = _SessioniID;
      SessioneNow = _SessioneNow;
      IDTree = _IDTree;
      IDCliente = _IDCliente;
      IDSessione = _IDSessione;



      _ID = ID;
      DataRow xnodehere = null;
      dati = cBusinessObjects.GetData(id, typeof(PianificazioneNewSingolo));

      foreach (DataRow dd in dati.Rows)
      {

        xnodehere = dd;

      }
      if (xnodehere == null)
        xnodehere = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);



      MasterFile mf = MasterFile.Create();
      //XmlDataProviderManager _lm;

      TextBlock txt;

      idsessionerevisione = cBusinessObjects.CercaSessione("Bilancio", "Revisione", IDSessione, cBusinessObjects.idcliente);


      if (idsessionerevisione == "-1")
      {
        _x = null;
        txt = new TextBlock();
        txt.Text = "Pianificazione non eseguita.";
        txt.Margin = new Thickness(5, 5, 0, 0);
        brdDefinizione.Children.Clear();
        brdDefinizione.Children.Add(txt);
        return;
      }

      DataRow pianificazionenode = null;

      DataTable datiP = cBusinessObjects.GetData(274, typeof(PianificazioneNewWD_Node), cBusinessObjects.idcliente, int.Parse(idsessionerevisione), 1);

      SortedDictionary<int, string> VociBilancio = new SortedDictionary<int, string>();
      switch (ID)
      {
        case "126":
          VociBilancio.Add(80, "3.4.1@Immobilizzazioni immateriali@1@1@1@1@1@1@1@0@pv");
          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "80")
            {
              pianificazionenode = dd;
            }
          }
          break;
        case "129":
          VociBilancio.Add(81, "3.4.2@Immobilizzazioni materiali@1@1@1@1@1@1@1@0@txt3c");
          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "81")
            {
              pianificazionenode = dd;
            }
          }
          break;
        case "130":
          VociBilancio.Add(82, "3.4.3@Immobilizzazioni finanziarie@1@1@1@1@1@1@1@0@pv");

          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "82")
            {
              pianificazionenode = dd;
            }
          }
          break;
        case "131":
          VociBilancio.Add(83, "3.4.4@Rimanenze di Magazzino@1@1@1@1@1@1@1@0@txt4c");

          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "83")
            {
              pianificazionenode = dd;
            }
          }
          break;
        case "132":
          VociBilancio.Add(85, "3.4.5@Rimanenze - Opere a lungo termine@1@1@1@1@1@1@1@0@txt4c");
          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "85")
            {
              pianificazionenode = dd;
            }
          }
          break;
        case "133":
          VociBilancio.Add(86, "3.4.6@Attività finanziarie non immobilizzate@1@1@1@1@1@1@1@0@txt5c");
          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "86")
            {
              pianificazionenode = dd;
            }
          }
          break;
        case "134":
          VociBilancio.Add(87, "3.4.7@Crediti commerciali (Clienti)@1@1@1@1@1@1@1@0@txt2c");

          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "87")
            {
              pianificazionenode = dd;
            }
          }
          break;
        case "135":
          VociBilancio.Add(88, "3.4.8@Crediti e debiti infragruppo@1@1@1@1@1@1@1@0@pv");

          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "88")
            {
              pianificazionenode = dd;
            }
          }
          break;
        case "136":
          VociBilancio.Add(89, "3.4.9@Crediti tributari e per imposte differite attive@1@1@1@1@1@1@1@0@pv");

          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "89")
            {
              pianificazionenode = dd;
            }
          }
          break;
        case "137":
          VociBilancio.Add(90, "3.4.10@Crediti verso altri@1@1@1@1@1@1@1@0@pv");
          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "90")
            {
              pianificazionenode = dd;
            }
          }
          break;
        case "138":
          VociBilancio.Add(91, "3.4.11@Cassa e Banche@1@1@1@1@1@1@1@0@txt5c");

          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "91")
            {
              pianificazionenode = dd;
            }
          }
          break;
        case "139":
          VociBilancio.Add(92, "3.4.12@Ratei e risconti (attivi e passivi)@1@1@1@1@1@1@1@0@pv");

          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "92")
            {
              pianificazionenode = dd;
            }
          }
          break;
        case "140":
          VociBilancio.Add(93, "3.4.13@Patrimonio netto@1@1@1@1@1@1@1@0@pv");

          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "93")
            {
              pianificazionenode = dd;
            }
          }
          break;
        case "141":
          VociBilancio.Add(94, "3.4.14@Fondi per rischi ed oneri@1@1@1@1@1@1@1@0@pv");


          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "94")
            {
              pianificazionenode = dd;
            }
          }
          break;
        case "142":
          VociBilancio.Add(95, "3.4.15@Fondo TFR (Trattamento Fine Rapporto)@1@1@1@1@1@1@1@0@txt6c");

          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "95")
            {
              pianificazionenode = dd;
            }
          }
          break;
        case "178":
          VociBilancio.Add(96, "3.4.16@Mutui e finanziamenti non bancari@1@1@1@1@1@1@1@0@txt5c");

          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "96")
            {
              pianificazionenode = dd;
            }
          }

          break;
        case "179":
          VociBilancio.Add(97, "3.4.17@Debiti commerciali (Fornitori)@1@1@1@1@1@1@1@0@txt3c");

          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "97")
            {
              pianificazionenode = dd;
            }
          }
          break;
        case "180":
          VociBilancio.Add(98, "3.4.18@Debiti tributari e imposte differite passive@1@1@1@1@1@1@1@0@pv");

          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "98")
            {
              pianificazionenode = dd;
            }
          }
          break;
        case "181":
          VociBilancio.Add(99, "3.4.19@Debiti verso altri@1@1@1@1@1@1@1@0@pv");

          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "99")
            {
              pianificazionenode = dd;
            }
          }
          break;
        //case "144":
        //    VociBilancio.Add( 100, "3.4.20@Conti d'ordine@1@1@1@1@1@1@1@0@pv" );
        //    if (_x.Document.SelectSingleNode("/Dati//Dato[@ID='" + "274" + "']") != null)
        //    {
        //        pianificazionenode = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + "274" + "']").SelectSingleNode("Node[@ID='" + 100 + "']");
        //    }
        //    break;
        case "145":
          VociBilancio.Add(101, "3.4.21@Conto economico@1@1@1@1@1@1@1@0@pv");


          foreach (DataRow dd in datiP.Rows)
          {
            if (dd["ID"].ToString() == "101")
            {
              pianificazionenode = dd;
            }
          }
          break;
        //case "147":
        //    VociBilancio.Add( 102, "3.4.22@Bilancio Consolidato@0@1@1@1@1@1@0@0@pv" );
        //    if (_x.Document.SelectSingleNode("/Dati//Dato[@ID='" + "274" + "']") != null)
        //    {
        //        pianificazionenode = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + "274" + "']").SelectSingleNode("Node[@ID='" + 102 + "']");
        //    }
        //    break;
        default:
          return;
      }

      if (pianificazionenode != null && pianificazionenode["xaml"].ToString() != "")
      {
        /*
        XmlNodeList tmpnode2 = _x.Document.SelectNodes("//Node[@xaml]");

        foreach (XmlNode item in tmpnode2)
        {
            if (!(item.Attributes["xaml"].Value.Contains("\\XAML\\")))
            {
                DirectoryInfo di = new DirectoryInfo(App.AppDataDataFolder + "\\XAML");
                if (!di.Exists)
                {
                    di.Create();
                }

                string newXamlFile = "\\XAML\\" + Guid.NewGuid().ToString() + ".xaml";
                FileInfo fxaml = new FileInfo(App.AppDataDataFolder + newXamlFile);

                while (fxaml.Exists)
                {
                    newXamlFile = "\\XAML\\" + Guid.NewGuid().ToString() + ".xaml";
                    fxaml = new FileInfo(App.AppDataDataFolder + newXamlFile);
                }

                StreamWriter sw = fxaml.CreateText();
                sw.WriteLine(item.Attributes["xaml"].Value);
                sw.Flush();
                sw.Close();

                item.Attributes["xaml"].Value = newXamlFile;
            }
            else
            {
                break;
            }
        }
        */

        xnodehere["PianificazioneNewSingle"] = "Estesa";

        // _x.Save();

        visiblenode.Clear();
        visiblenodeID.Clear();

        string tipoBilancio = "";
        DataTable datibilanciotestata = cBusinessObjects.GetData(227, typeof(Excel_Bilancio_Testata));


        tipoBilancio = "";
        foreach (DataRow dt in datibilanciotestata.Rows)
        {
          tipoBilancio = dt["tipoBilancio"].ToString();
        }

        XmlDataProviderManager _y = null;

        switch (tipoBilancio)
        {
          case "2016":
            _y = new XmlDataProviderManager(App.AppLEAD2016, true);
            break;
          default:
            _y = new XmlDataProviderManager(App.AppLEAD, true);
            break;
        }
        foreach (KeyValuePair<int, string> vb in VociBilancio)
        {
          foreach (XmlNode item in _y.Document.SelectNodes("/LEADS/LEAD[@ID='" + vb.Value.Split('@')[0] + "']/RIGA"))
          {
            DataTable datiPian = cBusinessObjects.GetData(274, typeof(PianificazioneNewWD_Valore));

            foreach (DataRow nodehere in datiPian.Rows)
            {
              if (nodehere["ID"].ToString() == item.Attributes["ID"].Value)
              {
                if (nodehere["CONTROLLO"].ToString() != "" && nodehere["CONTROLLO"].ToString() == "True"
                  && nodehere["Titolo"].ToString() != "" && nodehere["Tipo"].ToString() != "")
                {
                  if (!visiblenode.Contains(nodehere["ID"].ToString()))
                  {
                    visiblenode.Add(nodehere["ID"].ToString(),
                      ((nodehere["Titolo"].ToString() == "Totale") ?
                        nodehere["Tipo"].ToString() : nodehere["Titolo"].ToString()));
                    visiblenodeID.Add(nodehere["ID"].ToString());
                  }
                }
              }
            }

          }
        }

        string xamlhere = "";

        if (pianificazionenode["xaml"].ToString().Contains("\\XAML\\"))
        {
          FileInfo fxaml = new FileInfo(App.AppDataDataFolder + pianificazionenode["xaml"].ToString());

          if (fxaml.Exists)
          {
            StreamReader sr = new StreamReader(fxaml.OpenRead());

            xamlhere = sr.ReadToEnd();
          }
        }
        else
        {
          xamlhere = pianificazionenode["xaml"].ToString();
        }

        if (xamlhere != "")
        {
          StringReader stringReader = new StringReader(xamlhere);
          XmlReader xmlReader = XmlReader.Create(stringReader);

          Grid wp = (Grid)System.Windows.Markup.XamlReader.Load(xmlReader);
          if (wp.Children[1].GetType().Name == "TextBox")
          {
            ((TextBox)(wp.Children[1])).HorizontalAlignment = HorizontalAlignment.Left;
            ((TextBox)(wp.Children[1])).Margin = new Thickness(0);
            //if (ID == "126")
            //{
            //    ((TextBox)(wp.Children[1])).Margin = new Thickness(-60, 0, 0, 0);
            //}
            //else
            //{
            //    ((TextBox)(wp.Children[1])).Margin = new Thickness(-75, 0, 0, 0);
            //}                    
          }

          if (wp.Children[3].GetType().Name == "TextBox")
          {
            ((TextBox)(wp.Children[3])).HorizontalAlignment = HorizontalAlignment.Left;
            ((TextBox)(wp.Children[3])).Margin = new Thickness(0);
          }

          wp.PreviewKeyDown += Void_PreviewKeyDown;
          wp.PreviewMouseDown += Void_PreviewMouseDown;
          //wp.IsEnabled = false;
          wp.Visibility = Visibility.Visible;

          brdDefinizione.Children.Add(wp);
        }
      }
      else
      {
        #region secondo blocco






        DataTable datiPsingle = cBusinessObjects.GetData(int.Parse(IDPianificazione), typeof(PianificazioneNew), cBusinessObjects.idcliente, int.Parse(idsessionerevisione), 1);
        DataTable statodatiPsingle = cBusinessObjects.GetData(int.Parse(IDPianificazione), typeof(StatoNodi), cBusinessObjects.idcliente, int.Parse(idsessionerevisione), 1);
        string statopSingle = "";
        foreach (DataRow dd in statodatiPsingle.Rows)
        {
          statopSingle = dd["Stato"].ToString();
        }

        Grid grd = new Grid();

        ColumnDefinition cd = new ColumnDefinition();
        cd.Width = new GridLength(200, GridUnitType.Pixel);
        cd.SharedSizeGroup = "ssg0";
        grd.ColumnDefinitions.Add(cd);

        cd = new ColumnDefinition();
        cd.Width = new GridLength(150, GridUnitType.Pixel);
        cd.SharedSizeGroup = "ssg1";
        grd.ColumnDefinitions.Add(cd);

        cd = new ColumnDefinition();
        cd.Width = new GridLength(100, GridUnitType.Pixel);
        grd.ColumnDefinitions.Add(cd);

        cd = new ColumnDefinition();
        cd.Width = new GridLength(30, GridUnitType.Pixel);
        grd.ColumnDefinitions.Add(cd);

        cd = new ColumnDefinition();
        cd.Width = new GridLength(30, GridUnitType.Pixel);
        grd.ColumnDefinitions.Add(cd);

        cd = new ColumnDefinition();
        cd.Width = new GridLength(30, GridUnitType.Pixel);
        grd.ColumnDefinitions.Add(cd);

        cd = new ColumnDefinition();
        cd.Width = new GridLength(30, GridUnitType.Pixel);
        grd.ColumnDefinitions.Add(cd);

        cd = new ColumnDefinition();
        cd.Width = new GridLength(30, GridUnitType.Pixel);
        grd.ColumnDefinitions.Add(cd);

        cd = new ColumnDefinition();
        cd.Width = new GridLength(30, GridUnitType.Pixel);
        grd.ColumnDefinitions.Add(cd);

        cd = new ColumnDefinition();
        cd.Width = new GridLength(30, GridUnitType.Pixel);
        grd.ColumnDefinitions.Add(cd);

        cd = new ColumnDefinition();
        cd.Width = new GridLength(0, GridUnitType.Pixel);
        grd.ColumnDefinitions.Add(cd);

        cd = new ColumnDefinition();
        cd.Width = new GridLength(1, GridUnitType.Star);
        cd.SharedSizeGroup = "ssg3";
        grd.ColumnDefinitions.Add(cd);

        cd = new ColumnDefinition();
        cd.Width = new GridLength(20, GridUnitType.Pixel);
        cd.SharedSizeGroup = "ssg4";
        grd.ColumnDefinitions.Add(cd);

        cd = new ColumnDefinition();
        cd.Width = new GridLength(50, GridUnitType.Pixel);
        cd.SharedSizeGroup = "ssg5";
        grd.ColumnDefinitions.Add(cd);

        RowDefinition rd = new RowDefinition();
        rd.Height = new GridLength(20, GridUnitType.Pixel);
        grd.RowDefinitions.Add(rd);

        rd = new RowDefinition();
        rd.Height = new GridLength(30, GridUnitType.Pixel);
        grd.RowDefinitions.Add(rd);

        txt = new TextBlock();
        txt.Text = "A";
        txt.ToolTip = "ESAME FISICO";
        txt.Margin = new Thickness(5, 0, 0, 0);
        grd.Children.Add(txt);
        Grid.SetRow(txt, 1);
        Grid.SetColumn(txt, 3);

        txt = new TextBlock();
        txt.Text = "B";
        txt.ToolTip = "CONFERMA";
        txt.Margin = new Thickness(5, 0, 0, 0);
        grd.Children.Add(txt);
        Grid.SetRow(txt, 1);
        Grid.SetColumn(txt, 4);

        txt = new TextBlock();
        txt.Text = "C";
        txt.ToolTip = "DOCUMENTAZIONE";
        txt.Margin = new Thickness(5, 0, 0, 0);
        grd.Children.Add(txt);
        Grid.SetRow(txt, 1);
        Grid.SetColumn(txt, 5);

        txt = new TextBlock();
        txt.Text = "D";
        txt.ToolTip = "PROCEDURE DI ANALISI COMPARATIVA";
        txt.Margin = new Thickness(5, 0, 0, 0);
        grd.Children.Add(txt);
        Grid.SetRow(txt, 1);
        Grid.SetColumn(txt, 6);

        txt = new TextBlock();
        txt.Text = "E";
        txt.ToolTip = "INDAGINE";
        txt.Margin = new Thickness(5, 0, 0, 0);
        grd.Children.Add(txt);
        Grid.SetRow(txt, 1);
        Grid.SetColumn(txt, 7);

        txt = new TextBlock();
        txt.Text = "F";
        txt.ToolTip = "RIPETIZIONE";
        txt.Margin = new Thickness(5, 0, 0, 0);
        grd.Children.Add(txt);
        Grid.SetRow(txt, 1);
        Grid.SetColumn(txt, 8);

        txt = new TextBlock();
        txt.Text = "G";
        txt.ToolTip = "OSSERVAZIONE DIRETTA";
        txt.Margin = new Thickness(5, 0, 0, 0);
        grd.Children.Add(txt);
        Grid.SetRow(txt, 1);
        Grid.SetColumn(txt, 9);

        grd.Margin = new Thickness(15, 0, 0, 0);

        brdDefinizione.Children.Add(grd);

        foreach (KeyValuePair<int, string> item in VociBilancio)
        {
          DataRow tmpnode = null;
          foreach (DataRow dd in datiPsingle.Rows)
          {
            if (dd["ID"].ToString() == item.Key.ToString())
              tmpnode = dd;
          }


          //if ( tmpnode != null )
          //{
          //    xnode.RemoveChild( tmpnode );
          //    tmpnode = null;
          //}

          if (tmpnode == null || statopSingle == "" || (statopSingle != "" && ((App.TipoTreeNodeStato)(Convert.ToInt32(statopSingle))) != App.TipoTreeNodeStato.Completato && ((App.TipoTreeNodeStato)(Convert.ToInt32(statopSingle))) != App.TipoTreeNodeStato.DaCompletare))
          {
            TextBlock txti = new TextBlock();
            txti.Text = "Pianificazione non eseguita.";
            txti.Margin = new Thickness(5, 5, 0, 0);
            brdDefinizione.Children.Clear();
            brdDefinizione.Children.Add(txti);
            return;
            //string xml = "<Node ID=\"" + item.Key.ToString() + "\" Voce=\"" + item.Value.ToString().Split( '@' )[0] + "\" Titolo=\"" + item.Value.ToString().Split( '@' )[1] + "\" EsameFisico=\"\" Ispezione=\"\" Indagine=\"\" Osservazione=\"\" Ricalcolo=\"\" Riesecuzione=\"\" Conferma=\"\" Comparazioni=\"\" Esecutore=\"\" Nota=\"\" />";
            //XmlDocument doctmp = new XmlDocument();
            //doctmp.LoadXml(xml);

            //XmlNode tmpNodeint = doctmp.SelectSingleNode("/Node");
            //XmlNode cliente = _x.Document.ImportNode(tmpNodeint, true);

            //xnode.AppendChild(cliente);

            //tmpnode = xnode.SelectSingleNode( "Node[@ID='" + item.Key.ToString() + "']" );
          }

          Border brd = new Border();
          brd.Padding = new Thickness(4.0, 0.0, 0.0, 0.0);
          brd.Margin = new Thickness(4.0, 0.0, 4.0, 0.0);

          grd = new Grid();

          cd = new ColumnDefinition();
          cd.Width = new GridLength(200, GridUnitType.Pixel);
          cd.SharedSizeGroup = "ssg0";
          grd.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(150, GridUnitType.Pixel);
          cd.SharedSizeGroup = "ssg1";
          grd.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(100, GridUnitType.Pixel);
          grd.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(30, GridUnitType.Pixel);
          grd.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(30, GridUnitType.Pixel);
          grd.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(30, GridUnitType.Pixel);
          grd.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(30, GridUnitType.Pixel);
          grd.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(30, GridUnitType.Pixel);
          grd.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(30, GridUnitType.Pixel);
          grd.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(30, GridUnitType.Pixel);
          grd.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(0, GridUnitType.Pixel);
          grd.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(1, GridUnitType.Star);
          cd.SharedSizeGroup = "ssg3";
          grd.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(20, GridUnitType.Pixel);
          cd.SharedSizeGroup = "ssg4";
          grd.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(50, GridUnitType.Pixel);
          cd.SharedSizeGroup = "ssg5";
          grd.ColumnDefinitions.Add(cd);

          rd = new RowDefinition();
          rd.Height = new GridLength(20, GridUnitType.Pixel);
          grd.RowDefinitions.Add(rd);

          rd = new RowDefinition();
          grd.RowDefinitions.Add(rd);

          txt = new TextBlock();
          txt.Text = "RISCHIO DI INDIVIDUAZIONE: ";
          grd.Children.Add(txt);
          Grid.SetRow(txt, 0);
          Grid.SetColumn(txt, 0);


          if (tmpnode != null)
          {
            xnodehere["Voce"] = ((tmpnode != null && tmpnode["Voce"].ToString() != "") ? tmpnode["Voce"].ToString() : "");
            xnodehere["Titolo"] = tmpnode["Titolo"].ToString();
            xnodehere["Esecutore"] = tmpnode["Esecutore"].ToString();
            xnodehere["Nota"] = tmpnode["Nota"].ToString();
          }


          string RIvalue = "Sconosciuto";


          if (tmpnode != null && tmpnode["cmbRI"].ToString() != null && tmpnode["cmbRI"].ToString() != "")
          {
            xnodehere["cmbRI"] = tmpnode["cmbRI"].ToString();
            switch (tmpnode["cmbRI"].ToString())
            {
              case "MA":
                RIvalue = "Molto Alto";
                break;
              case "A":
                RIvalue = "Alto";
                break;
              case "M":
                RIvalue = "Medio";
                break;
              case "B":
                RIvalue = "Basso";
                break;
              case "MB":
                RIvalue = "Molto Basso";
                break;
              case "PV":
                RIvalue = "Proced Validità";
                break;
              default:
              case "NA":
                RIvalue = "Non Applicabile";
                break;

            }
          }

          txt = new TextBlock();
          txt.Text = RIvalue.ToUpper();
          txt.FontWeight = FontWeights.Bold;
          grd.Children.Add(txt);
          Grid.SetRow(txt, 0);
          Grid.SetColumn(txt, 1);

          txt = new TextBlock();
          txt.Text = "|  EVIDENZE: ";
          grd.Children.Add(txt);
          Grid.SetRow(txt, 0);
          Grid.SetColumn(txt, 2);

          Image img = new Image();
          img.Name = "_" + item.Key.ToString() + "_EsameFisico";
          img.ToolTip = "ESAME FISICO";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;


          if (item.Value.ToString().Split('@')[2] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);




            xnodehere["EsameFisico"] = "X";
            if (tmpnode != null)
              tmpnode["EsameFisico"] = "X";
          }
          else
          {
            if (tmpnode != null && tmpnode["EsameFisico"].ToString() != "" && tmpnode["EsameFisico"].ToString() == "True")
            {
              xnodehere["EsameFisico"] = "True";

              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              xnodehere["EsameFisico"] = "False";

              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }

          this.RegisterName(img.Name, img);

          grd.Children.Add(img);
          Grid.SetRow(img, 0);
          Grid.SetColumn(img, 3);

          img = new Image();
          img.Name = "_" + item.Key.ToString() + "_Ispezione";
          img.ToolTip = "CONFERMA";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;



          if (item.Value.ToString().Split('@')[3] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);


            if (tmpnode != null)
              tmpnode["Ispezione"] = "X";

            xnodehere["Ispezione"] = "X";
          }
          else
          {
            if (tmpnode != null && tmpnode["Ispezione"].ToString() != "" && tmpnode["Ispezione"].ToString() == "True")
            {
              xnodehere["Ispezione"] = "True";

              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              xnodehere["Ispezione"] = "False";

              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }

          this.RegisterName(img.Name, img);

          grd.Children.Add(img);
          Grid.SetRow(img, 0);
          Grid.SetColumn(img, 4);

          img = new Image();
          img.Name = "_" + item.Key.ToString() + "_Indagine";
          img.ToolTip = "DOCUMENTAZIONE";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;



          if (item.Value.ToString().Split('@')[4] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);


            if (tmpnode != null)
              tmpnode["Indagine"] = "X";


            xnodehere["Indagine"] = "X";
          }
          else
          {
            if (tmpnode != null && tmpnode["Indagine"].ToString() != "" && tmpnode["Indagine"].ToString() == "True")
            {

              xnodehere["Indagine"] = "True";

              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              xnodehere["Indagine"] = "False";

              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }

          this.RegisterName(img.Name, img);

          grd.Children.Add(img);
          Grid.SetRow(img, 0);
          Grid.SetColumn(img, 5);

          img = new Image();
          img.Name = "_" + item.Key.ToString() + "_Osservazione";
          img.ToolTip = "PROCEDURE DI ANALISI COMPARATIVA";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;



          if (item.Value.ToString().Split('@')[5] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);


            if (tmpnode != null)
              tmpnode["Osservazione"] = "X";


            xnodehere["Osservazione"] = "X";
          }
          else
          {
            if (tmpnode != null && tmpnode["Osservazione"].ToString() != "" && tmpnode["Osservazione"].ToString() == "True")
            {
              xnodehere["Osservazione"] = "True";

              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              xnodehere["Osservazione"] = "False";

              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }

          this.RegisterName(img.Name, img);

          grd.Children.Add(img);
          Grid.SetRow(img, 0);
          Grid.SetColumn(img, 6);

          img = new Image();
          img.Name = "_" + item.Key.ToString() + "_Ricalcolo";
          img.ToolTip = "INDAGINE";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;


          if (item.Value.ToString().Split('@')[6] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);

            if (tmpnode != null)
              tmpnode["Ricalcolo"] = "X";


            xnodehere["Ricalcolo"] = "X";
          }
          else
          {
            if (tmpnode != null && tmpnode["Ricalcolo"].ToString() != "" && tmpnode["Ricalcolo"].ToString() == "True")
            {
              xnodehere["Ricalcolo"] = "True";

              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              xnodehere["Ricalcolo"] = "False";

              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }
          this.RegisterName(img.Name, img);

          grd.Children.Add(img);
          Grid.SetRow(img, 0);
          Grid.SetColumn(img, 7);

          img = new Image();
          img.Name = "_" + item.Key.ToString() + "_Riesecuzione";
          img.ToolTip = "RIPETIZIONE";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;



          if (item.Value.ToString().Split('@')[7] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);


            if (tmpnode != null)
              tmpnode["Riesecuzione"] = "X";


            xnodehere["Riesecuzione"] = "X";
          }
          else
          {
            if (tmpnode != null && tmpnode["Riesecuzione"].ToString() != "" && tmpnode["Riesecuzione"].ToString() == "True")
            {
              xnodehere["Riesecuzione"] = "True";

              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              xnodehere["Riesecuzione"] = "False";

              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }

          this.RegisterName(img.Name, img);

          grd.Children.Add(img);
          Grid.SetRow(img, 0);
          Grid.SetColumn(img, 8);

          img = new Image();
          img.Name = "_" + item.Key.ToString() + "_Conferma";
          img.ToolTip = "OSSERVAZIONE DIRETTA";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;


          if (item.Value.ToString().Split('@')[8] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);

            if (tmpnode != null)
              tmpnode["Conferma"] = "X";


            xnodehere["Conferma"] = "X";
          }
          else
          {
            if (tmpnode != null && tmpnode["Conferma"].ToString() != "" && tmpnode["Conferma"].ToString() == "True")
            {
              xnodehere["Conferma"] = "True";

              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              xnodehere["Conferma"] = "False";

              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }

          this.RegisterName(img.Name, img);

          grd.Children.Add(img);
          Grid.SetRow(img, 0);
          Grid.SetColumn(img, 9);

          brd.Child = grd;

          brdDefinizione.Children.Add(brd);

          StackPanel stk = new StackPanel();
          stk.Margin = new Thickness(0, 5, 0, 0);
          stk.Orientation = Orientation.Horizontal;
          stk.HorizontalAlignment = HorizontalAlignment.Center;

          TextBlock txtblk = new TextBlock();
          txtblk.Text = "Legenda: VEDI SUGGERIMENTI";
          txtblk.FontWeight = FontWeights.Bold;
          txtblk.Margin = new Thickness(10, 0, 0, 10);
          txtblk.HorizontalAlignment = HorizontalAlignment.Center;
          stk.Children.Add(txtblk);

          //TextBlock txtblk = new TextBlock();
          //txtblk.Text = "Legenda Evidenze: A";
          //txtblk.FontWeight = FontWeights.Bold;
          //txtblk.Margin = new Thickness(10, 0, 0, 10);
          //stk.Children.Add(txtblk);

          //txtblk = new TextBlock();
          //txtblk.Text = "= Esame Fisico";
          //txtblk.Margin = new Thickness(5, 0, 0, 10);
          //stk.Children.Add(txtblk);

          //txtblk = new TextBlock();
          //txtblk.Text = "B";
          //txtblk.FontWeight = FontWeights.Bold;
          //txtblk.Margin = new Thickness(10, 0, 0, 10);
          //stk.Children.Add(txtblk);

          //txtblk = new TextBlock();
          //txtblk.Text = "= Conferma";
          //txtblk.Margin = new Thickness(5, 0, 0, 10);
          //stk.Children.Add(txtblk);

          //txtblk = new TextBlock();
          //txtblk.Text = "C";
          //txtblk.FontWeight = FontWeights.Bold;
          //txtblk.Margin = new Thickness(10, 0, 0, 10);
          //stk.Children.Add(txtblk);

          //txtblk = new TextBlock();
          //txtblk.Text = "= Documentazione";
          //txtblk.Margin = new Thickness(5, 0, 0, 10);
          //stk.Children.Add(txtblk);

          //txtblk = new TextBlock();
          //txtblk.Text = "D";
          //txtblk.FontWeight = FontWeights.Bold;
          //txtblk.Margin = new Thickness(10, 0, 0, 10);
          //stk.Children.Add(txtblk);

          //txtblk = new TextBlock();
          //txtblk.Text = "= Procedura di analisi comparativa";
          //txtblk.Margin = new Thickness(5, 0, 0, 10);
          //stk.Children.Add(txtblk);

          //txtblk = new TextBlock();
          //txtblk.Text = "E";
          //txtblk.FontWeight = FontWeights.Bold;
          //txtblk.Margin = new Thickness(10, 0, 0, 10);
          //stk.Children.Add(txtblk);

          //txtblk = new TextBlock();
          //txtblk.Text = "= Indagine";
          //txtblk.Margin = new Thickness(5, 0, 0, 10);
          //stk.Children.Add(txtblk);

          //txtblk = new TextBlock();
          //txtblk.Text = "F";
          //txtblk.FontWeight = FontWeights.Bold;
          //txtblk.Margin = new Thickness(10, 0, 0, 10);
          //stk.Children.Add(txtblk);

          //txtblk = new TextBlock();
          //txtblk.Text = "= Ripetizione";
          //txtblk.Margin = new Thickness(5, 0, 0, 10);
          //stk.Children.Add(txtblk);

          //txtblk = new TextBlock();
          //txtblk.Text = "G";
          //txtblk.FontWeight = FontWeights.Bold;
          //txtblk.Margin = new Thickness(10, 0, 0, 10);
          //stk.Children.Add(txtblk);

          //txtblk = new TextBlock();
          //txtblk.Text = "= Osservazione diretta";
          //txtblk.Margin = new Thickness(5, 0, 0, 10);
          //stk.Children.Add(txtblk);

          brdDefinizione.Children.Add(stk);
        }
        #endregion
      }
    }

    private void Void_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      e.Handled = true;
      return;
    }

    private void Void_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      e.Handled = true;
      return;
    }

    private void RischioIntrinseco_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount == 2)
      {
        txt_MouseDownCicli(sender, e, "2.8.7");
      }
    }

    private void CicloVendite_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount == 2)
      {
        txt_MouseDownCicli(sender, e, "2.9.1");
      }
    }

    private void CicloAcquisti_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount == 2)
      {
        txt_MouseDownCicli(sender, e, "2.9.2");
      }
    }

    private void CicloMagazzino_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount == 2)
      {
        txt_MouseDownCicli(sender, e, "2.9.3");
      }
    }

    private void CicloTesoreria_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount == 2)
      {
        txt_MouseDownCicli(sender, e, "2.9.4");
      }
    }

    private void CicloPersonale_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount == 2)
      {
        txt_MouseDownCicli(sender, e, "2.9.5");
      }
    }

    void tbEsecutore_LostFocus(object sender, RoutedEventArgs e)
    {
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }

      string name = ((TextBox)sender).Name.Split('_')[1];
      foreach (DataRow dd in dati.Rows)
      {
        if (dd["ID"].ToString() == name)
        {
          dd["Esecutore"] = ((TextBox)sender).Text;
        }
      }

    }

    void tbNota_LostFocus(object sender, RoutedEventArgs e)
    {
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }

      string name = ((TextBox)sender).Name.Split('_')[1];
      foreach (DataRow dd in dati.Rows)
      {
        if (dd["ID"].ToString() == name)
        {
          dd["Nota"] = ((TextBox)sender).Text;
        }
      }

    }

    public int Save()
    {
      cBusinessObjects.SaveData(id, dati, typeof(PianificazioneNewSingolo));
      return 0;
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {

      foreach (DataRow dd in dati.Rows)
      {
        dd["Testo"] = ((TextBox)sender).Text;

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

    private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      Image i = ((Image)sender);

      TextBlock t = ((TextBlock)(((Grid)(i.Parent)).Children[1]));

      UIElement u = ((Grid)(i.Parent)).Children[2];

      if (u.Visibility == System.Windows.Visibility.Collapsed)
      {
        u.Visibility = System.Windows.Visibility.Visible;
        t.TextAlignment = TextAlignment.Center;
        var uriSource = new Uri(down, UriKind.Relative);
        i.Source = new BitmapImage(uriSource);
      }
      else
      {
        t.TextAlignment = TextAlignment.Left;
        u.Visibility = System.Windows.Visibility.Collapsed;
        var uriSource = new Uri(left, UriKind.Relative);
        i.Source = new BitmapImage(uriSource);
      }
    }

    private void ImageNota_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      string name = ((Image)sender).Name.Replace("_NotaImg", "");

      TextBox txtNota = (TextBox)this.FindName(name + "_Nota");

      if (txtNota.Visibility == System.Windows.Visibility.Collapsed)
      {
        txtNota.Visibility = System.Windows.Visibility.Visible;
        var uriSource = new Uri(up, UriKind.Relative);
        ((Image)sender).Source = new BitmapImage(uriSource);
      }
      else
      {
        txtNota.Visibility = System.Windows.Visibility.Collapsed;
        var uriSource = new Uri(down, UriKind.Relative);
        ((Image)sender).Source = new BitmapImage(uriSource);
      }
    }

    private void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      return;

#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
      string tipo = ((Image)sender).Name.Split('_').Last();
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
      string name = ((Image)sender).Name.Split('_')[1];

      var uriSource = new Uri(uncheck, UriKind.Relative);

      XmlNode tmpnode = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + _ID + "']/Node[@ID='" + name + "']");
      if (tmpnode != null && tmpnode.Attributes[tipo] != null)
      {
        if (tmpnode.Attributes[tipo].Value == "True")
        {
          tmpnode.Attributes[tipo].Value = "";

          uriSource = new Uri(uncheck, UriKind.Relative);
          ((Image)sender).Source = new BitmapImage(uriSource);
        }
        else
        {
          tmpnode.Attributes[tipo].Value = "True";

          uriSource = new Uri(check, UriKind.Relative);
          ((Image)sender).Source = new BitmapImage(uriSource);
        }
      }
      else
      {
        uriSource = new Uri(uncheck, UriKind.Relative);
        ((Image)sender).Source = new BitmapImage(uriSource);
      }
    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      Resizer(Convert.ToInt32(e.NewSize.Width));
    }

    public void Resizer(int newsize)
    {
      //double actualwidth = ((Grid)(txtDescrizioneIntensita.Parent)).ActualWidth;

      //for ( int i = 2; i < brdDefinizione.Children.Count - 1; i++ )
      //{
      //    Grid grid = ((Grid)(((Border)(brdDefinizione.Children[i])).Child));
      //    ((TextBox)(grid.Children[11])).Width = (actualwidth - 710 > 100)? actualwidth - 710: 100;
      //    ((TextBox)(grid.Children[14])).Width = (actualwidth - 270 > 200)? actualwidth - 270: 200;
      //}

      //txtConsiderazioni.Width = actualwidth - 100;

    }

    void txt_MouseDownCicli(object sender, MouseButtonEventArgs e, string Codice)
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
          if (item.Attributes["Codice"].Value == Codice)
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

      App.MessaggioSolaScrittura = "Carta in sola lettura, premere tasto ESCI";
      App.MessaggioSolaScritturaStato = "Carta in sola lettura, premere tasto ESCI";

      wa.ShowDialog();

      App.MessaggioSolaScrittura = "Occorre selezionare Sblocca Stato per modificare il contenuto.";
      App.MessaggioSolaScritturaStato = "Sessione in sola lettura, impossibile modificare lo stato.";
    }

    void txt_MouseDownCicli(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount == 2)
      {
        MasterFile mf = MasterFile.Create();

        Hashtable revisioneNow = mf.GetRevisione(SessioniID[SessioneNow].ToString());
        string revisioneAssociata = App.AppDataDataFolder + "\\" + revisioneNow["FileData"].ToString();
        string revisioneTreeAssociata = App.AppDataDataFolder + "\\" + revisioneNow["File"].ToString();
        string revisioneIDAssociata = SessioniID[SessioneNow].ToString();

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
        wa.SessioniTitoli.Add(0, SessioniTitoli[SessioneNow].ToString());

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

        App.MessaggioSolaScrittura = "Carta in sola lettura, premere tasto ESCI";
        App.MessaggioSolaScritturaStato = "Carta in sola lettura, premere tasto ESCI";

        wa.ShowDialog();

        App.MessaggioSolaScrittura = "Occorre selezionare Sblocca Stato per modificare il contenuto.";
        App.MessaggioSolaScritturaStato = "Sessione in sola lettura, impossibile modificare lo stato.";
      }
    }

    void cmbRI_Changed(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems[0].ToString().Contains('*') == true)
      {

        DataTable datiR = cBusinessObjects.GetData(int.Parse(IDRischioGlobale), typeof(RischioGlobale), cBusinessObjects.idcliente, int.Parse(idsessionerevisione), 1);
        DataRow node = null;
        foreach (DataRow dd in datiR.Rows)
        {
          node = dd;
        }

        if (((ComboBox)sender).Name.Split('_')[3] == "pv")
        {
          ((ComboBox)sender).SelectedItem = ((ComboBoxItem)((ComboBox)sender).Items[5]);
          ((ComboBox)sender).Text = ((ComboBoxItem)((ComboBox)sender).Items[5]).Content.ToString();
        }
        else
        {
          int selecteditem = 0;
          if (node != null)
          {
            switch (node[((ComboBox)sender).Name.Split('_')[3]].ToString())
            {
              case "Molto Alto":
                selecteditem = 0;
                break;
              case "Alto":
                selecteditem = 1;
                break;
              case "Medio":
                selecteditem = 2;
                break;
              case "Basso":
                selecteditem = 3;
                break;
              case "Molto Basso":
                selecteditem = 4;
                break;
              default:
                selecteditem = 6;
                break;
            }
          }


            ((ComboBox)sender).SelectedItem = ((ComboBoxItem)((ComboBox)sender).Items[selecteditem]);
          ((ComboBox)sender).Text = ((ComboBoxItem)((ComboBox)sender).Items[selecteditem]).Content.ToString();
        }
      }
      else
      {
        string name = ((ComboBox)sender).Name.Split('_')[1];
        DataRow tmpnode = null;
        foreach (DataRow dd in dati.Rows)
        {
          if (dd["ID"].ToString() == name)
          {
            tmpnode = dd;
          }
        }
        if (tmpnode != null)
        {
          string resultvalue = "";

          switch (((ComboBox)sender).SelectedIndex)
          {
            case 0:
              resultvalue = "MA";
              break;
            case 1:
              resultvalue = "A";
              break;
            case 2:
              resultvalue = "M";
              break;
            case 3:
              resultvalue = "B";
              break;
            case 4:
              resultvalue = "MB";
              break;
            case 5:
              resultvalue = "PV";
              break;
            case 6:
              resultvalue = "NA";
              break;
            default:
              resultvalue = "";
              break;
          }

          tmpnode["cmbRI"] = resultvalue;
        }
      }
    }

    void txt_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount == 2)
      {
        MasterFile mf = MasterFile.Create();
        string bilancioAssociato = mf.GetBilancioAssociatoFromRevisioneFile(Sessioni[SessioneNow].ToString());
        string bilancioTreeAssociato = mf.GetBilancioTreeAssociatoFromRevisioneFile(Sessioni[SessioneNow].ToString());
        string bilancioIDAssociato = mf.GetBilancioIDAssociatoFromRevisioneFile(Sessioni[SessioneNow].ToString());

        if (bilancioAssociato == "")
        {
          MessageBox.Show("Per accedere alla carta occorre aver creato il bilanco.", "Attenzione");
          e.Handled = true;
          return;
        }

        XmlDataProviderManager _xNew = new XmlDataProviderManager(bilancioAssociato);

        WindowWorkArea wa = new WindowWorkArea(ref _xNew);

        //Nodi
        wa.NodeHome = 0;

        RevisoftApplication.XmlManager xt = new XmlManager();
        xt.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
        XmlDataProvider TreeXmlProvider = new XmlDataProvider();
        TreeXmlProvider.Document = xt.LoadEncodedFile(bilancioTreeAssociato);

        if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
        {
          foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
          {
            if (item.Attributes["Codice"].Value == ((TextBlock)(sender)).Text)
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
        wa.Sessioni.Add(0, bilancioAssociato);

        wa.SessioniTitoli.Clear();
        wa.SessioniTitoli.Add(0, SessioniTitoli[SessioneNow].ToString());

        wa.SessioniID.Clear();
        wa.SessioniID.Add(0, bilancioIDAssociato);

        wa.SessioneHome = 0;
        wa.SessioneNow = 0;

        //Variabili
        wa.ReadOnly = true;
        wa.ReadOnlyOLD = true;
        wa.ApertoInSolaLettura = true;

        //passaggio dati
        wa.IDTree = "4";
        wa.IDSessione = bilancioIDAssociato;
        wa.IDCliente = IDCliente;

        wa.Stato = App.TipoTreeNodeStato.Sconosciuto;
        wa.OldStatoNodo = wa.Stato;

        //apertura
        wa.Load();

        App.MessaggioSolaScrittura = "Carta in sola lettura, premere tasto ESCI";
        App.MessaggioSolaScritturaStato = "Carta in sola lettura, premere tasto ESCI";

        wa.ShowDialog();

        App.MessaggioSolaScrittura = "Occorre selezionare Sblocca Stato per modificare il contenuto.";
        App.MessaggioSolaScritturaStato = "Sessione in sola lettura, impossibile modificare lo stato.";
      }
    }

  }
}
