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
    public partial class ucValutazioneAmbiente : UserControl
    {
        public int id;
        private DataTable dati = null;

   
        private Hashtable Sessioni;
        private int SessioneNow;
        private string IDTree;

		public ucValutazioneAmbiente()
        {
            InitializeComponent();            
        }

        public void Load( string ID, Hashtable _Sessioni, int _SessioneNow, string _IDTree, string IDCliente, string IDSessione)
        {

         
            id = int.Parse(ID);
            cBusinessObjects.idcliente = int.Parse(IDCliente);
            cBusinessObjects.idsessione = int.Parse(IDSessione);

            dati = cBusinessObjects.GetData(id, typeof(ValutazioneAmbiente));

            Sessioni = _Sessioni;
            SessioneNow = _SessioneNow;
            IDTree = _IDTree;

			ArrayList IDNodes = new ArrayList();
            
            IDNodes.Add("964");
			IDNodes.Add("64");
			IDNodes.Add("65");
			IDNodes.Add("66");
			IDNodes.Add("67");
			IDNodes.Add("68");
			IDNodes.Add("69");

			int Alto = 0;
			int Medio = 0;
			int Basso = 0;

			int AltoTot = 0;
			int MedioTot = 0;
			int BassoTot = 0;

			TextBlock txt;
			int row = 1;

			Grid grd = new Grid();
			ColumnDefinition cd = new ColumnDefinition();
			cd.Width = GridLength.Auto;
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
			grd.RowDefinitions.Add(rd);

			txt = new TextBlock();
            txt.Text = "Doppio Click per accedere alle Carte di Lavoro";
			grd.Children.Add(txt);
			Grid.SetRow(txt, 0);
			Grid.SetColumn(txt, 0);

			Border brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.BorderBrush = Brushes.LightGray;
			brd.Background = Brushes.LightGray;
			brd.Padding = new Thickness(2.0);

			txt = new TextBlock();
			txt.Text = "Alto";
            txt.FontSize = 14;
            txt.TextAlignment = TextAlignment.Center;
            txt.FontWeight = FontWeights.Bold;
            txt.Margin = new Thickness(0, 0, 0, 10);

			brd.Child = txt;

			grd.Children.Add(brd);
			Grid.SetRow(brd, 0);
			Grid.SetColumn(brd, 1);

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.BorderBrush = Brushes.LightGray;
			brd.Background = Brushes.LightGray;
			brd.Padding = new Thickness(2.0);

			txt = new TextBlock();
			txt.Text = "Medio";
            txt.FontSize = 14;
            txt.TextAlignment = TextAlignment.Center;
            txt.FontWeight = FontWeights.Bold;
            txt.Margin = new Thickness(0, 0, 0, 10);

			brd.Child = txt;

			grd.Children.Add(brd);
			Grid.SetRow(brd, 0);
			Grid.SetColumn(brd, 2);

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.BorderBrush = Brushes.LightGray;
			brd.Background = Brushes.LightGray;
			brd.Padding = new Thickness(2.0);

			txt = new TextBlock();
			txt.Text = "Basso";
            txt.FontSize = 14;
            txt.TextAlignment = TextAlignment.Center;
            txt.FontWeight = FontWeights.Bold;
            txt.Margin = new Thickness(0, 0, 0, 10);

			brd.Child = txt;

			grd.Children.Add(brd);
			Grid.SetRow(brd, 0);
			Grid.SetColumn(brd, 3);

			foreach (string IDN in IDNodes)				
			{
				
                DataTable chkpdati = cBusinessObjects.GetData(int.Parse(IDN), typeof(CheckListPlus));

                //if(node.Name == "Dato" && node.Attributes["ID"].Value != _ID)
                if (chkpdati.Rows.Count>0)
				{
					rd = new RowDefinition();
					grd.RowDefinitions.Add(rd);

					Alto = 0;
					Medio = 0;
					Basso = 0;

				
                    foreach (DataRow dtrow in chkpdati.Rows)
                    {
						if(dtrow["value"] != null)
						{
							switch (dtrow["value"].ToString())
							{
								case "Alto":
                                case "Si":
									Alto++;
									AltoTot++;
									break;
								case "Medio":
                              	case "No":
									Medio++;
									MedioTot++;
									break;
								//case "NA":
               case "Basso":
                               	//case "":
									Basso++;
									BassoTot++;
                  break;

               default: break;
              }							
						}
					}

					RevisoftApplication.XmlManager xt = new XmlManager();
					xt.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
					XmlDataProvider TreeXmlProvider = new XmlDataProvider();
					TreeXmlProvider.Document = cBusinessObjects.NewLoadEncodedFile("",(Convert.ToInt32(App.TipoFile.Revisione)).ToString() );
					XmlNode tnode = TreeXmlProvider.Document.SelectSingleNode("/Tree//Node[@ID=" + IDN + "]");
                    if(tnode==null)
                    {
	                 tnode = TreeXmlProvider.Document.SelectSingleNode("/Tree//Node/Node[@ID=" + IDN + "]");    
                    }
					brd = new Border();
					brd.BorderThickness = new Thickness(1.0);
					brd.BorderBrush = Brushes.LightGray;
					if (row % 2 == 0)
					{
						brd.Background = new SolidColorBrush(Color.FromArgb(126, 241, 241, 241));
					}
					else
					{
						brd.Background = Brushes.White;
					}

					brd.Padding = new Thickness(2.0);

					txt = new TextBlock();
					txt.Text = tnode.Attributes["Codice"].Value + "\t" + tnode.Attributes["Titolo"].Value;
                    txt.ToolTip = "Fare Doppio CLick per aprire la Carta di lavoro " + tnode.Attributes["Codice"].Value;
                    txt.MouseDown += new MouseButtonEventHandler( txt_MouseDownCicli );

				
                    bool trovato = false;

                    foreach (DataRow dtrow in dati.Rows)
                    {
                        if (dtrow["ID"].ToString() == IDN)
                        {
                            trovato = true;
                        }
                    }
                    if(!trovato)
                    {
                        dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, IDN);
                    }

                     foreach (DataRow dtrow in dati.Rows)
                    {
                        if (dtrow["ID"].ToString() == IDN)
                        {

                            dtrow["name"] = tnode.Attributes["Codice"].Value + " " + tnode.Attributes["Titolo"].Value;

					        txt.FontSize = 13;

					        brd.Child = txt;

					        grd.Children.Add(brd);
					        Grid.SetRow(brd, row);
					        Grid.SetColumn(brd, 0);

					        brd = new Border();
					        brd.BorderThickness = new Thickness(1.0);
					        brd.BorderBrush = Brushes.LightGray;
					        if (row % 2 == 0)
					        {
						        brd.Background = new SolidColorBrush(Color.FromArgb(126, 241, 241, 241));
					        }
					        else
					        {
						        brd.Background = Brushes.White;
					        }

					        brd.Padding = new Thickness(2.0);

					        txt = new TextBlock();
					        txt.Text = Alto.ToString();
                            txt.TextAlignment = TextAlignment.Center;

                            dtrow["Alto"] = txt.Text;

					        brd.Child = txt;

					        grd.Children.Add(brd);
					        Grid.SetRow(brd, row);
					        Grid.SetColumn(brd, 1);

					        brd = new Border();
					        brd.BorderThickness = new Thickness(1.0);
					        brd.BorderBrush = Brushes.LightGray;
					        if (row % 2 == 0)
					        {
						        brd.Background = new SolidColorBrush(Color.FromArgb(126, 241, 241, 241));
					        }
					        else
					        {
						        brd.Background = Brushes.White;
					        }

					        brd.Padding = new Thickness(2.0);

					        txt = new TextBlock();
					        txt.Text = Medio.ToString();
					        txt.TextAlignment = TextAlignment.Center;
                            
                            dtrow["Medio"] = txt.Text;

					        brd.Child = txt;

					        grd.Children.Add(brd);
					        Grid.SetRow(brd, row);
					        Grid.SetColumn(brd, 2);

					        brd = new Border();
					        brd.BorderThickness = new Thickness(1.0);
					        brd.BorderBrush = Brushes.LightGray;
					        if (row % 2 == 0)
					        {
						        brd.Background = new SolidColorBrush(Color.FromArgb(126, 241, 241, 241));
					        }
					        else
					        {
						        brd.Background = Brushes.White;
					        }

					        brd.Padding = new Thickness(2.0);

					        txt = new TextBlock();
					        txt.Text = Basso.ToString();
					        txt.TextAlignment = TextAlignment.Center;

                            dtrow["Basso"] = txt.Text;
                           
                            brd.Child = txt;

                            grd.Children.Add(brd);
                            Grid.SetRow(brd, row);
                            Grid.SetColumn(brd, 3);
                        }
                    }
                    row++;
				}
			}

			rd = new RowDefinition();
			grd.RowDefinitions.Add(rd);

            dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, "-1");
            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["ID"].ToString() == "-1")
                {


                    dtrow["name"] = "Totale";

			        txt = new TextBlock();
			        grd.Children.Add(txt);
			        Grid.SetRow(txt, row);
			        Grid.SetColumn(txt, 0);

			        brd = new Border();
			        brd.BorderThickness = new Thickness(1.0);
			        brd.BorderBrush = Brushes.LightGray;
			        brd.Background = Brushes.LightGray;
			        brd.Padding = new Thickness(2.0);

			        txt = new TextBlock();
			        txt.Text = AltoTot.ToString();
                    txt.TextAlignment = TextAlignment.Center;
                    txt.FontWeight = FontWeights.Bold;
                    txt.Foreground = Brushes.Red;

                    dtrow["Alto"] = txt.Text;

			        brd.Child = txt;

			        grd.Children.Add(brd);
			        Grid.SetRow(brd, row);
			        Grid.SetColumn(brd, 1);

			        brd = new Border();
			        brd.BorderThickness = new Thickness(1.0);
			        brd.BorderBrush = Brushes.LightGray;
			        brd.Background = Brushes.LightGray;
			        brd.Padding = new Thickness(2.0);

			        txt = new TextBlock();
			        txt.Text = MedioTot.ToString();
                    txt.TextAlignment = TextAlignment.Center;
                    txt.FontWeight = FontWeights.Bold;
                    txt.Foreground = Brushes.Red;

                    dtrow["Medio"] = txt.Text;
                       
			        brd.Child = txt;

			        grd.Children.Add(brd);
			        Grid.SetRow(brd, row);
			        Grid.SetColumn(brd, 2);

			        brd = new Border();
			        brd.BorderThickness = new Thickness(1.0);
			        brd.BorderBrush = Brushes.LightGray;
			        brd.Background = Brushes.LightGray;
			        brd.Padding = new Thickness(2.0);

			        txt = new TextBlock();
			        txt.Text = BassoTot.ToString();
                    txt.TextAlignment = TextAlignment.Center;
                    txt.FontWeight = FontWeights.Bold;
                    txt.Foreground = Brushes.Red;


                    dtrow["Basso"] = txt.Text;
                    

			        brd.Child = txt;

			        grd.Children.Add(brd);
			        Grid.SetRow(brd, row);
			        Grid.SetColumn(brd, 3);

			        brdMain.Child = grd;
                }
            }
        }

        public int Save()
		{
            // elimina totale
            foreach (DataRow dtrow in this.dati.Rows)
            {
                if (dtrow["name"].ToString()=="-1")
                {
                    dtrow.Delete();
                    break;
                }

            }
            dati.AcceptChanges();
            return cBusinessObjects.SaveData(id, dati, typeof(ValutazioneAmbiente));
        }

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double newsize = e.NewSize.Width - 30.0;

			try
			{
				brdMain.Width = Convert.ToDouble(newsize);
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}
		}

        void txt_MouseDownCicli( object sender, MouseButtonEventArgs e )
        {
            if ( e.ClickCount == 2 )
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
                TreeXmlProvider.Document =cBusinessObjects.NewLoadEncodedFile( "", (Convert.ToInt32(App.TipoFile.Revisione)).ToString() );

                if ( TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode( "/Tree" ) != null )
                {
                    foreach ( XmlNode item in TreeXmlProvider.Document.SelectNodes( "/Tree//Node" ) )
                    {
                        if ( item.Attributes["Codice"].Value == ((TextBlock)(sender)).ToolTip.ToString().Replace( "Fare Doppio CLick per aprire la Carta di lavoro ", "" ) )
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

                wa.ShowDialog();
            }
        }
    }
}
