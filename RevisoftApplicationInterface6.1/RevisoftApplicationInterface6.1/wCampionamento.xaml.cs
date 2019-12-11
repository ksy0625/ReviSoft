using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
using System.Data;

namespace RevisoftApplication
{
    public partial class wCampionamento : System.Windows.Window
    {
        Hashtable ht = new Hashtable();
        string _Cliente = "";
        string _IDCliente = "";
        string _Esercizio = "";
        string _IDSessione = "";
        string _IDTree = "";
        public bool diagres = false;

        XmlNode _node = null;

        public string changedID = "";

        public wCampionamento(XmlNode node,string IDCliente, string Cliente, string Esercizio, string IDSessione, string IDTree)
        {
            InitializeComponent();

            _Cliente = Cliente;
            _IDCliente = IDCliente;
            _Esercizio = Esercizio;
            _IDSessione = IDSessione;
            _IDTree = IDTree;

            _node = node;

            foreach (XmlNode item in node.OwnerDocument.SelectNodes("//Node"))
            {
               if((item.ChildNodes.Count == 1 && item.ParentNode.Attributes["Tipologia"].Value != "MultiNodo") || item.Attributes["Tipologia"].Value == "MultiNodo")
                {
                    if (item.Attributes["Codice"].Value.Contains(".0") || item.Attributes["Codice"].Value.Contains(".A") || item.Attributes["Codice"].Value.Contains(".B") || item.Attributes["Codice"].Value.Contains(".C") || item.Attributes["Codice"].Value.Contains(".D"))
                    {
                        if (!ht.ContainsValue(item.ParentNode.Attributes["ID"].Value) && !ht.Contains(item.ParentNode.Attributes["Codice"].Value + " " + item.ParentNode.Attributes["Titolo"].Value))
                        {
                         
                            cmbCartediLavoro.Items.Add(item.ParentNode.Attributes["Codice"].Value + " " + item.ParentNode.Attributes["Titolo"].Value);
                            ht.Add(item.ParentNode.Attributes["Codice"].Value + " " + item.ParentNode.Attributes["Titolo"].Value, item.ParentNode.Attributes["ID"].Value);
                        }
                    }
                    else
                    {
                        if (!ht.ContainsValue(item.Attributes["ID"].Value) && !ht.Contains(item.Attributes["Codice"].Value + " " + item.Attributes["Titolo"].Value))
                        {
                            cmbCartediLavoro.Items.Add(item.Attributes["Codice"].Value + " " + item.Attributes["Titolo"].Value);
                            ht.Add(item.Attributes["Codice"].Value + " " + item.Attributes["Titolo"].Value, item.Attributes["ID"].Value);
                        }
                    }
                }                    
            }        
        }       

