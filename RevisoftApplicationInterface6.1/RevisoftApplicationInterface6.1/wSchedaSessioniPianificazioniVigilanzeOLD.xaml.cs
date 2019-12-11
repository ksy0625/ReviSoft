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

namespace RevisoftApplication
{
  public partial class wSchedaSessioniPianificazioniVigilanzeOLD : Window
  {

    public Hashtable htSessioni = new Hashtable();
    public string DataInizio = "";
    public string DataFine = "";
    public string lastData = "";
    public string lastKey = "";
    public XmlDataProviderManager _x;
    public string Cliente = "";
    public string IDCliente = "";
    public string nota = "";
    public string IDTree = "";
    public string IDSessione = "";

    public App.TipoTreeNodeStato OldStatoNodo;
    public App.TipoTreeNodeStato Stato = App.TipoTreeNodeStato.Sconosciuto;

    public bool ReadOnly = true;

    Brush ButtonStatoSelectedColor = new SolidColorBrush(Color.FromArgb(255, 247, 168, 39));
    Color ButtonToolBarSelectedColor = Color.FromArgb(126, 130, 189, 228);
    Color ButtonToolBarPulseColor = Color.FromArgb(126, 82, 101, 115);

    public wSchedaSessioniPianificazioniVigilanzeOLD()
    {
      InitializeComponent();
      lab1.Foreground = App._arrBrushes[0];
    }

    public void ConfiguraMaschera()
    {
      labelTitolo.Text = "Sessioni nel periodo " + DataInizio + " - " + DataFine;

      UpdateGridCombo();

      switch (Stato)
      {
        case App.TipoTreeNodeStato.DaCompletare:
          btn_Stato_DaCompletare.Background = ButtonStatoSelectedColor;
          btn_Stato_Completato.Background = btn_NodoHelp.Background;
          btn_Stato_SbloccaNodo.Background = btn_NodoHelp.Background;
          ReadOnly = true;
          break;
        case App.TipoTreeNodeStato.Completato:
          btn_Stato_Completato.Background = ButtonStatoSelectedColor;
          btn_Stato_DaCompletare.Background = btn_NodoHelp.Background;
          btn_Stato_SbloccaNodo.Background = btn_NodoHelp.Background;
          ReadOnly = true;
          break;
        default:
          ReadOnly = false;
          break;
      }

    }

