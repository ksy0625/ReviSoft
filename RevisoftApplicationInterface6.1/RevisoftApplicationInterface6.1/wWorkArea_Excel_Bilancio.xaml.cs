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
using System.Globalization;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;
using System.Threading;
using RevisoftApplication;
using System.Data;



namespace UserControls
{
	public partial class uc_Excel_Bilancio : UserControl
    {
       
        public int id;
        private DataTable dati = null;
        private DataTable datiALL = null;
        private DataTable datiTestata = null;

        private string IDB_Padre = "227";
		private string IDBA_Padre = "229";

        private string left = "./Images/icone/navigate_down.png";
        private string down = "./Images/icone/navigate_left.png";

        public int countertabindex = 1;
		public string Titolo = "";

		private int WidthColonne = 100;

        private XmlDataProviderManager _x = null;
		private XmlDataProviderManager _x_AP = null;
		private XmlDataProviderManager _y = null;
        private string _ID = "";
		private bool alreadyShowed = false;

		Hashtable ht_negativi = new Hashtable();
		Hashtable ht_somme = new Hashtable();
		
		Hashtable b_valoreEA = new Hashtable();
		Hashtable b_valoreEP = new Hashtable();
		Hashtable b_valoreDIFF = new Hashtable();

        Hashtable rowBV = new Hashtable();

        private bool _readonly = false;

        protected object _control_parent = null;

		public bool Abbreviato
		{
			set
			{
				if (value)
				{
					IDB_Padre = IDBA_Padre;
				}
			}
		}

		public bool ReadOnly
		{
			set
			{
				_readonly = value;
			}
		}

		public uc_Excel_Bilancio(int _countertabindex)
        {


            CultureInfo culture = CultureInfo.CreateSpecificCulture("it-IT"); 
            if (alreadyShowed) { }
            InitializeComponent();

            Loaded += Uc_Excel_Bilancio_Loaded;

            countertabindex = _countertabindex;
        }

        private void Uc_Excel_Bilancio_Loaded(object sender, RoutedEventArgs e)
        {
            _control_parent = UIHelper.TryFindParent<ucNodoMultiploVerticale>((UIElement)sender); //Workaround perchè altrimenti non si trova il parent
        }

        public void LoadDataSource(ref XmlDataProviderManager x, string ID, XmlDataProviderManager x_AP, string filebilancio,string IDCliente,string IDSessione)
        {
            if ( ID == "134" )
            {
                IDB_Padre = "134";
            }

            if (ID == "166")
            {
                IDB_Padre = "166";
            }

            if (ID == "172")
            {
                IDB_Padre = "172";
                IDBA_Padre = "172";
            }

            if (ID == "2016134")
            {
                IDB_Padre = "2016134";
            }

            if (ID == "2016174")
            {
                IDB_Padre = "2016174";
                IDBA_Padre = "2016174";
            }

            if (ID == "2016186")
            {
                IDB_Padre = "2016186";
                IDBA_Padre = "2016186";
            }

            id = int.Parse(ID);
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());

            DataTable datistato = cBusinessObjects.GetData(id, typeof(StatoNodi));
            DataTable datiBV = cBusinessObjects.GetData(id, typeof(BilancioVerifica));

            

            dati = cBusinessObjects.GetData(id, typeof(Excel_Bilancio));
            datiALL = dati.Copy();
       
            dati = cBusinessObjects.GetDataFiltered(dati, System.IO.Path.GetFileName(filebilancio),"template");

            datiTestata = cBusinessObjects.GetData(id, typeof(Excel_Bilancio_Testata));
            if (datiTestata.Rows.Count == 0)
                datiTestata.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);


            //_x = x.Clone();

            _x = x;

			_ID = ID;
			_x_AP = x_AP;

			_y = new XmlDataProviderManager(filebilancio, true);

