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
    public partial class WindowWorkAreaCheckListPlus : Window
    {
        //private XmlDataProviderManager _x;

        public WindowWorkAreaCheckListPlus()
        {   
            InitializeComponent();
        }

        public bool ReadOnly
        {
            set
            {
                CheckListPlus.ReadOnly = value;
            }
        }

        public void LoadDataSource( string ID, string IDCliente, string IDSessione)
        {
           
            
            CheckListPlus.Load(ID, IDCliente, IDSessione);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Width != 0 && e.PreviousSize.Height != 0)
            {
                double wd = e.NewSize.Width - e.PreviousSize.Width;
                double hd = e.NewSize.Height - e.PreviousSize.Height;
                CheckListPlus.Width = CheckListPlus.Width + wd;
                CheckListPlus.Height = CheckListPlus.Height + hd;
            }

            //CheckListPlus.Window_SizeChanged(sender, e);
        }

        private void Window_ItemSizeChanged(object sender, RoutedEventArgs e)
        {
            CheckListPlus.Resizer(Convert.ToInt32(this.Width));
        }

        private void Window_LayoutUpdated(object sender, EventArgs e)
        {
            CheckListPlus.Resizer(Convert.ToInt32(this.Width));
        }
    }
}
