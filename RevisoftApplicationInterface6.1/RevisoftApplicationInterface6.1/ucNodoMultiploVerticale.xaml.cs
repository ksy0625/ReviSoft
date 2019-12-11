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
using UserControls2;
using System.IO;
using System.Data;

namespace UserControls
{
    public partial class ucNodoMultiploVerticale : UserControl
    {
        public int id;
        private string down = "./Images/icone/navigate_down.png";
        private string left = "./Images/icone/navigate_left.png";

        private XmlDataProviderManager _x;
		private XmlDataProviderManager _xBilanci;
        private string _ID;
		private string _IDTree;

		private Hashtable objList = new Hashtable();
		private Hashtable IDList = new Hashtable();

		private Hashtable _Sessioni;
		private int _SessioneNow;
        public WindowWorkArea Owner;
        XmlNodeList _xnl;
        Hashtable   _SessioniTitoli; 
        Hashtable   _SessioniID;
        string      _IDCliente;
        string      _IDSessione;
        string _tab;

        public bool ContieneBilancio = false;

        int countertabindex = 1;

        XmlDataProviderManager xdpm;

        bool afterpianificazione = false;
        Hashtable NodiPianificazione = null;
        ArrayList NodiPianificazioneID = null;

        public ucNodoMultiploVerticale()
        {
            InitializeComponent();            
        }

        private bool _ReadOnly = true;

        public bool ReadOnly 
        {
            set
            {
                _ReadOnly = value;
            }
        }