    public void generateTree()
    {
      _x.Save();

      RevisoftApplication.XmlManager x = new XmlManager();
      x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
      MasterFile mf = MasterFile.Create();

      string SelectedTreeSource = App.AppTemplateTreeVigilanza;

      XmlDataProviderManager _xTXP = new XmlDataProviderManager(SelectedTreeSource);

      try
      {
        foreach (XmlNode item in _xTXP.Document.SelectNodes("/Tree//Node[@ID][@Codice][@Titolo]"))
        {
          string isTitolo = "";

          if (item.ChildNodes.Count > 0)
          {
            isTitolo = "Father=\"1\"";
          }

          if (_x.Document.SelectSingleNode("//Dato[@ID=\"100003\"]/Valore[@ID=\"" + item.Attributes["ID"].Value + "\"]") == null)
          {
            string xml = "<Valore ID=\"" + item.Attributes["ID"].Value + "\" " + isTitolo + " Codice=\"" + item.Attributes["Codice"].Value.Replace("&", "&amp;").Replace("\"", "'") + "\" Titolo=\"" + item.Attributes["Titolo"].Value.Replace("&", "&amp;").Replace("\"", "'") + "\" Checked=\"False\" />";

            XmlDocument doctmp = new XmlDocument();
            doctmp.LoadXml(xml);

            XmlNode tmpNode = doctmp.SelectSingleNode("/Valore");
            XmlNode cliente = _x.Document.ImportNode(tmpNode, true);

            _x.Document.SelectSingleNode("//Dato[@ID=\"100003\"]").AppendChild(cliente);
          }
          else
          {
            //già creato l'albero, non viene aggiornato
            return;

            //XmlNode itemhere = _x.Document.SelectSingleNode( "//Dato[@ID=\"100003\"]" ).SelectSingleNode( "//Valore[@ID=\"" + item.Attributes["ID"].Value + "\"]" );
            //itemhere.Attributes["Codice"].Value = item.Attributes["Codice"].Value;
            //itemhere.Attributes["Titolo"].Value = item.Attributes["Titolo"].Value;
          }
        }

        //foreach ( XmlNode item in _x.Document.SelectSingleNode( "//Dato[@ID=\"100003\"]" ).SelectNodes("//Valore"))
        //{
        //    foreach ( DictionaryEntry itemS in htSessioni )
        //    {
        //        if ( item.SelectSingleNode( "//Valore[@ID=\"" + item.Attributes["ID"].Value + "\"]/Pianificazione[@Data=\"" + itemS.Value.ToString() + "\"]" ) == null )
        //        {
        //            string xml = "<Pianificazione ID=\"" + itemS.Key.ToString() + "\" Data=\"" + itemS.Value.ToString() + "\" />";

        //            XmlDocument doctmp = new XmlDocument();
        //            doctmp.LoadXml( xml );

        //            XmlNode tmpNode = doctmp.SelectSingleNode( "/Pianificazione" );
        //            XmlNode cliente = item.OwnerDocument.ImportNode( tmpNode, true );

        //            item.AppendChild( cliente );
        //        }

        //        if ( item.SelectSingleNode( "//Valore[@ID=\"" + item.Attributes["ID"].Value + "\"]/Pianificazione[@Data=\"" + itemS.Value.ToString() + "\"]" ).Attributes["OK"] == null )
        //        {
        //            XmlAttribute attr = item.OwnerDocument.CreateAttribute( "OK" );
        //            item.SelectSingleNode( "//Valore[@ID=\"" + item.Attributes["ID"].Value + "\"]/Pianificazione[@Data=\"" + itemS.Value.ToString() + "\"]" ).Attributes.Append( attr );
        //        }

        //        if ( item.SelectSingleNode( "//Valore[@ID=\"" + item.Attributes["ID"].Value + "\"]/Pianificazione[@Data=\"" + itemS.Value.ToString() + "\"]" ).Attributes["Checked"] == null )
        //        {
        //            XmlAttribute attr = item.OwnerDocument.CreateAttribute( "Checked" );
        //            item.SelectSingleNode( "//Valore[@ID=\"" + item.Attributes["ID"].Value + "\"]/Pianificazione[@Data=\"" + itemS.Value.ToString() + "\"]" ).Attributes.Append( attr );
        //        }
        //    }

        //    //foreach ( XmlNode itemOK in item.SelectNodes( "//Pianificazione" ) )
        //    //{
        //    //    if ( itemOK.Attributes["OK"] == null )
        //    //    {
        //    //        itemOK.ParentNode.RemoveChild( itemOK );
        //    //    }
        //    //}
        //}
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaSessioniPianificazioniVigilanzeOLD.generateTree exception");
        string log = ex.Message;
      }

      _x.Save();
    }

