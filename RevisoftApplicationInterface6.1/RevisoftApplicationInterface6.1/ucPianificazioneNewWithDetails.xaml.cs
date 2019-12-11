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
using System.Windows.Markup;
using System.IO;
using System.Data;

namespace UserControls
{
  public partial class ucPianificazioneNewWithDetails : UserControl
  {
    public int id;
    private DataTable datiN = null;
    private DataTable datiV = null;
    private DataTable datiVRighe = null;

    private string check = "./Images/icone/ana_stato_ok_blu.png";
    private string uncheck = "./Images/icone/check1-24x24.png";
    private string disabled = "./Images/icone/disabled.png";

    private string add = "./Images/icone/add2.png";
    private string remove = "./Images/icone/delete2.png";

    private string up = "./Images/icone/navigate_up.png";
    private string down = "./Images/icone/navigate_down.png";
    private string left = "./Images/icone/navigate_left.png";

    private XmlDataProviderManager _x;
    private string _ID = "-1";
    private string IDRischioGlobale = "22";
    private string IDB_Padre = "227";

    private bool alreadydonefirstbutton = false;
    private bool canbeexecuted = false;

    Hashtable Sessioni = new Hashtable();
    Hashtable SessioniTitoli = new Hashtable();
    Hashtable SessioniID = new Hashtable();
    int SessioneNow;
    string IDTree;
    string IDCliente;

    string IDSessione;
    private bool _ReadOnly = false;

    string bilancioAssociato = "";
    string bilancioTreeAssociato = "";
    string bilancioIDAssociato = "";
    XmlDataProviderManager _xBTree;
    //XmlDataProviderManager _xB;

    List<string> donehere = new List<string>();

    //private bool somethinghaschanged = true;

    Hashtable VociBilancio = new Hashtable();
    SortedDictionary<int, string> VociBilancio2 = new SortedDictionary<int, string>();

    //XmlNode xnodeOLD = null;

    public bool ReadOnly
    {
      set
      {
        _ReadOnly = value;
      }
    }

    public ucPianificazioneNewWithDetails()
    {
      VociBilancio.Add("80", "3.4.1");
      VociBilancio.Add("81", "3.4.2");
      VociBilancio.Add("82", "3.4.3");
      VociBilancio.Add("83", "3.4.4");
      VociBilancio.Add("85", "3.4.5");
      VociBilancio.Add("86", "3.4.6");
      VociBilancio.Add("87", "3.4.7");
      VociBilancio.Add("88", "3.4.8");
      VociBilancio.Add("89", "3.4.9");
      VociBilancio.Add("90", "3.4.10");
      VociBilancio.Add("91", "3.4.11");
      VociBilancio.Add("92", "3.4.12");
      VociBilancio.Add("93", "3.4.13");
      VociBilancio.Add("94", "3.4.14");
      VociBilancio.Add("95", "3.4.15");
      VociBilancio.Add("96", "3.4.16");
      VociBilancio.Add("97", "3.4.17");
      VociBilancio.Add("98", "3.4.18");
      VociBilancio.Add("99", "3.4.19");
      //VociBilancio.Add("100", "3.4.20");
      VociBilancio.Add("101", "3.4.21");
      //VociBilancio.Add("102", "3.4.22");

      VociBilancio2.Add(80, "3.4.1@Immobilizzazioni immateriali@1@1@1@1@1@1@1@0@pv");//txt3c
      VociBilancio2.Add(81, "3.4.2@Immobilizzazioni materiali@1@1@1@1@1@1@1@0@txt3c");
      VociBilancio2.Add(82, "3.4.3@Immobilizzazioni finanziarie@1@1@1@1@1@1@1@0@pv");
      VociBilancio2.Add(83, "3.4.4@Rimanenze di Magazzino@1@1@1@1@1@1@1@0@txt4c");
      VociBilancio2.Add(85, "3.4.5@Rimanenze - Opere a lungo termine@1@1@1@1@1@1@1@0@txt4c");
      VociBilancio2.Add(86, "3.4.6@Attività finanziarie non immobilizzate@1@1@1@1@1@1@1@0@txt5c");
      VociBilancio2.Add(87, "3.4.7@Crediti commerciali (Clienti)@1@1@1@1@1@1@1@0@txt2c");
      VociBilancio2.Add(88, "3.4.8@Crediti e debiti infragruppo@1@1@1@1@1@1@1@0@pv");
      VociBilancio2.Add(89, "3.4.9@Crediti tributari e per imposte differite attive@1@1@1@1@1@1@1@0@pv");
      VociBilancio2.Add(90, "3.4.10@Crediti verso altri@1@1@1@1@1@1@1@0@pv");
      VociBilancio2.Add(91, "3.4.11@Cassa e Banche@1@1@1@1@1@1@1@0@txt5c");
      VociBilancio2.Add(92, "3.4.12@Ratei e risconti (attivi e passivi)@1@1@1@1@1@1@1@0@pv");
      VociBilancio2.Add(93, "3.4.13@Patrimonio netto@@1@1@1@1@1@1@1@0@pv");
      VociBilancio2.Add(94, "3.4.14@Fondi per rischi ed oneri@1@1@1@1@1@1@1@0@pv");
      VociBilancio2.Add(95, "3.4.15@Fondo TFR (Trattamento Fine Rapporto)@1@1@1@1@1@1@1@0@txt6c");
      VociBilancio2.Add(96, "3.4.16@Mutui e finanziamenti non bancari@1@1@1@1@1@1@10ù@0@txt5c");
      VociBilancio2.Add(97, "3.4.17@Debiti commerciali (Fornitori)@1@1@1@1@1@1@1@0@txt3c");
      VociBilancio2.Add(98, "3.4.18@Debiti tributari e imposte differite passive@1@1@1@1@1@1@1@0@pv");
      VociBilancio2.Add(99, "3.4.19@Debiti verso altri@1@1@1@1@1@1@1@0@pv");
      //VociBilancio2.Add(100, "3.4.20@Conti d'ordine@1@1@1@1@1@1@1@0@pv");
      VociBilancio2.Add(101, "3.4.21@Conto economico@1@1@1@1@1@1@1@0@pv");
      //VociBilancio2.Add( 102, "3.4.22@Bilancio Consolidato@0@1@1@1@1@1@0@0@pv" );
      if (genericdescription == 0) { }
      InitializeComponent();
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

      double.TryParse(valore, out dblValore);

      if (dblValore == 0.0)
      {
        return "";
      }
      else
      {
        dblValore = dblValore * 100.0;
        return String.Format("{0:#,0.00}", dblValore);
      }
    }

    Hashtable b_valoreEA = new Hashtable();

    Hashtable b_NoData = new Hashtable();
    Hashtable b_Titolo = new Hashtable();
    ArrayList b_Ordine = new ArrayList();

    ArrayList b_Ordine_complete = new ArrayList();
    Hashtable b_Ordine_completeHT = new Hashtable();

    bool Materialità_1 = false;
    bool Materialità_2 = false;
    bool Materialità_3 = false;

    private XmlDataProviderManager _y = null;

    string tipoBilancio = "";

    public void Load_originale(string ID, string FileRevisione, Hashtable _Sessioni, Hashtable _SessioniTitoli, Hashtable _SessioniID, int _SessioneNow, string _IDTree, string _IDCliente, string _IDSessione)
    {

      id = int.Parse(ID);

      cBusinessObjects.idcliente = int.Parse(_IDCliente.ToString());
      cBusinessObjects.idsessione = int.Parse(_IDSessione.ToString());

      datiN = cBusinessObjects.GetData(id, typeof(PianificazioneNewWD_Node));
      datiV = cBusinessObjects.GetData(id, typeof(PianificazioneNewWD_Valore));

      // mette a posto bug delle colonne invertite

      bool NotenumericoRealR = true;
      bool NotenumericoNumber = true;
      bool notatrovata = false;
      int n;
      foreach (DataRow dd in datiV.Rows)
      {
        if (dd["NoteRealRow"].ToString() != "")
        {
          notatrovata = true;
          NotenumericoRealR = int.TryParse(dd["NoteRealRow"].ToString(), out n);
          NotenumericoNumber = int.TryParse(dd["NoteNumber"].ToString(), out n);
        }

        if (!NotenumericoRealR || !NotenumericoNumber)
          break;
      }

      if (notatrovata && (!NotenumericoRealR || !NotenumericoNumber))
      {

        foreach (DataRow dd in datiV.Rows)
        {
          string tempNoteRealRow = dd["NoteRealRow"].ToString();
          string tempNoteNumber = dd["NoteNumber"].ToString();
          string tempNote = dd["Note"].ToString();

          if (!NotenumericoRealR)
          {
            dd["Note"] = tempNoteRealRow;
            dd["NoteRealRow"] = tempNote;
          }
          if (!NotenumericoNumber)
          {
            dd["Note"] = tempNoteNumber;
            dd["NoteNumber"] = tempNote;
          }
          tempNoteRealRow = dd["NoteRealRow"].ToString();
          tempNoteNumber = dd["NoteNumber"].ToString();
          int NoteNumberInt = 0;
          int NoteRealRowInt = 0;
          int.TryParse(tempNoteRealRow, out NoteRealRowInt);
          int.TryParse(tempNoteNumber, out NoteNumberInt);
          if (NoteNumberInt > NoteRealRowInt)
          {
            dd["NoteRealRow"] = tempNoteNumber;
            dd["NoteNumber"] = tempNoteRealRow;
          }
        }

        datiV.AcceptChanges();
        cBusinessObjects.SaveData(id, datiV, typeof(PianificazioneNewWD_Valore));
      }


      datiVRighe = cBusinessObjects.GetData(id, typeof(PianificazioneNewWD_ValoreRighe));

      canbeexecuted = false;

      try
      {

        Sessioni = _Sessioni;
        SessioniTitoli = _SessioniTitoli;
        SessioniID = _SessioniID;
        SessioneNow = _SessioneNow;
        IDTree = _IDTree;
        IDCliente = _IDCliente;
        IDSessione = _IDSessione;


        _ID = ID;

        MasterFile mf = MasterFile.Create();
        bilancioAssociato = mf.GetBilancioAssociatoFromRevisioneFile(Sessioni[SessioneNow].ToString());
        bilancioTreeAssociato = mf.GetBilancioTreeAssociatoFromRevisioneFile(Sessioni[SessioneNow].ToString());
        bilancioIDAssociato = mf.GetBilancioIDAssociatoFromRevisioneFile(Sessioni[SessioneNow].ToString());

        brdPrima.Visibility = System.Windows.Visibility.Collapsed;
        brdSeconda.Visibility = System.Windows.Visibility.Collapsed;
        brdTerza.Visibility = System.Windows.Visibility.Collapsed;

        if (bilancioTreeAssociato != "")
        {
          _xBTree = new XmlDataProviderManager(bilancioTreeAssociato);
        }
        string idsessionebilancio = cBusinessObjects.CercaSessione("Revisione", "Bilancio", IDSessione, cBusinessObjects.idcliente);

        DataTable datibilanciotestata = cBusinessObjects.GetData(227, typeof(Excel_Bilancio_Testata), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);

        if (datibilanciotestata.Rows.Count == 0)
        {
          MessageBox.Show("Bilancio Ordinario Assente.");
          return;
        }
        tipoBilancio = "";
        foreach (DataRow dt in datibilanciotestata.Rows)
        {
          tipoBilancio = dt["tipoBilancio"].ToString();
        }



        switch (tipoBilancio)
        {
          case "2016":
            _y = new XmlDataProviderManager(App.AppLEAD2016, true);
            break;
          default:
            _y = new XmlDataProviderManager(App.AppLEAD, true);
            break;
        }

        b_valoreEA.Clear();

        RetrieveData();

        if (b_valoreEA.Count == 0)
        {
          MessageBox.Show("Bilancio Ordinario Assente.");
          return;
        }

        string ID_Materialità_1 = "77";
        string ID_Materialità_2 = "78";
        string ID_Materialità_3 = "199";


        DataTable tmpNode_true = null;


        string statomat = "";
        DataTable statom = null;
        DataTable datimaterialita = cBusinessObjects.GetData(int.Parse(ID_Materialità_1), typeof(Excel_LimiteMaterialitaSPCE));
        if (datimaterialita.Rows.Count > 0)
        {
          statomat = "";
          statom = cBusinessObjects.GetData(int.Parse(ID_Materialità_1), typeof(StatoNodi));
          foreach (DataRow dd in statom.Rows)
          {

            if (dd["Stato"].ToString() == "")
            {
              statomat = App.TipoTreeNodeStato.DaCompletare.ToString();
            }
            else
            {
              statomat = dd["Stato"].ToString();
            }
          }
          if (datimaterialita.Rows.Count > 0 && ((App.TipoTreeNodeStato)(Convert.ToInt32(statomat))) == App.TipoTreeNodeStato.Completato)
          {
            Materialità_1 = true;

            //   ucExcel_LimiteMaterialitaSPCE uce_lm = new ucExcel_LimiteMaterialitaSPCE();
            //   uce_lm.Load( ID_Materialità_1, FileRevisione, IpotesiMaterialita.Prima, IDCliente, IDSessione);
            //   uce_lm.Save();

            brdPrima.Visibility = System.Windows.Visibility.Visible;
            brdSeconda.Visibility = System.Windows.Visibility.Collapsed;
            brdTerza.Visibility = System.Windows.Visibility.Collapsed;
            tmpNode_true = datimaterialita;
          }
        }
        if (!Materialità_1)
        {

          datimaterialita = cBusinessObjects.GetData(int.Parse(ID_Materialità_2), typeof(Excel_LimiteMaterialitaSPCE));
          statom = cBusinessObjects.GetData(int.Parse(ID_Materialità_2), typeof(StatoNodi));
          foreach (DataRow dd in statom.Rows)
          {
            statomat = dd["Stato"].ToString().Trim();
          }
          if (datimaterialita.Rows.Count > 0 && ((App.TipoTreeNodeStato)(Convert.ToInt32(statomat))) == App.TipoTreeNodeStato.Completato)
          {
            Materialità_2 = true;

            //    ucExcel_LimiteMaterialitaSPCE uce_lm = new ucExcel_LimiteMaterialitaSPCE();
            //   uce_lm.Load( ID_Materialità_2, FileRevisione, IpotesiMaterialita.Seconda, IDCliente, IDSessione);
            //    uce_lm.Save();

            brdPrima.Visibility = System.Windows.Visibility.Collapsed;
            brdSeconda.Visibility = System.Windows.Visibility.Visible;
            brdTerza.Visibility = System.Windows.Visibility.Collapsed;
            tmpNode_true = datimaterialita;
          }
        }

        if (!Materialità_2)
        {
          datimaterialita = cBusinessObjects.GetData(int.Parse(ID_Materialità_3), typeof(Excel_LimiteMaterialitaSPCE));
          statom = cBusinessObjects.GetData(int.Parse(ID_Materialità_3), typeof(StatoNodi));
          foreach (DataRow dd in statom.Rows)
          {
            statomat = dd["Stato"].ToString().Trim();
          }
          if (datimaterialita.Rows.Count > 0 && ((App.TipoTreeNodeStato)(Convert.ToInt32(statomat))) == App.TipoTreeNodeStato.Completato)
          {
            Materialità_3 = true;

            //     ucExcel_LimiteMaterialitaSPCE uce_lm = new ucExcel_LimiteMaterialitaSPCE();
            //      uce_lm.Load(ID_Materialità_3, FileRevisione, IpotesiMaterialita.Terza, IDCliente, IDSessione);
            //      uce_lm.Save();

            brdPrima.Visibility = System.Windows.Visibility.Collapsed;
            brdSeconda.Visibility = System.Windows.Visibility.Collapsed;
            brdTerza.Visibility = System.Windows.Visibility.Visible;
            tmpNode_true = datimaterialita;
          }
        }



        if (tmpNode_true != null)
        {

          foreach (DataRow dtrow in tmpNode_true.Rows)
          {
            if (dtrow["ID"].ToString() == "txt7BILANCIO")
              txt7.Text = dtrow["value"].ToString();
            if (dtrow["ID"].ToString() == "txt7_2spBILANCIO")
              txt7_2sp.Text = dtrow["value"].ToString();
            if (dtrow["ID"].ToString() == "txt7_2ceBILANCIO")
              txt7_2ce.Text = dtrow["value"].ToString();
            if (dtrow["ID"].ToString() == "txt7_3spBILANCIO")
              txt7_3sp.Text = dtrow["value"].ToString();
            if (dtrow["ID"].ToString() == "txt7_3ceBILANCIO")
              txt7_3ce.Text = dtrow["value"].ToString();
            if (dtrow["ID"].ToString() == "txt9BILANCIO")
              txt9.Text = dtrow["value"].ToString();
            if (dtrow["ID"].ToString() == "txt9_2spBILANCIO")
              txt9_2sp.Text = dtrow["value"].ToString();
            if (dtrow["ID"].ToString() == "txt9_2ceBILANCIO")
              txt9_2ce.Text = dtrow["value"].ToString();
            if (dtrow["ID"].ToString() == "txt9_3spBILANCIO")
              txt9_3sp.Text = dtrow["value"].ToString();
            if (dtrow["ID"].ToString() == "txt9_3ceBILANCIO")
              txt9_3ce.Text = dtrow["value"].ToString();
            if (dtrow["ID"].ToString() == "txt12BILANCIO")
              txt12.Text = dtrow["value"].ToString();
            if (dtrow["ID"].ToString() == "txt12_2spBILANCIO")
              txt12_2sp.Text = dtrow["value"].ToString();
            if (dtrow["ID"].ToString() == "txt12_2ceBILANCIO")
              txt12_2ce.Text = dtrow["value"].ToString();
            if (dtrow["ID"].ToString() == "txt12_3spBILANCIO")
              txt12_3sp.Text = dtrow["value"].ToString();
            if (dtrow["ID"].ToString() == "txt12_3ceBILANCIO")
              txt12_3ce.Text = dtrow["value"].ToString();

          }

        }




        #region   primo blocco

        DataTable datiRishcioGlobale = cBusinessObjects.GetData(int.Parse(IDRischioGlobale), typeof(RischioGlobale));

        DataRow dtrischioglobale = null;
        foreach (DataRow node in datiRishcioGlobale.Rows)
        {

          txt1.Text = node["txt1"].ToString().ToUpper();
          txt3.Text = node["txt3"].ToString().ToUpper();
          txt3c.Text = node["txt3c"].ToString().ToUpper();
          txt4.Text = node["txt4"].ToString().ToUpper();
          txt4c.Text = node["txt4c"].ToString().ToUpper();
          txt6.Text = node["txt6"].ToString().ToUpper();
          txt6c.Text = node["txt6c"].ToString().ToUpper();
          txt5.Text = node["txt5"].ToString().ToUpper();
          txt5c.Text = node["txt5c"].ToString().ToUpper();
          txt2.Text = node["txt2"].ToString().ToUpper();
          txt2c.Text = node["txt2c"].ToString().ToUpper();
          dtrischioglobale = node;

        }
        #endregion


        #region secondo blocco

        Border brtst = new Border();
        brtst.BorderThickness = new Thickness(1);
        brtst.BorderBrush = Brushes.Black;
        brtst.Padding = new Thickness(10, 10, 10, 0);
        brtst.Margin = new Thickness(0, 0, 0, 5);
        brtst.HorizontalAlignment = HorizontalAlignment.Center;

        StackPanel stk3 = new StackPanel();
        stk3.Orientation = Orientation.Vertical;
        stk3.HorizontalAlignment = HorizontalAlignment.Center;

        StackPanel stk = new StackPanel();
        stk.Orientation = Orientation.Horizontal;
        stk.HorizontalAlignment = HorizontalAlignment.Center;

        TextBlock txtblk = new TextBlock();
        txtblk.Text = "Legenda: VEDI SUGGERIMENTI";
        txtblk.FontWeight = FontWeights.Bold;
        txtblk.Margin = new Thickness(10, 0, 0, 10);
        txtblk.TextAlignment = TextAlignment.Center;
        stk.Children.Add(txtblk);

        //StackPanel stk4 = new StackPanel();
        //stk4.Orientation = Orientation.Horizontal;

        //TextBlock txtblk = new TextBlock();
        //txtblk.Text = "Legenda sino alla versione 4.12.2: A";
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


        //txtblk = new TextBlock();
        //txtblk.Text = "Legenda : A";
        //txtblk.FontWeight = FontWeights.Bold;
        //txtblk.Margin = new Thickness(10, 0, 0, 10);
        //stk4.Children.Add(txtblk);

        //txtblk = new TextBlock();
        //txtblk.Text = "= Ispezione";
        //txtblk.Margin = new Thickness(5, 0, 0, 10);
        //stk4.Children.Add(txtblk);

        //txtblk = new TextBlock();
        //txtblk.Text = "B";
        //txtblk.FontWeight = FontWeights.Bold;
        //txtblk.Margin = new Thickness(10, 0, 0, 10);
        //stk4.Children.Add(txtblk);

        //txtblk = new TextBlock();
        //txtblk.Text = "= Osservazione";
        //txtblk.Margin = new Thickness(5, 0, 0, 10);
        //stk4.Children.Add(txtblk);

        //txtblk = new TextBlock();
        //txtblk.Text = "C";
        //txtblk.FontWeight = FontWeights.Bold;
        //txtblk.Margin = new Thickness(10, 0, 0, 10);
        //stk4.Children.Add(txtblk);

        //txtblk = new TextBlock();
        //txtblk.Text = "= Conferma esterna";
        //txtblk.Margin = new Thickness(5, 0, 0, 10);
        //stk4.Children.Add(txtblk);

        //txtblk = new TextBlock();
        //txtblk.Text = "D";
        //txtblk.FontWeight = FontWeights.Bold;
        //txtblk.Margin = new Thickness(10, 0, 0, 10);
        //stk4.Children.Add(txtblk);

        //txtblk = new TextBlock();
        //txtblk.Text = "= Ricalcolo";
        //txtblk.Margin = new Thickness(5, 0, 0, 10);
        //stk4.Children.Add(txtblk);

        //txtblk = new TextBlock();
        //txtblk.Text = "E";
        //txtblk.FontWeight = FontWeights.Bold;
        //txtblk.Margin = new Thickness(10, 0, 0, 10);
        //stk4.Children.Add(txtblk);

        //txtblk = new TextBlock();
        //txtblk.Text = "= Riesecuzione";
        //txtblk.Margin = new Thickness(5, 0, 0, 10);
        //stk4.Children.Add(txtblk);

        //txtblk = new TextBlock();
        //txtblk.Text = "F";
        //txtblk.FontWeight = FontWeights.Bold;
        //txtblk.Margin = new Thickness(10, 0, 0, 10);
        //stk4.Children.Add(txtblk);

        //txtblk = new TextBlock();
        //txtblk.Text = "= Procedure di analisi comparativa";
        //txtblk.Margin = new Thickness(5, 0, 0, 10);
        //stk4.Children.Add(txtblk);

        //txtblk = new TextBlock();
        //txtblk.Text = "G";
        //txtblk.FontWeight = FontWeights.Bold;
        //txtblk.Margin = new Thickness(10, 0, 0, 10);
        //stk4.Children.Add(txtblk);

        //txtblk = new TextBlock();
        //txtblk.Text = "= Indagine";
        //txtblk.Margin = new Thickness(5, 0, 0, 10);
        //stk4.Children.Add(txtblk);



        //stk3.Children.Add(stk4);


        stk3.Children.Add(stk);
        stk = new StackPanel();
        stk.Orientation = Orientation.Horizontal;
        stk.HorizontalAlignment = HorizontalAlignment.Center;

        txtblk = new TextBlock();
        txtblk.Text = "ET";
        txtblk.FontWeight = FontWeights.Bold;
        txtblk.Margin = new Thickness(10, 0, 0, 5);
        stk.Children.Add(txtblk);

        txtblk = new TextBlock();
        txtblk.Text = "= Errore Trascurabile";
        txtblk.Margin = new Thickness(5, 0, 0, 5);
        stk.Children.Add(txtblk);

        txtblk = new TextBlock();
        txtblk.Text = "MO";
        txtblk.FontWeight = FontWeights.Bold;
        txtblk.Margin = new Thickness(10, 0, 0, 5);
        stk.Children.Add(txtblk);

        txtblk = new TextBlock();
        txtblk.Text = "= Materialità Operativa";
        txtblk.Margin = new Thickness(5, 0, 0, 5);
        stk.Children.Add(txtblk);

        stk3.Children.Add(stk);


        brtst.Child = stk3;

        brdDefinizione.Children.Add(brtst);

        b_Ordine_completeHT.Clear();

        string file = "";

        foreach (KeyValuePair<int, string> item in VociBilancio2)
        {
          bool hasdata = false;
          string Codice = item.Value.ToString().Split('@')[0];
          if (item.Key.ToString() == "91")
          {
            //int a = 1;
          }
          //PATRIMONIALE ATTIVO
          switch (tipoBilancio)
          {
            case "2016":
              file = App.AppTemplateBilancio_Attivo2016;
              break;
            default:
              file = App.AppTemplateBilancio_Attivo;
              break;
          }

          GenerateData(file, Codice, "PATRIMONIALE ATTIVO", true, false);

          if (CheckExistence(Codice))
          {
            hasdata = true;
            //stpMain.Children.Add(GenerateGrid("PATRIMONIALE ATTIVO"));
          }

          bool check345 = hasdata;

          //PATRIMONIALE PASSIVO
          switch (tipoBilancio)
          {
            case "2016":
              file = App.AppTemplateBilancio_Passivo2016;
              break;
            default:
              file = App.AppTemplateBilancio_Passivo;
              break;
          }

          GenerateData(file, Codice, "PATRIMONIALE PASSIVO", true, false);

          if (CheckExistence(Codice))
          {
            hasdata = true;
            //stpMain.Children.Add(GenerateGrid("PATRIMONIALE PASSIVO"));
          }

          //CONTO ECONOMICO
          switch (tipoBilancio)
          {
            case "2016":
              file = App.AppTemplateBilancio_ContoEconomico2016;
              break;
            default:
              file = App.AppTemplateBilancio_ContoEconomico;
              break;
          }

          GenerateData(file, Codice, "CONTO ECONOMICO", true, false);

          if (Codice == "3.4.5" && check345 == false)
          {
            ;
          }
          else
          {

            if (CheckExistence(Codice))
            {
              hasdata = true;
              //stpMain.Children.Add(GenerateGrid("PATRIMONIALE PASSIVO"));
            }
          }

          if (hasdata == false)
          {
            continue;
          }

          DataRow tmpnode = null;

          foreach (DataRow dd in datiN.Rows)
          {
            if (dd["ID"].ToString() == item.Key.ToString())
            {
              tmpnode = dd;
            }
          }
          if (tmpnode == null)
          {
            tmpnode = datiN.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            tmpnode["ID"] = item.Key.ToString();
            tmpnode["Voce"] = Codice;
            tmpnode["Chiuso"] = "True";
            tmpnode["Titolo"] = item.Value.ToString().Split('@')[1].Replace("&", "&amp;").Replace("\"", "'");

          }



          Border b = new Border();
          b.CornerRadius = new CornerRadius(5.0);
          b.BorderBrush = Brushes.Blue;
          b.BorderThickness = new Thickness(2.0);
          b.Padding = new Thickness(4.0);
          b.Margin = new Thickness(4.0);

          Grid g = new Grid();
          g.Name = "Grid_" + item.Key.ToString();

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
          i.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

          g.Children.Add(i);

          StackPanel sp3 = new StackPanel();
          sp3.Orientation = Orientation.Horizontal;

          TextBlock txt = new TextBlock();
          txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          txt.Text = tmpnode["Voce"].ToString() + " - " + tmpnode["Titolo"].ToString();
          txt.Tag = tmpnode["ID"].ToString();

          txt.FontSize = 13;
          txt.FontWeight = FontWeights.Bold;
          txt.Margin = new Thickness(5.0);
          txt.Foreground = Brushes.Gray;

          sp3.Children.Add(txt);
          sp3.SetValue(Grid.RowProperty, 0);
          sp3.SetValue(Grid.ColumnProperty, 1);

          g.Children.Add(sp3);

          Grid grd = new Grid();

          cd = new ColumnDefinition();
          cd.Width = new GridLength(1, GridUnitType.Star);
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
          rd.Height = GridLength.Auto;
          grd.RowDefinitions.Add(rd);

          txt = new TextBlock();
          txt.Text = "Esecutore / Personale assegnato";
          grd.Children.Add(txt);
          Grid.SetRow(txt, 0);
          Grid.SetColumn(txt, 0);
          Grid.SetColumnSpan(txt, 4);

          rd = new RowDefinition();
          rd.Height = GridLength.Auto;
          grd.RowDefinitions.Add(rd);

          TextBox tb = new TextBox();
          tb.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
          tb.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          tb.LostFocus += new RoutedEventHandler(tbEsecutore_LostFocus);
          tb.TextWrapping = TextWrapping.Wrap;
          tb.AcceptsReturn = true;
          tb.HorizontalAlignment = HorizontalAlignment.Left;
          tb.Margin = new Thickness(0);
          tb.Name = "_" + item.Key.ToString() + "_Esecutore";
          if (tmpnode != null && tmpnode["Esecutore"].ToString() != "")
          {
            tb.Text = tmpnode["Esecutore"].ToString();
          }

          this.RegisterName(tb.Name, tb);

          grd.Children.Add(tb);
          Grid.SetRow(tb, 1);
          Grid.SetColumn(tb, 0);
          Grid.SetColumnSpan(tb, 4);

          rd = new RowDefinition();
          rd.Height = GridLength.Auto;
          grd.RowDefinitions.Add(rd);

          txt = new TextBlock();
          txt.Text = "Note";
          grd.Children.Add(txt);
          Grid.SetRow(txt, 2);
          Grid.SetColumn(txt, 0);
          Grid.SetColumnSpan(txt, 4);

          rd = new RowDefinition();
          rd.Height = GridLength.Auto;
          grd.RowDefinitions.Add(rd);

          tb = new TextBox();
          tb.Name = "_" + item.Key.ToString() + "_Nota";

          if (tmpnode != null && tmpnode["Nota"].ToString() != "")
          {
            tb.Text = tmpnode["Nota"].ToString();
          }
          else
          {
            tb.Text = "";
          }

          tb.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
          tb.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
          tb.LostFocus += new RoutedEventHandler(tbNota_LostFocus);
          tb.TextWrapping = TextWrapping.Wrap;
          tb.AcceptsReturn = true;
          tb.Foreground = Brushes.Blue;

          this.RegisterName(tb.Name, tb);

          grd.Children.Add(tb);
          Grid.SetRow(tb, 3);
          Grid.SetColumn(tb, 0);
          Grid.SetColumnSpan(tb, 4);

          rd = new RowDefinition();
          rd.Height = GridLength.Auto;
          grd.RowDefinitions.Add(rd);

          rd = new RowDefinition();
          rd.Height = GridLength.Auto;
          grd.RowDefinitions.Add(rd);

          Border brd2 = new Border();
          brd2.BorderBrush = Brushes.Gray;
          brd2.BorderThickness = new Thickness(1.0);
          brd2.HorizontalAlignment = HorizontalAlignment.Center;
          brd2.Margin = new Thickness(10.0);
          brd2.Padding = new Thickness(10.0);

          StackPanel stpbrdV = new StackPanel();
          stpbrdV.Orientation = Orientation.Vertical;

          txt = new TextBlock();
          txt.Text = "Rischio di individuazione";
          txt.FontWeight = FontWeights.Bold;
          txt.Margin = new Thickness(0, 0, 0, 5);
          //grd.Children.Add(txt);
          //Grid.SetRow(txt, 4);
          //Grid.SetColumn(txt, 0);
          //Grid.SetColumnSpan(txt, 4);
          stpbrdV.Children.Add(txt);





          StackPanel stpbrd = new StackPanel();
          stpbrd.Orientation = Orientation.Horizontal;

          txt = new TextBlock();
          txt.Text = "Proposto";
          txt.Margin = new Thickness(0, 0, 10, 0);

          stpbrd.Children.Add(txt);

          //grd.Children.Add(txt);
          //Grid.SetRow(txt, 5);
          //Grid.SetColumn(txt, 0);

          txt = new TextBlock();
          txt.Margin = new Thickness(10, 0, 10, 0);
          //txt.Background = Brushes.LightGray;
          txt.Width = 150;
          txt.FontWeight = FontWeights.Bold;
          txt.Margin = new Thickness(0.0, 0.0, 10.0, 0.0);

          if (item.Value.ToString().Split('@')[10] == "pv")
          {
            txt.Text = "Procedure Validità";
          }
          else
          {
            txt.Text = "Procedure Validità";
            if (dtrischioglobale != null)
            {

              try
              {
                switch (((dtrischioglobale[item.Value.ToString().Split('@')[10]].ToString() == "") ? "6" : dtrischioglobale[item.Value.ToString().Split('@')[10]].ToString()))
                {
                  case "Molto Alto":
                    txt.Text = "Molto Alto";
                    break;
                  case "Alto":
                    txt.Text = "Alto";
                    break;
                  case "Medio":
                    txt.Text = "Medio";
                    break;
                  case "Basso":
                    txt.Text = "Basso";
                    break;
                  case "Molto Basso":
                    txt.Text = "Molto Basso";
                    break;
                  default:
                    txt.Text = "Procedure Validità";
                    break;
                }
              }
              catch (Exception)
              {
              }

            }

          }




          if (tmpnode != null && tmpnode["cmbRI_Proposto"].ToString() != "")
          {
            tmpnode["cmbRI_Proposto"] = txt.Text;
          }

          stpbrd.Children.Add(txt);

          //grd.Children.Add(txt);
          //Grid.SetRow(txt, 5);
          //Grid.SetColumn(txt, 1);


          txt = new TextBlock();
          txt.Text = "Scelto";

          stpbrd.Children.Add(txt);

          //grd.Children.Add(txt);
          //Grid.SetRow(txt, 5);
          //Grid.SetColumn(txt, 2);

          ComboBox newCombo = new ComboBox();
          newCombo.Name = "_" + item.Key.ToString() + "_ComboBoxRI_" + item.Value.ToString().Split('@')[10];

          this.RegisterName(newCombo.Name, newCombo);

          if (alreadydonefirstbutton == false)
          {
            alreadydonefirstbutton = true;
            newCombo.SelectionChanged += new SelectionChangedEventHandler(changeAll);
          }

          newCombo.SelectionChanged += new SelectionChangedEventHandler(cmbRI_Changed);

          newCombo.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
          newCombo.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          newCombo.Margin = new Thickness(10, 0, 0, 0);



          ComboBoxItem newitem = new ComboBoxItem();
          newitem.HorizontalContentAlignment = HorizontalAlignment.Center;
          newitem.Content = "MA  - Molto Alto";
          newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "A    - Alto";
          newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "M    - Medio";
          newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "B    - Basso";
          newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "MB  - Molto Basso";
          newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "PV  - Proced Validità";
          newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "NA  - Non Applicabile";
          newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "*    - Ripristina R.I. Automatico";
          newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "?";
          newCombo.Items.Add(newitem);

          if (tmpnode != null && tmpnode["cmbRI"].ToString() != "" && tmpnode["cmbRI"].ToString() != "")
          {
            int selecteditem = 0;

            switch (tmpnode["cmbRI"].ToString())
            {
              case "MA":
                selecteditem = 0;
                break;
              case "A":
                selecteditem = 1;
                break;
              case "M":
                selecteditem = 2;
                break;
              case "B":
                selecteditem = 3;
                break;
              case "MB":
                selecteditem = 4;
                break;
              case "PV":
                selecteditem = 5;
                break;
              case "NA":
                selecteditem = 6;
                break;
              default:
                selecteditem = 8;
                break;
            }

            newCombo.SelectedItem = ((ComboBoxItem)newCombo.Items[selecteditem]);
            newCombo.Text = ((ComboBoxItem)newCombo.Items[selecteditem]).Content.ToString();
          }
          else
          {
            if (item.Value.ToString().Split('@')[10] == "pv")
            {
              newCombo.SelectedItem = ((ComboBoxItem)newCombo.Items[5]);
              newCombo.Text = ((ComboBoxItem)newCombo.Items[5]).Content.ToString();
            }
            else
            {
              int selecteditem = 0;
              if (dtrischioglobale != null)
              {

                try
                {
                  switch (((dtrischioglobale[item.Value.ToString().Split('@')[10]] == null) ? "6" : dtrischioglobale[item.Value.ToString().Split('@')[10]].ToString()))
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
                      selecteditem = 8;
                      break;
                  }
                }
                catch (Exception)
                {

                }
              }

              newCombo.SelectedItem = ((ComboBoxItem)newCombo.Items[selecteditem]);
              newCombo.Text = ((ComboBoxItem)newCombo.Items[selecteditem]).Content.ToString();

            }
          }


          newCombo.Width = 200;

          stpbrd.Children.Add(newCombo);

          stpbrdV.Children.Add(stpbrd);

          brd2.Child = stpbrdV;

          grd.Children.Add(brd2);
          Grid.SetRow(brd2, 4);
          Grid.SetRowSpan(brd2, 2);
          Grid.SetColumn(brd2, 0);
          Grid.SetColumnSpan(brd2, 3);

          //grd.Children.Add(newCombo);
          //Grid.SetRow(newCombo, 5);
          //Grid.SetColumn(newCombo, 3);

          rd = new RowDefinition();
          grd.RowDefinitions.Add(rd);

          Grid grdNew = new Grid();
          grdNew.Margin = new Thickness(0.0, 10.0, 0.0, 0.0);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(20, GridUnitType.Pixel);
          grdNew.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(1, GridUnitType.Star);
          grdNew.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(120, GridUnitType.Pixel);
          grdNew.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(50, GridUnitType.Pixel);
          grdNew.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(50, GridUnitType.Pixel);
          grdNew.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(70, GridUnitType.Pixel);
          grdNew.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(50, GridUnitType.Pixel);
          grdNew.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(30, GridUnitType.Pixel);
          grdNew.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(30, GridUnitType.Pixel);
          grdNew.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(30, GridUnitType.Pixel);
          grdNew.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(30, GridUnitType.Pixel);
          grdNew.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(30, GridUnitType.Pixel);
          grdNew.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(30, GridUnitType.Pixel);
          grdNew.ColumnDefinitions.Add(cd);

          cd = new ColumnDefinition();
          cd.Width = new GridLength(80, GridUnitType.Pixel);
          grdNew.ColumnDefinitions.Add(cd);

          //cd = new ColumnDefinition();
          //cd.Width = new GridLength(1, GridUnitType.Star);
          //grdNew.ColumnDefinitions.Add(cd);

          rd = new RowDefinition();
          rd.Height = GridLength.Auto;
          grdNew.RowDefinitions.Add(rd);

          rd = new RowDefinition();
          rd.Height = GridLength.Auto;
          grdNew.RowDefinitions.Add(rd);

          Border brdfc = new Border();
          brdfc.BorderBrush = Brushes.DarkGray;
          brdfc.BorderThickness = new Thickness(1, 1, 0, 0);

          txt = new TextBlock();
          txt.TextAlignment = TextAlignment.Center;
          txt.Text = "VOCI DI BILANCIO";
          txt.FontWeight = FontWeights.Bold;

          brdfc.Child = txt;

          grdNew.Children.Add(brdfc);
          Grid.SetRow(brdfc, 0);
          Grid.SetRowSpan(brdfc, 1);
          Grid.SetColumn(brdfc, 0);
          Grid.SetColumnSpan(brdfc, 2);

          brdfc = new Border();
          brdfc.BorderBrush = Brushes.DarkGray;
          brdfc.BorderThickness = new Thickness(0, 1, 1, 0);

          txt = new TextBlock();
          txt.TextAlignment = TextAlignment.Center;
          txt.Text = "VALORE";
          txt.FontWeight = FontWeights.Bold;

          brdfc.Child = txt;

          grdNew.Children.Add(brdfc);
          Grid.SetRow(brdfc, 0);
          Grid.SetRowSpan(brdfc, 1);
          Grid.SetColumn(brdfc, 2);

          brdfc = new Border();
          brdfc.BorderBrush = Brushes.DarkGray;
          brdfc.BorderThickness = new Thickness(1, 1, 0, 0);
          brdfc.Margin = new Thickness(10, 0, 0, 0);

          txt = new TextBlock();
          txt.TextAlignment = TextAlignment.Center;
          txt.Text = "INFERIORE A";
          txt.FontWeight = FontWeights.Bold;

          brdfc.Child = txt;

          grdNew.Children.Add(brdfc);
          Grid.SetRow(brdfc, 0);
          Grid.SetColumn(brdfc, 3);
          Grid.SetColumnSpan(brdfc, 2);

          brdfc = new Border();
          brdfc.BorderBrush = Brushes.DarkGray;
          brdfc.BorderThickness = new Thickness(0, 1, 1, 0);
          brdfc.Padding = new Thickness(0, 0, 3, 0);

          txt = new TextBlock();
          txt.TextAlignment = TextAlignment.Center;
          txt.FontSize = 0.9 * txt.FontSize;
          txt.Text = "CONTROLLO";
          txt.FontWeight = FontWeights.Bold;

          brdfc.Child = txt;

          grdNew.Children.Add(brdfc);
          Grid.SetRow(brdfc, 0);
          Grid.SetRowSpan(brdfc, 1);
          Grid.SetColumn(brdfc, 5);

          brdfc = new Border();
          brdfc.BorderBrush = Brushes.DarkGray;
          brdfc.BorderThickness = new Thickness(1, 1, 1, 0);
          brdfc.Margin = new Thickness(10, 0, 0, 0);

          txt = new TextBlock();
          txt.TextAlignment = TextAlignment.Center;
          txt.Text = "EVIDENZE";
          txt.FontWeight = FontWeights.Bold;

          brdfc.Child = txt;

          grdNew.Children.Add(brdfc);
          Grid.SetRow(brdfc, 0);
          Grid.SetColumn(brdfc, 6);
          Grid.SetColumnSpan(brdfc, 7);//8

          txt = new TextBlock();
          txt.TextAlignment = TextAlignment.Center;
          txt.Text = "NOTE";
          txt.FontWeight = FontWeights.Bold;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 0);
          Grid.SetRowSpan(txt, 1);
          Grid.SetColumn(txt, 13);//14

          txt = new TextBlock();
          txt.Text = "ET";
          txt.ToolTip = "ERRORE TRASCURABILE";
          txt.TextAlignment = TextAlignment.Center;
          txt.FontWeight = FontWeights.Bold;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 3);

