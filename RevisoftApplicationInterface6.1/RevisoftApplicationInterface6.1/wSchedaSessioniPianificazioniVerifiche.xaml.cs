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
using System.Data.SqlClient;
using System.Data;

namespace RevisoftApplication
{
  public partial class wSchedaSessioniPianificazioniVerifiche : Window
  {
    public int id;
    private DataTable dati = null;
    private DataTable datiTestata = null;

    public Hashtable htSessioni = new Hashtable();
    ArrayList sortingal = new ArrayList();
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

    public bool m_isModified = false; // E.B.


    public wSchedaSessioniPianificazioniVerifiche()
    {
      InitializeComponent();
      lab1.Foreground = App._arrBrushes[0];
      id = 100013;
    }

    public void ConfiguraMaschera()
    {
      cBusinessObjects.idcliente = int.Parse(IDCliente);
      cBusinessObjects.idsessione = int.Parse(IDSessione);
      labelTitolo.Text = "Sessioni nel periodo " + DataInizio + " - " + DataFine;
      dati = cBusinessObjects.GetData(id, typeof(PianificazioneVerifiche));
      datiTestata = cBusinessObjects.GetData(id, typeof(PianificazioneVerificheTestata));
      if (dati.Rows.Count == 0)
      {
        //controllo se esiste una vigilanza con le medesime date
        MasterFile mf = MasterFile.Create();
        ArrayList vigilanze = mf.GetPianificazioniVigilanze(IDCliente);

        string vigilanzaID = "-1";

        foreach (Hashtable item in vigilanze)
        {
          if (item["DataInizio"].ToString() == DataInizio && item["DataFine"].ToString() == DataFine)
          {
            vigilanzaID = item["ID"].ToString();
          }
        }

        if (vigilanzaID != "-1")
        {
          if (MessageBox.Show("Vuoi importare le stesse sessioni della vigilanza?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
          {
            Hashtable htSelected = mf.GetPianificazioniVerifica(vigilanzaID);

            if (htSelected.Count != 0)
            {
              DataTable pianificazioneSource = cBusinessObjects.GetData(100003, typeof(PianificazioneVerificheTestata), cBusinessObjects.idcliente, int.Parse(vigilanzaID), 27);
              cBusinessObjects.SaveData(100013, pianificazioneSource, typeof(PianificazioneVerificheTestata), -1, 26);
              datiTestata = cBusinessObjects.GetData(id, typeof(PianificazioneVerificheTestata));
            }
            lastKey = "0";
            foreach (DataRow itemV in datiTestata.Rows)
            {
              if (int.Parse(lastKey) < int.Parse(itemV["ID"].ToString()))
              {
                lastKey = itemV["ID"].ToString();
                generateTree(lastKey);
                lastData = itemV["Data"].ToString();
                if (!htSessioni.Contains(lastKey))
                {
                  htSessioni.Add(lastKey, lastData);

                }
              }
            }
          }
        }
        generateTree("0");


        datiTestata = cBusinessObjects.GetData(id, typeof(PianificazioneVerificheTestata));
        dati = cBusinessObjects.GetData(id, typeof(PianificazioneVerifiche));


      }

      UpdateGridCombo();

      switch (Stato)
      {
        case App.TipoTreeNodeStato.DaCompletare:
          btn_Stato_DaCompletare.Background = ButtonStatoSelectedColor;
          btn_Stato_Completato.Background = btn_NodoHelp.Background;
          ReadOnly = false;
          break;
        case App.TipoTreeNodeStato.Completato:
          btn_Stato_Completato.Background = ButtonStatoSelectedColor;
          btn_Stato_DaCompletare.Background = btn_NodoHelp.Background;
          ReadOnly = false;
          break;
        default:
          ReadOnly = false;
          break;
      }

    }

    public void generateTree(string P_ID)
    {
      cBusinessObjects.idcliente = int.Parse(IDCliente);
      cBusinessObjects.idsessione = int.Parse(IDSessione);
      RevisoftApplication.XmlManager x = new XmlManager();
      x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
      MasterFile mf = MasterFile.Create();

      string SelectedTreeSource = App.AppTemplateTreeVerifica;

      XmlDataProvider _xTXP = new XmlDataProvider();
      _xTXP.Document = cBusinessObjects.NewLoadEncodedFile(SelectedTreeSource, "2");

      try
      {
        int conta = 1;

        foreach (XmlNode item in _xTXP.Document.SelectNodes("/Tree//Node[@ID][@Codice][@Titolo]"))
        {
          string isTitolo = "";

          if (item.ChildNodes.Count > 1)
          {
            isTitolo = "Father=\"1\"";
          }

          bool trovato = false;
          foreach (DataRow dt in dati.Rows)
          {
            if (dt["PianificazioneID"].ToString() != P_ID)
              continue;
            if (dt["Codice"].ToString() == item.Attributes["Codice"].Value.Replace("&", "&amp;").Replace("\"", "'"))
              trovato = true;
          }

          if (!trovato)
          {
            DataRow dd = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            dd["ID"] = conta; // item.Attributes["ID"].Value;
            dd["NODE_ID"] = item.Attributes["ID"].Value;
            dd["PianificazioneID"] = P_ID; // item.Attributes["ID"].Value;
            conta++;
            dd["Titolo"] = item.Attributes["Titolo"].Value.Replace("&", "&amp;").Replace("\"", "'");
            if (isTitolo != "")
              dd["Father"] = "1";
            dd["Codice"] = item.Attributes["Codice"].Value.Replace("&", "&amp;").Replace("\"", "'");
            dd["Checked"] = "False";
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
        cBusinessObjects.SaveData(id, dati, typeof(PianificazioneVerifiche));

      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaSessioniPianificazioniVigilanze.generateTree exception");
        string log = ex.Message;
      }


    }


    private void sorting()
    {
      sortingal.Clear();

      foreach (DictionaryEntry item in htSessioni)
      {
        sortingal.Add(Convert.ToDateTime(item.Value));
      }

      sortingal.Sort();
    }

    private void UpdateGridCombo()
    {
      sorting();



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

        foreach (DateTime date in sortingal)
        {
          foreach (DictionaryEntry item in htSessioni)
          {
            if (date.ToShortDateString() != item.Value.ToString())
            {
              continue;
            }

            TextBlock txtb = new TextBlock();
            txtb.Text = "Sessione n. " + (indexrow + 1).ToString();
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

    void btnCancellaTutto_Click(object sender, RoutedEventArgs e)
    {

      if (MessageBox.Show("Tutte le sessioni di questa pianificazione verranno cancellate. Procedere?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
      {
        return;
      }
      dati.Clear();
      datiTestata.Clear();




      lastKey = "";
      lastData = "";
      htSessioni.Clear();

      m_isModified = true; // E.B.
      ConfiguraStatoNodo(App.TipoTreeNodeStato.Sconosciuto, false);
      notcheckonexit = true;
      cBusinessObjects.SaveData(id, dati, typeof(PianificazioneVerifiche));
      cBusinessObjects.SaveData(id, datiTestata, typeof(PianificazioneVerificheTestata));


      base.Close();
    }

    void btnCancella_Click(object sender, RoutedEventArgs e)
    {


      if (MessageBox.Show("Questa sessione verrà cancellata. Procedere?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
      {
        return;
      }

      string IDhere = ((Button)(sender)).Name.Split('_').Last();

      for (int i = dati.Rows.Count - 1; i >= 0; i--)
      {
        DataRow dtrow = dati.Rows[i];
        if (dtrow["PianificazioneID"].ToString() != IDhere)
          continue;
        if (dtrow["Checked"].ToString() == "True")
        {
          if (MessageBox.Show("Questa sessione contiene dei programmi di lavoro. Procedere con la cancellazione?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
          {
            return;
          }
          break;
        }
      }



      for (int i = dati.Rows.Count - 1; i >= 0; i--)
      {

        DataRow dtrow = dati.Rows[i];
        if (dtrow["PianificazioneID"].ToString() == IDhere)
          dtrow.Delete();
      }

      dati.AcceptChanges();
      for (int i = datiTestata.Rows.Count - 1; i >= 0; i--)
      {

        DataRow dtrow = datiTestata.Rows[i];
        if (dtrow["ID"].ToString() == IDhere)
          dtrow.Delete();
      }

      datiTestata.AcceptChanges();
      m_isModified = true; // E.B.

      lastKey = "1";
      lastData = "";
      htSessioni.Clear();

      foreach (DataRow item in datiTestata.Rows)
      {

        if (int.Parse(lastKey) < int.Parse(item["ID"].ToString()))
        {
          lastKey = item["ID"].ToString();
          lastData = item["Data"].ToString();
        }
        if (!htSessioni.Contains(item["ID"].ToString()))
        {
          htSessioni.Add(item["ID"].ToString(), item["Data"].ToString());
        }

      }

      cBusinessObjects.SaveData(id, dati, typeof(PianificazioneVerifiche));
      cBusinessObjects.SaveData(id, datiTestata, typeof(PianificazioneVerificheTestata));

      UpdateGridCombo();

    }

    void txt_LostFocus(object sender, RoutedEventArgs e)
    {



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
        cBusinessObjects.logger.Error(ex, "wSchedaSessioniPianificazioniVerifiche.txt_LostFocus exception");
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

      //string previous = ( Convert.ToInt32( IDhere ) - 1 ).ToString();
      //if ( htSessioni.ContainsKey( previous ) )
      //{
      //    if ( dt.CompareTo( Convert.ToDateTime( htSessioni[previous] ) ) < 0 )
      //    {
      //        MessageBox.Show( "Attenzione le date devono essere sequenziali." );
      //        //UpdateGridCombo();
      //        //return;
      //    }
      //}

      if (dt.CompareTo(Convert.ToDateTime(DataFine)) > 0)
      {
        MessageBox.Show("Attenzione: data successiva alla fine del periodo.");
        UpdateGridCombo();
        return;
      }

      //string next = ( Convert.ToInt32( IDhere ) + 1 ).ToString();
      //if ( htSessioni.ContainsKey( next ) )
      //{
      //    if ( dt.CompareTo( Convert.ToDateTime( htSessioni[next] ) ) > 0 )
      //    {
      //        MessageBox.Show( "Attenzione le date devono essere sequenziali." );
      //        //UpdateGridCombo();
      //        //return;
      //    }
      //}

      DataTable datipianificazione = cBusinessObjects.GetData(id, typeof(PianificazioneVerificheTestata));

      foreach (DataRow item in datipianificazione.Rows)
      {
        if (IDhere == item["ID"].ToString())
          item["Data"] = dt.ToShortDateString();
      }

      cBusinessObjects.SaveData(id, datipianificazione, typeof(PianificazioneVerificheTestata));


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




      try
      {
        string lastdatehere = "";

        if (lastData == "")
        {
          lastdatehere = DataInizio;
        }
        else
        {
          foreach (DictionaryEntry item in htSessioni)
          {
            DateTime dt = Convert.ToDateTime(item.Value);
            dt = dt.AddDays(1);

            string datehere = dt.ToShortDateString();
            if (htSessioni.ContainsValue(datehere))
            {
              continue;

            }
            else
            {
              lastdatehere = dt.ToShortDateString();
            }
          }
        }


        DataTable datipianificazioneT = cBusinessObjects.GetData(id, typeof(PianificazioneVerificheTestata));
        foreach (DataRow itemV in datipianificazioneT.Rows)
        {
          if (int.Parse(lastKey) < int.Parse(itemV["ID"].ToString()))
          {
            lastKey = itemV["ID"].ToString();
            lastData = itemV["Data"].ToString();
          }
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

        DataRow ddt = datipianificazioneT.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
        ddt["ID"] = lastKeyhere;
        ddt["Data"] = lastdatehere;

        cBusinessObjects.SaveData(id, datipianificazioneT, typeof(PianificazioneVerificheTestata));
        datiTestata = cBusinessObjects.GetData(id, typeof(PianificazioneVerificheTestata));
     
        generateTree(lastKeyhere);

        m_isModified = true;

        if (!htSessioni.Contains(lastKeyhere))
        {
          lastData = lastdatehere;
          lastKey = lastKeyhere;
          htSessioni.Add(lastKey, lastData);
        }


        UpdateGridCombo();

        //MessageBox.Show("Inserita data: " + lastdatehere);

      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wSchedaSessioniPianificazioniVigilanze.buttonAdd_Click exception");
        string log = ex.Message;
        return;
      }
    }

    private void buttonApri_Click(object sender, RoutedEventArgs e)
    {
      foreach (DictionaryEntry item in htSessioni)
      {
        DateTime dt = new DateTime();

        dt = Convert.ToDateTime(item.Value.ToString());

        if (dt.CompareTo(Convert.ToDateTime(DataInizio)) < 0)
        {
          MessageBox.Show("Attenzione: data antecedente.");
          e.Handled = true;
          return;
        }

        if (dt.CompareTo(Convert.ToDateTime(DataFine)) > 0)
        {
          MessageBox.Show("Attenzione: data posteriore.");
          e.Handled = true;
          return;
        }
      }

      wWorkAreaTree_PianificazioniVerifiche PVList = new wWorkAreaTree_PianificazioniVerifiche();
      PVList.IDP = "100013";
      PVList.Owner = this;
      PVList.TipoAttivita = App.TipoAttivita.Verifica;
      PVList.DataInizio = DataInizio;
      PVList.DataFine = DataFine;
      PVList.IDCliente = IDCliente;
      PVList.Cliente = Cliente;
      PVList._x = _x;

      PVList.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

      PVList.ReadOnly = ReadOnly;

      PVList.LoadTreeSource();
      PVList.ShowDialog();
      // E.B.
      if (!m_isModified) m_isModified = PVList.m_isModified;
    }

    //----------------------------------------------------------------------------+
    //                             btn_SOSPESI_Click                              |
    //----------------------------------------------------------------------------+
    private void btn_SOSPESI_Click(object sender, RoutedEventArgs e)
    {

      Sospesi o = new Sospesi();

      o.Owner = this;
      if (System.Windows.SystemParameters.PrimaryScreenWidth < 1100
        || System.Windows.SystemParameters.PrimaryScreenHeight < 600)
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

      o.Load("100013", IDCliente, IDSessione);
      o.ShowDialog();

    }

    private void btn_Stato_Completato_Click(object sender, RoutedEventArgs e)
    {
      cBusinessObjects.DeleteTree((Convert.ToInt32(App.TipoFile.Verifica)).ToString(), IDCliente);
      m_isModified = true;
      ConfiguraStatoNodo(App.TipoTreeNodeStato.Completato, true);
    }

    private void btn_Stato_DaCompletare_Click(object sender, RoutedEventArgs e)
    {

      m_isModified = true;
      ConfiguraStatoNodo(App.TipoTreeNodeStato.DaCompletare, true);
    }

    private void ConfiguraStatoNodo(App.TipoTreeNodeStato stato, bool uscita)
    {
      if (!m_isModified && Stato != stato) m_isModified = true;
      Stato = stato;
      if (uscita == true)
      {
        base.Close();
      }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (notcheckonexit == false)
      {
        if (Stato != App.TipoTreeNodeStato.Completato && Stato != App.TipoTreeNodeStato.DaCompletare)
        {
          MessageBox.Show("Assegnare uno STATO per uscire.");
          e.Cancel = true;
          return;
        }

        foreach (DictionaryEntry item in htSessioni)
        {
          DateTime dt = new DateTime();

          dt = Convert.ToDateTime(item.Value.ToString());

          if (dt.CompareTo(Convert.ToDateTime(DataInizio)) < 0)
          {
            MessageBox.Show("Attenzione: data antecedente.");//data antecedente all'inizio del periodo
            e.Cancel = true;
            return;
          }

          if (dt.CompareTo(Convert.ToDateTime(DataFine)) > 0)
          {
            MessageBox.Show("Attenzione: data successiva.");// data successiva alla fine del periodo
            e.Cancel = true;
            return;
          }
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
      return;
      XmlNode node = _x.Document.SelectSingleNode("//Dato[@ID=\"100013\"]");
      XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");

      if (App.AppSetupAlertSuCompletato == true)
      {
        if (NodoDato != null && NodoDato.Attributes["Stato"] != null && NodoDato.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString())
        {
          if (MessageBox.Show("Stai modificando lo stato di un argomento che risulta già completato. Vuoi continuare? \r\n (Se non vuoi questo messaggio, vai ad Impostazioni e deseleziona)", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
          {
            return;
          }
        }

        if (NodoDato != null && NodoDato.Attributes["Stato"] != null && NodoDato.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString())
        {
          if (MessageBox.Show("Stai modificando lo stato di un argomento che risulta già da completare. Vuoi continuare? \r\n (Se non vuoi questo messaggio, vai ad Impostazioni e deseleziona)", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
          {
            return;
          }
        }
      }

      if (NodoDato != null && NodoDato.Attributes["Stato"] != null && NodoDato.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Scrittura)).ToString())
      {
        return;
      }

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


      IndiceSessioni o = new IndiceSessioni();
      o.daPianificazione = true;

      XmlNode node = _x.Document.SelectSingleNode("//Dato[@ID=\"100013\"]");

      o.Tree = IDTree;
      o.Cliente = IDCliente;
      o.Sessione = IDSessione;
      o.Nodo = "100013";

      o.Owner = this;

      o.Load();

      o.ShowDialog();

      lastKey = "";
      lastData = "";
      htSessioni.Clear();


      foreach (DataRow itemV in datiTestata.Rows)
      {
        int index = 1;

        foreach (DataRow item in dati.Rows) //itemV.SelectNodes( "//Dato[@ID=\"100013\"]/Valore[@ID=\"" + itemV.Attributes["ID"].Value + "\"]/Pianificazione" ) )
        {
          if (item["ID"].ToString() != itemV["ID"].ToString())
            continue;
          item["PianificazioneID"] = index.ToString();
          lastKey = item["ID"].ToString();
          lastData = item["Data"].ToString();

          if (!htSessioni.Contains(lastKey))
          {
            htSessioni.Add(lastKey, lastData);
          }
          index++;
        };
      }
      cBusinessObjects.SaveData(id, dati, typeof(PianificazioneVerifiche));

      m_isModified = true; // E.B.
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