    private void UpdateGridCombo()
    {
      _x.Save();

      DynamicGrid.Children.Clear();
      DynamicGrid.ColumnDefinitions.Clear();
      DynamicGrid.RowDefinitions.Clear();

      if (htSessioni.Count > 0)
      {
        ColumnDefinition gridCol1 = new ColumnDefinition();
        ColumnDefinition gridCol2 = new ColumnDefinition();
        ColumnDefinition gridCol3 = new ColumnDefinition();

        DynamicGrid.ColumnDefinitions.Add(gridCol1);
        DynamicGrid.ColumnDefinitions.Add(gridCol2);
        DynamicGrid.ColumnDefinitions.Add(gridCol3);

        for (int i = 0; i < htSessioni.Count; i++)
        {
          RowDefinition gridRow1 = new RowDefinition();
          gridRow1.Height = new GridLength(30); //andrea da 20 a 30
          DynamicGrid.RowDefinitions.Add(gridRow1);
        }

        int indexrow = 0;

        foreach (DictionaryEntry item in htSessioni)
        {
          TextBlock txtb = new TextBlock();
          txtb.Text = "Sessione n. " + item.Key.ToString();
          txtb.Width = 130;
          txtb.Height = 24;
          txtb.Margin = new Thickness(0, 0, 0, 0);
          Grid.SetRow(txtb, indexrow);
          Grid.SetColumn(txtb, 0);
          DynamicGrid.Children.Add(txtb);


          //TextBox txt = new TextBox(); andrea
          DatePicker txt = new DatePicker();
          txt.Name = "txt_" + item.Key.ToString();
          txt.Text = item.Value.ToString();
          txt.Width = 130;
          txt.Height = 24;
          txt.Margin = new Thickness(10, 0, 0, 0);
          txt.LostFocus += txt_LostFocus;
          Grid.SetRow(txt, indexrow);
          Grid.SetColumn(txt, 1);
          DynamicGrid.Children.Add(txt);

          Button btn = new Button();
          btn.Name = "btnDelete_" + item.Key.ToString();
          btn.Margin = new Thickness(10, 0, 0, 0);
          btn.Height = 24;
          btn.Content = " Cancella ";
          btn.Click += btnCancella_Click;
          Grid.SetRow(btn, indexrow);
          Grid.SetColumn(btn, 2);
          DynamicGrid.Children.Add(btn);

          indexrow++;
        }

        buttonComando.Visibility = System.Windows.Visibility.Visible;
      }
      else
      {
        ColumnDefinition gridCol1 = new ColumnDefinition();
        DynamicGrid.ColumnDefinitions.Add(gridCol1);

        RowDefinition gridRow1 = new RowDefinition();
        gridRow1.Height = new GridLength(30);
        DynamicGrid.RowDefinitions.Add(gridRow1);

        TextBlock txtb = new TextBlock();
        txtb.Text = "Nessuna sessione attualmente presente";
        Grid.SetRow(txtb, 0);
        Grid.SetColumn(txtb, 0);
        DynamicGrid.Children.Add(txtb);

        buttonComando.Visibility = System.Windows.Visibility.Collapsed;
      }
    }

    void btnCancella_Click(object sender, RoutedEventArgs e)
    {
      if (ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }

      string IDhere = ((Button)(sender)).Name.Split('_').Last();
      foreach (XmlNode tbd in _x.Document.SelectNodes("//Pianificazione[@ID=\"" + IDhere + "\"]"))
      {
        tbd.ParentNode.RemoveChild(tbd);
      }

      lastKey = "";
      lastData = "";
      htSessioni.Clear();

      foreach (XmlNode itemV in _x.Document.SelectNodes("//Dato[@ID=\"100003\"]/Valore"))
      {
        int index = 1;
        foreach (XmlNode item in itemV.SelectNodes("//Dato[@ID=\"100003\"]/Valore[@ID=\"" + itemV.Attributes["ID"].Value + "\"]/Pianificazione"))
        {
          item.Attributes["ID"].Value = index.ToString();
          lastKey = item.Attributes["ID"].Value;
          lastData = item.Attributes["Data"].Value;

          if (!htSessioni.Contains(lastKey))
          {
            htSessioni.Add(lastKey, lastData);
          }
          index++;
        };
      }

      UpdateGridCombo();
    }

