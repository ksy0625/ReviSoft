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
using UserControls2;
using System.Data;



namespace UserControls
{
    public partial class ucDiscussioniTeam : UserControl
    {

        public int id;
        private DataTable dati = null;

        private string down = "./Images/icone/navigate_down.png";
        private string left = "./Images/icone/navigate_left.png";

        private XmlDataProviderManager _x;
        private string _ID;

		private Hashtable objList = new Hashtable();

        public WindowWorkArea Owner;

        public ucDiscussioniTeam()
        {
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
            id = int.Parse(ID);
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());

        

            _ID = ID;
            
            stack.Children.Clear();
            objList = new Hashtable();

            StackPanel sp2 = new StackPanel();
            sp2.Orientation = Orientation.Horizontal;
            sp2.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

            Button btnDuplica = new Button();
            btnDuplica.Padding = new Thickness(5.0);
            btnDuplica.Content = "Crea Nuova Discussione";
            btnDuplica.Click += btnDuplica_Click;

            sp2.Children.Add( btnDuplica );

            stack.Children.Add( sp2 );

            dati = cBusinessObjects.GetData(id, typeof(DiscussioniTeam));

            foreach (DataRow dtrow in dati.Rows)
            {
                Border b = new Border();
                b.CornerRadius = new CornerRadius(5.0);
                b.BorderBrush = Brushes.LightGray;
                b.BorderThickness = new Thickness(1.0);
                b.Padding = new Thickness(4.0);
                b.Margin = new Thickness(4.0); 

                Grid g = new Grid();

                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(15.0);
                g.ColumnDefinitions.Add(cd);

                cd = new ColumnDefinition();
                cd.Width = GridLength.Auto;
                g.ColumnDefinitions.Add(cd);

                g.RowDefinitions.Add(new RowDefinition());
                g.RowDefinitions.Add(new RowDefinition());

                Image i = new Image();
                i.SetValue(Grid.RowProperty, 0); 
                i.SetValue(Grid.ColumnProperty, 0);

                var uriSource = new Uri( ((dtrow["Chiuso"] != null && dtrow["Chiuso"].ToString() == "True")? left: down), UriKind.Relative );
                i.Source = new BitmapImage(uriSource);
                i.Height = 10.0;
                i.Width = 10.0;
                i.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

                g.Children.Add(i);

                StackPanel sp3 = new StackPanel();
                sp3.Orientation = Orientation.Horizontal;

				TextBlock tb = new TextBlock();
				tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
				tb.Text = dtrow["data"].ToString();
                tb.Tag = dtrow["ID"].ToString();
                
                tb.FontSize = 13;
                tb.FontWeight = FontWeights.Bold;
                tb.Margin = new Thickness(5.0);
                tb.Foreground = Brushes.Gray;

                sp3.Children.Add(tb);

                Button btnRinominaDuplica = new Button();
                btnRinominaDuplica.Padding = new Thickness(5.0);
                btnRinominaDuplica.Content = "Rinomina Discussione";
                btnRinominaDuplica.Margin = new Thickness(10, 0, 0, 0);
                btnRinominaDuplica.Tag = dtrow["ID"].ToString();
                btnRinominaDuplica.Click += btnRinominaDuplica_Click;

                sp3.Children.Add(btnRinominaDuplica);

                Button btnEliminaDuplica = new Button();
                btnEliminaDuplica.Padding = new Thickness(5.0);
                btnEliminaDuplica.Content = "Elimina Discussione";
                btnEliminaDuplica.Margin = new Thickness(10, 0, 0, 0);
                btnEliminaDuplica.Tag = dtrow["ID"].ToString();
                btnEliminaDuplica.Click += btnEliminaDuplica_Click;

                sp3.Children.Add(btnEliminaDuplica);

                sp3.SetValue(Grid.RowProperty, 0);
                sp3.SetValue(Grid.ColumnProperty, 1);               

                g.Children.Add(sp3);
                
                ucTestoFromNode Testo = new ucTestoFromNode();
                Testo.ReadOnly = _ReadOnly;
                Testo.Load(dtrow["name"].ToString());

                Testo.SetValue(Grid.RowProperty, 1);
                Testo.SetValue(Grid.ColumnProperty, 1);

                Testo.Visibility = System.Windows.Visibility.Visible;
                uriSource = new Uri(down, UriKind.Relative);
                ((Image)(g.Children[0])).Source = new BitmapImage(uriSource);
               
                objList.Add(dtrow["ID"].ToString(), Testo);
                if (dtrow["Chiuso"] != null && dtrow["Chiuso"].ToString() == "True")
                {
                    Testo.Visibility = System.Windows.Visibility.Collapsed;
                }
                g.Children.Add(Testo);

                b.Child = g;

                stack.Children.Add(b);
            }

            foreach ( UIElement item in stack.Children )
            {
                try
                {
                    ( (UserControl)( ( (Grid)( ( (Border)( item ) ).Child ) ).Children[2] ) ).Width = stack.Width - 30;
                }
                catch ( Exception ex )
                {
                    string log = ex.Message;
                }
            }
        }
                
