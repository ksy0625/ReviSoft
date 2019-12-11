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

namespace UserControls
{
    public partial class ucTestoFromNode : UserControl
    {
		private bool firsttime = true;
        //XmlNode nodeTesto = null;


        private bool _ReadOnly = false;

        public ucTestoFromNode()
        {
            InitializeComponent();

            mainRTB.Focus();
        }
        
        public void FocusNow()
        {
            mainRTB.Focus();
        }

        public bool ReadOnly 
        {
            set
            {
				_ReadOnly = value;

                mainRTB.IsReadOnly = _ReadOnly;
                txtValore.IsReadOnly = _ReadOnly;
            }
        }

        public void Load(string _nodeTesto)
        {
          

          

            if(_nodeTesto == "")
            {
                _nodeTesto = "{\\rtf1\\ansi\\ansicpg1252\\uc1\\htmautsp\\deff2{\\fonttbl{\\f0\\fcharset0 Times New Roman; } {\\f2\\fcharset0 Segoe UI; } }";
                _nodeTesto += "{\\colortbl\\red0\\green0\\blue0;\\red255\\green255\\blue255; }\\loch\\hich\\dbch\\pard\\plain\\ltrpar\\itap0{\\lang1033\\fs24\\f2\\cf0 \\cf0\\ql{\\f2 {\\lang1040\\ltrch Argomento:}\\li0\\ri0\\sa0\\sb0\\fi0\\qj\\par}";
                _nodeTesto += "{\\f2 {\\lang1040\\ltrch }\\li0\\ri0\\sa0\\sb0\\fi0\\qj\\par}";
                _nodeTesto += "{\\f2 {\\lang1040\\ltrch Partecipanti:}\\li0\\ri0\\sa0\\sb0\\fi0\\qj\\par}";
                _nodeTesto += "{\\f2 {\\lang1040\\ltrch }\\li0\\ri0\\sa0\\sb0\\fi0\\qj\\par}";
                _nodeTesto += "}";
                _nodeTesto += "}";
            }

			if (_nodeTesto.Trim() == "Titolo")
			{
                _nodeTesto = "";
			}
            
            this.mainRTB.Selection.ApplyPropertyValue( FlowDocument.TextAlignmentProperty, TextAlignment.Justify );
            
            try
            {
                MemoryStream stream = new MemoryStream( ASCIIEncoding.Default.GetBytes(_nodeTesto) );
                
                this.mainRTB.Selection.Load( stream, DataFormats.Rtf );

                this.mainRTB.ScrollToEnd();

                TextRange tr = new TextRange( mainRTB.Document.ContentStart,
                           mainRTB.Document.ContentEnd );
                MemoryStream ms = new MemoryStream();
                tr.Save( ms, DataFormats.Text );
                txtValore.Text = ASCIIEncoding.Default.GetString( ms.ToArray() );
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
                txtValore.Text = "";
            }

            mainRTB.PreviewKeyDown += OnClearClipboard;
        }

        private void OnClearClipboard( object sender, KeyEventArgs keyEventArgs )
        {
            if(keyEventArgs.Key == Key.V && ( Keyboard.Modifiers & ModifierKeys.Control ) != 0 )
            {
                if ( Clipboard.ContainsImage())
                {
                    Clipboard.Clear();
                }

                if(Clipboard.ContainsText())
                {
                    string valueclipboard = Clipboard.GetText(TextDataFormat.Text).Trim();
                    Clipboard.SetText( valueclipboard, TextDataFormat.Text );

                    MemoryStream stream = new MemoryStream( ASCIIEncoding.Default.GetBytes( valueclipboard ) );

                    this.mainRTB.Selection.Load( stream, DataFormats.Rtf );

                    this.mainRTB.ScrollToEnd();
                }
            }            
        }

		public string Save()
		{
            string nodeTesto = "";
            TextRange tr = new TextRange( mainRTB.Document.ContentStart,
                            mainRTB.Document.ContentEnd );

            MemoryStream ms = new MemoryStream();
            tr.Save( ms, DataFormats.Rtf );
            string xamlText = ASCIIEncoding.Default.GetString( ms.ToArray() );

            nodeTesto = xamlText.Replace( "\\f1", "\\f0" ).Replace( "\\f2", "\\f0" ).Replace( "{\\f0\\fcharset0 Times New Roman;}", "{\\f0 Arial;\\f1 Wingdings 2;\\f2 Wingdings;}" );

            tr.Save( ms, DataFormats.Text );

            txtValore.Text = ASCIIEncoding.Default.GetString( ms.ToArray() );		
            
			return nodeTesto;
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

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double tmp = e.NewSize.Width - 80.0;

			if (tmp <= 20)
			{
				return;
			}
            
			txtValore.Width = tmp - 20;
			grdMainContainer.Width = tmp;

            FocusNow();
		}

        private void UserControl_Loaded( object sender, RoutedEventArgs e )
        {
            FocusNow();
        }
    }
}