		public void Load(ref XmlDataProviderManager x, string ID, string tab, XmlNodeList xnl, Hashtable Sessioni, int SessioneNow, string IDTree, Hashtable SessioniTitoli, Hashtable SessioniID, string IDCliente, string IDSessione)
        {

            id = int.Parse(ID.ToString());
         
            MasterFile mf = MasterFile.Create();
            string filet = mf.GetTreeAssociatoFromFileData( Sessioni[SessioneNow].ToString() );
            filet = App.AppDataDataFolder + "\\" + filet;

            xdpm = new XmlDataProviderManager( filet );

			_Sessioni = Sessioni;
			_SessioneNow = SessioneNow;

            _x = x;
			_xBilanci = x.Clone();
			_ID = ID;
			_IDTree = IDTree;

            _tab = tab;

            _xnl = xnl;
            _SessioniTitoli = SessioniTitoli;
            _SessioniID = SessioniID;
            _IDCliente = IDCliente;
            _IDSessione = IDSessione;

            stack.Children.Clear();
            objList = new Hashtable();
            IDList = new Hashtable();

            if(ID == "204" || ID == "213" || ID == "214" || ID == "215" || ID == "215" || ID == "216" )
            {
                StackPanel sp2 = new StackPanel();
                sp2.Orientation = Orientation.Horizontal;
                sp2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

                Button btnDuplica = new Button();
                btnDuplica.Padding = new Thickness(5.0);
                btnDuplica.Content = "Duplica Ciclo";
                btnDuplica.Click += btnDuplica_Click;

                sp2.Children.Add( btnDuplica );

                stack.Children.Add( sp2 );
            }
            

            foreach (XmlNode item in xnl)
            {
                if (item.Name != "Node")
                {
                    continue;
                }
                

                Border b = new Border();
                b.CornerRadius = new CornerRadius(5.0);
                b.BorderBrush = Brushes.LightGray;
                b.BorderThickness = new Thickness(1.0);
                b.Padding = new Thickness(4.0);
                b.Margin = new Thickness(4.0); 

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

                var uriSource = new Uri( ((item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True")? left: down), UriKind.Relative );
                i.Source = new BitmapImage(uriSource);
                i.Height = 10.0;
                i.Width = 10.0;
                i.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

                g.Children.Add(i);

                StackPanel sp3 = new StackPanel();
                sp3.Orientation = Orientation.Horizontal;

                TextBlock tb = new TextBlock();
                tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                tb.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

                Hashtable n = new Hashtable();

                bool tobeclosed = false;
                XmlNode tmpa = null;

                switch (item.Attributes["ID"].Value)
                {
                    case "2016194":
                        n = mf.GetAnagrafica(Convert.ToInt32(Owner.IDCliente));
                        tb.Text = "Presidente: " + n["Presidente"].ToString();
                        /*
                        tmpa  = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + item.Attributes["ID"].Value + "]");
                        if (tmpa.Attributes["TitoloTab"] == null)
                        {
                            XmlAttribute attr = tmpa.OwnerDocument.CreateAttribute("TitoloTab");
                            tmpa.Attributes.Append(attr);
                        }
                        tmpa.Attributes["TitoloTab"].Value = tb.Text;
                        _x.Save();
                         */
                        tobeclosed = true;
                        break;
                    case "201613":
                        n = mf.GetAnagrafica(Convert.ToInt32(Owner.IDCliente));
                        tb.Text = "Membro Effettivo: " + n["MembroEffettivo"].ToString();
                        /*

                        tmpa = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + item.Attributes["ID"].Value + "]");
                        if (tmpa.Attributes["TitoloTab"] == null)
                        {
                            XmlAttribute attr = tmpa.OwnerDocument.CreateAttribute("TitoloTab");
                            tmpa.Attributes.Append(attr);
                        }
                        tmpa.Attributes["TitoloTab"].Value = tb.Text;
                        _x.Save();
                         */
                        tobeclosed = true;
                        break;
                    case "2016195":
                        n = mf.GetAnagrafica(Convert.ToInt32(Owner.IDCliente));
                        tb.Text = "Membro Effettivo: " + n["MembroEffettivo2"].ToString();
                        /*
                        tmpa = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + item.Attributes["ID"].Value + "]");
                        if (tmpa.Attributes["TitoloTab"] == null)
                        {
                            XmlAttribute attr = tmpa.OwnerDocument.CreateAttribute("TitoloTab");
                            tmpa.Attributes.Append(attr);
                        }
                        tmpa.Attributes["TitoloTab"].Value = tb.Text;
                        _x.Save();
                         */
                        tobeclosed = true;
                        break;
                    case "2016206":
                        n = mf.GetAnagrafica(Convert.ToInt32(Owner.IDCliente));
                        tb.Text = "Sindaco Supplente: " + ((n["SindacoSupplente"] == null)? "": n["SindacoSupplente"].ToString());
                        /*

                        tmpa = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + item.Attributes["ID"].Value + "]");
                        if (tmpa.Attributes["TitoloTab"] == null)
                        {
                            XmlAttribute attr = tmpa.OwnerDocument.CreateAttribute("TitoloTab");
                            tmpa.Attributes.Append(attr);
                        }
                        tmpa.Attributes["TitoloTab"].Value = tb.Text;
                        _x.Save();
                        */
                        tobeclosed = true;
                        break;
                    case "2016205":
                        n = mf.GetAnagrafica(Convert.ToInt32(Owner.IDCliente));
                        tb.Text = "Sindaco Supplente: " + ((n["SindacoSupplente2"] == null) ? "" : n["SindacoSupplente2"].ToString());
                        /*
                        tmpa = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + item.Attributes["ID"].Value + "]");
                        if (tmpa.Attributes["TitoloTab"] == null)
                        {
                            XmlAttribute attr = tmpa.OwnerDocument.CreateAttribute("TitoloTab");
                            tmpa.Attributes.Append(attr);
                        }
                        tmpa.Attributes["TitoloTab"].Value = tb.Text;
                        _x.Save();
                        */
                        tobeclosed = true;
                        break;
                    default:
                    if(item.Attributes["Tab"].Value=="")
                        tb.Text = item.Attributes["Titolo"].Value;
                     else
                       tb.Text = item.Attributes["Tab"].Value;
                        break;
                }

                
                tb.Tag = item.Attributes["ID"].Value;

                tb.FontSize = 13;
                tb.FontWeight = FontWeights.Bold;
                tb.Margin = new Thickness(5.0);
                tb.Foreground = Brushes.Gray;

                sp3.Children.Add(tb);

                if (ID == "204" || ID == "213" || ID == "214" || ID == "215" || ID == "215" || ID == "216")
                {
                    if (item.Attributes["Duplicato"] != null)
                    {
                        Button btnRinominaDuplica = new Button();
                        btnRinominaDuplica.Padding = new Thickness(5.0);
                        btnRinominaDuplica.Content = "Rinomina Ciclo";
                        btnRinominaDuplica.Margin = new Thickness(10, 0, 0, 0);
                        btnRinominaDuplica.Tag = item.Attributes["ID"].Value;
                        btnRinominaDuplica.Click += btnRinominaDuplica_Click;

                        sp3.Children.Add(btnRinominaDuplica);

                        Button btnEliminaDuplica = new Button();
                        btnEliminaDuplica.Padding = new Thickness(5.0);
                        btnEliminaDuplica.Content = "Elimina Ciclo";
                        btnEliminaDuplica.Margin = new Thickness(10, 0, 0, 0);
                        btnEliminaDuplica.Tag = item.Attributes["ID"].Value;
                        btnEliminaDuplica.Click += btnEliminaDuplica_Click;

                        sp3.Children.Add(btnEliminaDuplica);
                    }
                }

                if (ID == "222")
                {
                    if (item.Attributes["ID"] != null && (item.Attributes["ID"].Value == "223" || item.Attributes["ID"].Value == "224" || item.Attributes["ID"].Value == "225"))
                    {
                        Button btnCopiada1211 = new Button();
                        btnCopiada1211.Padding = new Thickness(5.0);
                        btnCopiada1211.Content = "Copia dati da 1.21.1";
                        btnCopiada1211.Margin = new Thickness(50, 0, 0, 0);
                        btnCopiada1211.Background = Brushes.LightBlue;
                        btnCopiada1211.Tag = item.Attributes["ID"].Value;
                        btnCopiada1211.Click += BtnCopiada1211_Click;

                        sp3.Children.Add(btnCopiada1211);
                    }
                }

                sp3.SetValue(Grid.RowProperty, 0);
                sp3.SetValue(Grid.ColumnProperty, 1);

                g.Children.Add(sp3);

                string file = "";
                string tipoBilancio = "";

                uc_Excel_Bilancio uce_b;
                XmlDataProviderManager _x_AP = null;



                switch (item.Attributes["Tipologia"].Value)
                {
                    case "Testo":
                        if (afterpianificazione)
                        {
                            afterpianificazione = false;
                            //Gestisco caso particolare in cui non devo appoggiarmi alle pianificazioni
                            //vedere poi come metterlo nell'xml e togliere la logica
                            bool displayTesto = item.Attributes["Codice"] != null && item.Attributes["Codice"].Value.StartsWith("3.4.");
                            if (displayTesto)
                            {
                                ucTesto Testo = new ucTesto();
                                Testo.ReadOnly = _ReadOnly;
                                Testo.Load( item.Attributes["ID"].Value,IDCliente,IDSessione);

                                Testo.SetValue(Grid.RowProperty, 1);
                                Testo.SetValue(Grid.ColumnProperty, 1);

                                Testo.Visibility = System.Windows.Visibility.Collapsed;
                                uriSource = new Uri(left, UriKind.Relative);
                                ((Image)(g.Children[0])).Source = new BitmapImage(uriSource);

                                ////gestione finestra aperta solo se dati presenti
                                //if (_IDTree == "1" && (item.Attributes["ID"].Value == "258" || item.Attributes["ID"].Value == "259" || item.Attributes["ID"].Value == "260" || item.Attributes["ID"].Value == "261" || item.Attributes["ID"].Value == "262"))
                                //{
                                //    Testo.Visibility = System.Windows.Visibility.Collapsed;
                                //    uriSource = new Uri(left, UriKind.Relative);
                                //    ((Image)(g.Children[0])).Source = new BitmapImage(uriSource);
                                //}
                                //else
                                //{
                                //    Testo.Visibility = System.Windows.Visibility.Visible;
                                //    uriSource = new Uri(down, UriKind.Relative);
                                //    ((Image)(g.Children[0])).Source = new BitmapImage(uriSource);
                                //}

                                objList.Add(item.Attributes["ID"].Value, Testo);
                                IDList.Add(item.Attributes["ID"].Value, "Testo");
                                if (item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True")
                                {
                                    Testo.Visibility = System.Windows.Visibility.Collapsed;
                                }
                                g.Children.Add(Testo);
                            }
                            else if (NodiPianificazione.Count > 0)
                            {
                                StackPanel stphere = new StackPanel();
                                stphere.Orientation = Orientation.Vertical;
                                stphere.Visibility = Visibility.Collapsed;

                                foreach (string de in NodiPianificazioneID)
                                {
                                    XmlNode tmp = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + item.Attributes["ID"].Value + "]");

                                    if (tmp == null)
                                    {
                                        string xml = "<Dato ID=\"" + item.Attributes["ID"].Value + "\"/>";

                                        XmlDocument doctmp = new XmlDocument();
                                        doctmp.LoadXml(xml);

                                        XmlNode tmpNode_int = doctmp.SelectSingleNode("Dato");
                                        XmlNode node = _x.Document.ImportNode(tmpNode_int, true);

                                        _x.Document.SelectSingleNode("/Dati").AppendChild(node);
                                    }

                                    tmp = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + item.Attributes["ID"].Value + "]/Valore[@ID=" + de + "]");

                                    if (tmp == null)
                                    {
                                        string xml = "<Valore ID=\"" + de + "\" value=\"\"/>";

                                        XmlDocument doctmp = new XmlDocument();
                                        doctmp.LoadXml(xml);

                                        XmlNode tmpNode_int = doctmp.SelectSingleNode("Valore");
                                        XmlNode node = _x.Document.ImportNode(tmpNode_int, true);

                                        _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + item.Attributes["ID"].Value + "']").AppendChild(node);

                                        tmp = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + item.Attributes["ID"].Value + "']/Valore[@ID=" + de + "]");
                                    }

                                    TextBlock stpTxtblk = new TextBlock();
                                    stpTxtblk.FontWeight = FontWeights.Bold;
                                    stpTxtblk.Text = NodiPianificazione[de].ToString();

                                    if (tmp.Attributes["name"] == null)
                                    {
                                        XmlAttribute attr = tmp.OwnerDocument.CreateAttribute("name");
                                        tmp.Attributes.Append(attr);
                                    }
                                    tmp.Attributes["name"].Value = NodiPianificazione[de].ToString();

                                    stphere.Children.Add(stpTxtblk);

                                    TextBox stpTxtbox = new TextBox();
                                    stpTxtbox.Name = "txt_" + item.Attributes["ID"].Value + "_" + de;
                                    stpTxtbox.Text = tmp.Attributes["value"].Value;
                                    stpTxtbox.TextWrapping = TextWrapping.Wrap;
                                    stpTxtbox.Width = 1000;
                                    stpTxtbox.LostFocus += StpTxtbox_LostFocus;

                                    stphere.Children.Add(stpTxtbox);
                                }

                                stphere.SetValue(Grid.RowProperty, 1);
                                stphere.SetValue(Grid.ColumnProperty, 1);
                                //stphere.Visibility = System.Windows.Visibility.Visible;
                                uriSource = new Uri(down, UriKind.Relative);
                                ((Image)(g.Children[0])).Source = new BitmapImage(uriSource);
                                g.Children.Add(stphere);
                            }                            
                            else
                            {
                                TextBlock txtblk = new TextBlock();
                                txtblk.Text = "Nessuna voce di pianificazione con controllo a SI";

                                txtblk.SetValue(Grid.RowProperty, 1);
                                txtblk.SetValue(Grid.ColumnProperty, 1);
                                txtblk.Visibility = System.Windows.Visibility.Visible;
                                uriSource = new Uri(down, UriKind.Relative);
                                ((Image)(g.Children[0])).Source = new BitmapImage(uriSource);
                                g.Children.Add(txtblk);
                            }
                        }
                        else
                        {
                            ucTesto Testo = new ucTesto();
                            Testo.ReadOnly = _ReadOnly;
                            Testo.Load(item.Attributes["ID"].Value,IDCliente,IDSessione);
                            Testo.SetValue(Grid.RowProperty, 1);
                            Testo.SetValue(Grid.ColumnProperty, 1);

                            //gestione finestra aperta solo se dati presenti
                            if (_IDTree == "1" && (item.Attributes["ID"].Value == "258" || item.Attributes["ID"].Value == "259" || item.Attributes["ID"].Value == "260" || item.Attributes["ID"].Value == "261" || item.Attributes["ID"].Value == "262"))
                            {
                                Testo.Visibility = System.Windows.Visibility.Collapsed;
                                uriSource = new Uri(left, UriKind.Relative);
                                ((Image)(g.Children[0])).Source = new BitmapImage(uriSource);
                            }
                            else
                            {
                                Testo.Visibility = System.Windows.Visibility.Visible;
                                uriSource = new Uri(down, UriKind.Relative);
                                ((Image)(g.Children[0])).Source = new BitmapImage(uriSource);
                            }

                            objList.Add(item.Attributes["ID"].Value, Testo);
                            IDList.Add(item.Attributes["ID"].Value, "Testo");
                            if (item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True")
                            {
                                Testo.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            g.Children.Add(Testo);
                        }
                        break;

                    case "Relazione: Testo proposto a Scelta Multipla":
                        ucTestoPropostoMultiplo Testopm = new ucTestoPropostoMultiplo();
                        Testopm.ReadOnly = _ReadOnly;
                        Testopm.Load(item.Attributes["ID"].Value,IDCliente,IDSessione);

                        Testopm.SetValue( Grid.RowProperty, 1 );
                        Testopm.SetValue( Grid.ColumnProperty, 1 );

                        objList.Add( item.Attributes["ID"].Value, Testopm );
                        IDList.Add( item.Attributes["ID"].Value, "Relazione: Testo proposto a Scelta Multipla" );
                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            Testopm.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        g.Children.Add( Testopm );
                        break;
                    case "Accettazionedelrischio_6_1":
                       ucAccettazionedelrischio_6_1 ucaccr = new ucAccettazionedelrischio_6_1();
                        ucaccr.ReadOnly = _ReadOnly;
                        ucaccr.LoadDataSource( item.Attributes["ID"].Value,IDCliente,IDSessione);
                   
                        ucaccr.SetValue( Grid.RowProperty, 1 );
                        ucaccr.SetValue( Grid.ColumnProperty, 1 );

                        objList.Add(item.Attributes["ID"].Value, ucaccr);
                        IDList.Add(item.Attributes["ID"].Value, "Accettazionedelrischio_6_1");
                       
                        g.Children.Add(ucaccr);
                        break;
                      break;
                    case "Excel: Compensi e Risorse":
                        ucCompensiERisorse uce_cer = new ucCompensiERisorse();
                        uce_cer.ReadOnly = _ReadOnly;
                        uce_cer.Load( item.Attributes["ID"].Value,IDCliente,IDSessione);
                     
                        uce_cer.SetValue(Grid.RowProperty, 1);
                        uce_cer.SetValue(Grid.ColumnProperty, 1);

                        objList.Add(item.Attributes["ID"].Value, uce_cer);
                        IDList.Add(item.Attributes["ID"].Value, "Excel: Compensi e Risorse");
                        if (item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True")
                        {
                            uce_cer.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        g.Children.Add(uce_cer);
                        break;

                    case "Tabella":
                        ucTabella Tabella = new ucTabella();
                        Tabella.ReadOnly = _ReadOnly;
                        Tabella.Load(item.Attributes["ID"].Value, "", _IDTree, "", IDCliente, IDSessione);

                        Tabella.SetValue(Grid.RowProperty, 1);
                        Tabella.SetValue(Grid.ColumnProperty, 1);

						objList.Add(item.Attributes["ID"].Value, Tabella);
						IDList.Add(item.Attributes["ID"].Value, "Tabella");
                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            Tabella.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        g.Children.Add(Tabella);
                        break;

                    case "Prospetto IVA":
                        ucProspettoIVA prospettoiva = new ucProspettoIVA();
                        prospettoiva.ReadOnly = _ReadOnly;

                        if (_Sessioni.Contains(_SessioneNow + 1))
                        {
                            prospettoiva.PrevSession = _Sessioni[_SessioneNow + 1].ToString();
                        }

                        if(item.Attributes["ID"].Value == "180")
                        {
                            prospettoiva.normal = false;
                        }

                        prospettoiva.LoadDataSource( item.Attributes["ID"].Value ,IDCliente,IDSessione);

                        prospettoiva.SetValue( Grid.RowProperty, 1 );
                        prospettoiva.SetValue( Grid.ColumnProperty, 1 );

                        objList.Add( item.Attributes["ID"].Value, prospettoiva );
                        IDList.Add( item.Attributes["ID"].Value, "Prospetto IVA" );
                        //if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        //{
                            prospettoiva.Visibility = System.Windows.Visibility.Collapsed;
                        //}
                        g.Children.Add( prospettoiva );
                        break;

                    case "Tabella Replicabile":
                        ucTabellaReplicata TabellaReplicata = new ucTabellaReplicata();
                        TabellaReplicata.ReadOnly = _ReadOnly;
                        TabellaReplicata.Load( item.Attributes["ID"].Value, item.Attributes["Tab"].Value, _IDTree, IDCliente, IDSessione);

                        TabellaReplicata.SetValue(Grid.RowProperty, 1);
                        TabellaReplicata.SetValue(Grid.ColumnProperty, 1);

                        //gestione finestra aperta solo se dati presenti
                        if ( _IDTree == "1" && ( item.Attributes["ID"].Value == "250" || item.Attributes["ID"].Value == "251" ) )
                        {
                            TabellaReplicata.Visibility = System.Windows.Visibility.Collapsed;
                            uriSource = new Uri( left, UriKind.Relative );
                            ( (Image)( g.Children[0] ) ).Source = new BitmapImage( uriSource );
                        }
                        else
                        {
                            TabellaReplicata.Visibility = System.Windows.Visibility.Visible;
                            uriSource = new Uri( down, UriKind.Relative );
                            ( (Image)( g.Children[0] ) ).Source = new BitmapImage( uriSource );
                        }

						objList.Add(item.Attributes["ID"].Value, TabellaReplicata);
						IDList.Add(item.Attributes["ID"].Value, "Tabella Replicabile");
                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            TabellaReplicata.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        g.Children.Add(TabellaReplicata);
                        break;

                                            
                
                    case "CheckListCicli":
                        ucCheckListCicli CheckListc = new ucCheckListCicli();
                        CheckListc.ReadOnly = _ReadOnly;
                        CheckListc.Load( item.Attributes["ID"].Value,IDCliente,IDSessione );

                        CheckListc.SetValue( Grid.RowProperty, 1 );
                        CheckListc.SetValue( Grid.ColumnProperty, 1 );

                        CheckListc.Visibility = System.Windows.Visibility.Visible;
                        uriSource = new Uri( down, UriKind.Relative );
                        ( (Image)( g.Children[0] ) ).Source = new BitmapImage( uriSource );
                        objList.Add( item.Attributes["ID"].Value, CheckListc );
                        IDList.Add( item.Attributes["ID"].Value, "CheckListCicli" );
                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            CheckListc.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        g.Children.Add( CheckListc );
                        break;
                    case "CheckListCicliMulti":
                        ucCheckListCicli CheckListcm = new ucCheckListCicli();
                        CheckListcm.ReadOnly = _ReadOnly;
                        CheckListcm.Load(  item.Attributes["ID"].Value,IDCliente,IDSessione );

                        CheckListcm.SetValue( Grid.RowProperty, 1 );
                        CheckListcm.SetValue( Grid.ColumnProperty, 1 );

                        CheckListcm.Visibility = System.Windows.Visibility.Visible;
                        uriSource = new Uri( down, UriKind.Relative );
                        ( (Image)( g.Children[0] ) ).Source = new BitmapImage( uriSource );
                        objList.Add( item.Attributes["ID"].Value, CheckListcm );
                        IDList.Add( item.Attributes["ID"].Value, "CheckListCicli" );
                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            CheckListcm.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        g.Children.Add( CheckListcm );
                        break;

                    case "Check List":
                        ucCheckList CheckList = new ucCheckList();
                        CheckList.ReadOnly = _ReadOnly;
                        CheckList.Load( item.Attributes["ID"].Value,IDCliente,IDSessione);

                        CheckList.SetValue(Grid.RowProperty, 1);
                        CheckList.SetValue(Grid.ColumnProperty, 1);

                        //gestione finestra aperta solo se dati presenti
                        if ( ( _IDTree == "1" && ( item.Attributes["ID"].Value == "71" || item.Attributes["ID"].Value == "72" || item.Attributes["ID"].Value == "73" || item.Attributes["ID"].Value == "74" || item.Attributes["ID"].Value == "75" ) ) || ( _IDTree == "4" && ( item.Attributes["ID"].Value == "80" || item.Attributes["ID"].Value == "81" || item.Attributes["ID"].Value == "82" || item.Attributes["ID"].Value == "83" || item.Attributes["ID"].Value == "85" || item.Attributes["ID"].Value == "86" || item.Attributes["ID"].Value == "87" || item.Attributes["ID"].Value == "88" || item.Attributes["ID"].Value == "89" || item.Attributes["ID"].Value == "90" || item.Attributes["ID"].Value == "91" || item.Attributes["ID"].Value == "92" || item.Attributes["ID"].Value == "93" || item.Attributes["ID"].Value == "94" || item.Attributes["ID"].Value == "95" || item.Attributes["ID"].Value == "96" || item.Attributes["ID"].Value == "97" || item.Attributes["ID"].Value == "98" || item.Attributes["ID"].Value == "99" || item.Attributes["ID"].Value == "100" || item.Attributes["ID"].Value == "101" ) ) )
                        {
                            CheckList.Visibility = System.Windows.Visibility.Collapsed;
                            uriSource = new Uri( left, UriKind.Relative );
                            ( (Image)( g.Children[0] ) ).Source = new BitmapImage( uriSource );
                        }
                        else
                        {
                            CheckList.Visibility = System.Windows.Visibility.Visible;
                            uriSource = new Uri( down, UriKind.Relative );
                            ( (Image)( g.Children[0] ) ).Source = new BitmapImage( uriSource );
                        }

                        if(tobeclosed)
                        {
                            CheckList.Visibility = System.Windows.Visibility.Collapsed;
                            uriSource = new Uri(left, UriKind.Relative);
                            ((Image)(g.Children[0])).Source = new BitmapImage(uriSource);
                        }

						objList.Add(item.Attributes["ID"].Value, CheckList);
						IDList.Add(item.Attributes["ID"].Value, "Check List");
                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            CheckList.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        g.Children.Add(CheckList);
                        break;

                    case "Check List +":
                        ucCheckListPlus CheckListPlus = new ucCheckListPlus();
                        CheckListPlus.ReadOnly = _ReadOnly;
                        CheckListPlus.Load( item.Attributes["ID"].Value,IDCliente, Sessioni[SessioneNow].ToString());

                        CheckListPlus.SetValue(Grid.RowProperty, 1);
                        CheckListPlus.SetValue(Grid.ColumnProperty, 1);

						objList.Add(item.Attributes["ID"].Value, CheckListPlus);
						IDList.Add(item.Attributes["ID"].Value, "Check List +");
                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            CheckListPlus.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        g.Children.Add(CheckListPlus);
                        break;
                     case "Check List + 6_1":
                        ucCheckListPlus_6_1 CheckListPlus_6_1 = new ucCheckListPlus_6_1();
                        CheckListPlus_6_1.ReadOnly = _ReadOnly;
                        CheckListPlus_6_1.Load( item.Attributes["ID"].Value,IDCliente, Sessioni[SessioneNow].ToString());

                        CheckListPlus_6_1.SetValue(Grid.RowProperty, 1);
                        CheckListPlus_6_1.SetValue(Grid.ColumnProperty, 1);

						objList.Add(item.Attributes["ID"].Value, CheckListPlus_6_1);
						IDList.Add(item.Attributes["ID"].Value, "Check List + 6_1");
                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            CheckListPlus_6_1.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        g.Children.Add(CheckListPlus_6_1);
                        break;
                    //case "Nodo Multiplo":
                    //    ucNodoMultiplo NodoMultiplo = new ucNodoMultiplo();
                    //    NodoMultiplo.ReadOnly = _ReadOnly;
                    //    NodoMultiplo.Load(ref _x, item.Attributes["ID"].Value, item.Attributes["Tab"].Value, item.ChildNodes, new Hashtable(), 0 , "", _IDTree);

                    //    NodoMultiplo.SetValue(Grid.RowProperty, 1);
                    //    NodoMultiplo.SetValue(Grid.ColumnProperty, 1);

                    //    objList.Add(item.Attributes["ID"].Value, NodoMultiplo);
                    //    IDList.Add(item.Attributes["ID"].Value, "Nodo Multiplo");
                    //    if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                    //    {
                    //        NodoMultiplo.Visibility = System.Windows.Visibility.Collapsed;
                    //    }
                    //    g.Children.Add(NodoMultiplo);
                    //    break;

                    case "Nodo Multiplo":
                        ucNodoMultiploVerticale NodoMultiplo = new ucNodoMultiploVerticale();
                        NodoMultiplo.ReadOnly = _ReadOnly;
                        NodoMultiplo.Load(ref  x,  ID,  tab,  xnl,  Sessioni,  SessioneNow,  IDTree,  SessioniTitoli,  SessioniID,  IDCliente,  IDSessione );

                        NodoMultiplo.SetValue( Grid.RowProperty, 1 );
                        NodoMultiplo.SetValue( Grid.ColumnProperty, 1 );

                        objList.Add( item.Attributes["ID"].Value, NodoMultiplo );
                        IDList.Add( item.Attributes["ID"].Value, "Nodo Multiplo" );
                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            NodoMultiplo.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        g.Children.Add( NodoMultiplo );
                        break;

					case "Excel: Errori Rilevati":
						uc_Excel_ErroriRilevati uce_er = new uc_Excel_ErroriRilevati();

						uce_er.ReadOnly = _ReadOnly;

						uce_er.LoadDataSource( item.Attributes["ID"].Value,IDCliente,IDSessione);
						
						uce_er.SetValue(Grid.RowProperty, 1);
						uce_er.SetValue(Grid.ColumnProperty, 1);

                        uce_er.Visibility = System.Windows.Visibility.Collapsed;
                            uriSource = new Uri( left, UriKind.Relative );
                            ( (Image)( g.Children[0] ) ).Source = new BitmapImage( uriSource );

						objList.Add(item.Attributes["ID"].Value, uce_er);
						IDList.Add(item.Attributes["ID"].Value, "Excel: Errori Rilevati");
                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            uce_er.Visibility = System.Windows.Visibility.Collapsed;
                        }
						g.Children.Add(uce_er);
						break;

                    case "Excel: Errori Rilevati New":
                        uc_Excel_ErroriRilevatiNew uce_ernew = new uc_Excel_ErroriRilevatiNew();

                        uce_ernew.Owner = Owner;
                        uce_ernew.ReadOnly = _ReadOnly;

                        uce_ernew.LoadDataSource(  item.Attributes["ID"].Value,IDCliente,IDSessione );

                        uce_ernew.SetValue( Grid.RowProperty, 1 );
                        uce_ernew.SetValue( Grid.ColumnProperty, 1 );

                        uce_ernew.Visibility = System.Windows.Visibility.Collapsed;
                        uriSource = new Uri( left, UriKind.Relative );
                        ( (Image)( g.Children[0] ) ).Source = new BitmapImage( uriSource );

                        objList.Add( item.Attributes["ID"].Value, uce_ernew );
                        IDList.Add( item.Attributes["ID"].Value, "Excel: Errori Rilevati New" );
                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            uce_ernew.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        g.Children.Add( uce_ernew );
                        break;


					case "Excel: Bilancio Patrimoniale Attivo":
                        cBusinessObjects.idcliente = int.Parse(IDCliente);
                        cBusinessObjects.idsessione = int.Parse(IDSessione);
                        uce_b = new uc_Excel_Bilancio(countertabindex);

                        uce_b.Titolo = item.Attributes["Tab"].Value;

						uce_b.ReadOnly = _ReadOnly;

                        //if (Sessioni.Contains((SessioneNow + 1)))
                        //{
                        //	_x_AP = new XmlDataProviderManager(Sessioni[(SessioneNow + 1)].ToString());
                        //}
                        if (ID == "321")
                        {
                            if (this.Owner != null)
                            {
                                this.Owner.txtTitoloNodo.Text = this.Owner.txtTitoloNodo.Text.Split(' ')[0] + " Consolidato";
                            }
                            file = App.AppTemplateBilancio_Attivo2016_Consolidato;
                        }
                        else
                        {
                            DataTable datibilanciotestata = null;
                            DataRow nodedata = null;
                            datibilanciotestata = cBusinessObjects.GetData(int.Parse(ID), typeof(Excel_Bilancio_Testata));
                            nodedata = null;
                            foreach (DataRow dt in datibilanciotestata.Rows)
                            {
                                nodedata = dt;
                                tipoBilancio = dt["tipoBilancio"].ToString();
                            }
                            if(nodedata == null)
                            {
                                nodedata = datibilanciotestata.Rows.Add(int.Parse(ID), cBusinessObjects.idcliente, cBusinessObjects.idsessione);
                            }

                            if ( tipoBilancio == "")
                            {

                                        wSceltaTipologiaBilancio stp = new wSceltaTipologiaBilancio();
                                        //stp.Owner = this.Owner;
                                        stp.setFather(ID);
                                        stp.ShowDialog();
                                        if (stp.typechosen != "")
                                        {
                                            tipoBilancio = stp.typechosen;
                                            nodedata["tipoBilancio"]= tipoBilancio;
                                            cBusinessObjects.SaveData(int.Parse(ID), datibilanciotestata, typeof(Excel_Bilancio_Testata));

                                        }
                                        else
                                        {
                                            this.Owner.ConsentiChiusuraFinestra = true;

                                            this.Owner.Close();
                                            return;
                                        }
                                
                            }

                            switch (tipoBilancio)
                            {
                                case "2016":
                                    try
                                    {
                                        this.Owner.txtTitoloNodo.Text = this.Owner.txtTitoloNodo.Text.Split(' ')[0] + " Bilancio Ordinario dal 2016";
                                    }
                                    catch (Exception ex)
                                    {
                                        string log = ex.Message;
                                    }
                                    file = App.AppTemplateBilancio_Attivo2016;
                                    break;
                                default:
                                    try
                                    {
                                        this.Owner.txtTitoloNodo.Text = this.Owner.txtTitoloNodo.Text.Split(' ')[0] + " Bilancio Ordinario Ante 2016";
                                    }
                                    catch (Exception ex)
                                    {
                                        string log = ex.Message;
                                    }
                                    file = App.AppTemplateBilancio_Attivo;
                                    break;
                            }
                        }                    

                        uce_b.LoadDataSource(ref _xBilanci, ID, _x_AP, file, IDCliente, IDSessione);

                        uce_b.SetValue(Grid.RowProperty, 1);
						uce_b.SetValue(Grid.ColumnProperty, 1);

						if (!objList.Contains(ID))
						{
							ArrayList al = new ArrayList();
							al.Add(uce_b);
							objList.Add(ID, al);
							IDList.Add(ID, "Excel: Bilancio Patrimoniale Attivo");
						}
						else
						{
							((ArrayList)(objList[ID])).Add(uce_b);
						}

                        uce_b.Visibility = System.Windows.Visibility.Collapsed;

                        g.Children.Add(uce_b);

                        countertabindex = uce_b.countertabindex;
                        break;

					case "Excel: Bilancio Patrimoniale Passivo":
                        cBusinessObjects.idcliente = int.Parse(IDCliente);
                        cBusinessObjects.idsessione = int.Parse(IDSessione);
                        file = App.AppTemplateBilancio_Passivo;

						uce_b = new uc_Excel_Bilancio(countertabindex);

                        uce_b.Titolo = item.Attributes["Tab"].Value;

						uce_b.ReadOnly = _ReadOnly;

                        //if (Sessioni.Contains((SessioneNow + 1)))
                        //{
                        //	_x_AP = new XmlDataProviderManager(Sessioni[(SessioneNow + 1)].ToString());
                        //}
                        if (ID == "321")
                        {
                            if (this.Owner != null)
                            {
                                this.Owner.txtTitoloNodo.Text = this.Owner.txtTitoloNodo.Text.Split(' ')[0] + " Consolidato";
                            }
                            file = App.AppTemplateBilancio_Passivo2016_Consolidato;
                        }
                        else
                        {
                            DataTable datibilanciotestata2 = null;
                            DataRow nodedata2 = null;
                            datibilanciotestata2 = cBusinessObjects.GetData(int.Parse(ID), typeof(Excel_Bilancio_Testata));
                            nodedata2 = null;
                            foreach (DataRow dt in datibilanciotestata2.Rows)
                            {
                                nodedata2 = dt;
                                tipoBilancio = dt["tipoBilancio"].ToString();
                            }
                            if (nodedata2 == null)
                            {
                                nodedata2 = datibilanciotestata2.Rows.Add(int.Parse(ID), cBusinessObjects.idcliente, cBusinessObjects.idsessione);
                            }

                            if (tipoBilancio == "")
                            {
                                        wSceltaTipologiaBilancio stp = new wSceltaTipologiaBilancio();
                                        //stp.Owner = this.Owner;
                                        stp.setFather(ID);
                                        stp.ShowDialog();
                                        if (stp.typechosen != "")
                                        {
                                            tipoBilancio = stp.typechosen;
                                            nodedata2["tipoBilancio"] = tipoBilancio;
                                            cBusinessObjects.SaveData(int.Parse(ID), datibilanciotestata2, typeof(Excel_Bilancio_Testata));

                                        }
                                        else
                                        {
                                            this.Owner.ConsentiChiusuraFinestra = true;

                                            this.Owner.Close();
                                            return;
                                        }
                                 
                            }

                            switch (tipoBilancio)
                            {
                                case "2016":
                                    file = App.AppTemplateBilancio_Passivo2016;
                                    break;
                                default:
                                    file = App.AppTemplateBilancio_Passivo;
                                    break;
                            }
                        }

                        uce_b.LoadDataSource(ref _xBilanci, ID, _x_AP, file, IDCliente, IDSessione);

                        uce_b.SetValue(Grid.RowProperty, 1);
						uce_b.SetValue(Grid.ColumnProperty, 1);

						if (!objList.Contains(ID))
						{
							ArrayList al = new ArrayList();
							al.Add(uce_b);
							objList.Add(ID, al);
							IDList.Add(ID, "Excel: Bilancio Patrimoniale Attivo");
						}
						else
						{
							((ArrayList)(objList[ID])).Add(uce_b);
						}

                        
                            uce_b.Visibility = System.Windows.Visibility.Collapsed;
                       
						g.Children.Add(uce_b);

                        countertabindex = uce_b.countertabindex;
                        break;

					case "Excel: Bilancio Conto Economico":
                        cBusinessObjects.idcliente = int.Parse(IDCliente);
                        cBusinessObjects.idsessione = int.Parse(IDSessione);
                        file = App.AppTemplateBilancio_ContoEconomico;

						uce_b = new uc_Excel_Bilancio(countertabindex);

                        uce_b.Titolo = item.Attributes["Tab"].Value;

						uce_b.ReadOnly = _ReadOnly;

                        //if (Sessioni.Contains((SessioneNow + 1)))
                        //{
                        //	_x_AP = new XmlDataProviderManager(Sessioni[(SessioneNow + 1)].ToString());
                        //}
                        if (ID == "321")
                        {
                            if (this.Owner != null)
                            {
                                this.Owner.txtTitoloNodo.Text = this.Owner.txtTitoloNodo.Text.Split(' ')[0] + " Consolidato";
                            }
                            file = App.AppTemplateBilancio_ContoEconomico2016_Consolidato;
                        }
                        else
                        {
                            DataTable datibilanciotestata3 = null;
                            DataRow nodedata3 = null;
                            datibilanciotestata3 = cBusinessObjects.GetData(int.Parse(ID), typeof(Excel_Bilancio_Testata));
                            nodedata3 = null;
                            foreach (DataRow dt in datibilanciotestata3.Rows)
                            {
                                nodedata3 = dt;
                                tipoBilancio = dt["tipoBilancio"].ToString();
                            }
                            if (nodedata3== null)
                            {
                                nodedata3 = datibilanciotestata3.Rows.Add(int.Parse(ID), cBusinessObjects.idcliente, cBusinessObjects.idsessione);
                            }

                            if (tipoBilancio == "")
                            {
                                        wSceltaTipologiaBilancio stp = new wSceltaTipologiaBilancio();
                                        //stp.Owner = this.Owner;
                                        stp.setFather(ID);
                                        stp.ShowDialog();
                                        if (stp.typechosen != "")
                                        {
                                            tipoBilancio = stp.typechosen;
                                            nodedata3["tipoBilancio"] = tipoBilancio;
                                            cBusinessObjects.SaveData(int.Parse(ID), datibilanciotestata3, typeof(Excel_Bilancio_Testata));

                                        }
                                        else
                                        {
                                            this.Owner.ConsentiChiusuraFinestra = true;

                                            this.Owner.Close();
                                            return;
                                        }
                                
                            }

                            switch (tipoBilancio)
                            {
                                case "2016":
                                    file = App.AppTemplateBilancio_ContoEconomico2016;
                                    break;
                                default:
                                    file = App.AppTemplateBilancio_ContoEconomico;
                                    break;
                            }
                        }

                        uce_b.LoadDataSource(ref _xBilanci, ID, _x_AP, file, IDCliente, IDSessione);

                        uce_b.SetValue(Grid.RowProperty, 1);
						uce_b.SetValue(Grid.ColumnProperty, 1);

						if (!objList.Contains(ID))
						{
							ArrayList al = new ArrayList();
							al.Add(uce_b);
							objList.Add(ID, al);
							IDList.Add(ID, "Excel: Bilancio Patrimoniale Attivo");
						}
						else
						{
							((ArrayList)(objList[ID])).Add(uce_b);
						}

                     
                            uce_b.Visibility = System.Windows.Visibility.Collapsed;
                      
						g.Children.Add(uce_b);

                        countertabindex = uce_b.countertabindex;
                        break;

					case "Excel: Bilancio Abbreviato Patrimoniale Attivo":
                        cBusinessObjects.idcliente = int.Parse(IDCliente);
                        cBusinessObjects.idsessione = int.Parse(IDSessione);
                        file = App.AppTemplateBilancioAbbreviato_Attivo;

						uce_b = new uc_Excel_Bilancio(countertabindex);

                        uce_b.Titolo = item.Attributes["Tab"].Value;

						uce_b.ReadOnly = _ReadOnly;
						uce_b.Abbreviato = true;

                        //if (Sessioni.Contains((SessioneNow + 1)))
                        //{
                        //	_x_AP = new XmlDataProviderManager(Sessioni[(SessioneNow + 1)].ToString());
                        //}
                        DataTable datibilanciotestata4 = null;
                        DataRow nodedata4 = null;
                        datibilanciotestata4 = cBusinessObjects.GetData(int.Parse(ID), typeof(Excel_Bilancio_Testata));
                        nodedata4 = null;
                        foreach (DataRow dt in datibilanciotestata4.Rows)
                        {
                            nodedata4 = dt;
                            tipoBilancio = dt["tipoBilancio"].ToString();
                        }
                        if (nodedata4 == null)
                        {
                            nodedata4 = datibilanciotestata4.Rows.Add(int.Parse(ID), cBusinessObjects.idcliente, cBusinessObjects.idsessione);
                        }

                        if (tipoBilancio == "")
                        {
                                    wSceltaTipologiaBilancio stp = new wSceltaTipologiaBilancio();
                                    //stp.Owner = this.Owner;
                                    stp.setFather(ID);
                                    stp.ShowDialog();
                                    if (stp.typechosen != "")
                                    {
                                        tipoBilancio = stp.typechosen;
                                        nodedata4["tipoBilancio"] = tipoBilancio;
                                        cBusinessObjects.SaveData(int.Parse(ID), datibilanciotestata4, typeof(Excel_Bilancio_Testata));

                                    }
                                    else
                                    {
                                        this.Owner.ConsentiChiusuraFinestra = true;

                                        this.Owner.Close();
                                        return;
                                    }
                              
                        }

                        switch (tipoBilancio)
                        {
                            case "Micro":
                                try
                                {
                                    this.Owner.txtTitoloNodo.Text = this.Owner.txtTitoloNodo.Text.Split(' ')[0] + " Bilancio Micro";
                                }
                                catch (Exception ex)
                                {
                                    string log = ex.Message;
                                }
                                
                                file = App.AppTemplateBilancioMicro_Attivo2016;
                                break;
                            case "2016":
                                try
                                {
                                    this.Owner.txtTitoloNodo.Text = this.Owner.txtTitoloNodo.Text.Split(' ')[0] + " Bilancio Abbreviato dal 2016";
                                }
                                catch (Exception ex)
                                {
                                    string log = ex.Message;
                                }
                                
                                file = App.AppTemplateBilancioAbbreviato_Attivo2016;
                                break;
                            default:
                                try
                                {
                                    this.Owner.txtTitoloNodo.Text = this.Owner.txtTitoloNodo.Text.Split(' ')[0] + " Bilancio Abbreviato Ante 2016";
                                }
                                catch (Exception ex)
                                {
                                    string log = ex.Message;
                                }
                                
                                file = App.AppTemplateBilancioAbbreviato_Attivo;
                                break;
                        }

                        uce_b.LoadDataSource(ref _xBilanci, ID, _x_AP, file, IDCliente, IDSessione);

                        uce_b.SetValue(Grid.RowProperty, 1);
						uce_b.SetValue(Grid.ColumnProperty, 1);

						if (!objList.Contains(ID))
						{
							ArrayList al = new ArrayList();
							al.Add(uce_b);
							objList.Add(ID, al);
							IDList.Add(ID, "Excel: Bilancio Patrimoniale Attivo");
						}
						else
						{
							((ArrayList)(objList[ID])).Add(uce_b);
						}

                       
                            uce_b.Visibility = System.Windows.Visibility.Collapsed;
                        
						g.Children.Add(uce_b);

                        countertabindex = uce_b.countertabindex;
                        break;

					case "Excel: Bilancio Abbreviato Patrimoniale Passivo":
                        cBusinessObjects.idcliente = int.Parse(IDCliente);
                        cBusinessObjects.idsessione = int.Parse(IDSessione);
                        file = App.AppTemplateBilancioAbbreviato_Passivo;

						uce_b = new uc_Excel_Bilancio(countertabindex);

                        uce_b.Titolo = item.Attributes["Tab"].Value;

						uce_b.ReadOnly = _ReadOnly;
						uce_b.Abbreviato = true;

                        //if (Sessioni.Contains((SessioneNow + 1)))
                        //{
                        //	_x_AP = new XmlDataProviderManager(Sessioni[(SessioneNow + 1)].ToString());
                        //}
                        DataTable datibilanciotestata5 = null;
                        DataRow nodedata5 = null;
                        datibilanciotestata5 = cBusinessObjects.GetData(int.Parse(ID), typeof(Excel_Bilancio_Testata));
                        nodedata5 = null;
                        foreach (DataRow dt in datibilanciotestata5.Rows)
                        {
                            nodedata5 = dt;
                            tipoBilancio = dt["tipoBilancio"].ToString();
                        }
                        if (nodedata5 == null)
                        {
                            nodedata5 = datibilanciotestata5.Rows.Add(int.Parse(ID), cBusinessObjects.idcliente, cBusinessObjects.idsessione);
                        }

                        if (tipoBilancio == "")
                        {
                                    wSceltaTipologiaBilancio stp = new wSceltaTipologiaBilancio();
                                    //stp.Owner = this.Owner;
                                    stp.setFather(ID);
                                    stp.ShowDialog();
                                    if (stp.typechosen != "")
                                    {
                                        tipoBilancio = stp.typechosen;
                                        nodedata5["tipoBilancio"] = tipoBilancio;
                                        cBusinessObjects.SaveData(int.Parse(ID), datibilanciotestata5, typeof(Excel_Bilancio_Testata));

                                    }
                                    else
                                    {
                                        this.Owner.ConsentiChiusuraFinestra = true;

                                        this.Owner.Close();
                                        return;
                                    }
                         
                        }

                        switch (tipoBilancio)
                        {
                            case "Micro":
                                file = App.AppTemplateBilancioMicro_Passivo2016;
                                break;
                            case "2016":
                                file = App.AppTemplateBilancioAbbreviato_Passivo2016;
                                break;
                            default:
                                file = App.AppTemplateBilancioAbbreviato_Passivo;
                                break;
                        }

                        uce_b.LoadDataSource(ref _xBilanci, ID, _x_AP, file, IDCliente, IDSessione);

                        uce_b.SetValue(Grid.RowProperty, 1);
						uce_b.SetValue(Grid.ColumnProperty, 1);

						if (!objList.Contains(ID))
						{
							ArrayList al = new ArrayList();
							al.Add(uce_b);
							objList.Add(ID, al);
							IDList.Add(ID, "Excel: Bilancio Patrimoniale Attivo");
						}
						else
						{
							((ArrayList)(objList[ID])).Add(uce_b);
						}

                  
                            uce_b.Visibility = System.Windows.Visibility.Collapsed;
               
						g.Children.Add(uce_b);

                        countertabindex = uce_b.countertabindex;
                        break;

					case "Excel: Bilancio Abbreviato Conto Economico":
                        cBusinessObjects.idcliente = int.Parse(IDCliente);
                        cBusinessObjects.idsessione = int.Parse(IDSessione);
                        file = App.AppTemplateBilancioAbbreviato_ContoEconomico;

						uce_b = new uc_Excel_Bilancio(countertabindex);

                        uce_b.Titolo = item.Attributes["Tab"].Value;

						uce_b.ReadOnly = _ReadOnly;
						uce_b.Abbreviato = true;

                        //if (Sessioni.Contains((SessioneNow + 1)))
                        //{
                        //	_x_AP = new XmlDataProviderManager(Sessioni[(SessioneNow + 1)].ToString());
                        //}

                        DataTable datibilanciotestata6 = cBusinessObjects.GetData(int.Parse(ID), typeof(Excel_Bilancio_Testata));
                        DataRow nodedata6 = null;
                        foreach (DataRow dt in datibilanciotestata6.Rows)
                        {
                            nodedata6 = dt;
                            tipoBilancio = dt["tipoBilancio"].ToString();
                        }
                        if (nodedata6 == null)
                        {
                            nodedata6 = datibilanciotestata6.Rows.Add(int.Parse(ID), cBusinessObjects.idcliente, cBusinessObjects.idsessione);
                        }

                        if (tipoBilancio == "")
                        {
                                    wSceltaTipologiaBilancio stp = new wSceltaTipologiaBilancio();
                                    //stp.Owner = this.Owner;
                                    stp.setFather(ID);
                                    stp.ShowDialog();
                                    if (stp.typechosen != "")
                                    {
                                        tipoBilancio = stp.typechosen;
                                        nodedata6["tipoBilancio"] = tipoBilancio;
                                        cBusinessObjects.SaveData(int.Parse(ID), datibilanciotestata6, typeof(Excel_Bilancio_Testata));

                                    }
                                    else
                                    {
                                        this.Owner.ConsentiChiusuraFinestra = true;

                                        this.Owner.Close();
                                        return;
                                    }
                         
                        }

                        switch (tipoBilancio)
                        {
                            case "Micro":
                                file = App.AppTemplateBilancioMicro_ContoEconomico2016;
                                break;
                            case "2016":
                                file = App.AppTemplateBilancioAbbreviato_ContoEconomico2016;
                                break;
                            default:
                                file = App.AppTemplateBilancioAbbreviato_ContoEconomico;
                                break;
                        }

                        uce_b.LoadDataSource(ref _xBilanci, ID, _x_AP, file, IDCliente, IDSessione);

                        uce_b.SetValue(Grid.RowProperty, 1);
						uce_b.SetValue(Grid.ColumnProperty, 1);

						if (!objList.Contains(ID))
						{
							ArrayList al = new ArrayList();
							al.Add(uce_b);
							objList.Add(ID, al);
							IDList.Add(ID, "Excel: Bilancio Patrimoniale Attivo");
						}
						else
						{
							((ArrayList)(objList[ID])).Add(uce_b);
						}

                       
                            uce_b.Visibility = System.Windows.Visibility.Collapsed;
                       
						g.Children.Add(uce_b);

                        countertabindex = uce_b.countertabindex;
                        break;

					case "Excel: Bilancio":
                        cBusinessObjects.idcliente = int.Parse(IDCliente);
                        cBusinessObjects.idsessione = int.Parse(IDSessione);
                        file =  App.AppTemplateBilancio_Attivo;

						uce_b = new uc_Excel_Bilancio(countertabindex);

                        uce_b.Titolo = item.Attributes["Tab"].Value;

						uce_b.ReadOnly = _ReadOnly;

                        //if (Sessioni.Contains((SessioneNow + 1)))
                        //{
                        //	_x_AP = new XmlDataProviderManager(Sessioni[(SessioneNow + 1)].ToString());
                        //}

                        DataTable datibilanciotestata7 = cBusinessObjects.GetData(int.Parse(ID), typeof(Excel_Bilancio_Testata));
                        DataRow nodedata7 = null;
                        foreach (DataRow dt in datibilanciotestata7.Rows)
                        {
                            nodedata7 = dt;
                            tipoBilancio = dt["tipoBilancio"].ToString();
                        }
                        if (nodedata7 == null)
                        {
                            nodedata7 = datibilanciotestata7.Rows.Add(int.Parse(ID), cBusinessObjects.idcliente, cBusinessObjects.idsessione);
                        }

                        if (tipoBilancio == "")
                        {
                                    wSceltaTipologiaBilancio stp = new wSceltaTipologiaBilancio();
                                    //stp.Owner = this.Owner;
                                    stp.setFather(ID);
                                    stp.ShowDialog();
                                    if (stp.typechosen != "")
                                    {
                                        tipoBilancio = stp.typechosen;
                                        nodedata7["tipoBilancio"] = tipoBilancio;
                                        cBusinessObjects.SaveData(int.Parse(ID), datibilanciotestata7, typeof(Excel_Bilancio_Testata));

                                    }
                                    else
                                    {
                                        this.Owner.ConsentiChiusuraFinestra = true;

                                        this.Owner.Close();
                                        return;
                                    }
                            
                        }

                        switch (tipoBilancio)
                        {
                            case "2016":
                                file = App.AppTemplateBilancio_Attivo2016;
                                break;
                            default:
                                file = App.AppTemplateBilancio_Attivo;
                                break;
                        }

                        uce_b.LoadDataSource(ref _xBilanci, ID, _x_AP, file, IDCliente, IDSessione);

                        uce_b.SetValue(Grid.RowProperty, 1);
						uce_b.SetValue(Grid.ColumnProperty, 1);

						if (!objList.Contains(ID))
						{
							ArrayList al = new ArrayList();
							al.Add(uce_b);
							objList.Add(ID, al);
							IDList.Add(ID, "Excel: Bilancio Patrimoniale Attivo");
						}
						else
						{
							((ArrayList)(objList[ID])).Add(uce_b);
						}

                        
                            uce_b.Visibility = System.Windows.Visibility.Collapsed;
                       
						g.Children.Add(uce_b);

                        countertabindex = uce_b.countertabindex;
                        break;

					case "Excel: Materialità SP + CE":
						ucExcel_LimiteMaterialitaSPCE uce_lm = new ucExcel_LimiteMaterialitaSPCE(IDTree);

						uce_lm.Load( ID, Sessioni[SessioneNow].ToString(), IpotesiMaterialita.Prima, IDCliente, IDSessione);

                        uce_lm.ReadOnly = _ReadOnly;

						uce_lm.SetValue(Grid.RowProperty, 1);
						uce_lm.SetValue(Grid.ColumnProperty, 1);

						if (!objList.Contains(ID))
						{
							objList.Add(ID, uce_lm);
							IDList.Add(ID, "Excel: Materialità SP + CE");
						}

                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            uce_lm.Visibility = System.Windows.Visibility.Collapsed;
                        }
						g.Children.Add(uce_lm);
						break;

					case "Excel: Materialità SP e CE":
						ucExcel_LimiteMaterialitaSPCE uce_lm2 = new ucExcel_LimiteMaterialitaSPCE(IDTree);

						uce_lm2.Load( ID, Sessioni[SessioneNow].ToString(), IpotesiMaterialita.Seconda, IDCliente, IDSessione);

                        uce_lm2.ReadOnly = _ReadOnly;

						uce_lm2.SetValue(Grid.RowProperty, 1);
						uce_lm2.SetValue(Grid.ColumnProperty, 1);

						if (!objList.Contains(ID))
						{
							objList.Add(ID, uce_lm2);
							IDList.Add(ID, "Excel: Materialità SP e CE");
						}

                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            uce_lm2.Visibility = System.Windows.Visibility.Collapsed;
                        }
						g.Children.Add(uce_lm2);
						break;

					case "Excel: Materialità Personalizzata":
						ucExcel_LimiteMaterialitaSPCE uce_lm3 = new ucExcel_LimiteMaterialitaSPCE(IDTree);

						uce_lm3.Load( ID, Sessioni[SessioneNow].ToString(), IpotesiMaterialita.Terza, IDCliente, IDSessione);

                        uce_lm3.ReadOnly = _ReadOnly;

						uce_lm3.SetValue(Grid.RowProperty, 1);
						uce_lm3.SetValue(Grid.ColumnProperty, 1);

						if (!objList.Contains(ID))
						{
							objList.Add(ID, uce_lm3);
							IDList.Add(ID, "Excel: Materialità Personalizzata");
						}

                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            uce_lm3.Visibility = System.Windows.Visibility.Collapsed;
                        }
						g.Children.Add(uce_lm3);
						break;

					case "Excel: Affidamenti Bancari":
						ucExcel_Affidamenti uce_ab = new ucExcel_Affidamenti();

                        uce_ab.ReadOnly = _ReadOnly;

						uce_ab.LoadDataSource(ID, IDCliente, IDSessione);

                        uce_ab.SetValue(Grid.RowProperty, 1);
						uce_ab.SetValue(Grid.ColumnProperty, 1);

						if (!objList.Contains(ID))
						{
							objList.Add(ID, uce_ab);
							IDList.Add(ID, "Excel: Affidamenti Bancari");
						}

                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            uce_ab.Visibility = System.Windows.Visibility.Collapsed;
                        }
						g.Children.Add(uce_ab);
						break;

                    case "Excel: Riconciliazioni Banche":
                        ucExcel_Riconciliazioni uce_rb = new ucExcel_Riconciliazioni();

                        uce_rb.ReadOnly = _ReadOnly;

                        uce_rb.LoadDataSource( item.Attributes["ID"].Value, IDCliente, IDSessione);

                        uce_rb.SetValue(Grid.RowProperty, 1);
                        uce_rb.SetValue(Grid.ColumnProperty, 1);

                        if (!objList.Contains(ID))
                        {
                            objList.Add(item.Attributes["ID"].Value, uce_rb);
                            IDList.Add(item.Attributes["ID"].Value, "Excel: Riconciliazioni Banche");
                        }

                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            uce_rb.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        g.Children.Add(uce_rb);
                        break;

                    case "Excel: Cassa Contante":
                        ucExcel_CassaContanteNew uce_cc = new ucExcel_CassaContanteNew();
                        
                        uce_cc.ReadOnly = _ReadOnly;

                        uce_cc.LoadDataSource(  item.Attributes["ID"].Value, IDCliente, IDSessione);

                        uce_cc.SetValue( Grid.RowProperty, 1 );
                        uce_cc.SetValue( Grid.ColumnProperty, 1 );

                        if ( !objList.Contains( item.Attributes["ID"].Value ) )
                        {
                            objList.Add( item.Attributes["ID"].Value, uce_cc );
                            IDList.Add( item.Attributes["ID"].Value, "Excel: Cassa Contante" );
                        }

                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            uce_cc.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        g.Children.Add( uce_cc );
                        break;

                    case "Excel: Cassa Contante Altre Valute":
                        ucExcel_CassaContanteAltreValute uce_ccav = new ucExcel_CassaContanteAltreValute();

                        uce_ccav.ReadOnly = _ReadOnly;

                        uce_ccav.LoadDataSource( item.Attributes["ID"].Value, IDCliente, IDSessione);

                        uce_ccav.SetValue( Grid.RowProperty, 1 );
                        uce_ccav.SetValue( Grid.ColumnProperty, 1 );

                        if ( !objList.Contains( item.Attributes["ID"].Value ) )
                        {
                            objList.Add( item.Attributes["ID"].Value, uce_ccav );
                            IDList.Add( item.Attributes["ID"].Value, "Excel: Cassa Contante Altre Valute" );
                        }

                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            uce_ccav.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        g.Children.Add( uce_ccav );
                        break;

                    case "Excel: ScrittureMagazzino":
                        ucExcel_ScrittureMagazzino uce_sm = new ucExcel_ScrittureMagazzino();

                        uce_sm.ReadOnly = _ReadOnly;

                        uce_sm.LoadDataSource(  item.Attributes["ID"].Value, IDCliente, IDSessione);

                        uce_sm.SetValue( Grid.RowProperty, 1 );
                        uce_sm.SetValue( Grid.ColumnProperty, 1 );

                        if ( !objList.Contains( item.Attributes["ID"].Value ) )
                        {
                            objList.Add( item.Attributes["ID"].Value, uce_sm );
                            IDList.Add( item.Attributes["ID"].Value, "Excel: ScrittureMagazzino" );
                        }

                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            uce_sm.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        g.Children.Add( uce_sm );
                        break;

                    case "Excel: PianificazioneNew":
                        ucPianificazioneNewSingolo uce_pNew = new ucPianificazioneNewSingolo();

                        afterpianificazione = true;

                        uce_pNew.ReadOnly = _ReadOnly;

                        uce_pNew.Load(ID, Sessioni[SessioneNow].ToString(), Sessioni, new Hashtable(), new Hashtable(), SessioneNow, IDTree, IDCliente,IDSessione);

                        uce_pNew.SetValue(Grid.RowProperty, 1);
                        uce_pNew.SetValue(Grid.ColumnProperty, 1);

                        if (!objList.Contains(ID))
                        {
                            objList.Add(ID, uce_pNew);
                            IDList.Add(ID, "Excel: PianificazioneNew");
                        }

                        //if ( !objList.Contains( item.Attributes["ID"].Value ) )
                        //{
                        //    objList.Add( item.Attributes["ID"].Value, uce_pNew );
                        //    IDList.Add( item.Attributes["ID"].Value, "Excel: PianificazioneNew" );
                        //}

                        //if (item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True")
                        {
                            uriSource = new Uri(left, UriKind.Relative);
                            ((Image)(g.Children[0])).Source = new BitmapImage(uriSource);
                            uce_pNew.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        g.Children.Add(uce_pNew);

                        NodiPianificazione = uce_pNew.visiblenode;
                        NodiPianificazioneID = uce_pNew.visiblenodeID;
                        break;
					//case "Excel: Riepilogo Affidamenti Bancari":

					//    ucExcel_RiepilogoAffidamenti uce_rab = new ucExcel_RiepilogoAffidamenti();
						
					//    try
					//    {
					//        uce_rab.LoadDataSource(ref _x, ID);
					//    }
					//    catch (Exception ex)
					//    {
					//        string log = ex.Message;
					//        break;
					//    }

					//    uce_rab.SetValue(Grid.RowProperty, 1);
					//    uce_rab.SetValue(Grid.ColumnProperty, 1);

					//    objList.Add(item.Attributes["ID"].Value, uce_rab);
					//    IDList.Add(item.Attributes["ID"].Value, "Excel: Riepilogo Affidamenti Bancari");
					//    g.Children.Add(uce_rab);
					//    break;

					case "Excel: Lead":

						ucLead uce_lead = new ucLead();

						uce_lead.ReadOnly = _ReadOnly;

						//if (Sessioni.Contains((SessioneNow + 1)))
						//{
						//	_x_AP = new XmlDataProviderManager(Sessioni[(SessioneNow + 1)].ToString());
						//}

						//gestione finestra aperta solo se dati presenti
						if (_x.Document.SelectNodes("/Dati/Dato[@ID=" + item.Attributes["ID"].Value + "]/Valore").Count > 0)
						{
							uce_lead.Visibility = System.Windows.Visibility.Visible;
							uriSource = new Uri(down, UriKind.Relative);
							((Image)(g.Children[0])).Source = new BitmapImage(uriSource);
						}
						else
						{
							uce_lead.Visibility = System.Windows.Visibility.Collapsed;
							uriSource = new Uri(left, UriKind.Relative);
							((Image)(g.Children[0])).Source = new BitmapImage(uriSource);
						}


						uce_lead.LoadDataSource( item.Attributes["ID"].Value,  IDCliente, IDSessione, _x_AP, _IDTree); //ref _xBilanci

						uce_lead.SetValue(Grid.RowProperty, 1);
						uce_lead.SetValue(Grid.ColumnProperty, 1);

						objList.Add(item.Attributes["ID"].Value, uce_lead);
						IDList.Add(item.Attributes["ID"].Value, "Excel: Lead");

						g.Children.Add(uce_lead);

						break;

					case "Excel":
						if (item.Attributes["ID"].Value == "200")
						{
							ucValutazioneAmbiente uce_va = new ucValutazioneAmbiente();

							uce_va.Load( item.Attributes["ID"].Value,  Sessioni, SessioneNow, IDTree,IDCliente,IDSessione);

							uce_va.SetValue(Grid.RowProperty, 1);
							uce_va.SetValue(Grid.ColumnProperty, 1);

							objList.Add(item.Attributes["ID"].Value, uce_va);
							IDList.Add(item.Attributes["ID"].Value, "Excel");
                            if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                            {
                                uce_va.Visibility = System.Windows.Visibility.Collapsed;
                            }
							g.Children.Add(uce_va);
						}
                        else if ( item.Attributes["ID"].Value == "202" )
                        {
                            ucCicli uce_c = new ucCicli();

                            uce_c.Load( ref _x, item.Attributes["ID"].Value, Sessioni[SessioneNow].ToString(), Sessioni, SessioniTitoli, SessioniID, SessioneNow, IDTree, IDCliente, IDSessione );

                            uce_c.SetValue( Grid.RowProperty, 1 );
                            uce_c.SetValue( Grid.ColumnProperty, 1 );

                            objList.Add( item.Attributes["ID"].Value, uce_c );
                            IDList.Add( item.Attributes["ID"].Value, "Excel" );
                            if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                            {
                                uce_c.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            g.Children.Add( uce_c );
                        }
                        else if ( item.Attributes["ID"].Value == "22" && IDTree != "2" )
                        {
                            ucRischioGlobale uce_rg = new ucRischioGlobale();
                            
                            uce_rg.Owner = Owner;

                            uce_rg.Load( item.Attributes["ID"].Value, Sessioni, SessioneNow, IDTree,IDCliente,IDSessione );

                            uce_rg.SetValue( Grid.RowProperty, 1 );
                            uce_rg.SetValue( Grid.ColumnProperty, 1 );

                            objList.Add( item.Attributes["ID"].Value, uce_rg );
                            IDList.Add( item.Attributes["ID"].Value, "Excel" );
                            if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                            {
                                uce_rg.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            g.Children.Add( uce_rg );
                        }
						break;
                    case "Rischio_di_individuazione_6_1":
                         ucRischioIndividuazione uce_RI = new ucRischioIndividuazione();
                        
                        uce_RI.ReadOnly = _ReadOnly;

                        uce_RI.Load(  item.Attributes["ID"].Value, IDCliente, IDSessione);

                        uce_RI.SetValue( Grid.RowProperty, 1 );
                        uce_RI.SetValue( Grid.ColumnProperty, 1 );

                        if ( !objList.Contains( item.Attributes["ID"].Value ) )
                        {
                            objList.Add( item.Attributes["ID"].Value, uce_RI );
                            IDList.Add( item.Attributes["ID"].Value, "Rischio_di_individuazione_6_1" );
                        }
                        g.Children.Add( uce_RI );
                        break;
					case "Report":
						ucAltoMedioBasso uce_amb = new ucAltoMedioBasso();

						uce_amb.ReadOnly = _ReadOnly;

                        uce_amb.Owner = Owner;

						try
						{
							uce_amb.Load(item.Attributes["ID"].Value,IDCliente,IDSessione);
						}
						catch (Exception ex)
						{
							string log = ex.Message;
							uce_amb.Load( "-1",IDCliente, IDSessione);
                        }

						uce_amb.SetValue(Grid.RowProperty, 1);
						uce_amb.SetValue(Grid.ColumnProperty, 1);

						objList.Add(item.Attributes["ID"].Value, uce_amb);
						IDList.Add(item.Attributes["ID"].Value, "Report");
                        if ( item.Attributes["Chiuso"] != null && item.Attributes["Chiuso"].Value == "True" )
                        {
                            uce_amb.Visibility = System.Windows.Visibility.Collapsed;
                        }
						g.Children.Add(uce_amb);
						break;
                    default:
                        break;
                }

             

                b.Child = g;
             
                    foreach (UIElement ctr in g.Children)
                    {
                        try
                        {
                      
                        if(ctr.GetType().Name.IndexOf("uc_Excel_Bilancio")==0)
                          ctr.Visibility = System.Windows.Visibility.Visible;
                        else
                            if((ctr.GetType().Name.IndexOf("uc")==0) ||(ctr.GetType().Name.IndexOf("wW")==0) )
                                ctr.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        catch (Exception ex)
                        {
                            string log = ex.Message;
                        }
                    }
            


                stack.Children.Add(b);
            }

            foreach ( UIElement item in stack.Children )
            {
                try
                {
                    ( (UserControl)( ( (Grid)( ( (Border)( item ) ).Child ) ).Children[2] ) ).Width = stack.Width - 30;
                }
                catch ( Exception ex )
                {
                    string log = ex.Message;
                }
            }
        }

       private void BtnCopiada1211_Click(object sender, RoutedEventArgs e)
        {
            if (_ReadOnly)
            {
                MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
                return;
            }

            wSceltaIncarico si = new wSceltaIncarico(_IDCliente);
            si.ShowDialog();

            if (si.IncaricoSelected == "-1")
            {
                return;
            }

            MasterFile mf = MasterFile.Create();

            Hashtable htIncarico = mf.GetIncarico(si.IncaricoSelected);

            if (htIncarico.Contains("FileData"))
            {
                XmlDataProviderManager _t = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + htIncarico["FileData"].ToString());

                string IDIncarico = "-1";
                switch (((Button)(sender)).Tag.ToString())
                {
                    case "223":
                        IDIncarico = "201611";
                        break;
                    case "224":
                        IDIncarico = "2016197";
                        break;
                    case "225":
                        IDIncarico = "2016198";
                        break;
                    default:
                        break;
                }

                string tipologia = string.Empty;
                XmlNode nTipologia;
                if (htIncarico.Contains("File"))
                {
                    XmlDataProviderManager treeIncarico = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + htIncarico["File"].ToString());
                    nTipologia = treeIncarico.Document.SelectSingleNode("//Node[@ID=" + IDIncarico + "]");
                    if (nTipologia != null)
                    {
                        if (nTipologia.Attributes["Tipologia"] != null)
                            tipologia = (treeIncarico.Document.SelectSingleNode("//Node[@ID=" + IDIncarico + "]")).Attributes["Tipologia"].Value;
                    }
                }

                // carica dati vecchi
                DataTable datifrom = cBusinessObjects.GetData(int.Parse(IDIncarico),typeof(Tabella),cBusinessObjects.idcliente,int.Parse(htIncarico["ID"].ToString()),3);
                
                cBusinessObjects.SaveData(int.Parse(((Button)(sender)).Tag.ToString()), datifrom,typeof(Tabella));
      

/*
                XmlNode oldnode = _x.Document.SelectSingleNode("//Dato[@ID=" + ((Button)(sender)).Tag.ToString() + "]");
                if(oldnode != null && _t != null && _t.Document != null && _t.Document.SelectSingleNode("//Dato[@ID=" + IDIncarico + "]") != null)
                {
                    oldnode.InnerXml = "";

                    foreach (XmlNode newnode in _t.Document.SelectSingleNode("//Dato[@ID=" + IDIncarico + "]").ChildNodes)
                    {
                        XmlNode importednode = _x.Document.ImportNode(newnode, true);
                        oldnode.AppendChild(importednode);
                    }
                }
                else
                {
                    return;
                }
*/
                //_x.Save();
               //    if (oldnode.Attributes["Tipologia"] != null && !string.IsNullOrEmpty(tipologia)) oldnode.Attributes["Tipologia"].Value = tipologia;
             //     StaticUtilities.MarkNodeAsModified(oldnode, App.MOD_ATTRIB);
               //  _x.isModified = true;
                // _x.Save(true);

                Load(ref _x, _ID, _tab, _xnl, _Sessioni, _SessioneNow, _IDTree, _SessioniTitoli, _SessioniID, _IDCliente, _IDSessione);                
            }
            else
            {
                return;
            }
        }

        private void StpTxtbox_LostFocus(object sender, RoutedEventArgs e)
        {
            string[] splitted = ((TextBox)sender).Name.Split('_');

            XmlNode tmp = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + splitted[1] + "']/Valore[@ID=" + splitted[2] + "]");
            tmp.Attributes["value"].Value = ((TextBox)sender).Text;
        }

