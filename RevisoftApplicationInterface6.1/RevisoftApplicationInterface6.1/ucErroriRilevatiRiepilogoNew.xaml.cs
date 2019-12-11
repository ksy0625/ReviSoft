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
using System.Xml.XPath;
using System.Data;

namespace UserControls
{ 
    public partial class ucErroriRilevatiRiepilogoNew : UserControl
    {
        public int id;
        private DataTable dati = null;
        private DataTable datibilancio = null;
        string idsessionebilancio = "";



        private XmlDataProviderManager _x;
		private XmlDataProviderManager _lm;
        private XmlDataProviderManager _lb;
        private string _ID = "-1";

		private string IDB_Padre = "227";
		private string IDBA_Padre = "229";

        private string up = "./Images/icone/navigate_up.png";
        private string down = "./Images/icone/navigate_down.png";
        private string left = "./Images/icone/navigate_left.png";


		Hashtable valoreEA = new Hashtable();

		Hashtable SommeDaExcel = new Hashtable();
		Hashtable ValoriDaExcelEA = new Hashtable();

		private string ID_Materialità_1 = "77";
		private string ID_Materialità_2 = "78";
		private string ID_Materialità_3 = "199";
		
		private bool Materialità_1 = false;
		private bool Materialità_2 = false;
		private bool Materialità_3 = false;

		//double txt1;		
		//double txt2;		
		//double txt3;		
		//double txt4;	
		//double txt7_3sp;
		//double txt7_3ec;

		string txt7 = "";
		string txt7_2sp = "";
		string txt7_2ce = "";
		string txt7_3sp = "";
		string txt7_3ec = "";		

		string txt9 = "";
		string txt9_2sp = "";
		string txt9_2ec = "";
		string txt9_3sp = "";
		string txt9_3ec = "";

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


        Hashtable indicerow = new Hashtable();

        public ucErroriRilevatiRiepilogoNew()
        {
            if (up.Equals("")) { }
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


        public bool Load(  string ID, string FileConclusione, Hashtable _Sessioni, Hashtable _SessioniTitoli, Hashtable _SessioniID, int _SessioneNow, string _IDTree, string _IDCliente, string _IDSessione )
        {
            id = int.Parse(ID);

            Sessioni = _Sessioni;
            SessioniTitoli = _SessioniTitoli;
            SessioniID = _SessioniID;
            SessioneNow = _SessioneNow;
            IDTree = _IDTree;
            IDCliente = _IDCliente;
            IDSessione = _IDSessione;

		
            _ID = ID;

            #region Dati da bilancio
            
            dati = cBusinessObjects.GetData(id, typeof(Excel_ErroriRilevati_riepilogo));

            RetrieveData( IDB_Padre);
			if (valoreEA.Count == 0)
			{
				RetrieveData( IDBA_Padre);
			}
            #endregion
             idsessionebilancio = cBusinessObjects.CercaSessione("Conclusione", "Bilancio", IDSessione, cBusinessObjects.idcliente);

            datibilancio = cBusinessObjects.GetData(int.Parse(IDB_Padre), typeof(Excel_Bilancio), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);
            if(datibilancio.Rows.Count==0)
                 datibilancio = cBusinessObjects.GetData(int.Parse(IDBA_Padre), typeof(Excel_Bilancio), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);

            #region DA MATERIALITA'

            //SommeDaExcel.Add("TotaleAttivita", "3|4|8|9|10|11|12|13|14|17|18|19|20|21|25|26|27|28|32|33|35|36|38|39|41|42|43|44|51|52|53|54|55|59|60|62|63|65|66|68|69|71|72|74|75|77|78|81|82|83|84|85|86|90|91|92|98|99"); //102
            //ValoriDaExcelEA.Add("TotaleAttivita", GetValoreEA("TotaleAttivita"));

            //SommeDaExcel.Add("Patrimonionetto", "3|4|8|9|10|11|12|13|14|17|18|19|20|21|25|26|27|28|32|33|35|36|38|39|41|42|43|44|51|52|53|54|55|59|60|62|63|65|66|68|69|71|72|74|75|77|78|81|82|83|84|85|86|90|91|92|98|99|-108|-109|-110|-111|-112|-113|-115|-116|-117|-118|-119|-120|-124|-125|-126|-129|-133|-134|-136|-137|-139|-140|-142|-143|-145|-146|-148|-149|-151|-152|-154|-155|-157|-158|-160|-161|-163|-164|-166|-167|-169|-170|-172|-173|-176|-177"); //185
            //ValoriDaExcelEA.Add("Patrimonionetto", GetValoreEA("Patrimonionetto"));

            //SommeDaExcel.Add("RicaviEsercizio", "189|190|191|192|194|195|-198|-199|-200|-202|-203|-204|-205|-206|-208|-209|-210|-211|-212|-213|-214|-215|222|223|224|227|228|229|230|231|232|234|235|236|237|-239|-240|-241|-242|-243|247|248|249|-251|-252|-253|257|258|-260|-261|-262|-267|268");//271
            //ValoriDaExcelEA.Add("RicaviEsercizio", GetValoreEA("RicaviEsercizio"));

            //SommeDaExcel.Add("RisultatoImposte", "189|190|191|192|194|195|-198|-199|-200|-202|-203|-204|-205|-206|-208|-209|-210|-211|-212|-213|-214|-215|222|223|224|227|228|229|230|231|232|234|235|236|237|-239|-240|-241|-242|-243|247|248|249|-251|-252|-253|257|258|-260|-261|-262"); //265
            //ValoriDaExcelEA.Add("RisultatoImposte", GetValoreEA("RisultatoImposte"));

            //txt1 = 0.0;// (double)(ValoriDaExcelEA["TotaleAttivita"]);

            //txt2 = 0.0;// (double)(ValoriDaExcelEA["Patrimonionetto"]);

            //txt3 = 0.0;// (double)(ValoriDaExcelEA["RicaviEsercizio"]);

            //txt4 = 0.0;// (double)(ValoriDaExcelEA["RisultatoImposte"]);

            //txt7_3sp = 0.0;
            //txt7_3ec = 0.0;

            MasterFile mf = MasterFile.Create();

            string FileDataRevisione  = "";

            if ( IDTree == "4" )
            {
                FileDataRevisione = mf.GetRevisioneAssociataFromBilancioFile( FileConclusione );

                if ( FileDataRevisione != "" )
                {
                    _lm = new XmlDataProviderManager( FileDataRevisione );
                }
                else
                {
                    _lm = null;
                }

             
            }
            else
            {
                FileDataRevisione  = mf.GetRevisioneAssociataFromConclusioneFile( FileConclusione );
                string FileDataBilancio = mf.GetBilancioAssociatoFromConclusioneFile( FileConclusione );

                if ( FileDataRevisione != "" )
                {
                    _lm = new XmlDataProviderManager( FileDataRevisione );
                }
                else
                {
                    _lm = null;
                }

                if ( FileDataBilancio != "" )
                {
                    _lb = new XmlDataProviderManager( FileDataBilancio );
                }
                else
                {
                    _lb = null;
                }
            }

            if (datibilancio.Rows.Count==0 )
            {
                MessageBox.Show( "E' necessario avere almeno generato il controllo di bilancio per questa sessione", "Attenzione" );
                return false;
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

                            tmpNode_true = datimaterialita;
                        }
                    }
                }
            }

            
			if (Materialità_1 == false && Materialità_2 == false && Materialità_3 == false)
			{
				MessageBox.Show("E' necessario completare prima la materialità", "Attenzione");
                return false;
			}

            if ( ((Materialità_1) ? 1 : 0) + ((Materialità_2) ? 1 : 0) + ((Materialità_3) ? 1 : 0) >= 2 )
            {
                MessageBox.Show( "Il sommario delle rettifiche può essere considerato valido solo nel caso sia stato utilizzato un solo calcolo della materialità.", "Attenzione" );
                return false;
            }

			if (tmpNode_true != null)
			{
                foreach (DataRow dtrow in tmpNode_true.Rows)
                {
                    if (dtrow["ID"].ToString() == "txt7BILANCIO")
                        txt7 = dtrow["value"].ToString();
                    if (dtrow["ID"].ToString() == "txt7_2spBILANCIO")
                        txt7_2sp = dtrow["value"].ToString();
                    if (dtrow["ID"].ToString() == "txt7_2ceBILANCIO")
                        txt7_2ce = dtrow["value"].ToString();
                    if (dtrow["ID"].ToString() == "txt7_3spBILANCIO")
                        txt7_3sp = dtrow["value"].ToString();
                    if (dtrow["ID"].ToString() == "txt7_3ecBILANCIO")
                        txt7_3ec = dtrow["value"].ToString();
                    if (dtrow["ID"].ToString() == "txt9BILANCIO")
                        txt9 = dtrow["value"].ToString();
                    if (dtrow["ID"].ToString() == "txt9_2spBILANCIO")
                        txt9_2sp = dtrow["value"].ToString();
                    if (dtrow["ID"].ToString() == "txt9_2ceBILANCIO")
                        txt9_2ec = dtrow["value"].ToString();
                    if (dtrow["ID"].ToString() == "txt9_3spBILANCIO")
                        txt9_3sp = dtrow["value"].ToString();
                    if (dtrow["ID"].ToString() == "txt9_3ecBILANCIO")
                        txt9_3ec = dtrow["value"].ToString();
                }

			}
			#endregion

