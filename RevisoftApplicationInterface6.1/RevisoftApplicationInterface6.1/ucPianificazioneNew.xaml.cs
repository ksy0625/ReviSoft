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
    public partial class ucPianificazioneNew : UserControl
    {
        public int id;
        private DataTable dati = null;
        private string check = "./Images/icone/ana_stato_ok_blu.png";
		private string uncheck = "./Images/icone/check1-24x24.png";
        private string disabled = "./Images/icone/disabled.png";

		private string up = "./Images/icone/navigate_up.png"; 
		private string down = "./Images/icone/navigate_down.png";
		private string left = "./Images/icone/navigate_left.png";

		//private XmlDataProviderManager _x;
        private string _ID = "-1";
		private string IDRischioGlobale = "22";

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

        SortedDictionary<int, string> VociBilancio = new SortedDictionary<int, string>();
        
        public bool ReadOnly 
        {
            set
            {
				_ReadOnly = value;
            }
        }

        public ucPianificazioneNew()
        {
            InitializeComponent();            
        }

        private string ConvertNumber( string valore )
        {
            double dblValore = 0.0;

            double.TryParse( valore, out dblValore );

            if ( dblValore == 0.0 )
            {
                return "";
            }
            else
            {
                return String.Format( "{0:#,#}", dblValore );
            }
        }

        private string ConvertPercent( string valore )
        {
            double dblValore = 0.0;

            double.TryParse( valore, out dblValore );

            if ( dblValore == 0.0 )
            {
                return "";
            }
            else
            {
                dblValore = dblValore * 100.0;
                return String.Format( "{0:#,0.00}", dblValore );
            }
        }

        public void Load(  string ID, string FileRevisione, Hashtable _Sessioni, Hashtable _SessioniTitoli, Hashtable _SessioniID, int _SessioneNow, string _IDTree, string _IDCliente, string _IDSessione )
        {
            id = int.Parse(ID);

            cBusinessObjects.idcliente = int.Parse(_IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(_IDSessione.ToString());
            canbeexecuted = false;

            VociBilancio.Add(80, "3.4.1@Immobilizzazioni immateriali@1@1@1@1@1@1@1@0@pv");
            VociBilancio.Add(81, "3.4.2@Immobilizzazioni materiali@1@1@1@1@1@1@1@0@txt3c");
            VociBilancio.Add(82, "3.4.3@Immobilizzazioni finanziarie@1@1@1@1@1@1@1@0@pv");
            VociBilancio.Add(83, "3.4.4@Rimanenze di Magazzino@1@1@1@1@1@1@1@0@txt4c");
            VociBilancio.Add(85, "3.4.5@Rimanenze - Opere a lungo termine@1@1@1@1@1@1@1@0@txt4c");
            VociBilancio.Add(86, "3.4.6@Attività finanziarie non immobilizzate@1@1@1@1@1@1@1@0@txt5c");
            VociBilancio.Add(87, "3.4.7@Crediti commerciali (Clienti)@1@1@1@1@1@1@1@0@txt2c");
            VociBilancio.Add(88, "3.4.8@Crediti e debiti infragruppo@1@1@1@1@1@1@1@0@pv");
            VociBilancio.Add(89, "3.4.9@Crediti tributari e per imposte differite attive@1@1@1@1@1@1@1@0@pv");
            VociBilancio.Add(90, "3.4.10@Crediti verso altri@1@1@1@1@1@1@1@0@pv");
            VociBilancio.Add(91, "3.4.11@Cassa e Banche@1@1@1@1@1@1@1@0@txt5c");
            VociBilancio.Add(92, "3.4.12@Ratei e risconti (attivi e passivi)@1@1@1@1@1@1@1@0@pv");
            VociBilancio.Add(93, "3.4.13@Patrimonio netto@1@1@1@1@1@1@1@0@pv");
            VociBilancio.Add(94, "3.4.14@Fondi per rischi ed oneri@1@1@1@1@1@1@1@0@pv");
            VociBilancio.Add(95, "3.4.15@Fondo TFR (Trattamento Fine Rapporto)@1@1@1@1@1@1@1@0@txt6c");
            VociBilancio.Add(96, "3.4.16@Mutui e finanziamenti non bancari@1@1@1@1@1@1@1@0@txt5c");
            VociBilancio.Add(97, "3.4.17@Debiti commerciali (Fornitori)@1@1@1@1@1@1@1@0@txt3c");
            VociBilancio.Add(98, "3.4.18@Debiti tributari e imposte differite passive@1@1@1@1@1@1@1@0@pv");
            VociBilancio.Add(99, "3.4.19@Debiti verso altri@1@1@1@1@1@1@1@0@pv");
            //VociBilancio.Add(100, "3.4.20@Conti d'ordine@1@1@1@1@1@1@1@0@pv");
            VociBilancio.Add(101, "3.4.21@Conto economico@1@1@1@1@1@1@1@0@pv");
            //VociBilancio.Add( 102, "3.4.22@Bilancio Consolidato@0@1@1@1@1@1@0@0@pv" );

            Sessioni = _Sessioni;
            SessioniTitoli = _SessioniTitoli;
            SessioniID = _SessioniID;
            SessioneNow = _SessioneNow;
            IDTree = _IDTree;
            IDCliente = _IDCliente;
            IDSessione = _IDSessione;

		
            _ID = ID;

            MasterFile mf = MasterFile.Create();
            bilancioAssociato = mf.GetBilancioAssociatoFromRevisioneFile( Sessioni[SessioneNow].ToString() );
            bilancioTreeAssociato = mf.GetBilancioTreeAssociatoFromRevisioneFile( Sessioni[SessioneNow].ToString() );
            bilancioIDAssociato = mf.GetBilancioIDAssociatoFromRevisioneFile( Sessioni[SessioneNow].ToString() );

            brdPrima.Visibility = System.Windows.Visibility.Collapsed;
            brdSeconda.Visibility = System.Windows.Visibility.Collapsed;
            brdTerza.Visibility = System.Windows.Visibility.Collapsed;

            if ( bilancioTreeAssociato != "" )
            {
                _xBTree = new XmlDataProviderManager( bilancioTreeAssociato );
            }

            string ID_Materialità_1 = "77";
            string ID_Materialità_2 = "78";
            string ID_Materialità_3 = "199";

#pragma warning disable CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato
            bool Materialità_1 = false;
#pragma warning restore CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato
#pragma warning disable CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato
            bool Materialità_2 = false;
#pragma warning restore CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato
#pragma warning disable CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato
            bool Materialità_3 = false;
#pragma warning restore CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato

          

         
            DataTable tmpNode_true=null;

            
               
                    string statomat="";
                    DataTable statom = null;
                    DataTable datimaterialita = null;
                    statomat="";
                    statom = cBusinessObjects.GetData(int.Parse(ID_Materialità_1), typeof(StatoNodi));  // SINTETICA
                    foreach(DataRow dd in statom.Rows)
                    {
                       
                        if( dd["Stato"].ToString()=="")
                        {
                            statomat = App.TipoTreeNodeStato.DaCompletare.ToString();
                        }
                        else
                        {
                            statomat = dd["Stato"].ToString();
                        }
                    }
                    datimaterialita = cBusinessObjects.GetData(int.Parse(ID_Materialità_1), typeof(Excel_LimiteMaterialitaSPCE));
                    if (datimaterialita.Rows.Count > 0 && ((App.TipoTreeNodeStato)(Convert.ToInt32(statomat))) == App.TipoTreeNodeStato.Completato)
                    {
                        Materialità_1 = true;
                        tmpNode_true = datimaterialita;
                    }
                    if(!Materialità_1)
                    {
                        statomat="";
                        statom = cBusinessObjects.GetData(int.Parse(ID_Materialità_3), typeof(StatoNodi));  // DETTAGLIATA
                        foreach(DataRow dd in statom.Rows)
                        {
                       
                            if( dd["Stato"].ToString()=="")
                            {
                                statomat = App.TipoTreeNodeStato.DaCompletare.ToString();
                            }
                            else
                            {
                                statomat = dd["Stato"].ToString();
                            }
                        }
                        datimaterialita = cBusinessObjects.GetData(int.Parse(ID_Materialità_3), typeof(Excel_LimiteMaterialitaSPCE));
                        if (datimaterialita.Rows.Count > 0 && ((App.TipoTreeNodeStato)(Convert.ToInt32(statomat))) == App.TipoTreeNodeStato.Completato)
                        {
                            Materialità_3 = true;
                            tmpNode_true = datimaterialita;
                        }
                    }
                

            if ( tmpNode_true != null )
            {
               
              foreach(DataRow dtrow in tmpNode_true.Rows)
              {

                    if (dtrow["ID"].ToString() == "txt7")
                        txt7.Text = dtrow["value"].ToString();

                    if (dtrow["ID"].ToString() == "txt7_2sp")
                        txt7_2sp.Text = dtrow["value"].ToString();

                    if (dtrow["ID"].ToString() == "txt7_2ce")
                        txt7_2ce.Text = dtrow["value"].ToString();

                    if (dtrow["ID"].ToString() == "txt7_3sp")
                        txt7_3sp.Text = dtrow["value"].ToString();

                    if (dtrow["ID"].ToString() == "txt7_3ce")
                        txt7_3ce.Text = dtrow["value"].ToString();

                    if (dtrow["ID"].ToString() == "txt9")
                        txt9.Text = dtrow["value"].ToString();

                    if (dtrow["ID"].ToString() == "txt9_2sp")
                        txt9_2sp.Text = dtrow["value"].ToString();

                    if (dtrow["ID"].ToString() == "txt9_2ce")
                        txt9_2ce.Text = dtrow["value"].ToString();

                    if (dtrow["ID"].ToString() == "txt9_3sp")
                        txt9_3sp.Text = dtrow["value"].ToString();

                    if (dtrow["ID"].ToString() == "txt9_3ce")
                        txt9_3ce.Text = dtrow["value"].ToString();

                    if (dtrow["ID"].ToString() == "txt12")
                        txt12.Text = dtrow["value"].ToString();

                    if (dtrow["ID"].ToString() == "txt12_2sp")
                        txt12_2sp.Text = dtrow["value"].ToString();

                    if (dtrow["ID"].ToString() == "txt12_2ce")
                        txt12_2ce.Text = dtrow["value"].ToString();

                    if (dtrow["ID"].ToString() == "txt12_3sp")
                        txt12_3sp.Text = dtrow["value"].ToString();

                    if (dtrow["ID"].ToString() == "txt12_3ce")
                        txt12_3ce.Text = dtrow["value"].ToString();

              }
            }
            #region primo blocco

            DataTable  datiRishcioGlobale = cBusinessObjects.GetData(int.Parse(IDRischioGlobale), typeof(RischioGlobale));
            DataRow dtrischioglobale = null;
            foreach(DataRow dtrow in datiRishcioGlobale.Rows)
			{
                    txt1.Text = dtrow["txt1"].ToString().ToUpper(); 
                    txt3.Text = dtrow["txt3"].ToString().ToUpper();        
                    txt3c.Text = dtrow["txt3c"].ToString().ToUpper();     
                    txt4.Text = dtrow["txt4"].ToString().ToUpper();        
                    txt4c.Text = dtrow["txt4c"].ToString().ToUpper();        
                    txt6.Text = dtrow["txt6"].ToString().ToUpper();           
                    txt6c.Text = dtrow["txt6c"].ToString().ToUpper();             
                    txt5.Text = dtrow["txt5"].ToString().ToUpper();       
                    txt5c.Text = dtrow["txt5c"].ToString().ToUpper();           
                    txt2.Text = dtrow["txt2"].ToString().ToUpper();
                    txt2c.Text = dtrow["txt2c"].ToString().ToUpper();
                    dtrischioglobale = dtrow;


            }
#endregion

#region secondo blocco

            StackPanel stk = new StackPanel();
            stk.Orientation = Orientation.Horizontal;
            stk.HorizontalAlignment = HorizontalAlignment.Center;

            TextBlock txtblk = new TextBlock();
            txtblk.Text = "Legenda: VEDI SUGGERIMENTI";
            txtblk.TextAlignment = TextAlignment.Center;
            txtblk.FontWeight = FontWeights.Bold;
            txtblk.Margin = new Thickness( 10, 0, 0, 10 );
            stk.Children.Add( txtblk );

            //TextBlock txtblk = new TextBlock();
            //txtblk.Text = "Legenda sino alla versione 4.12.2: A";
            //txtblk.FontWeight = FontWeights.Bold;
            //txtblk.Margin = new Thickness( 10, 0, 0, 10 );
            //stk.Children.Add( txtblk );

            //txtblk = new TextBlock();
            //txtblk.Text = "= Esame Fisico";
            //txtblk.Margin = new Thickness( 5, 0, 0, 10 );
            //stk.Children.Add( txtblk );

            //txtblk = new TextBlock();
            //txtblk.Text = "B";
            //txtblk.FontWeight = FontWeights.Bold;
            //txtblk.Margin = new Thickness( 10, 0, 0, 10 );
            //stk.Children.Add( txtblk );

            //txtblk = new TextBlock();
            //txtblk.Text = "= Conferma";
            //txtblk.Margin = new Thickness( 5, 0, 0, 10 );
            //stk.Children.Add( txtblk );

            //txtblk = new TextBlock();
            //txtblk.Text = "C";
            //txtblk.FontWeight = FontWeights.Bold;
            //txtblk.Margin = new Thickness( 10, 0, 0, 10 );
            //stk.Children.Add( txtblk );

            //txtblk = new TextBlock();
            //txtblk.Text = "= Documentazione";
            //txtblk.Margin = new Thickness( 5, 0, 0, 10 );
            //stk.Children.Add( txtblk );

            //txtblk = new TextBlock();
            //txtblk.Text = "D";
            //txtblk.FontWeight = FontWeights.Bold;
            //txtblk.Margin = new Thickness( 10, 0, 0, 10 );
            //stk.Children.Add( txtblk );

            //txtblk = new TextBlock();
            //txtblk.Text = "= Procedura di analisi comparativa";
            //txtblk.Margin = new Thickness( 5, 0, 0, 10 );
            //stk.Children.Add( txtblk );

            //txtblk = new TextBlock();
            //txtblk.Text = "E";
            //txtblk.FontWeight = FontWeights.Bold;
            //txtblk.Margin = new Thickness( 10, 0, 0, 10 );
            //stk.Children.Add( txtblk );

            //txtblk = new TextBlock();
            //txtblk.Text = "= Indagine";
            //txtblk.Margin = new Thickness( 5, 0, 0, 10 );
            //stk.Children.Add( txtblk );

            //txtblk = new TextBlock();
            //txtblk.Text = "F";
            //txtblk.FontWeight = FontWeights.Bold;
            //txtblk.Margin = new Thickness( 10, 0, 0, 10 );
            //stk.Children.Add( txtblk );

            //txtblk = new TextBlock();
            //txtblk.Text = "= Ripetizione";
            //txtblk.Margin = new Thickness( 5, 0, 0, 10 );
            //stk.Children.Add( txtblk );

            //txtblk = new TextBlock();
            //txtblk.Text = "G";
            //txtblk.FontWeight = FontWeights.Bold;
            //txtblk.Margin = new Thickness( 10, 0, 0, 10 );
            //stk.Children.Add( txtblk );

            //txtblk = new TextBlock();
            //txtblk.Text = "= Osservazione diretta";
            //txtblk.Margin = new Thickness( 5, 0, 0, 10 );
            //stk.Children.Add( txtblk );

            brdDefinizione.Children.Add( stk );


            //stk = new StackPanel();
            //stk.Orientation = Orientation.Horizontal;

            //txtblk = new TextBlock();
            //txtblk.Text = "Legenda: A";
            //txtblk.FontWeight = FontWeights.Bold;
            //txtblk.Margin = new Thickness(10, 0, 0, 10);
            //stk.Children.Add(txtblk);

            //txtblk = new TextBlock();
            //txtblk.Text = "= Ispezione";
            //txtblk.Margin = new Thickness(5, 0, 0, 10);
            //stk.Children.Add(txtblk);

            //txtblk = new TextBlock();
            //txtblk.Text = "B";
            //txtblk.FontWeight = FontWeights.Bold;
            //txtblk.Margin = new Thickness(10, 0, 0, 10);
            //stk.Children.Add(txtblk);

            //txtblk = new TextBlock();
            //txtblk.Text = "= Osservazione";
            //txtblk.Margin = new Thickness(5, 0, 0, 10);
            //stk.Children.Add(txtblk);

            //txtblk = new TextBlock();
            //txtblk.Text = "C";
            //txtblk.FontWeight = FontWeights.Bold;
            //txtblk.Margin = new Thickness(10, 0, 0, 10);
            //stk.Children.Add(txtblk);

            //txtblk = new TextBlock();
            //txtblk.Text = "= Conferma esterna";
            //txtblk.Margin = new Thickness(5, 0, 0, 10);
            //stk.Children.Add(txtblk);

            //txtblk = new TextBlock();
            //txtblk.Text = "D";
            //txtblk.FontWeight = FontWeights.Bold;
            //txtblk.Margin = new Thickness(10, 0, 0, 10);
            //stk.Children.Add(txtblk);

            //txtblk = new TextBlock();
            //txtblk.Text = "= Ricalcolo";
            //txtblk.Margin = new Thickness(5, 0, 0, 10);
            //stk.Children.Add(txtblk);

            //txtblk = new TextBlock();
            //txtblk.Text = "E";
            //txtblk.FontWeight = FontWeights.Bold;
            //txtblk.Margin = new Thickness(10, 0, 0, 10);
            //stk.Children.Add(txtblk);

            //txtblk = new TextBlock();
            //txtblk.Text = "= Riesecuzione";
            //txtblk.Margin = new Thickness(5, 0, 0, 10);
            //stk.Children.Add(txtblk);

            //txtblk = new TextBlock();
            //txtblk.Text = "F";
            //txtblk.FontWeight = FontWeights.Bold;
            //txtblk.Margin = new Thickness(10, 0, 0, 10);
            //stk.Children.Add(txtblk);

            //txtblk = new TextBlock();
            //txtblk.Text = "= Procedure di analisi comparativa";
            //txtblk.Margin = new Thickness(5, 0, 0, 10);
            //stk.Children.Add(txtblk);

            //txtblk = new TextBlock();
            //txtblk.Text = "G";
            //txtblk.FontWeight = FontWeights.Bold;
            //txtblk.Margin = new Thickness(10, 0, 0, 10);
            //stk.Children.Add(txtblk);

            //txtblk = new TextBlock();
            //txtblk.Text = "= Indagine";
            //txtblk.Margin = new Thickness(5, 0, 0, 10);
            //stk.Children.Add(txtblk);

            //brdDefinizione.Children.Add(stk);

           
            dati = cBusinessObjects.GetData(id, typeof(PianificazioneNew));
            DataRow dt = null;

            Grid grd = new Grid();

            ColumnDefinition cd = new ColumnDefinition();
            cd.Width = new GridLength( 35, GridUnitType.Pixel );
            cd.SharedSizeGroup = "ssg0";
            grd.ColumnDefinitions.Add( cd );

			cd = new ColumnDefinition();
            cd.Width = new GridLength( 250, GridUnitType.Pixel );
			cd.SharedSizeGroup = "ssg1";
			grd.ColumnDefinitions.Add(cd);

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 50, GridUnitType.Pixel );
            grd.ColumnDefinitions.Add( cd );

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
            cd.Width = new GridLength( 30, GridUnitType.Pixel );
            grd.ColumnDefinitions.Add( cd );

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 30, GridUnitType.Pixel );
            grd.ColumnDefinitions.Add( cd );

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 30, GridUnitType.Pixel );
            grd.ColumnDefinitions.Add( cd );

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 30, GridUnitType.Pixel );
            grd.ColumnDefinitions.Add( cd );

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 0, GridUnitType.Pixel );
            grd.ColumnDefinitions.Add( cd );

			cd = new ColumnDefinition();
            cd.Width = new GridLength( 1, GridUnitType.Star );
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

            TextBlock txt = new TextBlock();
			txt.TextAlignment = TextAlignment.Center;
			txt.Text = "VOCI DI BILANCIO";
            txt.FontWeight = FontWeights.Bold;
			grd.Children.Add(txt);
			Grid.SetRow(txt, 0);
			Grid.SetColumnSpan(txt, 2);
			Grid.SetColumn(txt, 0);

            txt = new TextBlock();
            txt.TextAlignment = TextAlignment.Center;
            txt.Text = "R I";
            txt.ToolTip = "Rischio di Individuazione";
            txt.FontWeight = FontWeights.Bold;
            grd.Children.Add( txt );
            Grid.SetRow( txt, 0 );
            Grid.SetRowSpan( txt, 1 );
            Grid.SetColumn( txt, 2 );

			txt = new TextBlock();
			txt.TextAlignment = TextAlignment.Center;
            txt.Text = "EVIDENZE";
            txt.FontWeight = FontWeights.Bold;
			grd.Children.Add(txt);
			Grid.SetRow(txt, 0);
			Grid.SetColumn(txt, 3);
			Grid.SetColumnSpan(txt, 8);

            txt = new TextBlock();
            txt.TextAlignment = TextAlignment.Center;
            txt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            txt.Text = "Esecutore della revisione \r\n Personale assegnato";
            grd.Children.Add( txt );
            Grid.SetRow( txt, 0 );
            Grid.SetColumn( txt, 10 );
            Grid.SetColumnSpan( txt, 3 );
            Grid.SetRowSpan( txt, 2 );

            txt = new TextBlock();
            txt.TextAlignment = TextAlignment.Center;
            txt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            txt.Text = "Voce";
            grd.Children.Add( txt );
            Grid.SetRow( txt, 1 );
            Grid.SetColumn( txt, 0 );

            txt = new TextBlock();
            txt.TextAlignment = TextAlignment.Center;
            txt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            txt.Text = "Descrizione";
            grd.Children.Add( txt );
            Grid.SetRow( txt, 1 );
            Grid.SetColumn( txt, 1 );

            txt = new TextBlock();
            txt.TextAlignment = TextAlignment.Center;
            txt.FontFamily = new FontFamily( "Wingdings" ); 
            txt.FontSize = 16.0;
            txt.Text = "";
            txt.ToolTip = "";
            txt.Margin = new Thickness( 5, 0, 0, 0 );
            grd.Children.Add( txt );
            Grid.SetRow( txt, 1 );
            Grid.SetColumn( txt, 2 );

			txt = new TextBlock();
            txt.Text = "A";
            txt.ToolTip = "ISPEZIONE";
            txt.Margin = new Thickness( 5, 0, 0, 0 );
			grd.Children.Add(txt);
			Grid.SetRow(txt, 1);
			Grid.SetColumn(txt, 3);

            txt = new TextBlock();
            txt.Text = "B";
            txt.ToolTip = "OSSERVAZIONE";
            txt.Margin = new Thickness( 5, 0, 0, 0 );
            grd.Children.Add( txt );
            Grid.SetRow( txt, 1 );
            Grid.SetColumn( txt, 4 );

            txt = new TextBlock();
            txt.Text = "C";
            txt.ToolTip = "CONFERMA ESTERNA";
            txt.Margin = new Thickness( 5, 0, 0, 0 );
            grd.Children.Add( txt );
            Grid.SetRow( txt, 1 );
            Grid.SetColumn( txt, 5 );

            txt = new TextBlock();
            txt.Text = "D";
            txt.ToolTip = "RICALCOLO";
            txt.Margin = new Thickness( 5, 0, 0, 0 );
            grd.Children.Add( txt );
            Grid.SetRow( txt, 1 );
            Grid.SetColumn( txt, 6 );

            txt = new TextBlock();
            txt.Text = "E";
            txt.ToolTip = "RIESECUZIONE";
            txt.Margin = new Thickness( 5, 0, 0, 0 );
            grd.Children.Add( txt );
            Grid.SetRow( txt, 1 );
            Grid.SetColumn( txt, 7 );

            txt = new TextBlock();
            txt.Text = "F";
            txt.ToolTip = "PROCEDURE DI ANALISI COMPARATIVA";
            txt.Margin = new Thickness( 5, 0, 0, 0 );
            grd.Children.Add( txt );
            Grid.SetRow( txt, 1 );
            Grid.SetColumn( txt, 8 );

            txt = new TextBlock();
            txt.Text = "G";
            txt.ToolTip = "INDAGINE";
            txt.Margin = new Thickness( 5, 0, 0, 0 );
            grd.Children.Add( txt );
            Grid.SetRow( txt, 1 );
            Grid.SetColumn( txt, 9 );

            txt = new TextBlock();
            txt.Text = "H";
            txt.Margin = new Thickness( 5, 0, 0, 0 );
            grd.Children.Add( txt );
            Grid.SetRow( txt, 1 );
            Grid.SetColumn( txt, 10 );

            grd.Margin = new Thickness( 15, 0, 0, 0 );

			brdDefinizione.Children.Add(grd);

			foreach (KeyValuePair<int, string> item in VociBilancio)
			{
                dt = null;
                foreach (DataRow dtrow in dati.Rows)
                {
                        if(dtrow["ID"].ToString()== item.Key.ToString())
                           dt = dtrow;
                }
                if(dt==null)
                    {
                        dt=dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, item.Key.ToString());
                        dt["Voce"] = item.Value.ToString().Split('@')[0];
                        dt["Titolo"] = item.Value.ToString().Split('@')[1].Replace("&", "&amp;").Replace("\"", "'");
                        

                    }
				
                //if ( tmpnode != null )
                //{
                //    xnode.RemoveChild( tmpnode );
                //    tmpnode = null;
                //}

                Border brd = new Border();				
				brd.CornerRadius = new CornerRadius(5.0);
				brd.BorderThickness = new Thickness(1.0);
				brd.BorderBrush = Brushes.LightGray;
				brd.Padding = new Thickness(4.0, 4.0, 0.0, 4.0);
				brd.Margin = new Thickness(4.0);

				grd = new Grid();

                cd = new ColumnDefinition();
                cd.Width = new GridLength( 35, GridUnitType.Pixel );
                cd.SharedSizeGroup = "ssg0";
                grd.ColumnDefinitions.Add( cd );

                cd = new ColumnDefinition();
                cd.Width = new GridLength( 250, GridUnitType.Pixel );
                cd.SharedSizeGroup = "ssg1";
                grd.ColumnDefinitions.Add( cd );

                cd = new ColumnDefinition();
                cd.Width = new GridLength( 50, GridUnitType.Pixel );
                grd.ColumnDefinitions.Add( cd );

                cd = new ColumnDefinition();
                cd.Width = new GridLength( 30, GridUnitType.Pixel );
                grd.ColumnDefinitions.Add( cd );

                cd = new ColumnDefinition();
                cd.Width = new GridLength( 30, GridUnitType.Pixel );
                grd.ColumnDefinitions.Add( cd );

                cd = new ColumnDefinition();
                cd.Width = new GridLength( 30, GridUnitType.Pixel );
                grd.ColumnDefinitions.Add( cd );

                cd = new ColumnDefinition();
                cd.Width = new GridLength( 30, GridUnitType.Pixel );
                grd.ColumnDefinitions.Add( cd );

                cd = new ColumnDefinition();
                cd.Width = new GridLength( 30, GridUnitType.Pixel );
                grd.ColumnDefinitions.Add( cd );

                cd = new ColumnDefinition();
                cd.Width = new GridLength( 30, GridUnitType.Pixel );
                grd.ColumnDefinitions.Add( cd );

                cd = new ColumnDefinition();
                cd.Width = new GridLength( 30, GridUnitType.Pixel );
                grd.ColumnDefinitions.Add( cd );

                cd = new ColumnDefinition();
                cd.Width = new GridLength( 0, GridUnitType.Pixel );
                grd.ColumnDefinitions.Add( cd );

                cd = new ColumnDefinition();
                cd.Width = new GridLength( 1, GridUnitType.Star );
                cd.SharedSizeGroup = "ssg3";
                grd.ColumnDefinitions.Add( cd );

                cd = new ColumnDefinition();
                cd.Width = new GridLength( 20, GridUnitType.Pixel );
                cd.SharedSizeGroup = "ssg4";
                grd.ColumnDefinitions.Add( cd );

                cd = new ColumnDefinition();
                cd.Width = new GridLength( 50, GridUnitType.Pixel );
                cd.SharedSizeGroup = "ssg5";
                grd.ColumnDefinitions.Add( cd );

                rd = new RowDefinition();
                rd.Height = new GridLength( 20, GridUnitType.Pixel );
                grd.RowDefinitions.Add( rd );

				rd = new RowDefinition();
				grd.RowDefinitions.Add(rd);

                txt = new TextBlock();
                txt.Text = item.Value.ToString().Split( '@' )[0];
                txt.ToolTip = "Doppio Click per aprire Carta di Lavoro " + item.Value.ToString().Split( '@' )[0] + " solo se Bilancio già inserito";
                txt.MouseDown += new MouseButtonEventHandler( txt_MouseDown );
                grd.Children.Add( txt );
                Grid.SetRow( txt, 0 );
                Grid.SetColumn( txt, 0 );

				txt = new TextBlock();
                txt.Text = item.Value.ToString().Split( '@' )[1];
				grd.Children.Add(txt);
				Grid.SetRow(txt, 0);
				Grid.SetColumn(txt, 1);

                ComboBox newCombo = new ComboBox();
                newCombo.Name = "_" + item.Key.ToString() + "_ComboBoxRI_" + item.Value.ToString().Split( '@' )[10];

                this.RegisterName(newCombo.Name, newCombo);

                if (alreadydonefirstbutton == false)
                {
                    alreadydonefirstbutton = true;
                    newCombo.SelectionChanged += new SelectionChangedEventHandler(changeAll);
                }

                newCombo.SelectionChanged += new SelectionChangedEventHandler(cmbRI_Changed);

                newCombo.PreviewMouseLeftButtonDown += new MouseButtonEventHandler( obj_PreviewMouseLeftButtonDown );
                newCombo.PreviewKeyDown += new KeyEventHandler( obj_PreviewKeyDown );

               
                ComboBoxItem newitem = new ComboBoxItem();
                newitem.Content = "MA  - Molto Alto";
                newCombo.Items.Add( newitem );
                newitem = new ComboBoxItem();
                newitem.Content = "A    - Alto";
                newCombo.Items.Add( newitem );
                newitem = new ComboBoxItem();
                newitem.Content = "M    - Medio";
                newCombo.Items.Add( newitem );
                newitem = new ComboBoxItem();
                newitem.Content = "B    - Basso";
                newCombo.Items.Add( newitem );
                newitem = new ComboBoxItem();
                newitem.Content = "MB  - Molto Basso";
                newCombo.Items.Add( newitem );
                newitem = new ComboBoxItem();
                newitem.Content = "PV  - Proced Validità";
                newCombo.Items.Add( newitem );
                newitem = new ComboBoxItem();
                newitem.Content = "NA  - Non Applicabile";
                newCombo.Items.Add( newitem );
                newitem = new ComboBoxItem();
                newitem.Content = "*    - Ripristina R.I. Automatico";
                newCombo.Items.Add( newitem );

                bool donehere = false;

                //if ( _xBTree != null )
                //{
                //    XmlNode BNodeAssociato = _xBTree.Document.SelectSingleNode( "//Node[@Codice='" + item.Value.ToString().Split( '@' )[0] + "']" );

                //    if ( BNodeAssociato != null )
                //    {
                //        BNodeAssociato = BNodeAssociato.SelectSingleNode( "Sessioni/Sessione[@Alias='" + SessioniTitoli[SessioneNow].ToString() + "']" );
                //        if ( BNodeAssociato != null && BNodeAssociato.Attributes["Stato"] != null && BNodeAssociato.Attributes["Stato"].Value == "0" )
                //        {
                //            int selecteditem = 6;
                //            newCombo.SelectedItem = ( (ComboBoxItem)newCombo.Items[selecteditem] );
                //            newCombo.Text = ( (ComboBoxItem)newCombo.Items[selecteditem] ).Content.ToString();
                //            donehere = true;
                //        }
                //    }
                //}

                if ( donehere == false)
                {
                    if ( dt != null && dt["cmbRI"].ToString() != "" )
                    {
                        int selecteditem = 0;

                        switch (dt["cmbRI"].ToString() )
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
                            default:
                            case "NA":
                                selecteditem = 6;
                                break;
                        }

                        newCombo.SelectedItem = ( (ComboBoxItem)newCombo.Items[selecteditem] );
                        newCombo.Text = ( (ComboBoxItem)newCombo.Items[selecteditem] ).Content.ToString();
                    }
                    else
                    {
                        if ( item.Value.ToString().Split( '@' )[10] == "pv" )
                        {
                            newCombo.SelectedItem = ( (ComboBoxItem)newCombo.Items[5] );
                            newCombo.Text = ( (ComboBoxItem)newCombo.Items[5] ).Content.ToString();
                        }
                        else
                        {
                            int selecteditem = 0;
                            string switcrischio = "6";
                                if (dtrischioglobale != null)
                                    switcrischio = dtrischioglobale[item.Value.ToString().Split('@')[10]].ToString();

                            switch (switcrischio)
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

                            newCombo.SelectedItem = ( (ComboBoxItem)newCombo.Items[selecteditem] );
                            newCombo.Text = ( (ComboBoxItem)newCombo.Items[selecteditem] ).Content.ToString();
                        }
                    }
                }

                grd.Children.Add( newCombo );
                Grid.SetRow( newCombo, 0 );
                Grid.SetColumn( newCombo, 2 );                

                Image img = new Image();
				img.Name = "_" + item.Key.ToString() + "_EsameFisico";
				img.Height = 20.0;
				img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                if(item.Value.ToString().Split( '@' )[2] == "0")
                {
                    var uriSourceint = new Uri( disabled, UriKind.Relative );
					img.Source = new BitmapImage(uriSourceint);

                   if(dt!=null)
                      dt["EsameFisico"] = "X";
                }
                else
                {

                    img.ToolTip = "ISPEZIONE /ESAME FISICO";
                    if (dt != null && dt["EsameFisico"].ToString() != "" && dt["EsameFisico"].ToString() == "True" )
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
				Grid.SetRow(img, 0);
				Grid.SetColumn(img, 3);

                img = new Image();
                img.Name = "_" + item.Key.ToString() + "_Ispezione";
                img.Height = 20.0;
                img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                if ( item.Value.ToString().Split( '@' )[3] == "0" )
                {
                    var uriSourceint = new Uri( disabled, UriKind.Relative );
                    img.Source = new BitmapImage( uriSourceint );

                    if ( dt != null )
                      dt["Ispezione"] = "X";
                }
                else
                {

                    img.ToolTip = "OSSERVAZIONE";
                    if (dt != null && dt["Ispezione"].ToString() != "" && dt["Ispezione"].ToString() == "True")
                    {
                        var uriSourceint = new Uri( check, UriKind.Relative );
                        img.Source = new BitmapImage( uriSourceint );
                    }
                    else
                    {
                        var uriSourceint = new Uri( uncheck, UriKind.Relative );
                        img.Source = new BitmapImage( uriSourceint );
                    }

                    img.MouseLeftButtonDown += new MouseButtonEventHandler( img_MouseLeftButtonDown );
                    img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler( obj_PreviewMouseLeftButtonDown );
                    img.PreviewKeyDown += new KeyEventHandler( obj_PreviewKeyDown );
                }

                this.RegisterName( img.Name, img );

                grd.Children.Add( img );
                Grid.SetRow( img, 0 );
                Grid.SetColumn( img, 4 );

                img = new Image();
                img.Name = "_" + item.Key.ToString() + "_Indagine";
                img.Height = 20.0;
                img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                if ( item.Value.ToString().Split( '@' )[4] == "0" )
                {
                    var uriSourceint = new Uri( disabled, UriKind.Relative );
                    img.Source = new BitmapImage( uriSourceint );

                    if (dt!= null )
                     dt["Indagine"] = "X";
                }
                else
                {

                    img.ToolTip = "CONFERMA ESTERNA";
                    if (dt != null && dt["Indagine"].ToString() != "" && dt["Indagine"].ToString() == "True")
                    {
                        var uriSourceint = new Uri( check, UriKind.Relative );
                        img.Source = new BitmapImage( uriSourceint );
                    }
                    else
                    {
                        var uriSourceint = new Uri( uncheck, UriKind.Relative );
                        img.Source = new BitmapImage( uriSourceint );
                    }

                    img.MouseLeftButtonDown += new MouseButtonEventHandler( img_MouseLeftButtonDown );
                    img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler( obj_PreviewMouseLeftButtonDown );
                    img.PreviewKeyDown += new KeyEventHandler( obj_PreviewKeyDown );
                }

                this.RegisterName( img.Name, img );

                grd.Children.Add( img );
                Grid.SetRow( img, 0 );
                Grid.SetColumn( img, 5 );

                img = new Image();
                img.Name = "_" + item.Key.ToString() + "_Osservazione";
                img.Height = 20.0;
                img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                if ( item.Value.ToString().Split( '@' )[5] == "0" )
                {
                    var uriSourceint = new Uri( disabled, UriKind.Relative );
                    img.Source = new BitmapImage( uriSourceint );

                    if ( dt != null )
                      dt["Osservazione"] = "X";
                }
                else
                {
                    img.ToolTip = "RICALCOLO";
                    if (dt != null && dt["Osservazione"].ToString() != "" && dt["Osservazione"].ToString() == "True")
                    {
                        var uriSourceint = new Uri( check, UriKind.Relative );
                        img.Source = new BitmapImage( uriSourceint );
                    }
                    else
                    {
                        var uriSourceint = new Uri( uncheck, UriKind.Relative );
                        img.Source = new BitmapImage( uriSourceint );
                    }

                    img.MouseLeftButtonDown += new MouseButtonEventHandler( img_MouseLeftButtonDown );
                    img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler( obj_PreviewMouseLeftButtonDown );
                    img.PreviewKeyDown += new KeyEventHandler( obj_PreviewKeyDown );
                }

                this.RegisterName( img.Name, img );

                grd.Children.Add( img );
                Grid.SetRow( img, 0 );
                Grid.SetColumn( img, 6 );

                img = new Image();
                img.Name = "_" + item.Key.ToString() + "_Ricalcolo";
                img.Height = 20.0;
                img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                if ( item.Value.ToString().Split( '@' )[6] == "0" )
                {
                    var uriSourceint = new Uri( disabled, UriKind.Relative );
                    img.Source = new BitmapImage( uriSourceint );

                    if ( dt!= null )
                        dt["Ricalcolo"] = "X";
                }
                else
                {
                    img.ToolTip = "RIESECUZIONE";
                    if (dt != null && dt["Ricalcolo"].ToString() != "" && dt["Ricalcolo"].ToString() == "True")
                    {
                        var uriSourceint = new Uri( check, UriKind.Relative );
                        img.Source = new BitmapImage( uriSourceint );
                    }
                    else
                    {
                        var uriSourceint = new Uri( uncheck, UriKind.Relative );
                        img.Source = new BitmapImage( uriSourceint );
                    }

                    img.MouseLeftButtonDown += new MouseButtonEventHandler( img_MouseLeftButtonDown );
                    img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler( obj_PreviewMouseLeftButtonDown );
                    img.PreviewKeyDown += new KeyEventHandler( obj_PreviewKeyDown );
                }
                this.RegisterName( img.Name, img );

                grd.Children.Add( img );
                Grid.SetRow( img, 0 );
                Grid.SetColumn( img, 7 );

                img = new Image();
                img.Name = "_" + item.Key.ToString() + "_Riesecuzione";
                img.Height = 20.0;
                img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                if ( item.Value.ToString().Split( '@' )[7] == "0" )
                {
                    var uriSourceint = new Uri( disabled, UriKind.Relative );
                    img.Source = new BitmapImage( uriSourceint );

                    if (dt!= null )
                       dt["Riesecuzione"] = "X";
                }
                else
                {
                    img.ToolTip = "PROCEDURE DI ANALISI COMPARATIVA";
                    if (dt != null && dt["Riesecuzione"].ToString() != "" && dt["Riesecuzione"].ToString() == "True")
                    {
                        var uriSourceint = new Uri( check, UriKind.Relative );
                        img.Source = new BitmapImage( uriSourceint );
                    }
                    else
                    {
                        var uriSourceint = new Uri( uncheck, UriKind.Relative );
                        img.Source = new BitmapImage( uriSourceint );
                    }

                    img.MouseLeftButtonDown += new MouseButtonEventHandler( img_MouseLeftButtonDown );
                    img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler( obj_PreviewMouseLeftButtonDown );
                    img.PreviewKeyDown += new KeyEventHandler( obj_PreviewKeyDown );
                }

                this.RegisterName( img.Name, img );

                grd.Children.Add( img );
                Grid.SetRow( img, 0 );
                Grid.SetColumn( img, 8 );

                img = new Image();
                img.Name = "_" + item.Key.ToString() + "_Conferma";
                img.Height = 20.0;
                img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                if ( item.Value.ToString().Split( '@' )[8] == "0" )
                {
                    var uriSourceint = new Uri( disabled, UriKind.Relative );
                    img.Source = new BitmapImage( uriSourceint );

                    if (dt!= null )
                       dt["Conferma"] = "X";
                }
                else
                {
                    img.ToolTip = "INDAGINE";
                    if (dt != null && dt["Conferma"].ToString() != "" && dt["Conferma"].ToString() == "True")
                     {
                        var uriSourceint = new Uri( check, UriKind.Relative );
                        img.Source = new BitmapImage( uriSourceint );
                    }
                    else
                    {
                        var uriSourceint = new Uri( uncheck, UriKind.Relative );
                        img.Source = new BitmapImage( uriSourceint );
                    }

                    img.MouseLeftButtonDown += new MouseButtonEventHandler( img_MouseLeftButtonDown );
                    img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler( obj_PreviewMouseLeftButtonDown );
                    img.PreviewKeyDown += new KeyEventHandler( obj_PreviewKeyDown );
                }

                this.RegisterName( img.Name, img );

                grd.Children.Add( img );
                Grid.SetRow( img, 0 );
                Grid.SetColumn( img, 9 );

                img = new Image();
                img.Name = "_" + item.Key.ToString() + "_Comparazioni";
                img.Height = 20.0;
                img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                if ( item.Value.ToString().Split( '@' ).Length > 9 && item.Value.ToString().Split( '@' )[9] == "0" )
                {
                    var uriSourceint = new Uri( disabled, UriKind.Relative );
                    img.Source = new BitmapImage( uriSourceint );

                    if ( dt!= null )
                            dt["Comparazioni"] = "X";
                }
                else
                {
                   if (dt != null && dt["Comparazioni"].ToString() != "" && dt["Comparazioni"].ToString() == "True")
                   {
                        var uriSourceint = new Uri( check, UriKind.Relative );
                        img.Source = new BitmapImage( uriSourceint );
                    }
                    else
                    {
                        var uriSourceint = new Uri( uncheck, UriKind.Relative );
                        img.Source = new BitmapImage( uriSourceint );
                    }

                    img.MouseLeftButtonDown += new MouseButtonEventHandler( img_MouseLeftButtonDown );
                    img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler( obj_PreviewMouseLeftButtonDown );
                    img.PreviewKeyDown += new KeyEventHandler( obj_PreviewKeyDown );
                }

                this.RegisterName( img.Name, img );

                grd.Children.Add( img );
                Grid.SetRow( img, 0 );
                Grid.SetColumn( img, 10 );
                
				TextBox tb = new TextBox();
				tb.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
				tb.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
				tb.LostFocus += new RoutedEventHandler(tbEsecutore_LostFocus);
				tb.TextWrapping = TextWrapping.Wrap;
				tb.AcceptsReturn = true;
				tb.Name = "_" + item.Key.ToString() + "_Esecutore";
				if(dt!=null&& dt["Esecutore"].ToString() != "")
				{
					tb.Text = dt["Esecutore"].ToString();
				}

				this.RegisterName(tb.Name, tb);

				grd.Children.Add(tb);
				Grid.SetRow(tb, 0);
				Grid.SetColumn(tb, 11);

				img = new Image();
				img.Name = "_" + item.Key.ToString() + "_NotaImg";
				img.Margin = new Thickness(0.0);
				img.ToolTip = "Nota";
				img.Height = 10.0;
				img.Width = 10.0;
				img.Margin = new Thickness(10, 0, 0, 0);
				img.MouseLeftButtonDown += new MouseButtonEventHandler(ImageNota_MouseLeftButtonDown);
				img.VerticalAlignment = System.Windows.VerticalAlignment.Center;
               if (dt != null &&  dt["Nota"].ToString() == "")
     			{
					var uriSourceint = new Uri(up, UriKind.Relative);
					img.Source = new BitmapImage(uriSourceint);
				}
				else
				{
					var uriSourceint = new Uri(down, UriKind.Relative);
					img.Source = new BitmapImage(uriSourceint);
				}

				this.RegisterName(img.Name, img);

				grd.Children.Add(img);
				Grid.SetRow(img, 0);
				Grid.SetColumn(img, 12);

				Label lbl = new Label();
				lbl.Margin = new Thickness(0.0);
                lbl.Padding = new Thickness( 0.0 );
				var bc = new BrushConverter();
				lbl.Foreground = (Brush)bc.ConvertFrom("#F5A41C");
				lbl.Content = "Nota";
				grd.Children.Add(lbl);
				Grid.SetRow(lbl, 0);
				Grid.SetColumn(lbl, 13);

				tb = new TextBox();
				tb.Name = "_" + item.Key.ToString() + "_Nota";
                if (dt != null && dt["Nota"].ToString() != "")
  				{
					tb.Text = dt["Nota"].ToString();
				}
				else
				{
					tb.Text = "";
				}

				if (tb.Text != "")
				{
					tb.Visibility = System.Windows.Visibility.Visible;
				}
				else
				{
					tb.Visibility = System.Windows.Visibility.Collapsed;
				}

				tb.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
				tb.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
                tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
				tb.LostFocus += new RoutedEventHandler(tbNota_LostFocus);
				tb.TextWrapping = TextWrapping.Wrap;
				tb.AcceptsReturn = true;
				tb.Margin = new Thickness(0.0, 10.0, 10.0, 5.0);
				tb.Foreground = Brushes.Blue;

				this.RegisterName(tb.Name, tb);

				grd.Children.Add(tb);
				Grid.SetRow(tb, 1);
				Grid.SetColumn(tb, 1);
				Grid.SetColumnSpan(tb, 11);
				
				brd.Child = grd;

				brdDefinizione.Children.Add(brd);
			}

            grd = new Grid();

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 35, GridUnitType.Pixel );
            cd.SharedSizeGroup = "ssg0";
            grd.ColumnDefinitions.Add( cd );

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 250, GridUnitType.Pixel );
            cd.SharedSizeGroup = "ssg1";
            grd.ColumnDefinitions.Add( cd );

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 50, GridUnitType.Pixel );
            grd.ColumnDefinitions.Add( cd );

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 30, GridUnitType.Pixel );
            grd.ColumnDefinitions.Add( cd );

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 30, GridUnitType.Pixel );
            grd.ColumnDefinitions.Add( cd );

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 30, GridUnitType.Pixel );
            grd.ColumnDefinitions.Add( cd );

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 30, GridUnitType.Pixel );
            grd.ColumnDefinitions.Add( cd );

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 30, GridUnitType.Pixel );
            grd.ColumnDefinitions.Add( cd );

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 30, GridUnitType.Pixel );
            grd.ColumnDefinitions.Add( cd );

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 30, GridUnitType.Pixel );
            grd.ColumnDefinitions.Add( cd );

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 0, GridUnitType.Pixel );
            grd.ColumnDefinitions.Add( cd );

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 1, GridUnitType.Star );
            cd.SharedSizeGroup = "ssg3";
            grd.ColumnDefinitions.Add( cd );

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 20, GridUnitType.Pixel );
            cd.SharedSizeGroup = "ssg4";
            grd.ColumnDefinitions.Add( cd );

            cd = new ColumnDefinition();
            cd.Width = new GridLength( 50, GridUnitType.Pixel );
            cd.SharedSizeGroup = "ssg5";
            grd.ColumnDefinitions.Add( cd );

            rd = new RowDefinition();
            rd.Height = new GridLength( 20, GridUnitType.Pixel );
            grd.RowDefinitions.Add( rd );

            txt = new TextBlock();
            txt.TextAlignment = TextAlignment.Center;
            txt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            txt.Text = "Voce";
            grd.Children.Add( txt );
            Grid.SetRow( txt, 0 );
            Grid.SetColumn( txt, 0 );

            txt = new TextBlock();
            txt.TextAlignment = TextAlignment.Center;
            txt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            txt.Text = "Descrizione";
            grd.Children.Add( txt );
            Grid.SetRow( txt, 0 );
            Grid.SetColumn( txt, 1 );

            txt = new TextBlock();
            txt.TextAlignment = TextAlignment.Center;
            txt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            txt.Text = "R I";
            grd.Children.Add( txt );
            Grid.SetRow( txt, 0 );
            Grid.SetColumn( txt, 2 );

            txt = new TextBlock();
            txt.Text = "A";
            txt.ToolTip = "ISPEZIONE";
            txt.Margin = new Thickness( 5, 0, 0, 0 );
            grd.Children.Add( txt );
            Grid.SetRow( txt, 0 );
            Grid.SetColumn( txt, 3 );

            txt = new TextBlock();
            txt.Text = "B";
            txt.ToolTip = "OSSERVAZIONE";
            txt.Margin = new Thickness( 5, 0, 0, 0 );
            grd.Children.Add( txt );
            Grid.SetRow( txt, 0 );
            Grid.SetColumn( txt, 4 );

            txt = new TextBlock();
            txt.Text = "C";
            txt.ToolTip = "CONFERMA ESTERNA";
            txt.Margin = new Thickness( 5, 0, 0, 0 );
            grd.Children.Add( txt );
            Grid.SetRow( txt, 0 );
            Grid.SetColumn( txt, 5 );

            txt = new TextBlock();
            txt.Text = "D";
            txt.ToolTip = "RICALCOLO";
            txt.Margin = new Thickness( 5, 0, 0, 0 );
            grd.Children.Add( txt );
            Grid.SetRow( txt, 0 );
            Grid.SetColumn( txt, 6 );

            txt = new TextBlock();
            txt.Text = "E";
            txt.ToolTip = "RIESECUZIONE";
            txt.Margin = new Thickness( 5, 0, 0, 0 );
            grd.Children.Add( txt );
            Grid.SetRow( txt, 0 );
            Grid.SetColumn( txt, 7 );

            txt = new TextBlock();
            txt.Text = "F";
            txt.ToolTip = "PROCEDURE DI ANALISI COMPARATIVA";
            txt.Margin = new Thickness( 5, 0, 0, 0 );
            grd.Children.Add( txt );
            Grid.SetRow( txt, 0 );
            Grid.SetColumn( txt, 8 );

            txt = new TextBlock();
            txt.Text = "G";
            txt.ToolTip = "INDAGINE";
            txt.Margin = new Thickness( 5, 0, 0, 0 );
            grd.Children.Add( txt );
            Grid.SetRow( txt, 0 );
            Grid.SetColumn( txt, 9 );

            txt = new TextBlock();
            txt.Text = "H";
            grd.Children.Add( txt );
            Grid.SetRow( txt, 0 );
            Grid.SetColumn( txt, 10 );

            grd.Margin = new Thickness( 15, 0, 0, 0 );

            brdDefinizione.Children.Add( grd );        