        void btnRinominaDuplica_Click(object sender, RoutedEventArgs e)
        {
            if ( _ReadOnly )
            {
                MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione" );
                return;
            }

            var dialog = new wInputBox( "Inserire nuovo Nome del Ciclo" );
            dialog.ShowDialog();

            if ( dialog.ResponseText.Trim() == "" )
            {
                return;
            }

            string newtitle = dialog.ResponseText;

            foreach ( XmlNode item2 in _xnl )
            {
                if ( item2.Name != "Node" )
                {
                    continue;
                }

                if ( item2.Attributes["ID"].Value == ((Button)(sender)).Tag.ToString() )
                {
                    xdpm.Document.SelectSingleNode("//Node[@ID=" + item2.Attributes["ID"].Value + "]").Attributes["Tab"].Value = newtitle;
                    item2.Attributes["Tab"].Value = newtitle;
                    _xnl = _xnl[0].ParentNode.ChildNodes;
                    break;
                }
            }

            xdpm.Save();
            _x.Save();

            Load( ref _x, _ID, _tab, _xnl, _Sessioni, _SessioneNow, _IDTree, _SessioniTitoli, _SessioniID, _IDCliente, _IDSessione );
        }

        void btnEliminaDuplica_Click( object sender, RoutedEventArgs e )
        {
            if ( _ReadOnly )
            {
                MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione" );
                return;
            }

            if(MessageBox.Show( "Attenzione il ciclo verrà eliminato. proseguire?", "Attenzione", MessageBoxButton.YesNo ) == MessageBoxResult.No)
            {
                return;
            }

            ArrayList IDToDelete = new ArrayList();

            foreach ( XmlNode item in _xnl )
            {
                if (item.Name != "Node")
                {
                    continue;
                }

                if(item.Attributes["Duplicato"] != null)
                {
                    if (((Button)(sender)).Tag.ToString() == item.Attributes["ID"].Value)
                    {
                        IDToDelete.Add(item.Attributes["ID"].Value);
                    }
                }
            }

            foreach ( string item in IDToDelete )
            {
                foreach ( XmlNode item2 in _xnl )
                {
                    if ( item2.Name != "Node" )
                    {
                        continue;
                    }

                    if ( item2.Attributes["ID"].Value == item )
                    {
                        _xnl[0].ParentNode.RemoveChild( item2 );
                        _xnl = _xnl[0].ParentNode.ChildNodes;
                        break;
                    }
                }

                XmlNode del = _x.Document.SelectSingleNode( "//Dato[@ID=" + item + "]" );
                del.ParentNode.RemoveChild( del );
                del = xdpm.Document.SelectSingleNode( "//Node[@ID=" + item + "]" );
                del.ParentNode.RemoveChild( del );

            }

            xdpm.Save();
            _x.Save();

            Load( ref _x, _ID, _tab, _xnl, _Sessioni, _SessioneNow, _IDTree, _SessioniTitoli, _SessioniID, _IDCliente, _IDSessione );
        }

