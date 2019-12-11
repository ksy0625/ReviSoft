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
    public partial class ucErroriRilevatiRiepilogo : UserControl
    {
		private XmlDataProviderManager _x;
		private XmlDataProviderManager _lm;
        private XmlDataProviderManager _lb;
        private string _ID = "-1";

		private string IDB_Padre = "227";
		private string IDBA_Padre = "229";


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

		public ucErroriRilevatiRiepilogo()
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


        public bool Load( ref XmlDataProviderManager x, string ID, string FileConclusione, Hashtable _Sessioni, Hashtable _SessioniTitoli, Hashtable _SessioniID, int _SessioneNow, string _IDTree, string _IDCliente, string _IDSessione )
        {
            Sessioni = _Sessioni;
            SessioniTitoli = _SessioniTitoli;
            SessioniID = _SessioniID;
            SessioneNow = _SessioneNow;
            IDTree = _IDTree;
            IDCliente = _IDCliente;
            IDSessione = _IDSessione;

			_x = x.Clone();
            _ID = ID;
			
			#region Dati da bilancio

			RetrieveData(_x, IDB_Padre);
			if (valoreEA.Count == 0)
			{
				RetrieveData(_x, IDBA_Padre);
			}
			#endregion

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

            if ( IDTree == "4" )
            {
                string FileDataRevisione = mf.GetRevisioneAssociataFromBilancioFile( FileConclusione );

                if ( FileDataRevisione != "" )
                {
                    _lm = new XmlDataProviderManager( FileDataRevisione );
                }
                else
                {
                    _lm = null;
                }

                _lb = x;
            }
            else
            {
                string FileDataRevisione = mf.GetRevisioneAssociataFromConclusioneFile( FileConclusione );
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

			XmlNode tmpNode = null;
			XmlNode tmpNode_true = null;

			if (_lm != null)
			{
				tmpNode = _lm.Document.SelectSingleNode("/Dati//Dato[@ID='" + ID_Materialità_1 + "']");

				if (tmpNode != null && tmpNode.Attributes["Stato"] != null && ((App.TipoTreeNodeStato)(Convert.ToInt32(tmpNode.Attributes["Stato"].Value))) == App.TipoTreeNodeStato.Completato)
				{
					Materialità_1 = true;
					tmpNode_true = tmpNode;
				}

				//if (tmpNode_true == null)
				{
					tmpNode = _lm.Document.SelectSingleNode("/Dati//Dato[@ID='" + ID_Materialità_2 + "']");

					if (tmpNode != null && tmpNode.Attributes["Stato"] != null && ((App.TipoTreeNodeStato)(Convert.ToInt32(tmpNode.Attributes["Stato"].Value))) == App.TipoTreeNodeStato.Completato)
					{
						Materialità_2 = true;
						tmpNode_true = tmpNode;
					}

					//if (tmpNode_true == null)
					{
						tmpNode = _lm.Document.SelectSingleNode("/Dati//Dato[@ID='" + ID_Materialità_3 + "']");

						if (tmpNode != null && tmpNode.Attributes["Stato"] != null && ((App.TipoTreeNodeStato)(Convert.ToInt32(tmpNode.Attributes["Stato"].Value))) == App.TipoTreeNodeStato.Completato)
						{
							Materialità_3 = true;
							tmpNode_true = tmpNode;
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
				tmpNode = tmpNode_true;

                if ( tmpNode.Attributes["txt7BILANCIO"] != null )
                {
                    txt7 = tmpNode.Attributes["txt7BILANCIO"].Value;
                }
                else
                {
                    if ( tmpNode.Attributes["txt7"] != null )
                        txt7 = tmpNode.Attributes["txt7"].Value;
                }

                if ( tmpNode.Attributes["txt7_2spBILANCIO"] != null )
                {
                    txt7_2sp = tmpNode.Attributes["txt7_2spBILANCIO"].Value;
                }
                else
                {
                    if ( tmpNode.Attributes["txt7_2sp"] != null )
                        txt7_2sp = tmpNode.Attributes["txt7_2sp"].Value;
                }

                if ( tmpNode.Attributes["txt7_2ceBILANCIO"] != null )
                {
                    txt7_2ce = tmpNode.Attributes["txt7_2ceBILANCIO"].Value;
                }
                else
                {
                    if ( tmpNode.Attributes["txt7_2ce"] != null )
                        txt7_2ce = tmpNode.Attributes["txt7_2ce"].Value;
                }

                if ( tmpNode.Attributes["txt7_3spBILANCIO"] != null )
                {
                    txt7_3sp = tmpNode.Attributes["txt7_3spBILANCIO"].Value;
                }
                else
                {
                    if ( tmpNode.Attributes["txt7_3sp"] != null )
                        txt7_3sp = tmpNode.Attributes["txt7_3sp"].Value;
                }

                if ( tmpNode.Attributes["txt7_3ecBILANCIO"] != null )
                {
                    txt7_3ec = tmpNode.Attributes["txt7_3ecBILANCIO"].Value;
                }
                else
                {
                    if ( tmpNode.Attributes["txt7_3ec"] != null )
                        txt7_3ec = tmpNode.Attributes["txt7_3ec"].Value;
                }

                if ( tmpNode.Attributes["txt9BILANCIO"] != null )
				{
                    txt9 = tmpNode.Attributes["txt9BILANCIO"].Value;
				}
                else
                {
                    if ( tmpNode.Attributes["txt9"] != null )
                        txt9 = tmpNode.Attributes["txt9"].Value;
                }

                if ( tmpNode.Attributes["txt9_2spBILANCIO"] != null )
                {
                    txt9_2sp = tmpNode.Attributes["txt9_2spBILANCIO"].Value;
                }
                else
                {
                    if ( tmpNode.Attributes["txt9_2sp"] != null )
                        txt9_2sp = tmpNode.Attributes["txt9_2sp"].Value;
                }

                if ( tmpNode.Attributes["txt9_2ceBILANCIO"] != null )
                {
                    txt9_2ec = tmpNode.Attributes["txt9_2ceBILANCIO"].Value;
                }
                else
                {
                    if ( tmpNode.Attributes["txt9_2ce"] != null )
                        txt9_2ec = tmpNode.Attributes["txt9_2ce"].Value;
                }

                if ( tmpNode.Attributes["txt9_3spBILANCIO"] != null )
                {
                    txt9_3sp = tmpNode.Attributes["txt9_3spBILANCIO"].Value;
                }
                else
                {
                    if ( tmpNode.Attributes["txt9_3sp"] != null )
                        txt9_3sp = tmpNode.Attributes["txt9_3sp"].Value;
                }

                if ( tmpNode.Attributes["txt9_3ecBILANCIO"] != null )
                {
                    txt9_3ec = tmpNode.Attributes["txt9_3ecBILANCIO"].Value;
                }
                else
                {
                    if ( tmpNode.Attributes["txt9_3ec"] != null )
                        txt9_3ec = tmpNode.Attributes["txt9_3ec"].Value;
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

            if ( _lb != null )
            {   
                foreach ( XmlNode item in _lb.Document.SelectNodes( "/Dati/Dato[@ID]/Valore[@tipo='ErroriRilevati']" ) )
                {
                    int indiceinterno = 0;

                    if ( item.Attributes["ID"] != null )
                    {
                        int.TryParse( item.Attributes["ID"].Value, out indiceinterno );
                    }

                    if ( indiceinterno > indice )
                    {
                        indice = indiceinterno;
                    }
                }

                Dictionary<int, XmlNode> lista = new Dictionary<int, XmlNode>();

                foreach ( XmlNode item in _lb.Document.SelectNodes( "/Dati/Dato[@ID]/Valore[@tipo='ErroriRilevati']" ) )
                {
                    if ( item.Attributes["name"].Value == "Totale" )
                    {
                        continue;
                    }

                    XmlNode tnode = TreeXmlProvider.Document.SelectSingleNode( "/Tree//Node[@ID=" + item.ParentNode.Attributes["ID"].Value + "]" );
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

                foreach (KeyValuePair<int, XmlNode> itemD in lista.OrderBy(key => key.Key))
                {
                    XmlNode item = itemD.Value;

                    XmlNode tnode = TreeXmlProvider.Document.SelectSingleNode( "/Tree//Node[@ID=" + item.ParentNode.Attributes["ID"].Value + "]" );

                    if ( item.Attributes["ID"] == null )
                    {
                        XmlAttribute attr = _lb.Document.CreateAttribute( "ID" );
                        attr.Value = (++indice).ToString();
                        item.Attributes.Append( attr );
                    }

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
                    txt.Text = (item.Attributes["name"].Value.Length > 30) ? item.Attributes["name"].Value.Substring( 0, 30 ) + "[...]" : item.Attributes["name"].Value;
                    txt.ToolTip = item.Attributes["name"].Value;
                    txt.Name = "txtName" + row.ToString();
                    txt.Margin = new Thickness( 0, 0, 0, 0 );
                    txt.Padding = new Thickness( 0, 0, 0, 0 );
                    txt.TextAlignment = TextAlignment.Left;
                    txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    this.RegisterName( txt.Name, txt );

                    brd.Child = txt;

                    grdMain.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 1 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.Background = Brushes.LightYellow;
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Text = ConvertNumber( item.Attributes["importo"].Value );
                    txt.Name = "txtEA" + row.ToString();
                    this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;

                    brd.Child = txt;

                    grdMain.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 2 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    TextBox txta = new TextBox();
                    txta.Name = "txtAP" + row.ToString();
                    txta.Tag = "txtAP" + item.Attributes["ID"].Value;
                    txta.BorderThickness = new Thickness( 0 );

                    XmlNode tmpNodeAP = _x.Document.SelectSingleNode( "/Dati//Dato[@ID='" + _ID + "']" );

                    if ( tmpNodeAP != null && tmpNodeAP.Attributes["txtAP" + row] != null )
                    {
                        txta.Text = ConvertNumber( tmpNodeAP.Attributes["txtAP" + row].Value );
                    }

                    txta.PreviewMouseLeftButtonDown += new MouseButtonEventHandler( obj_PreviewMouseLeftButtonDown );
                    txta.PreviewKeyDown += new KeyEventHandler( obj_PreviewKeyDown );
                    txta.TextChanged += new TextChangedEventHandler( TextBox_TextChanged );
                    txta.LostFocus += new RoutedEventHandler( txt1_LostFocus );
                    this.RegisterName( txta.Name, txta );
                    txta.TextAlignment = TextAlignment.Right;
                    //if (item.Attributes["txtAP" + item.Attributes["name"].Value] == null)
                    //{
                    //    XmlAttribute attr = item.OwnerDocument.CreateAttribute("txtAP" + item.Attributes["name"].Value);
                    //    item.Attributes.Append(attr);
                    //}

                    //string valore = item.Attributes[("txtAP" + item.Attributes["name"].Value.ToString()).ToString()].Value;
                    //txt1.Text = valore;

                    brd.Child = txta;

                    grdMain.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 3 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.LightYellow;

                    txt = new TextBlock();
                    txt.Text = ConvertNumber( item.Attributes["importo"].Value );
                    txt.Name = "txtPN" + row.ToString();
                    this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;

                    brd.Child = txt;

                    grdMain.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 4 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.LightYellow;

                    txt = new TextBlock();
                    txt.Name = "txtDIFF" + row.ToString();
                    this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;

                    brd.Child = txt;

                    grdMain.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 5 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.White;

                    txt = new TextBlock();
                    txt.Text = ConvertNumber( ((item.Attributes["impattofiscale"] != null) ? item.Attributes["impattofiscale"].Value : "0") );
                    txt.Name = "txtIMPOSTE" + row.ToString();
                    this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;

                    brd.Child = txt;

                    grdMain.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 6 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 1.0, 1.0 );
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.White;

                    CheckBox chk = new CheckBox();
                    chk.Name = "chkIrrilevante" + row.ToString();
                    this.RegisterName( chk.Name, chk );
                    if ( tmpNodeAP != null && tmpNodeAP.Attributes["chkIrrilevante" + row] != null )
                    {
                        chk.IsChecked = Convert.ToBoolean( tmpNodeAP.Attributes["chkIrrilevante" + row].Value );
                    }

                    chk.Checked += new RoutedEventHandler( chk_Checked );
                    chk.Unchecked += new RoutedEventHandler( chk_Checked );
                    chk.PreviewMouseLeftButtonDown += new MouseButtonEventHandler( obj_PreviewMouseLeftButtonDown );
                    chk.PreviewKeyDown += new KeyEventHandler( obj_PreviewKeyDown );
                    chk.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    chk.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    chk.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;

                    brd.Child = chk;

                    grdMain.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 7 );
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
			brd.Background = Brushes.LightYellow;

			txt = new TextBlock();
			txt.Name = "txtTotEA";
			this.RegisterName(txt.Name, txt);
			txt.TextAlignment = TextAlignment.Right;
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
            brd.BorderThickness = new Thickness( 1.0, 0, 0.0, 1.0 );
			brd.BorderBrush = Brushes.Black;
			brd.Background = Brushes.LightYellow;

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
			Grid.SetColumn(brd, 5);

            brd = new Border();
            brd.BorderThickness = new Thickness( 1.0, 0.0, 1.0, 1.0 );
            brd.BorderBrush = Brushes.Black;
            brd.Background = Brushes.White;

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
            Grid.SetColumn( brd, 6 );

            rd = new RowDefinition();
            rd.Height = new GridLength( 30.0 );
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
            brd.Background = Brushes.PaleGreen;

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
			Grid.SetColumn(brd, 4);

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
            brd.BorderBrush = Brushes.Black;
            brd.Background = Brushes.PaleGreen;

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
			Grid.SetColumn(brd, 5);

           
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
            Grid.SetColumn( brd, 5 );


















            row = 2;
            indiceinternoRow = 0;

            foreach (KeyValuePair<int, XmlNode> itemD in lista.OrderBy(key => key.Key))
                {
                    XmlNode item = itemD.Value;

                    XmlNode tnode = TreeXmlProvider.Document.SelectSingleNode( "/Tree//Node[@ID=" + item.ParentNode.Attributes["ID"].Value + "]" );

                    if ( item.Attributes["ID"] == null )
                    {
                        XmlAttribute attr = _lb.Document.CreateAttribute( "ID" );
                        attr.Value = (++indice).ToString();
                        item.Attributes.Append( attr );
                    }

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
                    txt.Text = (item.Attributes["name"].Value.Length > 30) ? item.Attributes["name"].Value.Substring( 0, 30 ) + "[...]" : item.Attributes["name"].Value;
                    txt.ToolTip = item.Attributes["name"].Value;
                    txt.Name = "txtNameX" + indicerow[indiceinternoRow].ToString();
                    txt.Margin = new Thickness( 0, 0, 0, 0 );
                    txt.Padding = new Thickness( 0, 0, 0, 0 );
                    txt.TextAlignment = TextAlignment.Left;
                    txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    this.RegisterName( txt.Name, txt );

                    brd.Child = txt;

                    grdMainET.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 1 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.Background = Brushes.LightYellow;
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Text = ConvertNumber( item.Attributes["importo"].Value );
                    txt.TextAlignment = TextAlignment.Right;

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
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.LightGoldenrodYellow;

                    txt = new TextBlock();
                    txt.Name = "txtSP" + indicerow[indiceinternoRow].ToString();
                    this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;

                    brd.Child = txt;

                    grdMainET.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 4 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 1.0, 1.0 );
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.LightGoldenrodYellow;

                    txt = new TextBlock();
                    txt.Name = "txtCE" + indicerow[indiceinternoRow].ToString();
                    this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;

                    brd.Child = txt;

                    grdMainET.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 5 );
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
            Grid.SetColumn( brd, 5 );

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

			CalculateValues(null);

            return true;
        }

        private void chk_Checked( object sender, RoutedEventArgs e )
        {
            CalculateValues( null );
        }

		public XmlDataProviderManager Save()
		{		
			XmlNode tmpNode = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + _ID + "']");

			if (tmpNode != null)
			{
#region datiprostampa
				if (tmpNode.Attributes["txtErroreTollerabileSP"] == null)
				{
					XmlAttribute attr = _x.Document.CreateAttribute("txtErroreTollerabileSP");
					tmpNode.Attributes.Append(attr);
				}

                tmpNode.Attributes["txtErroreTollerabileSP"].Value = txtErroreTollerabileSP.Text;

				if (tmpNode.Attributes["txtErroreTollerabileCE"] == null)
				{
					XmlAttribute attr = _x.Document.CreateAttribute("txtErroreTollerabileCE");
					tmpNode.Attributes.Append(attr);
				}

                tmpNode.Attributes["txtErroreTollerabileCE"].Value = txtErroreTollerabileCE.Text;
#endregion
			}

			CalculateValues(tmpNode);

			_x.Save();

			return _x;
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

		private void RetrieveData(XmlDataProviderManager _x, string ID)
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

		private void CalculateValues(XmlNode tmpNode)
		{
			double TotvalueEA = 0.0;
            double TotvalueAP = 0.0;
            double TotvaluePN = 0.0;
			double TotvalueDIFF = 0.0;
            double TotvalueIMPOSTE = 0.0;            

			if (tmpNode == null)
			{
				tmpNode = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + _ID + "']");
			}

			if (tmpNode.Attributes["rowTOT"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("rowTOT");
				tmpNode.Attributes.Append(attr);
			}

			tmpNode.Attributes["rowTOT"].Value = rowTOT.ToString();
                        
            for (int i = 1; i <= indicerow.Count; i++)
			//for (int i = 4; i <= rowTOT; i++)
			{
                TextBlock txtCodice = (TextBlock)this.FindName( "txtCodice" + indicerow[i].ToString() );

                if ( tmpNode.Attributes["txtCodice" + indicerow[i].ToString()] == null )
				{
                    XmlAttribute attr = _x.Document.CreateAttribute( "txtCodice" + indicerow[i].ToString() );
					tmpNode.Attributes.Append(attr);
				}

                tmpNode.Attributes["txtCodice" + indicerow[i].ToString()].Value = txtCodice.Text;

                TextBlock txtName = (TextBlock)this.FindName( "txtName" + indicerow[i].ToString() );

                if ( tmpNode.Attributes["txtName" + indicerow[i].ToString()] == null )
				{
                    XmlAttribute attr = _x.Document.CreateAttribute( "txtName" + indicerow[i].ToString() );
					tmpNode.Attributes.Append(attr);
				}

                tmpNode.Attributes["txtName" + indicerow[i].ToString()].Value = txtName.Text;



                double valueIMPOSTE = 0.0;
                TextBlock txtIMPOSTE = (TextBlock)this.FindName( "txtIMPOSTE" + indicerow[i].ToString() );               

                if ( tmpNode.Attributes["txtIMPOSTE" + indicerow[i].ToString()] == null )
                {
                    XmlAttribute attr = _x.Document.CreateAttribute( "txtIMPOSTE" + indicerow[i].ToString() );
                    tmpNode.Attributes.Append( attr );
                }

                tmpNode.Attributes["txtIMPOSTE" + indicerow[i].ToString()].Value = txtIMPOSTE.Text;

                double.TryParse( txtIMPOSTE.Text, out valueIMPOSTE );




				double valueETSP = 0.0;
                double.TryParse(txtErroreTollerabileSP.Text, out valueETSP);

				double valueETCE = 0.0;
                double.TryParse(txtErroreTollerabileCE.Text, out valueETCE);

				double valueAP = 0.0;
                TextBox txtAP = (TextBox)this.FindName( "txtAP" + indicerow[i].ToString() );
				double.TryParse(txtAP.Text, out valueAP);

                if ( tmpNode.Attributes["txtAP" + indicerow[i].ToString()] == null )
				{
                    XmlAttribute attr = _x.Document.CreateAttribute( "txtAP" + indicerow[i].ToString() );
					tmpNode.Attributes.Append(attr);
				}

                tmpNode.Attributes["txtAP" + indicerow[i].ToString()].Value = txtAP.Text;

                TextBox txtAPX = (TextBox)this.FindName( "txtAPX" + indicerow[i].ToString() );
                if ( txtAPX != null )
                {
                    txtAPX.Text = txtAP.Text;
                }

				double valueEA = 0.0;
                TextBlock txtEA = (TextBlock)this.FindName( "txtEA" + indicerow[i].ToString() );
				double.TryParse(txtEA.Text, out valueEA);

                if ( tmpNode.Attributes["txtEA" + indicerow[i].ToString()] == null )
				{
                    XmlAttribute attr = _x.Document.CreateAttribute( "txtEA" + indicerow[i].ToString() );
					tmpNode.Attributes.Append(attr);
				}

                tmpNode.Attributes["txtEA" + indicerow[i].ToString()].Value = txtEA.Text;

				double valueDIFF = 0.0;
                TextBlock txtDIFF = (TextBlock)this.FindName( "txtDIFF" + indicerow[i].ToString() );

                CheckBox chkIrrilevante = (CheckBox)this.FindName( "chkIrrilevante" + indicerow[i].ToString() );

                if ( tmpNode.Attributes["chkIrrilevante" + indicerow[i].ToString()] == null )
                {
                    XmlAttribute attr = _x.Document.CreateAttribute( "chkIrrilevante" + indicerow[i].ToString() );
                    tmpNode.Attributes.Append( attr );
                }

                tmpNode.Attributes["chkIrrilevante" + indicerow[i].ToString()].Value = chkIrrilevante.IsChecked.ToString();

                double valuePN = 0.0;
                TextBlock txtPN = (TextBlock)this.FindName( "txtPN" + indicerow[i].ToString() );
                
                if ( chkIrrilevante.IsChecked == true )
                {
                    txtPN.Text = ConvertNumber( "0" );
                    txtDIFF.Text = ConvertNumber( "0" );
                    txtIMPOSTE.Foreground = Brushes.White;
                    valueIMPOSTE = 0;
                }
                else
                {
                    txtPN.Text = txtEA.Text;
                    txtDIFF.Text = ConvertNumber( (((valueEA - valueAP) > 0)? (valueEA - valueAP) : 0).ToString() );
                    txtIMPOSTE.Foreground = Brushes.Black;
                }
                
                double.TryParse( txtPN.Text, out valuePN );

                if ( tmpNode.Attributes["txtPN" + indicerow[i].ToString()] == null )
                {
                    XmlAttribute attr = _x.Document.CreateAttribute( "txtPN" + indicerow[i].ToString() );
                    tmpNode.Attributes.Append( attr );
                }

                tmpNode.Attributes["txtPN" + indicerow[i].ToString()].Value = txtPN.Text;

                if ( tmpNode.Attributes["txtDIFF" + indicerow[i].ToString()] == null )
				{
                    XmlAttribute attr = _x.Document.CreateAttribute( "txtDIFF" + indicerow[i].ToString() );
					tmpNode.Attributes.Append(attr);
				}

                tmpNode.Attributes["txtDIFF" + indicerow[i].ToString()].Value = txtDIFF.Text;

				double.TryParse(txtDIFF.Text, out valueDIFF);

                TextBlock txtSP = (TextBlock)this.FindName( "txtSP" + indicerow[i].ToString() );

                txtSP.Text = ConvertNumber( (valuePN < 0) ? "0" : ((Math.Abs( valuePN ) - Math.Abs( valueETSP )) < 0) ? "0" : (Math.Abs( valuePN ) - Math.Abs( valueETSP )).ToString() );


                if ( tmpNode.Attributes["txtSP" + indicerow[i].ToString()] == null )
				{
                    XmlAttribute attr = _x.Document.CreateAttribute( "txtSP" + indicerow[i].ToString() );
					tmpNode.Attributes.Append(attr);
				}

                tmpNode.Attributes["txtSP" + indicerow[i].ToString()].Value = txtSP.Text;

                TextBlock txtCE = (TextBlock)this.FindName( "txtCE" + indicerow[i].ToString() );

                txtCE.Text = ConvertNumber( (valueDIFF < 0 )? "0" : ((Math.Abs( valueDIFF ) - Math.Abs( valueETCE )) < 0) ? "0" : (Math.Abs( valueDIFF ) - Math.Abs( valueETCE )).ToString() );

                if ( tmpNode.Attributes["txtCE" + indicerow[i].ToString()] == null )
				{
                    XmlAttribute attr = _x.Document.CreateAttribute( "txtCE" + indicerow[i].ToString() );
					tmpNode.Attributes.Append(attr);
				}

                tmpNode.Attributes["txtCE" + indicerow[i].ToString()].Value = txtCE.Text;

				TotvalueEA += valueEA;
                TotvalueAP += valueAP;
                TotvaluePN += valuePN;
				TotvalueDIFF += valueDIFF;
                TotvalueIMPOSTE += valueIMPOSTE;
			}

			TextBlock txtTotEA = (TextBlock)this.FindName("txtTotEA");
			txtTotEA.Text = ConvertNumber(TotvalueEA.ToString());

			if (tmpNode.Attributes["txtTotEA"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtTotEA");
				tmpNode.Attributes.Append(attr);
			}

			tmpNode.Attributes["txtTotEA"].Value = txtTotEA.Text;

            TextBlock txtTotAP = (TextBlock)this.FindName( "txtTotAP" );
            txtTotAP.Text = ConvertNumber( TotvalueAP.ToString() );

            if ( tmpNode.Attributes["txtTotAP"] == null )
            {
                XmlAttribute attr = _x.Document.CreateAttribute( "txtTotAP" );
                tmpNode.Attributes.Append( attr );
            }

            tmpNode.Attributes["txtTotAP"].Value = txtTotAP.Text;

            TextBlock txtTotPN = (TextBlock)this.FindName( "txtTotPN" );
            txtTotPN.Text = ConvertNumber( TotvaluePN.ToString() );

            if ( tmpNode.Attributes["txtTotPN"] == null )
            {
                XmlAttribute attr = _x.Document.CreateAttribute( "txtTotPN" );
                tmpNode.Attributes.Append( attr );
            }

            tmpNode.Attributes["txtTotPN"].Value = txtTotPN.Text;

			TextBlock txtTotDIFF = (TextBlock)this.FindName("txtTotDIFF");
			txtTotDIFF.Text = ConvertNumber(TotvalueDIFF.ToString());

			if (tmpNode.Attributes["txtTotDIFF"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtTotDIFF");
				tmpNode.Attributes.Append(attr);
			}

			tmpNode.Attributes["txtTotDIFF"].Value = txtTotDIFF.Text;

            TextBlock txtTotIMPOSTE = (TextBlock)this.FindName( "txtTotIMPOSTE" );
            txtTotIMPOSTE.Text = ConvertNumber( TotvalueIMPOSTE.ToString() );

            if ( tmpNode.Attributes["txtTotIMPOSTE"] == null )
			{
                XmlAttribute attr = _x.Document.CreateAttribute( "txtTotIMPOSTE" );
				tmpNode.Attributes.Append(attr);
			}

            tmpNode.Attributes["txtTotIMPOSTE"].Value = txtTotIMPOSTE.Text;
            

			double TotMaterialitaSP = 0.0;
			TextBlock txtTotMaterialitaSP = (TextBlock)this.FindName("txtTotMaterialitaSP");
			double.TryParse(txtTotMaterialitaSP.Text, out TotMaterialitaSP);

			if (tmpNode.Attributes["txtTotMaterialitaSP"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtTotMaterialitaSP");
				tmpNode.Attributes.Append(attr);
			}

			tmpNode.Attributes["txtTotMaterialitaSP"].Value = txtTotMaterialitaSP.Text;

			double TotMaterialitaCE = 0.0;
			TextBlock txtTotMaterialitaCE = (TextBlock)this.FindName("txtTotMaterialitaCE");
			double.TryParse(txtTotMaterialitaCE.Text, out TotMaterialitaCE);

			if (tmpNode.Attributes["txtTotMaterialitaCE"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtTotMaterialitaCE");
				tmpNode.Attributes.Append(attr);
			}

			tmpNode.Attributes["txtTotMaterialitaCE"].Value = txtTotMaterialitaCE.Text;

            TextBlock txtTotEccedenzaSP = (TextBlock)this.FindName( "txtTotEccedenzaSP" );
            txtTotEccedenzaSP.Text = ConvertNumber( ((Math.Abs( TotvaluePN ) - Math.Abs( TotMaterialitaSP )) < 0) ? "0" : (Math.Abs( TotvaluePN ) - Math.Abs( TotMaterialitaSP )).ToString() );

            if ( tmpNode.Attributes["txtTotEccedenzaSP"] == null )
			{
                XmlAttribute attr = _x.Document.CreateAttribute( "txtTotEccedenzaSP" );
				tmpNode.Attributes.Append(attr);
			}

            tmpNode.Attributes["txtTotEccedenzaSP"].Value = txtTotEccedenzaSP.Text;

            TextBlock txtTotEccedenzaCE = (TextBlock)this.FindName( "txtTotEccedenzaCE" );
            txtTotEccedenzaCE.Text = ConvertNumber( ((Math.Abs( TotvalueDIFF ) - Math.Abs( TotMaterialitaCE )) < 0) ? "0" : (Math.Abs( TotvalueDIFF ) - Math.Abs( TotMaterialitaCE )).ToString() );

            if ( tmpNode.Attributes["txtTotEccedenzaCE"] == null )
            {
                XmlAttribute attr = _x.Document.CreateAttribute( "txtTotEccedenzaCE" );
                tmpNode.Attributes.Append( attr );
            }

            tmpNode.Attributes["txtTotEccedenzaCE"].Value = txtTotEccedenzaCE.Text;

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

            //if (tmpNode.Attributes["txtGIUDIZIOSP"] == null)
            //{
            //    XmlAttribute attr = _x.Document.CreateAttribute("txtGIUDIZIOSP");
            //    tmpNode.Attributes.Append(attr);
            //}

            //tmpNode.Attributes["txtGIUDIZIOSP"].Value = txtGIUDIZIOSP.Text;

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

            //if (tmpNode.Attributes["txtGIUDIZIOCE"] == null)
            //{
            //    XmlAttribute attr = _x.Document.CreateAttribute("txtGIUDIZIOCE");
            //    tmpNode.Attributes.Append(attr);
            //}

            //tmpNode.Attributes["txtGIUDIZIOCE"].Value = txtGIUDIZIOCE.Text;
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
			CalculateValues(null);
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
			foreach (XmlNode item in _x.Document.SelectNodes("/Dati/Dato[@ID]/Valore[@tipo='ErroriRilevati']"))
			{
				if (item.Attributes["ID"] != null && "txtAP" + item.Attributes["ID"].Value == ((TextBox)(sender)).Tag.ToString())
				{
					if (item.Attributes["txtAP"] == null)
					{
						XmlAttribute attr = _x.Document.CreateAttribute("txtAP");
						item.Attributes.Append(attr);
					}

					item.Attributes["txtAP"].Value = ((TextBox)(sender)).Text;
				}
			}
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
