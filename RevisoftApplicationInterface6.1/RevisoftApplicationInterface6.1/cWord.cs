using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;
using Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Excel;

//andrea
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection; 



namespace RevisoftApplication
{
    class WordLib
    {

		private Microsoft.Office.Interop.Word.Application wrdApp;
        private Microsoft.Office.Interop.Excel.Application xlsApp;

		private _Document wrdDoc;
        private Workbook workBook;
        private Worksheet sheet;

		private Object oMissing = System.Reflection.Missing.Value;
		private Object oFalse = false;

		private string checkbox_Cheched = "\u0052";
		private string checkbox_UnCheched = "\u00A3";
		private string freccia_dx = "ð";
		private string freccia_bassodx = "Ä";

		object oEndOfDoc = "\\endofdoc";

		private string filename = "";
		private string font = "Arial";

		private bool firsttitle = true;
		public bool Watermark = true;
		public bool TitoloVerbale = true;		
		public string TemplateFileCompletePath = "";

		public WordLib()
        {
			wrdApp = new Microsoft.Office.Interop.Word.Application();
			wrdApp.Visible = false;
        }

		public void Open(Hashtable dati, string Cliente, string CodiceFiscale, string Sessione, string Titolo, bool Esercizio, bool TitoloInPrimaPagina)
		{
            //verifico presenza template
			FileInfo fi = new FileInfo(TemplateFileCompletePath);
			if (!fi.Exists)
			{
                App.ErrorLevel = App.ErrorTypes.Errore;
                RevisoftApplication.WindowGestioneMessaggi m = new WindowGestioneMessaggi();
                m.TipoMessaggioErrore = WindowGestioneMessaggi.TipologieMessaggiErrore.MancaPrintTemplate;
                m.VisualizzaMessaggio();
				return;
			}

            //creo template temporaneo
			filename = App.AppTempFolder + "\\" + Guid.NewGuid().ToString();
			fi.CopyTo(filename);

			wrdDoc = wrdApp.Documents.Open(filename);
			wrdDoc.Select();

            //excel
			xlsApp = new Microsoft.Office.Interop.Excel.Application();
			xlsApp.Visible = false;
			xlsApp.DisplayClipboardWindow = false;
			xlsApp.DisplayAlerts = false;

			workBook = xlsApp.Workbooks.Open(App.AppTemplateStampaBilancio, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

			sheet = (Worksheet)workBook.Worksheets[1];

			//Header
            if (Watermark)
            {
                foreach (Section wordSection in wrdDoc.Sections)
                {
                    wordSection.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Tables[wordSection.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Tables.Count].Rows[wordSection.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Tables[wordSection.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Tables.Count].Rows.Count].Cells[1].Range.Text = Cliente + Environment.NewLine + CodiceFiscale;
                    wordSection.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Tables[wordSection.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Tables.Count].Rows[wordSection.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Tables[wordSection.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Tables.Count].Rows.Count].Cells[2].Range.Text = Sessione;
                }
            }

			//Titolo prima pagina
            if (TitoloInPrimaPagina && Watermark)
			{
				wrdApp.Selection.Paragraphs.Add();
				wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
				wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Text = Environment.NewLine + Environment.NewLine + Environment.NewLine + "Azienda: " + Cliente + Environment.NewLine + Environment.NewLine + Environment.NewLine;

				wrdApp.Selection.Paragraphs.Add();
				wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
				if (Esercizio)
				{
					wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Text += "Esercizio di lavoro: " + Sessione + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine;
				}
				else
				{
					wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Text += "Sessione di lavoro: " + Sessione + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine;
				}

				wrdApp.Selection.Paragraphs.Add();
				wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
				wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Font.Bold = 1;
				float oldsize = wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Font.Size;
				wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Font.Size = 30;
				wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Text = Titolo + Environment.NewLine;

				wrdApp.Selection.Paragraphs.Add();
				wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Font.Bold = 0;
				wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
				wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Font.Size = oldsize;
				wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Text = "\f\r";
			}

			//watermark
            if (Watermark)
            {
                wrdApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekMainDocument;
                wrdApp.ActiveDocument.Sections[1].Range.Select();

                GestioneLicenza gl = new GestioneLicenza();
                string file = gl.GeneraFileFiligrana();

                try
                {
                    wrdApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekFirstPageHeader;

                    wrdApp.Selection.HeaderFooter.Shapes.AddPicture(file, false, true).Select();
                    wrdApp.Selection.ShapeRange.Rotation = -45;
                    wrdApp.Selection.ShapeRange.Top = 150;
                    wrdApp.Selection.ShapeRange.Left = 30;
                    wrdApp.Selection.ShapeRange.WrapFormat.Side = WdWrapSideType.wdWrapBoth;
                    wrdApp.Selection.ShapeRange.RelativeHorizontalPosition = WdRelativeHorizontalPosition.wdRelativeHorizontalPositionMargin;
                    wrdApp.Selection.ShapeRange.RelativeVerticalPosition = WdRelativeVerticalPosition.wdRelativeVerticalPositionMargin;
                }
                catch (Exception ex)
                {
                    string log = ex.Message;
                }


                try
                {
                    wrdApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekPrimaryHeader;

                    wrdApp.Selection.HeaderFooter.Shapes.AddPicture(file, false, true).Select();
                    wrdApp.Selection.ShapeRange.Rotation = -45;
                    wrdApp.Selection.ShapeRange.Top = 150;
                    wrdApp.Selection.ShapeRange.Left = 30;
                    wrdApp.Selection.ShapeRange.WrapFormat.Side = WdWrapSideType.wdWrapBoth;
                    wrdApp.Selection.ShapeRange.RelativeHorizontalPosition = WdRelativeHorizontalPosition.wdRelativeHorizontalPositionMargin;
                    wrdApp.Selection.ShapeRange.RelativeVerticalPosition = WdRelativeVerticalPosition.wdRelativeVerticalPositionMargin;
                }
                catch (Exception ex)
                {
                    string log = ex.Message;
                }
            }
            else
            {
				if (TitoloVerbale)
				{
					wrdApp.Selection.Paragraphs.Add();
					wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
					wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Text = "VERBALE DI VERIFICA PERIODICA DEL COLLEGIO SINDACALE" + Environment.NewLine + Environment.NewLine;

					string riga = "";

					if (dati.Contains("Data"))
					{
						riga += "Oggi " + dati["Data"].ToString() + " ";
					}

					if (dati.Contains("Inizio"))
					{
						riga += "alle ore " + dati["Inizio"].ToString() + " ";
					}

					wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Text = riga + "presso la sede della società si è riunito il collegio sindacale nelle persone di:" + Environment.NewLine + Environment.NewLine;

					if (dati.Contains("Presidente") && dati["Presidente"].ToString() != "")
					{
						wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Text = "PRESIDENTE\t\t\t" + dati["Presidente"].ToString() + Environment.NewLine;
					}

					if (dati.Contains("Sindaco1") && dati["Sindaco1"].ToString() != "")
					{
						wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Text = "SINDACO EFFETTIVO\t\t" + dati["Sindaco1"].ToString() + Environment.NewLine;
					}

					if (dati.Contains("Sindaco2") && dati["Sindaco2"].ToString() != "")
					{
						wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Text = "SINDACO EFFETTIVO\t\t" + dati["Sindaco2"].ToString() + Environment.NewLine;
					}

					wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Text = Environment.NewLine + "per effettuare la verifica periodica disposta dalle vigenti norme di legge." + Environment.NewLine + Environment.NewLine;

					if (dati.Contains("AssisitoDa") && dati["AssisitoDa"].ToString() != "")
					{
						wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Text = "Il collegio è assistito da\t\t" + dati["AssisitoDa"].ToString() + Environment.NewLine;
					}

					wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Text = Environment.NewLine + "Il collegio procede alla verifica come di seguito indicato:" + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine;

					wrdApp.Selection.Paragraphs[wrdApp.Selection.Paragraphs.Count].Range.Text = "\f\r";
				}
            }

			wrdApp.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekMainDocument;
		}

		public void AddTitle(string title)
		{
			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);

			if (firsttitle)
			{
				firsttitle = false;
			}
			else
			{
				p.Range.Text = "\f\r" + Environment.NewLine;

				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
			}

			p.Range.Font.Bold = 1;
			float oldsize = p.Range.Font.Size;
			p.Range.Font.Size = 20;
			p.Range.Text = title + Environment.NewLine;

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Font.Bold = 0;
			p.Range.Font.Size = oldsize;
			p.Range.Text = Environment.NewLine;
		}