        void btnDuplica_Click( object sender, RoutedEventArgs e )
        {
            if ( _ReadOnly )
            {
                MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione" );
                return;
            }

            Save( App.TipoTreeNodeStato.DaCompletare );

            string lastid = _x.Document.SelectSingleNode( "//Dati" ).Attributes["LastID"].Value;

            string newID1 = ( Convert.ToInt32( lastid ) + 1 ).ToString();
            string newID2 = ( Convert.ToInt32( lastid ) + 2 ).ToString();

            _x.Document.SelectSingleNode( "//Dati" ).Attributes["LastID"].Value = newID2;

            var dialog = new wInputBox( "Inserire Nome del Ciclo" );
            dialog.ShowDialog();
            if (!dialog.diagres)
            {
                return;
            }
            if (dialog.ResponseText.Trim() == "")
            {
                return;
            }

            string newtitle = dialog.ResponseText;

            string oldID1 = "";
            string oldID2 = "";

            XmlNode newnode = _xnl[0].CloneNode( true );
            oldID1 = newnode.Attributes["ID"].Value;
            newnode.Attributes["ID"].Value = newID1;
            newnode.Attributes["Tab"].Value = "Descrizione del Ciclo " + newtitle;
            XmlAttribute attr = _xnl[0].OwnerDocument.CreateAttribute( "Duplicato" );
            newnode.Attributes.Append( attr );
            _xnl[0].OwnerDocument.ImportNode( newnode, true );
            _xnl[0].ParentNode.InsertAfter( newnode, _xnl[1] );

            XmlNode xdpmn = xdpm.Document.SelectSingleNode( "//Node[@ID=" + _xnl[0].Attributes["ID"].Value + "]" );
            XmlNode xdpmn1 = xdpm.Document.SelectSingleNode( "//Node[@ID=" + _xnl[1].Attributes["ID"].Value + "]" );
            newnode = xdpmn.CloneNode( true );
            newnode.Attributes["ID"].Value = newID1;
            newnode.Attributes["Tab"].Value = "Descrizione del Ciclo " + newtitle;
            attr = xdpmn.OwnerDocument.CreateAttribute( "Duplicato" );
            newnode.Attributes.Append( attr );
            xdpmn.OwnerDocument.ImportNode( newnode, true );
            xdpmn.ParentNode.InsertAfter( newnode, xdpmn1 );

            newnode = _xnl[1].CloneNode( true );
            oldID2 = newnode.Attributes["ID"].Value;
            newnode.Attributes["ID"].Value = newID2;
            newnode.Attributes["Tab"].Value = "Test sul Ciclo " + newtitle;
            attr = _xnl[1].OwnerDocument.CreateAttribute( "Duplicato" );
            newnode.Attributes.Append( attr );
            _xnl[1].OwnerDocument.ImportNode( newnode, true );
            _xnl[1].ParentNode.InsertAfter( newnode, _xnl[2] );

            XmlNode xdpmn2 = xdpm.Document.SelectSingleNode( "//Node[@ID=" + _xnl[2].Attributes["ID"].Value + "]" );
            newnode = xdpmn1.CloneNode( true );
            newnode.Attributes["ID"].Value = newID2;
            newnode.Attributes["Tab"].Value = "Test sul Ciclo " + newtitle;
            attr = xdpmn1.OwnerDocument.CreateAttribute( "Duplicato" );
            newnode.Attributes.Append( attr );
            xdpmn1.OwnerDocument.ImportNode( newnode, true );
            xdpmn1.ParentNode.InsertAfter( newnode, xdpmn2 );

            xdpm.Save();
            _xnl = _xnl[1].ParentNode.ChildNodes;

            XmlNode nodedata1 = _x.Document.SelectSingleNode( "//Dato[@ID=" + oldID1 + "]" );
            XmlNode nodedata2 = _x.Document.SelectSingleNode( "//Dato[@ID=" + oldID2 + "]" );

            XmlNode newnodedata1 = nodedata1.CloneNode( true );
            newnodedata1.Attributes["ID"].Value = newID1;
            attr = nodedata1.OwnerDocument.CreateAttribute( "Duplicato" );
            newnodedata1.Attributes.Append( attr );
            _x.Document.ImportNode( newnodedata1, true );
            nodedata1.ParentNode.InsertAfter( newnodedata1, nodedata2 );

            XmlNode newnodedata2 = nodedata2.CloneNode( true );
            newnodedata2.Attributes["ID"].Value = newID2;
            attr = nodedata2.OwnerDocument.CreateAttribute( "Duplicato" );
            newnodedata2.Attributes.Append( attr );
            _x.Document.ImportNode( newnodedata2, true );
            nodedata2.ParentNode.InsertAfter( newnodedata2, newnodedata1 );
            
            _x.Save();

            Load( ref _x, _ID, _tab, _xnl, _Sessioni, _SessioneNow, _IDTree, _SessioniTitoli, _SessioniID, _IDCliente, _IDSessione );
        }

        
		public XmlDataProviderManager Save(App.TipoTreeNodeStato StatoSalvataggio)
		{
            return SaveInterno(StatoSalvataggio, true);
        }

