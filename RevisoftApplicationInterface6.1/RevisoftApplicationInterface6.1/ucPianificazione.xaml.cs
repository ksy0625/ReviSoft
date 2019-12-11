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
    public partial class ucPianificazione : UserControl
    {
        public int id;
        private DataTable dati = null;
        private DataTable dati_rischio_globale = null;

        private int Offset = 260;
        private int OffsetNote = 270 + 1000;
        private int Minimo = 200;

		private string check = "./Images/icone/check2-24x24.png";
		private string uncheck = "./Images/icone/check1-24x24.png";

		private string up = "./Images/icone/navigate_up.png";
		private string down = "./Images/icone/navigate_down.png";
		private string left = "./Images/icone/navigate_left.png";

		private XmlDataProviderManager _x;
        private string _ID = "-1";
		//private string IDRischioGlobale = "22";

		private bool _ReadOnly = false;
        
        public bool ReadOnly 
        {
            set
            {
				_ReadOnly = value;
            }
        }

		public ucPianificazione()
        {
            if (Offset==0 || OffsetNote==0 || Minimo==0) { }
            InitializeComponent();            
        }

        public void Load( string ID,string IDCliente,string IDSessione)
        {
            
            id = int.Parse(ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());

            _ID = ID;

            dati = cBusinessObjects.GetData(id, typeof(Pianificazione));
            dati_rischio_globale = cBusinessObjects.GetData(id, typeof(RischioGlobale));

            DataRow datirow = null;
            foreach (DataRow dtrow in dati.Rows)
            {
                datirow = dtrow;
            }

            DataRow datirow_rischio_globale = null;
            foreach (DataRow dtrow in dati_rischio_globale.Rows)
            {
                datirow_rischio_globale = dtrow;
            }


            #region primo blocco
            SortedDictionary<string, string> Nodi = new SortedDictionary<string, string>();

			Nodi.Add("Attribuzione complessiva\r\nrischio AMBIENTE", "txt1");
			Nodi.Add("Attribuzione rischio globale\r\nCICLO VENDITE", "txt2c");
			Nodi.Add("Attribuzione rischio globale\r\nCICLO ACQUISTI", "txt3c");
			Nodi.Add("Attribuzione rischio globale\r\nCICLO MAGAZZINO", "txt4c");
			Nodi.Add("Attribuzione rischio globale\r\nCICLO TESORERIA", "txt5c");
			Nodi.Add("Attribuzione rischio globale\r\nCICLO PERSONALE", "txt6c");

			TextBlock txt;
			Image img;
			Uri uriSource;
			int row = 1;

			Grid grd = new Grid();
			ColumnDefinition cd = new ColumnDefinition();
			cd.Width = GridLength.Auto;
			grd.ColumnDefinitions.Add(cd);
			cd = new ColumnDefinition();
			cd.Width = new GridLength(100, GridUnitType.Pixel);
			grd.ColumnDefinitions.Add(cd);
			cd = new ColumnDefinition();
			cd.Width = new GridLength(100, GridUnitType.Pixel);
			grd.ColumnDefinitions.Add(cd);
			cd = new ColumnDefinition();
			cd.Width = new GridLength(100, GridUnitType.Pixel);
			grd.ColumnDefinitions.Add(cd);
			cd = new ColumnDefinition();
			cd.Width = new GridLength(100, GridUnitType.Pixel);
			grd.ColumnDefinitions.Add(cd);
			cd = new ColumnDefinition();
			cd.Width = new GridLength(100, GridUnitType.Pixel);
			grd.ColumnDefinitions.Add(cd);

			RowDefinition rd = new RowDefinition();
			grd.RowDefinitions.Add(rd);

			txt = new TextBlock();
			grd.Children.Add(txt);
			Grid.SetRow(txt, 0);
			Grid.SetColumn(txt, 0);

			Border brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.BorderBrush = Brushes.LightGray;
			brd.Background = Brushes.LightGray;
			brd.Padding = new Thickness(2.0);

			txt = new TextBlock();
			txt.Text = "Molto Alto";
            txt.FontSize = 14;
            txt.TextAlignment = TextAlignment.Center;
            txt.FontWeight = FontWeights.Bold;
            txt.Margin = new Thickness(0, 0, 0, 10);

			brd.Child = txt;

			grd.Children.Add(brd);
			Grid.SetRow(brd, 0);
			Grid.SetColumn(brd, 1);

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.BorderBrush = Brushes.LightGray;
			brd.Background = Brushes.LightGray;
			brd.Padding = new Thickness(2.0);

			txt = new TextBlock();
			txt.Text = "Alto";
			txt.FontSize = 14;
			txt.TextAlignment = TextAlignment.Center;
			txt.FontWeight = FontWeights.Bold;
			txt.Margin = new Thickness(0, 0, 0, 10);

			brd.Child = txt;

			grd.Children.Add(brd);
			Grid.SetRow(brd, 0);
			Grid.SetColumn(brd, 2);

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.BorderBrush = Brushes.LightGray;
			brd.Background = Brushes.LightGray;
			brd.Padding = new Thickness(2.0);

			txt = new TextBlock();
			txt.Text = "Medio";
            txt.FontSize = 14;
            txt.TextAlignment = TextAlignment.Center;
            txt.FontWeight = FontWeights.Bold;
            txt.Margin = new Thickness(0, 0, 0, 10);

			brd.Child = txt;

			grd.Children.Add(brd);
			Grid.SetRow(brd, 0);
			Grid.SetColumn(brd, 3);

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.BorderBrush = Brushes.LightGray;
			brd.Background = Brushes.LightGray;
			brd.Padding = new Thickness(2.0);

			txt = new TextBlock();
			txt.Text = "Basso";
            txt.FontSize = 14;
            txt.TextAlignment = TextAlignment.Center;
            txt.FontWeight = FontWeights.Bold;
            txt.Margin = new Thickness(0, 0, 0, 10);

			brd.Child = txt;

			grd.Children.Add(brd);
			Grid.SetRow(brd, 0);
			Grid.SetColumn(brd, 4);

			brd = new Border();
			brd.BorderThickness = new Thickness(1.0);
			brd.BorderBrush = Brushes.LightGray;
			brd.Background = Brushes.LightGray;
			brd.Padding = new Thickness(2.0);

			txt = new TextBlock();
			txt.Text = "Molto Basso";
			txt.FontSize = 14;
			txt.TextAlignment = TextAlignment.Center;
			txt.FontWeight = FontWeights.Bold;
			txt.Margin = new Thickness(0, 0, 0, 10);

			brd.Child = txt;

			grd.Children.Add(brd);
			Grid.SetRow(brd, 0);
			Grid.SetColumn(brd, 5);

		//	XmlNode node = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + IDRischioGlobale + "]");

			if (datirow_rischio_globale != null)
			{
				foreach (KeyValuePair<string, string> item in Nodi)
				{
					rd = new RowDefinition();
					grd.RowDefinitions.Add(rd);

					brd = new Border();
					brd.BorderThickness = new Thickness(1.0);
					brd.BorderBrush = Brushes.LightGray;
					if (row % 2 == 0)
					{
						brd.Background = new SolidColorBrush(Color.FromArgb(126, 241, 241, 241));
					}
					else
					{
						brd.Background = Brushes.White;
					}

					brd.Padding = new Thickness(2.0);

					txt = new TextBlock();
					txt.Text = item.Key.ToString();
					txt.FontSize = 13;

					brd.Child = txt;

					grd.Children.Add(brd);
					Grid.SetRow(brd, row);
					Grid.SetColumn(brd, 0);

					brd = new Border();
					brd.BorderThickness = new Thickness(1.0);
					brd.BorderBrush = Brushes.LightGray;
					if (row % 2 == 0)
					{
						brd.Background = new SolidColorBrush(Color.FromArgb(126, 241, 241, 241));
					}
					else
					{
						brd.Background = Brushes.White;
					}

					brd.Padding = new Thickness(2.0);

					img = new Image();
					if (datirow_rischio_globale[item.Value.ToString()] != null && datirow_rischio_globale[item.Value.ToString()].ToString() == "Molto Alto")
					{
						uriSource = new Uri(check, UriKind.Relative);
					}
					else
					{
						uriSource = new Uri(uncheck, UriKind.Relative);
					}

					img.Source = new BitmapImage(uriSource);
					img.Width = 16.0;

					brd.Child = img;

					grd.Children.Add(brd);
					Grid.SetRow(brd, row);
					Grid.SetColumn(brd, 1);

					brd = new Border();
					brd.BorderThickness = new Thickness(1.0);
					brd.BorderBrush = Brushes.LightGray;
					if (row % 2 == 0)
					{
						brd.Background = new SolidColorBrush(Color.FromArgb(126, 241, 241, 241));
					}
					else
					{
						brd.Background = Brushes.White;
					}

					brd.Padding = new Thickness(2.0);

					img = new Image();
					if (datirow_rischio_globale[item.Value.ToString()] != null && datirow_rischio_globale[item.Value.ToString()].ToString() == "Alto")
					{
						uriSource = new Uri(check, UriKind.Relative);
					}
					else
					{
						uriSource = new Uri(uncheck, UriKind.Relative);
					}

					img.Source = new BitmapImage(uriSource);
					img.Width = 16.0;

					brd.Child = img;

					grd.Children.Add(brd);
					Grid.SetRow(brd, row);
					Grid.SetColumn(brd, 2);

					brd = new Border();
					brd.BorderThickness = new Thickness(1.0);
					brd.BorderBrush = Brushes.LightGray;
					if (row % 2 == 0)
					{
						brd.Background = new SolidColorBrush(Color.FromArgb(126, 241, 241, 241));
					}
					else
					{
						brd.Background = Brushes.White;
					}

					brd.Padding = new Thickness(2.0);

					img = new Image();

					if (datirow_rischio_globale[item.Value.ToString()] != null && datirow_rischio_globale[item.Value.ToString()].ToString() == "Medio")
					{
						uriSource = new Uri(check, UriKind.Relative);
					}
					else
					{
						uriSource = new Uri(uncheck, UriKind.Relative);
					}

					img.Source = new BitmapImage(uriSource);
					img.Width = 16.0;

					brd.Child = img;

					grd.Children.Add(brd);
					Grid.SetRow(brd, row);
					Grid.SetColumn(brd, 3);

					brd = new Border();
					brd.BorderThickness = new Thickness(1.0);
					brd.BorderBrush = Brushes.LightGray;
					if (row % 2 == 0)
					{
						brd.Background = new SolidColorBrush(Color.FromArgb(126, 241, 241, 241));
					}
					else
					{
						brd.Background = Brushes.White;
					}

					brd.Padding = new Thickness(2.0);

					img = new Image();
                    if (datirow_rischio_globale[item.Value.ToString()] != null && datirow_rischio_globale[item.Value.ToString()].ToString() == "Basso")
            		{
						uriSource = new Uri(check, UriKind.Relative);
					}
					else
					{
						uriSource = new Uri(uncheck, UriKind.Relative);
					}

					img.Source = new BitmapImage(uriSource);
					img.Width = 16.0;

					brd.Child = img;

					grd.Children.Add(brd);
					Grid.SetRow(brd, row);
					Grid.SetColumn(brd, 4);

					brd = new Border();
					brd.BorderThickness = new Thickness(1.0);
					brd.BorderBrush = Brushes.LightGray;
					if (row % 2 == 0)
					{
						brd.Background = new SolidColorBrush(Color.FromArgb(126, 241, 241, 241));
					}
					else
					{
						brd.Background = Brushes.White;
					}

					brd.Padding = new Thickness(2.0);

					img = new Image();

                    if (datirow_rischio_globale[item.Value.ToString()] != null && datirow_rischio_globale[item.Value.ToString()].ToString() == "Molto Basso")
					{
						uriSource = new Uri(check, UriKind.Relative);
					}
					else
					{
						uriSource = new Uri(uncheck, UriKind.Relative);
					}

					img.Source = new BitmapImage(uriSource);
					img.Width = 16.0;

					brd.Child = img;

					grd.Children.Add(brd);
					Grid.SetRow(brd, row);
					Grid.SetColumn(brd, 5);

					row++;
				}
			}

			brdSommarioRischi.Child = grd;
#endregion

#region secondo blocco
			
	
			SortedDictionary<int, string> VociBilancio = new SortedDictionary<int, string>();

			VociBilancio.Add(80, "Immobilizzazioni immateriali");
			VociBilancio.Add(81, "Immobilizzazioni materiali");
			VociBilancio.Add(82, "Immobilizzazioni finanziarie");
			VociBilancio.Add(83, "Rimanenze di Magazzino");
			VociBilancio.Add(85, "Rimanenze - Opere a lungo termine");
			VociBilancio.Add(86, "Attività finanziarie non immobilizzate");
			VociBilancio.Add(87, "Crediti verso clienti");
			VociBilancio.Add(88, "Crediti e debiti infragruppo");
			VociBilancio.Add(89, "Crediti tributari e per imposte differite attive");
			VociBilancio.Add(90, "Crediti verso altri");
			VociBilancio.Add(91, "Cassa e Banche");
			VociBilancio.Add(92, "Ratei e risconti (attivi e passivi)");
			VociBilancio.Add(93, "Patrimonio netto");
			VociBilancio.Add(94, "Fondi per rischi ed oneri");
			VociBilancio.Add(95, "Fondo TFR e debiti connessi all'pers.dip.");
			VociBilancio.Add(96, "Mutui e finanziamenti");
			VociBilancio.Add(97, "Debiti verso fornitori");
			VociBilancio.Add(98, "Debiti tributari");
			VociBilancio.Add(99, "Debiti verso altri");
			VociBilancio.Add(100, "Conti d'ordine");
			VociBilancio.Add(101, "Conto economico");
			VociBilancio.Add(102, "Bilancio Consolidato");

			grd = new Grid();

			cd = new ColumnDefinition();
			cd.Width = new GridLength(1, GridUnitType.Star);
			cd.SharedSizeGroup = "ssg1";
			grd.ColumnDefinitions.Add(cd);

			cd = new ColumnDefinition();
			cd.Width = new GridLength(50, GridUnitType.Pixel);
			cd.SharedSizeGroup = "ssg2";
			grd.ColumnDefinitions.Add(cd);

			cd = new ColumnDefinition();
			cd.Width = new GridLength(50, GridUnitType.Pixel);
			cd.SharedSizeGroup = "ssg3";
			grd.ColumnDefinitions.Add(cd);

			cd = new ColumnDefinition();
			cd.Width = new GridLength(50, GridUnitType.Pixel);
			cd.SharedSizeGroup = "ssg4";
			grd.ColumnDefinitions.Add(cd);

			cd = new ColumnDefinition();
            cd.Width = new GridLength( 1, GridUnitType.Star );
			cd.SharedSizeGroup = "ssg5";
			grd.ColumnDefinitions.Add(cd);

			cd = new ColumnDefinition();
			cd.Width = new GridLength(20, GridUnitType.Pixel);
			cd.SharedSizeGroup = "ssg6";
			grd.ColumnDefinitions.Add(cd);

			cd = new ColumnDefinition();
			cd.Width = new GridLength(50, GridUnitType.Pixel);
			cd.SharedSizeGroup = "ssg7";
			grd.ColumnDefinitions.Add(cd);

			rd = new RowDefinition();
			rd.Height = new GridLength(20, GridUnitType.Pixel);
			grd.RowDefinitions.Add(rd);

			rd = new RowDefinition();
			rd.Height = new GridLength(20, GridUnitType.Pixel);
			grd.RowDefinitions.Add(rd);

			txt = new TextBlock();
			txt.TextAlignment = TextAlignment.Center;
			txt.Text = "VOCI DI BILANCIO";
			grd.Children.Add(txt);
			Grid.SetRow(txt, 0);
			Grid.SetRowSpan(txt, 2);
			Grid.SetColumn(txt, 0);

			txt = new TextBlock();
			txt.TextAlignment = TextAlignment.Center;
			txt.Text = "INTENSITA' DI REVISIONE";
			grd.Children.Add(txt);
			Grid.SetRow(txt, 0);
			Grid.SetColumn(txt, 1);
			Grid.SetColumnSpan(txt, 3);

			txt = new TextBlock();

			txt.TextAlignment = TextAlignment.Center;
			txt.Text = "ALTA";
			grd.Children.Add(txt);
			Grid.SetRow(txt, 1);
			Grid.SetColumn(txt, 1);

			txt = new TextBlock();

			txt.TextAlignment = TextAlignment.Center;
			txt.Text = "BASSA";
			grd.Children.Add(txt);
			Grid.SetRow(txt, 1);
			Grid.SetColumn(txt, 2);

			txt = new TextBlock();

			txt.TextAlignment = TextAlignment.Center;
			txt.Text = "N/A";
			grd.Children.Add(txt);
			Grid.SetRow(txt, 1);
			Grid.SetColumn(txt, 3);

			txt = new TextBlock();
			txt.TextAlignment = TextAlignment.Center;
			txt.Text = "Esecutore della revisione / Note";
			grd.Children.Add(txt);
			Grid.SetRow(txt, 0);
			Grid.SetColumn(txt, 4);

			txt = new TextBlock();
			txt.TextAlignment = TextAlignment.Center;
			txt.Text = "(Se diverso dall'utilizzatore del software)";
			grd.Children.Add(txt);
			Grid.SetRow(txt, 1);
			Grid.SetColumn(txt, 4);

			brdDefinizione.Children.Add(grd);

			foreach (KeyValuePair<int, string> item in VociBilancio)
			{
			
				brd = new Border();				
				brd.CornerRadius = new CornerRadius(5.0);
				brd.BorderThickness = new Thickness(1.0);
				brd.BorderBrush = Brushes.LightGray;
				brd.Padding = new Thickness(4.0, 4.0, 0.0, 4.0);
				brd.Margin = new Thickness(4.0);

				grd = new Grid();
				
				cd = new ColumnDefinition();
				cd.Width = new GridLength(1, GridUnitType.Star);
				cd.SharedSizeGroup = "ssg1";
				grd.ColumnDefinitions.Add(cd);

				cd = new ColumnDefinition();
				cd.Width = new GridLength(50, GridUnitType.Pixel);
				cd.SharedSizeGroup = "ssg2";
				grd.ColumnDefinitions.Add(cd);

				cd = new ColumnDefinition();
				cd.Width = new GridLength(50, GridUnitType.Pixel);
				cd.SharedSizeGroup = "ssg3";
				grd.ColumnDefinitions.Add(cd);

				cd = new ColumnDefinition();
				cd.Width = new GridLength(50, GridUnitType.Pixel);
				cd.SharedSizeGroup = "ssg4";
				grd.ColumnDefinitions.Add(cd);

				cd = new ColumnDefinition();
                cd.Width = new GridLength( 1, GridUnitType.Star );
				cd.SharedSizeGroup = "ssg5";
				grd.ColumnDefinitions.Add(cd);
				
				cd = new ColumnDefinition();
				cd.Width = new GridLength(20, GridUnitType.Pixel);
				cd.SharedSizeGroup = "ssg6";
				grd.ColumnDefinitions.Add(cd);
				
				cd = new ColumnDefinition();
				cd.Width = new GridLength(50, GridUnitType.Pixel);
				cd.SharedSizeGroup = "ssg7";
				grd.ColumnDefinitions.Add(cd);

				rd = new RowDefinition();
				grd.RowDefinitions.Add(rd);

				rd = new RowDefinition();
				grd.RowDefinitions.Add(rd);

				txt = new TextBlock();
				txt.Text = item.Value.ToString();
				grd.Children.Add(txt);
				Grid.SetRow(txt, 0);
				Grid.SetColumn(txt, 0);

				img = new Image();
				img.Name = "_" + item.Key.ToString() + "_Alta";
				img.Height = 20.0;
				img.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

				if (datirow != null && datirow["field_" + item.Key.ToString()].ToString() != null && datirow["field_" + item.Key.ToString()].ToString() == "Alta")
				{
					var uriSourceint = new Uri(check, UriKind.Relative);
					img.Source = new BitmapImage(uriSourceint);
				}
				else
				{
					var uriSourceint = new Uri(uncheck, UriKind.Relative);
					img.Source = new BitmapImage(uriSourceint);
				}

				img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
				img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
				img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);

				this.RegisterName(img.Name, img);

				grd.Children.Add(img);
				Grid.SetRow(img, 0);
				Grid.SetColumn(img, 1);

				img = new Image();
				img.Name = "_" + item.Key.ToString() + "_Bassa";
				img.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
				img.Height = 20.0;

				if (datirow != null && datirow["field_" + item.Key.ToString()] != null && datirow["field_" + item.Key.ToString()].ToString() == "Bassa")
				{
					var uriSourceint = new Uri(check, UriKind.Relative);
					img.Source = new BitmapImage(uriSourceint);
				}
				else
				{
					var uriSourceint = new Uri(uncheck, UriKind.Relative);
					img.Source = new BitmapImage(uriSourceint);
				}

				img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
				img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
				img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);

				this.RegisterName(img.Name, img);

				grd.Children.Add(img);
				Grid.SetRow(img, 0);
				Grid.SetColumn(img, 2);

				img = new Image();
				img.Name = "_" + item.Key.ToString() + "_NA";
				img.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
				img.Height = 20.0;

                if (datirow != null && datirow["field_" + item.Key.ToString()] != null && datirow["field_" + item.Key.ToString()].ToString() == "NA")
				{
					var uriSourceint = new Uri(check, UriKind.Relative);
					img.Source = new BitmapImage(uriSourceint);
				}
				else
				{
					var uriSourceint = new Uri(uncheck, UriKind.Relative);
					img.Source = new BitmapImage(uriSourceint);
				}

				img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
				img.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
				img.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);

				this.RegisterName(img.Name, img);

				grd.Children.Add(img);
				Grid.SetRow(img, 0);
				Grid.SetColumn(img, 3);

				TextBox tb = new TextBox();
				tb.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
				tb.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
				tb.LostFocus += new RoutedEventHandler(tbEsecutore_LostFocus);
				tb.TextWrapping = TextWrapping.Wrap;
				tb.AcceptsReturn = true;
				tb.Name = "_" + item.Key.ToString() + "_Esecutore";
				if (datirow != null && datirow["Esecutore"] != null)
				{
					tb.Text = datirow["Esecutore"].ToString();
				}

				this.RegisterName(tb.Name, tb);

				grd.Children.Add(tb);
				Grid.SetRow(tb, 0);
				Grid.SetColumn(tb, 4);

				img = new Image();
				img.Name = "_" + item.Key.ToString() + "_NotaImg";
				img.Margin = new Thickness(0.0);
				img.ToolTip = "Nota";
				img.Height = 10.0;
				img.Width = 10.0;
				img.Margin = new Thickness(10, 0, 0, 0);
				img.MouseLeftButtonDown += new MouseButtonEventHandler(ImageNota_MouseLeftButtonDown);
				img.VerticalAlignment = System.Windows.VerticalAlignment.Center;

                if (datirow != null && datirow["Nota"] != null && datirow["Nota"].ToString() == "")
      			{
					var uriSourceint = new Uri(up, UriKind.Relative);
					img.Source = new BitmapImage(uriSourceint);
				}
				else
				{
					var uriSourceint = new Uri(down, UriKind.Relative);
					img.Source = new BitmapImage(uriSourceint);
				}

				this.RegisterName(img.Name, img);

				grd.Children.Add(img);
				Grid.SetRow(img, 0);
				Grid.SetColumn(img, 5);

				Label lbl = new Label();
				lbl.Margin = new Thickness(0.0);
				lbl.VerticalAlignment = System.Windows.VerticalAlignment.Center;
				lbl.FontWeight = FontWeights.Bold;
				var bc = new BrushConverter();
				lbl.Foreground = (Brush)bc.ConvertFrom("#F5A41C");
				lbl.Content = "Nota";
				grd.Children.Add(lbl);
				Grid.SetRow(lbl, 0);
				Grid.SetColumn(lbl, 6);

				tb = new TextBox();
				tb.Name = "_" + item.Key.ToString() + "_Nota";

                if (datirow != null && datirow["Nota"] != null)
				{
					tb.Text = datirow["Nota"].ToString();
				}
				else
				{
					tb.Text = "";
				}

				if (tb.Text != "")
				{
					tb.Visibility = System.Windows.Visibility.Visible;
				}
				else
				{
					tb.Visibility = System.Windows.Visibility.Collapsed;
				}

				tb.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(obj_PreviewMouseLeftButtonDown);
				tb.PreviewKeyDown += new KeyEventHandler(obj_PreviewKeyDown);
				tb.LostFocus += new RoutedEventHandler(tbNota_LostFocus);
				tb.TextWrapping = TextWrapping.Wrap;
				tb.AcceptsReturn = true;
				tb.Margin = new Thickness(0.0, 10.0, 10.0, 5.0);
				tb.Foreground = Brushes.Blue;
				tb.FontWeight = FontWeights.Bold;

				this.RegisterName(tb.Name, tb);

				grd.Children.Add(tb);
				Grid.SetRow(tb, 1);
				Grid.SetColumn(tb, 0);
				Grid.SetColumnSpan(tb, 7);
				
				brd.Child = grd;

				brdDefinizione.Children.Add(brd);
			}                                 
