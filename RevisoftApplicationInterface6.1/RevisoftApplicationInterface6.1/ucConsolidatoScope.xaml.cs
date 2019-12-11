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

namespace UserControls
{
    public partial class ucConsolidatoScope : UserControl
    {
        private XmlDataProviderManager _x;
        private string _ID = "-1";
		private bool firsttime = true;
        ArrayList dynamicRDB= new ArrayList();
        Hashtable HTNode = new Hashtable();

        private string down = "./Images/icone/navigate_down.png";
        private string left = "./Images/icone/navigate_left.png";

		private bool _ReadOnly = false;

        private Dictionary<string, XmlNode> lista = new Dictionary<string, XmlNode>();

        public ucConsolidatoScope()
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

        public void Load( ref XmlDataProviderManager x, string ID )
        {
			_x = x.Clone();

            _ID = ID;

            XmlNode node = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + ID + "]");
            XmlNode tmp = _x.Document.SelectSingleNode( "/Dati/Dato[@ID=313]" );

            int rowattuale = 0;

            if (tmp.SelectNodes("Valore").Count == 0)
            {
                TextBlock txthere = new TextBlock();
                txthere.Text = "Manca l'indicazione delle Componenti nella Carta di lavoro 3.11.2.";
                gg.Children.Add(txthere);

                return;
            }

            #region assegnazione scope
                                 
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
            tb.Text = "Assegnazione degli Scope";
            
            tb.FontSize = 13;
            tb.FontWeight = FontWeights.Bold;
            tb.Margin = new Thickness( 5.0 );
            tb.Foreground = Brushes.Gray;

            tb.SetValue( Grid.RowProperty, 0 );
            tb.SetValue( Grid.ColumnProperty, 1 );

            g.Children.Add( tb );

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;
            
            StackPanel spheader = new StackPanel();
            spheader.Orientation = Orientation.Horizontal;

            TextBox t = new TextBox();
            t.Width = 200;
            t.FontWeight = FontWeights.Bold;
            t.TextAlignment = TextAlignment.Center;
            t.IsReadOnly = true;
            t.Background = Brushes.LightGray;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);            
            t.Text = "Componenti";
            spheader.Children.Add(t);

            t = new TextBox();
            t.Width = 200;
            t.FontWeight = FontWeights.Bold;
            t.TextAlignment = TextAlignment.Center;
            t.IsReadOnly = true;
            t.Background = Brushes.LightGray;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = "ATTIVO";
            spheader.Children.Add(t);

            t = new TextBox();
            t.Width = 200;
            t.FontWeight = FontWeights.Bold;
            t.TextAlignment = TextAlignment.Center;
            t.IsReadOnly = true;
            t.Background = Brushes.LightGray;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = "VALORE DELLA PRODUZIONE";
            spheader.Children.Add(t);
            
            t = new TextBox();
            t.Width = 150;
            t.FontWeight = FontWeights.Bold;
            t.TextAlignment = TextAlignment.Center;
            t.IsReadOnly = true;
            t.Background = Brushes.LightGray;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = "SCOPE";
            spheader.Children.Add(t);

            sp.Children.Add(spheader);

            //for (int j = 0; j < 1000; j++)
            //{
            //    if (node != null && node.Attributes["name_" + j.ToString()] != null)
            //    {
            //        node.Attributes.Remove(node.Attributes["name_" + j.ToString()]);
            //    }

            //    if (node != null && node.Attributes["attivo_" + j.ToString()] != null)
            //    {
            //        node.Attributes.Remove(node.Attributes["attivo_" + j.ToString()]);
            //    }

            //    if (node != null && node.Attributes["attivo2_" + j.ToString()] != null)
            //    {
            //        node.Attributes.Remove(node.Attributes["attivo2_" + j.ToString()]);
            //    }

            //    if (node != null && node.Attributes["valoreproduzione_" + j.ToString()] != null)
            //    {
            //        node.Attributes.Remove(node.Attributes["valoreproduzione_" + j.ToString()]);
            //    }

            //    if (node != null && node.Attributes["valoreproduzione2_" + j.ToString()] != null)
            //    {
            //        node.Attributes.Remove(node.Attributes["valoreproduzione2_" + j.ToString()]);
            //    }

            //    if (node != null && node.Attributes["scope_" + j.ToString()] != null)
            //    {
            //        node.Attributes.Remove(node.Attributes["scope_" + j.ToString()]);
            //    }
            //}

            int rowhere = 0;