		public void Add(XmlNode nodeTree, XmlNode nodeData, string Cliente, string Tree, string Sessione)
		{
			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			//Titolo
			string StrToAdd = "";
			
			if(Watermark)
			{
				StrToAdd += "Carta di lavoro: ";
			}

			StrToAdd += nodeTree.Attributes["Codice"].Value + " " + nodeTree.Attributes["Titolo"].Value;

			p.Range.Font.Bold = 1;
			p.Range.Text += StrToAdd + Environment.NewLine;

			//Istruzioni prima di tutto
			try
			{
				string istruzione = nodeTree.Attributes["Nota"].Value;

				while (istruzione.IndexOf('<') != -1)
				{
					int inizio = istruzione.IndexOf('<');
					int fine = istruzione.IndexOf('>');

					if (inizio == -1 || fine == -1)
					{
						break;
					}
					else
					{
						istruzione = istruzione.Remove(inizio, fine - inizio + 1);				
					}
				}

				istruzione = istruzione.Replace("&egrave;", "è").Replace("&agrave;", "à").Replace("&igrave;", "i").Replace("&ograve;", "ò").Replace("&ugrave;", "ù");

				if (istruzione.Trim() != "")
				{
					object r2 = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					Paragraph p2 = wrdDoc.Content.Paragraphs.Add(ref r2);
					p2.Range.Font.Bold = 0;
					p2.Range.Text += Environment.NewLine + "Istruzioni: " + Environment.NewLine + istruzione + Environment.NewLine;
				}
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}

			//Contenuto
			switch (nodeTree.Attributes["Tipologia"].Value)
			{
				case "Testo":
					AddTesto(nodeTree, nodeData);
					break;
				case "Tabella":
					AddTabella(nodeTree, nodeData);
					break;
				case "Tabella Replicabile":
					AddTabellaReplicabile(nodeTree, nodeData);
					break;
				case "Check List con Risultato":
				case "Check List":
					AddCheckList(nodeTree, nodeData);
					break;
				case "Check List +":
					AddCheckListpiu(nodeTree, nodeData);
					break;
				case "Nodo Multiplo":
				case "Nodo Multiplo Orizzontale":
					AddNodoMultiplo(nodeTree, nodeData);
					break;
				case "Excel: Numeri Casuali":
					AddNumeriCasuali(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					break;
				case "Excel: Capitale Sociale":
					AddCapitaleSociale(nodeTree, nodeData);
					break;
				case "Excel: Versamento imposte e contributi":
					AddVersamentoImposteContributi(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					break;
				case "Excel: Compensazioni":
					AddCompensazioni(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					break;
				case "Excel: Sospesi di Cassa":
					AddSospesidiCassa(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					break;
				case "Excel: Cassa Titoli":
					AddCassaTitoli(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					break;
				case "Excel: Cassa Assegni":
					AddCassaAssegni(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					break;
				case "Excel: Cassa Contante":
					AddCassaContante(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					break;
				case "Excel: Cassa Valori Bollati":
					AddCassaValoriBollati(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					break;
				case "Excel: Riconciliazioni Banche":
					AddRiconciliazioni(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					break;
				case "Excel: Materialità SP + CE":
					AddMaterialitaIpotesi1(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					break;
				case "Excel: Materialità SP e CE":
					AddMaterialitaIpotesi2(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					break;
				case "Excel: Materialità Personalizzata":
					AddMaterialitaIpotesi3(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					break;
				case "Excel: Affidamenti Bancari":
					AddAffidamenti(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					break;
				case "Excel: Errori Rilevati Riepilogo":
					AddRiepilogoErroriRilevati(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					break;
				case "Excel: Bilancio Riclassificato":
				case "Excel: Bilancio Abbreviato Riclassificato":
					AddBilancioRiclassificato(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					break;
				case "Excel: Bilancio Indici":
					AddBilancioIndici(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					break;
				case "Excel: Bilancio Abbreviato Indici":
					AddBilancioAbbreviatoIndici(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					break;					
				case "Excel":
					if (nodeTree.Attributes["ID"].Value == "202")
					{
						AddCicli(nodeTree, nodeData, nodeTree.Attributes["ID"].Value);
					}
					else if (nodeTree.Attributes["ID"].Value == "22")
					{
						AddRischioGlobale(nodeTree, nodeData, nodeTree.Attributes["ID"].Value); 
					}
					break;
				default:
					break;
			}

			//Note
			try
			{
				string nota = nodeData.Attributes["Osservazioni"].Value;

				object r2 = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				Paragraph p2 = wrdDoc.Content.Paragraphs.Add(ref r2);
				p2.Range.Font.Bold = 0;
				p2.Range.Text += Environment.NewLine + "Osservazioni Conclusive: " + nota + Environment.NewLine;
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}

			//Documenti associati
			XmlDataProviderManager xda = new XmlDataProviderManager(App.AppDocumentiDataFile, true);
			bool firsttime = true;
			foreach (XmlNode item in xda.Document.SelectNodes("//DOCUMENTI//DOCUMENTO[@Cliente='" + Cliente + "'][@Tree='" + Tree + "'][@Sessione='" + Sessione + "'][@Nodo='" + nodeTree.Attributes["ID"].Value + "']"))
			{
				if (firsttime)
				{
					object r4 = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					Paragraph p4 = wrdDoc.Content.Paragraphs.Add(ref r4);
					p4.Range.Font.Bold = 0;

					p4.Range.Text += Environment.NewLine + "Documenti Associati:" + Environment.NewLine;

					firsttime = false;
				}

				object r3 = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				Paragraph p3 = wrdDoc.Content.Paragraphs.Add(ref r3);
				p3.Range.Font.Bold = 0;

				p3.Range.Text += "\t- " + ((item.Attributes["Titolo"].Value.Trim() == "") ? "Nessun Titolo inserito" : item.Attributes["Titolo"].Value.Trim()) + ": " + ((item.Attributes["Descrizione"].Value.Trim() == "") ? "Nessuna Descrizione inserita" : item.Attributes["Descrizione"].Value.Trim()) + Environment.NewLine;
			}
		}

#region Tabella Replicabile
		private void AddTabellaReplicabile(XmlNode nodeTree, XmlNode nodeData)
		{
			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			//p.Range.Font.Bold = 0;
				
			XmlNode newnode = nodeData.Clone();
			ArrayList alTabs = new ArrayList();
			ArrayList alChildsToBeRemoved = new ArrayList();

			foreach (XmlNode item in newnode.ChildNodes)
			{
				if (item.Name == "Valore")
				{
					try 
					{
    					if(!alTabs.Contains(item.Attributes["Tab"].Value))
						{
							alTabs.Add(item.Attributes["Tab"].Value);							
						}

						alChildsToBeRemoved.Add(item);
					}
					catch (Exception ex)
					{
						string log = ex.Message;
					}
				}
			}

			foreach (XmlNode item in alChildsToBeRemoved)
			{
				newnode.RemoveChild(item);
			}

			bool esisteDefault = false;

			foreach (XmlNode item in newnode.ChildNodes)
			{
				if (item.Name == "Valore")
				{
					if (item.Attributes["Tab"] == null)
					{
						esisteDefault = true;
						break;
					}
				}
			}

			if (esisteDefault)
			{
				p.Range.Text = Environment.NewLine + nodeTree.Attributes["Tab"].Value + ":" + Environment.NewLine;
				AddTabella(nodeTree, newnode);

				p.Range.Text += Environment.NewLine;
			}

			foreach (string tab in alTabs)
			{
				alChildsToBeRemoved.Clear();

				if (!esisteDefault)
				{
					p.Range.Text += Environment.NewLine + tab + ":" + Environment.NewLine;
				}
				else
				{
					esisteDefault = false;
				}
	
				XmlNode newnodeint = nodeData.Clone();

				foreach (XmlNode item in newnodeint.ChildNodes)
				{
					if (item.Name == "Valore")
					{
						try 
						{
							if (item.Attributes["Tab"].Value == tab)
							{
								;
							}
							else
							{
								alChildsToBeRemoved.Add(item);
							}
						}
						catch (Exception ex)
						{
							string log = ex.Message;
							alChildsToBeRemoved.Add(item);
						}
					}
				}

				foreach (XmlNode item in alChildsToBeRemoved)
				{
					newnodeint.RemoveChild(item);
				}

				AddTabella(nodeTree, newnodeint);
			}
		}
#endregion

#region Tabella
		private void AddTabella(XmlNode nodeTree, XmlNode nodeData)
		{
			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 2, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(242, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(242, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Font.Bold = 1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Descrizione";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Bold = 1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "Dati";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			int index = 0;
			foreach (XmlNode item in nodeData.ChildNodes)//.SelectNodes("/Dati//Dato[@ID=" + nodeTree.Attributes["ID"].Value + "]/Valore"))
			{
				if (item.Name == "Valore")
				{
					AddNodoTabella(item, index++);
				}
			}

			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}

		private void AddNodoTabella(XmlNode node, int index)
		{
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = ((index%2 == 1)? WdColor.wdColorGray10 : WdColor.wdColorWhite);
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Font.Name = font;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = node.Attributes["name"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = ((index % 2 == 1) ? WdColor.wdColorGray10 : WdColor.wdColorWhite);
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Name = font;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = node.Attributes["value"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
		}

		#endregion 

#region Testo
		private void AddTesto(XmlNode nodeTree, XmlNode nodeData)
		{
			foreach (XmlNode item in nodeData.ChildNodes)//.SelectNodes("/Dati//Dato[@ID=" + nodeTree.Attributes["ID"].Value + "]/Valore"))
			{
				if (item.Name == "Valore")
				{
					object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
					p.Range.Font.Bold = 0;
					p.Range.Text = Environment.NewLine + item.Attributes["value"].Value + ":" + Environment.NewLine + item.Attributes["name"].Value + Environment.NewLine;
				}
			}
		}
		#endregion

#region Check List
		private void AddCheckList(XmlNode nodeTree, XmlNode nodeData)
		{
			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 8, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(25, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(340, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(15, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[4].SetWidth(20, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[5].SetWidth(15, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[6].SetWidth(25, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[7].SetWidth(15, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[8].SetWidth(30, WdRulerStyle.wdAdjustNone);

			foreach (XmlNode item in nodeData.ChildNodes)//.SelectNodes("/Dati//Dato[@ID=" + nodeTree.Attributes["ID"].Value + "]/Valore"))
			{
				if (item.Name == "Valore")
				{
					AddNodoCheckList(item);
				}
			}

			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			if (nodeData.Attributes["risultato"] != null && nodeData.Attributes["risultato"].Value != "")
			{
				p.Range.Text += nodeData.Attributes["risultato"].Value + Environment.NewLine;
			}
		}

		private void AddNodoCheckList(XmlNode node)
		{
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Font.Name = font;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = node.Attributes["Codice"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.KeepTogether = -1;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Name = font;

			try 
			{			
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = node.Attributes["name"].Value + ((node.Attributes["Nota"] != null && node.Attributes["Nota"].Value.Trim() == "") ? "" : Environment.NewLine + Environment.NewLine + "Nota: " + node.Attributes["Nota"].Value.Trim() + Environment.NewLine);
			}
			catch (Exception ex)
			{
				string log = ex.Message;
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = node.Attributes["name"].Value;
			}

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.KeepTogether = -1;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Name = "Wingdings 2";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (node.Attributes["value"].Value == "Si") ? checkbox_Cheched : checkbox_UnCheched;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Name = font;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "Si";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Name = "Wingdings 2";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (node.Attributes["value"].Value == "No") ? checkbox_Cheched : checkbox_UnCheched;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Font.Name = font;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = "No";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.Font.Name = "Wingdings 2";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.Text = (node.Attributes["value"].Value == "NA" || node.Attributes["value"].Value == "") ? checkbox_Cheched : checkbox_UnCheched;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[8].Range.Font.Name = font;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[8].Range.Text = "N/A";

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
		}

		#endregion 

#region Check List +

		Hashtable ht_RowstoBeMerged = new Hashtable();
		Hashtable ht_CellsToBeMerged = new Hashtable();

		private void AddCheckListpiu(XmlNode nodeTree, XmlNode nodeData)
		{
			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 7, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(25, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(15, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(137, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[4].SetWidth(15, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[5].SetWidth(137, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[6].SetWidth(15, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[7].SetWidth(137, WdRulerStyle.wdAdjustNone);

			ht_RowstoBeMerged.Clear();

			foreach (XmlNode item in nodeData.ChildNodes)//.SelectNodes("/Dati//Dato[@ID=" + nodeTree.Attributes["ID"].Value + "]/Valore"))
			{
				if (item.Name == "Valore")
				{
					AddNodoCheckListpiu(item);
				}
			}

			foreach (DictionaryEntry row in ht_RowstoBeMerged)
			{
				foreach (DictionaryEntry cell in ((Hashtable)(row.Value)))
				{
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[Convert.ToInt32(row.Key.ToString())].Cells[Convert.ToInt32(cell.Key.ToString())].Merge(wrdDoc.Tables[wrdDoc.Tables.Count].Rows[Convert.ToInt32(row.Key.ToString())].Cells[Convert.ToInt32(cell.Value.ToString())]);
				}				
			}
			
			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}

		private void AddNodoCheckListpiu(XmlNode node)
		{
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			ht_CellsToBeMerged = new Hashtable();

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Font.Italic = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Font.Name = font;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = Environment.NewLine + node.Attributes["Codice"].Value;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Italic = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Name = font;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = Environment.NewLine + node.Attributes["name"].Value;
			ht_CellsToBeMerged.Add(2, 7);
			
			ht_RowstoBeMerged.Add(wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count, ht_CellsToBeMerged);
			
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Italic = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Name = "Wingdings 2";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Bold = (node.Attributes["value"].Value == "Si") ? 1 : 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (node.Attributes["value"].Value == "Si") ? checkbox_Cheched : checkbox_UnCheched;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Italic = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Name = font;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Bold = (node.Attributes["value"].Value == "Si") ? 1 : 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "Alto";

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Italic = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Name = "Wingdings 2";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Bold = (node.Attributes["value"].Value == "No") ? 1 : 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (node.Attributes["value"].Value == "No") ? checkbox_Cheched : checkbox_UnCheched;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Italic = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Name = font;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Bold = (node.Attributes["value"].Value == "No") ? 1 : 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = "Medio";

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Font.Italic = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Font.Name = "Wingdings 2";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Bold = (node.Attributes["value"].Value == "NA" || node.Attributes["value"].Value == "") ? 1 : 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = (node.Attributes["value"].Value == "NA" || node.Attributes["value"].Value == "") ? checkbox_Cheched : checkbox_UnCheched;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.Font.Italic = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.Font.Name = font;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.Bold = (node.Attributes["value"].Value == "NA" || node.Attributes["value"].Value == "") ? 1 : 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.Text = "Basso";
			

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			ht_CellsToBeMerged = new Hashtable();

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Name = font;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Italic = 1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = node.Attributes["opzione1"].Value;
			ht_CellsToBeMerged.Add(2, 3);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Name = font;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Italic = 1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = node.Attributes["opzione2"].Value;

			ht_CellsToBeMerged.Add(4, 5);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Font.Name = font;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Font.Italic = 1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = node.Attributes["opzione3"].Value;			
			ht_CellsToBeMerged.Add(6, 7);

			ht_RowstoBeMerged.Add(wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count, ht_CellsToBeMerged);
			try
			{
				if (node.Attributes["Nota"].Value.Trim() != "")
				{
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
					ht_CellsToBeMerged = new Hashtable();

					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = node.Attributes["Nota"].Value.Trim();
					ht_CellsToBeMerged.Add(2, 7);

					ht_RowstoBeMerged.Add(wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count, ht_CellsToBeMerged);
				}
			}
			catch (Exception ex)
			{
				string log = ex.Message;
				
			}
		}

		#endregion
		
#region AddAltoMedioBasso

		private enum AltoMedioBasso { Sconosciuto, Alto, Medio, Basso };

		private void AddAltoMedioBasso(XmlNode nodeTree, XmlNode nodeData)
		{
			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 7, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(25, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(15, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(137, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[4].SetWidth(15, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[5].SetWidth(137, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[6].SetWidth(15, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[7].SetWidth(137, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();

			string valore = "";

			if (nodeData.Attributes["value"] != null)
			{
				valore = nodeData.Attributes["value"].Value;
			}

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Italic = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Name = "Wingdings 2";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Bold = (valore == (Convert.ToInt32(AltoMedioBasso.Alto)).ToString()) ? 1 : 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (valore == (Convert.ToInt32(AltoMedioBasso.Alto)).ToString()) ? checkbox_Cheched : checkbox_UnCheched;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Italic = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Name = font;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Bold = (valore == (Convert.ToInt32(AltoMedioBasso.Alto)).ToString()) ? 1 : 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "Alto";

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Italic = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Name = "Wingdings 2";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Bold = (valore == (Convert.ToInt32(AltoMedioBasso.Medio)).ToString()) ? 1 : 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (valore == (Convert.ToInt32(AltoMedioBasso.Medio)).ToString()) ? checkbox_Cheched : checkbox_UnCheched;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Italic = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Name = font;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Bold = (valore == (Convert.ToInt32(AltoMedioBasso.Medio)).ToString()) ? 1 : 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = "Medio";

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Font.Italic = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Font.Name = "Wingdings 2";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Bold = (valore == (Convert.ToInt32(AltoMedioBasso.Basso)).ToString() || valore == "") ? 1 : 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = (valore == (Convert.ToInt32(AltoMedioBasso.Basso)).ToString() || valore == "") ? checkbox_Cheched : checkbox_UnCheched;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.Font.Italic = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.Font.Name = font;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.Bold = (valore == (Convert.ToInt32(AltoMedioBasso.Basso)).ToString() || valore == "") ? 1 : 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.Text = "Basso";
			
			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}

		#endregion

#region Nodo Multiplo & Nodo Multiplo Orizzontale
		private void AddNodoMultiplo(XmlNode nodeTree, XmlNode nodeData)
		{
			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Font.Bold = 0;
			if (nodeTree.Attributes["Tab"].Value != "")
			{
				p.Range.Text = Environment.NewLine + nodeTree.Attributes["Tab"].Value + ":" + Environment.NewLine;
			}
			else
			{
				p.Range.Text = Environment.NewLine;
			}

			foreach (XmlNode item in nodeTree.ChildNodes)
			{
				if (item.Name == "Node")
				{
					if (item.Attributes["Tab"].Value != "")
					{
						p.Range.Text += item.Attributes["Tab"].Value + ":" + Environment.NewLine;
					}
					else
					{
						p.Range.Text = Environment.NewLine;
					}

					XmlNode itemData = nodeData.SelectSingleNode("//Dato[@ID=" + item.Attributes["ID"].Value + "]");

					switch (item.Attributes["Tipologia"].Value)
					{
						case "Testo":
							AddTesto(item, itemData);
							break;
						case "Tabella":
							AddTabella(item, itemData);
							break;
						case "Tabella Replicabile":
							AddTabellaReplicabile(item, itemData);
							break;
						case "Check List":
							AddCheckList(item, itemData);
							break;
						case "Check List +":
							AddCheckListpiu(item, itemData);
							break;
						case "Nodo Multiplo":
						case "Nodo Multiplo Orizzontale":
							AddNodoMultiplo(item, itemData);
							break;
						case "Excel: Errori Rilevati":
							AddErroriRilevati(nodeTree, nodeData, item.Attributes["ID"].Value);
							break;
						case "Excel: Bilancio Patrimoniale Attivo":
						case "Excel: Bilancio Patrimoniale Passivo":
						case "Excel: Bilancio Conto Economico":
						case "Excel: Bilancio Abbreviato Patrimoniale Attivo":
						case "Excel: Bilancio Abbreviato Patrimoniale Passivo":
						case "Excel: Bilancio Abbreviato Conto Economico":
						case "Excel: Bilancio":
							AddBilancio(nodeTree, nodeData, item.Attributes["ID"].Value, item.Attributes["Tab"].Value);
							break;
						case "Excel":
							if (item.Attributes["ID"].Value == "200")
							{
								AddValutazioneAmbiente(nodeTree, nodeData, item.Attributes["ID"].Value);
							}
							break;
						case "Report":
							AddAltoMedioBasso(item, itemData);
							break;
						default:
							break;
					}
				}
			}
		}
#endregion

#region Numeri Casuali
		private void AddNumeriCasuali(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			XmlNode nodenodo = nodeData.SelectSingleNode("/Dati//Dato[@ID=" + ID + "]");

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 7, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[4].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[5].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[6].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[7].SetWidth(50, WdRulerStyle.wdAdjustNone);
		
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = (nodenodo.Attributes["txt1"] == null)? "" : nodenodo.Attributes["txt1"].Value;			
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txt2"] == null) ? "" : nodenodo.Attributes["txt2"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txt3"] == null) ? "" : nodenodo.Attributes["txt3"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt4"] == null) ? "" : nodenodo.Attributes["txt4"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt5"] == null) ? "" : nodenodo.Attributes["txt5"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = (nodenodo.Attributes["txt6"] == null) ? "" : nodenodo.Attributes["txt6"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.Text = (nodenodo.Attributes["txt7"] == null) ? "" : nodenodo.Attributes["txt7"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = (nodenodo.Attributes["txt8"] == null) ? "" : nodenodo.Attributes["txt8"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txt9"] == null) ? "" : nodenodo.Attributes["txt9"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txt10"] == null) ? "" : nodenodo.Attributes["txt10"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt11"] == null) ? "" : nodenodo.Attributes["txt11"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt12"] == null) ? "" : nodenodo.Attributes["txt12"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = (nodenodo.Attributes["txt13"] == null) ? "" : nodenodo.Attributes["txt13"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.Text = (nodenodo.Attributes["txt14"] == null) ? "" : nodenodo.Attributes["txt14"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}
#endregion		

#region Capitale Sociale

		public enum TipologiaDato { Intero = 1, Stringa = 2, Double = 3, Percent = 4, TipoRipartizione = 5, Alto = 6, Medio = 7, Basso = 8, StringaDx = 9, TipoAffidamento = 10 };

		private void AddCapitaleSociale(XmlNode nodeTree, XmlNode nodeData)
		{
			#region capitale sociale
			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine + "COMPOSIZIONE CAPITALE SOCIALE:" + Environment.NewLine;

			Hashtable Header = new Hashtable();
			Hashtable Colonne = new Hashtable();
			Hashtable Lunghezze = new Hashtable();
			Hashtable Tipologia = new Hashtable();

			Header.Add(1, "Capitale Sociale / tipi e cat. azioni");
			Colonne.Add(1, "name");
			Lunghezze.Add(1, 242);
			Tipologia.Add(1, TipologiaDato.Stringa);
			Header.Add(2, "Deliberato");
			Colonne.Add(2, "deliberato");
			Lunghezze.Add(2, 80);
			Tipologia.Add(2, TipologiaDato.Double);
			Header.Add(3, "Sottoscritto");
			Colonne.Add(3, "sottoscritto");
			Lunghezze.Add(3, 80);
			Tipologia.Add(3, TipologiaDato.Double);
			Header.Add(4, "Versato");
			Colonne.Add(4, "versato");
			Lunghezze.Add(4, 80);
			Tipologia.Add(4, TipologiaDato.Double);

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, Header.Count, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			CreateColumn(Lunghezze);

			CreateTable(Header);

			int index = 0;
			foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + nodeTree.Attributes["ID"].Value + "]/Valore[@tipo='CapitaleSociale']"))
			{
				if (item.Name == "Valore")
				{
					AddNodoTable(item, index++, Colonne, Tipologia);
				}
			}

			#endregion

			#region Capitale sociale 2

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			Header = new Hashtable();
			Colonne = new Hashtable();
			Lunghezze = new Hashtable();
			Tipologia = new Hashtable();

			Header.Add(1, "Tipi e categorie di azioni");
			Colonne.Add(1, "name");
			Lunghezze.Add(1, 242);
			Tipologia.Add(1, TipologiaDato.Stringa);
			Header.Add(2, "Val.Nom.");
			Colonne.Add(2, "valnom");
			Lunghezze.Add(2, 80);
			Tipologia.Add(2, TipologiaDato.Double);
			Header.Add(3, "Numero");
			Colonne.Add(3, "numero");
			Lunghezze.Add(3, 80);
			Tipologia.Add(3, TipologiaDato.Double);
			Header.Add(4, "Totale");
			Colonne.Add(4, "totale");
			Lunghezze.Add(4, 80);
			Tipologia.Add(4, TipologiaDato.Double);

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, Header.Count, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			CreateColumn(Lunghezze);

			CreateTable(Header);

			index = 0;
			foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + nodeTree.Attributes["ID"].Value + "]/Valore[@tipo='TipiAzioni']"))
			{
				if (item.Name == "Valore")
				{
					AddNodoTable(item, index++, Colonne, Tipologia);
				}
			}

			#endregion

			#region ripartizione
			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine + "RIPARTIZIONE CAPITALE SOCIALE:" + Environment.NewLine;

			Header = new Hashtable();
			Colonne = new Hashtable();
			Lunghezze = new Hashtable();
			Tipologia = new Hashtable();

			Header.Add(1, "Soci");
			Colonne.Add(1, "name");
			Lunghezze.Add(1, 150);
			Tipologia.Add(1, TipologiaDato.Stringa);
			Header.Add(2, "Numero");
			Colonne.Add(2, "numero");
			Lunghezze.Add(2, 70);
			Tipologia.Add(2, TipologiaDato.Double);
			Header.Add(3, "V.N.");
			Colonne.Add(3, "valnom");
			Lunghezze.Add(3, 40);
			Tipologia.Add(3, TipologiaDato.Double);
			Header.Add(4, "Ammontare");
			Colonne.Add(4, "totale");
			Lunghezze.Add(4, 70);
			Tipologia.Add(4, TipologiaDato.Double);
			Header.Add(5, "%");
			Colonne.Add(5, "percentuale");
			Lunghezze.Add(5, 60);
			Tipologia.Add(5, TipologiaDato.Percent);
			Header.Add(6, "Tipo");
			Colonne.Add(6, "tiporipartizione");
			Lunghezze.Add(6, 90);
			Tipologia.Add(6, TipologiaDato.TipoRipartizione);

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, Header.Count, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			CreateColumn(Lunghezze);

			CreateTable(Header);

			index = 0;
			foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + nodeTree.Attributes["ID"].Value + "]/Valore[@tipo='Ripartizione']"))
			{
				if (item.Name == "Valore")
				{
					AddNodoTable(item, index++, Colonne, Tipologia);
				}
			}

			#endregion

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}

		private void CreateColumn(Hashtable colonne)
		{
			for (int i = 1; i <= colonne.Count; i++)
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Columns[i].SetWidth(((int)(colonne[i])), WdRulerStyle.wdAdjustNone);
			}
		}

		private void CreateTable(Hashtable colonne)
		{
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();

			for (int i = 1; i <= colonne.Count; i++)
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.Font.Bold = -1;
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.Text = colonne[i].ToString();
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			}
		}

		private void AddNodoTable(XmlNode node, int index, Hashtable colonne, Hashtable tipologie)
		{
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();

			for (int i = 1; i <= colonne.Count; i++)
			{
				if (node.Attributes["bold"] != null)
				{
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
				}
				else
				{
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Shading.BackgroundPatternColor = ((index % 2 == 1) ? WdColor.wdColorGray10 : WdColor.wdColorWhite);
				}

				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.Font.Bold = 0;
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.Font.Name = font;

				string valore = "";
				
				if (node.Attributes[colonne[i].ToString()] != null)
				{
					switch ((TipologiaDato)(tipologie[i]))
					{
						case TipologiaDato.Intero:
							valore = node.Attributes[colonne[i].ToString()].Value;
							double intero = 0;

							double.TryParse(valore, out intero);

							valore = Convert.ToInt32(intero).ToString();

							if (valore == "0")
							{
								valore = "";
							}

							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
							break;
						case TipologiaDato.Stringa:
							valore = node.Attributes[colonne[i].ToString()].Value;
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
							break;
						case TipologiaDato.StringaDx:
							valore = node.Attributes[colonne[i].ToString()].Value;
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
							break;
						case TipologiaDato.Double:
							valore = ConvertNumber(node.Attributes[colonne[i].ToString()].Value);
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
							break;
						case TipologiaDato.Percent:
							valore = ConvertPercent(node.Attributes[colonne[i].ToString()].Value);
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
							break;
						case TipologiaDato.TipoRipartizione:
							valore = node.Attributes[colonne[i].ToString()].Value;
							switch (valore)
							{
								case "Q":
									valore = "Quote s.r.l.";
									break;
								case "AO":
									valore = "Azioni ordinarie";
									break;
								case "AP":
									valore = "Azioni privilegiate";
									break;
								case "Div":
								default:								
									valore = "Altro";
									break;
							}
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
							break;
						case TipologiaDato.TipoAffidamento:
							valore = node.Attributes[colonne[i].ToString()].Value;
							switch (valore)
							{
								case "a":
									valore = "conto corrente";
									break;
                              case "b":
									valore = "sbf - riba";
									break;
                                case "c":
									valore = "anticipo fatture";
									break;
                               case "d":
									valore = "anticipo export";
									break;
                               case "e":
									valore = "anticipo import";
									break;
                               case "f":
									valore = "chirografario";
									break;
                               case "g":
									valore = "mutui ipotecari o similari";
									break;
                                case "h":
									valore = "operazioni a termine";
									break;
								case "i":
									valore = "finanza derivata";
									break;
								case "l":
									valore = "garanzie prestate";
									break;
								case "m":
									valore = "altro 1";
									break;
                                case "n":
									valore = "altro 2";
									break;
								default:
									valore = "Altro";
									break;
							}
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
							break;
						case TipologiaDato.Alto:
							valore = node.Attributes[colonne[i].ToString()].Value;

							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.Font.Bold = 0;
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.Font.Italic = 0;
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.Font.Name = "Wingdings 2";
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.Font.Bold = (valore == (Convert.ToInt32(AltoMedioBasso.Alto)).ToString()) ? 1 : 0;
							valore = (valore == (Convert.ToInt32(AltoMedioBasso.Alto)).ToString()) ? checkbox_Cheched : checkbox_UnCheched;
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
							break;
						case TipologiaDato.Medio:
							valore = node.Attributes[colonne[i].ToString()].Value;

							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.Font.Bold = 0;
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.Font.Italic = 0;
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.Font.Name = "Wingdings 2";
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.Font.Bold = (valore == (Convert.ToInt32(AltoMedioBasso.Medio)).ToString()) ? 1 : 0;
							valore = (valore == (Convert.ToInt32(AltoMedioBasso.Medio)).ToString()) ? checkbox_Cheched : checkbox_UnCheched;
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;							
							break;
						case TipologiaDato.Basso:
							valore = node.Attributes[colonne[i].ToString()].Value;

							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.Font.Bold = 0;
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.Font.Italic = 0;
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.Font.Name = "Wingdings 2";
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.Font.Bold = (valore == (Convert.ToInt32(AltoMedioBasso.Basso)).ToString()) ? 1 : 0;
							valore = (valore == (Convert.ToInt32(AltoMedioBasso.Basso)).ToString()) ? checkbox_Cheched : checkbox_UnCheched;
							wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;							
							break;
						default:
							break;
					}				
				}
				
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Range.Text = valore;
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[i].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
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
				return String.Format("{0:#,#.00}", dblValore);
			}
		}

		private string ConvertPercent(string valore)
		{
			double dblValore = 0.0;

			double.TryParse(valore, out dblValore);

			if (dblValore == 0.0)
			{
				return "";
			}
			else
			{
				dblValore = dblValore * 100.0;
				return String.Format("{0:#,#.00}", dblValore) + " %";
			}
		}
#endregion

#region ValutazioneAmbiente
		private void AddValutazioneAmbiente(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			Hashtable Header = new Hashtable();
			Hashtable Colonne = new Hashtable();
			Hashtable Lunghezze = new Hashtable();
			Hashtable Tipologia = new Hashtable();

			Header.Add(1, "");
			Colonne.Add(1, "name");
			Lunghezze.Add(1, 242);
			Tipologia.Add(1, TipologiaDato.Stringa);
			Header.Add(2, "Alto");
			Colonne.Add(2, "Alto");
			Lunghezze.Add(2, 80);
			Tipologia.Add(2, TipologiaDato.Intero);
			Header.Add(3, "Medio");
			Colonne.Add(3, "Medio");
			Lunghezze.Add(3, 80);
			Tipologia.Add(3, TipologiaDato.Intero);
			Header.Add(4, "Basso");
			Colonne.Add(4, "Basso");
			Lunghezze.Add(4, 80);
			Tipologia.Add(4, TipologiaDato.Intero);

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, Header.Count, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			CreateColumn(Lunghezze);

			CreateTable(Header);

			int index = 0;
			foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + ID + "]/Valore"))
			{
				if (item.Name == "Valore")
				{
					AddNodoTable(item, index++, Colonne, Tipologia);
				}
			}

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}
		#endregion

#region ErroriRilevati
		private void AddErroriRilevati(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			Hashtable Header = new Hashtable();
			Hashtable Colonne = new Hashtable();
			Hashtable Lunghezze = new Hashtable();
			Hashtable Tipologia = new Hashtable();

			Header.Add(1, "Descrizione errore");
			Colonne.Add(1, "name");
			Lunghezze.Add(1, 242);
			Tipologia.Add(1, TipologiaDato.Stringa);
			Header.Add(2, "Importo");
			Colonne.Add(2, "importo");
			Lunghezze.Add(2, 80);
			Tipologia.Add(2, TipologiaDato.Double);

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, Header.Count, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			CreateColumn(Lunghezze);

			CreateTable(Header);

			int index = 0;
			foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + ID + "]/Valore"))
			{
				if (item.Name == "Valore")
				{
					AddNodoTable(item, index++, Colonne, Tipologia);
				}
			}

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}
		#endregion

#region Riepilogo Errori Rilevati
		private void AddRiepilogoErroriRilevati(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			XmlNode nodenodo = nodeData.SelectSingleNode("/Dati//Dato[@ID=" + ID + "]");

			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine + "ERRORE TRASCURABILE SINGOLO" + Environment.NewLine + "Stato Patrimoniale: " + ((nodenodo.Attributes["txtErroreTollerabileSP"] == null) ? "" : ConvertNumber(nodenodo.Attributes["txtErroreTollerabileSP"].Value)) + Environment.NewLine + "Stato Economico: " + ((nodenodo.Attributes["txtErroreTollerabileCE"] == null) ? "" : ConvertNumber(nodenodo.Attributes["txtErroreTollerabileCE"].Value)) + Environment.NewLine;

			Hashtable Header = new Hashtable();
			Hashtable Colonne = new Hashtable();
			Hashtable Lunghezze = new Hashtable();
			Hashtable Tipologia = new Hashtable();

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 8, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(120, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(70, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[4].SetWidth(70, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[5].SetWidth(70, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[6].SetWidth(10, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[7].SetWidth(70, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[8].SetWidth(70, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Voce N°";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "Richiamo dell'errore";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "Errore Esercizio Attuale";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "Eventuale Analogo Errore Esercizio Precedente";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = "Errore Netto a Carico dell'Esercizio";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.Text = "Stato Patrimoniale";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;			

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[8].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[8].Range.Text = "Stato Economico";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[8].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[8].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			if (nodenodo.Attributes["rowTOT"] != null)
			{
				for (int i = 4; i <= Convert.ToInt32(nodenodo.Attributes["rowTOT"].Value); i++)
				{
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = (nodenodo.Attributes["txtCodice" + i.ToString()] == null) ? "" : nodenodo.Attributes["txtCodice" + i.ToString()].Value;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtName" + i.ToString()] == null) ? "" : nodenodo.Attributes["txtName" + i.ToString()].Value;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEA" + i.ToString()] == null) ? "" : ConvertNumber(nodenodo.Attributes["txtEA" + i.ToString()].Value);
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txtAP" + i.ToString()] == null) ? "" : ConvertNumber(nodenodo.Attributes["txtAP" + i.ToString()].Value);
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txtDIFF" + i.ToString()] == null) ? "" : ConvertNumber(nodenodo.Attributes["txtDIFF" + i.ToString()].Value);
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleNone;

					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.Text = (nodenodo.Attributes["txtSP" + i.ToString()] == null) ? "" : ConvertNumber(nodenodo.Attributes["txtSP" + i.ToString()].Value);
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
					
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[8].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[8].Range.Text = (nodenodo.Attributes["txtCE" + i.ToString()] == null) ? "" : ConvertNumber(nodenodo.Attributes["txtCE" + i.ToString()].Value);
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[8].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
					wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[8].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
				}
			}

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtTotEA"] == null) ? "" : ConvertNumber(nodenodo.Attributes["txtTotEA"].Value);
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txtTotDIFF"] == null) ? "" : ConvertNumber(nodenodo.Attributes["txtTotDIFF"].Value);
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.Text = "ERRORI DA SEGNALARE IN RELAZIONE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Merge(wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[8]);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "S.P.";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;
			
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = "C.E.";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "LIMITE DI MATERIALITA' ERRORE COMPLESSIVO TOLLERABILE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtTotMaterialitaSP"] == null) ? "" : ConvertNumber(nodenodo.Attributes["txtTotMaterialitaSP"].Value);
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txtTotMaterialitaCE"] == null) ? "" : ConvertNumber(nodenodo.Attributes["txtTotMaterialitaCE"].Value);
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[7].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "GIUDIZIO SUL BILANCIO";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtGIUDIZIOSP"] == null) ? "" : nodenodo.Attributes["txtGIUDIZIOSP"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txtGIUDIZIOCE"] == null) ? "" : nodenodo.Attributes["txtGIUDIZIOCE"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}
		#endregion		

        //public string ReplaceXml(string valore)
        //{
        //    string returnvalue = valore;

        //    returnvalue = returnvalue.Replace(" ", "").Replace("'", "").Replace("<", "").Replace("/", "").Replace("\\", "").Replace(">", "").Replace("\"", "");

        //    return returnvalue;
        //}

#region VersamentoImposteContributi
		private void AddVersamentoImposteContributi(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			ArrayList periodi = new ArrayList();

			foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + ID + "]/Valore[@tipo='VersamentoImposteContributi']"))
			{
				if (item.Attributes["periodo"] != null && !periodi.Contains(item.Attributes["periodo"].Value))
				{
					periodi.Add(item.Attributes["periodo"].Value);
				}
			}

			foreach (string periodo in periodi)
			{
				object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Bold = -1;
				p.Range.Text = Environment.NewLine + periodo + Environment.NewLine + Environment.NewLine;

				if (nodeData.Attributes["PeriodoDiRiferimento_" + StaticUtilities.ReplaceXml(periodo.ToString())] != null)
				{
					r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					p = wrdDoc.Content.Paragraphs.Add(ref r);
					p.Range.Bold = 0;
					p.Range.Text = "Periodo di riferimento: " + nodeData.Attributes["PeriodoDiRiferimento_" + StaticUtilities.ReplaceXml(periodo.ToString())].Value + Environment.NewLine;
				}

				if (nodeData.Attributes["AMezzo_" + StaticUtilities.ReplaceXml(periodo.ToString())] != null)
				{
					r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					p = wrdDoc.Content.Paragraphs.Add(ref r);
					p.Range.Bold = 0;
					p.Range.Text = "A mezzo: " + nodeData.Attributes["AMezzo_" + StaticUtilities.ReplaceXml(periodo.ToString())].Value + Environment.NewLine;
				}

				if (nodeData.Attributes["DataDiPagamento_" + StaticUtilities.ReplaceXml(periodo.ToString())] != null)
				{
					r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					p = wrdDoc.Content.Paragraphs.Add(ref r);
					p.Range.Bold = 0;
					p.Range.Text = "Data di pagamento: " + nodeData.Attributes["DataDiPagamento_" + StaticUtilities.ReplaceXml(periodo.ToString())].Value + Environment.NewLine;
				}
				
				Hashtable Header = new Hashtable();
				Hashtable Colonne = new Hashtable();
				Hashtable Lunghezze = new Hashtable();
				Hashtable Tipologia = new Hashtable();

				Header.Add(1, "Descrizione tributo");
				Colonne.Add(1, "name");
				Lunghezze.Add(1, 230);
				Tipologia.Add(1, TipologiaDato.Stringa);
				Header.Add(2, "Codice tributo");
				Colonne.Add(2, "codice");
				Lunghezze.Add(2, 100);
				Tipologia.Add(2, TipologiaDato.Stringa);
				Header.Add(3, "Importo pagato");
				Colonne.Add(3, "importoPagato");
				Lunghezze.Add(3, 80);
				Tipologia.Add(3, TipologiaDato.Double);
				Header.Add(4, "Importo compensato");
				Colonne.Add(4, "importoCompensato");
				Lunghezze.Add(4, 80);
				Tipologia.Add(4, TipologiaDato.Double);

				wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, Header.Count, ref oMissing, ref oMissing);

				wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
				wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

				CreateColumn(Lunghezze);

				CreateTable(Header);

				int index = 0;
				foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + ID + "]/Valore[@tipo='VersamentoImposteContributi'][@periodo='" + periodo + "']"))
				{
					if (item.Name == "Valore")
					{
						AddNodoTable(item, index++, Colonne, Tipologia);
					}
				}

				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Text = Environment.NewLine;
			}			
		}
		#endregion

#region Compensazioni
		private void AddCompensazioni(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			ArrayList periodi = new ArrayList();

			foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + ID + "]/Valore[@tipo='Compensazioni']"))
			{
				if (item.Attributes["periodo"] != null && !periodi.Contains(item.Attributes["periodo"].Value))
				{
					periodi.Add(item.Attributes["periodo"].Value);
				}
			}

			foreach (string periodo in periodi)
			{
				object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Bold = -1;
				p.Range.Text = Environment.NewLine + periodo + Environment.NewLine + Environment.NewLine;

				if (nodeData.Attributes["CreditoEsistente_" + StaticUtilities.ReplaceXml(periodo.ToString())] != null)
				{
					r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
					p = wrdDoc.Content.Paragraphs.Add(ref r);
					p.Range.Bold = 0;
					p.Range.Text = "Credito esistente: " + nodeData.Attributes["CreditoEsistente_" + StaticUtilities.ReplaceXml(periodo.ToString())].Value + Environment.NewLine;
				}
				
				Hashtable Header = new Hashtable();
				Hashtable Colonne = new Hashtable();
				Hashtable Lunghezze = new Hashtable();
				Hashtable Tipologia = new Hashtable();

				Header.Add(1, "Data");
				Colonne.Add(1, "name");
				Lunghezze.Add(1, 230);
				Tipologia.Add(1, TipologiaDato.Stringa);
				Header.Add(2, "Tributi compensati");
				Colonne.Add(2, "codice");
				Lunghezze.Add(2, 180);
				Tipologia.Add(2, TipologiaDato.Stringa);
				Header.Add(3, "Importi");
				Colonne.Add(3, "importoPagato");
				Lunghezze.Add(3, 80);
				Tipologia.Add(3, TipologiaDato.Double);

				wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, Header.Count, ref oMissing, ref oMissing);

				wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
				wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

				CreateColumn(Lunghezze);

				CreateTable(Header);

				int index = 0;
				foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + ID + "]/Valore[@tipo='Compensazioni'][@periodo='" + periodo + "']"))
				{
					if (item.Name == "Valore")
					{
						AddNodoTable(item, index++, Colonne, Tipologia);
					}
				}

				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Text = Environment.NewLine;
			}			
		}
		#endregion

#region Cicli
		private void AddCicli(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			Hashtable Header = new Hashtable();
			Hashtable Colonne = new Hashtable();
			Hashtable Lunghezze = new Hashtable();
			Hashtable Tipologia = new Hashtable();

			Header.Add(1, "");
			Colonne.Add(1, "name");
			Lunghezze.Add(1, 242);
			Tipologia.Add(1, TipologiaDato.Stringa);
			Header.Add(2, "Alto");
			Colonne.Add(2, "value");
			Lunghezze.Add(2, 80);
			Tipologia.Add(2, TipologiaDato.Alto);
			Header.Add(3, "Medio");
			Colonne.Add(3, "value");
			Lunghezze.Add(3, 80);
			Tipologia.Add(3, TipologiaDato.Medio);
			Header.Add(4, "Basso");
			Colonne.Add(4, "value");
			Lunghezze.Add(4, 80);
			Tipologia.Add(4, TipologiaDato.Basso);

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, Header.Count, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			CreateColumn(Lunghezze);

			CreateTable(Header);

			int index = 0;
			foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + ID + "]/Valore"))
			{
				if (item.Name == "Valore")
				{
					AddNodoTable(item, index++, Colonne, Tipologia);
				}
			}

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}
		#endregion

#region SospesidiCassa
		private void AddSospesidiCassa(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			if (nodeData.Attributes["PeriodoDiRiferimento"] != null)
			{
				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Bold = 0;
				p.Range.Text = "Data di riferimento: " + nodeData.Attributes["PeriodoDiRiferimento"].Value + Environment.NewLine;
			}

			Hashtable Header = new Hashtable();
			Hashtable Colonne = new Hashtable();
			Hashtable Lunghezze = new Hashtable();
			Hashtable Tipologia = new Hashtable();

			Header.Add(1, "Debitore");
			Colonne.Add(1, "name");
			Lunghezze.Add(1, 200);
			Tipologia.Add(1, TipologiaDato.Stringa);
			Header.Add(2, "Causale");
			Colonne.Add(2, "codice");
			Lunghezze.Add(2, 130);
			Tipologia.Add(2, TipologiaDato.Stringa);
			Header.Add(3, "Data prelievo");
			Colonne.Add(3, "importoPagato");
			Lunghezze.Add(3, 80);
			Tipologia.Add(3, TipologiaDato.Stringa);
			Header.Add(4, "Importo");
			Colonne.Add(4, "importoCompensato");
			Lunghezze.Add(4, 80);
			Tipologia.Add(4, TipologiaDato.Double);

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, Header.Count, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			CreateColumn(Lunghezze);

			CreateTable(Header);

			int index = 0;
			foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + ID + "]/Valore[@tipo='SospesiDiCassa']"))
			{
				if (item.Name == "Valore")
				{
					AddNodoTable(item, index++, Colonne, Tipologia);
				}
			}

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}
		#endregion

#region CassaTitoli
		private void AddCassaTitoli(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			if (nodeData.Attributes["CreditoEsistente"] != null)
			{
				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Bold = 0;
				p.Range.Text = "Data di riferimento: " + nodeData.Attributes["CreditoEsistente"].Value + Environment.NewLine;
			}

			Hashtable Header = new Hashtable();
			Hashtable Colonne = new Hashtable();
			Hashtable Lunghezze = new Hashtable();
			Hashtable Tipologia = new Hashtable();

			Header.Add(1, "Titolo");
			Colonne.Add(1, "name");
			Lunghezze.Add(1, 200);
			Tipologia.Add(1, TipologiaDato.Stringa);
			Header.Add(2, "Scadenza");
			Colonne.Add(2, "codice");
			Lunghezze.Add(2, 130);
			Tipologia.Add(2, TipologiaDato.Stringa);
			Header.Add(3, "Euro");
			Colonne.Add(3, "importoPagato");
			Lunghezze.Add(3, 80);
			Tipologia.Add(3, TipologiaDato.Double);

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, Header.Count, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			CreateColumn(Lunghezze);

			CreateTable(Header);

			int index = 0;
			foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + ID + "]/Valore[@tipo='CassaTitoli']"))
			{
				if (item.Name == "Valore")
				{
					AddNodoTable(item, index++, Colonne, Tipologia);
				}
			}

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}
		#endregion		

#region CassaAssegni
		private void AddCassaAssegni(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			if (nodeData.Attributes["PeriodoDiRiferimento"] != null)
			{
				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Bold = 0;
				p.Range.Text = "Data di riferimento: " + nodeData.Attributes["PeriodoDiRiferimento"].Value + Environment.NewLine;
			}

			Hashtable Header = new Hashtable();
			Hashtable Colonne = new Hashtable();
			Hashtable Lunghezze = new Hashtable();
			Hashtable Tipologia = new Hashtable();

			Header.Add(1, "Traente");
			Colonne.Add(1, "name");
			Lunghezze.Add(1, 200);
			Tipologia.Add(1, TipologiaDato.Stringa);
			Header.Add(2, "Banca");
			Colonne.Add(2, "codice");
			Lunghezze.Add(2, 130);
			Tipologia.Add(2, TipologiaDato.Stringa);
			Header.Add(3, "Piazza");
			Colonne.Add(3, "importoPagato");
			Lunghezze.Add(3, 80);
			Tipologia.Add(3, TipologiaDato.Stringa);
			Header.Add(4, "Importo");
			Colonne.Add(4, "importoCompensato");
			Lunghezze.Add(4, 80);
			Tipologia.Add(4, TipologiaDato.Double);

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, Header.Count, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			CreateColumn(Lunghezze);

			CreateTable(Header);

			int index = 0;
			foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + ID + "]/Valore[@tipo='CassaAssegni']"))
			{
				if (item.Name == "Valore")
				{
					AddNodoTable(item, index++, Colonne, Tipologia);
				}
			}

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}
		#endregion		

#region CassaContante
		private void AddCassaContante(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			if (nodeData.Attributes["CreditoEsistente"] != null)
			{
				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Bold = 0;
				p.Range.Text = "Data di riferimento: " + nodeData.Attributes["CreditoEsistente"].Value + Environment.NewLine;
			}

			Hashtable Header = new Hashtable();
			Hashtable Colonne = new Hashtable();
			Hashtable Lunghezze = new Hashtable();
			Hashtable Tipologia = new Hashtable();

			Header.Add(1, "N°pezzi");
			Colonne.Add(1, "numeropezzi");
			Lunghezze.Add(1, 100);
			Tipologia.Add(1, TipologiaDato.Intero);
			Header.Add(2, "Unitario");
			Colonne.Add(2, "unitario");
			Lunghezze.Add(2, 130);
			Tipologia.Add(2, TipologiaDato.StringaDx);
			Header.Add(3, "Euro");
			Colonne.Add(3, "euro");
			Lunghezze.Add(3, 80);
			Tipologia.Add(3, TipologiaDato.Double);

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, Header.Count, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			CreateColumn(Lunghezze);

			CreateTable(Header);

			int index = 0;
			foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + ID + "]/Valore[@tipo='CassaContante']"))
			{
				if (item.Name == "Valore")
				{
					AddNodoTable(item, index++, Colonne, Tipologia);
				}
			}

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			if (nodeData.Attributes["txtTotaleComplessivo"] != null)
			{
				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Bold = 0;
				p.Range.Text = "Totale Complessivo: " + ConvertNumber(nodeData.Attributes["txtTotaleComplessivo"].Value) + Environment.NewLine;
			}
			
			if (nodeData.Attributes["txtSaldoSchedaContabile"] != null)
			{
				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Bold = 0;
				p.Range.Text = "Saldo Scheda Contabile: " + ConvertNumber(nodeData.Attributes["txtSaldoSchedaContabile"].Value) + Environment.NewLine;
			}

			if (nodeData.Attributes["txtDifferenza"] != null)
			{
				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Bold = 0;
				p.Range.Text = "Differenza: " + ConvertNumber(nodeData.Attributes["txtDifferenza"].Value) + Environment.NewLine;
			}			
		}
		#endregion		
		
#region CassaValoriBollati
		private void AddCassaValoriBollati(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			if (nodeData.Attributes["CreditoEsistente"] != null)
			{
				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Bold = 0;
				p.Range.Text = "Data di riferimento: " + nodeData.Attributes["CreditoEsistente"].Value + Environment.NewLine;
			}

			Hashtable Header = new Hashtable();
			Hashtable Colonne = new Hashtable();
			Hashtable Lunghezze = new Hashtable();
			Hashtable Tipologia = new Hashtable();

			Header.Add(1, "N°pezzi");
			Colonne.Add(1, "numeropezzi");
			Lunghezze.Add(1, 100);
			Tipologia.Add(1, TipologiaDato.Intero);
			Header.Add(2, "Unitario");
			Colonne.Add(2, "unitario");
			Lunghezze.Add(2, 130);
			Tipologia.Add(2, TipologiaDato.StringaDx);
			Header.Add(3, "Euro");
			Colonne.Add(3, "euro");
			Lunghezze.Add(3, 80);
			Tipologia.Add(3, TipologiaDato.Double);

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, Header.Count, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			CreateColumn(Lunghezze);

			CreateTable(Header);

			int index = 0;
			foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + ID + "]/Valore[@tipo='CassaContante']"))
			{
				if (item.Name == "Valore")
				{
					AddNodoTable(item, index++, Colonne, Tipologia);
				}
			}

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			if (nodeData.Attributes["txtTotaleComplessivo"] != null)
			{
				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Bold = 0;
				p.Range.Text = "Totale Complessivo: " + ConvertNumber(nodeData.Attributes["txtTotaleComplessivo"].Value) + Environment.NewLine;
			}
			
			if (nodeData.Attributes["txtSaldoSchedaContabile"] != null)
			{
				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Bold = 0;
				p.Range.Text = "Saldo Scheda Contabile: " + ConvertNumber(nodeData.Attributes["txtSaldoSchedaContabile"].Value) + Environment.NewLine;
			}

			if (nodeData.Attributes["txtDifferenza"] != null)
			{
				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Bold = 0;
				p.Range.Text = "Differenza: " + ConvertNumber(nodeData.Attributes["txtDifferenza"].Value) + Environment.NewLine;
			}			
		}
		#endregion		

#region Riconciliazioni
		private void AddRiconciliazioni(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			if (nodeData.Attributes["CreditoEsistente"] != null)
			{
				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Bold = 0;
				p.Range.Text = "Data di riferimento: " + nodeData.Attributes["CreditoEsistente"].Value + Environment.NewLine;
			}

			Hashtable Header = new Hashtable();
			Hashtable Colonne = new Hashtable();
			Hashtable Lunghezze = new Hashtable();
			Hashtable Tipologia = new Hashtable();

			Header.Add(1, "Banca");
			Colonne.Add(1, "banca");
			Lunghezze.Add(1, 110);
			Tipologia.Add(1, TipologiaDato.Stringa);
			Header.Add(2, "c/c n°");
			Colonne.Add(2, "ccn");
			Lunghezze.Add(2, 50);
			Tipologia.Add(2, TipologiaDato.Stringa);
			Header.Add(3, "Saldo contabile");
			Colonne.Add(3, "saldocontabile");
			Lunghezze.Add(3, 70);
			Tipologia.Add(3, TipologiaDato.Double);
			Header.Add(4, "Saldo e/c Banca");
			Colonne.Add(4, "saldoec");
			Lunghezze.Add(4, 70);
			Tipologia.Add(4, TipologiaDato.Double);
			Header.Add(5, "Differenza");
			Colonne.Add(5, "differenza");
			Lunghezze.Add(5, 70);
			Tipologia.Add(5, TipologiaDato.Double);
			Header.Add(6, "Riconciliato");
			Colonne.Add(6, "riconciliato");
			Lunghezze.Add(6, 70);
			Tipologia.Add(6, TipologiaDato.Double);
			Header.Add(7, "Imp. con ric.");
			Colonne.Add(7, "importoconriconciliato");
			Lunghezze.Add(7, 70);
			Tipologia.Add(7, TipologiaDato.Double);

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, Header.Count, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			CreateColumn(Lunghezze);

			CreateTable(Header);

			int index = 0;
			foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + ID + "]/Valore[@tipo='Riconciliazioni']"))
			{
				if (item.Name == "Valore")
				{
					AddNodoTable(item, index++, Colonne, Tipologia);
				}
			}

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}
		#endregion		
	
#region Rischio Globale
		private void AddRischioGlobale(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			XmlNode nodenodo = nodeData.SelectSingleNode("/Dati//Dato[@ID=" + ID + "]");

			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			
			#region legenda
			p.Range.Text = Environment.NewLine + "LEGENDA RISCHIO GLOBALE:" + Environment.NewLine;

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 6, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(180, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[4].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[5].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[6].SetWidth(80, WdRulerStyle.wdAdjustNone);
		
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "TABELLA DI RIFERIMENTO";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;	

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "RISCHIO INTRINSECO (valutazione dell'ambiente e controllo int.)";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;

			Cell cellfrom_1 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1];
			Cell cellto_1 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2];
			Cell cellfrom_2 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3];
			Cell cellto_2 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
			
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "ALTO";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "MEDIO";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = "BASSO";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "RISCHIO DI CONTROLLO (CICLI)";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			Cell cellfrom_3 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "ALTO";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "molto alto";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "alto";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = "medio";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = "RISCHIO GLOBALE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			Cell cellfrom_4 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "MEDIO";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "alto";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "medio";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = "basso";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			Cell cellto_3 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1];
			
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "BASSO";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "medio";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "basso";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = "molto basso";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			
			Cell cellto_4 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6];

			cellfrom_1.Merge(cellto_1);
			cellfrom_2.Merge(cellto_2);
			cellfrom_3.Merge(cellto_3);
			cellfrom_4.Merge(cellto_4);
			#endregion

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			#region calcolo
			p.Range.Text = Environment.NewLine + "CALCOLO RISCHIO GLOBALE:" + Environment.NewLine;

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 6, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(180, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(20, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(80, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[4].SetWidth(80, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[5].SetWidth(20, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[6].SetWidth(80, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "RISCHIO INTRINSECO valutazione dell'ambiente e controllo interno";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Font.Name = "Wingdings";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = freccia_dx;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			if (nodenodo.Attributes["txt1"] != null)
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = nodenodo.Attributes["txt1"].Value;
			}
			else
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "";
			}
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			#region ciclo vendite
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "RISCHIO DI CONTROLLO ciclo vendite";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Name = "Wingdings";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = freccia_bassodx;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			if (nodenodo.Attributes["txt2"] != null)
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = nodenodo.Attributes["txt2"].Value;
			}
			else
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "";
			}
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Name = "Wingdings";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = freccia_dx;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			if (nodenodo.Attributes["txt2c"] != null)
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = nodenodo.Attributes["txt2c"].Value;
			}
			else
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = "";
			}
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			#endregion

			#region ciclo acquisti
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "RISCHIO DI CONTROLLO ciclo acquisti";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Name = "Wingdings";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = freccia_bassodx;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			if (nodenodo.Attributes["txt3"] != null)
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = nodenodo.Attributes["txt3"].Value;
			}
			else
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "";
			}
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Name = "Wingdings";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = freccia_dx;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			if (nodenodo.Attributes["txt3c"] != null)
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = nodenodo.Attributes["txt3c"].Value;
			}
			else
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = "";
			}
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			#endregion

			#region ciclo personale
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "RISCHIO DI CONTROLLO ciclo personale";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Name = "Wingdings";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = freccia_bassodx;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			if (nodenodo.Attributes["txt4"] != null)
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = nodenodo.Attributes["txt4"].Value;
			}
			else
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "";
			}
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Name = "Wingdings";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = freccia_dx;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			if (nodenodo.Attributes["txt4c"] != null)
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = nodenodo.Attributes["txt4c"].Value;
			}
			else
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = "";
			}
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			#endregion

