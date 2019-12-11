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

namespace UserControls
{
    public partial class ucExcel_BilancioAbbreviatoIndici : UserControl
    {
        private string down = "./Images/icone/navigate_down.png";
        private string left = "./Images/icone/navigate_left.png";

		private string IDB_Padre = "229";

		Hashtable valoreEA = new Hashtable();
		Hashtable valoreEP = new Hashtable();

		Hashtable SommeDaExcel = new Hashtable();
		Hashtable ValoriDaExcelEA = new Hashtable();
		Hashtable ValoriDaExcelEP = new Hashtable();

		public ucExcel_BilancioAbbreviatoIndici()
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

		public void Load(ref XmlDataProviderManager _x, XmlDataProviderManager x_AP, string _ID)
        {
			#region Dati da bilancio

			RetrieveData(_x, x_AP, IDB_Padre);

			#endregion

			SommeDaExcel.Add("B10", "89|54|59|98|99|3|4|32|80");
			ValoriDaExcelEA.Add("B10", GetValoreEA("B10"));
			ValoriDaExcelEP.Add("B10", GetValoreEP("B10"));

			SommeDaExcel.Add("B13", "50");
			ValoriDaExcelEA.Add("B13", GetValoreEA("B13"));
			ValoriDaExcelEP.Add("B13", GetValoreEP("B13"));

			SommeDaExcel.Add("B21", "16|7|23|33|60|53");
			ValoriDaExcelEA.Add("B21", GetValoreEA("B21"));
			ValoriDaExcelEP.Add("B21", GetValoreEP("B21"));

			SommeDaExcel.Add("B31", "133|176|177");
			ValoriDaExcelEA.Add("B31", GetValoreEA("B31"));
			ValoriDaExcelEP.Add("B31", GetValoreEP("B31"));

			SommeDaExcel.Add("B37", "123|129|134");
			ValoriDaExcelEA.Add("B37", GetValoreEA("B37"));
			ValoriDaExcelEP.Add("B37", GetValoreEP("B37"));

			SommeDaExcel.Add("B45", "108|108|109|110|111|112|113|115|116|117|118|119|120|121|-120|-108|189|190|192|194|195|-198|-212|-199|-208|-202|-203|-204|-200|-209|-213|-214|-215|222|223|224|227|228|229|230|231|232|234|235|236|237|-239|-240|-241|-242|243|239|240|241|242|-239|-240|-241|-242|246|-250|256|-259|-266");
			ValoriDaExcelEA.Add("B45", GetValoreEA("B45"));
			ValoriDaExcelEP.Add("B45", GetValoreEP("B45"));
			
			SommeDaExcel.Add("B53", "189|190|192|194|195");
			ValoriDaExcelEA.Add("B53", GetValoreEA("B53"));
			ValoriDaExcelEP.Add("B53", GetValoreEP("B53"));

			SommeDaExcel.Add("B63", "189|190|192|194|195|-198|-212|-199|-208|-202|-203|-204|-200|-209|-213|-214|-215");
			ValoriDaExcelEA.Add("B63", GetValoreEA("B63"));
			ValoriDaExcelEP.Add("B63", GetValoreEP("B63"));

			SommeDaExcel.Add("B68", "222|223|224|227|228|229|230|231|232|234|235|236|237|-239|-240|-241|-242|243|239|240|241|242|-239|-240|-241|-242");
			ValoriDaExcelEA.Add("B68", GetValoreEA("B68"));
			ValoriDaExcelEP.Add("B68", GetValoreEP("B68"));

			SommeDaExcel.Add("B77", "189|190|192|194|195|-198|-212|-199|-208|-202|-203|-204|-200|-209|-213|-214|-215|222|223|224|227|228|229|230|231|232|234|235|236|237|-239|-240|-241|-242|243|239|240|241|242|-239|-240|-241|-242|246|-250|256|-259|-266");
			ValoriDaExcelEA.Add("B77", GetValoreEA("B77"));
			ValoriDaExcelEP.Add("B77", GetValoreEP("B77"));

			txtEA_1.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEA["B31"]) == 0.0) ? "" : ((double)(ValoriDaExcelEA["B10"]) / (double)(ValoriDaExcelEA["B31"])).ToString());
			txtEP_1.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEP["B31"]) == 0.0) ? "" : ((double)(ValoriDaExcelEP["B10"]) / (double)(ValoriDaExcelEP["B31"])).ToString());

			txtEA_2.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEA["B31"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B10"]) + (double)(ValoriDaExcelEA["B13"])) / (double)(ValoriDaExcelEA["B31"])).ToString());
			txtEP_2.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEP["B31"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B10"]) + (double)(ValoriDaExcelEP["B13"])) / (double)(ValoriDaExcelEP["B31"])).ToString());

			txtEA_3.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEA["B45"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B31"]) + (double)(ValoriDaExcelEA["B37"])) / (double)(ValoriDaExcelEA["B45"])).ToString());
			txtEP_3.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEP["B45"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B31"]) + (double)(ValoriDaExcelEP["B37"])) / (double)(ValoriDaExcelEP["B45"])).ToString());
			
			txtEA_4.Text = cBusinessObjects.ConvertNumber((((double)(ValoriDaExcelEA["B31"]) + (double)(ValoriDaExcelEA["B37"])) == 0.0) ? "" : ((double)(ValoriDaExcelEA["B45"]) / ((double)(ValoriDaExcelEA["B31"]) + (double)(ValoriDaExcelEA["B37"]))).ToString());
			txtEP_4.Text = cBusinessObjects.ConvertNumber((((double)(ValoriDaExcelEP["B31"]) + (double)(ValoriDaExcelEP["B37"])) == 0.0) ? "" : ((double)(ValoriDaExcelEP["B45"]) / ((double)(ValoriDaExcelEP["B31"]) + (double)(ValoriDaExcelEP["B37"]))).ToString());
						
			txtEA_5.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEA["B21"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B45"])) / (double)(ValoriDaExcelEA["B21"])).ToString());
			txtEP_5.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEP["B21"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B45"])) / (double)(ValoriDaExcelEP["B21"])).ToString());

			txtEA_6.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEA["B21"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B37"]) + (double)(ValoriDaExcelEA["B45"])) / (double)(ValoriDaExcelEA["B21"])).ToString());
			txtEP_6.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEP["B21"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B37"]) + (double)(ValoriDaExcelEP["B45"])) / (double)(ValoriDaExcelEP["B21"])).ToString());

			txtEA_7.Text = cBusinessObjects.ConvertNumber((((double)(ValoriDaExcelEA["B13"]) + (double)(ValoriDaExcelEA["B10"]) - (double)(ValoriDaExcelEA["B31"])) == 0.0) ? "" : ((double)(ValoriDaExcelEA["B63"]) * 100.0 / ((double)(ValoriDaExcelEA["B13"]) + (double)(ValoriDaExcelEA["B10"]) - (double)(ValoriDaExcelEA["B31"]))).ToString()) + "%";
			txtEP_7.Text = cBusinessObjects.ConvertNumber((((double)(ValoriDaExcelEP["B13"]) + (double)(ValoriDaExcelEP["B10"]) - (double)(ValoriDaExcelEP["B31"])) == 0.0) ? "" : ((double)(ValoriDaExcelEP["B63"]) * 100.0 / ((double)(ValoriDaExcelEP["B13"]) + (double)(ValoriDaExcelEP["B10"]) - (double)(ValoriDaExcelEP["B31"]))).ToString()) + "%";
						
			txtEA_8.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEA["B45"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B77"])) * 100.0 / (double)(ValoriDaExcelEA["B45"])).ToString()) + "%";
			txtEP_8.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEP["B45"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B77"])) * 100.0 / (double)(ValoriDaExcelEP["B45"])).ToString()) + "%";
			
			txtEA_9.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEA["B53"]) == 0.0) ? "" : (((double)(ValoriDaExcelEA["B63"])) * 100.0 / (double)(ValoriDaExcelEA["B53"])).ToString()) + "%";
			txtEP_9.Text = cBusinessObjects.ConvertNumber(((double)(ValoriDaExcelEP["B53"]) == 0.0) ? "" : (((double)(ValoriDaExcelEP["B63"])) * 100.0 / (double)(ValoriDaExcelEP["B53"])).ToString()) + "%";

			if (((double)(ValoriDaExcelEA["B63"])) <= 0.0)
			{
				txtEA_10.Text = "n.c.";
			}
			else //if (((double)(ValoriDaExcelEA["B72"])) / -((double)(ValoriDaExcelEA["B77"])) > 1.0)
			{
				txtEA_10.Text = cBusinessObjects.ConvertNumber((((double)(ValoriDaExcelEA["B63"])) / -((double)(ValoriDaExcelEA["B68"]))).ToString());
			}

			if (((double)(ValoriDaExcelEP["B63"])) <= 0.0)
			{
				txtEP_10.Text = "n.c.";
			}
			else
			{
				txtEP_10.Text = cBusinessObjects.ConvertNumber((((double)(ValoriDaExcelEP["B63"])) / -((double)(ValoriDaExcelEP["B68"]))).ToString());
			}

			XmlNode xnode = _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]");

			if (xnode.Attributes["txtEA_1"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_1");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEA_1"].Value = txtEA_1.Text;

			if (xnode.Attributes["txtEP_1"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_1");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEP_1"].Value = txtEP_1.Text;

			if (xnode.Attributes["txtEA_2"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_2");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEA_2"].Value = txtEA_2.Text;

			if (xnode.Attributes["txtEP_2"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_2");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEP_2"].Value = txtEP_2.Text;

			if (xnode.Attributes["txtEA_3"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_3");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEA_3"].Value = txtEA_3.Text;

			if (xnode.Attributes["txtEP_3"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_3");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEP_3"].Value = txtEP_3.Text;

			if (xnode.Attributes["txtEA_4"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_4");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEA_4"].Value = txtEA_4.Text;

			if (xnode.Attributes["txtEP_4"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_4");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEP_4"].Value = txtEP_4.Text;

			if (xnode.Attributes["txtEA_5"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_5");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEA_5"].Value = txtEA_5.Text;

			if (xnode.Attributes["txtEP_5"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_5");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEP_5"].Value = txtEP_5.Text;

			if (xnode.Attributes["txtEA_6"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_6");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEA_6"].Value = txtEA_6.Text;

			if (xnode.Attributes["txtEP_6"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_6");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEP_6"].Value = txtEP_6.Text;

			if (xnode.Attributes["txtEA_7"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_7");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEA_7"].Value = txtEA_7.Text;

			if (xnode.Attributes["txtEP_7"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_7");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEP_7"].Value = txtEP_7.Text;

			if (xnode.Attributes["txtEA_8"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_8");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEA_8"].Value = txtEA_8.Text;

			if (xnode.Attributes["txtEP_8"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_8");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEP_8"].Value = txtEP_8.Text;

			if (xnode.Attributes["txtEA_9"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_9");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEA_9"].Value = txtEA_9.Text;

			if (xnode.Attributes["txtEP_9"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_9");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEP_9"].Value = txtEP_9.Text;

			if (xnode.Attributes["txtEA_10"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEA_10");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEA_10"].Value = txtEA_10.Text;

			if (xnode.Attributes["txtEP_10"] == null)
			{
				XmlAttribute attr = _x.Document.CreateAttribute("txtEP_10");
				xnode.Attributes.Append(attr);
			}

			xnode.Attributes["txtEP_10"].Value = txtEP_10.Text;

			_x.Save();
        }

        private double GetValoreEA(string Cella)
        {
            double returnvalue = 0.0;

            if (SommeDaExcel.Contains(Cella))
            {
                foreach (string ID in SommeDaExcel[Cella].ToString().Split('|'))
                {
                    bool negativo = false;
                    int intero;
                    int.TryParse(ID, out intero);

                    if (intero < 0)
                    {
                        negativo = true;
                        intero = -intero;
                    }

                    double dblValore = 0.0;

                    if (valoreEA.Contains(intero.ToString()))
                    {
                        double.TryParse(valoreEA[intero.ToString()].ToString(), out dblValore);
                    }

                    if (negativo)
                    {
                        returnvalue -= dblValore;
                    }
                    else
                    {
                        returnvalue += dblValore;
                    }
                }
            }

            return returnvalue;
        }

        private double GetValoreEP(string Cella)
        {
            double returnvalue = 0.0;

            if (SommeDaExcel.Contains(Cella))
            {
                foreach (string ID in SommeDaExcel[Cella].ToString().Split('|'))
                {
                    bool negativo = false;
                    int intero;
                    int.TryParse(ID, out intero);

                    if (intero < 0)
                    {
                        negativo = true;
                        intero = -intero;
                    }

                    double dblValore = 0.0;

                    if (valoreEP.Contains(intero.ToString()))
                    {
                        double.TryParse(valoreEP[intero.ToString()].ToString(), out dblValore);
                    }

                    if (negativo)
                    {
                        returnvalue -= dblValore;
                    }
                    else
                    {
                        returnvalue += dblValore;
                    }
                }
            }

            return returnvalue;
        }

		private void RetrieveData(XmlDataProviderManager _x, XmlDataProviderManager x_AP, string ID)
		{
			foreach (XmlNode node in _x.Document.SelectNodes("/Dati//Dato[@ID='" + ID + "']/Valore"))
			{
				//Calcolo valori attuali

				if (node.Attributes["EA"] != null)
				{
                    if ( !valoreEA.Contains( node.Attributes["ID"].Value ) )
                    {
                        valoreEA.Add( node.Attributes["ID"].Value, node.Attributes["EA"].Value );
                    }
				}
				else
				{
                    if ( !valoreEA.Contains( node.Attributes["ID"].Value ) )
                    {
                        valoreEA.Add( node.Attributes["ID"].Value, "0" );
                    }
				}

				if (x_AP == null || (x_AP != null && x_AP.Document.SelectSingleNode("/Dati//Dato[@ID='" + ID + "']/Valore[@ID='" + node.Attributes["ID"].Value + "']") == null))
				{
					if (node.Attributes["EP"] != null)
					{
                        if ( !valoreEP.Contains( node.Attributes["ID"].Value ) )
                        {
                            valoreEP.Add( node.Attributes["ID"].Value, node.Attributes["EP"].Value );
                        }
					}
					else
					{
                        if ( !valoreEP.Contains( node.Attributes["ID"].Value ) )
                        {
                            valoreEP.Add( node.Attributes["ID"].Value, "0" );
                        }
					}
				}

				//Calcolo valori anno precedente
				if (x_AP != null)
				{
					XmlNode tmpNode = x_AP.Document.SelectSingleNode("/Dati//Dato[@ID='" + ID + "']/Valore[@ID='" + node.Attributes["ID"].Value + "']");
					if (tmpNode != null)
					{
						if (tmpNode.Attributes["EA"] != null)
						{
                            if ( !valoreEP.Contains( node.Attributes["ID"].Value ) )
                            {
                                valoreEP.Add( node.Attributes["ID"].Value, tmpNode.Attributes["EA"].Value );
                            }
						}
						else
						{
                            if ( !valoreEP.Contains( node.Attributes["ID"].Value ) )
                            {
                                valoreEP.Add( node.Attributes["ID"].Value, "0" );
                            }
						}
					}
					else
					{
                        if ( !valoreEP.Contains( node.Attributes["ID"].Value ) )
                        {
                            valoreEP.Add( node.Attributes["ID"].Value, "0" );
                        }
					}
				}
			}
		}

		

		private string ConvertInteger(string valore)
		{
			double dblValore = 0.0;

			double.TryParse(valore, out dblValore);

			if (dblValore == 0.0)
			{
				return "";
			}
			else
			{
				return String.Format("{0:#,0}", dblValore);
			}
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double newsize = e.NewSize.Width - 30.0;

			try
			{				
				foreach (UIElement item in stack.Children)
				{
					((UserControl)(((Grid)(((Border)(item)).Child)).Children[2])).Width = newsize - 30;
				}

				stack.Width = Convert.ToDouble(newsize);
			}
			catch (Exception ex)
			{
				string log = ex.Message;
			}
		}

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image i = ((Image)sender);
            UserControl u = ((UserControl)(((Grid)(i.Parent)).Children[2]));

            if (u.Visibility == System.Windows.Visibility.Collapsed)
            {
                u.Visibility = System.Windows.Visibility.Visible;
                var uriSource = new Uri(down, UriKind.Relative);
                i.Source = new BitmapImage(uriSource);
            }
            else
            {
                u.Visibility = System.Windows.Visibility.Collapsed;
                var uriSource = new Uri(left, UriKind.Relative);
                i.Source = new BitmapImage(uriSource);
            }
        }
    }
}