#endregion	

			if (datirow["Testo"].ToString() != "")
			{
				txtConsiderazioni.Text = datirow["Testo"].ToString();
			}

		}

		void tbEsecutore_LostFocus(object sender, RoutedEventArgs e)
		{
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}

            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["Esecutore"] =((TextBox)sender).Text;
            }
			
		}

		void tbNota_LostFocus(object sender, RoutedEventArgs e)
		{
			if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}

		
            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["Nota"] = ((TextBox)sender).Text;
            }
           
		
		}

		public int Save()
		{
            return cBusinessObjects.SaveData(id, dati, typeof(Pianificazione));
           
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{

                foreach (DataRow dtrow in dati.Rows)
                {
                    dtrow["Testo"] = ((TextBox)sender).Text;
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

		private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Image i = ((Image)sender);

			TextBlock t = ((TextBlock)(((Grid)(i.Parent)).Children[1]));

			UIElement u =  ((Grid)(i.Parent)).Children[2];

			if (u.Visibility == System.Windows.Visibility.Collapsed)
			{
				u.Visibility = System.Windows.Visibility.Visible;
				t.TextAlignment = TextAlignment.Center;
				var uriSource = new Uri(down, UriKind.Relative);
				i.Source = new BitmapImage(uriSource);
			}
			else
			{
				t.TextAlignment = TextAlignment.Left;
				u.Visibility = System.Windows.Visibility.Collapsed;
				var uriSource = new Uri(left, UriKind.Relative);
				i.Source = new BitmapImage(uriSource);
			}
		}

		private void ImageNota_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			string name = ((Image)sender).Name.Replace("_NotaImg", "");

			TextBox txtNota = (TextBox)this.FindName(name + "_Nota");

			if (txtNota.Visibility == System.Windows.Visibility.Collapsed)
			{
				txtNota.Visibility = System.Windows.Visibility.Visible;
				var uriSource = new Uri(up, UriKind.Relative);
				((Image)sender).Source = new BitmapImage(uriSource);
			}
			else
			{
				txtNota.Visibility = System.Windows.Visibility.Collapsed;
				var uriSource = new Uri(down, UriKind.Relative);
				((Image)sender).Source = new BitmapImage(uriSource);
			}
		}

		private void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			string tipo = ((Image)sender).Name.Split('_').Last();
			string name = ((Image)sender).Name.Split('_')[1];

			Image img = (Image)this.FindName("_" + name + "_Alta");
			var uriSource = new Uri(uncheck, UriKind.Relative);
			img.Source = new BitmapImage(uriSource);
			img = (Image)this.FindName("_" + name + "_Bassa");
			uriSource = new Uri(uncheck, UriKind.Relative);
			img.Source = new BitmapImage(uriSource);
			img = (Image)this.FindName("_" + name + "_NA");
			uriSource = new Uri(uncheck, UriKind.Relative);
			img.Source = new BitmapImage(uriSource);

			XmlNode tmpnode = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + _ID + "']/Node[@ID='" + name + "']");
			if (tmpnode != null && tmpnode.Attributes["Valore"] != null)
			{
				if (tmpnode.Attributes["Valore"].Value == name)
				{
					tmpnode.Attributes["Valore"].Value = "";
				}
				else
				{
					tmpnode.Attributes["Valore"].Value = tipo;

					var uriSourceint = new Uri(check, UriKind.Relative);
					((Image)sender).Source = new BitmapImage(uriSourceint);
				}
			}
		}

        private void UserControl_SizeChanged( object sender, SizeChangedEventArgs e )
        {
            Resizer( Convert.ToInt32( e.NewSize.Width ) );
        }

        public void Resizer( int newsize )
        {
            double actualwidth = ((Grid)(txtDescrizioneIntensita.Parent)).ActualWidth;

            for ( int i = 1; i < brdDefinizione.Children.Count; i++ )
            {
                Grid grid = ((Grid)(((Border)(brdDefinizione.Children[i])).Child));
                ((TextBox)(grid.Children[4])).Width = actualwidth - 495;
                ((TextBox)(grid.Children[7])).Width = actualwidth - 250;
            }

            txtConsiderazioni.Width = actualwidth - 100;

        }

    }
}