			#region ciclo tesoreria
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "RISCHIO DI CONTROLLO ciclo tesoreria";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Name = "Wingdings";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = freccia_bassodx;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			if (nodenodo.Attributes["txt5"] != null)
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = nodenodo.Attributes["txt5"].Value;
			}
			else
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "";
			}
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Name = "Wingdings";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = freccia_dx;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			if (nodenodo.Attributes["txt5c"] != null)
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = nodenodo.Attributes["txt5c"].Value;
			}
			else
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = "";
			}
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			#endregion

			#region ciclo magazzino
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Font.Bold = -1;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "RISCHIO DI CONTROLLO ciclo magazzino";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Font.Name = "Wingdings";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = freccia_bassodx;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			if (nodenodo.Attributes["txt6"] != null)
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = nodenodo.Attributes["txt6"].Value;
			}
			else
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "";
			}
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Font.Name = "Wingdings";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = freccia_dx;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Font.Bold = 0;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			if (nodenodo.Attributes["txt6c"] != null)
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = nodenodo.Attributes["txt6c"].Value;
			}
			else
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = "";
			}
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			#endregion
			#endregion

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}
		#endregion

#region Materialita Ipotesi1
		private void AddMaterialitaIpotesi1(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			XmlNode nodenodo = nodeData.SelectSingleNode("/Dati//Dato[@ID=" + ID + "]");

			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			
#region parametri di riferimento

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 6, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(100, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(80, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[4].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[5].SetWidth(80, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[6].SetWidth(80, WdRulerStyle.wdAdjustNone);
		
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Parametri di riferimento";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;


			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "Valori di bilancio";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "Percentuali";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = "Limiti";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;


			Cell cellfrom_1 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3];
			Cell cellto_1 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4];
			Cell cellfrom_2 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5];
			Cell cellto_2 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6];
			Cell cellfrom_3 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1];
			Cell cellfrom_4 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			Cell cellto_3 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1];
			Cell cellto_4 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "Minima";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "Massima";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = "Minima";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = "Massimo";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Totale Attività";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txt1"] == null) ? "" : nodenodo.Attributes["txt1"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txt1_p_min"] == null) ? "" : nodenodo.Attributes["txt1_p_min"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt1_p_max"] == null) ? "" : nodenodo.Attributes["txt1_p_max"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt1min"] == null) ? "" : nodenodo.Attributes["txt1min"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = (nodenodo.Attributes["txt1lmax"] == null) ? "" : nodenodo.Attributes["txt1lmax"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Patrimonio Netto";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txt2"] == null) ? "" : nodenodo.Attributes["txt2"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txt2_p_min"] == null) ? "" : nodenodo.Attributes["txt2_p_min"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt2_p_max"] == null) ? "" : nodenodo.Attributes["txt2_p_max"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt2lmin"] == null) ? "" : nodenodo.Attributes["txt2lmin"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = (nodenodo.Attributes["txt2lmax"] == null) ? "" : nodenodo.Attributes["txt2lmax"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;


			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Ricavi dell'esercizio";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txt3"] == null) ? "" : nodenodo.Attributes["txt3"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txt3_p_min"] == null) ? "" : nodenodo.Attributes["txt3_p_min"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt3_p_max"] == null) ? "" : nodenodo.Attributes["txt3_p_max"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt3lmin"] == null) ? "" : nodenodo.Attributes["txt3lmin"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = (nodenodo.Attributes["txt3lmax"] == null) ? "" : nodenodo.Attributes["txt3lmax"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Risultati ante Imposte";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txt4"] == null) ? "" : nodenodo.Attributes["txt4"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txt4_p_min"] == null) ? "" : nodenodo.Attributes["txt4_p_min"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt4_p_max"] == null) ? "" : nodenodo.Attributes["txt4_p_max"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt4lmin"] == null) ? "" : nodenodo.Attributes["txt4lmin"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = (nodenodo.Attributes["txt4lmax"] == null) ? "" : nodenodo.Attributes["txt4lmax"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			cellfrom_1.Merge(cellto_1);
			cellfrom_2.Merge(cellto_2);
			cellfrom_3.Merge(cellto_3);
			cellfrom_4.Merge(cellto_4);
			#endregion

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine + "1° TIPO - limite di materialità - media fra tutti i minimi ed i massimi" + Environment.NewLine;

			#region IPOTESI

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 6, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(180, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[4].SetWidth(80, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[5].SetWidth(80, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Totali minimi / massimi";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt5"] == null) ? "" : nodenodo.Attributes["txt5"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt6"] == null) ? "" : nodenodo.Attributes["txt6"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "MATERIALITA' - MEDIA fra minimi e massimi";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt7"] == null) ? "" : nodenodo.Attributes["txt7"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			Cell fromx = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4];
			Cell tox = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "ERRORE TOLLERABILE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "50%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt9"] == null) ? "" : nodenodo.Attributes["txt9"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			Cell fromy = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4];
			Cell toy = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Materialità Operativa ";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "25%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "50%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt12"] == null) ? "" : nodenodo.Attributes["txt12"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt13"] == null) ? "" : nodenodo.Attributes["txt13"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			fromx.Merge(tox);
			fromy.Merge(toy);
			#endregion

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine + "Dottrina Nazionale" + Environment.NewLine;
			
			#region DOTTRINA NAZIONALE

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 6, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(180, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[4].SetWidth(80, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[5].SetWidth(80, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Parametri di riferimento";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "Percentuali";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "Limiti";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;


			Cell cellfrom_5 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2];
			Cell cellto_5 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3];
			Cell cellfrom_6 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4];
			Cell cellto_6 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5];
			Cell cellfrom_7 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			Cell cellto_7 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "Minima";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "Massima";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "Minima";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = "Massimo";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Totale Attività";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "0,50%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "1,00%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt1lmindn"] == null) ? "" : nodenodo.Attributes["txt1lmindn"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt1lmaxdn"] == null) ? "" : nodenodo.Attributes["txt1lmaxdn"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Patrimonio Netto";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "1,00%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "5,00%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt2lmindn"] == null) ? "" : nodenodo.Attributes["txt2lmindn"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt2lmaxdn"] == null) ? "" : nodenodo.Attributes["txt2lmaxdn"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;


			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Ricavi dell'esercizio";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "0,50%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "1,00%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt3lmindn"] == null) ? "" : nodenodo.Attributes["txt3lmindn"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt3lmaxdn"] == null) ? "" : nodenodo.Attributes["txt3lmaxdn"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Risultato ante imposte";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "5,00%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "10,00%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt4lmindn"] == null) ? "" : nodenodo.Attributes["txt4lmindn"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt4lmaxdn"] == null) ? "" : nodenodo.Attributes["txt4lmaxdn"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			cellfrom_5.Merge(cellto_5);
			cellfrom_6.Merge(cellto_6);
			cellfrom_7.Merge(cellto_7);
			#endregion

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}
		#endregion

