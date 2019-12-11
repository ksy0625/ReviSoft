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
    public partial class ucConsolidatoIstruzioni : UserControl
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

        private Dictionary<string, XmlNode> lista = new Dictionary<string, XmlNode>();

        public ucConsolidatoIstruzioni()
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

        public void Load(string ID, string IDCliente, string IDSessione)
        {
            id = int.Parse(ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());

     

            DataTable datiExcel_Consolidato = cBusinessObjects.GetData(313, typeof(Excel_Consolidato));

            dati = cBusinessObjects.GetData(id, typeof(ConsolidatoIstruzioni));

            int rowattuale = 0;

            if (datiExcel_Consolidato.Rows.Count == 0)
            {
                TextBlock txthere = new TextBlock();
                txthere.Text = "Manca l'indicazione delle Componenti nella Carta di lavoro 3.11.2.";
                gg.Children.Add(txthere);

                return;
            }

            foreach (DataRow dtrow in datiExcel_Consolidato.Rows)
            {
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
                tb.Text = dtrow["name"].ToString();

                foreach (DataRow dtrow2 in dati.Rows)
                {
                    if (int.Parse(dtrow2["riga"].ToString())==rowattuale)
                        dtrow2["titolo" ] = tb.Text;
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
                rtfb.Name = "rtfb_" + rowattuale;
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
                txtValore.Name = "txtValore_" + rowattuale; 
                this.RegisterName( txtValore.Name, txtValore );

                string testo = "";

                foreach (DataRow dtrow2 in dati.Rows)
                {
                    if (int.Parse(dtrow2["riga"].ToString()) == rowattuale)
                        testo= dtrow2["testo"].ToString();
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
            DataTable datiExcel_Consolidato = cBusinessObjects.GetData(313, typeof(Excel_Consolidato));


            int rowattuale = 0;
            foreach (DataRow dtrow in datiExcel_Consolidato.Rows)
            {
                RichTextBox rtfhere = (RichTextBox)this.FindName( "rtfb_" + rowattuale);
                TextBlock txthere = (TextBlock)this.FindName( "txtValore_" + rowattuale);

                TextRange tr = new TextRange( rtfhere.Document.ContentStart, rtfhere.Document.ContentEnd );
                MemoryStream ms = new MemoryStream();
                tr.Save( ms, DataFormats.Rtf );
                string xamlText = ASCIIEncoding.Default.GetString( ms.ToArray() );

                CheckBox chk1here = (CheckBox)this.FindName("chk1_" + rowattuale);
                CheckBox chk2here = (CheckBox)this.FindName("chk2_" + rowattuale);
                CheckBox chk3here = (CheckBox)this.FindName("chk3_" + rowattuale);

                foreach (DataRow dtrow2 in dati.Rows)
                {
                    if (int.Parse(dtrow2["riga"].ToString()) == rowattuale)
                    {
                        dtrow2["testo"] = xamlText.Replace("\\f1", "\\f0").Replace("\\f2", "\\f0").Replace("{\\f0\\fcharset0 Times New Roman;}", "{\\f0 Arial;\\f1 Wingdings 2;\\f2 Wingdings;}");
                        dtrow2["chk1"] = (chk3here.IsChecked == true) ? chk1here.Content.ToString() : "";
                        dtrow2["chk3"] = (chk2here.IsChecked == true) ? chk2here.Content.ToString() : "";
                        dtrow2["chk3"] = (chk3here.IsChecked == true) ? chk3here.Content.ToString() : "";

                    }
                  
                }
              
                rowattuale++;
            }


            return cBusinessObjects.SaveData(id,dati, typeof(ConsolidatoIstruzioni));

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
                    ( (Grid)( ( (StackPanel)( ( (Grid)( ( (Border)( item ) ).Child ) ).Children[2] ) ).Children[0] ) ).MinWidth = newsize - 70;
                    ( (RichTextBox)( ( (StackPanel)( ( (Grid)( ( (StackPanel)( ( (Grid)( ( (Border)( item ) ).Child ) ).Children[2] ) ).Children[0] ) ).Children[0] ) ).Children[1] ) ).Width = newsize - 70;
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
                return "";
            }
            else
            {
                return String.Format( "{0:#,#}", dblValore );
            }
        }
    }
}
