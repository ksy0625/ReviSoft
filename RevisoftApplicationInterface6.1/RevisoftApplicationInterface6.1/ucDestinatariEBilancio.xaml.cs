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
using System.IO;
using System.Data;


namespace UserControls
{
    public partial class ucDestinatariEBilancio : UserControl
    {
        public int id;
        private DataTable dati = null;
      

        private string _ID = "";
        //XmlDataProviderManager _d;
        public string _IDTree = "";

        private string IDB_Padre = "227";
        private string IDB_Padre_Consolidato = "321";
        private string IDBA_Padre = "229";

        bool isordinario = false;
        bool isordinario2016 = false;
        bool isabbreviato = false;
        bool isabbreviato2016 = false;

        Hashtable valoreEA = new Hashtable();

        Hashtable SommeDaExcel = new Hashtable();

        public ucDestinatariEBilancio()
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

        private void RetrieveData( string ID)
        {
            DataTable dati_bilancio = null;
            DataTable datiTestata = null;

            string tipoBilancio = "";
            string statoBilancio = "";
            string tipoBilancioA = "";
            string statoBilancioA = "";


            string idsessionebilancio = cBusinessObjects.CercaSessione("", "Bilancio", cBusinessObjects.idsessione.ToString(), cBusinessObjects.idcliente);

             datiTestata = cBusinessObjects.GetData(int.Parse(IDB_Padre), typeof(Excel_Bilancio_Testata), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);

            foreach (DataRow dtrow in datiTestata.Rows)
            {
                if (dtrow["tipoBilancio"].ToString() != "")
                    tipoBilancio = dtrow["tipoBilancio"].ToString();
            
            }
            DataTable statom = cBusinessObjects.GetData(int.Parse(IDB_Padre), typeof(StatoNodi), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);
            foreach (DataRow dd in statom.Rows)
            {
                statoBilancio = dd["Stato"].ToString().Trim();
            }


            if (true)
            {
                if (_IDTree == "31" || _IDTree == "32")
                {
                    ID = IDB_Padre_Consolidato;
                }
                else
                {
                    if (_IDTree != "19")
                    {
                        ID = "-1";
                      

                      
                        if (statoBilancio == "2")
                        {

                            switch (tipoBilancio)
                            {
                                case "2016":
                                    ((Grid)(brdBilancio.Child)).RowDefinitions[11].Height = new GridLength(0);
                                    isordinario2016 = true;
                                    break;
                                default:
                                    isordinario = true;
                                    break;
                            }

                            ID = IDB_Padre;
                        }
                        
                        datiTestata = cBusinessObjects.GetData(int.Parse(IDBA_Padre), typeof(Excel_Bilancio_Testata), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);

                     
                        foreach (DataRow dtrow in datiTestata.Rows)
                        {
                            if (dtrow["tipoBilancio"].ToString() != "")
                                tipoBilancioA = dtrow["tipoBilancio"].ToString();
                           
                        }
                        statom = cBusinessObjects.GetData(int.Parse(IDBA_Padre), typeof(StatoNodi), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);
                        foreach (DataRow dd in statom.Rows)
                        {
                            statoBilancioA = dd["Stato"].ToString().Trim();
                        }

                        if (statoBilancioA == "2")
                        {
                           
                            switch (tipoBilancioA)
                            {
                                case "2016":
                                case "Micro":
                                    ((Grid)(brdBilancio.Child)).RowDefinitions[11].Height = new GridLength(0);
                                    isabbreviato2016 = true;
                                    break;
                                default:
                                    isabbreviato = true;
                                    break;
                            }


                            ID = IDBA_Padre;
                        }
                    }

                }
                if (ID != "-1")
                {
                
                    dati_bilancio = cBusinessObjects.GetData(int.Parse(ID), typeof(Excel_Bilancio), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);

                    foreach (DataRow dtrow in dati_bilancio.Rows)
                    {
                        //Calcolo valori attuali
                        if (dtrow["EA"].ToString() != "")
                        {
                            valoreEA.Add(dtrow["ID"].ToString(), dtrow["EA"].ToString());
                        }
                        else
                        {
                            valoreEA.Add(dtrow["ID"].ToString(), "0");
                        }
                    }
                }
            }
        }
        

