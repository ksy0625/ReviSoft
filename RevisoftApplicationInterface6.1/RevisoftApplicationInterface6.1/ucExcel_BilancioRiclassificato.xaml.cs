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
    public partial class ucExcel_BilancioRiclassificato : UserControl
    {
        public int id;
        private DataTable dati = null;
        private DataTable datiTestata = null;

        private string down = "./Images/icone/navigate_down.png";
        private string left = "./Images/icone/navigate_left.png";

		private string IDB_Padre = "227";
		private string IDBA_Padre = "229";

		string FileXML = App.AppTemplateBilancio_Riclassificato;

		Hashtable valoreEA = new Hashtable();
		Hashtable valoreEP = new Hashtable();

		//XmlDataProviderManager x;
		string ID;
		//XmlNode xnode;
		//XmlNode xnodeValore;

		public ucExcel_BilancioRiclassificato()
        {
            InitializeComponent();            
        }

        private bool _ReadOnly = true;
		private bool _Abbreviato = false;

		public bool Abbreviato
		{
			set
			{
				if (value)
				{
					IDB_Padre = IDBA_Padre;
					FileXML = App.AppTemplateBilancioAbbreviato_Riclassificato;
					_Abbreviato = value;
				}
			}
		}

        public bool ReadOnly 
        {
            set
            {
                _ReadOnly = value;
            }
        }

		public void Load(XmlDataProviderManager x_AP, string _ID, string IDCliente, string IDSessione)
        {
            id = int.Parse(_ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
         
            if (_ID == "325")
            {
                IDB_Padre = "321";
                FileXML = App.AppTemplateBilancio_Riclassificato2016_Consolidato;
            }
            else
            {
                if (_ID == "139")
                {
                    IDB_Padre = "134";
                }

                if (_ID == "170")
                {
                    IDB_Padre = "166";
                }
                if (_ID == "173")
                {
                    IDB_Padre = "172";
                }

                if (_ID == "2016178")
                {
                    IDB_Padre = "2016174";
                    IDBA_Padre = "2016174";
                }

                if (_ID == "2016139")
                {
                    IDB_Padre = "2016134";
                    IDBA_Padre = "2016134";
                }

                if (_ID == "2016190")
                {
                    IDB_Padre = "2016186";
                    IDBA_Padre = "2016186";
                }
                datiTestata = cBusinessObjects.GetData(int.Parse(IDB_Padre), typeof(Excel_Bilancio_Testata));
            
                string tipoBilancio = "";

                foreach (DataRow dtrow in datiTestata.Rows)
                {
                    if (dtrow["tipoBilancio"].ToString() != "")
                        tipoBilancio = dtrow["tipoBilancio"].ToString();
                }
                if (_Abbreviato)
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
                else
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
            }

       
			ID = _ID;
            dati = cBusinessObjects.GetData(id, typeof(Excel_BilancioRiclassificato));
            dati.Clear();

            #region Dati da bilancio

            RetrieveData( x_AP, IDB_Padre);

			#endregion

			XmlDataProviderManager _y = new XmlDataProviderManager(FileXML, true);

			AddTable(_y, "ATTIVO", ref grdATTIVITA, "TOTALE ATTIVITA'");
			AddTable(_y, "PASSIVO", ref grdPASSIVITA, "TOTALE PASSIVITA'");

			AddTable(_y, "CONTO ECONOMICO", ref grdCONTOECONOMICO, "RISULTATO OPERATIVO");

			if (!_Abbreviato)
			{
				AddTable(_y, "SINTESI", ref grdSINTESI, "CAPITALE INVESTITO");
			}
			else
			{
                TabSintesi.Visibility = System.Windows.Visibility.Collapsed;
			}

           cBusinessObjects.SaveData(id,dati, typeof(Excel_BilancioRiclassificato));
        }

		private void AddTable(XmlDataProviderManager _y, string xpath, ref Grid grd, string titoloTotale)
		{
            int row = 0;

			Border brd;
			TextBlock txt;
			RowDefinition rd;
			
			double totEA_final = 0.0;
			double totEP_final = 0.0;

			//double tot_totEA = 0.0;
			//double tot_totEP = 0.0; 
			//double parziale_totEA = 0.0;
			//double parziale_totEP = 0.0;

			Hashtable ht_totEA = new Hashtable();
			Hashtable ht_totEP = new Hashtable();
			//Hashtable ht_rowtotEA = new Hashtable();
			//Hashtable ht_rowtotEP = new Hashtable();

			Hashtable ht_tipo = new Hashtable();

			foreach (XmlNode item in _y.Document.SelectNodes("/Dato/MacroGruppo[@name='" + xpath + "']/Bilancio"))
			{
				if (item.Attributes["name"] == null || item.Attributes["tipo"] == null)
				{
					continue;
				}

                DataRow crow = null;
                foreach (DataRow dtrow in dati.Rows)
                {
                    if(dtrow["row"].ToString()== row.ToString() && dtrow["Titolo"].ToString()== xpath)
                    {
                        crow = dtrow;
                    }
                }
                if(crow==null)
                {
                    crow=dati.Rows.Add(id,cBusinessObjects.idcliente, cBusinessObjects.idsessione);
                }
                crow["row"] = row;
                crow["Titolo"] = xpath;
                crow["tipo"] = item.Attributes["tipo"].Value;

                if ( item.Attributes["tipo"].Value == "spazio" )
                {
                    rd = new RowDefinition();
                    rd.Height = new GridLength( 10.0 );
                    grd.RowDefinitions.Add( rd );
                    row++;
                    
                    ht_tipo.Add( row, item.Attributes["tipo"].Value );
                    continue;
                }
                else if ( item.Attributes["tipo"].Value == "rigarossa" )
                {
                    rd = new RowDefinition();
                    grd.RowDefinitions.Add( rd );
                    rd.Height = new GridLength( 2.0 );
                    row++;
                    
                    ht_tipo.Add( row, item.Attributes["tipo"].Value );

                    brd = new Border();
                    brd.BorderThickness = new Thickness( 1.0 );
                    brd.BorderBrush = Brushes.DarkRed;
                    grd.Children.Add( brd );
                    
                    Grid.SetRow( brd, row );
                    Grid.SetColumn( brd, 0 );
                    Grid.SetColumnSpan( brd, 6 );
                    continue;
                }
                else
                {
                    rd = new RowDefinition();
                    grd.RowDefinitions.Add( rd );
                    row++;

                    ht_tipo.Add( row, item.Attributes["tipo"].Value );
                }


                crow["name"] = item.Attributes["name"].Value;

				txt = new TextBlock();
				txt.Text = item.Attributes["name"].Value;

                if ( item.Attributes["tipo"].Value == "semitotale" )
                {
                    txt.FontWeight = FontWeights.Bold;
                }

				if (item.Attributes["tipo"].Value == "totale")
				{
					txt.FontWeight = FontWeights.Bold;
                    txt.Background = Brushes.LavenderBlush;
				}

                if ( item.Attributes["tipo"].Value == "grantotale" )
                {
                    txt.FontWeight = FontWeights.Bold;
                    txt.Background = Brushes.DarkRed;
                    txt.Foreground = Brushes.White;
                }

				txt.TextAlignment = TextAlignment.Left;
				txt.TextWrapping = TextWrapping.Wrap;
                txt.FontSize = 13;
                
				grd.Children.Add(txt);
				Grid.SetRow(txt, row);
				Grid.SetColumn(txt, 0);

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

				txt = new TextBlock();
				txt.Text = ConvertNumber(totEA.ToString());
				txt.TextAlignment = TextAlignment.Right;
                txt.FontSize = 13;


				crow["EA"] = txt.Text;
                
                if ( item.Attributes["tipo"].Value == "semitotale" )
                {
                    txt.FontWeight = FontWeights.Bold;
                }

                if ( item.Attributes["tipo"].Value == "totale" )
                {
                    txt.FontWeight = FontWeights.Bold;
                    txt.Background = Brushes.LavenderBlush;
                }

                if ( item.Attributes["tipo"].Value == "grantotale" )
                {
                    txt.FontWeight = FontWeights.Bold;
                    txt.Background = Brushes.DarkRed;
                    txt.Foreground = Brushes.White;
                }

                grd.Children.Add( txt );
                Grid.SetRow( txt, row );
                Grid.SetColumn( txt, 1 );

				ht_totEP.Add(row, totEP);

				txt = new TextBlock();
				txt.Text = ConvertNumber(totEP.ToString());
				txt.TextAlignment = TextAlignment.Right;
                txt.FontSize = 13;

				crow["EP"] = txt.Text;

                if ( item.Attributes["tipo"].Value == "semitotale" )
                {
                    txt.FontWeight = FontWeights.Bold;
                }

                if ( item.Attributes["tipo"].Value == "totale" )
                {
                    txt.FontWeight = FontWeights.Bold;
                    txt.Background = Brushes.LavenderBlush;
                }

                if ( item.Attributes["tipo"].Value == "grantotale" )
                {
                    txt.FontWeight = FontWeights.Bold;
                    txt.Background = Brushes.DarkRed;
                    txt.Foreground = Brushes.White;
                }

                grd.Children.Add( txt );
                Grid.SetRow( txt, row );
                Grid.SetColumn( txt, 3 );
                				
				txt = new TextBlock();
				txt.Text = ConvertNumber((totEA - totEP + 0.001).ToString());
				txt.TextAlignment = TextAlignment.Right;
                txt.FontSize = 13;

				crow["DIFF"] = txt.Text;

                if ( item.Attributes["tipo"].Value == "semitotale" )
                {
                    txt.FontWeight = FontWeights.Bold;
                }

                if ( item.Attributes["tipo"].Value == "totale" )
                {
                    txt.FontWeight = FontWeights.Bold;
                    txt.Background = Brushes.LavenderBlush;
                }

                if ( item.Attributes["tipo"].Value == "grantotale" )
                {
                    txt.FontWeight = FontWeights.Bold;
                    txt.Background = Brushes.DarkRed;
                    txt.Foreground = Brushes.White;
                }
                
                grd.Children.Add( txt );
                Grid.SetRow( txt, row );
                Grid.SetColumn( txt, 5 );

				if((item.Attributes["tipo"].Value == "totale" || item.Attributes["tipo"].Value == "grantotale") && item.Attributes["final"] != null)
				{
					totEA_final = totEA;
					totEP_final = totEP;
				}
			}
            dati.AcceptChanges();

			for (int i = 1; i <= row; i++)
			{
				//if (!ht_totEA.Contains(i))
				//{
				//    if (ht_rowtotEA.Contains(i))
				//    {
				//        AddRowWithTot(ref i, ref grd, Convert.ToDouble(ht_rowtotEA[i].ToString()), Convert.ToDouble(ht_rowtotEP[i].ToString()), tot_totEA, tot_totEP, "");
				//        continue;
				//    }
				//    continue;
				//}

				if (!ht_totEA.Contains(i))
				{
					continue;
				}
                DataRow dd = null;
                foreach (DataRow dtrow in dati.Rows)
                {
                    if((dtrow["row"].ToString()==(i-1).ToString())&& (dtrow["Titolo"].ToString() == xpath))
                    dd = dtrow;
                }

    
				txt = new TextBlock();
				txt.Text = ConvertPercent(((totEA_final == 0.0) ? 0.0 : (Convert.ToDouble(ht_totEA[i].ToString()) / totEA_final)).ToString());
				txt.TextAlignment = TextAlignment.Right;
                txt.FontSize = 13;

				if (dd != null)
				{
                    dd["PERCENT_EA"] = txt.Text;
				}

                if ( ht_tipo[i].ToString() == "semitotale" )
                {
                    txt.FontWeight = FontWeights.Bold;
                }

                if ( ht_tipo[i].ToString() == "totale" )
                {
                    txt.FontWeight = FontWeights.Bold;
                    txt.Background = Brushes.LavenderBlush;
                }

                if ( ht_tipo[i].ToString() == "grantotale" )
                {
                    txt.FontWeight = FontWeights.Bold;
                    txt.Background = Brushes.DarkRed;
                    txt.Foreground = Brushes.White;
                }

				grd.Children.Add(txt);
                Grid.SetRow( txt, i );
                Grid.SetColumn( txt, 2 );

				txt = new TextBlock();
				txt.Text = ConvertPercent(((totEP_final == 0.0) ? 0.0 : (Convert.ToDouble(ht_totEP[i].ToString()) / totEP_final)).ToString());
				txt.TextAlignment = TextAlignment.Right;
                txt.FontSize = 13;

				if (dd != null)
				{
                    dd["PERCENT_EP"] = txt.Text;
				}

                if ( ht_tipo[i].ToString() == "semitotale" )
                {
                    txt.FontWeight = FontWeights.Bold;
                }

                if ( ht_tipo[i].ToString() == "totale" )
                {
                    txt.FontWeight = FontWeights.Bold;
                    txt.Background = Brushes.LavenderBlush;
                }

                if ( ht_tipo[i].ToString() == "grantotale" )
                {
                    txt.FontWeight = FontWeights.Bold;
                    txt.Background = Brushes.DarkRed;
                    txt.Foreground = Brushes.White;
                }

                grd.Children.Add( txt );
                Grid.SetRow( txt, i );
                Grid.SetColumn( txt, 4 );
                dati.AcceptChanges();
			}

			//AddRowWithTot(ref row, ref grd, tot_totEA, tot_totEP, tot_totEA, tot_totEP, titoloTotale);
		}

		private void AddRowWithTot(ref int row, ref Grid grd, double totEA, double totEP, double tot_totEA, double tot_totEP, string title)
		{
			Border brd;
			TextBlock txt;
			RowDefinition rd;
			
			rd = new RowDefinition();
			grd.RowDefinitions.Add(rd);
			row++;

			brd = new Border();
			brd.BorderThickness = new Thickness(0.0, 0.0, 1.0, 0.0);

			brd.BorderBrush = Brushes.Black;

			txt = new TextBlock();
			txt.Text = title;
			txt.FontWeight = FontWeights.Bold;
			txt.TextAlignment = TextAlignment.Left;
			txt.TextWrapping = TextWrapping.Wrap;

			brd.Child = txt;

			grd.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 0);

			brd = new Border();
			brd.BorderThickness = new Thickness(0.0, 1.0, 0.0, 1.0);
			brd.BorderBrush = Brushes.Black;

			txt = new TextBlock();
			txt.Text = ConvertNumber(totEA.ToString());
			txt.TextAlignment = TextAlignment.Right;

			brd.Child = txt;

			grd.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 1);

			brd = new Border();
			brd.BorderThickness = new Thickness(0.0, 1.0, 0.0, 1.0);
			brd.BorderBrush = Brushes.Black;

			txt = new TextBlock();
			txt.Text = ConvertPercent(((tot_totEA == 0.0) ? 0.0 : (totEA / tot_totEA)).ToString());
			txt.TextAlignment = TextAlignment.Right;

			brd.Child = txt;

			grd.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 2);

			brd = new Border();
			brd.BorderThickness = new Thickness(0.0, 1.0, 0.0, 1.0);
			brd.BorderBrush = Brushes.Black;

			txt = new TextBlock();
			txt.Text = ConvertNumber(totEP.ToString());
			txt.TextAlignment = TextAlignment.Right;

			brd.Child = txt;

			grd.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 3);

			brd = new Border();
			brd.BorderThickness = new Thickness(0.0, 1.0, 0.0, 1.0);
			brd.BorderBrush = Brushes.Black;

			txt = new TextBlock();
			txt.Text = ConvertPercent(((tot_totEP == 0.0) ? 0.0 : (totEP / tot_totEP)).ToString());
			txt.TextAlignment = TextAlignment.Right;

			brd.Child = txt;

			grd.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 4);

			brd = new Border();
			brd.BorderThickness = new Thickness(0.0, 1.0, 1.0, 1.0);
			brd.BorderBrush = Brushes.Black;

			txt = new TextBlock();
			txt.Text = ConvertNumber((totEA - totEP).ToString());
			txt.TextAlignment = TextAlignment.Right;

			brd.Child = txt;

			grd.Children.Add(brd);
			Grid.SetRow(brd, row);
			Grid.SetColumn(brd, 5);
		}

		private void RetrieveData( XmlDataProviderManager x_AP, string ID)
		{
            DataTable datiBil = cBusinessObjects.GetData(int.Parse(ID), typeof(Excel_Bilancio));

            foreach (DataRow dtrow in datiBil.Rows)
			{
				//Calcolo valori attuali

				if (dtrow["EA"].ToString() != "" && !valoreEA.Contains(dtrow["ID"].ToString()))
				{
					valoreEA.Add(dtrow["ID"].ToString(), dtrow["EA"].ToString());
				}
				else
				{
                    if(!valoreEA.Contains(dtrow["ID"].ToString()))
                        valoreEA.Add(dtrow["ID"].ToString(), "0");
				}

		          if (true)
                    {
					if (dtrow["EP"].ToString() != "" && !valoreEP.Contains(dtrow["ID"].ToString()))
					{
						valoreEP.Add(dtrow["ID"].ToString(), dtrow["EP"].ToString());
					}
					else
					{
                        if(!valoreEP.Contains(dtrow["ID"].ToString()))
					   	valoreEP.Add(dtrow["ID"].ToString(), "0");
					}
				}

                //Calcolo valori anno precedente
                if (dtrow["EP"].ToString() != "" && !valoreEA.Contains(dtrow["ID"].ToString()))
                {
                    valoreEP.Add(dtrow["ID"].ToString(), dtrow["EP"].ToString());
                }
                else
                {
                    if (!valoreEP.Contains(dtrow["ID"].ToString()))
                        valoreEP.Add(dtrow["ID"].ToString(), "0");
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

			stack.Width = Convert.ToDouble(newsize);			
		}

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image i = ((Image)sender);
            Grid u = ((Grid)(((Grid)(i.Parent)).Children[2]));

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
    }
}