        public XmlDataProviderManager SaveInterno(App.TipoTreeNodeStato StatoSalvataggio, bool tbs )
        {
            if (tbs == true)
            {
                xdpm.Save();
            }

            foreach (DictionaryEntry item in objList)
            {
                XmlDataProviderManager tmp_x = null;

                switch (IDList[item.Key.ToString()].ToString())
                {
                    case "Testo":
                        if (!afterpianificazione)
                        {
                           ((ucTesto)(item.Value)).Save();
                         }
                        else
                        {
                            continue;
                        }
                        break;

                    case "Relazione: Testo proposto a Scelta Multipla":
                        ((ucTestoPropostoMultiplo)(item.Value)).Save();
                        break;

                    case "Tabella":
                        ((ucTabella)(item.Value)).Save();   
                        break;

                    case "Excel: PianificazioneNew":
                        ((ucPianificazioneNewSingolo)(item.Value)).Save();
                        break;
                        
                    case "Excel: Compensi e Risorse":
                        ((ucCompensiERisorse)(item.Value)).Save();
                        break;
                    case "CompensiERisorse_6_1":
                        ((ucCompensiERisorse_6_1)(item.Value)).Save();
                        break;
                    case "Accettazionedelrischio_6_1":
                        ((ucAccettazionedelrischio_6_1)(item.Value)).Save();
                        break;
                    case "Prospetto IVA":
                        ((ucProspettoIVA)(item.Value)).Save();
                       break;

                    case "Tabella Replicabile":
                        ((ucTabellaReplicata)(item.Value)).Save();
                        break;

                    case "CheckListCicli":
                        ((ucCheckListCicli)(item.Value)).Save();
                        break;

                    case "CheckListCicliMulti":
                         ((ucCheckListCicli)(item.Value)).Save();
                        break;

                    case "Check List":
                        ((ucCheckList)(item.Value)).Save();
                        break;

                    case "Check List +":
                       ((ucCheckListPlus)(item.Value)).Save();
                        break;
                    case "Check List + 6_1":
                       ((ucCheckListPlus_6_1)(item.Value)).Save();
                        break;
                    case "Nodo Multiplo":
                        //tmp_x = ((ucNodoMultiplo)(item.Value)).Save();
                        break;

                    case "Excel: Errori Rilevati":
                        ((uc_Excel_ErroriRilevati)(item.Value)).Save();
                        break;

                    case "Excel: Errori Rilevati New":
                        ((uc_Excel_ErroriRilevatiNew)(item.Value)).Save();
                        break;

                    case "Excel: Bilancio":
                    case "Excel: Bilancio Abbreviato Conto Economico":
                    case "Excel: Bilancio Abbreviato Patrimoniale Passivo":
                    case "Excel: Bilancio Abbreviato Patrimoniale Attivo":
                    case "Excel: Bilancio Conto Economico":
                    case "Excel: Bilancio Patrimoniale Passivo":
                    case "Excel: Bilancio Patrimoniale Attivo":
                      
                        foreach (uc_Excel_Bilancio bilancio in ((ArrayList)(item.Value)))
                        {
                           bilancio.Save();     
                        }
                    
                        break;

                    case "Excel: Materialità SP + CE":
                    case "Excel: Materialità SP e CE":
                    case "Excel: Materialità Personalizzata":
                        ((ucExcel_LimiteMaterialitaSPCE)(item.Value)).Save();
                        break;

                    case "Excel: Affidamenti Bancari":
                        ((ucExcel_Affidamenti)(item.Value)).Save();
                        break;

                    case "Excel: Riconciliazioni Banche":
                         ((ucExcel_Riconciliazioni)(item.Value)).Save();
                        break;

                    case "Excel: Cassa Contante":
                         ((ucExcel_CassaContanteNew)(item.Value)).Save();
                        break;

                    case "Excel: Cassa Contante Altre Valute":
                        ((ucExcel_CassaContanteAltreValute)(item.Value)).Save();
                        break;

                    case "Excel: Lead":
                        if (((ucLead)(item.Value)).Visibility == System.Windows.Visibility.Visible)
                        {
                             ((ucLead)(item.Value)).Save();
                        }
                        break;

                    case "Excel: ScrittureMagazzino":
                         ((ucExcel_ScrittureMagazzino)(item.Value)).Save();
                        break;

                    case "Excel":
                        if (item.Key.ToString() == "200")
                        {
                             ((ucValutazioneAmbiente)(item.Value)).Save();
                        }
                        else if (item.Key.ToString() == "22" )
                        {
                           ((ucRischioGlobale)(item.Value)).Save();
                        }
                        break;
                    case "Report":
                        ((ucAltoMedioBasso)(item.Value)).Save(StatoSalvataggio);
                      
                        break;

                    default:
                        tmp_x = null;
                        break;
                }

                if (tmp_x == null)
                {
                    continue;
                }

                XmlNode NodoDaImportare = tmp_x.Document.SelectSingleNode("/Dati//Dato[@ID=" + item.Key.ToString() + "]");

                XmlNode NodoDaSostituire = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + item.Key.ToString() + "']");
                XmlNode NodoImportato = _x.Document.ImportNode(NodoDaImportare, true);

                NodoDaSostituire.ParentNode.AppendChild(NodoImportato);
                NodoDaSostituire.ParentNode.RemoveChild(NodoDaSostituire);
            }

