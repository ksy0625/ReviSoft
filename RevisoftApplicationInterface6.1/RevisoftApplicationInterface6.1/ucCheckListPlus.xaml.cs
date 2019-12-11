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
using UserControls;
using System.Data;

namespace UserControls2
{
    public partial class ucCheckListPlus : UserControl
    {
        public int id;
        private DataTable dati = null;

        private XmlDataProviderManager _x;
        public bool txtFound = false;
        private int Offset = 230;
        private int OffsetNote = 0;    
        private int Minimo = 200;
        private string down = "./Images/icone/navigate_down.png";
        private string up = "./Images/icone/navigate_up.png";

        public ucCheckListPlus()
        {
            if (Offset==0 || OffsetNote == 0) { }
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

        public void Load(string ID, string IDCliente, string IDSessione)
        {

            id = int.Parse(ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
            dati = cBusinessObjects.GetData(id, typeof(CheckListPlus));

            Binding b = new Binding();
            b.Source = dati;
            itmDomande.SetBinding(ItemsControl.ItemsSourceProperty, b);

            
        }

		public int Save()
		{
            return cBusinessObjects.SaveData(id, dati, typeof(CheckListPlus));
        }

        private void UserControl_SizeChanged( object sender, SizeChangedEventArgs e )
        {
            Resizer( Convert.ToInt32( e.NewSize.Width ) );
        }

        public void Resizer(int newsize)
        {
            //for ( int i = 0; i < itmDomande.Items.Count; i++ )
            //{
            //    ContentPresenter cp = itmDomande.ItemContainerGenerator.ContainerFromIndex( i ) as ContentPresenter;

            //    TextBox tt = FindTextBoxkWithWrap( cp );

            //    if ( tt != null )
            //    {
            //        int newWidth = newsize - OffsetNote;

            //        if ( newWidth <= Minimo )
            //        {
            //            newWidth = Minimo;
            //        }

            //        if ( tt.Width == newWidth )
            //        {
            //            break;
            //        }

            //        tt.Width = newWidth;
            //    }
            //}
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
    }
}