            bool firsttime = true;
			ht_somme.Clear();
			b_valoreEP.Clear();
			b_valoreEA.Clear();
            //bool trovato;
            object tmpchild=null;
            string tmpnamehere = "";
            foreach (XmlNode item in _y.Document.SelectNodes("/Dato/MacroGruppo"))
			{
                if (firsttime)
                {
                    firsttime = false;
                }
                else
                {
                    break;
                }

				//Border brd = new Border();
				//brd.BorderThickness = new Thickness(1.0);
				//brd.CornerRadius = new CornerRadius(10.0);
				//brd.BorderBrush = Brushes.LightGray;

				//LinearGradientBrush lgb = new LinearGradientBrush();
				//lgb.StartPoint = new Point(0.5, 0.0);
				//lgb.EndPoint = new Point(0.5, 1.0);
				//lgb.GradientStops.Add(new GradientStop(Brushes.LightGray.Color, 0.0));
				//lgb.GradientStops.Add(new GradientStop(Brushes.Gray.Color, 1.0));

				//brd.Background = lgb;

				//TextBlock txt = new TextBlock();
				//txt.Text = item.Attributes["name"].Value;
				//txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
				//txt.FontWeight = FontWeights.Bold;
				//txt.FontSize = 18.0;
				//txt.Margin = new Thickness(0.0, 10.0, 0.0, 10.0);

				//brd.Child = txt;

				//stpMain.Children.Add(brd);

				Border brd2 = new Border();
				brd2.BorderThickness = new Thickness(1.0);
				brd2.CornerRadius = new CornerRadius(10.0);
				brd2.BorderBrush = Brushes.LightGray;
				brd2.Margin = new Thickness(0.0, 0.0, 0.0, 15.0);
				brd2.Padding = new Thickness(15.0, 0.0, 15.0, 0.0);

				LinearGradientBrush lgb2 = new LinearGradientBrush();
				lgb2.StartPoint = new Point(0.0, 0.5);
				lgb2.EndPoint = new Point(1.0, 0.5);
				lgb2.GradientStops.Add(new GradientStop(Brushes.White.Color, 0.0));
				lgb2.GradientStops.Add(new GradientStop(Brushes.LightGray.Color, 1.0));

				brd2.Background = lgb2;

                Grid grd = new Grid();
                ColumnDefinition cd = new ColumnDefinition();
				cd.Width = GridLength.Auto;
				grd.ColumnDefinitions.Add(cd);
				cd = new ColumnDefinition();
				cd.Width = new GridLength(1, GridUnitType.Star);
				grd.ColumnDefinitions.Add(cd);
				cd = new ColumnDefinition();
				cd.Width = new GridLength(WidthColonne + 10); 
				grd.ColumnDefinitions.Add(cd);
				cd = new ColumnDefinition();
				cd.Width = new GridLength(WidthColonne + 10); 
				grd.ColumnDefinitions.Add(cd);
				cd = new ColumnDefinition();
				cd.Width = new GridLength(WidthColonne + 10); 
				grd.ColumnDefinitions.Add(cd);

				#region dati

				bool firstrow = true;
				bool AnnoPrecedenteEsiste = false;
				string somma = "";
								
				RowDefinition rd;
				int row = 1;

				ArrayList b_ID = new ArrayList();
				Hashtable b_rigaVuota		= new Hashtable();
				Hashtable b_Codice			= new Hashtable();
				Hashtable b_paddingCodice	= new Hashtable();
				Hashtable b_boldCodice		= new Hashtable();
				Hashtable b_name			= new Hashtable();
				Hashtable b_boldName		= new Hashtable();
				Hashtable b_sizeName		= new Hashtable();
				Hashtable b_italicName		= new Hashtable();
				Hashtable b_colorName		= new Hashtable();
				Hashtable b_bgName			= new Hashtable();
				Hashtable b_noData			= new Hashtable();
				Hashtable b_negativo		= new Hashtable();
				Hashtable b_somma			= new Hashtable();
				Hashtable b_bgEP			= new Hashtable();
				Hashtable b_boldEP			= new Hashtable();
				Hashtable b_colorEP			= new Hashtable();
				Hashtable b_bgEA			= new Hashtable();
				Hashtable b_boldEA			= new Hashtable();
				Hashtable b_colorEA			= new Hashtable();
				Hashtable b_bgID			= new Hashtable();
				Hashtable b_colorID			= new Hashtable();				
				
				foreach (XmlNode bilancio in item.SelectNodes("Bilancio"))
				{
					string IDTMP = bilancio.Attributes["ID"].Value;

					b_ID.Add(IDTMP);

					if (bilancio.Attributes["rigaVuota"] != null)
					{
						b_rigaVuota.Add(IDTMP, bilancio.Attributes["rigaVuota"].Value);
					}

					if (bilancio.Attributes["Codice"] != null)
					{
						b_Codice.Add(IDTMP, bilancio.Attributes["Codice"].Value);
					}

					if (bilancio.Attributes["paddingCodice"] != null)
					{
						b_paddingCodice.Add(IDTMP, bilancio.Attributes["paddingCodice"].Value);
					}

					if (bilancio.Attributes["boldCodice"] != null)
					{
						b_boldCodice.Add(IDTMP, bilancio.Attributes["boldCodice"].Value);
					}

					if (bilancio.Attributes["name"] != null)
					{
						b_name.Add(IDTMP, bilancio.Attributes["name"].Value);
					}

					if (bilancio.Attributes["boldName"] != null)
					{
						b_boldName.Add(IDTMP, bilancio.Attributes["boldName"].Value);
					}

					if (bilancio.Attributes["sizeName"] != null)
					{
						b_sizeName.Add(IDTMP, bilancio.Attributes["sizeName"].Value);
					}

					if (bilancio.Attributes["italicName"] != null)
					{
						b_italicName.Add(IDTMP, bilancio.Attributes["italicName"].Value);
					}

					if (bilancio.Attributes["colorName"] != null)
					{
						b_colorName.Add(IDTMP, bilancio.Attributes["colorName"].Value);
					}

					if (bilancio.Attributes["bgName"] != null)
					{
						b_bgName.Add(IDTMP, bilancio.Attributes["bgName"].Value);
					}

					if (bilancio.Attributes["noData"] != null)
					{
						b_noData.Add(IDTMP, bilancio.Attributes["noData"].Value);
					}

					if (bilancio.Attributes["negativo"] != null)
					{
						b_negativo.Add(IDTMP, bilancio.Attributes["negativo"].Value);
					}

					if (bilancio.Attributes["somma"] != null)
					{
						b_somma.Add(IDTMP, bilancio.Attributes["somma"].Value);
					}

					if (bilancio.Attributes["bgEP"] != null)
					{
						b_bgEP.Add(IDTMP, bilancio.Attributes["bgEP"].Value);
					}

					if (bilancio.Attributes["boldEP"] != null)
					{
						b_boldEP.Add(IDTMP, bilancio.Attributes["boldEP"].Value);
					}

					if (bilancio.Attributes["colorEP"] != null)
					{
						b_colorEP.Add(IDTMP, bilancio.Attributes["colorEP"].Value);
					}

					if (bilancio.Attributes["bgEA"] != null)
					{
						b_bgEA.Add(IDTMP, bilancio.Attributes["bgEA"].Value);
					}

					if (bilancio.Attributes["boldEA"] != null)
					{
						b_boldEA.Add(IDTMP, bilancio.Attributes["boldEA"].Value);
					}

					if (bilancio.Attributes["colorEA"] != null)
					{
						b_colorEA.Add(IDTMP, bilancio.Attributes["colorEA"].Value);
					}

					if (bilancio.Attributes["bgID"] != null)
					{
						b_bgID.Add(IDTMP, bilancio.Attributes["bgID"].Value);
					}

					if (bilancio.Attributes["colorID"] != null)
					{
						b_colorID.Add(IDTMP, bilancio.Attributes["colorID"].Value);
					}
                    DataRow tmpNode = null;
                    foreach (DataRow dtrow in dati.Rows)
                    {
                        if (dtrow["ID"].ToString() == IDTMP)
                            tmpNode = dtrow;
                    }
                    if (tmpNode==null)
                    {
                        tmpNode = dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, System.IO.Path.GetFileName(filebilancio));
                        tmpNode["ID"] = IDTMP;
                    }


                    tmpNode["Titolo"] = Titolo;
                    if (bilancio.Attributes["rigaVuota"] != null)
                        tmpNode["rigaVuota"] = bilancio.Attributes["rigaVuota"].Value;
                    if (bilancio.Attributes["Codice"] != null)
                        tmpNode["Codice"] = bilancio.Attributes["Codice"].Value;
                    if (bilancio.Attributes["paddingCodice"] != null)
                        tmpNode["paddingCodice"] = bilancio.Attributes["paddingCodice"].Value;
                    if (bilancio.Attributes["name"] != null)
                        tmpNode["name"] = bilancio.Attributes["name"].Value;
                    if (bilancio.Attributes["noData"] != null)
                        tmpNode["noData"] = bilancio.Attributes["noData"].Value;
                    if (bilancio.Attributes["bgEP"] != null)
                        tmpNode["bg"] = bilancio.Attributes["bgEP"].Value;

                        if (tmpNode["EA"].ToString() != "")
                        {
                            b_valoreEA[IDTMP] = tmpNode["EA"].ToString();
                        }
                        else
                        {
                            b_valoreEA[IDTMP] = "0";
                        }
                     //   bool trovato2 = false;
                       
                      //  foreach(DataRow dd in datistato.Rows)
                       //      {
                        //         if (dd["Stato"].ToString() == "2")
                     //                trovato2 = true;
                      //       }
                      //   if (x_AP == null || (x_AP != null && !trovato2))
                      //  {
                            if (tmpNode["EP"].ToString() != "")
                            {
                                b_valoreEP.Add(IDTMP, tmpNode["EP"].ToString());
                            }
                            else
                            {
                                b_valoreEP.Add(IDTMP, "0");
                            }
                     //   }

                    //Calcolo valori anno precedente
                    if (x_AP != null)
                    {
                        bool trovato3 = false;
                       
                        foreach (DataRow dd in datistato.Rows)
                        {
                            if (dd["Stato"].ToString() == "2")
                                trovato3 = true;
                        }
                         if (trovato3)
                        {
                            if (tmpNode["EA"].ToString() != "")
                            {
                                b_valoreEP.Add(IDTMP, tmpNode["EA"].ToString());
                                AnnoPrecedenteEsiste = true;
                            }
                            else
                            {
                                b_valoreEP.Add(IDTMP, "0");
                            }
                        }
                        else
                        {
                            if (!b_valoreEP.Contains(IDTMP))
                            {
                                b_valoreEP.Add(IDTMP, "0");
                            }
                        }
                    }

                }
#endregion