#region Materialita Ipotesi2
		private void AddMaterialitaIpotesi2(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			XmlNode nodenodo = nodeData.SelectSingleNode("/Dati//Dato[@ID=" + ID + "]");

			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);

			#region parametri di riferimento

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 6, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(100, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(80, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[4].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[5].SetWidth(80, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[6].SetWidth(80, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Parametri di riferimento";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;


			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "Valori di bilancio";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "Percentuali";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = "Limiti";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;


			Cell cellfrom_1 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3];
			Cell cellto_1 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4];
			Cell cellfrom_2 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5];
			Cell cellto_2 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6];
			Cell cellfrom_3 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1];
			Cell cellfrom_4 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			Cell cellto_3 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1];
			Cell cellto_4 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "Minima";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "Massima";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = "Minima";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = "Massimo";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Totale Attività";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txt1"] == null) ? "" : nodenodo.Attributes["txt1"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txt1_p_min"] == null) ? "" : nodenodo.Attributes["txt1_p_min"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt1_p_max"] == null) ? "" : nodenodo.Attributes["txt1_p_max"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt1min"] == null) ? "" : nodenodo.Attributes["txt1min"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = (nodenodo.Attributes["txt1lmax"] == null) ? "" : nodenodo.Attributes["txt1lmax"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Patrimonio Netto";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txt2"] == null) ? "" : nodenodo.Attributes["txt2"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txt2_p_min"] == null) ? "" : nodenodo.Attributes["txt2_p_min"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt2_p_max"] == null) ? "" : nodenodo.Attributes["txt2_p_max"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt2lmin"] == null) ? "" : nodenodo.Attributes["txt2lmin"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = (nodenodo.Attributes["txt2lmax"] == null) ? "" : nodenodo.Attributes["txt2lmax"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;


			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Ricavi dell'esercizio";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txt3"] == null) ? "" : nodenodo.Attributes["txt3"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txt3_p_min"] == null) ? "" : nodenodo.Attributes["txt3_p_min"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt3_p_max"] == null) ? "" : nodenodo.Attributes["txt3_p_max"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt3lmin"] == null) ? "" : nodenodo.Attributes["txt3lmin"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = (nodenodo.Attributes["txt3lmax"] == null) ? "" : nodenodo.Attributes["txt3lmax"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Risultati ante Imposte";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txt4"] == null) ? "" : nodenodo.Attributes["txt4"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txt4_p_min"] == null) ? "" : nodenodo.Attributes["txt4_p_min"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt4_p_max"] == null) ? "" : nodenodo.Attributes["txt4_p_max"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt4lmin"] == null) ? "" : nodenodo.Attributes["txt4lmin"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.Text = (nodenodo.Attributes["txt4lmax"] == null) ? "" : nodenodo.Attributes["txt4lmax"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			cellfrom_1.Merge(cellto_1);
			cellfrom_2.Merge(cellto_2);
			cellfrom_3.Merge(cellto_3);
			cellfrom_4.Merge(cellto_4);
			#endregion

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine + "2° TIPO - limite di materialità - medie fra i minimi ed i massimi per S.P. e C.E." + Environment.NewLine;

			#region IPOTESI SP

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 6, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(180, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[4].SetWidth(80, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[5].SetWidth(80, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Totali minimi / massimi - ATTIVITA' + PATRIMONIO";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt5_2sp"] == null) ? "" : nodenodo.Attributes["txt5_2sp"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt6_2sp"] == null) ? "" : nodenodo.Attributes["txt6_2sp"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "MATERIALITA' - MEDIA fra minimi e massimi";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt7_2sp"] == null) ? "" : nodenodo.Attributes["txt7_2sp"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			Cell fromx = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4];
			Cell tox = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "ERRORE TOLLERABILE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "50%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt9_2sp"] == null) ? "" : nodenodo.Attributes["txt9_2sp"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			Cell fromy = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4];
			Cell toy = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Materialità Operativa ";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "25%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "50%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt12_2sp"] == null) ? "" : nodenodo.Attributes["txt12_2sp"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt13_2sp"] == null) ? "" : nodenodo.Attributes["txt13_2sp"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			fromx.Merge(tox);
			fromy.Merge(toy);
			#endregion

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			#region IPOTESI EC

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 6, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(180, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[4].SetWidth(80, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[5].SetWidth(80, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Totali minimi / massimi - ECONOMICI";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt5_2ce"] == null) ? "" : nodenodo.Attributes["txt5_2ce"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt6_2ce"] == null) ? "" : nodenodo.Attributes["txt6_2ce"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "MATERIALITA' - MEDIA fra minimi e massimi";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt7_2ce"] == null) ? "" : nodenodo.Attributes["txt7_2ce"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			Cell fromxec = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4];
			Cell toxec = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "ERRORE TOLLERABILE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "50%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt9_2ce"] == null) ? "" : nodenodo.Attributes["txt9_2ce"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			Cell fromyec = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4];
			Cell toyec = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Materialità Operativa ";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "25%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "50%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt12_2ce"] == null) ? "" : nodenodo.Attributes["txt12_2ce"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt13_2ce"] == null) ? "" : nodenodo.Attributes["txt13_2ce"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			fromxec.Merge(toxec);
			fromyec.Merge(toyec);
			#endregion

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine + "Dottrina Nazionale" + Environment.NewLine;

			#region DOTTRINA NAZIONALE

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 6, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(180, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[4].SetWidth(80, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[5].SetWidth(80, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Parametri di riferimento";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "Percentuali";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "Limiti";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;


			Cell cellfrom_5 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2];
			Cell cellto_5 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3];
			Cell cellfrom_6 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4];
			Cell cellto_6 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5];
			Cell cellfrom_7 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			Cell cellto_7 = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "Minima";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "Massima";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "Minima";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = "Massimo";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Totale Attività";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "0,50%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "1,00%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt1lmindn"] == null) ? "" : nodenodo.Attributes["txt1lmindn"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt1lmaxdn"] == null) ? "" : nodenodo.Attributes["txt1lmaxdn"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Patrimonio Netto";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "1,00%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "5,00%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt2lmindn"] == null) ? "" : nodenodo.Attributes["txt2lmindn"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt2lmaxdn"] == null) ? "" : nodenodo.Attributes["txt2lmaxdn"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;


			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Ricavi dell'esercizio";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "0,50%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "1,00%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt3lmindn"] == null) ? "" : nodenodo.Attributes["txt3lmindn"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt3lmaxdn"] == null) ? "" : nodenodo.Attributes["txt3lmaxdn"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Risultato ante imposte";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "5,00%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "10,00%";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt4lmindn"] == null) ? "" : nodenodo.Attributes["txt4lmindn"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt4lmaxdn"] == null) ? "" : nodenodo.Attributes["txt4lmaxdn"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			cellfrom_5.Merge(cellto_5);
			cellfrom_6.Merge(cellto_6);
			cellfrom_7.Merge(cellto_7);
			#endregion

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}
		#endregion

