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
using System.ComponentModel;
using System.Data;

namespace UserControls
{
    public partial class ucTestoPropostoMultiploNonEsclusivo : UserControl
    {
        public int id;
        private DataTable dati = null;

        private string _ID = "-1";
		private bool firsttime = true;
        ArrayList dynamicRDB= new ArrayList();
        Hashtable HTNode = new Hashtable();

		private bool _ReadOnly = false;

        public ucTestoPropostoMultiploNonEsclusivo()
        {
            InitializeComponent();

            //txtValore.Focus();
        }
        
        public void FocusNow()
        {
            //txtValore.Focus();
        }

        public bool ReadOnly 
        {
            set
            {
				_ReadOnly = value;
                                
                foreach ( string rdbName in dynamicRDB )
                {
                    CheckBox rdbhere = (CheckBox)this.FindName( rdbName );

                    rdbhere.IsHitTestVisible = !_ReadOnly;
                    string suffix = rdbName.Replace( "rdb_", "" );

                    RichTextBox rtfhere = (RichTextBox)this.FindName( "rtfb_" + suffix );
                    ToolBar tbhere = (ToolBar)this.FindName( "toolb_" + suffix );

                    rtfhere.PreviewKeyDown -= obj_PreviewKeyDown;
                    rtfhere.PreviewMouseLeftButtonDown -= obj_PreviewMouseLeftButtonDown;

                    tbhere.PreviewKeyDown -= obj_PreviewKeyDown;
                    tbhere.PreviewMouseLeftButtonDown -= obj_PreviewMouseLeftButtonDown;

                    if ( _ReadOnly == true )
                    {
                        rtfhere.PreviewKeyDown += obj_PreviewKeyDown;
                        rtfhere.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDown;

                        tbhere.PreviewKeyDown += obj_PreviewKeyDown;
                        tbhere.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDown;
                    }
                }
            }
        }

        public void Load(string ID, string IDCliente, string IDSessione)
        {
            id = int.Parse(ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());

            _ID = ID;

            dati = cBusinessObjects.GetData(id, typeof(TestoPropostoMultiplo));

            if (dati.Rows.Count == 0)
            {
                dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, cBusinessObjects.empty_rtf);
            }
            int rowattuale = 0;
            int blockindex = Convert.ToInt32(ID);
            bool disabilitatoperassenzascelta = false;