				foreach (string tmpID in b_ID)
				{
                    rd = new RowDefinition();
                                        
					if (firstrow)
					{
						rd.Height = new GridLength(40);
					}
					else
					{
						rd.Height = new GridLength(20);
					}

					grd.RowDefinitions.Add(rd);

					bool gotonextrow = false;

					#region riga vuota
					if(b_rigaVuota.Contains(tmpID) && b_rigaVuota[tmpID].ToString() == "1")
					{
						gotonextrow = true;
					}
					#endregion

					if (!gotonextrow)
					{
                        #region codice
                        
                        TextBlock txtCodice = new TextBlock();
                        
						if (firstrow)
						{
							txtCodice.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
						}

						if (b_Codice.Contains(tmpID))
						{
							txtCodice.Text = b_Codice[tmpID].ToString();
						}
						else
						{
							txtCodice.Text = "";
						}

						if (b_paddingCodice.Contains(tmpID))
						{
							txtCodice.Padding = new Thickness(Convert.ToDouble(b_paddingCodice[tmpID].ToString()) * 1, 0.0, 0.0, 0.0);
						}

						if (b_boldCodice.Contains(tmpID) && b_boldCodice[tmpID].ToString() == "1")
						{
							txtCodice.FontWeight = FontWeights.Bold;
						}
						else
						{
							txtCodice.FontWeight = FontWeights.Normal;
						}
                        
                        grd.Children.Add(txtCodice);
						Grid.SetRow(txtCodice, row);
						Grid.SetColumn(txtCodice, 0);
                        #endregion

                        #region testo

                        TextBlock txtName = new TextBlock();
                        txtName.TextWrapping = TextWrapping.Wrap;
                        txtName.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

                        StackPanel stp = new StackPanel();
                        stp.Orientation = Orientation.Horizontal;

                        double marginhere = 0.0;
                        foreach (DataRow dtrow in dati.Rows)
                        {
                            if (dtrow["ID"].ToString() == tmpID)
                            {
                                if(dtrow["opened"].ToString()!="")
                                {
                                    btnEspandiBV.Visibility = Visibility.Visible;

                                    Image imgbtn = new Image();
                                    imgbtn.Name = "btn_Expand_" + tmpID;
                                    imgbtn.Margin = new Thickness(2, 5, 2, 0);
                                    imgbtn.ToolTip = "Espandi";
                                    imgbtn.Tag = "opened";
                                    imgbtn.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                                    imgbtn.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                                    if (this.FindName(imgbtn.Name) == null)
                                    {
                                        this.RegisterName(imgbtn.Name, imgbtn);
                                    }


                                    var uriSource = new Uri(left, UriKind.Relative);
                                    imgbtn.Source = new BitmapImage(uriSource);
                                    imgbtn.Height = 10.0;
                                    imgbtn.Width = 10.0;
                                    imgbtn.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

                                    stp.Children.Add(imgbtn);
                                }
                                else
                                {
                                    marginhere = 14.0;

                                }
                            }
                        }

                     

                        if (firstrow)
						{
							txtName.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
						}						

						if (b_name.Contains(tmpID))
						{
							txtName.Text = b_name[tmpID].ToString();
						}
						else
						{
							txtName.Text = "";
						}

						if (b_boldName.Contains(tmpID) && b_boldName[tmpID].ToString() == "1")
						{
							txtName.FontWeight = FontWeights.Bold;
						}
						else
						{
							txtName.FontWeight = FontWeights.Normal;
						}

                        //andrea
                        if (b_sizeName.Contains(tmpID))
                        {
                            txtName.FontSize = Convert.ToDouble(b_sizeName[tmpID].ToString()) + 2.0;
                        }
                        else
                            txtName.FontSize +=  2.0;

						if (b_italicName.Contains(tmpID) && b_italicName[tmpID].ToString() == "1")
						{
							txtName.FontStyle = FontStyles.Italic;
						}
						else
						{
							txtName.FontStyle = FontStyles.Normal;
						}						

						if (b_colorName.Contains(tmpID))
						{
							switch (b_colorName[tmpID].ToString())
							{
								case "Blue":
									txtName.Foreground = Brushes.Blue;
									break;
								case "Gray":
									txtName.Foreground = Brushes.Gray;
									break;
								default:
									txtName.Foreground = Brushes.Black;
									break;
							}
						}

						if (b_bgName.Contains(tmpID))
						{
							switch (b_bgName[tmpID].ToString())
							{
								case "LightBlue":
									txtName.Background = Brushes.LightBlue;
									break;
								case "Yellow":
									txtName.Background = Brushes.LightYellow;
									break;
								default:
									txtName.Background = Brushes.Transparent;
									break;
							}
						}

                        bool noDataPre = false;

                        if ((b_noData.Contains(tmpID) && b_noData[tmpID].ToString() == "1") || b_somma.Contains(tmpID))
                        {
                            noDataPre = true;
                        }

                        if (noDataPre)
                        {
                            stp.Children.Add(txtName);
                            txtName.Margin = new Thickness(marginhere, 0.0, 0.0, 0.0);
                        }
                        else
                        {
                            Border brdText = new Border();
                            brdText.BorderThickness = new Thickness(0.0, 0.0, 0.0, 1.0);
                            brdText.BorderBrush = Brushes.Gray;
                            brdText.Margin = new Thickness(marginhere, 0.0, 0.0, 0.0);

                            txtName.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);

                            brdText.Child = txtName;

                            stp.Children.Add(brdText);
                            
                        }

                        grd.Children.Add(stp);
                        Grid.SetRow(stp, row);
                        Grid.SetColumn(stp, 1);
                        
                        #endregion

                        bool noData = false;

						if (b_noData.Contains(tmpID) && b_noData[tmpID].ToString() == "1")
						{
							noData = true;
						}

						if (!noData)
						{
							somma = "";

							if (b_somma.Contains(tmpID))
							{
								somma = b_somma[tmpID].ToString();
							}

							#region esercizio attuale

							if (firstrow)
							{
                                if (IDB_Padre == "166" || IDB_Padre == "172")
                                {
                                    TextBox txtTitoloEA = new TextBox();
                                     txtTitoloEA.AllowDrop = false;

                               
                                    foreach (DataRow dtrow in datiTestata.Rows)
                                    {
                                    
                                        if (dtrow["TitoloEA"].ToString() != "")
                                        {
                                            txtTitoloEA.Text = dtrow["TitoloEA"].ToString();
                                        }
                                    }
                                   
                                    txtTitoloEA.Width = Convert.ToDouble(WidthColonne - 10);
                                    txtTitoloEA.Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
                                    txtTitoloEA.TextAlignment = TextAlignment.Center;
                                    txtTitoloEA.FontWeight = FontWeights.Bold;
                                    txtTitoloEA.TextWrapping = TextWrapping.Wrap;
                                    txtTitoloEA.LostFocus += TxtTitoloEA_LostFocus;
                                    txtTitoloEA.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
                                    txtTitoloEA.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
                                    grd.Children.Add(txtTitoloEA);
                                    Grid.SetRow(txtTitoloEA, 0);
                                    Grid.SetColumn(txtTitoloEA, 2);
                                }
                                else
                                {
                                    TextBlock txtTitoloEA = new TextBlock();

                                    if (IDB_Padre == "134" || IDB_Padre == "2016134" || IDB_Padre == "2016174" || IDB_Padre == "2016186")
                                    {
                                        txtTitoloEA.Text = "ULTIMO ES. APPROVATO";
                                    }
                                    else
                                    {
                                        txtTitoloEA.Text = "ESERCIZIO ATTUALE";
                                    }

                                    txtTitoloEA.Width = Convert.ToDouble(WidthColonne - 10);
                                    txtTitoloEA.Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
                                    txtTitoloEA.TextAlignment = TextAlignment.Center;
                                    txtTitoloEA.FontWeight = FontWeights.Bold;
                                    txtTitoloEA.TextWrapping = TextWrapping.Wrap;
                                    grd.Children.Add(txtTitoloEA);
                                    Grid.SetRow(txtTitoloEA, 0);
                                    Grid.SetColumn(txtTitoloEA, 2);
                                }
							}

							TextBox txtEA = new TextBox();
              txtEA.AllowDrop = false;
							txtEA.TextAlignment = TextAlignment.Right;
							txtEA.Width = Convert.ToDouble(WidthColonne);
							txtEA.Name = "txtEA_" + tmpID;
							txtEA.LostFocus += new RoutedEventHandler(TextBox_LostFocus);
							txtEA.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
							txtEA.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);


                            txtEA.TabIndex = countertabindex++;

                            if (_readonly)
							{
								//txtEA.IsReadOnly = true;
								txtEA.IsTabStop = false;
							}
							else
							{
                                
                                txtEA.IsTabStop = true;
							}

							//somma = "";

							//try
							//{
							//    somma = bilancio.Attributes["somma"].Value;
							//}
							//catch (Exception ex)
							//{
							//    string log = ex.Message;
							//}

							if (somma != "")
							{
								txtEA.IsReadOnly = true;
								txtEA.IsTabStop = false;

								bool atleastonenotzero = false;

								string[] tobeadded = somma.Split('|');
								double totale = 0.0;
								foreach (string tba in tobeadded)
								{
									double tmpvalue = 0.0;
									if (b_valoreEA.Contains(tba))
									{
										tmpvalue = 0.0;
										double.TryParse(b_valoreEA[tba.Replace("-", "")].ToString(), out tmpvalue);
									}
									else
									{
										//XmlNode tryNode;
                                        foreach (DataRow dtrow in datiALL.Rows)
                                        {
                                           if(dtrow["ID"].ToString() == tba.Replace("-", ""))
                                           {
                                                if(dtrow["EA"].ToString()!="")
                                                double.TryParse(dtrow["EA"].ToString(), out tmpvalue);
                                            }
                                        }
                                       
									}

									if (tmpvalue != 0.0)
									{
										atleastonenotzero = true;

										if (Convert.ToInt32(tba) > 0)
										{
											totale += tmpvalue;
										}
										else
										{
											totale -= tmpvalue;
										}
									}
								}

								if (atleastonenotzero)
								{
									totale += 0.001;
								}

								txtEA.Text = ConvertNumber(totale.ToString());
							}
							else
							{
								if (b_valoreEA.Contains(tmpID))
								{
                                    if(b_valoreEA[tmpID].ToString()!="")
                                    {

                                        
                                        txtEA.Text = ConvertNumber(b_valoreEA[tmpID].ToString());
                                       

                                    }
                                    else
                                    {
                                        txtEA.Text = ConvertNumber("0");
                                    }
                                     
                                }
								else
								{
									txtEA.Text = ConvertNumber("0");
								}
							}
                            txtEA.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);

                            if (b_valoreEA.Contains(tmpID))
							{
								b_valoreEA[tmpID] = txtEA.Text;
							}
							else
							{
								b_valoreEA.Add(tmpID, txtEA.Text);
							}