    void txt_LostFocus(object sender, RoutedEventArgs e)
    {
      if (ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }

      string IDhere = ((DatePicker)(sender)).Name.Split('_').Last();
      string newvalue = ((DatePicker)(sender)).Text;

      if (htSessioni[IDhere].ToString() == newvalue)
      {
        return;
      }

      DateTime dt = new DateTime();

      try
      {
        dt = Convert.ToDateTime(newvalue);
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaSessioniPianificazioniVigilanzeOLD.txt_LostFocus exception");
        string log = ex.Message;
        MessageBox.Show("Attenzione data inserita non valida");
        UpdateGridCombo();
        return;
      }

      if (htSessioni.ContainsValue(newvalue))
      {
        MessageBox.Show("Attenzione data già presente");
        UpdateGridCombo();
        return;
      }

      if (dt.CompareTo(Convert.ToDateTime(DataInizio)) < 0)
      {
        MessageBox.Show("Attenzione: data antecedente all'inizio del periodo.");
        UpdateGridCombo();
        return;
      }

      string previous = (Convert.ToInt32(IDhere) - 1).ToString();
      if (htSessioni.ContainsKey(previous))
      {
        if (dt.CompareTo(Convert.ToDateTime(htSessioni[previous])) < 0)
        {
          MessageBox.Show("Attenzione le date devono essere sequenziali. Si consiglia di inserire subito la data quando si crea la sessione.");
          UpdateGridCombo();
          return;
        }
      }

      if (dt.CompareTo(Convert.ToDateTime(DataFine)) > 0)
      {
        MessageBox.Show("Attenzione: data successiva alla fine del periodo.");
        UpdateGridCombo();
        return;
      }

      string next = (Convert.ToInt32(IDhere) + 1).ToString();
      if (htSessioni.ContainsKey(next))
      {
        if (dt.CompareTo(Convert.ToDateTime(htSessioni[next])) > 0)
        {
          MessageBox.Show("Attenzione le date devono essere sequenziali.");
          UpdateGridCombo();
          return;
        }
      }

      foreach (XmlNode tbd in _x.Document.SelectNodes("//Pianificazione[@ID=\"" + IDhere + "\"]"))
      {
        tbd.Attributes["Data"].Value = dt.ToShortDateString();
      }

      htSessioni[IDhere] = newvalue;

      foreach (DictionaryEntry item in htSessioni)
      {
        lastData = item.Value.ToString();
        lastKey = item.Key.ToString();
      }

      UpdateGridCombo();
      return;
    }

    private void buttonAdd_Click(object sender, RoutedEventArgs e)
    {
      if (ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }

      try
      {
        string lastdatehere = "";

        if (lastData == "")
        {
          lastdatehere = DataInizio;
        }
        else
        {
          DateTime dt = Convert.ToDateTime(lastData);
          dt = dt.AddDays(1);

          if (dt.CompareTo(Convert.ToDateTime(DataFine)) > 0)
          {
            MessageBox.Show("Attenzione data finale già raggiunta");
            return;
          }

          lastdatehere = dt.ToShortDateString();
        }

        string lastKeyhere = "";

        if (lastKey == "")
        {
          lastKeyhere = "1";
        }
        else
        {
          lastKeyhere = (Convert.ToInt32(lastKey) + 1).ToString();
        }

        foreach (XmlNode itemV in _x.Document.SelectNodes("//Dato[@ID=\"100003\"]/Valore"))
        {
          string xml = "<Pianificazione ID=\"" + lastKeyhere + "\" Data=\"" + lastdatehere + "\" Checked=\"False\" />";

          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);

          XmlNode tmpNode = doctmp.SelectSingleNode("/Pianificazione");
          XmlNode cliente = itemV.OwnerDocument.ImportNode(tmpNode, true);

          itemV.AppendChild(cliente);

          if (!htSessioni.Contains(lastKeyhere))
          {
            lastData = lastdatehere;
            lastKey = lastKeyhere;
            htSessioni.Add(lastKey, lastData);
          }
        }

        UpdateGridCombo();

      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaSessioniPianificazioniVigilanzeOLD.buttonAdd_Click exception");
        string log = ex.Message;
        return;
      }
    }