        private void obj_PreviewMouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            if ( _ReadOnly )
            {
                MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione" );
                return;
            }
        }

        private void obj_PreviewKeyDown( object sender, KeyEventArgs e )
        {
            if ( _ReadOnly )
            {
                MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione" );
                return;
            }
        }

        public void Load(  string __ID, string FileData, string IDCliente, string IDTree,string IDSessione )
        {
            id = int.Parse(__ID);
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
            
            dati = cBusinessObjects.GetData(id, typeof(DestinatariEBilancio));

            
                _ID = __ID;
        
            _IDTree = IDTree;

            if ( _ReadOnly == true )
            {
                if ( IDTree != "19"  || __ID == "281")
                {
                    cmbDestinatari.PreviewKeyDown += obj_PreviewKeyDown;
                    cmbDestinatari.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDown;
                }

                if (__ID != "281")
                {
                    cmbBilancio.PreviewKeyDown += obj_PreviewKeyDown;
                    cmbBilancio.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDown;
                }
            }


            if ( IDTree == "19")
            {
                if (__ID != "281")
                {
                    brdDestinatari.Visibility = System.Windows.Visibility.Collapsed;
                }

                if (__ID == "281")
                {
                    brdBilancio2.Visibility = System.Windows.Visibility.Collapsed;
                    cmbitem1.Content = "Al Consiglio di Amministrazione";
                    cmbitem2.Content = "All'Amministratore Unico";
                    cmbitem3.Content = "Alla Direzione generale";
                }

                brdBilancio.Visibility = System.Windows.Visibility.Collapsed;
            }

            DataRow xnode = null;
            if (dati.Rows.Count == 0)
                dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);

            foreach (DataRow dtrow in dati.Rows)
            {
                xnode = dtrow;
            }


