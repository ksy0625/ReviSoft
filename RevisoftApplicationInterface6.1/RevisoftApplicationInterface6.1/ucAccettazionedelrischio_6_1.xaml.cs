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
	public partial class ucAccettazionedelrischio_6_1 : UserControl
    {
          public int id;
          private DataTable dati = null;
       
          private XmlDataProviderManager _x = null;
          private string _ID = "";

	      private bool _ReadOnly = false;


     
        public ucAccettazionedelrischio_6_1()
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
         

          dati = cBusinessObjects.GetData(id, typeof(Accettazionedelrischio_6_1));
         
          if(dati.Rows.Count==0)
            {
                DataRow dd = dati.Rows.Add();
                dd["ID_CLIENTE"] =cBusinessObjects.idcliente;
                dd["ID_SESSIONE"] =cBusinessObjects.idsessione;

            }
          foreach (DataRow dtrow in dati.Rows)
            {
            
                txtRischio.Text = dtrow["Rischio"].ToString();
                
            }
          

        }
        
    
    public int Save()
    {
            double temp = 0;
      foreach (DataRow dtrow in dati.Rows)
      {
                dtrow["Rischio"] = txtRischio.Text;
             
             
      }


        return cBusinessObjects.SaveData(id, dati, typeof(Accettazionedelrischio_6_1));

    }

    
      

    }
}