            foreach (DataRow dtrow in dati.Rows)
            {
                bool notApplicable = false;

                if (dtrow["value"].ToString().Contains("N/A"))
                {
                    notApplicable = true;
                }

                RowDefinition rdg = new RowDefinition();
                rdg.Height = new GridLength(20);
                g.RowDefinitions.Add( rdg );
                rdg = new RowDefinition();
                if ( notApplicable )
                {
                    rdg.Height = new GridLength( 30 );
                }
                else
                {
                    rdg.Height = new GridLength( 200 );
                }
                g.RowDefinitions.Add( rdg );

                //RDB                
                CheckBox rdb = new CheckBox();
                rdb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                rdb.Checked += rdb_Checked;

                if ( rowattuale == 0 )
                {
                    rdb.Visibility = System.Windows.Visibility.Collapsed;
                    rdb.IsChecked = true;
                    disabilitatoperassenzascelta = false;
                }
                else
                {
                    disabilitatoperassenzascelta = true;
                }

                if (dtrow["strchecked"] != null )
                {
                    rdb.IsChecked = ( (dtrow["strchecked"].ToString() == "true" ) ? true : false );
                }
                else
                {
                    rdb.IsChecked = false;
                }

                rdb.Name = "rdb_" + blockindex.ToString() + "_" + (rowattuale + 1).ToString();
                dynamicRDB.Add( rdb.Name );
                HTNode.Add( rdb.Name, dtrow);
                this.RegisterName( rdb.Name, rdb );

                rdb.SetValue( Grid.RowProperty, rowattuale );
                rdb.SetValue( Grid.RowSpanProperty, 2 );
                rdb.SetValue( Grid.ColumnProperty, 0 );

                if ( _ReadOnly == true )
                {
                    rdb.IsHitTestVisible = false;
                }

                g.Children.Add( rdb );

                //TITOLO
                TextBlock tb = new TextBlock();
                tb.Focusable = false;
                tb.Text = dtrow["name"].ToString();
                tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                tb.Margin = new Thickness( 0, 10, 0, 0 );
                tb.FontSize = 13;
                tb.Foreground = Brushes.Gray;
                tb.Height = 25;
                tb.FontWeight = FontWeights.SemiBold;
                tb.Visibility = System.Windows.Visibility.Collapsed;

                tb.SetValue( Grid.RowProperty, rowattuale );
                tb.SetValue( Grid.ColumnProperty, 1 );

                rowattuale++;

                g.Children.Add( tb );

                // RTF TEXT BOX
                Grid grtf = new Grid();
                grtf.Margin = new Thickness( 0, 0, 0, 10 );
                grtf.MinWidth = 550;

                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = GridLength.Auto;
                grtf.ColumnDefinitions.Add( cd );

                grtf.RowDefinitions.Add( new RowDefinition() );

                StackPanel dkp = new StackPanel();

                RichTextBox rtfb = new RichTextBox();
                rtfb.FontSize = 16.0;
                rtfb.Selection.ApplyPropertyValue( FlowDocument.TextAlignmentProperty, TextAlignment.Justify );
                rtfb.AcceptsTab = true;
                Style style = new Style( typeof( Paragraph ) );
                style.Setters.Add( new Setter( Paragraph.MarginProperty, new Thickness( 0, 0, 0, 0 ) ) );
                rtfb.Resources.Add( typeof( Paragraph ), style );

                if (dati.Rows.Count <= 1 )
                {
                    grtf.Height = 300;
                    rtfb.Height = 260;
                    //rtfb.Background = Brushes.LightGray;
                }
                else
                {
                    grtf.Height = 200;
                    rtfb.Height = 160;
                }
               
                rtfb.Width = 550;
                rtfb.Name = "rtfb_" + blockindex.ToString() + "_" + rowattuale.ToString();
                this.RegisterName( rtfb.Name, rtfb );

                rtfb.PreviewKeyDown += OnClearClipboard;

                if ( disabilitatoperassenzascelta )
                {
                    rtfb.PreviewKeyDown += obj_PreviewKeyDownSelezioneRDB;
                    rtfb.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDownSelezioneRDB;
                }

                if ( _ReadOnly == true )
                {
                    rtfb.PreviewKeyDown += obj_PreviewKeyDown;
                    rtfb.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDown;
                }

                TextBlock txtValore = new TextBlock();
                txtValore.Visibility = System.Windows.Visibility.Collapsed;
                txtValore.Name = "txtValore_" + blockindex.ToString() + "_" + rowattuale.ToString();
                this.RegisterName( txtValore.Name, txtValore );
                                
                MemoryStream stream = new MemoryStream( Encoding.UTF8.GetBytes( dtrow["value"].ToString() ) );
                rtfb.Selection.Load( stream, DataFormats.Rtf );

                TextRange tr = new TextRange( rtfb.Document.ContentStart, rtfb.Document.ContentEnd );
                MemoryStream ms = new MemoryStream();
                tr.Save( ms, DataFormats.Text );

                txtValore.Text = Encoding.UTF8.GetString( ms.ToArray() );

                ToolBar toolb = new ToolBar();
                toolb.Name = "toolb_" + blockindex.ToString() + "_" + rowattuale.ToString();
                this.RegisterName( toolb.Name, toolb );
                if ( disabilitatoperassenzascelta )
                {
                    toolb.PreviewKeyDown += obj_PreviewKeyDownSelezioneRDB;
                    toolb.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDownSelezioneRDB;
                }

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
                
                if (dtrow["strchecked"] != null && dtrow["strchecked"].ToString() == "true" )
                {
                    rdb.IsChecked = true;
                }
                else
                {
                    rdb.IsChecked = false;

                    if (dtrow["strchecked"] == null && notApplicable )
                    {
                        rdb.IsChecked = true;
                    }
                }

                if ( notApplicable )
                {
                    txtValore.Visibility = System.Windows.Visibility.Visible;
                    dkp.Children.Add( txtValore );
                    grtf.Height = 20;
                }
                else
                {
                    dkp.Children.Add( toolb );
                    dkp.Children.Add( rtfb );
                    dkp.Children.Add( txtValore );
                }

                dkp.SetValue( Grid.RowProperty, 0 );
                dkp.SetValue( Grid.ColumnProperty, 0 );

                grtf.Children.Add( dkp );

                grtf.SetValue( Grid.RowProperty, rowattuale );
                grtf.SetValue( Grid.ColumnProperty, 1 );

                rowattuale++;

                g.Children.Add( grtf );
                
            }
        }