            if (_ID == "227" && _IDTree != "23")
            {
                SaveRiclassificato("206", "227");
                Indici("207");
            }

            if (_ID == "229" && _IDTree != "23")
            {
                SaveRiclassificato("219", "229");
                IndiciAbbreviati("220");
            }

            if (_ID == "134" && _IDTree == "3")
            {
                SaveRiclassificato("139", "134");
                Indici("140");
            }

            if (_ID == "166" && _IDTree == "2")
            {
                SaveRiclassificato("170", "166");
                Indici("171");
            }

            if (_ID == "172" && _IDTree == "2")
            {
                SaveRiclassificato("173", "172");
                Indici("174");
            }

            if (_ID == "2016174" && _IDTree == "3")
            {
                SaveRiclassificato("2016178", "2016174");
                IndiciAbbreviati("2016179");
            }

            if (_ID == "2016134" && _IDTree == "3")
            {
                SaveRiclassificato("2016139", "2016134");
                IndiciAbbreviati("2016140");
            }

            if (_ID == "2016186" && _IDTree == "3")
            {
                SaveRiclassificato("2016190", "2016186");
                IndiciAbbreviati("2016191");
            }

            _x.Save();

            return _x;
        }

        Hashtable valoreEA = new Hashtable();
		Hashtable valoreEP = new Hashtable();

		private void SaveRiclassificato(string IDR, string IDP)
		{

            return;


			ucExcel_BilancioRiclassificato uce_br = new ucExcel_BilancioRiclassificato();
			XmlDataProviderManager _x_AP = null;

			//if (_Sessioni.Contains((_SessioneNow + 1)))
			//{
			//	_x_AP = new XmlDataProviderManager(_Sessioni[(_SessioneNow + 1)].ToString());
			//}

			XmlNode xnode = _x.Document.SelectSingleNode("/Dati/Dato[@ID='" + IDR + "']");
			xnode.RemoveAll();
			XmlAttribute attr = _x.Document.CreateAttribute("ID");
			attr.Value = IDR;
			xnode.Attributes.Append(attr);

			#region Dati da bilancio

			RetrieveData(_x, _x_AP, IDP);

			#endregion

			string FileXML = "";

            string tipoBilancio = "";

            if (_x.Document.SelectSingleNode("//Dato[@ID='" + IDP + "']") != null && _x.Document.SelectSingleNode("//Dato[@ID='" + IDP + "']").Attributes["tipoBilancio"] != null)
            {
                tipoBilancio = _x.Document.SelectSingleNode("//Dato[@ID='" + IDP + "']").Attributes["tipoBilancio"].Value;
            }

            if (IDP == "227" || IDP == "134" || IDP == "2016134" || IDP == "166")
			{
                switch (tipoBilancio)
                {
                    case "2016":
                        FileXML = App.AppTemplateBilancio_Riclassificato2016;
                        break;
                    default:
                        FileXML = App.AppTemplateBilancio_Riclassificato;
                        break;
                }
            }
			else
			{
                switch (tipoBilancio)
                {
                    case "Micro":
                        FileXML = App.AppTemplateBilancioMicro_Riclassificato2016;
                        break;
                    case "2016":
                        FileXML = App.AppTemplateBilancioAbbreviato_Riclassificato2016;
                        break;
                    default:
                        FileXML = App.AppTemplateBilancioAbbreviato_Riclassificato;
                        break;
                }
			}

			XmlDataProviderManager _y = new XmlDataProviderManager(FileXML, true);

			AddTableData(_y, "ATTIVO", "TOTALE ATTIVITA'", xnode, IDR);
			AddTableData(_y, "PASSIVO", "TOTALE PASSIVITA'", xnode, IDR);

			AddTableData(_y, "CONTO ECONOMICO", "RISULTATO OPERATIVO", xnode, IDR);

			if (IDP == "227" || IDP == "134" || IDP == "2016134" || IDP == "166")
			{
				AddTableData(_y, "SINTESI", "CAPITALE INVESTITO", xnode, IDR);
			}
		}

		Hashtable SommeDaExcel = new Hashtable();
		Hashtable ValoriDaExcelEA = new Hashtable();
		Hashtable ValoriDaExcelEP = new Hashtable();

		private void IndiciAbbreviati(string IDR)
		{
			SommeDaExcel = new Hashtable();

			ValoriDaExcelEA = new Hashtable();
			ValoriDaExcelEP = new Hashtable();

			SommeDaExcel.Add("B10", "89|1059|1060|97|98|2|80");
			ValoriDaExcelEA.Add("B10", GetValoreEA("B10"));
			ValoriDaExcelEP.Add("B10", GetValoreEP("B10"));

			SommeDaExcel.Add("B13", "50|201655"); 
            ValoriDaExcelEA.Add("B13", GetValoreEA("B13"));
			ValoriDaExcelEP.Add("B13", GetValoreEP("B13"));

			SommeDaExcel.Add("B21", "16|7|23|33|60|53");
			ValoriDaExcelEA.Add("B21", GetValoreEA("B21"));
			ValoriDaExcelEP.Add("B21", GetValoreEP("B21"));

			SommeDaExcel.Add("B31", "133|175");
			ValoriDaExcelEA.Add("B31", GetValoreEA("B31"));
			ValoriDaExcelEP.Add("B31", GetValoreEP("B31"));

			SommeDaExcel.Add("B37", "123|129|134");
			ValoriDaExcelEA.Add("B37", GetValoreEA("B37"));
			ValoriDaExcelEP.Add("B37", GetValoreEP("B37"));

			SommeDaExcel.Add("B45", "108|109|110|111|112|114|100114|119|2016114|2016998|11611");
			ValoriDaExcelEA.Add("B45", GetValoreEA("B45"));
			ValoriDaExcelEP.Add("B45", GetValoreEP("B45"));

			SommeDaExcel.Add("B53", "189|2016190|190|191|192|194|195");
			ValoriDaExcelEA.Add("B53", GetValoreEA("B53"));
			ValoriDaExcelEP.Add("B53", GetValoreEP("B53"));

			SommeDaExcel.Add("B63", "189|2016190|190|191|192|194|195|198|212|199|2016208|208|209|210|211|202|203|204|205|206|2016204|200|209|213|214|215");
			ValoriDaExcelEA.Add("B63", GetValoreEA("B63"));
			ValoriDaExcelEP.Add("B63", GetValoreEP("B63"));

			SommeDaExcel.Add("B68", "222|223|224|2016224|20162241|235|236|237|234|232|231|228|229|230|227|2016237|2016231|2016229|240|241|242|239|2016242|243");
			ValoriDaExcelEA.Add("B68", GetValoreEA("B68"));
			ValoriDaExcelEP.Add("B68", GetValoreEP("B68"));

			SommeDaExcel.Add("B77", "108|109|110|111|112|114|100114|119|2016114|2016998");
			ValoriDaExcelEA.Add("B77", GetValoreEA("B77"));
			ValoriDaExcelEP.Add("B77", GetValoreEP("B77"));

			XmlNode xnode = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + IDR + "]");

