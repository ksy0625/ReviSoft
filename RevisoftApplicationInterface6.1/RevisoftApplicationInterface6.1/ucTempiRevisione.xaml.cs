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
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Data;

namespace UserControls
{ 

    public partial class ucTempiRevisione : UserControl
    {

        public int id;
        private DataTable dati = null;

        Hashtable htData = new Hashtable();
        int numcolumns = 4;
        string[] columnsAlias = { "Fasi", "Attività da Svolgere", "Esecutore - Personale Assegnato", "Ore" };
        string[] columnsValues = { "fase", "attivita", "esecutore", "ore" };
        double[] columnsWidth = { 1.0, 4.0, 2.0, 1.0 };
        double totalwidth = 8.0;
        double[] columnsMinWidth = { 0.0, 0.0, 0.0, 0.0 };
        string[] columnsTypes = { "string", "string", "string", "money" };
        string[] columnsAlignment = { "left", "left", "left", "right" };
        bool[] conditionalReadonly = { true, true, false, false };
        //string conditionalAttribute = "new";
        bool[] columnsHasTotal = { false, false, false, true };        
      


        private int Offset = 260;
        private int OffsetNote = 270 + 1000;
        private int Minimo = 200;

		private string check = "./Images/icone/check2-24x24.png";
		private string uncheck = "./Images/icone/check1-24x24.png";

		private string up = "./Images/icone/navigate_up.png";
		private string down = "./Images/icone/navigate_down.png";
		private string left = "./Images/icone/navigate_left.png";

		//private XmlDataProviderManager _x;
        private string _ID = "-1";
		private string IDCompensiERisorse= "42";

		private bool _ReadOnly = false;
        private bool _StartingCalculation = true;
        
        public bool ReadOnly 
        {
            set
            {
				_ReadOnly = value;
            }
        }

        public ucTempiRevisione()
        {
            if (Offset==0 || OffsetNote==0 || Minimo==0 || check.Equals("")
                || uncheck.Equals("") || up.Equals("") || IDCompensiERisorse.Equals("")) { }
            InitializeComponent();            
        }

        public void Load( string ID, string IDCliente, string IDSessione)
        {
            id = int.Parse(ID);
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());

            dati = cBusinessObjects.GetData(id, typeof(TempiRevisione));
   
            _ID = ID ;

          
            _StartingCalculation = false;

         
            GenerateTable(tblMainContainer,  numcolumns, columnsAlias, columnsValues, columnsWidth, conditionalReadonly);
            GenerateTotal();
        }


        private void GenerateTable(Grid gridcontainer,  int numcolumn, string[] columnsAlias, string[] columnsValues, double[] columnsWidth, bool[] conditionalReadonly)
        {
            gridcontainer.SizeChanged += Gridcontainer_SizeChanged;
            htData = new Hashtable();

            gridcontainer.ColumnDefinitions.Clear();
            gridcontainer.RowDefinitions.Clear();
            gridcontainer.Children.Clear();

            int row = 0;

            /*DEFINIZIONE COLONNE*/
            ColumnDefinition cd;

            for (int i = 0; i < numcolumn; i++)
            {
                cd = new ColumnDefinition();
                cd.Width = new GridLength(columnsWidth[i], GridUnitType.Star);
                cd.MinWidth = columnsMinWidth[i];
                gridcontainer.ColumnDefinitions.Add(cd);
            }

            /*HEADERS*/
            RowDefinition rd;
            TextBox txt;
            TextBlock lbl;
            Border brd;

            for (int i = 0; i < numcolumn; i++)
            {
                rd = new RowDefinition();
                gridcontainer.RowDefinitions.Add(rd);

                brd = new Border();
                brd.BorderThickness = new Thickness(1.0);
                brd.BorderBrush = Brushes.LightGray;
                brd.Background = Brushes.LightGray;
                brd.Padding = new Thickness(2.0);

                lbl = new TextBlock();
                lbl.Text = columnsAlias[i];
                lbl.TextAlignment = TextAlignment.Center;
                lbl.TextWrapping = TextWrapping.Wrap;
                lbl.FontWeight = FontWeights.Bold;

                brd.Child = lbl;

                gridcontainer.Children.Add(brd);
                Grid.SetRow(brd, row);
                Grid.SetColumn(brd, i);
            }

            row++;

            /*DATI*/
            foreach (DataRow dtrow in dati.Rows)
            {
                rd = new RowDefinition();
                gridcontainer.RowDefinitions.Add(rd);

                for (int i = 0; i < numcolumn; i++)
                {
                    brd = new Border();
                    brd.BorderThickness = new Thickness(1.0);
                    brd.BorderBrush = Brushes.LightGray;

                    brd.Padding = new Thickness(0.0);
                    brd.Margin = new Thickness(0.0);

                    if (row % 2 == 0)
                    {
                        brd.Background = new SolidColorBrush(Color.FromArgb(255, 241, 241, 241));
                    }
                    else
                    {
                        brd.Background = Brushes.White;
                    }

                    txt = new TextBox();
                    txt.Name = "txt_" + i.ToString() + "_" + row.ToString();

                   

                    htData.Add(txt.Name, dtrow[columnsValues[i]]);
                    txt.Text = dtrow[columnsValues[i]].ToString();
                    switch (columnsAlignment[i])
                    {
                        case "right":
                            txt.TextAlignment = TextAlignment.Right;
                            break;
                        case "left":
                        default:
                            txt.TextAlignment = TextAlignment.Left;
                            break;
                    }                    
                    
                    txt.TextWrapping = TextWrapping.Wrap;
                    txt.GotFocus += Txt_GotFocus;
                    txt.LostFocus += Txt_LostFocus;
                    txt.PreviewKeyDown += Txt_PreviewKeyDown;
                    txt.PreviewMouseDown += Txt_PreviewMouseDown;
                    txt.BorderThickness = new Thickness(0.0);
                    txt.Background = Brushes.Transparent;

                    brd.Child = txt;

                    gridcontainer.Children.Add(brd);
                    Grid.SetRow(brd, row);
                    Grid.SetColumn(brd, i);
                }

                row++;
            }
        }

