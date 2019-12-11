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
    public partial class ucRischioGlobale : UserControl
    {
        public int id;
        private DataTable dati = null;
        private string check = "./Images/icone/Stato/check2.png";
		private string uncheck = "./Images/icone/Stato/nothing.png";

		private XmlDataProviderManager _x;
        private string _ID = "-1";

        public WindowWorkArea Owner = null;

        Hashtable Sessioni;
        int SessioneNow;
        string IDTree;

		public ucRischioGlobale()
        {
            if (check.Equals("") || uncheck.Equals("")) { }
            InitializeComponent();
        }

        public void Load( string ID, Hashtable _Sessioni, int _SessioneNow, string _IDTree,string IDCliente,string IDSessione )
        {
            id = int.Parse(ID);
           
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
            _ID = ID;

            Sessioni = _Sessioni;
            SessioneNow = _SessioneNow;
            IDTree = _IDTree;

            //((XmlNode)(((WindowWorkArea)(this.Owner)).Nodes[((WindowWorkArea)(this.Owner)).NodeNow])).OwnerDocument


            dati = cBusinessObjects.GetData(id, typeof(RischioGlobale));
            DataRow datirow = null;
            if(dati.Rows.Count==0)
            {
                dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            }

            foreach (DataRow dtrow in dati.Rows)
            {
                datirow = dtrow;
            }


            AltoMedioBasso valore = AltoMedioBasso.Sconosciuto;
            
         //   XmlNode node = _x.Document.SelectSingleNode("/Dati//Dato[@ID='201']");
            DataTable nodeTable = cBusinessObjects.GetData(201, typeof(clsAltoMedioBasso));
            DataRow node = null;
            foreach (DataRow dtrow in nodeTable.Rows)
            {
                node = dtrow;
            }

            if (node != null && node["value"].ToString() != "")
            {
                try
                {
                    if(((XmlNode)(((WindowWorkArea)(this.Owner)).Nodes[((WindowWorkArea)(this.Owner)).NodeNow])).OwnerDocument.SelectSingleNode("Tree//Node[@ID=70]").SelectSingleNode("Sessioni/Sessione[@Selected='#AA82BDE4']").Attributes["Stato"].Value.ToString() == "2")
                    {
                        valore = ((AltoMedioBasso)(Convert.ToInt32(node["value"].ToString())));
                    }
                }
                catch (Exception ex)
                {
                    string log = ex.Message;
                }
                
            }

            nodeTable = cBusinessObjects.GetData(256, typeof(clsAltoMedioBasso));
            node = null;
            foreach (DataRow dtrow in nodeTable.Rows)
            {
                node = dtrow;
            }

        //    node = _x.Document.SelectSingleNode("/Dati//Dato[@ID='256']");

            if (node != null && node["value"].ToString() != "")
            {
                if (((XmlNode)(((WindowWorkArea)(this.Owner)).Nodes[((WindowWorkArea)(this.Owner)).NodeNow])).OwnerDocument.SelectSingleNode("Tree//Node[@ID=254]").SelectSingleNode("Sessioni/Sessione[@Selected='#AA82BDE4']").Attributes["Stato"].Value.ToString() == "2")
                {
                    valore = ((AltoMedioBasso)(Convert.ToInt32(node["value"].ToString())));
                }                
            }

          
            datirow["txt1"] = valore.ToString();
         
           
            
            valore = AltoMedioBasso.Sconosciuto;
            nodeTable = cBusinessObjects.GetData(205, typeof(clsAltoMedioBasso));
            node = null;
            foreach (DataRow dtrow in nodeTable.Rows)
            {
                node = dtrow;
            }

           //node = _x.Document.SelectSingleNode("/Dati//Dato[@ID='205']");

            if (node != null && node["value"].ToString() != "")
            {
                valore = ((AltoMedioBasso)(Convert.ToInt32(node["value"].ToString())));
            }

            if ((datirow != null) && (datirow["txt2"].ToString() == ""))
            {
                datirow["txt2"] = valore.ToString();
            }


            valore = AltoMedioBasso.Sconosciuto;

            //  node = _x.Document.SelectSingleNode("/Dati//Dato[@ID='217']");
            nodeTable = cBusinessObjects.GetData(217, typeof(clsAltoMedioBasso));
            node = null;
            foreach (DataRow dtrow in nodeTable.Rows)
            {
                node = dtrow;
            }

            if (node != null && node["value"].ToString() != "")
            {
                valore = ((AltoMedioBasso)(Convert.ToInt32(node["value"].ToString())));
            }


            datirow["txt3"] = valore.ToString();

         
            valore = AltoMedioBasso.Sconosciuto;

           // node = _x.Document.SelectSingleNode("/Dati//Dato[@ID='218']");
            nodeTable = cBusinessObjects.GetData(218, typeof(clsAltoMedioBasso));
            node = null;
            foreach (DataRow dtrow in nodeTable.Rows)
            {
                node = dtrow;
            }

            if (node != null && node["value"].ToString() != "")
            {
                valore = ((AltoMedioBasso)(Convert.ToInt32(node["value"].ToString())));
            }

            if ((datirow != null) && (datirow["txt4"].ToString() == ""))
            {
                datirow["txt4"] = valore.ToString();
            }


            valore = AltoMedioBasso.Sconosciuto;

        //    node = _x.Document.SelectSingleNode("/Dati//Dato[@ID='219']");

            nodeTable = cBusinessObjects.GetData(219, typeof(clsAltoMedioBasso));
            node = null;
            foreach (DataRow dtrow in nodeTable.Rows)
            {
                node = dtrow;
            }

            if (node != null && node["value"].ToString() != "")
            {
                valore = ((AltoMedioBasso)(Convert.ToInt32(node["value"].ToString())));
            }
           
             datirow["txt5"] = valore.ToString();


            valore = AltoMedioBasso.Sconosciuto;

          //  node = _x.Document.SelectSingleNode("/Dati//Dato[@ID='220']");
            nodeTable = cBusinessObjects.GetData(220, typeof(clsAltoMedioBasso));
            node = null;
            foreach (DataRow dtrow in nodeTable.Rows)
            {
                node = dtrow;
            }
            if (node != null && node["value"].ToString() != "")
            {
                valore = ((AltoMedioBasso)(Convert.ToInt32(node["value"].ToString())));
            }


            datirow["txt6"] = valore.ToString();



            string risultato = AltoMedioBasso.Sconosciuto.ToString();

            if (datirow != null && datirow["txt1"].ToString() != AltoMedioBasso.Sconosciuto.ToString() && datirow["txt2"].ToString() != AltoMedioBasso.Sconosciuto.ToString())
            {
                if (datirow["txt1"].ToString() == AltoMedioBasso.Alto.ToString())
                {
                    if (datirow["txt2"].ToString() == AltoMedioBasso.Alto.ToString())
                    {
                        risultato = "Molto Basso";
                    }
                    else if (datirow["txt2"].ToString() == AltoMedioBasso.Medio.ToString())
                    {
                        risultato = "Basso";
                    }
                    else if (datirow["txt2"].ToString() == AltoMedioBasso.Basso.ToString())
                    {
                        risultato = "Medio";
                    }
                }
                else if (datirow["txt1"].ToString() == AltoMedioBasso.Medio.ToString())
                {
                    if (datirow["txt2"].ToString() == AltoMedioBasso.Alto.ToString())
                    {
                        risultato = "Basso";
                    }
                    else if (datirow["txt2"].ToString() == AltoMedioBasso.Medio.ToString())
                    {
                        risultato = "Medio";
                    }
                    else if (datirow["txt2"].ToString() == AltoMedioBasso.Basso.ToString())
                    {
                        risultato = "Alto";
                    }
                }
                else if (datirow["txt1"].ToString() == AltoMedioBasso.Basso.ToString())
                {
                    if (datirow["txt2"].ToString() == AltoMedioBasso.Alto.ToString())
                    {
                        risultato = "Medio";
                    }
                    else if (datirow["txt2"].ToString() == AltoMedioBasso.Medio.ToString())
                    {
                        risultato = "Alto";
                    }
                    else if (datirow["txt2"].ToString() == AltoMedioBasso.Basso.ToString())
                    {
                        risultato = "Molto Alto";
                    }
                }
            }

            if (datirow != null)
            {
                datirow["txt2c"] = risultato;
            }
          

            risultato = AltoMedioBasso.Sconosciuto.ToString();

            if (datirow != null && datirow["txt1"].ToString() != AltoMedioBasso.Sconosciuto.ToString() && datirow["txt3"].ToString() != AltoMedioBasso.Sconosciuto.ToString())
            {
                if (datirow["txt1"].ToString() == AltoMedioBasso.Alto.ToString())
                {
                    if (datirow["txt3"].ToString() == AltoMedioBasso.Alto.ToString())
                    {
                        risultato = "Molto Basso";
                    }
                    else if (datirow["txt3"].ToString() == AltoMedioBasso.Medio.ToString())
                    {
                        risultato = "Basso";
                    }
                    else if (datirow["txt3"].ToString() == AltoMedioBasso.Basso.ToString())
                    {
                        risultato = "Medio";
                    }
                }
                else if (datirow["txt1"].ToString() == AltoMedioBasso.Medio.ToString())
                {
                    if (datirow["txt3"].ToString() == AltoMedioBasso.Alto.ToString())
                    {
                        risultato = "Basso";
                    }
                    else if (datirow["txt3"].ToString() == AltoMedioBasso.Medio.ToString())
                    {
                        risultato = "Medio";
                    }
                    else if (datirow["txt3"].ToString() == AltoMedioBasso.Basso.ToString())
                    {
                        risultato = "Alto";
                    }
                }
                else if (datirow["txt1"].ToString() == AltoMedioBasso.Basso.ToString())
                {
                    if (datirow["txt3"].ToString() == AltoMedioBasso.Alto.ToString())
                    {
                        risultato = "Medio";
                    }
                    else if (datirow["txt3"].ToString() == AltoMedioBasso.Medio.ToString())
                    {
                        risultato = "Alto";
                    }
                    else if (datirow["txt3"].ToString() == AltoMedioBasso.Basso.ToString())
                    {
                        risultato = "Molto Alto";
                    }
                }
            }

            if (datirow != null)
            {
                datirow["txt3c"] = risultato;
            }
          

            risultato = AltoMedioBasso.Sconosciuto.ToString();

            if (datirow != null && datirow["txt1"].ToString() != AltoMedioBasso.Sconosciuto.ToString() && datirow["txt4"].ToString() != AltoMedioBasso.Sconosciuto.ToString())
            {
                if (datirow["txt1"].ToString() == AltoMedioBasso.Alto.ToString())
                {
                    if (datirow["txt4"].ToString() == AltoMedioBasso.Alto.ToString())
                    {
                        risultato = "Molto Basso";
                    }
                    else if (datirow["txt4"].ToString() == AltoMedioBasso.Medio.ToString())
                    {
                        risultato = "Basso";
                    }
                    else if (datirow["txt4"].ToString() == AltoMedioBasso.Basso.ToString())
                    {
                        risultato = "Medio";
                    }
                }
                else if (datirow["txt1"].ToString() == AltoMedioBasso.Medio.ToString())
                {
                    if (datirow["txt4"].ToString() == AltoMedioBasso.Alto.ToString())
                    {
                        risultato = "Basso";
                    }
                    else if (datirow["txt4"].ToString() == AltoMedioBasso.Medio.ToString())
                    {
                        risultato = "Medio";
                    }
                    else if (datirow["txt4"].ToString() == AltoMedioBasso.Basso.ToString())
                    {
                        risultato = "Alto";
                    }
                }
                else if (datirow["txt1"].ToString() == AltoMedioBasso.Basso.ToString())
                {
                    if (datirow["txt4"].ToString() == AltoMedioBasso.Alto.ToString())
                    {
                        risultato = "Medio";
                    }
                    else if (datirow["txt4"].ToString() == AltoMedioBasso.Medio.ToString())
                    {
                        risultato = "Alto";
                    }
                    else if (datirow["txt4"].ToString() == AltoMedioBasso.Basso.ToString())
                    {
                        risultato = "Molto Alto";
                    }
                }
            }

            if (datirow != null)
            {
                datirow["txt4c"] = risultato;
            }
           

            risultato = AltoMedioBasso.Sconosciuto.ToString();

            if (datirow != null && datirow["txt1"].ToString() != AltoMedioBasso.Sconosciuto.ToString() && datirow["txt5"].ToString() != AltoMedioBasso.Sconosciuto.ToString())
            {
                if (datirow["txt1"].ToString() == AltoMedioBasso.Alto.ToString())
                {
                    if (datirow["txt5"].ToString() == AltoMedioBasso.Alto.ToString())
                    {
                        risultato = "Molto Basso";
                    }
                    else if (datirow["txt5"].ToString() == AltoMedioBasso.Medio.ToString())
                    {
                        risultato = "Basso";
                    }
                    else if (datirow["txt5"].ToString() == AltoMedioBasso.Basso.ToString())
                    {
                        risultato = "Medio";
                    }
                }
                else if (datirow["txt1"].ToString() == AltoMedioBasso.Medio.ToString())
                {
                    if (datirow["txt5"].ToString() == AltoMedioBasso.Alto.ToString())
                    {
                        risultato = "Basso";
                    }
                    else if (datirow["txt5"].ToString() == AltoMedioBasso.Medio.ToString())
                    {
                        risultato = "Medio";
                    }
                    else if (datirow["txt5"].ToString() == AltoMedioBasso.Basso.ToString())
                    {
                        risultato = "Alto";
                    }
                }
                else if (datirow["txt1"].ToString() == AltoMedioBasso.Basso.ToString())
                {
                    if (datirow["txt5"].ToString() == AltoMedioBasso.Alto.ToString())
                    {
                        risultato = "Medio";
                    }
                    else if (datirow["txt5"].ToString() == AltoMedioBasso.Medio.ToString())
                    {
                        risultato = "Alto";
                    }
                    else if (datirow["txt5"].ToString() == AltoMedioBasso.Basso.ToString())
                    {
                        risultato = "Molto Alto";
                    }
                }
            }

            if (datirow != null)
            {
                datirow["txt5c"] = risultato;
            }
        

            risultato = AltoMedioBasso.Sconosciuto.ToString();

            if (datirow!= null && datirow["txt1"].ToString() != AltoMedioBasso.Sconosciuto.ToString()  && datirow["txt6"].ToString() != AltoMedioBasso.Sconosciuto.ToString())
            {
                if (datirow["txt1"].ToString() == AltoMedioBasso.Alto.ToString())
                {
                    if (datirow["txt6"].ToString() == AltoMedioBasso.Alto.ToString())
                    {
                        risultato = "Molto Basso";
                    }
                    else if (datirow["txt6"].ToString() == AltoMedioBasso.Medio.ToString())
                    {
                        risultato = "Basso";
                    }
                    else if (datirow["txt6"].ToString() == AltoMedioBasso.Basso.ToString())
                    {
                        risultato = "Medio";
                    }
                }
                else if (datirow["txt1"].ToString() == AltoMedioBasso.Medio.ToString())
                {
                    if (datirow["txt6"].ToString() == AltoMedioBasso.Alto.ToString())
                    {
                        risultato = "Basso";
                    }
                    else if (datirow["txt6"].ToString() == AltoMedioBasso.Medio.ToString())
                    {
                        risultato = "Medio";
                    }
                    else if (datirow["txt6"].ToString() == AltoMedioBasso.Basso.ToString())
                    {
                        risultato = "Alto";
                    }
                }
                else if (datirow["txt1"].ToString() == AltoMedioBasso.Basso.ToString())
                {
                    if (datirow["txt6"].ToString() == AltoMedioBasso.Alto.ToString())
                    {
                        risultato = "Medio";
                    }
                    else if (datirow["txt6"].ToString() == AltoMedioBasso.Medio.ToString())
                    {
                        risultato = "Alto";
                    }
                    else if (datirow["txt6"].ToString() == AltoMedioBasso.Basso.ToString())
                    {
                        risultato = "Molto Alto";
                    }
                }
            }

            if (datirow != null)
            { 
               datirow["txt6c"] = risultato;
            }
          


            if (datirow!=null && datirow["txt1"] != null)
			{
                txt1.Text = datirow["txt1"].ToString().ToUpper();
			}
			else
			{
				txt1.Text = AltoMedioBasso.Sconosciuto.ToString();
			}

            if (datirow != null && datirow["txt2"] != null)
 			{
                txt2.Text = datirow["txt2"].ToString().ToUpper();
            }
			else
			{
				txt2.Text = AltoMedioBasso.Sconosciuto.ToString();
			}

            if (datirow != null && datirow["txt2c"] != null)
			{
                 txt2c.Text = datirow["txt2c"].ToString().ToUpper();
            }
			else
			{
				txt2c.Text = AltoMedioBasso.Sconosciuto.ToString();
			}

            if (datirow != null && datirow["txt3"] != null)
 			{
                txt3.Text = datirow["txt3"].ToString().ToUpper();
			}
			else
			{
				txt3.Text = AltoMedioBasso.Sconosciuto.ToString();
			}

            if (datirow != null && datirow["txt3c"] != null)
			{
               
                txt3c.Text = datirow["txt3c"].ToString().ToUpper();
            }
			else
			{
				txt3c.Text = AltoMedioBasso.Sconosciuto.ToString();
			}

            if (datirow != null && datirow["txt4"] != null)
 			{
               
                txt4.Text = datirow["txt4"].ToString().ToUpper();
            }
			else
			{
				txt4.Text = AltoMedioBasso.Sconosciuto.ToString();
			}

            if (datirow != null && datirow["txt4c"] != null)
			{
                txt4c.Text = datirow["txt4c"].ToString().ToUpper();
             
			}
			else
			{
				txt4c.Text = AltoMedioBasso.Sconosciuto.ToString();
			}

            if (datirow != null && datirow["txt5"] != null)
			{
                txt5.Text = datirow["txt5"].ToString().ToUpper();
            }
			else
			{
				txt5.Text = AltoMedioBasso.Sconosciuto.ToString();
			}

            if (datirow != null && datirow["txt5c"] != null)
			{
                txt5c.Text = datirow["txt5c"].ToString().ToUpper();
            }
			else
			{
				txt5c.Text = AltoMedioBasso.Sconosciuto.ToString();
			}

            if (datirow != null && datirow["txt6"] != null)
			{
             
                txt6.Text = datirow["txt6"].ToString().ToUpper();
            }
			else
			{
				txt6.Text = AltoMedioBasso.Sconosciuto.ToString();
			}

            if (datirow != null && datirow["txt6c"] != null)
			{
                txt6c.Text = datirow["txt6c"].ToString().ToUpper();

            }
			else
			{
				txt6c.Text = AltoMedioBasso.Sconosciuto.ToString();
			}
            if (txt1.Text.ToUpper() == AltoMedioBasso.Sconosciuto.ToString().ToUpper())
                txt1.Text = "NON APPLICABILE";
            if (txt2.Text.ToUpper() == AltoMedioBasso.Sconosciuto.ToString().ToUpper())
                txt2.Text = "NON APPLICABILE";
            if (txt2c.Text.ToUpper() == AltoMedioBasso.Sconosciuto.ToString().ToUpper())
                txt2c.Text = "NON APPLICABILE";
            if (txt3.Text.ToUpper() == AltoMedioBasso.Sconosciuto.ToString().ToUpper())
                txt3.Text = "NON APPLICABILE";
            if (txt3c.Text.ToUpper() == AltoMedioBasso.Sconosciuto.ToString().ToUpper())
                txt3c.Text = "NON APPLICABILE";
            if (txt4.Text.ToUpper() == AltoMedioBasso.Sconosciuto.ToString().ToUpper())
                txt4.Text =  "NON APPLICABILE";
            if (txt4c.Text.ToUpper() == AltoMedioBasso.Sconosciuto.ToString().ToUpper())
                txt4c.Text = "NON APPLICABILE";
            if (txt5.Text.ToUpper() == AltoMedioBasso.Sconosciuto.ToString().ToUpper())
                txt5.Text = "NON APPLICABILE";
            if (txt5c.Text.ToUpper() == AltoMedioBasso.Sconosciuto.ToString().ToUpper())
                txt5c.Text =  "NON APPLICABILE";
            if (txt6.Text.ToUpper() == AltoMedioBasso.Sconosciuto.ToString().ToUpper())
                txt6.Text =  "NON APPLICABILE";
            if (txt6c.Text.ToUpper() == AltoMedioBasso.Sconosciuto.ToString().ToUpper())
                txt6c.Text =  "NON APPLICABILE";

           cBusinessObjects.uc_controls.Clear();
           cBusinessObjects.uc_controls.Add("txt1", txt1);
           cBusinessObjects.uc_controls.Add("txt2", txt2);
           cBusinessObjects.uc_controls.Add("txt2c", txt2c);
           cBusinessObjects.uc_controls.Add("txt3", txt3);
           cBusinessObjects.uc_controls.Add("txt3c", txt3c);
           cBusinessObjects.uc_controls.Add("txt4", txt4);
           cBusinessObjects.uc_controls.Add("txt4c", txt4c);
           cBusinessObjects.uc_controls.Add("txt5", txt5);
           cBusinessObjects.uc_controls.Add("txt5c", txt5c);
           cBusinessObjects.uc_controls.Add("txt6", txt6);
           cBusinessObjects.uc_controls.Add("txt6c", txt6c);


        }


		public int Save()
		{
            return cBusinessObjects.SaveData(id, dati, typeof(RischioGlobale));
         
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
            double newsize = e.NewSize.Width - 30.0;

            try
            {
                brdMain.Width = Convert.ToDouble( newsize );
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }

		}

        private void UserControl_Loaded( object sender, RoutedEventArgs e )
        {
            ;
        }

        //private void UserControl_Loaded( object sender, RoutedEventArgs e )
        //{
        //    XmlDocument doctmp = new XmlDocument();
        //    doctmp.Load( App.AppTemplateFolder + "\\TranscodificaTemplate.xml" );

        //    MasterFile mf = MasterFile.Create();
        //    string file = mf.GetTreeAssociatoFromFileData( Sessioni[SessioneNow].ToString() );
        //    file = App.AppDataDataFolder + "\\" + file;

        //    XmlDataProviderManager xdpm = new XmlDataProviderManager( file );

        //    string templateHere = xdpm.Document.SelectSingleNode( "/Tree//REVISOFT" ).Attributes["Template"].Value;


        //    XmlNode nodeFrom = doctmp.SelectSingleNode( "/TEMPLATES/TEMPLATE[@VERSION=\"" + templateHere + "\"]/TRANSCODE[@HERE=\"" + _ID + "\"]" );
        //    if ( nodeFrom != null && nodeFrom.Attributes["MESSAGE"] != null )
        //    {
        //        MessageBox.Show( nodeFrom.Attributes["MESSAGE"].Value );
        //    }
        //}

        private void RischioIntrinseco_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            txt_MouseDownCicli( sender, e, "2.8.7" );           
        }

        private void CicloVendite_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            txt_MouseDownCicli( sender, e, "2.9.1" );           
        }

        private void CicloAcquisti_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            txt_MouseDownCicli( sender, e, "2.9.2" );           
        }

        private void CicloMagazzino_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            txt_MouseDownCicli( sender, e, "2.9.3" );           
        }

        private void CicloTesoreria_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            txt_MouseDownCicli( sender, e, "2.9.4" );           
        }

        private void CicloPersonale_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            txt_MouseDownCicli( sender, e, "2.9.5" );           
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
    }
}