			if (xnode.Attributes["txtEA_1"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_1");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEP_1"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_1");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEA_2"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_2");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEP_2"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_2");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEA_3"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_3");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEP_3"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_3");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEA_4"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_4");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEP_4"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_4");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEA_5"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_5");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEP_5"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_5");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEA_6"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_6");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEP_6"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_6");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEA_7"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_7");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEP_7"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_7");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEA_8"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_8");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEP_8"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_8");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEA_9"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_9");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEP_9"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_9");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEA_10"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_10");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEP_10"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_10");
				xnode.Attributes.Append(attr);
			}
			
			xnode.Attributes["txtEA_1"].Value = ConvertNumber(((double)(ValoriDaExcelEA["B31"]) == 0.0) ? "" : ((double)(ValoriDaExcelEA["B10"]) / (double)(ValoriDaExcelEA["B31"])).ToString());
			xnode.Attributes["txtEP_1"].Value = ConvertNumber(((double)(ValoriDaExcelEP["B31"]) == 0.0) ? "" : ((double)(ValoriDaExcelEP["B10"]) / (double)(ValoriDaExcelEP["B31"])).ToString());

			xnode.Attributes["txtEA_2"].Value = ConvertNumber(((double)(ValoriDaExcelEA["B31"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B10"]) + (double)(ValoriDaExcelEA["B13"])) / (double)(ValoriDaExcelEA["B31"])).ToString());
			xnode.Attributes["txtEP_2"].Value = ConvertNumber(((double)(ValoriDaExcelEP["B31"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B10"]) + (double)(ValoriDaExcelEP["B13"])) / (double)(ValoriDaExcelEP["B31"])).ToString());

			xnode.Attributes["txtEA_3"].Value = ConvertNumber(((double)(ValoriDaExcelEA["B45"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B31"]) + (double)(ValoriDaExcelEA["B37"])) / (double)(ValoriDaExcelEA["B45"])).ToString());
			xnode.Attributes["txtEP_3"].Value = ConvertNumber(((double)(ValoriDaExcelEP["B45"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B31"]) + (double)(ValoriDaExcelEP["B37"])) / (double)(ValoriDaExcelEP["B45"])).ToString());

			xnode.Attributes["txtEA_4"].Value = ConvertNumber((((double)(ValoriDaExcelEA["B31"]) + (double)(ValoriDaExcelEA["B37"])) == 0.0) ? "" : ((double)(ValoriDaExcelEA["B45"]) / ((double)(ValoriDaExcelEA["B31"]) + (double)(ValoriDaExcelEA["B37"]))).ToString());
			xnode.Attributes["txtEP_4"].Value = ConvertNumber((((double)(ValoriDaExcelEP["B31"]) + (double)(ValoriDaExcelEP["B37"])) == 0.0) ? "" : ((double)(ValoriDaExcelEP["B45"]) / ((double)(ValoriDaExcelEP["B31"]) + (double)(ValoriDaExcelEP["B37"]))).ToString());

			xnode.Attributes["txtEA_5"].Value = ConvertNumber(((double)(ValoriDaExcelEA["B21"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B45"])) / (double)(ValoriDaExcelEA["B21"])).ToString());
			xnode.Attributes["txtEP_5"].Value = ConvertNumber(((double)(ValoriDaExcelEP["B21"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B45"])) / (double)(ValoriDaExcelEP["B21"])).ToString());

			xnode.Attributes["txtEA_6"].Value = ConvertNumber(((double)(ValoriDaExcelEA["B21"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B37"]) + (double)(ValoriDaExcelEA["B45"])) / (double)(ValoriDaExcelEA["B21"])).ToString());
			xnode.Attributes["txtEP_6"].Value = ConvertNumber(((double)(ValoriDaExcelEP["B21"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B37"]) + (double)(ValoriDaExcelEP["B45"])) / (double)(ValoriDaExcelEP["B21"])).ToString());

			xnode.Attributes["txtEA_7"].Value = ConvertNumber((((double)(ValoriDaExcelEA["B13"]) + (double)(ValoriDaExcelEA["B10"]) - (double)(ValoriDaExcelEA["B31"])) == 0.0) ? "" : ((double)(ValoriDaExcelEA["B63"]) * 100.0 / ((double)(ValoriDaExcelEA["B13"]) + (double)(ValoriDaExcelEA["B10"]) - (double)(ValoriDaExcelEA["B31"]))).ToString()) + "%";
			xnode.Attributes["txtEP_7"].Value = ConvertNumber((((double)(ValoriDaExcelEP["B13"]) + (double)(ValoriDaExcelEP["B10"]) - (double)(ValoriDaExcelEP["B31"])) == 0.0) ? "" : ((double)(ValoriDaExcelEP["B63"]) * 100.0 / ((double)(ValoriDaExcelEP["B13"]) + (double)(ValoriDaExcelEP["B10"]) - (double)(ValoriDaExcelEP["B31"]))).ToString()) + "%";

			xnode.Attributes["txtEA_8"].Value = ConvertNumber(((double)(ValoriDaExcelEA["B77"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B45"])) * 100.0 / (double)(ValoriDaExcelEA["B77"])).ToString()) + "%";
			xnode.Attributes["txtEP_8"].Value = ConvertNumber(((double)(ValoriDaExcelEP["B77"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B45"])) * 100.0 / (double)(ValoriDaExcelEP["B77"])).ToString()) + "%";

			xnode.Attributes["txtEA_9"].Value = ConvertNumber(((double)(ValoriDaExcelEA["B53"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B63"])) * 100.0 / (double)(ValoriDaExcelEA["B53"])).ToString()) + "%";
			xnode.Attributes["txtEP_9"].Value = ConvertNumber(((double)(ValoriDaExcelEP["B53"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B63"])) * 100.0 / (double)(ValoriDaExcelEP["B53"])).ToString()) + "%";

			if (((double)(ValoriDaExcelEA["B63"])) <= 0.0)
			{
				xnode.Attributes["txtEA_10"].Value = "n.c.";
			}
			else //if (((double)(ValoriDaExcelEA["B72"])) / -((double)(ValoriDaExcelEA["B77"])) > 1.0)
			{
				xnode.Attributes["txtEA_10"].Value = ConvertNumber((((double)(ValoriDaExcelEA["B63"])) / -((double)(ValoriDaExcelEA["B68"]))).ToString());
			}

			if (((double)(ValoriDaExcelEP["B63"])) <= 0.0)
			{
				xnode.Attributes["txtEP_10"].Value = "n.c.";
			}
			else
			{
				xnode.Attributes["txtEP_10"].Value = ConvertNumber((((double)(ValoriDaExcelEP["B63"])) / -((double)(ValoriDaExcelEP["B68"]))).ToString());
			}
		}

		private void Indici(string IDR)
		{
        //MM
            return;

			SommeDaExcel = new Hashtable();

			ValoriDaExcelEA = new Hashtable();
			ValoriDaExcelEP = new Hashtable();

			SommeDaExcel.Add("B7", "59|62|65|68");
			ValoriDaExcelEA.Add("B7", GetValoreEA("B7"));
			ValoriDaExcelEP.Add("B7", GetValoreEP("B7"));

			SommeDaExcel.Add("B12", "3|4|81|82|83|84|85|86|32|35|38|41|98|99|77|71|74|59|62|65|68|90|91|92");
			ValoriDaExcelEA.Add("B12", GetValoreEA("B12"));
			ValoriDaExcelEP.Add("B12", GetValoreEP("B12"));

			SommeDaExcel.Add("B15", "51|52|53|54");
			ValoriDaExcelEA.Add("B15", GetValoreEA("B15"));
			ValoriDaExcelEP.Add("B15", GetValoreEP("B15"));

			SommeDaExcel.Add("B25", "17|18|19|20|21|8|9|10|11|12|13|14|25|26|27|28|43|44|33|36|39|42|60|63|66|69|72|75|78");
			ValoriDaExcelEA.Add("B25", GetValoreEA("B25"));
			ValoriDaExcelEP.Add("B25", GetValoreEP("B25"));

			SommeDaExcel.Add("B34", "151|157|160|163");
			ValoriDaExcelEA.Add("B34", GetValoreEA("B34"));
			ValoriDaExcelEP.Add("B34", GetValoreEP("B34"));

			SommeDaExcel.Add("B39", "142|151|157|160|163|133|136|139|145|154|169|172|176|177|166");
			ValoriDaExcelEA.Add("B39", GetValoreEA("B39"));
			ValoriDaExcelEP.Add("B39", GetValoreEP("B39"));

			SommeDaExcel.Add("B46", "124|125|126|129|134|137|140|143|146|149|152|155|158|161|164|167|170|173");
			ValoriDaExcelEA.Add("B46", GetValoreEA("B46"));
			ValoriDaExcelEP.Add("B46", GetValoreEP("B46"));

			SommeDaExcel.Add("B54", "108|109|110|111|112|113|115|116|117|118|119|120|-120|-108|108|267|268|189|190|191|192|194|195|198|212|199|208|209|210|202|203|204|205|206|200|211|213|214|215|222|223|224|227|228|229|230|231|232|234|235|236|237|243|239|240|241|242|247|248|249|251|252|253|257|258|260|261|262");
			ValoriDaExcelEA.Add("B54", GetValoreEA("B54"));
			ValoriDaExcelEP.Add("B54", GetValoreEP("B54"));

			SommeDaExcel.Add("B62", "189|190|191|192|194|195");
			ValoriDaExcelEA.Add("B62", GetValoreEA("B62"));
			ValoriDaExcelEP.Add("B62", GetValoreEP("B62"));

			SommeDaExcel.Add("B65", "198|212");
			ValoriDaExcelEA.Add("B65", GetValoreEA("B65"));
			ValoriDaExcelEP.Add("B65", GetValoreEP("B65"));

			SommeDaExcel.Add("B72", "189|190|191|192|194|195|198|212|199|208|209|210|202|203|204|205|206|200|211|213|214|215");
			ValoriDaExcelEA.Add("B72", GetValoreEA("B72"));
			ValoriDaExcelEP.Add("B72", GetValoreEP("B72"));

			SommeDaExcel.Add("B77", "222|223|224|227|228|229|230|231|232|234|235|236|237|243|239|240|241|242");
			ValoriDaExcelEA.Add("B77", GetValoreEA("B77"));
			ValoriDaExcelEP.Add("B77", GetValoreEP("B77"));

			SommeDaExcel.Add("B87", "267|268|189|190|191|192|194|195|198|212|199|208|209|210|202|203|204|205|206|200|211|213|214|215|222|223|224|227|228|229|230|231|232|234|235|236|237|243|239|240|241|242|247|248|249|251|252|253|257|258|260|261|262");
			ValoriDaExcelEA.Add("B87", GetValoreEA("B87"));
			ValoriDaExcelEP.Add("B87", GetValoreEP("B87"));

			SommeDaExcel.Add("I22", "17|18|19|20|21|8|9|10|11|12|13|14|25|26|27|28|43|44|33|36|39|42|60|63|66|69|72|75|78|59|62|65|68|71|74|77|98|99|3|4|81|82|83|84|85|86|32|35|38|41|-151|-157|-160|-163|-133|-136|-139|-145|-154|-169|-172|-166|-176|-177|51|52|53|54|55|-148");
			ValoriDaExcelEA.Add("I22", GetValoreEA("I22"));
			ValoriDaExcelEP.Add("I22", GetValoreEP("I22"));

			XmlNode xnode = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + IDR + "]");

			if (xnode.Attributes["txtEA_1"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_1");
				xnode.Attributes.Append(attr);
			}
			
			if (xnode.Attributes["txtEP_1"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_1");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEA_2"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_2");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEP_2"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_2");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEA_3"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_3");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEP_3"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_3");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEA_4"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_4");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEP_4"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_4");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEA_5"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_5");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEP_5"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_5");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEA_6"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_6");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEP_6"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_6");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEA_7"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_7");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEP_7"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_7");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEA_8"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_8");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEP_8"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_8");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEA_9"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_9");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEP_9"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_9");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEA_10"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_10");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEP_10"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_10");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEA_11"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_11");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEP_11"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_11");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEA_12"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_12");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEP_12"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_12");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEA_13"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_13");
				xnode.Attributes.Append(attr);
			}

			if (xnode.Attributes["txtEP_13"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_13");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEA_1"].Value = ConvertNumber(((double)(ValoriDaExcelEA["B39"]) == 0.0) ? "" : ((double)(ValoriDaExcelEA["B12"]) / (double)(ValoriDaExcelEA["B39"])).ToString());
			xnode.Attributes["txtEP_1"].Value = ConvertNumber(((double)(ValoriDaExcelEP["B39"]) == 0.0) ? "" : ((double)(ValoriDaExcelEP["B12"]) / (double)(ValoriDaExcelEP["B39"])).ToString());

			xnode.Attributes["txtEA_2"].Value = ConvertNumber(((double)(ValoriDaExcelEA["B39"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B12"]) + (double)(ValoriDaExcelEA["B15"])) / (double)(ValoriDaExcelEA["B39"])).ToString());
			xnode.Attributes["txtEP_2"].Value = ConvertNumber(((double)(ValoriDaExcelEP["B39"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B12"]) + (double)(ValoriDaExcelEP["B15"])) / (double)(ValoriDaExcelEP["B39"])).ToString());

			xnode.Attributes["txtEA_3"].Value = ConvertNumber(((double)(ValoriDaExcelEA["B54"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B39"]) + (double)(ValoriDaExcelEA["B46"])) / (double)(ValoriDaExcelEA["B54"])).ToString());
			xnode.Attributes["txtEP_3"].Value = ConvertNumber(((double)(ValoriDaExcelEP["B54"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B39"]) + (double)(ValoriDaExcelEP["B46"])) / (double)(ValoriDaExcelEP["B54"])).ToString());

			xnode.Attributes["txtEA_4"].Value = ConvertNumber((((double)(ValoriDaExcelEA["B39"]) + (double)(ValoriDaExcelEA["B46"])) == 0.0) ? "" : ((double)(ValoriDaExcelEA["B54"]) / ((double)(ValoriDaExcelEA["B39"]) + (double)(ValoriDaExcelEA["B46"]))).ToString());
			xnode.Attributes["txtEP_4"].Value = ConvertNumber((((double)(ValoriDaExcelEP["B39"]) + (double)(ValoriDaExcelEP["B46"])) == 0.0) ? "" : ((double)(ValoriDaExcelEP["B54"]) / ((double)(ValoriDaExcelEP["B39"]) + (double)(ValoriDaExcelEP["B46"]))).ToString());

			xnode.Attributes["txtEA_5"].Value = ConvertNumber(((double)(ValoriDaExcelEA["B25"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B54"])) / (double)(ValoriDaExcelEA["B25"])).ToString());
			xnode.Attributes["txtEP_5"].Value = ConvertNumber(((double)(ValoriDaExcelEP["B25"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B54"])) / (double)(ValoriDaExcelEP["B25"])).ToString());

			xnode.Attributes["txtEA_6"].Value = ConvertNumber(((double)(ValoriDaExcelEA["B25"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B46"]) + (double)(ValoriDaExcelEA["B54"])) / (double)(ValoriDaExcelEA["B25"])).ToString());
			xnode.Attributes["txtEP_6"].Value = ConvertNumber(((double)(ValoriDaExcelEP["B25"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B46"]) + (double)(ValoriDaExcelEP["B54"])) / (double)(ValoriDaExcelEP["B25"])).ToString());

			xnode.Attributes["txtEA_7"].Value = ConvertNumber(((double)(ValoriDaExcelEA["I22"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B72"])) / (double)(ValoriDaExcelEA["I22"]) * 100.0).ToString()) + "%";
			xnode.Attributes["txtEP_7"].Value = ConvertNumber(((double)(ValoriDaExcelEP["I22"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B72"])) / (double)(ValoriDaExcelEP["I22"]) * 100.0).ToString()) + "%";

			xnode.Attributes["txtEA_8"].Value = ConvertNumber(((double)(ValoriDaExcelEA["B54"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B87"])) / (double)(ValoriDaExcelEA["B54"]) * 100.0).ToString()) + "%";
			xnode.Attributes["txtEP_8"].Value = ConvertNumber(((double)(ValoriDaExcelEP["B54"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B87"])) / (double)(ValoriDaExcelEP["B54"]) * 100.0).ToString()) + "%";

			xnode.Attributes["txtEA_9"].Value = ConvertNumber(((double)(ValoriDaExcelEA["B62"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B72"])) / (double)(ValoriDaExcelEA["B62"]) * 100.0).ToString()) + "%";
			xnode.Attributes["txtEP_9"].Value = ConvertNumber(((double)(ValoriDaExcelEP["B62"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B72"])) / (double)(ValoriDaExcelEP["B62"]) * 100.0).ToString()) + "%";

			if (((double)(ValoriDaExcelEA["B77"])) >= 0.0)
			{
				xnode.Attributes["txtEA_10"].Value = "n.c.";
			}
			else //if (((double)(ValoriDaExcelEA["B72"])) / -((double)(ValoriDaExcelEA["B77"])) > 1.0)
			{
				xnode.Attributes["txtEA_10"].Value = ConvertNumber((((double)(ValoriDaExcelEA["B72"])) / -((double)(ValoriDaExcelEA["B77"]))).ToString());
			}

			if (((double)(ValoriDaExcelEP["B77"])) >= 0.0)
			{
				xnode.Attributes["txtEP_10"].Value = "n.c.";
			}
			else
			{
				xnode.Attributes["txtEP_10"].Value = ConvertNumber((((double)(ValoriDaExcelEP["B72"])) / -((double)(ValoriDaExcelEP["B77"]))).ToString());
			}

			xnode.Attributes["txtEA_11"].Value = ConvertInteger(((double)(ValoriDaExcelEA["B62"]) == 0.0) ? "" : Math.Abs((((double)(ValoriDaExcelEA["B7"])) * 365.0 / (double)(ValoriDaExcelEA["B62"]))).ToString());
			xnode.Attributes["txtEP_11"].Value = ConvertInteger(((double)(ValoriDaExcelEP["B62"]) == 0.0) ? "" : Math.Abs((((double)(ValoriDaExcelEP["B7"])) * 365.0 / (double)(ValoriDaExcelEP["B62"]))).ToString());

			xnode.Attributes["txtEA_12"].Value = ConvertInteger(((double)(ValoriDaExcelEA["B65"]) == 0.0) ? "" : Math.Abs((((double)(ValoriDaExcelEA["B34"])) * 365.0 / (double)(ValoriDaExcelEA["B65"]))).ToString());
			xnode.Attributes["txtEP_12"].Value = ConvertInteger(((double)(ValoriDaExcelEP["B65"]) == 0.0) ? "" : Math.Abs((((double)(ValoriDaExcelEP["B34"])) * 365.0 / (double)(ValoriDaExcelEP["B65"]))).ToString());

			xnode.Attributes["txtEA_13"].Value = ConvertInteger(((double)(ValoriDaExcelEA["B62"]) == 0.0) ? "" : Math.Abs((((double)(ValoriDaExcelEA["B15"])) * 365.0 / (double)(ValoriDaExcelEA["B62"]))).ToString());
			xnode.Attributes["txtEP_13"].Value = ConvertInteger(((double)(ValoriDaExcelEP["B62"]) == 0.0) ? "" : Math.Abs((((double)(ValoriDaExcelEP["B15"])) * 365.0 / (double)(ValoriDaExcelEP["B62"]))).ToString());
		}

		private string ConvertInteger(string valore)
		{
			double dblValore = 0.0;

			double.TryParse(valore, out dblValore);

			if (dblValore == 0.0)
			{
				return "";
			}
			else
			{
				return String.Format("{0:#,0}", dblValore);
			}
		}

		private double GetValoreEA(string Cella)
		{
			double returnvalue = 0.0;

			if (SommeDaExcel.Contains(Cella))
			{
				foreach (string ID in SommeDaExcel[Cella].ToString().Split('|'))
				{
					bool negativo = false;
					int intero;
					int.TryParse(ID, out intero);

					if (intero < 0)
					{
						negativo = true;
						intero = -intero;
					}

					double dblValore = 0.0;

					if (valoreEA.Contains(intero.ToString()))
					{
						double.TryParse(valoreEA[intero.ToString()].ToString(), out dblValore);
					}

					if (negativo)
					{
						returnvalue -= dblValore;
					}
					else
					{
						returnvalue += dblValore;
					}
				}
			}

			return returnvalue;
		}

		private double GetValoreEP(string Cella)
		{
			double returnvalue = 0.0;

			if (SommeDaExcel.Contains(Cella))
			{
				foreach (string ID in SommeDaExcel[Cella].ToString().Split('|'))
				{
					bool negativo = false;
					int intero;
					int.TryParse(ID, out intero);

					if (intero < 0)
					{
						negativo = true;
						intero = -intero;
					}

					double dblValore = 0.0;

					if (valoreEP.Contains(intero.ToString()))
					{
						double.TryParse(valoreEP[intero.ToString()].ToString(), out dblValore);
					}

					if (negativo)
					{
						returnvalue -= dblValore;
					}
					else
					{
						returnvalue += dblValore;
					}
				}
			}

			return returnvalue;
		}

		private void AddTableData(XmlDataProviderManager _y, string xpath, string titoloTotale, XmlNode xnode, string IDR)
		{
			XmlNode xnodeValore = null;

			int row = 0;

			double totEA_final = 0.0;
			double totEP_final = 0.0;

			Hashtable ht_totEA = new Hashtable();
			Hashtable ht_totEP = new Hashtable();

			Hashtable ht_tipo = new Hashtable();

			row++;

			foreach (XmlNode item in _y.Document.SelectNodes("/Dato/MacroGruppo[@name='" + xpath + "']/Bilancio"))
			{
				if (item.Attributes["name"] == null || item.Attributes["tipo"] == null)
				{
					continue;
				}

				row++;

				if (xnode != null)
				{
					string xml = "<Valore Titolo='" + xpath + "' row='" + row.ToString() + "'/>";
					XmlDocument doctmp = new XmlDocument();
					doctmp.LoadXml(xml);

					XmlNode tmpNode = doctmp.SelectSingleNode("Valore");

					XmlNode node = _x.Document.ImportNode(tmpNode, true);
					xnodeValore = xnode.AppendChild(node);
				}

				ht_tipo.Add(row, item.Attributes["tipo"].Value);

				if (xnodeValore.Attributes["tipo"] == null)
				{
					XmlAttribute attr = _x.Document.CreateAttribute("tipo");
					xnodeValore.Attributes.Append(attr);
				}

				xnodeValore.Attributes["tipo"].Value = item.Attributes["tipo"].Value;

				if (xnodeValore.Attributes["name"] == null)
				{
					XmlAttribute attr = _x.Document.CreateAttribute("name");
					xnodeValore.Attributes.Append(attr);
				}

				xnodeValore.Attributes["name"].Value = item.Attributes["name"].Value;

				if (item.Attributes["tipo"].Value == "titolo")
				{
					continue;
				}

				double totEA = 0.0;
				double totEP = 0.0;

				foreach (string ID in item.Attributes["somma"].Value.Split('|'))
				{
					string realID = ID;

					double dblValore = 0.0;

					bool negativo = false;

					if (ID.Contains('-'))
					{
						realID = ID.Replace("-", "");
						negativo = true;
					}

					if (valoreEA.Contains(realID))
					{
						double.TryParse(valoreEA[realID].ToString(), out dblValore);
					}

					//parziale_totEA += dblValore;
					//tot_totEA += dblValore;
					if (negativo)
					{
						totEA -= dblValore;
					}
					else
					{
						totEA += dblValore;
					}

					dblValore = 0.0;

					if (valoreEP.Contains(realID))
					{
						double.TryParse(valoreEP[realID].ToString(), out dblValore);
					}

					//parziale_totEP += dblValore;
					//tot_totEP += dblValore;
					if (negativo)
					{
						totEP -= dblValore;
					}
					else
					{
						totEP += dblValore;
					}
				}

				ht_totEA.Add(row, totEA);

				if (xnodeValore.Attributes["EA"] == null)
				{
					XmlAttribute attr = _x.Document.CreateAttribute("EA");
					xnodeValore.Attributes.Append(attr);
				}

				xnodeValore.Attributes["EA"].Value = ConvertNumber(totEA.ToString());

				ht_totEP.Add(row, totEP);

				if (xnodeValore.Attributes["EP"] == null)
				{
					XmlAttribute attr = _x.Document.CreateAttribute("EP");
					xnodeValore.Attributes.Append(attr);
				}

				xnodeValore.Attributes["EP"].Value = ConvertNumber(totEP.ToString());

				if (xnodeValore.Attributes["DIFF"] == null)
				{
					XmlAttribute attr = _x.Document.CreateAttribute("DIFF");
					xnodeValore.Attributes.Append(attr);
				}

				xnodeValore.Attributes["DIFF"].Value = ConvertNumber((totEA - totEP + 0.001).ToString());

				if (item.Attributes["tipo"].Value == "totale" && item.Attributes["final"] != null)
				{
					totEA_final = totEA;
					totEP_final = totEP;
				}
			}

			for (int i = 1; i <= row; i++)
			{
				if (!ht_totEA.Contains(i))
				{
					continue;
				}

				xnodeValore = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + IDR + "]/Valore[@Titolo='" + xpath + "'][@row='" + i.ToString() + "']");

				if (xnodeValore != null)
				{
					if (xnodeValore.Attributes["PERCENT_EA"] == null)
					{
						XmlAttribute attr = _x.Document.CreateAttribute("PERCENT_EA");
						xnodeValore.Attributes.Append(attr);
					}

					xnodeValore.Attributes["PERCENT_EA"].Value = ConvertPercent(((totEA_final == 0.0) ? 0.0 : (Convert.ToDouble(ht_totEA[i].ToString()) / totEA_final)).ToString());
				}

				if (xnodeValore != null)
				{
					if (xnodeValore.Attributes["PERCENT_EP"] == null)
					{
						XmlAttribute attr = _x.Document.CreateAttribute("PERCENT_EP");
						xnodeValore.Attributes.Append(attr);
					}

					xnodeValore.Attributes["PERCENT_EP"].Value = ConvertPercent(((totEP_final == 0.0) ? 0.0 : (Convert.ToDouble(ht_totEP[i].ToString()) / totEP_final)).ToString());

				}
			}
		}

		private void RetrieveData(XmlDataProviderManager _x, XmlDataProviderManager x_AP, string ID)
		{
			foreach (XmlNode node in _x.Document.SelectNodes("/Dati//Dato[@ID='" + ID + "']/Valore"))
			{
				//Calcolo valori attuali

				if (node.Attributes["EA"] != null)
				{
					if (!valoreEA.Contains(node.Attributes["ID"].Value))
					{
						valoreEA.Add(node.Attributes["ID"].Value, node.Attributes["EA"].Value);
					}
				}
				else
				{
					if (!valoreEA.Contains(node.Attributes["ID"].Value))
					{
						valoreEA.Add(node.Attributes["ID"].Value, "0");
					}
				}

				if (x_AP == null || (x_AP != null && x_AP.Document.SelectSingleNode("/Dati//Dato[@ID='" + ID + "']/Valore[@ID='" + node.Attributes["ID"].Value + "']") == null))
				{
					if (node.Attributes["EP"] != null)
					{
						if (!valoreEP.Contains(node.Attributes["ID"].Value))
						{
							valoreEP.Add(node.Attributes["ID"].Value, node.Attributes["EP"].Value);
						}
					}
					else
					{
						if (!valoreEP.Contains(node.Attributes["ID"].Value))
						{
							valoreEP.Add(node.Attributes["ID"].Value, "0");
						}
					}
				}

				//Calcolo valori anno precedente
				if (x_AP != null)
				{
					XmlNode tmpNode = x_AP.Document.SelectSingleNode("/Dati//Dato[@ID='" + ID + "']/Valore[@ID='" + node.Attributes["ID"].Value + "']");
					if (tmpNode != null)
					{
						if (tmpNode.Attributes["EA"] != null)
						{
							if (!valoreEP.Contains(node.Attributes["ID"].Value))
							{
								valoreEP.Add(node.Attributes["ID"].Value, tmpNode.Attributes["EA"].Value);
							}
						}
						else
						{
							if (!valoreEP.Contains(node.Attributes["ID"].Value))
							{
								valoreEP.Add(node.Attributes["ID"].Value, "0");
							}
						}
					}
					else
					{
						if (!valoreEP.Contains(node.Attributes["ID"].Value))
						{
							valoreEP.Add(node.Attributes["ID"].Value, "0");
						}
					}
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
				return String.Format("{0:#,0}", dblValore);
			}
		}

		private string ConvertPercent(string valore)
		{
			double dblValore = 0.0;

			double.TryParse(valore, out dblValore);

			dblValore = dblValore * 100.0;

			if (dblValore == 0.0)
			{
				return "0,00%";
			}
			else
			{
				return String.Format("{0:0.00}", dblValore) + "%";
			}
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double newsize = e.NewSize.Width - 30.0;
							
			foreach (UIElement item in stack.Children)
			{
				try
				{
					((UserControl)(((Grid)(((Border)(item)).Child)).Children[2])).Width = newsize - 30;
				}
				catch (Exception ex)
				{
					string log = ex.Message;
				}
			}

			try
			{
				stack.Width = Convert.ToDouble(newsize);
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}
		}

		//public void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		//{
		//    Resizer(Convert.ToInt32(e.NewSize.Width));
		//}

		//public void Resizer(int newsize)
		//{
		//    try
		//    {
		//        stack.Width = Convert.ToDouble(newsize);
		//        foreach (UIElement item in stack.Children)
		//        {
		//            ((UserControl)(((Grid)(((Border)(item)).Child)).Children[2])).Width = newsize - 30;
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        string log = ex.Message;
		//    }                       
		//}

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {


            if (sender is Image)
            {
                Image i = ((Image)sender);
                     try
                {
                    UserControl u = ((UserControl)(((Grid)(i.Parent)).Children[2]));

                    if (u.Visibility == System.Windows.Visibility.Collapsed)
                    {
                        u.Visibility = System.Windows.Visibility.Visible;

                        //if(u.GetType().Name == "ucLead")
                        //{
                        //    ((ucLead)u).UpdateFromBilancio();
                        //}

                        if (u.GetType().Name == "ucTesto")
                        {
                            u.Focus();
                            ((ucTesto)u).FocusNow();

                            //System.Windows.Forms.SendKeys.Send("{TAB}");
                            u.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                        }

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
                catch (Exception ex)
                {
                    string log = ex.Message;

                    try
                    {
                        StackPanel u = ((StackPanel)(((Grid)(i.Parent)).Children[2]));

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
                    catch (Exception ex2)
                    {
                        string log2 = ex2.Message;
                    }
                }
            }
            else if (sender is TextBlock)
            {
                TextBlock i = ((TextBlock)sender);
                try
                {
                    UserControl u = ((UserControl)((((Grid) (((StackPanel)(i.Parent)).Parent)).Children[2])));

                    if (u.Visibility == System.Windows.Visibility.Collapsed)
                    {
                        u.Visibility = System.Windows.Visibility.Visible;

                    
                        if (u.GetType().Name == "ucTesto")
                        {
                            u.Focus();
                            ((ucTesto)u).FocusNow();

                            //System.Windows.Forms.SendKeys.Send("{TAB}");
                            u.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                        }

                   
                    }
                    else
                    {
                        u.Visibility = System.Windows.Visibility.Collapsed;
               
                    }
                }
                catch (Exception ex)
                {
                    string log = ex.Message;
                    try
                    {
                        StackPanel u = ((StackPanel)(((Grid)(i.Parent)).Children[2]));

                        if (u.Visibility == System.Windows.Visibility.Collapsed)
                        {
                            u.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            u.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                    catch (Exception ex2)
                    {
                        string log2 = ex2.Message;
                    }
                }
            }
          }
    }
}
