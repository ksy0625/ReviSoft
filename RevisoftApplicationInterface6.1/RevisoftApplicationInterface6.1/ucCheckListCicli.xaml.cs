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
    public partial class ucCheckListCicli : UserControl
    {

        public int id;
        private DataTable dati = null;

        private XmlDataProviderManager _x;
		string _ID;

        public bool txtFound = false;
        private int Offset = 260;
        private int OffsetNote = 50;        
        private int Minimo = 200;
        private string down = "./Images/icone/navigate_down.png";
        private string up = "./Images/icone/navigate_up.png";

		private char divisoriaCondizioniMultiple = '/';
		private char divisoriaRisultato = '=';
		private char divisoriaIDValore = '@';
		private char divisoriaOR = '|';
		private char divisoriaAND = '&';

		private bool _ReadOnly = true;
		private bool _WithResult = false;
		private string _Condizione = "";
        
        private bool _NoData = true;

        public ucCheckListCicli()
        {
            InitializeComponent();
        }

        public bool ReadOnly
        {
            set
            {
                _ReadOnly = value;
            }
        }

		public string Condizione
		{
			set
			{
				if (value != "")
				{
					brdRisultato.Visibility = System.Windows.Visibility.Visible;
					_WithResult = true;
					_Condizione = value;
				}
			}
		}


        public void Load(string ID, string IDCliente, string IDSessione)
        {

            id = int.Parse(ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
            string name = "";
            
			_ID = ID;
            dati = cBusinessObjects.GetData(id, typeof(CheckList));

            if (dati.Rows.Count > 0)
                _NoData = false;

            Binding b = new Binding();
            b.Source = dati;
            itmDomande.SetBinding(ItemsControl.ItemsSourceProperty, b);
          
            //MM ???
            foreach (DataRow dtrow in dati.Rows)
            {
                //MM ???
                name = dtrow["name"].ToString();
               
            }

            if (_WithResult)
			{
				CalcolaCondizione();
			}

            this.mainRTB.Selection.ApplyPropertyValue( FlowDocument.TextAlignmentProperty, TextAlignment.Justify );

            try
            {
                MemoryStream stream = new MemoryStream( ASCIIEncoding.Default.GetBytes(name) );
                this.mainRTB.Selection.Load( stream, DataFormats.Rtf );

                TextRange tr = new TextRange( mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd );
                MemoryStream ms = new MemoryStream();
                tr.Save( ms, DataFormats.Text );
                txtValore.Text = ASCIIEncoding.Default.GetString( ms.ToArray() );
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
                txtValore.Text = "";
            }

            txtValore.Focus();

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

		public int Save()
		{
            foreach (DataRow dtrow in dati.Rows)
            {
                if (txtRisultato.Text != "")
                {
                    dtrow["risultato"] = txtRisultato.Text;
                }
                else
                {
                    dtrow["risultato"] = "";
                }
            }
          

           
            TextRange tr = new TextRange( mainRTB.Document.ContentStart,
                            mainRTB.Document.ContentEnd );
            MemoryStream ms = new MemoryStream();
            tr.Save( ms, DataFormats.Rtf );
            string xamlText = ASCIIEncoding.Default.GetString( ms.ToArray() );
//MM ???
            string name = xamlText.Replace( "\\f1", "\\f0" ).Replace( "\\f2", "\\f0" ).Replace( "{\\f0\\fcharset0 Times New Roman;}", "{\\f0 Arial;\\f1 Wingdings 2;\\f2 Wingdings;}" );

            tr.Save( ms, DataFormats.Text );

            txtValore.Text = ASCIIEncoding.Default.GetString( ms.ToArray() );

            return cBusinessObjects.SaveData(id, dati, typeof(CheckList));

        }

        private void TextBox_TextChanged( object sender, TextChangedEventArgs e )
        {
        }

        public void Resizer(int newsize)
        {
			//if (newsize > Offset)
			//{
			//    grdMainContainer.Width = newsize - Offset;
			//}

			for (int i = 0; i < itmDomande.Items.Count; i++)
			{
				ContentPresenter cp = itmDomande.ItemContainerGenerator.ContainerFromIndex(i) as ContentPresenter;

				TextBlock t = FindTextBlockWithWrap(cp);

				if (t != null)
				{
					txtFound = true;
					int newWidth = newsize - Offset;

					if (newWidth <= Minimo)
					{
						newWidth = Minimo;
					}

					if (t.Width == newWidth)
					{
						break;
					}

					t.Width = newWidth;
				}
				else
				{
					break;
				}

                TextBox tt = FindTextBoxkWithWrap( cp );

                if ( tt != null )
                {
                    int newWidth = newsize - OffsetNote;

                    if ( newWidth <= Minimo )
                    {
                        newWidth = Minimo;
                    }

                    if ( tt.Width == newWidth )
                    {
                        break;
                    }

                    tt.Width = newWidth;
                }
			}

            double tmp = newsize - 80.0;

            if ( tmp <= 20 )
            {
                return;
            }

            txtTitolo.Width = tmp - 20;
            txtValore.Width = tmp - 20;
        }

        public TextBlock FindTextBlockWithWrap(DependencyObject depObj)
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is TextBlock && ((TextBlock)child).TextWrapping == TextWrapping.Wrap)
                    {
                        return (TextBlock)child;
                    }

                    TextBlock childItem = FindTextBlockWithWrap(child);
                    if (childItem != null)
                    {
                        return childItem;
                    }
                }
            }
            return null;
        }

        public TextBox FindTextBoxkWithWrap( DependencyObject depObj )
        {
            if ( depObj != null )
            {
                for ( int i = 0; i < VisualTreeHelper.GetChildrenCount( depObj ); i++ )
                {
                    DependencyObject child = VisualTreeHelper.GetChild( depObj, i );
                    if ( child != null && child is TextBox && ((TextBox)child).TextWrapping == TextWrapping.Wrap )
                    {
                        return (TextBox)child;
                    }

                    TextBox childItem = FindTextBoxkWithWrap( child );
                    if ( childItem != null )
                    {
                        return childItem;
                    }
                }
            }
            return null;
        }

        public TextBox FindTextBox(DependencyObject depObj)
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is TextBox)
                    {
                        return (TextBox)child;
                    }

                    TextBox childItem = FindTextBox(child);
                    if (childItem != null)
                    {
                        return childItem;
                    }
                }
            }
            return null;
        }

        public Image FindImage(DependencyObject depObj)
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is Image)
                    {
                        return (Image)child;
                    }

                    Image childItem = FindImage(child);
                    if (childItem != null)
                    {
                        return childItem;
                    }
                }
            }
            return null;
        }

        private void itmDomande_LayoutUpdated(object sender, EventArgs e)
        {
            if (!txtFound)
            {
                Resizer(Minimo);                
            }
        }

		private void CalcolaCondizione()
		{
			txtRisultato.Text = "";

			string[] condizioni = _Condizione.Split(divisoriaCondizioniMultiple);
			foreach (string condizione in condizioni)
			{
				if (condizione.Split(divisoriaRisultato).Count() == 2)
				{
					string risultato = condizione.Split(divisoriaRisultato)[1];
					string rimanente = condizione.Split(divisoriaRisultato)[0];
					
					if (rimanente.IndexOf(divisoriaOR) != -1)
					{
						string[] coppieIDValori = rimanente.Split(divisoriaOR);

						foreach (string IDValore in coppieIDValori)
						{
							if (IDValore.Split(divisoriaIDValore).Count() == 2)
							{
								string IDint = IDValore.Split(divisoriaIDValore)[0];
								string valore = IDValore.Split(divisoriaIDValore)[1];

								XmlNode node = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]/Valore[@ID=" + IDint + "]");

								if (node == null)
								{
									continue;
								}

								if (node.Attributes["value"].Value == valore)
								{
									txtRisultato.Text = "Risultato Check List: " + risultato;
									return;
								}
							}
						}
					}
					else
					{
						string[] coppieIDValori = rimanente.Split(divisoriaAND);

						if (coppieIDValori.Count() > 0)
						{
							bool ok = true;

							foreach (string IDValore in coppieIDValori)
							{
								if (IDValore.Split(divisoriaIDValore).Count() == 2)
								{
									string IDint = IDValore.Split(divisoriaIDValore)[0];
									string valore = IDValore.Split(divisoriaIDValore)[1];

									XmlNode node = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]/Valore[@ID=" + IDint + "]");

									if (node == null)
									{
										continue;
									}

									if (node.Attributes["value"].Value != valore)
									{
										ok = false;
										break;
									}
								}
							}

							if (ok)
							{
								txtRisultato.Text = "Risultato Check List: " + risultato;
								return;
							}
						}
					}
				}
			}		
		}

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject child = ((Image)sender).Parent;

            TextBox t = FindTextBox(child);
            Image i = FindImage(child);

            if (t.Visibility == System.Windows.Visibility.Collapsed)
            {
                t.Visibility = System.Windows.Visibility.Visible;
                var uriSource = new Uri(up, UriKind.Relative);
                i.Source = new BitmapImage(uriSource);
            }
            else
            {
                t.Visibility = System.Windows.Visibility.Collapsed;
                var uriSource = new Uri(down, UriKind.Relative);
                i.Source = new BitmapImage(uriSource);
            }
        }

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Resizer(Convert.ToInt32(e.NewSize.Width)); 
		}

		private void RadioButton_Checked(object sender, RoutedEventArgs e)
		{
			if (_WithResult)
			{
				CalcolaCondizione();
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

        private void UserControl_Loaded( object sender, RoutedEventArgs e )
        {
            if ( _NoData)//_ID == "253" || _ID == "252" )
            {
                itmDomande.Visibility = System.Windows.Visibility.Hidden;
                //MessageBox.Show( "Utilizzare Tasto Commenti" );
            }
        }
    }
}