            foreach (XmlNode item in tmp.SelectNodes("Valore"))
            {
                if(item.Attributes["name"] == null)
                {
                    continue;
                }

                StackPanel sprow = new StackPanel();
                sprow.Orientation = Orientation.Horizontal;

                t = new TextBox();
                t.Width = 200;
                t.IsReadOnly = true;
                t.BorderBrush = Brushes.DarkGray;
                t.BorderThickness = new Thickness(1);
                t.Text = item.Attributes["name"].Value;
                sprow.Children.Add(t);

                if (node.Attributes["name_" + rowhere.ToString()] == null)
                {
                    XmlAttribute attr = _x.Document.CreateAttribute("name_" + rowhere.ToString());
                    node.Attributes.Append(attr);
                }
                node.Attributes["name_" + rowhere.ToString()].Value = t.Text;

                t = new TextBox();
                t.Width = 130;
                t.TextAlignment = TextAlignment.Right;
                t.IsReadOnly = true;
                t.BorderBrush = Brushes.DarkGray;
                t.BorderThickness = new Thickness(1);
                t.Text = cBusinessObjects.ConvertNumber( ((item.Attributes["attivo"] == null)? "0" : item.Attributes["attivo"].Value));
                sprow.Children.Add(t);

                if (node.Attributes["attivo_" + rowhere.ToString()] == null)
                {
                    XmlAttribute attr = _x.Document.CreateAttribute("attivo_" + rowhere.ToString());
                    node.Attributes.Append(attr);
                }
                node.Attributes["attivo_" + rowhere.ToString()].Value = t.Text;

                t = new TextBox();
                t.Width = 70;
                t.TextAlignment = TextAlignment.Right;
                t.IsReadOnly = true;
                t.BorderBrush = Brushes.DarkGray;
                t.BorderThickness = new Thickness(1);
                try
                {
                    t.Text = cBusinessObjects.ConvertNumber((Convert.ToDouble(((item.Attributes["attivo"] == null) ? "0" : item.Attributes["attivo"].Value)) / Convert.ToDouble(((tmp.Attributes["attivoTOT"] == null) ? "0" : tmp.Attributes["attivoTOT"].Value)) * 100.0).ToString()) + "%";
                }
                catch (Exception ex)
                {
                    string log = ex.Message;
                }
                sprow.Children.Add(t);

                if (node.Attributes["attivo2_" + rowhere.ToString()] == null)
                {
                    XmlAttribute attr = _x.Document.CreateAttribute("attivo2_" + rowhere.ToString());
                    node.Attributes.Append(attr);
                }
                node.Attributes["attivo2_" + rowhere.ToString()].Value = t.Text;

                t = new TextBox();
                t.Width = 130;
                t.TextAlignment = TextAlignment.Right;
                t.IsReadOnly = true;
                t.BorderBrush = Brushes.DarkGray;
                t.BorderThickness = new Thickness(1);
                t.Text = ((item.Attributes["valoreproduzione"] == null) ? "0" : item.Attributes["valoreproduzione"].Value);
                sprow.Children.Add(t);

                if (node.Attributes["valoreproduzione_" + rowhere.ToString()] == null)
                {
                    XmlAttribute attr = _x.Document.CreateAttribute("valoreproduzione_" + rowhere.ToString());
                    node.Attributes.Append(attr);
                }
                node.Attributes["valoreproduzione_" + rowhere.ToString()].Value = t.Text;

                t = new TextBox();
                t.Width = 70;
                t.TextAlignment = TextAlignment.Right;
                t.IsReadOnly = true;
                t.BorderBrush = Brushes.DarkGray;
                t.BorderThickness = new Thickness(1);
                try
                {
                    t.Text = cBusinessObjects.ConvertNumber((Convert.ToDouble(((item.Attributes["valoreproduzione"] == null) ? "0" : item.Attributes["valoreproduzione"].Value)) / Convert.ToDouble(((tmp.Attributes["valoreproduzioneTOT"] == null) ? "0" : tmp.Attributes["valoreproduzioneTOT"].Value)) * 100.0).ToString()) + "%";
                }
                catch (Exception ex)
                {
                    string log = ex.Message;
                }

                sprow.Children.Add(t);

                if (node.Attributes["valoreproduzione2_" + rowhere.ToString()] == null)
                {
                    XmlAttribute attr = _x.Document.CreateAttribute("valoreproduzione2_" + rowhere.ToString());
                    node.Attributes.Append(attr);
                }
                node.Attributes["valoreproduzione2_" + rowhere.ToString()].Value = t.Text;

                ComboBox cmb = new ComboBox();
                cmb.Width = 150;
                cmb.Tag = rowhere.ToString();
                cmb.Items.Add("Altro");
                cmb.Items.Add("Full Audit");
                cmb.Items.Add("Limited");
                cmb.Items.Add("Desk Review");
                cmb.Items.Add("Specific");
                cmb.PreviewMouseLeftButtonDown += obj_PreviewMouseLeftButtonDown;
                cmb.PreviewKeyDown += obj_PreviewKeyDown;

                if (node.Attributes["scope_" + rowhere.ToString()] != null)
                {
                    cmb.SelectedValue = node.Attributes["scope_" + rowhere.ToString()].Value;
                }
                else
                {
                    cmb.SelectedIndex = 0;
                }
                
                if (node.Attributes["scope_" + rowhere.ToString()] == null)
                {
                    XmlAttribute attr = _x.Document.CreateAttribute("scope_" + rowhere.ToString());
                    node.Attributes.Append(attr);
                }
                node.Attributes["scope_" + rowhere.ToString()].Value = cmb.SelectedValue.ToString();

                cmb.SelectionChanged += Cmb_SelectionChanged;

                sprow.Children.Add(cmb);

                sp.Children.Add(sprow);

                rowhere++;
            }