							if (b_bgEA.Contains(tmpID))
							{
								switch (b_bgEA[tmpID].ToString())
								{
									case "LightBlue":
										txtEA.Background = Brushes.LightBlue;
										break;
									case "Brown":
										txtEA.Background = Brushes.SandyBrown;
										break;
									case "Green":
										txtEA.Background = Brushes.LightSeaGreen;
										break;
									case "Yellow":
										txtEA.Background = Brushes.LightYellow;
										break;
									case "Transparent":
										txtEA.Background = Brushes.Transparent;
										break;
									default:
										txtEA.Background = Brushes.White;
										break;
								}
							}

							if (b_boldEA.Contains(tmpID) && b_boldEA[tmpID].ToString() == "1")
							{
								txtEA.FontWeight = FontWeights.Bold;
							}
							else
							{
								txtEA.FontWeight = FontWeights.Normal;
							}

							if (b_colorEA.Contains(tmpID))
							{
								switch (b_colorEA[tmpID].ToString())
								{
									case "Red":
										txtEA.Foreground = Brushes.Red;
										break;
									case "Transparent":
										txtEA.Foreground = Brushes.Transparent;
										break;
									default:
										txtEA.Foreground = Brushes.Black;
										break;
								}
							}

							grd.Children.Add(txtEA);
							Grid.SetRow(txtEA, row);
							Grid.SetColumn(txtEA, 2);
							#endregion

							#region esercizio precedente

							if (firstrow)
							{
                                if (IDB_Padre == "166" || IDB_Padre == "172")
                                {
                                    TextBox txtTitoloEP = new TextBox();
                                    txtTitoloEP.AllowDrop = false;

                  
                                   
                                    foreach (DataRow dtrow in datiTestata.Rows)
                                    {
                                        if (dtrow["TitoloEP"].ToString() != "" && dtrow["TitoloEP"].ToString() != "")
                                        {
                                            txtTitoloEP.Text = dtrow["TitoloEP"].ToString();
                                        }
                                    }



                                    txtTitoloEP.Width = Convert.ToDouble(WidthColonne - 10);
                                    txtTitoloEP.Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
                                    txtTitoloEP.TextAlignment = TextAlignment.Center;
                                    txtTitoloEP.FontWeight = FontWeights.Bold;
                                    txtTitoloEP.TextWrapping = TextWrapping.Wrap;
                                    txtTitoloEP.LostFocus += TxtTitoloEP_LostFocus;
                                    txtTitoloEP.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
                                    txtTitoloEP.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
                                    grd.Children.Add(txtTitoloEP);
                                    Grid.SetRow(txtTitoloEP, 0);
                                    Grid.SetColumn(txtTitoloEP, 3);
                                }
                                else
                                {
                                    TextBlock txtTitoloEP = new TextBlock();
                                    txtTitoloEP.Text = "ESERCIZIO PRECEDENTE";
                                    txtTitoloEP.Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
                                    txtTitoloEP.Width = Convert.ToDouble(WidthColonne - 10);
                                    txtTitoloEP.TextAlignment = TextAlignment.Center;
                                    txtTitoloEP.FontWeight = FontWeights.Bold;
                                    txtTitoloEP.TextWrapping = TextWrapping.Wrap;
                                    grd.Children.Add(txtTitoloEP);
                                    Grid.SetRow(txtTitoloEP, 0);
                                    Grid.SetColumn(txtTitoloEP, 3);
                                }
							}

							TextBox txtEP = new TextBox();
            
