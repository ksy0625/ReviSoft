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

namespace UserControls
{ 
    public partial class ucErroriRilevatiRiepilogoNN : UserControl
    {
		private XmlDataProviderManager _x;
		private XmlDataProviderManager _lm;
        private XmlDataProviderManager _lb;
        private string _ID = "-1";
        
		Hashtable valoreEA = new Hashtable();

		Hashtable SommeDaExcel = new Hashtable();
		Hashtable ValoriDaExcelEA = new Hashtable();

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

        public ucErroriRilevatiRiepilogoNN()
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

            double totaleimporto = 0.0;

            if ( _lb != null )
            {
                foreach ( XmlNode item in _lb.Document.SelectNodes( "/Dati/Dato[@ID]/Valore[@tipo='ErroriRilevatiMR']" ) )
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

                foreach ( XmlNode item in _lb.Document.SelectNodes( "/Dati/Dato[@ID]/Valore[@tipo='ErroriRilevatiMR']" ) )
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

                foreach ( KeyValuePair<int, XmlNode> itemD in lista.OrderBy( key => key.Key ) )
                {
                    XmlNode item = itemD.Value;

                    XmlNode tnode = TreeXmlProvider.Document.SelectSingleNode( "/Tree//Node[@ID=" + item.ParentNode.Attributes["ID"].Value + "]" );

                    if ( item.Attributes["ID"] == null )
                    {
                        XmlAttribute attr = _lb.Document.CreateAttribute( "ID" );
                        attr.Value = ( ++indice ).ToString();
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
                    grdMainMR.RowDefinitions.Add( rd );
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

                    grdMainMR.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 0 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Text = ( item.Attributes["name"].Value.Length > 30 ) ? item.Attributes["name"].Value.Substring( 0, 30 ) + "[...]" : item.Attributes["name"].Value;
                    txt.ToolTip = item.Attributes["name"].Value;
                    txt.Name = "txtName" + row.ToString();
                    txt.Margin = new Thickness( 0, 0, 0, 0 );
                    txt.Padding = new Thickness( 0, 0, 0, 0 );
                    txt.TextAlignment = TextAlignment.Left;
                    txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    this.RegisterName( txt.Name, txt );

                    brd.Child = txt;

                    grdMainMR.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 1 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.Background = Brushes.LightYellow;
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Text = item.Attributes["contoimputato"].Value;
                    txt.Name = "txtcontoimputato" + row.ToString();
                    this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;

                    brd.Child = txt;

                    grdMainMR.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 2 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Text = item.Attributes["contoproposto"].Value;
                    txt.Name = "txtcontoproposto" + row.ToString();
                    this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;

                    brd.Child = txt;

                    grdMainMR.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 3 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Text = ConvertNumber( item.Attributes["importo"].Value );

                    double valueimporto = 0;
                    double.TryParse( txt.Text, out valueimporto );
                    totaleimporto += valueimporto;
                    
                    txt.Name = "txtEA" + row.ToString();
                    this.RegisterName( txt.Name, txt );
                    txt.TextAlignment = TextAlignment.Right;

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
                    this.RegisterName( chk.Name, chk );
                    chk.IsChecked = Convert.ToBoolean( item.Attributes["corretto"].Value );
                    chk.IsHitTestVisible = false;

                    chk.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    chk.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    chk.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;

                    brd.Child = chk;

                    grdMainMR.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 5 );
                }


                rowTOT = row;

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
                brd.Background = Brushes.LightYellow;

                txt = new TextBlock();
                txt.Name = "txtTotEA";
                this.RegisterName( txt.Name, txt );
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

			CalculateValues(null);

            indice = 0;
            row = 3;

            if ( _lb != null )
            {
                foreach ( XmlNode item in _lb.Document.SelectNodes( "/Dati/Dato[@ID]/Valore[@tipo='ErroriRilevatiNN']" ) )
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

                foreach ( XmlNode item in _lb.Document.SelectNodes( "/Dati/Dato[@ID]/Valore[@tipo='ErroriRilevatiNN']" ) )
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
                
                foreach ( KeyValuePair<int, XmlNode> itemD in lista.OrderBy( key => key.Key ) )
                {
                    XmlNode item = itemD.Value;

                    XmlNode tnode = TreeXmlProvider.Document.SelectSingleNode( "/Tree//Node[@ID=" + item.ParentNode.Attributes["ID"].Value + "]" );

                    if ( item.Attributes["ID"] == null )
                    {
                        XmlAttribute attr = _lb.Document.CreateAttribute( "ID" );
                        attr.Value = ( ++indice ).ToString();
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
                    grdMainNN.RowDefinitions.Add( rd );
                    row++;

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.BorderBrush = Brushes.Black;
                    
                    txt = new TextBlock();
                    txt.Text = tnode.ParentNode.Attributes["Codice"].Value;
                    txt.ToolTip = tnode.ParentNode.Attributes["Titolo"].Value;
                    txt.TextAlignment = TextAlignment.Left;
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
                    txt.Text = ( item.Attributes["name"].Value.Length > 30 ) ? item.Attributes["name"].Value.Substring( 0, 30 ) + "[...]" : item.Attributes["name"].Value;
                    txt.ToolTip = item.Attributes["name"].Value;
                    txt.Margin = new Thickness( 0, 0, 0, 0 );
                    txt.Padding = new Thickness( 0, 0, 0, 0 );
                    txt.TextAlignment = TextAlignment.Left;
                    txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                    brd.Child = txt;

                    grdMainNN.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 1 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 0, 1.0 );
                    brd.Background = Brushes.LightYellow;
                    brd.BorderBrush = Brushes.Black;

                    txt = new TextBlock();
                    txt.Text = item.Attributes["numero"].Value;
                    txt.TextAlignment = TextAlignment.Right;

                    brd.Child = txt;

                    grdMainNN.Children.Add( brd );
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 2 );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0, 0, 1.0, 1.0 );
                    brd.BorderBrush = Brushes.Black;
                    brd.Background = Brushes.White;

                    CheckBox chk = new CheckBox();
                    chk.IsChecked = Convert.ToBoolean( item.Attributes["corretto"].Value );
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

            return true;
        }

        private void chk_Checked( object sender, RoutedEventArgs e )
        {
            CalculateValues( null );
        }

		public XmlDataProviderManager Save()
		{		
			XmlNode tmpNode = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + _ID + "']");

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
