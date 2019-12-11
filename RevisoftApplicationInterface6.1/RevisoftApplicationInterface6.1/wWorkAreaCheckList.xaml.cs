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

namespace RevisoftApplication
{
    public partial class WindowWorkAreaCheckList : Window
    {
        private XmlDataProviderManager _x;

        public WindowWorkAreaCheckList()
        {   
            InitializeComponent();
        }

        public bool ReadOnly
        {
            set
            {
                CheckList.ReadOnly = value;                
            }
        }

        public void LoadDataSource(ref XmlDataProviderManager x, string ID)
        {
            _x = x;
            
            CheckList.Load( ID, cBusinessObjects.idcliente.ToString(), cBusinessObjects.idsessione.ToString());
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width != 0 && e.PreviousSize.Height != 0)
            {
                double wd = e.NewSize.Width - e.PreviousSize.Width;
                double hd = e.NewSize.Height - e.PreviousSize.Height;
                CheckList.Width = CheckList.Width + wd;
                CheckList.Height = CheckList.Height + hd;
            }

            //CheckList.Window_SizeChanged(sender, e);
        }

        private void Window_ItemSizeChanged(object sender, RoutedEventArgs e)
        {
            //CheckList.Resizer(Convert.ToInt32(this.Width));
        }

        private void Window_LayoutUpdated(object sender, EventArgs e)
        {
            //CheckList.Resizer(Convert.ToInt32(this.Width));
        }
    }
}
