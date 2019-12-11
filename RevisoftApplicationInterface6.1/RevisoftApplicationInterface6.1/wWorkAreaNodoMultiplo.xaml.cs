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
using UserControls;

namespace RevisoftApplication
{
    public partial class WindowWorkAreaNodoMultiplo : Window
    {
        private XmlDataProviderManager _x;

        public WindowWorkAreaNodoMultiplo()
        {
            InitializeComponent();            
        }

        public bool ReadOnly
        {
            set
            {
                NodoMultiplo.ReadOnly = value;
            }
        }

		public void LoadDataSource(ref XmlDataProviderManager x, string ID, string tab, XmlNodeList xnl, Hashtable Sessioni, int SessioneNow, string _IDTree)
        {
            _x = x;

            NodoMultiplo.Load( ref _x, ID, tab, xnl, Sessioni, SessioneNow, _IDTree, new Hashtable(), new Hashtable(), "", "" );
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _x.Save();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
			try
			{
				NodoMultiplo.Width = e.NewSize.Width - 30;
				//NodoMultiplo.UserControl_SizeChanged(sender, e);   
			}
			catch (Exception ex)
			{
                cBusinessObjects.logger.Error(ex, "wWorkAreaNodoMultiplo.Window_SizeChanged exception");
                string log = ex.Message;
			}
        }
    }
}