            if (IDTree != "19")
            {
                MasterFile mf = MasterFile.Create();

                string FileBilancio = "";
                string FileRevisione = "";

                if (IDTree == "22")
                {
                    FileBilancio = mf.GetBilancioAssociatoFromRelazioneVFile(FileData);
                    FileRevisione = mf.GetRevisioneAssociataFromRelazioneVFile(FileData);
                }

                if (IDTree == "32")
                {
                    FileBilancio = mf.GetBilancioAssociatoFromRelazioneVCFile(FileData);
                    FileRevisione = mf.GetRevisioneAssociataFromRelazioneVCFile(FileData);
                }

                if (IDTree == "21")
                {
                    FileBilancio = mf.GetBilancioAssociatoFromRelazioneBFile(FileData);
                    FileRevisione = mf.GetRevisioneAssociataFromRelazioneBFile(FileData);
                }

                if (IDTree == "31")
                {
                    FileBilancio = mf.GetBilancioAssociatoFromRelazioneBCFile(FileData);
                    FileRevisione = mf.GetRevisioneAssociataFromRelazioneBCFile(FileData);
                }

                if (IDTree == "23")
                {
                    FileBilancio = mf.GetBilancioAssociatoFromRelazioneBVFile(FileData);
                    FileRevisione = mf.GetRevisioneAssociataFromRelazioneBVFile(FileData);
                }

                XmlDataProviderManager _b = null;

                //if (FileBilancio != "" && (new FileInfo(FileBilancio)).Exists)
                if (!string.IsNullOrEmpty(FileBilancio))
                {
                    _b = new XmlDataProviderManager(FileBilancio);
                }
                else
                {
                    _b = null;
                }

                //XmlDataProviderManager _y = null;

             
                #region Dati da revisione


                xnode["RagioneSociale"] = "Dato Mancante: Compilare Carta di Lavoro 2.1.1";

                xnode["RagioneSociale"] = cBusinessObjects.GetRagioneSociale();
                xnode["Indirizzo"] = cBusinessObjects.GetIndirizzo();
                xnode["REA"] = cBusinessObjects.GetREA();
                xnode["CapitaleSociale"] = cBusinessObjects.GetCapitaleSociale();


     
                #endregion

              
                #region Dati da bilancio

                if (_b != null)
                {
                    RetrieveData("-1");

                    if (isabbreviato == true)
                    {
                        SommeDaExcel.Add("ValoreProduzione", "189|190|191|192|194|195");
                        SommeDaExcel.Add("CostiProduzione", "198|199|200|202|203|204|205|206|208|209|210|211|212|213|214|215");
                        SommeDaExcel.Add("RisultatoGestione", "222|223|224|227|228|229|230|231|232|234|235|236|237|239|240|241|242|243");
                        SommeDaExcel.Add("Rettifiche", "246|250");
                        SommeDaExcel.Add("RisultatoExtragestione", "256|259");
                        SommeDaExcel.Add("Imposte", "266");
                        SommeDaExcel.Add("UtilePerditaEconomico", "189|190|192|194|195|198|199|200|202|203|204|208|209|212|213|214|215|210|211|222|223|224|227|228|229|230|231|232|234|235|236|237|239|240|241|242|243|246|250|256|259|266");
                        SommeDaExcel.Add("Attivita", "3|4|10071|10072|10073|10081|10082|10083|10092|10093|23|50|1059|1060|80|89|97");
                        SommeDaExcel.Add("UtilePerditaPatrimoniale", "120");
                        SommeDaExcel.Add("Patrimonio", "108|109|110|111|112|113|100114|119");
                        SommeDaExcel.Add("Passivita", "123|129|133|134|175");
                    }

                    if (isabbreviato2016 == true)
                    {
                        SommeDaExcel.Add("ValoreProduzione", "189|2016190|190|191|192|194|195");
                        SommeDaExcel.Add("CostiProduzione", "212|213|214|215|2016208|208|209|210|211|202|203|204|205|206|2016204|200|199|198");
                        SommeDaExcel.Add("RisultatoGestione", "222|223|224|2016224|20162241|235|236|237|234|232|231|228|229|230|227|2016237|2016231|2016229|240|241|242|239|2016242|243");
                        SommeDaExcel.Add("Rettifiche", "247|248|249|251|252|253|2016249|20162491|20162492|20162493");
                        SommeDaExcel.Add("RisultatoExtragestione", "");
                        SommeDaExcel.Add("Imposte", "267|268|217005|217006|2016267");
                        SommeDaExcel.Add("UtilePerditaEconomico", "189|2016190|190|191|192|194|195|212|213|214|215|2016208|208|209|210|211|202|203|204|205|206|2016204|200|199|198|222|223|224|2016224|20162241|235|236|237|234|232|231|228|229|230|227|2016237|2016231|2016229|240|241|242|239|2016242|243|247|248|249|251|252|253|2016249|20162491|20162492|20162493|267|268|217005|217006|2016267");

                        SommeDaExcel.Add("Attivita", "2|7|16|1009|50|1059|1060|80|89|201655|98");
                        SommeDaExcel.Add("UtilePerditaPatrimoniale", "11611");
                        SommeDaExcel.Add("Patrimonio", "108|109|110|111|114|112|100114|119|11611|2016114|2016998");
                        SommeDaExcel.Add("Passivita", "108|109|110|111|112|100114|119|11611|123|129|133|134|175|2016114|2016998|114");
                    }

                    if (isordinario2016 == true)
                    {
                        SommeDaExcel.Add("ValoreProduzione", "189|190|191|192|194|195");
                        SommeDaExcel.Add("CostiProduzione", "198|199|200|202|203|204|205|206|208|209|210|211|212|213|214|215");
                        SommeDaExcel.Add("RisultatoGestione", "222|223|224|2016224|20162241|235|236|237|234|232|231|228|229|230|227|2016237|2016229|240|241|242|239|2016242|243");
                        SommeDaExcel.Add("Rettifiche", "247|248|249|251|252|253|2016249|20162491|20162492|20162493");
                        SommeDaExcel.Add("RisultatoExtragestione", "");
                        SommeDaExcel.Add("Imposte", "267|268|217005|217006|2016267");
                        SommeDaExcel.Add("UtilePerditaEconomico", "247|248|249|251|252|253|2016249|20162491|20162492|20162493|198|199|200|202|203|204|205|206|208|209|210|211|212|213|214|215|189|190|191|192|194|195|222|223|224|2016224|20162241|235|236|237|234|232|231|228|229|230|227|2016237|2016229|240|241|242|239|2016242|243|267|268|217005|217006|2016267");

                        SommeDaExcel.Add("Attivita", "3|4|8|9|10|11|12|13|14|17|18|19|20|21|25|26|27|28|32|33|35|36|38|39|41|42|43|44|51|52|53|54|55|59|60|62|63|65|66|68|69|71|72|73|77|78|81|82|83|84|85|86|90|91|92|98|201655|201627|201638|201639|201677|201678|201651|201683");
                        SommeDaExcel.Add("UtilePerditaPatrimoniale", "11611");
                        SommeDaExcel.Add("Patrimonio", "108|109|110|111|112|113|117|11600|11601|11602|11603|11604|11605|115|11606|11607|116|11608|11609|11610|118|119|120|1160|11700|11701|114|20161131|20161132|20161133|20161134|20161135|20161136|20161137|20161138|20161139|20161140|20161141|20161142|20171142|20161143|2016114|2016998|11611");
                        SommeDaExcel.Add("Passivita", "108|109|110|111|112|113|117|11600|11601|11602|11603|11604|11605|115|11606|11607|116|11608|11609|11610|118|119|120|1160|11700|11701|114|20161131|20161132|20161133|20161134|20161135|20161136|20161137|20161138|20161139|20161140|20161141|20161142|20171142|20161143|2016114|2016998|11611|124|125|126|2016126|129|133|134|136|137|139|140|142|143|145|146|148|149|151|152|154|155|157|158|160|161|163|164|166|167|169|170|172|173|2016163|2016164|175");
                    }

                    if (_IDTree == "31" || _IDTree == "32")
                    {
                        SommeDaExcel.Add("ValoreProduzione", "189|190|191|192|194|195");
                        SommeDaExcel.Add("CostiProduzione", "198|199|200|202|203|204|205|206|208|209|210|211|212|213|214|215");
                        SommeDaExcel.Add("RisultatoGestione", "222|223|224|2016224|20162241|235|236|237|234|232|231|228|229|230|227|2016237|2016229|240|241|242|239|2016242|243");
                        SommeDaExcel.Add("Rettifiche", "247|248|249|251|252|253|2016249|20162491|20162492|20162493");
                        SommeDaExcel.Add("RisultatoExtragestione", "");
                        SommeDaExcel.Add("Imposte", "267|268|217005|217006|2016267");
                        SommeDaExcel.Add("UtilePerditaEconomico", "247|248|249|251|252|253|2016249|20162491|20162492|20162493|198|199|200|202|203|204|205|206|208|209|210|211|212|213|214|215|189|190|191|192|194|195|222|223|224|2016224|20162241|235|236|237|234|232|231|228|229|230|227|2016237|2016229|240|241|242|239|2016242|243|267|268|217005|217006|2016267");

                        SommeDaExcel.Add("Attivita", "3|4|8|9|10|11|12|13|14|17|18|19|20|21|25|26|27|28|32|33|35|36|38|39|41|42|43|44|51|52|53|54|55|59|60|62|63|65|66|68|69|71|72|73|77|78|81|82|83|84|85|86|90|91|92|98|201655|201627|201638|201639|201677|201678|201651|201683");
                        SommeDaExcel.Add("UtilePerditaPatrimoniale", "11611");
                        SommeDaExcel.Add("Patrimonio", "108|109|110|111|112|113|117|11600|11601|11602|11603|11604|11605|115|11606|11607|116|11608|11609|11610|118|119|120|1160|11700|11701|114|20161131|20161132|20161133|20161134|20161135|20161136|20161137|20161138|20161139|20161140|20161141|20161142|20171142|20181142|20161143|2016114|2016998|11611");
                        SommeDaExcel.Add("Passivita", "108|109|110|111|112|113|117|11600|11601|11602|11603|11604|11605|115|11606|11607|116|11608|11609|11610|118|119|120|1160|11700|11701|114|20161131|20161132|20161133|20161134|20161135|20161136|20161137|20161138|20161139|20161140|20161141|20161142|20171142|20181142|20161143|2016114|2016998|11611|124|125|2018125|126|2016126|129|133|134|136|137|139|140|142|143|145|146|148|149|151|152|154|155|157|158|160|161|163|164|166|167|169|170|172|173|2016163|2016164|175");
                    }

                    if (isordinario == true)
                    {
                        SommeDaExcel.Add("ValoreProduzione", "189|190|191|192|194|195");
                        SommeDaExcel.Add("CostiProduzione", "198|199|200|202|203|204|205|206|208|209|210|211|212|213|214|215");
                        SommeDaExcel.Add("RisultatoGestione", "222|223|224|227|228|229|230|231|232|234|235|236|237|239|240|241|242|243");
                        SommeDaExcel.Add("Rettifiche", "247|248|249|251|252|253");
                        SommeDaExcel.Add("RisultatoExtragestione", "257|258|260|261|262|21700412|21700414");
                        SommeDaExcel.Add("Imposte", "267|268|217005|217006");
                        SommeDaExcel.Add("UtilePerditaEconomico", "189|190|191|192|194|195|198|199|200|202|203|204|205|206|208|209|210|211|212|213|214|215|222|223|224|227|228|229|230|231|232|234|235|236|237|239|240|241|242|243|247|248|249|251|252|253|257|258|260|261|262|21700412|21700414|267|268|217005|217006");
                        SommeDaExcel.Add("Attivita", "3|4|8|9|10|11|12|13|14|17|18|19|20|21|25|26|27|28|32|33|35|36|38|39|41|42|43|44|51|52|53|54|55|59|60|62|63|65|66|68|69|71|72|74|75|77|78|81|82|83|84|85|86|90|91|92|98|99");
                        SommeDaExcel.Add("UtilePerditaPatrimoniale", "120");
                        SommeDaExcel.Add("Patrimonio", "108|109|110|111|112|113|117|11600|11601|11602|11603|11604|11605|115|11606|11607|116|11608|11609|11610|118|119|1160|11700|11701");
                        SommeDaExcel.Add("Passivita", "124|125|126|129|133|134|136|137|139|140|142|143|145|146|148|149|151|152|154|155|157|158|160|161|163|164|166|167|169|170|172|173|176|177");
                    }

                    txtValoreProduzione.Text = ConvertInteger(GetValoreEA("ValoreProduzione").ToString());
                    txtCostiProduzione.Text = ConvertInteger(GetValoreEA("CostiProduzione").ToString());
                    txtRisultatoGestione.Text = ConvertInteger(GetValoreEA("RisultatoGestione").ToString());
                    txtRettifiche.Text = ConvertInteger(GetValoreEA("Rettifiche").ToString());
                    txtRisultatoExtragestione.Text = ConvertInteger(GetValoreEA("RisultatoExtragestione").ToString());
                    txtImposte.Text = ConvertInteger(GetValoreEA("Imposte").ToString());
                    txtUtilePerditaEconomico.Text = ConvertInteger(GetValoreEA("UtilePerditaEconomico").ToString());
                    txtAttivita.Text = ConvertInteger(GetValoreEA("Attivita").ToString());
                    txtUtilePerditaPatrimoniale.Text = ConvertInteger(GetValoreEA("UtilePerditaPatrimoniale").ToString());
                    txtPatrimonioNetto.Text = ConvertInteger(GetValoreEA("Patrimonio").ToString());//Patrimonio - Utile
                    txtPassivita.Text = ConvertInteger(GetValoreEA("Passivita").ToString());//Passività - Partimonio netto


             

                    xnode["txtValoreProduzione"] = txtValoreProduzione.Text;
                    xnode["txtCostiProduzione"] = txtCostiProduzione.Text;
                    xnode["txtRisultatoGestione"] = txtRisultatoGestione.Text;
                    xnode["txtRettifiche"] = txtRettifiche.Text;
                    xnode["txtRisultatoExtragestione"] = txtRisultatoExtragestione.Text;
                    xnode["txtImposte"] = txtImposte.Text;
                    xnode["txtUtilePerditaEconomico"] = txtUtilePerditaEconomico.Text;
                    xnode["txtAttivita"] = txtAttivita.Text;
                    xnode["txtPassivita"] = txtPassivita.Text;
                    xnode["txtPatrimonioNetto"] = txtPatrimonioNetto.Text;
                    xnode["txtUtilePerditaPatrimoniale"] = txtUtilePerditaPatrimoniale.Text;
                }
                else
                {
                    if (xnode["txtValoreProduzione"].ToString() == "")
                    {
                        txtValoreProduzione.Text = "0";
                        xnode["txtValoreProduzione"] = "0";
                    }
                    else
                    {
                        txtValoreProduzione.Text = xnode["txtValoreProduzione"].ToString();
                    }


                    if (xnode["txtCostiProduzione"].ToString() == "")
                    {
                    
                        txtCostiProduzione.Text = "0";
                        xnode["txtCostiProduzione"] = "0";
                    }
                    else
                    {
                        txtCostiProduzione.Text = xnode["txtCostiProduzione"].ToString();
                    }

                    if (xnode["txtRisultatoGestione"].ToString() == "")
                    {
                      
                        txtRisultatoGestione.Text = "0";
                        xnode["txtRisultatoGestione"] = "0";
                    }
                    else
                    {
                        txtRisultatoGestione.Text = xnode["txtRisultatoGestione"].ToString();
                    }

                    if (xnode["txtRettifiche"].ToString() == "")
                    { 
                        txtRettifiche.Text = "0";
                        xnode["txtRettifiche"] = "0";
                    }
                    else
                    {
                        txtRettifiche.Text = xnode["txtRettifiche"].ToString();
                    }

                    if (xnode["txtRisultatoExtragestione"].ToString() == "")
                    {

                        txtRisultatoExtragestione.Text = "0";
                        xnode["txtRisultatoExtragestione"] = "0";
                    }
                    else
                    {
                        txtRisultatoExtragestione.Text = xnode["txtRisultatoExtragestione"].ToString();
                    }

                    if (xnode["txtImposte"].ToString() == "")
                    {
                     
                        txtImposte.Text = "0";
                        xnode["txtImposte"] = "0";
                    }
                    else
                    {
                        txtImposte.Text = xnode["txtImposte"].ToString();
                    }

                    if (xnode["txtUtilePerditaEconomico"].ToString() == "")
                    {
                        txtUtilePerditaEconomico.Text = "0";
                        xnode["txtUtilePerditaEconomico"] = "0";
                    }
                    else
                    {
                        txtUtilePerditaEconomico.Text = xnode["txtUtilePerditaEconomico"].ToString();
                    }

                    if (xnode["txtAttivita"].ToString() == "")
                    {
                        
                        txtAttivita.Text = "0";
                        xnode["txtAttivita"]= "0";
                    }
                    else
                    {
                        txtAttivita.Text = xnode["txtAttivita"].ToString();
                    }

                    if (xnode["txtPassivita"].ToString() == "")
                    {
                        txtPassivita.Text = "0";
                        xnode["txtPassivita"] = "0";
                    }
                    else
                    {
                        txtPassivita.Text = xnode["txtPassivita"].ToString();
                    }

                    if (xnode["txtPatrimonioNetto"].ToString() == "")
                    {
                      
                        txtPatrimonioNetto.Text = "0";
                        xnode["txtPatrimonioNetto"] = "0";
                    }
                    else
                    {
                        txtPatrimonioNetto.Text = xnode["txtPatrimonioNetto"].ToString();
                    }

                    if (xnode["txtUtilePerditaPatrimoniale"].ToString() == "")
                    {
                     
                        txtUtilePerditaPatrimoniale.Text = "0";
                        xnode["txtUtilePerditaPatrimoniale"] = "0";
                    }
                    else
                    {
                        txtUtilePerditaPatrimoniale.Text = xnode["txtUtilePerditaPatrimoniale"].ToString();
                    }
                }
                #endregion
            }

