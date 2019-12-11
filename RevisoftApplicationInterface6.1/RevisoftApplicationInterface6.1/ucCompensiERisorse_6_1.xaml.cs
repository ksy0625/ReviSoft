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

    public partial class ucCompensiERisorse_6_1 : UserControl
    {

        public int id;
        private DataTable datigtCompensoTtotali = null;
        private DataTable datigtCompensoRevisione = null;    
        private DataTable datiStimaore = null;
      

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
        private string IDCompensiERisorse = "42";

        private bool _ReadOnly = false;
        private bool _StartingCalculation = true;

        GenericTable gtCompensoRevisione = null;
        GenericTable gtTotali =null;
   
        public bool ReadOnly
        {
            set
            {
                _ReadOnly = value;
            }
        }

        public ucCompensiERisorse_6_1()
        {
            if (Offset==0 || OffsetNote==0 || Minimo==0 || check.Equals("")
                || uncheck.Equals("") || up.Equals("") || IDCompensiERisorse.Equals("")
                || _StartingCalculation) { }
            InitializeComponent();
        }

        public void Load(string ID, string IDCliente, string IDSessione)
        {

            id = int.Parse(ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());

            _ID =  ID;
        

            datiStimaore = cBusinessObjects.GetData(id, typeof(CompensiERisorse_CompensoRevisione_Stimaore_6_1));  
            if(datiStimaore.Rows.Count==0)
            {
                DataRow aa = datiStimaore.Rows.Add();
                aa["ID_SESSIONE"]= cBusinessObjects.idsessione;
                aa["ID_CLIENTE"]= cBusinessObjects.idcliente;
            }

            foreach(DataRow dtt in datiStimaore.Rows)
            {
                txtTotale_Attivo.Text = dtt["Totale_Attivo"].ToString(); 
                txtTotale_Ricavi.Text = dtt["Totale_Ricavi"].ToString(); 
                txtSettore.Text = dtt["txtSettore"].ToString(); 
                txtRischio.Text = dtt["txtRischio"].ToString(); 

            }
           



            RicalcolaStimaOre();

            datigtCompensoRevisione = cBusinessObjects.GetData(id, typeof(CompensiERisorse_CompensoRevisione_6_1));
            datigtCompensoTtotali = cBusinessObjects.GetData(id, typeof(CompensiERisorse_Totali_6_1));
     
            if(datigtCompensoTtotali.Rows.Count==0)
            {
                DataRow dd=datigtCompensoTtotali.Rows.Add();
                dd["qualifica"] = "Junior";
                dd=datigtCompensoTtotali.Rows.Add();
                dd["qualifica"] = "Senior";
                dd=datigtCompensoTtotali.Rows.Add();
                dd["qualifica"] = "Reviewer";
                dd=datigtCompensoTtotali.Rows.Add();
                dd["qualifica"] = "Revisore o membro CS";
            }
         
           

       
            _StartingCalculation = false;
            
            Binding b = new Binding();
			b.Source = datigtCompensoRevisione;
			dtgAffidamenti.SetBinding(ItemsControl.ItemsSourceProperty, b);



            
            gtTotali = new GenericTable( tblTerminiEsecuzione, _ReadOnly);


            gtTotali.ColumnsAlias = new string[] { "Qualifica", "Ore", "Tariffa", "Compenso" };
            gtTotali.ColumnsValues = new string[] { "qualifica", "ore", "tariffa", "compenso"};
            gtTotali.ColumnsWidth = new double[] { 7.0, 2.0, 3.0,3.0 };
            gtTotali.ColumnsMinWidth = new double[] { 0.0, 0.0, 0.0, 0.0 };
            gtTotali.ColumnsTypes = new string[] { "string", "int", "money","money" };
            gtTotali.ColumnsAlignment = new string[] { "left","right", "right", "right" };
            gtTotali.ColumnsReadOnly = new bool[] { true, true,  false,  true  };
            gtTotali.ConditionalReadonly = new bool[] { false, false,  false,  false  };
            gtTotali.ConditionalAttribute = "new";
            gtTotali.ColumnsHasTotal = new bool[] { false, true, false, true };
            gtTotali.AliasTotale = "Totale";
            gtTotali.ColumnAliasTotale = 0;
            gtTotali.dati = datigtCompensoTtotali;
            gtTotali.xml = false;
            gtTotali.TotalToBeCalculated += GtCompensoRevisione_TotalToBeCalculated;
       
            gtTotali.GenerateTable();
      
          
        }

        private void GtCompensoRevisione_TotalToBeCalculated(object sendername, EventArgs e)
        {
            if (((string)sendername).Split('_').Count() < 2)
            {
                return;
            }

            string idcolumn = ((string)sendername).Split('_')[1];
            string idrow = ((string)sendername).Split('_')[2];
            
            if ( idcolumn == "3" )
            {
                return;
            }
             if ( idcolumn == "1" )
            {
                return;
            }
            GenerateTotal();
           
        }


        public int Save()
		{
        
            foreach(DataRow dtt in datiStimaore.Rows)
            {
            if( txtTotale_Attivo.Text!="")
                dtt["Totale_Attivo"] = txtTotale_Attivo.Text;
            if( txtTotale_Ricavi.Text!="")
                dtt["Totale_Ricavi"] = txtTotale_Ricavi.Text;
            if( txtNumeroOre.Text!="")
                dtt["txtMedia"] = txtMedia.Text;
            if( txtNumeroOre.Text!="")
                dtt["txtNumeroOre"] = txtNumeroOre.Text;
        
                dtt["txtSettore"] = txtSettore.Text;                    
            if( txtTotale_Attivo.Text!="")
                dtt["txtRischio"] = txtPercMaggRid.Text;
                dtt["txtRischio"] = txtRischio.Text;
            if( txtPercRischioMaggRid.Text!="")
                dtt["txtPercRischioMaggRid"] = txtPercRischioMaggRid.Text;
                dtt["txtTotaleOre"] = txtTotaleOre.Text;
          

            }
            cBusinessObjects.SaveData(id, datiStimaore, typeof(CompensiERisorse_CompensoRevisione_Stimaore_6_1));
            cBusinessObjects.SaveData(id, datigtCompensoRevisione, typeof(CompensiERisorse_CompensoRevisione_6_1));
            cBusinessObjects.SaveData(id, datigtCompensoTtotali, typeof(CompensiERisorse_Totali_6_1));
            return 0;
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
       

        private void AddRowCompensoRevisione( object sender, RoutedEventArgs e )
        {
            gtCompensoRevisione.AddRow(true);
           
        }

        private void DeleteRowCompensoRevisione( object sender, RoutedEventArgs e )
        {
            gtCompensoRevisione.DeleteRow();
          
            return;
        }
        
#endregion

        private void txtTariffaOraria_LostFocus( object sender, RoutedEventArgs e )
        {
            GtCompensoRevisione_TotalToBeCalculated("txt_4_0", e);
        }


        private void txtTariffaOraria_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                ((TextBox)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            gtCompensoRevisione.SetFocus();

        

        }

        private void AddRowErroriRilevati(object sender, RoutedEventArgs e)
        {
            if (_ReadOnly)
			    {
				    MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				    return;
			    }

        
               datigtCompensoRevisione.Rows.Add(id, cBusinessObjects.idcliente,cBusinessObjects.idsessione);

        }

        private void DeleteRowErroriRilevati(object sender, RoutedEventArgs e)
        {
        if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				return;
			}

			if (MessageBox.Show("Si è sicuri di procedere con l'eliminazione?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				

				if (dtgAffidamenti.SelectedCells.Count < 1)
				{
					
					MessageBox.Show("Selezionare una riga");
					return;
				}

				try
				{
                    int k = 0;
                    foreach (DataRow dtrow in this.datigtCompensoRevisione.Rows)
                    {
                        if (k == dtgAffidamenti.Items.IndexOf(dtgAffidamenti.SelectedCells[0].Item))
                        {
                        dtrow.Delete();
                        break;
                        }

                        k++;

                    }
                    this.datigtCompensoRevisione.AcceptChanges();
                    GenerateTotal();

					return;
				}
				catch (Exception ex)
				{
					string log = ex.Message;

					MessageBox.Show("Solo le righe inserite dall'utente possono essere cancellate");
				}
			}
        }

        public void GenerateTotal()
        {
       
            foreach(DataRow dtot in datigtCompensoTtotali.Rows)
            {
              
               double totaleore = 0.0;
               foreach(DataRow dt in datigtCompensoRevisione.Rows)
                {
                    if (dtot["qualifica"].ToString() == dt["qualifica"].ToString())
                    {
                        double oret = 0.0;
                        double.TryParse(dt["ore"].ToString(), out oret);
                        totaleore += oret;
                   
                     }
                }
                dtot["ore"] = totaleore;
                double tariffa = 0.0;
                double.TryParse(dtot["tariffa"].ToString(), out tariffa);
                dtot["compenso"] = Convert.ToDouble(totaleore * tariffa);
              
            }

           gtTotali.GenerateTable(); 
        }

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
          if (_ReadOnly)
			{
				MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				e.Cancel = true;
				return;
			}
        }

        private void DataGrid_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void dtgErroriRilevati_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            GenerateTotal();
        }

        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
         if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
            }
        }

        private void dtgErroriRilevati_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void txtstimaaore_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {

             TextBox tt = (TextBox)(sender);
             if(tt.Name!="txtNumeroOre" && tt.Name!="txtPercMaggRid" && tt.Name!="txtPercRischioMaggRid" && tt.Name!="txtTotaleOre"  )
                try
                {
                    tt.Text = cBusinessObjects.ConvertNumber(tt.Text);

                }
                catch (Exception eee)
                {
                    tt.Text = "";
                }
            }
        
            RicalcolaStimaOre();

        }
        
        public void RicalcolaStimaOre()
        {
            double mediatot = 0;
            double temp = 0;
            if(txtTotale_Attivo.Text!="")
            {
                  double.TryParse(txtTotale_Attivo.Text, out temp);
                  mediatot += temp;

            }
            if(txtTotale_Ricavi.Text!="")
            {
                  double.TryParse(txtTotale_Ricavi.Text, out temp);
                   mediatot += temp;
            }
            mediatot = mediatot / 2;
            txtMedia.Text = cBusinessObjects.ConvertNumber(mediatot.ToString()).ToString();

            if(mediatot>=1 && mediatot<2000000)
            {
             txtNumeroOre.Text = "80";
            }
            if(mediatot>=2000000 && mediatot<5000000)
            {
             txtNumeroOre.Text = "130";
            }
            if(mediatot>=5000000 && mediatot<7000000)
            {
             txtNumeroOre.Text = "160";
            }
            if(mediatot>=7000000 && mediatot<10000000)
            {
             txtNumeroOre.Text = "180";
            }
            if(mediatot>=10000000 && mediatot<15000000)
            {
             txtNumeroOre.Text = "220";
            }
            if(mediatot>=15000000 && mediatot<20000000)
            {
             txtNumeroOre.Text = "250";
            }
            if(mediatot>=20000000 && mediatot<30000000)
            {
             txtNumeroOre.Text = "310";
            }
            if(mediatot>=30000000 && mediatot<40000000)
            {
             txtNumeroOre.Text = "360";
            }
            if(mediatot>=40000000 && mediatot<50000000)
            {
             txtNumeroOre.Text = "400";
            }            
            if(txtSettore.SelectedIndex==0)  // industriale
            {
                txtPercMaggRid.Text = "0%";
            }
            if(txtSettore.SelectedIndex==1)  // commerciale servizi
            {
                txtPercMaggRid.Text = "-15%";
            }
            if(txtSettore.SelectedIndex==2)  // produzioni su commessa
            {
                txtPercMaggRid.Text = "10%";
            }
            if(txtSettore.SelectedIndex==3)  // immobiliare
            {
                txtPercMaggRid.Text = "-50%";
            }
            if(txtRischio.SelectedIndex==0)  // basso
            {
                txtPercRischioMaggRid.Text = "0%";
            }
            if(txtRischio.SelectedIndex==1)  // moderato
            {
                txtPercRischioMaggRid.Text = "20%";
            }
            if(txtRischio.SelectedIndex==2)  // alto
            {
                txtPercRischioMaggRid.Text = "40%";
            }
            if(txtNumeroOre.Text=="")
            {
                txtTotaleOre.Text = "";
            }
            else
            {
                int totore = 0;
                Int32.TryParse(txtNumeroOre.Text, out totore);

                if(txtPercMaggRid.Text!="")
                {
                   double.TryParse(txtPercMaggRid.Text.Replace("%",""), out temp);
                   temp = 1+temp/100;
                   totore = Convert.ToInt32(totore*temp);
                }

                 if(txtPercRischioMaggRid.Text!="")
                {
                   double.TryParse(txtPercRischioMaggRid.Text.Replace("%",""), out temp);
                   temp = 1+temp/100;
                   totore =  Convert.ToInt32(totore*temp);
                }
                
                txtTotaleOre.Text = totore.ToString();
            }
          
            

        }

    }
}