#endregion	

			if (dt != null)
			{
				txtConsiderazioni.Text = dt["Testo"].ToString();
			}

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

                foreach (KeyValuePair<int, string> item in VociBilancio)
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

        private void RischioIntrinseco_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            if ( e.ClickCount == 2 )
            {
                txt_MouseDownCicli( sender, e, "2.8.7" );
            }
        }

        private void CicloVendite_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            if ( e.ClickCount == 2 )
            {
                txt_MouseDownCicli( sender, e, "2.9.1" );
            }
        }

        private void CicloAcquisti_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            if ( e.ClickCount == 2 )
            {
                txt_MouseDownCicli( sender, e, "2.9.2" );
            }
        }

        private void CicloMagazzino_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            if ( e.ClickCount == 2 )
            {
                txt_MouseDownCicli( sender, e, "2.9.3" );
            }
        }

        private void CicloTesoreria_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            if ( e.ClickCount == 2 )
            {
                txt_MouseDownCicli( sender, e, "2.9.4" );
            }
        }

        private void CicloPersonale_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            if ( e.ClickCount == 2 )
            {
                txt_MouseDownCicli( sender, e, "2.9.5" );
            }
        }

		void tbEsecutore_LostFocus(object sender, RoutedEventArgs e)
		{
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}

			string name = ((TextBox)sender).Name.Split('_')[1];

            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["ID"].ToString() != name)
                    continue;

                dtrow["Esecutore"] = ((TextBox)sender).Text;
            }
			
		}

		void tbNota_LostFocus(object sender, RoutedEventArgs e)
		{
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}

			string name = ((TextBox)sender).Name.Split('_')[1];
            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["ID"].ToString() != name)
                    continue;
                dtrow["Nota"] = ((TextBox)sender).Text;
            }

         
		}

		public int Save()
		{
			
			return cBusinessObjects.SaveData(id,dati, typeof(PianificazioneNew));
        }

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["Testo"] = ((TextBox)sender).Text;
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

		private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Image i = ((Image)sender);

			TextBlock t = ((TextBlock)(((Grid)(i.Parent)).Children[1]));

			UIElement u =  ((Grid)(i.Parent)).Children[2];

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
			string tipo = ((Image)sender).Name.Split('_').Last();
			string name = ((Image)sender).Name.Split('_')[1];

            var uriSource = new Uri( uncheck, UriKind.Relative );
            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["ID"].ToString() !=name)
                    continue;
               if (dtrow[tipo].ToString() == "True")
                {
                    dtrow[tipo] = "";
                    uriSource = new Uri(uncheck, UriKind.Relative);
                    ((Image)sender).Source = new BitmapImage(uriSource);
                }
                else
                {
                    dtrow[tipo] = "True";
                    uriSource = new Uri(check, UriKind.Relative);
                    ((Image)sender).Source = new BitmapImage(uriSource);
                }
            }
            if(dati.Rows.Count==0)
            {
                uriSource = new Uri( uncheck, UriKind.Relative );
                ((Image)sender).Source = new BitmapImage( uriSource );
            }
		}

        private void UserControl_SizeChanged( object sender, SizeChangedEventArgs e )
        {
            Resizer( Convert.ToInt32( e.NewSize.Width ) );
        }

        public void Resizer( int newsize )
        {
            double actualwidth = ((Grid)(txtDescrizioneIntensita.Parent)).ActualWidth;

            for ( int i = 3; i < brdDefinizione.Children.Count - 1; i++ )
            {
                Grid grid = ((Grid)(((Border)(brdDefinizione.Children[i])).Child));
                ((TextBox)(grid.Children[11])).Width = (actualwidth - 710 > 100)? actualwidth - 710: 100;
                ((TextBox)(grid.Children[14])).Width = (actualwidth - 270 > 200)? actualwidth - 270: 200;
            }

            txtConsiderazioni.Width = actualwidth - 100;

        }

        void txt_MouseDownCicli( object sender, MouseButtonEventArgs e, string Codice )
        {
            MasterFile mf = MasterFile.Create();

            Hashtable revisioneNow = mf.GetRevisioneFromFileData( Sessioni[SessioneNow].ToString() );
            string revisioneAssociata = App.AppDataDataFolder + "\\" + revisioneNow["FileData"].ToString();
            string revisioneTreeAssociata = App.AppDataDataFolder + "\\" + revisioneNow["File"].ToString();
            string revisioneIDAssociata = revisioneNow["ID"].ToString();
            string IDCliente = revisioneNow["Cliente"].ToString();

            if ( revisioneAssociata == "" )
            {
                e.Handled = true;
                return;
            }

            XmlDataProviderManager _xNew = new XmlDataProviderManager( revisioneAssociata );

            WindowWorkArea wa = new WindowWorkArea( ref _xNew );

            //Nodi
            wa.NodeHome = 0;

            RevisoftApplication.XmlManager xt = new XmlManager();
            xt.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
            XmlDataProvider TreeXmlProvider = new XmlDataProvider();
            TreeXmlProvider.Document = xt.LoadEncodedFile( revisioneTreeAssociata );

            if ( TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode( "/Tree" ) != null )
            {
                foreach ( XmlNode item in TreeXmlProvider.Document.SelectNodes( "/Tree//Node" ) )
                {
                    if ( item.Attributes["Codice"].Value == Codice )
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
            wa.Sessioni.Add( 0, revisioneAssociata );

            wa.SessioniTitoli.Clear();
            wa.SessioniTitoli.Add( 0, "" );

            wa.SessioniID.Clear();
            wa.SessioniID.Add( 0, revisioneIDAssociata );

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

        void txt_MouseDownCicli( object sender, MouseButtonEventArgs e )
        {
            if ( e.ClickCount == 2 )
            {
                MasterFile mf = MasterFile.Create();

                Hashtable revisioneNow = mf.GetRevisione( SessioniID[SessioneNow].ToString() );
                string revisioneAssociata = App.AppDataDataFolder + "\\" + revisioneNow["FileData"].ToString();
                string revisioneTreeAssociata = App.AppDataDataFolder + "\\" + revisioneNow["File"].ToString();
                string revisioneIDAssociata = SessioniID[SessioneNow].ToString();

                if ( revisioneAssociata == "" )
                {
                    e.Handled = true;
                    return;
                }

                XmlDataProviderManager _xNew = new XmlDataProviderManager( revisioneAssociata );

                WindowWorkArea wa = new WindowWorkArea( ref _xNew );

                //Nodi
                wa.NodeHome = 0;

                RevisoftApplication.XmlManager xt = new XmlManager();
                xt.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
                XmlDataProvider TreeXmlProvider = new XmlDataProvider();
                TreeXmlProvider.Document = xt.LoadEncodedFile( revisioneTreeAssociata );

                if ( TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode( "/Tree" ) != null )
                {
                    foreach ( XmlNode item in TreeXmlProvider.Document.SelectNodes( "/Tree//Node" ) )
                    {
                        if ( item.Attributes["Codice"].Value == ((TextBlock)(sender)).ToolTip.ToString().Replace("Fare Doppio CLick per aprire la Carta di lavoro ", "") )
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
                wa.Sessioni.Add( 0, revisioneAssociata );

                wa.SessioniTitoli.Clear();
                wa.SessioniTitoli.Add( 0, SessioniTitoli[SessioneNow].ToString() );

                wa.SessioniID.Clear();
                wa.SessioniID.Add( 0, revisioneIDAssociata );

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

        void cmbRI_Changed( object sender, SelectionChangedEventArgs e )
        {
            if ( e.AddedItems[0].ToString().Contains( '*' ) == true )
            {
               
                DataTable datiRishcioGlobale = cBusinessObjects.GetData(int.Parse(IDRischioGlobale), typeof(RischioGlobale));
                foreach (DataRow dtrow in datiRishcioGlobale.Rows)
                {

                    if ( ( (ComboBox)sender ).Name.Split( '_' )[3] == "pv" )
                    {
                        ( (ComboBox)sender ).SelectedItem = ( (ComboBoxItem)( (ComboBox)sender ).Items[5] );
                        ( (ComboBox)sender ).Text = ( (ComboBoxItem)( (ComboBox)sender ).Items[5] ).Content.ToString();
                    }
                    else
                    {
                        int selecteditem = 0;

                        switch (dtrow[( (ComboBox)sender ).Name.Split( '_' )[3]].ToString() )
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

                        ( (ComboBox)sender ).SelectedItem = ( (ComboBoxItem)( (ComboBox)sender ).Items[selecteditem] );
                        ( (ComboBox)sender ).Text = ( (ComboBoxItem)( (ComboBox)sender ).Items[selecteditem] ).Content.ToString();
                    }
                }
            }
            else
            {
                string name = ( (ComboBox)sender ).Name.Split( '_' )[1];


                foreach (DataRow dtrow in dati.Rows)
                {
                    if (dtrow["ID"].ToString() != name)
                        continue;

                    string resultvalue = "";

                    switch( ( (ComboBox)sender ).SelectedIndex )
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

                    dtrow["cmbRI"] = resultvalue;
                }
            }
        }

        void txt_MouseDown( object sender, MouseButtonEventArgs e )
        {
            if ( e.ClickCount == 2 )
            {
                if(bilancioAssociato == "")
                {
                    MessageBox.Show( "Per accedere alla carta occorre aver creato il bilanco.", "Attenzione" );
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
                wa.SessioniTitoli.Add( 0, SessioniTitoli[SessioneNow].ToString());

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
}
