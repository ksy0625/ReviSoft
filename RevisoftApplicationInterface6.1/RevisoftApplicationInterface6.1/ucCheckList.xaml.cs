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
    public partial class ucCheckList : UserControl
    {
        public int id;
        private DataTable dati = null;

    

        public bool txtFound = false;
        private int Offset = 280;
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

        public ucCheckList()
        {
            InitializeComponent();
      SolidColorBrush br = Resources["colorLabelNota"] as SolidColorBrush;
      br.Color = (App._arrBrushes[0] as SolidColorBrush).Color;
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

        bool resizecouldbedone = false;

        public void Load(string ID, string IDCliente, string IDSessione)
        {

            id = int.Parse(ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
    
		 


            dati = cBusinessObjects.GetData(id, typeof(CheckList));

            if(dati.Rows.Count>0)
                _NoData = false;

            Binding b = new Binding();
            b.Source = dati;
            itmDomande.SetBinding(ItemsControl.ItemsSourceProperty, b);
            

            if (_WithResult)
			{
				CalcolaCondizione();
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
          return cBusinessObjects.SaveData(id, dati, typeof(CheckList));

		  }
        
        public void Resizer(int newsize)
        {
            //if (newsize > Offset)
            //{
            //    grdMainContainer.Width = newsize - Offset;
            ////}

            if (resizecouldbedone == false)
            {
                return;
            }

            resizecouldbedone = false;

            int newWidth = newsize - Offset;

            if (newWidth <= Minimo)
            {
                newWidth = Minimo;
            }

            int newWidth2 = newsize - OffsetNote;

            if (newWidth2 <= Minimo)
            {
                newWidth2 = Minimo;
            }

            for (int i = 0; i < itmDomande.Items.Count; i++)
			        {
				        ContentPresenter cp = itmDomande.ItemContainerGenerator.ContainerFromIndex(i) as ContentPresenter;

				        TextBlock t = FindTextBlockWithWrap(cp);

				        if (t != null && t.Visibility == Visibility.Visible)
				        {
					        txtFound = true;
					
					        t.Width = newWidth;
				        }
				        else
				        {
					        break;
				        }

                        TextBox tt = FindTextBoxkWithWrap( cp );

                        if ( tt != null && t.Visibility == Visibility.Visible)
                        {      
                            tt.Width = newWidth2;
                        }
			        }

            resizecouldbedone = true;
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
                foreach (DataRow dtrow in dati.Rows)
                {
                  if (dtrow["ID"].ToString() == IDint)
                  {
                    txtRisultato.Text = "Risultato Check List: " + risultato;
                    return;
                  }           
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
                                  foreach (DataRow dtrow in dati.Rows)
                                  {
                                    if (dtrow["ID"].ToString() == IDint)
                                    {
                                      if (dtrow["value"].ToString() != valore)
                                      {
                                        ok = false;
                                        break;
                                      }
                                    }
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
            if (sender is Image)
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
             if (sender is Label)
            {
                DependencyObject child = ((Label)sender).Parent;

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


        }

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Resizer(Convert.ToInt32(e.NewSize.Width)); 
		}

		private void RadioButton_Checked(object sender, RoutedEventArgs e)
		{
			if (_WithResult)
			{	
             RadioButton rb = sender as RadioButton;
             if (rb != null)
                {
                    if ((bool)rb.IsChecked)
                    {
                       CalcolaCondizione();
                    }
                }
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

            resizecouldbedone = true;
        }
    }
}
