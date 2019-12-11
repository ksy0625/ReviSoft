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
	public partial class ucLead : UserControl
    {
        public int id;
        private DataTable dati = null;
        private string IDB_Padre = "227";

		public string Titolo = "";

		private int WidthColonne = 100;
		private int WidthColonneRef = 50;

		private string titolo_Altro = "Ulteriori dati opzionali (acquisizione non automatica)";
		private Brush SfondoTitoli = Brushes.WhiteSmoke;

        private XmlDataProviderManager _x = null;
		private XmlDataProviderManager _x_AP = null;
		private XmlDataProviderManager _y = null;
        private string _ID = "";

		Hashtable ht_negativi = new Hashtable();
		Hashtable ht_somme = new Hashtable();
		
		Hashtable b_valoreEA = new Hashtable();
		Hashtable b_valoreEP = new Hashtable();
		Hashtable b_valoreDIFF = new Hashtable();

		Hashtable b_NoData = new Hashtable();
		Hashtable b_Titolo = new Hashtable();
		ArrayList b_Ordine = new ArrayList();

		private bool _readonly = false;

		private string Codice = "";

		int maxid = 1000;

		public bool ReadOnly
		{
			set
			{
				_readonly = value;
			}
		}

		public ucLead()
        {
            
            InitializeComponent();
            try
            {
                FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
               new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            }
            catch (Exception e)
            {

            }
        }

		private void RetrieveData(XmlDataProviderManager _x, XmlDataProviderManager x_AP)
		{
            DataTable dati_bilancio = cBusinessObjects.GetData(int.Parse(IDB_Padre), typeof(Excel_Bilancio));
            foreach (DataRow node in dati_bilancio.Rows)
     		{
				//Calcolo valori attuali

				if (node["EA"].ToString() !="")
				{
                    if ( !b_valoreEA.Contains( node["ID"].ToString() ) )
                    {
                        b_valoreEA.Add( node["ID"].ToString(), node["EA"].ToString() );
                    }
                    else
                    {
                        b_valoreEA[node["ID"].ToString()] = node["EA"].ToString();
                    }
				}
				else
				{
                    if ( !b_valoreEA.Contains( node["ID"].ToString() ) )
                    {
                        b_valoreEA.Add( node["ID"].ToString(), "0");
                    }
                    else
                    {
                        b_valoreEA[node["ID"].ToString()] = "0";
                    }
				}
                bool trovato = false;
                foreach (DataRow node2 in dati_bilancio.Rows)
                {
                   if(node2 ["ID"].ToString()== node["ID"].ToString())
                      trovato = true;
                }

                if (x_AP == null|| (x_AP!=null && !trovato))
                    //MM
                    //|| (x_AP != null && x_AP.Document.SelectSingleNode("/Dati//Dato[@ID='" + IDB_Padre + "']/Valore[@ID='" + node["ID"].ToString() + "']") == null))
				{
					if (node["EP"].ToString() != "")
                    {
                        if ( !b_valoreEP.Contains( node["ID"].ToString() ) )
                        {
                            b_valoreEP.Add( node["ID"].ToString(), node["EP"].ToString() );
                        }
                        else
                        {
                            b_valoreEP[node["ID"].ToString()] = node["EP"].ToString();
                        }
					}
					else
					{
                        if ( !b_valoreEP.Contains( node["ID"].ToString() ) )
                        {
                            b_valoreEP.Add( node["ID"].ToString(), "0" );
                        }
                        else
                        {
                            b_valoreEP[node["ID"].ToString()] = "0";
                        }
					}
				}

				//Calcolo valori anno precedente
				if (x_AP != null)
				{
                    DataRow tmpNode = null;
                    foreach (DataRow node2 in dati_bilancio.Rows)
                    {
                        if (node2["ID"].ToString() == node["ID"].ToString())
                            tmpNode = node2;
                    }
                   	if (tmpNode != null)
					{
						if (tmpNode["EA"].ToString() != "")
						{
                            if ( !b_valoreEP.Contains( node["ID"].ToString() ) )
                            {
                                b_valoreEP.Add( node["ID"].ToString(), tmpNode["EA"].ToString() );
                            }
                            else
                            {
                                b_valoreEP[node["ID"].ToString()] = tmpNode["EA"].ToString();
                            }
						}
						else
						{
                            if ( !b_valoreEP.Contains( node["ID"].ToString() ) )
                            {
                                b_valoreEP.Add( node["ID"].ToString(), "0" );
                            }
                            else
                            {
                                b_valoreEP[node["ID"].ToString()] = "0";
                            }
						}
					}
					else
					{
                        if ( !b_valoreEP.Contains( node["ID"].ToString() ) )
                        {
                            b_valoreEP.Add( node["ID"].ToString(), "0" );
                        }
					}
                  
				}

				//if (node["DIFF"] != null)
				//{
				//    b_valoreDIFF.Add(node["ID"].ToString(), node["DIFF"].ToString());
				//}
				//else
				{
					double ep = 0.0;
					double ea = 0.0;

					double.TryParse(b_valoreEA[node["ID"].ToString()].ToString(), out ea);
					double.TryParse(b_valoreEP[node["ID"].ToString()].ToString(), out ep);

					double tmptotvalue = ea - ep;
					if (ea != 0.0 || ep != 0.0)
					{
						tmptotvalue += 0.001;
					}

					b_valoreDIFF.Add(node["ID"].ToString(), ConvertNumber((tmptotvalue).ToString()));
				}
			}
		}

        public void LoadDataSource(string ID, string IDCliente, string IDSessione, XmlDataProviderManager x_AP, string IDTree)
        {

            id = int.Parse(ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());

            dati = cBusinessObjects.GetData(id, typeof(Leads));

            _ID = ID;
			_x_AP = x_AP;
            
            string tipoBilancio = "";

         
            DataTable datiTestata = cBusinessObjects.GetData(int.Parse(IDB_Padre), typeof(Excel_Bilancio_Testata));
            foreach (DataRow dtrow in datiTestata.Rows)
            {
                if (dtrow["tipoBilancio"].ToString() != "")
                    tipoBilancio = dtrow["tipoBilancio"].ToString();
               
            }

            if (IDB_Padre == "227")
            {
                switch (tipoBilancio)
                {
                    case "2016":
                        _y = new XmlDataProviderManager(App.AppLEAD2016, true);
                        break;
                    default:
                        _y = new XmlDataProviderManager(App.AppLEAD, true);
                        break;
                }
            }
            else
            {
                switch (tipoBilancio)
                {
                    case "Micro":
                    case "2016":
                        _y = new XmlDataProviderManager(App.AppLEAD2016, true);
                        break;
                    default:
                        _y = new XmlDataProviderManager(App.AppLEAD, true);
                        break;
                }
            }
            
			XmlDataProviderManager template_x = null;

			switch ((App.TipoFile)(System.Convert.ToInt32(IDTree)))
			{
				case App.TipoFile.Revisione:
					template_x = new XmlDataProviderManager(App.AppTemplateTreeRevisione);
					break;
				case App.TipoFile.Verifica:
					template_x = new XmlDataProviderManager(App.AppTemplateTreeVerifica);
					break;
				case App.TipoFile.Incarico:
                case App.TipoFile.IncaricoCS:
                case App.TipoFile.IncaricoSU:
                case App.TipoFile.IncaricoREV:
					template_x = new XmlDataProviderManager(App.AppTemplateTreeIncarico);
					break;
                case App.TipoFile.ISQC:
                    template_x = new XmlDataProviderManager(App.AppTemplateTreeISQC);
                    break;
                case App.TipoFile.Bilancio:
					template_x = new XmlDataProviderManager(App.AppTemplateTreeBilancio);
					break;
                case App.TipoFile.Vigilanza:
                    template_x = new XmlDataProviderManager( App.AppTemplateTreeVigilanza );
                    break;
				case App.TipoFile.Licenza:
				case App.TipoFile.Master:
				case App.TipoFile.Info:
				case App.TipoFile.Messagi:
				case App.TipoFile.ImportExport:
				case App.TipoFile.ImportTemplate:
				case App.TipoFile.BackUp:
				case App.TipoFile.Formulario:
				case App.TipoFile.ModellPredefiniti:
				case App.TipoFile.DocumentiAssociati:
				default:
					break;
			}

			XmlNode xmlTMP = template_x.Document.SelectSingleNode("//Tree//Node[@ID='" + ID + "']");

            if ( xmlTMP != null )
            {
                xmlTMP = xmlTMP.ParentNode;

                if ( xmlTMP.Attributes["Codice"] != null )
                {
                    Codice = xmlTMP.Attributes["Codice"].Value.Trim();
                }
            }

			GenerateAll(false);

            UpdateFromBilancio();
        }

		void GenerateAll(bool WithData)
		{
            string sAppTemplateBilancio_Attivo = "";
            string sAppTemplateBilancio_Passivo = "";
            string sAppTemplateBilancio_ContoEconomico = "";

            string tipoBilancio = "";


            DataTable datiTestata = cBusinessObjects.GetData(int.Parse(IDB_Padre), typeof(Excel_Bilancio_Testata));
            foreach (DataRow dtrow in datiTestata.Rows)
            {
                if (dtrow["tipoBilancio"].ToString() != "")
                    tipoBilancio = dtrow["tipoBilancio"].ToString();

            }

            if (IDB_Padre == "227")
            {
                switch (tipoBilancio)
                {
                    case "2016":
                        _y = new XmlDataProviderManager(App.AppLEAD2016, true);
                        sAppTemplateBilancio_Attivo = App.AppTemplateBilancio_Attivo2016;
                        sAppTemplateBilancio_Passivo = App.AppTemplateBilancio_Passivo2016;
                        sAppTemplateBilancio_ContoEconomico = App.AppTemplateBilancio_ContoEconomico2016;
                        break;
                    default:
                        _y = new XmlDataProviderManager(App.AppLEAD, true);
                        sAppTemplateBilancio_Attivo = App.AppTemplateBilancio_Attivo;
                        sAppTemplateBilancio_Passivo = App.AppTemplateBilancio_Passivo;
                        sAppTemplateBilancio_ContoEconomico = App.AppTemplateBilancio_ContoEconomico;
                        break;
                }
            }
            else
            {
                switch (tipoBilancio)
                {
                    case "Micro":
                        _y = new XmlDataProviderManager(App.AppLEAD2016, true);
                        sAppTemplateBilancio_Attivo = App.AppTemplateBilancioMicro_Attivo2016;
                        sAppTemplateBilancio_Passivo = App.AppTemplateBilancioMicro_Passivo2016;
                        sAppTemplateBilancio_ContoEconomico = App.AppTemplateBilancioMicro_ContoEconomico2016;
                        break;
                    case "2016":
                        _y = new XmlDataProviderManager(App.AppLEAD2016, true);
                        sAppTemplateBilancio_Attivo = App.AppTemplateBilancioAbbreviato_Attivo2016;
                        sAppTemplateBilancio_Passivo = App.AppTemplateBilancioAbbreviato_Passivo2016;
                        sAppTemplateBilancio_ContoEconomico = App.AppTemplateBilancioAbbreviato_ContoEconomico2016;
                        break;
                    default:
                        _y = new XmlDataProviderManager(App.AppLEAD, true);
                        sAppTemplateBilancio_Attivo = App.AppTemplateBilancioAbbreviato_Attivo;
                        sAppTemplateBilancio_Passivo = App.AppTemplateBilancioAbbreviato_Passivo;
                        sAppTemplateBilancio_ContoEconomico = App.AppTemplateBilancioAbbreviato_ContoEconomico;
                        break;
                }
            }
            //PATRIMONIALE ATTIVO
            bool check345 = false;

            string file = sAppTemplateBilancio_Attivo;

			GenerateData(file, Codice, "PATRIMONIALE ATTIVO", WithData);

            Grid grd = GenerateGrid("PATRIMONIALE ATTIVO");

            if (grd.Children.Count > 6)
            {
                stpMain.Children.Add(grd);
                check345 = true;
            }

            //PATRIMONIALE PASSIVO
            file = sAppTemplateBilancio_Passivo;

			GenerateData(file, Codice, "PATRIMONIALE PASSIVO", WithData);

			grd = GenerateGrid("PATRIMONIALE PASSIVO");

            if (grd.Children.Count > 6)
            {
                if (stpMain.Children.Count > 0)
                {
                    grd.Margin = new Thickness(0, 15, 0, 0);
                }

                stpMain.Children.Add(grd);
            }

            //CONTO ECONOMICO
            file = sAppTemplateBilancio_ContoEconomico;

			GenerateData(file, Codice, "CONTO ECONOMICO", WithData);

            if (Codice == "3.4.5" && check345 == false)
            {
                ;
            }
            else
            { 
                grd = GenerateGrid("CONTO ECONOMICO");

                if (grd.Children.Count > 6)
                {
                    if (stpMain.Children.Count > 0)
                    {
                        grd.Margin = new Thickness(0, 15, 0, 0);
                    }

                    stpMain.Children.Add(grd);
                }
            }		

			//N RIGHE
			b_Titolo.Clear();
			b_NoData.Clear();
			b_Ordine.Clear();

            
            foreach (DataRow dtrow in dati.Rows)
            {

                if (dtrow["Tipo"].ToString() != titolo_Altro)
                {
                    continue;
                }

                if (Convert.ToInt32(dtrow["ID"].ToString()) > maxid)
                  maxid = Convert.ToInt32(dtrow["ID"].ToString());
				b_Ordine.Add(dtrow["ID"].ToString());
                if (!b_NoData.Contains(dtrow["ID"].ToString())) // ebdebug
				          b_NoData.Add(dtrow["ID"].ToString(), false);
                if (!b_Titolo.Contains(dtrow["ID"].ToString())) // ebdebug
                  b_Titolo.Add(dtrow["ID"].ToString(), dtrow["Titolo"].ToString());
			}

			if (b_Ordine.Count == 0)
			{
				AddNewNodeAltro();

				b_Ordine.Add(maxid.ToString());
				b_NoData.Add(maxid.ToString(), false);
				b_Titolo.Add(maxid.ToString(), "");
			}

			grd = GenerateGrid(titolo_Altro);

			if (stpMain.Children.Count > 0)
			{
				grd.Margin = new Thickness(0, 15, 0, 0);
			}

			stpMain.Children.Add(grd);

			StackPanel stpnew = new StackPanel();
			stpnew.Orientation = Orientation.Horizontal;

			Button btn = new Button();
			btn.ToolTip = "Aggiungi Riga";
			btn.Content = "Aggiungi Riga";
			btn.Margin = new Thickness(7.0);
			btn.Click += new RoutedEventHandler(btn_ClickAdd);

			stpnew.Children.Add(btn);

			btn = new Button();
			btn.ToolTip = "Elimina Ultima Riga";
			btn.Content = "Elimina Ultima Riga";
			btn.Margin = new Thickness(7.0);
			btn.Click += new RoutedEventHandler(btn_ClickDelete);

			stpnew.Children.Add(btn);

			stpMain.Children.Add(stpnew);
		}

		void AddNewNodeAltro()
		{
            DataRow dd= dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            dd["Tipo"] = titolo_Altro.Replace("&", "&amp;").Replace("\"", "'");
            dd["ID"] = maxid;

           
		}

		void UpdateAltroGrid()
		{
			//Aggiorno tabella
			b_Titolo.Clear();
			b_NoData.Clear();
			b_Ordine.Clear();
            foreach (DataRow dtrow in dati.Rows)
            {
                if(dtrow["Tipo"].ToString()!= titolo_Altro)
                {
                    continue;
                }

				if (Convert.ToInt32(dtrow["ID"].ToString()) > maxid)
				{
					maxid = Convert.ToInt32(dtrow["ID"].ToString());
				}

				b_Ordine.Add(dtrow["ID"].ToString());
				b_NoData.Add(dtrow["ID"].ToString(), false);
				b_Titolo.Add(dtrow["ID"].ToString(), dtrow["Titolo"].ToString());

			}

			Grid grd = GenerateGrid(titolo_Altro);

			if (stpMain.Children.Count > 2)
			{
				grd.Margin = new Thickness(0, 15, 0, 0);
			}

			stpMain.Children.RemoveAt(stpMain.Children.Count - 2);
			stpMain.Children.Insert(stpMain.Children.Count - 1, grd);

		}

		void btn_ClickDelete(object sender, RoutedEventArgs e)
		{
			if (_readonly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}
            for (int i = dati.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dtrow = dati.Rows[i];
                if (dtrow["Tipo"].ToString() == titolo_Altro)
                    dtrow.Delete();
            }


            dati.AcceptChanges();
            bool trovato = false;
            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["Tipo"].ToString() != titolo_Altro)
                {
                    continue;
                }
                trovato = true;
            }

            if (!trovato)
			{
				AddNewNodeAltro();
			}

			UpdateAltroGrid();
		}

		void btn_ClickAdd(object sender, RoutedEventArgs e)
		{
			if (_readonly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}

			maxid++;

			AddNewNodeAltro();

			UpdateAltroGrid();
		}

		public void UpdateFromBilancio()
		{
            //if ( _readonly )
            //{
            //    //MessageBox.Show(  App.MessaggioSolaScrittura, "Attenzione" );
            //    return;
            //}

			//if (MessageBox.Show("Vuoi importare dal bilancio i dati per la carta Lead?\r\nAttenzione: eventuali dati presenti saranno sovrascritti!", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				b_valoreEA.Clear();
				b_valoreEP.Clear();
				b_valoreDIFF.Clear();

				RetrieveData(_x, _x_AP);

				if (b_valoreEA.Count == 0)
				{
					//MessageBox.Show("Bilancio Ordinario Assente. I dati del bilancio abbreviato sono aggregati in un numero insufficiente di voci per essere indirizzati nelle specifiche aree di controllo.");
					return;
				}
                //MM
                /*	XmlNode item = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]/Valore[@Tipo!=\"" + titolo_Altro + "\"]");

                while (item != null)
				{
					item.ParentNode.RemoveChild(item);
					item = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]/Valore[@Tipo!=\"" + titolo_Altro + "\"]");
				}
                */
				stpMain.Children.Clear();

				GenerateAll(true);
			}
		}

		private void GenerateData(string file, string Codice, string Titolo, bool WithData)
		{
            string itemTipo = "";
            string itemID = "";
         
            string itemEA = "";
            string itemEP = "";
            string itemincdec = "";
            string itemsomma = "";
            string itemTitolo = "";



            XmlDataProviderManager _yBilancio = new XmlDataProviderManager(file, true);

			b_Titolo.Clear();
			b_NoData.Clear();
			b_Ordine.Clear();

            foreach (XmlNode item in _y.Document.SelectNodes("/LEADS/LEAD[@ID='" + Codice + "']/RIGA"))
            {
       //         XmlNode itemBilancio = _yBilancio.Document.SelectSigleNodeNodes("/Dato/MacroGruppo[@Codice='" + item + "']")
       //         foreach (XmlNode itemBilancio in _yBilancio.Document.SelectNodes("/Dato/MacroGruppo"))
			    //{
				//foreach (XmlNode item in _y.Document.SelectNodes("/LEADS/LEAD[@ID='" + Codice + "']/RIGA"))
				//{
                if(b_Ordine.Contains(item.Attributes["ID"].Value))
                {
                    continue;
                }

					XmlNode nodobilancio = _yBilancio.Document.SelectSingleNode("/Dato/MacroGruppo/Bilancio[@ID='" + item.Attributes["ID"].Value + "']");
                    if ( nodobilancio != null || (item.Attributes["TIPO"] != null && item.Attributes["TIPO"].Value == Titolo))
					{
                        itemTipo = Titolo;
                        itemID = item.Attributes["ID"].Value;


                        b_Ordine.Add(item.Attributes["ID"].Value);

                        if ( nodobilancio != null && (nodobilancio.Attributes["noData"] != null || nodobilancio.Attributes["rigaVuota"] != null) )
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
                                string epvalue = "";
                                string diffvalue = "";

                                string additeivesomma = "";

                                if(item.Attributes["SOMMA"] != null)
                                {
                                    double tdeavalue = 0;
                                    double tdepvalue = 0;
                                    double tddiffvalue = 0;

                                    foreach ( string itemSOMMA in item.Attributes["SOMMA"].Value.Split('|') )
                                    {
                                        double deavalue = 0;
                                        double depvalue = 0;
                                        double ddiffvalue = 0;

                                        double.TryParse(( b_valoreEA.Contains( itemSOMMA ) ? b_valoreEA[itemSOMMA].ToString() : "0" ), out deavalue);
                                        double.TryParse(( b_valoreEP.Contains( itemSOMMA ) ? b_valoreEP[itemSOMMA].ToString() : "0" ), out depvalue);
                                        double.TryParse(( b_valoreDIFF.Contains( itemSOMMA ) ? b_valoreDIFF[itemSOMMA].ToString() : "0" ), out ddiffvalue);
                                        
                                        tdeavalue += deavalue;
                                        tdepvalue += depvalue;
                                        tddiffvalue += ddiffvalue;
                                    }

                                    eavalue = ConvertNumber(tdeavalue.ToString());
                                    epvalue = ConvertNumber( tdepvalue.ToString() );
                                    diffvalue = ConvertNumber( tddiffvalue.ToString() );

                                    additeivesomma =  item.Attributes["SOMMA"].Value;
                                }
                                else
                                {
                                    eavalue = ( b_valoreEA.Contains( item.Attributes["ID"].Value ) ? b_valoreEA[item.Attributes["ID"].Value].ToString() : "0" );
                                    epvalue = ( b_valoreEP.Contains( item.Attributes["ID"].Value ) ? b_valoreEP[item.Attributes["ID"].Value].ToString() : "0" );
                                    diffvalue = ( b_valoreDIFF.Contains( item.Attributes["ID"].Value ) ? b_valoreDIFF[item.Attributes["ID"].Value].ToString() : "0" );
                                }
                       
                             itemEA = eavalue;
                             itemEP = epvalue;
                             itemincdec = diffvalue;
                             itemsomma = additeivesomma;

                         	}

							b_NoData.Add(item.Attributes["ID"].Value, false);
						}

						string titolo = "";

                        if ( nodobilancio != null && nodobilancio.Attributes["Codice"] != null )
						{
							titolo += nodobilancio.Attributes["Codice"].Value + " ";
						}

                        if ( nodobilancio != null && nodobilancio.Attributes["name"] != null )
						{
							titolo += nodobilancio.Attributes["name"].Value;
						}

                        titolo = ( ( item.Attributes["TITOLO"] != null ) ? item.Attributes["TITOLO"].Value : titolo );

                        itemTitolo = titolo;

                       
						b_Titolo.Add(item.Attributes["ID"].Value, titolo);
                        DataRow dd = null;
                        foreach (DataRow dtrow in dati.Rows)
                        {
                        if (dtrow["ID"].ToString() == itemID)
                            dd = dtrow;

                        }
                        if(dd==null)
                        {
                            dd=dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
                           
                        }
                        dd["ID"] = itemID;
                        dd["Titolo"] = itemTitolo;
                        dd["Tipo"] = itemTipo;
                        dd["somma"] = itemsomma;
                        dd["EA"] = itemEA;
                        dd["EP"] = itemEP;
                        dd["incdec"] = itemincdec;


                }
				//}
			}
		}

		private Grid GenerateGrid(string Titolo)
		{
			Grid grd = new Grid();

			if (b_Titolo.Count > 0)
			{				
				ColumnDefinition cd = new ColumnDefinition();
				cd.Width = new GridLength(1, GridUnitType.Star);
				cd.MinWidth = 150.0;
				grd.ColumnDefinitions.Add(cd);
				cd = new ColumnDefinition();
				cd.Width = new GridLength(WidthColonneRef + 10);
				grd.ColumnDefinitions.Add(cd);
				cd = new ColumnDefinition();
				cd.Width = new GridLength(WidthColonne + 10);
				grd.ColumnDefinitions.Add(cd);
				cd = new ColumnDefinition();
				cd.Width = new GridLength(WidthColonneRef + 10);
				grd.ColumnDefinitions.Add(cd);
				cd = new ColumnDefinition();
				cd.Width = new GridLength(WidthColonne + 10);
				grd.ColumnDefinitions.Add(cd);
				cd = new ColumnDefinition();
				cd.Width = new GridLength(+WidthColonneRef + WidthColonne + 10);
				grd.ColumnDefinitions.Add(cd);

				RowDefinition rd = new RowDefinition();

				//rd.Height = new GridLength(20);

				grd.RowDefinitions.Add(rd);

				Border brd = new Border();
				brd.BorderThickness = new Thickness(1.0, 1.0, 1.0, 1.0);
				brd.Background = SfondoTitoli;

				TextBlock lbl = new TextBlock();
				lbl.Text = Titolo;
				lbl.FontWeight = FontWeights.Bold;
				brd.Child = lbl;

				grd.Children.Add(brd);
				Grid.SetRow(brd, 0);
				Grid.SetColumn(brd, 0);

				TextBox txt = new TextBox();
				txt.BorderThickness = new Thickness(0.0);
				txt.IsReadOnly = true;
				txt.IsTabStop = false;
				grd.Children.Add(txt);
				Grid.SetRow(txt, 0);
				Grid.SetColumn(txt, 1);

				brd = new Border();
				brd.BorderThickness = new Thickness(1.0, 1.0, 1.0, 1.0);
				brd.Background = SfondoTitoli;

				lbl = new TextBlock();
				lbl.Text = "ESERCIZIO ATTUALE";
				lbl.TextWrapping = TextWrapping.Wrap;
				lbl.FontWeight = FontWeights.Bold;
				lbl.TextAlignment = TextAlignment.Center;

				brd.Child = lbl;

				grd.Children.Add(brd);
				Grid.SetRow(brd, 0);
				Grid.SetColumn(brd, 2);

				txt = new TextBox();
				txt.BorderThickness = new Thickness(0.0);
				txt.IsReadOnly = true;
				txt.IsTabStop = false;
				grd.Children.Add(txt);
				Grid.SetRow(txt, 0);
				Grid.SetColumn(txt, 3);

				brd = new Border();
				brd.BorderThickness = new Thickness(1.0, 1.0, 1.0, 1.0);
				brd.Background = SfondoTitoli;

				lbl = new TextBlock();
				lbl.Text = "ESERCIZIO PRECEDENTE";
				lbl.TextWrapping = TextWrapping.Wrap;
				lbl.FontWeight = FontWeights.Bold;
				lbl.TextAlignment = TextAlignment.Center;

				brd.Child = lbl;

				grd.Children.Add(brd);
				Grid.SetRow(brd, 0);
				Grid.SetColumn(brd, 4);

				brd = new Border();
				brd.BorderThickness = new Thickness(1.0, 1.0, 1.0, 1.0);
				brd.Margin = new Thickness(WidthColonneRef, 0, 0, 0);
				brd.Background = SfondoTitoli;

				lbl = new TextBlock();
				lbl.Text = "incremento decremento";
				lbl.TextWrapping = TextWrapping.Wrap;
				lbl.FontWeight = FontWeights.Bold;
				lbl.TextAlignment = TextAlignment.Center;

				brd.Child = lbl;

				grd.Children.Add(brd);
				Grid.SetRow(brd, 0);
				Grid.SetColumn(brd, 5);

				int row = 0;

                bool total = false;

				foreach (string item in b_Ordine)
				{
                    DataRow tmprow = null;
                    foreach (DataRow dtrow in dati.Rows)
                    {
                        if (dtrow["ID"].ToString() == item)
                            tmprow = dtrow;

                    }
                    DataRow tmpRowChild = null;

                    bool hasdata = false;

                    if ((bool)(b_NoData[item]) == true)
                    {
                        foreach (XmlNode child in _y.Document.SelectNodes("/LEADS/LEAD[@ID='" + Codice + "']/RIGA[@PADRE='" + item + "']"))
                        {
                            if ((bool)(b_NoData[child.Attributes["ID"].Value]) == true)
                            {
                                foreach (XmlNode grandchild in _y.Document.SelectNodes("/LEADS/LEAD[@ID='" + Codice + "']/RIGA[@PADRE='" + child.Attributes["ID"].Value + "']"))
                                {
                                    if ((bool)(b_NoData[grandchild.Attributes["ID"].Value]) == true)
                                    {
                                        foreach (XmlNode grandgrandchild in _y.Document.SelectNodes("/LEADS/LEAD[@ID='" + Codice + "']/RIGA[@PADRE='" + grandchild.Attributes["ID"].Value + "']"))
                                        {
                                            if ((bool)(b_NoData[grandgrandchild.Attributes["ID"].Value]) == true)
                                            {
                                                
                                            }
                                            else
                                            {
                                                foreach (DataRow dtrow in dati.Rows)
                                                {
                                                    if (dtrow["ID"].ToString() == grandchild.Attributes["ID"].Value)
                                                        tmpRowChild = dtrow;

                                                }
                                                if (tmpRowChild != null && tmpRowChild["EA"].ToString() != "" && tmpRowChild["EP"].ToString() != null && ((tmpRowChild["EA"].ToString() != "") && tmpRowChild["EA"].ToString() != "") || (tmpRowChild["EP"].ToString() != "" && tmpRowChild["EP"].ToString() != "0"))
                                           {
                                                    hasdata = true;
                                                    total = true;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (DataRow dtrow in dati.Rows)
                                        {
                                            if (dtrow["ID"].ToString() == grandchild.Attributes["ID"].Value)
                                                tmpRowChild = dtrow;

                                        }
                                        if (tmpRowChild != null && tmpRowChild["EA"].ToString() != "" && tmpRowChild["EP"].ToString() != null && ((tmpRowChild["EA"].ToString() != "" )&& tmpRowChild["EA"].ToString() != "") || (tmpRowChild["EP"].ToString() != "" && tmpRowChild["EP"].ToString() != "0"))
                                        {
                                            hasdata = true;
                                            total = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (DataRow dtrow in dati.Rows)
                                {
                                    if (dtrow["ID"].ToString() == child.Attributes["ID"].Value)
                                        tmpRowChild = dtrow;

                                }
                                if (tmpRowChild != null && tmpRowChild["EA"].ToString() != "" && tmpRowChild["EP"].ToString() != "" && ((tmpRowChild["EA"].ToString() != "") && tmpRowChild["EA"].ToString() != "") || (tmpRowChild["EP"].ToString() != "" && tmpRowChild["EP"].ToString() != "0"))      
                                {
                                    hasdata = true;
                                    total = true;
                                }
                                
                            }
                        } 
                    }
                    else
                    {
                        if (tmprow != null && ((tmprow["Tipo"].ToString() == "" || tmprow["Tipo"].ToString() == "Ulteriori dati opzionali (acquisizione non automatica)") || (tmprow["EA"].ToString() != "" && tmprow["EP"].ToString() != "" && ((tmprow["EA"].ToString() != "" && tmprow["EA"].ToString() != "0") || (tmprow["EP"].ToString() != "" && tmprow["EP"].ToString() != "0")))))
                        {
                            hasdata = true;
                            total = true;
                        }
                    }
                   
                    if (hasdata == false || (b_Titolo[item].ToString() == "Totale" && total == false))
                    {
                        continue;
                    }

                    if(b_Titolo[item].ToString() == "Totale" && total == true)
                    {
                        total = false;
                    }
                    
                    if ((bool)(b_NoData[item]) == true)
                    {
                        //Riga vuota
                        rd = new RowDefinition();
                        rd.Height = new GridLength(20);
                        grd.RowDefinitions.Add(rd);

                        row++;
                    }

                    if (b_Titolo[item].ToString() == "Totale" || b_Titolo[item].ToString() == "Sub Totale")
                    {
                        rd = new RowDefinition();
                        rd.Height = new GridLength(20);
                        grd.RowDefinitions.Add(rd);

                        row++;
                    }

                    rd = new RowDefinition();
					grd.RowDefinitions.Add(rd);
					
					row++;

                    bool bold = false;
                    
					if (Titolo == titolo_Altro)
					{
						txt = new TextBox();
						//txt.Margin = new Thickness(0, 5, 0, 0);
						txt.Name = "txtTitolo_" + item;
						txt.LostFocus += new RoutedEventHandler(TextBox_LostFocus);
						txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
						txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);

						if (tmprow != null && tmprow["Titolo"].ToString() != "")
						{
							txt.Text = tmprow["Titolo"].ToString();
						}

						grd.Children.Add(txt);
						Grid.SetRow(txt, row);
						Grid.SetColumn(txt, 0);
					}
					else
					{
						lbl = new TextBlock();
						lbl.Text = b_Titolo[item].ToString();
                        if ( lbl.Text == "Totale" || lbl.Text == "Sub Totale")
                        {
                            bold = true;
                        }

                        if ( bold == true)
                        {
                            lbl.FontWeight = FontWeights.Bold;
                        }

						lbl.TextWrapping = TextWrapping.Wrap;
						if ((bool)(b_NoData[item]) == true)
						{
							lbl.FontWeight = FontWeights.Bold;
						}

						grd.Children.Add(lbl);
						Grid.SetRow(lbl, row);
						Grid.SetColumn(lbl, 0);
					}

					if ((bool)(b_NoData[item]) == false)
					{
						txt = new TextBox();
                        if ( bold == true )
                        {
                            txt.FontWeight = FontWeights.Bold;
                        }
						txt.BorderThickness = new Thickness(0.0);
						//txt.Margin = new Thickness(0, 5, 0, 0);
						txt.Name = "txtrefEA_" + item;
						txt.IsReadOnly = true;
						if (tmprow != null && tmprow["refEA"].ToString() != "")
						{
							txt.Text = tmprow["refEA"].ToString();
						}
						txt.IsTabStop = false;
						txt.LostFocus += new RoutedEventHandler(TextBox_LostFocus);
						txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
						txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
						grd.Children.Add(txt);
						Grid.SetRow(txt, row);
						Grid.SetColumn(txt, 1);

						txt = new TextBox();
                        if ( bold == true )
                        {
                            txt.FontWeight = FontWeights.Bold;
                        }
						//txt.Margin = new Thickness(0, 5, 0, 0);
						txt.Name = "txtEA_" + item;
                        if (tmprow != null && tmprow["EA"].ToString() != "")
         				{
							txt.Text = ConvertNumber(tmprow["EA"].ToString());
						}
						txt.TextAlignment = TextAlignment.Right;
						txt.LostFocus += new RoutedEventHandler(TextBox_LostFocus);
						txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
						txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);

						if (this.FindName(txt.Name) != null)
						{
							this.UnregisterName(txt.Name);
						}
						this.RegisterName(txt.Name, txt);

						grd.Children.Add(txt);
						Grid.SetRow(txt, row);
						Grid.SetColumn(txt, 2);

						txt = new TextBox();
                        if ( bold == true )
                        {
                            txt.FontWeight = FontWeights.Bold;
                        }
						txt.BorderThickness = new Thickness(0.0);
						//txt.Margin = new Thickness(0, 5, 0, 0);
						txt.Name = "txtrefEP_" + item;
						txt.IsReadOnly = true;
                        if (tmprow != null && tmprow["refEP"].ToString() != "")
                 		{
							txt.Text = tmprow["refEP"].ToString();
						}
						txt.IsTabStop = false;
						txt.LostFocus += new RoutedEventHandler(TextBox_LostFocus);
						txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
						txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
						grd.Children.Add(txt);
						Grid.SetRow(txt, row);
						Grid.SetColumn(txt, 3);

						txt = new TextBox();
                        if ( bold == true )
                        {
                            txt.FontWeight = FontWeights.Bold;
                        }
						//txt.Margin = new Thickness(0, 5, 0, 0);
						txt.Name = "txtEP_" + item;
                        if (tmprow != null && tmprow["EP"].ToString() != "")
                        {
						txt.Text = ConvertNumber(tmprow["EP"].ToString());
						}
						txt.TextAlignment = TextAlignment.Right;
						txt.LostFocus += new RoutedEventHandler(TextBox_LostFocus);
						txt.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
						txt.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);

						if (this.FindName(txt.Name) != null)
						{
							this.UnregisterName(txt.Name);
						}
						this.RegisterName(txt.Name, txt);

						grd.Children.Add(txt);
						Grid.SetRow(txt, row);
						Grid.SetColumn(txt, 4);

						txt = new TextBox();
                        if ( bold == true )
                        {
                            txt.FontWeight = FontWeights.Bold;
                        }
						//txt.Margin = new Thickness(WidthColonneRef, 5, 0, 0);
						txt.Margin = new Thickness(WidthColonneRef, 0, 0, 0);
						txt.Name = "txtincdec_" + item;
                        if (tmprow != null && tmprow["incdec"].ToString() != "")
						{
							txt.Text = ConvertNumber(tmprow["incdec"].ToString());
						}
						txt.TextAlignment = TextAlignment.Right;
						txt.IsReadOnly = true;
						txt.IsTabStop = false;

						if (this.FindName(txt.Name) != null)
						{
							this.UnregisterName(txt.Name);
						}
						this.RegisterName(txt.Name, txt);

						grd.Children.Add(txt);
						Grid.SetRow(txt, row);
						Grid.SetColumn(txt, 5);
					}
				}
			}

			return grd;
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

		public int Save()
		{

            cBusinessObjects.SaveData(id, dati, typeof(Leads));
			return 0;
		}

		private void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			TextBox txt = ((TextBox)sender);

			string strID = txt.Name.Split('_').Last();

			string strAttribute = txt.Name.Split('_').First().Replace("txt", "");

			if (strAttribute == "EP" || strAttribute == "EA")
			{
				txt.Text = ConvertNumber(txt.Text);
			}

		    DataRow tmpRow = null;
            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["ID"].ToString() == strID)
                    tmpRow = dtrow;

            }
            if (tmpRow != null )
			{
                tmpRow[strAttribute] = txt.Text;
			}

			if (strAttribute == "EP" || strAttribute == "EA")
			{
				TextBox txtEA = (TextBox)this.FindName("txtEA_" + strID);
				TextBox txtEP = (TextBox)this.FindName("txtEP_" + strID);
				TextBox txtincdec = (TextBox)this.FindName("txtincdec_" + strID);

				double risultato = 0.0;
				double valEA = 0.0;
				double valEP = 0.0;

				double.TryParse(txtEA.Text, out valEA);
				double.TryParse(txtEP.Text, out valEP);
				risultato = valEA - valEP;

				txtincdec.Text = ConvertNumber(risultato.ToString());

				
                if (tmpRow != null)
                {
                    tmpRow["incdec"] = txt.Text;
                }
            }
            foreach (DataRow dtrow in dati.Rows)
             {
                if(dtrow["somma"].ToString().Split('|').Contains(strID))
                {
                    TextBox txtEASomma = (TextBox)this.FindName( "txtEA_" + dtrow["ID"].ToString());
                    TextBox txtEPSomma = (TextBox)this.FindName( "txtEP_" + dtrow["ID"].ToString());
                    TextBox txtincdecSomma = (TextBox)this.FindName( "txtincdec_" + dtrow["ID"].ToString());

                    double risultatotot = 0.0;
                    double valEAtot = 0.0;
                    double valEPtot = 0.0;

                    foreach ( string idSomma in dtrow["somma"].ToString().Split( '|' ) )
	                {
                        TextBox txtEAtmp = (TextBox)this.FindName( "txtEA_" + idSomma );
                        TextBox txtEPtmp = (TextBox)this.FindName( "txtEP_" + idSomma );

                        if(txtEAtmp == null || txtEPtmp == null)
                        {
                            continue;
                        }

                        double risultato = 0.0;
                        double valEA = 0.0;
                        double valEP = 0.0;

                        double.TryParse( txtEAtmp.Text, out valEA );
                        double.TryParse( txtEPtmp.Text, out valEP );
                        risultato = valEA - valEP;

                        valEAtot += valEA;
                        valEPtot += valEP;
                        risultatotot += risultato;
	                }

                    txtEASomma.Text = ConvertNumber( valEAtot.ToString() );
                    txtEPSomma.Text = ConvertNumber( valEPtot.ToString() );
                    txtincdecSomma.Text = ConvertNumber( risultatotot.ToString() );

                  
                        dtrow["EA"] = txtEASomma.Text;
                 

                        dtrow["EP"] = txtEPSomma.Text;
                 

                        dtrow["incdec"] = txtincdecSomma.Text;
                   

                }
            }

		//	_x.Save();
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			;
		} 
    }
}