            StackPanel spfooter = new StackPanel();
            spfooter.Orientation = Orientation.Horizontal;

            t = new TextBox();
            t.Width = 200;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Text = "Totale AGGREGATO";
            t.Background = Brushes.LightYellow;
            spfooter.Children.Add(t);

            if (node.Attributes["name_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("name_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["name_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 130;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.LightYellow;
            t.Text = cBusinessObjects.ConvertNumber(((tmp.Attributes["attivoTOT"] == null) ? "0" : tmp.Attributes["attivoTOT"].Value));
            spfooter.Children.Add(t);

            if (node.Attributes["attivo_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("attivo_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["attivo_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 70;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.LightYellow;
            t.Text = "";
            spfooter.Children.Add(t);

            if (node.Attributes["attivo2_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("attivo2_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["attivo2_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 130;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.LightYellow;
            t.Text = cBusinessObjects.ConvertNumber(((tmp.Attributes["valoreproduzioneTOT"] == null) ? "0" : tmp.Attributes["valoreproduzioneTOT"].Value));
            spfooter.Children.Add(t);

            if (node.Attributes["valoreproduzione_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("valoreproduzione_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["valoreproduzione_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 70;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.LightYellow;
            t.Text = "";
            spfooter.Children.Add(t);

            if (node.Attributes["valoreproduzione2_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("valoreproduzione2_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["valoreproduzione2_" + rowhere.ToString()].Value = t.Text;

            if (node.Attributes["scope_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_" + rowhere.ToString()].Value = "";

            sp.Children.Add(spfooter);

            rowhere++;
            spfooter = new StackPanel();
            spfooter.Orientation = Orientation.Horizontal;

            t = new TextBox();
            t.Width = 200;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = "Totale scritture consolidamento";
            spfooter.Children.Add(t);

            if (node.Attributes["name_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("name_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["name_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 130;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = cBusinessObjects.ConvertNumber(((tmp.Attributes["attivo2"] == null) ? "0" : tmp.Attributes["attivo2"].Value));
            spfooter.Children.Add(t);

            if (node.Attributes["attivo_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("attivo_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["attivo_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 70;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = "";
            spfooter.Children.Add(t);

            if (node.Attributes["attivo2_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("attivo2_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["attivo2_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 130;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = cBusinessObjects.ConvertNumber(((tmp.Attributes["valoreproduzione2"] == null) ? "0" : tmp.Attributes["valoreproduzione2"].Value));
            spfooter.Children.Add(t);

            if (node.Attributes["valoreproduzione_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("valoreproduzione_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["valoreproduzione_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 70;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = "";
            spfooter.Children.Add(t);

            if (node.Attributes["valoreproduzione2_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("valoreproduzione2_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["valoreproduzione2_" + rowhere.ToString()].Value = t.Text;

            if (node.Attributes["scope_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_" + rowhere.ToString()].Value = "";

            sp.Children.Add(spfooter);

            rowhere++;
            spfooter = new StackPanel();
            spfooter.Orientation = Orientation.Horizontal;

            t = new TextBox();
            t.Width = 200;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Text = "Totale CONSOLIDATO";
            t.Background = Brushes.LightYellow;
            spfooter.Children.Add(t);

            if (node.Attributes["name_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("name_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["name_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 130;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.LightYellow;
            t.Text = cBusinessObjects.ConvertNumber(((tmp.Attributes["attivo"] == null) ? "0" : tmp.Attributes["attivo"].Value));
            spfooter.Children.Add(t);

            if (node.Attributes["attivo_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("attivo_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["attivo_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 70;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.LightYellow;
            t.Text = "";
            spfooter.Children.Add(t);

            if (node.Attributes["attivo2_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("attivo2_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["attivo2_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 130;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.LightYellow;
            t.Text = cBusinessObjects.ConvertNumber(((tmp.Attributes["valoreproduzione"] == null) ? "0" : tmp.Attributes["valoreproduzione"].Value));
            spfooter.Children.Add(t);

            if (node.Attributes["valoreproduzione_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("valoreproduzione_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["valoreproduzione_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 70;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.LightYellow;
            t.Text = "";
            spfooter.Children.Add(t);

            if (node.Attributes["valoreproduzione2_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("valoreproduzione2_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["valoreproduzione2_" + rowhere.ToString()].Value = t.Text;

            if (node.Attributes["scope_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_" + rowhere.ToString()].Value = "";

            sp.Children.Add(spfooter);

            sp.SetValue(Grid.RowProperty, 1);
            sp.SetValue(Grid.ColumnProperty, 1);

            sp.Visibility = System.Windows.Visibility.Collapsed;
            uriSource = new Uri(left, UriKind.Relative);
            ((Image)(g.Children[0])).Source = new BitmapImage(uriSource);

            g.Children.Add(sp);

            b.Child = g;

            RowDefinition rdg = new RowDefinition();
            gg.RowDefinitions.Add(rdg);

            b.SetValue(Grid.RowProperty, rowattuale);
            b.SetValue(Grid.ColumnProperty, 0);

            rowattuale++;

            gg.Children.Add(b);

            #endregion

            #region rischio specifico
            b = new Border();
            b.CornerRadius = new CornerRadius(5.0);
            b.BorderBrush = Brushes.LightGray;
            b.BorderThickness = new Thickness(1.0);
            b.Padding = new Thickness(4.0);
            b.Margin = new Thickness(4.0);

            g = new Grid();

            cd = new ColumnDefinition();
            cd.Width = new GridLength(15.0);
            g.ColumnDefinitions.Add(cd);

            cd = new ColumnDefinition();
            cd.Width = GridLength.Auto;
            g.ColumnDefinitions.Add(cd);

            g.RowDefinitions.Add(new RowDefinition());
            g.RowDefinitions.Add(new RowDefinition());

            i = new Image();
            i.SetValue(Grid.RowProperty, 0);
            i.SetValue(Grid.ColumnProperty, 0);

            uriSource = new Uri(left, UriKind.Relative);
            i.Source = new BitmapImage(uriSource);
            i.Height = 10.0;
            i.Width = 10.0;
            i.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

            g.Children.Add(i);

            tb = new TextBlock();
            tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            tb.Text = "Rischio Specifico";
            
            tb.FontSize = 13;
            tb.FontWeight = FontWeights.Bold;
            tb.Margin = new Thickness(5.0);
            tb.Foreground = Brushes.Gray;

            tb.SetValue(Grid.RowProperty, 0);
            tb.SetValue(Grid.ColumnProperty, 1);

            g.Children.Add(tb);

            sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;

            spheader = new StackPanel();
            spheader.Orientation = Orientation.Horizontal;

            t = new TextBox();
            t.Width = 200;
            t.FontWeight = FontWeights.Bold;
            t.TextAlignment = TextAlignment.Center;
            t.IsReadOnly = true;
            t.Background = Brushes.LightGray;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = "Componenti";
            spheader.Children.Add(t);

            t = new TextBox();
            t.Width = 600;
            t.FontWeight = FontWeights.Bold;
            t.TextAlignment = TextAlignment.Center;
            t.IsReadOnly = true;
            t.Background = Brushes.LightGray;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = "Rischio Specifico";
            spheader.Children.Add(t);
            
            sp.Children.Add(spheader);

            rowhere = 0;

            foreach (XmlNode item in tmp.SelectNodes("Valore"))
            {
                if (item.Attributes["name"] == null)
                {
                    continue;
                }

                StackPanel sprow = new StackPanel();
                sprow.Orientation = Orientation.Horizontal;

                t = new TextBox();
                t.Width = 200;
                t.IsReadOnly = true;
                t.BorderBrush = Brushes.DarkGray;
                t.BorderThickness = new Thickness(1);
                t.Text = item.Attributes["name"].Value;
                sprow.Children.Add(t);

                if (node.Attributes["name_" + rowhere.ToString()] == null)
                {
                    XmlAttribute attr = _x.Document.CreateAttribute("name_" + rowhere.ToString());
                    node.Attributes.Append(attr);
                }
                node.Attributes["name_" + rowhere.ToString()].Value = t.Text;
                
                t = new TextBox();
                t.Width = 600;
                t.TextWrapping = TextWrapping.Wrap;
                t.Tag = rowhere.ToString();
                t.BorderBrush = Brushes.DarkGray;
                t.BorderThickness = new Thickness(1);
                t.LostFocus += T_LostFocus;
                t.Text = ((node.Attributes["nota_" + rowhere.ToString()] == null)? "" : node.Attributes["nota_" + rowhere.ToString()].Value);
                sprow.Children.Add(t);
                
                sp.Children.Add(sprow);

                rowhere++;
            }

            sp.SetValue(Grid.RowProperty, 1);
            sp.SetValue(Grid.ColumnProperty, 1);

            sp.Visibility = System.Windows.Visibility.Collapsed;
            uriSource = new Uri(left, UriKind.Relative);
            ((Image)(g.Children[0])).Source = new BitmapImage(uriSource);

            g.Children.Add(sp);

            b.Child = g;

            rdg = new RowDefinition();
            gg.RowDefinitions.Add(rdg);

            b.SetValue(Grid.RowProperty, rowattuale);
            b.SetValue(Grid.ColumnProperty, 0);

            rowattuale++;

            gg.Children.Add(b);

            #endregion

            #region raggruppamento per scope
            b = new Border();
            b.CornerRadius = new CornerRadius(5.0);
            b.BorderBrush = Brushes.LightGray;
            b.BorderThickness = new Thickness(1.0);
            b.Padding = new Thickness(4.0);
            b.Margin = new Thickness(4.0);

            g = new Grid();

            cd = new ColumnDefinition();
            cd.Width = new GridLength(15.0);
            g.ColumnDefinitions.Add(cd);

            cd = new ColumnDefinition();
            cd.Width = GridLength.Auto;
            g.ColumnDefinitions.Add(cd);

            g.RowDefinitions.Add(new RowDefinition());
            g.RowDefinitions.Add(new RowDefinition());

            i = new Image();
            i.SetValue(Grid.RowProperty, 0);
            i.SetValue(Grid.ColumnProperty, 0);

            uriSource = new Uri(left, UriKind.Relative);
            i.Source = new BitmapImage(uriSource);
            i.Height = 10.0;
            i.Width = 10.0;
            i.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

            g.Children.Add(i);

            tb = new TextBlock();
            tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            tb.Text = "Raggruppamento per Scope";
            
            tb.FontSize = 13;
            tb.FontWeight = FontWeights.Bold;
            tb.Margin = new Thickness(5.0);
            tb.Foreground = Brushes.Gray;

            tb.SetValue(Grid.RowProperty, 0);
            tb.SetValue(Grid.ColumnProperty, 1);

            g.Children.Add(tb);

            sp = new StackPanel();
            sp.Name = "stpScope";
            try
            {
                this.UnregisterName(sp.Name);
            }
            catch (Exception ex)
            {
                string log = ex.Message;
            }
            this.RegisterName(sp.Name, sp);

            sp.Orientation = Orientation.Vertical;

            calculatescope();

            sp.SetValue(Grid.RowProperty, 1);
            sp.SetValue(Grid.ColumnProperty, 1);

            sp.Visibility = System.Windows.Visibility.Collapsed;
            uriSource = new Uri(left, UriKind.Relative);
            ((Image)(g.Children[0])).Source = new BitmapImage(uriSource);

            g.Children.Add(sp);

            b.Child = g;

            rdg = new RowDefinition();
            gg.RowDefinitions.Add(rdg);

            b.SetValue(Grid.RowProperty, rowattuale);
            b.SetValue(Grid.ColumnProperty, 0);

            rowattuale++;

            gg.Children.Add(b);

            #endregion

            _x.Save();
        }

        private void calculatescope()
        {
            XmlNode node = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]");
            XmlNode tmp = _x.Document.SelectSingleNode("/Dati/Dato[@ID=313]");

            StackPanel sp = (StackPanel)this.FindName("stpScope");
            sp.Children.Clear();

            StackPanel spheader = new StackPanel();
            spheader.Orientation = Orientation.Horizontal;

            TextBox t = new TextBox();
            t.Width = 200;
            t.FontWeight = FontWeights.Bold;
            t.TextAlignment = TextAlignment.Center;
            t.IsReadOnly = true;
            t.Background = Brushes.LightGray;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = "SCOPE";
            spheader.Children.Add(t);

            t = new TextBox();
            t.Width = 200;
            t.FontWeight = FontWeights.Bold;
            t.TextAlignment = TextAlignment.Center;
            t.IsReadOnly = true;
            t.Background = Brushes.LightGray;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = "ATTIVO";
            spheader.Children.Add(t);

            t = new TextBox();
            t.Width = 200;
            t.FontWeight = FontWeights.Bold;
            t.TextAlignment = TextAlignment.Center;
            t.IsReadOnly = true;
            t.Background = Brushes.LightGray;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = "VALORE DELLA PRODUZIONE";
            spheader.Children.Add(t);

            sp.Children.Add(spheader);

            for (int j = 0; j < 1000; j++)
            {
                if (node != null && node.Attributes["scope_name_" + j.ToString()] != null)
                {
                    node.Attributes.Remove(node.Attributes["scope_name_" + j.ToString()]);
                }

                if (node != null && node.Attributes["scope_attivo_" + j.ToString()] != null)
                {
                    node.Attributes.Remove(node.Attributes["scope_attivo_" + j.ToString()]);
                }

                if (node != null && node.Attributes["scope_attivo2_" + j.ToString()] != null)
                {
                    node.Attributes.Remove(node.Attributes["scope_attivo2_" + j.ToString()]);
                }

                if (node != null && node.Attributes["scope_valoreproduzione_" + j.ToString()] != null)
                {
                    node.Attributes.Remove(node.Attributes["scope_valoreproduzione_" + j.ToString()]);
                }

                if (node != null && node.Attributes["scope_valoreproduzione2_" + j.ToString()] != null)
                {
                    node.Attributes.Remove(node.Attributes["scope_valoreproduzione2_" + j.ToString()]);
                }
            }

            Hashtable anow = new Hashtable();
            Hashtable pnow = new Hashtable();

            int rowhere = 0;

            for (int j = 0; j < 1000; j++)
            {
                if (node.Attributes["scope_" + j.ToString()] == null || node.Attributes["scope_" + j.ToString()].Value == "")
                {
                    break;
                }

                if (anow.ContainsKey(node.Attributes["scope_" + j.ToString()].Value))
                {
                    anow[node.Attributes["scope_" + j.ToString()].Value] = (double)(anow[node.Attributes["scope_" + j.ToString()].Value]) + Convert.ToDouble(((node.Attributes["attivo_" + j.ToString()] == null || node.Attributes["attivo_" + j.ToString()].Value == "") ? "0" : node.Attributes["attivo_" + j.ToString()].Value));
                }
                else
                {
                    anow.Add(node.Attributes["scope_" + j.ToString()].Value, Convert.ToDouble(((node.Attributes["attivo_" + j.ToString()] == null || node.Attributes["attivo_" + j.ToString()].Value == "") ? "0" : node.Attributes["attivo_" + j.ToString()].Value)));
                }

                if (pnow.ContainsKey(node.Attributes["scope_" + j.ToString()].Value))
                {
                    pnow[node.Attributes["scope_" + j.ToString()].Value] = (double)(pnow[node.Attributes["scope_" + j.ToString()].Value]) + Convert.ToDouble(((node.Attributes["valoreproduzione_" + j.ToString()] == null || node.Attributes["valoreproduzione_" + j.ToString()].Value == "") ? "0" : node.Attributes["valoreproduzione_" + j.ToString()].Value));
                }
                else
                {
                    pnow.Add(node.Attributes["scope_" + j.ToString()].Value, Convert.ToDouble(((node.Attributes["valoreproduzione_" + j.ToString()] == null || node.Attributes["valoreproduzione_" + j.ToString()].Value == "") ? "0" : node.Attributes["valoreproduzione_" + j.ToString()].Value)));
                }
            }

            foreach (DictionaryEntry item in anow)
            {
                StackPanel sprow = new StackPanel();
                sprow.Orientation = Orientation.Horizontal;

                t = new TextBox();
                t.Width = 200;
                t.IsReadOnly = true;
                t.BorderBrush = Brushes.DarkGray;
                t.BorderThickness = new Thickness(1);
                t.Text = item.Key.ToString();
                sprow.Children.Add(t);

                if (node.Attributes["scope_name_" + rowhere.ToString()] == null)
                {
                    XmlAttribute attr = _x.Document.CreateAttribute("scope_name_" + rowhere.ToString());
                    node.Attributes.Append(attr);
                }
                node.Attributes["scope_name_" + rowhere.ToString()].Value = t.Text;

                t = new TextBox();
                t.Width = 130;
                t.TextAlignment = TextAlignment.Right;
                t.IsReadOnly = true;
                t.BorderBrush = Brushes.DarkGray;
                t.BorderThickness = new Thickness(1);
                t.Text = cBusinessObjects.ConvertNumber(((double)(item.Value)).ToString());
                sprow.Children.Add(t);

                if (node.Attributes["scope_attivo_" + rowhere.ToString()] == null)
                {
                    XmlAttribute attr = _x.Document.CreateAttribute("scope_attivo_" + rowhere.ToString());
                    node.Attributes.Append(attr);
                }
                node.Attributes["scope_attivo_" + rowhere.ToString()].Value = t.Text;

                t = new TextBox();
                t.Width = 70;
                t.TextAlignment = TextAlignment.Right;
                t.IsReadOnly = true;
                t.BorderBrush = Brushes.DarkGray;
                t.BorderThickness = new Thickness(1);
                try
                {
                    t.Text = cBusinessObjects.ConvertNumber((((double)(item.Value)) / Convert.ToDouble(((tmp.Attributes["attivoTOT"] == null) ? "0" : tmp.Attributes["attivoTOT"].Value)) * 100.0).ToString()) + "%";
                }
                catch (Exception ex)
                {
                    string log = ex.Message;
                }
                
                sprow.Children.Add(t);

                if (node.Attributes["scope_attivo2_" + rowhere.ToString()] == null)
                {
                    XmlAttribute attr = _x.Document.CreateAttribute("scope_attivo2_" + rowhere.ToString());
                    node.Attributes.Append(attr);
                }
                node.Attributes["scope_attivo2_" + rowhere.ToString()].Value = t.Text;

                t = new TextBox();
                t.Width = 130;
                t.TextAlignment = TextAlignment.Right;
                t.IsReadOnly = true;
                t.BorderBrush = Brushes.DarkGray;
                t.BorderThickness = new Thickness(1);
                t.Text = cBusinessObjects.ConvertNumber(((double)(pnow[item.Key.ToString()])).ToString());
                sprow.Children.Add(t);

                if (node.Attributes["scope_valoreproduzione_" + rowhere.ToString()] == null)
                {
                    XmlAttribute attr = _x.Document.CreateAttribute("scope_valoreproduzione_" + rowhere.ToString());
                    node.Attributes.Append(attr);
                }
                node.Attributes["scope_valoreproduzione_" + rowhere.ToString()].Value = t.Text;

                t = new TextBox();
                t.Width = 70;
                t.TextAlignment = TextAlignment.Right;
                t.IsReadOnly = true;
                t.BorderBrush = Brushes.DarkGray;
                t.BorderThickness = new Thickness(1);
                try
                {
                    t.Text = cBusinessObjects.ConvertNumber((((double)(pnow[item.Key.ToString()])) / Convert.ToDouble(((tmp.Attributes["valoreproduzioneTOT"] == null) ? "0" : tmp.Attributes["valoreproduzioneTOT"].Value)) * 100.0).ToString()) + "%";

                }
                catch (Exception ex)
                {
                    string log = ex.Message;
                }

                sprow.Children.Add(t);

                if (node.Attributes["scope_valoreproduzione2_" + rowhere.ToString()] == null)
                {
                    XmlAttribute attr = _x.Document.CreateAttribute("scope_valoreproduzione2_" + rowhere.ToString());
                    node.Attributes.Append(attr);
                }
                node.Attributes["scope_valoreproduzione2_" + rowhere.ToString()].Value = t.Text;

                sp.Children.Add(sprow);

                rowhere++;
            }

            StackPanel spfooter = new StackPanel();
            spfooter.Orientation = Orientation.Horizontal;

            t = new TextBox();
            t.Width = 200;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Text = "Totale AGGREGATO";
            t.Background = Brushes.LightYellow;
            spfooter.Children.Add(t);

            if (node.Attributes["scope_name_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_name_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_name_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 130;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.LightYellow;
            t.Text = cBusinessObjects.ConvertNumber(((tmp.Attributes["attivoTOT"] == null) ? "0" : tmp.Attributes["attivoTOT"].Value));
            spfooter.Children.Add(t);

            if (node.Attributes["scope_attivo_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_attivo_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_attivo_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 70;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.LightYellow;
            t.Text = "";
            spfooter.Children.Add(t);

            if (node.Attributes["scope_attivo2_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_attivo2_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_attivo2_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 130;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.LightYellow;
            try
            { 
                t.Text = cBusinessObjects.ConvertNumber(((tmp.Attributes["valoreproduzioneTOT"] == null) ? "0" : tmp.Attributes["valoreproduzioneTOT"].Value));
            }
            catch (Exception ex)
            {
                string log = ex.Message;
            }

            spfooter.Children.Add(t);

            if (node.Attributes["scope_valoreproduzione_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_valoreproduzione_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_valoreproduzione_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 70;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.LightYellow;
            t.Text = "";
            spfooter.Children.Add(t);

            if (node.Attributes["scope_valoreproduzione2_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_valoreproduzione2_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_valoreproduzione2_" + rowhere.ToString()].Value = t.Text;

            sp.Children.Add(spfooter);

            rowhere++;
            spfooter = new StackPanel();
            spfooter.Orientation = Orientation.Horizontal;

            t = new TextBox();
            t.Width = 200;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = "Totale scritture consolidamento";
            spfooter.Children.Add(t);

            if (node.Attributes["scope_name_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_name_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_name_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 130;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = cBusinessObjects.ConvertNumber(((tmp.Attributes["attivo2"] == null) ? "0" : tmp.Attributes["attivo2"].Value));
            spfooter.Children.Add(t);

            if (node.Attributes["scope_attivo_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_attivo_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_attivo_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 70;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = "";
            spfooter.Children.Add(t);

            if (node.Attributes["scope_attivo2_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_attivo2_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_attivo2_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 130;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = cBusinessObjects.ConvertNumber(((tmp.Attributes["valoreproduzione2"] == null) ? "0" : tmp.Attributes["valoreproduzione2"].Value));
            spfooter.Children.Add(t);

            if (node.Attributes["scope_valoreproduzione_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_valoreproduzione_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_valoreproduzione_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 70;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.Text = "";
            spfooter.Children.Add(t);

            if (node.Attributes["scope_valoreproduzione2_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_valoreproduzione2_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_valoreproduzione2_" + rowhere.ToString()].Value = t.Text;

            sp.Children.Add(spfooter);

            rowhere++;
            spfooter = new StackPanel();
            spfooter.Orientation = Orientation.Horizontal;

            t = new TextBox();
            t.Width = 200;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Text = "Totale CONSOLIDATO";
            t.Background = Brushes.LightYellow;
            spfooter.Children.Add(t);

            if (node.Attributes["scope_name_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_name_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_name_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 130;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.LightYellow;
            t.Text = cBusinessObjects.ConvertNumber(((tmp.Attributes["attivo"] == null) ? "0" : tmp.Attributes["attivo"].Value));
            spfooter.Children.Add(t);

            if (node.Attributes["scope_attivo_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_attivo_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_attivo_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 70;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.LightYellow;
            t.Text = "";
            spfooter.Children.Add(t);

            if (node.Attributes["scope_attivo2_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_attivo2_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_attivo2_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 130;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.LightYellow;
            t.Text = cBusinessObjects.ConvertNumber(((tmp.Attributes["valoreproduzione"] == null) ? "0" : tmp.Attributes["valoreproduzione"].Value));
            spfooter.Children.Add(t);

            if (node.Attributes["scope_valoreproduzione_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_valoreproduzione_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_valoreproduzione_" + rowhere.ToString()].Value = t.Text;

            t = new TextBox();
            t.Width = 70;
            t.TextAlignment = TextAlignment.Right;
            t.IsReadOnly = true;
            t.BorderBrush = Brushes.DarkGray;
            t.BorderThickness = new Thickness(1);
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.LightYellow;
            t.Text = "";
            spfooter.Children.Add(t);

            if (node.Attributes["scope_valoreproduzione2_" + rowhere.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_valoreproduzione2_" + rowhere.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_valoreproduzione2_" + rowhere.ToString()].Value = t.Text;

            sp.Children.Add(spfooter);
        }

        private void T_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox cmb = (TextBox)sender;

            XmlNode node = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]");

            if (node.Attributes["nota_" + cmb.Tag.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("nota_" + cmb.Tag.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["nota_" + cmb.Tag.ToString()].Value = cmb.Text;

            _x.Save();
        }

        private void Cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;

            if (cmb.SelectedValue == null)
            {
                return;
            }

            XmlNode node = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]");

            if (node.Attributes["scope_" + cmb.Tag.ToString()] == null)
            {
                XmlAttribute attr = _x.Document.CreateAttribute("scope_" + cmb.Tag.ToString());
                node.Attributes.Append(attr);
            }
            node.Attributes["scope_" + cmb.Tag.ToString()].Value = cmb.SelectedValue.ToString();

            _x.Save();

            calculatescope();
        }

        public XmlDataProviderManager Save()
		{
            return _x;
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