        void btnRinominaDuplica_Click(object sender, RoutedEventArgs e)
        {
            if (_ReadOnly)
            {
                MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
                return;
            }

            var dialog = new wInputBox("Inserire nuova data della discussione");
            dialog.ShowDialog();
            if (!dialog.diagres)
            {
                return;
            }

            if (dialog.ResponseText.Trim() == "")
            {
                return;
            }

            string newtitle = dialog.ResponseText;
            foreach (DataRow dtrow in dati.Rows)
            {
                if(dtrow["ID"].ToString() == ((Button)(sender)).Tag.ToString())
                   dtrow["data"] = newtitle;
             
            }
            cBusinessObjects.SaveData(id, dati, typeof(DiscussioniTeam));
            Load( _ID,cBusinessObjects.idcliente.ToString(),cBusinessObjects.idsessione.ToString());
        }

        void btnEliminaDuplica_Click( object sender, RoutedEventArgs e )
        {
            if (_ReadOnly)
            {
                MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
                return;
            }

            if (MessageBox.Show("Attenzione La discussione verrà eliminata. si vuole procedere?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }

       
            foreach (DataRow dtrow in this.dati.Rows)
            {
                if (dtrow["ID"].ToString() == ((Button)(sender)).Tag.ToString())
                {   
                    dtrow.Delete();
                    break;
                }
            }
            dati.AcceptChanges();
            cBusinessObjects.SaveData(id, dati, typeof(DiscussioniTeam));
            Load(_ID, cBusinessObjects.idcliente.ToString(), cBusinessObjects.idsessione.ToString());
        }

        void btnDuplica_Click( object sender, RoutedEventArgs e )
        {
            if ( _ReadOnly )
            {
                MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione" );
                return;
            }

            var dialog = new wInputBox("Inserire Data della Discussione");
            dialog.ShowDialog();
            if (!dialog.diagres)
            {
                return;
            }
            if (dialog.ResponseText.Trim() == "")
            {
                return;
            }

            string newtitle = dialog.ResponseText;

            int lastID = 0;
            foreach (DataRow dtrow in dati.Rows)
            {
                int IDHere = Convert.ToInt32(dtrow["ID"].ToString());
                if (IDHere > lastID)
                {
                    lastID = IDHere;
                }
            }

            lastID++;
       
            dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, newtitle,"false", lastID);
            cBusinessObjects.SaveData(id, dati, typeof(DiscussioniTeam));

            Load(_ID, cBusinessObjects.idcliente.ToString(), cBusinessObjects.idsessione.ToString());
        }

        public int Save()
        {
           
            foreach (DictionaryEntry item in objList)
            {
                foreach (DataRow dtrow in dati.Rows)
                {
                    if(dtrow["ID"].ToString() == item.Key.ToString())
                        dtrow["name"] = ((ucTestoFromNode)(item.Value)).Save();
                }
            }
            return cBusinessObjects.SaveData(id, dati, typeof(DiscussioniTeam));
        }

        

		private string ConvertInteger(string valore)
		{
			double dblValore = 0.0;

			double.TryParse(valore, out dblValore);

			if (dblValore == 0.0)
			{
				return "";
			}
			else
			{
				return String.Format("{0:#,0}", dblValore);
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
				return String.Format("{0:#,0}", dblValore);
			}
		}

		private string ConvertPercent(string valore)
		{
			double dblValore = 0.0;

			double.TryParse(valore, out dblValore);

			dblValore = dblValore * 100.0;

			if (dblValore == 0.0)
			{
				return "0,00%";
			}
			else
			{
				return String.Format("{0:0.00}", dblValore) + "%";
			}
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double newsize = e.NewSize.Width - 30.0;
							
			foreach (UIElement item in stack.Children)
			{
				try
				{
					((UserControl)(((Grid)(((Border)(item)).Child)).Children[2])).Width = newsize - 30;
				}
				catch (Exception ex)
				{
					string log = ex.Message;
				}
			}

			try
			{
				stack.Width = Convert.ToDouble(newsize);
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}
		}

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image i = ((Image)sender);

			try
			{
				UserControl u = ((UserControl)(((Grid)(i.Parent)).Children[2]));

				if (u.Visibility == System.Windows.Visibility.Collapsed)
				{
					u.Visibility = System.Windows.Visibility.Visible;

                    _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + _ID + "']/Valore[@ID='" + ((TextBlock)(((StackPanel)(((Grid)(i.Parent)).Children[1])).Children[0])).Tag.ToString() + "']").Attributes["Chiuso"].Value = "False";

					var uriSource = new Uri(down, UriKind.Relative);
					i.Source = new BitmapImage(uriSource);
				}
				else
				{
                    _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + _ID + "']/Valore[@ID='" + ((TextBlock)(((StackPanel)(((Grid)(i.Parent)).Children[1])).Children[0])).Tag.ToString() + "']").Attributes["Chiuso"].Value = "True";

                    u.Visibility = System.Windows.Visibility.Collapsed;
					var uriSource = new Uri(left, UriKind.Relative);
					i.Source = new BitmapImage(uriSource);
				}
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}           
        }
    }
}
