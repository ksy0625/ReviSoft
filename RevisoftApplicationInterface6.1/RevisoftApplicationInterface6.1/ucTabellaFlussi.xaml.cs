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
    public partial class ucTabellaFlussi : UserControl
    {
        private XmlDataProviderManager _x;

		private bool _ReadOnly = false;

		public string XPath = "";
		public ArrayList OldXPath = new ArrayList();

        public ucTabellaFlussi()
        {
            InitializeComponent();   
        }
		
        public bool ReadOnly
        {
            set
            {
		      		_ReadOnly = value;
            }
        }

        public void Load( XmlDataProviderManager x, string xpath )
        {
            _x = x;

            Binding b = new Binding();
            b.Source = x.Document;
            b.XPath = xpath;           

			XPath = b.XPath;

            dtgMain.SetBinding(ItemsControl.ItemsSourceProperty, b);
        }

		public XmlDataProviderManager Save()
		{
			_x.Save();

			return _x;
		}

        private void DataGrid_SourceUpdated(object sender, DataTransferEventArgs e)
        {
			;
        }

        private void dtgMain_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            ;
            //if ( dtgMain.SelectedItem != null )
            //{
            //    dtgMain.UnselectAll();
            //}

        }
        
    }
}