    private void buttonApri_Click(object sender, RoutedEventArgs e)
    {
      wWorkAreaTree_PianificazioniVigilanze PVList = new wWorkAreaTree_PianificazioniVigilanze();
      PVList.IDP = "100003";
      PVList.Owner = this;
      PVList.TipoAttivita = App.TipoAttivita.Vigilanza;
      PVList.DataInizio = DataInizio;
      PVList.DataFine = DataFine;
      PVList.IDCliente = IDCliente;
      PVList.Cliente = Cliente;
      PVList._x = _x;

      PVList.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

      PVList.ReadOnly = ReadOnly;

      PVList.LoadTreeSource();
      PVList.ShowDialog();
    }

    private void btn_SOSPESI_Click(object sender, RoutedEventArgs e)
    {
      XmlNode node = _x.Document.SelectSingleNode("//Dato[@ID=\"100003\"]");

      Sospesi o = new Sospesi();
      o.Owner = this;

      if (System.Windows.SystemParameters.PrimaryScreenWidth < 1100 || System.Windows.SystemParameters.PrimaryScreenHeight < 600)
      {
        o.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        o.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        o.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        o.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        o.MinHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        o.MinWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
      }
      else
      {
        o.Width = 1100;
        o.Height = 600;
      }

      o.ReadOnly = ReadOnly;

      o.Stato = Stato;



      o.Load(node.Attributes["ID"].Value, IDCliente, IDSessione);

      o.ShowDialog();
    }

    private void btn_Stato_Completato_Click(object sender, RoutedEventArgs e)
    {
      if (Stato == App.TipoTreeNodeStato.DaCompletare)
      {
        MessageBox.Show("La Carta di Lavoro è nello stato " + App.TipoTreeNodeStato.DaCompletare + ". Occorre selezionare Sblocca Stato per modificare il contenuto.");
        return;
      }

      ConfiguraStatoNodo(App.TipoTreeNodeStato.Completato, true);
    }

    private void btn_Stato_DaCompletare_Click(object sender, RoutedEventArgs e)
    {
      if (Stato == App.TipoTreeNodeStato.Completato)
      {
        MessageBox.Show("La Carta di Lavoro è nello stato " + App.TipoTreeNodeStato.Completato + ". Occorre selezionare Sblocca Stato per modificare il contenuto.");
        return;
      }

      ConfiguraStatoNodo(App.TipoTreeNodeStato.DaCompletare, true);
    }

    private void ConfiguraStatoNodo(App.TipoTreeNodeStato stato, bool uscita)
    {
      Stato = stato;
      base.Close();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (notcheckonexit == false)
      {
        if (!ReadOnly && Stato != App.TipoTreeNodeStato.Completato && Stato != App.TipoTreeNodeStato.DaCompletare)
        {
          MessageBox.Show("Assegnare uno STATO per uscire.");
          e.Cancel = true;
        }
      }
    }

    private void btn_GuidaRevisoft_Click(object sender, RoutedEventArgs e)
    {
      GuidaRevisoft(true);
    }

    private void GuidaRevisoft(bool posizioneMouse)
    {

      wGuidaRevisoft w = new wGuidaRevisoft();
      w.Owner = Window.GetWindow(this);

      if (System.Windows.SystemParameters.PrimaryScreenWidth < 1100 || System.Windows.SystemParameters.PrimaryScreenHeight < 600)
      {
        w.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        w.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        w.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        w.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        w.MinHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        w.MinWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
      }
      else
      {
        w.Width = 1100;
        w.Height = 600;
      }

      w.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

      if (nota != "" && nota != "<P align=left>&nbsp;</P>" && nota != "<P class=MsoNormal style=\"MARGIN: 0cm 0cm 0pt; LINE-HEIGHT: normal\" align=left>&nbsp;</P>")
      {
        w.testoHtml = nota;
      }
      else
      {
        w.testoHtml = "<html><body>Nessun aiuto disponibile per la Carta di Lavoro selezionata</body></html>";
      }

      w.MostraGuida();
      w.ShowDialog();
    }