        private void Gridcontainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (Border item in ((Grid)(sender)).Children)
            {
                if (item.Child.GetType().Name == "TextBox")
                {
                    for (int i = 0; i < columnsValues.Length; i++)
                    {
                        if (i == Convert.ToInt32(((TextBox)(item.Child)).Name.Split('_')[1]))
                        {
                            ((TextBox)(item.Child)).Width = ((e.NewSize.Width - 10.0) / totalwidth * columnsWidth[i]) - 4.0;

                            if(i == 3)
                            {
                                double textwidth = ((TextBox)(item.Child)).Width + 5;
                                lblTotale.Margin = new Thickness(0, -10, 20 + textwidth, 10);
                                txtTotale.Margin = new Thickness(0, -10, 10, 10);
                                txtTotale.Width = textwidth;
                            }
                        }
                    }
                }
            }
        }

        private void Txt_GotFocus(object sender, RoutedEventArgs e)
        {
            foreach (Border item in ((Grid)(((Border)(((TextBox)sender).Parent)).Parent)).Children)
            {
                if (item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_')[2] == ((TextBox)sender).Name.Split('_')[2])
                {
                    item.BorderBrush = App._arrBrushes[0];
                }
                else
                {
                    item.BorderBrush = Brushes.LightGray;
                }
            }

            ((TextBox)sender).SelectAll();
        }

        private void Txt_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_ReadOnly)
            {
                MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
                return;
            }
        }

        private void Txt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_ReadOnly)
            {
                MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
                return;
            }

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                ((TextBox)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void Txt_LostFocus(object sender, RoutedEventArgs e)
        {
            string value = ((TextBox)sender).Text;

            int idcolumn = Convert.ToInt32(((TextBox)sender).Name.Split('_')[1]);
            int idrow = Convert.ToInt32(((TextBox)sender).Name.Split('_')[2]);

            switch (columnsTypes[idcolumn])
            {
                case "money":
                    value = cBusinessObjects.ConvertNumber(value);
                    break;
                case "string":
                default:
                    break;
            }            

            htData[((TextBox)sender).Name] = value;
            ((TextBox)sender).Text = value;
            int J = 1;
            foreach (DataRow dtrow in dati.Rows)
            {
                if (J == idrow)
                {
                   

                     switch (columnsTypes[idcolumn])
                    {
                        case "money":
                           double outd = 0;
                            double.TryParse( value, out outd);  
                            dtrow[columnsValues[idcolumn]] = outd;
                            break;
                        case "string":
                        default:
                          dtrow[columnsValues[idcolumn]] = value;
                            break;
                    }            
                }


                J++;
            }
           
            if (columnsHasTotal[idcolumn])
            {
                GenerateTotal();
            }
        }

        public int Save()
		{
            return cBusinessObjects.SaveData(id, dati, typeof(TempiRevisione));
        }

      
        
        private void GenerateTotal()
        {
            if ( _StartingCalculation )
            {
                return;
            }
            
            double TotaleOre = 0.0;

            foreach ( DictionaryEntry item in htData)//_x.Document.SelectNodes( "/Dati/Dato[@ID" + _ID + "]/Valore[@tipo='CompensoRevisione']" ) )
            {
                if (item.Key.ToString().Split('_')[1] == "3")
                {

                    try
                    {
                        TotaleOre += Convert.ToDouble(item.Value.ToString());
                    }
                    catch(Exception)
                    {

                    }
                }
            }



            try
            {
                txtTotale.Text = cBusinessObjects.ConvertNumber(TotaleOre.ToString());
               
            }
            catch ( Exception ex )
            {
                string log = ex.Message;
            }
        }

        private void Image_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            Image i = ((Image)sender);

            TextBlock t = ((TextBlock)(((Grid)(i.Parent)).Children[1]));

            UIElement u = ((Grid)(i.Parent)).Children[2];

            if ( u.Visibility == System.Windows.Visibility.Collapsed )
            {
                u.Visibility = System.Windows.Visibility.Visible;
                t.TextAlignment = TextAlignment.Center;
                var uriSource = new Uri( down, UriKind.Relative );
                i.Source = new BitmapImage( uriSource );
            }
            else
            {
                t.TextAlignment = TextAlignment.Left;
                u.Visibility = System.Windows.Visibility.Collapsed;
                var uriSource = new Uri( left, UriKind.Relative );
                i.Source = new BitmapImage( uriSource );
            }
        }
        
