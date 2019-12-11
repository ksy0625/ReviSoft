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
    public partial class WindowWorkAreaTabella : Window
    {
        private XmlDataProviderManager _x;

        public WindowWorkAreaTabella()
        {
            InitializeComponent();            
        }

		public void LoadDataSource(ref XmlDataProviderManager x, string ID, string _IDTree,string IDCliente,string IDSessione)
        {
            _x = x;

            Tabella.Load(ID, "", _IDTree, "", IDCliente, IDSessione);
        }

        public bool ReadOnly
        {
            set
            {
                Tabella.ReadOnly = value;
            }
        }
    }
}