          txt = new TextBlock();
          txt.Text = "MO";
          txt.ToolTip = "MATERIALITA' OPERATIVA";
          txt.TextAlignment = TextAlignment.Center;
          txt.FontWeight = FontWeights.Bold;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 4);

          txt = new TextBlock();
          txt.Text = "A";
          txt.ToolTip = "ISPEZIONE";
          txt.TextAlignment = TextAlignment.Center;
          txt.Margin = new Thickness(20, 0, 0, 0);
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 6);

          txt = new TextBlock();
          txt.Text = "B";
          txt.ToolTip = "OSSERVAZIONE";
          txt.TextAlignment = TextAlignment.Center;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 7);

          txt = new TextBlock();
          txt.Text = "C";
          txt.ToolTip = "CONFERMA ESTERNA /DOCUMENTAZIONE";
          txt.TextAlignment = TextAlignment.Center;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 8);

          txt = new TextBlock();
          txt.Text = "D";
          txt.ToolTip = "RICALCOLO";
          txt.TextAlignment = TextAlignment.Center;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 9);

          txt = new TextBlock();
          txt.Text = "E";
          txt.ToolTip = "RIESECUZIONE";
          txt.TextAlignment = TextAlignment.Center;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 10);

          txt = new TextBlock();
          txt.Text = "F";
          txt.ToolTip = "PROCEDURE DI ANALISI COMPARATIVA";
          txt.TextAlignment = TextAlignment.Center;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 11);

          txt = new TextBlock();
          txt.Text = "G";
          txt.ToolTip = "INDAGINE";
          txt.TextAlignment = TextAlignment.Center;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 12);

          //txt = new TextBlock();
          //txt.Text = "H";
          //txt.TextAlignment = TextAlignment.Center;
          //grdNew.Children.Add(txt);
          //Grid.SetRow(txt, 1);
          //Grid.SetColumn(txt, 13);

          int rowhere = 1;
          b_Ordine_complete.Clear();

          //PATRIMONIALE ATTIVO
          switch (tipoBilancio)
          {
            case "2016":
              file = App.AppTemplateBilancio_Attivo2016;
              break;
            default:
              file = App.AppTemplateBilancio_Attivo;
              break;
          }

          bool isdone1 = false;
          bool isdone2 = false;

          GenerateData(file, Codice, "PATRIMONIALE ATTIVO", true, true);

          isdone1 = GenerateGrid(item, grdNew, Codice, ref rowhere, "PATRIMONIALE ATTIVO");

          b_Ordine_complete.AddRange(b_Ordine);

          //PATRIMONIALE PASSIVO
          switch (tipoBilancio)
          {
            case "2016":
              file = App.AppTemplateBilancio_Passivo2016;
              break;
            default:
              file = App.AppTemplateBilancio_Passivo;
              break;
          }

          GenerateData(file, Codice, "PATRIMONIALE PASSIVO", true, true);

          isdone2 = GenerateGrid(item, grdNew, Codice, ref rowhere, "PATRIMONIALE PASSIVO");

          b_Ordine_complete.AddRange(b_Ordine);

          //CONTO ECONOMICO
          switch (tipoBilancio)
          {
            case "2016":
              file = App.AppTemplateBilancio_ContoEconomico2016;
              break;
            default:
              file = App.AppTemplateBilancio_ContoEconomico;
              break;
          }

          GenerateData(file, Codice, "CONTO ECONOMICO", true, true);

          GenerateGrid(item, grdNew, Codice, ref rowhere, "CONTO ECONOMICO");



          DataRow tmpnodehere = null;

          foreach (DataRow dd in datiN.Rows)
          {
            if (dd["ID"].ToString() == item.Key.ToString())
            {
              tmpnodehere = dd;
            }
          }
          if (tmpnodehere == null)
          {
            tmpnodehere = datiN.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            tmpnodehere["ID"] = item.Key.ToString();
            tmpnodehere["Voce"] = Codice;
            tmpnodehere["Chiuso"] = "True";
            tmpnodehere["Titolo"] = item.Value.ToString().Split('@')[1].Replace("&", "&amp;").Replace("\"", "'");

          }




          b_Ordine_complete.AddRange(b_Ordine);

          ArrayList tmpAL = new ArrayList();
          tmpAL.AddRange(b_Ordine_complete);

          b_Ordine_completeHT.Add(item.Key.ToString(), tmpAL);

          StackPanel stpNoteAll = new StackPanel();
          stpNoteAll.Margin = new Thickness(0, 20, 0, 0);
          stpNoteAll.Name = "stpNoteAll_" + item.Key.ToString();
          if (this.FindName(stpNoteAll.Name) != null)
          {
            this.UnregisterName(stpNoteAll.Name);
          }
          this.RegisterName(stpNoteAll.Name, stpNoteAll);

          rd = new RowDefinition();
          rd.Height = GridLength.Auto;
          grdNew.RowDefinitions.Add(rd);
          rowhere++;

          grdNew.Children.Add(stpNoteAll);
          Grid.SetRow(stpNoteAll, rowhere);
          Grid.SetColumn(stpNoteAll, 0);
          Grid.SetColumnSpan(stpNoteAll, 14);

          FillNote(item.Key.ToString());

          //if (!(latrownote.ContainsKey(item.Key.ToString())))
          //{
          //    latrownote.Add(item.Key.ToString(), 0);
          //}

          //latrownote[item.Key.ToString()] = rowhere;

          grd.Children.Add(grdNew);
          Grid.SetRow(grdNew, 6);
          Grid.SetColumn(grdNew, 0);
          Grid.SetColumnSpan(grdNew, 4);


          grd.SetValue(Grid.RowProperty, 1);
          grd.SetValue(Grid.ColumnProperty, 1);

          //if(tmpnode.Attributes["Chiuso"] != null && tmpnode.Attributes["Chiuso"].Value == "True")
          //{
          grd.Visibility = System.Windows.Visibility.Collapsed;
          //}

          g.Children.Add(grd);

          b.Child = g;

          brdDefinizione.Children.Add(b);

        }

        #endregion


        /*
        SEMBRA CHE IL TESTO FINALE SIA STATO MESSO NELLO XAML NASCOSTO QUINDI COMMENTO NB PER ORA NON e' PREVISTO NELLA TABELLA DB
        foreach (DataRow dd in dati.Rows)
        {
            if (dd["Testo"].ToString() != null)
            {
                txtConsiderazioni.Text = dd["Testo"].ToString();
            }
        }
        */

      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }

      canbeexecuted = true;
    }
    public void Load(string ID, string FileRevisione, Hashtable _Sessioni, Hashtable _SessioniTitoli, Hashtable _SessioniID, int _SessioneNow, string _IDTree, string _IDCliente, string _IDSessione)
    {
      ArrayList tmpAL;
      bool check345, hasdata, isdone1, isdone2, notatrovata, bVal;
      bool NotenumericoNumber, NotenumericoRealR;
      DataRow dtrischioglobale, tmpnode;
      DataTable datibilanciotestata, datimaterialita, datiRischioGlobale;
      DataTable statom, tmpNode_true;
      int i, n, NoteNumberInt, NoteRealRowInt, rowhere, selecteditem;
      MasterFile mf;
      string Codice, file, ID_Materialità_1, ID_Materialità_2, ID_Materialità_3;
      string idsessionebilancio, statomat, str, str2, tempNote, tempNoteNumber;
      string tempNoteRealRow;
      DataRow[] arrDataRows;
      int[] arrGridLens =
      {
        20, 1, 120, 50, 50, 70, 50, 30, 30, 30, 30, 30, 30, 80
      };

      id = int.Parse(ID);
      cBusinessObjects.idcliente = int.Parse(_IDCliente.ToString());
      cBusinessObjects.idsessione = int.Parse(_IDSessione.ToString());
      datiN = cBusinessObjects.GetData(id, typeof(PianificazioneNewWD_Node));
      datiV = cBusinessObjects.GetData(id, typeof(PianificazioneNewWD_Valore));

      //----------------------------- mette a posto bug delle colonne invertite
      NotenumericoRealR = true;
      NotenumericoNumber = true;
      notatrovata = false;
      foreach (DataRow dd in datiV.Rows)
      {
        if (dd["NoteRealRow"].ToString() != "")
        {
          notatrovata = true;
          NotenumericoRealR = int.TryParse(dd["NoteRealRow"].ToString(), out n);
          NotenumericoNumber = int.TryParse(dd["NoteNumber"].ToString(), out n);
        }
        if (!NotenumericoRealR || !NotenumericoNumber) break;
      }
      if (notatrovata && (!NotenumericoRealR || !NotenumericoNumber))
      {
        foreach (DataRow dd in datiV.Rows)
        {
          tempNoteRealRow = dd["NoteRealRow"].ToString();
          tempNoteNumber = dd["NoteNumber"].ToString();
          tempNote = dd["Note"].ToString();
          if (!NotenumericoRealR)
          {
            dd["Note"] = tempNoteRealRow;
            dd["NoteRealRow"] = tempNote;
          }
          if (!NotenumericoNumber)
          {
            dd["Note"] = tempNoteNumber;
            dd["NoteNumber"] = tempNote;
          }
          tempNoteRealRow = dd["NoteRealRow"].ToString();
          tempNoteNumber = dd["NoteNumber"].ToString();
          NoteNumberInt = 0;
          NoteRealRowInt = 0;
          int.TryParse(tempNoteRealRow, out NoteRealRowInt);
          int.TryParse(tempNoteNumber, out NoteNumberInt);
          if (NoteNumberInt > NoteRealRowInt)
          {
            dd["NoteRealRow"] = tempNoteNumber;
            dd["NoteNumber"] = tempNoteRealRow;
          }
        }
        datiV.AcceptChanges();
        cBusinessObjects.SaveData(id, datiV, typeof(PianificazioneNewWD_Valore));
      }
      datiVRighe = cBusinessObjects.GetData(
        id, typeof(PianificazioneNewWD_ValoreRighe));
      canbeexecuted = false;
      try
      {
        Sessioni = _Sessioni;
        SessioniTitoli = _SessioniTitoli;
        SessioniID = _SessioniID;
        SessioneNow = _SessioneNow;
        IDTree = _IDTree;
        IDCliente = _IDCliente;
        IDSessione = _IDSessione;
        _ID = ID;
        mf = MasterFile.Create();
        bilancioAssociato = mf.GetBilancioAssociatoFromRevisioneFile(
          Sessioni[SessioneNow].ToString());
        bilancioTreeAssociato = mf.GetBilancioTreeAssociatoFromRevisioneFile(
          Sessioni[SessioneNow].ToString());
        bilancioIDAssociato = mf.GetBilancioIDAssociatoFromRevisioneFile(
          Sessioni[SessioneNow].ToString());
        brdPrima.Visibility = System.Windows.Visibility.Collapsed;
        brdSeconda.Visibility = System.Windows.Visibility.Collapsed;
        brdTerza.Visibility = System.Windows.Visibility.Collapsed;
        if (!string.IsNullOrEmpty(bilancioTreeAssociato))
        {
          _xBTree = new XmlDataProviderManager(bilancioTreeAssociato);
        }
        idsessionebilancio = cBusinessObjects.CercaSessione(
          "Revisione", "Bilancio", IDSessione, cBusinessObjects.idcliente);
        datibilanciotestata = cBusinessObjects.GetData(
          227, typeof(Excel_Bilancio_Testata), cBusinessObjects.idcliente,
          int.Parse(idsessionebilancio), 4);
        if (datibilanciotestata.Rows.Count == 0)
        {
          MessageBox.Show("Bilancio Ordinario assente", "ERRORE", MessageBoxButton.OK,
            MessageBoxImage.Error);
          return;
        }
        tipoBilancio = "";
        //--------------------------------------------------------------------+
        //    A una revisione non può essere associato più di un bilancio     |
        //               ordinario, quindi basta la prima riga                |
        //--------------------------------------------------------------------+
        //foreach (DataRow dt in datibilanciotestata.Rows)
        //  tipoBilancio = dt["tipoBilancio"].ToString();
        tipoBilancio = datibilanciotestata.Rows[0]["tipoBilancio"].ToString().Trim();
        switch (tipoBilancio)
        {
          case "2016":
            _y = new XmlDataProviderManager(App.AppLEAD2016, true);
            break;
          default:
            _y = new XmlDataProviderManager(App.AppLEAD, true);
            break;
        }
        b_valoreEA.Clear();
        RetrieveData();
        if (b_valoreEA.Count == 0)
        {
          MessageBox.Show("Bilancio Ordinario non compilato", "ERRORE",
            MessageBoxButton.OK, MessageBoxImage.Error);
          return;
        }
        ID_Materialità_1 = "77"; //------------- 2.11.1 Materialità - Sintetica
        ID_Materialità_2 = "78"; //------------------------------ non esiste???
        ID_Materialità_3 = "199"; //---------- 2.11.3 Materialità - Dettagliata
        tmpNode_true = null;
        statomat = "";
        statom = null;

        //------------------ verifica presenza "2.11.1 Materialità - Sintetica"
        datimaterialita = cBusinessObjects.GetData(
          int.Parse(ID_Materialità_1), typeof(Excel_LimiteMaterialitaSPCE));
        if (datimaterialita.Rows.Count > 0)
        {
          statom = cBusinessObjects.GetData(int.Parse(ID_Materialità_1), typeof(StatoNodi));
          if (statom.Rows.Count > 0)
          {
            //------------------------------------- deve esistere una sola riga
            statomat = statom.Rows[0]["Stato"].ToString().Trim();
            if (!string.IsNullOrEmpty(statomat))
            {
              if ((App.TipoTreeNodeStato)Convert.ToInt32(statomat) == App.TipoTreeNodeStato.Completato)
              {
                arrDataRows = datimaterialita.Select("ID='rbtTipoMaterialitaBilancio1'");
                if (arrDataRows.Count() > 0)
                {
                  bVal = Convert.ToBoolean(arrDataRows[0]["value"].ToString());
                  Materialità_1 = true;
                  brdPrima.Visibility = bVal ? Visibility.Visible : Visibility.Collapsed;
                  brdSeconda.Visibility = bVal ? Visibility.Collapsed : Visibility.Visible;
                  //brdTerza.Visibility = Visibility.Collapsed;
                  tmpNode_true = datimaterialita;
                }
              }
            }
          }
        }

        //--------------- verifica presenza materialità nodo 78 - non esiste???
        if (!Materialità_1)
        {
          datimaterialita = cBusinessObjects.GetData(
            int.Parse(ID_Materialità_2), typeof(Excel_LimiteMaterialitaSPCE));
          statom = cBusinessObjects.GetData(
            int.Parse(ID_Materialità_2), typeof(StatoNodi));
          foreach (DataRow dd in statom.Rows)
            statomat = dd["Stato"].ToString().Trim();
          if (datimaterialita.Rows.Count > 0
            && ((App.TipoTreeNodeStato)(Convert.ToInt32(statomat)))
              == App.TipoTreeNodeStato.Completato)
          {
            Materialità_2 = true;
            brdPrima.Visibility = System.Windows.Visibility.Collapsed;
            brdSeconda.Visibility = System.Windows.Visibility.Visible;
            brdTerza.Visibility = System.Windows.Visibility.Collapsed;
            tmpNode_true = datimaterialita;
          }
        }

        //---------------- verifica presenza "2.11.3 Materialità - Dettagliata"
        if (!Materialità_1 && !Materialità_2)
        {
          datimaterialita = cBusinessObjects.GetData(
            int.Parse(ID_Materialità_3), typeof(Excel_LimiteMaterialitaSPCE));
          if (datimaterialita.Rows.Count > 0)
          {
            statom = cBusinessObjects.GetData(int.Parse(ID_Materialità_3), typeof(StatoNodi));
            if (statom.Rows.Count > 0)
            {
              //----------------------------------- deve esistere una sola riga
              statomat = statom.Rows[0]["Stato"].ToString().Trim();
              if (!string.IsNullOrEmpty(statomat))
              {
                if ((App.TipoTreeNodeStato)Convert.ToInt32(statomat)
                  == App.TipoTreeNodeStato.Completato)
                {
                  arrDataRows =
                    datimaterialita.Select("ID='rbtTipoMaterialitaBilancio1'");
                  if (arrDataRows.Count() > 0)
                  {
                    bVal = Convert.ToBoolean(arrDataRows[0]["value"].ToString());
                    Materialità_3 = true;
                    brdPrima.Visibility = bVal ? Visibility.Visible : Visibility.Collapsed;
                    //brdSeconda.Visibility = Visibility.Collapsed;
                    brdTerza.Visibility = bVal ? Visibility.Collapsed : Visibility.Visible;
                    tmpNode_true = datimaterialita;
                  }
                }
              }
            }
          }
        }

        if (tmpNode_true != null)
        {
          foreach (DataRow dtrow in tmpNode_true.Rows)
          {
            str = dtrow["ID"].ToString(); str2 = dtrow["value"].ToString();
            if (str == "txt7BILANCIO") txt7.Text = str2;
            if (str == "txt7_2spBILANCIO") txt7_2sp.Text = str2;
            if (str == "txt7_2ceBILANCIO") txt7_2ce.Text = str2;
            if (str == "txt7_3spBILANCIO") txt7_3sp.Text = str2;
            if (str == "txt7_3ceBILANCIO") txt7_3ce.Text = str2;
            if (str == "txt9BILANCIO") txt9.Text = str2;
            if (str == "txt9_2spBILANCIO") txt9_2sp.Text = str2;
            if (str == "txt9_2ceBILANCIO") txt9_2ce.Text = str2;
            if (str == "txt9_3spBILANCIO") txt9_3sp.Text = str2;
            if (str == "txt9_3ceBILANCIO") txt9_3ce.Text = str2;
            if (str == "txt12BILANCIO") txt12.Text = str2;
            if (str == "txt12_2spBILANCIO") txt12_2sp.Text = str2;
            if (str == "txt12_2ceBILANCIO") txt12_2ce.Text = str2;
            if (str == "txt12_3spBILANCIO") txt12_3sp.Text = str2;
            if (str == "txt12_3ceBILANCIO") txt12_3ce.Text = str2;
          }
        }
        #region PRIMO BLOCCO
        datiRischioGlobale =
          cBusinessObjects.GetData(int.Parse(IDRischioGlobale),
            typeof(RischioGlobale));
        dtrischioglobale = null;
        foreach (DataRow node in datiRischioGlobale.Rows)
        {
          txt1.Text = node["txt1"].ToString().ToUpper();
          txt3.Text = node["txt3"].ToString().ToUpper();
          txt3c.Text = node["txt3c"].ToString().ToUpper();
          txt4.Text = node["txt4"].ToString().ToUpper();
          txt4c.Text = node["txt4c"].ToString().ToUpper();
          txt6.Text = node["txt6"].ToString().ToUpper();
          txt6c.Text = node["txt6c"].ToString().ToUpper();
          txt5.Text = node["txt5"].ToString().ToUpper();
          txt5c.Text = node["txt5c"].ToString().ToUpper();
          txt2.Text = node["txt2"].ToString().ToUpper();
          txt2c.Text = node["txt2c"].ToString().ToUpper();
          dtrischioglobale = node;
        }
        #endregion
        #region SECONDO BLOCCO
        Border brtst = new Border();
        brtst.BorderThickness = new Thickness(1);
        brtst.BorderBrush = Brushes.Black;
        brtst.Padding = new Thickness(10, 10, 10, 0);
        brtst.Margin = new Thickness(0, 0, 0, 5);
        brtst.HorizontalAlignment = HorizontalAlignment.Center;

        StackPanel stk3 = new StackPanel();
        stk3.Orientation = Orientation.Vertical;
        stk3.HorizontalAlignment = HorizontalAlignment.Center;

        StackPanel stk = new StackPanel();
        stk.Orientation = Orientation.Horizontal;
        stk.HorizontalAlignment = HorizontalAlignment.Center;

        TextBlock txtblk = new TextBlock();
        txtblk.Text = "Legenda: VEDI SUGGERIMENTI";
        txtblk.FontWeight = FontWeights.Bold;
        txtblk.Margin = new Thickness(10, 0, 0, 10);
        txtblk.TextAlignment = TextAlignment.Center;
        stk.Children.Add(txtblk);
        stk3.Children.Add(stk);

        stk = new StackPanel();
        stk.Orientation = Orientation.Horizontal;
        stk.HorizontalAlignment = HorizontalAlignment.Center;

        txtblk = new TextBlock();
        txtblk.Text = "ET";
        txtblk.FontWeight = FontWeights.Bold;
        txtblk.Margin = new Thickness(10, 0, 0, 5);
        stk.Children.Add(txtblk);

        txtblk = new TextBlock();
        txtblk.Text = "= Errore Trascurabile";
        txtblk.Margin = new Thickness(5, 0, 0, 5);
        stk.Children.Add(txtblk);

        txtblk = new TextBlock();
        txtblk.Text = "MO";
        txtblk.FontWeight = FontWeights.Bold;
        txtblk.Margin = new Thickness(10, 0, 0, 5);
        stk.Children.Add(txtblk);

        txtblk = new TextBlock();
        txtblk.Text = "= Materialità Operativa";
        txtblk.Margin = new Thickness(5, 0, 0, 5);
        stk.Children.Add(txtblk);

        stk3.Children.Add(stk);
        brtst.Child = stk3;
        brdDefinizione.Children.Add(brtst);
        b_Ordine_completeHT.Clear();
        file = string.Empty;
        foreach (KeyValuePair<int, string> item in VociBilancio2)
        {
          hasdata = false;
          Codice = item.Value.ToString().Split('@')[0];

          //-------------------------------------------- ??? cosa significa ???
          if (item.Key.ToString() == "91")
          {
            //int a = 1;
          }

          //------------------------------------------------------------------+
          //                       PATRIMONIALE ATTIVO                        |
          //------------------------------------------------------------------+
          switch (tipoBilancio)
          {
            case "2016":
              file = App.AppTemplateBilancio_Attivo2016; break;
            default:
              file = App.AppTemplateBilancio_Attivo; break;
          }
          GenerateData(file, Codice, "PATRIMONIALE ATTIVO", true, false);
          if (CheckExistence(Codice))
          {
            hasdata = true;
            //stpMain.Children.Add(GenerateGrid("PATRIMONIALE ATTIVO"));
          }
          check345 = hasdata;
          //------------------------------------------------------------------+
          //                       PATRIMONIALE PASSIVO                       |
          //------------------------------------------------------------------+
          switch (tipoBilancio)
          {
            case "2016":
              file = App.AppTemplateBilancio_Passivo2016; break;
            default:
              file = App.AppTemplateBilancio_Passivo; break;
          }
          GenerateData(file, Codice, "PATRIMONIALE PASSIVO", true, false);
          if (CheckExistence(Codice))
          {
            hasdata = true;
            //stpMain.Children.Add(GenerateGrid("PATRIMONIALE PASSIVO"));
          }
          //------------------------------------------------------------------+
          //                         CONTO ECONOMICO                          |
          //------------------------------------------------------------------+
          switch (tipoBilancio)
          {
            case "2016":
              file = App.AppTemplateBilancio_ContoEconomico2016; break;
            default:
              file = App.AppTemplateBilancio_ContoEconomico; break;
          }
          GenerateData(file, Codice, "CONTO ECONOMICO", true, false);
          if (Codice == "3.4.5" && check345 == false)
          {
            ;
          }
          else
          {
            if (CheckExistence(Codice))
            {
              hasdata = true;
              //stpMain.Children.Add(GenerateGrid("PATRIMONIALE PASSIVO"));
            }
          }
          if (hasdata == false)
          {
            continue;
          }
          tmpnode = null;
          foreach (DataRow dd in datiN.Rows)
          {
            if (dd["ID"].ToString() == item.Key.ToString())
            {
              tmpnode = dd;
            }
          }
          if (tmpnode == null)
          {
            tmpnode = datiN.Rows.Add(
              id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            tmpnode["ID"] = item.Key.ToString();
            tmpnode["Voce"] = Codice;
            tmpnode["Chiuso"] = "True";
            tmpnode["Titolo"] = item.Value.ToString()
              .Split('@')[1].Replace("&", "&amp;").Replace("\"", "'");
          }
          Border b = new Border();
          b.CornerRadius = new CornerRadius(5.0);
          b.BorderBrush = Brushes.Blue;
          b.BorderThickness = new Thickness(2.0);
          b.Padding = new Thickness(4.0);
          b.Margin = new Thickness(4.0);
          Grid g = new Grid();
          g.Name = "Grid_" + item.Key.ToString();
          ColumnDefinition cd = new ColumnDefinition();
          cd.Width = new GridLength(15.0);
          g.ColumnDefinitions.Add(cd);
          cd = new ColumnDefinition();
          cd.Width = GridLength.Auto;
          g.ColumnDefinitions.Add(cd);
          g.RowDefinitions.Add(new RowDefinition());
          g.RowDefinitions.Add(new RowDefinition());
          Image img = new Image();
          img.SetValue(Grid.RowProperty, 0);
          img.SetValue(Grid.ColumnProperty, 0);
          var uriSource = new Uri(left, UriKind.Relative);
          img.Source = new BitmapImage(uriSource);
          img.Height = 10.0;
          img.Width = 10.0;
          img.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);
          g.Children.Add(img);
          StackPanel sp3 = new StackPanel();
          sp3.Orientation = Orientation.Horizontal;
          TextBlock txt = new TextBlock();
          txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          txt.Text = tmpnode["Voce"].ToString() + " - " + tmpnode["Titolo"].ToString();
          txt.Tag = tmpnode["ID"].ToString();
          txt.FontSize = 13;
          txt.FontWeight = FontWeights.Bold;
          txt.Margin = new Thickness(5.0);
          txt.Foreground = Brushes.Gray;
          sp3.Children.Add(txt);
          sp3.SetValue(Grid.RowProperty, 0);
          sp3.SetValue(Grid.ColumnProperty, 1);
          g.Children.Add(sp3);
          Grid grd = new Grid();
          cd = new ColumnDefinition();
          cd.Width = new GridLength(1, GridUnitType.Star);
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
          rd.Height = GridLength.Auto;
          grd.RowDefinitions.Add(rd);
          txt = new TextBlock();
          txt.Text = "Esecutore / Personale assegnato";
          grd.Children.Add(txt);
          Grid.SetRow(txt, 0);
          Grid.SetColumn(txt, 0);
          Grid.SetColumnSpan(txt, 4);
          rd = new RowDefinition();
          rd.Height = GridLength.Auto;
          grd.RowDefinitions.Add(rd);
          TextBox tb = new TextBox();
          tb.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
          tb.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          tb.LostFocus += new RoutedEventHandler(tbEsecutore_LostFocus);
          tb.TextWrapping = TextWrapping.Wrap;
          tb.AcceptsReturn = true;
          tb.HorizontalAlignment = HorizontalAlignment.Left;
          tb.Margin = new Thickness(0);
          tb.Name = "_" + item.Key.ToString() + "_Esecutore";
          if (tmpnode != null)
          {
            str = tmpnode["Esecutore"].ToString();
            if (!string.IsNullOrEmpty(str)) tb.Text = str;
          }
          this.RegisterName(tb.Name, tb);
          grd.Children.Add(tb);
          Grid.SetRow(tb, 1);
          Grid.SetColumn(tb, 0);
          Grid.SetColumnSpan(tb, 4);
          rd = new RowDefinition();
          rd.Height = GridLength.Auto;
          grd.RowDefinitions.Add(rd);
          txt = new TextBlock();
          txt.Text = "Note";
          grd.Children.Add(txt);
          Grid.SetRow(txt, 2);
          Grid.SetColumn(txt, 0);
          Grid.SetColumnSpan(txt, 4);
          rd = new RowDefinition();
          rd.Height = GridLength.Auto;
          grd.RowDefinitions.Add(rd);
          tb = new TextBox();
          tb.Name = "_" + item.Key.ToString() + "_Nota";
          tb.Text = string.Empty;
          if (tmpnode != null)
          {
            str = tmpnode["Nota"].ToString();
            if (!string.IsNullOrEmpty(str)) tb.Text = str;
          }
          tb.PreviewMouseLeftButtonDown +=
            new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
          tb.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
          tb.LostFocus += new RoutedEventHandler(tbNota_LostFocus);
          tb.TextWrapping = TextWrapping.Wrap;
          tb.AcceptsReturn = true;
          tb.Foreground = Brushes.Blue;
          this.RegisterName(tb.Name, tb);
          grd.Children.Add(tb);
          Grid.SetRow(tb, 3);
          Grid.SetColumn(tb, 0);
          Grid.SetColumnSpan(tb, 4);
          rd = new RowDefinition();
          rd.Height = GridLength.Auto;
          grd.RowDefinitions.Add(rd);
          rd = new RowDefinition();
          rd.Height = GridLength.Auto;
          grd.RowDefinitions.Add(rd);
          Border brd2 = new Border();
          brd2.BorderBrush = Brushes.Gray;
          brd2.BorderThickness = new Thickness(1.0);
          brd2.HorizontalAlignment = HorizontalAlignment.Center;
          brd2.Margin = new Thickness(10.0);
          brd2.Padding = new Thickness(10.0);
          StackPanel stpbrdV = new StackPanel();
          stpbrdV.Orientation = Orientation.Vertical;
          txt = new TextBlock();
          txt.Text = "Rischio di individuazione";
          txt.FontWeight = FontWeights.Bold;
          txt.Margin = new Thickness(0, 0, 0, 5);
          stpbrdV.Children.Add(txt);
          StackPanel stpbrd = new StackPanel();
          stpbrd.Orientation = Orientation.Horizontal;
          txt = new TextBlock();
          txt.Text = "Proposto";
          txt.Margin = new Thickness(0, 0, 10, 0);
          stpbrd.Children.Add(txt);
          txt = new TextBlock();
          txt.Margin = new Thickness(10, 0, 10, 0);
          txt.Width = 150;
          txt.FontWeight = FontWeights.Bold;
          txt.Margin = new Thickness(0.0, 0.0, 10.0, 0.0);
          if (item.Value.ToString().Split('@')[10] == "pv")
          {
            txt.Text = "Procedure Validità";
          }
          else
          {
            txt.Text = "Procedure Validità";
            if (dtrischioglobale != null)
            {
              try
              {
                switch (
                  ((dtrischioglobale[item.Value.ToString()
                    .Split('@')[10]].ToString() == "") ?
                      "6" : dtrischioglobale[item.Value.ToString().Split('@')[10]].ToString()))
                {
                  case "Molto Alto": txt.Text = "Molto Alto"; break;
                  case "Alto": txt.Text = "Alto"; break;
                  case "Medio": txt.Text = "Medio"; break;
                  case "Basso": txt.Text = "Basso"; break;
                  case "Molto Basso": txt.Text = "Molto Basso"; break;
                  default: txt.Text = "Procedure Validità"; break;
                }
              }
              catch (Exception) { }
            }
          }
          if (tmpnode != null && tmpnode["cmbRI_Proposto"].ToString() != "")
          {
            tmpnode["cmbRI_Proposto"] = txt.Text;
          }
          stpbrd.Children.Add(txt);
          txt = new TextBlock();
          txt.Text = "Scelto";
          stpbrd.Children.Add(txt);
          ComboBox newCombo = new ComboBox();
          newCombo.Name = "_" + item.Key.ToString() + "_ComboBoxRI_"
            + item.Value.ToString().Split('@')[10];
          this.RegisterName(newCombo.Name, newCombo);
          if (alreadydonefirstbutton == false)
          {
            alreadydonefirstbutton = true;
            newCombo.SelectionChanged +=
              new SelectionChangedEventHandler(changeAll);
          }
          newCombo.SelectionChanged +=
            new SelectionChangedEventHandler(cmbRI_Changed);
          newCombo.PreviewMouseLeftButtonDown +=
            new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
          newCombo.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          newCombo.Margin = new Thickness(10, 0, 0, 0);

          ComboBoxItem newitem = new ComboBoxItem();
          newitem.HorizontalContentAlignment = HorizontalAlignment.Center;
          newitem.Content = "MA  - Molto Alto"; newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "A    - Alto"; newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "M    - Medio"; newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "B    - Basso"; newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "MB  - Molto Basso"; newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "PV  - Proced Validità"; newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "NA  - Non Applicabile"; newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "*    - Ripristina R.I. Automatico";
          newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "?"; newCombo.Items.Add(newitem);
          if (tmpnode != null && tmpnode["cmbRI"].ToString() != ""
            && tmpnode["cmbRI"].ToString() != "")
          {
            selecteditem = 0;
            switch (tmpnode["cmbRI"].ToString())
            {
              case "MA": selecteditem = 0; break;
              case "A": selecteditem = 1; break;
              case "M": selecteditem = 2; break;
              case "B": selecteditem = 3; break;
              case "MB": selecteditem = 4; break;
              case "PV": selecteditem = 5; break;
              case "NA": selecteditem = 6; break;
              default: selecteditem = 8; break;
            }
            newCombo.SelectedItem = ((ComboBoxItem)newCombo.Items[selecteditem]);
            newCombo.Text =
              ((ComboBoxItem)newCombo.Items[selecteditem]).Content.ToString();
          }
          else
          {
            if (item.Value.ToString().Split('@')[10] == "pv")
            {
              newCombo.SelectedItem = ((ComboBoxItem)newCombo.Items[5]);
              newCombo.Text =
                ((ComboBoxItem)newCombo.Items[5]).Content.ToString();
            }
            else
            {
              selecteditem = 0;
              if (dtrischioglobale != null)
              {
                try
                {
                  switch ((
                    (dtrischioglobale[item.Value.ToString().Split('@')[10]] == null) ?
                      "6" :
                      dtrischioglobale[item.Value.ToString().Split('@')[10]].ToString()))
                  {
                    case "Molto Alto": selecteditem = 0; break;
                    case "Alto": selecteditem = 1; break;
                    case "Medio": selecteditem = 2; break;
                    case "Basso": selecteditem = 3; break;
                    case "Molto Basso": selecteditem = 4; break;
                    default: selecteditem = 8; break;
                  }
                }
                catch (Exception)
                {
                }
              }
              newCombo.SelectedItem = ((ComboBoxItem)newCombo.Items[selecteditem]);
              newCombo.Text =
                ((ComboBoxItem)newCombo.Items[selecteditem]).Content.ToString();
            }
          }
          newCombo.Width = 200;
          stpbrd.Children.Add(newCombo);
          stpbrdV.Children.Add(stpbrd);
          brd2.Child = stpbrdV;
          grd.Children.Add(brd2);
          Grid.SetRow(brd2, 4);
          Grid.SetRowSpan(brd2, 2);
          Grid.SetColumn(brd2, 0);
          Grid.SetColumnSpan(brd2, 3);
          rd = new RowDefinition();
          grd.RowDefinitions.Add(rd);
          Grid grdNew = new Grid();
          grdNew.Margin = new Thickness(0.0, 10.0, 0.0, 0.0);

          for (i = 0; i < arrGridLens.Count(); i++)
          {
            cd = new ColumnDefinition();
            cd.Width = new GridLength(arrGridLens[i],
              arrGridLens[i] == 1 ? GridUnitType.Star : GridUnitType.Pixel);
            grdNew.ColumnDefinitions.Add(cd);
          }

          rd = new RowDefinition();
          rd.Height = GridLength.Auto;
          grdNew.RowDefinitions.Add(rd);
          rd = new RowDefinition();
          rd.Height = GridLength.Auto;
          grdNew.RowDefinitions.Add(rd);
          Border brdfc = new Border();
          brdfc.BorderBrush = Brushes.DarkGray;
          brdfc.BorderThickness = new Thickness(1, 1, 0, 0);

          txt = new TextBlock();
          txt.TextAlignment = TextAlignment.Center;
          txt.Text = "VOCI DI BILANCIO";
          txt.FontWeight = FontWeights.Bold;
          brdfc.Child = txt;
          grdNew.Children.Add(brdfc);
          Grid.SetRow(brdfc, 0);
          Grid.SetRowSpan(brdfc, 1);
          Grid.SetColumn(brdfc, 0);
          Grid.SetColumnSpan(brdfc, 2);
          brdfc = new Border();
          brdfc.BorderBrush = Brushes.DarkGray;
          brdfc.BorderThickness = new Thickness(0, 1, 1, 0);

          txt = new TextBlock();
          txt.TextAlignment = TextAlignment.Center;
          txt.Text = "VALORE";
          txt.FontWeight = FontWeights.Bold;
          brdfc.Child = txt;
          grdNew.Children.Add(brdfc);
          Grid.SetRow(brdfc, 0);
          Grid.SetRowSpan(brdfc, 1);
          Grid.SetColumn(brdfc, 2);
          brdfc = new Border();
          brdfc.BorderBrush = Brushes.DarkGray;
          brdfc.BorderThickness = new Thickness(1, 1, 0, 0);
          brdfc.Margin = new Thickness(10, 0, 0, 0);

          txt = new TextBlock();
          txt.TextAlignment = TextAlignment.Center;
          txt.Text = "INFERIORE A";
          txt.FontWeight = FontWeights.Bold;
          brdfc.Child = txt;
          grdNew.Children.Add(brdfc);
          Grid.SetRow(brdfc, 0);
          Grid.SetColumn(brdfc, 3);
          Grid.SetColumnSpan(brdfc, 2);
          brdfc = new Border();
          brdfc.BorderBrush = Brushes.DarkGray;
          brdfc.BorderThickness = new Thickness(0, 1, 1, 0);
          brdfc.Padding = new Thickness(0, 0, 3, 0);

          txt = new TextBlock();
          txt.TextAlignment = TextAlignment.Center;
          txt.FontSize = 0.9 * txt.FontSize;
          txt.Text = "CONTROLLO";
          txt.FontWeight = FontWeights.Bold;
          brdfc.Child = txt;
          grdNew.Children.Add(brdfc);
          Grid.SetRow(brdfc, 0);
          Grid.SetRowSpan(brdfc, 1);
          Grid.SetColumn(brdfc, 5);
          brdfc = new Border();
          brdfc.BorderBrush = Brushes.DarkGray;
          brdfc.BorderThickness = new Thickness(1, 1, 1, 0);
          brdfc.Margin = new Thickness(10, 0, 0, 0);

          txt = new TextBlock();
          txt.TextAlignment = TextAlignment.Center;
          txt.Text = "EVIDENZE";
          txt.FontWeight = FontWeights.Bold;
          brdfc.Child = txt;
          grdNew.Children.Add(brdfc);
          Grid.SetRow(brdfc, 0);
          Grid.SetColumn(brdfc, 6);
          Grid.SetColumnSpan(brdfc, 7);//8

          txt = new TextBlock();
          txt.TextAlignment = TextAlignment.Center;
          txt.Text = "NOTE";
          txt.FontWeight = FontWeights.Bold;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 0);
          Grid.SetRowSpan(txt, 1);
          Grid.SetColumn(txt, 13);//14

          txt = new TextBlock();
          txt.Text = "ET";
          txt.ToolTip = "ERRORE TRASCURABILE";
          txt.TextAlignment = TextAlignment.Center;
          txt.FontWeight = FontWeights.Bold;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 3);

          txt = new TextBlock();
          txt.Text = "MO";
          txt.ToolTip = "MATERIALITA' OPERATIVA";
          txt.TextAlignment = TextAlignment.Center;
          txt.FontWeight = FontWeights.Bold;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 4);

          txt = new TextBlock();
          txt.Text = "A";
          txt.ToolTip = "ISPEZIONE";
          txt.TextAlignment = TextAlignment.Center;
          txt.Margin = new Thickness(20, 0, 0, 0);
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 6);

          txt = new TextBlock();
          txt.Text = "B";
          txt.ToolTip = "OSSERVAZIONE";
          txt.TextAlignment = TextAlignment.Center;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 7);

          txt = new TextBlock();
          txt.Text = "C";
          txt.ToolTip = "CONFERMA ESTERNA /DOCUMENTAZIONE";
          txt.TextAlignment = TextAlignment.Center;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 8);

          txt = new TextBlock();
          txt.Text = "D";
          txt.ToolTip = "RICALCOLO";
          txt.TextAlignment = TextAlignment.Center;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 9);

          txt = new TextBlock();
          txt.Text = "E";
          txt.ToolTip = "RIESECUZIONE";
          txt.TextAlignment = TextAlignment.Center;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 10);

          txt = new TextBlock();
          txt.Text = "F";
          txt.ToolTip = "PROCEDURE DI ANALISI COMPARATIVA";
          txt.TextAlignment = TextAlignment.Center;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 11);

          txt = new TextBlock();
          txt.Text = "G";
          txt.ToolTip = "INDAGINE";
          txt.TextAlignment = TextAlignment.Center;
          grdNew.Children.Add(txt);
          Grid.SetRow(txt, 1);
          Grid.SetColumn(txt, 12);

          rowhere = 1;
          b_Ordine_complete.Clear();

          //------------------------------------------------------------------+
          //                       PATRIMONIALE ATTIVO                        |
          //------------------------------------------------------------------+
          switch (tipoBilancio)
          {
            case "2016":
              file = App.AppTemplateBilancio_Attivo2016; break;
            default:
              file = App.AppTemplateBilancio_Attivo; break;
          }
          isdone1 = false;
          isdone2 = false;
          GenerateData(file, Codice, "PATRIMONIALE ATTIVO", true, true);
          isdone1 = GenerateGrid(
            item, grdNew, Codice, ref rowhere, "PATRIMONIALE ATTIVO");
          b_Ordine_complete.AddRange(b_Ordine);

          //------------------------------------------------------------------+
          //                       PATRIMONIALE PASSIVO                       |
          //------------------------------------------------------------------+
          switch (tipoBilancio)
          {
            case "2016":
              file = App.AppTemplateBilancio_Passivo2016; break;
            default:
              file = App.AppTemplateBilancio_Passivo; break;
          }
          GenerateData(file, Codice, "PATRIMONIALE PASSIVO", true, true);
          isdone2 = GenerateGrid(
            item, grdNew, Codice, ref rowhere, "PATRIMONIALE PASSIVO");
          b_Ordine_complete.AddRange(b_Ordine);

          //------------------------------------------------------------------+
          //                         CONTO ECONOMICO                          |
          //------------------------------------------------------------------+
          switch (tipoBilancio)
          {
            case "2016":
              file = App.AppTemplateBilancio_ContoEconomico2016; break;
            default:
              file = App.AppTemplateBilancio_ContoEconomico; break;
          }
          GenerateData(file, Codice, "CONTO ECONOMICO", true, true);
          GenerateGrid(item, grdNew, Codice, ref rowhere, "CONTO ECONOMICO");
          DataRow tmpnodehere = null;
          foreach (DataRow dd in datiN.Rows)
          {
            if (dd["ID"].ToString() == item.Key.ToString())
              tmpnodehere = dd;
          }
          if (tmpnodehere == null)
          {
            tmpnodehere = datiN.Rows.Add(
              id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            tmpnodehere["ID"] = item.Key.ToString();
            tmpnodehere["Voce"] = Codice;
            tmpnodehere["Chiuso"] = "True";
            tmpnodehere["Titolo"] = item.Value.ToString()
              .Split('@')[1].Replace("&", "&amp;").Replace("\"", "'");
          }
          b_Ordine_complete.AddRange(b_Ordine);
          tmpAL = new ArrayList();
          tmpAL.AddRange(b_Ordine_complete);
          b_Ordine_completeHT.Add(item.Key.ToString(), tmpAL);
          StackPanel stpNoteAll = new StackPanel();
          stpNoteAll.Margin = new Thickness(0, 20, 0, 0);
          stpNoteAll.Name = "stpNoteAll_" + item.Key.ToString();
          if (this.FindName(stpNoteAll.Name) != null)
            this.UnregisterName(stpNoteAll.Name);
          this.RegisterName(stpNoteAll.Name, stpNoteAll);
          rd = new RowDefinition();
          rd.Height = GridLength.Auto;
          grdNew.RowDefinitions.Add(rd);
          rowhere++;
          grdNew.Children.Add(stpNoteAll);
          Grid.SetRow(stpNoteAll, rowhere);
          Grid.SetColumn(stpNoteAll, 0);
          Grid.SetColumnSpan(stpNoteAll, 14);
          FillNote(item.Key.ToString());
          grd.Children.Add(grdNew);
          Grid.SetRow(grdNew, 6);
          Grid.SetColumn(grdNew, 0);
          Grid.SetColumnSpan(grdNew, 4);
          grd.SetValue(Grid.RowProperty, 1);
          grd.SetValue(Grid.ColumnProperty, 1);
          grd.Visibility = System.Windows.Visibility.Collapsed;
          g.Children.Add(grd);
          b.Child = g;
          brdDefinizione.Children.Add(b);
        }
        #endregion
        //--------------------------------------------------------------------+
        //   SEMBRA CHE IL TESTO FINALE SIA STATO MESSO NELLO XAML NASCOSTO   |
        //     QUINDI COMMENTO NB PER ORA NON È PREVISTO NELLA TABELLA DB     |
        //--------------------------------------------------------------------+
        /*---------------------------------------------------------------------
                foreach (DataRow dd in dati.Rows)
                {
                    if (dd["Testo"].ToString() != null)
                    {
                        txtConsiderazioni.Text = dd["Testo"].ToString();
                    }
                }
        ---------------------------------------------------------------------*/
      }
      catch (Exception) { }
      canbeexecuted = true;
    }


    private void changeAll(object sender, SelectionChangedEventArgs e)
    {
      if (canbeexecuted == true)
      {
        if (MessageBox.Show("Si vuole attribuire questa scelta a tutti gli altri rischi di individuazione?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
        {
          return;
        }

        int selectedindex = ((ComboBox)sender).SelectedIndex;

        foreach (KeyValuePair<int, string> item in VociBilancio2)
        {
          string namehere = "_" + item.Key.ToString() + "_ComboBoxRI_" + item.Value.ToString().Split('@')[10];
          ComboBox newCombo = (ComboBox)this.FindName(namehere);

          if (newCombo != null)
          {
            newCombo.SelectedIndex = selectedindex;
            cmbRI_Changed(newCombo, e);
          }
        }
      }
    }

    private void FillNote(string itemkey)
    {
      if (this.FindName("stpNoteAll_" + itemkey) != null)
      {
        StackPanel stphere = ((StackPanel)(this.FindName("stpNoteAll_" + itemkey)));
        stphere.Orientation = Orientation.Vertical;
        stphere.Children.Clear();

        foreach (string itemdata in (ArrayList)(b_Ordine_completeHT[itemkey]))
        {
          DataRow tmpNodenew = null;
          foreach (DataRow dd in datiV.Rows)
          {
            if (dd["ID"].ToString() == itemdata && dd["Codice"].ToString() == VociBilancio[itemkey].ToString())
              tmpNodenew = dd;
          }


          if (tmpNodenew != null && tmpNodenew["NoteNumber"].ToString() != "" && tmpNodenew["NoteRealRow"].ToString() != "")
          {

            if (stphere.Children.Count == 0)
            {
              TextBlock tbl = new TextBlock();
              tbl.Text = "Note:";
              tbl.FontWeight = FontWeights.Bold;

              stphere.Children.Add(tbl);
            }

            StackPanel stp = new StackPanel();
            stp.Margin = new Thickness(5);
            stp.Name = "stpNote_" + itemdata + "_" + tmpNodenew["NoteRealRow"].ToString() + "_" + itemkey;
            stp.Orientation = Orientation.Horizontal;

            TextBlock txtnote = new TextBlock();
            txtnote.Text = tmpNodenew["NoteNumber"].ToString() + ":";
            txtnote.Name = "txtblkNote_" + itemdata + "_" + tmpNodenew["NoteRealRow"].ToString() + "_" + itemkey;
            txtnote.FontWeight = FontWeights.Bold;
            if (this.FindName(txtnote.Name) != null)
            {
              this.UnregisterName(txtnote.Name);
            }
            this.RegisterName(txtnote.Name, txtnote);

            stp.Children.Add(txtnote);

            TextBox txthere = new TextBox();
            txthere.Name = "txtNote_" + itemdata + "_" + tmpNodenew["NoteRealRow"].ToString() + "_" + itemkey;
            txthere.Width = 850;
            txthere.Foreground = Brushes.Blue;
            txthere.TextWrapping = TextWrapping.Wrap;
            txthere.AcceptsReturn = true;
            if (tmpNodenew != null && tmpNodenew["Note"].ToString() != "")
            {
              txthere.Text = tmpNodenew["Note"].ToString();
            }
            txthere.TextAlignment = TextAlignment.Left;
            txthere.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            txthere.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
            txthere.KeyUp += new KeyEventHandler(txtNota_LostFocus);

            if (this.FindName(txthere.Name) != null)
            {
              this.UnregisterName(txthere.Name);
            }
            this.RegisterName(txthere.Name, txthere);

            stp.Children.Add(txthere);

            stphere.Children.Add(stp);
          }
        }
      }
    }

    FrameworkElement CloneFrameworkElement(FrameworkElement originalElement)
    {
      string elementString = XamlWriter.Save(originalElement);

      StringReader stringReader = new StringReader(elementString);
      XmlReader xmlReader = XmlReader.Create(stringReader);
      FrameworkElement clonedElement = (FrameworkElement)XamlReader.Load(xmlReader);

      return clonedElement;
    }

    int lastint = 0;
    int genericdescription = 0;

    private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      Image i = ((Image)sender);

      if (i.Name.Contains("_Btn"))
      {
        string[] splitted = i.Name.Split('_');
        DataRow tmpNode = null;

        foreach (DataRow dd in datiV.Rows)
        {
          if (dd["ID"].ToString() == splitted[2])
            tmpNode = dd;
        }

        var uriSource = new Uri(left, UriKind.Relative);

        if (tmpNode["Chiuso"].ToString() != null && tmpNode["Chiuso"].ToString() == "True")
        {
          tmpNode["Chiuso"] = "False";

          for (int j = Convert.ToInt32(splitted[3]); j < Convert.ToInt32(splitted[4]); j++)
          {
            ((Grid)(i.Parent)).RowDefinitions[j].Height = GridLength.Auto;
          }

          uriSource = new Uri(up, UriKind.Relative);
        }
        else
        {
          tmpNode["Chiuso"] = "True";

          uriSource = new Uri(left, UriKind.Relative);

          for (int j = Convert.ToInt32(splitted[3]); j < Convert.ToInt32(splitted[4]); j++)
          {
            ((Grid)(i.Parent)).RowDefinitions[j].Height = new GridLength(0, GridUnitType.Pixel);
          }
        }

        i.Source = new BitmapImage(uriSource);

        return;
      }
      else if (i.Name.Contains("_RemoveNota"))
      {
        if (_ReadOnly)
        {
          MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
          return;
        }

        if (MessageBox.Show("Cancellazione", "Sicuri di voler cancellare la nota associata?", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
        {
          string[] splitted = i.Name.Split('_');
          DataRow tmpNodenew = null;
          foreach (DataRow dd in datiV.Rows)
          {
            if (dd["ID"].ToString() == splitted[1] && dd["Codice"].ToString() == VociBilancio[splitted[3]].ToString())
              tmpNodenew = dd;
          }
          if (tmpNodenew == null)
          {
            return;
          }


          tmpNodenew["Note"] = "";
          tmpNodenew["NoteNumber"] = "";
          tmpNodenew["NoteRealRow"] = "";

          Grid grd = ((Grid)((StackPanel)i.Parent).Parent);
          StackPanel stphere = ((StackPanel)i.Parent);
          stphere.Children.Clear();

          Image imgbtn = new Image();
          imgbtn.Name = "_" + splitted[1] + "_" + splitted[2] + "_" + splitted[3] + "_AddNota";
          imgbtn.ToolTip = "Aggiungi Nota";
          imgbtn.Margin = new Thickness(0, 5, 0, 0);
          imgbtn.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          imgbtn.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          var uriSource = new Uri(add, UriKind.Relative);
          imgbtn.Source = new BitmapImage(uriSource);
          imgbtn.Height = 10.0;
          imgbtn.Width = 10.0;
          imgbtn.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

          if (this.FindName(imgbtn.Name) != null)
          {
            this.UnregisterName(imgbtn.Name);
          }
          this.RegisterName(imgbtn.Name, imgbtn);

          stphere.Children.Add(imgbtn);

          int indexnotanew = 1;
          ArrayList nametxtblocklist = new ArrayList();
          Hashtable nametxtblocklistHT = new Hashtable();

          foreach (UIElement item in grd.Children)
          {
            if (item.GetType().Name == "StackPanel")
            {
              if ((((StackPanel)item).Children[0]).GetType().Name == "TextBlock")
              {
                if (((TextBlock)((((StackPanel)item).Children[0]))).Name.Contains("txtNoteNumber_"))
                {
                  string[] splitted2 = ((TextBlock)((((StackPanel)item).Children[0]))).Name.Split('_');
                  nametxtblocklist.Add(Convert.ToInt32(splitted2[2]));
                  nametxtblocklistHT.Add(Convert.ToInt32(splitted2[2]), ((TextBlock)((((StackPanel)item).Children[0]))).Name);
                }
              }
            }
          }

          nametxtblocklist.Sort();

          foreach (int item in nametxtblocklist)
          {
            if (this.FindName(nametxtblocklistHT[item].ToString()) != null)
            {
              TextBlock tb = ((TextBlock)(this.FindName(nametxtblocklistHT[item].ToString())));
              tb.Text = indexnotanew.ToString();
              DataRow tmpNodehere = null;
              foreach (DataRow dd in datiV.Rows)
              {
                if (dd["ID"].ToString() == ((TextBlock)(this.FindName(nametxtblocklistHT[item].ToString()))).Name.Split('_')[1] && dd["Codice"].ToString() == VociBilancio[((TextBlock)(this.FindName(nametxtblocklistHT[item].ToString()))).Name.Split('_')[3]].ToString())
                  dd["NoteNumber"] = tb.Text;
              }


              indexnotanew++;
            }
          }

          FillNote(splitted[3]);
        }

        return;
      }
      else if (i.Name.Contains("_AddNota"))
      {
        if (_ReadOnly)
        {
          MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
          return;
        }

        string[] splitted = i.Name.Split('_');
        lastint++;
        DataRow tmpNode = null;
        foreach (DataRow dd in datiV.Rows)
        {
          if (dd["ID"].ToString() == splitted[1] && dd["Codice"].ToString() == VociBilancio[splitted[3]].ToString())
            tmpNode = dd;
        }
        if (tmpNode == null)
        {
          return;
        }



        tmpNode["NoteRealRow"] = splitted[2];

        Grid grdstp = ((Grid)(((StackPanel)i.Parent).Parent));

        StackPanel stphere = ((StackPanel)i.Parent);
        stphere.Children.Clear();

        TextBlock txtnote2 = new TextBlock();
        txtnote2.Name = "txtNoteNumber_" + splitted[1] + "_" + splitted[2] + "_" + splitted[3];
        txtnote2.FontWeight = FontWeights.Bold;
        txtnote2.Margin = new Thickness(0, 0, 5, 0);

        if (tmpNode["NoteNumber"].ToString() == "")
        {
          tmpNode["NoteNumber"] = lastint.ToString();
        }

        txtnote2.Text = tmpNode["NoteNumber"].ToString();

        if (this.FindName(txtnote2.Name) != null)
        {
          this.UnregisterName(txtnote2.Name);
        }
        this.RegisterName(txtnote2.Name, txtnote2);

        stphere.Children.Add(txtnote2);

        Image imgbtn = new Image();
        imgbtn.Name = "_" + splitted[1] + "_" + splitted[2] + "_" + splitted[3] + "_RemoveNota";
        imgbtn.ToolTip = "Rimuovi Nota";
        imgbtn.Margin = new Thickness(0, 5, 0, 0);
        imgbtn.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
        imgbtn.VerticalAlignment = System.Windows.VerticalAlignment.Top;

        var uriSource = new Uri(remove, UriKind.Relative);
        imgbtn.Source = new BitmapImage(uriSource);
        imgbtn.Height = 10.0;
        imgbtn.Width = 10.0;
        imgbtn.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

        if (this.FindName(imgbtn.Name) != null)
        {
          this.UnregisterName(imgbtn.Name);
        }
        this.RegisterName(imgbtn.Name, imgbtn);

        stphere.Children.Add(imgbtn);

        int indexnotanew = 1;
        ArrayList nametxtblocklist = new ArrayList();
        Hashtable nametxtblocklistHT = new Hashtable();

        foreach (UIElement item in grdstp.Children)
        {
          if (item.GetType().Name == "StackPanel")
          {
            if (((StackPanel)item).Children.Count > 0 && (((StackPanel)item).Children[0]).GetType().Name == "TextBlock")
            {
              if (((TextBlock)((((StackPanel)item).Children[0]))).Name.Contains("txtNoteNumber_"))
              {
                string[] splitted2 = ((TextBlock)((((StackPanel)item).Children[0]))).Name.Split('_');
                if (splitted2.Count() > 2)
                {
                  nametxtblocklist.Add(Convert.ToInt32(splitted2[2]));
                  nametxtblocklistHT.Add(Convert.ToInt32(splitted2[2]), ((TextBlock)((((StackPanel)item).Children[0]))).Name);
                }
              }
            }
          }
        }

        nametxtblocklist.Sort();

        foreach (int item in nametxtblocklist)
        {
          if (this.FindName(nametxtblocklistHT[item].ToString()) != null)
          {
            TextBlock tb = ((TextBlock)(this.FindName(nametxtblocklistHT[item].ToString())));
            tb.Text = indexnotanew.ToString();

            foreach (DataRow dd in datiV.Rows)
            {
              if (dd["ID"].ToString() == ((TextBlock)(this.FindName(nametxtblocklistHT[item].ToString()))).Name.Split('_')[1] && dd["Codice"].ToString() == VociBilancio[((TextBlock)(this.FindName(nametxtblocklistHT[item].ToString()))).Name.Split('_')[3]].ToString())
                dd["NoteNumber"] = tb.Text;
            }


            indexnotanew++;
          }
        }

        FillNote(splitted[3]);

        return;
      }
      else if (i.Name.Contains("_Add"))
      {
        if (_ReadOnly)
        {
          MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
          return;
        }

        string stkea = i.Name.Replace("_Add", "_STKEA");
        string stknote = i.Name.Replace("_Add", "_STKNOTE");
        string stkname = i.Name.Replace("_Add", "_STKName");
        string[] splittedstring = stkea.Split('_');

        if (splittedstring[2] == "11611" || splittedstring[2] == "120")
        {
          return;
        }


        if (this.FindName(stkea) != null)
        {
          StackPanel stkp = ((StackPanel)(this.FindName(stkea)));
          string idsessionebilancio = cBusinessObjects.CercaSessione("Revisione", "Bilancio", IDSessione, cBusinessObjects.idcliente);

          DataTable datibilancioverifica = cBusinessObjects.GetData(int.Parse(IDB_Padre), typeof(BilancioVerifica), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);

          bool trovato = false;
          foreach (DataRow dd in datibilancioverifica.Rows)
          {
            if (dd["ID"].ToString() == splittedstring[2])
              trovato = true;
          }
          if (trovato)
          {
            //Esiste bilancio di verifica

            //Azzero valori attuali

            for (int z = 0; z < stkp.Children.Count; z++)
            {

              for (int ii = datiVRighe.Rows.Count - 1; ii >= 0; ii--)
              {

                DataRow dtrow = datiVRighe.Rows[ii];
                if (dtrow["ID"].ToString() == splittedstring[2] && dtrow["Codice"].ToString() == VociBilancio[splittedstring[4]].ToString())
                {
                  if (dtrow["row"].ToString() == z.ToString())
                  {
                    dtrow.Delete();
                  }
                }

              }
              datiVRighe.AcceptChanges();

            }

            for (int s = (stkp.Children.Count - 1); s > 0; s--)
            {
              stkp.Children.Remove(stkp.Children[s]);
            }

            int rowhere = 2;

            TextBlock tbhere = new TextBlock();
            tbhere.Name = "txtEA_" + splittedstring[2] + "_TOT";
            tbhere.Background = Brushes.LightGreen;
            tbhere.Text = "";//Valore Bilancio
            tbhere.ToolTip = "Squadratura";
            tbhere.VerticalAlignment = VerticalAlignment.Center;
            tbhere.TextAlignment = TextAlignment.Right;
            tbhere.Margin = new Thickness(0, 5, 0, 0);
            stkp.Children.Add(tbhere);

            if (this.FindName(tbhere.Name) != null)
            {
              this.UnregisterName(tbhere.Name);
            }
            this.RegisterName(tbhere.Name, tbhere);


            foreach (DataRow nodeBV in datibilancioverifica.Rows)
            {

              if (nodeBV["ID"].ToString() == splittedstring[2] && nodeBV["esercizio"].ToString() == "EA")
              {


                TextBox txt = new TextBox();
                //txt.Margin = new Thickness(0, 5, 0, 0);
                txt.Name = "txtEA_" + splittedstring[2] + "_" + rowhere.ToString();
                txt.Background = Brushes.LightGoldenrodYellow;
                txt.Height = 20.0;
                txt.VerticalAlignment = VerticalAlignment.Center;
                DataRow tmpnode = null;
                for (int ii = datiVRighe.Rows.Count - 1; ii >= 0; ii--)
                {

                  DataRow dtrow = datiVRighe.Rows[ii];
                  if (dtrow["ID"].ToString() == splittedstring[2] && dtrow["Codice"].ToString() == VociBilancio[splittedstring[4]].ToString())
                  {
                    if (dtrow["row"].ToString() == rowhere.ToString())
                    {
                      tmpnode = dtrow;
                    }
                  }

                }

                if (tmpnode == null)
                {
                  tmpnode = datiVRighe.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
                  tmpnode["ID"] = splittedstring[2];
                  tmpnode["Codice"] = VociBilancio[splittedstring[4]].ToString();
                }

                txt.Text = ConvertNumber(((nodeBV["valore"].ToString() != "") ? nodeBV["valore"].ToString() : ""));
                tmpnode["EA"] = txt.Text;
                tmpnode["row"] = rowhere;

                txt.TextAlignment = TextAlignment.Right;
                txt.IsHitTestVisible = false;

                //txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
                //txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
                //txt.LostFocus += new RoutedEventHandler(txtEA_LostFocus);

                stkp.Children.Add(txt);

                rowhere++;
              }
            }


          }
          else
          {
            int rowhere = stkp.Children.Count + 1;
            for (int ii = datiVRighe.Rows.Count - 1; ii >= 0; ii--)
            {

              DataRow dtrow = datiVRighe.Rows[ii];
              if (dtrow["ID"].ToString() == splittedstring[2] && dtrow["Codice"].ToString() == VociBilancio[splittedstring[4]].ToString())
              {
                if (rowhere <= Convert.ToInt32(dtrow["row"].ToString()))
                {
                  rowhere = Convert.ToInt32(dtrow["row"].ToString()) + 1;
                }
              }

            }



            if (rowhere == 2)
            {
              TextBlock tbhere = new TextBlock();
              tbhere.Name = "txtEA_" + splittedstring[2] + "_TOT";
              tbhere.Background = Brushes.LightGreen;
              tbhere.Text = "";//Valore Bilancio
              tbhere.ToolTip = "Squadratura";
              tbhere.VerticalAlignment = VerticalAlignment.Center;
              tbhere.TextAlignment = TextAlignment.Right;
              tbhere.Margin = new Thickness(0, 5, 0, 0);
              stkp.Children.Add(tbhere);

              if (this.FindName(tbhere.Name) != null)
              {
                this.UnregisterName(tbhere.Name);
              }
              this.RegisterName(tbhere.Name, tbhere);
            }

            TextBox txt = new TextBox();
            //txt.Margin = new Thickness(0, 5, 0, 0);
            txt.Name = "txtEA_" + splittedstring[2] + "_" + rowhere.ToString();
            txt.Background = Brushes.LightGoldenrodYellow;
            txt.Height = 20.0;
            txt.VerticalAlignment = VerticalAlignment.Center;
            trovato = false;
            foreach (DataRow dtrow in datiVRighe.Rows)
            {
              if (dtrow["row"].ToString() == rowhere.ToString() && dtrow["ID"].ToString() == splittedstring[2] && dtrow["Codice"].ToString() == VociBilancio[splittedstring[4]].ToString())
              {
                trovato = true;
              }

            }

            if (!trovato)
            {

              DataRow aatmpnode = datiVRighe.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
              aatmpnode["ID"] = splittedstring[2];
              aatmpnode["row"] = rowhere;
              aatmpnode["Codice"] = VociBilancio[splittedstring[4]].ToString();

            }
            txt.TextAlignment = TextAlignment.Right;
            txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
            txt.LostFocus += new RoutedEventHandler(txtEA_LostFocus);

            stkp.Children.Add(txt);
          }
        }

        if (this.FindName(stkname) != null)
        {
          StackPanel stkp = ((StackPanel)(this.FindName(stkname)));
          string idsessionebilancio = cBusinessObjects.CercaSessione("Revisione", "Bilancio", IDSessione, cBusinessObjects.idcliente);

          DataTable datibilancioverifica = cBusinessObjects.GetData(int.Parse(IDB_Padre), typeof(BilancioVerifica), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);


          bool trovato = false;
          foreach (DataRow dd in datibilancioverifica.Rows)
          {
            if (dd["ID"].ToString() == splittedstring[2])
            {
              trovato = true;
            }
          }

          if (trovato)
          {
            //Esiste bilancio di verifica

            //Azzero valori attuali

            for (int z = 0; z < stkp.Children.Count; z++)
            {

              for (int ii = datiVRighe.Rows.Count - 1; ii >= 0; ii--)
              {

                DataRow dtrow = datiVRighe.Rows[ii];
                if (dtrow["ID"].ToString() == splittedstring[2] && dtrow["Codice"].ToString() == VociBilancio[splittedstring[4]].ToString())
                {
                  if (dtrow["row"].ToString() == z.ToString())
                  {
                    dtrow["Codice"] = "";
                    dtrow["Titolo"] = "";
                  }
                }

              }

            }

            for (int s = (stkp.Children.Count - 1); s > 0; s--)
            {
              stkp.Children.Remove(stkp.Children[s]);
            }

            int rowhere = 2;

            StackPanel nstr = new StackPanel();
            nstr.Orientation = Orientation.Horizontal;

            TextBlock tbhere = new TextBlock();
            //tbhere.Background = Brushes.LightYellow;
            tbhere.Text = "Codice";
            tbhere.Width = 130.0;
            tbhere.FontWeight = FontWeights.Bold;
            tbhere.Margin = new Thickness(20, 5, 0, 0);
            nstr.Children.Add(tbhere);

            tbhere = new TextBlock();
            //tbhere.Background = Brushes.LightYellow;
            tbhere.Text = "Denominazione Conto Contabile";
            //tbhere.Width = 200.0;
            tbhere.FontWeight = FontWeights.Bold;
            tbhere.Margin = new Thickness(0, 5, 0, 0);
            nstr.Children.Add(tbhere);

            stkp.Children.Add(nstr);

            foreach (DataRow nodeBV in datibilancioverifica.Rows)
            //_xB.Document.SelectNodes("/Dati//Dato[@ID='" + IDB_Padre + "']/Valore[@ID='" + splittedstring[2] + "']/BV[@esercizio='EA']"))
            {
              if (nodeBV["ID"].ToString() == splittedstring[2] && nodeBV["esercizio"].ToString() == "EA")
              {


                StackPanel stkpp = new StackPanel();
                stkpp.Orientation = Orientation.Horizontal;
                stkpp.HorizontalAlignment = HorizontalAlignment.Stretch;

                Image imgbtn = new Image();
                imgbtn.Name = "_" + _ID + "_" + splittedstring[2] + "_" + splittedstring[3] + "_" + splittedstring[4] + "_" + rowhere + "_Remove";
                imgbtn.ToolTip = "Rimuovi Conto Contabile";
                imgbtn.Margin = new Thickness(0, 5, 0, 0);
                imgbtn.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                imgbtn.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                var uriSource = new Uri(remove, UriKind.Relative);
                imgbtn.Source = new BitmapImage(uriSource);
                imgbtn.Height = 10.0;
                imgbtn.Width = 10.0;
                imgbtn.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

                stkpp.Children.Add(imgbtn);

                StackPanel nstr2 = new StackPanel();
                nstr2.Orientation = Orientation.Horizontal;
                nstr2.Name = "StackPanel_" + splittedstring[2] + "_" + rowhere.ToString();

                TextBox txt = new TextBox();
                //txt.Margin = new Thickness(0, 5, 0, 0);
                txt.Name = "txtCodice_" + splittedstring[2] + "_" + rowhere.ToString();
                txt.Background = Brushes.LightGoldenrodYellow;
                txt.Width = 130.0;
                txt.Height = 20.0;
                txt.VerticalAlignment = VerticalAlignment.Center;

                txt.Text = ((nodeBV["codice"].ToString() != "") ? nodeBV["codice"].ToString() : "");
                DataRow tmpdatirighe = null;
                foreach (DataRow dtrow in datiVRighe.Rows)
                {
                  if (dtrow["row"].ToString() == rowhere.ToString() && dtrow["ID"].ToString() == splittedstring[2] && dtrow["Codice"].ToString() == VociBilancio[splittedstring[4]].ToString())
                  {
                    tmpdatirighe = dtrow;
                  }

                }

                if (tmpdatirighe == null)
                {
                  tmpdatirighe = datiVRighe.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
                  tmpdatirighe["ID"] = splittedstring[2];
                  tmpdatirighe["row"] = rowhere;
                }

                tmpdatirighe["Codice"] = txt.Text;


                txt.IsHitTestVisible = false;

                txt.TextAlignment = TextAlignment.Left;
                //txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
                //txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
                //txt.LostFocus += new RoutedEventHandler(txtCodice_LostFocus);

                nstr2.Children.Add(txt);

                txt = new TextBox();
                //txt.Margin = new Thickness(0, 5, 0, 0);
                txt.Name = "txtTitolo_" + splittedstring[2] + "_" + rowhere.ToString();
                txt.Background = Brushes.LightGoldenrodYellow;

                if (this.FindName(txt.Name) != null)
                {
                  this.UnregisterName(txt.Name);
                }
                this.RegisterName(txt.Name, txt);


                double actualwidth = ((Grid)(txtDescrizioneIntensita.Parent)).ActualWidth;
                txt.Width = (actualwidth - 800 > 100) ? actualwidth - 800 : 100;

                txt.Height = 20.0;
                txt.VerticalAlignment = VerticalAlignment.Center;


                txt.Text = ((nodeBV["titolo"].ToString() != "") ? nodeBV["titolo"].ToString() : "");
                tmpdatirighe["Titolo"] = txt.Text;

                txt.TextAlignment = TextAlignment.Left;
                txt.IsHitTestVisible = false;

                //txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
                //txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
                //txt.LostFocus += new RoutedEventHandler(txtTitolo_LostFocus);

                nstr2.Children.Add(txt);

                stkpp.Children.Add(nstr2);

                stkp.Children.Add(stkpp);

                rowhere++;
              }
            }
          }
          else
          {
            int rowhere = stkp.Children.Count + 1;

            for (int ii = datiVRighe.Rows.Count - 1; ii >= 0; ii--)
            {

              DataRow dtrow = datiVRighe.Rows[ii];
              if (dtrow["ID"].ToString() == splittedstring[2] && dtrow["Codice"].ToString() == VociBilancio[splittedstring[4]].ToString())
              {
                if (rowhere <= Convert.ToInt32(dtrow["row"].ToString()))
                {
                  rowhere = Convert.ToInt32(dtrow["row"].ToString()) + 1;
                }
              }

            }
            if (rowhere == 2)
            {
              StackPanel nstr = new StackPanel();
              nstr.Orientation = Orientation.Horizontal;

              TextBlock tbhere = new TextBlock();
              //tbhere.Background = Brushes.LightYellow;
              tbhere.Text = "Codice";
              tbhere.Width = 130.0;
              tbhere.FontWeight = FontWeights.Bold;
              tbhere.Margin = new Thickness(20, 5, 0, 0);
              nstr.Children.Add(tbhere);

              tbhere = new TextBlock();
              //tbhere.Background = Brushes.LightYellow;
              tbhere.Text = "Denominazione Conto Contabile";
              //tbhere.Width = 200.0;
              tbhere.FontWeight = FontWeights.Bold;
              tbhere.Margin = new Thickness(0, 5, 0, 0);
              nstr.Children.Add(tbhere);

              stkp.Children.Add(nstr);
            }

            StackPanel stkpp = new StackPanel();
            stkpp.Orientation = Orientation.Horizontal;
            stkpp.HorizontalAlignment = HorizontalAlignment.Stretch;

            Image imgbtn = new Image();
            imgbtn.Name = "_" + _ID + "_" + splittedstring[2] + "_" + splittedstring[3] + "_" + splittedstring[4] + "_" + rowhere + "_Remove";
            imgbtn.ToolTip = "Rimuovi Conto Contabile";
            imgbtn.Margin = new Thickness(0, 5, 0, 0);
            imgbtn.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            imgbtn.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            var uriSource = new Uri(remove, UriKind.Relative);
            imgbtn.Source = new BitmapImage(uriSource);
            imgbtn.Height = 10.0;
            imgbtn.Width = 10.0;
            imgbtn.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

            stkpp.Children.Add(imgbtn);

            StackPanel nstr2 = new StackPanel();
            nstr2.Orientation = Orientation.Horizontal;
            nstr2.Name = "StackPanel_" + splittedstring[2] + "_" + rowhere.ToString();

            TextBox txt = new TextBox();
            //txt.Margin = new Thickness(0, 5, 0, 0);
            txt.Name = "txtCodice_" + splittedstring[2] + "_" + rowhere.ToString();
            txt.Background = Brushes.LightGoldenrodYellow;
            txt.Width = 130.0;
            txt.Height = 20.0;
            txt.VerticalAlignment = VerticalAlignment.Center;
            DataRow tmpdatirighe = null;

            foreach (DataRow dtrow in datiVRighe.Rows)
            {
              if (dtrow["row"].ToString() == rowhere.ToString() && dtrow["ID"].ToString() == splittedstring[2] && dtrow["Codice"].ToString() == VociBilancio[splittedstring[4]].ToString())
              {
                tmpdatirighe = dtrow;
              }

            }

            if (tmpdatirighe == null)
            {
              tmpdatirighe = datiVRighe.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
              tmpdatirighe["ID"] = splittedstring[2];
              tmpdatirighe["row"] = rowhere;
            }


            txt.TextAlignment = TextAlignment.Left;
            txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
            txt.LostFocus += new RoutedEventHandler(txtCodice_LostFocus);

            nstr2.Children.Add(txt);

            txt = new TextBox();
            //txt.Margin = new Thickness(0, 5, 0, 0);
            txt.Name = "txtTitolo_" + splittedstring[2] + "_" + rowhere.ToString();
            txt.Background = Brushes.LightGoldenrodYellow;

            if (this.FindName(txt.Name) != null)
            {
              this.UnregisterName(txt.Name);
            }
            this.RegisterName(txt.Name, txt);


            double actualwidth = ((Grid)(txtDescrizioneIntensita.Parent)).ActualWidth;
            txt.Width = (actualwidth - 800 > 100) ? actualwidth - 800 : 100;

            txt.Height = 20.0;
            txt.VerticalAlignment = VerticalAlignment.Center;

            txt.TextAlignment = TextAlignment.Left;
            txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
            txt.LostFocus += new RoutedEventHandler(txtTitolo_LostFocus);

            nstr2.Children.Add(txt);

            stkpp.Children.Add(nstr2);

            stkp.Children.Add(stkpp);
          }
        }

        return;
      }
      else if (i.Name.Contains("_Remove"))
      {
        if (_ReadOnly)
        {
          MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
          return;
        }

        string[] presplitted = i.Name.Split('_');

        string rowitem = presplitted[5];

        string stkea = presplitted[0] + "_" + presplitted[1] + "_" + presplitted[2] + "_" + presplitted[3] + "_" + presplitted[4] + "_STKEA";
        string stkname = presplitted[0] + "_" + presplitted[1] + "_" + presplitted[2] + "_" + presplitted[3] + "_" + presplitted[4] + "_STKName";
        string[] splittedstring = stkea.Split('_');

        //    XmlNode tmpnode = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]/Valore[@ID=\"" + splittedstring[2] + "\"][@Codice=\"" + VociBilancio[splittedstring[4]] + "\"]");


        if (this.FindName(stkea) != null)
        {
          StackPanel stkp = ((StackPanel)(this.FindName(stkea)));
          for (int ii = datiVRighe.Rows.Count - 1; ii >= 0; ii--)
          {

            DataRow dtrow = datiVRighe.Rows[ii];
            if (dtrow["ID"].ToString() == splittedstring[2] && dtrow["Codice"].ToString() == VociBilancio[splittedstring[4]].ToString())
            {
              if (dtrow["row"].ToString() == rowitem.ToString())
              {
                dtrow.Delete();
              }
            }

          }
          datiVRighe.AcceptChanges();



          for (int w = 0; w < stkp.Children.Count; w++)
          {
            UIElement item = stkp.Children[w];

            if (item.GetType().Name == "TextBox")
            {
              if (((TextBox)item).Name == "txtEA_" + splittedstring[2] + "_" + rowitem.ToString())
              {
                stkp.Children.Remove(item);
              }
            }
          }

          if (stkp.Children.Count == 2)
          {
            stkp.Children.RemoveAt(1);
          }

          double val1 = 0.0;
          double valtot = 0.0;
          double testd = 0.0;

          int indexhere = 0;

          foreach (UIElement item in stkp.Children)
          {
            if (item.GetType().Name == "TextBox")
            {
              if (indexhere == 0)
              {
                if (Double.TryParse(((TextBox)item).Text, out testd))
                {
                  val1 = Convert.ToSingle(((TextBox)item).Text);
                }
              }
              else
              {
                if (Double.TryParse(((TextBox)item).Text, out testd))
                {
                  valtot += Convert.ToSingle(((TextBox)item).Text);
                }
              }

              indexhere++;
            }
          }

          if (this.FindName("txtEA_" + splittedstring[2] + "_TOT") != null)
          {
            TextBlock stphere = ((TextBlock)(this.FindName("txtEA_" + splittedstring[2] + "_TOT")));
            stphere.Text = ConvertNumber((val1 - valtot).ToString());
          }
        }

        if (this.FindName(stkname) != null)
        {
          StackPanel stkp = ((StackPanel)(this.FindName(stkname)));


          for (int w = 0; w < stkp.Children.Count; w++)
          {
            UIElement item = stkp.Children[w];

            if (item.GetType().Name == "StackPanel")
            {
              if ((((StackPanel)item).Children[1]).GetType().Name == "StackPanel")
              {
                if (((StackPanel)((StackPanel)item).Children[1]).Name == "StackPanel_" + splittedstring[2] + "_" + rowitem.ToString())
                {
                  stkp.Children.Remove(item);
                }
              }
            }
          }

          if (stkp.Children.Count == 2)
          {
            stkp.Children.RemoveAt(1);
          }
        }

        return;
      }

      try
      {
        DataRow tmpnode = null;
        foreach (DataRow dd in datiN.Rows)
        {
          if (dd["ID"].ToString() == ((Grid)(i.Parent)).Name.Split('_')[1])
            tmpnode = dd;

        }
        if (tmpnode == null)
        {
          tmpnode = datiN.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
          tmpnode["ID"] = ((Grid)(i.Parent)).Name.Split('_')[1];
        }



        Grid u = ((Grid)(((Grid)(i.Parent)).Children[2]));

        if (u.Visibility == System.Windows.Visibility.Collapsed)
        {
          u.Visibility = System.Windows.Visibility.Visible;
          var uriSource = new Uri(down, UriKind.Relative);
          i.Source = new BitmapImage(uriSource);

          tmpnode["Chiuso"] = "False";
        }
        else
        {
          u.Visibility = System.Windows.Visibility.Collapsed;
          var uriSource = new Uri(left, UriKind.Relative);
          i.Source = new BitmapImage(uriSource);

          tmpnode["Chiuso"] = "True";
        }
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
    }

    private void RetrieveData()
    {
      string idsessionebilancio = cBusinessObjects.CercaSessione("Revisione", "Bilancio", IDSessione, cBusinessObjects.idcliente);

      DataTable datibilancio = cBusinessObjects.GetData(int.Parse(IDB_Padre), typeof(Excel_Bilancio), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);

      foreach (DataRow node in datibilancio.Rows)
      {
        //Calcolo valori attuali

        if (node["EA"].ToString() != "")
        {
          if (!b_valoreEA.Contains(node["ID"].ToString()))
          {
            b_valoreEA.Add(node["ID"].ToString(), node["EA"].ToString());
          }
          else
          {
            b_valoreEA[node["ID"].ToString()] = node["EA"].ToString();
          }
        }
        else
        {
          if (!b_valoreEA.Contains(node["ID"].ToString()))
          {
            b_valoreEA.Add(node["ID"].ToString(), "0");
          }
          else
          {
            b_valoreEA[node["ID"].ToString()] = "0";
          }
        }
      }
    }

    private void GenerateData(string file, string Codice, string Titolo, bool WithData, bool insertdata)
    {
      XmlDataProviderManager _yBilancio = new XmlDataProviderManager(file, true);

      b_Titolo.Clear();
      b_NoData.Clear();
      b_Ordine.Clear();

      foreach (XmlNode item in _y.Document.SelectNodes("/LEADS/LEAD[@ID='" + Codice + "']/RIGA"))
      {
        XmlNode nodobilancio = _yBilancio.Document.SelectSingleNode("/Dato/MacroGruppo/Bilancio[@ID='" + item.Attributes["ID"].Value + "']");
        if ((nodobilancio != null || (item.Attributes["TIPO"] != null && item.Attributes["TIPO"].Value == Titolo)) && !(item.Attributes["ID"].Value == "190" && Codice == "3.4.7"))
        {
          DataRow dt = null;
          foreach (DataRow ddd in datiV.Rows)
          {
            if (ddd["ID"].ToString() == item.Attributes["ID"].Value && ddd["Codice"].ToString() == Codice)
            {
              dt = ddd;
            }
          }
          if (dt == null)
            dt = datiV.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
          dt["Codice"] = Codice;
          dt["Tipo"] = Titolo;
          dt["ID"] = item.Attributes["ID"].Value;


          b_Ordine.Add(item.Attributes["ID"].Value);

          if (nodobilancio != null && (nodobilancio.Attributes["noData"] != null || nodobilancio.Attributes["rigaVuota"] != null))
          {
            b_NoData.Add(item.Attributes["ID"].Value, true);
          }
          else
          {
            if (!WithData)
            {


            }
            else
            {
              string eavalue = "";



              if (item.Attributes["SOMMA"] != null)
              {
                double tdeavalue = 0;

                foreach (string itemSOMMA in item.Attributes["SOMMA"].Value.Split('|'))
                {
                  double deavalue = 0;

                  double.TryParse((b_valoreEA.Contains(itemSOMMA) ? b_valoreEA[itemSOMMA].ToString() : "0"), out deavalue);

                  tdeavalue += deavalue;
                }

                eavalue = ConvertNumber(tdeavalue.ToString());

                dt["somma"] = item.Attributes["SOMMA"].Value;
              }
              else
              {
                eavalue = (b_valoreEA.Contains(item.Attributes["ID"].Value) ? b_valoreEA[item.Attributes["ID"].Value].ToString() : "0");
              }

              dt["EA"] = eavalue;


            }

            b_NoData.Add(item.Attributes["ID"].Value, false);
          }

          string titolo = "";

          if (nodobilancio != null && nodobilancio.Attributes["Codice"] != null)
          {
            titolo += nodobilancio.Attributes["Codice"].Value + " ";
          }

          if (nodobilancio != null && nodobilancio.Attributes["name"] != null)
          {
            titolo += nodobilancio.Attributes["name"].Value;
          }
          if (!(item.Attributes["ID"].Value == "190" && Codice == "3.4.7"))
            titolo = ((item.Attributes["TITOLO"] != null) ? item.Attributes["TITOLO"].Value : titolo);

          dt["Titolo"] = titolo;

          b_Titolo.Add(item.Attributes["ID"].Value, titolo);



        }
      }
    }

    private bool GenerateGrid_originale(KeyValuePair<int, string> item, Grid grd, string Codice, ref int rowhere, string Titolo)
    {
      int initialrowhere = rowhere + 1;
      bool total = false;

      bool checkexistance = false;

      foreach (string itemdata in b_Ordine)
      {
        DataRow tmpNode = null; // _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]/Valore[@ID=\"" + itemdata + "\"][@Codice=\"" + Codice + "\"]");

        foreach (DataRow dd in datiV.Rows)
        {
          if (dd["ID"].ToString() == itemdata && dd["Codice"].ToString() == Codice)
            tmpNode = dd;

        }

        //XmlNode tmpNodeChild = null;

        bool hasdata = false;

        if (b_NoData.Contains(itemdata) && (bool)(b_NoData[itemdata]) == true)
        {
          foreach (XmlNode child in _y.Document.SelectNodes("/LEADS/LEAD[@ID='" + Codice + "']/RIGA[@PADRE='" + itemdata + "']"))
          {
            if (b_NoData.Contains(child.Attributes["ID"].Value) && (bool)(b_NoData[child.Attributes["ID"].Value]) == true)
            {
              foreach (XmlNode grandchild in _y.Document.SelectNodes("/LEADS/LEAD[@ID='" + Codice + "']/RIGA[@PADRE='" + child.Attributes["ID"].Value + "']"))
              {
                if (b_NoData.Contains(grandchild.Attributes["ID"].Value) && (bool)(b_NoData[grandchild.Attributes["ID"].Value]) == true)
                {
                  foreach (XmlNode grandgrandchild in _y.Document.SelectNodes("/LEADS/LEAD[@ID='" + Codice + "']/RIGA[@PADRE='" + grandchild.Attributes["ID"].Value + "']"))
                  {
                    if (b_NoData.Contains(grandgrandchild.Attributes["ID"].Value) && (bool)(b_NoData[grandgrandchild.Attributes["ID"].Value]) == true)
                    {
                      ;
                    }
                    else
                    {
                      //    tmpNodeChild = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]/Valore[@ID=\"" + grandgrandchild.Attributes["ID"].Value + "\"][@Codice=\"" + Codice + "\"]");
                      foreach (DataRow dd in datiV.Rows)
                      {
                        if (dd["ID"].ToString() == grandgrandchild.Attributes["ID"].Value && dd["Codice"].ToString() == Codice)
                        {
                          hasdata = true;
                          total = true;
                        }

                      }

                    }
                  }
                }
                else
                {
                  //  tmpNodeChild = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]/Valore[@ID=\"" + grandchild.Attributes["ID"].Value + "\"][@Codice=\"" + Codice + "\"]");
                  foreach (DataRow dd in datiV.Rows)
                  {
                    if (dd["ID"].ToString() == grandchild.Attributes["ID"].Value && dd["Codice"].ToString() == Codice)
                    {
                      hasdata = true;
                      total = true;
                    }

                  }

                }
              }
            }
            else
            {
              foreach (DataRow dd in datiV.Rows)
              {
                if (dd["ID"].ToString() == child.Attributes["ID"].Value && dd["Codice"].ToString() == Codice)
                {
                  hasdata = true;
                  total = true;
                }

              }

            }
          }
        }
        else
        {
          if (tmpNode != null && ((tmpNode["Tipo"].ToString() == "" || tmpNode["Tipo"].ToString() == "Ulteriori dati opzionali (acquisizione non automatica)") || (((tmpNode["EA"].ToString() != "" && tmpNode["EA"].ToString() != "0")))))
          {

            hasdata = true;
            total = true;
          }
        }

        if (hasdata == false || (b_Titolo[itemdata].ToString() == "Totale" && total == false))
        {
          continue;
        }


        if (b_Titolo[itemdata].ToString() == "Totale" && total == true)
        {
          total = false;
        }


        if ((bool)(b_NoData[itemdata]) == true)
        {
          //Riga vuota
          rowhere++;
          RowDefinition rd2 = new RowDefinition();
          rd2.Height = new GridLength(10);
          grd.RowDefinitions.Add(rd2);

        }

        RowDefinition rd = new RowDefinition();
        rd.Height = GridLength.Auto;
        grd.RowDefinitions.Add(rd);

        rowhere++;

        bool bold = false;

        StackPanel stkp = new StackPanel();
        stkp.Margin = new Thickness(0, 0, 0, 5);
        stkp.Orientation = Orientation.Vertical;
        stkp.Name = "_" + _ID + "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_STKName";
        this.RegisterName(stkp.Name, stkp);

        TextBlock lbl = new TextBlock();
        lbl.Height = 20.0;
        lbl.VerticalAlignment = VerticalAlignment.Center;

        if (b_Titolo[itemdata].ToString() == "Totale")
        {
          Image imgbtn = new Image();
          imgbtn.Name = "_" + _ID + "_" + itemdata + "_" + initialrowhere.ToString() + "_" + rowhere.ToString() + "_Btn";
          imgbtn.ToolTip = "Espandi";
          imgbtn.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          imgbtn.VerticalAlignment = System.Windows.VerticalAlignment.Center;

          var uriSource = new Uri(left, UriKind.Relative);


          //if (tmpNode.Attributes["Chiuso"].Value == "True")
          //{
          //    uriSource = new Uri(left, UriKind.Relative);

          //    for (int i = initialrowhere; i < rowhere; i++)
          //    {
          //        grd.RowDefinitions[i].Height = new GridLength(0, GridUnitType.Pixel);
          //    }
          //}
          //else
          //{
          for (int i = initialrowhere; i < rowhere; i++)
          {
            grd.RowDefinitions[i].Height = GridLength.Auto;
          }

          uriSource = new Uri(up, UriKind.Relative);
          //}

          imgbtn.Source = new BitmapImage(uriSource);
          imgbtn.Height = 10.0;
          imgbtn.Width = 10.0;
          imgbtn.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

          grd.Children.Add(imgbtn);
          Grid.SetRow(imgbtn, rowhere);
          Grid.SetColumn(imgbtn, 0);

          lbl.Text = Titolo;
          bold = true;
        }
        else
        {
          if ((bool)(b_NoData[itemdata]) == false && itemdata != "11611" && itemdata != "120")
          {
            Image imgbtn = new Image();
            imgbtn.Name = "_" + _ID + "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_Add";
            imgbtn.Margin = new Thickness(0, 5, 0, 0);
            imgbtn.ToolTip = "Aggiungi Conto Contabile";
            imgbtn.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            imgbtn.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            var uriSource = new Uri(add, UriKind.Relative);
            imgbtn.Source = new BitmapImage(uriSource);
            imgbtn.Height = 10.0;
            imgbtn.Width = 10.0;
            imgbtn.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

            grd.Children.Add(imgbtn);
            Grid.SetRow(imgbtn, rowhere);
            Grid.SetColumn(imgbtn, 0);
          }

          lbl.Text = b_Titolo[itemdata].ToString();
        }

        if (lbl.Text == "Sub Totale")
        {
          bold = true;
        }

        if (bold == true)
        {
          lbl.FontWeight = FontWeights.Bold;
        }

        lbl.TextWrapping = TextWrapping.Wrap;
        lbl.HorizontalAlignment = HorizontalAlignment.Left;
        if ((bool)(b_NoData[itemdata]) == true)
        {
          lbl.FontWeight = FontWeights.Bold;
        }

        stkp.Children.Add(lbl);

        // XmlNode tmpNode = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]/Valore[@ID=\"" + itemdata + "\"][@Codice=\"" + Codice + "\"]");

        foreach (DataRow attritem in datiVRighe.Rows)
        {
          if (attritem["ID"].ToString() != itemdata || attritem["Codice"].ToString() != Codice)
            continue;

          if (stkp.Children.Count == 1)
          {
            StackPanel nstr = new StackPanel();
            nstr.Orientation = Orientation.Horizontal;

            TextBlock tbhere = new TextBlock();
            //tbhere.Background = Brushes.LightYellow;
            tbhere.Text = "Codice";
            tbhere.Width = 130.0;
            tbhere.FontWeight = FontWeights.Bold;
            tbhere.Margin = new Thickness(20, 5, 0, 0);
            nstr.Children.Add(tbhere);

            tbhere = new TextBlock();
            //tbhere.Background = Brushes.LightYellow;
            tbhere.Text = "Denominazione Conto Contabile";
            tbhere.FontWeight = FontWeights.Bold;
            tbhere.Margin = new Thickness(0, 5, 0, 0);
            nstr.Children.Add(tbhere);

            stkp.Children.Add(nstr);
          }

          StackPanel stkpp = new StackPanel();
          stkpp.Orientation = Orientation.Horizontal;
          stkpp.HorizontalAlignment = HorizontalAlignment.Stretch;

          string rowitem = attritem["row"].ToString();

          Image imgbtn = new Image();
          imgbtn.Name = "_" + _ID + "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_" + rowitem + "_Remove";
          imgbtn.ToolTip = "Rimuovi Conto Contabile";
          imgbtn.Margin = new Thickness(0, 5, 0, 0);
          imgbtn.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          imgbtn.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          var uriSource = new Uri(remove, UriKind.Relative);
          imgbtn.Source = new BitmapImage(uriSource);
          imgbtn.Height = 10.0;
          imgbtn.Width = 10.0;
          imgbtn.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

          stkpp.Children.Add(imgbtn);

          StackPanel nstr2 = new StackPanel();
          nstr2.Orientation = Orientation.Horizontal;
          nstr2.Name = "StackPanel_" + itemdata + "_" + rowitem;

          TextBox txt = new TextBox();
          //txt.Margin = new Thickness(0, 5, 0, 0);
          txt.Name = "txtCodice_" + itemdata + "_" + rowitem;
          txt.Background = Brushes.LightGoldenrodYellow;
          txt.Width = 130.0;
          txt.Height = 20.0;
          txt.VerticalAlignment = VerticalAlignment.Center;


          txt.Text = attritem["Codice"].ToString();

          //if (tmpNode.Attributes["Codice_" + rowhere] == null)
          //{
          //    XmlAttribute attr = tmpNode.OwnerDocument.CreateAttribute("Codice_" + rowhere);
          //    tmpNode.Attributes.Append(attr);
          //}
          txt.TextAlignment = TextAlignment.Left;
          txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
          txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          txt.LostFocus += new RoutedEventHandler(txtCodice_LostFocus);

          nstr2.Children.Add(txt);

          txt = new TextBox();
          //txt.Margin = new Thickness(0, 5, 0, 0);
          txt.Name = "txtTitolo_" + itemdata + "_" + rowitem;
          txt.Background = Brushes.LightGoldenrodYellow;

          if (this.FindName(txt.Name) != null)
          {
            this.UnregisterName(txt.Name);
          }
          this.RegisterName(txt.Name, txt);

          txt.Height = 20.0;
          txt.VerticalAlignment = VerticalAlignment.Center;


          txt.Text = attritem["Titolo"].ToString();
          txt.ToolTip = txt.Text;

          //if (tmpNode.Attributes["Titolo_" + rowhere] == null)
          //{
          //    XmlAttribute attr = tmpNode.OwnerDocument.CreateAttribute("Titolo_" + rowhere);
          //    tmpNode.Attributes.Append(attr);
          //}
          txt.TextAlignment = TextAlignment.Left;
          txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
          txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          txt.LostFocus += new RoutedEventHandler(txtTitolo_LostFocus);
          txt.HorizontalAlignment = HorizontalAlignment.Left;

          nstr2.Children.Add(txt);

          stkpp.Children.Add(nstr2);


          //TextBox txtitem = new TextBox();
          //txtitem.Name = "txtTitolo_" + itemdata + "_" + rowitem;
          //txtitem.Background = Brushes.LightYellow;
          //txtitem.Width = 270.0;
          //txtitem.Height = 20.0;
          //txtitem.VerticalAlignment = VerticalAlignment.Center;

          //if (tmpNode != null && tmpNode.Attributes["Titolo_" + rowitem] != null)
          //{
          //    txtitem.Text = tmpNode.Attributes["Titolo_" + rowitem].Value;
          //}
          //txtitem.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
          //txtitem.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          //txtitem.LostFocus += new RoutedEventHandler(txtTitolo_LostFocus);

          //stkpp.Children.Add(txtitem);

          stkp.Children.Add(stkpp);

        }

        grd.Children.Add(stkp);
        Grid.SetRow(stkp, rowhere);
        Grid.SetColumn(stkp, 1);

        //if (!donehere.Contains(itemdata))
        //{
        //    donehere.Add(itemdata);
        //}

        if ((bool)(b_NoData[itemdata]) == false)
        {
          stkp = new StackPanel();
          stkp.Margin = new Thickness(0, 0, 0, 5);
          stkp.Orientation = Orientation.Vertical;
          stkp.Name = "_" + _ID + "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_STKEA";
          this.RegisterName(stkp.Name, stkp);

          TextBox txt = new TextBox();
          if (bold == true)
          {
            txt.FontWeight = FontWeights.Bold;
          }
          //txt.Margin = new Thickness(0, 5, 0, 0);
          //txt.Name = "txtEA_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString();
          txt.Height = 20.0;
          txt.VerticalAlignment = VerticalAlignment.Center;
          if (tmpNode != null && tmpNode["EA"].ToString() != "")
          {
            txt.Text = ConvertNumber(tmpNode["EA"].ToString());
          }
          txt.IsReadOnly = true;
          txt.TextAlignment = TextAlignment.Right;
          //txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
          //txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          //txt.LostFocus += new RoutedEventHandler(txtEA_LostFocus);

          //if (this.FindName(txt.Name) != null)
          //{
          //    this.UnregisterName(txt.Name);
          //}
          //this.RegisterName(txt.Name, txt);

          stkp.Children.Add(txt);
          foreach (DataRow attritem in datiVRighe.Rows)
          {
            if (attritem["ID"].ToString() != itemdata || attritem["Codice"].ToString() != Codice)
              continue;


            if (stkp.Children.Count == 1)
            {
              TextBlock tbhere = new TextBlock();
              tbhere.Name = "txtEA_" + itemdata + "_TOT";
              tbhere.Background = Brushes.LightGreen;
              tbhere.Text = "";//Valore Bilancio
              tbhere.ToolTip = "Squadratura";
              tbhere.VerticalAlignment = VerticalAlignment.Center;
              tbhere.TextAlignment = TextAlignment.Right;
              tbhere.Margin = new Thickness(0, 5, 0, 0);
              stkp.Children.Add(tbhere);

              if (this.FindName(tbhere.Name) != null)
              {
                this.UnregisterName(tbhere.Name);
              }
              this.RegisterName(tbhere.Name, tbhere);
            }

            string rowitem = attritem["row"].ToString();
            txt = new TextBox();
            //txt.Margin = new Thickness(0, 5, 0, 0);
            txt.Name = "txtEA_" + itemdata + "_" + rowitem;
            txt.Background = Brushes.LightGoldenrodYellow;
            txt.Height = 20.0;
            txt.VerticalAlignment = VerticalAlignment.Center;

            txt.Text = ConvertNumber(tmpNode["EA"].ToString());

            txt.TextAlignment = TextAlignment.Right;
            txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
            txt.LostFocus += new RoutedEventHandler(txtEA_LostFocus);

            stkp.Children.Add(txt);

            double val1 = 0.0;
            double valtot = 0.0;
            double testd = 0.0;

            int indexhere = 0;

            foreach (UIElement itemstkp in stkp.Children)
            {
              if (itemstkp.GetType().Name == "TextBox")
              {
                if (indexhere == 0)
                {
                  if (Double.TryParse(((TextBox)itemstkp).Text, out testd))
                  {
                    val1 = Convert.ToSingle(((TextBox)itemstkp).Text);
                  }
                }
                else
                {
                  if (Double.TryParse(((TextBox)itemstkp).Text, out testd))
                  {
                    valtot += Convert.ToSingle(((TextBox)itemstkp).Text);
                  }
                }

                indexhere++;
              }
            }


            if (this.FindName("txtEA_" + itemdata + "_TOT") != null)
            {
              TextBlock stphere = ((TextBlock)(this.FindName("txtEA_" + itemdata + "_TOT")));
              stphere.Text = ConvertNumber((val1 - valtot).ToString());
            }

          }

          grd.Children.Add(stkp);
          Grid.SetRow(stkp, rowhere);
          Grid.SetColumn(stkp, 2);

          //StackPanel stpNoteSuper = new StackPanel();
          //stpNoteSuper.Orientation = Orientation.Vertical;
          //stpNoteSuper.Name = "_" + _ID + "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_STKNOTE";
          //this.RegisterName(stpNoteSuper.Name, stpNoteSuper);

          StackPanel stpnote = new StackPanel();
          stpnote.Orientation = Orientation.Horizontal;
          stpnote.HorizontalAlignment = HorizontalAlignment.Center;
          bool trovato = false;
          if (tmpNode != null)
          {

            foreach (DataRow dd in datiV.Rows)
            {
              if (dd["ID"].ToString() == tmpNode["ID"].ToString() && dd["NoteNumber"].ToString() != "" && dd["NoteRealRow"].ToString() != "")
              {
                trovato = true;
                TextBlock txtnote = new TextBlock();
                txtnote.Name = "txtNoteNumber_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString();
                txtnote.FontWeight = FontWeights.Bold;
                txtnote.Margin = new Thickness(0, 0, 5, 0);

                txtnote.Text = dd["NoteNumber"].ToString();

                if (this.FindName(txtnote.Name) != null)
                {
                  this.UnregisterName(txtnote.Name);
                }
                this.RegisterName(txtnote.Name, txtnote);

                stpnote.Children.Add(txtnote);

                Image imgbtn = new Image();
                imgbtn.Name = "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_RemoveNota";
                imgbtn.ToolTip = "Rimuovi Nota";
                imgbtn.Margin = new Thickness(0, 5, 0, 0);
                imgbtn.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                imgbtn.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                var uriSource = new Uri(remove, UriKind.Relative);
                imgbtn.Source = new BitmapImage(uriSource);
                imgbtn.Height = 10.0;
                imgbtn.Width = 10.0;
                imgbtn.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

                if (this.FindName(imgbtn.Name) != null)
                {
                  this.UnregisterName(imgbtn.Name);
                }
                this.RegisterName(imgbtn.Name, imgbtn);

                stpnote.Children.Add(imgbtn);
              }
            }
          }
          if (!trovato)
          {
            Image imgbtn = new Image();
            imgbtn.Name = "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_AddNota";
            imgbtn.ToolTip = "Aggiungi Nota";
            imgbtn.Margin = new Thickness(0, 5, 0, 0);
            imgbtn.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            imgbtn.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            var uriSource = new Uri(add, UriKind.Relative);
            imgbtn.Source = new BitmapImage(uriSource);
            imgbtn.Height = 10.0;
            imgbtn.Width = 10.0;
            imgbtn.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

            if (this.FindName(imgbtn.Name) != null)
            {
              this.UnregisterName(imgbtn.Name);
            }
            this.RegisterName(imgbtn.Name, imgbtn);

            stpnote.Children.Add(imgbtn);
          }

          //stpNoteSuper.Children.Add(stpnote);

          //foreach (XmlAttribute attritem in tmpNode.Attributes)
          //{
          //    if (attritem.Name.Contains("NOTECC_"))
          //    {
          //        if (stpNoteSuper.Children.Count == 1)
          //        {
          //            TextBlock tbhere = new TextBlock();
          //            tbhere.Text = "";
          //            tbhere.FontWeight = FontWeights.Bold;
          //            tbhere.Margin = new Thickness(0, 5, 0, 0);
          //            stpNoteSuper.Children.Add(tbhere);
          //        }

          //        StackPanel stpnote2 = new StackPanel();
          //        stpnote2.Orientation = Orientation.Horizontal;
          //        stpnote2.HorizontalAlignment = HorizontalAlignment.Center;

          //        string rowitem = attritem.Name.Replace("NOTECC_", "");
          //        Image imgbtn = new Image();
          //        imgbtn.Name = "imgNOTECC_" + itemdata + "_" + rowitem + "_AddNota";
          //        imgbtn.ToolTip = "Aggiungi Nota";
          //        imgbtn.Margin = new Thickness(0, 5, 0, 0);
          //        imgbtn.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          //        imgbtn.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          //        var uriSource = new Uri(add, UriKind.Relative);
          //        imgbtn.Source = new BitmapImage(uriSource);
          //        imgbtn.Height = 10.0;
          //        imgbtn.Width = 10.0;
          //        imgbtn.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

          //        if (this.FindName(imgbtn.Name) != null)
          //        {
          //            this.UnregisterName(imgbtn.Name);
          //        }
          //        this.RegisterName(imgbtn.Name, imgbtn);

          //        stpnote2.Children.Add(imgbtn);

          //        stpNoteSuper.Children.Add(stpnote2);
          //    }
          //}

          grd.Children.Add(stpnote);
          Grid.SetRow(stpnote, rowhere);
          Grid.SetColumn(stpnote, 13);//14

          bool arebothempty = true;

          Image img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_ET";
          img.ToolTip = "INFERIORE A ERRORE TRASCURABILE";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;



          if (
                  (Materialità_1 == true && txt9.Text != "" && Math.Abs(Convert.ToDouble(txt9.Text)) > Math.Abs(Convert.ToDouble(tmpNode["EA"].ToString())))
                  ||
                  (Materialità_2 == true && Titolo.Contains("PATRIMONIALE") && txt9_2sp.Text != "" && Math.Abs(Convert.ToDouble(txt9_2sp.Text)) > Math.Abs(Convert.ToDouble(tmpNode["EA"].ToString())))
                  ||
                  (Materialità_2 == true && Titolo.Contains("ECONOMIC") && txt9_2ce.Text != "" && Math.Abs(Convert.ToDouble(txt9_2ce.Text)) > Math.Abs(Convert.ToDouble(tmpNode["EA"].ToString())))
                  ||
                  (Materialità_3 == true && Titolo.Contains("PATRIMONIALE") && txt9_3sp.Text != "" && Math.Abs(Convert.ToDouble(txt9_3sp.Text)) > Math.Abs(Convert.ToDouble(tmpNode["EA"].ToString())))
                  ||
                  (Materialità_3 == true && Titolo.Contains("ECONOMIC") && txt9_3ce.Text != "" && Math.Abs(Convert.ToDouble(txt9_3ce.Text)) > Math.Abs(Convert.ToDouble(tmpNode["EA"].ToString())))
              )
          {
            tmpNode["ET"] = "True";
            var uriSourceint = new Uri(check, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);

            arebothempty = false;
          }
          else
          {
            tmpNode["ET"] = "False";
            var uriSourceint = new Uri(uncheck, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);
          }

          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 3);

          img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_MO";
          img.ToolTip = "INFERIORE A MATERIALITA' OPERATIVA";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;


          if (
                  (Materialità_1 == true && txt12.Text != "" && Math.Abs(Convert.ToDouble(txt12.Text)) > Math.Abs(Convert.ToDouble(tmpNode["EA"].ToString())))
                  ||
                  (Materialità_2 == true && Titolo.Contains("PATRIMONIALE") && txt12_2sp.Text != "" && Math.Abs(Convert.ToDouble(txt12_2sp.Text)) > Math.Abs(Convert.ToDouble(tmpNode["EA"].ToString())))
                  ||
                  (Materialità_2 == true && Titolo.Contains("ECONOMIC") && txt12_2ce.Text != "" && Math.Abs(Convert.ToDouble(txt12_2ce.Text)) > Math.Abs(Convert.ToDouble(tmpNode["EA"].ToString())))
                  ||
                  (Materialità_3 == true && Titolo.Contains("PATRIMONIALE") && txt12_3sp.Text != "" && Math.Abs(Convert.ToDouble(txt12_3sp.Text)) > Math.Abs(Convert.ToDouble(tmpNode["EA"].ToString())))
                  ||
                  (Materialità_3 == true && Titolo.Contains("ECONOMIC") && txt12_3ce.Text != "" && Math.Abs(Convert.ToDouble(txt12_3ce.Text)) > Math.Abs(Convert.ToDouble(tmpNode["EA"].ToString())))
              )
          {
            tmpNode["MO"] = "True";
            var uriSourceint = new Uri(check, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);

            arebothempty = false;
          }
          else
          {
            tmpNode["MO"] = "False";
            var uriSourceint = new Uri(uncheck, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);
          }

          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 4);

          ComboBox newCombo = new ComboBox();
          newCombo.Name = "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_CONTROLLO";
          newCombo.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
          newCombo.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          newCombo.Margin = new Thickness(10, 0, 0, 0);
          newCombo.ToolTip = "CONTROLLO";
          newCombo.Height = 20.0;
          newCombo.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          newCombo.VerticalAlignment = System.Windows.VerticalAlignment.Top;
          newCombo.Width = 50.0;

          ComboBoxItem newitem = new ComboBoxItem();
          newitem.Content = "SI";
          newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "NO";
          newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "?";
          newCombo.Items.Add(newitem);
          newCombo.HorizontalContentAlignment = HorizontalAlignment.Center;

          if (tmpNode["CONTROLLO"].ToString() == "")
          {
            if (arebothempty)
            {
              tmpNode["CONTROLLO"] = "True";
            }
            else
            {
              tmpNode["CONTROLLO"] = "?";
            }
          }

          if (tmpNode != null && tmpNode["CONTROLLO"].ToString() != "")
          {
            if (tmpNode["CONTROLLO"].ToString() == "True")
            {
              newCombo.SelectedIndex = 0;
            }
            else if (tmpNode["CONTROLLO"].ToString() == "False")
            {
              newCombo.SelectedIndex = 1;
            }
            else
            {
              newCombo.SelectedIndex = 2;
            }
          }
          else
          {
            newCombo.SelectedIndex = 2;
          }

          newCombo.SelectionChanged += new SelectionChangedEventHandler(cmbControllo_Changed);

          //img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
          //img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
          //img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);

          grd.Children.Add(newCombo);
          Grid.SetRow(newCombo, rowhere);
          Grid.SetColumn(newCombo, 5);

          img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_EsameFisico";
          img.Margin = new Thickness(20, 0, 0, 0);
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          if (item.Value.ToString().Split('@')[2] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);


            tmpNode["EsameFisico"] = "X";
          }
          else
          {
            img.ToolTip = "ISPEZIONE";

            if (tmpNode != null && tmpNode["EsameFisico"].ToString() != "" && tmpNode["EsameFisico"].ToString() == "True")
            {
              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }

          this.RegisterName(img.Name, img);

          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 6);

          img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_Ispezione";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          if (item.Value.ToString().Split('@')[3] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);


            tmpNode["Ispezione"] = "X";
          }
          else
          {
            img.ToolTip = "OSSERVAZIONE";
            if (tmpNode != null && tmpNode["Ispezione"].ToString() != null && tmpNode["Ispezione"].ToString() == "True")
            {
              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }

          this.RegisterName(img.Name, img);

          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 7);

          img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_Indagine";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          if (item.Value.ToString().Split('@')[4] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);



            tmpNode["Indagine"] = "X";
          }
          else
          {
            img.ToolTip = "CONFERMA ESTERNA";
            if (tmpNode != null && tmpNode["Indagine"].ToString() != null && tmpNode["Indagine"].ToString() == "True")
            {
              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }

          this.RegisterName(img.Name, img);

          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 8);

          img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_Osservazione";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          if (item.Value.ToString().Split('@')[5] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);



            tmpNode["Osservazione"] = "X";
          }
          else
          {
            img.ToolTip = "RICALCOLO";
            if (tmpNode != null && tmpNode["Osservazione"].ToString() != "" && tmpNode["Osservazione"].ToString() == "True")
            {
              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }

          this.RegisterName(img.Name, img);

          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 9);

          img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_Ricalcolo";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          if (item.Value.ToString().Split('@')[6] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);

            tmpNode["Ricalcolo"] = "X";
          }
          else
          {
            img.ToolTip = "RIESECUZIONE";
            if (tmpNode != null && tmpNode["Ricalcolo"].ToString() != null && tmpNode["Ricalcolo"].ToString() == "True")
            {
              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }
          this.RegisterName(img.Name, img);

          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 10);

          img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_Riesecuzione";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          if (item.Value.ToString().Split('@')[7] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);


            tmpNode["Riesecuzione"] = "X";
          }
          else
          {
            img.ToolTip = "PROCEDURE DI ANALISI COMPARATIVA";
            if (tmpNode != null && tmpNode["Riesecuzione"].ToString() != "" && tmpNode["Riesecuzione"].ToString() == "True")
            {
              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }

          this.RegisterName(img.Name, img);

          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 11);

          img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_Conferma";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          if (item.Value.ToString().Split('@')[8] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);


            tmpNode["Conferma"] = "X";
          }
          else
          {
            img.ToolTip = "INDAGINE";
            if (tmpNode != null && tmpNode["Conferma"].ToString() != "" && tmpNode["Conferma"].ToString() == "True")
            {
              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }

          this.RegisterName(img.Name, img);

          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 12);

          //img = new Image();
          //img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_" + item.Key.ToString() + "_Comparazioni";
          //img.Height = 20.0;
          //img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          //img.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          //if (item.Value.ToString().Split('@').Length > 9 && item.Value.ToString().Split('@')[9] == "0")
          //{
          //    var uriSourceint = new Uri(disabled, UriKind.Relative);
          //    img.Source = new BitmapImage(uriSourceint);

          //    if (tmpNode.Attributes["Comparazioni"] == null)
          //    {
          //        XmlAttribute attr = xnode.OwnerDocument.CreateAttribute("Comparazioni");
          //        tmpNode.Attributes.Append(attr);
          //    }

          //    tmpNode.Attributes["Comparazioni"].Value = "X";
          //}
          //else
          //{
          //    if (tmpNode != null && tmpNode.Attributes["Comparazioni"] != null && tmpNode.Attributes["Comparazioni"].Value == "True")
          //    {
          //        var uriSourceint = new Uri(check, UriKind.Relative);
          //        img.Source = new BitmapImage(uriSourceint);
          //    }
          //    else
          //    {
          //        var uriSourceint = new Uri(uncheck, UriKind.Relative);
          //        img.Source = new BitmapImage(uriSourceint);
          //    }

          //    img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
          //    img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
          //    img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          //}

          //this.RegisterName(img.Name, img);

          //grd.Children.Add(img);
          //Grid.SetRow(img, rowhere);
          //Grid.SetColumn(img, 13);
        }

        checkexistance = true;
      }

      return checkexistance;
    }
    private bool GenerateGrid(KeyValuePair<int, string> item, Grid grd, string Codice, ref int rowhere, string Titolo)
    {
      bool areBothEmpty, bold, checkexistance, checkMat1, checkMat2, checkMat3, hasdata, total, trovato;
      double dbl1, dbl2, testd, val1, valtot;
      int indexhere, initialrowhere;
      string rowitem, str;

      total = false;
      checkexistance = false;
      initialrowhere = rowhere + 1;
      foreach (string itemdata in b_Ordine)
      {
        DataRow tmpNode = null;
        foreach (DataRow dd in datiV.Rows)
        {
          if (dd["ID"].ToString() == itemdata && dd["Codice"].ToString() == Codice)
            tmpNode = dd;
        }
        hasdata = false;
        if (b_NoData.Contains(itemdata)
          && (bool)(b_NoData[itemdata]) == true)
        {
          foreach (XmlNode child in
            _y.Document.SelectNodes(
              "/LEADS/LEAD[@ID='" + Codice + "']/RIGA[@PADRE='" + itemdata + "']"))
          {
            if (b_NoData.Contains(child.Attributes["ID"].Value)
              && (bool)(b_NoData[child.Attributes["ID"].Value]) == true)
            {
              foreach (XmlNode grandchild in
                _y.Document.SelectNodes(
                  "/LEADS/LEAD[@ID='" + Codice + "']/RIGA[@PADRE='"
                  + child.Attributes["ID"].Value + "']"))
              {
                if (b_NoData.Contains(grandchild.Attributes["ID"].Value)
                  && (bool)(b_NoData[grandchild.Attributes["ID"].Value]) == true)
                {
                  foreach (XmlNode grandgrandchild in
                    _y.Document.SelectNodes(
                      "/LEADS/LEAD[@ID='" + Codice + "']/RIGA[@PADRE='"
                      + grandchild.Attributes["ID"].Value + "']"))
                  {
                    if (b_NoData.Contains(grandgrandchild.Attributes["ID"].Value)
                      && (bool)(b_NoData[grandgrandchild.Attributes["ID"].Value]) == true)
                    {
                      ; //------------------------------------------------- ???
                    }
                    else
                    {
                      foreach (DataRow dd in datiV.Rows)
                      {
                        if (dd["ID"].ToString() == grandgrandchild.Attributes["ID"].Value
                          && dd["Codice"].ToString() == Codice)
                        {
                          hasdata = true;
                          total = true;
                        }
                      }
                    }
                  }
                }
                else
                {
                  foreach (DataRow dd in datiV.Rows)
                  {
                    if (dd["ID"].ToString() == grandchild.Attributes["ID"].Value
                      && dd["Codice"].ToString() == Codice)
                    {
                      hasdata = true;
                      total = true;
                    }
                  }
                }
              }
            }
            else
            {
              foreach (DataRow dd in datiV.Rows)
              {
                if (dd["ID"].ToString() == child.Attributes["ID"].Value
                  && dd["Codice"].ToString() == Codice)
                {
                  hasdata = true;
                  total = true;
                }
              }
            }
          }
        }
        else
        {
          if (tmpNode != null
            && ((tmpNode["Tipo"].ToString() == ""
              || tmpNode["Tipo"].ToString() == "Ulteriori dati opzionali (acquisizione non automatica)")
            || (((tmpNode["EA"].ToString() != "" && tmpNode["EA"].ToString() != "0")))))
          {
            hasdata = true;
            total = true;
          }
        }
        if (hasdata == false || (b_Titolo[itemdata].ToString() == "Totale"
          && total == false))
        {
          continue;
        }
        if (b_Titolo[itemdata].ToString() == "Totale" && total == true)
        {
          total = false;
        }
        if ((bool)(b_NoData[itemdata]) == true)
        {
          //-------------------------------------------------------- riga vuota
          rowhere++;
          RowDefinition rd2 = new RowDefinition();
          rd2.Height = new GridLength(10);
          grd.RowDefinitions.Add(rd2);
        }
        RowDefinition rd = new RowDefinition();
        rd.Height = GridLength.Auto;
        grd.RowDefinitions.Add(rd);

        rowhere++;
        bold = false;

        StackPanel stkp = new StackPanel();
        stkp.Margin = new Thickness(0, 0, 0, 5);
        stkp.Orientation = Orientation.Vertical;
        stkp.Name = "_" + _ID + "_" + itemdata + "_" + rowhere.ToString()
          + "_" + item.Key.ToString() + "_STKName";
        this.RegisterName(stkp.Name, stkp);

        TextBlock lbl = new TextBlock();
        lbl.Height = 20.0;
        lbl.VerticalAlignment = VerticalAlignment.Center;

        if (b_Titolo[itemdata].ToString() == "Totale")
        {
          Image imgbtn = new Image();
          imgbtn.Name = "_" + _ID + "_" + itemdata + "_"
            + initialrowhere.ToString() + "_" + rowhere.ToString() + "_Btn";
          imgbtn.ToolTip = "Espandi";
          imgbtn.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          imgbtn.VerticalAlignment = System.Windows.VerticalAlignment.Center;

          var uriSource = new Uri(left, UriKind.Relative);
          for (int i = initialrowhere; i < rowhere; i++)
          {
            grd.RowDefinitions[i].Height = GridLength.Auto;
          }

          uriSource = new Uri(up, UriKind.Relative);

          imgbtn.Source = new BitmapImage(uriSource);
          imgbtn.Height = 10.0;
          imgbtn.Width = 10.0;
          imgbtn.MouseLeftButtonDown +=
            new MouseButtonEventHandler(Image_MouseLeftButtonDown);

          grd.Children.Add(imgbtn);
          Grid.SetRow(imgbtn, rowhere);
          Grid.SetColumn(imgbtn, 0);

          lbl.Text = Titolo;
          bold = true;
        }
        else
        {
          if ((bool)(b_NoData[itemdata]) == false && itemdata != "11611"
            && itemdata != "120")
          {
            Image imgbtn = new Image();
            imgbtn.Name = "_" + _ID + "_" + itemdata + "_" + rowhere.ToString()
              + "_" + item.Key.ToString() + "_Add";
            imgbtn.Margin = new Thickness(0, 5, 0, 0);
            imgbtn.ToolTip = "Aggiungi Conto Contabile";
            imgbtn.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            imgbtn.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            var uriSource = new Uri(add, UriKind.Relative);
            imgbtn.Source = new BitmapImage(uriSource);
            imgbtn.Height = 10.0;
            imgbtn.Width = 10.0;
            imgbtn.MouseLeftButtonDown +=
              new MouseButtonEventHandler(Image_MouseLeftButtonDown);

            grd.Children.Add(imgbtn);
            Grid.SetRow(imgbtn, rowhere);
            Grid.SetColumn(imgbtn, 0);
          }
          lbl.Text = b_Titolo[itemdata].ToString();
        }
        if (lbl.Text == "Sub Totale") bold = true;
        if (bold) lbl.FontWeight = FontWeights.Bold;

        lbl.TextWrapping = TextWrapping.Wrap;
        lbl.HorizontalAlignment = HorizontalAlignment.Left;
        if ((bool)(b_NoData[itemdata]) == true)
        {
          lbl.FontWeight = FontWeights.Bold;
        }

        stkp.Children.Add(lbl);

        foreach (DataRow attritem in datiVRighe.Rows)
        {
          if (attritem["ID"].ToString() != itemdata
            || attritem["Codice"].ToString() != Codice)
            continue;

          if (stkp.Children.Count == 1)
          {
            StackPanel nstr = new StackPanel();
            nstr.Orientation = Orientation.Horizontal;

            TextBlock tbhere = new TextBlock();
            tbhere.Text = "Codice";
            tbhere.Width = 130.0;
            tbhere.FontWeight = FontWeights.Bold;
            tbhere.Margin = new Thickness(20, 5, 0, 0);
            nstr.Children.Add(tbhere);

            tbhere = new TextBlock();
            tbhere.Text = "Denominazione Conto Contabile";
            tbhere.FontWeight = FontWeights.Bold;
            tbhere.Margin = new Thickness(0, 5, 0, 0);
            nstr.Children.Add(tbhere);

            stkp.Children.Add(nstr);
          }

          StackPanel stkpp = new StackPanel();
          stkpp.Orientation = Orientation.Horizontal;
          stkpp.HorizontalAlignment = HorizontalAlignment.Stretch;

          rowitem = attritem["row"].ToString();

          Image imgbtn = new Image();
          imgbtn.Name = "_" + _ID + "_" + itemdata + "_" + rowhere.ToString()
            + "_" + item.Key.ToString() + "_" + rowitem + "_Remove";
          imgbtn.ToolTip = "Rimuovi Conto Contabile";
          imgbtn.Margin = new Thickness(0, 5, 0, 0);
          imgbtn.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          imgbtn.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          var uriSource = new Uri(remove, UriKind.Relative);
          imgbtn.Source = new BitmapImage(uriSource);
          imgbtn.Height = 10.0;
          imgbtn.Width = 10.0;
          imgbtn.MouseLeftButtonDown +=
            new MouseButtonEventHandler(Image_MouseLeftButtonDown);

          stkpp.Children.Add(imgbtn);

          StackPanel nstr2 = new StackPanel();
          nstr2.Orientation = Orientation.Horizontal;
          nstr2.Name = "StackPanel_" + itemdata + "_" + rowitem;

          TextBox txt = new TextBox();
          txt.Name = "txtCodice_" + itemdata + "_" + rowitem;
          txt.Background = Brushes.LightGoldenrodYellow;
          txt.Width = 130.0;
          txt.Height = 20.0;
          txt.VerticalAlignment = VerticalAlignment.Center;


          txt.Text = attritem["Codice"].ToString();

          txt.TextAlignment = TextAlignment.Left;
          txt.PreviewMouseLeftButtonDown +=
            new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
          txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          txt.LostFocus += new RoutedEventHandler(txtCodice_LostFocus);

          nstr2.Children.Add(txt);

          txt = new TextBox();
          txt.Name = "txtTitolo_" + itemdata + "_" + rowitem;
          txt.Background = Brushes.LightGoldenrodYellow;

          if (this.FindName(txt.Name) != null)
          {
            this.UnregisterName(txt.Name);
          }
          this.RegisterName(txt.Name, txt);

          txt.Height = 20.0;
          txt.VerticalAlignment = VerticalAlignment.Center;


          txt.Text = attritem["Titolo"].ToString();
          txt.ToolTip = txt.Text;
          txt.TextAlignment = TextAlignment.Left;
          txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
          txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          txt.LostFocus += new RoutedEventHandler(txtTitolo_LostFocus);
          txt.HorizontalAlignment = HorizontalAlignment.Left;

          nstr2.Children.Add(txt);
          stkpp.Children.Add(nstr2);
          stkp.Children.Add(stkpp);
        }

        grd.Children.Add(stkp);
        Grid.SetRow(stkp, rowhere);
        Grid.SetColumn(stkp, 1);

        if ((bool)(b_NoData[itemdata]) == false)
        {
          stkp = new StackPanel();
          stkp.Margin = new Thickness(0, 0, 0, 5);
          stkp.Orientation = Orientation.Vertical;
          stkp.Name = "_" + _ID + "_" + itemdata + "_" + rowhere.ToString()
            + "_" + item.Key.ToString() + "_STKEA";
          this.RegisterName(stkp.Name, stkp);

          TextBox txt = new TextBox();
          if (bold == true)
          {
            txt.FontWeight = FontWeights.Bold;
          }
          txt.Height = 20.0;
          txt.VerticalAlignment = VerticalAlignment.Center;
          if (tmpNode != null && tmpNode["EA"].ToString() != "")
          {
            txt.Text = ConvertNumber(tmpNode["EA"].ToString());
          }
          txt.IsReadOnly = true;
          txt.TextAlignment = TextAlignment.Right;
          stkp.Children.Add(txt);
          foreach (DataRow attritem in datiVRighe.Rows)
          {
            if (attritem["ID"].ToString() != itemdata
              || attritem["Codice"].ToString() != Codice)
              continue;
            if (stkp.Children.Count == 1)
            {
              TextBlock tbhere = new TextBlock();
              tbhere.Name = "txtEA_" + itemdata + "_TOT";
              tbhere.Background = Brushes.LightGreen;
              tbhere.Text = ""; //----------------------------- valore bilancio
              tbhere.ToolTip = "Squadratura";
              tbhere.VerticalAlignment = VerticalAlignment.Center;
              tbhere.TextAlignment = TextAlignment.Right;
              tbhere.Margin = new Thickness(0, 5, 0, 0);
              stkp.Children.Add(tbhere);

              if (this.FindName(tbhere.Name) != null)
              {
                this.UnregisterName(tbhere.Name);
              }
              this.RegisterName(tbhere.Name, tbhere);
            }

            rowitem = attritem["row"].ToString();
            txt = new TextBox();
            txt.Name = "txtEA_" + itemdata + "_" + rowitem;
            txt.Background = Brushes.LightGoldenrodYellow;
            txt.Height = 20.0;
            txt.VerticalAlignment = VerticalAlignment.Center;
            txt.Text = ConvertNumber(tmpNode["EA"].ToString());
            txt.TextAlignment = TextAlignment.Right;
            txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
            txt.LostFocus += new RoutedEventHandler(txtEA_LostFocus);

            stkp.Children.Add(txt);

            val1 = 0.0;
            valtot = 0.0;
            testd = 0.0;
            indexhere = 0;
            foreach (UIElement itemstkp in stkp.Children)
            {
              if (itemstkp.GetType().Name == "TextBox")
              {
                if (indexhere == 0)
                {
                  if (Double.TryParse(((TextBox)itemstkp).Text, out testd))
                  {
                    val1 = Convert.ToSingle(((TextBox)itemstkp).Text);
                  }
                }
                else
                {
                  if (Double.TryParse(((TextBox)itemstkp).Text, out testd))
                  {
                    valtot += Convert.ToSingle(((TextBox)itemstkp).Text);
                  }
                }
                indexhere++;
              }
            }
            if (this.FindName("txtEA_" + itemdata + "_TOT") != null)
            {
              TextBlock stphere =
                ((TextBlock)(this.FindName("txtEA_" + itemdata + "_TOT")));
              stphere.Text = ConvertNumber((val1 - valtot).ToString());
            }
          }
          grd.Children.Add(stkp);
          Grid.SetRow(stkp, rowhere);
          Grid.SetColumn(stkp, 2);

          StackPanel stpnote = new StackPanel();
          stpnote.Orientation = Orientation.Horizontal;
          stpnote.HorizontalAlignment = HorizontalAlignment.Center;
          trovato = false;
          if (tmpNode != null)
          {
            foreach (DataRow dd in datiV.Rows)
            {
              if (dd["ID"].ToString() == tmpNode["ID"].ToString()
                && dd["NoteNumber"].ToString() != ""
                && dd["NoteRealRow"].ToString() != "")
              {
                trovato = true;
                TextBlock txtnote = new TextBlock();
                txtnote.Name = "txtNoteNumber_" + itemdata + "_"
                  + rowhere.ToString() + "_" + item.Key.ToString();
                txtnote.FontWeight = FontWeights.Bold;
                txtnote.Margin = new Thickness(0, 0, 5, 0);
                txtnote.Text = dd["NoteNumber"].ToString();
                if (this.FindName(txtnote.Name) != null)
                {
                  this.UnregisterName(txtnote.Name);
                }
                this.RegisterName(txtnote.Name, txtnote);
                stpnote.Children.Add(txtnote);

                Image imgbtn = new Image();
                imgbtn.Name = "_" + itemdata + "_" + rowhere.ToString() + "_"
                  + item.Key.ToString() + "_RemoveNota";
                imgbtn.ToolTip = "Rimuovi Nota";
                imgbtn.Margin = new Thickness(0, 5, 0, 0);
                imgbtn.HorizontalAlignment =
                  System.Windows.HorizontalAlignment.Center;
                imgbtn.VerticalAlignment =
                  System.Windows.VerticalAlignment.Top;

                var uriSource = new Uri(remove, UriKind.Relative);
                imgbtn.Source = new BitmapImage(uriSource);
                imgbtn.Height = 10.0;
                imgbtn.Width = 10.0;
                imgbtn.MouseLeftButtonDown +=
                  new MouseButtonEventHandler(Image_MouseLeftButtonDown);

                if (this.FindName(imgbtn.Name) != null)
                {
                  this.UnregisterName(imgbtn.Name);
                }
                this.RegisterName(imgbtn.Name, imgbtn);

                stpnote.Children.Add(imgbtn);
              }
            }
          }
          if (!trovato)
          {
            Image imgbtn = new Image();
            imgbtn.Name = "_" + itemdata + "_" + rowhere.ToString() + "_"
              + item.Key.ToString() + "_AddNota";
            imgbtn.ToolTip = "Aggiungi Nota";
            imgbtn.Margin = new Thickness(0, 5, 0, 0);
            imgbtn.HorizontalAlignment =
              System.Windows.HorizontalAlignment.Center;
            imgbtn.VerticalAlignment =
              System.Windows.VerticalAlignment.Top;

            var uriSource = new Uri(add, UriKind.Relative);
            imgbtn.Source = new BitmapImage(uriSource);
            imgbtn.Height = 10.0;
            imgbtn.Width = 10.0;
            imgbtn.MouseLeftButtonDown +=
              new MouseButtonEventHandler(Image_MouseLeftButtonDown);

            if (this.FindName(imgbtn.Name) != null)
            {
              this.UnregisterName(imgbtn.Name);
            }
            this.RegisterName(imgbtn.Name, imgbtn);
            stpnote.Children.Add(imgbtn);
          }

          grd.Children.Add(stpnote);
          Grid.SetRow(stpnote, rowhere);
          Grid.SetColumn(stpnote, 13);//14

          areBothEmpty = true;

          Image img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_"
            + item.Key.ToString() + "_ET";
          img.ToolTip = "INFERIORE A ERRORE TRASCURABILE";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;
          //----------------------------------------- condizioni per spunta blu
          dbl2 = Math.Abs(Convert.ToDouble(tmpNode["EA"].ToString()));

          //------------------------------------------------------------------+
          //       Materialità_1: valore in base alla tabella visibile        |
          //         brdPrima: txt9, brdSeconda: txt9_2sp o txt9_2ce          |
          //------------------------------------------------------------------+
          checkMat1 = false;
          if (Materialità_1)
          {
            str = brdPrima.Visibility == Visibility.Visible ?
              txt9.Text : Titolo.Contains("ECONOMICO") ?
                txt9_2ce.Text : txt9_2sp.Text;
            if (!string.IsNullOrEmpty(str))
            {
              dbl1 = Math.Abs(Convert.ToDouble(str));
              checkMat1 = dbl1 > dbl2;
            }
          }

          //------------------------------------------------------------------+
          //   Materialità_2: sembra inesistente, ma usa sempre brdSeconda    |
          //        PATRIMONIALE: txt9_2sp, CONTO ECONOMICO: txt9_2ce         |
          //------------------------------------------------------------------+
          checkMat2 = false;
          if (Materialità_2)
          {
            str = string.Empty;
            if (Titolo.Contains("PATRIMONIALE")) str = txt9_2sp.Text;
            if (Titolo.Contains("ECONOMIC")) str = txt9_2ce.Text;
            if (!string.IsNullOrEmpty(str))
            {
              dbl1 = Math.Abs(Convert.ToDouble(str));
              checkMat2 = dbl1 > dbl2;
            }
          }

          //------------------------------------------------------------------+
          //       Materialità_3: valore in base alla tabella visibile        |
          //          brdPrima: txt9, brdTerza: txt9_3sp o txt9_3ce           |
          //------------------------------------------------------------------+
          checkMat3 = false;
          if (Materialità_3)
          {
            str = brdPrima.Visibility == Visibility.Visible ?
              txt9.Text : Titolo.Contains("ECONOMICO") ?
                txt9_3ce.Text : txt9_3sp.Text;
            if (!string.IsNullOrEmpty(str))
            {
              dbl1 = Math.Abs(Convert.ToDouble(str));
              checkMat3 = dbl1 > dbl2;
            }
          }

          if (checkMat1 || checkMat2 || checkMat3)
          {
            tmpNode["ET"] = "True";
            var uriSourceint = new Uri(check, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);
            areBothEmpty = false;
          }
          else
          {
            tmpNode["ET"] = "False";
            var uriSourceint = new Uri(uncheck, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);
          }

          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 3);

          img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_"
            + item.Key.ToString() + "_MO";
          img.ToolTip = "INFERIORE A MATERIALITA' OPERATIVA";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;
          //----------------------------------------- condizioni per spunta blu
          dbl2 = Math.Abs(Convert.ToDouble(tmpNode["EA"].ToString()));

          //------------------------------------------------------------------+
          //       Materialità_1: valore in base alla tabella visibile        |
          //        brdPrima: txt12, brdSeconda: txt12_2sp o txt12_2ce        |
          //------------------------------------------------------------------+
          checkMat1 = false;
          if (Materialità_1)
          {
            str = brdPrima.Visibility == Visibility.Visible ?
              txt12.Text : Titolo.Contains("ECONOMICO") ?
                txt12_2ce.Text : txt12_2sp.Text;
            if (!string.IsNullOrEmpty(str))
            {
              dbl1 = Math.Abs(Convert.ToDouble(str));
              checkMat1 = dbl1 > dbl2;
            }
          }

          //------------------------------------------------------------------+
          //   Materialità_2: sembra inesistente, ma usa sempre brdSeconda    |
          //       PATRIMONIALE: txt12_2sp, CONTO ECONOMICO: txt12_2ce        |
          //------------------------------------------------------------------+
          checkMat2 = false;
          if (Materialità_2)
          {
            str = string.Empty;
            if (Titolo.Contains("PATRIMONIALE")) str = txt12_2sp.Text;
            if (Titolo.Contains("ECONOMIC")) str = txt12_2ce.Text;
            if (!string.IsNullOrEmpty(str))
            {
              dbl1 = Math.Abs(Convert.ToDouble(str));
              checkMat2 = dbl1 > dbl2;
            }
          }

          //------------------------------------------------------------------+
          //       Materialità_3: valore in base alla tabella visibile        |
          //         brdPrima: txt12, brdTerza: txt12_3sp o txt12_3ce         |
          //------------------------------------------------------------------+
          checkMat3 = false;
          if (Materialità_3)
          {
            str = brdPrima.Visibility == Visibility.Visible ?
              txt12_3sp.Text : Titolo.Contains("ECONOMICO") ?
                txt12_3ce.Text : txt12_3sp.Text;
            if (!string.IsNullOrEmpty(str))
            {
              dbl1 = Math.Abs(Convert.ToDouble(str));
              checkMat3 = dbl1 > dbl2;
            }
          }

          if (checkMat1 || checkMat2 || checkMat3)
          {
            tmpNode["MO"] = "True";
            var uriSourceint = new Uri(check, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);
            areBothEmpty = false;
          }
          else
          {
            tmpNode["MO"] = "False";
            var uriSourceint = new Uri(uncheck, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);
          }

          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 4);

          ComboBox newCombo = new ComboBox();
          newCombo.Name = "_" + itemdata + "_" + rowhere.ToString() + "_"
            + item.Key.ToString() + "_CONTROLLO";
          newCombo.PreviewMouseLeftButtonDown +=
            new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
          newCombo.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          newCombo.Margin = new Thickness(10, 0, 0, 0);
          newCombo.ToolTip = "CONTROLLO";
          newCombo.Height = 20.0;
          newCombo.HorizontalAlignment =
            System.Windows.HorizontalAlignment.Center;
          newCombo.VerticalAlignment =
            System.Windows.VerticalAlignment.Top;
          newCombo.Width = 50.0;

          ComboBoxItem newitem = new ComboBoxItem();
          newitem.Content = "SI";
          newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "NO";
          newCombo.Items.Add(newitem);
          newitem = new ComboBoxItem();
          newitem.Content = "?";
          newCombo.Items.Add(newitem);
          newCombo.HorizontalContentAlignment =
            HorizontalAlignment.Center;

          if (tmpNode["CONTROLLO"].ToString() == "")
          {
            if (areBothEmpty)
            {
              tmpNode["CONTROLLO"] = "True";
            }
            else
            {
              tmpNode["CONTROLLO"] = "?";
            }
          }

          if (tmpNode != null && tmpNode["CONTROLLO"].ToString() != "")
          {
            if (tmpNode["CONTROLLO"].ToString() == "True")
            {
              newCombo.SelectedIndex = 0;
            }
            else if (tmpNode["CONTROLLO"].ToString() == "False")
            {
              newCombo.SelectedIndex = 1;
            }
            else
            {
              newCombo.SelectedIndex = 2;
            }
          }
          else
          {
            newCombo.SelectedIndex = 2;
          }

          newCombo.SelectionChanged +=
            new SelectionChangedEventHandler(cmbControllo_Changed);

          grd.Children.Add(newCombo);
          Grid.SetRow(newCombo, rowhere);
          Grid.SetColumn(newCombo, 5);

          img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_"
            + item.Key.ToString() + "_EsameFisico";
          img.Margin = new Thickness(20, 0, 0, 0);
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          if (item.Value.ToString().Split('@')[2] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);
            tmpNode["EsameFisico"] = "X";
          }
          else
          {
            img.ToolTip = "ISPEZIONE";
            if (tmpNode != null && tmpNode["EsameFisico"].ToString() != ""
              && tmpNode["EsameFisico"].ToString() == "True")
            {
              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown +=
              new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown +=
              new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }
          this.RegisterName(img.Name, img);
          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 6);

          img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_"
            + item.Key.ToString() + "_Ispezione";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          if (item.Value.ToString().Split('@')[3] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);
            tmpNode["Ispezione"] = "X";
          }
          else
          {
            img.ToolTip = "OSSERVAZIONE";
            if (tmpNode != null && tmpNode["Ispezione"].ToString() != null
              && tmpNode["Ispezione"].ToString() == "True")
            {
              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown +=
              new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown +=
              new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }
          this.RegisterName(img.Name, img);

          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 7);

          img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_"
            + item.Key.ToString() + "_Indagine";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          if (item.Value.ToString().Split('@')[4] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);
            tmpNode["Indagine"] = "X";
          }
          else
          {
            img.ToolTip = "CONFERMA ESTERNA";
            if (tmpNode != null && tmpNode["Indagine"].ToString() != null
              && tmpNode["Indagine"].ToString() == "True")
            {
              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown +=
              new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown +=
              new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }
          this.RegisterName(img.Name, img);
          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 8);

          img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_"
            + item.Key.ToString() + "_Osservazione";
          img.Height = 20.0;
          img.HorizontalAlignment =
            System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          if (item.Value.ToString().Split('@')[5] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);
            tmpNode["Osservazione"] = "X";
          }
          else
          {
            img.ToolTip = "RICALCOLO";
            if (tmpNode != null && tmpNode["Osservazione"].ToString() != ""
              && tmpNode["Osservazione"].ToString() == "True")
            {
              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown +=
              new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown +=
              new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }
          this.RegisterName(img.Name, img);
          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 9);

          img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_"
            + item.Key.ToString() + "_Ricalcolo";
          img.Height = 20.0;
          img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          if (item.Value.ToString().Split('@')[6] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);
            tmpNode["Ricalcolo"] = "X";
          }
          else
          {
            img.ToolTip = "RIESECUZIONE";
            if (tmpNode != null && tmpNode["Ricalcolo"].ToString() != null
              && tmpNode["Ricalcolo"].ToString() == "True")
            {
              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown +=
              new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown +=
              new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }
          this.RegisterName(img.Name, img);
          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 10);

          img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_"
            + item.Key.ToString() + "_Riesecuzione";
          img.Height = 20.0;
          img.HorizontalAlignment =
            System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          if (item.Value.ToString().Split('@')[7] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);
            tmpNode["Riesecuzione"] = "X";
          }
          else
          {
            img.ToolTip = "PROCEDURE DI ANALISI COMPARATIVA";
            if (tmpNode != null && tmpNode["Riesecuzione"].ToString() != ""
              && tmpNode["Riesecuzione"].ToString() == "True")
            {
              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown +=
              new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown +=
              new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }
          this.RegisterName(img.Name, img);
          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 11);

          img = new Image();
          img.Name = "_" + itemdata + "_" + rowhere.ToString() + "_"
            + item.Key.ToString() + "_Conferma";
          img.Height = 20.0;
          img.HorizontalAlignment =
            System.Windows.HorizontalAlignment.Center;
          img.VerticalAlignment = System.Windows.VerticalAlignment.Top;

          if (item.Value.ToString().Split('@')[8] == "0")
          {
            var uriSourceint = new Uri(disabled, UriKind.Relative);
            img.Source = new BitmapImage(uriSourceint);
            tmpNode["Conferma"] = "X";
          }
          else
          {
            img.ToolTip = "INDAGINE";
            if (tmpNode != null && tmpNode["Conferma"].ToString() != ""
              && tmpNode["Conferma"].ToString() == "True")
            {
              var uriSourceint = new Uri(check, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }
            else
            {
              var uriSourceint = new Uri(uncheck, UriKind.Relative);
              img.Source = new BitmapImage(uriSourceint);
            }

            img.MouseLeftButtonDown +=
              new MouseButtonEventHandler(img_MouseLeftButtonDown);
            img.PreviewMouseLeftButtonDown +=
              new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
            img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
          }
          this.RegisterName(img.Name, img);
          grd.Children.Add(img);
          Grid.SetRow(img, rowhere);
          Grid.SetColumn(img, 12);
        }
        checkexistance = true;
      }
      return checkexistance;
    }

    private void cmbControllo_Changed(object sender, SelectionChangedEventArgs e)
    {
      string tipo = ((ComboBox)sender).Name.Split('_').Last();
      string name = ((ComboBox)sender).Name.Split('_')[1];
      string rowhere = ((ComboBox)sender).Name.Split('_')[2];
      string codice = ((ComboBox)sender).Name.Split('_')[3];

      e.Handled = true;

      //   XmlNode tmpnode = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]/Valore[@ID=\"" + name + "\"][@Codice=\"" + VociBilancio[codice] + "\"]");
      foreach (DataRow attritem in datiV.Rows)
      {
        if (attritem["ID"].ToString() != name || attritem["Codice"].ToString() != VociBilancio[codice].ToString())
          continue;

        string value = ((ComboBoxItem)((ComboBox)sender).SelectedItem).Content.ToString();

        if (value == "SI")
        {
          attritem[tipo] = "True";
        }
        else if (value == "NO")
        {
          attritem[tipo] = "False";
        }
        else
        {
          attritem[tipo] = "?";
        }
      }
      //somethinghaschanged = true;

      //((ComboBox)sender).SelectedItem = ((ComboBoxItem)((ComboBox)sender).Items[selecteditem]);
      //((ComboBox)sender).Text = ((ComboBoxItem)((ComboBox)sender).Items[selecteditem]).Content.ToString();
    }

    private void Txt_LostFocus(object sender, RoutedEventArgs e)
    {
      throw new NotImplementedException();
    }

    private bool CheckExistence(string Codice)
    {
      bool total = false;

      foreach (string item in b_Ordine)
      {
        //XmlNode tmpNode = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]/Valore[@ID=\"" + item + "\"][@Codice=\"" + Codice + "\"]");
        DataRow tmpNode = null;
        foreach (DataRow dd in datiV.Rows)
        {
          if (dd["ID"].ToString() == item && dd["Codice"].ToString() == Codice)
            tmpNode = dd;
        }


        //XmlNode tmpNodeChild = null;

        bool hasdata = false;

        if ((b_NoData.Contains(item) && (bool)(b_NoData[item]) == true) || !(item == "190" && Codice == "3.4.7"))
        {
          foreach (XmlNode child in _y.Document.SelectNodes("/LEADS/LEAD[@ID='" + Codice + "']/RIGA[@PADRE='" + item + "']"))
          {
            if (b_NoData.Contains(child.Attributes["ID"].Value) && (bool)(b_NoData[child.Attributes["ID"].Value]) == true)
            {
              foreach (XmlNode grandchild in _y.Document.SelectNodes("/LEADS/LEAD[@ID='" + Codice + "']/RIGA[@PADRE='" + child.Attributes["ID"].Value + "']"))
              {
                if (b_NoData.Contains(grandchild.Attributes["ID"].Value) && (bool)(b_NoData[grandchild.Attributes["ID"].Value]) == true)
                {
                  foreach (XmlNode grandgrandchild in _y.Document.SelectNodes("/LEADS/LEAD[@ID='" + Codice + "']/RIGA[@PADRE='" + grandchild.Attributes["ID"].Value + "']"))
                  {
                    if (b_NoData.Contains(grandgrandchild.Attributes["ID"].Value) && (bool)(b_NoData[grandgrandchild.Attributes["ID"].Value]) == true)
                    {
                      ;
                    }
                    else
                    {
                      foreach (DataRow dd in datiV.Rows)
                      {
                        if (dd["EA"].ToString() != "" && dd["EA"].ToString() != "0" && dd["ID"].ToString() == grandgrandchild.Attributes["ID"].Value && dd["Codice"].ToString() == Codice)
                        {

                          hasdata = true;
                          total = true;
                        }
                      }
                    }
                  }
                }
                else
                {
                  foreach (DataRow dd in datiV.Rows)
                  {
                    if (dd["EA"].ToString() != "" && dd["EA"].ToString() != "0" && dd["ID"].ToString() == grandchild.Attributes["ID"].Value && dd["Codice"].ToString() == Codice)
                    {

                      hasdata = true;
                      total = true;
                    }
                  }

                }
              }
            }
            else if ((item == "190" && Codice == "3.4.7")) continue;
            else
            {
              foreach (DataRow dd in datiV.Rows)
              {
                if (dd["EA"].ToString() != "" && dd["EA"].ToString() != "0" && dd["ID"].ToString() == child.Attributes["ID"].Value && dd["Codice"].ToString() == Codice)
                {

                  hasdata = true;
                  total = true;
                }
              }

            }
          }
        }
        else
        {
          if (tmpNode != null && ((tmpNode["Tipo"].ToString() == "" || tmpNode["Tipo"].ToString() == "Ulteriori dati opzionali (acquisizione non automatica)") || (tmpNode["EA"].ToString() != "" && ((tmpNode["EA"].ToString() != "" && tmpNode["EA"].ToString() != "0")))))
          {
            hasdata = true;
            total = true;
          }
        }

        if (hasdata == false || (b_Titolo[item].ToString() == "Totale" && total == false))
        {
          continue;
        }
      }



      return total;
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
      foreach (DataRow dd in datiN.Rows)
      {
        if (dd["ID"].ToString() == name)
        {
          dd["Esecutore"] = ((TextBox)sender).Text;

          //somethinghaschanged = true;
        }
      }


    }

    void txtNota_LostFocus(object sender, RoutedEventArgs e)
    {
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }

      string name = ((TextBox)sender).Name.Split('_')[1];


      foreach (DataRow dd in datiV.Rows)
      {
        if (dd["ID"].ToString() == name && dd["Codice"].ToString() == VociBilancio[((TextBox)sender).Name.Split('_')[3]].ToString())
        {
          dd["Note"] = ((TextBox)sender).Text;

          //somethinghaschanged = true;
        }
      }

    }

    void txtEA_LostFocus(object sender, RoutedEventArgs e)
    {
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }

      string name = ((TextBox)sender).Name.Split('_')[1];
      string rowhere = ((TextBox)sender).Name.Split('_')[2];

      ((TextBox)sender).Text = ConvertNumber(((TextBox)sender).Text);

      string codice = ((StackPanel)(((TextBox)sender).Parent)).Name.Split('_')[4];
      foreach (DataRow dd in datiVRighe.Rows)
      {
        if (dd["ID"].ToString() == name && dd["row"].ToString() == rowhere)
        {
          dd["EA"] = ((TextBox)sender).Text;

          //somethinghaschanged = true;
        }
      }


      double val1 = 0.0;
      double valtot = 0.0;
      double testd = 0.0;

      int indexhere = 0;

      foreach (UIElement item in ((StackPanel)(((TextBox)sender).Parent)).Children)
      {
        if (item.GetType().Name == "TextBox")
        {
          if (indexhere == 0)
          {
            if (Double.TryParse(((TextBox)item).Text, out testd))
            {
              val1 = Convert.ToSingle(((TextBox)item).Text);
            }
          }
          else
          {
            if (Double.TryParse(((TextBox)item).Text, out testd))
            {
              valtot += Convert.ToSingle(((TextBox)item).Text);
            }
          }

          indexhere++;
        }
      }

      if (this.FindName("txtEA_" + name + "_TOT") != null)
      {
        TextBlock stphere = ((TextBlock)(this.FindName("txtEA_" + name + "_TOT")));
        stphere.Text = ConvertNumber((val1 - valtot).ToString());
      }

      if (valtot > val1)
      {
        MessageBox.Show("Il valore o la somma dei valori dei Conti Contabili inseriti è maggiore del valore corrispondente.", "Attenzione");
      }
    }

    void txtTitolo_LostFocus(object sender, RoutedEventArgs e)
    {
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }

      string name = ((TextBox)sender).Name.Split('_')[1];
      string rowhere = ((TextBox)sender).Name.Split('_')[2];

      string codice = ((StackPanel)(((StackPanel)(((StackPanel)(((TextBox)sender).Parent)).Parent)).Parent)).Name.Split('_')[4];
      foreach (DataRow dd in datiVRighe.Rows)
      {
        if (dd["ID"].ToString() == name && dd["row"].ToString() == rowhere)
        {
          dd["Titolo"] = ((TextBox)sender).Text;

          //somethinghaschanged = true;
        }
      }

    }

    void txtCodice_LostFocus(object sender, RoutedEventArgs e)
    {
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }

      string name = ((TextBox)sender).Name.Split('_')[1];
      string rowhere = ((TextBox)sender).Name.Split('_')[2];

      string codice = ((StackPanel)(((StackPanel)(((StackPanel)(((TextBox)sender).Parent)).Parent)).Parent)).Name.Split('_')[4];
      foreach (DataRow dd in datiVRighe.Rows)
      {
        if (dd["ID"].ToString() == name && dd["row"].ToString() == rowhere)
        {
          dd["Codice"] = ((TextBox)sender).Text;

          //somethinghaschanged = true;
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
      foreach (DataRow dd in datiN.Rows)
      {
        if (dd["ID"].ToString() == name)
        {
          dd["Nota"] = ((TextBox)sender).Text;

          //somethinghaschanged = true;
        }
      }

    }

    public int Save()
    {

      double actualwidth = ((Grid)(txtDescrizioneIntensita.Parent)).ActualWidth;

      //XmlNode xnode = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]");

      //XmlNodeList nodelist = xnode.SelectNodes("Valore[@ID]");

      //for (int i = 0; i < nodelist.Count; i++)
      //{
      //    if (!donehere.Contains(nodelist[i].Attributes["ID"].Value))
      //    {
      //        xnode.RemoveChild(nodelist[i]);
      //    }
      //}

      for (int i = 1; i < brdDefinizione.Children.Count; i++)
      {
        Grid gridparent = ((Grid)(((Border)(brdDefinizione.Children[i])).Child));
        Grid grid = ((Grid)(gridparent.Children[2]));
        //grid.Width = actualwidth - 70;

        StringBuilder outstr = new StringBuilder();

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.OmitXmlDeclaration = true;
        settings.NewLineOnAttributes = true;


        XamlDesignerSerializationManager dsm = new XamlDesignerSerializationManager(XmlWriter.Create(outstr, settings));
        dsm.XamlWriterMode = XamlWriterMode.Expression;

        XamlWriter.Save(grid, dsm);
        string savedControls = outstr.ToString();

        foreach (DataRow tmpnode in datiN.Rows)
        {
          if (tmpnode["ID"].ToString() == gridparent.Name.Split('_')[1].ToString())
          {

            DirectoryInfo di = new DirectoryInfo(App.AppDataDataFolder + "\\XAML");

            if (!di.Exists)
            {
              di.Create();
            }


            FileInfo fxamlhere = new FileInfo(App.AppDataDataFolder + tmpnode["xaml"].ToString());

            if (!fxamlhere.Exists)
            {
              string newXamlFile = "\\XAML\\" + Guid.NewGuid().ToString() + ".xaml";
              FileInfo fxaml = new FileInfo(App.AppDataDataFolder + newXamlFile);

              while (fxaml.Exists)
              {
                newXamlFile = "\\XAML\\" + Guid.NewGuid().ToString() + ".xaml";
                fxaml = new FileInfo(App.AppDataDataFolder + newXamlFile);
              }

              tmpnode["xaml"] = newXamlFile;

              fxamlhere = new FileInfo(App.AppDataDataFolder + tmpnode["xaml"].ToString());
            }

            StreamWriter sw = fxamlhere.CreateText();
            sw.WriteLine(savedControls);
            sw.Flush();
            sw.Close();
          }
        }
      }

      cBusinessObjects.SaveData(id, datiN, typeof(PianificazioneNewWD_Node));
      cBusinessObjects.SaveData(id, datiV, typeof(PianificazioneNewWD_Valore));
      cBusinessObjects.SaveData(id, datiVRighe, typeof(PianificazioneNewWD_ValoreRighe));

      return 0;
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      return;
      // il blocco testo è nascosto
      XmlNode tmpnode = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + _ID + "']");

      if (tmpnode != null)
      {
        if (tmpnode.Attributes["Testo"] == null)
        {
          XmlAttribute attr = _x.Document.CreateAttribute("Testo");
          tmpnode.Attributes.Append(attr);
        }

        tmpnode.Attributes["Testo"].Value = ((TextBox)sender).Text;
        //somethinghaschanged = true;
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
      string tipo = ((Image)sender).Name.Split('_').Last();
      string name = ((Image)sender).Name.Split('_')[1];
      string rowhere = ((Image)sender).Name.Split('_')[2];
      string codice = ((Image)sender).Name.Split('_')[3];

      var uriSource = new Uri(uncheck, UriKind.Relative);
      DataRow tmpnode = null;

      foreach (DataRow dd in datiV.Rows)
      {
        if (dd["ID"].ToString() == name && dd["Codice"].ToString() == VociBilancio[codice].ToString())
        {
          tmpnode = dd;


          //somethinghaschanged = true;
        }
      }


      if (tmpnode != null)
      {

        if (tmpnode[tipo].ToString() == "True")
        {
          tmpnode[tipo] = "";

          uriSource = new Uri(uncheck, UriKind.Relative);
          ((Image)sender).Source = new BitmapImage(uriSource);
        }
        else
        {
          tmpnode[tipo] = "True";

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
      double actualwidth = ((Grid)(txtDescrizioneIntensita.Parent)).ActualWidth;

      for (int i = 1; i < brdDefinizione.Children.Count; i++)
      {
        Grid grid = ((Grid)(((Border)(brdDefinizione.Children[i])).Child));
        grid.Width = (actualwidth - 70 > 970) ? actualwidth - 70 : 970;

        grid = ((Grid)(grid.Children[2]));

        ((TextBox)(grid.Children[1])).Width = (actualwidth - 110 > 930) ? actualwidth - 110 : 930;
        ((TextBox)(grid.Children[3])).Width = (actualwidth - 110 > 930) ? actualwidth - 110 : 930;

        grid = ((Grid)(grid.Children[5]));
        foreach (UIElement item in grid.Children)
        {
          if (item.GetType().Name == "StackPanel" && ((StackPanel)item).Name.Contains("_STKName"))
          {
            try
            {
              UIElement itemhere = ((StackPanel)item).Children[0];

              if (itemhere.GetType().Name == "TextBlock")
              {
                ((TextBlock)itemhere).Width = (actualwidth - 860 > 250) ? actualwidth - 860 : 250;
              }

              //if (((StackPanel)item).Children.Count > 1)
              //{
              //    itemhere = ((StackPanel)item).Children[1];

              //    if (itemhere.GetType().Name == "TextBlock")
              //    {
              //        ((TextBlock)itemhere).Width = (actualwidth - 860 > 400) ? actualwidth - 860 : 400;
              //    }
              //}

              for (int jj = 2; jj < ((StackPanel)item).Children.Count; jj++)
              {
                itemhere = ((StackPanel)item).Children[jj];
                if (itemhere.GetType().Name == "StackPanel")
                {
                  itemhere = ((StackPanel)itemhere).Children[1];

                  if (itemhere.GetType().Name == "StackPanel")
                  {
                    itemhere = ((StackPanel)itemhere).Children[1];

                    if (itemhere.GetType().Name == "TextBox")
                    {
                      ((TextBox)itemhere).Width = (actualwidth - 860 > 160) ? actualwidth - 860 : 160;
                    }
                  }
                }
                //((TextBox)((StackPanel)(((StackPanel)(((StackPanel)item).Children[jj])).Children[1])).Children[1]).Width = (actualwidth - 860 > 200) ? actualwidth - 860 : 200;
              }
            }
            catch (Exception ex)
            {
              string log = ex.Message;
            }
          }

          if (item.GetType().Name == "StackPanel" && ((StackPanel)item).Name.Contains("stpNoteAll_"))
          {
            foreach (UIElement item2 in ((StackPanel)item).Children)
            {
              try
              {
                if (item2.GetType().Name == "StackPanel")
                {
                  ((TextBox)(((StackPanel)item2).Children[1])).Width = (actualwidth - 120 > 930) ? actualwidth - 120 : 930;
                }
              }
              catch (Exception ex)
              {
                string log = ex.Message;
              }
            }
          }
        }
      }

      txtConsiderazioni.Width = (actualwidth - 110 > 930) ? actualwidth - 110 : 930;
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


        DataTable datiRishcioGlobale = cBusinessObjects.GetData(int.Parse(IDRischioGlobale), typeof(RischioGlobale));

        string nv = "";
        foreach (DataRow node in datiRishcioGlobale.Rows)
        {
          nv = node[((ComboBox)sender).Name.Split('_')[3]].ToString();
        }


        if (((ComboBox)sender).Name.Split('_')[3] == "pv")
        {
          ((ComboBox)sender).SelectedItem = ((ComboBoxItem)((ComboBox)sender).Items[5]);
          ((ComboBox)sender).Text = ((ComboBoxItem)((ComboBox)sender).Items[5]).Content.ToString();
        }
        else
        {
          int selecteditem = 0;

          switch (nv)
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

            ((ComboBox)sender).SelectedItem = ((ComboBoxItem)((ComboBox)sender).Items[selecteditem]);
          ((ComboBox)sender).Text = ((ComboBoxItem)((ComboBox)sender).Items[selecteditem]).Content.ToString();
        }
      }
      else
      {
        string name = ((ComboBox)sender).Name.Split('_')[1];
        DataRow tmpnode = null;
        foreach (DataRow dd in datiN.Rows)
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
          //somethinghaschanged = true;
        }
      }
    }

    void txt_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount == 2)
      {
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