        private void CalcolaValori()
        {
            int intQuantita = 0;
            int intFrom = 0;
            int intTo = 0;

            if (int.TryParse(txtQuantita.Text, out intQuantita) == true)
            {
                if(intQuantita <= 0)
                {
                    MessageBox.Show("Attenzione inserire una quantità numerica e maggiore di zero");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Attenzione inserire una valore numerico e maggiore di zero");
                return;
            }

            if (int.TryParse(txtFrom.Text, out intFrom) == true)
            {
                if (intFrom <= 0)
                {
                    MessageBox.Show("Attenzione inserire un valore numerico e maggiore di zero");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Attenzione inserire una Quantità numerica e maggiore di zero");
                return;
            }

            if (int.TryParse(txtTo.Text, out intTo) == true)
            {
                if (intTo <= intFrom)
                {
                    MessageBox.Show("Attenzione inserire una valore numerico e maggiore del primo valore");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Attenzione inserire una valore numerico e maggiore del primo valore");
                return;
            }

            ArrayList chosen = new ArrayList();
            int MaxIterations = 1000;

            while(intQuantita > 0)
            {
                MaxIterations = 1000;

                Random rnd = new Random(DateTime.Now.Millisecond);
                int chosenhere = rnd.Next(intFrom, intTo);
                while(chosen.Contains(chosenhere))
                {
                    if(MaxIterations-- <= 0)
                    {
                        break;
                    }
                    chosenhere = rnd.Next(intFrom, intTo);
                }

                if (MaxIterations <= 0)
                {
                    break;
                }

                chosen.Add(chosenhere);
                intQuantita--;
            }

            chosen.Sort();

            txtChosen.Items.Clear();

            foreach(int item in chosen)
            {
                txtChosen.Items.Add(item);
            }
            
        }

        private void Button_Calcola_Click(object sender, RoutedEventArgs e)
        {
            diagres = true;
            CalcolaValori();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            diagres = true;
            if (cmbCartediLavoro.SelectedIndex == -1)
            {
                MessageBox.Show("Selezionare una carta di lavoro");
                return;
            }

            if (txtTitolo.Text.Trim() == "")
            {
                MessageBox.Show("Inserire un titolo");
                return;
            }

            if(txtChosen.Items.Count == 0)
            {
                MessageBox.Show("Generare prima i numeri casuali");
                return;
            }

            if (!CreatePDF())
            {
                return;
            }

            this.Close();
        }

        private bool CreatePDF()
        {
           
            try
            {
                changedID = ht[cmbCartediLavoro.SelectedValue].ToString();
                
                string rtf_text = "";
                rtf_text += "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1040\\deflangfe1040\\deftab709";
                rtf_text += "{\\fonttbl{\\f0 Cambria}}";
                rtf_text += "{\\colortbl;\\red0\\green255\\blue255;\\red204\\green204\\blue204;\\red255\\green255\\blue255;\\red230\\green230\\blue230;}";
                rtf_text += "\\viewkind4\\uc1";

                rtf_text += "\\fs24 \\qc \\b " + _Cliente + " \\b0 \\line \\line ";
                //rtf_text += "\\fs24 \\qc " + _Esercizio + " \\line \\line ";

                rtf_text += "\\fs18 \\ql \\b 3.3.1 - Campionamento - numeri casuali \\b0 \\line \\ql La funzione ha generato i numeri casuali esposti nel presente documento \\line \\line ";

                rtf_text += "\\fs18 \\ql \\b Documento associato alla carta di lavoro: \\b0 \\line \\ql " + cmbCartediLavoro.SelectedValue.ToString() + " \\line \\line ";

                rtf_text += "\\fs18 \\ql \\b Scopo della ricerca casuale: \\b0 \\line \\ql " + txtScopo.Text + " \\line \\line ";


                rtf_text += "\\fs18 \\qc \\b N° " + txtQuantita.Text + " numeri casuali generati da " + txtFrom.Text + " a " + txtTo.Text + " \\b0 \\line \\line ";
                
                string inizioriga = "\\trowd\\trpaddl50\\trpaddt15\\trpaddr50\\trpaddb15\\trpaddfl3\\trpaddft3\\trpaddfr3\\trpaddfb3 ";
                string colore2 = "\\clcbpat3";
                string bordi = "\\clbrdrl\\brdrw10\\brdrs\\clbrdrt\\brdrw10\\brdrs\\clbrdrr\\brdrw10\\brdrs\\clbrdrb\\brdrw10\\brdrs"; //\\clpadt100
                string cell2 = "\\clvertalc\\cellx9900";
                string inizioriga2 = "\\pard\\intbl\\tx2291";
                string fineriga = "\\row ";

                rtf_text += " \\pard\\keep";

                rtf_text += "\\pard\\bgdkdcross\\cfpat1\\shading59110\\tx2291\\par";

                rtf_text += inizioriga + colore2 + bordi + cell2 + inizioriga2;

                rtf_text += " \\fs18 \\ql ";

                bool firstonedone = false;

                foreach (int item in txtChosen.Items)
                {
                    if (firstonedone == true)
                    {
                        rtf_text += " - ";
                    }
                    else
                    {
                        firstonedone = true;
                    }

                    rtf_text += item.ToString();
                }
                         
                rtf_text += " \\cell";

                rtf_text += fineriga;

                rtf_text += "\\pard\\bgdkdcross\\cfpat1\\shading59110\\tx2291\\par";

                rtf_text += "}";

                rtf_text = Convert2RTF(rtf_text);

                string filename = App.AppTempFolder + Guid.NewGuid().ToString();

                TextWriter tw = new StreamWriter(filename + ".rtf");
                tw.Write(rtf_text);
                tw.Close();

                //MM
                cDocNet wrdDoc = new cDocNet();
                wrdDoc.PageSetupPaperSize = "A4";
                wrdDoc.SaveAs(filename + ".pdf", filename + ".rtf", "WdSaveFormat.wdFormatPDF");
                //MM

                FileInfo fi = new FileInfo(filename + ".rtf");
                fi.Delete();
                DataTable dati = cBusinessObjects.GetData(int.Parse(changedID), typeof(ArchivioDocumenti), int.Parse(_IDCliente), int.Parse(_IDSessione));
                
                DataRow dt = dati.Rows.Add(int.Parse(changedID), int.Parse(_IDCliente), int.Parse(_IDSessione));

                DataTable tempdt = cBusinessObjects.ExecutesqlDataTable("SELECT MAX(ID) AS LASTID FROM ArchivioDocumenti");
                foreach (DataRow dd in tempdt.Rows)
                {
                    if (dd["LASTID"].ToString() == "")
                        dt["ID"] = 1;
                    else
                        dt["ID"] = int.Parse(dd["LASTID"].ToString()) + 1;
                }
                string newName = dt["ID"].ToString() + ".pdf";
                dt["File"] = newName;
                dt["Visualizza"] = "True";
                dt["NodoExtended"] = cmbCartediLavoro.SelectedValue;
                dt["Tree"] = _IDTree;
                dt["Titolo"] = txtTitolo.Text;
                cBusinessObjects.idcliente = int.Parse(_IDCliente);
                cBusinessObjects.idsessione = int.Parse(_IDSessione);
                dt["Tipo"] = ((rdbCorrente.IsChecked == true) ? (Convert.ToInt32(TipoDocumento.Corrente)).ToString() : (Convert.ToInt32(TipoDocumento.Permanente)).ToString());
                dt["TipoExtended"] = ((rdbCorrente.IsChecked == true) ? "Corrente" : "Permanente");




                string directory = App.AppModelliFolder;

                string file = filename + ".pdf";

                FileInfo fi2 = new FileInfo(file);
                if (fi2.Exists)
                {
                    string newfile = App.AppDocumentiFolder + "\\" + newName;
                    FileInfo newf = new FileInfo(newfile);
                    if (newf.Exists)
                    {
                        File.Delete(newfile);
                    }

                    fi2.CopyTo(newfile);
                }

                cBusinessObjects.SaveData(int.Parse(changedID), dati, typeof(ArchivioDocumenti));
                
                MessageBox.Show("Documento numeri casuali generato con successo");

               
            }
            catch (Exception ex)
            {
                cBusinessObjects.logger.Error(ex, "wCampionamento.CreatePDF exception");
              
                MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        public string Convert2RTFChar(string carattere)
        {
            string newChar = "";

            switch (carattere)
            {
                //case "!":
                //    newChar = "\\'21";
                //    break;
                case "\"":
                    newChar = "\\'22";
                    break;
                //case "#":
                //    newChar = "\\'23";
                //    break;
                case "$":
                    newChar = "\\'24";
                    break;
                case "%":
                    newChar = "\\'25";
                    break;
                case "&":
                    newChar = "\\'26";
                    break;
                case "'":
                    newChar = "\\'27";
                    break;
                //case "(":
                //    newChar = "\\'28";
                //    break;
                //case ")":
                //    newChar = "\\'29";
                //    break;
                //case "*":
                //    newChar = "\\'2a";
                //    break;
                //case "+":
                //    newChar = "\\'2b";
                //    break;
                //case ",":
                //    newChar = "\\'2c";
                //    break;
                //case "-":
                //    newChar = "\\'2d";
                //    break;
                //case ".":
                //    newChar = "\\'2e";
                //    break;
                //case "/":
                //    newChar = "\\'2f";
                //    break;
                //case ":":
                //    newChar = "\\'3a";
                //    break;
                //case ";":
                //    newChar = "\\'3b";
                //    break;
                //case "<":
                //    newChar = "\\'3c";
                //    break;
                //case "=":
                //    newChar = "\\'3d";
                //    break;
                //case ">":
                //    newChar = "\\'3e";
                //    break;
                //case "?":
                //    newChar = "\\'3f";
                //    break;
                //case "@":
                //    newChar = "\\'40";
                //    break;
                //case "[":
                //    newChar = "\\'5b";
                //    break;
                //case "\\":
                //    newChar = "\\'5c";
                //    break;
                //case "]":
                //    newChar = "\\'5d";
                //    break;
                //case "^":
                //    newChar = "\\'5e";
                //    break;
                //case "_":
                //    newChar = "\\'5f";
                //    break;
                //case "`":
                //    newChar = "\\'60";
                //    break;
                //case "{":
                //    newChar = "\\'7b";
                //    break;
                //case "|":
                //    newChar = "\\'7c";
                //    break;
                //case "}":
                //    newChar = "\\'7d";
                //    break;
                //case "~":
                //    newChar = "\\'7e";
                //    break;
                case "€":
                    newChar = "\\'80";
                    break;
                //case "͵":
                //    newChar = "\\'82";
                //    break;
                //case "ƒ":
                //    newChar = "\\'83";
                //    break;
                //case ""
                //    newChar = "\\'84";
                //    break;
                case "…":
                    newChar = "\\'85";
                    break;
                //case "†":
                //    newChar = "\\'86";
                //case "‡":
                //    newChar = "\\'87";
                //    break;
                case "∘":
                    newChar = "\\'88";
                    break;
                //case "‰":
                //    newChar = "\\'89";
                //    break;
                //case "Š":
                //    newChar = "\\'8a";
                //    break;
                //case "‹":
                //    newChar = "\\'8b";
                //    break;
                //case "Œ":
                //    newChar = "\\'8c";
                //    break;
                //case "Ž":
                //    newChar = "\\'8e";
                //    break;
                //case "‘":
                //    newChar = "\\'91";
                //    break;
                case "’":
                    newChar = "\\'92";
                    break;
                case "“":
                    newChar = "\\'93";
                    break;
                case "”":
                    newChar = "\\'94";
                    break;
                //case "•":
                //    newChar = "\\'95";
                //    break;
                //case "–":
                //    newChar = "\\'96";
                //    break;
                //case "—":
                //    newChar = "\\'97";
                //    break;
                //case "~":
                //    newChar = "\\'98";
                //    break;
                //case "™":
                //    newChar = "\\'99";
                //    break;
                //case "š":
                //    newChar = "\\'9a";
                //    break;
                //case "›":
                //    newChar = "\\'9b";
                //    break;
                //case "œ":
                //    newChar = "\\'9c";
                //    break;
                //case "ž":
                //    newChar = "\\'9e";
                //    break;
                //case "Ÿ":
                //    newChar = "\\'9f";
                //    break;
                //case "¡":
                //    newChar = "\\'a1";
                //    break;
                //case "¢":
                //    newChar = "\\'a2";
                //    break;
                //case "£":
                //    newChar = "\\'a3";
                //    break;
                //case "¤":
                //    newChar = "\\'a4";
                //    break;
                //case "¥":
                //    newChar = "\\'a5";
                //    break;
                //case "¦":
                //    newChar = "\\'a6";
                //    break;
                //case "§":
                //    newChar = "\\'a7";
                //    break;
                //case "¨":
                //    newChar = "\\'a8";
                //    break;
                case "©":
                    newChar = "\\'a9";
                    break;
                //case "ª":
                //    newChar = "\\'aa";
                //    break;
                //case "«":
                //    newChar = "\\'ab";
                //    break;
                //case "¬":
                //    newChar = "\\'ac";
                //    break;
                //case "®":
                //    newChar = "\\'ae";
                //    break;
                //case "¯":
                //    newChar = "\\'af";
                //    break;
                case "°":
                    newChar = "\\'b0";
                    break;
                case "±":
                    newChar = "\\'b1";
                    break;
                case "²":
                    newChar = "\\'b2";
                    break;
                case "³":
                    newChar = "\\'b3";
                    break;
                //case "´":
                //    newChar = "\\'b4";
                //    break;
                case "µ":
                    newChar = "\\'b5";
                    break;
                //case "¶":
                //    newChar = "\\'b6";
                //    break;
                //case "•":
                //  newChar = "\\'b7";
                //break;
                //case "¸":
                //    newChar = "\\'b8";
                //    break;
                //case "¹":
                //    newChar = "\\'b9";
                //    break;
                //case "º":
                //    newChar = "\\'ba";
                //    break;
                //case "»":
                //    newChar = "\\'bb";
                //    break;
                //case "¼":
                //    newChar = "\\'bc";
                //    break;
                //case "½":
                //    newChar = "\\'bd";
                //    break;
                //case "¾":
                //    newChar = "\\'be";
                //    break;
                //case "¿":
                //    newChar = "\\'bf";
                //    break;
                case "À":
                    newChar = "\\'c0";
                    break;
                case "Á":
                    newChar = "\\'c1";
                    break;
                case "Â":
                    newChar = "\\'c2";
                    break;
                case "Ã":
                    newChar = "\\'c3";
                    break;
                case "Ä":
                    newChar = "\\'c4";
                    break;
                case "Å":
                    newChar = "\\'c5";
                    break;
                case "Æ":
                    newChar = "\\'c6";
                    break;
                case "Ç":
                    newChar = "\\'c7";
                    break;
                case "È":
                    newChar = "\\'c8";
                    break;
                case "É":
                    newChar = "\\'c9";
                    break;
                case "Ê":
                    newChar = "\\'ca";
                    break;
                case "Ë":
                    newChar = "\\'cb";
                    break;
                case "Ì":
                    newChar = "\\'cc";
                    break;
                case "Í":
                    newChar = "\\'cd";
                    break;
                case "Î":
                    newChar = "\\'ce";
                    break;
                case "Ï":
                    newChar = "\\'cf";
                    break;
                case "Ð":
                    newChar = "\\'d0";
                    break;
                case "Ñ":
                    newChar = "\\'d1";
                    break;
                case "Ò":
                    newChar = "\\'d2";
                    break;
                case "Ó":
                    newChar = "\\'d3";
                    break;
                case "Ô":
                    newChar = "\\'d4";
                    break;
                case "Õ":
                    newChar = "\\'d5";
                    break;
                case "Ö":
                    newChar = "\\'d6";
                    break;
                //case "×":
                //    newChar = "\\'d7";
                //    break;
                case "Ø":
                    newChar = "\\'d8";
                    break;
                case "Ù":
                    newChar = "\\'d9";
                    break;
                case "Ú":
                    newChar = "\\'da";
                    break;
                case "Û":
                    newChar = "\\'db";
                    break;
                case "Ü":
                    newChar = "\\'dc";
                    break;
                case "Ý":
                    newChar = "\\'dd";
                    break;
                case "Þ":
                    newChar = "\\'de";
                    break;
                case "ß":
                    newChar = "\\'df";
                    break;
                case "à":
                    newChar = "\\'e0";
                    break;
                case "á":
                    newChar = "\\'e1";
                    break;
                case "â":
                    newChar = "\\'e2";
                    break;
                case "ã":
                    newChar = "\\'e3";
                    break;
                case "ä":
                    newChar = "\\'e4";
                    break;
                case "å":
                    newChar = "\\'e5";
                    break;
                case "æ":
                    newChar = "\\'e6";
                    break;
                case "ç":
                    newChar = "\\'e7";
                    break;
                case "è":
                    newChar = "\\'e8";
                    break;
                case "é":
                    newChar = "\\'e9";
                    break;
                case "ê":
                    newChar = "\\'ea";
                    break;
                case "ë":
                    newChar = "\\'eb";
                    break;
                case "ì":
                    newChar = "\\'ec";
                    break;
                case "í":
                    newChar = "\\'ed";
                    break;
                case "î":
                    newChar = "\\'ee";
                    break;
                case "ï":
                    newChar = "\\'ef";
                    break;
                case "ð":
                    newChar = "\\'f0";
                    break;
                case "ñ":
                    newChar = "\\'f1";
                    break;
                case "ò":
                    newChar = "\\'f2";
                    break;
                case "ó":
                    newChar = "\\'f3";
                    break;
                case "ô":
                    newChar = "\\'f4";
                    break;
                case "õ":
                    newChar = "\\'f5";
                    break;
                case "ö":
                    newChar = "\\'f6";
                    break;
                case "÷":
                    newChar = "\\'f7";
                    break;
                case "ø":
                    newChar = "\\'f8";
                    break;
                case "ù":
                    newChar = "\\'f9";
                    break;
                case "ú":
                    newChar = "\\'fa";
                    break;
                case "û":
                    newChar = "\\'fb";
                    break;
                case "ü":
                    newChar = "\\'fc";
                    break;
                case "ý":
                    newChar = "\\'fd";
                    break;
                case "þ":
                    newChar = "\\'fe";
                    break;
                case "ÿ":
                    newChar = "\\'ff";
                    break;
            }

            return newChar;
        }

        public string Convert2RTFString(string buff, string replaceChar)
        {
            return buff.Replace(replaceChar, Convert2RTFChar(replaceChar));
        }


        private string Convert2RTF(string buff)
        {
            buff = buff.Replace("\\'", "\\#");
            buff = Convert2RTFString(buff, "'"); //va messo per primo o causa problemi
            buff = buff.Replace("\\#", "\\'");

            //for (char c = '!'; c <= 'ÿ'; c++)
            //{
            //    buff = Convert2RTFString(buff, c.ToString() );
            //}

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
    }
}