#region COMPENSO Revisione
        private void AggiungiNodoCompensoRevisione( string Fase, string ID, string Attivita, string Esecutore, string Ore )
        {
            if ( _ReadOnly && Fase == "" )
            {
                MessageBox.Show(  App.MessaggioSolaScrittura, "Attenzione" );
                return;
            }

      
            bool trovata=false;
            foreach (DataRow dtrow in dati.Rows)
            {
                if(dtrow["fase"].ToString()==Fase)
                trovata = true;

            }

            if ((Fase=="")||(!trovata))
            {
                 double core = 0;
                try
                {
                    core= double.Parse(Ore);
                }
                catch(Exception)
                {
                    core = 0;
                }
        
                dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Fase, Attivita, Esecutore, core);
                
                GenerateTable(tblMainContainer, numcolumns, columnsAlias, columnsValues, columnsWidth, conditionalReadonly);

            }
        }
        
        private void AddRowCompensoRevisione( object sender, RoutedEventArgs e )
        {
            AggiungiNodoCompensoRevisione( "", _ID, "", "", "" );
        }

        private void DeleteRowCompensoRevisione( object sender, RoutedEventArgs e )
        {
            if ( _ReadOnly )
            {
                MessageBox.Show(  App.MessaggioSolaScrittura, "Attenzione" );
                return;
            }

            if ( MessageBox.Show( "Si è sicuri di procedere con l'eliminazione?", "Attenzione", MessageBoxButton.YesNo ) == MessageBoxResult.Yes )
            {

                int idrow = -1;
                foreach (Border item in tblMainContainer.Children)
                {
                    if (item.BorderBrush == App._arrBrushes[0])
                    { 
                        idrow = Convert.ToInt32(((TextBox)(item.Child)).Name.Split('_')[2]);
                        break;
                    }
                }

                if (idrow == -1)
                {
                    MessageBox.Show("Selezionare una riga");
                    return;
                }

                try
                {
                    foreach (DataRow dtrow in dati.Rows)
                    {

                    }
                    for (int i = dati.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dtrow = dati.Rows[i];
                        if (i == (idrow-1))
                            dtrow.Delete();
                    }
                    this.dati.AcceptChanges();
                   

                    GenerateTable(tblMainContainer, numcolumns, columnsAlias, columnsValues, columnsWidth, conditionalReadonly);

                    GenerateTotal();

                    return;
                }
                catch ( Exception ex )
                {
                    string log = ex.Message;

                    MessageBox.Show( "Solo le righe inserite dall'utente possono essere cancellate" );
                }
            }
        }
        
#endregion

        private void txtTariffaOraria_LostFocus( object sender, RoutedEventArgs e )
        {
            GenerateTotal();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Border item in tblMainContainer.Children)
            {
                if (item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).IsReadOnly == true)
                {
                    ((TextBox)(item.Child)).Focus();
                    ((TextBox)(item.Child)).SelectAll();
                    return;
                }
            }
        }
    }
}
