using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Xml;
using System.Collections;
using UserControls;
using System.Windows.Media.Imaging;
using System.IO;

namespace RevisoftApplication
{

    public partial class wWorkAreaTree_PianificazioniVigilanzeOLD : System.Windows.Window
    {
        public string IDP = "";
        public string SelectedTreeSource = "";
        public string SelectedDataSource = "";
        public string SelectedSessioneSource = "";
                
		private string _cliente = "";
        private App.TipoAttivita _TipoAttivita = App.TipoAttivita.Sconosciuto;

        public string TitoloSessione = "";
        public string ImportFileName = "";

		public string IDTree = "-1";
		public string IDCliente = "-1";
		public string IDSessione = "-1";

        public string DataInizio = "";
        public string DataFine = "";

        public bool ReadOnly = true;

        public XmlDataProviderManager _x;

        XmlNode pianificazioniNode;

		Hashtable htComboID = new Hashtable();

		public string Cliente 
		{       
			get 
			{ 
				return _cliente; 
			}
			set 
			{ 
				_cliente = value;
				GeneraTitolo();
			}
		}

        public App.TipoAttivita TipoAttivita
        {
            get
            {
                return _TipoAttivita;
            }
            set
            {
                _TipoAttivita = value;
            }
        }

		private void GeneraTitolo()
		{
            txtTitoloPeriodo.Text = "Perioido dal " + DataInizio + " al " + DataFine;
		}

        public wWorkAreaTree_PianificazioniVigilanzeOLD()
        {   
            if (alreadydone) { }
            InitializeComponent();
        }

        #region TreeDataSource

        private void SaveTreeSource()
        {
            _x.Save();
        }

