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
    public partial class WindowWorkAreaTabellaReplicata : Window
    {
        private XmlDataProviderManager _x;

        public WindowWorkAreaTabellaReplicata()
        {
            InitializeComponent();            
        }

        public bool ReadOnly
        {
            set
            {
                TabellaReplicata.ReadOnly = value;
            }
        }

		public void LoadDataSource(ref XmlDataProviderManager x, string ID, string tab, string _IDTree,string IDCliente,string IDSessione)
        {
            _x = x;

            TabellaReplicata.Load(ID, tab, _IDTree, IDCliente, IDSessione);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _x.Save();
        }
    }
}