							txtEP.TextAlignment = TextAlignment.Right;
							txtEP.Width = Convert.ToDouble(WidthColonne);
							txtEP.Name = "txtEP_" + tmpID;
              txtEP.AllowDrop = false;
							txtEP.LostFocus += new RoutedEventHandler(TextBox_LostFocus);
							txtEP.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
							txtEP.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);

                            txtEP.TabIndex = 1000 + countertabindex++;

                            if (_readonly || AnnoPrecedenteEsiste)
							{
								//txtEP.IsReadOnly = true;
								txtEP.IsTabStop = false;
							}
							else
							{
                                
                                txtEP.IsTabStop = true;
							}

							if (b_negativo.Contains(tmpID))
							{
								ht_negativi.Add(tmpID, true);
							}							

							if (somma != "")
							{
								bool atleastonenotzero = false;

								txtEP.IsReadOnly = true;
								txtEP.IsTabStop = false;
								ht_somme.Add(tmpID, somma);

								string[] tobeadded = somma.Split('|');
								double totale = 0.0;
								foreach (string tba in tobeadded)
								{
									double tmpvalue = 0.0;
									if (b_valoreEP.Contains(tba))
									{
										tmpvalue = 0.0;
										double.TryParse(b_valoreEP[tba.Replace("-", "")].ToString(), out tmpvalue);
									}

									if (tmpvalue != 0.0)
									{
										atleastonenotzero = true;

										if (Convert.ToInt32(tba) > 0)
										{
											totale += tmpvalue;
										}
										else
										{
											totale -= tmpvalue;
										}
									}
								}

								if (atleastonenotzero)
								{
									totale += 0.001;
								}

								txtEP.Text = ConvertNumber(totale.ToString());
							}
							else
							{
								
                                if (b_valoreEP[tmpID].ToString() != "")
                                {
                                    txtEP.Text = ConvertNumber(b_valoreEP[tmpID].ToString());
                                }
                                else
                                {
                                    txtEP.Text = ConvertNumber("0");
                                }
                            }

							if (b_valoreEP.Contains(tmpID))
							{
								b_valoreEP[tmpID] = txtEP.Text;
							}
							else
							{
								b_valoreEP.Add(tmpID, txtEP.Text);
							}
                            txtEP.TextChanged += new TextChangedEventHandler(TextBox_TextChanged);

                            if (b_bgEP.Contains(tmpID))
							{
								switch (b_bgEP[tmpID].ToString())
								{
									case "LightBlue":
										txtEP.Background = Brushes.LightBlue;
										break;
									case "Brown":
										txtEP.Background = Brushes.SandyBrown;
										break;
									case "Green":
										txtEP.Background = Brushes.LightSeaGreen;
										break;
									case "Yellow":
										txtEP.Background = Brushes.LightYellow;
										break;
									case "Transparent":
										txtEP.Background = Brushes.Transparent;
										break;
									default:
										txtEP.Background = Brushes.White;
										break;
								}
							}

							if (b_boldEP.Contains(tmpID) && b_boldEP[tmpID].ToString() == "1")
							{
								txtEP.FontWeight = FontWeights.Bold;
							}
							else
							{
								txtEP.FontWeight = FontWeights.Normal;
							}

							if (b_colorEP.Contains(tmpID))
							{
								switch (b_colorEP[tmpID].ToString())
								{
									case "Red":
										txtEP.Foreground = Brushes.Red;
										break;
									case "Transparent":
										txtEP.Foreground = Brushes.Transparent;
										break;
									default:
										txtEP.Foreground = Brushes.Black;
										break;
								}
							}

							grd.Children.Add(txtEP);
							Grid.SetRow(txtEP, row);
							Grid.SetColumn(txtEP, 3);
							#endregion

							#region incremento decremento

							if (firstrow)
							{
								TextBlock txtTitoloInDe = new TextBlock();
								txtTitoloInDe.Text = "Increm. (decrem.)";
								txtTitoloInDe.Width = Convert.ToDouble(WidthColonne - 10);
								txtTitoloInDe.Margin = new Thickness(0.0, 10.0, 0.0, 0.0);
								txtTitoloInDe.TextAlignment = TextAlignment.Center;
								txtTitoloInDe.FontWeight = FontWeights.Bold;
								txtTitoloInDe.TextWrapping = TextWrapping.Wrap;
								grd.Children.Add(txtTitoloInDe);
								Grid.SetRow(txtTitoloInDe, 0);
								Grid.SetColumn(txtTitoloInDe, 4);
								firstrow = false;
							}

							TextBox txtInDe = new TextBox();
							txtInDe.TextAlignment = TextAlignment.Right;
							txtInDe.Width = Convert.ToDouble(WidthColonne);
							txtInDe.Name = "txtInDe_" + tmpID;
              txtInDe.AllowDrop = false;
							txtInDe.Background = Brushes.WhiteSmoke;
							txtInDe.IsReadOnly = true;
							txtInDe.IsTabStop = false;

							double ep = 0.0;
							double ea = 0.0;

							double.TryParse(txtEA.Text, out ea);
							double.TryParse(txtEP.Text, out ep);

							double tmptotvalue = ea - ep;
							if (ea != 0.0 || ep != 0.0)
							{
								tmptotvalue += 0.001;
							}

							txtInDe.Text = ConvertNumber((tmptotvalue).ToString());

							if (b_valoreDIFF.Contains(tmpID))
							{
								b_valoreDIFF[tmpID] = txtInDe.Text;
                              
							}
							else
							{
								b_valoreDIFF.Add(tmpID, txtInDe.Text);
							}

							if (b_bgID.Contains(tmpID))
							{
								switch (b_bgID[tmpID].ToString())
								{
									case "LightBlue":
										txtInDe.Background = Brushes.LightBlue;
										break;
									case "Brown":
										txtInDe.Background = Brushes.SandyBrown;
										break;
									case "Green":
										txtInDe.Background = Brushes.LightSeaGreen;
										break;
									case "Yellow":
										txtInDe.Background = Brushes.LightYellow;
										break;
									case "Transparent":
										txtInDe.Background = Brushes.Transparent;
										break;
									default:
										txtInDe.Background = Brushes.White;
										break;
								}
							}

							if (b_colorID.Contains(tmpID))
							{
								switch (b_colorID[tmpID].ToString())
								{
									case "Red":
										txtInDe.Foreground = Brushes.Red;
										break;
									case "Transparent":
										txtInDe.Foreground = Brushes.Transparent;
										break;
									default:
										txtInDe.Foreground = Brushes.Black;
										break;
								}
							}

							grd.Children.Add(txtInDe);
							Grid.SetRow(txtInDe, row);
							Grid.SetColumn(txtInDe, 4);
                            #endregion
                            
                        }
					}

                    List<string> codicidone = new List<string>();
                  
                    foreach(DataRow BV in datiBV.Rows)
                    {
                        if (BV["ID"].ToString() != tmpID)
                            continue;
                    
                        string codice = ((BV["codice"].ToString() != "") ? BV["codice"].ToString() : "");

                        bool eaexists = true;
                        bool epexists = true;

                        if (((BV["esercizio"].ToString() != "") ? BV["esercizio"].ToString() : "") == "EA")
                        {
                            if(((BV["valore"].ToString() != "") ? BV["valore"].ToString() : "") == "")
                            {
                                eaexists = false;
                            }
                        }
                        else
                        {
                            foreach (DataRow nodeBVtmp in datiBV.Rows)
                            {
                               if (nodeBVtmp["ID"].ToString() != tmpID)
                                  continue;
                    
                                if (nodeBVtmp["esercizio"].ToString() != "EA")
                                    continue;
                               
                                if (nodeBVtmp["valore"].ToString() == "")
                                {
                                
                                        eaexists = false;
   
                                }
                            }
                        }


                        if (((BV["esercizio"].ToString() != "") ? BV["esercizio"].ToString() : "") == "EP")
                        {
                            if (BV["valore"].ToString() != "") 
                            {
                                epexists = false;
                            }
                        }
                        else
                        {
                            foreach (DataRow nodeBVtmp in datiBV.Rows)
                            {
                                if (nodeBVtmp["ID"].ToString() != tmpID)
                                  continue;
                                if (nodeBVtmp["esercizio"].ToString() != "EP")
                                    continue;
                                if (nodeBVtmp["valore"].ToString() == "")
                                {
                                    epexists = false;
                                }
                            }
                           
                        }

                        if(eaexists == false && epexists == false)
                        {
                            continue;
                        }

                        
                        if(codicidone.Contains(codice))
                        {
                            continue;
                        }
                        else
                        {
                            codicidone.Add(codice);
                        }

                        row++;

                        rowBV.Add(row, tmpID);

                        rd = new RowDefinition();
                        
                        grd.RowDefinitions.Add(rd);
                        
                        TextBlock txtBV = new TextBlock();
                        txtBV.TextWrapping = TextWrapping.Wrap;
                        txtBV.Margin = new Thickness(30, 0, 0, 0);
                        txtBV.Foreground = Brushes.DarkGreen;
                        txtBV.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                        txtBV.Text = codice + "-" + ((BV["titolo"].ToString() != "")? BV["titolo"].ToString() : "");

                        grd.Children.Add(txtBV);
                        Grid.SetRow(txtBV, row);
                        Grid.SetColumn(txtBV, 1);

                        TextBox txtEABV = new TextBox();
                        txtEABV.AllowDrop = false;
                        txtEABV.TextAlignment = TextAlignment.Right;
                        txtEABV.Foreground = Brushes.DarkGreen;
                        txtEABV.Width = Convert.ToDouble(WidthColonne);
                        if (((BV["esercizio"].ToString() != "") ? BV["esercizio"].ToString() : "") == "EA")
                        {
                            txtEABV.Text = ((BV["valore"].ToString() != "") ? BV["valore"].ToString() : "");
                        }
                        else
                        {
                            foreach (DataRow nodeBVtmp2 in datiBV.Rows)
                            {
                             if (nodeBVtmp2["ID"].ToString() != tmpID)
                                  continue;
                                if ((nodeBVtmp2["esercizio"].ToString() == "EA")&& (nodeBVtmp2["codice"].ToString() == codice))
                                  
                                if (nodeBVtmp2["valore"].ToString() != "")
                                {
                                        txtEABV.Text = nodeBVtmp2["valore"].ToString();
                                 }
                            }
                           
                        }
                        txtEABV.IsEnabled = false;
                        txtEABV.IsTabStop = false;

                        grd.Children.Add(txtEABV);
                        Grid.SetRow(txtEABV, row);
                        Grid.SetColumn(txtEABV, 2);

                        TextBox txtEPBV = new TextBox();
                        txtEPBV.AllowDrop = false;
                        txtEPBV.TextAlignment = TextAlignment.Right;
                        txtEPBV.Foreground = Brushes.DarkGreen;
                        txtEPBV.Width = Convert.ToDouble(WidthColonne);
                        if (((BV["esercizio"].ToString() != "") ? BV["esercizio"].ToString() : "") == "EP")
                        {
                            txtEPBV.Text = ((BV["valore"].ToString() != "") ? BV["valore"].ToString() : "");
                        }
                        else
                        {
                          
                            foreach (DataRow nodeBVtmp2 in datiBV.Rows)
                            {
                                 if (nodeBVtmp2["ID"].ToString() != tmpID)
                                      continue;
                                if ((nodeBVtmp2["esercizio"].ToString() == "EP") && (nodeBVtmp2["codice"].ToString() == codice))


                                if (nodeBVtmp2["valore"].ToString() != "")
                                    {
                                        txtEPBV.Text = nodeBVtmp2["valore"].ToString();
                                    }
                            }
                        }
                        txtEPBV.IsEnabled = false;
                        txtEPBV.IsTabStop = false;

                        grd.Children.Add(txtEPBV);
                        Grid.SetRow(txtEPBV, row);
                        Grid.SetColumn(txtEPBV, 3);

                        TextBox txtDiffBV = new TextBox();
                        txtDiffBV.TextAlignment = TextAlignment.Right;
                        txtDiffBV.Foreground = Brushes.DarkGreen;
                        txtDiffBV.AllowDrop = false;
                        txtDiffBV.Width = Convert.ToDouble(WidthColonne);
                        double valueea = 0.0;
                        double valueep = 0.0;
                        double valuediff = 0.0;

                        double.TryParse(txtEABV.Text, out valueea);
                        double.TryParse(txtEPBV.Text, out valueep);
                        valuediff = valueea - valueep;

                        txtDiffBV.Text = ConvertNumber(valuediff.ToString());
                        txtDiffBV.IsEnabled = false;
                        txtDiffBV.IsTabStop = false;

                        grd.Children.Add(txtDiffBV);
                        Grid.SetRow(txtDiffBV, row);
                        Grid.SetColumn(txtDiffBV, 4);
                    }
                    
                    row++;
				}				

				brd2.Child = grd;

				stpMain.Children.Add(brd2);
            

            foreach (object chidrenhere in grd.Children)
                {
                    if(chidrenhere.GetType().Name == "TextBox")
                    {
                        string namehere = ((TextBox)chidrenhere).Name;

                        if(namehere.IndexOf("txtEP_") >= 0)
                        {
                            namehere = namehere.Replace("txtEP_", "");
                            UpdateCalculation(((TextBox)chidrenhere), namehere);
                            tmpnamehere = namehere;
                            tmpchild = chidrenhere;
                        }
                    }
                    
                }

            }
            
            UpdateCalculation(((TextBox)tmpchild), tmpnamehere);

            alreadyShowed = true;

            UpdateOpenedClosed();
            
        }

        private void TxtTitoloEA_LostFocus(object sender, RoutedEventArgs e)
        {  
            foreach (DataRow dtrow in datiTestata.Rows)
            {
                dtrow["TitoloEA"] = ((TextBox)sender).Text;
            }
        }

        private void TxtTitoloEP_LostFocus(object sender, RoutedEventArgs e)
        {
            foreach (DataRow dtrow in datiTestata.Rows)
            {
                dtrow["TitoloEP"] = ((TextBox)sender).Text;
            }

        }        

        private void UpdateOpenedClosed()
        {
            foreach (DictionaryEntry item in rowBV)
            {
                foreach (DataRow dtrow in dati.Rows)
                {
                    if (dtrow["ID"].ToString() == item.Value.ToString())
                    {
                        if (dtrow["opened"].ToString() == "0")
                        {
                            ((Grid)(((Border)(stpMain.Children[0])).Child)).RowDefinitions[(int)(item.Key)].Height = new GridLength(0);
                        }
                        else
                        {
                            ((Grid)(((Border)(stpMain.Children[0])).Child)).RowDefinitions[(int)(item.Key)].Height = new GridLength(20);
                        }
                    }
                }
            }
        }
        
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image i = ((Image)sender);
            string idtmp = i.Name.Replace("btn_Expand_", "");
            double height = 0.0;
            string openedvalue = "1";

            if (i.Tag.ToString() == "opened")
            {
                i.Tag = "closed";
                var uriSource = new Uri(down, UriKind.Relative);
                i.Source = new BitmapImage(uriSource);
                height = 0.0;
                openedvalue = "0";
            }
            else
            {
                i.Tag = "opened";
                var uriSource = new Uri(left, UriKind.Relative);
                i.Source = new BitmapImage(uriSource);
                height = 20.0;
                openedvalue = "1";
            }
            
            foreach (DictionaryEntry item in rowBV)
            {
                if(item.Value.ToString() == idtmp)
                {
                    ((Grid)(((Border)(stpMain.Children[0])).Child)).RowDefinitions[(int)(item.Key)].Height = new GridLength(height);
                }                
            }

         
            foreach (DataRow dtrow in dati.Rows)
            {
                if(dtrow["ID"].ToString()== idtmp)
                   dtrow["opened"] = openedvalue;
            }

        }

        private string ConvertNumber(string valore,string ris="")
		{
			double dblValore = 0.0;
      if (valore == "-")
        return valore;
			double.TryParse(valore.Replace(".",""), out dblValore);

			if (dblValore == 0.0)
			{
                if (ris != "")
                    return ris;
                else
                    return "";
			}
			else
			{
				return String.Format("{0:#,0}", dblValore);
			}
		}

		private void UpdateCalculation(TextBox sender, string ID)
		{
            

            //ucNodoMultiploVerticale parent = TryFindParent<ucNodoMultiploVerticale>(sender.Parent);

            //var windows = System.Windows.Window.GetWindow(this);
            //ucNodoMultiploVerticale parentWindow = UIHelper.TryFindParent<ucNodoMultiploVerticale>((uc_Excel_Bilancio)(_control_parent));


            IEnumerable childrens = ((Grid)(sender.Parent)).Children;

            if (_control_parent != null)
            {
                DependencyObject converted_parent = _control_parent as DependencyObject;
                if(converted_parent != null)
                { 
                    childrens = FindVisualChildren<TextBox>(converted_parent);
                }
            }

            //foreach (UIElement item in ((Grid)(sender.Parent)).Children)
            foreach (UIElement item in childrens)
            {
                

                if (item.GetType().Name == "TextBox")
				{
					TextBox txt = ((TextBox)item);
                   

                    string ID_NOW = txt.Name.Split('_').Last();
                  
                    if (txt.Name == "txtInDe_" + ID)
					{
						double ep = 0;
						double ea = 0;

						double.TryParse(b_valoreEP[ID].ToString(), out ep);
						double.TryParse(b_valoreEA[ID].ToString(), out ea);

						double tmptotvalue = ea - ep;
						if (ea != 0.0 || ep != 0.0)
						{
							tmptotvalue += 0.001;
						}

						txt.Text = ConvertNumber((tmptotvalue).ToString());

						if (b_valoreDIFF.Contains(ID))
						{
							b_valoreDIFF[ID] = txt.Text;
						}
						else
						{
							b_valoreDIFF.Add(ID, txt.Text);
						}
					}
					else if (ht_somme.Contains(ID_NOW))
					{
                      
                        string[] arrayS = ht_somme[ID_NOW].ToString().Split('|');
						if (arrayS.Contains(ID) || arrayS.Contains("-" + ID))
						{
							if (txt.Name == "txtEP_" + ID_NOW)
							{
                               
                                double epst = 0;
								double eps = 0;
								
								bool atleastonenotzero = false;

								foreach (string IDS in arrayS)
								{

									if (!b_valoreEP.Contains(IDS.Replace("-", "")))
									{
                                        bool idstrovato = false;
								        foreach (DataRow dtrow in datiALL.Rows)
                                        {
                                         

                                           if(dtrow["ID"].ToString() == IDS.Replace("-", ""))
                                           {
                                                if (dtrow["EP"].ToString() != "")
                                                {
                                                    b_valoreEP.Add(IDS.Replace("-", ""), dtrow["EP"].ToString());
                                                }
                                                else
                                                {
                                                    b_valoreEP.Add(IDS.Replace("-", ""), "0.0");
                                                }
                                                idstrovato = true;
                                            }
                                        }
                                        if (!idstrovato)
										{
											b_valoreEP.Add(IDS.Replace("-", ""), "0.0");
										}
									}

									double.TryParse(b_valoreEP[IDS.Replace("-", "")].ToString(), out eps);

									if (eps != 0.0)
									{
										atleastonenotzero = true;

										if (Convert.ToInt32(IDS) > 0)
										{
											epst += eps;
										}
										else
										{
											epst -= eps;
										}
									}
								}

								if (atleastonenotzero)
								{
									epst += 0.001;
								}

								txt.Text = ConvertNumber((epst).ToString());

								if (b_valoreEP.Contains(ID_NOW))
								{
                                   
                                    b_valoreEP[ID_NOW] = txt.Text;
								}
								else
								{
									b_valoreEP.Add(ID_NOW, txt.Text);
								}
							}

							if (txt.Name == "txtEA_" + ID_NOW)
							{
								double east = 0;
								double eas = 0;
                            
                                bool atleastonenotzero = false;

								foreach (string IDS in arrayS)
								{
									if (!b_valoreEA.Contains(IDS.Replace("-", "")))
									{
                                        bool idstrovato = false;
                                        foreach (DataRow dtrow in datiALL.Rows)
                                        {
                                            if (dtrow["ID"].ToString() == IDS.Replace("-", ""))
                                            {
                                                if (dtrow["EA"].ToString() != "")
                                                {
                                                    b_valoreEA.Add(IDS.Replace("-", ""), dtrow["EA"].ToString());
                                                }
                                                else
                                                {
                                                    b_valoreEA.Add(IDS.Replace("-", ""), "0.0");
                                                }
                                                idstrovato = true;
                                            }
                                        }
                                        if (!idstrovato)
                                        {
                                            b_valoreEA.Add(IDS.Replace("-", ""), "0.0");
                                        }
                                     
									}

									double.TryParse(b_valoreEA[IDS.Replace("-", "")].ToString(), out eas);

									if (eas != 0.0)
									{
										atleastonenotzero = true;

										if (Convert.ToInt32(IDS) > 0)
										{
											east += eas;
										}
										else
										{
											east -= eas;
										}
									}
								}

								if (atleastonenotzero)
								{
									east += 0.001;
								}

								txt.Text = ConvertNumber((east).ToString());

								if (b_valoreEA.Contains(ID_NOW))
								{
									b_valoreEA[ID_NOW] = txt.Text;
								}
								else
								{
									b_valoreEA.Add(ID_NOW, txt.Text);
								}
							}

							if (txt.Name == "txtInDe_" + ID_NOW)
							{
								double epst = 0;
								double eps = 0;
								double east = 0;
								double eas = 0;

								foreach (string IDS in arrayS)
								{
									double.TryParse(b_valoreEP[IDS.Replace("-", "")].ToString(), out eps);

									if (Convert.ToInt32(IDS) > 0)
									{
										epst += eps;
									}
									else
									{
										epst -= eps;
									}
									
									double.TryParse(b_valoreEA[IDS.Replace("-", "")].ToString(), out eas);

									if (Convert.ToInt32(IDS) > 0)
									{
										east += eas;
									}
									else
									{
										east -= eas;
									}
								}

								double tmptotvalue = east - epst;
								if (east != 0.0 || epst != 0.0)
								{
									tmptotvalue += 0.001;
								}

								txt.Text = ConvertNumber((tmptotvalue).ToString());

								if (b_valoreDIFF.Contains(ID_NOW))
								{
									b_valoreDIFF[ID_NOW] = txt.Text;
								}
								else
								{
									b_valoreDIFF.Add(ID_NOW, txt.Text);
								}
							}
						}
					}
                    if (txt.Name == "txtEA_102")
                    {
                        if (txt.Text != "")
                            if (!cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("102"))
                                cBusinessObjects.ht_diff_tra_uc_bilancio.Add("102", txt.Text);
                            else
                                cBusinessObjects.ht_diff_tra_uc_bilancio["102"] = txt.Text;
                    }
                    if (txt.Name == "txtEA_180")
                    {
                        if (txt.Text != "" )
                            if (!cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("180"))
                                cBusinessObjects.ht_diff_tra_uc_bilancio.Add("180", txt.Text);
                            else
                                cBusinessObjects.ht_diff_tra_uc_bilancio["180"] = txt.Text;
                    }
                   

                    if (txt.Name == "txtEP_102")
                    {
                       if (txt.Text != ""  )
                            if (!cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("102EP"))
                                cBusinessObjects.ht_diff_tra_uc_bilancio.Add("102EP", txt.Text);
                            else
                                cBusinessObjects.ht_diff_tra_uc_bilancio["102EP"] = txt.Text;
                    }
                    if (txt.Name == "txtEP_180")
                    {
                        if (txt.Text != ""  )
                            if (!cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("180EP"))
                                cBusinessObjects.ht_diff_tra_uc_bilancio.Add("180EP", txt.Text);
                            else
                                cBusinessObjects.ht_diff_tra_uc_bilancio["180EP"] = txt.Text;
                    }
                  
                    if (txt.Name == "txtInDe_102")
                    {
                        if (txt.Text != ""  )
                            if (!cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("102InDe"))
                                cBusinessObjects.ht_diff_tra_uc_bilancio.Add("102InDe", txt.Text);
                            else
                                cBusinessObjects.ht_diff_tra_uc_bilancio["102InDe"] = txt.Text;
                    }
                    if (txt.Name == "txtInDe_180")
                    {
                        if (txt.Text != "" )
                            if (!cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("180InDe"))
                                cBusinessObjects.ht_diff_tra_uc_bilancio.Add("180InDe", txt.Text);
                            else
                                cBusinessObjects.ht_diff_tra_uc_bilancio["180InDe"] = txt.Text;
                    }
                
                    if (txt.Name == "txtEP_11611")
                    {
                        if (txt.Text != "")
                            if (!cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("11611EP"))
                                cBusinessObjects.ht_diff_tra_uc_bilancio.Add("11611EP", txt.Text);
                            else
                                cBusinessObjects.ht_diff_tra_uc_bilancio["11611EP"] = txt.Text;
                    }
                    if (txt.Name == "txtEA_11611")
                    {
                        if (txt.Text != "")
                            if (!cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("11611"))
                                cBusinessObjects.ht_diff_tra_uc_bilancio.Add("11611", txt.Text);
                            else
                                cBusinessObjects.ht_diff_tra_uc_bilancio["11611"] = txt.Text;
                    }
                    if (txt.Name == "txtInDe_11611")
                    {
                        if (txt.Text != "")
                            if (!cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("11611InDe"))
                                cBusinessObjects.ht_diff_tra_uc_bilancio.Add("11611InDe", txt.Text);
                            else
                                cBusinessObjects.ht_diff_tra_uc_bilancio["11611InDe"] = txt.Text;
                    }
                    if (txt.Name == "txtEP_271")
                    {
                        if (txt.Text != "")
                            if (!cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("271EP"))
                                cBusinessObjects.ht_diff_tra_uc_bilancio.Add("271EP", txt.Text);
                            else
                                cBusinessObjects.ht_diff_tra_uc_bilancio["271EP"] = txt.Text;
                    }
                    if (txt.Name == "txtEA_271")
                    {
                        if (txt.Text != "")
                            if (!cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("271"))
                                cBusinessObjects.ht_diff_tra_uc_bilancio.Add("271", txt.Text);
                            else
                                cBusinessObjects.ht_diff_tra_uc_bilancio["271"] = txt.Text;
                    }
                    if (txt.Name == "txtInDe_271")
                    {
                        if (txt.Text != "")
                            if (!cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("271InDe"))
                                cBusinessObjects.ht_diff_tra_uc_bilancio.Add("271InDe", txt.Text);
                            else
                                cBusinessObjects.ht_diff_tra_uc_bilancio["271InDe"] = txt.Text;
                    }
                   
                    if (txt.Name == "txtInDe_175")
                    {
                        if (txt.Text != "")
                            if (!cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("175InDe"))
                                cBusinessObjects.ht_diff_tra_uc_bilancio.Add("175InDe", txt.Text);
                            else
                                cBusinessObjects.ht_diff_tra_uc_bilancio["175InDe"] = txt.Text;
                    }
                   
                    if (txt.Name == "txtEA_181" || txt.Name == "txtEA_9999999180")
                    {
                        double a = 0;
                        double b = 0;
                        if (cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("102") && cBusinessObjects.ht_diff_tra_uc_bilancio["102"].ToString() != "")
                            double.TryParse(cBusinessObjects.ht_diff_tra_uc_bilancio["102"].ToString(), out a);
                        if (cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("180") && cBusinessObjects.ht_diff_tra_uc_bilancio["180"].ToString() != "")
                            double.TryParse(cBusinessObjects.ht_diff_tra_uc_bilancio["180"].ToString(), out b);
                        a = a - b;
                        txt.Text = ConvertNumber((a).ToString(),"0");
                    }
                    if (txt.Name == "txtEP_181" || txt.Name == "txtEP_9999999180")
                    {
                        double a = 0;
                        double b = 0;
                        if (cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("102EP") && cBusinessObjects.ht_diff_tra_uc_bilancio["102EP"].ToString() != "")
                            double.TryParse(cBusinessObjects.ht_diff_tra_uc_bilancio["102EP"].ToString(), out a);
                        if (cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("180EP") && cBusinessObjects.ht_diff_tra_uc_bilancio["180EP"].ToString() != "")
                            double.TryParse(cBusinessObjects.ht_diff_tra_uc_bilancio["180EP"].ToString(), out b);
                        a = a - b;
                        txt.Text = ConvertNumber((a).ToString(),"0");
                    }
                    if (txt.Name == "txtInDe_181"  || txt.Name == "txtInDe_9999999180")
                    {
                        double a = 0;
                        double b = 0;
                        if (cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("102InDe") && cBusinessObjects.ht_diff_tra_uc_bilancio["102InDe"].ToString() != "")
                            double.TryParse(cBusinessObjects.ht_diff_tra_uc_bilancio["102InDe"].ToString(), out a);
                        if (cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("180InDe") && cBusinessObjects.ht_diff_tra_uc_bilancio["180InDe"].ToString() != "")
                            double.TryParse(cBusinessObjects.ht_diff_tra_uc_bilancio["180InDe"].ToString(), out b);
                        a = a - b;                    
                        txt.Text = ConvertNumber((a).ToString(),"0");
                    }
                    
                    
                    if (txt.Name == "txtEA_99999999180")
                    {
                        double a = 0;
                        double b = 0;
                        if (cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("11611") && cBusinessObjects.ht_diff_tra_uc_bilancio["11611"].ToString() != "")
                            double.TryParse(cBusinessObjects.ht_diff_tra_uc_bilancio["11611"].ToString(), out a);
                        if (cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("271") && cBusinessObjects.ht_diff_tra_uc_bilancio["271"].ToString() != "")
                            double.TryParse(cBusinessObjects.ht_diff_tra_uc_bilancio["271"].ToString(), out b);
                        a = a - b;
                        txt.Text = ConvertNumber((a).ToString(),"0");
                    }
                    if (txt.Name == "txtEP_99999999180" )
                    {
                        double a = 0;
                        double b = 0;
                        if (cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("11611EP") && cBusinessObjects.ht_diff_tra_uc_bilancio["11611EP"].ToString() != "")
                            double.TryParse(cBusinessObjects.ht_diff_tra_uc_bilancio["11611EP"].ToString(), out a);
                        if (cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("271EP") && cBusinessObjects.ht_diff_tra_uc_bilancio["271EP"].ToString() != "")
                            double.TryParse(cBusinessObjects.ht_diff_tra_uc_bilancio["271EP"].ToString(), out b);
                        a = a - b;
                        txt.Text = ConvertNumber((a).ToString(),"0");
                    }
                    
                 
                     if (txt.Name == "txtInDe_99999999180")
                    {
                        double a = 0;
                        double b = 0;
                        if (cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("11611InDe") && cBusinessObjects.ht_diff_tra_uc_bilancio["11611InDe"].ToString() != "")
                            double.TryParse(cBusinessObjects.ht_diff_tra_uc_bilancio["11611InDe"].ToString(), out a);
                        if (cBusinessObjects.ht_diff_tra_uc_bilancio.Contains("271InDe") && cBusinessObjects.ht_diff_tra_uc_bilancio["271InDe"].ToString() != "")
                            double.TryParse(cBusinessObjects.ht_diff_tra_uc_bilancio["271InDe"].ToString(), out b);
                        a = a - b;
                        txt.Text = ConvertNumber((a).ToString(),"0");
                    }
                   
                }

			}

          
		}

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        public static T TryFindParent<T>(DependencyObject current) where T : class
        {
            DependencyObject parent = VisualTreeHelper.GetParent(current);
            if (parent == null)
                parent = LogicalTreeHelper.GetParent(current);
            if (parent == null)
                return null;

            if (parent is T)
                return parent as T;
            else
                return TryFindParent<T>(parent);
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
            
            TextBox txt = ((TextBox)sender);

            if (txt.Name.Contains("_9999999180") || txt.Name.Contains("_99999999180") || txt.Name.Contains("_181"))
                return;

            string ID = txt.Name.Split('_').Last();

            //if (ht_negativi.Contains(ID))
            //{
            //    double tmpvalue = 0.0;

            //    double.TryParse(txt.Text, out tmpvalue);

            //    if (tmpvalue > 0.0)
            //    {
            //        txt.Text = "-" + txt.Text;
            //    }
            //}

            switch (txt.Name.Split('_').First())
            {
                case "txtEP":
                    b_valoreEP[ID] = txt.Text;
                    break;
                case "txtEA":
                    b_valoreEA[ID] = txt.Text;
                    break;
                default:
                    return;
            }
          
            txt.Text = ConvertNumber(txt.Text);
            txt.CaretIndex = txt.Text.Length;
            UpdateCalculation(txt, ID);
            //if (!alreadyShowed)
            //{
            //    return;
            //}

            //TextBox txt = ((TextBox)sender);

            //string ID = txt.Name.Split('_').Last();

            //if (ht_negativi.Contains(ID))
            //{
            //    try 
            //    {
            //        if (Convert.ToDouble(txt.Text) > 0)
            //        {
            //            txt.Text = "-" + txt.Text;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        string log = ex.Message;
            //    }				
            //}
            //switch (txt.Name.Split('_').First())
            //{
            //    case "txtEP":
            //        ht_ep[ID] = txt.Text;
            //        break;
            //    case "txtEA":
            //        ht_ea[ID] = txt.Text;
            //        break;
            //    default:
            //        return;
            //}

            //UpdateCalculation(txt, ID);
        }

		private void obj_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (_readonly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}
		}

		private void obj_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (_readonly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}
		}

		private void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{

            TextBox txt = ((TextBox)sender);

			string ID = txt.Name.Split('_').Last();

			//if (ht_negativi.Contains(ID))
			//{
			//    double tmpvalue = 0.0;

			//    double.TryParse(txt.Text, out tmpvalue);

			//    if (tmpvalue > 0.0)
			//    {
			//        txt.Text = "-" + txt.Text;
			//    }
			//}

			switch (txt.Name.Split('_').First())
			{
				case "txtEP":
					b_valoreEP[ID] = txt.Text;
					break;
				case "txtEA":
					b_valoreEA[ID] = txt.Text;
					break;
				default:
					return;
			}

			txt.Text = ConvertNumber(txt.Text);

			UpdateCalculation(txt, ID);
		}

	
        public int Save()
        {
        
            if (IDB_Padre == "166" || IDB_Padre == "172")
            {
                // TODO MM
                //   nodehere.Attributes["TitoloEA"].Value = tmpNode.Attributes["TitoloEA"].Value;
                //  nodehere.Attributes["TitoloEP"].Value = tmpNode.Attributes["TitoloEP"].Value;
            }
            string filet = "";
            foreach (DataRow dtrow in dati.Rows)
            {
                filet = dtrow["template"].ToString();
                string tmpID = dtrow["ID"].ToString();
                if (!b_valoreDIFF.Contains(tmpID))
                {
                    continue;
                }
                if (b_valoreEA.Contains(tmpID))
                {
                    if (b_valoreEA[tmpID].ToString() != "")
                        dtrow["EA"] = double.Parse(b_valoreEA[tmpID].ToString()); 
                }

                if (b_valoreDIFF.Contains(tmpID))
                {
                    dtrow["DIFF"] = b_valoreDIFF[tmpID].ToString();
                }

                // if (_x_AP == null || (_x_AP != null && _x_AP.Document.SelectSingleNode("/Dati//Dato[@ID='" + _ID + "']/Valore[@ID='" + tmpID + "']") == null))
                {
                    if (b_valoreEP.Contains(tmpID))
                    {
                        if(b_valoreEP[tmpID].ToString() != "")
                          dtrow["EP"] = double.Parse(b_valoreEP[tmpID].ToString());
                    }
                }
            }

            cBusinessObjects.SaveData(id,datiTestata, typeof(Excel_Bilancio_Testata));
            cBusinessObjects.Executesql("DELETE FROM Excel_Bilancio WHERE ID_SCHEDA="+cBusinessObjects.GetIDTree(id).ToString()+" AND ID_CLIENTE="+cBusinessObjects.idcliente.ToString() + " AND ID_SESSIONE="+cBusinessObjects.idsessione.ToString() + " AND template='" + filet + "'");
            return cBusinessObjects.SaveData(id, dati, typeof(Excel_Bilancio));

        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{



			//if (e.PreviousSize.Width == 0.0)
			//{
			//    return;
			//}

			//double percent = e.NewSize.Width / e.PreviousSize.Width;

			//foreach (UIElement item in ((Grid)(((Border)(stpMain.Children[0])).Child)).Children)
			//{
			//    if (item.GetType().Name == "TextBlock")
			//    {
			//        TextBlock txt = ((TextBlock)item);

			//        txt.Width = txt.Width * percent;
			//    }

			//    if (item.GetType().Name == "Border")
			//    {
			//        Border txt = ((Border)item);

			//        txt.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
			//        txt.Width = txt.Width * percent;

			//        ((TextBlock)(txt.Child)).Width = txt.Width - 3.0;
			//        ((TextBlock)(txt.Child)).Width = ((TextBlock)(txt.Child)).Width * percent;
			//    }
			//}
		}

        private void btnEspandiTuttoBV_Click(object sender, RoutedEventArgs e)
        {
            Button i = ((Button)sender);

            double height = 0.0;
            string openedvalue = "0";

            if (i.Tag.ToString() == "opened")
            {
                i.Content = "Espandi Tutte le voci del Bilancio di Verifica";
                i.Tag = "closed";
                height = 0.0;
                openedvalue = "0";
            }
            else
            {
                i.Content = "Chiudi Tutte le voci del Bilancio di Verifica";
                i.Tag = "opened";
                height = 20.0;
                openedvalue = "1";
            }

            foreach (DictionaryEntry item in rowBV)
            {
                if (this.FindName("btn_Expand_" + item.Value.ToString()) != null)
                {
                    Image img = (Image)(this.FindName("btn_Expand_" + item.Value.ToString()));

                    if (openedvalue == "0")
                    {
                        img.Tag = "closed";
                        var uriSource = new Uri(down, UriKind.Relative);
                        img.Source = new BitmapImage(uriSource);
                    }
                    else
                    {
                        img.Tag = "opened";
                        var uriSource = new Uri(left, UriKind.Relative);
                        img.Source = new BitmapImage(uriSource);
                    }
                }

                ((Grid)(((Border)(stpMain.Children[0])).Child)).RowDefinitions[(int)(item.Key)].Height = new GridLength(height);
            }
            foreach (DataRow dtrow in datiTestata.Rows)
            {
                dtrow["opened"] = openedvalue;
            }

        
        }
    }
}