        void rdb_Checked( object sender, RoutedEventArgs e )
        {
            foreach (string rdbName in dynamicRDB)
	        {
                CheckBox rdbhere = (CheckBox)this.FindName( rdbName );
                string suffix = rdbName.Replace("rdb_","");

                RichTextBox rtfhere = (RichTextBox)this.FindName( "rtfb_" + suffix );
                ToolBar tbhere = (ToolBar)this.FindName( "toolb_" + suffix );

                rtfhere.PreviewKeyDown -= obj_PreviewKeyDownSelezioneRDB;
                rtfhere.PreviewMouseLeftButtonDown -= obj_PreviewMouseLeftButtonDownSelezioneRDB;

                tbhere.PreviewKeyDown -= obj_PreviewKeyDownSelezioneRDB;
                tbhere.PreviewMouseLeftButtonDown -= obj_PreviewMouseLeftButtonDownSelezioneRDB;

                DataRow nodehere = ((DataRow)(HTNode[rdbName]));


                if(rdbhere.IsChecked == false)
                {
                    rtfhere.PreviewKeyDown += obj_PreviewKeyDownSelezioneRDB;
                    rtfhere.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDownSelezioneRDB;

                    tbhere.PreviewKeyDown += obj_PreviewKeyDownSelezioneRDB;
                    tbhere.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDownSelezioneRDB;

                    nodehere["checked"] = "false";
                }
                else
                {
                    nodehere["checked"] = "true";

                    rtfhere.PreviewKeyDown -= obj_PreviewKeyDownSelezioneRDB;
                    rtfhere.PreviewMouseLeftButtonDown -= obj_PreviewMouseLeftButtonDownSelezioneRDB;

                    tbhere.PreviewKeyDown -= obj_PreviewKeyDownSelezioneRDB;
                    tbhere.PreviewMouseLeftButtonDown -= obj_PreviewMouseLeftButtonDownSelezioneRDB;
                }
	        }
            
        }

        public bool CheckifOK()
        {
          

            if ( dati.Rows.Count <= 1 )
            {
                return true;
            }

            int rowattuale = 1;
            int blockindex = Convert.ToInt32( _ID );
            bool atleastone = false;

            foreach (DataRow dtrow in dati.Rows)
            {
                string suffix = blockindex + "_" + rowattuale;

                rowattuale = rowattuale + 2;

                CheckBox rdbhere = (CheckBox)this.FindName( "rdb_" + suffix );

                if ( rdbhere.IsChecked == true )
                {
                    atleastone = true;
                }
            }

            if ( !atleastone )
            {
                MessageBox.Show( "Attenzione, non è stata selezionata nessuna opzione.", "Attenzione" );
                return false;
            }

            return true;
        }

		public int Save()
		{

      

            int rowattuale = 1;
            int blockindex = Convert.ToInt32( _ID );

            foreach (DataRow dtrow in dati.Rows)
            {
                string suffix = blockindex + "_" + rowattuale;

                rowattuale = rowattuale + 2;

                CheckBox rdbhere = (CheckBox)this.FindName( "rdb_" + suffix );
                RichTextBox rtfhere = (RichTextBox)this.FindName( "rtfb_" + suffix );
                TextBlock txthere = (TextBlock)this.FindName( "txtValore_" + suffix );

                TextRange tr = new TextRange( rtfhere.Document.ContentStart, rtfhere.Document.ContentEnd );
                MemoryStream ms = new MemoryStream();
                tr.Save( ms, DataFormats.Rtf );
                string xamlText = ASCIIEncoding.Default.GetString( ms.ToArray() );


                dtrow["strchecked"] = ( ( rdbhere.IsChecked == true ) ? "true" : "false" );

                dtrow["value"] = xamlText.Replace( "\\f1", "\\f0" ).Replace( "\\f2", "\\f0" ).Replace( "{\\f0\\fcharset0 Times New Roman;}", "{\\f0 Arial;\\f1 Wingdings 2;\\f2 Wingdings;}" );

                tr.Save( ms, DataFormats.Text );

                if ( !dtrow["value"].ToString().Contains( "N/A" ) )
                {
                    txthere.Text = ASCIIEncoding.Default.GetString( ms.ToArray() );
                }
            }

            return cBusinessObjects.SaveData(id, dati, typeof(TestoPropostoMultiplo));
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

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double tmp = e.NewSize.Width - 80.0;

			if (tmp <= 20)
			{
				return;
			}

            foreach ( string rdbName in dynamicRDB )
            {
                CheckBox rdbhere = (CheckBox)this.FindName( rdbName );
                string suffix = rdbName.Replace( "rdb_", "" );

                RichTextBox rtfhere = (RichTextBox)this.FindName( "rtfb_" + suffix );
                ToolBar tbhere = (ToolBar)this.FindName( "toolb_" + suffix );

                rtfhere.Width = tmp - 20;
                tbhere.Width = tmp - 20;
            }

            FocusNow();
		}

        private void OnClearClipboard( object sender, KeyEventArgs keyEventArgs )
        {
            if ( keyEventArgs.Key == Key.V && ( Keyboard.Modifiers & ModifierKeys.Control ) != 0 )
            {
                if ( Clipboard.ContainsImage() )
                {
                    Clipboard.Clear();
                }

                if ( Clipboard.ContainsText() )
                {
                    Clipboard.SetText( Clipboard.GetText( TextDataFormat.Text ).Trim(), TextDataFormat.Text );
                }
            }
        }

        private void UserControl_Loaded( object sender, RoutedEventArgs e )
        {
            FocusNow();
        }
    }
}
