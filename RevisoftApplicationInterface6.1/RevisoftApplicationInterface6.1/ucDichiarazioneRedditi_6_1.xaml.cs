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
using System.Globalization;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;
using System.Threading;
using RevisoftApplication;
using System.Data;

namespace UserControls
{
	public partial class ucDichiarazioneRedditi_6_1 : UserControl
    {
          public int id;
          private DataTable datirate = null;
          private DataTable datidichiarazione = null;

          private XmlDataProviderManager _x = null;
          private string _ID = "";

	      private bool _ReadOnly = false;


        GenericTable gtdati1 = null;
     
        public ucDichiarazioneRedditi_6_1()
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("it-IT"); 
            InitializeComponent();
        }

		public bool ReadOnly
		{
			set
			{
				_ReadOnly = value;
			}
		}
     

        public void LoadDataSource(string ID, string IDCliente, string IDSessione)
        {

            id = int.Parse(ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());

           _ID =  ID;
         

          datirate = cBusinessObjects.GetData(id, typeof(DichiarazioneRedditi_6_1_Rate));
          datidichiarazione=  cBusinessObjects.GetData(id, typeof(DichiarazioneRedditi_6_1));
          if(datidichiarazione.Rows.Count==0)
            {
                DataRow dd = datidichiarazione.Rows.Add();
                dd["ID_CLIENTE"] =cBusinessObjects.idcliente;
                dd["ID_SESSIONE"] =cBusinessObjects.idsessione;

            }
          foreach (DataRow dtrow in datidichiarazione.Rows)
            {
            
                txt_periodo_imposta.Text = dtrow["periodo_imposta"].ToString();
            
                txt_H3.Text = dtrow["H3"].ToString();
                txt_H4.Text = dtrow["H4"].ToString();
                txt_C1R1.Text = dtrow["C1R1"].ToString();
                txt_C2R1.Text = dtrow["C2R1"].ToString();
                txt_C3R1.Text = dtrow["C3R1"].ToString();
                txt_C4R1.Text = dtrow["C4R1"].ToString();
                txt_C1R2.Text = dtrow["C1R2"].ToString();
                txt_C2R2.Text = dtrow["C2R2"].ToString();
                txt_C3R2.Text = dtrow["C3R2"].ToString();
                txt_C4R2.Text = dtrow["C4R2"].ToString();
                txt_C1R3.Text = dtrow["C1R3"].ToString();
                txt_C2R3.Text = dtrow["C2R3"].ToString();
                txt_C3R3.Text = dtrow["C3R3"].ToString();
                txt_C4R3.Text = dtrow["C4R3"].ToString();        
                txt_C1R4.Text = dtrow["C1R4"].ToString();
                txt_C2R4.Text = dtrow["C2R4"].ToString();
                txt_C3R4.Text = dtrow["C3R4"].ToString();
                txt_C4R4.Text = dtrow["C4R4"].ToString();        
                txt_C1R5.Text = dtrow["C1R5"].ToString();
                txt_C2R5.Text = dtrow["C2R5"].ToString();
                txt_C3R5.Text = dtrow["C3R5"].ToString();
                txt_C4R5.Text = dtrow["C4R5"].ToString();        
    
                txt_C1ACC1.Text = dtrow["C1ACC1"].ToString();
                txt_C2ACC1.Text = dtrow["C2ACC1"].ToString();
                txt_C3ACC1.Text = dtrow["C3ACC1"].ToString();
                txt_C1ACC1.Text = dtrow["C4ACC1"].ToString();   
                txt_compensazione1.Text = dtrow["compensazione1"].ToString();   
                txt_C1ACC2.Text = dtrow["C1ACC2"].ToString();
                txt_C2ACC2.Text = dtrow["C2ACC2"].ToString();
                txt_C3ACC2.Text = dtrow["C3ACC2"].ToString();
                txt_C1ACC2.Text = dtrow["C4ACC2"].ToString();   
                txt_compensazione2.Text = dtrow["compensazione2"].ToString();   
                txt_datapagamento.Text = dtrow["datapagamento"].ToString();   
     
            }

            txt_H1.Text = "IRES";
            txt_H1.IsReadOnly = true;
            txt_H2.Text = "IRAP";
            txt_H2.IsReadOnly = true;
            gtdati1 = new GenericTable( tbrate, _ReadOnly);

            gtdati1.ColumnsAlias = new string[] { "Rateizzi", "Scadenze", "Rata",  "Pagato il"};
            gtdati1.ColumnsValues = new string[] { "ID", "scadenze", "rata","pagatoil" };
            gtdati1.ColumnsWidth = new double[] { 1.0, 1.0, 1.0,1.0 };
            gtdati1.ColumnsMinWidth = new double[] { 0.0, 0.0, 0.0 ,0.0};
            gtdati1.ColumnsTypes = new string[] { "string", "string", "money", "string"  };
            gtdati1.ColumnsAlignment = new string[] { "center", "left", "right", "left"  };
            gtdati1.ColumnsReadOnly = new bool[] { true, false, false, false };
            gtdati1.ConditionalReadonly = new bool[] { false, false, false,false };
            gtdati1.ConditionalAttribute = "new";
            gtdati1.ColumnsHasTotal = new bool[] { false, false, false,false };
            gtdati1.dati = datirate;
            gtdati1.xml = false;
            gtdati1.GenerateTable();
            generatot();
        }
        
    
    public int Save()
    {
            double temp = 0;
      foreach (DataRow dtrow in datidichiarazione.Rows)
      {
                dtrow["periodo_imposta"] = txt_periodo_imposta.Text;
                dtrow["H1"] = txt_H1.Text;
                dtrow["H2"] = txt_H2.Text;
                dtrow["H3"] = txt_H3.Text;
                dtrow["H4"] = txt_H4.Text;
                dtrow["C1R1"] = txt_C1R1.Text;
                dtrow["C2R1"] = txt_C2R1.Text;
                dtrow["C3R1"] = txt_C3R1.Text;
                dtrow["C4R1"] = txt_C4R1.Text;

                dtrow["C1R2"] = txt_C1R2.Text;
                dtrow["C2R2"] = txt_C2R2.Text;
                dtrow["C3R2"] = txt_C3R2.Text;
                dtrow["C4R2"] = txt_C4R2.Text;

                dtrow["C1R3"] = txt_C1R3.Text;
                dtrow["C2R3"] = txt_C2R3.Text;
                dtrow["C3R3"] = txt_C3R3.Text;
                dtrow["C4R3"] = txt_C4R3.Text;

                dtrow["C1R4"] = txt_C1R4.Text;
                dtrow["C2R4"] = txt_C2R4.Text;
                dtrow["C3R4"] = txt_C3R4.Text;
                dtrow["C4R4"] = txt_C4R4.Text;


                temp = 0;
                double.TryParse(txt_C1R5.Text, out temp);
                dtrow["C1R5"] = temp;
                temp = 0;
                double.TryParse(txt_C2R5.Text, out temp);
                dtrow["C2R5"] = temp;
                temp = 0;
                double.TryParse(txt_C3R5.Text, out temp);
                dtrow["C3R5"] = temp;
                temp = 0;
                double.TryParse(txt_C4R5.Text, out temp);
                dtrow["C4R5"] = temp;

                temp = 0;
                double.TryParse(txt_C1ACC1.Text, out temp);
                dtrow["C1ACC1"] = temp;

                temp = 0;
                double.TryParse(txt_C2ACC1.Text, out temp);
                dtrow["C2ACC1"] = temp;

                temp = 0;
                double.TryParse(txt_C3ACC1.Text, out temp);
                dtrow["C3ACC1"] = temp;

                temp = 0;
                double.TryParse(txt_C4ACC1.Text, out temp);
                dtrow["C4ACC1"] = temp;

                temp = 0;
                double.TryParse(txt_compensazione1.Text, out temp);
                dtrow["compensazione1"] = temp;
             
                temp = 0;
                double.TryParse(txt_C1ACC2.Text, out temp);
                dtrow["C1ACC2"] = temp;

                temp = 0;
                double.TryParse(txt_C2ACC2.Text, out temp);
                dtrow["C2ACC2"] = temp;

                temp = 0;
                double.TryParse(txt_C3ACC2.Text, out temp);
                dtrow["C3ACC2"] = temp;

                temp = 0;
                double.TryParse(txt_C4ACC2.Text, out temp);
                dtrow["C4ACC2"] = temp;

                temp = 0;
                double.TryParse(txt_compensazione2.Text, out temp);
                dtrow["compensazione2"] = temp;

                dtrow["datapagamento"] = txt_datapagamento.Text;


             

             
      }


      cBusinessObjects.SaveData(id, datirate, typeof(DichiarazioneRedditi_6_1_Rate));
      return cBusinessObjects.SaveData(id, datidichiarazione, typeof(DichiarazioneRedditi_6_1));

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

		private void txtPeriodoDiRiferimento_LostFocus(object sender, RoutedEventArgs e)
		{
            
   
		}

		private void txtSaldoSchedaContabile_LostFocus(object sender, RoutedEventArgs e)
		{
			
	        	
			
		}

        private void txt_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tt = (TextBox)(sender);
            try
            {
                tt.Text = cBusinessObjects.ConvertInteger(tt.Text);
                
            }
            catch (Exception eee)
            {
                tt.Text = "";
            }
              generatot();
        }
         private void   generatot()
        {
          

            double ttot = 0;
            double temp = 0;
          
            double.TryParse(txt_C1R5.Text, out temp);
            if(temp>0)
              ttot += temp;

            double.TryParse(txt_C2R5.Text, out temp);
            if(temp>0)
              ttot += temp;

             double.TryParse(txt_C3R5.Text, out temp);
            if(temp>0)
              ttot += temp;

            double.TryParse(txt_C4R5.Text, out temp);
            if(temp>0)
              ttot += temp;

            txtTotale_debitiimposta.Text=cBusinessObjects.ConvertInteger(ttot.ToString());
            temp = 0;
           
            double.TryParse(txt_C1ACC1.Text, out temp);
            ttot += temp;
            double.TryParse(txt_C2ACC1.Text, out temp);
            ttot += temp;
            double.TryParse(txt_C3ACC1.Text, out temp);
            ttot += temp;
            double.TryParse(txt_C4ACC1.Text, out temp);
            ttot += temp;

            txtTotale.Text=cBusinessObjects.ConvertInteger(ttot.ToString());
            temp = 0;
            double.TryParse(txt_compensazione1.Text, out temp);
            ttot += temp;

            txt_saldo_da_pagare.Text=cBusinessObjects.ConvertInteger(ttot.ToString());

            temp = 0;
            ttot = 0;

            double.TryParse(txt_C1ACC2.Text, out temp);
            ttot += temp;
            double.TryParse(txt_C2ACC2.Text, out temp);
            ttot += temp;
            double.TryParse(txt_C3ACC2.Text, out temp);
            ttot += temp;
            double.TryParse(txt_C4ACC2.Text, out temp);
            ttot += temp;

            temp = 0;
            double.TryParse(txt_compensazione2.Text, out temp);
            ttot += temp;

            txt_saldo_da_pagare2.Text=cBusinessObjects.ConvertInteger(ttot.ToString());
            
        }

    }
}
