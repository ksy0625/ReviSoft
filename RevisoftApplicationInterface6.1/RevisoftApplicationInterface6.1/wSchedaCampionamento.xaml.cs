//----------------------------------------------------------------------------+
//                        wSchedaCampionamento.xaml.cs                        |
//----------------------------------------------------------------------------+
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
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using System.Collections;
using System.ComponentModel;
using UserControls;
using System.Media;
using System.Threading;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;
using SautinSoft.Document;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace RevisoftApplication
{
    public partial class wSchedaCampionamento : System.Windows.Window
    {
        public enum TipologieCampionamento { Sconosciuto, Clienti, Fornitori, Magazzino };
        public TipologieCampionamento _tipologia = TipologieCampionamento.Sconosciuto;
        public XmlDataProviderManager RevisioneAssociata = null;

        bool Materialità_1 = false;
        bool Materialità_2 = false;
        bool Materialità_3 = false;

        string MaterialitaOperativa = "";
        string MaterialitaBilancio = "";
        string MaterialitaOperativa2 = "";
        string MaterialitaBilancio2 = "";

        string RischioIndividuazione = "";
        string attributeRischioIndividuazione = "";
        string ciclo = "";

        bool esistealmenounavoce = false;
        DataSet RawData = null;

        ArrayList ALrowsstratificate_scelte = new ArrayList();
        ArrayList ALrowsstratificate = new ArrayList();
        ArrayList ALrowsIntermediatevalue = new ArrayList();
        ArrayList CompleteListStratification = new ArrayList();
        ArrayList ALtxtTipoCampionamento_Info = new ArrayList();
        ArrayList ALtxtTotaleSaldiCampione = new ArrayList();
        ArrayList ALtxtTotaleSaldo = new ArrayList();

        DataSet FinalData = null;
        DataTable dataCampionamento = null;
        DataTable dataCampionamentoValori = null;

        public ExcelPackage excelworkBook;
        public ExcelWorksheet excelSheet;

        int lastColIncludeFormulas = 0;
        int lastRowIncludeFormulas = 0;

        Hashtable valorilabel = new Hashtable();

        List<string> colonne = new List<string>();

        int rowintestazione = 1;

        string indexcolumnsaldo = "0";
        Hashtable intervalloMIN = new Hashtable();
        Hashtable intervalloMAX = new Hashtable();
        Hashtable intervalli = new Hashtable();

        string colonneobbligatorie = "";

        private string left = "./Images/icone/wa_nav_sess_prev.png";
        private string right = "./Images/icone/wa_nav_sess_next.png";
        private string addimg = "./Images/icone/add2.png";
        private string deleteimg = "./Images/icone/close.png";

        //Datatable per tenere in memoria le relazioni del campionamento
        public int id;
        public int idt;
        private DataTable dati = null;
        public int nodeNumber = 0;



        public class ComboboxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        public XmlNode node
        {
            get
            {
                return ((XmlNode)(((WindowWorkArea)(this.Owner))._x.Document.SelectSingleNode("/Dati//Dato[@ID='" + ((XmlNode)(((WindowWorkArea)(this.Owner)).Nodes[((WindowWorkArea)(this.Owner)).NodeNow])).Attributes["ID"].Value + "']")));
            }
        }

        public wSchedaCampionamento()
        {
            if (esistealmenounavoce) { }
            InitializeComponent();
            labelTitolo.Foreground = App._arrBrushes[0];
        }

        public bool Load()
        {
            string sFinalData, sRawData;
            int idScheda = cBusinessObjects.GetIDTree(nodeNumber);

            sFinalData = string.Empty;
            sRawData = string.Empty;

            colonne.Clear();

            switch (_tipologia)
            {
                case TipologieCampionamento.Sconosciuto:
                    return false;
                case TipologieCampionamento.Clienti:
                    labelTitolo.Content = "Clienti";

                    colonne.Add("Codice");
                    colonne.Add("Descrizione");
                    colonne.Add("Progressivo DARE");
                    colonne.Add("SALDO");
                    colonne.Add("Attributo di STRATIFICAZIONE");

                    colonneobbligatorie = "0|1|3";

                    indexcolumnsaldo = "3";

                    ///HACK: clienti e fornitori erano inveriti attributeRischioIndividuazione = "txt3c";
                    attributeRischioIndividuazione = "txt2c";
                    ciclo = "Ciclo Vendite";
                    break;
                case TipologieCampionamento.Fornitori:
                    labelTitolo.Content = "Fornitori";

                    colonne.Add("Codice");
                    colonne.Add("Descrizione");
                    colonne.Add("Progressivo AVERE");
                    colonne.Add("SALDO");
                    colonne.Add("Attributo di STRATIFICAZIONE");

                    colonneobbligatorie = "0|1|3";

                    indexcolumnsaldo = "3";

                    ///HACK: clienti e fornitori erano inveriti attributeRischioIndividuazione = "txt2c";
                    attributeRischioIndividuazione = "txt3c";
                    ciclo = "Ciclo Acquisti";
                    break;
                case TipologieCampionamento.Magazzino:
                    labelTitolo.Content = "Rimanenze di Magazzino";

                    colonne.Add("Codice");
                    colonne.Add("Descrizione");
                    colonne.Add("Unità di misura");
                    colonne.Add("Totale CARICO");
                    colonne.Add("Totale SCARICO");
                    colonne.Add("Quantità GIACENTE");
                    colonne.Add("VALORE quantità giacente");
                    colonne.Add("Attributo di STRATIFICAZIONE");

                    colonneobbligatorie = "0|1|5|6";

                    indexcolumnsaldo = "6";

                    attributeRischioIndividuazione = "txt4c";
                    ciclo = "Ciclo Magazzino";

                    btnCircolarizzazione.Visibility = Visibility.Hidden;
                    DataCircolarizzazione.Visibility = Visibility.Collapsed;
                    stpCircolarizzazione.Visibility = Visibility.Collapsed;
                    break;
                default:
                    return false;
            }

            GetValoriEsterni();
            //salvataggio relazioni campionamento

            //node.Attributes["FinalData"] = datifrom.Rows[0];
            //node.Attributes["RawData"] = datifrom.Rows[1];
            //LC
            dataCampionamento = cBusinessObjects.GetData(nodeNumber, typeof(Campionamento));
            if (dataCampionamento.Rows.Count < 1)
                dataCampionamento.Rows.Add(idScheda, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
            dataCampionamentoValori = cBusinessObjects.GetData(nodeNumber, typeof(CampionamentoValori));

            //  cBusinessObjects.SaveData(int.Parse(IDHere), datifrom, typeof(Campionamento));


            //if (node != null && node.Attributes["FinalData"] != null && node.Attributes["FinalData"].Value != "<NewDataSet />")
            sFinalData = dataCampionamento.Rows[0]["FinalData"].ToString();
            sRawData = dataCampionamento.Rows[0]["RawData"].ToString();
            if (!string.IsNullOrEmpty(sFinalData) && sFinalData != "<NewDataSet />")
            {
                if (!(((WindowWorkArea)(this.Owner)).ReadOnly))
                {
                    wAlertCampionamento wsa = new wAlertCampionamento();
                    wsa.ShowDialog();
                }

                //using (StringReader sw = new StringReader(node.Attributes["FinalData"].Value))
                using (StringReader sw = new StringReader(sFinalData))
                {
                    FinalData = new DataSet();
                    FinalData.ReadXml(sw);
                }

                if (!string.IsNullOrEmpty(sRawData))
                //if (node != null && node.Attributes["RawData"] != null)
                {
                    //using (StringReader sw = new StringReader(node.Attributes["RawData"].Value))
                    using (StringReader sw = new StringReader(sRawData))
                    {
                        RawData = new DataSet();
                        RawData.ReadXml(sw);
                    }
                }
            }
            else
            {
                if ((((WindowWorkArea)(this.Owner)).ReadOnly))
                {
                    MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
                    this.Close();
                }

                FinalData = null;

                //if (node != null && node.Attributes["RawData"] != null)
                if (!string.IsNullOrEmpty(sRawData))
                {
                    //using (StringReader sw = new StringReader(node.Attributes["RawData"].Value))
                    using (StringReader sw = new StringReader(sRawData))
                    {
                        RawData = new DataSet();
                        RawData.ReadXml(sw);
                    }
                }
                else
                {
                    RawData = null;

                    Utilities u = new Utilities();
                    string Nomefile = u.sys_OpenFileDialog("", App.TipoFile.BilancioDiVerifica);

                    if (Nomefile == null || Nomefile == "")
                    {
                        this.Close();
                        return false;
                    }

                    if (node.Attributes["Nomefile"] == null)
                    {
                        XmlAttribute attr = node.OwnerDocument.CreateAttribute("Nomefile");
                        node.Attributes.Append(attr);
                    }

                    node.Attributes["Nomefile"].Value = Nomefile;
                    dataCampionamento.Rows[0]["NomeFile"] = Nomefile;


                    //excel.Calculation = Microsoft.Office.Interop.Excel.XlCalculation.xlCalculationManual;
                    var file = new FileInfo(Nomefile);
                    excelworkBook = new ExcelPackage(file);

                    if (excelworkBook.Compatibility.IsWorksheets1Based)
                        excelSheet = excelworkBook.Workbook.Worksheets[1];
                    else
                        excelSheet = excelworkBook.Workbook.Worksheets[0];
                }
            }

            CreateInterface();

            return true;
        }

        private void GetValoriEsterni()
        {
            //try
            {
                string IDRischioGlobale = "22";

                string ID_Materialità_1 = "77";
                string ID_Materialità_2 = "78";
                string ID_Materialità_3 = "199";

                MaterialitaOperativa = "";
                MaterialitaBilancio = "";
                MaterialitaOperativa2 = "";
                MaterialitaBilancio2 = "";

                RischioIndividuazione = "";

                //DataRow tmpNode = null;

                string rbtTipoMaterialitaPianificata1 = "";



                string idsessionedatimaterialita = cBusinessObjects.CercaSessione("Bilancio", "Revisione", cBusinessObjects.idsessione.ToString(), cBusinessObjects.idcliente);

                DataTable datimaterialita = cBusinessObjects.GetData(int.Parse(ID_Materialità_1), typeof(Excel_LimiteMaterialitaSPCE), cBusinessObjects.idcliente, int.Parse(idsessionedatimaterialita), 1);


                if (datimaterialita.Rows.Count > 0)
                {
                    string statomat = "";
                    DataTable statom = cBusinessObjects.GetData(int.Parse(ID_Materialità_1), typeof(StatoNodi), cBusinessObjects.idcliente, int.Parse(idsessionedatimaterialita), 1);

                    foreach (DataRow dd in statom.Rows)
                    {
                        statomat = dd["Stato"].ToString().Trim();
                    }
                    if (datimaterialita.Rows.Count > 0 && statomat != "" && ((App.TipoTreeNodeStato)(Convert.ToInt32(statomat))) == App.TipoTreeNodeStato.Completato)
                    {
                        Materialità_1 = true;


                    }

                    if (!Materialità_1)
                    {
                        datimaterialita = cBusinessObjects.GetData(int.Parse(ID_Materialità_2), typeof(Excel_LimiteMaterialitaSPCE), cBusinessObjects.idcliente, int.Parse(idsessionedatimaterialita), 1);

                        statom = cBusinessObjects.GetData(int.Parse(ID_Materialità_2), typeof(StatoNodi), cBusinessObjects.idcliente, int.Parse(idsessionedatimaterialita), 1);
                        foreach (DataRow dd in statom.Rows)
                        {
                            statomat = dd["Stato"].ToString().Trim();
                        }
                        if (datimaterialita.Rows.Count > 0 && statomat != "" && ((App.TipoTreeNodeStato)(Convert.ToInt32(statomat))) == App.TipoTreeNodeStato.Completato)
                        {
                            Materialità_2 = true;
                        }
                    }

                    if (!Materialità_1 && !Materialità_2)
                    {
                        datimaterialita = cBusinessObjects.GetData(int.Parse(ID_Materialità_3), typeof(Excel_LimiteMaterialitaSPCE), cBusinessObjects.idcliente, int.Parse(idsessionedatimaterialita), 1);
                        statom = cBusinessObjects.GetData(int.Parse(ID_Materialità_3), typeof(StatoNodi), cBusinessObjects.idcliente, int.Parse(idsessionedatimaterialita), 1);
                        foreach (DataRow dd in statom.Rows)
                        {
                            statomat = dd["Stato"].ToString().Trim();
                        }
                        if (datimaterialita.Rows.Count > 0 && statomat != "" && ((App.TipoTreeNodeStato)(Convert.ToInt32(statomat))) == App.TipoTreeNodeStato.Completato)
                        {
                            Materialità_3 = true;
                        }
                    }
                }



                if (Materialità_1 || Materialità_2 || Materialità_3)
                {
                    foreach (DataRow dtrow in datimaterialita.Rows)
                    {
                        if (dtrow["ID"].ToString() != "rbtTipoMaterialitaPianificata1")
                            continue;

                        if (dtrow["value"].ToString() == "True")
                        {
                            rbtTipoMaterialitaPianificata1 = "True";
                        }
                    }

                    foreach (DataRow dtrow in datimaterialita.Rows)
                    {
                        if (rbtTipoMaterialitaPianificata1 == "True")
                        {
                            if (dtrow["ID"].ToString() == "txt12")
                                MaterialitaOperativa = ((dtrow["value"].ToString() == "") ? "" : dtrow["value"].ToString());
                            if (dtrow["ID"].ToString() == "txt12BILANCIO")
                                MaterialitaBilancio = ((dtrow["value"].ToString() == "") ? "" : dtrow["value"].ToString());
                        }
                        else
                        {
                            if (dtrow["ID"].ToString() == "txt12_2sp")
                                MaterialitaOperativa = ((dtrow["value"].ToString() == "") ? "" : dtrow["value"].ToString());
                            if (dtrow["ID"].ToString() == "txt12_2ce")
                                MaterialitaOperativa2 = ((dtrow["value"].ToString() == "") ? "" : dtrow["value"].ToString());
                            if (dtrow["ID"].ToString() == "txt12_2spBILANCIO")
                                MaterialitaBilancio = ((dtrow["value"].ToString() == "") ? "" : dtrow["value"].ToString());
                            if (dtrow["ID"].ToString() == "txt12_2ceBILANCIO")
                                MaterialitaBilancio2 = ((dtrow["value"].ToString() == "") ? "" : dtrow["value"].ToString());

                        }

                    }
                    /*
                        else if (Materialità_2)
                        {
                            MaterialitaOperativa = ((tmpNode_true.Attributes["txt12_2sp"] == null) ? "" : tmpNode_true.Attributes["txt12_2sp"].Value);
                            MaterialitaOperativa2 = ((tmpNode_true.Attributes["txt12_2ce"] == null) ? "" : tmpNode_true.Attributes["txt12_2ce"].Value);

                            MaterialitaBilancio = ((tmpNode_true.Attributes["txt12_2spBILANCIO"] == null) ? "" : tmpNode_true.Attributes["txt12_2spBILANCIO"].Value);
                            MaterialitaBilancio2 = ((tmpNode_true.Attributes["txt12_2ceBILANCIO"] == null) ? "" : tmpNode_true.Attributes["txt12_2ceBILANCIO"].Value);
                        }
                        else if (Materialità_3)
                        {
                            MaterialitaOperativa = ((tmpNode_true.Attributes["txt12_3sp"] == null) ? "" : tmpNode_true.Attributes["txt12_3sp"].Value);
                            MaterialitaOperativa2 = ((tmpNode_true.Attributes["txt12_3ce"] == null) ? "" : tmpNode_true.Attributes["txt12_3ce"].Value);

                            MaterialitaBilancio = ((tmpNode_true.Attributes["txt12_3spBILANCIO"] == null) ? "" : tmpNode_true.Attributes["txt12_3spBILANCIO"].Value);
                            MaterialitaBilancio2 = ((tmpNode_true.Attributes["txt12_3ceBILANCIO"] == null) ? "" : tmpNode_true.Attributes["txt12_3ceBILANCIO"].Value);
                        }*/

                }


                DataTable datiRishcioGlobale = cBusinessObjects.GetData(int.Parse(IDRischioGlobale), typeof(RischioGlobale), cBusinessObjects.idcliente, int.Parse(idsessionedatimaterialita), 1);


                foreach (DataRow node in datiRishcioGlobale.Rows)
                {
                    RischioIndividuazione = ((node[attributeRischioIndividuazione].ToString() == "") ? "" : node[attributeRischioIndividuazione].ToString());
                }




            }
        }

        //------------------------------------------------------------------------+
        //                            CreateInterface                             |
        //------------------------------------------------------------------------+
        private void CreateInterface()
        {
            int i, irow, j;
            List<string> donehere;
            string attributo, intervallo;
            stpSelezioneEstrapolazione.Visibility = Visibility.Collapsed;
            stpSelezioneEstrapolazione_btn.Visibility = Visibility.Collapsed;
            stpSelezioneCampione.Visibility = Visibility.Collapsed;
            stpSelezioneCampione_btn.Visibility = Visibility.Collapsed;
            switch (_tipologia)
            {
                case TipologieCampionamento.Clienti:
                    labelTitolo.Content = "Clienti";
                    break;
                case TipologieCampionamento.Fornitori:
                    labelTitolo.Content = "Fornitori";
                    break;
                case TipologieCampionamento.Magazzino:
                    labelTitolo.Content = "Rimanenze di Magazzino";
                    break;
            }
            stpFinal.Visibility = Visibility.Collapsed;
            stpFinal_btn.Visibility = Visibility.Collapsed;
            stpMotivazione.Visibility = Visibility.Collapsed;
            stpMotivazione_btn.Visibility = Visibility.Collapsed;
            valorilabel = new Hashtable();

            #region riga intestazione
            TextBlock lbl = new TextBlock();
            Button btn = new Button();
            StackPanel stpIntestazione = new StackPanel();
            stpIntestazione.Visibility = (RawData == null && FinalData == null) ?
              Visibility.Visible : Visibility.Collapsed;
            stpIntestazione.Margin = new Thickness(10);
            stpIntestazione.Orientation = System.Windows.Controls.Orientation.Vertical;
            stpIntestazione.Name = "stpIntestazione";
            try { this.UnregisterName(stpIntestazione.Name); }
            catch (Exception) { }
            this.RegisterName(stpIntestazione.Name, stpIntestazione);

            StackPanel stpRiga_1 = new StackPanel();
            stpRiga_1.Orientation = System.Windows.Controls.Orientation.Vertical;
            StackPanel stpRiga_label = new StackPanel();
            stpRiga_label.Orientation = System.Windows.Controls.Orientation.Vertical;
            StackPanel stptext = new StackPanel();
            stptext.Orientation = System.Windows.Controls.Orientation.Horizontal;
            lbl = new TextBlock();
            lbl.Text = "Il file XLS ";
            stptext.Children.Add(lbl);
            lbl = new TextBlock();
            lbl.FontWeight = FontWeights.Bold;
            lbl.Text = "dovrà contenere un solo foglio.";
            stptext.Children.Add(lbl);
            stpRiga_label.Children.Add(stptext);

            lbl = new TextBlock();
            lbl.Text = "Specifiche caratteristiche /attributi degli item " +
              "(clienti Italia, Estero, ecc..) dovranno essere segnalati " +
              "in una specifica colonna.";
            stpRiga_label.Children.Add(lbl);
            lbl = new TextBlock();
            lbl.Margin = new Thickness(0, 15, 0, 0);
            lbl.FontWeight = FontWeights.Bold;
            lbl.Text = "Fase 1) Selezionare la riga che contiene la " +
              "descrizione delle colonne.";
            stpRiga_label.Children.Add(lbl);
            lbl = new TextBlock();
            lbl.Text = "Selezionare la riga che indica il contenuto delle " +
              "colonne del file XLS da importare (";
            for (i = 0; i < colonne.Count; i++)
            {
                if (i != 0) lbl.Text += ", ";
                lbl.Text += colonne[i];
            }
            lbl.Text += ")";
            stpRiga_label.Children.Add(lbl);

            lbl = new TextBlock();
            lbl.Text = "Il software non importerà le righe che precedono " +
              "quella selezionata.";
            stpRiga_label.Children.Add(lbl);
            stpRiga_1.Children.Add(stpRiga_label);
            irow = 1;
            lastColIncludeFormulas = 1;
            lastRowIncludeFormulas = 20;
            if (RawData == null && FinalData == null)
            {
                irow = excelSheet.Dimension.End.Row;
                lastColIncludeFormulas = excelSheet.Dimension.End.Column;
                lastRowIncludeFormulas = excelSheet.Dimension.End.Row;
            }

            ComboBox lst = new ComboBox();
            lst.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            lst.Margin = new Thickness(10);
            lst.Items.Clear();
            lst.Name = "lst_Intestazione";
            for (i = 1; i <= lastRowIncludeFormulas; i++)
                lst.Items.Add(i.ToString());
            lst.SelectionChanged += Lst_SelectionChanged;
            try { this.UnregisterName(lst.Name); }
            catch (Exception) { }
            this.RegisterName(lst.Name, lst);

            ScrollViewer sw = new ScrollViewer();
            sw.MaxWidth = 1000.0;
            sw.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            StackPanel stpRiga_2 = new StackPanel();
            stpRiga_2.Name = "Riga2";
            stpRiga_2.Orientation = System.Windows.Controls.Orientation.Horizontal;
            try { this.UnregisterName(stpRiga_2.Name); }
            catch (Exception) { }
            this.RegisterName(stpRiga_2.Name, stpRiga_2);
            sw.Content = stpRiga_2;
            stpRiga_1.Children.Add(lst);
            stpIntestazione.Children.Add(stpRiga_1);
            stpIntestazione.Children.Add(sw);
            stackPanel1.Children.Add(stpIntestazione);
            #endregion

            #region riga scelta colonne
            StackPanel stpScelte = new StackPanel();
            if (RawData == null && FinalData == null)
            {
                stpScelte.Visibility = Visibility.Visible;
                stp1_btn.Visibility = Visibility.Visible;
            }
            else
            {
                stpScelte.Visibility = Visibility.Collapsed;
                stp1_btn.Visibility = Visibility.Collapsed;
            }
            stpScelte.Margin = new Thickness(10);
            stpScelte.Name = "stpScelte";
            try { this.UnregisterName(stpScelte.Name); }
            catch (Exception) { }
            this.RegisterName(stpScelte.Name, stpScelte);

            stpRiga_label = new StackPanel();
            stpRiga_label.Orientation = System.Windows.Controls.Orientation.Vertical;
            lbl = new TextBlock();
            lbl.Text = "Fase 2) Selezionare le colonne.";
            lbl.FontWeight = FontWeights.Bold;
            stpRiga_label.Children.Add(lbl);

            lbl = new TextBlock();
            lbl.Text = "Indicare a fianco di ciascuna voce sottostante, la " +
              "LETTERA della COLONNA del file XLS contenente i dati da " +
              "importare con dell’apposita tendina.";
            stpRiga_label.Children.Add(lbl);
            stpScelte.Children.Add(stpRiga_label);

            for (i = 0; i < colonne.Count; i++)
            {
                StackPanel stpRiga_colonna = new StackPanel();
                stpRiga_colonna.Margin = new Thickness(0, 10, 0, 0);
                stpRiga_colonna.Orientation =
                  System.Windows.Controls.Orientation.Horizontal;
                lbl = new TextBlock();
                lbl.Text = colonne[i];
                lbl.Name = "lblcolumn_" + i.ToString();
                lbl.Width = 200;
                try { this.UnregisterName(lbl.Name); }
                catch (Exception) { }
                this.RegisterName(lbl.Name, lbl);
                if (i == (colonne.Count - 1))
                {
                    //if (node.Attributes["stpStratificazionesino"] != null && node.Attributes["stpStratificazionesino"].Value == "SI")
                    if (EsisteAttributo("stpStratificazionesino")
                      && GetRigaAttributo("stpStratificazionesino") == "SI")
                        lbl.Visibility = Visibility.Visible;
                    else lbl.Visibility = Visibility.Collapsed;
                    StackPanel stpStratificazionesino = new StackPanel();
                    stpStratificazionesino.Name = "stpStratificazionesino";
                    try { this.UnregisterName(stpStratificazionesino.Name); }
                    catch (Exception) { }
                    this.RegisterName(stpStratificazionesino.Name, stpStratificazionesino);
                    stpStratificazionesino.Orientation =
                      System.Windows.Controls.Orientation.Horizontal;
                    stpStratificazionesino.Margin = new Thickness(0, 40, 0, 0);

                    TextBlock txtstratif = new TextBlock();
                    txtstratif.Text = "Vuoi eseguire stratificazioni?";
                    txtstratif.FontWeight = FontWeights.Bold;
                    stpStratificazionesino.Children.Add(txtstratif);
                    RadioButton rdbstratifSi = new RadioButton();
                    rdbstratifSi.Content = "SI";
                    rdbstratifSi.GroupName = "Stratificazione";
                    /*-----------------------------------------------------------------------------
                              if (node.Attributes["stpStratificazionesino"] != null
                                && node.Attributes["stpStratificazionesino"].Value == "SI")
                    -----------------------------------------------------------------------------*/
                    if (EsisteAttributo("stpStratificazionesino")
                      && GetRigaAttributo("stpStratificazionesino") == "SI")
                        rdbstratifSi.IsChecked = true;
                    else rdbstratifSi.IsChecked = false;
                    rdbstratifSi.Checked += RdbstratifSi_Checked;
                    rdbstratifSi.Margin = new Thickness(10, 0, 0, 0);
                    stpStratificazionesino.Children.Add(rdbstratifSi);
                    RadioButton rdbstratifNo = new RadioButton();
                    rdbstratifNo.Content = "NO";
                    rdbstratifNo.GroupName = "Stratificazione";
                    /*-----------------------------------------------------------------------------
                              if (node.Attributes["stpStratificazionesino"] != null
                                && node.Attributes["stpStratificazionesino"].Value != "NO")
                    -----------------------------------------------------------------------------*/
                    if (EsisteAttributo("stpStratificazionesino")
                      && GetRigaAttributo("stpStratificazionesino") != "NO")
                    {
                        rdbstratifNo.IsChecked = false;
                    }
                    else
                    {
                        rdbstratifNo.IsChecked = true;
                        if (node.Attributes["stpStratificazionesino"] == null)
                        {
                            XmlAttribute attr = node.OwnerDocument.CreateAttribute("stpStratificazionesino");
                            node.Attributes.Append(attr);
                        }
                        node.Attributes["stpStratificazionesino"].Value = "NO";
                        SetRigaAttributo("stpStratificazionesino", "NO");
                    }
                    rdbstratifNo.Margin = new Thickness(10, 0, 0, 0);
                    rdbstratifNo.Checked += RdbstratifNo_Checked;
                    stpStratificazionesino.Children.Add(rdbstratifNo);
                    stpScelte.Children.Add(stpStratificazionesino);

                    TextBlock txthere4 = new TextBlock();
                    txthere4.Text = "Se la stratificazione deve essere eseguita " +
                      "anche per attributi, sceglierli tramite la tendina " +
                      "sottostante e procedere con AVANTI.";
                    txthere4.Name = "txthere4";
                    txthere4.Visibility = Visibility.Collapsed;
                    try { this.UnregisterName(txthere4.Name); }
                    catch (Exception) { }
                    this.RegisterName(txthere4.Name, txthere4);
                    stpScelte.Children.Add(txthere4);
                    txthere4 = new TextBlock();
                    txthere4.Text = "Se si vuole stratificare SOLO per intervalli " +
                      "monetari, non indicare gli attributi, e procedere con AVANTI.";
                    txthere4.Name = "txthere5";
                    txthere4.Visibility = Visibility.Collapsed;
                    try { this.UnregisterName(txthere4.Name); }
                    catch (Exception) { }
                    this.RegisterName(txthere4.Name, txthere4);
                    stpScelte.Children.Add(txthere4);
                }
                stpRiga_colonna.Children.Add(lbl);

                ComboBox lst_colonna = new ComboBox();
                lst_colonna.Name = "lst_" + i.ToString();
                lst_colonna.SelectionChanged += Lst_Colonna_SelectionChanged;
                lst_colonna.Width = 200;
                lst_colonna.Margin = new Thickness(10, 0, 0, 0);
                try { this.UnregisterName(lst_colonna.Name); }
                catch (Exception) { }
                this.RegisterName(lst_colonna.Name, lst_colonna);
                lst_colonna.Items.Clear();
                if (i == colonne.Count - 1)
                {
                    /*-----------------------------------------------------------------------------
                              if (node.Attributes["stpStratificazionesino"] != null
                                && node.Attributes["stpStratificazionesino"].Value == "SI")
                    -----------------------------------------------------------------------------*/
                    if (EsisteAttributo("stpStratificazionesino")
                      && GetRigaAttributo("stpStratificazionesino") == "SI")
                        lst_colonna.Visibility = Visibility.Visible;
                    else lst_colonna.Visibility = Visibility.Collapsed;
                }
                stpRiga_colonna.Children.Add(lst_colonna);
                if (i == colonne.Count - 1)
                {
                    TextBlock txt_lst = new TextBlock();
                    txt_lst.Text = "Tipo Attributo";
                    txt_lst.Name = "txt_lst";
                    txt_lst.Margin = new Thickness(10, 0, 10, 0);
                    /*-----------------------------------------------------------------------------
                              if (node.Attributes["stpStratificazionesino"] != null
                                && node.Attributes["stpStratificazionesino"].Value == "SI")
                    -----------------------------------------------------------------------------*/
                    if (EsisteAttributo("stpStratificazionesino")
                      && GetRigaAttributo("stpStratificazionesino") == "SI")
                        txt_lst.Visibility = Visibility.Visible;
                    else txt_lst.Visibility = Visibility.Collapsed;
                    try { this.UnregisterName(txt_lst.Name); }
                    catch (Exception) { }
                    this.RegisterName(txt_lst.Name, txt_lst);
                    stpRiga_colonna.Children.Add(txt_lst);

                    ComboBox lst_attr = new ComboBox();
                    lst_attr.Name = "lst_attr";
                    lst_attr.Width = 150;
                    lst_attr.Margin = new Thickness(10, 0, 0, 0);
                    /*-----------------------------------------------------------------------------
                              if (node.Attributes["stpStratificazionesino"] != null
                                && node.Attributes["stpStratificazionesino"].Value == "SI")
                    -----------------------------------------------------------------------------*/
                    if (EsisteAttributo("stpStratificazionesino")
                      && GetRigaAttributo("stpStratificazionesino") == "SI")
                        lst_attr.Visibility = Visibility.Visible;
                    else lst_attr.Visibility = Visibility.Collapsed;
                    try { this.UnregisterName(lst_attr.Name); }
                    catch (Exception) { }
                    this.RegisterName(lst_attr.Name, lst_attr);
                    lst_attr.Items.Clear();
                    lst_attr.Items.Add("Aree geografiche");
                    lst_attr.Items.Add("Canali distributivi");
                    lst_attr.Items.Add("Settori merceologici");
                    lst_attr.Items.Add("Vetustà del credito");
                    lst_attr.Items.Add("Unità di misura");
                    lst_attr.Items.Add("Altri");
                    stpRiga_colonna.Children.Add(lst_attr);

                    TextBox txt_attr = new TextBox();
                    txt_attr.Name = "txt_attr";
                    txt_attr.Width = 250;
                    txt_attr.Margin = new Thickness(10, 0, 0, 0);
                    /*-----------------------------------------------------------------------------
                              if (node.Attributes["stpStratificazionesino"] != null
                                && node.Attributes["stpStratificazionesino"].Value == "SI")
                    -----------------------------------------------------------------------------*/
                    if (EsisteAttributo("stpStratificazionesino")
                      && GetRigaAttributo("stpStratificazionesino") == "SI")
                        txt_attr.Visibility = Visibility.Visible;
                    else txt_attr.Visibility = Visibility.Collapsed;
                    try { this.UnregisterName(txt_attr.Name); }
                    catch (Exception) { }
                    this.RegisterName(txt_attr.Name, txt_attr);
                    stpRiga_colonna.Children.Add(txt_attr);
                }
                stpScelte.Children.Add(stpRiga_colonna);
            }

            StackPanel stpBottoni = new StackPanel();
            stpBottoni.Orientation = System.Windows.Controls.Orientation.Horizontal;
            btn = new Button();
            btn.Name = "FirstIndietroBTN";
            btn.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            btn.Margin = new Thickness(30, 5, 5, 5);
            btn.Padding = new Thickness(5);

            StackPanel imgsuggerimenti = new StackPanel();
            Image img2 = new Image();
            img2.Width = 16;
            img2.Height = 16;
            Uri uriSource2 = new Uri("./Images/icone/gomma16.png", UriKind.Relative);
            img2.Source = new BitmapImage(uriSource2);
            imgsuggerimenti.Children.Add(img2);
            TextBlock txtSuggerimenti = new TextBlock();
            txtSuggerimenti.Text = "Cancella tutto e Chiudi";
            imgsuggerimenti.Children.Add(txtSuggerimenti);
            btn.Content = imgsuggerimenti;
            btn.Visibility = Visibility.Collapsed;
            btn.Click += Btn_Back_Cestino2_Click;
            try { this.UnregisterName(btn.Name); }
            catch (Exception) { }
            this.RegisterName(btn.Name, btn);
            stpBottoni.Children.Add(btn);

            btn = new Button();
            btn.Name = "FirstSalvaAvantiBTN";
            btn.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            btn.Margin = new Thickness(5);
            btn.Padding = new Thickness(10);
            StackPanel stpk2 = new StackPanel();
            stpk2.Orientation = System.Windows.Controls.Orientation.Horizontal;
            stpk2.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            TextBlock txtk2 = new TextBlock();
            txtk2.Margin = new Thickness(0, 0, 0, 5);
            txtk2.Text = "Avanti";
            stpk2.Children.Add(txtk2);

            img2 = new Image();
            img2.Margin = new Thickness(10, 0, 0, 0);
            img2.Width = 16;
            img2.Height = 16;
            uriSource2 = new Uri("./Images/icone/wa_nav_sess_next.png", UriKind.Relative);
            img2.Source = new BitmapImage(uriSource2);
            stpk2.Children.Add(img2);
            btn.Content = stpk2;
            btn.Click += Btn_Next_SceltaColonne_Click;
            try { this.UnregisterName(btn.Name); }
            catch (Exception) { }
            this.RegisterName(btn.Name, btn);
            stpBottoni.Children.Add(btn);

            btn = new Button();
            btn.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            btn.Margin = new Thickness(30, 5, 5, 5);
            btn.Padding = new Thickness(5);

            imgsuggerimenti = new StackPanel();
            img2 = new Image();
            img2.Width = 16;
            img2.Height = 16;
            uriSource2 = new Uri("./Images/icone/door2.png", UriKind.Relative);
            img2.Source = new BitmapImage(uriSource2);
            imgsuggerimenti.Children.Add(img2);
            txtSuggerimenti = new TextBlock();
            txtSuggerimenti.Text = "Esci Senza Salvare";
            imgsuggerimenti.Children.Add(txtSuggerimenti);
            btn.Content = imgsuggerimenti;
            btn.Click += Btn_Esci_Click;
            stpBottoni.Children.Add(btn);
            btn = new Button();
            btn.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            btn.Margin = new Thickness(30, 5, 5, 5);
            btn.Padding = new Thickness(5);

            imgsuggerimenti = new StackPanel();
            img2 = new Image();
            img2.Width = 16;
            img2.Height = 16;
            uriSource2 = new Uri("./Images/icone/Guida.png", UriKind.Relative);
            img2.Source = new BitmapImage(uriSource2);
            imgsuggerimenti.Children.Add(img2);
            txtSuggerimenti = new TextBlock();
            txtSuggerimenti.Text = "Suggerimenti";
            imgsuggerimenti.Children.Add(txtSuggerimenti);
            btn.Content = imgsuggerimenti;
            btn.Click += OpenSuggerimenti;
            stpBottoni.Children.Add(btn);
            stp1_btn.Children.Add(stpBottoni);
            stackPanel1.Children.Add(stpScelte);
            #endregion

            #region Cestino
            StackPanel stpCestino = new StackPanel();
            stpCestino.Width = 1100.0;
            stpCestino.Height = 680.0;
            stpCestino.Orientation = System.Windows.Controls.Orientation.Vertical;
            stpCestino.Name = "stpCestino";
            stpCestino.Margin = new Thickness(10);
            try { this.UnregisterName(stpCestino.Name); }
            catch (Exception) { }
            this.RegisterName(stpCestino.Name, stpCestino);

            lbl = new TextBlock();
            lbl.Text = "Fase 3) Righe da non importare.";
            lbl.Margin = new Thickness(5, 5, 5, 0);
            lbl.FontWeight = FontWeights.Bold;
            lbl.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            stpCestino.Children.Add(lbl);

            lbl = new TextBlock();
            lbl.MaxWidth = 1000.0;
            lbl.TextAlignment = TextAlignment.Left;
            lbl.TextWrapping = TextWrapping.Wrap;
            lbl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            lbl.Text = "Nell'apposita finestra di sinistra appariranno le " +
              "righe del file XLS da importare.";
            lbl.Margin = new Thickness(5, 5, 5, 0);
            stpCestino.Children.Add(lbl);

            lbl = new TextBlock();
            lbl.Text = "Con un tick nell'apposita check box, anche con " +
              "selezione multipla, dovranno essere selezionate le righe " +
              "contenenti dati da non importare (totali, sub totali, e tutto " +
              "ciò che non costituisce una voce efficace) e trasferite nel " +
              "cestino, agendo sull'apposito comando.";
            lbl.MaxWidth = 1000.0;
            lbl.TextAlignment = TextAlignment.Left;
            lbl.TextWrapping = TextWrapping.Wrap;
            lbl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            lbl.Margin = new Thickness(5, 5, 5, 0);
            stpCestino.Children.Add(lbl);

            lbl = new TextBlock();
            lbl.Text = "In caso di errore si potrà selezionare la riga dal " +
              "cestino e reimmetterla nell'elenco originario.";
            lbl.MaxWidth = 1000.0;
            lbl.TextAlignment = TextAlignment.Left;
            lbl.TextWrapping = TextWrapping.Wrap;
            lbl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            lbl.Margin = new Thickness(5, 5, 5, 0);
            stpCestino.Children.Add(lbl);

            StackPanel stpBVCestino = new StackPanel();
            stpBVCestino.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            stpBVCestino.Orientation = System.Windows.Controls.Orientation.Horizontal;

            StackPanel stp = new StackPanel();
            stp.Margin = new Thickness(0, 0, 10, 0);
            stp.Orientation = System.Windows.Controls.Orientation.Vertical;

            lbl = new TextBlock();
            lbl.Text = "Righe del file XLS.";
            lbl.Margin = new Thickness(5, 5, 5, 0);
            lbl.FontWeight = FontWeights.Bold;
            stp.Children.Add(lbl);
            System.Windows.Controls.Border brd = new System.Windows.Controls.Border();
            brd.Margin = new Thickness(5);
            brd.BorderThickness = new Thickness(1);
            brd.BorderBrush = Brushes.Black;
            brd.Background = Brushes.White;
            ScrollViewer sw2 = new ScrollViewer();
            sw2.BorderBrush = Brushes.Black;
            sw2.BorderThickness = new Thickness(1);
            sw2.Name = "sw_ElencoBVCestino";
            try { this.UnregisterName(sw2.Name); }
            catch (Exception) { }
            this.RegisterName(sw2.Name, sw2);
            sw2.Width = 500;
            sw2.Height = 460;
            brd.Child = sw2;
            stp.Children.Add(brd);
            stpBVCestino.Children.Add(stp);

            stp = new StackPanel();
            stp.Orientation = System.Windows.Controls.Orientation.Vertical;
            stp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Button btnpassaggio = new Button();
            Image img = new Image();
            btnpassaggio.Margin = new Thickness(5);
            var uriSource = new Uri(right, UriKind.Relative);
            btnpassaggio.ToolTip = "Porta Le voci Selezionate a SINISTRA nel CESTINO.";
            img.Source = new BitmapImage(uriSource);
            btnpassaggio.Content = img;
            btnpassaggio.Click += BtnpassaggioVersoCestino_Click;
            stp.Children.Add(btnpassaggio);
            btnpassaggio = new Button();
            btnpassaggio.Margin = new Thickness(5);
            img = new Image();
            uriSource = new Uri(left, UriKind.Relative);
            btnpassaggio.ToolTip = "Porta Le voci Selezionate a DESTRA fuori " +
              "dal CESTINO.";
            img.Source = new BitmapImage(uriSource);
            btnpassaggio.Content = img;
            btnpassaggio.Click += BtnpassaggioDaCestino_Click;
            stp.Children.Add(btnpassaggio);
            stpBVCestino.Children.Add(stp);

            stp = new StackPanel();
            stp.Orientation = System.Windows.Controls.Orientation.Vertical;

            lbl = new TextBlock();
            lbl.Text = "Cestino - Righe non importate.";
            lbl.Margin = new Thickness(5);
            lbl.FontWeight = FontWeights.Bold;
            stp.Children.Add(lbl);
            brd = new System.Windows.Controls.Border();
            brd.Margin = new Thickness(5);
            brd.BorderThickness = new Thickness(1);
            brd.BorderBrush = Brushes.Black;
            brd.Background = Brushes.White;
            ScrollViewer sw4 = new ScrollViewer();
            sw4.Name = "sw_ElencoAssociazioniCestino";
            try { this.UnregisterName(sw4.Name); }
            catch (Exception) { }
            this.RegisterName(sw4.Name, sw4);
            sw4.Height = 460;
            sw4.Width = 500;
            brd.Child = sw4;
            stp.Children.Add(brd);
            stpBVCestino.Children.Add(stp);
            stpCestino.Children.Add(stpBVCestino);

            stp = new StackPanel();
            stp.Orientation = System.Windows.Controls.Orientation.Horizontal;
            stp.Margin = new Thickness(5);

            StackPanel stpBottoni2 = new StackPanel();
            stpBottoni2.Orientation = System.Windows.Controls.Orientation.Horizontal;
            btn = new Button();
            btn.Margin = new Thickness(5);
            btn.Padding = new Thickness(5);
            StackPanel stpk = new StackPanel();
            stpk.Orientation = System.Windows.Controls.Orientation.Horizontal;
            stpk.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            img = new Image();
            img.Width = 16;
            img.Height = 16;
            uriSource = new Uri("./Images/icone/wa_nav_sess_prev.png", UriKind.Relative);
            img.Source = new BitmapImage(uriSource);
            stpk.Children.Add(img);

            TextBlock txtk = new TextBlock();
            txtk.Margin = new Thickness(5, 0, 0, 0);
            txtk.Text = "Indietro";
            stpk.Children.Add(txtk);
            btn.Content = stpk;
            btn.Click += Btn_Back_Cestino_Click;
            stpBottoni2.Children.Add(btn);
            btn = new Button();
            stpk = new StackPanel();
            stpk.Orientation = System.Windows.Controls.Orientation.Horizontal;
            stpk.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtk = new TextBlock();
            txtk.Margin = new Thickness(0, 0, 0, 5);
            txtk.Text = "Avanti";
            stpk.Children.Add(txtk);
            img = new Image();
            img.Margin = new Thickness(10, 0, 0, 0);
            img.Width = 16;
            img.Height = 16;
            uriSource = new Uri("./Images/icone/wa_nav_sess_next.png", UriKind.Relative);
            img.Source = new BitmapImage(uriSource);
            stpk.Children.Add(img);
            btn.Content = stpk;
            btn.Click += Btn_Next_Cestino_Click;
            btn.Margin = new Thickness(5);
            btn.Padding = new Thickness(10);
            stpBottoni2.Children.Add(btn);
            btn = new Button();
            btn.Margin = new Thickness(30, 5, 5, 5);
            btn.Padding = new Thickness(5);

            imgsuggerimenti = new StackPanel();
            img2 = new Image();
            img2.Width = 16;
            img2.Height = 16;
            uriSource2 = new Uri("./Images/icone/door2.png", UriKind.Relative);
            img2.Source = new BitmapImage(uriSource2);
            imgsuggerimenti.Children.Add(img2);
            txtSuggerimenti = new TextBlock();
            txtSuggerimenti.Text = "Esci Senza Salvare";
            imgsuggerimenti.Children.Add(txtSuggerimenti);
            btn.Content = imgsuggerimenti;
            btn.Click += Btn_Esci_Click;
            btn.Margin = new Thickness(30, 5, 5, 5);
            btn.Padding = new Thickness(10);
            stpBottoni2.Children.Add(btn);
            btn = new Button();
            btn.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            btn.Margin = new Thickness(30, 5, 5, 5);
            btn.Padding = new Thickness(5);

            imgsuggerimenti = new StackPanel();
            img2 = new Image();
            img2.Width = 16;
            img2.Height = 16;
            uriSource2 = new Uri("./Images/icone/Guida.png", UriKind.Relative);
            img2.Source = new BitmapImage(uriSource2);
            imgsuggerimenti.Children.Add(img2);
            txtSuggerimenti = new TextBlock();
            txtSuggerimenti.Text = "Suggerimenti";
            imgsuggerimenti.Children.Add(txtSuggerimenti);
            btn.Content = imgsuggerimenti;
            btn.Click += OpenSuggerimenti;
            stpBottoni2.Children.Add(btn);
            stp2_btn.Children.Add(stpBottoni2);
            stpCestino.Children.Add(stp);
            stpCestino.Visibility = Visibility.Collapsed;
            stp2_btn.Visibility = Visibility.Collapsed;
            stackPanel1.Children.Add(stpCestino);
            if (FinalData == null)
            {
                if (RawData == null)
                {
                    stpCestino.Visibility = Visibility.Collapsed;
                    stp2_btn.Visibility = Visibility.Collapsed;
                }
                else
                {
                    stpCestino.Visibility = Visibility.Visible;
                    stp2_btn.Visibility = Visibility.Visible;
                    VisualizzaListaDaAssociare_Cestino();
                    VisualizzaListaAssociate_Cestino();
                }
            }
            #endregion

            if (FinalData != null)
            {
                tabFinal.Items.Clear();
                donehere = new List<string>();
                for (j = 0; j < FinalData.Tables[0].Rows.Count; j++)
                {
                    if (donehere.Contains(FinalData.Tables[0].Rows[j][0].ToString()))
                        continue;
                    donehere.Add(FinalData.Tables[0].Rows[j][0].ToString());
                    attributo = FinalData.Tables[0].Rows[j][0].ToString().Split('|')[0];
                    intervallo = FinalData.Tables[0].Rows[j][0].ToString().Split('|')[1];
                    TabItem ti = new TabItem();
                    ti.MinWidth = 150.0;
                    ti.Background = Brushes.LightGoldenrodYellow;
                    if (attributo == "")
                    {
                        ti.Header = string.IsNullOrEmpty(intervallo) ?
                          "Nessuna Stratificazione" : intervallo;
                    }
                    else
                    {
                        ti.Header = (intervallo == "") ?
                          attributo : attributo + " - " + intervallo;
                    }
                    ti.Tag = attributo + "|" + intervallo;
                    tabFinal.Items.Add(ti);
                }
                tabFinal.SelectedIndex = 0;
                CreateFinal();
            }
        }

        //------------------------------------------------------------------------+
        //                          RdbstratifNo_Checked                          |
        //------------------------------------------------------------------------+
        private void RdbstratifNo_Checked(object sender, RoutedEventArgs e)
        {
            StackPanel stphere = (StackPanel)this.FindName("stpStratificazionesino");

            TextBlock txthere5 = (TextBlock)this.FindName("txthere5");
            txthere5.Visibility = Visibility.Collapsed;
            TextBlock txthere4 = (TextBlock)this.FindName("txthere4");
            txthere4.Visibility = Visibility.Collapsed;

            ComboBox lst_attr = (ComboBox)this.FindName("lst_attr");
            TextBox txt_attr = (TextBox)this.FindName("txt_attr");
            ComboBox lstStratif = (ComboBox)this.FindName("lst_" + (colonne.Count - 1).ToString());
            TextBlock txt_lst = (TextBlock)this.FindName("txt_lst");

            TextBlock lbl_attr = (TextBlock)this.FindName("lblcolumn_" + (colonne.Count - 1).ToString());

            lst_attr.Visibility = Visibility.Collapsed;
            txt_attr.Visibility = Visibility.Collapsed;
            lstStratif.Visibility = Visibility.Collapsed;
            lbl_attr.Visibility = Visibility.Collapsed;
            txt_lst.Visibility = Visibility.Collapsed;

            if (node.Attributes["stpStratificazionesino"] == null)
            {
                XmlAttribute attr = node.OwnerDocument.CreateAttribute("stpStratificazionesino");
                node.Attributes.Append(attr);
            }

            node.Attributes["stpStratificazionesino"].Value = "NO";
            SetRigaAttributo("stpStratificazionesino", "NO");
        }

        //------------------------------------------------------------------------+
        //                          RdbstratifSi_Checked                          |
        //------------------------------------------------------------------------+
        private void RdbstratifSi_Checked(object sender, RoutedEventArgs e)
        {
            StackPanel stphere = (StackPanel)this.FindName("stpStratificazionesino");

            ComboBox lst_attr = (ComboBox)this.FindName("lst_attr");
            TextBox txt_attr = (TextBox)this.FindName("txt_attr");
            TextBlock txt_lst = (TextBlock)this.FindName("txt_lst");
            ComboBox lstStratif = (ComboBox)this.FindName("lst_" + (colonne.Count - 1).ToString());

            TextBlock lbl_attr = (TextBlock)this.FindName("lblcolumn_" + (colonne.Count - 1).ToString());

            TextBlock txthere5 = (TextBlock)this.FindName("txthere5");
            txthere5.Visibility = Visibility.Visible;
            TextBlock txthere4 = (TextBlock)this.FindName("txthere4");
            txthere4.Visibility = Visibility.Visible;

            lst_attr.Visibility = Visibility.Visible;
            txt_attr.Visibility = Visibility.Visible;
            lstStratif.Visibility = Visibility.Visible;
            lbl_attr.Visibility = Visibility.Visible;
            txt_lst.Visibility = Visibility.Visible;

            if (node.Attributes["stpStratificazionesino"] == null)
            {
                XmlAttribute attr = node.OwnerDocument.CreateAttribute("stpStratificazionesino");
                node.Attributes.Append(attr);
            }

            node.Attributes["stpStratificazionesino"].Value = "SI";
            SetRigaAttributo("stpStratificazionesino", "SI");
        }

        #region Bottoni Next / Prev

        private void Btn_Next_Cestino_Click(object sender, RoutedEventArgs e)
        {
            int i;

            if (RawData == null) return;
            for (i = 0; i < RawData.Tables[0].Rows.Count; i++)
            {
                System.Windows.Controls.CheckBox chkda =
                  (System.Windows.Controls.CheckBox)
                    this.FindName("chkCestinoDa_" + i.ToString());
                System.Windows.Controls.CheckBox chka =
                  (System.Windows.Controls.CheckBox)
                    this.FindName("chkCestinoA_" + i.ToString());
                if ((chkda != null && chkda.IsChecked == true && chkda.IsVisible == true)
                  || (chka != null && chka.IsChecked == true && chka.IsVisible == true))
                {
                    MessageBox.Show("Non hai trasferito nel cestino (o viceversa) " +
                      "la o le righe selezionate.");
                    return;
                }
            }

            StackPanel stpCestino = (StackPanel)this.FindName("stpCestino");
            stpCestino.Visibility = Visibility.Collapsed;
            stp2_btn.Visibility = Visibility.Collapsed;

            /*-----------------------------------------------------------------------------
                  if (node.Attributes["stpStratificazionesino"] != null
                    && node.Attributes["stpStratificazionesino"].Value == "SI")
            -----------------------------------------------------------------------------*/
            if (EsisteAttributo("stpStratificazionesino")
              && GetRigaAttributo("stpStratificazionesino") == "SI")
            {
                stpIntervalliMonetari.Visibility = Visibility.Visible;
                stpIntervalliMonetari_btn.Visibility = Visibility.Visible;
                CreaIntervalliMonetari("");
            }
            else
            {
                CreaFirstStep();
                stpMotivazione.Visibility = Visibility.Visible;
                stpMotivazione_btn.Visibility = Visibility.Visible;
            }
        }

        //------------------------------------------------------------------------+
        //                         Btn_Back_Cestino_Click                         |
        //------------------------------------------------------------------------+
        private void Btn_Back_Cestino_Click(object sender, RoutedEventArgs e)
        {
            int i;
            string str;

            StackPanel stpIntestazione = (StackPanel)this.FindName("stpIntestazione");
            stpIntestazione.Visibility = Visibility.Visible;

            StackPanel stpScelte = (StackPanel)this.FindName("stpScelte");
            stpScelte.Visibility = Visibility.Visible;
            stp1_btn.Visibility = Visibility.Visible;

            StackPanel stpCestino = (StackPanel)this.FindName("stpCestino");
            stpCestino.Visibility = Visibility.Collapsed;
            stp2_btn.Visibility = Visibility.Collapsed;

            Button btnIndietro = (Button)this.FindName("FirstIndietroBTN");
            btnIndietro.Visibility = Visibility.Visible;

            Button btnSA = (Button)this.FindName("FirstSalvaAvantiBTN");
            btnSA.Click -= Btn_Next_SceltaColonne_Click;
            btnSA.Click += Btn_Next_SceltaColonne2_Click;

            ComboBox lst_Intestazione = (ComboBox)this.FindName("lst_Intestazione");

            if (lst_Intestazione != null)
            {
                //if (node.Attributes["lst_Intestazione"] != null)
                if (EsisteAttributo("lst_Intestazione"))
                {
                    str = GetRigaAttributo("lst_Intestazione");
                    lst_Intestazione.Items.Clear();
                    //lst_Intestazione.Items.Add(node.Attributes["lst_Intestazione"].Value);
                    lst_Intestazione.Items.Add(str);
                    //lst_Intestazione.SelectedValue = node.Attributes["lst_Intestazione"].Value;
                    lst_Intestazione.SelectedValue = str;
                    lst_Intestazione.IsReadOnly = true;
                }
            }

            for (i = 0; i < colonne.Count; i++)
            {
                ComboBox lsthere = (ComboBox)this.FindName("lst_" + i.ToString());
                if (lsthere != null)
                {
                    //if (node.Attributes["lst_" + i.ToString()] != null)
                    str = string.Format("lst_{0}", i);
                    if (EsisteAttributo(str))
                    {
                        lsthere.Items.Clear();
                        //lsthere.Items.Add(node.Attributes["lst_" + i.ToString()].Value);
                        lsthere.Items.Add(GetRigaAttributo(str));
                        //lsthere.SelectedValue = node.Attributes["lst_" + i.ToString()].Value;
                        lsthere.SelectedValue = GetRigaAttributo(str);
                        lsthere.IsReadOnly = true;
                    }
                }
            }

            ComboBox lst_attr = (ComboBox)this.FindName("lst_attr");
            if (lst_attr != null)
            {
                //if (node.Attributes["lst_attr"] != null)
                if (EsisteAttributo("lst_attr"))
                {
                    lst_attr.Items.Clear();
                    //lst_attr.Items.Add(node.Attributes["lst_attr"].Value);
                    //lst_attr.SelectedValue = node.Attributes["lst_attr"].Value;
                    str = GetRigaAttributo("lst_attr");
                    lst_attr.Items.Add(str);
                    lst_attr.SelectedValue = str;
                    lst_attr.IsReadOnly = true;
                }
            }

            TextBox txt_attr = (TextBox)this.FindName("txt_attr");
            if (lst_attr != null)
            {
                //if (node.Attributes["txt_attr"] != null)
                if (EsisteAttributo("txt_attr"))
                {
                    //txt_attr.Text = node.Attributes["txt_attr"].Value;
                    txt_attr.Text = GetRigaAttributo("txt_attr");
                    txt_attr.IsReadOnly = true;
                }
            }

            ComboBox lstStratif = (ComboBox)this.FindName("lst_" + (colonne.Count - 1).ToString());
            TextBlock lbl_attr = (TextBlock)this.FindName("lblcolumn_" + (colonne.Count - 1).ToString());
            /*-----------------------------------------------------------------------------
                  if (node.Attributes["stpStratificazionesino"] != null
                    && node.Attributes["stpStratificazionesino"].Value == "SI")
            -----------------------------------------------------------------------------*/
            if (EsisteAttributo("stpStratificazionesino")
              && GetRigaAttributo("stpStratificazionesino") == "SI")
            {
                lst_attr.Visibility = Visibility.Visible;
                txt_attr.Visibility = Visibility.Visible;
                lstStratif.Visibility = Visibility.Visible;
                lbl_attr.Visibility = Visibility.Visible;
            }
        }

        //------------------------------------------------------------------------+
        //                        Btn_Back_Cestino2_Click                         |
        //------------------------------------------------------------------------+
        private void Btn_Back_Cestino2_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Attenzione, se si torna indietro da questo " +
              "punto si cancellano tutte le attività già svolte. Significa che " +
              "il campionamento e l'eventuale circolarizzazione svolte vanno " +
              "perdute e rifatte. Procedere?", "Attenzione", MessageBoxButton.OKCancel)
              == MessageBoxResult.Cancel) return;
            ButtonDeleteRaw_Click(sender, e);
        }

        //------------------------------------------------------------------------+
        //                             Btn_Esci_Click                             |
        //------------------------------------------------------------------------+
        private void Btn_Esci_Click(object sender, RoutedEventArgs e)
        {
            cBusinessObjects.SaveData(
              nodeNumber, dataCampionamentoValori, typeof(CampionamentoValori));
            this.Close();
        }

        private void Btn_Next_SceltaColonne_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < colonne.Count - 1; i++)
            {
                if (colonneobbligatorie.Contains(i.ToString()))
                {
                    ComboBox lst = (ComboBox)this.FindName("lst_" + i.ToString());
                    if (lst.SelectedIndex == 0 || lst.SelectedIndex == -1)
                    {
                        MessageBox.Show("Selezionare " + colonne[i]);
                        return;
                    }
                }
            }

            StackPanel stpIntestazione = (StackPanel)this.FindName("stpIntestazione");
            stpIntestazione.Visibility = Visibility.Collapsed;

            StackPanel stpScelte = (StackPanel)this.FindName("stpScelte");
            stpScelte.Visibility = Visibility.Collapsed;
            stp1_btn.Visibility = Visibility.Collapsed;

            StackPanel stpCestino = (StackPanel)this.FindName("stpCestino");
            stpCestino.Visibility = Visibility.Visible;
            stp2_btn.Visibility = Visibility.Visible;

            GetDataFromExcel(true);

            VisualizzaListaDaAssociare_Cestino();
            VisualizzaListaAssociate_Cestino();
        }

        private void Btn_Next_SceltaColonne2_Click(object sender, RoutedEventArgs e)
        {
            StackPanel stpIntestazione = (StackPanel)this.FindName("stpIntestazione");
            stpIntestazione.Visibility = Visibility.Collapsed;

            StackPanel stpScelte = (StackPanel)this.FindName("stpScelte");
            stpScelte.Visibility = Visibility.Collapsed;
            stp1_btn.Visibility = Visibility.Collapsed;

            StackPanel stpCestino = (StackPanel)this.FindName("stpCestino");
            stpCestino.Visibility = Visibility.Visible;
            stp2_btn.Visibility = Visibility.Visible;

            VisualizzaListaDaAssociare_Cestino();
            VisualizzaListaAssociate_Cestino();
        }

        private void Btn_Back_IntervalliMonetari_Click(object sender, RoutedEventArgs e)
        {
            stpIntervalliMonetari.Visibility = Visibility.Collapsed;
            stpIntervalliMonetari_btn.Visibility = Visibility.Collapsed;

            StackPanel stpCestino = (StackPanel)this.FindName("stpCestino");
            stpCestino.Visibility = Visibility.Visible;
            stp2_btn.Visibility = Visibility.Visible;

            VisualizzaListaDaAssociare_Cestino();
            VisualizzaListaAssociate_Cestino();
        }

        private void Btn_Next_IntervalliMonetari_Click(object sender, RoutedEventArgs e)
        {
            stpIntervalliMonetari.Visibility = Visibility.Collapsed;
            stpIntervalliMonetari_btn.Visibility = Visibility.Collapsed;

            CreaFirstStep();

            stpMotivazione.Visibility = Visibility.Visible;
            stpMotivazione_btn.Visibility = Visibility.Visible;
        }

        //------------------------------------------------------------------------+
        //                       Btn_Back_Motivazioni_Click                       |
        //------------------------------------------------------------------------+
        private void Btn_Back_Motivazioni_Click(object sender, RoutedEventArgs e)
        {
            stpMotivazione.Visibility = Visibility.Collapsed;
            stpMotivazione_btn.Visibility = Visibility.Collapsed;
            /*-----------------------------------------------------------------------------
                  if (node.Attributes["stpStratificazionesino"] != null
                    && node.Attributes["stpStratificazionesino"].Value == "SI")
            -----------------------------------------------------------------------------*/
            if (EsisteAttributo("stpStratificazionesino")
              && GetRigaAttributo("stpStratificazionesino") == "SI")
            {
                stpIntervalliMonetari.Visibility = Visibility.Visible;
                stpIntervalliMonetari_btn.Visibility = Visibility.Visible;
                CreaIntervalliMonetari("");
            }
            else
            {
                StackPanel stpCestino = (StackPanel)this.FindName("stpCestino");
                stpCestino.Visibility = Visibility.Visible;
                stp2_btn.Visibility = Visibility.Visible;
                VisualizzaListaDaAssociare_Cestino();
                VisualizzaListaAssociate_Cestino();
            }
        }

        //------------------------------------------------------------------------+
        //                       Btn_Next_Motivazioni_Click                       |
        //------------------------------------------------------------------------+
        private void Btn_Next_Motivazioni_Click(object sender, RoutedEventArgs e)
        {
            stpMotivazione.Visibility = Visibility.Collapsed;
            stpMotivazione_btn.Visibility = Visibility.Collapsed;

            TextRange tr = new TextRange(mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);
            MemoryStream ms = new MemoryStream();
            tr.Save(ms, DataFormats.Rtf);
            string xamlText = ASCIIEncoding.Default.GetString(ms.ToArray());

            if (node.Attributes["Motivazioni"] == null)
            {
                XmlAttribute attr = node.OwnerDocument.CreateAttribute("Motivazioni");
                node.Attributes.Append(attr);
            }

            node.Attributes["Motivazioni"].Value = xamlText.Replace("\\f1", "\\f0").Replace("\\f2", "\\f0").Replace("\\f3", "\\f0").Replace("{\\f0\\fcharset0 Times New Roman;}", "{\\f0 Arial;\\f1 Wingdings 2;\\f2 Wingdings;}");
            SetRigaAttributo("Motivazioni", xamlText.Replace("\\f1",
              "\\f0").Replace("\\f2", "\\f0").Replace("\\f3",
              "\\f0").Replace("{\\f0\\fcharset0 Times New Roman;}",
              "{\\f0 Arial;\\f1 Wingdings 2;\\f2 Wingdings;}"));

            if (node.Attributes["Scelta"] == null)
            {
                XmlAttribute attr = node.OwnerDocument.CreateAttribute("Scelta");
                node.Attributes.Append(attr);
            }

            if (rdbRagionato.IsChecked == true)
            {
                node.Attributes["Scelta"].Value = "Ragionato";
                SetRigaAttributo("Scelta", "Ragionato");
            }

            if (rdbCasuale.IsChecked == true)
            {
                node.Attributes["Scelta"].Value = "Casuale";
                SetRigaAttributo("Scelta", "Casuale");
            }

            if (rdbMUS.IsChecked == true)
            {
                node.Attributes["Scelta"].Value = "MUS";
                SetRigaAttributo("Scelta", "MUS");
            }

          ((WindowWorkArea)(this.Owner))._x.Save();

            /*-----------------------------------------------------------------------------
                  if (node.Attributes["stpStratificazionesino"] != null
                    && node.Attributes["stpStratificazionesino"].Value == "SI")
            -----------------------------------------------------------------------------*/
            if (EsisteAttributo("stpStratificazionesino")
              && GetRigaAttributo("stpStratificazionesino") == "SI")
            {
                stpSelezioneCampione.Visibility = Visibility.Visible;
                stpSelezioneCampione_btn.Visibility = Visibility.Visible;
                CreaStratificazione();
            }
            else
            {
                CompleteListStratification.Clear();
                CompleteListStratification.Add("" + "|" + "");
                stpSelezioneEstrapolazione.Visibility = Visibility.Visible;
                stpSelezioneEstrapolazione_btn.Visibility = Visibility.Visible;
                tabCampionamento_Calculate();
                tabCampionamento.SelectedIndex = 0;
            }
        }

        private void Btn_Back_Stratificazioni_Click(object sender, RoutedEventArgs e)
        {
            stpSelezioneCampione.Visibility = Visibility.Collapsed;
            stpSelezioneCampione_btn.Visibility = Visibility.Collapsed;
            stpMotivazione.Visibility = Visibility.Visible;
            stpMotivazione_btn.Visibility = Visibility.Visible;
            CreaFirstStep();
        }

        //------------------------------------------------------------------------+
        //                     Btn_Next_Stratificazioni_Click                     |
        //------------------------------------------------------------------------+
        private void Btn_Next_Stratificazioni_Click(object sender, RoutedEventArgs e)
        {
            string completelist, str;
            List<string> AttributiChosen = new List<string>();
            List<string> IntervalliChosen = new List<string>();
            int i;

            if (node == null) return;
            if (node.Attributes["Stratificazioni_Attributo"] == null)
            {
                XmlAttribute attr = node.OwnerDocument.CreateAttribute(
                  "Stratificazioni_Attributo");
                node.Attributes.Append(attr);
            }
            completelist = "";
            foreach (System.Windows.Controls.CheckBox item
              in stpStratificazione_Attributo_Interna.Children)
            {
                if (item.IsChecked == true)
                {
                    completelist += ((completelist == "") ? "" : "|") + item.Tag.ToString();
                    AttributiChosen.Add(item.Tag.ToString());
                }
            }

            node.Attributes["Stratificazioni_Attributo"].Value = completelist;
            SetRigaAttributo("Stratificazioni_Attributo", completelist);

            if (completelist == "")
            {
                if (node.Attributes["Stratificazioni_Intervalli"] == null)
                {
                    XmlAttribute attr = node.OwnerDocument.CreateAttribute(
                      "Stratificazioni_Intervalli");
                    node.Attributes.Append(attr);
                }

                completelist = ""; // inutile

                foreach (System.Windows.Controls.CheckBox item
                  in stpStratificazione_Intervalli_Interna.Children)
                {
                    if (item.IsChecked == true)
                    {
                        completelist += ((completelist == "") ? "" : "|") + item.Content.ToString();
                        IntervalliChosen.Add(item.Content.ToString());
                    }
                }

                node.Attributes["Stratificazioni_Intervalli"].Value = completelist;
                SetRigaAttributo("Stratificazioni_Intervalli", completelist);
            }
            else
            {
                if (node.Attributes["Stratificazioni_Intervalli"] != null)
                {
                    node.Attributes.Remove(node.Attributes["Stratificazioni_Intervalli"]);
                }
                RemoveRigaAttributo("Stratificazioni_Intervalli");
            }

            stpSelezioneEstrapolazione.Visibility = Visibility.Visible;
            stpSelezioneEstrapolazione_btn.Visibility = Visibility.Visible;
            stpSelezioneCampione.Visibility = Visibility.Collapsed;
            stpSelezioneCampione_btn.Visibility = Visibility.Collapsed;

            txtTipoCampionamento.Text = "Campionamento ";

            //if (node.Attributes["Scelta"] != null)
            if (EsisteAttributo("Scelta"))
            {
                //txtTipoCampionamento.Text += node.Attributes["Scelta"].Value;
                txtTipoCampionamento.Text += GetRigaAttributo("Scelta");
            }

            CompleteListStratification.Clear();
            for (i = 0; i < 1000; i++)
            {
                str = string.Format("CompleteListStratification_{0}", i);
                if (node != null && node.Attributes[str] != null)
                {
                    node.Attributes.Remove(node.Attributes[str]);
                }
                RemoveRigaAttributo(str);
            }

            if (AttributiChosen.Count > 0)
            {
                foreach (string attributo in AttributiChosen)
                {
                    if (!CompleteListStratification.Contains(
                      ((attributo.Contains('|')) ? attributo : attributo + "|" + "")))
                    {
                        CompleteListStratification.Add(
                          ((attributo.Contains('|')) ? attributo : attributo + "|" + ""));
                    }
                }
            }
            else
            {
                if (IntervalliChosen.Count > 0)
                {
                    foreach (string intervallo in IntervalliChosen)
                    {
                        if (!CompleteListStratification.Contains("" + "|" + intervallo))
                        {
                            CompleteListStratification.Add("" + "|" + intervallo);
                        }
                    }
                }
                else
                {
                    if (!CompleteListStratification.Contains("" + "|" + ""))
                    {
                        CompleteListStratification.Add("" + "|" + "");
                    }
                }
            }

            tabCampionamento_Calculate();

            ((WindowWorkArea)(this.Owner))._x.Save();

            tabCampionamento.SelectedIndex = 0;
        }

        //------------------------------------------------------------------------+
        //                             Chk_Unchecked                              |
        //------------------------------------------------------------------------+
        private void Chk_Unchecked(object sender, RoutedEventArgs e)
        {
            string str, str2;
            TabItem ti;
            List<int> rowsstratificate_scelte;

            ti = (TabItem)(tabCampionamento.SelectedItem);
            rowsstratificate_scelte =
              (List<int>)(ALrowsstratificate_scelte[
                CompleteListStratification.IndexOf(ti.Tag.ToString())]);
            rowsstratificate_scelte.Remove(
              Convert.ToInt32(((System.Windows.Controls.CheckBox)sender).Tag.ToString()));

            str = string.Format("rowsstratificate_scelte_{0}",
              CompleteListStratification.IndexOf(ti.Tag.ToString()));

            if (node.Attributes[str] == null)
            {
                XmlAttribute attr = node.OwnerDocument.CreateAttribute(str);
                node.Attributes.Append(attr);
            }

            node.Attributes[str].Value = "";
            str2 = string.Empty;
            foreach (int item in rowsstratificate_scelte)
            {
                if (node.Attributes[
                  "rowsstratificate_scelte_" +
                  CompleteListStratification.IndexOf(ti.Tag.ToString())].Value != "")
                {
                    node.Attributes[
                      "rowsstratificate_scelte_" +
                      CompleteListStratification.IndexOf(ti.Tag.ToString())].Value += "|";
                }
                if (!string.IsNullOrEmpty(str2)) str2 += "|";
                str2 += item.ToString();
                node.Attributes[
                  "rowsstratificate_scelte_" +
                  CompleteListStratification.IndexOf(ti.Tag.ToString())].Value +=
                    item.ToString();
            }
            SetRigaAttributo(str, str2);
        }

        //------------------------------------------------------------------------+
        //                              Chk_Checked                               |
        //------------------------------------------------------------------------+
        private void Chk_Checked(object sender, RoutedEventArgs e)
        {
            List<int> rowsstratificate_scelte;
            string str, str2;
            TabItem ti;

            ti = (TabItem)(tabCampionamento.SelectedItem);
            rowsstratificate_scelte =
              (List<int>)(ALrowsstratificate_scelte[
                CompleteListStratification.IndexOf(ti.Tag.ToString())]);
            rowsstratificate_scelte.Add(
              Convert.ToInt32(((System.Windows.Controls.CheckBox)sender).Tag.ToString()));

            str = string.Format("rowsstratificate_scelte_{0}",
                CompleteListStratification.IndexOf(ti.Tag.ToString()));
            if (node.Attributes[str] == null)
            {
                XmlAttribute attr = node.OwnerDocument.CreateAttribute(str);
                node.Attributes.Append(attr);
                SetRigaAttributo(str, "");
            }

            node.Attributes[str].Value = "";
            str2 = string.Empty;
            foreach (int item in rowsstratificate_scelte)
            {
                if (node.Attributes[str].Value != "")
                    node.Attributes[str].Value += "|";
                node.Attributes[str].Value += item.ToString();
                if (!string.IsNullOrEmpty(str2)) str2 += "|";
                str2 += item.ToString();
            }
            SetRigaAttributo(str2, str);
        }

        //------------------------------------------------------------------------+
        //                             Btn_Back_Final                             |
        //------------------------------------------------------------------------+
        private void Btn_Back_Final(object sender, RoutedEventArgs e)
        {
            stpSelezioneEstrapolazione.Visibility = Visibility.Collapsed;
            stpSelezioneEstrapolazione_btn.Visibility = Visibility.Collapsed;
            /*-----------------------------------------------------------------------------
                  if (node.Attributes["stpStratificazionesino"] != null
                    && node.Attributes["stpStratificazionesino"].Value == "SI")
            -----------------------------------------------------------------------------*/
            if (EsisteAttributo("stpStratificazionesino")
              && GetRigaAttributo("stpStratificazionesino") == "SI")
            {
                stpSelezioneCampione.Visibility = Visibility.Visible;
                stpSelezioneCampione_btn.Visibility = Visibility.Visible;
                CreaStratificazione();
            }
            else
            {
                CreaFirstStep();
                stpMotivazione.Visibility = Visibility.Visible;
                stpMotivazione_btn.Visibility = Visibility.Visible;
            }
        }

        //------------------------------------------------------------------------+
        //                             Btn_Next_Final                             |
        //------------------------------------------------------------------------+
        private void Btn_Next_Final(object sender, RoutedEventArgs e)
        {
            double dbl, saldoscelto, totalesaldoscelte;
            int i;
            List<int> rowsstratificate_scelte;
            List<string> tmparray;
            string result, str;

            stpSelezioneEstrapolazione.Visibility = Visibility.Collapsed;
            stpSelezioneEstrapolazione_btn.Visibility = Visibility.Collapsed;
            switch (_tipologia)
            {
                case TipologieCampionamento.Clienti:
                    labelTitolo.Content = "Clienti";
                    break;
                case TipologieCampionamento.Fornitori:
                    labelTitolo.Content = "Fornitori";
                    break;
                case TipologieCampionamento.Magazzino:
                    labelTitolo.Content = "Rimanenze di Magazzino";
                    break;
            }
            labelTitolo.Content += "  -  Campioni estratti per la rilevazione " +
              "degli errori";
            stpFinal.Visibility = Visibility.Visible;
            stpFinal_btn.Visibility = Visibility.Visible;
            FinalData = null;
            tabFinal.SelectionChanged -= tabFinal_SelectionChanged;
            try
            {
                if (tabFinal.Items.Count > 0) tabFinal.Items.Clear();
                if (tabFinal.Items.Count == 0)
                {
                    foreach (TabItem tabitem in tabCampionamento.Items)
                    {
                        TabItem ti = new TabItem();
                        ti.MinWidth = 150.0;
                        ti.Background = Brushes.LightGoldenrodYellow;
                        ti.Header = tabitem.Header;
                        ti.Tag = tabitem.Tag;
                        tabFinal.Items.Add(ti);
                    }
                }
            }
            catch (Exception) { }
            tabFinal.SelectionChanged += tabFinal_SelectionChanged;
            FinalData = new DataSet();
            System.Data.DataTable dataTable = new System.Data.DataTable();
            dataTable.TableName = "dataTable";
            dataTable.Columns.Add("Stratificazione");
            dataTable.Columns.Add("Codice");
            dataTable.Columns.Add("Descrizione");
            switch (_tipologia)
            {
                case TipologieCampionamento.Clienti:
                case TipologieCampionamento.Fornitori:
                    dataTable.Columns.Add("Saldo");
                    dataTable.Columns.Add("Esito");
                    break;
                case TipologieCampionamento.Magazzino:
                    dataTable.Columns.Add("Q.tà Giacente");
                    dataTable.Columns.Add("Valore");
                    break;
            }
            dataTable.Columns.Add("Errore Rilevato");
            switch (_tipologia)
            {
                case TipologieCampionamento.Clienti:
                case TipologieCampionamento.Fornitori:
                    dataTable.Columns.Add("Indirizzo");
                    break;
                case TipologieCampionamento.Magazzino:
                    break;
            }
            totalesaldoscelte = 0.0;
            saldoscelto = 0.0;
            ALtxtTotaleSaldiCampione.Clear();
            foreach (TabItem tabitem in tabCampionamento.Items)
            {
                rowsstratificate_scelte =
                  (List<int>)(ALrowsstratificate_scelte[
                    CompleteListStratification.IndexOf(tabitem.Tag.ToString())]);
                totalesaldoscelte = 0.0;
                foreach (int item in rowsstratificate_scelte)
                {
                    tmparray = new List<string>();
                    tmparray.Add(tabitem.Tag.ToString()); //Codice
                    if (RawData != null)
                    {
                        tmparray.Add(RawData.Tables[0].Rows[item][0].ToString()); //Codice
                        tmparray.Add(RawData.Tables[0].Rows[item][1].ToString()); //Descrizione
                        switch (_tipologia)
                        {
                            case TipologieCampionamento.Clienti:
                            case TipologieCampionamento.Fornitori:
                                tmparray.Add(RawData.Tables[0].Rows[item][3].ToString()); //Saldo
                                saldoscelto = 0;
                                double.TryParse(RawData.Tables[0].Rows[item][3].ToString(), out saldoscelto);
                                totalesaldoscelte += saldoscelto;
                                tmparray.Add("(Selezionare)"); //Esito
                                break;
                            case TipologieCampionamento.Magazzino:
                                tmparray.Add(RawData.Tables[0].Rows[item][5].ToString()); //Q.tà Giacente
                                tmparray.Add(RawData.Tables[0].Rows[item][6].ToString()); //Valore
                                saldoscelto = 0;
                                double.TryParse(RawData.Tables[0].Rows[item][6].ToString(), out saldoscelto);
                                totalesaldoscelte += saldoscelto;
                                break;
                        }
                        tmparray.Add(""); //Errore Rilevato
                        dataTable.Rows.Add(tmparray.ToArray());
                    }
                }
                ALtxtTotaleSaldiCampione.Add(totalesaldoscelte);
            }
            FinalData.Tables.Add(dataTable);
            using (StringWriter sw = new StringWriter())
            {
                FinalData.WriteXml(sw);
                result = sw.ToString();
            }
            if (node != null)
            {
                if (node.Attributes["FinalData"] == null)
                {
                    XmlAttribute attr = node.OwnerDocument.CreateAttribute("FinalData");
                    node.Attributes.Append(attr);
                }
                node.Attributes["FinalData"].Value = result;
                dataCampionamento.Rows[0]["FinalData"] = result;
            }
            for (i = 0; i < ALtxtTotaleSaldiCampione.Count; i++)
            {
                str = string.Format("ALtxtTotaleSaldiCampione_{0}", i);
                if (node.Attributes[str] == null)
                {
                    XmlAttribute attr = node.OwnerDocument.CreateAttribute(str);
                    node.Attributes.Append(attr);
                }
                dbl = (double)ALtxtTotaleSaldiCampione[i];
                node.Attributes[str].Value = dbl.ToString();
                SetRigaAttributo(str, dbl.ToString());
            }
          ((WindowWorkArea)(this.Owner))._x.Save();
            CreateFinal();
        }

        #endregion

        #region Riga Intestazione

        //------------------------------------------------------------------------+
        //                          Lst_SelectionChanged                          |
        //------------------------------------------------------------------------+
        private void Lst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i, jj;
            string valuehere;

            rowintestazione = Convert.ToInt32(((string)(((ComboBox)sender).SelectedItem)));
            StackPanel stphere = (StackPanel)this.FindName("Riga2");
            if (stphere == null) return;
            stphere.Children.Clear();
            stphere.CanHorizontallyScroll = true;

            ComboboxItem item = new ComboboxItem();
            item.Text = "(Selezionare)";
            item.Value = "0";

            for (i = 0; i < colonne.Count; i++)
            {
                ComboBox lsthere = (ComboBox)this.FindName("lst_" + i.ToString());
                lsthere.Items.Clear();
                lsthere.Items.Add(item);
            }

            TextBlock txthere = new TextBlock();
            txthere.Text = "Riga XLS selezionata > ";
            txthere.FontWeight = FontWeights.Bold;
            txthere.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            stphere.Children.Add(txthere);

            for (i = 1; i <= 100; i++)
            {
                valuehere = "";
                try
                {
                    ExcelRange objRange = excelSheet.Cells[rowintestazione, i];
                    valuehere = objRange.Merge ?
                      Convert.ToString((excelSheet.Cells[1, 1]).Text).Trim() :
                      Convert.ToString(objRange.Text).Trim();
                    if (valuehere.Trim() == "") continue;
                }
                catch (Exception) { continue; }

                StackPanel stp = new StackPanel();
                stp.Orientation = System.Windows.Controls.Orientation.Vertical;

                TextBlock txt = new TextBlock();
                txt.Text = ColumnIndexToColumnLetter(i);

                item = new ComboboxItem();
                item.Text = txt.Text + " - " + valuehere;
                item.Value = txt.Text;

                for (jj = 0; jj < colonne.Count; jj++)
                {
                    ComboBox lsthere = (ComboBox)this.FindName("lst_" + jj.ToString());
                    lsthere.Items.Add(item);
                }

                txt.TextAlignment = TextAlignment.Center;
                txt.FontWeight = FontWeights.Bold;
                txt.Width = 100.0;
                txt.Margin = new Thickness(5, 5, 0, 0);
                stp.Children.Add(txt);

                txt = new TextBlock();
                txt.Text = valuehere;
                txt.TextAlignment = TextAlignment.Center;
                txt.Width = 100.0;
                txt.Margin = new Thickness(5, 0, 0, 5);
                stp.Children.Add(txt);

                stphere.Children.Add(stp);
            }
        }

        #endregion

        #region Intervalli monetari

        //------------------------------------------------------------------------+
        //                 tabIntervalliMonetari_SelectionChanged                 |
        //------------------------------------------------------------------------+
        private void tabIntervalliMonetari_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string stratificazionescelta;

            stratificazionescelta = "";
            if (tabIntervalliMonetari.SelectedItem == null)
                tabIntervalliMonetari.SelectedIndex = 0;
            if (tabIntervalliMonetari.SelectedIndex != -1)
            {
                if (tabIntervalliMonetari.Items.Count == 1)
                    stratificazionescelta =
                      ((TabItem)(tabIntervalliMonetari.Items[0])).Tag.ToString();
                else
                {
                    TabItem ti = (TabItem)(tabIntervalliMonetari.SelectedItem);
                    stratificazionescelta = ti.Tag.ToString();
                }
            }
            if (stratificazionescelta == "") stratificazionescelta = "_";
            CreaIntervalliMonetari(stratificazionescelta);
        }

        //------------------------------------------------------------------------+
        //                         CreaIntervalliMonetari                         |
        //------------------------------------------------------------------------+
        private void CreaIntervalliMonetari(string stratificazione)
        {
            bool atleastone, resultbool;
            double valuehere;
            Hashtable sl;
            int i;
            List<string> sa;
            string attributohere, str, str2;
            TextBlock lbl;

            if (RawData == null) return;
            sl = new Hashtable();
            sa = new List<string>();
            intervalloMIN.Clear();
            intervalloMAX.Clear();

            for (i = 0; i < RawData.Tables[0].Rows.Count; i++)
            {
                /*-----------------------------------------------------------------------------
                        if (node.Attributes["RigheCancellate"] != null
                          && node.Attributes["RigheCancellate"].Value.Split('|')
                            .Contains(RawData.Tables[0].Rows[i][0].ToString()
                            + "-" + RawData.Tables[0].Rows[i][1].ToString()))
                        if (node.Attributes["RigheCancellate"] != null && node.Attributes["RigheCancellate"].Value.Split('|').Contains(RawData.Tables[0].Rows[i][0].ToString() + "-" + RawData.Tables[0].Rows[i][1].ToString()))
                        {
                          continue;
                        }
                -----------------------------------------------------------------------------*/
                if (EsisteAttributo("RigheCancellate")
                  && GetRigaAttributo("RigheCancellate").Split('|')
                    .Contains(RawData.Tables[0].Rows[i][0].ToString()
                    + "-" + RawData.Tables[0].Rows[i][1].ToString())) continue;
                attributohere = TOXMLAttribute(
                  RawData.Tables[0].Rows[i][colonne.Count - 1].ToString().Trim());
                if (!sa.Contains(attributohere) && attributohere != "")
                    sa.Add(attributohere);
                if (attributohere == "") attributohere = "_";
                valuehere = 0.0;
                resultbool = false;
                resultbool = double.TryParse(
                  Convert.ToString(RawData.Tables[0].Rows[i][
                    Convert.ToInt32(indexcolumnsaldo)].ToString()).Trim(),
                      out valuehere);
                if (resultbool)
                {
                    if (!sl.ContainsKey(attributohere))
                        sl.Add(attributohere, new List<double>());
                    ((List<double>)(sl[attributohere])).Add(valuehere);
                }
            }
            foreach (DictionaryEntry item in sl)
            {
                ((List<double>)(item.Value)).Sort();
                intervalloMIN.Add(
                  item.Key.ToString(), ((List<double>)(item.Value)).Min());

                str = string.Format("intervalloMIN_{0}",
                  TOXMLAttribute(item.Key.ToString()));
                str2 = ((double)intervalloMIN[item.Key.ToString()]).ToString();
                if (node.Attributes[str] == null)
                {
                    XmlAttribute attr = node.OwnerDocument.CreateAttribute(str);
                    node.Attributes.Append(attr);
                }
                node.Attributes[str].Value = str2;
                SetRigaAttributo(str, str2);
                intervalloMAX.Add(item.Key, ((List<double>)(item.Value)).Max());

                str = string.Format("intervalloMAX_{0}",
                  TOXMLAttribute(item.Key.ToString()));
                str2 = ((double)intervalloMAX[item.Key.ToString()]).ToString();
                if (node.Attributes[str] == null)
                {
                    XmlAttribute attr = node.OwnerDocument.CreateAttribute(str);
                    node.Attributes.Append(attr);
                }
                node.Attributes[str].Value = str2;
                SetRigaAttributo(str, str2);
            }

            sa.Sort();
            intervalli.Clear();
            intervalloMIN.Clear();
            intervalloMAX.Clear();
            if (sa.Count > 0)
            {
                foreach (string item in sa)
                {
                    /*-----------------------------------------------------------------------------
                              if (node != null
                                && node.Attributes["Intervalli_" + TOXMLAttribute(item)] != null
                                && node.Attributes["Intervalli_" + TOXMLAttribute(item)].Value != "")
                    -----------------------------------------------------------------------------*/
                    str = string.Format("Intervalli_{0}", TOXMLAttribute(item));
                    if (EsisteAttributo(str)
                      && !string.IsNullOrEmpty(GetRigaAttributo(str)))
                    {
                        /*-----------------------------------------------------------------------------
                                    intervalli.Add(TOXMLAttribute(item),
                                      node.Attributes[str].Value.Split('|').ToList()
                                        .ConvertAll(s => double.Parse(s)));
                        -----------------------------------------------------------------------------*/
                        intervalli.Add(TOXMLAttribute(item),
                          GetRigaAttributo(str).Split('|').ToList()
                            .ConvertAll(s => double.Parse(s)));
                    }
                    else intervalli.Add(TOXMLAttribute(item), new List<double>());
                    /*-----------------------------------------------------------------------------
                              if (node != null
                                && node.Attributes["intervalloMIN_" + TOXMLAttribute(item)] != null
                                && node.Attributes["intervalloMIN_" + TOXMLAttribute(item)].Value != "")
                    -----------------------------------------------------------------------------*/
                    str = string.Format("intervalloMIN_{0}", TOXMLAttribute(item));
                    if (EsisteAttributo(str)
                      && !string.IsNullOrEmpty(GetRigaAttributo(str)))
                    {
                        //intervalloMIN.Add(TOXMLAttribute(item), node.Attributes[str].Value);
                        intervalloMIN.Add(TOXMLAttribute(item), GetRigaAttributo(str));
                    }
                    else intervalloMIN.Add(TOXMLAttribute(item), new List<double>());
                    /*-----------------------------------------------------------------------------
                              if (node != null
                                && node.Attributes["intervalloMAX_" + TOXMLAttribute(item)] != null
                                && node.Attributes["intervalloMAX_" + TOXMLAttribute(item)].Value != "")
                    -----------------------------------------------------------------------------*/
                    str = string.Format("intervalloMAX_{0}", TOXMLAttribute(item));
                    if (EsisteAttributo(str)
                      && !string.IsNullOrEmpty(GetRigaAttributo(str)))
                    {
                        /*-----------------------------------------------------------------------------
                                    intervalloMAX.Add(TOXMLAttribute(item),
                                      node.Attributes["intervalloMAX_" + TOXMLAttribute(item)].Value);
                        -----------------------------------------------------------------------------*/
                        intervalloMAX.Add(TOXMLAttribute(item), GetRigaAttributo(str));
                    }
                    else intervalloMAX.Add(TOXMLAttribute(item), new List<double>());
                }
            }
            else
            {
                /*-----------------------------------------------------------------------------
                        if (node != null
                          && node.Attributes["Intervalli__"] != null
                          && node.Attributes["Intervalli__"].Value != "")
                -----------------------------------------------------------------------------*/
                str = "Intervalli__";
                if (EsisteAttributo(str)
                  && !string.IsNullOrEmpty(GetRigaAttributo(str)))
                {
                    /*-----------------------------------------------------------------------------
                              intervalli.Add("_",
                                node.Attributes["Intervalli__"].Value.Split('|').ToList()
                                  .ConvertAll(s => double.Parse(s)));
                    -----------------------------------------------------------------------------*/
                    intervalli.Add("_",
                      GetRigaAttributo(str).Split('|').ToList()
                        .ConvertAll(s => double.Parse(s)));
                }
                else intervalli.Add("_", new List<double>());
                /*-----------------------------------------------------------------------------
                        if (node != null
                          && node.Attributes["intervalloMIN_" + "_"] != null
                          && node.Attributes["intervalloMIN_" + "_"].Value != "")
                -----------------------------------------------------------------------------*/
                str = "intervalloMIN__";
                if (EsisteAttributo(str)
                  && !string.IsNullOrEmpty(GetRigaAttributo(str)))
                {
                    //intervalloMIN.Add("_", node.Attributes["intervalloMIN_" + "_"].Value);
                    intervalloMIN.Add("_", GetRigaAttributo(str));
                }
                else intervalloMIN.Add("_", new List<double>());
                /*-----------------------------------------------------------------------------
                        if (node != null
                          && node.Attributes["intervalloMAX_" + "_"] != null
                          && node.Attributes["intervalloMAX_" + "_"].Value != "")
                -----------------------------------------------------------------------------*/
                str = "intervalloMAX__";
                if (EsisteAttributo(str)
                  && !string.IsNullOrEmpty(GetRigaAttributo(str)))
                {
                    //intervalloMAX.Add("_", node.Attributes["intervalloMAX_" + "_"].Value);
                    intervalloMAX.Add("_", GetRigaAttributo(str));
                }
                else intervalloMAX.Add("_", new List<double>());
            }

            if (stratificazione == "")
            {
                tabIntervalliMonetari.Items.Clear();
                if (sa.Count > 0)
                {
                    foreach (string item in sa)
                    {
                        TabItem ti = new TabItem();
                        ti.MinWidth = 150.0;
                        ti.Background = Brushes.LightGoldenrodYellow;
                        ti.Header = item;
                        ti.Tag = item;
                        tabIntervalliMonetari.Items.Add(ti);
                    }
                    tabIntervalliMonetari.SelectedIndex = 0;
                    tabIntervalliMonetari.Visibility = Visibility.Visible;
                }
                else tabIntervalliMonetari.Visibility = Visibility.Collapsed;
                stratificazione = "_";
            }

            if (!intervalli.ContainsKey(stratificazione))
            {
                foreach (DictionaryEntry item2 in intervalli)
                {
                    stratificazione = item2.Key.ToString();
                    break;
                }
            }
            stpIntervalliMonetari2.Children.Clear();

            atleastone = false;
            for (i = 0; i < ((List<double>)(intervalli[stratificazione])).Count; i++)
            {
                atleastone = true;
                StackPanel stpRiga_colonna = new StackPanel();
                stpRiga_colonna.Margin = new Thickness(0, 10, 0, 0);
                stpRiga_colonna.Orientation = System.Windows.Controls.Orientation.Horizontal;
                if (i == 0)
                {
                    lbl = new TextBlock();
                    lbl.Text = "Strato 1 - da: ";
                    lbl.Width = 100;
                    stpRiga_colonna.Children.Add(lbl);

                    lbl = new TextBlock();
                    lbl.Name = "txtda_" + stratificazione + "_" + i.ToString();
                    try { this.UnregisterName(lbl.Name); }
                    catch (Exception) { }
                    this.RegisterName(lbl.Name, lbl);
                    lbl.Text = ConvertNumberNoDecimal(
                      intervalloMIN[stratificazione].ToString());
                    lbl.Width = 50;
                    stpRiga_colonna.Children.Add(lbl);

                    lbl = new TextBlock();
                    lbl.Text = " a: ";
                    lbl.Width = 30;
                    stpRiga_colonna.Children.Add(lbl);

                    TextBlock txta = new TextBlock();
                    txta.Name = "txta_" + stratificazione + "_" + i.ToString();
                    txta.Text = ConvertNumber(
                      (((List<double>)(intervalli[stratificazione]))[i] - 0.01).ToString());
                    txta.Width = 50;
                    try { this.UnregisterName(txta.Name); }
                    catch (Exception) { }
                    this.RegisterName(txta.Name, txta);
                    stpRiga_colonna.Children.Add(txta);

                    Button btnadd2 = new Button();
                    btnadd2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    btnadd2.Width = 20.0;
                    btnadd2.Tag = i.ToString();
                    btnadd2.Margin = new Thickness(30, 0, 0, 0);
                    Image img2 = new Image();
                    Uri uriSource2 = new Uri(addimg, UriKind.Relative);
                    btnadd2.ToolTip = "Aggiungi valore intermedio intervallo.";
                    img2.Source = new BitmapImage(uriSource2);
                    btnadd2.Content = img2;
                    btnadd2.Click += Btnadd_Click;

                    stpRiga_colonna.Children.Add(btnadd2);

                    btnadd2 = new Button();
                    btnadd2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    btnadd2.Width = 20.0;
                    btnadd2.Tag = i.ToString();
                    btnadd2.Margin = new Thickness(10, 0, 0, 0);
                    img2 = new Image();
                    uriSource2 = new Uri(deleteimg, UriKind.Relative);
                    btnadd2.ToolTip = "Rimuovi intervallo.";
                    img2.Source = new BitmapImage(uriSource2);
                    btnadd2.Content = img2;
                    btnadd2.Click += Btnremove_Click;

                    stpRiga_colonna.Children.Add(btnadd2);

                    stpIntervalliMonetari2.Children.Add(stpRiga_colonna);

                    stpRiga_colonna = new StackPanel();
                    stpRiga_colonna.Margin = new Thickness(0, 10, 0, 0);
                    stpRiga_colonna.Orientation =
                      System.Windows.Controls.Orientation.Horizontal;
                }

                lbl = new TextBlock();
                lbl.Text = "Strato " + (i + 2).ToString() + " - da: ";
                lbl.Width = 100;
                stpRiga_colonna.Children.Add(lbl);

                TextBlock txtda = new TextBlock();
                txtda.Name = "txtda_" + stratificazione + "_" + (i + 1).ToString();
                txtda.Text = ConvertNumber(
                  ((List<double>)(intervalli[stratificazione]))[i].ToString());
                txtda.Width = 50;
                try { this.UnregisterName(txtda.Name); }
                catch (Exception) { }
                this.RegisterName(txtda.Name, txtda);
                stpRiga_colonna.Children.Add(txtda);

                lbl = new TextBlock();
                lbl.Text = " a: ";
                lbl.Width = 30;
                stpRiga_colonna.Children.Add(lbl);

                if (i == (((List<double>)(intervalli[stratificazione])).Count - 1))
                {
                    lbl = new TextBlock();
                    lbl.Name = "txta_" + stratificazione + "_" + (i + 1).ToString();
                    lbl.Text = ConvertNumber(intervalloMAX[stratificazione].ToString());
                    lbl.Width = 50;
                    try { this.UnregisterName(lbl.Name); }
                    catch (Exception) { }
                    this.RegisterName(lbl.Name, lbl);
                    stpRiga_colonna.Children.Add(lbl);
                }
                else
                {
                    TextBlock txta = new TextBlock();
                    txta.Name = "txta_" + stratificazione + "_" + (i + 1).ToString();
                    txta.Text = ConvertNumber(
                      (((List<double>)(intervalli[stratificazione]))[i + 1] - 0.01)
                        .ToString());
                    txta.Width = 50;
                    try { this.UnregisterName(txta.Name); }
                    catch (Exception) { }
                    this.RegisterName(txta.Name, txta);
                    stpRiga_colonna.Children.Add(txta);
                }

                Button btnadd = new Button();
                btnadd.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                btnadd.Width = 20.0;
                btnadd.Tag = (i + 1).ToString();
                btnadd.Margin = new Thickness(30, 0, 0, 0);
                Image img = new Image();
                Uri uriSource = new Uri(addimg, UriKind.Relative);
                btnadd.ToolTip = "Aggiungi valore intermedio intervallo.";
                img.Source = new BitmapImage(uriSource);
                btnadd.Content = img;
                btnadd.Click += Btnadd_Click;

                stpRiga_colonna.Children.Add(btnadd);

                btnadd = new Button();
                btnadd.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                btnadd.Width = 20.0;
                btnadd.Tag = (i + 1).ToString();
                btnadd.Margin = new Thickness(10, 0, 0, 0);
                img = new Image();
                uriSource = new Uri(deleteimg, UriKind.Relative);
                btnadd.ToolTip = "Rimuovi intervallo.";
                img.Source = new BitmapImage(uriSource);
                btnadd.Content = img;
                btnadd.Click += Btnremove_Click;

                stpRiga_colonna.Children.Add(btnadd);
                stpIntervalliMonetari2.Children.Add(stpRiga_colonna);
            }

            if (!atleastone)
            {
                StackPanel stpRiga_colonna = new StackPanel();
                stpRiga_colonna.Margin = new Thickness(0, 10, 0, 0);
                stpRiga_colonna.Orientation =
                  System.Windows.Controls.Orientation.Horizontal;
                lbl = new TextBlock();
                lbl.Text = "Strato unico - da: ";
                lbl.Width = 100;
                stpRiga_colonna.Children.Add(lbl);

                lbl = new TextBlock();
                lbl.Name = "txtda_" + stratificazione + "_" + "1";
                try { this.UnregisterName(lbl.Name); }
                catch (Exception) { }
                this.RegisterName(lbl.Name, lbl);
                lbl.Text = ConvertNumber(intervalloMIN[stratificazione].ToString());
                lbl.Width = 50;
                stpRiga_colonna.Children.Add(lbl);

                lbl = new TextBlock();
                lbl.Text = " a: ";
                lbl.Width = 30;
                stpRiga_colonna.Children.Add(lbl);

                lbl = new TextBlock();
                lbl.Name = "txta_" + stratificazione + "_" + "1";
                try { this.UnregisterName(lbl.Name); }
                catch (Exception) { }
                this.RegisterName(lbl.Name, lbl);
                lbl.Text = ConvertNumber(intervalloMAX[stratificazione].ToString());
                lbl.Width = 50;
                stpRiga_colonna.Children.Add(lbl);

                Button btnadd = new Button();
                btnadd.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                btnadd.Width = 20.0;
                btnadd.Tag = "1";
                btnadd.Margin = new Thickness(30, 0, 0, 0);
                Image img = new Image();
                Uri uriSource = new Uri(addimg, UriKind.Relative);
                btnadd.ToolTip = "Aggiungi valore intermedio intervallo.";
                img.Source = new BitmapImage(uriSource);
                btnadd.Content = img;
                btnadd.Click += Btnadd_Click;

                stpRiga_colonna.Children.Add(btnadd);

                btnadd = new Button();
                btnadd.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                btnadd.Width = 20.0;
                btnadd.Tag = "1";
                btnadd.Margin = new Thickness(10, 0, 0, 0);
                img = new Image();
                uriSource = new Uri(deleteimg, UriKind.Relative);
                btnadd.ToolTip = "Rimuovi intervallo.";
                img.Source = new BitmapImage(uriSource);
                btnadd.Content = img;
                btnadd.Click += Btnremove_Click;

                stpRiga_colonna.Children.Add(btnadd);
                stpIntervalliMonetari2.Children.Add(stpRiga_colonna);
            }
          ((WindowWorkArea)(this.Owner))._x.Save();
        }

        //------------------------------------------------------------------------+
        //                             TOXMLAttribute                             |
        //------------------------------------------------------------------------+
        private string TOXMLAttribute(string valore)
        {
            return valore.Replace(" ", "_").Replace(":", "")
              .Replace(".", "").Replace(",", "");
        }

        //------------------------------------------------------------------------+
        //                              Btnadd_Click                              |
        //------------------------------------------------------------------------+
        private void Btnadd_Click(object sender, RoutedEventArgs e)
        {
            double valuea, valueda, valuenow;
            int i;
            string indexhere, str, stratificazionescelta;
            TextBlock txt_a, txt_da;

            stratificazionescelta = "";
            if (tabIntervalliMonetari.SelectedItem == null)
                tabIntervalliMonetari.SelectedIndex = 0;
            if (tabIntervalliMonetari.SelectedIndex != -1)
            {
                if (tabIntervalliMonetari.Items.Count == 1)
                    stratificazionescelta =
                      ((TabItem)tabIntervalliMonetari.Items[0]).Tag.ToString();
                else
                {
                    TabItem ti = (TabItem)(tabIntervalliMonetari.SelectedItem);
                    stratificazionescelta = ti.Tag.ToString();
                }
            }
            if (stratificazionescelta == "") stratificazionescelta = "_";
            indexhere = ((Button)sender).Tag.ToString();
            txt_da = (TextBlock)this.FindName(
              "txtda_" + stratificazionescelta + "_" + indexhere);
            txt_a = (TextBlock)this.FindName(
              "txta_" + stratificazionescelta + "_" + indexhere);
            wInputBox dialog = new wInputBox(
              "Inserire nuovo valore tra " + txt_da.Text + " e " + txt_a.Text);
            dialog.ShowDialog();
            valueda = 0.0; double.TryParse(txt_da.Text, out valueda);
            valuea = 0.0; double.TryParse(txt_a.Text, out valuea);
            valuenow = 0.0; double.TryParse(dialog.ResponseText, out valuenow);
            if (valuenow > valueda && valuenow < valuea)
            {
                ((List<double>)(intervalli[stratificazionescelta])).Add(valuenow);
                ((List<double>)(intervalli[stratificazionescelta])).Sort();

                if (node != null)
                {
                    str = string.Format("Intervalli_{0}", TOXMLAttribute(stratificazionescelta));
                    if (node.Attributes[str] == null)
                    {
                        XmlAttribute attr = node.OwnerDocument.CreateAttribute(str);
                        node.Attributes.Append(attr);
                    }
                    node.Attributes[str].Value =
                      String.Join("|",
                        ((List<double>)(intervalli[stratificazionescelta])).ToArray());
                    SetRigaAttributo(str,
                      String.Join("|",
                        ((List<double>)(intervalli[stratificazionescelta])).ToArray()));
                }
                CreaIntervalliMonetari(stratificazionescelta);
            }
            else MessageBox.Show("Valore non compreso nel range");
            //PRISC
            ALrowsstratificate_scelte.Clear();
            ALrowsstratificate.Clear();
            ALrowsIntermediatevalue.Clear();
            //ALtxtTotaleSaldiCampione.Clear();
            ALtxtTotaleSaldo.Clear();
            for (i = 0; i < 1000; i++)
            {
                str = string.Format("rowsstratificate_scelte_{0}", i);
                if (node != null && node.Attributes[str] != null)
                    node.Attributes.Remove(node.Attributes[str]);
                RemoveRigaAttributo(str);
            }
        }

        //------------------------------------------------------------------------+
        //                            Btnremove_Click                             |
        //------------------------------------------------------------------------+
        private void Btnremove_Click(object sender, RoutedEventArgs e)
        {
            int i, indexhere;
            string str, stratificazionescelta;

            stratificazionescelta = "";
            if (tabIntervalliMonetari.SelectedItem == null)
                tabIntervalliMonetari.SelectedIndex = 0;
            if (tabIntervalliMonetari.SelectedIndex != -1)
            {
                if (tabIntervalliMonetari.Items.Count == 1)
                    stratificazionescelta =
                      ((TabItem)tabIntervalliMonetari.Items[0]).Tag.ToString();
                else
                {
                    TabItem ti = (TabItem)(tabIntervalliMonetari.SelectedItem);
                    stratificazionescelta = ti.Tag.ToString();
                }
            }
            if (stratificazionescelta == "") stratificazionescelta = "_";
            indexhere = Convert.ToInt32(((Button)sender).Tag.ToString());
            ((List<double>)(intervalli[stratificazionescelta])).RemoveAt(indexhere - 1);
            ((List<double>)(intervalli[stratificazionescelta])).Sort();
            if (node != null)
            {
                str = string.Format("Intervalli_{0}", TOXMLAttribute(stratificazionescelta));
                if (node.Attributes[str] == null)
                {
                    XmlAttribute attr = node.OwnerDocument.CreateAttribute(str);
                    node.Attributes.Append(attr);
                }
                node.Attributes[str].Value =
                  String.Join("|",
                    ((List<double>)intervalli[stratificazionescelta]).ToArray());
                SetRigaAttributo(str,
                  String.Join("|",
                    ((List<double>)intervalli[stratificazionescelta]).ToArray()));
            }
            CreaIntervalliMonetari(stratificazionescelta);
            //PRISC
            ALrowsstratificate_scelte.Clear();
            ALrowsstratificate.Clear();
            ALrowsIntermediatevalue.Clear();
            //ALtxtTotaleSaldiCampione.Clear();
            ALtxtTotaleSaldo.Clear();
            for (i = 0; i < 1000; i++)
            {
                str = string.Format("rowsstratificate_scelte_{0}", i);
                if (node != null && node.Attributes[str] != null)
                    node.Attributes.Remove(node.Attributes[str]);
                if (EsisteAttributo(str)) RemoveRigaAttributo(str);
            }
        }

        #endregion

        #region first step

        //------------------------------------------------------------------------+
        //                             CreaFirstStep                              |
        //------------------------------------------------------------------------+
        private void CreaFirstStep()
        {
            string str;

            if (node != null)
            {
                txtMotivazione.Text = "";
                this.mainRTB.Document.Blocks.Clear();
                this.mainRTB.Selection.ApplyPropertyValue(
                  FlowDocument.TextAlignmentProperty, TextAlignment.Justify);

                //if (node.Attributes["Motivazioni"] != null)
                if (EsisteAttributo("Motivazioni"))
                {
                    str = GetRigaAttributo("Motivazioni");
                    try
                    {
                        /*-----------------------------------------------------------------------------
                                    MemoryStream stream = new MemoryStream(
                                      ASCIIEncoding.Default.GetBytes(
                                        node.Attributes["Motivazioni"].Value));
                        -----------------------------------------------------------------------------*/
                        MemoryStream stream = new MemoryStream(
                          ASCIIEncoding.Default.GetBytes(str));
                        this.mainRTB.Selection.Load(stream, DataFormats.Rtf);
                        TextRange tr = new TextRange(
                          mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);
                        MemoryStream ms = new MemoryStream();
                        tr.Save(ms, DataFormats.Text);
                        txtMotivazione.Text = ASCIIEncoding.Default.GetString(ms.ToArray());
                    }
                    catch (Exception) { txtMotivazione.Text = ""; }
                }
                else txtMotivazione.Text = "";

                mainRTB.Focus();
                mainRTB.CaretPosition = mainRTB.Document.ContentEnd;
                mainRTB.ScrollToEnd();

                rdbRagionato.IsChecked = false;
                rdbCasuale.IsChecked = false;
                rdbMUS.IsChecked = false;

                //if (node.Attributes["Scelta"] != null)
                if (EsisteAttributo("Scelta"))
                {
                    switch (GetRigaAttributo("Scelta"))
                    {
                        case "Ragionato":
                            rdbRagionato.IsChecked = true;
                            break;
                        case "Casuale":
                            rdbCasuale.IsChecked = true;
                            break;
                        case "MUS":
                            rdbMUS.IsChecked = true;
                            break;
                    }
                }
            }

            if (Materialità_1 == true)
            {
                materialita1.Visibility = Visibility.Visible;
                materialita1Operativa.Text = MaterialitaOperativa;
                materialita1Bilancio.Text = MaterialitaBilancio;
            }
            else materialita1.Visibility = Visibility.Collapsed;

            if (Materialità_2 == true || Materialità_3 == true)
            {
                materialita23SP.Visibility = Visibility.Visible;
                materialita23OperativaSP.Text = MaterialitaOperativa;
                materialita23BilancioSP.Text = MaterialitaBilancio;

                materialita23CE.Visibility = Visibility.Visible;
                materialita23OperativaCE.Text = MaterialitaOperativa2;
                materialita23BilancioCE.Text = MaterialitaBilancio2;
            }
            else
            {
                materialita23SP.Visibility = Visibility.Collapsed;
                materialita23CE.Visibility = Visibility.Collapsed;
            }

            if (RischioIndividuazione != "")
            {
                rischioindividuazione.Visibility = Visibility.Visible;
                RischioIndividuazioneTitolo.Text =
                  "Rischio di individuazione " + ciclo + ": ";
                RischioIndividuazioneValore.Text = RischioIndividuazione;
                RischioIndividuazioneValore.Text += " (Divisore: ";
                switch (RischioIndividuazione.ToUpper())
                {
                    case "MOLTO ALTO":
                        RischioIndividuazioneValore.Text += "0,7";
                        break;
                    case "ALTO":
                        RischioIndividuazioneValore.Text += "1";
                        break;
                    case "MEDIO":
                        RischioIndividuazioneValore.Text += "1,5";
                        break;
                    case "BASSO":
                        RischioIndividuazioneValore.Text += "2";
                        break;
                    case "MOLTO BASSO":
                        RischioIndividuazioneValore.Text += "3";
                        break;
                    default:
                        break;
                }
                RischioIndividuazioneValore.Text += " )";
            }
            else rischioindividuazione.Visibility = Visibility.Collapsed;
            ((WindowWorkArea)(this.Owner))._x.Save();
        }

        //------------------------------------------------------------------------+
        //                     obj_PreviewMouseLeftButtonDown                     |
        //------------------------------------------------------------------------+
        private void obj_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((((WindowWorkArea)(this.Owner)).ReadOnly))
                MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        }

        //------------------------------------------------------------------------+
        //                           obj_PreviewKeyDown                           |
        //------------------------------------------------------------------------+
        private void obj_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((((WindowWorkArea)(this.Owner)).ReadOnly))
                MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        }

        //------------------------------------------------------------------------+
        //                       GestoreEvento_DatiCambiati                       |
        //------------------------------------------------------------------------+
        private void GestoreEvento_DatiCambiati(object sender, RoutedEventArgs e)
        { }

        #endregion

        #region Stratificazione

        //------------------------------------------------------------------------+
        //                          CreaStratificazione                           |
        //------------------------------------------------------------------------+
        private void CreaStratificazione()
        {
            int i;
            List<string> sl;
            string selectedhere, str, str2, valorehere;

            //PRISC
            ALrowsstratificate_scelte.Clear();
            ALrowsstratificate.Clear();
            ALrowsIntermediatevalue.Clear();
            //ALtxtTotaleSaldiCampione.Clear();
            ALtxtTotaleSaldo.Clear();

            for (i = 0; i < 1000; i++)
            {
                /*-----------------------------------------------------------------------------
                        if (node != null
                          && node.Attributes["rowsstratificate_scelte_" + i.ToString()] != null)
                -----------------------------------------------------------------------------*/
                str = string.Format("rowsstratificate_scelte_{0}", i);
                if (EsisteAttributo(str))
                {
                    node.Attributes.Remove(
                      node.Attributes["rowsstratificate_scelte_" + i.ToString()]);
                    RemoveRigaAttributo(str);
                }
            }
            if (RawData == null) return;
            selectedhere = "";
            sl = new List<string>();
            for (i = 0; i < RawData.Tables[0].Rows.Count; i++)
            {
                /*-----------------------------------------------------------------------------
                        if (node.Attributes["RigheCancellate"] != null
                          && node.Attributes["RigheCancellate"].Value.Split('|')
                            .Contains(RawData.Tables[0].Rows[i][0].ToString() + "-"
                              + RawData.Tables[0].Rows[i][1].ToString()))
                        {
                          continue;
                        }
                -----------------------------------------------------------------------------*/
                str = "RigheCancellate";
                if (EsisteAttributo(str)
                  && GetRigaAttributo(str).Split('|')
                    .Contains(RawData.Tables[0].Rows[i][0].ToString() + "-"
                      + RawData.Tables[0].Rows[i][1].ToString())) continue;
                valorehere = TOXMLAttribute(RawData.Tables[0].Rows[i][colonne.Count - 1].ToString().Trim());
                if (!sl.Contains(valorehere) && valorehere != "")
                    sl.Add(valorehere);
            }
            sl.Sort();
            if (sl.Count > 0)
            {
                //if (node.Attributes["Stratificazioni_Attributo"] != null)
                str = "Stratificazioni_Attributo";
                if (EsisteAttributo(str))
                {
                    //selectedhere = node.Attributes["Stratificazioni_Attributo"].Value;
                    selectedhere = GetRigaAttributo(str);
                }

                str = "Stratificazioni_Attributo_ALL";
                if (node.Attributes[str] == null)
                {
                    XmlAttribute attr = node.OwnerDocument.CreateAttribute(str);
                    node.Attributes.Append(attr);
                }

                node.Attributes[str].Value = "";
                SetRigaAttributo(str, string.Empty);

                stpStratificazione_Attributo.Visibility = Visibility.Visible;
                stpStratificazione_Attributo_Interna.Children.Clear();
                stpStratificazione_Intervalli.Visibility = Visibility.Collapsed;
                stpStratificazione_Intervalli.Children.Clear();

                intervalli.Clear();
                intervalloMIN.Clear();
                intervalloMAX.Clear();

                foreach (string item in sl)
                {
                    /*-----------------------------------------------------------------------------
                              if (node != null
                                && node.Attributes["Intervalli_" + TOXMLAttribute(item)] != null
                                && node.Attributes["Intervalli_" + TOXMLAttribute(item)].Value != "")
                    -----------------------------------------------------------------------------*/
                    str = string.Format("Intervalli_{0}", TOXMLAttribute(item));
                    if (EsisteAttributo(str)
                      && !string.IsNullOrEmpty(GetRigaAttributo(str)))
                    {
                        intervalli.Add(item,
                          //node.Attributes["Intervalli_" + TOXMLAttribute(item)].Value.Split('|').ToList().ConvertAll(s => double.Parse(s)));
                          GetRigaAttributo(str).Split('|').ToList()
                            .ConvertAll(s => double.Parse(s)));
                    }
                    else intervalli.Add(item, new List<double>());
                    /*-----------------------------------------------------------------------------
                              if (node != null
                                && node.Attributes["intervalloMIN_" + TOXMLAttribute(item)] != null
                                && node.Attributes["intervalloMIN_" + TOXMLAttribute(item)].Value != "")
                    -----------------------------------------------------------------------------*/
                    str = string.Format("intervalloMIN_{0}", TOXMLAttribute(item));
                    if (EsisteAttributo(str)
                      && !string.IsNullOrEmpty(GetRigaAttributo(str)))
                    {
                        intervalloMIN.Add(item,
                          //node.Attributes["intervalloMIN_" + TOXMLAttribute(item)].Value);
                          GetRigaAttributo(str));
                    }
                    else intervalloMIN.Add(item, new List<double>());
                    /*-----------------------------------------------------------------------------
                              if (node != null
                                && node.Attributes["intervalloMAX_" + TOXMLAttribute(item)] != null
                                && node.Attributes["intervalloMAX_" + TOXMLAttribute(item)].Value != "")
                    -----------------------------------------------------------------------------*/
                    str = string.Format("intervalloMAX_{0}", TOXMLAttribute(item));
                    if (EsisteAttributo(str)
                      && !string.IsNullOrEmpty(GetRigaAttributo(str)))
                    {
                        intervalloMAX.Add(item,
                          //node.Attributes["intervalloMAX_" + TOXMLAttribute(item)].Value);
                          GetRigaAttributo(str));
                    }
                    else intervalloMAX.Add(item, new List<double>());
                    if (((List<double>)(intervalli[item])).Count > 0)
                    {
                        for (i = 0; i < ((List<double>)(intervalli[item])).Count; i++)
                        {
                            valorehere = "";
                            if (i == 0)
                            {
                                valorehere = "Strato 1 - da: ";
                                valorehere += ConvertNumber(intervalloMIN[item].ToString());
                                valorehere += " a: ";
                                valorehere += ConvertNumber(
                                  (((List<double>)(intervalli[item]))[i] - 0.01).ToString());

                                System.Windows.Controls.CheckBox chk2 =
                                  new System.Windows.Controls.CheckBox();
                                chk2.Margin = new Thickness(5);
                                chk2.Content = item + " - " + valorehere;
                                chk2.Tag = item + "|" + valorehere;

                                if (selectedhere.Contains(item + "|" + valorehere))
                                    chk2.IsChecked = true;

                                stpStratificazione_Attributo_Interna.Children.Add(chk2);

                                str = "Stratificazioni_Attributo_ALL";
                                str2 = GetRigaAttributo(str);
                                node.Attributes[str].Value +=
                                  ((node.Attributes[str].Value == "") ? "" : "|")
                                    + item + " - " + valorehere;
                                if (!string.IsNullOrEmpty(str2)) str2 += "|";
                                str2 += item + " - " + valorehere;
                                SetRigaAttributo(str, str2);
                            }

                            valorehere = "Strato " + (i + 2).ToString() + " - da: ";
                            valorehere += ConvertNumber(
                              ((List<double>)(intervalli[item]))[i].ToString());
                            valorehere += " a: ";

                            if (i == (((List<double>)(intervalli[item])).Count - 1))
                                valorehere += ConvertNumber(intervalloMAX[item].ToString());
                            else
                                valorehere += ConvertNumber(
                                  (((List<double>)(intervalli[item]))[i + 1] - 0.01).ToString());

                            System.Windows.Controls.CheckBox chk =
                              new System.Windows.Controls.CheckBox();
                            chk.Margin = new Thickness(5);
                            chk.Content = item + " - " + valorehere;
                            chk.Tag = item + "|" + valorehere;

                            if (selectedhere.Contains(item + "|" + valorehere))
                                chk.IsChecked = true;
                            stpStratificazione_Attributo_Interna.Children.Add(chk);

                            str = "Stratificazioni_Attributo_ALL";
                            str2 = GetRigaAttributo(str);
                            node.Attributes[str].Value +=
                              ((node.Attributes[str].Value == "") ? "" : "|")
                                + item + " - " + valorehere;
                            if (!string.IsNullOrEmpty(str2)) str2 += "|";
                            str2 += item + " - " + valorehere;
                            SetRigaAttributo(str, str2);
                        }
                    }
                    else
                    {
                        System.Windows.Controls.CheckBox chk =
                          new System.Windows.Controls.CheckBox();
                        chk.Margin = new Thickness(5);
                        chk.Content = item;
                        chk.Tag = item;

                        if (selectedhere.Split('|').Contains(item))
                            chk.IsChecked = true;
                        stpStratificazione_Attributo_Interna.Children.Add(chk);

                        str = "Stratificazioni_Attributo_ALL";
                        str2 = GetRigaAttributo(str);
                        node.Attributes[str].Value +=
                          ((node.Attributes[str].Value == "") ? "" : "|") + item;
                        if (!string.IsNullOrEmpty(str2)) str2 += "|";
                        str2 += item;
                        SetRigaAttributo(str, str2);
                    }
                }
            }
            else
            {
                stpStratificazione_Attributo.Visibility = Visibility.Collapsed;

                intervalli.Clear();
                intervalloMIN.Clear();
                intervalloMAX.Clear();

                string item_ = "_";

                /*-----------------------------------------------------------------------------
                        if (node != null
                          && node.Attributes["Intervalli_" + TOXMLAttribute(item_)] != null
                          && node.Attributes["Intervalli_" + TOXMLAttribute(item_)].Value != "")
                -----------------------------------------------------------------------------*/
                str = string.Format("Intervalli_{0}", TOXMLAttribute(item_));
                if (EsisteAttributo(str)
                  && !string.IsNullOrEmpty(GetRigaAttributo(str)))
                {
                    intervalli.Add(item_,
                      /*-----------------------------------------------------------------------------
                                  node.Attributes["Intervalli_" + TOXMLAttribute(item_)].Value
                                    .Split('|').ToList().ConvertAll(s => double.Parse(s)));
                      -----------------------------------------------------------------------------*/
                      GetRigaAttributo(str)
                        .Split('|').ToList().ConvertAll(s => double.Parse(s)));
                }
                else intervalli.Add(item_, new List<double>());
                /*-----------------------------------------------------------------------------
                        if (node != null
                          && node.Attributes["intervalloMIN_" + TOXMLAttribute(item_)] != null
                          && node.Attributes["intervalloMIN_" + TOXMLAttribute(item_)].Value != "")
                -----------------------------------------------------------------------------*/
                str = string.Format("intervalloMIN_{0}", TOXMLAttribute(item_));
                if (EsisteAttributo(str)
                  && !string.IsNullOrEmpty(GetRigaAttributo(str)))
                {
                    intervalloMIN.Add(item_,
                      //node.Attributes["intervalloMIN_" + TOXMLAttribute(item_)].Value);
                      GetRigaAttributo(str));
                }
                else intervalloMIN.Add(item_, new List<double>());
                /*-----------------------------------------------------------------------------
                        if (node != null
                          && node.Attributes["intervalloMAX_" + TOXMLAttribute(item_)] != null
                          && node.Attributes["intervalloMAX_" + TOXMLAttribute(item_)].Value != "")
                -----------------------------------------------------------------------------*/
                str = string.Format("intervalloMAX_{0}", TOXMLAttribute(item_));
                if (EsisteAttributo(str)
                  && !string.IsNullOrEmpty(GetRigaAttributo(str)))
                {
                    intervalloMAX.Add(item_,
                      //node.Attributes["intervalloMAX_" + TOXMLAttribute(item_)].Value);
                      GetRigaAttributo(str));
                }
                else intervalloMAX.Add(item_, new List<double>());

                sl = new List<string>();
                selectedhere = "";

                for (i = 0; i < ((List<double>)(intervalli["_"])).Count; i++)
                {
                    valorehere = "";
                    if (i == 0)
                    {
                        valorehere = "Strato 1 - da: ";
                        valorehere += ConvertNumber(intervalloMIN["_"].ToString());
                        valorehere += " a: ";
                        valorehere += ConvertNumber(
                          (((List<double>)(intervalli["_"]))[i] - 0.01).ToString());
                        sl.Add(valorehere);
                    }

                    valorehere = "Strato " + (i + 2).ToString() + " - da: ";
                    valorehere += ConvertNumber(
                      ((List<double>)(intervalli["_"]))[i].ToString());
                    valorehere += " a: ";
                    if (i == (((List<double>)(intervalli["_"])).Count - 1))
                        valorehere += ConvertNumber(intervalloMAX["_"].ToString());
                    else
                        valorehere += ConvertNumber(
                          (((List<double>)(intervalli["_"]))[i + 1] - 0.01).ToString());
                    sl.Add(valorehere);
                }

                if (sl.Count > 0)
                {
                    //if (node.Attributes["Stratificazioni_Intervalli"] != null)
                    str = "Stratificazioni_Intervalli";
                    if (EsisteAttributo(str))
                    {
                        //selectedhere = node.Attributes["Stratificazioni_Intervalli"].Value;
                        selectedhere = GetRigaAttributo(str);
                    }

                    str = "Stratificazioni_Intervalli_ALL";
                    //if (node.Attributes["Stratificazioni_Intervalli_ALL"] == null)
                    if (!EsisteAttributo(str))
                    {
                        XmlAttribute attr = node.OwnerDocument.CreateAttribute(str);
                        node.Attributes.Append(attr);
                    }
                    node.Attributes[str].Value = "";
                    SetRigaAttributo(str, string.Empty);

                    stpStratificazione_Intervalli.Visibility = Visibility.Visible;
                    stpStratificazione_Intervalli_Interna.Children.Clear();

                    foreach (string item in sl)
                    {
                        System.Windows.Controls.CheckBox chk =
                          new System.Windows.Controls.CheckBox();
                        chk.Margin = new Thickness(5);
                        chk.Content = item;
                        chk.Tag = item;

                        if (selectedhere.Split('|').Contains(item))
                            chk.IsChecked = true;
                        stpStratificazione_Intervalli_Interna.Children.Add(chk);

                        str = "Stratificazioni_Intervalli_ALL";
                        str2 = GetRigaAttributo(str);
                        node.Attributes["Stratificazioni_Intervalli_ALL"].Value +=
                          ((node.Attributes["Stratificazioni_Intervalli_ALL"].Value == "") ?
                            "" : "|") + item;
                        if (!string.IsNullOrEmpty(str2)) str2 += "|";
                        str2 += item;
                        SetRigaAttributo(str, str2);
                    }
                }
                else stpStratificazione_Intervalli.Visibility = Visibility.Collapsed;
            }
          ((WindowWorkArea)(this.Owner))._x.Save();
        }

        #endregion

        #region Scelta Colonne

        private void Lst_Colonna_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { }

        #endregion

        #region final

        //------------------------------------------------------------------------+
        //                              CreateFinal                               |
        //------------------------------------------------------------------------+
        private void CreateFinal()
        {
            bool isdoneclear;
            double errorehere, totalErroriRilevati, totalErroriRilevatiStrato;
            double totalesaldo, valuehere;
            int i, indexhereselected, j, numcampioni, numerrori, real_i, row;
            string additiveNumeroErroriSuCampione, additiveTotaleErrori;
            string additiveTotaleErroriCampione, additivetxtMaterialita;
            string additivetxtPercentualeErroriSuCampione;
            string additivetxtPercentualeErroriSustrato;
            string additivetxtPercentualeProiettata;
            string additivetxtPercentualeSuTotaleDebiti, additivetxtTotaleDebiti;
            string additivetxtTotaleSaldiCampione, serrore, str, stratificazionescelta;

            if (FinalData == null) Btn_Next_Final(null, null);
            isdoneclear = false;
            for (i = 0; i < 1000; i++)
            {
                //if (node.Attributes["CompleteListStratification_" + i.ToString()] != null)
                str = string.Format("CompleteListStratification_{0}", i);
                if (EsisteAttributo(str))
                {
                    if (isdoneclear == false)
                    {
                        isdoneclear = true;
                        CompleteListStratification.Clear();
                    }
                    /*-----------------------------------------------------------------------------
                              if (!CompleteListStratification.Contains(
                                node.Attributes["CompleteListStratification_" + i.ToString()]
                                  .Value.ToString()))
                    -----------------------------------------------------------------------------*/
                    if (!CompleteListStratification.Contains(GetRigaAttributo(str)))
                    {
                        /*-----------------------------------------------------------------------------
                                    CompleteListStratification.Add(
                                      node.Attributes["CompleteListStratification_" + i.ToString()]
                                        .Value.ToString());
                        -----------------------------------------------------------------------------*/
                        CompleteListStratification.Add(GetRigaAttributo(str));
                    }
                }
            }

            stratificazionescelta = "";
            indexhereselected = 0;
            if (tabFinal.SelectedItem == null) tabFinal.SelectedIndex = 0;
            if (tabFinal.SelectedIndex != -1)
            {
                if (tabFinal.Items.Count == 1)
                {
                    indexhereselected = 0;
                    stratificazionescelta = ((TabItem)(tabFinal.Items[0])).Tag.ToString();
                }
                else
                {
                    TabItem ti = (TabItem)(tabFinal.SelectedItem);
                    stratificazionescelta = ti.Tag.ToString();
                    indexhereselected =
                      CompleteListStratification.IndexOf(ti.Tag.ToString());
                }
            }

            isdoneclear = false;
            for (i = 0; i < 1000; i++)
            {
                //if (node.Attributes["txtTipoCampionamento_Info_" + i.ToString()] != null)
                str = string.Format("txtTipoCampionamento_Info_{0}", i);
                if (EsisteAttributo(str))
                {
                    if (isdoneclear == false)
                    {
                        isdoneclear = true;
                        ALtxtTipoCampionamento_Info.Clear();
                    }
                    /*-----------------------------------------------------------------------------
                              ALtxtTipoCampionamento_Info.Add(
                                node.Attributes["txtTipoCampionamento_Info_" + i.ToString()]
                                  .Value.ToString());
                    -----------------------------------------------------------------------------*/
                    ALtxtTipoCampionamento_Info.Add(GetRigaAttributo(str));
                }
            }

            isdoneclear = false;
            for (i = 0; i < 1000; i++)
            {
                //if (node.Attributes["ALtxtTotaleSaldiCampione_" + i.ToString()] != null)
                str = string.Format("ALtxtTotaleSaldiCampione_{0}", i);
                if (EsisteAttributo(str))
                {
                    if (isdoneclear == false)
                    {
                        isdoneclear = true;
                        ALtxtTotaleSaldiCampione.Clear();
                    }
                    valuehere = 0;
                    /*-----------------------------------------------------------------------------
                              double.TryParse(
                                node.Attributes["ALtxtTotaleSaldiCampione_" + i.ToString()]
                                  .Value.ToString(), out valuehere);
                    -----------------------------------------------------------------------------*/
                    double.TryParse(GetRigaAttributo(str), out valuehere);
                    ALtxtTotaleSaldiCampione.Add(valuehere);
                }
            }

            isdoneclear = false;
            for (i = 0; i < 1000; i++)
            {
                //if (node.Attributes["ALtxtTotaleSaldo_" + i.ToString()] != null)
                str = string.Format("ALtxtTotaleSaldo_{0}", i);
                if (EsisteAttributo(str))
                {
                    if (isdoneclear == false)
                    {
                        isdoneclear = true;
                        ALtxtTotaleSaldo.Clear();
                    }
                    valuehere = 0;
                    /*-----------------------------------------------------------------------------
                              double.TryParse(
                                node.Attributes["ALtxtTotaleSaldo_" + i.ToString()]
                                  .Value.ToString(), out valuehere);
                    -----------------------------------------------------------------------------*/
                    double.TryParse(GetRigaAttributo(str), out valuehere);
                    ALtxtTotaleSaldo.Add(valuehere);
                }
            }

            additivetxtTotaleSaldiCampione = "";
            additivetxtTotaleDebiti = "";
            additivetxtPercentualeSuTotaleDebiti = "";
            additivetxtPercentualeErroriSuCampione = "";
            additivetxtPercentualeProiettata = "";
            additivetxtMaterialita = "";
            additiveNumeroErroriSuCampione = "";
            additiveTotaleErrori = "";
            additivetxtPercentualeErroriSustrato = "";
            additiveTotaleErroriCampione = "";

            switch (_tipologia)
            {
                case TipologieCampionamento.Clienti:
                    additivetxtTotaleSaldiCampione = "Tot. Selezione Strato";
                    additivetxtTotaleDebiti = "Totale Crediti";
                    additivetxtPercentualeSuTotaleDebiti = "Campione (val/%)";
                    additivetxtPercentualeErroriSustrato = "% errori su strato";
                    additivetxtPercentualeProiettata = "Proiezione errori su popolaz.";
                    additivetxtMaterialita = "Materialità Operativa";
                    additiveNumeroErroriSuCampione = "N° Errori / N° Item";
                    additiveTotaleErrori = "Totale Errori Strato";
                    additiveTotaleErroriCampione = "Totale Errori Campione";
                    additivetxtPercentualeErroriSuCampione = "% errori su campione";
                    break;
                case TipologieCampionamento.Fornitori:
                    additivetxtTotaleSaldiCampione = "Tot. Selezione Strato";
                    additivetxtTotaleDebiti = "Totale Debiti";
                    additivetxtPercentualeSuTotaleDebiti = "Campione (val/%)";
                    additivetxtPercentualeErroriSustrato = "% errori su strato";
                    additivetxtPercentualeProiettata = "Proiezione errori su popolaz.";
                    additivetxtMaterialita = "Materialità Operativa";
                    additiveNumeroErroriSuCampione = "N° Errori / N° Item";
                    additiveTotaleErrori = "Totale Errori Strato";
                    additiveTotaleErroriCampione = "Totale Errori Campione";
                    additivetxtPercentualeErroriSuCampione = "% errori su campione";
                    break;
                case TipologieCampionamento.Magazzino:
                    additivetxtTotaleSaldiCampione = "Tot. Selezione Strato";
                    additivetxtTotaleDebiti = "Totale Inventario";
                    additivetxtPercentualeSuTotaleDebiti = "Campione (val/%)";
                    additivetxtPercentualeErroriSustrato = "% errori su strato";
                    additivetxtPercentualeProiettata = "Proiezione errori su popolaz.";
                    additivetxtMaterialita = "Materialità Operativa";
                    additiveNumeroErroriSuCampione = "N° Errori / N° Item";
                    additiveTotaleErrori = "Totale Errori Strato";
                    additiveTotaleErroriCampione = "Totale Errori Campione";
                    additivetxtPercentualeErroriSuCampione = "% errori su campione";
                    break;
                case TipologieCampionamento.Sconosciuto:
                default:
                    break;
            }

            /*-----------------------------------------------------------------------------
                  if (node.Attributes["Scelta"] != null)
                  {
                    switch (node.Attributes["Scelta"].Value)
                    {
                      case "Ragionato":
                        //additivetxtPercentualeErroriSuCampione = "";
                        break;
                      default:
                        break;
                    }
                  }
            -----------------------------------------------------------------------------*/

            if (additivetxtTotaleSaldiCampione != "")
            {
                try
                {
                    txtTotaleSaldiCampione.Text = ConvertNumber(
                      ALtxtTotaleSaldiCampione[indexhereselected].ToString());
                    lblTotaleSaldiCampione.Text = additivetxtTotaleSaldiCampione;
                    txtTotaleSaldiCampione.Visibility = Visibility.Visible;
                    lblTotaleSaldiCampione.Visibility = Visibility.Visible;
                }
                catch (Exception) { }
            }
            else
            {
                txtTotaleSaldiCampione.Visibility = Visibility.Collapsed;
                lblTotaleSaldiCampione.Visibility = Visibility.Collapsed;
            }

            if (additivetxtTotaleDebiti != "")
            {
                try
                {
                    lblTotaleDebiti.Text = additivetxtTotaleDebiti;
                    txtTotaleDebiti.Text = ConvertNumber(
                      ALtxtTotaleSaldo[indexhereselected].ToString());
                    lblTotaleDebiti.Visibility = Visibility.Visible;
                    txtTotaleDebiti.Visibility = Visibility.Visible;
                }
                catch (Exception) { }
            }
            else
            {
                lblTotaleDebiti.Visibility = Visibility.Collapsed;
                txtTotaleDebiti.Visibility = Visibility.Collapsed;
            }

            totalesaldo = 0;

            if (additivetxtPercentualeSuTotaleDebiti != "")
            {
                try
                {
                    lblPercentualeSuTotaleDebiti.Text =
                      additivetxtPercentualeSuTotaleDebiti;

                    foreach (double ditem in ALtxtTotaleSaldiCampione)
                        totalesaldo += ditem;
                    txtPercentualeSuTotaleDebiti.Text = ConvertNumber(
                      totalesaldo.ToString()) + " / " +
                      ConvertNumber(
                        (100.0 * Math.Round(totalesaldo, 2)
                          / Math.Round((double)(ALtxtTotaleSaldo[indexhereselected]), 2))
                            .ToString());
                    lblPercentualeSuTotaleDebiti.Visibility = Visibility.Visible;
                    txtPercentualeSuTotaleDebiti.Visibility = Visibility.Visible;
                }
                catch (Exception) { }
            }
            else
            {
                lblPercentualeSuTotaleDebiti.Visibility = Visibility.Collapsed;
                txtPercentualeSuTotaleDebiti.Visibility = Visibility.Collapsed;
            }

            switch (_tipologia)
            {
                case TipologieCampionamento.Clienti:
                    labelTitolo.Content = "Clienti";
                    break;
                case TipologieCampionamento.Fornitori:
                    labelTitolo.Content = "Fornitori";
                    break;
                case TipologieCampionamento.Magazzino:
                    labelTitolo.Content = "Rimanenze di Magazzino";
                    break;
            }

            labelTitolo.Content += "  -  Campioni estratti per la rilevazione " +
              "degli errori";

            stpFinal.Visibility = Visibility.Visible;
            stpFinal_btn.Visibility = Visibility.Visible;

            grdFinalHeader.Children.Clear();
            grdFinalHeader.ColumnDefinitions.Clear();
            grdFinalHeader.RowDefinitions.Clear();
            grdFinalHeader.Width = 1270;
            grdFinalHeader.MaxWidth = 1270;
            grdFinalHeader.MinWidth = 1270;
            grdFinal.Children.Clear();
            grdFinal.ColumnDefinitions.Clear();
            grdFinal.RowDefinitions.Clear();
            grdFinal.Width = 1270;
            grdFinal.MaxWidth = 1270;
            grdFinal.MinWidth = 1270;

            ColumnDefinition cd;

            real_i = -1;

            for (i = 1; i < FinalData.Tables[0].Columns.Count; i++)
            {
                /*-----------------------------------------------------------------------------
                        if (node.Attributes["stpStratificazionesino"] != null
                          && node.Attributes["stpStratificazionesino"].Value != "SI")
                -----------------------------------------------------------------------------*/
                str = "stpStratificazionesino";
                if (EsisteAttributo(str) && GetRigaAttributo(str) != "SI")
                {
                    if (FinalData.Tables[0].Columns[i].ColumnName == "Stratificazione")
                        continue;
                }
                /*-----------------------------------------------------------------------------
                        if (FinalData.Tables[0].Columns[i].ColumnName == "Esito"
                          && node.Attributes["Final_Choice"] != null
                          && node.Attributes["Final_Choice"].Value != "Final_Circolarizzazione")
                -----------------------------------------------------------------------------*/
                str = "Final_Choice";
                if (FinalData.Tables[0].Columns[i].ColumnName == "Esito"
                  && EsisteAttributo(str)
                  && GetRigaAttributo(str) != "Final_Circolarizzazione")
                    continue;

                if (FinalData.Tables[0].Columns[i].ColumnName == "Indirizzo")
                    continue;

                real_i++;

                cd = new ColumnDefinition();
                if (i == 2) cd.Width = new GridLength(6, GridUnitType.Star);
                else
                {
                    if (FinalData.Tables[0].Columns[i].ColumnName == "Esito")
                        cd.Width = new GridLength(4, GridUnitType.Star);
                    else cd.Width = new GridLength(2, GridUnitType.Star);
                }

                grdFinalHeader.ColumnDefinitions.Add(cd);

                cd = new ColumnDefinition();
                if (i == 2) cd.Width = new GridLength(6, GridUnitType.Star);
                else
                {
                    if (FinalData.Tables[0].Columns[i].ColumnName == "Esito")
                        cd.Width = new GridLength(4, GridUnitType.Star);
                    else cd.Width = new GridLength(2, GridUnitType.Star);
                }

                grdFinal.ColumnDefinitions.Add(cd);
            }

            //--------------------------------------------------------------- HEADERS
            row = 0;
            RowDefinition rd;
            System.Windows.Controls.Border brd;
            TextBlock lbl;

            real_i = -1;

            totalErroriRilevati = 0;

            for (i = 1; i < FinalData.Tables[0].Columns.Count; i++)
            {
                rd = new RowDefinition();
                grdFinalHeader.RowDefinitions.Add(rd);
                /*-----------------------------------------------------------------------------
                        if (node.Attributes["stpStratificazionesino"] != null
                          && node.Attributes["stpStratificazionesino"].Value != "SI")
                -----------------------------------------------------------------------------*/
                str = "stpStratificazionesino";
                if (EsisteAttributo(str) && GetRigaAttributo(str) != "SI")
                {
                    if (FinalData.Tables[0].Columns[i].ColumnName == "Stratificazione")
                        continue;
                }
                /*-----------------------------------------------------------------------------
                        if (FinalData.Tables[0].Columns[i].ColumnName == "Esito"
                          && node.Attributes["Final_Choice"] != null
                          && node.Attributes["Final_Choice"].Value != "Final_Circolarizzazione")
                -----------------------------------------------------------------------------*/
                str = "Final_Choice";
                if (FinalData.Tables[0].Columns[i].ColumnName == "Esito"
                  && EsisteAttributo(str)
                  && GetRigaAttributo(str) != "Final_Circolarizzazione")
                    continue;

                if (FinalData.Tables[0].Columns[i].ColumnName == "Indirizzo") continue;

                real_i++;

                brd = new System.Windows.Controls.Border();
                brd.BorderThickness = new Thickness(1.0);
                brd.BorderBrush = Brushes.LightGray;
                brd.Background = Brushes.LightGray;
                brd.Padding = new Thickness(2.0);

                lbl = new TextBlock();

                lbl.TextAlignment = TextAlignment.Center;
                lbl.Text = FinalData.Tables[0].Columns[i].ColumnName;

                lbl.TextWrapping = TextWrapping.Wrap;
                lbl.FontWeight = FontWeights.Bold;

                brd.Child = lbl;

                grdFinalHeader.Children.Add(brd);
                Grid.SetRow(brd, row);
                Grid.SetColumn(brd, real_i);
            }

            numerrori = 0;
            numcampioni = 0;
            totalErroriRilevatiStrato = 0;

            for (j = 0; j < FinalData.Tables[0].Rows.Count; j++)
            {
                for (i = 1; i < FinalData.Tables[0].Columns.Count; i++)
                {
                    if (FinalData.Tables[0].Columns[i].ColumnName == "Errore Rilevato")
                    {
                        serrore = FinalData.Tables[0].Rows[j][i].ToString();
                        errorehere = 0; double.TryParse(serrore, out errorehere);
                        totalErroriRilevati += errorehere;
                    }
                }
            }

            for (j = 0; j < FinalData.Tables[0].Rows.Count; j++)
            {
                if (stratificazionescelta != ""
                  && stratificazionescelta != "|"
                  && stratificazionescelta != FinalData.Tables[0].Rows[j][0].ToString())
                    continue;

                real_i = -1;

                for (i = 1; i < FinalData.Tables[0].Columns.Count; i++)
                {
                    rd = new RowDefinition();
                    grdFinal.RowDefinitions.Add(rd);
                    /*-----------------------------------------------------------------------------
                              if (FinalData.Tables[0].Columns[i].ColumnName == "Esito"
                                && node.Attributes["Final_Choice"] != null
                                && node.Attributes["Final_Choice"].Value != "Final_Circolarizzazione")
                    -----------------------------------------------------------------------------*/
                    str = "Final_Choice";
                    if (FinalData.Tables[0].Columns[i].ColumnName == "Esito"
                      && EsisteAttributo(str)
                      && GetRigaAttributo(str) != "Final_Circolarizzazione")
                        continue;

                    if (FinalData.Tables[0].Columns[i].ColumnName == "Indirizzo") continue;

                    real_i++;

                    brd = new System.Windows.Controls.Border();
                    brd.BorderThickness = new Thickness(1.0);
                    brd.BorderBrush = Brushes.LightGray;
                    brd.Padding = new Thickness(2.0);

                    if (FinalData.Tables[0].Columns[i].ColumnName == "Esito")
                    {
                        ComboBox cmb = new ComboBox();

                        cmb.PreviewKeyDown += obj_PreviewKeyDown;
                        cmb.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDown;

                        cmb.Tag = i.ToString() + "_" + j.ToString();

                        cmb.Items.Add("(Selezionare)");
                        cmb.Items.Add("A = RISPOSTA IN ACCORDO");
                        cmb.Items.Add("B = RISPOSTA RICONCILIATA SENZA ECCEZIONI");
                        cmb.Items.Add("C = RISPOSTA RICONCILIATA CON ECCEZIONI");
                        cmb.Items.Add("D = NON RISPOSTO - PROCEDURE ALTERNATIVE");

                        cmb.SelectedItem = FinalData.Tables[0].Rows[j][i].ToString();

                        cmb.SelectionChanged += Cmb_SelectionChanged;

                        brd.Child = cmb;
                    }
                    else if (FinalData.Tables[0].Columns[i].ColumnName == "Errore Rilevato")
                    {
                        TextBox txt = new TextBox();
                        txt.PreviewKeyDown += obj_PreviewKeyDown;
                        txt.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDown;

                        txt.Tag = i.ToString() + "_" + j.ToString();

                        txt.Name = "txterrore_" + j.ToString();
                        try { this.UnregisterName(txt.Name); }
                        catch (Exception) { }
                        this.RegisterName(txt.Name, txt);

                        txt.Text = FinalData.Tables[0].Rows[j][i].ToString();

                        errorehere = 0; double.TryParse(txt.Text, out errorehere);
                        if (errorehere != 0) numerrori++;
                        numcampioni++;

                        totalErroriRilevatiStrato += errorehere;

                        txt.TextAlignment = TextAlignment.Right;
                        txt.TextWrapping = TextWrapping.Wrap;
                        txt.LostFocus += Txt_LostFocus;
                        brd.Child = txt;
                    }
                    else
                    {
                        lbl = new TextBlock();

                        if (FinalData.Tables[0].Columns[i].ColumnName == "Saldo")
                        {
                            lbl.TextAlignment = TextAlignment.Right;
                            lbl.Text = ConvertNumber(
                              FinalData.Tables[0].Rows[j][i].ToString());
                        }
                        else if (FinalData.Tables[0].Columns[i].ColumnName == "Descrizione")
                        {
                            lbl.TextAlignment = TextAlignment.Left;
                            lbl.Text = FinalData.Tables[0].Rows[j][i].ToString();
                        }
                        else
                        {
                            lbl.TextAlignment = TextAlignment.Center;
                            lbl.Text = FinalData.Tables[0].Rows[j][i].ToString();
                        }

                        lbl.TextWrapping = TextWrapping.Wrap;
                        brd.Child = lbl;
                    }
                    grdFinal.Children.Add(brd);
                    Grid.SetRow(brd, row);
                    Grid.SetColumn(brd, real_i);
                }
                row++;
            }

            if (additivetxtPercentualeErroriSuCampione != "")
            {
                try
                {
                    lblTotaleErroriCampione.Text = additiveTotaleErroriCampione;
                    txtTotaleErroriCampione.Text = ConvertNumber(
                      totalErroriRilevatiStrato.ToString());

                    lblPercentualeErroriSustrato.Text = additivetxtPercentualeErroriSustrato;
                    txtPercentualeErroriSustrato.Text = ConvertNumber(
                      (100.0 * Math.Round(totalErroriRilevatiStrato, 2)
                        / Math.Round((double)(ALtxtTotaleSaldiCampione[indexhereselected]), 2))
                          .ToString());

                    lblPercentualeErroriSuCampione.Text = additivetxtPercentualeErroriSuCampione;
                    txtPercentualeErroriSuCampione.Text = ConvertNumber(
                      (100.0 * Math.Round(totalErroriRilevati, 2)
                        / Math.Round(totalesaldo, 2)).ToString());
                    lblPercentualeErroriSuCampione.Visibility = Visibility.Visible;
                    txtPercentualeErroriSuCampione.Visibility = Visibility.Visible;

                    lblPercentualeProiettata.Text = additivetxtPercentualeProiettata;
                    txtPercentualeProiettata.Text = ConvertNumber(
                      (Math.Round((double)(ALtxtTotaleSaldo[indexhereselected]), 2)
                        * Math.Round(totalErroriRilevati, 2) / Math.Round(totalesaldo, 2))
                          .ToString());

                    lblPercentualeProiettata.Visibility = Visibility.Visible;
                    txtPercentualeProiettata.Visibility = Visibility.Visible;

                    lblMaterialità.Text = additivetxtMaterialita;
                    txtMaterialità.Text = MaterialitaOperativa;

                    lblMaterialità.Visibility = Visibility.Visible;
                    txtMaterialità.Visibility = Visibility.Visible;

                    lblNumeroErroriSuCampione.Text = additiveNumeroErroriSuCampione;
                    lblTotaleErrori.Text = additiveTotaleErrori;

                    txtNumeroErroriSuCampione.Text = numerrori.ToString() + " / " +
                      numcampioni.ToString();
                    txtTotaleErrori.Text = ConvertNumber(totalErroriRilevati.ToString());

                    lblNumeroErroriSuCampione.Visibility = Visibility.Visible;
                    lblTotaleErrori.Visibility = Visibility.Visible;
                    txtNumeroErroriSuCampione.Visibility = Visibility.Visible;
                    txtTotaleErrori.Visibility = Visibility.Visible;
                }
                catch (Exception) { }
            }
            else
            {
                lblPercentualeErroriSuCampione.Visibility = Visibility.Collapsed;
                txtPercentualeErroriSuCampione.Visibility = Visibility.Collapsed;

                lblPercentualeProiettata.Visibility = Visibility.Collapsed;
                txtPercentualeProiettata.Visibility = Visibility.Collapsed;

                lblNumeroErroriSuCampione.Visibility = Visibility.Collapsed;
                lblTotaleErrori.Visibility = Visibility.Collapsed;
                txtNumeroErroriSuCampione.Visibility = Visibility.Collapsed;
                txtTotaleErrori.Visibility = Visibility.Collapsed;
            }

            if (tabFinal.Items.Count <= 1)
            {
                lblTotaleSaldiCampione.Visibility = Visibility.Collapsed;
                txtTotaleSaldiCampione.Visibility = Visibility.Collapsed;
                lblNumeroErroriSuCampione.Visibility = Visibility.Collapsed;
                txtNumeroErroriSuCampione.Visibility = Visibility.Collapsed;
                lblTotaleErrori.Visibility = Visibility.Collapsed;
                txtTotaleErroriCampione.Visibility = Visibility.Collapsed;
                lblPercentualeErroriSustrato.Visibility = Visibility.Collapsed;
                txtPercentualeErroriSustrato.Visibility = Visibility.Collapsed;
            }
            /*-----------------------------------------------------------------------------
                  if (node.Attributes["Final_Choice"] != null
                    && node.Attributes["Final_Choice"].Value == "Final_Circolarizzazione")
            -----------------------------------------------------------------------------*/
            str = "Final_Choice";
            if (EsisteAttributo(str)
              && GetRigaAttributo(str) == "Final_Circolarizzazione")
                btnCircolarizzazione.Visibility = Visibility.Visible;
            else btnCircolarizzazione.Visibility = Visibility.Hidden;
            /*-----------------------------------------------------------------------------
                  if (node.Attributes["Scelta"] != null)
                  {
                    switch (node.Attributes["Scelta"].Value)
                    {
                      case "Ragionato":
                        lblPercentualeProiettata.Visibility = Visibility.Collapsed;
                        txtPercentualeProiettata.Visibility = Visibility.Collapsed;
                        break;
                      default:
                        break;
                    }
                  }
            -----------------------------------------------------------------------------*/
            str = "Scelta";
            if (EsisteAttributo(str) && GetRigaAttributo(str) == "Ragionato")
            {
                lblPercentualeProiettata.Visibility = Visibility.Collapsed;
                txtPercentualeProiettata.Visibility = Visibility.Collapsed;
            }
          ((WindowWorkArea)(this.Owner))._x.Save();
            cBusinessObjects.SaveData(nodeNumber, dataCampionamento, typeof(Campionamento));
        }

        //------------------------------------------------------------------------+
        //                             Txt_LostFocus                              |
        //------------------------------------------------------------------------+
        private void Txt_LostFocus(object sender, RoutedEventArgs e)
        {
            double errorehere, totalErroriRilevati, totalErroriRilevatiStrato, totalesaldo;
            int indexhereselected, j, numcampioni, numerrori;
            string additivetxtPercentualeErroriSuCampione, str;
            string additivetxtPercentualeProiettata, result, sFinalData;
            string stratificazionescelta;
            string[] splitted;

            /*-----------------------------------------------------------------------------
                  if (node != null
                    && node.Attributes["FinalData"] != null
                    && node.Attributes["FinalData"].Value != "<NewDataSet />")
            -----------------------------------------------------------------------------*/
            sFinalData = dataCampionamento.Rows[0]["FinalData"].ToString();
            if (node != null && !string.IsNullOrEmpty(sFinalData)
              && sFinalData != "<NewDataSet />")
            {
                //using (StringReader sw = new StringReader(node.Attributes["FinalData"].Value))
                using (StringReader sw = new StringReader(sFinalData))
                {
                    FinalData = new DataSet();
                    FinalData.ReadXml(sw);
                }
            }
            stratificazionescelta = "";
            indexhereselected = 0;
            if (tabFinal.SelectedItem == null) tabFinal.SelectedIndex = 0;
            if (tabFinal.SelectedIndex != -1)
            {
                if (tabFinal.Items.Count == 1)
                {
                    indexhereselected = 0;
                    stratificazionescelta = ((TabItem)(tabFinal.Items[0])).Tag.ToString();
                }
                else
                {
                    TabItem ti = (TabItem)(tabFinal.SelectedItem);
                    stratificazionescelta = ti.Tag.ToString();
                    indexhereselected = CompleteListStratification.IndexOf(ti.Tag.ToString());
                }
            }
            splitted = ((TextBox)sender).Tag.ToString().Split('_');
            ((TextBox)sender).Text = ConvertNumber(((TextBox)sender).Text);
            FinalData.Tables[0].Rows
              [Convert.ToInt32(splitted[1])]
              [Convert.ToInt32(splitted[0])] = ((TextBox)sender).Text;

            numerrori = 0;
            numcampioni = 0;
            totalErroriRilevati = 0;
            totalErroriRilevatiStrato = 0;
            for (j = 0; j < FinalData.Tables[0].Rows.Count; j++)
            {
                errorehere = 0;
                double.TryParse(FinalData.Tables[0].Rows
                  [j][Convert.ToInt32(splitted[0])].ToString(), out errorehere);
                totalErroriRilevati += errorehere;
                if (stratificazionescelta != "" && stratificazionescelta != "|"
                  && stratificazionescelta != FinalData.Tables[0].Rows[j][0].ToString())
                    continue;
                if (errorehere != 0) numerrori++;
                numcampioni++;
                totalErroriRilevatiStrato += errorehere;
            }

            additivetxtPercentualeErroriSuCampione = "";

            switch (_tipologia)
            {
                case TipologieCampionamento.Clienti:
                    additivetxtPercentualeErroriSuCampione = "% errori sul campione";
                    break;
                case TipologieCampionamento.Fornitori:
                    additivetxtPercentualeErroriSuCampione = "% errori sul campione";
                    break;
                case TipologieCampionamento.Magazzino:
                    additivetxtPercentualeErroriSuCampione = "% errori sul campione";
                    break;
                case TipologieCampionamento.Sconosciuto:
                default:
                    break;
            }
            /*-----------------------------------------------------------------------------
                  if (node.Attributes["Scelta"] != null)
                  {
                    switch (node.Attributes["Scelta"].Value)
                    {
                      case "Ragionato":
                        //additivetxtPercentualeErroriSuCampione = "";
                        break;
                      default:
                        break;
                    }
                  }
            -----------------------------------------------------------------------------*/
            additivetxtPercentualeProiettata = "Proiezione errori su popolaz.";
            totalesaldo = 0;
            foreach (double ditem in ALtxtTotaleSaldiCampione) totalesaldo += ditem;
            if (additivetxtPercentualeErroriSuCampione != "")
            {
                try
                {
                    txtTotaleErroriCampione.Text = ConvertNumber(
                      totalErroriRilevatiStrato.ToString());
                    txtPercentualeErroriSustrato.Text = ConvertNumber(
                      (100.0 * Math.Round(totalErroriRilevatiStrato, 2)
                        / Math.Round((double)(ALtxtTotaleSaldiCampione[indexhereselected]), 2))
                          .ToString());
                    lblPercentualeErroriSuCampione.Text =
                      additivetxtPercentualeErroriSuCampione;
                    txtPercentualeErroriSuCampione.Text = ConvertNumber(
                      (100.0 * Math.Round(totalErroriRilevati, 2)
                        / Math.Round(totalesaldo, 2)).ToString());
                    txtPercentualeErroriSuCampione.Visibility = Visibility.Visible;
                    lblPercentualeProiettata.Text = additivetxtPercentualeProiettata;
                    txtPercentualeProiettata.Text = ConvertNumber((Math.Round((double)(ALtxtTotaleSaldo[indexhereselected]), 2) * Math.Round(totalErroriRilevati, 2) / Math.Round(totalesaldo, 2)).ToString()); //(double)(ALtxtTotaleSaldiCampione[indexhereselected])).ToString())
                    lblPercentualeProiettata.Visibility = Visibility.Visible;
                    txtPercentualeProiettata.Visibility = Visibility.Visible;
                    txtNumeroErroriSuCampione.Text = numerrori.ToString() + " / " + numcampioni.ToString();
                    txtTotaleErrori.Text = ConvertNumber(totalErroriRilevati.ToString());
                }
                catch (Exception) { }
            }
            else
            {
                txtPercentualeErroriSuCampione.Visibility = Visibility.Collapsed;
                lblPercentualeProiettata.Visibility = Visibility.Collapsed;
                txtPercentualeProiettata.Visibility = Visibility.Collapsed;
            }
            using (StringWriter sw = new StringWriter())
            {
                FinalData.WriteXml(sw);
                result = sw.ToString();
            }
            if (node != null)
            {
                if (node.Attributes["FinalData"] == null)
                {
                    XmlAttribute attr = node.OwnerDocument.CreateAttribute("FinalData");
                    node.Attributes.Append(attr);
                }
                node.Attributes["FinalData"].Value = result;
                dataCampionamento.Rows[0]["FinalData"] = result;
            }
            if (tabFinal.Items.Count <= 1)
            {
                lblTotaleSaldiCampione.Visibility = Visibility.Collapsed;
                txtTotaleSaldiCampione.Visibility = Visibility.Collapsed;
                lblNumeroErroriSuCampione.Visibility = Visibility.Collapsed;
                txtNumeroErroriSuCampione.Visibility = Visibility.Collapsed;
                lblTotaleErrori.Visibility = Visibility.Collapsed;
                txtTotaleErroriCampione.Visibility = Visibility.Collapsed;
                lblPercentualeErroriSustrato.Visibility = Visibility.Collapsed;
                txtPercentualeErroriSustrato.Visibility = Visibility.Collapsed;
            }
            /*-----------------------------------------------------------------------------
                  if (node.Attributes["Scelta"] != null)
                  {
                    switch (node.Attributes["Scelta"].Value)
                    {
                      case "Ragionato":
                        lblPercentualeProiettata.Visibility = Visibility.Collapsed;
                        txtPercentualeProiettata.Visibility = Visibility.Collapsed;
                        break;
                      default:
                        break;
                    }
                  }
            -----------------------------------------------------------------------------*/
            str = "Scelta";
            if (EsisteAttributo(str) && GetRigaAttributo(str) == "Ragionato")
            {
                lblPercentualeProiettata.Visibility = Visibility.Collapsed;
                txtPercentualeProiettata.Visibility = Visibility.Collapsed;
            }
          ((WindowWorkArea)(this.Owner))._x.Save();
        }

        //------------------------------------------------------------------------+
        //                          Cmb_SelectionChanged                          |
        //------------------------------------------------------------------------+
        private void Cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string result;
            string[] splitted;

            splitted = ((ComboBox)sender).Tag.ToString().Split('_');
            FinalData.Tables[0].Rows
              [Convert.ToInt32(splitted[1])][Convert.ToInt32(splitted[0])] =
                ((ComboBox)sender).SelectedItem;
            TextBox txt = (TextBox)this.FindName("txterrore_" + splitted[1]);
            if (((ComboBox)sender).SelectedItem.ToString() == "A = RISPOSTA IN ACCORDO"
              || ((ComboBox)sender).SelectedItem.ToString() == "B = RISPOSTA RICONCILIATA SENZA ECCEZIONI")
            {
                txt.Text = "0";
                txt.IsReadOnly = true;
            }
            else txt.IsReadOnly = false;
            using (StringWriter sw = new StringWriter())
            {
                FinalData.WriteXml(sw);
                result = sw.ToString();
            }
            if (dataCampionamento.Rows.Count > 0)
                dataCampionamento.Rows[0]["FinalData"] = result;
            if (node != null)
            {
                if (node.Attributes["FinalData"] == null)
                {
                    XmlAttribute attr = node.OwnerDocument.CreateAttribute("FinalData");
                    node.Attributes.Append(attr);
                }
                node.Attributes["FinalData"].Value = result;
                dataCampionamento.Rows[0]["FinalData"] = result;
            }
          ((WindowWorkArea)(this.Owner))._x.Save();
        }

        #endregion

        #region ALTRO

        //------------------------------------------------------------------------+
        //                            GetDataFromExcel                            |
        //------------------------------------------------------------------------+
        private void GetDataFromExcel(bool erase)
        {
            int firstrow, i, j;
            string ce, result, str, str2;

            esistealmenounavoce = false;
            firstrow = rowintestazione + 1;
            if (erase) RawData = null;
            ExcelRange objRange = null;
            if (RawData == null)
            {
                RawData = new DataSet();
                System.Data.DataTable dataTable = new System.Data.DataTable();
                dataTable.TableName = "dataTable";
                for (i = 0; i < colonne.Count; i++) dataTable.Columns.Add(colonne[i]);
                for (i = firstrow; i <= lastRowIncludeFormulas; i++)
                {
                    List<string> tmparray = new List<string>();
                    if (excelSheet != null)
                    {
                        esistealmenounavoce = true;
                        for (j = 0; j < colonne.Count; j++)
                        {
                            ComboBox lst = (ComboBox)this.FindName("lst_" + j.ToString());
                            if (lst == null || lst.SelectedItem == null) tmparray.Add("");
                            else
                            {
                                try
                                {
                                    ce = ((ComboboxItem)(lst.SelectedItem)).Value.ToString() +
                                      i.ToString() + ":" +
                                      ((ComboboxItem)(lst.SelectedItem)).Value.ToString() +
                                      i.ToString();
                                    objRange = excelSheet.Cells[ce];
                                    if (objRange.Merge)
                                    {
                                        tmparray.Add(
                                          Convert.ToString(
                                            (excelSheet.Cells[1, 1]).Text).Trim().Replace("\"", "")
                                              .Replace("&", "").Replace("<", "").Replace(">", ""));
                                    }
                                    else
                                    {
                                        tmparray.Add(
                                          Convert.ToString(
                                            objRange.Text).Trim().Replace("\"", "").Replace("&", "")
                                              .Replace("<", "").Replace(">", ""));
                                    }
                                }
                                catch (Exception) { tmparray.Add(""); }
                            }
                        }
                        dataTable.Rows.Add(tmparray.ToArray());
                    }
                }
                RawData.Tables.Add(dataTable);
                using (StringWriter sw = new StringWriter())
                {
                    RawData.WriteXml(sw);
                    result = sw.ToString();
                }
                if (node != null)
                {
                    if (node.Attributes["RawData"] == null)
                    {
                        XmlAttribute attr = node.OwnerDocument.CreateAttribute("RawData");
                        node.Attributes.Append(attr);
                    }
                    node.Attributes["RawData"].Value = result;
                    dataCampionamento.Rows[0]["RawData"] = result;
                    ComboBox lst_Intestazione = (ComboBox)this.FindName("lst_Intestazione");
                    if (lst_Intestazione != null)
                    {
                        if (node.Attributes["lst_Intestazione"] == null)
                        {
                            XmlAttribute attr =
                              node.OwnerDocument.CreateAttribute("lst_Intestazione");
                            node.Attributes.Append(attr);
                        }
                        node.Attributes["lst_Intestazione"].Value =
                          lst_Intestazione.SelectedValue.ToString();
                        SetRigaAttributo("lst_Intestazione",
                          lst_Intestazione.SelectedValue.ToString());
                    }
                    for (i = 0; i < colonne.Count; i++)
                    {
                        ComboBox lsthere = (ComboBox)this.FindName("lst_" + i.ToString());
                        if (lsthere != null && lsthere.SelectedItem != null)
                        {
                            str = string.Format("lst_{0}", i);
                            if (node.Attributes[str] == null)
                            {
                                XmlAttribute attr = node.OwnerDocument.CreateAttribute(str);
                                node.Attributes.Append(attr);
                            }
                            str2 = lsthere.SelectedValue.ToString();
                            node.Attributes[str].Value = str2;
                            SetRigaAttributo(str, str2);
                        }
                    }

                    ComboBox lst_attr = (ComboBox)this.FindName("lst_attr");
                    if (lst_attr != null && lst_attr.SelectedValue != null)
                    {
                        str = "lst_attr";
                        if (node.Attributes[str] == null)
                        {
                            XmlAttribute attr = node.OwnerDocument.CreateAttribute(str);
                            node.Attributes.Append(attr);
                        }
                        str2 = lst_attr.SelectedValue.ToString();
                        node.Attributes[str].Value = str2;
                        SetRigaAttributo(str, str2);
                    }

                    TextBox txt_attr = (TextBox)this.FindName("txt_attr");
                    if (lst_attr != null)
                    {
                        str = "txt_attr";
                        if (node.Attributes[str] == null)
                        {
                            XmlAttribute attr = node.OwnerDocument.CreateAttribute(str);
                            node.Attributes.Append(attr);
                        }
                        str2 = txt_attr.Text;
                        node.Attributes[str].Value = str2;
                        SetRigaAttributo(str, str2);
                    }
                }
            }
          ((WindowWorkArea)(this.Owner))._x.Save();
        }

        //------------------------------------------------------------------------+
        //                       ColumnIndexToColumnLetter                        |
        //------------------------------------------------------------------------+
        private string ColumnIndexToColumnLetter(int colIndex)
        {
            int div, mod;
            string colLetter;

            div = colIndex; mod = 0; colLetter = string.Empty;
            while (div > 0)
            {
                mod = (div - 1) % 26;
                colLetter = (char)(65 + mod) + colLetter;
                div = (int)((div - mod) / 26);
            }
            return colLetter;
        }

        //------------------------------------------------------------------------+
        //                             ConvertNumber                              |
        //------------------------------------------------------------------------+
        private string ConvertNumber(string valore)
        {
            double dblValore;

            dblValore = 0.0; double.TryParse(valore, out dblValore);
            return (dblValore == 0.0) ?
              "0,00" : string.Format("{0:#,0.00}", dblValore);
        }

        //------------------------------------------------------------------------+
        //                            ConvertNumberNeg                            |
        //------------------------------------------------------------------------+
        private string ConvertNumberNeg(string valore)
        {
            double dblValore;

            dblValore = 0.0; double.TryParse(valore, out dblValore);
            return (dblValore == 0.0) ?
              "0,00" : string.Format("{0:#,#.00}", dblValore * (-1.0));
        }

        //------------------------------------------------------------------------+
        //                         ConvertNumberNoDecimal                         |
        //------------------------------------------------------------------------+
        private string ConvertNumberNoDecimal(string valore)
        {
            double dblValore;

            dblValore = 0.0; double.TryParse(valore, out dblValore);
            return (dblValore == 0.0) ?
              "0" : string.Format("{0:#,0}", Math.Round(dblValore));
        }

        #endregion

        //------------------------------------------------------------------------+
        //                   rdbFinal_Circolarizzazione_Checked                   |
        //------------------------------------------------------------------------+
        private void rdbFinal_Circolarizzazione_Checked(object sender, RoutedEventArgs e)
        {
            if (node.Attributes["Final_Choice"] == null)
            {
                XmlAttribute attr = node.OwnerDocument.CreateAttribute("Final_Choice");
                node.Attributes.Append(attr);
            }

            if (rdbFinal_Circolarizzazione.IsChecked == true)
            {
                node.Attributes["Final_Choice"].Value = "Final_Circolarizzazione";
                SetRigaAttributo("Final_Choice", "Final_Circolarizzazione");
            }
            else
            {
                node.Attributes["Final_Choice"].Value = "Final_Procedure";
                SetRigaAttributo("Final_Choice", "Final_Procedure");
            }
          ((WindowWorkArea)(this.Owner))._x.Save();
        }

        //------------------------------------------------------------------------+
        //                       tabCampionamento_Calculate                       |
        //------------------------------------------------------------------------+
        private void tabCampionamento_Calculate()
        {
            bool thereisatleastoneintervallo;
            double dMaterialitaOperativa, dR, firstvalue, herevalue, J, secondvalue;
            double totalesaldo, totalesaldocomplessivo;
            int counterinterno, countertotale, i, indexCompleteListStratification;
            int j, numerototaleitemcasuali, r, rMUS, startingRmus;
            List<int> rowsIntermediatevalue, rowsstratificate, rowsstratificate_scelte;
            Random rand, randMUS;
            string attributo, intervallo, str, str2;
            string[] splittedintervallo;

            if (node.Attributes["Final_Choice"] == null)
            {
                XmlAttribute attr = node.OwnerDocument.CreateAttribute("Final_Choice");
                node.Attributes.Append(attr);
            }

            //if (node.Attributes["Final_Choice"].Value == "Final_Circolarizzazione")
            if (GetRigaAttributo("Final_Choice") == "Final_Circolarizzazione")
            {
                rdbFinal_Circolarizzazione.IsChecked = true;
            }
            else
            {
                node.Attributes["Final_Choice"].Value = "Final_Procedure";
                SetRigaAttributo("Final_Choice", "Final_Procedure");
                rdbFinal_Procedure.IsChecked = true;
            }

            tabCampionamento.Items.Clear();
            indexCompleteListStratification = -1;
            ALrowsstratificate_scelte.Clear();
            ALrowsstratificate.Clear();
            ALrowsIntermediatevalue.Clear();
            //ALtxtTotaleSaldiCampione.Clear();
            ALtxtTotaleSaldo.Clear();
            ALtxtTipoCampionamento_Info.Clear();

            if (CompleteListStratification.Count == 1
              && (string)(CompleteListStratification[0]) == "Nessuna Stratificazione")
                CompleteListStratification[0] = "|";

            foreach (string stratificationhere in CompleteListStratification)
            {
                indexCompleteListStratification++;
                attributo = ""; intervallo = "";
                if (stratificationhere.ToString().Split('|').Count() > 1)
                {
                    attributo = stratificationhere.ToString().Split('|')[0];
                    intervallo = stratificationhere.ToString().Split('|')[1];
                }

                rowsstratificate_scelte = new List<int>();
                rowsstratificate = new List<int>();
                rowsIntermediatevalue = new List<int>();

                totalesaldo = 0; totalesaldocomplessivo = 0; J = 0;
                dMaterialitaOperativa = 0; dR = 0;

                double.TryParse(MaterialitaBilancio, out dMaterialitaOperativa);
                if (dMaterialitaOperativa == 0)
                    double.TryParse(MaterialitaOperativa, out dMaterialitaOperativa);
                switch (RischioIndividuazione.ToUpper())
                {
                    case "MOLTO ALTO":
                        dR = 0.7;
                        break;
                    case "ALTO":
                        dR = 1;
                        break;
                    case "MEDIO":
                        dR = 1.5;
                        break;
                    case "BASSO":
                        dR = 2;
                        break;
                    case "MOLTO BASSO":
                        dR = 3;
                        break;
                }

                J = (dR == 0 || dMaterialitaOperativa == 0) ?
                  0 : dMaterialitaOperativa / dR;

                for (j = 0; j < RawData.Tables[0].Rows.Count; j++)
                {
                    /*-----------------------------------------------------------------------------
                              if (node.Attributes["RigheCancellate"] != null
                                && node.Attributes["RigheCancellate"].Value.Split('|').Contains(
                                  RawData.Tables[0].Rows[j][0].ToString() + "-" +
                                    RawData.Tables[0].Rows[j][1].ToString()))
                    -----------------------------------------------------------------------------*/
                    str = "RigheCancellate";
                    if (EsisteAttributo(str)
                      && GetRigaAttributo(str).Split('|').Contains(
                        RawData.Tables[0].Rows[j][0].ToString() + "-" +
                          RawData.Tables[0].Rows[j][1].ToString()))
                        continue;
                    thereisatleastoneintervallo = false;
                    herevalue = 0;
                    double.TryParse(
                      RawData.Tables[0].Rows
                        [j][Convert.ToInt32(indexcolumnsaldo)].ToString(), out herevalue);
                    totalesaldocomplessivo += herevalue;
                    if (attributo != ""
                      && attributo != RawData.Tables[0].Rows
                        [j][(RawData.Tables[0].Columns.Count - 1)].ToString())
                        continue;
                    if (intervallo != "")
                    {
                        firstvalue = 0; secondvalue = 0;
                        splittedintervallo = intervallo.Split(' ');
                        double.TryParse(splittedintervallo[4], out firstvalue);
                        double.TryParse(splittedintervallo[6], out secondvalue);
                        if (herevalue >= firstvalue && herevalue <= secondvalue)
                            thereisatleastoneintervallo = true;
                    }
                    else thereisatleastoneintervallo = true;
                    if (thereisatleastoneintervallo)
                    {
                        rowsstratificate.Add(j);
                        totalesaldo += herevalue;
                    }
                }
                ALtxtTotaleSaldo.Add(totalesaldocomplessivo);
                //if (node.Attributes["Scelta"] != null)
                if (EsisteAttributo("Scelta"))
                {
                    //switch (node.Attributes["Scelta"].Value)
                    switch (GetRigaAttributo("Scelta"))
                    {
                        case "Casuale":
                            rowsstratificate_scelte.Clear();
                            foreach (int item in rowsstratificate)
                                rowsstratificate_scelte.Add(item);
                            if (J == 0)
                            {
                                if (dR == 0)
                                {
                                    ALtxtTipoCampionamento_Info.Add(
                                      "Rischio di individuazione non selezionato");
                                }
                                else
                                {
                                    ALtxtTipoCampionamento_Info.Add(
                                      "Materialità operativa non inserita");
                                }
                            }
                            else
                            {
                                numerototaleitemcasuali = Math.Abs(
                                  Convert.ToInt32(Math.Ceiling(totalesaldo / J)));
                                if (numerototaleitemcasuali > rowsstratificate.Count)
                                    numerototaleitemcasuali = rowsstratificate.Count;
                                ALtxtTipoCampionamento_Info.Add(
                                  "INTERVALLO DI SELEZIONE: Materialità operativa / Rischio " +
                                  "Individuazione = " +
                                  ConvertNumber(dMaterialitaOperativa.ToString()) + " / " +
                                  dR.ToString() + " = " +
                                  ConvertNumber((dMaterialitaOperativa / dR).ToString()) +
                                  ";               AMPIEZZA DEL CAMPIONE: Totale Crediti " +
                                  "/ Intervallo di Selezione = " +
                                  ConvertNumber(totalesaldo.ToString()) + " / " +
                                  ConvertNumber((dMaterialitaOperativa / dR).ToString()) +
                                  " = " + numerototaleitemcasuali.ToString());
                                rand = new Random();
                                for (i = 0; rowsstratificate_scelte.Count > numerototaleitemcasuali; ++i)
                                {
                                    r = rand.Next(0, rowsstratificate_scelte.Count - 1);
                                    rowsstratificate_scelte.RemoveAt(r);
                                }
                            }
                            break;
                        case "Ragionato":
                            rowsstratificate_scelte.Clear();
                            ALtxtTipoCampionamento_Info.Add(
                              "Gli item che formeranno il campione devono essere " +
                              "selezionati mediante la checkbox");
                            break;
                        case "MUS":
                            rowsstratificate_scelte.Clear();
                            if (J == 0)
                            {
                                if (dR == 0)
                                    ALtxtTipoCampionamento_Info.Add(
                                      "Rischio di individuazione non selezionato");
                                else
                                {
                                    ALtxtTipoCampionamento_Info.Add(
                                      "Materialità operativa non inserita");
                                }
                            }
                            else
                            {
                                numerototaleitemcasuali = Math.Abs(
                                  Convert.ToInt32(Math.Ceiling(totalesaldo / J)));
                                randMUS = new Random();
                                startingRmus = randMUS.Next(0, Convert.ToInt32(Math.Ceiling(J)));
                                rMUS = startingRmus;
                                counterinterno = 0;
                                countertotale = 0;
                                while (countertotale < 5000
                                  && rowsstratificate_scelte.Count < numerototaleitemcasuali)
                                {
                                    countertotale++;
                                    herevalue = 0;
                                    double.TryParse(
                                      RawData.Tables[0].Rows
                                        [rowsstratificate[counterinterno]]
                                        [Convert.ToInt32(indexcolumnsaldo)].ToString(),
                                      out herevalue);
                                    rMUS -= Convert.ToInt32(Math.Ceiling(Math.Abs(herevalue)));
                                    if (rMUS < 0)
                                    {
                                        rMUS = Convert.ToInt32(Math.Ceiling(J)) - rMUS;
                                        if (!rowsstratificate_scelte.Contains(
                                          rowsstratificate[counterinterno]))
                                            rowsstratificate_scelte.Add(
                                              rowsstratificate[counterinterno]);
                                    }
                                    if (rowsIntermediatevalue.Count >= counterinterno + 1
                                      && rowsIntermediatevalue.Contains(counterinterno))
                                        rowsIntermediatevalue[counterinterno] = rMUS;
                                    else rowsIntermediatevalue.Add(rMUS);
                                    counterinterno++;
                                    if (counterinterno == rowsstratificate.Count)
                                        counterinterno = 0;
                                }
                                if (numerototaleitemcasuali > rowsstratificate.Count)
                                {
                                    for (i = 0; i < rowsstratificate.Count; i++)
                                    {
                                        if (!rowsstratificate_scelte.Contains(rowsstratificate[i]))
                                            rowsstratificate_scelte.Add(rowsstratificate[i]);
                                    }
                                }
                                ALtxtTipoCampionamento_Info.Add(
                                  "INTERVALLO DI SELEZIONE: Materialità operativa / " +
                                  "Rischio Indiv. = " +
                                  ConvertNumber(dMaterialitaOperativa.ToString()) + " / " +
                                  dR.ToString() + " = " + ConvertNumber(J.ToString()) +
                                  ";  AMPIEZZA DEL CAMPIONE: Tot Crediti / Interv. di Sel. = " +
                                  ConvertNumber(totalesaldo.ToString()) + " / " +
                                  ConvertNumber((dMaterialitaOperativa / dR).ToString()) +
                                  " = " + numerototaleitemcasuali.ToString() +
                                  ((rowsstratificate_scelte.Count.ToString()
                                    == numerototaleitemcasuali.ToString()) ?
                                      "" : " (trovati: " +
                                        rowsstratificate_scelte.Count.ToString() + ")") +
                                        "; NUMERO CASUALE: " +
                                        ConvertNumber(startingRmus.ToString()));
                            }
                            break;
                    }
                }

                ALrowsstratificate_scelte.Add(rowsstratificate_scelte);
                ALrowsstratificate.Add(rowsstratificate);
                ALrowsIntermediatevalue.Add(rowsIntermediatevalue);

                TabItem ti = new TabItem();
                ti.MinWidth = 150.0;
                ti.Background = Brushes.LightGoldenrodYellow;
                if (attributo == "")
                {
                    ti.Header = (intervallo == "") ?
                      "Nessuna Stratificazione" : intervallo;
                }
                else
                {
                    ti.Header = (intervallo == "") ?
                      attributo : attributo + " - " + intervallo;
                }
                ti.Tag = attributo + "|" + intervallo;
                tabCampionamento.Items.Add(ti);
            }

            for (i = 0; i < ALtxtTipoCampionamento_Info.Count; i++)
            {
                str = string.Format("txtTipoCampionamento_Info_{0}", i);
                if (node.Attributes[str] == null)
                {
                    XmlAttribute attr = node.OwnerDocument.CreateAttribute(str);
                    node.Attributes.Append(attr);
                }
                str2 = (string)(ALtxtTipoCampionamento_Info[i]);
                node.Attributes[str].Value = str2;
                SetRigaAttributo(str, str2);
            }

            for (i = 0; i < CompleteListStratification.Count; i++)
            {
                str = string.Format("CompleteListStratification_{0}", i);
                if (node.Attributes[str] == null)
                {
                    XmlAttribute attr = node.OwnerDocument.CreateAttribute(str);
                    node.Attributes.Append(attr);
                }
                str2 = (((string)(CompleteListStratification[i]) == "|") ?
                  "Nessuna Stratificazione" :
                  (string)(CompleteListStratification[i]));
                node.Attributes[str].Value = str2;
                SetRigaAttributo(str, str2);
            }

            for (i = 0; i < ALtxtTotaleSaldo.Count; i++)
            {
                str = string.Format("ALtxtTotaleSaldo_{0}", i);
                if (node.Attributes[str] == null)
                {
                    XmlAttribute attr = node.OwnerDocument.CreateAttribute(str);
                    node.Attributes.Append(attr);
                }
                str2 = ((double)ALtxtTotaleSaldo[i]).ToString();
                node.Attributes[str].Value = str2;
                SetRigaAttributo(str, str2);
            }
        }

        //------------------------------------------------------------------------+
        //                       tabFinal_SelectionChanged                        |
        //------------------------------------------------------------------------+
        private void tabFinal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateFinal();
        }

        //------------------------------------------------------------------------+
        //                   tabCampionamento_SelectionChanged                    |
        //------------------------------------------------------------------------+
        private void tabCampionamento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ColumnDefinition cd;
            int counterhere, i, row;
            List<int> a, rowsIntermediatevalue, rowsstratificate, rowsstratificate_scelte;
            RowDefinition rd;
            string attributo, intervallo, str;
            TabItem ti;
            TextBlock lbl;

            if (tabCampionamento.Items.Count == 0) return;
            ti = (TabItem)tabCampionamento.SelectedItem;
            attributo = ti.Tag.ToString().Split('|')[0];
            intervallo = ti.Tag.ToString().Split('|')[1];
            rowsstratificate_scelte =
              (List<int>)(ALrowsstratificate_scelte[
                CompleteListStratification.IndexOf(ti.Tag.ToString())]);
            rowsstratificate =
              (List<int>)(ALrowsstratificate[
                CompleteListStratification.IndexOf(ti.Tag.ToString())]);
            rowsIntermediatevalue =
              (List<int>)(ALrowsIntermediatevalue[
                CompleteListStratification.IndexOf(ti.Tag.ToString())]);
            txtTipoCampionamento_Info.Text =
              (string)(ALtxtTipoCampionamento_Info[
                CompleteListStratification.IndexOf(ti.Tag.ToString())]);

            grdCampionamentoHeader.Children.Clear();
            grdCampionamentoHeader.ColumnDefinitions.Clear();
            grdCampionamentoHeader.RowDefinitions.Clear();
            grdCampionamentoHeader.Width = 1270;
            grdCampionamentoHeader.MaxWidth = 1270;
            grdCampionamentoHeader.MinWidth = 1270;
            grdCampionamento.Children.Clear();
            grdCampionamento.ColumnDefinitions.Clear();
            grdCampionamento.RowDefinitions.Clear();
            grdCampionamento.Width = 1270;
            grdCampionamento.MaxWidth = 1270;
            grdCampionamento.MinWidth = 1270;

            for (i = 0; i < RawData.Tables[0].Columns.Count - 1; i++)
            {
                cd = new ColumnDefinition();
                cd.Width = new GridLength((i == 1) ? 6 : 2, GridUnitType.Star);
                grdCampionamentoHeader.ColumnDefinitions.Add(cd);
                cd = new ColumnDefinition();
                cd.Width = new GridLength((i == 1) ? 6 : 2, GridUnitType.Star);
                grdCampionamento.ColumnDefinitions.Add(cd);
            }

            //if (node.Attributes["Scelta"] != null)
            if (EsisteAttributo("Scelta"))
            {
                //switch (node.Attributes["Scelta"].Value)
                switch (GetRigaAttributo("Scelta"))
                {
                    case "Casuale":
                        break;
                    case "Ragionato":
                    case "MUS":
                        cd = new ColumnDefinition();
                        cd.Width = new GridLength(1, GridUnitType.Star);
                        grdCampionamentoHeader.ColumnDefinitions.Add(cd);
                        cd = new ColumnDefinition();
                        cd.Width = new GridLength(1, GridUnitType.Star);
                        grdCampionamento.ColumnDefinitions.Add(cd);
                        break;
                }
            }

            //--------------------------------------------------------------- HEADERS
            row = 0;
            System.Windows.Controls.Border brd;
            for (i = 0; i < RawData.Tables[0].Columns.Count - 1; i++)
            {
                rd = new RowDefinition();
                grdCampionamentoHeader.RowDefinitions.Add(rd);

                brd = new System.Windows.Controls.Border();
                brd.BorderThickness = new Thickness(1.0);
                brd.BorderBrush = Brushes.LightGray;
                brd.Background = Brushes.LightGray;
                brd.Padding = new Thickness(2.0);

                lbl = new TextBlock();
                lbl.Text = RawData.Tables[0].Columns[i].ColumnName;
                lbl.TextAlignment = TextAlignment.Center;
                lbl.TextWrapping = TextWrapping.Wrap;
                lbl.FontWeight = FontWeights.Bold;

                brd.Child = lbl;

                grdCampionamentoHeader.Children.Add(brd);
                Grid.SetRow(brd, row);
                Grid.SetColumn(brd, i);
            }

            //if (node.Attributes["Scelta"] != null)
            if (EsisteAttributo("Scelta"))
            {
                //switch (node.Attributes["Scelta"].Value)
                switch (GetRigaAttributo("Scelta"))
                {
                    case "Casuale":
                        break;
                    case "Ragionato":
                        brd = new System.Windows.Controls.Border();
                        brd.BorderThickness = new Thickness(1.0);
                        brd.BorderBrush = Brushes.LightGray;
                        brd.Background = Brushes.LightGray;
                        brd.Padding = new Thickness(2.0);

                        lbl = new TextBlock();
                        lbl.Text = "Scelta";
                        lbl.TextAlignment = TextAlignment.Center;
                        lbl.TextWrapping = TextWrapping.Wrap;

                        brd.Child = lbl;

                        grdCampionamentoHeader.Children.Add(brd);
                        Grid.SetRow(brd, row);
                        Grid.SetColumn(brd, RawData.Tables[0].Columns.Count);
                        break;
                    case "MUS":
                        brd = new System.Windows.Controls.Border();
                        brd.BorderThickness = new Thickness(1.0);
                        brd.BorderBrush = Brushes.LightGray;
                        brd.Background = Brushes.LightGray;
                        brd.Padding = new Thickness(2.0);

                        lbl = new TextBlock();
                        lbl.Text = "Calcolo";
                        lbl.TextAlignment = TextAlignment.Center;
                        lbl.TextWrapping = TextWrapping.Wrap;

                        brd.Child = lbl;

                        grdCampionamentoHeader.Children.Add(brd);
                        Grid.SetRow(brd, row);
                        Grid.SetColumn(brd, RawData.Tables[0].Columns.Count);
                        break;
                }
            }

            counterhere = -1;
            foreach (int j in rowsstratificate)
            {
                counterhere++;
                for (i = 0; i < RawData.Tables[0].Columns.Count - 1; i++)
                {
                    rd = new RowDefinition();
                    grdCampionamento.RowDefinitions.Add(rd);

                    brd = new System.Windows.Controls.Border();
                    brd.BorderThickness = new Thickness(1.0);
                    if (rowsstratificate_scelte.Contains(j))
                        brd.Background = Brushes.Yellow;
                    brd.BorderBrush = Brushes.LightGray;
                    brd.Padding = new Thickness(2.0);

                    lbl = new TextBlock();

                    switch (i)
                    {
                        case 1:
                            lbl.Text = RawData.Tables[0].Rows[j][i].ToString();
                            lbl.TextAlignment = TextAlignment.Left;
                            break;
                        case 2:
                        case 3:
                            lbl.Text = ConvertNumber(RawData.Tables[0].Rows[j][i].ToString());
                            lbl.TextAlignment = TextAlignment.Right;
                            break;
                        case 0:
                        default:
                            lbl.Text = RawData.Tables[0].Rows[j][i].ToString();
                            lbl.TextAlignment = TextAlignment.Center;
                            break;
                    }

                    lbl.TextWrapping = TextWrapping.Wrap;

                    brd.Child = lbl;

                    grdCampionamento.Children.Add(brd);
                    Grid.SetRow(brd, row);
                    Grid.SetColumn(brd, i);
                }

                //if (node.Attributes["Scelta"] != null)
                if (EsisteAttributo("Scelta"))
                {
                    //switch (node.Attributes["Scelta"].Value)
                    switch (GetRigaAttributo("Scelta"))
                    {
                        case "Casuale":
                            break;
                        case "Ragionato":
                            brd = new System.Windows.Controls.Border();
                            brd.BorderThickness = new Thickness(1.0);
                            if (rowsstratificate_scelte.Contains(j))
                            {
                                brd.Background = Brushes.Yellow;
                            }
                            brd.BorderBrush = Brushes.LightGray;
                            brd.Padding = new Thickness(2.0);

                            System.Windows.Controls.CheckBox chk =
                              new System.Windows.Controls.CheckBox();
                            chk.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                            str = string.Format("rowsstratificate_scelte_{0}",
                              CompleteListStratification.IndexOf(ti.Tag.ToString()));
                            /*-----------------------------------------------------------------------------
                                          if (node.Attributes["rowsstratificate_scelte_" +
                                            CompleteListStratification.IndexOf(ti.Tag.ToString())] != null)
                            -----------------------------------------------------------------------------*/
                            if (EsisteAttributo(str)
                              && !string.IsNullOrEmpty(GetRigaAttributo(str)))
                            {
                                a = new List<int>();
                                foreach (string item in
                                  /*-----------------------------------------------------------------------------
                                                    node.Attributes["rowsstratificate_scelte_" +
                                                      CompleteListStratification.IndexOf(ti.Tag.ToString())].Value
                                                        .Split('|'))
                                  -----------------------------------------------------------------------------*/
                                  GetRigaAttributo(str).Split('|'))
                                    a.Add(Convert.ToInt32(item));

                                if (ALrowsstratificate_scelte.Count <
                                  CompleteListStratification.IndexOf(ti.Tag.ToString()) + 1)
                                    ALrowsstratificate_scelte.Add(a);
                                else
                                    ALrowsstratificate_scelte[
                                      CompleteListStratification.IndexOf(ti.Tag.ToString())] = a;
                            }
                            /*-----------------------------------------------------------------------------
                                          if (node.Attributes["rowsstratificate_scelte_" +
                                            CompleteListStratification.IndexOf(ti.Tag.ToString())] != null
                                            && node.Attributes["rowsstratificate_scelte_" +
                                              CompleteListStratification.IndexOf(ti.Tag.ToString())].Value
                                                .Split('|').Contains(j.ToString()))
                            -----------------------------------------------------------------------------*/
                            str = string.Format("rowsstratificate_scelte_{0}",
                              CompleteListStratification.IndexOf(ti.Tag.ToString()));
                            if (EsisteAttributo(str)
                              && GetRigaAttributo(str).Split('|').Contains(j.ToString()))
                                chk.IsChecked = true;
                            else chk.IsChecked = false;

                            chk.Tag = j.ToString();
                            chk.Checked += Chk_Checked;
                            chk.Unchecked += Chk_Unchecked;
                            brd.Child = chk;

                            grdCampionamento.Children.Add(brd);
                            Grid.SetRow(brd, row);
                            Grid.SetColumn(brd, RawData.Tables[0].Columns.Count);
                            break;
                        case "MUS":
                            brd = new System.Windows.Controls.Border();
                            brd.BorderThickness = new Thickness(1.0);
                            if (rowsstratificate_scelte.Contains(j))
                                brd.Background = Brushes.Yellow;
                            brd.BorderBrush = Brushes.LightGray;
                            brd.Padding = new Thickness(2.0);
                            lbl = new TextBlock();
                            try
                            {
                                lbl.Text = ConvertNumber(
                                  rowsIntermediatevalue[counterhere].ToString());
                                lbl.TextAlignment = TextAlignment.Right;
                            }
                            catch (Exception) { lbl.Text = ""; }

                            lbl.TextWrapping = TextWrapping.Wrap;
                            brd.Child = lbl;
                            grdCampionamento.Children.Add(brd);
                            Grid.SetRow(brd, row);
                            Grid.SetColumn(brd, RawData.Tables[0].Columns.Count);
                            break;
                    }
                }
                row++;
            }
          ((WindowWorkArea)(this.Owner))._x.Save();
        }

        #region Cestino

        //------------------------------------------------------------------------+
        //                   VisualizzaListaDaAssociare_Cestino                   |
        //------------------------------------------------------------------------+
        private void VisualizzaListaDaAssociare_Cestino()
        {
            bool alternate;
            int i;
            string str;

            ScrollViewer sw_ElencoBV = (ScrollViewer)this.FindName("sw_ElencoBVCestino");
            sw_ElencoBV.BorderBrush = Brushes.Black;
            sw_ElencoBV.BorderThickness = new Thickness(1);
            StackPanel stpElencoBV = (StackPanel)this.FindName("stpElencoBVCestino");
            if (stpElencoBV == null)
            {
                stpElencoBV = new StackPanel();
                stpElencoBV.Name = "stpElencoBVCestino";
                try { this.UnregisterName(stpElencoBV.Name); }
                catch (Exception) { }
                this.RegisterName(stpElencoBV.Name, stpElencoBV);
            }
            else stpElencoBV.Children.Clear();
            stpElencoBV.Orientation = System.Windows.Controls.Orientation.Vertical;
            alternate = true;
            for (i = 0; i < RawData.Tables[0].Rows.Count; i++)
            {
                /*-----------------------------------------------------------------------------
                        if (node.Attributes["RigheCancellate"] != null
                          && node.Attributes["RigheCancellate"].Value
                            .Split('|').Contains(
                              RawData.Tables[0].Rows[i][0].ToString() + "-" +
                              RawData.Tables[0].Rows[i][1].ToString()))
                -----------------------------------------------------------------------------*/
                str = "RigheCancellate";
                if (EsisteAttributo(str)
                  && GetRigaAttributo(str)
                    .Split('|').Contains(
                      RawData.Tables[0].Rows[i][0].ToString() + "-" +
                      RawData.Tables[0].Rows[i][1].ToString()))
                    continue;

                StackPanel stp = new StackPanel();
                stp.Orientation = System.Windows.Controls.Orientation.Horizontal;

                if (alternate)
                {
                    stp.Background = Brushes.LightGray;
                    alternate = false;
                }
                else alternate = true;

                TextBlock txt = new TextBlock();
                txt.Width = 100;
                txt.Margin = new Thickness(5, 0, 0, 0);
                txt.ToolTip = RawData.Tables[0].Rows[i][0].ToString();
                txt.Text = RawData.Tables[0].Rows[i][0].ToString();
                stp.Children.Add(txt);

                txt = new TextBlock();
                txt.Width = 200;
                txt.Margin = new Thickness(5, 0, 0, 0);
                txt.ToolTip = RawData.Tables[0].Rows[i][1].ToString();
                txt.Text = RawData.Tables[0].Rows[i][1].ToString();
                stp.Children.Add(txt);

                txt = new TextBlock();
                txt.Width = 150;
                txt.Margin = new Thickness(5, 0, 0, 0);
                txt.ToolTip = RawData.Tables[0].Rows[i][2].ToString();
                txt.Text = RawData.Tables[0].Rows[i][2].ToString();
                txt.TextAlignment = TextAlignment.Right;
                stp.Children.Add(txt);

                System.Windows.Controls.CheckBox chk =
                  new System.Windows.Controls.CheckBox();
                chk.Name = "chkCestinoDa_" + i.ToString();
                chk.Tag = RawData.Tables[0].Rows[i][0].ToString() + "-" +
                  RawData.Tables[0].Rows[i][1].ToString();
                try { this.UnregisterName(chk.Name); }
                catch (Exception) { }
                this.RegisterName(chk.Name, chk);
                stp.Children.Add(chk);
                stpElencoBV.Children.Add(stp);
            }
            sw_ElencoBV.Content = stpElencoBV;
            sw_ElencoBV.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            sw_ElencoBV.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        //------------------------------------------------------------------------+
        //                    VisualizzaListaAssociate_Cestino                    |
        //------------------------------------------------------------------------+
        private void VisualizzaListaAssociate_Cestino()
        {
            bool alternate;
            int indexhere;
            string str;

            ScrollViewer sw_ElencoAssociazioni = (ScrollViewer)this.FindName("sw_ElencoAssociazioniCestino");
            sw_ElencoAssociazioni.BorderBrush = Brushes.Black;
            sw_ElencoAssociazioni.BorderThickness = new Thickness(1);
            StackPanel stp = new StackPanel();
            alternate = true;
            str = "RigheCancellate";
            if (node.Attributes[str] == null)
            {
                XmlAttribute attr = node.OwnerDocument.CreateAttribute(str);
                node.Attributes.Append(attr);
            }
            indexhere = 0;
            foreach (string itemhere in
              //node.Attributes["RigheCancellate"].Value.Split('|'))
              GetRigaAttributo(str).Split('|'))
            {
                if (itemhere == "") continue;
                indexhere++;
                StackPanel stpriga = new StackPanel();
                stpriga.Orientation = System.Windows.Controls.Orientation.Horizontal;

                if (alternate)
                {
                    stpriga.Background = Brushes.LightGray;
                    alternate = false;
                }
                else alternate = true;

                System.Windows.Controls.CheckBox chk =
                  new System.Windows.Controls.CheckBox();
                chk.Name = "chkCestinoA_" + indexhere.ToString();
                chk.Tag = itemhere;
                try { this.UnregisterName(chk.Name); }
                catch (Exception) { }
                this.RegisterName(chk.Name, chk);

                stpriga.Children.Add(chk);

                TextBlock txt = new TextBlock();
                txt.Margin = new Thickness(5, 0, 0, 0);
                txt.Width = 200;
                txt.ToolTip = itemhere;
                txt.Text = itemhere;
                stpriga.Children.Add(txt);
                stp.Children.Add(stpriga);
            }
            sw_ElencoAssociazioni.Content = stp;
        }

        //------------------------------------------------------------------------+
        //                     BtnpassaggioVersoCestino_Click                     |
        //------------------------------------------------------------------------+
        private void BtnpassaggioVersoCestino_Click(object sender, RoutedEventArgs e)
        {
            string str;

            StackPanel stpElencoBV = (StackPanel)this.FindName("stpElencoBVCestino");
            if (node.Attributes["RigheCancellate"] == null)
            {
                XmlAttribute attr = node.OwnerDocument.CreateAttribute("RigheCancellate");
                node.Attributes.Append(attr);
                SetRigaAttributo("RigheCancellate", "");
            }
            foreach (StackPanel item in stpElencoBV.Children)
            {
                /*-----------------------------------------------------------------------------
                        if (((System.Windows.Controls.CheckBox)(item.Children[3])).IsChecked == true
                          && !node.Attributes["RigheCancellate"].Value.Split('|')
                            .Contains(((System.Windows.Controls.CheckBox)(item.Children[3])).Tag.ToString()))
                -----------------------------------------------------------------------------*/
                str = GetRigaAttributo("RigheCancellate");
                if (((System.Windows.Controls.CheckBox)item.Children[3]).IsChecked == true
                  && !str.Split('|').Contains(
                    ((System.Windows.Controls.CheckBox)item.Children[3]).Tag.ToString()))
                {
                    node.Attributes["RigheCancellate"].Value +=
                      ((node.Attributes["RigheCancellate"].Value == "") ? "" : "|") +
                        ((System.Windows.Controls.CheckBox)item.Children[3]).Tag.ToString();
                    str += (string.IsNullOrEmpty(str) ? "" : "|") +
                        ((System.Windows.Controls.CheckBox)item.Children[3]).Tag.ToString();
                    SetRigaAttributo("RigheCancellate", str);
                }
            }
            VisualizzaListaDaAssociare_Cestino();
            VisualizzaListaAssociate_Cestino();
        }

        //------------------------------------------------------------------------+
        //                      BtnpassaggioDaCestino_Click                       |
        //------------------------------------------------------------------------+
        private void BtnpassaggioDaCestino_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewer sw_ElencoAssociazioni;
            StackPanel stpElencoBV;
            ArrayList tobeshown;
            string str;

            sw_ElencoAssociazioni =
              (ScrollViewer)this.FindName("sw_ElencoAssociazioniCestino");
            sw_ElencoAssociazioni.BorderBrush = Brushes.Black;
            sw_ElencoAssociazioni.BorderThickness = new Thickness(1);
            stpElencoBV = (StackPanel)sw_ElencoAssociazioni.Content;
            if (node.Attributes["RigheCancellate"] == null)
            {
                XmlAttribute attr =
                  node.OwnerDocument.CreateAttribute("RigheCancellate");
                node.Attributes.Append(attr);
                SetRigaAttributo("RigheCancellate", "");
            }
            tobeshown = new ArrayList();
            foreach (StackPanel item in stpElencoBV.Children)
            {
                if (((System.Windows.Controls.CheckBox)item.Children[0]).IsChecked == true)
                {
                    node.Attributes["RigheCancellate"].Value =
                      node.Attributes["RigheCancellate"].Value.Replace(
                        ((System.Windows.Controls.CheckBox)item.Children[0])
                          .Tag.ToString() + "|", "");
                    node.Attributes["RigheCancellate"].Value =
                      node.Attributes["RigheCancellate"].Value.Replace("|" +
                        ((System.Windows.Controls.CheckBox)item.Children[0])
                          .Tag.ToString(), "");
                    node.Attributes["RigheCancellate"].Value =
                      node.Attributes["RigheCancellate"].Value.Replace(
                        ((System.Windows.Controls.CheckBox)item.Children[0])
                          .Tag.ToString(), "");
                    str = GetRigaAttributo("RigheCancellate");
                    str = str.Replace(
                      ((System.Windows.Controls.CheckBox)item.Children[0])
                        .Tag.ToString() + "|", "");
                    str = str.Replace("|" +
                      ((System.Windows.Controls.CheckBox)item.Children[0])
                        .Tag.ToString(), "");
                    str = str.Replace(
                      ((System.Windows.Controls.CheckBox)item.Children[0])
                        .Tag.ToString(), "");
                    SetRigaAttributo("RigheCancellate", str);
                }
            }
            VisualizzaListaDaAssociare_Cestino();
            VisualizzaListaAssociate_Cestino();
        }

        #endregion

        //------------------------------------------------------------------------+
        //                       ButtonDatiOriginali_Click                        |
        //------------------------------------------------------------------------+
        private void ButtonDatiOriginali_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage wb;
            ExcelWorksheet ws;
            int col, Idx, row;
            string sRawData;

            //if (node != null && node.Attributes["RawData"] != null)
            sRawData = dataCampionamento.Rows[0]["RawData"].ToString();
            if (!string.IsNullOrEmpty(sRawData))
            {
                using (StringReader sw = new StringReader(sRawData))
                {
                    RawData = new DataSet();
                    RawData.ReadXml(sw);
                }
            }
            if (RawData == null) return;
            //------------------------------------------------------------- crea xlsx
            wb = null;
            object missing = Type.Missing;
            ws = null;
            try
            {
                ws = excelworkBook.Workbook.Worksheets.Add("Foglio 1");
                if (wb.Compatibility.IsWorksheets1Based)
                    ws.Cells[1, 1, 1, FinalData.Tables[0].Rows.Count * 2]
                      .Style.Font.Bold = true;
                else
                    ws.Cells[0, 0, 0, FinalData.Tables[0].Rows.Count * 2]
                      .Style.Font.Bold = true;
                for (Idx = 0; Idx < RawData.Tables[0].Columns.Count; Idx++)
                {
                    if (excelworkBook.Compatibility.IsWorksheets1Based)
                    {
                        ws.Cells[1, 1 + Idx].Value =
                          RawData.Tables[0].Columns[Idx].ColumnName;
                    }
                    else
                    {
                        ws.Cells[0, Idx].Value =
                          RawData.Tables[0].Columns[Idx].ColumnName;
                    }
                }
                for (row = 0; row < RawData.Tables[0].Rows.Count; row++)
                {
                    for (col = 0; col < RawData.Tables[0].Columns.Count; col++)
                    {
                        if (_tipologia != TipologieCampionamento.Magazzino
                          && (col == 2 || col == 3))
                        {
                            if (excelworkBook.Compatibility.IsWorksheets1Based)
                            {
                                ws.Cells[1 + row, 1 + col].Value =
                                  Convert.ToDouble(RawData.Tables[0].Rows[row][col].ToString());
                                ws.Cells[1 + row, 1 + col].Style.Numberformat.Format = "#,##0.00";
                            }
                            else
                            {
                                ws.Cells[row, col].Value =
                                  Convert.ToDouble(RawData.Tables[0].Rows[row][col].ToString());
                                ws.Cells[row, col].Style.Numberformat.Format = "#,##0.00";
                            }
                        }
                        else
                        {
                            if (excelworkBook.Compatibility.IsWorksheets1Based)
                                ws.Cells[1 + row, 1 + col].Value =
                                  RawData.Tables[0].Rows[row][col].ToString();
                            else
                                ws.Cells[row, col].Value =
                                  RawData.Tables[0].Rows[row][col].ToString();
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        //------------------------------------------------------------------------+
        //                     ButtonCircolarizzazione_Click                      |
        //------------------------------------------------------------------------+
        private void ButtonCircolarizzazione_Click(object sender, RoutedEventArgs e)
        {
            int i;
            string str;

            if ((((WindowWorkArea)(this.Owner)).ReadOnly))
            {
                MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
                return;
            }
            switch (_tipologia)
            {
                case TipologieCampionamento.Clienti:
                    labelTitolo.Content = "Clienti";
                    break;
                case TipologieCampionamento.Fornitori:
                    labelTitolo.Content = "Fornitori";
                    break;
                case TipologieCampionamento.Magazzino:
                    labelTitolo.Content = "Rimanenze di Magazzino";
                    break;
            }
            stpFinal.Visibility = Visibility.Collapsed;
            stpFinal_btn.Visibility = Visibility.Collapsed;
            stpCompleteCircolarizzazione.Visibility = Visibility.Visible;
            stpCompleteCircolarizzazione_btn.Visibility = Visibility.Visible;
            /*-----------------------------------------------------------------------------
                  if (node.Attributes["stpStratificazionesino"] != null
                    && node.Attributes["stpStratificazionesino"].Value == "SI")
            -----------------------------------------------------------------------------*/
            str = "stpStratificazionesino";
            if (EsisteAttributo(str)
              && GetRigaAttributo(str) == "SI")
            {
                stpStratiCircolarizzazione.Children.Clear();
                TextBlock txt = new TextBlock();
                txt.Text = "Selezionare gli Strati da circolarizzare:";
                stpStratiCircolarizzazione.Children.Add(txt);

                for (i = 0; i < CompleteListStratification.Count; i++)
                {
                    System.Windows.Controls.CheckBox chk =
                      new System.Windows.Controls.CheckBox();
                    chk.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    chk.Margin = new Thickness(10, 10, 0, 0);
                    chk.Foreground = Brushes.Blue;
                    chk.IsChecked = true;
                    chk.FontWeight = FontWeights.Bold;
                    chk.Tag = CompleteListStratification[i].ToString();
                    chk.Content = CompleteListStratification[i]
                      .ToString().Replace("|", " - ");
                    stpStratiCircolarizzazione.Children.Add(chk);
                }
                stpStratiCircolarizzazione.Visibility = Visibility.Visible;
            }
            else
                stpStratiCircolarizzazione.Visibility = Visibility.Collapsed;
        }

        //------------------------------------------------------------------------+
        //                   ButtonCircolarizzazioneBack_Click                    |
        //------------------------------------------------------------------------+
        private void ButtonCircolarizzazioneBack_Click(object sender, RoutedEventArgs e)
        {
            switch (_tipologia)
            {
                case TipologieCampionamento.Clienti:
                    labelTitolo.Content = "Clienti";
                    break;
                case TipologieCampionamento.Fornitori:
                    labelTitolo.Content = "Fornitori";
                    break;
                case TipologieCampionamento.Magazzino:
                    labelTitolo.Content = "Rimanenze di Magazzino";
                    break;
            }
            labelTitolo.Content += "  -  Campioni estratti per la rilevazione " +
              "degli errori";
            stpFinal.Visibility = Visibility.Visible;
            stpFinal_btn.Visibility = Visibility.Visible;
            stpCompleteCircolarizzazione.Visibility = Visibility.Collapsed;
            stpCompleteCircolarizzazione_btn.Visibility = Visibility.Collapsed;
        }

        //------------------------------------------------------------------------+
        //                 ButtonCircolarizzazioneComplete_Click                  |
        //------------------------------------------------------------------------+
        private void ButtonCircolarizzazioneComplete_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage excelworkBook2;
            ExcelRange objRange;
            ExcelWorksheet excelSheet2;
            FileInfo fi, file;
            Hashtable valueshere;
            int firstrow, i;
            string cap, citta, filetemplate, indirizzo, Nomefile, templateurl;
            Utilities u;

            MessageBox.Show("Il file creato deve essere SALVATO CON NOME per " +
              "allocarlo della cartella desiderata. con il solo SALVA verrà " +
              "allocato tra i file temporanei");
            valueshere = new Hashtable();
            valueshere.Add("[RAGIONESOCIALE]", "___");
            valueshere.Add("[INDIRIZZO]", "___");
            valueshere.Add("[CAP]", "___");
            valueshere.Add("[CITTA]", "___");
            valueshere.Add("[DATA]", txtData.Text);
            valueshere.Add("[SALDO]", "___");
            valueshere.Add("[REVISORE]", txtRevisore.Text);
            valueshere.Add("[REVISOREINDIRIZZO]", txtRevisoreIndirizzo.Text);
            valueshere.Add("[REVISORECAP]", txtRevisoreCAP.Text);
            valueshere.Add("[REVISORECITTA]", txtRevisoreCitta.Text);
            valueshere.Add("[EMAIL]", txtEmail.Text);
            valueshere.Add("[PEC]", txtPEC.Text);
            valueshere.Add("[FAX]", txtFax.Text);
            valueshere.Add("[LUOGO_DATA]", txtDataSpedizione.Text);

            u = new Utilities();
            Nomefile = u.sys_OpenFileDialog("", App.TipoFile.BilancioDiVerifica);
            if (string.IsNullOrEmpty(Nomefile)) return;

            excelworkBook2 = null;
            excelSheet2 = null;
            file = new FileInfo(Nomefile);
            excelworkBook2 = new ExcelPackage(file);
            if (excelworkBook2.Compatibility.IsWorksheets1Based)
                excelSheet2 = excelworkBook2.Workbook.Worksheets[1];
            else
                excelSheet2 = excelworkBook2.Workbook.Worksheets[0];

            firstrow = 2;
            lastColIncludeFormulas = excelSheet2.Dimension.End.Column;
            lastRowIncludeFormulas = excelSheet2.Dimension.End.Row;

            objRange = null;
            object missing = System.Type.Missing;
            try
            {
                object filename =
                  App.AppTempFolder + Guid.NewGuid().ToString() + ".doc";
                filetemplate = App.AppTemplateFolder;
                switch (_tipologia)
                {
                    case TipologieCampionamento.Clienti:
                        filetemplate += "\\161.rtf";
                        break;
                    case TipologieCampionamento.Fornitori:
                        filetemplate += "\\162.rtf";
                        break;
                    case TipologieCampionamento.Sconosciuto:
                    case TipologieCampionamento.Magazzino:
                    default:
                        return;
                }

                try
                {
                    templateurl = string.Empty;
                    switch (_tipologia)
                    {
                        case TipologieCampionamento.Clienti:
                            templateurl = "https://www.revisoft.it/Templates/161.rtf";
                            break;
                        case TipologieCampionamento.Fornitori:
                            templateurl = "https://www.revisoft.it/Templates/162.rtf";
                            break;
                        case TipologieCampionamento.Sconosciuto:
                        case TipologieCampionamento.Magazzino:
                        default:
                            return;
                    }
                    using (var client2 = new System.Net.WebClient())
                    {
                        var content2 = client2.DownloadData(templateurl);
                        using (var stream2 = new MemoryStream(content2))
                        {
                            System.IO.FileStream output =
                              new System.IO.FileStream(filetemplate, FileMode.Create);
                            stream2.CopyTo(output);
                            stream2.Close();
                            output.Close();
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Attenzione, template non trovato");
                    return;
                }
                fi = new FileInfo(filetemplate);
                if (!fi.Exists)
                {
                    MessageBox.Show("Attenzione, template non trovato");
                    return;
                }
                //MM
                //  fi.CopyTo(filename.ToString());
                DocumentCore.Serial = "10022773750";
                DocumentCore dc = new DocumentCore();
                //MM   bool firstdone = false;

                for (i = firstrow; i <= lastRowIncludeFormulas; i++)
                {
                    indirizzo = string.Empty; cap = string.Empty; citta = string.Empty;
                    try
                    {
                        objRange = excelSheet2.Cells[i, 3];
                        if (objRange.Merge)
                            indirizzo = Convert.ToString(
                              excelSheet.Cells[1, 1].Text).Trim().Replace("\"", "")
                                .Replace("&", "").Replace("<", "").Replace(">", "");
                        else
                            indirizzo = Convert.ToString(
                              objRange.Text).Trim().Replace("\"", "").Replace("&", "")
                                .Replace("<", "").Replace(">", "");

                        objRange = excelSheet2.Cells[i, 4];
                        if (objRange.Merge)
                            cap = Convert.ToString(
                              excelSheet2.Cells[1, 1].Text).Trim().Replace("\"", "")
                                .Replace("&", "").Replace("<", "").Replace(">", "");
                        else
                            cap = Convert.ToString(
                              objRange.Text).Trim().Replace("\"", "").Replace("&", "")
                                .Replace("<", "").Replace(">", "");

                        objRange = excelSheet2.Cells[i, 5];
                        if (objRange.Merge)
                            citta = Convert.ToString(
                              excelSheet2.Cells[1, 1].Text.Trim().Replace("\"", "")
                                .Replace("&", "").Replace("<", "").Replace(">", ""));
                        else
                            citta = Convert.ToString(
                              objRange.Text).Trim().Replace("\"", "").Replace("&", "")
                                .Replace("<", "").Replace(">", "");
                    }
                    catch (Exception) { }

                    if (indirizzo != "")
                    {
                        valueshere["[INDIRIZZO]"] = indirizzo;
                        valueshere["[CAP]"] = cap;
                        valueshere["[CITTA]"] = citta;
                        valueshere["[RAGIONESOCIALE]"] =
                          FinalData.Tables[0].Rows[i - 2][2].ToString();
                        valueshere["[SALDO]"] = ConvertNumber(
                          FinalData.Tables[0].Rows[i - 2][3].ToString());

                        //MM
                        var dataSource = new[] {
              new {
                  DATA = txtData.Text,
                  REVISORE = txtRevisore.Text,
                  REVISOREINDIRIZZO = txtRevisoreIndirizzo.Text,
                  REVISORECAP =txtRevisoreCAP.Text,
                  REVISORECITTA = txtRevisoreCitta.Text,
                  EMAIL = txtDataSpedizione.Text,
                  PEC = txtDataSpedizione.Text,
                  FAX = txtDataSpedizione.Text,
                  LUOGO_DATA = txtDataSpedizione.Text,
                  INDIRIZZO = indirizzo,
                  CAP = cap,
                  CITTA = citta,
                  SALDO = ConvertNumber(
                    FinalData.Tables[0].Rows[i - 2][3].ToString()),
                  RAGIONESOCIALE=FinalData.Tables[0].Rows[i - 2][2].ToString()
              } };

                        DocumentCore.Serial = "10022773750";
                        DocumentCore dc_template = DocumentCore.Load(filetemplate);

                        foreach (ContentRange item in dc_template.Content.Find(@"[DATA]"))
                            item.Replace(txtData.Text);
                        foreach (ContentRange item in dc_template.Content.Find(@"[REVISORE]"))
                            item.Replace(txtRevisore.Text);
                        foreach (ContentRange item in dc_template.Content.Find(@"[REVISOREINDIRIZZO]"))
                            item.Replace(txtRevisoreIndirizzo.Text);
                        foreach (ContentRange item in dc_template.Content.Find(@"[REVISORECAP]"))
                            item.Replace(txtRevisoreCAP.Text);
                        foreach (ContentRange item in dc_template.Content.Find(@"[REVISORECITTA]"))
                            item.Replace(txtRevisoreCitta.Text);
                        foreach (ContentRange item in dc_template.Content.Find(@"[EMAIL]"))
                            item.Replace(txtEmail.Text);
                        foreach (ContentRange item in dc_template.Content.Find(@"[PEC]"))
                            item.Replace(txtPEC.Text);
                        foreach (ContentRange item in dc_template.Content.Find(@"[FAX]"))
                            item.Replace(txtFax.Text);
                        foreach (ContentRange item in dc_template.Content.Find(@"[LUOGO_DATA]"))
                            item.Replace(txtDataSpedizione.Text);
                        foreach (ContentRange item in dc_template.Content.Find(@"[INDIRIZZO]"))
                            item.Replace(indirizzo);
                        foreach (ContentRange item in dc_template.Content.Find(@"[CAP]"))
                            item.Replace(cap);
                        foreach (ContentRange item in dc_template.Content.Find(@"[CITTA]"))
                            item.Replace(citta);
                        foreach (ContentRange item in dc_template.Content.Find(@"[SALDO]"))
                            item.Replace(FinalData.Tables[0].Rows[i - 2][3].ToString());
                        foreach (ContentRange item in dc_template.Content.Find(@"[RAGIONESOCIALE]"))
                            item.Replace(FinalData.Tables[0].Rows[i - 2][2].ToString());

                        // dc_template.MailMerge.Execute(dataSource);

                        //MM    selection.InsertFile(
                        //MM                        filetemplate
                        //MM                    , ref missing
                        //MM                    , ref missing
                        //MM                     , ref missing
                        //MM                    , ref missing);
                        //MM
                        //MM  object pageBreak = Microsoft.Office.Interop.Word.WdBreakType.wdSectionBreakNextPage;

                        //MM  selection.InsertBreak(ref pageBreak);
                        dc.Content.End.Insert(dc_template.Content);
                        /*MM
                        foreach (DictionaryEntry item in valueshere)
                        {


                            foreach (Microsoft.Office.Interop.Word.Range tmpRange in doc.StoryRanges)
                            {
                                tmpRange.Find.Text = item.Key.ToString();
                                tmpRange.Find.Replacement.Text = item.Value.ToString();
                                //tmpRange.Find.Replacement.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphJustify;
                                tmpRange.Find.Wrap = Microsoft.Office.Interop.Word.WdFindWrap.wdFindContinue;
                                object replaceAll = Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll;

                                tmpRange.Find.Execute(ref missing, ref missing, ref missing,
                                    ref missing, ref missing, ref missing, ref missing,
                                    ref missing, ref missing, ref missing, ref replaceAll,
                                    ref missing, ref missing, ref missing, ref missing);
                            }
                        }
                        */
                        //doc.Close(ref missing, ref missing, ref missing);
                        //word.Application.Quit(ref missing, ref missing, ref missing);
                    }
                }

                dc.Save(filename.ToString());
                System.Diagnostics.Process.Start(
                  new System.Diagnostics.ProcessStartInfo(filename.ToString())
                  { UseShellExecute = true });
            }
            catch (Exception)
            {
                //doc.Close(ref missing, ref missing, ref missing);
                //word.Application.Quit(ref missing, ref missing, ref missing);
            }
        }

        private void ButtonDatiCircolarizzazione_Click(object sender, RoutedEventArgs e)
        {
            bool found;
            ExcelPackage wb;
            ExcelWorksheet ws;
            FileInfo file;
            int col, countercolumn, i, Idx, row;
            string filename, sFinalData;

            /*-----------------------------------------------------------------------------
                  if (node != null
                    && node.Attributes["FinalData"] != null
                    && node.Attributes["FinalData"].Value != "<NewDataSet />")
            -----------------------------------------------------------------------------*/
            sFinalData = dataCampionamento.Rows[0]["FinalData"].ToString();
            if (!string.IsNullOrEmpty(sFinalData) && sFinalData != "<NewDataSet />")
            {
                /*-----------------------------------------------------------------------------
                        using (StringReader sw = new StringReader(
                          node.Attributes["FinalData"].Value))
                -----------------------------------------------------------------------------*/
                using (StringReader sw = new StringReader(sFinalData))
                {
                    FinalData = new DataSet();
                    FinalData.ReadXml(sw);
                }
            }
            if (FinalData == null) return;

            wb = new ExcelPackage();
            ws = null;
            object missing = Type.Missing;
            try
            {
                ws = wb.Workbook.Worksheets.Add("Foglio 1");
                if (wb.Compatibility.IsWorksheets1Based)
                    ws.Cells[1, 1, 1, FinalData.Tables[0].Rows.Count * 2]
                      .Style.Font.Bold = true;
                else
                    ws.Cells[0, 0, 0, FinalData.Tables[0].Rows.Count * 2]
                      .Style.Font.Bold = true;

                countercolumn = -1;
                for (Idx = 0; Idx < FinalData.Tables[0].Columns.Count; Idx++)
                {
                    if (Idx == 0 || Idx == 3 || Idx == 4 || Idx == 5) continue;
                    countercolumn++;
                    if (wb.Compatibility.IsWorksheets1Based)
                        ws.Cells[1, 1 + countercolumn].Value =
                          FinalData.Tables[0].Columns[Idx].ColumnName;
                    else
                        ws.Cells[0, countercolumn].Value =
                          FinalData.Tables[0].Columns[Idx].ColumnName;
                }

                if (wb.Compatibility.IsWorksheets1Based)
                {
                    countercolumn++;
                    ws.Cells[1, 1 + countercolumn].Value = "Indirizzo";
                    countercolumn++;
                    ws.Cells[1, 1 + countercolumn].Value = "Cap";
                    countercolumn++;
                    ws.Cells[1, 1 + countercolumn].Value = "Città";
                }
                else
                {
                    countercolumn++;
                    ws.Cells[0, countercolumn].Value = "Indirizzo";
                    countercolumn++;
                    ws.Cells[0, countercolumn].Value = "Cap";
                    countercolumn++;
                    ws.Cells[0, countercolumn].Value = "Città";
                }

                for (row = 0; row < FinalData.Tables[0].Rows.Count; row++)
                {
                    found = false;
                    if (stpStratiCircolarizzazione.Visibility == Visibility.Collapsed)
                        found = true;
                    else
                    {
                        for (i = 1; i < stpStratiCircolarizzazione.Children.Count; i++)
                        {
                            if (FinalData.Tables[0].Rows[row][0].ToString() ==
                              ((System.Windows.Controls.CheckBox)
                                stpStratiCircolarizzazione.Children[i]).Tag.ToString()
                              && ((System.Windows.Controls.CheckBox)
                                stpStratiCircolarizzazione.Children[i]).IsChecked == true)
                                found = true;
                        }
                    }
                    if (!found) continue;

                    countercolumn = -1;
                    for (col = 0; col < FinalData.Tables[0].Columns.Count; col++)
                    {
                        if (col == 0 || col == 3 || col == 4 || col == 5) continue;
                        countercolumn++;
                        if (wb.Compatibility.IsWorksheets1Based)
                            ws.Cells[2 + row, 1 + countercolumn].Value =
                              FinalData.Tables[0].Rows[row][col].ToString()
                                .Replace("(Selezionare)", "");
                        else
                            ws.Cells[1 + row, countercolumn].Value =
                              FinalData.Tables[0].Rows[row][col].ToString()
                                .Replace("(Selezionare)", "");
                    }
                }

                filename = App.AppTempFolder + Guid.NewGuid().ToString() + ".xlsx";
                file = new FileInfo(filename);
                wb.SaveAs(file);
                System.Diagnostics.Process.Start(
                  new System.Diagnostics.ProcessStartInfo(filename)
                  { UseShellExecute = true });
            }
            catch (Exception) { }
        }

        //------------------------------------------------------------------------+
        //                         ButtonDatiFinal_Click                          |
        //------------------------------------------------------------------------+
        private void ButtonDatiFinal_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage wb;
            ExcelRange rng;
            ExcelWorksheet ws;
            FileInfo file;
            int col, Idx, row;
            string filename;

            if (FinalData == null) return;

            wb = new ExcelPackage();
            ws = null;
            object missing = Type.Missing;
#pragma warning disable CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato
            rng = null;
#pragma warning restore CS0219 // La variabile è assegnata, ma il suo valore non viene mai usato

            try
            {
                ws = wb.Workbook.Worksheets.Add("Foglio 1");
                if (wb.Compatibility.IsWorksheets1Based)
                    ws.Cells[1, 1, 1, FinalData.Tables[0].Rows.Count * 2]
                      .Style.Font.Bold = true;
                else
                    ws.Cells[0, 0, 0, FinalData.Tables[0].Rows.Count * 2]
                      .Style.Font.Bold = true;

                for (Idx = 0; Idx < FinalData.Tables[0].Columns.Count - 1; Idx++)
                {
                    if (wb.Compatibility.IsWorksheets1Based)
                        ws.Cells[1, 1 + Idx].Value =
                          FinalData.Tables[0].Columns[Idx].ColumnName;
                    else
                        ws.Cells[0, Idx].Value =
                          FinalData.Tables[0].Columns[Idx].ColumnName;
                }

                for (row = 0; row < FinalData.Tables[0].Rows.Count; row++)
                {
                    for (col = 0; col < FinalData.Tables[0].Columns.Count - 1; col++)
                    {
                        if (col == 3)
                        {
                            if (wb.Compatibility.IsWorksheets1Based)
                            {
                                ws.Cells[2 + row, 1 + col].Value =
                                  Convert.ToDouble(FinalData.Tables[0].Rows[row][col].ToString());
                                ws.Cells[2 + row, 1 + col].Style.Numberformat.Format = "#,##0.00";
                            }
                            else
                            {
                                ws.Cells[1 + row, col].Value =
                                  Convert.ToDouble(FinalData.Tables[0].Rows[row][col].ToString());
                                ws.Cells[1 + row, col].Style.Numberformat.Format = "#,##0.00";
                            }
                        }
                        else
                        {
                            if (wb.Compatibility.IsWorksheets1Based)
                                ws.Cells[2 + row, 1 + col].Value =
                                  FinalData.Tables[0].Rows[row][col].ToString()
                                    .Replace("(Selezionare)", "");
                            else
                                ws.Cells[1 + row, col].Value =
                                  FinalData.Tables[0].Rows[row][col].ToString()
                                    .Replace("(Selezionare)", "");
                        }
                    }
                }

                filename = App.AppTempFolder + Guid.NewGuid().ToString() + ".xlsx";
                file = new FileInfo(filename);
                wb.SaveAs(file);
                System.Diagnostics.Process.Start(
                  new System.Diagnostics.ProcessStartInfo(filename)
                  { UseShellExecute = true });
            }
            catch (Exception) { }
        }

        //------------------------------------------------------------------------+
        //                            ButtonDati_Click                            |
        //------------------------------------------------------------------------+
        private void ButtonDati_Click(object sender, RoutedEventArgs e)
        {
            FileInfo fi;
            int a1, a2, i, row;
            string bordi, cell1CeR2, cell2CeR2, cell3CeR2, cell4CeR2, colore1, colore2;
            string filename, fineriga, inizioriga, inizioriga2, istruzione, rtf_text;
            string str, str2, test, testoadditivo, v1, v2;
            string[] splittedstring, arrIstruzioni;

            rtf_text = "";
            rtf_text += "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1040\\deflangfe1040\\deftab709";
            rtf_text += "{\\fonttbl{\\f0 Cambria}}";
            rtf_text += "{\\colortbl;\\red0\\green255\\blue255;\\red204\\green204\\blue204;\\red255\\green255\\blue255;\\red230\\green230\\blue230;}";
            rtf_text += "\\viewkind4\\uc1";
            //rtf_text += "\\fs28 \\qc " + ((WindowWorkArea)(this.Owner)).Cliente + " \\ql \\fs28 \\line \\line ";
            rtf_text += "\\fs28 \\b Campionamento " + ciclo + "\\fs24 \\b0 \\line \\line \\line ";

            /*-----------------------------------------------------------------------------
                  if (node.Attributes["Nomefile"] != null )
                  {
                    string istruzione = node.Attributes["Nomefile"].Value;
                    if (istruzione.Trim() != "")
                    {
                      rtf_text += "\\b File dati di ingresso: \\b0  ";
                      rtf_text += "\\pard\\keepn \\i " + Convert2RTF(istruzione.Replace("\\", "/")) + " \\i0\\par";
                      rtf_text += " \\line ";
                    }
                  }
            -----------------------------------------------------------------------------*/

            if (node.Attributes["lst_Intestazione"] != null)
            {
                istruzione = node.Attributes["lst_Intestazione"].Value;
                if (istruzione.Trim() != "")
                {
                    rtf_text += "\\b Riga intestazione: \\b0  ";
                    rtf_text += "\\pard\\keepn \\i " + istruzione + " \\i0\\par";
                    rtf_text += " \\line ";
                }
            }

            for (i = 0; i < colonne.Count; i++)
            {
                if (node.Attributes["lst_" + i.ToString()] != null)
                {
                    istruzione = node.Attributes["lst_" + i.ToString()].Value;
                    if (istruzione.Trim() != "")
                    {
                        rtf_text += "\\b Colonna " + colonne[i] + ": \\b0  ";
                        rtf_text += "\\pard\\keepn \\i " + Convert2RTF(istruzione.Replace("\\", "/")) + " \\i0\\par";
                        rtf_text += " \\line ";
                    }
                }
            }

            if (node.Attributes["lst_attr"] != null)
            {
                istruzione = node.Attributes["lst_attr"].Value;
                if (istruzione.Trim() != "")
                {
                    rtf_text += "\\b Attributo scelto: \\b0  ";
                    rtf_text += "\\pard\\keepn \\i " + istruzione + " \\i0\\par";
                    rtf_text += " \\line ";
                }
            }

            if (node.Attributes["txt_attr"] != null)
            {
                istruzione = node.Attributes["txt_attr"].Value;
                if (istruzione.Trim() != "")
                {
                    rtf_text += "\\b Nota aggiuntiva all'attributo scelto: \\b0  ";
                    rtf_text += "\\pard\\keepn \\i " + Convert2RTF(istruzione.Replace("\\", "/")) + " \\i0\\par";
                    rtf_text += " \\line ";
                }
            }

            a1 = 0;

            str = "Stratificazioni_Attributo_ALL";
            //if (node.Attributes["Stratificazioni_Attributo_ALL"] != null)
            if (EsisteAttributo(str))
            {
                /*-----------------------------------------------------------------------------
                        arrIstruzioni = node.Attributes["Stratificazioni_Attributo_ALL"].Value
                          .Split('|');
                -----------------------------------------------------------------------------*/
                arrIstruzioni = GetRigaAttributo(str).Split('|');

                if (arrIstruzioni.Count() > 0)
                {
                    rtf_text += "\\b Attributi presenti: \\b0  ";
                    rtf_text += " \\line ";
                }

                foreach (string item in arrIstruzioni)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        testoadditivo = "";
                        //if (node.Attributes["Stratificazioni_Attributo"] != null)
                        str = "Stratificazioni_Attributo";
                        if (EsisteAttributo(str))
                        {
                            /*-----------------------------------------------------------------------------
                                          if (node.Attributes["Stratificazioni_Attributo"].Value
                                            .Split('|').Contains(item))
                            -----------------------------------------------------------------------------*/
                            if (GetRigaAttributo(str).Split('|').Contains(item))
                                testoadditivo = " (Selezionato per campionamento)";
                        }
                        a1++;
                        rtf_text += "\\pard\\keepn \\i " + Convert2RTF(item.Replace("\\", "/")) + testoadditivo + " \\i0\\par";
                    }
                }

                if (a1 > 0) rtf_text += " \\line ";
            }

            a2 = 0;
            //if (node.Attributes["Stratificazioni_Intervalli_ALL"] != null)
            str = "Stratificazioni_Intervalli_ALL";
            if (EsisteAttributo(str))
            {
                /*-----------------------------------------------------------------------------
                        arrIstruzioni = node.Attributes["Stratificazioni_Intervalli_ALL"].Value
                          .Split('|');
                -----------------------------------------------------------------------------*/
                arrIstruzioni = GetRigaAttributo(str).Split('|');

                if (arrIstruzioni.Count() > 0)
                {
                    rtf_text += "\\b Intervalli monetari: \\b0  ";
                    rtf_text += " \\line ";
                }

                foreach (string item in arrIstruzioni)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        testoadditivo = string.Empty;
                        //if (node.Attributes["Stratificazioni_Intervalli"] != null)
                        str = "Stratificazioni_Intervalli";
                        if (EsisteAttributo(str))
                        {
                            /*-----------------------------------------------------------------------------
                                          if (node.Attributes["Stratificazioni_Intervalli"].Value
                                            .Split('|').Contains(item))
                            -----------------------------------------------------------------------------*/
                            if (GetRigaAttributo(str).Split('|').Contains(item))
                                testoadditivo = " (Selezionato per campionamento)";
                        }
                        a2++;
                        rtf_text += "\\pard\\keepn \\i " + Convert2RTF(item.Replace("\\", "/")) + testoadditivo + " \\i0\\par";
                    }
                }
                if (a2 > 0) rtf_text += " \\line ";
            }

            //if (node.Attributes["Motivazioni"] != null)
            str = "Motivazioni";
            if (EsisteAttributo(str))
            {
                rtf_text += "\\b Motivazioni scelta di campionamento: \\b0  ";
                //test = node.Attributes["Motivazioni"].Value;
                test = GetRigaAttributo(str);
                if (test.Split('\n').Length > 12)
                {
                    test = test.Replace(test.Split('\n')[0] + "\n", "");
                    test = test.Substring(0, test.Length - 1);
                    test = test.Replace("{\\f0 \\li0\\ri0\\sa0\\sb0\\fi0\\ql\\par}", "");
                    test = test.Replace("\\fs21 ", "\\fs21\\f0 ");
                    test = test.Replace("\\lang", "\\f0\\lang");
                    test = test.Replace("\\f3", "");
                }
                else
                    test = test.Replace("{\\f0 \\li0\\ri0\\sa0\\sb0\\fi0\\ql\\par}", "")
                      .Replace("{\\f0 {\\ltrch }\\li0\\ri0\\sa0\\sb0\\fi0\\ql\\par}", "")
                      .Replace("{\\f0 {\\ltrch }\\li0\\ri0\\sa0\\sb0\\fi0\\ql\\par}", "")
                      .Replace("{\\f0 \\li0\\ri0\\sa0\\sb0\\fi0\\ql\\par}", "")
                      .Replace("{\\f0\\fcharset0 Segoe UI;}", "")
                      .Replace("\\f1", "\\f0").Replace("\\f2", "\\f0")
                      .Replace("\\f3", "\\f0").Replace("\\f4", "\\f0")
                      .Replace("{{\\pntext", "{\\f0{\\pntext")
                      .Replace("\\f1", "\\f0").Replace("\\f2", "\\f0")
                      .Replace("{\\f0\\fcharset0 Times New Roman;}{\\f0\\fcharset0 Tahoma;}",
                      "{\\f0 Arial;\\f1 Wingdings 2;\\f2 Wingdings;}")
                      .Replace("\\f0 Wingdings 2", "\\f1 Wingdings 2")
                      .Replace("\\f0 Wingdings", "\\f2 Wingdings");

                test = test.Replace("\\ql", "\\qj");
                while (test.Split('{').Length < test.Split('}').Length)
                    test = test.Remove(test.LastIndexOf("}"), 1);

                rtf_text += "\\pard\\keepn\\f0\\qj\\li1440\\ri1440 " + test + "\\line \\par\n";
                rtf_text += " \\line ";
            }

            //if (node.Attributes["Scelta"] != null)
            str = "Scelta";
            if (EsisteAttributo(str))
            {
                //istruzione = node.Attributes["Scelta"].Value;
                istruzione = GetRigaAttributo(str);
                if (istruzione.Trim() != "")
                {
                    rtf_text += "\\b Tipologia di campionamento scelto: \\b0  ";
                    rtf_text += "\\pard\\keepn \\i " + Convert2RTF(istruzione.Replace("\\", "/")) + " \\i0\\par";
                    rtf_text += " \\line ";
                }
            }

            //if (node.Attributes["Final_Choice"] != null)
            str = "Final_Choice";
            if (EsisteAttributo(str))
            {
                //istruzione = node.Attributes["Final_Choice"].Value;
                istruzione = GetRigaAttributo(str);

                if (istruzione.Trim() != "")
                {
                    rtf_text += "\\b Conferma campione: \\b0  ";
                    if (node.Attributes["Final_Choice"].Value == "Final_Circolarizzazione")
                        rtf_text += "\\pard\\keepn \\i Per Circolarizzazione \\i0\\par";
                    else
                        rtf_text += "\\pard\\keepn \\i Per procedure di validità (controlli di sostanza) \\i0\\par";
                    rtf_text += " \\line ";
                }
            }

            inizioriga = "\\trowd\\trpaddl50\\trpaddt15\\trpaddr50\\trpaddb15\\trpaddfl3\\trpaddft3\\trpaddfr3\\trpaddfb3 ";
            fineriga = "\\row ";
            colore2 = "\\clcbpat2";
            colore1 = "\\clcbpat3";
            inizioriga2 = "\\pard\\intbl\\tx2291";
            bordi = "\\clbrdrl\\brdrw10\\brdrs\\clbrdrt\\brdrw10\\brdrs\\clbrdrr\\brdrw10\\brdrs\\clbrdrb\\brdrw10\\brdrs"; //\\clpadt100
            cell1CeR2 = "\\clvertalc\\cellx2700";
            cell2CeR2 = "\\clvertalc\\cellx5300";
            cell3CeR2 = "\\clvertalc\\cellx6300";
            cell4CeR2 = "\\clvertalc\\cellx7900";

            for (i = 0; i < Math.Max(a1, 1) * Math.Max(a2, 1); i++)
            {
                /*-----------------------------------------------------------------------------
                        if (node.Attributes["txtTipoCampionamento_Info_" + i.ToString()] != null
                          && node.Attributes["CompleteListStratification_" + i.ToString()] != null)
                -----------------------------------------------------------------------------*/
                str = string.Format("txtTipoCampionamento_Info_{0}", i);
                str2 = string.Format("CompleteListStratification_{0}", i);
                if (EsisteAttributo(str) && EsisteAttributo(str2))
                {
                    v1 = GetRigaAttributo(str); v2 = GetRigaAttributo(str2);
                    rtf_text += " \\line ";

                    /*-----------------------------------------------------------------------------
                              if (node.Attributes["CompleteListStratification_" + i.ToString()].Value
                                .Replace("|", " ") == "Nessuna Stratificazione"
                                || node.Attributes["CompleteListStratification_" + i.ToString()].Value
                                  .Replace("|", " ") == "")
                              {
                              }
                              else
                    -----------------------------------------------------------------------------*/
                    if (str2.Replace("|", " ") != "Nessuna Stratificazione"
                      && string.IsNullOrEmpty(str2.Replace("|", " ")))
                    {
                        rtf_text += "\\pard\\keepn  \\b Stratificazione " +
                          /*-----------------------------------------------------------------------------
                                        node.Attributes["CompleteListStratification_" + i.ToString()].Value
                                          .Replace("|", " ") + ": \\b0 \\par";
                          -----------------------------------------------------------------------------*/
                          str2.Replace("|", " ") + ": \\b0 \\par";
                    }

                    splittedstring =
                      /*-----------------------------------------------------------------------------
                                  node.Attributes["txtTipoCampionamento_Info_" + i.ToString()].Value
                                    .Split(';');
                      -----------------------------------------------------------------------------*/
                      str.Split(';');

                    foreach (string item in splittedstring)
                    {
                        if (item.Trim() !=
                          "Gli item che formeranno il campione devono essere " +
                            "selezionati mediante la checkbox")
                            rtf_text += "\\pard\\keepn \\i " + item.Trim() + " \\i0 \\line \\line \\par";
                    }

                    rtf_text += inizioriga + "\n" + colore2 + bordi + cell1CeR2 + colore2 + bordi + cell2CeR2;

                    //if (node.Attributes["Final_Choice"] != null)
                    if (EsisteAttributo("Final_Choice"))
                    {
                        //if (node.Attributes["Final_Choice"].Value == "Final_Circolarizzazione")
                        if (GetRigaAttributo("Final_Choice") == "Final_Circolarizzazione")
                            rtf_text += colore2 + bordi + cell3CeR2;
                    }

                    rtf_text += colore2 + bordi + cell4CeR2 + inizioriga2;
                    rtf_text += " \\qc " + "Codice - Descrizione" + "\\cell";

                    switch (_tipologia)
                    {
                        case TipologieCampionamento.Clienti:
                            rtf_text += " \\qc " + "Saldo" + "\\cell";
                            break;
                        case TipologieCampionamento.Fornitori:
                            rtf_text += " \\qc " + "Saldo" + "\\cell";
                            break;
                        case TipologieCampionamento.Magazzino:
                            rtf_text += " \\qc " + "Qta Giacente / Valore" + "\\cell";
                            break;
                    }

                    //if (node.Attributes["Final_Choice"] != null)
                    if (EsisteAttributo("Final_Choice"))
                    {
                        //if (node.Attributes["Final_Choice"].Value == "Final_Circolarizzazione")
                        if (GetRigaAttributo("Final_Choice") == "Final_Circolarizzazione")
                            rtf_text += " \\qc " + "Esito" + "\\cell";
                    }

                    rtf_text += " \\qc " + "Errore Rilevato" + "\\cell";
                    rtf_text += fineriga;

                    for (row = 0; row < FinalData.Tables[0].Rows.Count; row++)
                    {
                        /*-----------------------------------------------------------------------------
                                    if (node.Attributes["CompleteListStratification_" + i.ToString()].Value
                                      .Replace("|", " ") != "Nessuna Stratificazione"
                                      && node.Attributes["CompleteListStratification_" + i.ToString()].Value
                                        .Replace("|", " ") != "")
                        -----------------------------------------------------------------------------*/
                        if (str2.Replace("|", " ") != "Nessuna Stratificazione"
                          && !string.IsNullOrEmpty(str2.Replace("|", " ")))
                        {
                            if (FinalData.Tables[0].Rows[row][0].ToString() !=
                              //node.Attributes["CompleteListStratification_" + i.ToString()].Value)
                              str2)
                                continue;
                        }

                        rtf_text += inizioriga + "\n" + colore1 + bordi + cell1CeR2 + colore1 + bordi + cell2CeR2;

                        //if (node.Attributes["Final_Choice"] != null)
                        if (EsisteAttributo("Final_Choice"))
                        {
                            //if (node.Attributes["Final_Choice"].Value == "Final_Circolarizzazione")
                            if (GetRigaAttributo("Final_Choice") == "Final_Circolarizzazione")
                                rtf_text += colore1 + bordi + cell3CeR2;
                        }

                        rtf_text += colore1 + bordi + cell4CeR2 + inizioriga2;
                        rtf_text += " \\ql " + FinalData.Tables[0].Rows[row][1].ToString() + " - " + FinalData.Tables[0].Rows[row][2].ToString() + " \\cell";

                        switch (_tipologia)
                        {
                            case TipologieCampionamento.Clienti:
                                rtf_text += " \\qr " + ConvertNumber(FinalData.Tables[0].Rows[row][3].ToString()) + " \\cell";
                                break;
                            case TipologieCampionamento.Fornitori:
                                rtf_text += " \\qr " + ConvertNumber(FinalData.Tables[0].Rows[row][3].ToString()) + " \\cell";
                                break;
                            case TipologieCampionamento.Magazzino:
                                rtf_text += " \\qr " + ConvertNumberNoDecimal(FinalData.Tables[0].Rows[row][3].ToString()) + " / " + ConvertNumber(FinalData.Tables[0].Rows[row][4].ToString()) + " \\cell";
                                break;
                        }

                        //if (node.Attributes["Final_Choice"] != null)
                        if (EsisteAttributo("Final_Choice"))
                        {
                            //if (node.Attributes["Final_Choice"].Value == "Final_Circolarizzazione")
                            if (GetRigaAttributo("Final_Choice") == "Final_Circolarizzazione")
                                rtf_text += " \\ql " + FinalData.Tables[0].Rows[row][4].ToString().Replace("(Selezionare)", "") + " \\cell";
                        }

                        rtf_text += " \\qr " + ConvertNumber(FinalData.Tables[0].Rows[row][5].ToString()) + " \\cell";
                        rtf_text += fineriga;
                    }
                }
            }

            rtf_text += "}";
            rtf_text = Convert2RTF(rtf_text);

            filename = App.AppTempFolder + Guid.NewGuid().ToString();

            TextWriter tw = new StreamWriter(filename + ".rtf");
            tw.Write(rtf_text);
            tw.Close();

            //MM
            cDocNet wrdDoc = new cDocNet();
            wrdDoc.PageSetupPaperSize = "A4";
            wrdDoc.PageSetupOrientation = WdOrientation.wdOrientLandscape;
            wrdDoc.SaveAs(filename + ".pdf", filename + ".rtf", "WdSaveFormat.wdFormatPDF");
            //MM

            fi = new FileInfo(filename + ".rtf");
            fi.Delete();

            System.Diagnostics.Process.Start(filename + ".pdf");
        }

        //------------------------------------------------------------------------+
        //                            Convert2RTFChar                             |
        //------------------------------------------------------------------------+
        public string Convert2RTFChar(string carattere)
        {
            string newChar = "";

            switch (carattere)
            {
                //case "!":newChar = "\\'21";break;
                case "\"": newChar = "\\'22"; break;
                //case "#":newChar = "\\'23";break;
                case "$": newChar = "\\'24"; break;
                case "%": newChar = "\\'25"; break;
                case "&": newChar = "\\'26"; break;
                case "'": newChar = "\\'27"; break;
                /*
                case "(":newChar = "\\'28";break;
                case ")":newChar = "\\'29";break;
                case "*":newChar = "\\'2a";break;
                case "+":newChar = "\\'2b";break;
                case ",":newChar = "\\'2c";break;
                case "-":newChar = "\\'2d";break;
                case ".":newChar = "\\'2e";break;
                case "/":newChar = "\\'2f";break;
                case ":":newChar = "\\'3a";break;
                case ";":newChar = "\\'3b";break;
                case "<":newChar = "\\'3c";break;
                case "=":newChar = "\\'3d";break;
                case ">":newChar = "\\'3e";break;
                case "?":newChar = "\\'3f";break;
                case "@":newChar = "\\'40";break;
                case "[":newChar = "\\'5b";break;
                case "\\":newChar = "\\'5c";break;
                case "]":newChar = "\\'5d";break;
                case "^":newChar = "\\'5e";break;
                case "_":newChar = "\\'5f";break;
                case "`":newChar = "\\'60";break;
                case "{":newChar = "\\'7b";break;
                case "|":newChar = "\\'7c";break;
                case "}":newChar = "\\'7d";break;
                case "~":newChar = "\\'7e";break;
                */
                case "€": newChar = "\\'80"; break;
                // case "?":newChar = "\\'82";break;
                // case "ƒ":newChar = "\\'83";break;
                // case ""newChar = "\\'84";break;
                case "…": newChar = "\\'85"; break;
                // case "†":newChar = "\\'86";break;
                // case "‡":newChar = "\\'87";break;
                case "°": newChar = "\\'88"; break;
                // case "‰":newChar = "\\'89";break;
                // case "Š":newChar = "\\'8a";break;
                // case "‹":newChar = "\\'8b";break;
                // case "Œ":newChar = "\\'8c";break;
                // case "Ž":newChar = "\\'8e";break;
                // case "‘":newChar = "\\'91";break;
                case "’": newChar = "\\'92"; break;
                case "“": newChar = "\\'93"; break;
                case "”": newChar = "\\'94"; break;
                // case "•":newChar = "\\'95";break;
                // case "–":newChar = "\\'96";break;
                // case "—":newChar = "\\'97";break;
                // case "~":newChar = "\\'98";break;
                // case "™":newChar = "\\'99";break;
                // case "š":newChar = "\\'9a";break;
                // case "›":newChar = "\\'9b";break;
                // case "œ":newChar = "\\'9c";break;
                // case "ž":newChar = "\\'9e";break;
                // case "Ÿ":newChar = "\\'9f";break;
                // case "¡":newChar = "\\'a1";break;
                // case "¢":newChar = "\\'a2";break;
                // case "£":newChar = "\\'a3";break;
                // case "¤":newChar = "\\'a4";break;
                // case "¥":newChar = "\\'a5";break;
                // case "¦":newChar = "\\'a6";break;
                // case "§":newChar = "\\'a7";break;
                // case "¨":newChar = "\\'a8";break;
                case "©": newChar = "\\'a9"; break;
                // case "ª":newChar = "\\'aa";break;
                // case "«":newChar = "\\'ab";break;
                // case "¬":newChar = "\\'ac";break;
                // case "®":newChar = "\\'ae";break;
                // case "¯":newChar = "\\'af";break;

//                case "°": newChar = "\\'b0"; break;  //justin modify


                case "±": newChar = "\\'b1"; break;
                case "²": newChar = "\\'b2"; break;
                case "³": newChar = "\\'b3"; break;
                //case "´":newChar = "\\'b4";break;
                case "µ": newChar = "\\'b5"; break;
                // case "¶":newChar = "\\'b6";break;
                // case "•":newChar = "\\'b7";break;
                // case "¸":newChar = "\\'b8";break;
                // case "¹":newChar = "\\'b9";break;
                // case "º":newChar = "\\'ba";break;
                // case "»":newChar = "\\'bb";break;
                // case "¼":newChar = "\\'bc";break;
                // case "½":newChar = "\\'bd";break;
                // case "¾":newChar = "\\'be";break;
                // case "¿":newChar = "\\'bf";break;
                case "À": newChar = "\\'c0"; break;
                case "Á": newChar = "\\'c1"; break;
                case "Â": newChar = "\\'c2"; break;
                case "Ã": newChar = "\\'c3"; break;
                case "Ä": newChar = "\\'c4"; break;
                case "Å": newChar = "\\'c5"; break;
                case "Æ": newChar = "\\'c6"; break;
                case "Ç": newChar = "\\'c7"; break;
                case "È": newChar = "\\'c8"; break;
                case "É": newChar = "\\'c9"; break;
                case "Ê": newChar = "\\'ca"; break;
                case "Ë": newChar = "\\'cb"; break;
                case "Ì": newChar = "\\'cc"; break;
                case "Í": newChar = "\\'cd"; break;
                case "Î": newChar = "\\'ce"; break;
                case "Ï": newChar = "\\'cf"; break;
                case "Ð": newChar = "\\'d0"; break;
                case "Ñ": newChar = "\\'d1"; break;
                case "Ò": newChar = "\\'d2"; break;
                case "Ó": newChar = "\\'d3"; break;
                case "Ô": newChar = "\\'d4"; break;
                case "Õ": newChar = "\\'d5"; break;
                case "Ö": newChar = "\\'d6"; break;
                //case "×":newChar = "\\'d7";break;
                case "Ø": newChar = "\\'d8"; break;
                case "Ù": newChar = "\\'d9"; break;
                case "Ú": newChar = "\\'da"; break;
                case "Û": newChar = "\\'db"; break;
                case "Ü": newChar = "\\'dc"; break;
                case "Ý": newChar = "\\'dd"; break;
                case "Þ": newChar = "\\'de"; break;
                case "ß": newChar = "\\'df"; break;
                case "à": newChar = "\\'e0"; break;
                case "á": newChar = "\\'e1"; break;
                case "â": newChar = "\\'e2"; break;
                case "ã": newChar = "\\'e3"; break;
                case "ä": newChar = "\\'e4"; break;
                case "å": newChar = "\\'e5"; break;
                case "æ": newChar = "\\'e6"; break;
                case "ç": newChar = "\\'e7"; break;
                case "è": newChar = "\\'e8"; break;
                case "é": newChar = "\\'e9"; break;
                case "ê": newChar = "\\'ea"; break;
                case "ë": newChar = "\\'eb"; break;
                case "ì": newChar = "\\'ec"; break;
                case "í": newChar = "\\'ed"; break;
                case "î": newChar = "\\'ee"; break;
                case "ï": newChar = "\\'ef"; break;
                case "ð": newChar = "\\'f0"; break;
                case "ñ": newChar = "\\'f1"; break;
                case "ò": newChar = "\\'f2"; break;
                case "ó": newChar = "\\'f3"; break;
                case "ô": newChar = "\\'f4"; break;
                case "õ": newChar = "\\'f5"; break;
                case "ö": newChar = "\\'f6"; break;
                case "÷": newChar = "\\'f7"; break;
                case "ø": newChar = "\\'f8"; break;
                case "ù": newChar = "\\'f9"; break;
                case "ú": newChar = "\\'fa"; break;
                case "û": newChar = "\\'fb"; break;
                case "ü": newChar = "\\'fc"; break;
                case "ý": newChar = "\\'fd"; break;
                case "þ": newChar = "\\'fe"; break;
                case "ÿ": newChar = "\\'ff"; break;
            }
            return newChar;
        }

        //------------------------------------------------------------------------+
        //                           Convert2RTFString                            |
        //------------------------------------------------------------------------+
        public string Convert2RTFString(string buff, string replaceChar)
        {
            return buff.Replace(replaceChar, Convert2RTFChar(replaceChar));
        }

        //------------------------------------------------------------------------+
        //                              Convert2RTF                               |
        //------------------------------------------------------------------------+
        private string Convert2RTF(string buff)
        {
            buff = buff.Replace("\\'", "\\#");
            buff = Convert2RTFString(buff, "'"); //va messo per primo o causa problemi
            buff = buff.Replace("\\#", "\\'");
            buff = Convert2RTFString(buff, "%");
            buff = Convert2RTFString(buff, "ì");
            buff = Convert2RTFString(buff, "è");
            buff = Convert2RTFString(buff, "é");
            buff = Convert2RTFString(buff, "ò");
            buff = Convert2RTFString(buff, "à");
            buff = Convert2RTFString(buff, "ù");
            buff = Convert2RTFString(buff, "°");
            buff = Convert2RTFString(buff, "€");
            buff = Convert2RTFString(buff, "\"");
            buff = Convert2RTFString(buff, "’");
            buff = Convert2RTFString(buff, "”");
            buff = Convert2RTFString(buff, "“");
            return buff;
        }

        //------------------------------------------------------------------------+
        //                            OpenSuggerimenti                            |
        //------------------------------------------------------------------------+
        private void OpenSuggerimenti(object sender, RoutedEventArgs e)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.Refresh();
                    process.StartInfo.FileName =
                      App.AppModelliFolder + "\\SUGGERIMENTI campionamento.pdf";
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                    process.StartInfo.ErrorDialog = false;
                    process.StartInfo.Verb = "open";
                    process.Start();
                }
            }
            catch (Exception) { }
        }

        //------------------------------------------------------------------------+
        //                        ButtonDeleteFinal_Click                         |
        //------------------------------------------------------------------------+
        private void ButtonDeleteFinal_Click(object sender, RoutedEventArgs e)
        {
            if ((((WindowWorkArea)(this.Owner)).ReadOnly))
            {
                MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
                return;
            }

            stpSelezioneEstrapolazione.Visibility = Visibility.Visible;
            stpSelezioneEstrapolazione_btn.Visibility = Visibility.Visible;

            switch (_tipologia)
            {
                case TipologieCampionamento.Clienti:
                    labelTitolo.Content = "Clienti";
                    break;
                case TipologieCampionamento.Fornitori:
                    labelTitolo.Content = "Fornitori";
                    break;
                case TipologieCampionamento.Magazzino:
                    labelTitolo.Content = "Rimanenze di Magazzino";
                    break;
            }

            stpFinal.Visibility = Visibility.Collapsed;
            stpFinal_btn.Visibility = Visibility.Collapsed;
            tabCampionamento_Calculate();
            tabCampionamento.SelectedIndex = 0;
        }

        //------------------------------------------------------------------------+
        //                         ButtonDeleteRaw_Click                          |
        //------------------------------------------------------------------------+
        private void ButtonDeleteRaw_Click(object sender, RoutedEventArgs e)
        {
            int i;
            string str;

            if ((((WindowWorkArea)(this.Owner)).ReadOnly))
            {
                MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
                return;
            }

            if (MessageBox.Show("Sicuro di voler cancellare tutti i dati?",
              "Attenzione", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;

            if (node != null && node.Attributes["Nomefile"] != null)
            {
                node.Attributes.Remove(node.Attributes["Nomefile"]);
                dataCampionamento.Rows[0]["Nomefile"] = string.Empty;
            }

            if (node != null && node.Attributes["stpStratificazionesino"] != null)
            {
                node.Attributes.Remove(node.Attributes["stpStratificazionesino"]);
                RemoveRigaAttributo("stpStratificazionesino");
            }

            if (node != null && node.Attributes["Stratificazioni_Intervalli"] != null)
            {
                node.Attributes.Remove(node.Attributes["Stratificazioni_Intervalli"]);
                RemoveRigaAttributo("Stratificazioni_Intervalli");
            }

            if (node != null && node.Attributes["Stratificazioni_Attributo"] != null)
            {
                node.Attributes.Remove(node.Attributes["Stratificazioni_Attributo"]);
                RemoveRigaAttributo("Stratificazioni_Attributo");
            }

            if (node != null && node.Attributes["Intervalli"] != null)
            {
                node.Attributes.Remove(node.Attributes["Intervalli"]);
                RemoveRigaAttributo("Intervalli");
            }

            if (node != null && node.Attributes["Motivazioni"] != null)
            {
                node.Attributes.Remove(node.Attributes["Motivazioni"]);
                RemoveRigaAttributo("Motivazioni");
            }

            if (node != null && node.Attributes["Stratificazioni_Intervalli"] != null)
            {
                node.Attributes.Remove(node.Attributes["Stratificazioni_Intervalli"]);
                RemoveRigaAttributo("Stratificazioni_Intervalli");
            }

            if (node != null && node.Attributes["Stratificazioni_Intervalli_ALL"] != null)
            {
                node.Attributes.Remove(node.Attributes["Stratificazioni_Intervalli_ALL"]);
                RemoveRigaAttributo("Stratificazioni_Intervalli_ALL");
            }

            if (node != null && node.Attributes["RigheCancellate"] != null)
            {
                node.Attributes.Remove(node.Attributes["RigheCancellate"]);
                RemoveRigaAttributo("RigheCancellate");
            }

            if (node != null && node.Attributes["Scelta"] != null)
            {
                node.Attributes.Remove(node.Attributes["Scelta"]);
                RemoveRigaAttributo("Scelta");
            }

            if (node != null && node.Attributes["Final_Choice"] != null)
            {
                node.Attributes.Remove(node.Attributes["Final_Choice"]);
                RemoveRigaAttributo("Final_Choice");
            }

            for (i = 0; i < 1000; i++)
            {
                str = string.Format("rowsstratificate_scelte_{0}", i);
                if (node != null && node.Attributes[str] != null)
                {
                    node.Attributes.Remove(node.Attributes[str]);
                    RemoveRigaAttributo(str);
                }

                str = string.Format("txtTipoCampionamento_Info_{0}", i);
                if (node != null && node.Attributes[str] != null)
                {
                    node.Attributes.Remove(node.Attributes[str]);
                    RemoveRigaAttributo(str);
                }

                str = string.Format("CompleteListStratification_{0}", i);
                if (node != null && node.Attributes[str] != null)
                {
                    node.Attributes.Remove(node.Attributes[str]);
                    RemoveRigaAttributo(str);
                }

                str = string.Format("ALtxtTotaleSaldiCampione_{0}", i);
                if (node != null && node.Attributes[str] != null)
                {
                    node.Attributes.Remove(node.Attributes[str]);
                    RemoveRigaAttributo(str);
                }

                str = string.Format("ALtxtTotaleSaldo_{0}", i);
                if (node != null && node.Attributes[str] != null)
                {
                    node.Attributes.Remove(node.Attributes[str]);
                    RemoveRigaAttributo(str);
                }

                str = string.Format("lst_{0}", i);
                if (node != null && node.Attributes[str] != null)
                {
                    node.Attributes.Remove(node.Attributes[str]);
                    RemoveRigaAttributo(str);
                }

                str = string.Format("lst_attr{0}", i);
                if (node != null && node.Attributes[str] != null)
                {
                    node.Attributes.Remove(node.Attributes[str]);
                    RemoveRigaAttributo(str);
                }

                str = string.Format("txt_attr{0}", i);
                if (node != null && node.Attributes[str] != null)
                {
                    node.Attributes.Remove(node.Attributes[str]);
                    RemoveRigaAttributo(str);
                }
            }

            if (node != null)
            {
                /*-----------------------------------------------------------------------------
                        List<string> listhere = new List<string>();
                        foreach (XmlAttribute item in node.Attributes)
                        {
                          if (item.Name.Contains("Intervalli_")
                            || item.Name.Contains("IntervalloMIN_")
                            || item.Name.Contains("IntervalloMAX_"))
                            listhere.Add(item.Name);
                        }

                        foreach (string item in listhere)
                        {
                          if (node.Attributes[item] != null)
                            node.Attributes.Remove(node.Attributes[item]);
                        }
                -----------------------------------------------------------------------------*/
                foreach (DataRow dr in dataCampionamentoValori.Rows)
                {
                    str = dr["NomeAttributo"].ToString();
                    if (str.Contains("Intervalli_") || str.Contains("IntervalloMIN_")
                      || str.Contains("IntervalloMAX_"))
                        RemoveRigaAttributo(str);

                    if (node.Attributes[str] != null)
                        node.Attributes.Remove(node.Attributes[str]);
                }
            }

            str = "lst_Intestazione";
            if (node != null && node.Attributes[str] != null)
            {
                node.Attributes.Remove(node.Attributes[str]);
                RemoveRigaAttributo(str);
            }

            str = "stpStratificazionesino";
            if (node != null && node.Attributes[str] != null)
            {
                node.Attributes.Remove(node.Attributes[str]);
                RemoveRigaAttributo(str);
            }

            if (node != null && node.Attributes["RawData"] != null)
            {
                node.Attributes.Remove(node.Attributes["RawData"]);
                RawData = null;
                dataCampionamento.Rows[0]["RawData"] = string.Empty;
            }

            if (node != null && node.Attributes["FinalData"] != null)
            {
                node.Attributes.Remove(node.Attributes["FinalData"]);
                FinalData = null;
                dataCampionamento.Rows[0]["FinalData"] = string.Empty;
            }

            str = "TassoRotazione";
            if (node != null && node.Attributes[str] != null)
            {
                node.Attributes.Remove(node.Attributes[str]);
                FinalData = null;
                RemoveRigaAttributo(str);
                dataCampionamento.Rows[0]["FinalData"] = string.Empty;
            }
          ((WindowWorkArea)(this.Owner))._x.Save();
            this.Close();
        }

        //------------------------------------------------------------------------+
        //                            GetRigaAttributo                            |
        //------------------------------------------------------------------------+
        private string GetRigaAttributo(string attributo)
        {
            DataRow[] arrDr;
            string str;

            if (string.IsNullOrEmpty(attributo)
              || dataCampionamentoValori == null) return null;
            str = string.Format("NomeAttributo='{0}'", attributo.Replace("'", "''"));
            arrDr = dataCampionamentoValori.Select(str);
            return (arrDr.Length > 0) ?
              arrDr[0]["ValoreAttributo"].ToString() : string.Empty;
        }

        //------------------------------------------------------------------------+
        //                            SetRigaAttributo                            |
        //------------------------------------------------------------------------+
        private void SetRigaAttributo(string attributo, string valore)
        {
            DataRow[] arrDr;
            string str;

            if (string.IsNullOrEmpty(attributo)
              || dataCampionamentoValori == null) return;
            str = string.Format("NomeAttributo='{0}'", attributo.Replace("'", "''"));
            arrDr = dataCampionamentoValori.Select(str);
            if (arrDr.Length < 1)
            {
                dataCampionamentoValori.Rows.Add(
                  cBusinessObjects.GetIDTree(nodeNumber), cBusinessObjects.idcliente,
                  cBusinessObjects.idsessione, attributo, valore);
            }
            else arrDr[0]["ValoreAttributo"] = valore;
        }

        //------------------------------------------------------------------------+
        //                          RemoveRigaAttributo                           |
        //------------------------------------------------------------------------+
        private void RemoveRigaAttributo(string attributo)
        {
            DataRow[] arrDr;
            string str;

            if (string.IsNullOrEmpty(attributo)
              || dataCampionamentoValori == null) return;
            str = string.Format("NomeAttributo='{0}'", attributo.Replace("'", "''"));
            arrDr = dataCampionamentoValori.Select(str);
            if (arrDr.Length < 1) return;
            foreach (DataRow dr in arrDr) dr.Delete();
            dataCampionamentoValori.AcceptChanges();
        }

        //------------------------------------------------------------------------+
        //                            EsisteAttributo                             |
        //------------------------------------------------------------------------+
        private bool EsisteAttributo(string attributo)
        {
            DataRow[] arrDr;
            string str;

            if (string.IsNullOrEmpty(attributo)
              || dataCampionamentoValori == null) return false;
            str = string.Format("NomeAttributo='{0}'", attributo.Replace("'", "''"));
            arrDr = dataCampionamentoValori.Select(str);
            return (arrDr.Length > 0);
        }
    }
}
