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
using RevisoftApplication;
using System.Data;

namespace UserControls
{
	public partial class uc_Excel_NumeriCasuali : UserControl
    {
        public int id;
        private DataTable dati = null;
        private bool _ReadOnly = false;
		
		public bool ReadOnly
		{
			set
			{
				_ReadOnly = value;
			}
		}

		public uc_Excel_NumeriCasuali()
        {			
            InitializeComponent();

			CultureInfo culture = CultureInfo.CreateSpecificCulture("it-IT");
        }

        public void LoadDataSource(string ID, string IDCliente, string IDSessione)   
        {
		
	

           dati = cBusinessObjects.GetData(id, typeof(Excel_NumeriCasuali));
           foreach (DataRow dtrow in dati.Rows)
            {

            if (dtrow["txt1"] != null)
			{
				txt1.Text = dtrow["txt1"].ToString();
			}

			if (dtrow["txt2"] != null)
			{
				txt2.Text = dtrow["txt2"].ToString();
			}

			if (dtrow["txt3"] != null)
			{
				txt3.Text = dtrow["txt3"].ToString();
			}

			if (dtrow["txt4"] != null)
			{
				txt4.Text = dtrow["txt4"].ToString();
			}

			if (dtrow["txt5"] != null)
			{
				txt5.Text = dtrow["txt5"].ToString();
			}

			if (dtrow["txt6"] != null)
			{
				txt6.Text = dtrow["txt6"].ToString();
			}

			if (dtrow["txt7"] != null)
			{
				txt7.Text = dtrow["txt7"].ToString();
			}

			if (dtrow["txt8"] != null)
			{
				txt8.Text = dtrow["txt8"].ToString();
			}

			if (dtrow["txt9"] != null)
			{
				txt9.Text = dtrow["txt9"].ToString();
			}

			if (dtrow["txt10"] != null)
			{
				txt10.Text = dtrow["txt10"].ToString();
			}

			if (dtrow["txt11"] != null)
			{
				txt11.Text = dtrow["txt11"].ToString();
			}

			if (dtrow["txt12"] != null)
			{
				txt12.Text = dtrow["txt12"].ToString();
			}

			if (dtrow["txt13"] != null)
			{
				txt13.Text = dtrow["txt13"].ToString();
			}

			if (dtrow["txt14"] != null)
			{
				txt14.Text = dtrow["txt14"].ToString();
			}
            }
        }

        private void buttonGeneraNumeriCasuali_Click(object sender, RoutedEventArgs ea)
        {
            GeneraNumeriCasuali();
        }

		public int Save()
		{
            if (dati.Rows.Count == 0)
                dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);

            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["txt1"] = txt1.Text;
                dtrow["txt2"] = txt2.Text;
                dtrow["txt3"] = txt3.Text;
                dtrow["txt4"] = txt4.Text;
                dtrow["txt5"] = txt5.Text;
                dtrow["txt6"] = txt6.Text;
                dtrow["txt7"] = txt7.Text;
                dtrow["txt8"] = txt8.Text;
                dtrow["txt9"] = txt9.Text;
                dtrow["txt10"] = txt10.Text;
                dtrow["txt11"] = txt11.Text;
                dtrow["txt12"] = txt12.Text;
                dtrow["txt13"] = txt13.Text;
                dtrow["txt14"] = txt14.Text;
            }
            return cBusinessObjects.SaveData(id, dati, typeof(Excel_NumeriCasuali));
		}
		
        private void GeneraNumeriCasuali()
        {
            int tra = 0;
            int e = 0;
            
            try 
	        {
                tra = Convert.ToInt32(txtTra.Text);
	        }
	        catch (Exception ex)
	        {
                cBusinessObjects.logger.Error(ex, "wWorkArea_Excel_NumeriCasuali.GeneraNumeriCasuali1 exception");
                txtTra.Text = tra.ToString();
                string log = ex.Message;
	        }

            try
            {
                e = Convert.ToInt32(txtE.Text);
            }
            catch (Exception ex)
            {
                cBusinessObjects.logger.Error(ex, "wWorkArea_Excel_NumeriCasuali.GeneraNumeriCasuali2 exception");
                txtE.Text = e.ToString();
                string log = ex.Message;
            }

            if (e < tra)
            {
                txtTra.Text = e.ToString();
                txtE.Text = tra.ToString();

                e = tra;
                tra = Convert.ToInt32(txtTra.Text);
            }

            ArrayList AlreadyGiven = new ArrayList();

            Populate(txt1, ref AlreadyGiven, tra, e);
            Populate(txt2, ref AlreadyGiven, tra, e);
            Populate(txt3, ref AlreadyGiven, tra, e);
            Populate(txt4, ref AlreadyGiven, tra, e);
            Populate(txt5, ref AlreadyGiven, tra, e);
            Populate(txt6, ref AlreadyGiven, tra, e);
            Populate(txt7, ref AlreadyGiven, tra, e);
            Populate(txt8, ref AlreadyGiven, tra, e);
            Populate(txt9, ref AlreadyGiven, tra, e);
            Populate(txt10, ref AlreadyGiven, tra, e);
            Populate(txt11, ref AlreadyGiven, tra, e);
            Populate(txt12, ref AlreadyGiven, tra, e);
            Populate(txt13, ref AlreadyGiven, tra, e);
            Populate(txt14, ref AlreadyGiven, tra, e);            
        }
		
        private void Populate(TextBox t, ref ArrayList ag, int tra, int e)
        {
            Random r = new Random(tra);
            int val = r.Next(tra, e);

            int counter = 0;

            while (ag.Contains(val) && counter < 1000)
            {
                val = r.Next(tra, e);
                counter++;
            }

            if (ag.Contains(val))
            {
                t.Text = "";
            }
            else
            {
                ag.Add(val);

                t.Text = val.ToString();
            }
        }
    }
}