			#region MATERIALITA' CALCOLO
			//double dblValore = 0.0;

			//double TotMin = 0.0;
			//double TotMax = 0.0;

			//double TotMinSP = 0.0;
			//double TotMaxSP = 0.0;
			//double TotMinEC = 0.0;
			//double TotMaxEC = 0.0;

			//dblValore = 0.0;
			//dblValore = txt1;

			//double txt1min = ((dblValore * 0.5 / 100.0));
			//double txt1lmindn = (dblValore * 0.5 / 100.0);
			//TotMinSP += dblValore * 0.5 / 100.0;
			//double txt1lmax = (dblValore * 1.0 / 100.0);
			//double txt1lmaxdn = (dblValore * 1.0 / 100.0);
			//TotMaxSP += dblValore * 1.0 / 100.0;

			//dblValore = 0.0;
			//dblValore = txt2;

			//double txt2lmin = ((dblValore * 1.0 / 100.0));
			//double txt2lmindn = ((dblValore * 1.0 / 100.0));
			//TotMinSP += dblValore * 1.0 / 100.0;
			//double txt2lmax = ((dblValore * 5.0 / 100.0));
			//double txt2lmaxdn = ((dblValore * 5.0 / 100.0));
			//TotMaxSP += dblValore * 5.0 / 100.0;

			//dblValore = 0.0;
			//dblValore = txt3;

			//double txt3lmin = ((dblValore * 0.5 / 100.0));
			//double txt3lmindn = ((dblValore * 0.5 / 100.0));
			//TotMinEC += dblValore * 0.5 / 100.0;
			//double txt3lmax = ((dblValore * 1.0 / 100.0));
			//double txt3lmaxdn = ((dblValore * 1.0 / 100.0));
			//TotMaxEC += dblValore * 1.0 / 100.0;

			//dblValore = 0.0;
			//dblValore = txt4;

			//double txt4lmin = ((dblValore * 5.0 / 100.0));
			//double txt4lmindn = ((dblValore * 5.0 / 100.0));
			//TotMinEC += dblValore * 5.0 / 100.0;
			//double txt4lmax = ((dblValore * 10.0 / 100.0));
			//double txt4lmaxdn = ((dblValore * 10.0 / 100.0));
			//TotMaxEC += dblValore * 10.0 / 100.0;

			//TotMin = TotMinSP + TotMinEC;
			//TotMax = TotMaxSP + TotMaxEC;
			#endregion

			RowDefinition rd;
			Border brd;
			TextBlock txt;
			int row = 3;

			RevisoftApplication.XmlManager xt = new XmlManager();
			xt.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
			XmlDataProvider TreeXmlProvider = new XmlDataProvider();
			TreeXmlProvider.Document = xt.LoadEncodedFile(App.AppTemplateTreeBilancio);

			//Controllo degli indici pro salvataggio AP
			int indice = 0;
         
            DataTable datiErrori = cBusinessObjects.GetData(-1, typeof(Excel_ErroriRilevati), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);
            datiErrori.Columns.Add("ID", typeof(int));
            int j = 0;
            foreach (DataRow item in datiErrori.Rows)
            {
                item["ID"] = j;
                j++;
            }


