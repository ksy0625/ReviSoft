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
using System.Data;

namespace UserControls
{
    public partial class ucLuogoDataFirma : UserControl
    {
        public int id;
        private DataTable dati = null;

        private string _ID = "";
     

        public ucLuogoDataFirma()
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
        
		public void Load( string __ID, string FileData, string IDCliente, string IDTree, string IDSessione)
        {
            id = int.Parse(__ID);
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
            _ID = __ID;

            dati = cBusinessObjects.GetData(id, typeof(LuogoDataFirma));
            MasterFile mf = MasterFile.Create();

            if ( ( mf.GetAnagrafica( Convert.ToInt32( IDCliente ) ) )["OrganoDiControllo"] != null && ( mf.GetAnagrafica( Convert.ToInt32( IDCliente ) ) )["OrganoDiControllo"].ToString() == "1" && IDTree != "21")
            {
                brdFirma.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                brdFirma.Visibility = System.Windows.Visibility.Collapsed;

                if((mf.GetAnagrafica( Convert.ToInt32( IDCliente ) ))["OrganoDiControllo"] == null)
                {
                    MessageBox.Show( "Aggiornare i dati dell'Organo di Controllo nella scheda del CLIENTE (Modifica Cliente)" );
                }
            }

            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["cmbFirma"] != null)
                {
                    cmbFirma.Text = dtrow["cmbFirma"].ToString();
                }
                if (dtrow["txtData"] != null)
                {
                    txtData.Text = dtrow["txtData"].ToString();
                }
                if (dtrow["txtLuogo"] != null)
                {
                    txtLuogo.Text = dtrow["txtLuogo"].ToString();
                }
            }
          

		
        }

        public int Save()
        {
            if (dati.Rows.Count == 0)
                dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);

            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["cmbFirma"] = cmbFirma.Text;
                dtrow["txtData"] = txtData.Text;
                dtrow["txtLuogo"] = txtLuogo.Text;
                
            }
            return cBusinessObjects.SaveData(id, dati, typeof(LuogoDataFirma));
        }
        
		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{

		}

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