            if ( xnode["cmbDestinatari"].ToString() == "" )
            {
               
                xnode["cmbDestinatari"] = "(Selezionare una voce)";
            }

            cmbDestinatari.Text = xnode["cmbDestinatari"].ToString();

         
            if (IDTree == "31" || IDTree == "32")
            {
                if (xnode["cmbBilancio"].ToString() == "")
                {
                   
                    xnode["cmbBilancio"] = "al consolidato";
                }

                cmbBilancio.Text = "al consolidato";
            }
            else
            {
                if (xnode["cmbBilancio"].ToString() == "")
                {
                    xnode["cmbBilancio"] = "all'esercizio";
                }

                cmbBilancio.Text = "all'esercizio";
            }

        
        }

        public int Save()
        {
            foreach (DataRow xnode in dati.Rows)
            {
                if ( _IDTree != "19" )
                {
                   

                    xnode["txtValoreProduzione"] = txtValoreProduzione.Text;

                   

                    xnode["txtCostiProduzione"] = txtCostiProduzione.Text;

                   

                    xnode["txtRisultatoGestione"] = txtRisultatoGestione.Text;

                   

                    xnode["txtRettifiche"] = txtRettifiche.Text;
                
                    xnode["txtRisultatoExtragestione"] = txtRisultatoExtragestione.Text;

                   
                    xnode["txtImposte"] = txtImposte.Text;

                 
                    xnode["txtUtilePerditaEconomico"] = txtUtilePerditaEconomico.Text;

                   
                    xnode["txtAttivita"] = txtAttivita.Text;

                   
                    xnode["txtPassivita"] = txtPassivita.Text;

                    
                    xnode["txtPatrimonioNetto"] = txtPatrimonioNetto.Text;

                   
                    xnode["txtUtilePerditaPatrimoniale"] = txtUtilePerditaPatrimoniale.Text;
                    
                    xnode["cmbBilancio"] = cmbBilancio.Text;
                }
                
              
                xnode["cmbDestinatari"] = cmbDestinatari.Text;

            }
            return cBusinessObjects.SaveData(id, dati, typeof(DestinatariEBilancio));
        }

        private double GetValoreEA( string Cella )
        {
            double returnvalue = 0.0;

            if ( SommeDaExcel.Contains( Cella ) )
            {
                foreach ( string ID in SommeDaExcel[Cella].ToString().Split( '|' ) )
                {
                    double dblValore = 0.0;

                    if ( valoreEA.Contains( ID ) )
                    {
                        double.TryParse( valoreEA[ID].ToString(), out dblValore );
                    }

                    returnvalue += dblValore;
                }
            }

            return returnvalue;
        }



        private string ConvertInteger( string valore )
        {
            double dblValore = 0.0;

            double.TryParse( valore, out dblValore );

            if ( dblValore == 0.0 )
            {
                return "0";
            }
            else
            {
                return String.Format( "{0:#,0}", dblValore );
            }
        }

        private void UserControl_SizeChanged( object sender, SizeChangedEventArgs e )
        {
            //double newsize = e.NewSize.Width - 30.0;

            //try
            //{				
            //    foreach (UIElement item in stack.Children)
            //    {
            //        ((UserControl)(((Grid)(((Border)(item)).Child)).Children[2])).Width = newsize - 30;
            //    }

            //    stack.Width = Convert.ToDouble(newsize);
            //}
            //catch (Exception ex)
            //{
            //    string log = ex.Message;
            //}
        }

        private void Image_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {

        }
    }
}