                if (true)
            {

            

                 foreach ( DataRow item in datiErrori.Rows )
                {
                    int indiceinterno = 0;

                    if ( item["ID"].ToString() != "" )
                    {
                        int.TryParse( item["ID"].ToString(), out indiceinterno );
                    }

                    if ( indiceinterno > indice )
                    {
                        indice = indiceinterno;
                    }
                }

                Dictionary<int, DataRow> lista = new Dictionary<int, DataRow>();

                foreach (DataRow item in datiErrori.Rows)
                {
                    if ( item["name"].ToString() == "Totale" )
                    {
                        continue;
                    }
                 

                    XmlNode tnode = TreeXmlProvider.Document.SelectSingleNode( "/Tree//Node[@ID=" + cBusinessObjects.Gest_ID_SCHEDA(item["ID_SCHEDA"].ToString(),4) + "]" );
                    if ( tnode == null )
                    {
                        continue;
                    }

                    int chiave = Convert.ToInt32( tnode.Attributes["Codice"].Value.Replace( ".", "" ).Replace( "A", "" ).Replace( "B", "" ).Replace( "C", "" ).Replace( "D", "" ) );

                    while ( lista.Keys.Contains( chiave ) )
                    {
                        chiave = chiave + 1;
                    }


                    lista.Add( chiave, item );
                }

                bool first = true;
                                
                int indiceinternoRow = 0;

                foreach (KeyValuePair<int, DataRow> itemD in lista.OrderBy(key => key.Key))
                {
                    DataRow item = itemD.Value;

                   

                    XmlNode tnode = TreeXmlProvider.Document.SelectSingleNode( "/Tree//Node[@ID=" + cBusinessObjects.Gest_ID_SCHEDA(item["ID_SCHEDA"].ToString(), 4) + "]" );


                    rd = new RowDefinition();
                    if ( first )
                    {
                        first = false;
                        rd.Height = new GridLength( 0.0 );
                    }
                    else
                    {
                        rd.Height = new GridLength( 20.0 );
                    }
                    grdMain.RowDefinitions.Add( rd );
                    row++;

                    
                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    indiceinternoRow++;
                    indicerow.Add( indiceinternoRow, row.ToString() ); 

                    txt = new TextBlock();
                    txt.Name = "txtCodice" + row.ToString();
                    this.RegisterName( txt.Name, txt );
                    txt.Text = tnode.ParentNode.Attributes["Codice"].Value;
                    txt.ToolTip = tnode.ParentNode.Attributes["Titolo"].Value;
                    txt.TextAlignment = TextAlignment.Left;
                    txt.Padding = new Thickness( 3, 0, 3, 0 );
                    txt.TextWrapping = TextWrapping.Wrap;
                    txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    txt.MaxHeight = Convert.ToInt32( tnode.ParentNode.Attributes["ID"].Value );
                    txt.MouseDown += new MouseButtonEventHandler( txt_MouseDown );

                    brd.Child = txt;
                    
                    grdMain.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 0 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    
                    txt.Text = (item["name"].ToString().Length > 30) ? item["name"].ToString().Substring( 0, 30 ) + "[...]" :item["name"].ToString();
                    txt.ToolTip = item["name"].ToString();
                    txt.Name = "txtName" + row.ToString();
                    txt.Margin = new Thickness( 0, 0, 0, 0 );
                    txt.Padding = new Thickness( 3, 0, 3, 0 );
                    txt.TextAlignment = TextAlignment.Left;
                    txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    this.RegisterName( txt.Name, txt );

                    brd.Child = txt;

                    grdMain.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 1 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.Background = Brushes.White;
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Text = ConvertNumber( item["importo"].ToString());
                    txt.Name = "txtEA" + row.ToString();
                    this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;
                    txt.Padding = new Thickness( 3, 0, 3, 0 );

                    brd.Child = txt;

                    grdMain.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 2 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Name = "txtAP" + row.ToString();
                    txt.Tag = "txtAP" + item["ID"].ToString();


                    DataRow tmpNodeAP = null;
                    foreach(DataRow dd in dati.Rows)
                    {
                        if(row.ToString() == dd["row"].ToString())
                        tmpNodeAP = dd;
                    }
                

                    if ( tmpNodeAP != null && tmpNodeAP["txtAP" ].ToString() != "" )
                    {
                        txt.Text = ConvertNumber( tmpNodeAP["txtAP" ].ToString());
                    }

                    if ( item["importoAP"].ToString() != "" )
                    {
                        txt.Text = ConvertNumber( item["importoAP"].ToString());
                    }

                    //txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler( obj_PreviewMouseLeftButtonDown );
                    //txt.PreviewKeyDown += new KeyEventHandler( obj_PreviewKeyDown );
                    //txt.TextChanged += new TextChangedEventHandler( TextBox_TextChanged );
                    //txt.LostFocus += new RoutedEventHandler( txt1_LostFocus );
                    this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;
                    txt.Padding = new Thickness( 3, 0, 3, 0 );
             
                    brd.Child = txt;

                    grdMain.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 3 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.LightYellow;

                    txt = new TextBlock();
                    txt.Text = ConvertNumber( item["importo"].ToString());
                    txt.Name = "txtPN" + row.ToString();
                    this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;
                    txt.Padding = new Thickness( 3, 0, 3, 0 );

                    brd.Child = txt;

                    grdMain.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 4 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness(1.0, 0, 0, 1.0);
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.LightYellow;

                    txt = new TextBlock();
                    txt.Text = ConvertNumber(((item["impattofiscalePN"].ToString() != null) ? item["impattofiscalePN"].ToString() : "0"));
                    txt.Name = "txtIMPOSTEPN" + row.ToString();
                    this.RegisterName(txt.Name, txt);
                    txt.TextAlignment = TextAlignment.Right;
                    txt.Padding = new Thickness(3, 0, 3, 0);

                    brd.Child = txt;

                    grdMain.Children.Add(brd);
                    Grid.SetRow(brd, row);
                    Grid.SetColumn(brd, 5);

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.PaleGreen;

                    txt = new TextBlock();
                    txt.Name = "txtDIFF" + row.ToString();
                    this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;
                    txt.Padding = new Thickness( 3, 0, 3, 0 );

                    brd.Child = txt;

                    grdMain.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 6 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.PaleGreen;

                    txt = new TextBlock();
                    txt.Text = ConvertNumber( ((item["impattofiscale"].ToString() != "") ? item["impattofiscale"].ToString() : "0") );
                    txt.Name = "txtIMPOSTE" + row.ToString();
                    this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;
                    txt.Padding = new Thickness( 3, 0, 3, 0 );

                    brd.Child = txt;

                    grdMain.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 7 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 1.0, 1.0 );
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.White;

                    CheckBox chk = new CheckBox();
                    chk.Name = "chkIrrilevante" + row.ToString();
                    this.RegisterName( chk.Name, chk );
                    chk.IsChecked = Convert.ToBoolean( ( ( item["corretto"].ToString() != "" ) ? item["corretto"].ToString() : "False" ) );

                    //if ( tmpNodeAP != null && tmpNodeAP.Attributes["chkIrrilevante" + row] != null )
                    //{
                    //    chk.IsChecked = Convert.ToBoolean( tmpNodeAP.Attributes["chkIrrilevante" + row].Value );
                    //}

                    //chk.Checked += new RoutedEventHandler( chk_Checked );
                    //chk.Unchecked += new RoutedEventHandler( chk_Checked );
                    chk.IsHitTestVisible = false;
                    chk.PreviewMouseLeftButtonDown += new MouseButtonEventHandler( obj_PreviewMouseLeftButtonDown2 );
                    chk.PreviewKeyDown += new KeyEventHandler( obj_PreviewKeyDown2 );
                    chk.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    chk.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    chk.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;

                    brd.Child = chk;

                    grdMain.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 8 );
                }
            

			rowTOT = row;

            rd = new RowDefinition();
            rd.Height = new GridLength( 20.0 );
            grdMain.RowDefinitions.Add( rd );

			rd = new RowDefinition();
			rd.Height = new GridLength(30.0);
			grdMain.RowDefinitions.Add(rd);
			row++;

            brd = new Border();
            brd.BorderThickness = new Thickness( 0.0 );
            brd.BorderBrush = Brushes.Black;
            brd.Background = Brushes.White;

            txt = new TextBlock();
            txt.Text = "TOTALE DELLE RETTIFICHE";
            txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            txt.Height = 30;
            txt.Margin = new Thickness( 0, 0, 20, 0 );
            txt.FontWeight = FontWeights.Bold;

            txt.Padding = new Thickness( 0, 7, 3, 0 );
            brd.Child = txt;

            grdMain.Children.Add( brd );
            Grid.SetRow( brd, row );
            Grid.SetColumn( brd, 1 );

			brd = new Border();
            brd.BorderThickness = new Thickness( 1.0, 0, 0.0, 1.0 );
			brd.BorderBrush = Brushes.Black;
			brd.Background = Brushes.White;

			txt = new TextBlock();
			txt.Name = "txtTotEA";
			this.RegisterName(txt.Name, txt);
            txt.TextAlignment = TextAlignment.Right;
            txt.Padding = new Thickness( 3, 0, 3, 0 );
            txt.FontWeight = FontWeights.Bold;

            txt.Padding = new Thickness( 0, 7, 3, 0 );
            txt.Height = 30;
			brd.Child = txt;

			grdMain.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 2);

            brd = new Border();
            brd.BorderThickness = new Thickness( 1.0, 0, 0.0, 1.0 );
            brd.BorderBrush = Brushes.Black;
            brd.Background = Brushes.White;

            txt = new TextBlock();
            txt.Name = "txtTotAP";
            this.RegisterName( txt.Name, txt );
            txt.TextAlignment = TextAlignment.Right;
            txt.FontWeight = FontWeights.Bold;

            txt.Padding = new Thickness( 0, 7, 3, 0 );
            txt.Height = 30;
            brd.Child = txt;

            grdMain.Children.Add( brd );
            Grid.SetRow( brd, row );
            Grid.SetColumn( brd, 3 );

            brd = new Border();
            brd.BorderThickness = new Thickness( 1.0, 0, 0.0, 1.0 );
            brd.BorderBrush = Brushes.Black;
            brd.Background = Brushes.LightYellow;

            txt = new TextBlock();
            txt.Name = "txtTotPN";
            this.RegisterName( txt.Name, txt );
            txt.TextAlignment = TextAlignment.Right;
            txt.FontWeight = FontWeights.Bold;

            txt.Padding = new Thickness( 0, 7, 3, 0 );
            txt.Height = 30;
            brd.Child = txt;

            grdMain.Children.Add( brd );
            Grid.SetRow( brd, row );
            Grid.SetColumn( brd, 4 );

            brd = new Border();
            brd.BorderThickness = new Thickness(1.0, 0, 0.0, 1.0);
            brd.BorderBrush = Brushes.Black;
            brd.Background = Brushes.LightYellow;

            txt = new TextBlock();
            txt.Name = "txtTotIMPOSTEPN";
            this.RegisterName(txt.Name, txt);
            txt.TextAlignment = TextAlignment.Right;
            txt.FontWeight = FontWeights.Bold;

            txt.Padding = new Thickness(0, 7, 3, 0);
            txt.Height = 30;
            brd.Child = txt;

            grdMain.Children.Add(brd);
            Grid.SetRow(brd, row);
            Grid.SetColumn(brd, 5);

            brd = new Border();
            brd.BorderThickness = new Thickness( 1.0, 0, 0.0, 1.0 );
			brd.BorderBrush = Brushes.Black;
			brd.Background = Brushes.PaleGreen;

			txt = new TextBlock();
			txt.Name = "txtTotDIFF";
			this.RegisterName(txt.Name, txt);
			txt.TextAlignment = TextAlignment.Right;
            txt.FontWeight = FontWeights.Bold;

            txt.Padding = new Thickness( 0, 7, 3, 0 );
            txt.Height = 30;
			brd.Child = txt;

			grdMain.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 6);

            brd = new Border();
            brd.BorderThickness = new Thickness( 1.0, 0.0, 1.0, 1.0 );
            brd.BorderBrush = Brushes.Black;
            brd.Background = Brushes.PaleGreen;

            txt = new TextBlock();
            txt.Name = "txtTotIMPOSTE";
            this.RegisterName( txt.Name, txt );
            txt.TextAlignment = TextAlignment.Right;
            txt.FontWeight = FontWeights.Bold;

            txt.Padding = new Thickness( 0, 7, 3, 0 );
            txt.Height = 30;
            brd.Child = txt;

            grdMain.Children.Add( brd );
            Grid.SetRow( brd, row );
            Grid.SetColumn( brd, 7 );

            rd = new RowDefinition();
            rd.Height = new GridLength( 30.0 );
			grdMain.RowDefinitions.Add(rd);
			row++;




                //NUOVO TOTALE
                brd = new Border();
                brd.BorderThickness = new Thickness(1.0, 0, 0.0, 1.0);
                brd.BorderBrush = Brushes.Black;
                brd.Background = Brushes.LightYellow;

                txt = new TextBlock();
                txt.Name = "txtTotTotPN";
                this.RegisterName(txt.Name, txt);
                txt.TextAlignment = TextAlignment.Center;
                txt.FontWeight = FontWeights.Bold;

                txt.Padding = new Thickness(0, 7, 3, 0);
                txt.Height = 30;
                brd.Child = txt;

                grdMain.Children.Add(brd);
                Grid.SetRow(brd, row);
                Grid.SetColumnSpan(brd, 2);
                Grid.SetColumn(brd, 4);

                brd = new Border();
                brd.BorderThickness = new Thickness(1.0, 0, 1.0, 1.0);
                brd.BorderBrush = Brushes.Black;
                brd.Background = Brushes.PaleGreen;

                txt = new TextBlock();
                txt.Name = "txtTotTotCE";
                this.RegisterName(txt.Name, txt);
                txt.TextAlignment = TextAlignment.Center;
                txt.FontWeight = FontWeights.Bold;

                txt.Padding = new Thickness(0, 7, 3, 0);
                txt.Height = 30;
                brd.Child = txt;

                grdMain.Children.Add(brd);
                Grid.SetRow(brd, row);
                Grid.SetColumnSpan(brd, 2);
                Grid.SetColumn(brd, 6);

                rd = new RowDefinition();
                rd.Height = new GridLength(30.0);
                grdMain.RowDefinitions.Add(rd);
                row++;











                brd = new Border();

			txt = new TextBlock();
			txt.TextAlignment = TextAlignment.Center;

			brd.Child = txt;

			grdMain.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 2);

			brd = new Border();

			txt = new TextBlock();
			txt.TextAlignment = TextAlignment.Center;

            txt.Padding = new Thickness( 0, 7, 3, 0 );
            txt.Height = 30;
			brd.Child = txt;

			grdMain.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 4);

			rd = new RowDefinition();
			rd.Height = new GridLength(30.0);
			grdMain.RowDefinitions.Add(rd);
			row++;

            brd = new Border();

			txt = new TextBlock();
			txt.Text = "LIMITE DI MATERIALITA' / SIGNIFICATIVITA' di Bilancio";
            txt.FontWeight = FontWeights.Bold;
			txt.TextAlignment = TextAlignment.Right;
            txt.Margin = new Thickness( 0, 0, 20, 0 );

            txt.Padding = new Thickness( 0, 7, 3, 0 );
            txt.Height = 30;
			brd.Child = txt;

			grdMain.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 0);
			Grid.SetColumnSpan(brd, 4);
            
			brd = new Border();

            brd.BorderThickness = new Thickness( 1.0, 1.0, 0.0, 1.0 );
			brd.BorderBrush = Brushes.Black;
            brd.Background = Brushes.White;

			txt = new TextBlock();
			txt.Name = "txtTotMaterialitaSP";
			this.RegisterName(txt.Name, txt);
			txt.TextAlignment = TextAlignment.Right;
            txt.FontWeight = FontWeights.Bold;

            txt.Padding = new Thickness( 0, 7, 3, 0 );
            txt.Height = 30;
			brd.Child = txt;

			grdMain.Children.Add(brd);
			Grid.SetRow(brd, row);
                Grid.SetColumnSpan(brd, 2);
			Grid.SetColumn(brd, 4);

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
            brd.BorderBrush = Brushes.Black;
            brd.Background = Brushes.White;

			txt = new TextBlock();
			txt.Name = "txtTotMaterialitaCE";
			this.RegisterName(txt.Name, txt);
            txt.TextAlignment = TextAlignment.Right;
            txt.FontWeight = FontWeights.Bold;

            txt.Padding = new Thickness( 0, 7, 3, 0 );
            txt.Height = 30;
			brd.Child = txt;

			grdMain.Children.Add(brd);
			Grid.SetRow(brd, row);
                Grid.SetColumnSpan(brd, 2);
                Grid.SetColumn(brd, 6);

           
            rd = new RowDefinition();
            rd.Height = new GridLength( 40.0 );
            grdMain.RowDefinitions.Add( rd );
            row++;
            
			brd = new Border();
            brd.Margin = new Thickness( 0,10,0,0 );

			txt = new TextBlock();
			txt.Text = "ECCEDENZA RISPETTO ALLA MATERIALITA'";
            txt.FontWeight = FontWeights.Bold;
            txt.TextAlignment = TextAlignment.Right;
            txt.Margin = new Thickness( 0, 0, 20, 0 );

            txt.Padding = new Thickness( 0, 7, 3, 0 );
            txt.Height = 30;
			brd.Child = txt;

			grdMain.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 0);
			Grid.SetColumnSpan(brd, 4);

            brd = new Border();

            brd.BorderThickness = new Thickness( 1.0, 1.0, 0.0, 1.0 );
            brd.BorderBrush = Brushes.Black;
            brd.Background = Brushes.White;
            brd.Margin = new Thickness( 0, 10, 0, 0 );

            txt = new TextBlock();
            txt.Name = "txtTotEccedenzaSP";
            this.RegisterName( txt.Name, txt );
            txt.TextAlignment = TextAlignment.Right;
            txt.FontWeight = FontWeights.Bold;

            txt.Padding = new Thickness( 0, 7, 3, 0 );
            txt.Height = 30;
            brd.Child = txt;

            grdMain.Children.Add( brd );
            Grid.SetRow( brd, row );
                Grid.SetColumnSpan(brd, 2);
                Grid.SetColumn( brd, 4 );

            brd = new Border();
            brd.BorderThickness = new Thickness( 1.0 );
            brd.BorderBrush = Brushes.Black;
            brd.Background = Brushes.White;
            brd.Margin = new Thickness( 0, 10, 0, 0 );

            txt = new TextBlock();
            txt.Name = "txtTotEccedenzaCE";
            this.RegisterName( txt.Name, txt );
            txt.TextAlignment = TextAlignment.Right;
            txt.FontWeight = FontWeights.Bold;

            txt.Padding = new Thickness( 0, 7, 3, 0 );
            txt.Height = 30;
            brd.Child = txt;

            grdMain.Children.Add( brd );
            Grid.SetRow( brd, row );
                Grid.SetColumnSpan(brd, 2);
                Grid.SetColumn( brd, 6 );






            row = 2;
            indiceinternoRow = 0;

            foreach (KeyValuePair<int, DataRow> itemD in lista.OrderBy(key => key.Key))
                {
                    DataRow item = itemD.Value;
                    //MM RIVEDERE
                    XmlNode tnode = TreeXmlProvider.Document.SelectSingleNode( "/Tree//Node[@ID=" + cBusinessObjects.Gest_ID_SCHEDA(item["ID_SCHEDA"].ToString(), 4) + "]" );

                  


                    rd = new RowDefinition();
                    if ( first )
                    {
                        first = false;
                        rd.Height = new GridLength( 0.0 );
                    }
                    else
                    {
                        rd.Height = new GridLength( 20.0 );
                    }
                    grdMainET.RowDefinitions.Add( rd );
                    row++;

                    
                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    indiceinternoRow++;

                    txt = new TextBlock();
                    txt.Name = "txtCodiceX" + indicerow[indiceinternoRow].ToString();
                    this.RegisterName( txt.Name, txt );
                    txt.Text = tnode.ParentNode.Attributes["Codice"].Value;
                    txt.ToolTip = tnode.ParentNode.Attributes["Titolo"].Value;
                    txt.TextAlignment = TextAlignment.Left;
                    txt.Padding = new Thickness( 3, 0, 3, 0 );
                    txt.TextWrapping = TextWrapping.Wrap;
                    txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    txt.MaxHeight = Convert.ToInt32( tnode.ParentNode.Attributes["ID"].Value );
                    txt.MouseDown += new MouseButtonEventHandler( txt_MouseDown );

                    brd.Child = txt;
                    
                    grdMainET.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 0 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Text = (item["name"].ToString().Length > 30) ? item["name"].ToString().Substring( 0, 30 ) + "[...]" : item["name"].ToString();
                    txt.ToolTip = item["name"].ToString();
                    txt.Name = "txtNameX" + indicerow[indiceinternoRow].ToString();
                    txt.Margin = new Thickness( 0, 0, 0, 0 );
                    txt.Padding = new Thickness( 3, 0, 3, 0 );
                    txt.TextAlignment = TextAlignment.Left;
                    txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    this.RegisterName( txt.Name, txt );

                    brd.Child = txt;

                    grdMainET.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 1 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.Background = Brushes.White;
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Text = ConvertNumber( item["importo"].ToString());
                    txt.TextAlignment = TextAlignment.Right;
                    txt.Padding = new Thickness( 3, 0, 3, 0 );

                    brd.Child = txt;

                    grdMainET.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 2 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    TextBox txta = new TextBox();
                    txta.Name = "txtAPX" + indicerow[indiceinternoRow].ToString();
                    txta.BorderThickness = new Thickness( 0 );
                    txta.IsReadOnly = true;
                    this.RegisterName( txta.Name, txta );
                    txta.TextAlignment = TextAlignment.Right;

                    brd.Child = txta;

                    grdMainET.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 3 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness(1.0, 0, 0, 1.0);
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.LightYellow;

                    txt = new TextBlock();
                    txt.Name = "txtNETTOPN" + indicerow[indiceinternoRow].ToString();
                    this.RegisterName(txt.Name, txt);
                    txt.TextAlignment = TextAlignment.Right;
                    txt.Padding = new Thickness(3, 0, 3, 0);

                    brd.Child = txt;

                    grdMainET.Children.Add(brd);
                    Grid.SetRow(brd, row);
                    Grid.SetColumn(brd, 4);

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.LightYellow;

                    txt = new TextBlock();
                    txt.Name = "txtSP" + indicerow[indiceinternoRow].ToString();
                    this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;
                    txt.Padding = new Thickness( 3, 0, 3, 0 );

                    brd.Child = txt;

                    grdMainET.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 5 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness(1.0, 0, 0, 1.0);
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.PaleGreen;

                    txt = new TextBlock();
                    txt.Name = "txtNETTOCE" + indicerow[indiceinternoRow].ToString();
                    this.RegisterName(txt.Name, txt);
                    txt.TextAlignment = TextAlignment.Right;
                    txt.Padding = new Thickness(3, 0, 3, 0);

                    brd.Child = txt;

                    grdMainET.Children.Add(brd);
                    Grid.SetRow(brd, row);
                    Grid.SetColumn(brd, 6);

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0.0, 1.0 );
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.PaleGreen;

                    txt = new TextBlock();
                    txt.Name = "txtCE" + indicerow[indiceinternoRow].ToString();
                    this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;
                    txt.Padding = new Thickness( 3, 0, 3, 0 );

                    brd.Child = txt;

                    grdMainET.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 7 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness(1.0, 0, 1.0, 1.0);
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.White;

                    CheckBox chk = new CheckBox();
                    chk.Name = "chkIrrilevanteX" + row.ToString();
                    this.RegisterName(chk.Name, chk);
                    chk.IsChecked = Convert.ToBoolean(((item["corretto"].ToString() != "") ? item["corretto"].ToString() : "False"));
                    chk.IsHitTestVisible = false;
                    chk.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    chk.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    chk.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;

                    brd.Child = chk;

                    grdMainET.Children.Add(brd);
                    Grid.SetRow(brd, row);
                    Grid.SetColumn(brd, 8);
                }
            }

			rowTOT = row;

            rd = new RowDefinition();
            rd.Height = new GridLength( 30.0 );
            grdMainET.RowDefinitions.Add( rd );

			rd = new RowDefinition();
			rd.Height = new GridLength(30.0);
			grdMainET.RowDefinitions.Add(rd);
			row++;

            rd = new RowDefinition();
            rd.Height = new GridLength( 30.0 );
            grdMainET.RowDefinitions.Add( rd );
            row++;

            brd = new Border();

            txt = new TextBlock();
            txt.Text = "ERRORE TRASCURABILE";
            txt.FontWeight = FontWeights.Bold;
            txt.TextAlignment = TextAlignment.Right;
            txt.Margin = new Thickness( 0, 0, 20, 0 );

            txt.Padding = new Thickness( 0, 7, 3, 0 );
            txt.Height = 30;
            brd.Child = txt;

            grdMainET.Children.Add( brd );
            Grid.SetRow( brd, row );
            Grid.SetColumn( brd, 1 );
            Grid.SetColumnSpan( brd, 3 );

            brd = new Border();

            brd.BorderThickness = new Thickness( 1.0, 1.0, 0.0, 1.0 );
            brd.BorderBrush = Brushes.Black;
            brd.Background = Brushes.White;

            txtErroreTollerabileSP = new TextBlock();
            txtErroreTollerabileSP.TextAlignment = TextAlignment.Right;
            txtErroreTollerabileSP.FontWeight = FontWeights.Bold;

            txtErroreTollerabileSP.Height = 30;
            txtErroreTollerabileSP.Padding = new Thickness( 0, 7, 3, 0 );
            brd.Child = txtErroreTollerabileSP;

            grdMainET.Children.Add( brd );
            Grid.SetRow( brd, row );
            Grid.SetColumnSpan(brd, 2);
            Grid.SetColumn( brd, 4 );

            brd = new Border();
            brd.BorderThickness = new Thickness( 1.0 );
            brd.BorderBrush = Brushes.Black;
            brd.Background = Brushes.White;

            txtErroreTollerabileCE = new TextBlock();
            txtErroreTollerabileCE.TextAlignment = TextAlignment.Right;
            txtErroreTollerabileCE.FontWeight = FontWeights.Bold;

            txtErroreTollerabileCE.Height = 30;
            txtErroreTollerabileCE.Padding = new Thickness( 0, 7, 3, 0 );
            brd.Child = txtErroreTollerabileCE;

            grdMainET.Children.Add( brd );
            Grid.SetRow( brd, row );
            Grid.SetColumnSpan(brd, 2);
            Grid.SetColumn( brd, 6 );

    		#region Inserimento dati da MATERIALITA' nelle celle locali
			TextBlock txtTotMaterialitaSP = (TextBlock)this.FindName("txtTotMaterialitaSP");
			TextBlock txtTotMaterialitaCE = (TextBlock)this.FindName("txtTotMaterialitaCE");

			if (Materialità_1)
			{
                txtErroreTollerabileSP.Text = ConvertNumber(txt9);
                txtErroreTollerabileCE.Text = ConvertNumber(txt9);

				txtTotMaterialitaSP.Text = ConvertNumber(txt7);
				txtTotMaterialitaCE.Text = ConvertNumber(txt7); 
			}

			//Seconda ipotesi
			if (Materialità_2)
			{				
                txtErroreTollerabileSP.Text = ConvertNumber(txt9_2sp);
                txtErroreTollerabileCE.Text = ConvertNumber(txt9_2ec);

				txtTotMaterialitaSP.Text = ConvertNumber(txt7_2sp);
				txtTotMaterialitaCE.Text = ConvertNumber(txt7_2ce); 				
			}

			//Terza ipotesi
			if (Materialità_3)
			{
                txtErroreTollerabileSP.Text = ConvertNumber(txt9_3sp);
                txtErroreTollerabileCE.Text = ConvertNumber(txt9_3ec);

				txtTotMaterialitaSP.Text = ConvertNumber(txt7_3sp);
				txtTotMaterialitaCE.Text = ConvertNumber(txt7_3ec);
			}
			#endregion


            double totaleimporto = 0.0;
            row = 3;
            indice = 0;

            if ( true)
            {
                DataTable datiErroriRilevatiMR = cBusinessObjects.GetData(-1, typeof(Excel_ErroriRilevatiMR), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);

                datiErroriRilevatiMR.Columns.Add("ID", typeof(int));
                int j2 = 0;
                foreach (DataRow item in datiErroriRilevatiMR.Rows)
                {
                    item["ID"] = j2;
                    j2++;
                }
                foreach ( DataRow item in datiErroriRilevatiMR.Rows)
                {
                    int indiceinterno = 0;

                    if ( item["ID"].ToString() != "" )
                    {
                        int.TryParse( item["ID"].ToString(), out indiceinterno );
                    }

                    if ( indiceinterno > indice )
                    {
                        indice = indiceinterno;
                    }
                }

                Dictionary<int, DataRow> lista = new Dictionary<int, DataRow>();

                foreach (DataRow item in datiErroriRilevatiMR.Rows)
                {
                    if ( item["name"].ToString() == "Totale" )
                    {
                        continue;
                    }
                    //MM RIVEDERE item.ParentNode.Attributes["ID"].Value
                    XmlNode tnode = TreeXmlProvider.Document.SelectSingleNode( "/Tree//Node[@ID=" + cBusinessObjects.Gest_ID_SCHEDA(item["ID_SCHEDA"].ToString(), 4) + "]" );
                    if ( tnode == null )
                    {
                        continue;
                    }

                    int chiave = Convert.ToInt32( tnode.Attributes["Codice"].Value.Replace( ".", "" ).Replace( "A", "" ).Replace( "B", "" ).Replace( "C", "" ).Replace( "D", "" ) );

                    while ( lista.Keys.Contains( chiave ) )
                    {
                        chiave = chiave + 1;
                    }


                    lista.Add( chiave, item );
                }

                bool first = true;

                int indiceinternoRow = 0;

                foreach ( KeyValuePair<int, DataRow> itemD in lista.OrderBy( key => key.Key ) )
                {
                    DataRow item = itemD.Value;
                    //MM RIVEDERE
                    XmlNode tnode = TreeXmlProvider.Document.SelectSingleNode( "/Tree//Node[@ID=" + cBusinessObjects.Gest_ID_SCHEDA(item["ID_SCHEDA"].ToString(), 4) + "]" );

                   

                    rd = new RowDefinition();
                    if ( first )
                    {
                        first = false;
                        rd.Height = new GridLength( 0.0 );
                    }
                    else
                    {
                        rd.Height = new GridLength( 20.0 );
                    }
                    grdMainMR.RowDefinitions.Add( rd );
                    row++;


                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    indiceinternoRow++;
                    //indicerow.Add( indiceinternoRow, row.ToString() );

                    txt = new TextBlock();
                    txt.Name = "txtCodice" + row.ToString();
                    //this.RegisterName( txt.Name, txt );
                    txt.Text = tnode.ParentNode.Attributes["Codice"].Value;              
                    txt.ToolTip = tnode.ParentNode.Attributes["Titolo"].Value;
                    txt.TextAlignment = TextAlignment.Left;
                    txt.Padding = new Thickness( 3, 0, 3, 0 );
                    txt.TextWrapping = TextWrapping.Wrap;
                    txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    txt.MaxHeight = Convert.ToInt32( tnode.ParentNode.Attributes["ID"].Value );
                    txt.MouseDown += new MouseButtonEventHandler( txt_MouseDown );

                    brd.Child = txt;

                    grdMainMR.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 0 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Text = ( item["name"].ToString().Length > 30 ) ? item["name"].ToString().Substring( 0, 30 ) + "[...]" : item["name"].ToString();
                    txt.ToolTip = item["name"].ToString();
                    txt.Name = "txtName" + row.ToString();
                    txt.Margin = new Thickness( 0, 0, 0, 0 );
                    txt.Padding = new Thickness( 3, 0, 3, 0 );
                    txt.TextAlignment = TextAlignment.Left;
                    txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    //this.RegisterName( txt.Name, txt );

                    brd.Child = txt;

                    grdMainMR.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 1 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Text = item["contoimputato"].ToString();
                    txt.Name = "txtcontoimputato" + row.ToString();
                    //this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;
                    txt.Padding = new Thickness( 3, 0, 3, 0 );

                    brd.Child = txt;

                    grdMainMR.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 2 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Text = item["contoproposto"].ToString();
                    txt.Name = "txtcontoproposto" + row.ToString();
                    //this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;
                    txt.Padding = new Thickness( 3, 0, 3, 0 );

                    brd.Child = txt;

                    grdMainMR.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 3 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Text = ConvertNumber( item["importo"].ToString());

                    double valueimporto = 0;
                    double.TryParse( txt.Text, out valueimporto );
                    totaleimporto += valueimporto;

                    txt.Name = "txtEA" + row.ToString();
                    //this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;
                    txt.Padding = new Thickness( 3, 0, 3, 0 );

                    brd.Child = txt;

                    grdMainMR.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 4 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 1.0, 1.0 );
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.White;

                    CheckBox chk = new CheckBox();
                    chk.Name = "chkCorretto" + row.ToString();
                    //this.RegisterName( chk.Name, chk );
                    if (item["corretto"].ToString() != "")
                        chk.IsChecked = Convert.ToBoolean(item["corretto"].ToString());
                    else
                        chk.IsChecked = false;
                   
                    chk.IsHitTestVisible = false;

                    chk.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    chk.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    chk.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;

                    brd.Child = chk;

                    grdMainMR.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 5 );
                }

                rd = new RowDefinition();
                rd.Height = new GridLength( 20.0 );
                grdMainMR.RowDefinitions.Add( rd );

                rd = new RowDefinition();
                rd.Height = new GridLength( 30.0 );
                grdMainMR.RowDefinitions.Add( rd );
                row++;

                brd = new Border();
                brd.BorderThickness = new Thickness( 0.0 );
                brd.BorderBrush = Brushes.Black;
                brd.Background = Brushes.White;

                txt = new TextBlock();
                txt.Text = "TOTALE DELLE RETTIFICHE";
                txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                txt.Height = 30;
                txt.Margin = new Thickness( 0, 0, 20, 0 );
                txt.FontWeight = FontWeights.Bold;

                txt.Padding = new Thickness( 0, 7, 3, 0 );
                brd.Child = txt;

                grdMainMR.Children.Add( brd );
                Grid.SetRow( brd, row );
                Grid.SetColumn( brd, 1 );

                brd = new Border();
                brd.BorderThickness = new Thickness( 1.0, 0, 1.0, 1.0 );
                brd.BorderBrush = Brushes.Black;

                txt = new TextBlock();
                txt.Name = "txtTotEA";
                //this.RegisterName( txt.Name, txt );
                txt.Text = ConvertNumber( totaleimporto.ToString() );
                txt.TextAlignment = TextAlignment.Right;
                txt.FontWeight = FontWeights.Bold;

                txt.Padding = new Thickness( 0, 7, 3, 0 );
                txt.Height = 30;
                brd.Child = txt;

                grdMainMR.Children.Add( brd );
                Grid.SetRow( brd, row );
                Grid.SetColumn( brd, 4 );

            }

            CalculateValues(  );

            indice = 0;
            row = 3;

            if (true )
            {
                DataTable datiErroriRilevatiNN = cBusinessObjects.GetData(-1, typeof(Excel_ErroriRilevatiNN), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);

                datiErroriRilevatiNN.Columns.Add("ID", typeof(int));
                int j3 = 0;
                foreach (DataRow item in datiErroriRilevatiNN.Rows)
                {
                    item["ID"] = j3;
                    j3++;
                }
                foreach ( DataRow item in datiErroriRilevatiNN.Rows)
                {
                    int indiceinterno = 0;

                    if ( item["ID"].ToString() != "" )
                    {
                        int.TryParse( item["ID"].ToString(), out indiceinterno );
                    }

                    if ( indiceinterno > indice )
                    {
                        indice = indiceinterno;
                    }
                }

                Dictionary<int, DataRow> lista = new Dictionary<int, DataRow>();

                foreach (DataRow item in datiErroriRilevatiNN.Rows)
                {
                    if ( item["name"].ToString() == "Totale" )
                    {
                        continue;
                    }
                    //MM RIVEDERE
                    XmlNode tnode = TreeXmlProvider.Document.SelectSingleNode( "/Tree//Node[@ID=" + cBusinessObjects.Gest_ID_SCHEDA(item["ID_SCHEDA"].ToString(), 4) + "]" );
                    if ( tnode == null )
                    {
                        continue;
                    }

                    int chiave = Convert.ToInt32( tnode.Attributes["Codice"].Value.Replace( ".", "" ).Replace( "A", "" ).Replace( "B", "" ).Replace( "C", "" ).Replace( "D", "" ) );

                    while ( lista.Keys.Contains( chiave ) )
                    {
                        chiave = chiave + 1;
                    }


                    lista.Add( chiave, item );
                }

                bool first = true;

                foreach ( KeyValuePair<int, DataRow> itemD in lista.OrderBy( key => key.Key ) )
                {
                    DataRow item = itemD.Value;

                    XmlNode tnode = TreeXmlProvider.Document.SelectSingleNode( "/Tree//Node[@ID=" + cBusinessObjects.Gest_ID_SCHEDA(item["ID_SCHEDA"].ToString(), 4) + "]" );

                 

                    rd = new RowDefinition();
                    if ( first )
                    {
                        first = false;
                        rd.Height = new GridLength( 0.0 );
                    }
                    else
                    {
                        rd.Height = new GridLength( 20.0 );
                    }
                    grdMainNN.RowDefinitions.Add( rd );
                    row++;

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Text = tnode.ParentNode.Attributes["Codice"].Value;
                    txt.ToolTip = tnode.ParentNode.Attributes["Titolo"].Value;
                    txt.TextAlignment = TextAlignment.Left;
                    txt.Padding = new Thickness( 3, 0, 3, 0 );
                    txt.TextWrapping = TextWrapping.Wrap;
                    txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    txt.MaxHeight = Convert.ToInt32( tnode.ParentNode.Attributes["ID"].Value );
                    txt.MouseDown += new MouseButtonEventHandler( txt_MouseDown );

                    brd.Child = txt;

                    grdMainNN.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 0 );

                    

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Text = item["numero"].ToString();
                    txt.Padding = new Thickness( 3, 0, 3, 0 );
                    txt.TextAlignment = TextAlignment.Right;

                    brd.Child = txt;

                    grdMainNN.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 1 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Text = item["name"].ToString(); //( item.Attributes["name"].Value.Length > 30 ) ? item.Attributes["name"].Value.Substring( 0, 30 ) + "[...]" : item.Attributes["name"].Value;
                    txt.ToolTip = item["name"].ToString();
                    txt.Margin = new Thickness( 0, 0, 0, 0 );
                    txt.Padding = new Thickness( 3, 0, 3, 0 );
                    txt.TextAlignment = TextAlignment.Left;
                    txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                    brd.Child = txt;

                    grdMainNN.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 2 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 1.0, 1.0 );
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.White;

                    CheckBox chk = new CheckBox();
                    if (item["corretto"].ToString() != "")
                        chk.IsChecked = Convert.ToBoolean(item["corretto"].ToString());
                    else
                        chk.IsChecked = false;

                   chk.IsHitTestVisible = false;

                    chk.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    chk.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    chk.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;

                    brd.Child = chk;

                    grdMainNN.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 3 );
                }

                rd = new RowDefinition();
                rd.Height = new GridLength( 20.0 );
                grdMainNN.RowDefinitions.Add( rd );
            }

			CalculateValues();


            Image_MouseLeftButtonDown( img1, null);
            Image_MouseLeftButtonDown( img2, null );
            Image_MouseLeftButtonDown( img3, null );

            return true;
        }

        private void chk_Checked( object sender, RoutedEventArgs e )
        {
            CalculateValues(  );
        }

		public int Save()
		{		
			
          	
			CalculateValues();
            foreach (DataRow tmpNode in dati.Rows)
            {

                tmpNode["txtErroreTollerabileSP"] = txtErroreTollerabileSP.Text;
                tmpNode["txtErroreTollerabileCE"] = txtErroreTollerabileCE.Text;
             
            }

            cBusinessObjects.SaveData(id, dati, typeof(Excel_ErroriRilevati_riepilogo));    

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

		private void RetrieveData( string ID)
		{
			if (_x != null)
			{
				foreach (XmlNode node in _x.Document.SelectNodes("/Dati//Dato[@ID='" + ID + "']/Valore"))
				{
					//Calcolo valori attuali

					if (node.Attributes["EA"] != null)
					{
						valoreEA.Add(node.Attributes["ID"].Value, node.Attributes["EA"].Value);
					}
					else
					{
						valoreEA.Add(node.Attributes["ID"].Value, "0");
					}
				}
			}
		}

		private void CalculateValues()
		{
			double TotvalueEA = 0.0;
            double TotvalueAP = 0.0;
            double TotvaluePN = 0.0;
			double TotvalueDIFF = 0.0;
            double TotvalueIMPOSTE = 0.0;
            double TotvalueIMPOSTEPN = 0.0;

            DataRow tmpNode = null;

           

            for (int i = 1; i <= indicerow.Count; i++)
			{
            
                tmpNode = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
                
                    if( rowTOT.ToString()=="")
                      tmpNode["rowTOT"] = 0;
                    else
                      tmpNode["rowTOT"] = rowTOT.ToString();
                TextBlock txtCodice = (TextBlock)this.FindName( "txtCodice" + indicerow[i].ToString() );

                tmpNode["row"] = indicerow[i].ToString();
               tmpNode["txtCodice"] = txtCodice.Text;

                TextBlock txtName = (TextBlock)this.FindName( "txtName" + indicerow[i].ToString() );
                
                tmpNode["txtName"] = ((TextBlock)(txtName)).ToolTip.ToString();
                
                double valueIMPOSTE = 0.0;
                TextBlock txtIMPOSTE = (TextBlock)this.FindName( "txtIMPOSTE" + indicerow[i].ToString() );               

                tmpNode["txtIMPOSTE" ] = txtIMPOSTE.Text;
                double.TryParse( txtIMPOSTE.Text, out valueIMPOSTE );

                double valueIMPOSTEPN = 0.0;
                TextBlock txtIMPOSTEPN = (TextBlock)this.FindName("txtIMPOSTEPN" + indicerow[i].ToString());

                tmpNode["txtIMPOSTEPN"] = txtIMPOSTEPN.Text;

                double.TryParse(txtIMPOSTEPN.Text, out valueIMPOSTEPN);
                
                double valueETSP = 0.0;
                double.TryParse(txtErroreTollerabileSP.Text, out valueETSP);

				double valueETCE = 0.0;
                double.TryParse(txtErroreTollerabileCE.Text, out valueETCE);

				double valueAP = 0.0;
                TextBlock txtAP = (TextBlock)this.FindName( "txtAP" + indicerow[i].ToString() );
				double.TryParse(txtAP.Text, out valueAP);

               
                tmpNode["txtAP"] = txtAP.Text;
                
                TextBox txtAPX = (TextBox)this.FindName( "txtAPX" + indicerow[i].ToString() );
                if ( txtAPX != null )
                {
                    txtAPX.Text = txtAP.Text;
                }

				double valueEA = 0.0;
                TextBlock txtEA = (TextBlock)this.FindName( "txtEA" + indicerow[i].ToString() );
				double.TryParse(txtEA.Text, out valueEA);

                tmpNode["txtEA"] = txtEA.Text;

				double valueDIFF = 0.0;
                TextBlock txtDIFF = (TextBlock)this.FindName( "txtDIFF" + indicerow[i].ToString() );

                CheckBox chkIrrilevante = (CheckBox)this.FindName( "chkIrrilevante" + indicerow[i].ToString() );

             
                tmpNode["chkIrrilevante"] = chkIrrilevante.IsChecked.ToString();

                double valuePN = 0.0;
                TextBlock txtPN = (TextBlock)this.FindName( "txtPN" + indicerow[i].ToString() );
                
                if ( chkIrrilevante.IsChecked == true )
                {
                    txtPN.Text = ConvertNumber( "0" );
                    txtDIFF.Text = ConvertNumber( "0" );
                    txtIMPOSTE.Text = ConvertNumber("0");
                    valueIMPOSTE = 0;
                    txtIMPOSTEPN.Text = ConvertNumber("0");
                    valueIMPOSTEPN = 0;
                }
                else
                {
                    txtPN.Text = txtEA.Text;
                    txtDIFF.Text = ConvertNumber( ( valueEA - valueAP ).ToString() );// ( ( ( valueEA - valueAP ) > 0 ) ? ( valueEA - valueAP ) : 0 ).ToString() );
                    txtIMPOSTE.Foreground = Brushes.Black;
                    txtIMPOSTEPN.Foreground = Brushes.Black;
                }
                
                double.TryParse( txtPN.Text, out valuePN );

                tmpNode["txtPN"] = txtPN.Text;

                tmpNode["txtDIFF"] = txtDIFF.Text;

				double.TryParse(txtDIFF.Text, out valueDIFF);
                

                TextBlock txtNETTOPN = (TextBlock)this.FindName("txtNETTOPN" + indicerow[i].ToString());

                txtNETTOPN.Text = ConvertNumber((valueIMPOSTEPN + valuePN).ToString());

                tmpNode["txtNETTOPN" ] = txtNETTOPN.Text;

                
                TextBlock txtSP = (TextBlock)this.FindName( "txtSP" + indicerow[i].ToString() );

                txtSP.Text = ConvertNumber( /*(((valueIMPOSTEPN + valuePN)) < 0) ? "0" :*/ ((Math.Abs(valueIMPOSTEPN + valuePN) - Math.Abs( valueETSP )) < 0) ? "0" : (Math.Abs(valueIMPOSTEPN + valuePN) - Math.Abs( valueETSP )).ToString() );
                

                tmpNode["txtSP" ] = txtSP.Text;
                
                TextBlock txtNETTOCE = (TextBlock)this.FindName("txtNETTOCE" + indicerow[i].ToString());

                txtNETTOCE.Text = ConvertNumber((valueIMPOSTE + valueDIFF).ToString());
                
                tmpNode["txtNETTOCE" ] = txtNETTOCE.Text;

                
                TextBlock txtCE = (TextBlock)this.FindName( "txtCE" + indicerow[i].ToString() );

                txtCE.Text = ConvertNumber( /*(((valueIMPOSTE + valueDIFF)) < 0 )? "0" :*/ ((Math.Abs(valueIMPOSTE + valueDIFF) - Math.Abs( valueETCE )) < 0) ? "0" : (Math.Abs(valueIMPOSTE + valueDIFF) - Math.Abs( valueETCE )).ToString() );

                tmpNode["txtCE" ] = txtCE.Text;

				TotvalueEA += valueEA;
                TotvalueAP += valueAP;
                TotvaluePN += valuePN;
				TotvalueDIFF += valueDIFF;
                TotvalueIMPOSTE += valueIMPOSTE;
                TotvalueIMPOSTEPN += valueIMPOSTEPN;


			    TextBlock txtTotEA = (TextBlock)this.FindName("txtTotEA");
			    txtTotEA.Text = ConvertNumber(TotvalueEA.ToString());


			    tmpNode["txtTotEA"] = txtTotEA.Text;

                TextBlock txtTotAP = (TextBlock)this.FindName( "txtTotAP" );
                txtTotAP.Text = ConvertNumber( TotvalueAP.ToString() );


                tmpNode["txtTotAP"] = txtTotAP.Text;

                TextBlock txtTotPN = (TextBlock)this.FindName( "txtTotPN" );
                txtTotPN.Text = ConvertNumber( TotvaluePN.ToString() );

                tmpNode["txtTotPN"] = txtTotPN.Text;
            
                TextBlock txtTotTotPN = (TextBlock)this.FindName("txtTotTotPN");
                txtTotTotPN.Text = ConvertNumber((TotvaluePN + TotvalueIMPOSTEPN).ToString());


                tmpNode["txtTotTotPN"] = txtTotTotPN.Text;


                TextBlock txtTotTotCE = (TextBlock)this.FindName("txtTotTotCE");
                txtTotTotCE.Text = ConvertNumber((TotvalueDIFF+ TotvalueIMPOSTE).ToString());
            
                tmpNode["txtTotTotCE"] = txtTotTotCE.Text;


                TextBlock txtTotDIFF = (TextBlock)this.FindName("txtTotDIFF");
			    txtTotDIFF.Text = ConvertNumber(TotvalueDIFF.ToString());
            
			    tmpNode["txtTotDIFF"] = txtTotDIFF.Text;

                TextBlock txtTotIMPOSTE = (TextBlock)this.FindName( "txtTotIMPOSTE" );
                txtTotIMPOSTE.Text = ConvertNumber( TotvalueIMPOSTE.ToString() );


                tmpNode["txtTotIMPOSTE"] = txtTotIMPOSTE.Text;

                TextBlock txtTotIMPOSTEPN = (TextBlock)this.FindName("txtTotIMPOSTEPN");
                txtTotIMPOSTEPN.Text = ConvertNumber(TotvalueIMPOSTEPN.ToString());

                tmpNode["txtTotIMPOSTEPN"] = txtTotIMPOSTEPN.Text;


                double TotMaterialitaSP = 0.0;
			    TextBlock txtTotMaterialitaSP = (TextBlock)this.FindName("txtTotMaterialitaSP");
			    double.TryParse(txtTotMaterialitaSP.Text, out TotMaterialitaSP);


			    tmpNode["txtTotMaterialitaSP"] = txtTotMaterialitaSP.Text;

			    double TotMaterialitaCE = 0.0;
			    TextBlock txtTotMaterialitaCE = (TextBlock)this.FindName("txtTotMaterialitaCE");
			    double.TryParse(txtTotMaterialitaCE.Text, out TotMaterialitaCE);
            

			    tmpNode["txtTotMaterialitaCE"] = txtTotMaterialitaCE.Text;

                TextBlock txtTotEccedenzaSP = (TextBlock)this.FindName( "txtTotEccedenzaSP" );
                txtTotEccedenzaSP.Text = ConvertNumber( ((Math.Abs(TotvaluePN + TotvalueIMPOSTEPN) - Math.Abs( TotMaterialitaSP )) < 0) ? "0" : (Math.Abs(TotvaluePN + TotvalueIMPOSTEPN) - Math.Abs( TotMaterialitaSP )).ToString() );

           
                tmpNode["txtTotEccedenzaSP"] = txtTotEccedenzaSP.Text;

                TextBlock txtTotEccedenzaCE = (TextBlock)this.FindName( "txtTotEccedenzaCE" );
                txtTotEccedenzaCE.Text = ConvertNumber( ((Math.Abs(TotvalueDIFF + TotvalueIMPOSTE) - Math.Abs( TotMaterialitaCE )) < 0) ? "0" : (Math.Abs(TotvalueDIFF + TotvalueIMPOSTE) - Math.Abs( TotMaterialitaCE )).ToString() );


                tmpNode["txtTotEccedenzaCE"] = txtTotEccedenzaCE.Text;

                //TextBlock txtGIUDIZIOSP = (TextBlock)this.FindName("txtGIUDIZIOSP");
                //txtGIUDIZIOSP.FontWeight = FontWeights.Bold;
                //txtGIUDIZIOSP.TextAlignment = TextAlignment.Center;
                //txtGIUDIZIOSP.Foreground = Brushes.Red;

                //if(Math.Abs(TotMaterialitaSP) >= Math.Abs(TotvalueEA))
                //{
                //    txtGIUDIZIOSP.Text = "POSITIVO";
                //}
                //else
                //{
                //    txtGIUDIZIOSP.Text = "NEGATIVO";
                //}

                //if (tmpNode["txtGIUDIZIOSP"] == null)
                //{
                //    XmlAttribute attr = _x.Document.CreateAttribute("txtGIUDIZIOSP");
                //    tmpNode.Append(attr);
                //}

                //tmpNode["txtGIUDIZIOSP"].Value = txtGIUDIZIOSP.Text;

                //TextBlock txtGIUDIZIOCE = (TextBlock)this.FindName("txtGIUDIZIOCE");
                //txtGIUDIZIOCE.FontWeight = FontWeights.Bold;
                //txtGIUDIZIOCE.TextAlignment = TextAlignment.Center;
                //txtGIUDIZIOCE.Foreground = Brushes.Red;

                //if (Math.Abs(TotMaterialitaCE) >= Math.Abs(TotvalueDIFF))
                //{
                //    txtGIUDIZIOCE.Text = "POSITIVO";
                //}
                //else
                //{
                //    txtGIUDIZIOCE.Text = "NEGATIVO";
                //}

                //if (tmpNode["txtGIUDIZIOCE"] == null)
                //{
                //    XmlAttribute attr = _x.Document.CreateAttribute("txtGIUDIZIOCE");
                //    tmpNode.Append(attr);
                //}

                //tmpNode["txtGIUDIZIOCE"].Value = txtGIUDIZIOCE.Text;
            }
        }

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double newsize = e.NewSize.Width - 30.0;

            //for ( int i = 4; i <= rowTOT; i++ )
            //{
            //    TextBlock txtName = (TextBlock)this.FindName( "txtName" + i.ToString() );
            //    txtName.Width = newsize - 890;
            //}
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			CalculateValues();
		}

        void txt_MouseDown( object sender, MouseButtonEventArgs e )
        {
            if ( e.ClickCount == 2 )
            {
                //Se vecchio nodo presente nello stesso bilancio
                if ( IDTree == "4" )
                {

                    WindowWorkArea wa = new WindowWorkArea( ref _x );

                    //Nodi
                    int index = -1;
                    wa.NodeHome = -1;

                    RevisoftApplication.XmlManager xt = new XmlManager();
                    xt.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                    XmlDataProvider TreeXmlProvider = new XmlDataProvider();
                    TreeXmlProvider.Document = xt.LoadEncodedFile( App.AppTemplateTreeBilancio );

                    if ( TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode( "/Tree" ) != null )
                    {
                        foreach ( XmlNode item in TreeXmlProvider.Document.SelectNodes( "/Tree//Node" ) )
                        {
                            if ( item.Attributes["Tipologia"].Value == "Nodo Multiplo" || item.ChildNodes.Count == 1 )
                            {
                                index++;

                                if ( item.Attributes["ID"].Value == ((TextBlock)(sender)).MaxHeight.ToString() )
                                {
                                    wa.NodeHome = index;
                                }

                                wa.Nodes.Add( index, item );
                            }
                        }
                    }

                    if ( wa.NodeHome == -1 )
                    {
                        e.Handled = true;
                        return;
                    }

                    wa.NodeNow = wa.NodeHome;

                    wa.Owner = Window.GetWindow( this );

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

                    foreach ( DictionaryEntry item in Sessioni )
                    {
                        if ( item.Value.ToString() == _x.File )
                        {
                            wa.SessioneHome = Convert.ToInt32( item.Key.ToString() );
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

                    App.MessaggioSolaScrittura = "Carta in sola lettura, premere tasto ESCI";
                    App.MessaggioSolaScritturaStato = "Carta in sola lettura, premere tasto ESCI";

                    wa.ShowDialog();

                    App.MessaggioSolaScrittura = "Occorre selezionare Sblocca Stato per modificare il contenuto.";
                    App.MessaggioSolaScritturaStato = "Sessione in sola lettura, impossibile modificare lo stato.";
                }
                else
                {
                    MasterFile mf = MasterFile.Create();
                    string bilancioAssociato = mf.GetBilancioAssociatoFromConclusioneFile( Sessioni[SessioneNow].ToString() );
                    string bilancioTreeAssociato = mf.GetBilancioTreeAssociatoFromConclusioneFile( Sessioni[SessioneNow].ToString() );
                    string bilancioIDAssociato = mf.GetBilancioIDAssociatoFromConclusioneFile( Sessioni[SessioneNow].ToString() );


                    if ( bilancioAssociato == "" )
                    {
                        e.Handled = true;
                        return;
                    }

                    XmlDataProviderManager _xNew = new XmlDataProviderManager( bilancioAssociato );

                    WindowWorkArea wa = new WindowWorkArea( ref _xNew );

                    //Nodi
                    wa.NodeHome = 0;

                    RevisoftApplication.XmlManager xt = new XmlManager();
                    xt.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                    XmlDataProvider TreeXmlProvider = new XmlDataProvider();
                    TreeXmlProvider.Document = xt.LoadEncodedFile( bilancioTreeAssociato );

                    if ( TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode( "/Tree" ) != null )
                    {
                        foreach ( XmlNode item in TreeXmlProvider.Document.SelectNodes( "/Tree//Node" ) )
                        {
                            if ( item.Attributes["Codice"].Value == ((TextBlock)(sender)).Text )
                            {
                                wa.Nodes.Add( 0, item );
                            }
                        }
                    }

                    if ( wa.Nodes.Count == 0 )
                    {
                        e.Handled = true;
                        return;
                    }

                    wa.NodeNow = wa.NodeHome;

                    wa.Owner = Window.GetWindow( this );

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
                    wa.Sessioni.Add( 0, bilancioAssociato );

                    wa.SessioniTitoli.Clear();
                    wa.SessioniTitoli.Add( 0, SessioniTitoli[SessioneNow].ToString() );

                    wa.SessioniID.Clear();
                    wa.SessioniID.Add( 0, bilancioIDAssociato );

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

		private void txt1_LostFocus(object sender, RoutedEventArgs e)
		{
			foreach (DataRow item in dati.Rows) //_x.Document.SelectNodes("/Dati/Dato[@ID]/Valore[@tipo='ErroriRilevati']"))
			{
				if ( "txtAP" + item["ID"].ToString() == ((TextBox)(sender)).Tag.ToString())
				{
                  
                    item["txtAP"] = ((TextBox)(sender)).Text;
				}
			}
		}

        private void obj_PreviewMouseLeftButtonDown2( object sender, MouseButtonEventArgs e )
        {
            return;
        }
        private void obj_PreviewKeyDown2( object sender, KeyEventArgs e )
        {

            return;
        }

		private void obj_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}
		}

		private void obj_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}
		}

        private void Image_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            Image i = ( (Image)sender );

            TextBlock t = ( (TextBlock)( ( (Grid)( i.Parent ) ).Children[1] ) );

            UIElement u =  ( (Grid)( i.Parent ) ).Children[2];

            if ( u.Visibility == System.Windows.Visibility.Collapsed )
            {
                u.Visibility = System.Windows.Visibility.Visible;
                t.TextAlignment = TextAlignment.Center;
                t.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                var uriSource = new Uri( down, UriKind.Relative );
                i.Source = new BitmapImage( uriSource );
            }
            else
            {
                t.TextAlignment = TextAlignment.Left;
                t.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                u.Visibility = System.Windows.Visibility.Collapsed;
                var uriSource = new Uri( left, UriKind.Relative );
                i.Source = new BitmapImage( uriSource );
            }
        }

        private void UserControl_KeyUp( object sender, KeyEventArgs e )
        {
            if ( e.Key == Key.Enter )
            {
                var element = Keyboard.FocusedElement;
                if ( element.GetType().Name == "TextBox")
                {
                    if ( ((TextBox)element).Name.Contains( "txtAP" ) )
                    {
                        ((TextBox)element).Text = ConvertNumber( ((TextBox)element).Text );
                    }
                }
            }
        }
    }
}