        public void LoadTreeSource()
        {
            Hashtable htSessioni = new Hashtable();

            pianificazioniNode = _x.Document.SelectSingleNode( "//Dato[@ID=\"100003\"]" );

            foreach ( XmlNode itemV in pianificazioniNode.SelectNodes( "//Pianificazione" ) )
            {
                if ( !htSessioni.Contains( itemV.Attributes["ID"].Value ) )
                {
                    htSessioni.Add( itemV.Attributes["ID"].Value, itemV.Attributes["Data"].Value );
                }
            }

            grdHeaderContainer.Children.Clear();
            grdHeaderContainer.ColumnDefinitions.Clear();
            grdHeaderContainer.RowDefinitions.Clear();

            ColumnDefinition gridCol1H = new ColumnDefinition();
            gridCol1H.Width = new GridLength( 350, GridUnitType.Pixel );
            grdHeaderContainer.ColumnDefinitions.Add( gridCol1H );

            ColumnDefinition gridCol2H = new ColumnDefinition();
            gridCol2H.Width = new GridLength( 70.0, GridUnitType.Pixel );
            grdHeaderContainer.ColumnDefinitions.Add( gridCol2H );

            foreach ( DictionaryEntry item in htSessioni )
            {
                ColumnDefinition gridColN = new ColumnDefinition();
                gridColN.Width = new GridLength( 70.0, GridUnitType.Pixel );
                grdHeaderContainer.ColumnDefinitions.Add( gridColN );
            }

            RowDefinition gridRow1 = new RowDefinition();
            gridRow1.Height = new GridLength( 20 );
            grdHeaderContainer.RowDefinitions.Add( gridRow1 );

            RowDefinition gridRow2 = new RowDefinition();
            gridRow2.Height = new GridLength( 20 );
            grdHeaderContainer.RowDefinitions.Add( gridRow2 );

            Border brd = new Border();
            brd.BorderBrush = Brushes.Black;
            brd.BorderThickness = new Thickness( 1, 1, 1, 1 );
            TextBlock txtb = new TextBlock();
            txtb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            txtb.Text = "NA";
            txtb.Height = 20;
            txtb.Margin = new Thickness( 0, 0, 0, 0 );
            brd.Child = txtb;
            Grid.SetRow( brd, 0 );
            Grid.SetRowSpan( brd, 2 );
            Grid.SetColumn( brd, 1 );
            grdHeaderContainer.Children.Add( brd );

            int indexcolumn = 2;

            foreach ( DictionaryEntry item in htSessioni )
            {
                brd = new Border();
                brd.BorderBrush = Brushes.Black;
                brd.BorderThickness = new Thickness( 0, 1, 1, 0 );
                txtb = new TextBlock();
                txtb.Text = item.Key.ToString() + "° Sessione";
                txtb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                txtb.Height = 20;
                txtb.Margin = new Thickness( 0, 0, 0, 0 );
                brd.Child = txtb;
                Grid.SetRow( brd, 0 );
                Grid.SetColumn( brd, indexcolumn );
                grdHeaderContainer.Children.Add( brd );

                brd = new Border();
                brd.BorderBrush = Brushes.Black;
                brd.BorderThickness = new Thickness( 0, 0, 1, 1 );
                txtb = new TextBlock();
                txtb.Text = item.Value.ToString();
                txtb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                txtb.Height = 20;
                txtb.Margin = new Thickness( 0, 0, 0, 0 );
                brd.Child = txtb;
                Grid.SetRow( brd, 1 );
                Grid.SetColumn( brd, indexcolumn );
                grdHeaderContainer.Children.Add( brd );

                indexcolumn++;
            }


            grdMainContainer.Children.Clear();
            grdMainContainer.ColumnDefinitions.Clear();
            grdMainContainer.RowDefinitions.Clear();

            ColumnDefinition gridCol1 = new ColumnDefinition();
            gridCol1.Width = new GridLength( 350, GridUnitType.Pixel );
            grdMainContainer.ColumnDefinitions.Add( gridCol1 );

            ColumnDefinition gridCol2 = new ColumnDefinition();
            gridCol2.Width = new GridLength( 70.0, GridUnitType.Pixel );
            gridCol2.MinWidth = 70.0;
            grdMainContainer.ColumnDefinitions.Add( gridCol2 );

            foreach ( DictionaryEntry item in htSessioni )
            {
                ColumnDefinition gridColN = new ColumnDefinition();
                gridColN.Width = new GridLength( 70.0, GridUnitType.Pixel );
                grdMainContainer.ColumnDefinitions.Add( gridColN );
            }

            foreach ( XmlNode item in pianificazioniNode.SelectNodes( "//Dato[@ID=\"100003\"]/Valore" ) )
            {
                RowDefinition gridRow3 = new RowDefinition();
                gridRow3.Height = new GridLength( 20 );
                grdMainContainer.RowDefinitions.Add( gridRow3 );
            }
                           
            int indexrow = 0;

            foreach ( XmlNode item in pianificazioniNode.SelectNodes( "//Dato[@ID=\"100003\"]/Valore" ) )
            {
                indexcolumn = 0;

                bool istitolo = false;
                if(item.Attributes["Father"] != null)
                {
                    istitolo = true;
                }

                txtb = new TextBlock();
                txtb.Text = item.Attributes["Codice"].Value + " " + item.Attributes["Titolo"].Value;
                if(istitolo)
                {
                    txtb.FontWeight = FontWeights.Bold;
                }                
                txtb.Height = 20;
                //txtb.Margin = new Thickness( item.Attributes["Codice"].Value.Split('.').Count() * 10, 0, 0, 0 );
                Grid.SetRow( txtb, indexrow );
                Grid.SetColumn( txtb, indexcolumn );
                grdMainContainer.Children.Add( txtb );
                indexcolumn++;

                brd = new Border();
                brd.BorderBrush = Brushes.Black;
                brd.BorderThickness = new Thickness( 1, 0, 1, 0 );
                if(!istitolo)
                {
                    CheckBox chk = new CheckBox();
                    chk.Name = "chkNA_" + item.Attributes["ID"].Value;
                    this.RegisterName( chk.Name, chk );                    
                    XmlNode nodehere = pianificazioniNode.SelectSingleNode( "//Valore[@ID=\"" + item.Attributes["ID"].Value + "\"]" );
                    if ( nodehere != null && nodehere.Attributes["Checked"] != null && nodehere.Attributes["Checked"].Value == "True" )
                    {
                        chk.IsChecked = true;
                    }
                    else
                    {
                        chk.IsChecked = false;
                    }
                    
                    chk.Checked += chk_Checked;
                    chk.Unchecked += chk_Unchecked;
                    chk.PreviewMouseDown += chk_PreviewMouseDown;

                    chk.Margin = new Thickness( 0, 0, 0, 0 );
                    chk.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    brd.Child = chk;
                }
                Grid.SetRow( brd, indexrow );
                Grid.SetColumn( brd, indexcolumn );
                grdMainContainer.Children.Add( brd );
                indexcolumn++;

                foreach ( DictionaryEntry itemS in htSessioni )
                {
                    brd = new Border();
                    brd.BorderBrush = Brushes.Black;
                    brd.BorderThickness = new Thickness( 0, 0, 1, 0 );
                    if ( !istitolo )
                    {
                        CheckBox chkS = new CheckBox();
                        chkS.Name = "chk_" + itemS.Key.ToString() + "_" + item.Attributes["ID"].Value;
                        this.RegisterName( chkS.Name, chkS );                        
                        chkS.Margin = new Thickness( 0, 0, 0, 0 );
                        chkS.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        XmlNode nodehere = pianificazioniNode.SelectSingleNode( "//Valore[@ID=\"" + item.Attributes["ID"].Value + "\"]/Pianificazione[@ID=\"" + itemS.Key.ToString() + "\"]" );
                        if ( nodehere != null && nodehere.Attributes["Checked"] != null && nodehere.Attributes["Checked"].Value == "True" )
                        {
                            chkS.IsChecked = true;
                        }
                        else
                        {
                            chkS.IsChecked = false;
                        }

                        chkS.Checked += chkS_Checked;
                        chkS.Unchecked += chkS_Unchecked;
                        chkS.PreviewMouseDown += chk_PreviewMouseDown;

                        brd.Child = chkS;
                    }
                    Grid.SetRow( brd, indexrow );
                    Grid.SetColumn( brd, indexcolumn );
                    grdMainContainer.Children.Add( brd );
                    indexcolumn++;
                }
                
                indexrow++;
            }

            LoadDataSource();
        }