#region Materialita Ipotesi3
		private void AddMaterialitaIpotesi3(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			XmlNode nodenodo = nodeData.SelectSingleNode("/Dati//Dato[@ID=" + ID + "]");

			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine + "3° TIPO - limite di materialità - stabilito con criteri alternativi" + Environment.NewLine;

			#region IPOTESI SP

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 6, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(180, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[4].SetWidth(80, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[5].SetWidth(80, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "MATERIALITA' - MEDIA fra minimi e massimi";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt7_3sp"] == null) ? "" : nodenodo.Attributes["txt7_3sp"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			Cell fromx = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4];
			Cell tox = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "ERRORE TOLLERABILE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txt8_3sp"] == null) ? "" : nodenodo.Attributes["txt8_3sp"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt9_3sp"] == null) ? "" : nodenodo.Attributes["txt9_3sp"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			Cell fromy = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4];
			Cell toy = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Materialità Operativa ";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txt10_3sp"] == null) ? "" : nodenodo.Attributes["txt10_3sp"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txt11_3sp"] == null) ? "" : nodenodo.Attributes["txt11_3sp"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt12_3sp"] == null) ? "" : nodenodo.Attributes["txt12_3sp"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt13_3sp"] == null) ? "" : nodenodo.Attributes["txt13_3sp"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			fromx.Merge(tox);
			fromy.Merge(toy);
			#endregion

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			#region IPOTESI EC

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 6, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(180, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(50, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[4].SetWidth(80, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[5].SetWidth(80, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "MATERIALITA' - MEDIA fra minimi e massimi";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt7_3ec"] == null) ? "" : nodenodo.Attributes["txt7_3ec"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			Cell fromxec = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4];
			Cell toxec = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "ERRORE TOLLERABILE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txt8_3ec"] == null) ? "" : nodenodo.Attributes["txt8_3ec"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt9_3ec"] == null) ? "" : nodenodo.Attributes["txt9_3ec"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			Cell fromyec = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4];
			Cell toyec = wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5];

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Materialità Operativa ";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txt10_3ec"] == null) ? "" : nodenodo.Attributes["txt10_3ec"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txt11_3ec"] == null) ? "" : nodenodo.Attributes["txt11_3ec"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = (nodenodo.Attributes["txt12_3ec"] == null) ? "" : nodenodo.Attributes["txt12_3ec"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.Text = (nodenodo.Attributes["txt13_3ec"] == null) ? "" : nodenodo.Attributes["txt13_3ec"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			fromxec.Merge(toxec);
			fromyec.Merge(toyec);
			#endregion

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine + "Si suggerisce di allegare fra 'documenti associati' il file con le modalità di determinazione" + Environment.NewLine;
		}
		#endregion

#region Affidamenti
		private void AddAffidamenti(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			ArrayList periodi = new ArrayList();

			foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + ID + "]/Valore[@tipo='Affidamenti']"))
			{
				if (item.Attributes["banca"] != null && !periodi.Contains(item.Attributes["banca"].Value))
				{
					periodi.Add(item.Attributes["banca"].Value);
				}
			}

			Hashtable Header = new Hashtable();
			Hashtable Colonne = new Hashtable();
			Hashtable Lunghezze = new Hashtable();
			Hashtable Tipologia = new Hashtable();

			int index;

			foreach (string periodo in periodi)
			{
				object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Bold = -1;
				p.Range.Text = Environment.NewLine + periodo + Environment.NewLine + Environment.NewLine;

				Header = new Hashtable();
				Colonne = new Hashtable();
				Lunghezze = new Hashtable();
				Tipologia = new Hashtable();

				Header.Add(1, "TipoAffidamento");
				Colonne.Add(1, "tipoaffidamento");
				Lunghezze.Add(1, 120);
				Tipologia.Add(1, TipologiaDato.TipoAffidamento);
				Header.Add(2, "ad inizio esercizio");
				Colonne.Add(2, "inizio");
				Lunghezze.Add(2, 90);
				Tipologia.Add(2, TipologiaDato.Double);
				Header.Add(3, "alla data verifica");
				Colonne.Add(3, "dataverifica");
				Lunghezze.Add(3, 90);
				Tipologia.Add(3, TipologiaDato.Double);
				Header.Add(4, "utilizzo");
				Colonne.Add(4, "utilizzo");
				Lunghezze.Add(4, 90);
				Tipologia.Add(4, TipologiaDato.Double);
				Header.Add(5, "scadenza");
				Colonne.Add(5, "scadenza");
				Lunghezze.Add(5, 90);
				Tipologia.Add(5, TipologiaDato.Stringa);

				wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, Header.Count, ref oMissing, ref oMissing);

				wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
				wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

				CreateColumn(Lunghezze);

				CreateTable(Header);

				index = 0;
				foreach (XmlNode item in nodeData.SelectNodes("/Dati//Dato[@ID=" + ID + "]/Valore[@tipo='Affidamenti'][@banca='" + periodo + "']"))
				{
					if (item.Name == "Valore")
					{
						AddNodoTable(item, index++, Colonne, Tipologia);
					}
				}

				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Text = Environment.NewLine;
			}

			object r2 = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p2 = wrdDoc.Content.Paragraphs.Add(ref r2);
			p2.Range.Bold = -1;
			p2.Range.Text = Environment.NewLine + "RIEPILOGO" + Environment.NewLine + Environment.NewLine;

			List<string> Alias = new List<string>();

			Alias.Add("a");
			Alias.Add("b");
			Alias.Add("c");
			Alias.Add("d");
			Alias.Add("e");
			Alias.Add("f");
			Alias.Add("g");
			Alias.Add("h");
			Alias.Add("i");
			Alias.Add("l");
			Alias.Add("m");
			Alias.Add("n");

			Hashtable htAlias = new Hashtable();
			htAlias.Add("a", "conto corrente");
			htAlias.Add("b", "sbf - riba");
			htAlias.Add("c", "anticipo fatture");
			htAlias.Add("d", "anticipo export");
			htAlias.Add("e", "anticipo import");
			htAlias.Add("f", "chirografario");
			htAlias.Add("g", "mutui ipotecari o similari");
			htAlias.Add("h", "operazioni a termine");
			htAlias.Add("i", "finanza derivata");
			htAlias.Add("l", "garanzie prestate");
			htAlias.Add("m", "altro 1");
			htAlias.Add("n", "altro 2");

			Hashtable htInizio = new Hashtable();
			Hashtable htFine = new Hashtable();
			Hashtable htUtilizzo = new Hashtable();

			foreach (XmlNode tmpnode in nodeData.SelectNodes("/Dati//Dato[@ID='" + ID + "']/Valore[@tipo='Affidamenti']"))
			{
				if (tmpnode.Attributes["tipoaffidamento"] == null)
				{
					continue;
				}

				if (tmpnode.Attributes["tipoaffidamento"].Value == "")
				{
					continue;
				}

				if (!htInizio.Contains(tmpnode.Attributes["tipoaffidamento"].Value))
				{
					htInizio.Add(tmpnode.Attributes["tipoaffidamento"].Value, 0.0);
				}

				if (tmpnode.Attributes["inizio"] != null)
				{
					double value = 0.0;
					double.TryParse(tmpnode.Attributes["inizio"].Value, out value);
					htInizio[tmpnode.Attributes["tipoaffidamento"].Value] = (double)htInizio[tmpnode.Attributes["tipoaffidamento"].Value] + value;
				}

				if (!htFine.Contains(tmpnode.Attributes["tipoaffidamento"].Value))
				{
					htFine.Add(tmpnode.Attributes["tipoaffidamento"].Value, 0.0);
				}

				if (tmpnode.Attributes["dataverifica"] != null)
				{
					double value = 0.0;
					double.TryParse(tmpnode.Attributes["dataverifica"].Value, out value);
					htFine[tmpnode.Attributes["tipoaffidamento"].Value] = (double)htFine[tmpnode.Attributes["tipoaffidamento"].Value] + value;
				}

				if (!htUtilizzo.Contains(tmpnode.Attributes["tipoaffidamento"].Value))
				{
					htUtilizzo.Add(tmpnode.Attributes["tipoaffidamento"].Value, 0.0);
				}

				if (tmpnode.Attributes["utilizzo"] != null)
				{
					double value = 0.0;
					double.TryParse(tmpnode.Attributes["utilizzo"].Value, out value);
					htUtilizzo[tmpnode.Attributes["tipoaffidamento"].Value] = (double)htUtilizzo[tmpnode.Attributes["tipoaffidamento"].Value] + value;
				}
			}

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 6, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(180, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(80, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(80, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[4].SetWidth(80, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Tipo Affidamento";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "ad inizio esercizio";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "alla data verifica";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = "utilizzo";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			foreach (string item in Alias)
			{
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = htAlias[item].ToString();
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

				double valore = 0.0;
				if (htInizio.Contains(item))
				{
					valore = (double)htInizio[item];
				}

				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = valore.ToString();
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

				if (htFine.Contains(item))
				{
					valore = (double)htFine[item];
				}

				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = valore.ToString();
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

				if (htUtilizzo.Contains(item))
				{
					valore = (double)htUtilizzo[item];
				}

				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = valore.ToString();
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
				wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			}

			

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "Totale";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			double somma = 0.0;
			foreach (DictionaryEntry item in htInizio)
			{
				somma += (double)(item.Value);
			}

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = somma.ToString();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			somma = 0.0;
			foreach (DictionaryEntry item in htFine)
			{
				somma += (double)(item.Value);
			}

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = somma.ToString();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			somma = 0.0;
			foreach (DictionaryEntry item in htUtilizzo)
			{
				somma += (double)(item.Value);
			}

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray20;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.Text = somma.ToString();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			r2 = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p2 = wrdDoc.Content.Paragraphs.Add(ref r2);
			p2.Range.Bold = -1;
			p2.Range.Text = Environment.NewLine;
		}
#endregion

#region Bilancio
		private void AddBilancio(XmlNode nodeTree, XmlNode nodeData, string ID, string Titolo)
		{
			string IDInterno = nodeData.Attributes["ID"].Value;

			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

            Excel.Range ExcelRange;

            sheet.Columns[1, Type.Missing].ColumnWidth = 3;
            sheet.Columns[2, Type.Missing].ColumnWidth = 3;
            sheet.Columns[3, Type.Missing].ColumnWidth = 2;
            sheet.Columns[4, Type.Missing].ColumnWidth = 2;
            sheet.Columns[5, Type.Missing].ColumnWidth = 35;
            sheet.Columns[6, Type.Missing].ColumnWidth = 13;
            sheet.Columns[7, Type.Missing].ColumnWidth = 13;
            sheet.Columns[8, Type.Missing].ColumnWidth = 13;

			sheet.Cells[1, 1].Value = "";
			sheet.Cells[1, 2].Value = "";
            sheet.Cells[1, 3].Value = "";
            sheet.Cells[1, 4].Value = "";
            sheet.Cells[1, 5].Value = "";
			sheet.Cells[1, 6].Value = "ESERCIZIO attuale";
            sheet.Cells[1, 7].Value = "ESERCIZIO precedente";
			sheet.Cells[1, 8].Value = "Increm. (decrem.)";

            //formattazione intestazione
            ExcelRange = sheet.get_Range("A1", "H1");
            ExcelRange.Font.Bold = true;
            ExcelRange.Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            ExcelRange.WrapText = true;


			int row = 1;

#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
            string rangeStart, rangeStop;
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata

			foreach (XmlNode nodenodo in nodeData.SelectNodes("/Dati/Dato[@ID='" + IDInterno + "']/Valore[@Titolo='" + Titolo + "']"))
			{
				//if (nodenodo.Attributes["Titolo"] != null && nodenodo.Attributes["Titolo"].Value == Titolo)
				{
					row++;
                    rangeStart = "A" + row.ToString();

					if (nodenodo.Attributes["rigaVuota"] != null)
					{
						continue;
					}

                    //lev1
                    if (nodenodo.Attributes["Codice"] == null)
#pragma warning disable CS0642 // L'istruzione vuota è probabilmente errata
                        ;
#pragma warning restore CS0642 // L'istruzione vuota è probabilmente errata
                    else if (nodenodo.Attributes["paddingCodice"] == null)
                    {
                        sheet.Cells[row, 1].Value = nodenodo.Attributes["Codice"].Value;
                    }
                    else if (nodenodo.Attributes["paddingCodice"].Value == "15")
                    {
                        sheet.Cells[row, 2].Value = nodenodo.Attributes["Codice"].Value;
                        rangeStart = "B" + row.ToString();
                    }
                    else if (nodenodo.Attributes["paddingCodice"].Value == "30")
                    {
                        sheet.Cells[row, 3].Value = nodenodo.Attributes["Codice"].Value;
                        rangeStart = "C" + row.ToString();
                    }
                    else if (nodenodo.Attributes["paddingCodice"].Value == "45")
                    {
                        sheet.Cells[row, 4].Value = nodenodo.Attributes["Codice"].Value;
                        rangeStart = "D" + row.ToString();
                    }



                    //andrea, prima partiva da 2
					sheet.Cells[row, 5].Value = (nodenodo.Attributes["name"] == null) ? " " : nodenodo.Attributes["name"].Value;
					sheet.Cells[row, 5].Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
					sheet.Cells[row, 5].WrapText = true;

                    if (nodenodo.Attributes["boldName"] != null)
                    {
                        sheet.Cells[row, 5].Font.Bold = true;
                    }
                        
                    //unisco celle titolo
                    //rangeStop = "E" + row.ToString();
                    //Excel.Range TitRange = sheet.get_Range(rangeStart, rangeStop);
                    //TitRange.Merge();


					if (nodenodo.Attributes["noData"] == null)
					{
						sheet.Cells[row, 6].Value = (nodenodo.Attributes["EA"] == null) ? " " : ConvertNumber(nodenodo.Attributes["EA"].Value);
						sheet.Cells[row, 6].Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;

						sheet.Cells[row, 7].Value = (nodenodo.Attributes["EP"] == null) ? " " : ConvertNumber(nodenodo.Attributes["EP"].Value);
						sheet.Cells[row, 7].Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;

						sheet.Cells[row, 8].Value = (nodenodo.Attributes["DIFF"] == null) ? " " : ConvertNumber(nodenodo.Attributes["DIFF"].Value);
						sheet.Cells[row, 8].Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;

					}

                    if (nodenodo.Attributes["bg"] != null)
                    {
                        string a, b;
                        a = "F" + row.ToString();
                        b = "H" + row.ToString();
                        Excel.Range TotalRange = sheet.get_Range(a, b);
                        TotalRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);

                        if (nodenodo.Attributes["bg"].Value == "Brown")
                            TotalRange.Font.Bold = true;

                    }

				}

                //andrea
                string theEnd = "H" + row.ToString();
                //formattazione
                ExcelRange = sheet.get_Range("A1", theEnd);
                ExcelRange.Font.Name = "Arial";
                ExcelRange.Font.Size = 9;
                ((Excel.Range)ExcelRange).Style.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                ((Excel.Range)ExcelRange).EntireRow.RowHeight = 12;
                
                //griglia
                ExcelRange = sheet.get_Range("F1", theEnd); //F
                ExcelRange.Borders.Weight = Excel.XlBorderWeight.xlThin;





/*
					sheet.Cells[row, 1].Value = (nodenodo.Attributes["Codice"] == null) ? " " : nodenodo.Attributes["Codice"].Value;
					if (nodenodo.Attributes["paddingCodice"] != null)
					{
						if (nodenodo.Attributes["paddingCodice"].Value == "30")
						{
							sheet.Cells[row, 1].Value = "          " + sheet.Cells[row, 1].Value;
						}
						else
						{
							sheet.Cells[row, 1].Value = "     " + sheet.Cells[row, 1].Value;
						}
					}
					//sheet.Cells[row, 1].LeftPadding = (nodenodo.Attributes["paddingCodice"] == null) ? Convert.ToSingle(0.0) : Convert.ToSingle(nodenodo.Attributes["paddingCodice"].Value);
*/ 

			}


			//Table t = wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 5, WdDefaultTableBehavior.wdWord8TableBehavior, WdAutoFitBehavior.wdAutoFitWindow);

			////t.AllowPageBreaks = false;
			//t.Range.Font.Bold = 0;
			//t.Range.Font.Size = Convert.ToSingle(11.0);

			//t.Columns[1].SetWidth(70, WdRulerStyle.wdAdjustNone);
			//t.Columns[2].SetWidth(200, WdRulerStyle.wdAdjustNone);
			//t.Columns[3].SetWidth(80, WdRulerStyle.wdAdjustNone);
			//t.Columns[4].SetWidth(80, WdRulerStyle.wdAdjustNone);
			//t.Columns[5].SetWidth(80, WdRulerStyle.wdAdjustNone);

			//Row row = t.Rows.Add();
			//row.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			//row.Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			//row.Cells[3].Range.Text = "ESERCIZIO attuale";
			//row.Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			//row.Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			//row.Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			//row.Cells[4].Range.Text = "ESERCIZIO precedente";
			//row.Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			//row.Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			//row.Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray30;
			//row.Cells[5].Range.Text = "Increm. (decrem.)";
			//row.Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			//row.Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			//foreach (XmlNode nodenodo in nodeData.SelectNodes("/Dati/Dato[@ID='" + IDInterno + "']/Valore[@Titolo='" + Titolo + "']"))
			//{
			//    //if (nodenodo.Attributes["Titolo"] != null && nodenodo.Attributes["Titolo"].Value == Titolo)
			//    {
			//        row = t.Rows.Add();
			//        row.Height = 9;

			//        if (nodenodo.Attributes["rigaVuota"] != null)
			//        {
			//            row.Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			//            row.Cells[1].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
			//            row.Cells[1].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
			//            row.Cells[1].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;

			//            row.Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			//            row.Cells[2].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
			//            row.Cells[2].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
			//            row.Cells[2].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;

			//            row.Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			//            row.Cells[3].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
			//            row.Cells[3].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
			//            row.Cells[3].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;

			//            row.Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			//            row.Cells[4].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
			//            row.Cells[4].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
			//            row.Cells[4].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;

			//            row.Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			//            row.Cells[5].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
			//            row.Cells[5].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
			//            row.Cells[5].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;
			//            continue;
			//        }

			//        row.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			//        row.Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			//        row.Cells[1].Range.Text = (nodenodo.Attributes["Codice"] == null) ? "" : nodenodo.Attributes["Codice"].Value;
			//        row.Cells[1].LeftPadding = (nodenodo.Attributes["paddingCodice"] == null) ? Convert.ToSingle(0.0) : Convert.ToSingle(nodenodo.Attributes["paddingCodice"].Value);
			//        row.Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			//        row.Cells[1].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;

			//        row.Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			//        row.Cells[2].Range.Text = (nodenodo.Attributes["name"] == null) ? "" : nodenodo.Attributes["name"].Value;
			//        row.Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			//        row.Cells[2].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;

			//        if (nodenodo.Attributes["noData"] == null)
			//        {
			//            WdColor colore = (nodenodo.Attributes["bg"] == null) ? WdColor.wdColorGray10 : ((nodenodo.Attributes["bg"].Value == "Green") ? WdColor.wdColorGray20 : WdColor.wdColorGray30);
			//            row.Cells[3].Shading.BackgroundPatternColor = colore;
			//            row.Cells[3].Range.Text = (nodenodo.Attributes["EA"] == null) ? "" : ConvertNumber(nodenodo.Attributes["EA"].Value);
			//            row.Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			//            row.Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			//            row.Cells[4].Shading.BackgroundPatternColor = colore;
			//            row.Cells[4].Range.Text = (nodenodo.Attributes["EP"] == null) ? "" : ConvertNumber(nodenodo.Attributes["EP"].Value);
			//            row.Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			//            row.Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			//            row.Cells[5].Shading.BackgroundPatternColor = colore;
			//            row.Cells[5].Range.Text = (nodenodo.Attributes["DIFF"] == null) ? "" : ConvertNumber(nodenodo.Attributes["DIFF"].Value);
			//            row.Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			//            row.Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			//        }
			//        else
			//        {
			//            row.Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			//            row.Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			//            row.Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			//            row.Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			//            row.Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			//            row.Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;
			//        }
			//    }
			//}


			Microsoft.Office.Interop.Excel.Range excelRange = sheet.UsedRange;

			excelRange.Copy();

			p.Range.PasteExcelTable(false, false, true);

			excelRange.Delete();

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;


		}
		#endregion	

#region Bilancio Riclassificato
		private void AddBilancioRiclassificato(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			string IDInterno = nodeData.Attributes["ID"].Value;

			ArrayList al = new ArrayList();
			al.Add("ATTIVO");
			al.Add("PASSIVO");
			al.Add("CONTO ECONOMICO");
			al.Add("SINTESI");

			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;

			foreach (string item in al)
			{
				if (nodeData.SelectNodes("/Dati/Dato[@ID='" + IDInterno + "']/Valore[@Titolo='" + item + "']").Count <= 0)
				{
					continue;
				}

				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Text = item + Environment.NewLine;

				Table t = wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 6, WdDefaultTableBehavior.wdWord8TableBehavior, WdAutoFitBehavior.wdAutoFitWindow);

				//t.AllowPageBreaks = false;
				t.Range.Font.Bold = 0;
				t.Range.Font.Size = Convert.ToSingle(11.0);

				t.Columns[1].SetWidth(150, WdRulerStyle.wdAdjustNone);
				t.Columns[2].SetWidth(80, WdRulerStyle.wdAdjustNone);
				t.Columns[3].SetWidth(60, WdRulerStyle.wdAdjustNone);
				t.Columns[4].SetWidth(80, WdRulerStyle.wdAdjustNone);
				t.Columns[5].SetWidth(60, WdRulerStyle.wdAdjustNone);
				t.Columns[6].SetWidth(80, WdRulerStyle.wdAdjustNone);

				Row row = t.Rows.Add();
				row.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

				row.Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
				row.Cells[1].Range.Text = item;
				row.Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

				row.Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
				row.Cells[2].Range.Text = "ESERCIZIO attuale";
				row.Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

				row.Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
				row.Cells[3].Range.Text = "%";
				row.Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

				row.Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
				row.Cells[4].Range.Text = "ESERCIZIO precedente";
				row.Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

				row.Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
				row.Cells[5].Range.Text = "%";
				row.Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

				row.Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
				row.Cells[6].Range.Text = "variazione";
				row.Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;				

				foreach (XmlNode nodenodo in nodeData.SelectNodes("/Dati/Dato[@ID='" + IDInterno + "']/Valore[@Titolo='" + item + "']"))
				{
					//if (nodenodo.Attributes["Titolo"] != null && nodenodo.Attributes["Titolo"].Value == Titolo)
					{
						row = t.Rows.Add();
						row.Height = 9;

						if (nodenodo.Attributes["tipo"].Value  == "titolo")
						{
							row.Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
							row.Cells[1].Range.Text = (nodenodo.Attributes["name"] == null) ? "" : nodenodo.Attributes["name"].Value;
							row.Cells[1].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
							row.Cells[1].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
							row.Cells[1].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;

							row.Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
							row.Cells[2].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
							row.Cells[2].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
							row.Cells[2].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;

							row.Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
							row.Cells[3].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
							row.Cells[3].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
							row.Cells[3].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;

							row.Cells[4].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
							row.Cells[4].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
							row.Cells[4].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
							row.Cells[4].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;

							row.Cells[5].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
							row.Cells[5].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
							row.Cells[5].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
							row.Cells[5].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;

							row.Cells[6].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
							row.Cells[6].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
							row.Cells[6].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
							row.Cells[6].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;
							continue;
						}

						row.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

						row.Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
						row.Cells[1].Range.Text = (nodenodo.Attributes["name"] == null) ? "" : nodenodo.Attributes["name"].Value;
						row.Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
						row.Cells[1].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;

						row.Cells[2].Shading.BackgroundPatternColor = (nodenodo.Attributes["tipo"].Value != "totale" && nodenodo.Attributes["tipo"].Value != "semitotale") ? WdColor.wdColorGray10 : WdColor.wdColorGray30;
						row.Cells[2].Range.Text = (nodenodo.Attributes["EA"] == null) ? "" : ConvertNumber(nodenodo.Attributes["EA"].Value);
						row.Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
						row.Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

						if (item != "SINTESI")
						{
							row.Cells[3].Shading.BackgroundPatternColor = (nodenodo.Attributes["tipo"].Value != "totale" && nodenodo.Attributes["tipo"].Value != "semitotale") ? WdColor.wdColorGray10 : WdColor.wdColorGray30;
							row.Cells[3].Range.Text = (nodenodo.Attributes["PERCENT_EA"] == null) ? "" : nodenodo.Attributes["PERCENT_EA"].Value;
							row.Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
							row.Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
						}

						row.Cells[4].Shading.BackgroundPatternColor = (nodenodo.Attributes["tipo"].Value != "totale" && nodenodo.Attributes["tipo"].Value != "semitotale") ? WdColor.wdColorGray20 : WdColor.wdColorGray30;
						row.Cells[4].Range.Text = (nodenodo.Attributes["EP"] == null) ? "" : ConvertNumber(nodenodo.Attributes["EP"].Value);
						row.Cells[4].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
						row.Cells[4].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

						if (item != "SINTESI")
						{
							row.Cells[5].Shading.BackgroundPatternColor = (nodenodo.Attributes["tipo"].Value != "totale" && nodenodo.Attributes["tipo"].Value != "semitotale") ? WdColor.wdColorGray20 : WdColor.wdColorGray30;
							row.Cells[5].Range.Text = (nodenodo.Attributes["PERCENT_EP"] == null) ? "" : nodenodo.Attributes["PERCENT_EP"].Value;
							row.Cells[5].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
							row.Cells[5].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

							row.Cells[6].Shading.BackgroundPatternColor = (nodenodo.Attributes["tipo"].Value != "totale" && nodenodo.Attributes["tipo"].Value != "semitotale") ? WdColor.wdColorWhite : WdColor.wdColorGray30;
							row.Cells[6].Range.Text = (nodenodo.Attributes["DIFF"] == null) ? "" : ConvertNumber(nodenodo.Attributes["DIFF"].Value);
							row.Cells[6].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
							row.Cells[6].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
						}
					}
				}

				r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
				p = wrdDoc.Content.Paragraphs.Add(ref r);
				p.Range.Text = Environment.NewLine;
			}			
		}
		#endregion			
				
#region Bilancio Indici
		private void AddBilancioIndici(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			XmlNode nodenodo = nodeData.SelectSingleNode("/Dati//Dato[@ID=" + ID + "]");

			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine + "INDICI DI STRUTTURA" + Environment.NewLine;

			#region INDICI DI STRUTTURA

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 3, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(240, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(100, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(100, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "ESERCIZIO ATTUALE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "ESERCIZIO PRECEDENTE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "INDICE DI LIQUIDITA' SECCA\r\natt. corr / pass. corr.";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_1"] == null) ? "" : nodenodo.Attributes["txtEA_1"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_1"] == null) ? "" : nodenodo.Attributes["txtEP_1"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "INDICE DI LIQUIDITA' CORRENTE\r\nliquidità + magazz. / pass. corr";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_2"] == null) ? "" : nodenodo.Attributes["txtEA_2"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_2"] == null) ? "" : nodenodo.Attributes["txtEP_2"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "INDICE DI INDEBITAMENTO\r\nmezzi terzi / mezzi propri";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_3"] == null) ? "" : nodenodo.Attributes["txtEA_3"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_3"] == null) ? "" : nodenodo.Attributes["txtEP_3"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "INDIPENDENZA FINANZIARIA\r\nmezzi propri / mezzi terzi";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_4"] == null) ? "" : nodenodo.Attributes["txtEA_4"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_4"] == null) ? "" : nodenodo.Attributes["txtEP_4"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "INDICE DI STRUTTURA\r\npatr. netto / tot. immobilizz.";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_5"] == null) ? "" : nodenodo.Attributes["txtEA_5"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_5"] == null) ? "" : nodenodo.Attributes["txtEP_5"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "INDICE DI COPERTURA\r\npatr. netto + deb.m/l / tot. immobilizz.";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_6"] == null) ? "" : nodenodo.Attributes["txtEA_6"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_6"] == null) ? "" : nodenodo.Attributes["txtEP_6"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			#endregion

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine + "INDICI DI REDDITIVITA'" + Environment.NewLine;

			#region INDICI DI REDDITIVITA'

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 3, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(240, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(100, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(100, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "ESERCIZIO ATTUALE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "ESERCIZIO PRECEDENTE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "R.O.I.\r\nutile operativo / capitale investito";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_7"] == null) ? "" : nodenodo.Attributes["txtEA_7"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_7"] == null) ? "" : nodenodo.Attributes["txtEP_7"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "R.O.E.\r\nutile netto / patr. netto";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_8"] == null) ? "" : nodenodo.Attributes["txtEA_8"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_8"] == null) ? "" : nodenodo.Attributes["txtEP_8"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "R.O.S.\r\nutile operativo / vendite nette";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_9"] == null) ? "" : nodenodo.Attributes["txtEA_9"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_9"] == null) ? "" : nodenodo.Attributes["txtEP_9"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "INDICE TENSIONE FINANZIARIA\r\nrisultato operativo / gestione fin. netta";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_10"] == null) ? "" : nodenodo.Attributes["txtEA_10"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_10"] == null) ? "" : nodenodo.Attributes["txtEP_10"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			#endregion

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine + "INDICI DI ROTAZIONE" + Environment.NewLine;

			#region INDICI DI ROTAZIONE

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 3, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(240, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(100, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(100, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "ESERCIZIO ATTUALE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "ESERCIZIO PRECEDENTE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "ROTAZIONE CREDITI (giorni)\r\ncredito / vendite * 365";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_11"] == null) ? "" : nodenodo.Attributes["txtEA_11"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_11"] == null) ? "" : nodenodo.Attributes["txtEP_11"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "ROTAZ. FORNITORI (giorni)\r\nfornitori / acquisti *365";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_12"] == null) ? "" : nodenodo.Attributes["txtEA_12"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_12"] == null) ? "" : nodenodo.Attributes["txtEP_12"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "ROTAZ. MAGAZZINO (giorni)\r\nrimanenza / vendite * 365";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_13"] == null) ? "" : nodenodo.Attributes["txtEA_13"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_13"] == null) ? "" : nodenodo.Attributes["txtEP_13"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
			#endregion

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}
		#endregion		

