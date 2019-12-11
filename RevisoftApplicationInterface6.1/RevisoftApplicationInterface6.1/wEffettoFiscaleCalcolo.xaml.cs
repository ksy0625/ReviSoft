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
using System.Windows.Shapes;
using System.Xml;
using System.Data;

namespace RevisoftApplication
{
    public partial class wEffettoFiscaleCalcolo : Window
    {
        private string _adder = "";
        public DataRow _node = null;
        public wEffettoFiscaleCalcolo(DataRow node, string adder)
        {
            InitializeComponent();

            _node = node;

            if (adder == "")
            {
                txtImportoRettifica.Text = ConvertNumberNoDecimal(node["suutileattuale"].ToString());
            }
            else
            {
                txtImportoRettifica.Text = ConvertNumberNoDecimal(node["suPNattuale"].ToString());
            }

            _adder = adder;



            txtV_IRES.Text = ((_node["txtV_IRES" + _adder].ToString() == "")? "0": _node["txtV_IRES" + _adder].ToString());
            //txtP_IRES.Text = ((_node["txtP_IRES" + _adder] == null) ? "27,50%" : _node["txtP_IRES" + _adder].Value);
            //Iuri portata da 27,5 a 24
            txtP_IRES.Text = ((_node["txtP_IRES" + _adder].ToString() == "") ? "24,00%" : _node["txtP_IRES" + _adder].ToString());

            txtV_IRAP.Text = ((_node["txtV_IRAP" + _adder].ToString() == "") ? "0" : _node["txtV_IRAP" + _adder].ToString());
            txtP_IRAP.Text = ((_node["txtP_IRAP" + _adder].ToString() == "") ? "3,90%" : _node["txtP_IRAP" + _adder].ToString());

            txt1.Text = ((_node["txt1" + _adder].ToString() == "") ? "" : _node["txt1" + _adder].ToString());
            txtV_1.Text = ((_node["txtV_1" + _adder].ToString() == "") ? "0" : _node["txtV_1" + _adder].ToString());
            txtP_1.Text = ((_node["txtP_1" + _adder].ToString() == "") ? "0,00%" : _node["txtP_1" + _adder].ToString());

            txt2.Text = ((_node["txt2" + _adder].ToString() == "") ? "" : _node["txt2" + _adder].ToString());
            txtV_2.Text = ((_node["txtV_2" + _adder].ToString() == "") ? "0" : _node["txtV_2" + _adder].ToString());
            txtP_2.Text = ((_node["txtP_2" + _adder].ToString() == "") ? "0,00%" : _node["txtP_2" + _adder].ToString());

            txt3.Text = ((_node["txt3" + _adder].ToString() == "") ? "" : _node["txt3" + _adder].ToString());
            txtV_3.Text = ((_node["txtV_3" + _adder].ToString() == "") ? "0" : _node["txtV_3" + _adder].ToString());
            txtP_3.Text = ((_node["txtP_3" + _adder].ToString() == "") ? "0,00%" : _node["txtP_3" + _adder].ToString());

            txt4.Text = ((_node["txt4" + _adder].ToString() ==  "") ? "" : _node["txt4" + _adder].ToString());
            txtV_4.Text = ((_node["txtV_4" + _adder].ToString() == "") ? "0" : _node["txtV_4" + _adder].ToString());

            txt5.Text = ((_node["txt5" + _adder].ToString() == "") ? "" : _node["txt5" + _adder].ToString());
            txtV_5.Text = ((_node["txtV_5" + _adder].ToString() == "") ? "0" : _node["txtV_5" + _adder].ToString());

            CalcolaValori();           
        }

       

        private void btnIRESP(object sender, RoutedEventArgs e)
        {
            double totale = 0.0;
            double dblValore = 0.0;

            double.TryParse(txtImportoRettifica.Text, out totale);

            double.TryParse(txtP_IRES.Text.Replace("%", ""), out dblValore);

            txtV_IRES.Text = ConvertNumberNoDecimal((totale * dblValore / 100.0).ToString());
        }

        private void btnIRESM(object sender, RoutedEventArgs e)
        {
            double totale = 0.0;
            double dblValore = 0.0;

            double.TryParse(txtImportoRettifica.Text, out totale);

            double.TryParse(txtP_IRES.Text.Replace("%", ""), out dblValore);

            txtV_IRES.Text = ConvertNumberNoDecimal((-1.0 * totale * dblValore / 100.0).ToString());
        }

        private void btnIRAPP(object sender, RoutedEventArgs e)
        {
            double totale = 0.0;
            double dblValore = 0.0;

            double.TryParse(txtImportoRettifica.Text, out totale);

            double.TryParse(txtP_IRAP.Text.Replace("%", ""), out dblValore);

            txtV_IRAP.Text = ConvertNumberNoDecimal((totale * dblValore / 100.0).ToString());
        }

        private void btnIRAPM(object sender, RoutedEventArgs e)
        {
            double totale = 0.0;
            double dblValore = 0.0;

            double.TryParse(txtImportoRettifica.Text, out totale);

            double.TryParse(txtP_IRAP.Text.Replace("%", ""), out dblValore);

            txtV_IRAP.Text = ConvertNumberNoDecimal((-1.0 * totale * dblValore / 100.0).ToString());
        }

        private void btn1P(object sender, RoutedEventArgs e)
        {
            double totale = 0.0;
            double dblValore = 0.0;

            double.TryParse(txtImportoRettifica.Text, out totale);

            double.TryParse(txtP_1.Text.Replace("%", ""), out dblValore);

            txtV_1.Text = ConvertNumberNoDecimal((totale * dblValore / 100.0).ToString());
        }

