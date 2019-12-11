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

    public partial class ucCompensiERisorse : UserControl
    {

        public int id;
        private DataTable datigtEsecutoriRevisione = null;
        private DataTable datigtCompensoRevisione = null;
        private DataTable datigtTerminiEsecuzione = null;


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
        private string IDCompensiERisorse = "42";

        private bool _ReadOnly = false;
        private bool _StartingCalculation = true;

        GenericTable gtEsecutoriRevisione = null;
        GenericTable gtCompensoRevisione = null;
        GenericTable gtTerminiEsecuzione = null;

        public bool ReadOnly
        {
            set
            {
                _ReadOnly = value;
            }
        }

        public ucCompensiERisorse()
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
            bool oldNode = false;

                 

             datigtEsecutoriRevisione = cBusinessObjects.GetData(id, typeof(CompensiERisorse_EsecutoriRevisione));
             datigtCompensoRevisione = cBusinessObjects.GetData(id, typeof(CompensiERisorse_CompensoRevisione));
             datigtTerminiEsecuzione = cBusinessObjects.GetData(id, typeof(CompensiERisorse_TerminiEsecuzione));


            if (datigtTerminiEsecuzione.Rows.Count > 0)
            {
                oldNode = true;
            }

            oldNode = true;

            foreach (DataRow dtrow in datigtCompensoRevisione.Rows)
              {
                try
                  {
                    txtTotale.Text = (dtrow["txtTotale"] != null) ? dtrow["txtTotale"].ToString() : "";
                    txtTariffaOraria.Text = (dtrow["txtTariffaOraria"] != null) ? dtrow["txtTariffaOraria"].ToString() : "";
                    txtCompenso.Text = (dtrow["txtCompenso"] != null) ? dtrow["txtCompenso"].ToString() : "";
                  }
                  catch (Exception)
                  {
                  
                  }
              }
     

            _StartingCalculation = false;

          

            if (oldNode)
            {
                gtEsecutoriRevisione = new GenericTable( tblEsecutoriRevisione, _ReadOnly);

                gtEsecutoriRevisione.ColumnsAlias = new string[] { "Nominativo", "Qualifica" };
                gtEsecutoriRevisione.ColumnsValues = new string[] { "nome", "qualifica" };
                gtEsecutoriRevisione.ColumnsWidth = new double[] { 1.0, 1.0 };
                gtEsecutoriRevisione.ColumnsMinWidth = new double[] { 0.0, 0.0 };
                gtEsecutoriRevisione.ColumnsTypes = new string[] { "string", "string" };
                gtEsecutoriRevisione.ColumnsAlignment = new string[] { "left", "left" };
                gtEsecutoriRevisione.ConditionalReadonly = new bool[] { false, false };
                gtEsecutoriRevisione.ConditionalAttribute = "new";
                gtEsecutoriRevisione.ColumnsHasTotal = new bool[] { false, false };
                gtEsecutoriRevisione.AliasTotale = "Totale";
                gtEsecutoriRevisione.ColumnAliasTotale = 1;
                gtEsecutoriRevisione.dati = datigtEsecutoriRevisione;
                gtEsecutoriRevisione.xml = false;
                gtEsecutoriRevisione.GenerateTable();
            }
            else
            {
                brdEsecutoriRevisione.Visibility = Visibility.Collapsed;
            }

            if (oldNode)
            {
                gtCompensoRevisione = new GenericTable( tblCompensoRevisione, _ReadOnly);

                gtCompensoRevisione.ColumnsAlias = new string[] { "Fase", "Attività", "Esecutore - Personale assegnato", "Ore" };
                gtCompensoRevisione.ColumnsValues = new string[] { "fase", "attivita", "esecutore", "ore" };
                gtCompensoRevisione.ColumnsWidth = new double[] { 1.0, 8.0, 4.0, 2.0 };
                gtCompensoRevisione.ColumnsMinWidth = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
                gtCompensoRevisione.ColumnsTypes = new string[] { "string", "string", "string", "money" };
                gtCompensoRevisione.ColumnsAlignment = new string[] { "left", "left", "left", "right" };
                gtCompensoRevisione.ConditionalReadonly = new bool[] { true, true, false, false };
                gtCompensoRevisione.ConditionalAttribute = "new";
                gtCompensoRevisione.ColumnsHasTotal = new bool[] { false, false, false, false };
                gtCompensoRevisione.AliasTotale = "Totale";
                gtCompensoRevisione.ColumnAliasTotale = 1;
                gtCompensoRevisione.dati = datigtCompensoRevisione;
                gtCompensoRevisione.xml = false;
                gtCompensoRevisione.TotalToBeCalculated += GtCompensoRevisione_TotalToBeCalculated;
                gtCompensoRevisione.GenerateTable();

                if (datigtCompensoRevisione.Rows.Count == 0)
                {
               
                    string Attivita = "Fase INTERIM: Determinazione del rischio e della materialità - Pianificazione della revisione";
                    string Fase = "1";
                    datigtCompensoRevisione.Rows.Add(id,cBusinessObjects.idcliente,cBusinessObjects.idsessione, Fase, Attivita);
            
                    Attivita = "Partecipazioni all'inventario di magazzino";
                    Fase = "2";
                    datigtCompensoRevisione.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Fase, Attivita);
              
                    Attivita = "Fase FINAL: Controllo del bilancio come pianificato";
                    Fase = "3";
                   
                    datigtCompensoRevisione.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Fase, Attivita);
                    Attivita = "Redazione Relazione";
                    Fase = "4";
                    datigtCompensoRevisione.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Fase, Attivita);
                    gtCompensoRevisione.GenerateTable();
                    recalculateTotal();
                }
            }
            else
            {
                gtCompensoRevisione = new GenericTable( tblCompensoRevisione, _ReadOnly);

                gtCompensoRevisione.ColumnsAlias = new string[] { "Fase", "Attività", "Esecutore - Personale assegnato", "Data Termine", "Ore" };
                gtCompensoRevisione.ColumnsValues = new string[] { "fase", "attivita", "esecutore", "termini", "ore" };
                gtCompensoRevisione.ColumnsWidth = new double[] { 1.0, 7.0, 3.0, 1.0, 1.0 };
                gtCompensoRevisione.ColumnsMinWidth = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
                gtCompensoRevisione.ColumnsTypes = new string[] { "string", "string", "string", "string", "money" };
                gtCompensoRevisione.ColumnsAlignment = new string[] { "left", "left", "left", "right", "right" };
                gtCompensoRevisione.ConditionalReadonly = new bool[] { true, true, false, false, false };
                gtCompensoRevisione.ConditionalAttribute = "new";
                gtCompensoRevisione.ColumnsHasTotal = new bool[] { false, false, false, false, false };
                gtCompensoRevisione.AliasTotale = "Totale";
                gtCompensoRevisione.ColumnAliasTotale = 1;
                gtCompensoRevisione.dati = datigtCompensoRevisione;
                gtCompensoRevisione.xml = false;
                gtCompensoRevisione.TotalToBeCalculated += GtCompensoRevisione_TotalToBeCalculated;
                gtCompensoRevisione.GenerateTable();

                  if (datigtCompensoRevisione.Rows.Count == 0)
                  {
                  
                    string Attivita = "Fase INTERIM: Determinazione del rischio e della materialità - Pianificazione della revisione";
                    string Fase = "1";
                    datigtCompensoRevisione.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Fase, Attivita);


                    Attivita = "Partecipazioni all'inventario di magazzino";
                    Fase = "2";
                    datigtCompensoRevisione.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Fase, Attivita);
                

                    Attivita = "Fase FINAL: Controllo del bilancio come pianificato";
                    Fase = "3";
                    datigtCompensoRevisione.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Fase, Attivita);
                 

                    Attivita = "Redazione Relazione";
                    Fase = "4";
                    datigtCompensoRevisione.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Fase, Attivita);
                    gtCompensoRevisione.GenerateTable();
                    recalculateTotal();
                }
            }

            if (oldNode)
            {
                gtTerminiEsecuzione = new GenericTable( tblTerminiEsecuzione, _ReadOnly);

                gtTerminiEsecuzione.ColumnsAlias = new string[] { "Fase", "Attività da svolgere", "Data Termine" };
                gtTerminiEsecuzione.ColumnsValues = new string[] { "fase", "attivita", "termini" };
                gtTerminiEsecuzione.ColumnsWidth = new double[] { 1.0, 9.0, 5.0 };
                gtTerminiEsecuzione.ColumnsMinWidth = new double[] { 0.0, 0.0, 0.0 };
                gtTerminiEsecuzione.ColumnsTypes = new string[] { "string", "string", "string" };
                gtTerminiEsecuzione.ColumnsAlignment = new string[] { "left", "left", "right" };
                gtTerminiEsecuzione.ConditionalReadonly = new bool[] { true, true, false };
                gtTerminiEsecuzione.ConditionalAttribute = "new";
                gtTerminiEsecuzione.ColumnsHasTotal = new bool[] { false, false, false };
                gtTerminiEsecuzione.AliasTotale = "Totale";
                gtTerminiEsecuzione.ColumnAliasTotale = 1;
                gtTerminiEsecuzione.dati = datigtTerminiEsecuzione;
                gtTerminiEsecuzione.xml = false;
                gtTerminiEsecuzione.GenerateTable();

               if (datigtTerminiEsecuzione.Rows.Count == 0)
                 {
                    gtTerminiEsecuzione.Xpathparentnode = "//Dati/Dato[@ID" + _ID + "]";

                    string Attivita = "Fase INTERIM: Determinazione del rischio e della materialità - Pianificazione della revisione";
                    string Fase = "1";
                    datigtTerminiEsecuzione.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Fase, Attivita);
                    Attivita = "Partecipazioni all'inventario di magazzino";

                    Fase = "2";
                    datigtTerminiEsecuzione.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Fase, Attivita);
             
                    Attivita = "Fase FINAL: Controllo del bilancio come pianificato";
                    Fase = "3";
                    datigtTerminiEsecuzione.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Fase, Attivita);
             
                    Attivita = "Redazione Relazione";
                    Fase = "4";
                    datigtTerminiEsecuzione.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, Fase, Attivita);
                    gtTerminiEsecuzione.GenerateTable();
                }
            }
            else
            {
                brdTerminiEsecuzione.Visibility = Visibility.Collapsed;
            }
            recalculateTotal();
        }

        private void GtCompensoRevisione_TotalToBeCalculated(object sendername, EventArgs e)
        {
            if (((string)sendername).Split('_').Count() < 2)
            {
                return;
            }

            string idcolumn = ((string)sendername).Split('_')[1];
            string idrow = ((string)sendername).Split('_')[2];
            
            if (idcolumn != "3" && idcolumn != "4" )
            {
                return;
            }

            recalculateTotal();
        }

        public void recalculateTotal()
        {
            txtTotale.Text = gtCompensoRevisione.GenerateSpecificTotal("4");

            if (txtTotale.Text == "")
            {
                txtTotale.Text = gtCompensoRevisione.GenerateSpecificTotal("3");
                if (txtTotale.Text == "")
                {
                    txtTotale.Text = (Convert.ToDouble(0)).ToString();
                }
            }

            if (txtTariffaOraria.Text == "")
            {
                txtTariffaOraria.Text = (Convert.ToDouble(0)).ToString();
            }

             txtCompenso.Text = (Convert.ToDouble(txtTotale.Text) * ((txtTariffaOraria.Text == "") ? Convert.ToDouble(0) : Convert.ToDouble(txtTariffaOraria.Text))).ToString();
             txtTariffaOraria.Text = cBusinessObjects.ConvertNumber(txtTariffaOraria.Text);
             txtCompenso.Text =  cBusinessObjects.ConvertNumber(txtCompenso.Text);
          

        }

        public int Save()
		{

            foreach (DataRow dtrow in datigtCompensoRevisione.Rows)
            {
               
                try
                {
                    dtrow["txtTotale"]= double.Parse(txtTotale.Text);
                }
                catch (Exception)
                {
                    dtrow["txtTotale"] = 0;
                }
                try
                {
                    dtrow["txtCompenso"] = double.Parse(txtCompenso.Text);
                }
                catch (Exception)
                {
                    dtrow["txtCompenso"] = 0;
                }
                try
                {
                    dtrow["txtTariffaOraria"] = double.Parse(txtTariffaOraria.Text);
                }
                catch (Exception)
                {
                    dtrow["txtTariffaOraria"] = 0;
                }
            }

            cBusinessObjects.SaveData(id, datigtEsecutoriRevisione, typeof(CompensiERisorse_EsecutoriRevisione));
            cBusinessObjects.SaveData(id, datigtCompensoRevisione, typeof(CompensiERisorse_CompensoRevisione));
            return cBusinessObjects.SaveData(id, datigtTerminiEsecuzione, typeof(CompensiERisorse_TerminiEsecuzione));
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


#region ESECUTORI Revisione
        private void AggiungiNodoEsecutoriRevisione( )
        {
          
            gtEsecutoriRevisione.AddRow();
        }

        private void AddRowEsecutoriRevisione( object sender, RoutedEventArgs e )
        {
            AggiungiNodoEsecutoriRevisione( );
        }

        private void DeleteRowEsecutoriRevisione( object sender, RoutedEventArgs e )
        {
            gtEsecutoriRevisione.DeleteRow();
                  
        }

#endregion

#region COMPENSO Revisione
       

        private void AddRowCompensoRevisione( object sender, RoutedEventArgs e )
        {
            gtCompensoRevisione.AddRow(true);
           
        }

        private void DeleteRowCompensoRevisione( object sender, RoutedEventArgs e )
        {
            gtCompensoRevisione.DeleteRow();
            recalculateTotal();
            return;
        }
        
#endregion

#region TERMINI ESECUZIONE
        private void AggiungiNodoTerminiEsecuzione( )
        {
            gtTerminiEsecuzione.AddRow(true);
        }

        private void AggiungiNodoTerminiEsecuzioneNew(string Fase, string ID, string Attivita, string Termini)
        {
            gtTerminiEsecuzione.AddRow(true);

        }

        private void AddRowTerminiEsecuzione( object sender, RoutedEventArgs e )
        {
            AggiungiNodoTerminiEsecuzione( );
        }

        private void DeleteRowTerminiEsecuzione( object sender, RoutedEventArgs e )
        {
            gtTerminiEsecuzione.DeleteRow();
           
        }

        private void CopiaDaCompenso(object sender, RoutedEventArgs e)
        {
            foreach (DataRow dtrow in datigtCompensoRevisione.Rows)
            {
                AggiungiNodoTerminiEsecuzioneNew(dtrow["fase"].ToString(), _ID, dtrow["attivita"].ToString(), "");
            }

       
            
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

            txtTotale.Width = tblCompensoRevisione.ActualWidth / 13;
            txtTariffaOraria.Width = tblCompensoRevisione.ActualWidth / 13;
            txtCompenso.Width = tblCompensoRevisione.ActualWidth / 13;
        }
    }
}