#region Bilancio Abbreviato Indici
		private void AddBilancioAbbreviatoIndici(XmlNode nodeTree, XmlNode nodeData, string ID)
		{
			XmlNode nodenodo = nodeData.SelectSingleNode("/Dati//Dato[@ID=" + ID + "]");

			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine + "INDICI DI STRUTTURA" + Environment.NewLine;

			#region INDICI DI STRUTTURA

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 3, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(240, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(100, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(100, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "ESERCIZIO ATTUALE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "ESERCIZIO PRECEDENTE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "INDICE DI LIQUIDITA' SECCA\r\natt. corr / pass. corr.";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_1"] == null) ? "" : nodenodo.Attributes["txtEA_1"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_1"] == null) ? "" : nodenodo.Attributes["txtEP_1"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "INDICE DI LIQUIDITA' CORRENTE\r\nliquidità + magazz. / pass. corr";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_2"] == null) ? "" : nodenodo.Attributes["txtEA_2"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_2"] == null) ? "" : nodenodo.Attributes["txtEP_2"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "INDICE DI INDEBITAMENTO\r\nmezzi terzi / mezzi propri";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_3"] == null) ? "" : nodenodo.Attributes["txtEA_3"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_3"] == null) ? "" : nodenodo.Attributes["txtEP_3"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "INDIPENDENZA FINANZIARIA\r\nmezzi propri / mezzi terzi";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_4"] == null) ? "" : nodenodo.Attributes["txtEA_4"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_4"] == null) ? "" : nodenodo.Attributes["txtEP_4"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "INDICE DI STRUTTURA\r\npatr. netto / tot. immobilizz.";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_5"] == null) ? "" : nodenodo.Attributes["txtEA_5"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_5"] == null) ? "" : nodenodo.Attributes["txtEP_5"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "INDICE DI COPERTURA\r\npatr. netto + deb.m/l / tot. immobilizz.";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_6"] == null) ? "" : nodenodo.Attributes["txtEA_6"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_6"] == null) ? "" : nodenodo.Attributes["txtEP_6"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			#endregion

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine + "INDICI DI REDDITIVITA'" + Environment.NewLine;

			#region INDICI DI REDDITIVITA'

			wrdDoc.Tables.Add(wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range, 1, 3, ref oMissing, ref oMissing);

			wrdDoc.Tables[wrdDoc.Tables.Count].AllowPageBreaks = false;
			wrdDoc.Tables[wrdDoc.Tables.Count].Range.Font.Bold = 0;

			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[1].SetWidth(240, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[2].SetWidth(100, WdRulerStyle.wdAdjustNone);
			wrdDoc.Tables[wrdDoc.Tables.Count].Columns[3].SetWidth(100, WdRulerStyle.wdAdjustNone);

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = "ESERCIZIO ATTUALE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorGray10;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = "ESERCIZIO PRECEDENTE";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "R.O.I.\r\nutile operativo / capitale investito";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_7"] == null) ? "" : nodenodo.Attributes["txtEA_7"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_7"] == null) ? "" : nodenodo.Attributes["txtEP_7"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "R.O.E.\r\nutile netto / patr. netto";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_8"] == null) ? "" : nodenodo.Attributes["txtEA_8"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_8"] == null) ? "" : nodenodo.Attributes["txtEP_8"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "R.O.S.\r\nutile operativo / vendite nette";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_9"] == null) ? "" : nodenodo.Attributes["txtEA_9"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_9"] == null) ? "" : nodenodo.Attributes["txtEP_9"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Add();
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.Text = "INDICE TENSIONE FINANZIARIA\r\nrisultato operativo / gestione fin. netta";
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[1].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleNone;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.Text = (nodenodo.Attributes["txtEA_10"] == null) ? "" : nodenodo.Attributes["txtEA_10"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[2].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Shading.BackgroundPatternColor = WdColor.wdColorWhite;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.Text = (nodenodo.Attributes["txtEP_10"] == null) ? "" : nodenodo.Attributes["txtEP_10"].Value;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
			wrdDoc.Tables[wrdDoc.Tables.Count].Rows[wrdDoc.Tables[wrdDoc.Tables.Count].Rows.Count].Cells[3].Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;

			#endregion

			r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Text = Environment.NewLine;
		}
		#endregion	

#region Funzioni varie
		public void LastParagraph(Hashtable dati)
		{
			object r = wrdDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
			Paragraph p = wrdDoc.Content.Paragraphs.Add(ref r);
			p.Range.Font.Bold = 0;
			p.Range.Text = "\f\r";

			string riga = "";

			if (dati.Contains("Fine"))
			{
				riga += "Alle ore " + dati["Fine"].ToString() + " ";
			}

			p.Range.Font.Bold = 0;
			p.Range.Text += riga + "la verifica del Collegio Sindacale viene conclusa, previa stesura e sottoscrizione del presente verbale." + Environment.NewLine;

			if (dati.Contains("Presidente") && dati["Presidente"].ToString() != "")
			{
				p.Range.Font.Bold = 0;
				p.Range.Text += "PRESIDENTE\t\t\t" + dati["Presidente"].ToString();
			}

			if (dati.Contains("Sindaco1") && dati["Sindaco1"].ToString() != "")
			{
				p.Range.Font.Bold = 0;
				p.Range.Text += "SINDACO EFFETTIVO\t\t" + dati["Sindaco1"].ToString();
			}

			if (dati.Contains("Sindaco2") && dati["Sindaco2"].ToString() != "")
			{
				p.Range.Font.Bold = 0;
				p.Range.Text += "SINDACO EFFETTIVO\t\t" + dati["Sindaco2"].ToString();
			}
		}

		public void Save()
		{
            wrdDoc.PageSetup.PaperSize = WdPaperSize.wdPaperA4;
            wrdDoc.SaveAs(filename + ".doc");

            wrdDoc.Close(ref oFalse, ref oMissing, ref oMissing);
            wrdApp.Quit();

            System.Diagnostics.Process.Start(filename + ".doc");
		}

		public void SavePDF()
		{
			object fileFormat = WdSaveFormat.wdFormatPDF;

            wrdDoc.PageSetup.PaperSize = WdPaperSize.wdPaperA4;
            wrdDoc.SaveAs(filename + ".pdf", fileFormat);

            wrdDoc.Close(ref oFalse, ref oMissing, ref oMissing);
            wrdApp.Quit();

			System.Diagnostics.Process.Start(filename + ".pdf");
		}

		public void Close()
		{
            //wrdDoc.Close(ref oFalse, ref oMissing, ref oMissing);
            //wrdApp.Quit();

			workBook.Close(false);

			xlsApp.Quit();
		}
#endregion
	}
}
