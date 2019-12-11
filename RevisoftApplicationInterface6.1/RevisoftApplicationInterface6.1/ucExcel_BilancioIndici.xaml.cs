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
    public partial class ucExcel_BilancioIndici : UserControl
    {
        public int id;
        private DataTable dati = null;
        private DataTable datiTestata  = null;
       

        private string down = "./Images/icone/navigate_down.png";
        private string left = "./Images/icone/navigate_left.png";

		private string IDB_Padre = "227";

		Hashtable valoreEA = new Hashtable();
		Hashtable valoreEP = new Hashtable();

		Hashtable SommeDaExcel = new Hashtable();
		Hashtable ValoriDaExcelEA = new Hashtable();
		Hashtable ValoriDaExcelEP = new Hashtable();

		public ucExcel_BilancioIndici()
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

		public void Load(ref XmlDataProviderManager _x, XmlDataProviderManager x_AP, string _ID,string IDCliente,string IDSessione)
        {

            id = int.Parse(_ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());


            if ( _ID == "326")
            {
                IDB_Padre = "321";
            }

            if (_ID == "207")
            {
                IDB_Padre = "227";
            }

            if (_ID == "171")
            {
                IDB_Padre = "166";
            }
            if (_ID == "174")
            {
                IDB_Padre = "172";
            }

            if (_ID == "220")
            {
                IDB_Padre = "229";
            }

            if ( _ID  == "140")
            {
                IDB_Padre = "134";
            }

            if (_ID == "2016140")
            {
                IDB_Padre = "2016134";
            }

            if (_ID == "2016179")
            {
                IDB_Padre = "2016174";
            }

            if (_ID == "2016191")
            {
                IDB_Padre = "2016186";
            }
            datiTestata = cBusinessObjects.GetData(int.Parse(IDB_Padre), typeof(Excel_Bilancio_Testata));
         
            #region Dati da bilancio

            RetrieveData(_x, x_AP, IDB_Padre);

            string tipoBilancio = "";
            bool abbreviato = false;
            if (_ID == "326")
            {
                SommeDaExcel.Clear();
                //crediti verso clienti
                SommeDaExcel.Add("B7", "59|62|65|68|201677|201678");
                //liquidità immediate differite
                SommeDaExcel.Add("B12", "3|4|81|82|83|84|85|86|32|35|38|41|76|201683|201651|98|77|71|73|59|62|65|68|201677|201678|90|91|92");
                //Disponibilità
                SommeDaExcel.Add("B15", "51|52|53|54|201655|55|-148|-149");
                //attivo fisso
                SommeDaExcel.Add("B25", "17|18|19|20|21|8|9|10|11|12|13|14|25|26|27|28|201627|43|44|33|36|39|42|60|63|66|69|72|78|201638|201639|40");
                //Debiti Commerciali
                SommeDaExcel.Add("B34", "151|157|160|163|2016163");
                //passività correnti
                SommeDaExcel.Add("B39", "142|151|157|160|163|2016163|133|136|139|145|154|169|172|175|176|177|166");
                //passività consolidate
                SommeDaExcel.Add("B46", "124|125|2018125|126|129|2016164|2016126|134|137|140|143|146|152|155|158|161|164|167|170|173");
                //mezzi propri
                SommeDaExcel.Add("B54", "108|109|110|111|112|114|2016114|2016998|20161131|20161132|20161133|20161134|20161135|20161136|20161137|20161138|20161139|20161140|20161141|20171142|20181142|20161142|20161143|117|11600|11601|11602|11603|11604|11605|115|11606|11607|116|11608|11609|11610|118|119|1160|11700|11701|11611");
                //valore della produzione
                SommeDaExcel.Add("B62", "189|190|191|192|194|195");
                //consumi
                SommeDaExcel.Add("B65", "198|212");
                //utile operativo
                SommeDaExcel.Add("B72", "189|190|191|192|194|195|198|212|199|208|209|210|202|203|204|205|206|200|211|213|214|215");
                //risultato netto della gestione finanziaria
                SommeDaExcel.Add("B77", "222|223|224|2016224|20162241|235|236|237|234|232|231|228|229|230|227|2016237|2016229|240|241|242|239|2016242|243");
                //utile esercizio
                SommeDaExcel.Add("B87", "11611");
                //capitale investito
                SommeDaExcel.Add("I22", "33|36|39|42|60|63|66|69|72|78|201638|201639|40|25|26|27|28|201627|43|44|8|9|10|11|12|13|14|17|18|19|20|21|55|51|52|53|54|201655|3|4|81|82|83|84|85|86|32|35|38|41|76|201683|201651|98|99|77|71|73|59|62|65|68|201677|201678|-151|-157|-160|-163|-2016163|-133|-136|-139|-145|-154|-169|-172|-166|-175|-176|-177|-148|-149");
                
            }
            else
            {

              
                foreach (DataRow dtrow in datiTestata.Rows)
                {
                    if (dtrow["tipoBilancio"].ToString() != "")
                        tipoBilancio = dtrow["tipoBilancio"].ToString();
                }

                if (IDB_Padre == "227" || IDB_Padre == "134" || IDB_Padre == "2016134" || IDB_Padre == "166")
                {
                    switch (tipoBilancio)
                    {
                        case "2016":
                            SommeDaExcel.Clear();
                            //crediti verso clienti
                            SommeDaExcel.Add("B7", "59|62|65|68|201677|201678");
                            //liquidità immediate differite
                            SommeDaExcel.Add("B12", "3|4|81|82|83|84|85|86|32|35|38|41|76|201683|201651|98|77|71|73|59|62|65|68|201677|201678|90|91|92");
                            //Disponibilità
                            SommeDaExcel.Add("B15", "51|52|53|54|201655|55|-148|-149");
                            //attivo fisso
                            SommeDaExcel.Add("B25", "17|18|19|20|21|8|9|10|11|12|13|14|25|26|27|28|201627|43|44|33|36|39|42|60|63|66|69|72|78|201638|201639|40");
                            //Debiti Commerciali
                            SommeDaExcel.Add("B34", "151|157|160|163|2016163");
                            //passività correnti
                            SommeDaExcel.Add("B39", "142|151|157|160|163|2016163|133|136|139|145|154|169|172|175|176|177|166");
                            //passività consolidate
                            SommeDaExcel.Add("B46", "124|125|126|129|2016164|2016126|134|137|140|143|146|152|155|158|161|164|167|170|173");
                            //mezzi propri
                            SommeDaExcel.Add("B54", "108|109|110|111|112|114|2016114|2016998|20161131|20161132|20161133|20161134|20161135|20161136|20161137|20161138|20161139|20161140|20161141|20171142|20161142|20161143|117|11600|11601|11602|11603|11604|11605|115|11606|11607|116|11608|11609|11610|118|119|1160|11700|11701|11611");
                            //valore della produzione
                            SommeDaExcel.Add("B62", "189|190|191|192|194|195");
                            //consumi
                            SommeDaExcel.Add("B65", "198|212");
                            //utile operativo
                            SommeDaExcel.Add("B72", "189|190|191|192|194|195|198|212|199|208|209|210|202|203|204|205|206|200|211|213|214|215");
                            //risultato netto della gestione finanziaria
                            SommeDaExcel.Add("B77", "222|223|224|2016224|20162241|235|236|237|234|232|231|228|229|230|227|2016237|2016229|240|241|242|239|2016242|243");
                            //utile esercizio
                            SommeDaExcel.Add("B87", "11611");
                            //capitale investito
                            SommeDaExcel.Add("I22", "33|36|39|42|60|63|66|69|72|78|201638|201639|40|25|26|27|28|201627|43|44|8|9|10|11|12|13|14|17|18|19|20|21|55|51|52|53|54|201655|3|4|81|82|83|84|85|86|32|35|38|41|76|201683|201651|98|99|77|71|73|59|62|65|68|201677|201678|-151|-157|-160|-163|-2016163|-133|-136|-139|-145|-154|-169|-172|-166|-175|-176|-177|-148|-149");
                            break;
                        default:
                            SommeDaExcel.Clear();
                            //crediti verso clienti
                            SommeDaExcel.Add("B7", "59|62|65|68");
                            //liquidità immediate differite
                            SommeDaExcel.Add("B12", "3|4|81|82|83|84|85|86|32|35|38|41|98|99|77|71|74|59|62|65|68|90|91|92");
                            //Disponibilità
                            SommeDaExcel.Add("B15", "-148|55|51|52|53|54");
                            //attivo fisso
                            SommeDaExcel.Add("B25", "17|18|19|20|21|8|9|10|11|12|13|14|25|26|27|28|43|44|33|36|39|42|60|63|66|69|72|75|78");
                            //Debiti Commerciali
                            SommeDaExcel.Add("B34", "151|157|160|163");
                            //passività correnti
                            SommeDaExcel.Add("B39", "142|151|157|160|163|133|136|139|145|154|169|172|176|177|166");
                            //passività consolidate
                            SommeDaExcel.Add("B46", "124|125|126|129|134|137|140|143|146|152|155|158|161|164|167|170|173");
                            //mezzi propri
                            SommeDaExcel.Add("B54", "108|109|110|111|112|113|117|11600|11601|11602|11603|11604|11605|115|11606|11607|116|11608|11609|11610|118|119|1160|11700|11701|120");
                            //valore della produzione
                            SommeDaExcel.Add("B62", "189|190|191|192|194|195");
                            //consumi
                            SommeDaExcel.Add("B65", "198|212");
                            //utile operativo
                            SommeDaExcel.Add("B72", "189|190|191|192|194|195|198|212|199|208|209|210|202|203|204|205|206|200|211|213|214|215");
                            //risultato netto della gestione finanziaria
                            SommeDaExcel.Add("B77", "222|223|224|227|228|229|230|231|232|234|235|236|237|239|240|241|242|243");
                            //utile esercizio
                            SommeDaExcel.Add("B87", "120");
                            //capitale investito
                            SommeDaExcel.Add("I22", "17|18|19|20|21|8|9|10|11|12|13|14|25|26|27|28|43|44|33|36|39|42|60|63|66|69|72|75|78|59|62|65|68|71|74|77|98|99|3|4|81|82|83|84|85|86|32|35|38|41|51|52|53|54|55|-151|-157|-160|-163|-133|-136|-139|-145|-154|-169|-172|-166|-176|-177|-148|-149");
                            break;
                    }
                }
                else
                {
                    abbreviato = true;

                    switch (tipoBilancio)
                    {
                        case "Micro":
                            SommeDaExcel.Clear();
                            //crediti verso clienti
                            SommeDaExcel.Add("B7", "");
                            //liquidità immediate differite
                            SommeDaExcel.Add("B12", "89|1059|1060|97|98|2|80");
                            //Disponibilità
                            SommeDaExcel.Add("B15", "50|201655");
                            //attivo fisso
                            SommeDaExcel.Add("B25", "7|16|1009");
                            //Debiti Commerciali
                            SommeDaExcel.Add("B34", "");
                            //passività correnti
                            SommeDaExcel.Add("B39", "133|175");
                            //passività consolidate
                            SommeDaExcel.Add("B46", "123|129|134");
                            //mezzi propri
                            SommeDaExcel.Add("B54", "108|109|110|111|112|114|100114|119|2016114|2016998|11611");
                            //valore della produzione
                            SommeDaExcel.Add("B62", "189|2016190|190|191|192|194|195");
                            //consumi
                            SommeDaExcel.Add("B65", "198|212");
                            //utile operativo
                            SommeDaExcel.Add("B72", "189|2016190|190|191|192|194|195|198|212|199|2016208|208|209|210|211|202|203|204|205|206|2016204|200|209|213|214|215");
                            //risultato netto della gestione finanziaria
                            SommeDaExcel.Add("B77", "222|223|224|2016224|20162241|235|236|237|234|232|231|228|229|230|227|2016237|2016231|2016229|240|241|242|239|2016242|243");
                            //utile esercizio
                            SommeDaExcel.Add("B87", "11611");
                            //capitale investito
                            SommeDaExcel.Add("I22", "7|16|1009|50|201655|89|1059|1060|97|98|2|80");
                            break;
                        case "2016":
                            SommeDaExcel.Clear();
                            //crediti verso clienti
                            SommeDaExcel.Add("B7", "");
                            //liquidità immediate differite
                            SommeDaExcel.Add("B12", "89|1059|1060|97|98|2|80");
                            //Disponibilità
                            SommeDaExcel.Add("B15", "50|201655");
                            //attivo fisso
                            SommeDaExcel.Add("B25", "7|16|1009");
                            //Debiti Commerciali
                            SommeDaExcel.Add("B34", "");
                            //passività correnti
                            SommeDaExcel.Add("B39", "133|175");
                            //passività consolidate
                            SommeDaExcel.Add("B46", "123|129|134");
                            //mezzi propri
                            SommeDaExcel.Add("B54", "108|109|110|111|112|114|100114|119|2016114|2016998|11611");
                            //valore della produzione
                            SommeDaExcel.Add("B62", "189|2016190|190|191|192|194|195");
                            //consumi
                            SommeDaExcel.Add("B65", "198|212");
                            //utile operativo
                            SommeDaExcel.Add("B72", "189|2016190|190|191|192|194|195|198|212|199|2016208|208|209|210|211|202|203|204|205|206|2016204|200|209|213|214|215");
                            //risultato netto della gestione finanziaria
                            SommeDaExcel.Add("B77", "222|223|224|2016224|20162241|235|236|237|234|232|231|228|229|230|227|2016237|2016231|2016229|240|241|242|239|2016242|243");
                            //utile esercizio
                            SommeDaExcel.Add("B87", "11611");
                            //capitale investito
                            SommeDaExcel.Add("I22", "7|16|1009|50|201655|89|1059|1060|97|98|2|80");
                            break;
                        default:
                            SommeDaExcel.Clear();
                            //crediti verso clienti
                            SommeDaExcel.Add("B7", "");
                            //liquidità immediate differite
                            SommeDaExcel.Add("B12", "89|1059|1060|97|3|4|80");
                            //Disponibilità
                            SommeDaExcel.Add("B15", "50|201655");
                            //attivo fisso
                            SommeDaExcel.Add("B25", "7|16|1009");
                            //Debiti Commerciali
                            SommeDaExcel.Add("B34", "");
                            //passività correnti
                            SommeDaExcel.Add("B39", "133|175");
                            //passività consolidate
                            SommeDaExcel.Add("B46", "123|129|134");
                            //mezzi propri
                            SommeDaExcel.Add("B54", "108|109|110|111|112|114|100114|119|2016114|2016998|11611");
                            //valore della produzione
                            SommeDaExcel.Add("B62", "189|2016190|190|191|192|194|195");
                            //consumi
                            SommeDaExcel.Add("B65", "198|212");
                            //utile operativo
                            SommeDaExcel.Add("B72", "189|2016190|190|191|192|194|195|198|212|199|2016208|208|209|210|211|202|203|204|205|206|2016204|200|209|213|214|215");
                            //risultato netto della gestione finanziaria
                            SommeDaExcel.Add("B77", "222|223|224|2016224|20162241|235|236|237|234|232|231|228|229|230|227|2016237|2016231|2016229|240|241|242|239|2016242|243");
                            //utile esercizio
                            SommeDaExcel.Add("B87", "11611");
                            //capitale investito
                            SommeDaExcel.Add("I22", "7|16|1009|50|201655|89|1059|1060|97|98|2|80");
                            break;
                    }
                }
            }

            #endregion
                            
			ValoriDaExcelEA.Add("B7", GetValoreEA("B7"));
			ValoriDaExcelEP.Add("B7", GetValoreEP("B7"));
            
			ValoriDaExcelEA.Add("B12", GetValoreEA("B12"));
			ValoriDaExcelEP.Add("B12", GetValoreEP("B12"));
            
			ValoriDaExcelEA.Add("B15", GetValoreEA("B15"));
			ValoriDaExcelEP.Add("B15", GetValoreEP("B15"));            
            
			ValoriDaExcelEA.Add("B25", GetValoreEA("B25"));
			ValoriDaExcelEP.Add("B25", GetValoreEP("B25"));            
            
			ValoriDaExcelEA.Add("B34", GetValoreEA("B34"));
			ValoriDaExcelEP.Add("B34", GetValoreEP("B34"));
            
            ValoriDaExcelEA.Add("B39", GetValoreEA("B39"));
			ValoriDaExcelEP.Add("B39", GetValoreEP("B39"));            
            
			ValoriDaExcelEA.Add("B46", GetValoreEA("B46"));
			ValoriDaExcelEP.Add("B46", GetValoreEP("B46"));

            ValoriDaExcelEA.Add("B54", GetValoreEA("B54"));
			ValoriDaExcelEP.Add("B54", GetValoreEP("B54"));
                        
			ValoriDaExcelEA.Add("B62", GetValoreEA("B62"));
			ValoriDaExcelEP.Add("B62", GetValoreEP("B62"));
            
            ValoriDaExcelEA.Add("B65", GetValoreEA("B65"));
			ValoriDaExcelEP.Add("B65", GetValoreEP("B65"));
            
			ValoriDaExcelEA.Add("B72", GetValoreEA("B72"));
			ValoriDaExcelEP.Add("B72", GetValoreEP("B72"));
                        
			ValoriDaExcelEA.Add("B77", GetValoreEA("B77"));
			ValoriDaExcelEP.Add("B77", GetValoreEP("B77"));
            			
			ValoriDaExcelEA.Add("B87", GetValoreEA("B87"));
			ValoriDaExcelEP.Add("B87", GetValoreEP("B87"));

            ValoriDaExcelEA.Add("I22", GetValoreEA("I22"));
			ValoriDaExcelEP.Add("I22", GetValoreEP("I22"));

			txtEA_1.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEA["B39"]) == 0.0) ? "" : ((double)(ValoriDaExcelEA["B12"]) / (double)(ValoriDaExcelEA["B39"])).ToString());
			txtEP_1.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEP["B39"]) == 0.0) ? "" : ((double)(ValoriDaExcelEP["B12"]) / (double)(ValoriDaExcelEP["B39"])).ToString());

            txtEA_2.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEA["B39"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B12"]) + (double)(ValoriDaExcelEA["B15"])) / (double)(ValoriDaExcelEA["B39"])).ToString());
            txtEP_2.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEP["B39"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B12"]) + (double)(ValoriDaExcelEP["B15"])) / (double)(ValoriDaExcelEP["B39"])).ToString());
            
            txtEA_3.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEA["B54"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B39"]) + (double)(ValoriDaExcelEA["B46"])) / (double)(ValoriDaExcelEA["B54"])).ToString());
			txtEP_3.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEP["B54"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B39"]) + (double)(ValoriDaExcelEP["B46"])) / (double)(ValoriDaExcelEP["B54"])).ToString());

			txtEA_4.Text = cBusinessObjects.ConvertNumber((((double)(ValoriDaExcelEA["B39"]) + (double)(ValoriDaExcelEA["B46"])) == 0.0) ? "" : ((double)(ValoriDaExcelEA["B54"]) / ((double)(ValoriDaExcelEA["B39"]) + (double)(ValoriDaExcelEA["B46"]))).ToString());
			txtEP_4.Text = cBusinessObjects.ConvertNumber((((double)(ValoriDaExcelEP["B39"]) + (double)(ValoriDaExcelEP["B46"])) == 0.0) ? "" : ((double)(ValoriDaExcelEP["B54"]) / ((double)(ValoriDaExcelEP["B39"]) + (double)(ValoriDaExcelEP["B46"]))).ToString());

			txtEA_5.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEA["B25"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B54"])) / (double)(ValoriDaExcelEA["B25"])).ToString());
			txtEP_5.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEP["B25"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B54"])) / (double)(ValoriDaExcelEP["B25"])).ToString());

			txtEA_6.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEA["B25"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B46"]) + (double)(ValoriDaExcelEA["B54"])) / (double)(ValoriDaExcelEA["B25"])).ToString());
			txtEP_6.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEP["B25"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B46"]) + (double)(ValoriDaExcelEP["B54"])) / (double)(ValoriDaExcelEP["B25"])).ToString());

			txtEA_7.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEA["I22"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B72"])) / (double)(ValoriDaExcelEA["I22"]) * 100.0).ToString()) + "%";
			txtEP_7.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEP["I22"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B72"])) / (double)(ValoriDaExcelEP["I22"]) * 100.0).ToString()) + "%";

			txtEA_8.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEA["B54"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B87"])) / (double)(ValoriDaExcelEA["B54"]) * 100.0).ToString()) + "%";
			txtEP_8.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEP["B54"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B87"])) / (double)(ValoriDaExcelEP["B54"]) * 100.0).ToString()) + "%";

			txtEA_9.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEA["B62"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B72"])) / (double)(ValoriDaExcelEA["B62"]) * 100.0).ToString()) + "%";
			txtEP_9.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEP["B62"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B72"])) / (double)(ValoriDaExcelEP["B62"]) * 100.0).ToString()) + "%";

			if (((double)(ValoriDaExcelEA["B77"])) >= 0.0)
			{
				txtEA_10.Text = "n.c.";
			}
			else //if (((double)(ValoriDaExcelEA["B72"])) / -((double)(ValoriDaExcelEA["B77"])) > 1.0)
			{
				txtEA_10.Text = cBusinessObjects.ConvertNumber((((double)(ValoriDaExcelEA["B72"])) / -((double)(ValoriDaExcelEA["B77"]))).ToString());
			}

			if (((double)(ValoriDaExcelEP["B77"])) >= 0.0)
			{
				txtEP_10.Text = "n.c.";
			}
			else
			{
				txtEP_10.Text = cBusinessObjects.ConvertNumber((((double)(ValoriDaExcelEP["B72"])) / -((double)(ValoriDaExcelEP["B77"]))).ToString());
			}
            
            if(abbreviato == true)
            {
                labelROI.Text = "R.O.I. (*)";
                NotaROI.Visibility = Visibility.Visible;

                txtEA_11.Text = "n.c.";
                txtEP_11.Text = "n.c.";

                txtEA_12.Text = "n.c.";
                txtEP_12.Text = "n.c.";
            }
            else
            {
                labelROI.Text = "R.O.I.";
                NotaROI.Visibility = Visibility.Collapsed;

                txtEA_11.Text = ConvertInteger(((double)(ValoriDaExcelEA["B62"]) == 0.0) ? "" : Math.Abs((((double)(ValoriDaExcelEA["B7"])) * 365.0 / (double)(ValoriDaExcelEA["B62"]))).ToString());
                txtEP_11.Text = ConvertInteger(((double)(ValoriDaExcelEP["B62"]) == 0.0) ? "" : Math.Abs((((double)(ValoriDaExcelEP["B7"])) * 365.0 / (double)(ValoriDaExcelEP["B62"]))).ToString());

                txtEA_12.Text = ConvertInteger(((double)(ValoriDaExcelEA["B65"]) == 0.0) ? "" : Math.Abs((((double)(ValoriDaExcelEA["B34"])) * 365.0 / (double)(ValoriDaExcelEA["B65"]))).ToString());
                txtEP_12.Text = ConvertInteger(((double)(ValoriDaExcelEP["B65"]) == 0.0) ? "" : Math.Abs((((double)(ValoriDaExcelEP["B34"])) * 365.0 / (double)(ValoriDaExcelEP["B65"]))).ToString());
            }

            txtEA_13.Text = ConvertInteger(((double)(ValoriDaExcelEA["B62"]) == 0.0) ? "" : Math.Abs((((double)(ValoriDaExcelEA["B15"])) * 365.0 / (double)(ValoriDaExcelEA["B62"]))).ToString());
			txtEP_13.Text = ConvertInteger(((double)(ValoriDaExcelEP["B62"]) == 0.0) ? "" : Math.Abs((((double)(ValoriDaExcelEP["B15"])) * 365.0 / (double)(ValoriDaExcelEP["B62"]))).ToString());

	
            dati = cBusinessObjects.GetData(id, typeof(Excel_BilancioIndici));
            if (dati.Rows.Count == 0)
                dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            foreach (DataRow dtrow in dati.Rows)
            {

                dtrow["txtEA_1"] = txtEA_1.Text;
                dtrow["txtEP_1"] = txtEP_1.Text;
                dtrow["txtEA_2"] = txtEA_2.Text;
                dtrow["txtEP_2"] = txtEP_2.Text;
                dtrow["txtEA_3"] = txtEA_3.Text;
                dtrow["txtEP_3"] = txtEP_3.Text;
                dtrow["txtEA_4"] = txtEA_4.Text;
                dtrow["txtEP_4"] = txtEP_4.Text;
                dtrow["txtEA_5"] = txtEA_5.Text;
                dtrow["txtEP_5"] = txtEP_5.Text;
                dtrow["txtEA_6"] = txtEA_6.Text;
                dtrow["txtEP_6"] = txtEP_6.Text;
                dtrow["txtEA_7"] = txtEA_7.Text;
                dtrow["txtEP_7"] = txtEP_7.Text;
                dtrow["txtEA_8"] = txtEA_8.Text;
                dtrow["txtEP_8"] = txtEP_8.Text;
                dtrow["txtEA_9"] = txtEA_9.Text;
                dtrow["txtEP_9"] = txtEP_9.Text;
                dtrow["txtEA_10"] = txtEA_10.Text;
                dtrow["txtEP_10"] = txtEP_10.Text;
                dtrow["txtEA_11"] = txtEA_11.Text;
                dtrow["txtEP_11"] = txtEP_11.Text;
                dtrow["txtEA_12"] = txtEA_12.Text;
                dtrow["txtEP_12"] = txtEP_12.Text;
                dtrow["txtEA_13"] = txtEA_13.Text;
                dtrow["txtEP_13"] = txtEP_13.Text;
            }
            cBusinessObjects.SaveData(id, dati, typeof(Excel_BilancioIndici));
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

		private void RetrieveData(XmlDataProviderManager _x, XmlDataProviderManager x_AP, string ID)
		{
           
            DataTable datiBil = cBusinessObjects.GetData(int.Parse(ID), typeof(Excel_Bilancio));
            foreach (DataRow dtrow in datiBil.Rows)
            {
   			//Calcolo valori attuali

                if ( !valoreEA.Contains(dtrow["ID"].ToString() ) )
                {
                    if (dtrow["EA"] != null )
                    {
                        valoreEA.Add(dtrow["ID"].ToString(), dtrow["EA"].ToString());
                    }
                    else
                    {
                        valoreEA.Add(dtrow["ID"].ToString(), "0" );
                    }
                }

                if ( !valoreEP.Contains(dtrow["ID"].ToString()) )
                {
                    // if ( x_AP == null || ( x_AP != null && x_AP.Document.SelectSingleNode( "/Dati//Dato[@ID='" + ID + "']/Valore[@ID='" + node.Attributes["ID"].Value + "']" ) == null ) )
                    if(true)
                    {
                        if (dtrow["EP"] != null )
                        {
                            valoreEP.Add(dtrow["ID"].ToString(), dtrow["EP"].ToString());
                        }
                        else
                        {
                            valoreEP.Add(dtrow["ID"].ToString(), "0" );
                        }
                    }
                }

				//Calcolo valori anno precedente
				if (x_AP != null)
				{
                    if ( !valoreEP.Contains(dtrow["ID"].ToString()) )
                    {
                        XmlNode tmpNode = null;
                     //   XmlNode tmpNode = x_AP.Document.SelectSingleNode( "/Dati//Dato[@ID='" + ID + "']/Valore[@ID='" + node.Attributes["ID"].Value + "']" );
                        if ( tmpNode != null )
                        {
                            if ( tmpNode.Attributes["EA"] != null )
                            {
                                valoreEP.Add(dtrow["ID"].ToString(), tmpNode.Attributes["EA"].Value );
                            }
                            else
                            {
                                valoreEP.Add(dtrow["ID"].ToString(), "0" );
                            }
                        }
                        else
                        {
                            valoreEP.Add(dtrow["ID"].ToString(), "0" );
                        }
                    }
				}
			}
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

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double newsize = e.NewSize.Width - 30.0;

			try
			{				
				foreach (UIElement item in stack.Children)
				{
					((UserControl)(((Grid)(((Border)(item)).Child)).Children[2])).Width = newsize - 30;
				}

				stack.Width = Convert.ToDouble(newsize);
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}
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