        private void btn1M(object sender, RoutedEventArgs e)
        {
            double totale = 0.0;
            double dblValore = 0.0;

            double.TryParse(txtImportoRettifica.Text, out totale);

            double.TryParse(txtP_1.Text.Replace("%", ""), out dblValore);

            txtV_1.Text = ConvertNumberNoDecimal((-1.0 * totale * dblValore / 100.0).ToString());
        }

        private void btn2P(object sender, RoutedEventArgs e)
        {
            double totale = 0.0;
            double dblValore = 0.0;

            double.TryParse(txtImportoRettifica.Text, out totale);

            double.TryParse(txtP_2.Text.Replace("%", ""), out dblValore);

            txtV_2.Text = ConvertNumberNoDecimal((totale * dblValore / 100.0).ToString());
        }

        private void btn2M(object sender, RoutedEventArgs e)
        {
            double totale = 0.0;
            double dblValore = 0.0;

            double.TryParse(txtImportoRettifica.Text, out totale);

            double.TryParse(txtP_2.Text.Replace("%", ""), out dblValore);

            txtV_2.Text = ConvertNumberNoDecimal((-1.0 * totale * dblValore / 100.0).ToString());
        }

        private void btn3P(object sender, RoutedEventArgs e)
        {
            double totale = 0.0;
            double dblValore = 0.0;

            double.TryParse(txtImportoRettifica.Text, out totale);

            double.TryParse(txtP_3.Text.Replace("%", ""), out dblValore);

            txtV_3.Text = ConvertNumberNoDecimal((totale * dblValore / 100.0).ToString());
        }

        private void btn3M(object sender, RoutedEventArgs e)
        {
            double totale = 0.0;
            double dblValore = 0.0;

            double.TryParse(txtImportoRettifica.Text, out totale);

            double.TryParse(txtP_3.Text.Replace("%", ""), out dblValore);

            txtV_3.Text = ConvertNumberNoDecimal((-1.0 * totale * dblValore / 100.0).ToString());
        }

        private void CalcolaValori()
        {
            double totale = 0.0;
            double dblValore = 0.0;

            //double.TryParse(txtImportoRettifica.Text, out dblValore);
            //totale = dblValore;

            double.TryParse(txtV_IRES.Text, out dblValore);
            totale += dblValore;
            
            double.TryParse(txtV_IRAP.Text, out dblValore);
            totale += dblValore;

            double.TryParse(txtV_1.Text, out dblValore);
            totale += dblValore;

            double.TryParse(txtV_2.Text, out dblValore);
            totale += dblValore;
            
            double.TryParse(txtV_3.Text, out dblValore);
            totale += dblValore;

            double.TryParse(txtV_4.Text, out dblValore);
            totale += dblValore;
            
            double.TryParse(txtV_5.Text, out dblValore);
            totale += dblValore;

            txtV_TOT.Text = ConvertNumberNoDecimal(totale.ToString());
        }

        private string ConvertNumberNoDecimal(string valore)
        {
            double dblValore = 0.0;

            double.TryParse(valore, out dblValore);

            if (dblValore == 0.0)
            {
                return "";
            }
            else
            {
                return String.Format("{0:#,#}", dblValore);
            }
        }

        public string getTotal()
        {
            if (txtV_TOT.Text!="")
              _node["impattofiscale" + _adder] = txtV_TOT.Text;
            return txtV_TOT.Text;
        }

        public void setEmpty()
        {
           
            _node["impattofiscale" + _adder] = "0";     
            _node["txtV_IRES" + _adder] = "0";
            _node["txtV_IRAP" + _adder] = "0";
            _node["txtV_1" + _adder] = "0";
            _node["txtV_2" + _adder] = "0";
            _node["txtV_3" + _adder] = "0";
            _node["txtV_4" + _adder] = "0";
            _node["txtV_5" + _adder] = "0";       
            _node["txt1" + _adder] = "";      
            _node["txt2" + _adder] = "";        
            _node["txt3" + _adder] = "";    
            _node["txt4" + _adder] = "";
            _node["txt5" + _adder] = "";
       
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtV_TOT.Text!="")
              _node["impattofiscale" + _adder] = txtV_TOT.Text;
            _node["txtV_IRES" + _adder] = txtV_IRES.Text;
            _node["txtV_IRAP" + _adder] = txtV_IRAP.Text;   
            _node["txtV_1" + _adder] = txtV_1.Text;
            _node["txtV_2" + _adder] = txtV_2.Text;
            _node["txtV_3" + _adder] = txtV_3.Text;    
            _node["txtV_4" + _adder] = txtV_4.Text;
            _node["txtV_5" + _adder] = txtV_5.Text; 
            _node["txtP_IRES" + _adder] = txtP_IRES.Text;
            _node["txtP_IRAP" + _adder] = txtP_IRAP.Text;
            _node["txtP_1" + _adder] = txtP_1.Text;
            _node["txtP_2" + _adder] = txtP_2.Text;
            _node["txtP_3" + _adder] = txtP_3.Text;
            _node["txt1" + _adder] = txt1.Text;
            _node["txt2" + _adder] = txt2.Text;
            _node["txt3" + _adder] = txt3.Text;
            _node["txt4" + _adder] = txt4.Text;    
            _node["txt5" + _adder] = txt5.Text;
            e.Handled = true;
            this.Close();
       
        }

        private void txt_TextChanged(object sender, RoutedEventArgs e)
        {
             ((TextBox)sender).Text = ConvertNumberNoDecimal(((TextBox)sender).Text);
            CalcolaValori();
        }

        private void txtP_TextChanged(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Text = cBusinessObjects.ConvertNumber(((TextBox)sender).Text);
        }
        
    }
}
