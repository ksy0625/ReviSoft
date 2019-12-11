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
    public partial class ucRelazioneErroriRilevati : UserControl
    {
        public int id;
    
        private DataTable dati = null;
        //private string _ID = "-1";
		private bool firsttime = true;
        ArrayList dynamicRDB= new ArrayList();
        Hashtable HTNode = new Hashtable();

        private string down = "./Images/icone/navigate_down.png";
        private string left = "./Images/icone/navigate_left.png";

		private bool _ReadOnly = false;

        private Dictionary<int, DataRow> lista = new Dictionary<int, DataRow>();

        public ucRelazioneErroriRilevati()
        {
            InitializeComponent();
        }
        
        public void FocusNow()
        {
        }

        public bool ReadOnly 
        {
            set
            {
				_ReadOnly = value;
            }
        }

        public void Load(string _ID, string FileData, string IDCliente,string IDSessione, string IDTree )
        {
		
            id = int.Parse(_ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
            dati= cBusinessObjects.GetData(id, typeof(RelazioneErroriRilevati));
            bool dativuoti = false;
           if(dati.Rows.Count==0)
           {
                dativuoti = true;
           }

            MasterFile mf = MasterFile.Create();

            string FileBilancio = "";
            string idsessionebilancio= "";
            if ( IDTree == "22" )
            {
                idsessionebilancio=cBusinessObjects.CercaSessione("RelazioneV", "Bilancio", IDSessione, cBusinessObjects.idcliente);
            }

            if ( IDTree == "21" )
            {
                idsessionebilancio=cBusinessObjects.CercaSessione("RelazioneB", "Bilancio", IDSessione, cBusinessObjects.idcliente);

            }

            if (IDTree == "32")
            {
                idsessionebilancio=cBusinessObjects.CercaSessione("RelazioneVC", "Bilancio", IDSessione, cBusinessObjects.idcliente);

            }

            if (IDTree == "31")
            {
                idsessionebilancio=cBusinessObjects.CercaSessione("RelazioneBC", "Bilancio", IDSessione, cBusinessObjects.idcliente);

            }

            if ( IDTree == "23" )
            {
                idsessionebilancio=cBusinessObjects.CercaSessione("RelazioneBV", "Bilancio", IDSessione, cBusinessObjects.idcliente);

            }

            if( IDTree == "19")
            {
                FileBilancio = mf.GetBilancioAssociatoFromConclusioniFile(FileData);
                idsessionebilancio=cBusinessObjects.CercaSessione("Conclusione", "Bilancio", IDSessione, cBusinessObjects.idcliente);

            }

           

            RevisoftApplication.XmlManager xt = new XmlManager();
            xt.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
            XmlDataProvider TreeXmlProvider = new XmlDataProvider();
            TreeXmlProvider.Document = xt.LoadEncodedFile( App.AppTemplateTreeBilancio );

            int rowattuale = 0;
           
            DataTable datierr = cBusinessObjects.GetData(-1, typeof(Excel_ErroriRilevati), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);
            DataTable datierr_note = cBusinessObjects.GetData(-1, typeof(Excel_ErroriRilevati_Note), cBusinessObjects.idcliente, int.Parse(idsessionebilancio), 4);
            
            int chiave = 1;
            foreach(DataRow item in datierr.Rows)
            {
                  if ( item["name"].ToString() == "Totale" ||  item["corretto"].ToString() != "False" )
                    {
                        continue;
                    }
                 lista.Add( chiave, item );
                
                  if(dativuoti)
                  {
                    DataRow tmp = dati.Rows.Add();
                    tmp["ID_SCHEDA"] = id;
                    tmp["ID_CLIENTE"] = int.Parse(IDCliente);
                    tmp["ID_SESSIONE"] = int.Parse(IDSessione);
                    tmp["ID"] = chiave;
                     foreach(DataRow itemnote in datierr_note.Rows)
                    {
                      if(itemnote["rif"].ToString()==item["rif"].ToString() && itemnote["ID_SCHEDA"].ToString()==item["ID_SCHEDA"].ToString())
                      tmp["testo"] = itemnote["name"].ToString();
                    }
                  }
                chiave++;
            }

         
        

            if ( lista.Count == 0)
            {
                TextBlock txtblk = new TextBlock();
                txtblk.Text = "Nessun errore rilevato presente.";
                gg.Children.Add( txtblk );
                return;
            }

            foreach (KeyValuePair<int, DataRow> itemD in lista.OrderBy(key => key.Key))
            {

              
                DataRow item = itemD.Value;

                XmlNode tnode = TreeXmlProvider.Document.SelectSingleNode( "/Tree//Node[@ID=" + + cBusinessObjects.Gest_ID_SCHEDA(item["ID_SCHEDA"].ToString(),4)  + "]" );

                Border b = new Border();
                b.CornerRadius = new CornerRadius( 5.0 );
                b.BorderBrush = Brushes.LightGray;
                b.BorderThickness = new Thickness( 1.0 );
                b.Padding = new Thickness( 4.0 );
                b.Margin = new Thickness( 4.0 );

                Grid g = new Grid();

                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength( 15.0 );
                g.ColumnDefinitions.Add( cd );

                cd = new ColumnDefinition();
                cd.Width = GridLength.Auto;
                g.ColumnDefinitions.Add( cd );

                g.RowDefinitions.Add( new RowDefinition() );
                g.RowDefinitions.Add( new RowDefinition() );

                Image i = new Image();
                i.SetValue( Grid.RowProperty, 0 );
                i.SetValue( Grid.ColumnProperty, 0 );

                var uriSource = new Uri( left , UriKind.Relative );
                i.Source = new BitmapImage( uriSource );
                i.Height = 10.0;
                i.Width = 10.0;
                i.MouseLeftButtonDown += new MouseButtonEventHandler( Image_MouseLeftButtonDown );

                g.Children.Add( i );

                TextBlock tb = new TextBlock();
                tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                tb.Text = tnode.ParentNode.Attributes["Titolo"].Value;// tnode.ParentNode.Attributes["Codice"].Value + " - " + tnode.ParentNode.Attributes["Titolo"].Value + " - Rif. " + item.Attributes["rif"].Value;
                foreach(DataRow dd in dati.Rows)
                {
                   if(dd["ID"].ToString()==itemD.Key.ToString())
                        {
                        dd["titolo"]=tb.Text;
                        }
                }
          
                tb.FontSize = 13;
                tb.FontWeight = FontWeights.Bold;
                tb.Margin = new Thickness( 5.0 );
                tb.Foreground = Brushes.Gray;

                tb.SetValue( Grid.RowProperty, 0 );
                tb.SetValue( Grid.ColumnProperty, 1 );

                g.Children.Add( tb );

                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Vertical;

                CheckBox chk = new CheckBox();
                chk.Name = "chkInserireRelazione_" + itemD.Key;

                if ( _ReadOnly == true )
                {
                    chk.PreviewKeyDown += obj_PreviewKeyDown;
                    chk.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDown;
                }

                this.RegisterName( chk.Name, chk );
              
                   
                foreach(DataRow dd in dati.Rows)
                {
                   if(dd["ID"].ToString()==itemD.Key.ToString())
                       {
                       if ( dd["chkInserireRelazione"].ToString() != "" )
                            {
                                chk.IsChecked = true;
                            }
                            else
                            {
                                chk.IsChecked = false;
                            }
                       }
                }
                chk.Content = "Inserire questo Errore Rilevato in Relazione";
                sp.Children.Add( chk );

#region Tabella Rettifica
                Grid gRettifica = new Grid();
                gRettifica.Margin = new Thickness( 0, 20, 0, 0 );
                //Importo Rettifica
                cd = new ColumnDefinition();
                gRettifica.ColumnDefinitions.Add( cd );
                //Già su PN Es.Prec
                cd = new ColumnDefinition();
                gRettifica.ColumnDefinitions.Add( cd );
                //Effetto su PN attuale: importo lordo
                cd = new ColumnDefinition();
                gRettifica.ColumnDefinitions.Add( cd );
                //Effetto su PN attuale: effetto fiscale
                cd = new ColumnDefinition();
                gRettifica.ColumnDefinitions.Add(cd);
                //Effetto su CE attuale: importo lordo
                cd = new ColumnDefinition();
                gRettifica.ColumnDefinitions.Add(cd);
                //Effetto su CE attuale: effetto fiscale
                cd = new ColumnDefinition();
                gRettifica.ColumnDefinitions.Add(cd);

                //titoli
                RowDefinition rd = new RowDefinition();
                gRettifica.RowDefinitions.Add(rd);

                tb = new TextBlock();
                tb.TextAlignment = TextAlignment.Center;
                tb.FontWeight = FontWeights.Bold;
                tb.Text = "";
                tb.SetValue(Grid.RowProperty, 0);
                tb.SetValue(Grid.ColumnProperty, 0);
                gRettifica.Children.Add(tb);

                tb = new TextBlock();
                tb.TextAlignment = TextAlignment.Center;
                tb.FontWeight = FontWeights.Bold;
                tb.Text = "";
                tb.SetValue(Grid.RowProperty, 0);
                tb.SetValue(Grid.ColumnProperty, 1);
                gRettifica.Children.Add(tb);

                tb = new TextBlock();
                tb.TextAlignment = TextAlignment.Center;
                tb.FontWeight = FontWeights.Bold;
                tb.Text = "Effetto su PN attuale";
                tb.SetValue(Grid.RowProperty, 0);
                tb.SetValue(Grid.ColumnProperty, 2);
                tb.SetValue(Grid.ColumnSpanProperty, 2);
                gRettifica.Children.Add(tb);

                tb = new TextBlock();
                tb.TextAlignment = TextAlignment.Center;
                tb.FontWeight = FontWeights.Bold;
                tb.Text = "Effetto su CE attuale";
                tb.SetValue(Grid.RowProperty, 0);
                tb.SetValue(Grid.ColumnProperty, 4);
                tb.SetValue(Grid.ColumnSpanProperty, 2);
                gRettifica.Children.Add(tb);
                
                rd = new RowDefinition();
                gRettifica.RowDefinitions.Add( rd );

                tb = new TextBlock();
                tb.TextAlignment = TextAlignment.Center;
                tb.FontWeight = FontWeights.Bold;
                tb.Text = "Importo Rettifica";
                tb.SetValue( Grid.RowProperty, 1 );
                tb.SetValue( Grid.ColumnProperty, 0 );
                gRettifica.Children.Add( tb );

                tb = new TextBlock();
                tb.TextAlignment = TextAlignment.Center;
                tb.FontWeight = FontWeights.Bold;
                tb.Text = "Già su PN Es.Prec.";
                tb.SetValue( Grid.RowProperty, 1 );
                tb.SetValue( Grid.ColumnProperty, 1 );
                gRettifica.Children.Add( tb );

                tb = new TextBlock();
                tb.TextAlignment = TextAlignment.Center;
                tb.FontWeight = FontWeights.Bold;
                tb.Text = "Importo Lordo";
                tb.SetValue( Grid.RowProperty, 1 );
                tb.SetValue( Grid.ColumnProperty, 2 );
                gRettifica.Children.Add( tb );

                tb = new TextBlock();
                tb.TextAlignment = TextAlignment.Center;
                tb.FontWeight = FontWeights.Bold;
                tb.Text = "Effetto Fiscale";
                tb.SetValue(Grid.RowProperty, 1);
                tb.SetValue(Grid.ColumnProperty, 3);
                gRettifica.Children.Add(tb);

                tb = new TextBlock();
                tb.TextAlignment = TextAlignment.Center;
                tb.FontWeight = FontWeights.Bold;
                tb.Text = "Importo Lordo";
                tb.SetValue( Grid.RowProperty, 1 );
                tb.SetValue( Grid.ColumnProperty, 4 );
                gRettifica.Children.Add( tb );

                tb = new TextBlock();
                tb.TextAlignment = TextAlignment.Center;
                tb.FontWeight = FontWeights.Bold;
                tb.Text = "Effetto Fiscale";
                tb.SetValue( Grid.RowProperty, 1 );
                tb.SetValue( Grid.ColumnProperty, 5 );
                gRettifica.Children.Add( tb );

                //valori
                rd = new RowDefinition();
                gRettifica.RowDefinitions.Add( rd );

                Border bordertb = new Border();
                bordertb.BorderBrush = Brushes.Black;
                bordertb.BorderThickness = new Thickness( 1.0 );
                bordertb.Padding = new Thickness( 2.0 );

                tb = new TextBlock();
                tb.Text = ConvertNumberNoDecimal(item["importo"].ToString());
                tb.TextAlignment = TextAlignment.Right;
                string tmpimporto= tb.Text;
             
                bordertb.SetValue( Grid.RowProperty, 2 );
                bordertb.SetValue( Grid.ColumnProperty, 0 );
                bordertb.Child = tb;

                gRettifica.Children.Add( bordertb );

                bordertb = new Border();
                bordertb.BorderBrush = Brushes.Black;
                bordertb.BorderThickness = new Thickness( 0.0, 1.0, 1.0, 1.0 );
                bordertb.Padding = new Thickness( 2.0 );

                tb = new TextBlock();
                tb.Text = ConvertNumberNoDecimal(((tmpimporto == "")? "0" : item["importoAP"].ToString()));
                tb.TextAlignment = TextAlignment.Right;

             
                bordertb.SetValue( Grid.RowProperty, 2 );
                bordertb.SetValue( Grid.ColumnProperty, 1 );
                bordertb.Child = tb;

                gRettifica.Children.Add( bordertb );

                bordertb = new Border();
                bordertb.BorderBrush = Brushes.Black;
                bordertb.BorderThickness = new Thickness( 0.0, 1.0, 1.0, 1.0 );
                bordertb.Padding = new Thickness( 2.0 );

                tb = new TextBlock();
                tb.Text = ConvertNumberNoDecimal(((item["suPNattuale"].ToString() == null)? "0" : item["suPNattuale"].ToString()));
                tb.TextAlignment = TextAlignment.Right;


                bordertb.SetValue( Grid.RowProperty, 2 );
                bordertb.SetValue( Grid.ColumnProperty, 2 );
                bordertb.Child = tb;

                gRettifica.Children.Add( bordertb );

                bordertb = new Border();
                bordertb.BorderBrush = Brushes.Black;
                bordertb.BorderThickness = new Thickness(0.0, 1.0, 1.0, 1.0);
                bordertb.Padding = new Thickness(2.0);

                tb = new TextBlock();
                tb.Text = ConvertNumberNoDecimal(((item["impattofiscalePN"].ToString() == "") ? "0" : item["impattofiscalePN"].ToString()));
                tb.TextAlignment = TextAlignment.Right;

             

                bordertb.SetValue(Grid.RowProperty, 2);
                bordertb.SetValue(Grid.ColumnProperty, 3);
                bordertb.Child = tb;

                gRettifica.Children.Add(bordertb);
                
                bordertb = new Border();
                bordertb.BorderBrush = Brushes.Black;
                bordertb.BorderThickness = new Thickness( 0.0, 1.0, 1.0, 1.0 );
                bordertb.Padding = new Thickness( 2.0 );

                tb = new TextBlock();
                tb.Text = ConvertNumberNoDecimal(((item["suutileattuale"].ToString() == "")? "0" : item["suutileattuale"].ToString()));
                tb.TextAlignment = TextAlignment.Right;

           

                bordertb.SetValue( Grid.RowProperty, 2 );
                bordertb.SetValue( Grid.ColumnProperty, 4 );
                bordertb.Child = tb;

                gRettifica.Children.Add( bordertb );

                bordertb = new Border();
                bordertb.BorderBrush = Brushes.Black;
                bordertb.BorderThickness = new Thickness( 0.0, 1.0, 1.0, 1.0 );
                bordertb.Padding = new Thickness( 2.0 );

                tb = new TextBlock();
                tb.Text = ConvertNumberNoDecimal(((item["impattofiscale"].ToString() == "")? "0" : item["impattofiscale"].ToString()));
                tb.TextAlignment = TextAlignment.Right;

             
                bordertb.SetValue( Grid.RowProperty, 2 );
                bordertb.SetValue( Grid.ColumnProperty, 5 );
                bordertb.Child = tb;

                gRettifica.Children.Add( bordertb );

                sp.Children.Add( gRettifica );
#endregion

                #region Chk
                tb = new TextBlock();
                tb.Margin = new Thickness( 0, 20, 0, 0 );
                tb.Text = "Il rilievo costituisce deviazione per:";			
				sp.Children.Add(tb);
							
                chk = new CheckBox();
                chk.Name = "chk1_" + itemD.Key;

                if ( _ReadOnly == true )
                {
                    chk.PreviewKeyDown += obj_PreviewKeyDown;
                    chk.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDown;
                }

                this.RegisterName( chk.Name, chk );
                foreach(DataRow dd in dati.Rows)
                {
                   if(dd["ID"].ToString()==itemD.Key.ToString())
                       {
                       if(dd["chk1"].ToString()!= "")
                            {
                                chk.IsChecked = true;
                            }
                            else
                            {
                                chk.IsChecked = false;
                            }
                       }
                }
            
                chk.Content = "norme di legge o principi contabili da applicare";
                sp.Children.Add( chk );
									
                chk = new CheckBox();
                chk.Name = "chk2_" + itemD.Key;

                if ( _ReadOnly == true )
                {
                    chk.PreviewKeyDown += obj_PreviewKeyDown;
                    chk.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDown;
                }
             
                foreach(DataRow dd in dati.Rows)
                {
                   if(dd["ID"].ToString()==itemD.Key.ToString())
                       {
                       if(dd["chk2"].ToString()!= "")
                            {
                                chk.IsChecked = true;
                            }
                            else
                            {
                                chk.IsChecked = false;
                            }
                       }
                }
          
                this.RegisterName( chk.Name, chk );
                chk.Content = "modalità di applicazione di norme di legge e principi contabili sui quali si concorda";
                sp.Children.Add( chk );
										
                chk = new CheckBox();
                chk.Name = "chk3_" + itemD.Key;

                if ( _ReadOnly == true )
                {
                    chk.PreviewKeyDown += obj_PreviewKeyDown;
                    chk.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDown;
                }
                 foreach(DataRow dd in dati.Rows)
                {
                   if(dd["ID"].ToString()==itemD.Key.ToString())
                       {
                       if(dd["chk3"].ToString()!= "")
                            {
                                chk.IsChecked = true;
                            }
                            else
                            {
                                chk.IsChecked = false;
                            }
                       }
                }
               
                this.RegisterName( chk.Name, chk );
                chk.Content = "completezza d'informativa";
                sp.Children.Add( chk );
                #endregion

                #region RTF TEXT BOX
                Grid grtf = new Grid();
                grtf.Margin = new Thickness( 0, 20, 0, 0 );
                grtf.Height = 280;
                grtf.MinWidth = 550;

                cd = new ColumnDefinition();
                cd.Width = GridLength.Auto;
                grtf.ColumnDefinitions.Add( cd );

                grtf.RowDefinitions.Add( new RowDefinition() );

                StackPanel dkp = new StackPanel();

                RichTextBox rtfb = new RichTextBox();
                rtfb.FontSize = 16.0;
                rtfb.Selection.ApplyPropertyValue( FlowDocument.TextAlignmentProperty, TextAlignment.Justify );
                rtfb.Name = "rtfb_" + itemD.Key;
                this.RegisterName( rtfb.Name, rtfb );
                rtfb.AcceptsTab = true;
                Style style = new Style( typeof( Paragraph ) );
                style.Setters.Add( new Setter( Paragraph.MarginProperty, new Thickness( 0, 0, 0, 0 ) ) );
                rtfb.Resources.Add( typeof( Paragraph ), style );
                rtfb.Height = 230;
                grtf.Width = 550;

                rtfb.PreviewKeyDown += OnClearClipboard;

                if ( _ReadOnly == true )
                {
                    rtfb.PreviewKeyDown += obj_PreviewKeyDown;
                    rtfb.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDown;
                }

                TextBlock txtValore = new TextBlock();
                txtValore.Visibility = System.Windows.Visibility.Collapsed;
                txtValore.Name = "txtValore_" + itemD.Key; 
                this.RegisterName( txtValore.Name, txtValore );

                string testo = item["name"].ToString();
                foreach(DataRow dd in dati.Rows)
                {
                   if(dd["ID"].ToString()==itemD.Key.ToString())
                       {
                       testo = dd["testo"].ToString();
                       }
                }
               

                if ( testo.Trim() != "" )
                {
                    MemoryStream stream = new MemoryStream( Encoding.UTF8.GetBytes( testo ) );
                    rtfb.Selection.Load( stream, DataFormats.Rtf );

                    TextRange tr = new TextRange( rtfb.Document.ContentStart, rtfb.Document.ContentEnd );
                    MemoryStream ms = new MemoryStream();
                    tr.Save( ms, DataFormats.Text );

                    txtValore.Text = Encoding.UTF8.GetString( ms.ToArray() );
                }
                else
                {
                    txtValore.Text = "";
                }

                ToolBar toolb = new ToolBar();

                if ( _ReadOnly == true )
                {
                    toolb.PreviewKeyDown += obj_PreviewKeyDown;
                    toolb.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDown;
                }

                toolb.Height = 30;

                Button btn = new Button();
                btn.Command = ApplicationCommands.Cut;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Taglia";
                btn.IsTabStop = false;
                Image img = new Image();
                img.Source = new BitmapImage( new Uri( "./Images/EditCut.png", UriKind.Relative ) );
                btn.Content = img;
                toolb.Items.Add( btn );

                  
                btn = new Button();
                btn.Command = ApplicationCommands.Copy;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Copia";
                btn.IsTabStop = false;
                img = new Image();
                img.Source = new BitmapImage( new Uri( "./Images/EditCopy.png", UriKind.Relative ) );
                btn.Content = img;
                toolb.Items.Add( btn );

                btn = new Button();
                btn.Command = ApplicationCommands.Paste;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Incolla";
                btn.IsTabStop = false;
                img = new Image();
                img.Source = new BitmapImage( new Uri( "./Images/EditPaste.png", UriKind.Relative ) );
                btn.Content = img;
                toolb.Items.Add( btn );

                btn = new Button();
                btn.Command = ApplicationCommands.Undo;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Undo";
                btn.IsTabStop = false;
                img = new Image();
                img.Source = new BitmapImage( new Uri( "./Images/EditUndo.png", UriKind.Relative ) );
                btn.Content = img;
                toolb.Items.Add( btn );

                btn = new Button();
                btn.Command = ApplicationCommands.Redo;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Redo";
                btn.IsTabStop = false;
                img = new Image();
                img.Source = new BitmapImage( new Uri( "./Images/EditRedo.png", UriKind.Relative ) );
                btn.Content = img;
                toolb.Items.Add( btn );

                btn = new Button();
                btn.Command = EditingCommands.ToggleBold;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Grassetto";
                btn.IsTabStop = false;
                TextBlock txtstyle = new TextBlock();
                txtstyle.FontWeight = FontWeights.Bold;
                txtstyle.Text = "B";
                btn.Content = txtstyle;
                toolb.Items.Add( btn );

                btn = new Button();
                btn.Command = EditingCommands.ToggleItalic;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Italico";
                btn.IsTabStop = false;
                txtstyle = new TextBlock();
                txtstyle.FontWeight = FontWeights.Bold;
                txtstyle.FontStyle = FontStyles.Italic;
                txtstyle.Text = "I";
                btn.Content = txtstyle;
                toolb.Items.Add( btn );

                btn = new Button();
                btn.Command = EditingCommands.ToggleUnderline;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Sottolinea";
                btn.IsTabStop = false;
                txtstyle = new TextBlock();
                txtstyle.FontWeight = FontWeights.Bold;
                txtstyle.TextDecorations = TextDecorations.Underline;
                txtstyle.Text = "U";
                btn.Content = txtstyle;
                toolb.Items.Add( btn );

                btn = new Button();
                btn.Command = EditingCommands.IncreaseFontSize;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Aumenta Font";
                btn.IsTabStop = false;
                img = new Image();
                img.Source = new BitmapImage( new Uri( "./Images/CharacterGrowFont.png", UriKind.Relative ) );
                btn.Content = img;
                toolb.Items.Add( btn );

                btn = new Button();
                btn.Command = EditingCommands.DecreaseFontSize;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Diminuisci Font";
                btn.IsTabStop = false;
                img = new Image();
                img.Source = new BitmapImage( new Uri( "./Images/CharacterShrinkFont.png", UriKind.Relative ) );
                btn.Content = img;
                toolb.Items.Add( btn );

                btn = new Button();
                btn.Command = EditingCommands.ToggleBullets;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Elenco Puntato";
                btn.IsTabStop = false;
                img = new Image();
                img.Source = new BitmapImage( new Uri( "./Images/ListBullets.png", UriKind.Relative ) );
                btn.Content = img;
                toolb.Items.Add( btn );

                btn = new Button();
                btn.Command = EditingCommands.ToggleNumbering;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Elenco Numerato";
                btn.IsTabStop = false;
                img = new Image();
                img.Source = new BitmapImage( new Uri( "./Images/ListNumbering.png", UriKind.Relative ) );
                btn.Content = img;
                toolb.Items.Add( btn );

                btn = new Button();
                btn.Command = EditingCommands.AlignLeft;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Allineato a sinistra";
                btn.IsTabStop = false;
                img = new Image();
                img.Source = new BitmapImage( new Uri( "./Images/ParagraphLeftJustify.png", UriKind.Relative ) );
                btn.Content = img;
                toolb.Items.Add( btn );

                btn = new Button();
                btn.Command = EditingCommands.AlignCenter;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Allineato Centrato";
                btn.IsTabStop = false;
                img = new Image();
                img.Source = new BitmapImage( new Uri( "./Images/ParagraphCenterJustify.png", UriKind.Relative ) );
                btn.Content = img;
                toolb.Items.Add( btn );

                btn = new Button();
                btn.Command = EditingCommands.AlignRight;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Allineato a destra";
                btn.IsTabStop = false;
                img = new Image();
                img.Source = new BitmapImage( new Uri( "./Images/ParagraphRightJustify.png", UriKind.Relative ) );
                btn.Content = img;
                toolb.Items.Add( btn );

                btn = new Button();
                btn.Command = EditingCommands.AlignJustify;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Allineato Giustificato";
                btn.IsTabStop = false;
                img = new Image();
                img.Source = new BitmapImage( new Uri( "./Images/ParagraphFullJustify.png", UriKind.Relative ) );
                btn.Content = img;
                toolb.Items.Add( btn );

                btn = new Button();
                btn.Command = EditingCommands.IncreaseIndentation;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Tabulazione a destra";
                btn.IsTabStop = false;
                img = new Image();
                img.Source = new BitmapImage( new Uri( "./Images/ParagraphIncreaseIndentation.png", UriKind.Relative ) );
                btn.Content = img;
                toolb.Items.Add( btn );

                btn = new Button();
                btn.Command = EditingCommands.DecreaseIndentation;
                btn.CommandTarget = rtfb;
                btn.ToolTip = "Tabulazione a sinistra";
                btn.IsTabStop = false;
                img = new Image();
                img.Source = new BitmapImage( new Uri( "./Images/ParagraphDecreaseIndentation.png", UriKind.Relative ) );
                btn.Content = img;
                toolb.Items.Add( btn );

                dkp.Children.Add( toolb );
                dkp.Children.Add( rtfb );
                dkp.Children.Add( txtValore );

                dkp.SetValue( Grid.RowProperty, 0 );
                dkp.SetValue( Grid.ColumnProperty, 0 );

                grtf.Children.Add( dkp );

                sp.Children.Add( grtf );
#endregion

                sp.SetValue( Grid.RowProperty, 1 );
                sp.SetValue( Grid.ColumnProperty, 1 );

                sp.Visibility = System.Windows.Visibility.Collapsed;
                uriSource = new Uri( left, UriKind.Relative );
                ( (Image)( g.Children[0] ) ).Source = new BitmapImage( uriSource );

                g.Children.Add( sp );

                b.Child = g;

                RowDefinition rdg = new RowDefinition();
                gg.RowDefinitions.Add( rdg );

                b.SetValue( Grid.RowProperty, rowattuale );
                b.SetValue( Grid.ColumnProperty, 0 );

                rowattuale++;

                gg.Children.Add( b );
            }

         
        }

		public int Save()
		{
            
            foreach (KeyValuePair<int, DataRow> itemD in lista.OrderBy(key => key.Key))
            {
                RichTextBox rtfhere = (RichTextBox)this.FindName( "rtfb_" + itemD.Key );
                TextBlock txthere = (TextBlock)this.FindName( "txtValore_" + itemD.Key );

                TextRange tr = new TextRange( rtfhere.Document.ContentStart, rtfhere.Document.ContentEnd );
                MemoryStream ms = new MemoryStream();
                tr.Save( ms, DataFormats.Rtf );
                string xamlText = ASCIIEncoding.Default.GetString( ms.ToArray() );
                CheckBox chkInserireRelazionehere = (CheckBox)this.FindName( "chkInserireRelazione_" + itemD.Key );
                CheckBox chk1here = (CheckBox)this.FindName( "chk1_" + itemD.Key );
                CheckBox chk2here = (CheckBox)this.FindName( "chk2_" + itemD.Key );
                CheckBox chk3here = (CheckBox)this.FindName( "chk3_" + itemD.Key );
                foreach(DataRow dd in dati.Rows)
                {
                   if(dd["ID"].ToString()==itemD.Key.ToString())
                       {
                        dd["testo"] =  xamlText.Replace( "\\f1", "\\f0" ).Replace( "\\f2", "\\f0" ).Replace( "{\\f0\\fcharset0 Times New Roman;}", "{\\f0 Arial;\\f1 Wingdings 2;\\f2 Wingdings;}" );
                        dd["chkInserireRelazione"] = ( chkInserireRelazionehere.IsChecked == true ) ? chkInserireRelazionehere.Content.ToString() : "";  
                        dd["chk1"] = ( chk1here.IsChecked == true ) ? chk1here.Content.ToString() : "";    
                        dd["chk2"] = ( chk2here.IsChecked == true ) ? chk2here.Content.ToString() : "";  
                        dd["chk3"] = ( chk3here.IsChecked == true ) ? chk3here.Content.ToString() : ""; 
                       }
                }
             
            }

            cBusinessObjects.SaveData(id, dati, typeof(RelazioneErroriRilevati));

            return 0;
		}

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
			if (firsttime)
			{
				firsttime = false;			
				return;
			}           
        }

		private void obj_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}
		}

		private void obj_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}
		}

        private void obj_PreviewMouseLeftButtonDownSelezioneRDB( object sender, MouseButtonEventArgs e )
        {
            if ( !_ReadOnly )
            {
                MessageBox.Show( "Per poter modificare il contenuto, bisogna prima selezionare questa voce", "Attenzione" );
            }
            return;
        }

        private void obj_PreviewKeyDownSelezioneRDB( object sender, KeyEventArgs e )
        {
            if ( !_ReadOnly )
            {
                MessageBox.Show( "Per poter modificare il contenuto, bisogna prima selezionare questa voce", "Attenzione" );
            }
            return;
        }

        private void UserControl_SizeChanged( object sender, SizeChangedEventArgs e )
        {
            double newsize = e.NewSize.Width - 30.0;

            foreach ( UIElement item in gg.Children )
            {
                try
                {
                    ( (StackPanel)( ( (Grid)( ( (Border)( item ) ).Child ) ).Children[2] ) ).Width = newsize - 50;
                    ( (Grid)( ( (StackPanel)( ( (Grid)( ( (Border)( item ) ).Child ) ).Children[2] ) ).Children[6] ) ).MinWidth = newsize - 70;
                    ( (RichTextBox)( ( (StackPanel)( ( (Grid)( ( (StackPanel)( ( (Grid)( ( (Border)( item ) ).Child ) ).Children[2] ) ).Children[6] ) ).Children[0] ) ).Children[1] ) ).Width = newsize - 70;
                }
                catch ( Exception ex )
                {
                    string log = ex.Message;
                }
            }

            try
            {
                gg.Width = Convert.ToDouble( newsize );
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }
        }

        private void UserControl_Loaded( object sender, RoutedEventArgs e )
        {
            FocusNow();
        }

        private void OnClearClipboard( object sender, KeyEventArgs keyEventArgs )
        {
            if ( Clipboard.ContainsImage() && keyEventArgs.Key == Key.V && ( Keyboard.Modifiers & ModifierKeys.Control ) != 0 )
                Clipboard.Clear();
        }

        private void Image_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            Image i = ( (Image)sender );

            try
            {
                StackPanel u = ( (StackPanel)( ( (Grid)( i.Parent ) ).Children[2] ) );

                if ( u.Visibility == System.Windows.Visibility.Collapsed )
                {
                    u.Visibility = System.Windows.Visibility.Visible;
                    var uriSource = new Uri( down, UriKind.Relative );
                    i.Source = new BitmapImage( uriSource );
                }
                else
                {
                    u.Visibility = System.Windows.Visibility.Collapsed;
                    var uriSource = new Uri( left, UriKind.Relative );
                    i.Source = new BitmapImage( uriSource );
                }
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }
        }

        private string ConvertNumberNoDecimal( string valore )
        {
            double dblValore = 0.0;

            double.TryParse( valore, out dblValore );

            if ( dblValore == 0.0 )
            {
                return "0";
            }
            else
            {
                return String.Format( "{0:#,#}", dblValore );
            }
        }
    }
}