    private void btn_Stato_SbloccaNodo_Click(object sender, RoutedEventArgs e)
    {
      XmlNode node = _x.Document.SelectSingleNode("//Dato[@ID=\"100003\"]");
      XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");

      if (App.AppSetupAlertSuCompletato == true)
      {
        if (NodoDato != null && NodoDato.Attributes["Stato"] != null && NodoDato.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString())
        {
          if (MessageBox.Show("Stai modificando lo stato di un argomento che risulta già completato. Vuoi continuare?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
          {
            return;
          }
        }
      }

      if (NodoDato != null && NodoDato.Attributes["Stato"] != null && NodoDato.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Scrittura)).ToString())
      {
        return;
      }

      btn_Stato_SbloccaNodo.Background = ButtonStatoSelectedColor;
      btn_Stato_DaCompletare.Background = btn_NodoHelp.Background;
      btn_Stato_Completato.Background = btn_NodoHelp.Background;

      switch (Stato)
      {
        case App.TipoTreeNodeStato.Completato:
          AnimateBackgroundColor(btn_Stato_Completato, Color.FromArgb(255, 247, 168, 39), ButtonToolBarSelectedColor, 1);
          break;
        case App.TipoTreeNodeStato.DaCompletare:
          AnimateBackgroundColor(btn_Stato_DaCompletare, Color.FromArgb(255, 247, 168, 39), ButtonToolBarSelectedColor, 1);
          break;
        default:
          break;
      }

      Stato = App.TipoTreeNodeStato.Scrittura;
      ReadOnly = false;
    }

    bool notcheckonexit = false;

    private void btn_Chiudi_Click(object sender, RoutedEventArgs e)
    {
      notcheckonexit = true;
      base.Close();
    }

    private void btn_CopiaDaAltraSessione_Click(object sender, RoutedEventArgs e)
    {
      if (ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }

      IndiceSessioni o = new IndiceSessioni();
      o.daPianificazione = true;

      XmlNode node = _x.Document.SelectSingleNode("//Dato[@ID=\"100003\"]");

      o.Tree = IDTree;
      o.Cliente = IDCliente;
      o.Sessione = IDSessione;
      o.Nodo = node.Attributes["ID"].Value;

      o.Owner = this;

      o.Load();

      o.ShowDialog();

      lastKey = "";
      lastData = "";
      htSessioni.Clear();

      foreach (XmlNode itemV in _x.Document.SelectNodes("//Dato[@ID=\"100003\"]/Valore"))
      {
        int index = 1;
        foreach (XmlNode item in itemV.SelectNodes("//Dato[@ID=\"100003\"]/Valore[@ID=\"" + itemV.Attributes["ID"].Value + "\"]/Pianificazione"))
        {
          item.Attributes["ID"].Value = index.ToString();
          lastKey = item.Attributes["ID"].Value;
          lastData = item.Attributes["Data"].Value;

          if (!htSessioni.Contains(lastKey))
          {
            htSessioni.Add(lastKey, lastData);
          }
          index++;
        };
      }

      UpdateGridCombo();
    }

    private void AnimateBackgroundColor(Button btn, Color from, Color to, int seconds)
    {
      SolidColorBrush brush = new SolidColorBrush(from);

      btn.Background = brush;
      System.Windows.Media.Animation.ColorAnimation a = new System.Windows.Media.Animation.ColorAnimation();
      a.From = from;
      a.To = to;
      a.Duration = new Duration(TimeSpan.FromSeconds(seconds));
      a.AutoReverse = true;
      btn.Background.BeginAnimation(SolidColorBrush.ColorProperty, a);
    }

    //private void btn_EsciSenzaSalvare_Click( object sender, RoutedEventArgs e )
    //{
    //    base.Close();
    //    return;
    //}
  }
}