        void chk_PreviewMouseDown( object sender, MouseButtonEventArgs e )
        {
            if ( ReadOnly )
            {
                MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione" );
                e.Handled = true;
                return;
            }
        }

        void chkS_Unchecked( object sender, RoutedEventArgs e )
        {
            if ( ReadOnly )
            {
                MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione" );
                return;
            }

            SaveTreeSource();
            pianificazioniNode = _x.Document.SelectSingleNode( "//Dato[@ID=\"100003\"]" );

            string name = ((CheckBox)(sender)).Name;
            string[] splittedName = name.Split('_');

            XmlNode nodehere = pianificazioniNode.SelectSingleNode( "//Valore[@ID=\"" + splittedName[2] + "\"]/Pianificazione[@ID=\"" + splittedName[1] + "\"]" );
            nodehere.Attributes["Checked"].Value = "False";

            SaveTreeSource();
        }

        void chkS_Checked( object sender, RoutedEventArgs e )
        {
            if ( ReadOnly )
            {
                MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione" );
                return;
            }

            SaveTreeSource();
            pianificazioniNode = _x.Document.SelectSingleNode( "//Dato[@ID=\"100003\"]" );

            string name = ( (CheckBox)( sender ) ).Name;
            string[] splittedName = name.Split( '_' );

            //foreach ( XmlNode item in pianificazioniNode.SelectNodes( "//Valore[@ID=\"" + splittedName[2] + "\"]/Pianificazione") )
            //{
            //    item.Attributes["Checked"].Value = "False";
            //    if ( item.Attributes["ID"].Value != splittedName[1] )
            //    {
            //        ( (CheckBox)( this.FindName( "chk_" + item.Attributes["ID"].Value + "_" + splittedName[2] ) ) ).IsChecked = false;
            //    }
            //}

            ( (CheckBox)( this.FindName( "chkNA_" + splittedName[2] ) ) ).IsChecked = false;

            XmlNode nodehere = pianificazioniNode.SelectSingleNode( "//Valore[@ID=\"" + splittedName[2] + "\"]/Pianificazione[@ID=\"" + splittedName[1] + "\"]" );
            nodehere.Attributes["Checked"].Value = "True";

            SaveTreeSource();
        }

        bool alreadydone = false;

        void chk_Unchecked( object sender, RoutedEventArgs e )
        {
            SaveTreeSource();
            pianificazioniNode = _x.Document.SelectSingleNode( "//Dato[@ID=\"100003\"]" );

            string name = ( (CheckBox)( sender ) ).Name;
            string[] splittedName = name.Split( '_' );

            foreach ( XmlNode item in pianificazioniNode.SelectNodes( "//Valore[@ID=\"" + splittedName[1] + "\"]/Pianificazione" ) )
            {
                ( (CheckBox)( this.FindName( "chk_" + item.Attributes["ID"].Value + "_" + splittedName[1] ) ) ).Visibility = System.Windows.Visibility.Visible;
            }

            XmlNode nodehere = pianificazioniNode.SelectSingleNode( "//Valore[@ID=\"" + splittedName[1] + "\"]" );
            nodehere.Attributes["Checked"].Value = "False";

            SaveTreeSource();
        }

        void chk_Checked( object sender, RoutedEventArgs e )
        {
            SaveTreeSource();
            pianificazioniNode = _x.Document.SelectSingleNode( "//Dato[@ID=\"100003\"]" );

            string name = ( (CheckBox)( sender ) ).Name;
            string[] splittedName = name.Split( '_' );

            foreach ( XmlNode item in pianificazioniNode.SelectNodes( "//Valore[@ID=\"" + splittedName[1] + "\"]/Pianificazione" ) )
            {
                item.Attributes["Checked"].Value = "False";
                ( (CheckBox)( this.FindName( "chk_" + item.Attributes["ID"].Value + "_" + splittedName[1] ) ) ).IsChecked = false;
                ( (CheckBox)( this.FindName( "chk_" + item.Attributes["ID"].Value + "_" + splittedName[1] ) ) ).Visibility = System.Windows.Visibility.Hidden;
            }

            XmlNode nodehere = pianificazioniNode.SelectSingleNode( "//Valore[@ID=\"" + splittedName[1] + "\"]" );
            nodehere.Attributes["Checked"].Value = "True";

            SaveTreeSource();
        }

        #endregion

        #region DataDataSource

        private void LoadDataSource()
        {
			; 
        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
			;
        }
        
		private void buttonChiudi_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

    }
}