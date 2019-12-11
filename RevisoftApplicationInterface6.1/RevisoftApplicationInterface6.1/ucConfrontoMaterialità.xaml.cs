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
  public partial class ucConfrontoMaterialita : UserControl
  {
    public int id;
    private DataTable dati = null;

    private XmlDataProviderManager _x;
    private XmlDataProviderManager _lm;
    private string _ID = "-1";

    Hashtable valoreEA = new Hashtable();

    Hashtable SommeDaExcel = new Hashtable();
    Hashtable ValoriDaExcelEA = new Hashtable();

    private string ID_Materialità_1 = "77";
    private string ID_Materialità_2 = "78";
    private string ID_Materialità_3 = "199";

    private bool Materialità_1 = false;
    private bool Materialità_2 = false;
    private bool Materialità_3 = false;

    Hashtable Sessioni = new Hashtable();
    Hashtable SessioniTitoli = new Hashtable();
    Hashtable SessioniID = new Hashtable();
    int SessioneNow;
    string IDTree;
    string IDCliente;
    string IDSessione;

    private int rowTOT = 0;

    TextBlock txtErroreTollerabileSP = new TextBlock();
    TextBlock txtErroreTollerabileCE = new TextBlock();

    public ucConfrontoMaterialita()
    {
      InitializeComponent();
    }

    public bool _ReadOnly = false;

    public bool ReadOnly
    {
      set
      {
        _ReadOnly = value;
      }
    }


    public bool Load(string ID, string FileConclusione, Hashtable _Sessioni, Hashtable _SessioniTitoli, Hashtable _SessioniID, int _SessioneNow, string _IDTree, string _IDCliente, string _IDSessione)
    {
     
      id = int.Parse(ID.ToString());
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

      #region DA MATERIALITA'

      MasterFile mf = MasterFile.Create();
      string FileDataRevisione = mf.GetRevisioneAssociataFromConclusioneFile(FileConclusione);

      if (FileDataRevisione != "")
      {
        _lm = new XmlDataProviderManager(FileDataRevisione);
      }
      else
      {
        _lm = null;
      }


      DataTable tmpNode_true = null;

      string idsessionedatimaterialita = cBusinessObjects.CercaSessione("Conclusione", "Revisione", IDSessione, cBusinessObjects.idcliente);

      DataTable datimaterialita = cBusinessObjects.GetData(int.Parse(ID_Materialità_1), typeof(Excel_LimiteMaterialitaSPCE), cBusinessObjects.idcliente, int.Parse(idsessionedatimaterialita), 1);


      if (datimaterialita.Rows.Count > 0)
      {
        string statomat = "";
        DataTable statom = cBusinessObjects.GetData(int.Parse(ID_Materialità_1), typeof(StatoNodi), cBusinessObjects.idcliente, int.Parse(idsessionedatimaterialita), 1);

        foreach (DataRow dd in statom.Rows)
        {
          statomat = dd["Stato"].ToString().Trim();
        }
        if (datimaterialita.Rows.Count > 0 && statomat != "" && ((App.TipoTreeNodeStato)(Convert.ToInt32(statomat))) == App.TipoTreeNodeStato.Completato)
        {
          Materialità_1 = true;

          ucExcel_LimiteMaterialitaSPCE uce_lm = new ucExcel_LimiteMaterialitaSPCE(IDTree);
          uce_lm.Load(ID_Materialità_1, FileDataRevisione, IpotesiMaterialita.Prima, IDCliente, IDSessione);
          uce_lm.Save();

          brdPrima.Visibility = System.Windows.Visibility.Visible;
          brdSeconda.Visibility = System.Windows.Visibility.Collapsed;
          brdTerza.Visibility = System.Windows.Visibility.Collapsed;
          tmpNode_true = datimaterialita;
        }

        //if (tmpNode_true == null)
        {
          datimaterialita = cBusinessObjects.GetData(int.Parse(ID_Materialità_2), typeof(Excel_LimiteMaterialitaSPCE), cBusinessObjects.idcliente, int.Parse(idsessionedatimaterialita), 1);

          statom = cBusinessObjects.GetData(int.Parse(ID_Materialità_2), typeof(StatoNodi), cBusinessObjects.idcliente, int.Parse(idsessionedatimaterialita), 1);
          foreach (DataRow dd in statom.Rows)
          {
            statomat = dd["Stato"].ToString().Trim();
          }
          if (datimaterialita.Rows.Count > 0 && statomat != "" && ((App.TipoTreeNodeStato)(Convert.ToInt32(statomat))) == App.TipoTreeNodeStato.Completato)
          {
            Materialità_2 = true;

            ucExcel_LimiteMaterialitaSPCE uce_lm = new ucExcel_LimiteMaterialitaSPCE(IDTree);
            uce_lm.Load(ID_Materialità_2, FileDataRevisione, IpotesiMaterialita.Seconda, IDCliente, IDSessione);
            uce_lm.Save();

            brdPrima.Visibility = System.Windows.Visibility.Collapsed;
            brdSeconda.Visibility = System.Windows.Visibility.Visible;
            brdTerza.Visibility = System.Windows.Visibility.Collapsed;
            tmpNode_true = datimaterialita;
          }

          //if (tmpNode_true == null)
          {
            datimaterialita = cBusinessObjects.GetData(int.Parse(ID_Materialità_3), typeof(Excel_LimiteMaterialitaSPCE), cBusinessObjects.idcliente, int.Parse(idsessionedatimaterialita), 1);
            statom = cBusinessObjects.GetData(int.Parse(ID_Materialità_3), typeof(StatoNodi), cBusinessObjects.idcliente, int.Parse(idsessionedatimaterialita), 1);
            foreach (DataRow dd in statom.Rows)
            {
              statomat = dd["Stato"].ToString().Trim();
            }
            if (datimaterialita.Rows.Count > 0 && statomat != "" && ((App.TipoTreeNodeStato)(Convert.ToInt32(statomat))) == App.TipoTreeNodeStato.Completato)
            {
              Materialità_3 = true;

              ucExcel_LimiteMaterialitaSPCE uce_lm = new ucExcel_LimiteMaterialitaSPCE(IDTree);
              uce_lm.Load(ID_Materialità_3, FileDataRevisione, IpotesiMaterialita.Terza, IDCliente, IDSessione);
              uce_lm.Save();

              brdPrima.Visibility = System.Windows.Visibility.Collapsed;
              brdSeconda.Visibility = System.Windows.Visibility.Collapsed;
              brdTerza.Visibility = System.Windows.Visibility.Visible;
              tmpNode_true = datimaterialita;
            }
          }
        }
      }

      if (tmpNode_true != null)
      {

        foreach (DataRow dtrow in tmpNode_true.Rows)
        {
          if (dtrow["ID"].ToString() == "txt7")
            txt7.Text = dtrow["value"].ToString();
          if (dtrow["ID"].ToString() == "txt7_2sp")
            txt7_2sp.Text = dtrow["value"].ToString();
          if (dtrow["ID"].ToString() == "txt7_2ce")
            txt7_2ce.Text = dtrow["value"].ToString();
          if (dtrow["ID"].ToString() == "txt7_3sp")
            txt7_3sp.Text = dtrow["value"].ToString();
          if (dtrow["ID"].ToString() == "txt7_3ec")
            txt7_3ec.Text = dtrow["value"].ToString();
          if (dtrow["ID"].ToString() == "txt9")
            txt9.Text = dtrow["value"].ToString();
          if (dtrow["ID"].ToString() == "txt9_2sp")
            txt9_2sp.Text = dtrow["value"].ToString();
          if (dtrow["ID"].ToString() == "txt9_2ce")
            txt9_2ce.Text = dtrow["value"].ToString();
          if (dtrow["ID"].ToString() == "txt9_3ec")
            txt9_3ec.Text = dtrow["value"].ToString();
          if (dtrow["ID"].ToString() == "txt7BILANCIO")
            txt7BILANCIO.Text = dtrow["value"].ToString();
          if (dtrow["ID"].ToString() == "txt7_2spBILANCIO")
            txt7_2spBILANCIO.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt7_2ceBILANCIO")
            txt7_2ceBILANCIO.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt7_3spBILANCIO")
            txt7_3spBILANCIO.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt7_3ecBILANCIO")
            txt7_3ecBILANCIO.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt9BILANCIO")
            txt9BILANCIO.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt9_2spBILANCIO")
            txt9_2spBILANCIO.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt9_2ceBILANCIO")
            txt9_2ceBILANCIO.Text = dtrow["value"].ToString();


          if (dtrow["ID"].ToString() == "txt9_3spBILANCIO")
            txt9_3spBILANCIO.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt9_3ecBILANCIO")
            txt9_3ecBILANCIO.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt12")
            txt12.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt12_2sp")
            txt12_2sp.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt12_2sp")
            txt12_2sp.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt12_2ce")
            txt12_2ce.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt12_3sp")
            txt12_3sp.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt12_3ec")
            txt12_3ec.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt12BILANCIO")
            txt12BILANCIO.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt12_2spBILANCIO")
            txt12_2spBILANCIO.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt12_2ceBILANCIO")
            txt12_2ceBILANCIO.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt12_3spBILANCIO")
            txt12_3spBILANCIO.Text = dtrow["value"].ToString();

          if (dtrow["ID"].ToString() == "txt12_3ecBILANCIO")
            txt12_3ecBILANCIO.Text = dtrow["value"].ToString();


        }

      }





      if (Materialità_1 == false && Materialità_2 == false && Materialità_3 == false)
      {
        MessageBox.Show("E' necessario completare prima la materialità", "Attenzione");
        return false;

      }

      if (((Materialità_1) ? 1 : 0) + ((Materialità_2) ? 1 : 0) + ((Materialità_3) ? 1 : 0) >= 2)
      {
        MessageBox.Show("Il confronto materialità può essere considerato valido solo nel caso sia stato utilizzato un solo calcolo della materialità.", "Attenzione");
        return false;
      }



      #endregion

      #region CONFRONTO MATERIALITA' CALCOLO
      txt7VALORE.Text = ConvertNumber((Convert.ToDouble((txt7BILANCIO.Text == "") ? "0" : txt7BILANCIO.Text) - Convert.ToDouble((txt7.Text == "") ? "0" : txt7.Text)).ToString());
      txt9VALORE.Text = ConvertNumber((Convert.ToDouble((txt9BILANCIO.Text == "") ? "0" : txt9BILANCIO.Text) - Convert.ToDouble((txt9.Text == "") ? "0" : txt9.Text)).ToString());
      txt12VALORE.Text = ConvertNumber((Convert.ToDouble((txt12BILANCIO.Text == "") ? "0" : txt12BILANCIO.Text) - Convert.ToDouble((txt12.Text == "") ? "0" : txt12.Text)).ToString());

      txt7_2spVALORE.Text = ConvertNumber((Convert.ToDouble((txt7_2spBILANCIO.Text == "") ? "0" : txt7_2spBILANCIO.Text) - Convert.ToDouble((txt7_2sp.Text == "") ? "0" : txt7_2sp.Text)).ToString());
      txt7_2ceVALORE.Text = ConvertNumber((Convert.ToDouble((txt7_2ceBILANCIO.Text == "") ? "0" : txt7_2ceBILANCIO.Text) - Convert.ToDouble((txt7_2ce.Text == "") ? "0" : txt7_2ce.Text)).ToString());
      txt7_3spVALORE.Text = ConvertNumber((Convert.ToDouble((txt7_3spBILANCIO.Text == "") ? "0" : txt7_3spBILANCIO.Text) - Convert.ToDouble((txt7_3sp.Text == "") ? "0" : txt7_3sp.Text)).ToString());
      txt7_3ecVALORE.Text = ConvertNumber((Convert.ToDouble((txt7_3ecBILANCIO.Text == "") ? "0" : txt7_3ecBILANCIO.Text) - Convert.ToDouble((txt7_3ec.Text == "") ? "0" : txt7_3ec.Text)).ToString());

      txt9_2spVALORE.Text = ConvertNumber((Convert.ToDouble((txt9_2spBILANCIO.Text == "") ? "0" : txt9_2spBILANCIO.Text) - Convert.ToDouble((txt9_2sp.Text == "") ? "0" : txt9_2sp.Text)).ToString());
      txt9_2ceVALORE.Text = ConvertNumber((Convert.ToDouble((txt9_2ceBILANCIO.Text == "") ? "0" : txt9_2ceBILANCIO.Text) - Convert.ToDouble((txt9_2ce.Text == "") ? "0" : txt9_2ce.Text)).ToString());
      txt9_3spVALORE.Text = ConvertNumber((Convert.ToDouble((txt9_3spBILANCIO.Text == "") ? "0" : txt9_3spBILANCIO.Text) - Convert.ToDouble((txt9_3sp.Text == "") ? "0" : txt9_3sp.Text)).ToString());
      txt9_3ecVALORE.Text = ConvertNumber((Convert.ToDouble((txt9_3ecBILANCIO.Text == "") ? "0" : txt9_3ecBILANCIO.Text) - Convert.ToDouble((txt9_3ec.Text == "") ? "0" : txt9_3ec.Text)).ToString());

      txt12_2spVALORE.Text = ConvertNumber((Convert.ToDouble((txt12_2spBILANCIO.Text == "") ? "0" : txt12_2spBILANCIO.Text) - Convert.ToDouble((txt12_2sp.Text == "") ? "0" : txt12_2sp.Text)).ToString());
      txt12_2ceVALORE.Text = ConvertNumber((Convert.ToDouble((txt12_2ceBILANCIO.Text == "") ? "0" : txt12_2ceBILANCIO.Text) - Convert.ToDouble((txt12_2ce.Text == "") ? "0" : txt12_2ce.Text)).ToString());
      txt12_3spVALORE.Text = ConvertNumber((Convert.ToDouble((txt12_3spBILANCIO.Text == "") ? "0" : txt12_3spBILANCIO.Text) - Convert.ToDouble((txt12_3sp.Text == "") ? "0" : txt12_3sp.Text)).ToString());
      txt12_3ecVALORE.Text = ConvertNumber((Convert.ToDouble((txt12_3ecBILANCIO.Text == "") ? "0" : txt12_3ecBILANCIO.Text) - Convert.ToDouble((txt12_3ec.Text == "") ? "0" : txt12_3ec.Text)).ToString());

      txt7PERCENTUALE.Text = ConvertPercent(((Convert.ToDouble((txt7BILANCIO.Text == "") ? "0" : txt7BILANCIO.Text) - Convert.ToDouble((txt7.Text == "") ? "0" : txt7.Text)) / Convert.ToDouble((txt7.Text == "") ? "0" : txt7.Text)).ToString());
      txt9PERCENTUALE.Text = ConvertPercent(((Convert.ToDouble((txt9BILANCIO.Text == "") ? "0" : txt9BILANCIO.Text) - Convert.ToDouble((txt9.Text == "") ? "0" : txt9.Text)) / Convert.ToDouble((txt9.Text == "") ? "0" : txt9.Text)).ToString());
      txt12PERCENTUALE.Text = ConvertPercent(((Convert.ToDouble((txt12BILANCIO.Text == "") ? "0" : txt12BILANCIO.Text) - Convert.ToDouble((txt12.Text == "") ? "0" : txt12.Text)) / Convert.ToDouble((txt12.Text == "") ? "0" : txt12.Text)).ToString());

      txt7_2spPERCENTUALE.Text = ConvertPercent(((Convert.ToDouble((txt7_2spBILANCIO.Text == "") ? "0" : txt7_2spBILANCIO.Text) - Convert.ToDouble((txt7_2sp.Text == "") ? "0" : txt7_2sp.Text)) / Convert.ToDouble((txt7_2sp.Text == "") ? "0" : txt7_2sp.Text)).ToString());
      txt7_2cePERCENTUALE.Text = ConvertPercent(((Convert.ToDouble((txt7_2ceBILANCIO.Text == "") ? "0" : txt7_2ceBILANCIO.Text) - Convert.ToDouble((txt7_2ce.Text == "") ? "0" : txt7_2ce.Text)) / Convert.ToDouble((txt7_2ce.Text == "") ? "0" : txt7_2ce.Text)).ToString());
      txt7_3spPERCENTUALE.Text = ConvertPercent(((Convert.ToDouble((txt7_3spBILANCIO.Text == "") ? "0" : txt7_3spBILANCIO.Text) - Convert.ToDouble((txt7_3sp.Text == "") ? "0" : txt7_3sp.Text)) / Convert.ToDouble((txt7_3sp.Text == "") ? "0" : txt7_3sp.Text)).ToString());
      txt7_3ecPERCENTUALE.Text = ConvertPercent(((Convert.ToDouble((txt7_3ecBILANCIO.Text == "") ? "0" : txt7_3ecBILANCIO.Text) - Convert.ToDouble((txt7_3ec.Text == "") ? "0" : txt7_3ec.Text)) / Convert.ToDouble((txt7_3ec.Text == "") ? "0" : txt7_3ec.Text)).ToString());

      txt9_2spPERCENTUALE.Text = ConvertPercent(((Convert.ToDouble((txt9_2spBILANCIO.Text == "") ? "0" : txt9_2spBILANCIO.Text) - Convert.ToDouble((txt9_2sp.Text == "") ? "0" : txt9_2sp.Text)) / Convert.ToDouble((txt9_2sp.Text == "") ? "0" : txt9_2sp.Text)).ToString());
      txt9_2cePERCENTUALE.Text = ConvertPercent(((Convert.ToDouble((txt9_2ceBILANCIO.Text == "") ? "0" : txt9_2ceBILANCIO.Text) - Convert.ToDouble((txt9_2ce.Text == "") ? "0" : txt9_2ce.Text)) / Convert.ToDouble((txt9_2ce.Text == "") ? "0" : txt9_2ce.Text)).ToString());
      txt9_3spPERCENTUALE.Text = ConvertPercent(((Convert.ToDouble((txt9_3spBILANCIO.Text == "") ? "0" : txt9_3spBILANCIO.Text) - Convert.ToDouble((txt9_3sp.Text == "") ? "0" : txt9_3sp.Text)) / Convert.ToDouble((txt9_3sp.Text == "") ? "0" : txt9_3sp.Text)).ToString());
      txt9_3ecPERCENTUALE.Text = ConvertPercent(((Convert.ToDouble((txt9_3ecBILANCIO.Text == "") ? "0" : txt9_3ecBILANCIO.Text) - Convert.ToDouble((txt9_3ec.Text == "") ? "0" : txt9_3ec.Text)) / Convert.ToDouble((txt9_3ec.Text == "") ? "0" : txt9_3ec.Text)).ToString());

      txt12_2spPERCENTUALE.Text = ConvertPercent(((Convert.ToDouble((txt12_2spBILANCIO.Text == "") ? "0" : txt12_2spBILANCIO.Text) - Convert.ToDouble((txt12_2sp.Text == "") ? "0" : txt12_2sp.Text)) / Convert.ToDouble((txt12_2sp.Text == "") ? "0" : txt12_2sp.Text)).ToString());
      txt12_2cePERCENTUALE.Text = ConvertPercent(((Convert.ToDouble((txt12_2ceBILANCIO.Text == "") ? "0" : txt12_2ceBILANCIO.Text) - Convert.ToDouble((txt12_2ce.Text == "") ? "0" : txt12_2ce.Text)) / Convert.ToDouble((txt12_2ce.Text == "") ? "0" : txt12_2ce.Text)).ToString());
      txt12_3spPERCENTUALE.Text = ConvertPercent(((Convert.ToDouble((txt12_3spBILANCIO.Text == "") ? "0" : txt12_3spBILANCIO.Text) - Convert.ToDouble((txt12_3sp.Text == "") ? "0" : txt12_3sp.Text)) / Convert.ToDouble((txt12_3sp.Text == "") ? "0" : txt12_3sp.Text)).ToString());
      txt12_3ecPERCENTUALE.Text = ConvertPercent(((Convert.ToDouble((txt12_3ecBILANCIO.Text == "") ? "0" : txt12_3ecBILANCIO.Text) - Convert.ToDouble((txt12_3ec.Text == "") ? "0" : txt12_3ec.Text)) / Convert.ToDouble((txt12_3ec.Text == "") ? "0" : txt12_3ec.Text)).ToString());

      #endregion


      return true;
    }

    public int Save()
    {

      dati = cBusinessObjects.GetData(id, typeof(ConfrontoMaterialita));
      if (dati.Rows.Count == 0)
      {
        DataRow dd = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
        dd["txtErroreTollerabileSP"] = txtErroreTollerabileSP.Text;
        dd["txtErroreTollerabileCE"] = txtErroreTollerabileCE.Text;
      }
      cBusinessObjects.SaveData(id, dati, typeof(ConfrontoMaterialita));


      return 0;
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


    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      double newsize = e.NewSize.Width - 30.0;

      for (int i = 4; i <= rowTOT; i++)
      {
        TextBlock txtName = (TextBlock)this.FindName("txtName" + i.ToString());
        txtName.Width = newsize - 890;
      }

      //try
      //{
      //    brdMain.Width = Convert.ToDouble(newsize);
      //    for ( int i = 0; i < grdMain.Children.Count; i++ )
      //    {
      //        object  var = grdMain.Children[i];
      //        if ( var.GetType().Name == "Border" )
      //        {
      //            object var2 = ((Border)var).Child;

      //            if ( ((TextBlock)var2).TextWrapping == TextWrapping.Wrap && ((TextBlock)var2).Name.IndexOf("txtName") != -1 )
      //            {
      //                ((TextBlock)var2).Width = newsize - 700;
      //            }
      //        }
      //    }
      //}
      //catch (Exception ex)
      //{
      //    string log = ex.Message;
      //}
    }

    void txt_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount == 2)
      {
        WindowWorkArea wa = new WindowWorkArea(ref _x);

        //Nodi
        int index = -1;
        wa.NodeHome = -1;

        RevisoftApplication.XmlManager xt = new XmlManager();
        xt.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
        XmlDataProvider TreeXmlProvider = new XmlDataProvider();
        TreeXmlProvider.Document = xt.LoadEncodedFile(App.AppTemplateTreeBilancio);

        if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
        {
          foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
          {
            if (item.Attributes["Tipologia"].Value == "Nodo Multiplo" || item.ChildNodes.Count == 1)
            {
              index++;

              if (item.Attributes["ID"].Value == ((TextBlock)(sender)).MaxHeight.ToString())
              {
                wa.NodeHome = index;
              }

              wa.Nodes.Add(index, item);
            }
          }
        }

        if (wa.NodeHome == -1)
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
        wa.Sessioni = Sessioni;
        wa.SessioniTitoli = SessioniTitoli;
        wa.SessioniID = SessioniID;

        foreach (DictionaryEntry item in Sessioni)
        {
          if (item.Value.ToString() == _x.File)
          {
            wa.SessioneHome = Convert.ToInt32(item.Key.ToString());
            wa.SessioneNow = wa.SessioneHome;
            break;
          }
        }

        //Variabili
        wa.ReadOnly = true;
        wa.ReadOnlyOLD = true;
        wa.ApertoInSolaLettura = true;

        //XmlNode nodeSessione = node.SelectSingleNode( "Sessioni/Sessione[@Alias=\"" + selectedAliasCodificato + "\"]" );
        //if ( nodeSessione != null )
        //{
        //    wa.Stato = ((App.TipoTreeNodeStato)(Convert.ToInt32( nodeSessione.Attributes["Stato"].Value )));
        //    wa.OldStatoNodo = wa.Stato;
        //}

        //passaggio dati
        wa.IDTree = IDTree;
        wa.IDSessione = IDSessione;
        wa.IDCliente = IDCliente;

        //apertura
        wa.Load();

        wa.ShowDialog();
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
  }
}
